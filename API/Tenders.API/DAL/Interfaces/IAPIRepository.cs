using System.Collections.Generic;
using Tenders.API.Models;

namespace Tenders.API.DAL.Interfaces
{
    public interface IAPIRepository<T> where T : ModelBase
    {
        IEnumerable<T> Get(int skip, int take, string quickSearch = "", bool isActive = true);
        long Count(int skip, int take, string quickSearch = "", bool isActive = true);
        T GetOne(string Id);
        string Create(T Item);
        bool Update(T Item);
        bool Delete(string Id);
        bool ChangeActiveFlag(string Id, bool IsActive);
        bool Exists(string Id, bool IsActive = true);
        long CountAll(bool IsActive = true);
    }
}
