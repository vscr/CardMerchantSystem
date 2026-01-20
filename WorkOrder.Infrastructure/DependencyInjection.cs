using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WorkOrder.Infrastructure.Persistence;

namespace WorkOrder.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkOrderInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<WorkOrderDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(WorkOrderDbContext).Assembly.FullName)));

        return services;
    }
}