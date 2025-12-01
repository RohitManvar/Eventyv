using Microsoft.EntityFrameworkCore;
using EventBriteClone.Models;

namespace EventBriteClone.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<EventLike> EventLikes { get; set; }
        public DbSet<Following> Followings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Event Configuration
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(5000);
                entity.Property(e => e.Location).HasMaxLength(500);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Organizer)
                    .WithMany(u => u.OrganizedEvents)
                    .HasForeignKey(e => e.OrganizerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Events)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Category Configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IconClass).HasMaxLength(50);
            });

            // Ticket Configuration
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).HasPrecision(18, 2);

                entity.HasOne(t => t.Event)
                    .WithMany(e => e.Tickets)
                    .HasForeignKey(t => t.EventId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Order Configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.OrderDate).HasDefaultValueSql("GETDATE()");

                entity.HasOne(o => o.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // OrderItem Configuration
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasPrecision(18, 2);

                entity.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oi => oi.Ticket)
                    .WithMany()
                    .HasForeignKey(oi => oi.TicketId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // EventLike Configuration
            modelBuilder.Entity<EventLike>(entity =>
            {
                entity.HasKey(el => new { el.UserId, el.EventId });

                entity.HasOne(el => el.User)
                    .WithMany(u => u.LikedEvents)
                    .HasForeignKey(el => el.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(el => el.Event)
                    .WithMany(e => e.Likes)
                    .HasForeignKey(el => el.EventId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Following Configuration
            modelBuilder.Entity<Following>(entity =>
            {
                entity.HasKey(f => new { f.FollowerId, f.FollowedId });

                entity.HasOne(f => f.Follower)
                    .WithMany(u => u.Following)
                    .HasForeignKey(f => f.FollowerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(f => f.Followed)
                    .WithMany(u => u.Followers)
                    .HasForeignKey(f => f.FollowedId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Music", IconClass = "fa-music" },
                new Category { Id = 2, Name = "Nightlife", IconClass = "fa-moon" },
                new Category { Id = 3, Name = "Performing & Visual Arts", IconClass = "fa-palette" },
                new Category { Id = 4, Name = "Holidays", IconClass = "fa-calendar" },
                new Category { Id = 5, Name = "Dating", IconClass = "fa-heart" },
                new Category { Id = 6, Name = "Hobbies", IconClass = "fa-puzzle-piece" },
                new Category { Id = 7, Name = "Business", IconClass = "fa-briefcase" },
                new Category { Id = 8, Name = "Food & Drink", IconClass = "fa-utensils" }
            );
        }
    }
}