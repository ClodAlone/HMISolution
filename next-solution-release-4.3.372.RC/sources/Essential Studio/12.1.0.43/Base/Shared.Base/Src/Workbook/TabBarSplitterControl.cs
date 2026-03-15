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
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms
{
    # region Delegates & EventArgs classes

    public delegate void TabBarVisibleChangedEventHandler(Object sender, TabBarVisibleChangedEventArgs arg);

    public class TabBarVisibleChangedEventArgs : EventArgs
    {
        [Syncfusion.Documentation.DocumentationExclude()]
        protected TabBarPage tabBar = null;
        [Syncfusion.Documentation.DocumentationExclude()]  
        
        public  TabBarPage TabBarpage
        {
            get { return this.tabBar; }
            
        }

        public TabBarVisibleChangedEventArgs()
        { }

        public TabBarVisibleChangedEventArgs(TabBarPage ctrl, bool visible)
		{
            this.tabBar = ctrl;
		}
    }
    #endregion
    /// <summary>
	/// Provides functionality for displaying several <see cref="TabBarPage"/> controls
	/// in an Excel-like workbook control. Each <see cref="TabBarPage"/> might optionally
	/// support splitting its view with a dynamic splitter frame.
	/// </summary>
	[
	ToolboxItem( true ),
	Designer( typeof( Syncfusion.Windows.Forms.TabBarSplitterControlDesigner ),
		typeof( System.ComponentModel.Design.IDesigner ) ),
	]
	[System.Drawing.ToolboxBitmap( typeof( Syncfusion.Windows.Forms.TabBarSplitterControl ), "ToolboxIcons.TabBarSplitterControl.bmp" )]
	[Description( "Provides functionality for displaying several TabBarPage controls in an Excel-like workbook control" )]
	public class TabBarSplitterControl: SplitterControl, INonClientPaintingSupport,IVisualStyle 
	{
		TabBar bar = null;
		TabBarPageCollection tabPages = null;
		TabBarPage activePage = null;
		int activePageIndex = 0;
		bool showIcons = true;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);



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
		private ControlDrawing cd = new ControlDrawing();

		/// <summary>
		/// Occurs after the active page has changed.
		/// </summary>
		[Description( "Occurs after the active page has changed." )]
		public event ControlEventHandler ActivePageChanged;

		/// <summary>
		/// Occurs before the active page is changed.
		/// </summary>
		[Description( "Occurs before the active page is changed." )]
		public event ControlEventHandler ActivePageChanging;

        /// <summary>
        /// Occurs when the TabBarPage is show or Hide or Add or Remove.
        /// </summary>
        [Description("Occurs when the TabBarPage is show or Hide or Add or Remove.")]
        public event TabBarVisibleChangedEventHandler TabBarVisibleChanged;

		/// <summary>
		/// Initializes a new <see cref="TabBarSplitterControl"/>.
		/// </summary>
		public TabBarSplitterControl()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(TabBarSplitterControl));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            CTRLSIZE = this.Size;

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
                    base.EnableTouchMode = value;
                    this.bar.EnableTouchMode = value;
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
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }

        #endregion

        /// <override/>
		protected override void InitLayout()
		{
			base.InitLayout();

			Form f = FindForm();
			if( f != null )
				f.Load += new EventHandler( ParentFormLoad );
			parentForm = f;
		}

		Form parentForm = null;

		private void ParentFormLoad( object sender, EventArgs e )
		{
			if( this.ActivePage == null && tabPages.Count > 0 )
				this.ActivePage = this.tabPages[0];
			if( parentForm != null )
			{
				parentForm.Load -= new EventHandler( ParentFormLoad );
				parentForm = null;
			}
		}

		/// <override/>
		protected override void OnControlRemoved( ControlEventArgs e )
		{
            if (ActivePage == e.Control)
            {
                ActivePage = null;

                if (this.tabPages != null && this.tabPages.Count > 0)
                    ActivePage = FindNextActivePage();
            }

			base.OnControlRemoved( e );
		}

        private TabBarPage FindNextActivePage()
        {
            for (int count = 0; count < tabPages.Count; count++)
            {
                if (this.tabPages[count].TabEnabled)
                    return this.tabPages[count];
            }

            return null;
        }


		/// <override/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				SuspendLayout();
                
                if( parentForm != null )
                    parentForm.Load -= new EventHandler(ParentFormLoad);


                if( tabPages != null )
                {
                    tabPages.Clear();
                    tabPages = null;
                }

                if (bar != null)
                {
                    bar.TabBarChild.Dispose();
                    bar.TabBarChild = null;
                    bar.SelectedIndexChanging -= new SelectedIndexEventHandler(BarSelectedIndexChanging);
                    bar.SelectedIndexChanged -= new SelectedIndexEventHandler(BarSelectedIndexChanged);
                    bar.TabBarChild.TabMoving -= new TabMovedEventHandler(TabsMoving);
                    bar.TabBarChild.TabMoved -= new TabMovedEventHandler(TabsMoved);
                    bar.ImageList.Dispose();
                    bar.ImageList = null;
                    bar.Dispose();
                    bar = null;
                }
				NativeMethods.DeleteObject(this.cachedRgn);
				this.cachedRgn = IntPtr.Zero;
                parentForm = null;
			}

			base.Dispose( disposing );
		}

		/// <override/>
		protected override Control CreateScrollBarContainer( ScrollBars sbType, int index )
		{
			if( sbType == ScrollBars.Horizontal && index == 0 )
			{
				return Bar;
			}
			return base.CreateScrollBarContainer( sbType, index );
		}

		/// <summary>
		/// Overridden method of refresh.
		/// </summary>
		public override void Refresh()
		{
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Created, RowCount, ColumnCount, ActivePage, Size );
#else
			;
