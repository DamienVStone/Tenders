using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Tenders.Core.Abstractions.Services;

namespace Tenders.Core.Services
{
    public class HttpContentService : IHttpContentService
    {
        public FormUrlEncodedContent GetFormUrlEncoded(params string[] keyValues)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var keyValue in keyValues)
            {
                var item = keyValue.Split('=');
                dictionary[item[0]] = item[1];
            }

            return new FormUrlEncodedContent(dictionary);
        }

        public StringContent GetJsonContent(object data)
        {
            return GetJsonContent(JsonConvert.SerializeObject(data));
        }

        public StringContent GetJsonContent(string data)
        {
            return new StringContent(data, Encoding.UTF8, "application/json");
        }
    }
}
