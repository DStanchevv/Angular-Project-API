using System.ComponentModel.DataAnnotations;

namespace MovieAPI.ViewModels
{
    public class RegisterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
