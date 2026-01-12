using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Fee.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddFeeApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Validators
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}