using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MovieAPI.Data.Models;
using MovieAPI.Services.Interfaces;
using MovieAPI.ViewModels;
using System.Security.Claims;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService movieService;
        public MovieController(IMovieService movieService)
        {
            this.movieService = movieService;
        }

        [HttpGet("get-all-movies")]
        public async Task<IActionResult> GetAllMovies()
        {
            var movies = await movieService.GetAllMovies();
            if(movies == null)
            {
                return NotFound();
            }

            return Ok(movies);
        }

        [HttpGet("get-movie/{movieId}")]
        public async Task<IActionResult> GetMovie([FromRoute]int movieId)
        {
            var movie = await movieService.GetMovie(movieId);

            if(movie == null) 
            { 
                return NotFound();
            }

            return Ok(movie);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("create-movie")]
        public async Task<IActionResult> AddMovie([FromForm]AddMovieDTO movie)
        {
            if(ModelState.IsValid) 
            {
                var addedMovie = await movieService.AddMovie(movie);
                return Ok(addedMovie);
            }

            return BadRequest();
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("update-movie/{movieId}")]
        public async Task<IActionResult> UpdateMovie([FromRoute]int movieId, [FromForm]UpdateMovieDTO updates)
        {
            if(ModelState.IsValid) 
            { 
                var updatedMovie = await movieService.UpdateMovie(movieId, updates);
                if(updatedMovie == null)
                {
                    return BadRequest();
                }
                return Ok(updatedMovie);
            }

            return BadRequest();
        }

        //[Authorize(Roles = "Admin")]
        [HttpDelete("delete-movie/{movieId}")]
        public async Task<IActionResult> DeleteMovie([FromRoute]int movieId)
        {
            var successful = await movieService.DeleteMovie(movieId);
            if(successful) 
            { 
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("get-all-movie-comments/{movieId}")]
        public async Task<IActionResult> GetAllMovieComments([FromRoute] int movieId)
        {
            var movie = await movieService.GetMovie(movieId);
            
            if (movie == null)
            {
                return NotFound();
            }

            var comments = await movieService.GetMovieComments(movieId);
            return Ok(comments);
        }

        //[Authorize]
        [HttpPost("write-comment/{movieId}")]
        public async Task<IActionResult> CommentMovie(int movieId, AddCommentDTO commentToAdd)
        {
            var movie = await movieService.GetMovie(movieId);

            if (movie == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var addedComment = await movieService.AddComment(commentToAdd, User.FindFirstValue(ClaimTypes.NameIdentifier), movieId);
                if(addedComment == null) 
                { 
                    return BadRequest();
                }
                return Ok(addedComment);
            }

            return null;
        }

        //[Authorize]
        [HttpPost("rate-movie/{movieId}")]
        public async Task<IActionResult> RateMovie(int movieId, int rating)
        {
            var movie = await movieService.GetMovie(movieId);

            if (movie == null)
            {
                return NotFound();
            }

            if(rating >= 1 || rating <= 10) 
            {
                var res = await movieService.AddRating(movieId, rating, User.FindFirstValue(ClaimTypes.NameIdentifier));
                
                if(res == false)
                {
                    return BadRequest();
                }

                return Ok();
            }

            return null;
        }

        //[Authorize]
        [HttpPut("update-comment")]
        public async Task<IActionResult> UpdateComment(int commentId, UpdateCommentDTO updates)
        {
            if (ModelState.IsValid)
            {
                var updatedComment = await movieService.UpdateComment(commentId, User.FindFirstValue(ClaimTypes.NameIdentifier), updates);
                if (updatedComment == null)
                {
                    return BadRequest();
                }
                return Ok(updatedComment);
            }

            return BadRequest();
        }

        //[Authorize]
        [HttpDelete("delete-comment")]
        public async Task<IActionResult> DeleteComment(int commentid)
        {
            var successful = await movieService.DeleteComment(commentid);
            if (successful)
            {
                return Ok();
            }
            return BadRequest();
        }
        //[Authorize]
        [HttpPut("update-rating")]
        public async Task<IActionResult> UpdateReview(int ratingId, int rating)
        {
            if (ModelState.IsValid)
            {
                var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var updatedRating = await movieService.UpdateRating(ratingId, User.FindFirstValue(ClaimTypes.NameIdentifier), rating);
                if (updatedRating == null)
                {
                    return BadRequest();
                }
                return Ok(updatedRating);
            }

            return BadRequest();
        }

        [HttpGet("search-movie-by-name")]
        public async Task<IActionResult> SearchMovieByName(string movieName)
        {
            var movies = await movieService.GetMovieByName(movieName);

            if (movies == null)
            {
                return NotFound();
            }

            return Ok(movies);
        }

        [HttpGet("order-movie-by-release-date-asc")]
        public async Task<IActionResult> OrderMovieByReleaseDateAsc()
        {
            var movie = await movieService.GetOrderedMovieByReleaseDateAsc();

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpGet("order-movie-by-release-date-desc")]
        public async Task<IActionResult> OrderMovieByReleaseDateDesc()
        {
            var movie = await movieService.GetOrderedMovieByReleaseDateDesc();

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpGet("order-movie-by-name-asc")]
        public async Task<IActionResult> OrderMovieByNameAsc()
        {
            var movie = await movieService.GetOrderedMovieByNameAsc();

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpGet("order-movie-by-name-desc")]
        public async Task<IActionResult> OrderMovieByNameDesc()
        {
            var movie = await movieService.GetOrderedMovieByNameDesc();

            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }

        [HttpDelete("delete-image")]
        public async Task<IActionResult> DeleteImageById(int imageId, int movieId)
        {
            var successful = await movieService.DeleteImage(imageId, movieId);
            if (successful)
            {
                return Ok();
            }
            return BadRequest();
        }

    }
}
