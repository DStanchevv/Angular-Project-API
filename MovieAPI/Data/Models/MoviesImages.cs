using System.ComponentModel.DataAnnotations.Schema;

namespace MovieAPI.Data.Models
{
    public class MoviesImages
    {
        [ForeignKey(nameof(Image))]
        public int ImageId { get; set; }
        public Image Image { get; set; } = null!;

        [ForeignKey(nameof(Movie))]
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
    }
}
