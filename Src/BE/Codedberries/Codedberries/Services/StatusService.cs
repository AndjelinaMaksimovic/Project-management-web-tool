using Codedberries.Models.DTOs;
using Codedberries.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.Data;
using System.Threading.Tasks;

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

            if (statusDTO == null)
            {
                throw new ArgumentNullException(nameof(statusDTO), "Status creation DTO cannot be null!");
            }

            if (string.IsNullOrWhiteSpace(statusDTO.Name))
            {
                throw new ArgumentException("Status name cannot be empty!");
            }

            if (statusDTO.ProjectId <= 0)
            {
                throw new ArgumentException("ProjectId must be greater than 0!");
            }

            var existingProject = _databaseContext.Projects.FirstOrDefault(p => p.Id == statusDTO.ProjectId);

            if (existingProject == null)
            {
                throw new ArgumentException($"Project with ID {statusDTO.ProjectId} does not exist in database!");
            }

            var existingStatus = _databaseContext.Statuses
                .FirstOrDefault(s => s.ProjectId == statusDTO.ProjectId && s.Name.ToLower() == statusDTO.Name.ToLower());

            if (existingStatus != null)
            {
                throw new InvalidOperationException("Status with the same name already exists on the project!");
            }

            // UserProjects --- //
            var userProject = _databaseContext.UserProjects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == statusDTO.ProjectId);

            if (userProject == null)
            {
                throw new UnauthorizedAccessException($"No match for UserId {userId} and ProjectId {statusDTO.ProjectId} in UserProjects table! User does not have a role for this project!");
            }

            var userRoleId = userProject.RoleId;
            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == userRoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (userRole.CanEditProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to create status!");
            }
            // ---------------- //

            var existingStatuses = _databaseContext.Statuses
                .Where(s => s.ProjectId == statusDTO.ProjectId)
                .OrderBy(s => s.Order)
                .ToList();

            int newStatusOrder = existingStatuses.Any() ? existingStatuses.Last().Order + 1 : 1;

            var newStatus = new Models.Status(statusDTO.Name, statusDTO.ProjectId, newStatusOrder);

            _databaseContext.Statuses.Add(newStatus);

            Activity activity = new Activity(user.Id, statusDTO.ProjectId, $"User {user.Firstname} {user.Lastname} has created the status {statusDTO.Name}", DateTime.Now);
            _databaseContext.Activities.Add(activity);
            _databaseContext.SaveChangesAsync();

            var projectUsers = _databaseContext.UserProjects
            .Where(up => up.ProjectId == statusDTO.ProjectId && up.UserId != userId)
            .Select(up => up.UserId)
            .ToList();

            // Create UserNotification for each user on the project
            foreach (var projectUser in projectUsers)
            {
                UserNotification userNotification = new UserNotification(projectUser, activity.Id, seen: false);
                _databaseContext.UserNotifications.Add(userNotification);
            }

            await _databaseContext.SaveChangesAsync();

            await _databaseContext.SaveChangesAsync();
        }

        public List<StatusDTO> GetStatusByProjectId(HttpContext httpContext, StatusProjectIdDTO request)
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
                throw new UnauthorizedAccessException("User role not found!");
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request object cannot be null!");
            }

            if (request.ProjectId <= 0)
            {
                throw new ArgumentException("ProjectId must be greater than 0!");
            }

            var existingProject = _databaseContext.Projects.FirstOrDefault(p => p.Id == request.ProjectId);

            if (existingProject == null)
            {
                throw new ArgumentException($"Project with ID {request.ProjectId} does not exist in database!");
            }

            var statuses = _databaseContext.Statuses
                .Where(s => s.ProjectId == request.ProjectId)
                .OrderBy(s => s.Order)
                .Select(s => new StatusDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    ProjectId = s.ProjectId,
                    Order = s.Order
                })
                .ToList();

            if (statuses.Count == 0)
            {
                throw new ArgumentException($"No statuses found for Project ID {request.ProjectId}!");
            }

            return statuses;
        }

        public async System.Threading.Tasks.Task DeleteStatusesByProjectId(HttpContext httpContext, StatusDeletionDTO request)
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

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request object cannot be null!");
            }

            if (request.StatusId <= 0)
            {
                throw new ArgumentException("StatusId must be greater than 0!");
            }

            if (request.ProjectId <= 0)
            {
                throw new ArgumentException("ProjectId must be greater than 0!");
            }

            var existingProject = await _databaseContext.Projects.FindAsync(request.ProjectId);

            if (existingProject == null)
            {
                throw new ArgumentException($"Project with ID {request.ProjectId} does not exist in the database!");
            }

            var statusToDelete = await _databaseContext.Statuses.FirstOrDefaultAsync(s => s.Id == request.StatusId && s.ProjectId == request.ProjectId);

            if (statusToDelete == null)
            {
                throw new ArgumentException($"Status with ID {request.StatusId} and Project ID {request.ProjectId} not found!");
            }

            var tasksWithStatus = await _databaseContext.Tasks.AnyAsync(t => t.StatusId == request.StatusId && t.ProjectId == request.ProjectId);

            if (tasksWithStatus)
            {
                throw new InvalidOperationException($"Tasks with status ID {request.StatusId} exist on project with ID {request.ProjectId}. Cannot delete status!");
            }

            // UserProjects --- //
            var userProject = _databaseContext.UserProjects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == request.ProjectId);

            if (userProject == null)
            {
                throw new UnauthorizedAccessException($"No match for UserId {userId} and ProjectId {request.ProjectId} in UserProjects table! User does not have a role for this project!");
            }

            var userRoleId = userProject.RoleId;
            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == userRoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (userRole.CanEditProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to delete status!");
            }
            // ---------------- //

            _databaseContext.Statuses.Remove(statusToDelete);

            // finding all statuses with higher order than this one that is deleted
            var statusesToUpdate = await _databaseContext.Statuses
                .Where(s => s.ProjectId == request.ProjectId && s.Order > statusToDelete.Order)
                .ToListAsync();

            // update orders with the ones with higher order
            foreach (var status in statusesToUpdate)
            {
                status.Order -= 1;
            }

            Activity activity = new Activity(user.Id, statusToDelete.ProjectId, $"User {user.Firstname} {user.Lastname} has deleted the status {statusToDelete.Name}", DateTime.Now);
            _databaseContext.Activities.Add(activity);
            _databaseContext.SaveChangesAsync();

            var projectUsers = _databaseContext.UserProjects
            .Where(up => up.ProjectId == statusToDelete.ProjectId && up.UserId != userId)
            .Select(up => up.UserId)
            .ToList();

            // Create UserNotification for each user on the project
            foreach (var projectUser in projectUsers)
            {
                UserNotification userNotification = new UserNotification(projectUser, activity.Id, seen: false);
                _databaseContext.UserNotifications.Add(userNotification);
            }

            await _databaseContext.SaveChangesAsync();

            await _databaseContext.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task ChangeStatusesOrder(HttpContext httpContext, StatusOrderChangeDTO request)
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

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request object cannot be null!");
            }

            if (request.ProjectId <= 0)
            {
                throw new ArgumentException("ProjectId must be greater than 0!");
            }

            var existingProject = _databaseContext.Projects.FirstOrDefault(p => p.Id == request.ProjectId);

            if (existingProject == null)
            {
                throw new ArgumentException($"Project with ID {request.ProjectId} does not exist in database!");
            }

            var hasStatuses = _databaseContext.Statuses.Any(s => s.ProjectId == request.ProjectId);
            
            if (!hasStatuses)
            {
                throw new ArgumentException($"No statuses found for Project ID {request.ProjectId}!");
            }

            if (request.NewOrder == null || !request.NewOrder.Any())
            {
                throw new ArgumentException("NewOrder list cannot be null or empty!");
            }

            if(request.NewOrder.Count < 2)
{
                throw new ArgumentException("NewOrder list must contain at least two elements!");
            }

            // UserProjects --- //
            var userProject = _databaseContext.UserProjects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == request.ProjectId);

            if (userProject == null)
            {
                throw new UnauthorizedAccessException($"No match for UserId {userId} and ProjectId {request.ProjectId} in UserProjects table! User does not have a role for this project!");
            }

            var userRoleId = userProject.RoleId;
            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == userRoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (userRole.CanEditProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to edit status order!");
            }
            // ---------------- //

            // collect all statuses with provided ProjectId and sort them with current order
            var statuses = await _databaseContext.Statuses
                .Where(s => s.ProjectId == request.ProjectId)
                .OrderBy(s => s.Order)
                .ToListAsync();

            if (statuses.Count != request.NewOrder.Count)
            {
                throw new ArgumentException("The number of statuses in the NewOrder list does not match the number of statuses in the database!");
            }

            foreach (var statusId in request.NewOrder)
            {
                if (!statuses.Any(s => s.Id == statusId))
                {
                    throw new ArgumentException($"Invalid status ID {statusId} with the provided NewOrder list!");
                }
            }

            // setting new statuses order
            for (int i = 0; i < statuses.Count; i++)
            {
                var statusId = request.NewOrder[i];
                var statusToUpdate = statuses.FirstOrDefault(s => s.Id == statusId);

                if (statusToUpdate != null)
                {
                    statusToUpdate.Order = i + 1;
                }
                else
                {
                    throw new ArgumentException($"Status with ID {statusId} is missing!");
                }
            }

            await _databaseContext.SaveChangesAsync();
        }

        public async Task<StatusDTO> ChangeStatusName(HttpContext httpContext, ChangeStatusNameDTO request)
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

            if (request.Id <= 0)
            {
                throw new ArgumentException("StatusId must be greater than 0!");
            }

            var status = await _databaseContext.Statuses
                .FirstOrDefaultAsync(t => t.Id == request.Id);

            if (status == null)
            {
                throw new ArgumentException($"Status with ID {request.Id} not found in database!");
            }

            // UserProjects --- //
            var userProject = _databaseContext.UserProjects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == status.ProjectId);

            if (userProject == null)
            {
                throw new UnauthorizedAccessException($"No match for UserId {userId} and ProjectId {status.ProjectId} in UserProjects table!");
            }

            var userRoleId = userProject.RoleId;
            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == userRoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (userRole.CanEditProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to edit Task!");
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                status.Name = request.Name;
            }
            
            await _databaseContext.SaveChangesAsync();

            var StatusDTO = new StatusDTO
            {
                Id = status.Id,
                Name = status.Name,
                ProjectId = status.ProjectId,
                Order = status.Order
            };

            Activity activity = new Activity(user.Id, status.ProjectId, $"User {user.Firstname} {user.Lastname} has changed the name of the status {status.Name}", DateTime.Now);
            _databaseContext.Activities.Add(activity);
            _databaseContext.SaveChangesAsync();

            var projectUsers = _databaseContext.UserProjects
            .Where(up => up.ProjectId == status.ProjectId && up.UserId != userId)
            .Select(up => up.UserId)
            .ToList();

            // Create UserNotification for each user on the project
            foreach (var projectUser in projectUsers)
            {
                UserNotification userNotification = new UserNotification(projectUser, activity.Id, seen: false);
                _databaseContext.UserNotifications.Add(userNotification);
            }

            await _databaseContext.SaveChangesAsync();

            return StatusDTO;

        }
    }
}
