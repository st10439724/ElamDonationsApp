using ElamIgugu.Web.Data;
using ElamIgugu.Web.Helpers;
using ElamIgugu.Web.Models;
using ElamIgugu.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ElamIgugu.Web.Controllers;

/// <summary>Donor-only pages. Admins are turned away automatically by the role check.</summary>
[Authorize(Roles = AppRoles.Donor)]
public class DonorController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly NpoOptions _npo;
    private const int MaxProofBytes = 2 * 1024 * 1024;

    public DonorController(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager,
        IOptions<NpoOptions> npo)
    {
        _db = db;
        _userManager = userManager;
        _npo = npo.Value;
    }

    // The donor page: welcome, Donate / Drop Off button, and "My donations"
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;
        var user = await _userManager.FindByIdAsync(userId);

        // A donor can only ever see their OWN donations (filtered by DonorId)
        var donations = await _db.Donations
            .AsNoTracking()
            .Where(d => d.DonorId == userId)
            .OrderByDescending(d => d.DateSubmitted)
            .ToListAsync();

        return View(new DonorDashboardViewModel
        {
            FullName = user?.FullName ?? string.Empty,
            Donations = donations
        });
    }

    // Posted from the pop-up: saves a new donation (goods or money) with status Submitted
    [HttpPost]
    public async Task<IActionResult> Create(CreateDonationViewModel model)
    {
        if (!DonationOptions.ItemTypes.Contains(model.ItemType))
        {
            TempData["Error"] = "Please choose what you are donating.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please check your details (the description can be up to 200 characters).";
            return RedirectToAction(nameof(Index));
        }

        var isMoney = model.ItemType == DonationOptions.Money;
        decimal? amount = null;
        string city;
        DonationProof? proof = null;

        if (isMoney)
        {
            if (model.Amount is null or < 1 or > 1_000_000)
            {
                TempData["Error"] = "Please enter the amount you plan to pay, in whole rand (R1 to R1,000,000).";
                return RedirectToAction(nameof(Index));
            }

            amount = model.Amount.Value;

            // A money donation needs a proof-of-payment PDF (max 2 MB)
            var file = model.Proof;
            if (file is null || file.Length == 0)
            {
                TempData["Error"] = "Please upload your proof of payment (a PDF).";
                return RedirectToAction(nameof(Index));
            }
            if (file.Length > MaxProofBytes)
            {
                TempData["Error"] = "The proof of payment is too big. Please keep it under 2 MB.";
                return RedirectToAction(nameof(Index));
            }

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var bytes = ms.ToArray();

            // Must end in .pdf AND really start like a PDF file ("%PDF")
            var looksLikePdf = bytes.Length > 4 && bytes[0] == 0x25 && bytes[1] == 0x50 && bytes[2] == 0x44 && bytes[3] == 0x46;
            if (!Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase) || !looksLikePdf)
            {
                TempData["Error"] = "The proof of payment must be a PDF file.";
                return RedirectToAction(nameof(Index));
            }

            var safeName = Path.GetFileName(file.FileName);
            if (safeName.Length > 100) safeName = safeName[^100..];
            proof = new DonationProof { FileName = safeName, Data = bytes };
            city = "N/A";   // a bank payment has no collection city
        }
        else
        {
            if (model.City is null || !DonationOptions.Cities.Contains(model.City))
            {
                TempData["Error"] = "Please choose your city.";
                return RedirectToAction(nameof(Index));
            }

            city = model.City;
        }

        var donation = new Donation
        {
            DonorId = _userManager.GetUserId(User)!,   // taken from the sign-in, never from the form
            ItemType = model.ItemType,
            Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
            Amount = amount,
            ProofFileName = proof?.FileName,
            Proof = proof,   // saved in the same step as the donation
            City = city,
            DateSubmitted = DateTime.UtcNow,
            Status = DonationStatus.Submitted
        };

        _db.Donations.Add(donation);
        await _db.SaveChangesAsync();   // the Id (and so the Reference) exists after this

        if (isMoney)
        {
            TempData["Success"] =
                $"Thank you! Your {Rand.Format(amount)} donation is saved as Submitted with reference {donation.Reference}, " +
                "and your proof of payment is attached. An admin will check it and mark it Received.";
        }
        else
        {
            TempData["Success"] = "Your donation is saved as Submitted. Now contact the NPO to arrange the hand-over.";
        }

        return RedirectToAction(nameof(Index));
    }
}












































































































