using CardMerchantSystem.Shared.Kernel;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Transaction.Application.Behaviors;
using Transaction.Application.Commands;
using Transaction.Application.DTOs;

namespace Transaction.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddTransactionApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // FluentValidation validators
        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(
         typeof(IPipelineBehavior<ProcessTransactionCommand, Result<TransactionResultDto>>),
         typeof(IdempotencyBehavior));

        return services;
    }
}