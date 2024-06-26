﻿using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieAPI.Data;
using MovieAPI.Data.Models;
using MovieAPI.Services.Interfaces;
using MovieAPI.ViewModels;

namespace MovieAPI.Services
{
    public class MovieService : IMovieService
    {
        private readonly DataContext context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public MovieService(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<Comment> AddComment(AddCommentDTO comment, string userId, int movieId)
        {
            var existingComment = await context.Comments
                .FirstOrDefaultAsync(x => x.Description == comment.Description && x.UserId == userId);

            if (existingComment != null)
            {
                return null;
            }

            Comment commentToAdd = new Comment()
            {
                Description = comment.Description,
                UserId = userId,
            };

            await context.Comments.AddAsync(commentToAdd);
            await context.SaveChangesAsync();

            MoviesComments movieComment = new MoviesComments()
            {
                CommentId = commentToAdd.Id,
                MovieId = movieId,
            };

            await context.MoviesComments.AddAsync(movieComment);
            await context.SaveChangesAsync();

            return commentToAdd;
        }

        public async Task<Movie> AddMovie(AddMovieDTO movie)
        {
            var movies = await context.Movies.ToListAsync();
            if(movies.Any(m => m.Name == movie.Name))
            {
                return null;
            } 
            
            Movie movieToAdd = new Movie()
            {
                Name = movie.Name,
                Description = movie.Description,
                DirectorName = movie.DirectorName,
                ReleaseDate = DateTime.Parse(movie.ReleaseDate),
                Length = movie.Length,
            };
            await context.Movies.AddAsync(movieToAdd);
            await context.SaveChangesAsync();

            var files = movie.files;
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    var isValidImage = IsImageValid(file);

                    if (isValidImage)
                    {
                        var uniqueFileName = GetUniqueFileName(file);
                        var filePath = SaveImageToFile(file, uniqueFileName);

                        var image = await SaveImageToDatabase(filePath, context);

                        await SaveMovieImageToDatabase(movieToAdd.Id, image.Id);
                    }
                }
            }

            return movieToAdd;
        }

