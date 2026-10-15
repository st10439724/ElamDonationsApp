using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElamIgugu.Web.Models;

/// <summary>The two statuses agreed with the client.</summary>
public enum DonationStatus
{
    Submitted = 0,  // donor has told the NPO what they plan to give (or pay)
    Received = 1    // an admin has confirmed the items or the payment arrived
}

/// <summary>One donation a donor has told the NPO about: goods or money.</summary>
public class Donation
{
    public int Id { get; set; }

    [Required]
    public string DonorId { get; set; } = string.Empty;
    public ApplicationUser? Donor { get; set; }

    [Required, StringLength(50)]
    public string ItemType { get; set; } = string.Empty;

    // Optional detail from the donor, e.g. "used blankets" or "new soccer shoes"
    [StringLength(200)]
    public string? Description { get; set; }

    // Only filled in for money donations (whole rand, stored with 2 decimals)
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Amount { get; set; }

    [Required, StringLength(30)]
    public string City { get; set; } = string.Empty;

    // Name of the uploaded proof-of-payment PDF (money donations only). The file itself is in DonationProof.
    [StringLength(100)]
    public string? ProofFileName { get; set; }
    public DonationProof? Proof { get; set; }

    public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;

    public DonationStatus Status { get; set; } = DonationStatus.Submitted;

    // Filled in when an admin clicks Verify
    public string? VerifiedByAdminId { get; set; }
    public DateTime? DateVerified { get; set; }

    // Not stored in the database
    [NotMapped]
    public bool HasProof => !string.IsNullOrEmpty(ProofFileName);

    [NotMapped]
    public bool IsMoney => ItemType == DonationOptions.Money;

    
    [NotMapped]
    public string Reference => $"EIL-{Id:D4}";
}