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
                Project project = await _projectService.CreateProject(HttpContext, body);

                ProjectInfoDTO newProjectInfoDTO = new ProjectInfoDTO();
                newProjectInfoDTO.Id = project.Id;
                newProjectInfoDTO.Name = project.Name;
                newProjectInfoDTO.Description = project.Description;

                return Ok(newProjectInfoDTO);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new ErrorMsg(ex.Message)); // does not have permission
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
                return StatusCode(500, $"An error occurred while deleting the project: {ex.Message}");
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
                return BadRequest(new ErrorMsg (ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching projects: {ex.Message}");
            }
        }
    }
}
