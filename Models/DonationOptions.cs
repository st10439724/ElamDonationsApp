namespace ElamIgugu.Web.Models;

/// <summary>The fixed lists shown in the Donate pop-up. The server also checks against them.</summary>
public static class DonationOptions
{
    /// <summary>The item type that means "I will pay money by EFT".</summary>
    public const string Money = "Money (EFT)";

    public static readonly string[] ItemTypes = { "Clothes", "Non-perishable food", Money };

    public static readonly string[] Cities = { "PE", "Alice", "EL", "Joburg" };
}
