using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices;
using Microsoft.EntityFrameworkCore;

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

        public async Task<DateTime> GetUserInvalidationTimestampAsync(Guid userId)
        {
            var user = await _userRepository.GetByCondition(u => u.Id == userId).FirstOrDefaultAsync();
            return user?.TokenInvalidationTimestamp ?? DateTime.MinValue;
        }

        public async Task<bool> IsTokenValidAsync(DateTime tokenIssuedAt, Guid userId)
        {
            var globalInvalidationTimestamp = await GetGlobalInvalidationTimestampAsync();
            var userInvalidationTimestamp = await GetUserInvalidationTimestampAsync(userId);

            return tokenIssuedAt > globalInvalidationTimestamp && tokenIssuedAt > userInvalidationTimestamp;
        }
    }
}
