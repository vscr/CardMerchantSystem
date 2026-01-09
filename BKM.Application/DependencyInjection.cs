using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BKM.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBKMApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        return services;
    }
}