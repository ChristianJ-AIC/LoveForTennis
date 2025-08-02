using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LoveForTennis.Application.Interfaces;
using LoveForTennis.Application.Services;
using LoveForTennis.Core.Entities;
using LoveForTennis.Core.Constants;
using LoveForTennis.Core.Models;
using LoveForTennis.Infrastructure.Data;
using Xunit;

namespace LoveForTennis.Web.Tests;

public class LoginAuthenticationTest
{
    [Fact]
    public async Task SeededUsers_CanLoginWithCorrectCredentials()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb-{Guid.NewGuid()}"));
        
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            // User settings
            options.User.RequireUniqueEmail = true;

            // Sign in settings
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>();
        
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IDataSeeder, DataSeeder>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddLogging();

        var serviceProvider = services.BuildServiceProvider();
        
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
        
        var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

        // Act - Seed the users first
        await dataSeeder.SeedPrincipalUsersAsync();

        // Test login with admin credentials
        var loginRequest = new LoginRequest
        {
            Email = "admin@dummy.com",
            Password = "Test1234!",
            RememberMe = false
        };

        var loginResult = await authService.LoginAsync(loginRequest);

        // Assert
        Assert.True(loginResult.Success, $"Login failed: {loginResult.Message}");
        Assert.NotNull(loginResult.User);
        Assert.Equal("admin@dummy.com", loginResult.User.Email);
    }

    [Theory]
    [InlineData("admin@dummy.com", UserRoles.Admin)]
    [InlineData("boardmember@dummy.com", UserRoles.BoardMember)]
    [InlineData("coach@dummy.com", UserRoles.Coach)]
    [InlineData("player@dummy.com", UserRoles.Player)]
    public async Task AllSeededUsers_CanLoginWithCorrectCredentials(string email, string expectedRole)
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb-{Guid.NewGuid()}"));
        
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            // User settings
            options.User.RequireUniqueEmail = true;

            // Sign in settings
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>();
        
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IDataSeeder, DataSeeder>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddLogging();

        var serviceProvider = services.BuildServiceProvider();
        
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
        
        var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

        // Act - Seed the users first
        await dataSeeder.SeedPrincipalUsersAsync();

        // Test login with specific user credentials
        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = "Test1234!",
            RememberMe = false
        };

        var loginResult = await authService.LoginAsync(loginRequest);

        // Assert
        Assert.True(loginResult.Success, $"Login failed for {email}: {loginResult.Message}");
        Assert.NotNull(loginResult.User);
        Assert.Equal(email, loginResult.User.Email);
        Assert.Contains(expectedRole, loginResult.User.Roles);
    }
}