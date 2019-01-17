using Tenders.API.Models;

namespace Tenders.API.DAL.Interfaces
{
    public interface IFTPPathRepo : IAPIRepository<FTPPath>
    {
        FTPPath GetSinglePathByName(string PathName, bool IsActive = true);
        bool PathExistsByName(string PathName, bool IsActive = true);
        FTPPath GetOldestIndexedPath(int Timeout);
    }
}
