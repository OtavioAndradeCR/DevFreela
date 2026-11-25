using DevFreela.Application.Queries.GetAllProjects;
using DevFreela.Core.Entities;
using DevFreela.Core.Repositories;
using Xunit;

namespace DevFreela.Tests.Handlers;

public class GetAllProjectsQueryHandlerTests
{
    private sealed class FakeProjectRepository(List<Project> projects) : IProjectRepository
    {
        public Task<List<Project>> GetAllProjectsAsync() => Task.FromResult(projects);

        public Task<Project> GetProjectByIdAsync(int id) =>
            Task.FromResult(projects.SingleOrDefault(project => project.Id == id));
    }

    private static List<Project> CreateProjects() =>
    [
        new Project("Website Institucional", "Website", 1, 2, 1000m),
        new Project("Aplicativo Mobile", "App", 1, 2, 2000m),
        new Project("Sistema Financeiro", "Sistema", 1, 2, 3000m)
    ];

    [Fact]
    public async Task Handle_WhenQueryIsNull_ShouldReturnAllProjects()
    {
        var projects = CreateProjects();
        var handler = new GetAllProjectsQueryHandler(new FakeProjectRepository(projects));

        var result = await handler.Handle(new GetAllProjectsQuery(null), CancellationToken.None);

        Assert.Equal(3, result.Count);
        Assert.Contains(result, item => item.Title == "Website Institucional");
        Assert.Contains(result, item => item.Title == "Aplicativo Mobile");
        Assert.Contains(result, item => item.Title == "Sistema Financeiro");
    }

    [Fact]
    public async Task Handle_WhenQueryMatchesIgnoringCase_ShouldReturnMatchingProjects()
    {
        var handler = new GetAllProjectsQueryHandler(new FakeProjectRepository(CreateProjects()));

        var result = await handler.Handle(new GetAllProjectsQuery("website"), CancellationToken.None);

        var project = Assert.Single(result);
        Assert.Equal("Website Institucional", project.Title);
    }

    [Fact]
    public async Task Handle_WhenQueryDoesNotMatch_ShouldReturnEmptyList()
    {
        var handler = new GetAllProjectsQueryHandler(new FakeProjectRepository(CreateProjects()));

        var result = await handler.Handle(new GetAllProjectsQuery("inexistente"), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WhenQueryIsWhitespace_ShouldReturnAllProjects()
    {
        var projects = CreateProjects();
        var handler = new GetAllProjectsQueryHandler(new FakeProjectRepository(projects));

        var result = await handler.Handle(new GetAllProjectsQuery("   "), CancellationToken.None);

        Assert.Equal(projects.Count, result.Count);
    }
}
