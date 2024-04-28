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
        public async Task<IActionResult> SetProfilePicture(ProfilePictureDTO body)
        {
            try
            {
                await _userService.SetProfilePicture(HttpContext, body);

                return Ok("Profile picture successfully updated.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMsg(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorMsg($"An error occurred while processing your request: {ex.Message}"));
            }
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

        [HttpGet("getUser")]
        public async Task<IActionResult> GetUser([FromQuery] UserIdDTO request)
        {
            try
            {
                var user =  _userService.GetUser(HttpContext, request);

                return Ok(user);
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

        [HttpGet("getMyData")]  // current session user info
        public async Task<IActionResult> GetCurrentSessionUser()
        {
            try
            {
                var currentSessionUser = await _userService.GetCurrentSessionUserData(HttpContext);
                
                return Ok(currentSessionUser);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred: {ex.Message}"));
            }
        }
    }
}
