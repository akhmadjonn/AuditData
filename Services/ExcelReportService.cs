using AuditData.Data;
using AuditData.DTOs;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace AuditData.Services;

public class ExcelReportService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ExcelReportService> _logger;

    public ExcelReportService(ApplicationDbContext context, ILogger<ExcelReportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<byte[]> GenerateAlreadyPaidOrdersReportAsync()
    {
        _logger.LogInformation("Generating AlreadyPaidOrders report");

        var spreadsheets = await _context.Spreadsheets.ToListAsync();
        var payments = await _context.Payments.ToListAsync();

        var results = new List<OrderPaymentReportDto>();

        foreach (var spreadsheet in spreadsheets)
        {
            // Case 1: Spreadsheets.LoadId == Payments.LoadNumber && Payments.SheetName == 'Pmt'
            var case1Match = payments.FirstOrDefault(p =>
                p.LoadNumber == spreadsheet.LoadId &&
                p.SheetName == "Pmt");

            if (case1Match != null)
            {
                results.Add(MapToReportDto(spreadsheet, case1Match, "Pmt Match"));
                continue;
            }

            // Case 2: Spreadsheets.LoadId == Payments.InvoiceNumber && Payments.SheetName == 'NF'
            var case2Match = payments.FirstOrDefault(p =>
                p.InvoiceNumber == spreadsheet.LoadId &&
                p.SheetName == "NF");

            if (case2Match != null)
            {
                results.Add(MapToReportDto(spreadsheet, case2Match, "NF Match"));
                continue;
            }

            // Case 3: Spreadsheets.LoadId == Payments.LoadNumber && Payments.SheetName == 'OP  SP' && Payments.InvoiceAmount >= Spreadsheets.InvoicedAmount
            var case3Match = payments.FirstOrDefault(p =>
                p.LoadNumber == spreadsheet.LoadId &&
                p.SheetName == "OP  SP" &&
                p.InvoiceAmount >= spreadsheet.InvoicedAmount);

            if (case3Match != null)
            {
                results.Add(MapToReportDto(spreadsheet, case3Match, "OP SP Full Payment"));
                continue;
            }
        }

        return GenerateExcelFile(results, "Already Paid Orders");
    }

    public async Task<byte[]> GenerateShortPaidOrdersReportAsync()
    {
        _logger.LogInformation("Generating ShortPaidOrders report");

        var spreadsheets = await _context.Spreadsheets.ToListAsync();
        var payments = await _context.Payments.ToListAsync();

        var results = new List<OrderPaymentReportDto>();

        foreach (var spreadsheet in spreadsheets)
        {
            // Spreadsheets.LoadId == Payments.LoadNumber && Payments.SheetName == 'OP  SP' && Payments.InvoiceAmount < Spreadsheets.InvoicedAmount
            var match = payments.FirstOrDefault(p =>
                p.LoadNumber == spreadsheet.LoadId &&
                p.SheetName == "OP  SP" &&
                p.InvoiceAmount < spreadsheet.InvoicedAmount);

            if (match != null)
            {
                results.Add(MapToReportDto(spreadsheet, match, "Short Payment"));
            }
        }

        return GenerateExcelFile(results, "Short Paid Orders");
    }

    public async Task<byte[]> GenerateChargeBackOrdersReportAsync()
    {
        _logger.LogInformation("Generating ChargeBackOrders report");

        var spreadsheets = await _context.Spreadsheets.ToListAsync();
        var payments = await _context.Payments.ToListAsync();

        var results = new List<OrderPaymentReportDto>();

        foreach (var spreadsheet in spreadsheets)
        {
            // Spreadsheets.LoadId == Payments.LoadNumber && Payments.SheetName == 'CB'
            var match = payments.FirstOrDefault(p =>
                p.LoadNumber == spreadsheet.LoadId &&
                p.SheetName == "CB");

            if (match != null)
            {
                results.Add(MapToReportDto(spreadsheet, match, "Charge Back"));
            }
        }

        return GenerateExcelFile(results, "Charge Back Orders");
    }

    public async Task<byte[]> GenerateUnpaidOrdersReportAsync()
    {
        _logger.LogInformation("Generating UnpaidOrders report");

        var spreadsheets = await _context.Spreadsheets.ToListAsync();
        var payments = await _context.Payments.ToListAsync();

        var results = new List<OrderPaymentReportDto>();

        foreach (var spreadsheet in spreadsheets)
        {
            bool isMatched = false;

            // Check if matched in any of the previous reports
            // Case 1: Pmt
            if (payments.Any(p => p.LoadNumber == spreadsheet.LoadId && p.SheetName == "Pmt"))
            {
                isMatched = true;
            }

            // Case 2: NF
            if (!isMatched && payments.Any(p => p.InvoiceNumber == spreadsheet.LoadId && p.SheetName == "NF"))
            {
                isMatched = true;
            }

            // Case 3: OP SP Full Payment
            if (!isMatched && payments.Any(p => p.LoadNumber == spreadsheet.LoadId && p.SheetName == "OP  SP" && p.InvoiceAmount >= spreadsheet.InvoicedAmount))
            {
                isMatched = true;
            }

            // Case 4: OP SP Short Payment
            if (!isMatched && payments.Any(p => p.LoadNumber == spreadsheet.LoadId && p.SheetName == "OP  SP" && p.InvoiceAmount < spreadsheet.InvoicedAmount))
            {
                isMatched = true;
            }

            // Case 5: CB
            if (!isMatched && payments.Any(p => p.LoadNumber == spreadsheet.LoadId && p.SheetName == "CB"))
            {
                isMatched = true;
            }

            // If not matched in any case, add to unpaid orders
            if (!isMatched)
            {
                results.Add(MapToReportDto(spreadsheet, null, "Unpaid"));
            }
        }

        return GenerateExcelFile(results, "Unpaid Orders");
    }

    private OrderPaymentReportDto MapToReportDto(Models.Spreadsheet spreadsheet, Models.Payment? payment, string matchType)
    {
        var dto = new OrderPaymentReportDto
        {
            // From Spreadsheets
            SpreadsheetId = spreadsheet.Id,
            Broker = spreadsheet.Broker,
            LoadId = spreadsheet.LoadId,
            PUDate = spreadsheet.PUDate,
            Origin = spreadsheet.Origin,
            DELDate = spreadsheet.DELDate,
            Destination = spreadsheet.Destination,
            Mileage = spreadsheet.Mileage,
            PerMile = spreadsheet.PerMile,
            Rate = spreadsheet.Rate,
            Dispatcher = spreadsheet.Dispatcher,
            Status = spreadsheet.Status,
            InvoicedAmount = spreadsheet.InvoicedAmount,
            DispatchNotes = spreadsheet.DispatchNotes,
            AccountingNotes = spreadsheet.AccountingNotes,
            MatchType = matchType
        };

        if (payment != null)
        {
            // From Payments
            dto.PaymentId = payment.Id;
            dto.InvoiceNumber = payment.InvoiceNumber;
            dto.LoadNumber = payment.LoadNumber;
            dto.PurchaseDate = payment.PurchaseDate;
            dto.PaymentDate = payment.PaymentDate;
            dto.CheckNumber = payment.CheckNumber;
            dto.DebtorName = payment.DebtorName;
            dto.FeeDays = payment.FeeDays;
            dto.InvoiceAmount = payment.InvoiceAmount;
            dto.ActivityType = payment.ActivityType;
            dto.CheckAmount = payment.CheckAmount;
            dto.SheetName = payment.SheetName;

            // Calculate difference
            dto.Difference = (spreadsheet.InvoicedAmount ?? 0) - (payment.InvoiceAmount ?? 0);
        }

        return dto;
    }

    private byte[] GenerateExcelFile(List<OrderPaymentReportDto> data, string sheetName)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add(sheetName);

        // Headers
        worksheet.Cells[1, 1].Value = "Match Type";
        worksheet.Cells[1, 2].Value = "Broker";
        worksheet.Cells[1, 3].Value = "Load ID";
        worksheet.Cells[1, 4].Value = "PU Date";
        worksheet.Cells[1, 5].Value = "Origin";
        worksheet.Cells[1, 6].Value = "DEL Date";
        worksheet.Cells[1, 7].Value = "Destination";
        worksheet.Cells[1, 8].Value = "Mileage";
        worksheet.Cells[1, 9].Value = "Per Mile";
        worksheet.Cells[1, 10].Value = "Rate";
        worksheet.Cells[1, 11].Value = "Dispatcher";
        worksheet.Cells[1, 12].Value = "Status";
        worksheet.Cells[1, 13].Value = "Invoiced Amount";
        worksheet.Cells[1, 14].Value = "Dispatch Notes";
        worksheet.Cells[1, 15].Value = "Accounting Notes";
        worksheet.Cells[1, 16].Value = "Invoice Number";
        worksheet.Cells[1, 17].Value = "Load Number";
        worksheet.Cells[1, 18].Value = "Purchase Date";
        worksheet.Cells[1, 19].Value = "Payment Date";
        worksheet.Cells[1, 20].Value = "Check Number";
        worksheet.Cells[1, 21].Value = "Debtor Name";
        worksheet.Cells[1, 22].Value = "Fee Days";
        worksheet.Cells[1, 23].Value = "Invoice Amount (Payment)";
        worksheet.Cells[1, 24].Value = "Activity Type";
        worksheet.Cells[1, 25].Value = "Check Amount";
        worksheet.Cells[1, 26].Value = "Sheet Name";
        worksheet.Cells[1, 27].Value = "Difference";

        // Style header row
        using (var range = worksheet.Cells[1, 1, 1, 27])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
        }

        // Data rows
        int row = 2;
        foreach (var item in data)
        {
            worksheet.Cells[row, 1].Value = item.MatchType;
            worksheet.Cells[row, 2].Value = item.Broker;
            worksheet.Cells[row, 3].Value = item.LoadId;
            worksheet.Cells[row, 4].Value = item.PUDate;
            worksheet.Cells[row, 5].Value = item.Origin;
            worksheet.Cells[row, 6].Value = item.DELDate;
            worksheet.Cells[row, 7].Value = item.Destination;
            worksheet.Cells[row, 8].Value = item.Mileage;
            worksheet.Cells[row, 9].Value = item.PerMile;
            worksheet.Cells[row, 10].Value = item.Rate;
            worksheet.Cells[row, 11].Value = item.Dispatcher;
            worksheet.Cells[row, 12].Value = item.Status;
            worksheet.Cells[row, 13].Value = item.InvoicedAmount;
            worksheet.Cells[row, 14].Value = item.DispatchNotes;
            worksheet.Cells[row, 15].Value = item.AccountingNotes;
            worksheet.Cells[row, 16].Value = item.InvoiceNumber;
            worksheet.Cells[row, 17].Value = item.LoadNumber;
            worksheet.Cells[row, 18].Value = item.PurchaseDate;
            worksheet.Cells[row, 19].Value = item.PaymentDate;
            worksheet.Cells[row, 20].Value = item.CheckNumber;
            worksheet.Cells[row, 21].Value = item.DebtorName;
            worksheet.Cells[row, 22].Value = item.FeeDays;
            worksheet.Cells[row, 23].Value = item.InvoiceAmount;
            worksheet.Cells[row, 24].Value = item.ActivityType;
            worksheet.Cells[row, 25].Value = item.CheckAmount;
            worksheet.Cells[row, 26].Value = item.SheetName;
            worksheet.Cells[row, 27].Value = item.Difference;
            row++;
        }

        // Auto-fit columns
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        return package.GetAsByteArray();
    }
}
