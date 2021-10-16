using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace MongoFunWojtek
{
    static class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello Monogo!");

            var hostBuilder = new HostBuilder();
            hostBuilder.ConfigureHostConfiguration(builder =>
            {
                builder.AddEnvironmentVariables("DOTNET_");
                builder.AddCommandLine(args);
            });

            hostBuilder.ConfigureAppConfiguration(builder => { builder.AddJsonFile("appsettings.json"); });

            hostBuilder.ConfigureServices(Installer.Install);

            await hostBuilder.Build().RunAsync();
        }
    }
}
