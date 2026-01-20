using BulkCardPrint.Domain.Repositories;
using BulkCardPrint.Infrastructure.Persistence;
using BulkCardPrint.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BulkCardPrint.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBulkCardPrintInfrastructure(this IServiceCollection services, string connectionString)
    {
        // DbContext
        services.AddDbContext<BulkCardPrintDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(BulkCardPrintDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IPrintVendorRepository, PrintVendorRepository>();
        services.AddScoped<IPrintBatchRepository, PrintBatchRepository>();

        return services;
    }
}