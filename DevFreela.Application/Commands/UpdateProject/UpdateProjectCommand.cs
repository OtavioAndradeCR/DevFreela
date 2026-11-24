using System.ComponentModel.DataAnnotations;
using MediatR;

namespace DevFreela.Application.Commands.UpdateProject
{
    public class UpdateProjectCommand : IRequest<bool>
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Description { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ParseLimitsInInvariantCulture = true)]
        public decimal TotalCost { get; set; }
    }
}