using Codedberries.Models.DTOs;
using Codedberries.Models;

namespace Codedberries.Services
{
    public class StatusService
    {
        private readonly AppDatabaseContext _databaseContext;
        private readonly AuthorizationService _authorizationService;

        public StatusService(AppDatabaseContext dbContext, AuthorizationService authorizationService)
        {
            _databaseContext = dbContext;
            _authorizationService = authorizationService;
        }

        public async System.Threading.Tasks.Task CreateStatus(HttpContext httpContext, StatusCreationDTO statusDTO)
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
                throw new UnauthorizedAccessException("User role not found!");
            }

            if (userRole.CanCreateTask == false || userRole.CanCreateProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to create status!");
            }

            var newStatus = new Models.Status(statusDTO.Name);

            _databaseContext.Statuses.Add(newStatus);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
