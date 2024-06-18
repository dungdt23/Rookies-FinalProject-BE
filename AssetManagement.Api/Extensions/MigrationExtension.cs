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

        //public static async Task<IHost> SeedDatabase(this IHost host)
        //{
        //    using (var scope = host.Services.CreateScope())
        //    {
        //        var services = scope.ServiceProvider;
        //        var logger = services.GetRequiredService<ILogger<Program>>();

        //        var context = services.GetRequiredService<AssetManagementDBContext>();
        //        // Begin transaction
        //        using (var transaction = context.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                await context.Seed(services);

        //                // Commit transaction if seeding is successful
        //                transaction.Commit();
        //            }
        //            catch (Exception seedingEx)
        //            {
        //                // Rollback transaction if an error occurs during seeding
        //                transaction.Rollback();
        //                logger.LogError(seedingEx, "An error occurred while seeding the database. Rolling back seeding operation.");
        //            }
        //        }
        //    }
        //    return host;
        //}
    }
}
