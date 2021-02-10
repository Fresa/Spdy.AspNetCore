using System.IO;

namespace Spdy.AspNetCore.IntegrationTests.TestFramework.Upgrading
{
    internal interface IHttpDuplexStreamFeature
    {
        public Stream Body { get; }
    }
}