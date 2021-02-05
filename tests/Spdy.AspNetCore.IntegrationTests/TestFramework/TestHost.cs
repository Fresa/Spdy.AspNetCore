using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using Spdy.AspNetCore.IntegrationTests.TestFramework.Upgrading;
using Test.It.Specifications;
using Test.It.While.Hosting.Your.Web.Application;

namespace Spdy.AspNetCore.IntegrationTests.TestFramework
{
    public sealed class TestHost : IWebApplicationHost, IServer
    {
        private IHost? _host;

        private TestServer GetTestServer()
        {
            var testServer = _host.GetTestServer();
            testServer.PreserveExecutionContext = true;
            return testServer;
        }
        
        public IWebSocketClient CreateWebSocketClient() => throw new NotImplementedException();

        public Uri BaseAddress => GetTestServer()
            .BaseAddress;

        public async Task<IServer> StartAsync(
            ITestConfigurer testConfigurer,
            CancellationToken cancellationToken = new())
        {
            if (_host == null)
            {
                IConfiguration configuration = default!;
                var hostBuilder =
                    Program.CreateHostBuilder(new string[0])
                           .ConfigureAppConfiguration(
                               (
                                   context,
                                   _) =>
                               {
                                   configuration =
                                       context.Configuration;
                               })
                           .ConfigureServices(
                               collection =>
                               {
                                   collection.AddTransient<IStartupFilter,
                                       UpgradeMiddlewareFilter>();
                                   collection.AddLogging(
                                       builder =>
                                       {
                                           builder.ClearProviders();
                                           builder.AddNLog(configuration);
                                       });
                                   collection.AddControllers(
                                       options =>
                                           options.Filters.Add(
                                               new
                                                   HttpResponseExceptionFilter()));
                                   testConfigurer.Configure(
                                       new ServiceCollectionServiceContainer(
                                           collection));
                               })
                           .ConfigureWebHost(builder => builder.UseTestServer())
                           .UseNLog();
                _host = hostBuilder
                    .Build();
                await _host.StartAsync(cancellationToken)
                           .ConfigureAwait(false);
            }

            return this;
        }
        public HttpMessageHandler CreateHttpMessageHandler()
            => new UpgradeMessageHandler(GetTestServer());
        
        public void Dispose()
        {
            _host?.Dispose();
        }

        public async Task StopAsync(
            CancellationToken cancellationToken = new())
        {
            if (_host != null)
            {
                await _host.StopAsync(cancellationToken)
                           .ConfigureAwait(false);
            }
        }
    }

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