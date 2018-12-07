using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLogger
{
    class UnauthorizedException : Exception
    {
        public UnauthorizedException(string name) : base("Неверный логин/пароль для логгера: " + name) { }
    }
}
