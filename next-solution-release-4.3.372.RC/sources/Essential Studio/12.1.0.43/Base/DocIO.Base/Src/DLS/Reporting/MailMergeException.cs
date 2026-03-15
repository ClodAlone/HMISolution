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
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for MailMergeException.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class MailMergeException : Exception
    {
        #region Class constants
        /// <summary>
        /// Default exception message.
        /// </summary>
        private const string DEF_MESSAGE = "Incorrect syntax of mail merge fields";
        #endregion

        #region Class Initialize/Finalize methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MailMergeException()
            : base(DEF_MESSAGE)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MailMergeException"/> class.
        /// </summary>
        /// <param name="innerExc">The inner exc.</param>
        public MailMergeException(Exception innerExc)
            : this(DEF_MESSAGE, innerExc)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MailMergeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MailMergeException(string message)
            : base(message)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="MailMergeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerExc">The inner exc.</param>
        public MailMergeException(string message, Exception innerExc)
            : base(message, innerExc)
        { }
        #endregion
    }
}