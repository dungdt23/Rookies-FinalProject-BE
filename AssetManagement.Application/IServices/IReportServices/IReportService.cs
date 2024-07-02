using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using ClosedXML.Excel;

namespace AssetManagement.Application.IServices.IReportServices
{
    public interface IReportService
    {
        Task<XLWorkbook> ExportAssetManagementFile(Guid locationId);
        Task<PagedResponse<ResponseReportDto>> GetReportData(Guid locationId, ReportFilter? filter, int? index, int? size);

    }
}
