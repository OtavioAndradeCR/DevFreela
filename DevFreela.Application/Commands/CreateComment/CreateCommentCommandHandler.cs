using DevFreela.Core.Entities;
using DevFreela.Application.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DevFreela.Application.Commands.CreateComment
{
    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, bool>
    {
        private readonly IDevFreelaDbContext _dbContext;

        public CreateCommentCommandHandler(IDevFreelaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var projectExists = await _dbContext.Projects
                .AnyAsync(project => project.Id == request.IdProject, cancellationToken);
            var userExists = await _dbContext.Users
                .AnyAsync(user => user.Id == request.IdUser, cancellationToken);

            if (!projectExists || !userExists)
            {
                return false;
            }

            var comment = new ProjectComment(request.Content, request.IdProject, request.IdUser);

            await _dbContext.ProjectComments.AddAsync(comment, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}