using System.Globalization;

namespace ElamIgugu.Web.Helpers;

/// <summary>Shows an amount of money as South African rand, e.g. R1,500.</summary>
public static class Rand
{
    public static string Format(decimal? amount) =>
        amount is null ? string.Empty : "R" + amount.Value.ToString("N0", CultureInfo.InvariantCulture);
}
