namespace AssetManagement.Application.Dtos.Common
{
    public class ResponsePaginatedResultDto<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
    }
}
