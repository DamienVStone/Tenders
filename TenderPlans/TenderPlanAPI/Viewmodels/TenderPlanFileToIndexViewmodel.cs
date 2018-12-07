using MongoDB.Bson;

namespace TenderPlanAPI.Viewmodels
{
    /// <summary>
    /// Модель файла тендер плана для индексатора.
    /// </summary>
    public class TenderPlanFileToIndexViewmodel
    {
        /// <summary>
        /// Идентификатор модели FTPFile, которой соответствует это имя
        /// </summary>
        public ObjectId FTPFileId { get; set; }

        /// <summary>
        /// Имя файла
        /// </summary>
        public string Name { get; set; }

    }
}
