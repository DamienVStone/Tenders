using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenderPlanAPI.Models
{
    public class ModelBase
    {
        public ModelBase()
        {
            CreatedDate = DateTime.Now;
            Id = ObjectId.GenerateNewId();
            IsActive = true;
        }
        public ObjectId Id { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual DateTime CreatedDate { get; set; }
    }
}

