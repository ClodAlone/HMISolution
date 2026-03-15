using System;

namespace Bornander.UI.Commands.Tolerant
{
    public class CommandWarningException : Exception
    {
        public object Warning { get; private set; }


        public CommandWarningException(string message, object warning)
            : this(message, null, warning)
        {
        }

        public CommandWarningException(string message, Exception innerException, object warning)
            : base(message, innerException)
        {
            Warning = warning;
        }
    }
}
