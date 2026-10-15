using System.ComponentModel.DataAnnotations;

namespace ElamIgugu.Web.Models.ViewModels;

/// <summary>The fields posted when someone registers for a webinar.</summary>
public class RsvpViewModel
{
    [Required]
    public int WebinarId { get; set; }

    [Required, StringLength(100)]
    public string AttendeeName { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\+?[0-9 ]{9,15}$")]
    public string ContactNumber { get; set; } = string.Empty;
}
