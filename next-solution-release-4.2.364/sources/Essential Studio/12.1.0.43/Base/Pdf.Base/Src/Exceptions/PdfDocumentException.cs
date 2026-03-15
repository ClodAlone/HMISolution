#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

namespace Syncfusion.Pdf
{
    /// <summary>
    /// General exception class.
    /// </summary>
    public class PdfException : Exception
    {
        #region Constructors
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Initializes object by default error message.
        /// </summary>
        public PdfException()
            : base()
        {
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Initializes object by specified error message.
        /// </summary>
        /// <param name="message">User defined error message.</param>
        public PdfException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes object by specified error message and inner
        /// exception object.
        /// </summary>
        /// <param name="message">User defined error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public PdfException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }

    /// <summary>
    /// Base PDF document exception.
    /// </summary>
    public class PdfDocumentException : PdfException
    {
        #region Constants
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Default exception message.
        /// </summary>
        private const string ErrorMessage = @"Critical error on the document level.";
        #endregion

        #region Constructors
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Initializes object by default error message.
        /// </summary>
        public PdfDocumentException()
            : this(ErrorMessage)
        {
        }

        /// <summary>
        /// Initializes object by default error message and inner
        /// exception object.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public PdfDocumentException(Exception innerException)
            : this(ErrorMessage, innerException)
        {
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Initializes object by specified error message.
        /// </summary>
        /// <param name="message">User defined error message.</param>
        public PdfDocumentException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes object by specified error message and inner
        /// exception object.
        /// </summary>
        /// <param name="message">User defined error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public PdfDocumentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }
}
