using Microsoft.AspNetCore.Identity;
using LoveForTennis.Application.Interfaces;
using LoveForTennis.Core.Entities;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace LoveForTennis.Application.Services;

public class RoleService : IRoleService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        UserManager<ApplicationUser> userManager,
        ILogger<RoleService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<bool> AssignRoleToUserAsync(ApplicationUser user, string role)
    {
        try
        {
            var roleClaim = new Claim("role", role);
            var result = await _userManager.AddClaimAsync(user, roleClaim);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("Successfully assigned role {Role} to user {UserId}", role, user.Id);
                return true;
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Failed to assign role {Role} to user {UserId}: {Errors}", role, user.Id, errors);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {Role} to user {UserId}", role, user.Id);
            return false;
        }
    }

    public async Task<IList<Claim>> GetUserRoleClaimsAsync(ApplicationUser user)
    {
        try
        {
            var claims = await _userManager.GetClaimsAsync(user);
            return claims.Where(c => c.Type == "role").ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role claims for user {UserId}", user.Id);
            return new List<Claim>();
        }
    }

    public async Task<bool> UserHasRoleAsync(ApplicationUser user, string role)
    {
        try
        {
            var roleClaims = await GetUserRoleClaimsAsync(user);
            return roleClaims.Any(c => c.Value == role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} has role {Role}", user.Id, role);
            return false;
        }
    }

    public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
    {
        try
        {
            var roleClaims = await GetUserRoleClaimsAsync(user);
            return roleClaims.Select(c => c.Value).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for user {UserId}", user.Id);
            return new List<string>();
        }
    }
}