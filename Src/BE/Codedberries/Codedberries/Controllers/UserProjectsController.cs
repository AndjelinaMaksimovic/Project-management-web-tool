using Codedberries.Helpers;
using Codedberries.Models.DTOs;
using Codedberries.Models;
using Codedberries.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codedberries.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProjectsController : ControllerBase
    {
        private readonly UserProjectsService _userProjectService;

        public UserProjectsController(UserProjectsService userProjectService)
        {
            _userProjectService = userProjectService;
        }

        // gets all users on project with provided id
        [HttpGet("getUsersOnProject")]
        public async Task<IActionResult> GetUsersByProjectId([FromQuery] ProjectIdDTO request)
        {
            try
            {
                var userProjectDTOs = await _userProjectService.GetUsersByProjectId(HttpContext, request);

                return Ok(userProjectDTOs);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorMsg($"An error occurred while processing your request: {ex.Message}"));
            }
        }

        // get all projects that user is on by its id
        [HttpGet("userProjects")]
        public async Task<IActionResult> GetUserProjects([FromQuery] UserIdDTO request)
        {
            try
            {
                var userProjectsInformation = await _userProjectService.GetUserProjectsInformation(HttpContext, request);
                
                return Ok(userProjectsInformation);
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
                return StatusCode(500, new ErrorMsg($"An error occurred while fetching user projects: {ex.Message}"));
            }
        }

        [HttpDelete("removeUserFromProject")]
        public async Task<IActionResult> RemoveUserFromProject(DeleteUserFromProjectDTO request)
        {
            try
            {
                await _userProjectService.RemoveUserFromProject(HttpContext, request);
             
                return Ok("User successfully removed from project.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMsg(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ErrorMsg(ex.Message ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred while removing user from project: {ex.Message}"));
            }
        }
    }
}
