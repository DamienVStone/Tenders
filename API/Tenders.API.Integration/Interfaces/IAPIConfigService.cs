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
        /// Url для задания состояния будущего аукциона
        /// </summary>
        Uri SetFutureAuctionState { get; }
        /// <summary>
        /// Url для получения будущего аукциона
        /// </summary>
        Uri GetFutureAuction { get; }
        /// <summary>
        /// Url для файлов которые подлежат обновлению в индексе
        /// </summary>
        Uri SendNewIndexedFilesUrl { get; }
        /// <summary>
        /// Url текущего состояния индекса
        /// </summary>
        Uri GetCurrentIndexUrl { get; }
        /// <summary>
        /// Url списока файлов, подлежащих индексации
        /// </summary>
        Uri GetUpdatedTenderPlansUrl { get; }
        /// <summary>
        /// Url получения следующего пути для индексации
        /// </summary>
        Uri GetNextPathForIndexingUrl { get; }
        Uri GetNextArchiveForMonitoring { get; }
        Uri SendFailedPathNotice { get; }
        Uri SendFailedArchiveNotice { get; }
        /// <summary>
        /// Url для отправки файлов
        /// </summary>
        Uri SendFilesUrl(string pathId);
        /// <summary>
        /// Url для отправки дерева файлов
        /// </summary>
        Uri SendFileTreeUrl(string pathId, string rootId);

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
        Uri GetPathById(string Id);
    }
}
