using System;
using Tenders.API.Attributes;

namespace Tenders.API.Models
{
    public class FTPPath : ModelBase
    {
        public FTPPath() : base()
        {
            LastTimeIndexed = DateTimeOffset.MinValue;
        }

        [QuickSearch]
        public string Path { get; set; }
        [QuickSearch]
        public string Login { get; set; }
        [QuickSearch]
        public string Password { get; set; }
        [QuickSearch]
        public DateTimeOffset LastTimeIndexed { get; set; }
        public bool HasErrors { get; set; }
    }
}
