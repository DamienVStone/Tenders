using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenderPlanIndexer.Models;

namespace TenderPlanIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            ApiClient.SetApiAddress("http://localhost/api");
#else
            ApiClient.SetApiAddress(args.Length > 0 ? args[0] : "http://localhost:1234/api");
#endif
            var indexTsk = ApiClient.Get()
                .GetCurrentIndexAsync()
                .ContinueWith(r => r.Result.ToDictionary(i => i.TenderPlanId));
            var filesTsk = ApiClient.Get()
                .GetUpdatedTenderPlansAsync()
                .ContinueWith(r => r.Result
                                .Select(f => new { fid = f.FTPFileId, s= f.Name.Split("_") })
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

            Task.WaitAll(indexTsk, filesTsk);
            var index = indexTsk.Result;
            var files = filesTsk.Result;

            var key = new object();
            var filesToSend = new List<TenderPlanIndex>();
            files.AsParallel().ForAll( fti =>
            {
                if (index.Keys.Contains(fti.TenderPlanId) && index[fti.TenderPlanId].RevisionId<fti.RevisionId)
                {
                    lock (key)
                    {
                        filesToSend.Add(fti);
                    }
                }
                else if(!index.Keys.Contains(fti.TenderPlanId))
                {
                    lock (key)
                    {
                        filesToSend.Add(fti);
                    }
                }
            });

            ApiClient.Get().SendNewIndexedFiles(filesToSend);
        }
    }
}
