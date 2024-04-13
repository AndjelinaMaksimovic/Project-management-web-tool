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
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorMsg($"An error occurred while creating the status: {ex.Message}"));
            }
        }

        [HttpGet("getStatus")]
        public IActionResult GetStatusByProjectId([FromQuery] StatusProjectIdDTO request)
        {
            try
            {
                var statuses = _statusService.GetStatusByProjectId(HttpContext, request);

                return Ok(statuses);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMsg(ex.Message));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
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

        [HttpDelete("deleteStatus")]
        public async Task<IActionResult> DeleteStatus(StatusDeletionDTO request)
        {
            try
            {
                await _statusService.DeleteStatusesByProjectId(HttpContext, request);

                return Ok("Status successfully deleted.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMsg(ex.Message));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ErrorMsg(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred while deleting status: {ex.Message}"));
            }
        }

        [HttpPost("changeOrder")]
        public async Task<IActionResult> ChangeStatusesOrder([FromBody] StatusOrderChangeDTO request)
        {
            try
            {
                await _statusService.ChangeStatusesOrder(HttpContext, request);
                
                return Ok("Statuses order changed successfully.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(401, ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }
    }
}
