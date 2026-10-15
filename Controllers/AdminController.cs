using ElamIgugu.Web.Data;
using ElamIgugu.Web.Models;
using ElamIgugu.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElamIgugu.Web.Controllers;

/// <summary>Admin-only pages: the donors list, the donations list and the Verify action.</summary>
[Authorize(Roles = AppRoles.Admin)]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string tab = "donations", string status = "All")
    {
        tab = tab == "donors" ? "donors" : "donations";

        // Donors list (with how many donations each has made)
        var donorUsers = await _userManager.GetUsersInRoleAsync(AppRoles.Donor);
        var countsByDonor = await _db.Donations
            .GroupBy(d => d.DonorId)
            .Select(g => new { DonorId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.DonorId, x => x.Count);

        var donors = donorUsers
            .OrderBy(u => u.FullName)
            .Select(u => new DonorRow
            {
                Id = u.Id,
                FullName = u.FullName,
                ContactNumber = u.PhoneNumber ?? string.Empty,
                Email = u.Email ?? string.Empty,
                CreatedDate = u.CreatedDate,
                DonationCount = countsByDonor.TryGetValue(u.Id, out var c) ? c : 0
            })
            .ToList();

        // Donations list, optionally filtered by status
        var query = _db.Donations.AsNoTracking().AsQueryable();
        if (Enum.TryParse<DonationStatus>(status, out var wanted))
            query = query.Where(d => d.Status == wanted);
        else
            status = "All";

        var donations = await query
            .OrderBy(d => d.Status == DonationStatus.Received)   // waiting (Submitted) first
            .ThenByDescending(d => d.DateSubmitted)
            .Select(d => new DonationRow
            {
                Id = d.Id,
                DonorName = d.Donor!.FullName,
                ItemType = d.ItemType,
                Description = d.Description,
                Amount = d.Amount,
                ProofFileName = d.ProofFileName,
                City = d.City,
                DateSubmitted = d.DateSubmitted,
                Status = d.Status
            })
            .ToListAsync();

        return View(new AdminDashboardViewModel
        {
            Tab = tab,
            StatusFilter = status,
            DonorCount = donors.Count,
            SubmittedCount = await _db.Donations.CountAsync(d => d.Status == DonationStatus.Submitted),
            ReceivedCount = await _db.Donations.CountAsync(d => d.Status == DonationStatus.Received),
            MoneyReceived = await _db.Donations
                .Where(d => d.Status == DonationStatus.Received)
                .SumAsync(d => d.Amount ?? 0m),
            Donors = donors,
            Donations = donations
        });
    }

    // One donor: their details and all of their donations (each can be opened)
    public async Task<IActionResult> Donor(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null || !await _userManager.IsInRoleAsync(user, AppRoles.Donor))
            return NotFound();

        var donations = await _db.Donations.AsNoTracking()
            .Where(d => d.DonorId == id)
            .OrderBy(d => d.Status == DonationStatus.Received)
            .ThenByDescending(d => d.DateSubmitted)
            .Select(d => new DonationRow
            {
                Id = d.Id,
                DonorName = user.FullName,
                ItemType = d.ItemType,
                Description = d.Description,
                Amount = d.Amount,
                ProofFileName = d.ProofFileName,
                City = d.City,
                DateSubmitted = d.DateSubmitted,
                Status = d.Status
            })
            .ToListAsync();

        return View(new DonorDetailViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            ContactNumber = user.PhoneNumber ?? string.Empty,
            Email = user.Email ?? string.Empty,
            CreatedDate = user.CreatedDate,
            Donations = donations
        });
    }

    // One donation: donor name, what they donated, the proof of payment (money), and the Verify button
    public async Task<IActionResult> Donation(int id)
    {
        var model = await _db.Donations.AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DonationDetailViewModel
            {
                Id = d.Id,
                DonorId = d.DonorId,
                DonorName = d.Donor!.FullName,
                DonorPhone = d.Donor.PhoneNumber ?? string.Empty,
                DonorEmail = d.Donor.Email ?? string.Empty,
                ItemType = d.ItemType,
                Description = d.Description,
                Amount = d.Amount,
                City = d.City,
                DateSubmitted = d.DateSubmitted,
                DateVerified = d.DateVerified,
                Status = d.Status,
                ProofFileName = d.ProofFileName
            })
            .FirstOrDefaultAsync();

        return model is null ? NotFound() : View(model);
    }

    // Shows the uploaded proof-of-payment PDF (admins only; the admin does not have to open it)
    public async Task<IActionResult> Proof(int id)
    {
        var proof = await _db.DonationProofs.AsNoTracking().FirstOrDefaultAsync(p => p.DonationId == id);
        if (proof is null)
            return NotFound();

        Response.Headers["X-Content-Type-Options"] = "nosniff";
        return File(proof.Data, "application/pdf");
    }

    // Submitted -> Received. Only an admin can do this, and only once per donation.
    [HttpPost]
    public async Task<IActionResult> Verify(int id)
    {
        var donation = await _db.Donations.FindAsync(id);
        if (donation is null)
            return NotFound();

        if (donation.IsMoney && !donation.HasProof)
        {
            TempData["Error"] = "This money donation has no proof of payment, so it cannot be verified.";
            return RedirectToAction(nameof(Donation), new { id });
        }

        if (donation.Status == DonationStatus.Submitted)
        {
            donation.Status = DonationStatus.Received;
            donation.VerifiedByAdminId = _userManager.GetUserId(User);
            donation.DateVerified = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Donation marked as Received.";
        }

        return RedirectToAction(nameof(Donation), new { id });
    }
}