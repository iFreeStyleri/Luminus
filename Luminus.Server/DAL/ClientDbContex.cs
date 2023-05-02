using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminus.Server.DAL
{
    public class ClientDbContext : DbContext
    {
        private const string ConnectionString = "Filename=client.db";
        public DbSet<User> Users => Set<User>();
        public DbSet<Message> Messages => Set<Message>();
        public ClientDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //Использовать строку подключения для Sqlite
            optionsBuilder.UseSqlite(ConnectionString);
        }
    }
}
