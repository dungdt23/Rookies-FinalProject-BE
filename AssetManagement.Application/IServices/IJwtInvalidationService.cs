namespace AssetManagement.Application.IServices
{
    public interface IJwtInvalidationService
    {
        Task UpdateGlobalInvalidationTimeStampAsync();
        Task<DateTime> GetGlobalInvalidationTimestampAsync();
        Task<DateTime> GetUserInvalidationTimestampAsync(Guid userId);
        Task<bool> IsTokenValidAsync(string token, Guid userId);
    }
}
