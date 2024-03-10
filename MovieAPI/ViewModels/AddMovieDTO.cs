using System.ComponentModel.DataAnnotations;

namespace MovieAPI.ViewModels
{
    public class AddMovieDTO
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public string ReleaseDate { get; set; } = null!;
        [Required]
        public string DirectorName { get; set; } = null!;
        public int Length { get; set; }
        public List<IFormFile>? files { get; set; }
    }
}
