using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenderPlanAPI.Enums;

namespace TenderPlanAPI.Models
{
    public class FTPEntry : ModelBase
    {
        /// <summary>
        /// Размер
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// Название
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Дата изменения
        /// </summary>
        public DateTimeOffset Modified { get; set; }
        /// <summary>
        /// Свойства
        /// </summary>
        public StateFile State { get; set; }
        /// <summary>
        /// Ссылка на путь
        /// </summary>
        public ObjectId Path { get; set; }
        /// <summary>
        /// Ссылка на родителя 
        /// </summary>
        public ObjectId? Parent { get; set; }
        /// <summary>
        /// Является ли этот элемент дерикторией
        /// </summary>
        public bool IsDirectory { get; set; }
    }
}
