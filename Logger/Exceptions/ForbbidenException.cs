using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogger
{
    class ForbbidenException : Exception
    {
        public ForbbidenException(string name) : base("Отказано в доступе пользователю: "+name) { }
    }
}
