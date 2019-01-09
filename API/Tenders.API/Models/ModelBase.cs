using System;

namespace TenderPlanAPI.Models
{
    public class ModelBase
    {
        public ModelBase()
        {
            CreatedDate = DateTime.Now;
            Id = Guid.NewGuid();
            IsActive = true;
        }
        public Guid Id { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual DateTime CreatedDate { get; set; }
    }
}

