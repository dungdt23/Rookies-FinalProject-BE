using Microsoft.AspNetCore.Http;
namespace AssetManagement.Application.ApiResponses
{
    public class ApiResponse
    {
        public object Data { get; set; } = new { };

        public int StatusCode { get; set; } = StatusCodes.Status200OK;

        public string Message { get; set; } = string.Empty;
    }
}
