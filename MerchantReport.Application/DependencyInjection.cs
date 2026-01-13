using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MerchantReport.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddMerchantReportApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Validators
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}