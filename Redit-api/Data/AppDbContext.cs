using Microsoft.EntityFrameworkCore;
using Redit_api.Models;

namespace Redit_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Community> Communities => Set<Community>();

        
    }
}