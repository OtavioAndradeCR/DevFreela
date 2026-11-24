using DevFreela.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevFreela.Application.Commands.UpdateProject
{
    public class UpdateProjectCommandhandler : IRequestHandler<UpdateProjectCommand, bool>
    {
        private readonly DevFreelaDbContext _dbContext;

        public UpdateProjectCommandhandler(DevFreelaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _dbContext.Projects
                .SingleOrDefaultAsync(project => project.Id == request.Id, cancellationToken);

            if (project == null)
            {
                return false;
            }

            project.Update(request.Title, request.Description, request.TotalCost);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}