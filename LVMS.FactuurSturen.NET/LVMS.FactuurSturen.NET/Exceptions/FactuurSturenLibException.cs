using System;

namespace LVMS.FactuurSturen.Exceptions
{
    public class FactuurSturenLibException : Exception
    {
        public FactuurSturenLibException()
        {

        }

        public FactuurSturenLibException(string message ) : base(message)
        {

        }

        public FactuurSturenLibException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
