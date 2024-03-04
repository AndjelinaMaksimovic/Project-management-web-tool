using Microsoft.AspNetCore.Mvc;
using Codedberries.Models;
using System.ComponentModel.DataAnnotations;
using Codedberries.Helpers;
using System.IdentityModel.Tokens.Jwt;
using Codedberries.Services;
using System.Net.Mail;
using System.Net;

namespace Codedberries.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvitesController : ControllerBase
    {

        private readonly AppDatabaseContext _databaseContext;

        public InvitesController(AppDatabaseContext context)
        {
            _databaseContext = context;
        }

        [HttpPost("SendInvite/{email}/{roleId}")]
        public IActionResult AddInvite(string email, int roleId)
        {
            if (Helper.IsEmailValid(email))
            {
                Invite invite = new Invite();
                invite.Email = email;
                invite.Token = TokenService.GenerateToken(email);
                invite.RoleId = roleId;

                _databaseContext.Invites.Add(invite);
                _databaseContext.SaveChanges();

                MailService mailService = new MailService("smtp.gmail.com", 587, "EMAIL_HERE", "PASSWORD_HERE"); // CHANGE THIS
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
