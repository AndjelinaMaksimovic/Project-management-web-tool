using Codedberries.Models.DTOs;
using Codedberries.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codedberries.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly RequestsService _requestsService;

        public RequestsController(RequestsService requestsService)
        {
            _requestsService = requestsService;
        }

        [HttpPost("userFullName")]
        public IActionResult GetUserFullName(UserIdDTO body)
        {
            UserFullNameDTO userFullName = _requestsService.GetUserFullNameById(body.UserId);
            
            if (userFullName == null)
            {
                return NotFound(new { message = "User not found!" }); // TO-DO ErrorDTO
            }

            return Ok(userFullName);
        }

        [HttpGet("allRolesNames")]
        public IActionResult GetAllRolesNames()
        {
            AllRolesNamesDTO allRolesNames = _requestsService.GetRoleNames();
            
            if (allRolesNames == null)
            {
                return NotFound(new { message = "No roles found!" }); // TO-DO ErrorDTO
            }

            return Ok(allRolesNames);
        }
    }
}
