using AssetManagement.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Api.Extensions
{
    public static class MigrationExtension
    {
        public static IHost MigrationDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AssetManagementDBContext>();
                dbContext.Database.Migrate();
            }

            return host;
        }
    }
}
