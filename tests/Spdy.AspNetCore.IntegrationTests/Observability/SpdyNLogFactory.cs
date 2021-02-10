using Log.It;
using Spdy.Logging;

namespace Spdy.AspNetCore.IntegrationTests.Observability
{
    internal sealed class SpdyNLogFactory : Logging.ILogFactory
    {
        private readonly ILogicalThreadContext _logContext;

        public SpdyNLogFactory(ILogicalThreadContext logContext)
        {
            _logContext = logContext;
        }

        public Logging.ILogger Create(string logger)
        {
            return new SpdyNLogLogger(logger, _logContext);
        }

        public Logging.ILogger Create<T>()
        {
            return new SpdyNLogLogger(typeof(T).GetPrettyName(), _logContext);
        }
    }
}