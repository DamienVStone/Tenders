using AppLogger.Configuration;
using System;
using System.Net.Http;

namespace AppLogger
{
    public static class Common
    {
        private static string _apiUrl;

        static Common()
        {
            var section = LoggerConfigSection.Instance;
            if (section == null) throw new InvalidOperationException("Не могу найти секцию SSDSApiLogger в файле конфигурации");

            _apiUrl = section.ApiUrl;
        }

        public static void ClearOutdated()
        {
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(_apiUrl+"/api/clear", null).Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException("Не удалось очистить устревшие статусы. Ответ: "+response.StatusCode);
                }
            }
        }

    }
}
