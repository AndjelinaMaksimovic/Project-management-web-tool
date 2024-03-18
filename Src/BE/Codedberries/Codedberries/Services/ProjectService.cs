using Codedberries.Models.DTOs;
using Codedberries.Models;
using Microsoft.AspNetCore.Http;

namespace Codedberries.Services
{
    public class ProjectService
    {
        private readonly AppDatabaseContext _databaseContext;
        private readonly AuthorizationService _authorizationService;

        public ProjectService(AppDatabaseContext databaseContext, AuthorizationService authorizationService)
        {
            _databaseContext = databaseContext;
            _authorizationService = authorizationService;
        }

        public async Task<Project> CreateProject(HttpContext httpContext, ProjectCreationRequestDTO request)
        {
            var userId = _authorizationService.GetUserIdFromSession(httpContext);
            var permission = userId.HasValue ? _authorizationService.CanCreateProject(userId.Value) : false;

            if (!permission)
            {
                throw new UnauthorizedAccessException("User does not have permission to create a project!");
            }

            Project project = new Project(request.Name, request.ParentProjectId, request.Description);

            if (request.UserIds != null && request.UserIds.Any())
            {
                foreach (int user_id in request.UserIds)
                {
                    var user = _databaseContext.Users.FirstOrDefault(u => u.Id == user_id);

                    if (user != null)
                    {
                        project.Users.Add(user);
                    }
                }
            }

            _databaseContext.Projects.Add(project);
            await _databaseContext.SaveChangesAsync();

            return project;
        }
    }
}
