using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ElamIgugu.Web.Models;

/// <summary>
/// A person who can sign in. Extends Identity's built-in user (which already holds
/// Email, PhoneNumber, PasswordHash, lockout fields and the TwoFactorEnabled flag).
/// Donors and admins are both ApplicationUsers; their role decides what they can see.
/// The contact number is stored in Identity's PhoneNumber field.
/// </summary>
public class ApplicationUser : IdentityUser
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public ICollection<Donation> Donations { get; set; } = new List<Donation>();
}
