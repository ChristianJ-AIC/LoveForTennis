using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using LoveForTennis.Core.Entities;

namespace LoveForTennis.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<DummyEntity> DummyEntities { get; set; }

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
    }
}