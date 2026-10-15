using System.ComponentModel.DataAnnotations;

namespace ElamIgugu.Web.Models;

//fix it 
public class DonationProof
{
    [Key]
    public int DonationId { get; set; }          // same Id as the donation it belongs to - breaking ill fix 
    public Donation? Donation { get; set; }

    [Required, StringLength(100)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    public byte[] Data { get; set; } = Array.Empty<byte>();
}