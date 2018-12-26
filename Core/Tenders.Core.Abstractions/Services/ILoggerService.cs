using System.Threading.Tasks;

namespace Tenders.Core.Abstractions.Services
{
    public interface ILoggerService
    {
        Task Log(string message);
    }
}
