using DevFreela.Core.Entities;
using DevFreela.Core.Enums;

namespace DevFreela.Tests.Entities;

public class ProjectTests
{
    private static Project CreateProject()
    {
        return new Project(
            "Projeto de teste",
            "Descrição do projeto de teste",
            idClient: 1,
            idFreelancer: 2,
            totalCost: 1500m);
    }

    [Fact]
    public void Constructor_ShouldInitializeProjectWithCreatedStatus()
    {
        // Act
        var project = CreateProject();

        // Assert
        Assert.Equal(ProjectStatusEnum.Created, project.Status);
        Assert.Equal("Projeto de teste", project.Title);
        Assert.Equal("Descrição do projeto de teste", project.Description);
        Assert.Equal(1, project.IdClient);
        Assert.Equal(2, project.IdFreelancer);
        Assert.Equal(1500m, project.TotalCost);
        Assert.Empty(project.Comments);
        Assert.NotEqual(default, project.CreatedAt);
        Assert.Null(project.StartedAt);
        Assert.Null(project.FinishedAt);
    }

    [Fact]
    public void Start_WhenProjectIsCreated_ShouldMoveToInProgress()
    {
        // Arrange
        var project = CreateProject();

        // Act
        var result = project.Start();

        // Assert
        Assert.True(result);
        Assert.Equal(ProjectStatusEnum.InProgress, project.Status);
        Assert.NotNull(project.StartedAt);
    }

    [Fact]
    public void Start_WhenProjectIsNotCreated_ShouldReturnFalse()
    {
        // Arrange
        var project = CreateProject();
        project.Start();

        // Act
        var result = project.Start();

        // Assert
        Assert.False(result);
        Assert.Equal(ProjectStatusEnum.InProgress, project.Status);
    }

    [Fact]
    public void Finish_WhenProjectIsInProgress_ShouldMoveToFinished()
    {
        // Arrange
        var project = CreateProject();
        project.Start();

        // Act
        var result = project.Finish();

        // Assert
        Assert.True(result);
        Assert.Equal(ProjectStatusEnum.Finished, project.Status);
        Assert.NotNull(project.FinishedAt);
    }

    [Fact]
    public void Finish_WhenProjectIsNotInProgress_ShouldReturnFalse()
    {
        // Arrange
        var project = CreateProject();

        // Act
        var result = project.Finish();

        // Assert
        Assert.False(result);
        Assert.Equal(ProjectStatusEnum.Created, project.Status);
        Assert.Null(project.FinishedAt);
    }

    [Fact]
    public void Cancel_WhenProjectIsCreated_ShouldMoveToCancelled()
    {
        // Arrange
        var project = CreateProject();

        // Act
        var result = project.Cancel();

        // Assert
        Assert.True(result);
        Assert.Equal(ProjectStatusEnum.Cancelled, project.Status);
    }

    [Fact]
    public void Cancel_WhenProjectIsInProgress_ShouldMoveToCancelled()
    {
        // Arrange
        var project = CreateProject();
        project.Start();

        // Act
        var result = project.Cancel();

        // Assert
        Assert.True(result);
        Assert.Equal(ProjectStatusEnum.Cancelled, project.Status);
    }

    [Fact]
    public void Cancel_WhenProjectIsFinished_ShouldReturnFalse()
    {
        // Arrange
        var project = CreateProject();
        project.Start();
        project.Finish();

        // Act
        var result = project.Cancel();

        // Assert
        Assert.False(result);
        Assert.Equal(ProjectStatusEnum.Finished, project.Status);
    }

    [Fact]
    public void Cancel_WhenProjectIsAlreadyCancelled_ShouldReturnFalse()
    {
        // Arrange
        var project = CreateProject();
        project.Cancel();

        // Act
        var result = project.Cancel();

        // Assert
        Assert.False(result);
        Assert.Equal(ProjectStatusEnum.Cancelled, project.Status);
    }
}
