using System;
using System.Linq;
using Hydra.Basket.Function;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

[assembly: WebJobsStartup(typeof(Startup))]
namespace Hydra.Basket.Function
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });

            string mongConn = Environment.GetEnvironmentVariable("MongoConnection");
            builder.Services.AddSingleton((s) =>
            {
                MongoClient client = new MongoClient(mongConn);

                return client;
            });
        }
    }
}