#endif


			if( !Size.IsEmpty && Created )
			{
				RecreateIconList();

				try
				{
					if( ActivePage == null || !TabBarPages.Contains( ActivePage ) )
					{
						RefreshActivePage();
					}
					if( this.ActivePage == null && tabPages.Count > 0 )
					{
						this.ActivePage = this.tabPages[0];
						return;
					}
					if( ActivePage != null )
						ActivePage.PerformLayout();
				}
				finally
				{
					Bar.PerformLayout();
					base.Refresh();
				}
			}
		}

		void RefreshActivePage()
		{
			if( activePageIndex >= 0 && activePageIndex < TabBarPages.Count )
			{
				ActivePage = TabBarPages[activePageIndex];
				activePageIndex = -1;
			}
			else if( TabBarPages.Count == 0 )
			{
				Bar.SelectedIndex = -1;
				Bar.Tabs.Clear();
			}
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( RowCount, ColumnCount, ActivePage );
#else
			;
#endif

		}


		/// <summary>
		/// Returns the tab bar.
		/// </summary>
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public TabBar Bar
		{
			get
			{
				if( bar == null )
				{
					TraceUtil.TraceCurrentMethodInfoIf( Switches.Workbook.TraceVerbose );
					SuspendLayout();
					Control sb = CreateScrollBar( ScrollBars.Horizontal, 0 );
					bar = new TabBar( sb );
					bar.SuspendLayout();
					bar.BeginUpdate();
					bar.Resizable = sb != null;
					bar.DisabledColor = System.Drawing.SystemColors.GrayText;
					bar.EnableButtonFlags = ArrowType.First|ArrowType.Last|ArrowType.Next|ArrowType.Previous;
					bar.EnabledColor = System.Drawing.SystemColors.WindowText;
					bar.ButtonLook = this.ButtonLook;
					bar.DisplayArrowButtons = Syncfusion.Windows.Forms.DisplayArrowButtons.All;
					bar.ShowToolTips = this.ShowToolTips;
					bar.BackColor = System.Drawing.SystemColors.Window;
					bar.SelectedIndexChanging += new SelectedIndexEventHandler( BarSelectedIndexChanging );
					bar.SelectedIndexChanged += new SelectedIndexEventHandler( BarSelectedIndexChanged );
					bar.TabBarChild.TabMoving += new TabMovedEventHandler( TabsMoving );
					bar.TabBarChild.TabMoved += new TabMovedEventHandler( TabsMoved );
					bar.ImageList = new ImageList(/*container*/);
					bar.Dock = DockStyle.None;
					bar.Anchor = AnchorStyles.None;
                    bar.Height = 40;
                    if(this.hScrollBars == null)
                        this.hScrollBars = new Control[2];
					hScrollBars[0] = bar;
					bar.EndUpdate( false );
					bar.ResumeLayout( false );
					ResumeLayout( false );
				}
				return bar;
			}
		}

		/// <summary>
		/// Overridden method of Tostring()
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Concat( new String[] {
												  "TabBarSplitterControl: {",
												  this.Text,
												  ", TabCount: ",
												  Bar.TabCount.ToString(),
												  "}"
											  } );
		}

		/// <summary>
		/// Called when the user is dragging a tab.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">Event data.</param>
		protected virtual void TabsMoving( object sender, TabMovedEventArgs e )
		{
			if( !this.ActivePage.Validate() )
			{
				e.Cancel = true;
			}
		}

		/// <summary>
		/// Called when the user finishes dragging a tab.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">Event data.</param>
		protected virtual void TabsMoved( object sender, TabMovedEventArgs e )
		{
#if DEBUG
			if( Switches.Workbook.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( e.Tab, e.DestTab );
#else
			;
#endif

			RecreateIconList();
			SortControlCollection();
			Refresh();
		}

		/// <summary>
		/// Indicates whether Icons should be displayed in the tabs.
		/// </summary>
		[Description( "Indicates if Icons should be displayed in the tabs." )]
		[DefaultValue( true )]
		public bool ShowIcons
		{
			get
			{
				return showIcons;
			}
			set
			{
				if( showIcons != value )
				{
					showIcons = value;
					PerformLayout();
					Refresh();
				}
			}
		}

		/// <summary>
		/// Gets or sets the visual style of the tabBarSplitterControl.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Specifies the style with which tabBarSplitterControl will appear." )]
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
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnStyleChanged()
        {
            if (this.Style == TabBarSplitterStyle.Metro)
            {
                TabBarPageCollection t = this.tabPages;
                foreach (InternalTab page in this.Bar.TabBarChild.Tabs)
                {
                    page.m_renderer = new MetroTabsRenderer(page);
                }
                this.MetroScrollBar = true;
                this.TabFolderDelta = 11;
                this.BorderStyle = BorderStyle.None;
            }
			else if( this.Style == TabBarSplitterStyle.Office2007 )
            {
                TabBarPageCollection t = this.tabPages;
                foreach (InternalTab page in this.Bar.TabBarChild.Tabs)
                {
                    page.m_renderer = new Office2007TabsRenderer(page);
                }
                this.MetroScrollBar = false;
				this.TabFolderDelta = 11;
				this.BorderStyle = BorderStyle.FixedSingle;
				this.Office2007ScrollBars = true;
				int scheme = (int)this.Office2007ColorScheme;
				this.Office2007ScrollBarsColorScheme = (Office2007ColorScheme)scheme;
			}

            else
            {
                this.TabFolderDelta = 6;
                this.Office2007ScrollBars = false;
                this.MetroScrollBar = false;
            }

			m_office2007ColorTable = Office2007Colors.GetColorTable( m_colorScheme );
			if( this.Bar != null )
			{
				this.Bar.Style = this.Style;
				this.Bar.Office2007ColorScheme = this.Office2007ColorScheme;
			}

			if( this.tabPages != null )
			{
				foreach( TabBarPage page in this.tabPages )
				{
					page.Invalidate();
				}
			}
			this.InvalidateWindow();
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
					this.Invalidate();
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

		private BorderStyle VSBorderStyle
		{
			get
			{
				return ( this.Style == TabBarSplitterStyle.Office2007 && !this.ThemesEnabled ) ? BorderStyle.FixedSingle : this.BorderStyle;
			}
		}

		/// <summary>
		/// Gets or sets office 2007 style scroll bars. (overridden property)
		/// </summary>
		public override bool Office2007ScrollBars
		{
			get
			{
				return base.Office2007ScrollBars;
			}
			set
			{
				if( this.Style == TabBarSplitterStyle.Office2007 )
				{
					base.Office2007ScrollBars = true;
				}
				else
				{
					base.Office2007ScrollBars = value;
				}
			}
		}
        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string style;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
                if (this.Office2007ScrollBars)
                {
                    if (value == "Office2007Blue")
                        Office2007ScrollBarsColorScheme = Syncfusion.Windows.Forms.Office2007ColorScheme.Blue;
                    else if (value == "Office2007Silver")
                        Office2007ScrollBarsColorScheme = Syncfusion.Windows.Forms.Office2007ColorScheme.Silver;
                    else if (value == "Office2007Black")
                        Office2007ScrollBarsColorScheme = Syncfusion.Windows.Forms.Office2007ColorScheme.Black;
                    else if (value == "Managed")
                        Office2007ScrollBarsColorScheme = Syncfusion.Windows.Forms.Office2007ColorScheme.Managed;
                }
                else
                {
                    if (value == "Office2007Blue")
                        Office2010ScrollBarsColorScheme = Office2010ColorScheme.Blue;
                    else if (value == "Office2007Silver")
                        Office2010ScrollBarsColorScheme = Office2010ColorScheme.Silver;
                    else if (value == "Office2007Black")
                        Office2010ScrollBarsColorScheme = Office2010ColorScheme.Black;
                    else if (value == "Managed")
                        Office2010ScrollBarsColorScheme = Office2010ColorScheme.Managed;
              
                }
                if (value == "Office2007Blue")
                    Office2007ColorScheme = Office2007Theme.Blue;
                else if (value == "Office2007Silver")
                    Office2007ColorScheme = Office2007Theme.Silver;
                else if (value == "Office2007Black")
                    Office2007ColorScheme = Office2007Theme.Black;
                else if (value == "Managed")
                    Office2007ColorScheme = Office2007Theme.Managed;
                
            }
        }

		/// <summary>
		/// Gets or sets office 2007 scroll bars colorScheme. (overridden property)
		/// </summary>
		public override Office2007ColorScheme Office2007ScrollBarsColorScheme
		{
			get
			{
				return base.Office2007ScrollBarsColorScheme;
			}
			set
			{
				if( this.Style == TabBarSplitterStyle.Office2007 )
				{
					int scheme = (int)this.Office2007ColorScheme;
					base.Office2007ScrollBarsColorScheme = (Office2007ColorScheme)scheme;
				}
				else
				{
					base.Office2007ScrollBarsColorScheme = value;
				}
			}
		}

		/// <override/>
		protected override /*ContainerControl*/ CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle &= ~0x200 /*WS_EX_CLIENTEDGE*/;
				cp.Style &= ~0x800000 /*WS_BORDER*/;
				if( !( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled ) )
				{
					if( this.VSBorderStyle == BorderStyle.FixedSingle )
					{
						cp.Style |= 0x800000;
					}
					if( this.VSBorderStyle == BorderStyle.Fixed3D )
					{
						cp.ExStyle |= 0x200;
					}
				}
				cp.Style = cp.Style | (int)ControlStyles.AllPaintingInWmPaint | (int)ControlStyles.UserPaint | (int)WhidbeyCompatibleControlStyles.DoubleBuffer;
				return cp;
			}
		}

		private void InvalidateWindow()
		{
			int redrawFlags = NativeMethods.RDW_FRAME | NativeMethods.RDW_UPDATENOW | NativeMethods.RDW_INVALIDATE;
			NativeMethodsHelper.RedrawWindow( this.Handle, redrawFlags );
		}

		private IntPtr cachedRgn = IntPtr.Zero;
		IntPtr INonClientPaintingSupport.NonClientPaint( PaintEventArgs e, Rectangle displayRect, Rectangle windowRectInScreen )
		{
			Graphics g = e.Graphics;
			Rectangle bounds = displayRect;

			int w = 2;
			if( this.VSBorderStyle == BorderStyle.FixedSingle )
			{
				w = 1;
			}
			// The borders as 4 rectangles
			Rectangle[] clipRects = new Rectangle[]
						{
							new Rectangle( bounds.Location, new Size( w, bounds.Height ) ),
							new Rectangle( bounds.Location, new Size( bounds.Width, w ) ),
							new Rectangle( bounds.Width - w, bounds.Y, w, bounds.Height ),
							new Rectangle( bounds.X, bounds.Height - w, bounds.Width, w )
						};

			// Fill the border-rectangles with the bg brush, since some of the 
			// 3d border types are only 1 pixel wide.
			for( int i = 0; i < 4; i++ )
			{
                using (Brush brush = new SolidBrush(this.BackColor))
                    g.FillRectangle(brush, clipRects[i]);
			}

			Color borderColor = Color.Black;
			if( this.Style == TabBarSplitterStyle.Office2007 )
			{
				borderColor = this.Office2007ColorTable.TabBarSplitterBorderColor;
			}

			cd.DrawBorder( g, bounds, this.VSBorderStyle, Border3DStyle.Sunken, ButtonBorderStyle.Solid, borderColor );

			// return a region excluding where you just drew.
			return NativeMethods.CreateRectRgn( windowRectInScreen.Left + w, windowRectInScreen.Top + w, windowRectInScreen.Right - w, windowRectInScreen.Bottom - w );
		}

		/// </override>
		protected override void OnPaint( PaintEventArgs pe )
		{
			base.OnPaint( pe );

			if( this.Style == TabBarSplitterStyle.Office2007 && !splitterInfo.sizeGripBounds.IsEmpty )
			{
				pe.Graphics.ResetClip();

				this.PaintGripOffice2007( pe.Graphics, this.ReverseRectangleRTL( splitterInfo.sizeGripBounds ), this.Office2007ColorTable.TabBarSplitterSizeGripperLightColor,
					this.Office2007ColorTable.TabBarSplitterSizeGripperDarkColor, this.Office2007ColorTable.TabBarSplitterSizeGripperColor );
			}
		}

		private void PaintGripOffice2007( Graphics g, Rectangle rc, Color clGripLight, Color clGripDark, Color clGripBackground )
		{
			int gripWidth = 2;
			using( Brush br = new SolidBrush( clGripBackground ) )
			{
				g.FillRectangle( br, rc );
			}

			if( this.ShowSizeGrip )
			{
				using( Brush brushLight = new SolidBrush( clGripLight ) )
				{
					if( this.RightToLeft == RightToLeft.Yes )
					{
						Rectangle rcLight = new Rectangle( rc.Left + 2, rc.Bottom - 4, gripWidth, gripWidth );
						g.FillRectangle( brushLight, rcLight );
						rcLight.X += 4;
						g.FillRectangle( brushLight, rcLight );
						rcLight.X += 4;
						g.FillRectangle( brushLight, rcLight );
						rcLight.X -= 4;
						rcLight.Y -= 4;
						g.FillRectangle( brushLight, rcLight );
						rcLight.X -= 4;
						g.FillRectangle( brushLight, rcLight );
						rcLight.Y -= 4;
						g.FillRectangle( brushLight, rcLight );
					}
					else
					{
						Rectangle rcLight = new Rectangle( rc.Right - 4, rc.Bottom - 4, gripWidth, gripWidth );
						g.FillRectangle( brushLight, rcLight );
						rcLight.X -= 4;
						g.FillRectangle( brushLight, rcLight );
						rcLight.X -= 4;
						g.FillRectangle( brushLight, rcLight );
						rcLight.X += 4;
						rcLight.Y -= 4;
						g.FillRectangle( brushLight, rcLight );
						rcLight.X += 4;
						g.FillRectangle( brushLight, rcLight );
						rcLight.Y -= 4;
						g.FillRectangle( brushLight, rcLight );
					}
				}

				using( Brush brushDark = new SolidBrush( clGripDark ) )
				{
					if( this.RightToLeft == RightToLeft.Yes )
					{
						Rectangle rcDark = new Rectangle( rc.Left + 3, rc.Bottom - 5, gripWidth, gripWidth );
						g.FillRectangle( brushDark, rcDark );
						rcDark.X += 4;
						g.FillRectangle( brushDark, rcDark );
						rcDark.X += 4;
						g.FillRectangle( brushDark, rcDark );
						rcDark.X -= 4;
						rcDark.Y -= 4;
						g.FillRectangle( brushDark, rcDark );
						rcDark.X -= 4;
						g.FillRectangle( brushDark, rcDark );
						rcDark.Y -= 4;
						g.FillRectangle( brushDark, rcDark );
					}
					else
					{
						Rectangle rcDark = new Rectangle( rc.Right - 5, rc.Bottom - 5, gripWidth, gripWidth );
						g.FillRectangle( brushDark, rcDark );
						rcDark.X -= 4;
						g.FillRectangle( brushDark, rcDark );
						rcDark.X -= 4;
						g.FillRectangle( brushDark, rcDark );
						rcDark.X += 4;
						rcDark.Y -= 4;
						g.FillRectangle( brushDark, rcDark );
						rcDark.X += 4;
						g.FillRectangle( brushDark, rcDark );
						rcDark.Y -= 4;
						g.FillRectangle( brushDark, rcDark );
					}
				}
			}
		}

		/// </override>
		protected override void WndProc( ref Message m )
		{
			if( m.Msg == NativeMethods.WM_NCPAINT && !( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled ) )
			{
				this.cachedRgn = DrawingUtils.NCPaintHelper( this, this, ref m );
			}

			base.WndProc( ref m );
		}

		void SortControlCollection()
		{
			int index = 0;
			for( int n = 0; n < TabBarPages.Count; n++ )
			{
				TabBarPage page = TabBarPages[n];
				if( Controls.Contains( page ) )
				{
					page.TabIndex = index;
					TabBarPages[n].TabStop = true;
					Controls.SetChildIndex( page, index );
					index++;
				}
			}
		}


		void RecreateIconList()
		{
			Bar.ImageList.Images.Clear();
			for( int n = 0; n < Bar.TabBarChild.Tabs.Count; n++ )
			{
				InternalTab tab = Bar.TabBarChild.Tabs[n];
				TabBarPage page = tab.Cookie as TabBarPage;
				if( showIcons && page != null && page.Icon != null )
				{
					Image image = TabBarPages[n].Icon.ToBitmap();
					tab.ImageIndex = Bar.ImageList.Images.Count;
					Bar.ImageList.Images.Add( image );// System.Drawing.Color.Transparent);
				}
				else
					tab.ImageIndex = -1;
			}
		}

		/// <summary>
		/// Gets or sets the index of the active page.
		/// </summary>
		[DefaultValue( 0 ),
		Description( "Indicates the index of the active page." )]
		public int ActivePageIndex
		{
			get
			{
				if( ActivePage != null )
					return TabBarPages.IndexOf( activePage );
				return activePageIndex;
			}
			set
			{
				activePageIndex = value;
				RefreshActivePage();
			}
		}

		/// <summary>
		///     Gets or sets the currently visible TabBarPage.
		/// </summary>
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ), Browsable( false )]
		public TabBarPage ActivePage
		{
			get
			{
				return activePage;
			}
			set
			{
#if DEBUG
				if( Switches.Workbook.TraceVerbose )
					TraceUtil.TraceCurrentMethodInfo( ActivePage, value );
#else
				;
#endif

				if( value != null && activePage != value )
				{
                    TabBarPage activeTabBarPage = value as TabBarPage;
                    if (activeTabBarPage.TabEnabled || this.DesignMode)
                    {

					try
					{
						SuspendLayout();
						Bar.BeginUpdate();
						OnActivePageChanging( new ControlEventArgs( value ) );

						if( value != null )
						{
							ActiveControl = value;
							if( ActiveControl != value )
								return;
							ActiveControl = null;
						}

						InternalTabBar tabBar = Bar.TabBarChild;
						for( int i = 0; i < tabBar.Tabs.Count; i++ )
						{
							if( tabBar.Tabs[i] == null )
								continue;

							Control control = tabBar.Tabs[i].Cookie as Control;
							if( control != null )
							{
								if( control != value )
									(control as TabBarPage).Active = false;
							}
						}

						if( activePage != null && activePage != value )
						{
							activePage.Active = false;
							splitterInfo = activePage.splitterInfo;
							this.horizontalSplitterBar.UnwireTabPage( activePage );
							this.verticalSplitterBar.UnwireTabPage( activePage );
							activePage.horizontalSplitterBar = null;
							activePage.verticalSplitterBar = null;
						}
						activePage = value;
						splitterInfo = oSplitterInfo;
						if( activePage != null )
						{
							splitterInfo = activePage.splitterInfo;
							this.horizontalSplitterBar.WireTabPage( activePage );
							this.verticalSplitterBar.WireTabPage( activePage );
							Rectangle bounds = ReverseRectangleRTL( InnerBounds );
							bounds.X = 0;
							bounds.Width = ClientRectangle.Width;
							activePage.Bounds = bounds;
							activePage.Active = true;
							activePage.flatLook = ButtonLook == ButtonLook.Flat;

							for( int n = 0; n < this.vScrollBars.Length; n++ )
							{
								if( this.vScrollBars[n] != null )
								{
									if (this.vScrollBars[n].IsDisposed )
										vScrollBars[n] = CreateScrollBarContainer(ScrollBars.Vertical, n);
									activePage.Controls.Add( this.vScrollBars[n] );
									this.vScrollBars[n].BringToFront();
								}
							}
							Bar.SelectedIndex = TabBarPages.IndexOf( activePage );
						}
						else
						{
							for( int n = 0; n < this.vScrollBars.Length; n++ )
							{
								if( this.vScrollBars[n] != null )
								{
									this.Controls.Add( this.vScrollBars[n] );
									this.vScrollBars[n].BringToFront();
								}
							}
						}
					}
					catch( Exception ex )
					{
						TraceUtil.TraceExceptionCatched( ex );
						if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
							throw;
						if( activePage != null )
							ActiveControl = activePage;
					}
					finally
					{
						Bar.EndUpdate( false );
						ResumeLayout( false );
						Bar.TabBarChild.RefreshCurrentTab( true );
						OnActivePageChanged( new ControlEventArgs( activePage ) );
						if( value != null )
							Refresh();
					}
				}

                    else
                    {
                        if (activeTabBarPage != null)
                        {
                            activeTabBarPage.Bounds = ReverseRectangleRTL(InnerBounds);
                            for (int n = 0; n < this.vScrollBars.Length; n++)
                            {
                                if (this.vScrollBars[n] != null)
                                {
                                    this.vScrollBars[n].Enabled = false;
                                    activeTabBarPage.Controls.Add(this.vScrollBars[n]);
                                }
                            }
                            foreach (Control control in activeTabBarPage.Controls)
                            {
                                control.Enabled = false;
                            }

                        }

                    }
                }
            }
        }


		/// <summary>
		/// Raises the <see cref="ActivePageChanged"/> event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected virtual void OnActivePageChanged( ControlEventArgs e )
		{
#if DEBUG
			if( Switches.TabBarSplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name, e.Control );
#else
			;
#endif

			try
			{
				if( ActivePageChanged != null )
					ActivePageChanged( this, e );
			}
			catch( Exception ex )
			{
				TraceUtil.TraceExceptionCatched( ex );
				if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
					throw;
			}
		}

		/// <summary>
		/// Raises the <see cref="OnActivePageChanging"/> event.
		/// </summary>
		/// <param name="e">Event data.</param>
		protected virtual void OnActivePageChanging( ControlEventArgs e )
		{
#if DEBUG
			if( Switches.TabBarSplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name, e.Control );
#else
			;
#endif

			try
			{
				if( ActivePageChanging != null )
					ActivePageChanging( this, e );
			}
			catch( Exception ex )
			{
				TraceUtil.TraceExceptionCatched( ex );
				if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
					throw;
			}
		}

		void BarSelectedIndexChanged( object sender, SelectedIndexEventArgs e )
		{
		}

        void BarSelectedIndexChanging(object sender, SelectedIndexEventArgs e)
        {
            InternalTab tab = e.Tab;
            TabBarPage saved = ActivePage;
            if (IsHandleCreated)
            {
                if (tab != null && tab.Cookie is TabBarPage)
                {
                    TabBarPage selectedPage = tab.Cookie as TabBarPage;
                    if (selectedPage.TabEnabled || this.DesignMode)
                    {
                        ActivePage = (TabBarPage)tab.Cookie;
                        if (ActivePage != (TabBarPage)tab.Cookie)
                            e.Cancel = true;
                        else if (ActivePage.CanFocus)
                            ActivePage.Focus();
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

		/// <summary>
		/// Gets or sets the number of milliseconds to wait before repeatedly firing scroll event.
		/// </summary>
		[
		DefaultValue( 200 ),
		Description( "Milliseconds to wait before firing scroll event." )
		]
		public int RepeatClickDelay
		{
			get
			{
				return Bar.RepeatClickDelay;
			}
			set
			{
				ScrollBar scrollBar = Bar.scrollBar as ScrollBar;

				if( scrollBar != null )
				{
					int prevValue = Bar.RepeatClickDelay;
					double ratio = ( prevValue * 1.0 )/value;
					double addLargeValue = scrollBar.LargeChange * ratio;
					double addSmallValue = scrollBar.SmallChange * ratio;

					if( addLargeValue < 1 )
						addLargeValue = 1;
					else if( addLargeValue > scrollBar.Maximum )
						addLargeValue = scrollBar.Maximum/2.0;

					if( addSmallValue < 1 )
						addSmallValue = 1;
					else if( addSmallValue > scrollBar.Maximum )
						addSmallValue = scrollBar.Maximum/10.0;

					scrollBar.LargeChange = (int)addLargeValue;
					scrollBar.SmallChange = (int)addSmallValue;
				}

				Bar.RepeatClickDelay = value;
			}
		}

		/// <summary>
		/// Gets or sets the color of the arrows in the enabled buttons.
		/// </summary>
		[Description( "The color of arrows in enabled buttons." )]
		public Color EnabledColor
		{
			get
			{
				return Bar.EnabledColor;
			}
			set
			{
				Bar.EnabledColor = value;
			}
		}
		/// <summary>
		/// Resets <see cref="EnabledColor"/> to default.
		/// </summary>
		public void ResetEnabledColor()
		{
			Bar.ResetEnabledColor();
		}
		internal bool ShouldSerializeEnabledColor()
		{
			return EnabledColor != SystemColors.GrayText;
		}

		/// <summary>
		/// Gets or sets the color of the arrows in the disabled buttons.
		/// </summary>
		[Description( "The color of arrows in disabled buttons." )]
		public Color DisabledColor
		{
			get
			{
				return Bar.DisabledColor;
			}
			set
			{
				Bar.DisabledColor = value;
			}
		}
		/// <summary>
		/// Resets <see cref="DisabledColor"/> to default.
		/// </summary>
		public void ResetDisabledColor()
		{
			Bar.ResetDisabledColor();
		}
		internal bool ShouldSerializeDisabledColor()
		{
			return DisabledColor != SystemColors.GrayText;
		}

		/// <override/>
		protected override void OnButtonLookChanged( EventArgs e )
		{
			if( activePage != null )
				activePage.flatLook = ( ButtonLook == ButtonLook.Flat );
			Bar.ButtonLook = this.ButtonLook;
			Bar.Refresh();
#if DEBUG
			if( Switches.TabBarSplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name, this.ButtonLook );
#else
			;
#endif

			base.OnButtonLookChanged( e );
		}


		// Fields
		/// <override/>
		protected override Control.ControlCollection CreateControlsInstance()
		{
			return new ControlCollection( this );
		}

		internal new class ControlCollection: Control.ControlCollection
		{
			// Fields
			private TabBarSplitterControl owner;

			// Constructors
			public ControlCollection( TabBarSplitterControl owner )
				: base( owner )
			{
				this.owner = owner;
			}

			// Methods
			public override void Add( Control value )
			{
				base.Add( value );
				TabBarPage page = value as TabBarPage;
				if( page != null )
				{
					if( !this.owner.TabBarPages.Contains( page ) )
						this.owner.TabBarPages.Add( page );

					ISite iSite1 = this.owner.Site;
					if( iSite1 != null )
					{
						ISite iSite2 = page.Site;
						if( iSite2 == null )
						{
							IContainer iContainer = iSite1.Container;
							iContainer.Add( (IComponent)page );
						}
					}
				}
			}

			public override void Remove( Control value )
			{
				base.Remove( value );
				TabBarPage page = value as TabBarPage;
				if( page != null )
				{
					ISite iSite1 = this.owner.Site;
					if( iSite1 != null )
					{
						ISite iSite2 = page.Site;
						if( iSite2 == null )
						{
							IContainer iContainer = iSite1.Container;
							iContainer.Remove( (IComponent)page );
						}
					}
					this.owner.TabBarPages.Remove( page );
				}
			}
		}


		/// <summary>
		///     Returns the list of TabBarPages displayed in the TabBar. Each TabBarPage
		///     will have a tab associated with it.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( true ),
		Description( "Indicates the list of TabBarPages displayed in the TabBar." )
		]
		public TabBarPageCollection TabBarPages
		{
			get
			{
				if( this.tabPages == null )
				{
					this.tabPages = OnCreateTabBarPageCollection();
				}
				return this.tabPages;
			}
		}

		/// <summary>
		/// Creates the <see cref="TabBarPageCollection"/>.
		/// </summary>
		/// <returns>The new <see cref="TabBarPageCollection"/>.</returns>
		public virtual TabBarPageCollection OnCreateTabBarPageCollection()
		{
			return new TabBarPageCollection( this );
		}

		/// <override/>
		protected override void OnLayout( LayoutEventArgs levent )
		{
			if( Size.IsEmpty || Disposing || !Created )
				return;

			//Trace.WriteLine("TabBarSplitterControl.OnLayout");
			//RecreateIconList();
#if DEBUG
			if( Switches.TabBarSplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name, levent.AffectedProperty, levent.AffectedControl );
#else
			;
#endif

			base.OnLayout( levent );
			TabBarPage page = this.ActivePage;
			if( page != null )
			{
				page.horizontalSplitterBar = this.horizontalSplitterBar;
				page.verticalSplitterBar = this.verticalSplitterBar;
				Rectangle bounds = ReverseRectangleRTL( InnerBounds );
				bounds.X = 0;
				bounds.Width = ClientRectangle.Width;
				page.Bounds = bounds;
				//page.BringToFront();

				for( int n = 0; n < this.vScrollBars.Length; n++ )
				{
					if( this.vScrollBars[n] != null )
					{
						page.Controls.Add( this.vScrollBars[n] );
						this.vScrollBars[n].BringToFront();
					}
				}

			}
			SortControlCollection();
		}

		/// <override/>
		protected override bool ProcessDialogKey( Keys key )
		{
			if( IsDisposed )
				return true;
#if DEBUG

			if( Switches.SplitterControlEvents.TraceVerbose )

				TraceUtil.TraceCurrentMethodInfo( this, key );
#else

			;
#endif


			Keys keyCode = key & Keys.KeyCode;
			bool shiftKeyDown = ( Control.ModifierKeys & Keys.Shift ) != Keys.None;
			bool menuKeyDown = ( Control.ModifierKeys & Keys.Menu ) != Keys.None;
			bool controlKeyDown = ( Control.ModifierKeys & Keys.Control ) != Keys.None;

			switch( keyCode )
			{
				case Keys.PageDown:
				if( controlKeyDown )
					this.ActivateNextPage( false );
				return true;
				case Keys.PageUp:
				if( controlKeyDown )
					this.ActivateNextPage( true );
				return true;
			}
			return base.ProcessDialogKey( key );
		}

		/// <override/>
		[SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode=true )]
		protected override bool ProcessKeyEventArgs( ref Message m )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( m.ToString(), this );
#else
			;
#endif

			return base.ProcessKeyEventArgs( ref m );
		}

		/// <override/>
		protected override void OnKeyDown( KeyEventArgs e )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( e.KeyCode, this );
#else
			;
#endif

			base.OnKeyDown( e );
		}

		/// <override/>
		[SecurityPermission( SecurityAction.LinkDemand, UnmanagedCode=true )]
		protected override bool ProcessKeyPreview( ref Message m )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( m.ToString(), this );
