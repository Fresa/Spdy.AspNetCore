using System.Threading;
using System.Threading.Tasks;

namespace Spdy.AspNetCore.IntegrationTests.TestFramework.Helpers
{
    internal static class SpdyStreamExtensions
    {
        internal static Task WaitForFullyClosedAsync(
            this SpdyStream stream,
            CancellationToken cancellationToken) =>
            Task.WhenAll(
                stream.Local.WaitForClosedAsync(cancellationToken),
                stream.Remote.WaitForClosedAsync(cancellationToken));
    }
}