﻿using Codedberries.Models;
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
                    throw new ArgumentException($"User with id {userProject.UserId} does not have a role with id {userProject.RoleId} in database!");
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

            var providedUser = _databaseContext.Users.FirstOrDefault(u => u.Id == request.UserId);

            if (providedUser == null)
            {
                throw new ArgumentException($"Provided user with ID {request.UserId} not found in database!");
            }

            if (providedUser.RoleId == null)
            {
                throw new ArgumentException($"Provided user with ID {request.UserId} does not have any role assigned!");
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
                var roleOnProject = _databaseContext.Roles.FirstOrDefault(r => r.Id == userProject.RoleId);

                if (roleOnProject == null)
                {
                    throw new ArgumentException($"Role with ID {userProject.RoleId} not found in database!");
                }

                var project = await _databaseContext.Projects
                            .FirstOrDefaultAsync(p => p.Id == userProject.ProjectId);

                if (project == null)
                {
                    throw new ArgumentException($"Project with ID {userProject.ProjectId} not found in database!");
                }

                var projectStatuses = await _databaseContext.Statuses
                    .Where(s => s.ProjectId == project.Id)
                    .Select(s => new StatusDTO
                    {
                        Id = s.Id,
                        Name = s.Name,
                        ProjectId = project.Id,
                        Order = s.Order
                    })
                    .ToListAsync();

                var projectCategories = await _databaseContext.Categories
                    .Where(c => c.ProjectId == project.Id)
                    .Select(c => new CategoryDTO
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToListAsync();

                var projectUsers = await _databaseContext.Users
                    .Where(u => u.Projects.Any(p => p.Id == project.Id))
                    .Select(u => new UserDTO
                    {
                        Id = u.Id,
                        FirstName = u.Firstname,
                        LastName = u.Lastname,
                        ProfilePicture = u.ProfilePicture
                    })
                    .ToListAsync();

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
                    ProjectIsStarred = _databaseContext.Starred.Any(s => s.ProjectId == project.Id && s.UserId == request.UserId),
                    ProjectStatuses = projectStatuses,
                    ProjectCategories = projectCategories,
                    ProjectUsers = projectUsers
                };

                userProjectInformation.Add(userProjectInformationDTO);
            }

            return userProjectInformation;
        }

        public async System.Threading.Tasks.Task RemoveUserFromProject(HttpContext httpContext, DeleteUserFromProjectDTO request)
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

            if (userRole.CanEditProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to edit Project!");
            }

            if (userRole.CanRemoveUserFromProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to remove user from Project!");
            }
            // ---------------- //

            if (request.UserId <= 0)
            {
                throw new ArgumentException("Provided UserId must be greater than 0!");
            }

            var userToDelete = await _databaseContext.Users.FindAsync(request.UserId);

            if (userToDelete == null)
            {
                throw new ArgumentException($"Provided User with ID {request.UserId} not found in database!");
            }

            if (userToDelete.RoleId == null)
            {
                throw new InvalidOperationException($"Provided User with ID {request.UserId} does not have a role assigned!");
            }

            var userProjectToDelete = await _databaseContext.UserProjects
                .FirstOrDefaultAsync(up => up.ProjectId == request.ProjectId && up.UserId == request.UserId);

            if (userProjectToDelete == null)
            {
                throw new ArgumentException($"User with ID {request.UserId} is not assigned to project with ID {request.ProjectId}!");
            }

            // find all task IDs where the user is assigned in the specified project
            var taskIds = await _databaseContext.TaskUsers
                .Where(tu => tu.UserId == request.UserId && _databaseContext.Tasks.Any(t => t.Id == tu.TaskId && t.ProjectId == request.ProjectId))
                .Select(tu => tu.TaskId)
                .ToListAsync();

            // check if all tasks are finished or if there are no tasks
            var allTasksFinished = !taskIds.Any() || await _databaseContext.Tasks
                .Where(t => taskIds.Contains(t.Id))
                .AllAsync(t => t.FinishedDate != null);

            if (!allTasksFinished)
            {
                throw new InvalidOperationException("Cannot remove user from project because they have active tasks!");
            }

            if (userRole.CanEditTask == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to edit Tasks! Cannot remove provided user from the task assignments before removing from project!");
            }

            // remove the user from the task assignments within the project
            var taskUsersToRemove = await _databaseContext.TaskUsers
                .Where(tu => tu.UserId == request.UserId && taskIds.Contains(tu.TaskId))
                .ToListAsync();

            _databaseContext.TaskUsers.RemoveRange(taskUsersToRemove);

            // remove the user from the project
            _databaseContext.UserProjects.Remove(userProjectToDelete);
            
            await _databaseContext.SaveChangesAsync();
        }

        // adds new user to project
        public async System.Threading.Tasks.Task AddUserToProject(HttpContext httpContext, AddUserToProjectDTO request)
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

            // provided data

            if (request.UserId <= 0)
            {
                throw new ArgumentException("User ID must be greater than zero!");
            }

            var requestedUser = _databaseContext.Users.FirstOrDefault(u => u.Id == request.UserId);

            if (requestedUser == null)
            {
                throw new ArgumentException("User with the provided ID does not exist in database!");
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

            if (request.RoleId <= 0)
            {
                throw new ArgumentException("Role ID must be greater than zero!");
            }

            var requestedRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == request.RoleId);

            if (requestedRole == null)
            {
                throw new ArgumentException("Role with the provided ID does not exist in database!");
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

            if (userRole.CanAddUserToProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to add new users to Project!");
            }

            // ---------------- //

            // is user already on project?
            var existingUserProject = _databaseContext.UserProjects
                .FirstOrDefault(up => up.UserId == request.UserId && up.ProjectId == request.ProjectId);

            if (existingUserProject != null)
            {
                throw new ArgumentException($"User {existingUserProject.UserId} already exsists on provided project!");
            }

            // adds
            var userToAddToProject = new UserProject
            {
                UserId = request.UserId,
                ProjectId = request.ProjectId,
                RoleId = request.RoleId
            };

            _databaseContext.UserProjects.Add(userToAddToProject);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
