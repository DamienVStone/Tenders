using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogger
{
    class MaxAttemptsException : Exception
    {
        public MaxAttemptsException(string name): base("Превышено количество попыток повтора запроса для "+name){}

    }
}
