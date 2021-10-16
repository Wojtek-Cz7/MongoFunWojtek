using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
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
                var mongoUrl = x.GetService<MongoUrl>();
                var settings = MongoClientSettings.FromUrl(mongoUrl);
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
            collection.AddHostedService<MongoInitService>();
            collection.AddHostedService<UserInterfaceService>();
        }
    }
}
