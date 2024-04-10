using Codedberries.Helpers;
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
                return NotFound(new ErrorMsg("User not found!"));
            }

            return Ok(userFullName);
        }

        [HttpPost("userRole")]
        public IActionResult GetUserRole(UserIdDTO body)
        {
            UserRoleDTO userRole = _userService.GetUserRole(body.UserId);

            if (userRole == null)
            {
                return NotFound(new ErrorMsg("User role not found!"));
            }

            return Ok(userRole);
        }

        [HttpPost("setProfilePicture")]
        public IActionResult SetProfilePicture(ProfilePictureDTO body)
        {
            bool isSet = _userService.SetProfilePicture(body.UserId, body.ProfilePicture);

            if (isSet == false)
            {
                return NotFound(new ErrorMsg("User not found!"));
            }

            return Ok(new { resp = "Success" });
        }

        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsers([FromQuery] UserFilterDTO request)
        {
            try
            {
                var users = await _userService.GetUsers(HttpContext, request);

                return Ok(users);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMsg(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new ErrorMsg(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred: {ex.Message}"));
            }
        }
    }
}
