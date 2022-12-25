using Luminus.Chat.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminus.Chat.DAL
{
    public class ClientDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public ClientDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source = USER-PC\\SQLEXPRESS; Initial Catalog = Luminus; Integrated Security = True; TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
        }
    }
}
