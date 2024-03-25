using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MovieAPI.Data;
using MovieAPI.Data.Models;
using MovieAPI.Services.Interfaces;
using MovieAPI.ViewModels;

namespace MovieAPI.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ITokenService tokenService;
        public UserService(DataContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
        {
            this.context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
        }

        public async Task<NewUserDTO> CreateUser(RegisterDTO register)
        {
            var applicationUser = new ApplicationUser()
            {
                UserName = register.Email.Split("@")[0],
                Email = register.Email
            };
            
            var createdUser = await userManager.CreateAsync(applicationUser, register.Password);

            if(createdUser.Succeeded)
            {
                var role = await userManager.AddToRoleAsync(applicationUser, "User");
                if (role.Succeeded)
                {
                    var newUser = new NewUserDTO()
                    {
                        Username = applicationUser.Email.Split("@")[0],
                        Email = applicationUser.Email,
                        Role = "User",
                        Token = tokenService.CreateToken(applicationUser)
                    };
                    return newUser;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public async Task<ApplicationUser> FindUser(string email)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Email == email);
            if(user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<UserProfileDTO> FindUserById(string id)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            var role = await userManager.IsInRoleAsync(user, "User") ? "User" : "Admin";
            var loggedUser = new UserProfileDTO
            {
                Username = user.Email.Split("@")[0],
                Email = user.Email,
                Role = role,
            };
            return loggedUser;
        }

        public async Task<List<GetAllUserCommentsDTO>> GetAllUserComments(string userId)
        {
            var comments = await context.MoviesComments.Include(c => c.Comment).Where(c => c.Comment.UserId == userId).Include(c => c.Movie).Select(c => new GetAllUserCommentsDTO
            {
                Movie = c.Movie.Name,
                Comment = c.Comment.Description,
            }).ToListAsync();

            if(comments.Count() == 0) 
            {
                return null;
            }

            return comments;
        }

        public async Task<List<GetAllUserRatingsDTO>> GetAllUserRatings(string userId)
        {
            var ratings = await context.MoviesRatings.Include(r => r.Rating).Where(r => r.Rating.UserId == userId).Include(r => r.Movie).Select(r => new GetAllUserRatingsDTO
            {
                Movie = r.Movie.Name,
                Rating = r.Rating.RatingScore,
            }).ToListAsync();

            if(ratings.Count() == 0)
            {
                return null;
            }

            return ratings;
        }

        public async Task<NewUserDTO> GetLoggedUser(ApplicationUser user)
        {
            if(user != null)
            {
                if(await userManager.IsInRoleAsync(user, "User"))
                {
                    var loggedUser = new NewUserDTO
                    {
                        Username = user.Email.Split("@")[0],
                        Email = user.Email,
                        Role = "User",
                        Token = tokenService.CreateToken(user)
                    };
                    return loggedUser;
                }
                else
                {
                    var loggedUser = new NewUserDTO
                    {
                        Username = user.Email.Split("@")[0],
                        Email = user.Email,
                        Role = "Admin",
                        Token = tokenService.CreateToken(user)
                    };
                    return loggedUser;
                }
            }
            return null;  
        }

        public async Task<SignInResult> TrySignIn(ApplicationUser user, string password)
        {
            var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
            return result;
        }
    }
}
