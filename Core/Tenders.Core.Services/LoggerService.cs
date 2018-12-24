using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tenders.Core.Abstractions.Services;

namespace Tenders.Core.Services
{
    public class LoggerService : ILoggerService
    {
        public async Task Log(string message)
        {
            await Task.Run(() =>
            {
                message = $"[{DateTime.Now}] {message}";
                Console.WriteLine(message);
                Debug.WriteLine(message);
            });
        }
    }
}
