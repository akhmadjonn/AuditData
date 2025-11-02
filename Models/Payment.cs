using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditData.Models;

public class Payment
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string? InvoiceNumber { get; set; }

    [MaxLength(100)]
    public string? LoadNumber { get; set; }

    [MaxLength(50)]
    public string? PurchaseDate { get; set; }

    [MaxLength(50)]
    public string? PaymentDate { get; set; }

    [MaxLength(200)]
    public string? CheckNumber { get; set; }

    [MaxLength(300)]
    public string? DebtorName { get; set; }

    public int? FeeDays { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? InvoiceAmount { get; set; }

    [MaxLength(50)]
    public string? ActivityType { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? CheckAmount { get; set; }

    [Required]
    [MaxLength(100)]
    public string SheetName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? CreatedAt { get; set; }
}
