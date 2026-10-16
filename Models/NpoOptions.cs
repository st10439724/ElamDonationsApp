namespace ElamIgugu.Web.Models;


public class NpoOptions
{
    public string InstagramHandle { get; set; } = string.Empty;
    public List<string> PhoneNumbers { get; set; } = new();
    public string Email { get; set; } = string.Empty;
    public string DropOffAddress { get; set; } = string.Empty;
    public string Slogan { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public BankOptions Bank { get; set; } = new();
}


public class BankOptions
{
    public string BankName { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
}