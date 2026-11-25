using DevFreela.Core.Entities;
using Xunit;

namespace DevFreela.Tests.Entities;

public class ProjectCommentTests
{
    [Fact]
    public void Constructor_ShouldInitializeCommentProperties()
    {
        var comment = new ProjectComment(
            "Comentário de teste",
            idProject: 10,
            idUser: 20);

        Assert.Equal("Comentário de teste", comment.Content);
        Assert.Equal(10, comment.IdProject);
        Assert.Equal(20, comment.IdUser);
        Assert.Null(comment.Project);
        Assert.Null(comment.User);
        Assert.NotEqual(default, comment.CreatedAt);
    }
}
