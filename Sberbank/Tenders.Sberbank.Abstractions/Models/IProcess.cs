using System;
using Tenders.Core.Models;

namespace Tenders.Sberbank.Abstractions.Models
{
    /// <summary>
    /// Описание процесса
    /// </summary>
    public interface IProcess
    {
        /// <summary>
        /// Дата и время начала выполнения
        /// </summary>
        DateTime Started { get; set; }
        /// <summary>
        /// Дата и время завершения выполнения
        /// </summary>
        DateTime? Ended { get; set; }
        /// <summary>
        /// Состояние
        /// </summary>
        ProcessState State { get; set; }
    }
}
