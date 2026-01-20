using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Courier.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCourierApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}