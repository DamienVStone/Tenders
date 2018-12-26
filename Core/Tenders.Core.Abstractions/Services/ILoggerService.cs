using System.Threading.Tasks;

namespace Tenders.Core.Abstractions.Services
{
    /// <summary>
    /// Сервис логирования
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// Добавление данных в лог
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <returns></returns>
        Task Log(string message);
        /// <summary>
        /// Установка состояния для ключа
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="state">Состояние</param>
        /// <returns></returns>
        Task State(string key, string state);
    }
}
