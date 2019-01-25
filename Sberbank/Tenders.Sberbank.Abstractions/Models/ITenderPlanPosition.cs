using System;
using Tenders.Core.Abstractions.Models;
using Tenders.Core.Models;

namespace Tenders.Sberbank.Abstractions.Models
{
    /// <summary>
    /// Позиция плана закупок
    /// </summary>
    public interface ITenderPlanPosition : IModelBase
    {
        /// <summary>
        /// Внешний номер плана закупок
        /// </summary>
        string TenderPlanExternalId { get; set; }
        /// <summary>
        /// Номер позиции
        /// </summary>
        string PositionNumber { get; set; }
        /// <summary>
        /// Внешний номер
        /// </summary>
        string ExtNumber { get; set; }
        /// <summary>
        /// Предмет закупки
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Планируемая дата размещения извещения
        /// </summary>
        DateTime DateStart { get; set; }
        /// <summary>
        /// Планируемая дата окончания
        /// </summary>
        DateTime DateEnd { get; set; }
        /// <summary>
        /// Максимальная цена контракта
        /// </summary>
        decimal Price { get; set; }
        /// <summary>
        /// Способ определения поставщика
        /// </summary>
        string SPOP { get; set; }
        /// <summary>
        /// Номер извещения
        /// </summary>
        string NotificationNumber { get; set; }
        /// <summary>
        /// Идентификационный код закупки
        /// </summary>
        string IKZ { get; set; }
        /// <summary>
        /// Дата размещения
        /// </summary>
        DateTime? PublishDate { get; set; }
        /// <summary>
        /// Флаг слежения за публикацией
        /// </summary>
        bool IsMonitor { get; set; }
        /// <summary>
        /// План закупок
        /// </summary>
        ITenderPlan TenderPlan { get; set; }

        /// <summary>
        /// Подразделение
        /// </summary>
        Division Division { get; set; }
    }
}
