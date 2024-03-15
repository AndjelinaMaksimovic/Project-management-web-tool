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

        [HttpGet("allRolesNames")]
        public IActionResult GetAllRolesNames()
        {
            AllRolesNamesDTO allRolesNames = _roleService.GetRoleNames();

            if (allRolesNames == null)
            {
                return NotFound(new { message = "No roles found!" }); // TO-DO ErrorDTO
            }

            return Ok(allRolesNames);
        }
    }
}
