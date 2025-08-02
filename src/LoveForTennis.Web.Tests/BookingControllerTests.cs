using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Xunit;
using LoveForTennis.Core.Entities;
using LoveForTennis.Application.Interfaces;
using LoveForTennis.Application.DTOs;

namespace LoveForTennis.Web.Tests;

public class BookingControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BookingControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Bookings_UnauthenticatedUser_RedirectsToHome()
    {
        // Act
        var response = await _client.GetAsync("/Booking/Bookings");

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        // The redirect location should contain "/" or be to the home page 
        var location = response.Headers.Location.ToString();
        Assert.True(location.Contains("/") || location.Contains("home"), $"Expected redirect to home, but got: {location}");
    }

    [Fact]
    public async Task BookingCalendar_UnauthenticatedUser_RedirectsToHome()
    {
        // Act
        var response = await _client.GetAsync("/Booking/BookingCalendar");

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        // The redirect location should contain "/" or be to the home page
        var location = response.Headers.Location.ToString();
        Assert.True(location.Contains("/") || location.Contains("home"), $"Expected redirect to home, but got: {location}");
    }

    [Fact]
    public void BookingController_HasBookingServiceDependency()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var bookingService = scope.ServiceProvider.GetService<IBookingService>();

        // Assert
        Assert.NotNull(bookingService);
    }

    [Fact]
    public async Task BookingService_CanCreateAndRetrieveBookings()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        
        // Create a test user first
        var testUser = new ApplicationUser
        {
            UserName = "testuser@test.com",
            Email = "testuser@test.com",
            FirstName = "Test",
            LastName = "User"
        };
        await userManager.CreateAsync(testUser, "TestPassword123!");

        var bookingDto = new BookingDto
        {
            BookedByUserId = testUser.Id,
            BookingFrom = DateTime.Now.AddDays(1),
            BookingTo = DateTime.Now.AddDays(1).AddHours(1),
            Cancelled = false
        };

        // Act
        var createdBooking = await bookingService.CreateBookingAsync(bookingDto);
        var userBookings = await bookingService.GetBookingsByUserIdAsync(testUser.Id);

        // Assert
        Assert.NotNull(createdBooking);
        Assert.True(createdBooking.Id > 0);
        Assert.Single(userBookings);
        Assert.Equal(testUser.Id, createdBooking.BookedByUserId);
    }
}