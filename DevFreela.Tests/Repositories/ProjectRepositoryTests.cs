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
}
