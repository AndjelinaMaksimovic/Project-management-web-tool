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
    }
}
