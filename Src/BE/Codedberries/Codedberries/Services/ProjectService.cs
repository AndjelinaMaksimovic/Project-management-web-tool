using Codedberries.Models.DTOs;
using Codedberries.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace Codedberries.Services
{
    public class ProjectService
    {
        private readonly AppDatabaseContext _databaseContext;
        private readonly AuthorizationService _authorizationService;
        private readonly TaskService _taskService;
        private readonly StatusService _statusService;

        public ProjectService(AppDatabaseContext databaseContext, AuthorizationService authorizationService, TaskService taskService, StatusService statusService)
        {
            _databaseContext = databaseContext;
            _authorizationService = authorizationService;
            _taskService = taskService;
            _statusService = statusService;
        }

        public async Task<Project> CreateProject(HttpContext httpContext, ProjectCreationRequestDTO request)
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

            if (userRole != null && userRole.CanCreateProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to create project!");
            }

            Project project = new Project(request.Name, request.Description, request.DueDate);

            if (request.StartDate != null)
            {
                project.StartDate = request.StartDate;
            }

            if (request.UserIds != null && request.UserIds.Any())
            {
                foreach (int user_id in request.UserIds)
                {
                    var userToAddToProject = _databaseContext.Users.FirstOrDefault(u => u.Id == user_id);

                    if (userToAddToProject == null)
                    {
                        throw new ArgumentException($"User with ID {userId} not found.");
                    }
                    else
                    {
                        project.Users.Add(userToAddToProject);
                    }
                }
            }

            _databaseContext.Projects.Add(project);
            await _databaseContext.SaveChangesAsync();

            await _statusService.CreateStatus(httpContext, new StatusCreationDTO { Name = "New", ProjectId = project.Id });
            await _statusService.CreateStatus(httpContext, new StatusCreationDTO { Name = "In Progress", ProjectId = project.Id });
            await _statusService.CreateStatus(httpContext, new StatusCreationDTO { Name = "Done", ProjectId = project.Id });

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

        public List<ProjectDTO> GetFilteredProjects(HttpContext httpContext, ProjectFilterDTO filter)
        {
            var userId = _authorizationService.GetUserIdFromSession(httpContext);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid session!");
            }

            IQueryable<Project> query = _databaseContext.Projects;

            if (filter != null)
            {
                if (filter.ProjectId.HasValue)
                    query = query.Where(p => p.Id == filter.ProjectId);

                if (filter.AssignedTo.HasValue)
                    query = query.Where(p => p.Users.Any(u => u.Id == filter.AssignedTo));

                if (filter.DueDateAfter.HasValue)
                    query = query.Where(p => p.DueDate > filter.DueDateAfter);

                if (filter.DueDateBefore.HasValue)
                    query = query.Where(p => p.DueDate < filter.DueDateBefore);
            }
            else
            {
                throw new ArgumentException("No filters provided for project search!");
            }


            var projects = query.Select(p => new ProjectDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Users = p.Users.Select(u => new UserDTO
                {
                    Id = u.Id,
                    FirstName = u.Firstname,
                    LastName = u.Lastname,
                    ProfilePicture = u.ProfilePicture
                }).ToList(),
                DueDate = p.DueDate,
                StartDate = p.StartDate
            }).ToList();

            return projects;
        }

        public async Task<UpdatedProjectInfoDTO> UpdateProject(HttpContext httpContext, ProjectUpdateRequestDTO request)
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
            if (userRole != null && userRole.CanEditProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to edit Project!");
            }
            if (request.ProjectId <= 0 || request.IsEmpty())
            {
                throw new ArgumentException("Not enough parameters for task update!");
            }
            var project = await _databaseContext.Projects.FirstOrDefaultAsync(t => t.Id == request.ProjectId);
            if (project == null)
            {
                throw new ArgumentException($"Project with ID {request.ProjectId} not found!");
            }
            if (!string.IsNullOrEmpty(request.Name))
            {
                project.Name = request.Name;
            }
            if (!string.IsNullOrEmpty(request.Description))
            {
                project.Description = request.Description;
            }
            if (request.Users != null)
            {
                project.Users.Clear();
                var userProjectsToRemove = _databaseContext.UserProjects.Where(up => up.ProjectId == request.ProjectId);
                _databaseContext.UserProjects.RemoveRange(userProjectsToRemove);
                _databaseContext.SaveChanges();
                foreach (var userDto in request.Users)
                {
                    var userToAdd = await _databaseContext.Users.FindAsync(userDto);
                    if (userToAdd != null)
                    {
                        project.Users.Add(userToAdd);
                    }
                }
            }

            if (request.DueDate.HasValue)
            {
                project.DueDate = request.DueDate.Value;
            }

            if (request.StartDate.HasValue)
            {
                project.StartDate = request.StartDate.Value;
            }

            await _databaseContext.SaveChangesAsync();

            return new UpdatedProjectInfoDTO
            {
                Name = project.Name,
                Description = project.Description,
                Users = project.Users.Select(u => new UserDTO
                {
                    Id = u.Id,
                    FirstName = u.Firstname,
                    LastName = u.Lastname,
                    ProfilePicture = u.ProfilePicture
                }).ToList(),
                StartDate = project.StartDate,
                DueDate = project.DueDate
            };
        }
    }
}
