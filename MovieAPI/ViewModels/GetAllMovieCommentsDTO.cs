namespace MovieAPI.ViewModels
{
    public class GetAllMovieCommentsDTO
    {
        public int Id { get; set; }
        public string User { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
