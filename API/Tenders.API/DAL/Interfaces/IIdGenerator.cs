namespace Tenders.API.DAL.Interfaces
{
    public interface IIdProvider
    {
        string GenerateId();
        bool IsIdValid(string Id);
        bool AreIdsEqual(string Id, string OtherId);
    }
}
