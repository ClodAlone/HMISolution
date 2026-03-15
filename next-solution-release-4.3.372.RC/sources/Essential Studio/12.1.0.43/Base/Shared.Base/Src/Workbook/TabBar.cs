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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms
{
	/// <summary>
	///     A control for creating Tabbed Dialogs or Excel workbook type
	///     windows. This control provides a row of tabs that the
	///     user can select from one at a time. After each selection, a notification
	///     is provided that allows for changing of UI.
	///     The tab bar can share the row with an associated scrollbar. The
	///     user can adjust the width of the tab bar and increase the scrollbar's
	///     size by dragging a splitter found in the middle of the row.
	/// </summary>
	[
		//EditorAttribute(typeof(System.Windows.Forms.Design.ParentControlDesigner), typeof(System.Drawing.Design.UITypeEditor)),
		ToolboxItem( false ),
		DefaultProperty( "Text" ),
		DefaultEvent( "SelectedIndexChanged" ),
		Designer( typeof( Syncfusion.Windows.Forms.TabBarDesigner ),
			typeof( System.ComponentModel.Design.IDesigner ) )
	]
	public class TabBar: ArrowButtonBar,
		IInternalTabParent,
		IInternalTabBarParent,
		IInternalButtonParent,
		IInternalSplitterParent,
		IScrollBarContainer
	{

        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);




		// Events

		/// <summary>
		/// Occurs after the selected tab index has changed.
		/// </summary>
		public event SelectedIndexEventHandler SelectedIndexChanged;


		/// <summary>
		/// Occurs before the selected tab index is changing.
		/// </summary>
		public event SelectedIndexEventHandler SelectedIndexChanging;

		private InternalTabBar tabBar = null;

		// IInternalTabParent MeasureTools cache:
		private Brush tabBrush = SystemBrushes.Control;
		private Font boldFont = null;
		private Font underlineFont = null;
		private int tabFolderDelta = 6;
		private Graphics tempGraphics = null;
		private Graphics paintGraphics = null;

		// Cursor for dragging tabs:
		private Cursor overrideTabCursor = null;

		// Splitter bar:
		private Cursor overrideSplitCursor = null;
		private InternalSplitter splitter = null;
		private int splitPos = 80;  // 80%


		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal Control scrollBar = null; // shared Control

		ImageList imageList = null;
		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal int selectedIndex = -1;
		private bool scrolled = false;
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

		/// <overload>
		/// Initializes a new <see cref="TabBar"/>.
		/// </overload>
		/// <summary>
		/// Initializes a new <see cref="TabBar"/>.
		/// </summary>
		public TabBar()
		{
		}

		/// <summary>
		/// Initializes a new <see cref="TabBar"/> with a scrollbar.
		/// </summary>
		/// <param name="scrollBar">The scrollbar to be displayed to the right of the tab bar.</param>
		public TabBar( Control scrollBar )
		{
			this.scrollBar = scrollBar;
            CTRLSIZE.Height = 17;
		}
        #region For Touch

        bool isScaling = false;

        bool _touchMode = false;
        /// <summary>
        /// Gets or sets value to enable or disable the Touchmode to the controls.
        /// </summary>
        /// <remarks>Scale factor will be updated automatically if scalefactor is equal to 1</remarks>
        [Browsable(true),DefaultValue(false),
        Category("Layout"), Description("Gets or sets value to enable or disable the Touchmode to the controls."),
    ]
        public bool EnableTouchMode
        {
            get
            {
                return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }

        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }

        /// <summary>
        /// Scale the control based on the scale factor passed in the argument.
        /// </summary>
        /// <param name="scaleFactor">value to scale the factor based upon.</param>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.Height = (int)(CTRLSIZE.Height * scaleFactor);
            isScaling = false;
            this.ResumeLayout();
            this.Refresh();
            this.Invalidate();
        }
        /// <summary></summary>
        /// <param name="e"/>
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
            if (!EnableTouchMode && CTRLSIZE != this.Size)
            {
                CTRLSIZE = this.Size;
            }
        }
        #endregion
        Control IScrollBarContainer.ScrollBar
		{
			get
			{
				return this.scrollBar;
			}
			set
			{
				if( value != this.scrollBar )
				{
					Controls.Remove( this.scrollBar );
					this.scrollBar = value;
					Controls.Add( this.scrollBar );
				}
			}
		}

		// IInternalButtonParent interface:

		void IInternalButtonParent.OnClickedButton( InternalButton button )
		{
			InternalTabBar tabBar = TabBarChild;
			InternalArrowButton arrow = button as InternalArrowButton;
			if( arrow != null )
			{
				if( tabBar.ScrollBehavior == InternalTabBarScrollBehavior.ScrollPixels )
					tabBar.Scroll( arrow.Type, 4 );
				else
					tabBar.ScrollTab( arrow.Type );

				EnableButtonFlags = tabBar.EnableButtonFlags;
			}
			else
			{
				InternalTab tab = button as InternalTab;
				OnClickedTab( tab );
			}
		}

		/// <override/>
		protected override void OnMouseDown( MouseEventArgs e )
		{
			scrolled = false;
#if DEBUG
			if( Switches.TabBarEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name, e.X, e.Y, e.Button, e.Clicks );
#else
			;
#endif

			base.OnMouseDown( e );
		}

		/// <summary>
		/// Occurs when the user clicks on a tab.
		/// </summary>
		/// <param name="tab">The tab that was clicked.</param>
		/// <remarks>
		/// Sets the <see cref="SelectedIndex"/>.
		/// </remarks>
		protected virtual void OnClickedTab( InternalTab tab )
		{
			if( tab != null )
			{
				SelectedIndex = Tabs.IndexOf( tab );
				// Avoid dragging when tab was scrolled into view.
				if( scrolled )
					OnCancelMode( EventArgs.Empty );
			}
		}

		// IInternalTabParent interface:

		void IInternalTabParent.DisposeMeasureTools()
		{
			if( tempGraphics != null )
			{
				tempGraphics.Dispose();
				tempGraphics = null;
			}

			if( boldFont != null )
			{
				boldFont.Dispose();
				boldFont = null;
			}
		}


		void IInternalTabParent.GetMeasureTools( out Graphics g, out Font font, out int delta )
		{
			if( paintGraphics != null )
				g = paintGraphics;
			else if( tempGraphics != null )
				g = tempGraphics;
			else
				g = tempGraphics = CreateGraphics();  // RecalcLayout will Dispose this one.

			// REVIEW: what happens when font is disposed in AdjustSize?
			if( boldFont == null )
				boldFont = FontUtil.CreateFont( Font, FontStyle.Bold );
			font = boldFont;

			delta = tabFolderDelta;
		}


		// TODO: Color properties
		/*
		Color disabledTextColor = SystemColors.GrayText;
		Color disabledBackColor = SystemColors.Control;
			
		Color selectedTabBackColor = SystemColors.Window;
		Color selectedTabForeColor = SystemColors.WindowText;
		
		Color inactiveTabBackColor = SystemColors.Control;
		Color inactiveTabForeColor = SystemColors.WindowText;
		
		Color dragTabBackColor = SystemColors.Highlight;
		Color dragTabForeColor = SystemColors.HighlightText;
		
		Color checkedTabBackColor = SystemColors.Window;
		Color checkedTabForeColor = SystemColors.HighlightText;
		*/

        void IInternalTabParent.GetDrawingTools(InternalTab tab, out Brush fillBrush, out Color textColor, out Font font, out int delta)
        {
            TabBarPage tabPage = null;
            SolidBrush tabButtonBrush = null;

            if (tab != null && tab.Cookie != null && (tab.Cookie as TabBarPage) != null)
            {
                tabPage = tab.Cookie as TabBarPage;
                tabButtonBrush = new SolidBrush(tabPage.TabBackColor);
            }
            tabPage = tab.Cookie as TabBarPage;
            if (!tab.Enabled)
                textColor = SystemColors.GrayText;
            else
                textColor = tabPage == null ? ForeColor : tabPage.ForeColor;


            if (!tabPage.TabEnabled)
            {
                fillBrush = SystemBrushes.Control;
                textColor = SystemColors.GrayText;
                font = tabPage.Font;
                delta = tabFolderDelta;

            }
            else
            {
                textColor = tabPage == null ? ForeColor : tabPage.ForeColor;
                if (tab.Pushed)
                {
                    fillBrush = SystemBrushes.Window;
                    if (boldFont == null)
                        boldFont = FontUtil.CreateFont(Font, FontStyle.Bold);
                    font = boldFont;
                }
                else if (tab.Hovered)
                {
                    fillBrush = tabBrush;
                    if (underlineFont == null)
                        underlineFont = FontUtil.CreateFont(Font, FontStyle.Underline);
                    font = underlineFont;
                }
                else
                {
                    font = Font;

                    if (tabPage != null && !tabPage.ThemesEnabled)
                    {
                        fillBrush = tabButtonBrush;
                    }
                    else
                    {
                        fillBrush = tabBrush;
                    }
                }

                if (tab.DragTarget)
                {
                    fillBrush = SystemBrushes.Highlight;
                    textColor = SystemColors.HighlightText;
                }
                else if (tab.Checked)
                    fillBrush = SystemBrushes.Window;

                delta = tabFolderDelta;
            }
        }

		// IInternalTabBarParent interface:

		bool IInternalTabBarParent.OnScroll( ArrowType arrowType, int pixels )
		{
			return true;
		}

		void IInternalTabBarParent.OnScrolled()
		{
			scrolled = true;
			EnableButtonFlags = TabBarChild.EnableButtonFlags;
		}

		/// <summary>
		/// Occurs when a tab is dragged by the user. You can set e.Cancel = True
		/// to prevent dragging a tab.
		/// </summary>
		/// <example>
		/// <code lang="C#">
		/// public Form1()
		/// {
		/// 	InitializeComponent();
		/// 
		/// 	this.tabControl1.Bar.DraggingTab += new TabMovedEventHandler(Bar_DraggingTab);
		/// }
		/// 
		/// private void Bar_DraggingTab(object sender, TabMovedEventArgs e)
		/// {
		/// 	Console.WriteLine("Bar_DraggingTab {0}, {1}", e.Tab, e.DestTab);
		/// 	e.Cancel = true; // Do not allow dragging the tab
		/// }</code>
		/// </example>
		public event TabMovedEventHandler DraggingTab;

		/// <summary>
		/// Raises the <see cref="DraggingTab"/> event.
		/// </summary>
		/// <param name="e">A <see cref="TabMovedEventArgs" /> that contains the event data.</param>
		protected virtual void OnDraggingTab( TabMovedEventArgs e )
		{
			if( DraggingTab != null )
				DraggingTab( this, e );
		}


		bool IInternalTabBarParent.OnDraggingTab( int nTab, int nTarget )
		{
			TabMovedEventArgs e = new TabMovedEventArgs( nTab, nTarget );
			OnDraggingTab( e );
			if( e.Cancel )
				return false;

			InternalTabBar tabBar = TabBarChild;
			tabBar.IsDragTabMode = true;
			return true;
		}

		void IInternalTabBarParent.OnDraggedTab( int nTab, int nTarget )
		{
			InternalTabBar tabBar = TabBarChild;
			tabBar.IsDragTabMode = false;
			selectedIndex = -1;
		}

		Cursor IInternalTabBarParent.OverrideCursor
		{
			get
			{
				return overrideTabCursor;
			}
			set
			{
				if( overrideTabCursor != value )
				{
					overrideTabCursor = value;

					if( !DesignMode && IsHandleCreated )
					{
						NativeMethods.SendMessage( Handle, NativeMethods.WM_SETCURSOR, (int)Handle, NativeMethods.HTCLIENT );
					}
				}
			}
		}

		// IInternalSplitterParent interface:

		/// <summary>
		/// Occurs when the users drags the splitbar.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="x">The current horizontal position in pixels.</param>
		/// <param name="y">The current vertical position in pixels.</param>
		public virtual void OnMoveSplitter( object sender, int x, int y )
		{
			if( sender == splitter )
			{
				Rectangle rect = ComputeTabBarBounds();
				int delta = x*100/rect.Width;
				if( this.RightToLeft == RightToLeft.Yes )
					delta = -delta;
				RelativeWidth = Math.Min( 95, Math.Max( 5, RelativeWidth + delta ) );
				Update();
			}
		}

		/// <summary>
		/// Occurs after the user moved the splitter bar.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		public virtual void OnMovedSplitter( object sender )
		{
			if( sender == splitter )
			{
				OnRelativeWidthChanged( null );
			}
		}


		/// <summary>
		/// Occurs when the <see cref="RelativeWidth"/> property has changed.
		/// </summary>
		public event EventHandler RelativeWidthChanged;

		/// <summary>
		/// Raises the <see cref="RelativeWidthChanged"/> event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected virtual void OnRelativeWidthChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.TabBarEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name, this.RelativeWidth );
