using Codedberries.Helpers;
using Codedberries.Models;
using Codedberries.Models.DTOs;
using Codedberries.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace Codedberries.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly AppDatabaseContext _databaseContext;

        public RegistrationController(AppDatabaseContext context)
        {
            _databaseContext = context;
        }

        [HttpPost("CreateUser")]
        public IActionResult CreateUser([FromBody] CreateUserDTO body)
        {
            // TODO - Check if the logged user is a Super Admin
            if (!Helper.IsEmailValid(body.Email)) return BadRequest("Invalid email"); /* TO-DO ErrorMessageDTO */
            if(_databaseContext.Users.FirstOrDefault(u => u.Email == body.Email) != null) return BadRequest("User with the same email already exists");

            try
            {
                User user = new User(body.Email, "123", body.FirstName, body.LastName, null);
                user.ActivationToken = TokenService.GenerateToken(body.Email);

                _databaseContext.Users.Add(user);
                _databaseContext.SaveChanges();

                string activationLink = "http://localhost:4200/activate?token="+ user.ActivationToken + "&email=" + body.Email; // CHANGE WITH FRONTEND URL

                MailService mailService = new MailService("smtp.gmail.com", 587, "codedberries.pm@gmail.com", "vmzlvzehywdyjfal"); // CHANGE THIS
                mailService.SendMessage(body.Email, "Account activation", EmailTemplates.ActivationEmail(body.FirstName, body.LastName, activationLink));
            }
            catch (Exception ex)
            {
                return BadRequest(new { resp = "Error: " + ex.Message}); /* TO-DO ErrorMessageDTO */
            }

            return Ok(new { resp = "Success" });
        }

        [HttpPost("Activate")]
        public IActionResult ActivateAccount([FromBody] ActivateAccountDTO body)
        {
            if(!TokenService.ValidateToken(body.Token)) return BadRequest("Invalid token"); /* TO-DO ErrorMessageDTO */

            User? user = _databaseContext.Users.FirstOrDefault(x => x.Activated == false && x.ActivationToken == body.Token && x.Email == body.Email);
            if (user != null)
            {
                user.Password = body.Password;
                user.Activated = true;
                user.ActivationToken = null;
                _databaseContext.SaveChanges();

                return Ok(new { resp = "Success" });
            }
            return BadRequest(new { resp = "User not found" }); /* TO-DO ErrorMessageDTO */
        }
    }
}
