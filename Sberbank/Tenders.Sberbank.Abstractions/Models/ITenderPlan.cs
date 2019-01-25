using System;
using Tenders.Core.Abstractions.Models;

namespace Tenders.Sberbank.Abstractions.Models
{
    /// <summary>
    /// План закупок
    /// </summary>
    public interface ITenderPlan : IModelBase
    {
        /// <summary>
        /// Внешний ключ
        /// </summary>
        string ExternalId { get; set; }
        /// <summary>
        /// Номер плана
        /// </summary>
        string PlanNumber { get; set; }
        /// <summary>
        /// Год
        /// </summary>
        int Year { get; set; }
        /// <summary>
        /// Наименование поставщика
        /// </summary>
        string CustomerName { get; set; }
        /// <summary>
        /// Номер поставщика
        /// </summary>
        string CustomerRegNum { get; set; }
        /// <summary>
        /// Дата публикации
        /// </summary>
        DateTime PublishDate { get; set; }
        /// <summary>
        /// Версия документа
        /// </summary>
        int Version { get; set; }
        /// <summary>
        /// Позиции
        /// </summary>
        ITenderPlanPosition[] Positions { get; set; }
    }
}
