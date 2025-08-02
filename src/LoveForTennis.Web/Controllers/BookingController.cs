using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using LoveForTennis.Core.Entities;
using LoveForTennis.Application.Interfaces;

namespace LoveForTennis.Web.Controllers;

[Authorize]
public class BookingController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBookingService _bookingService;

    public BookingController(UserManager<ApplicationUser> userManager, IBookingService bookingService)
    {
        _userManager = userManager;
        _bookingService = bookingService;
    }

    public async Task<IActionResult> Bookings()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Index", "Home");
        }

        // Fetch user's bookings
        var userBookings = await _bookingService.GetBookingsByUserIdAsync(user.Id);
        var now = DateTime.Now;

        // Get 10 nearest future bookings (upcoming)
        var upcomingBookings = userBookings
            .Where(b => b.BookingFrom > now && !b.Cancelled)
            .OrderBy(b => b.BookingFrom)
            .Take(10)
            .ToList();

        // Get 10 most recent past bookings
        var pastBookings = userBookings
            .Where(b => b.BookingTo <= now && !b.Cancelled)
            .OrderByDescending(b => b.BookingFrom)
            .Take(10)
            .ToList();

        ViewBag.UserEmail = user.Email;
        ViewBag.UserName = $"{user.FirstName} {user.LastName}";
        ViewBag.UpcomingBookings = upcomingBookings;
        ViewBag.PastBookings = pastBookings;
        ViewBag.TotalBookings = userBookings.Count(b => !b.Cancelled);
        
        return View();
    }

    public IActionResult BookingCalendar()
    {
        return View();
    }
}