using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tenders.Integration.API.Services;
using Tenders.Sberbank.Abstractions.Models;

namespace Tenders.Integration.API.Interfaces
{
    public interface IAPIDataProviderService
    {
        Task Authenticate(CancellationToken ct);
        Task<SignResponse> SignAsync(string data, CancellationToken ct);
        Task<string> GetFingerprintAsync(CancellationToken ct);
        Task SyncronizeByKeyAsync(string key, CancellationToken ct);
        Task<IAuctionInfo> GetNextAuction(CancellationToken ct);
        Task<bool> SetFutureAuctionOnServiceState(IAuctionInfo auction, CancellationToken ct);
        Task<bool> SetFutureAuctionServicedState(IAuctionInfo auction, CancellationToken ct);
        Task<T> GetNextPathForIndexing<T>(CancellationToken ct);
        Task<T> GetNextArchiveForIndexing<T>(CancellationToken ct);
        Task<T> GetPathById<T>(string id, CancellationToken ct);
        Task<bool> SendPathFailedNotice(string Id, CancellationToken ct);
        Task<bool> SendArchiveFailedNotice(string Id, CancellationToken ct);
        Task<string> SendFilesAsync(StringContent files, string pathId, CancellationToken ct);
        Task<string> SendFileTreeAsync(StringContent files, string pathId, string rootId, CancellationToken ct);
        Task<bool> SendNewIndexedFiles(StringContent index, CancellationToken ct);
        Task<IEnumerable<T>> GetCurrentIndexAsync<T>(CancellationToken ct);
        Task<IEnumerable<T>> GetUpdatedTenderPlansAsync<T>(CancellationToken ct);
    }
}
