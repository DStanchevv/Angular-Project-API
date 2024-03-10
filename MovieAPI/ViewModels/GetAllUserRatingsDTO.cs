namespace MovieAPI.ViewModels
{
    public class GetAllUserRatingsDTO
    {
        public string Movie { get; set; } = null!;
        public int Rating { get; set; }
    }
}
