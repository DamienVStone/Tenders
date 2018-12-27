using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web;
using Tenders.Core.Abstractions.Services;
using Tenders.Core.Helpers;
using Tenders.Integration.API.JobSeeker.Interfaces;
using Tenders.Sberbank.Abstractions.Models;

namespace Tenders.Integration.API.JobSeeker.Services
{
    public class JobSeekerConfigService : IJobSeekerConfigService
    {
        private readonly IConfigService configService;

        public JobSeekerConfigService(
            IConfigService configService
            )
        {
            this.configService = configService ?? throw new System.ArgumentNullException(nameof(configService));
        }

        public string GetJob(IAuctionInfo auctionInfo, string containerTag)
        {
            var result = HttpUtility
                .UrlDecode(configService["jobseeker.JobTemplate"])
                .Replace("{{REGNUMBER}}", auctionInfo.RegNumber)
                .Replace("{{BUILDNUMBER}}", containerTag)
                .Replace("{{AUCTIONINFO}}", JsonConvert.SerializeObject(auctionInfo).Replace("\"", "'"))
                .Replace("{{WORKERS}}", auctionInfo.Workers.ToString());

            return result;
        }

    }

    public class K8sConfigMapsConfigService : IConfigService
    {
        // 
        private object o = new object();
        private Dictionary<string, string> _configs;

        public string this[string index]
        {
            get
            {
                if (_configs == null)
                    lock (o)
                        if (_configs == null)
                            _configs = new Dictionary<string, string>();

                if (!_configs.ContainsKey(index))
                    lock (o)
                        if (!_configs.ContainsKey(index))
                        {
                            var result = @"kubectl get cm api -n tenders -o jsonpath='{.data.api\.TokenUrl}'".Bash()?.Trim();
                            if (string.IsNullOrEmpty(result))
                                result = @"kubectl get cm sberbank -n tenders -o jsonpath='{.data.api\.TokenUrl}'".Bash()?.Trim();
                            if (string.IsNullOrEmpty(result))
                                result = @"kubectl get cm jobseeker -n tenders -o jsonpath='{.data.api\.TokenUrl}'".Bash()?.Trim();
                            if (string.IsNullOrEmpty(result))
                                throw new KeyNotFoundException(index);
                            _configs[index] = result;
                        }

                return _configs[index];
            }
        }
    }
}
