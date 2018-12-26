using Newtonsoft.Json;
using System.Web;
using Tenders.Core.Abstractions.Services;
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
}
