#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO
{
  /// <summary>
  /// Event arguments for ReadOnlyFile event.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public class ReadOnlyFileEventArgs : EventArgs
  {
    #region Class members
    /// <summary>
    /// Indicates if file should be rewritten, default - False.
    /// </summary>
    private bool m_bRewrite;
    #endregion

    #region Class properties
    /// <summary>
    /// Indicates whether Read-only file should be rewritten.
    /// </summary>
    public bool ShouldRewrite
    {
      get
      {
        return m_bRewrite;
      }
      set
      {
        m_bRewrite = value;
      }
    }
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    public ReadOnlyFileEventArgs()
      : base()
    {
    }
    #endregion
  }

  /// <summary>
  /// Event arguments for PasswordRequired event.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public class PasswordRequiredEventArgs : EventArgs
  {
    #region Class members
    /// <summary>
    /// Indicates whether we should stop parsing protected workbook.
    /// </summary>
    private bool m_bStopParsing;
    /// <summary>
    /// New password to try.
    /// </summary>
    private string m_strNewPassword;
    
    #endregion

    #region Class properties
    /// <summary>
    /// Indicates whether we should stop parsing protected workbook.
    /// </summary>
    public bool StopParsing
    {
      get
      {
        return m_bStopParsing;
      }
      set
      {
        m_bStopParsing = value;
      }
    }
    /// <summary>
    /// New password to try.
    /// </summary>
    public string NewPassword
    {
      get
      {
        return m_strNewPassword;
      }
      set
      {
        m_strNewPassword = value;
      }
    }
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    public PasswordRequiredEventArgs()
      : base()
    {
    }
    #endregion
  }

  public class ConnectionPassword:EventArgs
  {
      private string m_connectionPassword;
      public string PasswordToConnectDB
      {
          get
          {
              return m_connectionPassword;
          }
          set
          {              
              m_connectionPassword=value;
          }
      }
  }

  /// <summary>
  /// Represents the method that will handle the ReadOnlyFile event.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public delegate void ReadOnlyFileEventHandler( object sender, ReadOnlyFileEventArgs e );
  /// <summary>
  /// Represents the method that will handle the PasswordRequired event.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public delegate void PasswordRequiredEventHandler( object sender, PasswordRequiredEventArgs e );
  /// <summary>
  /// Represents the method that will handle the PasswordRequired event.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public delegate void ConnectionPasswordEventHandler(object sender, ConnectionPassword e);
}
