using System;

namespace DriverBaseInterfaces
{
    /// <summary>
    ///  Represents errors that occur if the driver cannot be exdecuted for missing option in the license.
    /// </summary>
    public class LicenseOptionMissingException : Exception
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the DriverBaseInterfaces.LicenseOptionMissingException class.
        /// </summary>
        public LicenseOptionMissingException()
        { }

        /// <summary>
        /// Initializes a new instance of the DriverBaseInterfaces.LicenseOptionMissingException class 
        /// with a specified error message.
        /// </summary>
        public LicenseOptionMissingException(string message) 
            : base (message)
        { }

        /// <summary>
        /// Initializes a new instance of the DriverBaseInterfaces.LicenseOptionMissingException class 
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a null reference
        ///  (Nothing in Visual Basic) if no inner exception is specified.
        /// </param>
        public LicenseOptionMissingException(string message, Exception innerException) 
            : base(message, innerException)
        { }
        #endregion
    }
}
