using Luminus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using System.Collections.Generic;

namespace Luminus.API.DAL
{
    public class ClientDbContext : DbContext
    {
        public const string ConnectionString = "Filename=client.db";
        public DbSet<User> Users => Set<User>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<MessageFile> Files => Set<MessageFile>();
        public ClientDbContext(DbContextOptions opt) : base(opt)
        {
            Database.EnsureCreated();
        }
    }
}
