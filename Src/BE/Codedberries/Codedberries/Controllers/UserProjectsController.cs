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

    }
}
