using System;
using System.ComponentModel.DataAnnotations;
using TenderPlanAPI.Enums;

namespace TenderPlanAPI.Models
{
    public class TenderPlanPosition : ModelBase
    {
        /// <summary>
        /// Дата публикации
        /// </summary>
        [Display(Name = "Дата публикации")]
        public DateTime RequestStart { get; set; }

        /// <summary>
        /// Дата подачи
        /// </summary>
        [Display(Name = "Дата подачи")]
        public DateTime RequestSent { get; set; }

        /// <summary>
        /// Дата окончания приема заявок
        /// </summary>
        [Display(Name = "Дата окончания приема заявок")]
        public DateTime RequestEnd { get; set; }

        /// <summary>
        /// Планируемая дата размещения извещения
        /// </summary>
        [Display(Name = "Планируемая дата размещения извещения")]
        public DateTime PlanPublishDate { get; set; }

        /// <summary>
        /// Планируемая дата окончания
        /// </summary>
        [Display(Name = "Планируемая дата окончания")]
        public DateTime PlanContractEnd { get; set; }

        /// <summary>
        /// Номер извещения
        /// </summary>
        [Display(Name = "Номер извещения")]
        public string RegNumber { get; set; }

        /// <summary>
        /// Ссылка для подачи
        /// </summary>
        [Display(Name = "Ссылка для подачи")]
        public string Url { get; set; }

        /// <summary>
        /// Внешний ID
        /// </summary>
        [Display(Name = "Внешний ID")]
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
        public DivisionProcedure DivisionProcedure { get; set; }

        /// <summary>
        /// Объект закупки
        /// </summary>
        [Display(Name = "Объект закупки")]
        public string Subject { get; set; }

        /// <summary>
        /// Способ определения поставщика
        /// </summary>
        [Display(Name = "Способ определения поставщика")]
        public string SPOP { get; set; }

        /// <summary>
        /// Максимальная цена контракта
        /// </summary>
        [Display(Name = "Максимальная цена контракта")]
        public decimal Price { get; set; }

        ///// <summary>
        ///// Состояние
        ///// </summary>
        [Display(Name = "Состояние")]
        public LotState State { get; set; }

        ///// <summary>
        ///// Результат
        ///// </summary>
        [Display(Name = "Результат")]
        public LotResult Result { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        [Display(Name = "Комментарий")]
        public string Description { get; set; }

        ///// <summary>
        ///// ЭТП 
        ///// </summary>
        [Display(Name = "ЭТП")]
        public ETPType ETPType { get; set; }

        /// <summary>
        /// Валюта
        /// </summary>
        [Display(Name = "Валюта")]
        public string Currency { get; set; }

        /// <summary>
        /// Регион
        /// </summary>
        [Display(Name = "Регион")]
        public string Region { get; set; }

        ///// <summary>
        ///// План закупок
        ///// </summary>
        [Display(Name = "План закупок")]
        public TenderPlan TenderPlan { get; set; }

        /// <summary>
        /// Идентификационный код закупки
        /// </summary>
        [Display(Name = "Идентификационный код закупки")]
        public string IKZ { get; set; }
    }
}
