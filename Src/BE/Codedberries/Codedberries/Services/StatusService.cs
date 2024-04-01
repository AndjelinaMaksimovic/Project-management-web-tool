using Codedberries.Models.DTOs;
using Codedberries.Models;
using Microsoft.AspNetCore.Http;

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

            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found!");
            }

            if (userRole.CanCreateTask == false || userRole.CanCreateProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to create status!");
            }

            var newStatus = new Models.Status(statusDTO.Name, statusDTO.ProjectId);

            _databaseContext.Statuses.Add(newStatus);
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
                throw new UnauthorizedAccessException("User not found!");
            }

            var statuses = _databaseContext.Statuses
                .Where(s => s.ProjectId == request.ProjectId)
                .Select(s => new StatusDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    ProjectId = s.ProjectId
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

            if (userRole.CanDeleteProject == false || userRole.CanRemoveTask == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to delete status!");
            }
        }
    }
}
