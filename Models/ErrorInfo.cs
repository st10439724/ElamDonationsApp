namespace ElamIgugu.Web.Models;

/// <summary>Passed to the generic error page so a problem can be traced in the logs.</summary>
public class ErrorInfo
{
    public string? RequestId { get; set; }
}
