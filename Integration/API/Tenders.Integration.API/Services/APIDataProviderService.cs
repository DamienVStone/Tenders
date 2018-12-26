using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
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
        
        public async Task<string> SignAsync(string data, CancellationToken ct)
        {
            await logger.Log("Подписание документа");
            var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("Data", data) });
            var result = await httpClientService.PostAsync(configService.SignText, content, ct);
            await logger.Log("Документ подписан");
            return JsonConvert
                .DeserializeObject<StringResponse>(result.Text)
                .Data;
        }

        public async Task SyncronizeByKeyAsync(string key, CancellationToken ct)
        {
            await logger.Log("Ожидание очереди");
            await httpClientService.GetAsync($"{configService.SynchronizeByKey}?token={configService.SecurityToken}&key={key}", ct);
            await logger.Log("Очередь освободилась");
        }
    }
}
