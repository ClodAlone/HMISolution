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
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms
{
	// TODO: Double-click on splitterbar -> split / unsplit

	/// <summary>
	/// Provides notification methods when the user drags the vertical
	/// or horizontal splitter bar.
	/// </summary>
	interface IInternalSplitterParent
	{

		/// <summary>
		/// Occurs when the user drags the splitter bar.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="x">The current horizontal position in pixels.</param>
		/// <param name="y">The current vertical position in pixels.</param>
		void OnMoveSplitter( object sender, int x, int y );

		/// <summary>
		/// Occurs after the user moves the splitter bar.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		void OnMovedSplitter( object sender );

		/// <summary>
		/// Repaints the splitter bar.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		void InvalidateSplitter( object sender );

		/// <summary>
		/// Gets / sets the cursor to display.
		/// </summary>
		Cursor OverrideCursor
		{
			get;
			set;
		}
	}

	/// <internalonly/>
	[Syncfusion.Documentation.DocumentationExclude()]
	public enum InternalSplitterKind
	{
		/// <internalonly/>
		HorizontalBar,
		/// <internalonly/>
		VerticalBar
	}

	/// <internalonly/>
	[
	Flags,
	Syncfusion.Documentation.DocumentationExclude()
	]
	public enum InternalSplitterState
	{
		/// <internalonly/>
		Normal=0x00,
		/// <internalonly/>
		Disabled=0x01,
		/// <internalonly/>
		Hovered=0x02,
		/// <internalonly/>
		Pushed=0x04,
	}

	/// <internalonly/>
	[Syncfusion.Documentation.DocumentationExclude()]
	public class InternalSplitter: IDisposable
	{		
        internal InternalSplitterState state = InternalSplitterState.Normal;
		internal InternalSplitterKind kind;
		internal bool dirty = false;
		internal Control parent;
		internal Rectangle bounds;
		internal string tooltip = "Resize";
		internal int tooltipID = -1;
		internal Point cachedMousePos;
		internal ArrayList controlList = new ArrayList();
		private ThemedScrollBarDrawing themedDrawing = null;
		private bool m_bMouseDoubleClicked;
		/// <summary>
		/// Specifies office 2007 color scheme.
		/// </summary>
		private Office2007Theme m_colorScheme = Office2007Theme.Blue;
		/// <summary>
		/// Style of the control.
		/// </summary>
		private TabBarSplitterStyle m_style = TabBarSplitterStyle.Default;
		/// <summary>
		/// Color table for Office2007 visual style.
		/// </summary>
		private Office2007Colors m_office2007ColorTable = null;

		/// <internalonly/>
		public InternalSplitter( Control parent, InternalSplitterKind kind )
		{
			this.kind = kind;
			this.parent = parent;

			// Add events this.parent.
			WireParent( parent );

			ButtonBar bar = parent as ButtonBar;
			if( bar != null )
				bar.CancelMode += new EventHandler( OnCancelModeEvent );

			if( XPThemes.IsThemedOS )
			{
				this.themedDrawing = new ThemedScrollBarDrawing( this.parent );
			}
		}

        void WireParent( Control parent )
		{
			parent.MouseDown += new MouseEventHandler( OnMouseDownEvent );
			parent.MouseMove += new MouseEventHandler( OnMouseMoveEvent );
			parent.MouseUp += new MouseEventHandler( OnMouseUpEvent );
			parent.MouseLeave += new EventHandler( OnMouseLeaveEvent );
			parent.MouseDoubleClick += new MouseEventHandler( OnMouseDoubleClick );
		}

		void UnwireParent( Control parent )
		{
			parent.MouseDown -= new MouseEventHandler( OnMouseDownEvent );
			parent.MouseMove -= new MouseEventHandler( OnMouseMoveEvent );
			parent.MouseUp -= new MouseEventHandler( OnMouseUpEvent );
			parent.MouseLeave -= new EventHandler( OnMouseLeaveEvent );
			parent.MouseDoubleClick -= new MouseEventHandler( OnMouseDoubleClick );
		}

		/// <internalonly/>
		public void WireTabPage( Control parent )
		{
			WireParent( parent );
			controlList.Add( parent );
		}

		/// <internalonly/>
		public void UnwireTabPage( Control parent )
		{
			UnwireParent( parent );
			controlList.Remove( parent );
		}

		/// <internalonly/>
		~InternalSplitter()
		{
			Dispose( false );
		}

		/// <internalonly/>
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/// <internalonly/>
		protected virtual void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( themedDrawing != null )
				{
					themedDrawing.Dispose();
					themedDrawing = null;
				}

				if( this.parent != null )
				{
					this.UnwireParent( parent );

					ButtonBar bar = parent as ButtonBar;
					if( bar != null )
						bar.CancelMode -= new EventHandler( OnCancelModeEvent );
				}

				foreach( Control control in new ArrayList( this.controlList ) )
					UnwireTabPage( control );

				this.parent = null;
			}
		}

		internal ControlToolTip TooltipProvider
		{
			get
			{
				IControlToolTipProvider target = this.parent as IControlToolTipProvider;
				if( target != null )
					return target.GetControlToolTip();
				return null;
			}
		}

		/// <internalonly/>
		public void ResetToolTip()
		{
			lock( this )
			{
				ControlToolTip toolTipProvider = this.TooltipProvider;
				if( toolTipProvider != null )
					toolTipProvider.InitToolTip( ref this.tooltipID, "", Rectangle.Empty );
			}
		}


		/// <internalonly/>
		public void InitToolTip( Rectangle bounds )
		{
			lock( this )
			{
				ControlToolTip toolTipProvider = this.TooltipProvider;
				if( toolTipProvider != null )
					toolTipProvider.InitToolTip( ref this.tooltipID, this.ToolTip, bounds );
			}
		}

		/// <internalonly/>
		public System.Windows.Forms.ButtonState GetWinFormButtonState( bool flatLook )
		{
			System.Windows.Forms.ButtonState state = System.Windows.Forms.ButtonState.Normal;
			if( !this.Enabled )
				state |= System.Windows.Forms.ButtonState.Inactive;
			if( this.Pushed )
				state |= System.Windows.Forms.ButtonState.Pushed;

			if( flatLook )
			{
				if( !Hovered && !Pushed )
					state |= System.Windows.Forms.ButtonState.Flat;
			}

			return state;
		}

		/// <summary>
		/// Gets or sets the visual style of the tabBarSplitterControl.
		/// </summary>
		public TabBarSplitterStyle Style
		{
			get
			{
				return m_style;
			}
			set
			{
				if( m_style != value )
				{
					m_style = value;
					this.OnStyleChanged();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnStyleChanged()
		{
			m_office2007ColorTable = Office2007Colors.GetColorTable( m_colorScheme );
		}

		/// <summary>
		/// Gets or sets office 2007 color scheme.
		/// </summary>
		public Office2007Theme Office2007ColorScheme
		{
			get
			{
				return m_colorScheme;
			}

			set
			{
				if( m_colorScheme != value )
				{
					m_colorScheme = value;
					this.OnStyleChanged();
				}
			}
		}

		/// <summary>
		/// Gets color table for Office2007 visual style.
		/// </summary>
		internal Office2007Colors Office2007ColorTable
		{
			get
			{
				Office2007Colors colorTable = ( m_office2007ColorTable == null ) ?
					Office2007Colors.Default : m_office2007ColorTable;

				return colorTable;
			}
		}

		/// <internalonly/>
		public void Paint( Graphics g, bool flatLook )
		{
			Debug.Assert( g != null, "Must pass valid graphics" );
			try
			{
				if( bounds.IsEmpty )
				{
					//Debug.WriteLine("InternalButtonPar.Paint: Bounds is empty. Nothing to draw.");
					return;
				}

				InitToolTip( bounds );

				if( !g.ClipBounds.IntersectsWith( bounds ) )
					return; // nothing to draw

				ButtonState buttonState = GetWinFormButtonState( flatLook );
				Rectangle r = bounds;
				/*if (flatLook && buttonState != ButtonState.Pushed)
				{
					if (this.kind == InternalSplitterKind.HorizontalBar)
						OffsetTop(ref r, 1);
					else
						OffsetLeft(ref r, 1);
				}*/

				IThemedControl parent = this.parent as IThemedControl;
				if( parent != null && XPThemes.IsThemedOS && XPThemes.IsThemeActive && parent.ThemesEnabled )
				{
					this.themedDrawing.DrawThumb( g, r, this.kind == InternalSplitterKind.HorizontalBar, buttonState );
				}
				else if( this.Style == TabBarSplitterStyle.Office2007 )
				{
					if( this.Kind == InternalSplitterKind.VerticalBar )
					{
						r.Height--;
					}
					else
					{
						r.Width--;
					}

					Color startColor = this.Office2007ColorTable.TabBarSplitterTabStartColor;
					Color endColor = this.Office2007ColorTable.TabBarSplitterTabEndColor;

					using( LinearGradientBrush br = new LinearGradientBrush( r, startColor, endColor, LinearGradientMode.Vertical ) )
					{
						g.FillRectangle( br, r );
					}

					using( Pen pen = new Pen( this.Office2007ColorTable.TabBarSplitterBorderColor ) )
					{
						g.DrawRectangle( pen, r );
					}
				}
                else if (this.Style == TabBarSplitterStyle.Metro)
                {
                    DrawButton(g, r, buttonState);
                }
                else
                {
                    ControlPaint.DrawButton(g, r, buttonState);
                }

			}
			finally
			{
				this.dirty = false;
			}
		}
        public void DrawButton(Graphics g, Rectangle rect, ButtonState Buttonstate)
        {
            SolidBrush brush = new SolidBrush(Color.FromArgb (255, Color.LightGray));
            if (Buttonstate == ButtonState.Normal)
            {
                brush = new SolidBrush(Color.Gray);
                g.FillRectangle(brush, rect);
            }
            else if (Buttonstate == ButtonState.Flat)
            {
                g.FillRectangle(brush, rect);
            }
            else if (Buttonstate == ButtonState.Pushed)
            {
                 brush = new SolidBrush(ColorTranslator.FromHtml("#119EDA"));
                g.FillRectangle(brush, rect);
            }
            brush.Dispose();
        }
		/// <internalonly/>
		public static void OffsetLeft( ref Rectangle r, int dx )
		{
			r.X += dx;
			r.Width = Math.Max( r.Width - dx, 0 );
		}

		/// <internalonly/>
		public static void OffsetTop( ref Rectangle r, int dy )
		{
			r.Y += dy;
			r.Height = Math.Max( r.Height - dy, 0 );
		}

		/// <internalonly/>
		public void Refresh()
		{
			this.Dirty = true;
			parent.Invalidate( bounds );

			IInternalSplitterParent notifier = parent as IInternalSplitterParent;
			if( notifier != null )
			{
				if( Pushed || Hovered )
					notifier.OverrideCursor = this.kind == InternalSplitterKind.HorizontalBar ? Cursors.VSplit : Cursors.HSplit;
				else
					notifier.OverrideCursor = null;
			}

			foreach( object obj in this.controlList )
			{
				notifier = obj as IInternalSplitterParent;
				if( notifier != null )
				{
					notifier.InvalidateSplitter( this );

					if( Pushed || Hovered )
						notifier.OverrideCursor = this.kind == InternalSplitterKind.HorizontalBar ? Cursors.VSplit : Cursors.HSplit;
					else
						notifier.OverrideCursor = null;
				}
			}
		}

		/// <internalonly/>
		public bool HitTest( int x, int y )
		{
			return bounds.Contains( x, y );
		}

		/// <internalonly/>
		public void CancelMode()
		{
			lock( this )
			{
				if( Pushed )
					this.Container.Capture = false;
				Pushed = false;
				Hovered = false;
				if( this.Dirty )
					Refresh();

				IInternalSplitterParent notifier = parent as IInternalSplitterParent;
				if( notifier != null )
				{
					notifier.OverrideCursor = null;
					notifier.OnMovedSplitter( null );
				}
				foreach( object obj in this.controlList )
				{
					notifier = obj as IInternalSplitterParent;
					if( notifier != null )
						notifier.OverrideCursor = null;
				}
			}
		}

		/// <internalonly/>
		protected void OnCancelModeEvent( object sender, EventArgs e )
		{
			CancelMode();
		}

		/// <internalonly/>
		protected void OnMouseDownEvent( object sender, MouseEventArgs e )
		{
			//Debug.Assert(sender == parent, "OnMouseDownEvent: sender must be same as parent.");

			lock( this )
			{
				if( this.Pushed )
				{
					if( e.Button != MouseButtons.Left )
						CancelMode();
					return;
				}

				// Check if parent is processing mouse messages.
				Control control = sender as Control;
				this.cachedMousePos = Point.Empty;
				Pushed = HitTest( e.X, e.Y );
				if( Pushed )
				{
					if( this.Dirty )
					{
						this.cachedMousePos = new Point( e.X, e.Y );
						Refresh();
					}
					control.Capture = true;
				}
			}
		}

		/// <internalonly/>
		protected void OnMouseUpEvent( object sender, MouseEventArgs e )
		{
			//Debug.Assert(sender == parent, "OnMouseDownEvent: sender must be same as parent.");

			lock( this )
			{
				bool notify = Pushed;
				//CancelMode();
				if( Pushed )
					this.Container.Capture = false;
				Pushed = false;
				Hovered = false;

				if( m_bMouseDoubleClicked )
				{
					m_bMouseDoubleClicked = false;
					return;
				}

				if( notify )
				{
					try
					{
						IInternalSplitterParent notifier = parent as IInternalSplitterParent;

						if( notifier != null )
						{
							if( !HitTest( e.X, e.Y ) )
							{
								int dx = e.X - bounds.Left - bounds.Width / 2;
								int dy = e.Y - bounds.Top - bounds.Height / 2;

								notifier.OnMoveSplitter( this, dx, dy );
								notifier.OnMovedSplitter( this );
							}
							else
							{
								this.cachedMousePos = Point.Empty;
								OnMouseMoveEvent( sender, e );
							}
						}
					}
					catch( Exception ex )
					{
						TraceUtil.TraceExceptionCatched( ex );
						if( !ExceptionManager.RaiseExceptionCatched( null, ex ) )
							throw;
					}
				}
			}
		}

		/// <internalonly/>
		protected void OnMouseMoveEvent( object sender, MouseEventArgs e )
		{
			//Debug.Assert(sender == parent, "OnMouseDownEvent: sender must be same as parent.");

			lock( this )
			{
				// Check if parent is processing mouse messages.
				bool isOverSplitter = !Pushed && HitTest( e.X, e.Y );

				if( !this.cachedMousePos.IsEmpty
					&& ( this.kind == InternalSplitterKind.HorizontalBar && Math.Abs( e.X - this.cachedMousePos.X ) < 3
					|| this.kind == InternalSplitterKind.VerticalBar && Math.Abs( e.Y - this.cachedMousePos.Y ) < 3 ) )
					return;

				this.cachedMousePos = Point.Empty;

				if( !Pushed )
				{
					Hovered = isOverSplitter;
					if( this.Dirty )
						Refresh();
				}

				if( Pushed )
				{
					if( this.Dirty )
						Refresh();

					try
					{
						IInternalSplitterParent notifier = parent as IInternalSplitterParent;

						if( notifier != null && !HitTest( e.X, e.Y ) )
						{
							int dx = e.X - bounds.Left - bounds.Width / 2;
							int dy = e.Y - bounds.Top - bounds.Height / 2;

							notifier.OnMoveSplitter( this, dx, dy );
						}
					}
					catch( Exception ex )
					{
						TraceUtil.TraceExceptionCatched( ex );
						if( !ExceptionManager.RaiseExceptionCatched( null, ex ) )
							throw;
					}
				}
			}
		}

		/// <internalonly/>
		protected void OnMouseLeaveEvent( object sender, EventArgs e )
		{
			lock( this )
			{
				if( !Pushed )
				{
					Hovered = false;
					if( this.Dirty )
						Refresh();
				}
			}
		}

		/// <internalonly/>
		protected void OnMouseDoubleClick( object sender, MouseEventArgs e )
		{			
			m_bMouseDoubleClicked = true;
		}

		/// <internalonly/>
		public string ToolTip
		{
			get
			{
				return this.tooltip;
			}
			set
			{
				this.tooltip = value;
			}
		}

		/// <internalonly/>
		public Rectangle Bounds
		{
			get
			{
				return bounds;
			}
			set
			{
				bounds = value;
			}
		}

		/// <internalonly/>
		public Control Container
		{
			get
			{
				return parent;
			}
			set
			{
				if( parent != value )
				{
					if( parent != null )
						this.UnwireParent( parent );
					parent = value;
					if( parent != null )
						this.WireParent( parent );
				}
			}
		}

		/// <internalonly/>
		public InternalSplitterKind Kind
		{
			get
			{
				return kind;
			}
			set
			{
				kind = value;
			}
		}


		// Dirty bit.
		/// <internalonly/>
		public bool Dirty
		{
			get
			{
				return this.dirty;
			}
			set
			{
				this.dirty = value;
			}
		}

		// Enabled state.
		/// <internalonly/>
		public bool Enabled
		{
			get
			{
				return ( ( this.state & InternalSplitterState.Disabled ) == 0 );
			}
			set
			{
				// Note that setting the button enabled will negate the Disabled flag.
				if( value != this.Enabled )
				{
					if( value )
						this.state &= ~InternalSplitterState.Disabled;
					else
						this.state = InternalSplitterState.Disabled;

					this.dirty = true;
				}
			}
		}

		// Hovered state.
		/// <internalonly/>
		public bool Hovered
		{
			get
			{
				return ( ( this.state & InternalSplitterState.Hovered ) != 0 );
			}
			set
			{
				if( Enabled && value != this.Hovered )
				{
					if( value )
						// Checked is the only other valid flag.
						this.state = InternalSplitterState.Hovered;
					else
						this.state &= ~InternalSplitterState.Hovered;
					this.dirty = true;
				}
			}
		}

		// Pushed state.
		/// <internalonly/>
		public bool Pushed
		{
			get
			{
				return ( ( this.state & InternalSplitterState.Pushed ) != 0 );
			}
			set
			{
				if( Enabled && value != this.Pushed )
				{
					if( value )
						// Checked is the only other valid flag.
						this.state = InternalSplitterState.Pushed;
					else
						this.state &= ~InternalSplitterState.Pushed;
					this.dirty = true;
				}
			}
		}
	}
}
