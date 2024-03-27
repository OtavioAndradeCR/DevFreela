using DevFreela.Core.Entities;

namespace DevFreela.Core.Repositories;

public interface IUserRepository
{
	public Task<User> GetUserById(int id);
}
