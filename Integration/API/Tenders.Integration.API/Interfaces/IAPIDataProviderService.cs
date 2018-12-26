using System.Threading;
using System.Threading.Tasks;
using Tenders.Sberbank.Abstractions.Models;

namespace Tenders.Integration.API.Interfaces
{
    public interface IAPIDataProviderService
    {
        Task Authenticate(CancellationToken ct);
        Task<string> SignAsync(string data, CancellationToken ct);
        Task<string> GetFingerprintAsync(CancellationToken ct);
        Task SyncronizeByKeyAsync(string key, CancellationToken ct);
        Task<IAuctionInfo> GetNextAuction(CancellationToken ct);
    }
}
