using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using LoveForTennis.Application.Interfaces;
using LoveForTennis.Core.Models;
using LoveForTennis.Core.Constants;
using LoveForTennis.Core.Entities;
using System.Security.Claims;
using Xunit;

namespace LoveForTennis.Web.Tests;

public class RoleAuthorizationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public RoleAuthorizationIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task CompleteRoleFlow_RegisterLoginAndAuthorize_WorksCorrectly()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
        var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var registerRequest = new RegisterRequest
        {
            FirstName = "Integration",
            LastName = "Test",
            Email = "integration@example.com",
            Password = "TestPassword123",
            ConfirmPassword = "TestPassword123"
        };

        // Act & Assert - Registration assigns Player role
        var registerResult = await authService.RegisterAsync(registerRequest);
        Assert.True(registerResult.Success);
        Assert.NotNull(registerResult.User);
        Assert.Contains(UserRoles.Player, registerResult.User.Roles);

        // Act & Assert - User has Player role claim
        var user = await userManager.FindByEmailAsync(registerRequest.Email);
        Assert.NotNull(user);
        
        var userRoles = await roleService.GetUserRolesAsync(user);
        Assert.Contains(UserRoles.Player, userRoles);
        Assert.Single(userRoles);

        var roleClaims = await roleService.GetUserRoleClaimsAsync(user);
        Assert.Single(roleClaims);
        Assert.Equal("role", roleClaims.First().Type);
        Assert.Equal(UserRoles.Player, roleClaims.First().Value);

        // Act & Assert - User has Player role check
        var hasPlayerRole = await roleService.UserHasRoleAsync(user, UserRoles.Player);
        Assert.True(hasPlayerRole);

        var hasAdminRole = await roleService.UserHasRoleAsync(user, UserRoles.Admin);
        Assert.False(hasAdminRole);
    }

    [Fact]
    public async Task ProtectedEndpoints_RequireAuthentication()
    {
        // Test that protected endpoints redirect to home/login when not authenticated
        var protectedRoutes = new[]
        {
            "/Account/Profile",
            "/Account/Details", 
            "/Booking/Bookings"
        };

        foreach (var route in protectedRoutes)
        {
            var response = await _client.GetAsync(route);
            Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("/", response.Headers.Location?.ToString());
        }
    }

    [Fact]
    public async Task AuthenticationPolicies_AreConfiguredCorrectly()
    {
        // Verify that authorization policies are configured in the DI container
        using var scope = _factory.Services.CreateScope();
        var authorizationService = scope.ServiceProvider.GetService<Microsoft.AspNetCore.Authorization.IAuthorizationService>();
        Assert.NotNull(authorizationService);
    }
}