using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web_API.Models;

namespace Web_API.Data
{
    public class Web_APIContext : DbContext
    {
        public Web_APIContext (DbContextOptions<Web_APIContext> options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Email);
            modelBuilder.Entity<User>().HasMany(u => u.Order).WithOne(u => u.User).HasForeignKey(u => u.UserEmail);

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "server=localhost;user=root;password=root;database=ordersapi;",
                new MySqlServerVersion(new Version(8, 0, 11))
            );
        }
        public DbSet<User> User { get; set; }
        public DbSet<Order> Order { get; set; }
    }
}
