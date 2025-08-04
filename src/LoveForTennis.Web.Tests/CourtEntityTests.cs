using Microsoft.EntityFrameworkCore;
using LoveForTennis.Core.Entities;
using LoveForTennis.Core.Enums;
using LoveForTennis.Infrastructure.Data;
using Xunit;

namespace LoveForTennis.Web.Tests
{
    public class CourtEntityTests
    {
        [Fact]
        public async Task Court_Entity_Should_Be_Created_With_Correct_Properties()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Courts")
                .Options;

            using var context = new ApplicationDbContext(options);
            
            // Act - The seed data should be applied automatically
            await context.Database.EnsureCreatedAsync();
            
            // Assert - Check that courts are seeded correctly
            var courts = await context.Courts.ToListAsync();
            
            Assert.Equal(3, courts.Count);
            
            // Check Court 1
            var court1 = courts.FirstOrDefault(c => c.Name == "Court 1");
            Assert.NotNull(court1);
            Assert.Equal("Clay outdoor court with hourly booking", court1.Description);
            Assert.Equal(CourtSurfaceType.Clay, court1.SurfaceType);
            Assert.Equal(BookingTimeType.Hour, court1.AllowedBookingTimeType);
            Assert.Equal(InOrOutdoorType.Outdoor, court1.InOrOutdoorType);
            Assert.Equal(new TimeOnly(7, 0), court1.BookingAllowedFrom);
            Assert.Equal(new TimeOnly(22, 0), court1.BookingAllowedTill);
            Assert.Equal(14, court1.BookingsOpenForNumberOfDaysIntoTheFuture);
            
            // Check Court 2
            var court2 = courts.FirstOrDefault(c => c.Name == "Court 2");
            Assert.NotNull(court2);
            Assert.Equal(CourtSurfaceType.RedPlus, court2.SurfaceType);
            Assert.Equal(InOrOutdoorType.Outdoor, court2.InOrOutdoorType);
            
            // Check Court 3
            var court3 = courts.FirstOrDefault(c => c.Name == "Court 3");
            Assert.NotNull(court3);
            Assert.Equal(CourtSurfaceType.Hard, court3.SurfaceType);
            Assert.Equal(InOrOutdoorType.Indoor, court3.InOrOutdoorType);
        }
        
        [Fact]
        public async Task Court_Entity_Can_Be_Added_Programmatically()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_AddCourt")
                .Options;

            using var context = new ApplicationDbContext(options);
            await context.Database.EnsureCreatedAsync();
            
            // Act
            var newCourt = new Court
            {
                Name = "Test Court",
                Description = "A test court for unit testing",
                SurfaceType = CourtSurfaceType.Grass,
                AllowedBookingTimeType = BookingTimeType.HalfHour,
                InOrOutdoorType = InOrOutdoorType.Outdoor,
                BookingAllowedFrom = new TimeOnly(8, 0),
                BookingAllowedTill = new TimeOnly(20, 0),
                BookingsOpenForNumberOfDaysIntoTheFuture = 7
            };
            
            context.Courts.Add(newCourt);
            await context.SaveChangesAsync();
            
            // Assert
            var savedCourt = await context.Courts.FirstOrDefaultAsync(c => c.Name == "Test Court");
            Assert.NotNull(savedCourt);
            Assert.Equal(CourtSurfaceType.Grass, savedCourt.SurfaceType);
            Assert.Equal(BookingTimeType.HalfHour, savedCourt.AllowedBookingTimeType);
            Assert.Equal(InOrOutdoorType.Outdoor, savedCourt.InOrOutdoorType);
            Assert.Equal(new TimeOnly(8, 0), savedCourt.BookingAllowedFrom);
            Assert.Equal(new TimeOnly(20, 0), savedCourt.BookingAllowedTill);
            Assert.Equal(7, savedCourt.BookingsOpenForNumberOfDaysIntoTheFuture);
            
            // Assert new nullable properties have default null values
            Assert.Null(savedCourt.IsDisabledFrom);
            Assert.Null(savedCourt.IsDisabledTo);
            Assert.Null(savedCourt.IsDisabledByUser);
        }
        
        [Fact]
        public async Task Court_Entity_CanSetDisabledProperties()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_DisabledProps")
                .Options;

            using var context = new ApplicationDbContext(options);
            await context.Database.EnsureCreatedAsync();
            
            var disabledFrom = DateTime.UtcNow;
            var disabledTo = DateTime.UtcNow.AddDays(7);
            var disabledBy = "admin@example.com";
            
            // Act
            var newCourt = new Court
            {
                Name = "Disabled Test Court",
                Description = "A test court with disabled properties",
                SurfaceType = CourtSurfaceType.Hard,
                AllowedBookingTimeType = BookingTimeType.Hour,
                InOrOutdoorType = InOrOutdoorType.Indoor,
                BookingAllowedFrom = new TimeOnly(9, 0),
                BookingAllowedTill = new TimeOnly(21, 0),
                BookingsOpenForNumberOfDaysIntoTheFuture = 14,
                IsDisabledFrom = disabledFrom,
                IsDisabledTo = disabledTo,
                IsDisabledByUser = disabledBy
            };
            
            context.Courts.Add(newCourt);
            await context.SaveChangesAsync();
            
            // Assert
            var savedCourt = await context.Courts.FirstOrDefaultAsync(c => c.Name == "Disabled Test Court");
            Assert.NotNull(savedCourt);
            Assert.Equal(disabledFrom.ToString("yyyy-MM-dd HH:mm:ss"), savedCourt.IsDisabledFrom?.ToString("yyyy-MM-dd HH:mm:ss"));
            Assert.Equal(disabledTo.ToString("yyyy-MM-dd HH:mm:ss"), savedCourt.IsDisabledTo?.ToString("yyyy-MM-dd HH:mm:ss"));
            Assert.Equal(disabledBy, savedCourt.IsDisabledByUser);
        }
    }
}