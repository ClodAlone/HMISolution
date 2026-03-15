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
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Tools
{
	internal class GroupBarItemPopup:
		ScrollableControl,
		IMessageFilter
	{
		#region Constants

		private const int VK_ESCAPE = 0x1B;
		private const int SWP_SHOWPOPUP = NativeMethods.SWP_SHOWWINDOW | NativeMethods.SWP_NOACTIVATE;
		private const int SWP_HIDEPOPUP = NativeMethods.SWP_HIDEWINDOW | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOZORDER |
			NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE;
		private const int SPI_GETDROPSHADOW = 0x1024;
		private const int CS_DROPSHADOW = 0x20000;
		private const uint RDW_FLAGS = NativeMethods.RDW_FRAME | NativeMethods.RDW_ALLCHILDREN | NativeMethods.RDW_INVALIDATE
			| NativeMethods.RDW_INTERNALPAINT | NativeMethods.RDW_UPDATENOW;

		public const int BorderWidth = 5;
		public const int GripperHeight = 14;

		private const int c_nGripperLinesOffset = 3;

		#endregion

		#region Fields

		/// <summary>
		/// Owner control of the popup.
		/// </summary>
		private GroupBar m_owner;

		/// <summary>
		/// CallWnd hook.
		/// </summary>
		private WndProcHooker m_hook;

		/// <summary>
		/// Popup visibility.
		/// </summary>
		private bool m_bOpened = false;

		/// <summary>
		/// Indicates whether popup is shown to the left of <see cref="GroupBar"/>.
		/// </summary>
		private bool m_bFlippedX = false;

		/// <summary>
		/// Indicates whether popup is shown to the top of <see cref="GroupBar"/> 
		/// </summary>
		private bool m_bFlippedY = false;

		#endregion

		#region Constructors

		public GroupBarItemPopup( GroupBar owner )
		{
			Debug.Assert( null != owner );

			m_owner = owner;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a value indicating whether the popup is displayed.
		/// </summary>
		/// <returns>true if the popup is displayed; otherwise, false. The default is false.</returns>
		public bool Opened
		{
			get
			{
				return m_bOpened;
			}
		}

		internal int BottomBorderHeight
		{
			get
			{
				return m_owner.ShowPopupGripper ? GripperHeight : BorderWidth;
			}
		}

		#endregion

		#region Events
		
		public event EventHandler OpenedChanged;

		public event MouseClickCancelEventHandler BeforeClose;

		#endregion

		#region Overrides

		/// <summary>
		/// Gets the required creation parameters when the control handle is created.
		/// </summary>
		/// <value></value>
		/// <returns>A <see cref="T:System.Windows.Forms.CreateParams"/> that contains the required creation parameters when the handle to the control is created.</returns>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				bool bDropShadowEnabled = false;
								
				NativeMethods.SystemParametersInfo( SPI_GETDROPSHADOW, 0, ref bDropShadowEnabled, 0 );

				if( bDropShadowEnabled )
				{
					cp.ClassStyle |= CS_DROPSHADOW;
				}

				cp.Style |= unchecked( (int)(NativeMethods.WS_POPUP) );

				return cp;
			}
		}

		protected void OnWmCreated()
		{
			m_hook = new WndProcHooker( this.Handle, this );
			m_hook.HookMessages = true;

			Application.AddMessageFilter( this );
		}

		protected void OnWmDestroyed()
		{
			if( null != m_hook )
			{
				m_hook.HookMessages = false;
				m_hook.Dispose();
				m_hook = null;
			}

			Application.RemoveMessageFilter( this );
		}

		/// <summary>
		/// Processes Windows messages.
		/// </summary>
		/// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message"/> to process.</param>
		protected override void WndProc( ref Message m )
		{
			switch( m.Msg )
			{
				case NativeMethods.WM_ACTIVATE:
					OmWmActivate( ref m );
					return;

				case NativeMethods.WM_MOUSEACTIVATE:
					m.Result = (IntPtr)NativeMethods.MA_NOACTIVATE;
					return;

				case NativeMethods.WM_NCCALCSIZE:
					OnWmNcCalcSize( ref m );
					break;

				case NativeMethods.WM_NCHITTEST:
					if( OnWmNcHitTest( ref m ) ) return;
					break;

				case NativeMethods.WM_NCPAINT:
					OnWmNcPaint( ref m );
					return;

				case NativeMethods.WM_CREATE:
				{
					base.WndProc( ref m );

					if( m.Result == IntPtr.Zero )
					{
						OnWmCreated();
					}

					return;
				}

				case NativeMethods.WM_DESTROY:
					OnWmDestroyed();
					break;

			}

			base.WndProc( ref m );
		}

		#endregion

		#region Methods

		/// <summary>
		/// Shows the popup.
		/// </summary>
		/// <param name="bShow">if set to <c>true</c> shows the popup; hides overwise.</param>
		/// <param name="args">The <see cref="GroupBar.BeforePopupEventArgs"/> instance containing the event data.</param>
		public void Show( bool bShow, GroupBar.BeforePopupEventArgs args )
		{
			if( bShow )
			{
				Debug.Assert( null != args );
				Debug.Assert( null != args.Item );
				Debug.Assert( !args.Cancel );

				m_bFlippedX = args.FlippedX;
				m_bFlippedY = args.FlippedY;

				GroupBarItem selItem = args.Item;

				if( selItem != null )
				{
					this.SuspendLayout();

					this.RightToLeft = m_owner.RightToLeft;

					if( !this.IsHandleCreated )
					{
						CreateHandle();
					}

					Rectangle rcPopupBounds = args.PopupBounds;

					NativeMethods.SetWindowPos( this.Handle, (IntPtr)NativeMethods.HWND_TOPMOST,
						rcPopupBounds.X, rcPopupBounds.Y, rcPopupBounds.Width, rcPopupBounds.Height, SWP_SHOWPOPUP );

					m_bOpened = true;
					OnOpenedChaged();

					this.ResumeLayout( true );
				}
			}
			else
			{
				Hide();
			}
		}

		/// <summary>
		/// Hides the popup.
		/// </summary>
		public new void Hide()
		{
			Hide( Point.Empty );
		}

		/// <summary>
		/// Hides the popup.
		/// </summary>
		/// <param name="pt">Allows to determine whether to clsoe popup in <see cref="BeforeClose"/> event handler.</param>
		public void Hide( Point pt )
		{
			MouseClickCancelEventArgs args = OnBeforeClose( pt );

			if( !args.Cancel )
			{
				NativeMethods.SetWindowPos( this.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP_HIDEPOPUP );

				m_bOpened = false;
				OnOpenedChaged();
			}
		}

		#endregion

		#region Implementation

		private void OnOpenedChaged()
		{
			if( null != this.OpenedChanged )
			{
				this.OpenedChanged( this, EventArgs.Empty );
			}
		}

		private MouseClickCancelEventArgs OnBeforeClose( Point pt )
		{
			MouseClickCancelEventArgs args = new MouseClickCancelEventArgs( pt, false );

			if( null != this.BeforeClose )
			{
				this.BeforeClose( this, args );
			}

			return args;
		}

		private int GetHitTest( int x, int y )
		{
			int nResult = NativeMethods.HTERROR;

			NativeMethods.RECT rc = new NativeMethods.RECT();
			NativeMethods.GetWindowRect( (int)this.Handle, ref rc );

			if( NativeMethods.PtInRect( ref rc, new NativeMethods.POINT( x, y ) ) )
			{
				// Offset from top/left
				int x1 = x - rc.left;
				int y1 = y - rc.top;

				// Offset from bottom/right
				int x2 = rc.Width - x1;
				int y2 = rc.Height - y1;

				if( m_bFlippedX )
				{
					x1 ^= x2;
					x2 ^= x1;
					x1 ^= x2;
				}

				if( m_bFlippedY )
				{
					y1 ^= y2;
					y2 ^= y1;
					y1 ^= y2;
				}

				if( x2 < this.BottomBorderHeight && y2 < this.BottomBorderHeight )
				{
					nResult = GetBottomRightHitTest();
				}
				else if( x2 < BorderWidth )
				{
					if( y2 < this.BottomBorderHeight )
					{
						nResult = GetBottomRightHitTest();
					}
					else
					{
						if( (m_owner.PopupResizeMode & PopupResizeMode.Horizontal) != 0 )
						{
							nResult = GetHitTestRight();
						}
					}
				}
				else if( y2 < this.BottomBorderHeight )
				{
					if( (m_owner.PopupResizeMode & PopupResizeMode.Vertical) != 0 )
					{
						nResult = GetHitTestBottom();
					}
				}
				else
				{
					if( y1 >= BorderWidth )
					{
						nResult = NativeMethods.HTCLIENT;
					}
				}
			}

			return nResult;
		}

		private int GetHitTestRight()
		{
			return m_bFlippedX ? NativeMethods.HTLEFT : NativeMethods.HTRIGHT;
		}

		private int GetHitTestBottom()
		{
			return m_bFlippedY ? NativeMethods.HTTOP : NativeMethods.HTBOTTOM;
		}

		private int GetBottomRightHitTest()
		{
			int nResult = NativeMethods.HTERROR;

			switch( m_owner.PopupResizeMode )
			{
				case PopupResizeMode.Horizontal:
					nResult = GetHitTestRight();
					break;
				case PopupResizeMode.Vertical:
					nResult = GetHitTestBottom();
					break;
				case PopupResizeMode.Both:
					if( m_bFlippedX )
					{
						if( m_bFlippedY )
						{
							nResult = NativeMethods.HTTOPLEFT;
						}
						else
						{
							nResult = NativeMethods.HTBOTTOMLEFT;
						}
					}
					else
					{
						if( m_bFlippedY )
						{
							nResult = NativeMethods.HTTOPRIGHT;
						}
						else
						{
							nResult = NativeMethods.HTBOTTOMRIGHT;
						}						
					}
					
					break;
			}

			return nResult;
		}

		private void DrawNcArea( Graphics g, Rectangle rect )
		{
			Rectangle innerRect = rect;

			innerRect.Inflate( -BorderWidth, -BorderWidth );

			if( m_owner.ShowPopupGripper )
			{
				innerRect.Height -= GripperHeight - BorderWidth;
			}

			using( Region r = new Region( rect ) )
			using( Brush brush = new SolidBrush( m_owner.ClientAreaBackground ) )
			{
				r.Exclude( innerRect );
				g.FillRegion( brush, r );
			}		

			using( Pen pen = new Pen( m_owner.BorderColor ) )
			{
				if( m_owner.ShowPopupGripper )
				{
					rect = DrawGripper( g, rect, pen );
				}

				rect.Height--; rect.Width--;

				g.DrawRectangle( pen, rect );

				innerRect.X--; innerRect.Y--;
				innerRect.Width++; innerRect.Height++;

				g.DrawRectangle( pen, innerRect );
			}
		}

		private Rectangle DrawGripper( Graphics g, Rectangle rect, Pen pen )
		{
			Rectangle gripperRect = new Rectangle( new Point( 0, rect.Height - GripperHeight ), new Size( rect.Width, GripperHeight ) );
            bool bDrawMirrored = false;
            if (RightToLeft.Yes == this.RightToLeft)
            {
                if (!m_owner.PopupRightToLeft)
                    bDrawMirrored = true;
                else
                    bDrawMirrored = false;
            }
            else
            {
                if (!m_owner.PopupRightToLeft)
                    bDrawMirrored = false;
                else
                    bDrawMirrored = true;
            }
            using (CMirroredDrawer md = new CMirroredDrawer(g, gripperRect, bDrawMirrored))
			{
				Point offset = new Point( rect.Width - GripperHeight, rect.Height );
				Point start = new Point( 3, -3 );
				Point start2 = start;
				Point end = new Point( GripperHeight - 3, 3 - GripperHeight );
				Point end2 = end;

				start2.X++;
				end2.Y++;

				start.Offset( offset ); end.Offset( offset );
				start2.Offset( offset ); end2.Offset( offset );

				for( int i = 0; i < 3; ++i )
				{
					g.DrawLine( pen, start, end );
					g.DrawLine( pen, start2, end2 );

					start.X += c_nGripperLinesOffset; end.Y += c_nGripperLinesOffset;
					start2.X += c_nGripperLinesOffset; end2.Y += c_nGripperLinesOffset;
				}
			}
			return rect;
		}

		#endregion

		#region IMessageFilter Members

		bool IMessageFilter.PreFilterMessage( ref Message m )
		{
			switch( m.Msg )
			{
				case NativeMethods.WM_NCLBUTTONDOWN:
				case NativeMethods.WM_NCRBUTTONDOWN:
				case NativeMethods.WM_NCMBUTTONDOWN:
					OnXButtonDown( m, false );
					break;

				case NativeMethods.WM_LBUTTONDOWN:
				case NativeMethods.WM_RBUTTONDOWN:
				case NativeMethods.WM_MBUTTONDOWN:
					OnXButtonDown( m, true );
					break;

				case NativeMethods.WM_KEYDOWN:
					OnKeyDown( m );
					break;

				case NativeMethods.WM_ACTIVATE:
					OnActivate( m );
					break;
			}

			return false;
		}

		#endregion

		#region Message handlers

		private void OnXButtonDown( Message m, bool bClient )
		{
			if( this.Opened )
			{
				if( m.HWnd != this.Handle )
				{
					if( !NativeMethods.IsChild( this.Handle, m.HWnd ) )
					{
						int x = NativeMethods.LOWORD( m.LParam );
						int y = NativeMethods.HIWORD( m.LParam );
						Point pt = Point.Empty;

						if( bClient )
						{
							NativeMethods.POINT ptScreen = new NativeMethods.POINT( x, y );

							if( NativeMethods.ClientToScreen( m.HWnd, ref ptScreen ) != 0 )
							{
								pt = new Point( ptScreen.X, ptScreen.Y );
							}
						}
						else
						{
							pt = new Point( x, y );
						}

						Hide( pt );
					}
				}
				else
				{
				}
			}
		}

		private void OnKeyDown( Message m )
		{
			if( this.Opened && (int)m.WParam == VK_ESCAPE )
			{
				Hide();
			}
		}

		private void OnActivate( Message m )
		{
			if( this.Opened )
			{
				Control top = m_owner.TopLevelControl;

				if( top != null && top.Handle == m.HWnd
					&& NativeMethods.WA_INACTIVE == NativeMethods.LOWORD( m.WParam ) )
				{
					if( this.IsHandleCreated && this.Handle != m.LParam )
					{
						Hide();
					}
				}
			}
		}

		private void OmWmActivate( ref Message m )
		{
			if( NativeMethods.LOWORD( m.WParam ) == NativeMethods.WA_INACTIVE )
			{
				Point pt;
				NativeMethods.GetCursorPos( out pt );
                if (!this.RectangleToScreen(this.DisplayRectangle).Contains(pt))
				Hide( pt );
			}
			else
			{
				m_owner.Focus();
			}

			m.Result = IntPtr.Zero;
		}

		private void OnWmNcPaint( ref Message m )
		{
			IntPtr hdc = NativeMethods.GetWindowDC( this.Handle );

			if( hdc != IntPtr.Zero )
			{
				NativeMethods.RECT rc = new NativeMethods.RECT();
				NativeMethods.GetWindowRect( (int)this.Handle, ref rc );

				using( Graphics g = Graphics.FromHdc( hdc ) )
				{
					DrawNcArea( g, new Rectangle( 0, 0, rc.Width, rc.Height ) );

					int right = rc.Width - BorderWidth;

					if( right - BorderWidth > 0 )
					{
						int bottom = rc.Height - this.BottomBorderHeight;

						if( bottom - this.BottomBorderHeight > 0 )
						{
							NativeMethods.ExcludeClipRect( hdc, BorderWidth, BorderWidth, right, bottom );
						}
					}
				}

				NativeMethods.ReleaseDC( this.Handle, hdc );
			}

			m.Result = IntPtr.Zero;
		}

		private bool OnWmNcHitTest( ref Message m )
		{
			bool bResult = false;
			int x = NativeMethods.LOWORD( m.LParam );
			int y = NativeMethods.HIWORD( m.LParam );

			int hitTest = GetHitTest( x, y );

			if( hitTest != NativeMethods.HTERROR )
			{
				m.Result = (IntPtr)hitTest;
				bResult = true;
			}

			return bResult;
		}

		private void OnWmNcCalcSize( ref Message m )
		{
			NativeMethods.RECT rc = (NativeMethods.RECT)m.GetLParam( typeof( NativeMethods.RECT ) );

			rc.top += BorderWidth;
			rc.left += BorderWidth;
			rc.right -= BorderWidth;
			rc.bottom -= this.BottomBorderHeight;

			Marshal.StructureToPtr( rc, m.LParam, false );

			m.Result = IntPtr.Zero;
		}

		#endregion
	}
}