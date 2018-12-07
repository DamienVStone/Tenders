using System;
using System.ComponentModel.DataAnnotations;

namespace TenderPlanAPI.Enums
{
    [SerializableAttribute]
    public enum LotState
    {

        /// <summary> 
        /// Не интересует
        /// </summary>
        [Display(Name = "Не интересует")]
        NoInterest = -1,
        /// <summary> 
        /// В ожидании
        /// </summary>
        [Display(Name = "В ожидании")]
        Pending = 0,
        /// <summary> 
        /// Найден
        /// </summary>
        [Display(Name = "Найден")]
        Found = 1,
        /// <summary>
        /// В обработке
        /// </summary>
        [Display(Name = "В обработке")]
        OnService = 2,
        /// <summary>
        /// Обработан
        /// </summary>
        [Display(Name = "Обработан")]
        Serviced = 3
    }
}