        public async Task<bool> AddRating(int movieId, int rating, string userId)
        {
            var movieRatings = await context.MoviesRatings.Include(mr => mr.Rating).ToListAsync();
            var newRating = new Rating()
            {
                UserId = userId,
                RatingScore = rating,
            };
            if (movieRatings.Any(mr => mr.MovieId == movieId && mr.Rating.UserId == userId))
            {
                return false;
            }
            await context.Ratings.AddAsync(newRating);
            await context.SaveChangesAsync();
            
            var movieRating = new MoviesRatings()
            {
                MovieId = movieId,
                RatingId = newRating.Id
            };
            await context.MoviesRatings.AddAsync(movieRating);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteComment(int commentId)
        {
            var comment = await context.Comments.FindAsync(commentId);
            if (comment != null)
            {
                var movieComment = await context.MoviesComments.Where(mc => mc.CommentId == commentId).FirstOrDefaultAsync();
                context.MoviesComments.Remove(movieComment);
                context.Comments.Remove(comment);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteMovie(int id)
        {
            var movie = await context.Movies.FindAsync(id);
            if (movie == null)
            {
                return false;
            }

            var movieImages = await context.MoviesImages.Where(x => x.MovieId == id).ToListAsync();
            if (movieImages != null && movieImages.Any())
            {
                foreach (var mi in movieImages)
                {
                    var img = await context.Images.FindAsync(mi.ImageId);
                    if (img != null)
                    {
                        var fileName = Path.GetFileName(img.Path);

                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Movies", fileName);

                        if (File.Exists(imagePath))
                        {
                            File.Delete(imagePath);
                        }
                    }
                }
                context.MoviesImages.RemoveRange(movieImages);
            }

            var movieRatings = await context.MoviesRatings.Where(x => x.MovieId == id).ToListAsync();
            if (movieRatings != null && movieRatings.Any())
            {
                context.MoviesRatings.RemoveRange(movieRatings);
                foreach (var mr in movieRatings)
                {
                    if (mr.Rating != null) // Check if rating is null
                    {
                        context.Ratings.Remove(mr.Rating);
                    }
                }
            }

            var movieComments = await context.MoviesComments.Where(x => x.MovieId == id).ToListAsync();
            if (movieComments != null && movieComments.Any())
            {
                context.MoviesComments.RemoveRange(movieComments);
                foreach (var mc in movieComments)
                {
                    if (mc.Comment != null) // Check if comment is null
                    {
                        context.Comments.Remove(mc.Comment);
                    }
                }
            }

            context.Movies.Remove(movie);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<GetAllMoviesDTO>> GetOrderedMoviesById()
        {
            var movies = await context.Movies.Select(m => new GetAllMoviesDTO
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                ReleaseDate = m.ReleaseDate,
                DirectorName = m.DirectorName,
                Length = m.Length,
                Images = context.MoviesImages.Where(mi => mi.MovieId == m.Id).Include(mi => mi.Image).Select(mi => mi.Image.Path).ToList(),
            }).OrderByDescending(m => m.Id).ToListAsync();

            if (movies.Count == 0)
            {
                return null;
            }

            return movies;
        }

        public async Task<List<GetAllMoviesDTO>> GetAllMovies(int page, int perPage)
        {
            var movies = await context.Movies.Select(movie => new GetAllMoviesDTO
            {
                Id = movie.Id,
                Name = movie.Name,
                Description = movie.Description,
                DirectorName = movie.DirectorName,
                ReleaseDate = movie.ReleaseDate,
                Length = movie.Length,
                Images = context.MoviesImages.Where(mi => mi.MovieId == movie.Id).Include(mi => mi.Image).Select(mi => mi.Image.Path).ToList(),
            }).ToListAsync();

            if(movies.Count == 0) 
            {
                return null;
            }

            var skip = (page - 1) * perPage;

            return movies.Skip(skip).Take(perPage).ToList();
        }

        public async Task<GetAllMoviesDTO> GetMovie(int id)
        {
            var movie = await context.Movies.Where(m => m.Id == id).Select(movie => new GetAllMoviesDTO
            {
                Id = movie.Id,
                Name = movie.Name,
                Description = movie.Description,
                DirectorName = movie.DirectorName,
                ReleaseDate = movie.ReleaseDate,
                Length = movie.Length,
                Images = context.MoviesImages.Where(mi => mi.MovieId == id).Include(mi => mi.Image).Select(mi => mi.Image.Path).ToList(),
                Comments = context.MoviesComments.Where(mc => mc.MovieId == id).Include(mc => mc.Comment).OrderByDescending(mc => mc.CommentId).Select(mc => $"{mc.CommentId}-{mc.Comment.User.Email}-{mc.Comment.Description}").ToList(),
                Rating = context.MoviesRatings.Where(mr => mr.MovieId == id).Include(mr => mr.Rating).Average(mr => mr.Rating.RatingScore).ToString(),
            }).FirstOrDefaultAsync();

            if(movie == null) 
            {
                return null;
            }

            return movie;
        }

        public async Task<List<GetAllMoviesDTO>> GetMovieByName(string name)
        {
            var movies = await context.Movies.Where(m => m.Name.ToLower().Contains(name.ToLower())).Select(m => new GetAllMoviesDTO
            {
                Id=m.Id,
                Name = m.Name,
                Description = m.Description,
                DirectorName = m.DirectorName,
                ReleaseDate = m.ReleaseDate,
                Length = m.Length,
                Images = context.MoviesImages.Where(mi => mi.MovieId == m.Id).Include(mi => mi.Image).Select(mi => mi.Image.Path).ToList(),
            }).ToListAsync();

            if(movies.Count == 0)
            {
                return null;
            }

            return movies;
        }

        public async Task<List<GetAllMovieCommentsDTO>> GetMovieComments(int movieId)
        {
            var movie = await context.Movies.FindAsync(movieId);
            var comments = await context.MoviesComments.Where(x => x.MovieId == movieId).Select(x =>
            new GetAllMovieCommentsDTO 
            {
                Id = x.CommentId,
                User = x.Comment.User.UserName,
                Description = x.Comment.Description,
            }).OrderByDescending(mc => mc.Id).ToListAsync();

            if(comments.Count() == 0) 
            {
                return null;
            }

            return comments;
        }

        public async Task<List<GetAllMoviesDTO>> GetOrderedMovieByReleaseDateAsc()
        {
            var movies = await context.Movies.Select(m => new GetAllMoviesDTO
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                ReleaseDate = m.ReleaseDate,
                DirectorName = m.DirectorName,
                Length = m.Length,
                Images = context.MoviesImages.Where(mi => mi.MovieId ==m.Id).Include(mi => mi.Image).Select(mi => mi.Image.Path).ToList(),
            }).OrderBy(m => m.ReleaseDate).ToListAsync();

            if(movies.Count == 0) 
            {
                return null;
            }

            return movies;
        }

        public async Task<List<GetAllMoviesDTO>> GetOrderedMovieByReleaseDateDesc()
        {
            var movies = await context.Movies.Select(m => new GetAllMoviesDTO
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                ReleaseDate = m.ReleaseDate,
                DirectorName = m.DirectorName,
                Length = m.Length,
                Images = context.MoviesImages.Where(mi => mi.MovieId == m.Id).Include(mi => mi.Image).Select(mi => mi.Image.Path).ToList(),
            }).OrderByDescending(m => m.ReleaseDate).ToListAsync();

            if (movies.Count == 0)
            {
                return null;
            }

            return movies;
        }

        public async Task<List<GetAllMoviesDTO>> GetOrderedMovieByNameAsc()
        {
            var movies = await context.Movies.Select(m => new GetAllMoviesDTO
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                ReleaseDate = m.ReleaseDate,
                DirectorName = m.DirectorName,
                Length = m.Length,
                Images = context.MoviesImages.Where(mi => mi.MovieId == m.Id).Include(mi => mi.Image).Select(mi => mi.Image.Path).ToList(),
            }).OrderBy(m => m.Name).ToListAsync();

            if (movies.Count == 0)
            {
                return null;
            }

            return movies;
        }

