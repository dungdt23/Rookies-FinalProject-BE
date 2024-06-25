using AssetManagement.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AssetManagement.Api.Middlewares
{
	public class ValidateUserMiddleware
	{
		private readonly RequestDelegate _next;

		public ValidateUserMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
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

				if (userIdClaim != null)
				{
					using (var scope = serviceProvider.CreateScope())
					{
						var dbContext = scope.ServiceProvider.GetRequiredService<AssetManagementDBContext>();
						var user = await dbContext.Users.FindAsync(userIdGuid);
						if (user == null || user.IsDeleted)
						{
							context.Response.StatusCode = StatusCodes.Status401Unauthorized;
							return;
						}
					}
				}


			}
			await _next(context);
		}
	}
}
