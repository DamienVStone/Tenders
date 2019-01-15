using System.Collections.Generic;
using TenderPlanAPI.Models;
using Tenders.API.DAL.Interfaces;

namespace Tenders.API.DAL.Mongo
{
    public class BaseMongoRepo<T> : IAPIRepository<T> where T : ModelBase
    {
        public bool ChangeActiveFlag(string Id, bool IsActive)
        {
            throw new System.NotImplementedException();
        }

        public long CountAll(bool IsActive = true)
        {
            throw new System.NotImplementedException();
        }

        public string Create(T Item)
        {
            throw new System.NotImplementedException();
        }

        public bool Delete(string Id)
        {
            throw new System.NotImplementedException();
        }

        public bool Exists(string Id, bool IsActive = true)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<T> Get(int Skip, int Take, bool IsActive = true)
        {
            throw new System.NotImplementedException();
        }

        public T GetOne(string Id)
        {
            throw new System.NotImplementedException();
        }

        public bool Update(T Item)
        {
            throw new System.NotImplementedException();
        }
    }
}
