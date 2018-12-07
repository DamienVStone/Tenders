using AppLogger.Models;
using AppLogger.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web.Script.Serialization;

namespace AppLogger
{
    class LogCache
    {
        readonly object _cacheLock = new object();
        List<LogEntry> _cache = new List<LogEntry>();

        public LogCache(Action<List<string>> SendAction, long TimerInterval)
        {
            var serializer = new JavaScriptSerializer();

            var timer = new Timer();
            timer.Interval = TimerInterval;
            timer.Elapsed += (s, e) =>
            {
                timer.Stop();
                List<string> serializedData = null;
                lock (_cacheLock)
                {
                    if (_cache.Count != 0)
                    {
                        serializedData = _cache
                            .SplitIntoBatches(5000)
                            .Select(b => serializer.Serialize(b))
                            .ToList();
                        _cache.Clear();
                    }
                }

                SendAction(serializedData ?? new List<string>());
                timer.Start();
            };
            timer.Start();
        }

        public void Add(LogEntry Value)
        {
            lock (_cacheLock)
            {
                _cache.Add(Value);
            }
        }

    }
}
