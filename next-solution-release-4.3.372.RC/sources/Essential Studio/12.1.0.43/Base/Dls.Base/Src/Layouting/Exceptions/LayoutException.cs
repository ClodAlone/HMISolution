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
using System.Runtime.Serialization;
#endregion

namespace Syncfusion.Layouting.Exceptions
{
  /// <summary>
  /// Summary description for StreamReadError.
  /// </summary>
  [ Serializable ]
  public class LayoutException : ApplicationException
  {
    #region Class constants
    /// <summary>
    /// Default exception message.
    /// </summary>
    private const string DEF_MESSAGE = "Incorrect layouting process";
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public LayoutException()
      : base( DEF_MESSAGE )
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="innerExc"></param>
    public LayoutException( Exception innerExc )
      : this( DEF_MESSAGE, innerExc )
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="message"></param>
    public LayoutException( string message )
      : base( message )
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerExc"></param>
    public LayoutException( string message, Exception innerExc )
      : base( message, innerExc )
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    public LayoutException( SerializationInfo info, StreamingContext context )
      : base( info, context )
    {
    }
    #endregion
  }
  /// <summary>
  /// 
  /// </summary>
  public class InvalidLayoutStateException : LayoutException
  {
    #region Class constants
    /// <summary>
    /// Default exception message.
    /// </summary>
    private const string DEF_MESSAGE = "Invalid layout state";
    #endregion
    
    #region Class initialize/finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public InvalidLayoutStateException()
      : base( DEF_MESSAGE )
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="innerExc"></param>
    public InvalidLayoutStateException( Exception innerExc )
      : this( DEF_MESSAGE, innerExc )
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="message"></param>
    public InvalidLayoutStateException( string message )
      : base( message )
    {
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerExc"></param>
    public InvalidLayoutStateException( string message, Exception innerExc )
      : base( message, innerExc )
    {
    }

    #endregion
  }
}