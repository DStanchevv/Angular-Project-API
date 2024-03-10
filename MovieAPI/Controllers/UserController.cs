using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Data.Models;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userService.FindUser(login.UserName);

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
            return Ok(loggedUser);
        }

        [HttpPost("register")]
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
