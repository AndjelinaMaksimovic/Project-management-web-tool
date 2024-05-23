using System.Diagnostics;
using Codedberries.Helpers;
using Codedberries.Models;
using Codedberries.Models.DTOs;
using Microsoft.EntityFrameworkCore;

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

            Models.Activity activity = new Models.Activity(user.Id, request.ProjectId, $"User {user.Email} has created the Milestone {request.Name}", TimeOnly.FromDateTime(DateTime.Now));
            _databaseContext.Activities.Add(activity);
            _databaseContext.SaveChangesAsync();

            var projectUsers = _databaseContext.UserProjects
            .Where(up => up.ProjectId == request.ProjectId && up.UserId != userId)
            .Select(up => up.UserId)
            .ToList();

            // Create UserNotification for each user on the project
            foreach (var projectUser in projectUsers)
            {
                UserNotification userNotification = new UserNotification(projectUser, activity.Id, seen: false);
                _databaseContext.UserNotifications.Add(userNotification);
            }

            await _databaseContext.SaveChangesAsync();

        }

        public async Task<List<MilestoneDTO>> GetAllMylestonesByProjects(HttpContext httpContext, ProjectIdDTO dto)
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
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            var Milestones = await _databaseContext.Milestones
                .Where(c => c.ProjectId == dto.ProjectId)
                .Select(c => new MilestoneDTO { Id = c.MilestoneId, Name = c.Name, Date=c.Date})
                .ToListAsync();

            if (Milestones == null || !Milestones.Any())
            {
                throw new InvalidOperationException("No milestones found for the specified project.");
            }

            return Milestones;
        }

        public async Task<UpdatedMilestoneInfoDTO> UpdateMilestone(HttpContext httpContext, MilestoneUpdateRequestDTO request)
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

            if (request.IsEmpty())
            {
                throw new ArgumentException("Not enough parameters for Milestone update!");
            }

            if (request.Id <= 0)
            {
                throw new ArgumentException("MilestoneId must be greater than 0!");
            }

            var milestone = await _databaseContext.Milestones
                .FirstOrDefaultAsync(t => t.MilestoneId == request.Id);

            if (milestone == null)
            {
                throw new ArgumentException($"Milestone with ID {request.Id} not found in database!");
            }

            var userProject = _databaseContext.UserProjects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == milestone.ProjectId);

            if (userProject == null)
            {
                throw new UnauthorizedAccessException($"No match for UserId {userId} and ProjectId {milestone.ProjectId} in UserProjects table!");
            }

            var userRoleId = userProject.RoleId;
            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == userRoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (userRole.CanEditTask == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to edit Milestone!");
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                milestone.Name = request.Name;
            }

            if(request.Date.HasValue)
            {
                milestone.Date = request.Date.Value;
            }
            
            await _databaseContext.SaveChangesAsync();

            var updatedMilestoneInfo = new UpdatedMilestoneInfoDTO
            {
                MilestoneId = milestone.MilestoneId,
                Name = milestone.Name,
                Date=milestone.Date,
                ProjectId=milestone.ProjectId
            };
            
            Models.Activity activity = new Models.Activity(user.Id, milestone.ProjectId, $"User {user.Email} has updated the Milestone {request.Name}", TimeOnly.FromDateTime(DateTime.Now));
            _databaseContext.Activities.Add(activity);
            _databaseContext.SaveChangesAsync();

            var projectUsers = _databaseContext.UserProjects
            .Where(up => up.ProjectId == milestone.ProjectId && up.UserId != userId)
            .Select(up => up.UserId)
            .ToList();

            // Create UserNotification for each user on the project
            foreach (var projectUser in projectUsers)
            {
                UserNotification userNotification = new UserNotification(projectUser, activity.Id, seen: false);
                _databaseContext.UserNotifications.Add(userNotification);
            }

            await _databaseContext.SaveChangesAsync();

            return updatedMilestoneInfo;
        }
    }
}
