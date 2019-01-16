using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;

namespace Tenders.API.DAL.Mongo
{
    public class ObjectIdStringSerializer : SerializerBase<string>
    {
        public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var type = context.Reader.GetCurrentBsonType();
            if(type == BsonType.ObjectId)
            {
                var id = context.Reader.ReadObjectId();
                return id.ToString();
            }
            else if(type == BsonType.String)
            {
                return context.Reader.ReadString();
            }
            else
            {
                context.Reader.ReadNull();
                return null;
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                context.Writer.WriteNull();
                return;
            }
            ObjectId r;
            if (!ObjectId.TryParse(value, out r))
                throw new ArgumentException($"Значение {value} не является корректным значением типа ObjectId");

            context.Writer.WriteObjectId(r);
        }
    }
}
