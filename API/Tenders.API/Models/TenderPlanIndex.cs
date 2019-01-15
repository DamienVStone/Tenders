using System;

namespace TenderPlanAPI.Models
{
    public class TenderPlanIndex : ModelBase
    {
        public TenderPlanIndex()
        {
            IsOutdated = true;
        }

        /// <summary>
        /// Идентификатор файла, которому соответствует индекс
        /// </summary>
        public string FTPFileId { get; set; }

        /// <summary>
        /// Идентификатор плана закупок
        /// </summary>
        public string TenderPlanId { get; set; }

        /// <summary>
        /// Идентификатор ревизии
        /// </summary>
        public long RevisionId { get; set; }

        /// <summary>
        /// Требуется ли перечитывать информацию из файла и обновлять в базе.
        /// </summary>
        public bool IsOutdated { get; set; }

    }
}
