using Microsoft.Extensions.DependencyInjection;
using Tenders.Core.Abstractions.Services;
using Tenders.Core.DI;
using Tenders.Core.Services;
using Tenders.Integration.API.JobSeeker.Interfaces;
using Tenders.Integration.API.JobSeeker.Services;
using Tenders.Sberbank.Abstractions.Models;
using Tenders.Sberbank.Models;
using Xunit;

namespace Tenders.Integration.API.JobSeeker.Tests
{
    public class JobSeekerConfigServiceTest
    {
        public JobSeekerConfigServiceTest()
        {
            Container.Init(sc =>
            {
                Container.Registration.Register(sc);
                sc.AddSingleton<IConfigService, LocalFileConfigService>();
                sc.AddSingleton<IJobSeekerConfigService, JobSeekerConfigService>();
            });
        }

        static IAuctionInfo auction = new AuctionInfo
        {
            Bid = 0.0m,
            Code = "000000000000",
            Workers = 30
        };

        [Fact]
        public void IsConfigFilled()
        {
            var service = Container.GetService<IJobSeekerConfigService>();
            Assert.NotNull(service);
            Assert.False(string.IsNullOrEmpty(service.GetJob(auction, "latest")));
        }
    }
}
