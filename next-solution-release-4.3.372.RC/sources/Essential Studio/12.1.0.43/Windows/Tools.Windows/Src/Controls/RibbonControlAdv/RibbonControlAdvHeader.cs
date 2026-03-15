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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Collections;
using Syncfusion.Windows.Forms.Tools.Win32API;
using System.Reflection;

namespace Syncfusion.Windows.Forms.Tools
{
	#region *** RibbonControlAdvHeader
	/// <summary>
	/// Header of RibbonControlAdv.
	/// </summary>
	[ToolboxItem( false )]
	[DesignerSerializer( typeof( Design.RibbonControlAdvHeaderSerializer ), typeof( CodeDomSerializer ) )]
	internal partial class RibbonControlAdvHeader: ToolStrip, IRibbonHeader, ILayoutSupport, IMessageFilter
	{
		#region Constants
		/// <summary>
		/// Height of quick panel.
		/// </summary>
        const int DEF_PANEL_HEIGHT = 25;
        private const int DPI_125_DEF_PANEL_HEIGHT = 37;
        private const int DPI_150_DEF_PANEL_HEIGHT = 45;
		private const int DEF_QUICK_HEIGHT = 22;
		private const int MIN_TABS_HEIGHT = 22;
        private const int DPI_125_DEF_QUICK_HEIGHT = 29;
        private const int DPI_150_DEF_QUICK_HEIGHT = 34;
        const int DEF_PANEL_HEIGHT_TOUCH = 32;
		const int SEPARATOR_WIDTH = 0;
		const int MENUBUTTON_MARGIN = 2;
        const int MENUBUTTON_MARGIN_125 = 5;
        const int MENUBUTTON_MARGIN_150 = 7;
		const int MENUBUTTON_PADDING = 1;
		const int MENUBUTTON_GAP = 6;
		const int TAB_OFFSET = 4;

		RedrawWindowFlags RDW_UPDATEFRAME =
			RedrawWindowFlags.RDW_INVALIDATE |
			RedrawWindowFlags.RDW_FRAME |
			RedrawWindowFlags.RDW_UPDATENOW;

		const int IMAGE_PADDING = 6;

		internal static Padding smSystemButtons;
		internal static Padding smQuickItems;

		/// <summary> Used for layouting Title. </summary>
		const int MIN_TITLE_TEXT_LENGTH = 24;
		/// <summary> used for layouting quick access items.</summary>
		const int DEF_ICON_SIZE = 16;
		/// <summary> used for layouting quick access items.</summary>
		const int DEF_PADDING = 10;
		/// <summary> Width of scroll button that used for scrolling tabItems. </summary>
		const int SCROLL_BUTTON_WIDTH = 13;
		/// <summary> Interval for timer. </summary>
		const int TIMER_INT = 200;
		/// <summary> Width of separator between Tab items. </summary>
		private const int TAB_ITEMS_SEPARATOR_WIDTH = 3;
		/// <summary> Width of the Separator used in Quick Access Items Office 2010.</summary>
		const int QUICK_ITEM_SEPARATOR_WIDTH = 3;
		/// <summary> Height of the Separator used in Quick Access Items Office 2010.</summary>
        const int DPI_125_QUICK_ITEM_SEPARATOR_WIDTH = 4;
        /// <summary> Height of the Separator used in Quick Access Items Office 2010.</summary>
        const int DPI_150_QUICK_ITEM_SEPARATOR_WIDTH = 4;
        /// <summary> Height of the Separator used in Quick Access Items Office 2010.</summary>
		const int QUICK_ITEM_SEPARATOR_HEIGHT = 15;
        /// <summary> Height of the 125 DPI Separator used in Quick Access Items Office 2010.</summary>
        const int DPI_125_QUICK_ITEM_SEPARATOR_HEIGHT = 18;
        /// <summary> Height of the 150 DPI Separator used in Quick Access Items Office 2010.</summary>
        const int DPI_150_QUICK_ITEM_SEPARATOR_HEIGHT = 24;
		private const int WMSZ_LEFT = 1;
		private const int WMSZ_RIGHT = 2;
		private const int WMSZ_TOP = 3;
		private const int WMSZ_TOPLEFT = 4;
		private const int WMSZ_TOPRIGHT = 5;
		private const int WMSZ_BOTTOM = 6;
		private const int WMSZ_BOTTOMLEFT = 7;
		private const int WMSZ_BOTTOMRIGHT = 8;
        private string overFlowButtonToolTip = "Show DropDown";
        private string quickDropDownToolTipText = "Customize Quick Access Toolbar";
		internal const int WMU_UPDATESYSBUTTONS = (int)Msg.WM_USER + 1;

		static readonly string OVERFLOW_ACCELERATOR = "00";

		#endregion

		#region Enums
		/// <summary>
		/// Different areas of the control.
		/// </summary>
		protected enum ScrollButtonsArea
		{
			/// <summary> Out of scroll buttons. </summary>
			None,
			/// <summary> Right scroll button. </summary>
			RightScrollButton,
			/// <summary> Left scroll button. </summary>
			LeftScrollButton
		}
		#endregion

		#region Delegates
		private delegate int SendMessageDelegate( IntPtr hWnd, Msg msg, int wParam, int lParam );
		#endregion

		#region Events
		/// <summary>
		/// Raised when selected(checked) ToolStipTabItem has changed.
		/// </summary>
		public event SelectedTabChangedEventHandler SelectedTabChanged;
		private void OnSelectedTabChanged( ToolStripTabItem prevSelectedTab, ToolStripTabItem newSelectedTab )
		{
			if( this.SelectedTabChanged != null )
			{
				SelectedTabChangedEventArgs args = new SelectedTabChangedEventArgs( prevSelectedTab, newSelectedTab );
				this.SelectedTabChanged( this, args );
			}
		}
		#endregion

		#region Nested Classes

		#region *** RibbonControlAdvLayoutEngine
		/// <summary>
		/// Layout engine for RibbonControlAdvHeader.
		/// </summary>
		private class RibbonControlAdvLayoutEngine: LayoutEngine
		{
			#region Nested classes
			/// <summary> Tab items list. </summary>
			private class MainItemsList: List<ToolStripItem>
			{
				#region Initialization
				/// <summary>
				/// 
				/// </summary>
				public MainItemsList()
					: base()
				{
				}
				/// <summary>
				/// 
				/// </summary>
				public MainItemsList( RibbonControlAdvHeader header, ObservableList<ToolStripItem> list, Size displaySize, int tabItemsWidth )
					: base()
				{
					int iTabItemsNeededWidth = 0;
					float fCoefficient = 0f;

					// Add available items to list.
					for( int i = 0, count = list.Count; i < count; i++ )
					{
						ToolStripItem item = list[i];

						if( item != null && item.Available && item.Owner == header && header.IsVisibleGroup( item ) )
						{
							if( item.AutoSize )
							{
								item.Size = item.GetPreferredSize( displaySize );
								iTabItemsNeededWidth += item.Width;
							}
							else
							{
								tabItemsWidth -= item.Width;
							}

							tabItemsWidth -= TAB_ITEMS_SEPARATOR_WIDTH;

							this.Add( item );
						}
					}

					// Calculate coefficient.
					if( iTabItemsNeededWidth > 0 )
					{
						fCoefficient = (float)tabItemsWidth / iTabItemsNeededWidth;
						header.m_bDrawSeparators = !( fCoefficient > 1.0f );

						if( fCoefficient > 1.0f )
						{
							fCoefficient = 1.0f;
						}
						else if( fCoefficient < 0.5f )
						{
							fCoefficient = 0.5f;
						}

						float factor = (float)Math.Round( fCoefficient, 1 );
						header.m_fFactor = 0.5f + ( 1.0f - factor );

						// Take into consideration coefficient.
						foreach( ToolStripItem item in this )
						{
							if( item.AutoSize )
							{
								item.Width = (int)( item.Width * fCoefficient );
							}
						}
					}
				}

				#endregion
			}
			/// <summary> MdiButtons list. </summary>
			private class MdiButtonsCollection: List<ToolStripItem>
			{
				#region Initialization
				/// <summary>
				/// 
				/// </summary>
				public MdiButtonsCollection()
					: base()
				{
				}
				/// <summary>
				/// 
				/// </summary>
				public MdiButtonsCollection( ToolStripItemCollection collection, Size displaySize )
					: base()
				{
					for( int i = 0, count = collection.Count; i < count; i++ )
					{
						SystemButton item = collection[i] as SystemButton;

						if( item != null && item.Available )
						{
							item.Size = item.GetPreferredSize( displaySize );

							m_iWidth += item.Width;
							this.Add( item );
						}
					}
				}
				#endregion

				#region Properties
				/// <summary> Gets MdiButtons width. </summary>
				internal int Width
				{
					get
					{
						return m_iWidth;
					}
				}
				#endregion

				#region Fields
				/// <summary> MdiButtons width. </summary>
				private int m_iWidth = 0;
				#endregion
			}
			#endregion

			#region Methods
			/// <summary>
			/// 
			/// </summary>
			/// <param name="proposedSize"></param>
			/// <param name="header"></param>
			/// <param name="bMinWidth"></param>
			/// <returns></returns>
			public Size GetPreferredSize( Size proposedSize, RibbonControlAdvHeader header, bool bMinWidth )
			{
				int totalHeight = 0;
				int totalWidth = 0;

				// Take into consideration MenuButton.
				if( header.m_menuButton.Available )
				{
					if (header.RibbonStyle == RibbonStyle.Office2007)
						totalHeight = header.MenuButtonWidth + 2 * MENUBUTTON_MARGIN;
					else
						totalHeight = header.MenuButton.Height;
                    if (header.RibbonStyle == RibbonStyle.Office2013 && header.AutoHide)
                        totalHeight =header.m_owner.RibbonAutoHideHeight;
                    if (header.RibbonStyle == RibbonStyle.Office2013)
                        totalWidth = totalHeight + 2;
                    else
					    totalWidth = totalHeight + MENUBUTTON_GAP;
				}

				// Size of Quick items on quick panel.
                Size szQuick = header.GetItemsSizePreferred(header.QuickItems, DEF_PANEL_HEIGHT);
                Bitmap bit = new Bitmap(10, 10);
                using (Graphics g = Graphics.FromImage(bit))
                {
                    if (g.DpiX > 120 || header.ribbonTouchModeEnabled)
                    {
                        szQuick = header.GetItemsSizePreferred(header.QuickItems, DPI_150_DEF_PANEL_HEIGHT);
                    }
                    else if (g.DpiX > 96)
                    {
                        szQuick = header.GetItemsSizePreferred(header.QuickItems, DPI_125_DEF_PANEL_HEIGHT);
                    }
                }
                bit.Dispose();
				//Size of one item on quick panel.
				Size szOneQuick = new Size( 0, 0 );

				if( header.QuickItems.Count > 0 )
				{
					szOneQuick = header.QuickItems[0].AutoSize ?
						header.QuickItems[0].GetPreferredSize( Size.Empty ) :
						header.QuickItems[0].Size;
				}

				// Size of HelpButton.
				Size szHelpButton = Size.Empty;
				if( header.m_bDisplayHelpButton )
					szHelpButton = header.HelpButton.Size;

				// Width of HelpButton.
				int iHelpButtonWidth = 0;
				iHelpButtonWidth = szHelpButton.Width;

				// Size of MinimizeButton.
				Size szMinimizeButton = Size.Empty;
				if (header.m_bDisplayMinimizeButton)
					szMinimizeButton = header.MinimizeButton.Size;

                    szMinimizeButton = header.Ribbon2013MinimizeButton.Size;
				// Width of MinimizeButton.
				int iMinimizeButtonWidth = 0;
				iMinimizeButtonWidth = szMinimizeButton.Width;

				// Title size.
				Size szTitle = header.GetTitleSize();
				// Title width.
				int titleWidth = ( szTitle.Width < MIN_TITLE_TEXT_LENGTH ) ? MIN_TITLE_TEXT_LENGTH : szTitle.Width;

				ToolStripItem quickAccessButton = header.QuickAccessButton;
				ToolStripItem quickOverflowButton = header.QuickOverflowButton;

				// Calculate quickOverflow button width.
				int quickAccessButtonWidth = 0;
				int quickOverflowButtonWidth = 0;

				if( quickAccessButton.GetCurrentParent() == header )
					quickAccessButtonWidth = quickAccessButton.Width + quickAccessButton.Margin.Horizontal;
                if(quickOverflowButton!= null )
				if( quickOverflowButton.Available && !header.ShowQuickPanelBelowRibbon )
					quickOverflowButtonWidth = quickOverflowButton.Width + quickOverflowButton.Margin.Horizontal;

				int quickItemsWidth = szOneQuick.Width + szQuick.Height / 2;

				// Calculate header width.
				int headerWidth = quickItemsWidth + quickAccessButtonWidth + quickOverflowButtonWidth;

				int headerHeight = Math.Max( szTitle.Height, szQuick.Height + smQuickItems.Vertical );

				// Increase header width according to SystemButtons width.
				Size szSystem = header.GetItemsSizePreferred( header.SystemButtons, DEF_PANEL_HEIGHT );

				RibbonForm form = header.Form;

				if( form != null && form.CompositionEnabled )
				{
					szSystem.Width = form.DwmButtonBounds.Width;
				}

				int sysHeight = szSystem.Height + smSystemButtons.Vertical;

				if( headerHeight < sysHeight )
				{
					headerHeight = sysHeight;
				}

				// Increase header width according to SystemButtons width.
				Size szMdi = Size.Empty;
				if( header.m_bDisplayMdiButtons )
				{
					szMdi = header.GetItemsSizePreferred( header.MdiButtons, MIN_TABS_HEIGHT );
				}

				int iSystemButtonsLength = szSystem.Width;
				int iMdiHelpButtonsLength = szMdi.Width + iHelpButtonWidth + iMinimizeButtonWidth;

				if( iSystemButtonsLength > iMdiHelpButtonsLength )
					headerWidth += iSystemButtonsLength;
				else
					headerWidth += iMdiHelpButtonsLength;

				// Increase header width according to Title width.
				if(!(header.Parent as RibbonControlAdv).CanReduceCaptionLength)
				headerWidth += titleWidth;

				// Set items width and height.
				int itemsWidth = headerWidth;
				int itemsHeight = headerHeight;

				if( header.MainItems.Count > 0 )
				{
					// Size of Main items
					Size szMain = header.GetItemsSizePreferred( header.MainItems, MIN_TABS_HEIGHT );

					if( headerWidth < szMain.Width && !bMinWidth )
					{
						itemsWidth = headerWidth;
					}
					itemsHeight += SEPARATOR_WIDTH + szMain.Height;
				}
                else if(header.RibbonStyle == RibbonStyle.Office2010 || header.RibbonStyle == RibbonStyle.Office2013)
				{
					int DEF_HEIGHT = 23;
					totalHeight += DEF_HEIGHT;
				}

				// Calculate total size.
				totalWidth += itemsWidth;
                if (header.AutoHide && header.RibbonStyle == RibbonStyle.Office2013)
                {
                    if ((header.Parent as RibbonControlAdv).RibbonStatus)
                        totalHeight = itemsHeight;
                    else
                        totalHeight = 25;
                }
                else if (totalHeight < itemsHeight)
				{
					totalHeight = itemsHeight;
				}

				Size szBorder = header.BorderSize;

				totalWidth += szBorder.Width*2;
				totalHeight += szBorder.Height;

				return new Size( totalWidth, totalHeight );
			}
			#endregion

			#region Overrides
			/// <summary>
			/// Lays out toolstrip items in RibbonControlAdv.
			/// </summary>
			/// <param name="container"></param>
			/// <param name="layoutEventArgs"></param>
			/// <returns></returns>
			public override bool Layout( object container, LayoutEventArgs layoutEventArgs )
			{
				RibbonControlAdvHeader parent = (RibbonControlAdvHeader)container;
				if( parent.IsHandleCreated )
				{
					if( parent.RightToLeft == RightToLeft.No )
					{
						return LayoutLeftToRight( parent, layoutEventArgs );
					}
					else
					{
						return LayoutRightToLeft( parent, layoutEventArgs );
					}
				}
				return false;
			}
			#endregion

