using Codedberries.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Codedberries.Models;

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
            modelBuilder.Entity<User>()
                .HasMany(e => e.Projects)
                .WithMany(e => e.Users)
                .UsingEntity<UserProject>();
        }
    }
}
