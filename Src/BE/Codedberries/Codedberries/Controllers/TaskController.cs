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
    }
}
