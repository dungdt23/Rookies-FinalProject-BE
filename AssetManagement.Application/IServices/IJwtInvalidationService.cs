namespace AssetManagement.Application.IServices
{
    public interface IJwtInvalidationService
    {
        Task UpdateGlobalInvalidationTimeStampAsync(DateTime timestamp);
        Task<DateTime> GetGlobalInvalidationTimestampAsync();
        Task<DateTime> GetUserInvalidationTimestampAsync(Guid userId);
        Task<bool> IsTokenValidAsync(DateTime tokenIssuedAt, Guid userId);
    }
}
