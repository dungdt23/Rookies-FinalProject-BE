using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices.IReportServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AutoMapper;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AssetManagement.Application.Services.ReportServices
{
    public class ReportService : IReportService
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;
        public ReportService(IGenericRepository<Category> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
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
            var reportResult = await GetReportData(locationId, null, null, null);
            var reportDatas = reportResult.Data;

            foreach (var data in reportDatas)
            {
                DataRow row = table.NewRow();
                row["Category"] = data.CategoryName;
                row["Total"] = data.Total;
                row["Assigned"] = data.Assigned;
                row["Available"] = data.Available;
                row["Not available"] = data.NotAvailable;
                row["Waiting for recycling"] = data.WaitingForRecycling;
                row["Recycled"] = data.Recycled;
                table.Rows.Add(row);
            }
            return table;
        }
        public async Task<PagedResponse<ResponseReportDto>> GetReportData(Guid locationId, ReportFilter? filter, int? index, int? size)
        {
            var query = _categoryRepository.GetByCondition(c => !c.IsDeleted && c.Assets.Any(a => a.LocationId == locationId))
                                                 .Include(c => c.Assets.Where(a => a.LocationId == locationId))
                                                 .AsNoTracking();
            var totalCount = query.Count();
            if (totalCount == 0) return new PagedResponse<ResponseReportDto>
            {
                TotalCount = totalCount,
                Message = ReportApiResponseMessageConstant.ReportGetNotFound
            };
            if (filter != null)
            {
                Func<Category, object> sortCondition;
                switch (filter.sortby)
                {
                    case TypeReportSort.CategoryName:
                        sortCondition = x => x.CategoryName;
                        break;
                    case TypeReportSort.Total:
                        sortCondition = x => x.Assets.Where(x => !x.IsDeleted).Count();
                        break;
                    case TypeReportSort.Assigned:
                        sortCondition = x => x.Assets.Where(x => !x.IsDeleted && x.State == Domain.Enums.TypeAssetState.Assigned).Count();
                        break;
                    case TypeReportSort.Available:
                        sortCondition = x => x.Assets.Where(x => !x.IsDeleted && x.State == Domain.Enums.TypeAssetState.Available).Count();
                        break;
                    case TypeReportSort.NotAvailable:
                        sortCondition = x => x.Assets.Where(x => !x.IsDeleted && x.State == Domain.Enums.TypeAssetState.NotAvailable).Count();
                        break;
                    case TypeReportSort.WaitingForRecycling:
                        sortCondition = x => x.Assets.Where(x => !x.IsDeleted && x.State == Domain.Enums.TypeAssetState.WaitingForRecycling).Count();
                        break;
                    case TypeReportSort.Recycled:
                        sortCondition = x => x.Assets.Where(x => !x.IsDeleted && x.State == Domain.Enums.TypeAssetState.Recycled).Count();
                        break;
                    default:
                        sortCondition = x => x.CategoryName;
                        break;
                }
                if (filter.ascending)
                {
                    query = query.OrderBy(sortCondition).AsQueryable();
                }
                else
                {
                    query = query.OrderByDescending(sortCondition).AsQueryable();
                }
            }
            if (index.HasValue && size.HasValue) query = query.Skip((index.Value - 1) * size.Value).Take(size.Value);
            var categories = query.ToList();
            var response = _mapper.Map<IEnumerable<ResponseReportDto>>(categories);
            return new PagedResponse<ResponseReportDto>
            {
                Data = response,
                TotalCount = totalCount,
                Message = ReportApiResponseMessageConstant.ReportGetSuccess
            };
        }
    }
}
