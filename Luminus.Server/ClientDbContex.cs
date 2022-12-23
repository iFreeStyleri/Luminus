using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminus.Server
{
    public class ClientDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Message> Messages => Set<Message>();
        public ClientDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("ConnectionString");
        }
    }
}
