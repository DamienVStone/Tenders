using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Linq;
using Tenders.API.Attributes;
using Tenders.API.DAL.Mongo;
using Tenders.Core.Helpers;

namespace Tenders.API.Models
{
    public class ModelBase
    {
        public ModelBase()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
        }

        public void GenerateQuickSearchString()
        {
            GetType()
            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Where(property => property.GetCustomAttributes(typeof(QuickSearchAttribute), true).Any())
            .ToList()
            .ForEach(property =>
            {
                var result = string.Empty;
                if (property.PropertyType.IsEnum)
                {
                    var e = (Enum)property.GetValue(this);
                    result = e?.GetDisplayValue();
                }
                else if (property.PropertyType.IsClass && property.PropertyType.IsAssignableFrom(typeof(ModelBase)))
                {
                    var mb = (ModelBase)property.GetValue(this);
                    result = mb?.QuickSearch;
                }
                else
                    result = property.GetValue(this)?.ToString();

                if (!string.IsNullOrEmpty(result?.Trim()))
                    QuickSearch += $"{result.ToSearchString()}|";
            });
        }

        [BsonSerializer(typeof(ObjectIdStringSerializer))]
        [QuickSearch]
        public string Id { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public string QuickSearch { get; set; }
    }
}

