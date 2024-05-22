
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

        public ProjectService(AppDatabaseContext databaseContext, AuthorizationService authorizationService, TaskService taskService)
        {
            _databaseContext = databaseContext;
            _authorizationService = authorizationService;
            _taskService = taskService;
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

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (userRole.CanCreateProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to create project!");
            }

            // users won't be added at this service, therefore this is under /* */
            /*
            if (userRole.CanAddUserToProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to add users to project!");
            }
            */

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

            // if user does not have a permission to create Statuses for project, project won't be created
            using var transaction = await _databaseContext.Database.BeginTransactionAsync();

            try
            {
                _databaseContext.Projects.Add(project);
                await _databaseContext.SaveChangesAsync();

                var userProject = new UserProject
                {
                    UserId = user.Id,
                    ProjectId = project.Id,
                    RoleId = user.RoleId.Value 
                };

                _databaseContext.UserProjects.Add(userProject);
                await _databaseContext.SaveChangesAsync();

                // users won't be added at this service, therefore this is under /* */
                /*
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

                                var userProject = new UserProject
                                {
                                    UserId = userToAddToProject.Id,
                                    ProjectId = project.Id,
                                    RoleId = userToAddToProject.RoleId.Value
                                };

                                _databaseContext.UserProjects.Add(userProject);
                                await _databaseContext.SaveChangesAsync();
                            }
                        }
                    }
                    */

                // default statuses creation
                if (userRole.CanCreateTask == false || userRole.CanCreateProject == false)
                    {
                        throw new UnauthorizedAccessException("User does not have permission to create status, user can't create new project!");
                    }

                    var projectStatuses = new List<string> { "New", "In Progress", "Done" };

                    foreach (var statusName in projectStatuses)
                    {
                        var existingStatuses = _databaseContext.Statuses
                            .Where(s => s.ProjectId == project.Id)
                            .OrderBy(s => s.Order)
                            .ToList();

                        int newStatusOrder = existingStatuses.Any() ? existingStatuses.Last().Order + 1 : 1;
                        var newStatus = new Models.Status(statusName, project.Id, newStatusOrder);

                        _databaseContext.Statuses.Add(newStatus);

                        await _databaseContext.SaveChangesAsync();
                    }
                    Activity activity = new Activity(user.Id, project.Id, $"User {user.Id} has created the project {project.Id}");
                    _databaseContext.Activities.Add(activity);
                    await _databaseContext.SaveChangesAsync();

                await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"{ex.Message}");
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

                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);

                if (userRole == null)
                {
                    throw new ArgumentException("User role not found in database!");
                }

                if (userRole.CanViewProject == false)
                {
                    throw new UnauthorizedAccessException("User does not have permission to view active projects!");
                }


                        var activeProjects = _databaseContext.UserProjects
                        .Where(up => up.UserId == userId) // Filter by user ID
                        .Join(_databaseContext.Projects, // Join with Projects table
                        up => up.ProjectId, // Match UserProject's ProjectId
                        p => p.Id, // Match Project's Id
                        (up, p) => p) // Select the Project
                        .Where(p => !p.Archived) // Filter out archived projects
                        .Select(p => new ProjectInformationDTO
                        {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        StartDate = p.StartDate,
                        DueDate = p.DueDate,
                        Archived = p.Archived,
                        IsStarred = _databaseContext.Starred.Any(s => s.ProjectId == p.Id && s.UserId == userId),
                        Statuses = p.Statuses.Select(s => new StatusDTO
                        {
                            Id = s.Id,
                            Name = s.Name,
                            ProjectId = s.ProjectId,
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

                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);

                if (userRole == null)
                {
                    throw new ArgumentException("User role not found in database!");
                }

                if (userRole.CanViewProject == false)
                {
                    throw new UnauthorizedAccessException("User does not have permission to view archived projects!");
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
                        IsStarred = _databaseContext.Starred.Any(s => s.ProjectId == p.Id && s.UserId == userId),
                        Statuses = p.Statuses.Select(s => new StatusDTO
                        {
                            Id = s.Id,
                            Name = s.Name,
                            ProjectId = s.ProjectId,
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
                .Include(p => p.Categories)
                .Where(p => p.Users.Any(u => u.Id == userId));

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

                if (filter.IsStarred.HasValue)
                {
                    if (filter.IsStarred == true)
                    {
                        // ones that are starred for current user
                        var starredProjectIds = _databaseContext.Starred
                            .Where(s => s.UserId == userId)
                            .Select(s => s.ProjectId)
                            .ToList();

                        query = query.Where(p => starredProjectIds.Contains(p.Id));
                    }
                    else
                    {
                        // ones that are not starred for current user
                        var starredProjectIds = _databaseContext.Starred
                            .Where(s => s.UserId == userId)
                            .Select(s => s.ProjectId)
                            .ToList();

                        query = query.Where(p => !starredProjectIds.Contains(p.Id));
                    }
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
                IsStarred = _databaseContext.Starred.Any(s => s.ProjectId == p.Id && s.UserId == userId),
                Statuses = p.Statuses.Select(s => new StatusDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    ProjectId = s.ProjectId,
                    Order = s.Order
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

            if (filter.SortByStartDate.HasValue)
            {
                projects.Sort((x, y) =>
                {
                    if (x.StartDate < y.StartDate) return (bool)filter.SortByStartDate ? -1 : 1;
                    else if (x.StartDate > y.StartDate) return !(bool)filter.SortByStartDate ? -1 : 1;
                    return 0;
                });
            }

            if (filter.SortByDueDate.HasValue)
            {
                projects.Sort((x, y) =>
                {
                    if (x.DueDate < y.DueDate) return (bool)filter.SortByDueDate ? -1 : 1;
                    else if (x.DueDate > y.DueDate) return !(bool)filter.SortByDueDate ? -1 : 1;
                    return 0;
                });
            }

            return projects;
        }

        public async System.Threading.Tasks.Task UpdateProject(HttpContext httpContext, ProjectUpdateRequestDTO request)
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
                var existingProjectWithName = _databaseContext.Projects
                    .Any(p => p.Name == request.Name && p.Id != request.ProjectId);

                if (existingProjectWithName)
                {
                    throw new ArgumentException($"A project with the name '{request.Name}' already exists in database!");
                }

                project.Name = request.Name;
            }

            if (!string.IsNullOrEmpty(request.Description))
            {
                project.Description = request.Description;
            }

            // replaces current list of assigned users with new list that is provided?
            if (request.Users != null && request.Users.Any())
            {
                if (userRole.CanRemoveUserFromProject == false)
                {
                    throw new UnauthorizedAccessException("User does not have permission to remove user from project!");
                }

                var invalidUsers = request.Users.Except(_databaseContext.Users.Select(u => u.Id));

                if (invalidUsers.Any())
                {
                    throw new ArgumentException($"One or more users provided do not exist in the database!");
                }

                var userProjectsToRemove = _databaseContext.UserProjects.Where(up => up.ProjectId == request.ProjectId);
                _databaseContext.UserProjects.RemoveRange(userProjectsToRemove);

                project.Users.Clear();

                foreach (var userDto in request.Users)
                {
                    var userToAdd = await _databaseContext.Users.FindAsync(userDto);

                    if (userToAdd != null)
                    {
                        if (userToAdd.RoleId == null)
                        {
                            throw new ArgumentException($"User with id {userToAdd.Id} does not have any roles assigned!");
                        }

                        var newUserProject = new UserProject
                        {
                            UserId = userToAdd.Id,
                            ProjectId = project.Id,
                            RoleId = userToAdd.RoleId.Value
                        };

                        _databaseContext.UserProjects.Add(newUserProject);
                    }
                }
            }

            if (request.StartDate.HasValue && request.DueDate.HasValue)
            {
                if (Helper.IsDateRangeValid(request.StartDate.Value, request.DueDate.Value) == false)
                {
                    throw new ArgumentException("Invalid StartDate and DueDate range!");
                }
            }

            if (request.StartDate.HasValue)
            {
                if (request.StartDate <= DateTime.MinValue || request.StartDate >= DateTime.MaxValue)
                {
                    throw new ArgumentException("StartDate must be a valid date!");
                }

                if (request.StartDate > project.DueDate)
                {
                    if (!request.DueDate.HasValue)
                    {
                        throw new ArgumentException("StartDate cannot be after DueDate!");
                    }
                }

                project.StartDate = request.StartDate.Value;
            }

            if (request.DueDate.HasValue)
            {
                if (request.DueDate <= DateTime.MinValue || request.DueDate >= DateTime.MaxValue)
                {
                    throw new ArgumentException("DueDate must be a valid date!");
                }

                if (request.DueDate < project.StartDate)
                {
                    if (!request.StartDate.HasValue)
                    {
                        throw new ArgumentException("DueDate cannot be before StartDate!");
                    }
                }

                project.DueDate = request.DueDate.Value;
            }

            if (request.Archived.HasValue)
            {
                if (request.Archived.Value != true && request.Archived.Value != false)
                {
                    throw new ArgumentException("Invalid value for Archived! Expected true or false!");
                }

                if (request.Archived.Value == true && project.Archived == true)
                {
                    throw new ArgumentException("Project is already archived!");
                }

                if (request.Archived.Value == false && project.Archived == false)
                {
                    throw new ArgumentException("Project is already not archived!");
                }

                project.Archived = request.Archived.Value;
            }
            Activity activity = new Activity(user.Id, project.Id, $"User {user.Id} has updated the project {project.Id}");
            _databaseContext.Activities.Add(activity);
            await _databaseContext.SaveChangesAsync();

            await _databaseContext.SaveChangesAsync();
        }

        // does both makes it archived or makes it active
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

            var project = _databaseContext.Projects.Find(projectId);

            if (project == null)
            {
                throw new ArgumentException($"Project with ID {projectId} not found in database!");
            }


            // archive/active
            project.Archived = !project.Archived;

            Activity activity = new Activity(user.Id, project.Id, $"User {user.Id} has archived the project {project.Id}");
            _databaseContext.Activities.Add(activity);
            await _databaseContext.SaveChangesAsync();

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

        public int CalculateProjectProgress(int projectId)
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
                return 100;
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
                return 0; // there are no finished tasks
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

            return (int)progressPercentage;
        }

        public async System.Threading.Tasks.Task ToggleStarredProject(HttpContext httpContext, ProjectIdDTO request)
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

            var targetProject = _databaseContext.Projects.FirstOrDefault(p => p.Id == request.ProjectId);

            if (targetProject == null)
            {
                throw new ArgumentException($"Provided project with ID {request.ProjectId} does not exist in database!");
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

            var existingStarredProject = await _databaseContext.Starred
                    .FirstOrDefaultAsync(sp => sp.ProjectId == request.ProjectId && sp.UserId == userId);

            // it was already starred
            if (existingStarredProject != null)
            {
                _databaseContext.Starred.Remove(existingStarredProject);
            }
            else // new one is starred
            {
                var starredProject = new Starred(request.ProjectId, user.Id);

                _databaseContext.Starred.Add(starredProject);
            }

            await _databaseContext.SaveChangesAsync();
        }

        public async Task<List<ProjectInformationDTO>> GetStarredProjectsByUserId(HttpContext httpContext)
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

            if (userRole != null && userRole.CanViewProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to view Project!");
            }

            var starredRows = await _databaseContext.Starred
                .Where(sp => sp.UserId == user.Id)
                .ToListAsync();

            if (starredRows == null || !starredRows.Any())
            {
                throw new ArgumentException($"No starred projects found for user with ID {user.Id}!");
            }

            var starredProjects = new List<ProjectInformationDTO>();

            foreach (var row in starredRows)
            {
                var project = await _databaseContext.Projects
                    .Where(p => p.Id == row.ProjectId)
                    .Select(p => new ProjectInformationDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        StartDate = p.StartDate,
                        DueDate = p.DueDate,
                        Archived = p.Archived,
                        IsStarred = _databaseContext.Starred.Any(s => s.ProjectId == p.Id && s.UserId == userId),
                        Statuses = p.Statuses.Select(s => new StatusDTO
                        {
                            Id = s.Id,
                            Name = s.Name,
                            ProjectId = s.ProjectId,
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
                    .FirstOrDefaultAsync();

                if (project == null)
                {
                    throw new ArgumentException($"Found project with ID {row.ProjectId} does not exist in the database!");
                }

                starredProjects.Add(project);
            }

            if (starredProjects.Count == 0)
            {
                throw new ArgumentException($"No starred projects found for the specified user ID {user.Id}!");
            }

            return starredProjects;
        }

        public async Task<List<ActivityDTO>> GetAllProjectActivity(HttpContext httpContext, ProjectIdDTO request)
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

            if (request.ProjectId <= 0)
            {
                throw new ArgumentException("ProjectId must be greater than 0!");
            }

            var activities = await _databaseContext.Activities
                .Where(c => c.ProjectId == request.ProjectId)
                .Select(c => new ActivityDTO { Id = c.Id, ProjectId = c.ProjectId, ActivityDescription=c.ActivityDescription, UserId=c.UserId })
                .ToListAsync();

            if (activities == null || !activities.Any())
            {
                throw new InvalidOperationException("No activities found for the specified project.");
            }

            return activities;
        }

        public async Task<List<ActivityDTO>> GetAllUserActivity(HttpContext httpContext)
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

            var activities = await _databaseContext.Activities
                .Where(c => c.UserId == userId)
                .Select(c => new ActivityDTO { Id = c.Id, ProjectId = c.ProjectId, ActivityDescription = c.ActivityDescription, UserId = c.UserId })
                .ToListAsync();

            if (activities == null || !activities.Any())
            {
                throw new InvalidOperationException("No activities found for the specified user.");
            }

            return activities;
        }

        public async Task<List<ActivityDTO>> GetActivitiesByUsersProjects(HttpContext httpContext)
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

            var userProjects = await _databaseContext.UserProjects
            .Where(c => c.UserId == userId)
            .Select(c => c.ProjectId)
            .ToListAsync();

            var activities = await _databaseContext.Activities
                .Where(c => userProjects.Contains(c.ProjectId) && c.UserId != userId)
                .ToListAsync();

            var validActivities = activities.Select(act => new ActivityDTO
            {
                Id = act.Id,
                ProjectId = act.ProjectId,
                ActivityDescription = act.ActivityDescription,
                UserId = act.UserId
            }).ToList();

            if (!validActivities.Any())
            {
                throw new InvalidOperationException("No activities found for the specified user.");
            }

            return validActivities;
        }
    }
}
