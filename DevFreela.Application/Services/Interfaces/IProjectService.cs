using DevFreela.Application.ViewModels;

namespace DevFreela.Application.Services.Interfaces
{
    public interface IProjectService
    {
        List<ProjectViewModel> GetAll(string query);
        ProjectDetailsViewModel GetById(int id);
        Task<bool> Start(int id, CancellationToken cancellationToken = default);
        Task<bool> Finish(int id, CancellationToken cancellationToken = default);
    }
}