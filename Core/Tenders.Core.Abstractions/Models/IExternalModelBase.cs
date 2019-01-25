namespace Tenders.Core.Abstractions.Models
{
    /// <summary>
    /// Базовая модель с внешней ссылкой
    /// </summary>
    public interface IExternalModelBase : IModelBase
    {
        /// <summary>
        /// Внешний идентификатор
        /// </summary>
        string ExternalId { get; set; }
    }
}
