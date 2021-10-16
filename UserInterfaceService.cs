using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MongoFunWojtek
{
    class UserInterfaceService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        public UserInterfaceService(IHostApplicationLifetime hostApplicationLifetime)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(500, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Yield();

                Console.Write("Command: ");
                var command = Console.ReadLine();
                switch (command)
                {
                    case "hello":
                        Console.WriteLine("Hello Mongo command ;o");
                        break;
                    case "exit":
                        _hostApplicationLifetime.StopApplication();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
