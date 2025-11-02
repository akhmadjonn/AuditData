using AuditData.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuditData.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly ExcelReportService _reportService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(ExcelReportService reportService, ILogger<ReportsController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    [HttpGet("already-paid-orders")]
    public async Task<IActionResult> DownloadAlreadyPaidOrders()
    {
        try
        {
            _logger.LogInformation("Generating AlreadyPaidOrders report");
            var fileBytes = await _reportService.GenerateAlreadyPaidOrdersReportAsync();
            var fileName = $"AlreadyPaidOrders_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating AlreadyPaidOrders report");
            return StatusCode(500, new { success = false, message = "Error generating report" });
        }
    }

    [HttpGet("short-paid-orders")]
    public async Task<IActionResult> DownloadShortPaidOrders()
    {
        try
        {
            _logger.LogInformation("Generating ShortPaidOrders report");
            var fileBytes = await _reportService.GenerateShortPaidOrdersReportAsync();
            var fileName = $"ShortPaidOrders_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating ShortPaidOrders report");
            return StatusCode(500, new { success = false, message = "Error generating report" });
        }
    }

    [HttpGet("charge-back-orders")]
    public async Task<IActionResult> DownloadChargeBackOrders()
    {
        try
        {
            _logger.LogInformation("Generating ChargeBackOrders report");
            var fileBytes = await _reportService.GenerateChargeBackOrdersReportAsync();
            var fileName = $"ChargeBackOrders_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating ChargeBackOrders report");
            return StatusCode(500, new { success = false, message = "Error generating report" });
        }
    }

    [HttpGet("unpaid-orders")]
    public async Task<IActionResult> DownloadUnpaidOrders()
    {
        try
        {
            _logger.LogInformation("Generating UnpaidOrders report");
            var fileBytes = await _reportService.GenerateUnpaidOrdersReportAsync();
            var fileName = $"UnpaidOrders_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating UnpaidOrders report");
            return StatusCode(500, new { success = false, message = "Error generating report" });
        }
    }
}
