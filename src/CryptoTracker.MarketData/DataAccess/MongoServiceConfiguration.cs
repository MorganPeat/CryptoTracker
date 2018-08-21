using System;
using CryptoTracker.MarketData.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoTracker.MarketData.DataAccess
{
    public static class MongoServiceConfiguration
    {
        public static void AddMongoRepositories(this IServiceCollection services, Action<MongoDBOptions> setupAction)
        {
            services.Configure(setupAction);
            services.AddSingleton<MongoConnection>();

            services.AddSingleton(x => x.GetService<MongoConnection>().GetCurrencyRepository());            
            services.AddSingleton(x => x.GetService<MongoConnection>().GetSettingsRepository());

            services.AddSingleton<IUpdateSchema>(x => x.GetService<CurrencyRepository>());
            services.AddSingleton<IUpdateSchema>(x => x.GetService<SettingsRepository>());

            services.AddSingleton<SchemaUpdater>();

        }
    }
}