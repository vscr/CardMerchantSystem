using Card.Domain.Repositories;
using Card.Infrastructure.Persistence;
using Card.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Card.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCardInfrastructure(this IServiceCollection services, string connectionString)
    {
        // DbContext
        services.AddDbContext<CardDbContext>(options =>
            options.UseSqlServer(connectionString, b =>
                b.MigrationsAssembly(typeof(CardDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<ICardApplicationRepository, CardApplicationRepository>();

        return services;
    }
}