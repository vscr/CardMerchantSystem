using Dispute.Domain.Repositories;
using Dispute.Infrastructure.Configurations;
using Dispute.Infrastructure.Persistence;
using Dispute.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dispute.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDisputeInfrastructure(this IServiceCollection services, string connectionString)
    {
        // DbContext
        services.AddDbContext<DisputeDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(DisputeDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IDisputeRepository, DisputeRepository>();

        return services;
    }
}