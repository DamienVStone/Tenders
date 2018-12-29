
namespace Tenders.Synchronization.FtpMonitoring.Abstractions
{
    public interface IFtpPath
    {
        string Id { get; set; }
        string Path { get; set; }
        string Login { get; set; }
        string Password { get; set; }
    }
}
