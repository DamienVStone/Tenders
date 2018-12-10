﻿namespace Sberbank.Bidding.Helpers
{
    public static partial class Api
    {
        private class Token
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string userName { get; set; }
            public string issued { get; set; }
            public string expires { get; set; }
        }
    }
}
