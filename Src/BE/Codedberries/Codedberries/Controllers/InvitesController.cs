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

namespace Codedberries.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvitesController : ControllerBase
    {
        private readonly Config _config;
        private readonly AppDatabaseContext _databaseContext;
        private readonly TokenService _tokenService;

        public InvitesController(IOptions<Config> config, AppDatabaseContext context, TokenService tokenService)
        {
            _config = config.Value;
            _databaseContext = context;
            _tokenService = tokenService;
        }

        [HttpPost("SendInvite/{email}/{roleId}")]
        public IActionResult AddInvite(string email, int roleId)
        {
            if (Helper.IsEmailValid(email))
            {
                Invite invite = new Invite();
                invite.Email = email;
                invite.Token = _tokenService.GenerateToken(email);
                invite.RoleId = roleId;

                _databaseContext.Invites.Add(invite);
                _databaseContext.SaveChanges();

                MailService mailService = new MailService(_config.SmtpHost, _config.SmtpPort, _config.SmtpUsername, _config.SmtpPassword);
                mailService.SendMessage(email, "Invite", ""); // TODO - Add invite link

                return Ok("Success");
            }
            else
            {
                return BadRequest("Invalid email");
            }
        }

        [HttpPost("AcceptInvite/{token}/{email}")]
        public IActionResult AcceptInvite(string token, string email)
        {
            Invite? invite = _databaseContext.Invites.FirstOrDefault(x => x.Token == token && x.Email == email);
            if (invite != null)
            {
                // TO DO - USER REGISTRATION
                _databaseContext.Invites.Remove(invite);
                _databaseContext.SaveChanges();

                return Ok("Success");
            }
            return BadRequest("Invalid token");
        }
    }
}
