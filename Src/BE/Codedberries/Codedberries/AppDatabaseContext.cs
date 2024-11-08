﻿using Codedberries.Models;
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
        public DbSet<Starred> Starred { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<TypeOfTaskDependency> TypesOfTaskDependency { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<TaskUser> TaskUsers { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<PasswordChangeToken> PasswordChangeTokens { get; set; }

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

            modelBuilder.Entity<TaskDependency>()
                .HasOne(td => td.TypeOfDependency)
                .WithMany()
                .HasForeignKey(td => td.TypeOfDependencyId);

            modelBuilder.Entity<Category>()
                .HasIndex(c => new { c.Name, c.ProjectId })
                .IsUnique();

            modelBuilder.Entity<Models.Task>()
               .HasOne(t => t.Category)
               .WithMany()
               .HasForeignKey(t => t.CategoryId);

            modelBuilder.Entity<Priority>()
                .HasIndex(c=>c.Name) 
                .IsUnique();

            modelBuilder.Entity<Models.Task>()
              .HasOne(t => t.Status)
              .WithMany()
              .HasForeignKey(t => t.StatusId);

            modelBuilder.Entity<Models.Task>()
              .HasOne(t => t.Priority)
              .WithMany()
              .HasForeignKey(t => t.PriorityId);

            modelBuilder.Entity<Models.Milestone>()
                .HasOne(tc => tc.Project)
                .WithMany()
                .HasForeignKey(tc => tc.ProjectId);


            modelBuilder.Entity<TaskComment>()
                .HasOne(tc => tc.User)
                .WithMany()
                .HasForeignKey(tc => tc.UserId);

            modelBuilder.Entity<TaskComment>()
                .HasOne(tc => tc.Task)
                .WithMany()
                .HasForeignKey(tc => tc.TaskId);

            modelBuilder.Entity<TypeOfTaskDependency>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<TaskUser>()
            .HasKey(tu => new { tu.TaskId, tu.UserId });

            modelBuilder.Entity<TaskUser>()
                .HasOne(tu => tu.Task)
                .WithMany() 
                .HasForeignKey(tu => tu.TaskId);

            modelBuilder.Entity<TaskUser>()
                .HasOne(tu => tu.User)
                .WithMany()
                .HasForeignKey(tu => tu.UserId);

            modelBuilder.Entity<Activity>()
                .HasOne(t=>t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<Activity>()
                .HasOne(t => t.Project)
                .WithMany()
                .HasForeignKey(t => t.ProjectId);

            modelBuilder.Entity<UserNotification>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<UserNotification>()
                .HasOne(t => t.Activity)
                .WithMany()
                .HasForeignKey(t => t.ActivityId);

            modelBuilder.Entity<PasswordChangeToken>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId);
        }

        public void ApplyMigrations()
        {
            Database.Migrate();
        }
    }
}