using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using Tenders.Core.Abstractions.Services;
using Tenders.Core.DI;
using Tenders.Core.Services;
using Tenders.Integration.API.Interfaces;
using Tenders.Integration.API.Services;
using Tenders.Sberbank.Abstractions.Services;
using Tenders.Sberbank.Models;
using Tenders.Sberbank.Services;

namespace Tenders.Sberbank.Requesting
{
    public class Program
    {
        private static readonly ISberbankActionsService actionsService;
        private static readonly IAPIDataProviderService apiService;
        private static CancellationTokenSource cts;
        private static CancellationToken ct;

        static Program()
        {
            Container.Init(sc =>
            {
                Container.Registration.Register(sc);
                sc.AddTransient<ISberbankActionsService, SberbankActionsService>();
                sc.AddTransient<ISberbankHttpClientService, SberbankHttpClientService>();
                sc.AddSingleton<ISberbankConfigService, SberbankConfigService>();
                sc.AddSingleton<ISberbankXmlService, SberbankXmlService>();
                sc.AddSingleton<IAPIDataProviderService, APIDataProviderService>();
                sc.AddSingleton<IAPIConfigService, APIConfigService>();
                sc.AddSingleton<IAPIHttpClientService, APIHttpClientService>();
                sc.AddSingleton<ILoggerService, LoggerService>();
                sc.AddSingleton<IProxyService, ProxyService>();
            });

            actionsService = Container.GetService<ISberbankActionsService>();
            apiService = Container.GetService<IAPIDataProviderService>();
            cts = new CancellationTokenSource();
            ct = cts.Token;
        }

        static int Main(string[] args)
        {
            _start();
            return 0;
        }

        static async void _start()
        {
            await apiService.Authenticate(ct);
            await actionsService.AuthenticateAsync(ct);

            while (true)
            {
                var localActionsService = Container.GetService<ISberbankActionsService>();

                var from = DateTime.Now.Date;
                var to = DateTime.Now.AddDays(1).Date;
                var searchParameters = new SearchParameters
                {
                    Text = "страхование осаго осгоп",
                    PublicDateFrom = from,
                    PublicDateTo = to
                };

                var lots = localActionsService.GuestSearchAsync(searchParameters, ct);

                // TODO
                // 1. Кеш поданных
                // 2. Подача
                // 3. Отправка в управление аукционами
            }
        }
    }
}
