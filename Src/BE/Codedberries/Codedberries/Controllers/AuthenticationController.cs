using Codedberries.Models;
using Codedberries.Models.DTOs;
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

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO body)
        {
            Session currentSession = _userService.LoginUser(body.Email, body.Password);

            if (currentSession != null)
            {
                _userService.CreateSessionCookie(HttpContext, currentSession);
                return Ok(new { sessionId = currentSession.Token });
            }

            return Unauthorized(new { message = "Login fail!" });
        }

        [HttpPost("logout")]
        public IActionResult Logout([FromBody] LogoutDTO body)
        {
            if (_userService.LogoutUser(body.Token))
            {
                return Ok(new { message = "Logout successful!" });
            }
            return BadRequest(new { message = "Logout fail!" });
        }
    }
}
