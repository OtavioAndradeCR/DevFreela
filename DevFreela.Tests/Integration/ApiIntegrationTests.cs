using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace DevFreela.Tests.Integration;

public sealed class ApiIntegrationTests : IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ApiIntegrationTests()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetProjects_WhenNoProjectsExist_ShouldReturnOkAndEmptyList()
    {
        var response = await _client.GetAsync("/api/projects");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var projects = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
        Assert.NotNull(projects);
        Assert.Empty(projects);
    }

    [Fact]
    public async Task GetProjects_WhenQueryMatches_ShouldReturnOnlyMatchingProjects()
    {
        var clientId = await CreateUserAsync("Cliente");
        var freelancerId = await CreateUserAsync("Freelancer");

        await CreateProjectAsync(clientId, freelancerId, "Sistema financeiro");
        await CreateProjectAsync(clientId, freelancerId, "Portal institucional");

        var response = await _client.GetAsync("/api/projects?query=financeiro");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var projects = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
        Assert.NotNull(projects);
        var project = Assert.Single(projects);
        Assert.Equal("Sistema financeiro", project.GetProperty("title").GetString());
    }

    [Fact]
    public async Task GetProjects_WhenQueryDoesNotMatch_ShouldReturnEmptyList()
    {
        var clientId = await CreateUserAsync("Cliente");
        var freelancerId = await CreateUserAsync("Freelancer");

        await CreateProjectAsync(clientId, freelancerId, "Sistema financeiro");

        var response = await _client.GetAsync("/api/projects?query=inexistente");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var projects = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
        Assert.NotNull(projects);
        Assert.Empty(projects);
    }

    [Fact]
    public async Task GetUser_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync("/api/users/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_WhenInputIsValid_ShouldReturnCreatedAndAllowRetrieval()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/users", new
        {
            FullName = "Usuário de integração",
            Email = "integracao@example.com",
            BirthDate = new DateTime(1995, 5, 20)
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var userId = GetIdFromLocation(createResponse);
        var getResponse = await _client.GetAsync($"/api/users/{userId}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var user = await getResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Usuário de integração", user.GetProperty("fullName").GetString());
        Assert.Equal("integracao@example.com", user.GetProperty("email").GetString());
    }

    [Fact]
    public async Task CreateUser_WhenEmailIsInvalid_ShouldReturnBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/users", new
        {
            FullName = "Usuário inválido",
            Email = "email-invalido",
            BirthDate = new DateTime(1995, 5, 20)
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProject_WhenProjectDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync("/api/projects/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ProjectLifecycle_WhenCreated_ShouldAllowStartAndFinish()
    {
        var clientId = await CreateUserAsync("Cliente");
        var freelancerId = await CreateUserAsync("Freelancer");
        var projectId = await CreateProjectAsync(clientId, freelancerId);

        var getResponse = await _client.GetAsync($"/api/projects/{projectId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var startResponse = await _client.PutAsync($"/api/projects/{projectId}/start", null);
        Assert.Equal(HttpStatusCode.NoContent, startResponse.StatusCode);

        var finishResponse = await _client.PutAsync($"/api/projects/{projectId}/finish", null);
        Assert.Equal(HttpStatusCode.NoContent, finishResponse.StatusCode);

        var invalidStartResponse = await _client.PutAsync($"/api/projects/{projectId}/start", null);
        Assert.Equal(HttpStatusCode.Conflict, invalidStartResponse.StatusCode);
    }

    [Fact]
    public async Task AddComment_WhenProjectAndUserExist_ShouldReturnNoContent()
    {
        var clientId = await CreateUserAsync("Cliente");
        var freelancerId = await CreateUserAsync("Freelancer");
        var projectId = await CreateProjectAsync(clientId, freelancerId);

        var response = await _client.PostAsJsonAsync($"/api/projects/{projectId}/comments", new
        {
            Content = "Comentário criado no teste de integração.",
            IdUser = clientId
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task GetSkills_WhenNoSkillsExist_ShouldReturnOkAndEmptyList()
    {
        var response = await _client.GetAsync("/api/skills");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var skills = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
        Assert.NotNull(skills);
        Assert.Empty(skills);
    }

    [Fact]
    public async Task UpdateProject_WhenProjectExists_ShouldReturnNoContentAndPersistChanges()
    {
        var clientId = await CreateUserAsync("Cliente");
        var freelancerId = await CreateUserAsync("Freelancer");
        var projectId = await CreateProjectAsync(clientId, freelancerId);

        var updateResponse = await _client.PutAsJsonAsync($"/api/projects/{projectId}", new
        {
            Title = "Projeto atualizado",
            Description = "Descrição atualizada pelo teste de integração.",
            TotalCost = 2500m
        });

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/projects/{projectId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var project = await getResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Projeto atualizado", project.GetProperty("title").GetString());
        Assert.Equal("Descrição atualizada pelo teste de integração.", project.GetProperty("description").GetString());
        Assert.Equal(2500m, project.GetProperty("totalCost").GetDecimal());
    }

    [Fact]
    public async Task UpdateProject_WhenProjectDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.PutAsJsonAsync("/api/projects/999", new
        {
            Title = "Projeto atualizado",
            Description = "Descrição atualizada.",
            TotalCost = 2500m
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CancelProject_WhenProjectCanBeCancelled_ShouldReturnNoContent()
    {
        var clientId = await CreateUserAsync("Cliente");
        var freelancerId = await CreateUserAsync("Freelancer");
        var projectId = await CreateProjectAsync(clientId, freelancerId);

        var response = await _client.PutAsync($"/api/projects/{projectId}/cancel", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task CancelProject_WhenProjectIsFinished_ShouldReturnConflict()
    {
        var clientId = await CreateUserAsync("Cliente");
        var freelancerId = await CreateUserAsync("Freelancer");
        var projectId = await CreateProjectAsync(clientId, freelancerId);

        var startResponse = await _client.PutAsync($"/api/projects/{projectId}/start", null);
        Assert.Equal(HttpStatusCode.NoContent, startResponse.StatusCode);

        var finishResponse = await _client.PutAsync($"/api/projects/{projectId}/finish", null);
        Assert.Equal(HttpStatusCode.NoContent, finishResponse.StatusCode);

        var cancelResponse = await _client.PutAsync($"/api/projects/{projectId}/cancel", null);

        Assert.Equal(HttpStatusCode.Conflict, cancelResponse.StatusCode);
    }

    [Fact]
    public async Task CancelProject_WhenProjectDoesNotExist_ShouldReturnNotFound()
    {
        var response = await _client.PutAsync("/api/projects/999/cancel", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<int> CreateUserAsync(string role)
    {
        var response = await _client.PostAsJsonAsync("/api/users", new
        {
            FullName = $"Usuário {role}",
            Email = $"{role.ToLowerInvariant()}-{Guid.NewGuid():N}@example.com",
            BirthDate = new DateTime(1990, 1, 1)
        });

        response.EnsureSuccessStatusCode();
        return GetIdFromLocation(response);
    }

    private async Task<int> CreateProjectAsync(int clientId, int freelancerId, string title = "Projeto de integração")
    {
        var response = await _client.PostAsJsonAsync("/api/projects", new
        {
            Title = title,
            Description = "Projeto criado durante o teste de integração.",
            IdClient = clientId,
            IdFreelancer = freelancerId,
            TotalCost = 1500m
        });

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return GetIdFromLocation(response);
    }

    private static int GetIdFromLocation(HttpResponseMessage response)
    {
        var location = response.Headers.Location;
        Assert.NotNull(location);

        var lastSegment = location.OriginalString.TrimEnd('/').Split('/').Last();
        return int.Parse(lastSegment);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
