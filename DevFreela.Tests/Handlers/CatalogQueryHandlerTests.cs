using DevFreela.Application.Queries.GetAllSkills;
using DevFreela.Application.Queries.GetUser;
using DevFreela.Core.Entities;
using DevFreela.Core.Repositories;
using Xunit;

namespace DevFreela.Tests.Handlers;

public class CatalogQueryHandlerTests
{
    private sealed class FakeUserRepository(User user) : IUserRepository
    {
        public Task<User> GetUserById(int id) =>
            Task.FromResult(user.Id == id ? user : null);
    }

    private sealed class FakeSkillRepository(List<Skill> skills) : ISkillRepository
    {
        public Task<List<Skill>> GetallSkillsAsync() => Task.FromResult(skills);
    }

    [Fact]
    public async Task GetUserById_WhenUserExists_ShouldReturnUserViewModel()
    {
        var user = new User("Usuário de teste", "teste@example.com", new DateTime(1995, 1, 1));
        var handler = new GetUserByIdHandler(new FakeUserRepository(user));

        var result = await handler.Handle(new GetUserById(user.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(user.FullName, result.FullName);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task GetUserById_WhenUserDoesNotExist_ShouldReturnNull()
    {
        var user = new User("Usuário de teste", "teste@example.com", new DateTime(1995, 1, 1));
        var handler = new GetUserByIdHandler(new FakeUserRepository(user));

        var result = await handler.Handle(new GetUserById(999), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllSkills_WhenSkillsExist_ShouldReturnSkillViewModels()
    {
        var skills = new List<Skill>
        {
            new("C#"),
            new("ASP.NET Core"),
            new("SQL Server")
        };
        var handler = new GetAllSkillsQueryHandler(new FakeSkillRepository(skills));

        var result = await handler.Handle(new GetAllSkillsQuery(), CancellationToken.None);

        Assert.Equal(3, result.Count);
        Assert.Equal(new[] { "C#", "ASP.NET Core", "SQL Server" },
            result.Select(skill => skill.Description));
    }

    [Fact]
    public async Task GetAllSkills_WhenNoSkillsExist_ShouldReturnEmptyList()
    {
        var handler = new GetAllSkillsQueryHandler(new FakeSkillRepository([]));

        var result = await handler.Handle(new GetAllSkillsQuery(), CancellationToken.None);

        Assert.Empty(result);
    }
}
