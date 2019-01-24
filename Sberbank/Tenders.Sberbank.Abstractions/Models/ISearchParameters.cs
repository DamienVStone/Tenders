using System;

namespace Tenders.Sberbank.Abstractions.Models
{
    /// <summary>
    /// Параметры фильтрации процедур
    /// </summary>
    public interface ISearchParameters
    {
        /// <summary>
        /// Номер извещения
        /// </summary>
        string NotificationNumber { get; set; }

        /// <summary>
        /// Искомый текст
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Дата публикации с 
        /// </summary>
        DateTime PublicDateFrom { get; set; }

        /// <summary>
        /// Дата публикации по
        /// </summary>
        DateTime PublicDateTo { get; set; }
    }
}
