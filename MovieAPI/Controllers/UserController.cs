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

            var loggedUser = userService.GetLoggedUser(user);

            HttpContext.Response.Cookies.Append("token", loggedUser.Token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddDays(7),
                SameSite = SameSiteMode.Lax,
                Secure = false,
                IsEssential = true,
                Path = "/"
                // You can customize other properties of the cookie like expiration, domain, etc. here
            });

            return Ok();
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
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok("Logged out successfully");
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
