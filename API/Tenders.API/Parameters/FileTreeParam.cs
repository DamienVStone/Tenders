using MongoDB.Bson;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TenderPlanAPI.Parameters
{
    /// <summary>
    /// Модель содержащая в себе дерево проиндексированных ботом файлов для записи в бд.
    /// </summary>
    public class FileTreeParam
    {
        /// <summary>
        /// Идентификатор объекта FTPPath в котором должно быть проиндексированно это дерево
        /// </summary>
        [Required]
        public string PathId { get; set; }

        /// <summary>
        /// Файл, который является корнем дерева индексации
        /// </summary>
        [Required]
        public FTPEntryParam TreeRoot { get; set; }

        /// <summary>
        /// Файлы, составляющие дерево
        /// </summary>
        [Required]
        public ISet<FTPEntryParam> Files { get; set; }

    }
}
