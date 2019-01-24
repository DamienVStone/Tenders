using System;
using Tenders.Sberbank.Abstractions.Models;

namespace Tenders.Sberbank.Models
{
    public class SearchParameters : ISearchParameters
    {
        public string NotificationNumber { get; set; }
        public DateTime PublicDateFrom { get; set; }
        public DateTime PublicDateTo { get; set; }
        public string Text { get; set; }
    }
}
