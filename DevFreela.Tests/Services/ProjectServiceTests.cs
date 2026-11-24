using DevFreela.Application.Services.Implementations;
using DevFreela.Core.Entities;
using DevFreela.Core.Enums;
using DevFreela.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DevFreela.Tests.Services;

public class ProjectServiceTests
{
    private static DevFreelaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DevFreelaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DevFreelaDbContext(options);
    }

    private static async Task<(User Client, User Freelancer)> SeedUsers(DevFreelaDbContext context)
    {
        var client = new User("Cliente de teste", "cliente@example.com", new DateTime(1990, 1, 1));
        var freelancer = new User("Freelancer de teste", "freelancer@example.com", new DateTime(1992, 1, 1));

        context.Users.AddRange(client, freelancer);
        await context.SaveChangesAsync();

        return (client, freelancer);
    }

    [Fact]
    public async Task GetAll_WhenProjectsExist_ShouldReturnAllProjects()
    {
        await using var context = CreateContext();
        var (client, freelancer) = await SeedUsers(context);

        context.Projects.AddRange(
            new Project("Website institucional", "Projeto de website", client.Id, freelancer.Id, 1500m),
            new Project("Aplicativo mobile", "Projeto mobile", client.Id, freelancer.Id, 3000m));
        await context.SaveChangesAsync();

        var service = new ProjectService(context);

        var result = service.GetAll(null);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, project => project.Title == "Website institucional");
        Assert.Contains(result, project => project.Title == "Aplicativo mobile");
    }

    [Fact]
    public async Task GetAll_WhenQueryMatchesTitle_ShouldReturnMatchingProjects()
    {
        await using var context = CreateContext();
        var (client, freelancer) = await SeedUsers(context);

        context.Projects.AddRange(
            new Project("Website institucional", "Projeto de website", client.Id, freelancer.Id, 1500m),
            new Project("Aplicativo mobile", "Projeto mobile", client.Id, freelancer.Id, 3000m));
        await context.SaveChangesAsync();

        var service = new ProjectService(context);

        var result = service.GetAll("Website");

        var project = Assert.Single(result);
        Assert.Equal("Website institucional", project.Title);
    }

    [Fact]
    public async Task GetAll_WhenQueryDoesNotMatch_ShouldReturnEmptyList()
    {
        await using var context = CreateContext();
        var (client, freelancer) = await SeedUsers(context);

        context.Projects.Add(
            new Project("Website institucional", "Projeto de website", client.Id, freelancer.Id, 1500m));
        await context.SaveChangesAsync();

        var service = new ProjectService(context);

        var result = service.GetAll("Inexistente");

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetById_WhenProjectExists_ShouldReturnProjectDetails()
    {
        await using var context = CreateContext();
        var (client, freelancer) = await SeedUsers(context);
        var project = new Project("Website institucional", "Descrição completa", client.Id, freelancer.Id, 2500m);
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var service = new ProjectService(context);

        var result = service.GetById(project.Id);

        Assert.NotNull(result);
        Assert.Equal(project.Id, result.Id);
        Assert.Equal("Website institucional", result.Title);
        Assert.Equal("Descrição completa", result.Description);
        Assert.Equal(2500m, result.TotalCost);
        Assert.Equal("Cliente de teste", result.ClientFullName);
        Assert.Equal("Freelancer de teste", result.FreelancerFullName);
        Assert.Null(result.StartedAt);
        Assert.Null(result.FinishedAt);
    }

    [Fact]
    public async Task GetById_WhenProjectDoesNotExist_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var service = new ProjectService(context);

        var result = service.GetById(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task Start_WhenProjectCanBeStarted_ShouldUpdateStatusAndReturnTrue()
    {
        await using var context = CreateContext();
        var (client, freelancer) = await SeedUsers(context);
        var project = new Project("Projeto", "Descrição", client.Id, freelancer.Id, 100m);
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var service = new ProjectService(context);

        var result = await service.Start(project.Id);

        Assert.True(result);
        var persistedProject = await context.Projects.SingleAsync();
        Assert.Equal(ProjectStatusEnum.InProgress, persistedProject.Status);
        Assert.NotNull(persistedProject.StartedAt);
    }

    [Fact]
    public async Task Start_WhenProjectDoesNotExist_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var service = new ProjectService(context);

        var result = await service.Start(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task Start_WhenProjectCannotBeStarted_ShouldReturnFalse()
    {
        await using var context = CreateContext();
        var (client, freelancer) = await SeedUsers(context);
        var project = new Project("Projeto", "Descrição", client.Id, freelancer.Id, 100m);
        project.Start();
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var service = new ProjectService(context);

        var result = await service.Start(project.Id);

        Assert.False(result);
        Assert.Equal(ProjectStatusEnum.InProgress, project.Status);
    }

    [Fact]
    public async Task Finish_WhenProjectCanBeFinished_ShouldUpdateStatusAndReturnTrue()
    {
        await using var context = CreateContext();
        var (client, freelancer) = await SeedUsers(context);
        var project = new Project("Projeto", "Descrição", client.Id, freelancer.Id, 100m);
        project.Start();
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var service = new ProjectService(context);

        var result = await service.Finish(project.Id);

        Assert.True(result);
        var persistedProject = await context.Projects.SingleAsync();
        Assert.Equal(ProjectStatusEnum.Finished, persistedProject.Status);
        Assert.NotNull(persistedProject.FinishedAt);
    }

    [Fact]
    public async Task Finish_WhenProjectDoesNotExist_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var service = new ProjectService(context);

        var result = await service.Finish(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task Finish_WhenProjectCannotBeFinished_ShouldReturnFalse()
    {
        await using var context = CreateContext();
        var (client, freelancer) = await SeedUsers(context);
        var project = new Project("Projeto", "Descrição", client.Id, freelancer.Id, 100m);
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var service = new ProjectService(context);

        var result = await service.Finish(project.Id);

        Assert.False(result);
        Assert.Equal(ProjectStatusEnum.Created, project.Status);
    }
}
