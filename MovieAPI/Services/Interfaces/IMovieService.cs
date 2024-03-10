using MovieAPI.Data.Models;
using MovieAPI.ViewModels;

namespace MovieAPI.Services.Interfaces
{
    public interface IMovieService
    {
        Task<List<GetAllMoviesDTO>> GetAllMovies();
        Task<GetAllMoviesDTO> GetMovie(int id);
        Task<Movie> AddMovie(AddMovieDTO movie);
        Task<Movie> UpdateMovie(int movieId, UpdateMovieDTO updates);
        Task<bool> DeleteMovie(int id);
        Task<List<GetAllMovieCommentsDTO>> GetMovieComments(int movieId);
        Task<Comment> AddComment(AddCommentDTO comment, string userId, int movieId);
        Task<bool> AddRating(int movieId, int rating, string userId);
        Task<Comment> UpdateComment(int commentId, string userId, UpdateCommentDTO updates);
        Task<bool> DeleteComment(int commentId);
        Task<Rating> UpdateRating(int ratingId, string userId, int rating);
        Task<List<GetAllMoviesDTO>> GetMovieByName(string name);
        Task<List<GetAllMoviesDTO>> GetOrderedMovieByReleaseDateAsc();
        Task<List<GetAllMoviesDTO>> GetOrderedMovieByReleaseDateDesc();
        Task<List<GetAllMoviesDTO>> GetOrderedMovieByNameAsc();
        Task<List<GetAllMoviesDTO>> GetOrderedMovieByNameDesc();
        Task<bool> DeleteImage(int imageId, int movieId);
    }
}
