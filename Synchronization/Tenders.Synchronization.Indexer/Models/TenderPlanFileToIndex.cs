namespace Tenders.Synchronization.TenderPlan.Indexer.Models
{
    /// <summary>
    /// Информация о файле, получаемом от API для индексации
    /// </summary>
    public class TenderPlanFileToIndex
    {
        /// <summary>
        /// Идентификатор файла (ObjectId) в базе данных API
        /// </summary>
        public string FTPFileId { get; set; }

        /// <summary>
        /// Имя файла, подлежащего индексации
        /// </summary>
        public string Name { get; set; }

    }
}
