using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CryptoTracker.MarketData.DataAccess.Repositories
{
    public class SettingsRepository : IUpdateSchema
    {
        private const string NameField = "name";
        private const string ValueField = "value";

        private readonly IMongoCollection<BsonDocument> _mongoCollection;

        public SettingsRepository(IMongoCollection<BsonDocument> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }


        public async Task<T> Get<T>(string name, CancellationToken cancellationToken)
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq(NameField, name);
            BsonDocument doc = await _mongoCollection.Find(filter).SingleOrDefaultAsync(cancellationToken);

            if (doc != null && doc.TryGetValue(ValueField, out BsonValue value))
            {
                return (T) BsonTypeMapper.MapToDotNetValue(value, BsonTypeMapperOptions.Defaults);
            }

            return default(T);
        }

        public async Task Set<T>(string name, T value)
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq(NameField, name);
            BsonValue bsonValue = BsonTypeMapper.MapToBsonValue(value);
            await _mongoCollection.UpdateOneAsync(filter, Builders<BsonDocument>.Update.Set(ValueField, bsonValue), new UpdateOptions {IsUpsert = true});
        }

        async Task IUpdateSchema.Update(int currentDbSchemaVersion, CancellationToken cancellationToken)
        {
            if (currentDbSchemaVersion < 1)
            {
                // Unique index on setting name
                await _mongoCollection.EnsureIndex("idx_setting_name", Builders<BsonDocument>.IndexKeys.Ascending(NameField), cancellationToken);
            }
        }
    }
}