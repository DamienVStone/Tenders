using System.Threading;
using System.Threading.Tasks;
using Tenders.Sberbank.Abstractions.Models;

namespace Tenders.Sberbank.Abstractions.Services
{
    /// <summary>
    /// Действия на площадке
    /// </summary>
    public interface ISberbankActionsService
    {
        /// <summary>
        /// Вход на площадку
        /// </summary>
        /// <param name="ct">Токен отмены</param>
        /// <returns></returns>
        Task AuthenticateAsync(CancellationToken ct);

        /// <summary>
        /// Поиск процедуры (необходима авторизация)
        /// </summary>
        /// <param name="parameters">Параметры поиска</param>
        /// <returns></returns>
        Task<ISearchResult> SearchAsync(ISearchParameters parameters, CancellationToken ct);
        /// <summary>
        /// Получение данных о текущих торгах по аукциону
        /// </summary>
        /// <param name="auction">аукцион</param>
        /// <param name="ct">токен отмены</param>
        /// <returns></returns>
        Task<ITradePlace> GetTradeDataAsync(ISearchResultEntry auction, CancellationToken ct);

        /// <summary>
        /// Подача ценового предложения
        /// </summary>
        /// <param name="price">Сумма</param>
        /// <param name="tradeData">Данные об аукционе</param>
        Task<ITradePlace> BidAsync(decimal price, string ASID, ITradePlace tradeData, CancellationToken ct);
    }
}
