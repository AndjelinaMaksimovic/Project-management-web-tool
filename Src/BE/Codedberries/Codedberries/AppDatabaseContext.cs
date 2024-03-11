using Codedberries.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Codedberries.Models;

namespace Codedberries
{
    public class AppDatabaseContext : DbContext
    {
        public DbSet<Invite> Invites { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=database.db");
        }
    }
}
