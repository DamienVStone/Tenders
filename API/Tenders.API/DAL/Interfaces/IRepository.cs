using System;
using System.Collections.Generic;
using TenderPlanAPI.Models;

namespace Tenders.API.DAL.Interfaces
{
    public interface IAPIRepository<T> where T:ModelBase
    {
        IEnumerable<T> GetAll();
        T GetOne(Guid id);
        void Create(T item);
        void Update(T item);
        void Delete(Guid id);
    }
}
