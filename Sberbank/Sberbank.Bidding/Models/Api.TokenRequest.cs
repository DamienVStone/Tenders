using System.Collections.Generic;
using System.Net.Http;

namespace Sberbank.Bidding.Helpers
{
    public static partial class Api
    {
        private class TokenRequest
        {
            public string grant_type;
            public string username;
            public string password;

            public static TokenRequest Create()
            {
                return new TokenRequest()
                {
                    grant_type = "password",
                    password = Constants.API_PASSWORD,
                    username = Constants.API_LOGIN
                };
            }

            public static FormUrlEncodedContent AsContent()
            {
                return new FormUrlEncodedContent(new[] {
                         new KeyValuePair<string, string>("grant_type", "password"),
                         new KeyValuePair<string, string>("password", Constants.API_PASSWORD),
                         new KeyValuePair<string, string>("username", Constants.API_LOGIN)
                    });
            }
        }
    }
}
