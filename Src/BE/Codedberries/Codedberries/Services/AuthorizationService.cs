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
                    return userRole.CanAddNewUser;
                }
            }
            return false;
        }

        public bool canAddUserToProject(int userId, int projectId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var userProject = _databaseContext.UserProjects.FirstOrDefault(up => up.UserId == userId && up.ProjectId == projectId);
                    if(userProject != null)
                    {
                        return userRole.CanAddUserToProject;
                    }
                }
            }
            return false;
        }

        public bool canRemoveUserFromProject(int userId, int projectId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var userProject = _databaseContext.UserProjects.FirstOrDefault(up => up.UserId == userId && up.ProjectId == projectId);
                    if (userProject != null)
                    {
                        return userRole.CanRemoveUserFromProject;
                    }
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
                    return userRole.CanCreateProject;
                }
            }
            return false;
        }
        
        public bool canDeleteProject(int userId, int projectId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var userProject = _databaseContext.UserProjects.FirstOrDefault(up => up.UserId == userId && up.ProjectId == projectId);
                    if (userProject != null)
                    {
                        return userRole.CanDeleteProject;
                    }
                }
            }
            return false;
        }

        public bool canEditProject(int userId, int projectId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var userProject = _databaseContext.UserProjects.FirstOrDefault(up => up.UserId == userId && up.ProjectId == projectId);
                    if (userProject != null)
                    {
                        return userRole.CanEditProject;
                    }
                }
            }
            return false;
        }

        public bool canViewProject(int userId, int projectId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var userProject = _databaseContext.UserProjects.FirstOrDefault(up => up.UserId == userId && up.ProjectId == projectId);
                    if (userProject != null)
                    {
                        return userRole.CanViewProject;
                    }
                }
            }
            return false;
        }

        public bool canAddTaskToUser(int userId, int projectId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var userProject = _databaseContext.UserProjects.FirstOrDefault(up => up.UserId == userId && up.ProjectId == projectId);
                    if (userProject != null)
                    {
                        return userRole.CanAddTaskToUser;
                    }
                }
            }
            return false;
        }
        public bool canCreateTask(int userId, int projectId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var userProject = _databaseContext.UserProjects.FirstOrDefault(up => up.UserId == userId && up.ProjectId == projectId);
                    if (userProject != null)
                    {
                        return userRole.CanCreateTask;
                    }
                }
            }
            return false;
        }
        
        public bool canRemoveTask(int userId, int projectId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var userProject = _databaseContext.UserProjects.FirstOrDefault(up => up.UserId == userId && up.ProjectId == projectId);
                    if (userProject != null)
                    {
                        return userRole.CanRemoveTask;
                    }
                }
            }
            return false;
        }

        public bool canEditTask(int userId, int projectId)
        {
            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var userProject = _databaseContext.UserProjects.FirstOrDefault(up => up.UserId == userId && up.ProjectId == projectId);
                    if (userProject != null)
                    {
                        return userRole.CanEditTask;
                    }
                }
            }
            return false;
        }
    }
}
