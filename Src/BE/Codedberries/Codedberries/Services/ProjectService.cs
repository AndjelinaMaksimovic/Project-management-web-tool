using Codedberries.Models.DTOs;
using Codedberries.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Codedberries.Services
{
    public class ProjectService
    {
        private readonly AppDatabaseContext _databaseContext;
        private readonly AuthorizationService _authorizationService;
        private readonly TaskService _taskService;

        public ProjectService(AppDatabaseContext databaseContext, AuthorizationService authorizationService, TaskService taskService)
        {
            _databaseContext = databaseContext;
            _authorizationService = authorizationService;
            _taskService = taskService;
        }

        public async Task<Project> CreateProject(HttpContext httpContext, ProjectCreationRequestDTO request)
        {
            var userId = _authorizationService.GetUserIdFromSession(httpContext);
            var permission = userId.HasValue ? _authorizationService.CanCreateProject(userId.Value) : false;

            if (!permission)
            {
                throw new UnauthorizedAccessException("User does not have permission to create a project!");
            }

            Project project = new Project(request.Name, request.Description, request.DueDate);

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

        public AllProjectsDTO GetProjects()
        {
            List<string> projectsNames = _databaseContext.Projects.Select(r => r.Name).ToList();
            List<int> projectsIds = _databaseContext.Projects.Select(r => r.Id).ToList();

            return new AllProjectsDTO { ProjectsNames = projectsNames, ProjectsIds = projectsIds };
        }

        public void DeleteProject(HttpContext httpContext, int projectId)
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

            if (userRole != null && userRole.CanDeleteProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to delete project!");
            }

            var project = _databaseContext.Projects.Find(projectId);

            if (project == null)
            {
                throw new ArgumentException($"Project with ID {projectId} does not exist.");
            }

            /*
            // all tasks connected with project
            var tasksOnProject = _databaseContext.Tasks.Where(t => t.ProjectId == projectId).ToList();

            foreach (var task in tasksOnProject)
            {
                _taskService.DeleteTask(task.Id);
            }
            */

            _databaseContext.Projects.Remove(project);
            _databaseContext.SaveChanges();
        }
    }
}
