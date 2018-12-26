namespace Tenders.Sberbank.Abstractions.Models
{
    /// <summary>
    /// Параметры фильтрации процедур
    /// </summary>
    public interface ISearchParameters
    {
        /// <summary>
        /// Номер извещения
        /// </summary>
        string Regnumber { get; set; }
    }
}
