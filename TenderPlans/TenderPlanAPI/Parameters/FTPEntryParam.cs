using MongoDB.Bson;
using System;

namespace TenderPlanAPI.Parameters
{
    public class FTPEntryParam
    {
        public ObjectId Id { get; set; }
        /// <summary>
        /// Размер
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// Является ли данный файл директорией
        /// </summary>
        public bool IsDirectory { get; set; }
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Дата изменения
        /// </summary>
        public DateTimeOffset DateModified { get; set; }
        /// <summary>
        /// Ссылка на путь
        /// </summary>
        public ObjectId Parent { get; set; }
    }
}
