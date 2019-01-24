using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tenders.Core.Abstractions.Services;
using Tenders.Integration.API.Interfaces;
using Tenders.Sberbank.Abstractions.Models;
using Tenders.Sberbank.Models;

namespace Tenders.Integration.API.Services
{
    public class APIDataProviderService : IAPIDataProviderService
    {
        private readonly IAPIConfigService configService;
        private readonly IAPIHttpClientService httpClientService;
        private readonly ILoggerService logger;

        public APIDataProviderService(
            IAPIConfigService configService,
            IAPIHttpClientService httpClientService,
            ILoggerService logger
            )
        {
            this.configService = configService ?? throw new ArgumentNullException(nameof(configService));
            this.httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Authenticate(CancellationToken ct)
        {
            await logger.Log("Авторизация на сервере управления");
            var result = (await httpClientService.PostAsync(configService.Token, TokenRequest.AsContent(configService), ct))?.Text;
            var token = JsonConvert.DeserializeObject<Token>(result);
            httpClientService.AddDefaultHeaders("Authorization", $"{token.token_type} {token.access_token}");
            await logger.Log("Авторизация на сервере управления завершена");
        }

        public async Task<string> GetFingerprintAsync(CancellationToken ct)
        {
            await logger.Log("Получение отпечатка ЭП");
            var r = (await httpClientService.GetAsync(configService.GetFingerprint, ct))?.Text;
            if (string.IsNullOrEmpty(r))
                throw new FormatException("Json string is empty");

            try
            {
                await logger.Log("Отпечаток ЭП получен");
                return JsonConvert.DeserializeObject<StringResponse>(r).Data;
            }
            catch (Exception e)
            {
                throw new Exception("Deserialization error", e);
            }
        }

        public async Task<IAuctionInfo> GetNextAuction(CancellationToken ct)
        {
            await logger.Log("Получение следующего аукциона");
            var result = await httpClientService.GetAsync($"{configService.GetFutureAuction}?token={configService.SecurityToken}", ct);
            var auction = JsonConvert
                .DeserializeObject<AuctionInfo>(result.Text);
            await logger.Log("Получен аукцион " + result?.Text);
            return auction;
        }

        public async Task<bool> SetFutureAuctionOnServiceState(IAuctionInfo auction, CancellationToken ct)
        {
            await logger.Log($"Перевод аукциона {auction.Code} в обработку");
            var result = await _setState(auction, ct, "1");
            await logger.Log($"Аукцион {auction.Code} в обработке");
            return result;
        }

        public async Task<bool> SetFutureAuctionServicedState(IAuctionInfo auction, CancellationToken ct)
        {
            await logger.Log($"Перевод аукциона {auction.Code} в обработанные");
            var result = await _setState(auction, ct, "2");
            await logger.Log($"Аукцион {auction.Code} обработан");
            return result;
        }

        private async Task<bool> _setState(IAuctionInfo auction, CancellationToken ct, string state)
        {
            await httpClientService.PostAsync(
                $"{configService.SetFutureAuctionState}?token={configService.SecurityToken}&regNumber={auction.Code}&state={state}",
                new StringContent(""),
                ct
            );
            return true;
        }

        public async Task<SignResponse> SignAsync(string data, CancellationToken ct)
        {
            await logger.Log("Подписание документа");
            var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("Data", data) });
            var result = await httpClientService.PostAsync(configService.SignText, content, ct);
            await logger.Log("Документ подписан");
            return JsonConvert
                .DeserializeObject<SignResponse>(result.Text);
        }

        public async Task SyncronizeByKeyAsync(string key, CancellationToken ct)
        {
            await logger.Log("Ожидание очереди");
            await httpClientService.GetAsync($"{configService.SynchronizeByKey}?token={configService.SecurityToken}&key={key}", ct);
            await logger.Log("Очередь освободилась");
        }

        public async Task<T> GetNextPathForIndexing<T>(CancellationToken ct)
        {
            var result = await httpClientService.GetAsync(configService.GetNextPathForIndexingUrl, ct);
            return JsonConvert.DeserializeObject<T>(result.Text);
        }

        public async Task<T> GetNextArchiveForIndexing<T>(CancellationToken ct)
        {
            var result = await httpClientService.GetAsync(configService.GetNextArchiveForMonitoring, ct);
            return JsonConvert.DeserializeObject<T>(result.Text);
        }

        public async Task<bool> SendPathFailedNotice(string Id, CancellationToken ct)
        {
            var content = new StringContent("", Encoding.Unicode, MediaTypeNames.Application.Json);
            var result = await httpClientService.PostAsync(configService.SendFailedPathNotice.AbsoluteUri + $"?id={Id}", content, ct);
            return true;
        }

        public async Task<bool> SendArchiveFailedNotice(string Id, CancellationToken ct)
        {
            var content = new StringContent("", Encoding.Unicode, MediaTypeNames.Application.Json);
            var result = await httpClientService.PostAsync(configService.SendFailedArchiveNotice.AbsoluteUri + $"?Id={Id}", content, ct);
            return true;
        }

        public async Task<string> SendFilesAsync(StringContent files, string pathId, CancellationToken ct)
        {
            var result = await httpClientService.PostAsync(configService.SendFilesUrl(pathId), files, ct);
            return result.Text;
        }

        public async Task<string> SendFileTreeAsync(StringContent files, string pathId, CancellationToken ct)
        {
            var result = await httpClientService.PostAsync(configService.SendFileTreeUrl(pathId), files, ct);
            return result.Text;
        }

        public async Task<bool> SendNewIndexedFiles(StringContent index, CancellationToken ct)
        {
            var result = await httpClientService.PostAsync(configService.SendNewIndexedFilesUrl, index, ct);
            return true;
        }

        public async Task<IEnumerable<T>> GetCurrentIndexAsync<T>(CancellationToken ct)
        {
            var result = await httpClientService.GetAsync(configService.GetCurrentIndexUrl, ct);
            return JsonConvert.DeserializeObject<List<T>>(result.Text);
        }

        public async Task<IEnumerable<T>> GetUpdatedTenderPlansAsync<T>(CancellationToken ct)
        {
            var result = await httpClientService.GetAsync(configService.GetUpdatedTenderPlansUrl, ct);
            return JsonConvert.DeserializeObject<List<T>>(result.Text);
        }

        public async Task<T> GetPathById<T>(string id, CancellationToken ct)
        {
            var res = await httpClientService.GetAsync(configService.GetPathById(id), ct);
            return JsonConvert.DeserializeObject<T>(res.Text);
        }
    }
}
