using System.ComponentModel.DataAnnotations;

namespace DevFreela.Application.InputModels
{
    public class CreateUserInputModel
    {
        [Required]
        [StringLength(120, MinimumLength = 3)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(254)]
        public string Email { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        // Password is retained for compatibility with the original request model.
        // This project does not implement credential storage or authentication.
        public string Password { get; set; }
    }
}