#else
			;
#endif

			return base.ProcessKeyPreview( ref m );
		}

		/// <override/>
		protected override bool ProcessKeyMessage( ref Message m )
		{
#if DEBUG
			if( Switches.SplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( m.ToString(), this );
#else
			;
#endif

			return base.ProcessKeyMessage( ref m );
		}

		internal void ChildKeyDown( object sender, KeyEventArgs e )
		{
			if( !e.Handled )
			{
				Keys keyCode = e.KeyCode;
				bool controlKeyDown = ( e.Modifiers & Keys.Control ) != Keys.None;

				switch( keyCode )
				{
					case Keys.Next:
					if( controlKeyDown )
					{
						this.ActivateNextPage( false );
						e.Handled = true;
					}
					break;
					case Keys.Prior:
					if( controlKeyDown )
					{
						this.ActivateNextPage( true );
						e.Handled = true;
					}
					break;
				}
			}
		}

        /// <summary>
        /// Hides the <see cref="TabBarPage"/>.
        /// </summary>
        /// <param name="page"></param>
        public void HidePage(TabBarPage page)
        {
            string lastVisiblePage = "";
            InternalTabCollection tablist = this.Bar.TabBarChild.Tabs;
            for (int i = 0; i < tablist.Count; i++)
            {
                  if (tablist[i].Cookie == page)
                    {
                        tablist[i].Visible = false;
                        this.Refresh();
                        break;
                    }
				else if (tablist[i].Visible)
					lastVisiblePage = tablist[i].Label;
            }
            if (page.Active)
			{
				foreach (TabBarPage tabBarPage in this.tabPages)
				{
					if (tabBarPage.Text == lastVisiblePage)
						this.ActivePage = tabBarPage;
				}
			}
            OnTabBarVisibleChanged(new TabBarVisibleChangedEventArgs(page, false));
         }
        public void ShowPage(TabBarPage page)
        {
            InternalTabCollection tablist = this.Bar.TabBarChild.Tabs;
            for (int i = 0; i < tablist.Count; i++)
            {
                if (tablist[i].Cookie == page)
                {
                    tablist[i].Visible = true ;
                    this.Refresh();
                    break;
                }
            }
            OnTabBarVisibleChanged(new TabBarVisibleChangedEventArgs(page,true));
        }
        public  virtual void OnTabBarVisibleChanged(TabBarVisibleChangedEventArgs arg)
        {
#if DEBUG
            if (Switches.TabBarSplitterControlEvents.TraceVerbose)
                TraceUtil.TraceCurrentMethodInfo(Name, this.TabIndex);
#else
			;
#endif

            try
            {
                if (this.TabBarVisibleChanged != null)
                    this.TabBarVisibleChanged( this,arg);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                    throw;
            }
        }


              
		/// <summary>
		/// Activates the next or previous page.
		/// </summary>
		/// <param name="prev">True if previous pane should be activated; False if next pane should be activated.</param>
		public void ActivateNextPage( bool prev )
		{
			int index = ActivePageIndex;
			if( !prev )
			{
				if( index < this.TabBarPages.Count )
					index++;
				else
					index = 0;
			}
			else
			{
				if( index > 0 )
					index--;
				else
					index = this.TabBarPages.Count-1;
			}
			ActivePageIndex = index;
		}

		/// <override/>
		protected override void OnCancelMode( EventArgs e )
		{
#if DEBUG
			if( Switches.TabBarSplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name );
#else
			;
#endif

			base.OnCancelMode( e );
		}

		/// <override/>
		protected override void OnValidatingLostFocus()
		{
			if( bar != null )
				bar.NotifyCancelMode();
			base.OnValidatingLostFocus();
		}


		/// <override/>
		protected override void OnVisibleChanged( EventArgs e )
		{
			if( Visible && ActivePage == null )
			{
				SuspendLayout();
				RefreshActivePage();
				ResumeLayout( false );
			}
#if DEBUG
			if( Switches.TabBarSplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name, this.Visible );
#else
			;
#endif

			base.OnVisibleChanged( e );
		}


		/// <summary>
		/// Gets or sets the delta used for drawing the tabs.
		/// </summary>
		[
		Browsable( true ),
		Description( "Delta used for drawing the tabs." ),
		DefaultValue( 6 )
		]
		public int TabFolderDelta
		{
			get
			{
				return Bar.TabFolderDelta;
			}
			set
			{
				Bar.TabFolderDelta = value;
				Bar.TabBarChild.AdjustSize( true, false );
				Refresh();
			}
		}

		/// <summary>
		/// Gets or sets the scroll behavior of this tab bar: pixel or tabs.
		/// </summary>
		[
		Browsable( true ),
		Description( "Scrolling tabs by pixel or whole tabs." ),
		DefaultValue( InternalTabBarScrollBehavior.ScrollPixels )
		]
		public InternalTabBarScrollBehavior ScrollBehavior
		{
			get
			{
				return Bar.ScrollBehavior;
			}
			set
			{
				Bar.ScrollBehavior = value;
			}
		}

		/// <summary>
		/// Gets or sets the width of the tab bar relative to the width of the client bounds.
		/// </summary>
		[
		Browsable( true ),
		Description( "Specify the width of the tab bar relative to the width of client bounds." ),
		DefaultValue( 80 )
		]
		public int RelativeWidth
		{
			get
			{
				return Bar.RelativeWidth;
			}
			set
			{
				Bar.RelativeWidth = value;
			}
		}

		/// <summary>
		/// Enables or disables the resizing of tab bar.
		/// </summary>
		[
		Browsable( true ),
		Description( "Enable or Disable resizing the tab bar." ),
		DefaultValue( true )
		]
		public bool Resizable
		{
			get
			{
				return Bar.Resizable;
			}
			set
			{
				Bar.Resizable = value;
			}
		}

		/// <override/>
		protected override void OnFontChanged( EventArgs e )
		{
#if DEBUG
			if( Switches.TabBarSplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name, this.Font );
#else
			;
#endif

			base.OnFontChanged( e );
			Refresh();
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

		/// <override/>
		protected override void OnShowToolTipsChanged( EventArgs e )
		{
			if( bar != null )
				bar.ShowToolTips = ShowToolTips;
#if DEBUG
			if( Switches.TabBarSplitterControlEvents.TraceVerbose )
				TraceUtil.TraceCurrentMethodInfo( Name, this.ShowToolTips );
#else
			;
#endif

			base.OnShowToolTipsChanged( e );
		}


		/// <summary>
		/// Gets or sets the arrow buttons to be shown in an arrow bar.
		/// </summary>
		[
		DefaultValue( DisplayArrowButtons.All ),
		Description( "Specifies arrow buttons to be shown in an arrow bar." )
		]
		public DisplayArrowButtons ScrollButtons
		{
			get
			{
				return Bar.DisplayArrowButtons;
			}
			set
			{
				Bar.DisplayArrowButtons = value;
			}
		}

	}

	/// <summary>
	/// A collection of <see cref="TabBarPage"/> objects.
	/// </summary>
	/// <remarks>
	/// You can access this collection with the <see cref="TabBarSplitterControl.TabBarPages"/> of a <see cref="TabBarSplitterControl"/>.
	/// </remarks>
	public class TabBarPageCollection: IList
	{
		// Fields
		private TabBarSplitterControl owner;

		// Constructors
		/// <summary>
		/// Initializes a new <see cref="TabBarPageCollection"/> and
		/// associates it with a <see cref="TabBarSplitterControl"/>.
		/// </summary>
		/// <param name="owner">The <see cref="TabBarSplitterControl"/> that manages this collection.</param>
		public TabBarPageCollection( TabBarSplitterControl owner )
		{
			if( owner == null )
				throw new ArgumentNullException( @"owner" );

			this.owner = owner;
		}

		// Methods
		/// <summary>
		/// Returns an enumerator that lets you enumerate through the list of <see cref="TabBarPage"/> items.
		/// </summary>
		/// <returns>An enumerator.</returns>
		public virtual IEnumerator GetEnumerator()
		{
			TabBarPage[] tabPages = new TabBarPage[this.Count];
			this.CopyTo( tabPages, 0 );
			return tabPages.GetEnumerator();
		}

		/// <summary>
		/// Returns the number of pages.
		/// </summary>
		[
		EditorBrowsable( EditorBrowsableState.Advanced ),
		Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		]
		public virtual /*ICollection*/ int Count
		{
			get
			{
				InternalTabCollection tabList = this.owner.Bar.TabBarChild.Tabs;
				return tabList.Count;
			}
		}

		/// <summary>
		/// Removes an item at the specified index.
		/// </summary>
		/// <param name="index">The item index.</param>
		public virtual /*IList*/ void RemoveAt( int index )
		{
			if (index >= 0 && index < this.Count)
			{
				TabBarPage page = this[index];
				if (page != null)
				{
					this.Remove(page);
				}
			}
		} // end of method RemoveAt

		/// <summary>
		/// Returns False.
		/// </summary>
		public virtual /*IList*/ bool IsReadOnly
		{
			get
			{
				return false;
			} // end of method get_IsReadOnly
		}

		/// <summary>
		/// Clears all entries from the list.
		/// </summary>
		public virtual /*IList*/ void Clear()
		{
			InternalTabCollection tabs = this.owner.Bar.TabBarChild.Tabs;

			foreach( InternalTab tab in tabs )
			{
				tab.Dispose();
			}

			tabs.Clear();
		}

		void System.Collections.IList.Remove( object value )
		{
			if( value is TabBarPage )
				this.Remove( (TabBarPage)value );
			else
				throw new ArgumentException( @"value" );
		} // end of method System.Collections.IList.Remove

		void ICollection.CopyTo( Array dest, int index )
		{
			this.CopyTo( (TabBarPage[])dest, index );
		}

		void System.Collections.IList.Insert( int index, object value )
		{
			if( value is TabBarPage )
			{
				this.Add( (TabBarPage)value );
				this.IndexOf( (TabBarPage)value );
			}
			else
				throw new ArgumentException( @"value" );
		} // end of method System.Collections.IList.Insert

		/// <summary>
		/// Inserts a <see cref="TabBarPage"/> at the specified index.
		/// </summary>
		/// <param name="index">The index where the page should be inserted.</param>
		/// <param name="value">The page to be inserted.</param>
		public void Insert( int index, TabBarPage value )
		{
			InternalTabCollection tabs = this.owner.Bar.TabBarChild.Tabs;
			InternalTab tab = new InternalTab( owner, value );
			tab.ToolTip = value.ToolTipText;
			tab.Label = value.Text;
			tabs.Insert( index, tab );
			if( !owner.Controls.Contains( value ) )
				owner.Controls.Add( value );
			this.owner.Refresh();
		} // end of method System.Collections.IList.Insert
		int System.Collections.IList.IndexOf( object page )
		{
			if( page is TabBarPage )
				return this.IndexOf( (TabBarPage)page );
			return -1;
		} // end of method System.Collections.IList.IndexOf

		bool System.Collections.IList.Contains( object page )
		{
			if( page is TabBarPage )
				return this.Contains( (TabBarPage)page );
			return false;
		} // end of method System.Collections.IList.Contains

		int System.Collections.IList.Add( object value )
		{
			if( value is TabBarPage )
			{
				this.Add( (TabBarPage)value );
				return this.IndexOf( (TabBarPage)value );
			}
			else
				throw new ArgumentException( @"value" );
		} // end of method System.Collections.IList.Add

		bool System.Collections.IList.IsFixedSize
		{
			get
			{
				return false;
			} // end of method System.Collections.IList.get_IsFixedSize
		}

		bool System.Collections.ICollection.IsSynchronized
		{
			get
			{
				return false;
			} // end of method System.Collections.ICollection.get_IsSynchronized
		}

		object System.Collections.ICollection.SyncRoot
		{
			get
			{
				return (object)this;
			} // end of method System.Collections.ICollection.get_SyncRoot
		}

		object System.Collections.IList.this[int index]
		{
			set
			{
				if( value is TabBarPage )
				{
					this[index] = (TabBarPage)value;
					return;
				}
				else
					throw new ArgumentException( @"value" );
			} // end of method System.Collections.IList.set_Item
			get
			{
				return (object)this[index];
			} // end of method System.Collections.IList.get_Item
		}

		/// <summary>
		/// Gets / sets the <see cref="TabBarPage"/> at the specified index.
		/// </summary>
		public virtual TabBarPage this[int index]
		{
			get
			{
				//return this.owner.Controls[index] as TabBarPage;
				return this.owner.Bar.TabBarChild.Tabs[index].Cookie as TabBarPage;
			}
			set
			{
				InternalTab tab = this.owner.Bar.TabBarChild.Tabs[index];
				if( tab != null )
					tab.Cookie = value;
				else
				{
					tab = new InternalTab( this.owner, value );
					this.owner.Bar.TabBarChild.Tabs[index] = tab;
				}
				tab.ToolTip = value.ToolTipText;
				tab.Label = value.Text;
				this.owner.Refresh();
			}
		}

		/// <summary>
		/// Adds a <see cref="TabBarPage"/> to the collection.
		/// </summary>
		/// <param name="value">The page to be added.</param>
		public virtual void Add( TabBarPage value )
		{
			owner.SuspendLayout();
			InternalTabCollection tabs = this.owner.Bar.TabBarChild.Tabs;
			InternalTab tab = new InternalTab( owner, value );
			tab.ToolTip = value.ToolTipText;
			tab.Label = value.Text;
			tab.Style = this.owner.Style;
			tab.Office2007ColorScheme = this.owner.Office2007ColorScheme;
			tabs.Add( tab );
			if( !owner.Controls.Contains( value ) )
				owner.Controls.Add( value );
			owner.ResumeLayout( false );
			owner.Refresh();
            owner.OnTabBarVisibleChanged(new TabBarVisibleChangedEventArgs(value,true));
		}

		/// <summary>
		/// Adds an array of <see cref="TabBarPage"/> to this collection.
		/// </summary>
		/// <param name="value">The pages to be added.</param>
		public void AddRange( TabBarPage[] value )
		{
			this.owner.SuspendLayout();
			foreach( TabBarPage page in value )
			{
				if( page != null )
					this.Add( page );
			}
			owner.ResumeLayout( false );
			owner.Refresh();
		}

		/// <summary>
		/// Indicates whether the specified <see cref="TabBarPage"/> belongs to this collection.
		/// </summary>
		/// <param name="page">The page to be tested.</param>
		/// <returns>True if page belongs to collection; False otherwise.</returns>
		public bool Contains( TabBarPage page )
		{
			if( page == null )
				throw new ArgumentNullException( @"value" );
			return ( this.IndexOf( page ) != -1 );
		} // end of method Contains


		/// <summary>
		/// Returns the index of the specified page to be queried.
		/// </summary>
		/// <param name="page">The page to look up.</param>
		/// <returns>The zero-based index of the page; -1 if not found.</returns>
		public int IndexOf( TabBarPage page )
		{
			if( page == null )
				throw new ArgumentNullException( @"value" );
			for( int n0 = 0; n0 < this.Count; n0++ )
			{
				if( this[n0] == page )
					return n0;
			}
			return -1;
		} // end of method IndexOf


		/// <summary>
		/// Removes a page from the collection.
		/// </summary>
		/// <param name="value">The page to remove.</param>
		public virtual void Remove( TabBarPage value )
		{
			owner.SuspendLayout();
			try
			{
				InternalTabCollection tabs = this.owner.Bar.TabBarChild.Tabs;
				int index = this.owner.Bar.TabBarChild.FindTab( value );
				if( index != -1 )
					this.owner.Bar.TabBarChild.Tabs.Remove( tabs[index] );
				if( owner.ActiveControl == value )
					owner.ActiveControl = null;
				if( owner.Controls.Contains( value ) )
					owner.Controls.Remove( value );
				owner.ActivePageIndex = Math.Max( 0, index-1 );
			}
			catch( Exception ex )
			{
				TraceUtil.TraceExceptionCatched( ex );
				if( !ExceptionManager.RaiseExceptionCatched( this, ex ) )
					throw;
			}
            owner.OnTabBarVisibleChanged(new TabBarVisibleChangedEventArgs(value,false));
			owner.ResumeLayout( false );
			owner.Refresh();
		}

		/// <overload>
		///   <para>Copies the collection objects to a one-dimensional <see cref="System.Array" /> instance beginning at the
		/// specified index.</para>
		/// </overload>
		/// <summary>
		///   <para>Copies the collection objects to a one-dimensional <see cref="System.Array" /> instance beginning at the
		/// specified index.</para>
		/// </summary>
		/// <param name="pages">
		///   <para>The one-dimensional <see cref="System.Array" /> that is the destination of the values copied from the collection.</para>
		/// </param>
		/// <param name="index">The index of the array at which to begin inserting.</param>
		public void CopyTo( TabBarPage[] pages, int index )
		{
			InternalTabCollection tabList = this.owner.Bar.TabBarChild.Tabs;
			if( tabList != null )
			{
				for( int i = 0; i < tabList.Count; i++ )
					pages[index++] = tabList[i].Cookie as TabBarPage;
			}
		}
	}

	/// <summary>
	/// Specifies the style with which tabBarSplitterControl will appear.
	/// </summary>
	public enum TabBarSplitterStyle
	{
		/// <summary>
		/// Default appearance.
		/// </summary>
		Default,
		/// <summary>
		/// Office 2007-like appearance.
		/// </summary>
		Office2007,
		/// <summary>
		/// Metro like appearance.
		/// </summary>
		Metro
	}
}