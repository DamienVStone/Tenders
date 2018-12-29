using System.Collections.Generic;
using System.Net.Http;
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
        Task<bool> SetFutureAuctionOnServiceState(IAuctionInfo auction, CancellationToken ct);
        Task<bool> SetFutureAuctionServicedState(IAuctionInfo auction, CancellationToken ct);
        Task<T> GetNextPathForIndexing<T>(CancellationToken ct);
        Task<string> SendFilesAsync(StringContent files, string pathId, CancellationToken ct);
        Task<string> SendFileTreeAsync(StringContent files, CancellationToken ct);
        Task<bool> SendNewIndexedFiles(StringContent index, CancellationToken ct);
        Task<List<T>> GetCurrentIndexAsync<T>(CancellationToken ct);
        Task<List<T>> GetUpdatedTenderPlansAsync<T>(CancellationToken ct);
    }
}
