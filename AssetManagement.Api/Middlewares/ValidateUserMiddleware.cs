using AssetManagement.Application.IServices;
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
            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (authorizationHeader == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

                Guid userIdGuid;
                if (!Guid.TryParse(userIdClaim, out userIdGuid))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var blTimestampClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "BlTimestamp")?.Value;
                DateTime blTimestamp;

                if (!DateTime.TryParse(blTimestampClaim, out blTimestamp))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                if (!await jwtInvalidationService.IsTokenValidAsync(blTimestamp, userIdGuid))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }
            await _next(context);
        }
    }
}
