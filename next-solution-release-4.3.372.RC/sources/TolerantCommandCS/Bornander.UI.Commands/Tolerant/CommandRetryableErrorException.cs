using System;

namespace Bornander.UI.Commands.Tolerant
{
    public class CommandRetryableErrorException : Exception
    {
        public CommandRetryableErrorException(string message)
            : this(message, null)
        {
        }

        public CommandRetryableErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
