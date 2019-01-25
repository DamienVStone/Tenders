using System;
using System.Collections.Generic;
using Tenders.Core.Abstractions.Models;

namespace Tenders.Sberbank.Abstractions.Models
{
    public interface ILot : IExternalModelBase
    {
        DateTime PublishDate { get; set; }
        decimal Sum { get; set; }
        string Description { get; set; }
        string NotificationNumber { get; set; }
        Uri Url { get; set; }
        string Text { get; set; }
        string ETP { get; set; }
        IEnumerable<IProcess> Processes { get; set; }
    }
}
