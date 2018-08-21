using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CryptoTracker.MarketData.DomainModel;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CryptoTracker.MarketData.DataAccess.Repositories
{
    public class CurrencyRepository : IUpdateSchema
    {
        private readonly IMongoCollection<Currency> _mongoCollection;

        public CurrencyRepository(IMongoCollection<Currency> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }

        public async Task<IReadOnlyCollection<Currency>> GetAll(CancellationToken cancellationToken)
        {
            return await _mongoCollection
                .Find(new BsonDocument())
                .Sort(Builders<Currency>.Sort.Ascending(x => x.Symbol))
                .ToListAsync(cancellationToken);
        }

        public async Task Insert(IEnumerable<Currency> items, CancellationToken cancellationToken)
        {
            await _mongoCollection.InsertManyAsync(items, new InsertManyOptions {IsOrdered = false}, cancellationToken);
        }

        async Task IUpdateSchema.Update(int currentDbSchemaVersion, CancellationToken cancellationToken)
        {
            if (currentDbSchemaVersion < 1)
            {
                // Unique index on currency symbol
                await _mongoCollection.EnsureIndex("idx_currency_symbol", Builders<Currency>.IndexKeys.Ascending(ccy => ccy.Symbol), cancellationToken);

                // seed some data                
                var filter = Builders<Currency>.Filter;
                var update = Builders<Currency>.Update;
                var requests = new[]
                {
                    new UpdateOneModel<Currency>(filter.Eq(x => x.Symbol, "BTC"), update.Set(x => x.Name, "Bitcoin")) {IsUpsert = true},
                    new UpdateOneModel<Currency>(filter.Eq(x => x.Symbol, "LTC"), update.Set(x => x.Name, "Litecoin")) {IsUpsert = true},
                    new UpdateOneModel<Currency>(filter.Eq(x => x.Symbol, "ETH"), update.Set(x => x.Name, "Ethereum")) {IsUpsert = true},
                    new UpdateOneModel<Currency>(filter.Eq(x => x.Symbol, "XRP"), update.Set(x => x.Name, "Ripple")) {IsUpsert = true},
                    new UpdateOneModel<Currency>(filter.Eq(x => x.Symbol, "XMR"), update.Set(x => x.Name, "Monero")) {IsUpsert = true}
                };
                await _mongoCollection.BulkWriteAsync(requests, new BulkWriteOptions {IsOrdered = false}, cancellationToken);
            }
        }
    }
}