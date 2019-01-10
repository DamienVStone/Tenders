using System;
using System.Collections.Generic;
using TenderPlanAPI.Models;

namespace Tenders.API.DAL.Interfaces
{
    public interface IAPIRepository<T> where T:ModelBase
    {
        IEnumerable<T> Get(int Skip, int Take, bool IsActive = true);
        T GetOne(Guid Id);
        Guid Create(T Item);
        bool Update(T Item);
        bool Delete(Guid Id);
        bool ChangeActiveFlag(Guid Id, bool IsActive);
        bool Exists(Guid Id, bool IsActive = true);
        long CountAll(bool IsActive = true);
    }
}