//using ElamIgugu.Web.Data;
//using ElamIgugu.Web.Helpers;
//using ElamIgugu.Web.Models;
//using ElamIgugu.Web.Models.ViewModels;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Options;

//namespace ElamIgugu.Web.Controllers;

///// <summary>Donor-only pages. Admins are turned away automatically by the role check.</summary>
//[Authorize(Roles = AppRoles.Donor)]
//public class DonorController : Controller
//{
//    private readonly ApplicationDbContext _db;
//    private readonly UserManager<ApplicationUser> _userManager;
//    private readonly NpoOptions _npo;

//    public DonorController(
//        ApplicationDbContext db,
//        UserManager<ApplicationUser> userManager,
//        IOptions<NpoOptions> npo)
//    {
//        _db = db;
//        _userManager = userManager;
//        _npo = npo.Value;
//    }

//    // The donor page: welcome, Donate / Drop Off button, and "My donations"
//    public async Task<IActionResult> Index()
//    {
//        var userId = _userManager.GetUserId(User)!;
//        var user = await _userManager.FindByIdAsync(userId);

//        // A donor can only ever see their OWN donations (filtered by DonorId)
//        var donations = await _db.Donations
//            .AsNoTracking()
//            .Where(d => d.DonorId == userId)
//            .OrderByDescending(d => d.DateSubmitted)
//            .ToListAsync();

//        return View(new DonorDashboardViewModel
//        {
//            FullName = user?.FullName ?? string.Empty,
//            Donations = donations
//        });
//    }

//    // Posted from the pop-up: saves a new donation (goods or money) with status Submitted
//    [HttpPost]
//    public async Task<IActionResult> Create(CreateDonationViewModel model)
//    {
//        if (!ModelState.IsValid || !DonationOptions.ItemTypes.Contains(model.ItemType))
//        {
//            TempData["Error"] = "Please choose what you are donating.";
//            return RedirectToAction(nameof(Index));
//        }

//        var isMoney = model.ItemType == DonationOptions.Money;
//        decimal? amount = null;
//        string city;

//        if (isMoney)
//        {
//            if (model.Amount is null or < 1 or > 1_000_000)
//            {
//                TempData["Error"] = "Please enter the amount you plan to pay, in whole rand (R1 to R1,000,000).";
//                return RedirectToAction(nameof(Index));
//            }

//            amount = model.Amount.Value;
//            city = "N/A";   // a bank payment has no collection city
//        }
//        else
//        {
//            if (!DonationOptions.Cities.Contains(model.City))
//            {
//                TempData["Error"] = "Please choose your city.";
//                return RedirectToAction(nameof(Index));
//            }

//            city = model.City;
//        }

//        var donation = new Donation
//        {
//            DonorId = _userManager.GetUserId(User)!,   // taken from the sign-in, never from the form
//            ItemType = model.ItemType,
//            Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
//            Amount = amount,
//            City = city,
//            DateSubmitted = DateTime.UtcNow,
//            Status = DonationStatus.Submitted
//        };

//        _db.Donations.Add(donation);
//        await _db.SaveChangesAsync();   // the Id (and so the Reference) exists after this

//        if (isMoney)
//        {
//            var bank = _npo.Bank;
//            TempData["Success"] =
//                $"Saved as Submitted. Please pay {Rand.Format(amount)} by EFT to {bank.BankName}, " +
//                $"account name {bank.AccountName}, account number {bank.AccountNumber}, " +
//                $"using the reference {donation.Reference}. An admin marks it Received once the payment shows on the bank statement.";
//        }
//        else
//        {
//            TempData["Success"] = "Your donation is saved as Submitted. Now contact the NPO to arrange the hand-over.";
//        }

//        return RedirectToAction(nameof(Index));
//    }
//}
