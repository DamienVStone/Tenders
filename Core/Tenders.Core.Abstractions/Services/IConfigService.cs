namespace Tenders.Core.Abstractions.Services
{
    /// <summary>
    /// Сервис работы с конфигурациями приложения
    /// </summary>
    public interface IConfigService
    {
        /// <summary>
        /// Получить конфигурацию по имени
        /// </summary>
        /// <param name="index">имя конфигурации</param>
        /// <returns>конфигурация</returns>
        string this[string index] { get; }
    }//Test master commit
}
