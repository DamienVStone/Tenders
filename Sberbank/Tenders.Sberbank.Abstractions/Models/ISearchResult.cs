using Tenders.Core.Abstractions.Models;

namespace Tenders.Sberbank.Abstractions.Models
{
    /// <summary>
    /// Результат фильтрации процедур
    /// </summary>
    public interface ISearchResult
    {
        /// <summary>
        /// Отобранные процедуры
        /// </summary>
        ISearchResultEntry[] Entries { get; set; }
    }
}
