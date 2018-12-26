using Tenders.Core.Services;
using Tenders.Integration.API.Interfaces;
using Tenders.Sberbank.Abstractions.Services;

namespace Tenders.Sberbank.Services
{
    public class SberbankDataProvider : DataProvider, ISberbankDataProvider
    {
        private readonly ISberbankConfigService configService;
        private readonly IAPIDataProviderService apiDataProviderService;

        public SberbankDataProvider(
            ISberbankConfigService configService,
            IAPIDataProviderService ApiDataProviderService
        )
        {
            this.configService = configService ?? throw new System.ArgumentNullException(nameof(configService));
            apiDataProviderService = ApiDataProviderService ?? throw new System.ArgumentNullException(nameof(ApiDataProviderService));
        }
    }
}
