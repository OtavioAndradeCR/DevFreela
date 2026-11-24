using DevFreela.Application.Commands.CreateComment;
using DevFreela.Application.Commands.CreateProject;
using DevFreela.Application.Commands.Delete;
using DevFreela.Application.Commands.UpdateProject;
using DevFreela.Application.Persistence;
using DevFreela.Core.Entities;
using DevFreela.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DevFreela.Tests.Handlers;

public class ProjectCommandHandlerTests
{
    private static TestDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestDbContext(options);
    }

    [Fact]
    public async Task CreateProject_ShouldPersistProjectAndReturnGeneratedId()
    {
        // Arrange
        await using var context = CreateContext();
        var handler = new CreateProjectCommandHandler(context);
        var command = new CreateProjectCommand
        {
            Title = "Projeto de teste",
            Description = "Descrição para o projeto",
            IdClient = 1,
            IdFreelancer = 2,
            TotalCost = 2500m
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var project = await context.Projects.SingleAsync();
        Assert.Equal(project.Id, result);
        Assert.True(result > 0);
        Assert.Equal(command.Title, project.Title);
        Assert.Equal(ProjectStatusEnum.Created, project.Status);
    }

    [Fact]
    public async Task UpdateProject_WhenProjectExists_ShouldUpdateAndReturnTrue()
    {
        // Arrange
        await using var context = CreateContext();
        var project = new Project("Título antigo", "Descrição antiga", 1, 2, 100m);
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var handler = new UpdateProjectCommandhandler(context);
        var command = new UpdateProjectCommand
        {
            Id = project.Id,
            Title = "Título atualizado",
            Description = "Descrição atualizada",
            TotalCost = 350m
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedProject = await context.Projects.SingleAsync();
        Assert.True(result);
        Assert.Equal("Título atualizado", updatedProject.Title);
        Assert.Equal("Descrição atualizada", updatedProject.Description);
        Assert.Equal(350m, updatedProject.TotalCost);
    }

    [Fact]
    public async Task UpdateProject_WhenProjectDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        await using var context = CreateContext();
        var handler = new UpdateProjectCommandhandler(context);
        var command = new UpdateProjectCommand
        {
            Id = 999,
            Title = "Título atualizado",
            Description = "Descrição atualizada",
            TotalCost = 350m
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteProject_WhenProjectCanBeCancelled_ShouldCancelAndReturnTrue()
    {
        // Arrange
        await using var context = CreateContext();
        var project = new Project("Projeto", "Descrição", 1, 2, 100m);
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var handler = new DeleteProjectCommandHandler(context);

        // Act
        var result = await handler.Handle(new DeleteProjectCommand(project.Id), CancellationToken.None);

        // Assert
        var persistedProject = await context.Projects.SingleAsync();
        Assert.True(result);
        Assert.Equal(ProjectStatusEnum.Cancelled, persistedProject.Status);
    }

    [Fact]
    public async Task DeleteProject_WhenProjectDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        await using var context = CreateContext();
        var handler = new DeleteProjectCommandHandler(context);

        // Act
        var result = await handler.Handle(new DeleteProjectCommand(999), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteProject_WhenProjectCannotBeCancelled_ShouldReturnFalse()
    {
        // Arrange
        await using var context = CreateContext();
        var project = new Project("Projeto", "Descrição", 1, 2, 100m);
        project.Start();
        project.Finish();
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var handler = new DeleteProjectCommandHandler(context);

        // Act
        var result = await handler.Handle(new DeleteProjectCommand(project.Id), CancellationToken.None);

        // Assert
        Assert.False(result);
        Assert.Equal(ProjectStatusEnum.Finished, project.Status);
    }

    [Fact]
    public async Task CreateComment_WhenProjectAndUserExist_ShouldPersistComment()
    {
        // Arrange
        await using var context = CreateContext();
        var project = new Project("Projeto", "Descrição", 1, 2, 100m);
        var user = new User("Usuário de teste", "teste@example.com", new DateTime(1995, 1, 1));
        context.Projects.Add(project);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateCommentCommandHandler(context);
        var command = new CreateCommentCommand
        {
            Content = "Comentário de teste",
            IdProject = project.Id,
            IdUser = user.Id
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        var comment = await context.ProjectComments.SingleAsync();
        Assert.True(result);
        Assert.Equal(command.Content, comment.Content);
        Assert.Equal(project.Id, comment.IdProject);
        Assert.Equal(user.Id, comment.IdUser);
    }

    [Fact]
    public async Task CreateComment_WhenProjectDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        await using var context = CreateContext();
        var user = new User("Usuário de teste", "teste@example.com", new DateTime(1995, 1, 1));
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new CreateCommentCommandHandler(context);
        var command = new CreateCommentCommand
        {
            Content = "Comentário de teste",
            IdProject = 999,
            IdUser = user.Id
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        Assert.Empty(await context.ProjectComments.ToListAsync());
    }

    [Fact]
    public async Task CreateComment_WhenUserDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        await using var context = CreateContext();
        var project = new Project("Projeto", "Descrição", 1, 2, 100m);
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var handler = new CreateCommentCommandHandler(context);
        var command = new CreateCommentCommand
        {
            Content = "Comentário de teste",
            IdProject = project.Id,
            IdUser = 999
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        Assert.Empty(await context.ProjectComments.ToListAsync());
    }
}

internal sealed class TestDbContext : DbContext, IDevFreelaDbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<UserSkill> UserSkills { get; set; }
    public DbSet<ProjectComment> ProjectComments { get; set; }
}
