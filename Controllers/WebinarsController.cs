using ElamIgugu.Web.Data;
using ElamIgugu.Web.Models;
using ElamIgugu.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElamIgugu.Web.Controllers;

/// <summary>Public: anyone can view webinars and RSVP, no account needed.</summary>
public class WebinarsController : Controller
{
    private readonly ApplicationDbContext _db;

    public WebinarsController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var upcoming = await _db.Webinars
            .AsNoTracking()
            .Where(w => w.StartTime >= DateTime.Today)
            .OrderBy(w => w.StartTime)
            .ToListAsync();

        return View(upcoming);
    }

    [HttpPost]
    public async Task<IActionResult> Rsvp(RsvpViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please enter your name and a valid contact number (9 to 15 digits).";
            return RedirectToAction(nameof(Index));
        }

        var webinarExists = await _db.Webinars.AnyAsync(w => w.Id == model.WebinarId);
        if (!webinarExists)
            return NotFound();

        var number = model.ContactNumber.Replace(" ", string.Empty);

        // Stop the same number registering twice for the same webinar
        var alreadyRegistered = await _db.WebinarRsvps
            .AnyAsync(r => r.WebinarId == model.WebinarId && r.ContactNumber == number);

        if (alreadyRegistered)
        {
            TempData["Success"] = "You are already registered for this webinar.";
            return RedirectToAction(nameof(Index));
        }

        _db.WebinarRsvps.Add(new WebinarRsvp
        {
            WebinarId = model.WebinarId,
            AttendeeName = model.AttendeeName.Trim(),
            ContactNumber = number,
            RsvpDate = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "You are registered. The NPO will be in touch using the number you gave.";
        return RedirectToAction(nameof(Index));
    }
}
