using Microsoft.AspNetCore.Mvc;
using Codedberries.Models;
using System.ComponentModel.DataAnnotations;
using Codedberries.Helpers;
using System.IdentityModel.Tokens.Jwt;
using Codedberries.Services;
using System.Net.Mail;
using System.Net;
using Codedberries.Models.DTOs;

namespace Codedberries.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvitesController : ControllerBase
    {

        private readonly AppDatabaseContext _databaseContext;

        public InvitesController(AppDatabaseContext context)
        {
            _databaseContext = context;
        }

        [HttpPost("SendInvite")]
        public IActionResult AddInvite([FromBody] SendInviteDTO body)
        {
            if (Helper.IsEmailValid(body.Email))
            {
                Invite invite = new Invite();
                invite.Email = body.Email;
                invite.Token = TokenService.GenerateToken(body.Email);
                invite.RoleId = body.RoleId;

                _databaseContext.Invites.Add(invite);
                _databaseContext.SaveChanges();

                MailService mailService = new MailService("smtp.gmail.com", 587, "codedberries.pm@gmail.com", "vmzlvzehywdyjfal"); // CHANGE THIS
                mailService.SendMessage(body.Email, "Invite", ""); // TODO - Add invite link

                return Ok(new { resp = "Success" });
            }
            else
            {
                return BadRequest("Invalid email"); /* TO-DO ErrorMessageDTO */
            }
        }

        [HttpPost("AcceptInvite")]
        public IActionResult AcceptInvite([FromBody] AcceptInviteDTO body)
        {
            Invite? invite = _databaseContext.Invites.FirstOrDefault(x => x.Token == body.Token && x.Email == body.Email);
            if (invite != null)
            {
                // TO DO - USER REGISTRATION
                _databaseContext.Invites.Remove(invite);
                _databaseContext.SaveChanges();

                return Ok(new { resp = "Success" });
            }
            return BadRequest("Invalid token"); /* TO-DO ErrorMessageDTO */
        }
    }
}
