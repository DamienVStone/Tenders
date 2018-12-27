using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenderPlanAPI.Enums
{
    public enum StateFile
    {
        /// <summary>
        /// В Ожидании обработки
        /// </summary>
        Pending = -1,
        /// <summary>
        /// Новый
        /// </summary>
        New = 0,
        /// <summary>
        /// Изменен
        /// </summary>
        Modified = 1,
        /// <summary>
        /// Проиндексирован
        /// </summary>
        Indexed = 2,
        /// <summary>
        /// Поврежден
        /// </summary>
        Corrupted = 3
    }
}
