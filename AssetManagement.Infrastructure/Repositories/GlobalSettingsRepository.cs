using AssetManagement.Application.IRepositories;
using AssetManagement.Domain.Entities;
using AssetManagement.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Repositories
{
    public class GlobalSettinsgRepository : IGlobalSettingsRepository
    {
        private readonly AssetManagementDBContext _context;

        public GlobalSettinsgRepository(AssetManagementDBContext context)
        {
            _context = context;
        }

        public async Task<GlobalSetting?> GetGlobalSettingAsync()
        {
            return await _context.GlobalSettings.FirstOrDefaultAsync();
        }

        public async Task UpdateGlobalInvalidationTimestampAsync(DateTime timestamp)
        {
            var globalSetting = await _context.GlobalSettings.FirstOrDefaultAsync();
            if (globalSetting != null)
            {
                globalSetting.GlobalInvalidationTimestamp = timestamp;
                _context.GlobalSettings.Update(globalSetting);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newSetting = new GlobalSetting
                {
                    Id = 1,  // Assuming you use a fixed ID for global settings
                    GlobalInvalidationTimestamp = timestamp
                };
                _context.GlobalSettings.Add(newSetting);
                await _context.SaveChangesAsync();
            }
        }
    }
}
