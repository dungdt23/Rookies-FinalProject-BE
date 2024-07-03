using AssetManagement.Application.Exceptions.Common;
using AssetManagement.Application.Exceptions.Token;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices;
using AssetManagement.Domain.Constants;
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

        public async Task ValidateJwtTokenAsync(JwtSecurityToken jwtToken)
        {
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimNameConstants.UserId)?.Value;

            // Check if user exists
            Guid userId = Guid.Parse(userIdClaim!);

            var user = await _userRepository.GetByCondition(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new NotFoundException("Wrong token format");


            // Check the timestamp on token
            var blTimestampClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimNameConstants.BlackListTimeStamp)?.Value;
            if (blTimestampClaim == null)
                throw new NotFoundException("Wrong token format");

            DateTime tokenIssuedAt = DateTime.Parse(blTimestampClaim);

            var globalInvalidationTimestamp = await GetGlobalInvalidationTimestampAsync();
            var userInvalidationTimestamp = user.TokenInvalidationTimestamp;
            if (tokenIssuedAt <= globalInvalidationTimestamp || tokenIssuedAt <= userInvalidationTimestamp)
                throw new TokenInvalidException("The token is invalid due to a global or user-specific invalidation timestamp.");


            // Check if user has changed their password for the first time
            var isPasswordChangedFirstTime = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimNameConstants.IsPasswordChangedFirstTime)?.Value;
            if (isPasswordChangedFirstTime == null)
                throw new NotFoundException("Wrong token format");

            if (isPasswordChangedFirstTime == "0")
                throw new PasswordNotChangedFirstTimeException("You need to change your password first before having access to the api.");
        }
    }
}
