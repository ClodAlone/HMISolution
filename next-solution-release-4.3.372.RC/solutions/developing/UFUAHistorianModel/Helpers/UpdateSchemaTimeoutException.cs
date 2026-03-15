using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UFUAHistorianModel
{
    public class UpdateSchemaTimeoutException : TimeoutException
    {
        #region Constructors
                // Summary:
        //     Initializes a new instance of the System.TimeoutException class.
        public UpdateSchemaTimeoutException()
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.TimeoutException class with the
        //     specified error message.
        //
        // Parameters:
        //   message:
        //     The message that describes the error.
        public UpdateSchemaTimeoutException(string message) : 
            base(message)
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.TimeoutException class with serialized
        //     data.
        //
        // Parameters:
        //   info:
        //     The System.Runtime.Serialization.SerializationInfo object that contains serialized
        //     object data about the exception being thrown.
        //
        //   context:
        //     The System.Runtime.Serialization.StreamingContext object that contains contextual
        //     information about the source or destination. The context parameter is reserved
        //     for future use, and can be specified as null.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The info parameter is null.
        //
        //   System.Runtime.Serialization.SerializationException:
        //     The class name is null, or System.Exception.HResult is zero (0).
        protected UpdateSchemaTimeoutException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {

        }
        //
        // Summary:
        //     Initializes a new instance of the System.TimeoutException class with the
        //     specified error message and inner exception.
        //
        // Parameters:
        //   message:
        //     The message that describes the error.
        //
        //   innerException:
        //     The exception that is the cause of the current exception. If the innerException
        //     parameter is not null, the current exception is raised in a catch block that
        //     handles the inner exception.
        public UpdateSchemaTimeoutException(string message, Exception innerException) :
            base(message, innerException)
        {

        }
        #endregion
    }
}