        public async Task<List<GetAllMoviesDTO>> GetOrderedMovieByNameDesc()
        {
            var movies = await context.Movies.Select(m => new GetAllMoviesDTO
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                ReleaseDate = m.ReleaseDate,
                DirectorName = m.DirectorName,
                Length = m.Length,
                Images = context.MoviesImages.Where(mi => mi.MovieId == m.Id).Include(mi => mi.Image).Select(mi => mi.Image.Path).ToList(),
            }).OrderByDescending(m => m.Name).ToListAsync();

            if (movies.Count == 0)
            {
                return null;
            }

            return movies;
        }

        public async Task<Comment> UpdateComment(int commentId, string userId, UpdateCommentDTO updates)
        {
            var commentToUpdate = await context.Comments.Where(c => c.Id == commentId && c.UserId == userId).FirstOrDefaultAsync();
            
            if(commentToUpdate == null) 
            {
                return null;
            }

            commentToUpdate.Description = updates.Description;

            await context.SaveChangesAsync();

            return commentToUpdate;
        }

        public async Task<Movie> UpdateMovie(int movieId, UpdateMovieDTO updates)
        {
            var movieToUpdate = await context.Movies.FindAsync(movieId);
            
            if(movieToUpdate == null)
            {
                return null;
            }

            if(!updates.Name.IsNullOrEmpty())
                movieToUpdate.Name = updates.Name;
            if (!updates.Description.IsNullOrEmpty())
                movieToUpdate.Description = updates.Description;
            if (!updates.DirectorName.IsNullOrEmpty())
                movieToUpdate.DirectorName = updates.DirectorName;
           
            try
            {
                if (!updates.ReleaseDate.IsNullOrEmpty())
                    movieToUpdate.ReleaseDate = DateTime.Parse(updates.ReleaseDate);
            }
            catch
            {
                return null;
            }

            if (!updates.ReleaseDate.IsNullOrEmpty())
                movieToUpdate.Length = updates.Length;

            var files = updates.files;
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    var isValidImage = IsImageValid(file);

                    if (isValidImage)
                    {
                        var uniqueFileName = GetUniqueFileName(file);
                        var filePath = SaveImageToFile(file, uniqueFileName);

                        var image = await SaveImageToDatabase(filePath, context);

                        await SaveMovieImageToDatabase(movieToUpdate.Id, image.Id);
                    }
                }
            }

            await context.SaveChangesAsync();

            return movieToUpdate;
        }

        public async Task<Rating> UpdateRating(int movieId, string userId, int rating)
        {
            var ratingToUpdate = await context.MoviesRatings.Where(mr => mr.MovieId == movieId && mr.Rating.UserId == userId).FirstOrDefaultAsync();
            var newRating = await context.Ratings.Where(r => r.Id == ratingToUpdate.RatingId).FirstOrDefaultAsync();

            if (ratingToUpdate == null)
            {
                return null;
            }

            if(rating >= 1 && rating <= 10) 
            { 
                newRating.RatingScore = rating;
                
                await context.SaveChangesAsync();
                return newRating;
            }

            return null;
        }

        public async Task<bool> DeleteImage(string imgLink)
        {
            var image = await context.Images.Where(i => i.Path == imgLink).FirstOrDefaultAsync();
            
            if(image != null)
            {
                var moviesImages = await context.MoviesImages.Where(mi => mi.ImageId == image.Id).ToListAsync();
                if(moviesImages.Any()) 
                {
                    context.MoviesImages.RemoveRange(moviesImages);
                    await context.SaveChangesAsync();
                }

                context.Images.Remove(image);
                await context.SaveChangesAsync();

                var fileName = Path.GetFileName(imgLink);

                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Movies", fileName);

                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                }

                return true;
            }

            return false;
        }

        private bool IsImageValid(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            var validExtensions = new[] { ".jpg", ".jpeg", ".png" };

            return file != null && file.Length > 0 && validExtensions.Contains(extension);
        }

        private string GetUniqueFileName(IFormFile file)
        {
            return $"{Guid.NewGuid()}_{file.FileName}";
        }

        private string SaveImageToFile(IFormFile file, string uniqueFileName)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Movies");
            var filePath = Path.Combine(folder, uniqueFileName);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return $"http://localhost:5099/Images/Movies/{uniqueFileName}";
        }

        private async Task<Image> SaveImageToDatabase(string filePath, DataContext context)
        {
            var image = new Image { Path = filePath };
            await context.Images.AddAsync(image);
            await context.SaveChangesAsync();

            return image;
        }

        private async Task SaveMovieImageToDatabase(int movieId, int imageId)
        {
            var movieImage = new MoviesImages { MovieId = movieId, ImageId = imageId };
            await context.MoviesImages.AddAsync(movieImage);
            await context.SaveChangesAsync();
        }

        public async Task<List<GetAllMoviesDTO>> GetOrderedMovieByRatingDesc()
        {
            var moviesWithRatings = await context.Movies
                .Select(m => new
                {
                    Movie = m,
                    AverageRating = context.MoviesRatings
                        .Where(mr => mr.MovieId == m.Id)
                        .Select(mr => (double?)mr.Rating.RatingScore)
                        .DefaultIfEmpty()
                        .Average()
                })
                .OrderByDescending(m => m.AverageRating)
                .ToListAsync();

            var movies = moviesWithRatings.Select(m => new GetAllMoviesDTO
            {
                Id = m.Movie.Id,
                Name = m.Movie.Name,
                Description = m.Movie.Description,
                ReleaseDate = m.Movie.ReleaseDate,
                DirectorName = m.Movie.DirectorName,
                Length = m.Movie.Length,
                Rating = m.AverageRating.HasValue ? m.AverageRating.Value.ToString() : "N/A",
                Images = context.MoviesImages
                    .Where(mi => mi.MovieId == m.Movie.Id)
                    .Include(mi => mi.Image)
                    .Select(mi => mi.Image.Path)
                    .ToList(),
            }).ToList();

            if (movies.Count == 0)
            {
                return null;
            }

            return movies;
        }
    }
}
