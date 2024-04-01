using Codedberries.Helpers;
using Codedberries.Models.DTOs;
using Codedberries.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codedberries.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly StatusService _statusService;

        public StatusController(StatusService statusService)
        {
            _statusService = statusService;
        }

        [HttpPost("createStatus")]
        public async System.Threading.Tasks.Task<IActionResult> CreateStatus([FromBody] StatusCreationDTO statusDTO)
        {
            try
            {
                await _statusService.CreateStatus(HttpContext, statusDTO);

                return Ok("Status successfully created.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorMsg($"An error occurred while creating the status: {ex.Message}"));
            }
        }

        [HttpGet("getStatus")]
        public IActionResult GetStatusByProjectId(StatusProjectIdDTO request)
        {
            try
            {
                var status = _statusService.GetStatusByProjectId(HttpContext, request);

                return Ok(status);
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
                return StatusCode(500, new ErrorMsg($"An error occurred while fetching statuses: {ex.Message}"));
            }
        }
    }
}
