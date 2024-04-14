﻿using Codedberries.Models.DTOs;
using Codedberries.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Codedberries.Helpers;
using System.Threading.Tasks;

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

        public async System.Threading.Tasks.Task CreateProject(HttpContext httpContext, ProjectCreationRequestDTO request)
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

            if (string.IsNullOrEmpty(request.Name))
            {
                throw new ArgumentException("Project name is required!");
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                throw new ArgumentException("Project description must not be empty!");
            }

            if (Helper.IsDateRangeValid(request.StartDate, request.DueDate) == false)
            {
                throw new ArgumentException("Invalid date range!");
            }

            var existingProject = _databaseContext.Projects.FirstOrDefault(p => p.Name == request.Name);

            if (existingProject != null)
            {
                throw new ArgumentException($"Project with name '{request.Name}' already exists in the database!");
            }

            Project project = new Project(request.Name, request.Description, request.DueDate);
            project.StartDate = request.StartDate;

            if (request.UserIds == null || !request.UserIds.Any())
            {
                throw new ArgumentException("At least one user must be specified for the project!");
            }
            else
            {
                foreach (int user_id in request.UserIds)
                {
                    if (user_id <= 0)
                    {
                        throw new ArgumentException("Invalid user ID specified!");
                    }

                    var userToAddToProject = _databaseContext.Users.FirstOrDefault(u => u.Id == user_id);

                    if (userToAddToProject == null)
                    {
                        throw new ArgumentException($"User with ID {user_id} not found in database!");
                    }
                    else
                    {
                        if (userToAddToProject.RoleId == null)
                        {
                            throw new ArgumentException($"User with ID {userToAddToProject.Id} does not have any role assigned!");
                        }

                        project.Users.Add(userToAddToProject);
                    }
                }
            }

            // if user does not have a permission to create Statuses for project, project won't be created
            using var transaction = await _databaseContext.Database.BeginTransactionAsync();

            try
            {
                _databaseContext.Projects.Add(project);
                await _databaseContext.SaveChangesAsync();

                await _statusService.CreateStatus(httpContext, new StatusCreationDTO { Name = "New", ProjectId = project.Id });
                await _statusService.CreateStatus(httpContext, new StatusCreationDTO { Name = "In Progress", ProjectId = project.Id });
                await _statusService.CreateStatus(httpContext, new StatusCreationDTO { Name = "Done", ProjectId = project.Id });

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"{ ex.Message }");
            }
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

        public  void ArchiveProject(HttpContext httpContext, int projectId)
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
                throw new UnauthorizedAccessException("User does not have permission to archive project!");
            }

            var project =  _databaseContext.Projects.Find(projectId);

            if (project == null)
            {
                throw new ArgumentException($"Project with ID {projectId} not found!");
            }

            project.Archived=!project.Archived;
            _databaseContext.SaveChanges();
        }

        public async Task<double> GetProjectProgress(HttpContext httpContext, ProjectIdDTO request)
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
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (request.ProjectId <= 0)
            {
                throw new ArgumentException("ProjectId must be greater than 0!");
            }

            var project = await _databaseContext.Projects.FindAsync(request.ProjectId);

            if (project == null)
            {
                throw new ArgumentException($"Project with ID {request.ProjectId} not found in database!");
            }
        }

            public double CalculateProjectProgress(int projectId)
        {
            var project = _databaseContext.Projects
                .Include(p => p.Statuses)
                .Include(p => p.Categories)
                .SingleOrDefault(p => p.Id == projectId);

            if (project == null)
            {
                throw new ArgumentException("Project not found in database!");
            }

            // if all tasks on the project are completed
            bool allTasksCompleted = _databaseContext.Tasks
                .All(t => t.ProjectId == projectId && t.Status.Name == "Done");

            // if DueDate of the project has passed
            bool projectDueDatePassed = project.DueDate < DateTime.Now;

            if (allTasksCompleted || projectDueDatePassed)
            {
                return 100.0;
            }

            // number of tasks that were completed within the planned time frame and were not archived
            int completedTasksCount = _databaseContext.Tasks
                .Count(t => t.ProjectId == projectId && t.Status.Name == "Done" && t.DueDate <= DateTime.Now && !t.Archived);

            // total number of tasks in the project that have not been archived
            int totalTasksCount = _databaseContext.Tasks
                .Count(t => t.ProjectId == projectId && !t.Archived);

            if (totalTasksCount == 0)
            {
                return 0.0; // there are no finished tasks
            }

            // priority
            double priorityFactor = 1.0; // default factor
            var highPriorityTasksCount = _databaseContext.Tasks.Count(t => t.ProjectId == projectId && t.Priority.Name == "High" && !t.Archived);
            
            if (highPriorityTasksCount > 0)
            {
                // increase progress percentage if there are high priority tasks
                priorityFactor = 1.10; // high priority tasks have 10% higher impact
            }

            // task dependencies
            // progress is increased by 5% for each completed dependent task
            var dependentTasksCount = _databaseContext.Tasks.Count(t => t.ProjectId == projectId && t.Dependencies.Any(d => d.Status.Name == "Done"));
            

            // calculating progres percentage
            double progressPercentage = (double)completedTasksCount / totalTasksCount * 100;
            progressPercentage *= priorityFactor;
            progressPercentage += dependentTasksCount * 5;

            progressPercentage = Math.Min(progressPercentage, 100);

            return progressPercentage;
        }
    }
}
