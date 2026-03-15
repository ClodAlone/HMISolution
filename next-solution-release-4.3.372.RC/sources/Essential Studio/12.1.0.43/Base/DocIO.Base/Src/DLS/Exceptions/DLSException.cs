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
  /// Represents general exception in DLS library.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public class DLSException : Exception
  {
    #region Class constants
    /// <summary>
    /// Default exception message.
    /// </summary>
    private const string DEF_MESSAGE = "Exception in DLS library";
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes a new instance of the <see cref="DLSException"/> class.
    /// </summary>
    public DLSException()
      : base( DEF_MESSAGE )
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="DLSException"/> class.
    /// </summary>
    /// <param name="innerExc">The inner exc.</param>
    public DLSException( Exception innerExc )
      : this( DEF_MESSAGE, innerExc )
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="DLSException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public DLSException( string message )
      : base( message )
    { }
    /// <summary>
    /// Initializes a new instance of the <see cref="DLSException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="innerExc">The inner exc.</param>
    public DLSException( string message, Exception innerExc )
      : base( message, innerExc )
    { }
    #endregion
  }
}
