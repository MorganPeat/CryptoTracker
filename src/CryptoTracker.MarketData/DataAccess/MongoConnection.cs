using CryptoTracker.MarketData.DataAccess.Repositories;
using CryptoTracker.MarketData.DomainModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace CryptoTracker.MarketData.DataAccess
{
    public class MongoConnection
    {
        private const string Database = "CryptoTracker_MarketData";
        private static readonly MongoCollectionSettings DefaultCollectionSettings = new MongoCollectionSettings {AssignIdOnInsert = true};
        private readonly IMongoDatabase _db;

        static MongoConnection()
        {
            BsonClassMap.RegisterClassMap<Currency>(cm =>
            {
                cm.AutoMap();
                cm.MapIdField(x => x.Id);
            });
        }

        public MongoConnection(IOptions<MongoDBOptions> mongoConfiguration, ILogger<MongoConnection> logger)
        {           
            // Keep mongo client hidden here; don't expose it to the whole app
            string connectionString = mongoConfiguration.Value.ConnectionString;
            logger.LogInformation(LoggingEvents.ConnectingToMongo, "Connecting to mongo at {connectionString}", connectionString);
            var client = new MongoClient(connectionString);
            _db = client.GetDatabase(Database);
        }


        public CurrencyRepository GetCurrencyRepository()
        {
            IMongoCollection<Currency> collection = _db.GetCollection<Currency>("Currencies", DefaultCollectionSettings);
            return new CurrencyRepository(collection);
        }

        public SettingsRepository GetSettingsRepository()
        {
            IMongoCollection<BsonDocument> collection = _db.GetCollection<BsonDocument>("Settings", DefaultCollectionSettings);
            return new SettingsRepository(collection);
        }
    }
}