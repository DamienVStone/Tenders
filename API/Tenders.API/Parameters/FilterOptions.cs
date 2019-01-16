using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenderPlanAPI.Parameters
{
    public class FilterOptions
    {
        public FilterOptions()
        {
            Path = "";
            Login = "";
        }
        private int pageSize;
        public int Page { get; set; }
        public int PageSize { get { return pageSize; } set { pageSize = value > 50 ? 50 : value; } }
        public int Skip { get { return Page * PageSize; } }
        public int Take { get { return PageSize; } }
       
    }
}
