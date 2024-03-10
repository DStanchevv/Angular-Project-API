using System.ComponentModel.DataAnnotations.Schema;

namespace MovieAPI.Data.Models
{
    public class MoviesComments
    {
        [ForeignKey(nameof(Movie))]
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        [ForeignKey(nameof(Comment))]
        public int CommentId { get; set; }
        public Comment Comment { get; set; } = null!;

    }
}
