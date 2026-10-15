using System.ComponentModel.DataAnnotations;

namespace ElamIgugu.Web.Models;

/// <summary>
/// A career day webinar. Loaded into the database by the seeder (there is no admin
/// screen for webinars in this version, as agreed). StartTime is stored as local
/// South African time exactly as entered.
/// </summary>
public class Webinar
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Topic { get; set; } = string.Empty;

    public DateTime StartTime { get; set; }

    [Required, StringLength(100)]
    public string Speaker { get; set; } = string.Empty;

    public ICollection<WebinarRsvp> Rsvps { get; set; } = new List<WebinarRsvp>();
}
