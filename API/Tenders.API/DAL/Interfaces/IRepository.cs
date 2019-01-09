using System;
using System.Collections.Generic;
using TenderPlanAPI.Models;

namespace Tenders.API.DAL.Intefaces
{
    public interface IRepository<T> where T:ModelBase
    {
        IEnumerable<T> GetAll();
        T GetOne(Guid id);
        void Create(T item);
        void Update(T item);
        void Delete(Guid id);
    }
}
