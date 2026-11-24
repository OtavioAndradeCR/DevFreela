using DevFreela.Application.ViewModels;
using DevFreela.Core.Repositories;
using MediatR;

namespace DevFreela.Application.Queries.GetAllProjects
{
    public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, List<ProjectViewModel>>
    {
        private readonly IProjectRepository _projectRepository;

        public GetAllProjectsQueryHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<List<ProjectViewModel>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
        {
            var projects = await _projectRepository.GetAllProjectsAsync();

            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                projects = projects
                    .Where(project => project.Title.Contains(request.Query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return projects
                .Select(project => new ProjectViewModel(project.Id, project.Title, project.CreatedAt))
                .ToList();
        }
    }
}