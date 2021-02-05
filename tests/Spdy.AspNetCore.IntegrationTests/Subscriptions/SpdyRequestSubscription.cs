using System;
using System.Threading;
using System.Threading.Tasks;

namespace Spdy.AspNetCore.IntegrationTests.Subscriptions
{
    public sealed class SpdyRequestSubscription
    {
        private OnConnectedSpdySessionAsync?
            _onConnectedSpdySessionSubscription;

        public delegate Task OnConnectedSpdySessionAsync(
            SpdySession portForwardSpdySession,
            CancellationToken cancellationToken = default);

        internal async Task WaitAsync(
            SpdySession spdySession,
            CancellationToken cancellationToken = default)
        {
            if (_onConnectedSpdySessionSubscription == default)
            {
                throw new InvalidOperationException(
                    "Missing subscription for messages sent over spdy");
            }

            await _onConnectedSpdySessionSubscription.Invoke(spdySession, cancellationToken)
                              .ConfigureAwait(false);
        }

        public void OnConnected(OnConnectedSpdySessionAsync subscribe)
        {
            if (_onConnectedSpdySessionSubscription != null)
            {
                throw new ArgumentException(
                    "Spdy subscription already exists");
            }

            _onConnectedSpdySessionSubscription = subscribe;
        }
    }
}