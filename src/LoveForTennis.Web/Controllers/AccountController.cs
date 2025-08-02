using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using LoveForTennis.Core.Entities;
using LoveForTennis.Application.Interfaces;
using LoveForTennis.Core.Models;

namespace LoveForTennis.Web.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IAuthService _authService;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IAuthService authService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return Json(new AuthResponse
            {
                Success = false,
                Message = "Invalid input data."
            });
        }

        var result = await _authService.LoginAsync(request);
        
        if (result.Success && result.User != null)
        {
            // Sign in the user for web authentication
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                await _signInManager.SignInAsync(user, request.RememberMe);
            }
            
            return Json(result);
        }

        return Json(result);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return Json(new AuthResponse
            {
                Success = false,
                Message = "Invalid input data."
            });
        }

        var result = await _authService.RegisterAsync(request);
        return Json(result);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return Json(new AuthResponse
            {
                Success = false,
                Message = "Invalid input data."
            });
        }

        var result = await _authService.ForgotPasswordAsync(request);
        return Json(result);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return Json(new AuthResponse
            {
                Success = false,
                Message = "Invalid input data."
            });
        }

        var result = await _authService.ResetPasswordAsync(request);
        return Json(result);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ProfileStatus()
    {
        // Check if user is authenticated
        if (!User.Identity.IsAuthenticated)
        {
            return Json(new { authenticated = false });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Json(new { authenticated = false });
        }

        var userInfo = await _authService.GetUserInfoAsync(user.Id);
        return Json(new { authenticated = true, user = userInfo });
    }

    public async Task<IActionResult> Details()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var userInfo = await _authService.GetUserInfoAsync(user.Id);
        return View(userInfo);
    }

    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var userInfo = await _authService.GetUserInfoAsync(user.Id);
        return View(userInfo);
    }

    public async Task<IActionResult> Payment()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Index", "Home");
        }

        // For now, we'll just show a placeholder page
        // In a real application, you would fetch payment information
        ViewBag.UserEmail = user.Email;
        ViewBag.UserName = $"{user.FirstName} {user.LastName}";
        
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}