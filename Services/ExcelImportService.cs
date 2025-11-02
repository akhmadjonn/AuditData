using AuditData.Data;
using AuditData.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace AuditData.Services;

public class ExcelImportService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExcelImportService> _logger;

    public ExcelImportService(ApplicationDbContext context, ILogger<ExcelImportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(bool Success, string Message, int RecordsImported)> ImportSpreadsheetsAsync(Stream fileStream, string fileName)
    {
        try
        {
            using var package = new ExcelPackage(fileStream);

            // Get the first worksheet (Unique sheet)
            if (package.Workbook.Worksheets.Count == 0)
            {
                return (false, "Excel file contains no worksheets.", 0);
            }

            var worksheet = package.Workbook.Worksheets[0];
            _logger.LogInformation($"Processing sheet: {worksheet.Name}");

            var recordsImported = 0;
            var rowCount = worksheet.Dimension?.Rows ?? 0;

            if (rowCount <= 1)
            {
                return (false, "Excel file contains no data rows.", 0);
            }

            // Start from row 2 (skip header row)
            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    var spreadsheet = new Spreadsheet
                    {
                        Broker = GetCellValue(worksheet, row, 2),
                        LoadId = GetCellValue(worksheet, row, 3),
                        PUDate = GetDateStringValue(worksheet, row, 4),
                        Origin = GetCellValue(worksheet, row, 5),
                        DELDate = GetDateStringValue(worksheet, row, 6),
                        Destination = GetCellValue(worksheet, row, 7),
                        Mileage = GetIntValue(worksheet, row, 8),
                        PerMile = GetDecimalValue(worksheet, row, 9),
                        Rate = GetDecimalValue(worksheet, row, 10),
                        Dispatcher = GetCellValue(worksheet, row, 11),
                        Status = GetCellValue(worksheet, row, 12),
                        InvoicedAmount = GetDecimalValue(worksheet, row, 13),
                        DispatchNotes = GetCellValue(worksheet, row, 14),
                        AccountingNotes = GetCellValue(worksheet, row, 15),
                        CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    _context.Spreadsheets.Add(spreadsheet);
                    recordsImported++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Error processing row {row}: {ex.Message}");
                }
            }

            await _context.SaveChangesAsync();
            return (true, $"Successfully imported {recordsImported} records from {fileName}", recordsImported);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error importing spreadsheets file: {fileName}");
            return (false, $"Error: {ex.Message}", 0);
        }
    }

    public async Task<(bool Success, string Message, int RecordsImported)> ImportPaymentsAsync(Stream fileStream, string fileName)
    {
        try
        {
            using var package = new ExcelPackage(fileStream);

            if (package.Workbook.Worksheets.Count == 0)
            {
                return (false, "Excel file contains no worksheets.", 0);
            }

            var recordsImported = 0;

            // Process ALL sheets in the workbook
            foreach (var worksheet in package.Workbook.Worksheets)
            {
                _logger.LogInformation($"Processing sheet: {worksheet.Name}");

                var rowCount = worksheet.Dimension?.Rows ?? 0;

                // Find the header row (it's usually row 4 based on our analysis)
                int headerRow = FindHeaderRow(worksheet);
                if (headerRow == -1)
                {
                    _logger.LogWarning($"Could not find header row in sheet {worksheet.Name}");
                    continue;
                }

                // Start from the row after the header
                for (int row = headerRow + 1; row <= rowCount; row++)
                {
                    try
                    {
                        // Skip empty rows
                        var invoiceNumber = GetCellValue(worksheet, row, 2);
                        if (string.IsNullOrWhiteSpace(invoiceNumber))
                            continue;

                        var payment = new Payment
                        {
                            InvoiceNumber = invoiceNumber,
                            LoadNumber = GetCellValue(worksheet, row, 3),
                            PurchaseDate = GetDateStringValue(worksheet, row, 4),
                            PaymentDate = GetDateStringValue(worksheet, row, 5),
                            CheckNumber = GetCellValue(worksheet, row, 6),
                            DebtorName = GetCellValue(worksheet, row, 7),
                            FeeDays = GetIntValue(worksheet, row, 8),
                            InvoiceAmount = GetDecimalValue(worksheet, row, 9),
                            ActivityType = GetCellValue(worksheet, row, 10),
                            CheckAmount = GetDecimalValue(worksheet, row, 11),
                            SheetName = worksheet.Name,
                            CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                        };

                        _context.Payments.Add(payment);
                        recordsImported++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Error processing row {row} in sheet {worksheet.Name}: {ex.Message}");
                    }
                }
            }

            await _context.SaveChangesAsync();
            return (true, $"Successfully imported {recordsImported} records from {fileName}", recordsImported);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error importing payments file: {fileName}");
            return (false, $"Error: {ex.Message}", 0);
        }
    }

    private int FindHeaderRow(ExcelWorksheet worksheet)
    {
        // Look for the row containing "Invoice Number" which indicates the header
        for (int row = 1; row <= Math.Min(10, worksheet.Dimension?.Rows ?? 0); row++)
        {
            var cellValue = GetCellValue(worksheet, row, 2);
            if (cellValue != null && cellValue.Contains("Invoice Number", StringComparison.OrdinalIgnoreCase))
            {
                return row;
            }
        }
        return -1;
    }

    private string? GetCellValue(ExcelWorksheet worksheet, int row, int col)
    {
        try
        {
            var value = worksheet.Cells[row, col].Value;
            return value?.ToString()?.Trim();
        }
        catch
        {
            return null;
        }
    }

    private string? GetDateStringValue(ExcelWorksheet worksheet, int row, int col)
    {
        try
        {
            var value = worksheet.Cells[row, col].Value;
            if (value == null) return null;

            DateTime dateTime;

            if (value is DateTime dt)
            {
                dateTime = dt;
            }
            else if (DateTime.TryParse(value.ToString(), out DateTime parsed))
            {
                dateTime = parsed;
            }
            else
            {
                return null;
            }

            // Return date as string in ISO format
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        catch
        {
            return null;
        }
    }

    private int? GetIntValue(ExcelWorksheet worksheet, int row, int col)
    {
        try
        {
            var value = worksheet.Cells[row, col].Value;
            if (value == null) return null;

            if (int.TryParse(value.ToString(), out int result))
                return result;

            return null;
        }
        catch
        {
            return null;
        }
    }

    private decimal? GetDecimalValue(ExcelWorksheet worksheet, int row, int col)
    {
        try
        {
            var value = worksheet.Cells[row, col].Value;
            if (value == null) return null;

            if (decimal.TryParse(value.ToString(), out decimal result))
                return result;

            return null;
        }
        catch
        {
            return null;
        }
    }
}
