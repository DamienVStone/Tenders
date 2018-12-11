using Newtonsoft.Json;
using Sberbank.Bidding.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Sberbank.Bidding.Helpers
{
    public static partial class Api
    {
        private static Token _token = default(Token);
        private static HttpClient _client;

        static Api()
        {
            _client = Http.GetApiClient();
        }

        public static async Task<string> SignAsync(this string data, CancellationToken ct)
        {
            var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("Data", data) });
            return JsonConvert
                .DeserializeObject<StringResponse>(await Http.StringRequestPost(Constants.API_SIGN_TEXT_URL, content, _client, ct))
                .Data;
        }

        public static async Task<string> GetFingerprintAsync(CancellationToken ct)
        {
            return JsonConvert
                .DeserializeObject<StringResponse>(await Http.StringRequestGet(Constants.API_GET_FINGERPRINT_URL, _client, ct))
                .Data;
        }

        //public static async Task<FutureAuction[]> GetFutureAuctionsAsync(CancellationToken ct)
        //{
        //    var settings = new JsonSerializerSettings
        //    {
        //        DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
        //        DateTimeZoneHandling = DateTimeZoneHandling.Local
        //    };

        //    var result = await Http.StringRequestGet($"{Constants.API_GET_FUTURE_AUCTIONS_URL}?token={Constants.AUCTION_MANAGER_TOKEN}", _client, ct);
        //    if (string.IsNullOrEmpty(result))
        //        return null;

        //    return JsonConvert.DeserializeObject<FutureAuction[]>(result, settings);
        //}

        public static async Task SyncronizeByKey(CancellationToken ct)
        {
            await Http.StringRequestGet($"{Constants.API_SYNCRONIZE_BY_KEY_URL}?token={Constants.AUCTION_MANAGER_TOKEN}&key=tenders-sberbank-bidder", _client, ct);
        }

        public static async Task<FutureAuction> GetFutureAuctionAsync(CancellationToken ct)
        {
            var settings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };

            var result = await Http.StringRequestGet($"{Constants.API_GET_FUTURE_AUCTIONS_URL}?token={Constants.AUCTION_MANAGER_TOKEN}", _client, ct);
            if (string.IsNullOrEmpty(result))
                return null;

            return JsonConvert.DeserializeObject<FutureAuction>(result, settings);
        }

        public static async Task AuthenticateAsync(CancellationToken ct)
        {
            var result = await Http.StringRequestPost(Constants.API_TOKEN_URL, TokenRequest.AsContent(), _client, ct);
            _token = JsonConvert.DeserializeObject<Token>(result);
            _addAuth(_client);
        }

        private static void _addAuth(HttpClient c)
        {
            if (_token == null)
                throw new UnauthorizedAccessException("You should call Authenticate(...) before!");

            c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_token.token_type, _token.access_token);
        }
    }
}
