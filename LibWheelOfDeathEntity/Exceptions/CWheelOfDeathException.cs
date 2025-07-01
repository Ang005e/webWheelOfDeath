using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibWheelOfDeath.Exceptions;
public class CWheelOfDeathException : ApplicationException
{
    public CWheelOfDeathException() : base() { }
    public CWheelOfDeathException(string message) : base(message) { }
    public CWheelOfDeathException(string? message, Exception? innerException) : base(message, innerException) { }
}
