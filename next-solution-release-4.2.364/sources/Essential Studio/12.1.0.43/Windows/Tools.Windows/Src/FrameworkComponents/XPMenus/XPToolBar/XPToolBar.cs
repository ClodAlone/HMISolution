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
using System.Reflection;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Design;
using Syncfusion.Collections;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.ComponentModel;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Drawing;
#endregion

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	/// <summary>
	/// The XPToolBar class provides you a tool bar like look-and-feel that you can use
	/// outside the XPMenus framework to display BarItems.
	/// </summary>
	/// <remarks>
	/// The XP tool bar look-and-feel is becoming so popular that they are beginning to get used
	/// outside of the form's menu structure, inside the forms. A good example is the PropertyGrid
	/// control that uses this look and feel to provide users some command buttons at the top
	/// (the Categorized/Alphabetic button, etc.).
	/// <para>This XPToolBar makes it easy for you to get this tool bar look-and-feel outside the
	/// BarManager framework. Drop this control anywhere in your form and fill the items list
	/// with BarItems. You can also insert separators using the BeginGroupAt method.</para>
	/// </remarks>
	/// <example>
	/// <code lang="C#">
	/// In code, you can initialize an XPToolBar as follows:
	/// XPToolBar xpToolBar1;
	/// // Create a new tool bar control.
	/// 
	/// XPToolBar xptoolbar2 = new XPToolBar();
	/// // Add some one or more instances of BarItem to it.
	/// xptoolbar2.Items.AddRange(new BarItem[]{this.barItem1, this.barItem2, this.barItem3});
	/// // Setup a separator.
	/// xptoolbar2.BeginGroupAt(this.barItem2);
	/// 
	/// // Set its position and add it to the Form.
	/// xptoolbar2.Dock = DockStyle.Top;
	/// xptoolbar2.Size = new Size(200, 30);
	/// this.Controls.Add(xptoolbar2);
	/// </code>
	/// <code lang="VB">
	/// Dim xpToolBar1 As XPToolBar
	/// ' Create a new tool bar control.
	/// Dim xptoolbar2 As New XPToolBar()
	/// 
	/// ' Add some one or more instances of BarItem to it.
	/// xptoolbar2.Items.AddRange(New BarItem() {Me.barItem1, Me.barItem2, Me.barItem3})
	/// ' Setup a separator.
	/// xptoolbar2.BeginGroupAt(Me.barItem2)
	/// 
	/// ' Set its position and add it to the Form.
	/// xptoolbar2.Dock = DockStyle.Top
	/// xptoolbar2.Size = New Size(200, 30)
	/// Me.Controls.Add(xptoolbar2)
	/// </code>
	/// </example>
	[ToolboxItem(true),
	Designer(
		typeof(Syncfusion.Windows.Forms.Tools.Design.XPToolBarDesigner),
		typeof(System.ComponentModel.Design.IDesigner)),
	ToolboxBitmap(typeof(CommandBar), "ToolboxIcons.XPToolBar.bmp"),
	Description("Provides tool bar like look-and-feel to use outside XPMenus framework.")
	]
	public class XPToolBar : BarControlInternal, IBarHost, IDesignable, IIgnoreWorkingArea,IVisualStyle 
	{
		/// <summary>
		/// Indicates whether to use show chevron if not all BarItems are visible.
		/// </summary>
		private bool m_bShowChevron = false;
		private bool needLayout = false;
		private BarManager designTimeBarManager = null;
		private VisuallyInheritableIntList separatorList;
		private bool largeIcons = false;
		private Bar internalBar = null;
		private BrushInfo bgBrush = null;
		private bool _uiUpdateMFCStyle = false;
		private bool bBarItemActiveFormCheckOverride = false;

		/// <summary>
		/// Indicates whether to show BarItem highlighted when mouse is moves over it.
		/// </summary>
		private bool m_bShowHighlightRectangle = true;

		/// <summary>
		/// Indicates whether items of the XPToolBar will be draws Horizontal 
		/// when XPToolBar is vertical docked.
		/// </summary>
		private bool m_bRotateWhenVertical = false;

		/// <summary>
		/// Colorschemes for Office2007 visual style.
		/// </summary>
		private Office2007Theme m_office2007Theme = Office2007Theme.Blue;
        /// <summary>
        /// Colorschemes for Office2007 visual style.
        /// </summary>
        private Office2010Theme m_office2010Theme = Office2010Theme.Blue;
        /// <summary>
        /// Color table for Office2010 visual style.
        /// </summary>
        private Office2010Colors m_office2010ColorTable = null;
		/// <summary>
		/// Color table for Office2007 visual style.
		/// </summary>
		private Office2007Colors m_office2007ColorTable = null;
        static bool bDevEnv = (Application.ExecutablePath.ToLower().IndexOf("devenv.exe") >= 0);
        static bool s_isDevEnv = SystemInformationExt.IsDotNetApp && (Application.ExecutablePath.ToLower().IndexOf("devenv.exe") < 0);
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        /// <summary>
        /// Default font style of the control
        /// </summary>
        private static Font FONTSTYLE = default(Font);

        /// <summary>
        /// Font which stored after changed in design
        /// </summary>

        private static Font USERFONTSTYLE = default(Font);
		static XPToolBar()
		{

		}
		/// <summary>
		/// Initializes a new instance of the XPToolBar class.
		/// </summary>
		public XPToolBar()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			   new Syncfusion.Core.Licensing.LicensedComponent(typeof(XPToolBar));
			}
			finally
			{
			AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
			}
			this.TabStop = false;
			this.bgBrush = BrushInfo.Empty;

			this.Bar = new Bar(null);
			this.Bar.Items.Parent = this;			

			this.internalBar = this.Bar;

			this.BarHost = this;
			if(!this.DesignMode)
				this.AllowDrop = false;
            CTRLSIZE = this.Size;
            FONTSTYLE = this.Font;
            USERFONTSTYLE = FONTSTYLE;
			this.separatorList = new VisuallyInheritableIntList(this);
			this.separatorList.CollectionChanged += new CollectionChangeEventHandler(this.SeparatorList_Changed);
		}


		/// <summary>
		/// Indicates whether to show chevron.
		/// </summary>
		[ DefaultValue( false ), Description("Indicates whether to show chevron.")]
		public bool ShowChevron
		{
			get
			{
				return m_bShowChevron;
			}
			set
			{
				if( value != m_bShowChevron )
				{
					m_bShowChevron = value;

					this.RendererChanged( null );
				}
			}
		}
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

        /// <summary>
        /// Scale the control based on the scale factor passed in the argument.
        /// </summary>
        /// <param name="scaleFactor">value to scale the factor based upon.</param>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            if (FONTSTYLE == USERFONTSTYLE)
                this.Font = new Font(FONTSTYLE.FontFamily, FONTSTYLE.Size * scaleFactor, this.Font.Style, this.Font.Unit, this.Font.GdiCharSet, this.Font.GdiVerticalFont);
            else
                this.Font = new Font(USERFONTSTYLE.FontFamily, FONTSTYLE.Size * scaleFactor, this.Font.Style, this.Font.Unit, this.Font.GdiCharSet, this.Font.GdiVerticalFont);
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            foreach (object item in this.Bar.Items)
            {
                if (item is ParentBarItem)
                {
                    TouchSupport(item as ParentBarItem,scaleFactor);
                }
                else
                    (item as BarItem).CustomTextFont = new Font(USERFONTSTYLE.FontFamily, FONTSTYLE.Size * scaleFactor, (item as BarItem).CustomTextFont.Style, (item as BarItem).CustomTextFont.Unit, (item as BarItem).CustomTextFont.GdiCharSet, (item as BarItem).CustomTextFont.GdiVerticalFont);

            }
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }
        /// <summary>
        /// Scale the control based on the scale factor passed in the argument.
        /// </summary>
        private void TouchSupport(ParentBarItem pItem,float sf)
        {
            foreach (BarItem bitem in pItem.Items)
            {
                //bitem.CustomTextFont = new Font(USERFONTSTYLE.FontFamily, FONTSTYLE.Size * sf, bitem.CustomTextFont.Style, bitem.CustomTextFont.Unit, bitem.CustomTextFont.GdiCharSet, bitem.CustomTextFont.GdiVerticalFont);
                if (bitem is ParentBarItem)
                {
                    TouchSupport(bitem as ParentBarItem, sf);
                }
            }
        }
		/// <summary>
		/// Indicates whether to highlight BarItem when mouse moves over it.
		/// </summary>
		[
		DefaultValue( true ),
		Description("Indicates whether to highlight BarItem when mouse moves over it.")
		]
		public bool ShowHighlightRectangle
		{
			get
			{
				return m_bShowHighlightRectangle;
			}
			set
			{
				if( value != m_bShowHighlightRectangle )
				{
					m_bShowHighlightRectangle = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether items of the XPToolBar will be draws Horizontal 
		/// when XPToolBar is vertical docked.
		/// </summary>
		[
		Category( "Behavior" ),
		DefaultValue( false ),
		Description( "Indicates whether items of the XPToolBar will be draws Horizontal when XPToolBar is vertical docked." )
		]
		public bool RotateWhenVertical
		{
			get
			{
				return m_bRotateWhenVertical;
			}
			set
			{
				if( value != m_bRotateWhenVertical )
				{
					m_bRotateWhenVertical = value;
					OnRotateWhenVerticalChanged();				
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

                if (value == "Office2007Blue")
                        Office2007Theme = Office2007Theme.Blue;
                else if (value == "Office2007Silver")
                        Office2007Theme = Office2007Theme.Silver;
                else if (value == "Office2007Black")
                        Office2007Theme = Office2007Theme.Black;
                else if (value == "Office2010Blue")
                    Office2010Theme = Office2010Theme.Blue;
                else if (value == "Office2010Silver")
                    Office2010Theme = Office2010Theme.Silver;
                else if (value == "Office2010Black")
                    Office2010Theme = Office2010Theme.Black;
                else if (value == "Managed")
                {
                    if (this.Style == VisualStyle.Office2010)
                        Office2010Theme = Office2010Theme.Managed;
                    else
                        Office2007Theme = Office2007Theme.Managed;
                }
            }
        }
		/// <summary>
		/// Gets or sets colorschemes for Office2007 visual style.
		/// </summary>
		[
		Description( "Gets or sets colorschemes for Office2007 visual style." ),
		Category( "Appearance" ),
		DefaultValue( Office2007Theme.Blue )
		]
		public Office2007Theme Office2007Theme
		{
			get { return m_office2007Theme; }
			set
			{
				if( m_office2007Theme != value )
				{
					m_office2007Theme = value;
					UpdateColorScheme();
					Invalidate();
				}
			}
		}
        /// <summary>
        /// Gets or sets colorschemes for Office2010 visual style.
        /// </summary>
        [
        Description("Gets or sets colorschemes for Office2010 visual style."),
        Category("Appearance"),
        DefaultValue(Office2010Theme.Blue)
        ]
        public Office2010Theme Office2010Theme
        {
            get { return m_office2010Theme; }
            set
            {
                if (m_office2010Theme != value)
                {
                    m_office2010Theme = value;
                    UpdateColorScheme();
                    Invalidate();
                }
            }
        }
		/// <summary>
		/// Gets color table for Office2007 visual style.
		/// </summary>
		private Office2007Colors Office2007ColorTable
		{
			get
			{
				Office2007Colors colorTable = ( m_office2007ColorTable == null ) ? 
					Office2007Colors.Default :	m_office2007ColorTable;

				return colorTable;
			}
		}
        /// <summary>
        /// Gets color table for Office2007 visual style.
        /// </summary>
        private Office2010Colors Office2010ColorTable
        {
            get
            {
                Office2010Colors colorTable = (m_office2010ColorTable == null) ?
                    Office2010Colors.Default : m_office2010ColorTable;

                return colorTable;
            }
        }
		protected override void UpdateColorScheme()
		{
            
			base.UpdateColorScheme();
            if (this.Style == VisualStyle.Office2007)
            {
                Office2007OutlookColors.UpdateMenuColors(Office2007Theme);
                m_office2007ColorTable = Office2007Colors.GetColorTable(Office2007Theme);
            }
            else
            {
                Office2010OutlookColors.UpdateMenuColors(Office2010Theme);
                m_office2010ColorTable = Office2010Colors.GetColorTable(Office2010Theme);
            }
		}

        
		protected virtual void OnRotateWhenVerticalChanged()
		{
			this.Bar.BarStyle = ( m_bRotateWhenVertical ) ?
				this.Bar.BarStyle | BarStyle.RotateWhenVertical :
				this.Bar.BarStyle & ~BarStyle.RotateWhenVertical;
		}

		/// <summary>
		/// Sets dock style for renderer.
		/// </summary>
		private void SetRendererDock( BarRenderer renderer, DockStyle dock )
		{
			if( renderer != null )
			{
				switch( dock )
				{
					case DockStyle.Left :
					{
						renderer.Alignment = CommandBarDockState.Left;
						break;
					}
					case DockStyle.Right :
					{
						renderer.Alignment = CommandBarDockState.Right;
						break;
					}
					case DockStyle.Top :
					{
						renderer.Alignment = CommandBarDockState.Top;
						break;
					}
					case DockStyle.Bottom :
					{
						renderer.Alignment = CommandBarDockState.Bottom;
						break;
					}
					default : 
					{
						renderer.Alignment = CommandBarDockState.Top;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Gets correctly size for XPToolBar
		/// </summary>
		private Size GetCorrectlySize( int preferredWidth, int preferredHeight )
		{
			int correctWidth = preferredWidth;
			int correctHeight = preferredHeight;
			Size preferredSize = GetPreferredSizeInternal( preferredWidth, preferredHeight );

			if( this.IsVerticallyAligned )
			{
				correctWidth = preferredSize.Width;

				if( correctHeight < preferredSize.Height && !this.ShowChevron )
					correctHeight = preferredSize.Height;
			}
			else
			{
				correctHeight = preferredSize.Height;

				if( correctWidth < preferredSize.Width && !this.ShowChevron )
					correctWidth = preferredSize.Width;
			}

			return new Size( correctWidth, correctHeight );
		}

        /// <summary>
        /// Gets correctly location for XPToolBar
        /// </summary>
        private Point GetCorrectlyLocation( int x, int y, int width, int height )
        {
            Point correctPoint = new Point( x, y );
			
            if( ( this.Dock == DockStyle.Bottom || this.Dock == DockStyle.Right )
                && this.Parent != null && this.Parent.ClientSize != Size.Empty )
            {
                if( this.IsVerticallyAligned )
                {
					correctPoint.X = this.Parent.ClientSize.Width - width;
                }
                else
                {
                   correctPoint.Y = this.Parent.ClientSize.Height - height;
                }
            }

            return correctPoint;
        }


        private bool m_bIsDisposing = false;

		/// <override/>
		protected override void Dispose(bool disposing)
		{
            m_bIsDisposing = true;

            base.Dispose( disposing );

            this.DisposeInternalBar();

			if( disposing )
			{
                if( Bar != null )
                {
                    if( Items != null )
                    {
                        foreach( BarItem item in Items )
                        {
                            item.Dispose();
                        }
                    }
                    Bar = null;                                        
                }
                
				if(this.separatorList != null)
				{
					this.separatorList.CollectionChanged -= new CollectionChangeEventHandler(this.SeparatorList_Changed);
					this.separatorList = null;
				}

				Application.Idle -= new EventHandler(this.OnIdle);
			}            

            m_bIsDisposing = false;
		}

		private void DisposeInternalBar()
		{
			if(this.internalBar != null)
			{
				this.internalBar.Dispose();
				this.internalBar = null;
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="BarItem.UpdateUI"/> events for the BarItems
		/// should be fired MFC style on Application.Idle.
		/// </summary>
		/// <value>True to fire the UpdateUI event; false otherwise. Default is false.</value>
		/// <remarks>Take a look at <see cref="BarItem.UpdateUI"/> event description for more 
		/// information on when and how this pattern should be used.</remarks>
		[Description("Specifies whether the UpdateUI event for the BarItems should be fired in a MFC style fashion."),
		DefaultValue(false),
		Category("Behavior")
		]
		public bool UpdateUIMFCStyle
		{
			get
			{
				return this._uiUpdateMFCStyle;
			}
			set
			{
				if(this._uiUpdateMFCStyle != value)
				{
					this._uiUpdateMFCStyle = value;
					if(this._uiUpdateMFCStyle == false)
						Application.Idle -= new EventHandler(this.OnIdle);
					else
						Application.Idle += new EventHandler(this.OnIdle);
				}
			}
		}

		/// <summary>
		/// Indicates whether the BarItems should check for ActiveForm before displaying tooltip. Workaround for using from MFC applications.
		/// </summary>
		[Description("Specifies whether the BarItems should check for ActiveForm before displaying tooltip."),
		DefaultValue(false),
		Category("Behavior")
		]
		public bool BarItemActiveFormCheckOverride
		{
			get
			{
				return this.bBarItemActiveFormCheckOverride;
			}
			set
			{
				if(this.bBarItemActiveFormCheckOverride != value)
				{
					this.bBarItemActiveFormCheckOverride = value;
				}
			}
		}

		/// <summary>
        /// Gets or sets a value indicating whether the user can give the focus to this
        ///  control using the TAB key.
		/// </summary>
		[DefaultValue(false)]
		public new bool TabStop
		{
			get{return base.TabStop;}
			set{base.TabStop = value;}
		}

		private void OnIdle(object sender, EventArgs e)
		{
			if(this.UpdateUIMFCStyle)
			{
				foreach(BarItem item in this.Items)
				{
					if(item.Visible)
						item.PerformUpdateUI();
				}
			}
		}

		protected override void OnHandleCreated( EventArgs e )
		{
			this.HostedForm = this.FindForm();
			base.OnHandleCreated( e );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.WndProc"/>.
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			// To avoid taking focus.
			// OnMouseDown will ensure that the parent has focus.
			if (m.Msg == 0x21/*WM_MOUSEACTIVATE*/) 
			{
				// FindForm will be null when the control is hosted in a native Form.
				if(this.FindForm() != null)
				{
					m.Result = (IntPtr)3;
					return;
				}
			}

			base.WndProc(ref m);
		}
		
		bool IDesignable.DesignMode
		{
			get{return base.DesignMode;}
		}

		private void EnsureParentHasFocus()
		{
			bool forceFocus = true;
			IntPtr wndFocus = NativeMethods.GetFocus();
			if(wndFocus != IntPtr.Zero)
			{
				Control controlFocus = Control.FromHandle(wndFocus);
				if(controlFocus != null && controlFocus.FindForm() == this.FindForm())
					forceFocus = false;
			}
			if(forceFocus)
			{
				Form immediateFormParent = this.FindForm();
				if(immediateFormParent != null)
				{
					MdiSysMenuProvider sysmenuprovider = null;
					if(immediateFormParent.IsMdiChild == true)
					{
						sysmenuprovider = MdiSysMenuManager.GetProviderForForm(immediateFormParent.MdiParent);
					}

					if(immediateFormParent.ParentForm != null
						&& !immediateFormParent.ParentForm.ContainsFocus)
						immediateFormParent.ParentForm.Focus();

					if(immediateFormParent.ContainsFocus == false)
						immediateFormParent.Focus();

				}
			}
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseDown"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.EnsureParentHasFocus();

			// If the app is a native windows app, then make this selectable, beacuse the PopupManager 
			// cannot be relied upon to deactivate (call HidePopup on the renderer). We will instead deactivate when the Control loses focus.
			if(!SystemInformationExt.IsDotNetApp)
				this.Focus();

			base.OnMouseDown(e);
		}

        /// <summary>
        /// Overrides the ShouldPreProcessTab method.
        /// </summary>
        /// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public override bool ShouldPreProcessTab()
		{
			return false;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public override bool NeedKey(Keys key)
		{
//			this snipped of code is commented because of isue 289
//			if((key == Keys.Tab) || (key == (Keys.Tab | Keys.Shift)))
//					return true;
//			else
				return base.NeedKey(key);
		}

        /// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.ProcessCmdKey"/>.
		/// </summary>
		/// <param name="keyData"></param>
		protected override bool ProcessDialogKey(Keys keyData)
		{
			if(keyData == Keys.Down || keyData == Keys.Up
				|| keyData == Keys.Right || keyData == Keys.Left)
				// Don't want base class to move focus.
				return false;

			return base.ProcessDialogKey(keyData);
		}

        /// <summary>
        /// Forces BarItem to fire The ItemClick event, when specified shortcut for this BarItem entered by the user.
        /// </summary>
        /// <param name="keyData">Shortcut to process.</param>
		public void ProcessShortcut( Keys keyData )
		{
            BarItem item = GetItemByShortcut( (int)keyData );
			if( item == null ) return;
			
			if(item.Enabled && item.Visible)
				item.PerformClick();
		}
		protected override bool ProcessMnemonic(char charCode)
		{
			if(this.barRenderer.ProcessShortcut(charCode))
				return true;
			else
				return base.ProcessMnemonic(charCode);
		}
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnGotFocus"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			if(!this.barRenderer.IsKeyboardNavigationOn()
				&& !this.barRenderer.IsShowingDropdown())
				this.barRenderer.StartKeyboardNavigation();
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnLostFocus"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			if(!this.ContainsFocus)
				this.barRenderer.OnFormDeactivated();
		}

		private BarManager tempBarManager;
        private ArrayList tempItems;
		[Browsable(false),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public void BeforeCodeDomSerialize()
		{
			this.tempBarManager = this.Bar.Manager;
			this.Bar.Manager = null;
            if (BarManager.DesignerBarClone)
            {
                this.tempItems = (ArrayList)this.Items.Clone();
                this.Items.Clear();
                this.Bar.UpdatedBarItemPositions = null;
                this.UpdatedBarItemPositions = null;
            }
		}

		[Browsable(false),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public void AfterCodeDomSerialize()
		{
			this.Bar.Manager = tempBarManager;
            if (BarManager.DesignerBarClone)
            {
                foreach (object obj in this.tempItems)
                {
                    this.Items.Add(obj);
                }
            }
		}

		/// <override/>
		protected override Size DefaultSize 
		{ 
			get
			{
				return ((Size)(new Size(100, 28)));
			}
		}
		

		protected override Form HostedForm
		{
			get
			{
				return base.HostedForm;
			}
			set
			{
				base.HostedForm = value;

				// Make sure to do this only in runtime. Otherwise VS.Net chokes when using Enterprise Template Projects.
				if( null != this.HostedForm &&
					!(this.DesignMode || bDevEnv) )
				{
					// Init ContextMenuPlaceHolder to listen for shortcut command keys.
					ContextMenuPlaceHolder cm = this.HostedForm.ContextMenu as ContextMenuPlaceHolder;
					if(cm == null)
						cm = new ContextMenuPlaceHolder();

					cm.MainMenuForm = this.HostedForm;

					cm.AddToolBar( this );
				}
			}
		}
        [Description("Gets or sets the bar.")]
		[Syncfusion.Documentation.DocumentationExclude()]
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        [ DesignerSerializationVisibility( DesignerSerializationVisibility.Content ) ]
#else
        [ DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ) ]
#endif
        public override Bar Bar
		{
			get{return base.Bar;}
			set
			{
				if(base.Bar != value)
				{
					if(this.designTimeBarManager != null && value != null)
						value.Manager = this.designTimeBarManager;

                    if (base.Bar != null && base.Bar.Items != null )
					{
						base.Bar.Items.CollectionChanged -= new CollectionChangeEventHandler( Items_CollectionChanged );
					}
					base.Bar = value;

					if( base.Bar != null && base.Bar.Items != null )
					{
						base.Bar.Items.CollectionChanged += new CollectionChangeEventHandler( Items_CollectionChanged );
					}
					// So that the DesignMode property will get propogated
					if(base.Bar != null && base.Bar.Items != null)
						base.Bar.Items.Parent = this;
		
					if(this.separatorList != null)
						this.UpdateSeparatorIndices();

					this.internalBar = null;
				}
			}
		}

		[Browsable(false),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public void InitBarFromDesigner(BarManager manager)
		{
            if (manager != this.designTimeBarManager)
            {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                UnSubscribeDragDrop();
#endif

				this.designTimeBarManager = manager;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                SubscribeDragDrop();
#endif

				if(this.Bar != null)
				{
					Bar tempBar = this.Bar;
					this.Bar = null;
					tempBar.Manager = this.designTimeBarManager;
					this.Bar = tempBar;
				}
			}
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        protected internal void SubscribeDragDrop()
        {
            if( designTimeBarManager != null )
            {
                designTimeBarManager.RegisterDragDropControl( this );
            }
        }

        protected internal void UnSubscribeDragDrop()
        {
            if( designTimeBarManager != null )
            {
                designTimeBarManager.UnRegisterDragDropControl( this );
            }
        }
#endif

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal override void RendererChanged(BarRenderer rendererNew)
		{
            if (!m_bIsDisposing)
            {
				BarRenderer newRenderer = null;
				
				if( m_bShowChevron )
				{
					newRenderer = new XpToolBarChevronRenderer( this );
				}
				else
				{
					newRenderer = new BarControlBarRenderer( this );
				}

				newRenderer.ThemesEnabled = this.ThemesEnabled;
				base.RendererChanged( newRenderer );
		
				this.barRenderer.ValidateDndHelper();
				this.barRenderer.dndHelper.AllowRecord = false;
				this.barRenderer.HintViaHotKeyPrefix = false;

				SetRendererDock( barRenderer, this.Dock );
            }
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public bool IsItemHit(Point ptClient)
		{
			int hitItem = this.barRenderer.HitTestBarItems(ptClient);
			if(hitItem != -1)
				return true;
			else
				return false;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void Bar_PropertyChanged(object sender, SyncfusionPropertyChangedEventArgs e)
		{
			base.Bar_PropertyChanged(sender, e);
			
			if(this.DesignMode)
			{
				IToolbarDesigners toolbarDesigner = (this.GetService(typeof(IDesignerHost)) as IDesignerHost).GetDesigner(this) as IToolbarDesigners;
				if(toolbarDesigner != null)
					toolbarDesigner.SetDirty();
			}
		}

		[Documentation.DocumentationExclude()]
		protected override bool GetUseControlForeColor()
		{
			return true;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override bool RequiresActiveFormForMouseTrack()
		{
			// If native app, then return false.
			// The idea is that if a native app, then mouse track all the time
			// .. if a .Net app then mouse track only when the parent Form has focus.
			return s_isDevEnv;
		}

        /// <summary>
        /// Read only. Gets the invisible BarItems that are hidden in the popup when the ShowChevron is set to true.
        /// </summary>
        /// <value>Array or BarItems.</value>
        /// <remarks>Should be used when ShowChevron is set to true</remarks>
        [Browsable(false), ReadOnly(true),
		Syncfusion.Documentation.DocumentationExclude()
		]
        public BarItems HiddenBarItems
        {
            get
            {
                BarItems barItems = new BarItems();
                barItems.AddRange(this.barRenderer.InvisibleBarItems);

                if (this.ShowChevron && barItems.Count >= 0)
                    return barItems;

                else
                    return new BarItems();
            }
        }

		#region BAR_REFLECTED_MEMBERS
		/// <summary>
		/// Returns the collection of BarItem objects associated 
		/// with this XPToolBar.
		/// </summary>
		/// <remarks>
		/// A BarItems collection that represents the list of BarItem objects 
		/// stored in the XPToolBar.
		/// <para>You can use this property to obtain a reference to the list of bar items 
		/// that are currently stored in the XPToolBar. With the reference to the 
		/// collection of bar items for the XPToolBar (provided by this property), 
		/// you can add and remove bar items, determine the total number of bar items 
		/// and clear the list of bar items from the collection.</para>
		/// </remarks>
		/// <example>
		/// The following example code adds three bar items to the XPToolBar.
		/// <code>
		/// private void Form_Load(object sender, System.EventArgs e)
		/// {
		/// 	this.barControl1.Items.Add(this.barItem1);
		/// 	this.barControl1.Items.Add(this.barItem2);
		/// 	this.barControl1.Items.Add(this.parentBarItem1);
		/// }
		/// </code>
		/// </example>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Description( "Indicates the BarItems collections." )
		]
		public BarItems Items
		{
			get
			{
				if(this.Bar == null)
					return null;
				return this.Bar.Items;
			}
		}
		/// <summary>
		/// Advanced property, meant for use at design-time.
		/// </summary>
		/// <remarks>Do not use this property directly.</remarks>
		[
		Browsable(false),
		DefaultValue(null),
		]
		public IntListDesignTime UpdatedSeparatorPositions
		{
			get{return this.separatorList.DesignTimeChanges;}
			set
			{
				this.separatorList.DesignTimeChanges = value;
			}
		}
		/// <summary>
		/// Advanced property, meant for use at design-time.
		/// </summary>
		/// <remarks>Do not use this property directly.</remarks>
		[
		Browsable(false),
		DefaultValue(null),
		]
		public IntListDesignTime UpdatedBarItemPositions
		{
			get
			{
				if (this.Bar.Items != null)
					return (this.Bar.Items.DesignTimeChanges);
				return null;
			}
			set
			{
				this.Bar.Items.DesignTimeChanges = value;
			}
		}
		
		/// <summary>
		/// Gets / sets the background color, gradient and other styles of the toolbar.
		/// </summary>
		/// <remarks>
		/// The <see cref="TreeViewAdv"/> provides this property to enable specialized
		/// custom gradient backgrounds. This property is used only when XP Themes is disabled
		/// through the <see cref="ThemesEnabled"/> property or when themes are inactive in the OS.
		/// </remarks>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Appearance"),
		Description("Lets you set the background color, gradient, etc. when not themed.")
		]
		public BrushInfo BackgroundColor
		{
			get
			{
				return this.bgBrush;
			}

			set
			{
				if(this.bgBrush != value)
				{
					this.bgBrush = value;
					this.Invalidate();
				}
			}
		}
		private void ResetBackgroundColor()
		{
			this.BackgroundColor = BrushInfo.Empty;
		}
		private bool ShouldSerializeBackgroundColor()
		{
			if(this.bgBrush == BrushInfo.Empty)
				return false;
			else
				return true;
		}

		/// <summary>
		/// Indicates whether the images from the <see cref="BarItem.LargeImageList"/>
		/// of the <see cref="BarItem"/>s should be used while drawing the BarItems.
		/// </summary>
		/// <value>True to use the large image list; false to use the default image list. Default value is false.</value>
		/// <remarks>
		/// Make sure that a large image list is associated with the BarItem when you
		/// set this property to true. Exceptions will be thrown otherwise.
		/// </remarks>
		[
		Browsable(true),
		Category("Appearance"),
		DefaultValue(false),
		Description("Indicates whether the images from the LargeImageList should be used.")]
		public override bool LargeIcons
		{
			get
			{
				return this.largeIcons;
			}
			set
			{
				if(this.largeIcons != value)
				{
					this.largeIcons = value;

					foreach( BarItem bi in this.Items )
					{
						bi.LargeIcons = value;
					}

					this.OnBarBoundsChanged();
				}
			}
		}

		private void SeparatorList_Changed(object sender, CollectionChangeEventArgs e)
		{
			// Update my separators hashtable here.
			this.Bar.ClearSeparators();
			foreach(int index in this.separatorList)
			{
				if(index < this.Items.Count)
					this.Bar.BeginGroupAt(this.Items[index]);
			}
		}
		// Code relies on SeparatorIndices getting called after Items in "InitializeComponent"
		// due to alphabetical order
		// Serializing this list in design-time is redundant here (since the content gets stored in the underlying Bar's constructor),
		// but is still necessary for to support Visual Inheritance.

        /// <summary>
        /// gets the separator indices.
        /// </summary>
        [Description("Gets the separator Indices.")]
		[
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
#else
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
#endif
		EditorBrowsable( EditorBrowsableState.Advanced ),
		Syncfusion.Documentation.DocumentationExclude(),
		Editor( typeof( SeparatorIndexCollectionEditor ), typeof( UITypeEditor ) )
		]
		public VisuallyInheritableIntList SeparatorIndices
		{
			get{return this.separatorList;}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void UpdateSeparatorIndices()
		{
			this.separatorList.SuspendEvents();
			this.separatorList.Clear();

			if(this.Items == null)
				return;

			int i = -1;
			foreach(BarItem item in this.Items)
			{
				i++;
				if(this.Bar.IsGroupBeginning(item))
					this.separatorList.Add(i);
			}
			this.separatorList.ResumeEvents(false);
		}

		/// <summary>
		/// Lets you specify a separator in the BarItems list. The separator will be
		/// just before the specified BarItem.
		/// </summary>
		/// <param name="barItem">A BarItem present in the Items list.</param>
		public void BeginGroupAt(BarItem barItem)
		{
			this.Bar.BeginGroupAt(barItem);
			this.UpdateSeparatorIndices();
		}
		/// <summary>
		/// Removes the separator just before this BarItem.
		/// </summary>
		/// <param name="barItem">A BarItem present in the Items list.</param>
		public void RemoveGroupAt(BarItem barItem)
		{
			this.Bar.RemoveGroupAt(barItem);
			this.UpdateSeparatorIndices();
		}
		/// <summary>
		/// Indicates whether a separator is drawn just before the specified BarItem.
		/// </summary>
		/// <param name="barItem">A BarItem present in the Items list.</param>
		/// <returns>True if there is a separator; false if not.</returns>
		public bool IsGroupBeginning(BarItem barItem)
		{
			return this.Bar.IsGroupBeginning(barItem);
		}
		#endregion BAR_REFLECTED_MEMBERS

		/// <summary>
		/// Returns the preferred size based on the current settings.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Size"/> instance.</value>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public Size PreferredSize
#else
		public new Size PreferredSize
#endif		
		{
			get
			{
				return this.GetPreferredSizeInternal( this.Width, this.Height );
			}
		}
		private Size GetPreferredSizeInternal( int curWidth, int curHeight )
		{
			SizeF prefSize = new SizeF( Int32.MaxValue, Int32.MaxValue );

			if( this.IsHorizontallyAligned )
			{
				prefSize.Width = curWidth;
				prefSize.Height = 10;
			}
			// For vertical alignment we want the items to be aligned vertically.
			// So, we specify that there is very little width and let the preferred size
			// be computed based on that width. The side effect is that when aligned
			// verticaly you will never see multi-lines.
			
			if( this.IsVerticallyAligned )
			{
				prefSize.Width = 10;
				prefSize.Height = curHeight;
			}

			GraphicsProvider gp = new GraphicsProvider( this );
			this.barRenderer.GetPreferredSize( gp, ref prefSize );
			gp.Dispose();

			prefSize.Width ++;
			prefSize.Height ++;
			return Size.Round( prefSize );
		}
		
		private bool IsHorizontallyAligned
		{
			get
		 {return this.Dock == DockStyle.Top || this.Dock == DockStyle.Bottom || this.Dock == DockStyle.Fill;}
		}
		private bool IsVerticallyAligned
		{
			get
			{return this.Dock == DockStyle.Left || this.Dock == DockStyle.Right;}
		}

		#region LAYOUT

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			Size prefSize = GetCorrectlySize(width, height);
			width = prefSize.Width;
			height = prefSize.Height;

			Point corectLocation = GetCorrectlyLocation(x, y, width, height);
			x = corectLocation.X;
			y = corectLocation.Y;
			base.SetBoundsCore(x, y, width, height, specified);
		}
			
		protected override void OnDockChanged(EventArgs e)
		{
			SetRendererDock(barRenderer, this.Dock);
			UpdateSize();
			
			base.OnDockChanged (e);
		}

		protected override void OnFontChanged(EventArgs e)
		{
            base.FontHeight = -1;

            foreach (BarItem item in this.Items)
            {
                item.CustomTextFont = this.Font;
            }

			this.UpdateSize();
			base.OnFontChanged(e);
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool NeedLayout
		{
			get{return this.needLayout;}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void SetNeedLayout(bool value)
		{
			this.needLayout = value;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public void OnBarBoundsChanged()
		{
			this.SetNeedLayout(true);
			this.Invalidate();
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		public void MoveMenuNavigation(bool forward)
		{
			
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnPaint"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e )
		{
			if(this.NeedLayout)
			{
				GraphicsProvider gp = new GraphicsProvider(e.Graphics);
				this.Layout(gp);
				gp.Dispose();
			}
			
			base.OnPaint(e);
		}
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			if (!this.UseThemes)
			{
				if (this.bgBrush.IsEmpty)
				{
					switch (this.Style)
					{
						case VisualStyle.Office2003:
							{
								DrawBackgroundOffice2003(e.Graphics);
								break;
							}
						case VisualStyle.Office2007Outlook:
						case VisualStyle.Office2007:
							{
								DrawBackgroundOffice2007(e.Graphics);
								break;
							}
                        case VisualStyle.Office2010:
                            {
                                DrawBackgroundOffice2010(e.Graphics);
                                break;
                            }
						case VisualStyle.VS2005:
							{
								DrawBackgroundVS2005(e.Graphics);
								break;
							}
						default:
							{
								base.OnPaintBackground(e);
								break;
							}
					}
				}
				else
				{
					base.OnPaintBackground(e);
					BrushPaint.FillRectangle(e.Graphics, this.ClientRectangle, this.bgBrush);
				}
			}
			else
			{
				base.OnPaintBackground(e);
			}
		}

		/// <summary>
		/// Draws background for Office2003 visual style.
		/// </summary>
		private void DrawBackgroundOffice2003( Graphics g )
		{
			BrushInfo background = new BrushInfo( GradientStyle.Vertical, 
				Office2003Colors.MenuMarginColorLight, 
				Office2003Colors.MenuMarginColorDark );
					
			BrushPaint.FillRectangle( g, this.ClientRectangle, background );
		}

		/// <summary>
		/// Draws background for VS2005 visual style.
		/// </summary>
		private void DrawBackgroundVS2005( Graphics g )
		{
			BrushInfo background = new BrushInfo( GradientStyle.Vertical, 
				VS2005Colors.CommandBarLightColor, 
				VS2005Colors.CommandBarDarkColor );
					
			BrushPaint.FillRectangle( g, this.ClientRectangle, background );
		}

		/// <summary>
		/// Draws background for Office2007 visual style.
		/// </summary>
		private void DrawBackgroundOffice2007( Graphics g )
		{
			Color color1 = Office2007ColorTable.GroupBarItemColorLight;
			Color color2 = Office2007ColorTable.GroupBarItemColorDark;

			Blend blend = new Blend();
			blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };
			blend.Factors = new float[] { 0.0F, 0.5F, 1.0F, 0.5F };

			using( LinearGradientBrush brush = new LinearGradientBrush( this.ClientRectangle, 
					   color1, color2, LinearGradientMode.Vertical ) )
			{
				brush.Blend = blend;
				g.FillRectangle( brush, this.ClientRectangle );
			}
		}

        /// <summary>
        /// Draws background for Office2010 visual style.
        /// </summary>
        private void DrawBackgroundOffice2010(Graphics g)
        {
            Color color1 = Office2010ColorTable.GroupBarItemColorLight;
            Color color2 = Office2010ColorTable.GroupBarItemColorDark;

            Blend blend = new Blend();
            blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };
            blend.Factors = new float[] { 0.0F, 0.5F, 1.0F, 0.5F };

            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                       color1, color2, LinearGradientMode.Vertical))
            {
                brush.Blend = blend;
                g.FillRectangle(brush, this.ClientRectangle);
            }
        }
        /// <summary>
        /// Specified whether themed background of the parent control will be drawn.
        /// </summary>
        /// <returns></returns>
		public override bool ShouldDelegateBGDrawingToParentWhenThemed()
		{
			return false;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void Layout(IGraphicsProvider gp)
		{
			this.SetNeedLayout(false);
			this.UpdateSize();
			base.Layout(gp);
		}
		private void UpdateSize()
		{
			if (this.Parent == null)
				return;

			this.Size = GetCorrectlySize(this.Width, this.Height);
		}

		#endregion LAYOUT

		#region DRAGANDDROP
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal override void ProcessDragOver(DragEventArgs drgevent)
		{	
			base.ProcessDragOver(drgevent);
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal override void ProcessDragDrop(System.Windows.Forms.DragEventArgs e)
		{
			base.ProcessDragDrop(e);
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal override void ProcessDragLeave(EventArgs e)
		{
			base.ProcessDragLeave(e);
		}
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnDragOver"/>.
		/// </summary>
		/// <param name="drgevent"></param>
		protected override void OnDragOver( DragEventArgs drgevent )
		{
			//if( !this.DesignMode )
			{
				base.OnDragOver( drgevent );
			}
		}
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnDragDrop"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDragDrop( System.Windows.Forms.DragEventArgs e )
		{
			//if( !this.DesignMode )
			{
				base.OnDragDrop( e );
			}
		}
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnDragLeave"/>.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDragLeave( EventArgs e )
		{
			//if( !this.DesignMode )
			{
				base.OnDragLeave( e );
			}
		}
		#endregion DRAGANDDROP

		private Hashtable m_shortcutsHash = new Hashtable();

		private BarItem GetItemByShortcut( int shortcut )
		{
			return m_shortcutsHash[ shortcut ] as BarItem;
		}

		private void Items_CollectionChanged( object sender, CollectionChangeEventArgs e )
		{
			BarItem barItem = e.Element as BarItem;

			if( null != barItem )
			{
				barItem.PropertyChanged += new SyncfusionPropertyChangedEventHandler(barItem_PropertyChanged);
				ParentBarItem parent = barItem as ParentBarItem;

				switch( e.Action )
				{
					case CollectionChangeAction.Add:
						if( parent != null )
						{				
							parent.Style = this.Style;
							
							if( parent.Items != null )
							{
								parent.Items.CollectionChanged += new CollectionChangeEventHandler( Items_CollectionChanged );
							}
						}

						IIgnoreWorkingArea iwa = barItem as IIgnoreWorkingArea;

						if( iwa != null )
						{
							iwa.IgnoreWorkingArea = this.IgnoreWorkingArea;
						}

						if( barItem.Shortcut != Shortcut.None )
						{
							this.m_shortcutsHash[(int)barItem.Shortcut] = barItem;
						}
						break;

					case CollectionChangeAction.Remove:
						if( parent != null && null != parent.Items )
						{
							parent.Items.CollectionChanged -= new CollectionChangeEventHandler( Items_CollectionChanged );
						}

						if( barItem.Shortcut != Shortcut.None )
						{
							this.m_shortcutsHash.Remove( ( int )barItem.Shortcut );
						}
						break;
				}
			}
		}

		private void barItem_PropertyChanged( object sender, SyncfusionPropertyChangedEventArgs e )
		{
			if( e.PropertyName == "Shortcut" )
			{
				if( e.OldValue != null && m_shortcutsHash[ ( int )e.OldValue ] != null )
				{
                  m_shortcutsHash.Remove( ( int )e.OldValue );
				}

				if( e.NewValue != null )
				{
                 m_shortcutsHash[ ( int )e.NewValue ] = sender;
				}
			}
			else if( e.PropertyName == "Visible" )
			{
				LayoutEventArgs args = new LayoutEventArgs( this, "Visible" );
				this.OnLayout( args );
			}
		}
		/// <summary>
		/// Shows the popup of the specified item.
		/// </summary>
		/// <param name="item">Parent bar item to show popup items.</param>
		public void ShowPopup( ParentBarItem item )
		{
			this.barRenderer.SetCurrentTrackItem( this.Items.IndexOf(item), true, true  );
		}

		/// <summary>
		/// Hides the currently open popup.
		/// </summary>
		public void HidePopup()
		{
			this.barRenderer.SetCurrentTrackItem( -1, false, false );
		}
		#region IIgnoreWorkingArea Members

		private bool m_bIgnoreWorkingArea = false;

		/// <summary>
        /// Provides information, whether popup is ignoring
        /// working area of the display before showing.
		/// </summary>
        [ DefaultValue( false ) ]
        [ Description("Provides information, whether popup is ignoring working area of the display before showing.") ]
		public bool IgnoreWorkingArea
		{
			get
			{
				return m_bIgnoreWorkingArea;
			}
			set
			{
				if( m_bIgnoreWorkingArea != value )
				{
					m_bIgnoreWorkingArea = value;
					foreach( BarItem bi in this.Items )
					{
						IIgnoreWorkingArea iwa = bi as IIgnoreWorkingArea;
						if( iwa != null )
						{
							iwa.IgnoreWorkingArea = this.IgnoreWorkingArea;
						}
					}
				}
			}
		}

		#endregion
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class BarControlBarRenderer : MultilineBarRenderer
	{
		public BarControlBarRenderer(IBarControl parent)
			: base(parent)
		{
		}
		protected override CustomizingPopupMenu CreateCustomizationPopup()
		{
			return ( null != this.Bar.Manager ) ? new BarControlCustomizingMenu() : null;
		}
		public override PopupRelativeAlignment GetFirstPopupAlignPreference()
		{
			switch(this.parent.GetControl().Dock)
			{
				default:
				case DockStyle.Top:
				case DockStyle.Fill:
				case DockStyle.None:
					return PopupRelativeAlignment.BottomLeft;
				case DockStyle.Left:
					return PopupRelativeAlignment.RightBottom;
				case DockStyle.Bottom:
					return PopupRelativeAlignment.TopRight;
				case DockStyle.Right:
					return PopupRelativeAlignment.LeftBottom;
			}
		}

		public Point GetLocationForPopupAlignment( PopupRelativeAlignment prevAlign,
			out PopupRelativeAlignment newAlign, bool bIsRightToLeft )
		{
			DropDownBarItemRenderer renderer = GetCurrentRenderer() as DropDownBarItemRenderer;

			if(renderer != null)
				return renderer.GetLocationForPopupAlignment( prevAlign, out newAlign, bIsRightToLeft );

			newAlign = PopupRelativeAlignment.Default;
			return Point.Empty;
		}

		public override void GetPreferredSize(IGraphicsProvider gp, ref SizeF preferredSize)
		{
			float availableWidth = preferredSize.Width;

			if( this.IsVerticallyAligned )
			{
				availableWidth = preferredSize.Height;
			}

			base.GetPreferredSize( gp, ref preferredSize );

			if( this.IsVerticallyAligned )
			{
				preferredSize = new SizeF( preferredSize.Height, preferredSize.Width );
			}
		}

		protected override bool ProcessKeyDown( Keys key )
		{
			bool bHandled = base.ProcessKeyDown( key );

			if( key == Keys.Tab )
			{
				bHandled = false;
			}

			return bHandled;
		}
	}
	
	internal class BarControlCustomizingMenu : CustomizingPopupMenu
	{
		BarItem moveNext, movePrevious;
		
		protected override void InitCustomizationMenu()
		{
			base.InitCustomizationMenu();
			this.moveNext = new BarItem(SR.GetString(SR.MoveNextMenuItemText));
			this.movePrevious = new BarItem(SR.GetString(SR.MovePrevMenuItemText));

			this.moveNext.Click += new EventHandler(this.CustomizingMenuItemClicked);
			this.movePrevious.Click += new EventHandler(this.CustomizingMenuItemClicked);

			this.customizationMenu.Items.Insert(1, this.movePrevious);
			this.customizationMenu.Items.Insert(1, this.moveNext);
		}

		protected override void CustomizingMenuItemClicked(object sender, EventArgs e)
		{
			if(sender == this.moveNext)
			{
				int index = this.parentItem.Items.IndexOf(this.selectedItem);
				if(index < this.parentItem.Items.Count - 1)
				{
					this.helper.InsertItem(this.parentItem.Items[index + 1],
						this.selectedItem, false);
				}
			}
			else if(sender == this.movePrevious)
			{
				int index = this.parentItem.Items.IndexOf(this.selectedItem);
				if(index > 0)
				{
					this.helper.InsertItem(this.parentItem.Items[index - 1],
						this.selectedItem, true);
				}
			}
			else base.CustomizingMenuItemClicked(sender, e);
		}
	}

	internal class SeparatorIndexCollectionEditor:
		CollectionEditor
	{
		public SeparatorIndexCollectionEditor( Type type ):
			base( type )
		{
		}

		public SeparatorIndexCollectionEditor():
			base(typeof( VisuallyInheritableIntList ))
		{
		}

		protected override object CreateInstance( Type itemType )
		{
			return (int)1;
		}

		protected override Type[] CreateNewItemTypes()
		{
			return new Type[] { typeof(int) };
		}
	}
}
