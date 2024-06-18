using AssetManagement.Application.ApiResponses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AssetManagement.Api.Authorizations;

public class CustomAuthorization : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _claimRole;
    public CustomAuthorization(string claimRole)
    {
        _claimRole = claimRole;
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new JsonResult(new ApiResponse
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "Unauthorized"
            });
            context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var typeClaim = user.Claims.FirstOrDefault(u => u.Type == "type")?.Value;
        if (typeClaim != _claimRole)
        {
            context.Result = new JsonResult(new ApiResponse
            {
                StatusCode = StatusCodes.Status403Forbidden,
                Message = "Forbidden"
            });
            context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

    }

}
