using AssetManagement.Application.Exceptions.Token;
using AssetManagement.Application.Exceptions.User;
using AssetManagement.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

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
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    var response = new { message = "Authorization header is missing." };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
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
                catch (WrongTokenFormatException ex)
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    var response = new { message = ex.Message };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }
                catch (UserNotExistException ex)
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    var response = new { message = ex.Message };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }
                catch (TokenInvalidException ex)
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    var response = new { message = ex.Message };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }
                catch (PasswordNotChangedFirstTimeException ex)
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    var response = new { message = ex.Message };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    return;
                }
            }
            await _next(context);
        }
    }
}
