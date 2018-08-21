using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace CryptoTracker.MarketData.DataAccess
{
    public static class MongoIndexExtensions
    {
        public static async Task EnsureIndex<T>(this IMongoCollection<T> mongoCollection, string indexName, IndexKeysDefinition<T> definition, CancellationToken cancellationToken)
        {
            var model = new CreateIndexModel<T>(definition, new CreateIndexOptions {Unique = true, Name = indexName});
            await mongoCollection.Indexes.CreateOneAsync(model, new CreateOneIndexOptions(), cancellationToken);
        }
    }
}