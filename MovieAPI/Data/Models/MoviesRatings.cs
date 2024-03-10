using System.ComponentModel.DataAnnotations.Schema;

namespace MovieAPI.Data.Models
{
    public class MoviesRatings
    {
        [ForeignKey(nameof(Movie))]
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        [ForeignKey(nameof(Rating))]
        public int RatingId { get; set; }
        public Rating Rating { get; set; } = null!;
    }
}
