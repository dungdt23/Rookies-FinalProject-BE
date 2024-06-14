using Microsoft.AspNetCore.Http;

namespace AssetManagement.Application.ApiResponses
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int StatusCode { get; set; } = StatusCodes.Status200OK;
        public int TotalCount { get; set; }
        public string Message { get; set; }
    }
}
