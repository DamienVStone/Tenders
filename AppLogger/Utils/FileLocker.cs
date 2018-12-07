using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogger.Utils
{
    class FileLocker
    {

        private static FileLocker _ins;
        private static object key = new object();

        public static FileLocker Instance {
            get
            {
                if(_ins == null)
                {
                    lock (key)
                    {
                        if (_ins == null)
                        {
                            _ins = new FileLocker();
                        }
                    }
                }

                return _ins;
            }
        }

        //------------------------------------------------------------------------------

        private object o = new object();
        private Dictionary<string, object> _lockInfo = new Dictionary<string, object>();

        private FileLocker() { }

        public void LockAction(string key, Action action)
        {
            if (!_lockInfo.ContainsKey(key))
            {
                lock (o)
                {
                    if (!_lockInfo.ContainsKey(key))
                    {
                        _lockInfo[key] = new object();
                    }
                }
            }

            lock (_lockInfo[key])
            {
                action();
            }
        }
    }
}
