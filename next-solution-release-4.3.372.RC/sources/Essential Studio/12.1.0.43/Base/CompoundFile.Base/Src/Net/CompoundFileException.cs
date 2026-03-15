#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Net
#else

#if (SILVERLIGHT || WP)
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

namespace Syncfusion.CompoundFile.XlsIO.Net
#endif
{
    /// <summary>
    /// This is exception thrown when experiencing problems with compound file.
    /// </summary>
#if !SILVERLIGHT && !WINRT && !WP
    [Serializable]
#endif
    class CompoundFileException : Exception
    {
        #region Constants
        /// <summary>
        /// Default exception message.
        /// </summary>
        private const string DefaultExceptionMessage = "Unable to parse compound file. Wrong file format.";
        #endregion

        #region Methods
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CompoundFileException()
            : base(DefaultExceptionMessage)
        {
        }
        /// <summary>
        /// Initializes new instance of the exception.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public CompoundFileException(string message)
            : base(message)
        {
        }
        #endregion
    }
}
