using Codedberries.Helpers;
using Codedberries.Models.DTOs;

namespace Codedberries.Services
{
    public class MilestoneService
    {
        private readonly AppDatabaseContext _databaseContext;
        private readonly AuthorizationService _authorizationService;

        public MilestoneService(AppDatabaseContext databaseContext, AuthorizationService authorizationService)
        {
            _databaseContext = databaseContext;
            _authorizationService = authorizationService;
        }

        public async System.Threading.Tasks.Task CreateMilestone(HttpContext httpContext, MilestoneCreationRequestDTO request)
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

            if (request.ProjectId <= 0)
            {
                throw new ArgumentException("Project ID must be greater than zero!");
            }

            var requestedProject = _databaseContext.Projects.FirstOrDefault(p => p.Id == request.ProjectId);

            if (requestedProject == null)
            {
                throw new ArgumentException("Project with the provided ID does not exist in database!");
            }

            // UserProjects --- //
            var userProject = _databaseContext.UserProjects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == request.ProjectId);

            if (userProject == null)
            {
                throw new UnauthorizedAccessException($"No match for UserId {userId} and ProjectId {request.ProjectId} in UserProjects table!");
            }

            var userRoleId = userProject.RoleId;
            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == userRoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (userRole.CanCreateTask == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to create Milestone!");
            }

            if (string.IsNullOrEmpty(request.Name))
            {
                throw new ArgumentException(" name is required!");
            }
            

            Models.Milestone milestone = new Models.Milestone(request.Name, request.ProjectId, request.Date);

            
            _databaseContext.Milestones.Add(milestone);
            await _databaseContext.SaveChangesAsync();
            
            
        }
    }
}
