using DevFreela.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevFreela.Application.Commands.Delete
{
    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, bool>
    {
        private readonly IDevFreelaDbContext _dbContext;

        public DeleteProjectCommandHandler(DevFreelaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _dbContext.Projects
                .SingleOrDefaultAsync(project => project.Id == request.Id, cancellationToken);

            if (project == null)
            {
                return false;
            }

            // The domain operation cancels the project instead of physically deleting it.
            project.Cancel();
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}