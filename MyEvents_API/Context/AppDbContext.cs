using Microsoft.EntityFrameworkCore;
using MyEvents_API.Models;

namespace MyEvents_API.Context
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<User_Registration> UserRegistration { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User_Registration>().ToTable("user_registration");
        }
       
    }
}
