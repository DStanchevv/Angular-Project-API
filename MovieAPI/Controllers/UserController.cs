using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Services.Interfaces;
using MovieAPI.ViewModels;
using System.Security.Claims;

namespace MovieAPI.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("/api/login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userService.FindUser(login.Email);

            if(user == null)
            {
                return Unauthorized("Invalid username!");
            }

            var result = await userService.TrySignIn(user, login.Password);
            
            if (!result.Succeeded)
            {
                return Unauthorized("Username/Password not found!");
            }

            var loggedUser = await userService.GetLoggedUser(user);

            HttpContext.Response.Cookies.Append("token", loggedUser.Token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(7),
                SameSite = SameSiteMode.Lax,
                Secure = false,
                IsEssential = true,
                Path = "/"
            });

            return Ok(loggedUser);
        }

        [HttpPost("/api/register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdUser = await userService.CreateUser(register);

                if (createdUser != null)
                {
                    return Ok(createdUser);
                }
                else
                {
                    return BadRequest("Something went wrong!");
                }
            }
            catch (Exception e)
            {
                return BadRequest($"Something went wrong!");
            }
        }

        [HttpPost("/api/logout")]
        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("token", new CookieOptions { Path = "/" });
            return Ok("Logged out successfully");
        }

        [HttpGet("/api/profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var user = await userService.FindUserById(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        [HttpGet("get-all-comments")]
        public async Task<IActionResult> GetAllUserComments()
        {
            var comments = await userService.GetAllUserComments(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if(comments == null)
            {
                return NotFound();
            }

            return Ok(comments);
        }
        
        [HttpGet("get-all-ratings")]
        public async Task<IActionResult> GetAllUserRatings()
        {
            var ratings = await userService.GetAllUserRatings(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (ratings == null)
            {
                return NotFound();
            };

            return Ok(ratings);
        }

    }
}
