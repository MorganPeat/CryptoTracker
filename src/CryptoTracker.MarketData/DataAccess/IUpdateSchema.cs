using System.Threading;
using System.Threading.Tasks;

namespace CryptoTracker.MarketData.DataAccess
{
    public interface IUpdateSchema
    {
        Task Update(int currentDbSchemaVersion, CancellationToken cancellationToken);
    }
}