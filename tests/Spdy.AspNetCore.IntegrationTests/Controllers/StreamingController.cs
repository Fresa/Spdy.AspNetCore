using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Spdy.AspNetCore.IntegrationTests.Subscriptions;

namespace Spdy.AspNetCore.IntegrationTests.Controllers
{
    [ApiController]
    [Route("api/v1/[Controller]")]
    public class StreamingController : ControllerBase
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly SpdyRequestSubscription _subscriptions;

        public StreamingController(
            IHostApplicationLifetime hostApplicationLifetime,
            SpdyRequestSubscription subscriptions)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _subscriptions = subscriptions;
        }

        [HttpGet]
        public async Task<ActionResult> PortForward(
            CancellationToken cancellationToken)
        {
            if (HttpContext.Spdy()
                           .IsSpdyRequest)
            {
                var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken,
                    _hostApplicationLifetime.ApplicationStopping);

                await using var spdySession = await HttpContext.Spdy()
                                                               .AcceptSpdyAsync()
                                                               .ConfigureAwait(false);

                await _subscriptions.WaitAsync(spdySession, cancellationTokenSource.Token)
                                    .ConfigureAwait(false);

                return Ok();
            }

            return BadRequest();
        }
    }
}