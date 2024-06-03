using Microsoft.AspNetCore.Mvc;
using Codedberries.Models;
using System.ComponentModel.DataAnnotations;
using Codedberries.Helpers;
using System.IdentityModel.Tokens.Jwt;
using Codedberries.Services;
using System.Net.Mail;
using System.Net;
using Codedberries.Environment;
using Microsoft.Extensions.Options;
using Codedberries.Models.DTOs;

namespace Codedberries.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvitesController : ControllerBase
    {
        private readonly Config _config;
        private readonly AppDatabaseContext _databaseContext;
        private readonly InviteService _inviteService;

        public InvitesController(IOptions<Config> config, AppDatabaseContext context, InviteService inviteService)
        {
            _config = config.Value;
            _databaseContext = context;
            _inviteService = inviteService;
        }

        [HttpPost("SendInvite")]
        public IActionResult AddInvite([FromBody] SendInviteDTO body)
        {
            try
            {
                _inviteService.AddUser(HttpContext, body);

                return Ok("User successfully created.");
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
                return StatusCode(500, new ErrorMsg($"An error occurred while creating new User: {ex.InnerException}"));
            }
        }

        [HttpPost("AcceptInvite")]
        public IActionResult AcceptInvite([FromBody] ActivateAccountDTO body)
        {
            try
            {
                _inviteService.AcceptInvite(HttpContext, body);

                return Ok("User successfully activated.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred while activating account: {ex.Message}"));
            }
        }

        [HttpPost("CheckInvite")]
        public IActionResult CheckInvite([FromBody] CheckInviteDTO body)
        {
            try
            {
                _inviteService.CheckInvite(HttpContext, body);

                return Ok("User successfully activated.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorMsg($"An error occurred while checking token: {ex.Message}"));
            }
        }
    }
}
