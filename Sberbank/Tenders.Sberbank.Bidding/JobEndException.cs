using System;
using System.Collections.Generic;
using System.Text;

namespace Sberbank.Bidding
{
    public class JobEndException : Exception
    {
        public JobEndException() : base()
        {
        }
        public JobEndException(string message) : base(message)
        {
        }
        public JobEndException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