			#region Implementation
			/// <summary>
			/// Lays out toolstrip items in RibbonControlAdv from left to right.
			/// </summary>
			/// <param name="parent"></param>
			/// <param name="layoutEventArgs"></param>
			/// <returns></returns>
			private bool LayoutLeftToRight( RibbonControlAdvHeader parent, LayoutEventArgs layoutEventArgs )
            {
                using (Graphics g = Graphics.FromImage(new Bitmap(10, 10)))
                {
                    Rectangle rcDisplay = parent.DisplayRectangle;
                    Size displaySize = rcDisplay.Size;

                    int totalHeight = displaySize.Height;
                    int left = rcDisplay.X;

                    // Set location of MenuButton.
                    ToolStripMenuButton menuButton = parent.m_menuButton;
                    if (menuButton.Available && menuButton.GetCurrentParent() == parent)
                    {
                        int width = parent.MenuButtonWidth;
                        Size textsize = TextRenderer.MeasureText(menuButton.Text, menuButton.Font);
                        if (parent.m_owner.RibbonStyle == RibbonStyle.Office2007)
                        {
                            width = Math.Min(width, totalHeight - 2 * MENUBUTTON_MARGIN);
                            if (g.DpiX > 120)
                            {
                                menuButton.Size = new Size(RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_150, RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_150);
                            }
                            else if (g.DpiX > 96)
                            {
                                menuButton.Size = new Size(RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_125, RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_125);
                            }
                            else
                            {
                                menuButton.Size = new Size(width, width);
                            }
                            parent.SetItemLocation(menuButton, new Point(left + MENUBUTTON_MARGIN, rcDisplay.Y + MENUBUTTON_MARGIN));
                        }
                        else if (parent.m_owner.RibbonStyle == RibbonStyle.Office2013)
                        {
                            if (g.DpiX > 120)
                            {
                                if (RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_125 > textsize.Width && textsize.Height < 35)
                                    menuButton.Size = new Size(RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_150 + 16, 35);
                                else
                                    menuButton.Size = textsize;
                            }
                            else if (g.DpiX > 96 || parent.ribbonTouchModeEnabled)
                            {
                                if (RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_125 + 14 > textsize.Width && textsize.Height < 30)
                                    menuButton.Size = new Size(RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_125 + 14, 30);
                                else
                                    menuButton.Size = textsize;
                            }
                            else
                            {
                                if (width > textsize.Width && textsize.Height < 25)
                                    menuButton.Size = new Size(width, 25);
                                else
                                    menuButton.Size = textsize;
                            }
                            parent.SetItemLocation(menuButton, new Point(left, rcDisplay.Bottom + parent.BorderSize.Height + 1 - menuButton.Height + 1));
                        }
                        else
                        {
                            if (g.DpiX > 120)
                            {
                                if (RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_150 + 16 > textsize.Width && textsize.Height < 35)
                                    menuButton.Size = new Size(RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_150 + 16, 35);
                                else
                                    menuButton.Size = textsize;
                                parent.SetItemLocation(menuButton, new Point(left + 10, rcDisplay.Bottom + parent.BorderSize.Height + 1 - menuButton.Height + 1));
                            }
                            else if (g.DpiX > 96)
                            {
                                if (RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_125 + 14 > textsize.Width && textsize.Height < 30)
                                    menuButton.Size = new Size(RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_125 + 14, 30);
                                else
                                    menuButton.Size = textsize;
                                parent.SetItemLocation(menuButton, new Point(left + 8, rcDisplay.Bottom + parent.BorderSize.Height + 1 - menuButton.Height + 1));
                            }
                            else
                            {
                                if (width > textsize.Width && textsize.Height < 25)
                                    menuButton.Size = new Size(width, 25);
                                else
                                    menuButton.Size = textsize;
                                parent.SetItemLocation(menuButton, new Point(left + 8, rcDisplay.Bottom + parent.BorderSize.Height + 1 - menuButton.Height + 1));
                            }

                        }
                        if (parent.m_owner.RibbonStyle == RibbonStyle.Office2013)
                            left = menuButton.Bounds.Right + 2;
                        else
                            left = menuButton.Bounds.Right + MENUBUTTON_GAP;
                    }

                    // Set location of MenuButton.
                    ImageButton backStageMenuButton = parent.imageButton1;
                    if (backStageMenuButton.Visible)
                    {
                        int width = parent.MenuButtonWidth;
                        backStageMenuButton.Visible = false;
                        Size textsize = TextRenderer.MeasureText(menuButton.Text, menuButton.Font);
                        if (parent.m_owner.RibbonStyle == RibbonStyle.Office2013)
                        {
                            if (g.DpiX > 120)
                            {
                                backStageMenuButton.Size = new Size(70, 55);
                            }
                            else if (g.DpiX > 96)
                            {
                                backStageMenuButton.Size = new Size(65, 50);
                            }
                            else
                            {
                                backStageMenuButton.Size = new Size(60, 45);
                            }
                            object host = new ToolStripControlHost(backStageMenuButton);
                        }
                    }

                    int tabsLeft = rcDisplay.X + TAB_OFFSET;

                    // Correct tabsOffset.
                    if (menuButton.Available && menuButton.Bounds.Right + MENUBUTTON_GAP > tabsLeft)
                    {
                        if (parent.m_owner.RibbonStyle == RibbonStyle.Office2013)
                            tabsLeft = menuButton.Bounds.Right + 2;
                        else
                            tabsLeft = menuButton.Bounds.Right + MENUBUTTON_GAP;
                    }

                    // Set location of System buttons and calculate their width.
                    int iSysButtonsOffset;

                    RibbonForm form = parent.Form;

                    if (form == null || !form.CompositionEnabled || parent.RibbonStyle == RibbonStyle.Office2013)
                    {
                        iSysButtonsOffset = rcDisplay.Right - smSystemButtons.Right;
                        if (form != null && !form.CompositionEnabled && form.WindowState == FormWindowState.Maximized && parent.RibbonStyle == RibbonStyle.Office2013)
                            iSysButtonsOffset -= 2 + parent.BorderSize.Width;
                        if (parent.m_bDisplaySysButtons)
                        {
                            int buttonsTop = rcDisplay.Y + smSystemButtons.Top;

                            buttonsTop += (parent.RibbonStyle == RibbonStyle.Office2010 || parent.RibbonStyle == RibbonStyle.Office2013) ? 6 : 0;

                            for (int i = parent.SystemButtons.Count - 1; i >= 0; i--)
                            {
                                SystemButton item = parent.SystemButtons[i] as SystemButton;

                                if (item != null && item.Available)
                                {
                                    item.Size = item.GetPreferredSize(displaySize);

                                    iSysButtonsOffset -= item.Width;

                                    item.Location = new Point(iSysButtonsOffset, buttonsTop);
                                }
                            }
                        }

                        iSysButtonsOffset -= smSystemButtons.Left;
                    }
                    else if (form != null && form.CompositionEnabled && form.FormBorderStyle == FormBorderStyle.None)
                    {
                        iSysButtonsOffset = parent.DisplayRectangle.Right;
                    }
                    else
                    {
                        iSysButtonsOffset = form.DwmButtonBounds.Left - smSystemButtons.Left;
                    }

                    // Set location of Main Items.
                    int mainItemsHeight = DEF_PANEL_HEIGHT;
                    int iTabItemsRectangleHeight = mainItemsHeight;

                    int iTabItemsRectangleTop = rcDisplay.Bottom - DEF_PANEL_HEIGHT;
                    int iTabItemsRectangleRight = rcDisplay.Right - smSystemButtons.Right;

                    MdiButtonsCollection mdiButtons = new MdiButtonsCollection(parent.MdiButtons, displaySize);

                    if (parent.m_bDisplayMdiButtons)
                    {
                        iTabItemsRectangleRight -= mdiButtons.Width;
                    }

                    if (parent.m_bDisplayHelpButton)
                    {
                        SystemButton item = parent.HelpButton as SystemButton;

                        if (item != null && item.Available)
                        {
                            item.Size = item.GetPreferredSize(displaySize);
                            iTabItemsRectangleRight -= item.Width;
                        }
                    }

                    if (parent.m_bDisplayMinimizeButton)
                    {
                        MinimizeSystemButton item = parent.MinimizeButton as MinimizeSystemButton;

                        if (item != null && item.Available)
                        {
                            item.Size = item.GetPreferredSize(displaySize);
                            iTabItemsRectangleRight -= item.Width;
                        }
                    }

                    MainItemsList visibleMainItems = new MainItemsList(parent, parent.m_mainItems, displaySize, iTabItemsRectangleRight - tabsLeft);

                    parent.m_iSepatators.Clear();

                    for (int i = 0, count = visibleMainItems.Count, x = tabsLeft - parent.ScrollPositionInternal; i < count; i++)
                    {
                        ToolStripItem item = visibleMainItems[i];

                        int offset = (parent.RibbonStyle == RibbonStyle.Office2007) ? 0 : 1 + parent.BorderSize.Height;

                        parent.SetItemLocation(item, new Point(x, rcDisplay.Bottom + offset - item.Height));

                        parent.m_iSepatators.Add(x + item.Width + (TAB_ITEMS_SEPARATOR_WIDTH / 2));

                        if (item.Height + item.Margin.Vertical > mainItemsHeight)
                        {
                            mainItemsHeight = item.Height + item.Margin.Vertical;
                            iTabItemsRectangleHeight = item.Height;
                            iTabItemsRectangleTop = rcDisplay.Bottom - item.Height;
                        }

                        x += (item.Width + TAB_ITEMS_SEPARATOR_WIDTH);
                    }

                    // Set location of Mdi buttons and Help Button.
                    int nMdiTop = rcDisplay.Y + parent.QuickPanelHeight + smSystemButtons.Top;
                    int nMdiLeft = rcDisplay.Right - smSystemButtons.Right;
                    if (parent.m_bDisplayMdiButtons)
                    {
                        for (int i = mdiButtons.Count - 1; i >= 0; i--)
                        {
                            SystemButton item = mdiButtons[i] as SystemButton;

                            if (item != null)
                            {
                                nMdiLeft -= item.Width;
                                item.Location = new Point(nMdiLeft, nMdiTop);
                            }
                        }
                    }
                    if (parent.RibbonStyle == RibbonStyle.Office2013)
                        nMdiLeft -= 15;

                    if (parent.RibbonStyle == RibbonStyle.Office2010 || parent.RibbonStyle == RibbonStyle.Office2013)
                    {
                        nMdiLeft -= 6;
                        nMdiTop += 3;
                    }

                    if (parent.m_bDisplayHelpButton)
                    {
                        SystemButton item = parent.HelpButton as SystemButton;

                        if (item != null && item.Available)
                        {
                            nMdiLeft -= item.Width;

                            item.Size = item.GetPreferredSize(displaySize);


                            if (g.DpiX > 96 || parent.ribbonTouchModeEnabled)
                            {
                                if (parent.RibbonStyle != RibbonStyle.Office2013)
                                {
                                    item.Location = new Point(nMdiLeft, nMdiTop);
                                }
                                else if (parent.RibbonStyle == RibbonStyle.Office2013)
                                {
                                    int xLocation = form.ClientRectangle.Width - item.Width - 5;
                                    foreach (RibbonControlAdvHeader.SystemButton button in parent.SystemButtons)
                                    {
                                        if (button.Visible)
                                            xLocation -= button.Width;
                                    }
                                    if ((parent.Parent as RibbonControlAdv).ShowRibbonDisplayOptionButton && (parent.Parent as RibbonControlAdv).HeaderInternal.Ribbon2013MinimizeButton.Visible)
                                    {
                                        xLocation -= parent.Ribbon2013MinimizeButton.Width;
                                    }
                                    if (form.WindowState == FormWindowState.Maximized)
                                        item.Location = new Point(xLocation - (2 * RibbonForm.DPI_125_BORDER_WIDTH), 7);
                                    else
                                        item.Location = new Point(xLocation - 2, 7);
                                }
                            }
                            else
                            {
                                if (parent.RibbonStyle != RibbonStyle.Office2013)
                                {
                                    item.Location = new Point(nMdiLeft, nMdiTop);
                                }
                                else if (parent.RibbonStyle == RibbonStyle.Office2013)
                                {
                                    int xLocation = form.ClientRectangle.Width - item.Width;
                                    foreach (RibbonControlAdvHeader.SystemButton button in parent.SystemButtons)
                                    {
                                        if (button.Visible)
                                            xLocation -= button.Width;
                                    }
                                    if ((parent.Parent as RibbonControlAdv).ShowRibbonDisplayOptionButton)
                                    {
                                        if ((parent.Parent as RibbonControlAdv).BackStageView == null || ((parent.Parent as RibbonControlAdv).BackStageView != null && !(parent.Parent as RibbonControlAdv).BackStageView.IsVisible))
                                            xLocation -= parent.Ribbon2013MinimizeButton.Width;
                                    }
                                    if (form.WindowState == FormWindowState.Maximized)
                                        item.Location = new Point(xLocation - item.Width + RibbonForm.BORDER_WIDTH, 7);
                                    else
                                        item.Location = new Point(xLocation-2, 7);
                                }
                            }
                        }
                    }

                    if (parent.m_bDisplayMinimizeButton)
                    {
                        MinimizeSystemButton item = parent.MinimizeButton as MinimizeSystemButton;

                        if (item != null && item.Available)
                        {
                            nMdiLeft -= item.Width;

                            item.Size = item.GetPreferredSize(displaySize);

                            if (g.DpiX > 120 || parent.ribbonTouchModeEnabled)
                            {
                                item.Location = new Point(nMdiLeft, nMdiTop + 10); //minimize button location
                            }
                            else if (g.DpiX > 96)
                            {
                                item.Location = new Point(nMdiLeft, nMdiTop + 5); //minimize button location
                            }
                            else
                                item.Location = new Point(nMdiLeft, nMdiTop);
                        }
                    }
                    if (parent.RibbonStyle == RibbonStyle.Office2013)
                    {
                        Ribbon2013MinimizeSystemButton item = parent.Ribbon2013MinimizeButton as Ribbon2013MinimizeSystemButton;

                        if (item != null && item.Visible)
                        {
                            nMdiLeft -= item.Width;
                            foreach (SystemButton button in parent.SystemButtons)
                            {
                                item.Size = button.Size; ;
                            }

                            if (parent.ribbonTouchModeEnabled)
                            {
                                if (form != null)
                                {
                                    int xLocation = form.Width - 57;
                                    foreach (SystemButton button in parent.SystemButtons)
                                    {
                                        if (button.Visible)
                                            xLocation -= button.Width;
                                    }
                                    if (form.CompositionEnabled)
                                        if (form.WindowState == FormWindowState.Normal)
                                        {
                                            item.Location = new Point(xLocation + RibbonForm.BORDER_WIDTH, 7);
                                        }
                                        else
                                        {
                                            item.Location = new Point(xLocation - (RibbonForm.BORDER_WIDTH + 4), 7);
                                        }
                                    else
                                        if (form.WindowState == FormWindowState.Normal)
                                        {
                                            item.Location = new Point(xLocation + RibbonForm.BORDER_WIDTH, 7);
                                        }
                                        else
                                        {
                                            item.Location = new Point(xLocation - (RibbonForm.BORDER_WIDTH + 7), 7);
                                        }
                                }
                            }
                            else
                            {
                             if (g.DpiX > 96)
                            {
                                if (form != null)
                                {
                                    int xLocation = form.Width - 40;
                                    foreach (SystemButton button in parent.SystemButtons)
                                    {
                                        if (button.Visible)
                                            xLocation -= button.Width;
                                    }
                                    if (form.CompositionEnabled)
                                        if (form.WindowState == FormWindowState.Normal)
                                        {
                                            item.Location = new Point(xLocation + RibbonForm.BORDER_WIDTH, 7);
                                        }
                                        else
                                        {
                                            item.Location = new Point(xLocation - (RibbonForm.BORDER_WIDTH + 4), 7);
                                        }
                                    else
                                        if (form.WindowState == FormWindowState.Normal)
                                        {
                                            item.Location = new Point(xLocation + RibbonForm.BORDER_WIDTH, 7);
                                        }
                                        else
                                        {
                                            item.Location = new Point(xLocation - (RibbonForm.BORDER_WIDTH + 7), 7);
                                        }
                                }
                            }
                                else
                                    if (form != null)
                                    {
                                        int xLocation = form.Width - 30;
                                        foreach (SystemButton button in parent.SystemButtons)
                                        {
                                            if (button.Visible)
                                                xLocation -= button.Width;
                                        }
                                        if (form.CompositionEnabled)
                                            if (form.WindowState == FormWindowState.Normal)
                                            {
                                                item.Location = new Point(xLocation + RibbonForm.BORDER_WIDTH, 7);
                                            }
                                            else
                                            {
                                                item.Location = new Point(xLocation - (RibbonForm.BORDER_WIDTH + 4), 7);
                                            }
                                        else
                                            if (form.WindowState == FormWindowState.Normal)
                                            {
                                                item.Location = new Point(xLocation + RibbonForm.BORDER_WIDTH, 7);
                                            }
                                            else
                                            {
                                                item.Location = new Point(xLocation - (RibbonForm.BORDER_WIDTH + 7), 7);
                                            }
                                    }
                            }
                        }
                    }
                    // Set rectangle in which TabItems must be painted.
                    parent.TabItemsRectangle = new Rectangle(tabsLeft, iTabItemsRectangleTop, iTabItemsRectangleRight - tabsLeft, iTabItemsRectangleHeight);

                    mainItemsHeight += SEPARATOR_WIDTH;

                    Rectangle quickRect = Rectangle.Empty;
                    //Set location of quick items.
                    if (!parent.ShowQuickPanelBelowRibbon)
                    {
                        parent.m_quickItems.Owner = parent;

                        int quickHeight = 0, quickPos = 0, len = 0, topPos = 0;

                        if (parent.RibbonStyle == RibbonStyle.Office2007)
                        {
                            quickHeight = parent.QuickPanelHeight;
                            quickPos = left;
                            len = iSysButtonsOffset - left - MIN_TITLE_TEXT_LENGTH;
                            topPos = rcDisplay.Y + smQuickItems.Top;
                        }
                        else
                        {
                            quickHeight = parent.QuickPanelHeight;
                            quickPos = (parent.Form != null && parent.Form.ShowIcon) ? DEF_ICON_SIZE + DEF_PADDING : DEF_PADDING;
                            len = iSysButtonsOffset - left - MIN_TITLE_TEXT_LENGTH;
                            topPos = rcDisplay.Y + smQuickItems.Top + 6;
                        }
                        quickRect = parent.m_quickItems.Layout(new Rectangle(quickPos, topPos, len, quickHeight));
                    }

                    // Calculate rectangle for title text.
                    string sTitle = parent.Title;

                    if (sTitle != string.Empty)
                    {
                        int x = 0;

                        if (parent.QuickPanelVisible && !parent.ShowQuickPanelBelowRibbon)
                        {
                            x = quickRect.Right;
                        }
                        else
                        {
                            x = left;
                        }

                        Rectangle rcText = new Rectangle(x, rcDisplay.Y + 1, iSysButtonsOffset - x, parent.QuickPanelHeight - 1);

                        TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix;
                        Size textSize = TextRenderer.MeasureText(sTitle, parent.TitleFont, rcText.Size, flags);

                        switch (parent.TitleAlignment)
                        {
                            case TextAlignment.Center:
                                rcText.X = rcText.X + (rcText.Width - textSize.Width) / 2;
                                break;
                            case TextAlignment.Right:
                                rcText.X = rcText.Right - textSize.Width;
                                break;
                            case TextAlignment.Left:
                                if ((parent != null) && (parent.Parent != null) && (parent.Parent is RibbonControlAdv)
                                    && (parent.Parent as RibbonControlAdv).RibbonStyle != RibbonStyle.Office2007)
                                {
                                    if (!(parent.Parent as RibbonControlAdv).QuickPanelVisible && (parent.Parent as RibbonControlAdv).MenuButtonVisible)
                                        rcText.X = 36;
                                    else if (!(parent.Parent as RibbonControlAdv).QuickPanelVisible)
                                        rcText.X = rcText.X + (rcText.Width - textSize.Width) / 12;

                                }
                                break;
                        }

                        rcText.Width = textSize.Width;
                        parent.m_rcTitle = rcText;
                    }
                    else
                    {
                        parent.m_rcTitle = Rectangle.Empty;
                    }

                    // Manage tab groups.
                    int top, height;
                    if (parent.RibbonStyle == RibbonStyle.Office2007)
                    {
                        top = rcDisplay.Y + 1;
                        height = parent.QuickPanelHeight - 1;
                    }
                    else
                    {
                        int offset = parent.TabItemsRectangle.Height;
                        top = rcDisplay.Y;
                        height = rcDisplay.Height - offset / 2;
                    }

                    int leftAllowed = 0;

                    if (parent.QuickPanelVisible && !parent.ShowQuickPanelBelowRibbon)
                        leftAllowed = quickRect.Right;
                    else
                        leftAllowed = left;

                    int rightAllowed = iSysButtonsOffset;
                    Rectangle titleRect = parent.m_rcTitle;

                    ToolStripTabGroup currentGroup = null;
                    int curGroupLeft = 0;

                    foreach (ToolStripTabGroup group in parent.m_groups)
                    {
                        group.BoundsList.Clear();
                    }

                    for (int i = 0, count = visibleMainItems.Count; i < count; i++)
                    {
                        ToolStripItem item = visibleMainItems[i];

                        bool bGroupOK =
                            (item.Bounds.Left >= leftAllowed && item.Bounds.Right <= rightAllowed
                            && (titleRect.Left > item.Bounds.Right || titleRect.Right < item.Bounds.Left));

                        ToolStripTabGroup group = null;
                        if (bGroupOK && parent.m_hashGroups.ContainsKey(item))
                        {
                            group = parent.m_hashGroups[item];
                        }

                        if (currentGroup != group)
                        {
                            if (currentGroup != null)
                            {
                                currentGroup.BoundsList.Add(new Rectangle(curGroupLeft, top, item.Bounds.Left - curGroupLeft, height));
                            }

                            currentGroup = group;
                            if (currentGroup != null)
                            {
                                curGroupLeft = item.Bounds.Left;
                            }
                        }

                        if (currentGroup != null && i == count - 1)
                        {
                            currentGroup.BoundsList.Add(new Rectangle(curGroupLeft, top, item.Bounds.Right - curGroupLeft + 1, height));
                        }
                    }
                }
				return parent.AutoSize;
			}
			/// <summary>
			/// Lays out toolstrip items in RibbonControlAdv from right to left.
			/// </summary>
			/// <param name="parent"></param>
			/// <param name="layoutEventArgs"></param>
			/// <returns></returns>
			private bool LayoutRightToLeft( RibbonControlAdvHeader parent, LayoutEventArgs layoutEventArgs )
			{
				Rectangle rcDisplay = parent.DisplayRectangle;
				Size displaySize = rcDisplay.Size;

				int totalHeight = displaySize.Height;
				int right = rcDisplay.Right;

				ToolStripMenuButton menuButton = parent.m_menuButton;

				// Set location of MenuButton.
				if( menuButton.Available && menuButton.GetCurrentParent() == parent )
				{
					int width = Math.Max( parent.MenuButtonWidth, totalHeight - 2 * MENUBUTTON_MARGIN );
                      Bitmap bit = new Bitmap(10,10);
                      using (Graphics g = Graphics.FromImage(bit))
                      {
                          Size textsize = TextRenderer.MeasureText(menuButton.Text, menuButton.Font);
                          if (parent.m_owner.RibbonStyle == RibbonStyle.Office2007)
                          {
                              if (g.DpiX > 120)
                              {
                                  menuButton.Size = new Size(width - 5, width - 5);
                                  Point point = new Point(right - MENUBUTTON_MARGIN - menuButton.Width, rcDisplay.Y + MENUBUTTON_MARGIN);
                                  parent.SetItemLocation(menuButton, point);
                              }
                              else if (g.DpiX > 96)
                              {
                                  menuButton.Size = new Size(width-5 , width-5 );
                                  Point point = new Point(right - MENUBUTTON_MARGIN - menuButton.Width, rcDisplay.Y + MENUBUTTON_MARGIN);
                                  parent.SetItemLocation(menuButton, point);
                              }
                              else
                              {
                                  menuButton.Size = new Size(width, width);
                                  Point point = new Point(right - MENUBUTTON_MARGIN - menuButton.Width, rcDisplay.Y + MENUBUTTON_MARGIN);
                                  parent.SetItemLocation(menuButton, point);
                              }
                          }
                          else if (parent.m_owner.RibbonStyle == RibbonStyle.Office2010)
                          {
                              if (g.DpiX > 120)
                              {
                                  if (RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_150 + 16 > textsize.Width && textsize.Height < 35)
                                      menuButton.Size = new Size(RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_150 + 16, 35);
                                  else
                                      menuButton.Size = textsize;
                                  Point point = new Point(right - MENUBUTTON_MARGIN_150 - menuButton.Width, rcDisplay.Bottom + parent.BorderSize.Height + 1 - menuButton.Height + 1);
                                  parent.SetItemLocation(menuButton, point);
                              }
                              else if (g.DpiX > 96)
                              {
                                  if (RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_125 + 14 > textsize.Width && textsize.Height < 30)
                                      menuButton.Size = new Size(RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_125 + 14, 30);
                                  else
                                      menuButton.Size = textsize;
                                  Point point = new Point(right - MENUBUTTON_MARGIN_125 - menuButton.Width, rcDisplay.Bottom + parent.BorderSize.Height + 1 - menuButton.Height + 1);
                                  parent.SetItemLocation(menuButton, point);
                              }
                              else
                              {
                                  menuButton.Size = new Size(width, 25);
                                  Point point;
                                  if (parent.Form != null && parent.Form is RibbonForm && !(parent.Form as RibbonForm).CompositionEnabled)
                                      point = new Point(right - (RibbonForm.BORDER_WIDTH - MENUBUTTON_MARGIN) - menuButton.Width, rcDisplay.Bottom + parent.BorderSize.Height + 1 - menuButton.Height + 1);
                                  else
                                      point = new Point(right - MENUBUTTON_MARGIN - menuButton.Width, rcDisplay.Bottom + parent.BorderSize.Height + 1 - menuButton.Height + 1);
                                  parent.SetItemLocation(menuButton, point);
                              }
                          }
                          else if (parent.m_owner.RibbonStyle == RibbonStyle.Office2013)
                          {
                              if (g.DpiX > 120 || parent.ribbonTouchModeEnabled)
                              {
                                  if (RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_150 + 16 > textsize.Width && textsize.Height < 35)
                                      menuButton.Size = new Size(RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_150 + 16, 35);
                                  else
                                      menuButton.Size = textsize;
                                  Point point = new Point(right  - menuButton.Width, rcDisplay.Bottom + parent.BorderSize.Height + 1 - menuButton.Height + 1);
                                  parent.SetItemLocation(menuButton, point);
                              }
                              else if (g.DpiX > 96)
                              {
                                  if (RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_125 + 14 > textsize.Width && textsize.Height < 30)
                                      menuButton.Size = new Size(RibbonControlAdv.DEF_MENU_BUTTON_WIDTH_125 + 14, 30);
                                  else
                                      menuButton.Size = textsize;
                                  Point point = new Point(right- menuButton.Width, rcDisplay.Bottom + parent.BorderSize.Height + 1 - menuButton.Height + 1);
                                  parent.SetItemLocation(menuButton, point);
                              }
                              else
                              {
                                  menuButton.Size = new Size(width, 25);
                                  Point point = new Point(right - menuButton.Width, rcDisplay.Bottom + parent.BorderSize.Height + 1 - menuButton.Height + 1);
                                  parent.SetItemLocation(menuButton, point);
                              }
                          }
                          if (parent.m_owner.RibbonStyle == RibbonStyle.Office2013)
                              right = menuButton.Bounds.Left - 2;
                          else
                              right = menuButton.Bounds.Left - MENUBUTTON_GAP;
                      }
				}

				int tabsRight = rcDisplay.Right - TAB_OFFSET;

				// Correct tabsOffset.
				if( menuButton.Available && menuButton.Bounds.Left - MENUBUTTON_GAP < tabsRight )
				{
					if (parent.m_owner.RibbonStyle == RibbonStyle.Office2013)
						tabsRight = menuButton.Bounds.Left - 2;
					else
						tabsRight = menuButton.Bounds.Left - MENUBUTTON_GAP;
				}

				// Set location of System buttons and calculate their width.
				int iSysButtonsOffset;
                int Itemlocation = 4;
				RibbonForm form = parent.Form;
				if( form == null || !form.CompositionEnabled || form.RibbonStyle == RibbonStyle.Office2013)
				{
					iSysButtonsOffset = rcDisplay.X + smSystemButtons.Left + 5;

					if( parent.m_bDisplaySysButtons )
					{
						int buttonsTop = rcDisplay.Y + smSystemButtons.Top;

						buttonsTop += (parent.RibbonStyle == RibbonStyle.Office2010 || parent.RibbonStyle == RibbonStyle.Office2013) ? 6 : 0;

						for( int i = parent.SystemButtons.Count - 1; i >= 0; i-- )
						{
							SystemButton item = parent.SystemButtons[i] as SystemButton;

							if( item != null && item.Available )
							{
								item.Size = item.GetPreferredSize( displaySize );

								item.Location = new Point( iSysButtonsOffset, buttonsTop );

								iSysButtonsOffset += item.Width;
							}
						}
					}
                    else if (parent.RibbonStyle == RibbonStyle.Office2013)
                    {
                        int buttonsTop = rcDisplay.Y + smSystemButtons.Top;
                        for (int i = parent.SystemButtons.Count - 1; i >= 0; i--)
                        {
                            SystemButton item = parent.SystemButtons[i] as SystemButton;
                            if (item != null && item.Available)
                            {
                                item.Size = item.GetPreferredSize(displaySize);
                                iSysButtonsOffset -= item.Width;
                                item.Location = new Point(Itemlocation, 7);
                                Itemlocation += item.Width;
                            }
                        }
                    }

					iSysButtonsOffset += smSystemButtons.Right;
				}
				else
				{
					iSysButtonsOffset = form.DwmButtonBounds.Right + smSystemButtons.Right;
				}

				// Set location of Main Items.
				int mainItemsHeight = DEF_PANEL_HEIGHT;
				int iTabItemsRectangleHeight = mainItemsHeight;

				int iTabItemsRectangleTop = rcDisplay.Bottom - DEF_PANEL_HEIGHT;
				int iTabItemsRectangleLeft = rcDisplay.X + smSystemButtons.Left;

				MdiButtonsCollection mdiButtons = new MdiButtonsCollection( parent.MdiButtons, displaySize );

				if( parent.m_bDisplayMdiButtons )
				{
					iTabItemsRectangleLeft += mdiButtons.Width;
				}

				if( parent.m_bDisplayHelpButton )
				{
					SystemButton item = parent.HelpButton as SystemButton;

					if( item != null && item.Available )
					{
						item.Size = item.GetPreferredSize( displaySize );
						iTabItemsRectangleLeft += item.Width;
					}
				}

				if (parent.m_bDisplayMinimizeButton)
				{
					MinimizeSystemButton item = parent.MinimizeButton as MinimizeSystemButton;

					if (item != null && item.Available)
					{
						item.Size = item.GetPreferredSize(displaySize);
						iTabItemsRectangleLeft += item.Width;
					}
				}
                if (parent.RibbonStyle == RibbonStyle.Office2013)
                {
                    Ribbon2013MinimizeSystemButton item = parent.Ribbon2013MinimizeButton as Ribbon2013MinimizeSystemButton;
                    Bitmap bit = new Bitmap(10, 10);
                    using (Graphics g = Graphics.FromImage(bit))
                    {
                        if (item != null && item.Available)
                        {
                            item.Size = item.GetPreferredSize(displaySize);
                            iTabItemsRectangleLeft += item.Width;
                            int itemloc = 3;
                            if (form != null && !form.CompositionEnabled)
                                itemloc = parent.BorderSize.Width + 2;
                            for (int i = parent.SystemButtons.Count - 1; i >= 0; i--)
                            {
                                SystemButton item1 = parent.SystemButtons[i] as SystemButton;
                                if (item1.Visible)
                                    itemloc += item1.Width;
                            }
                            if (form != null)
                            {
                                if (form.MaximizeBox || form.MinimizeBox)
                                {
                                    if (form.HelpButton)
                                        item.Location = new Point(itemloc, 7);
                                    else
                                        item.Location = new Point(itemloc, 7);
                                }
                                else
                                    item.Location = new Point(itemloc, 7);
                            }
                        }
                    }
                    bit.Dispose();
                }
				MainItemsList visibleMainItems = new MainItemsList( parent, parent.m_mainItems, displaySize, tabsRight - iTabItemsRectangleLeft );

				parent.m_iSepatators.Clear();

				for( int i = 0, count = visibleMainItems.Count, x = tabsRight - parent.ScrollPositionInternal; i < count; i++ )
				{
					ToolStripItem item = visibleMainItems[i];

					int offset = (parent.RibbonStyle == RibbonStyle.Office2007) ? 0 : 1 + parent.BorderSize.Height;

					Point point = new Point( x - item.Width, rcDisplay.Bottom + offset - item.Height );

					parent.SetItemLocation( item, point );

					parent.m_iSepatators.Add( point.X - ( TAB_ITEMS_SEPARATOR_WIDTH / 2 ) );

					if( item.Height + item.Margin.Vertical > mainItemsHeight )
					{
						mainItemsHeight = item.Height + item.Margin.Vertical;
						iTabItemsRectangleHeight = item.Height;
						iTabItemsRectangleTop = rcDisplay.Bottom - item.Height;
					}

					x -= ( item.Width + TAB_ITEMS_SEPARATOR_WIDTH );
				}

				// Set location of Mdi buttons and Help Button and calculate their width.
				int nMdiTop = rcDisplay.Y + parent.QuickPanelHeight + smSystemButtons.Top;
				int nMdiLeft = rcDisplay.X + smSystemButtons.Left;

				if( parent.m_bDisplayMdiButtons )
				{

					for( int i = mdiButtons.Count - 1; i >= 0; i-- )
					{
						SystemButton item = mdiButtons[i] as SystemButton;

						if( item != null )
						{
							item.Location = new Point( nMdiLeft, nMdiTop );
							nMdiLeft += item.Width;
						}
					}
				}

				if (parent.RibbonStyle == RibbonStyle.Office2010 || parent.RibbonStyle == RibbonStyle.Office2010)
				{
					nMdiLeft += 6;
					nMdiTop += 3;
				}

				if( parent.m_bDisplayHelpButton )
				{
					SystemButton item = parent.HelpButton as SystemButton;

					if( item != null && item.Available )
					{
						item.Size = item.GetPreferredSize( displaySize );
                        if (parent.RibbonStyle != RibbonStyle.Office2013)
                            item.Location = new Point(nMdiLeft, nMdiTop);
                        else if(parent.RibbonStyle == RibbonStyle.Office2013)
                        {
                            int itemloc = 3;
                            for (int i = parent.SystemButtons.Count - 1; i >= 0; i--)
                            {
                                SystemButton item1 = parent.SystemButtons[i] as SystemButton;
                                if (item1.Visible)
                                    itemloc += item1.Width;
                            }
                            if (form != null)
                            {
                                item.Location = new Point(itemloc + item.Width, 7);
                            }
                        }
					}
				}

				if (parent.m_bDisplayMinimizeButton)
				{
					MinimizeSystemButton item = parent.MinimizeButton as MinimizeSystemButton;

					if (item != null && item.Available)
					{
						nMdiLeft += item.Width;

						item.Size = item.GetPreferredSize(displaySize);

						item.Location = new Point(nMdiLeft, nMdiTop);
					}
				}

				// Set rectangle in which TabItems must be painted.
				parent.TabItemsRectangle = new Rectangle( iTabItemsRectangleLeft, iTabItemsRectangleTop, tabsRight - iTabItemsRectangleLeft, iTabItemsRectangleHeight );

				mainItemsHeight += SEPARATOR_WIDTH;

				Rectangle quickRect = Rectangle.Empty;
				//Set location of quick items.
				if( !parent.ShowQuickPanelBelowRibbon )
				{
					int quickHeight = 0, quickLeft = 0, quickTop = 0, quickLen = 0;

					if (parent.RibbonStyle == RibbonStyle.Office2007)
					{
						parent.m_quickItems.Owner = parent;
						quickHeight = parent.QuickPanelHeight;
						quickLeft = iSysButtonsOffset + MIN_TITLE_TEXT_LENGTH;
						quickTop = rcDisplay.Y + smQuickItems.Top;

						quickLen = right - quickLeft;
					}
					else
					{
						parent.m_quickItems.Owner = parent;
						quickHeight = parent.QuickPanelHeight;
						quickLeft = (parent.Form != null && parent.Form.ShowIcon)
										? iSysButtonsOffset + MIN_TITLE_TEXT_LENGTH + DEF_ICON_SIZE
										: iSysButtonsOffset + MIN_TITLE_TEXT_LENGTH;
						quickTop = rcDisplay.Y + smQuickItems.Top + 6;

						quickLen = right - quickLeft;
                        Bitmap bit = new Bitmap(10, 10);
                        using (Graphics g = Graphics.FromImage(bit))
                        {
                            if (g.DpiX > 120 || parent.ribbonTouchModeEnabled)
                                quickLeft = quickLeft + 45; //Qucik items position on RTL
                            else if (g.DpiX > 96) 
                                quickLeft = quickLeft + 30; //Qucik items position on RTL
                        }
                        bit.Dispose();
					}

					quickRect = parent.m_quickItems.Layout( new Rectangle( quickLeft, quickTop, quickLen, quickHeight ) );
				}

				// Calculate rectangle for title text.
				string sTitle = parent.Title;
				if( sTitle != string.Empty )
				{
					int textLeft = iSysButtonsOffset;
					int textWidth = 0;

					if( parent.QuickPanelVisible && !parent.ShowQuickPanelBelowRibbon )
					{
						textWidth = quickRect.Left - textLeft;
					}
					else
					{
						textWidth = right - textLeft;
					}

					Rectangle rcText = new Rectangle( textLeft, rcDisplay.Y + 1, textWidth, parent.QuickPanelHeight - 1 );

					TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter|TextFormatFlags.NoPrefix;
					Size textSize = TextRenderer.MeasureText( sTitle, parent.TitleFont, rcText.Size, flags );

					Point loc = Point.Empty;
					int y = rcText.Top + ( rcText.Height - textSize.Height ) / 2;
					switch( parent.TitleAlignment )
					{
						case TextAlignment.Right:
						loc = new Point( rcText.Left, y );
						break;
						case TextAlignment.Center:
						loc = new Point( rcText.Left + ( rcText.Width - textSize.Width ) / 2, y );
						break;
                        case TextAlignment.Left:
                        loc = new Point(rcText.Right - textSize.Width, y);
                        break;
                    }

					parent.m_rcTitle = new Rectangle( loc, textSize );
				}
				else
				{
					parent.m_rcTitle = Rectangle.Empty;
				}

				// Manage tab groups.
				int top, height;
				if (parent.RibbonStyle == RibbonStyle.Office2007)
				{
					top = rcDisplay.Y + 1;
					height = parent.QuickPanelHeight - 1;
				}
				else
				{
					int offset = parent.TabItemsRectangle.Height;
					top = rcDisplay.Y;
					height = rcDisplay.Height - offset / 2;
				}

				int leftAllowed = iSysButtonsOffset;
				int rightAllowed = parent.QuickPanelVisible && !parent.ShowQuickPanelBelowRibbon ? quickRect.Left : right;

				Rectangle titleRect = parent.m_rcTitle;

				ToolStripTabGroup currentGroup = null;
				int curGroupRight = 0;

				foreach( ToolStripTabGroup group in parent.m_groups )
				{
					group.BoundsList.Clear();
				}

				for( int i = 0, count = visibleMainItems.Count; i < count; i++ )
				{
					ToolStripItem item = visibleMainItems[i];

					bool bGroupOK =
						( item.Bounds.Left >= leftAllowed && item.Bounds.Right <= rightAllowed
						&& ( titleRect.Left > item.Bounds.Right || titleRect.Right < item.Bounds.Left ) );

					ToolStripTabGroup group = null;
					if( bGroupOK && parent.m_hashGroups.ContainsKey( item ) )
					{
						group = parent.m_hashGroups[item];
					}

					if( currentGroup != group )
					{
						if( currentGroup != null )
						{
							currentGroup.BoundsList.Add( new Rectangle( item.Bounds.Right, top, curGroupRight - item.Bounds.Right, height ) );
						}

						currentGroup = group;
						if( currentGroup != null )
						{
							curGroupRight = item.Bounds.Right;
						}
					}

					if( currentGroup != null && i == count - 1 )
					{
						currentGroup.BoundsList.Add( new Rectangle( item.Bounds.Left, top, curGroupRight - item.Bounds.Left + 1, height ) );
					}
				}

				return parent.AutoSize;
			}
			#endregion
		}

		#endregion

		#region *** SystemButton
		internal class SystemButton: ToolStripButton, ICustomItem
		{
			#region Constructors
			public SystemButton( RibbonControlAdvHeader header, int sysCommand )
				: base()
			{
				m_images = new Hashtable();
				m_imagesSelected = new Hashtable();

				m_header = header;
				m_sysCommand = sysCommand;

				this.Parent = header;
				this.AutoToolTip = false;
				this.Margin = new Padding( 0 );
				this.Padding = new Padding( IMAGE_PADDING );
				this.ImageScaling = ToolStripItemImageScaling.None;

				m_header.RendererChanged += new EventHandler( OnRendererChanged );
			}
			#endregion

