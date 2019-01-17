using System;
using System.ComponentModel.DataAnnotations;
using TenderPlanAPI.Enums;
using Tenders.API.Attributes;

namespace TenderPlanAPI.Models
{
    /// <summary>
    /// Позиция плана-графика
    /// </summary>
    public class TenderPlanPosition : ModelBase
    {
        /// <summary>
        /// Дата публикации
        /// </summary>
        [Display(Name = "Дата публикации")]
        [QuickSearch]
        public DateTime RequestStart { get; set; }

        /// <summary>
        /// Дата подачи
        /// </summary>
        [Display(Name = "Дата подачи")]
        [QuickSearch]
        public DateTime RequestSent { get; set; }

        /// <summary>
        /// Дата окончания приема заявок
        /// </summary>
        [Display(Name = "Дата окончания приема заявок")]
        [QuickSearch]
        public DateTime RequestEnd { get; set; }

        /// <summary>
        /// Планируемая дата размещения извещения
        /// </summary>
        [Display(Name = "Планируемая дата размещения извещения")]
        [QuickSearch]
        public DateTime PlanPublishDate { get; set; }

        /// <summary>
        /// Планируемая дата окончания
        /// </summary>
        [Display(Name = "Планируемая дата окончания")]
        [QuickSearch]
        public DateTime PlanContractEnd { get; set; }

        /// <summary>
        /// Номер извещения
        /// </summary>
        [Display(Name = "Номер извещения")]
        [QuickSearch]
        public string RegNumber { get; set; }

        /// <summary>
        /// Ссылка для подачи
        /// </summary>
        [Display(Name = "Ссылка для подачи")]
        [QuickSearch]
        public string Url { get; set; }

        /// <summary>
        /// Внешний ID
        /// </summary>
        [Display(Name = "Внешний ID")]
        [QuickSearch]
        public string ExternalId { get; set; }

        /// <summary>
        /// Поряковый номер
        /// </summary>
        [Display(Name = "Порядковый номер")]
        public int BidOrder { get; set; }

        ///// <summary>
        ///// Подразделение
        ///// </summary>
        [Display(Name = "Подразделение")]
        [QuickSearch]
        public DivisionProcedure DivisionProcedure { get; set; }

        /// <summary>
        /// Объект закупки
        /// </summary>
        [Display(Name = "Объект закупки")]
        [QuickSearch]
        public string Subject { get; set; }

        /// <summary>
        /// Способ определения поставщика
        /// </summary>
        [Display(Name = "Способ определения поставщика")]
        [QuickSearch]
        public string SPOP { get; set; }

        /// <summary>
        /// Максимальная цена контракта
        /// </summary>
        [Display(Name = "Максимальная цена контракта")]
        [QuickSearch]
        public decimal Price { get; set; }

        ///// <summary>
        ///// Состояние
        ///// </summary>
        [Display(Name = "Состояние")]
        [QuickSearch]
        public LotState State { get; set; }

        ///// <summary>
        ///// Результат
        ///// </summary>
        [Display(Name = "Результат")]
        [QuickSearch]
        public LotResult Result { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [Display(Name = "Комментарий")]
        [QuickSearch]
        public string Description { get; set; }

        ///// <summary>
        ///// ЭТП 
        ///// </summary>
        [Display(Name = "ЭТП")]
        [QuickSearch]
        public ETPType ETPType { get; set; }

        /// <summary>
        /// Валюта
        /// </summary>
        [Display(Name = "Валюта")]
        [QuickSearch]
        public string Currency { get; set; }

        /// <summary>
        /// Регион
        /// </summary>
        [Display(Name = "Регион")]
        [QuickSearch]
        public string Region { get; set; }

        ///// <summary>
        ///// План закупок
        ///// </summary>
        [Display(Name = "План закупок")]
        [QuickSearch]
        public TenderPlan TenderPlan { get; set; }

        /// <summary>
        /// Идентификационный код закупки
        /// </summary>
        [Display(Name = "Идентификационный код закупки")]
        [QuickSearch]
        public string IKZ { get; set; }
    }
}
