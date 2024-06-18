
using System.IdentityModel.Tokens.Jwt;

namespace AssetManagement.Api.Controllers;

public static class ControllerExtension
{
    public static string GetClaim(this HttpContext context, string claimType)
    {
        var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if(authorizationHeader == null){
            return string.Empty;
        }
        var token = authorizationHeader.Substring("Bearer ".Length).Trim();
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
        var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        return claim;
    }
}
