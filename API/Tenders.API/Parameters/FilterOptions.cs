using System.Collections.Generic;

namespace Tenders.API.Parameters
{
    public class FilterOptions : FTPPathParam
    {
        private int pageSize;
        public int Page { get; set; }
        public int PageSize { get { return pageSize <= 0 ? 10 : pageSize; } set { pageSize = value > 50 ? 50 : value; } }
        public int Skip { get { return Page * PageSize; } }
        public int Take { get { return PageSize; } }
        public string QuickSearch { get; set; }
        public Dictionary<string, string> Filter { get; set; }
    }
}
