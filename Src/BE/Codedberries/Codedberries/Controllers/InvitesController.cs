﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly TokenService _tokenService;

        public InvitesController(IOptions<Config> config, AppDatabaseContext context, TokenService tokenService)
        {
            _config = config.Value;
            _databaseContext = context;
            _tokenService = tokenService;
        }

        [HttpPost("SendInvite")]
        public IActionResult AddInvite([FromBody] SendInviteDTO body)
        {
            if (Helper.IsEmailValid(body.Email))
            {
                Invite invite = new Invite();
                invite.Email = body.Email;
                invite.Token = _tokenService.GenerateToken(body.Email);
                invite.RoleId = body.RoleId;

                _databaseContext.Invites.Add(invite);
                _databaseContext.SaveChanges();

                MailService mailService = new MailService(_config.SmtpHost, _config.SmtpPort, _config.SmtpUsername, _config.SmtpPassword);
                mailService.SendMessage(body.Email, "Invite", ""); // TODO - Add invite link

                return Ok(new { resp = "Success" });
            }
            else
            {
                return BadRequest(new ErrorMsg("Invalid email"));
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
            return BadRequest(new ErrorMsg("Invalid token"));
        }
    }
}
