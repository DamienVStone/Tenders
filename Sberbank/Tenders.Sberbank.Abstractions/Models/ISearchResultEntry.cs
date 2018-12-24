namespace Tenders.Sberbank.Abstractions.Models
{
    public interface ISearchResultEntry
    {
#pragma warning disable IDE1006 // Стили именования
        string reqID { get; set; }
        string purchID { get; set; }
        string ASID { get; set; }
#pragma warning restore IDE1006 // Стили именования
    }
}
