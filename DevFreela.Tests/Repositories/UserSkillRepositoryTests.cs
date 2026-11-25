using DevFreela.Core.Entities;
using DevFreela.Infrastructure.Persistence;
using DevFreela.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DevFreela.Tests.Repositories;

public class UserRepositoryTests
{
    private static DevFreelaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DevFreelaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DevFreelaDbContext(options);
    }

    [Fact]
    public async Task GetUserById_WhenUserExists_ShouldReturnUser()
    {
        await using var context = CreateContext();
        var user = new User("Usuário de teste", "usuario@example.com", new DateTime(1995, 5, 20));
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context);

        var result = await repository.GetUserById(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.FullName, result.FullName);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task GetUserById_WhenUserDoesNotExist_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var repository = new UserRepository(context);

        var result = await repository.GetUserById(999);

        Assert.Null(result);
    }
}

public class SkillRepositoryTests
{
    private static DevFreelaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DevFreelaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DevFreelaDbContext(options);
    }

    [Fact]
    public async Task GetAllSkillsAsync_WhenSkillsExist_ShouldReturnAllSkills()
    {
        await using var context = CreateContext();
        context.Skills.AddRange(new Skill("C#"), new Skill("ASP.NET Core"));
        await context.SaveChangesAsync();

        var repository = new SkillRepository(context);

        var result = await repository.GetallSkillsAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, skill => skill.Description == "C#");
        Assert.Contains(result, skill => skill.Description == "ASP.NET Core");
    }

    [Fact]
    public async Task GetAllSkillsAsync_WhenNoSkillsExist_ShouldReturnEmptyList()
    {
        await using var context = CreateContext();
        var repository = new SkillRepository(context);

        var result = await repository.GetallSkillsAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
