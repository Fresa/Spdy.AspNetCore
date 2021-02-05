using Microsoft.Extensions.Configuration;
using NLog.Extensions.Logging;
using NLog.Web;
using Spdy.AspNetCore.IntegrationTests.TestFramework.Helpers;

namespace Spdy.AspNetCore.IntegrationTests.Observability
{
    internal static class NLogBuilderExtensions
    {
        private static readonly ExclusiveLock NlogConfigurationLock =
            new();

        internal static void ConfigureNLogOnce(
            IConfiguration configuration)
        {
            if (!NlogConfigurationLock.TryAcquire())
            {
                return;
            }

            var nLogConfig = new NLogLoggingConfiguration(
                configuration.GetSection("NLog"));
            NLogBuilder.ConfigureNLog(nLogConfig);
        }
    }
}