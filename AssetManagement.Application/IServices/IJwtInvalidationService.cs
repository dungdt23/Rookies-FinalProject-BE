namespace AssetManagement.Application.IServices
{
    public interface IJwtInvalidationService
    {
        Task GlobalInvalidationAsync();
        Task UserInvalidationAsync(Guid userId);
        Task<DateTime> GetGlobalInvalidationTimestampAsync();
        Task<DateTime> GetUserInvalidationTimestampAsync(Guid userId);
        Task<bool> IsTokenValidAsync(string token, Guid userId);
    }
}
