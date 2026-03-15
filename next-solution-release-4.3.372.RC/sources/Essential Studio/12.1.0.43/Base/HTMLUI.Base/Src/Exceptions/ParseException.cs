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

namespace Syncfusion.HTMLUI.Base
{
	/// <summary>
	/// This exception is thrown by CSS and HTML parsers when finding a problem
	/// in a document which cannot be resolved by the parser internally.
	/// </summary>
  [ Serializable ]
  public class ParseException : ApplicationException
	{
    #region Class constants
    /// <summary>
    /// Default text used by this exception when message is not specified by user.
    /// </summary>
    private const string DEF_MESSAGE = "Parsing of document failed. Please check document structure and encoding.";
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Overloaded constructor.
    /// </summary>
    public ParseException()
      : base( DEF_MESSAGE )
    {
    }
    /// <summary>
    /// Constructor with a user specified exception message.
    /// </summary>
    /// <param name="message">Message to be displayed on throwing an exception.</param>
    public ParseException( string message )
      : base( message )
    {
    }
    /// <summary>
    /// Create exception based on other exception - Wrap inner exception.
    /// </summary>
    /// <param name="inner">Inner exception.</param>
    public ParseException( Exception inner )
      : base( DEF_MESSAGE, inner )
    {
    }
    /// <summary>
    /// Create exception class with a user specified message and inner exception.
    /// </summary>
    /// <param name="message">User specified message.</param>
    /// <param name="inner">Exception on which this instance of exception class is based.</param>
    public ParseException( string message, Exception inner )
      : base( message, inner )
    {
    }
    #endregion
  }
}