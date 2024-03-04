using BlogServer.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogServer.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        // DbSet for users
        public DbSet<User> Users { get; set; }

        // DbSet for blog posts
        public DbSet<PostBlog> Posts { get; set; }

        // Override OnModelCreating to configure table names
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure table names
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<PostBlog>().ToTable("Posts");
        }
    }
}
