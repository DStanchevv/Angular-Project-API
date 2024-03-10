namespace MovieAPI.Data.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }
        public string DirectorName { get; set; } = null!;
        public int Length { get; set; }
    }
}
