using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Linq;
using Tenders.API.Attributes;
using Tenders.API.DAL.Mongo;
using Tenders.Core.Abstractions.Models;
using Tenders.Core.Helpers;

namespace Tenders.API.Models
{
    public class ModelBase : Core.Models.ModelBase
    {
        [BsonSerializer(typeof(ObjectIdStringSerializer))]
        [QuickSearch]
        public override string Id { get; set; }
    }
}

