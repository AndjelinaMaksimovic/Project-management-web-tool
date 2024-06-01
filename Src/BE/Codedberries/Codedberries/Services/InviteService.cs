using Codedberries.Models.DTOs;
using Codedberries.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Codedberries.Helpers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System;
using Microsoft.Extensions.DependencyModel;
using Microsoft.AspNetCore.Mvc;
using Codedberries.Environment;
using System.ComponentModel;
using Microsoft.Extensions.Options;

namespace Codedberries.Services
{
    public class InviteService
    {
        private readonly Config _config;
        private readonly AppDatabaseContext _databaseContext;
        private readonly AuthorizationService _authorizationService;
        private readonly TokenService _tokenService;

        public InviteService(IOptions<Config> config, AppDatabaseContext databaseContext, AuthorizationService authorizationService, TokenService tokenService)
        {
            _config = config.Value;
            _databaseContext = databaseContext;
            _authorizationService = authorizationService;
            _tokenService = tokenService;
        }

        public void AddUser(HttpContext httpContext, SendInviteDTO body)
        {
            var userId = _authorizationService.GetUserIdFromSession(httpContext);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid session!");
            }

            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found!");
            }

            if (user.RoleId == null)
            {
                throw new UnauthorizedAccessException("User does not have any role assigned!");
            }

            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (userRole.CanEditUser == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to add user!");
            }

            if (_databaseContext.Users.FirstOrDefault(u => u.Email == body.Email) != null) new Exception("User with the same email already exists");

            if (!Helper.IsEmailValid(body.Email)) throw new Exception("Email is not valid!");

            string token = _tokenService.GenerateToken(body.Email);
            User newUser = new User(body.Email, "123", body.Firstname, body.Lastname, body.RoleId, null, token, false);

            _databaseContext.Users.Add(newUser);
            _databaseContext.SaveChanges();

            MailService mailService = new MailService(_config.SmtpHost, _config.SmtpPort, _config.SmtpUsername, _config.SmtpPassword);
            mailService.SendMessage(body.Email, "Invite", EmailTemplates.ActivationEmail(body.Firstname, body.Lastname, _config.FrontendURL + "/activate?email=" + body.Email + "&token=" + token + "&firstname=" + body.Firstname + "&lastname=" + body.Lastname)); // TODO - Add invite link
        }

        public void AcceptInvite(HttpContext httpContext, ActivateAccountDTO body)
        {
            User user = _databaseContext.Users.FirstOrDefault(x => x.ActivationToken == body.Token && x.Email == body.Email);
            if (user == null) throw new Exception("Invalid token!");
            
            user.SetPassword(body.Password);
            user.Activated = true;
            user.ActivationToken = null;
            _databaseContext.SaveChanges();
        }

        public void CheckInvite(HttpContext httpContext, CheckInviteDTO body)
        {
            User user = _databaseContext.Users.FirstOrDefault(x => x.ActivationToken == body.Token && x.Email == body.Email);
            if (user == null) throw new Exception("Invalid token!");
        }
    }
}
