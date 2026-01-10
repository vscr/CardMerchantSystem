using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HSM.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddHSMApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        return services;
    }
}