using Microsoft.EntityFrameworkCore;
using Redit_api.Models;

namespace Redit_api.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
            
        }

        public DbSet<UserDTO> Users => Set<UserDTO>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Let EF know about the PG enum for metadata
            modelBuilder.HasPostgresEnum<UserStatus>("user_status");

            // Force the property to use the PG enum type (prevents int binding)
            modelBuilder.Entity<UserDTO>(b =>
            {
                b.ToTable("user", "public");

                // mapping to PG enum type
                b.Property(u => u.AccountStatus)
                    .HasColumnName("account_status")
                    .HasColumnType("user_status");
            });
        }
    }
}