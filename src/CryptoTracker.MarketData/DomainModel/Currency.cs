using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CryptoTracker.MarketData.DomainModel
{
    public class Currency
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
    }
}