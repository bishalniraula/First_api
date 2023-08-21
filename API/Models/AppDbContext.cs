using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    public class AppDbContext:DbContext

    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options) { }
        public DbSet<Brand>Brands { get; set; }
        public DbSet<User>users { get; set; }

    }
}
