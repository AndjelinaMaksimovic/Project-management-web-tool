using Codedberries.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Codedberries.Models;

namespace Codedberries
{
    public class AppDatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        public DbSet<Invite> Invites { get; set; }
        public DbSet<Session> Sessions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=database.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<RolePermission>()
                .HasKey(e => new { e.RoleId, e.PermissionId });

            modelBuilder.Entity<UserProject>()
                .HasKey(e => new { e.UserId, e.ProjectId });

            modelBuilder.Entity<User>()
                .HasMany(e => e.Projects)
                .WithMany(e => e.Users)
                .UsingEntity<UserProject>();

            modelBuilder.Entity<Role>()
                .HasMany(e => e.Permissions)
                .WithMany(e => e.Roles)
                .UsingEntity<RolePermission>();

            modelBuilder.Entity<Models.Task>()
                .HasMany(e => e.Dependencies)
                .WithMany(e => e.DependentTasks)
                .UsingEntity<TaskDependency>();

            modelBuilder.Entity<TaskDependency>()
                .HasKey(e => new { e.TaskId, e.DependentTaskId });
        }

        public bool CanPerformActionBasedOnRole(int userId, string action)
        {
            var user = Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var permission = Permissions.FirstOrDefault(p => p.Description == action);
                    return permission != null && userRole.Permissions.Contains<Permission>(permission);
                }
            }
            return false;
        }


        public bool canAddNewUser(int userId)
        {
            var user = Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var permission = Permissions.FirstOrDefault(p => p.Description == "dodavanje novih korisnika alata");
                    return permission != null && userRole.Permissions.Contains<Permission>(permission);
                }
            }
            return false;
        }


        public bool canAddUserToProject(int userId)
        {
            var user = Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var permission = Permissions.FirstOrDefault(p => p.Description == "Dodavanje korisnika na projekat");
                    return permission != null && userRole.Permissions.Contains<Permission>(permission);
                }
            }
            return false;
        }


        public bool canRemoveUserFromProject(int userId)
        {
            var user = Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.RoleId.HasValue)
            {
                var userRole = Roles.FirstOrDefault(r => r.Id == user.RoleId);
                if (userRole != null)
                {
                    var permission = Permissions.FirstOrDefault(p => p.Description == "Uklanjanje korisnika sa projekta.");
                    return permission != null && userRole.Permissions.Contains<Permission>(permission);
                }
            }
            return false;
        }






        public void AddPermissionsToRole(int roleId, List<int> permissionIds)
        {
            var role = Roles.FirstOrDefault(r => r.Id == roleId);
            if (role != null)
            {
                var permissionsToAdd = Permissions.Where(p => permissionIds.Contains(p.Id)).ToList();
                foreach (var permission in permissionsToAdd)
                {
                    role.Permissions.Add(permission);
                }
                SaveChanges();
            }
        }


        public void ApplyMigrations()
        {
            Database.Migrate();
        }
    }
}