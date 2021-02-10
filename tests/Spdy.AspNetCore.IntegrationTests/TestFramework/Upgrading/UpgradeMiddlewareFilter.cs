using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Spdy.AspNetCore.IntegrationTests.TestFramework.Upgrading
{
    internal sealed class UpgradeMiddlewareFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<UpgradeTestMiddleware>();
                next(builder);
            };
        }
    }
}