using Tenders.Core.Models;

namespace Tenders.Sberbank.Abstractions.Models
{
    /// <summary>
    /// Процесс подачи заявки на участие
    /// </summary>
    public interface IRequestProcess : IProcess
    {
        /// <summary>
        /// Порядковый номер заявки после подачи
        /// </summary>
        int BidOrder { get; set; }
        /// <summary>
        /// Способ определения поставщика
        /// </summary>
        string SPOP { get; set; } // TODO: сделать справочник способов определения поставщика
        /// <summary>
        /// Результат подачи
        /// </summary>
        PurchaseRequestResult Result { get; set; }
    }
}
