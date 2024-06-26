using AssetManagement.Domain.Entities;

namespace AssetManagement.Application.IRepositories
{
    public interface IGlobalSettingsRepository
    {
        Task<GlobalSetting?> GetGlobalSettingAsync();
        Task UpdateGlobalInvalidationTimestampAsync(DateTime timestamp);
    }
}
