using Microsoft.EntityFrameworkCore;
using Redit_api.Models;
using Redit_api.Models.Status;

namespace Redit_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserDTO> Users => Set<UserDTO>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Let EF know about the PG enum for metadata
            modelBuilder.HasPostgresEnum<UserStatus>("user_status");

            // Force the property to use the PG enum type (prevents int binding)
            //modelBuilder.Entity<UserDTO>()
              //  .Property(u => u.AccountStatus)
                //.HasColumnType("user_status");   // <-- NO HasConversion<string>() here

            modelBuilder.Ignore<Community>();
            base.OnModelCreating(modelBuilder);
        }
    }
}