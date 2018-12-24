namespace Tenders.Sberbank.Abstractions.Models
{
    public interface IBid
    {
        string reqID { get; set; }
        string xmlData { get; set; }
        string Hash { get; set; }
    }
}
