using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Campaign.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCampaignApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // FluentValidation validators
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}