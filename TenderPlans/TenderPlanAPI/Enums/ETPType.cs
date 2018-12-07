using System;
using System.ComponentModel.DataAnnotations;

namespace TenderPlanAPI.Enums
{
    [SerializableAttribute]
    public enum ETPType
    {
        /// <summary>
        /// Система торгов Сбербанк-АСТ
        /// </summary>
        [Display(Name = "Система торгов Сбербанк-АСТ")]
        Sberbank = 0,
        /// <summary>
        /// Единая электронная торговая площадка
        /// </summary>
        [Display(Name = "Единая электронная торговая площадка")]
        Roseltorg = 1,
        /// <summary>
        /// Электронная площадка России 
        /// </summary>
        [Display(Name = "Электронная площадка России")]
        RTS = 2,
        /// <summary>
        /// Общероссийская система электронной торговли
        /// </summary>
        [Display(Name = "Общероссийская система электронной торговли")]
        OSET = 3,
        /// <summary>
        /// Национальная электронная площадка
        /// </summary>
        [Display(Name = "Национальная электронная площадка")]
        NEP = 4,
        /// <summary>
        /// Всероссийская универсальная площадка
        /// </summary>
        [Display(Name = "Всероссийская универсальная площадка")]
        RAD = 5
    }
}
