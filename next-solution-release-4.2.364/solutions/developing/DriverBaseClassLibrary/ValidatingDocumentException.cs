using System;

namespace DriverBaseInterfaces
{
    /// <summary>
    ///  Represents errors that occur if the driver cannot be exdecuted for missing option in the license.
    /// </summary>
    public class ValidatingDocumentException : Exception
    {
        #region Declaratrions
        readonly string filePath;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the DriverBaseInterfaces.LicenseOptionMissingException class.
        /// </summary>
        public ValidatingDocumentException(string filePath)
        {
            this.filePath = filePath;
        }

        /// <summary>
        /// Initializes a new instance of the DriverBaseInterfaces.LicenseOptionMissingException class 
        /// with a specified error message.
        /// </summary>
        public ValidatingDocumentException(string filePath, string message) 
            : base (message)
        {
            this.filePath = filePath;
        }

        /// <summary>
        /// Initializes a new instance of the DriverBaseInterfaces.LicenseOptionMissingException class 
        /// with a specified error message and a reference to the inner exception that is the cause of this exception.
        public ValidatingDocumentException(string filePath, string message, Exception innerException) 
            : base(message, innerException)
        {
            this.filePath = filePath;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Full path of the file who has not been validated.
        /// </summary>
        public string FilePath
        {
            get
            {
                return filePath;
            }
        }
        #endregion
    }
}
