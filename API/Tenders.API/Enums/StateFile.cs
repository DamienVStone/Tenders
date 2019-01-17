using System.ComponentModel.DataAnnotations;

namespace Tenders.API.Enums
{
    /// <summary>
    /// Состояние файла
    /// </summary>
    public enum StateFile
    {
        /// <summary>
        /// В Ожидании обработки
        /// </summary>
        [Display(Name = "В Ожидании обработки")]
        Pending = -1,
        /// <summary>
        /// Новый
        /// </summary>
        [Display(Name = "Новый")]
        New = 0,
        /// <summary>
        /// Изменен
        /// </summary>
        [Display(Name = "Изменен")]
        Modified = 1,
        /// <summary>
        /// Проиндексирован
        /// </summary>
        [Display(Name = "Проиндексирован")]
        Indexed = 2,
        /// <summary>
        /// Поврежден
        /// </summary>
        [Display(Name = "Поврежден")]
        Corrupted = 3
    }
}
