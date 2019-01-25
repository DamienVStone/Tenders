using System;
using System.Collections.Generic;
using Tenders.Core.Models;
using Tenders.Sberbank.Abstractions.Models;

namespace Tenders.Sberbank.Models
{
    public class Lot : ExternalModelBase, ILot
    {
        public DateTime PublishDate { get; set; }
        public decimal Sum { get; set; }
        public string Description { get; set; }
        public string NotificationNumber { get; set; }
        public Uri Url { get; set; }
        public string Text { get; set; }
        public string ETP { get; set; }
        public IEnumerable<IProcess> Processes { get; set; }
    }
}