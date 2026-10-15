namespace ElamIgugu.Web.Models.ViewModels;

public class DonorRow
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public int DonationCount { get; set; }
}

public class DonationRow
{
    public int Id { get; set; }
    public string DonorName { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public string City { get; set; } = string.Empty;
    public DateTime DateSubmitted { get; set; }
    public DonationStatus Status { get; set; }
    public string? ProofFileName { get; set; }

    public bool IsMoney => ItemType == DonationOptions.Money;
    public string Reference => $"EIL-{Id:D4}";
}

public class AdminDashboardViewModel
{
    public string Tab { get; set; } = "donations";      // "donations" or "donors"
    public string StatusFilter { get; set; } = "All";   // "All", "Submitted" or "Received"

    public int DonorCount { get; set; }
    public int SubmittedCount { get; set; }
    public int ReceivedCount { get; set; }
    public decimal MoneyReceived { get; set; }          // total of verified (Received) money donations

    public List<DonorRow> Donors { get; set; } = new();
    public List<DonationRow> Donations { get; set; } = new();
}

/// <summary>The "one donor" screen: who they are and all of their donations.</summary>
public class DonorDetailViewModel
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public List<DonationRow> Donations { get; set; } = new();
}

/// <summary>The "one donation" screen where the admin checks the details and verifies.</summary>
public class DonationDetailViewModel
{
    public int Id { get; set; }
    public string DonorId { get; set; } = string.Empty;
    public string DonorName { get; set; } = string.Empty;
    public string DonorPhone { get; set; } = string.Empty;
    public string DonorEmail { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public string City { get; set; } = string.Empty;
    public DateTime DateSubmitted { get; set; }
    public DateTime? DateVerified { get; set; }
    public DonationStatus Status { get; set; }
    public string? ProofFileName { get; set; }

    public bool IsMoney => ItemType == DonationOptions.Money;
    public string Reference => $"EIL-{Id:D4}";
}





















































































//namespace ElamIgugu.Web.Models.ViewModels;

//public class DonorRow
//{
//    public string FullName { get; set; } = string.Empty;
//    public string ContactNumber { get; set; } = string.Empty;
//    public string Email { get; set; } = string.Empty;
//    public DateTime CreatedDate { get; set; }
//    public int DonationCount { get; set; }
//}

//public class DonationRow
//{
//    public int Id { get; set; }
//    public string DonorName { get; set; } = string.Empty;
//    public string ItemType { get; set; } = string.Empty;
//    public string? Description { get; set; }
//    public decimal? Amount { get; set; }
//    public string City { get; set; } = string.Empty;
//    public DateTime DateSubmitted { get; set; }
//    public DonationStatus Status { get; set; }

//    public bool IsMoney => ItemType == DonationOptions.Money;
//    public string Reference => $"EIL-{Id:D4}";
//}

//public class AdminDashboardViewModel
//{
//    public string Tab { get; set; } = "donations";      // "donations" or "donors"
//    public string StatusFilter { get; set; } = "All";   // "All", "Submitted" or "Received"

//    public int DonorCount { get; set; }
//    public int SubmittedCount { get; set; }
//    public int ReceivedCount { get; set; }
//    public decimal MoneyReceived { get; set; }          // total of verified (Received) money donations

//    public List<DonorRow> Donors { get; set; } = new();
//    public List<DonationRow> Donations { get; set; } = new();
//}
