using Final_Pr_Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_Pr_Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rol> Rol {  get; set; }
    }
}
