using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LoveForTennis.Core.Entities;
using LoveForTennis.Core.Enums;

namespace LoveForTennis.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<DummyEntity> DummyEntities { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingPlayer> BookingPlayers { get; set; }
    public DbSet<Court> Courts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DummyEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BookedByUserId).IsRequired();
            entity.Property(e => e.BookingFrom).IsRequired();
            entity.Property(e => e.BookingTo).IsRequired();
            entity.Property(e => e.Cancelled).IsRequired();
            entity.Property(e => e.Created).IsRequired();
            entity.Property(e => e.LastUpdated);

            // Foreign key to ApplicationUser
            entity.HasOne(e => e.BookedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.BookedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Navigation property for Players
            entity.HasMany(e => e.Players)
                  .WithOne(bp => bp.Booking)
                  .HasForeignKey(bp => bp.BookingId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BookingPlayer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BookingId).IsRequired();
            entity.Property(e => e.PlayerUserId).IsRequired();
            entity.Property(e => e.Created).IsRequired();
            entity.Property(e => e.LastUpdated);

            // Foreign key to Booking
            entity.HasOne(e => e.Booking)
                  .WithMany(b => b.Players)
                  .HasForeignKey(e => e.BookingId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Foreign key to ApplicationUser
            entity.HasOne(e => e.PlayerUser)
                  .WithMany()
                  .HasForeignKey(e => e.PlayerUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Court>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.SurfaceType).IsRequired();
            entity.Property(e => e.AllowedBookingTimeType).IsRequired();
            entity.Property(e => e.InOrOutdoorType).IsRequired();
            entity.Property(e => e.BookingAllowedFrom).IsRequired();
            entity.Property(e => e.BookingAllowedTill).IsRequired();
            entity.Property(e => e.BookingsOpenForNumberOfDaysIntoTheFuture);
        });

        // Seed data
        modelBuilder.Entity<DummyEntity>().HasData(
            new DummyEntity
            {
                Id = 1,
                Name = "Tennis Ball",
                Description = "Professional tennis ball for tournaments",
                CreatedAt = DateTime.UtcNow
            },
            new DummyEntity
            {
                Id = 2,
                Name = "Tennis Racket",
                Description = "High-quality tennis racket for professional players",
                CreatedAt = DateTime.UtcNow
            },
            new DummyEntity
            {
                Id = 3,
                Name = "Tennis Court",
                Description = "Standard size tennis court with clay surface",
                CreatedAt = DateTime.UtcNow
            }
        );

        modelBuilder.Entity<Court>().HasData(
            new Court
            {
                Id = 1,
                Name = "Court 1",
                Description = "Clay outdoor court with hourly booking",
                SurfaceType = CourtSurfaceType.Clay,
                AllowedBookingTimeType = BookingTimeType.Hour,
                InOrOutdoorType = InOrOutdoorType.Outdoor,
                BookingAllowedFrom = new TimeOnly(7, 0),
                BookingAllowedTill = new TimeOnly(22, 0),
                BookingsOpenForNumberOfDaysIntoTheFuture = 14
            },
            new Court
            {
                Id = 2,
                Name = "Court 2",
                Description = "RedPlus outdoor court with hourly booking",
                SurfaceType = CourtSurfaceType.RedPlus,
                AllowedBookingTimeType = BookingTimeType.Hour,
                InOrOutdoorType = InOrOutdoorType.Outdoor,
                BookingAllowedFrom = new TimeOnly(7, 0),
                BookingAllowedTill = new TimeOnly(22, 0),
                BookingsOpenForNumberOfDaysIntoTheFuture = 14
            },
            new Court
            {
                Id = 3,
                Name = "Court 3",
                Description = "Hard indoor court with hourly booking",
                SurfaceType = CourtSurfaceType.Hard,
                AllowedBookingTimeType = BookingTimeType.Hour,
                InOrOutdoorType = InOrOutdoorType.Indoor,
                BookingAllowedFrom = new TimeOnly(7, 0),
                BookingAllowedTill = new TimeOnly(22, 0),
                BookingsOpenForNumberOfDaysIntoTheFuture = 14
            }
        );
    }
}