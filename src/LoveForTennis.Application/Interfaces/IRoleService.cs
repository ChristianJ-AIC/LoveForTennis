using LoveForTennis.Core.Entities;
using System.Security.Claims;

namespace LoveForTennis.Application.Interfaces;

public interface IRoleService
{
    Task<bool> AssignRoleToUserAsync(ApplicationUser user, string role);
    Task<IList<Claim>> GetUserRoleClaimsAsync(ApplicationUser user);
    Task<bool> UserHasRoleAsync(ApplicationUser user, string role);
    Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
}