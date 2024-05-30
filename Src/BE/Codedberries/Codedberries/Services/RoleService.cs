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
                    CanViewProject=role.CanViewProject
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

            /*  // !!!! change to canEditUsers when it gets added
            if (userRole.CanEditProject == false)
            {
                throw new InvalidOperationException("User does not have permission create new custom role!");
            }
            */


            var newRole = new Role(request.CustomRoleName)
            {
                CanAddNewUser = false,
                CanAddUserToProject = false,
                CanRemoveUserFromProject = false,
                CanCreateProject = false,
                CanDeleteProject = false,
                CanEditProject = false,
                CanViewProject = false,
                CanAddTaskToUser = false,
                CanCreateTask = false,
                CanRemoveTask = false,
                CanEditTask = false
            };

            foreach (var permission in request.Permissions)
            {
                var property = newRole.GetType().GetProperty(permission);
                if (property != null && property.PropertyType == typeof(bool))
                {
                    property.SetValue(newRole, true);
                }
            }

            _databaseContext.Roles.Add(newRole);
            await _databaseContext.SaveChangesAsync();

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
