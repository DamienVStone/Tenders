﻿using System.Threading;
using System.Threading.Tasks;
using Tenders.Sberbank.Abstractions.Models;
using Tenders.Synchronization.FtpMonitoring.Abstractions;

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
        Task<IFtpPath> GetNextPathToIndex();
    }
}
