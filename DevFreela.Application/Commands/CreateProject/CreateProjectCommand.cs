using System.ComponentModel.DataAnnotations;
using MediatR;

namespace DevFreela.Application.Commands.CreateProject
{
    public class CreateProjectCommand : IRequest<int>
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Description { get; set; }

        [Range(1, int.MaxValue)]
        public int IdClient { get; set; }

        [Range(1, int.MaxValue)]
        public int IdFreelancer { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal TotalCost { get; set; }
    }
}