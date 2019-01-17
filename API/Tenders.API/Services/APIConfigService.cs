using System.Collections.Generic;
using Tenders.API.Services.Interfaces;
using Tenders.Core.Abstractions.Services;

namespace Tenders.API.Services
{
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
            catch (KeyNotFoundException) { }

            DbConnectionString = $"{config["api.DbConnectionScheme"]}://{creds}{config["api.DbHost"]}:{config["api.DbPort"]}/";
            DbName = config["api.DbName"];
            if (!string.IsNullOrEmpty(DbName))
            {
                DbConnectionString += DbName;
            }

            FTPIndexingTimeout = int.Parse(config["api.FTPIndexingTimeout"]);
        }

    }
}
