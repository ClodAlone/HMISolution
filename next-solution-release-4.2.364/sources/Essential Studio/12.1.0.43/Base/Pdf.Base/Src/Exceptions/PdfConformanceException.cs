#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT && !NETFX_CORE && !WP
using System;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// Exception of this type is raised when the document contains object which are not 
    /// supported by current document standard.
    /// </summary>
    [Serializable]
    public class PdfConformanceException : PdfDocumentException
    {
        #region Constants
        /// <summary>
        /// Default exception message.
        /// </summary>
        private const string ErrorMessage = @"PDF Conformance-level exception.";
        #endregion

        #region Class Constructors
        /// <summary>
        /// Initializes object with default error message.
        /// </summary>
        public PdfConformanceException()
            : this(ErrorMessage)
        {
        }

        /// <summary>
        /// Initializes object with default error message and inner
        /// exception object.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public PdfConformanceException(Exception innerException)
            : this(ErrorMessage, innerException)
        {
        }

        /// <summary>
        /// Initializes object by specified error message.
        /// </summary>
        /// <param name="message">User defined error message.</param>
        public PdfConformanceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes object with specified error message and inner
        /// exception object.
        /// </summary>
        /// <param name="message">User defined error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public PdfConformanceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion
    }
}
#endif
