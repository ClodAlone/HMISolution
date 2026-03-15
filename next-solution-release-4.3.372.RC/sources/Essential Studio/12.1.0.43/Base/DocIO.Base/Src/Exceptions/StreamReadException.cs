#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.Runtime.Serialization;
#endregion

namespace Syncfusion.DocIO
{
    /// <summary>
    /// Summary description for StreamReadError.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class StreamReadException : Exception
    {
        #region Class constants
        /// <summary>
        /// Default exception message.
        /// </summary>
        private const string DEF_MESSAGE = "Was unable to read sufficient bytes from the stream";
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal StreamReadException()
            : base(DEF_MESSAGE)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="innerExc"></param>
        internal StreamReadException(Exception innerExc)
            : this(DEF_MESSAGE, innerExc)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        internal StreamReadException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerExc"></param>
        internal StreamReadException(string message, Exception innerExc)
            : base(message, innerExc)
        {
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        ///
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        internal StreamReadException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

    }

    /// <summary>
    /// Summary description for StreamReadError.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class StreamWriteException : Exception
    {
        #region Class constants
        /// <summary>
        /// Default exception message.
        /// </summary>
        private const string DEF_MESSAGE = "Incorrect writes process";
        #endregion

        #region Class Initialize/Finalize methods
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal StreamWriteException()
            : base(DEF_MESSAGE)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="innerExc"></param>
        internal StreamWriteException(Exception innerExc)
            : this(DEF_MESSAGE, innerExc)
        {
        }
#endif

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        internal StreamWriteException(string message)
            : base(message)
        {
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerExc"></param>
        internal StreamWriteException(string message, Exception innerExc)
            : base(message, innerExc)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        internal StreamWriteException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion
    }

    /// <summary>
    /// Summary description for StreamReadError.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class FileCorruptException : Exception
    {
        #region Class constants
        /// <summary>
        /// Default exception message.
        /// </summary>
        private const string DEF_MESSAGE = "Document is corrupted and impossible to load";
        #endregion
#if !SILVERLIGHT && !WP
        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        public FileCorruptException()
            : base(DEF_MESSAGE)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCorruptException"/> class.
        /// </summary>
        /// <param name="innerExc">The inner exc.</param>
        public FileCorruptException(Exception innerExc)
            : this(DEF_MESSAGE, innerExc)
        {
        }

        /// <summary>
        /// Throws Exception when document appears to be corrupted and impossible.
        /// </summary>
        /// <param name="message"></param>
        public FileCorruptException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Throws Exception when document appears to be corrupted and impossible.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerExc"></param>
        public FileCorruptException(string message, Exception innerExc)
            : base(message, innerExc)
        {
        }

        /// <summary>
        /// Throws Exception when document appears to be corrupted and impossible.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public FileCorruptException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
#endif
    }
}