using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DevFreela.Core.Entities;
using DevFreela.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
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

    [Theory]
    [InlineData("")]
    [InlineData("AB")]
    public async Task CreateUser_WhenFullNameIsInvalid_ShouldReturnBadRequest(string fullName)
    {
        var response = await _client.PostAsJsonAsync("/api/users", new
        {
            FullName = fullName,
            Email = "usuario.valido@example.com",
            BirthDate = new DateTime(1995, 5, 20)
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("", "Descrição válida para o projeto.", 1500)]
    [InlineData("Projeto válido", "AB", 1500)]
    [InlineData("Projeto válido", "Descrição válida para o projeto.", 0)]
    public async Task CreateProject_WhenInputIsInvalid_ShouldReturnBadRequest(
        string title,
        string description,
        decimal totalCost)
    {
        var clientId = await CreateUserAsync("Cliente");
        var freelancerId = await CreateUserAsync("Freelancer");

        var response = await _client.PostAsJsonAsync("/api/projects", new
        {
            Title = title,
            Description = description,
            IdClient = clientId,
            IdFreelancer = freelancerId,
            TotalCost = totalCost
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
    public async Task GetProject_WhenProjectExists_ShouldReturnProjectDetails()
    {
        var clientId = await CreateUserAsync("Cliente");
        var freelancerId = await CreateUserAsync("Freelancer");
        var projectId = await CreateProjectAsync(clientId, freelancerId, "Projeto para consulta");

        var response = await _client.GetAsync($"/api/projects/{projectId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var project = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(projectId, project.GetProperty("id").GetInt32());
        Assert.Equal("Projeto para consulta", project.GetProperty("title").GetString());
        Assert.Equal("Projeto criado durante o teste de integração.", project.GetProperty("description").GetString());
        Assert.Equal(1500m, project.GetProperty("totalCost").GetDecimal());
        Assert.Equal("Usuário Cliente", project.GetProperty("clientFullName").GetString());
        Assert.Equal("Usuário Freelancer", project.GetProperty("freelancerFullName").GetString());
        Assert.Equal(JsonValueKind.Null, project.GetProperty("startedAt").ValueKind);
        Assert.Equal(JsonValueKind.Null, project.GetProperty("finishedAt").ValueKind);
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
    public async Task ProjectLifecycle_WhenProjectIsAlreadyFinished_ShouldReturnConflict()
    {
        var clientId = await CreateUserAsync("Cliente");
        var freelancerId = await CreateUserAsync("Freelancer");
        var projectId = await CreateProjectAsync(clientId, freelancerId);

        var startResponse = await _client.PutAsync($"/api/projects/{projectId}/start", null);
        Assert.Equal(HttpStatusCode.NoContent, startResponse.StatusCode);

        var finishResponse = await _client.PutAsync($"/api/projects/{projectId}/finish", null);
        Assert.Equal(HttpStatusCode.NoContent, finishResponse.StatusCode);

        var repeatedFinishResponse = await _client.PutAsync($"/api/projects/{projectId}/finish", null);

        Assert.Equal(HttpStatusCode.Conflict, repeatedFinishResponse.StatusCode);
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
    public async Task AddComment_WhenProjectDoesNotExist_ShouldReturnNotFound()
    {
        var userId = await CreateUserAsync("Cliente");

        var response = await _client.PostAsJsonAsync("/api/projects/999/comments", new
        {
            Content = "Comentário para projeto inexistente.",
            IdUser = userId
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddComment_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        var clientId = await CreateUserAsync("Cliente");
        var freelancerId = await CreateUserAsync("Freelancer");
        var projectId = await CreateProjectAsync(clientId, freelancerId);

        var response = await _client.PostAsJsonAsync($"/api/projects/{projectId}/comments", new
        {
            Content = "Comentário de usuário inexistente.",
            IdUser = 999
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("start")]
    [InlineData("finish")]
    public async Task ProjectLifecycle_WhenProjectDoesNotExist_ShouldReturnNotFound(string action)
    {
        var response = await _client.PutAsync($"/api/projects/999/{action}", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
    public async Task GetSkills_WhenSkillsExist_ShouldReturnSkillDetails()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DevFreelaDbContext>();
            context.Skills.AddRange(
                new Skill("C#"),
                new Skill("ASP.NET Core"));

            await context.SaveChangesAsync();
        }

        var response = await _client.GetAsync("/api/skills");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var skills = await response.Content.ReadFromJsonAsync<List<JsonElement>>();
        Assert.NotNull(skills);
        Assert.Equal(2, skills.Count);
        Assert.Contains(skills, skill => skill.GetProperty("description").GetString() == "C#");
        Assert.Contains(skills, skill => skill.GetProperty("description").GetString() == "ASP.NET Core");
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

    [Theory]
    [InlineData("", "Descrição válida para atualização.", 2500)]
    [InlineData("AB", "Descrição válida para atualização.", 2500)]
    [InlineData("Projeto válido", "AB", 2500)]
    [InlineData("Projeto válido", "Descrição válida para atualização.", 0)]
    public async Task UpdateProject_WhenInputIsInvalid_ShouldReturnBadRequest(
        string title,
        string description,
        decimal totalCost)
    {
        var clientId = await CreateUserAsync("Cliente");
        var freelancerId = await CreateUserAsync("Freelancer");
        var projectId = await CreateProjectAsync(clientId, freelancerId);

        var response = await _client.PutAsJsonAsync($"/api/projects/{projectId}", new
        {
            Title = title,
            Description = description,
            TotalCost = totalCost
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
