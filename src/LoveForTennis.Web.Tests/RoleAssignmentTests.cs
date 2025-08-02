using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using LoveForTennis.Application.Interfaces;
using LoveForTennis.Core.Models;
using LoveForTennis.Core.Constants;
using Xunit;

namespace LoveForTennis.Web.Tests;

public class RoleAssignmentTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public RoleAssignmentTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegisterUser_AssignsPlayerRole()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
        var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();

        var registerRequest = new RegisterRequest
        {
            FirstName = "Test",
            LastName = "Player", 
            Email = "testplayer@example.com",
            Password = "TestPassword123",
            ConfirmPassword = "TestPassword123"
        };

        // Act
        var result = await authService.RegisterAsync(registerRequest);

        // Assert
        Assert.True(result.Success, $"Registration failed: {result.Message}");
        Assert.NotNull(result.User);
        Assert.Contains(UserRoles.Player, result.User.Roles);
    }

    [Fact]
    public async Task RegisterUser_GetUserInfo_IncludesPlayerRole()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

        var registerRequest = new RegisterRequest
        {
            FirstName = "Test2",
            LastName = "Player2",
            Email = "testplayer2@example.com", 
            Password = "TestPassword123",
            ConfirmPassword = "TestPassword123"
        };

        // Act
        var registerResult = await authService.RegisterAsync(registerRequest);
        var userInfo = await authService.GetUserInfoAsync(registerResult.User!.Id);

        // Assert
        Assert.NotNull(userInfo);
        Assert.Contains(UserRoles.Player, userInfo.Roles);
        Assert.Single(userInfo.Roles); // Should only have Player role
    }
}