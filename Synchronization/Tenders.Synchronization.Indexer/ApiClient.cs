//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Net.Mime;
//using System.Threading.Tasks;
//using TenderPlanIndexer.Models;

//namespace TenderPlanIndexer
//{
//    /// <summary>
//    /// Класс реализующей взаимодействие с API
//    /// </summary>
//    public class ApiClient
//    {
//        /// <summary>
//        /// Основная точка входа для API, включая слово 'api'. Например: 'http://example.com/api'
//        /// </summary>
//        private static string _apiAddress;
        
//        public static void SetApiAddress(string Address)
//        {
//            if (_apiAddress != null) throw new InvalidOperationException("Адрес АПИ же установлен");
//            _apiAddress = Address;
//        }

//        /// <summary>
//        /// Возвращает экземпляр АПИ. Такой способ выбран, чтобы все кленты имели единый адрес АПИ.
//        /// </summary>
//        public static ApiClient Get()
//        {
//            if (_apiAddress == null) throw new InvalidOperationException("Необходимо установтиь адрес АПИ");
//            return new ApiClient();
//        }

//        private ApiClient(){}

//        /// <summary>
//        /// Отправляю все файлы, которые подлежат обновлению в индексе на API
//        /// </summary>
//        /// <param name="index"></param>
//        /// <returns></returns>
//        public bool SendNewIndexedFiles(List<TenderPlanIndex> index)
//        {
//            using (var client = new HttpClient())
//            {
//                var content = new StringContent(JsonConvert.SerializeObject(index));
//                content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json);
//                var resp = client.PostAsync(_apiAddress+ "/TenderPlanIndex", content).Result;
//                return resp.IsSuccessStatusCode;
//            }
//        }

//        /// <summary>
//        /// Возвращает текущее состояние индекса
//        /// </summary>
//        public async Task<List<TenderPlanIndex>> GetCurrentIndexAsync() {
//            return await _getListFromPathAsync<TenderPlanIndex>("/TenderPlanIndex");
//        }

//        /// <summary>
//        /// Возвращает список файлов, подлежащих индексации
//        /// </summary>
//        public async Task<List<TenderPlanFileToIndex>> GetUpdatedTenderPlansAsync()
//        {
//            return await _getListFromPathAsync<TenderPlanFileToIndex>("/FTPFile/GetTenderPlansToIndex");
//        }

//        /// <summary>
//        /// Возвращает список объектов по указанному пути относительно адреса АПИ
//        /// </summary>
//        /// <typeparam name="T">Тип возвращаемых объектов</typeparam>
//        /// <param name="path">Путь к контроллеру</param>
//        /// <returns></returns>
//        private async Task<List<T>> _getListFromPathAsync<T>(string path)
//        {
//            using (var client = new HttpClient())
//            {
//                var resp = await client.GetStringAsync(_apiAddress + path);
//                return JsonConvert.DeserializeObject<List<T>>(resp);
//            }
//        }

//    }
//}
