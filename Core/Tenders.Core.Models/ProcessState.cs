using System.ComponentModel.DataAnnotations;

namespace Tenders.Core.Models
{
    /// <summary>
    /// Статусы процесса
    /// </summary>
    public enum ProcessState
    {
        /// <summary>
        /// Новый
        /// </summary>
        [Display(Name = "Новый")]
        New = 0,
        /// <summary>
        /// Обработка
        /// </summary>
        [Display(Name = "Обработка")]
        Processing = 1,
        /// <summary>
        /// Обработан
        /// </summary>
        [Display(Name = "Обработан")]
        Processed = 2,
    }
}
