using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace LoveForTennis.Web.Tests;

public class AuthenticationRedirectTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthenticationRedirectTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false // Don't follow redirects automatically so we can test them
        });
    }

    [Theory]
    [InlineData("/Account/Profile")]
    [InlineData("/Account/Details")]
    [InlineData("/Booking/Bookings")]
    [InlineData("/Account/Payment")]
    public async Task UnauthenticatedUser_AccessingProtectedRoute_RedirectsToHomePage(string protectedRoute)
    {
        // Act
        var response = await _client.GetAsync(protectedRoute);

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        
        var location = response.Headers.Location?.ToString();
        Assert.NotNull(location);
        
        // Should redirect to home page with ReturnUrl parameter
        // Location might be absolute or relative, so check if it contains the expected path
        Assert.Contains("/?ReturnUrl=", location);
        Assert.Contains(Uri.EscapeDataString(protectedRoute), location);
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/Home/Index")]
    [InlineData("/Home/Privacy")]
    public async Task UnauthenticatedUser_AccessingPublicRoute_DoesNotRedirect(string publicRoute)
    {
        // Act
        var response = await _client.GetAsync(publicRoute);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HomePage_AccessibleWithoutAuthentication()
    {
        // Act
        var response = await _client.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Welcome to Love For Tennis", content);
    }
}