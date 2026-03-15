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
using System.Windows.Forms.Design;
using System.Reflection;
using System.Runtime.Serialization;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Runtime.InteropServices;	
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools.Renderers;

namespace Syncfusion.Windows.Forms.Tools
{		

	/// Helper Recursive functions	
	[Syncfusion.Documentation.DocumentationExclude()]
	class DockUtilities
	{
		public static void RecGetChildControllers(DockControllerBase dc, ArrayList alchildren)
		{
			if(dc is DockHostController)
			{
				alchildren.Add(dc);
				return;
			}			
			for(int i = 0; i < dc.ChildCount; i++)
				RecGetChildControllers(dc.GetChildAt(i), alchildren);				
		}
	}


	// DockHostControllers provide the visual feedback while dragging and are responsible for 
	// sizing the host controls. 
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DockHostController : DockStateControllerBase, IResizable
	{
		protected DockHost ctrlHost = null;
		// Relationship lists
		protected ArrayList alDockDCR = null;		
		protected ArrayList alFloatDCR = null;
		protected ArrayList siblingDCR = new ArrayList();

		// References the shared tabcontrol
		protected DockTabControl ctrlDockTab = null;	
		protected internal bool bDockVisible = true;
		protected bool bFloatOnly = false;
		protected bool bFloating = false;
		protected bool bHideCaption = false;
		protected bool bClosing = false;
	
		protected bool bAHVisible = true;
		protected bool bCloseVisible = true;
		protected bool bMaxVisible = true;
		protected bool bMenuBtnVisible = false;
		private bool m_bIsMirrored = false;
		private string m_UniqueName = "";
		private bool bAHStateOnClose = false;
		private bool m_bMdiChildState = false;
		private FormWindowState m_mdiWindowState = FormWindowState.Normal;
        internal bool bClosingByMouse = false;
		// MDI - switching
		public bool bInMDIMode = false;
		public Control ctrlReference = null;

		protected const int nMinControlWidth = 25;
		protected const int nMinControlHeight = 45;
		protected Size m_storedSize = Size.Empty;
		protected Rectangle m_mdiChildBounds = Rectangle.Empty;
		protected bool m_autoHiddedBeforeHide = false;

		private Point controlLocation;
		private Size controlSize;

		/// <summary>
		/// Gets/sets if control is closing.
		/// </summary>
		protected internal bool Closing
		{
			get
			{
				return bClosing;
			}
			set
			{
				if( bClosing != value )
				{
					bClosing = value;
				}
			}
		}

		/// <summary>
		/// Gets / sets the Control's location in undocked mode.
		/// </summary>
		protected internal Point ControlLocation
		{
			get { return controlLocation; }
			set
			{
				if( controlLocation != value )
				{
					controlLocation = value;
				}
			}
		}
		/// <summary>
		/// Gets/Sets size of controller, when controllers dock visibility is false
		/// and must be set to controller after showing it.
		/// </summary>
		internal Size StoredSize
		{
			get
			{
				return m_storedSize;
			}
			set
			{
				if( m_storedSize != value )
				{
					m_storedSize = value;
				}
			}
		}

		internal bool MdiChildState
		{
			get
			{
				return m_bMdiChildState;
			}
			set
			{
				if (m_bMdiChildState != value)
				{
					m_bMdiChildState = value;
				}
			}
		}

		internal Rectangle MdiChildBounds
		{
			get
			{ 
				Rectangle bounds = Rectangle.Empty;
				if( this.bInMDIMode || this.MdiChildState )
				{
					if( this.DockVisibility )
					{
						if( this.ctrlReference != null )
							bounds = this.ctrlReference.Parent.Bounds;
					}
					else
						bounds = m_mdiChildBounds;

				}

				return bounds;
			}
			set
			{
				if( m_mdiChildBounds != value )
				{
					m_mdiChildBounds = value;
				}
			}
		}

		internal FormWindowState MdiWindowState
		{
			get
			{
				return m_mdiWindowState;
			}
			set
			{
				if (m_mdiWindowState != value)
				{
					m_mdiWindowState = value;
				}
			}
		}
		internal override ArrayList SiblingDCR
		{
			get
			{
				return siblingDCR;
			}
		}

		/// <summary>
		/// Gets / sets the Control's size in undocked mode.
		/// </summary>
		protected internal Size ControlSize
		{
			get { return controlSize; }
			set
			{
				if ( controlSize != value )
				{
					controlSize = value;
				}
			}
		}

		public bool IsSelectedTabPage
		{
			get
			{
				DockTabController dockTabController = ParentController as DockTabController;
				if( ( dockTabController != null ) &&
					( dockTabController.SelectedController == this ) )
				{
					return true;
				}
				else
				{
					if( this.bInMDIMode && dockingMgr.HostFormMdiManager != null )
					{
						TabbedMDIManager mdiManager = this.dockingMgr.HostFormMdiManager;

						if( mdiManager.TabGroupHosts.Length > 0 )
						{
							MDITabPanel panel = mdiManager.TabGroupHosts[0].MDITabPanel;
							if( panel != null && panel.TabPages.Count != 0 )
							{
								TabPageAdv selectedPage = panel.SelectedTab;
								MDIChildTabData mdiData = selectedPage.TabData as MDIChildTabData;

								if( mdiData != null && mdiData.MdiChild != null
									&& mdiData.MdiChild is DockingWrapperForm && mdiData.MdiChild.Controls.Count != 0 )
								{
									Control dmControl = mdiData.MdiChild.Controls[0];
									if( this.ctrlReference == dmControl )
										return true;
								}
							}
						}
					}
				}

				return false;
			}
		}

		public string UniqueName
		{
			get	{ return m_UniqueName; }
			set { m_UniqueName = value; }
		}

		internal void RemoveTabRelation( bool floatingRel )
		{
			DockStateControllerWrapper wrapper = floatingRel ? this.InternalFloatWrapper : this.InternalDockWrapper;

			if( wrapper != null &&
				wrapper.DockRelationControllers.Contains( this ) )
				wrapper.DockRelationControllers.Remove( this );
		}

		public override DockControllerBase ParentController
		{
			set 
			{ 
				this.dcParent = value;
				if(value != null)
				{
					if((value is FloatingFormController) || 
						((value is DockTabController) && (value.ParentController is FloatingFormController)) )
					{
						this.HideCaption = true;
					}
					else
					{
						this.HideCaption = false;
					}

					DockControllerBase parentCtrlr = this.ParentController;

					if( parentCtrlr is DockTabController )
						parentCtrlr = parentCtrlr.ParentController;

					DockStateControllerWrapper wrapper = parentCtrlr.Floating 
						? this.InternalFloatWrapper : this.InternalDockWrapper;

					if( !this.bInAutoHide && wrapper != null
						&& parentCtrlr != wrapper.ParentController )
						RemoveTabRelation( parentCtrlr.Floating );
				}
			}

			get 
			{ 
				return this.dcParent; 
			} 
		}	

		/// <summary>
		/// Returns the host control.
		/// </summary>
		public override Control HostControl
		{
			get { return this.ctrlHost; }
		}

		/// <summary>
		/// Gets / sets the dock label for the controller.
		/// </summary>
		public String DockLabel
		{
			get
			{
				return this.HostControl.Text;
			}
			set
			{
				if(this.ctrlHost.Text != value)
				{
					String oldtext = this.ctrlHost.Text;
					this.ctrlHost.Text = value;
					if(this.bDockVisible == true)
					{
						this.ctrlHost.Invalidate(true);
						DockTabController tabctrl = null;

						if(this.dockInfoCurrent.DP == DockPreference.Tabbed)
						{
							tabctrl = this.ParentController as DockTabController;
							Debug.Assert((tabctrl != null), "Error: Invalid Parent Controller.\n");
							foreach(DockTabPage page in tabctrl.TabControl.TabPages)
							{
								if(page.dhcClient == this)
									page.Text = value;
							}
						}

						if(this.bFloating == true)
						{
							Form floatingfrm = this.ctrlHost.ParentForm;
							if(floatingfrm.Text == oldtext)
								floatingfrm.Text = value;
						}

						if(this.bInAutoHide == true)
						{
							MainFormController mfctrlr = this.ToplevelController as MainFormController;
							DockingStyle dockStyle = this.DINew.dStyle;

							// if control is tabbed, get docked side from its parent
							if( tabctrl != null )
								dockStyle = tabctrl.DICurrent.dStyle;
							AHTabControl ahTab = mfctrlr.GetAHTabControl( dockStyle );
							if( ahTab != null )
							{
								ahTab.UpdateAHTabText( this );
								ahTab.ForceLayout();
							}
						}
					}
				}
			}
		}

        //Newly Added
        public int MdiImageIndex
        {
            get { return this.ctrlHost.MdiImageIndex; }
            set
            {
                if (this.ctrlHost.MdiImageIndex != value)
                {
                    this.ctrlHost.MdiImageIndex = value;
                    this.ctrlHost.Invalidate(true);

                    if (this.dockingMgr.DesignMode == true && !this.dockingMgr.InBeginEndInit
                        && !this.dockingMgr.ReloadingDesigner)
                    // Forcibly update the designer state
                    {
                        this.dockingMgr.UpdateDesigner();
                        this.dockingMgr.UpdateFloatingFormsImages();
                    }
                }
            }
        }
		
        /// <summary>
        /// Gets / sets the image index for the host controller.
        /// </summary>
		public int ImageIndex
		{
			get { return this.ctrlHost.ImageIndex; } 
			set 
			{ 
				if(this.ctrlHost.ImageIndex != value)
				{
					this.ctrlHost.ImageIndex = value;
					if(this.bDockVisible == true)
					{
						this.ctrlHost.Invalidate(true);

						if(this.dockInfoCurrent.DP == DockPreference.Tabbed)
						{
							DockTabController tabctrl = this.ParentController as DockTabController;
							Debug.Assert((tabctrl != null), "Error: Invalid Parent Controller.\n");
							foreach(DockTabPage page in tabctrl.TabControl.TabPages)
							{
								if(page.dhcClient == this)
									page.ImageIndex = value;					
							}					
						}

						if(this.bInAutoHide == true)
						{
							MainFormController mfctrlr = this.ToplevelController as MainFormController;
							mfctrlr.GetAHTabControl(ParentController.DICurrent.dStyle).UpdateAHTabImage(this);
						}
					}

					if( this.dockingMgr.DesignMode == true && !this.dockingMgr.InBeginEndInit 
						&& !this.dockingMgr.ReloadingDesigner )
						// Forcibly update the designer state
						this.dockingMgr.UpdateDesigner();
				}
			}
		}

		/// <summary>
		/// Gets / sets the control image for the controller.
		/// </summary>
		public Icon ControlImage
		{
			get { return this.ctrlHost.controlImage; }
			set { this.ctrlHost.controlImage = value; }
		}

		/// <summary>
		/// Gets / sets the layout rectangle for the controller.
		/// </summary>
		public override Rectangle LayoutRect
		{
			get { return this.ctrlHost.Bounds; }
			set 
			{
				if( this.Minimized == Minimization.Vertical )
					value.Size = new Size(value.Size.Width, ctrlHost.Bounds.Height);
				if( this.Minimized == Minimization.Horizontal )
					value.Size = new Size(ctrlHost.Bounds.Width, value.Size.Height);
				if( this.FreezeResize )
				{
					if( !this.IsVerticallyResizable() )
						value.Size = new Size( value.Size.Width, this.LayoutRect.Height );
					if( !this.IsHorizontallyResizable() )
						value.Size = new Size( this.LayoutRect.Width, value.Size.Height);
				}

				// If the host control dimensions are out of sync with the LayoutRect, then update the LayoutRect and 
				// the DICurrent information
               
                 if (value.Width == 0 && !this.bInAutoHide)
                   value.Width= 1;
               
                if (this.ctrlHost.Bounds.Equals(value) == false)
                     this.ctrlHost.Bounds = value;
                
				this.dockInfoCurrent.rcDockArea = this.ctrlHost.RectangleToScreen(new Rectangle( Point.Empty, this.ctrlHost.Size ));
			}
		}
		
		public override bool Floating
		{
			get 
			{ 
				DockControllerBase controller = this;
				if( this.DockVisibility )
				{
					while( controller.ParentController != null )
						controller = controller.ParentController;
				}
				else
				{
					while( controller.DICurrent.dController != null 
						&& !(controller is FloatingFormController) )
					{
						controller = controller.DICurrent.dController;

						if( controller == this )
							break;
					}
				}
				return controller is FloatingFormController;
			}
			set	{ this.bFloating = value; }
		}
		
		public override int ChildCount
		{
			get { return -1; }
		}

		public override int ChildHostCount
		{
			get { return 0; }
		}

		public override IEnumerator DCR
		{
			get
			{
				if(this.Floating == true)
					return this.alFloatDCR.GetEnumerator();
				else 
					return this.alDockDCR.GetEnumerator();
			}
		}

		
		public override DCRelationship DCRCurrent
		{
			// Add/return the dcr from the head of the relevant list
			get 
			{
				if(this.Floating == true)
				{
					if(this.alFloatDCR.Count > 0)
						return this.alFloatDCR[0] as DCRelationship;
					else
						return null;
				}
				else
				{
					if(this.alDockDCR.Count > 0)
						return this.alDockDCR[0] as DCRelationship;
					else
						return null;
				}
			}

			set 
			{
				if(this.Floating == true)
					this.InsertIntoDCR(this.alFloatDCR, 0, value);
				else
					this.InsertIntoDCR(this.alDockDCR, 0, value);
			}
		}

		public ArrayList DockDCRList
		{
			get { return this.alDockDCR; }
			set { this.alDockDCR = value; }
		}

		public ArrayList FloatDCRList
		{
			get { return this.alFloatDCR; }
			set { this.alFloatDCR = value; }
		}

		/// <summary>
		/// Gets / sets the visibility state of the controller.
		/// </summary>
		public bool DockVisibility
		{
			get
			{ 
				if((this.bInMDIMode == true) && (this.bDockVisible == false))
				{
					// The dockcontrol is being displayed as an MDI child.
					if(this.HostControl.Controls.Count == 0)
						return true;
				}
				return this.bDockVisible;	
			}

			set 
			{
				if(value != this.bDockVisible)
				{
					if(value == true)
					{
						DockVisibilityChangingEventArgs arg = bClosingByMouse ?
							new DockVisibilityChangingEventArgs(ctrlHost.Controls[0], false, DockingAction.ByMouse) : new DockVisibilityChangingEventArgs(ctrlHost.Controls[0]);
						if (DockingManager.bCancelVisibilityChangingEvent || !this.bInMDIMode)
						{
							if( !dockingMgr.bFiredVisibilityChangingEvent )
							{
								dockingMgr.FireDockVisibilityChangingEvent ( arg );
							}
						}
						if (!arg.Cancel && !dockingMgr.bCancelVisibilityChangingEvent)
						{
							DockVisibilityChangedEventArgs args = bClosingByMouse ?
								new DockVisibilityChangedEventArgs(ctrlHost.Controls[0], DockingAction.ByMouse) : new DockVisibilityChangedEventArgs(ctrlHost.Controls[0]);
							bool fired = this.DockingManager.bFiredVisibilityChangedEvent;
							if( m_autoHiddedBeforeHide )
							{
								ToggleAutoHideMode(false);
								this.ctrlHost.Controls[0].Visible = true;
								bDockVisible = true;
								if( !this.dockingMgr.bFiredVisibilityChangedEvent )
								{
									this.dockingMgr.FireDockVisibilityChangedEvent(args);
								}
							}
							else
							{
								this.DockingManager.bFiredVisibilityChangedEvent = true;
								if( !this.bInMDIMode && !this.bClosing  && !this.MdiChildState )
								{
									this.ShowController();
								}
								else
								{
									this.ctrlHost.Controls[0].Visible = true;
								}

								if( !fired )
									this.DockingManager.FireDockVisibilityChangedEvent(args);
								
							}
							if((this.bInMDIMode == true) && (this.bFloatOnly == false))
								this.dockingMgr.SetAsMDIChild(this.HostControl.Controls[0], true);

							this.dockingMgr.bFiredVisibilityChangedEvent = fired;
						}
						else
						{
							dockingMgr.bCancelVisibilityChangingEvent = true;
						}
					}
					else
					{
						m_autoHiddedBeforeHide = bInAutoHide;
						this.CloseController();
					}
				}
			}
		}

		public override DockInfo DIPrevious
		{
			get { return this.dockInfoPrevious; }
			set 
			{ 
				if(this.dockInfoPrevious.dController != null)
					this.dockInfoPrevious.dController.ControllerChanged -= new ControllerChangedEH(this.dhc_ControllerChanged);
				this.dockInfoPrevious = value;
			}
		}

		public DockTabControl DockTab
		{
			get { return this.ctrlDockTab; }
			set { this.ctrlDockTab = value; }
		}

		public override bool AutoHideMode
		{
			get
			{
				return bInAutoHide;
			}
			set 
			{ 
				if(this.bInAutoHide != value)
					this.ToggleAutoHideMode();
			}
		}

		internal bool AutoHiddedBeforeHide
		{
			get
			{
				return m_autoHiddedBeforeHide;
			}
			set
			{
				if( m_autoHiddedBeforeHide != value )
				{
					m_autoHiddedBeforeHide = value;
				}
			}
		}

		public bool FloatOnly
		{
			get { return this.bFloatOnly; }
			set
			{
				if (this.bFloatOnly != value)
				{
					if( value == true )
					{
						Control ctrl = this.ctrlReference;
						if( ctrl == null && this.HostControl.Controls.Count > 0 )
							ctrl = this.HostControl.Controls[0];

						this.DockingManager.FireDockStateChangeEvent( "DockStateChanging"
							, new DockStateChangeEventArgs( new Control[] { ctrl } ) );
						if( this.DockVisibility == true )
						{
							bool freezed = this.FreezeResize;
							this.FreezeResize = false;

							DockHostController sibling = null;
							Rectangle rctransient = Rectangle.Empty;
							bool hidden = false;

							if( this.bInMDIMode )
								this.DockingManager.SetAsMdiChild(this, false, Rectangle.Empty);
							if( this.bInAutoHide == true )
							{
								this.ToggleAutoHideMode();
								hidden = true;
							}
							if( this.dockInfoCurrent.DP == DockPreference.Tabbed )
							{
								DockTabController tabctrl = this.ParentController as DockTabController;
								Debug.Assert(( tabctrl != null ), "Error: Invalid Parent Controller.\n");

								if( hidden )
								{
									rctransient = tabctrl.HostController.DIPrevious.rcDockArea;
									foreach( DockTabPage dtp in tabctrl.TabControl.TabPages )
									{
										if( dtp.dhcClient != this )
											sibling = dtp.dhcClient;
									}
								}

								tabctrl.RemoveDockHostFromTab(this, !this.Floating);
							}

							DockTabController dtc = this.ParentController as DockTabController;

							if( dtc != null )
								dtc.RemoveDockHostFromTab(this, false);

							if( this.ParentController is SizingController
								&& !this.IsSingleFloatControl )
							{
								Point location = this.HostControl.PointToScreen(Point.Empty);
								this.dockingMgr.UndockFromController(this);
								bool showff = (PreviousFloatLocation == Point.Empty && PreviousFloatSize == Size.Empty);
								this.DINew.rcDockArea = Rectangle.Empty;
								this.SharedForm = null;
								FloatingForm ff = this.CreateFloatingFrame(location, showff);

								if( PreviousFloatLocation != Point.Empty
									&& PreviousFloatSize != Size.Empty )
								{
									ff.Location = PreviousFloatLocation;
									ff.Size = PreviousFloatSize;
									PreviousFloatLocation = Point.Empty;
									PreviousFloatSize = Size.Empty;
								}

								if( !showff )
									ff.Show();
							}

							if( sibling != null )
							{
								rctransient = sibling.DITransient.rcDockArea;
								this.dockingMgr.UndockFromController(sibling);
								sibling.LayoutRect = rctransient;
								sibling.ToggleAutoHideMode(false);
								sibling.DITransient.rcDockArea = rctransient;
								sibling.DINew.rcDockArea = rctransient;
							}

							this.FreezeResize = freezed;
						}

						this.bFloatOnly = value;
						this.DockingManager.FireDockStateChangeEvent( "DockStateChanged"
							, new DockStateChangeEventArgs( new Control[] { this.HostControl.Controls[0] } ) );
					}
					else
					{
						this.DockingManager.FireDockStateChangeEvent( "DockStateChanging"
							, new DockStateChangeEventArgs( new Control[] { this.HostControl.Controls[0] } ) );
						this.bFloatOnly = value;

						if( this.bInMDIMode == true )
							this.dockingMgr.SetAsMDIChild(this.HostControl.Controls[0], true);
						this.DockingManager.FireDockStateChangeEvent( "DockStateChanged"
							, new DockStateChangeEventArgs( new Control[] { this.HostControl.Controls[0] } ) );
					}
				}
			}
		}
		
		public bool HideCaption
		{
			get 
			{
				CheckCaptionVisibility();
				return ((this.bHideCaption) || (!this.DockingManager.ShowCaption)); 
			}
			set 
			{ 
				if(this.bHideCaption != value)
				{					
					this.bHideCaption = value;
					if( this.ParentController is DockTabController )
						this.ParentController.AdjustLayout();
					else
						this.AdjustLayout();
				}
			}
		}

		public bool MaximizeButtonVisibility
		{
			get
			{
				return this.bMaxVisible && CanMaximize;
			}
			set
			{
				if( this.bMaxVisible != value )
				{
					this.bMaxVisible = value;
					this.ctrlHost.Invalidate(true);
				}
			}
		}

		public bool AutoHideButtonVisibility
		{
			get { return this.bAHVisible; }
			set 
			{ 
				if(this.bAHVisible != value)
				{
					this.bAHVisible = value;
					this.ctrlHost.Invalidate(true);
				}
			}
		}

		public bool CloseButtonVisibility
		{
			get { return this.bCloseVisible; }
			set
			{ 
				if(this.bCloseVisible != value)
				{
					this.bCloseVisible = value;
					this.ctrlHost.Invalidate(true);
				}
			}
		}

		public bool MenuButtonVisiblity
		{
			get {	return this.bMenuBtnVisible; }
			set
			{
				if( this.bMenuBtnVisible != value )
				{
					this.bMenuBtnVisible = value;
					this.ctrlHost.Invalidate(true);
				}
			}
		}

		internal bool IsMirrored
		{
			get { return m_bIsMirrored; }

			set
			{ 
				if (m_bIsMirrored != value)
				{
					m_bIsMirrored = value;
					
					if (null != ctrlDockTab)
					{
						ctrlDockTab.RightToLeft = m_bIsMirrored ? 
							System.Windows.Forms.RightToLeft.Yes : 
							System.Windows.Forms.RightToLeft.No;
					}
					this.ctrlHost.Invalidate(true);
				}
			}
		}

		public override Size MinimumSize
		{
			get
			{
				Size mSize = this.minSize;
                
                if(!this.dockingMgr.bApplyMinMaxExtents)
                    mSize = Size.Empty;

				return mSize;
			}

			set
			{
				if(this.minSize != value)
				{
					this.minSize = value;
					FloatingFormController parentController = ParentController as FloatingFormController;
					if( parentController != null )
					{
						FloatingForm floatingForm = parentController.HostControl as FloatingForm;
						if( floatingForm != null )
							floatingForm.UpdateFormMinimumSize();
					}
				}
			}
		}		
		
		public DockHostController(DockingManager dmgr, DockHost host) : base(dmgr)
		{
			this.ctrlHost = host;
			this.alDockDCR = new ArrayList();
			this.alFloatDCR = new ArrayList();

			IsMirrored = dmgr.IsMirrored;
		}

		public override void AddToDCR(DCRelationship dcr)
		{
			ArrayList al = (this.Floating == true) ? this.alFloatDCR : this.alDockDCR;
			this.InsertIntoDCR(al, al.Count, dcr);
		}		

		public override void RemoveFromDCR( DCRelationship dcr )
		{
			if( this.alDockDCR.Contains(dcr) )
				this.alDockDCR.Remove(dcr);
			if( this.alFloatDCR.Contains(dcr) )
				this.alFloatDCR.Remove(dcr);
		}

		public override void InsertIntoDCR(ArrayList al, int nindex, DCRelationship dcr)
		{
			// If the arraylist param is null, then use the arraylist for the current state
			if(al == null)
				al = (this.Floating == true) ? this.alFloatDCR : this.alDockDCR;
			foreach(DCRelationship dcrexist in al)
			{
				if( (dcrexist.nRelation == dcr.nRelation) && (dcrexist.bChild == dcr.bChild) )
				{
					int nexistindex = al.IndexOf(dcrexist);
					if(nexistindex != nindex)
					{
						al.Remove(dcrexist);	// Remove and reinsert at location
						if(nexistindex < nindex)
							nindex--;
						al.Insert(nindex, dcr);						
					}
					else if(dcrexist.nIndex != dcr.nIndex)
					{
						dcrexist.nIndex = dcr.nIndex;
					}					
					return;
				}
			}
			al.Insert(nindex, dcr);
		}
		
		public override void AddChild(DockControllerBase dc, Syncfusion.Windows.Forms.Tools.DockingStyle db)
		{

		}

		public override void InsertChild(DockControllerBase dc, int i, Syncfusion.Windows.Forms.Tools.DockingStyle db)
		{

		}

		public override void RemoveChild(DockControllerBase dc)
		{
		}

		public override DockControllerBase GetChildAt(int index)
		{
			return null;
		}

		public override int GetChildHostIndex(DockControllerBase child)
		{
			return -1;
		}

		public override void AdjustLayout()
		{
			if(this.ctrlHost.Controls.Count > 0)
			{
				if(this.DockTab == null)
					this.ctrlHost.Controls[0].Bounds = this.ctrlHost.ClientRectangle;
			}
		}

        internal void CheckCaptionVisibility()
        {
			if( this.Floating )
			{
				FloatingFormController ffc = this.ToplevelController 
					as FloatingFormController;

				if( ffc != null )
				{
					if( ffc.HideChildCaptions )
						this.HideCaption = true;
					else
						this.HideCaption = false;
				}
			}
        }

		protected void CheckSiblingRelations()
		{
			foreach( ControllerDCRPair pair in this.SiblingDCR )
			{
				if( pair.Controller != null && pair.DCR != null )
					pair.Controller.RemoveFromDCR(pair.DCR);
			}
			this.siblingDCR.Clear();
		}
	
		public void ShowController()
		{	
			this.bDockVisible = true;
			if(this.dockInfoTransient.dController != null)
					this.dockInfoTransient.dController.ControllerChanged -= new ControllerChangedEH(this.TransientControllerChanged);

			this.dockInfoNew = new DockInfo(this.dockInfoTransient);

			if( m_prevWrapper != null
				&& m_prevWrapper.ParentController != null )
			{
				if( m_prevWrapper == this.InternalFloatWrapper
					&& !this.dockingMgr.DisallowFloating && this.AllowFloating )
				{
                    TransitToPrevFloat( this.DockingManager.bLoadVisibility );
				}
				else
				{
					TransitToPrevDock();
				}
			}
			else
			{
				this.dockingMgr.HostControl.Controls.Add(this.HostControl);
				SizingController sc = new SizingController(this.dockingMgr, this.dockingMgr.HostControl,
					DockPreference.Horizontal);
				sc.ParentController = this.dockingMgr.dcHostForm;
				sc.AddChild(this, DockingStyle.Left);				
				this.DITransient.rcDockArea = this.LayoutRect;
				sc.LayoutRect = this.LayoutRect;
				this.dockingMgr.dcHostForm.AddChild(sc, DockingStyle.Left);
				this.dockingMgr.dcHostForm.AdjustLayout();		
			}

			if( m_storedSize != Size.Empty )
				this.DockingManager.SetControlSize(this.HostControl.Controls[0], m_storedSize);

			m_storedSize = Size.Empty;
   		    this.ctrlHost.Controls[0].Visible = true;
			this.ctrlHost.Visible = true;
			
			if( this.bAHStateOnClose )
				this.DockingManager.SetAutoHideMode(this.ctrlHost.Controls[0] ,true);
			this.HostControl.Focus();
			if (!this.dockingMgr.bFiredVisibilityChangedEvent)
            {
			// Fire the DockVisibilityChanged event
			this.dockingMgr.FireDockVisibilityChangedEvent(new DockVisibilityChangedEventArgs(this.ctrlHost.Controls[0]));
            }
		}

		internal void TabhostCloseAllControllers()
		{
			Debug.Assert( ((this.Floating == true) && (this.ctrlDockTab != null)) );
			this.ParentController.CloseController();
		}

		protected internal FloatingFormController GetParentFloatingFormController()
		{
			DockTabController dtc = ParentController as DockTabController;
			if( dtc != null )
			{
				return dtc.ParentController as FloatingFormController;
			}
			else
			{
				return ParentController as FloatingFormController;
			}
		}

        private TabPageAdv FindTabPageByName(DockTabControl tabControl, string tabName)
        {
            foreach (TabPageAdv tp in tabControl.TabPages)
            {
                if (tp.Text == tabName)
                    return tp;
            }
            return null;
        }

		// Undock the controller from the present dock/float state and then hide and dispose it
		public override void CloseController()
		{
            if (bClosingByMouse && ctrlHost.Controls.Count == 0)
            {
                if (ctrlHost.IsDisposed)
                    ctrlHost.Visible = false;
                return;
            }
            DockVisibilityChangingEventArgs arg = bClosingByMouse ?
                new DockVisibilityChangingEventArgs(ctrlHost.Controls[0], false, DockingAction.ByMouse) : new DockVisibilityChangingEventArgs(ctrlHost.Controls[0]);
            if (DockingManager.bCancelVisibilityChangingEvent || !this.bInMDIMode)
			{
                if( !dockingMgr.bFiredVisibilityChangingEvent )
				    dockingMgr.FireDockVisibilityChangingEvent ( arg );
			}

            if (this.ParentController is FloatingFormController)
            {
                FloatingFormController floatController = this.ParentController as FloatingFormController;
                FloatingForm floatForm = floatController.HostControl as FloatingForm;
                floatForm.CancelVisibleChanging = arg.Cancel;
            }

            if (this.ParentController is DockTabController)
            {
                DockTabController dockTabController = this.ParentController as DockTabController;
                if (dockTabController.ParentController is FloatingFormController)
                {
                    DockTabControl tabControl = dockTabController.TabControl as DockTabControl;
                    if (!arg.Cancel)
                    {
                        TabPageAdv tp = FindTabPageByName(tabControl, this.DockingManager.GetDockLabel(arg.Control));
                        tp.TabVisible = false;
                    }
                }
            }

			if ( !arg.Cancel )
			{
				this.bDockVisible = false;
				if( this.ParentController != null )
				{
					if(this.ParentController.GetType() == typeof(DockTabController))
					{
						(this.ParentController as DockTabController).CloseController(this);
					}
					else	// Close this controller and return
					{
						if(this.Floating == true || ParentController is FloatingFormController)
						{
							this.ParentController.ControllerChanged += new ControllerChangedEH(this.TransientControllerChanged);
							this.dockInfoTransient = new DockInfo(this.ParentController, this.dockInfoCurrent.dStyle, this.dockInfoCurrent.nPriority, 
								this.ParentController.GetChildHostIndex(this), this.dockInfoCurrent.DP, this.dockInfoCurrent.rcDockArea);
							ExitFloatingFrame();
							if (this.FloatOnly)
								this.MdiChildState = false;
						}
						else
						{
							if(this.bInAutoHide == true)
							{
								bAHStateOnClose = true;
								this.ExitAutoHideMode(true);
                                if (this.dockInfoTransient.dController != null)
								    this.dockInfoTransient.dController.ControllerChanged += new ControllerChangedEH(this.TransientControllerChanged);
							}
							else
							{
								bAHStateOnClose = false;
								this.ParentController.ControllerChanged += new ControllerChangedEH(this.TransientControllerChanged);
								this.dockInfoTransient = new DockInfo(this.ParentController, this.dockInfoCurrent.dStyle, this.dockInfoCurrent.nPriority, 
									this.ParentController.GetChildHostIndex(this), this.dockInfoCurrent.DP, this.dockInfoTransient.rcDockArea);
							}
                            if (this.ParentController != null && this.ParentController is DockTabController)
                            {
                                (this.ParentController as DockTabController).RemoveDockHostFromTab(this, false);
                            }
							this.dockingMgr.UndockFromController(this);
						}
						if( !this.bInMDIMode )
							this.ctrlHost.Controls[0].Visible = false;
				
						this.ParentController = null;
						this.ctrlHost.Visible = false;			
					}
				}

				if(this.dockingMgr.DHCInFocus == this)
					this.dockingMgr.DHCInFocus = null;

				// Fire the DockVisibilityChanged event
				DockVisibilityChangedEventArgs args = bClosingByMouse ?
					new DockVisibilityChangedEventArgs(ctrlHost.Controls[0], DockingAction.ByMouse) : new DockVisibilityChangedEventArgs(ctrlHost.Controls[0]);  
				if( !DockingManager.bFiredVisibilityChangedEvent )
				{
					this.dockingMgr.FireDockVisibilityChangedEvent(args);
				}
				//Fix for #1580: Application lose focus when floating window close
                this.dockingMgr.HostControl.Select();
			}
		}
		public Rectangle GetTabDockTargetRectangle()
		{ 
			Rectangle target = Rectangle.Empty;
			Rectangle rcdrop = new Rectangle(0, 0, this.ctrlHost.Bounds.Width, this.ctrlHost.Bounds.Height);
			if( this.ctrlDockTab != null )
			{ 
				target = this.ctrlHost.Controls[0].RectangleToScreen(rcdrop);
				bool singleFloat = this.ParentController.ParentController is FloatingFormController;
				int tabBorderWidth = 4;
				switch( this.DockingManager.DockTabAlignment )
				{ 
					case DockTabAlignmentStyle.Bottom:
						if(	!singleFloat )
							target.Height -= this.ctrlDockTab.Height;						
						break;

					case DockTabAlignmentStyle.Top:
						target.Y -= this.ctrlDockTab.Height + tabBorderWidth;
						if( !singleFloat )
							target.Height -= this.ctrlDockTab.Height;
						break;

					case DockTabAlignmentStyle.Left:
						target.X -= this.ctrlDockTab.Width + tabBorderWidth;
						if( !singleFloat )
							target.Height -= this.ctrlDockTab.Width;						
						break;

					case DockTabAlignmentStyle.Right:
						if( !singleFloat )
							target.Height -= this.ctrlDockTab.Width;
						break;
				}
			}
			return target;
		}

		public override void GetDockInfo(Control ctrl, Point ptscreen, DockInfo di)
		{
			Rectangle rcdrop = new Rectangle(0,0,this.ctrlHost.Bounds.Width, this.ctrlHost.Bounds.Height);

			if(this.ctrlDockTab != null)
				di.dController = this.ctrlDockTab.InternalController;
			else
				di.dController = this;

			di.nPriority = 0;
			Point ptclient = this.ctrlHost.PointToClient(ptscreen);
		
			// If the caption area is hit, then provide tab docking feedback
			Rectangle rccaption = Rectangle.Empty;
			if(this.bHideCaption == true)	// Form provides the caption
			{
				int captionht = CaptionPainter.CaptionHeight+4;
				rccaption = new Rectangle(0, 0-captionht, this.ctrlHost.Width, captionht);
			}
			else
				rccaption = new Rectangle(0, 0, this.ctrlHost.Width, CaptionPainter.CaptionHeight+4);

			if(rccaption.Contains(ptclient))
			{
				di.dStyle = Syncfusion.Windows.Forms.Tools.DockingStyle.Tabbed;
				di.DP = DockPreference.Tabbed;
				di.nDockIndex = 0;

				Rectangle rect = this.ctrlHost.Controls[0].RectangleToScreen(rcdrop);
				rect.Size = this.ctrlHost.Controls[0].Size;
				di.rcDockArea = rect;
				di.rcControlArea = di.rcDockArea;
				if( this.ctrlDockTab == null )
					return;
			}

			// If the cursor is over the tab, then use the tabcontroller to get the dockinfo
			if( this.ctrlDockTab != null && (rccaption.Contains(ptclient) ||
				this.ctrlDockTab.RectangleToScreen(this.ctrlDockTab.ClientRectangle).Contains(ptscreen) ) )
			{
				if( ( this.ctrlDockTab.RectangleToScreen(this.ctrlDockTab.ClientRectangle).Contains(ptscreen) ) )
				{
					this.ctrlDockTab.InternalController.GetDockInfo(ctrl, ptscreen, di);					
					di.rcControlArea = di.rcDockArea;					
				}
				di.rcDockArea = GetTabDockTargetRectangle();				
				return;
			}

			di.rcControlArea = ctrlHost.RectangleToScreen(rcdrop);

			int nwidth = (di.rcDockArea.Width < this.ctrlHost.Width/2) ? di.rcDockArea.Width : this.ctrlHost.Width/2;
			int nheight = (di.rcDockArea.Height < this.ctrlHost.Height/2) ? di.rcDockArea.Height : this.ctrlHost.Height/2;
			
			// If this DockHost has minimum bounds set then adjust the dock info rect to preserve the min bounds
			Size minsize = this.minSize;
			if(this.ctrlDockTab != null)	// If in a tabbed group get the group's min extents
				minsize = this.ctrlDockTab.InternalController.MinimumSize;
			if((minsize.Width != 0) && ((this.ctrlHost.Width-nwidth) < minsize.Width))
				nwidth = this.ctrlHost.Width-minsize.Width;
			if((minsize.Height != 0) && ((this.ctrlHost.Height-nheight) < minsize.Height))
				nheight = this.ctrlHost.Height-minsize.Height;
			
			Rectangle[] rcborders = new Rectangle[4];
			Rectangle rchitrect = Rectangle.Inflate(this.ctrlHost.ClientRectangle, 1, 0);	// Pad for border thickness.
			if(this.ctrlDockTab != null)
				rchitrect.Height -= this.ctrlDockTab.Height;
			this.ComputeLRTBBorders(rchitrect, this.DockBoundary, ref rcborders);		

			if((rcborders[0].Contains(ptclient)==true) && (nwidth > DockHostController.nMinControlWidth))
			{
				di.dStyle = Syncfusion.Windows.Forms.Tools.DockingStyle.Left;
				di.DP = DockPreference.Horizontal;				
				di.rcDockArea = this.ctrlHost.RectangleToScreen(new Rectangle(rcdrop.X, rcdrop.Y, nwidth, rcdrop.Height));
			}
			else if((rcborders[1].Contains(ptclient)==true) && (nheight > DockHostController.nMinControlHeight))
			{
				di.dStyle = Syncfusion.Windows.Forms.Tools.DockingStyle.Top;
				di.DP = DockPreference.Vertical;				
				di.rcDockArea = this.ctrlHost.RectangleToScreen(new Rectangle(rcdrop.X, rcdrop.Y, rcdrop.Width, nheight));
			}
			else if((rcborders[2].Contains(ptclient)==true) && (nwidth > DockHostController.nMinControlWidth))
			{
				di.dStyle = Syncfusion.Windows.Forms.Tools.DockingStyle.Right;				
				di.DP = DockPreference.Horizontal;				
				di.rcDockArea = this.ctrlHost.RectangleToScreen(new Rectangle(rcdrop.Right-nwidth, rcdrop.Y, nwidth, rcdrop.Height));
			}
			else if((rcborders[3].Contains(ptclient)==true) && (nheight > DockHostController.nMinControlHeight))
			{
				di.dStyle = Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom;
				di.DP = DockPreference.Vertical;				
				di.rcDockArea = this.ctrlHost.RectangleToScreen(new Rectangle(rcdrop.X, rcdrop.Bottom-nheight, rcdrop.Width, nheight));
			}
			else
				di.dController = null;						
			di.nDockIndex = this.dockInfoCurrent.nDockIndex;			
		}

		public override bool QueryDropProceedWithDock(Control ctrldrop, Syncfusion.Windows.Forms.Tools.DockingStyle style)
		{
			if(this.bFloatOnly == true)
				return false;
			DockAllowEventArgs arg = new DockAllowEventArgs(ctrldrop.Controls[0], this.ctrlHost.Controls[0], style); 
			this.dockingMgr.FireDockAllowEvent(arg);
			return !arg.Cancel;			
		}		

		// Controller changed eventhandler for Dock <-> Float transitions.
		public virtual void dhc_ControllerChanged(Object sender, ControllerChangedEventArgs e)
		{
			if( sender.Equals(this.dockInfoPrevious.dController) == false )
			{
				Debug.Assert(false, "Error: Event subscriptions out of sync.\n");
				return;
			}
			
			// Unsubscribe from the previous controller and subscribe to this new controller's event
			if(this.dockInfoPrevious.dController != null)
				this.dockInfoPrevious.dController.ControllerChanged -= new ControllerChangedEH(this.dhc_ControllerChanged);
			// Update the dockinfo within the DIPrevious info cache. This new controller will 
			// be used for redocking/refloating ops			
			DockInfo dinew = e.NewDockInfo;
			if(dinew.dController != null)
			{
				// Retain the dockindex, and update the remaining attributes
				this.dockInfoPrevious = new DockInfo(dinew.dController, dinew.dStyle, dinew.nPriority, this.dockInfoPrevious.nDockIndex, 
					dinew.DP, dinew.rcDockArea);
				dinew.dController.ControllerChanged += new ControllerChangedEH(this.dhc_ControllerChanged);
			}
			else 
			{
				// The floating dock controller has been docked. When a refloat occurs, float to a new frame
				this.dockInfoPrevious.dController = null;			
				if((dinew.rcDockArea.Width<=0 || dinew.rcDockArea.Height<=0) == false)
					this.dockInfoPrevious.rcDockArea = dinew.rcDockArea;
			}

			// If the dcr is non-null, then add this to this controller's current dcrlist
			if(e.DCRelation != null)
			{
				ArrayList al = (this.Floating == true) ? this.alDockDCR : this.alFloatDCR;
				this.InsertIntoDCR(al, al.Count, e.DCRelation);
			}
		}		

		public override void InvokePrevDockFloatTransition( bool showFloating )
		{
			if(this.Floating == false && !this.dockingMgr.DisallowFloating )
				this.TransitToPrevFloat( showFloating );
			else 
				this.TransitToPrevDock();
		}

        public void InvokePrevDockFloatTransition()
        {
            this.InvokePrevDockFloatTransition(true);
        }	

		public void TransitToPrevFloat( bool showFloating )
		{
            
            foreach (Control ctrl in this.DockingManager.alEnableDocking)
            {
                if (this.ctrlHost.Controls.Count > 0)
                {
                    if (this.ctrlHost.Controls[0].Equals(ctrl))
                    {
                        if (this.SharedForm != null && this.DockingManager != null && this.DockingManager.CustomizeControlCollection.Contains(ctrl))
                        {
                            this.SharedForm.Location = this.DockingManager.FloatingLocation;
                            this.DockingManager.CustomizeControlCollection.Remove(ctrl);
                        }
                    }
                }
            }

			bool showFloat = showFloating && !this.Closing;
			this.RestoreController();
			this.dockInfoNew = new DockInfo(this.dockInfoPrevious);

			Control[] ctrls = new Control[] {this.ctrlHost.Controls[0]};
			this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));

