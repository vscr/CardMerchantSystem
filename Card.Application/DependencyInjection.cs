using Card.Application.Behaviors;
using Card.Application.Commands;
using Card.Application.DTOs;
using Card.Application.Queries;
using Card.Application.Validators;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Card.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddCardApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // FluentValidation validators
        services.AddValidatorsFromAssembly(assembly);

        // Pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}