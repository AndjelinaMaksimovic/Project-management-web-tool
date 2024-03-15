using Codedberries.Models.DTOs;
using Codedberries.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codedberries.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {   
            _userService = userService;
        }


        [HttpPost("userFullName")]
        public IActionResult GetUserFullName(UserIdDTO body)
        {
            UserFullNameDTO userFullName = _userService.GetUserFullNameById(body.UserId);

            if (userFullName == null)
            {
                return NotFound(new { message = "User not found!" }); // TO-DO ErrorDTO
            }

            return Ok(userFullName);
        }

        [HttpPost("userRole")]
        public IActionResult GetUserRole(UserIdDTO body)
        {
            UserRoleDTO userRole = _userService.GetUserRole(body.UserId);

            if (userRole == null)
            {
                return NotFound(new { message = "User role not found!" }); // TO-DO ErrorDTO
            }

            return Ok(userRole);
        }
    }
}
