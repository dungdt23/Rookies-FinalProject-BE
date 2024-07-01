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

        public async Task UpdateGlobalInvalidationTimeStampAsync(DateTime timestamp)
        {
            await _globalSettingRepository.UpdateGlobalInvalidationTimestampAsync(timestamp);
        }

        public async Task<DateTime> GetGlobalInvalidationTimestampAsync()
        {
            var globalSetting = await _globalSettingRepository.GetGlobalSettingAsync();
            return globalSetting?.GlobalInvalidationTimestamp ?? DateTime.MinValue;
        }

        public async Task<bool> IsTokenValidAsync(JwtSecurityToken jwtToken)
        {
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            Guid userId;
            if (!Guid.TryParse(userIdClaim, out userId))
                return false;

            var user = await _userRepository.GetByCondition(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                return false;

            var blTimestampClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "BlTimestamp")?.Value;
            DateTime tokenIssuedAt;
            if (!DateTime.TryParse(blTimestampClaim, out tokenIssuedAt))
                return false;

            var globalInvalidationTimestamp = await GetGlobalInvalidationTimestampAsync();
            var userInvalidationTimestamp = user.TokenInvalidationTimestamp;
            if (tokenIssuedAt <= globalInvalidationTimestamp || tokenIssuedAt <= userInvalidationTimestamp)
                return false;

            return true;
        }
    }
}
