using AuditData.Models;
using Microsoft.EntityFrameworkCore;

namespace AuditData.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Spreadsheet> Spreadsheets { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure indexes for better query performance
        modelBuilder.Entity<Spreadsheet>()
            .HasIndex(s => s.LoadId);

        modelBuilder.Entity<Spreadsheet>()
            .HasIndex(s => s.Status);

        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.InvoiceNumber);

        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.SheetName);

        modelBuilder.Entity<Payment>()
            .HasIndex(p => p.LoadNumber);
    }
}
