using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LoveForTennis.Application.Interfaces;
using LoveForTennis.Application.Services;
using LoveForTennis.Core.Entities;
using LoveForTennis.Core.Constants;
using LoveForTennis.Infrastructure.Data;
using Xunit;

namespace LoveForTennis.Web.Tests;

public class DataSeederTests
{
    [Fact]
    public async Task SeedPrincipalUsersAsync_CreatesAllRequiredUsers_WithCorrectProperties()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb-{Guid.NewGuid()}"));
        
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IDataSeeder, DataSeeder>();
        services.AddLogging();

        var serviceProvider = services.BuildServiceProvider();
        
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
        
        var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();

        // Act
        await dataSeeder.SeedPrincipalUsersAsync();

        // Assert - Verify all 4 users were created
        var adminUser = await userManager.FindByEmailAsync("admin@dummy.com");
        var boardMemberUser = await userManager.FindByEmailAsync("boardmember@dummy.com");
        var coachUser = await userManager.FindByEmailAsync("coach@dummy.com");
        var playerUser = await userManager.FindByEmailAsync("player@dummy.com");

        Assert.NotNull(adminUser);
        Assert.NotNull(boardMemberUser);
        Assert.NotNull(coachUser);
        Assert.NotNull(playerUser);

        // Verify user properties
        Assert.Equal("Admin", adminUser.FirstName);
        Assert.Equal("Admin", adminUser.LastName);
        Assert.Equal("admin@dummy.com", adminUser.Email);
        Assert.True(adminUser.EmailConfirmed);

        Assert.Equal("BoardMember", boardMemberUser.FirstName);
        Assert.Equal("BoardMember", boardMemberUser.LastName);
        Assert.Equal("boardmember@dummy.com", boardMemberUser.Email);
        Assert.True(boardMemberUser.EmailConfirmed);

        Assert.Equal("Coach", coachUser.FirstName);
        Assert.Equal("Coach", coachUser.LastName);
        Assert.Equal("coach@dummy.com", coachUser.Email);
        Assert.True(coachUser.EmailConfirmed);

        Assert.Equal("Player", playerUser.FirstName);
        Assert.Equal("Player", playerUser.LastName);
        Assert.Equal("player@dummy.com", playerUser.Email);
        Assert.True(playerUser.EmailConfirmed);

        // Verify roles are assigned correctly
        var adminRoles = await roleService.GetUserRolesAsync(adminUser);
        var boardMemberRoles = await roleService.GetUserRolesAsync(boardMemberUser);
        var coachRoles = await roleService.GetUserRolesAsync(coachUser);
        var playerRoles = await roleService.GetUserRolesAsync(playerUser);

        // Admin users should have both Admin and Player roles
        Assert.Equal(2, adminRoles.Count);
        Assert.Contains(UserRoles.Admin, adminRoles);
        Assert.Contains(UserRoles.Player, adminRoles);

        // BoardMember users should have both BoardMember and Player roles
        Assert.Equal(2, boardMemberRoles.Count);
        Assert.Contains(UserRoles.BoardMember, boardMemberRoles);
        Assert.Contains(UserRoles.Player, boardMemberRoles);

        // Coach users should have both Coach and Player roles
        Assert.Equal(2, coachRoles.Count);
        Assert.Contains(UserRoles.Coach, coachRoles);
        Assert.Contains(UserRoles.Player, coachRoles);

        // Player users should only have the Player role
        Assert.Single(playerRoles);
        Assert.Contains(UserRoles.Player, playerRoles);
    }

    [Fact]
    public async Task SeedPrincipalUsersAsync_RunTwice_IsIdempotent()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb-{Guid.NewGuid()}"));
        
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IDataSeeder, DataSeeder>();
        services.AddLogging();

        var serviceProvider = services.BuildServiceProvider();
        
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.EnsureCreated();
        
        var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Act - Run seeding twice
        await dataSeeder.SeedPrincipalUsersAsync();
        await dataSeeder.SeedPrincipalUsersAsync();

        // Assert - Should still have exactly 4 users, not 8
        var allUsers = await userManager.Users.ToListAsync();
        Assert.Equal(4, allUsers.Count);

        // Verify specific users still exist and are unique
        var adminUsers = allUsers.Where(u => u.Email == "admin@dummy.com").ToList();
        var boardMemberUsers = allUsers.Where(u => u.Email == "boardmember@dummy.com").ToList();
        var coachUsers = allUsers.Where(u => u.Email == "coach@dummy.com").ToList();
        var playerUsers = allUsers.Where(u => u.Email == "player@dummy.com").ToList();

        Assert.Single(adminUsers);
        Assert.Single(boardMemberUsers);
        Assert.Single(coachUsers);
        Assert.Single(playerUsers);
    }
}