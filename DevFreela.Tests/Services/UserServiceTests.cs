using DevFreela.Application.InputModels;
using DevFreela.Application.Services.Implementations;
using DevFreela.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DevFreela.Tests.Services;

public class UserServiceTests
{
    private static DevFreelaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DevFreelaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DevFreelaDbContext(options);
    }

    [Fact]
    public async Task Create_ShouldPersistUserAndReturnGeneratedId()
    {
        await using var context = CreateContext();
        var service = new UserService(context);
        var inputModel = new CreateUserInputModel
        {
            FullName = "Usuário de teste",
            Email = "usuario@example.com",
            BirthDate = new DateTime(1995, 5, 20)
        };

        var result = await service.Create(inputModel);

        var persistedUser = await context.Users.SingleAsync();
        Assert.True(result > 0);
        Assert.Equal(result, persistedUser.Id);
        Assert.Equal(inputModel.FullName, persistedUser.FullName);
        Assert.Equal(inputModel.Email, persistedUser.Email);
        Assert.Equal(inputModel.BirthDate, persistedUser.BirthDate);
        Assert.True(persistedUser.Active);
    }

    [Fact]
    public async Task GetUser_WhenUserExists_ShouldReturnUserViewModel()
    {
        await using var context = CreateContext();
        var user = new DevFreela.Core.Entities.User(
            "Usuário de teste",
            "usuario@example.com",
            new DateTime(1995, 5, 20));
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new UserService(context);

        var result = service.GetUser(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.FullName, result.FullName);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task GetUser_WhenUserDoesNotExist_ShouldReturnNull()
    {
        await using var context = CreateContext();
        var service = new UserService(context);

        var result = service.GetUser(999);

        Assert.Null(result);
    }
}