#else
			;
#endif

			if( RelativeWidthChanged != null )
				RelativeWidthChanged( this, e );
		}


		/// <summary>
		/// Repaints the splitter bar.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		public virtual void InvalidateSplitter( object sender )
		{
			//
		}

		//
		Cursor IInternalSplitterParent.OverrideCursor
		{
			get
			{
				return overrideTabCursor;
			}
			set
			{
				if( overrideSplitCursor != value )
				{
					overrideSplitCursor = value;

					if( IsHandleCreated )
					{
						NativeMethods.SendMessage( Handle, NativeMethods.WM_SETCURSOR, (int)Handle, NativeMethods.HTCLIENT );
					}
				}
			}
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual InternalSplitter OnCreateInternalSplitter()
		{
			return new InternalSplitter( this, InternalSplitterKind.HorizontalBar );
		}

		// Cursor

		/// <summary>
		///     Handles the WM_SETCURSOR message.
		/// </summary>
		/// <internalonly/>
		private void WmSetCursor( ref Message m )
		{

			// Accessing through the Handle property has side effects that break this
			// logic. You must use InternalHandle.
			//
			if( m.WParam == Handle && ( (int)m.LParam & 0x0000FFFF ) == NativeMethods.HTCLIENT )
			{
				OnSetCursor( ref m );
			}
			else
			{
				DefWndProc( ref m );
			}

		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnSetCursor( ref Message m )
		{
			if( overrideSplitCursor != null )
			{
				Cursor.Current = overrideSplitCursor;
			}
			else if( overrideTabCursor != null )
				Cursor.Current = overrideTabCursor;
			else
				Cursor.Current = Cursor;
		}

		/// <override/>
		[SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode=true )]
		protected override void WndProc( ref Message msg )
		{
			if( DesignMode )
				base.WndProc( ref msg );
			else
			{
				switch( msg.Msg )
				{
					case NativeMethods.WM_SETCURSOR:
					WmSetCursor( ref msg );
					break;
					default:
					base.WndProc( ref msg );
					break;
				}
			}
		}

		/// <override/>
		protected override void OnPaint( PaintEventArgs pe )
		{
			shouldPaintButtonBar = false;
			base.OnPaint( pe );

			lock( this )
			{
				InternalTabBar tabBar = TabBarChild;
				tabBar.Dirty = true;

				// Tabs
				paintGraphics = pe.Graphics;
				try
				{
					if( tabBar.Bounds.IntersectsWith( pe.ClipRectangle ) )
					{
						Region clip = pe.Graphics.Clip;
						pe.Graphics.IntersectClip( tabBar.Bounds );
						pe.Graphics.FillRectangle( SystemBrushes.ScrollBar, tabBar.Bounds );
						tabBar.Paint( pe.Graphics, tabFolderDelta );
						pe.Graphics.Clip = clip;
						pe.Graphics.ExcludeClip( tabBar.Bounds );
					}

					if( splitter != null && splitter.Bounds.IntersectsWith( pe.ClipRectangle ) )
					{
						splitter.Style = this.Style;
						splitter.Paint( pe.Graphics, ButtonLook == ButtonLook.Flat );
						pe.Graphics.ExcludeClip( splitter.Bounds );
					}

					if( scrollBar != null && scrollBar.Visible
                        && RightToLeft != RightToLeft.Yes && scrollBar.Bounds.Right < ClientRectangle.Right )
					{
						Rectangle scrollBounds = scrollBar.Bounds;
						Rectangle gripBounds = new Rectangle(
							scrollBounds.Right,
							scrollBounds.Top,
							ClientRectangle.Right-scrollBounds.Right,
							scrollBounds.Height );

						ControlPaint.DrawSizeGrip( pe.Graphics, SystemColors.ScrollBar, gripBounds );
						pe.Graphics.ExcludeClip( gripBounds );
					}

					// ArrowButtons
					ButtonBarChild.Paint( pe.Graphics );
				}
				finally
				{
					paintGraphics = null;
					( (IInternalTabParent)this ).DisposeMeasureTools();
					tabBar.Dirty = false;
				}
			}
		}

		/// <override/>
		public override void ResetToolTips()
		{
			if( tabBar != null )
				tabBar.ResetToolTips();

			if( splitter != null )
				splitter.ResetToolTip();

			base.ResetToolTips();
		}

		/// <override/>
		protected override void OnLayout( LayoutEventArgs levent )
		{
			if( Size.IsEmpty )
				return;
#if DEBUG

			if( Switches.TabBarEvents.TraceVerbose )

				TraceUtil.TraceCurrentMethodInfo( Name, levent.AffectedControl, levent.AffectedProperty );
#else

			;
#endif

			base.OnLayout( levent );

			InternalTabBar tabBar = TabBarChild;
			InternalButtonBar buttonBar = ButtonBarChild;
			Rectangle clientRect = ComputeTabBarBounds();

			if( clientRect.IsEmpty )
				return;

			BeginUpdate();
			ResetToolTips();

			Rectangle buttonBarRect = buttonBar.Bounds;

			tabBar.Bounds = ComputeTabBarChildBounds( buttonBarRect );
			tabBar.AdjustSize( true, false );

			if( tabBar.CurrentTab > tabBar.Tabs.Count )
			{
				selectedIndex = tabBar.CurrentTab;
				tabBar.CurrentTab = -1;
			}
			else if( selectedIndex != -1 && selectedIndex < tabBar.Tabs.Count )
			{
				tabBar.CurrentTab = selectedIndex;
			}

			// tabBar.ScrollInView(Tabs[tabBar.CurrentTab]);

			Rectangle tabBounds = tabBar.Bounds;

			if( Resizable )
			{
				splitter.Bounds = new Rectangle( tabBounds.Right, clientRect.Top, 7, clientRect.Height );
				if( scrollBar != null )
				{
					Rectangle bounds = new Rectangle( splitter.Bounds.Right, clientRect.Top, clientRect.Width-splitter.Bounds.Right, clientRect.Height );
					bounds = ReverseRectangleRTL( bounds );
					scrollBar.Bounds = bounds;
				}
			}
			else if( scrollBar != null )
			{
				Rectangle bounds = new Rectangle( tabBounds.Right, clientRect.Top, clientRect.Width-tabBar.Bounds.Right, clientRect.Height );
				bounds = ReverseRectangleRTL( bounds );
				scrollBar.Bounds = bounds;
			}
			if( splitter != null )
			{
				splitter.Bounds = ReverseRectangleRTL( splitter.Bounds );
				if( this.RightToLeft == RightToLeft.Yes )
				{
					tabBar.Bounds = new Rectangle( splitter.Bounds.Right, tabBar.Bounds.Top, this.ClientRectangle.Width-this.ButtonBarChild.Bounds.Width-splitter.bounds.Right, tabBar.Bounds.Height );
					this.ButtonBarChild.Bounds = new Rectangle( tabBar.Bounds.Right, tabBar.Bounds.Top, this.ButtonBarChild.Bounds.Width, tabBar.Bounds.Height );
				}
			}

			EnableButtonFlags = tabBar.EnableButtonFlags;

			EndUpdate( false );
			buttonBar.InvalidateIfDirty();
			Rectangle r = new Rectangle( tabBounds.Right, clientRect.Top, clientRect.Width-tabBounds.Width, clientRect.Height );
			if( this.RightToLeft == RightToLeft.Yes )
				r = this.ClientRectangle;
			Invalidate( r );
			tabBar.InvalidateIfDirty();
		}

		new Rectangle ReverseRectangleRTL( Rectangle r )
		{
			if( this.RightToLeft == RightToLeft.Yes )
				return new Rectangle( this.ClientRectangle.Right - r.Right, r.Top, r.Width, r.Height );
			return r;
		}


		/// <override/>
		protected override void OnHandleCreated( EventArgs e )
		{
			if( scrollBar != null )
			{
				if( !Controls.Contains( scrollBar ) )
					Controls.Add( scrollBar );
				if( !scrollBar.IsHandleCreated )
					scrollBar.CreateControl();
			}
#if DEBUG
			if( Switches.TabBarEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name, Handle );
#else
			;
#endif

			base.OnHandleCreated( e );
		}

		/// <summary>
		/// Calculates the boundaries of the tab bar (both tabs and arrows) within the current control.
		/// </summary>
		/// <returns>The boundaries of the tab bar.</returns>
		/// <remarks>
		/// Will return <see cref="Control.ClientRectangle"/> unless overriden in derived class.
		/// </remarks>
		protected virtual Rectangle ComputeTabBarBounds()
		{
			return ClientRectangle;
		}

		/// <summary>
		/// Calculates the boundaries of the tab bar child (without arrow bar) within the current control.
		/// </summary>
		/// <param name="buttonBarRect">The size of the tab bar.</param>
		/// <returns>The boundaries of the inner tab bar.</returns>
		protected virtual Rectangle ComputeTabBarChildBounds( Rectangle buttonBarRect )
		{
			Rectangle tabBarRect = ComputeTabBarBounds();
			tabBarRect.X = buttonBarRect.Right;

			if( Resizable )
				tabBarRect.Width = ( tabBarRect.Width-buttonBarRect.Right )*RelativeWidth/100;
			else
				tabBarRect.Width -= buttonBarRect.Right;

			return tabBarRect;
		}

		// Arrow bar:
		/// <summary>
		/// Calculates the boundaries of the arrow bar within the current control.
		/// </summary>
		/// <returns>The boundaries of the arrow bar.</returns>
		protected override Rectangle ComputeButtonBarChildBounds()
		{
			Rectangle clientRect = ComputeTabBarBounds();

			int x = clientRect.Height;
			if( x > 20 ) x = 20;
			Size buttonSize = new Size( x, clientRect.Height );

			Rectangle arrowBarRect = clientRect;
			arrowBarRect.Width = x*ButtonBarChild.Buttons.Length;

			return arrowBarRect;
		}

		/// <override/>
		public override void Refresh()
		{
			if( Updating || Size.IsEmpty )
				return;

			InternalTabBar tabBar = TabBarChild;
			if( selectedIndex != -1 && selectedIndex < tabBar.Tabs.Count )
			{
				tabBar.CurrentTab = selectedIndex;
			}
			tabBar.Dirty = true; // force all buttons to redraw
			//MessageBox.Show("TabBar.Refresh");
			PerformLayout();
			base.Refresh();
		}

		/// <summary>
		/// Creates the inner tab bar that displays the tabs.
		/// </summary>
		/// <returns>The <see cref="InternalTabBar"/> for the inner tabs.</returns>
		protected virtual InternalTabBar OnCreateTabBarChild()
		{
			InternalTabBar tb = new InternalTabBar( this );
			tb.FlatLook = this.ButtonLook == ButtonLook.Flat;
			return tb;
		}

		/// <override/>
		protected override void OnButtonLookChanged( EventArgs e )
		{
			TabBarChild.FlatLook = this.ButtonLook == ButtonLook.Flat;
			TabBarChild.Dirty = true;
#if DEBUG
			if( Switches.TabBarEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( this.ButtonLook );
#else
			;
#endif

			base.OnButtonLookChanged( e );
		}

		// Properties

		/// <summary>
		/// Gets / sets the delta used for drawing the tabs.
		/// </summary>
		[
			Browsable( true ),
			//Category("TabBar"),
			Description( "Delta used for drawing the tabs." ),
			DefaultValue( 6 )
		]
		public int TabFolderDelta
		{
			get
			{
				return tabFolderDelta;
			}
			set
			{
				if( tabFolderDelta != value )
				{
					tabFolderDelta = value;
					Refresh();
				}
			}
		}

		/// <summary>
		/// Gets / sets the scroll behavior of this tab bar: pixel or tabs.
		/// </summary>
		[
			Browsable( true ),
			//Category("ScrollButtons"),
			Description( "Scrolling tabs by pixel or whole tabs." ),
			DefaultValue( InternalTabBarScrollBehavior.ScrollPixels )
		]
		public InternalTabBarScrollBehavior ScrollBehavior
		{
			get
			{
				return TabBarChild.ScrollBehavior;
			}
			set
			{
				TabBarChild.ScrollBehavior = value;
			}
		}

		/// <summary>
		/// Gets / sets the inner tab bar that displays the tabs.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public InternalTabBar TabBarChild
		{
			get
			{
				if( tabBar == null )
				{
					tabBar = OnCreateTabBarChild();
					tabBar.Dirty = true;
				}

				return tabBar;
			}

			set
			{
				if( tabBar != value )
				{
					tabBar = value;
                    if( tabBar != null )
						tabBar.Dirty = true;
					PerformLayout();
				}
			}
		}

		/// <override/>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public override ISite Site
		{
			get
			{
				return base.Site;
			}
			set
			{
				ISite iSite = Site;
				IContainer iContainer;
				InternalTabCollection tabItems = TabBarChild.Tabs;
				if( value == null && iSite != null )
				{
					iContainer = iSite.Container;
					if( iContainer != null )
					{
						foreach( InternalTab tab in tabItems )
						{
							if( tab != null && tab.Site != null )
								iContainer.Remove( (IComponent)tab );
						}
						base.Site = value;
					}
				}
				else if( value != null && iSite == null )
				{
					base.Site = value;
					iContainer = value.Container;
					if( iContainer != null )
					{
						foreach( InternalTab tab in tabItems )
						{
							if( tab != null && tab.Site == null )
								iContainer.Add( (IComponent)tab );
						}
					}
				}
			}
		}

		/// <summary>
		/// Gets / sets the width of the tab bar relative to the width of the client bounds.
		/// </summary>
		[
			Browsable( true ),
			//Category("TabBar"),
			Description( "Specify the width of the tab bar relative to the width of client bounds." ),
			DefaultValue( 80 )
		]
		public int RelativeWidth
		{
			get
			{
				return splitPos;
			}
			set
			{
				if( splitPos != value )
				{
					splitPos = value;
					PerformLayout();
				}
			}
		}

		bool inSelectedIndex = false;

		/// <summary>
		///     Gets / sets the index of the currently selected tab in the strip, if there
		///     is one. If the value is -1, there is currently no selection. If the
		///     value is 0 or greater, then the value is the index of the currently
		///     selected tab.
		/// </summary>
		[
			Browsable( true ),
			//Category("TabBar"),
			DefaultValue( false )
		]
		public int SelectedIndex
		{
			get
			{
				return selectedIndex;
			}
			set
			{
				if( value < -1 )
					throw new ArgumentException( SR.GetString( @"InvalidLowBoundArgumentEx", @"value", value.ToString(), @"-1" ) );

				bool noEvents = inSelectedIndex;
				inSelectedIndex = true;
				try
				{

					InternalTab tab = null;
					if( TabBarChild.Tabs != null && value >= 0 && value < TabBarChild.Tabs.Count )
						tab = TabBarChild.Tabs[value];

					if( !IsHandleCreated && selectedIndex != value )
					{
						if( noEvents || OnSelectedIndexChanging( value, tab ) )
						{
							selectedIndex = value;
							if( !noEvents )
								OnSelectedIndexChanged( value, tab );
						}

					}
					else if( selectedIndex != value || TabBarChild.CurrentTab != value )
					{
						BeginUpdate();
						if( !noEvents && !OnSelectedIndexChanging( value, tab ) )
							OnCancelMode( EventArgs.Empty );
						else
						{
							// If Selected Index is out of bounds, we'll cache it in selectedIndex.
							if( TabBarChild.Tabs != null && value >= 0 && value < TabBarChild.Tabs.Count )
							{
								TabBarChild.CurrentTab = value;
								selectedIndex = value; // -1
							}
							else
							{
								selectedIndex = value;
								TabBarChild.CurrentTab = -1;
							}

							EnableButtonFlags = tabBar.EnableButtonFlags;

							TabBarChild.InvalidateIfDirty();

							if( !noEvents )
								OnSelectedIndexChanged( TabBarChild.CurrentTab, tab );
						}
						EndUpdate( true );
						TabBarChild.RefreshCurrentTab( true );
					}
				}
				finally
				{
					inSelectedIndex = false;
				}
			}
		}

		/// <summary>
		/// Enables / disables the resizing of tab bar.
		/// </summary>
		[
			Browsable( true ),
			//Category("TabBar"),
			Description( "Enable or Disable resizing the tab bar." ),
			DefaultValue( false )
		]
		public bool Resizable
		{
			get
			{
				return splitter != null;
			}
			set
			{
				if( value != Resizable )
				{
					if( !value )
						splitter = null;
					else
						splitter = OnCreateInternalSplitter();

					PerformLayout();
				}
			}
		}

		/// <summary>
		/// Returns the list of tabs displayed in the tab bar.
		/// </summary>
		[
			//Category("TabBar"),
			DescriptionAttribute( "List of tabs displayed in tab bar." ),
			DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
			// EditorAttribute(typeof(System.ComponentModel.Design.CollectionEditor), typeof(System.Drawing.Design.UITypeEditor)),
		]
		public InternalTabCollection Tabs
		{
			get
			{
				return TabBarChild.Tabs;
			}
		}

		/// <summary>
		/// Gets or sets the visual style of the tabBar.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies the style with which tabBar will appear." )]
		[DefaultValue( TabBarSplitterStyle.Default )]
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
					this.PerformLayout();
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnStyleChanged()
		{
			m_office2007ColorTable = Office2007Colors.GetColorTable( m_colorScheme );

			foreach( InternalArrowButton button in this.ButtonBarChild.Buttons )
			{
				button.Style = this.Style;
				button.Office2007ColorScheme = this.Office2007ColorScheme;
			}
			foreach( InternalTab tab in this.Tabs )
			{
				tab.Style = this.Style;
				tab.Office2007ColorScheme = this.Office2007ColorScheme;
			}
			if( splitter != null )
			{
				this.splitter.Style = this.Style;
				this.splitter.Office2007ColorScheme = this.Office2007ColorScheme;
			}
		}

		/// <summary>
		/// Gets or sets office 2007 color scheme.
		/// </summary>
		[Description( "Gets or sets office 2007 color scheme." )]
		[Category( "Appearance" )]
		[DefaultValue( Office2007Theme.Blue )]
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
					Office2007Colors.Default :	m_office2007ColorTable;

				return colorTable;
			}
		}

		/// </override>
		public override DisplayArrowButtons DisplayArrowButtons
		{
			get
			{
				return base.DisplayArrowButtons;
			}
			set
			{
				if( base.DisplayArrowButtons != value )
				{
					base.DisplayArrowButtons = value;
					foreach( InternalArrowButton button in this.ButtonBarChild.Buttons )
					{
						button.Style = this.Style;
						button.Office2007ColorScheme = this.Office2007ColorScheme;
					}
				}
			}
		}

		// SelectedIndexChangedEvent 

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void RaiseSelectedIndexChangedEvent( SelectedIndexEventArgs e )
		{
#if DEBUG
			if( Switches.TabBarEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name, e );
#else
			;
#endif

			if( SelectedIndexChanged != null )
			{
				SelectedIndexChanged( this, e );
			}
		}

		// SelectedIndexChangingEvent 

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void RaiseSelectedIndexChangingEvent( SelectedIndexEventArgs e )
		{
#if DEBUG
			if( Switches.TabBarEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name, e );
#else
			;
#endif

			if( SelectedIndexChanging != null )
			{
				SelectedIndexChanging( this, e );
			}
		}

		/// <summary>
		///     Finds and returns the tab that holds the specified item as cookie.
		/// </summary>
		public int FindItem( object item )
		{
			return TabBarChild.FindTab( item );
		}

		/// <summary>
		///     Removes the tab that holds the specified item as cookie.
		/// </summary>
		public void RemoveItem( object item )
		{
			int index = TabBarChild.FindTab( item );
			if( index != -1 )
			{
				Tabs.Remove( Tabs[index] );
			}
		}

		internal int FindItemExact( object item )
		{
			return TabBarChild.FindTab( item );
		}

		internal object GetItem( int index )
		{
			if( index < 0 || index >= TabBarChild.Tabs.Count )
				throw new ArgumentException( SR.GetString( @"InvalidArgument", @"index", index.ToString() ) );

			InternalTab tab = TabBarChild.Tabs[index];
			if( tab != null )
				return tab.Cookie;

			return null;
		}

		/// <override/>
		protected override void OnFontChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.TabBarEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Font );
