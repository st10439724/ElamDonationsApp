namespace ElamIgugu.Web.Helpers;

/// <summary>
/// Dates are stored in UTC. This shows them in South African time (UTC+2), whether the
/// app runs on Windows ("South Africa Standard Time") or Linux ("Africa/Johannesburg").
/// </summary>
public static class SaTime
{
    private static readonly TimeZoneInfo Zone = FindZone();

    private static TimeZoneInfo FindZone()
    {
        foreach (var id in new[] { "South Africa Standard Time", "Africa/Johannesburg" })
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
            catch (TimeZoneNotFoundException) { }
            catch (InvalidTimeZoneException) { }
        }
        return TimeZoneInfo.Utc;
    }

    public static string Format(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Zone)
                    .ToString("dd MMM yyyy");
}
