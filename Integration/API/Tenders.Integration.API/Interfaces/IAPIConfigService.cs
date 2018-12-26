using System;

namespace Tenders.Integration.API.Interfaces
{
    /// <summary>
    /// Сервис предоставления настроек для работы с управляющим API
    /// </summary>
    public interface IAPIConfigService
    {
        /// <summary>
        /// Хост
        /// </summary>
        Uri Host { get; }
        /// <summary>
        /// Url для подписания текста
        /// </summary>
        Uri SignText { get; }
        /// <summary>
        /// Url для получения отпечатка
        /// </summary>
        Uri GetFingerprint { get; }
        /// <summary>
        /// Url для получения токена авторизации
        /// </summary>
        Uri Token { get; }
        /// <summary>
        /// Url для получения прокси
        /// </summary>
        Uri GetProxy { get; }
        /// <summary>
        /// Url для синхронизации отправки запросов
        /// </summary>
        Uri SynchronizeByKey { get; }
        /// <summary>
        /// Токен безопасности для доступа к старому API
        /// </summary>
        string SecurityToken { get; }

        /// <summary>
        /// Логин к API
        /// </summary>
        string Username { get; }
        /// <summary>
        /// Пароль к API
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Флаг использования прокси
        /// </summary>
        bool UseProxy { get; }

        /// <summary>
        /// Хост прокси для отладки
        /// </summary>
        string DebugProxyHost { get; }
        /// <summary>
        /// Пользователь прокси для отладки
        /// </summary>
        string DebugProxyUsername { get; }
        /// <summary>
        /// Пароль прокси для отладки
        /// </summary>
        string DebugProxyPassword { get; }
    }
}
