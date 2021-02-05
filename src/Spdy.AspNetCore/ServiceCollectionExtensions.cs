using Microsoft.Extensions.DependencyInjection;

namespace Spdy.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSpdy(
            this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddTransient<SpdyMiddleware>();
        }
    }
}