
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CardMerchantSystem.Shared.Data;

public static class MigrationHelper
{
    /// <summary>
    /// Tüm DbContext'leri migrate eder
    /// </summary>
    public static async Task MigrateAllDatabasesAsync(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<DatabaseProvider>>();

        try
        {
            logger.LogInformation("Starting database migrations...");

            // Tüm DbContext tiplerini al
            var dbContextTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(DbContext)));

            foreach (var contextType in dbContextTypes)
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetService(contextType) as DbContext;

                if (context != null)
                {
                    logger.LogInformation("Migrating {ContextName}...", contextType.Name);
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Successfully migrated {ContextName}", contextType.Name);
                }
            }

            logger.LogInformation("All database migrations completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during database migration");
            throw;
        }
    }
}