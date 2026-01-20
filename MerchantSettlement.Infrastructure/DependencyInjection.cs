using MerchantSettlement.Domain.Repositories;
using MerchantSettlement.Infrastructure.Persistence;
using MerchantSettlement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MerchantSettlement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMerchantSettlementInfrastructure(this IServiceCollection services, string connectionString)
        {
            // DbContext
            services.AddDbContext<MerchantSettlementDbContext>(options =>
                options.UseSqlServer(connectionString, b =>
                    b.MigrationsAssembly(typeof(MerchantSettlementDbContext).Assembly.FullName)));

            // Repositories
            services.AddScoped<IMerchantSettlementReconciliationRepository, MerchantSettlementReconciliationRepository>();
            services.AddScoped<IMerchantSettlementBatchRepository, MerchantSettlementBatchRepository>();
            services.AddScoped<IMerchantPayoutRepository, MerchantPayoutRepository>();
            services.AddScoped<IMerchantDailySettlementSummaryRepository, MerchantDailySettlementSummaryRepository>();

            return services;
        }
    }
}
