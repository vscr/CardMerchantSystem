using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WorkOrder.Domain.Repositories;
using WorkOrder.Infrastructure.Persistence;
using WorkOrder.Infrastructure.Repositories;

namespace WorkOrder.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkOrderInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<WorkOrderDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(WorkOrderDbContext).Assembly.FullName)));

        services.AddScoped<IWorkOrderTypeRepository, WorkOrderTypeRepository>();
        services.AddScoped<IWorkOrderItemRepository, WorkOrderItemRepository>();

        return services;
    }
}