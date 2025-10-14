using Microsoft.EntityFrameworkCore;
using Redit_api.Models;
using Redit_api.Models.Status;

namespace Redit_api.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
            
        }

        public DbSet<UserDTO> Users => Set<UserDTO>();
        public DbSet<PostDTO> Posts => Set<PostDTO>();
        public DbSet<Comment> Comments => Set<Comment>();
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<UserStatus>("user_status");
            modelBuilder.HasPostgresEnum<UserRole>("user_role");
            modelBuilder.HasPostgresEnum<PostStatus>("post_status");

            modelBuilder.Entity<UserDTO>(b =>
            {
                b.ToTable("user", "public");

                b.Property(u => u.AccountStatus)
                    .HasColumnName("account_status")
                    .HasColumnType("user_status");

                b.Property(u => u.Role)
                    .HasColumnName("role")
                    .HasColumnType("user_role");
            });
        }
    }
}