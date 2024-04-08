using Codedberries.Models;
using Codedberries.Models.DTOs;
using Microsoft.EntityFrameworkCore;
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

        public bool SetProfilePicture(int userId, string? profilePicture)
        {
            User? user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null) return false;

            user.ProfilePicture = profilePicture;
            _databaseContext.SaveChanges();

            return true;
        }

        public async Task<List<UserInformationDTO>> GetUsers(HttpContext httpContext, UserFilterDTO body)
        {
            var userId = this.GetCurrentSessionUser(httpContext);

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

            var usersQuery = _databaseContext.Users.AsQueryable();

            if (body.ProjectId != null)
            {
                usersQuery = usersQuery.Where(u => u.Projects.Any(p => p.Id == body.ProjectId));
            }

            if (body.RoleId != null)
            {
                usersQuery = usersQuery.Where(u => u.RoleId == body.RoleId);
            }

            usersQuery = usersQuery.Where(u => u.Role.Name.ToLower().Contains("super user"));

            var users = await usersQuery
                .Select(u => new UserInformationDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    Firstname = u.Firstname,
                    Lastname = u.Lastname,
                    Activated = u.Activated,
                    ProfilePicture = u.ProfilePicture,
                    RoleId = u.RoleId,
                    RoleName = u.Role.Name,
                    Projects = u.Projects.Select(p => new ProjectDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        DueDate = p.DueDate,
                        StartDate = p.StartDate
                    }).ToList()
                }).ToListAsync();

            if (users.Count == 0)
            {
                throw new InvalidOperationException("No users found in database!");
            }

            return users;
        }

        /*
         * method GetCurrentSessionUser is created because it cannot be directly called
         * from the AuthorizationService class to prevent a circular dependency between
         * these classes (AuthorizationService needs UserService and UserService would
         * need AuthorizationService), this method will only be used here
         */
        public int? GetCurrentSessionUser(HttpContext httpContext)
        {
            string? sessionToken = "";

            if (httpContext.Request.Cookies.TryGetValue("sessionId", out sessionToken))
            {
                if (this.ValidateSession(sessionToken) == false)
                {
                    throw new UnauthorizedAccessException("Session is invalid or expired!");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Session cookie not found!");
            }

            var session = _databaseContext.Sessions.FirstOrDefault(s => s.Token == sessionToken);

            if (session != null && session.ExpirationTime > DateTime.UtcNow)
            {
                return session.UserId;
            }
            else
            {
                return null;
            }
        }
    }
}
