using Codedberries.Helpers;
using Codedberries.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codedberries.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriorityController : ControllerBase
    {
        private readonly PriorityService _priorityService;

        public PriorityController(PriorityService priorityService)
        {
            _priorityService = priorityService;
        }

        [HttpGet("allPriorities")]
        public async Task<IActionResult> GetAllPriorities()
        {
            try
            {
                var priorities = await _priorityService.GetAllPriorities(HttpContext);

                return Ok(priorities);
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
