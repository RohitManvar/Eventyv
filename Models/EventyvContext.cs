using Microsoft.EntityFrameworkCore;

namespace Eventyv.Models
{
    public class EventyvContext : DbContext
    {
        public EventyvContext(DbContextOptions<EventyvContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
