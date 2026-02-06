using CardMerchantSystem.Shared.Audit.Interceptors;
using CardMerchantSystem.Shared.Audit.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CardMerchantSystem.Shared.Audit;

/// <summary>
/// Audit Trail için DI registration extension'ları
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Audit Trail servislerini DI container'a ekler.
    /// Program.cs'de çağrılmalıdır.
    /// </summary>
    /// <example>
    /// builder.Services.AddAuditTrail();
    /// </example>
    public static IServiceCollection AddAuditTrail(this IServiceCollection services)
    {
        // HttpContextAccessor zaten varsa tekrar ekleme
        services.AddHttpContextAccessor();

        // Audit context accessor
        services.AddScoped<IAuditContextAccessor, AuditContextAccessor>();

        // Audit log writer (Dapper ile doğrudan SQL)
        services.AddScoped<IAuditLogWriter, AuditLogWriter>();

        // Audit service (sorgulama için)
        services.AddScoped<IAuditService, AuditService>();

        // SaveChanges interceptor
        services.AddScoped<AuditSaveChangesInterceptor>();

        return services;
    }
}