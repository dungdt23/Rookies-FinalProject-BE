using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices.IReportServices;
using AssetManagement.Domain.Entities;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AssetManagement.Application.Services.ReportServices
{
    public class ReportService : IReportService
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        public ReportService(IGenericRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<XLWorkbook> ExportAssetManagementFile(Guid locationId)
        {
            var assetData = await GetAssetsData(locationId);
            XLWorkbook wb = new XLWorkbook();
            var worksheet = wb.AddWorksheet(assetData, "Asset Management");

            //style for header of excel file
            string colorHex = "#c6e0b4";
            XLColor fillColor = XLColor.FromHtml(colorHex);
            var headerRow = worksheet.Row(1);
            headerRow.Style.Fill.BackgroundColor = fillColor;
            headerRow.Style.Font.FontColor = XLColor.SmokyBlack;
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRow.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            headerRow.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            headerRow.Style.Border.OutsideBorderColor = XLColor.Black;

            return wb;
        }
        private async Task<DataTable> GetAssetsData(Guid locationId)
        {
            DataTable table = new DataTable();
            table.TableName = "Asset Management";
            table.Columns.Add("Category");
            table.Columns.Add("Total");
            table.Columns.Add("Assigned");
            table.Columns.Add("Available");
            table.Columns.Add("Not available");
            table.Columns.Add("Waiting for recycling");
            table.Columns.Add("Recycled");
            var categories = await _categoryRepository.GetByCondition(c => !c.IsDeleted)
                .Include(c => c.Assets)
                .ToListAsync();
            foreach (var category in categories) 
            {
                DataRow row = table.NewRow();
                row["Category"] = category.CategoryName;
                row["Total"] = category.Assets.Where(x => !x.IsDeleted && x.LocationId == locationId).Count();
                row["Assigned"] = category.Assets.Where(x => !x.IsDeleted && x.LocationId == locationId && x.State == Domain.Enums.TypeAssetState.Assigned).Count();
                row["Available"] = category.Assets.Where(x => !x.IsDeleted && x.LocationId == locationId && x.State == Domain.Enums.TypeAssetState.Available).Count();
                row["Not available"] = category.Assets.Where(x => !x.IsDeleted && x.LocationId == locationId && x.State == Domain.Enums.TypeAssetState.NotAvailable).Count();
                row["Waiting for recycling"] = category.Assets.Where(x => !x.IsDeleted && x.LocationId == locationId && x.State == Domain.Enums.TypeAssetState.WaitingForRecycling).Count();
                row["Recycled"] = category.Assets.Where(x => !x.IsDeleted && x.LocationId == locationId && x.State == Domain.Enums.TypeAssetState.Recycled).Count();
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
