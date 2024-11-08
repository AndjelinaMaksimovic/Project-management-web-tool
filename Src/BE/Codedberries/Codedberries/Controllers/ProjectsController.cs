﻿using Codedberries.Helpers;
using Codedberries.Models.DTOs;
using Codedberries.Models;
using Codedberries.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codedberries.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectService _projectService;

        public ProjectsController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost("createNewProject")]
        public async Task<IActionResult> CreateProject([FromBody] ProjectCreationRequestDTO body)
        {
            try
            {
                ProjectIdDTO projectId=await _projectService.CreateProject(HttpContext, body);

                return Ok(projectId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new ErrorMsg(ex.Message)); // does not have permission
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message)); // user not found
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred while creating the project: {ex.Message}"));
            }
        }

        // all active projects
        [HttpGet("allProjects")]
        public IActionResult GetAllProjects()
        {
            try
            {
                AllProjectsDTO activeProjectsDTO = _projectService.GetActiveProjects(HttpContext);
                
                return Ok(activeProjectsDTO);
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
                return StatusCode(500, new ErrorMsg($"An error occurred while getting all active projects: {ex.Message}"));
            }
        }

        // all archieved projects
        [HttpGet("allArchivedProjects")]
        public IActionResult GetArchivedProjects()
        {
            try
            {
                var archivedProjects = _projectService.GetArchivedProjects(HttpContext);

                return Ok(archivedProjects);
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
                return StatusCode(500, new ErrorMsg($"An error occurred while fetching archived projects: {ex.Message}"));
            }
        }

        [HttpDelete("projectDeletion")]
        public IActionResult DeleteProject([FromBody] ProjectDeletionDTO body)
        {
            try
            {
                _projectService.DeleteProject(HttpContext, body.ProjectId);

                return Ok("Project successfully deleted.");
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ErrorMsg(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred while deleting the project: {ex.Message}"));
            }
        }

        [HttpGet("filterProjects")]
        public IActionResult FilterProjects([FromQuery] ProjectFilterDTO filters)
        {
            try
            {
                var projects = _projectService.GetFilteredProjects(HttpContext, filters);

                return Ok(projects);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred while fetching projects: {ex.Message}"));
            }
        }

        [HttpPut("updateProject")]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectUpdateRequestDTO request)
        {
            try
            {
                await _projectService.UpdateProject(HttpContext, request);

                return Ok("Project updated succesfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorMsg(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred while updating the task: {ex.Message}"));
            }
        }

        [HttpPut("archiveProject")]
        public async Task<IActionResult> ArchiveProject([FromBody] ProjectDeletionDTO body)
        {
            try
            {
                await _projectService.ArchiveProject(HttpContext, body.ProjectId);

                return Ok("Project succesfully archieved.");
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred while archiveing the project: {ex.Message}"));
            }
        }

        [HttpGet("getProjectProgress")]
        public async Task<IActionResult> GetProjectProgress([FromQuery] ProjectIdDTO request)
        {
            try
            {
                var projectProgress = await _projectService.GetProjectProgress(HttpContext, request);

                return Ok(projectProgress);
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
                return StatusCode(500, new ErrorMsg($"An error occurred while calculating the progress: {ex.Message}"));
            }
        }

        // starred/unstarred
        [HttpPost("toggleStarredProject")]
        public async Task<IActionResult> ToggleStarredProject([FromBody] ProjectIdDTO request)
        {
            try
            {
                await _projectService.ToggleStarredProject(HttpContext, request);

                return Ok("Starring/unstarring successful.");
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
                return StatusCode(500, new ErrorMsg($"An error occurred while starring/unstarring the project: {ex.Message}"));
            }
        }

        // get starred projects by current session user
        [HttpGet("getStarredProjects")]
        public async Task<IActionResult> GetStarredProjects()
        {
            try
            {
                var starredProjects = await _projectService.GetStarredProjectsByUserId(HttpContext);

                return Ok(starredProjects);
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
                return StatusCode(500, new ErrorMsg($"An error occurred while getting Starred projects: {ex.Message}"));
            }
        }

        [HttpPost("allProjectActivities")]
        public async Task<IActionResult> GetAllProjectActivity([FromBody] ProjectIdDTO request)
        {
            try
            {
                var activities = await _projectService.GetAllProjectActivity(HttpContext, request);

                return Ok(activities);
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

        [HttpPost("allUserActivities")]
        public async Task<IActionResult> GetAllUserActivity()
        {
            try
            {
                var activities = await _projectService.GetAllUserActivity(HttpContext);

                return Ok(activities);
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

        [HttpPost("allUserActivitiesById")]
        public async Task<IActionResult> GetAllUserActivityById([FromBody] UserActivityDTO request)
        {
            try
            {
                var activities = await _projectService.GetAllUserActivity(HttpContext, request);

                return Ok(activities);
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

        [HttpPost("allUsersProjectActivities")]
        public async Task<IActionResult> GetProjectUsersActivity()
        {
            try
            {
                var activities = await _projectService.GetActivitiesByUsersProjects(HttpContext);

                return Ok(activities);
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

        [HttpPost("NotificationsSeen")]
        public async Task<IActionResult> NotificationSeen()
        {
            try
            {
                await _projectService.NotificationsSeen(HttpContext);
                return Ok("All notifications Seen");
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
