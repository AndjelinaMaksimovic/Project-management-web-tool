using Codedberries.Helpers;
using Codedberries.Models.DTOs;
using Codedberries.Models;
using Codedberries.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace Codedberries.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController:ControllerBase
    {
        private readonly TaskService _taskService;

        public TaskController(TaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost("createNewTask")]
        public async Task<IActionResult> CreateTaks([FromBody] TaskCreationRequestDTO body)
        {
            try
            {
                await _taskService.CreateTask(HttpContext, body);

                return Ok("Task successfully created.");
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
        }

        [HttpGet("projectTasks")]
        public IActionResult GetProjectTasksWithFilters([FromQuery] TaskFilterParamsDTO filterParams)
        {
            try
            {
                var tasks = _taskService.GetTasksByFilters(HttpContext, filterParams);

                return Ok(tasks);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMsg(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorMsg("An error occurred while processing your request."));
            }
        }

        [HttpDelete("tasksDeletion")]
        public IActionResult DeleteTask([FromBody] TaskDeletionDTO deletionDTO)
        {
            try
            {
                _taskService.DeleteTask(HttpContext, deletionDTO.TaskId);

                return Ok("Task successfully deleted.");
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ErrorMsg(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMsg(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred while deleting the task: {ex.Message}"));
            }
        }

        [HttpPut("updateTask")]
        public async Task<IActionResult> UpdateTask([FromBody] TaskUpdateRequestDTO request)
        {
            try
            {
                var updatedTaskInfo = await _taskService.UpdateTask(HttpContext, request);

                return Ok(updatedTaskInfo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new ErrorMsg(ex.Message)); // unauthorized
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred while updating the task: {ex.Message}"));
            }
        }

        [HttpPut("archiveTask")]
        public async Task<IActionResult> ArchiveTask(int taskId)
        {
            try
            {
                var TaskInfo=await _taskService.ArchiveTask(taskId);

                return Ok(TaskInfo);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ErrorMsg(ex.Message)); // Task not found
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred while archiving/unarchiving the task: {ex.Message}"));
            }
        }
    }
}
