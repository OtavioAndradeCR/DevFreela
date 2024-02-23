using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevFreela.Application.Commands.Delete
{
    public class DeleteProjectCommand : IRequest<Unit>
    {
        public DeleteProjectCommand(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
    }
}