using DevFreela.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevFreela.Application.Commands.Delete
{
    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, bool?>
    {
        private readonly IDevFreelaDbContext _dbContext;

        public DeleteProjectCommandHandler(IDevFreelaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool?> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _dbContext.Projects
                .SingleOrDefaultAsync(project => project.Id == request.Id, cancellationToken);

            if (project == null)
            {
                return null;
            }

            // Cancellation changes the project state; it does not physically delete the record.
            if (!project.Cancel())
            {
                return false;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}