using System;
using System.Buffers;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Spdy.AspNetCore.IntegrationTests.Subscriptions;
using Spdy.AspNetCore.IntegrationTests.TestFramework;
using Spdy.AspNetCore.IntegrationTests.TestFramework.Helpers;
using Test.It;
using Xunit;
using Xunit.Abstractions;
using ReadResult = System.IO.Pipelines.ReadResult;

namespace Spdy.AspNetCore.IntegrationTests
{
    public class Given_a_spdy_enabled_api_endpoint
    {
        public class
            When_requesting_to_upgrade_to_spdy : XUnit2ServiceSpecificationAsync<
                TestHost>
        {
            private HttpResponseMessage _response = default!;
            private ReadResult _serverReceivedRequest;
            private string _clientReceivedResponse = "";

            public When_requesting_to_upgrade_to_spdy(
                ITestOutputHelper testOutputHelper)
                : base(testOutputHelper)
            {
            }

            protected override TimeSpan Timeout { get; set; } =
                TimeSpan.FromSeconds(5);

            protected override void Given(
                IServiceContainer container)
            {
                var spdyRequestSubscription =
                    new SpdyRequestSubscription();
                container.RegisterSingleton(
                    () => spdyRequestSubscription);

                spdyRequestSubscription.OnConnected(
                    async (
                        session,
                        cancellationToken) =>
                    {
                        using var stream = await session
                                                 .ReceiveStreamAsync(
                                                     cancellationToken)
                                                 .ConfigureAwait(false);

                        _serverReceivedRequest = await stream
                                               .ReceiveAsync(
                                                   cancellationToken: cancellationToken)
                                               .ConfigureAwait(false);

                        await stream.SendLastAsync(
                                        new ReadOnlyMemory<byte>(
                                            Encoding.UTF8.GetBytes(
                                                "This is a response")), cancellationToken: cancellationToken)
                                    .ConfigureAwait(false);
                    });
            }

            protected override async Task WhenAsync(
                CancellationToken cancellationToken)
            {
                _response = await
                    Server
                        .CreateHttpClient()
                        .SendAsync(
                            new HttpRequestMessage(
                                HttpMethod.Get,
                                new Uri($"{Server.BaseAddress}api/v1/streaming"))
                            {
                                Headers =
                                {
                                    { Microsoft.Net.Http.Headers.HeaderNames.Connection, SpdyMiddleware.Headers.Connection },
                                    { Microsoft.Net.Http.Headers.HeaderNames.Upgrade, SpdyMiddleware.Headers.Upgrade }
                                }
                            },
                            // This prevents the http client to buffer the response
                            HttpCompletionOption.ResponseHeadersRead,
                            cancellationToken)
                        .ConfigureAwait(false);

                var stream = await _response.Content.ReadAsStreamAsync(cancellationToken)
                                           .ConfigureAwait(false);

                await using var client = SpdySession.CreateClient(new StreamingNetworkClient(stream));
                using var spdyStream = client.CreateStream();
                
                await spdyStream.SendLastAsync(
                    Encoding.UTF8.GetBytes("This is a request"), cancellationToken: cancellationToken);
                
                var readResult = await spdyStream
                                   .ReceiveAsync(
                                       cancellationToken: cancellationToken)
                                   .ConfigureAwait(false);
                _clientReceivedResponse = Encoding.UTF8.GetString(readResult.Buffer.ToArray());

                await Task.WhenAll(
                              spdyStream.Local.WaitForClosedAsync(
                                  cancellationToken),
                              spdyStream.Remote.WaitForClosedAsync(
                                  cancellationToken))
                          .ConfigureAwait(false);
            }

            [Fact]
            public void It_should_receive_the_message_sent()
            {
                Encoding.ASCII.GetString(
                            _serverReceivedRequest.Buffer.ToArray())
                        .Should()
                        .Be("This is a request");
            }

            [Fact]
            public void
                It_should_switch_protocol_to_spdy()
            {
                _response.StatusCode.Should()
                         .Be(HttpStatusCode.SwitchingProtocols);
                _response.Headers.Connection.Should()
                         .Contain("Upgrade");
                _response.Headers.Upgrade.Should()
                         .Contain(new ProductHeaderValue("SPDY", "3.1"));
            }

            [Fact]
            public void It_should_receive_a_response()
            {
                _clientReceivedResponse.Should()
                        .Be("This is a response");
            }
        }
    }
}