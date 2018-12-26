using Tenders.Sberbank.Abstractions.Models;

namespace Tenders.Sberbank.Models
{
    public class SearchParameters : ISearchParameters
    {
        public string Regnumber { get; set; }
    }
}
