using Tenders.Core.Abstractions.Models;

namespace Tenders.Core.Models
{
    public class ExternalModelBase : ModelBase, IExternalModelBase
    {
        public string ExternalId { get; set; }
    }
}
