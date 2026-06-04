using MecFindit.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace MecFindit.DataLayer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<CampusUser> CampusUsers => Set<CampusUser>();
    public DbSet<ItemReport> ItemReports => Set<ItemReport>();
    public DbSet<ItemStatus> ItemStatuses => Set<ItemStatus>();
    public DbSet<ClaimRequest> ClaimRequests => Set<ClaimRequest>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CampusUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<ItemStatus>().HasData(
            new ItemStatus { Id = 1, StatusName = "Pending" },
            new ItemStatus { Id = 2, StatusName = "Approved" },
            new ItemStatus { Id = 3, StatusName = "Rejected" },
            new ItemStatus { Id = 4, StatusName = "Claimed" },
            new ItemStatus { Id = 5, StatusName = "Returned" },
            new ItemStatus { Id = 6, StatusName = "Closed" }
        );

        modelBuilder.Entity<CampusUser>().HasData(
            new CampusUser
            {
                Id = 1,
                FullName = "Main Admin",
                Email = "admin@mecfindit.com",
                Password = "admin123",
                PhoneNumber = "97723424",
                Role = "Admin",
                IsBanned = false,
                CreatedAt = new DateTime(2026, 1, 1)
            }
        );

        modelBuilder.Entity<ItemReport>()
            .HasOne(i => i.CampusUser)
            .WithMany(u => u.ItemReports)
            .HasForeignKey(i => i.CampusUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ItemReport>()
            .HasOne(i => i.ItemStatus)
            .WithMany(s => s.ItemReports)
            .HasForeignKey(i => i.ItemStatusId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ClaimRequest>()
            .HasOne(c => c.CampusUser)
            .WithMany(u => u.ClaimRequests)
            .HasForeignKey(c => c.CampusUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ClaimRequest>()
            .HasOne(c => c.ItemReport)
            .WithMany(i => i.ClaimRequests)
            .HasForeignKey(c => c.ItemReportId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.CampusUser)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.CampusUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.ItemReport)
            .WithMany(i => i.Notifications)
            .HasForeignKey(n => n.ItemReportId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.ClaimRequest)
            .WithMany()
            .HasForeignKey(n => n.ClaimRequestId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
