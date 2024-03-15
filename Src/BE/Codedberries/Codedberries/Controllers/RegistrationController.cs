using Codedberries.Environment;
using Codedberries.Helpers;
using Codedberries.Models;
using Codedberries.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace Codedberries.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly Config _config;
        private readonly AppDatabaseContext _databaseContext;
        private readonly TokenService _tokenService;

        public RegistrationController(IOptions<Config> config, AppDatabaseContext context, TokenService tokenService)
        {
            _config = config.Value;
            _databaseContext = context;
            _tokenService = tokenService;
        }

        [HttpPost("CreateUser/{email}/{firstname}/{lastname}")]
        public IActionResult CreateUser(string email, string firstname, string lastname)
        {
            // TODO - Check if the logged user is a Super Admin
            if (!Helper.IsEmailValid(email)) return BadRequest("Invalid email");
            if(_databaseContext.Users.FirstOrDefault(u => u.Email == email) != null) return BadRequest("User with the same email already exists");

            try
            {
                User user = new User(email, "123", firstname, lastname, null);
                user.ActivationToken = _tokenService.GenerateToken(email);

                _databaseContext.Users.Add(user);
                _databaseContext.SaveChanges();

                string activationLink = _config.FrontendURL + "/activate?token=" + user.ActivationToken + "&email=" + email; // CHANGE WITH FRONTEND URL

                MailService mailService = new MailService(_config.SmtpHost, _config.SmtpPort, _config.SmtpUsername, _config.SmtpPassword);
                mailService.SendMessage(email, "Account activation", EmailTemplates.ActivationEmail(firstname, lastname, activationLink));
            }
            catch (Exception ex)
            {
                return BadRequest(new { resp = "Error: " + ex.Message});
            }

            return Ok(new { resp = "Success" });
        }

        [HttpPost("Activate/{token}/{email}/{password}")]
        public IActionResult ActivateAccount(string token, string email, string password)
        {
            if(!_tokenService.ValidateToken(token)) return BadRequest("Invalid token");

            User? user = _databaseContext.Users.FirstOrDefault(x => x.Activated == false && x.ActivationToken == token && x.Email == email);
            if (user != null)
            {
                user.Password = password;
                user.Activated = true;
                user.ActivationToken = null;
                _databaseContext.SaveChanges();

                return Ok(new { resp = "Success" });
            }
            return BadRequest(new { resp = "User not found" });
        }
    }
}
