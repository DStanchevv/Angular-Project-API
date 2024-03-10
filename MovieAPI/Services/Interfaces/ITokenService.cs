
using MovieAPI.Data.Models;

namespace MovieAPI.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user);
    }
}
