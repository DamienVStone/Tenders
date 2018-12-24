using System.Collections.Generic;
using System.Net.Http;
using Tenders.Integration.API.Interfaces;

namespace Tenders.Integration.API.Services
{
    public class TokenRequest
    {
        public string grant_type;
        public string username;
        public string password;

        //public static TokenRequest Create()
        //{
        //    return new TokenRequest()
        //    {
        //        grant_type = "password",
        //        password = Constants.API_PASSWORD,
        //        username = Constants.API_LOGIN
        //    };
        //}

        public static FormUrlEncodedContent AsContent(IAPIConfigService config)
        {
            return new FormUrlEncodedContent(new[] {
                         new KeyValuePair<string, string>("grant_type", "password"),
                         new KeyValuePair<string, string>("password", config.Password),
                         new KeyValuePair<string, string>("username", config.Username)
                    });
        }
    }
}
