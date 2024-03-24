using Codedberries.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Codedberries
{
    public class AppDatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }

        public DbSet<Invite> Invites { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserProject> UserProjects { get; set; }
        public DbSet<Priority> Priorities { get; set; }

        public DbSet<Status> Statuses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=database.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<UserProject>()
                .HasKey(e => new { e.UserId, e.ProjectId });

            modelBuilder.Entity<User>()
                .HasMany(e => e.Projects)
                .WithMany(e => e.Users)
                .UsingEntity<UserProject>();

            modelBuilder.Entity<Models.Task>()
                .HasMany(e => e.Dependencies)
                .WithMany(e => e.DependentTasks)
                .UsingEntity<TaskDependency>();

            modelBuilder.Entity<TaskDependency>()
                .HasKey(e => new { e.TaskId, e.DependentTaskId });

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<Models.Task>()
               .HasOne(t => t.Category)
               .WithMany()
               .HasForeignKey(t => t.CategoryId);

<<<<<<< HEAD
            modelBuilder.Entity<Priority>()
                .HasIndex(c=>c.Name) 
                .IsUnique();
=======
            modelBuilder.Entity<Status>()
                .HasIndex(s => s.Name)
                .IsUnique();
                
                
>>>>>>> 0617cf3 (#71 Azuriran appDatabaseContext)
        }

        public void ApplyMigrations()
        {
            Database.Migrate();
        }
    }
}