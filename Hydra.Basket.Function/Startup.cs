using System;
using System.Linq;
using Hydra.Basket.Function;
using Hydra.Basket.Function.Authentication;
using Hydra.Basket.Function.Infrastructure;
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

            builder.Services.AddSingleton<IMongoBase, MongoBase>();
            builder.Services.AddTransient<IWebJobAuthorizeHelper, WebJobAuthorizeHelper>();

            string mongConn = Environment.GetEnvironmentVariable("MongoConnection");
            builder.Services.AddSingleton((s) =>
            {
                IMongoClient client = new MongoClient(mongConn);

                return client;
            });
        }
    }
}