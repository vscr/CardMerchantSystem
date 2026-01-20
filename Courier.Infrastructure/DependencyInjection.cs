using Courier.Domain.Repositories;
using Courier.Infrastructure.Persistence;
using Courier.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Courier.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCourierInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<CourierDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(CourierDbContext).Assembly.FullName)));

        services.AddScoped<ICourierCompanyRepository, CourierCompanyRepository>();
        services.AddScoped<IShipmentRepository, ShipmentRepository>();

        return services;
    }
}