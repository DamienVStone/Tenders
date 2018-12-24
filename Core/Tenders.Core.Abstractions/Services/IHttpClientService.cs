using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Tenders.Core.Abstractions.Services
{
    /// <summary>
    /// Сервис работы с Http запросами
    /// </summary>
    public interface IHttpClientService
    {
        /// <summary>
        /// Get запрос
        /// </summary>
        /// <param name="url">Адрес</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns></returns>
        Task<HtmlDocument> GetAsync(string url, CancellationToken ct);

        /// <summary>
        /// Get запрос
        /// </summary>
        /// <param name="url">Адрес</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns></returns>
        Task<HtmlDocument> GetAsync(Uri url, CancellationToken ct);

        /// <summary>
        /// Post запрос
        /// </summary>
        /// <param name="url">Адрес</param>
        /// <param name="formUrlEncodedContent">Тело запроса</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns></returns>
        Task<HtmlDocument> PostAsync(string url, HttpContent formUrlEncodedContent, CancellationToken ct);

        /// <summary>
        /// Post запрос
        /// </summary>
        /// <param name="url">Адрес</param>
        /// <param name="formUrlEncodedContent">Тело запроса</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns></returns>
        Task<HtmlDocument> PostAsync(Uri url, HttpContent formUrlEncodedContent, CancellationToken ct);

        void AddDefaultHeaders(string key, string value);
    }
}
