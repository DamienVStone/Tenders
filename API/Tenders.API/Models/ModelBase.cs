using MongoDB.Bson.Serialization.Attributes;
using System;
using Tenders.API.DAL.Mongo;

namespace TenderPlanAPI.Models
{
    public class ModelBase
    {
        public ModelBase()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
        }

        [BsonSerializer(typeof(ObjectIdStringSerializer))]
        public string Id { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual DateTime CreatedDate { get; set; }
    }
}

