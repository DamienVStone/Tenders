using Tenders.Core.Abstractions.Services;

namespace Tenders.API.Services
{
    public interface IAPIConfigService
    {
        string DbConnectionString { get; }
        /// <summary>
        /// Количество часов через которое путь на FTP должен быть автоматически проиндексирован. Используется в PathService
        /// </summary>
        int FTPIndexingTimeout { get; }
    }
    public class APIConfigService : IAPIConfigService
    {

        public string DbConnectionString { get; private set; }

        public int FTPIndexingTimeout { get; private set; }

        public APIConfigService(IConfigService config)
        {
            if (config == null) throw new System.ArgumentNullException(nameof(config));

            DbConnectionString = config["api.DbConnectionString"];
            FTPIndexingTimeout = int.Parse(config["api.FTPIndexingTimeout"]);
        }

    }
}
