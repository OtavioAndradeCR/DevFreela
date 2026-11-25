using DevFreela.Core.Entities;
using DevFreela.Core.Enums;
using DevFreela.Infrastructure.Persistence;
using DevFreela.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DevFreela.Tests.Repositories;

public class ProjectRepositoryTests
{
    private static DevFreelaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DevFreelaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DevFreelaDbContext(options);
    }

    private static Project CreateProject() =>
        new("Projeto de teste", "Descrição do projeto", 1, 2, 1500m);

    [Fact]
    public async Task CreateProjectAsync_ShouldPersistProject()
    {
        await using var context = CreateContext();
        var repository = new ProjectRepository(context);
        var project = CreateProject();

        await repository.CreateProjectAsync(project);

        var persistedProject = await context.Projects.SingleAsync();
        Assert.Equal(project.Id, persistedProject.Id);
        Assert.Equal(project.Title, persistedProject.Title);
        Assert.Equal(project.Description, persistedProject.Description);
        Assert.Equal(project.TotalCost, persistedProject.TotalCost);
    }

    [Fact]
    public async Task DeleteProjectAsync_WhenProjectExists_ShouldCancelAndPersistStatus()
    {
        await using var context = CreateContext();
        var project = CreateProject();
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var repository = new ProjectRepository(context);

        await repository.DeleteProjectAsync(project.Id);

        var persistedProject = await context.Projects.SingleAsync();
        Assert.Equal(ProjectStatusEnum.Cancelled, persistedProject.Status);
    }

    [Fact]
    public async Task GetProjectByIdAsync_WhenProjectExists_ShouldReturnProjectWithClientAndFreelancer()
    {
        await using var context = CreateContext();
        var client = new User("Cliente de teste", "cliente@example.com", new DateTime(1990, 1, 1));
        var freelancer = new User("Freelancer de teste", "freelancer@example.com", new DateTime(1992, 1, 1));

        context.Users.AddRange(client, freelancer);
        await context.SaveChangesAsync();

        var project = new Project("Projeto de teste", "Descrição do projeto", client.Id, freelancer.Id, 1500m);
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var repository = new ProjectRepository(context);

        var result = await repository.GetProjectByIdAsync(project.Id);

        Assert.NotNull(result);
        Assert.Equal(project.Id, result.Id);
        Assert.Equal("Projeto de teste", result.Title);
        Assert.NotNull(result.Client);
        Assert.Equal("Cliente de teste", result.Client.FullName);
        Assert.NotNull(result.Freelancer);
        Assert.Equal("Freelancer de teste", result.Freelancer.FullName);
    }

    [Fact]
    public async Task GetProjectByIdAsync_WhenProjectDoesNotExist_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var repository = new ProjectRepository(context);

        var result = await repository.GetProjectByIdAsync(999);

        Assert.Null(result);
    }

}
