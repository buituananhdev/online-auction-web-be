using Microsoft.Extensions.DependencyInjection;

namespace OnlineAuctionWeb.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddSignalRCore();

            return services;
        }
    }
}
