using Microsoft.AspNetCore.Identity;
using LoveForTennis.Application.Interfaces;
using LoveForTennis.Core.Entities;
using LoveForTennis.Core.Models;
using LoveForTennis.Core.Constants;
using Microsoft.Extensions.Logging;

namespace LoveForTennis.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRoleService _roleService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IRoleService roleService,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _roleService = roleService;
        _logger = logger;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isValidPassword)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var userRoles = await _roleService.GetUserRolesAsync(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful.",
                User = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = user.CreatedAt,
                    Roles = userRoles
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return new AuthResponse
            {
                Success = false,
                Message = "An error occurred during login. Please try again."
            };
        }
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Email is already registered."
                };
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                // Assign Player role to new user
                var roleAssigned = await _roleService.AssignRoleToUserAsync(user, UserRoles.Player);
                if (!roleAssigned)
                {
                    _logger.LogWarning("User {UserId} created successfully but failed to assign Player role", user.Id);
                }

                var userRoles = await _roleService.GetUserRolesAsync(user);

                return new AuthResponse
                {
                    Success = true,
                    Message = "Registration successful.",
                    User = new UserInfo
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        CreatedAt = user.CreatedAt,
                        Roles = userRoles
                    }
                };
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new AuthResponse
            {
                Success = false,
                Message = $"Registration failed: {errors}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
            return new AuthResponse
            {
                Success = false,
                Message = "An error occurred during registration. Please try again."
            };
        }
    }

    public async Task<AuthResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist for security reasons
                return new AuthResponse
                {
                    Success = true,
                    Message = "If the email exists, a password reset link has been sent."
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            // In a real application, you would send an email here
            // For now, we'll just log the token (in production, this should be sent via email)
            _logger.LogInformation("Password reset token for {Email}: {Token}", request.Email, token);

            return new AuthResponse
            {
                Success = true,
                Message = "If the email exists, a password reset link has been sent.",
                Token = token // In production, this should not be returned in the response
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during forgot password for email: {Email}", request.Email);
            return new AuthResponse
            {
                Success = false,
                Message = "An error occurred while processing your request. Please try again."
            };
        }
    }

    public async Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Invalid reset request."
                };
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (result.Succeeded)
            {
                return new AuthResponse
                {
                    Success = true,
                    Message = "Password has been reset successfully."
                };
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new AuthResponse
            {
                Success = false,
                Message = $"Password reset failed: {errors}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset for email: {Email}", request.Email);
            return new AuthResponse
            {
                Success = false,
                Message = "An error occurred while resetting your password. Please try again."
            };
        }
    }

    public async Task<UserInfo?> GetUserInfoAsync(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var userRoles = await _roleService.GetUserRolesAsync(user);

            return new UserInfo
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt,
                Roles = userRoles
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user info for user ID: {UserId}", userId);
            return null;
        }
    }
}