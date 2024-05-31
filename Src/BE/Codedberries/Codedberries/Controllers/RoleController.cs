using Codedberries.Helpers;
using Codedberries.Models.DTOs;
using Codedberries.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codedberries.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;

        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("allRoles")]
        public IActionResult GetAllRoles()
        {
            List<RolePermissionDTO> allRolesNames = _roleService.GetRoles();

            if (allRolesNames == null)
            {
                return NotFound(new ErrorMsg("No roles found!"));
            }

            return Ok(allRolesNames);
        }

        [HttpPost("createCustomRole")]
        public async Task<IActionResult> CreateCustomRole([FromBody] CustomRoleDTO body)
        {
            try
            {
                var result = await _roleService.AddNewCustomRole(HttpContext, body);

                if (result)
                {
                    return Ok("Custom role created successfully!");
                }
                else
                {
                    return BadRequest(new ErrorMsg("Failed to create custom role!"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorMsg("Internal server error: " + ex.Message));
            }
        }
    }
}
