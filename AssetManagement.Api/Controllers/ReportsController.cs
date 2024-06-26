using AssetManagement.Application.IServices.IReportServices;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Api.Controllers
{
    [Route("reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportsController(IReportService reportService)
        {
            _reportService = reportService; 
        }
        [HttpGet("{locationId}")]
        public async Task<IActionResult> Get(Guid locationId) 
        { 
            XLWorkbook wb = await _reportService.ExportAssetManagementFile(locationId);
            MemoryStream ms = new MemoryStream();
            wb.SaveAs(ms);
            // Reset the stream position 
            ms.Position = 0;
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AssetManagement_Report.xlsx");
        }
    }
}
