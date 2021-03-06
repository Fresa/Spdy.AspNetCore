using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Spdy.Network;

namespace Spdy.AspNetCore
{
    internal sealed class StreamingNetworkClient : INetworkClient
    {
        private readonly Stream _stream;

        public StreamingNetworkClient(Stream stream)
        {
            _stream = stream;
        }

        public ValueTask DisposeAsync() => new();

        public ValueTask<int> ReceiveAsync(
            Memory<byte> buffer,
            CancellationToken cancellationToken = default)
            => _stream.ReadAsync(buffer, cancellationToken);

        public async ValueTask<int> SendAsync(
            ReadOnlyMemory<byte> buffer,
            CancellationToken cancellationToken = default)
        {
            await _stream.WriteAsync(buffer, cancellationToken)
                         .ConfigureAwait(false);
            await _stream.FlushAsync(cancellationToken)
                         .ConfigureAwait(false);
            return buffer.Length;
        }
    }
}