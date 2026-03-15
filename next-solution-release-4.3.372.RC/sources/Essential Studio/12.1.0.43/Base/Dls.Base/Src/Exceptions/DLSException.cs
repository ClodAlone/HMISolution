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

namespace Syncfusion.DLS
{
  /// <summary>
  /// Represents general exception in DLS library.
  /// </summary>
  [ Serializable ]
  [ Syncfusion.Documentation.DocumentationExclude() ]
  public class DLSException : ApplicationException
  {
    #region Class constants
    /// <summary>
    /// Default exception message.
    /// </summary>
    private const string DEF_MESSAGE = "Exception in DLS library";
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public DLSException()
      : base( DEF_MESSAGE )
    {}
    /// <summary>
    ///Initializing constructor.
    /// </summary>
    /// <param name="innerExc"></param>
    public DLSException( Exception innerExc )
      : this( DEF_MESSAGE, innerExc )
    {}
    /// <summary>
    ///Initializing constructor.
    /// </summary>
    /// <param name="message"></param>
    public DLSException( string message )
      : base( message )
    {}
    /// <summary>
    ///Initializing constructor.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerExc"></param>
    public DLSException( string message, Exception innerExc )
      : base( message, innerExc )
    {}
    #endregion
  }
}