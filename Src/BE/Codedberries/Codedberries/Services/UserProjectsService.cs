using Codedberries.Models;
using Codedberries.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Codedberries.Services
{
    public class UserProjectsService
    {
        private readonly AppDatabaseContext _databaseContext;
        private readonly AuthorizationService _authorizationService;

        public UserProjectsService(AppDatabaseContext databaseContext, AuthorizationService authorizationService)
        {
            _databaseContext = databaseContext;
            _authorizationService = authorizationService;
        }

        // get all users that are on specific project
        public async Task<List<UserProjectsDTO>> GetUsersByProjectId(HttpContext httpContext, ProjectIdDTO request)
        {
            var userId = _authorizationService.GetUserIdFromSession(httpContext);

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

            if (request.ProjectId <= 0)
            {
                throw new ArgumentException("Project ID must be greater than zero!");
            }

            var requestedProject = _databaseContext.Projects.FirstOrDefault(p => p.Id == request.ProjectId);

            if (requestedProject == null)
            {
                throw new ArgumentException("Project with the provided ID does not exist in database!");
            }

            // --- //

            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);

            if (userRole == null)
            {
                throw new ArgumentException("User role not found in database!");
            }

            if (userRole.CanViewProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to view projects!");
            }

            var userProjects = _databaseContext.UserProjects
                .Where(up => up.ProjectId == request.ProjectId)
                .ToList();

            if (!userProjects.Any())
            {
                throw new ArgumentException($"There are no users for project with id {request.ProjectId}!");
            }

            var userProjectDTOs = new List<UserProjectsDTO>();

            foreach (var userProject in userProjects)
            {
                var userOnProject = _databaseContext.Users.FirstOrDefault(u => u.Id == userProject.UserId);
                var roleOnProject = _databaseContext.Roles.FirstOrDefault(r => r.Id == userProject.RoleId);

                if (userOnProject == null)
                {
                    throw new ArgumentException($"User with id {userProject.UserId} does not exist in database!");
                }

                if (roleOnProject == null)
                {
                    throw new ArgumentException($"User with id {userProject.RoleId} does not have a role with id {roleOnProject.Id} in database!");
                }

                var userProjectDTO = new UserProjectsDTO
                {
                    UserId = userOnProject.Id,
                    FirstName = userOnProject.Firstname,
                    LastName = userOnProject.Lastname,
                    ProjectId = userProject.ProjectId,
                    ProfilePicture = userOnProject.ProfilePicture,
                    RoleId = roleOnProject.Id,
                    RoleName = roleOnProject.Name
                };

                userProjectDTOs.Add(userProjectDTO);
            }

            return userProjectDTOs;
        }

        public async Task<List<UserProjectInformationDTO>> GetUserProjectsInformation(HttpContext httpContext, UserIdDTO request)
        {
            var userId = _authorizationService.GetUserIdFromSession(httpContext);

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
                throw new ArgumentException("User role not found in database!");
            }

            if (userRole.CanViewProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to view projects!");
            }

            if (request.UserId <= 0)
            {
                throw new ArgumentException("UserId must be greater than zero!");
            }

            var providedUser = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);

            if (providedUser == null)
            {
                throw new ArgumentException("Provided user not found in database!");
            }

            if (providedUser.RoleId == null)
            {
                throw new ArgumentException("Provided user does not have any role assigned!");
            }

            var allUserProjects = await _databaseContext.UserProjects
                .Where(up => up.UserId == request.UserId)
            .ToListAsync();

            if (!allUserProjects.Any())
            {
                throw new ArgumentException($"There are no projects found for user with id {request.UserId}!");
            }

            var userProjectInformation = new List<UserProjectInformationDTO>();

            foreach (var userProject in allUserProjects)
            {
                var project = _databaseContext.Projects.FirstOrDefault(p => p.Id == userProject.ProjectId);

                if (project == null)
                {
                    throw new ArgumentException($"Project with ID {userProject.ProjectId} not found in database!");
                }

                var roleOnProject = _databaseContext.Roles.FirstOrDefault(r => r.Id == userProject.RoleId);

                if (roleOnProject == null)
                {
                    throw new ArgumentException($"Role with ID {userProject.RoleId} not found in database!");
                }

                var userProjectInformationDTO = new UserProjectInformationDTO
                {
                    ProjectId = project.Id,
                    RoleIdOnProject = roleOnProject.Id,
                    RoleNameOnProject = roleOnProject.Name,
                    ProjectName = project.Name,
                    ProjectDescription = project.Description,
                    ProjectStartDate = project.StartDate,
                    ProjectDueDate = project.DueDate,
                    ProjectArchived = project.Archived,
                    ProjectStatuses = project.Statuses.Select(s => new StatusDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                        ProjectId = project.Id,
                        Order = s.Order
                    }).ToList(),
                    ProjectCategories = project.Categories.Select(c => new CategoryDTO
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList(),
                    ProjectUsers = project.Users.Select(u => new UserDTO
                    {
                        Id = u.Id,
                        FirstName = u.Firstname,
                        LastName = u.Lastname,
                        ProfilePicture = u.ProfilePicture
                    }).ToList()
                };

                userProjectInformation.Add(userProjectInformationDTO);
            }

            return userProjectInformation;
        }
    }
}