#else
			;
#endif

			base.OnFontChanged( e );

			PerformLayout();
		}

		/// <summary>
		/// Removes all tabs.
		/// </summary>
		protected void RemoveAll()
		{
			Controls.Clear();
			TabBarChild.Tabs.Clear();
		}

		internal void RemoveItem( int index )
		{
			int itemCount = TabBarChild.Tabs.Count;
			if( index < 0 || index >= itemCount )
				throw new ArgumentException( SR.GetString( @"InvalidArgument", @"index", index.ToString() ) );

			TabBarChild.Tabs.Remove( TabBarChild.Tabs[index] );
		}


		internal void UpdateSize()
		{
			PerformLayout();
		}


		/// <summary>
		///     Returns the imageList the control points at. This is where tabs that have imageIndex
		///     set will get their images from.
		/// </summary>
		/// <returns>
		///     An image list control.
		/// </returns>
		[
			DefaultValue( null ),
		]
		public ImageList ImageList
		{
			get
			{
				return imageList;
			}
			set
			{
				if( imageList != value )
				{
					imageList = value;
					OnImageListChanged( EventArgs.Empty );
				}
			}
		}

		/// <summary>
		/// Occurs when the <see cref="ImageList"/> has changed.
		/// </summary>
		public event EventHandler ImageListChanged;


		/// <summary>
		/// Raises the <see cref="ImageListChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="System.EventArgs" /> that contains the event data.</param>
		protected virtual void OnImageListChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.TabBarEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name, ImageList );
