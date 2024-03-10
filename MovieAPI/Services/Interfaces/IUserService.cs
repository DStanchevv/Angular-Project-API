using Microsoft.AspNetCore.Identity;
using MovieAPI.Data.Models;
using MovieAPI.ViewModels;

namespace MovieAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<GetAllUserCommentsDTO>> GetAllUserComments(string userId);
        Task<List<GetAllUserRatingsDTO>> GetAllUserRatings(string userId);
        Task<ApplicationUser> FindUser(string username);
        Task<SignInResult> TrySignIn(ApplicationUser user, string password);
        NewUserDTO GetLoggedUser(ApplicationUser user);
        Task<NewUserDTO> CreateUser(RegisterDTO register);
    }
}
