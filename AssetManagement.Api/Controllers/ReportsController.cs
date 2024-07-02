using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.IReportServices;
using AssetManagement.Domain.Constants;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
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
        [HttpGet("export")]
        [Authorize(Roles = TypeNameConstants.TypeAdmin)]
        public async Task<IActionResult> Get()
        {
            var locationIdClaim = HttpContext.GetClaim("locationId");
            var locationId = new Guid(locationIdClaim);
            XLWorkbook wb = await _reportService.ExportAssetManagementFile(locationId);
            MemoryStream ms = new MemoryStream();
            wb.SaveAs(ms);
            string formattedDate = DateTime.Now.ToString("dd/MM/yyyy");
            string fileName = "AssetManagement_Report_" + formattedDate + ".xlsx";
            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        [HttpGet]
        [Authorize(Roles = TypeNameConstants.TypeAdmin)]
        public async Task<IActionResult> Get([FromQuery] ReportFilter filter, int index = 1, int size = 10)
        {
            var locationIdClaim = HttpContext.GetClaim("locationId");
            var locationId = new Guid(locationIdClaim);
            var result = await _reportService.GetReportData(locationId, filter, index, size);
            return Ok(result);
        }
    }
}
