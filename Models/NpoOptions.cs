namespace ElamIgugu.Web.Models;

/// <summary>
/// The NPO's public details, bound from the "Npo" section of appsettings.json,
/// so they can be changed without touching any code.
/// </summary>
public class NpoOptions
{
    public string InstagramHandle { get; set; } = string.Empty;
    public List<string> PhoneNumbers { get; set; } = new();
    public string Email { get; set; } = string.Empty;
    public string Slogan { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public BankOptions Bank { get; set; } = new();
}

/// <summary>The NPO's bank account, as printed on its flyers. Shown to donors who choose to give money.</summary>
public class BankOptions
{
    public string BankName { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
}
