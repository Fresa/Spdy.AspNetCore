using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Spdy.AspNetCore.IntegrationTests.TestFramework.Upgrading
{
    internal sealed class CrossWiredStream : Stream
    {
        private readonly Pipe _read;
        private readonly Pipe _write;

        public CrossWiredStream()
            : this(
                new Pipe(new PipeOptions(useSynchronizationContext: false)), 
                new Pipe(new PipeOptions(useSynchronizationContext: false)))
        {
        }

        private CrossWiredStream(
            Pipe read,
            Pipe write)
        {
            _read = read;
            _write = write;
        }

        internal Stream CreateReverseStream()
            => new CrossWiredStream(_write, _read);


        public override bool CanTimeout => false;

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override Task FlushAsync(
            CancellationToken cancellationToken)
            => _write.Writer.FlushAsync(cancellationToken)
                     .AsTask();

        public override int Read(
            byte[] buffer,
            int offset,
            int count)
            => throw new NotImplementedException();

        public override void Close()
        {
            _read.Reader.Complete();
            _write.Writer.Complete();
        }

        public override async ValueTask WriteAsync(
            ReadOnlyMemory<byte> buffer,
            CancellationToken cancellationToken = new())
        {
            await _write.Writer.WriteAsync(buffer, cancellationToken)
                        .ConfigureAwait(false);
        }

        public override async ValueTask<int> ReadAsync(
            Memory<byte> buffer,
            CancellationToken cancellationToken = new())
        {
            var result = await _read.Reader.ReadAsync(cancellationToken)
                                    .ConfigureAwait(false);

            if (result.Buffer.Length == 0)
            {
                _read.Reader.AdvanceTo(result.Buffer.End);
                return 0;
            }

            var length = result.Buffer.Length > buffer.Length
                ? buffer.Length
                : (int) result.Buffer.Length;

            var data = result.Buffer.Slice(0, length);
            data.CopyTo(buffer.Span);

            _read.Reader.AdvanceTo(data.End, result.Buffer.End);
            return length;
        }

        public override long Seek(
            long offset,
            SeekOrigin origin)
            => throw new NotImplementedException();

        public override void SetLength(
            long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(
            byte[] buffer,
            int offset,
            int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
    }
}