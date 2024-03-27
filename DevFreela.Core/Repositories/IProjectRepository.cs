using DevFreela.Core.Entities;

namespace DevFreela.Core.Repositories
{
	public interface IProjectRepository
	{
		Task<List<Project>> GetAllProjectsAsync();
		Task<Project> GetProjectByIdAsync(int id);
	}
}
