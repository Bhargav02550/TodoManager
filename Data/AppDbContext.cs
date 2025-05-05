using Microsoft.EntityFrameworkCore;
using TodoManager.Models;

namespace TodoManager.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Todo> TodoItems { get; set; }
    }
}