#else
			;
#endif

			if( ImageListChanged != null )
				ImageListChanged( this, e );
		}



		/// <summary>
		/// Gets / sets the cookie of the current selected tab or sets the tab with the specified cookie.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public object SelectedItem
		{
			get
			{
				int n = SelectedIndex;
				if( n == -1 || n >= TabCount )
					return null;
				else
					return GetItem( n );
			}
			set
			{
				int n = FindItem( value );
				SelectedIndex = n;
			}
		}

		/// <summary>
		///     Indicates whether ToolTips are being shown for tabs that have ToolTips set on
		///     them.
		/// </summary>
		[
			DefaultValue( false ),
			//Category("TabBar")
		]
		public new bool ShowToolTips
		{
			get
			{
				return base.ShowToolTips;
			}
			set
			{
				base.ShowToolTips = value;
			}
		}

		/// <summary>
		///     Returns the number of tabs in the strip.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public int TabCount
		{
			get
			{
				return TabBarChild.Tabs.Count;
			}
		}

		/// <override/>
		[Browsable( false )]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}

		/// <summary>
		/// Raises the <see cref="SelectedIndexChanging"/> event.
		/// </summary>
		/// <param name="index">The new tab index.</param>
		/// <param name="tab">The object for the tab.</param>
		/// <returns>False if cancelled; True if successful.</returns>
		protected virtual bool OnSelectedIndexChanging( int index, InternalTab tab )
		{
			// MessageBox.Show(String.Format("OnSelectedIndexChanged({0})", args));
			SelectedIndexEventArgs eventArgs = new SelectedIndexEventArgs( index, tab );
			RaiseSelectedIndexChangingEvent( eventArgs );
			return eventArgs.Cancel == false;
		}

		/// <summary>
		/// Raises the <see cref="SelectedIndexChanged"/> event.
		/// </summary>
		/// <param name="index">The new tab index.</param>
		/// <param name="tab">The object for the tab.</param>
		protected virtual void OnSelectedIndexChanged( int index, InternalTab tab )
		{
			// MessageBox.Show(String.Format("OnSelectedIndexChanged({0})", args));
			SelectedIndexEventArgs eventArgs = new SelectedIndexEventArgs( index, tab );
			RaiseSelectedIndexChangedEvent( eventArgs );
		}

		/// <override/>
		protected override void OnCancelMode( EventArgs e )
		{
			if( this.tabBar != null )
				this.tabBar.CancelMode();
#if DEBUG
			if( Switches.TabBarEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name );
#else
			;
#endif

			base.OnCancelMode( e );
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( this.splitter != null )
				{
					this.splitter.Dispose();
					this.splitter = null;
				}
                if( this.tabBar != null )
                {
                    this.tabBar.Dispose();
                    this.tabBar = null;
                }
			}

			base.Dispose( disposing );
		}
	}
}
