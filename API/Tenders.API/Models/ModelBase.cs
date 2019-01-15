using System;

namespace TenderPlanAPI.Models
{
    public class ModelBase
    {
        public ModelBase()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
        }
        public string Id { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual DateTime CreatedDate { get; set; }
    }
}

