using Microsoft.EntityFrameworkCore;

namespace SignalRTestProject.API.Models
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}
