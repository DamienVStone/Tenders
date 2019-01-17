using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TenderPlanAPI.Models
{
    public class TenderPlan : ModelBase
    {
        /// <summary>
        /// Внешний ключ
        /// </summary>
        [Display(Name = "Внешний ключ")]
        public string ExternalId { get; set; }
        /// <summary>
        /// Номер плана
        /// </summary>
        [Display(Name = "Номер плана")]
        public string PlanNumber { get; set; }
        /// <summary>
        /// Год
        /// </summary>
        [Display(Name = "Год")]
        public int Year { get; set; }
        /// <summary>
        /// Дата публикации
        /// </summary>
        [Display(Name = "Дата публикации")]
        public DateTime PublishDate { get; set; }
        /// <summary>
        /// Версия документа
        /// </summary>
        [Display(Name = "Версия документа")]
        public int Version { get; set; }
        ///// <summary>
        ///// Заказчик
        ///// </summary>
        [Display(Name = "Заказчик")]
        public Customer Customer { get; set; }
        ///// <summary>
        ///// Позиции
        ///// </summary>
        [Display(Name = "Позиции")]
        public ICollection<TenderPlanPosition> Positions { get; set; }
    }
}
