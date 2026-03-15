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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Layout;
using System.Collections;

using Syncfusion.Windows.Forms.Tools.Win32API;
using System.ComponentModel.Design.Serialization;
using System.CodeDom;
using Syncfusion.Runtime.Serialization;
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary>
	/// RibbonControl advanced.
	/// </summary>
	[
	Designer( typeof( Design.RibbonControlAdvDesigner ) ),
	ToolboxBitmap( typeof( RibbonControlAdv ), "ToolboxIcons.RibbonControlAdv.bmp" ),
	ProvideProperty( "Description", typeof( Component ) ),
	ProvideProperty( "UseInQuickAccessMenu", typeof( Component ) ),
    ProvideProperty( "UseInCustomQuickAccessDialog", typeof( Component ) ),
	Localizable(true),
	Description("Provides advanced options to create Office 2007 Style Ribbon.")
	]
	public class RibbonControlAdv : ContainerControl, IOffice12Settings, IDockExtended, IExtenderProvider, ISupportInitialize, IMessageFilter, IRibbonStyleNotifier,IVisualStyle 
	{
		#region Constants
		/// <summary>
		/// Width of border.
		/// </summary>
		const int BORDER_TOP = 1;
		const int BORDER_BOTTOM = 1;
		/// <summary>
		/// Size of Left and Right margins of Ribbon panels
		/// </summary>
		const int PANEL_MARGIN = 4;
        /// <summary>
        /// Size of 125 DPI Left and Right margins of Ribbon panels
        /// </summary>
        const int DPI_125_PANEL_MARGIN = 7;
        /// <summary>
        /// Size of 150 DPI Left and Right margins of Ribbon panels
        /// </summary>
        const int DPI_150_PANEL_MARGIN = 9;
		/// <summary>
		/// Offset in pixels from screen used in RibbonPopup positioning.
		/// <summary>
		/// Width of menu button.
		/// </summary>
		internal const int DEF_MENU_BUTTON_WIDTH = 40;
        /// Width of menu button.
        /// </summary>
        internal const int DEF_MENU_BUTTON_WIDTH_125 = 56;
        /// Width of menu button.
        /// </summary>
        internal const int DEF_MENU_BUTTON_WIDTH_150 = 70;
		/// <summary>
		/// Width of menu button.
		/// </summary>
		internal const int DEF_2010_MENU_BUTTON_WIDTH = 56;
		/// <summary>
		/// 
		/// </summary>
		const SetWindowPosFlags SWP_SHOWPOPUP =
			SetWindowPosFlags.SWP_SHOWWINDOW |
			SetWindowPosFlags.SWP_NOACTIVATE;
		/// <summary>
		/// 
		/// </summary>
		const SetWindowPosFlags SWP_HIDEPOPUP =
			SetWindowPosFlags.SWP_HIDEWINDOW |
			SetWindowPosFlags.SWP_NOACTIVATE |
			SetWindowPosFlags.SWP_NOZORDER |
			SetWindowPosFlags.SWP_NOMOVE |
			SetWindowPosFlags.SWP_NOSIZE;

		const RedrawWindowFlags rdwUpdate = 
			RedrawWindowFlags.RDW_INVALIDATE | 
			RedrawWindowFlags.RDW_UPDATENOW;

		#endregion

		#region Nested classes

		#region *** RibbonControlLayout
		/// <summary>
		/// Layout engine for RibbonControlAdv.
		/// </summary>
		class RibbonControlLayout
			: LayoutEngine
		{
			#region Overrides
			/// <summary>
			/// Lays out items of RibbonControlAdv.
			/// </summary>
			/// <param name="container"></param>
			/// <param name="layoutEventArgs"></param>
			/// <returns></returns>
			public override bool Layout( object container, LayoutEventArgs layoutEventArgs )
			{
				RibbonControlAdv ribbon = container as RibbonControlAdv;
				if( ribbon != null )
				{
					Rectangle rcClient = ribbon.ClientRectangle;
					Rectangle rcDisplay = ribbon.DisplayRectangle;
					
					Size szHeader = ribbon.HeaderInternal.GetPreferredSize( Size.Empty );

					int iPanelHeight = rcDisplay.Height - szHeader.Height;

					if (ribbon.BottomToolstripVisible)
					{
						BottomToolstrip bottomToolstrip = ribbon.BottomToolstrip;

						int bottomToolstripHeight = bottomToolstrip.Height + bottomToolstrip.Margin.Vertical;
						int iBottomToolstripY = rcDisplay.Bottom - bottomToolstripHeight + bottomToolstrip.Margin.Top;

						bottomToolstrip.Width = rcDisplay.Width - bottomToolstrip.Margin.Horizontal;
						bottomToolstrip.Location = new Point(rcDisplay.X + bottomToolstrip.Margin.Left, iBottomToolstripY);

						iPanelHeight -= bottomToolstripHeight;
					}

					if (iPanelHeight >= 0)
					{
						if (ribbon.ShowPanel && !ribbon.MinimizePanel)
						{
							Rectangle rc = new Rectangle(rcDisplay.X + PANEL_MARGIN, rcDisplay.Y + szHeader.Height, rcDisplay.Width - 2 * PANEL_MARGIN, iPanelHeight);
                            using (Graphics g = Graphics.FromImage(new Bitmap (10,10)))
                            {
                                if (g.DpiX > 120)
                                {
                                    rc = new Rectangle(rcDisplay.X + DPI_150_PANEL_MARGIN, rcDisplay.Y + szHeader.Height, rcDisplay.Width - 2 * DPI_150_PANEL_MARGIN, iPanelHeight);
                                }
                                else if (g.DpiX > 96)
                                {
                                    rc = new Rectangle(rcDisplay.X + DPI_125_PANEL_MARGIN, rcDisplay.Y + szHeader.Height, rcDisplay.Width - 2 * DPI_125_PANEL_MARGIN, iPanelHeight);
                                }
                            }
                            if(ribbon.RibbonStyle ==RibbonStyle.Office2013)
								rc = new Rectangle(rcDisplay.X , rcDisplay.Y + szHeader.Height, rcDisplay.Width , iPanelHeight);
							foreach (Control c in ribbon.Controls)
							{
								if (c is RibbonPanel && c.Visible)
								{
									c.Bounds = rc;
								}
							}
						}
						else szHeader.Height += iPanelHeight;
					}
                    if (ribbon.HeaderInternal.AutoHide && (ribbon.BackStageView == null || (ribbon.BackStageView != null && !ribbon.BackStageView.IsVisible )))
                    {
                        if(ribbon.RibbonStatus)
                            ribbon.HeaderInternal.Bounds = new Rectangle(rcDisplay.X, rcDisplay.Y, rcDisplay.Width,szHeader.Height);
                        else
                            ribbon.HeaderInternal.Bounds = new Rectangle(rcDisplay.X, rcDisplay.Y, rcDisplay.Width, ribbon.RibbonAutoHideHeight);
                    }
                    else
                        ribbon.HeaderInternal.Bounds = new Rectangle(rcDisplay.X, rcDisplay.Y, rcDisplay.Width, szHeader.Height);

					return ribbon.AutoSize;
				}
				return base.Layout( container, layoutEventArgs );
			}
			#endregion
		}
		#endregion

		#region *** RibbonControlPopup
		internal class RibbonControlPopup : Control, IOffice12Settings
		{
			#region Constructors
			public RibbonControlPopup(RibbonControlAdv owner)
			{
				m_owner = owner;

				SetStyle(ControlStyles.ContainerControl, true);
			}
			#endregion

			#region Properties
			/// <summary>
			/// 
			/// </summary>
			protected override CreateParams CreateParams
			{
				get
				{
					CreateParams cp = base.CreateParams;

					cp.Style |= unchecked((int)(WindowStyles.WS_POPUP));

					return cp;
				}
			}
			/// <summary>
			/// Gets owner for RibbonControlPopup control.
			/// </summary>
			internal RibbonControlAdv Owner
			{
				get
				{
					return m_owner;
				}
			}
			#endregion

			#region Overrides
			/// <summary>
			/// 
			/// </summary>
			/// <param name="levent"></param>
			protected override void OnLayout(LayoutEventArgs levent)
			{
				foreach (Control c in this.Controls)
				{
					if (c.Visible)
					{
						c.SetBounds(0, 0, this.Width, this.Height);
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			protected override void WndProc(ref Message m)
			{
				switch ((Msg)m.Msg)
				{
					case Msg.WM_SIZE:
						UpdateRegion(WindowsAPI.LOW_ORDER((int)m.LParam), WindowsAPI.HIGH_ORDER((int)m.LParam));
						break;
					case Msg.WM_MOUSEACTIVATE:
						m.Result = (IntPtr)MouseActivateFlags.MA_NOACTIVATE;
						return;
				}
				base.WndProc(ref m);
			}
			#endregion
			
			#region Implementation
			/// <summary>
			/// 
			/// </summary>
			/// <param name="width"></param>
			/// <param name="height"></param>
			void UpdateRegion(int width, int height)
			{
				Region region = null;

				if (width > 0 && height > 0)
				{
					region = RendererUtils.GetRoundedRegion(new Rectangle(0, 0, width, height), 2);
				}

				this.Region = region;
			}
			/// <summary>
			/// Creates a handle and give back lost focus to the RibbonControlAdv control.
			/// </summary>
			internal void CreateHandleInternal()
			{
				CreateHandle();
				m_owner.Focus();
			}
			#endregion

			#region Fields
			/// <summary>
			/// Owner for RibbonControlPopup control.
			/// </summary>
			RibbonControlAdv m_owner;
			#endregion

			#region IOffice12Settings Members
			LauncherStyle IOffice12Settings.LauncherStyle
			{
				get { return m_owner.LauncherStyle; }
				set { m_owner.LauncherStyle = value; }
			}
			bool IOffice12Settings.ShowCaption
			{
				get { return m_owner.ShowCaption; }
				set { m_owner.ShowCaption = value; }
			}
			bool IOffice12Settings.ShowLauncher
			{
				get { return m_owner.ShowLauncher; }
				set { m_owner.ShowLauncher = value; }
			}
			CaptionStyle IOffice12Settings.CaptionStyle
			{
				get { return m_owner.CaptionStyle; }
				set { m_owner.CaptionStyle = value; }
			}
			CaptionTextStyle IOffice12Settings.CaptionTextStyle
			{
				get { return m_owner.CaptionTextStyle; }
				set { m_owner.CaptionTextStyle = value; }
			}
			public RibbonStyle RibbonStyle
			{
				get { return m_owner.RibbonStyle; }
			}
            public RightToLeft  RightToLeft
            {
                get { return m_owner.RightToLeft; }
            }
            [Browsable(false)]
            public Office2013ColorScheme Office2013ColorScheme
            {
                get { return m_owner.Office2013ColorScheme; }
            }
            [Browsable(false)]
            public Color MenuColor
            {
                get { return m_owner.MenuColor; }
            }
        /// <summary>
        /// Gets whether default highlight color should be used 
        /// </summary>
            internal bool UseDefaultHighlightColor
            {
                get { return m_owner.UseDefaultHighlightColor; }
            }
			CaptionAlignment IOffice12Settings.CaptionAlignment
			{
				get { return m_owner.CaptionAlignment; }
				set { m_owner.CaptionAlignment = value; }
			}
			Font IOffice12Settings.CaptionFont
			{
				get { return m_owner.CaptionFont; }
				set { m_owner.CaptionFont = value; }
			}
			int IOffice12Settings.CaptionMinHeight
			{
				get { return m_owner.CaptionMinHeight; }
				set { m_owner.CaptionMinHeight = value; }
			}
			ToolStripBorderStyle IOffice12Settings.BorderStyle
			{
				get { return m_owner.BorderStyle; }
				set { m_owner.BorderStyle = value; }
			}
			ToolStripEx.ColorScheme IOffice12Settings.OfficeColorScheme
			{
				get { return m_owner.OfficeColorScheme; }
				set { m_owner.OfficeColorScheme = value; }
			}
			#endregion
		}
		#endregion

		#endregion

		#region Constructors
		/// <summary>
		/// Initializes static members.
		/// </summary>
		static RibbonControlAdv()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(RibbonControlAdv));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
			m_ColorTables = new Hashtable();
			m_ColorTables[ToolStripEx.ColorScheme.Managed] = Office12ColorTable.ManagedColors;
			m_ColorTables[ ToolStripEx.ColorScheme.Silver ] = new Office12ColorTable();
			m_ColorTables[ ToolStripEx.ColorScheme.Blue ] = new OfficeBlue();
			m_ColorTables[ ToolStripEx.ColorScheme.Black ] = new OfficeBlack();
		}
		/// <summary>
		/// Creates and initializes new instance of RibbonControlAdv.
		/// </summary>
		public RibbonControlAdv() : this(new Size(100, 100))
		{
		}
		/// <summary>
		/// Creates and initializes new instance of RibbonControlAdv.
		/// </summary>
		/// <param name="size">Control's initial size.</param>
		public RibbonControlAdv( Size size )
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				new Syncfusion.Core.Licensing.LicensedComponent( typeof( RibbonControlAdv ) );
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}

			m_descriptions = new Dictionary<Component, string>();
			m_useInQMenuExtProp = new List<Component>();
            m_useInCustomQDialog = new List<Component>();

			base.Dock = DockStyle.None;

			this.SetStyle( ControlStyles.AllPaintingInWmPaint, true );

			m_contextMenuStrip = new ContextMenuStripEx();
			m_contextMenuSep = new ToolStripSeparator();
			m_contextMenuSep2 = new ToolStripSeparator();
			m_contextMenuSep3 = new ToolStripSeparator();

			m_contextMenuAddToQuick = new ToolStripMenuItem(null, null, OnAddToQuickPanelMenuItemClick);
			m_contextMenuRemoveFromQuick = new ToolStripMenuItem(null, null, OnRemoveFromQuickPanelMenuItemClick);
			m_contextMenuCustomize = new ToolStripMenuItem(null, null, OnCustomizeMenuItemClick);
			m_contextMenuShowBelow = new ToolStripMenuItem( null, null, OnShowBelowMenuItemClick );
			m_contextMenuMinimize = new ToolStripMenuItem();
			m_contextMenuMinimize.CheckOnClick = true;
			m_contextMenuMinimize.CheckStateChanged += new EventHandler( OnContextMenuMinimizeCheckStateChanged );

			m_contextMenuItems = new List<ToolStripItem>();

			this.TabStop = false;
		}
		#endregion
		 internal Size rSize;		
		#region Event Handlers
        /// <summary>
        /// Called by the delegate before context menu opens.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnBeforeContextMenuOpen(object sender, ContextMenuEventArgs e)
        {
            if (BeforeContextMenuOpen != null)
            {
                BeforeContextMenuOpen(sender, e);
            }
        }
		/// <summary>
		/// Called when header item is added.
		/// </summary>
		/// <param name="e">The <see cref="T:System.Windows.Forms.ToolStripItemEventArgs"/> instance containing the event data.</param>
		void OnHeaderItemAdded( object sender, ToolStripItemEventArgs e )
		{
			ToolStripTabItem item = e.Item as ToolStripTabItem;
			if( item != null )
			{
				item.Panel.Parent = this.MinimizePanel ? this.RibbonPopup as Control : this;
			}
		}
		/// <summary>
		/// Called when header item is removed.
		/// </summary>
		/// <param name="e">The <see cref="T:System.Windows.Forms.ToolStripItemEventArgs"/> instance containing the event data.</param>
		void OnHeaderItemRemoved( object sender, ToolStripItemEventArgs e )
		{
			ToolStripTabItem item = e.Item as ToolStripTabItem;
			if( item != null )
			{
				item.Panel.Parent = null;
			}
		}
		/// <summary>
		/// Called when parent style is changed.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		void OnParentStyleChanged( object sender, EventArgs e )
		{
			this.HeaderInternal.UpdateSystemButtons();
		}
		/// <summary>
		/// Called when parent is activated or deactivated.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		void OnParentStateChanged(object sender, EventArgs e)
		{
			WindowsAPI.RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, rdwUpdate);
			WindowsAPI.RedrawWindow(this.HeaderInternal.Handle, IntPtr.Zero, IntPtr.Zero, rdwUpdate);
		}
		private Color activeTabGroupColor = Color.Transparent;
		internal Color ActiveTabGroupColor
		{
			get { return activeTabGroupColor; }
			set { activeTabGroupColor = value; }
		}

		protected override void OnParentFontChanged(EventArgs e)
		{
			this.HeaderInternal.PerformLayout();
			this.HeaderInternal.Invalidate();
			base.OnParentFontChanged(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnParentMdiChildActivate(object sender, EventArgs e)
		{
			Form f = sender as Form;
			if (f != null && f.ActiveMdiChild!=null)
			{
				Form mdiChild = f.ActiveMdiChild;
				mdiChild.TextChanged += new EventHandler(mdiChild_TextChanged);
				mdiChild.Deactivate += new EventHandler(mdiChild_Deactivate);
				if (mdiChild != null && mdiChild.WindowState == FormWindowState.Maximized)
				{
					this.HeaderInternal.Invalidate();
				}
			}
		}

		void mdiChild_Deactivate(object sender, EventArgs e)
		{
			Form mdiChild = sender as Form;
			mdiChild.TextChanged -= new EventHandler(mdiChild_TextChanged);
			mdiChild.Deactivate -= new EventHandler(mdiChild_Deactivate);
		}

		void mdiChild_TextChanged(object sender, EventArgs e)
		{
			Form mdiChild = sender as Form;
			if (mdiChild.WindowState == FormWindowState.Maximized)
			{
				this.HeaderInternal.PerformLayout();
				this.HeaderInternal.Invalidate();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnParentShowSystemMenu(object sender, EventArgs e)
		{
			this.HeaderInternal.ShowSystemMenu();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnParentRegionChanged( object sender, EventArgs e )
		{
			UpdateRegion( sender as Control );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="szProposal"></param>
		void OnParentGetMinSize( ref Size szProposal )
		{
			Size size = this.HeaderInternal.GetMinimumSize();
			size.Height = this.Height;

			if( szProposal.Width < size.Width )
			{
				szProposal.Width = size.Width;
			}
			if( szProposal.Height < size.Height )
			{
				szProposal.Height = size.Height;
			}
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdMargings"></param>
        void OnParentGetMargins(ref Padding pdMargings)
        {
            if (this.Dock == DockStyleEx.TopMost)
            {
                RibbonControlAdvHeader header = this.HeaderInternal;
                pdMargings.Top = header.Top + header.DisplayRectangle.Top + header.QuickPanelHeight + 1;

                if (this.RibbonStyle == Tools.RibbonStyle.Office2010 || this.RibbonStyle == Tools.RibbonStyle.Office2013)
                    pdMargings.Top += header.TabItemsRectangle.Height + 8;
            }
            else if (this.Dock == DockStyleEx.BottomMost)
            {
                RibbonControlAdvHeader header = this.HeaderInternal;
                pdMargings.Top = header.DisplayRectangle.Height - (header.Height + header.QuickPanelHeight + 1);

                if (this.RibbonStyle == Tools.RibbonStyle.Office2010 || this.RibbonStyle == Tools.RibbonStyle.Office2013)
                    pdMargings.Top += header.TabItemsRectangle.Height + 8;
            }
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnHeaderDoubleClick( object sender, EventArgs e )
		{
			MouseEventArgs mea = e as MouseEventArgs;
			if (mea != null && mea.Button==MouseButtons.Left)
			{
				ToolStripTabItem item = this.HeaderInternal.GetItemAt(mea.Location) as ToolStripTabItem;
				if( item != null && item.Checked && item.DoubleClickEnabled )
				{
					this.MinimizePanel = !this.MinimizePanel;
				}
			}
		}
		/// <summary>
		/// Adds item to quick panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnAddToQuickPanelMenuItemClick( object sender, EventArgs e )
		{
			Component comp = ( ( ToolStripMenuItem )sender ).Tag as Component;

			if( comp != null )
			{
				ToolStripItem quickItem = QuickToolstripReflectable.GetItemToReflect( comp );

				if( quickItem != null )
				{
					this.Header.AddQuickItem( quickItem );
				}
			}
		}
		/// <summary>
		/// Removes item from quick panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnRemoveFromQuickPanelMenuItemClick( object sender, EventArgs e )
		{
			ToolStripItem item = ( ( ToolStripMenuItem )sender ).Tag as ToolStripItem;

			if( item != null )
			{
				this.HeaderInternal.QuickItems.Remove(item);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnCustomizeMenuItemClick( object sender, EventArgs e )
		{
			this.ShowCustomizeDialog();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnContextMenuMinimizeCheckStateChanged( object sender, EventArgs e )
		{
			this.MinimizePanel = m_contextMenuMinimize.Checked;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnShowBelowMenuItemClick( object sender, EventArgs e )
		{
			this.ShowQuickPanelBelowRibbon = !this.ShowQuickPanelBelowRibbon;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnHeaderSizeChanged(object sender, EventArgs e)
		{
			Control c = sender as Control;
			if (c != null)
			{
				int minHeight = c.Height;

                if (this.Height < minHeight)
                {
                    if (this.RibbonStatus && this.HeaderInternal.AutoHide && this.RibbonStyle == Tools.RibbonStyle.Office2013)
                        if (this.RibbonTouchModeEnabled)
                        {
                            this.Height = rSize.Height + RibbonTouchHeight;
                            this.PerformLayout();
                        }
                        else
                            this.Height = rSize.Height;
                    else if (this.RibbonStyle == Tools.RibbonStyle.Office2013 && !this.MinimizePanel)
                    {
                        if (this.RibbonTouchModeEnabled)
                            this.Height = rSize.Height + RibbonTouchHeight;
                        else
                            this.Height = rSize.Height;
                    }
                    else
                        this.Height = minHeight;
                }
                else if (RibbonStyle == Tools.RibbonStyle.Office2013 && this.HeaderInternal.AutoHide && !this.RibbonStatus)
                    this.Height = RibbonAutoHideHeight;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnApplicationIdle( object sender, EventArgs e )
		{
			if( m_bUpdateUIOnAppIdle )
			{
				OnUpdateUIOnAppIdle( EventArgs.Empty );
			}
		}
		/// <summary>
		/// Raises the UpdateUI event.
		/// </summary>
		/// <param name="args">An EventArgs that contains the event data.</param>
		private void OnUpdateUIOnAppIdle( EventArgs args )
		{
			if( UpdateUI != null )
			{
				UpdateUI( this, args );
			}
		}
		private void DropDown_Opened(object sender, EventArgs e)
		{
			OnAfterCustomizeDropDownPopup(e);
		}
		private void DropDown_Opening(object sender, EventArgs e)
		{
			DropDownEventArgs args = new DropDownEventArgs((sender as ToolStripDropDownButton).DropDown);
			OnBeforeCustomizeDropDownPopup(args); 
		}
		private void m_header_SelectedTabChanged(object sender, SelectedTabChangedEventArgs e)
		{
			OnSelectedTabItemChanged(e);
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="proposedSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSize(Size proposedSize)
		{
			Size size = this.HeaderInternal.GetPreferredSize(Size.Empty);

			if (!this.MinimizePanel && this.ShowPanel)
			{
				Size szPanels = Size.Empty;

				foreach (ToolStripTabItem item in this.HeaderInternal.MainItems)
				{
					RibbonPanel panel = item.Panel;
					
					if (panel != null)
					{
						Size szPanel = panel.GetPreferredSize(Size.Empty);

						if (szPanels.Width < szPanel.Width)
						{
							szPanels.Width = szPanel.Width;
						}

						if (szPanels.Height < szPanel.Height)
						{
							szPanels.Height = szPanel.Height;
						}
					}
				}

				if (size.Width < szPanels.Width)
				{
					size.Width = szPanels.Width;
				}
				size.Height += szPanels.Height;
			}

			if (this.ShowQuickPanelBelowRibbon)
			{
				Control c = this.BottomToolstrip;
				
				size.Height += c.Height + c.Margin.Vertical;
			}

			size.Height += BORDER_TOP + BORDER_BOTTOM;

			// Width shouldn't be changed by default layout 
			// when control is docked to the TopMost or BottomMost
			if (this.Dock == DockStyleEx.TopMost || this.Dock == DockStyleEx.BottomMost)
			{
				size.Width = this.Width;
			}

			return size;
		}
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.ControlAdded"></see> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.ControlEventArgs"></see> that contains the event data.</param>
		protected override void OnControlAdded( ControlEventArgs e )
		{
			base.OnControlAdded( e );

			RibbonPanel panel = e.Control as RibbonPanel;
			if( panel != null )
			{
				panel.Visible = m_bVisiblePanel && panel.TabItem.Checked;
			}
		}
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.ControlRemoved"></see> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.ControlEventArgs"></see> that contains the event data.</param>
		protected override void OnControlRemoved( ControlEventArgs e )
		{
			base.OnControlRemoved( e );
		}
		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"></see> and its child controls and optionally releases the managed resources.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				DetachParent();

				m_contextMenuAddToQuick.Click -= new EventHandler(OnAddToQuickPanelMenuItemClick);
				m_contextMenuRemoveFromQuick.Click -= new EventHandler(OnRemoveFromQuickPanelMenuItemClick);
				m_contextMenuCustomize.Click -= new EventHandler(OnCustomizeMenuItemClick);
				m_contextMenuShowBelow.Click -= new EventHandler(OnShowBelowMenuItemClick);
				m_contextMenuMinimize.CheckStateChanged -= new EventHandler(OnContextMenuMinimizeCheckStateChanged);
                if (m_header != null)
                {
                    m_header.ItemAdded -= new ToolStripItemEventHandler(OnHeaderItemAdded);
                    m_header.ItemRemoved -= new ToolStripItemEventHandler(OnHeaderItemRemoved);
                    m_header.DoubleClick -= new EventHandler(OnHeaderDoubleClick);
                    m_header.SizeChanged -= new EventHandler(OnHeaderSizeChanged);
                    m_header.QuickAccessButton.DropDownOpening -= new EventHandler(DropDown_Opening);
                    m_header.QuickAccessButton.DropDownOpened -= new EventHandler(DropDown_Opened);
                    m_header.SelectedTabChanged -= new SelectedTabChangedEventHandler(m_header_SelectedTabChanged);
                    m_header = null;
                }

                if (m_contextMenuStrip != null)
                {
                    m_contextMenuStrip.Dispose();
                    m_contextMenuStrip = null;
                }

                if( m_bUpdateUIOnAppIdle )
                {
                    Application.Idle -= new EventHandler(OnApplicationIdle);
                }
            }

			base.Dispose( disposing );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnParentChanged( EventArgs e )
		{
			base.OnParentChanged( e );
			UpdateParent();
            setOffice2007ForeColor();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged( EventArgs e )
		{
			base.OnSizeChanged( e );
			UpdateRegion( this.Parent );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLocationChanged( EventArgs e )
		{
			base.OnLocationChanged( e );
			UpdateRegion( this.Parent );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnHandleCreated( EventArgs e )
		{
			base.OnHandleCreated( e );

			Application.AddMessageFilter(this);
			m_callWndProcHook = new CallWndProcHook(new WindowsAPI.WindowProc(this.CallWndProc));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnHandleDestroyed( EventArgs e )
		{
			Application.RemoveMessageFilter(this);
			
			m_callWndProcHook.Dispose();
			m_callWndProcHook = null;
			base.OnHandleDestroyed( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint( PaintEventArgs e )
		{
			Graphics g = e.Graphics;
            Rectangle rcClip = e.ClipRectangle;

            using (Brush brush = new SolidBrush(this.ColorTable.RibbonBorder))
            {
                if (this.RibbonStyle == RibbonStyle.Office2010 || this.RibbonStyle == RibbonStyle.Office2013)
                {
                    SolidBrush brushs = new SolidBrush(ColorTranslator.FromHtml("#BBCEE6"));
                    if (this.RibbonStyle == RibbonStyle.Office2013)
                    {
                        if(this.BottomToolstripVisible)
                            brushs = new SolidBrush(ColorTranslator.FromHtml("#F0F0F0"));
                        else
                            brushs = new SolidBrush(ColorTranslator.FromHtml("#b1b5ba"));
                        g.FillRectangle(brushs, rcClip);
                    }
                    else
                    {
                        if (m_parent == null || !m_parent.CompositionEnabled)
                        {
                            if (this.OfficeColorScheme == ToolStripEx.ColorScheme.Silver)
                                brushs = new SolidBrush(ColorTranslator.FromHtml("#E5E7E9"));
                            if (this.OfficeColorScheme == ToolStripEx.ColorScheme.Black)
                                brushs = new SolidBrush(ColorTranslator.FromHtml("#717171"));
                            if (this.OfficeColorScheme == ToolStripEx.ColorScheme.Blue)
                                brushs = new SolidBrush(ColorTranslator.FromHtml("#BBCEE6"));
                            g.FillRectangle(brushs, rcClip);
                        }
                        else
                            g.DrawLine(Pens.Gray, rcClip.X, rcClip.Height, rcClip.Width, rcClip.Height);
                    }
                    brushs.Dispose();
                }
                else
                    g.FillRectangle(brush, rcClip);
            }

            if (m_parent == null || !m_parent.CompositionEnabled)
            {
                //Draw vertical border lines to continue RibbonForm border drawing.
                if (m_parent != null && !m_parent.ActiveState)
                {
                    using (Brush brush = new SolidBrush(ColorTable.RibbonBorderInactive))
                    {
                        Rectangle rc = m_parent.ClientRectangle;
                        rc.Offset(-this.Location.X, -this.Location.Y);

                        Padding borders = m_parent.BordersInternal;

                        g.FillRectangle(brush, rc.Left, rc.Top, borders.Left, rc.Height);
                        g.FillRectangle(brush, rc.Right - borders.Right, rc.Top, borders.Right, rc.Height);
                    }
                }
            }
            else
            {
                Rectangle rc = m_parent.ClientRectangle;
                g.FillRectangle(Brushes.Black, rc.Left - this.Left, rc.Top - this.Top, rc.Width, BORDER_TOP);
            }
		}
		/// <summary>
		/// Processes Windows messages.
		/// </summary>
		/// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message"></see> to process.</param>
		protected override void WndProc( ref Message m )
		{
			switch( ( Msg )m.Msg )
			{
				case Msg.WM_NCHITTEST:
					{
						m.Result = (IntPtr)HitTest.HTTRANSPARENT;
						return;
					}
			}
			base.WndProc( ref m );
		}
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnMinimizePanelChanged()
		{
			SuspendLayout();
            if (!m_header.AutoHide)
                this.VisiblePanel = false;

			Control parent = this.MinimizePanel ? (Control)this.RibbonPopup : this;

            int count = this.HeaderInternal.MainItems.Count;
            
            for (int item = 0; item < count; item++)
            {
                ToolStripTabItem tabItem = this.HeaderInternal.MainItems[item] as ToolStripTabItem;
                if (tabItem != null)
                {
                    tabItem.Panel.Parent = parent;
                }
            }

			if (!this.MinimizePanel)
			{
                using (Graphics g = this.CreateGraphics())
                {
                    Rectangle rc = this.DisplayRectangle;
                    if (this.RibbonStyle != RibbonStyle.Office2013)
                    {
                        if (g.DpiX > 120)
                            rc.X += DPI_150_PANEL_MARGIN;
                        else if (g.DpiX > 96)
                            rc.X += DPI_125_PANEL_MARGIN;
                        else
                            rc.X += PANEL_MARGIN;
                    }
                    rc.Y += this.HeaderInternal.GetPreferredSize(Size.Empty).Height;
                    if (this.RibbonStyle != RibbonStyle.Office2013)
                    {
                        if (g.DpiX > 120)
                            rc.Width -= 2 * DPI_150_PANEL_MARGIN;
                        else if (g.DpiX > 96)
                            rc.Width -= 2 * DPI_125_PANEL_MARGIN;
                        else
                            rc.Width -= 2 * PANEL_MARGIN;
                    }
                    rc.Height = m_panelHeight;

				foreach (ToolStripItem item in this.HeaderInternal.MainItems)
				{
					ToolStripTabItem tabItem = item as ToolStripTabItem;
					if (tabItem != null)
					{
						tabItem.Panel.Bounds = rc;
					}
				}

				this.VisiblePanel = m_bShowPanel;
			}
            }
			if (this.BottomToolstripVisible)
			{
				Rectangle rc = this.DisplayRectangle;
				BottomToolstrip bottomToolstrip = this.BottomToolstrip;

				int height = bottomToolstrip.Height + bottomToolstrip.Margin.Vertical;
				int top = rc.Bottom - height + bottomToolstrip.Margin.Top;

				bottomToolstrip.Location = new Point(rc.X + bottomToolstrip.Margin.Left, top);
			}

			m_contextMenuMinimize.Checked = this.MinimizePanel;

			ResumeLayout(false);
		}
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnShowPanelChanged()
		{
			this.VisiblePanel = m_bShowPanel && !this.MinimizePanel;
		}
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnVisiblePanelChanged()
		{
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaddingChanged( EventArgs e )
		{
			base.OnPaddingChanged( e );

			int minimumHeight = GetMinimumSize().Height;
			if( this.Height < minimumHeight || !m_bShowPanel )
			{
				this.Height = minimumHeight;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnAllowCollapseChanged()
		{
			if (!m_bAllowCollapse)
			{
				this.MinimizePanel = false;
			}
			m_contextMenuMinimize.Enabled = m_bAllowCollapse;
		}
		#endregion

        #region Delegates

        /// <summary>
        /// On right click delegate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void OnRightClick(Object sender, ContextMenuEventArgs e);

        #endregion
        internal int RibbonAutoHideHeight = 30;
		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bVisible"></param>
		private void SetVisiblePanelCore(bool bVisible)
		{
			m_bVisiblePanel = bVisible;

			if (m_bVisiblePanel)
			{
				if (!this.MinimizePanel && m_bCollapsedPanel)
				{
                    if (this.HeaderInternal.AutoHide)
                        if (this.RibbonStatus)
                            if (this.RibbonTouchModeEnabled)
                                this.Height = rSize.Height + RibbonTouchHeight;
                            else
                                this.Height = rSize.Height;
                        else
                            this.Height = RibbonAutoHideHeight;
                    else
                        if (this.RibbonTouchModeEnabled)
                        {
                            this.Height = rSize.Height + RibbonTouchHeight;
                        }
                        else
                            this.Height = rSize.Height;
					m_bCollapsedPanel = false;
				}
			}
			else
			{
				if (!m_bCollapsedPanel)
				{
					int minimizedHeight = this.GetMinimumSize().Height;
					
					m_panelHeight = this.Height - minimizedHeight;
                    if (this.HeaderInternal.AutoHide)
                        this.Height = RibbonAutoHideHeight;
                    else
                        this.Height = minimizedHeight ;

					m_bCollapsedPanel = true;
				}
			}

			ShowPanels(bVisible);

			this.HeaderInternal.Invalidate(false);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bShow"></param>
		private void ShowPanels(bool bShow)
		{
			foreach (ToolStripItem item in this.HeaderInternal.MainItems)
			{
				ToolStripTabItem tabtem = item as ToolStripTabItem;
				if (tabtem != null)
				{
					tabtem.ShowPanel = bShow;
				}
			}

			if (this.MinimizePanel)
			{
				ShowRibbonPopup(bShow);
			}
		}
		internal bool PopupVisible = false;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bShow"></param>
		private void ShowRibbonPopup(bool bShow)
		{
            if (!this.RibbonPopup.IsHandleCreated)
			{
				this.RibbonPopup.CreateHandleInternal();
			}

			if( bShow )
			{
				PopupVisible = true;
				RibbonControlAdvHeader header = this.HeaderInternal;
				ToolStripTabItem selectedTab = header.SelectedTab;

                if (selectedTab != null)
                {
                    RibbonPanel panel = selectedTab.Panel;
                    if (m_header != null)
                        panel.RightToLeft = m_header.RightToLeft;
                    Rectangle rcPopupBounds = GetRibbonPopupBounds(header);
                    int border = 0;
                    int PopupAdjustValue = 0;
                    if (this.RibbonStyle != Tools.RibbonStyle.Office2010 && m_parent != null && !m_parent.CompositionEnabled)
                        border = 5;
                    if (m_parent != null && m_parent.IsMaximized())
                        PopupAdjustValue = 2;
                    WindowsAPI.SetWindowPos(this.RibbonPopup.Handle, (IntPtr)SetWindowPosZOrder.HWND_TOPMOST, rcPopupBounds.X + PopupAdjustValue, rcPopupBounds.Y, rcPopupBounds.Width - border, rcPopupBounds.Height, SWP_SHOWPOPUP);
                }
			}
			else
			{
				PopupVisible = false;
				WindowsAPI.SetWindowPos( this.RibbonPopup.Handle, IntPtr.Zero, 0, 0, 0, 0, SWP_HIDEPOPUP );
			}
		}
		/// <summary>
		/// Gets bounds of the RibbonPopup.
		/// </summary>
		/// <param name="header"> A RibbonControlAdvHeader control. </param>
		/// <returns> Rectangle with RibbonPopup coordinates and size. </returns>
		private Rectangle GetRibbonPopupBounds( RibbonControlAdvHeader header )
		{
			Rectangle rcPopupBounds = Rectangle.Empty;
			Rectangle rcHeader = RectangleToScreen( header.Bounds );
			
			Screen screen = Screen.FromRectangle(rcHeader);
			Rectangle rcScreen = screen.WorkingArea;

			// Height.
			rcPopupBounds.Height = m_panelHeight;
			
			// Y-coordinate.
			rcPopupBounds.Y = rcHeader.Bottom + rcPopupBounds.Height > rcScreen.Height ?
					rcHeader.Y + header.QuickPanelHeight - rcPopupBounds.Height : rcHeader.Bottom;

			// X-coordinate and Width.
            if (this.RibbonStyle == RibbonStyle.Office2013)
            {
                rcPopupBounds.X = rcHeader.X;
                rcPopupBounds.Width = rcHeader.Width;
            }
            else
            {
                using (Graphics g = this.CreateGraphics())
                {
                    if (g.DpiX > 120)
                    {
                        rcPopupBounds.X = rcHeader.X + DPI_150_PANEL_MARGIN;
                        rcPopupBounds.Width = rcHeader.Width - 2 * DPI_150_PANEL_MARGIN;
                    }
                    else if (g.DpiX > 96)
                    {
                        rcPopupBounds.X = rcHeader.X + DPI_125_PANEL_MARGIN;
                        rcPopupBounds.Width = rcHeader.Width - 2 * DPI_125_PANEL_MARGIN;
                    }
                    else
                    {
                        rcPopupBounds.X = rcHeader.X + PANEL_MARGIN;
                        rcPopupBounds.Width = rcHeader.Width - 2 * PANEL_MARGIN;
                    }
                }
            }

			return rcPopupBounds;
		}
		/// <summary>
		/// 
		/// </summary>
		void ShowPanelCore(bool bShow)
		{
			this.VisiblePanel = bShow;
		}
		/// <summary>
		/// Redraws panels.
		/// </summary>
		private void UpdatePanels()
		{
			foreach( Control c in this.Controls )
			{
				RibbonPanel panel = c as RibbonPanel;

				if( panel != null && panel.Visible )
				{
					panel.UpdateCaptions();
				}
			}
		}
		/// <summary>
		/// Redraws the parent.
		/// </summary>
		private void UpdateParent()
		{
			AttachParent(this.Form);

			this.UpdateRenderers(false);
			this.UpdateRegion(m_parent);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="form"></param>
		private void AttachParent(RibbonForm form)
		{
			if (m_parent != form)
			{
				DetachParent();

				m_parent = form;

				if (m_parent != null)
				{
					m_parent.StyleChanged += new EventHandler(OnParentStyleChanged);
					m_parent.RegionChanged += new EventHandler(OnParentRegionChanged);
					m_parent.GetMinSize += new GetMinSizeHandler(OnParentGetMinSize);
					m_parent.GetMargins += new GetMarginsHandler(OnParentGetMargins);
					m_parent.Activated += new EventHandler(OnParentStateChanged);
					m_parent.Deactivate += new EventHandler(OnParentStateChanged);
					m_parent.MdiChildActivate += new EventHandler(OnParentMdiChildActivate);
					m_parent.ShowSystemMenu += new EventHandler(OnParentShowSystemMenu);
                    m_parent.Deactivate += new EventHandler(m_parent_Deactivate);
                    m_parent.SizeChanged += new EventHandler(m_parent_SizeChanged);
					this.HeaderInternal.Form = m_parent;
				}
			}
		}

        void m_parent_SizeChanged(object sender, EventArgs e)
        {
            if (this.RibbonStyle == Tools.RibbonStyle.Office2013 && m_parent.WindowState == FormWindowState.Normal && this.HeaderInternal.AutoHide)
            {
                this.HeaderInternal.AutoHide = false;
                RibbonStatus = false;
            }
        }

        void m_parent_Deactivate(object sender, EventArgs e)
        {
            if (RibbonStyle == Tools.RibbonStyle.Office2013)
            {
                if (this.HeaderInternal.popupControlContainer1.IsShowing())
                    this.HeaderInternal.popupControlContainer1.HidePopup();
                if (this.HeaderInternal.TouchModePop.IsShowing())
                    this.HeaderInternal.TouchModePop.HidePopup();
                this.HeaderInternal.dropDownSelected = false;
            }
        }
		/// <summary>
		/// 
		/// </summary>
		private void DetachParent()
		{
			if (m_parent != null)
			{
				m_parent.StyleChanged -= new EventHandler(OnParentStyleChanged);
				m_parent.RegionChanged -= new EventHandler(OnParentRegionChanged);
				m_parent.GetMinSize -= new GetMinSizeHandler(OnParentGetMinSize);
				m_parent.GetMargins -= new GetMarginsHandler(OnParentGetMargins);
				m_parent.Activated -= new EventHandler(OnParentStateChanged);
				m_parent.Deactivate -= new EventHandler(OnParentStateChanged);
				m_parent.MdiChildActivate -= new EventHandler(OnParentMdiChildActivate);
				m_parent.ShowSystemMenu -= new EventHandler(OnParentShowSystemMenu);

				m_parent = null;

				if (m_header != null)
				{
					m_header.Form = null;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void UpdateRegion( Control parent )
		{
			if (parent != null && (this.Dock == DockStyleEx.TopMost ||this.Dock == DockStyleEx.BottomMost))
			{
				Region rgParent = parent.Region;
				if( rgParent != null )
				{
					Region region = new Region( this.Bounds );

					region.Translate( -1, -1 );
					region.Intersect( rgParent );
					region.Translate( 2, 0 );
					region.Intersect( rgParent );
					region.Translate( 0, 2 );
					region.Intersect( rgParent );
					region.Translate( -2, 0 );
					region.Intersect( rgParent );

					region.Translate( -this.Left + 1, -this.Top - 1 );

					this.Region = region;
				}
				else this.Region = null;

				Invalidate();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private Size GetMinimumSize()
		{
			Size size = this.HeaderInternal.GetMinimumSize();

			size.Height += BORDER_TOP + BORDER_BOTTOM;
			size.Height += this.Padding.Vertical;
			if( this.ShowQuickPanelBelowRibbon )
			{
				size.Height += this.BottomToolstrip.Height + this.BottomToolstrip.Margin.Vertical;
			}

			return size;
		}
		/// <summary>
		/// Checks whethet toolstripitem of given type can be reflected in quick items panel.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private bool CanBeInQuickPanel( ToolStripItem item )
		{
			bool bResult = false;

			if( item != null )
			{
				bResult = 
					item is ToolStripButton || 
					item is ToolStripDropDownButton || 
					item is ToolStripSplitButton ||
					item is ToolStripTextBox || 
					item is ToolStripComboBox || 
					item is ToolStripComboBoxEx ||
                    item is ToolStripSplitButtonEx;
			}

			return bResult;
		}
		/// <summary>
		/// gets toolstrip item at specified position at toolstrip. Remembers about panel items.
		/// </summary>
		/// <param name="ts">Toolstrip to get item from.</param>
		/// <param name="p">Point to find item at in screen coordinates.</param>
		/// <returns>Found item or null if nothing was found.</returns>
		private ToolStripItem GetItemAtToolstrip( ToolStrip ts, Point p )
		{
			ToolStripItem item = null;

			if (ts != null)
			{
				Point locPoint = ts.PointToClient(p);
				item = ts.GetItemAt(locPoint);

				if (item is ToolStripPanelItem)
				{
					item = GetItemAtToolstrip(((ToolStripPanelItem)item).ToolStrip, p);
				}
			}
			return item;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="point"></param>
		/// <returns></returns>
        private void UpdateContextMenu(IntPtr hWnd, POINT point)
        {
            Control ctrl = Control.FromHandle(hWnd);
            if (ctrl == null)
                return;
            Point pointInsideGalleryDropDown = point;
            ToolStripGalleryDropDown toolStripGalleryDropDown = null;
            bool flag = false;
            bool isToolStripGalleryDropDown = (ctrl.GetType() == typeof(ToolStripGalleryDropDown));
            if (isToolStripGalleryDropDown)
            {
                toolStripGalleryDropDown = (ToolStripGalleryDropDown)ctrl;
                ToolStrip toolstrip = GetChildAtPoint(PointToClient(toolStripGalleryDropDown.Bounds.Location), GetChildAtPointSkip.Invisible) as ToolStrip;
                if(toolstrip!=null)
                flag = (toolstrip.GetType() == typeof(RibbonControlAdvHeader));
            }
            if (IsChild(this, hWnd) || IsChild(this.RibbonPopup, hWnd) || (isToolStripGalleryDropDown && !flag))
            {
                flag = false;
                m_contextMenuSep.Owner = null;
                m_contextMenuSep2.Owner = null;
                m_contextMenuSep3.Owner = null;
                m_contextMenuAddToQuick.Owner = null;
                m_contextMenuRemoveFromQuick.Owner = null;
                m_contextMenuCustomize.Owner = null;
                m_contextMenuShowBelow.Owner = null;
                m_contextMenuMinimize.Owner = null;
                if (isToolStripGalleryDropDown)
                {
                    if (toolStripGalleryDropDown != null)
                        point = toolStripGalleryDropDown.Bounds.Location;
                }
                if (UpdateContextMenuItems(point))
                {
                    int count = m_contextMenuItems.Count;
                    int max = m_contextMenuItems.Count;

                    ContextMenuEventArgs args = new ContextMenuEventArgs(m_contextMenuItems);
                    OnBeforeContextMenuOpen(this, args);
                    max = args.ContextMenuItems.Count;

                    if (!args.Cancel)
                    {
                        Control c = Control.FromHandle(hWnd);
                        List<ToolStripItem> stripItems = new List<ToolStripItem>();
                        if (c != null)
                        {
                            if (c.ContextMenuStrip == null)
                            {
                                c.ContextMenuStrip = m_contextMenuStrip;
                            }
                            if (isToolStripGalleryDropDown)
                            {
                                ToolStripItem item = toolStripGalleryDropDown.GetItemAt(toolStripGalleryDropDown.PointToClient(pointInsideGalleryDropDown));
                                if(c.ContextMenuStrip!=null)
                                c.ContextMenuStrip.OwnerItem = item;
                            }
                            if (max > count)
                            {
                                for (; count < max; )
                                {
                                    stripItems.Add(args.ContextMenuItems[count]);
                                    args.ContextMenuItems.RemoveAt(count);
                                    max = args.ContextMenuItems.Count;
                                }
                            }

                            ToolStripItemCollection items = c.ContextMenuStrip.Items;

                            if (stripItems.Count > 0)
                            {
                                items.AddRange(stripItems.ToArray());
                                items.Add(m_contextMenuSep);
                            }

                            items.AddRange(m_contextMenuItems.ToArray());
                        }
                    }
                }
            }
        }

        /// <summary>
		/// Updates context menu items.
		/// </summary>
		/// <param name="p">Point in screen coords.</param>
		/// <returns>True if additional menu items should be added; otherwise false.</returns>
		private bool UpdateContextMenuItems( Point p )
		{
			bool result = false;
			m_contextMenuItems.Clear();

			Point locPoint = PointToClient( p );
			RibbonPanel panel = GetChildAtPoint( locPoint, GetChildAtPointSkip.Invisible ) as RibbonPanel;

			if (panel == null && this.RibbonPopup.IsHandleCreated)
			{
				panel = this.RibbonPopup.GetChildAtPoint( this.RibbonPopup.PointToClient( p ), GetChildAtPointSkip.Invisible ) as RibbonPanel;
			}

			if( panel != null )
			{
				Point pnlPoint = panel.PointToClient( p );

				ToolStripEx ts = panel.GetChildAtPoint( pnlPoint, GetChildAtPointSkip.Invisible ) as ToolStripEx;

				if( ts != null )
				{
					Component comp = ts;

					ToolStripItem item = GetItemAtToolstrip( ts, p );

					if( CanBeInQuickPanel( item ) )
					{
					  comp = item;
					}

					bool bEnabled = true;

					foreach (ToolStripItem quickItem in this.HeaderInternal.QuickItems)
					{
						IQuickItem refl = quickItem as IQuickItem;

						if( refl != null && refl.ReflectedComponent == comp )
						{
							bEnabled = false;
							break;
						}
					}

					m_contextMenuAddToQuick.Text = this.SystemText.QuickAccessAddItemText;

					m_contextMenuAddToQuick.Tag = comp;
					m_contextMenuAddToQuick.Enabled = bEnabled;

					m_contextMenuItems.Add( m_contextMenuAddToQuick );

					m_contextMenuStrip.Renderer = ts.Renderer;

					result = true;
                }
                else if ((panel != null) && (ribbonStyle == RibbonStyle.Office2010 || ribbonStyle == RibbonStyle.Office2013) && (ts == null))
                {
                    if (panel.Controls.Count > 0)
                    {
                        if (panel.Controls[0] is ToolStripEx)
                        {
                            m_contextMenuStrip.Renderer = (panel.Controls[0] as ToolStripEx).Renderer;
                        }
                    }
                    else
                    {
                        if (ribbonStyle == RibbonStyle.Office2013)
                        {
                            m_contextMenuStrip.Renderer = new Syncfusion.Windows.Forms.Tools.Office2013ToolStripRenderer();
                            (m_contextMenuStrip.Renderer as Office2013ToolStripRenderer).ToolStipOffice2013ColorScheme = this.Office2013ColorScheme;
                            (m_contextMenuStrip.Renderer as Office2013ToolStripRenderer).MenuColor = ControlPaint.LightLight(this.MenuColor);
                        }
                    }
                    result = true;
                }
			}
			else
			{
				ToolStrip toolstrip = GetChildAtPoint( locPoint, GetChildAtPointSkip.Invisible ) as ToolStrip;
				if( toolstrip != null )
				{
					ToolStripItem item = GetItemAtToolstrip( toolstrip, p );

					if( item is IQuickItem )
					{
						m_contextMenuRemoveFromQuick.Text = this.SystemText.QuickAccessRemoveItemText;

						m_contextMenuRemoveFromQuick.Tag = item;
						m_contextMenuItems.Add( m_contextMenuRemoveFromQuick );

						result = true;
					}
					else if( !( item is RibbonControlAdvHeader.QuickItemsDropDownButton ) && !( item is RibbonControlAdvHeader.SystemButton ) )
					{
						result = true;
					}

					if( result )
					{
						m_contextMenuStrip.Renderer = toolstrip.Renderer;
					}
				}
			}

			if( result )
			{
				if( m_contextMenuItems.Count > 0 )
				{
					m_contextMenuItems.Add( m_contextMenuSep2 );
				}

				m_contextMenuCustomize.Text = this.SystemText.QuickAccessCustomizeMenuText;
				m_contextMenuItems.Add( m_contextMenuCustomize );

				m_contextMenuShowBelow.Text = ( this.ShowQuickPanelBelowRibbon ) ? ( this.SystemText.QuickAccessPlaceAboveText ) : ( this.SystemText.QuickAccessPlaceBelowText );
				m_contextMenuItems.Add( m_contextMenuShowBelow );

				m_contextMenuItems.Add( m_contextMenuSep3 );

				m_contextMenuMinimize.Text = this.SystemText.QuickAccessMinimizeRibbon;
				m_contextMenuItems.Add( m_contextMenuMinimize );
			}

			return result;
		}
		/// <summary>
		/// Gets ColorScheme according to type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static ToolStripEx.ColorScheme GetOfficeColorScheme( RibbonForm.ColorSchemeType type )
		{
			ToolStripEx.ColorScheme color;
			switch( type )
			{
				case RibbonForm.ColorSchemeType.Silver:
					color = ToolStripEx.ColorScheme.Silver;
					break;
				case RibbonForm.ColorSchemeType.Black:
					color = ToolStripEx.ColorScheme.Black;
					break;
				case RibbonForm.ColorSchemeType.Blue:
					color = ToolStripEx.ColorScheme.Blue;
					break;
				default:
					color = ToolStripEx.ColorScheme.Managed;
					break;
			}

			return color;
		}
        int defalutWidth = 200;
        int defaultHeight = 60;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ControlItem"></param>
        /// <param name="ItemHeaderText"></param>
        /// <param name="ItemMainText"></param>
        /// <param name="ItemHeaderFont"></param>
        /// <param name="ItemMainFont"></param>
        public void SetRibbon2013OptionValues(ControlItems ControlItem, string ItemHeaderText, string ItemMainText, Font ItemHeaderFont, Font ItemMainFont)
        {
            this.HeaderInternal.SetRibbon2013OptionValues(ControlItem, ItemHeaderText, ItemMainText, ItemHeaderFont, ItemMainFont);
            if ((TextRenderer.MeasureText(ItemHeaderText, ItemHeaderFont).Height + 2 * TextRenderer.MeasureText(ItemMainText, ItemMainFont).Height) + 10 > defaultHeight)
            {
                defaultHeight = (TextRenderer.MeasureText(ItemHeaderText, ItemHeaderFont).Height + 2 * TextRenderer.MeasureText(ItemMainText, ItemMainFont).Height) + 10;
                this.HeaderInternal.popupControlContainer1.Height = 3*defaultHeight+3;
                this.HeaderInternal.ribbonDropDownContainer1.Height =3* defaultHeight;
                this.HeaderInternal.controlItem1.Height = defaultHeight;
                this.HeaderInternal.controlItem2.Height = defaultHeight;
                this.HeaderInternal.controlItem3.Height = defaultHeight; 
            }
            if (TextRenderer.MeasureText(ItemHeaderText, ItemHeaderFont).Width > defalutWidth)
            {
                defalutWidth = TextRenderer.MeasureText(ItemHeaderText, ItemHeaderFont).Width +72;
                this.HeaderInternal.popupControlContainer1.Width = defalutWidth + 3;
                this.HeaderInternal.ribbonDropDownContainer1.Width = defalutWidth;
                this.HeaderInternal.controlItem1.Width = defalutWidth -1;
                this.HeaderInternal.controlItem2.Width = defalutWidth -1;
                this.HeaderInternal.controlItem3.Width = defalutWidth - 1;
            }
            if (TextRenderer.MeasureText(ItemMainText, ItemMainFont).Width > (2 * defalutWidth) + 100)
            {
                defalutWidth = TextRenderer.MeasureText(ItemMainText, ItemMainFont).Width ;
                this.HeaderInternal.popupControlContainer1.Width = ((defalutWidth + 100) / 2) + 3; 
                this.HeaderInternal.ribbonDropDownContainer1.Width = ((defalutWidth + 100) / 2); 
                this.HeaderInternal.controlItem1.Width = ((defalutWidth + 100) / 2) -1; 
                this.HeaderInternal.controlItem2.Width = ((defalutWidth + 100) / 2) -1; 
                this.HeaderInternal.controlItem3.Width = ((defalutWidth + 100)/2) -1; 
              
            }
        }
		/// <summary>
		/// Gets ColorSchemeType specific to the ColorScheme.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static RibbonForm.ColorSchemeType GetColorSchemeType(ToolStripEx.ColorScheme colorScheme)
		{
			RibbonForm.ColorSchemeType colorSchemeType;
			switch (colorScheme)
			{
				case ToolStripEx.ColorScheme.Black:
					colorSchemeType = RibbonForm.ColorSchemeType.Black;
					break;
				case ToolStripEx.ColorScheme.Blue:
					colorSchemeType = RibbonForm.ColorSchemeType.Blue;
					break;
				case ToolStripEx.ColorScheme.Silver:
					colorSchemeType = RibbonForm.ColorSchemeType.Silver;
					break;
				default:
					colorSchemeType = RibbonForm.ColorSchemeType.Managed;
					break;
			}

			return colorSchemeType;
		}
        private int ribboncheck=0;
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (ribboncheck == 2)
                rSize = this.Size;
            ribboncheck++;
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="nMsg"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		private IntPtr CallWndProc(IntPtr hWnd, int nMsg, IntPtr wParam, IntPtr lParam)
		{
			switch ((Msg)nMsg)
			{
				case Msg.WM_ACTIVATE:
					OnActivate(hWnd, wParam, lParam);
					break;
				case Msg.WM_MDIACTIVATE:
					OnWmMdiActivate(hWnd, wParam, lParam);
					break;
				case Msg.WM_CONTEXTMENU:
					OnWmContextMenu(hWnd, lParam);
					break;
				case Msg.WM_SYSCOMMAND:
					OnWmSysCommand(wParam);
					break;
			}
			return IntPtr.Zero;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="wParam"></param>
		private void OnActivate(IntPtr hWnd, IntPtr wParam, IntPtr lParam)
		{
			if( this.MinimizePanel )
			{
				Control f = this.TopLevelControl;

				if( f != null && f.Handle == hWnd && wParam == IntPtr.Zero )
				{
					Control popup = this.RibbonPopup;

					if (popup.IsHandleCreated && popup.Handle != lParam)
					{
						this.VisiblePanel = false;
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		private void OnWmMdiActivate(IntPtr hWnd, IntPtr wParam, IntPtr lParam)
		{
			Control parent = this.Parent;
			if (parent != null && parent.IsHandleCreated && WindowsAPI.IsChild(parent.Handle, hWnd))
			{
				if (wParam == IntPtr.Zero || wParam == hWnd)
				{
					this.HeaderInternal.SuspendLayout();

					List<ToolStripTabItem> mergedTabs = new List<ToolStripTabItem>();

					Control activatedForm = GetChildForm(lParam);
					if (activatedForm != null)
					{
						object[] controls = new object[activatedForm.Controls.Count];
						activatedForm.Controls.CopyTo(controls, 0);

						foreach (Control c in controls)
						{
							if (c is RibbonPanelMergeContainer)
							{
								ToolStripTabItem tab = new ToolStripTabItem();

								tab.Text = c.Text;
								tab.Panel = c as RibbonPanelMergeContainer;

								this.Header.AddMainItem(tab);

								mergedTabs.Add(tab);
							}
						}

						if (mergedTabs.Count > 0)
						{
							mergedTabs[0].Checked = true;
						}
					}

					Control deactivatedForm = Control.FromHandle(wParam);
					if (deactivatedForm != null)
					{
						foreach (ToolStripTabItem tab in m_mergedTabs)
						{
							this.HeaderInternal.MainItems.Remove(tab);

							tab.Panel.TabItem = null;
							tab.Panel.Visible = false;
							tab.Panel.Parent = deactivatedForm;

							tab.Dispose();
						}
					}

					m_mergedTabs.Clear();
					m_mergedTabs.AddRange(mergedTabs);

					this.HeaderInternal.ResumeLayout();

					WindowsAPI.PostMessage(this.HeaderInternal.Handle, RibbonControlAdvHeader.WMU_UPDATESYSBUTTONS, 0, 0);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="lParam"></param>
		private void OnWmContextMenu(IntPtr hWnd, IntPtr lParam)
		{
			UpdateContextMenu(hWnd, WindowsAPI.GetPointFromLPARAM(lParam));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="wParam"></param>
		private void OnWmSysCommand(IntPtr wParam)
		{
			switch ((SystemCommand)((int)wParam & 0xFFF0))
			{
				case SystemCommand.SC_MAXIMIZE:
				case SystemCommand.SC_MINIMIZE:
				case SystemCommand.SC_RESTORE:
				case SystemCommand.SC_CLOSE:
				case SystemCommand.SC_NEXTWINDOW:
				case SystemCommand.SC_PREVWINDOW:
				case SystemCommand.SC_CONTEXTHELP:
					WindowsAPI.PostMessage(this.HeaderInternal.Handle, RibbonControlAdvHeader.WMU_UPDATESYSBUTTONS, 0, 0);
					break;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnMouseWheelMsg(ref Message m)
		{
			bool bResult = false;

			if (m_bMouseWheelSupport && this.Form == System.Windows.Forms.Form.ActiveForm )
			{
				int iVirtualKeyDown = WindowsAPI.LOW_ORDER(m.WParam);

				// 0 value indicates that virtual keys aren't down.
				if (iVirtualKeyDown == 0)
				{
					Point pt = this.PointToClient(WindowsAPI.GetPointFromLPARAM(m.LParam));

					if (this.Bounds.Contains(pt))
					{
						// Get WHEEL_DELTA value to know in which side user rotates wheel.
						int iWhellDelta = WindowsAPI.HIGH_ORDER(m.WParam);
						this.HeaderInternal.CheckNextTab(iWhellDelta);

						m.Result = (IntPtr)0;
						bResult = true;
					}
				}
			}

			return bResult;
		}
        Point mousePoint = new Point(0, 0);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		private void OnWmMouseMove(ref Message m)
        {
            if (this.HeaderInternal.AutoHide)
            {
                if (this.Office2013ColorScheme == Tools.Office2013ColorScheme.White)
                    this.HeaderInternal.AutoHideMouseMoveColor = Color.White;
                else if (this.Office2013ColorScheme == Tools.Office2013ColorScheme.LightGray)
                    this.HeaderInternal.AutoHideMouseMoveColor = ColorTranslator.FromHtml("#F0F0F0");
                else
                    this.HeaderInternal.AutoHideMouseMoveColor = ColorTranslator.FromHtml("#F0F0F0");
            }
           mousePoint = new Point(WindowsAPI.LOW_ORDER(m.LParam), WindowsAPI.HIGH_ORDER(m.LParam));
			ToolStripTabItem item = null;
			if (this.ShowPanelOnMouseHove)
			{
				Form f = this.TopLevelControl as Form;

				if (f != null && f.Handle == GetForegroundWindow())
				{
					if (m.HWnd == this.HeaderInternal.Handle)
					{
						Point pt = WindowsAPI.GetPointFromLPARAM(m.LParam);

						item = this.HeaderInternal.GetItemAt(pt) as ToolStripTabItem;

						if (item != null && item != m_lastSelectedTab)
						{
							if (this.MinimizePanel && this.ShowPanel && !this.VisiblePanel)
							{
								if (!item.Checked)
								{
									item.Checked = true;
								}
								this.VisiblePanel = true;
							}

						}
					}
				}
			}
			m_lastSelectedTab = item;
		}
        private void OnWMNCMOUSEMOVE(ref Message m)
        {
            if (this.HeaderInternal.AutoHide)
            {
                if (new Rectangle(0, 0, this.Width - 65, 27).Contains(new Point(WindowsAPI.LOW_ORDER(m.LParam), WindowsAPI.HIGH_ORDER(m.LParam))))
                {
                    this.HeaderInternal.AutoHideMouseMoveColor = ColorTranslator.FromHtml("#e6f2fa");
                    this.Refresh();
                }
                else
                {
                    if (this.Office2013ColorScheme == Tools.Office2013ColorScheme.White)
                        this.HeaderInternal.AutoHideMouseMoveColor = Color.White;
                    else if (this.Office2013ColorScheme == Tools.Office2013ColorScheme.LightGray)
                        this.HeaderInternal.AutoHideMouseMoveColor = ColorTranslator.FromHtml("#F0F0F0");
                    else
                        this.HeaderInternal.AutoHideMouseMoveColor = ColorTranslator.FromHtml("#E0E0E0");
                    this.Refresh();
                }
            }
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		private void OnWmLButtonDown(ref Message m)
        {
            if (this.HeaderInternal != null && this.RibbonStyle == Tools.RibbonStyle.Office2013 && this.HeaderInternal.popupControlContainer1.IsShowing())
                this.HeaderInternal.popupControlContainer1.HidePopup();
            if (this.HeaderInternal.AutoHide && this.RibbonStyle == Tools.RibbonStyle.Office2013)
            {
                ToolStrip ts = Control.FromHandle(m.HWnd) as ToolStrip;
                if (ts == null)
                {
                    RibbonStatus = false;
                    foreach (ToolStripTabItem tab in this.HeaderInternal.MainItems)
                    {
                        tab.Visible = false;
                    }
                    if (this.Parent != null)
                    {
                        this.QuickPanelVisible = false;
                        this.MenuButtonVisible = false;
                       
                        (this.Parent as RibbonForm).ShowIcon = false;
                        (this.Parent as RibbonForm).MinimizeBox = false;
                        (this.Parent as RibbonForm).MaximizeBox = false;
                    }
                }

            }
			if (this.MinimizePanel && this.ShowPanel)
			{
				bool bShow = false;

				if (m.HWnd == this.HeaderInternal.Handle)
				{
					Point pt = WindowsAPI.GetPointFromLPARAM(m.LParam);

					ToolStripTabItem item = this.HeaderInternal.GetItemAt(pt) as ToolStripTabItem;
					if (item != null)
					{
						if (!item.Checked)
						{
							item.Checked = true;
							bShow = true;
						}
						else bShow = !this.VisiblePanel;
					}
				}
				else
				{
					if(IsPanelWnd(this.HeaderInternal.SelectedTab, m.HWnd))
					{
						ToolStrip ts = Control.FromHandle(m.HWnd) as ToolStrip;
						if (ts != null)
						{
							m_pressedItem = ts.GetItemAt(WindowsAPI.GetPointFromLPARAM(m.LParam));
						}
						bShow = true;
					}
					else if( IsPopupWnd( m.HWnd ) )
					{
						return;
					}
				}
				if(this.RibbonStyle==RibbonStyle.Office2007 || this.RibbonStyle == RibbonStyle.Office2010 || this.RibbonStyle == RibbonStyle.Office2013 ||(this.BackStageView!=null && !this.BackStageView.IsVisible)) 
				 this.VisiblePanel = bShow;
			}
		}
        private void OnWMNCLBUTTONUP(ref Message m)
        {

            if (this.HeaderInternal.AutoHide && this.RibbonStyle == Tools.RibbonStyle.Office2013)
            {
                if (new Rectangle(0, 0, this.Width - 65, 27).Contains(new Point(WindowsAPI.LOW_ORDER(m.LParam), WindowsAPI.HIGH_ORDER(m.LParam))))
                {
                    RibbonStatus = true;
                    this.QuickPanelVisible = true;
                    this.MinimizePanel = false;
                    foreach (ToolStripTabItem tab in this.HeaderInternal.VisibleTabItem)
                    {
                        tab.Visible = true;
                    }
                    if (this.Parent != null)
                    {
                        this.MenuButtonVisible = HeaderInternal.HandleMenuButtonVisibility;
                        (this.Parent as RibbonForm).ShowIcon = true;
                        (this.Parent as RibbonForm).MinimizeBox = true;
                        (this.Parent as RibbonForm).MaximizeBox = true;
                        this.NormalState = true;
                    }                  
                }
            }
        }
        internal bool RibbonStatus = false;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		private void OnWmLButtonUp(ref Message m)
		{
			if (this.MinimizePanel && this.VisiblePanel)
			{
				if (IsPanelWnd(this.HeaderInternal.SelectedTab, m.HWnd))
				{
					ToolStrip ts = Control.FromHandle(m.HWnd) as ToolStrip;
					if (ts != null)
					{
						ToolStripItem it = ts.GetItemAt(WindowsAPI.GetPointFromLPARAM(m.LParam));

						if (it == m_pressedItem)
						{
							if (it is ToolStripButton || (it is ToolStripDropDownItem && !((ToolStripDropDownItem)it).DropDown.Visible))
							{
								this.VisiblePanel = false;
							}
						}
					}
				}
                else
                {
                    ToolStrip ts = Control.FromHandle(m.HWnd) as ToolStrip;

                    if (ts != null)
                    {
                        ToolStripItem it = ts.GetItemAt(WindowsAPI.GetPointFromLPARAM(m.LParam));
                        if ((it is ToolStripDropDownItem) && !((ToolStripDropDownItem)it).DropDown.Visible)
                            this.VisiblePanel = false;
                    }
                }
			}
			m_pressedItem = null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		private void OnWmRButtonDown( ref Message m )
		{
			if( this.MinimizePanel && this.ShowPanel )
			{
				if( !IsPanelWnd( this.HeaderInternal.SelectedTab, m.HWnd ) && !IsPopupWnd( m.HWnd ) )
				{
					this.VisiblePanel = false;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		private void OnWmKeyDown(ref Message m)
		{
            if (this.SuperAccelerator != null && this.SuperAccelerator.GetAcceleratorCount() == 0 &&
               this.BackStageView != null && this.BackStageView.IsVisible && (int)m.WParam == (int)VirtualKeys.VK_ESCAPE)
            {
                this.BackStageView.IsVisible = false;
            }
			else if (this.MinimizePanel && (int)m.WParam == (int)VirtualKeys.VK_ESCAPE)
			{
				this.VisiblePanel = false;
			}
		}
		/// <summary>
		/// Forces the RibbonControlAdv to fire an UpdateUI event.
		/// </summary>
		public void PerformUpdateUI()
		{
			OnUpdateUIOnAppIdle( EventArgs.Empty );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="lParam"></param>
		/// <returns></returns>
		private Control GetChildForm(IntPtr lParam)
		{
			Control c = Control.FromHandle(lParam);

            if (c is DockingWrapperForm && c.Controls.Count == 1)
			{
				c = (c.Controls.Count > 0) ? c.Controls[0] : null;
			}

			return c;
		}
		/// <summary>
		/// Check if the hWnd is a child window of the parent control
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="hWnd"></param>
		/// <returns></returns>
		private static bool IsChild(Control parent, IntPtr hWnd)
		{
			if (parent.IsHandleCreated)
			{
				return WindowsAPI.IsChild(parent.Handle, hWnd);
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private Hashtable GetReflectableItems()
		{
			Hashtable result = new Hashtable();

			ArrayList items = GetReflectableItems(this.MenuButtonDropDown.Items);

			foreach (ToolStripTabItem tabItem in this.Header.MainItems)
			{
				RibbonPanel panel = tabItem.Panel;
				if (panel != null)
				{
					foreach (Control c in panel.Controls)
					{
						ToolStripEx ts = c as ToolStripEx;
						if (ts != null)
						{
							string sItem = ts.Name;

							if (!string.IsNullOrEmpty(sItem))
							{
								result[sItem] = ts;
							}

							items.AddRange(GetReflectableItems(ts.GetItems()));
						}
					}
				}
			}

			foreach (ToolStripItem item in items)
			{
				string sItem = item.Name;
				
				if (!string.IsNullOrEmpty(sItem))
				{
					result[sItem] = item;
				}
			}

			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		private ArrayList GetReflectableItems(ICollection items)
		{
			ArrayList result = new ArrayList();

			foreach (ToolStripItem item in items)
			{
				if (item is ToolStripPanelItem)
				{
					result.AddRange(GetReflectableItems(((ToolStripPanelItem)item).ToolStrip.GetItems()));
				}
				else result.Add(item);
			}

			return result;
		}
		#endregion

		#region Internal methods
		/// <summary>
		/// Updates the renderers.
		/// </summary>
		internal void UpdateRenderers(bool office12Mode)
		{
			this.HeaderInternal.UpdateRenderer();

			foreach (ToolStripItem item in this.HeaderInternal.MainItems)
			{
				ToolStripTabItem tabItem = item as ToolStripTabItem;
				if (tabItem != null)
				{
					RibbonPanel panel = tabItem.Panel;
					if (panel != null)
					{
						panel.UpdateRenderers(office12Mode);
					}
				}
			}

			this.BottomToolstrip.UpdateRenderer();
		}
		#endregion

		#region IOffice12Porperties Members

		/// <summary>
		/// Gets or sets style of the launcher.
		/// </summary>
		[Description( "Specifies the style of the launcher button." )]
		public LauncherStyle LauncherStyle
		{
			get
			{
				return m_LauncherStyle;
			}
			set
			{
				if( m_LauncherStyle != value )
				{
					m_LauncherStyle = value;
					UpdatePanels();
				}
			}
		}
        /// <summary>
        /// To ensure that RibbonControlAdv in AutoHide/Normal state
        /// </summary>
        internal bool NormalState = true;

		/// <summary>
		/// Gets or sets the visibility of MinizeButton of the <see cref="RibbonPanel"/>.
		/// </summary>
		private bool showMinimizeButton = false;

		/// <summary>
		/// Gets or sets the visibility of MinizeButton of the <see cref="RibbonPanel"/>.
		/// </summary>
		public bool ShowMinimizeButton
		{
			get 
            {
                if (this.RibbonStyle != Tools.RibbonStyle.Office2013)
                    return showMinimizeButton;
                else
                    return false;
            }
			set { showMinimizeButton = value; }
		}

        /// <summary>
        /// Gets or sets the visibility of RibbonDisplayOptionButton of the <see cref="RibbonControlAdv"/>.
        /// </summary>
        private bool showRibbonDisplayOptionButton = true;

        /// <summary>
        /// Gets or sets the visibility of RibbonDisplayOptionButton of the <see cref="RibbonControlAdv"/>.
        /// </summary>
        public bool ShowRibbonDisplayOptionButton
        {
            get 
            { 
                return showRibbonDisplayOptionButton; 
            }
            set 
            {
                if (value != showRibbonDisplayOptionButton)
                {
                    showRibbonDisplayOptionButton = value;
                    this.HeaderInternal.PerformLayout();
                }
            }
        }

		/// <summary>
		/// Gets or sets whether caption should be shown.
		/// </summary>
		/// <value>true, if Caption should be shown, false otherwise</value>
		[Description( "Specifies whether the caption should be shown." )]
		public virtual bool ShowCaption
		{
			get
			{
				return m_bShowCaption;
			}
			set
			{
				if( m_bShowCaption != value )
				{
					m_bShowCaption = value;
					UpdatePanels();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the launcher buttons are visible.
		/// </summary>
		/// <value>true, if Launcher buttons are visible, false otherwise</value>
		[Description("Gets or sets a value indicating whether the launcher buttons are visible.")]
		public virtual bool ShowLauncher
		{
			get
			{
				return m_bShowLauncher;
			}
			set
			{
				if( m_bShowLauncher != value )
				{
					m_bShowLauncher = value;

					UpdatePanels();
				}
			}
		}

		/// <summary>
		/// Gets or sets the caption should be aligned to top or bottom.
		/// </summary>
		[Description( "Specifies the style (Top, Bottom) of caption" )]
		public virtual CaptionStyle CaptionStyle
		{
			get
			{
				return m_CaptionStyle;
			}
			set
			{
				if( m_CaptionStyle != value )
				{
					m_CaptionStyle = value;

					UpdatePanels();
				}
			}
		}

		/// <summary>
		/// Gets or sets the caption text should be drawn etched, plain or with shadow.
		/// </summary>
		[Description( "Specifies the style of caption text." )]
		public virtual CaptionTextStyle CaptionTextStyle
		{
			get
			{
				return m_CaptionTextStyle;
			}
			set
			{
				if( m_CaptionTextStyle != value )
				{
					m_CaptionTextStyle = value;
					UpdatePanels();
				}
			}
		}

		/// <summary>
		/// Gets or sets the alignment of caption.
		/// </summary>
		[Description( "Specifies the alignment of caption in the control" )]
		public virtual CaptionAlignment CaptionAlignment
		{
			get
			{
				return m_CaptionAlignment;
			}
			set
			{
				if( m_CaptionAlignment != value )
				{
					m_CaptionAlignment = value;
					UpdatePanels();
				}
			}
		}
        /// <summary>
        /// Gets or Sets whether default highlight color should be used 
        /// </summary>
        [DefaultValue(true)]
        public bool UseDefaultHighlightColor
        {
            get
            {
                return useDefaultHighlightColor;
            }
            set
            {
                if (useDefaultHighlightColor != value)
                    useDefaultHighlightColor = value;
                this.HeaderInternal.UseDefaultHighlightColor = value;
                if (this.BackStageView != null && this.BackStageView.BackStage != null)
                    this.BackStageView.BackStage.UseDefaultHighlightColor = value;
                this.UpdateRenderers(false);
                this.UpdateStyles();
            }
        }
        /// <summary>
        /// Indicates whether the current value of the useDefaultHighlightColor property is to be serialized.
        /// </summary>
        /// <returns></returns>
        bool ShouldSerializeUseDefaultHighlightColor()
        {
            return this.UseDefaultHighlightColor != true;
        }
        /// <summary>
        /// Resets the useDefaultHighlightColor.
        /// </summary>
        void ResetUseDefaultHighlightColor()
        {
            this.UseDefaultHighlightColor = true;
        }
		/// <summary>
		/// Gets or sets the caption font.
		/// </summary>
		[Description( "Specifies the caption font." )]
		public virtual Font CaptionFont
		{
			get
			{
				if (this.m_CaptionFont == Control.DefaultFont && this.Parent!=null)
				{
					return this.Parent.Font;
				}
				return m_CaptionFont;
			}
			set
			{
				if( m_CaptionFont != value )
				{
					m_CaptionFont = value;

					UpdatePanels();
				}
			}
		}

		/// <summary>
		/// Gets or sets the minimum height of the caption.
		/// </summary>
		[Description( "Specifies the minimum height of caption." )]
		public virtual int CaptionMinHeight
		{
			get
			{
				return m_nCaptionMinHeight;
			}
			set
			{
				if( m_nCaptionMinHeight != value )
				{
					m_nCaptionMinHeight = value;

					UpdatePanels();
				}
			}
		}

		/// <summary>
		/// Gets or sets the border style for the control.
		/// </summary>
		[Description( "Specifies the borderstyle." )]
		public virtual ToolStripBorderStyle BorderStyle
		{
			get
			{
				return m_BorderStyle;
			}
			set
			{
				if( m_BorderStyle != value )
				{
					m_BorderStyle = value;

					UpdatePanels();
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
                {
                    RibbonStyle = RibbonStyle.Office2007;
                    OfficeColorScheme = ToolStripEx.ColorScheme.Blue;
                }
                else if (value == "Office2007Silver")
                {
                    RibbonStyle = RibbonStyle.Office2007;
                    OfficeColorScheme = ToolStripEx.ColorScheme.Silver;
                }
                else if (value == "Office2007Black")
                {
                    RibbonStyle = RibbonStyle.Office2007;
                    OfficeColorScheme = ToolStripEx.ColorScheme.Black;
                }
                else if (value == "Managed")
                {
                    RibbonStyle = RibbonStyle.Office2007;
                    OfficeColorScheme = ToolStripEx.ColorScheme.Managed;
                }
                else if (value == "Office2010")
                    RibbonStyle = RibbonStyle.Office2010;
            }
        }
		/// <summary>
		/// Gets or sets whether the Office color scheme should be Silver or Blue.
		/// </summary>
		[Description( "Specifies the color scheme (Silver, Blue)." )]
		public virtual ToolStripEx.ColorScheme OfficeColorScheme
		{
			get
			{
				if( m_ColorScheme == ToolStripEx.ColorScheme.Default )
				{
					RibbonForm form = this.Parent as RibbonForm;
					return form != null ? GetOfficeColorScheme( form.ColorScheme ) : ToolStripEx.ColorScheme.Managed;
				}

				return m_ColorScheme;
			}
			set
			{
				if( m_ColorScheme != value )
				{
					m_ColorScheme = value;
					UpdateRenderers(false);

					if (this.colorSchemeChanged != null)
					{
						this.colorSchemeChanged(this,GetColorSchemeType(m_ColorScheme));
					}
					setForeColor();
                    setOffice2007ForeColor();
					Invalidate();
				}
			}
		}
		#endregion

		#region IExtenderProvider Members
		/// <summary>
		/// Gets a value indicating whether a control can be extended.
		/// </summary>
		/// <param name="extendee"></param>
		/// <returns></returns>
		[Description("Gets a value indicating whether a control can be extended.")]
		public bool CanExtend(object extendee)
		{
			ToolStrip ts = extendee as ToolStrip;
			if(ts!=null)
			{
				return this.Contains(ts);
			}

			ToolStripItem item = extendee as ToolStripItem;
			if (item!=null && CanBeInQuickPanel(item))
			{
				ToolStrip owner = item.Owner;
				if (owner != null)
				{
					MenuDropDown.IPanel panel = owner as MenuDropDown.IPanel;
					if (panel != null)
					{
						owner = panel.Owner;
					}
					return this.Contains(owner) || owner==this.MenuButtonDropDown || this.MenuButtonDropDown.Contains(owner);
				}
			}
			return false;
		}
		/// <summary>
		/// Gets or sets text displayed with component in quick panel customizing dialog.
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		[Category("Appearance")]
		[Description("Gets or sets text displayed with component in quick panel customizing dialog.")]
		public string GetDescription(Component component)
		{
			if (m_descriptions.ContainsKey(component))
			{
				return m_descriptions[component];
			}

			return string.Empty;
		}
		/// <summary>
		/// Gets or sets text displayed with component in quick panel customizing dialog.
		/// </summary>
		/// <param name="component"></param>
		/// <param name="value"></param>
		[Category("Appearance")]
		[Description("Gets or sets text displayed with component in quick panel customizing dialog.")]
		public void SetDescription(Component component, string value)
		{
			if (!String.IsNullOrEmpty(value))
			{
				m_descriptions[component] = value;
			}
			else
			{
				m_descriptions.Remove(component);
			}
		}
		/// <summary>
		/// Indicates whether component should be available in quick access menu.
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		[Category("Appearance")]
		[Description("Indicates whether component should be available in quick access menu.")]
		public bool GetUseInQuickAccessMenu(Component component)
		{
			return m_useInQMenuExtProp.Contains(component);
		}
		/// <summary>
		/// Indicates whether component should be available in quick access menu.
		/// </summary>
		/// <param name="component"></param>
		/// <param name="value"></param>
		[Category("Appearance")]
		[Description("Indicates whether component should be available in quick access menu.")]
		public void SetUseInQuickAccessMenu(Component component, bool value)
		{
			if (value)
			{
				if (!m_useInQMenuExtProp.Contains(component))
				{
					m_useInQMenuExtProp.Add(component);
				}
			}
			else
			{
				m_useInQMenuExtProp.Remove(component);
			}
		}
        /// <summary>
        /// Indicates whether component should be available in custom quick access dialog.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        [Category("Appearance")]
        [Description("Indicates whether component should be available in custom quick access dialog.")]
        public bool GetUseInCustomQuickAccessDialog(Component component)
        {
            return !m_useInCustomQDialog.Contains(component);
        }
        /// <summary>
        /// Indicates whether component should be available in Custom quick access dialog.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        [Category("Appearance")]
        [Description("Indicates whether component should be available in custom quick access dialog.")]
        public void SetUseInCustomQuickAccessDialog(Component component, bool value)
        {
            if (!value)
            {
                if (!m_useInCustomQDialog.Contains(component))
                {
                    m_useInCustomQDialog.Add(component);
                }
            }
            else
            {
                if (m_useInCustomQDialog.Contains(component))
                {
                    m_useInCustomQDialog.Remove(component);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        private bool ShouldSerializeUseInCustomQuickAccessDialog(Component component)
        {
            return (component != null && m_useInCustomQDialog.Contains(component));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        void ResetUseInCustomQuickAccessDialog(Component component)
        {
            m_useInCustomQDialog.Add(component);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		private bool ShouldSerializeDescription(Component component)
		{
			return (component != null && m_descriptions.ContainsKey(component));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		void ResetDescription(Component component)
		{
			SetDescription(component, string.Empty);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		private bool ShouldSerializeUseInQuickAccessMenu(Component component)
		{
			return (component != null && m_useInQMenuExtProp.Contains(component));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		void ResetUseInQuickAccessMenu(Component component)
		{
			m_useInQMenuExtProp.Remove(component);
		}
		#endregion

		#region ISupportInitialize Members

		/// <summary>
		/// Performs starting of RibbonControl initialization.
		/// </summary>
		[Description("Performs starting of RibbonControl initialization.")]
		public void BeginInit()
		{
			this.OfficeMenu.SuspendLayout();
		}

		/// <summary>
		/// Performs completing of RibbonControl initialization.
		/// </summary>
		[Description("Performs completing of RibbonControl initialization.")]
		public void EndInit()
		{
			if (this.OfficeMenu != null)
			{
				this.OfficeMenu.ResumeLayout(false);
			}

            RibbonControlAdvHeader.QuickItemsCollection quickItems = this.HeaderInternal.QuickItems as RibbonControlAdvHeader.QuickItemsCollection;

            if (quickItems != null)
            {
                foreach (ToolStripItem item in quickItems)
                {
                    IQuickItem quickItem = item as IQuickItem;
                    if (quickItem != null)
                    {
                        quickItem.Reset();

                        SuperToolTip.SetToolTips(item, quickItem.ReflectedComponent);
                    }
                }

                quickItems.UpdateAccelerators(null);
            }
        }

		#endregion

		#region IMessageFilter Members
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		bool IMessageFilter.PreFilterMessage(ref Message m)
		{
			switch ((Msg)m.Msg)
			{
                case Msg.WM_NCMOUSEMOVE:
                    OnWMNCMOUSEMOVE(ref m);
                    break;
				case Msg.WM_MOUSEMOVE:
					OnWmMouseMove(ref m);
					break;
				case Msg.WM_LBUTTONDOWN:
				case Msg.WM_NCLBUTTONDOWN:
					OnWmLButtonDown(ref m);
					break;
				case Msg.WM_RBUTTONDOWN:
				case Msg.WM_MBUTTONDOWN:
				case Msg.WM_NCRBUTTONDOWN:
				case Msg.WM_NCMBUTTONDOWN:
					OnWmRButtonDown(ref m);
					break;
                case Msg.WM_NCLBUTTONUP:
                    OnWMNCLBUTTONUP(ref m);
                    break;
				case Msg.WM_LBUTTONUP:
					OnWmLButtonUp(ref m);
					break;
				case Msg.WM_KEYDOWN:
					OnWmKeyDown(ref m);
					break;
				case Msg.WM_MOUSEWHEEL:
					return OnMouseWheelMsg(ref m);
				case Msg.WM_SYSCOMMAND:
					OnWmSysCommand(m.WParam);
					break;
			}
			return false;
		}
		/// <summary>
		/// Returns TRUE if hWnd is a tabItem's panel or it's child
		/// </summary>
		/// <param name="hWnd"></param>
		/// <returns></returns>
		bool IsPanelWnd(ToolStripTabItem tabItem, IntPtr hWnd)
		{

			bool bResult = false;
			if (tabItem != null)
			{
				Control panel = tabItem.Panel;
				
				if (panel != null && panel.IsHandleCreated)
				{
					if (hWnd == panel.Handle || WindowsAPI.IsChild(panel.Handle, hWnd))
					{
						bResult = true;
					}
				}
			}
			return bResult;
		}
		/// <summary>
		/// Returns TRUE if hWnd is a handle of Popup window or of it's child
		/// </summary>
		/// <param name="intPtr"></param>
		/// <returns></returns>
		bool IsPopupWnd(IntPtr hWnd)
		{
			bool bResult = false;

			Control cTop = this.TopLevelControl;

			if (cTop != null)
			{
				while (hWnd != IntPtr.Zero && (WindowsAPI.GetWindowLong(hWnd, (int)SetWindowLongOffsets.GWL_STYLE) & (int)WindowStyles.WS_CHILD)!= 0 )
				{
					hWnd = WindowsAPI.GetParent(hWnd);
				}

				bResult = (hWnd != cTop.Handle);
			}
			
			return bResult;
		}
		#endregion

		#region ShouldSerialize & Reset Methods
		/// <summary>
		/// Indicates whether the current value of the LauncherStyle property is to be serialized.
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeLauncherStyle()
		{
			return ( m_LauncherStyle != LauncherStyle.Office2007 );
		}
		/// <summary>
		/// Resets the launcher style.
		/// </summary>
		void ResetLauncherStyle()
		{
			this.LauncherStyle = LauncherStyle.Office2007;
		}
		/// <summary>
		/// Indicates whether the current value of the ShowCaption property is to be serialized.
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeShowCaption()
		{
			return ( m_bShowCaption != true );
		}
		/// <summary>
		/// Resets the show caption.
		/// </summary>
		void ResetShowCaption()
		{
			m_bShowCaption = true;
		}
		/// <summary>
		/// Indicates whether the current value of the ShowLauncher property is to be serialized.
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeShowLauncher()
		{
			return ( m_bShowLauncher != true );
		}
		/// <summary>
		/// Indicates whether the current value of the ShowMinimizeButton property is to be serialized.
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeShowMinimizeButton()
		{
			return (showMinimizeButton != false);
		}
		/// <summary>
		/// Resets the ShowMinimizeButton.
		/// </summary>
		void ResetShowMinimizeButton()
		{
			 showMinimizeButton = false;
		}
		/// <summary>
		/// Resets the show launcher.
		/// </summary>
		void ResetShowLauncher()
		{
			m_bShowLauncher = true;
			UpdatePanels();
		}
		/// <summary>
		/// Indicates whether the current value of the CaptionStyle property is to be serialized.
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeCaptionStyle()
		{
			return ( m_CaptionStyle != CaptionStyle.Bottom );
		}
		/// <summary>
		/// Resets the caption style.
		/// </summary>
		void ResetCaptionStyle()
		{
			this.CaptionStyle = CaptionStyle.Bottom;
		}
		/// <summary>
		/// Indicates whether the current value of the TextStyle property is to be serialized.
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeCaptionTextStyle()
		{
			return ( m_CaptionTextStyle != CaptionTextStyle.Plain );
		}
		/// <summary>
		/// Resets the caption text style.
		/// </summary>
		void ResetCaptionTextStyle()
		{
			this.CaptionTextStyle = CaptionTextStyle.Shadow;
		}
		/// <summary>
		/// Indicates whether the current value of the CaptionAlignment property is to be serialized.
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeCaptionAlignment()
		{
			return ( m_CaptionAlignment != CaptionAlignment.Center );
		}
		/// <summary>
		/// Resets the caption alignment.
		/// </summary>
		void ResetCaptionAlignment()
		{
			this.CaptionAlignment = CaptionAlignment.Center;
		}
		/// <summary>
		/// Indicates whether the current value of the CaptionFont property is to be serialized.
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeCaptionFont()
		{
			return ( m_CaptionFont != Control.DefaultFont );
		}
		/// <summary>
		/// Resets the caption font.
		/// </summary>
		void ResetCaptionFont()
		{
			this.CaptionFont = Control.DefaultFont;
		}
		/// <summary>
		/// Indicates whether the current value of the CaptionMinHeight property is to be serialized.
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeCaptionMinHeight()
		{
			return ( m_nCaptionMinHeight != 0 );
		}
		/// <summary>
		/// Resets the height of the caption min.
		/// </summary>
		void ResetCaptionMinHeight()
		{
			this.CaptionMinHeight = 0;
		}
		/// <summary>
		/// Indicates whether the current value of the BorderStyle property is to be serialized.
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeBorderStyle()
		{
			return ( m_BorderStyle != ToolStripBorderStyle.Etched );
		}
		/// <summary>
		/// Resets the border style.
		/// </summary>
		void ResetBorderStyle()
		{
			this.BorderStyle = ToolStripBorderStyle.Etched;
		}
        /// <summary>
        /// Indicates whether the current value of the CustomRibbonHeaderImage property is to be serialized.
        /// </summary>
        /// <returns></returns>
        bool ShouldSerializeCustomRibbonHeaderImage()
        {
            return this.CustomRibbonHeaderImage != null;
        }
        /// <summary>
        /// Resets the CustomRibbonHeaderImage.
        /// </summary>
        void ResetCustomRibbonHeaderImage()
        {
            this.CustomRibbonHeaderImage = null;
        }
        /// <summary>
        /// Indicates whether the current value of the HideToolTip property is to be serialized.
        /// </summary>
        /// <returns></returns>
        bool ShouldSerializeHideToolTip()
        {
            return this.hideToolTip != false;
        }
        /// <summary>
        /// Indicates whether the current value of the RibbonTouchModeEnabled property is to be serialized.
        /// </summary>
        /// <returns></returns>
        bool ShouldSerializeRibbonTouchModeEnabled()
        {
            return this.ribbonTouchModeEnabled != false;
        }
        /// <summary>
        /// Resets the RibbonTouchModeEnabled.
        /// </summary>
        void ResetRibbonTouchModeEnabled()
        {
            this.ribbonTouchModeEnabled = false;
        }
        /// <summary>
        /// Indicates whether the current value of the QuickDropDownToolTipText property is to be serialized.
        /// </summary>
        /// <returns></returns>
        bool ShouldSerializeQuickDropDownToolTipText()
        {
            return this.QuickDropDownToolTipText != "Customize Quick Access Toolbar";
        }
        /// <summary>
        /// Resets the QuickDropDownToolTipText.
        /// </summary>
        void ResetQuickDropDownToolTipText()
        {
            this.QuickDropDownToolTipText = "Customize Quick Access Toolbar";
        }
        /// <summary>
        /// Resets the HideToolTip.
        /// </summary>
        void ResetHideToolTip()
        {
            this.hideToolTip = false;
        }
        /// <summary>
        /// Indicates whether the current value of the RibbonDisplayOptionToolTip property is to be serialized.
        /// </summary>
        /// <returns></returns>
        bool ShouldSerializeRibbonDisplayOptionToolTip()
        {
            return (RibbonDisplayOptionToolTip != "Ribbon Display Option");
        }
        /// <summary>
        /// Resets the RibbonDisplayOptionToolTip.
        /// </summary>
        void RestRibbonDisplayOptionToolTip()
        {
            RibbonDisplayOptionToolTip = "Ribbon Display Option";
        }
		/// <summary>
		/// Indicates whether the current value of the OfficeColorScheme property is to be serialized.
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeOfficeColorScheme()
		{
			return ( m_ColorScheme != ToolStripEx.ColorScheme.Default );
		}
		/// <summary>
		/// Resets the office color scheme.
		/// </summary>
		void ResetOfficeColorScheme()
		{
			this.OfficeColorScheme = ToolStripEx.ColorScheme.Default;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeTitleFont()
		{
			return this.HeaderInternal.ShouldSerializeTitleFont();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		bool ShouldSerializeRibbonStyle()
		{
			return this.RibbonStyle != RibbonStyle.Office2007;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		void ResetTitleFont()
		{
			this.HeaderInternal.ResetTitleFont();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeMenuButtonDropDown()
		{
			return !MenuButtonDropDown.IsAutoGenerated;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeOfficeMenu()
		{
			return MenuButtonDropDown.IsAutoGenerated;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeMinimumSize()
		{
			Size szHeader = this.HeaderInternal.GetPreferredSize(Size.Empty);
			Size szMinBase = base.MinimumSize;
			
			return szMinBase.Width > 0 || szMinBase.Height > szHeader.Height;
		}
		/// <summary>
		/// 
		/// </summary>
		void ResetMinimumSize()
		{
			base.MinimumSize = Size.Empty;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		bool ShouldSerializeRightToLeft()
		{
			return m_rightToLeft != RightToLeft.Inherit;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		new void ResetRightToLeft()
		{
			this.RightToLeft = RightToLeft.Inherit;
		}
		#endregion

		#region Properties
        /// <summary>
        /// Gets or sets a value indicating whether ToolStripTabItems need to be sorted.
        /// </summary>
        [
        Description("Specifies whether ToolStripTabItems need to be sorted."),
        Category("Appearance"),
        DefaultValue(false)
        ]
        public bool SortTabItems
        {
            get
            {
                return this.m_bSortTabItems;
            }
            set
            {
                if (this.m_bSortTabItems != value)
                {
                    this.m_bSortTabItems = value;
                    if (value == true && this.m_header != null && this.m_header.MainItems.Count > 0)
                    {
                        this.m_header.SortTabs();
                    }
                    else
                    {
                        this.m_header.UnSort();
                    }
                }
            }
        }
		/// <summary>
        /// Specifies whether QuickItemsDropDownButton need to be shown.
        /// </summary>
        [DefaultValue(true), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Specifies whether QuickItemsDropDownButton need to be shown.")]
        public bool ShowQuickItemsDropDownButton
        {
            get
            {
                return this.m_ShowQuickItemsDropDownButton;
            }
            set
            {
                if (this.m_ShowQuickItemsDropDownButton != value)
                {
                    this.m_ShowQuickItemsDropDownButton = value;
                    if(this.HeaderInternal != null)
                        this.HeaderInternal.ShowQuickItemsDropDownButton = value;
                    if (this.BottomToolstrip != null)
                        this.BottomToolstrip.ShowQuickItemsDropDownButton = value;
                    this.Refresh();
                }
            }
        }
		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(false), Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override bool AutoSize
		{
			get
			{
				return base.AutoSize;
			}
			set
			{
				base.AutoSize = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override LayoutEngine LayoutEngine
		{
			get
			{
				if( m_layoutEngine == null )
				{
					m_layoutEngine = new RibbonControlLayout();
				}
				return m_layoutEngine;
			}
		}
		/// <summary>
		/// Gets or sets which control borders are docked to its parent control and determines how a control is resized with its parent.
		/// </summary>
		/// <value></value>
		/// <returns>One of the <see cref="T:System.Windows.Forms.DockStyle"></see> values. The default is <see cref="F:System.Windows.Forms.DockStyle.None"></see>.</returns>
		[DefaultValue( typeof( DockStyleEx ), "TopMost" )]
		[Description( "Gets or sets which control borders are docked to its parent control and determines how a control is resized with its parent." )]
		public new DockStyleEx Dock
		{
			get
			{
				return m_Dock;
			}
			set
			{
				if( m_Dock != value )
				{
					m_Dock = value;

					switch( value )
					{
						case DockStyleEx.Top:
							base.Dock = DockStyle.Top;
							break;
						case DockStyleEx.Bottom:
							base.Dock = DockStyle.Bottom;
							break;
						case DockStyleEx.Left:
							base.Dock = DockStyle.Left;
							break;
						case DockStyleEx.Right:
							base.Dock = DockStyle.Right;
							break;
						case DockStyleEx.Fill:
							base.Dock = DockStyle.Fill;
							break;
						default:
							base.Dock = DockStyle.None;
							break;
					}
                    if (m_parent != null)
                        m_parent.PerformExtendedLayout();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override Rectangle DisplayRectangle
		{
			get
			{
				Rectangle rc = base.DisplayRectangle;

				rc.Y += BORDER_TOP;
				rc.Height -= BORDER_TOP + BORDER_BOTTOM;

				return rc;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override Size MinimumSize
		{
			get
			{
				Size szResult = base.MinimumSize;

				Size szHeader = this.HeaderInternal.GetPreferredSize(Size.Empty);

				if (szResult.Height < szHeader.Height || !m_bVisiblePanel)
				{
					szResult.Height = szHeader.Height;
				}

				return szResult;
			}
			set
			{
				base.MinimumSize = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public override RightToLeft RightToLeft
		{
			get
			{
				if (m_rightToLeft == RightToLeft.Inherit)
				{
					if (this.IsFormManager)
					{
						if (this.Form.RightToLeftLayout)
						{
							return this.Form.RightToLeft;
						}
					}
					else
					{
						Control parent = this.Parent;
						if (parent != null)
						{
							return parent.RightToLeft;
						}
					}
					return RightToLeft.No;
				}
				return m_rightToLeft;
			}
			set
			{
				if (m_rightToLeft != value)
				{
					m_rightToLeft = value;
					OnRightToLeftChanged(EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets header.
		/// </summary>																							
		[Browsable(false),DesignerSerializationVisibility( DesignerSerializationVisibility.Content)]
		public IRibbonHeader Header
		{
			get { return this.HeaderInternal; }
		}

        /// <summary>
        /// Indicates whether user can activate ToolStripItem(Button) on first click.
        /// </summary>
        [DefaultValue(false)]
        public bool ActivateOnFirstClick
        {
            get
            {
                return this.m_bActivateOnFirstClick;
            }
            set
            {
                if (this.m_bActivateOnFirstClick != value)
                {
                    this.m_bActivateOnFirstClick = value;
                }
            }
        }
        /// <summary>
        /// Indicates whether user can scroll TabItems by mouse wheel.
        /// </summary>
        [
        Browsable(false), 
        DefaultValue(true)
        ]
        public bool MouseWheelSupport
        {
            get
            {
                return this.m_bMouseWheelSupport;
            }
            set
            {
                if (this.m_bMouseWheelSupport != value)
                    this.m_bMouseWheelSupport = value;
            }
        }
		/// <summary>
		/// Gets or Sets the image to be displayed in the menu button.
		/// </summary>
		[DefaultValue( null )]
		[Category( "Menu Button" )]
		[Description( "Gets or sets the image to be displayed in the menu button." )]
		public Image MenuButtonImage
		{
			get { return HeaderInternal.MenuButtonImage; }
			set { HeaderInternal.MenuButtonImage = value; }
		}

		private bool canReduceCaptionLength = false;
		/// <summary>
		/// Gets or Sets value that enables resizing of form by truncate its caption text fit into the caption bar, if caption length is greater than form's width.
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool CanReduceCaptionLength
		{
			get { return canReduceCaptionLength; }
			set { canReduceCaptionLength = value; }
		}

        /// <summary>
        /// Gets or Sets the image to be displayed in the menu button.
        /// </summary>
        [DefaultValue(null)]
        [Category("Menu Button")]
        [Description("Gets or sets the text to be displayed in the menu button.")]
        public string MenuButtonText
        {
            get { return HeaderInternal.MenuButtonText; }
            set { HeaderInternal.MenuButtonText = value; }
        }

        /// <summary>
        /// Gets or Sets the font in the menu button.
        /// </summary>
        [DefaultValue(null)]
        [Category("Menu Button")]
        [Description("Gets or sets the font in the menu button.")]
        public Font MenuButtonFont
        {
            get { return HeaderInternal.MenuButtonFont; }
            set { HeaderInternal.MenuButtonFont = value; }
        }
        /// <summary>
        /// Gets or sets a value indicating whether menu button image should be scaled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if menu button image should be scaled; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This property should be set for images whose size is less than Menu Button size.
        /// </remarks>
        [DefaultValue(true), Category("Menu Button"),
        Description("Indicates whether the menu button image should be scaled")]
        public bool ScaleMenuButtonImage
        {
            get { return scaleMenuButtonImage; }
            set 
            {
                if (scaleMenuButtonImage != value)
                    scaleMenuButtonImage = value;
                this.HeaderInternal.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the selected ToolStripTabItem in the RibbonControlAdv.
        /// </summary>
        [Browsable (false),Description("Gets or sets which ToolStripTabItem is selected in RibbonControlAdv.")]
        public ToolStripTabItem SelectedTab
        {
            get { return this.HeaderInternal.SelectedTab; }
            set { this.HeaderInternal.SelectedTab = value; }
        }

		/// <summary>
		/// Gets or sets the ToolStripDropDown to be displayed when menu button is clicked.
		/// </summary>
		[Category("Menu Button"),Description("Gets or sets the ToolStripDropDown to be displayed when menu button is clicked.")]
		[TypeConverter(typeof(Design.MenuDropDownTypeConverter))]
		public ToolStripDropDown MenuButtonDropDown
		{
			get { return HeaderInternal.MenuButtonDropDown; }
			set { HeaderInternal.MenuButtonDropDown = value; }
		}
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
            }
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
            }
        }
		/// <summary>
		/// Gets the MenuDropDown to be displayed when menu button is clicked.
		/// </summary>
		[Category("Menu Button"),Description("Gets the MenuDropDown to be displayed when menu button is clicked.")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public MenuDropDown OfficeMenu
		{
			get { return HeaderInternal.MenuButtonDropDown as MenuDropDown; }
		}
		/// <summary>
		/// 
		/// </summary>
		[Category("Menu Button"), Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ToolStripItemCollection MenuButtonDropDownItems
		{
			get { return HeaderInternal.MenuButton.DropDownItems; }
		}
		/// <summary>
		/// Gets or sets width of menu button.
		/// </summary>
		[Category( "Menu Button" ), Description( "Gets or sets the menu button width." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
		[DefaultValue( DEF_MENU_BUTTON_WIDTH )]
		public int MenuButtonWidth
		{
			get
			{
				return HeaderInternal.MenuButtonWidth;
			}
			set
			{
				HeaderInternal.MenuButtonWidth = value;
			}
		}
		/// <summary>
		/// Gets or sets the BackStageView associated with the MenuButton.
		/// </summary>
		[DefaultValue((object)null), TypeConverter(typeof(ReferenceConverter))]
		public BackStageView BackStageView
		{
			get { return this.HeaderInternal.BackStageView; }
			set { this.HeaderInternal.BackStageView = value; }
		}
		private Color bmenuColor = ColorTranslator.FromHtml("#0072C6");
		/// <summary>
		/// Gets or sets visibility of menu button.
		/// </summary>
		[Category("Menu Button Color")]
		[Description("Gets or sets visibility of menu button color.")]
		public Color  MenuColor
		{
			get
			{
				return bmenuColor;
			}
			set
			{
				bmenuColor = value;
				this.HeaderInternal.MenuColor = value;
				if (this.BackStageView != null && this.BackStageView.BackStage != null)
				this.BackStageView.BackStage.MenuColor = value;
				this.UpdateRenderers(false);
				this.UpdateStyles();
                this.MenuColorChanged += delegate { };
                menuColorChanged(this, MenuColor);
			}
		}
        /// <summary>
        /// 
        /// </summary>
        public Office2013ColorScheme Office2013ColorScheme
        {
            get
            {
                return office2013ColorScheme;
            }
            set
            {
                if (office2013ColorScheme != value)
                {
                    office2013ColorScheme = value;

                    if (value == Office2013ColorScheme.DarkGray)
                    {
                        Color clr = ColorTranslator.FromHtml("#303030");
                        this.MenuColor = clr;
                    }
                    else
                    {
                        Color clr = ColorTranslator.FromHtml("#0072C6");
                        this.MenuColor = clr;
                    }
                }
                if (BottomToolstripVisible)
                {
                    this.BottomToolstripVisible = true;
                }
                this.Refresh();
            }
        }
		/// <summary>
		/// Gets or sets visibility of menu button.
		/// </summary>
		[Category( "Menu Button" )]
		[Description( "Gets or sets visibility of menu button." )]
		[DefaultValue(true), Localizable(true)]
		public bool MenuButtonVisible
		{
			get
			{
				return this.HeaderInternal.MenuButtonVisible;
			}
            set
            {
                this.HeaderInternal.MenuButtonVisible = value;
                if (!value && this.handleMenuButtonVisibile)
                {
                    this.BackStageView.handleMenuButtonVisibility = false;
                    this.handleMenuButtonVisibile = true;
                }
            }
		}
        bool handleMenuButtonVisibile = false;
		/// <summary>
		/// Gets or Sets whether the form title bar should be removed and replaced with
		/// built in RibbonControlAdv system buttons
		/// </summary>
		/// <value>
		/// 	<c>true</c> if form title bar should be visible; otherwise, <c>false</c>.
		/// </value>
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		[Description( "Gets or Sets whether the form title bar should be removed and replaced with built in RibbonControlAdv system buttons" )]
		public bool IsFormManager
		{
			get
			{
				RibbonForm form = this.Form;
				return form != null && form.Appearance == RibbonForm.AppearanceType.Office2007;
			}
			set
			{
				RibbonForm form = this.Form;

				if( form != null )
				{
					form.Appearance = value ? RibbonForm.AppearanceType.Office2007 : RibbonForm.AppearanceType.Normal;
				}
			}
		}
		/// <summary>
		/// Gets or sets panel's minimized status
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Gets or sets whether the panel is minimized.")]
		public bool MinimizePanel
		{
			get
			{
				return m_bMinimizePanel;
			}
			set
			{
				if (m_bMinimizePanel != value && (m_bAllowCollapse || !value))
				{
					SetVisiblePanelCore(false);

					m_bMinimizePanel = value;

					if (this.Parent != null)
					{
						this.Parent.SuspendLayout();
					}

					OnMinimizePanelChanged();

					if (this.Parent != null)
					{
						this.Parent.ResumeLayout(false);
						this.Parent.PerformLayout();
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets whether the panel should be shown for the ToolStripTabItem.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description( "Gets or sets whether the panel can be shown for the ToolStripTabItem." )]
		public bool ShowPanel
		{
			get
			{
				return m_bShowPanel;
			}
			set
			{
				if (m_bShowPanel != value)
				{
					m_bShowPanel = value;
					OnShowPanelChanged();
				}
			}
		}
		/// <summary>
		/// Gets or sets panel's visibility status
		/// </summary>
		internal bool VisiblePanel
		{
			get
			{
				return m_bVisiblePanel;
			}
			set
			{
				if (m_bVisiblePanel != value)
				{
					SetVisiblePanelCore(value);
					OnVisiblePanelChanged();
				}
			}
		}
		/// <summary>
		/// Indicates whether the user can expand or collapse the RibbonPanel.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(true)]
		[Description( "Indicates whether the user can expand or collapse the RibbonPanel." )]
		public bool AllowCollapse
		{
			get
			{
				return m_bAllowCollapse;
			}
			set
			{
				if (value != m_bAllowCollapse)
				{
					m_bAllowCollapse = value;
					OnAllowCollapseChanged();
				}
			}
		}
		/// <summary>
		/// Indicates whether the Minimized RibbonPanel will be shown on mouse hover.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Indicates whether the Minimized RibbonPanel will be shown on mouse hover.")]
		public bool ShowPanelOnMouseHove
		{
			get
			{
				return m_bShowPanelOnMouseHove;
			}
			set
			{
				if(m_bShowPanelOnMouseHove!=value)
				{
					m_bShowPanelOnMouseHove = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the form title alignment in RibbonControlAdv header.
		/// </summary>
		[DefaultValue( typeof( TextAlignment ), "Left" )]
		[Description( "Gets or sets the form title alignment in RibbonControlAdv header." )]
		public TextAlignment TitleAlignment
		{
			get
			{
				return this.HeaderInternal.TitleAlignment;
			}
			set
			{
				this.HeaderInternal.TitleAlignment = value;
			}
		}

        /// <summary>
        /// Specifies the color of the TittleText in the RibbonControlAdv header.
        /// </summary>
        [Description("Specifies the color of the TittleText in the RibbonControlAdv header.")]
        [DefaultValue(typeof(Color),"ActiveCaptionText")]
        public Color TitleColor
        {
            get
            {
                return this.m_titleColor;
            }
            set
            {
                if (this.m_titleColor != value)
                {
                    this.m_titleColor = value;
                    this.HeaderInternal.Invalidate();
                }
            }
        }

		/// <summary>
		/// Gets or sets the font settings of the title.
		/// </summary>
		[Description( "Gets or sets the font settings of the title." )]
		public Font TitleFont
		{
			get
			{
				return this.HeaderInternal.TitleFont;
			}
			set
			{
				this.HeaderInternal.TitleFont = value;
			}
		}
		/// <summary>
		/// Specifies the text for the items in the Quick Access Toolbar Editor.
		/// </summary>
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		[Description( "Specifies the text for the items in the Quick Access Toolbar Editor." )]
		public RibbonSystemText SystemText
		{
			get
			{
				if( m_systemText == null )
				{
					m_systemText = new RibbonSystemText(this);
				}
				return m_systemText;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected Office12ColorTable ColorTable
		{
			get
			{
				return m_ColorTables[ this.OfficeColorScheme ] as Office12ColorTable;
			}
		}
		/// <summary>
		/// Gets the parent form.
		/// </summary>
		internal RibbonForm Form
		{
			get { return this.Parent as RibbonForm; }
		}
		/// <summary>
		/// Gets collection of tab groups.
		/// </summary>
		[Description( "Collection of tab groups." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		public TabGroupCollection TabGroups
		{
			get
			{
				return this.HeaderInternal.Groups;
			}
		}
		/// <summary>
		/// Gets or sets visibility of quick panel.
		/// </summary>
		[Category("Layout"), Description( "Gets or sets visibility of quick panel." )]
		[DefaultValue( true )]
		public bool QuickPanelVisible
		{
			get
			{
				return this.HeaderInternal.QuickPanelVisible;
			}
			set
			{
				if( this.HeaderInternal.QuickPanelVisible != value )
				{
					this.HeaderInternal.QuickPanelVisible = value;
					this.BottomToolstripVisible = ( value && this.ShowQuickPanelBelowRibbon );
				}
			}
		}


        /// <summary>
        /// Gets or sets MinimizeButton ToolTip.
        /// </summary>
        [Description("Gets or sets MinimizeButton ToolTip.")]
        
        public string MinimizeToolTip
        {
            get
            {
                return this.HeaderInternal.MinimizeToolTip;
            }
            set
            {
                if (this.HeaderInternal.MinimizeToolTip != value)
                {
                    this.HeaderInternal.MinimizeToolTip = value;
                    
                }
            }
        }
        /// <summary>
        /// Gets or sets QuickDropDown ToolTip Text.
        /// </summary>
        [Description("Gets or sets QuickDropDown ToolTip Text.")]

        public string QuickDropDownToolTipText
        {
            get
            {
                return this.HeaderInternal.QuickDropDownToolTipText;
            }
            set
            {
                if (this.HeaderInternal.QuickDropDownToolTipText != value)
                {
                    this.HeaderInternal.QuickDropDownToolTipText = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets OverFlowButton ToolTip.
        /// </summary>
        [Description("Gets or sets Quick Item OverFlowButton ToolTip.")]

        public string OverFlowButtonToolTip
        {
            get
            {
                return this.HeaderInternal.OverFlowButtonToolTip;
            }
            set
            {
                if (this.HeaderInternal.OverFlowButtonToolTip != value)
                {
                    this.HeaderInternal.OverFlowButtonToolTip = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets MinimizeButton ToolTip.
        /// </summary>
        [Description("Gets or sets MaximizeButton ToolTip.")]
       
        public string MaximizeToolTip
        {
            get
            {
                return this.HeaderInternal.MaximizeToolTip;
            }
            set
            {
                if (this.HeaderInternal.MaximizeToolTip != value)
                {
                    
                    this.HeaderInternal.MaximizeToolTip = value;

                }
            }
        }

		/// <summary>
		/// Gets or sets value indicating whether quick access toolbar should be shown below ribbon.
		/// </summary>
		[Category( "Layout" ), Description( "Indicates whether quick access toolbar should be shown below ribbon." )]
		[DefaultValue( false )]
		public bool ShowQuickPanelBelowRibbon
		{
			get
			{
				return this.HeaderInternal.ShowQuickPanelBelowRibbon;
			}
			set
			{
				if (this.HeaderInternal.ShowQuickPanelBelowRibbon != value)
				{
					SuspendLayout();
					this.HeaderInternal.ShowQuickPanelBelowRibbon = value;
					this.BottomToolstripVisible = ( value && this.QuickPanelVisible );
                    this.customizeQTACheckBoxAdvChecked = this.HeaderInternal.ShowQuickPanelBelowRibbon;
                    this.BottomToolstrip.ShowItemToolTips = !this.HideToolTip;
					ResumeLayout( true );
				}
			}
		}
        /// <summary>
        /// Gets or sets the value for HideToolTip.
        /// </summary>
        [Description("Gets or sets value for HideToolTip")]
        public bool HideToolTip
        {
            get { return hideToolTip; }
            set
            {
                if (value != hideToolTip)
                {
                    hideToolTip = MenuButtonEnabled ? value : true;
                    this.HeaderInternal.HideMenuButtonToolTip = MenuButtonEnabled ? value : true;
                    this.BottomToolstrip.ShowItemToolTips = value;
                }
            }
        }
        private bool touchMode = false;
        /// <summary>
        /// Gets or sets value for TouchMode.
        /// </summary>
        [Description("Gets or sets value for TouchMode"),DefaultValue(false)]
        public bool TouchMode
        {
            get { return touchMode; }
            set
            {
                if (touchMode != value && this.RibbonStyle == Tools.RibbonStyle.Office2013)
                {
                    touchMode = value;
                    this.HeaderInternal.TouchMode = value;
                    if (value)
                    {
                        if (!this.HeaderInternal.QuickItems.Contains(this.HeaderInternal.SystemMode))
                        {
                            this.HeaderInternal.QuickItems.Add(this.HeaderInternal.SystemMode);
                        }
                    }
                    else
                    {
                        if (this.HeaderInternal.QuickItems.Contains(this.HeaderInternal.SystemMode))
                        {
                            this.HeaderInternal.QuickItems.Remove(this.HeaderInternal.SystemMode);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Indicates whether the TouchMode should be available.
        /// </summary>
        bool ShouldSerializeTouchMode()
        {
            return (touchMode != false);
        }
        /// <summary>
        /// Resets the TouchMode.
        /// </summary>
        void ResetTouchMode()
        {
            this.TouchMode = false;
        }
        /// <summary>
        /// Gets or sets value RibbonDisplayOptionToolTip.
        /// </summary>
        [Description("Gets or sets value for RibbonDisplayOptionToolTip")]
        public string RibbonDisplayOptionToolTip
        {
            get
            {
                return this.HeaderInternal.RibbonDisplayOptionToolTip;
            }
            set
            {
                if (this.HeaderInternal.RibbonDisplayOptionToolTip != value)
                    this.HeaderInternal.RibbonDisplayOptionToolTip = value;
            }
        }
        /// <summary>
        /// Gets or sets value HideMenuButtonToolTip.
        /// </summary>
        [Description("Gets or sets value for HideMenuButtonToolTip")]
        public bool HideMenuButtonToolTip
        {
            get { return hideMenuButtonToolTip; }
            set
            {
                hideMenuButtonToolTip = MenuButtonEnabled ? value : true;
                this.HeaderInternal.HideMenuButtonToolTip = MenuButtonEnabled ? value : true;
            }
        }
        private const int RibbonTouchHeight = 50;
        private bool ribbonTouchModeEnabled = false;
          /// <summary>
        /// Gets or sets value RibbonTouchModeEnabled.
        /// </summary>
        [Description("Gets or sets value for RibbonTouchModeEnabled")]
        public bool RibbonTouchModeEnabled
        {
            get { return ribbonTouchModeEnabled; }
            set
            {
                if (ribbonTouchModeEnabled != value)
                {
                    ribbonTouchModeEnabled = value;
                    this.HeaderInternal.RibbonTouchModeEnabled = value;
                    if (value)
                    {
                        this.HeaderInternal.TouchModeItem.StatusCheck = true;
                        this.HeaderInternal.MouseModeItem.StatusCheck = false;
                        if (!this.MinimizePanel)
                            this.Height += RibbonTouchHeight;
                        RibbonAutoHideHeight = 37;
                    }
                    else
                    {
                        this.HeaderInternal.TouchModeItem.StatusCheck = false;
                        this.HeaderInternal.MouseModeItem.StatusCheck = true;
                        this.Height -= RibbonTouchHeight;
                        RibbonAutoHideHeight = 25;
                    }
                }
            }
        }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private bool show2010CustomizeQuickItemDialog = false;
        /// <summary>
        /// Gets /Set the value for Showing 2010 Quick Items Dailog
        /// </summary>
        public bool Show2010CustomizeQuickItemDialog
        {
            get
            {
                return show2010CustomizeQuickItemDialog;
            }
            set
            {
                show2010CustomizeQuickItemDialog = value;
            }
        }
        public bool MenuButtonEnabled
        {
            get
            {
                return menuButtonEnabled;
            }
            set
            {
                menuButtonEnabled = value;
                this.HeaderInternal.MenuButton.Enabled = value;
            }
        }
		/// <summary>
		/// Indicates whether the <see cref="UpdateUI"/> event should be fired 
		/// in the next application idle event.
		/// </summary>
		[DefaultValue( false ),
		Description( "Specifies whether the UpdateUI event should be fired in the next Application Idle event." ),
		Category( "Behavior" )
		]
		public bool UpdateUIOnAppIdle
		{
			get
			{
				return m_bUpdateUIOnAppIdle;
			}
			set
			{
				if( m_bUpdateUIOnAppIdle != value )
				{
					m_bUpdateUIOnAppIdle = value;

					if( m_bUpdateUIOnAppIdle )
					{
						Application.Idle += new EventHandler( OnApplicationIdle );
					}
					else
					{
						Application.Idle -= new EventHandler( OnApplicationIdle );
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		[DefaultValue(false)]
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool TabStop
		{
			get { return base.TabStop; }
			set { base.TabStop = value; }
		}
		/// <summary>
		/// Occupied control height.
		/// Occupied height is the control height with or without height of RibbonPanel, 
		/// it depends on MinimizePanel property.
		/// </summary>
		[ Description("Returns control's height")]
		[ Obsolete("Use Height property instead")]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int OccupiedHeight
		{
			get { return this.Height; }
		}

        /// <summary>
        /// Gets or sets value indicating whether should show the context menu in Ribbon header.
        /// </summary>
        [Description(" Gets or sets value indicating whether should show the context menu in Ribbon header.")]
        [DefaultValue(true)]
        public bool ShowContextMenu
        {
            get
            {
                return this.HeaderInternal.ShowContextMenu;
            }
            set
            {
                this.HeaderInternal.ShowContextMenu = value;
            }
        }
		/// <summary>
		/// 
		/// </summary>
		[DefaultValue((object)null), TypeConverter(typeof(ReferenceConverter))]
		public SuperAccelerator SuperAccelerator
		{
			get
			{
				return this.HeaderInternal.SuperAccelerator;
			}
			set
			{
				this.HeaderInternal.SuperAccelerator = value;
			}
		}
		#endregion

		#region Internal Properties
		/// <summary>
		/// 
		/// </summary>
		internal RibbonControlAdvHeader HeaderInternal
		{
			get
			{
				if (m_header == null)
				{
					m_header = new RibbonControlAdvHeader(this);

					m_header.ItemAdded += new ToolStripItemEventHandler(OnHeaderItemAdded);
					m_header.ItemRemoved += new ToolStripItemEventHandler(OnHeaderItemRemoved);

					m_header.DoubleClick += new EventHandler(OnHeaderDoubleClick);
					m_header.SizeChanged += new EventHandler(OnHeaderSizeChanged);

					m_header.QuickAccessButton.DropDownOpening += new EventHandler(DropDown_Opening);
					m_header.QuickAccessButton.DropDownOpened += new EventHandler(DropDown_Opened);
					m_header.SelectedTabChanged += new SelectedTabChangedEventHandler(m_header_SelectedTabChanged);

					this.Controls.Add(m_header);
				}
				return m_header;
			}
		}
        private Image quickPanelImage = null;
        /// <summary>
        /// Gets or Sets the image to be displayed in the menu button.
        /// </summary>
        [DefaultValue(null)]
        [Description("Gets or sets the image of the CustomizedQuickAccessDialogBox PictureBox.")]
        public Image QuickPanelImage
        {
            get { return quickPanelImage; }
            set 
            {
                if(value != quickPanelImage)
                    quickPanelImage = value;
            }
        }

        private PictureBoxSizeMode quickPanelImageLayout = PictureBoxSizeMode.StretchImage;
        /// <summary>
        /// Gets or Sets the image Layout of QuickPanelImage.
        /// </summary>
        [DefaultValue(null)]
        [Description("Gets or sets the image layout of the CustomizedQuickAccessDialogBox PictureBox.")]
        public PictureBoxSizeMode QuickPanelImageLayout
        {
            get { return quickPanelImageLayout; }
            set
            {
                quickPanelImageLayout = value;
            }
        }

        /// <summary>
        /// Gets or sets the image of the ribbon.
        /// </summary>
        [Description("Gets or sets the image of the ribbon."), DefaultValue(RibbonHeaderImage.Birds)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RibbonHeaderImage RibbonHeaderImage
        {
            get { return ribbonHeaderImage; }
            set
            {
                if (ribbonHeaderImage != value)
                {
                    ribbonHeaderImage = value;

                    OnRibbonStyleChanged();
                    
                }
            }
        }

        private Image m_customRibbonHeaderImage = null;
        /// <summary>
        /// Gets or Sets the image Layout of QuickPanelImage.
        /// </summary>
        [DefaultValue(null)]
        [Description("Gets or sets the image layout of the CustomizedQuickAccessDialogBox PictureBox.")]
        public Image CustomRibbonHeaderImage
        {
            get { return m_customRibbonHeaderImage; }
            set 
            {
                if (m_customRibbonHeaderImage != value)
                {
                    m_customRibbonHeaderImage = value;
                    OnRibbonStyleChanged();
                }
            }
        }


        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                
            }
        }
		/// <summary>
		/// Gets or sets the style of the ribbon.
		/// </summary>
		[Description("Gets or sets the style of the ribbon."),DefaultValue(RibbonStyle.Office2007)]
		public RibbonStyle RibbonStyle
		{
			get { return ribbonStyle; }
			set 
			{
				if (ribbonStyle != value && !this.HeaderInternal.AutoHide)
				{
					ribbonStyle = value;

					OnRibbonStyleChanged();
                    if (this.ParentForm != null && this.ParentForm is RibbonForm)
                    {
                        (this.ParentForm as RibbonForm).PerformLayout();
                        m_header.UpdateSystemButtons();
                    }
                    if (value == RibbonStyle.Office2010 || value == RibbonStyle.Office2013)
					{
                        if (this.BackStageView != null && this.BackStageView.BackStage != null)
                            this.BackStageView.BackStage.BackStageStyle = value;
						this.TitleAlignment = TextAlignment.Center;
					}

				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void OnRibbonStyleChanged()
		{
			if(this.RibbonStyle == RibbonStyle.Office2007)
				this.MenuButtonWidth = RibbonControlAdv.DEF_MENU_BUTTON_WIDTH;
			else
			{
				if (this.MenuButtonWidth < DEF_2010_MENU_BUTTON_WIDTH)
					this.MenuButtonWidth = RibbonControlAdv.DEF_2010_MENU_BUTTON_WIDTH;
			}

            if (this.RibbonStyle == RibbonStyle.Office2013)
            {
                OfficeColorScheme = ToolStripEx.ColorScheme.Silver;
                CaptionFont = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            }
			this.UpdateRenderers(this.RibbonStyle == RibbonStyle.Office2007);

			if (this.ribbonStyleChanged != null)
				this.ribbonStyleChanged(this, this.RibbonStyle);

			this.PerformLayout();
			setForeColor();
		}

		internal void setForeColor()
		{
			foreach (ToolStripTabItem tab in this.Header.MainItems)
			{
				foreach (Control control in tab.Panel.Controls)
				{
					if (control is ToolStripEx)
					{
						if (m_ColorScheme == ToolStripEx.ColorScheme.Blue)
							(control as ToolStripEx).ForeColor = this.TitleColor = ColorTranslator .FromHtml ("#1E395B");
						else if (m_ColorScheme == ToolStripEx.ColorScheme.Silver)
							(control as ToolStripEx).ForeColor = this.TitleColor = ColorTranslator.FromHtml("#3B3B3B");
						else if (m_ColorScheme == ToolStripEx.ColorScheme.Black)
						{
							(control as ToolStripEx).ForeColor = Color.Black;
							this.TitleColor = Color.White;
						}
					}
				}
			}
		}

        /// <summary>
        /// To set Office2007 colorscheme ForeColor
        /// </summary>
        internal void setOffice2007ForeColor()
        {
            // To update MS Office 2007 color scheme in Visual Basic theme.
            if (this.Parent != null && this.Parent is RibbonForm && this.RibbonStyle == Tools.RibbonStyle.Office2007)
            {
                if (!((this.Parent as RibbonForm).CompositionEnabled))
                {
                    if (this.m_ColorScheme == ToolStripEx.ColorScheme.Black)
                    {
                        this.TitleColor = Color.White;
                    }
                    else
                    {
                        this.TitleColor = Color.Black;
                    }
                }
            }
        }


		/// <summary>
		/// 
		/// </summary>
		internal RibbonControlPopup RibbonPopup
		{
			get
			{
				if (m_popup == null)
				{
					m_popup = new RibbonControlPopup(this);
				}
				return m_popup;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal List<Component> ItemsToShowInQuickMenu
		{
			get
			{
				return m_useInQMenuExtProp;
			}
		}
        /// <summary>
        /// Gets the item to hide in Custom Quick Access dialog
        /// </summary>
        internal List<Component> ItemsToHideInCustomQuickDialog
        {
            get
            {
                return m_useInCustomQDialog;
            }
        }
		/// <summary>
		/// Gets toolstrip that contains quick items when they are places below ribbon.
		/// </summary>
		internal BottomToolstrip BottomToolstrip
		{
			get
			{
				if( m_bottomToolstrip == null )
				{
					m_bottomToolstrip = new BottomToolstrip((RibbonControlAdvHeader.QuickItemsCollection)this.HeaderInternal.QuickItems);
					m_bottomToolstrip.Visible = this.ShowQuickPanelBelowRibbon;
					m_bottomToolstrip.Renderer = new Office12ToolStripRenderer( new OfficeBlack() );

					this.Controls.Add( m_bottomToolstrip );
				}
				return m_bottomToolstrip;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal bool BottomToolstripVisible
		{
			get
			{
				return this.BottomToolstrip.Visible;
			}
			set
			{
				Control bottomToolstrip = this.BottomToolstrip;
                if (this.RibbonStyle == RibbonStyle.Office2013)
                {
                    m_bottomToolstrip.Renderer = new Office2013ToolStripRenderer();
                    (m_bottomToolstrip.Renderer as Office2013ToolStripRenderer).MenuColor = ControlPaint.LightLight(this.MenuColor);
                    (m_bottomToolstrip.Renderer as Office2013ToolStripRenderer).ToolStipOffice2013ColorScheme = this.Office2013ColorScheme;
                }
				if (bottomToolstrip.Visible != value)
				{
					bottomToolstrip.Visible = value;
					if( value )
					{
						this.Height += bottomToolstrip.Height + bottomToolstrip.Margin.Vertical;
					}
					else
					{
						this.Height -= bottomToolstrip.Height + bottomToolstrip.Margin.Vertical;
					}
				}
			}
		}
		#endregion

		#region Events
        /// <summary>
        /// Occurs when right clicked on the Ribbon control and before context menu opens
        /// </summary>
        [Description("Occurs when right clicked on the Ribbon control and before context menu opens.")]
        public event OnRightClick BeforeContextMenuOpen; 
		/// <summary>
		/// Occurs when the user double-clicks the Menu button.
		/// </summary>
		[Description("Occurs when the user double-clicks the Menu button.")]
		public event EventHandler MenuButtonDoubleClick
		{
			add { this.HeaderInternal.MenuButton.DoubleClick += value; }
			remove { this.HeaderInternal.MenuButton.DoubleClick -= value; }
		}
        /// <summary>
        /// Occurs when the user clicks the Menu button.
        /// </summary>
        [Description("Occurs when the user clicks the Menu button.")]
        public event EventHandler MenuButtonClick
        {
            add { this.HeaderInternal.MenuButton.Click += value; }
            remove { this.HeaderInternal.MenuButton.Click -= value; }
        }
		/// <summary>
		/// Occurs when the mouse pointer hovers over the Menu button.
		/// </summary>
		[Description("Occurs when the mouse pointer hovers over the Menu button.")]
		public event EventHandler MenuButtonHover
		{
			add { this.HeaderInternal.MenuButton.MouseHover += value; }
			remove { this.HeaderInternal.MenuButton.MouseHover -= value; }
		}
		/// <summary>
		/// Occurs if either the <see cref="UpdateUIOnAppIdle"/> is on.
		/// </summary>
		[ Description( "Occurs when the RibbonControlAdv.UpdateUIOnAppIdle is set to true." ) ]
		[	Category( "Appearance" ) ]
		public event EventHandler UpdateUI;

		/// <summary>
		/// Occurs before the DropDown of QuickItemsDropDownButton is shown.
		/// </summary>
		[Description("Occurs before the DropDown of QuickItemsDropDownButton is shown.")]
		public event DropDownEventHandler BeforeCustomizeDropDownPopup;
		/// <summary>
		/// Occurs after the DropDown of QuickItemsDropDownButton is shown.
		/// </summary>
		[Description("Occurs after the DropDown of QuickItemsDropDownButton is shown.")]
		public event EventHandler AfterCustomizeDropDownPopup;
		/// <summary>
		/// Occurs when selected(checked) ToolStripTabItem has changed.
		/// </summary>
		[Description("Occurs when selected(checked) ToolStripTabItem has changed.")]
		public event SelectedTabChangedEventHandler SelectedTabItemChanged;

		/// <summary>
		/// Occurs when ColorScheme is changed.
		/// </summary>
		[Description("Occurs when ColorScheme is changed.")]
		internal event ColorSchemeChanged colorSchemeChanged;
		[Description("Occurs when RibbonStyle is changed.")]
		internal event RibbonStyleChanged ribbonStyleChanged;
        [Description("Occurs when Menucolor is changed.")]
        internal event MenuColorChanged menuColorChanged;
		/// <summary>
		/// Hiding Click event
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler Click
		{
			add { base.Click += value; }
			remove { base.Click -= value; }
		}

		/// <summary>
		/// Hiding MouseClick event
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event MouseEventHandler MouseClick
		{
			add { base.MouseClick += value; }
			remove { base.MouseClick -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event MouseEventHandler MouseDoubleClick
		{
			add { base.MouseDoubleClick += value; }
			remove { base.MouseDoubleClick -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event MouseEventHandler MouseDown
		{
			add { base.MouseDown += value; }
			remove { base.MouseDown -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event MouseEventHandler MouseUp
		{
			add { base.MouseUp += value; }
			remove { base.MouseUp -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event MouseEventHandler MouseWheel
		{
			add { base.MouseWheel += value; }
			remove { base.MouseWheel -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler MouseEnter
		{
			add { base.MouseEnter += value; }
			remove { base.MouseEnter -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler MouseHover
		{
			add { base.MouseHover += value; }
			remove { base.MouseHover -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler MouseLeave
		{
			add { base.MouseLeave += value; }
			remove { base.MouseLeave -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event MouseEventHandler MouseMove
		{
			add { base.MouseMove += value; }
			remove { base.MouseMove -= value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler MouseCaptureChanged
		{
			add { base.MouseCaptureChanged += value; }
			remove { base.MouseCaptureChanged -= value; }
		}
		/// <summary>
		/// Hiding KeyDown event
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event KeyEventHandler KeyDown
		{
			add { base.KeyDown += value; }
			remove { base.KeyDown -= value; }
		}
		/// <summary>
		/// Hiding KeyPress event
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event KeyPressEventHandler KeyPress
		{
			add { base.KeyPress += value; }
			remove { base.KeyPress -= value; }
		}
		/// <summary>
		/// Hiding KeyUp event
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event KeyEventHandler KeyUp
		{
			add { base.KeyUp += value; }
			remove { base.KeyUp -= value; }
		}


		private void OnAfterCustomizeDropDownPopup(EventArgs args)
		{
			if (this.AfterCustomizeDropDownPopup != null)
			{
				this.AfterCustomizeDropDownPopup(this, args);
			}
		}
		private void OnBeforeCustomizeDropDownPopup(DropDownEventArgs args)
		{
			if (this.BeforeCustomizeDropDownPopup != null)
			{
				this.BeforeCustomizeDropDownPopup(this, args);
			}
		}
		private void OnSelectedTabItemChanged(SelectedTabChangedEventArgs args)
		{
			if (this.SelectedTabItemChanged != null)
			{
				this.SelectedTabItemChanged(this, args);
			}
		}
		#endregion

		#region Fields
        /// <summary>
        /// Specifies whether QuickItemsDropDownButton need to be shown.
        /// </summary>
        private bool m_ShowQuickItemsDropDownButton = true;
		/// <summary>
		/// 
		/// </summary>
		internal RibbonForm m_parent = null;
		/// <summary>
		/// Header.
		/// </summary>
		private RibbonControlAdvHeader m_header;
		/// <summary>
		/// Specifies the ribbon styled.
		/// </summary>
		private RibbonStyle ribbonStyle = RibbonStyle.Office2007;
        private RibbonHeaderImage ribbonHeaderImage = RibbonHeaderImage.None;
		/// <summary>
		/// 
		/// </summary>
		private RibbonControlPopup m_popup;
		/// <summary>
		/// Style of tool strips launchers.
		/// </summary>
		private LauncherStyle m_LauncherStyle = LauncherStyle.Office2007;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bShowCaption = true;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bShowLauncher = true;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bShowPanelOnMouseHove = false;
		/// <summary>
		/// Indicates panel's minimize status
		/// </summary>
		private bool m_bMinimizePanel = false;
		/// <summary>
		/// Indicates whether the panel can be shown.
		/// </summary>
		private bool m_bShowPanel = true;
		/// <summary>
		/// Indicates panel's visibility status
		/// </summary>
		private bool m_bVisiblePanel = true;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bCollapsedPanel = false;
		/// <summary>
		/// Indicates whether the user can expand or collapse the RibbonPanel.
		/// </summary>
		private bool m_bAllowCollapse = true;
		/// <summary>
		/// Indicates that user can scroll TabItems by mouse wheel.
		/// </summary>
		private bool m_bMouseWheelSupport = true;
		/// <summary>
		/// 
		/// </summary>
		private int m_panelHeight = 0;
        /// <summary>
        /// 
        /// </summary>
        private Color m_titleColor = SystemColors.ActiveCaptionText;
		/// <summary>
		/// 
		/// </summary>
		private CaptionStyle m_CaptionStyle = CaptionStyle.Bottom;
		/// <summary>
		/// 
		/// </summary>
		private CaptionTextStyle m_CaptionTextStyle = CaptionTextStyle.Plain;
		/// <summary>
		/// 
		/// </summary>
		private CaptionAlignment m_CaptionAlignment = CaptionAlignment.Center;
        /// <summary>
        /// Gets whether default highlight color should be used 
        /// </summary>
        private bool useDefaultHighlightColor = true;
		/// <summary>
		/// 
		/// </summary>
		private Font m_CaptionFont = Control.DefaultFont;
		/// <summary>
		/// 
		/// </summary>
		private int m_nCaptionMinHeight = 0;
		/// <summary>
		/// 
		/// </summary>
		private ToolStripEx.ColorScheme m_ColorScheme = ToolStripEx.ColorScheme.Default;
		/// <summary>
		/// 
		/// </summary>
		private ToolStripBorderStyle m_BorderStyle = ToolStripBorderStyle.Etched;
		/// <summary>
		/// 
		/// </summary>
		private RibbonControlLayout m_layoutEngine;
		/// <summary>
		/// 
		/// </summary>
		private DockStyleEx m_Dock = DockStyleEx.TopMost;
		/// <summary>
		/// 
		/// </summary>
		private RibbonSystemText m_systemText;
		/// <summary>
		/// Color Scheme for RibbonStyle2013
		/// </summary>
        private Office2013ColorScheme office2013ColorScheme = Office2013ColorScheme.White;
        /// <summary>
        /// 
        /// </summary>
		static Hashtable m_ColorTables;
		/// <summary>
		/// Container for extended property Description.
		/// </summary>
		private Dictionary<Component, string> m_descriptions;
		/// <summary>
		/// Container for extended property UseInQuickAccessMenu.
		/// </summary>
		private List<Component> m_useInQMenuExtProp;
        /// <summary>
		/// Container for extended property UseInQuickAccessDialog.
		/// </summary>
        private List<Component> m_useInCustomQDialog;
		/// <summary>
		/// 
		/// </summary>
		private CallWndProcHook m_callWndProcHook;

		private ToolStripSeparator m_contextMenuSep;
		private ToolStripSeparator m_contextMenuSep2;
		private ToolStripSeparator m_contextMenuSep3;
		private ToolStripMenuItem m_contextMenuAddToQuick;
		private ToolStripMenuItem m_contextMenuRemoveFromQuick;
		private ToolStripMenuItem m_contextMenuCustomize;
		private ToolStripMenuItem m_contextMenuShowBelow;
		private ToolStripMenuItem m_contextMenuMinimize;
		private ContextMenuStripEx m_contextMenuStrip;
		private List<ToolStripItem> m_contextMenuItems;
		/// <summary>
		/// Toolstrip that contains quick items when they are places below ribbon.
		/// </summary>
		private BottomToolstrip m_bottomToolstrip;
		/// <summary>
		/// Last pressed panel's ToolStripItem
		/// </summary>
		private ToolStripItem m_pressedItem;

		/// <summary>
		/// 
		/// </summary>
		private RightToLeft m_rightToLeft = RightToLeft.Inherit;
		/// <summary>
		/// Indicates whether the <see cref="UpdateUI"/> event should be fired
		/// in the next application idle event.
		/// </summary>
		private bool m_bUpdateUIOnAppIdle = false;
		/// <summary>
		/// 
		/// </summary>
		private ArrayList m_mergedTabs = new ArrayList();
		/// <summary>
		/// 
		/// </summary>
		private ToolStripItem m_lastSelectedTab = null;
        /// <summary>
        /// Specifies whether ToolStripTabItems need to be sorted.
        /// </summary>
        private bool m_bSortTabItems = false;

        private bool scaleMenuButtonImage = true;
        /// <summary>
        /// Indicates whether user can activate ToolStripItem(Button) on first click.
        /// </summary>
        private bool m_bActivateOnFirstClick = false;
        private bool hideMenuButtonToolTip = false;
        /// <summary>
        /// Indicates whether the Default ToolTip visibility status.
        /// </summary>
        private bool hideToolTip = false;
        private bool menuButtonEnabled = true;
        /// <summary>
        /// Indicates whether the customizeQTACheckBoxAdv checked state
        /// </summary>
       internal bool customizeQTACheckBoxAdvChecked = false;
        /// <summary>
        /// Indicates whether the customizeQTACheckBoxAdv checked state save
        /// </summary>
       internal bool customizeQTACheckBoxAdvCheckedSave = false;
		#endregion

		#region Public methods
		/// <summary>
		/// Shows QickItems customizing dialog.
		/// </summary>
		public void ShowCustomizeDialog()
		{
            if (this.RibbonStyle == RibbonStyle.Office2007 || this.RibbonStyle == RibbonStyle.Office2010 || Show2010CustomizeQuickItemDialog)
                CustomizeQuickItemsDialog.Execute(this.HeaderInternal);
            else
                Office2013CustomizeQuickItemsDialog.Execute(this.HeaderInternal);
			this.HeaderInternal.PerformLayout();
		}

		/// <summary>
		/// Saves the current state information to Isolated Storage.
		/// </summary>
		public void SaveState()
		{
			this.SaveState(AppStateSerializer.GetSingleton());
		}
		/// <summary>
		/// Reads the persisted state information from the Isolated Storage.
		/// </summary>
		public void LoadState()
		{
			this.LoadState(AppStateSerializer.GetSingleton());
		}
		/// <summary>
		/// Saves the current state information to the specified <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/>.
		/// </summary>
		/// <param name="serializer"></param>
		public void SaveState(AppStateSerializer serializer)
		{
			RibbonControlAdvStateInfo state = new RibbonControlAdvStateInfo();

			state.MinimizePanel = this.MinimizePanel;
			state.ShowQuickPanelBelowRibbon = this.ShowQuickPanelBelowRibbon;

			foreach (IQuickItem quickitem in this.Header.QuickItems)
			{
				string sItem = null;

				Component item = quickitem.ReflectedComponent;

				if (item is ToolStripItem)
				{
					sItem = ((ToolStripItem)item).Name;
				}
				else if (item is Control)
				{
					sItem = ((Control)item).Name;
				}
				
				if ( !string.IsNullOrEmpty(sItem) )
				{
					state.QuickItems.Add(sItem);
				}
			}

			serializer.SerializeObject(this.Name, state);
		}
		/// <summary>
		/// Reads the persisted state information from the specified <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/>.
		/// </summary>
		/// <param name="serializer"></param>
		public void LoadState(AppStateSerializer serializer)
		{
			RibbonControlAdvStateInfo state = serializer.DeserializeObject(this.Name) as RibbonControlAdvStateInfo;
			if (state != null)
			{
				this.MinimizePanel = state.MinimizePanel;
				this.ShowQuickPanelBelowRibbon = state.ShowQuickPanelBelowRibbon;

				Hashtable reflectableItems = GetReflectableItems();
				if (reflectableItems.Count > 0)
				{
					this.HeaderInternal.SuspendLayout();
					
					this.HeaderInternal.QuickItems.Clear();

					foreach (string s in state.QuickItems)
					{
						if (reflectableItems.ContainsKey(s))
						{
							ToolStripItem quickItem = QuickToolstripReflectable.GetItemToReflect(reflectableItems[s] as Component);

							if (quickItem != null)
							{
								this.HeaderInternal.AddQuickItem(quickItem);
							}
						}
					}

					this.HeaderInternal.ResumeLayout();
				}
			}
		}
		#endregion

		#region WinAPI
		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();
		#endregion

		#region IColorSchemeProvider Members

		public event ColorSchemeChanged ColorSchemeChanged
		{
			add { this.colorSchemeChanged += value; }
			remove { this.colorSchemeChanged -= value; }
		}
        public event MenuColorChanged MenuColorChanged
        {
            add { this.menuColorChanged += value; }
            remove { this.menuColorChanged -= value; }
        }
		public event RibbonStyleChanged RibbonStyleChanged
		{
			add { this.ribbonStyleChanged += value; }
			remove { this.ribbonStyleChanged -= value; }
		}

		#endregion
	}

    #region ContextmenuEventArgs Class

    /// <summary>
    /// Used in BeforeContextmenuOpen event.
    /// </summary>
    public class ContextMenuEventArgs : SyncfusionCancelEventArgs
    {
        private List<ToolStripItem> m_MenuItems = null;

        public ContextMenuEventArgs()
            : base()
        {
        }

        public ContextMenuEventArgs(List<ToolStripItem> menuItems)
            : base()
        {
            m_MenuItems = menuItems;
        }

        /// <summary>
        /// Gets the Context menu items list.
        /// </summary>
        public List<ToolStripItem> ContextMenuItems
        {
            get
            {
                return this.m_MenuItems;
            }
        }
    }

    #endregion

	#region RibbonControlAdvStateInfo
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	internal class RibbonControlAdvStateInfo
	{
		public RibbonControlAdvStateInfo() {}

		public bool MinimizePanel = false;
		public bool ShowQuickPanelBelowRibbon = false;
		public ArrayList QuickItems = new ArrayList();
	}
	#endregion

	#region Enum
	/// <summary>
	/// Specifies the Ribbon Style.
	/// </summary>
	public enum RibbonStyle
	{
		/// <summary>
		/// Specifies the Office 2007 Ribbon appearance.
		/// </summary>
		Office2007 = 0,
		/// <summary>
		/// Specifies the Office 2010 Ribbon appearance.
		/// </summary>
		Office2010,
        /// <summary>
		/// Specifies the Office 2013 Ribbon appearance.
		/// </summary>
		Office2013
	}
    public enum Office2013ColorScheme
    {
        /// <summary>
        /// Specifies the Office 2013 White Color Scheme.
        /// </summary>
        White = 0,
        /// <summary>
        /// Specifies the Office 2013 DarkGray Color Scheme.
        /// </summary>
        DarkGray,
        /// <summary>
        /// Specifies the Office 2013 LightGray Color Scheme.
        /// </summary>
        LightGray
    }
    public enum RibbonHeaderImage
    {
        /// <summary>
        /// Specifies the Office 2007 Ribbon appearance.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies the Office 2007 Ribbon appearance.
        /// </summary>
        Boxes3D,
        /// <summary>
        /// Specifies the Office 2010 Ribbon appearance.
        /// </summary>
        Birds,
        /// <summary>
        /// Specifies the Office 2013 Ribbon appearance.
        /// </summary>
        Bubbles,
        /// <summary>
        /// Specifies the Office 2010 Ribbon appearance.
        /// </summary>
        Butterflies,
        /// <summary>
        /// Specifies the Office 2013 Ribbon appearance.
        /// </summary>
        CircleBands,
        /// <summary>
        /// Specifies the Office 2010 Ribbon appearance.
        /// </summary>
        Circles2,
        /// <summary>
        /// Specifies the Office 2013 Ribbon appearance.
        /// </summary>
        Circles,
        /// <summary>
        /// Specifies the Office 2010 Ribbon appearance.
        /// </summary>
        DottedArrows,
        /// <summary>
        /// Specifies the Office 2013 Ribbon appearance.
        /// </summary>
        Floweral,
        /// <summary>
        /// Specifies the Office 2013 Ribbon appearance.
        /// </summary>
        Lines,
        /// <summary>
        /// Specifies the Office 2010 Ribbon appearance.
        /// </summary>
        Nodes,
        /// <summary>
        /// Specifies the Office 2013 Ribbon appearance.
        /// </summary>
        RoundedSquares,
        /// <summary>
        /// 
        /// </summary>
        Snowflakes,
        /// <summary>
        /// Specifies whether user can set custom image
        /// </summary>
        Custom
    }
	#endregion
}
#endif
