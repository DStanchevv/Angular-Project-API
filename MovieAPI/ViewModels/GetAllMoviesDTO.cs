namespace MovieAPI.ViewModels
{
    public class GetAllMoviesDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }
        public string DirectorName { get; set; } = null!;
        public int Length { get; set; }
        public List<string> Images { get; set; } = null!;
    }
}
