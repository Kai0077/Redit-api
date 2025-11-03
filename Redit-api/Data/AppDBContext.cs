using Microsoft.EntityFrameworkCore;
using Redit_api.Models;
using Redit_api.Models.DTO;
using Redit_api.Models.Status;
using Redit_api.Models.Views;

namespace Redit_api.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        // ==============================
        // DbSets
        // ==============================
        public DbSet<UserDTO> Users => Set<UserDTO>();
        public DbSet<PostDTO> Posts => Set<PostDTO>();
        public DbSet<CommentDTO> Comments => Set<CommentDTO>();
        public DbSet<CommunityDTO> Communities => Set<CommunityDTO>();
        public DbSet<ViewUserFollowers> VUserFollowers => Set<ViewUserFollowers>();
        public DbSet<ViewUserFollowing> VUserFollowing => Set<ViewUserFollowing>();

        

        // ==============================
        // Model Configuration
        // ==============================
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

            modelBuilder.Entity<CommunityDTO>(b =>
            {
                b.ToTable("community", "public");
                b.HasKey(c => c.Name);                 // ⬅️ make key explicit
                b.Property(c => c.Name).HasColumnName("name").HasMaxLength(100);
                b.Property(c => c.Description).HasColumnName("description");
                b.Property(c => c.ProfilePicture).HasColumnName("profile_picture");
                b.Property(c => c.OwnerUsername).HasColumnName("owner_username").HasMaxLength(50);
                b.Property(c => c.PinnedPostIds).HasColumnName("pinned_post_ids");
            });

            modelBuilder.Entity<PostDTO>(b =>
            {
                b.ToTable("post", "public");
                // Ensure enum column maps to the Postgres enum type
                b.Property(p => p.Status).HasColumnType("post_status");   // ⬅️ helpful
                // Optional: if community is nullable in DB (profile posts), leave it as is
            });
            
            modelBuilder.Entity<ViewUserFollowers>(b =>
            {
                b.ToView("v_user_followers");
                b.HasNoKey();
            });

            modelBuilder.Entity<ViewUserFollowing>(b =>
            {
                b.ToView("v_user_following");
                b.HasNoKey();
            });
        }
    }
}