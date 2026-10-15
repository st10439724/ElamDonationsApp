using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ElamIgugu.Web.Models.ViewModels;

/// <summary>What the Donor page needs: the donor's name and their own donations.</summary>
public class DonorDashboardViewModel
{
    public string FullName { get; set; } = string.Empty;
    public List<Donation> Donations { get; set; } = new();
}

/// <summary>The values chosen in the Donate / Drop Off pop-up.</summary>
public class CreateDonationViewModel
{
    [Required]
    public string ItemType { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Please keep the description under 200 characters.")]
    public string? Description { get; set; }

    // Whole rand. Only used when ItemType is Money (EFT).
    public int? Amount { get; set; }

    // The proof-of-payment PDF. Only needed for money donations, so it is checked in the controller.
    public IFormFile? Proof { get; set; }

    // Not needed for money donations, so it is checked in the controller instead of here
    public string? City { get; set; }
}