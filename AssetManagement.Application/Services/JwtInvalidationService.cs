using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace AssetManagement.Application.Services
{
    public class JwtInvalidationService : IJwtInvalidationService
    {
        private readonly IGlobalSettingsRepository _globalSettingRepository;
        private readonly IUserRepository _userRepository;

        public JwtInvalidationService(IGlobalSettingsRepository globalSettingRepository, IUserRepository userRepository)
        {
            _globalSettingRepository = globalSettingRepository;
            _userRepository = userRepository;
        }

        public async Task UpdateGlobalInvalidationTimeStampAsync()
        {
            var timestamp = DateTime.Now;
            await _globalSettingRepository.UpdateGlobalInvalidationTimestampAsync(timestamp);
        }

        public async Task<DateTime> GetGlobalInvalidationTimestampAsync()
        {
            var globalSetting = await _globalSettingRepository.GetGlobalSettingAsync();
            return globalSetting?.GlobalInvalidationTimestamp ?? DateTime.MinValue;
        }

        public async Task<DateTime> GetUserInvalidationTimestampAsync(Guid userId)
        {
            var user = await _userRepository.GetByCondition(u => u.Id == userId).FirstOrDefaultAsync();
            return user?.TokenInvalidationTimestamp ?? DateTime.MinValue;
        }

        public async Task<bool> IsTokenValidAsync(string token, Guid userId)
        {
            var globalInvalidationTimestamp = await GetGlobalInvalidationTimestampAsync();
            var userInvalidationTimestamp = await GetUserInvalidationTimestampAsync(userId);

            var tokenIssuedAt = GetTokenIssuedAt(token);

            if (tokenIssuedAt == null) return false;

            return tokenIssuedAt > globalInvalidationTimestamp && tokenIssuedAt > userInvalidationTimestamp;
        }

        private DateTime? GetTokenIssuedAt(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                return null;

            var blTimestampClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "BlTimestamp")?.Value;

            if (DateTime.TryParse(blTimestampClaim, out DateTime blTimestamp))
            {
                return blTimestamp;
            }

            return null;
        }
    }
}
