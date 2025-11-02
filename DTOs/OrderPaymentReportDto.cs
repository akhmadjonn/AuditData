namespace AuditData.DTOs;

public class OrderPaymentReportDto
{
    // From Spreadsheets
    public int SpreadsheetId { get; set; }
    public string? Broker { get; set; }
    public string? LoadId { get; set; }
    public string? PUDate { get; set; }
    public string? Origin { get; set; }
    public string? DELDate { get; set; }
    public string? Destination { get; set; }
    public int? Mileage { get; set; }
    public decimal? PerMile { get; set; }
    public decimal? Rate { get; set; }
    public string? Dispatcher { get; set; }
    public string? Status { get; set; }
    public decimal? InvoicedAmount { get; set; }
    public string? DispatchNotes { get; set; }
    public string? AccountingNotes { get; set; }

    // From Payments
    public int? PaymentId { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? LoadNumber { get; set; }
    public string? PurchaseDate { get; set; }
    public string? PaymentDate { get; set; }
    public string? CheckNumber { get; set; }
    public string? DebtorName { get; set; }
    public int? FeeDays { get; set; }
    public decimal? InvoiceAmount { get; set; }
    public string? ActivityType { get; set; }
    public decimal? CheckAmount { get; set; }
    public string? SheetName { get; set; }

    // Calculated fields
    public decimal? Difference { get; set; }
    public string? MatchType { get; set; }
}
