using System.Collections.Generic;
using Tenders.Core.Abstractions.Services;

namespace Tenders.API.Services
{
    public interface IAPIConfigService
    {
        string DbConnectionString { get; }
        string DbName { get; }
        /// <summary>
        /// Количество часов через которое путь на FTP должен быть автоматически проиндексирован. Используется в PathService
        /// </summary>
        int FTPIndexingTimeout { get; }
    }
    public class APIConfigService : IAPIConfigService
    {

        public string DbConnectionString { get; private set; }

        public int FTPIndexingTimeout { get; private set; }

        public string DbName { get; private set; }

        public APIConfigService(IConfigService config)
        {
            if (config == null) throw new System.ArgumentNullException(nameof(config));

            string creds = "";

            try
            {
                var username = config["api.DbUsername"];
                if (!string.IsNullOrWhiteSpace(username))
                {
                    creds += username;
                    try
                    {
                        var password = config["api.DbPassword"];
                        if (!string.IsNullOrWhiteSpace(password)) creds += ":" + password;
                    }
                    catch (KeyNotFoundException) { }
                    creds += "@";
                }
            }
            catch (KeyNotFoundException) {}

            DbConnectionString = $"{config["api.DbConnectionScheme"]}://{creds}{config["api.DbHost"]}:{config["api.DbPort"]}/";
            DbName = config["api.DbName"];
            FTPIndexingTimeout = int.Parse(config["api.FTPIndexingTimeout"]);
        }

    }
}
