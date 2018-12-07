using AppLogger.Models;
using AppLogger.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web.Script.Serialization;

namespace AppLogger
{
    class StatusCache
    {
        readonly object _cacheLock = new object();
        Dictionary<string, StatusEntry> _cache = new Dictionary<string, StatusEntry>();

        public StatusCache(Action<List<string>> SendAction, long TimerInterval)
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
                            .Values
                            .ToList()
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

        public void Add(string Key, StatusEntry Value)
        {
            lock (_cacheLock)
            {
                _cache[Key] = Value;
            }
        }
    }
}
