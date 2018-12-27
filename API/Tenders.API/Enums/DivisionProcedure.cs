using System;
using System.ComponentModel.DataAnnotations;

namespace TenderPlanAPI.Enums
{
    [SerializableAttribute]
    public enum DivisionProcedure
    {
        /// <summary>
        /// УКО
        /// </summary>
        [Display(Name = "1. УКО")]
        UKO = 1,
        /// <summary>
        /// ОСГОП
        /// </summary>
        [Display(Name = "1.1. ОСГОП")]
        UKOOSGOP = 11,
        /// <summary>
        /// ОСОПО
        /// </summary>
        [Display(Name = "1.2. ОСОПО")]
        UKOOSOPO = 12,
        /// <summary>
        /// ДРБ
        /// </summary>
        [Display(Name = "2. ДРБ")]
        DRB = 2,
        /// <summary>
        /// ОСАГО
        /// </summary>
        [Display(Name = "2.1. ОСАГО")]
        DRBOSAGO = 21
    }
}
