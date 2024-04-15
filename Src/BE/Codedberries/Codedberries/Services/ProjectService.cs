using Codedberries.Models.DTOs;
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
                throw new UnauthorizedAccessException("User not found in database!");
            }

            if (user.RoleId == null)
            {
                throw new UnauthorizedAccessException("User does not have any role assigned!");
            }

            /*
                project is being created, therefore we are not using UserProject model
                to check the role
            */

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
                        throw new ArgumentException("Invalid user ID specified! UserId must be > 0!");
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

        // get all active projects
        public AllProjectsDTO GetActiveProjects(HttpContext httpContext)
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

            var activeProjects = _databaseContext.Projects
                .Where(p => !p.Archived)
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
                        Name = s.Name
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
                .ToList();

            if (activeProjects.Count == 0)
            {
                throw new Exception("No active projects found!");
            }

            return new AllProjectsDTO { Projects = activeProjects };
        }

        // get all archieved projects
        public AllProjectsDTO GetArchivedProjects(HttpContext httpContext)
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

            var archivedProjects = _databaseContext.Projects
                .Where(p => p.Archived)
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
                        Name = s.Name
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
                .ToList();

            if (archivedProjects.Count == 0)
            {
                throw new Exception("No archived projects found!");
            }

            return new AllProjectsDTO { Projects = archivedProjects };
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

            if (projectId <= 0)
            {
                throw new ArgumentException("ProjectId must be greater than 0!");
            }

            // UserProjects --- //
            var userProject = _databaseContext.UserProjects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == projectId);

            if (userProject == null)
            {
                throw new UnauthorizedAccessException($"No match for UserId {userId} and ProjectId {projectId} in UserProjects table!");
            }

            var userRoleId = userProject.RoleId;
            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == userRoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (userRole.CanDeleteProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to delete Project!");
            }
            // ---------------- //

            var project = _databaseContext.Projects.Find(projectId);

            if (project == null)
            {
                throw new ArgumentException($"Project with ID {projectId} does not found in database!");
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

        public List<ProjectInformationDTO> GetFilteredProjects(HttpContext httpContext, ProjectFilterDTO filter)
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

            IQueryable<Project> query = _databaseContext.Projects
                .Include(p => p.Statuses)
                .Include(p => p.Categories);

            if (filter != null)
            {
                if (filter.ProjectId.HasValue)
                {
                    if (filter.ProjectId <= 0)
                    {
                        throw new ArgumentException("ProjectId must be greater than 0!");
                    }

                    var existingProject = _databaseContext.Projects.Any(p => p.Id == filter.ProjectId);
                    
                    if (!existingProject)
                    {
                        throw new ArgumentException($"Project with ID {filter.ProjectId} does not exist in the database!");
                    }

                    query = query.Where(p => p.Id == filter.ProjectId);
                }

                if (filter.AssignedTo != null && filter.AssignedTo.Any())
                {
                    var validUsers = _databaseContext.Users.Any(u => filter.AssignedTo.Contains(u.Id));
                    
                    if (!validUsers)
                    {
                        throw new ArgumentException("One or more users in the AssignedTo list are not valid!");
                    }

                    query = query.Where(p => p.Users.Any(u => filter.AssignedTo.Contains(u.Id)));
                }

                if (filter.StartDateAfter.HasValue)
                {
                    query = query.Where(p => p.StartDate > filter.StartDateAfter);
                }

                if (filter.StartDateBefore.HasValue)
                {
                    query = query.Where(p => p.StartDate < filter.StartDateBefore);
                }

                if (filter.ExactStartDate.HasValue)
                {
                    query = query.Where(p => p.StartDate == filter.ExactStartDate);
                }

                if (filter.DueDateAfter.HasValue)
                {
                    query = query.Where(p => p.DueDate > filter.DueDateAfter);
                }

                if (filter.DueDateBefore.HasValue)
                {
                    query = query.Where(p => p.DueDate < filter.DueDateBefore);
                }

                if (filter.ExactDueDate.HasValue)
                {
                    query = query.Where(p => p.DueDate == filter.ExactDueDate);
                }

                if (filter.IsArchived != null)
                {
                    query = query.Where(p => p.Archived == filter.IsArchived);
                }

                if (filter.StatusId.HasValue)
                {
                    var validStatus = _databaseContext.Statuses.Any(s => s.Id == filter.StatusId);
                    
                    if (!validStatus)
                    {
                        throw new ArgumentException($"Status with ID {filter.StatusId} does not exist in database!");
                    }

                    query = query.Where(p => p.Statuses.Any(s => s.Id == filter.StatusId));
                }

                if (filter.CategoryId.HasValue)
                {
                    var validCategory = _databaseContext.Categories.Any(c => c.Id == filter.CategoryId);
                    
                    if (!validCategory)
                    {
                        throw new ArgumentException($"Category with ID {filter.CategoryId} does not exist in database!");
                    }

                    query = query.Where(p => p.Categories.Any(c => c.Id == filter.CategoryId));
                }
            }
            else
            {
                throw new ArgumentException("No filters provided for project search!");
            }


            var projects = query.Select(p => new ProjectInformationDTO
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
                StartDate = p.StartDate,
                Archived = p.Archived,
                Statuses = p.Statuses.Select(s => new StatusDTO
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList(),
                Categories = p.Categories.Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            }).ToList();

            if (projects.Count == 0)
            {
                throw new Exception("No projects found for provided parameters!");
            }

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

            if (request.ProjectId <= 0)
            {
                throw new ArgumentException("ProjectId must be greater than 0!");
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

            if (userRole.CanEditProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to edit Project!");
            }
            // ---------------- //

            if (request.IsEmpty())
            {
                throw new ArgumentException("Not enough parameters for project update!");
            }

            var project = await _databaseContext.Projects.FirstOrDefaultAsync(t => t.Id == request.ProjectId);
            
            if (project == null)
            {
                throw new ArgumentException($"Project with ID {request.ProjectId} not found in database!");
            }
            
            if (!string.IsNullOrEmpty(request.Name))
            {
                project.Name = request.Name;
            }
            
            if (!string.IsNullOrEmpty(request.Description))
            {
                project.Description = request.Description;
            }
            
            // replaces current list of assigned users with new list that is provided?
            if (request.Users != null && request.Users.Any())
            {
                var invalidUsers = request.Users.Except(_databaseContext.Users.Select(u => u.Id));
                
                if (invalidUsers.Any())
                {
                    throw new ArgumentException($"One or more users provided do not exist in the database!");
                }

                project.Users.Clear();
                
                var userProjectsToRemove = _databaseContext.UserProjects.Where(up => up.ProjectId == request.ProjectId);
                _databaseContext.UserProjects.RemoveRange(userProjectsToRemove);
                
                foreach (var userDto in request.Users)
                {
                    var userToAdd = await _databaseContext.Users.FindAsync(userDto);
                    
                    if (userToAdd != null)
                    {
                        if(userToAdd.RoleId == null)
                        {
                            throw new ArgumentException($"User with id {userToAdd.Id} does not have any roles assigned!");
                        }

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
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Users = project.Users.Select(u => new UserDTO
                {
                    Id = u.Id,
                    FirstName = u.Firstname,
                    LastName = u.Lastname,
                    ProfilePicture = u.ProfilePicture
                }).ToList(),
                DueDate = project.DueDate,
                StartDate = project.StartDate,
                Archived = project.Archived,
                Statuses = project.Statuses.Select(s => new StatusDTO
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList(),
                Categories = project.Categories.Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            };
        }

        public async System.Threading.Tasks.Task ArchiveProject(HttpContext httpContext, int projectId)
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

            if (projectId <= 0)
            {
                throw new ArgumentException("ProjectId must be greater than 0!");
            }

            // UserProjects --- //
            var userProject = _databaseContext.UserProjects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == projectId);

            if (userProject == null)
            {
                throw new UnauthorizedAccessException($"No match for UserId {userId} and ProjectId {projectId} in UserProjects table!");
            }

            var userRoleId = userProject.RoleId;
            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == userRoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (userRole.CanEditProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to archive (edit) Project!");
            }
            // ---------------- //

            var project =  _databaseContext.Projects.Find(projectId);

            if (project == null)
            {
                throw new ArgumentException($"Project with ID {projectId} not found in database!");
            }

            project.Archived = !project.Archived;   // ? archieves but also unarchieves ?

            await _databaseContext.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task<ProjectProgressDTO> GetProjectProgress(HttpContext httpContext, ProjectIdDTO request)
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
                throw new ArgumentException("ProjectId must be greater than 0!");
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
            // ---------------- //

            var project = await _databaseContext.Projects.FindAsync(request.ProjectId);

            if (project == null)
            {
                throw new ArgumentException($"Project with ID {request.ProjectId} not found in database!");
            }

            var progressCalculator = this.CalculateProjectProgress(request.ProjectId);

            var projectProgressDTO = new ProjectProgressDTO
            {
                ProjectId = request.ProjectId,
                ProgressPercentage = progressCalculator
            };

            return projectProgressDTO;
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
                        // !!!!!! can default statuses be deleted? if "Done" is not existing it won't work

            // if DueDate of the project has passed
            bool projectDueDatePassed = project.DueDate < DateTime.Now;

            if (allTasksCompleted || projectDueDatePassed)
            {
                return 100.0;
            }

            // number of tasks that were completed and were not archived
            int completedTasksCount = _databaseContext.Tasks
                .Count(t => t.ProjectId == projectId && t.Status.Name == "Done" && t.Archived == false);

            /* !!!!!!
             * I can't check for dates if the task was completed within the deadline, 
             * for additional precision, because I don't have information when the task was completed
             */

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
