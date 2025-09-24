using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Eventyv.Models
{
    public class EventyvContext : DbContext
    {
        public EventyvContext(DbContextOptions<EventyvContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Event> Events { get; set; }
    }
}
