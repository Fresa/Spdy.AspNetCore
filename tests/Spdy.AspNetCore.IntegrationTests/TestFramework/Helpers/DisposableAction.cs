using System;

namespace Spdy.AspNetCore.IntegrationTests.TestFramework.Helpers
{
    internal class DisposableAction : IDisposable
    {
        private readonly Action _dispose;

        public DisposableAction(Action dispose)
        {
            _dispose = dispose;
        }

        public void Dispose()
        {
            _dispose();
        }
    }
}