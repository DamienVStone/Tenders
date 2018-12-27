using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using Tenders.Core.Abstractions.Services;
using Tenders.Core.DI;
using Tenders.Core.Services;
using Tenders.Integration.API.Interfaces;
using Tenders.Integration.API.JobSeeker.Interfaces;
using Tenders.Integration.API.JobSeeker.Services;
using Tenders.Integration.API.Services;

namespace Tenders.Integration.API.JobSeeker
{
    public class Program
    {
        private static IAPIDataProviderService apiDataProvider;
        private static ILoggerService logger;
        private static IJobSeekerConfigService configService;
        private static IJobSeekerActionsService actionsService;
        private static string containerTag;

        static void Main(string[] args)
        {
            InitContainer();

            if (args.Length == 0)
                throw new ArgumentNullException(nameof(args));

            containerTag = args[0];
            if (string.IsNullOrEmpty(containerTag))
                throw new ArgumentNullException("containerTag. " + string.Join(';', args));

            _doWork();
            Console.ReadLine();
        }

        static async void _doWork()
        {
            var cts = new CancellationTokenSource();
            await apiDataProvider.Authenticate(cts.Token);
            var auction = await apiDataProvider.GetNextAuction(cts.Token);
            var job = configService.GetJob(auction, containerTag);
            await logger.Log(actionsService.RunJob(job));
            var result = await apiDataProvider.SetFutureAuctionOnServiceState(auction, cts.Token);
            if (result)
                logger.Log("Успешно завершено").Wait();
            else
                logger.Log("Не удалось изменить статус аукциона").Wait();
        }

        public static void InitContainer()
        {
            Container.Init(sc =>
            {
                Container.Registration.Register(sc);
                sc.AddSingleton<IAPIDataProviderService, APIDataProviderService>();
                sc.AddSingleton<IAPIConfigService, APIConfigService>();
                sc.AddSingleton<IAPIHttpClientService, APIHttpClientService>();
#if DEBUG
                sc.AddSingleton<IConfigService, LocalFileConfigService>();
#else
                sc.AddSingleton<IConfigService, K8sConfigMapsConfigService>();
#endif
                sc.AddSingleton<IJobSeekerConfigService, JobSeekerConfigService>();
                sc.AddSingleton<IJobSeekerActionsService, JobSeekerActionsService>();
                sc.AddSingleton<ILoggerService, LoggerService>();
                sc.AddSingleton<IProxyService, ProxyService>();
            });

            apiDataProvider = Container.GetService<IAPIDataProviderService>();
            actionsService = Container.GetService<IJobSeekerActionsService>();
            logger = Container.GetService<ILoggerService>();
            configService = Container.GetService<IJobSeekerConfigService>();
        }
    }
}
