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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Timers;
using System.Diagnostics;
using Syncfusion.Windows.Forms.Tools.XPMenus;
using NativeMethods = Syncfusion.Runtime.InteropServices.NativeMethods;
using Syncfusion.Windows.Forms.Tools.Renderers;
using Syncfusion.Diagnostics;
using Microsoft.Win32;

namespace Syncfusion.Windows.Forms.Tools
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public enum AutoHideAnimationState { None, Hiding, Showing };
	[
		ToolboxItem(false),	
		DesignTimeVisible(false)
	]
	[Syncfusion.Documentation.DocumentationExclude()]
	public class AHTabPage : TabPageAdv
	{
		public DockStateControllerBase m_dhcClient = null;
		static protected int nNamingCount = 0;

		public AHTabPage(DockStateControllerBase dhc, String text, int imgindex) : base()
		{
			this.m_dhcClient = dhc;
			this.Text = text;
			this.ImageIndex = imgindex;

			AHTabPage.nNamingCount++;
			this.Name = String.Concat("AHTabPage_", AHTabPage.nNamingCount.ToString());
		}

		protected override ITabData CreateDefaultTabData()
		{
			TabGroupData tabGroupData = new TabGroupData();
			tabGroupData.Padding = new Point(3, 3);
			return tabGroupData;
		}

        public DockStateControllerBase DHCClient
        {
            get { return this.m_dhcClient; }
        }

		// Do not create the tabpage window. Tabpages merely serve as placeholders and handle creation is not required.
		protected override void CreateHandle()
		{
		}

		protected override void Dispose(bool bdisposing)
		{
			if((bdisposing == true) && (this.m_dhcClient != null))
			{
				this.m_dhcClient = null;
			}
			base.Dispose(bdisposing);
		}      
	}


	
	/// Summary description for AHTabControl.	
	[
		ToolboxItem(false),		
		DesignTimeVisible(false)
	]
	[Syncfusion.Documentation.DocumentationExclude()]
	public class AHTabControl : AHTabControlBase
	{
		internal int selectedIndex = 0;
		protected DockingManager dockingMgr = null;
		protected DockingStyle tabBorder = DockingStyle.Fill;
		internal int nClientIndex = -1;
		
		internal int ClientIndex = -1;
		static protected internal System.Windows.Forms.Timer tmrShowController = null;
		static protected internal System.Windows.Forms.Timer tmrHideController = null;
	
		static protected internal System.Windows.Forms.Timer tmrAnimation = null;
		protected DockStateControllerBase ddcAnimation = null;
		private Rectangle rcAnimation = Rectangle.Empty;
		protected Rectangle rcVisibleBounds = Rectangle.Empty;

		protected bool bLeadSpace = false;
		protected bool bTrailSpace = false;
		new internal bool MouseClick = false;
		protected bool bNeedActivate = false;

		protected internal AutoHideAnimationState AnimationState = AutoHideAnimationState.None;

		protected ThemedControlDrawing tdTab = null;

        /// </override>
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                if( base.Font != null && this.dockingMgr.AutoHideTabFont == value)
                {
                    base.Font = value;
                    this.SetItemSize();
                }
            }
        }

        /// </override>
        public override Font ActiveTabFont
        {
            get
            {
                return base.ActiveTabFont;
            }
            set
            {
                if( base.ActiveTabFont != value )
                {
                    base.ActiveTabFont = value;
                    this.SetItemSize();
                }
            }
        }

        /// </override>
        public override Point Padding
        {
            get
            {
                return base.Padding;
            }
            set
            {
                if( base.Padding != value )
                {
                    base.Padding = value;
                    this.SetItemSize();
                }
            }
        }

        /// </override>
        public override TabAlignment Alignment
        {
            get
            {
                return base.Alignment;
            }
            set
            {
                if( base.Alignment != value )
                {
                    base.Alignment = value;
                    this.SetItemSize();
                }
            }
        }

		public DockingStyle Edge
		{
			get
			{
				return tabBorder;
			}
		}

		protected override bool IsOffice2003Style
		{
			get
			{
				if( this.TabStyle.Name == "TabGroupRendererOffice2003" )
					return true;

				return false;
			}
		}

		public bool LeadSpace
		{
			set { this.bLeadSpace = value; }
		}

		public bool TrailSpace
		{
			set { this.bTrailSpace = value; }
		}

		public int EmptyLeft
		{
			get { return (int)(this.Renderer as AHTabRenderer).PadX; }
		}

		public ThemedControlDrawing ThemeDraw
		{
			get { return this.tdTab; }
		}

		public DockingManager DockingManager
		{
			get { return this.dockingMgr; }
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;

				if( this.TabPages == null || this.TabPages.Count == 0 )
				{
					cp.Height = 0;
					cp.Width = 0;
				}
				return cp;
			}
		}

		public AHTabControl(DockingManager dmgr, DockingStyle border)
		{
			this.dockingMgr = dmgr;
			this.tabBorder = border;
			this.TabStop = false;
			this.Visible = false;
			this.TabStyle = GetRendererType();
			if( border == DockingStyle.Right )
				this.ImageAlignmentR = RelativeImageAlignment.RightOfText;
			this.Multiline = false;
			this.ScrollIncrement = ScrollIncrement.Tab;
			this.TextAlignment = System.Drawing.StringAlignment.Center;
			this.TextLineAlignment = System.Drawing.StringAlignment.Center;
			this.Font = dmgr.AutoHideTabFont;
			this.ActiveTabFont = this.Font;            
			this.ItemSize = new Size(0, dmgr.AutoHideTabHeight);
			this.Padding = new Point(3,0);
			this.ImageList = this.dockingMgr.ImageList;
			this.SetStyle(ControlStyles.Selectable, false);
			(this.Renderer as AHTabRenderer).PadX = (float)2;
			(this.Renderer as AHTabRenderer).PadY = (float)2;
			
			if(XPThemes.IsThemedOS)
				this.tdTab = new ThemedControlDrawing(ThemedControls.TAB);

			this.dockingMgr.ImageListChanged += new EventHandler(this.dockingManager_ImageListChanged);

			this.RightToLeft = this.dockingMgr.RightToLeft;			
			this.ScrollButtons.ThemesEnabled = true;
		}
		/// <summary>
		/// Forces immediate layout of control.
		/// </summary>
		protected internal void ForceLayout()
		{
			if( this.IsHandleCreated )
			{
				Graphics g = this.CreateGraphics();
				this.Layout( g, false );
				g.Dispose();
			}
		}

		protected override void InitScrollButtons()
		{
			m_sbScrollButtons.TabStop = false;
			UpdateScrollButtonState();

			m_sbScrollButtons.Visible = true;
			m_sbScrollButtons.BringToFront();
			m_sbScrollButtons.UpDown += new UpDownEventHandler(this.UpDownEventHandler);
			m_sbScrollButtons.VSLikeButton = ( this.VSLikeScrollButton ||
				this.IsOffice2003Style || this.IsWhidbeyStyle );			

			UpdateScrollButtonsStyle();
		}

		// The message filter calls this function in response to mouse-move messages
		internal void MFMouseMessageHandler(Point ptscreen)
		{
			if( AHTabControl.tmrAnimation != null )
			{
				Point pt = this.PointToClient(Cursor.Position);

				if( this.GetActiveTabGroupOrSubGroupDockHostRect().Contains(ptscreen) ||
					GetTabGroupIndex(pt) == this.nClientIndex )
				{
					if( AnimationState == AutoHideAnimationState.Hiding &&
						( AHTabControl.tmrHideController != null || AHTabControl.tmrShowController != null) )
					{
						AnimationState = AutoHideAnimationState.Showing;
						AHTabControl.tmrAnimation.Tick -= new EventHandler(this.HideAnimation_TickEventHandler);
						AHTabControl.tmrAnimation.Tick += new EventHandler(this.ShowAnimation_TickEventHandler);
					}
				}
				else
				{
					if( AnimationState == AutoHideAnimationState.Showing &&
						( AHTabControl.tmrHideController != null || AHTabControl.tmrShowController != null ) )
					{						
						AnimationState = AutoHideAnimationState.Hiding;
						AHTabControl.tmrAnimation.Tick -= new EventHandler(this.ShowAnimation_TickEventHandler);						
						AHTabControl.tmrAnimation.Tick += new EventHandler(this.HideAnimation_TickEventHandler);
					}
				}				
				return;
			}

			//Error: ClientIndex is out of synch.
			if( this.nClientIndex < 0 )
			{
				string msg = string.Format(
							"AHTabControl[MFMouseMessageHandler] Error: nClientIndex has unacceptable value ({0}).\n",
							this.nClientIndex
													  );
				Exception he = new Exception( msg );
				TraceUtil.TraceExceptionCatched( he );
				ExceptionManager.RaiseExceptionCatched( this, he );
				return;
			}

			DockStateControllerBase dhc = (this.TabPages[this.nClientIndex] as AHTabPage).m_dhcClient;
			// If neither the clientindex tab or the displayed host contain the pt, start the hidecontroller timer			
            if ((this.RectangleToScreen(Rectangle.Inflate(this.GetTabRect(this.nClientIndex), 1, 1)).Contains(ptscreen) == false) && 
				(dhc.ParentController.HostControl.RectangleToScreen(Rectangle.Inflate(dhc.ParentController.LayoutRect,2,2)).Contains(ptscreen) == false) &&
				(dockingMgr.ForceHideActiveControl || (dhc.HostControl.ContainsFocus == false)))
			{
				if(AHTabControl.tmrHideController == null)
				{
					if(AHTabControl.tmrShowController != null)
					{						
						AHTabControl.tmrShowController.Tick -= new System.EventHandler(this.ShowController_TickEventHandler);
						AHTabControl.tmrShowController.Stop();
						AHTabControl.tmrShowController = null;
					}
                    if (dockingMgr.VisualStyle == VisualStyle.Metro && MouseClick)
                    {
                        AHTabControl.tmrHideController = new System.Windows.Forms.Timer();
                        AHTabControl.tmrHideController.Tick += new System.EventHandler(this.HideController_TickEventHandler);
                        AHTabControl.tmrHideController.Interval = DockingManager.nHideInterval;
                        AHTabControl.tmrHideController.Start();
                        MouseClick = false;
                    }
                    else
                    {
                        AHTabControl.tmrHideController = new System.Windows.Forms.Timer();
                        AHTabControl.tmrHideController.Tick += new System.EventHandler(this.HideController_TickEventHandler);
                        AHTabControl.tmrHideController.Interval = DockingManager.nHideInterval;
                        AHTabControl.tmrHideController.Start();
                    }
				}
			}
		}

		protected void Redraw()
		{
            using (Graphics g = Graphics.FromHwnd(this.Handle))
            {
                PaintEventArgs args = new PaintEventArgs(g, this.Bounds);
                this.OnPaint(args);
            }
		}

		// The message filter calls this function in response to WM_MOUSELEAVE and WM_NCMOUSELEAVE messages
		internal void MFNCMouseMessageHandler(Point ptscreen)
		{			
			if(AHTabControl.tmrAnimation != null)
				return;

			//Error: ClientIndex is out of synch.
			if( this.nClientIndex < 0 )
			{
				string msg = string.Format(
							"AHTabControl[MFMouseMessageHandler] Error: nClientIndex has unacceptable value ({0}).\n",
							this.nClientIndex
													  );
				Exception he = new Exception( msg );
				TraceUtil.TraceExceptionCatched( he );
				ExceptionManager.RaiseExceptionCatched( this, he );
				// Can also set nClientIndex to a default non subzero value if needed
				return;
			}

			DockStateControllerBase dhc = (this.TabPages[this.nClientIndex] as AHTabPage).m_dhcClient;
			// If neither the clientindex tab or the displayed host contain the pt, start the hidecontroller timer			
			if(dockingMgr.ForceHideActiveControl || (dhc.HostControl.ContainsFocus == false))
			{
				if(AHTabControl.tmrHideController == null)
				{
					if(AHTabControl.tmrShowController != null)
					{						
						AHTabControl.tmrShowController.Tick -= new System.EventHandler(this.ShowController_TickEventHandler);
						AHTabControl.tmrShowController.Stop();
						AHTabControl.tmrShowController = null;
					}
                    if (dockingMgr.VisualStyle != VisualStyle.Metro)
                    {
                        AHTabControl.tmrHideController = new System.Windows.Forms.Timer();
                        AHTabControl.tmrHideController.Tick += new System.EventHandler(this.HideController_TickEventHandler);
                        AHTabControl.tmrHideController.Interval = DockingManager.nHideInterval;
                        AHTabControl.tmrHideController.Start();	
                    }
				}
			}
		}

		public void dockingManager_ImageListChanged(Object obj, EventArgs e)
		{
			this.ImageList = this.dockingMgr.ImageList;
		}

		public void AddToEmptyLeft(int nleft)
		{
			(this.Renderer as AHTabRenderer).PadX += (float)nleft;
		}		

        private void ResetAutoHideIndex(DockingManager dmgr)
        {
            foreach (DockControllerBase dcbase in dmgr.alDockAreaControllers)
            {
                if (dcbase is DockHostController && (dcbase as DockStateControllerBase).AutoHideMode)
                {
                    foreach (AHTabPage page in this.TabPages)
                    {
                        if ((dcbase as DockStateControllerBase).Equals(page.DHCClient))
                        {
                            (dcbase as DockStateControllerBase).AutoHideIndex = this.TabPages.IndexOf(page);
                            break;
                        }
                    }
                }
            }
        }

        private void UpdatePreviousAutoHideIndex(DockStateControllerBase ddcbase)
        {   
            foreach (DockControllerBase db in ddcbase.DockingManager.alDockAreaControllers)
            {
                if (db is DockHostController && !(db as DockStateControllerBase).Equals(ddcbase))
                {
                    int queueIndex = (db as DockStateControllerBase).PreviousAutoHideIndex;
                    if (queueIndex != -1 && queueIndex < ddcbase.PreviousAutoHideIndex)
                    {
                        ddcbase.PreviousAutoHideIndex = queueIndex;
                        break;
                    }
                }
            }
        }

		public void AddTab(DockStateControllerBase ddcbase, bool bshowcontrol)
		{
			if(ddcbase.ChildCount <= 0)	// DockHostController
			{
				DockHostController dhc = ddcbase as DockHostController;
				Debug.Assert((dhc!=null), "Error: Invalid Controller.\n");
				
				// The dockhostcontroller is made the main group data item. The controller's text and image comprise
				// the sub-group data for this group.
                int index = -1;
                AHTabPage tab = null;
                if (ddcbase.PreviousAutoHideIndex == -1)
                {
                    tab = new AHTabPage(dhc, dhc.HostControl.Text, dhc.ImageIndex);
                    this.TabPages.Add(tab);
                    index = this.TabCount - 1;
                }
                else
                {
                    tab = new AHTabPage(dhc, dhc.HostControl.Text, dhc.ImageIndex);
                    UpdatePreviousAutoHideIndex(ddcbase);
                    index = ddcbase.PreviousAutoHideIndex;
                    if (index > this.TabPages.Count)
                        index = this.TabPages.Count;

                    this.TabPages.Insert(index, tab);                    
                    ddcbase.PreviousAutoHideIndex = -1; //reset the value
                    ResetAutoHideIndex(ddcbase.DockingManager);
                }
				TabGroupData tgroupdata = this.TabsData[index] as TabGroupData;
                
				tgroupdata.Items.Add( new TabGroupItem(dhc.HostControl.Text, dhc.ImageIndex, dhc) );
                
				if ( dhc.AutoHideIndex == -1 && tab != null)
					dhc.AutoHideIndex = this.TabPages.IndexOf(tab);

				if(this.Visible == false)
					this.Visible = true;
				if(bshowcontrol == true)
					ShowController(dhc, false);
			}
			else	// ddcbase is a tabcontroller. 
			{
				DockTabController docktabctrlr = ddcbase as DockTabController;
				Debug.Assert((docktabctrlr!=null), "Error: Invalid Controller.\n");
				DockTabControl docktab = docktabctrlr.TabControl;
				DockHostController dhcactive = docktabctrlr.HostController as DockHostController;
				dhcactive.DINew.rcDockArea = docktabctrlr.DINew.rcDockArea;

				// Add the docktabcontroller as the main group data item. 
				// Add all of the tabcontroller's child dockhost's text/image data as sub-group data under this group.
				this.TabPages.Add( new AHTabPage(docktabctrlr, null, -1) );
				TabGroupData tgroupdata = this.TabsData[this.TabCount-1] as TabGroupData;
				foreach(DockTabPage page in docktab.TabPages)
					tgroupdata.Items.Add( new TabGroupItem(page.dhcClient.HostControl.Text, page.dhcClient.ImageIndex, page.dhcClient) );
				tgroupdata.SelectedIndex = docktab.SelectedIndex;

				DockHostController dockHostCtrl = docktabctrlr.HostController as DockHostController;
				if( (dockHostCtrl != null) && (dockHostCtrl.AutoHideIndex == -1))
					dockHostCtrl.AutoHideIndex = this.TabPages.Count - 1;
				docktabctrlr.AutoHideIndex = this.TabPages.Count - 1;

				docktab.Visible = false;
				if(this.Visible == false)
					this.Visible = true;
				if(bshowcontrol == true)
					ShowController(docktabctrlr, false);
				docktabctrlr.AdjustLayout();	// Force layout adjustment to position client without the docktabcontrol.
			}
		}		
		
		public void RemoveTab(DockStateControllerBase dscbase)
		{
			int nclient = this.nClientIndex;   

			if(dscbase is DockHostController)
                nclient=((DockHostController)dscbase).AutoHideIndex;  

            if(dscbase is DockTabController)
                nclient = ((DockTabController)dscbase).AutoHideIndex;

			if( AHTabControl.tmrAnimation != null )
			{
				switch( AnimationState )
				{
					case AutoHideAnimationState.Hiding:
					case AutoHideAnimationState.None:
						AHTabControl.tmrAnimation.Tick -= new EventHandler(this.HideAnimation_TickEventHandler);
						this.dockingMgr.HideAutoHiddenControl();
						break;

					case AutoHideAnimationState.Showing:
						AHTabControl.tmrAnimation.Tick -= new EventHandler(this.ShowAnimation_TickEventHandler);
						break;
				}
				
				if(AHTabControl.tmrAnimation != null)
				{
					AHTabControl.tmrAnimation.Stop();
					AHTabControl.tmrAnimation = null;
				}
			}

			this.dockingMgr.AHInViewTab = null;
			this.nClientIndex = -1;

			DockTabController dockTabCtrl = dscbase as DockTabController;
			if( dockTabCtrl != null )
			{
				foreach( DockTabPage page in dockTabCtrl.TabControl.TabPages )
					page.dhcClient.AutoHideIndex = -1;
			}
			else
				dscbase.AutoHideIndex = -1;

			foreach(TabGroupData tgdata in this.TabsData)
			{
				for(int i = 0; i < tgdata.Items.Count; i++)
				{
					if( i != 0 )
					{
						TabGroupItem tabGroupItem = tgdata.Items[i] as TabGroupItem;
						if( tabGroupItem != null )
						{
							DockHostController dockHostCtrl = tabGroupItem.Tag as DockHostController;
							if( dockHostCtrl != null )
								dockHostCtrl.AutoHideIndex = -1;
						}
					}
				}
			}
			
			
			// If nclient is -1, then iterate the tabcollection and get hold of the tabindex for this dockcontroller
			if(nclient < 0)
			{
				DockControllerBase dcbase = null;
				if(dscbase is DockTabController)
					dcbase = (dscbase as DockTabController).HostController;
				else
					dcbase = dscbase;

				foreach(TabGroupData groupdata in this.TabsData)
				{
					if(groupdata.Items.Count > 1)
					{						
						foreach(TabGroupItem subitem in groupdata.Items)
						{														
							if(subitem.Tag == dcbase)
							{
								// Set as selected - Subsequent remove sub-index execution always removes the selected subindex.
								groupdata.SelectedIndex = groupdata.Items.IndexOf(subitem);
								nclient = this.TabsData.IndexOf(groupdata);
								break;
							}
						}
					}
					else if(groupdata.Items[0].Tag == dcbase)
					{
						nclient = this.TabsData.IndexOf(groupdata);
						break;
					}
				}
			}
			Debug.Assert((nclient >= 0), "Error: Invalid Controller.\n");
	
			TabGroupData tgroupdata = this.TabsData[nclient] as TabGroupData;
			if(dscbase.ChildCount <= 0)	//DockHostController
			{					
				DockHostController dhc = dscbase as DockHostController;
				Debug.Assert((dhc!=null), "Error: Invalid Controller.\n");
				// If a sub-group is present, then remove the particular sub-group tab
				if(tgroupdata.Items.Count > 1)
				{
					TabGroupItem tabitem = null;
					foreach( TabGroupItem item in tgroupdata.Items )
					{
						if( item.Tag == dscbase )
						{
							tabitem = item;
							tabitem.Tag = null;
						}
					}
			
					tgroupdata.Items.Remove(tabitem);
					this.OnTabPanelBoundsAffected();	// Force tabpanel to update layout
					DockStateControllerBase ddcnewactive = (this.SelectedTab as AHTabPage).m_dhcClient;
					ddcnewactive.HostControl.Visible = false;	
					Rectangle rcnewlayout = ddcnewactive.LayoutRect;
					if((rcnewlayout.Width > 0) && (rcnewlayout.Height > 0))
						ddcnewactive.DINew.rcDockArea = rcnewlayout;
					ddcnewactive.ParentController.LayoutRect = Rectangle.Empty;	
					
					// If the dockhostcontroller being removed is part of a tabbed group and is the last-but-one 
					// controller, then replace the TabPage's DockTabController reference with that of the last 
					// dockhostcontroller.
					if(tgroupdata.Items.Count == 1)
					{
						AHTabPage tabpage = this.TabPages[nclient] as AHTabPage;
						Debug.Assert(tabpage.m_dhcClient is DockTabController);
						tabpage.m_dhcClient = tgroupdata.Items[0].Tag as DockStateControllerBase;
					}
				}
				else
				{
					tgroupdata.Items[0].Tag = null;
					this.TabPages.RemoveAt(nclient);
				}
			}
			else	// DockTabController
			{
				DockTabController docktabctrlr = dscbase as DockTabController;
				Debug.Assert((docktabctrlr!=null), "Error: Invalid Controller.\n");				
				foreach(TabGroupItem tabitem in tgroupdata.Items)
					tabitem.Tag = null;
				this.TabPages.RemoveAt(nclient);
				docktabctrlr.HostController.DockTab.Visible = true;
				docktabctrlr.AdjustLayout();	// Layout adjustment positions the client with the docktabcontrol.
			}

			if( this.TabCount == 0 )
			{
				this.Visible = false;
				this.ddcAnimation = null;
			}

			foreach(TabGroupData tgdata in this.TabsData)
			{
				TabGroupItem tabGroupItem = tgdata.Items[0] as TabGroupItem;
				if( tabGroupItem != null )
				{
					DockHostController dhc = tabGroupItem.Tag as DockHostController;
					if( dhc != null )
					{
						dhc.AutoHideIndex = this.TabsData.IndexOf(tgdata);
						DockTabController dtc = dhc.ParentController as DockTabController;

						if(dtc != null)
							dtc.AutoHideIndex = dhc.AutoHideIndex;
					}
				}
			}
		}
		
		protected DockStyle GetDockBorder()
		{
			switch(this.Alignment)
			{
				case TabAlignment.Right:
					return DockStyle.Left;					
				case TabAlignment.Left:
					return DockStyle.Right;				
				case TabAlignment.Bottom:
					return DockStyle.Top;					
				case TabAlignment.Top:
					return DockStyle.Bottom;
				default:
					return DockStyle.None;
			}			
		}

		protected void InitiateShowAnimationStartEvent(DockStateControllerBase ddcbase, Control dockctrl)
		{
			this.dockingMgr.HostControl.SuspendLayout();
			DockStyle border = this.GetDockBorder();
			Rectangle rcshowbounds = this.CalcClientControlSize(ddcbase);
			AutoHideAnimationEventArgs aheventargs = new AutoHideAnimationEventArgs(dockctrl, border, rcshowbounds);				
			this.dockingMgr.FireAutoHideAnimationEvent("AutoHideAnimationStart", aheventargs);				
			this.rcAnimation = aheventargs.Bounds;
			this.dockingMgr.ahTabAnimate = this;
		}

		protected void InitiateHideAnimationStartEvent(DockStateControllerBase ddcbase, Control dockctrl)
		{
			Rectangle rchidebounds;
			this.dockingMgr.HostControl.SuspendLayout();
			DockStyle border = this.GetDockBorder();
			if((border == DockStyle.Top) || (border == DockStyle.Bottom))
				rchidebounds = new Rectangle(this.rcAnimation.Left, this.rcAnimation.Top, this.rcAnimation.Width, ddcbase.DINew.rcDockArea.Height);
			else
				rchidebounds = new Rectangle(this.rcAnimation.Left, this.rcAnimation.Top, ddcbase.DINew.rcDockArea.Width, this.rcAnimation.Height);
			AutoHideAnimationEventArgs aheventargs = new AutoHideAnimationEventArgs(dockctrl, border, rchidebounds);
			this.dockingMgr.FireAutoHideAnimationEvent("AutoHideAnimationStart", aheventargs);
			this.dockingMgr.ahTabAnimate = this;
			this.rcVisibleBounds = new Rectangle( Point.Empty, rchidebounds.Size );
		}

		protected void InitiateAnimationStopEvent(DockStateControllerBase ddcbase, Control dockctrl)
		{
			this.dockingMgr.HostControl.ResumeLayout();
			this.rcVisibleBounds = Rectangle.Empty;
			DockStyle border = this.GetDockBorder();
			AutoHideAnimationEventArgs aheventargs = new AutoHideAnimationEventArgs(dockctrl, border, Rectangle.Empty);
			this.dockingMgr.FireAutoHideAnimationEvent("AutoHideAnimationStop", aheventargs);
			ddcbase.bAutoHideSizing = false;
			this.dockingMgr.ahTabAnimate = null;
		}

		public void ShowController(DockStateControllerBase ddcbase, bool bAnimate)
		{
			ShowController( ddcbase, bAnimate, false );
		}

		public void ShowController(DockStateControllerBase ddcbase, bool banimate, bool bActivateAfterShow)
		{
			if( AHTabControl.tmrAnimation != null )
			{
				this.StopShowAnimation();
			}

			if( nClientIndex > -1 )
			{
				DockStateControllerBase dhc = (this.TabPages[this.nClientIndex] as AHTabPage).m_dhcClient;
				if( dhc != ddcbase )
					this.HideController(dhc, false);
			}


			if(AHTabControl.tmrShowController != null)
			{				
				AHTabControl.tmrShowController.Tick -= new System.EventHandler(this.ShowController_TickEventHandler);
				AHTabControl.tmrShowController.Stop();				
				AHTabControl.tmrShowController = null;
				
				// For a timed showcontroller, verify that the cursor has not moved out of the tab rect after triggering 
				// the timer. If so, preempt the showcontroller call
				if( this.RectangleToScreen(GetTabRect(this.nClientIndex)).Contains(Cursor.Position) == false ) 
				{
					this.nClientIndex = -1;
					return;
				}
			}			

			if(this.nClientIndex >= 0)	
			{
				// If a sub-group is present, then activate the sub-group tab
				TabGroupData tgroupdata = this.TabsData[this.nClientIndex] as TabGroupData;
				if(tgroupdata.Items.Count > 1)
				{
					DockTabController docktabctrlr = ddcbase as DockTabController;
                    //Fix for 1729: Nullreference exception when we dock into a autohidden docktab
                    if (docktabctrlr != null)
					    tgroupdata.SelectedIndex = docktabctrlr.HostController.DockTab.SelectedIndex;
				}
			}
			
			if(banimate == true)
			{
                InternalStopShowAnimation();
				if( AnimationState == AutoHideAnimationState.Hiding )
					StopHideAnimation();
				this.InitiateShowAnimationStartEvent(ddcbase, ddcbase.HostControl.Controls[0]);

				ddcbase.HostControl.BringToFront();

				this.ddcAnimation = ddcbase;
				this.ddcAnimation.bAutoHideSizing = true;
				int nstep = DockingManager.AnimationStep;
				switch(this.tabBorder)
				{
					case DockingStyle.Left:
						if( ddcAnimation.HostControl is DockHost )
						this.ddcAnimation.HostControl.Bounds = new Rectangle( rcAnimation.Left - rcAnimation.Width, rcAnimation.Top, 
							rcAnimation.Width, rcAnimation.Height );
						break;						
					case DockingStyle.Top:	
						if( ddcAnimation.HostControl is DockHost )
						this.ddcAnimation.HostControl.Bounds = new Rectangle( rcAnimation.Left, rcAnimation.Top - rcAnimation.Height,
							rcAnimation.Width, rcAnimation.Height );
						break;
					case DockingStyle.Right:
						this.ddcAnimation.HostControl.Bounds = new Rectangle( rcAnimation.Left + rcAnimation.Width, rcAnimation.Top,
							rcAnimation.Width, rcAnimation.Height );
						break;
					case DockingStyle.Bottom:
						this.ddcAnimation.HostControl.Bounds = new Rectangle( rcAnimation.Left, rcAnimation.Top + rcAnimation.Height,
							rcAnimation.Width, rcAnimation.Height );
						break;
				}
				// Recalculating location and size for all childs.
				RefreshInnerControlLayout( this.ddcAnimation.HostControl.Bounds );
				AHTabControl.tmrAnimation = new System.Windows.Forms.Timer();
				AHTabControl.tmrAnimation.Interval = DockingManager.AnimationSpeed;
				AHTabControl.tmrAnimation.Tick += new EventHandler(this.ShowAnimation_TickEventHandler);
				AHTabControl.tmrAnimation.Start();				
				this.BringToFront();
				this.bNeedActivate = bActivateAfterShow;
			}
			else						
			{
				this.dockingMgr.ahTabAnimate = null;
				this.rcAnimation = this.CalcClientControlSize(ddcbase);

                this.dockingMgr.AHInViewTab = this;
                if (this.dockingMgr.AHInViewTab.Parent == null
                    && this.dockingMgr.HostControl != null)
                    this.dockingMgr.AHInViewTab.Parent = this.dockingMgr.HostControl;

				ddcbase.HostControl.Bounds = this.rcAnimation;
				ddcbase.HostControl.BringToFront();
				ddcbase.HostControl.Region = null;
				this.nClientIndex = ddcbase.AutoHideIndex;

				// Layout and display the sizingcontroller/splitter
				SetClientControlSize(ddcbase, this.rcAnimation);
				ddcbase.HostControl.Visible = true;

				DockControllerBase splitter = ddcbase.ParentController.GetChildAt(1);				
				splitter.HostControl.Visible = true;
				splitter.HostControl.BringToFront();

				if(this.nClientIndex < 0)	// Happens only during addition of a new tab.
					this.nClientIndex = this.TabPages.Count-1;

				this.dockingMgr.AHInViewTab = this;

				if( bActivateAfterShow )
				{
					ddcbase.HostControl.Focus();
				}
			}
		}

		/// <summary>
		/// Recalculating location and size for inner control childs.
		/// </summary>
		/// <param name="rectangle"> New layout rectangle. </param>
		private void RefreshInnerControlLayout( Rectangle rectangle )
		{
			if( null != ddcAnimation )
			{
				DockControllerBase sizingparent = null;
				if( ddcAnimation.ParentController != null )
				{
					if( ddcAnimation.ParentController is SizingController )
						sizingparent = ddcAnimation.ParentController;
					else
						sizingparent = ddcAnimation.ParentController.ParentController;
				}

				if( sizingparent != null )
					sizingparent.LayoutRect = rectangle;
			}
		}

		internal void InternalStopShowAnimation()
		{
			if( this.nClientIndex >= 0 && AHTabControl.tmrAnimation != null )	// If a tab is currently being displayed, then hide it and show the new tab immediately.
			{				
				StopShowAnimation();
				this.dockingMgr.AHInViewTab = null;

				DockStateControllerBase dhcprev = (this.TabPages[this.nClientIndex] as AHTabPage).m_dhcClient;

				// Use the nprevsubgroup index to get hold of the current visible DockHostController
				Control dockctrl;				
				dockctrl = dhcprev.HostControl.Controls[0];
				
				this.InitiateHideAnimationStartEvent(dhcprev, dockctrl);

				dhcprev.HostControl.Visible = false;
				Rectangle rcprevlayout = dhcprev.LayoutRect;
				if ((rcprevlayout.Width > 0) && (rcprevlayout.Height > 0))
					dhcprev.DINew.rcDockArea = rcprevlayout;
				dhcprev.ParentController.LayoutRect = Rectangle.Empty;

				this.InitiateAnimationStopEvent(dhcprev, dockctrl);
				this.nClientIndex = -1;
			}
		}

		private bool hideAfterShow = false;

		public void HideController(DockStateControllerBase ddcbase, bool banimate)
		{
			HideController(ddcbase, banimate, false);
		}

		public void HideController(DockStateControllerBase ddcbase, bool banimate, bool bhidenow)
		{
			if(ddcbase == null)	
			{
				if( nClientIndex < 0 )
				{
					hideAfterShow = true;
					return;
				}

				ddcbase = (this.TabPages[this.nClientIndex] as AHTabPage).m_dhcClient;
			}			
			
			if( AHTabControl.tmrAnimation != null )
			{
				this.StopHideAnimation();				
			}
			if( AHTabControl.tmrHideController != null )
			{
				AHTabControl.tmrHideController.Tick -= new System.EventHandler(this.HideController_TickEventHandler);
				AHTabControl.tmrHideController.Stop();
				AHTabControl.tmrHideController = null;
			}

			DockControllerBase sizingparent = null;
			if(ddcbase.ParentController is SizingController)
				sizingparent = ddcbase.ParentController;
			else
				sizingparent = ddcbase.ParentController.ParentController;
            if (sizingparent == null) return;
			DockControllerBase splitter = sizingparent.GetChildAt(1);

			// If the cursor is over the clientindex tab or the displayed dockhost, 
			// ignore the hidecontroller call ( except situation after autohide button pressing )
			Point ptscreen = Cursor.Position;
			if( ( (this.GetTabGroupIndex(this.PointToClient(ptscreen)) != this.nClientIndex) && 
				(sizingparent.HostControl.RectangleToScreen(sizingparent.LayoutRect).Contains(ptscreen) == false) 
				&& (splitter.HostControl.Capture == false) || (bhidenow == true) ) && (banimate == true)  ) 
			{
                this.InitiateHideAnimationStartEvent(ddcbase, ddcbase.HostControl.Controls[0]);

				this.ddcAnimation = ddcbase;
				this.ddcAnimation.bAutoHideSizing = true;
				splitter.HostControl.Visible = false;

				this.RemoveAHTabControlTimer("HideAnimation");
				AHTabControl.tmrAnimation = new System.Windows.Forms.Timer();
				AHTabControl.tmrAnimation.Interval = DockingManager.AnimationSpeed;
				AHTabControl.tmrAnimation.Tick += new EventHandler(this.HideAnimation_TickEventHandler);
				AHTabControl.tmrAnimation.Start();
				this.AnimationState = AutoHideAnimationState.Hiding;
				this.rcAnimation = ddcbase.HostControl.Bounds;
				this.BringToFront();
			}
			else 
			{
				if( banimate && ddcAnimation == ddcbase )
					return;
				this.dockingMgr.ahTabAnimate = null;
				if( ( ddcbase.HostControl.Visible == false ) || ( banimate == false ) )	// Invoked during deserialization
				{
					splitter.HostControl.Visible = false;
					ddcbase.HostControl.Visible = false;
					sizingparent.LayoutRect = Rectangle.Empty;
					this.nClientIndex = -1;
					if( this.dockingMgr.AHInViewTab != null )
						this.dockingMgr.AHInViewTab = null;
				}
				else
				{
					if (AHTabControl.tmrHideController != null)
					{
						AHTabControl.tmrHideController.Tick -= new System.EventHandler(this.HideController_TickEventHandler);
						AHTabControl.tmrHideController.Stop();
						AHTabControl.tmrHideController = null;
					}
				}
				
				if( ddcAnimation != null && ddcAnimation == ddcbase && ddcAnimation.bInAutoHide )
				{
					if( Edge == DockingStyle.Left || Edge == DockingStyle.Right )
					{
						ddcAnimation.HostControl.Width = 0;
					}
					else
					{
						ddcAnimation.HostControl.Height = 0;
					}
					ddcAnimation.HostControl.Region = null;
					ddcAnimation.bAutoHideSizing = false;
					HideControllersSizingParent(ddcAnimation);
				}
			}
		}

		protected void ShowAnimation_TickEventHandler(Object sender, EventArgs e)
		{
			if(AHTabControl.tmrAnimation != null && this.ddcAnimation.HostControl != null )
			{
				Debug.Assert(this.ddcAnimation != null);				

				if( ddcAnimation.HostControl.Visible == false )
					ddcAnimation.HostControl.Visible = true;
				Rectangle rcbounds = this.ddcAnimation.HostControl.Bounds;
				int nstep = DockingManager.AnimationStep;
				int nMinTabHeight = 4;
				int nTabHeight = (DockingManager.AutoHideTabHeight > nMinTabHeight)? 
					DockingManager.AutoHideTabHeight : 
					nMinTabHeight;
				ddcAnimation.HostControl.SuspendLayout();
				switch(this.tabBorder)
				{
					case DockingStyle.Left:
						if(rcVisibleBounds.Width < rcbounds.Width - nstep)
						{
							while( nstep > nTabHeight )
							{
								Rectangle rcRgn = new Rectangle( Right - ddcAnimation.HostControl.Left - nTabHeight, 0, ddcAnimation.HostControl.Width - Right + ddcAnimation.HostControl.Left + nTabHeight, Height );
                                using(Region region =new Region( rcRgn ))
                                    ddcAnimation.HostControl.Region = region;
								rcVisibleBounds = rcRgn;
								ddcAnimation.HostControl.Left += nTabHeight;
								nstep -= nTabHeight;
							}
							if( nstep > 0 )
							{
								Rectangle rcRgn = new Rectangle( Right - ddcAnimation.HostControl.Left - nstep, 0, ddcAnimation.HostControl.Width - Right + ddcAnimation.HostControl.Left + nstep, Height );
                                using (Region region = new Region(rcRgn))
                                    ddcAnimation.HostControl.Region = region;
								rcVisibleBounds = rcRgn;
								ddcAnimation.HostControl.Left += nstep;
							}
							return;
						}
						break;						
					case DockingStyle.Top:
						if(rcVisibleBounds.Height < rcbounds.Height - nstep)
						{
							while( nstep > nTabHeight )
							{
								Rectangle rcRgn = new Rectangle( 0, Bottom - ddcAnimation.HostControl.Top - nTabHeight, Width, ddcAnimation.HostControl.Height - Bottom + ddcAnimation.HostControl.Top + nTabHeight );
                                using (Region region = new Region(rcRgn))
                                    ddcAnimation.HostControl.Region = region;
								rcVisibleBounds = rcRgn;
								ddcAnimation.HostControl.Top += nTabHeight;
								nstep -= nTabHeight;
							}
							if( nstep > 0 )
							{
								Rectangle rcRgn = new Rectangle( 0, Bottom - ddcAnimation.HostControl.Top - nstep, Width, ddcAnimation.HostControl.Height - Bottom + ddcAnimation.HostControl.Top + nstep );
                                using (Region region = new Region(rcRgn))
                                    ddcAnimation.HostControl.Region = region;
								rcVisibleBounds = rcRgn;
								ddcAnimation.HostControl.Top += nstep;
							}
							return;
						}
						break;
					case DockingStyle.Right:
						if(rcVisibleBounds.Width < rcbounds.Width - nstep)
						{
							while( nstep > nTabHeight )
							{
								Rectangle rcRgn = new Rectangle( 0, 0, ddcAnimation.HostControl.Width + Left - ddcAnimation.HostControl.Right + nTabHeight, Height );
                                using (Region region = new Region(rcRgn))
                                    ddcAnimation.HostControl.Region = region;
								rcVisibleBounds = rcRgn;
								ddcAnimation.HostControl.Left -= nTabHeight;
								nstep -= nTabHeight;
							}
							if( nstep > 0 )
							{
								Rectangle rcRgn = new Rectangle( 0, 0, ddcAnimation.HostControl.Width + Left - ddcAnimation.HostControl.Right + nstep, Height );
                                using (Region region = new Region(rcRgn))
                                    ddcAnimation.HostControl.Region = region;
								rcVisibleBounds = rcRgn;
								ddcAnimation.HostControl.Left -= nstep;
							}
							return;
						}
						break;
					case DockingStyle.Bottom:
						if(rcVisibleBounds.Height < rcbounds.Height - nstep)
						{
							while( nstep > nTabHeight )
							{
								Rectangle rcRgn = new Rectangle( 0, 0, Width, ddcAnimation.HostControl.Height + Top - ddcAnimation.HostControl.Bottom + nTabHeight );
                                using (Region region = new Region(rcRgn))
                                    ddcAnimation.HostControl.Region = region;
								rcVisibleBounds = rcRgn;
								ddcAnimation.HostControl.Top -= nTabHeight;
								nstep -= nTabHeight;
							}
							if( nstep > 0 )
							{
								Rectangle rcRgn = new Rectangle( 0, 0, Width, ddcAnimation.HostControl.Height + Top - ddcAnimation.HostControl.Bottom + nstep );
                                using (Region region = new Region(rcRgn))
                                    ddcAnimation.HostControl.Region = region;
								rcVisibleBounds = rcRgn;
								ddcAnimation.HostControl.Top -= nstep;
							}
							return;
						}
						break;
				}
				ddcAnimation.HostControl.Region = null;
				rcVisibleBounds = ddcAnimation.HostControl.Bounds;
				StopShowAnimation();
                if (this.dockingMgr != null && this.dockingMgr.VisualStyle == VisualStyle.Metro && ddcAnimation.HostControl.Controls[0] != null)
                    this.dockingMgr.ActivateControl(ddcAnimation.HostControl.Controls[0]);
			}
		}

		protected void StopShowAnimation()
		{
			AnimationState = AutoHideAnimationState.None;

			this.RemoveAHTabControlTimer("StopShowAnimation");
			if( this.ddcAnimation == null )
				return;
			if(this.ddcAnimation.HostControl.Controls[0].Anchor != (AnchorStyles.Left|AnchorStyles.Top))
				this.ddcAnimation.HostControl.Controls[0].Anchor = AnchorStyles.Left|AnchorStyles.Top;

			this.ddcAnimation.bAutoHideSizing = false;
			this.ddcAnimation.HostControl.Region = null;

			// Layout and display the hostcontrol/splitter
			rcAnimation = CalcClientControlSize( ddcAnimation );
			this.SetClientControlSize(this.ddcAnimation, this.rcAnimation);
			this.ddcAnimation.HostControl.Invalidate(false);

			DockControllerBase splitter = this.ddcAnimation.ParentController.GetChildAt(1);
			splitter.HostControl.Visible = true;
			splitter.HostControl.BringToFront();

			if(this.nClientIndex < 0)
			{
				int ntabcount = this.TabCount;
				for(int i=0; i<ntabcount; i++)
				{
					AHTabPage tabpage = this.TabPages[i] as AHTabPage;
					if(tabpage.m_dhcClient == this.ddcAnimation)
					{
						this.nClientIndex = i;
						break;
					}
				}
			}			
			this.dockingMgr.AHInViewTab = this;

			if( bNeedActivate )
			{
				this.ddcAnimation.HostControl.Focus();
				this.bNeedActivate = false;
			}

			this.InitiateAnimationStopEvent(this.ddcAnimation, this.ddcAnimation.HostControl.Controls[0]);

			// fix for defect 801
			// check whether to hide controller after show 
			if( hideAfterShow & nClientIndex != -1 )
			{
				HideController( null, true );
				hideAfterShow = false;
			}
		}

		protected void HideAnimation_TickEventHandler(Object sender, EventArgs e)
		{
			if(AHTabControl.tmrAnimation != null)
			{
				Debug.Assert(this.ddcAnimation != null);

				int nstep = DockingManager.AnimationStep;
				int nMinTabHeight = 4;
				int nTabHeight = (DockingManager.AutoHideTabHeight > nMinTabHeight)? 
					DockingManager.AutoHideTabHeight : 
					nMinTabHeight;
				Rectangle rcRgn = Rectangle.Empty;
				switch(this.tabBorder)
				{
					case DockingStyle.Left:
						if(rcVisibleBounds.Width > 0 )
						{
							int width = ddcAnimation.HostControl.Right - this.Right - nstep;
							int left = ddcAnimation.HostControl.Width - width;
							rcRgn = new Rectangle( left, 0, width, this.Height );
							if( width > 0 )
							{
								ddcAnimation.HostControl.Left -= nstep;
                                using (Region region = new Region(rcRgn))
                                    ddcAnimation.HostControl.Region = region;
							}
							rcVisibleBounds = rcRgn;
							return;
						}
						break;
					case DockingStyle.Top:
						if(rcVisibleBounds.Height > 0 )
						{
							int height = ddcAnimation.HostControl.Bottom - this.Bottom - nstep;
							int top = ddcAnimation.HostControl.Height - height;
							rcRgn = new Rectangle( 0, top, this.Width, height );
							if( height > 0 )
							{
								ddcAnimation.HostControl.Top -= nstep;
                                using (Region region = new Region(rcRgn))
                                    ddcAnimation.HostControl.Region = region;
							}
							rcVisibleBounds = rcRgn;
							return;
						}
						break;
					case DockingStyle.Right:
						if(rcVisibleBounds.Width > 0 )
						{
							int width = this.Left - ddcAnimation.HostControl.Left - nstep;
							rcRgn = new Rectangle( 0, 0, width, this.Height );
							if( width > 0 )
							{
								ddcAnimation.HostControl.Left += nstep;
                                using (Region region = new Region(rcRgn))
                                    ddcAnimation.HostControl.Region = region;
							}
							rcVisibleBounds = rcRgn;
							return;
						}
						break;
					case DockingStyle.Bottom:
						if(rcVisibleBounds.Height > 0)
						{
							int height = this.Top - ddcAnimation.HostControl.Top - nstep;
							rcRgn = new Rectangle( 0, 0, this.Width, height );
							if( height > 0 )
							{
								ddcAnimation.HostControl.Top += nstep;
                                using (Region region = new Region(rcRgn))
                                    ddcAnimation.HostControl.Region = region;
							}
							rcVisibleBounds = rcRgn;
							return;
						}
						break;
				}
				ddcAnimation.HostControl.Size = Size.Empty;
				ddcAnimation.HostControl.Region = null;
				StopHideAnimation();
			}
		}
		private void RemoveAHTabControlTimer(string eventName)
		{

			if (AHTabControl.tmrAnimation != null)
			{
				if (eventName.Equals("StopShowAnimation"))
					AHTabControl.tmrAnimation.Tick -= new EventHandler(this.ShowAnimation_TickEventHandler);
				else if (eventName.Equals("HideAnimation"))
					AHTabControl.tmrAnimation.Tick -= new EventHandler(this.HideAnimation_TickEventHandler);

				AHTabControl.tmrAnimation.Stop();
				AHTabControl.tmrAnimation = null;
			}

		}
		private void HideControllersSizingParent( DockStateControllerBase ddc )
		{
			DockControllerBase sizingparent = null;
			if( ddc.ParentController != null )
			{
				if( ddc.ParentController is SizingController )
					sizingparent = ddc.ParentController;
				else
					sizingparent = ddc.ParentController.ParentController;
			}

			if( sizingparent != null )
				sizingparent.LayoutRect = Rectangle.Empty;
		}

		protected void StopHideAnimation()
		{
			AnimationState = AutoHideAnimationState.None;

			this.RemoveAHTabControlTimer("HideAnimation");

			if( ddcAnimation != null )
			{
				if( this.ddcAnimation.HostControl.Controls[0].Anchor != ( AnchorStyles.Left | AnchorStyles.Top ) )
					this.ddcAnimation.HostControl.Controls[0].Anchor = AnchorStyles.Left | AnchorStyles.Top;

				this.ddcAnimation.bAutoHideSizing = false;

				this.HideControllersSizingParent(ddcAnimation);


				this.nClientIndex = -1;
				if( this.dockingMgr.AHInViewTab != null )
					this.dockingMgr.AHInViewTab = null;

				this.InitiateAnimationStopEvent(this.ddcAnimation, this.ddcAnimation.HostControl.Controls[0]);
			}
		}

		protected void ShowController_TickEventHandler(Object sender, EventArgs e)
		{
			if(AHTabControl.tmrShowController != null)
			{
				this.AnimationState = AutoHideAnimationState.Showing;
				//Debug.Assert((this.nClientIndex >= 0), "Error: Client Index should be a valid tab index.\n");
				if((this.nClientIndex >= 0) && (this.nClientIndex < this.TabPages.Count))
				{
					DockStateControllerBase dhsel = (this.TabPages[this.nClientIndex] as AHTabPage).m_dhcClient;
					this.ShowController(dhsel, true);					
				}
				else
				{
					AHTabControl.tmrShowController.Tick -= new System.EventHandler(this.ShowController_TickEventHandler);
					AHTabControl.tmrShowController.Stop();				
					AHTabControl.tmrShowController = null;
				}
			}
		}

		protected void HideController_TickEventHandler(Object sender, EventArgs e)
		{
			if( AHTabControl.tmrAnimation == null )
			{
				if(AHTabControl.tmrHideController != null)
				{
					this.AnimationState = AutoHideAnimationState.Hiding;
					//Debug.Assert((this.nClientIndex >= 0), "Error: Client Index should be a valid tab index.\n");
					if((this.nClientIndex >= 0) && (this.nClientIndex < this.TabPages.Count))
					{
						DockStateControllerBase dhsel = (this.TabPages[this.nClientIndex] as AHTabPage).m_dhcClient;
						if(dockingMgr.ForceHideActiveControl || dhsel.HostControl.ContainsFocus == false )
							this.HideController(dhsel, true);
					}
					else
					{
						AHTabControl.tmrHideController.Tick -= new System.EventHandler(this.HideController_TickEventHandler);
						AHTabControl.tmrHideController.Stop();
						AHTabControl.tmrHideController = null;
					}
				}
			}
		}

		protected internal void StopAllAnimations()
		{
			if( tmrShowController == null && tmrHideController == null && tmrAnimation == null )
				return;
			switch( AnimationState )
			{
				case AutoHideAnimationState.Hiding:
					StopHideAnimation();
					break;
				case AutoHideAnimationState.Showing:
					StopShowAnimation();
					break;
			}
		}

		public void UpdateAHTabText(DockStateControllerBase ddcbase)
		{
			foreach(TabGroupData groupdata in this.TabsData)
			{
				if(groupdata.Items.Count > 1)
				{
					foreach( TabGroupItem subitem in groupdata.Items )
					{
						if(subitem.Tag == ddcbase)
						{
							subitem.Text = ddcbase.HostControl.Text;							
							break;
						}
					}
				}
				else if( groupdata.Items[0].Tag == ddcbase )
				{
					groupdata.Items[0].Text = ddcbase.HostControl.Text;
					break;
				}
			}		
		}

		public void UpdateAHTabImage(DockStateControllerBase ddcbase)
		{
			DockHostController dhc = ddcbase as DockHostController;
			foreach(TabGroupData groupdata in this.TabsData)
			{
				if(groupdata.Items.Count > 1)
				{
					foreach( TabGroupItem subitem in groupdata.Items )
					{
						if(subitem.Tag == ddcbase)
						{
							subitem.ImageIndex = dhc.ImageIndex;							
							break;
						}
					}
				}
				else if( groupdata.Items[0].Tag == ddcbase )
				{
					groupdata.Items[0].ImageIndex = dhc.ImageIndex;
					break;
				}
			}		
		}
        private void InitializeDefaultMenu(PopupMenu menu)
        {
			menu.ParentBarItem.Style = DockingManager.VisualStyle;
            foreach (AHTabPage ah in this.TabPages)
            {
                if (ah.m_dhcClient is DockTabController)
                {
                    DockTabController dtc = ah.m_dhcClient as DockTabController;
                    foreach (DockTabPage dtp in dtc.TabControl.TabPages)
                    {
                        BarItem bitem = new BarItem(dtp.dhcClient.DockLabel);
                        
                        DockingManager dm = dtc.DockingManager;
                        //FR 1529 - Need an option to show DockIcon in the AutoHideDockTab ContextMenu 
                        if (dtc.TabControl.ImageList != null && dm.ShowIconInAutoHideContextMenu)
                        {
                            bitem.ImageList = dtc.TabControl.ImageList;
                            bitem.ImageIndex = dtp.ImageIndex;
                        }

                        bitem.Tag = dtp.dhcClient.HostControl.Controls[0];
                        bitem.Click += new EventHandler(OnAutohideMenu_Click);
                        menu.ParentBarItem.Items.Add(bitem);
                    }
                }
                else
                {
                    BarItem bitem = new BarItem((ah.m_dhcClient as DockHostController).DockLabel);
                    
                    //FR 1529 - Need an option to show DockIcon in the AutoHideDockTab ContextMenu                    
                    DockingManager dm = (ah.m_dhcClient as DockHostController).DockingManager;
                    if (dm.ImageList != null && dm.ShowIconInAutoHideContextMenu)
                    {
                        bitem.ImageList = dm.ImageList;
                        bitem.ImageIndex = dm.GetDockIcon(ah.m_dhcClient.HostControl.Controls[0]);
                    }

                    bitem.Tag = ah.m_dhcClient.HostControl.Controls[0];
                    bitem.Click += new EventHandler(OnAutohideMenu_Click);
                    menu.ParentBarItem.Items.Add(bitem);
                }
            }
        }
        void OnAutohideMenu_Click(object sender, EventArgs e)
        {
            BarItem item = sender as BarItem;
            if (item.Tag is Control)
            {
                Control ctrl = item.Tag as Control;
                DockingManager.ActivateControl(ctrl);
            }

        }

		private Point m_dragStartPoint = Point.Empty;
		private int m_dragOffset = 2;

		protected override void OnMouseDown(MouseEventArgs e)
		{
            if (e.Button == MouseButtons.Right && this.DockingManager.EnableAutoHideTabContextMenu )
            {
                PopupMenu menu = new PopupMenu();
                menu.ParentBarItem = new ParentBarItem();
                this.InitializeDefaultMenu(menu);

                AutoHideTabContextMenuEventArgs ahcmneuargs = new AutoHideTabContextMenuEventArgs(menu, Edge);
                this.DockingManager.FireAutoHideTabContextMenuEvent(ahcmneuargs);
                if (ahcmneuargs.ContextMenu != null)
                    ahcmneuargs.ContextMenu.Show(this, new Point(e.X, e.Y));
                return;
            }

            //When the EnableDragAutohiddenTabs is true and DisAllowFloating is set to true,dragging the autohidden controls floats the control.Fixed.

            if (e.Button == MouseButtons.Left && this.dockingMgr.EnableDragAutoHiddenTabs && !this.dockingMgr.DisallowFloating) 

				m_dragStartPoint = new Point(e.X, e.Y);
            if (dockingMgr.VisualStyle == VisualStyle.Metro)
                this.SelectAutoHideWindow(e);
			if (this.dockingMgr.AutoHideSelectionStyle != AutoHideSelectionStyle.Click)
				return;

			base.OnMouseDown(e);

			if(AHTabControl.tmrAnimation != null)
				return;

			Point ptclient = new Point(e.X, e.Y);
			Point ptscreen = this.PointToScreen(ptclient);
			int ngroup = this.GetTabGroupIndex(ptclient);

			if(this.nClientIndex >= 0)
			{
				DockStateControllerBase dhsel = (this.TabPages[this.nClientIndex] as AHTabPage).m_dhcClient;
				if(this.nClientIndex == ngroup)
				{
					dhsel.HostControl.Focus();					
				}
				else if( (this.RectangleToScreen(this.Bounds).Contains(ptscreen) == false) && 
					(dhsel.HostControl.RectangleToScreen(dhsel.HostControl.ClientRectangle).Contains(ptscreen) == false) )
				{
					if(AHTabControl.tmrHideController != null)
					{
						if(AHTabControl.tmrShowController != null)
						{						
							AHTabControl.tmrShowController.Tick -= new System.EventHandler(this.ShowController_TickEventHandler);
							AHTabControl.tmrShowController.Stop();
							AHTabControl.tmrShowController = null;
						}
						if(AHTabControl.tmrHideController != null)
						{
							AHTabControl.tmrHideController = new System.Windows.Forms.Timer();
							AHTabControl.tmrHideController.Tick += new System.EventHandler(this.HideController_TickEventHandler);
							AHTabControl.tmrHideController.Interval = DockingManager.nHideInterval;
							AHTabControl.tmrHideController.Start();
						}
					}
				}
			}

            if (dockingMgr.VisualStyle != VisualStyle.Metro)
                this.SelectAutoHideWindow(e);
		}

        protected override void OnMouseLeave(EventArgs e)
        {
            if (DockingManager.VisualStyle == VisualStyle.Metro && this.ClientIndex >= 0)
            {
                this.ClientIndex = -1;
                this.InvalidatePanel();
            }
            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //When the EnableDragAutohiddenTabs is true and DisAllowFloating is set to true,dragging the autohidden controls floats the control.Fixed.

            if (m_dragStartPoint != Point.Empty && this.dockingMgr.EnableDragAutoHiddenTabs && !this.dockingMgr.DisallowFloating) 

			{
				if( Math.Abs(m_dragStartPoint.X - e.X) > m_dragOffset
					|| Math.Abs(m_dragStartPoint.Y - e.Y) > m_dragOffset )
				{
					DockStateControllerBase target = GetControlFromPoint(new Point(e.X, e.Y));
					TransitControlToFloatState( target);
				}
			}

            if (this.dockingMgr.AutoHideSelectionStyle != AutoHideSelectionStyle.MouseHover)
                return;
            base.OnMouseMove(e);
            this.SelectAutoHideWindow(e);
        }

		internal virtual void TransitControlToFloatState( DockStateControllerBase target )
		{
			int floatFrameOffset = 10;
			Point ptScreen = Cursor.Position;
			ptScreen.Offset(-floatFrameOffset, -floatFrameOffset);
			FloatingForm ff = null;

			if( target != null )
			{
				target.DINew.rcDockArea.Height = 0;
				target.ParentController.RemoveChild(target);
				target.LayoutRect = new Rectangle(target.LayoutRect.Location
					, target.DITransient.rcDockArea.Size);
				this.RemoveTab(target);
				target.bAutoHideSizing = false;
				target.bInAutoHide = false;

				if( target is DockTabController )
				{
					DockTabController tabCtrl = target as DockTabController;
					int nrel = Guid.NewGuid().GetHashCode();
					DCRelationship tabRel = new DCRelationship(nrel, false, DockPreference.Tabbed, 0);
					int index = 0;

					foreach( DockTabPage page in tabCtrl.TabControl.TabPages )
					{
						DockHostController dhc = page.dhcClient;
						tabRel.nIndex = index;
						dhc.bInAutoHide = false;
						dhc.bAutoHideSizing = false;
						dhc.HostControl.Region = null;
						dhc.DockDCRList.Add(tabRel);
						dhc.PrevAutohideStyle = this.Edge;
						index++;
					}

					target = tabCtrl.HostController;
					target.HostControl.Visible = true;
					tabCtrl.TabControl.Visible = true;
					tabCtrl.DINew.rcDockArea.Size = target.DIPrevious.rcDockArea.Size;
					ff = tabCtrl.CreateFloatingFrame(ptScreen);
				}
				else if( target is DockHostController )
				{
					DockHostController dhc = target as DockHostController;
					dhc.HostControl.Region = null;
					dhc.HideCaption = false;
					dhc.HostControl.Visible = true;
					ff = dhc.CreateFloatingFrame(ptScreen, true);
				}

				target.PrevAutohideStyle = this.Edge;

				if( ff != null )
				{
					this.Capture = false;
					this.DockingManager.dcHostForm.AdjustLayout();
					InitiateFormDrag( target, floatFrameOffset, ff );
				}

				this.DockingManager.dcHostForm.AdjustLayout();
				this.m_dragStartPoint = Point.Empty;
			}
		}

		private void InitiateFormDrag( DockStateControllerBase target, int floatFrameOffset, FloatingForm ff )
		{
			Point curPoint = ff.Location;
			curPoint.Offset( floatFrameOffset, floatFrameOffset );
			FloatingFormController ffc = ff.InternalController as FloatingFormController;
			ff.RefreshRenderer();

			curPoint = ff.PointToClient( curPoint );
			ffc.HandleMouseDownImp( MouseButtons.Left, curPoint );

			this.dockingMgr.InitiateFormDrag( ff, curPoint, new Point( floatFrameOffset, floatFrameOffset ) );
		}

		protected override void OnMouseUp( MouseEventArgs e )
		{
			m_dragStartPoint = Point.Empty;

			base.OnMouseUp(e);
		}

        private void SelectAutoHideWindow(MouseEventArgs e)
        {
            if (TopLevelControl != null)
            {
                Form parent = TopLevelControl as Form;
                
                if (!(parent != null && parent.IsMdiContainer) && !TopLevelControl.ContainsFocus)
                    return;
            }

            Point ptclient = new Point(e.X, e.Y);
            Point ptscreen = this.PointToScreen(ptclient);

            ClientIndex = -1;
            if (m_tabPanelRenderer.HitTestTabs(new PointF(e.X, e.Y), false) == -1)
            {
                if(dockingMgr.VisualStyle == VisualStyle.Metro)
                this.Invalidate();
                return;

            }

            int ngroup = GetTabGroupIndex(ptclient);
            if (dockingMgr.VisualStyle == VisualStyle.Metro)
            {
                ClientIndex = ngroup;
                this.Invalidate();
                if (e.Clicks == 0)
                    return;
                this.selectedIndex = ngroup;
            }
			if( this.nClientIndex >= 0 && ngroup >= 0 && nClientIndex != ngroup )
			{
				DockStateControllerBase ddcbase = ( this.TabPages[this.nClientIndex] as AHTabPage ).m_dhcClient;
				this.HideController( ddcbase, false, true );
			}

			if( ( AHTabControl.tmrAnimation != null )
				|| ( AHTabControl.tmrHideController != null )
				|| ( AHTabControl.tmrShowController != null ) )
				return;

            int nprevsubgroup = -1;
            if (ngroup >= 0)
            {
                if (this.dockingMgr != null && this.dockingMgr.AHInViewTab != null && this.dockingMgr.VisualStyle == VisualStyle.Metro)
                    this.dockingMgr.HideAutoHiddenControl(true);
                if ((this.dockingMgr.AHInViewTab != null) && (this.dockingMgr.AHInViewTab != this))
                {
                    // A docking window belonging to another autohide tabcontrol is being displayed. Hide that first.
                    this.dockingMgr.AHInViewTab.HideController(null, true);
                    if (this.dockingMgr.AutoHideSelectionStyle == AutoHideSelectionStyle.Click)
                    {
                        this.dockingMgr.HideAutoHiddenControl(false);
                        this.AnimationState = AutoHideAnimationState.Hiding;
                        this.StopAllAnimations();
                        this.SelectAutoHideWindow(e);
                    }
                    return;
                }

                // If nhit is a group, then get the sub-group index
                TabGroupData tgroupdata = this.TabsData[ngroup] as TabGroupData;
                if (tgroupdata.Items.Count > 1)
                {
                    int nsubgroup = GetTabSubGroupIndex(ngroup, ptclient);
                    ClientIndex = nsubgroup;
                    // If the subgroupindex is different from the current selected sub-group index, then make the
                    // new subgroupindex, the selected tab)					
                    if (nsubgroup >= 0)
                    {
                        if (nsubgroup != tgroupdata.SelectedIndex)
                        {
                            //tgroupdata.SelectedIndex = nsubgroup;	// Sub-group selection happens in ShowController
                            // Make the dockhostcontroller that represents this subgroup, the hostcontroller for the docktab.
                            // This is accomplished by switching the docktab's selected index.
                            DockTabController docktabctrlr = (this.TabPages[ngroup] as AHTabPage).m_dhcClient as DockTabController;
                            Debug.Assert((docktabctrlr != null), "Error: Invalid group controller.\n");
                            docktabctrlr.HostController.DockTab.SelectedIndex = nsubgroup;
                            docktabctrlr.AdjustLayout();
                            if (ngroup == this.nClientIndex)
                            {
                                // A different subgroup within the same group is being activated
                                nprevsubgroup = tgroupdata.SelectedIndex;
                            }
                            else if (this.nClientIndex >= 0)
                            {
                                TabGroupData toldgroupdata = this.TabsData[this.nClientIndex] as TabGroupData;
                                if (toldgroupdata.Items.Count > 1)
                                    nprevsubgroup = toldgroupdata.SelectedIndex;
                            }
                        }
                    }
                    else	// Do not show any controls
                    {
                        ngroup = -1;
                    }
                }
            }

            if ((ngroup >= 0) && ((ngroup != this.nClientIndex) || (nprevsubgroup >= 0)))
            {
                // If a timerevent is in progress, stop timer.
                if (AHTabControl.tmrShowController != null)
                {
                    AHTabControl.tmrShowController.Tick -= new System.EventHandler(this.ShowController_TickEventHandler);
                    AHTabControl.tmrShowController.Stop();
                    AHTabControl.tmrShowController = null;
                    ngroup = -1;
                }

                if (this.nClientIndex >= 0)	// If a tab is currently being displayed, then hide it and show the new tab immediately.
                {
                    this.dockingMgr.AHInViewTab = null;

                    DockStateControllerBase dhcprev = (this.TabPages[this.nClientIndex] as AHTabPage).m_dhcClient;

                    // Use the nprevsubgroup index to get hold of the current visible DockHostController
                    Control dockctrl;
                    if ((nprevsubgroup >= 0) && (dhcprev is DockTabController))
                    {
                        DockTabController dtc = dhcprev as DockTabController;
                        DockTabPage tabpage = dtc.TabControl.TabPages[nprevsubgroup] as DockTabPage;
                        dockctrl = tabpage.dhcClient.HostControl.Controls[0];
                    }
                    else
                    {
                        dockctrl = dhcprev.HostControl.Controls[0];
                    }
                    this.InitiateHideAnimationStartEvent(dhcprev, dockctrl);

                    dhcprev.HostControl.Visible = false;
                    Rectangle rcprevlayout = dhcprev.LayoutRect;
                    if ((rcprevlayout.Width > 0) && (rcprevlayout.Height > 0))
                        dhcprev.DINew.rcDockArea = rcprevlayout;
                    dhcprev.ParentController.LayoutRect = Rectangle.Empty;

                    this.InitiateAnimationStopEvent(dhcprev, dockctrl);

                    this.nClientIndex = ngroup;
                    if (ngroup >= 0)
                        this.ShowController((this.TabPages[ngroup] as AHTabPage).m_dhcClient, true);
                }
                else	// Start the show timer
                {
                    this.nClientIndex = ngroup;
                    if (dockingMgr.VisualStyle == VisualStyle.Metro)
                        this.Invalidate();
                    if (ngroup >= 0)
                    {
                        AHTabControl.tmrShowController = new System.Windows.Forms.Timer();
                        AHTabControl.tmrShowController.Tick += new System.EventHandler(this.ShowController_TickEventHandler);
                        AHTabControl.tmrShowController.Interval = DockingManager.nShowInterval;
                        AHTabControl.tmrShowController.Start();
                    }
                }
            }
            else if (this.nClientIndex >= 0)
            {
                DockStateControllerBase dhsel = (this.TabPages[this.nClientIndex] as AHTabPage).m_dhcClient;
                if ((this.GetTabRect(this.nClientIndex).Contains(ptclient) == false) &&
                    (dhsel.ParentController.HostControl.RectangleToScreen(Rectangle.Inflate(dhsel.ParentController.LayoutRect, 2, 2)).Contains(ptscreen) == false)
                    && (dhsel.HostControl.ContainsFocus == false))
                {
                    if (AHTabControl.tmrHideController == null)
                    {
                        if (AHTabControl.tmrShowController != null)
                        {
                            AHTabControl.tmrShowController.Tick -= new System.EventHandler(this.ShowController_TickEventHandler);
                            AHTabControl.tmrShowController.Stop();
                            AHTabControl.tmrShowController = null;
                        }
                        AHTabControl.tmrHideController = new System.Windows.Forms.Timer();
                        AHTabControl.tmrHideController.Tick += new System.EventHandler(this.HideController_TickEventHandler);
                        AHTabControl.tmrHideController.Interval = DockingManager.nHideInterval;
                        AHTabControl.tmrHideController.Start();
                    }
                }
            }
        }

		protected DockStateControllerBase GetControlFromPoint( Point ptClient )
		{
			DockStateControllerBase targetController = null;
			int ngroup = GetTabGroupIndex(ptClient);
			int nprevsubgroup = -1;

			if( ngroup >= 0 )
			{
				// If nhit is a group, then get the sub-group index
				TabGroupData tgroupdata = this.TabsData[ngroup] as TabGroupData;
				if( tgroupdata.Items.Count > 1 )
				{
					int nsubgroup = GetTabSubGroupIndex(ngroup, ptClient);
					// If the subgroupindex is different from the current selected sub-group index, then make the
					// new subgroupindex, the selected tab)
					if( nsubgroup >= 0 )
					{
						if( nsubgroup != tgroupdata.SelectedIndex )
						{
							//tgroupdata.SelectedIndex = nsubgroup;	// Sub-group selection happens in ShowController
							// Make the dockhostcontroller that represents this subgroup, the hostcontroller for the docktab.
							// This is accomplished by switching the docktab's selected index.
							DockTabController docktabctrlr = ( this.TabPages[ngroup] as AHTabPage ).m_dhcClient as DockTabController;
							Debug.Assert(( docktabctrlr != null ), "Error: Invalid group controller.\n");
							docktabctrlr.HostController.DockTab.SelectedIndex = nsubgroup;
							docktabctrlr.AdjustLayout();
							if( ngroup == this.nClientIndex )
							{
								// A different subgroup within the same group is being activated
								nprevsubgroup = tgroupdata.SelectedIndex;
							}
							else if( this.nClientIndex >= 0 )
							{
								TabGroupData toldgroupdata = this.TabsData[this.nClientIndex] as TabGroupData;
								if( toldgroupdata.Items.Count > 1 )
									nprevsubgroup = toldgroupdata.SelectedIndex;
							}
						}
					}
					else	// Do not show any controls
					{
						ngroup = -1;
					}
				}
			}

			if( ngroup != -1 )
				 targetController = (this.TabPages[ngroup] as AHTabPage).m_dhcClient;
			
			return targetController;
		}

		protected int GetTabGroupIndex(Point pt)
		{
			int ntabheight = this.dockingMgr.AutoHideTabHeight;
			for(int i=0; i<this.TabPages.Count; i++)
			{
				Rectangle rctab = this.GetTabRect(i);				
				switch(this.tabBorder)
				{
					case DockingStyle.Left:
						rctab.X = 0;
						rctab.Width = ntabheight;
						break;
					case DockingStyle.Top:
						rctab.Y = 0;
						rctab.Height = ntabheight;
						break;
					case DockingStyle.Right:
						rctab.Width = ntabheight-2;
						break;
					case DockingStyle.Bottom:
						rctab.Height = ntabheight-2;
						break;
				}
				if(rctab.Contains(pt) == true)
					return i;
			}
			return -1;
		}
	
		protected int GetTabSubGroupIndex(int ngroup, Point pt)
		{
			TabGroupData tgroupdata = this.TabsData[ngroup] as TabGroupData;
			for(int i=0; i<tgroupdata.Items.Count; i++)
			{
				RectangleF rcitem = this.GetGroupItemRect(ngroup, i);
				// Apply the Point(3,3) padding here depending on the Tab orientation			
				RectangleF rcpaddeditem;				
				if((this.tabBorder == DockingStyle.Top) || (this.tabBorder == DockingStyle.Bottom))
					rcpaddeditem = new RectangleF(rcitem.Left-3, rcitem.Top, rcitem.Width+6, rcitem.Height);
				else
					rcpaddeditem = new RectangleF(rcitem.Left, rcitem.Top-3, rcitem.Width, rcitem.Height+6);
				if(rcpaddeditem.Contains(pt) == true)
					return i;
			}
			return -1;
		}

		public Rectangle GetActiveTabGroupOrSubGroupRect()
		{
			if(this.nClientIndex < 0)
				return Rectangle.Empty;
			TabGroupData tgroupdata = this.TabsData[this.nClientIndex] as TabGroupData;
			Rectangle rcitem = this.GetGroupItemRect(this.nClientIndex, tgroupdata.SelectedIndex);
			// Apply the Point(3,3) padding here depending on the Tab orientation			
			Rectangle rctab;				
			if((this.tabBorder == DockingStyle.Top) || (this.tabBorder == DockingStyle.Bottom))
				rctab = new Rectangle(rcitem.Left-3, rcitem.Top, rcitem.Width+6, rcitem.Height);
			else
				rctab = new Rectangle(rcitem.Left, rcitem.Top-3, rcitem.Width, rcitem.Height+6);
			return this.RectangleToScreen(rctab);
		}

		public Rectangle GetActiveTabGroupOrSubGroupDockHostRect()
		{
			if (this.nClientIndex < 0 || this.nClientIndex >= this.TabPages .Count )
				return Rectangle.Empty;
			DockStateControllerBase dhc = (this.TabPages[this.nClientIndex] as AHTabPage).m_dhcClient;
			Rectangle rect = dhc.HostControl.Parent.RectangleToScreen(dhc.ParentController.LayoutRect);            
			Rectangle ahTab = this.Parent.RectangleToScreen(this.Bounds);
			Rectangle form = this.dockingMgr.HostControl.RectangleToScreen(this.dockingMgr.dcHostForm.LayoutRect);
			rect = Rectangle.Intersect(rect, form);
			Region reg = new Region(rect);
			reg.Union(ahTab);
			RectangleF dockHostRect = Rectangle.Empty;
			RectangleF[] rectangles = reg.GetRegionScans(new Matrix());
            reg.Dispose();
            if( rectangles.Length > 0 )
                dockHostRect = rectangles[0];

			return Rectangle.Round(dockHostRect);
		}
		
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			base.SetBoundsCore(x, y, width, height, specified);	

			if(this.nClientIndex >= 0)
			{
				DockStateControllerBase ddcbase = (this.TabPages[this.nClientIndex] as AHTabPage).m_dhcClient;
				Rectangle rcbounds = CalcClientControlSize(ddcbase);	
				SetClientControlSize(ddcbase, rcbounds);
			}
		}

		protected Rectangle CalcClientControlSize(DockStateControllerBase ddcbase)
		{
			DockHostController dhc;
			if(ddcbase is DockTabController)
			{
				DockTabController dtc = ddcbase as DockTabController;
				dhc = (dtc.TabControl.SelectedTab as DockTabPage).dhcClient;				
			}
			else
				dhc = ddcbase as DockHostController;

			Rectangle rcbounds = Rectangle.Empty;
			int ntabheight = this.dockingMgr.AutoHideTabHeight;
			int nlead = this.bLeadSpace ? ntabheight : 0;		
			int ntrail = this.bTrailSpace ? ntabheight : 0;
			int nOffset = 0;
			switch(this.tabBorder)
			{
				case DockingStyle.Left:
					rcbounds = new Rectangle(this.Bounds.Right, this.Bounds.Top+nlead, dhc.DINew.rcDockArea.Width, this.Height-nlead-ntrail);
					if( ddcbase.bAutoHideSizing )
					{
						nOffset = rcVisibleBounds.Width - rcbounds.Width;
						rcbounds.Offset( nOffset, 0 );
						rcAnimation.Offset( nOffset, 0 );
					}
					break;
				case DockingStyle.Top:
					rcbounds = new Rectangle(this.Bounds.Left+nlead, this.Bounds.Bottom, this.Width-nlead-ntrail, dhc.DINew.rcDockArea.Height);
					if( ddcbase.bAutoHideSizing )
					{
						nOffset = rcVisibleBounds.Height - rcbounds.Height;
						rcbounds.Offset( 0, nOffset );
						rcAnimation.Offset( 0, nOffset );
					}
					break;
				case DockingStyle.Right:
					rcbounds = new Rectangle(this.Bounds.Left-dhc.DINew.rcDockArea.Width, this.Bounds.Top+nlead, dhc.DINew.rcDockArea.Width, this.Height-nlead-ntrail);
					if( ddcbase.bAutoHideSizing )
					{
						nOffset = rcbounds.Width - rcVisibleBounds.Width;
						rcbounds.Offset( nOffset, 0 );
						rcAnimation.Offset( nOffset, 0 );
					}
					break;
				case DockingStyle.Bottom:
					rcbounds = new Rectangle(this.Bounds.Left+nlead, this.Bounds.Top-dhc.DINew.rcDockArea.Height, this.Width-nlead-ntrail, dhc.DINew.rcDockArea.Height);
					if( ddcbase.bAutoHideSizing )
					{
						nOffset = rcbounds.Height - rcVisibleBounds.Height;
						rcbounds.Offset( 0, nOffset );
						rcAnimation.Offset( 0, nOffset );
					}
					break;
			}			
			return rcbounds;
		}

		protected void SetClientControlSize(DockStateControllerBase ddcbase, Rectangle rcbounds)
		{
			DockControllerBase splitter = ddcbase.ParentController.GetChildAt(1);
			splitter.HostControl.BringToFront();
			switch(this.tabBorder)
			{
				case DockingStyle.Left:					
					ddcbase.ParentController.LayoutRect = new Rectangle(rcbounds.X, rcbounds.Y, 
						rcbounds.Width+splitter.HostControl.Width, rcbounds.Height);
					break;
				case DockingStyle.Top:
					ddcbase.ParentController.LayoutRect = new Rectangle(rcbounds.X, rcbounds.Y, 
						rcbounds.Width, rcbounds.Height+splitter.HostControl.Height);
					break;
				case DockingStyle.Right:
					ddcbase.ParentController.LayoutRect = new Rectangle(rcbounds.X-splitter.HostControl.Width, rcbounds.Y, 
						rcbounds.Width+splitter.HostControl.Width, rcbounds.Height);
					break;
				case DockingStyle.Bottom:
					ddcbase.ParentController.LayoutRect = new Rectangle(rcbounds.X, rcbounds.Y-splitter.HostControl.Height, 
						rcbounds.Width, rcbounds.Height+splitter.HostControl.Height);
					break;
			}
		}

		protected void UpdateOffice2007Theme()
		{
			if (this.Renderer.Renderers.Count > 0)
			{
                if (this.Renderer.Renderers[0] is TabGroupRendererOffice2007)
                {
                    foreach (TabGroupRendererOffice2007 tgrOffice2007 in this.Renderer.Renderers)
                    {
                        tgrOffice2007.Theme = DockingManager.Office2007Theme;
                    }
                }
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (DockingManager.VisualStyle == VisualStyle.Office2007 ||
				DockingManager.VisualStyle == VisualStyle.Office2007Outlook)
			{
				UpdateOffice2007Theme();
			}
			base.OnPaint(e);
		}

        //Comment to avoid the exception while Clicking on the AutoHide pin
		/*public override Type TabStyle
		{
			get
			{
				return base.TabStyle;
			}
			set
			{
				Type groupRenderer = value.GetInterface("ITabGroupRenderer");
				if( groupRenderer != null )
					this.TabPanelData.TabStyle = ReflectionHelper.GetTabNameFromType( value );
			}
		}*/

	
		protected override void Dispose(bool bdisposing)
		{
			if((bdisposing == true) && (this.dockingMgr != null))
			{
				this.ImageList = null;
				this.dockingMgr.ImageListChanged -= new EventHandler(this.dockingManager_ImageListChanged);
				
				if(AHTabControl.tmrShowController != null)
				{					
					AHTabControl.tmrShowController.Tick -= new System.EventHandler(this.ShowController_TickEventHandler);
					AHTabControl.tmrShowController.Stop();
					AHTabControl.tmrShowController = null;
				}
				if(AHTabControl.tmrHideController != null)
				{					
					AHTabControl.tmrHideController.Tick -= new System.EventHandler(this.HideController_TickEventHandler);
					AHTabControl.tmrHideController.Stop();
					AHTabControl.tmrHideController = null;
				}	
				if(AHTabControl.tmrAnimation != null)
				{					
					AHTabControl.tmrAnimation.Tick -= new System.EventHandler(this.ShowAnimation_TickEventHandler);
					AHTabControl.tmrAnimation.Tick -= new System.EventHandler(this.HideAnimation_TickEventHandler);
					AHTabControl.tmrAnimation.Stop();				
					AHTabControl.tmrAnimation = null;
				}

                SystemEvents.UserPreferenceChanged -= (UserPreferenceChangedEventHandler)Delegate.CreateDelegate(
typeof(UserPreferenceChangedEventHandler), this, "UserPreferenceChanged");

				if(this.tdTab != null)
				{
					this.tdTab.Dispose();
					this.tdTab = null;
				}
				this.dockingMgr = null;
			}
			base.Dispose(bdisposing);
		}

		public void UpdateRenderer()
		{
            if (dockingMgr != null)
                TabStyle = GetRendererType();

            this.SetItemSize();
            this.Invalidate();
		}

        private void SetItemSize()
        {
            if( dockingMgr != null )
            {
                this.ItemSize = new Size( 0, dockingMgr.AutoHideTabHeight );
            }
        }

		private Type GetRendererType()
		{
			Type rendererType;
			switch( dockingMgr.VisualStyle )
			{
				case VisualStyle.Default :
					rendererType = typeof( TabGroupRenderer );
					break;
				case VisualStyle.Office2003 :
				case VisualStyle.OfficeXP :
					rendererType = typeof( Renderers.TabGroupRendererOffice2003 );
					break;
                case VisualStyle.Metro: 
                    rendererType = typeof(Renderers.TabGroupRendererVS2012);
                    break;
				case VisualStyle.VS2005:
					rendererType = typeof( Renderers.TabGroupRendererVS2005 );
					break;
                case VisualStyle.VS2010:
                    rendererType = typeof(Renderers.TabGroupRendererVS2010);
                    break;
				case VisualStyle.Office2007:
				case VisualStyle.Office2007Outlook:
					rendererType = typeof( Renderers.TabGroupRendererOffice2007 );
					break;
				default :
					rendererType = typeof( TabGroupRenderer );
					break;
			}
			return rendererType;
		}
	}
}
