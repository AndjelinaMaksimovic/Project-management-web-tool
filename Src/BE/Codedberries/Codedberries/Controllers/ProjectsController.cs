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
                await _projectService.CreateProject(HttpContext, body);

                return Ok("Project successfully created.");
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

        [HttpGet("allProjects")]
        public IActionResult GetAllProjects()
        {
            AllProjectsDTO allProjectsDTO = _projectService.GetProjects();

            if (allProjectsDTO == null)
            {
                return NotFound(new ErrorMsg("No projects found!"));
            }

            return Ok(allProjectsDTO);
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
                var updatedProjectInfo = _projectService.UpdateProject(HttpContext, request);

                return Ok(updatedProjectInfo);
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
        public IActionResult ArchiveProject([FromBody] ProjectDeletionDTO body)
        {
            try
            {
                _projectService.ArchiveProject(HttpContext, body.ProjectId);

                return Ok("project succesfully archieved.");
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred while archiving/unarchiving the project: {ex.Message}"));
            }
        }

        [HttpGet("getProjectProgress")]
        public async Task<ActionResult<ProjectProgressDTO>> GetProjectProgress([FromQuery] ProjectIdDTO request)
        {

        }
    }
}
