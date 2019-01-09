namespace Tenders.Synchronization.TenderPlan.Indexer.Models
{
    /// <summary>
    /// Файл индекса плана тендера
    /// Получается из общего формата файла tenderPlan[ГОД]_[TenderPlanId]_[TenderPlanRevision]
    /// </summary>
    public class TenderPlanIndex
    {
        /// <summary>
        /// Идентификатор файла (с которым связан этот индекс) на API.
        /// </summary>
        public string FTPFileId;
        /// <summary>
        /// Идентификатор плана тендера.
        /// Является первичным ключем на госзакупках
        /// </summary>
        public string TenderPlanId;

        /// <summary>
        /// Идентификатор ревизии.
        /// Должен быть как можно больше в рамках одного плана тендеров.
        /// Среди файлов с одинаковым TenderPlanId файл с наибольшим считается действующим.
        /// Тип обусловлен необходимостью сравнения и поиска максимума
        /// </summary>
        public long RevisionId;

    }
}
