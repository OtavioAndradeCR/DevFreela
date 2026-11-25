using DevFreela.Core.Entities;
using Xunit;

namespace DevFreela.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Constructor_ShouldInitializeUserAndCollections()
    {
        var birthDate = new DateTime(1994, 6, 15);

        var user = new User("Usuário de teste", "teste@example.com", birthDate);

        Assert.Equal("Usuário de teste", user.FullName);
        Assert.Equal("teste@example.com", user.Email);
        Assert.Equal(birthDate, user.BirthDate);
        Assert.True(user.Active);
        Assert.NotEqual(default, user.CreatedAt);
        Assert.NotNull(user.Skills);
        Assert.Empty(user.Skills);
        Assert.NotNull(user.OwnedProjects);
        Assert.Empty(user.OwnedProjects);
        Assert.NotNull(user.FreelanceProjects);
        Assert.Empty(user.FreelanceProjects);
        Assert.NotNull(user.Comments);
        Assert.Empty(user.Comments);
    }
}
