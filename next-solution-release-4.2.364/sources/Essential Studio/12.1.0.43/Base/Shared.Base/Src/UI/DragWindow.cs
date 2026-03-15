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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;

using Syncfusion.Documentation;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms
{
  [
  ToolboxItem( false ),
  DocumentationExclude()
  ]
  public class DragWindow : Form
  {
    // Fields
    private Bitmap dragBitmap = null;
    private bool isDragging = false;
    private Point origin = new Point( -30000, -30000 );
    private Cursor overrideCursor = null;
    /// <summary>
    /// Parent control supporting Drag operation.
    /// </summary>
    private IDragDispatcher m_parent;
    /// <summary>
    /// Indicates whether shift value is calculated.
    /// </summary>
    private bool bShiftCalculated;
    private Point keepOrigin = Point.Empty;

    internal const int SWP_NOSIZE = 1; // 0x0001 
    internal const int SWP_NOMOVE = 2; // 0x0002 
    internal const int SWP_NOZORDER = 4; // 0x0004 
    internal const int SWP_NOACTIVATE = 16; // 0x0010 
    internal const int SWP_SHOWWINDOW = 64; // 0x0040 
    internal const int SWP_HIDEWINDOW = 128; // 0x0080 
    internal const int SWP_DRAWFRAME = 32; // 0x0020 

    // Properties
    public Bitmap DragBitmap
    {
      get
      {
        return this.dragBitmap;
      }
      set
      {
        this.BackgroundImage = value;
        if( value == null )
        {
          this.StopDrag();
        }
        else
        {
          Size size = value.Size;
          this.origin = new Point( ( size.Width / 2 ), ( size.Height / 2 ) );

          // Whidbey added a call to SetWindowPos in its Form.MinimumSize property setter. When this
          // method is called it uses flags that change the z-order. This is not wanted for
          // this DragWindow control.
          //
          // The problem can be avoided by destroying the window handle before setting the property.
          // It will be recreated later automatically with correct z-order.
          if( Environment.Version.Major >= 2 )
          {
            this.DestroyHandle();
          }
          this.MinimumSize = value.Size;
          this.Size = value.Size;
        }

        this.dragBitmap = value;
      }
    }

    /// <override/>
    protected override CreateParams CreateParams
    {
      [ SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode=true ) ]
      get
      {
        CreateParams cp;
        cp = base.CreateParams;
        cp.ExStyle = ( cp.ExStyle | 0x80 /*WS_EX_TOOLWINDOW*/ );
        cp.Style = ( int )( cp.Style | -2147483648 /*0x80000000,WS_POPUP*/ );
        return cp;
      }
    }

    public Cursor WindowCursor
    {
      get
      {
        return base.Cursor;
      }
      set
      {
        if( this.overrideCursor != value )
        {
          this.overrideCursor = value;

          if( !this.DesignMode && IsHandleCreated )
          {
            NativeMethods.SendMessage( Handle, NativeMethods.WM_SETCURSOR, Handle, NativeMethods.HTCLIENT );
          }
        }
      }
    }

    
    // Constructors
    public DragWindow()
    {
      this.SetStyle( ControlStyles.SupportsTransparentBackColor, true );
      this.InitializeComponent();
    }

    // Methods
    public void ShowWindowTopMost()
    {
      // Do not check for IsHandleCreated! The call to this.Handle
      // will automatically create the Handle and grid code relies on this behavior.

      //if( this.IsHandleCreated )
      {
        NativeMethods.SetWindowPos( this.Handle, ( IntPtr )NativeMethods.HWND_TOPMOST, 
          0, 0, 0, 0, 
          SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE | SWP_SHOWWINDOW );
      }
    }

    private void InitializeComponent()
    {
      this.dragBitmap = null;
      this.Size = new Size( 0, 0 );
      this.MinimumSize = new Size( 0, 0 );
      this.Location = new Point( -10000, -10000 );
      this.StartPosition = FormStartPosition.Manual;
      this.FormBorderStyle = FormBorderStyle.None;
      this.Enabled = false;
      this.ShowInTaskbar = false;
      this.TopLevel = true; // Must be TopLevel to allow transparency
      //this.TopMost = true;
      //this.BringToFront();
      this.BackColor = Color.Red;
      //this.AllowTransparency = true;
      this.TransparencyKey = Color.Red;
      this.Opacity = 0.69f;
      ShowWindowTopMost();
    }

    /// <override/>
    [ SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode=true ) ]
    protected override void WndProc( ref Message msg )
    {
      switch( msg.Msg )
      {
        case NativeMethods.WM_SETCURSOR:
          WmSetCursor( ref msg );
          break;

        default:
          try
          {
            base.WndProc( ref msg );
          }
          catch( Win32Exception ex )
          {
            Debug.WriteLine( ex.Message + Environment.NewLine + ex.StackTrace, "Handled Exception" );
            //TraceUtil.TraceExceptionCatched(ex);
            // ignore the Win32Exception - this occurs when grid is used
            // OLE Inplace in Office applications ...
          }
          break;
      }
    }

    protected override void OnPaintBackground( PaintEventArgs pevent )
    {
      pevent.Graphics.FillRectangle( new SolidBrush( this.TransparencyKey ), pevent.ClipRectangle );
      base.OnPaintBackground( pevent );
    }

    /// <summary>
    ///     Handles the WM_SETCURSOR message.
    /// </summary>
    /// <internalonly/>
    private void WmSetCursor( ref Message m )
    {
      // Accessing through the Handle property has side effects that break this
      // logic. You must use InternalHandle.
      //
      if( m.WParam == this.Handle && ( ( int )m.LParam & 0x0000FFFF ) == NativeMethods.HTCLIENT )
      {
        OnSetCursor( ref m );
      }
      else
      {
        DefWndProc( ref m );
      }
    }

    /// <internalonly/>
    [ DocumentationExclude() ]
    protected virtual void OnSetCursor( ref Message m )
    {
      if( this.overrideCursor != null )
      {
        Cursor.Current = this.overrideCursor;
      }
      else
      {
        Cursor.Current = this.Cursor;
      }
    }

    private void _Move( Point p )
    {
      Point orgn = ( bShiftCalculated ) ? keepOrigin : origin;
      p.Offset( ( -( orgn ).X ), ( -( orgn ).Y ) );
      
      this.Location = p;
      if( this.BackgroundImage != null )
      {
        this.Size = this.BackgroundImage.Size;
      }
      else
      {
        this.Size = this.MinimumSize;
      }
    }

    public bool StartDrag( Point p )
    {
      if( this.BackgroundImage == null )
      {
        return false;
      }

      // force form handle creaation if it's not done yet
      if( !this.IsHandleCreated ) this.CreateHandle();

      this.isDragging = true;
      this._Move( p );
      ShowWindowTopMost();
      return true;
    }

    public bool MoveTo( Point p )
    {
      if( !this.isDragging )
      {
        return false;
      }

      this._Move( p );
      return true;
    }

    public bool StopDrag()
    {
      if( !this.isDragging )
      {
        return false;
      }

      this.BackgroundImage = null;
      this.isDragging = false;
      bShiftCalculated = false;
      
      SuspendLayout();
      this.Visible = false;
      ResumeLayout();
      
      // free handle resources when drag window no more needed to us
      if( this.IsHandleCreated ) this.DestroyHandle();

      return true;
    }

    public bool SetOrigin( Point pt )
    {
      bool originSet = false;

      if( !bShiftCalculated )
      {
        keepOrigin = pt;
        bShiftCalculated = true;
        originSet = true;
      }

      return originSet;
    }

    
    #region Drag methods
    /// <summary>
    /// Gets / sets the parent control supporting drag operation.
    /// </summary>
    public IDragDispatcher DragParent
    {
      get
      {
        return m_parent;
      }
      set
      {
        if( m_parent != value )
        {
          m_parent = value;
          this.AllowDrop = ( value != null );
        }
      }
    }

    protected override void OnDragOver( DragEventArgs drgevent )
    {
      if( DragParent != null )
      {
        DragParent.DispatchDragOver( drgevent );
      }
    }

    protected override void OnDragDrop( DragEventArgs drgevent )
    {
      if( DragParent != null )
      {
        DragParent.DispatchOnDragDrop( drgevent );
      }
    }

    protected override void OnDragEnter( DragEventArgs drgevent )
    {
      if( DragParent != null )
      {
        DragParent.DispatchOnDragEnter( drgevent );
      }
    }

    protected override void OnDragLeave( EventArgs e )
    {
      if( DragParent != null )
      {
        DragParent.DispatchOnDragLeave( e );
      }
    }

    protected override void OnQueryContinueDrag( QueryContinueDragEventArgs qcdevent )
    {
      if( DragParent != null )
      {
        DragParent.DispatchOnQueryContinueDrag( qcdevent );
      }
    }

    protected override void OnGiveFeedback( GiveFeedbackEventArgs gfbevent )
    {
      if( DragParent != null )
      {
        DragParent.DispatchOnGiveFeedback( gfbevent );
      }
    }

    public new DragDropEffects DoDragDrop( object data, DragDropEffects allowedEffects )
    {
      DragDropEffects result = DragDropEffects.None;

      if( DragParent != null )
      {
        result = DragParent.DispatchDoDragDrop( data, allowedEffects );
      }

      return result;
    }
    #endregion ;
  }

  /// <summary>
  /// Interface for implementing by parent control supporting Drag operation.
  /// Methods of this interface invoke corresponding drag methods of parent control.
  /// </summary>
  public interface IDragDispatcher
  {
    void DispatchDragOver( DragEventArgs drgevent );

    void DispatchOnDragDrop( DragEventArgs drgevent );

    void DispatchOnDragEnter( DragEventArgs drgevent );

    void DispatchOnDragLeave( EventArgs e );

    void DispatchOnQueryContinueDrag( QueryContinueDragEventArgs args );

    void DispatchOnGiveFeedback( GiveFeedbackEventArgs args );

    DragDropEffects DispatchDoDragDrop( object data, DragDropEffects allowedEffects );
  }
}