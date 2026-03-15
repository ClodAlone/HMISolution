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

namespace Syncfusion.XlsIO.Implementation.Exceptions
{
  /// <summary>
  /// This exception should be thrown when there are problems with formula parsing.
  /// </summary>
#if !SILVERLIGHT && !WINRT && !WP
  [ Serializable ]
#endif
  public class ParseException : ArgumentException
  {
    #region Class constants
    /// <summary>
    /// Default message format.
    /// </summary>
    private const string DEF_MESSAGE_FORMAT = "{0}. Formula: {1}, Position: {2}";
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    public ParseException() : base( "Can't parse formula." )
    {
    }
    /// <summary>
    /// Initializes a new instance of the class with a specified error message.
    /// </summary>
    /// <param name="message">Error message.</param>
    public ParseException( string message ) : base( message )
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
    public ParseException( string message, Exception innerException )
      : base( message, innerException )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="formula"></param>
    /// <param name="position"></param>
    /// <param name="innerException"></param>
    public ParseException( string message, string formula, int position, Exception innerException )
      : this( string.Format( DEF_MESSAGE_FORMAT, message, formula, position ), innerException )
    {
    }
    #endregion
  }
}
