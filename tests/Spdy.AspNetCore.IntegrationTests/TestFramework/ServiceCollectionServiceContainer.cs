using System;
using Microsoft.Extensions.DependencyInjection;
using Test.It;

namespace Spdy.AspNetCore.IntegrationTests.TestFramework
{
    internal sealed class ServiceCollectionServiceContainer : IServiceContainer
    {
        private readonly IServiceCollection _container;

        public ServiceCollectionServiceContainer(
            IServiceCollection container)
        {
            _container = container;
        }

        public void Register<TImplementation>(
            Func<TImplementation> configurer) where TImplementation : class
        {
            _container.AddTransient(_ => configurer());
        }

        public void Register<TService, TImplementation>()
            where TService : class where TImplementation : class, TService
        {
            _container.AddTransient<TService, TImplementation>();
        }

        public void RegisterSingleton<TImplementation>(
            Func<TImplementation> configurer) where TImplementation : class
        {
            _container.AddSingleton(_ => configurer());
        }

        public void RegisterSingleton<TService, TImplementation>()
            where TService : class where TImplementation : class, TService
        {
            _container.AddSingleton<TService, TImplementation>();
        }

        public TService Resolve<TService>() where TService : class
        {
            return (TService)_container.BuildServiceProvider().GetService(typeof(TService));
        }

        public void Dispose()
        {
        }
    }
}