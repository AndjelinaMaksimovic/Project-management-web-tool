using Codedberries.Models;
using Codedberries.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Codedberries.Services
{
    public class RoleService
    {
        private readonly AppDatabaseContext _databaseContext;
        private readonly AuthorizationService _authorizationService;

        public RoleService(AppDatabaseContext databaseContext, AuthorizationService authorizationService)
        {
            _databaseContext = databaseContext;
            _authorizationService = authorizationService;
        }

        public AllRolesNamesDTO GetRoleNames()
        {
            List<string> roleNames = _databaseContext.Roles.Select(r => r.Name).ToList();

            return new AllRolesNamesDTO { RoleNames = roleNames };
        }

        public async Task<bool> AddNewCustomRole(HttpContext httpContext, CustomRoleDTO request)
        {
            var userId = _authorizationService.GetUserIdFromSession(httpContext);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid session!");
            }

            var defaultUserRoleId = _databaseContext.Users.Where(u => u.Id == userId).Select(u => u.RoleId).FirstOrDefault();

            if (defaultUserRoleId == null)
            {
                throw new InvalidOperationException("User default role not found!");
            }

            var role = _databaseContext.Roles.FirstOrDefault(r => r.Id == defaultUserRoleId);
            
            if (role == null)
            {
                throw new InvalidOperationException("Role not found.");
            }

            var defaultTruePermissions = role.GetType().GetProperties()
                        .Where(p => p.PropertyType == typeof(bool) && (bool)p.GetValue(role) == true)
                        .Select(p => p.Name)
                        .ToList();

            var newRole = new Role(request.CustomRoleName);

            foreach (var property in newRole.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(bool) &&
                    (defaultTruePermissions.Contains(property.Name) || request.Permissions.Contains(property.Name)))
                {
                    property.SetValue(newRole, true);
                }
                else
                {
                    property.SetValue(newRole, false);
                }
            }

  
            _databaseContext.Roles.Add(newRole);
            await _databaseContext.SaveChangesAsync();

            await AddToUserProject(userId.Value, newRole.Id, request.ProjectId);

            return true;
        }

        public async Task<bool> AddToUserProject(int userId, int customRoleId, int projectId)
        {
            var userProject = new UserProject
            {
                UserId = userId,
                RoleId = customRoleId,
                ProjectId = projectId
            };

            _databaseContext.UserProjects.Add(userProject);
            await _databaseContext.SaveChangesAsync();

            return true;
        }
    }
}
