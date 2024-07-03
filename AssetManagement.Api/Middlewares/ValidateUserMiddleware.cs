using AssetManagement.Application.Exceptions.Common;
using AssetManagement.Application.Exceptions.Token;
using AssetManagement.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace AssetManagement.Api.Middlewares
{
    public class ValidateUserMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidateUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IJwtInvalidationService jwtInvalidationService)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }

            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (authorizationHeader == null)
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Authorization header is missing.");
                    return;
                }
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
                try
                {
                    // There always should be a valid token because other middlewares check for it
                    await jwtInvalidationService.ValidateJwtTokenAsync(jwtToken!);
                }
                catch (NotFoundException ex)
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(ex.Message);
                    return;
                }
                catch (TokenInvalidException ex)
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(ex.Message);
                    return;
                }
                catch (PasswordNotChangedFirstTimeException ex)
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync(ex.Message);
                    return;
                }
            }
            await _next(context);
        }
    }
}
