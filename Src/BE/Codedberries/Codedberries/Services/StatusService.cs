﻿using Codedberries.Models.DTOs;
using Codedberries.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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

            var existingStatuses = _databaseContext.Statuses
                .Where(s => s.ProjectId == statusDTO.ProjectId)
                .OrderBy(s => s.Order)
                .ToList();

            int newStatusOrder = existingStatuses.Any() ? existingStatuses.Last().Order + 1 : 1;

            var newStatus = new Models.Status(statusDTO.Name, statusDTO.ProjectId, newStatusOrder);

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

            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found!");
            }

            if (userRole.CanDeleteProject == false || userRole.CanRemoveTask == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to delete status!");
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

            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found!");
            }

            if (userRole.CanEditProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to edit status order!");
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
        }
    }
}
