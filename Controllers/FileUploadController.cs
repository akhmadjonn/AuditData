using AuditData.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuditData.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly ExcelImportService _importService;
    private readonly ILogger<FileUploadController> _logger;

    public FileUploadController(ExcelImportService importService, ILogger<FileUploadController> logger)
    {
        _importService = importService;
        _logger = logger;
    }

    [HttpPost("spreadsheets")]
    public async Task<IActionResult> UploadSpreadsheets(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { success = false, message = "No file uploaded." });
        }

        if (!IsExcelFile(file.FileName))
        {
            return BadRequest(new { success = false, message = "Only Excel files (.xlsx, .xls) are allowed." });
        }

        try
        {
            using var stream = file.OpenReadStream();
            var result = await _importService.ImportSpreadsheetsAsync(stream, file.FileName);

            if (result.Success)
            {
                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    recordsImported = result.RecordsImported
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.Message
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading spreadsheets file");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while processing the file."
            });
        }
    }

    [HttpPost("payments")]
    public async Task<IActionResult> UploadPayments(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { success = false, message = "No file uploaded." });
        }

        if (!IsExcelFile(file.FileName))
        {
            return BadRequest(new { success = false, message = "Only Excel files (.xlsx, .xls) are allowed." });
        }

        try
        {
            using var stream = file.OpenReadStream();
            var result = await _importService.ImportPaymentsAsync(stream, file.FileName);

            if (result.Success)
            {
                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    recordsImported = result.RecordsImported
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.Message
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading payments file");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while processing the file."
            });
        }
    }

    private bool IsExcelFile(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension == ".xlsx" || extension == ".xls";
    }
}
