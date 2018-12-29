using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TenderPlanIndexer.Models;
using Tenders.Core.Abstractions.Services;
using Tenders.Core.DI;
using Tenders.Core.Services;
using Tenders.Integration.API.Interfaces;
using Tenders.Integration.API.Services;

namespace TenderPlanIndexer
{
    class Program
    {
        private static IAPIDataProviderService apiDataProvider;
        private static ILoggerService logger;

        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            _initContainer();
            _doIndex(cts.Token).Wait();
        }

        private async static Task<bool> _doIndex(CancellationToken ct)
        {
            apiDataProvider.Authenticate(ct).Wait();

            var index = await apiDataProvider
                .GetCurrentIndexAsync<TenderPlanIndex>(ct)
                .ContinueWith(r => r.Result.ToDictionary(i => i.TenderPlanId));

            var files = await apiDataProvider
                .GetUpdatedTenderPlansAsync<TenderPlanFileToIndex>(ct)
                .ContinueWith(r => r.Result
                                .Select(f => new { fid = f.FTPFileId, s = f.Name.Split("_") })
                                .Where(p => p.s.Length == 3)
                                .Select(s => new { FileId = s.fid, PlanId = s.s[1], Revision = long.Parse(s.s[2].Substring(0, s.s[2].LastIndexOf('.'))) })
                                .GroupBy(
                                    t => t.PlanId,
                                    t => new { Rev = t.Revision, FId = t.FileId },
                                    (k, v) => new TenderPlanIndex
                                    {
                                        FTPFileId = v.FirstOrDefault().FId,
                                        TenderPlanId = k,
                                        RevisionId = v.Max(t => t.Rev)
                                    }
                                )
                                .ToList());

            var key = new object();
            var filesToSend = new List<TenderPlanIndex>();
            files.AsParallel().ForAll(fti =>
            {
                if (index.Keys.Contains(fti.TenderPlanId) && index[fti.TenderPlanId].RevisionId < fti.RevisionId)
                {
                    lock (key)
                    {
                        filesToSend.Add(fti);
                    }
                }
                else if (!index.Keys.Contains(fti.TenderPlanId))
                {
                    lock (key)
                    {
                        filesToSend.Add(fti);
                    }
                }
            });

            return await apiDataProvider.SendNewIndexedFiles(new StringContent(JsonConvert.SerializeObject(filesToSend), Encoding.UTF8, MediaTypeNames.Application.Json), ct);
        }

        private static void _initContainer()
        {
            Container.Init(sc =>
            {
                Container.Registration.Register(sc);
                sc.AddSingleton<IAPIDataProviderService, APIDataProviderService>();
                sc.AddSingleton<IAPIConfigService, APIConfigService>();
#if DEBUG
                sc.AddSingleton<IConfigService, LocalFileConfigService>();
#else
                sc.AddSingleton<IConfigService, EnvironmentVariablesConfigService>();
#endif
                sc.AddSingleton<IAPIHttpClientService, APIHttpClientService>();
                sc.AddSingleton<ILoggerService, LoggerService>();
            });

            apiDataProvider = Container.GetService<IAPIDataProviderService>();
            logger = Container.GetService<ILoggerService>();
        }
    }
}
