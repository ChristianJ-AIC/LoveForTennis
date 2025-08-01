using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using LoveForTennis.Core.Entities;
using LoveForTennis.Application.Interfaces;

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
        // Profile redirects to Details for now to maintain existing functionality
        return await Details();
    }

    public async Task<IActionResult> SignUps()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Index", "Home");
        }

        // For now, we'll show a placeholder page
        // In a real application, you would fetch user's sign-ups/bookings
        ViewBag.UserEmail = user.Email;
        ViewBag.UserName = $"{user.FirstName} {user.LastName}";
        
        return View();
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