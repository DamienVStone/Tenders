using System;

namespace Tenders.Helpers
{
    public class Constants
    {
        public static readonly string API_URL = Environment.GetEnvironmentVariable("API_URL");
        public static readonly string CORP_PROXY_URL = Environment.GetEnvironmentVariable("CORP_PROXY_URL");
        public static readonly string CORP_PROXY_LOGIN = Environment.GetEnvironmentVariable("CORP_PROXY_LOGIN");
        public static readonly string CORP_PROXY_PASSWORD = Environment.GetEnvironmentVariable("CORP_PROXY_PASSWORD");
        public static readonly string API_SIGN_TEXT_URL = API_URL + Environment.GetEnvironmentVariable("API_SIGN_TEXT_URL");
        public static readonly string API_GET_FINGERPRINT_URL = API_URL + Environment.GetEnvironmentVariable("API_GET_FINGERPRINT_URL");
        public static readonly string API_TOKEN_URL = API_URL + Environment.GetEnvironmentVariable("API_TOKEN_URL");
        public static readonly string API_LOGIN = Environment.GetEnvironmentVariable("API_LOGIN");
        public static readonly string API_PASSWORD = Environment.GetEnvironmentVariable("API_PASSWORD");
        public static readonly string API_GET_PROXY_URL = Environment.GetEnvironmentVariable("API_GET_PROXY_URL");
        public static readonly string API_SYNCRONIZE_BY_KEY_URL = "http://5.8.181.177/WebApi/SynchronizeByKey";//Environment.GetEnvironmentVariable("API_SYNCRONIZE_BY_KEY_URL");
        public static readonly string AUCTION_MANAGER_TOKEN = Environment.GetEnvironmentVariable("AUCTION_MANAGER_TOKEN");
        public static readonly string AUCTION_JSON = "{'workers':30,'code':'0895100000118000374','startTime':'2018-12-14T10:50:00','info':'Проверка 1','state':3,'minCost':0.00,'id':4,'isActive':true,'createdDate':'2018-12-07T15:44:01.117'}";//Environment.GetEnvironmentVariable("AUCTION_JSON");
    }
}
