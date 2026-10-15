using System.ComponentModel.DataAnnotations;

namespace ElamIgugu.Web.Models;

/// <summary>A sign-up for a webinar. No account is needed, so it only stores a name and number.</summary>
public class WebinarRsvp
{
    public int Id { get; set; }

    public int WebinarId { get; set; }
    public Webinar? Webinar { get; set; }

    [Required, StringLength(100)]
    public string AttendeeName { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string ContactNumber { get; set; } = string.Empty;

    public DateTime RsvpDate { get; set; } = DateTime.UtcNow;
}
