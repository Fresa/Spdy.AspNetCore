using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Spdy.AspNetCore.IntegrationTests
{
    public class Startup
    {
        public void ConfigureServices(
            IServiceCollection services)
        {
            services.AddSpdy();
        }

        public void Configure(
            IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseSpdy();
            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}