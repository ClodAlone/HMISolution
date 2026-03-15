using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLoggerManager
{
    public sealed class ExpressionValueConverterException : Exception
    {
        #region Constructors
        public ExpressionValueConverterException()
        { }

        public ExpressionValueConverterException(string message)
            : base(message)
        { }

        public ExpressionValueConverterException(string message, Exception e)
            : base(message, e)
        { }
        #endregion
    }
}
