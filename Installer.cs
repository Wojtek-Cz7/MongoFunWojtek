using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoFunWojtek
{
    public static class Installer
    {
        public static void Install(HostBuilderContext context, IServiceCollection collection)
        {
            collection.AddLogging(builder =>
            {
                builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                builder.AddConsole();
            });

            collection.AddSingleton<MongoUrl>(new MongoUrl(context.Configuration.GetSection("Mongo").Get<string>()));
            collection.AddSingleton(x =>
            {
                var logger = x.GetService<ILogger<MongoClientSettings>>();
                var mongoUrl = x.GetService<MongoUrl>();
                var settings = MongoClientSettings.FromUrl(mongoUrl);
                settings.ClusterConfigurator = builder =>
                {
                    builder.Subscribe<CommandStartedEvent>(e =>
                    {
                        logger.LogDebug("Executing command {CommandName} \n {Command}", e.CommandName, e.Command.ToJson());
                    });
                };
                return settings;
            });

            collection.AddSingleton<IMongoClient>(x =>
            {
                var settings = x.GetService<MongoClientSettings>();
                return new MongoClient(settings);
            });

            collection.AddSingleton<IMongoDatabase>(x =>
            {
                var databaseName = x.GetService<MongoUrl>()!.DatabaseName;
                var client = x.GetService<IMongoClient>();
                var database = client!.GetDatabase(databaseName);
                return database;
            }
                );


            // kolejność wstrzykiwania to kolejność uruchomień

            collection.AddSingleton<IBookRepository, BookRepository>();
            collection.AddHostedService<MongoInitService>();
            collection.AddHostedService<UserInterfaceService>();

            BookModelSetup.Setup();
        }
    }
}
