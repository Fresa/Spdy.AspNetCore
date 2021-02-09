using System.Threading.Tasks;

namespace Spdy.AspNetCore
{
    internal interface ISpdyFeature
    {
        bool IsSpdyRequest { get; }
        Task<SpdySession> AcceptAsync();
    }
}