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

#if !SILVERLIGHT

#region file using directives
using System;
using System.Runtime.Serialization;
#endregion

namespace Syncfusion.Layouting.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a layouting error occurs.
    /// </summary>
    [Serializable]
    internal class LayoutException : ApplicationException
    {
        #region Constants
        /// <summary>
        /// Default exception message.
        /// </summary>
        private const string DEF_MESSAGE = "Incorrect layouting process";
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutException"/> class.
        /// </summary>
        public LayoutException()
            : base(DEF_MESSAGE)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutException"/> class.
        /// </summary>
        /// <param name="innerExc">The inner exc.</param>
        public LayoutException(Exception innerExc)
            : this(DEF_MESSAGE, innerExc)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public LayoutException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerExc">The inner exc.</param>
        public LayoutException(string message, Exception innerExc)
            : base(message, innerExc)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        public LayoutException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
    }

    /// <summary>
    /// The exception that is thrown when a layouting error occurs.
    /// </summary>
    internal class InvalidLayoutStateException : LayoutException
    {
        #region Constants
        /// <summary>
        /// Default exception message.
        /// </summary>
        private const string DEF_MESSAGE = "Fatal error";
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        public InvalidLayoutStateException()
            : base(DEF_MESSAGE)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidLayoutStateException"/> class.
        /// </summary>
        /// <param name="innerExc">The inner exc.</param>
        public InvalidLayoutStateException(Exception innerExc)
            : this(DEF_MESSAGE, innerExc)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidLayoutStateException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidLayoutStateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidLayoutStateException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerExc">The inner exc.</param>
        public InvalidLayoutStateException(string message, Exception innerExc)
            : base(message, innerExc)
        {
        }
        #endregion
    }
}

#endif
