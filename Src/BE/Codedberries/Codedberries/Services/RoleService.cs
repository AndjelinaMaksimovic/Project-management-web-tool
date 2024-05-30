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

        public List<RolePermissionDTO> GetRoles()
        {
            List < Role > roles= _databaseContext.Roles.ToList();
            List<RolePermissionDTO> roleDTO= new List<RolePermissionDTO>(); ;

            foreach(var role in roles)
            {
                var DTO = new RolePermissionDTO
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    CanAddUserToProject = role.CanAddUserToProject,
                    CanAddNewUser=role.CanAddNewUser,
                    CanCreateProject=role.CanCreateProject,
                    CanDeleteProject=role.CanDeleteProject,
                    CanCreateTask=role.CanCreateTask,
                    CanAddTaskToUser=role.CanAddTaskToUser,
                    CanEditProject=role.CanEditProject,
                    CanEditTask=role.CanEditTask,
                    CanRemoveTask=role.CanRemoveTask,
                    CanRemoveUserFromProject=role.CanRemoveUserFromProject,
                    CanViewProject=role.CanViewProject,
                    CanEditUser=role.CanEditUser
                };
                roleDTO.Add(DTO);
            }
            return roleDTO;
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
                throw new InvalidOperationException("Role not found!");
            }

            if (role.CanAddUserToProject == false)
            {
                throw new InvalidOperationException("User does not have permission to add user to project!");
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
