using Codedberries.Models;
using Codedberries.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codedberries.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthenticationController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login/{email}/{password}")]
        public IActionResult Login(string email, string password)
        {
            Session currentSession = _userService.LoginUser(email, password);

            if (currentSession != null)
            {
                return Ok(new { sessionId = currentSession.Token });
            }

            return Unauthorized(new { message = "Login fail!" });
        }

        [HttpPost("logout/{token}")]
        public IActionResult Logout(string token)
        {
            if (_userService.LogoutUser(token))
            {
                return Ok(new { message = "Logout successful!" });
            }
            return BadRequest(new { message = "Logout fail!" });
        }
    }
}
