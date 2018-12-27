using Tenders.Sberbank.Abstractions.Models;

namespace Tenders.Integration.API.JobSeeker.Interfaces
{
    /// <summary>
    /// Конфигурация модуля JobSeeker
    /// </summary>
    public interface IJobSeekerConfigService
    {
        /// <summary>
        /// Возвращает yaml для создания Job'а
        /// </summary>
        /// <param name="auctionInfo">Информация об аукционе</param>
        /// <returns></returns>
        string GetJob(IAuctionInfo auctionInfo, string containerTag);
    }
}
