
using Microsoft.Extensions.DependencyInjection;

namespace Courier.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCourierInfrastructure(this IServiceCollection services, string connectionString)
        {

            return services;
        }
    }
}
