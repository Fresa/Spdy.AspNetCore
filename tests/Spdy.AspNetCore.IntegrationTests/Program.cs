using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Spdy.AspNetCore.IntegrationTests
{
    public class Program
    {
        public static Task Main(
            string[] args)
            => CreateHostBuilder(args)
               .Build()
               .RunAsync();

        public static IHostBuilder CreateHostBuilder(
            string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(
                    webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}