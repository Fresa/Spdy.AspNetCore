using Log.It;
using Log.It.With.NLog;
using Spdy.AspNetCore.IntegrationTests.TestFramework.Helpers;
using LogFactory = Log.It.LogFactory;

namespace Spdy.AspNetCore.IntegrationTests.Observability
{
    internal static class LogFactoryExtensions
    {
        private static readonly ExclusiveLock Lock = new();

        public static void InitializeOnce()
        {
            if (!Lock.TryAcquire())
            {
                return;
            }

            if (LogFactory.HasFactory)
            {
                return;
            }

            LogFactory.Initialize(new NLogFactory(new LogicalThreadContext()));
            Logging.LogFactory.TryInitializeOnce(
                new SpdyNLogFactory(new SpdyNLogLogContext()));
        }
    }
}