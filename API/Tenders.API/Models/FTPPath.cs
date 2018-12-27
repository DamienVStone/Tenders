using System;

namespace TenderPlanAPI.Models
{
    public class FTPPath : ModelBase
    {
        public FTPPath():base()
        {
            LastTimeIndexed = DateTimeOffset.MinValue;
        }

        public string Path { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTimeOffset LastTimeIndexed { get; set; }
    }
}
