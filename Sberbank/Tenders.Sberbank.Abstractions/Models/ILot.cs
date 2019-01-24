using System;
using Tenders.Core.Abstractions.Models;

namespace Tenders.Sberbank.Abstractions.Models
{
    public interface ILot : IModelBase
    {
        DateTime RequestStart { get; set; }
        DateTime RequestSent { get; set; }
        string ViewUrl { get; set; }
        decimal Sum { get; set; }
        int BidOrder { get; set; }
        string Description { get; set; }
        string ExternalId { get; set; }
        string NotificationNumber { get; set; }
        Uri Url { get; set; }
        string Text { get; set; }
    }

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
        DivisionProcedure DivisionProcedure { get; set; }
    }
}
