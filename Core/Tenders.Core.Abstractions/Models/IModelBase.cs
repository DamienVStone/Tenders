using System;

namespace Tenders.Core.Abstractions.Models
{
    /// <summary>
    /// Базовая модель хранилища
    /// </summary>
    public interface IModelBase
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// Флаг активности
        /// </summary>
        bool IsActive { get; set; }
        /// <summary>
        /// Дата создания
        /// </summary>
        DateTime CreatedDate { get; set; }
        /// <summary>
        /// Строка быстрого поиска
        /// </summary>
        string QuickSearch { get; set; }
        /// <summary>
        /// Метод генерации строки быстрого поиска
        /// </summary>
        void GenerateQuickSearchString();
    }
}
