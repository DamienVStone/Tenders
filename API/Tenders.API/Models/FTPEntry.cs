using MongoDB.Bson.Serialization.Attributes;
using System;
using TenderPlanAPI.Enums;
using Tenders.API.Attributes;
using Tenders.API.DAL.Mongo;

namespace TenderPlanAPI.Models
{
    /// <summary>
    /// Элемент FTP
    /// </summary>
    public class FTPEntry : ModelBase
    {
        /// <summary>
        /// Размер
        /// </summary>
        [QuickSearch]
        public long Size { get; set; }
        /// <summary>
        /// Название
        /// </summary>
        [QuickSearch]
        public string Name { get; set; }
        /// <summary>
        /// Дата изменения
        /// </summary>
        [QuickSearch]
        public DateTimeOffset Modified { get; set; }
        /// <summary>
        /// Свойства
        /// </summary>
        [QuickSearch]
        public StateFile State { get; set; }
        /// <summary>
        /// Ссылка на путь
        /// </summary>
        [BsonSerializer(typeof(ObjectIdStringSerializer))]
        [QuickSearch]
        public string Path { get; set; }
        /// <summary>
        /// Ссылка на родителя 
        /// </summary>
        [BsonSerializer(typeof(ObjectIdStringSerializer))]
        [QuickSearch]
        public string Parent { get; set; }
        /// <summary>
        /// Является ли этот элемент дерикторией
        /// </summary>
        public bool IsDirectory { get; set; }
    }
}
