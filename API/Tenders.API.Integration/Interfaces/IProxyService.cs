using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Tenders.Integration.API.Interfaces
{
    /// <summary>
    /// Сервис получения прокси (возможно нужен перенос в DataProvider)
    /// </summary>
    public interface IProxyService
    {
        /// <summary>
        /// Возвращает прокси
        /// </summary>
        /// <returns></returns>
        Task<WebProxy> GetProxy(CancellationToken ct);
    }
}
