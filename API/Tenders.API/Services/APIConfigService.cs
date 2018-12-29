using Tenders.Core.Abstractions.Services;

namespace Tenders.API.Services
{
    public interface IAPIConfigService
    {
        string DbConnectionString { get; }
    }
    public class APIConfigService : IAPIConfigService
    {

        public string DbConnectionString { get; private set; }

        public APIConfigService(IConfigService config)
        {
            if (config == null) throw new System.ArgumentNullException(nameof(config));

            DbConnectionString = config["api.DbConnectionString"];

        }

    }
}