			#region Overrides
			/// <summary>
			/// 
			/// </summary>
			/// <param name="disposing"></param>
			protected override void Dispose( bool disposing )
			{
				if( disposing )
				{
					this.Parent = null;

					if( m_header != null )
					{
						m_header.RendererChanged -= new EventHandler( OnRendererChanged );
						m_header = null;
					}

					ClearImages();
				}
				base.Dispose( disposing );
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="constrainingSize"></param>
			/// <returns></returns>
			public override Size GetPreferredSize( Size constrainingSize )
            {
                Bitmap bit = new Bitmap(10, 10);
                using (Graphics g = Graphics.FromImage(bit))
                {
                    if ((this.Parent is RibbonControlAdvHeader) && (this.Parent as RibbonControlAdvHeader).RibbonStyle == RibbonStyle.Office2010)
                    {
                        if (g.DpiX > 120)
                        {
                            return new Size(50, 29);
                        }
                        else if (g.DpiX > 96)
                        {
                            return new Size(42, 23);
                        }
                        else
                        {
                            return new Size(35, 19);
                        }
                    }
                    else  if ((this.Parent is RibbonControlAdvHeader) && (this.Parent as RibbonControlAdvHeader).RibbonStyle == RibbonStyle.Office2013)
                    {
                        if (g.DpiX > 120|| (this.Parent as RibbonControlAdvHeader).ribbonTouchModeEnabled)
                        {
                            return new Size(50, 29);
                        }
                        else if (g.DpiX > 96)
                        {
                            return new Size(30, 26);
                        }
                        else
                        {
                            return new Size(25, 19);
                        }
                    }
                    else
                    {
                        if (g.DpiX > 120)
                            return new Size(50, 29);
                        else if (g.DpiX > 96)
                            return new Size(42, 23);
                        //return new Size(SystemImages.IMAGE_SIZE + this.Padding.Horizontal + 8, SystemImages.IMAGE_SIZE + this.Padding.Vertical + 8);
                        else
                            return new Size(SystemImages.IMAGE_SIZE + this.Padding.Horizontal, SystemImages.IMAGE_SIZE + this.Padding.Vertical);
                    }
                }
			}
            private void findcontrol(Control cont)
            {
                foreach (Control ctrl in cont.Controls)
                {
                    ctrl.LostFocus += new EventHandler(ctrl_LostFocus);
                    if (ctrl.Controls.Count > 0)
                        findcontrol(ctrl);
                }
            }

            void ctrl_LostFocus(object sender, EventArgs e)
            {
                this.Invalidate();
                this.m_header.Invalidate();
            }
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnPaint( PaintEventArgs e )
			{
                if (this.m_header != null && this.m_header.Form != null)
                {
                    this.m_header.Form.LostFocus += new EventHandler(Form_LostFocus);
                    this.m_header.Form.GotFocus += new EventHandler(Form_GotFocus);
                    foreach (Control ctrl in this.m_header.Form.Controls)
                    {
                        ctrl.LostFocus += new EventHandler(ctrl_LostFocus);
                        if (ctrl.Controls.Count > 0)
                        {
                            findcontrol(ctrl);
                        }
                    }
                }
				if (this is HelpSystemButton && this.m_header != null && this.m_header.Form != null)
				{
					this.Image = this.m_header.Form.HelpButtonImage;
				}
				m_header.Renderer.DrawButtonBackground( new ToolStripItemRenderEventArgs( e.Graphics, this ) );
				if( this.Image != null )
				{
					Point loc = new Point( (this.Width - this.Image.Width)/2,(this.Height - this.Image.Height)/2);
					Rectangle rc = new Rectangle(loc, this.Image.Size);
					m_header.Renderer.DrawItemImage( new ToolStripItemImageRenderEventArgs( e.Graphics, this, rc ) );
				}
			}

            void Form_GotFocus(object sender, EventArgs e)
            {
                this.Invalidate();
                this.m_header.Invalidate();
            }

            void Form_LostFocus(object sender, EventArgs e)
            {
                this.Invalidate();
                this.m_header.Invalidate();
            }
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnClick( EventArgs e )
			{
				base.OnClick( e );

				Form form = this.Form;

				if( form != null )
				{
					switch( this.SysCommand )
					{
						case (int)SystemCommand.SC_MAXIMIZE:
						case (int)SystemCommand.SC_MINIMIZE:
						case (int)SystemCommand.SC_RESTORE:
						case (int)SystemCommand.SC_CLOSE:
						form.BeginInvoke( new SendMessageDelegate( WindowsAPI.SendMessage ), new object[] { form.Handle, Msg.WM_SYSCOMMAND, this.SysCommand, 0 } );
						break;
						case (int)SystemCommand.SC_CONTEXTHELP:
						Point p = Cursor.Position;
						int lParam = WindowsAPI.MAKELONG( p.X, p.Y );
						WindowsAPI.SendMessage( form.Handle, (int)Msg.WM_SYSCOMMAND, (int)SystemCommand.SC_CONTEXTHELP, lParam.ToString() );
						return;
					}
				}
			}
			#endregion

			#region Event handlers
			void OnRendererChanged( object sender, EventArgs e )
			{
				ClearImages();
			}
			#endregion

			#region Implementation
			/// <summary>
			/// 
			/// </summary>
			void ClearImages()
			{
				foreach( Image image in m_images.Values )
				{
					image.Dispose();
				}
				m_images.Clear();

				foreach( Image image in m_imagesSelected.Values )
				{
					image.Dispose();
				}
				m_imagesSelected.Clear();

				if (imagesOffice2010 != null)
				{
					imagesOffice2010.Images.Clear();
					imagesOffice2010 = null;
				}
			}

			private Image GetOffice2010Image(int command)
			{
				Image img = null;

                if (imagesOffice2010 == null)
                {
                    imagesOffice2010 = new ImageList();
                    Bitmap bmp = new Bitmap(15, 16);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        if (g.DpiX > 120)
                        {
                            bmp = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.150DPIRibbonControlAdv2010.bmp"));
                            bmp.MakeTransparent(Color.Magenta);
                            imagesOffice2010.ImageSize = new Size(25, 25);
                            imagesOffice2010.ColorDepth = ColorDepth.Depth32Bit;
                            imagesOffice2010.Images.AddStrip(bmp);
                        }
                        else if (g.DpiX > 96)
                        {
                            bmp = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.125DPIRibbonControlAdv2010.bmp"));
                            bmp.MakeTransparent(Color.Magenta);
                            imagesOffice2010.ImageSize = new Size(20, 20);
                            imagesOffice2010.ColorDepth = ColorDepth.Depth32Bit;
                            imagesOffice2010.Images.AddStrip(bmp);
                        }
                        else
                        {
                            bmp = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.Office2010SystemButtons.bmp"));
                            bmp.MakeTransparent(Color.Magenta);
                            imagesOffice2010.ImageSize = new Size(15, 16);
                            imagesOffice2010.ColorDepth = ColorDepth.Depth32Bit;
                            imagesOffice2010.Images.AddStrip(bmp);
                        }
                    }
                }
				switch (command)
				{
					case (int)SystemCommand.SC_MINIMIZE:
						img = imagesOffice2010.Images[SystemButton.MinimizeButtonID];
						break;
					case (int)SystemCommand.SC_MAXIMIZE:
						img = imagesOffice2010.Images[SystemButton.MaximizeButtonID];
						break;
					case (int)SystemCommand.SC_RESTORE:
						img = imagesOffice2010.Images[SystemButton.RestoreButtonID];
						break;
					case (int)SystemCommand.SC_CLOSE:
						img = imagesOffice2010.Images[SystemButton.CloseButtonID];
						break;
					case (int)SystemCommand.SC_CONTEXTHELP:
						img = SystemImages.GetOffice2010ImageHelp();
						break;
				}

				return img;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="command"></param>
			/// <returns></returns>
			Image GetImage( int command )
            {
                Hashtable images = (this.Selected) ? (m_imagesSelected) : (m_images);
                Image image = images[command] as Image;
                {
                    bool mdiactivate = false;
                    if (m_header.FindForm() != null)
                    {
                        foreach (Form mdi in this.m_header.Form.MdiChildren)
                        {
                            if (mdi.IsHandleCreated)
                            {
                                mdiactivate = true;
                              
                            }
                        }
                    }
                    switch (command)
                    {

                        case (int)SystemCommand.SC_MINIMIZE:
                            image = (this.m_header.Form != null && !this.m_header.Form.ContainsFocus) && !(this.m_header.Form as RibbonForm).designmode && !mdiactivate ? SystemImages.GetImageMinimize(Color.LightGray, this.m_header.m_owner.RibbonTouchModeEnabled) : SystemImages.GetImageMinimize(this.ImageForeground, this.m_header.m_owner.RibbonTouchModeEnabled);
                            break;
                        case (int)SystemCommand.SC_MAXIMIZE:
                            image = this.m_header.Form != null && !this.m_header.Form.ContainsFocus && !(this.m_header.Form as RibbonForm).designmode && !mdiactivate ? SystemImages.GetImageMaximize(Color.LightGray, this.m_header.m_owner.RibbonTouchModeEnabled) : SystemImages.GetImageMaximize(this.ImageForeground, this.m_header.m_owner.RibbonTouchModeEnabled);
                            break;
                        case (int)SystemCommand.SC_RESTORE:
                            image = this.m_header.Form != null && !this.m_header.Form.ContainsFocus && !(this.m_header.Form as RibbonForm).designmode && !mdiactivate ? SystemImages.GetImageRestore(Color.LightGray, this.m_header.m_owner.RibbonTouchModeEnabled) : SystemImages.GetImageRestore(this.ImageForeground, this.m_header.m_owner.RibbonTouchModeEnabled);
                            break;
                        case (int)SystemCommand.SC_CLOSE:
                            image = this.m_header.Form != null && !this.m_header.Form.ContainsFocus && !(this.m_header.Form as RibbonForm).designmode && !mdiactivate ? SystemImages.GetImageClose(Color.LightGray, this.m_header.m_owner.RibbonTouchModeEnabled) : SystemImages.GetImageClose(this.ImageForeground, this.m_header.m_owner.RibbonTouchModeEnabled);
                            break;
                        case (int)SystemCommand.SC_CONTEXTHELP:
                            image = SystemImages.GetImageHelp();
                            break;
                    }
                    images[command] = image;
                }
                return image;
            }
			#endregion

			#region ICustomItem Members

			ToolStrip ICustomItem.Owner
			{
				get
				{
					return m_header;
				}
			}

			ToolStripItemPlacement ICustomItem.Placement
			{
				get
				{
					return m_header.DisplayedItems.Contains( this ) ? ToolStripItemPlacement.Main : ToolStripItemPlacement.None;
				}
			}

			#endregion

			#region Properties
			/// <summary>
			/// 
			/// </summary>
			public override Image Image
			{
				get
				{
					if( base.Image == null )
					{
						if(this.m_header.RibbonStyle == RibbonStyle.Office2007)
							return GetImage( this.SysCommand );
						else if (this.m_header.RibbonStyle == RibbonStyle.Office2013)
							return GetImage(this.SysCommand);
						else
							return GetOffice2010Image(this.SysCommand);
					}
					return base.Image;
				}
				set
				{
					base.Image = value;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			public override string Text
			{
				get
				{
					Form form = m_header.FindForm();

					if( form != null && form.IsHandleCreated )
					{
						IntPtr hSysMenu = NativeMethods.GetSystemMenu( form.Handle, false );

						if( hSysMenu != IntPtr.Zero )
						{
							String text = new String( '\0', 64 );
							int res = NativeMethods.GetMenuString( hSysMenu, (uint)this.SysCommand, text, 64, NativeMethods.MF_BYCOMMAND );

							if( res > 0 )
							{
								int nShotcutStart = text.IndexOf( '\t' );

								if( nShotcutStart >= 0 )
								{
									text = text.Substring( 0, nShotcutStart );
								}
								else
								{
									int nEnd = text.IndexOf( '\0' );

									if( nEnd >= 0 )
									{
										text = text.Substring( 0, nEnd );
									}
								}

								int nAmp = text.IndexOf( '&' );

								if( nAmp >= 0 )
								{
									text = text.Remove( nAmp, 1 );
								}

								return text;
							}
						}
					}

					switch( this.SysCommand )
					{
						case (int)SystemCommand.SC_MINIMIZE:
							return SR.GetString(SR.ToolTipCaptionButtonMinimize, m_header);
						case (int)SystemCommand.SC_MAXIMIZE:
							return SR.GetString(SR.ToolTipCaptionButtonMaximize, m_header);
						case (int)SystemCommand.SC_CLOSE:
							return SR.GetString(SR.ToolTipCaptionButtonClose, m_header);
						case (int)SystemCommand.SC_RESTORE:
							return SR.GetString(SR.ToolTipCaptionButtonRestore, m_header);
						case (int)SystemCommand.SC_CONTEXTHELP:
							{
								RibbonForm ribbonForm = this.Form as RibbonForm;
								if (ribbonForm != null)
									return ribbonForm.HelpButtonToolTip;
								return SR.GetString(SR.ToolStripItemHelpButton, m_header);
							}
					}

					return base.Text;
				}
				set
				{
					base.Text = value;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public int SysCommand
			{
				get
				{
					if( m_sysCommand == (int)SystemCommand.SC_MAXIMIZE )
					{
						Form form = this.Form;
						if( form != null )
						{
							if( form.WindowState == FormWindowState.Maximized )
							{
								return (int)SystemCommand.SC_RESTORE;
							}
						}
					}
					return m_sysCommand;
				}
			}
			/// <summary>
			/// Gets or Sets coordinates of the upper-left corner
			/// </summary>
			public Point Location
			{
				get
				{
					return this.Bounds.Location;
				}
				set
				{
					SetBounds( new Rectangle( value, this.Size ) );
				}
			}
			/// <summary>
			/// 
			/// </summary>
			protected virtual Form Form
			{
				get
				{
					return m_header.Form;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			Color ImageForeground
			{
				get
				{
					ToolStripProfessionalRenderer renderer = m_header.Renderer as ToolStripProfessionalRenderer;
					if( renderer != null )
					{
						Office12ColorTable clrTable = renderer.ColorTable as Office12ColorTable;
						if( clrTable != null )
						{
							return ( this.Selected ) ? ( clrTable.SystemButtonForegroundSelected ) : ( clrTable.SystemButtonForeground );
						}
					}
					return SystemColors.ControlText;
				}
			}
			#endregion

			#region Fields
			RibbonControlAdvHeader m_header;
			const int RestoreButtonID = 0;
			const int MinimizeButtonID = 1;
			const int MaximizeButtonID = 2;
			const int CloseButtonID = 3;
			int m_sysCommand;

			Hashtable m_images;
			Hashtable m_imagesSelected;
			ImageList imagesOffice2010;
			#endregion
		}
		#endregion

		#region *** MdiSystemButton
		class MdiSystemButton: SystemButton
		{
			#region Constructors
			public MdiSystemButton( RibbonControlAdvHeader parent, int sysCommand )
				: base( parent, sysCommand )
			{
				m_mdiCommand = sysCommand;
			}
			#endregion

			#region Properties
			protected override Form Form
			{
				get
				{
					Form mainForm = base.Form;

					if( mainForm != null && mainForm.IsMdiContainer )
					{
						return mainForm.ActiveMdiChild;
					}

					return null;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public int MdiCommand
			{
				get
				{
					return m_mdiCommand;
				}
			}
			#endregion

			#region Fields
			/// <summary>
			/// 
			/// </summary>
			int m_mdiCommand;
			#endregion
		}
		#endregion

		#region *** HelpSystemButton
		class HelpSystemButton: SystemButton
		{
			#region Constants
			int HELP_IMAGE_SIZE = 21;
            int OFFICE2010_Height = 19;
            int OFFICE2010_Width = 24;
            int OFFICE2010_SIZE = 22;
			Image image = null;
			#endregion

			#region Constructors
			public HelpSystemButton( RibbonControlAdvHeader parent, int sysCommand )
				: base( parent, sysCommand )
			{
				this.Margin = new Padding( 0 );
				this.Padding = new Padding( 0 );
                if (parent.RibbonStyle == RibbonStyle.Office2013)
                    image = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.Help_125.png"));
                else if (parent.RibbonStyle == RibbonStyle.Office2010)
                    image = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.HelpButtonOffice2010.bmp"));
                else
                    image = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.HelpButton.bmp"));
                (image as Bitmap).MakeTransparent(Color.Magenta);
            }
			#endregion

			#region Overrides

			public override Image Image
			{
				get
				{
                    Bitmap bit = new Bitmap(10, 10);
                    using (Graphics g = Graphics.FromImage(bit))
                    {
                        bit.Dispose();
                        if (g.DpiX > 96)
                        {
                            if ((this.Parent as RibbonControlAdvHeader).RibbonStyle == RibbonStyle.Office2007)
                                return base.Image;
                            else if ((this.Parent as RibbonControlAdvHeader).RibbonStyle == RibbonStyle.Office2013)
                                return image = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.Help_125.png"));
                            else
                                return image;
                        }
                        else
                        {
                            if ((this.Parent as RibbonControlAdvHeader).RibbonStyle == RibbonStyle.Office2007)
                                return base.Image;
                            else if ((this.Parent as RibbonControlAdvHeader).RibbonStyle == RibbonStyle.Office2013)
                                return image = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.Help.png"));
                            else
                                return image;
                        }
                    }
				}
				set
				{
					if ((this.Parent as RibbonControlAdvHeader).RibbonStyle == RibbonStyle.Office2007)
						base.Image = value;
					else 
						image = value;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="constrainingSize"></param>
			/// <returns></returns>
			public override Size GetPreferredSize( Size constrainingSize )
			{
                Bitmap bit = new Bitmap(10, 10);
                using (Graphics g = Graphics.FromImage(bit))
                {
                    if (g.DpiX > 120 || (this.Parent as RibbonControlAdvHeader).ribbonTouchModeEnabled)
                    {
                        HELP_IMAGE_SIZE = 30;
                        OFFICE2010_SIZE = 34;
                    }
                    else if (g.DpiX > 96)
                    {
                        HELP_IMAGE_SIZE = 30;
                        OFFICE2010_SIZE = 26;
                    }
                    if ((Parent as RibbonControlAdvHeader).RibbonStyle == RibbonStyle.Office2007)
                        return new Size(HELP_IMAGE_SIZE, HELP_IMAGE_SIZE);
                    else
                        return new Size(OFFICE2010_Width, OFFICE2010_Height);
                }
			}
			#endregion
		}
		#endregion

		#region *** MinimizeButton
		class MinimizeSystemButton : ToolStripButton, ICustomItem
		{
			#region Fields
			const int DEFAULT_SIZE = 22;
            const int DPI_125_DEFAULT_SIZE = 26;
            const int DPI_150_DEFAULT_SIZE = 32;
			RibbonControlAdvHeader header;
			ImageList imageList = null;
			const int DownArrowID = 0;
			const int UpArrowID = 1;
			#endregion

			#region Ctor
			public MinimizeSystemButton(RibbonControlAdvHeader header)
				: base()
			{
				imageList = new ImageList();
				
				Bitmap bmp;
                using (Graphics g = Graphics.FromImage(new Bitmap(10, 10)))
                {
                    if (g.DpiX > 120 || header.RibbonTouchModeEnabled )
                    {
                        bmp = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.DPI_150_MinimizeButton.bmp"));
                        bmp.MakeTransparent(Color.Magenta);
                        imageList.ImageSize = new Size(25, 25);
                        imageList.ColorDepth = ColorDepth.Depth32Bit;
                        imageList.Images.AddStrip(bmp);
                    }
                    else if (g.DpiX > 96)
                    {
                        bmp = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.DPI_125_MinimizeButton.bmp"));
                        bmp.MakeTransparent(Color.Magenta);
                        imageList.ImageSize = new Size(20, 20);
                        imageList.ColorDepth = ColorDepth.Depth32Bit;
                        imageList.Images.AddStrip(bmp);
                    }
                    else
                    {
                        bmp = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.MinimizeButton.bmp"));
                        bmp.MakeTransparent(Color.Magenta);
                        imageList.ImageSize = new Size(16, 16);
                        imageList.ColorDepth = ColorDepth.Depth32Bit;
                        imageList.Images.AddStrip(bmp);
                    }
                }
				this.header = header;
				this.Margin = new Padding(0);
				this.Padding = new Padding(0);
				this.Parent = header;
				this.AutoToolTip = false;
				this.ImageScaling = ToolStripItemImageScaling.None;
			}
			#endregion

			#region Overrides

			/// <summary>
			/// Gets or Sets coordinates of the upper-left corner
			/// </summary>
			public Point Location
			{
				get
				{
					return this.Bounds.Location;
				}
				set
				{
					SetBounds(new Rectangle(value, this.Size));
				}
			}

			public override Image Image
			{
				   get
                {
                    if (this.header.Minimizeimage && this.header.BackStageView != null && !this.header.BackStageView.IsVisible)
                    {
                        if (this.header != null && this.header.m_owner.MinimizePanel)
                       {
                            this.header.Minimizeimageindex = 0;
                            return imageList.Images[DownArrowID];
                        }
                        else
                        {
                            this.header.Minimizeimageindex = 1;
                            return imageList.Images[UpArrowID];
                       }
                    }
                    else if(this.header.RibbonStyle == RibbonStyle.Office2007)
                    {
                        if (this.header != null && this.header.m_owner.MinimizePanel)
                        {
                            this.header.Minimizeimageindex = 0;
                            return imageList.Images[DownArrowID];
                        }
                        else
                        {
                            this.header.Minimizeimageindex = 1;
                            return imageList.Images[UpArrowID];
                        }
                    }
                    return imageList.Images[this.header.Minimizeimageindex];
              }
				set{ }
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				if (header != null)
				{
					header.Renderer.DrawButtonBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));

					if (this.Image != null)
					{
						Point loc = new Point((this.Width - this.Image.Width) / 2, (this.Height - this.Image.Height) / 2);
						Rectangle rc = new Rectangle(loc, this.Image.Size);
						header.Renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(e.Graphics, this, rc));
					}
				}
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					this.imageList.Images.Clear();
					this.imageList = null;
				}

				base.Dispose(disposing);
			}

			public override Size GetPreferredSize(Size constrainingSize)
			{
                Bitmap bit = new Bitmap(10, 10);
                using (Graphics g = Graphics.FromImage(bit))
                {
                    if (g.DpiX > 120 || header.ribbonTouchModeEnabled)
                    {
                        bit.Dispose();
                        return new Size(DPI_150_DEFAULT_SIZE, DPI_150_DEFAULT_SIZE);
                    }
                    else if (g.DpiX > 96)
                    {
                        bit.Dispose();
                        return new Size(DPI_125_DEFAULT_SIZE, DPI_125_DEFAULT_SIZE);
                    }
                    else
                    {
                        bit.Dispose();
                        return new Size(DEFAULT_SIZE, DEFAULT_SIZE);
                    }
                }
			}
			#endregion

			#region ICustomItem Members

			ToolStrip ICustomItem.Owner
			{
				get { return header; }
			}

			ToolStripItemPlacement ICustomItem.Placement
			{
				get { return header.DisplayedItems.Contains(this) ? ToolStripItemPlacement.Main : ToolStripItemPlacement.None; }
			}

			#endregion
		}
		#endregion

        #region *** Ribbon2013MinimizeSystemButton
        class Ribbon2013MinimizeSystemButton : ToolStripButton, ICustomItem
        {
            #region Fields
            const int DEFAULT_SIZE = 22;
            const int DEFAULT_HIGHT = 19;
            const int DPI_125_DEFAULT_SIZE = 26;
            const int DPI_150_DEFAULT_SIZE = 32;
            RibbonControlAdvHeader header;
            ImageList imageList = null;
            const int DownArrowID = 0;
            const int UpArrowID = 1;
            #endregion

            #region Ctor
            public Ribbon2013MinimizeSystemButton(RibbonControlAdvHeader header)
                : base()
            {
                imageList = new ImageList();

                Bitmap bmp;
                using (Graphics g = Graphics.FromImage(new Bitmap(10, 10)))
                {
                    if (g.DpiX > 120)
                    {
                        bmp = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.150arrow.png"));
                        bmp.MakeTransparent(Color.Magenta);
                        imageList.ImageSize = new Size(32, 32);
                        imageList.ColorDepth = ColorDepth.Depth32Bit;
                        imageList.Images.AddStrip(bmp);
                        bmp = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.150Disarrow.png"));
                        bmp.MakeTransparent(Color.Magenta);
                        imageList.ImageSize = new Size(32,32);
                        imageList.ColorDepth = ColorDepth.Depth32Bit;
                        imageList.Images.Add(bmp);
                    }
                    else if (g.DpiX > 96 || header.ribbonTouchModeEnabled)
                    {
                        bmp = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.arrow_125.png"));
                        bmp.MakeTransparent(Color.Magenta);
                        imageList.ImageSize = new Size(28, 28);
                        imageList.ColorDepth = ColorDepth.Depth32Bit;
                        imageList.Images.AddStrip(bmp);
                        bmp = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.Disarrow_125.png"));
                        bmp.MakeTransparent(Color.Magenta);
                        imageList.ImageSize = new Size(28, 28);
                        imageList.ColorDepth = ColorDepth.Depth32Bit;
                        imageList.Images.Add(bmp);
                       
                    }
                    else
                    {
                        bmp = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.arrow.png"));
                        bmp.MakeTransparent(Color.Magenta);
                        imageList.ImageSize = new Size(24, 24);
                        imageList.ColorDepth = ColorDepth.Depth32Bit;
                        imageList.Images.AddStrip(bmp);
                        bmp = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.Disarrow.png"));
                        bmp.MakeTransparent(Color.Magenta);
                        imageList.ImageSize = new Size(24, 24);
                        imageList.ColorDepth = ColorDepth.Depth32Bit;
                        imageList.Images.Add(bmp);
                    }
                }
                image = imageList.Images[0];
                this.header = header;
                this.Margin = new Padding(0);
                this.Padding = new Padding(0);
                this.Parent = header;
                this.AutoToolTip = false;
                this.ImageScaling = ToolStripItemImageScaling.None;
            }
            #endregion
    
            #region Overrides

            /// <summary>
            /// Gets or Sets coordinates of the upper-left corner
            /// </summary>
            public Point Location
            {
                get
                {
                    return this.Bounds.Location;
                }
                set
                {
                    SetBounds(new Rectangle(value, this.Size));
                }
            }
            Image image = null;
            public override Image Image
            {
                get
                {

                    return image;
                }
                set
                {
                    image = value;
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (this.Image != null)
                {
                    header.Renderer.DrawButtonBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
                    if (header.Form!=null && !header.Form.ContainsFocus && !(header.FindForm() as RibbonForm).designmode)
                    {
                        if (this.Image != imageList.Images[1])
                        {
                            this.Image = imageList.Images[1];
                        }
                    }
                    else
                    {
                        if (this.Image != imageList.Images[0])
                        {
                            this.Image = imageList.Images[0];
                        }
                    }
                    Point loc = new Point((this.Width - this.Image.Width) / 2, (this.Height - this.Image.Height) / 2);
                    Rectangle rc = new Rectangle(loc, this.Image.Size);
                    header.Renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(e.Graphics, this, rc));

                }

            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.imageList.Images.Clear();
                    this.imageList = null;
                }

                base.Dispose(disposing);
            }

            public override Size GetPreferredSize(Size constrainingSize)
            {
                Bitmap bit = new Bitmap(10, 10);
                using (Graphics g = Graphics.FromImage(bit))
                {
                    if (g.DpiX > 120)
                    {
                        bit.Dispose();
                        return new Size(DPI_150_DEFAULT_SIZE, DPI_150_DEFAULT_SIZE);
                    }
                    else if (g.DpiX > 96 || header.ribbonTouchModeEnabled)
                    {
                        bit.Dispose();
                        return new Size(30, DPI_125_DEFAULT_SIZE);
                    }
                    else
                    {
                        bit.Dispose();
                        return new Size(DEFAULT_SIZE + 2, DEFAULT_HIGHT);
                    }
                }
            }
            #endregion

            #region ICustomItem Members

            ToolStrip ICustomItem.Owner
            {
                get { return header; }
            }

            ToolStripItemPlacement ICustomItem.Placement
            {
                get { return header.DisplayedItems.Contains(this) ? ToolStripItemPlacement.Main : ToolStripItemPlacement.None; }
            }

            #endregion
        }
        #endregion

		#region *** SystemMenu
		class SystemMenuStrip: ContextMenuStrip
		{
			#region Constants
			const int MIIM_STATE = 0x00000001;
			const int MIIM_ID = 0x00000002;
			const int MIIM_SUBMENU = 0x00000004;
			const int MIIM_CHECKMARKS = 0x00000008;
			const int MIIM_TYPE = 0x00000010;
			const int MIIM_DATA = 0x00000020;
			const int MIIM_STRING = 0x00000040;
			const int MIIM_BITMAP = 0x00000080;
			const int MIIM_FTYPE = 0x00000100;

			const int MFT_STRING = 0x00000000;
			const int MFT_BITMAP = 0x00000004;
			const int MFT_SEPARATOR = 0x00000800;

			const int MF_BYCOMMAND = 0x00000000;
			const int MF_BYPOSITION = 0x00000400;

			const int MFS_DISABLED = 0x00000003; /*MFS_DISABLED|MFS_GRAYED*/

			const int HBMMENU_POPUP_CLOSE = 8;
			const int HBMMENU_POPUP_RESTORE = 9;
			const int HBMMENU_POPUP_MAXIMIZE = 10;
			const int HBMMENU_POPUP_MINIMIZE = 11;

			#endregion

			#region Structs
			[StructLayout( LayoutKind.Sequential )]
			struct MENUITEMINFO
			{
				public int cbSize;
				public int fMask;
				public int fType;
				public int fState;
				public int wID;
				public IntPtr hSubMenu;
				public IntPtr hbmpChecked;
				public IntPtr hbmpUnchecked;
				public IntPtr dwItemData;
				public IntPtr dwTypeData;
				public int cch;
				public IntPtr hbmpItem;
			}
			#endregion

			#region Constructors

			public SystemMenuStrip( Form form )
			{
				m_form = form;
			}
			#endregion

			#region Methods
			public bool UpdateItems( ToolStripRenderer renderer )
			{
				bool bResult = false;

				if( m_form != null && m_form.IsHandleCreated )
				{
					IntPtr hMenu = GetSystemMenu( m_form.Handle, false );
					if( hMenu != IntPtr.Zero )
					{
						this.Items.Clear();

						for( int i = 0, count = GetMenuItemCount( hMenu ); i < count; i++ )
						{
							MENUITEMINFO mii = new MENUITEMINFO();
							mii.cbSize = Marshal.SizeOf( mii );
							mii.fMask = MIIM_TYPE | MIIM_STATE | MIIM_ID;

							if( GetMenuItemInfo( hMenu, i, true, ref mii ) )
							{
								if( ( mii.fType & MFT_SEPARATOR ) == MFT_SEPARATOR )
								{
									this.Items.Add( new ToolStripSeparator() );
								}
								else
								{
									ToolStripMenuItem item = new ToolStripMenuItem();
									if( mii.cch > 0 )
									{
										StringBuilder lpString = new StringBuilder( mii.cch + 1 );
										if( GetMenuString( hMenu, i, lpString, mii.cch + 1, MF_BYPOSITION ) > 0 )
										{
											string sText = lpString.ToString();
											int iShortcut = sText.IndexOf( '\t' );

											if( iShortcut != -1 )
											{
												item.ShortcutKeyDisplayString = sText.Substring( iShortcut + 1 );
												sText = sText.Substring( 0, iShortcut );
											}
											item.Text = sText;
										}
									}

									item.Tag = mii.wID;

									switch( mii.wID & 0xFFF0 )
									{
										case 0xF000: //SC_SIZE
										case 0xF010: //SC_MOVE
										item.Enabled = !IsWindowMaximized( m_form.Handle );
										break;
										default:
										item.Enabled = ( mii.fState & MFS_DISABLED ) == 0;
										break;
									}

									item.Image = GetImage( ref mii );
									item.ImageScaling = ToolStripItemImageScaling.None;

									this.Items.Add( item );
								}
							}
						}
						this.Renderer = renderer;
						bResult = true;
					}
				}

				return bResult;
			}
			#endregion

			#region Overrides
			/// <summary>
			/// 
			/// </summary>
			/// <param name="disposing"></param>
			protected override void Dispose( bool disposing )
			{
				this.Items.Clear();
				base.Dispose( disposing );
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnItemAdded( ToolStripItemEventArgs e )
			{
				base.OnItemAdded( e );
				e.Item.Click += new EventHandler( OnClick );
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="e"></param>
			protected override void OnItemRemoved( ToolStripItemEventArgs e )
			{
				base.OnItemRemoved( e );
				e.Item.Click -= new EventHandler( OnClick );
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="m"></param>
			protected override void WndProc( ref Message m )
			{
				switch( (Msg)m.Msg )
				{
					case Msg.WM_SYSCOMMAND:
					switch( (int)m.WParam & 0xfff0 )
					{
						case (int)SystemCommand.SC_KEYMENU:
						m.Result = IntPtr.Zero;
						return;
					}
					break;
				}
				base.WndProc( ref m );
			}
			#endregion

			#region EventHandlers
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sender"></param>
			/// <param name="e"></param>
			void OnClick( object sender, EventArgs e )
			{
				ToolStripMenuItem item = sender as ToolStripMenuItem;
				if( item != null )
				{
					Point p = Cursor.Position;
					int lParam = WindowsAPI.MAKELONG( p.X, p.Y );
					WindowsAPI.PostMessage( m_form.Handle, (int)Msg.WM_SYSCOMMAND, (int)item.Tag, lParam );
				}
			}
			#endregion

			#region Implementation

			private Image GetImage( ref MENUITEMINFO mii )
			{
				Image image = null;

				if( ( mii.fType & MFT_BITMAP ) != 0 )
				{
					int bmp = WindowsAPI.LOW_ORDER( (int)mii.dwTypeData );
					switch( bmp )
					{
						case HBMMENU_POPUP_CLOSE:
						image = SystemImages.GetImageClose( SystemColors.ControlText,false );
						break;
						case HBMMENU_POPUP_RESTORE:
						image = SystemImages.GetImageRestore( SystemColors.ControlText ,false);
						break;
						case HBMMENU_POPUP_MAXIMIZE:
						image = SystemImages.GetImageMaximize( SystemColors.ControlText ,false);
						break;
						case HBMMENU_POPUP_MINIMIZE:
						image = SystemImages.GetImageMinimize( SystemColors.ControlText ,false);
						break;
						default:
						if( GetObjectType( (IntPtr)bmp ) != 0 )
						{
							image = Bitmap.FromHbitmap( (IntPtr)bmp );
						}
						break;
					}
				}

				return image;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="hWnd"></param>
			/// <returns></returns>
			static bool IsWindowMaximized( IntPtr hWnd )
			{
				WindowStyles style = (WindowStyles)WindowsAPI.GetWindowLong( hWnd, (int)SetWindowLongOffsets.GWL_STYLE );

				return ( style & WindowStyles.WS_MAXIMIZE ) != 0;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="hWnd"></param>
			/// <param name="bRevert"></param>
			/// <returns></returns>
			[DllImport( "User32.dll", CharSet=CharSet.Auto )]
			static extern IntPtr GetSystemMenu( IntPtr hWnd, bool bRevert );
			/// <summary>
			/// 
			/// </summary>
			/// <param name="hMenu"></param>
			/// <returns></returns>
			[DllImport( "User32.dll", CharSet=CharSet.Auto )]
			static extern int GetMenuItemCount( IntPtr hMenu );
			/// <summary>
			/// 
			/// </summary>
			/// <param name="hMenu"></param>
			/// <param name="uItem"></param>
			/// <param name="fByPosition"></param>
			/// <param name="mii"></param>
			/// <returns></returns>
			[DllImport( "User32.dll", CharSet=CharSet.Auto )]
			static extern bool GetMenuItemInfo( IntPtr hMenu, int uItem, bool fByPosition, ref MENUITEMINFO mii );

			[DllImport( "User32.dll", CharSet=CharSet.Auto )]
			static extern int GetMenuString( IntPtr hMenu, int item, StringBuilder lpString, int nMaxCount, int flag );

			[DllImport( "gdi32.dll" )]
			static extern int GetObjectType( IntPtr h );


			[DllImport( "gdi32.dll" )]
			static extern bool GetBitmapDimensionEx( IntPtr hBitmap, ref WIN32SIZE size );
			#endregion

			#region Fields
			Form m_form;
			#endregion
		}
		#endregion

		#region *** SystemToolTipInfo
		class SystemToolTipInfo: ToolTipInfo
		{
			public SystemToolTipInfo()
			{
				this.Separator = false;
				this.Header.Hidden = true;
				this.Footer.Hidden = true;
				this.Body.TextAlign = ContentAlignment.MiddleCenter;
			}
		}
		#endregion

		#region *** ToolStripItemCollectionFiltered
		/// <summary>
		/// Collection for filtering invisible items in ToolStripIOtemCollection.
		/// </summary>
		internal class ToolStripItemCollectionFiltered: ToolStripItemCollection
		{
			#region Fields
			/// <summary>
			/// Underlying IItemHidable control.
			/// </summary>
			private ILayoutSupport m_hidable;
			/// <summary>
			/// Underlying ToolStripItemCollection.
			/// </summary>
			private ToolStripItemCollection m_baseCollection;
			#endregion

			#region Constructors/Destructor
			/// <summary>
			/// Creates & initializes new instance of ToolStripItemCollectionFiltered.
			/// </summary>
			/// <param name="hidable">Underlying IItemHidable control.</param>
			/// <param name="baseCollection">Underlying ToolStripItemCollection.</param>
			public ToolStripItemCollectionFiltered( ToolStrip hidable, ToolStripItemCollection baseCollection )
				: base( hidable, new ToolStripItem[] { } )
			{
				m_hidable = hidable as ILayoutSupport;

				if( m_hidable == null )
					throw new ArgumentException( "hidable" );

				m_baseCollection = baseCollection;
			}
			#endregion

			#region Methods
			/// <summary>
			/// 
			/// </summary>
			public void Dispose()
			{
				m_hidable = null;
				m_baseCollection = null;
			}
			#endregion

			#region Overrides
			/// <summary>
			/// Performs filtering.
			/// </summary>
			/// <param name="index"></param>
			/// <returns></returns>
			public override ToolStripItem this[int index]
			{
				get
				{
					ToolStripItem item = m_baseCollection[index];
					return m_hidable.GetItem( item );
				}
			}
			/// <summary>
			/// Returns count of underlying collection.
			/// </summary>
			public override int Count
			{
				get
				{
					return m_baseCollection.Count;
				}
			}
			#endregion

		}
		#endregion

		#region *** MainItemsCollection
		/// <summary>
		/// 
		/// </summary>
		class MainItemsCollection: RibbonItemsCollection
		{
			#region Overrides
			/// <summary>
			/// 
			/// </summary>
			/// <param name="index"></param>
			/// <param name="value"></param>
			/// <returns></returns>
			protected override bool OnRemove( int index, object value )
			{
				bool bResult = base.OnRemove( index, value );

				if( bResult )
				{
					ToolStripTabItem tabItem = value as ToolStripTabItem;

					if( tabItem != null )
					{
						tabItem.Checked = false;
					}
				}

				return bResult;
			}
			#endregion
		}
		#endregion

		#endregion

		#region Constructors
		/// <summary>
		/// 
		/// </summary>
		static RibbonControlAdvHeader()
		{
			smSystemButtons = new Padding( 2, 2, 2, 0 );
			smQuickItems = new Padding( 0, 0, 0, 1 );
		}
		/// <summary>
		/// Creates & initializes new instance of RibbonControlAdvHeader.
		/// </summary>
		protected internal RibbonControlAdvHeader( RibbonControlAdv owner )
		{
			m_owner = owner;

			m_fakeHiddenItem = new ToolStripButton();
			m_fakeHiddenItem.Available = false;

			m_quickItems = new QuickItemsCollection( this );
			m_quickItems.ItemAdded += new EventHandler<ListItemEventArgs<ToolStripItem>>( OnQuickItemsItemAdded );
			m_quickItems.ItemRemoved += new EventHandler<ListItemEventArgs<ToolStripItem>>( OnRibbonItemRemoved );

			m_mainItems = new MainItemsCollection();
            visibleTabItems = new MainItemsCollection();
			m_mainItems.ItemAdded += new EventHandler<ListItemEventArgs<ToolStripItem>>( OnMainItemsItemAdded );
			m_mainItems.ItemRemoved += new EventHandler<ListItemEventArgs<ToolStripItem>>( OnRibbonItemRemoved );

			m_groups = new TabGroupCollection( this );
			m_groups.ItemRemoved += new EventHandler<ListItemEventArgs<ToolStripTabGroup>>( OnGroupRemoved );
			m_groups.ItemAdded += new EventHandler<ListItemEventArgs<ToolStripTabGroup>>( OnGroupAdded );
			m_hashGroups = new Dictionary<ToolStripItem, ToolStripTabGroup>();

			m_menuButton = new ToolStripMenuButton( this );
            PopUpItemsInitialize();
            TouchPopUpInitialize();
            m_menuButton.AutoSize = false;
            m_menuButton.DoubleClickEnabled = true;
            m_menuButton.ImageScaling = ToolStripItemImageScaling.SizeToFit;
            m_timer = new Timer();
            m_timer.Tick += new EventHandler(OnTimerTick);
            m_menuButton.Click += new EventHandler(m_menuButton_Click);
            this.Items.Add(m_menuButton);
            this.imageButton1.Click += new EventHandler(imageButton1_Click);
            imageButton1.BackgroundImageLayout = ImageLayout.Stretch;
            imageButton1.HoverImage = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.backh.png"));
            imageButton1.NormalImage = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.BackStage1.png"));
            imageButton1.Visible = false;
            imageButton1.Location = new Point(14, 6);
            imageButton1.Size = new System.Drawing.Size(100, 80);
            this.Items.Add(new ToolStripControlHost(imageButton1));
            this.MinimizeButton.Click += new EventHandler(OnMinimizeRibbonPanel);
            base.DefaultDropDownDirection = ToolStripDropDownDirection.BelowRight;
            base.Font = Control.DefaultFont;
            popupControlContainer1 = new RibbonMinimizePopUp();
            TouchModePop = new RibbonMinimizePopUp();
            this.popupControlContainer1.BorderStyle = BorderStyle.FixedSingle;
            this.popupControlContainer1.BackColor = Color.White;
            this.popupControlContainer1.Size = new Size(254, 184);
            this.popupControlContainer1.Visible = false;
            this.popupControlContainer1.BeforePopup += new CancelEventHandler(popupControlContainer1_BeforePopup);
            this.popupControlContainer1.BeforeCloseUp += new CancelEventHandler(popupControlContainer1_BeforeCloseUp);
            this.popupControlContainer1.Controls.Add(ribbonDropDownContainer1);
            ribbonDropDownContainer1.Location = new Point(2, 2);
            this.TouchModePop.BorderStyle = BorderStyle.FixedSingle;
            this.TouchModePop.BackColor = Color.White;
            this.TouchModePop.Size = new Size(254, 154);
            this.TouchModePop.Visible = false;
            this.TouchModePop.Controls.Add(TouchDropDown);
            TouchModePop.Location = new Point(2, 2);
            m_bReadyForLayout = true;
            this.Ribbon2013MinimizeButton.Click += new EventHandler(Ribbon2013MinimizeButton_Click);
            setRibbonToPopup();
        }

        void popupControlContainer1_BeforeCloseUp(object sender, CancelEventArgs e)
        {
            this.ribbonOptionDropDownSelected = false;
            this.Refresh();
        }

        void popupControlContainer1_BeforePopup(object sender, CancelEventArgs e)
        {
            this.ribbonOptionDropDownSelected = true;
            this.Refresh();
        }
        private void setRibbonToPopup()
        {
        }
        void Form_LostFocus(object sender, EventArgs e)
        {
            if (this.popupControlContainer1.IsShowing())
                this.popupControlContainer1.HidePopup();
            if (this.TouchModePop.IsShowing())
                this.TouchModePop.HidePopup();
            dropDownSelected = false;
        }

        void Ribbon2013MinimizeButton_Click(object sender, EventArgs e)
        {
            if (this.m_owner != null)
            {
                if (this.m_owner.MinimizePanel && !this.m_owner.HeaderInternal.AutoHide)
                {
                    controlItem2.StatusCheck = true;
                    controlItem1.StatusCheck = false;
                    controlItem3.StatusCheck = false;
                }
                else if (this.m_owner.HeaderInternal.AutoHide)
                {
                    controlItem1.StatusCheck = true;
                    controlItem2.StatusCheck = false;
                    controlItem3.StatusCheck = false;
                }
                else
                {
                    controlItem1.StatusCheck = false;
                    controlItem2.StatusCheck = false;
                    controlItem3.StatusCheck = true;
                }
            }
            this.ribbonDropDownContainer1.ColorScheme = m_owner.Office2013ColorScheme;
            int yLocation = (this.Parent as RibbonControlAdv).Bounds.Y +(this.Parent as RibbonControlAdv).HeaderInternal.Ribbon2013MinimizeButton.Bounds.Y + (this.Parent as RibbonControlAdv).HeaderInternal.Ribbon2013MinimizeButton.Bounds.Height;
            if (!this.popupControlContainer1.IsShowing())
                this.popupControlContainer1.ShowPopup(new Point((this.Parent.Parent as RibbonForm).Location.X + (this.Parent as RibbonControlAdv).HeaderInternal.Ribbon2013MinimizeButton.Bounds.X, this.Parent.Parent.Location.Y + yLocation));
            else
                this.popupControlContainer1.HidePopup();
        }

        void imageButton1_Click(object sender, EventArgs e)
        {
            NativeMethods.LockWindowUpdate(this.Handle);
           
            this.backStageView.IsVisible = false;
            m_owner.SelectedTab = this.BackStageView.SeletedTabItem;
            this.MenuButton.PerformClick();
            
            if (this.BackStageView != null)
                this.ShowItemToolTips = !this.BackStageView.IsVisible && !this.HideMenuButtonToolTip;
            NativeMethods.LockWindowUpdate(IntPtr.Zero);
        }
        internal ImageButton imageButton1 = new ImageButton();

        internal bool AutoHide = false;
        internal void SetRibbon2013OptionValues(ControlItems ControlItem ,string ItemHeaderText , string ItemMainText , Font ItemHeaderFont , Font ItemMainFont )
        {
            if (ControlItem == ControlItems.AutoHide)
            {
                controlItem1.HeaderText = ItemHeaderText;
                controlItem1.SubText = ItemMainText;
                this.controlItem1.HeaderTextFont = ItemHeaderFont;
                controlItem1.SubTextFont = ItemMainFont;
            }
            else if (ControlItem == ControlItems.RibbonMinimizePanel)
            {
                controlItem2.HeaderText = ItemHeaderText;
                controlItem2.SubText = ItemMainText;
                controlItem2.HeaderTextFont = ItemHeaderFont;
                controlItem2.SubTextFont = ItemMainFont;
            }
            else if(ControlItem == ControlItems.RibbonPanel)
            {
                controlItem3.HeaderText = ItemHeaderText;
                controlItem3.SubText = ItemMainText;
                controlItem3.HeaderTextFont = ItemHeaderFont;
                controlItem3.SubTextFont = ItemMainFont;
            }
        }
        Label touchlabel = null;
        private Double labelFontSize = 9F;
         private void TouchPopUpInitialize()
        {
            touchlabel = new Label();
            this.TouchModeItem = new ControlItem(); 
            this.MouseModeItem = new ControlItem();
            this.LabelItem = new ControlItem();
            TouchDropDown = new RibbonDropDownContainer();
            touchlabel.Text = "Optimize spacing between commands";
            touchlabel.TextAlign = ContentAlignment.MiddleCenter;
            Font DummyFont = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            using (Graphics g = this.CreateGraphics())
            {
                if (g.DpiX > 96)
                {
                    labelFontSize = DummyFont.Size / (g.DpiX / 96);
                }
            }
            Font labelFont = new Font(DummyFont.OriginalFontName, (float)labelFontSize, DummyFont.Style);
            touchlabel.Font = labelFont;
            touchlabel.ForeColor = ColorTranslator.FromHtml("#777777");
            touchlabel.Location = new Point(0, 0);
            touchlabel.BackColor = ColorTranslator.FromHtml("#eeeeee");
            touchlabel.BorderStyle = BorderStyle.None;
            touchlabel.Size = new Size(262, 30);
            this.TouchDropDown.Controls.Add(touchlabel);
            TouchDropDown.Location = new Point(1, 1);
            TouchDropDown.Name = "ribbonDropDownContainer1";
            TouchDropDown.RibbonOptionDropDownItems.Add(this.MouseModeItem);
            TouchDropDown.RibbonOptionDropDownItems.Add(this.TouchModeItem);
            TouchDropDown.Size = new System.Drawing.Size(252, 152);
            TouchDropDown.TabIndex = 0;
            TouchModeItem.BackColor = System.Drawing.Color.White;
            TouchModeItem.HeaderText = "Touch";
            TouchModeItem.HeaderTextFont = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            TouchModeItem.ItemImage = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.touchimage.png"));
            TouchModeItem.Location = new System.Drawing.Point(1, 40);
            TouchModeItem.Name = "TouchModeItem";
            TouchModeItem.Size = new System.Drawing.Size(252, 61);
            TouchModeItem.StatusCheck = false;
            TouchModeItem.SubText = "More space between commands. Optimized for use with touch."; 
            TouchModeItem.SubTextFont = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            TouchModeItem.TabIndex = 0;
            TouchModeItem.Text = "TouchModeItem";
            // 
            // MouseModeItem
            // 
            MouseModeItem.BackColor = System.Drawing.Color.White;
            MouseModeItem.HeaderText = "Mouse";
            MouseModeItem.HeaderTextFont = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            MouseModeItem.ItemImage = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.mouseimage.png")); //tocuhdropdown.png
            MouseModeItem.Location = new System.Drawing.Point(1, 120);
            MouseModeItem.Name = "MouseModeItem";
            MouseModeItem.Size = new System.Drawing.Size(252, 61);
            MouseModeItem.StatusCheck = true;
            MouseModeItem.SubText = "Standard ribbon and commands. Optimized for use with mouse.";
            MouseModeItem.SubTextFont = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            MouseModeItem.TabIndex = 0;
            MouseModeItem.Text = "MouseModeItem";
            foreach (ControlItem DropDownItem in this.TouchDropDown.RibbonOptionDropDownItems)
            {
                DropDownItem.MouseUp += new MouseEventHandler(DropDownItem_MouseUp);
            }
        }
         private bool ribbonTouchModeEnabled = false;
         internal bool RibbonTouchModeEnabled
         {
             get
             {
                 return ribbonTouchModeEnabled;
             }
             set
             {
                 ribbonTouchModeEnabled = value;
             }
         }
         void DropDownItem_MouseUp(object sender, MouseEventArgs e)
         {
             if ((sender as ControlItem).Name == "TouchModeItem")
             {
                 if (!m_owner.RibbonTouchModeEnabled)
                 {
                     this.TouchModePop.HidePopup();
                     dropDownSelected = false;
                 }
                 m_owner.RibbonTouchModeEnabled = true;
                 m_owner.BottomToolstrip.PerformLayout();
                 m_owner.PerformLayout();
                 m_owner.Refresh();
                 m_owner.UpdateRenderers(true);
               
             }
             else if ((sender as ControlItem).Name == "MouseModeItem")
             {
                 if (m_owner.RibbonTouchModeEnabled)
                 {
                     this.TouchModePop.HidePopup();
                     dropDownSelected = false;
                 }
                 m_owner.RibbonTouchModeEnabled = false;
                 m_owner.PerformLayout();
                 m_owner.BottomToolstrip.PerformLayout();
                 m_owner.UpdateRenderers(true);
                 m_owner.Refresh();
             }

         }
        private void PopUpItemsInitialize()
        {
            this.controlItem1 = new ControlItem(); 
            this.controlItem2 = new ControlItem();
            this.controlItem3 = new ControlItem();
            ribbonDropDownContainer1 = new RibbonDropDownContainer();
            this.ribbonDropDownContainer1.Controls.Add(this.controlItem1);
            this.ribbonDropDownContainer1.Controls.Add(this.controlItem2);
            this.ribbonDropDownContainer1.Controls.Add(this.controlItem3);
            ribbonDropDownContainer1.Location = new Point(0, 0);
            this.ribbonDropDownContainer1.Name = "ribbonDropDownContainer1";
            this.ribbonDropDownContainer1.RibbonOptionDropDownItems.Add(this.controlItem1);
            this.ribbonDropDownContainer1.RibbonOptionDropDownItems.Add(this.controlItem2);
            this.ribbonDropDownContainer1.RibbonOptionDropDownItems.Add(this.controlItem3);
            this.ribbonDropDownContainer1.Size = new System.Drawing.Size(250, 180);
            this.ribbonDropDownContainer1.TabIndex = 0; this.controlItem1.BackColor = System.Drawing.Color.White;
            this.controlItem1.HeaderText = "Auto-Hide Ribbon";
            this.controlItem1.HeaderTextFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.controlItem1.ItemImage = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.RibbonOption1.png"));
            this.controlItem1.Location = new System.Drawing.Point(1, 1);
            controlItem1.Name = "controlitem1";
            controlItem1.Size = new System.Drawing.Size(252, 62);
            controlItem1.StatusCheck = false;
            controlItem1.SubText = "Hide the Ribbon. Click at the top of the application to show it";
            controlItem1.SubTextFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            controlItem1.TabIndex = 0;
            controlItem1.Text = "controlItem1";
            // 
            // controlItem2
            // 
            controlItem2.BackColor = System.Drawing.Color.White;
            controlItem2.HeaderText = "Show Tabs";
            controlItem2.HeaderTextFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.controlItem2.ItemImage = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.RibbonOption2.png"));
            controlItem2.Location = new System.Drawing.Point(1, 101);
            controlItem2.Name = "controlItem2";
            controlItem2.Size = new System.Drawing.Size(250, 60);
            controlItem2.StatusCheck = false;
            controlItem2.SubText = "Show Ribbon tabs only.Click a tabs to show the commands";
            controlItem2.SubTextFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            controlItem2.TabIndex = 0;
            controlItem2.Text = "controlItem2";
            // 
            // controlItem3
            // 
            controlItem3.BackColor = System.Drawing.Color.White;
            controlItem3.HeaderText = "Show Tabs and Commands";
            controlItem3.HeaderTextFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.controlItem3.ItemImage = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.RibbonOption3.png"));
            controlItem3.Location = new System.Drawing.Point(1, 201);
            controlItem3.Name = "controlItem3";
            controlItem3.Size = new System.Drawing.Size(250, 60);
            controlItem3.StatusCheck = false;
            controlItem3.SubText = "Show Ribbon tabs and Commands all the time";
            controlItem3.SubTextFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            controlItem3.TabIndex = 0;
            controlItem3.Text = "controlItem3";
            foreach (ControlItem ctrl in this.ribbonDropDownContainer1.RibbonOptionDropDownItems)
            {
                ctrl.MouseUp += new MouseEventHandler(ctrl_MouseUp);
            }
        }
        /// <summary>
        /// List of objects storing info about top VisibleTabItem.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public ObservableList<ToolStripItem> VisibleTabItem
        {
            get
            {
                return visibleTabItems;
            }
        }
      internal  void ctrl_MouseUp(object sender, MouseEventArgs e)
        {
            if ((sender as ControlItem).Name == "controlitem1" && !this.AutoHide)
            {
                this.SuspendLayout();
                AutoHide = true;
                foreach (ToolStripTabItem tab in this.MainItems)
                {
                    if (tab.Visible)
                    {
                        if (!visibleTabItems.Contains(tab))
                            visibleTabItems.Add(tab);
                    }
                }
                foreach(ToolStripTabItem tab in this.MainItems)
                {
                    tab.Visible = false;
                }
                (this.Parent as RibbonControlAdv).MinimizePanel = true;
                ((this.Parent as RibbonControlAdv).Parent as RibbonForm).WindowState = FormWindowState.Maximized;
                ((this.Parent as RibbonControlAdv).Parent as RibbonForm).MinimizeBox = false;
                ((this.Parent as RibbonControlAdv).Parent as RibbonForm).MaximizeBox = false;
                ((this.Parent as RibbonControlAdv).Parent as RibbonForm).ShowIcon = false;
                m_owner.RibbonStatus = false;
                (this.Parent as RibbonControlAdv).QuickPanelVisible = false;
                HandleMenuButtonVisibility = (this.Parent as RibbonControlAdv).MenuButtonVisible;
                (this.Parent as RibbonControlAdv).MenuButtonVisible = false;
                (this.Parent as RibbonControlAdv).ShowMinimizeButton = false;
                autoHideShown = true;
                m_owner.NormalState = false;
                this.ResumeLayout(false);
                this.PerformLayout();
            }
            else if ((sender as ControlItem).Name == "controlItem2")
            {
                AutoHide = false;
                this.PerformLayout();
                (this.Parent as RibbonControlAdv).MinimizePanel = true;
                if (visibleTabItems.Count == 0)
                {
                    foreach (ToolStripTabItem tab in this.MainItems)
                    {
                        if (tab.Visible)
                            tab.Visible = true;
                    }
                }
                else
                {
                    foreach (ToolStripTabItem tab in this.VisibleTabItem)
                    {
                        tab.Visible = true;
                    }
                }
                ((this.Parent as RibbonControlAdv).Parent as RibbonForm).MinimizeBox = true;
                ((this.Parent as RibbonControlAdv).Parent as RibbonForm).MaximizeBox = true;
                this.ResumeLayout(false);
                ((this.Parent as RibbonControlAdv).Parent as RibbonForm).ShowIcon = true;
                (this.Parent as RibbonControlAdv).QuickPanelVisible = true;
                if (autoHideShown)
                    (this.Parent as RibbonControlAdv).MenuButtonVisible = HandleMenuButtonVisibility;
                (this.Parent as RibbonControlAdv).MinimizePanel = true; 
               
            }
            else if ((sender as ControlItem).Name == "controlItem3")
            {
                AutoHide = false;
                if (visibleTabItems.Count == 0)
                {
                    foreach (ToolStripTabItem tab in this.MainItems)
                    {
                        if (tab.Visible)
                            tab.Visible = true;
                    }
                }
                else
                {
                    foreach (ToolStripTabItem tab in this.VisibleTabItem)
                    {
                        tab.Visible = true;
                    }
                }
                m_owner.RibbonStatus = true;
                ((this.Parent as RibbonControlAdv).Parent as RibbonForm).ShowIcon = true;
                (this.Parent as RibbonControlAdv).QuickPanelVisible = true;
                if (autoHideShown)
                    (this.Parent as RibbonControlAdv).MenuButtonVisible = HandleMenuButtonVisibility;
                (this.Parent as RibbonControlAdv).Visible = true;
                (this.Parent as RibbonControlAdv).MinimizePanel = false;
                ((this.Parent as RibbonControlAdv).Parent as RibbonForm).ShowIcon = true;
                ((this.Parent as RibbonControlAdv).Parent as RibbonForm).MinimizeBox = true;
                ((this.Parent as RibbonControlAdv).Parent as RibbonForm).MaximizeBox = true;
            }
            this.popupControlContainer1.HidePopup();
        }
      private bool autoHideShown = false;
      internal bool HandleMenuButtonVisibility = true;
      internal RibbonDropDownContainer TouchDropDown;
      internal RibbonDropDownContainer ribbonDropDownContainer1;
      internal ControlItem TouchModeItem;
      private ControlItem LabelItem;
      internal ControlItem MouseModeItem;
      internal ControlItem controlItem1;
      internal ControlItem controlItem2;
      internal ControlItem controlItem3;
        void m_menuButton_Click(object sender, EventArgs e)
        {
            if (!mouseclick && this.RibbonStyle!=RibbonStyle.Office2007)
            {
                this.MinimizeButton.Enabled = false;
                this.Minimizeimage = false;
                if (this.backStageView != null)
                {
                    if (this.BackStageView != null && !this.BackStageView.IsVisible && !this.m_owner.MinimizePanel && this.m_owner.MenuButtonEnabled)
                    {
                        this.BackStageView.BackStage.BackStageStyle = this.m_owner.RibbonStyle;
                        this.BackStageView.IsVisible = true;
                        if (this.RibbonStyle == RibbonStyle.Office2013)
                        {
                            this.m_owner.ShowQuickItemsDropDownButton = false;
                            this.DisplayedItems.Remove(this.Ribbon2013MinimizeButton);
                            this.imageButton1.Visible = true;
                        }
                        this.m_owner.MinimizePanel = true;
                        this.Minimizeimage = false;
                    }
                    else if (this.BackStageView != null && this.BackStageView.IsVisible && this.m_owner.MinimizePanel)
                    {
                        this.BackStageView.IsVisible = false;
                        this.MinimizeButton.Enabled = true;
                        this.Minimizeimage = true;
                    }
                    else if (this.BackStageView != null && !this.BackStageView.IsVisible && this.m_owner.MenuButtonEnabled)
                    {
                        if (this.RibbonStyle == RibbonStyle.Office2013)
                        {
                            foreach (ToolStripTabItem tab in MainItems)
                            {
                                tab.Visible = false;
                            }
                            this.m_owner.ShowQuickItemsDropDownButton = false;
                            this.imageButton1.Visible = true;
                            this.DisplayedItems.Remove(this.Ribbon2013MinimizeButton);
                        }
                        this.BackStageView.BackStage.BackStageStyle = this.m_owner.RibbonStyle;
                        this.BackStageView.IsVisible = true;
                    }
                    else if (this.BackStageView != null && this.BackStageView.IsVisible)
                        this.BackStageView.IsVisible = false;
                }
            }
        }
		#endregion

		#region Methods
		/// <summary>
		/// 
		/// </summary>
		internal void UpdateRenderer()
		{
			if( m_owner != null )
			{
				if (m_owner.RibbonStyle == RibbonStyle.Office2010)
				{
					switch (m_owner.OfficeColorScheme)
					{
						case ToolStripEx.ColorScheme.Silver:
							this.Renderer = new Office2010RibbonHeaderRenderer(new Office2010ColorTable(Office2010ColorScheme.Silver));
							break;
						case ToolStripEx.ColorScheme.Black:
							this.Renderer = new Office2010RibbonHeaderRenderer(new Office2010ColorTable(Office2010ColorScheme.Black));
							break;
						default:
							this.Renderer = new Office2010RibbonHeaderRenderer(new Office2010ColorTable(Office2010ColorScheme.Blue));
							break;
					}
				}
				else if (m_owner.RibbonStyle == RibbonStyle.Office2013)
				{
					this.Renderer = new Office2013RibbonHeaderRenderer(new Office2010ColorTable(Office2010ColorScheme.Silver));
				}
				else
				{
					switch (m_owner.OfficeColorScheme)
					{
						case ToolStripEx.ColorScheme.Managed:
							this.Renderer = new RibbonControlAdvHeaderRenderer(Office12ColorTable.ManagedColors);
							break;
						case ToolStripEx.ColorScheme.Silver:
							this.Renderer = new RibbonControlAdvHeaderRenderer(new Office12ColorTable());
							break;
						case ToolStripEx.ColorScheme.Blue:
							this.Renderer = new RibbonControlAdvHeaderRenderer(new OfficeBlue());
							break;
						case ToolStripEx.ColorScheme.Black:
							this.Renderer = new RibbonControlAdvHeaderRenderer(new OfficeBlack());
							break;
					}
				}

				if (this.BackStageView != null && this.BackStageView.BackStage != null)
					this.BackStageView.BackStage.OfficeColorScheme = m_owner.OfficeColorScheme;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal void UpdateSystemButtons()
		{
			m_bDisplaySysButtons = false;
			m_bDisplayMdiButtons = false;
			m_bDisplayHelpButton = false;

			RibbonForm form = this.Form;

			if( form != null && form.Appearance == RibbonForm.AppearanceType.Office2007 )
			{
				m_bDisplaySysButtons = !form.CompositionEnabled && form.ControlBox && form.FormBorderStyle != FormBorderStyle.None;

                if (this.RibbonStyle == RibbonStyle.Office2013)
                    m_bDisplaySysButtons = true;
				if( m_bDisplaySysButtons )
				{
					bool bSizeBox = form.MaximizeBox || form.MinimizeBox;

					foreach( SystemButton sb in this.SystemButtons )
					{
						switch( sb.SysCommand )
						{
							case (int)SystemCommand.SC_MAXIMIZE:
							case (int)SystemCommand.SC_RESTORE:
							{
								sb.Available = bSizeBox;
								if( bSizeBox )
								{
									sb.Enabled = form.MaximizeBox;
								}
								break;
							}
							case (int)SystemCommand.SC_MINIMIZE:
							{
								sb.Available = bSizeBox;
								if( bSizeBox )
								{
									sb.Enabled = form.MinimizeBox;
								}
								break;
							}
							case (int)SystemCommand.SC_CLOSE:
							{
								sb.Enabled = form.CloseBox;
								break;
							}
						}
					}
				}

				if( form.IsMdiContainer )
				{
					Form child = form.ActiveMdiChild;
					if( child != null && child.WindowState == FormWindowState.Maximized )
					{
						Control mdiClient = child.Parent;
						if( mdiClient != null)
						{
							m_bDisplayMdiButtons = true;
						}
					}
				}

				if( form.HelpButton )
					m_bDisplayHelpButton = true;

				m_bDisplayMinimizeButton = m_owner.ShowMinimizeButton;
			}

			if( this.IsHandleCreated )
			{
				PerformLayout();
				Invalidate();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		internal Size GetMinimumSize()
		{
			Size size = m_layoutEngine.GetPreferredSize( Size.Empty, this, true );
			return size;
		}
		/// <summary>
		/// Gets text for component to use in quick panel stuff.
		/// </summary>
		/// <param name="comp">Component to get text for.</param>
		/// <returns>Text for component.</returns>
		public string GetText( Component comp )
		{
			if( comp is ToolStripEx )
			{
				return GetToolstripText( (ToolStripEx)comp );
			}
			else if( comp is ToolStripItem )
			{
				return GetItemText( (ToolStripItem)comp );
			}

			return string.Empty;
		}
		/// <summary>
		/// Gets image for component to use in quick panel stuff.
		/// </summary>
		/// <param name="comp">Component to get image for.</param>
		/// <returns>Image for component.</returns>
		public Image GetImage( Component comp )
		{
			if( comp is ToolStripEx )
			{
				return GetToolstripImage( (ToolStripEx)comp );
			}
			else if( comp is ToolStripItem )
			{
				return GetItemImage( (ToolStripItem)comp );
			}

			return new Bitmap( typeof( ToolStripButton ), "blank.bmp" );
		}
		#endregion

		#region IRibbonHeader implementation
		/// <summary>
		/// Adds item to the collection of quick items.
		/// </summary>
		/// <param name="item">ToolStripItem to be added.</param>
		public void AddQuickItem( ToolStripItem item )
		{
			if( item != null )
			{
				IQuickItem reflectableItem = item as IQuickItem;
				FieldInfo eventsField = typeof(Component).GetField("events", BindingFlags.NonPublic | BindingFlags.Instance);
				m_quickItems.Add(item);
				if (reflectableItem != null)
				{
                    if (item is ToolStripSplitButton || item is ToolStripDropDownButton || item is ToolStripButton)
					{
						EventHandlerList eventHandlerList = (EventHandlerList)eventsField.GetValue(reflectableItem.ReflectedComponent);
						eventsField.SetValue(item, eventHandlerList);
					}
				}

			}
		}
		/// <summary>
		/// Adds item to the collection of main items.
		/// </summary>
		/// <param name="item">ToolStripItem to be added.</param>
		public void AddMainItem( ToolStripItem item )
		{
			if( item != null )
			{
				SuspendLayout();
				if (m_owner.RibbonStyle == RibbonStyle.Office2013)
					item.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
				RemoveInfo( m_mainItems, item );

				m_mainItems.Add( item );

				ResumeLayout();
			}
			
			if (item is ToolStripTabItem && m_owner != null && m_owner.SortTabItems)
			{
				this.SortTabs();
			}
            this.popupControlContainer1.ParentControl = (this.Parent.Parent as RibbonForm);
		}
		
		protected internal void InsertMainItem(ToolStripTabItem item,int position)
		{
			if (item != null && position != -1)
			{
				SuspendLayout();
				RemoveInfo(m_mainItems, item);
				if (position == MainItems.Count)
					MainItems.Add(item);
				else
					MainItems.Insert(position, item);
				ResumeLayout();
			}
		}
        MainItemsCollection mainItemsCollection = new MainItemsCollection();
		public void SortTabs()
		{
			try
			{
				this.SuspendLayout();
                if (mainItemsCollection.Count != m_mainItems.Count)
                {
                    foreach (ToolStripItem item in m_mainItems)
                    {
                        mainItemsCollection.Add(item);
                    }
                }
				m_mainItems.Sort(new ToolStripTabItemsComparer());
				(m_mainItems[0] as ToolStripTabItem).Checked = true;
				this.ResumeLayout(true);
				this.PerformLayout();
			}
			catch { }
		}
        /// <summary>
        /// Unsorted tab items
        /// </summary>
        public void UnSort()
        {
            int i = 0;
            this.SuspendLayout();
            foreach (ToolStripItem item in mainItemsCollection)
            {
                if (item.Tag != null)
                {
                    int index = Int32.Parse(item.Tag.ToString());
                    index = index - 1;
                    m_mainItems[index] = item;
                    i++;
                }
            }
            (m_mainItems[0] as ToolStripTabItem).Checked = true;
            this.ResumeLayout(true);
            this.PerformLayout();
        }
		/// <summary>
		/// 
		/// </summary>
		public event ToolStripItemEventHandler QuickItemAdded
		{
			add { m_quickItems._ItemAdded += value; }
			remove { m_quickItems._ItemAdded -= value; }
		}
		/// <summary>
		/// 
		/// </summary>
		public event ToolStripItemEventHandler QuickItemRemoved
		{
			add { m_quickItems._ItemRemoved += value; }
			remove { m_quickItems._ItemRemoved -= value; }
		}
		/// <summary>
		/// 
		/// </summary>
		IRibbonItems IRibbonHeader.QuickItems
		{
			get { return (IRibbonItems)this.QuickItems; }
		}
		/// <summary>
		/// 
		/// </summary>
		IRibbonItems IRibbonHeader.MainItems
		{
			get { return (IRibbonItems)this.MainItems; }
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Gets instance of ComplexRibbonLayoutEngine.
		/// </summary>
		public override LayoutEngine LayoutEngine
		{
			get
			{
				if( m_layoutEngine == null )
				{
					m_layoutEngine = new RibbonControlAdvLayoutEngine();
				}
				return m_layoutEngine;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="proposedSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSize( Size proposedSize )
		{
			return m_layoutEngine.GetPreferredSize( proposedSize, this, false );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				SuspendLayout();

				if( m_timer != null )
				{
					m_timer.Stop();

					m_timer.Tick -= new EventHandler( OnTimerTick );

					m_timer.Dispose();
					m_timer = null;
				}

				if( m_toolTip != null )
				{
					m_toolTip.UpdateToolTip -= new UpdateToolTipHandler( OnUpdateToolTip );

					m_toolTip.Dispose();
					m_toolTip = null;
				}

				if( m_quickDropDownButton != null )
				{
					m_quickDropDownButton.Dispose();
					m_quickDropDownButton = null;
				}

				if( m_quickOverflowButton != null )
				{
					m_quickOverflowButton.Dispose();
					m_quickOverflowButton = null;
				}

				if( m_systemButtons != null )
				{
					Dispose( m_systemButtons );
					m_systemButtons = null;
				}

				if( m_mdiButtons != null )
				{
					Dispose( m_mdiButtons );
					m_mdiButtons = null;
				}

				if( m_filteredItems != null )
				{
					m_filteredItems.Dispose();
					m_filteredItems = null;
				}

				if( m_systemMenu != null )
				{
					m_systemMenu.Dispose();
					m_systemMenu = null;
				}

				if (backStageView != null)
					backStageView.ProvideBackStageBounds -= new ProvideBoundsEventHandler(OnProvideBackStageBounds);

				m_quickItems.ItemAdded -= new EventHandler<ListItemEventArgs<ToolStripItem>>( OnQuickItemsItemAdded );
				m_quickItems.ItemRemoved -= new EventHandler<ListItemEventArgs<ToolStripItem>>( OnRibbonItemRemoved );
				m_mainItems.ItemAdded -= new EventHandler<ListItemEventArgs<ToolStripItem>>( OnMainItemsItemAdded );
				m_mainItems.ItemRemoved -= new EventHandler<ListItemEventArgs<ToolStripItem>>( OnRibbonItemRemoved );
				m_groups.ItemRemoved -= new EventHandler<ListItemEventArgs<ToolStripTabGroup>>( OnGroupRemoved );
				m_groups.ItemAdded -= new EventHandler<ListItemEventArgs<ToolStripTabGroup>>( OnGroupAdded );

				m_owner = null;

				this.Renderer = null;

				ResumeLayout( false );
			}

			base.Dispose( disposing );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLayout( LayoutEventArgs e )
		{
			if( m_bReadyForLayout )
			{
				m_quickPanelHeight = -1;
				m_quickPanelWidth = -1;
				m_systemButtonsWidth = -1;
				base.OnLayout( e );
			}
            if (m_owner != null &&m_owner.RibbonStyle == Tools.RibbonStyle.Office2013 && m_owner.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
            {
                imageButton1.Location = new Point(m_owner.Width - imageButton1.Width + 10, 6);
                if(UseDefaultHighlightColor)
                    imageButton1.HoverImage = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.Rbackh.png"));
                else
                    imageButton1.HoverImage = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.Rback.png"));
                imageButton1.NormalImage = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.RBackStage1.png"));
            }
		}
		/// <summary>
		/// Clears info about item.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnItemRemoved( ToolStripItemEventArgs e )
		{
			if( e.Item.Name != null )
			{
				bool bRemove = true;
				foreach (ToolStripItem item in m_quickItems)
				{
					if (item == e.Item)
					{
						bRemove = m_quickItems.DestroyItemsOnRemove;
						break;
					}
				}
				if( bRemove )
				{
					RemoveInfoAboutItem( e.Item );
				}

				if( e.Item is IQuickItem && bRemove )
				{
					e.Item.Dispose();
				}
			}
			if (e.Item is IQuickItem && !this.ShowQuickPanelBelowRibbon)
			{
				IQuickItem reflectableItem = e.Item as IQuickItem;
				FieldInfo eventsField = typeof(Component).GetField("events", BindingFlags.NonPublic | BindingFlags.Instance);
				eventsField.SetValue(e.Item, null);
			}
			if( e.Item is ToolStripTabItem )
			{
				ToolStripTabItem tabItem = e.Item as ToolStripTabItem;

				tabItem.CheckStateChanged -= new EventHandler( OnTabCheckStateChanged );
				tabItem.VisibleChanged -= new EventHandler( OnTabVisibleChanged );
				tabItem.TextChanged -= new EventHandler(tabItem_TextChanged);
			}

			base.OnItemRemoved( e );
		}
		/// <summary>
		/// Initializes tab item.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnItemAdded( ToolStripItemEventArgs e )
		{
			base.OnItemAdded( e );

			if( e.Item is ToolStripTabItem )
			{
				ToolStripTabItem tabItem = (ToolStripTabItem)e.Item;

				if( m_checkedItem == null )
				{
					tabItem.Checked = true;
					m_checkedItem = tabItem;
				}
				else if( tabItem.Checked )
				{
					OnTabCheckStateChanged( tabItem, EventArgs.Empty );
				}

				Invalidate();

				tabItem.CheckStateChanged += new EventHandler( OnTabCheckStateChanged );
				tabItem.VisibleChanged += new EventHandler( OnTabVisibleChanged );
				tabItem.TextChanged += new EventHandler(tabItem_TextChanged);
			}
		}

		void tabItem_TextChanged(object sender, EventArgs e)
		{
			ToolStripTabItem item = sender as ToolStripTabItem;
			if (item != null && m_owner != null && m_owner.SortTabItems)
				this.SortTabs();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnItemClicked( ToolStripItemClickedEventArgs e )
		{
			base.OnItemClicked( e );

			ToolStripItem item = e.ClickedItem;
            if(this.QuickOverflowButton !=null)
			if( this.QuickOverflowButton.DropDown.Equals( item.GetCurrentParent() ) )
			{
				ToolStripDropDownItem dropDownItem = item as ToolStripDropDownItem;

				if( dropDownItem == null || !dropDownItem.DropDown.Visible )
				{
					QuickOverflowButton.HideDropDown();
				}
			}

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnParentChanged( EventArgs e )
		{
			base.OnParentChanged( e );
			UpdateRenderer();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLayoutCompleted( EventArgs e )
		{
			base.OnLayoutCompleted( e );

			if( m_checkedItem != null && m_checkedItem.Panel.IsHandleCreated )
			{
				WindowsAPI.RedrawWindow( m_checkedItem.Panel.Handle, IntPtr.Zero, IntPtr.Zero, RDW_UPDATEFRAME );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected override void SetDisplayedItems()
		{
			m_bHideItems = true;
			base.SetDisplayedItems();
			m_bHideItems = false;

			if( this.QuickPanelVisible && !this.ShowQuickPanelBelowRibbon )
			{
				if( HasOverflowItems )
				{
                    if(QuickOverflowButton!=null)
					this.DisplayedItems.Add( this.QuickOverflowButton );
				}
				else
				{
					if(this.ShowQuickItemsDropDownButton)
						this.DisplayedItems.Add( this.QuickAccessButton );
				}
			}

			if( m_bDisplaySysButtons )
			{
				foreach( ToolStripItem item in this.SystemButtons )
				{
					if( item.Available )
					{
						this.DisplayedItems.Add( item );
					}
				}
			}

			if( m_bDisplayMdiButtons )
			{
				for( int i = 0, count = this.MdiButtons.Count; i < count; i++ )
				{
					this.DisplayedItems.Insert( 0, this.MdiButtons[i] );
				}
			}

			if( m_bDisplayHelpButton )
			{
				this.DisplayedItems.Insert( 0, this.HelpButton );
			}

			if (m_bDisplayMinimizeButton)
			{
				this.DisplayedItems.Insert(0, this.MinimizeButton);
			}
            if (this.RibbonStyle == Tools.RibbonStyle.Office2013 && this.m_owner.ShowRibbonDisplayOptionButton)
            {
                if(this.BackStageView != null)
                {
                    if(!(this.BackStageView.IsVisible))
                        this.DisplayedItems.Insert(0, this.Ribbon2013MinimizeButton);
                }
                else
                    this.DisplayedItems.Insert(0, this.Ribbon2013MinimizeButton);
            }

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc( ref Message m )
		{
			switch( (Msg)m.Msg )
			{
				#region WM_MOUSEMOVE
				case Msg.WM_MOUSEMOVE:
				if( OnMouseMoveMsg( ref m ) )
				{
					m.Result = (IntPtr)0;
					return;
				}
				break;
				#endregion
				#region WM_MOUSELEAVE
				case Msg.WM_MOUSELEAVE:
				OnMouseLeaveMsg();
				break;
				#endregion
				#region WM_LBUTTONDOWN
				case Msg.WM_LBUTTONDOWN:
				if( OnLeftMouseDownMsg( ref m ) )
				{
					m.Result = (IntPtr)0;
					return;
				}
				break;
				#endregion
				#region WM_LBUTTONDBLCLK
				case Msg.WM_LBUTTONDBLCLK:
				if( OnLeftMouseDoubleClickMsg( ref m ) )
				{
					m.Result = (IntPtr)0;
					return;
				}
				break;
				#endregion
				#region WM_LBUTTONUP
				case Msg.WM_LBUTTONUP:
				if( OnLeftMouseUpMsg( ref m ) )
				{
					m.Result = (IntPtr)0;
					return;
				}
				break;
				#endregion
				#region WM_NCHITTEST
				case Msg.WM_NCHITTEST:
				if( OnWmNcHitTest( ref m ) )
				{
					return;
				}
				break;
				#endregion
				#region WM_NCLBUTTONDOWN
				case Msg.WM_NCLBUTTONDOWN:
				if( OnWmNcLButtonDown( ref m ) )
				{
					return;
				}
				break;
				#endregion
				#region WM_NCRBUTTONDOWN
				case Msg.WM_NCRBUTTONDOWN:
				if( OnWmNcRButtonDown( ref m ) )
				{
					return;
				}
				break;
				#endregion
				#region WM_NCLBUTTONDBLCLK
				case Msg.WM_NCLBUTTONDBLCLK:
				if( OnWmNcLButtonDblClk( ref m ) )
				{
					return;
				}
				break;
				#endregion

				#region WM_MOUSEACTIVATE
				case Msg.WM_MOUSEACTIVATE:
				if( OnMouseActivate( ref m ) )
				{
					return;
				}
				break;
				#endregion
				#region WM_CONTEXTMENU
				case Msg.WM_CONTEXTMENU:
				if( OnWmContextMenu( ref m ) )
				{
					return;
				}
				break;
				#endregion

				#region WMU_UPDATESYSBUTTONS
				case (Msg)WMU_UPDATESYSBUTTONS:
				UpdateSystemButtons();
				return;
				#endregion
			}
			base.WndProc( ref m );
		}
		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnTitleFontChanged()
		{
			PerformLayout();
			Update();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRendererChanged( EventArgs e )
		{
			base.OnRendererChanged( e );

			if( m_foreColor == Color.Empty )
			{
				base.ForeColor = this.DefForeColor;
			}
		}
        internal RibbonMinimizePopUp TouchModePop;
        internal RibbonMinimizePopUp popupControlContainer1;
		internal bool Minimizeimage = true;
        internal int Minimizeimageindex = 0;
        private ToolStripItem SelectedItem = null;
		/// <summary>
		/// Selects group.
		/// </summary>
		/// <param name="mea"></param>
		protected override void OnMouseDown( MouseEventArgs mea )
		    {
			base.OnMouseDown( mea );
            if (!AutoHide)
            {
                if (m_owner != null && !m_owner.MinimizePanel)
                {
                    ToolStripTabGroup selectedGroup = GetGroupUnderPoint(mea.Location);
                    if (selectedGroup != null)
                    {
                        foreach (ToolStripItem tsItem in m_mainItems)
                        {
                            ToolStripTabItem item = tsItem as ToolStripTabItem;
                            if (m_hashGroups.ContainsKey(item) && m_hashGroups[item] == selectedGroup)
                            {
                                item.Checked = true;
                                break;
                            }
                        }
                    }
                }
                foreach (ToolStripItem tsItem in m_mainItems)
                {
                    ToolStripTabItem item = tsItem as ToolStripTabItem;
                }
                if (this.BackStageView != null && this.BackStageView.BackStage != null && (this.RibbonStyle == RibbonStyle.Office2010 || this.RibbonStyle == RibbonStyle.Office2013))
                {
                    if (SelectedItem != null && SelectedItem is ToolStripMenuButton && SelectedItem.Text == MenuButton.Text)
                    {
                        mouseclick = true;
                        if (!this.BackStageView.IsVisible && this.m_owner.MenuButtonEnabled)
                        {
                            if (this.RibbonStyle == RibbonStyle.Office2013)
                            {
                                this.m_owner.ShowQuickItemsDropDownButton = false;
                                foreach (ToolStripTabGroup obj in this.m_owner.TabGroups)
                                {
                                    obj.Visible = false;
                                }
                                this.DisplayedItems.Remove(this.Ribbon2013MinimizeButton);
                            }
                            this.MenuButton.PerformClick();
                            this.BackStageView.BackStage.BackStageStyle = this.m_owner.RibbonStyle;
                            this.BackStageView.IsVisible = true;
                            if (this.BackStageView != null && this.BackStageView.IsVisible && this.m_owner.RibbonStyle == Tools.RibbonStyle.Office2013) 
                            {
                                this.ShowItemToolTips = !this.BackStageView.IsVisible && !this.HideMenuButtonToolTip;
                            }
                            this.m_owner.MinimizePanel = true;
                            this.Minimizeimage = false;
                        }
                        else if (this.BackStageView.IsVisible && this.m_owner.MinimizePanel)
                        {

                            this.BackStageView.IsVisible = false;
                            this.MinimizeButton.Enabled = true;
                            this.Minimizeimage = true;

                        }
                        else if (!this.BackStageView.IsVisible && this.m_owner.MenuButtonEnabled)
                            this.BackStageView.IsVisible = true;
                        else if (this.BackStageView.IsVisible)
                            this.BackStageView.IsVisible = false;
                    }
                    else if (SelectedItem != null && SelectedItem.Text == MinimizeButton.Text)
                    {
                        if (!this.BackStageView.IsVisible)
                            this.m_owner.MinimizePanel = !this.m_owner.MinimizePanel;
                        this.Minimizeimage = true;
                    }
                    else if (this.BackStageView.IsVisible && this.m_owner.MinimizePanel && SelectedItem is ToolStripTabItem)
                    {
                        if (this.Minimizeimageindex == 1)
                            this.m_owner.MinimizePanel = false;
                        else
                            this.m_owner.MinimizePanel = true;
                        this.MinimizeButton.Enabled = true;
                        this.Minimizeimage = true;
                        this.BackStageView.IsVisible = false;

                    }
                    else if (SelectedItem != null && SelectedItem is ToolStripTabItem && SelectedItem.Text != MinimizeButton.Text && SelectedItem.Text != MenuButton.Text)
                    {
                        this.BackStageView.IsVisible = false;
                    }
                }
                else
                {
                    if (SelectedItem != null && SelectedItem is Syncfusion.Windows.Forms.Tools.RibbonControlAdvHeader.MinimizeSystemButton && SelectedItem.Text == MinimizeButton.Text)
                    {
                        this.m_owner.MinimizePanel = !this.m_owner.MinimizePanel;
                        if (this.m_owner.MinimizePanel)
                            this.Minimizeimageindex = 0;
                        else
                            this.Minimizeimageindex = 1;
                    }
                }
            }
            if (this.BackStageView != null)
                this.ShowItemToolTips = !this.BackStageView.IsVisible && !this.HideMenuButtonToolTip;
		}
		/// <summary>
		/// 
		/// </summary>
		public override ToolStripItemCollection Items
		{
			get
			{
				ToolStripItemCollection items = base.Items;

				if( m_bHideItems && !this.Disposing )
				{
					if( m_filteredItems == null )
					{
						m_filteredItems = new ToolStripItemCollectionFiltered( this, items );
					}

					items = m_filteredItems;
				}

				return items;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public ToolStripItemCollection OverflowsItems
		{
			get
			{
				if( m_overflowItems == null )
				{
					m_overflowItems = new ToolStripItemCollection( this, new ToolStripItem[] { } );
				}

				return m_overflowItems;
			}
		}
        private Color menuColor = ColorTranslator.FromHtml("#0072C6");
        internal Color MenuColor
        {
            get
            {
                return menuColor;
            }
            set
            {
                if (menuColor != value)
                    menuColor = value;
                ribbonDropDownContainer1.MenuColor = value;
            }
        }
        /// <summary>
        /// Gets whether default highlight color should be used 
        /// </summary>
        private bool useDefaultHighlightColor = true;
        /// <summary>
        /// Gets or Sets whether default highlight color should be used 
        /// </summary>
        internal bool UseDefaultHighlightColor
        {
            get
            {
                return useDefaultHighlightColor;
            }
            set
            {
                if (useDefaultHighlightColor != value)
                    useDefaultHighlightColor = value;
                if (!useDefaultHighlightColor)
                {
                    imageButton1.HoverImage = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.back.png"));
                }
            }
        }
		/// <summary>
		/// Added Scroll buttons painting.
		/// </summary>
		/// <param name="e"></param> 
		protected override void OnPaint( PaintEventArgs e )
		{
            if (this.RibbonStyle == RibbonStyle.Office2013 || this.RibbonStyle == RibbonStyle.Office2010)
            {
                if (this.RibbonStyle == RibbonStyle.Office2013 && this.Renderer is Office2013RibbonHeaderRenderer)
                {
                    (this.Renderer as Office2013RibbonHeaderRenderer).MenuColor = this.MenuColor;
                    (this.Renderer as Office2013RibbonHeaderRenderer).UseDefaultHighlightColor = this.UseDefaultHighlightColor;
                }
                if (this.BackStageView != null && this.BackStageView.BackStage!=null )
                {
                    if (this.BackStageView.BackStage.MenuColor != this.MenuColor)
                    this.BackStageView.BackStage.MenuColor = this.MenuColor;
                    if ( this.Renderer is Office2013RibbonHeaderRenderer)
                        (this.Renderer as Office2013RibbonHeaderRenderer).MenuColor = this.BackStageView.BackStage.MenuColor;
                    else if (this.Renderer is Office2010RibbonHeaderRenderer )
                        (this.Renderer as Office2010RibbonHeaderRenderer).MenuColor = this.BackStageView.BackStage.MenuColor;
			
                }
            }
            if (this.RibbonStyle == RibbonStyle.Office2013)
            {
                if (this.BackStageView != null && this.BackStageView.BackStage != null && this.BackStageView.BackStage.Visible)
                {
                    using (SolidBrush b = new SolidBrush(Color.White))
                    {
                        if (m_owner.RightToLeft != System.Windows.Forms.RightToLeft.Yes)
                            e.Graphics.FillRectangle(b, new Rectangle(this.BackStageView.BackStage.ItemSize.Width - 4, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height));
                        else
                            e.Graphics.FillRectangle(b, new Rectangle(1, this.Bounds.Y, this.Bounds.Width, this.Bounds.Height));
                        using (Pen borderpen = new Pen(this.MenuColor))
                        {
                            e.Graphics.DrawLine(borderpen, new Point(0, 1), new Point(this.Bounds.Width, 1));
                        }
                    }
                }


                if (this.BackStageView != null && this.BackStageView.BackStage != null && this.BackStageView.BackStage.Visible)
                {
                    using (SolidBrush brush1 = new SolidBrush(m_owner.MenuColor))
                    {
                        RibbonControlAdv ribbon = this.Parent as RibbonControlAdv;
                        if ((this.Parent as RibbonControlAdv).RibbonHeaderImage != RibbonHeaderImage.None && !this.AutoHide)
                        {
                            Image headerImage = null;

                            if (this.m_owner.RibbonHeaderImage == RibbonHeaderImage.Custom)
                            {
                                headerImage = m_owner.CustomRibbonHeaderImage;
                            }
                            else
                            {
                                headerImage = Image.FromStream(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources." + (this.Parent as RibbonControlAdv).RibbonHeaderImage + ".bmp"));

                                if (ribbon.Office2013ColorScheme == Office2013ColorScheme.White)
                                    headerImage = Image.FromStream(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources." + (this.Parent as RibbonControlAdv).RibbonHeaderImage + ".bmp"));

                                else if (ribbon.Office2013ColorScheme == Office2013ColorScheme.LightGray)
                                    headerImage = Image.FromStream(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources." + (this.Parent as RibbonControlAdv).RibbonHeaderImage + "l.bmp"));

                                else if (ribbon.Office2013ColorScheme == Office2013ColorScheme.DarkGray)
                                    headerImage = Image.FromStream(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources." + (this.Parent as RibbonControlAdv).RibbonHeaderImage + "d.bmp"));
                            }

                            if (headerImage != null)
                            {
                                if (e.Graphics.DpiX > 120)
                                {
                                    if (m_owner.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                                    {
                                        e.Graphics.ScaleTransform(1.5F, 1.4F);
                                        e.Graphics.DrawImage(headerImage, new Rectangle(0, this.Bounds.Y, headerImage.Size.Width, headerImage.Size.Height));
                                    }
                                    else
                                    {
                                        e.Graphics.ScaleTransform(1.5F, 1.4F);
                                        e.Graphics.DrawImage(headerImage, new Rectangle(this.Width - (int)headerImage.Size.Width - (int)this.Width / 3, this.Bounds.Y, headerImage.Size.Width, headerImage.Size.Height));
                                    }
                                }
                                else if (e.Graphics.DpiX > 96)
                                {
                                    if (m_owner.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                                    {
                                        e.Graphics.ScaleTransform(1.5F, 1.3F);
                                        e.Graphics.DrawImage(headerImage, new Rectangle(0, this.Bounds.Y, headerImage.Size.Width, headerImage.Size.Height));
                                    }
                                    else
                                    {
                                        e.Graphics.ScaleTransform(1.5F, 1.3F);
                                        e.Graphics.DrawImage(headerImage, new Rectangle(this.Width - (int)headerImage.Size.Width - (int)this.Width / 4, this.Bounds.Y, headerImage.Size.Width, headerImage.Size.Height));
                                    }
                                }
                                else
                                    if (m_owner.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                                    {
                                        e.Graphics.DrawImage(headerImage, new Rectangle(0, this.Bounds.Y, headerImage.Size.Width, headerImage.Size.Height));
                                    }
                                    else
                                        e.Graphics.DrawImage(headerImage, new Rectangle(this.Width - (int)headerImage.Size.Width, this.Bounds.Y, headerImage.Size.Width, headerImage.Size.Height));
                            }
                        }
                        if (m_owner.RightToLeft != System.Windows.Forms.RightToLeft.Yes)
                        {
                            if (m_form.CompositionEnabled)
                            {
                                if (m_form.WindowState == FormWindowState.Maximized)
                                {
                                    e.Graphics.FillRectangle(brush1, new Rectangle(0, 0, this.BackStageView.BackStage.ItemSize.Width - 2, this.Bounds.Height));
                                }
                                else
                                {
                                    if (e.Graphics.DpiX < 97)
                                    {
                                        e.Graphics.FillRectangle(brush1, new Rectangle(0, 0, this.BackStageView.BackStage.ItemSize.Width - 1, this.Bounds.Height));
                                    }
                                    else
                                    {
                                        if ((this.Parent as RibbonControlAdv).RibbonHeaderImage == RibbonHeaderImage.None)
                                            e.Graphics.FillRectangle(brush1, new Rectangle(0, 0, this.BackStageView.BackStage.ItemSize.Width - 1, this.Bounds.Height));
                                        else
                                            e.Graphics.FillRectangle(brush1, new Rectangle(0, 0, this.BackStageView.BackStage.ItemSize.Width - 47, this.Bounds.Height));
                                    }
                                }
                            }
                            else
                            {
                                if (m_form.WindowState == FormWindowState.Maximized)
                                    e.Graphics.FillRectangle(brush1, new Rectangle(0, 0, this.BackStageView.BackStage.ItemSize.Width - 4, this.Bounds.Height));
                                else
                                    e.Graphics.FillRectangle(brush1, new Rectangle(0, 0, this.BackStageView.BackStage.ItemSize.Width - 1, this.Bounds.Height));
                            }
                        }
                        else
                        {
                            if (m_form.CompositionEnabled)
                            {
                                if (m_form.WindowState == FormWindowState.Maximized)
                                    e.Graphics.FillRectangle(brush1, new Rectangle(m_owner.Width - this.BackStageView.BackStage.ItemSize.Width + 13, 0, this.BackStageView.BackStage.ItemSize.Width - 1, this.Bounds.Height));
                                else
                                    e.Graphics.FillRectangle(brush1, new Rectangle(m_owner.Width - this.BackStageView.BackStage.ItemSize.Width + 1, 0, this.BackStageView.BackStage.ItemSize.Width - 2, this.Bounds.Height));
                            }
                            else
                            {
                                if (m_form.WindowState == FormWindowState.Maximized)
                                    e.Graphics.FillRectangle(brush1, new Rectangle(m_owner.Width - this.BackStageView.BackStage.ItemSize.Width + 10, 0, this.BackStageView.BackStage.ItemSize.Width - 1, this.Bounds.Height));
                                else
                                    e.Graphics.FillRectangle(brush1, new Rectangle(m_owner.Width - this.BackStageView.BackStage.ItemSize.Width - 3, 0, this.BackStageView.BackStage.ItemSize.Width - 2, this.Bounds.Height));
                            }
                        }
                        TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.RightToLeft;
                        Rectangle rctext = this.TitleRect;
                        rctext.Y += 4;
                        if (m_form != null)
                        {

                            if (m_form.ContainsFocus || m_form.designmode)
                            {
                                if (m_owner.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                                    TextRenderer.DrawText(e.Graphics, this.Title, this.TitleFont, this.DisplayRectangle, this.TitleColor, flags);
                                else
                                    TextRenderer.DrawText(e.Graphics, this.Title, this.TitleFont, rctext, this.TitleColor, flags);
                            }
                            else
                            {
                                if (m_owner.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
                                    TextRenderer.DrawText(e.Graphics, this.Title, this.TitleFont, this.DisplayRectangle, ControlPaint.LightLight(this.TitleColor), flags);
                                else
                                    TextRenderer.DrawText(e.Graphics, this.Title, this.TitleFont, rctext, ControlPaint.LightLight(this.TitleColor), flags);
                            }
                        }
                    }
                }
            }
            base.OnPaint(e);
			Rectangle rcTabItems = TabItemsRectangle;

			int iTabItemsWidth = GetTabItemsWidth();
			int iTabItemsRectangleWidth = rcTabItems.Width;

			if( iTabItemsWidth > iTabItemsRectangleWidth )
			{
				int iTabItemsRectangleHeight = rcTabItems.Height;
				int iTabItemsRectangleTop = rcTabItems.Top;

				// Choose needed Scroll buttons to further painting.
				if( RightToLeft == RightToLeft.Yes )
				{
					m_bIsLeftScroll = ( iTabItemsWidth + m_iScrollPosition > iTabItemsRectangleWidth );
					m_bIsRightScroll = ( m_iScrollPosition < 0 );
				}
				else
				{
					m_bIsLeftScroll = ( m_iScrollPosition > 0 );
					m_bIsRightScroll = ( iTabItemsWidth - m_iScrollPosition > iTabItemsRectangleWidth );
				}


				if( m_bIsRightScroll )
				{
					// Draw right scroll button over the Tab items.
					Rectangle rc = new Rectangle( rcTabItems.Right - SCROLL_BUTTON_WIDTH, iTabItemsRectangleTop, SCROLL_BUTTON_WIDTH, iTabItemsRectangleHeight );
					
					if(this.RibbonStyle == RibbonStyle.Office2007)
						(this.Renderer as RibbonControlAdvHeaderRenderer).DrawTabScrollButton( this, e.Graphics, rc, true );
					else if (this.RibbonStyle == RibbonStyle.Office2013 )
						(this.Renderer as Office2013RibbonHeaderRenderer).DrawTabScrollButton(this, e.Graphics, rc, true);
					else
						(this.Renderer as Office2010RibbonHeaderRenderer).DrawTabScrollButton(this, e.Graphics, rc, true);
				}
				else
				{
					// Set right scroll selected state to false to highlight Tab item on which cursor is.
					m_bRightScrollSelected = false;
				}

				if( m_bIsLeftScroll )
				{
					// Draw left scroll button over the Tab items.
					Rectangle rc = new Rectangle( rcTabItems.Left, iTabItemsRectangleTop, SCROLL_BUTTON_WIDTH, iTabItemsRectangleHeight );
					if (this.RibbonStyle == RibbonStyle.Office2007)
						(this.Renderer as RibbonControlAdvHeaderRenderer).DrawTabScrollButton(this, e.Graphics, rc, false);
					else if (this.RibbonStyle == RibbonStyle.Office2013 )
						(this.Renderer as Office2013RibbonHeaderRenderer).DrawTabScrollButton(this, e.Graphics, rc, false);
					else
						(this.Renderer as Office2010RibbonHeaderRenderer).DrawTabScrollButton(this, e.Graphics, rc, false);

				}
				else
				{
					// Set left scroll selected state to false to highlight Tab item on which cursor is.
					m_bLeftScrollSelected = false;
				}
			}
			else
			{
				m_bIsLeftScroll = false;
				m_bIsRightScroll = false;
			}

			if( m_bDrawSeparators && m_iSepatators.Count > 0 )
			{
                if (this.RibbonStyle == RibbonStyle.Office2007)
                    (this.Renderer as RibbonControlAdvHeaderRenderer).DrawSeparators(this, e.Graphics, rcTabItems);
			}
		}
		/// <summary>
		/// Added reaction on ScrollPositionInternal changes.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged( EventArgs e )
		{
			base.OnSizeChanged( e );

			ScrollPositionInternal = GetValidScrollPosition( ScrollPositionInternal );
		}
		/// <summary>
		/// Handles release of mouse capture.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseCaptureChanged( EventArgs e )
		{
			if( !this.Capture )
			{
				m_timer.Stop();
				this.PushedButton = ScrollButtonsArea.None;
			}
		}
        private Color autoHideMouseMoveColor = Color.White;
        public Color AutoHideMouseMoveColor
        {
            get
            {
                return autoHideMouseMoveColor;
            }
            set
            {
                autoHideMouseMoveColor = value;
                this.Refresh();
            }
        }
            
		/// <summary>
		/// 
		/// </summary>
		/// <param name="mea"></param>
		protected override void OnMouseUp( MouseEventArgs mea )
		{
			base.OnMouseUp( mea );
            mouseclick = false;
			this.Capture = false;
		}

		#endregion

		#region MessageHandlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmNcHitTest( ref Message m )
		{
			if( m_owner.IsFormManager )
			{
				Point pt = WindowsAPI.GetPointFromLPARAM( (int)m.LParam );
				if( GetHitTest( ref pt ) )
				{
					HitTest hitTest = HitTest.HTNOWHERE;

					IntPtr dwmResult = IntPtr.Zero;

					if( this.Form.CompositionEnabled )
					{
						DwmAPI.DwmDefWindowProc( this.Form.Handle, m.Msg, m.WParam, m.LParam, ref dwmResult );

						switch( (HitTest)dwmResult )
						{
							case HitTest.HTMINBUTTON:
							case HitTest.HTMAXBUTTON:
							case HitTest.HTCLOSE:
							case HitTest.HTHELP:
							hitTest = HitTest.HTTRANSPARENT;
							break;
						}
					}

					if( hitTest == HitTest.HTNOWHERE )
					{
						hitTest = this.Form.GetHitTest( pt );

						if( hitTest == HitTest.HTNOWHERE )
						{
							hitTest = HitTest.HTCAPTION;
						}
					}

					m.Result = (IntPtr)hitTest;
					return true;
				}
			}
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
        private bool OnWmNcLButtonDown(ref Message m)
        {
            if (!AutoHide)
            {
                if (this.popupControlContainer1.IsShowing())
                {
                    this.popupControlContainer1.HidePopup();
                    ctrl_MouseUp(this.controlItem1, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 1));
                }
                if (m_owner.IsFormManager)
                {
                    if (m_owner.Form != null && m_owner.Form.IsMaximized() && (HitTest)m.WParam == HitTest.HTCAPTION)
                    {
                        NativeMethods.SendMessage(m_owner.Form.Handle, NativeMethods.WM_SYSCOMMAND, 0xf012
                        , NativeMethods.MAKELPARAM(Cursor.Position.X, Cursor.Position.Y));
                    }
                    else
                    {
                        switch ((HitTest)m.WParam)
                        {
                            case HitTest.HTCAPTION:
                                MoveForm(m.LParam);
                                break;
                            case HitTest.HTTOP:
                                ResizeForm(WMSZ_TOP, m.LParam);
                                break;
                            case HitTest.HTBOTTOM:
                                ResizeForm(WMSZ_BOTTOM, m.LParam);
                                break;
                            case HitTest.HTLEFT:
                                ResizeForm(WMSZ_LEFT, m.LParam);
                                break;
                            case HitTest.HTRIGHT:
                                ResizeForm(WMSZ_RIGHT, m.LParam);
                                break;
                            case HitTest.HTTOPLEFT:
                                ResizeForm(WMSZ_TOPLEFT, m.LParam);
                                break;
                            case HitTest.HTTOPRIGHT:
                                ResizeForm(WMSZ_TOPRIGHT, m.LParam);
                                break;
                            case HitTest.HTBOTTOMLEFT:
                                ResizeForm(WMSZ_BOTTOMLEFT, m.LParam);
                                break;
                            case HitTest.HTBOTTOMRIGHT:
                                ResizeForm(WMSZ_BOTTOMRIGHT, m.LParam);
                                break;
                            default:
                                return false;
                        }
                    }
                }
                m.Result = IntPtr.Zero;
                return true;
            }
            return true;            
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmNcRButtonDown( ref Message m )
		{
			bool bResult = false;

			if( (int)m.WParam == (int)HitTest.HTCAPTION )
			{
				WindowsAPI.PostMessage( m.HWnd, (int)Msg.WM_CONTEXTMENU, (int)this.Handle/*m.HWnd*/, (int)m.LParam );

				m.Result = IntPtr.Zero;
				bResult = true;
			}

			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmNcLButtonDblClk( ref Message m )
		{
            if (!AutoHide)
            {
                if (m_owner.IsFormManager && this.Form.MaximizeBox)
                {
                    Rectangle rc;
                    if (this.RightToLeft == RightToLeft.Yes)
                    {
                        rc = new Rectangle(this.DisplayRectangle.Width - DEF_ICON_SIZE, DEF_PADDING, DEF_ICON_SIZE + DEF_PADDING, DEF_ICON_SIZE + DEF_PADDING);
                    }
                    else
                    {
                        rc = new Rectangle(DEF_PADDING, DEF_PADDING, DEF_ICON_SIZE + DEF_PADDING, DEF_ICON_SIZE + DEF_PADDING);
                    }
                    Point mouseposition = Cursor.Position;
                    Point pt = this.PointToClient(mouseposition);
                    if (rc.Contains(pt) && (this.RibbonStyle == RibbonStyle.Office2010 || this.RibbonStyle == Tools.RibbonStyle.Office2013))
                    {
                        SystemCommand sc = SystemCommand.SC_CLOSE;
                        BeginInvoke(new SendMessageDelegate(WindowsAPI.SendMessage), new object[] { this.Form.Handle, (int)Msg.WM_SYSCOMMAND, sc, (int)m.LParam });
                    }
                    else
                    {
                        if ((int)m.WParam == (int)HitTest.HTCAPTION)
                        {
                            SystemCommand sc = (this.Form.WindowState == FormWindowState.Maximized) ? SystemCommand.SC_RESTORE : SystemCommand.SC_MAXIMIZE;

                            BeginInvoke(new SendMessageDelegate(WindowsAPI.SendMessage), new object[] { this.Form.Handle, (int)Msg.WM_SYSCOMMAND, sc, (int)m.LParam });
                        }
                    }
                }
            }
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmContextMenu( ref Message m )
		{
			bool bResult = false;

			if( m.WParam == this.Handle )
			{
				if( this.Form == null || !this.Form.IsMdiChild )
				{
					Point pt = WindowsAPI.GetPointFromLPARAM( (int)m.LParam );
					if( GetHitTest( ref pt ) )
					{
						ShowSystemMenu( pt );
						bResult = true;
					}
				}
			}

			return bResult;
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Raises TabCheckedChanged event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnTabCheckStateChanged( object sender, EventArgs e )
		{
			ToolStripTabItem tabItem = (ToolStripTabItem)sender;

			if( tabItem.Checked && tabItem != m_checkedItem )
			{
				ToolStripTabItem oldTab = m_checkedItem;
				m_checkedItem = tabItem;

				if( oldTab != null )
					oldTab.Checked = false;

				ScrollTabItem( tabItem );
				this.OnSelectedTabChanged( oldTab, tabItem );
			}
			else if( !tabItem.Checked )
			{
				if( tabItem == m_checkedItem )
				{
					m_checkedItem = null;
					TryCheckNextTabItem( tabItem );
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnTabVisibleChanged( object sender, EventArgs e )
		{
			ToolStripTabItem tabItem = (ToolStripTabItem)sender;

			if( tabItem.Visible )
			{
				if( m_checkedItem == null )
					tabItem.Checked = true;
			}
			else
				tabItem.Checked = false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnMinimizeRibbonPanel(object sender, EventArgs e)
		{
			 if (SelectedItem != null && SelectedItem.Text != MinimizeButton.Text)
           {
                if (this.m_owner != null)
                    this.m_owner.MinimizePanel = !this.m_owner.MinimizePanel;
           }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		/// <param name="info"></param>
		void OnUpdateToolTip( Component component, ref ToolTipInfo info )
		{
			if( component is SystemButton )
			{
				info.Body.Text = ( (SystemButton)component ).Text;
			}
            else if (component == m_quickOverflowButton && m_owner != null)
            {
                if (BackStageView != null && BackStageView.IsVisible)
                    info.Body.Text = null;
                else
                    info.Body.Text = this.OverFlowButtonToolTip;
            }
            else
            {
                if (!this.m_owner.MinimizePanel)
                {
                    info.Body.Text = this.MinimizeToolTip;
                    info.Header.Text = this.MinimizeToolTip;
                }
                else
                {
                    info.Body.Text = this.MaximizeToolTip;
                    info.Header.Text = this.MaximizeToolTip;
                }
            }
			if( component == m_quickDropDownButton && m_owner!=null )
            {
                if (BackStageView != null && BackStageView.IsVisible)
                    info.Body.Text = null;
                else
                    info.Body.Text = this.QuickDropDownToolTipText;
			}
            if (component is Ribbon2013MinimizeSystemButton)
            {
                info.Body.Text = RibbonDisplayOptionToolTip;
            }
			if( this.Renderer is ToolStripProfessionalRenderer )
			{
				info.BackColor = ( (ToolStripProfessionalRenderer)Renderer ).ColorTable.ToolStripGradientBegin;
			}
            
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnQuickItemsItemAdded( object sender, ListItemEventArgs<ToolStripItem> e )
		{
			SuspendLayout();

			if( m_bShowQuickPanelBelowRibbon )
			{
				if( m_owner != null )
				{
					e.Item.Owner = m_owner.BottomToolstrip;
                    }
                    if (this.RibbonStyle == Tools.RibbonStyle.Office2010)
                    {
                        foreach (ToolStripItem quickItem in this.QuickItems)
                        {
                            if (quickItem is QuickToolstripReflectable)
                            {
                                quickItem.Enabled = !this.BackStageView.IsVisible;
                            }
                        }
                    }
                }
			else e.Item.Owner = this;

			IQuickItem qi = e.Item as IQuickItem;
			if( qi != null && qi.ReflectedComponent!=null )
			{
				qi.ReflectedComponent.Disposed += new EventHandler( OnReflectedComponentDisposed );
			}

			ResumeLayout();
			PerformLayout();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnMainItemsItemAdded( object sender, ListItemEventArgs<ToolStripItem> e )
		{
			e.Item.Owner = this;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnRibbonItemRemoved( object sender, ListItemEventArgs<ToolStripItem> e )
		{
			SuspendLayout();

			IQuickItem qi = e.Item as IQuickItem;
			if( qi != null && qi.ReflectedComponent != null )
			{
				qi.ReflectedComponent.Disposed -= new EventHandler( OnReflectedComponentDisposed );
			}

			e.Item.Owner = null;

			ResumeLayout();
			PerformLayout();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnReflectedComponentDisposed( object sender, EventArgs e )
		{
			foreach( ToolStripItem item in QuickItems )
			{
				IQuickItem qi = item as IQuickItem;

				if( qi != null && qi.Reflects( sender as IComponent ) )
				{
					this.QuickItems.Remove( item );
					break;
				}
			}
		}
		/// <summary>
		/// Removes group from hash.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnGroupRemoved( object sender, ListItemEventArgs<ToolStripTabGroup> args )
		{
			args.Item.NameChanged -= new EventHandler( OnGroupVisualParameterChanged );
			args.Item.ColorChanged -= new EventHandler( OnGroupVisualParameterChanged );
			args.Item.VisibilityChanged -= new EventHandler( OnGroupVisibilityChanged );

			List<ToolStripTabItem> keysToRemove = new List<ToolStripTabItem>();

			foreach( ToolStripTabItem item in m_hashGroups.Keys )
			{
				if( m_hashGroups[item] == args.Item )
				{
					keysToRemove.Add( item );
				}
			}

			foreach( ToolStripTabItem item in keysToRemove )
			{
				m_hashGroups.Remove( item );
			}
		}
		/// <summary>
		/// Subscribed for group events.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnGroupAdded( object sender, ListItemEventArgs<ToolStripTabGroup> e )
		{
			e.Item.NameChanged += new EventHandler( OnGroupVisualParameterChanged );
			e.Item.ColorChanged += new EventHandler( OnGroupVisualParameterChanged );
			e.Item.VisibilityChanged += new EventHandler( OnGroupVisibilityChanged );
		}
		/// <summary>
		/// Invalidates control.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnGroupVisualParameterChanged( object sender, EventArgs e )
		{
			Invalidate();
		}
		/// <summary>
		/// Checks another tab item if previous was hidden.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnGroupVisibilityChanged( object sender, EventArgs e )
		{
			ToolStripTabGroup group = sender as ToolStripTabGroup;

			if( group != null && !group.Visible && m_checkedItem!=null)
			{
				if( m_hashGroups.ContainsKey( m_checkedItem ) && m_hashGroups[m_checkedItem] == group )
				{
					m_checkedItem.Checked = false;
					CheckNextItem( m_checkedItem );
				}
			}

			PerformLayout();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnFormHandleCreated( object sender, EventArgs e )
		{
            Form form = (this.Form != null) ? this.Form : this.FindForm();
            form.LostFocus += new EventHandler(Form_LostFocus);
			Application.AddMessageFilter( this );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnFormHandleDestroyed( object sender, EventArgs e )
		{
			Application.RemoveMessageFilter( this );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnFormTextChanged( object sender, EventArgs e )
		{
			this.PerformLayout();
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		protected internal void RemoveInfoAboutItem( ToolStripItem item )
		{
			if( m_checkedItem == item )
			{
				CheckNextItem( item );
			}

			if( !RemoveInfo( m_quickItems, item ) )
			{
				RemoveInfo( m_mainItems, item );
			}

			PerformLayout();
		}
		/// <summary>
		/// Checks the first visible item that is not given item.
		/// </summary>
		/// <param name="item">Item to be unchecked.</param>
		private void CheckNextItem( ToolStripItem item )
		{
			ToolStripTabItem tabItem = null;

			for( int i = 0; i < m_mainItems.Count; i++ )
			{
				ToolStripTabItem curTabItem = m_mainItems[i] as ToolStripTabItem;

				if( curTabItem != null && curTabItem != item && IsVisibleGroup( curTabItem ) )
				{
					tabItem = curTabItem;
					break;
				}
			}

			if( tabItem != null )
			{
				tabItem.Checked = true;
			}
			else
			{
				m_checkedItem = null;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="itemsData"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		bool RemoveInfo( ObservableList<ToolStripItem> itemsData, ToolStripItem item )
		{
			// TODO: Possibly generic list of ToolStripItems will be turned to some type of hidden implementation of HashTable
			// for quick search in the future.
			for (int i = 0, count = itemsData.Count; i < count; i++)
			{
				if (itemsData[i] == item)
				{
					itemsData.RemoveAt(i);
					return true;
				}
			}
			
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="items"></param>
		/// <param name="minHeight"></param>
		/// <returns></returns>
		Size GetItemsSizePreferred( IList items, int minHeight )
		{
			Size szResult = new Size( 0, minHeight );

			foreach( object obj in items )
			{
				ToolStripItem item = obj as ToolStripItem;

				if( item != null && item.Available )
				{
					Size szItem = item.AutoSize ? item.GetPreferredSize( Size.Empty ) : item.Size;

					int itemHeight = szItem.Height + item.Margin.Top;

					szResult.Width += szItem.Width;

					if( szResult.Height < itemHeight )
					{
						szResult.Height = itemHeight;
					}
				}
			}
			return szResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		internal static Size GetItemsSize( IList items )
		{
			return GetItemsSize( items, DEF_PANEL_HEIGHT, true );
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        internal static Size GetItems125DPISize(IList items)
        {
            return GetItemsSize(items, DPI_125_DEF_PANEL_HEIGHT, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        internal static Size TouchItemSize(IList items)
        {
            return GetItemsSize(items, DEF_PANEL_HEIGHT_TOUCH, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        internal static Size GetItems150DPISize(IList items)
        {
            return GetItemsSize(items, DPI_150_DEF_PANEL_HEIGHT, true);
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		internal static Size GetItemsSize( IList items, int initialHeight, bool bWithMargins )
		{
			Size szResult = new Size( 0, initialHeight );

			foreach( object obj in items )
			{
				ToolStripItem item = obj as ToolStripItem;

				if( item != null && item.Available )
				{
					Size szItem = item.Size;

					int itemHeight = szItem.Height;
					if( bWithMargins )
					{
						itemHeight += item.Margin.Vertical;
					}

					szResult.Width += szItem.Width;

					if( szResult.Height < itemHeight )
					{
						szResult.Height = itemHeight;
					}
				}
			}
			return szResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private Size GetTitleSize()
		{
			Size size = Size.Empty;

			if( m_form != null && m_form.Appearance == RibbonForm.AppearanceType.Office2007 )
			{
				size = TextRenderer.MeasureText( m_form.Text, this.TitleFont );
			}

			return size;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool GetHitTest( ref Point ptScr )
		{
			Rectangle rc = new Rectangle( 0, 0, this.ClientSize.Width, this.Top+this.DisplayRectangle.Top+this.QuickPanelHeight);

			if (RibbonStyle != RibbonStyle.Office2007)
				rc.Height += TabItemsRectangle.Height + 5;

			Point pt = PointToClient( ptScr );

			return ( rc.Contains( pt ) && GetItemAt( pt ) == null && GetGroupUnderPoint( pt ) == null );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private Image GetItemImage( ToolStripItem item )
		{
			Image result = item.Image;

			if( result == null )
			{
				Bitmap bmp = null;

				if( item is ToolStripTextBox )
				{
					bmp = new Bitmap( typeof( ToolStripButton ), "TextBox.bmp" );
				}
				else if( item is ToolStripComboBox )
				{
					bmp = new Bitmap( typeof( ToolStripButton ), "ComboBox.bmp" );
				}
				else
				{
					bmp = new Bitmap( typeof( ToolStripButton ), "blank.bmp" );
				}

				if( bmp != null )
				{
					bmp.MakeTransparent( Color.Magenta );
					result = bmp;
				}
			}

			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="toolStrip"></param>
		/// <returns></returns>
		private Image GetToolstripImage( ToolStripEx toolStrip )
		{
			return ( toolStrip.Image != null ) ? ( toolStrip.Image ) : ( new Bitmap( typeof( ToolStripButton ), "blank.bmp" ) );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private string GetItemText( ToolStripItem item )
		{
			string result = GetQuickText( item );

			if( string.IsNullOrEmpty( result ) )
			{
				if( item is ToolStripTextBox || item is ToolStripComboBox || string.IsNullOrEmpty( item.Text ) )
				{
					result = item.Name;
				}
				else
				{
					result = item.Text;
				}
			}

			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="toolStrip"></param>
		/// <returns></returns>
		private string GetToolstripText( ToolStripEx toolStrip )
		{
			string result = GetQuickText( toolStrip );

			if( string.IsNullOrEmpty( result ) )
			{
				result = ( String.IsNullOrEmpty( toolStrip.Text ) ) ? ( toolStrip.Name ) : ( toolStrip.Text );
			}

			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="comp"></param>
		/// <returns></returns>
		private string GetQuickText( Component comp )
		{
			if( m_owner != null )
			{
				return m_owner.GetDescription( comp );
			}
			return string.Empty;
		}
		/// <summary>
		/// Gets tab group for item.
		/// </summary>
		/// <param name="item">Item to get tab group for.</param>
		/// <returns>Tab group for item.</returns>
		internal ToolStripTabGroup GetItemGroup( ToolStripItem item )
		{
			ToolStripTabGroup result = null;

			if( m_hashGroups.ContainsKey( item ) )
			{
				result = m_hashGroups[item];
			}

			return result;
		}
		/// <summary>
		/// Assigns tab group to item.
		/// </summary>
		/// <param name="item">Item to assign tab group to.</param>
		/// <param name="group">Tab group to assign.</param>
		internal void SetItemGroup( ToolStripItem item, ToolStripTabGroup group )
		{
			if( group != null )
			{
				m_hashGroups[item] = group;
			}
			else
			{
				m_hashGroups.Remove( item );
			}
			PerformLayout();
			Invalidate();
		}
		/// <summary>
		/// Gets group by point.
		/// </summary>
		/// <param name="p">Point to find group at.</param>
		/// <returns>Found group or null.</returns>
		private ToolStripTabGroup GetGroupUnderPoint( Point p )
		{
			foreach( ToolStripTabGroup group in m_groups )
			{
				foreach( Rectangle rect in group.BoundsList )
				{
					if( rect.Contains( p ) )
					{
						return group;
					}
				}
			}
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private bool IsVisibleGroup( ToolStripItem item )
		{
			if (m_hashGroups.ContainsKey(item))
			{
				return m_hashGroups[item].Visible;
			}

			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		internal static ToolStrip GetItemParent( ToolStripItem item )
		{
			ToolStrip ts = item.GetCurrentParent();

			if( ts == null )
			{
				ts = item.Owner;
			}
			return ts;
		}
		/// <summary>
		/// Returns width of all Tab items.
		/// </summary>
		/// <returns></returns>
		private int GetTabItemsWidth()
		{
			int result = 0;

			foreach( ToolStripItem item in m_mainItems )
			{
				if( item is ToolStripTabItem && item.Visible )
				{
					result += ( item.Width + TAB_ITEMS_SEPARATOR_WIDTH );
				}
			}

			return result;
		}
		/// <summary>
		/// Call RedrawWindow method to Repaint Scroll buttons.
		/// </summary>
		internal void RefreshScroll()
		{
			RedrawWindowFlags flags = RedrawWindowFlags.RDW_FRAME | RedrawWindowFlags.RDW_INVALIDATE;
			WindowsAPI.RedrawWindow( this.Handle, IntPtr.Zero, IntPtr.Zero, flags );
		}
		/// <summary>
		/// Process position in Layout for Tab items and if it not in right bounds that set correct value to it.
		/// </summary>
		/// <param name="position"> Position to process. </param>
		/// <returns></returns>
		private int GetValidScrollPosition( int position )
		{
			int iValue = position;

			if( iValue != 0 )
			{
				int iTabItemWidth = GetTabItemsWidth();
				int iWidth = TabItemsRectangle.Width;

				if( RightToLeft == RightToLeft.Yes )
				{
					if( iValue > 0 || iTabItemWidth < iWidth )
					{
						iValue = 0;
					}
					else if( iTabItemWidth > iWidth )
					{
						int iMaxPos = iWidth - iTabItemWidth;

						if( iValue < iMaxPos )
						{
							iValue = iMaxPos;
						}
					}
				}
				else
				{
					if( iValue < 0 || iTabItemWidth < iWidth )
					{
						iValue = 0;
					}
					else if( iTabItemWidth > iWidth )
					{
						int iMaxPos = iTabItemWidth - iWidth;

						if( iValue > iMaxPos )
						{
							iValue = iMaxPos;
						}
					}
				}
			}

			return iValue;
		}
		/// <summary>
		/// Move controls to right according to scroll position and their location.
		/// </summary>
		private void ScrollToRight( bool bRightToLeft )
		{
			int iLeft = TabItemsRectangle.Left;
			int iRight = TabItemsRectangle.Right - SCROLL_BUTTON_WIDTH;

			// Determine which control must be positioned first and move group to preview needed controls.
			if( bRightToLeft )
			{
				for( int i = m_mainItems.Count - 1; i >= 0; i-- )
				{
					ToolStripItem item = m_mainItems[i] as ToolStripItem;

					if( item != null && item.Visible )
					{
						if( item.Bounds.Right >= iRight )
						{
							int iChangePosition = ( item.Bounds.Left - iLeft ) - SCROLL_BUTTON_WIDTH;

							if( i == 0 )
							{
								iChangePosition += SCROLL_BUTTON_WIDTH;
							}

							if( iChangePosition != 0 )
							{
								ScrollPositionInternal = GetValidScrollPosition( ScrollPositionInternal + iChangePosition );
								break;
							}
						}
					}
				}
			}
			else
			{
				for( int i = 0, count = m_mainItems.Count; i < count; i++ )
				{
					ToolStripItem item = m_mainItems[i] as ToolStripItem;

					if( item != null && item.Visible )
					{
						if( item.Bounds.Right >= iRight )
						{
							int iChangePosition = ( item.Bounds.Left - iLeft ) - SCROLL_BUTTON_WIDTH;

							if( i == 0 )
							{
								iChangePosition += SCROLL_BUTTON_WIDTH;
							}

							if( iChangePosition != 0 )
							{
								ScrollPositionInternal = GetValidScrollPosition( ScrollPositionInternal + iChangePosition );
								break;
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// Move controls to left according to scroll position and their location.
		/// </summary>
		private void ScrollToLeft( bool bRightToLeft )
		{
			int iLeft = TabItemsRectangle.Left + SCROLL_BUTTON_WIDTH;
			int iWidth = TabItemsRectangle.Width - SCROLL_BUTTON_WIDTH;

			// Determine which control must be positioned last and move group to preview needed controls.
			if( bRightToLeft )
			{
				for( int i = 0, count = m_mainItems.Count; i < count; i++ )
				{
					ToolStripItem item = m_mainItems[i] as ToolStripItem;

					if( item != null && item.Visible )
					{
						if( item.Bounds.Left <= iLeft )
						{
							int iChangePosition = ( item.Bounds.Right - iLeft + SCROLL_BUTTON_WIDTH ) - iWidth;

							if( i == 0 )
							{
								iChangePosition -= SCROLL_BUTTON_WIDTH;
							}

							if( iChangePosition != 0 )
							{
								ScrollPositionInternal = GetValidScrollPosition( ScrollPositionInternal + iChangePosition );
								break;
							}
						}
					}
				}
			}
			else
			{
				for( int count = m_mainItems.Count - 1, i = count; i >= 0; i-- )
				{
					ToolStripItem item = m_mainItems[i] as ToolStripItem;

					if( item != null && item.Visible )
					{
						if( item.Bounds.Left <= iLeft )
						{
							int iChangePosition = ( item.Bounds.Right - iLeft + SCROLL_BUTTON_WIDTH ) - iWidth;

							if( i == count )
							{
								iChangePosition -= SCROLL_BUTTON_WIDTH;
							}

							if( iChangePosition != 0 )
							{
								ScrollPositionInternal = GetValidScrollPosition( ScrollPositionInternal + iChangePosition );
								break;
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// Initializes & starts timer.
		/// </summary>
		/// <param name="mousePushedArea">Area where mouse was pushed and caused timer to start.</param>
		private void StartTimer( ScrollButtonsArea mousePushedArea )
		{
			m_timer.Interval = m_timerInt * 4;
			m_timer.Tag = mousePushedArea;
			m_timer.Start();
		}
		/// <summary>
		/// Handles mouse keeping pushed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTimerTick( object sender, EventArgs e )
		{
			ScrollButtonsArea area = (ScrollButtonsArea)( ( (Timer)sender ).Tag );

			Point p = PointToClient( Control.MousePosition );

			switch( area )
			{
				case ScrollButtonsArea.RightScrollButton:
				if( RightScrollBounds.IsEmpty )
				{
					this.Capture = false;
				}
				else if( RightScrollBounds.Contains( p ) )
				{
					ScrollToRight( ( RightToLeft == RightToLeft.Yes ) );
				}
				break;

				case ScrollButtonsArea.LeftScrollButton:
				if( LeftScrollBounds.IsEmpty )
				{
					this.Capture = false;
				}
				else if( LeftScrollBounds.Contains( p ) )
				{
					ScrollToLeft( ( RightToLeft == RightToLeft.Yes ) );
				}
				break;
			}

			m_timer.Interval = m_timerInt;
		}
		/// <summary>
		/// Process action when user moves mouse over right or left scroll button or Tab items.
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnMouseMoveMsg( ref Message m )
		{
			bool bResult = false;
			bool bRightScrollSelected = false;

			Point p = WindowsAPI.GetPointFromLPARAM( (int)m.LParam );

			// If mouse over right scroll button than highlight it.
			if( m_bIsRightScroll )
			{
				bRightScrollSelected = RightScrollBounds.Contains( p );

				if( bRightScrollSelected != m_bRightScrollSelected )
				{
					m_bRightScrollSelected = bRightScrollSelected;
					RefreshScroll();
				}
			}
			// If mouse over left scroll button than highlight it.
			if( m_bIsLeftScroll && !bRightScrollSelected )
			{
				bool bLeftScrollSelected = LeftScrollBounds.Contains( p );

				if( bLeftScrollSelected != m_bLeftScrollSelected )
				{
					m_bLeftScrollSelected = bLeftScrollSelected;
					RefreshScroll();
				}
			}

			ToolStripItem item = GetItemAt( p );
            SelectedItem = item;
			if( item is ToolStripTabItem )
			{
				ToolStripTabItem tabItem = item as ToolStripTabItem;

				if( m_SelectedTabItem == null )
				{
					m_SelectedTabItem = tabItem;
				}

				if( m_bRightScrollSelected || m_bLeftScrollSelected )
				{
					m_SelectedTabItem.SelectedInternal = false;
				}
				else
				{
					Rectangle rcItemBounds = item.Bounds;
					Rectangle rcIntersection = Rectangle.Intersect( TabItemsRectangle, rcItemBounds );

					if( rcIntersection.Contains( p ) )
					{
						tabItem.SelectedInternal = true;

						if( m_SelectedTabItem != tabItem )
						{
							m_SelectedTabItem.SelectedInternal = false;
							m_SelectedTabItem = tabItem;
						}
					}
				}

				bResult = true;
			}
			else
			{
				if( m_SelectedTabItem != null )
				{
					m_SelectedTabItem.SelectedInternal = false;
					m_SelectedTabItem = null;
				}
			}

			return bResult;
		}
		/// <summary>
		/// Process action when user press left mouse button on right or left scroll button or Tab items.
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnLeftMouseDownMsg( ref Message m )
		{
			bool bResult = false;

			Point p = WindowsAPI.GetPointFromLPARAM( (int)m.LParam );

			if( m_bIsRightScroll && RightScrollBounds.Contains( p ) )
			{
				bResult = true;
				this.Capture = true;

				PushedButton = ScrollButtonsArea.RightScrollButton;
				ScrollToRight( RightToLeft == RightToLeft.Yes );
				StartTimer( ScrollButtonsArea.RightScrollButton );
			}
			if( !bResult && ( m_bIsLeftScroll && LeftScrollBounds.Contains( p ) ) )
			{
				bResult = true;

				this.Capture = true;

				PushedButton = ScrollButtonsArea.LeftScrollButton;
				ScrollToLeft( ( RightToLeft == RightToLeft.Yes ) );
				StartTimer( ScrollButtonsArea.LeftScrollButton );
			}

			if( !bResult )
			{
				ToolStripItem item = GetItemAt( p );

				if( item is ToolStripTabItem )
				{
					ToolStripTabItem tabItem = item as ToolStripTabItem;

					Rectangle rcItemBounds = item.Bounds;
					Rectangle rcIntersection = Rectangle.Intersect( TabItemsRectangle, rcItemBounds );

					if( !rcIntersection.Contains( p ) )
					{
						bResult = true;
					}
				}
			}

			return bResult;
		}
		/// <summary>
		/// When user double clicked on right or left scroll button return true and 
		/// not process this action, otherwise - return false.
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnLeftMouseDoubleClickMsg( ref Message m )
		{
			bool bResult = false;

			Point p = WindowsAPI.GetPointFromLPARAM( (int)m.LParam );

			if( ( m_bIsRightScroll && RightScrollBounds.Contains( p ) ) ||
				( m_bIsLeftScroll && LeftScrollBounds.Contains( p ) ) )
			{
				bResult = true;
			}

			return bResult;
		}
		/// <summary>
		/// When user press mouse on right or left scroll button return true and 
		/// not process this action, otherwise - false.
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnLeftMouseUpMsg( ref Message m )
		{
			Point p = WindowsAPI.GetPointFromLPARAM( (int)m.LParam );

			if( ( m_bIsRightScroll && RightScrollBounds.Contains( p ) ) ||
				( m_bIsLeftScroll && LeftScrollBounds.Contains( p ) ) || PushedButton != ScrollButtonsArea.None )
			{
				this.Capture = false;
				return true;
			}

			return false;
		}
		/// <summary>
		/// Remove selection from last selected TabItem. 
		/// </summary>
		private void OnMouseLeaveMsg()
		{
			if( m_SelectedTabItem != null )
			{
				m_SelectedTabItem.SelectedInternal = false;
			}

			if( m_bRightScrollSelected )
			{
				m_bRightScrollSelected = false;
				RefreshScroll();
			}
			else if( m_bLeftScrollSelected )
			{
				m_bLeftScrollSelected = false;
				RefreshScroll();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnMouseActivate( ref Message m )
		{
			bool bResult = false;
			if( WindowsAPI.HIGH_ORDER( (int)m.LParam ) == (int)Msg.WM_LBUTTONDOWN )
			{
				m.Result = (IntPtr)MouseActivateFlags.MA_ACTIVATE;
				bResult = true;
			}
			return bResult;
		}
		/// <summary>
		/// Checks next TabItem on the right of/on the left of currently selected one.
		/// </summary>
		/// <param name="delta"> Value that indicates which TabItem must be selected next. </param>
		internal void CheckNextTab( int delta )
		{
			bool bScrollToRight = ( this.RightToLeft == RightToLeft.Yes ) ? ( delta > 0 ) : ( delta < 0 );
			ToolStripTabItem tabItem = GetNextTab( m_checkedItem, bScrollToRight );

			if( tabItem != null )
			{
				tabItem.Checked = true;
			}
		}
		/// <summary>
		/// Method tries to get next available item.
		/// </summary>
		/// <param name="itemFrom"> Checked item. </param>
		/// <param name="bForward"> Direction for item search. </param>
		/// <returns></returns>
		internal ToolStripTabItem GetNextTab( ToolStripTabItem itemFrom, bool bForward )
		{
			ToolStripTabItem previousItem = null;

			for( int i = 0, count = this.m_mainItems.Count; i < count; i++ )
			{
				ToolStripTabItem item = this.m_mainItems[i] as ToolStripTabItem;

				if( item != null && IsVisible( item ) )
				{
					if( bForward )
					{
						if( previousItem == itemFrom )
						{
							return item;
						}
					}
					else
					{
						if( item == itemFrom )
						{
							return previousItem;
						}
					}
					previousItem = item;
				}
			}
			return null;
		}
		/// <summary>
		/// Checks TabItem visibility.
		/// </summary>
		/// <param name="item"> TabItem to check. </param>
		/// <returns></returns>
		private bool IsVisible( ToolStripTabItem item )
		{
			bool bResult = item.Available;
			if( bResult )
			{
				ToolStripTabGroup tabGroup = GetItemGroup( item );
				if( tabGroup!=null && !tabGroup.Visible )
				{
					bResult = false;
				}
			}
			return bResult;
		}
		/// <summary>
		/// Method scroll TabItem to show it at full length.
		/// </summary>
		/// <param name="tabItem"> TabItem to scroll. </param>
		private void ScrollTabItem( ToolStripTabItem tabItem )
		{
			// Get rectangle for TabItems on RibbonAdvHeader.
			Rectangle rcTabItems = TabItemsRectangle;

			if( m_bIsLeftScroll )
			{
				rcTabItems.X += SCROLL_BUTTON_WIDTH;
				rcTabItems.Width -= SCROLL_BUTTON_WIDTH;
			}

			if( m_bIsRightScroll )
			{
				rcTabItems.Width -= SCROLL_BUTTON_WIDTH;
			}

			Rectangle rcItem = tabItem.Bounds;

			// Scroll TabItems if needed to show TabItem at full length.
			if( rcItem.X < rcTabItems.X ) // Item located to the left of TabItemsRectangle.
			{
				int iChangePosition = rcItem.Left - rcTabItems.X;
				ScrollPositionInternal = GetValidScrollPosition( ScrollPositionInternal + iChangePosition );
			}
			else if( rcItem.Right > rcTabItems.Right ) // Item located to the right of TabItemsRectangle.
			{
				int iChangePosition = ( rcItem.Right - rcTabItems.X ) - rcTabItems.Width;
				ScrollPositionInternal = GetValidScrollPosition( ScrollPositionInternal + iChangePosition );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sc"></param>
		private void ResizeForm( int szFlag, IntPtr lParam )
		{
			if( this.Form.WindowState == FormWindowState.Normal )
			{
				WindowStyles style = (WindowStyles)WindowsAPI.GetWindowLong( this.Form.Handle, (int)SetWindowLongOffsets.GWL_STYLE );

				if( ( style & WindowStyles.WS_SIZEBOX ) == WindowStyles.WS_SIZEBOX )
				{
					WindowsAPI.PostMessage( this.Form.Handle, (int)Msg.WM_SYSCOMMAND, (int)SystemCommand.SC_SIZE | szFlag, (int)lParam );
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hWnd"></param>
		private void MoveForm( IntPtr lParam )
		{
			if( this.Form.WindowState == FormWindowState.Normal )
			{
				WindowsAPI.PostMessage( this.Form.Handle, (int)Msg.WM_SYSCOMMAND, (int)SystemCommand.SC_MOVE | 2, (int)lParam );
			}
		}
		/// <summary>
		/// Method tries to check next visible TabItem. Getting started from current TabItem 
		/// method goes forward to last TabItem, then if no one TabItem is checked it goes back to first TabItem. 
		/// If no one is checked in this case then TabItem will check itself if it's visible.
		/// </summary>
		private void TryCheckNextTabItem( ToolStripTabItem pTabItem )
		{
			int iIndex = this.MainItems.IndexOf( pTabItem );

			if( iIndex != -1 )
			{
				for( int i = iIndex + 1, count = this.MainItems.Count; i < count; i++ )
				{
					if( TryCheckNextTabItem( i ) )
						return;
				}

				for( int i = iIndex - 1; i >= 0; i-- )
				{
					if( TryCheckNextTabItem( i ) )
						return;
				}

				if( pTabItem.Visible )
					pTabItem.Checked = true;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="iIndex"></param>
		/// <returns></returns>
		private bool TryCheckNextTabItem( int iIndex )
		{
			bool bIsChecked = false;

			ToolStripTabItem item = this.MainItems[iIndex] as ToolStripTabItem;

			if( item != null && item.Visible )
			{
				item.Checked = true;
				bIsChecked = true;
			}

			return bIsChecked;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="items"></param>
		private void Dispose( ToolStripItemCollection items )
		{
			for( int i = items.Count - 1; i >= 0; i-- )
			{
				items[i].Dispose();
			}

			items.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		internal void ShowSystemMenu()
		{
			Rectangle rc = this.DisplayRectangle;

			int y = rc.Y + this.QuickPanelHeight;
			int x;

			if( this.RightToLeft == RightToLeft.Yes )
			{
				Size szMenu = this.SystemMenu.GetPreferredSize( Size.Empty );
				x = rc.Right-szMenu.Width;
			}
			else
			{
				x = rc.X;
			}

			ShowSystemMenu( PointToScreen( new Point( x, y ) ) );

			if( this.SystemMenu.Items.Count > 0 )
			{
				this.SystemMenu.Items[0].Select();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pt"></param>
		internal void ShowSystemMenu( Point pt )
		{
			if( m_bShowContextMenu )
			{
				this.SystemMenu.UpdateItems( this.Renderer );
				this.SystemMenu.Show( pt );
			}
		}
		#endregion

		#region ShouldSerialize & Reset Methods
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool ShouldSerializeFont()
		{
			return base.Font == Control.DefaultFont;
		}
		/// <summary>
		/// 
		/// </summary>
		public override void ResetFont()
		{
			base.Font = Control.DefaultFont;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		internal bool ShouldSerializeTitleFont()
		{
			return m_titleFont != null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		internal void ResetTitleFont()
		{
			this.TitleFont = null;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Specifies whether QuickItemsDropDownButton need to be shown.
		/// </summary>
		[DefaultValue(true)]
		internal bool ShowQuickItemsDropDownButton
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
					this.SetDisplayedItems();
				}
			}
		}
		/// <summary>
		/// Gets or sets the font of the text displayed by the control.
		/// </summary>
		[Description( "The font used to display tesxt in control" )]
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
		/// <summary>
		/// Gets or sets currently selected tab
		/// </summary>
		[Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public ToolStripTabItem SelectedTab
		{
			get 
			{ 
				return m_checkedItem;
			}
			set
			{
				if (m_mainItems.Contains(value) && !value.Checked)
				{
					value.Checked = true;
					m_checkedItem = value;
					OnTabCheckStateChanged(value, EventArgs.Empty);
				}
			}
		}
		/// <summary>
		/// Gets or sets the dropdown shown when menu button is clicked.
		/// </summary>
		[Description( "Gets or sets the dropdown shown when menu button is clicked." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public ToolStripDropDown MenuButtonDropDown
		{
			set
			{
				m_menuButton.DropDown = value;
			}
			get
			{
				return m_menuButton.DropDown;
			}
		}
		/// <summary>
		/// Gets or sets width of menu button.
		/// </summary>
		[Description( "Gets or sets width of menu button." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public int MenuButtonWidth
		{
			get
			{
				return m_menuButtonWidth;
			}
			set
			{
				m_menuButtonWidth = value;

				PerformLayout();
			}
		}
		/// <summary>
		/// Gets or sets the image of the menu main button.
		/// </summary>
		[Description( "Gets or sets the image of the menu button." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public Image MenuButtonImage
		{
			get
			{
				return m_menuButton.Image;
			}
			set
			{
				m_menuButton.Image = value;
			}
		}
        /// <summary>
        /// Gets or sets sets the text font  of the menu main button.
        /// </summary>
        [Description("Gets or sets the text font of the menu button.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Font MenuButtonFont
        {
            get
            {
                return m_menuButton.Font;
            }
            set
            {
                m_menuButton.Font = value;
            }
        }
		/// <summary>
		/// Gets or sets the text of the menu main button.
		/// </summary>
		[Description("Gets or sets the text of the menu button.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string MenuButtonText
		{
			get
			{
				return m_menuButton.Text;
			}
			set
			{
				m_menuButton.Text = value;
			}
		}
		/// <summary>
		/// Gets or sets visibility of menu button.
		/// </summary>
		[Description( "Gets or sets visibility of menu button." )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool MenuButtonVisible
		{
			get
			{
				return m_menuButton.Available;
			}
			set
			{
				m_menuButton.Available = value;
			}
		}
		/// <summary>
		/// List of objects storing info about items of quick panel.
		/// </summary>
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ), Browsable( false )]
		public ObservableList<ToolStripItem> QuickItems
		{
			get
			{
				return m_quickItems;
			}
		}
		/// <summary>
		/// List of objects storing info about top items.
		/// </summary>
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ), Browsable( false )]
		public ObservableList<ToolStripItem> MainItems
		{
			get
			{
				return m_mainItems;
			}
		}
		/// <summary>
		/// Specifies the title of the form in the RibbonControlAdv header.
		/// </summary>
		[Description( "Specifies the title of the form in the RibbonControlAdv header." )]
		public string Title
		{
			get
			{
				if( m_form != null && m_form.Appearance == RibbonForm.AppearanceType.Office2007 )
				{
					return m_form.Text;
				}
				return string.Empty;
			}
		}

		/// <summary>
		/// Specifies the color of the TittleText in the RibbonControlAdv header.
		/// </summary>
		[Description("Specifies the color of the TittleText in the RibbonControlAdv header.")]
		public Color TitleColor
		{
			get
			{
				if (m_owner != null)
				{
					return m_owner.TitleColor;
				}
				return Color.Empty;
			}
		}

		/// <summary>
		/// Gets or sets the form title alignment in RibbonControlAdv header.
		/// </summary>
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		[Description( "Gets or sets the form title alignment in RibbonControlAdv header." )]
		public TextAlignment TitleAlignment
		{
			get
			{
				return m_titleAlignment;
			}
			set
			{
				if( m_titleAlignment != value )
				{
					m_titleAlignment = value;

					PerformLayout();
					Invalidate();
				}
			}
		}
		/// <summary>
		/// Gets or sets the font settings of the form title in RibbonControlAdv header.
		/// </summary>
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		[Description( "Gets or sets the font settings of the form title in RibbonControlAdv header." )]
		public Font TitleFont
		{
			get
			{
				if( m_titleFont == null )
				{
					if (this.m_owner != null)
						return this.m_owner.CaptionFont;
					else
						return this.Font;
				}
				return m_titleFont;
			}
			set
			{
				if( m_titleFont != value )
				{
					m_titleFont = value;
					PerformLayout();
					Invalidate();
				}
			}
		}
		/// <summary>
		/// Gets or sets control fore color.
		/// </summary>
		[Category( "Appearance" ), Description( "Gets or sets control fore color." )]
		public new virtual Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				if( m_foreColor != value )
				{
					m_foreColor = value;
					base.ForeColor = m_foreColor.IsEmpty ? this.DefForeColor : m_foreColor;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private Color DefForeColor
		{
			get
			{
				ToolStripProfessionalRenderer renderer = this.Renderer as ToolStripProfessionalRenderer;

				if( renderer != null )
				{
					Office12ColorTable officeColorTable = renderer.ColorTable as Office12ColorTable;

					if( officeColorTable != null )
					{
						return officeColorTable.RibbonTabText;
					}
				}
				return Color.Empty;
			}
		}
		/// <summary>
		/// Quick items' customize menu
		/// </summary>
		internal QuickItemsDropDownButton QuickAccessButton
		{
			get
			{
				if( m_quickDropDownButton == null )
				{
					m_quickDropDownButton = new QuickItemsDropDownButton( this );

					ToolTipInfo ttInfo = new ToolTipInfo();
					ttInfo.Header.Hidden = true;
					ttInfo.Footer.Hidden = true;
					ttInfo.Body.Text = "Customize Quick Access Toolbar";

					this.ToolTip.SetToolTip( m_quickDropDownButton, ttInfo );
				}
				return m_quickDropDownButton;
			}
		}
		/// <summary>
		/// Overflow button to show hidden items.
		/// </summary>
		internal QuickItemsOverflowButton QuickOverflowButton
		{
			get
			{
				if( m_quickOverflowButton == null )
				{
                    try
                    {
                        m_quickOverflowButton = new QuickItemsOverflowButton(this);
                    }
                    catch  { }
					ToolTipInfo ttInfo = new ToolTipInfo();
					ttInfo.Header.Hidden = true;
					ttInfo.Footer.Hidden = true;
					ttInfo.Body.Text = this.OverFlowButtonToolTip;

					this.ToolTip.SetToolTip( m_quickOverflowButton, ttInfo );

					if( m_accelerator != null )
					{
						m_accelerator.SetAccelerator( m_quickOverflowButton, OVERFLOW_ACCELERATOR );
					}
				}
				return m_quickOverflowButton;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal RibbonStyle RibbonStyle
		{
			get 
			{ 
				if(this.m_owner!=null)
					return this.m_owner.RibbonStyle;

				return RibbonStyle.Office2007;
			}
		}
		/// <summary>
		///
		/// </summary>
		public BackStageView BackStageView
		{
			get { return backStageView; }
			set
			{
				if (backStageView != null)
					backStageView.ProvideBackStageBounds -= new ProvideBoundsEventHandler(OnProvideBackStageBounds);

				backStageView = value;

				if(backStageView!=null)
					backStageView.ProvideBackStageBounds += new ProvideBoundsEventHandler(OnProvideBackStageBounds);
			}
		}

		void OnProvideBackStageBounds(object sender, ProvideBoundsEventArgs e)
		{
			int top = 0, left = 0, width = 0, height = 0;
			
			top = this.Top + this.DisplayRectangle.Top + this.QuickPanelHeight + this.TabItemsRectangle.Height + 7;
			
			Form form = (this.Form != null) ? this.Form : this.FindForm();

			if (form != null)
			{
				width = form.Width + 1 - 2 * this.BorderSize.Width;
				height = form.Height + 1 - top - this.BorderSize.Width;
			}

			if (this.Form != null)
			{
				if (!Form.CompositionEnabled)
					left += this.BorderSize.Width;
				else
					top += 3;
			}

			e.Location = new Point(left, top);
			e.Size = new Size(width, height);
		}
		/// <summary>
		/// 
		/// </summary>
		internal ToolStripMenuButton MenuButton
		{
			get
			{
				return m_menuButton;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal RibbonForm Form
		{
			get
			{
				return m_form;
			}
			set
			{
				if( m_form != value )
				{
					if( m_form != null )
					{
						m_form.TextChanged -= new EventHandler( OnFormTextChanged );
						m_form.HandleCreated -= new EventHandler( OnFormHandleCreated );
						m_form.HandleDestroyed -= new EventHandler( OnFormHandleDestroyed );
					}

					m_form = value;

					if( m_form != null )
					{
						m_form.TextChanged += new EventHandler( OnFormTextChanged );

						if( m_form.TopLevel )
						{
							m_form.HandleCreated += new EventHandler( OnFormHandleCreated );
							m_form.HandleDestroyed += new EventHandler( OnFormHandleDestroyed );

							if( m_form.IsHandleCreated )
							{
								OnFormHandleCreated( m_form, EventArgs.Empty );
							}
						}
					}

					UpdateSystemButtons();
				}
			}
		}
		/// <summary>
		/// Gets the height of quick panel.
		/// </summary>
		internal int QuickPanelHeight
		{
			get
			{
				if( m_quickPanelHeight < 0 )
				{
					
                    Bitmap bit = new Bitmap(10, 10);
                    using (Graphics g = Graphics.FromImage(bit))
                    {
                        if (ribbonTouchModeEnabled)
                        {
                            m_quickPanelHeight = DEF_PANEL_HEIGHT_TOUCH;
                        }
                        else
                        {
                            if (g.DpiX > 120)
                                m_quickPanelHeight = DPI_150_DEF_QUICK_HEIGHT;
                            else if (g.DpiX > 96)
                                m_quickPanelHeight = DPI_125_DEF_QUICK_HEIGHT;
                            else
                                m_quickPanelHeight = DEF_QUICK_HEIGHT;
                        }
                    }
                    bit.Dispose();
					// -2 because borders can lay on panel borders.
					int itemsHeight = GetItemsSize( m_quickItems, DEF_QUICK_HEIGHT, false ).Height - 2;

					if( m_bDisplaySysButtons )
					{
						int systemHeight = GetItemsSize( this.SystemButtons ).Height;
						if( itemsHeight < systemHeight )
						{
							itemsHeight = systemHeight;
						}
					}

					if( m_quickPanelHeight < itemsHeight )
					{
						m_quickPanelHeight = itemsHeight;
					}
				}
				return m_quickPanelHeight;
			}
		}
		/// <summary>
		/// Gets the width of quick panel.
		/// </summary>
		internal int QuickPanelWidth
		{
			get
			{
				if( m_quickPanelWidth < 0 )
				{
					m_quickPanelWidth = 0;

					for( int i = 0, count = m_quickItems.Count; i < count; i++ )
					{
						ToolStripItem item = m_quickItems[i];

						if( item != null && item.Available && item.GetCurrentParent() == this )
						{
							m_quickPanelWidth += item.Width;
						}
					}
				}

				if (RibbonStyle == RibbonStyle.Office2007)
					return m_quickPanelWidth;
				else
					return m_quickPanelWidth + 2 * QUICK_ITEM_SEPARATOR_WIDTH + 6;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal int SystemButtonsWidth
		{
			get
			{
				if( m_systemButtonsWidth < 0 )
				{
					m_systemButtonsWidth = 0;

					if( m_bDisplaySysButtons )
					{
						for( int i = 0, count = this.SystemButtons.Count; i < count; i++ )
						{
							ToolStripItem item = this.SystemButtons[i];

							if( item != null && item.Available )
							{
								m_systemButtonsWidth += item.Width;
							}
						}
					}
				}
				return m_systemButtonsWidth;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal SuperAccelerator SuperAccelerator
		{
			get
			{
				return m_accelerator;
			}
			set
			{
				if( m_accelerator != value )
				{
					if( m_quickOverflowButton != null )
					{
						if( m_accelerator != null )
						{
							m_accelerator.SetAccelerator( m_quickOverflowButton, null );
						}
						if( value != null )
						{
							value.SetAccelerator( m_quickOverflowButton, OVERFLOW_ACCELERATOR );
						}
					}
					m_accelerator = value;
				}
			}
		}
        private ToolStripDropDownButton systemMode;
        internal ToolStripDropDownButton SystemMode
        {
            get
            {
                if (systemMode == null)
                {
                    systemMode = new ToolStripDropDownButton();
                    systemMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
                    systemMode.Image = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.tocuhdropdown.png")); 
                    systemMode.ImageTransparentColor = System.Drawing.Color.Magenta;
                    systemMode.Name = "Touch/Mouse Mode";
                    systemMode.Text = "Touch/Mouse Mode";
                    systemMode.Margin = new System.Windows.Forms.Padding(0);
                    systemMode.Padding = new System.Windows.Forms.Padding(0);
                    systemMode.DropDown.Margin = new System.Windows.Forms.Padding(0);
                    systemMode.DropDown.Padding = new System.Windows.Forms.Padding(0);
                    systemMode.DropDownOpening += new EventHandler(systemMode_DropDownOpening);
                    systemMode.DropDownClosed += new EventHandler(systemMode_DropDownClosed);
                }
                return systemMode;
            }
        }
        internal bool dropDownSelected = false;
        internal bool ribbonOptionDropDownSelected = false;
        void systemMode_DropDownClosed(object sender, EventArgs e)
        {
            this.TouchModePop.HidePopup();
        }

        void systemMode_DropDownOpening(object sender, EventArgs e)
        {
            dropDownSelected = true;
            this.TouchModePop.BeforeCloseUp += new CancelEventHandler(TouchModePop_BeforeCloseUp);
            if (this.ShowQuickPanelBelowRibbon)
            {
                this.TouchModePop.ShowPopup(new Point(m_form.Location.X + m_owner.Bounds.X + m_owner.BottomToolstrip.Bounds.X + systemMode.Bounds.Location.X, m_form.Location.Y + m_owner.Bounds.Y + m_owner.BottomToolstrip.Bounds.Y + systemMode.Bounds.Location.Y + systemMode.Height - 1));
            }
            else
            {
                this.TouchModePop.ShowPopup(new Point(m_form.Location.X + m_owner.Bounds.X + systemMode.Bounds.Location.X, m_form.Location.Y + m_owner.Bounds.Y + systemMode.Bounds.Location.Y + systemMode.Height - 1));
            }
        }

        void TouchModePop_BeforeCloseUp(object sender, CancelEventArgs e)
        {
            dropDownSelected = false;
            this.SystemMode.Invalidate();
        }
		/// <summary>
		/// 
		/// </summary>
        internal ToolStripItemCollection SystemButtons
		{
			get
			{
				if( m_systemButtons == null )
				{
					SuspendLayout();
					m_systemButtons = new ToolStripItemCollection( this, new ToolStripItem[] { } );

					foreach( int command in sysCommands )
					{
						SystemButton button = new SystemButton( this, command );

						this.ToolTip.SetToolTip( button, new SystemToolTipInfo() );
						m_systemButtons.Add( button );
					}
					ResumeLayout( false );
				}
				return m_systemButtons;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		ToolStripItemCollection MdiButtons
		{
			get
			{
				if( m_mdiButtons == null )
				{
					m_mdiButtons = new ToolStripItemCollection( this, new ToolStripItem[] { } );

					foreach( int command in sysCommands )
					{
						SystemButton button = new MdiSystemButton( this, command );

						this.ToolTip.SetToolTip( button, new SystemToolTipInfo() );
						m_mdiButtons.Add( button );
					}
				}
				return m_mdiButtons;
			}
		}
        /// <summary>
        /// Gets or sets tooltip for QuickDropDown.
        /// </summary>
        [Description("Gets or sets tooltip for QuickDropDown.")]
        [DefaultValue("Customize Quick Access Toolbar")]
        internal string QuickDropDownToolTipText
        {
            get
            {
                return this.quickDropDownToolTipText;
            }
            set
            {
                if (this.quickDropDownToolTipText != value)
                {
                    this.quickDropDownToolTipText = value;
                }
            }
        }



        /// <summary>
        /// Gets or sets tooltip for Over flow button
        /// </summary>
        [Description("Gets or sets tooltip for Over flow button")]
        [DefaultValue("Show DropDown")]
        public string OverFlowButtonToolTip
        {
            get
            {
                return this.overFlowButtonToolTip;
            }
            set
            {
                if (this.overFlowButtonToolTip != value)
                {
                    this.overFlowButtonToolTip = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets MinimizeButtonToolTip
        /// </summary>
        [Description("Gets or sets tooltip for Minimize button")]
        [DefaultValue("Minimize Ribbon")]
        public string MinimizeToolTip
        {
            get
            {
                return this.minimizeToolTip;
            }
            set
            {
                if (this.minimizeToolTip != value)
                {
                    this.minimizeToolTip = value;
                }
                
            }
        }


        /// <summary>
        /// Gets or sets MaximizeButtonToolTip
        /// </summary>
        [Description("Gets or sets the MaximizeButton Tooltip")]
        [DefaultValue("Maximize Ribbon")]
        public string MaximizeToolTip
        {
            get
            {
                return this.maximizeToolTip;
            }
            set
            {
                if (value == null)
                { 

                }
                if (this.maximizeToolTip != value)
                {
                    this.maximizeToolTip = value;
                }
               
            }
        }

		/// <summary>
		/// 
		/// </summary>
		internal ToolStripItem MinimizeButton
		{
			get
			{
				if (m_minimizeButton == null)
				{
					MinimizeSystemButton button = new MinimizeSystemButton(this);
					
					button.Text = "Minimize Ribbon";

					SystemToolTipInfo info = new SystemToolTipInfo();
					info.Body.Text = "Minimize Ribbon";
					info.Header.Text = "Minimize Ribbon";
					this.ToolTip.SetToolTip(button, info);

					m_minimizeButton = button;
				}

				return m_minimizeButton;
			}
		}
        private string ribbonDisplayOptionToolTip = "Ribbon Display Option";
        internal string RibbonDisplayOptionToolTip
        {
            get
            {
                return ribbonDisplayOptionToolTip;
            }
            set
            {
                if (ribbonDisplayOptionToolTip != value)
                    ribbonDisplayOptionToolTip = value;
            }
        }
        internal ToolStripItem Ribbon2013MinimizeButton
        {
            get
            {
                if (Office2013_minimizeButton == null)
                {
                    Ribbon2013MinimizeSystemButton button = new Ribbon2013MinimizeSystemButton(this);

                    button.Text = "Ribbon Display Option";

                    SystemToolTipInfo info = new SystemToolTipInfo();
                    info.Body.Text = "Ribbon Display Option";
                    info.Header.Text = "Ribbon Display Option";
                    this.ToolTip.SetToolTip(button, info);

                    Office2013_minimizeButton = button;
                }

                return Office2013_minimizeButton;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		ToolStripItem HelpButton
		{
			get
			{
				if( m_helpButton == null )
				{
					SystemButton button = new HelpSystemButton( this, helpCommand );
					if (Form != null)
					{
						button.Image = Form.HelpButtonImage;
						button.Text = Form.HelpButtonToolTip;
					}
					this.ToolTip.SetToolTip( button, new SystemToolTipInfo() );
					m_helpButton = button;
				}

				return m_helpButton;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		SuperToolTip ToolTip
		{
			get
			{
				if( m_toolTip == null )
				{
					m_toolTip = new SuperToolTip( this.TopLevelControl as Form );
					m_toolTip.UpdateToolTip += new UpdateToolTipHandler( OnUpdateToolTip );
				}
				return m_toolTip;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		SystemMenuStrip SystemMenu
		{
			get
			{
				if( m_systemMenu == null )
				{
					m_systemMenu = new SystemMenuStrip( this.Form );
				}
				return m_systemMenu;
			}
		}
		/// <summary>
		/// Gets collection of tab groups.
		/// </summary>
		public TabGroupCollection Groups
		{
			get
			{
				return m_groups;
			}
		}
		/// <summary>
		/// Gets hash of items and tab groups.
		/// </summary>
		internal Dictionary<ToolStripItem, ToolStripTabGroup> TabGroupsHash
		{
			get
			{
				return m_hashGroups;
			}
		}
		/// <summary>
		/// Gets rectangle of the title.
		/// </summary>
		internal Rectangle TitleRect
		{
			get
			{
				return m_rcTitle;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		internal bool HasOverflowItems
		{
			get { return OverflowsItems.Count > 0; }
		}
		/// <summary>
		/// Gets or sets visibility of quick panel.
		/// </summary>
		public bool QuickPanelVisible
		{
			get
			{
				return m_bQuickPanelVisible;
			}
			set
			{
				if( m_bQuickPanelVisible != value )
				{
					m_bQuickPanelVisible = value;
					PerformLayout();
				}
			}
		}
        private bool touchMode = false;
        internal bool TouchMode
        {
            get { return touchMode; }
            set
            {
                touchMode = value;
            }
        }
        public bool HideMenuButtonToolTip
        {
            get { return hideMenuButtonToolTip; }
            set
            {
                hideMenuButtonToolTip = value;
                if (this.HideMenuButtonToolTip)
                    this.ShowItemToolTips = false;
                else
                    this.ShowItemToolTips = true;
            }
        }
		/// <summary>
		/// Gets or sets value indicating whether quick access toolbar should be shown below ribbon.
		/// </summary>
		public bool ShowQuickPanelBelowRibbon
		{
			get
			{
				return m_bShowQuickPanelBelowRibbon;
			}
			set
			{
				if( m_bShowQuickPanelBelowRibbon != value )
				{
					m_bShowQuickPanelBelowRibbon = value;
					PerformLayout();
				}
			}
		}
		/// <summary> Gets or sets rectangle for tabItems. </summary>
		public Rectangle TabItemsRectangle
		{
			get
			{
				return m_rcTabItems;
			}
			set
			{
				m_rcTabItems = value;
			}
		}

		/// <summary> Gets or sets value indicating whether should show the context menu in Ribbon header. </summary>
		public bool ShowContextMenu
		{
			get
			{
				return m_bShowContextMenu;
			}
			set
			{
				m_bShowContextMenu = value;
			}
		}
		/// <summary> Get bounds of right scroll button. </summary>
		protected Rectangle RightScrollBounds
		{
			get
			{
				Rectangle rc = Rectangle.Empty;

				if( m_bIsRightScroll )
				{
					rc = new Rectangle( TabItemsRectangle.Right - SCROLL_BUTTON_WIDTH, TabItemsRectangle.Top, SCROLL_BUTTON_WIDTH, TabItemsRectangle.Height );
				}

				return rc;
			}
		}
		/// <summary> Get bounds of left scroll button. </summary>
		protected Rectangle LeftScrollBounds
		{
			get
			{
				Rectangle rc = Rectangle.Empty;

				if( m_bIsLeftScroll )
				{
					rc = new Rectangle( TabItemsRectangle.Left, TabItemsRectangle.Top, SCROLL_BUTTON_WIDTH, TabItemsRectangle.Height );
				}

				return rc;
			}
		}
		/// <summary> Gets value that indicates if right scroll button is selected. </summary>
		internal bool RightScrollSelected
		{
			get
			{
				return m_bRightScrollSelected;
			}
		}
		/// <summary> Gets value that indicates if left scroll button is selected. </summary>
		internal bool LeftScrollSelected
		{
			get
			{
				return m_bLeftScrollSelected;
			}
		}
		/// <summary> Area in which user pushed mouse button. </summary>
		protected ScrollButtonsArea PushedButton
		{
			get
			{
				return m_pushedButton;
			}
			set
			{
				if( m_pushedButton != value )
				{
					m_pushedButton = value;
				}
			}
		}
		/// <summary> Gets or sets position of rightmost tab Item. </summary>
		internal int ScrollPositionInternal
		{
			get
			{
				return m_iScrollPosition;
			}
			set
			{
				m_iScrollPosition = value;

				//Invalidate( TabItemsRectangle );
				PerformLayout();
			}
		}
		/// <summary> Gets or sets position of rightmost tab Item and transfer it value to ScrollPosition property. </summary>
		[Browsable( false )]
		public int ScrollPosition
		{
			get
			{
				return m_iScrollPosition;
			}
			set
			{
				ScrollPositionInternal = GetValidScrollPosition( value );
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

				if (m_owner!=null && m_owner.RibbonStyle == RibbonStyle.Office2007)
				{
					Size szBorder = this.BorderSize;

					rc.X += szBorder.Width;
					rc.Y += szBorder.Height;
					rc.Width -= 2 * szBorder.Width;
					rc.Height -= szBorder.Height;
				}
				else
				{
					rc.Height -= 4;
				}

				return rc;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private Size BorderSize
		{
			get
			{
			 if (m_owner != null)
			{
				if( m_owner.IsFormManager )
				{
					if (m_owner.RibbonStyle == RibbonStyle.Office2007)
						return new Size(6, 6);
					else
						return new Size(6, 3);
				}
			}
				return Size.Empty;
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
		RibbonControlAdv m_owner;
		/// <summary>
		/// 
		/// </summary>
		RibbonForm m_form;
		/// <summary>
		/// Collection of ToolStripItem instances with data about items in quick panel.
		/// </summary>
		private QuickItemsCollection m_quickItems;
		/// <summary>
		/// Collection of ToolStripItem instances with data about items in main panel.
		/// </summary>
		private MainItemsCollection m_mainItems;
        /// <summary>
        /// Collection of visible ToolStripItem instances with data about items in main panel.
        /// </summary>
        private MainItemsCollection visibleTabItems;
		/// <summary>
		/// Menu button.
		/// </summary>
		private ToolStripMenuButton m_menuButton;
        /// <summary>
        /// Menu button.
        /// </summary>
        private ToolStripMenuButton m_backStageMenuButton;
		/// <summary>
		/// 
		/// </summary>
		private BackStageView backStageView;
		/// <summary>
		/// 
		/// </summary>
		private ToolStripItemCollection m_systemButtons;
		/// <summary>
		/// 
		/// </summary>
		MinimizeSystemButton m_minimizeButton;
        /// <summary>
        /// 
        /// </summary>
        Ribbon2013MinimizeSystemButton Office2013_minimizeButton;
        /// <summary>
        /// 
        /// </summary>
        Panel ribbon2013PopUpPanel;
		/// <summary>
		/// 
		/// </summary>
		private ToolStripItemCollection m_mdiButtons;
		/// <summary>
		/// 
		/// </summary>
		private ToolStripItem m_helpButton;
		/// <summary>
		/// 
		/// </summary>
		private QuickItemsDropDownButton m_quickDropDownButton;
		/// <summary>
		/// 
		/// </summary>
		private QuickItemsOverflowButton m_quickOverflowButton;
		/// <summary>
		/// Instance of RibbonControlAdvLayoutEngine.
		/// </summary>
		private RibbonControlAdvLayoutEngine m_layoutEngine;
		/// <summary>
		/// Width of menu button.
		/// </summary>
		private int m_menuButtonWidth = RibbonControlAdv.DEF_MENU_BUTTON_WIDTH;
		/// <summary>
		/// Height of quick panel.
		/// </summary>
		private int m_quickPanelHeight = -1;
		/// <summary>
		/// Width of the quick panel
		/// </summary>
		private int m_quickPanelWidth = -1;
		/// <summary>
		/// 
		/// </summary>
		private int m_systemButtonsWidth = -1;
		/// <summary>
		/// Indicates whether control is ready for custom layout.
		/// </summary>
		private bool m_bReadyForLayout = false;
		/// <summary>
		/// Current checked tab item.
		/// </summary>
		private ToolStripTabItem m_checkedItem;
		/// <summary>
		/// 
		/// </summary>
		private SuperToolTip m_toolTip;
		/// <summary>
		/// 
		/// </summary>
		private SystemMenuStrip m_systemMenu;
		/// <summary>
		/// 
		/// </summary>
		private TextAlignment m_titleAlignment = TextAlignment.Left;
		/// <summary>
		/// 
		/// </summary>
		private Font m_titleFont = null;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bDisplaySysButtons = false;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bDisplayMdiButtons = false;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bDisplayMinimizeButton = false;
        /// <summary>
        /// 
        /// </summary>
        private string minimizeToolTip="Minimize Ribbon";
        /// <summary>
        /// 
        /// </summary>
        private string maximizeToolTip="Maximize Ribbon";
		/// <summary>
		/// 
		/// </summary>
		private bool m_bDisplayHelpButton = false;
		/// <summary>
		/// 
		/// </summary>
		private Color m_foreColor = Color.Empty;
		/// <summary>
		/// Collection of tab groups.
		/// </summary>
		private TabGroupCollection m_groups;
		/// <summary>
		/// Dictionary with tab items and tab groups.
		/// </summary>
		private Dictionary<ToolStripItem, ToolStripTabGroup> m_hashGroups;
		/// <summary>
		/// Rectangle of title.
		/// </summary>
		private Rectangle m_rcTitle = Rectangle.Empty;
		/// <summary>
		/// Indicates visibility of quick panel.
		/// </summary>
		private bool m_bQuickPanelVisible = true;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bShowQuickPanelBelowRibbon = false;
		/// <summary>
		/// Fake item to return for invisible items during filtering.
		/// </summary>
		private ToolStripItem m_fakeHiddenItem;
		/// <summary>
		/// Indicates whether items should be filtered.
		/// </summary>
		private bool m_bHideItems = false;
		/// <summary>
		/// Collection of filtered items.
		/// </summary>
		private ToolStripItemCollectionFiltered m_filteredItems;
		/// <summary>
		/// Collection of overflow items.
		/// </summary>
		private ToolStripItemCollection m_overflowItems;
		/// <summary>
		/// Rectangle in which tab items must be shown.
		/// </summary>
		private Rectangle m_rcTabItems = Rectangle.Empty;
		/// <summary>
		/// 
		/// </summary>
		private bool m_bShowContextMenu = true;

		/// <summary>
		/// 
		/// </summary>
		private SuperAccelerator m_accelerator;

		static int[] sysCommands =
		{
			(int)SystemCommand.SC_MINIMIZE,
			(int)SystemCommand.SC_MAXIMIZE,
			(int)SystemCommand.SC_CLOSE
		};
		/// <summary>
		/// 
		/// </summary>
		static int helpCommand = (int)SystemCommand.SC_CONTEXTHELP;
		/// <summary>
		/// Position of rightmost TabItem.
		/// </summary>
		private int m_iScrollPosition = 0;
		/// <summary>
		/// Indicates if right scroll bar is shown.
		/// </summary>
		private bool m_bIsRightScroll;
		/// <summary>
		/// Indicates if left scroll bar is shown.
		/// </summary>
		private bool m_bIsLeftScroll;
		/// <summary>
		/// Indicates if right scroll button is selected.
		/// </summary>
		private bool m_bRightScrollSelected = false;
		/// <summary>
		/// Indicates if left scroll button is selected.
		/// </summary>
		private bool m_bLeftScrollSelected = false;
		/// <summary>
		/// Timer for handling mouse keeping pushed.
		/// </summary>
		private Timer m_timer;
		/// <summary>
		/// Interval for timer.
		/// </summary>
		private int m_timerInt = TIMER_INT;
		/// <summary>
		/// Currently pushed button.
		/// </summary>
		private ScrollButtonsArea m_pushedButton;
		/// <summary>
		/// Selected Tab item.
		/// </summary>
		private ToolStripTabItem m_SelectedTabItem = null;
		/// <summary>
		/// List that contain separators X position.
		/// </summary>
		private List<int> m_iSepatators = new List<int>();
		/// <summary>
		/// Brush factor value to paint separator between Tab items.
		/// </summary>
		private float m_fFactor = 0.0f;
		/// <summary>
		/// Indicates if separators between Tab items must be drawn.
		/// </summary>
		private bool m_bDrawSeparators = false;
        private bool hideMenuButtonToolTip = false;
        internal bool mouseclick = false;
		#endregion

		#region ILayoutSupport Members
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public new void SetItemLocation( ToolStripItem item, Point location )
		{
			if( item.Owner == this )
			{
                if(this.m_owner.Parent is RibbonForm || this.RibbonStyle == Tools.RibbonStyle.Office2007)
					base.SetItemLocation( item, location );
                else
					base.SetItemLocation( item, new Point(location.X,location.Y + 3) );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public ToolStripItem GetItem( ToolStripItem item )
		{
			bool bHide = !IsVisibleGroup( item ) || ( GetItemParent( item ) != this );

			if( !bHide && !this.QuickPanelVisible )
			{
				foreach( ToolStripItem quickItem in this.QuickItems )
				{
					if( quickItem == item )
					{
						bHide = true;
						break;
					}
				}
			}

			return ( bHide ) ? ( m_fakeHiddenItem ) : ( item );
		}
		#endregion

		#region IMessageFilter Members
		/// <summary>
		/// Processing mouse messages from parents if header is disabled
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		bool IMessageFilter.PreFilterMessage( ref Message m )
		{
			bool bResult = false;

			if( m_owner != null && m_owner.IsFormManager && !this.Enabled )
			{
				if( IsParent( m.HWnd ) )
				{
					switch( (Msg)m.Msg )
					{
						case Msg.WM_MOUSEMOVE:
						{
							POINT pt = new POINT();

							pt.x = WindowsAPI.LOW_ORDER( (int)m.LParam );
							pt.y = WindowsAPI.HIGH_ORDER( (int)m.LParam );

							WindowsAPI.MapWindowPoints( m.HWnd, this.Handle, ref pt, 1 );

							if( this.ClientRectangle.Contains( pt.x, pt.y ) )
							{
								base.OnMouseMove( new MouseEventArgs( MouseButtons.None, 0, pt.x, pt.y, 0 ) );
							}
						}
						break;
						case Msg.WM_MOUSELEAVE:
						base.OnMouseLeave( EventArgs.Empty );
						break;
						case Msg.WM_LBUTTONDOWN:
						{
							POINT pt = new POINT();

							pt.x = WindowsAPI.LOW_ORDER( (int)m.LParam );
							pt.y = WindowsAPI.HIGH_ORDER( (int)m.LParam );

							WindowsAPI.MapWindowPoints( m.HWnd, IntPtr.Zero, ref pt, 1 );
							Point ptScr = (Point)pt;

							if( GetHitTest( ref ptScr ) )
							{
								MoveForm( (IntPtr)WindowsAPI.MAKELONG( ptScr.X, ptScr.Y ) );
								bResult = true;
							}
							else
							{
								WindowsAPI.MapWindowPoints( IntPtr.Zero, this.Handle, ref pt, 1 );
								if( GetItemAt( pt.x, pt.y ) is SystemButton )
								{
									base.OnMouseDown( new MouseEventArgs( MouseButtons.Left, 0, pt.x, pt.y, 0 ) );
									bResult = true;
								}
							}
						}
						break;
						case Msg.WM_LBUTTONUP:
						{
							POINT pt = new POINT();

							pt.x = WindowsAPI.LOW_ORDER( (int)m.LParam );
							pt.y = WindowsAPI.HIGH_ORDER( (int)m.LParam );

							WindowsAPI.MapWindowPoints( m.HWnd, this.Handle, ref pt, 1 );

							base.OnMouseUp( new MouseEventArgs( MouseButtons.Left, 0, pt.x, pt.y, 0 ) );
						}
						break;
						case Msg.WM_LBUTTONDBLCLK:
						{
							POINT pt = new POINT();

							pt.x = WindowsAPI.LOW_ORDER( (int)m.LParam );
							pt.y = WindowsAPI.HIGH_ORDER( (int)m.LParam );

							WindowsAPI.MapWindowPoints( m.HWnd, IntPtr.Zero, ref pt, 1 );
							Point ptScr = (Point)pt;

							if( GetHitTest( ref ptScr ) )
							{
								WindowStyles style = (WindowStyles)WindowsAPI.GetWindowLong( this.Form.Handle, (int)SetWindowLongOffsets.GWL_STYLE );

								if( ( style & WindowStyles.WS_SIZEBOX ) == WindowStyles.WS_SIZEBOX )
								{
									SystemCommand sc = ( style & WindowStyles.WS_MAXIMIZE ) == WindowStyles.WS_MAXIMIZE ? SystemCommand.SC_RESTORE : SystemCommand.SC_MAXIMIZE;

									BeginInvoke( new SendMessageDelegate( WindowsAPI.SendMessage ), new object[] { this.Form.Handle, (int)Msg.WM_SYSCOMMAND, sc, (int)m.LParam } );
								}
								bResult = true;
							}
						}
						break;
					}
				}
			}
			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="intPtr"></param>
		/// <returns></returns>
		bool IsParent( IntPtr hWnd )
		{
			if( this.Form.IsHandleCreated && WindowsAPI.IsChild( hWnd, this.Handle ) )
			{
				return this.Form.Handle == hWnd || WindowsAPI.IsChild( this.Form.Handle, hWnd );
			}
			return false;
		}
		#endregion
	}
	#endregion
    /// <summary>
    /// 
    /// </summary>
    public enum ControlItems
    {
        /// <summary>
        /// AutoHide Mode
        /// </summary>
        AutoHide,
        /// <summary>
        /// MinimizePanel Mode
        /// </summary>
        RibbonMinimizePanel,
        /// <summary>
        /// RibbonPanel Mode
        /// </summary>
        RibbonPanel
    }
    public class ImageButtonItem : ToolStripControlHost
    {
        private ImageButton imgButton;

        public ImageButtonItem()
            : base(new ImageButton())
        {
            this.imgButton = this.Control as ImageButton;
        }

        // Add properties, events etc. you want to expose...
    }
     [ToolboxItem(false)]
    public partial class ImageButton : PictureBox, IButtonControl
    {
        #region Consturctor
        public ImageButton()
        {
            this.BackColor = Color.Transparent;
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
        }
        public ImageButton(IContainer container)
        {
            container.Add(this);
        }
        #endregion
        private bool isDefault = false;
        private bool isHover = false;
        private bool isDown = false;


        #region IButtonControl Members

        private DialogResult m_DialogResult;
        public DialogResult DialogResult
        {
            get
            {
                return m_DialogResult;
            }
            set
            {
                m_DialogResult = value;
            }
        }

        public void NotifyDefault(bool value)
        {
            isDefault = value;
        }

        public void PerformClick()
        {
            base.OnClick(EventArgs.Empty);
        }

        #endregion

        #region ImageState
        private Image m_HoverImage;
        public Image HoverImage
        {
            get { return m_HoverImage; }
            set 
            { 
                m_HoverImage = value; 
                if (isHover) Image = value; 
            }
        }

        private Image m_DownImage;
        public Image DownImage
        {
            get { return m_DownImage; }
            set 
            { 
                m_DownImage = value; 
                if (isDown) Image = value; 
            }
        }
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
        }
        private Image m_NormalImage;
        public Image NormalImage
        {
            get { return m_NormalImage; }
            set 
            { 
                m_NormalImage = value; 
                if (!(isHover || isDown)) Image = value; 
            }
        }

        #endregion

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        #region Events
        protected override void OnMouseMove(MouseEventArgs e)
        {
            isHover = true;
            if (isDown)
            {
                if ((m_DownImage != null) && (Image != m_DownImage))
                    Image = m_DownImage;
            }
            else
                if (m_HoverImage != null)
                    Image = m_HoverImage;
                else
                    Image = m_NormalImage;
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isHover = false;
            Image = m_NormalImage;
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            OnMouseUp(null);
            isDown = true;
            if (m_DownImage != null)
                Image = m_DownImage;
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isDown = false;
            if (isHover)
            {
                if (m_HoverImage != null)
                    Image = m_HoverImage;
            }
            else
                Image = m_NormalImage;
            base.OnMouseUp(e);
        }

        private bool holdingSpace = false;

        public override bool PreProcessMessage(ref Message msg)
        {
            if (msg.Msg == WM_KEYUP)
            {
                if (holdingSpace)
                {
                    if ((int)msg.WParam == (int)Keys.Space)
                    {
                        OnMouseUp(null);
                        PerformClick();
                    }
                    else if ((int)msg.WParam == (int)Keys.Escape
                        || (int)msg.WParam == (int)Keys.Tab)
                    {
                        holdingSpace = false;
                        OnMouseUp(null);
                    }
                }
                return true;
            }
            else if (msg.Msg == WM_KEYDOWN)
            {
                if ((int)msg.WParam == (int)Keys.Space)
                {
                    holdingSpace = true;
                    OnMouseDown(null);
                }
                else if ((int)msg.WParam == (int)Keys.Enter)
                {
                    PerformClick();
                }
                return true;
            }
            else
                return base.PreProcessMessage(ref msg);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            holdingSpace = false;
            OnMouseUp(null);
            base.OnLostFocus(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            if ((!string.IsNullOrEmpty(Text)) && (pe != null) && (base.Font != null))
            {
                SolidBrush drawBrush = new SolidBrush(base.ForeColor);
                SizeF drawStringSize = pe.Graphics.MeasureString(base.Text, base.Font);
                PointF drawPoint;
                if (base.Image != null)
                    drawPoint = new PointF(base.Image.Width / 2 - drawStringSize.Width / 2, base.Image.Height / 2 - drawStringSize.Height / 2);
                else
                    drawPoint = new PointF(base.Width / 2 - drawStringSize.Width / 2,  base.Height / 2 - drawStringSize.Height / 2);
                pe.Graphics.DrawString(base.Text, base.Font, drawBrush, drawPoint);
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            Refresh();
            base.OnTextChanged(e);
        }
        #endregion

    }
    [ToolboxItem(false)]
    public class RibbonMinimizePopUp : Syncfusion.Windows.Forms.PopupControlContainer
    {
        private const int CS_DROPSHADOW = 0x00020000;
        protected override CreateParams CreateParams
        {
            get
            {
                // add the drop shadow flag for automatically drawing
                // a drop shadow around the form
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (this.Visible)
            {
                this.BorderStyle = System.Windows.Forms.BorderStyle.None;
                Rectangle rect = e.ClipRectangle;
                rect.Width -= 1;
                rect.Height -= 1;
                e.Graphics.DrawRectangle(new Pen(Color.LightGray), rect);
            }
        }
    }
 
   
    #region *** SystemImages
    class SystemImages
	{
		public const int IMAGE_SIZE = 9;
        public const int DPI_125_IMAGE_SIZE = 14;
        public const int DPI_150_IMAGE_SIZE = 19;
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
        public static Image GetImageClose(Color color, bool touchEnabled)
		{
			Bitmap bmp = new Bitmap( IMAGE_SIZE , IMAGE_SIZE );    
            Bitmap b = new Bitmap(bmp);
            using (Graphics g1 = Graphics.FromImage(b))
            {
                if (g1.DpiX > 120)
                {
                    bmp = new Bitmap(DPI_150_IMAGE_SIZE, DPI_150_IMAGE_SIZE);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        Rectangle rcBmp = new Rectangle(Point.Empty, new Size(DPI_150_IMAGE_SIZE, DPI_150_IMAGE_SIZE));
                        g.FillRectangle(Brushes.Transparent, rcBmp);
                        using (Pen pen = new Pen(Color.Black, 2))
                        {
                            g.DrawLine(pen, new Point(2, 2), new Point(DPI_150_IMAGE_SIZE -2, DPI_150_IMAGE_SIZE -2));
                            g.DrawLine(pen, new Point(2, DPI_150_IMAGE_SIZE-2), new Point(DPI_150_IMAGE_SIZE-2,2));
                        }
                    }
                }
                else if (g1.DpiX > 96 || touchEnabled)
                {
                    bmp = new Bitmap(IMAGE_SIZE + 5, IMAGE_SIZE + 5);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        Rectangle rcBmp = new Rectangle(Point.Empty, new Size(bmp.Size.Width + 5, bmp.Size.Height + 5));
                        g.FillRectangle(Brushes.Transparent, rcBmp);
                        int w = IMAGE_SIZE / 3;
                        using (Region rgn = new Region(Rectangle.Empty))
                        {
                            using (GraphicsPath path = new GraphicsPath())
                            {
                                path.AddPolygon(new Point[] { new Point(0, 1 + 1), new Point(w, 1 + 1), new Point(IMAGE_SIZE + 4, IMAGE_SIZE - 1 + 4 + 1), new Point(IMAGE_SIZE - w + 4, IMAGE_SIZE - 1 + 4 + 1) });
                                rgn.Union(path);
                            }
                            using (GraphicsPath path = new GraphicsPath())
                            {
                                path.AddPolygon(new Point[] { new Point(-1, IMAGE_SIZE - 1 + 4 + 1), new Point(IMAGE_SIZE - w + 4, 1 + 1), new Point(IMAGE_SIZE + 4, 1 + 1), new Point(w - 1, IMAGE_SIZE - 1 + 4 + 1) });
                                rgn.Union(path);
                            }
                            Color clrBegin = color;
                            Color clrEnd = Office12ColorTable.GetAlphaBlendedColor(clrBegin, Color.White, 160);
                            using (Brush brush = new LinearGradientBrush(rcBmp, clrBegin, clrEnd, LinearGradientMode.Vertical))
                            {
                                g.FillRegion(brush, rgn);
                            }
                            using (Region rgHighlight = rgn.Clone())
                            {
                                rgHighlight.Translate(0, 1);
                                rgHighlight.Exclude(rgn); rgHighlight.Exclude(new Rectangle(0, 0, IMAGE_SIZE, IMAGE_SIZE / 2 + 1));
                                using (Brush brush = new SolidBrush(Highlight))
                                {
                                    g.FillRegion(brush, rgHighlight);
                                }
                            }
                        }
                    }
                }
                else
                {
                    bmp = new Bitmap(IMAGE_SIZE, IMAGE_SIZE);

                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        Rectangle rcBmp = new Rectangle(Point.Empty, bmp.Size);

                        g.FillRectangle(Brushes.Transparent, rcBmp);

                        int w = IMAGE_SIZE / 3;

                        using (Region rgn = new Region(Rectangle.Empty))
                        {
                            using (GraphicsPath path = new GraphicsPath())
                            {
                                path.AddPolygon(new Point[] { new Point(0, 1), new Point(w, 1), new Point(IMAGE_SIZE, IMAGE_SIZE - 1), new Point(IMAGE_SIZE - w, IMAGE_SIZE - 1) });
                                rgn.Union(path);
                            }
                            using (GraphicsPath path = new GraphicsPath())
                            {
                                path.AddPolygon(new Point[] { new Point(-1, IMAGE_SIZE - 1), new Point(IMAGE_SIZE - w, 1), new Point(IMAGE_SIZE, 1), new Point(w - 1, IMAGE_SIZE - 1) });
                                rgn.Union(path);
                            }

                            Color clrBegin = color;
                            Color clrEnd = Office12ColorTable.GetAlphaBlendedColor(clrBegin, Color.White, 160);


                            using (Brush brush = new LinearGradientBrush(rcBmp, clrBegin, clrEnd, LinearGradientMode.Vertical))
                            {
                                g.FillRegion(brush, rgn);
                            }

                            using (Region rgHighlight = rgn.Clone())
                            {
                                rgHighlight.Translate(0, 1);

                                rgHighlight.Exclude(rgn);
                                rgHighlight.Exclude(new Rectangle(0, 0, IMAGE_SIZE, IMAGE_SIZE / 2 + 1));

                                using (Brush brush = new SolidBrush(Highlight))
                                {
                                    g.FillRegion(brush, rgHighlight);
                                }
                            }
                        }
                    }
                }
            }
			return bmp;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static Image GetImageMinimize( Color color , bool touchEnabled )
		{
			Bitmap bmp = new Bitmap( IMAGE_SIZE, IMAGE_SIZE );

           //For checking DPI
            Bitmap b = new Bitmap(bmp);
            using (Graphics g1 = Graphics.FromImage(b))
            {
                if (g1.DpiX > 120)
                {
                    bmp = new Bitmap(DPI_150_IMAGE_SIZE, DPI_150_IMAGE_SIZE);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        Rectangle rcBmp = new Rectangle(Point.Empty, new Size(bmp.Size.Width , bmp.Size.Height ));
                        g.FillRectangle(Brushes.Transparent, rcBmp);
                        using (Pen pen = new Pen(color, 1))
                        {
                            g.DrawLine(pen, 1, DPI_150_IMAGE_SIZE - 3, IMAGE_SIZE + 12, DPI_150_IMAGE_SIZE -3);
                        }
                        using (Pen pen = new Pen(Highlight))
                        {
                            g.DrawLine(pen, 1, DPI_150_IMAGE_SIZE - 2, IMAGE_SIZE + 12, DPI_150_IMAGE_SIZE - 2);
                        }
                    }
                }
                else if (g1.DpiX > 96 || touchEnabled)
                {
                    bmp = new Bitmap(DPI_125_IMAGE_SIZE, DPI_125_IMAGE_SIZE);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        Rectangle rcBmp = new Rectangle(Point.Empty, new Size(DPI_125_IMAGE_SIZE, DPI_125_IMAGE_SIZE));
                        g.FillRectangle(Brushes.Transparent, rcBmp);
                        using (Pen pen = new Pen(color , 1))
                        {
                            g.DrawLine(pen, 1, IMAGE_SIZE + 3, IMAGE_SIZE + 12, IMAGE_SIZE + 3);
                        }
                        using (Pen pen = new Pen(Highlight))
                        {
                            g.DrawLine(pen, 1, IMAGE_SIZE - 2 + 3, IMAGE_SIZE + 12, IMAGE_SIZE - 2 + 3);
                        }
                    }
                }
                else
                {
                    bmp = new Bitmap(IMAGE_SIZE, IMAGE_SIZE);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        Rectangle rcBmp = new Rectangle(Point.Empty, bmp.Size);
                        g.FillRectangle(Brushes.Transparent, rcBmp);
                        using (Pen pen = new Pen(color))
                        {
                            g.DrawLine(pen, 1, IMAGE_SIZE - 3, IMAGE_SIZE - 2, IMAGE_SIZE - 3);
                        }
                        using (Pen pen = new Pen(Highlight))
                        {
                            g.DrawLine(pen, 1, IMAGE_SIZE - 2, IMAGE_SIZE - 2, IMAGE_SIZE - 2);
                        }
                    }
                }
            }
			return bmp;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
        public static Image GetImageMaximize(Color color, bool touchEnabled)
		{
			Bitmap bmp = new Bitmap( IMAGE_SIZE, IMAGE_SIZE );

 			Bitmap b = new Bitmap(bmp);
            using (Graphics g1 = Graphics.FromImage(b))
            {
                if (g1.DpiX > 120)
                {
                    bmp = new Bitmap(DPI_150_IMAGE_SIZE, DPI_150_IMAGE_SIZE);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        Rectangle rcBmp = new Rectangle(0, 0, DPI_150_IMAGE_SIZE, DPI_150_IMAGE_SIZE);
                        using (Region rgn = new Region(rcBmp))
                        {
                            rcBmp.Width -= 3;
                            rcBmp.Height -= 6;
                            rcBmp.Y += 4;
                            rcBmp.X += 1;
                            using (Pen pen = new Pen(color, 1))
                            {
                                g.DrawRectangle(pen, rcBmp);
                                g.DrawLine(pen, new Point(0, rcBmp.Y + 2), new Point(rcBmp.Width, rcBmp.Y + 2));
                            }
                            using (Pen pen = new Pen(Highlight))
                            {
                                g.DrawLine(pen, new Point(2, rcBmp.Y + 3), new Point(rcBmp.Width - 1, rcBmp.Y + 3));
                                g.DrawLine(pen, new Point(0, rcBmp.Y + rcBmp.Height + 1), new Point(rcBmp.Width + 1, rcBmp.Y + rcBmp.Height + 1));
                            }
                        }
                    }
                }
                else if (g1.DpiX > 96 || touchEnabled)
                {
                    bmp = new Bitmap(DPI_125_IMAGE_SIZE, DPI_125_IMAGE_SIZE);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        Rectangle rcBmp = new Rectangle(0, 0, DPI_125_IMAGE_SIZE, DPI_125_IMAGE_SIZE);
                        using (Region rgn = new Region(rcBmp))
                        {
                            rcBmp.Width -= 3;
                            rcBmp.Height -= 6;
                            rcBmp.Y += 4;
                            rcBmp.X += 1;
                            using (Pen pen = new Pen(color, 1))
                            {
                                g.DrawRectangle(pen, rcBmp);
                                g.DrawLine(pen, new Point(rcBmp.X, rcBmp.Y + 1), new Point(rcBmp.Width, rcBmp.Y + 1));
                            }
                        }
                    }
                }
                else
                {
                    bmp = new Bitmap(IMAGE_SIZE, IMAGE_SIZE);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        Rectangle rcBmp = new Rectangle(0, 0, IMAGE_SIZE, IMAGE_SIZE - 1);
                        using (Region rgn = new Region(rcBmp))
                        {
                            rgn.Exclude(new Rectangle(1, rcBmp.Height / 2 - 1, rcBmp.Width - 2, rcBmp.Height / 2));
                            using (Brush brush = new SolidBrush(color))
                            {
                                g.FillRegion(brush, rgn);
                            }
                            using (Region rgHighlight = rgn.Clone())
                            {
                                rgHighlight.Translate(0, 1);
                                rgHighlight.Exclude(rgn);
                                using (Brush brush = new SolidBrush(Highlight))
                                {
                                    g.FillRegion(brush, rgHighlight);
                                }
                            }
                        }
                    }
                }
            }
            b.Dispose();
			return bmp;      
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
        public static Image GetImageRestore(Color color, bool touchEnabled)
		{
			Bitmap bmp = new Bitmap( IMAGE_SIZE, IMAGE_SIZE );

            //For checking DPI
            Bitmap b = new Bitmap(bmp);
            using (Graphics g1 = Graphics.FromImage(b))
            {
                if (g1.DpiX > 120)
                {
                    bmp = new Bitmap(DPI_150_IMAGE_SIZE +5, DPI_150_IMAGE_SIZE+5);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        Rectangle rcBmp = new Rectangle(Point.Empty, bmp.Size);
                        //g.FillRectangle( Brushes.Transparent, rcBmp );
                        Rectangle rc = new Rectangle(0, 3, DPI_150_IMAGE_SIZE, DPI_150_IMAGE_SIZE);
                        using (Pen pen = new Pen(color, 1))
                        {
                            using (Pen penHighlight = new Pen(Highlight))
                            {
                                rc.Y -= 1;
                                g.DrawRectangle(pen, rc.X + 3, rc.Y + 5, rc.Width - 7, rc.Height - 7);
                                g.SetClip(new Rectangle(2, rc.Y + 6, rc.Right - 6, rcBmp.Bottom - rc.Y), CombineMode.Exclude);
                                rc.X += 2;
                                rc.Y -= 3;
                                g.DrawRectangle(pen, rc.X + 6, rc.Y + 5, rc.Width - 7, rc.Height - 7);
                            }
                        }
                    }
                }
                else if (g1.DpiX > 96 || touchEnabled)
                {
                    bmp = new Bitmap(DPI_125_IMAGE_SIZE+2, DPI_125_IMAGE_SIZE+2);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        Rectangle rcBmp = new Rectangle(Point.Empty, bmp.Size);
                        //g.FillRectangle( Brushes.Transparent, rcBmp );
                        Rectangle rc = new Rectangle(0, 3, DPI_125_IMAGE_SIZE, DPI_125_IMAGE_SIZE);
                        using (Pen pen = new Pen(color , 1))
                        {
                            using (Pen penHighlight = new Pen(Highlight))
                            {
                                rc.Y -= 1;
                                g.DrawRectangle(pen, rc.X + 3, rc.Y + 5, rc.Width - 7, rc.Height - 7);
                                g.SetClip(new Rectangle(2, rc.Y + 6, rc.Right - 6, rcBmp.Bottom - rc.Y), CombineMode.Exclude);
                                rc.X += 2;
                                rc.Y -= 3;
                                g.DrawRectangle(pen, rc.X + 6, rc.Y + 5, rc.Width - 7, rc.Height - 7);
                            }
                        }
                    }
                }
                else
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        Rectangle rcBmp = new Rectangle(Point.Empty, bmp.Size);
                        g.FillRectangle(Brushes.Transparent, rcBmp);
                        Rectangle rc = new Rectangle(0, 3, IMAGE_SIZE - 2, IMAGE_SIZE - 3);
                        using (Pen pen = new Pen(color))
                        {
                            using (Pen penHighlight = new Pen(Highlight))
                            {
                                g.DrawRectangle(penHighlight, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
                                rc.Y -= 1;
                                g.DrawRectangle(pen, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
                                g.SetClip(new Rectangle(0, rc.Y, rc.Right, rcBmp.Bottom - rc.Y), CombineMode.Exclude);
                                rc.Y -= 1;
                                rc.X += 2;
                                g.DrawRectangle(penHighlight, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
                                rc.Y -= 1;
                                g.DrawRectangle(pen, rc.X, rc.Y, rc.Width - 1, rc.Height - 1);
                            }
                        }
                    }
                }
            }
			return bmp;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static Image GetImageHelp()
		{
            Bitmap bmp = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.HelpButton.bmp"));
			bmp.MakeTransparent( Color.Magenta );

			return bmp;
		}

		public static Image GetOffice2010ImageHelp()
		{
			Bitmap bmp = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.HelpButtonOffice2010.bmp"));

			bmp.MakeTransparent(Color.Magenta);

			return bmp;
		}
        public static Image GetOffice2013ImageHelp()
        {
            Bitmap bmp = new Bitmap(typeof(RibbonControlAdv).Assembly.GetManifestResourceStream("Syncfusion.Windows.Forms.Tools.Controls.RibbonControlAdv.Resources.Help.png"));
            bmp.MakeTransparent(Color.Magenta);
            return bmp;
        }

		#region Properties
		static Color Highlight
		{
			get
			{
				return Color.FromArgb( 160, Color.White );
			}
		}
		#endregion
	}
	#endregion

	#region *** ILayoutSupport
	/// <summary>
	/// Allows class to hide some of toolstrip items. Used in ToolStripItemCollectionFiltered.
	/// </summary>
	internal interface ILayoutSupport
	{
		#region Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		ToolStripItem GetItem( ToolStripItem item );
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		void SetItemLocation( ToolStripItem item, Point location );
		#endregion
	}
	#endregion

	#region *** IRibbonHeader
	public interface IRibbonHeader
	{
		/// <summary>
		/// Adds item to Tabs collection
		/// </summary>
		/// <param name="item"></param>
		void AddMainItem( ToolStripItem item );
		/// <summary>
		/// Adds item to Quick access bar.
		/// </summary>
		/// <param name="item"></param>
		void AddQuickItem( ToolStripItem item );

		/// <summary>
		/// Collection of Quick access items
		/// </summary>
		IRibbonItems QuickItems { get; }
		/// <summary>
		/// Collection of tab items
		/// </summary>
		IRibbonItems MainItems { get; }

		/// <summary>
		/// Occurs when a new item is added to the Quick access bar. 
		/// </summary>
		event ToolStripItemEventHandler QuickItemAdded;

		/// <summary>
		/// Occurs when an item is removed from the Quick access bar. 
		/// </summary>
		event ToolStripItemEventHandler QuickItemRemoved;
	}
	#endregion

	#region Delegates and EventArgs
	/// <summary>
	/// Provides data for SelectedTabChangedEventHandler.
	/// </summary>
	public class SelectedTabChangedEventArgs: EventArgs
	{
		private ToolStripTabItem m_prevSelectedTab;
		private ToolStripTabItem m_newSelectedTab;
		/// <summary>
		/// Previous selected ToolStripTabItem.
		/// </summary>
		public ToolStripTabItem PrevSelectedTab
		{
			get
			{
				return m_prevSelectedTab;
			}
		}
		/// <summary>
		/// New selected ToolStripTabItem.
		/// </summary>
		public ToolStripTabItem NewSelectedTab
		{
			get
			{
				return m_newSelectedTab;
			}
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="prevSelectedTab">Previous selected ToolStripTabItem.</param>
		/// <param name="newSelectedTab">New selected ToolStripTabItem.</param>
		public SelectedTabChangedEventArgs( ToolStripTabItem prevSelectedTab, ToolStripTabItem newSelectedTab )
		{
			m_newSelectedTab = newSelectedTab;
			m_prevSelectedTab = prevSelectedTab;
		}
	}
	/// <summary>
	/// Represents the method that will handle a SelectedTabChanged event.
	/// </summary>
	/// <param name="sender">Control that raises event.</param>
	/// <param name="e">Provides with previous and new selected ToolStripTabItems.</param>
	public delegate void SelectedTabChangedEventHandler( object sender, SelectedTabChangedEventArgs e );
	/// <summary>
	/// Provides data for DropDownEventHandler.
	/// </summary>
	public class DropDownEventArgs: EventArgs
	{
		private ToolStripDropDown m_dropDown;
		/// <summary>
		/// The DropDown that will be shown.
		/// </summary>
		public ToolStripDropDown DropDown
		{
			get
			{
				return m_dropDown;
			}
		}
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="dropDown">The DropDown that will be shown.</param>
		public DropDownEventArgs( ToolStripDropDown dropDown )
		{
			m_dropDown = dropDown;
		}
	}
	/// <summary>
	/// Represents the method that will handle a BeforeCustomizeDropDownPopup event.
	/// </summary>
	/// <param name="sender">Control that raises event.</param>
	/// <param name="e">Provides with DropDown that will be shown.</param>
	public delegate void DropDownEventHandler( object sender, DropDownEventArgs e );
	#endregion

	#region *** ToolStripTabItemsComparer

	public class ToolStripTabItemsComparer : IComparer
	{
		#region IComparer Members

		public int Compare(object x, object y)
		{
			ToolStripTabItem obj_X = x as ToolStripTabItem;
			ToolStripTabItem obj_Y = y as ToolStripTabItem;

			return obj_X.Text.CompareTo(obj_Y.Text);
		}

		#endregion
	}

	#endregion
}
#endif