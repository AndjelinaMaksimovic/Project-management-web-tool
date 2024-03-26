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
                Models.Task task = await _taskService.CreateTask(HttpContext, body);

                TaskInfoDTO newTaskInfoDTO = new TaskInfoDTO();
                newTaskInfoDTO.Id = task.Id;
                newTaskInfoDTO.Description = task.Description;
                newTaskInfoDTO.DueDate = task.DueDate;

                return Ok(newTaskInfoDTO);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new ErrorMsg(ex.Message)); // does not have permission
            }
        }

        [HttpGet("projectTasks")]
        public IActionResult GetProjectTasksWithFilters([FromQuery] TaskFilterParamsDTO filterParams)
        {
            try
            {
                var tasks = _taskService.GetTasksByFilters(filterParams);

                return Ok(tasks);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("tasksDeletion")]
        public IActionResult DeleteTask([FromBody] TaskDeletionDTO deletionDTO)
        {
            try
            {
                _taskService.DeleteTask(deletionDTO.TaskId);

                return Ok("Task successfully deleted.");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the task: {ex.Message}");
            }
        }
    }
}
