using System.IO;

namespace Spdy.AspNetCore.IntegrationTests
{
    internal interface IHttpDuplexStreamFeature
    {
        public Stream Body { get; }
    }
}