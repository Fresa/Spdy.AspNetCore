using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Spdy.AspNetCore.IntegrationTests
{
    public class Startup
    {
        public Startup(
            IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(
            IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc();
            services.AddSpdy();
        }

        public void Configure(
            IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseMiddleware<UpgradeTestMiddleware>();
            app.UseSpdy();
            
            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                });

        }
    }
}