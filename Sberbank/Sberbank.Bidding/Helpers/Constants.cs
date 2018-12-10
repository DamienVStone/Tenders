using System;

namespace Sberbank.Bidding.Helpers
{
    public static class Constants
    {
        #region sber
        public const string SBER_AUTH_STEP1_URL = "https://login.sberbank-ast.ru/Login.aspx?ReturnUrl=%2f%3fwa%3dwsignin1.0%26wtrealm%3dhttp%253a%252f%252fwww.sberbank-ast.ru%252f%26wreply%3daHR0cDovL3d3dy5zYmVyYmFuay1hc3QucnUvdHJhZGV6b25lL2RlZmF1bHQuYXNweA%253d%253d&wa=wsignin1.0&wtrealm=http%3a%2f%2fwww.sberbank-ast.ru%2f&wreply=aHR0cDovL3d3dy5zYmVyYmFuay1hc3QucnUvdHJhZGV6b25lL2RlZmF1bHQuYXNweA%3d%3d";
        public const string SBER_AUTH_STEP2_URL = "https://login.sberbank-ast.ru/default.aspx/?wa=wsignin1.0&wtrealm=http://www.sberbank-ast.ru/&wreply=aHR0cDovL3d3dy5zYmVyYmFuay1hc3QucnUvdHJhZGV6b25lL2RlZmF1bHQuYXNweA==";
        public const string SBER_AUTH_STEP3_URL = "http://www.sberbank-ast.ru/tradezone/default.aspx";
        public const string SBER_LOGON_REGISTER_TEXT = "<logonregister><request>Просьба обеспечить вход в личный кабинет Электронной торговой площадки «Сбербанк-АСТ». Подлинность и достоверность запроса на аутентификацию подтверждаю.</request><now>{{NOW}}</now><ticket>{{TICKET}}</ticket></logonregister>";
        public const string SBER_COOKIE_TEMPLATE = "_ym_uid=1502459726551214073; _ga=GA1.2.238098008.1502459722; __utma=99173852.238098008.1502459722.1523281306.1527755093.125; _ym_d=1530176747; ASP.NET_SessionId=f4n0j0sn54m5mrvp1jzfv3jk; {{AUTH_EXTRA_INFO}}";
        public const string SBER_SEARCH_TEMPLATE = "<query><purchcode>{{REG_NUMBER}}</purchcode><purchtype></purchtype><purchname></purchname><purchamountstart></purchamountstart><purchamountend></purchamountend><issmp>-1</issmp><orgid></orgid><orgname></orgname><purchstate></purchstate><purchbranchid></purchbranchid><purchbranchname></purchbranchname><regionid></regionid><regionname></regionname><publicdatestart></publicdatestart><publicdateend></publicdateend><requestdatestart></requestdatestart><requestdateend></requestdateend><auctionbegindatestart></auctionbegindatestart><auctionbegindateend></auctionbegindateend></query>";
        public const string SBER_SEARCH_URL = "http://www.sberbank-ast.ru/tradezone/Supplier/PurchaseRequestList.aspx";
        public const string SBER_TRADE_PLACE_URL_TEMPLATE = "http://www.sberbank-ast.ru/tradezone/Supplier/TradePlace.aspx?reqid={{TRADE_ID}}&ASID={{ASID}}";
        #endregion

        #region API
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
        public static readonly string API_GET_FUTURE_AUCTIONS_URL = Environment.GetEnvironmentVariable("API_GET_FUTURE_AUCTIONS_URL");
        public static readonly string API_SYNCRONIZE_BY_KEY_URL = Environment.GetEnvironmentVariable("API_SYNCRONIZE_BY_KEY_URL");
        public static readonly string AUCTION_MANAGER_TOKEN = Environment.GetEnvironmentVariable("AUCTION_MANAGER_TOKEN");
        #endregion
    }
}
