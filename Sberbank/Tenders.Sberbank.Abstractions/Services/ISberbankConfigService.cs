using Tenders.Sberbank.Abstractions.Models;

namespace Tenders.Sberbank.Abstractions.Services
{
    /// <summary>
    /// Сервис предоставления конфигурации для площадки Сбербанк-АСТ
    /// </summary>
    public interface ISberbankConfigService
    {
        /// <summary>
        /// Информация о поведении на аукционе
        /// </summary>
        IAuctionInfo AuctionInfo { get; }

        /// <summary>
        /// Ссылка на страницу авторизации
        /// </summary>
        string AuthStep1Url { get; }
        /// <summary>
        /// Ссылка на промежуточную страницу после отправки запроса на первую ссылку
        /// </summary>
        string AuthStep2Url { get; }
        /// <summary>
        /// Ссылка для POST запроса после ответа сервера на второй шаг
        /// </summary>
        string AuthStep3Url { get; }
        /// <summary>
        /// Ссылка на страницу с поиском лотов
        /// </summary>
        string PurchaseRequestListUrl { get; }
        /// <summary>
        /// Флаг работы под отладкой
        /// </summary>
        bool IsDebug { get; }

        /// <summary>
        /// Шаблон документа на подпись для авторизации на площадке
        /// </summary>
        string GetLogonRegisterData(string timeNow, string ticket);
        /// <summary>
        /// Шаблон документа для поиска процедуры на площадке
        /// </summary>
        /// <param name="parameters">Параметры поиска</param>
        /// <returns></returns>
        string GetSearchXml(ISearchParameters parameters);
        /// <summary>
        /// Ссылка для получения данных текущих торгов
        /// </summary>
        /// <param name="tradeId">Id торгов</param>
        /// <param name="asid"></param>
        /// <returns></returns>
        string GetTradePlaceUrl(string tradeId, string asid);
        /// <summary>
        /// Ссылка для подачи предложения
        /// </summary>
        /// <param name="tradeId">Id торгов</param>
        /// <param name="asid"></param>
        /// <returns></returns>
        string GetTradePlaceBidUrl(string tradeId, string asid);
        /// <summary>
        /// Данные для подачи ценового предложения
        /// </summary>
        /// <param name="price">Сумма</param>
        /// <param name="tradePlace">Информация о торгах</param>
        /// <returns></returns>
        string GetBidData(decimal price, ITradePlace tradePlace);
        /// <summary>
        /// Данные запроса на получение данных по текущим торгам
        /// </summary>
        /// <param name="tradeId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        string GetAsyncRefreshData(string tradeId, string time);
    }
}
