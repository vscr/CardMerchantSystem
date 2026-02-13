using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace Fraud.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddFraudApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        return services;
    }
}