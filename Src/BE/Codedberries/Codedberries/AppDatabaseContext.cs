using Codedberries.Models;
using Microsoft.EntityFrameworkCore;

namespace Codedberries
{
    public class AppDatabaseContext : DbContext
    {
        public DbSet<Invite> Invites { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=database.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
                .WithMany(e => e.Dependencies)
                .UsingEntity<TaskDependency>();
        }
    }
}
