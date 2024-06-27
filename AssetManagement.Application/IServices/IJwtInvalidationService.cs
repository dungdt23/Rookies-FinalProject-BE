﻿using System.IdentityModel.Tokens.Jwt;

namespace AssetManagement.Application.IServices
{
    public interface IJwtInvalidationService
    {
        Task UpdateGlobalInvalidationTimeStampAsync(DateTime timestamp);
        Task<DateTime> GetGlobalInvalidationTimestampAsync();
        Task<bool> IsTokenValidAsync(JwtSecurityToken jwtToken);
    }
}