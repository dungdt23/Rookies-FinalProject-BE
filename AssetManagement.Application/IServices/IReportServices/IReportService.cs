using ClosedXML.Excel;

namespace AssetManagement.Application.IServices.IReportServices
{
    public interface IReportService
    {
        Task<XLWorkbook> ExportAssetManagementFile(Guid locationId);

    }
}
