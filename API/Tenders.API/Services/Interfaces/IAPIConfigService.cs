namespace Tenders.API.Services.Interfaces
{
    public interface IAPIConfigService
    {
        string DbConnectionString { get; }
        string DbName { get; }
        /// <summary>
        /// Количество часов через которое путь на FTP должен быть автоматически проиндексирован. Используется в PathService
        /// </summary>
        int FTPIndexingTimeout { get; }
    }
}
