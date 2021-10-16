using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MongoFunWojtek
{
    public class MongoInitService : IHostedService
    {
        private readonly ILogger<MongoInitService> _logger;
        private readonly IMongoClient _client;

        public MongoInitService(ILogger<MongoInitService> logger, IMongoClient client )
        {
            _logger = logger;
            _client = client;
        }

        public async Task StartAsync(CancellationToken cancellation)
        {
            var databases = await (await _client.ListDatabaseNamesAsync(cancellation)).ToListAsync(cancellation);
            _logger.LogInformation("{0}", string.Join(",", databases));
        }

        public Task StopAsync(CancellationToken cancellation)
        {
            return Task.CompletedTask;
        }

    }
}
