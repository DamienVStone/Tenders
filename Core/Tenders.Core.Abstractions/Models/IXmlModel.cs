namespace Tenders.Core.Abstractions.Models
{
    /// <summary>
    /// Базовая модель XML объекта
    /// </summary>
    public interface IXmlModel<T>
    {
        /// <summary>
        /// Преобразование десериализованного результата к абстракции
        /// </summary>
        /// <returns></returns>
        T ToAbstraction();
    }
}
