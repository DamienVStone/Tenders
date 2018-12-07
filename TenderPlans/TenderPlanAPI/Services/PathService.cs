using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using TenderPlanAPI.Controllers;
using TenderPlanAPI.Models;

namespace TenderPlanAPI.Services
{
    public interface IPathService
    {
        FTPPath GetNotIndexedPath();
    }

    // Это синглтон. Смотри конфигурацию в файле Startup
    public class PathService: IPathService
    {
        private readonly object key = new object();
        private IConfiguration _config;

        public PathService(IConfiguration config):base()
        {
            _config = config;
        }

        public FTPPath GetNotIndexedPath()
        {
            lock(key){
                var timeout = int.Parse(_config["FTPPathIndexingTimeout"]);
                var filter = Builders<FTPPath>.Filter.Lte("LastTimeIndexed", DateTimeOffset.Now.AddHours(-timeout));
                var update = Builders<FTPPath>.Update.Set("LastTimeIndexed", DateTimeOffset.Now);
                return new DBConnectContext().FTPPath.FindOneAndUpdate(filter, update);
            }
        }
    }
}
