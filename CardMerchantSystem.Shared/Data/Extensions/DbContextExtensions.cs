// CardMerchantSystem.Shared/Data/Extensions/DbContextExtensions.cs

using Microsoft.EntityFrameworkCore;

namespace CardMerchantSystem.Shared.Data.Extensions;

/// <summary>
/// DbContext extension metodları
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Database provider'a göre DbContext'i yapılandırır
    /// </summary>
    public static DbContextOptionsBuilder ConfigureDatabase(
        this DbContextOptionsBuilder optionsBuilder,
        DatabaseProvider provider,
        string connectionString)
    {
        switch (provider)
        {
            case DatabaseProvider.SqlServer:
                optionsBuilder.UseSqlServer(
                    connectionString,
                    options => options
                        .EnableRetryOnFailure(maxRetryCount: 3)
                        .CommandTimeout(30));
                break;

            case DatabaseProvider.PostgreSql:
                optionsBuilder.UseNpgsql(
                    connectionString,
                    options => options
                        .CommandTimeout(30));
                break;

            default:
                throw new InvalidOperationException($"Unsupported database provider: {provider}");
        }

        return optionsBuilder;
    }
}