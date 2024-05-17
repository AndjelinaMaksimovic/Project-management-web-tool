using Codedberries.Helpers;
using Codedberries.Models.DTOs;
using Codedberries.Services;
using Microsoft.AspNetCore.Mvc;

namespace Codedberries.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MilestoneController:ControllerBase
    {
        private readonly MilestoneService _milestoneService;

        public MilestoneController(MilestoneService milestoneService)
        {
            _milestoneService = milestoneService;
        }

        [HttpPost("createNewMilestone")]
        public async Task<IActionResult> CreateMilestone([FromBody] MilestoneCreationRequestDTO body)
        {
            try
            {
                await _milestoneService.CreateMilestone(HttpContext, body);

                return Ok("Milestone successfully created.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new ErrorMsg(ex.Message)); // does not have permission
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred while creating new Milestone: {ex.Message}"));
            }
        }

        [HttpPost("allProjectMilestones")]
        public async Task<IActionResult> GetAllProjectMilestones([FromBody] ProjectIdDTO request)
        {
            try
            {
                var milestones = await _milestoneService.GetAllMylestonesByProjects(HttpContext, request);

                return Ok(milestones);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMsg(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred: {ex.Message}"));
            }
        }
    }
}
