using MediatR;

namespace DevFreela.Application.Commands.Delete
{
    public class DeleteProjectCommand : IRequest<bool>
    {
        public DeleteProjectCommand(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}