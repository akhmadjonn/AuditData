using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditData.Models;

public class Spreadsheet
{
    [Key]
    public int Id { get; set; }

    [MaxLength(200)]
    public string? Broker { get; set; }

    [MaxLength(100)]
    public string? LoadId { get; set; }

    public DateTime? PUDate { get; set; }

    [MaxLength(300)]
    public string? Origin { get; set; }

    public DateTime? DELDate { get; set; }

    [MaxLength(300)]
    public string? Destination { get; set; }

    public int? Mileage { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PerMile { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Rate { get; set; }

    [MaxLength(100)]
    public string? Dispatcher { get; set; }

    [MaxLength(100)]
    public string? Status { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? InvoicedAmount { get; set; }

    public string? DispatchNotes { get; set; }

    public string? AccountingNotes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
