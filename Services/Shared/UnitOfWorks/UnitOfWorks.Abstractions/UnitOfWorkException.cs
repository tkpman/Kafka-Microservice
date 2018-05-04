using System;
using System.Collections.Generic;
using System.Text;

namespace UnitOfWorks.Abstractions
{
    public class UnitOfWorkException
        : Exception
    {
        public UnitOfWorkException(string message, Exception exception)
            : base(message, exception) { }
    }
}
