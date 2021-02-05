using Microsoft.AspNetCore.Builder;

namespace Spdy.AspNetCore.IntegrationTests
{
    public class Startup
    {
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