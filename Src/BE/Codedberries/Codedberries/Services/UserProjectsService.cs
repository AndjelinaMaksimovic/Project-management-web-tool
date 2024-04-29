using Codedberries.Models.DTOs;

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
                    throw new ArgumentException($"User with id {userOnProject.Id} does not exist in database!");
                }

                if (roleOnProject == null)
                {
                    throw new ArgumentException($"User with id {userOnProject.Id} does not have a role with id {roleOnProject.Id} in database!");
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
    }
}
