﻿using System.ComponentModel.DataAnnotations;

namespace Tenders.API.Parameters
{
    public class TenderPlanIndexParam
    {
        /// <summary>
        /// Идентификатор файла с которым связан индекс
        /// </summary>
        [Required]
        public string FTPFileId { get; set; }

        /// <summary>
        /// Идентификатор плана закупок
        /// </summary>
        [Required]
        public string TenderPlanId { get; set; }

        /// <summary>
        /// Идентификатор ревизии
        /// Тип обусловлен необходимостью сравнения и поиска максимума
        /// </summary>
        [Required]
        public long RevisionId { get; set; }
    }
}
