using Microsoft.AspNetCore.Builder;

namespace Spdy.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSpdy(
            this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<SpdyMiddleware>();
        }
    }
}