			// Unsubscribe from the previous parent controller 
			if(this.dockInfoPrevious.dController != null)
				this.dockInfoPrevious.dController.ControllerChanged -= new ControllerChangedEH(this.dhc_ControllerChanged);
			Point ptlocation = Point.Empty;
			if( this.ctrlHost.Parent != null )
				ptlocation = this.ctrlHost.Parent.PointToScreen(this.ctrlHost.Location);

			ValidateSharedForm();

			DockHostController target = null;
			DockRelation dRel = null;
			if( InternalFloatWrapper != null )
				dRel = InternalFloatWrapper.Relations[this.UniqueName] as DockRelation;

			if( dRel != null )
			{
				int index = InternalFloatWrapper.DockRelationControllers.IndexOf( this );
				InternalFloatWrapper.DockRelationControllers.Remove(this);
				index = Math.Min( index, InternalFloatWrapper.DockRelationControllers.Count );
				target = FindTabbedDockTarget(true);
				InternalFloatWrapper.DockRelationControllers.Insert(index, this);
			}

			if( ((dRel == null || target == null)
				 || this.bFloatOnly ) && this.ParentController != null )
			{
				if( !( ParentController is DockTabController ) )
				{
					// Subscribe to the current parent controller
					this.ParentController.ControllerChanged += new ControllerChangedEH(this.dhc_ControllerChanged);
					this.dockInfoPrevious = new DockInfo(this.dockInfoCurrent);
					this.dockingMgr.UndockFromController(this);
				}
				else	// Docked in a tabcontroller
				{
					DockTabController tabctrl = this.ParentController as DockTabController;
					Debug.Assert(( tabctrl != null ), "Error: Invalid Parent Controller.\n");
					tabctrl.ParentController.ControllerChanged += new ControllerChangedEH(this.dhc_ControllerChanged);
					this.dockInfoPrevious = new DockInfo(tabctrl.ParentController, tabctrl.DICurrent.dStyle, tabctrl.DICurrent.nPriority,
						tabctrl.ParentController.GetChildHostIndex(tabctrl), tabctrl.DICurrent.DP, tabctrl.DICurrent.rcDockArea);
				}
			}
		
