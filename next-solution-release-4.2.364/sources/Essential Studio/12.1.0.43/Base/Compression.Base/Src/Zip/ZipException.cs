#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;

namespace Syncfusion.Compression.Zip
{
    /// <summary>
    /// This class represents exception type that is mostly raised when some
    /// problems with zip extraction/creation occurs.
    /// </summary>
    public class ZipException : ApplicationException
    {
        #region Constructors
        /// <summary>
        /// Initializes new instance of the exception class.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public ZipException( string message )
            : base( "Zip exception." + message )
        {
        }
        #endregion
    }
}
