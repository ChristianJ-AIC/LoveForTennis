using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using LoveForTennis.Application.Interfaces;
using LoveForTennis.Core.Entities;
using LoveForTennis.Core.Constants;

namespace LoveForTennis.Application.Services;

public class DataSeeder : IDataSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRoleService _roleService;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        UserManager<ApplicationUser> userManager,
        IRoleService roleService,
        ILogger<DataSeeder> logger)
    {
        _userManager = userManager;
        _roleService = roleService;
        _logger = logger;
    }

    public async Task SeedPrincipalUsersAsync()
    {
        var principalRoles = new[]
        {
            UserRoles.Admin,
            UserRoles.BoardMember,
            UserRoles.Coach,
            UserRoles.Player
        };

        foreach (var role in principalRoles)
        {
            await CreateUserForRoleAsync(role);
        }
    }

    private async Task CreateUserForRoleAsync(string role)
    {
        var email = $"{role.ToLower()}@dummy.com";
        
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            _logger.LogInformation("User {Email} already exists, skipping creation", email);
            return;
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = role,
            LastName = role,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, "Test1234!");
        
        if (result.Succeeded)
        {
            var roleAssigned = await _roleService.AssignRoleToUserAsync(user, role);
            if (roleAssigned)
            {
                _logger.LogInformation("Successfully created and assigned role {Role} to user {Email}", role, email);
            }
            else
            {
                _logger.LogWarning("Created user {Email} but failed to assign role {Role}", email, role);
            }
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogError("Failed to create user {Email}: {Errors}", email, errors);
        }
    }
}