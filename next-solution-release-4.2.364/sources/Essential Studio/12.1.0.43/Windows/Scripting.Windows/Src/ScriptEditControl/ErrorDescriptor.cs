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
using System.Diagnostics;
#endregion

namespace Syncfusion.Scripting.Design
{
  /// <summary>
  /// Represents item for error listBox
  /// </summary>
  public class ErrorDescriptor
  {
    #region Class members
    /// <summary>
    /// Line number where occuring error
    /// </summary>
    private int m_line;

    /// <summary>
    /// Column where occuring error
    /// </summary>
    private int m_col;

    /// <summary>
    /// The error message
    /// </summary>
    private string m_errMessage;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Disabled default constructor.
    /// </summary>
    private ErrorDescriptor()
    {
    }

    /// <summary>
    /// Initialized class properties.
    /// </summary>
    /// <param name="errMessage"></param>
    /// <param name="line"></param>
    /// <param name="pos"></param>
    public ErrorDescriptor( string errMessage, int line, int col )
    {
      if( errMessage == null )
      {
        throw new ArgumentNullException( "errMessage" );
      }

      if( errMessage.Length == 0 )
      {
        throw new ArgumentException( "errMessage - string can not be empty" );
      }
      if( line < 0 )
      {
        throw new ArgumentException( "line" );
      }
      if( col < 0 )
      {
        throw new ArgumentException( "col" );
      }

      m_errMessage = errMessage;
      m_line = line;
      m_col = col;
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Error Message
    /// </summary>
    public string Message
    {
      get
      {
        return m_errMessage;
      }
    }

    /// <summary>
    /// Error line
    /// </summary>
    public int Line
    {
      get
      {
        return m_line;
      }
    }

    /// <summary>
    /// Error colum number
    /// </summary>
    public int Column
    {
      get
      {
        return m_col;
      }
    }
    #endregion

    #region Class overrides
    public override string ToString()
    {
      return m_errMessage;
    }
    #endregion
  }
}