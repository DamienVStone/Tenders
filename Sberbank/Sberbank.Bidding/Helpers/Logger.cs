using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sberbank.Bidding.Helpers
{
    public static class Logger
    {
        private static ConcurrentBag<string> _logs = new ConcurrentBag<string>();
        public static async void Log(this string message)
        {
            await Task.Run(() =>
            {
                message += Environment.NewLine + "-----------------------------------------------------------------------------------";
                _logs.Add(message);
                Console.WriteLine(message);
            });
        }

        public static void Log(this object o)
        {
            Log(o.ToString());
        }

        public static Task<T> LogElapsed<T>(Func<Task<T>> a, string message)
        {
            var sw = new Stopwatch();
            sw.Start();
            var task = a();
            task.ContinueWith(t =>
            {
                sw.Stop();
                Log($"Message={message};Elapsed={sw.Elapsed}");
            });

            return task;
        }
    }
}
