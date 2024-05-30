using Codedberries.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Codedberries.Services
{
    public class AuthorizationService
    {
        private readonly AppDatabaseContext _databaseContext;
        private readonly UserService _userService;

        public AuthorizationService(AppDatabaseContext databaseContext, UserService userService)
        {
            _databaseContext = databaseContext;
            _userService = userService;
        }

        public int? GetUserIdFromSession(HttpContext httpContext)
        {
            string? sessionToken = "";

            if (httpContext.Request.Cookies.TryGetValue("sessionId", out sessionToken))
            {
                if (_userService.ValidateSession(sessionToken) == false)
                {
                    return null;
                }
            }

            var session = _databaseContext.Sessions.FirstOrDefault(s => s.Token == sessionToken);

            if (session != null && session.ExpirationTime > DateTime.UtcNow)
            {
                return session.UserId;
            }
            else
            {
                return null;
            }
        }

        public bool CanAddNewUser(int userId)
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

        public bool CanAddUserToProject(int userId, int projectId)
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

        public bool CanRemoveUserFromProject(int userId, int projectId)
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

        public bool CanCreateProject(int userId)
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
        
        public bool CanDeleteProject(int userId, int projectId)
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

        public bool CanEditProject(int userId, int projectId)
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

        public bool CanViewProject(int userId, int projectId)
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

        public bool CanAddTaskToUser(int userId, int projectId)
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
        public bool CanCreateTask(int userId, int projectId)
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
        
        public bool CanRemoveTask(int userId, int projectId)
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

        public bool CanEditTask(int userId, int projectId)
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

        public bool CanEditUser(int userId, int projectId)
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
                        return userRole.CanEditUser;
                    }
                }
            }
            return false;
        }
    }
}
