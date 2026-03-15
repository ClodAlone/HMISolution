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

#region File Using
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Syncfusion.Collections;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Win32;
using Syncfusion.Runtime.InteropServices;
#endregion

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	[FlagsAttribute,
	Syncfusion.Documentation.DocumentationExclude()]
	public enum ScrollArrowState
	{
		Up = 0x0001,
		Down = 0x0002,
		Disabled = 0x0004
	}

	[ToolboxItem( false ),
	Syncfusion.Documentation.DocumentationExclude()]
	public class MenuArrowButtonControl: Control
	{
		protected ScrollArrowState scrollArrowState = ScrollArrowState.Up;
		internal MenuArrowButtonControlWeakContainer menuArrowButtonControlWeakContainer = null;

		public MenuArrowButtonControl( ScrollArrowState scrollArrowState )
		{
			//| ControlStyles.Selectable
			this.SetStyle( ControlStyles.UserPaint, true );
			this.BackColor = MenuColors.MenuBGColor;
			this.scrollArrowState = scrollArrowState;
			this.TabStop = false;

			menuArrowButtonControlWeakContainer = new MenuArrowButtonControlWeakContainer( this );
			MenuColors.MenuColorsChanged += new EventHandler( this.menuArrowButtonControlWeakContainer.MenuColorsChangedWeakEventHandler );
			Office2003Colors.MenuColorsChanged += new EventHandler( menuArrowButtonControlWeakContainer.Office2003ColorsChangedWeakEventHandler );
		}

		protected override void OnSystemColorsChanged( EventArgs e )
		{
			base.OnSystemColorsChanged( e );
			MenuColors.SysColorsChanged( false );
			Office2003Colors.SysColorsChanged( false );
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			return new MenuButtonAccessibleObject( this );
		}

		class MenuButtonAccessibleObject: Control.ControlAccessibleObject
		{
			MenuArrowButtonControl parent;
			public MenuButtonAccessibleObject( MenuArrowButtonControl parent )
				: base( parent )
			{
				this.parent = parent;
			}
			public override AccessibleRole Role
			{
				get
				{
					return AccessibleRole.PushButton;
				}
			}
			public override string Name
			{
				get
				{
					return this.parent.Text;
				}
			}
			public override AccessibleStates State
			{
				get
				{
					AccessibleStates state = base.State;
					if( (parent.ArrowState & ScrollArrowState.Disabled) > 0 )
						state |= AccessibleStates.Unavailable;
					else
						state |= AccessibleStates.Focusable;

					if( parent.MouseHovering )
						state |= AccessibleStates.HotTracked;

					if( parent.MousePressed )
						state |= AccessibleStates.Focused;

					return state;
				}
			}
		}

		protected override void Dispose( bool disposing )
		{
			//MenuColors.MenuColorsChanged -= new EventHandler(this.MenuColorsChanged);
			//Office2003Colors.MenuColorsChanged -= new EventHandler(this.MenuColorsChanged);
            MenuColors.MenuColorsChanged -= new EventHandler(this.menuArrowButtonControlWeakContainer.MenuColorsChangedWeakEventHandler);
            Office2003Colors.MenuColorsChanged -= new EventHandler(menuArrowButtonControlWeakContainer.Office2003ColorsChangedWeakEventHandler);
			base.Dispose( disposing );
		}

		internal void MenuColorsChanged( object sender, EventArgs e )
		{
			this.BackColor = MenuColors.MenuBGColor;
		}


		public ScrollArrowState ArrowState
		{
			get { return this.scrollArrowState; }
			set
			{
				if( this.scrollArrowState != value )
				{
					this.scrollArrowState = value;
					if( (this.scrollArrowState & ScrollArrowState.Disabled) > 0 )
						this.MouseHovering = false;
					this.Invalidate();
				}
			}
		}
		private bool mousePressed = false;
		protected virtual bool MousePressed
		{
			get { return mousePressed; }
			set
			{
				if( this.mousePressed != value )
				{
					this.mousePressed = value;
					this.Invalidate();
				}
			}
		}
		private VisualStyle style = VisualStyle.OfficeXP;
		internal VisualStyle Style
		{
			get
			{
				return this.style;
			}
			set
			{
				this.style = value;
			}
		}
		private bool mouseHover = false;
		protected bool MouseHovering
		{
			get { return mouseHover; }
			set
			{
				if( this.mouseHover != value )
				{
					this.mouseHover = value;
					if( this.mouseHover && ((this.scrollArrowState & ScrollArrowState.Disabled) == 0) )
					{
						this.BackColor = GetArrowBackHighlightColor( this.Style );
					}
					else
						this.BackColor = MenuColors.MenuBGColor;

					this.Invalidate();
				}
			}
		}

		protected override void OnMouseLeave( EventArgs e )
		{
			this.MouseHovering = false;
			this.MousePressed = false;

			base.OnMouseLeave( e );
		}

		protected override void OnMouseMove( MouseEventArgs e )
		{
			MouseHovering = true;
			base.OnMouseMove( e );
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			UpdateColorScheme();
			bool scrollDown = true;
			Graphics g = e.Graphics;
			Rectangle rect = this.ClientRectangle;

			if( (this.scrollArrowState & ScrollArrowState.Up) > 0 )
				scrollDown = false;

			switch( this.Style )
			{
				case VisualStyle.Office2003:
				{
					DrawOffice2003Arrow( g, rect, scrollDown );
					break;
				}
				case VisualStyle.VS2005:
				{
					DrawVS2005Arrow( g, rect, scrollDown );
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					DrawOffice2007OutlookArrow( g, rect, scrollDown );
					break;
				}
				default:
				{
					DrawDefaultArrow( g, rect, scrollDown );
					break;
				}
			}
		}


		/// <summary>
		/// Gets color for background of the highlight arrow amenably with VisualStyle.
		/// </summary>
		private Color GetArrowBackHighlightColor( VisualStyle style )
		{
			Color color = Color.Empty;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					color = Office2003Colors.SelColor;
					break;
				}
                case VisualStyle.Office2010:
				case VisualStyle.Office2007:
				{
					color = MenuColors.SelColor;
					break;
				}
				case VisualStyle.VS2005:
				{
					color = VS2005Colors.MenuBackground;
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					color = Office2007OutlookColors.MenuBackground;
					break;
				}
				default:
				{
					color = MenuColors.SelColor;
					break;
				}
			}

			return color;
		}


		/// <summary>
		/// Update colors of the colorscheme.
		/// </summary>
		private void UpdateColorScheme()
		{
			MenuColors.UpdateMenuColors();
			Office2003Colors.UpdateMenuColors();
			VS2005Colors.UpdateMenuColors();
		}


		/// <summary>
		/// Gets rectangle for arrow.
		/// </summary>
		private Rectangle GetArrowRect( Rectangle rect )
		{
			int arrowWidth = 7;
			int arrowHeight = 8;
			Rectangle bounds = new Rectangle( rect.Left + (rect.Width - arrowWidth) / 2,
				rect.Top + (rect.Height - arrowHeight) / 2, arrowWidth, arrowHeight );

			return bounds;
		}


		/// <summary>
		/// Draws double arrow.
		/// </summary>
		private void DrawDoubleArrow( Graphics g, Rectangle rect, bool scrollDown )
		{
			Rectangle bounds = GetArrowRect( rect );
			Pen pen = new Pen( Brushes.Black, 1 );

			if( (this.scrollArrowState & ScrollArrowState.Disabled) > 0 )
				pen = new Pen( SystemColors.InactiveCaptionText, 1 );

			Point[] lines = null;

			if( !scrollDown ) // Scroll Up Arrow
			{
				Point[] temp = {new Point(bounds.Left, bounds.Bottom - 4),
								   new Point(bounds.Left + bounds.Width/2, bounds.Top),
								   new Point(bounds.Right - 1, bounds.Bottom - 4)};
				lines = temp;
			}
			else	// Scroll Down Arrow
			{
				Point[] temp = {new Point(bounds.Left, bounds.Top),
								   new Point(bounds.Left + bounds.Width/2, bounds.Bottom - 4),
								   new Point(bounds.Right - 1, bounds.Top)};
				lines = temp;
			}

			SmoothingMode oldS = g.SmoothingMode;
			g.SmoothingMode = SmoothingMode.AntiAlias;

			if( this.mousePressed )
			{
				lines[0].Offset( 1, 1 );
				lines[1].Offset( 1, 1 );
				lines[2].Offset( 1, 1 );
			}

			g.DrawLines( pen, lines );
			lines[0].Offset( 0, 3 );
			lines[1].Offset( 0, 3 );
			lines[2].Offset( 0, 3 );
			g.DrawLines( pen, lines );

			g.SmoothingMode = oldS;
		}


		/// <summary>
		/// Drawa single arrow.
		/// </summary>
		private void DrawSingleArrow( Graphics g, Rectangle rect, bool scrollDown )
		{
			Brush arrowBrush = new SolidBrush( Color.Black );
			Rectangle bounds = GetArrowRect( rect );
			GraphicsPath arrowPath = new GraphicsPath();
			Point[] points = null;

			if( scrollDown )
			{
				points = new Point[]{ new Point(bounds.Left, bounds.Top),
										new Point(bounds.Right, bounds.Top),
										new Point(bounds.Left + bounds.Width/2, bounds.Bottom - 4)};
			}
			else
			{
				points = new Point[]{ new Point(bounds.Left - 1, bounds.Bottom),
										new Point(bounds.Right, bounds.Bottom),
										new Point(bounds.Left + bounds.Width/2, bounds.Bottom - 5)};
			}

			arrowPath.AddLines( points );
			g.FillPath( arrowBrush, arrowPath );
            arrowBrush.Dispose();
            arrowPath.Dispose();
		}


		/// <summary>
		/// Draws border for arrow.
		/// </summary>
		private void DrawBorder( Graphics g, Rectangle rect )
		{
			// If mouse over or pressed, draw a 3D border around the rect:
			if( this.MouseHovering
				&& (this.scrollArrowState & ScrollArrowState.Disabled) == 0 )
			{
				// Draw a frame around it.
				if( !this.mousePressed )
				{
					ControlPaint.DrawBorder3D( g,
						rect, Border3DStyle.RaisedOuter, Border3DSide.Top | Border3DSide.Left | Border3DSide.Right | Border3DSide.Bottom );
				}
				else
				{
					ControlPaint.DrawBorder3D( g,
						rect, Border3DStyle.SunkenOuter, Border3DSide.Top | Border3DSide.Left | Border3DSide.Right | Border3DSide.Bottom );
				}
			}
		}


		/// <summary>
		/// Draws arrow for default style.
		/// </summary>
		private void DrawDefaultArrow( Graphics g, Rectangle rect, bool scrollDown )
		{
			DrawBorder( g, rect );
			DrawDoubleArrow( g, rect, scrollDown );
		}


		/// <summary>
		/// Draws arrow for Office2003 style.
		/// </summary>
		private void DrawOffice2003Arrow( Graphics g, Rectangle rect, bool scrollDown )
		{
			Rectangle bounds = GetArrowRect( rect );
			GraphicsPath path = new GraphicsPath();
			Rectangle circleBounds = bounds;
			circleBounds.Width += 1;
			circleBounds.Inflate( 3, 3 );
			path.AddEllipse( circleBounds );
			LinearGradientBrush brush = new LinearGradientBrush( circleBounds, Office2003Colors.MenuMarginColorLight,
				Office2003Colors.MenuMarginColorDark, LinearGradientMode.ForwardDiagonal );

			g.FillPath( brush, path );
			brush.Dispose();
			path.Dispose();

			DrawDefaultArrow( g, rect, scrollDown );
		}


		/// <summary>
		/// Draws arrow for VS2005 style.
		/// </summary>
		private void DrawVS2005Arrow( Graphics g, Rectangle rect, bool scrollDown )
		{
			DrawSingleArrow( g, rect, scrollDown );
		}

		/// <summary>
		/// Draws arrow for Office2007Outlook style.
		/// </summary>
		private void DrawOffice2007OutlookArrow( Graphics g, Rectangle rect, bool scrollDown )
		{
			DrawSingleArrow( g, rect, scrollDown );
		}
	}

	[ToolboxItem( false ),
	Syncfusion.Documentation.DocumentationExclude()]
	public class MenuExpandButton: MenuArrowButtonControl
	{
		private Timer timer;
		private MenuGridHost gridHost;
		public MenuExpandButton( ScrollArrowState scrollArrowState, MenuGridHost gridHost )
			: base( scrollArrowState )
		{
			this.gridHost = gridHost;
			this.timer = new Timer();
			this.timer.Interval = 5000; // After 5 seconds, expand
			this.timer.Tick += new EventHandler( this.TimerHandler );
			this.Text = "Menu Expand Button";
		}

        private MenuGrid GetGrid()
        {
            if (this.Parent != null)
            {
                foreach (Control con in this.Parent.Controls)
                {
                    if (con is MenuGrid)
                    {
                        return con as MenuGrid;
                    }
                }
            }
            return null;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            MenuGrid parentGrid = GetGrid();
            if(parentGrid != null)
                parentGrid.HighlightRange = GridRangeInfo.Empty;
            base.OnMouseEnter(e);
        }

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( this.timer != null )
				{
					this.timer.Tick -= new EventHandler( this.TimerHandler );
					this.timer.Dispose();
					this.timer = null;
				}
			}
			base.Dispose( disposing );
		}

		protected override void OnVisibleChanged( EventArgs e )
		{
			if( this.Visible )
				this.timer.Start();
			else
				this.timer.Stop();
			base.OnVisibleChanged( e );
		}

		private void TimerHandler( object source, EventArgs args )
		{
			if( this.gridHost.currentMenuGrid != null
				&& this.gridHost.currentMenuGrid.NeedDelayedExpansion() )
			{
				this.gridHost.currentMenuGrid.ExpandPartialMenus( false );
			}

			this.timer.Stop();
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
            MenuGrid parentGrid = this.gridHost.currentMenuGrid;
            if (parentGrid == null)
            {
                parentGrid = GetGrid();
            }
            if (parentGrid != null)
                parentGrid.ExpandPartialMenus(true);
		}
	}

	[ToolboxItem( false ),
	Syncfusion.Documentation.DocumentationExclude()]
	public class MenuScrollControl: MenuArrowButtonControl
	{
		private Timer timer = new Timer();

		private MenuGrid grid;
		public MenuGrid MenuGrid
		{
			get { return grid; }
			set { grid = value; }
		}
		public MenuScrollControl( ScrollArrowState scrollArrowState )
			: base( scrollArrowState )
		{
			this.timer.Tick += new EventHandler( this.TimerHandler );
			this.timer.Interval = 0x1f4;
			this.Text = "Menu Scroll Button";
		}

		protected override void Dispose( bool disposing )
		{
			this.timer.Tick -= new EventHandler( this.TimerHandler );
			base.Dispose( disposing );
		}

		protected override bool MousePressed
		{
			get { return base.MousePressed; }
			set
			{
				if( base.MousePressed != value )
				{
					base.MousePressed = value;

					if( !base.MousePressed )
						this.timer.Stop();
					else
						this.timer.Start();
				}
			}
		}
		private void TimerHandler( object source, EventArgs args )
		{
			this.DoScroll();
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			if( (this.scrollArrowState & ScrollArrowState.Disabled) == 0 )
			{
				this.MousePressed = true;
				DoScroll();
			}
			base.OnMouseDown( e );
		}
		protected virtual void DoScroll()
		{
			if( this.grid != null )
			{
				if( (this.scrollArrowState & ScrollArrowState.Down) > 0 )
					// Scroll down
					this.grid.Scroll( true );
				else
					// Scroll Up
					this.grid.Scroll( false );
			}
		}

		internal void UpdateScrollState()
		{
			if( this.grid == null )
				return;

			if( this.grid.CanScroll( (this.scrollArrowState & ScrollArrowState.Down) > 0 ) )
				this.ArrowState = this.ArrowState & ~ScrollArrowState.Disabled;
			else
				this.ArrowState = this.ArrowState | ScrollArrowState.Disabled;
		}

		protected override void OnMouseHover( EventArgs e )
		{
			if( this.MousePressed )
				this.DoScroll();
			base.OnMouseHover( e );
		}
		protected override void OnMouseUp( MouseEventArgs e )
		{
			this.MousePressed = false;
			base.OnMouseUp( e );
		}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class MenuGridHost: PopupHost
	{
		internal MenuGrid currentMenuGrid;
		protected internal const int ScrollHeight = 19;
		private MenuScrollControl scrollUp;
		private MenuScrollControl scrollDown;
		private MenuExpandButton menuExpandButton;
		public MenuGridHost()
			: base()
		{
			base.SuspendLayout();
			this.BorderGap = 4;
			this.menuExpandButton = new MenuExpandButton( ScrollArrowState.Down, this );
			this.menuExpandButton.Height = ScrollHeight;
			this.menuExpandButton.Width = 0;
			UtilFuncs.SetVisibleNoActivate( this.menuExpandButton, false );
			this.menuExpandButton.Dock = DockStyle.Bottom;
			// Will add later
			//this.Controls.Add(this.menuExpandButton);

			this.scrollUp = new MenuScrollControl( ScrollArrowState.Up );
			this.scrollDown = new MenuScrollControl( ScrollArrowState.Down );
			scrollUp.Height = ScrollHeight;
			scrollDown.Height = ScrollHeight;
			scrollUp.Width = 0;
			scrollDown.Width = 0;

			scrollDown.MouseEnter += new EventHandler( scrollDown_MouseEnter );
			scrollDown.MouseLeave += new EventHandler( ScrollButtonMouseLeave );

			scrollUp.MouseEnter += new EventHandler( scrollUp_MouseEnter );
			scrollUp.MouseLeave += new EventHandler( ScrollButtonMouseLeave );

			m_scrollTimer = new Timer();
			m_scrollTimer.Tick += new EventHandler( ScrollTimerTick );
			m_scrollTimer.Interval = ParentBarItem.DEF_SCROLL_SPEED;

			UtilFuncs.SetVisibleNoActivate( this.scrollUp, false );
			UtilFuncs.SetVisibleNoActivate( this.scrollDown, false );
			scrollUp.Dock = DockStyle.Top;
			scrollDown.Dock = DockStyle.Bottom;
			// Add them later for perf. reasons.
			//this.Controls.Add(scrollUp);
			//this.Controls.Add(scrollDown);

			this.DockPadding.All = BorderGap / 2;

			this.NeedShadow = true;
			base.ResumeLayout( false );
		}
		protected override void WndProc( ref Message m )
		{
			if( m.Msg == 0x21/*WM_MOUSEACTIVATE*/)
			{
				m.Result = (IntPtr)3; //MA_NOACTIVATE
				return;
			}

			base.WndProc( ref m );
		}

		protected override void SetBoundsCore( int x, int y, int width, int height, BoundsSpecified specified )
		{
			if( this.IsShowing() && (specified == BoundsSpecified.Location || specified == BoundsSpecified.Size) )
			{
				Rectangle borderRect = this.ClientRectangle;
				VisualStyle style = VisualStyle.OfficeXP;
				bool bRTL = false;
				bool bParent = false;

				if( this.currentMenuGrid != null )
				{
					style = this.currentMenuGrid.Style;
					bRTL = currentMenuGrid.IsRightToLeft();
					bParent = (currentMenuGrid.PopupParent is PopupMenu) ? false : true;
				}

				SetRegion( style, borderRect, bParent, bRTL );
			}

			base.SetBoundsCore( x, y, width, height, specified );
		}

		public override bool NeedShadow
		{
			get
			{
				if( this.currentMenuGrid != null
					&& this.currentMenuGrid.Customizing )
					return false;
				else
					return base.NeedShadow;
			}
			set
			{
				base.NeedShadow = value;
			}
		}
		public override void ShowPopup()
		{
			base.ShowPopup();
			if( this.NeedShadow && this.Shadow != null )
				this.Shadow.BorderOverlapLine = this.currentMenuGrid.OverlapBorderCue;
		}
		protected override void OnVisibleChanged( EventArgs e )
		{
			base.OnVisibleChanged( e );
			// Change the shadow window's state appropriately
			if( !this.Visible )
			{
				this.ShowScrollBars = false;
				UtilFuncs.SetVisibleNoActivate( this.menuExpandButton, false );
				this.currentMenuGrid = null;
			}
		}
		// Check if the control is related to the menu.
		public bool IsRelatedControl( Control control, bool askParent )
		{
			if( this.currentMenuGrid == null )
				return control == this;
			else
				return this.currentMenuGrid.IsRelatedControl( control, askParent );
		}

		// This form will first get activated if there was as control that took focus within
		// the menu. This will in turn deactivate the parent Form (which means we cannot rely
		// on the Form getting deactivated, when the user clicks outside the app).
		// Then when this form gets deactivated, make sure that that the active form is related.
		protected override void OnDeactivate( EventArgs e )
		{
			base.OnDeactivate( e );
			if( this.currentMenuGrid != null
				&&
				(Form.ActiveForm == null || !this.currentMenuGrid.IsRelatedControl( Form.ActiveForm, true )) )
			{
				bool closeMenu = true;
				if( Form.ActiveForm != null && Form.ActiveForm is MenuGridHost )
				{
					MenuGridHost menuHost = Form.ActiveForm as MenuGridHost;
					closeMenu = !menuHost.IsRelatedControl( this.currentMenuGrid, true );
				}
				if( closeMenu )
					this.currentMenuGrid.HidePopup( PopupCloseType.Deactivated );
			}
		}
		protected override void OnPaint( PaintEventArgs e )
		{
			// The interior transparant color will never get used since this is a just a Solid style
			Rectangle borderRect = this.ClientRectangle;
			VisualStyle style = VisualStyle.OfficeXP;
			bool bRTL = false;
			bool bParent = false;

			if( this.currentMenuGrid != null )
			{
				style = this.currentMenuGrid.Style;
				bRTL = currentMenuGrid.IsRightToLeft();
				bParent = (currentMenuGrid.PopupParent is PopupMenu) ? false : true;
			}

			SetRegion( style, borderRect, bParent, bRTL );

			bool bOffset = (this.currentMenuGrid != null && currentMenuGrid.IsRTL && this.NeedShadow);

			if( style == VisualStyle.Office2007 )
			{
				// draw border with Office2007 visual style
				borderRect.Width -= 1;
				borderRect.Height -= 1;
				bRTL = (bRTL) ? !this.NeedShadow : bRTL;
				Office2007MenuPainter.DrawMenuBorder( e.Graphics, borderRect, bParent, bRTL );
                if(this.Shadow!=null)
				this.Shadow.Visible = false;
			}
            else if (style == VisualStyle.Office2010)
            {
                // draw border with Office2010 visual style
                borderRect.Width -= 1;
                borderRect.Height -= 1;
                bRTL = (bRTL) ? !this.NeedShadow : bRTL;
                Office2010MenuPainter.DrawMenuBorder(e.Graphics, borderRect, bParent, bRTL);
                if (this.Shadow != null)
                    this.Shadow.Visible = false;
            }
            else if (style == VisualStyle.Metro)
            {
                bRTL = (bRTL) ? !this.NeedShadow : bRTL;
                MetroMenuPainter.DrawMenuBorder(e.Graphics, borderRect, bParent, bRTL);
                if (this.Shadow != null)
                    this.Shadow.Visible = false;
            }
            else
            {
                if (bOffset)
                {
                    borderRect.X--;
                }

				// draws outside border
				GridBorder outsideGridBorder = GetGridBorder( style );
				GridBorderPaint.DrawRectangle( e.Graphics, outsideGridBorder, borderRect, Color.Transparent, GridBorderSide.All );

				// draws outside border
				borderRect.Inflate( -1, -1 );
				GridBorder insideGridBorde = new GridBorder( GridBorderStyle.Solid, MenuColors.MenuBGColor );
				GridBorderPaint.DrawRectangle( e.Graphics, insideGridBorde, borderRect, Color.Transparent, GridBorderSide.All );
				outsideGridBorder = insideGridBorde = null;
			}

			if( this.currentMenuGrid != null && this.currentMenuGrid.OverlapBorderCue != null )
			{
				Point left = this.PointToClient( currentMenuGrid.OverlapBorderCue[0] );
				Point right = this.PointToClient( currentMenuGrid.OverlapBorderCue[1] );
				if( bOffset )
				{
					left.X -= 1;
					right.X -= 1;
				}

                if (currentMenuGrid.RightToLeft == RightToLeft.Yes && Environment.OSVersion.Version.Major > 6)
                {
                    right.X = borderRect.Width - right.X;
                    left.X = borderRect.Width - left.X;
                }

                e.Graphics.DrawLine(SystemPens.Control, left, right);
            }
		}

		private const int WS_EX_LAYOUTRTL = 0x400000;

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams result = base.CreateParams;

				if( this.NeedShadow )
				{
					bool bDropShadowEnabled = false;

					NativeMethods.SystemParametersInfo( NativeMethods.SPI_GETDROPSHADOW, 0, ref bDropShadowEnabled, 0 );

					if( bDropShadowEnabled )
					{
						result.ClassStyle |= NativeMethods.CS_DROPSHADOW;
					}

					if( currentMenuGrid != null && currentMenuGrid.IsRightToLeft() )
					{
						result.ExStyle |= WS_EX_LAYOUTRTL;
					}
				}

				return result;
			}
		}

		/// <summary>
		/// Update colors of the colorscheme.
		/// </summary>
		private void UpdateColorScheme()
		{
			MenuColors.UpdateMenuColors();
			Office2003Colors.UpdateMenuColors();
			VS2005Colors.UpdateMenuColors();
		}

		/// <summary>
		/// Sets drawing region.
		/// </summary>
		private void SetRegion( VisualStyle style, Rectangle rect, bool bParent, bool bRTL )
		{
			if( style == VisualStyle.Office2007 )
			{
				this.Region = Office2007MenuPainter.GetMenuRegion( rect, bParent, bRTL );
			}
            else if (style == VisualStyle.Office2010)
            {
                this.Region = Office2010MenuPainter.GetMenuRegion(rect, bParent, bRTL);
            }
			else
			{
				this.Region = null;
			}
		}

		private bool showScrollBars = false;
		bool ShowScrollBars
		{
			get { return this.showScrollBars; }
			set
			{
				if( this.showScrollBars != value )
				{
					this.showScrollBars = value;
					if( value )
					{
						this.Controls.Add( this.scrollDown );
						this.Controls.Add( this.scrollUp );
					}
					UtilFuncs.SetVisibleNoActivate( this.scrollDown, value );
					UtilFuncs.SetVisibleNoActivate( this.scrollUp, value );

					if( this.showScrollBars )
					{
						this.scrollDown.MenuGrid = this.currentMenuGrid;
						this.scrollUp.MenuGrid = this.currentMenuGrid;
						this.scrollDown.UpdateScrollState();
						this.scrollUp.UpdateScrollState();
					}
					else
					{
						this.scrollDown.MenuGrid = null;
						this.scrollUp.MenuGrid = null;
					}
				}
			}
		}

		private bool partialMenusOn = false;
		protected bool PartialMenusOn
		{
			get { return this.partialMenusOn; }
			set
			{
				if( this.partialMenusOn != value )
				{
					this.partialMenusOn = value;
					if( value )
						this.Controls.Add( this.menuExpandButton );
				}
			}
		}
		public void UpdateScrollStates( bool heightChanged )
		{
			if( !heightChanged )
			{
				this.scrollDown.UpdateScrollState();
				this.scrollUp.UpdateScrollState();
			}
		}

		protected override void ComputeMySize()
		{
			Size mySize = this.PopupControl.Size;
			mySize += new Size( BorderGap, BorderGap );
			if( this.ShowScrollBars )
				mySize.Height += (ScrollHeight * 2);
			if( this.PartialMenusOn )
				mySize.Height += this.menuExpandButton.Height;

			if( this.Size != mySize )
				this.Size = mySize;
		}
		protected override void ComputeControlLocation()
		{
			Point menuGridLocation = new Point( BorderGap / 2, BorderGap / 2 ); // Depends on BorderGap
			if( this.ShowScrollBars )
				menuGridLocation.Y += ScrollHeight;

			this.PopupControl.Location = menuGridLocation;
		}
		public void ShowMenu( MenuGrid menuGrid, Size prefSize, bool partialMenusOn/*, Form owner*/)
		{
			prefSize = PrepareLayout( menuGrid, prefSize, partialMenusOn );

			this.ShowPopup();
		}

		/// <summary>
		/// Prepares layout of popup host and hosted menu control before showing UI.
		/// </summary>
		/// <param name="menuGrid">Hosted menu control.</param>
		/// <param name="prefSize">Predered size of popup.</param>
		/// <param name="partialMenusOn">Indicates whether partial menus are used.</param>
		/// <returns>Updated szie of popup.</returns>
		private Size PrepareLayout( MenuGrid menuGrid, Size prefSize, bool partialMenusOn )
		{
			if( this.currentMenuGrid != menuGrid )
			{
				UtilFuncs.SetVisibleNoActivate( this, false );
			}

			this.currentMenuGrid = menuGrid;

			this.NeedShadow = menuGrid.ShowShadow;

			if( this.currentMenuGrid != null )
				this.menuExpandButton.Style = this.scrollDown.Style = this.scrollUp.Style = this.currentMenuGrid.Style;

			this.SuspendLayout();
            // Verify available size and turn on scrollbars if required.
			this.scrollDown.Width = prefSize.Width;
			this.scrollUp.Width = prefSize.Width;
			this.menuExpandButton.Width = prefSize.Width;

			this.PartialMenusOn = partialMenusOn;

			if( (!this.PartialMenusOn && prefSize.Height > SystemInformation.WorkingArea.Height - BorderGap) )
			{
				// Size is bigger than vertical resolution
				this.ShowScrollBars = true;
				prefSize.Height = SystemInformation.WorkingArea.Height - BorderGap - (ScrollHeight * 2);
			}
			else
			{
				this.ShowScrollBars = menuGrid.ShowScrollBars;
			}

			if( this.PartialMenusOn && prefSize.Height > SystemInformation.WorkingArea.Height - BorderGap )
				prefSize.Height = SystemInformation.WorkingArea.Height - BorderGap - this.menuExpandButton.Height;

			UtilFuncs.SetVisibleNoActivate( this.menuExpandButton, this.PartialMenusOn );

			menuGrid.Size = prefSize;

			if( menuGrid.ParentItem.Manager == null )
			{
				Control control = menuGrid.PopupParent.GetPopupParentControl();
                if (control != null)
                {
                    System.Reflection.PropertyInfo[] pInfo = control.GetType().GetProperties();
                    for (int i = 0; i < pInfo.Length; i++)
                    {
                        if (pInfo[i].Name == "ThemesEnabled")
                        {
                            if (pInfo[i].CanRead)
                                menuGrid.ThemesEnabled = (bool)pInfo[i].GetValue(control, null);
                            break;
                        }
                    }
                }
			}

			this.PopupControlContainer = menuGrid;

			this.ResumeLayout( false );

			return prefSize;
		}

		internal void RefreshLayout( MenuGrid menuGrid, Size prefSize, bool partialMenusOn )
		{
			prefSize = PrepareLayout( menuGrid, prefSize, partialMenusOn );
			ComputeLayout();
			UpdateVisibility();
		}

		internal bool preventAdjustLocation = false;
		protected override Point GetAdjustedLocation( Point loc )
		{
			if( preventAdjustLocation )
				return loc;

			int height = (currentMenuGrid == null) ? 0 : currentMenuGrid.PreferredHeight;
			bool bIsMenuOverlappingScreen = (height > SystemInformation.WorkingArea.Height - BorderGap);

			if( this.ShowScrollBars && bIsMenuOverlappingScreen )
			{
				Point newLoc = base.GetAdjustedLocation( loc );
				newLoc.Y = 0;
				return newLoc;
			}
			else
				return base.GetAdjustedLocation( loc );
		}

		private Timer m_scrollTimer = null;

		private bool m_bScrollingDown = false;

		private void StopScrollCurrentGrid()
		{
			m_scrollTimer.Stop();
		}

		private void StartScrollCurrentGrid( bool down )
		{
			if( this.currentMenuGrid != null && currentMenuGrid.ScrollOnMouseMove )
			{
				if( currentMenuGrid.ParentItem != null )
				{
					m_scrollTimer.Interval = currentMenuGrid.ParentItem.ScrollingSpeed;
				}

				m_bScrollingDown = down;

				m_scrollTimer.Start();
			}
		}

		private void scrollDown_MouseEnter( object sender, EventArgs e )
		{
			StartScrollCurrentGrid( true );
		}

		private void scrollUp_MouseEnter( object sender, EventArgs e )
		{
			StartScrollCurrentGrid( false );
		}

		private void ScrollButtonMouseLeave( object sender, EventArgs e )
		{
			StopScrollCurrentGrid();
		}

		private void ScrollTimerTick( object sender, EventArgs e )
		{
			if( currentMenuGrid != null && currentMenuGrid.ScrollOnMouseMove )
			{
				currentMenuGrid.Scroll( m_bScrollingDown );
			}
		}

		/// <summary>
		/// Gets GridBorder amenably with VisualStyle.
		/// </summary>
		private GridBorder GetGridBorder( VisualStyle style )
		{
			GridBorder gridBorder = null;

			switch( style )
			{
				case VisualStyle.Office2003:
				{
					gridBorder = new GridBorder( GridBorderStyle.Solid, Office2003Colors.DropdownBorderColor );
					break;
				}
				case VisualStyle.VS2005:
				{
					gridBorder = new GridBorder( GridBorderStyle.Solid, VS2005Colors.MenuBorderColor );
					break;
				}
				case VisualStyle.Office2007Outlook:
				{
					gridBorder = new GridBorder( GridBorderStyle.Solid, Office2007OutlookColors.MenuBorderColor );
					break;
				}
				default:
				{
					gridBorder = new GridBorder( GridBorderStyle.Solid, MenuColors.DropDownBorderColor );
					break;
				}
			}

			return gridBorder;
		}

	}
}
