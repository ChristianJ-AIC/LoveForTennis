using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using LoveForTennis.Core.Entities;

namespace LoveForTennis.Web.Controllers;

[Authorize]
public class BookingController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public BookingController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Bookings()
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
}