			this.bFloating = true;
			if(this.bFloatOnly == false)
			{
				if( !dockingMgr.ForbidWrapperLogic )
				{
					if( dRel != null && target != null )
					{
						this.dockInfoNew = dRel.Relation;
						this.dockInfoNew.DP = DockPreference.Tabbed;
						this.dockInfoNew.dController = target;
						SetPrevTabIndex( target, this.InternalFloatWrapper.DockRelationControllers );
						this.ApplyDockInfo();
						this.InternalFloatWrapper = null;
					}
					else
					{
                        if (this.SharedForm != null && this.SharedForm.IsDisposed)
                        {   
                            this.SharedForm.InternalController = null;
                            this.SharedForm = null;
                            CreateFloatingFrame(ptlocation, showFloat);                            
                        }
						else if( this.SharedForm != null )
						{
							if( this.SharedForm.Used )
								InternalTransitToPrevState(true);
							else
							{
								if( InternalFloatWrapper != null
									&& InternalFloatWrapper.ParentController is FloatingFormController
									&& m_form == this.SharedForm )
								{
									m_form.Enable(showFloat);
									InternalTransitToPrevState(true);
									this.SharedForm.Used = true;
								}
								else
								{
									CreateFloatingFrame(Point.Empty, showFloat);
								}
							}
						}
						else
						{
							CreateFloatingFrame(ptlocation, showFloat);
						}
					}
				}
				else
				{
					// If the new dockinfo has a valid controller, then dock to it. Else query 
					// the docking manager for a suitable floating frame.
					if( this.dockInfoNew.dController != null )
					{
						if( this.dockInfoNew.dController.AttemptDCRDocking(this, this.alFloatDCR.GetEnumerator()) == false )
						{
							if( this.dockInfoNew.dController.Floating == true )
								CreateFloatingFrame(ptlocation, showFloat);
							else
								this.dockingMgr.DockToNewSizingController(this);
						}
					}
					// Query the dockingmanager for the availability of a floating frame hosting a dc 
					// that has a relationship with this dockhost controller
					else if( this.dockingMgr.AttemptFloatingFormDCRDocking(this, this.alFloatDCR.GetEnumerator()) == false )
					{
						// No suitable floating targets are available. Create and dock to a new frame
						CreateFloatingFrame(ptlocation, showFloat);
					}
				}
			}
			else
			{
				this.CreateFloatingFrame(ptlocation, true);
			}
			this.HostControl.Focus();
			this.dockingMgr.FireDockStateChangeEvent("DockStateChanged", new DockStateChangeEventArgs(ctrls));
		}

		protected void ValidateSharedForm()
		{
			if( this.SharedForm != null )
			{
				FloatingFormController ffc = this.SharedForm.InternalController as FloatingFormController;

				if( ffc != null )
				{
					DockHostController dhc = ffc.GetChildAt(0) as DockHostController;

					if( dhc == null )
					{
						SizingController sc = ffc.GetChildAt(0) as SizingController;

						if( sc != null )
						{
							ArrayList controllers = sc.GetDockControllers();
							if( controllers.Count == 1 )
								dhc = controllers[0] as DockHostController;
						}
					}

					if( dhc != null && dhc.DockVisibility && dhc.FloatOnly )
						this.SharedForm = null;
				}
			}
		}

		protected DockHostController FindTabbedDockTarget( bool floating )
		{
			DockHostController target = null;
			DockStateControllerWrapper prevWrapper = floating 
				? InternalFloatWrapper : InternalDockWrapper;

			foreach( DockHostController hostCtrl in prevWrapper.DockRelationControllers )
			{
				DockControllerBase parentCtrl = hostCtrl.ParentController;
				SizingController parentSizing = parentCtrl as SizingController;
				bool tabParent = false;

				if( parentCtrl != null )
				{
					if( parentSizing == null )
					{
						parentSizing = parentCtrl.ParentController as SizingController;

						if( parentCtrl is DockTabController )
							tabParent = true;
					}

					bool suitable = ( !hostCtrl.FloatOnly && !hostCtrl.bInAutoHide
						&& parentSizing == prevWrapper.ParentController );

                    if( suitable && ( tabParent ? hostCtrl.DockTab != null : true ) && parentCtrl.Floating == floating )
					{
						target = hostCtrl;
						break;
					}
				}
			}

			if( target == null )
			{
				bool setToPrevPosition = true;

				if( prevWrapper.ParentController is SizingController && prevWrapper.Valid )
					setToPrevPosition = false;

				if( setToPrevPosition )
				{
					foreach( DockHostController hostCtrl in prevWrapper.DockRelationControllers )
					{
						if(( hostCtrl.DockVisibility && !hostCtrl.FloatOnly
							&& hostCtrl.Floating == floating && !hostCtrl.bInAutoHide ) && hostCtrl.ParentController != null )
						{
							if( hostCtrl.ParentController is DockTabController )
								target = ( hostCtrl.ParentController as DockTabController ).HostController;
							else
								target = hostCtrl;

							break;
						}
					}
				}
			}

			return target;
		}

		public void TransitToPrevDock()
		{
			bool freeze = this.dockingMgr.ForbidFreeze;
			this.dockingMgr.ForbidFreeze = true;
			this.MustResize = Direction.Both;
			if(this.bFloatOnly == true)
				return;

			this.dockInfoNew = new DockInfo(this.dockInfoPrevious);	
			if(this.dockInfoNew.dController == null)	// Will happen if the dockhost is initially floated.
				this.dockInfoNew = new DockInfo(this.dockingMgr.dcHostForm, Syncfusion.Windows.Forms.Tools.DockingStyle.Left, -1, 0, DockPreference.Horizontal, this.ctrlHost.Bounds);

			if( dockingMgr.DockToFill && (dockInfoNew.dController is SizingController) )
			{
				this.dockInfoNew.dController = dockInfoNew.dController.ParentController;
			}
			Control[] ctrls = new Control[] {this.ctrlHost.Controls[0]};
			this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));
			
			// Unsubscribe from the previous parent controller 
			if(this.dockInfoPrevious.dController != null)
				this.dockInfoPrevious.dController.ControllerChanged -= new ControllerChangedEH(this.dhc_ControllerChanged);

			DockHostController target = null;
			DockRelation dRel = null;

			if( InternalDockWrapper != null )
				dRel = InternalDockWrapper.Relations[this.UniqueName] as DockRelation;

			if( dRel != null )
			{
				int index = InternalDockWrapper.DockRelationControllers.IndexOf( this );
				InternalDockWrapper.DockRelationControllers.Remove(this);
				target = FindTabbedDockTarget(false);
				InternalDockWrapper.DockRelationControllers.Insert(Math.Max(0,index), this);
			}
			bool floating = this.Floating;

			if( this.ParentController != null )
			{
				if( !(this.ParentController is DockTabController) )
				{
					// Subscribe to the current parent controller
					this.ParentController.ControllerChanged += new ControllerChangedEH(this.dhc_ControllerChanged);
					this.dockInfoPrevious = new DockInfo(this.ParentController, this.dockInfoCurrent.dStyle, this.dockInfoCurrent.nPriority, this.ParentController.GetChildHostIndex(this),
						this.dockInfoCurrent.DP, this.dockInfoCurrent.rcDockArea);
					if( this.Floating )
						ExitFloatingFrame();
				}
				else	// Docked in a tabcontroller
				{
					DockTabController tabctrl = this.ParentController as DockTabController;
					Debug.Assert(( tabctrl != null ), "Error: Invalid Parent Controller.\n");

					if( tabctrl.ParentController != null )
					{
						tabctrl.ParentController.ControllerChanged += new ControllerChangedEH(this.dhc_ControllerChanged);

						this.dockInfoPrevious = new DockInfo(tabctrl.ParentController, tabctrl.DICurrent.dStyle, tabctrl.DICurrent.nPriority,
							tabctrl.ParentController.GetChildHostIndex(tabctrl), tabctrl.DICurrent.DP, tabctrl.DICurrent.rcDockArea);
					}
				}
			}

			if( m_prevAHStyle != DockingStyle.None && floating )
			{
				TransitToPrevAHState();

				return;
			}
			
			this.HideCaption = false;
			this.bFloating = false;

			if( dRel != null && target != null )
			{
				DCRelationship dcr = new DCRelationship(0, false
					, DockPreference.Tabbed, dRel.Relation.nDockIndex);
				SetPrevTabIndex(target, this.InternalDockWrapper.DockRelationControllers);
				target.InvokeTabbedDocking(this, dcr);
				this.InternalDockWrapper = null;
			}
			else
			{
				if( this.InternalDockWrapper != null 
					&& this.InternalDockWrapper.Valid )
				{
					InternalTransitToPrevState(false);
				}
				else
				{
					if( dockInfoNew.dController is SizingController
						&& dockInfoNew.dController.ParentController == null )
					{
						this.dockingMgr.DockToBlankSizingController(this);
					}
					else
					{
						if( this.dockInfoNew.dController != null )
						{
							if( this.dockInfoNew.dController.AttemptDCRDocking(this, this.alDockDCR.GetEnumerator()) == false )
								this.dockingMgr.DockToSizingController(this);
						}
						else
						{
							// Execution gets here only when the docktarget in dinew is a parentcontroller that does not have any 
							// children with a relationship with the new dockhost. In that case, tack on to the same sizing controller, if 
							// in non-tab mode or else dock to new sizing controller.
							this.dockingMgr.DockToBlankSizingController(this);
						}
					}
				}
			}
			
			if( DockingManager.DockToFill )
				ClearParentTransientSize();
			if( !this.AutoHiddedBeforeHide )
				this.HostControl.Focus();
			this.dockingMgr.FireDockStateChangeEvent("DockStateChanged", new DockStateChangeEventArgs(ctrls));
			this.dockingMgr.ForbidFreeze = freeze;
		}

		protected void SetPrevTabIndex( DockControllerBase target, ArrayList controllers )
		{
			if( controllers != null && target != null )
			{
				DockTabControl tabCtrl = null;
				DockTabController tabController = target.ParentController as DockTabController;
				if( tabController != null )
					tabCtrl = tabController.TabControl;
				DockHostController sibling = null;
				int index = 0;
				bool middle = false;
				if( tabCtrl != null )
				{
					foreach( DockHostController dhc in controllers )
					{
						int counter = 0;
						foreach( DockTabPage dtp in tabCtrl.TabPages )
						{
							if( dhc == dtp.dhcClient )
							{
								index = counter;
								sibling = dhc;
								break;
							}
							counter++;
						}
						if( middle && sibling != null )
							break;
						if( dhc == this )
						{
							if( sibling != null )
								break;
							else
								middle = true;
						}
					}
					if( !middle )
						index++;
				}
				else
				{
					if( controllers.IndexOf( target ) < controllers.IndexOf( this ) )
						index = 1;
				}

				this.DINew.nDockIndex = index;
			}
		}

		protected void ClearParentTransientSize()
		{
			SizingController topParent = this.ParentController as SizingController;

			while( topParent != null )
			{
				SizingController tempSc = topParent.ParentController as SizingController;

				if( tempSc != null )
					topParent = tempSc;
				else
					break;
			}

			if( topParent != null )
				CrealTransientSize(topParent);
		}

		private void CrealTransientSize( DockControllerBase dcb )
		{
			if( dcb.ChildControllers != null )
			{
				foreach( DockControllerBase baseCtrl in dcb.ChildControllers )
				{
					CrealTransientSize(baseCtrl);
				}
			}

			if( dcb.LayoutRect.Height != 0
				&& dcb.LayoutRect.Width != 0 )
				dcb.DITransient.rcDockArea = Rectangle.Empty;
		}

		internal virtual void TransitToPrevAHState()
		{
			MainFormController mfc = this.dockingMgr.dcHostForm;
			AHTabControl ahTab = mfc.GetAHTabControl(m_prevAHStyle);

			DockPreference dp = ( ( m_prevAHStyle == DockingStyle.Left ) || ( m_prevAHStyle == DockingStyle.Right ) )
				? DockPreference.Horizontal : DockPreference.Vertical;

			if( this.DockDCRList.Count != 0 )
				this.DCRCurrent = this.DockDCRList[0] as DCRelationship;

			if( !AttemptPrevAHDocking(ahTab, this, false) )
			{
				this.bInAutoHide = true;
				SizingController sc = new SizingController(this.dockingMgr, this.dockingMgr.HostControl, dp);
				sc.ParentController = mfc;
				sc.DICurrent = new DockInfo(this, m_prevAHStyle, -1, -1, dp, Rectangle.Empty);
				sc.AddChild(this, m_prevAHStyle);

				// Subscribe to the sizingcontroller's splitter's paint event
				DockControllerBase splitterdc = sc.GetChildAt(1);
				splitterdc.HostControl.Paint += new PaintEventHandler(mfc.OnAHSplitterPaint);
				ahTab.AddTab(this, false);
			}

			if( this.ParentController is DockTabController )
				this.ParentController.DITransient.dController = this.InternalDockWrapper.ParentController;
			else
				this.DITransient.dController = this.InternalDockWrapper.ParentController;

			this.DINew.dStyle = ahTab.Edge;
			this.HostControl.Bounds = Rectangle.Empty;
			mfc.HostControl.Controls.Add(this.HostControl);

			m_prevAHStyle = DockingStyle.None;
			mfc.AdjustLayout();
		}

		internal void InternalTransitToPrevState()
		{
			bool floating;
			if( this.m_prevWrapper.ParentController != null )
				floating = this.m_prevWrapper.Floating;
			else
				floating = true;
			floating = floating && !this.dockingMgr.DisallowFloating;

			InternalTransitToPrevState(floating);
		}

		internal void InternalTransitToPrevState( bool floating )
		{			
			SizingController sc = null;
			this.dockingMgr.ForbidFreeze = true;

			if( floating )
				sc = this.InternalFloatWrapper.ParentController as SizingController;
			else
				sc = this.InternalDockWrapper.ParentController as SizingController;

			Rectangle rect = this.LayoutRect;
            
            bool hiddenOnLoad = false;
            if (this.dockingMgr != null && this.HostControl != null && this.HostControl.Controls[0] != null
                && this.dockingMgr.GetHiddenOnLoad(this.HostControl.Controls[0]) && this.dockingMgr.m_bLoadingDockState)
            {
                hiddenOnLoad = true;
            }
            if(!hiddenOnLoad)
			    this.DIPrevious.rcDockArea = this.DICurrent.rcDockArea;
            bool setRelatedSize = true;

			if( floating )
			{
				InternalFloatWrapper.ParentController.ReplaceChild(this.InternalFloatWrapper, this);
				if( InternalFloatWrapper.ControlSize != Size.Empty )
					rect.Size = InternalFloatWrapper.ControlSize;
				this.Minimized = InternalFloatWrapper.Minimized;
				( this.ToplevelController as FloatingFormController ).RefreshFormCaption();
			}
			else
			{
				InternalDockWrapper.ParentController.ReplaceChild(this.InternalDockWrapper, this);

                SizingController parentSizing = InternalDockWrapper.ParentController as SizingController;
                if (InternalDockWrapper.ControlSize != Size.Empty)
                {
                    if (parentSizing != null && DockingManager.DockToFill && parentSizing.LayoutRect.Size != Size.Empty)
                    {
                        if (parentSizing.DockingOrder == DockPreference.Vertical && parentSizing.LayoutRect.Height != 0)
                        {
                            if (parentSizing.LayoutRect.Height <= InternalDockWrapper.ControlSize.Height)
                                setRelatedSize = false;
                        }
                    }
                    else if ((parentSizing.LayoutRect.Width == 0 || parentSizing.LayoutRect.Height == 0) && 
                        (parentSizing.dockInfoCurrent.dStyle == DockingStyle.Top || parentSizing.dockInfoCurrent.dStyle == DockingStyle.Bottom)
                        && parentSizing.dockInfoCurrent.dStyle != this.dockInfoCurrent.dStyle &&
                        (InternalDockWrapper.ControlSize.Width != 0 && InternalDockWrapper.ControlSize.Height != 0))
                    {
                        Rectangle sizingRect = new Rectangle(parentSizing.LayoutRect.Location, InternalDockWrapper.ControlSize);
                        parentSizing.LayoutRect = sizingRect;
                        //setRelatedSize = false;
                        int splitterDistance = 4;
                        Rectangle adjustedRect = Rectangle.Empty;
                        if (parentSizing.dockInfoCurrent.dStyle == DockingStyle.Top || parentSizing.dockInfoCurrent.dStyle == DockingStyle.Bottom)
                        {
                            adjustedRect = new Rectangle(this.DITransient.rcDockArea.Location, new Size
                            (this.DITransient.rcDockArea.Size.Width, this.DITransient.rcDockArea.Size.Height + splitterDistance));
                        }
                        else if (parentSizing.dockInfoCurrent.dStyle == DockingStyle.Right)
                        {
                            adjustedRect = new Rectangle(this.DITransient.rcDockArea.Location, new Size
                            (this.DITransient.rcDockArea.Size.Width + splitterDistance, this.DITransient.rcDockArea.Size.Height));
                        }
                        if (adjustedRect != Rectangle.Empty)
                            this.ParentController.LayoutRect = adjustedRect;
                    }
                        rect.Size = InternalDockWrapper.ControlSize;
                }
                
                this.Minimized = InternalDockWrapper.Minimized;

				if( InternalDockWrapper.DockRelationControllers.Count > 1 )
					this.TempWrapper = InternalDockWrapper;
			}

			this.LayoutRect = rect;
            if(setRelatedSize)
			    SetRelatedSize( floating );

            if (InternalDockWrapper != null)
            {
                SizingController parentSizingController = InternalDockWrapper.ParentController as SizingController;

                if (parentSizingController != null && parentSizingController.dockInfoCurrent.dStyle != DockingStyle.Left)
                    this.LayoutRect = this.DITransient.rcDockArea;
            }
			this.ToplevelController.HostControl.Controls.Add(this.HostControl);
			this.ToplevelController.AdjustLayout();
			this.AdjustLayout();
			this.dockingMgr.ForbidFreeze = false;
		}
		/// <summary>
		/// Sets controllers size according to siblings.
		/// </summary>
		/// <param name="floating">if location is floating.</param>
		internal void SetRelatedSize( bool floating )
		{
			DockControllerBase ctrl = this;
			ArrayList relations = null;
			int corretion = this.dockingMgr.SplitterWidth / 2;

			if( floating )
			{
				if( this.PreviousFloatSize.Width != 0
					&& this.PreviousFloatSize.Height != 0 )
				    this.DITransient.rcDockArea.Size = this.PreviousFloatSize;
				relations = this.StoredFloatSizes;
			}
			else
			{
				if( this.PreviousDockSize.Width != 0 
					&& this.PreviousDockSize.Height != 0 )
				    this.DITransient.rcDockArea.Size = this.PreviousDockSize;
				relations = this.StoredDockSizes;
			}

			this.DITransient.rcDockArea.Inflate(new Size(corretion, corretion));
			Size prevSize = this.DITransient.rcDockArea.Size;
			DockControllerBase prevChild = this;
			SizingController scParent = ctrl.ParentController as SizingController;
			int parentLevel = 1;
			while( scParent != null )
			{
				if( relations.Count < parentLevel++ )
					relations.Add(new RelationNamePair());
				scParent = scParent.ParentController as SizingController;
			}
			scParent = ctrl.ParentController as SizingController;

			foreach( RelationNamePair relationPair in relations )
			{
				float relation = relationPair.Relation;
				if( scParent == null )
					break;
				else
				{
					if( scParent.ChildCount == 3 && relation != 0 )
					{
						DockControllerBase sibling = null;
						int index = scParent.GetChildHostIndex(prevChild);

						if( index == 0 )
							sibling = scParent.GetChildAt(2);
						else
							sibling = scParent.GetChildAt(0);
						if( sibling.DITransient.rcDockArea == Rectangle.Empty )
							sibling.DITransient.rcDockArea = sibling.LayoutRect;

						if( !( sibling is DockStateControllerWrapper ) )
						{
							if( scParent.DockingOrder == DockPreference.Vertical )
							{
								if( sibling.DITransient.rcDockArea.Height != 0 )
								{
									prevChild.DITransient.rcDockArea.Height
										= ( int )( sibling.DITransient.rcDockArea.Height / relation );
								}
								else
									prevChild.DITransient.rcDockArea.Height = prevSize.Height;
							}
							else
							{
								if( sibling.DITransient.rcDockArea.Width != 0 )
								{
									prevChild.DITransient.rcDockArea.Width
										= ( int )( sibling.DITransient.rcDockArea.Width / relation );
								}
								else
									prevChild.DITransient.rcDockArea.Width = prevSize.Width;
							}
						}
						else
							if( prevSize != Size.Empty && ( prevChild.DITransient.rcDockArea.Height == 0
								|| prevChild.DITransient.rcDockArea.Width == 0 ) )
								prevChild.DITransient.rcDockArea =
									new Rectangle(prevChild.DITransient.rcDockArea.Location, prevSize);
					}
					else
					{
						if( prevSize != Size.Empty && ( prevChild.DITransient.rcDockArea.Height == 0
							|| prevChild.DITransient.rcDockArea.Width == 0 ) )
							prevChild.DITransient.rcDockArea =
								new Rectangle(prevChild.DITransient.rcDockArea.Location, prevSize);
					}

					prevSize = prevChild.DITransient.rcDockArea.Size;
				}

				prevChild = scParent;
				scParent = scParent.ParentController as SizingController;
			}
			prevChild.DITransient.rcDockArea = this.DITransient.rcDockArea;
			int splitWidth = this.dockingMgr.SplitterWidth;
			if( prevChild.LayoutRect.Height - splitWidth <= 0 || prevChild.LayoutRect.Width - splitWidth <= 0 )
				prevChild.LayoutRect = prevChild.DITransient.rcDockArea;
			relations.Clear();
		}

		public void TransitDockToFloat()
		{
			if(this.Floating == true)
				return;

			Control[] ctrls = null;
			if(this.ctrlDockTab == null)
			{
				ctrls = new Control[] {this.ctrlHost.Controls[0]};
				this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));

				this.alFloatDCR.Clear();
				// Before undocking, subscribe to the current parent controller's ControllerChanged event
				if(this.dockInfoPrevious.dController != null)
					this.dockInfoPrevious.dController.ControllerChanged -= new ControllerChangedEH(this.dhc_ControllerChanged);
				this.ParentController.ControllerChanged += new ControllerChangedEH(this.dhc_ControllerChanged);

				this.dockInfoPrevious = new DockInfo(this.ParentController, this.dockInfoCurrent.dStyle, this.dockInfoCurrent.nPriority, this.ParentController.GetChildHostIndex(this), 
					this.dockInfoCurrent.DP, this.dockInfoCurrent.rcDockArea);

				Point ptlocation = this.ctrlHost.Parent.PointToScreen(this.ctrlHost.Location);						
				this.dockingMgr.UndockFromController(this);
				this.HideCaption = true;
				this.bFloating = true;
				FloatingForm form = CreateFloatingFrame(ptlocation, true);
				this.dockingMgr.UpdateFloatingFormsImages();
			}
			else
			{
				DockTabController tabctrl = this.ctrlDockTab.InternalController as DockTabController;
				Debug.Assert(tabctrl != null);

				ctrls = new Control[tabctrl.TabControl.TabCount];
				int i = 0;
				foreach(DockTabPage page in tabctrl.TabControl.TabPages)
					ctrls[i++] = page.dhcClient.HostControl.Controls[0];					
				this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));

				tabctrl.DINew = new DockInfo(this.dockInfoNew);
				tabctrl.TransitDockToFloat();
			}
			this.dockingMgr.FireDockStateChangeEvent("DockStateChanged", new DockStateChangeEventArgs(ctrls));
		}

		public void TransitFloatToDock()
		{
			if(this.bFloatOnly == true)
			{
				Debug.Assert(false, "Non-dockable control.");
				return;
			}

			if(this.Floating == false)
				return;

			Control[] ctrls = null;
			if(this.ctrlDockTab == null)
			{
				ctrls = new Control[] {this.ctrlHost.Controls[0]};
				this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));

				// Clear the dockrelation list
				this.alDockDCR.Clear();
				// Unsubscribe from the previous parent controller and subscribe to the current parent controller
				if(this.dockInfoPrevious.dController != null)
					this.dockInfoPrevious.dController.ControllerChanged -= new ControllerChangedEH(this.dhc_ControllerChanged);
				if(this.ParentController != null)
				{
					this.ParentController.ControllerChanged += new ControllerChangedEH(this.dhc_ControllerChanged);
					this.dockInfoPrevious = new DockInfo(this.ParentController, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill, 0, this.ParentController.GetChildHostIndex(this), 
						this.dockInfoCurrent.DP, this.dockInfoCurrent.rcDockArea);
					this.ctrlHost.Parent.Location = new Point(this.dockInfoPrevious.rcDockArea.X, this.dockInfoPrevious.rcDockArea.Y);
					ExitFloatingFrame();	
				}
				else	// CloseController has been invoked on a floating window
				{
					this.dockInfoPrevious = new DockInfo(null, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill, 0, 0, this.dockInfoCurrent.DP, this.dockInfoCurrent.rcDockArea);
				}				
				this.bFloating = false;	
				this.HideCaption = false;
				this.dockInfoNew.dController.InvokeDocking(this);
				this.DockEdge = DINew.dStyle;
			}
			else
			{
				DockTabController tabctrl = this.ctrlDockTab.InternalController as DockTabController;
				Debug.Assert(tabctrl != null);

				ctrls = new Control[tabctrl.TabControl.TabCount];
				int i = 0;
				foreach(DockTabPage page in tabctrl.TabControl.TabPages)
					ctrls[i++] = page.dhcClient.HostControl.Controls[0];					
				this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));

				tabctrl.DINew = new DockInfo(this.dockInfoNew);
				tabctrl.TransitFloatToDock();
			}
			this.dockingMgr.FireDockStateChangeEvent("DockStateChanged", new DockStateChangeEventArgs(ctrls));
		}

		public void TransitDockInFloatToFloat()
		{
			if(this.Floating == false)
				return;
	
			Control[] ctrls = null;
			if(this.ctrlDockTab == null)
			{				
				ctrls = new Control[] {this.ctrlHost.Controls[0]};
				this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));

				this.alFloatDCR.Clear();				// Clear this controller's floatdcr list
				Point ptlocation = this.ctrlHost.Parent.PointToScreen(this.ctrlHost.Location);
				this.dockingMgr.UndockFromController(this);
				this.InternalFloatWrapper = null;
				this.HideCaption = true;
				CreateFloatingFrame(ptlocation, true);
				this.dockingMgr.UpdateFloatingFormsImages();
			}
			else
			{
				DockTabController tabctrl = this.ctrlDockTab.InternalController as DockTabController;
				Debug.Assert(tabctrl != null);

				ctrls = new Control[tabctrl.TabControl.TabCount];
				int i = 0;
				foreach(DockTabPage page in tabctrl.TabControl.TabPages)
					ctrls[i++] = page.dhcClient.HostControl.Controls[0];					
				this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));

				tabctrl.DINew = new DockInfo(this.dockInfoNew);
				tabctrl.TransitDockInFloatToFloat();
			}
			this.dockingMgr.FireDockStateChangeEvent("DockStateChanged", new DockStateChangeEventArgs(ctrls));
		}

		public void TransitFloatToDockInFloat()
		{
			if(this.Floating == false)
				return;

			Control[] ctrls = null;
			if(this.ctrlDockTab == null)
			{
				ctrls = new Control[] {this.ctrlHost.Controls[0]};
				this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));

				this.alFloatDCR.Clear();
				this.HideCaption = false;
				ExitFloatingFrame();
				this.dockInfoNew.dController.InvokeDocking(this);
				this.dockingMgr.UpdateFloatingFormsImages();
				this.SharedForm = null;
			}
			else
			{
				DockTabController tabctrl = this.ctrlDockTab.InternalController as DockTabController;
				Debug.Assert(tabctrl != null);

				ctrls = new Control[tabctrl.TabControl.TabCount];
				int i = 0;
				foreach(DockTabPage page in tabctrl.TabControl.TabPages)
					ctrls[i++] = page.dhcClient.HostControl.Controls[0];					
				this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));

				tabctrl.DINew = new DockInfo(this.dockInfoNew);
				tabctrl.TransitFloatToDockInFloat();
			}
			this.dockingMgr.FireDockStateChangeEvent("DockStateChanged", new DockStateChangeEventArgs(ctrls));
		}

		public void TransitDockToDockInFloat()
		{
			if(this.Floating == true)
				return;

			Control[] ctrls = null;
			if(this.ctrlDockTab == null)
			{
				ctrls = new Control[] {this.ctrlHost.Controls[0]};
				this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));

				// Before undocking, subscribe to the current parent controller's ControllerChanged event
				if(this.dockInfoPrevious.dController != null)
					this.dockInfoPrevious.dController.ControllerChanged -= new ControllerChangedEH(this.dhc_ControllerChanged);
				this.ParentController.ControllerChanged += new ControllerChangedEH(this.dhc_ControllerChanged);

				this.dockInfoPrevious = new DockInfo(this.ParentController, this.dockInfoCurrent.dStyle, this.dockInfoCurrent.nPriority, 
					this.ParentController.GetChildHostIndex(this), this.dockInfoCurrent.DP, this.dockInfoCurrent.rcDockArea);

				this.alFloatDCR.Clear();
				this.dockingMgr.UndockFromController(this);
				this.bFloating = true;
				this.dockInfoNew.dController.InvokeDocking(this);			
				this.dockingMgr.UpdateFloatingFormsImages();
			}
			else
			{
				DockTabController tabctrl = this.ctrlDockTab.InternalController as DockTabController;
				Debug.Assert(tabctrl != null);

				ctrls = new Control[tabctrl.TabControl.TabCount];
				int i = 0;
				foreach(DockTabPage page in tabctrl.TabControl.TabPages)
					ctrls[i++] = page.dhcClient.HostControl.Controls[0];					
				this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));

				tabctrl.DINew = new DockInfo(this.dockInfoNew);
				tabctrl.TransitDockToDockInFloat();
			}
			this.dockingMgr.FireDockStateChangeEvent("DockStateChanged", new DockStateChangeEventArgs(ctrls));
		}

		public void TransitDockToDock()
		{			
			Control[] ctrls = null;
			if(this.ctrlDockTab == null)
			{
				ctrls = new Control[] {this.ctrlHost.Controls[0]};
				this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));

				this.alDockDCR.Clear();
				this.dockingMgr.UndockFromController(this);
				this.InternalDockWrapper = null;
				this.dockInfoNew.dController.InvokeDocking(this);			
			}
			else
			{
				DockTabController tabctrl = this.ctrlDockTab.InternalController as DockTabController;
				Debug.Assert(tabctrl != null);

				ctrls = new Control[tabctrl.TabControl.TabCount];
				int i = 0;
				foreach(DockTabPage page in tabctrl.TabControl.TabPages)
					ctrls[i++] = page.dhcClient.HostControl.Controls[0];					
				this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));

				tabctrl.DINew = new DockInfo(this.dockInfoNew);
				tabctrl.TransitDockToDock();
			}
			this.dockingMgr.FireDockStateChangeEvent("DockStateChanged", new DockStateChangeEventArgs(ctrls));
		}
		
		public void MoveFloatToFloat()
		{
			// Move the floating frame to the new position
			// If the Cursor.Position.Y < 0, then could be multi-monitor setup so allow negative y coords.
			if((this.dockInfoNew.rcDockArea.Y < 0) && (Cursor.Position.Y >= 0))
				this.dockInfoNew.rcDockArea.Y = 0;
			this.ctrlHost.Parent.Location = new Point(this.dockInfoNew.rcDockArea.X, this.dockInfoNew.rcDockArea.Y);
			this.ctrlHost.DragRectangle = new Rectangle(this.ctrlHost.Parent.Location.X, this.ctrlHost.Parent.Location.Y, 
				this.ctrlHost.Bounds.Width, this.ctrlHost.Bounds.Height);				
			this.dockInfoCurrent.rcDockArea = this.ctrlHost.DragRectangle; 	

			// If this dockhost is the hostcontroller for a tab, then update the tab's current information
			if(this.ctrlDockTab != null)
				this.ctrlDockTab.InternalController.DICurrent.rcDockArea = this.dockInfoCurrent.rcDockArea; 
		}		

		internal void MoveFloat()
		{
			// Move the floating frame to new position
			// If the Cursor.Position.Y < 0, then could be multi-monitor setup so allow negative y coords.
			if((this.dockInfoNew.rcControlArea.Y < 0) && (Cursor.Position.Y >= 0))
				this.dockInfoNew.rcControlArea.Y = 0;

			Rectangle drag = GetDragRectangle();
			if( drag != Rectangle.Empty )
				this.ctrlHost.DragRectangle = drag;

			this.dockInfoCurrent.rcControlArea = this.ctrlHost.DragRectangle; 	

			// If this dockhost is the hostcontroller for a tab, then update the tab's current information
			if(this.ctrlDockTab != null)
				this.ctrlDockTab.InternalController.DICurrent.rcDockArea = this.dockInfoCurrent.rcDockArea; 
		}

		public override void InvokeDocking(DockControllerBase dc)
		{
			DockStateControllerBase dhc = dc as DockStateControllerBase;
			Debug.Assert((dhc != null), "Error: Invalid cast.\n");

			if( !this.Floating )
			{
				dhc.PrevAutohideStyle = DockingStyle.None;
				DockTabController dtc = dhc as DockTabController;

				if( dtc != null && !this.Floating )
					foreach( DockTabPage page in dtc.TabControl.TabPages )
					{
						page.dhcClient.PrevAutohideStyle = DockingStyle.None;
					}

				dhc.StoredDockSizes.Clear();
			}
			else
				dhc.StoredFloatSizes.Clear();

			// If this controller is serving as a tabhost, then make the tabcontroller as the docktarget
			if(this.DockTab != null)
				dhc.DINew.dController = this.DockTab.InternalController;

			DockHostController dhctlr = dhc as DockHostController;
			if( dhctlr != null )
			{
				dhctlr.HideCaption = false;
			}
			if(dhc.DINew.DP == DockPreference.Tabbed)
				this.InvokeTabbedDocking(dhc, null);
			else
				this.InvokeRegularDocking(dhc);

			if( this.Floating )			
				( this.ToplevelController as FloatingFormController ).RefreshFormCaption();

			// If dc, the controller being dragged, is from a different DockingManager then remove the DockHostController(s) from 
			// that DockingManager's alControllers list and add it to the current DockingManager instance that is hosting 
			// this docktarget/feedback controller.
			if(dc.DockingManager != this.dockingMgr)
				this.dockingMgr.ImportControl( dc );
		}
		
		protected void InvokeRegularDocking(DockStateControllerBase ddcbase)
		{
			if( this.Maximized )
			{
				this.Maximized = false;
			}

			this.dockingMgr.DockToNewSizingController(ddcbase);

			// Update the dcrs for all controllers involved in this dock operation
			SizingController sc = null;
			DockControllerBase feedbackdc = null;

			if(this.ctrlDockTab == null)
			{
				sc = this.ParentController as SizingController;
				feedbackdc = this;
			}
			else
			{
				sc = this.ctrlDockTab.InternalController.ParentController as SizingController;
				feedbackdc = this.ctrlDockTab.InternalController;
			}

			Debug.Assert((sc != null), "Error: Invalid Cast.\n");

			// When docking onto an existing dockhost, the new dockhost will inherit all of that controllers
			// existing relationships
			ArrayList al = (this.Floating) ? this.alFloatDCR : this.alDockDCR;
			foreach(DCRelationship dcr in al)
			{
				// Ignore tabbed relationships as this is a regular docking
				if(dcr.DP != DockPreference.Tabbed)
					ddcbase.AddToDCR(new DCRelationship(dcr.nRelation, dcr.bChild, dcr.DP, dcr.nIndex));
			}

			int ndcrcode = sc.GetHashCode(); 
			sc.DCRCurrent = new DCRelationship(ndcrcode, false, sc.DICurrent.DP, -1);
			ddcbase.DCRCurrent = new DCRelationship(ndcrcode, true, sc.DICurrent.DP, sc.GetChildHostIndex(ddcbase));
			feedbackdc.InsertIntoDCR( al, 0, new DCRelationship(ndcrcode, true, sc.DICurrent.DP, sc.GetChildHostIndex(feedbackdc)) );

			// Update the related parent level controllers
			// Get the sizing controller's parent, ie., the feedback controller's previous parent, and inform 
			// siblings of the new higher priority relation that the feedback controller will now have with 
			// the dockhost being currently docked. This new relation is characterised by the sizing controller.
			SizingController scparent = sc.ParentController as SizingController;
			if(scparent != null)
			{
				for(int i = 0; i<scparent.ChildCount; i++)
				{
					DockControllerBase dcsib = scparent.GetChildAt(i);
					DockStateControllerBase dhcsib = dcsib as DockStateControllerBase;
					if(dhcsib != null)
						dhcsib.InsertIntoDCR( null, 0, new DCRelationship(ndcrcode, false, scparent.DICurrent.DP, 
							scparent.GetChildHostIndex(dhcsib)) );
				}
			}
		}

		internal override bool FreezeResize
		{
			get
			{
				return base.FreezeResize;
			}
			set
			{
				if( base.FreezeResize != value )
				{
					base.FreezeResize = value;
					DockTabController parentTab = this.ParentController as DockTabController;

					if( value && parentTab != null )
						parentTab.FreezeResize = true;
				}
			}
		}

		protected virtual DockTabControl CreateDockTabControl()
		{
			return new DockTabControl(this.dockingMgr, this);
		}

		protected DCRelationship GetDCRelationship(DockHostController dhc, int nrelation)
		{
			IEnumerator ienum = dhc.DCR;
			while(ienum.MoveNext())
			{
				DCRelationship dhcdcrs = ienum.Current as DCRelationship;
				if(dhcdcrs.nRelation == nrelation)
				{
					return dhcdcrs;
				}
			}
			return null;
		}

		public void InvokeTabbedDocking( DockStateControllerBase ddcbase )
		{
			InvokeTabbedDocking(ddcbase, null);
		}

		internal void InvokeTabbedDocking(DockStateControllerBase ddcbase, DCRelationship dcr)
		{
			// If a tabcontrol does not exist, create one. Else add the new dockhost(s) to the existing tab.
			if(this.ctrlDockTab == null)
			{
				// This feedback controller will serve as the DockTab's LayoutController
				this.ctrlDockTab = this.CreateDockTabControl();
				int nrelation = (dcr == null) ? this.ctrlDockTab.GetHashCode() : dcr.nRelation;
				this.ctrlDockTab.InternalController.DCRCurrent = new DCRelationship(nrelation, false, this.dockInfoCurrent.DP, -1);
				this.ctrlDockTab.AddTab(new DockTabPage(this, this.ctrlHost.Text, this.ImageIndex));
				if(this.GetDCRelationship(this, nrelation) == null)	
					this.DCRCurrent = new DCRelationship(nrelation, true, DockPreference.Tabbed, 0);
			}

			DockHostController dhc = ddcbase as DockHostController;
			if( this.AutoHideMode && dhc != null )
				dhc.bInAutoHide = true;

			ArrayList dcchildlist = new ArrayList();
			DockUtilities.RecGetChildControllers(ddcbase, dcchildlist);

			// Get hold of the dockhost that has the focus
			String strfocus = null;
			foreach(DockHostController host in dcchildlist)
			{
				if(host.HostControl.ContainsFocus)
				{
					strfocus = host.HostControl.Text;
					break;
				}
			}			
			
			if(ddcbase.DINew.nDockIndex > this.ctrlDockTab.TabPages.Count)
				ddcbase.DINew.nDockIndex = this.ctrlDockTab.TabPages.Count;
			int insertpos = ddcbase.DINew.nDockIndex;	
			// If the DockHostController being inserted has the tabbed relation in the list, then use 
			// the relation index to determine the tab insertion position
			DCRelationship ddcbaserelation = this.GetDCRelationship(dcchildlist[0] as DockHostController, this.ctrlDockTab.InternalController.DCRCurrent.nRelation);
			bool recalculateInsertPos = true;

			if( dcr != null && dcr.nRelation == 0 )
				recalculateInsertPos = false;

			if(ddcbaserelation != null && recalculateInsertPos)
			{
				foreach(DockTabPage tabpage in this.ctrlDockTab.TabPages)
				{
					if(tabpage.dhcClient.DCRCurrent.nIndex > ddcbaserelation.nIndex)
					{
						int tabpos = this.ctrlDockTab.TabPages.IndexOf(tabpage);
						if(insertpos > tabpos)
							insertpos = tabpos;
					}
				}
			}

			bool bnewinsertion = false;
			foreach(DockHostController child in dcchildlist)
			{
				// If the childdockhost is docked within a tab and serves as the hostcontroller for that tab, reset the 
				// child's docktab reference to null. Having a valid docktab will interfere with the reparenting.
				if( (child.DICurrent.DP == DockPreference.Tabbed) && (child.DockTab != null) )
					child.DockTab = null;
				child.HostControl.Visible = false;
				this.HostControl.Parent.Controls.Add(child.HostControl);
				DockTabPage newpage = new DockTabPage(child, child.HostControl.Text, child.ImageIndex);
				this.ctrlDockTab.InsertTab(insertpos++, newpage);
				
				if(this.GetDCRelationship(child, this.ctrlDockTab.InternalController.DCRCurrent.nRelation) == null)	
				{
					bnewinsertion = true;				
				}
			}
			if(bnewinsertion == true)
			{
				// Update all tabpages with the new relationship nindex
				foreach(DockTabPage page in this.ctrlDockTab.TabPages)
				{
					page.dhcClient.DCRCurrent = new DCRelationship(this.ctrlDockTab.InternalController.DCRCurrent.nRelation, true, 
						DockPreference.Tabbed, this.ctrlDockTab.TabPages.IndexOf(page));
				}
			}

			// Make the selected tab the hostcontroller for the tabcontrol. 
			DockTabController tabcontroller = this.ctrlDockTab.InternalController as DockTabController;
			tabcontroller.PauseActivation = true;
			if(strfocus == null)
				this.ctrlDockTab.SelectedIndex = insertpos-1;
			else				
			{
				foreach(DockTabPage page in this.DockTab.TabPages)
				{
					if(page.Text == strfocus)
					{
						this.ctrlDockTab.SelectedTab = page;
						break;
					}
				}
			}				

			// Set the feedback controller's size/pos info for the selected tab. This will serve as the 
			// new hostcontroller for the tabcontrol.
			DockTabPage selpage = this.ctrlDockTab.SelectedTab as DockTabPage;
			if(selpage.dhcClient.HostControl != this.HostControl)
			{
				tabcontroller.HostController = selpage.dhcClient;
				selpage.dhcClient.HostControl.Size = this.HostControl.Size;
				selpage.dhcClient.HostControl.Location = this.HostControl.Location;
				tabcontroller.AdjustLayout();
				selpage.dhcClient.HostControl.Visible = true;
				if(this.DockingManager.DesignMode == false)
					selpage.dhcClient.HostControl.Focus();
				this.HostControl.Visible = false;
				selpage.dhcClient.DINew = new DockInfo(this.DINew);
			}
			tabcontroller.PauseActivation = false;

			if( !this.AutoHideMode )
			{
				DockStateControllerWrapper wrap = null;

				if( this.TempWrapper != null 
					&& this.TempWrapper.ParentController == tabcontroller.ParentController)
				{
					wrap = this.TempWrapper;
					wrap.InternalControl = tabcontroller;
					this.TempWrapper = null;
				}
				else
					wrap = new DockStateControllerWrapper(this.DockingManager, tabcontroller);
				tabcontroller.SetChildWrapper(null);
				wrap.ParentController = tabcontroller.ParentController;
				tabcontroller.ParentController.ChildWrapper = wrap;
				tabcontroller.SetChildWrapper(wrap);
				tabcontroller.ParentController.ChildWrapper = null;
			}
			if( this.LayoutRect.Width == 0 || this.LayoutRect.Height == 0 )
				this.ParentController.DITransient.rcDockArea = this.DITransient.rcDockArea;

			// If this controller, ie., the feedback/hostcontroller for the tab is docked, then similar to the InvokeDocking 
			// routine, all new dockhostcontrollers will inherit all relationships, other than the current tabbed relation that 
			// has already been added, of this controller.
			if( this.Floating == false )
			{
				foreach( DockHostController dhcnew in dcchildlist )
				{
					foreach( DCRelationship dcrinh in this.alDockDCR )
					{
						if( dcrinh.nRelation != dhcnew.DCRCurrent.nRelation )
							dhcnew.AddToDCR( new DCRelationship( dcrinh.nRelation, dcrinh.bChild, dcrinh.DP, dcrinh.nIndex ) );
					}
				}
			}
			else
			{
				FloatingForm parentForm = this.ToplevelController.HostControl as FloatingForm;
				parentForm.PaintNCArea();
			}

			if( this.Minimized != Minimization.None)
				this.ParentController.Minimized = this.Minimized;
			if( this.Maximized )
			{
				ddcbase.Maximized = true;
				if( tabcontroller != null )
					tabcontroller.StoredSizes = this.m_storedSizes;
			}

			if( this.FreezeResize || ddcbase.FreezeResize )
				( this.ParentController as DockTabController ).MustResize = Direction.None;

			foreach(DockTabPage page in tabcontroller.TabControl.TabPages)
				page.dhcClient.LayoutRect = this.LayoutRect;
		}

		internal override bool Maximized
		{
			get
			{
				return base.Maximized;
			}
			set
			{
				bool wasMaximized = m_bMaximized;			

				base.Maximized = value;

				if( !( this.ParentController is DockTabController ) )
					if( wasMaximized && !value )
					{
						ControlRestoredEventArgs args = new ControlRestoredEventArgs(this.HostControl.Controls[0]
							, ControlSizeStates.Maximize);
						this.dockingMgr.FireControlSizeStateChanged(ControlSizeStates.Restore, args);
					}
			}
		}

		internal override Minimization Minimized
		{
			get
			{
				return base.Minimized;
			}
			set
			{
				Minimization wasMinimized = m_bMinimized;
				base.Minimized = value;

				if( !( this.ParentController is DockTabController ) )
				{
					if( wasMinimized != Minimization.None && value == Minimization.None )
					{
						ControlRestoredEventArgs args = new ControlRestoredEventArgs(this.HostControl.Controls[0]
							, ControlSizeStates.Minimize);
						this.dockingMgr.FireControlSizeStateChanged(ControlSizeStates.Restore, args);
					}
					else if( wasMinimized == Minimization.None && value != Minimization.None )
					{
						ControlMinimizedEventArgs args = new ControlMinimizedEventArgs(this.HostControl.Controls[0]);
						this.dockingMgr.FireControlSizeStateChanged(ControlSizeStates.Minimize, args);
					}
				}
			}
		}

		internal void HandleMaximizeButtonUp()
		{
			if( this.Maximized )
				RestoreController();
			else
				MaximizeController();
		}

		internal void HandleRestoreButtonUp()
		{
			RestoreController();
		}

		internal override void RestoreController()
		{
			DockTabController tabParent = this.ParentController as DockTabController;

			if( tabParent != null )
				tabParent.RestoreController();
			else
				base.RestoreController();
		}

		internal override void MaximizeController()
		{
			DockTabController tabParent = this.ParentController as DockTabController;

			if( tabParent != null )
				tabParent.MaximizeController();
			else
			{
				if( CanMaximize )
				{
					ControlMaximizeEventArgs e = new ControlMaximizeEventArgs(this.HostControl.Controls[0]);
					this.dockingMgr.FireControlSizeStateChanged(ControlSizeStates.Maximize, e);

					if( !e.Cancel )
					{
						base.MaximizeController();

						ControlMaximizedEventArgs arg = new ControlMaximizedEventArgs(this.HostControl.Controls[0]);
						this.dockingMgr.FireControlSizeStateChanged(ControlSizeStates.Maximized, arg);
					}
				}
			}
		}

		public override bool QueryRelationship(DCRelationship dcr)
		{
			ArrayList al = (this.Floating == true) ? this.alFloatDCR : this.alDockDCR;
			foreach(DCRelationship dcrel in al)
			{
				if( (dcrel.nRelation == dcr.nRelation) && (dcrel.DP == dcr.DP) && (dcrel.bChild == dcr.bChild) )
					return true;
			}			
			return false;
		}

		public override bool AttemptDCRDocking(DockControllerBase ctrl, IEnumerator iedcr)
		{
			DockHostController host = ctrl as DockHostController;			
			Debug.Assert((host != null), "Error: Invalid DockHostController.\n");

			// Before docking to the controller specified in the newdockinfo, run through that controller's children
			// to see if the dockcontroller enjoys a higher relationship with any of the children. If this is true, 
			// then dock to that child, using the relation			
			ControllerDCRPair cdcrp = null;
			DockControllerBase dciterstart = this.ParentController;
			if( (dciterstart.ParentController == null) || (dciterstart.ParentController.MainFormController == true) )
				dciterstart = this;
			
			iedcr.Reset();
			while(iedcr.MoveNext() == true)
			{
				cdcrp = IterChildControllers(dciterstart, iedcr.Current as DCRelationship);
				if(cdcrp != null)
				{
					host.DINew.dController = cdcrp.Controller;
					host.DINew.DP = cdcrp.DCR.DP;
					host.DINew.nDockIndex = cdcrp.DCR.nIndex;
					host.DINew.dController.InvokeDCRDocking(host, cdcrp.DCR);
					return true;
				}
			}
			return false;
		}	

		public override void InvokeDCRDocking(DockControllerBase dc, DCRelationship dcr)
		{
			DockStateControllerBase dhc = dc as DockStateControllerBase;
			Debug.Assert((dhc != null), "Error: Invalid Controller.\n");

			// If this controller is serving as a tabhost, then make the tabcontroller as the docktarget			
			if(this.ParentController is DockTabController)
			{
				dhc.DINew.dController = this.ParentController;
				// If this dockhost is not the selected tab, then forcibly select it. InvokeTabbedDocking can be called
				// only on a dockhost that is serving as the hostcontroller for the tabcontroller. However, just to be safe,
				// we verify this condition and, in case of failure, take the precautionary measure of forcibly making 
				// this dockhost as the selected tab(and hence the hostcontroller) of the tabcontroller.
				if(this.DockTab == null)
				{
					DockTabControl tabcontrol = (this.ParentController as DockTabController).TabControl;					
					foreach(DockTabPage page in tabcontrol.TabPages)
					{
						if(page.dhcClient == this)
						{
							tabcontrol.SelectedTab = page;
							break;
						}
					}					
				}
			}

			if(dhc.DINew.DP == DockPreference.Tabbed)
			{
				InvokeTabbedDocking(dhc, dcr);								
			}
			else
			{
				InvokeInternalDCRDocking(dhc, dcr);
			}
		}

		protected void InvokeInternalDCRDocking(DockStateControllerBase ddcbase, DCRelationship dcr)
		{
			// When inserting into a dockhostcontroller, the DCR index, stored in the dockhost's DINew
			// is used as a hint to determine the dockborder of the new host
			IEnumerator iedcr = this.DCR;
			DCRelationship dcrthis = null;
			while(iedcr.MoveNext() == true)	// Get the child relation that matches the current relation
			{
				dcrthis = iedcr.Current as DCRelationship;
				if(dcrthis.nRelation == dcr.nRelation)
					break;
			}
			if( dcrthis.nIndex < ddcbase.DINew.nDockIndex )	
				ddcbase.DINew.dStyle = (ddcbase.DINew.DP == DockPreference.Horizontal) ? Syncfusion.Windows.Forms.Tools.DockingStyle.Right : Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom;
			else
				ddcbase.DINew.dStyle = (ddcbase.DINew.DP == DockPreference.Horizontal) ? Syncfusion.Windows.Forms.Tools.DockingStyle.Left : Syncfusion.Windows.Forms.Tools.DockingStyle.Top;				

			int nmaxwidth = this.LayoutRect.Width - (this.LayoutRect.Width/this.dockingMgr.MaxRedockFactor);
			int nmaxheight = this.LayoutRect.Height - (this.LayoutRect.Height/this.dockingMgr.MaxRedockFactor);
			if( (ddcbase.DINew.DP == DockPreference.Horizontal) && (ddcbase.DINew.rcDockArea.Width > nmaxwidth) )
				ddcbase.DINew.rcDockArea = new Rectangle(0, 0, nmaxwidth, this.LayoutRect.Height);
			else if( (ddcbase.DINew.DP == DockPreference.Vertical) && (ddcbase.DINew.rcDockArea.Height > nmaxheight) )
				ddcbase.DINew.rcDockArea = new Rectangle(0, 0, this.LayoutRect.Width, nmaxheight);
			
			this.dockingMgr.DockToNewSizingController(ddcbase);
			
			// Update the dcrs for the new parent sizing controller	and the two dockhost controllers
			SizingController sc = null;
			if(this.ctrlDockTab == null)
				sc = this.ParentController as SizingController;
			else
				sc = this.ctrlDockTab.InternalController.ParentController as SizingController;
			Debug.Assert((sc != null), "Error: Invalid Cast.\n");
			sc.DCRCurrent = new DCRelationship(dcr.nRelation, false, sc.DICurrent.DP, -1);
		}

		public override DockControllerBase RedockController(DockInfo di, bool bforcenew)
		{
			Debug.Assert((this.Floating == true), "Incorrect call.\n");

            Rectangle sizes = this.dockInfoNew.rcDockArea;

            this.dockInfoNew = new DockInfo(di);

            if (di.rcDockArea == Rectangle.Empty)
                this.dockInfoNew.rcDockArea = sizes;

			if( bforcenew == false ) 
			{
				if(dockInfoNew.dController.Floating == true)
					TransitFloatToDockInFloat();
				else
					TransitFloatToDock();
			}
			else
			{
				// This is a special case, where the dockhost has to always dock to a new sizing controller. 
				// This usually occurs when the feedback controller is a sizing controller.
				Debug.Assert((di.dController is SizingController), "Error: Invalid feedback controller.\n");

				if(this.dockInfoPrevious.dController != null)
					this.dockInfoPrevious.dController.ControllerChanged -= new ControllerChangedEH(this.dhc_ControllerChanged);
				this.ParentController.ControllerChanged += new ControllerChangedEH(this.dhc_ControllerChanged);

				this.dockInfoPrevious = new DockInfo(this.ParentController, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill, 0, this.ParentController.GetChildHostIndex(this), 
					this.dockInfoCurrent.DP, this.dockInfoCurrent.rcDockArea);

				ExitFloatingFrame();			

				// Clear the dockrelation list
				this.alDockDCR.Clear();

				this.dockingMgr.DockToNewSizingController(this);

				// Update the dcrs for all controllers involved in this dock operation				
				SizingController sc = this.ParentController as SizingController;
				Debug.Assert((sc != null), "Error: Invalid Cast.\n");

				int ndcrcode = sc.GetHashCode(); 
				sc.DCRCurrent = new DCRelationship(ndcrcode, false, sc.DICurrent.DP, -1);

				// Update the parent sizing controller's children
				for(int i = 0; i<sc.ChildCount; i++)
				{
					DockHostController dhcsib = sc.GetChildAt(i) as DockHostController;
					if(dhcsib != null)
						dhcsib.InsertIntoDCR( null, 0, new DCRelationship(ndcrcode, true, sc.DICurrent.DP, 
							sc.GetChildHostIndex(dhcsib)) );
				}								
			}

			if(this.dockInfoCurrent.DP == DockPreference.Tabbed)
				return this.ParentController;
			else
				return this;
		}

		public override void UpdateDCRIndex(DCRelationship dcrs)
		{
			ArrayList al = (this.Floating == true) ? this.alFloatDCR : this.alDockDCR;			
			bool bupdated = false;
			foreach(DCRelationship dcr in al)
			{
				if(dcr.nRelation == dcrs.nRelation)
				{
					dcr.nIndex = dcrs.nIndex;
					bupdated = true;
				}
			}
			// If the relation does not exist, then insert this at the head of the list
			if(bupdated == false)
				this.InsertIntoDCR( al, 0, new DCRelationship(dcrs.nRelation, true, dcrs.DP, this.ParentController.GetChildHostIndex(this)) );
		}

		protected FloatingForm tempForm = null;

		public virtual FloatingForm CreateFloatingFrame(Point ptlocation, bool show )
		{
			// Create an instance of the floating form and dock this host onto it
			FloatingForm frmfloat;
			bool restored = false;
			frmfloat = SelectActivationForm(ref restored);
			if( frmfloat.InternalController.DockingManager != this.dockingMgr )
				frmfloat.InternalController.DockingManager = this.dockingMgr;

			if( ( this.dockInfoNew.rcDockArea.Width <= 0 || this.dockInfoNew.rcDockArea.Height <= 0 ) == true )
			{
				if( ptlocation != Point.Empty )
				{
					Syncfusion.Runtime.InteropServices.NativeMethods.MoveWindow(frmfloat.Handle,
						ptlocation.X, ptlocation.Y, 0, 0, false);
					frmfloat.Size = this.ctrlHost.Bounds.Size;
					this.dockInfoNew.rcDockArea = frmfloat.Bounds;
					this.ctrlHost.DragRectangle = this.dockInfoNew.rcDockArea;
				}
			}
			else
			{
				// If the Cursor.Position.Y < 0, then could be multi-monitor setup so allow negative y coords.
				if( ( this.dockInfoNew.rcDockArea.Y < 0 ) && ( Cursor.Position.Y >= 0 ) )
					this.dockInfoNew.rcDockArea.Y = 0;
				if( this.DINew.rcDockArea != Rectangle.Empty)
					Syncfusion.Runtime.InteropServices.NativeMethods.MoveWindow(frmfloat.Handle,
						this.dockInfoNew.rcDockArea.Left, this.dockInfoNew.rcDockArea.Top, 0, 0, false);
				frmfloat.Size = this.ctrlHost.DragRectangle.Size;
			}

			this.HideCaption = false;
			if (this.dockingMgr.bLoadVisibility == true)
				frmfloat.Enable( show );

			if( restored
				&& this.InternalFloatWrapper != null
				&& this.InternalFloatWrapper.ParentController != null
				&& this.InternalFloatWrapper.ParentController.ChildControllers.Contains(InternalFloatWrapper) )
			{
				InternalTransitToPrevState(true);
			}
			else
				frmfloat.InternalController.AddChild(this, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill);

            this.dockingMgr.UpdateFloatingFormsImages();
			frmfloat.Controls.Add(this.ctrlHost);
			( frmfloat.InternalController as FloatingFormController ).RefreshFormCaption();
            
            //This method will call recursively to set tooltip for all child controls of the floated window.
            this.SetToolTipFloatingControl(this.ctrlHost.Controls);
						
			frmfloat.InternalController.AdjustLayout();

			RightToLeft rtlFormEffective = dockingMgr.IsMirrored ? 
				System.Windows.Forms.RightToLeft.Yes : 
				System.Windows.Forms.RightToLeft.No;
			frmfloat.UpdateChildsRightToLeft( rtlFormEffective );

			if( DockingManager.VisualStyle != VisualStyle.Default
				&& DockingManager.VisualStyle != VisualStyle.VS2005 )
			{
				NativeMethodsHelper.RedrawWindow( frmfloat.Handle
					, NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_FRAME | NativeMethods.RDW_ALLCHILDREN );
			}
			if( this.dockingMgr.DesignMode )
				this.ctrlHost.Focus();

			return frmfloat;
		}

		protected virtual FloatingForm SelectActivationForm( ref bool restored )
		{
			FloatingForm frmfloat;
			if( this.SharedForm != null && !this.SharedForm.IsDisposed && !this.SharedForm.Used &&
						  this.m_form != this.SharedForm )
			{
				this.SharedForm.Used = true;
				frmfloat = this.SharedForm;
				frmfloat.FormOwner.InternalForm = m_form;
				frmfloat.FormOwner = this;
				frmfloat.Enabled = true;
				m_form = frmfloat;
				restored = true;
				this.DockingManager.AddFFController(frmfloat.InternalController as FloatingFormController);
			}
			else
			{
				if( m_form != null && !m_form.IsDisposed && m_form.InternalController != null && !m_form.IsDisposed )
				{
					frmfloat = m_form;
					frmfloat.Enabled = true;
					m_form.Used = true;
					this.DockingManager.AddFFController(frmfloat.InternalController as FloatingFormController);
					if( m_form == this.SharedForm && !this.Floating )
						restored = true;
				}
				else
				{
					frmfloat = DockingManager.CreateFloatingForm();
					this.InternalForm = frmfloat;
				}
			}
			return frmfloat;
		}

        private void SetToolTipFloatingControl(System.Windows.Forms.Control.ControlCollection cntrls)
        {
            string sToolTip = string.Empty;
            if (this.dockingMgr.HostFormComponents != null)
            {
                foreach (Component c in dockingMgr.HostFormComponents)
                {
                    if (c is ToolTip)
                    {
                        ToolTip floatToolTip = new ToolTip();
                        foreach (Control ctrl in cntrls)
                        {
                            sToolTip = ((ToolTip)c).GetToolTip(ctrl);
                            if (sToolTip.Length != 0)
                            {
                                floatToolTip.SetToolTip(ctrl, sToolTip);
                            }
                            //Fixed the defect, If any docked panels contained the some child controls, tooltip does not show for that.
                            if (ctrl.Controls.Count > 0)
                            {
                                this.SetToolTipFloatingControl(ctrl.Controls);
                            }
                        }
                    }
                }
            }
        }

		protected internal virtual void ExitFloatingFrame()
		{
			FloatingForm frmfloat = this.ctrlHost.ParentForm as FloatingForm;
			this.dockInfoCurrent.rcDockArea = frmfloat.Bounds;
			this.dockInfoPrevious.rcDockArea.Location = frmfloat.Location;
			this.ctrlHost.DragRectangle = frmfloat.Bounds;

			this.dockingMgr.UndockFromController(this);	
			AssignFormBelongings( frmfloat );
		}

		internal virtual void AssignFormBelongings( FloatingForm frmfloat )
		{
			// If this was the only controller attached to the form, then dispose of the form
			if( frmfloat == this.InternalForm )
			{
				frmfloat.Disable();
				if( this.DINew.dController != null &&
					frmfloat.InternalController.DockingManager != this.DINew.dController.DockingManager )
					frmfloat.InternalController.DockingManager = this.DINew.dController.DockingManager;
				m_form = frmfloat;
				m_form.FormOwner = this;
				m_form.Used = false;
			}
			else
			{
				(frmfloat.InternalController as FloatingFormController).RefreshFormCaption();
			}

			this.SharedForm = frmfloat;
		}

		public void ToggleAutoHideMode( bool animate )
		{
			Debug.Assert(( this.DockVisibility ? !this.Floating : true ), "Invalid call - AutoHide unavailable while floating.\n");
			Control[] ctrls = null;
            bool prevFreezeDockStateChangeEvent = DockingManager.bFreezeDockStateChangeEvents;
			if(this.bInAutoHide == false)
			{
				MainFormController mfctrlr = dockingMgr.alDockAreaControllers[0] as MainFormController;

				if( mfctrlr != null )
				{
					if (AttemptPrevAHDocking(mfctrlr.GetAHTabControl(
						(this as DockStateControllerBase).DINew.dStyle), this, animate))
						return;
				}

				if((this.ParentController is DockTabController) == false)
				{
					ctrls = new Control[] {this.ctrlHost.Controls[0]};
					this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));
                    DockingManager.bFreezeDockStateChangeEvents = true;

					this.EnterAutoHideMode( animate );
				}
				else
				{
					DockTabController tabctrlr = this.ParentController as DockTabController;
					if(this.ctrlDockTab == null)
						tabctrlr.SelectedController = this;

					ctrls = new Control[tabctrlr.TabControl.TabCount];
					int i = 0;
					foreach(DockTabPage page in tabctrlr.TabControl.TabPages)
						ctrls[i++] = page.dhcClient.HostControl.Controls[0];	
					this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));
                    DockingManager.bFreezeDockStateChangeEvents = true;

					tabctrlr.EnterAutoHideMode();
				}
                DockingManager.bFreezeDockStateChangeEvents = prevFreezeDockStateChangeEvent;
				this.dockingMgr.FireDockStateChangeEvent("DockStateChanged", new DockStateChangeEventArgs(ctrls));
			}
			else
			{
				if((this.ParentController is DockTabController) == false)
				{
					ctrls = new Control[] {this.ctrlHost.Controls[0]};
					this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));
                    DockingManager.bFreezeDockStateChangeEvents = true;

					this.ExitAutoHideMode(false);
				}
				else
				{
					DockTabController tabctrlr = this.ParentController as DockTabController;
					if(this.ctrlDockTab == null)
						tabctrlr.SelectedController = this;

					ctrls = new Control[tabctrlr.TabControl.TabCount];
					int i = 0;
					foreach(DockTabPage page in tabctrlr.TabControl.TabPages)
						ctrls[i++] = page.dhcClient.HostControl.Controls[0];	
					this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));
                    DockingManager.bFreezeDockStateChangeEvents = true;

					tabctrlr.ExitAutoHideMode(false);
				}
				this.ctrlHost.Controls[0].Focus();
                DockingManager.bFreezeDockStateChangeEvents = prevFreezeDockStateChangeEvent;
				this.dockingMgr.FireDockStateChangeEvent("DockStateChanged", new DockStateChangeEventArgs(ctrls));

			}
		}

		internal bool AttemptPrevAHDocking( AHTabControl autoHideTab, DockControllerBase ctrl, bool animate )
		{
			bool success = false;

			if( autoHideTab != null )
			{
				for( int i = 0; i < autoHideTab.TabPages.Count; i++ )
				{
					AHTabPage page = autoHideTab.TabPages[i] as AHTabPage;

					if( page != null && page.m_dhcClient.DCRCurrent != null && ctrl.DCRCurrent != null )
					{
						if( (page.m_dhcClient.DCRCurrent.nRelation ==	ctrl.DCRCurrent.nRelation )
							&& ctrl.DCRCurrent.DP == DockPreference.Tabbed && ctrl.DCRCurrent.nRelation != 0)
						{
							DockingStyle style = page.m_dhcClient.DINew.dStyle;
							DockStateControllerBase tempController = page.m_dhcClient;

							if (!animate)
							{
								success = true;
								if( ctrl.ParentController != null
									&& ctrl.ParentController.GetChildHostIndex(ctrl) != -1)
									this.dockingMgr.UndockFromController(ctrl);
								autoHideTab.RemoveTab(tempController);
								Size ahSize = this.DINew.rcDockArea.Size; 
								this.dockingMgr.HostControl.Controls.Add(ctrl.HostControl);
								DockInfo tabInfo = tempController.DITransient;
								tempController.AttemptDCRDocking(ctrl, ctrl.DCR);
								((DockStateControllerBase)ctrl).DINew.dStyle = style;
								DockHostController host = ctrl as DockHostController;

								if( tempController is DockTabController )
								{
									autoHideTab.AddTab(tempController, false);
									if( host.DINew.rcDockArea.Size == Size.Empty )
										host.DINew.rcDockArea.Size = ahSize;
								}
								else
								{
									DockTabController tab = tempController.ParentController as DockTabController;
									autoHideTab.AddTab(tab, false);
									tab.DINew.dStyle = style;
									tab.bInAutoHide = true;
									tab.DITransient = tabInfo;
									tab.DINew.rcDockArea.Size = ahSize;
									tab.TabControl.Size = ahSize;
									foreach (DockTabPage dockPage in tab.TabControl.TabPages)
									{
										DockStateControllerBase controller = dockPage.dhcClient;
										controller.DINew.rcDockArea.Size = tab.TabControl.Size;
										controller.DITransient.rcDockArea.Size = tab.TabControl.Size;
									}									
								}
								(ctrl as DockHostController).bInAutoHide = true;
							}
							else 
							{
								autoHideTab.ShowController(tempController, false);
								tempController.ExitAutoHideMode(false);
							}
							break;
						}
					}
				}
			}
			return success;
		}
		
		public void ToggleAutoHideMode()
		{
			ToggleAutoHideMode( true );
		}
		
		protected void EnterAutoHideMode( bool animate )
		{
            bool hostFormVisible = false;
            if(this.DockingManager.HostForm != null)
                hostFormVisible = this.DockingManager.HostForm.Visible;
			MainFormController mfctrlr = dockingMgr.alDockAreaControllers[0] as MainFormController;
			if(this.ctrlDockTab == null)
			{
				if( this.Maximized )
					RestoreController();

				if( animate )
				{
					// Before entering AH mode, subscribe to the current parent controller's ControllerChanged event
					this.ParentController.ControllerChanged += new ControllerChangedEH(this.TransientControllerChanged);
					// Store the current info within DICurrent. Cannot use DIPrevious as this contains the float info.
					this.dockInfoTransient = new DockInfo(this.ParentController, this.dockInfoCurrent.dStyle, this.dockInfoCurrent.nPriority, 
						this.ParentController.GetChildHostIndex(this), this.dockInfoCurrent.DP, this.dockInfoCurrent.rcDockArea);
				}

				this.bInAutoHide = true;
				mfctrlr.EnterAutoHideMode(this, animate);

				// Get hold of the sibling DragSplitterController through the parent SizingController and 
				// subscribe to the splitter's DragSplitterMoved event
				DockControllerBase splitterdc = this.ParentController.GetChildAt(1);
				DragSplitter splitter = splitterdc.HostControl as DragSplitter;
				Debug.Assert(splitter != null);
				splitter.DragSplitterMoved += new SplitterEventHandler(this.DHCDragSplitterMoved);				
			}			
			mfctrlr.AdjustLayout();
			if( DockingManager.AHInViewTab != null )
				DockingManager.AHInViewTab.HideController( null, animate, true );
            if (this.DockingManager.HostForm != null && !hostFormVisible && this.DockingManager.HostForm.Visible)
                this.DockingManager.HostForm.Visible = hostFormVisible;
		}
		
		public override void EnterAutoHideMode()
		{
			EnterAutoHideMode( true );
		}

		public void LoadInAutoHideMode()
		{
			MainFormController mfctrlr = this.ToplevelController as MainFormController;
			this.dockInfoTransient = new DockInfo(this.dockInfoCurrent.dController, this.dockInfoCurrent.dStyle, this.dockInfoCurrent.nPriority,
				this.dockInfoCurrent.nDockIndex, this.dockInfoCurrent.DP, this.dockInfoCurrent.rcDockArea);
				
			this.bInAutoHide = true;
			mfctrlr.LoadInAutoHideMode(this);

			// Get hold of the sibling DragSplitterController through the parent SizingController and 
			// subscribe to the splitter's DragSplitterMoved event
			DockControllerBase splitterdc = this.ParentController.GetChildAt(1);
			DragSplitter splitter = splitterdc.HostControl as DragSplitter;
			Debug.Assert(splitter != null);
			splitter.DragSplitterMoved += new SplitterEventHandler(this.DHCDragSplitterMoved);

			mfctrlr.AdjustLayout();
		}

		public override void ExitAutoHideMode(bool bcloseonexit)
		{
			this.dockingMgr.HostControl.SuspendLayout();
			MainFormController mfctrlr = this.ToplevelController as MainFormController;
			if(this.ctrlDockTab == null)
			{
				// Unsubscribe from the form controller
				if( this.dockInfoTransient.dController != null )
					this.dockInfoTransient.dController.ControllerChanged -= new ControllerChangedEH(this.TransientControllerChanged);
				this.bInAutoHide = false;

				// Get hold of the sibling DragSplitterController through the parent SizingController and 
				// unsubscribe from the splitter's DragSplitterMoved event
				DockControllerBase splitterdc = this.ParentController.GetChildAt(1);
				DragSplitter splitter = splitterdc.HostControl as DragSplitter;
				Debug.Assert(splitter != null);
				splitter.DragSplitterMoved -= new SplitterEventHandler(this.DHCDragSplitterMoved);				

				mfctrlr.ExitAutoHideMode(this, bcloseonexit);
				this.bAutoHideSizing = false;
			}			
			mfctrlr.AdjustLayout();
			this.dockingMgr.HostControl.ResumeLayout();
		}

		public void DHCDragSplitterMoved(Object sender, SplitterEventArgs args)
		{
			Debug.Assert(this.bInAutoHide);
			if(this.bInAutoHide)
			{
				// AutoHide mode uses DINew for calculating the control bounds while displaying.
				if(this.DICurrent.DP == DockPreference.Tabbed)
				{
					if(this.ctrlDockTab != null)
						this.DINew.rcDockArea = this.DICurrent.rcDockArea;
				}
				else
				{
					this.DINew.rcDockArea = this.DICurrent.rcDockArea;
				}				
			}			
		}
		
		public DockInfo GetSerCurrentDI()
		{
			DockInfo di = DockInfo.NullInfo;
			if(this.DockVisibility == false)
			{
				if( (this.m_prevWrapper == null) || (this.m_prevWrapper.ParentController.Floating == true) )
				{
					di = new DockInfo(null, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill, 0, 0, DockPreference.All, this.dockInfoTransient.rcDockArea);					
				}
				else
				{
					MainFormController mfc = this.m_prevWrapper.ParentController.ToplevelController as MainFormController;
					if(this.dockInfoTransient.dController == mfc)
						di = new DockInfo(this.dockInfoTransient);
					else
						di = new DockInfo( mfc.GetDockInfoForController(this.dockInfoTransient.dController));
				}
			}
			else
			{
				if(this.Floating == true)
				{
					if(this.bInMDIMode == false)
						di = new DockInfo(null, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill, 0, 0, DockPreference.All, this.ctrlHost.ParentForm.Bounds);				
					else
						di = new DockInfo(null, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill, 0, 0, DockPreference.All, this.dockInfoCurrent.rcDockArea);
				}
				else 	
				{
					if(this.bInAutoHide == true)	// Get DIInfo from transient controller
					{
						MainFormController mfc = this.ToplevelController as MainFormController;
					
						DockStateControllerBase dcredock = this;
						if(this.ParentController is DockTabController)
							dcredock = this.ParentController as DockStateControllerBase;

						if(dcredock.DITransient.dController == mfc)
							di = new DockInfo(dcredock.DITransient);
						else
							di = new DockInfo( mfc.GetDockInfoForController(dcredock.DITransient.dController) );
					}
					else	// Normal docking
					{					
						di = this.DINew;
					}
				}
			
				// If housed in a tab, then formulate the tabbed relationship and add it to the dcrlist
				if( this.ParentController is DockTabController )
				{
					DockTabController docktabctrlr = this.ParentController as DockTabController;
					foreach(DockTabPage page in docktabctrlr.TabControl.TabPages)
					{
						if(page.dhcClient == this)
						{
							this.DCRCurrent = new DCRelationship(docktabctrlr.DCRCurrent.nRelation, true, 
								DockPreference.Tabbed, docktabctrlr.TabControl.TabPages.IndexOf(page));
							break;
						}
					}						
				}
			}
			return di;
		}

		public DockInfo GetSerPreviousDI()
		{
			DockInfo di = new DockInfo(this.dockInfoPrevious);
			if( (di.dController == null) || (di.dController.Floating == true) )
			{
				di.DP = DockPreference.All;
			}
			di.dController = null;
			return di;
		}	

		internal HitTestArea hitArea = HitTestArea.None;
		internal int hitButtonNumber = -1;

		internal void AssignHitArea( HitTestArea hitTestArea )
		{
			if( hitArea != hitTestArea || 
                Renderer.GetHighlightedButtonIndex() != Renderer.GetHitButtonIndex() )
			{
				hitArea = hitTestArea;
				int borderWidth = Renderer.ThinBorderWidth;
				int captionWidth = Renderer.CaptionWidth;
				Rectangle rectangle = new Rectangle( borderWidth, borderWidth,
					HostControl.Width - 2 * borderWidth, Renderer.CaptionWidth );
				HostControl.Invalidate( rectangle, true );
			}
		}
		
		private bool HitOnCaption( Point pt )
		{
			if( (RendererStyle == VisualStyle.Default)
				|| (RendererStyle == VisualStyle.VS2005 && Floating) )
			{
				CaptionPainter.CaptionHitTest caphittest = 
					ctrlHost.TitleBar.HitTest(MouseAction.LBtnDown, pt);
				return caphittest == CaptionPainter.CaptionHitTest.CaptionRect;
			}
			else
			{
				if( this.Floating )
				{
					if( this.IsSingleFloatControl )
					{
						pt.Offset( Renderer.ThinBorderWidth, Renderer.ThinBorderWidth + Renderer.CaptionWidth );
					}
					else
					{
						pt.Offset( Renderer.ThinBorderWidth, Renderer.ThinBorderWidth );
					}
				}
				HitTestArea hitTestArea = Renderer.HitTest( MouseButtons.Left, pt );
				AssignHitArea( hitTestArea );
				return hitTestArea == Renderers.HitTestArea.Caption;
			}
		}
		
		protected internal void HandleMouseDownImp(MouseButtons button, Point pt)
		{
			if(button == MouseButtons.Left)
			{				
				DockTabController dtc = ParentController as DockTabController;
				if( bAutoHideSizing || (dtc != null && dtc.bAutoHideSizing) )
				{
					return;
				}

				if( HitOnCaption( pt ) )
				{
					// Fire the DragAllow event, and provide a chance to preempt the drag
					DragAllowEventArgs dragallow = new DragAllowEventArgs(this.ctrlHost.Controls[0]);
					this.dockingMgr.FireDragAllowEvent(dragallow);
					if( !this.dockingMgr.DesignProcess && this.dockingMgr.DragProvider is BorderDragProvider )
						this.ctrlHost.Capture = true;
					if(dragallow.Cancel == false)
					{
						this.dockingMgr.DragProvider.ProcessMouseDown( this, ctrlHost, ctrlHost.PointToScreen(pt));
						Rectangle dragRect = GetDragRectangle();
						if( dragRect != Rectangle.Empty )
							this.ctrlHost.DragRectangle = dragRect;
					}
				}
			}
		}

		private Rectangle GetDragRectangle()
		{
			Rectangle drag = Rectangle.Empty;
			if(this.Floating == true)
			{
				if(this.bHideCaption == true || !(this.ParentController is FloatingFormController))
				{
					if(this.ctrlHost.Parent.Size.Equals(this.ctrlHost.DragRectangle.Size) == false)
					{
						if( this.ParentController is SizingController )
						{
							drag = this.ctrlHost.Bounds;
							drag.Height += SystemInformation.ToolWindowCaptionHeight;
						}
						else
							drag = this.ctrlHost.Parent.Bounds;
						drag.Inflate(3,3);
					}
				}
				else if(this.ctrlHost.Size.Equals(this.ctrlHost.DragRectangle.Size) == false)
					drag = this.ctrlHost.Bounds;				
			}

			return drag;
		}

		private bool CheckHitValue( CaptionPainter.CaptionHitTest hitTest, 
			CaptionPainter.CaptionHitTest targetHitTest, HitTestArea hitTestArea)
		{
			if( RendererStyle == VisualStyle.Default )
			{
				return hitTest == targetHitTest;
			}
			else
			{
				return hitArea == hitTestArea;
			}
		}

		protected internal void HandleMouseUpImp(MouseButtons button, Point pt)
		{
			DockingManager.LockActivationEvents++;
			try
			{
				if(this.ctrlHost.Capture == true)
					this.ctrlHost.Capture = false;

				if(this.dockingMgr.DragProvider.DraggingControl == null)
				{
					MouseAction action = (button == MouseButtons.Right) ? MouseAction.RBtnUp : MouseAction.LBtnUp;

                    // Clear the Drag-Start state.
                    if (button == MouseButtons.Left)					
                        this.dockingMgr.DragProvider.ProcessMouseUp(this, ctrlHost, ctrlHost.PointToScreen(pt));

					CaptionPainter.CaptionHitTest hittest = CaptionPainter.CaptionHitTest.None;
					DockTabController dtc = ParentController as DockTabController;
					if( bAutoHideSizing || (dtc != null && dtc.bAutoHideSizing) )
					{
						return;
					}
					if( RendererStyle == VisualStyle.Default )
					{
						hittest = this.ctrlHost.TitleBar.HitTest(action, pt);	
					}
					else
					{
						// Refresh paint data of renderer.
						this.HostControl.Invalidate(true);
						this.HostControl.Update();

						AssignHitArea( Renderer.HitTest( button, pt ) );
					}

					if(this.dockingMgr.DesignProcess == false)	// Ignore close and autohide in Designmode
					{

						if( CheckHitValue( hittest, CaptionPainter.CaptionHitTest.ButtonUp,
							HitTestArea.Button ) && button == MouseButtons.Left  )
						{
							int index = 
								( DockingManager.VisualStyle == VisualStyle.Default )?
								ctrlHost.cpTitleBar.GetCaptionButtonIndex() :
								Renderer.GetHitButtonIndex();
							if( index >= 0 )
							{
								CaptionButton cpbutton = 
									( DockingManager.VisualStyle == VisualStyle.Default )?
									ctrlHost.cpTitleBar.GetHitButton() :
									Renderer.GetHitButton();

								CancelEventArgs e = new CancelEventArgs(false);
								cpbutton.FireClickEvent( e );

								if( cpbutton != null && !e.Cancel )
								{
									switch( cpbutton.Type )
									{
										case CaptionButtonType.Close:
											if ((this.ctrlDockTab != null) && (this.bHideCaption == true))
												this.TabhostCloseAllControllers();
											else
											{
												bClosingByMouse = true;
												this.DockVisibility = false;
												bClosingByMouse = false;
											}
											break;

										case CaptionButtonType.Pin:
											hitArea = HitTestArea.None;
											if (button == MouseButtons.Left)	// Clear the Drag-Start state				
												this.dockingMgr.DragProvider.ProcessMouseUp(this, ctrlHost, ctrlHost.PointToScreen(pt));
											this.ToggleAutoHideMode();
											break;

										case CaptionButtonType.Menu:
											//Fix for #1614: Performing unwanted autohide animation before dockcontextmenu became visible
											if (this.ctrlHost.Controls[0] != null && this.ctrlHost.Controls[0].ContainsFocus == false)
												this.ctrlHost.Controls[0].Focus();
											this.dockingMgr.ShowMenu(this, this.ctrlHost.PointToScreen(pt));
											this.hitArea = HitTestArea.None;
											break;

										case CaptionButtonType.Maximize:
											this.HandleMaximizeButtonUp();
											break;

										case CaptionButtonType.Restore:
											this.HandleRestoreButtonUp();
											break;
									}
								}
							}
						}

						if((button == MouseButtons.Right) && CheckHitValue( hittest,
							CaptionPainter.CaptionHitTest.CaptionRect, HitTestArea.Caption ))
						{
							if (this.ctrlHost.Controls[0] != null && this.ctrlHost.Controls[0].ContainsFocus == false)
								this.ctrlHost.Controls[0].Focus();
							this.dockingMgr.ShowMenu(this, this.ctrlHost.PointToScreen(pt));
							this.hitArea = HitTestArea.None;
						}

						return;
					}
				}
				else if(button == MouseButtons.Left)  // A drag is in progress
				{
					Control ctrlfocus = null;
					if(this.HostControl.ContainsFocus == true)
						ctrlfocus = Control.FromHandle(Syncfusion.Runtime.InteropServices.NativeMethods.GetFocus());

					this.dockingMgr.DragProvider.ProcessMouseUp( this, ctrlHost, ctrlHost.PointToScreen( pt ) );
				}
			}
			finally
			{
				DockingManager.LockActivationEvents--;
			}
		}

		internal protected bool IsSingleFloatControl
		{
			get 
			{
				FloatingFormController ffc = this.ToplevelController as FloatingFormController;
				bool single = false;

				if( ffc != null )
				{
					if( ffc.dcChild is SizingController )
					{
						SizingController sc = ffc.dcChild as SizingController;

						if( sc.GetDockControllers().Count == 1 )
						{
							single = true;
						}
					}else
					{
						DockTabController parentDtc = ffc.dcChild as DockTabController;

						if( parentDtc == this.ParentController )
							single = true;
						else if( ffc.dcChild == this )
							single = true;
					}
				}

				return single;
			}
		}

		protected internal void HandleMouseMoveImp(MouseButtons button, Point pt)
		{
			Point ptscreen = this.ctrlHost.PointToScreen(pt);
			
			if(this.dockingMgr.DragProvider.DraggingControl == null)
			{
				if( RendererStyle == VisualStyle.Default )
				{
					CaptionPainter.CaptionHitTest hitval = this.ctrlHost.TitleBar.HitTest(MouseAction.MouseMove, pt);
					if( (hitval == CaptionPainter.CaptionHitTest.ButtonDown) || (hitval == CaptionPainter.CaptionHitTest.ButtonUp) || (hitval == CaptionPainter.CaptionHitTest.ButtonRect) )
					{
						int index = ctrlHost.cpTitleBar.GetCaptionButtonIndex();
						if( index >= 0 && ctrlHost.cpTitleBar.GetHitButton().Type == CaptionButtonType.Close )
							return;
					}
				}
				else
				{
					AssignHitArea( Renderer.HitTest( MouseButtons.None, pt ) );
				}
			}

            if (button == MouseButtons.Left)
                  this.dockingMgr.DragProvider.ProcessMouseMove(this, ctrlHost, ptscreen);
		}

		protected internal void HandleDoubleClickImp(Point ptclient)
		{
			if(this.AutoHideMode == true)	// Ignore drag, undock etc., if in AutoHide mode
				return;
			this.dockingMgr.DragProvider.TerminateDrag( this.HostControl as IDraggable
				, this.HostControl.PointToScreen( ptclient ) );
			bool bHitOnCaption = false;
			if( DockingManager.VisualStyle == VisualStyle.Default )
			{
				if( HitOnCaption(ptclient) )
				{
					bHitOnCaption = true;
				}
			}
			else
			{
				if( Renderer.HitTest( MouseButtons.None, ptclient ) == HitTestArea.Caption )
				{
					bHitOnCaption = true;
				}
			}

			if( bHitOnCaption )
			{
				bool allowFloating = this.AllowFloating;
				if( this.ParentController is DockTabController )
					allowFloating = this.ParentController.AllowFloating;
				if((this.dockingMgr.DisallowFloating || !allowFloating) && this.Floating == false)
					return;

				// Fire the DragAllow event, and provide a chance to preempt the drag
				DragAllowEventArgs dragallow = new DragAllowEventArgs(this.ctrlHost.Controls[0]);
				this.dockingMgr.FireDragAllowEvent(dragallow);
				if(dragallow.Cancel == false)
				{
					Control ctrlfocus = null;
					if(this.ctrlHost.ContainsFocus == true)
						ctrlfocus = Control.FromHandle(Syncfusion.Runtime.InteropServices.NativeMethods.GetFocus());

					this.dockingMgr.DragProvider.ProcessDoubleClick();
			
					if(this.ctrlDockTab == null)
					{
						this.InvokePrevDockFloatTransition( true );
					}
					else	// Hosted within a tabcontroller
					{
						DockTabController tabcontroller = this.ctrlDockTab.InternalController as DockTabController;
						tabcontroller.InvokePrevDockFloatTransition( true );					
					}
					dockingMgr.UpdateFloatingFormsImages();

					if(this.DockingManager.DesignProcess == false)
					{
						// A state change has occurred, make sure that this control has the focus
						this.ctrlHost.Parent.Focus();
						if(ctrlfocus != null)
							ctrlfocus.Focus();
						else
							this.ctrlHost.Controls[0].Focus();
					}
				}
			}
		
			// Forcibly update the designer state
			if(this.dockingMgr.DesignMode == true)
				this.dockingMgr.UpdateDesigner();
		}

		protected override void Dispose(bool bdisposing)
		{
			if((bdisposing == true) && (this.ctrlHost != null))
			{
				this.ctrlHost.Dispose();			
				this.ctrlHost = null;
			}
			base.Dispose(bdisposing);
		}
		
		internal override void AddWrapper(ControllerWrapper cw)
		{
			DockHostControllerWrapper dhcw = new DockHostControllerWrapper();
			dhcw.LayoutRect = LayoutRect;
			dhcw.ControlName = HostControl.Controls[0].Name;
			dhcw.UniqueName = UniqueName;
            dhcw.DockEdge = this.DockEdge;
			cw.Children.Add(dhcw);
		}		

		internal override void StoreControllers(ArrayList controllers)
		{
			base.StoreControllers (controllers);
			
			this.ParentController.RemoveChild(this);
			controllers.Add(this);
		}

		internal override void ApplyWrapper(ControllerWrapper cw)
		{
			base.ApplyWrapper (cw);
			LayoutControllerWrapper lcw = (LayoutControllerWrapper) cw;
			LayoutRect = lcw.LayoutRect;
		}

		internal override bool IsEqual(ControllerWrapper cw)
		{
			if( cw is DockHostControllerWrapper )
			{
				if( (cw as DockHostControllerWrapper).UniqueName == UniqueName )
				{
					return true;
				}
			}

			return false;
		}

		internal override bool IsFloatOnly()
		{
			return bFloatOnly;
		}

		#region IResizable implementation
	
		public Size CalculateSize(Size parentSize, Size newParentSize)
		{
			return ControllerSizeCalculator.CalculateSize(this, parentSize, newParentSize);
		}

		public bool IsVerticallyResizable()
		{
			Direction resizeDir = this.MustResize;

			return ( Minimized == Minimization.None || Minimized == Minimization.Horizontal ) &&
				( !this.FreezeResize || resizeDir == Direction.Vertical && resizeDir != Direction.None );
		}

		public bool IsHorizontallyResizable()
		{
			Direction resizeDir = this.MustResize;

			return ( Minimized == Minimization.None || Minimized == Minimization.Vertical ) &&
				( !this.FreezeResize || resizeDir == Direction.Horizontal && resizeDir != Direction.None );
		}

		#endregion

		internal override void SetAutohiddenControlSize( Size size )
		{
			DockTabController parentController = ParentController as DockTabController;
			if( parentController != null )
			{
				parentController.SetAutohiddenControlSize( size );
			}
			else
			{
				Rectangle rectangle = new Rectangle( DINew.rcDockArea.Location, size );

				DINew.rcDockArea = rectangle;
				DITransient.rcDockArea = rectangle;
			}
		}

		public override void ApplyDockInfo()
		{
			base.ApplyDockInfo();
			bool freeze = this.dockingMgr.ForbidFreeze;
			//DockStateControllerBase freezedTarget = null;
			Control ctrlfocus = null;
			if(this.HostControl.ContainsFocus == true)
				ctrlfocus = Control.FromHandle(Syncfusion.Runtime.InteropServices.NativeMethods.GetFocus());
			this.dockingMgr.ForbidFreeze = true;

			//this.dockingMgr.DragProvider.ProcessMouseUp(this.ctrlHost, this.ctrlHost.PointToScreen(pt));
		
			// There are a number of conditions that are possible here:
			// If DINew controller is null and dockhost's parentcontroller is a FloatingFormController, then 
			// the form is being is being moved around. Invoke MoveFloatToFloat().
			// If the DINew controller is null and float condition is false, then the dockhost is being dragged from 
			// a docked to a float state. Invoke TransitDockToFloat().
			// If DINew controller is null and float condition is true, then the dockhost is being dragged out 
			// of a floating form. Invoke TransitDockInFloatToFloat().
			// If the current and new controllers are the same, then an intra-state transition occurs. The dock host
			// retains the current dock/float state after the transition.
			// If the controllers vary, then a dock - float or float - dock inter-state transition takes place.
			if( this.ParentController != null && !(this.ToplevelController is DockStateControllerWrapper))
			{
				if( this.Maximized )
					ExitMaxMinState();

				if( this.dockInfoNew.dController == null )
				{
					bool allowFloating = this.AllowFloating;
					if( this.ParentController is DockTabController )
						allowFloating = this.ParentController.AllowFloating;

					if( ( this.dockingMgr.DisallowFloating == true || !allowFloating )
						&& !this.dockingMgr.DragProvider.CanFloatWhenDisallowFloating )
						return;
					// If hosted as the first child of a floatingform - gradient is true, then invoke a move
					if( this.bHideCaption == true )
						this.MoveFloatToFloat();
					else
					{
						if( this.Floating == false )
							this.TransitDockToFloat();
						else
							this.TransitDockInFloatToFloat();
					}
				}
				else
				{
					if( this.Floating == true )
					{
						if( this.dockInfoNew.dController.Floating == true )
							this.TransitFloatToDockInFloat();
						else
							this.TransitFloatToDock();
					}
					else
					{
						if( this.dockInfoNew.dController.Floating == true )
							this.TransitDockToDockInFloat();
						else
							this.TransitDockToDock();
					}
				}
			}
			else
			{
				this.dockInfoNew.dController.InvokeDocking(this);
			}
			
			if(this.dockingMgr.DesignMode == true)
			{
				// Forcibly update the designer state
				this.dockingMgr.UpdateDesigner();
			}
			else
			{
				if( this.dockingMgr.DesignProcess == false )
				{
					// Make sure that focus is retained
					this.ctrlHost.Parent.Focus();
					if( ctrlfocus != null )
						ctrlfocus.Focus();
					else
						this.ctrlHost.Controls[0].Focus();
				}
				this.dockingMgr.ForbidFreeze = freeze;
			}
		}

		protected internal override DockControllerBase QueryController( string uniqueName )
		{
			if( this.UniqueName == uniqueName )
				return this;
			else
				return null;
		}

		public override void MoveController()
		{
			MoveFloat();
		}

		public override void DockAsMDIChild()
		{
			if( ParentController is DockTabController )
			{
				ParentController.DockAsMDIChild();
			}
			else
			{
				dockingMgr.SetAsMDIChild( HostControl.Controls[0], true );
			}
		}

		internal override void DockAsMDIChild(DockInfo dockInfo)
		{
			if (ParentController is DockTabController)
			{
				ParentController.DockAsMDIChild();
			}
			else
			{
				dockInfo.dController.DockingManager.SetAsMDIChild(HostControl.Controls[0], true);
			}
		}

		internal DockingManagerRenderer Renderer
		{
			get
			{
				return DockingManager.Renderer;
			}
		}

		internal VisualStyle RendererStyle
		{
			get { return DockingManager.VisualStyle;  }
		}

		public override void UpdateControl()
		{
			if( ParentController != null )
				ParentController.UpdateControl();
			if( Floating )
			{
				NativeMethods.SetWindowPos( ctrlHost.Handle,
					IntPtr.Zero, 0, 0, 0, 0, NativeMethods.SWP_FRAMECHANGED 
					| NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE
					| NativeMethods.SWP_NOZORDER );
			}
			else
			{
				ctrlHost.UpdateControlSize();
			}
			ctrlHost.Invalidate(true);
		}
	}

	
	///    The DockHost class implements a container for hosting a dockable control. Any control embedded in a 
	///    DockHost automatically avails of the docking frameworks functionality. The DockHost is responsible 
	///    for the frame, caption rect, docking/floating transitions etc., DockHosts interact with the 
	///    docking framework through the DockHostController class. DockHost classes are never directly exposed 
	///    to the users. Users are oblivious of the existence of the dockhost class and all programmatic interaction 
	///    occurs through the DockingManager instance. Calling DockControl/FloatControl on the dockingmanager will create a dockhost for the
	///    particular control and lay it out as per the configuration. 	
	[
	ToolboxItem(false),
	DesignTimeVisible(false)
	]
	[Syncfusion.Documentation.DocumentationExclude()]	
	public class DockHost : System.Windows.Forms.ContainerControl
		, IDraggable
		, ITabFeedback
		, IDockingManagerDesignerMouseHook
		, IDockable
	{
		protected Rectangle rcDrag;
		protected internal CaptionPainter cpTitleBar; 
		protected DockHostController dcInternal;
		protected int nImageIndex = -1;
		protected internal Icon controlImage = null;
		protected internal static Pen pen;
		private Control m_lastActiveControl = null;
        //Newly Added
        protected int mImageIndex = -1;

        protected internal int BorderWidth
        {
            get
            {
                if (dcInternal.DockingManager.VisualStyle == VisualStyle.Default)
                {
                    return cpTitleBar.BorderWidth;
                }
                else
                {
                    return dcInternal.Renderer.BorderWidth;
                }
            }
        }

        protected internal int CaptionHeight
        {
            get
            {
                if (dcInternal.DockingManager.VisualStyle == VisualStyle.Default)
                {
                    return cpTitleBar.CaptionRect.Height 
                        + cpTitleBar.CaptionTopIndent + 1/*Caption bottom indent*/;
                }
                else
                {
                    return dcInternal.Renderer.CaptionWidth;
                }
            }
        }
				
		public new Rectangle ClientRectangle
		{
			get
			{
				if( dcInternal.RendererStyle == VisualStyle.Default )
				{
					Rectangle rcbase = base.ClientRectangle;
					if(this.dcInternal.HideCaption == true)
						return new Rectangle(1, 1, rcbase.Width-2, rcbase.Height-2);
					else
						return new Rectangle(1, 2+CaptionPainter.CaptionHeight+2, rcbase.Width-2, rcbase.Height-(2+CaptionPainter.CaptionHeight+3));
				}
				else
				{
				
					Rectangle rectangle = base.ClientRectangle;
					if( dcInternal.HideCaption )
					{
						rectangle.X = dcInternal.Renderer.ThinBorderWidth;
						rectangle.Y = dcInternal.Renderer.ThinBorderWidth;
						rectangle.Width -= 2 * dcInternal.Renderer.ThinBorderWidth;
						rectangle.Height -= 2 * dcInternal.Renderer.ThinBorderWidth;
					}
					else
					{
						rectangle.X = dcInternal.Renderer.ThinBorderWidth;
						rectangle.Y = dcInternal.Renderer.ThinBorderWidth + dcInternal.Renderer.CaptionWidth;
						rectangle.Width -= 2 * dcInternal.Renderer.ThinBorderWidth;
						rectangle.Height -= 2 * dcInternal.Renderer.ThinBorderWidth + dcInternal.Renderer.CaptionWidth;
					}
					return rectangle;
				}
			}
		}

		public DockControllerBase InternalController
		{
			get { return this.dcInternal; }
		}

		public int ImageIndex
		{
			get { return this.nImageIndex; } 
			set { this.nImageIndex = value;	}
		}

        //Newly Added
        public int MdiImageIndex
        {
            get { return this.mImageIndex; }
            set { this.mImageIndex = value; }
        }

		
		public DockInfo DragDockInfo
		{
			get { return this.dcInternal.DINew; }
			set { this.dcInternal.DINew = value; }
		}
		
		public Rectangle DragRectangle
		{
			get
			{
				// In design mode, always return the control bounds.				
				if((this.rcDrag.Width<=0)||(this.rcDrag.Height<=0) == true)
				{
					Point ptfloat = PointToScreen(this.Location);
					this.rcDrag = new Rectangle(ptfloat.X, ptfloat.Y, this.Bounds.Width, this.Bounds.Height);
				}
				if((this.dcInternal.MinimumSize.Width != 0) && (this.rcDrag.Width < this.dcInternal.MinimumSize.Width))
					this.rcDrag.Width = this.dcInternal.MinimumSize.Width;				
				if((this.dcInternal.MinimumSize.Height != 0) && (this.rcDrag.Height < this.dcInternal.MinimumSize.Height))
					this.rcDrag.Height = this.dcInternal.MinimumSize.Height;

				return this.rcDrag;
			}

			set { this.rcDrag = value; }
		}		

		public CaptionPainter TitleBar
		{
			get { return this.cpTitleBar; }
		}

		internal Control LastActiveControl
		{
			get { return this.m_lastActiveControl; }
			set { this.m_lastActiveControl = value; }
		}

		public DockHost(DockingManager dmgr, Control ctrl)
		{			
			this.InitializeDockHost(dmgr, ctrl);
			if(ctrl.Name != String.Empty)
				this.Name = String.Concat("DockHost_", ctrl.Name);

            if (!dmgr.DesignProcess && ctrl.Controls.Count > 0)
            {
                Control control = ctrl.Controls[0];
                if (control != null)
                {
                    control.HandleDestroyed += new EventHandler(HostControl_HandleDestroyed);
                    control.HandleCreated += new EventHandler(HostControl_HandleCreated);
                }
            }

			DockHostController dockHostController = this.InternalController as DockHostController;

			if( dockHostController != null )
			{
				DockingManager dockManager = dockHostController.DockingManager;
				dockHostController.UniqueName = NameGenerator.Generate(ctrl);
			}
		}

		protected void HostControl_HandleCreated( object sender, EventArgs e )
		{
			this.Enabled = true;
		}

		protected void HostControl_HandleDestroyed( object sender, EventArgs e )
		{
			this.Enabled = false;
		}
		
		internal protected static void UpdatePenColor( Color newColor )
		{
			if ( pen != null ) 
				pen.Dispose();
			pen = new Pen( newColor );
		}
		public virtual void InitializeDockHost(DockingManager dmgr, Control ctrl)
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserMouse, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			if( ctrl.Controls.Count > 0 && ctrl is ScrollableControl )
			{
				foreach( Control inner_ctrl in ctrl.Controls )
				{
					if( inner_ctrl.GetType().FullName.Equals("Syncfusion.Windows.Forms.Grid.GridControl") )
						this.SetStyle( ControlStyles.Selectable, false );
				}
			}
			else if( ctrl.GetType().FullName.Equals("Syncfusion.Windows.Forms.Grid.GridControl"))
				this.SetStyle( ControlStyles.Selectable, false );
			
			this.dcInternal = this.CreateDockHostController(dmgr); 
			dmgr.AddController(this.dcInternal);
			this.cpTitleBar = this.CreateCaptionPainter();
			this.dcInternal.AutoHideButtonVisibility = dmgr.AutoHideEnabled;
			this.dcInternal.CloseButtonVisibility = dmgr.CloseEnabled;
			this.dcInternal.MenuButtonVisiblity = dmgr.MenuButtonEnabled;
			this.dcInternal.MaximizeButtonVisibility = dmgr.MaximizeButtonEnabled;

			ctrl.Anchor = AnchorStyles.Left|AnchorStyles.Top;
			ctrl.Dock = DockStyle.None;

			this.Size = dcInternal.DockingManager.ToDockHostSize( ctrl.Size );
			this.Text = ctrl.Name;
			this.Visible = true;
			this.Controls.Add(ctrl);
			
			SubscribeDockHostEvents();
		}

		protected virtual DockHostController CreateDockHostController(DockingManager dmgr)
		{
			return new DockHostController(dmgr, this);
		}

		protected virtual CaptionPainter CreateCaptionPainter()
		{
			return new CaptionPainter(this.dcInternal);
		}

		protected internal void RefreshCaptionPainter()
		{
			cpTitleBar.LabelAlignment = dcInternal.DockingManager.DockLabelAlignment;
			this.Invalidate(true);
		}

		private void OnChildTabStopChanged( object sender, EventArgs arg )
		{
			Control ctrl = sender as Control;
			this.TabStop = ctrl.TabStop;
		}

		protected override void OnControlAdded(ControlEventArgs e)
		{
			base.OnControlAdded(e);

			if(this.dcInternal.DockingManager.DesignProcess == false)
				this.RecSubscribeChildControlEvents(e.Control, true);
			e.Control.TabStopChanged += new EventHandler( this.OnChildTabStopChanged );
			this.TabStop = e.Control.TabStop;
		}

        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
        }

		protected override void OnControlRemoved(ControlEventArgs e)
		{
			base.OnControlRemoved(e);

			if(null != this.dcInternal && this.dcInternal.DockingManager.DesignProcess == false)
				this.RecSubscribeChildControlEvents(e.Control, false);
			e.Control.TabStopChanged -= new EventHandler( this.OnChildTabStopChanged );
		}

		public void RecSubscribeChildControlEvents(Control ctrl, bool subscribe)
		{
			if(subscribe == true)
			{
				ctrl.GotFocus += new System.EventHandler(this.dhclient_GotFocus);
				ctrl.LostFocus += new System.EventHandler(this.dhclient_LostFocus);
                ctrl.Validating += new CancelEventHandler(this.dhclient_Validating);
				ctrl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dhcclient_MouseDown);
				ctrl.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.dhcclient_ControlAdded);
				ctrl.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.dhcclient_ControlRemoved);
			}
			else
			{
				ctrl.GotFocus -= new System.EventHandler(this.dhclient_GotFocus);
				ctrl.LostFocus -= new System.EventHandler(this.dhclient_LostFocus);
                ctrl.Validating -= new CancelEventHandler(this.dhclient_Validating);
				ctrl.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.dhcclient_MouseDown);
				ctrl.ControlAdded -= new System.Windows.Forms.ControlEventHandler(this.dhcclient_ControlAdded);
				ctrl.ControlRemoved -= new System.Windows.Forms.ControlEventHandler(this.dhcclient_ControlRemoved);

				if (this.dcInternal!=null && this.dcInternal.DockingManager.ctrlLastPainted == this)
					dcInternal.DockingManager.ctrlLastPainted = null;
			}
			if(ctrl.Controls.Count > 0)
			{
				IEnumerator iechildren = ctrl.Controls.GetEnumerator();			
				while(iechildren.MoveNext() == true)
				{
					RecSubscribeChildControlEvents(iechildren.Current as Control, subscribe);
				}
			}
		}

        protected void dhclient_Validating(object sender, CancelEventArgs e)
        {
            if (e.Cancel == true)
            {   
                this.InternalController.DockingManager.ValidatingCancelledControl = sender as Control;
            }
        }
		protected void DockHost_MouseLeave(Object sender, EventArgs e)
		{
			// Notify and repaint the captionbar when mouse leaves DockHost's bounds.
			if( dcInternal.DockingManager.VisualStyle == VisualStyle.Default )
				this.cpTitleBar.HitTest(MouseAction.MouseLeave, Point.Empty);
			else
			{
				this.dcInternal.Renderer.HitTest( MouseButtons.None, Point.Empty );
				Invalidate( dcInternal.Renderer.CaptionBounds, true );
			}
		}

		private void DockHost_MouseEnter(object sender, EventArgs e)
		{
			// Notify and repaint the captionbar when mouse enters DockHost's bounds.
			if( dcInternal.DockingManager.VisualStyle == VisualStyle.Default )
				this.cpTitleBar.HitTest(MouseAction.MouseMove, Point.Empty );
			else
			{
				this.dcInternal.Renderer.HitTest(MouseButtons.None, Point.Empty );
				Invalidate( dcInternal.Renderer.CaptionBounds, true );
			}
		}

		protected void this_ParentChanged(Object sender, EventArgs e)
		{
			// If this dockhost is serving as a hostcontroller for a tab and the parent property has changed,
			// then notify the tab
			if( (this.dcInternal.DockTab != null) && (this.Parent != null) )
				this.dcInternal.DockTab.ReparentTabChildren();
		}

		// Whenever the focus changes, repaint the caption rect
		protected void dhclient_GotFocus(Object sender, EventArgs e)
		{
            //Fix for issue #13051  "|| this.Contains(sender as Control)" added
            if (sender == this.Controls[0] || this.Contains(sender as Control))
			{
				if( !dcInternal.DockingManager.StopActivationEvents )
				{
					if( this.dcInternal.DockingManager.DHCInFocus != this.dcInternal )
					{
						this.dcInternal.DockingManager.mouseActivatedControl = null;
						this.dcInternal.DockingManager.DHCInFocus = this.dcInternal;
					}
				}
			}
			InvalidateCaption();
		}

		protected void dhclient_LostFocus(Object sender, EventArgs e)
		{
            Control ctrlFocus = Control.FromHandle(Syncfusion.Runtime.InteropServices.NativeMethods.GetFocus());

            Control ctrl = sender as Control;

            if (ctrlFocus is DockHost)
            {
                try
                {
                    (ctrlFocus as DockHost).Controls[0].Focus();
                }
                catch { }
            }
            if (this.InternalController.DockingManager.ValidatingCancelledControl != null
                && this.InternalController.DockingManager.ValidatingCancelledControl == sender as Control)
            {

                this.InternalController.DockingManager.ValidatingCancelledControl = null;
                ctrl.Focus();
                InvalidateCaption();
            }
            else if (this.ContainsFocus == false)
            {
                if (!dcInternal.DockingManager.StopActivationEvents)
                {
                    if (this.dcInternal.DockingManager.DHCInFocus == this.dcInternal)
                        this.dcInternal.DockingManager.DHCInFocus = null;
                }

                this.LastActiveControl = this.ActiveControl;
                InvalidateCaption();
            }
		}
		
		protected void dhcclient_MouseDown(Object sender, MouseEventArgs e)
		{		
			Control ctrl = sender as Control;
            if (this.Controls.Count > 0 && ctrl != null && !this.Controls[0].ContainsFocus && !(this.Controls[0] is ScrollableControl) &&
			( ( ctrl.CanSelect == false ) || ( ctrl.CanFocus == false ) ) )
				this.Controls[0].Focus();
                
            if (this.Controls.Count > 0 && this.dcInternal.DockingManager.DHCInFocus != this.dcInternal && this.Controls[0].Focused)
                this.dcInternal.DockingManager.DHCInFocus = this.dcInternal;
			
			InvalidateCaption();
		}
	
		internal void InvalidateCaption()
		{
			if( dcInternal.RendererStyle == VisualStyle.Default )
			{
				this.Invalidate(this.TitleBar.CaptionRect, true);
			}
			else
			{
				int borderWidth = dcInternal.Renderer.ThinBorderWidth;
				int captionWidth = dcInternal.Renderer.CaptionWidth;
				Rectangle rectangle = new Rectangle( borderWidth, borderWidth,
					Width - 2 * borderWidth, /*Height - 2 * borderWidth - captionWidth*/ dcInternal.Renderer.CaptionWidth );
				this.Invalidate( rectangle, true );
			}
		}

		protected void dhcclient_ControlAdded(Object sender, ControlEventArgs e)
		{
			this.RecSubscribeChildControlEvents(e.Control, true);
		}
		
		protected void dhcclient_ControlRemoved(Object sender, ControlEventArgs e)
		{
			this.RecSubscribeChildControlEvents(e.Control, false);
		}

		// Implementation of the IDraggable interface methods
		public bool IsSuitableDockTarget(DockControllerBase dc)
		{
			bool suitable = true;
			DockStateControllerBase baseCtrl = dc as DockStateControllerBase;
			if( baseCtrl != null && baseCtrl.AutoHideMode )
				suitable = false;
			// Return false if the target dc is the same as this control's dc or if the target dc 
			// is this control's parent controller, such as in a floating form case.
			// Also, no docking is allowed if the alwaysfloating member is set.
			if(dc.Equals(this.dcInternal) || this.dcInternal.FloatOnly || 
				(this.dcInternal.HideCaption && this.Parent.Equals(dc.HostControl)) ||
				dc is DragSplitterController)
				suitable = false;
			return suitable;
		}

		public bool InitiateDrag(MouseAction action, Point ptscreen)
		{
			return ! dcInternal.AutoHideMode;
		}

		public DragAxis AllowedDragAxis(ref Point ptdrag, Point ptdelta)		
		{
			return DragAxis.XY;
		}

		public bool DrawHollow()
		{
			return true;
		}
		
		public void AbortDrag()
		{
			if(this.dcInternal.DockingManager.DragProvider.DraggingControl != this)
			{
				Debug.Assert(false, "Invalid call");
				return;
			}

			// breaddtab will be set to true only if this DockHost was being dragged out of a DockTabController.
			bool breaddtab = false;
			if(this.dcInternal.DockTab != null)
			{
				breaddtab = true;
				foreach(DockTabPage page in this.dcInternal.DockTab.TabPages)
				{
					if(page.dhcClient == this.dcInternal)
					{
						breaddtab = false;
						break;
					}
				}
			}			
			if(breaddtab == false)
			{
				if(this.Capture == true)
					this.Capture = false;
			}
			else
			{
				// Readd the dockhost to the tabcontrol
				if(this.dcInternal.DockTab.Capture == true)
					this.dcInternal.DockTab.Capture = false;
				DockTabController tabcontroller = this.dcInternal.DockTab.InternalController as DockTabController;
				tabcontroller.bAllowDrag = false;
				tabcontroller.dragTabPage = null;
				DockTabPage tabpage = new DockTabPage(this.dcInternal, this.Text, this.ImageIndex);
				this.dcInternal.DockTab.TabPages.Add(tabpage);
				tabcontroller.PauseActivation = true;
				this.dcInternal.DockTab.SelectedTab = tabpage;
				tabcontroller.PauseActivation = false;
			}
			this.dcInternal.DockingManager.DragProvider.ForceStopDrag();
			// When dragging wasn't already terminated then terminate it.
			if (dcInternal.DockingManager.DragProvider.DraggingControl != null)
			{
				this.dcInternal.DockingManager.DragProvider.TerminateDrag(this, Point.Empty);
			}
			this.dcInternal.DINew = DockInfo.NullInfo;

			if(this.dcInternal.DockingManager.DesignMode == true)
				this.dcInternal.DockingManager.UpdateDesigner();
		}

		public bool QueryDragProceedWithDock()
		{
			// If the suggested dock rect is beyond the min/max extents, do not allow the dock operation
			if((this.dcInternal.MinimumSize.Width != 0) && (this.DragDockInfo.rcDockArea.Width < this.dcInternal.MinimumSize.Width))
				return false;
			if((this.dcInternal.MinimumSize.Height != 0) && (this.DragDockInfo.rcDockArea.Height < this.dcInternal.MinimumSize.Height))
				return false;			
			return true;
		}

		// ITabFeedback methods
		public bool ProvideTabFeedback(IDraggable idg, MouseAction action)
		{
			if(this.dcInternal.DockTab == null)
				return false;	

			// The selection index is changed while providing drag feedback. However, the tab controllers' state 
			// should remain unchanged. This is accomplished by temporarily pausing controller activation.
			DockTabController dtc = this.dcInternal.DockTab.InternalController as DockTabController;
			dtc.PauseActivation = true;

			DockControllerBase parent = idg.InternalController;
			if(idg.InternalController.ChildCount == -1)
			{
				// The controller being dragged is a dockhostcontroller. If this has a docktab, then use the tabcontroller
				DockHostController dhc = idg.InternalController as DockHostController;
				if( dhc.DockTab != null ) 
				{
					foreach(DockTabPage page in dhc.DockTab.TabPages)
					{
						if(page.dhcClient == dhc)
						{	
							parent = idg.InternalController.ParentController;
							break;
						}
					}
				}
			}
			// Perform a recursive iteration of the controller and provide tabs for all it's children
			ArrayList childlist = new ArrayList();
			DockUtilities.RecGetChildControllers(parent, childlist);
		
			// If tab(s) already exists for the dockhost seeking feedback, then ProvideTabFeedback() is being called
			// to remove or reposition the tab 
			Point ptscreen = Cursor.Position;
			Point ptclient = this.dcInternal.DockTab.PointToClient(ptscreen);
			int captionht = SystemInformation.ToolWindowCaptionHeight+4;
			Rectangle rccaption;
			if(this.dcInternal.HideCaption == false)
				rccaption = new Rectangle(0, 0, this.Width, captionht);
			else
				rccaption = new Rectangle(0,  0-captionht, this.Width, captionht);
			if( (idg.DragDockInfo.nDockIndex < 0) || (action == MouseAction.LBtnUp) || 
				action == MouseAction.MouseLeave )
			{	
				// The cursor has either moved out of the tabcontrol or the drag is over, remove all empty tabs
				Array pages = this.dcInternal.DockTab.TabPages.ToArray(typeof(DockTabPage));			
				foreach(DockTabPage page in pages)
				{
					if(page.dhcClient == null)
						this.dcInternal.DockTab.TabPages.Remove(page);
				}
				if(action != MouseAction.LBtnUp)
					idg.DragDockInfo.nDockIndex = -1;	// Reset dockindex
				if(pages.Length > this.dcInternal.DockTab.TabCount)	// Tabs have been removed
				{
					foreach(DockTabPage thispage in this.dcInternal.DockTab.TabPages)
					{
						if( thispage.dhcClient.Equals(this.dcInternal) )
						{
							// Set the tab for the current controller as the selected tab
							this.dcInternal.DockTab.SelectedTab = thispage;
							break;
						}
					}									
				}
				if(this.dcInternal.DockTab.nSwitchIndex != -1)
					this.dcInternal.DockTab.nSwitchIndex = -1;
			}
			else	// Cursor position has changed; move the empty tabs to the new insertion point
			{				
				bool bexists = false;				
				// Switch the selected tab with the new hitindex
				Array pages = this.dcInternal.DockTab.TabPages.ToArray(typeof(DockTabPage));			
				foreach(DockTabPage insertpage in pages)
				{
					if(insertpage.dhcClient == null)
					{
						bexists = true;

						TabPageAdv tempPage = null;
						DockTabControl tabControl = dcInternal.DockTab;
						TabPageAdv selectedPage = tabControl.SelectedTab;

						// firstIndex - current position of target control's first pages
						// index - target control's first page position after moving
						int firstIndex = tabControl.TabPages.IndexOf( insertpage );
						int index = tabControl.GetTabHitIndex( ptclient );
						int targetPagesCount = tabControl.TabCount;
						int sourcePagesCount = 1;
						if( parent is DockTabController )
						{
							DockTabController tabController = parent as DockTabController;
							sourcePagesCount = tabController.TabControl.TabPages.Count;
						}

						if( index == -1 )
						{
							if( ptclient.X < 10 )
								index = 0;
							else
								index = targetPagesCount;
						}

						// calculate index 
						if( index > (targetPagesCount - sourcePagesCount) )
							index = targetPagesCount - sourcePagesCount;

						if( firstIndex != index )
						{
							// move pages
							if( firstIndex > index )
							{
							
								for( int i = firstIndex; i < firstIndex + sourcePagesCount; i++ )
								{
									tempPage = tabControl.TabPages[ i ];
									tabControl.TabPages.RemoveAt( i );
									tabControl.TabPages.Insert( index + i - firstIndex , tempPage );
								}
							}
							else
							{
								for( int i = firstIndex; i < index ; i++ )
								{
									tempPage = tabControl.TabPages[ i + sourcePagesCount ];
									tabControl.TabPages.RemoveAt( i + sourcePagesCount );
									tabControl.TabPages.Insert( i, tempPage );
								}
							}

							// restoring selected page
							tabControl.SelectedTab = selectedPage;
						}
						break;
					}
				}				
				// If the tab is not already present, then insert it at the new position
				if(bexists == false)
				{
					int i = idg.DragDockInfo.nDockIndex;
					foreach(DockHostController child in childlist)
						this.dcInternal.DockTab.InsertTab( i++, new DockTabPage(null, child.HostControl.Text, child.ImageIndex) );

					// Get hold of the dockhost that has the focus
					String strfocus = null;
					foreach(DockHostController host in childlist)
					{
						if(host.HostControl.ContainsFocus)
						{
							strfocus = host.HostControl.Text;
							break;
						}
					}			

					if(strfocus != null)	// Make the most recent insertion as the selected tab						
					{
						foreach(DockTabPage page in this.dcInternal.DockTab.TabPages)
						{
							if(page.Text == strfocus)
							{
								this.dcInternal.DockTab.SelectedTab = page;
								break;
							}
						}
					}
				}
			}
			dtc.PauseActivation = false;
			this.dcInternal.DockTab.Update();
			return true;
		}
		
		private CaptionButtonState GetActiveButtonState()
		{
			return ( Control.MouseButtons | MouseButtons.Left ) == Control.MouseButtons	 ?
				CaptionButtonState.Pushed : CaptionButtonState.Active;
		}
		
		protected override void OnPaint(PaintEventArgs e)
		{
            if (this.dcInternal != null)
            {
                this.dcInternal.CheckCaptionVisibility();
                if (dcInternal.RendererStyle == VisualStyle.Default)
                {
                    Rectangle rcborder = Rectangle.Inflate(this.ClientRectangle,
                        cpTitleBar.BorderWidth, cpTitleBar.BorderWidth);
                    int tabHeight = this.dcInternal.DockingManager.DockTabHeight;
                    if (this.dcInternal.DockTab != null)
                    {
                        DockTabController parentDtc = this.dcInternal.ParentController as DockTabController;
                        rcborder = parentDtc.GetHostControlBounds();
                        rcborder.Inflate(1, 1);
                    }

                    this.cpTitleBar.RefreshCaptionButtonsCollection();
                    if (dcInternal.DockingManager.ctrlLastPainted != this)
                        cpTitleBar.ResetCaptonButtonsHitTest();

                    if (dcInternal.DockingManager.PaintBorders)
                        this.cpTitleBar.PaintCaption(e.Graphics, pen);
                    else
                    {   // Skip caption painting when DockHost is single under FloatingForm.
                        if (!(dcInternal.Floating && dcInternal.ParentController is FloatingFormController))
                            this.cpTitleBar.PaintCaption(e.Graphics, new Pen(SystemColors.ControlDark));
                    }

                    if (dcInternal.DockingManager.PaintBorders)
                    {
                        Rectangle rcDraw = new Rectangle(rcborder.Left, rcborder.Top, rcborder.Width - 1, rcborder.Height - 1);
                        e.Graphics.DrawRectangle(pen, rcDraw);
                    }
                }
                else
                {
                    PaintDockControlArgs args = this.GetPaintInfo();

                    if (!this.dcInternal.HideCaption)
                        this.dcInternal.DockingManager.FireProvideGraphicsItemsEvent(args.ProvideGraphicsItemsArgs);

                    if (args != null)
                    {
                        if (dcInternal.DockingManager.ctrlLastPainted != this)
                            dcInternal.Renderer.ResetButtonsHitTest();

                        dcInternal.Renderer.PaintDockedControl(e.Graphics,
                            new Rectangle(Point.Empty, Size), args);
                    }
                }
                dcInternal.DockingManager.ctrlLastPainted = this;
            }
			base.OnPaint(e);
		}

        private PaintDockControlArgs GetPaintInfo()
        {
            PaintDockControlArgs args = null;

            CaptionState captionState = cpTitleBar.DrawWithHighlight() ?
                    CaptionState.Active : CaptionState.Normal;

            CaptionButtonOptionsTable table = new CaptionButtonOptionsTable();
            int index = 0;
            CaptionButtonOptions options = null;
            ImageList imageList = dcInternal.DockingManager.ImageList;
            CaptionButtonsCollection captionButtons = dcInternal.DockingManager.GetCustomCaptionButtons(Controls[0]);
			if( captionButtons == null && dcInternal.DockingManager.TargetManagers.Count > 0 )
			{
				foreach( DockingManager dm in dcInternal.DockingManager.TargetManagers )
				{
					captionButtons = dm.GetCustomCaptionButtons( Controls[0] );
					if( captionButtons != null )
						break;
				}
			}
			if( captionButtons != null )
			{
				bool bRestoreButtonExists = dcInternal.DockingManager.CaptionButtons.ContainsButtonType( CaptionButtonType.Restore );
				for( int i = 0; i < captionButtons.Count; i++ )
				{
					CaptionButton button = captionButtons[i];
					options = new CaptionButtonOptions();
					switch( button.Type )
					{
						case CaptionButtonType.Close:
							if( dcInternal.CloseButtonVisibility )
							{
								table.Add( button, options );
								index++;
							}
							break;
						case CaptionButtonType.Pin:
							if( dcInternal.AutoHideButtonVisibility && !dcInternal.Floating )
							{
								options.ModifiedView = dcInternal.AutoHideMode;
								table.Add( button, options );
								index++;
							}
							break;
						case CaptionButtonType.Menu:
							if( dcInternal.MenuButtonVisiblity )
							{
								table.Add( button, options );
								index++;
							}
							break;
						case CaptionButtonType.Maximize:
							if( dcInternal.MaximizeButtonVisibility )
							{
								options.ModifiedView = dcInternal.Maximized;
								if( !bRestoreButtonExists || ( bRestoreButtonExists && !dcInternal.Maximized ) )
								{
									table.Add( button, options );
									index++;
								}
							}
							break;
						case CaptionButtonType.Restore:
							if( dcInternal.MaximizeButtonVisibility )
							{
								options.ModifiedView = !dcInternal.Maximized;
								if( bRestoreButtonExists && dcInternal.Maximized )
								{
									table.Add( button, options );
									index++;
								}
							}
							break;
						case CaptionButtonType.Custom:
							table.Add( captionButtons[i], options );
							index++;
							break;
					}
				}
			}

            dcInternal.Renderer.ControlBounds = new Rectangle(Point.Empty, Size);
            Caption caption = dcInternal.HideCaption ?
                null :
                new Caption(captionState, dcInternal.DockLabel,
                dcInternal.DockingManager.DockLabelAlignment,
                dcInternal.DockingManager.CaptionTextFont);

            //Fix for Designer Issue when a Docked Window is deleted.

			if( Controls.Count > 0 )
			{
				ProvideGraphicsItemsEventArgs pgargs = new ProvideGraphicsItemsEventArgs(
					Controls[0],
					dcInternal.DockingManager.Renderer.CaptionBounds,
					captionState == CaptionState.Active );

				int imageIndex = ( dcInternal.DockingManager.ShowCaptionImages ) ? dcInternal.ImageIndex : -1;

				args = new PaintDockControlArgs(
					caption, table, dcInternal.DockingManager.PaintBorders, false, imageIndex, pgargs, imageList );
				args.DesignMode = dcInternal.DockingManager.DesignProcess;
			}

            return args;
        }

        internal void RefreshRenderer()
        {
            dcInternal.Renderer.RefreshPaintInfo( new Rectangle( Point.Empty, Size), GetPaintInfo() );
            dcInternal.DockingManager.ctrlLastPainted = this;
        }

		// Implementation of IDesignerMouseHook
		public void HandleMouseDown(MouseButtons button, Point ptscreen)
		{
			this.dcInternal.HandleMouseDownImp(button, this.PointToClient(ptscreen));
		}	

		public void HandleMouseMove(MouseButtons button, Point ptscreen)
		{			
			this.dcInternal.HandleMouseMoveImp(button, this.PointToClient(ptscreen));
		}

		public void HandleMouseUp(MouseButtons button, Point ptscreen)
		{			
			this.dcInternal.HandleMouseUpImp(button, this.PointToClient(ptscreen));
			
			// Reset the hostcontrol's client as the selected control.
			Control selctrl = this.dcInternal.DockingManager.GetPrimarySelection() as Control;
			Control dockctrl = this.Controls[0];
			if( (selctrl == null) || (selctrl.Equals(dockctrl) == false) )
			{
				Control[] ctrlarray = new Control[] { dockctrl };
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				this.dcInternal.DockingManager.SetSelectedComponents(ctrlarray, SelectionTypes.Auto);
#else
				this.dcInternal.DockingManager.SetSelectedComponents(ctrlarray, SelectionTypes.MouseDown);
#endif
			}
			dockctrl.Focus();
		}

		public void HandleDoubleClick(Point ptscreen)
		{
			this.dcInternal.HandleDoubleClickImp(this.PointToClient(ptscreen));
		}

		public void HandleMouseLeave()
		{
			// No implementation
		}

		public void InitiateFloatingResize(Point ptscreen, int nchittest)
		{
			Debug.Assert(false, "No implementation in DockHost.");			
		}
		
		public bool GetDesignMode()
		{
			return this.dcInternal.DockingManager.DesignMode;
		}

		#region Overrides
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if ((this.dcInternal != null) && (this.dcInternal.DockingManager.DesignProcess == false))
			{
				DockingManager hostManager = this.dcInternal.DockingManager;

                if( hostManager.VisualStyle != VisualStyle.Default && hostManager.ctrlLastPainted != this )
                    RefreshRenderer();

				if (dcInternal.DockingManager.DHCInFocus != this.dcInternal)
				{
					dcInternal.DockingManager.DHCInFocus = this.dcInternal;
				}
				if (e.Button == MouseButtons.Left)
				{
					if (this.dcInternal.bAutoHideSizing == false)
						this.dcInternal.HandleMouseDownImp(e.Button, new Point(e.X, e.Y));
				}
				if(!this.ContainsFocus)
				{
					if(this.Controls[0].Controls.Count > 0)
					{
						if(this.LastActiveControl != null)
						{
							this.LastActiveControl.Focus();
							this.LastActiveControl = null;
						}
					}
					else
						this.Controls[0].Focus();
				}
				this.InvalidateCaption();
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if (this.dcInternal.bAutoHideSizing == false)
			{
				IDraggable idg = this.dcInternal.DockingManager.DragProvider.DraggingControl;
				if ((idg == null) || ((idg != null) && (idg == this)))
				{
					this.dcInternal.HandleMouseUpImp(e.Button, new Point(e.X, e.Y));
				}
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
            if ((this.dcInternal != null) && (this.dcInternal.DockingManager.DesignProcess == false))
            {
                IDraggable idg = this.dcInternal.DockingManager.DragProvider.DraggingControl;
                if (idg == null)
                    this.dcInternal.HandleMouseMoveImp(e.Button, new Point(e.X, e.Y));
                else if (idg == this)
                    this.dcInternal.HandleMouseMoveImp(e.Button, new Point(e.X, e.Y));
            }

			cpTitleBar.ProcessMouseMove(e);
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
            if (this.Controls.Count > 0)
            {
                for (int i = 0; i < this.Controls.Count; i++)
                {
                    if (this.Visible)
                        this.Controls[i].Visible = this.Visible;
                }
            }

			if ((this.Visible == true) && (this.dcInternal != null) &&
				(this.dcInternal.DockingManager.bLoadVisibility == false))
				this.Visible = false;
			base.OnVisibleChanged(e);
		}

		protected override void WndProc(ref Message msg)
		{
            if (msg.Msg == NativeMethods.WM_LBUTTONDOWN || msg.Msg == NativeMethods.WM_LBUTTONDBLCLK)
            {
                int x = NativeMethods.LOWORD(msg.LParam);
                int y = NativeMethods.HIWORD(msg.LParam);
                Control toFocus = null;
                DockHostController toFocus_dhc = this.dcInternal;
                if ((this.Controls.Count > 0) && (this.Controls[0].ContainsFocus == false))
                {
                    toFocus = this.Controls[0];
                    Point m_point = new Point(x, y);
                    if (!(toFocus.Bounds.Contains(m_point) || this.cpTitleBar.CaptionRect.Contains(m_point)))
                    {
                        return;
                    }
                }
            }
			if ((((msg.Msg == 0x0201 /*WM_LBUTTONDOWN*/) || (msg.Msg == 0x0202 /*WM_LBUTTONUP*/))
				&& (((int)msg.WParam == 0x0020 /*MK_XBUTTON1*/) || ((int)msg.WParam == 0x0040 /*MK_XBUTTON2*/)))
				|| ((msg.Msg == 0x020B /*WM_XBUTTONDOWN*/) || (msg.Msg == 0x020C /*WM_XBUTTONUP*/)))
			{
				if ((this.dcInternal != null) && (this.dcInternal.DockingManager.DesignProcess == false) &&
					(this.dcInternal.DockingManager.DragProvider.DraggingControl != null))
					return;
			}

			if ((msg.Msg == 0x0007 /*WM_SETFOCUS*/) && (this.dcInternal != null) && (this.dcInternal.DockingManager.DesignProcess  == false) ||
                (msg.Msg == NativeMethods.WM_PARENTNOTIFY) && (NativeMethods.LOWORD(msg.WParam) == NativeMethods.WM_LBUTTONDOWN))
			{
				this.InternalController.DockingManager.mouseActivatedControl = null;

				Control toFocus = null;
                DockHostController toFocus_dhc = this.dcInternal;
				if ((this.Controls.Count > 0) && (this.Controls[0].ContainsFocus == false))
					toFocus = this.Controls[0];
				if( msg.Msg == NativeMethods.WM_SETFOCUS )
				{
					Control focusLost = Control.FromHandle( msg.WParam );
                    //Fix for issue #13051
                    if (focusLost is Form && !(focusLost is FloatingForm) && this.dcInternal.DockingManager.LastActiveControl != null)
                    {
                        toFocus = this.dcInternal.DockingManager.LastActiveControl;
                        Control parent = toFocus.Parent;

                        while (parent != null)
                        {
                            if (parent is DockHost)
                            {
                                toFocus_dhc = (parent as DockHost).dcInternal;
                                break;
                            }
                            else
                                parent = parent.Parent;
                        }
                    }
					else if( focusLost != null )
					{
						Control parent = focusLost.Parent;
						while( parent != null )
						{
							if( parent == this )
								break;
							parent = parent.Parent;
						}

						if( parent != null )
							toFocus = focusLost;
					}
				}

                bool allowFocus = true;

                if ((msg.Msg == NativeMethods.WM_PARENTNOTIFY) && (NativeMethods.LOWORD(msg.WParam) == NativeMethods.WM_LBUTTONDOWN))
                {
                    int x = NativeMethods.LOWORD(msg.LParam);
                    int y = NativeMethods.HIWORD(msg.LParam);
                    Point m_point = new Point(x, y);
                    if (toFocus != null && !toFocus.Bounds.Contains(m_point))
                        allowFocus = false;
                }
                else if(toFocus != null)
                {
                    toFocus.Focus();
                }

                if (toFocus != null && !(toFocus is ScrollableControl) && allowFocus)
                    toFocus.Focus();
                
                if (this.dcInternal.DockingManager.DHCInFocus != toFocus_dhc)
                    this.dcInternal.DockingManager.DHCInFocus = toFocus_dhc;
				InvalidateCaption();
				return;
			}

			if ((msg.Msg == 0x0008 /*WM_KILLFOCUS*/) && (this.dcInternal != null) && (this.dcInternal.DockingManager.DesignProcess == false))
			{
				if (this.ContainsFocus == false)
				{
					if (this.dcInternal.DockingManager.DHCInFocus == this.dcInternal)
						this.dcInternal.DockingManager.DHCInFocus = null;
					InvalidateCaption();
					return;
				}
			}

			base.WndProc(ref msg);

			if ((this.dcInternal != null) && (this.dcInternal.DockingManager.DesignProcess == false) && this.dcInternal.DockingManager.EnableDoubleClickOnCaption)
			{
				if ((msg.Msg == 0x0203) && ((int)msg.WParam == 0x0001))	// WM_LBUTTONDBLCLK & MK_LBUTTON
				{
					int ptint = (int)msg.LParam;
					DockingManager dockingManager = dcInternal.DockingManager;
					try
					{
						dockingManager.LockActivationEvents++;
						this.dcInternal.HandleDoubleClickImp(new Point(Syncfusion.Runtime.InteropServices.NativeMethods.LOWORD(ptint), Syncfusion.Runtime.InteropServices.NativeMethods.HIWORD(ptint)));
					}
					finally
					{
						dockingManager.LockActivationEvents--;
					}
				}
			}
            else if ((this.dcInternal != null) && (this.dcInternal.DockingManager.DesignProcess == false) && !this.dcInternal.DockingManager.EnableDoubleClickOnCaption)
            {
                if ((msg.Msg == 0x0203) && ((int)msg.WParam == 0x0001))	// WM_LBUTTONDBLCLK & MK_LBUTTON
                {
                    int ptint = (int)msg.LParam;
                    dcInternal.DockingManager.RaiseCaptionDoubleClick(this.dcInternal.DockingManager.ActiveControl);
                }
            }
		}
		#endregion

		internal void UpdateControlSize()
		{
			if((this.Controls.Count > 0) && (this.dcInternal != null))
			{
				Region rgnclip = new Region( new Rectangle(0,0,this.Size.Width,this.Size.Height) );
				rgnclip.Exclude(this.ClientRectangle);
				this.Invalidate(rgnclip, true);
                rgnclip.Dispose();
				if(this.dcInternal.DockTab == null)
				{
					DockingManager dockingManager = dcInternal.DockingManager;
					if( !dcInternal.DockingManager.LockUpdates && ! dockingManager.DesignerInitializing )
					{
						if( this.Controls[ 0 ].Location != this.ClientRectangle.Location )
							this.Controls[ 0 ].Location = this.ClientRectangle.Location;
						if( this.Controls[ 0 ].Size != this.ClientRectangle.Size )
							this.Controls[ 0 ].Size = this.ClientRectangle.Size;
					}
				}
			}
		}
		
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			base.SetBoundsCore(x, y, width, height, specified);
			UpdateControlSize();
		}

		protected override bool ProcessDialogKey(Keys keydata)
		{
			if((this.dcInternal != null) && (this.dcInternal.DockingManager.DesignProcess == false) && this.dcInternal.DockingManager.BrowsingKey != Keys.None)
			{
				if((this.dcInternal.DockingManager.AHInViewTab != null) && 
					(this.dcInternal.AutoHideMode == true) && (keydata == Keys.Escape))
				{
					this.dcInternal.DockingManager.AHInViewTab.HideController(null, false);
					this.dcInternal.DockingManager.HostControl.Focus();
					return true;
				}
				Control actControl = this.dcInternal.DockingManager.ActiveControl;
				Control nextControl = this.GetNextControl( actControl );

				if ( keydata == dcInternal.DockingManager.BrowsingKey )
				{
					for( int i = 0; i < this.dcInternal.DockingManager.alEnableDocking.Count; i++ )
					{
						if( nextControl != actControl && nextControl.TabStop )
							break;

						nextControl = this.GetNextControl(nextControl);
					}

					if( nextControl.TabStop )
						this.dcInternal.DockingManager.ActivateControl(nextControl);

					return true;
				}
			}
			return base.ProcessDialogKey(keydata);
		}

		/// <summary>
		/// Retrieves the next control forward in the tab order of docked controls.
		/// </summary>
		/// <param name="curControl"> The Control to start the search with. </param>
		/// <returns> The next control in the tab order. </returns>
		public Control GetNextControl( Control curControl )
		{
			int curTabIndex = curControl.TabIndex; // TabIndex of argument (curControl).
			int curCtrlIndex = -1;	// Index of curControl in ArrayList of docked cotrols.
			int minTabIndex = curTabIndex; // The least control's TabIndex in ArrayList of docked cotrols.
			int ctrlMinDiffIndex = -1;	// The least difference between curTabIndex and TabIndex.
			//of controls in ArrayList of docked cotrols.
			int ctrlIndex = -1;	// Index of control which will be used as an output value.
			int minCtrlIndex = -1; // Index of control which has the minTabIndex TabIndex.
			int i = 0;
			foreach( Control ctrl in this.dcInternal.DockingManager.alEnableDocking )
			{
				DockHostController dhc = this.dcInternal.DockingManager.GetDockHostController(ctrl);
				if ( !dhc.DockVisibility )
				{		// When the control is invisible, miss it.
					i++;
					continue;
				}
				if( ctrl.TabIndex == curTabIndex )
				{	// Get the current control index in ArrayList of docked controls.
					curCtrlIndex = i;
					i++;
					continue;
				}
				if( ctrl.TabIndex > curTabIndex )
				{
					int diff = ctrl.TabIndex - curTabIndex;	
					if( ctrlMinDiffIndex == -1 )
					{
						ctrlMinDiffIndex = diff;
						ctrlIndex = i;
					}
					if( diff < ctrlMinDiffIndex )
					{
						ctrlIndex = i;
					}
				}
				else
				{
					if( ctrl.TabIndex < minTabIndex )
					{
						minTabIndex = ctrl.TabIndex;
						minCtrlIndex = i;
					}
				}
				i++;
			}
			if( ctrlIndex == -1 )
			{	// Occurs when there are no controls with TabIndex larger than curTabIndex.
				if( minCtrlIndex > -1 )
					ctrlIndex = minCtrlIndex;
				else
					ctrlIndex = curCtrlIndex;
			}
			return this.dcInternal.DockingManager.alEnableDocking[ctrlIndex] as Control;
		}

		public void SubscribeDockHostEvents()
		{
			if(this.dcInternal.DockingManager.DesignProcess == false)
				{			
					this.MouseLeave += new System.EventHandler(DockHost_MouseLeave);
					this.MouseEnter +=new EventHandler(DockHost_MouseEnter);
				}
			this.ParentChanged += new System.EventHandler(this.this_ParentChanged);
		}

		public void UnsubscribeDockHostEvents()
		{
			if(this.dcInternal.DockingManager.DesignProcess == false)
			{
				this.MouseLeave -= new System.EventHandler(this.DockHost_MouseLeave);
				this.MouseEnter -= new System.EventHandler(this.DockHost_MouseEnter);
			}
			this.ParentChanged -= new System.EventHandler(this.this_ParentChanged);
		}
		
		/// Clean up any resources being used.		
		protected override void Dispose(bool bdisposing)
		{			
			if((bdisposing == true) && (this.dcInternal != null))
			{
				bool bDesignProcess = dcInternal.DockingManager.DesignProcess;
				if((this.Controls.Count > 0) && (bDesignProcess == false))
					RecSubscribeChildControlEvents(this.Controls[0], false);
				this.UnsubscribeDockHostEvents();				
				this.cpTitleBar.Dispose();
				this.cpTitleBar = null;
				this.dcInternal = null;

				if( Controls.Count > 0 )
				{
					Control control = Controls[ 0 ];
					if( ( !bDesignProcess ) && ( control != null ) )
					{
						control.HandleDestroyed -= new EventHandler( HostControl_HandleDestroyed );
						control.HandleCreated -= new EventHandler( HostControl_HandleCreated );
					}
				}

				if( null != this.Parent )
				{
					ContainerControl container = this.Parent as ContainerControl;

					// if parent has active control that is equal to the current DockHost then we can't dispose it.
					if( null != container && container.ActiveControl == this )
					{
						container.SelectNextControl( this, true, true, true, true );
					}
				} 
			}

			base.Dispose(bdisposing);
		}

		public DockControllerBase GetController()
		{
			return dcInternal;
		}
	}

	internal class NameGenerator
	{
		public static string Generate(Control ctrl)
		{
			DateTime dateTime = DateTime.Now;
			Random random = new Random();
			return String.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}_{8}",
				ctrl.Name, dateTime.Year, dateTime.Month, dateTime.Day,
				dateTime.Hour, dateTime.Minute, dateTime.Second,
				dateTime.Millisecond, random.Next(1000));
		}
	}
}
