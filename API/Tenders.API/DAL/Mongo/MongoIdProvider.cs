using MongoDB.Bson;
using Tenders.API.DAL.Interfaces;

namespace Tenders.API.DAL.Mongo
{
    public class MongoIdProvider : IIdProvider
    {
        public bool AreIdsEqual(string Id, string OtherId)
        {
            return IsIdValid(Id) && IsIdValid(OtherId) && ObjectId.Parse(Id).Equals(ObjectId.Parse(OtherId));
        }

        public string GenerateId()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        public bool IsIdValid(string Id)
        {
            return !string.IsNullOrWhiteSpace(Id) && ObjectId.TryParse(Id, out ObjectId r);
        }
    }
}
