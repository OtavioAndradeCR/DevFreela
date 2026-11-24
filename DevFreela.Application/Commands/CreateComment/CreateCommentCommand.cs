using System.ComponentModel.DataAnnotations;
using MediatR;

namespace DevFreela.Application.Commands.CreateComment
{
    public class CreateCommentCommand : IRequest<Unit>
    {
        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Content { get; set; }

        public int IdProject { get; set; }

        [Range(1, int.MaxValue)]
        public int IdUser { get; set; }
    }
}