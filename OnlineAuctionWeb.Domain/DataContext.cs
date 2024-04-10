using Microsoft.EntityFrameworkCore;
using OnlineAuctionWeb.Domain.Models;

namespace OnlineAuctionWeb.Domain
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<AuctionMedia> AuctionMedias { get; set; }
        public DbSet<WatchList> WatchList { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auction>()
                .HasOne(p => p.User)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.FromUser)
                .WithMany(u => u.SentFeedbacks)
                .HasForeignKey(f => f.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.ToUser)
                .WithMany(u => u.ReceivedFeedbacks)
                .HasForeignKey(f => f.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserNotification>()
                .HasKey(un => new { un.UserId, un.NotificationId });

            modelBuilder.Entity<UserNotification>()
                .HasOne(un => un.User)
                .WithMany(u => u.UserNotifications)
                .HasForeignKey(un => un.UserId);

            modelBuilder.Entity<UserNotification>()
                .HasOne(un => un.Notification)
                .WithMany(n => n.UserNotifications)
                .HasForeignKey(un => un.NotificationId);

            // Indexes

            modelBuilder.Entity<Auction>()
               .HasIndex(p => new { p.ProductName, p.ProductStatus, p.DateCreated });

            modelBuilder.Entity<User>()
                .HasIndex(u => new { u.Email, u.FullName, u.Status, u.DateCreated })
                .IsUnique();

            modelBuilder.Entity<Feedback>()
                .HasIndex(f => new { f.FromUserId, f.ToUserId })
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => new { c.CategoryName })
                .IsUnique();

        }
    }
}
