using DevFreela.Application.Services.Interfaces;
using DevFreela.Application.ViewModels;
using DevFreela.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DevFreela.Application.Services.Implementations
{
    public class ProjectService : IProjectService
    {
        private readonly DevFreelaDbContext _dbContext;

        public ProjectService(DevFreelaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ProjectViewModel> GetAll(string query)
        {
            var projects = _dbContext.Projects.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                projects = projects.Where(project => project.Title.Contains(query));
            }

            return projects
                .Select(project => new ProjectViewModel(project.Id, project.Title, project.CreatedAt))
                .ToList();
        }

        public ProjectDetailsViewModel GetById(int id)
        {
            var project = _dbContext.Projects
                .Include(project => project.Client)
                .Include(project => project.Freelancer)
                .SingleOrDefault(project => project.Id == id);

            if (project == null)
            {
                return null;
            }

            return new ProjectDetailsViewModel(
                project.Id,
                project.Title,
                project.Description,
                project.TotalCost,
                project.StartedAt,
                project.FinishedAt,
                project.Client.FullName,
                project.Freelancer.FullName);
        }

        public async Task<bool> Start(int id, CancellationToken cancellationToken = default)
        {
            var project = await _dbContext.Projects
                .SingleOrDefaultAsync(project => project.Id == id, cancellationToken);

            if (project == null)
            {
                return false;
            }

            project.Start();
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> Finish(int id, CancellationToken cancellationToken = default)
        {
            var project = await _dbContext.Projects
                .SingleOrDefaultAsync(project => project.Id == id, cancellationToken);

            if (project == null)
            {
                return false;
            }

            project.Finish();
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}