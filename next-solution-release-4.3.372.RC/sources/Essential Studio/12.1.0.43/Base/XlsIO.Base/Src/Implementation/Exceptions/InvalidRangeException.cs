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

using System;

namespace Syncfusion.XlsIO.Implementation.Exceptions
{
  /// <summary>
  /// Is thrown when range is invalid, i.e. when can't copy / move range into new location.
  /// </summary>
#if !SILVERLIGHT && !WINRT && !WP
  [ Serializable ]
#endif
  public class InvalidRangeException
    : ApplicationException
  {
    #region Class constants
    /// <summary>
    /// Default exception message.
    /// </summary>
    private const string DEF_MESSAGE = "Invalid range. ";
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the class with default error message.
    /// </summary>
    public InvalidRangeException()
      : base( DEF_MESSAGE )
    {
    }
    /// <summary>
    /// Initializes a new instance of the class with a specified error message.
    /// </summary>
    /// <param name="message">Error message.</param>
    public InvalidRangeException( string message )
      : base( DEF_MESSAGE + message )
    {
    }
    /// <summary>
    /// Initializes a new instance of the Exception class with
    /// a specified error message and a reference to the inner
    /// exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">
    /// The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception.
    /// If the innerException parameter is not a NULL reference
    /// (Nothing in Visual Basic), the current exception is raised
    /// in a catch block that handles the inner exception.
    /// </param>
    public InvalidRangeException( string message, Exception innerException )
      : base( DEF_MESSAGE + message, innerException )
    {
    }
    #endregion
  }
}
