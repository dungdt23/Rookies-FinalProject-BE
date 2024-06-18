
using AssetManagement.Application.ApiResponses;
using Newtonsoft.Json;

namespace AssetManagement.Api.Middlewares;

public class UnauthorizedResponseMiddleware
{
    private readonly RequestDelegate _next;

    public UnauthorizedResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {

        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized ||
            context.Response.StatusCode == StatusCodes.Status403Forbidden)
        {
            var apiResponse = new ApiResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = context.Response.StatusCode == StatusCodes.Status401Unauthorized ? "Unauthorized" : "Forbidden"
            };

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(apiResponse));
        }
        await _next(context);
    }
}
