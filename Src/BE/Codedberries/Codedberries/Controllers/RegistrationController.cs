using Codedberries.Helpers;
using Codedberries.Models;
using Codedberries.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace Codedberries.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly AppDatabaseContext _databaseContext;

        public RegistrationController(AppDatabaseContext context)
        {
            _databaseContext = context;
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
                user.ActivationToken = TokenService.GenerateToken(email);

                _databaseContext.Users.Add(user);
                _databaseContext.SaveChanges();

                string activationLink = "https://localhost:7167/Registration/Activate/" + user.ActivationToken + "/" + email; // CHANGE WITH FRONTEND URL

                MailService mailService = new MailService("smtp.gmail.com", 587, "codedberries.pm@gmail.com", "vmzlvzehywdyjfal"); // CHANGE THIS
                mailService.SendMessage(email, "Account activation", EmailTemplates.ActivationEmail(firstname, lastname, activationLink));
            }
            catch (Exception ex)
            {
                return BadRequest("Error: " + ex.Message);
            }

            return Ok("Success");
        }

        [HttpPost("Activate/{token}/{email}/{password}")]
        public IActionResult ActivateAccount(string token, string email, string password)
        {
            if(!TokenService.ValidateToken(token)) return BadRequest("Invalid token");

            User? user = _databaseContext.Users.FirstOrDefault(x => x.Activated == false && x.ActivationToken == token && x.Email == email);
            if (user != null)
            {
                user.Password = password;
                user.Activated = true;
                user.ActivationToken = null;
                _databaseContext.SaveChanges();

                return Ok("Success");
            }
            return BadRequest("User not found");
        }
    }
}
