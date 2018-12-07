using System;
using System.ComponentModel.DataAnnotations;

namespace TenderPlanAPI.Enums
{
    [SerializableAttribute]
    public enum LotResult
    {
        /// <summary>
        /// Не выбрано
        /// </summary>
        [Display(Name = "Не выбрано")]
        Default = 0,
        /// <summary>
        /// Подана
        /// </summary>
        [Display(Name = "Подана")]
        Requested = 1,
        /// <summary>
        /// Не подана
        /// </summary>
        [Display(Name = "Не подана")]
        NotRequested = 2,
        /// <summary>
        /// Ошибка подачи
        /// </summary>
        [Display(Name = "Ошибка подачи")]
        Cancelled = 3,
        /// <summary>
        /// Отказ от участия
        /// </summary>
        [Display(Name = "Отказ от участия")]
        Withdrawn = 4
    }
}
