﻿using Codedberries.Helpers;
using Codedberries.Models.DTOs;
using Codedberries.Models;
using Codedberries.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred while creating new Task: {ex.Message}"));
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
        public IActionResult ArchiveTask([FromBody] TaskDeletionDTO body)
        {
            try
            {
                 _taskService.ArchiveTask(HttpContext,body.TaskId);

                return Ok("Task succesfully archieved");
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

        [HttpPost("createNewTaskComment")]
        public async Task<IActionResult> CreateTaskComment([FromBody] TaskCommentCreationRequestDTO body)
        {
            try
            {
                await _taskService.CreateTaskComment(HttpContext, body);

                return Ok("TaskComment successfully created.");
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

        [HttpGet("TaskComments")]
        public async Task<IActionResult> GetTaskComments([FromQuery] TaskIdDTO body)
        {
            try
            {
                var comments = await _taskService.GetTasksComments(HttpContext, body);

                return Ok(comments);
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

        [HttpPost("createTaskDependency")]
        public async Task<IActionResult> CreateTaskDependency([FromBody] TaskDependencyRequestDTO request)
        {
            try
            {
                await _taskService.CreateTaskDependency(HttpContext, request);

                return Ok("Task dependency created successfully.");
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
                return Conflict(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorMsg($"An error occurred: {ex.Message}"));
            }
        }

        [HttpDelete("deleteTaskDependency")]
        public async Task<IActionResult> DeleteTaskDependencies([FromBody] TaskDependencyDeletionDTO request)
        {
            try
            {
                await _taskService.DeleteTaskDependencies(HttpContext, request);

                return Ok("Task dependency deleted successfully.");
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
                return Conflict(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorMsg($"An error occurred: {ex.Message}"));
            }
        }

        [HttpPost("changeTaskProgress")]
        public async Task<IActionResult> ChangeTaskProgress(TaskProgressDTO request)
        {
            try
            {
                await _taskService.ChangeTaskProgress(HttpContext, request);

                return Ok("Task progress updated successfully.");
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
                return StatusCode(500, new ErrorMsg($"An error occurred while updating task progress: {ex.Message}"));
            }
        }
    }
}
