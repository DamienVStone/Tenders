namespace Tenders.API.Viewmodels
{
    /// <summary>
    /// Модель файла тендер плана для индексатора.
    /// </summary>
    public class TenderPlanFileToIndexViewmodel
    {
        /// <summary>
        /// Идентификатор модели FTPFile, которой соответствует это имя
        /// </summary>
        public string FTPFileId { get; set; }

        /// <summary>
        /// Имя файла
        /// </summary>
        public string Name { get; set; }

    }
}
