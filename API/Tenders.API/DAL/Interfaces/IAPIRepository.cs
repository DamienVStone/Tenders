using System.Collections.Generic;
using TenderPlanAPI.Models;

namespace Tenders.API.DAL.Interfaces
{
    public interface IAPIRepository<T> where T : ModelBase
    {
        IEnumerable<T> Get(int Skip, int Take, bool IsActive = true);
        T GetOne(string Id);
        string Create(T Item);
        bool Update(T Item);
        bool Delete(string Id);
        bool ChangeActiveFlag(string Id, bool IsActive);
        bool Exists(string Id, bool IsActive = true);
        long CountAll(bool IsActive = true);
    }
}
