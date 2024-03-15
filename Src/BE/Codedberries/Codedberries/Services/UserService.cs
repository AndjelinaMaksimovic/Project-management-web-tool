using Codedberries.Models;
using Codedberries.Models.DTOs;
using System.Security.Cryptography;

namespace Codedberries.Services
{
    public class UserService
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 10000;

        private readonly AppDatabaseContext _databaseContext;
        private const int SessionDurationHours = 24;

        public UserService(AppDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Session LoginUser(string email, string password)
        {
            User user = _databaseContext.Users.FirstOrDefault(u => u.Email == email);

            if (user != null) // && VerifyPassword(password, user.Password, user.PasswordSalt)) /* TO-DO */
            {
                // create new session
                var sessionToken = GenerateSessionToken();
                var expirationTime = DateTime.UtcNow.AddHours(SessionDurationHours);
                var session = new Session { UserId = user.Id, Token = sessionToken, ExpirationTime = expirationTime };

                _databaseContext.Sessions.Add(session);
                _databaseContext.SaveChanges();

                return session;
            }

            return null; // user not found or password incorrect
        }

        public bool LogoutUser(HttpContext httpContext)
        {
            string? sessionToken = "";
            httpContext.Request.Cookies.TryGetValue("sessionId", out sessionToken);
            var session = _databaseContext.Sessions.FirstOrDefault(s => s.Token == sessionToken);

            if (session != null)
            {
                _databaseContext.Sessions.Remove(session);
                _databaseContext.SaveChanges();

                return true; // session is removed
            }
            return false;
        }

        private bool VerifyPassword(string password, string hashedPassword, byte[] salt)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            byte[] saltFromHash = new byte[SaltSize];
            Array.Copy(hashBytes, 0, saltFromHash, 0, SaltSize);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltFromHash, Iterations))
            {
                byte[] key = pbkdf2.GetBytes(KeySize);

                for (int i = 0; i < KeySize; i++)
                {
                    if (key[i] != hashBytes[i + SaltSize])
                    {
                        return false; // wrong password
                    }
                }
            }

            return true;
        }

        public bool ValidateSession(string sessionToken)
        {
            var session = _databaseContext.Sessions.FirstOrDefault(s => s.Token == sessionToken);

            if (session != null && session.ExpirationTime > DateTime.UtcNow)
            {
                // update session
                session.ExpirationTime = DateTime.UtcNow.AddHours(SessionDurationHours);
                _databaseContext.SaveChanges();

                return true;
            }
            return false; // session not found or expired
        }

        private string GenerateSessionToken()
        {
            // unique session token
            return Guid.NewGuid().ToString();
        }

        public void CreateSessionCookie(HttpContext httpContext, Session session)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = false,
                SameSite = SameSiteMode.Strict,
                Expires = session.ExpirationTime
            };

            httpContext.Response.Cookies.Append("sessionId", session.Token, cookieOptions);
        }

        public void DeleteSessionCookie(HttpContext httpContext)
        {
            httpContext.Response.Cookies.Delete("sessionId");
        }

        public UserFullNameDTO GetUserFullNameById(int userId)
        {
            User user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            return new UserFullNameDTO
            {
                FirstName = user.Firstname,
                LastName = user.Lastname
            };
        }

        public UserRoleDTO GetUserRole(int userId)
        {
            User user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            Role userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);

            if (userRole == null)
            {
                return null;
            }

            return new UserRoleDTO
            {
                RoleId = userRole.Id,
                RoleName = userRole.Name
            };
        }
    }
}
