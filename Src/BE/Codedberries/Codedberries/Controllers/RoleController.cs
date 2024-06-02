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
            try
            {
                List<RolePermissionDTO> allRolesNames = _roleService.GetRoles(HttpContext);

                if (allRolesNames == null)
                {
                    return NotFound(new ErrorMsg("No roles found!"));
                }

                return Ok(allRolesNames);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ErrorMsg(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorMsg("Internal server error: " + ex.Message));
            }
        }

        [HttpPost("createCustomRole")]
        public async Task<IActionResult> CreateCustomRole([FromBody] CustomRoleDTO body)
        {
            try
            {
                var result = await _roleService.AddNewCustomRole(HttpContext, body);

                if (result.RoleId>0)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(new ErrorMsg("Failed to create custom role!"));
                }
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
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorMsg("Internal server error: " + ex.Message));
            }
        }
    }
}
