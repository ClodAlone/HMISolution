#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// This exception is thrown by CSS and HTML parsers when finding a problem
    /// in a document which cannot be resolved by the parser internally.
    /// </summary>
    [Serializable]
    public class ParseException : ApplicationException
    {
        #region Class constants
        /// <summary>
        /// Default text used by this exception when the message is not specified by user.
        /// </summary>
        private const string DEF_MESSAGE = "Parsing of document failed. Please check document structure and encoding.";
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Initializes a new instance of the ParseException class
        /// </summary>
        public ParseException()
            : base(DEF_MESSAGE)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ParseException class
        /// </summary>
        /// <param name="message">Message of exception.</param>
        public ParseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ParseException class
        /// </summary>
        /// <param name="inner">Inner exception.</param>
        public ParseException(Exception inner)
            : base(DEF_MESSAGE, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ParseException class
        /// </summary>
        /// <param name="message">Specified by user message.</param>
        /// <param name="inner">Exception on which this instance of exception class is based.</param>
        public ParseException(string message, Exception inner)
            : base(message, inner)
        {
        }
        #endregion
    }
}