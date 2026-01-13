using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Accounting.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountingApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Validators
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}