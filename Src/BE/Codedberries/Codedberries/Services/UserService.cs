using Codedberries.Models;
using Codedberries.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

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

            if (user != null && VerifyPassword(password, user.Password, user.PasswordSalt))
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
            string pass;
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
                var hashedBytes = sha256.ComputeHash(saltedPassword);
                 pass = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
            if(pass==hashedPassword) return true;
            else return false;
            
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

        public async System.Threading.Tasks.Task SetProfilePicture(HttpContext httpContext, ProfilePictureDTO request)
        {
            var userId = this.GetCurrentSessionUser(httpContext);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid session!");
            }

            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found in database!");
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

            if (request.UserId <= 0)
            {
                throw new ArgumentException("UserId must be grater than 0!");
            }

            var userToSetProfilePicture = _databaseContext.Users.FirstOrDefault(u => u.Id == request.UserId);

            if (userToSetProfilePicture == null)
            {
                throw new ArgumentException($"User with provided id {request.UserId} does not exist in database!");
            }

            if (request.ImageBytes == null || request.ImageBytes.Length == 0)
            {
                throw new ArgumentException("Image bytes cannot be null or empty!");
            }

            // generate picture name based on user id
            string imageName = $"{request.UserId}.jpg";

            // update profile picture
            userToSetProfilePicture.ProfilePicture = imageName;

            // save image file to folder ProfileImages
            string imagePath = Path.Combine("ProfileImages", $"{imageName}");
            await File.WriteAllBytesAsync(imagePath, request.ImageBytes);

            // save changes to database
            await _databaseContext.SaveChangesAsync();
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
                if (body.ProjectId <= 0)
                {
                    throw new ArgumentException("ProjectId must be greater than zero!");
                }
                else
                {
                    usersQuery = usersQuery.Where(u => u.Projects.Any(p => p.Id == body.ProjectId));
                }
            }

            if (body.RoleId != null)
            {
                if (body.RoleId <= 0)
                {
                    throw new ArgumentException("RoleId must be greater than zero!");
                }
                else
                {
                    usersQuery = usersQuery.Where(u => u.RoleId == body.RoleId);
                }
            }

            // usersQuery = usersQuery.Where(u => !u.Role.Name.ToLower().Contains("super user"));

            // get all users that are not super user, inlcuding ones that dont have any role assigned
            usersQuery = usersQuery.Where(u => u.RoleId == null || !(u.Role.CanAddNewUser
                                    && u.Role.CanAddUserToProject
                                    && u.Role.CanRemoveUserFromProject
                                    && u.Role.CanCreateProject
                                    && u.Role.CanDeleteProject
                                    && u.Role.CanEditProject
                                    && u.Role.CanViewProject
                                    && u.Role.CanAddTaskToUser
                                    && u.Role.CanCreateTask
                                    && u.Role.CanRemoveTask
                                    && u.Role.CanEditTask));

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

        public List<UserInformationDTO> GetUser(HttpContext httpContext, UserIdDTO body)
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

            IQueryable<User> query = _databaseContext.Users;
            var foundUser = _databaseContext.Users.FirstOrDefault(u => u.Id == body.UserId);

            var userInformationDTO = query.Where(s => s.Id == body.UserId)
                .Select(s => new UserInformationDTO
                {
                    Id = foundUser.Id,
                    Email = foundUser.Email,
                    Firstname = foundUser.Firstname,
                    Lastname = foundUser.Lastname,
                    Activated = foundUser.Activated,
                    ProfilePicture = foundUser.ProfilePicture,
                    RoleId = foundUser.RoleId,
                    RoleName = s.Role.Name,
                    Projects = s.Projects.Select(p => new ProjectDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        DueDate = p.DueDate,
                        StartDate = p.StartDate
                    }).ToList()
                }).ToList();

            if (userInformationDTO.Count == 0)
            {
                throw new ArgumentException($"No User found!");
            }

            return userInformationDTO;
        }

        public async Task<CurrentSessionUserDTO> GetCurrentSessionUserData(HttpContext httpContext)
        {
            var userId = this.GetCurrentSessionUser(httpContext);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid session!");
            }

            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found in database!");
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

            var tasks = await _databaseContext.Tasks
                .Where(t => t.UserId == userId)
                .Select(t => new TaskInformationDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    StartDate = t.StartDate,
                    DueDate = t.DueDate,
                    FinishedDate = t.FinishedDate,
                    UserId = t.UserId,
                    ProjectId = t.ProjectId,
                    StatusId = t.StatusId,
                    CategoryId = t.CategoryId,
                    PriorityId = t.PriorityId,
                    DifficultyLevel = t.DifficultyLevel,
                    Archived = t.Archived
                })
                .ToListAsync();

            var userProjects = await _databaseContext.UserProjects
                .Where(up => up.UserId == userId)
                .ToListAsync();

            var userProjectIds = userProjects.Select(up => up.ProjectId).ToList();

            var projects = await _databaseContext.Projects
                .Where(p => userProjectIds.Contains(p.Id))
                .Select(p => new ProjectInformationDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate,
                    DueDate = p.DueDate,
                    Archived = p.Archived,
                    Statuses = p.Statuses.Select(s => new StatusDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Order = s.Order
                    }).ToList(),
                    Categories = p.Categories.Select(c => new CategoryDTO
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList(),
                    Users = p.Users.Select(u => new UserDTO
                    {
                        Id = u.Id,
                        FirstName = u.Firstname,
                        LastName = u.Lastname,
                        ProfilePicture = u.ProfilePicture
                    }).ToList()
                })
                .ToListAsync();

            var currentSessionUserDTO = new CurrentSessionUserDTO
            {
                Id = user.Id,
                Email = user.Email,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Activated = user.Activated,
                ProfilePicture = user.ProfilePicture,
                RoleId = user.RoleId,
                RoleName = userRole.Name,
                Projects = projects,
                Tasks = tasks
            };

            return currentSessionUserDTO;
        }

        public string GetUserImagePath(HttpContext httpContext, int imageUserId)
        {
            var userId = this.GetCurrentSessionUser(httpContext);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid session!");
            }

            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found in database!");
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

            if (imageUserId <= 0)
            {
                throw new ArgumentException("UserId for image must be greater than zero!");
            }

            var userToGetPicture = _databaseContext.Users.FirstOrDefault(u => u.Id == imageUserId);

            if (userToGetPicture == null)
            {
                throw new ArgumentException($"User with provided id {imageUserId} does not exist in database!");
            }

            if(userToGetPicture.ProfilePicture == null)
            {
                throw new ArgumentException($"User with provided id {imageUserId} does not have profile picture set in database!");
            }

            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "ProfileImages", $"{imageUserId}.jpg");
            // Console.WriteLine($"Image path: {imagePath}");

            if (!File.Exists(imagePath))
            {
                throw new FileNotFoundException($"Image file for user with ID {imageUserId} not found in directory ProfileImages!", imagePath);
            }

            return imagePath;
        }

        public async System.Threading.Tasks.Task UpdateUserName(HttpContext httpContext, UpdateUserNameDTO request)
        {
            var userId = this.GetCurrentSessionUser(httpContext);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid session!");
            }

            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found in database!");
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

            var userToChange = await _databaseContext.Users.FindAsync(request.UserId);

            if (userToChange == null)
            {
                throw new ArgumentException($"User with ID {request.UserId} not found in databbase!");
            }

            userToChange.Firstname = request.FirstName;
            userToChange.Lastname = request.LastName;

            await _databaseContext.SaveChangesAsync();
        }
    }
}
