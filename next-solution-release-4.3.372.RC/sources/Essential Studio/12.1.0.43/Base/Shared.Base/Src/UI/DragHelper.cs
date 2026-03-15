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
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Documentation;
#endregion

namespace Syncfusion.Windows.Forms
{
  /// <summary></summary>
  [ DocumentationExclude() ]
  public class DragHelper : IDisposable
  {
    #region Class members
    /// <summary></summary>
    internal DragDropEffects lastDragDropEffect = DragDropEffects.None;
    /// <summary></summary>
    internal DragWindow dragWindow = new DragWindow();
    /// <summary></summary>
    private bool m_bIsDragging = false;
    /// <summary>Cursor of the parent object.</summary>
    private Cursor m_parentCursor = Cursors.Default;
    #endregion

    #region Class properties
    /// <summary></summary>
    public DragWindow DragWindow
    {
      get
      {
        return dragWindow;
      }
    }

    /// <summary></summary>
    public DragDropEffects LastDragDropEffect
    {
      get
      {
        return this.lastDragDropEffect;
      }
    }

    /// <summary></summary>
    public bool IsDragging
    {
      get
      {
        return m_bIsDragging;
      }
    }
    
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary></summary>
    public DragHelper()
    {
    }
    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
      if( dragWindow != null && !dragWindow.IsDisposed )
      {
        dragWindow.Dispose();
        dragWindow = null;
      }
    }
    #endregion
    
    #region Class Public Methods
    /// <summary></summary>
    /// <param name="bmp"/>
    /// <param name="startPoint"/>
    /// <param name="effects"/>
    public void StartDrag( Bitmap bmp, Point startPoint, DragDropEffects effects )
    {
      this.StopDrag();
      m_bIsDragging = true;
      this.lastDragDropEffect = effects;

      this.DragWindow.DragBitmap = bmp;
      this.DragWindow.Invalidate();
      this.DragWindow.StartDrag( startPoint );
      this.CheckDragCursor( effects );
    }

    /// <summary></summary>
    /// <param name="p"/>
    /// <param name="e"/>
    public void DoDrag( Point p, DragDropEffects e )
    {
      if( !m_bIsDragging )
      {
        return;
      }

      this.lastDragDropEffect = e;
      this.CheckDragCursor( e );
      this.DragWindow.MoveTo( p );
      this.DragWindow.ShowWindowTopMost();
    }

    /// <summary></summary>
    public void CancelDrag()
    {
      if( !m_bIsDragging )
      {
        return;
      }

      this.StopDrag();
    }

    /// <summary></summary>
    public void EndDrag()
    {
      if( !m_bIsDragging )
      {
        return;
      }

      this.StopDrag();
    }

    #endregion
    
    #region Class utility methods
    /// <summary></summary>
    /// <param name="e"/>
    protected void CheckDragCursor( DragDropEffects e )
    {
      m_parentCursor = Cursor.Current;
      if( e == DragDropEffects.None )
      {
        Cursor.Current = Cursors.No;
      }
      else
      {
        Cursor.Current = m_parentCursor;
      }
    }

    /// <summary></summary>
    protected void StopDrag()
    {
      m_bIsDragging = false;
      this.lastDragDropEffect = DragDropEffects.None;
      this.dragWindow.StopDrag();
    }

    #endregion
  }
}