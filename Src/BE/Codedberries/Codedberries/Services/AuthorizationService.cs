using Codedberries.Models;

namespace Codedberries.Services
{
    public class AuthorizationService
    {

        private readonly AppDatabaseContext _databaseContext;

       


        public bool canAddNewUser(int userId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var permission = _databaseContext.Permissions.FirstOrDefault(p => p.Id == 1);
                    return permission != null && userRole.Permissions.Contains<Permission>(permission);
                }
            }
            return false;
        }


        public bool canAddUserToProject(int userId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var permission = _databaseContext.Permissions.FirstOrDefault(p => p.Id ==2 );
                    return permission != null && userRole.Permissions.Contains<Permission>(permission);
                }
            }
            return false;
        }


        public bool canRemoveUserFromProject(int userId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var permission = _databaseContext.Permissions.FirstOrDefault(p => p.Id==3);
                    return permission != null && userRole.Permissions.Contains<Permission>(permission);
                }
            }
            return false;
        }


        public bool canCreateProject(int userId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var permission = _databaseContext.Permissions.FirstOrDefault(p => p.Id==4);
                    return permission != null && userRole.Permissions.Contains<Permission>(permission);
                }
            }
            return false;
        }

        public bool canDeleteProject(int userId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var permission = _databaseContext.Permissions.FirstOrDefault(p => p.Id==5);
                    return permission != null && userRole.Permissions.Contains<Permission>(permission);
                }
            }
            return false;
        }


        public bool canEditProject(int userId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var permission = _databaseContext.Permissions.FirstOrDefault(p => p.Id==6);
                    return permission != null && userRole.Permissions.Contains<Permission>(permission);
                }
            }
            return false;
        }

        public bool canViewProject(int userId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var permission = _databaseContext.Permissions.FirstOrDefault(p => p.Id==7);
                    return permission != null && userRole.Permissions.Contains<Permission>(permission);
                }
            }
            return false;
        }

        public bool canAddRoleAndPermissionToUser(int userId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var permission = _databaseContext.Permissions.FirstOrDefault(p => p.Id==11);
                    return permission != null && userRole.Permissions.Contains<Permission>(permission);
                }
            }
            return false;
        }

        public bool canAddTaskToUser(int userId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var permission = _databaseContext.Permissions.FirstOrDefault(p => p.Id == 12);
                    return permission != null && userRole.Permissions.Contains<Permission>(permission);
                }
            }
            return false;
        }

        public bool canCreateTask(int userId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var permission = _databaseContext.Permissions.FirstOrDefault(p => p.Id==8);
                    return permission != null && userRole.Permissions.Contains<Permission>(permission);
                }
            }
            return false;
        }

        public bool canRemoveTask(int userId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var permission = _databaseContext.Permissions.FirstOrDefault(p => p.Id==9);
                    return permission != null && userRole.Permissions.Contains<Permission>(permission);
                }
            }
            return false;
        }

        public bool canEditTask(int userId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var permission = _databaseContext.Permissions.FirstOrDefault(p => p.Id == 10);
                    return permission != null && userRole.Permissions.Contains<Permission>(permission);
                }
            }
            return false;
        }
    }
}
