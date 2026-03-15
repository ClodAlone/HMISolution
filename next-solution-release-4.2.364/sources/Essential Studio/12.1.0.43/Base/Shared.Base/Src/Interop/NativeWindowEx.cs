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
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.Interop
{
  /// <summary>Subclass window that allowing attaching of
  /// message filters classes into Message processing.</summary>
  public class NativeWindowEx : NativeWindow
  {
    #region Class constants
    /// <summary>Value that represent invalid window handle.</summary>
    public static readonly IntPtr InvalidHandle = new IntPtr( -1 );
    #endregion

    #region Fields
    /// <summary>Reference on message filter instance.</summary>
    private IMessageFilter m_messageFilter;
    #endregion

    #region Properties
    /// <summary>Gets and Sets reference on message filter instance.</summary>
    public IMessageFilter MessageFilter
    {
      get
      {
        return m_messageFilter;
      }
      set
      {
        m_messageFilter = value;
      }
    }
    #endregion
    
    #region Constructors
    /// <summary>Default constructor that subclass window by it handle automatically.</summary>
    /// <param name="hWnd">Window Handle.</param>
    public NativeWindowEx( IntPtr hWnd )
    {
      Debug.Assert( InvalidHandle != hWnd, "Class initialized by invalid window handle. Please check." );

      AssignHandle( hWnd );
    }
    #endregion

    #region Overrides
    /// <summary>Override of WndProc function.</summary>
    /// <param name="m">Reference on message processed by Window.</param>
    protected override void WndProc( ref Message m )
    {
      if( m_messageFilter != null && m_messageFilter.PreFilterMessage( ref m ) )
      {
        return;
      }
      
      base.WndProc( ref m );
    }
    #endregion
  }
}