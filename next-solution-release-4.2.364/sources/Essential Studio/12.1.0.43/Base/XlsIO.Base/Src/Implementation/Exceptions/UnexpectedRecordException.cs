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

using Syncfusion.XlsIO.Parser.Biff_Records;
#endregion

namespace Syncfusion.XlsIO.Implementation.Exceptions
{
  /// <summary>
  /// This exception should be thrown when unexpected record is met in the stream.
  /// </summary>
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
  [ Serializable ]
#endif
  public class UnexpectedRecordException : ApplicationException
  {
    #region Class constants
    /// <summary>
    /// Default message.
    /// </summary>
    private const string DEF_MESSAGE = "Unexpected record.";
    /// <summary>
    /// Message for exception message with record code.
    /// </summary>
    private const string DEF_MESSAGE_CODE = "Unexpected record {0}.";
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the class with default error message.
    /// </summary>
    public UnexpectedRecordException()
      : base( DEF_MESSAGE )
    {
    }
    /// <summary>
    /// Initializes a new instance of the class with default error message.
    /// </summary>
    /// <param name="recordCode">Record code that was met.</param>
    public UnexpectedRecordException( TBIFFRecord recordCode )
      : base( string.Format( DEF_MESSAGE_CODE, recordCode ) )
    {
    }
    /// <summary>
    /// Initializes a new instance of the class with a specified error message.
    /// </summary>
    /// <param name="message">Error message.</param>
    public UnexpectedRecordException( string message )
      : base( string.Format( DEF_MESSAGE_CODE, message ) )
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
    public UnexpectedRecordException( string message, Exception innerException )
      : base( message, innerException )
    {
    }
    #endregion
  }
}
