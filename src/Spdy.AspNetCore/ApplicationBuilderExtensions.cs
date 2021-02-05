using Microsoft.AspNetCore.Builder;

namespace Spdy.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSpdy(
            this IApplicationBuilder applicationBuilder)
        {
            var spdyMiddleware = new SpdyMiddleware();
            return applicationBuilder.Use(
                @delegate => context
                    => spdyMiddleware.InvokeAsync(context, @delegate));
        }
    }
}