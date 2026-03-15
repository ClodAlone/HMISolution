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
using System.Diagnostics;
using System.ComponentModel.Design;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools.Renderers;

namespace Syncfusion.Windows.Forms.Tools
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DockTabController : DockStateControllerBase, IResizable
	{
		protected DockHostController dcHostController = null;
		protected DockTabControl ctrlDockTab = null;
		protected bool bPause = false;

		protected DCRelationship currentRelationship = null;

		protected internal DockTabPage dragTabPage = null;
		protected internal bool bAllowDrag = false;
        internal bool bClosingByMouse = false;
		/// <summary>
		/// Variable indicating last hit tab button of tab control.
		/// </summary>
		protected internal int m_nHitTab = -1;

		public bool InDrag
		{
			get { return (this.dragTabPage != null); }
		}

		/// Gets a value indicating whether the control is being disposed of.
		private bool isClosing = false;
		/// <summary>
		/// Gets a value indicating whether the control is being disposed of.
		/// </summary>
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsClosing
		{
			get
			{
				return isClosing;
			}
		}

		internal override bool Maximized
		{
			get
			{
				return base.Maximized;
			}
			set
			{
				foreach( DockTabPage page in this.TabControl.TabPages )
				{
					DockHostController dhc = page.dhcClient;
					bool wasMaximized = dhc.Maximized;
					dhc.Maximized = value;

					if( wasMaximized && !value )
					{ 
						ControlRestoredEventArgs args = new ControlRestoredEventArgs(page.dhcClient.HostControl.Controls[0]
							, ControlSizeStates.Maximize);
						DockingManager.FireControlSizeStateChanged(ControlSizeStates.Restore, args);
					}
				}
				base.Maximized = value;
			}
		}		

		internal override void MaximizeController()
		{
			if( !CanMaximize )
				return;

			bool canMaximize = true;

			foreach( DockTabPage page in this.TabControl.TabPages )
			{
				ControlMaximizeEventArgs e = new ControlMaximizeEventArgs(
					page.dhcClient.HostControl.Controls[0]);
				DockingManager.FireControlSizeStateChanged(ControlSizeStates.Maximize, e);

				if( e.Cancel )
					canMaximize = false;
			}

			if( canMaximize )
			{
				base.MaximizeController();

				foreach( DockTabPage page in this.TabControl.TabPages )
				{
					ControlMaximizedEventArgs e = new ControlMaximizedEventArgs(
						page.dhcClient.HostControl.Controls[0]);
					DockingManager.FireControlSizeStateChanged(ControlSizeStates.Maximized, e);
				}
			}
		}

		public override Rectangle LayoutRect
		{
			get 
			{
				if( this.dcHostController != null )
					return this.dcHostController.LayoutRect;
				else
					return Rectangle.Empty;
			}
			set
			{
                if (this.dcHostController != null)//Fix for 1640:Application crashes when we dock a tabbed floating window by double clickking on it's caption 
                {
                    if (this.dcHostController.LayoutRect != value)
                    {
						if( Minimized == Minimization.Vertical )
							value.Size = new Size(value.Size.Width
								, this.dcHostController.LayoutRect.Height);
						else if( Minimized == Minimization.Horizontal )
							value.Size = new Size(this.dcHostController.LayoutRect.Width
								,value.Size.Height);

						if( this.FreezeResize )
						{
							if( !this.IsVerticallyResizable() )
								value.Size = new Size(value.Size.Width, this.LayoutRect.Height);
							if( !this.IsHorizontallyResizable() )
								value.Size = new Size(this.LayoutRect.Width, value.Size.Height);
						}
						foreach(DockTabPage page in this.TabControl.TabPages)
							page.dhcClient.LayoutRect = value;

                        if (this.dcHostController.HideCaption == true)
                            this.dockInfoCurrent.rcDockArea = this.HostControl.Parent.Bounds;
                        else
                            this.dockInfoCurrent.rcDockArea = this.HostControl.Parent.RectangleToScreen(
								this.HostControl.Bounds);
                        Size ctrlsize = this.HostControl.Controls[0].Size;
                        foreach (DockTabPage page in this.ctrlDockTab.TabPages)
                        {
                            if (page.dhcClient.HostControl == this.HostControl)
                                continue;
                            if (page.dhcClient.HostControl.Controls.Count > 0)
                                page.dhcClient.HostControl.Controls[0].Size = ctrlsize;
                        }
                    }
                }

			}
		}

		public override bool Floating
		{
			get { return this.ParentController.Floating; }
			set
			{
				foreach(DockTabPage page in this.ctrlDockTab.TabPages)
				{
					page.dhcClient.Floating = value;
				}
			}
		}

		public DockTabControl TabControl
		{
			get { return this.ctrlDockTab; }
		}

		public override Control HostControl
		{
			get { return (this.dcHostController != null) ? this.dcHostController.HostControl : null; }
		}

		public DockHostController HostController
		{
			get { return this.dcHostController; }
			set
			{
				// Unsubscribe from the previous hostcontroller
				if(this.dcHostController != null)
				{
					if(this.dcHostController.HostControl != null)
						this.dcHostController.HostControl.Resize -= new System.EventHandler(this.HostControl_Resize);
					if(this.dcHostController.DockTab == this.ctrlDockTab)
						this.dcHostController.DockTab = null;
				}
                this.dcHostController = value;
				if(value != null)
				{
					this.dcHostController.HostControl.Controls.Add(this.ctrlDockTab);
					this.dcHostController.DockTab = this.ctrlDockTab;
					this.dcHostController.HostControl.Resize += new System.EventHandler(this.HostControl_Resize);
					if( this.ToplevelController is FloatingFormController )
					{
						bool setCaption = this.ParentController is FloatingFormController;

						if( !setCaption )
						{
							SizingController sc = this.ToplevelController.ChildControllers[0] as SizingController;
							setCaption = ( sc != null && sc.GetDockControllers().Count == 1 );
						}

						if( setCaption )	// Set the floating form text
						{
							FloatingForm parentForm = this.ToplevelController.HostControl as FloatingForm;
							parentForm.Text = this.HostControl.Text;
							parentForm.SetCaption();
							NativeMethodsHelper.RedrawWindow( this.ParentController.HostControl.Handle , NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_FRAME );
						}
					}
				}
			}
		}

		internal override DockStateControllerWrapper InternalDockWrapper
		{
			get
			{
				DockTabPage page = this.TabControl.TabPages[0] as DockTabPage;
				return page.dhcClient.InternalDockWrapper;
			}
			set
			{
				SetChildWrapper(value);
			}
		}

		internal override DockStateControllerWrapper InternalFloatWrapper
		{
			get
			{
				DockTabPage page = this.TabControl.TabPages[0] as DockTabPage;
				return page.dhcClient.InternalFloatWrapper;
			}
			set
			{
				SetChildWrapper(value);
			}
		}

		internal override DockStateControllerWrapper TempWrapper
		{
			get
			{
				return base.TempWrapper;
			}
			set
			{
				base.TempWrapper = value;

				if( this.TabControl != null )
					foreach( DockTabPage page in this.TabControl.TabPages )
					{
						page.dhcClient.TempWrapper = value;
					}
			}
		}

		internal virtual void SetChildWrapper( DockStateControllerWrapper wrapper )
		{
			if( wrapper == null ||
				!( this.TempWrapper != null && this.TempWrapper.ParentController == wrapper.ParentController )	)
			{
				SetDockRelations(wrapper);
				bool floating = this.Floating;

				foreach( DockTabPage page in this.TabControl.TabPages )
				{
					DockHostController dhcCurrent = page.dhcClient;
					dhcCurrent.ForceSetWrapper = true;
					
					if( wrapper != null )
						floating = wrapper.Floating;

					if( floating )
						dhcCurrent.InternalFloatWrapper = wrapper;
					else
						dhcCurrent.InternalDockWrapper = wrapper;

					dhcCurrent.ForceSetWrapper = false;
				}

				this.TempWrapper = wrapper;
			}
			else
			{
				SetDockRelations(this.TempWrapper);

				foreach( DockTabPage page in this.TabControl.TabPages )
				{
					DockHostController dhcCurrent = page.dhcClient;

					if( wrapper.Floating )
						dhcCurrent.InternalFloatWrapper = this.TempWrapper;
					else
						dhcCurrent.InternalDockWrapper = this.TempWrapper;
				}
			}
		}

		protected internal override DockControllerBase QueryController( string uniqueName )
		{
			DockControllerBase dcb = null;

			foreach( DockTabPage page in this.TabControl.TabPages )
			{
				dcb = page.dhcClient.QueryController( uniqueName );

				if( dcb != null )
					break;
			}

			return dcb;
		}

		internal void SetDockRelations( DockStateControllerWrapper wrapper )
		{
			if( wrapper != null )
			{
				ArrayList tabControllers = new ArrayList();
				DockHostController leadHost = null;
				bool rearrange = this.TabControl.TabCount > wrapper.DockRelationControllers.Count;
				for( int i = 0; i < this.TabControl.TabCount; i++ )
				{
					DockTabPage page = this.TabControl.TabPages[i] as DockTabPage;
					if( leadHost == null )
						leadHost = page.dhcClient;
					tabControllers.Add( page.dhcClient );

					if( rearrange && wrapper.DockRelationControllers.Contains( page.dhcClient ) )
						wrapper.DockRelationControllers.Remove( page.dhcClient );

					if( !wrapper.DockRelationControllers.Contains( page.dhcClient ) )
						wrapper.DockRelationControllers.Insert( i, page.dhcClient );
				}

				int count = 0;

				foreach( DockTabPage page in this.TabControl.TabPages )
				{
					DockHostController dhcCurrent = page.dhcClient;
					DockInfo info = new DockInfo( leadHost, DockingStyle.Tabbed, count
						, count, DockPreference.Tabbed, page.dhcClient.LayoutRect );
					DockRelation dRel = new DockRelation();
					dRel.RelatedControllers = new ArrayList( tabControllers );
					dRel.Relation = info;
					wrapper.Relations[dhcCurrent.UniqueName] = dRel;
					count++;
				}
			}
		}

		public override DockControllerBase ParentController
		{
			set
			{
				this.dcParent = value;
				if(value != null)
				{
					if( value is FloatingFormController )
					{
						foreach(DockTabPage page in this.ctrlDockTab.TabPages)
							page.dhcClient.HideCaption = true;
					}
					else
					{
						foreach(DockTabPage page in this.ctrlDockTab.TabPages)
							page.dhcClient.HideCaption = false;
					}
				}
			}

			get
			{
				return this.dcParent;
			}
		}

		public bool PauseActivation
		{
			get { return this.bPause; }
			set { this.bPause = value; }
		}

		public override int ChildCount
		{
			get
			{
				return this.ctrlDockTab.TabPages.Count;
			}
		}

		public override IEnumerator DCR
		{
			get
			{
				return this.HostController.DCR;
			}
		}

		public override DCRelationship DCRCurrent
		{
			get
			{
				return this.currentRelationship;
			}

			set // Set this DCR value to all child dockhosts
			{
				if(this.currentRelationship == null)
					this.currentRelationship = value;
				if( ctrlDockTab.TabPages.Count > 0 )
					((DockTabPage)ctrlDockTab.TabPages[0]).dhcClient.DCRCurrent = value;
			}
		}

		public DockHostController SelectedController
		{
			get { return this.dcHostController ; }
			set
			{
				if(this.dcHostController != value)
				{
                    if (this.ctrlDockTab.TabPages == null)
                        return;
					foreach(DockTabPage page in this.ctrlDockTab.TabPages)
					{
						if(page.dhcClient == value)
						{
							this.ctrlDockTab.SelectedTab = page;
							return;
						}
					}
					Debug.Assert(false, "Error: Invalid Controller.");
				}
			}
		}

		public DockTabController(DockingManager dmgr, DockTabControl tabctrl) : base(dmgr)
		{
			this.ctrlDockTab = tabctrl;
			this.ctrlDockTab.SelectedIndexChanged += new System.EventHandler(this.DockTab_SelectedIndexChanged);
		}

		public override void AddToDCR(DCRelationship dcr)
		{
			foreach(DockTabPage page in this.ctrlDockTab.TabPages)
				page.dhcClient.AddToDCR(dcr);
		}

		public override void InsertIntoDCR(ArrayList al, int nindex, DCRelationship dcr)
		{
			foreach(DockTabPage page in this.ctrlDockTab.TabPages)
			{
				if(nindex == 0)	// The tab DCR should always be the first one
				{
					ArrayList dcrlist = this.Floating ? page.dhcClient.FloatDCRList : page.dhcClient.DockDCRList;
					if(dcrlist.Contains(page.dhcClient.DCRCurrent))
						page.dhcClient.InsertIntoDCR(dcrlist, dcrlist.IndexOf(page.dhcClient.DCRCurrent)+1, dcr);
					else
						page.dhcClient.InsertIntoDCR(null, nindex, dcr);
				}
				else
					page.dhcClient.InsertIntoDCR(null, nindex, dcr);
			}
		}
		public override void RemoveFromDCR( DCRelationship dcr )
		{
			foreach( DockTabPage page in this.ctrlDockTab.TabPages )
				page.dhcClient.RemoveFromDCR(dcr);
		}

		public override void UpdateDCRIndex(DCRelationship dcrs)
		{
			// The tabcontroller's position in the dock hierarchy has changed. Update this info in the child DCRs.
			dcrs.nIndex = this.ParentController.GetChildHostIndex(this);
			foreach(DockTabPage page in this.ctrlDockTab.TabPages)
				page.dhcClient.UpdateDCRIndex(dcrs);
		}

		public override void RemoveChild(DockControllerBase dc)
		{
			foreach(DockTabPage page in this.ctrlDockTab.TabPages)
			{
				if(page.dhcClient == dc)
					this.ctrlDockTab.TabPages.Remove(page);
			}
		}

		public override DockControllerBase GetChildAt(int index)
		{
			if( (index < 0) || (index > this.ctrlDockTab.TabPages.Count-1) )
			{
				Debug.Assert(false, "Error: Invalid index");
				return null;
			}
			return (this.ctrlDockTab.TabPages[index] as DockTabPage).dhcClient;
		}

		public override int GetChildHostIndex(DockControllerBase child)
		{
			foreach(DockTabPage page in this.ctrlDockTab.TabPages)
			{
				if(page.dhcClient == child)
					return this.ctrlDockTab.TabPages.IndexOf(page);
			}
			return -1;
		}

		public override void GetDockInfo(Control ctrl, Point pt, DockInfo di)
		{
			di.DP = DockPreference.Tabbed;
			di.dStyle = Syncfusion.Windows.Forms.Tools.DockingStyle.Fill;
			Point ptclient = this.ctrlDockTab.PointToClient(pt);
			if(ptclient.X < this.ctrlDockTab.GetTabRect(0).Right)
			{
				di.nDockIndex = 0;
				return;
			}
			int nhit = this.ctrlDockTab.GetTabHitIndex(ptclient);
			if(nhit >= 0)
			{
				di.nDockIndex = nhit;
				return;
			}

			// The cursor is in the space beyond the last tab. If the last tab is has a non-null dhcclient, then
			// return the lasttab index+1, else return the last tab index
			if( (this.ctrlDockTab.TabPages[this.ctrlDockTab.TabPages.Count-1] as DockTabPage).dhcClient == null)
				di.nDockIndex = this.ctrlDockTab.TabPages.Count-1;
			else
				di.nDockIndex = this.ctrlDockTab.TabPages.Count;
		}

		public override bool QueryDropProceedWithDock(Control ctrldrop, Syncfusion.Windows.Forms.Tools.DockingStyle style)
		{
			DockAllowEventArgs arg = new DockAllowEventArgs(ctrldrop.Controls[0], this.HostControl.Controls[0], style);
			this.dockingMgr.FireDockAllowEvent(arg);
			return !arg.Cancel;
		}

		public void SetHostCtrlForSelection()
		{
            bool useLockHost = true;

            //Checks whether form is already locked or not.
            if (this.dcHostController.DockingManager.lockedHostForm)
                useLockHost = false;
            if (useLockHost)
                this.dcHostController.DockingManager.LockHostFormUpdate();

			if((this.ctrlDockTab.TabCount == 0) || (this.ctrlDockTab.SelectedIndex < 0))
				return;
			// Make the dockhostcontroller associated with the new tab index as the hostcontroller, parent the
			// tabcontrol to this controller's HostControl and hide the previous controller
			DockHostController dcprev = this.HostController;
			this.HostController = (this.ctrlDockTab.TabPages[this.ctrlDockTab.SelectedIndex] as DockTabPage).dhcClient;
			this.dcHostController.LayoutRect = dcprev.LayoutRect;
			this.AdjustLayout();

			int lockEvents = this.dockingMgr.LockActivationEvents;
			this.dockingMgr.LockActivationEvents = 0;
			if( !this.bInAutoHide )
				NativeMethods.SetFocus(dcHostController.HostControl.Controls[0].Handle);
			this.dockingMgr.LockActivationEvents = lockEvents;

			if(this.InDrag == false)
				dcprev.HostControl.Visible = false;
			if(this.bInAutoHide == false)
				this.dcHostController.HostControl.Visible = true;
            if(useLockHost)
                this.dcHostController.DockingManager.UnlockHostFormUpdate();
		}

		public void DockTab_SelectedIndexChanged(Object sender, EventArgs e)
		{
			if(this.bPause == false)
				this.SetHostCtrlForSelection();
			
			this.dockingMgr.UpdateFloatingFormsImages();
		}

		public void HostControl_Resize(Object sender, EventArgs e)
		{
			AdjustLayout();
		}

		public override void AdjustLayout()
		{
			if( this.dcHostController == null )
				return;

			dcHostController.HostControl.Controls[0].Bounds = GetHostControlBounds();
		}

		internal protected Rectangle GetHostControlBounds()
		{
			Rectangle hostBounds = Rectangle.Empty;
			Rectangle rcclient = ( this.dcHostController.HostControl as DockHost ).ClientRectangle;
			int ntabheight = this.dockingMgr.DockTabHeight - 1;

			// Represents the top position of tab panels rectangle.
			// Default value set for DockTabAlignmentStyle.Bottom
			int tabPosTop = rcclient.Bottom - ntabheight;  

			int shiftValue = (dcHostController.AutoHideMode)? 0: ntabheight;
			int adjValue = (dcHostController.AutoHideMode)? 0: 3;
			ctrlDockTab.Alignment = (TabAlignment)Enum.Parse(typeof(DockTabAlignmentStyle),dockingMgr.DockTabAlignment.ToString(),true);
			ctrlDockTab.ShowScroll = this.dockingMgr.ShowDockTabScrollButton;

			if( this.dockingMgr.DockTabAlignment != DockTabAlignmentStyle.Bottom )
			{
				FloatingFormController ffc = this.ParentController as FloatingFormController;
				int captRectBottom = 0;	// bottom position of caption rectangle
				DockHostController dhc = this.HostController as DockHostController;
				if( dhc != null )
				{
					Rectangle captrect;
					if( dockingMgr.VisualStyle == VisualStyle.Default )
					{
						captrect = (dhc.HostControl as DockHost).TitleBar.CaptionRect;
					}
					else
					{
						int borderWidth = dockingMgr.Renderer.ThinBorderWidth;
						int captionWidth = ( this.DockingManager.ShowCaption )? dockingMgr.Renderer.CaptionWidth : 0;
						captrect = new Rectangle( borderWidth, borderWidth,
							LayoutRect.Width - 2 * borderWidth, captionWidth );
					}
					captRectBottom = captrect.Height + captrect.Y;
				}
				if( ffc != null && ffc.ParentController == null )
					tabPosTop = 1;
				else
					tabPosTop = captRectBottom + 2;
				switch( dockingMgr.DockTabAlignment )
				{
					case DockTabAlignmentStyle.Top:
						rcclient.Offset( 0, shiftValue + 1 );
						ctrlDockTab.Size = new Size(rcclient.Width, ntabheight - 1);
						ctrlDockTab.Location = new Point(rcclient.X, tabPosTop);
						hostBounds = new Rectangle( rcclient.X, rcclient.Y + adjValue - 1,
							rcclient.Width, rcclient.Height-shiftValue-3); // resize selected docked control
						break;
					case DockTabAlignmentStyle.Left:
						rcclient.Size = new Size( rcclient.Width - shiftValue, rcclient.Height + ntabheight );
						rcclient.Location = new Point( shiftValue + 1, tabPosTop );
						ctrlDockTab.Size = new Size( ntabheight, rcclient.Height - ntabheight );
						ctrlDockTab.Location = new Point( 2, tabPosTop );
						hostBounds = new Rectangle( rcclient.X + adjValue + 1, rcclient.Y,
							rcclient.Width - adjValue - 1, rcclient.Height-ntabheight);
						break;
					case DockTabAlignmentStyle.Right:
						rcclient.Size = new Size( rcclient.Width - shiftValue - 1, rcclient.Height + ntabheight );
						rcclient.Location = new Point( 1, tabPosTop );
						ctrlDockTab.Size = new Size( ntabheight, rcclient.Height - ntabheight );
						ctrlDockTab.Location = new Point( rcclient.Width+2, tabPosTop );
						hostBounds = new Rectangle( rcclient.X, rcclient.Y,
							rcclient.Width - adjValue - 1, rcclient.Height-ntabheight);
						break;
				}
			}
			else
			{
				int borderCorrection = 2;
				ctrlDockTab.Size = new Size( rcclient.Width, ntabheight + borderCorrection );
				ctrlDockTab.Location = new Point( rcclient.X, tabPosTop - borderCorrection );
				if( dockingMgr.VisualStyle == VisualStyle.Default )
					borderCorrection = 0;
				hostBounds = new Rectangle( rcclient.X, rcclient.Y,
					rcclient.Width, rcclient.Height - shiftValue - borderCorrection	);
			}

			return hostBounds;
		}

		// Close all tabpages and dispose tabcontrol
		public override void CloseController()
		{
			foreach( DockTabPage page in TabControl.TabPages )
			{
                if (page.dhcClient != null) // This might be null when tabs are in Floating form and Floating Form is closed.
                {
                    page.dhcClient.bClosingByMouse = this.bClosingByMouse;
                    page.dhcClient.CloseController();
                    page.dhcClient.bClosingByMouse = false;
                }
			}
		}

		public virtual void CloseController(DockHostController dcclose)
		{			
			this.isClosing = true;
			DockTabPage closepage = null;
			if(dcclose == null)
			{
				closepage = this.ctrlDockTab.SelectedTab as DockTabPage;
			}
			else
			{
				foreach(DockTabPage page in this.ctrlDockTab.TabPages)
				{
					if(page.dhcClient == dcclose)
					{
						closepage = page;
						break;
					}
				}
			}
			Debug.Assert((closepage != null), "Error: Invalid Controller.\n");			

			DockStateControllerWrapper wrap = new DockStateControllerWrapper(this.dockingMgr, this);
			wrap.ParentController = this.ParentController;			
			
			if(this.bInAutoHide == false)
			{
				this.ParentController.ChildWrapper = wrap;
				this.SetChildWrapper(wrap);
				this.ParentController.ChildWrapper = null;

				this.CheckFormOwnership(closepage.dhcClient);

				if(closepage != this.ctrlDockTab.SelectedTab)
					this.PauseActivation = true;
				if(this.ctrlDockTab.TabPages.Contains(closepage))
					this.ctrlDockTab.TabPages.Remove(closepage);
				closepage.dhcClient.ParentController = null;
				if(this.PauseActivation == true)
					this.PauseActivation = false;
				this.ParentController.ControllerChanged += new ControllerChangedEH(closepage.dhcClient.TransientControllerChanged);
				closepage.dhcClient.DITransient = new DockInfo(this.ParentController, this.dockInfoCurrent.dStyle, this.dockInfoCurrent.nPriority,
					this.ParentController.GetChildHostIndex(this), this.dockInfoCurrent.DP, this.dockInfoCurrent.rcDockArea);
				closepage.dhcClient.HostControl.Visible = false;

				if(this.Floating == true)
				{
					closepage.dhcClient.AdjustLayout();	// Updates the layout rect.
					FloatingForm frmfloat = (this.HostControl as ContainerControl).ParentForm as FloatingForm;
                    if (frmfloat.Owner != null)
                    {
                        // Reparent the control to the form's owner - the mainform
                        frmfloat.Owner.Controls.Add(closepage.dhcClient.HostControl);
                    }
					closepage.dhcClient.SharedForm = frmfloat;
					dcclose.DIPrevious.rcDockArea = frmfloat.Bounds;
					( dcclose.HostControl as DockHost ).DragRectangle = frmfloat.Bounds;

					// If the tabcontroller was the form's only controller, then dispose form upon exit.
					if(frmfloat.InternalController.ChildCount == 0)
					{
						frmfloat.Disable();
						frmfloat.InternalController.Dispose();
					}
				}
			}
			else	// If in autohide mode, transfer hosting and close the particular controller.
			{
				closepage.dhcClient.DINew = new DockInfo(this.dockInfoNew);	// Enables the formcontroller to retrieve the border tab.
				MainFormController frmctrlr = closepage.dhcClient.ToplevelController as MainFormController;
				if(closepage != this.ctrlDockTab.SelectedTab)
					this.PauseActivation = true;
				this.ctrlDockTab.TabPages.Remove(closepage);
				if(this.PauseActivation == true)
					this.PauseActivation = false;
				closepage.dhcClient.bInAutoHide = false;

				DockControllerBase splitterdc = this.ParentController.GetChildAt(1);
				DragSplitter splitter = splitterdc.HostControl as DragSplitter;
				Debug.Assert(splitter != null);
				splitter.DragSplitterMoved -= new SplitterEventHandler(closepage.dhcClient.DHCDragSplitterMoved);

				frmctrlr.ExitAutoHideMode(closepage.dhcClient, true);

				closepage.dhcClient.ParentController = null;
				this.dockInfoTransient.dController.ControllerChanged += new ControllerChangedEH(closepage.dhcClient.TransientControllerChanged);
				closepage.dhcClient.DITransient = new DockInfo(this.dockInfoTransient);

				if(this.ctrlDockTab.TabPages.Count == 1)
				{
					DockHostController remdhc = (this.ctrlDockTab.SelectedTab as DockTabPage).dhcClient;
					this.dockInfoTransient.dController.ControllerChanged -= new ControllerChangedEH(this.TransientControllerChanged);
					this.dockInfoTransient.dController.ControllerChanged += new ControllerChangedEH(remdhc.TransientControllerChanged);
					remdhc.DITransient = new DockInfo(this.dockInfoTransient);
					remdhc.DINew = new DockInfo(this.dockInfoNew);
				}
			}

			// If only one tab remains in the tabcontrol, then remove this tabpage from the control and substitute the
			// tabcontroller's position within the hierarchy with the hostcontroller associated with this tabpage.
			if(this.ctrlDockTab.TabPages.Count == 1)
			{
				DockHostController dhcnew = (this.ctrlDockTab.SelectedTab as DockTabPage).dhcClient;
				DockInfo diTrans = null;
				if( this.bInAutoHide )
					diTrans = new DockInfo(dhcnew.DITransient);
				this.PauseActivation = true;
				this.ctrlDockTab.TabPages.Remove(this.ctrlDockTab.SelectedTab);
				this.ParentController.ReplaceChild(this, dhcnew);
				this.HostController = null;
				dhcnew.DockTab = null;
				dhcnew.AdjustLayout();

				if( this.bInAutoHide )
					dhcnew.DITransient = diTrans;
				this.ctrlDockTab.Visible = false;
				this.ctrlDockTab.Dispose();
				this.ctrlDockTab = null;
			}
			this.isClosing = false;
		}

		public void TransitFloatToDock()
		{
			// Clear the dockdcrlist
			foreach(DockTabPage page in this.ctrlDockTab.TabPages)
			{
				page.dhcClient.DockDCRList.Clear();
				page.dhcClient.HideCaption = false;
			}
			InternalTransitFloatToDock(false);
			this.DockEdge = DINew.dStyle;

			if(this.ctrlDockTab != null)
				this.UpdateCurrentDCRelationship();
		}

		public void TransitDockToFloat()
		{
			// Clear floatdcrlist
			foreach(DockTabPage page in this.ctrlDockTab.TabPages)
			{
				page.dhcClient.FloatDCRList.Clear();
				page.dhcClient.HideCaption = true;
			}
			InternalStateUpdateForDockToFloat();

			Point ptlocation;
			if((this.dockInfoNew.rcDockArea.Width<=0 ||this.dockInfoNew.rcDockArea.Height<=0) == true)
				ptlocation = this.HostControl.Parent.PointToScreen(this.HostControl.Location);
			else
				ptlocation = this.dockInfoNew.rcDockArea.Location;
			this.dockingMgr.UndockFromController(this);
			this.Floating = true;
			CreateFloatingFrame(ptlocation);

			if(this.ctrlDockTab != null)
				this.UpdateCurrentDCRelationship();
				
			this.dockingMgr.UpdateFloatingFormsImages();
		}

		public void TransitDockToDock()
		{
			// Erase the dockdcr's for all child controllers
			foreach(DockTabPage page in this.ctrlDockTab.TabPages)
				page.dhcClient.DockDCRList.Clear();
			InternalTransitDockToDock(false);

			if(this.ctrlDockTab != null)
				this.UpdateCurrentDCRelationship();
		}

		public void TransitDockToDockInFloat()
		{
			// Clear floatdcrlist
			foreach(DockTabPage page in this.ctrlDockTab.TabPages)
				page.dhcClient.FloatDCRList.Clear();
			InternalStateUpdateForDockToFloat();
			InternalTransitDockToDock(true);

			if(this.ctrlDockTab != null)
				this.UpdateCurrentDCRelationship();
				
			this.dockingMgr.UpdateFloatingFormsImages();
		}

		public void TransitFloatToDockInFloat()
		{
			// Clear the floatdcrlist
			foreach(DockTabPage page in this.ctrlDockTab.TabPages)
			{
				page.dhcClient.FloatDCRList.Clear();
				page.dhcClient.HideCaption = false;
			}
			InternalTransitFloatToDock(true);

			if(this.ctrlDockTab != null)
				this.UpdateCurrentDCRelationship();

			this.dockingMgr.UpdateFloatingFormsImages();
		}

		public void TransitDockInFloatToFloat()
		{
			TransitDockToFloat();
		}

		protected void InternalStateUpdateForDockToFloat()
		{
			// Before undocking, add the tabbed relation to all the dockhostcontroller housed within this
			// tabcontroller and subscribe each child controller to the tab's parent controller's ControllerChanged
			// event
			foreach(DockTabPage page in this.ctrlDockTab.TabPages)
			{
				// If the tabcontrol is being floated out of a mainform, then update the previous dockinfo for all
				// child controllers. However, if the tab is being floated out of a floating form, then retain existing
				// info.
				if(this.Floating == false)
				{
					if(page.dhcClient.DIPrevious.dController != null)
						page.dhcClient.DIPrevious.dController.ControllerChanged -= new ControllerChangedEH(page.dhcClient.dhc_ControllerChanged);
					this.ParentController.ControllerChanged += new ControllerChangedEH(page.dhcClient.dhc_ControllerChanged);
					page.dhcClient.DIPrevious = new DockInfo(this.ParentController, this.dockInfoCurrent.dStyle, this.dockInfoCurrent.nPriority, this.ParentController.GetChildHostIndex(this),
						this.dockInfoCurrent.DP, this.dockInfoCurrent.rcDockArea);
				}
			}
		}

		protected void InternalTransitDockToDock(bool bpostfloatstate)
		{
			this.dockingMgr.UndockFromController(this);
			this.Floating = bpostfloatstate;
			this.dockInfoNew.dController.InvokeDocking(this);
			this.AdjustLayout();

			// If a tabbed docking takes place, then this tab is no longer be needed. InvokeDocking will reparent
			// the tabpage clients to the dock target's tabcontroller.
			if(this.dockInfoNew.DP == DockPreference.Tabbed)
			{
				this.PauseActivation = true;
				// Remove all tabs from this tabcontrol and dispose the control
				foreach(DockTabPage page in this.ctrlDockTab.TabPages)
					this.ctrlDockTab.TabPages.Remove(page);
				this.HostController = null;
				this.ctrlDockTab.Visible = false;
				this.ctrlDockTab.Dispose();
				this.ctrlDockTab = null;
			}
		}

		protected void InternalTransitFloatToDock(bool bpostfloatstate)
		{
			FloatingForm frmfloat = (this.HostControl as ContainerControl).ParentForm as FloatingForm;
			Debug.Assert((frmfloat != null), "Error: Invalid parent container.\n");

			// If, the tabcontroller is being docked to a non-floating controller, then update the diprevious info
			if(this.dockInfoNew.dController.Floating == false)
			{
				foreach(DockTabPage page in this.ctrlDockTab.TabPages)
				{
					if(page.dhcClient.DIPrevious.dController != null)
						page.dhcClient.DIPrevious.dController.ControllerChanged -= new ControllerChangedEH(page.dhcClient.dhc_ControllerChanged);
					page.dhcClient.DIPrevious = new DockInfo(null, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill, 0, -1, DockPreference.All, this.dockInfoCurrent.rcDockArea);
				}
			}

			if( this.HostController.IsSingleFloatControl )
			{
				foreach( DockTabPage page in this.TabControl.TabPages )
					page.dhcClient.PreviousFloatSize = frmfloat.Size;
				foreach( DockTabPage page in this.TabControl.TabPages )
					page.dhcClient.PreviousFloatLocation = frmfloat.Location;
			}

			this.dockingMgr.UndockFromController(this);
			this.Floating = bpostfloatstate;

			foreach( DockTabPage page in this.ctrlDockTab.TabPages )
			{
				page.dhcClient.AssignFormBelongings(frmfloat);
			}

			// If the tabcontroller was the form's only controller, then dispose form upon exit.
			DockControllerBase floatChild = frmfloat.InternalController.ChildControllers[0] as DockControllerBase;
			if( floatChild is DockStateControllerWrapper )
			{
				frmfloat.Disable();
			}

			this.dockInfoNew.dController.InvokeDocking(this);

			if(this.dockInfoNew.DP == DockPreference.Tabbed)
			{
				this.PauseActivation = true;
				// Remove those tabs that have been reparented. This behavior is different from a TransitDockToDock, where
				// all tabpages in this tab are removed, because of the need to support FloatingForm - Redocking behavior where
				// tabpages will be undocked individually and this tab remains a valid host even after the invokedocking call.
				foreach(DockTabPage page in this.ctrlDockTab.TabPages)
				{					
					this.ctrlDockTab.TabPages.Remove(page);
				}
				if(this.ctrlDockTab.TabPages.Count <= 0)
				{
					this.HostController = null;
					this.ctrlDockTab.Visible = false;
					this.ctrlDockTab.Dispose();
					this.ctrlDockTab = null;
				}
			}
		}

		public override void InvokePrevDockFloatTransition( bool showFloating )
		{
			bool transitToDock = this.Floating;

			if( transitToDock )
			{
				FloatingFormController topParent = this.ToplevelController as FloatingFormController;
				if( topParent != null )
				{
					SizingController sc = topParent.ChildControllers[0] as SizingController;

					if( sc != null && sc.GetDockControllers().Count > 1 )
						transitToDock = false;
				}
			}
			// If the tabcontroller is floating within a unique floating frame, then invoke a TransitToPrevDock
			// call on the tabcontroller. This method will iterate the tab's children and call TransitToPrevDock
			// on each child dockhostcontroller. However, if the tabcontroller is docked, either within the mainframe
			// or within a floatingframe, issue a TransitToPrevFloat() call on the tabcontroller.
			if( transitToDock || this.dockingMgr.DisallowFloating )
				TransitToPrevDock();
			else
				TransitToPrevFloat();
		}


		protected internal void TransitToPrevDock()
		{
			FloatingForm frmfloat = (this.HostControl as ContainerControl).ParentForm as FloatingForm;			

			this.HostController = null;
			this.PauseActivation = true;
			DockStateControllerBase baseCtrl = null;
            DockTabPage selectedPage = this.ctrlDockTab.SelectedTab as DockTabPage;
            DockHostController dhcSelected = 
                (selectedPage == null)?
                null:
                selectedPage.dhcClient;

			foreach(DockTabPage page in this.ctrlDockTab.TabPages)
			{
				page.dhcClient.DockTab = null;
				page.dhcClient.TransitToPrevDock();
				if(page.dhcClient.HostControl.Visible == false)
				{
					page.dhcClient.HostControl.Visible = true;
					page.dhcClient.HideCaption = false;
				}
				if( page.dhcClient.InternalForm != null &&
					( page.dhcClient.InternalForm == frmfloat ) )
					baseCtrl = page.dhcClient;

				page.dhcClient.SharedForm = frmfloat;
			}

            if (dhcSelected != null)
            {
                DockTabController dtc = dhcSelected.ParentController as DockTabController;
                if (dtc != null && dtc.ctrlDockTab != null)
                {
                    foreach (DockTabPage page in dtc.ctrlDockTab.TabPages)
                        if (page.dhcClient == dhcSelected)
                        {
                            dtc.ctrlDockTab.SelectedTab = page;
                            break;
                        }
                }
            }
			
			this.dockingMgr.UndockFromController(this);

			foreach( DockTabPage page in this.ctrlDockTab.TabPages )
			{
				this.ctrlDockTab.TabPages.Remove(page);
			}

			this.ctrlDockTab.Visible = false;
			this.ctrlDockTab.Dispose();
			this.ctrlDockTab = null;

			if( frmfloat != null )
			{
				SizingController sc = frmfloat.InternalController.ChildControllers[0] as SizingController;

				if( ( sc != null && sc.IsEmpty ) || frmfloat.InternalController.ChildControllers[0]
					is DockStateControllerWrapper )
				{
					frmfloat.Disable();
					frmfloat.Used = false;
					frmfloat.FormOwner = baseCtrl;
				}
			}

			if(this.ctrlDockTab != null)
				this.UpdateCurrentDCRelationship();
		}

		protected internal void TransitToPrevFloat()
		{
			this.FreezeResize = false;
			Control[] ctrls = new Control[this.ctrlDockTab.TabCount];
			int i = 0;
			foreach(DockTabPage page in this.ctrlDockTab.TabPages)
				ctrls[i++] = page.dhcClient.HostControl.Controls[0];
			this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));

			Rectangle rcfloat = Rectangle.Empty;
			if(this.Floating == false)
				rcfloat = this.HostController.DIPrevious.rcDockArea;
			this.dockInfoNew = new DockInfo(null, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill, 0, -1, DockPreference.All, rcfloat);
			// If this is a float-to-float transition, then clean up the floatdcr list.
			foreach(DockTabPage page in this.ctrlDockTab.TabPages)
			{
				if(this.Floating)
					page.dhcClient.FloatDCRList.Clear();
				page.dhcClient.HideCaption = true;
			}
			InternalStateUpdateForDockToFloat();

			Point ptlocation;
			if((this.dockInfoNew.rcDockArea.Width<=0 || this.dockInfoNew.rcDockArea.Height<=0) == true)
				ptlocation = this.HostControl.Parent.PointToScreen(this.HostControl.Location);
			else
				ptlocation = this.dockInfoNew.rcDockArea.Location;
			this.dockingMgr.UndockFromController(this);
			this.Floating = true;
			CreateFloatingFrame(ptlocation);

			if(this.ctrlDockTab != null)
				this.UpdateCurrentDCRelationship();

			this.AdjustLayout();
			this.dockingMgr.FireDockStateChangeEvent("DockStateChanged", new DockStateChangeEventArgs(ctrls));
			this.FreezeResize = true;
		}

		public override void Refresh()
		{
			if( !( this.ParentController is FloatingFormController ) )
				this.ParentController.Refresh();
		}

		protected void UpdateCurrentDCRelationship()
		{
			if(this.ctrlDockTab != null)
			{
				foreach(DockTabPage tabpage in this.ctrlDockTab.TabPages)
				{
					DCRelationship dcrelation = new DCRelationship(this.currentRelationship.nRelation, true,
						DockPreference.Tabbed, this.ctrlDockTab.TabPages.IndexOf(tabpage));
					bool bexists = false;
					IEnumerator ienum = tabpage.dhcClient.DCR;
					while(ienum.MoveNext())
					{
						DCRelationship dcr = ienum.Current as DCRelationship;
						if(dcr.nRelation == dcrelation.nRelation)
						{
							bexists = true;
							break;
						}
					}
					if(bexists == false)
					{
						tabpage.dhcClient.DCRCurrent = dcrelation;
					}
				}
			}
		}

		internal override Direction MustResize
		{
			get
			{	
				return base.MustResize;
			}
			set
			{
				foreach( DockTabPage page in this.TabControl.TabPages )
				{
					if( page.dhcClient.FreezeResize )
						page.dhcClient.MustResize = value;
				}

				base.MustResize = value;
			}
		}

		public void RemoveDockHostFromTab(DockControllerBase dc, bool btransittoprev)
		{
			// If this controller is not the selected tab, then select it. Only the selected tab can be removed.
			DockHostController dhc = dc as DockHostController;
			this.SelectedController = dhc;
            if (this.ctrlDockTab.SelectedTab == null) return;

			if( !this.dockingMgr.ForbidWrapperLogic )
			{
				DockStateControllerWrapper wrap = new DockStateControllerWrapper( this.dockingMgr, this );
				wrap.ParentController = this.ParentController;
				this.ParentController.ChildWrapper = wrap;
				this.SetChildWrapper( wrap );
				this.ParentController.ChildWrapper = null;
			}

			this.ctrlDockTab.TabPages.Remove(this.ctrlDockTab.SelectedTab);

			if(btransittoprev == true)
			{
				if( this.Floating == true || this.dockingMgr.DisallowFloating )
				{
					CheckFormOwnership( dhc );

					dhc.SharedForm = this.ToplevelController.HostControl as FloatingForm;
					dhc.TransitToPrevDock();
				}
				else
				{
					// Failing to set the DragRectangle will force the dockhost to use it's current bounds for the floating frame
					if( dhc.DIPrevious.rcDockArea.Size.IsEmpty == false )
						( dhc.HostControl as DockHost ).DragRectangle = dhc.DIPrevious.rcDockArea;
					dhc.TransitToPrevFloat(true);
				}
			}
			else
			{
				if(this.Floating == true)
				{
					CheckFormOwnership( dhc );
					// Float as an individual frame.
					dhc.CreateFloatingFrame(dhc.HostControl.PointToScreen(dhc.HostControl.Location), true);
					dhc.Floating = true;
				}
				else
				{
					Size size = dhc.LayoutRect.Size;
					size.Width = size.Width / 2;
					size.Height = size.Height / 2;

					dhc.LayoutRect = new Rectangle(dhc.LayoutRect.Location, size);
					// Redock to the parentcontroller containing this tabcontroller. ie., become a sibling of this tabcontroller.
					dhc.DINew = new DockInfo(this.ParentController, this.dockInfoCurrent.dStyle, this.dockInfoCurrent.nPriority,
										this.dockInfoCurrent.nDockIndex, this.dockInfoCurrent.DP, dhc.LayoutRect);
					this.InvokeDocking(dhc);
				}
			}

			dhc.AdjustLayout();
			dhc.HostControl.Visible = true;
			if(this.dockingMgr.DesignMode == false)
				dhc.HostControl.Focus();

			UpdateTabPages();
		}

		internal void UpdateTabPages()
		{
			// If this is the last tab, then remove the tabpage from the control and float/dock the last dockcontroller
			// The tabcontrol/controller will be disposed after the message processing is done.
			if(this.ctrlDockTab.TabPages.Count == 1)
			{
				DockHostController dhcnew = (this.ctrlDockTab.SelectedTab as DockTabPage).dhcClient;
				this.PauseActivation = true;
				this.ctrlDockTab.SelectedIndexChanged -= new System.EventHandler( this.DockTab_SelectedIndexChanged );
				this.ctrlDockTab.TabPages.Remove(this.ctrlDockTab.SelectedTab);
				this.ParentController.ReplaceChild(this, dhcnew);
				this.HostController = null;
				dhcnew.StoredSizes = this.StoredSizes;
				dhcnew.HostControl.Visible=true;
				if (dhcnew.Floating)
				{
					FloatingForm ParentForm = this.ToplevelController.HostControl as FloatingForm;
					ParentForm.Text = dhcnew.HostControl.Text;
					ParentForm.SetCaption();
				}
				dhcnew.DockTab = null;
				dhcnew.AdjustLayout();
				this.ctrlDockTab.Visible = false;
				this.ctrlDockTab.Dispose();
				this.ctrlDockTab = null;
			}
		}

		internal void CheckFormOwnership( DockHostController dhc )
		{
			DockHostController dhcnext = null;

			foreach( DockTabPage page in this.TabControl.TabPages )
			{
				if( page.dhcClient != dhc )
				{
					dhcnext = page.dhcClient;
					break;
				}
			}

			if( this.ToplevelController.HostControl == dhc.InternalForm )
			{
				FloatingForm buf = dhcnext.InternalForm;
				dhcnext.InternalForm = dhc.InternalForm;
				dhc.InternalForm = buf;
			}
		}

		public override void InvokeDocking(DockControllerBase dc)
		{
			this.HostController.InvokeDocking(dc);
		}

		public override bool QueryRelationship(DCRelationship dcr)
		{
			// If the tabcontroller's relation value matches the queried dcr, then return true
			if( (this.currentRelationship.nRelation == dcr.nRelation) && (dcr.DP == DockPreference.Tabbed) )
				return true;
			return false;
		}

		public override bool AttemptDCRDocking(DockControllerBase dc, IEnumerator iedcr)
		{
			DockHostController host = dc as DockHostController;
			Debug.Assert((host != null), "Error: Invalid DockHostController.\n");

			// Iterate the enumerator and see if the dockhost shares a relation with this tabcontroller. If true, call InvokeDocking.
			iedcr.Reset();
			while(iedcr.MoveNext() == true)
			{
				DCRelationship dcr = iedcr.Current as DCRelationship;
				if( this.QueryRelationship(dcr) == true)
				{
					host.DINew = new DockInfo(this, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill, this.dockInfoCurrent.nPriority, dcr.nIndex,
																		DockPreference.Tabbed, Rectangle.Empty);
					this.InvokeDocking(host);
					return true;
				}
			}

			// None of the tabcontroller's children have a relation with this dockhost. So the dockhost was probably a sibling
			// of the tabcontroller.
			// See if the tab's hostcontroller has a sibling relationship with this dockhost. Tabchildren inherit all
			// of the tabcontroller's relations. This is functionally equivalent to the tabcontroller maintaining it's own
			// relation list.
			iedcr.Reset();
			while(iedcr.MoveNext() == true)
			{
				DCRelationship dcrtabsib = iedcr.Current as DCRelationship;
     			foreach (DockTabPage page in this.TabControl.TabPages)
        		{
            		if (page.dhcClient.QueryRelationship(dcrtabsib) == true)
            		{
              			host.DINew.dController = this.HostController;
              			host.DINew.DP = dcrtabsib.DP;
              			host.DINew.nDockIndex = dcrtabsib.nIndex;
              			this.HostController.InvokeDCRDocking(host, dcrtabsib);
              			return true;
            		}
        		}
			}
			return false;      
		}

		public override void InvokeDCRDocking(DockControllerBase dc, DCRelationship dcr)
		{
			Debug.Assert((this.currentRelationship.nRelation == dcr.nRelation), "Error: Invalid Relationship.\n");
			this.HostController.InvokeDocking(dc);
		}

		public override DockControllerBase RedockController(DockInfo di, bool bforcenew)
		{
			this.dockInfoNew = new DockInfo(di);
			if(this.dockInfoNew.dController.Floating == true)
				TransitFloatToDockInFloat();
			else
				TransitFloatToDock();
			if(di.DP != DockPreference.Tabbed)
			{
				return this;
			}
			else	// This tab will be destroyed and the children will be parented to the new tabcontroller. Return the hosttab.
			{
				// If the feedback controller is a dockhost, housed within a tabcontroller ie., will have a DockPreference of Tabbed,
				// then return the parent tabcontroller. Else, if the feedback is provided directly by a tabcontroller, as evident
				// by the non-tabbed DP, return it.
				if(di.dController.DICurrent.DP == DockPreference.Tabbed)
					return di.dController.ParentController;
				else
					return di.dController;
			}
		}

		protected internal virtual FloatingForm CreateFloatingFrame(Point ptlocation)
		{
			// Create a new floating form and parent the tabcontrol and all tabpage clients to the form
			foreach( DockTabPage page in this.TabControl.TabPages )
			{
				page.dhcClient.PreviousDockSize = this.PreviousDockSize;
				page.dhcClient.PreviousFloatSize = this.PreviousFloatSize;
				page.dhcClient.StoredDockSizes = this.StoredDockSizes;
			}
			FloatingForm frmfloat;
			if( this.SharedForm != null && SharedForm.Used
				&& this.InternalFloatWrapper != null )
			{
				SizingController sc = this.InternalFloatWrapper.ParentController as SizingController;
				this.InternalFloatWrapper.ParentController.ReplaceChild(this.InternalFloatWrapper, this);
				this.InternalFloatWrapper = null;
				frmfloat = this.SharedForm;
			}
			else
			{
				if( this.HostController.InternalForm != null && !this.HostController.InternalForm.IsDisposed )
				{
					frmfloat = this.HostController.InternalForm;
					frmfloat.Enabled = true;
					this.DockingManager.AddFFController(frmfloat.InternalController as FloatingFormController);
					frmfloat.Used = true;
				}
				else
				{
					frmfloat = DockingManager.CreateFloatingForm();
					this.HostController.InternalForm = frmfloat;
				}

				Syncfusion.Runtime.InteropServices.NativeMethods.MoveWindow(frmfloat.Handle,
				ptlocation.X, ptlocation.Y, 0, 0, false);
				if( ( this.dockInfoNew.rcDockArea.Width <= 0 || this.dockInfoNew.rcDockArea.Height <= 0 ) == true )
					frmfloat.ClientSize = this.HostControl.Bounds.Size;
				else
					frmfloat.Size = this.dockInfoNew.rcDockArea.Size;
				if (this.dockingMgr.bLoadVisibility == true)
				{
					frmfloat.Enable();
				}
			}

			foreach( DockTabPage page in this.TabControl.TabPages )
			{
				page.dhcClient.SharedForm = null;
			}

			FloatingFormController ffc = frmfloat.InternalController as FloatingFormController;
			ffc.ClearSharedFormReferences(ffc.dcChild);

			ffc.AddChild(this, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill);
			frmfloat.Controls.Add(this.HostControl);
			ffc.RefreshFormCaption();
			ffc.AdjustLayout();

			if( DockingManager.VisualStyle != VisualStyle.Default 
				&& DockingManager.VisualStyle != VisualStyle.VS2005 )
			{
				NativeMethodsHelper.RedrawWindow( frmfloat.Handle, NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_FRAME );
			}

			return frmfloat;
		}

		public override void EnterAutoHideMode()
		{
			this.TempWrapper.Minimized = this.Minimized;
			MainFormController mfctrlr = this.dcHostController.ToplevelController as MainFormController;
			this.ParentController.ControllerChanged += new ControllerChangedEH(this.TransientControllerChanged);
			this.dockInfoTransient = new DockInfo(this.ParentController, this.dockInfoCurrent.dStyle, this.dockInfoCurrent.nPriority,
					this.ParentController.GetChildHostIndex(this), this.DICurrent.DP, this.DICurrent.rcDockArea);
			this.bInAutoHide = true;
			CheckDCRelationship();
			foreach(DockTabPage page in this.ctrlDockTab.TabPages)
				page.dhcClient.bInAutoHide = true;
			mfctrlr.EnterAutoHideMode(this, true);
			foreach( DockTabPage page in this.ctrlDockTab.TabPages )
			{
				page.dhcClient.DINew.rcDockArea = this.DINew.rcDockArea;	// The TabController's DINew.rcDockArea is set within the mfctrlr.EnterAutoHide() call.
				page.dhcClient.PreviousDockSize = this.PreviousDockSize;
				page.dhcClient.StoredDockSizes = this.StoredDockSizes;
			}

			// Get hold of the sibling DragSplitterController through the parent SizingController and
			// subscribe each child DockHostController the splitter's DragSplitterMoved event
			DockControllerBase splitterdc = this.ParentController.GetChildAt(1);
			DragSplitter splitter = splitterdc.HostControl as DragSplitter;
			Debug.Assert(splitter != null);
			foreach(DockTabPage page in this.ctrlDockTab.TabPages)
				splitter.DragSplitterMoved += new SplitterEventHandler(page.dhcClient.DHCDragSplitterMoved);

			mfctrlr.AdjustLayout();
			if( DockingManager.AHInViewTab != null )
				DockingManager.AHInViewTab.HideController( this, true, true );

			ExitMaxMinState();
		}

		public override void ExitAutoHideMode(bool bcloseonexit)
		{
			MainFormController mfctrlr = this.dcHostController.ToplevelController as MainFormController;
			this.dockInfoTransient.dController.ControllerChanged -= new ControllerChangedEH(this.TransientControllerChanged);
			this.bInAutoHide = false;
			// Workaround: if the size of LayoutRect of page.dhcClient is (0,0), 
			// set it equal to TabControl's LayoutRect in order to prevent its disappearing after autohide
			DockTabPage firstPage = this.TabControl.TabPages[0] as DockTabPage;
			if( firstPage != null )
			{
				firstPage.dhcClient.LayoutRect = this.LayoutRect;
			}
			foreach(DockTabPage page in this.ctrlDockTab.TabPages)
			{
				page.dhcClient.bInAutoHide = false;
				page.dhcClient.HostControl.Region = null;
			}
			
			mfctrlr.ExitAutoHideMode(this, bcloseonexit);
			mfctrlr.AdjustLayout();
		}
		private bool m_bCanSwitchTabs = false;

		protected internal void HandleMouseDownImp(MouseButtons button, Point ptclient)
		{
			if(this.dockingMgr.DesignProcess == false && this.dockingMgr.DragProviderStyle == DragProviderStyle.Standard)
				this.ctrlDockTab.Capture = true;

			int ntab = this.ctrlDockTab.GetTabHitIndex(ptclient);
			if(ntab >= 0)
			{
				m_bCanSwitchTabs = true;
				m_nHitTab = ntab;
				DockHost dhost = this.HostController.HostControl as DockHost;
				// In design mode, set the docked control as the selected component, in place of assigning focus.
				if(this.dockingMgr.DesignMode == true)
				{
					Control dockctrl = (this.ctrlDockTab.TabPages[ntab] as DockTabPage).dhcClient.HostControl.Controls[0];
					Control selctrl = this.dockingMgr.GetPrimarySelection() as Control;
					if( (selctrl == null) || (selctrl.Equals(dockctrl) == false) )
					{
						Control[] ctrl = new Control[] {dockctrl};
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
						this.dockingMgr.SetSelectedComponents(ctrl, SelectionTypes.Auto);
#else
						this.dockingMgr.SetSelectedComponents(ctrl, SelectionTypes.MouseDown);
#endif
						this.bAllowDrag = false;
					}
					dockctrl.Focus();
				}
				else if(this.dockingMgr.DesignProcess == false)
				{
					dhost.Focus();
				}

				if( button == MouseButtons.Left )
				{
					// Fire the DragAllow event, and provide a chance to preempt the drag
					DragAllowEventArgs dragallow = new DragAllowEventArgs( this.HostControl.Controls[0] );
					this.dockingMgr.FireDragAllowEvent( dragallow );
					if( dragallow.Cancel == false )
					{
						this.dockingMgr.DragProvider.SingleTabOperate = true;
						this.dockingMgr.DragProvider.ProcessMouseDown( this, dhost, ctrlDockTab.PointToScreen( ptclient ) );
						dhost.DragRectangle = dhost.Bounds;
						this.bAllowDrag = true;
					}
				}

				if( ctrlDockTab.SelectedIndex != ntab )
				{
					ctrlDockTab.SelectedIndex = ntab;
				}
			}
		}

		protected internal void HandleMouseUpImp(MouseButtons button, Point ptclient)
		{
			m_bCanSwitchTabs = false;

			if(this.bAllowDrag == true)
				this.bAllowDrag = false;

			if( button != MouseButtons.Left )
			{
				if( button == MouseButtons.Right )
					this.dockingMgr.ShowMenu( this.HostController, this.TabControl.PointToScreen( ptclient ), false );
				return;
			}

			if((this.dockingMgr.DesignProcess == false) && (this.ctrlDockTab.Capture == true))
				this.ctrlDockTab.Capture = false;

			if(this.dragTabPage != null)
			{
				DockHostController dhcdrag = this.dragTabPage.dhcClient;

				if( this.dockingMgr.DragProvider.DraggingControl == ( dhcdrag.HostControl as DockHost ) )
				{
					if( ( this.dockingMgr.DisallowFloating || !dhcdrag.AllowFloating ) 
						&& ( dhcdrag.DINew.dController == null ) )
					{
						( dhcdrag.HostControl as DockHost ).AbortDrag();
						this.dragTabPage = null;
						return;
					}

                    if( !this.ChildControllers.Contains( dhcdrag ) )
                        dhcdrag.DockTab = null;
				}

				dockingMgr.DragProvider.ProcessMouseUp(this, dhcdrag.HostControl as DockHost, ctrlDockTab.PointToScreen(ptclient));
				this.dragTabPage = null;
			}
			else
			{
				DockHost dhost = this.HostController.HostControl as DockHost;
				this.dockingMgr.DragProvider.ProcessMouseUp(this, dhost, ctrlDockTab.PointToScreen(ptclient));
			}
			if(this.ctrlDockTab != null)
				this.ctrlDockTab.nSwitchIndex = -1;
			this.dockingMgr.DragProvider.SingleTabOperate = false;
		}

		internal override bool AllowFloating
		{
			get
			{
				bool allowFloating = true;

				foreach( DockTabPage page in this.TabControl.TabPages )
				{
					if( !page.dhcClient.AllowFloating )
					{
						allowFloating = false;
						break;
					}
				}

				return allowFloating;
			}
			set
			{}
		}

		internal override bool FreezeResize
		{
			get
			{
				bool freeze = false;

				if( this.TabControl != null )
					foreach( DockTabPage dtp in this.TabControl.TabPages )
					{
						if( dtp.dhcClient != null && dtp.dhcClient.FreezeResize )
						{
							freeze = true;
							base.MustResize = dtp.dhcClient.MustResize;
							break;
						}
					}
				
				return freeze;
			}
			set
			{	
				if( value )
				{
					foreach( DockHostController dhc in m_freezedPages )
						dhc.FreezeResize = true;

					this.MustResize = Direction.None;
				}
				else
				{
					m_freezedPages.Clear();

					foreach( DockTabPage page in this.TabControl.TabPages )
					{
						if( page.dhcClient.FreezeResize )
						{
							m_freezedPages.Add(page.dhcClient);
							page.dhcClient.FreezeResize = value;
						}
					}

					this.MustResize = Direction.Both;
				}
			}
		}
		private ArrayList m_freezedPages = new ArrayList();

		protected internal void HandleMouseMoveImp(MouseButtons button, Point ptclient)
		{
			if(button == MouseButtons.Left)
			{
				if(this.dragTabPage != null)	// A tab drag is in place
				{
					if( (this.ctrlDockTab.TabPages.Count > 0) && (this.dragTabPage == this.ctrlDockTab.SelectedTab))
					{
						// Store the tabcontroller's DICurrent info within the dockhost's dicurrent info. This will be used
						// after the dragdock operation to setup the dockhost's diprevious info. The DIPrevious update and
						// the parentcontroller subscription cannot take place here as this is conditional upon the docktarget
						// at the end of the drag-dock operation.
						this.dragTabPage.dhcClient.DICurrent = new DockInfo(this.ParentController, this.dockInfoCurrent.dStyle, this.dockInfoCurrent.nPriority,
							this.ParentController.GetChildHostIndex(this), this.dockInfoCurrent.DP, this.dockInfoCurrent.rcDockArea);
					}

					Point ptscreen = this.ctrlDockTab.PointToScreen(ptclient);

					DockHost dhost = this.dragTabPage.dhcClient.HostControl as DockHost;
					this.dockingMgr.DragProvider.ProcessMouseMove(this, dhost, ptscreen);
				}
				else
				{
					if(this.ctrlDockTab.ClientRectangle.Contains(ptclient) == true)
					{
						int nindex = this.ctrlDockTab.GetTabHitIndex(ptclient);
						Rectangle rcselected = this.ctrlDockTab.GetTabRect(this.ctrlDockTab.SelectedIndex);

						if( (this.ctrlDockTab.nSwitchIndex == -1) ||
							((rcselected.Contains(ptclient) == true) && (this.ctrlDockTab.nSwitchIndex != this.ctrlDockTab.SelectedIndex)) )
							this.ctrlDockTab.nSwitchIndex = this.ctrlDockTab.SelectedIndex;

						if( (nindex >= 0) && (nindex != this.ctrlDockTab.SelectedIndex) )
						{
							// If the tab being dragged is smaller than the adjacent tab switching the tabs will cause
							// the two to revert back to the original positions during the subsequent mousemove.
							// To prevent this, we verify to see whether the mouse is beyond the leading or trailing edge
							// of the larger tab to an extent equal to the dragged tab and if so, return without switching.
							Rectangle rcindex = this.ctrlDockTab.GetTabRect(nindex);
							bool bExit = false;
							if ( rcselected.Width < rcindex.Width && nindex == this.ctrlDockTab.nSwitchIndex )
							{
								if (this.ctrlDockTab.SelectedIndex > nindex)
								{
									bExit = dockingMgr.IsMirrored ?
										ptclient.X < rcindex.Right - rcselected.Width :
										ptclient.X > rcindex.Left + rcselected.Width;
								}
								else
								{
									bExit = dockingMgr.IsMirrored ?
										ptclient.X > rcindex.Left + rcselected.Width :
										ptclient.X < rcindex.Right - rcselected.Width;
								}

							}

							if (!bExit && DockingManager.AllowTabsMoving)
							{
								// Switch the selected tab with the new hitindex
								if( m_nHitTab != nindex && m_bCanSwitchTabs)
								{
									this.PauseActivation = true;
									DockTabPage selpage = this.ctrlDockTab.SelectedTab as DockTabPage;
									this.ctrlDockTab.TabPages[this.ctrlDockTab.SelectedIndex] = this.ctrlDockTab.TabPages[nindex] as DockTabPage;
									UpdateTabsOrder( m_nHitTab , nindex);
									this.ctrlDockTab.TabPages[nindex] = selpage;
									this.ctrlDockTab.SelectedIndex = nindex;
									this.PauseActivation = false;
									m_nHitTab = nindex;
								}
								else
								{
									this.ctrlDockTab.SelectedIndex = nindex;
								}

								this.ctrlDockTab.Update();
							}
						}
					}
					else if(this.bAllowDrag == true)	// Start a tab drag operation making the selected tab as the drag tab
					{
						this.dragTabPage = this.ctrlDockTab.SelectedTab as DockTabPage;
					}
				}
			}
		}

		protected internal void UpdateTabsOrder( int oldIndex, int newIndex )
		{
			if( this.TempWrapper != null )
			{
				ArrayList tabs = this.TempWrapper.DockRelationControllers;
				if( tabs != null && tabs.Count == this.ctrlDockTab.TabCount )
				{
					DockHostController dhc = tabs[oldIndex] as DockHostController;
					tabs.RemoveAt( oldIndex );
					tabs.Insert( newIndex, dhc );
				}
                this.TabControl.OnTabsOrderChanged();
               
			}
		}

		protected internal void HandleDoubleClickImp(Point ptclient)
		{
			if((this.dockingMgr.DisallowFloating || !this.HostController.AllowFloating) && (this.Floating == false))
				return;

			if(this.ctrlDockTab.GetTabHitIndex(ptclient) >= 0)
			{
				// Fire the DragAllow event, and provide a chance to preempt the drag
				DragAllowEventArgs dragallow = new DragAllowEventArgs(this.HostControl.Controls[0]);
				this.dockingMgr.FireDragAllowEvent(dragallow);
				if(dragallow.Cancel == false)
				{
					this.RemoveDockHostFromTab(this.HostController, true);
					this.dockingMgr.DragProvider.ProcessDoubleClick();
					// Forcibly update the designer state
					if(this.dockingMgr.DesignMode == true)
						this.dockingMgr.UpdateDesigner();
					this.dockingMgr.UpdateFloatingFormsImages();
				}
			}
		}

		public override Size MinimumSize
		{
			get
			{
				// Enumerate the DockHosts in this tabbed group and return the most restrictive minimum width and height
				int minwidth = 0;
				int minheight = 0;

				if( ctrlDockTab != null )
				{
				foreach(DockTabPage tabpage in this.ctrlDockTab.TabPages)
				{
					if(tabpage.dhcClient != null)
					{
						if(tabpage.dhcClient.MinimumSize.Width > minwidth)
							minwidth = tabpage.dhcClient.MinimumSize.Width;
						if(tabpage.dhcClient.MinimumSize.Height > minheight)
							minheight = tabpage.dhcClient.MinimumSize.Height;
					}
				}
				}
				return new Size(minwidth, minheight);
			}
		}

		protected override void Dispose(bool bdisposing)
		{
			if((bdisposing == true) && (this.ctrlDockTab != null))
			{
				this.ctrlDockTab.SelectedIndexChanged -= new System.EventHandler(this.DockTab_SelectedIndexChanged);
				if(this.ctrlDockTab.TabPages != null)
				{
					foreach(DockTabPage page in this.ctrlDockTab.TabPages)
					{
						if( page.dhcClient != this.dcHostController && page.dhcClient != null )
							page.dhcClient.Dispose();
					}
				}
				if(this.dcHostController != null)
					this.dcHostController.Dispose();
				this.ctrlDockTab.Dispose();
				this.ctrlDockTab = null;
			}
			base.Dispose(bdisposing);
		}
		
		internal override void AddWrapper(ControllerWrapper cw)
		{
			DockTabControllerWrapper dtcw = new DockTabControllerWrapper();
			dtcw.LayoutRect = LayoutRect;
			
			foreach(DockTabPage dtp in TabControl.TabPages)
			{
				Control hostControl = dtp.dhcClient.HostControl;
				if( hostControl != null )
				{
					if( hostControl.Controls.Count > 0 )
					{
						Control control = hostControl.Controls[0];
						DockHostController dhc = DockingManager.GetDockHostController(control);
						if( dhc != null )
						{
							string tabPageName = dhc.UniqueName;
							dtcw.Controls.Add(tabPageName);
						}
					}
				}
			}
			foreach( DockHostController dhcChild in this.TempWrapper.DockRelationControllers )
			{
				if( dhcChild.bInAutoHide )
					dtcw.Controls.Add( dhcChild.UniqueName );
			}

			cw.Children.Add(dtcw);
		}
	
		internal override void StoreControllers(ArrayList controllers)
		{
			base.StoreControllers (controllers);

			this.ParentController.RemoveChild(this);
			controllers.Add(this);
		}

		internal override bool IsEqual(ControllerWrapper cw)
		{
			if( cw is DockTabControllerWrapper )
			{
				if( HostControl.Controls.Count > 0 )
				{
					DockHostController dhc = DockingManager.GetDockHostController(HostControl.Controls[0]);
					string tabPageName = dhc.UniqueName;
					DockTabControllerWrapper dtcw = cw as DockTabControllerWrapper;

					if( dtcw.Controls.Contains(tabPageName) ||
						dtcw.Controls.Contains("DockHost_"+tabPageName))
					{
						return true;
					}
					
					// Workaround. This code is needed to prevent incorrect 
					// loading UserControls

					Control control = HostControl.Controls[0];
					if( control is UserControl && dhc.UniqueName == "" )
					{
						dhc.UniqueName = NameGenerator.Generate(control);
						return true;
					}
					
					int count = 0;
					for( int i = 0; i < DockingManager.controllers.Count; i++ )
					{
						if( DockingManager.controllers[i] is DockTabController)
							count++;
					}
					
					if( count == 1 )
					{
						return true;
					}
					
				}
			}
			return false;
		}
		
		internal override void ResizeControllers(ControllerWrapper cw)
		{
			DockTabControllerWrapper dtcw = (DockTabControllerWrapper) cw;
			LayoutRect = dtcw.LayoutRect;
			DITransient.rcDockArea = Rectangle.Empty;
		}

		public override ArrayList ChildControllers
		{
			get
			{
				ArrayList child = new ArrayList();

				foreach( DockTabPage dtp in this.TabControl.TabPages )
				{
					child.Add( dtp.dhcClient );
				}

				return child;
			}
		}
		
		internal override bool IsFloatOnly()
		{
			foreach( DockTabPage tabPage in TabControl.TabPages )
			{
				if( tabPage.dhcClient.IsFloatOnly() )
					return true;
			}

			return false;
		}

		#region IResizable implementation
		
		public Size CalculateSize(Size parentSize, Size newParentSize)
		{
			return ControllerSizeCalculator.CalculateSize(this, parentSize, newParentSize);
		}

		public bool IsVerticallyResizable()
		{
			Direction resizeDir = this.MustResize;

			return Minimized == Minimization.None
				&& ( !this.FreezeResize || resizeDir == Direction.Vertical && resizeDir != Direction.None );
		}

		public bool IsHorizontallyResizable()
		{
			Direction resizeDir = this.MustResize;

			return Minimized == Minimization.None
				&& ( !this.FreezeResize || resizeDir == Direction.Horizontal && resizeDir != Direction.None );
		}

		#endregion

		internal override void SetAutohiddenControlSize( Size size )
		{
			Rectangle rectangle = Rectangle.Empty;
			switch( DICurrent.dStyle )
			{
				case DockingStyle.Left:
				case DockingStyle.Right:
				{
					rectangle = new Rectangle( DINew.rcDockArea.Location,
						new Size( size.Width, DINew.rcDockArea.Height ) );
					break;
				}

				case DockingStyle.Top:
				case DockingStyle.Bottom:
				{
					rectangle = new Rectangle( DINew.rcDockArea.Location,
						new Size( DINew.rcDockArea.Width, size.Height ) );
					break;
				}
			}

			DITransient.rcDockArea = rectangle;
			foreach( DockTabPage page in TabControl.TabPages )
			{
				DockInfo dockInfo = page.dhcClient.DINew;
				dockInfo.rcDockArea = rectangle;
			}
		}

		private void CheckDCRelationship()
		{
			DockTabControl tabControl = this.ctrlDockTab;
			foreach( DockTabPage page in tabControl.TabPages )
			{
				DCRelationship dcr = page.dhcClient.DCRCurrent;
				if( dcr != null && dcr.DP != DockPreference.Tabbed )
				{
					IEnumerator dcrList = page.dhcClient.DCR;
					dcrList.Reset();
					while( dcrList.MoveNext() )
					{
						DCRelationship relationship = dcrList.Current as DCRelationship;
						if( relationship.DP == DockPreference.Tabbed )
						{
							page.dhcClient.DCRCurrent = relationship;
							break;
						}
					}
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
				if( !this.FreezeResize )
				{
					base.Minimized = value;

					foreach( DockTabPage page in TabControl.TabPages )
					{
						DockHostController dhc = page.dhcClient;
						Minimization wasMinimized = dhc.Minimized;
						dhc.Minimized = value;

						if( wasMinimized != Minimization.None && value == Minimization.None )
						{
							ControlRestoredEventArgs args = new ControlRestoredEventArgs( dhc.HostControl.Controls[0]
								, ControlSizeStates.Minimize );
							this.dockingMgr.FireControlSizeStateChanged( ControlSizeStates.Restore, args );
						}
						else if( wasMinimized == Minimization.None && value != Minimization.None )
						{
							ControlMinimizedEventArgs args = new ControlMinimizedEventArgs( dhc.HostControl.Controls[0] );
							this.dockingMgr.FireControlSizeStateChanged( ControlSizeStates.Minimize, args );
						}
					}

					if( !this.AutoHideMode )
						this.TabControl.Visible = value == Minimization.None;
				}
			}
		}

		protected DockHostController FormOwner
		{
			get
			{
				foreach( DockTabPage page in this.TabControl.TabPages )
				{
					if( page.dhcClient.InternalForm != null &&
						page.dhcClient.InternalForm.Visible &&
						page.dhcClient.InternalForm.Enabled )
						return page.dhcClient;
				}
				return this.HostController;
			}
		}

		internal override FloatingForm InternalForm
		{
			get
			{
				return FormOwner == null ? null : FormOwner.InternalForm;
			}
			set
			{
				
				FormOwner.InternalForm = value;
			}
		}

		/// <summary>
		/// Recreates wrapper for current controller.
		/// </summary>
		private void RecreateWrapper()
		{
			DockStateControllerWrapper wrap = new DockStateControllerWrapper(this.dockingMgr, this);
			wrap.ParentController = this.ParentController;
			this.ParentController.ChildWrapper = wrap;
			this.SetChildWrapper(wrap);
			this.ParentController.ChildWrapper = wrap;
		}

		/// <summary>
		/// Gets type of current transit operation.
		/// </summary>
		/// <returns></returns>
		private TransitType GetTransitType( DockHostController dhcdrag )
		{
			if(this.Floating==false&&((dhcdrag.DINew.dController==null)||(dhcdrag.DINew.dController.Floating==true)))
				return TransitType.DockToFloat;
			else if((this.Floating==true)&&((dhcdrag.DINew.dController!=null)&&(dhcdrag.DINew.dController.Floating==false)))
				return TransitType.FloatToDock;
			else
				return TransitType.FloatToFloat;
		}

		public override void ApplyDockInfo()
		{
			base.ApplyDockInfo ();

			if( this.dragTabPage != null )
			{
				TransitType transitType = GetTransitType(this.dragTabPage.dhcClient);
				
				if(transitType!=TransitType.FloatToFloat)
					RecreateWrapper();
				// Remove the docktab from the tabcontrol. We do this here only for the drag effect.
				// The actual tab reactivation takes place within the MouseUp event.
				if( this.ctrlDockTab.TabPages.Contains(this.dragTabPage) )
				{
					this.dockingMgr.SetRelativeSize((this.dragTabPage as DockTabPage).dhcClient);
					this.PauseActivation = true;
					this.ctrlDockTab.TabPages.Remove(this.dragTabPage);
					this.PauseActivation = false;
					this.ctrlDockTab.Update();
				}

				DockHostController dhcdrag = this.dragTabPage.dhcClient;
				bool freezed = this.dockingMgr.ForbidFreeze;
				this.dockingMgr.ForbidFreeze = true;
				DockHostController dhcnext = ( this.ctrlDockTab.SelectedTab as DockTabPage ).dhcClient;
				if( this.ToplevelController.HostControl == dhcdrag.InternalForm )
				{
					FloatingForm buf = dhcnext.InternalForm;
					dhcnext.InternalForm = dhcdrag.InternalForm;
					dhcdrag.InternalForm = buf;
				}
				if(this.ctrlDockTab.TabPages.Count > 1)
				{
					this.SetHostCtrlForSelection();
				}
				else
					UpdateTabPages();

				this.dragTabPage = null;

				Control[] ctrls = new Control[] {dhcdrag.HostControl.Controls[0]};
				this.dockingMgr.FireDockStateChangeEvent("DockStateChanging", new DockStateChangeEventArgs(ctrls));

				// If a tab is being dragged out of a tabcontroller docked to the mainframe and floated or docked to a
				// floating controller, then update the diprevious info. Similarly, if the dockhost is being dragged out
				// of a floating controller and docked to the mainframe, update the diprevious info.
				if( transitType == TransitType.DockToFloat )
				{
					if(dhcdrag.DIPrevious.dController != null)
						dhcdrag.DIPrevious.dController.ControllerChanged -= new ControllerChangedEH(dhcdrag.dhc_ControllerChanged);
					this.ParentController.ControllerChanged += new ControllerChangedEH(dhcdrag.dhc_ControllerChanged);
					if(dhcdrag.DIPrevious.rcDockArea.Size.IsEmpty == false)
					{
						(dhcdrag.HostControl as DockHost).DragRectangle = dhcdrag.DIPrevious.rcDockArea;
					}
					dhcdrag.DIPrevious = new DockInfo(this.ParentController, this.dockInfoCurrent.dStyle,
						this.dockInfoCurrent.nPriority, this.ParentController.GetChildHostIndex(this),
						this.dockInfoCurrent.DP, this.dockInfoCurrent.rcDockArea);
					dhcdrag.Floating = true;
				}
				else if( transitType == TransitType.FloatToDock  )
				{
					if(dhcdrag.DIPrevious.dController != null)
						dhcdrag.DIPrevious.dController.ControllerChanged -= new ControllerChangedEH(dhcdrag.dhc_ControllerChanged);
					this.ParentController.ControllerChanged += new ControllerChangedEH(dhcdrag.dhc_ControllerChanged);
					dhcdrag.DIPrevious = new DockInfo(this.ParentController, this.dockInfoCurrent.dStyle,
						this.dockInfoCurrent.nPriority, this.ParentController.GetChildHostIndex(this),
						this.dockInfoCurrent.DP, this.dockInfoCurrent.rcDockArea);
					dhcdrag.Floating = false;
				}
				else
				{
					if( this.ParentController is FloatingFormController )
					{
						FloatingForm parentForm = (FloatingForm)this.ParentController.HostControl;
						(dhcdrag.HostControl as DockHost).DragRectangle = new Rectangle( parentForm.Location, parentForm.Size );
					}
					if( null != this.ctrlDockTab )
						RecreateWrapper();
					else
					{
						if( null != dhcnext.InternalFloatWrapper )
							dhcnext.InternalFloatWrapper.Relations.Clear();
					}
				}

				if(dhcdrag.DINew.dController == null )
				{
					// Refloating the controller, erase the old floatdcrlist
					dhcdrag.FloatDCRList.Clear();
					if( this.Floating )
						dhcdrag.InternalFloatWrapper = null;
					dhcdrag.CreateFloatingFrame(DockingManager.HostControl.
						PointToScreen(dhcdrag.HostControl.Location), true);
						
					this.dockingMgr.UpdateFloatingFormsImages();

					dhcdrag.Floating = true;
					dhcdrag.AdjustLayout();
				}
				else
				{
					// If controller is being redocked within a mainframe, then clear the dockdcr list
					if( (this.Floating == false) && (dhcdrag.DINew.dController.Floating == false) )
						dhcdrag.DockDCRList.Clear();

					if((this.dockingMgr.DockToFill) && (dhcdrag.DINew.dController == this.dockingMgr.dcHostForm)
						&& (dhcdrag.DINew.dController.ChildCount == 1))
					{
						// Special case where the client form is fully consumed by a DockTabController
						DockControllerBase dsctrlr = dhcdrag.DINew.dController.GetChildAt(0).GetChildAt(0);
						if(dsctrlr is DockStateControllerBase)	// Either a tabcontroller or a dockhostcontroller
						{
							dhcdrag.DINew.dController = dsctrlr;
							switch(dhcdrag.DINew.dStyle)
							{
								case DockingStyle.Left:
									dhcdrag.DINew.nDockIndex = 0;
									dhcdrag.DINew.DP = dsctrlr.DICurrent.DP;
									break;
								case DockingStyle.Top:
									dhcdrag.DINew.nDockIndex = 0;
									dhcdrag.DINew.DP = dsctrlr.DICurrent.DP;
									break;
								case DockingStyle.Right:
									dhcdrag.DINew.nDockIndex = dsctrlr.ChildCount;
									dhcdrag.DINew.DP = dsctrlr.DICurrent.DP;
									break;
								case DockingStyle.Bottom:
									dhcdrag.DINew.nDockIndex = dsctrlr.ChildCount;
									dhcdrag.DINew.DP = dsctrlr.DICurrent.DP;
									break;
								default:
									Debug.Assert(false, "Invalid DINew value");
									break;
							}
							dsctrlr.InvokeDocking(dhcdrag);
						}
						else
							dhcdrag.DINew.dController.InvokeDocking(dhcdrag);
					}
					else
					{
						dhcdrag.DINew.dController.InvokeDocking(dhcdrag);
					}
				}
				if(this.dockingMgr.DesignProcess == false)
					dhcdrag.HostControl.Focus();

				this.dockingMgr.FireDockStateChangeEvent("DockStateChanged", new DockStateChangeEventArgs(ctrls));

				// Forcibly update the designer state
				if(this.dockingMgr.DesignMode == true)
					this.dockingMgr.UpdateDesigner();

				this.dockingMgr.ForbidFreeze = freezed;
				this.dockingMgr.DragProvider.SingleTabOperate = false;
			}
			else
			{
				this.dragTabPage = null;
				// The taborder may have changed. Update the tabbed relation indices
				foreach(DockTabPage tabpage in this.ctrlDockTab.TabPages)
				{
					tabpage.dhcClient.DCRCurrent.nIndex = this.ctrlDockTab.TabPages.IndexOf(tabpage);
				}
			}
		}

		public override void DockAsMDIChild()
		{
			ArrayList pages = new ArrayList(TabControl.TabPages);
			Control focused = null;

			foreach( DockTabPage page in pages )
			{
				if( page.Visible )
				{
					focused = page.dhcClient.HostControl.Controls[0];
					pages.Remove(page);
					pages.Insert(0 , page);
					break;
				}
			}

			foreach( DockTabPage page in pages )
			{
				DockHostController hostController = page.dhcClient;
				dockingMgr.SetAsMDIChild( hostController.HostControl.Controls[0], true );
			}

			if( focused != null )
			{
				Form parentForm = focused.Parent as Form;

				if( parentForm != null )
					parentForm.Activate();
			}
		}

		public override void UpdateControl()
		{
			ctrlDockTab.UpdateRenderer();
			AdjustLayout();
		}
	}

	[
		ToolboxItem(false),
		DesignTimeVisible(false)
	]
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DockTabPage : TabPageAdv
	{
		public DockHostController dhcClient = null;
		protected static int nNamingCount = 0;

		public DockTabPage(DockHostController dhc, String text, int imgindex) : base()
		{
			this.dhcClient = dhc;
			this.Text = text;
			this.ImageIndex = imgindex;

			DockTabPage.nNamingCount++;
			this.Name = String.Concat("DockTabPage_", DockTabPage.nNamingCount.ToString());
		}

		// Do not create the tabpage window. Tabpages merely serve as placeholders and handle creation is not required.
		protected override void CreateHandle()
		{
		}

		protected override void Dispose(bool bdisposing)
		{
			if((bdisposing == true) && (this.dhcClient != null))
			{
				this.dhcClient = null;
			}
			base.Dispose(bdisposing);
		}
	}


	[
		ToolboxItem(false),
		DesignTimeVisible(false)
	]
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DockTabControl : TabControlAdv, IDockingManagerDesignerMouseHook
	{
		protected DockTabController dcInternal = null;
		protected internal int nSwitchIndex = -1;	// Used in the drag switching overlap offset implementation
		protected ThemedControlDrawing tdTab = null;
		private DockTabPage m_closingPage = null;
		private const int DEF_MIN_IMAGE_HEIGHT = 20;

		protected static int nNamingCount = 0;

		public DockStateControllerBase InternalController
		{
			get { return this.dcInternal; }
		}

		public ThemedControlDrawing ThemeDraw
		{
			get { return this.tdTab; }
		}

		public DockTabControl(DockingManager dmgr, DockHostController dhc)
		{
			AllowDrop = true;
			InitializeDockTabControl(dmgr, dhc);
			DockTabControl.nNamingCount++;
			this.Name = String.Concat("DockTabControl_", DockTabControl.nNamingCount.ToString());
		}

		protected virtual void InitializeDockTabControl(DockingManager dmgr, DockHostController dhc)
		{
			this.dcInternal = new DockTabController(dmgr, this);
			// Whenever a new tab control is being created, the tabcontroller replaces the dockhostcontroller
			// by itself within the dockhost's parent hierarchy. This allows the tabcontroller to takeover the
			// other controller's position within the layout framework.
			dhc.ParentController.ReplaceChild(dhc, this.dcInternal);
			dhc.ParentController = this.dcInternal;
			this.dcInternal.HostController = dhc;
			this.Alignment = (TabAlignment)Enum.Parse(typeof(DockTabAlignmentStyle),dmgr.DockTabAlignment.ToString(),true);
			this.SizeMode = TabSizeMode.ShrinkToFit;
			this.TextAlignment = System.Drawing.StringAlignment.Near;
			this.TextLineAlignment = System.Drawing.StringAlignment.Near;
			this.Font = dmgr.DockTabFont;
			this.ActiveTabFont = this.Font;
			this.ImageList = this.dcInternal.DockingManager.ImageList;
			this.Multiline = false;
			this.SetStyle(ControlStyles.Selectable, false);
			this.FocusOnTabClick = false;
			this.UseMnemonic = false;

			this.ItemSize = new Size(0, dmgr.DockTabHeight);
			(this.Renderer as SingleLineTabPanelRenderer).PadX = 5.0F;

			if( this.dcInternal.DockingManager.DockTabHeight > DEF_MIN_IMAGE_HEIGHT )
				this.Renderer.ForceDrawImage = true;

			Type rendererType;
			if(XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.dcInternal.DockingManager.ThemesEnabled)
			{
				rendererType = typeof(DockTabThemedRenderer);
				this.HotTrack = true;
			}
			else
			{
				rendererType = typeof(TabRenderer2D);
			}

			if( InternalController.DockingManager.Renderer.VisualStyle != VisualStyle.Default )
			{
				rendererType = GetRendererType();
			}

			this.TabStyle = rendererType;
			this.Padding = new Point(3,2);

			if(XPThemes.IsThemedOS)
				this.tdTab = new ThemedControlDrawing(ThemedControls.TAB);

			dmgr.ImageListChanged += new EventHandler(this.dockingManager_ImageListChanged);

			this.RightToLeft = 	dmgr.RightToLeft;
			dcInternal.DockEdge = dhc.DockEdge;
			this.ShowScroll = dmgr.ShowDockTabScrollButton;
		}

        /// </override>
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                if( base.Font != null )
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

		protected void UpdateOffice2007Theme()
		{
			if (this.Renderer.Renderers.Count > 0)
			{
				foreach (DockTabRendererOffice2007 dtrOffice2007 in this.Renderer.Renderers)
				{
					dtrOffice2007.Theme = dcInternal.DockingManager.Office2007Theme;
				}
			}
		}

		protected void dockingManager_ImageListChanged(Object obj, EventArgs e)
		{
			this.ImageList = this.dcInternal.DockingManager.ImageList;
		}

		public void InsertTab(int nindex, DockTabPage newpage)
		{
			nindex = (nindex < 0) ? 0 : nindex;
			nindex = (nindex > this.TabPages.Count) ? this.TabPages.Count : nindex;
			if(newpage.dhcClient != null)
			{
				newpage.dhcClient.DICurrent = new DockInfo(this.InternalController, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill, this.dcInternal.DICurrent.nPriority,
					nindex, DockPreference.Tabbed, new Rectangle(0,0,this.dcInternal.LayoutRect.Width,this.dcInternal.LayoutRect.Height));
				newpage.dhcClient.ParentController = this.InternalController;
				newpage.dhcClient.StoredSizes = this.InternalController.StoredSizes;
			

			bool bpauseset = this.dcInternal.PauseActivation ? true : false;
			this.dcInternal.PauseActivation = true;

			this.TabPages.Insert(nindex, newpage);

			if(bpauseset == false)
				this.dcInternal.PauseActivation = false;
            }
			// Readjust the DICurrent index info for all tabs
			int index = 0;
			foreach(DockTabPage page in this.TabPages)
			{
				if(page.dhcClient != null)
					page.dhcClient.DICurrent.nDockIndex = index++;
			}
		}

		public void AddTab(DockTabPage newpage)
		{
			if(newpage.dhcClient != null)
			{
				newpage.dhcClient.DICurrent = new DockInfo(this.InternalController, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill, this.dcInternal.DICurrent.nPriority,
					this.TabPages.Count, DockPreference.Tabbed, new Rectangle(0,0,this.dcInternal.LayoutRect.Width,this.dcInternal.LayoutRect.Height));
				newpage.dhcClient.ParentController = this.InternalController;
			}
			this.TabPages.Add(newpage);
		}

		// This hostcontroller calls this method, when it's parent has changed. Reparent remaining tab dockclients to the
		// hostcontroller's new parent
		public void ReparentTabChildren()
		{
			Control newparent = this.dcInternal.HostControl.Parent;
			foreach(DockTabPage page in this.TabPages)
			{
				if(page.dhcClient.HostControl.Parent != newparent)
					newparent.Controls.Add(page.dhcClient.HostControl);
			}
		}

		protected override void OnPaint(PaintEventArgs e )
		{
			if (dcInternal.DockingManager.VisualStyle == VisualStyle.Office2007
				|| dcInternal.DockingManager.VisualStyle == VisualStyle.Office2007Outlook)
			{
				UpdateOffice2007Theme();
			}

			base.OnPaint(e);
		}

 		/// <summary>
        /// Indicates the Scroll button whether should display or not
        /// </summary>
        public override bool ShowScroll
        {
            get
            {
                return base.ShowScroll;
            }
            set
            {
                base.ShowScroll = value;
                if (value == true)
                    this.SizeMode = Syncfusion.Windows.Forms.Tools.TabSizeMode.Normal;
                else
                    this.SizeMode = Syncfusion.Windows.Forms.Tools.TabSizeMode.ShrinkToFit;
            }
		}

        /// </override>
        protected override void OnStyleChanged()
        {
            base.OnStyleChanged();
            this.SetItemSize();
        }

        private void SetItemSize()
        {
            if( this.dcInternal != null && this.dcInternal.DockingManager != null )
            {
                int itemHeight = this.dcInternal.DockingManager.DockTabHeight;
                if( this.TabStyle == typeof( DockTabRendererOffice2007 ) )
                {
                    itemHeight++;
                }

                this.ItemSize = new Size( 0, itemHeight );
            }
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
			if(this.IsDisposed == false)
			{
				// Reset the hostcontrol's client as the selected control.
				Control selctrl = this.dcInternal.DockingManager.GetPrimarySelection() as Control;
				Control dockctrl = this.dcInternal.HostControl.Controls[0];
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
			Debug.Assert(false, "No implementation in DockTabControl.");
		}

		public bool GetDesignMode()
		{
			return this.dcInternal.DockingManager.DesignMode;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if((this.dcInternal != null) && (this.dcInternal.DockingManager.DesignProcess == false))
			{
				this.dcInternal.HandleMouseDownImp(e.Button, new Point(e.X, e.Y));
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
            base.OnMouseMove(e);
			if((this.dcInternal != null) && (this.dcInternal.DockingManager.DesignProcess == false))
			{
				IDraggable idg = this.dcInternal.DockingManager.DragProvider.DraggingControl;
                if (idg == null)
                    this.dcInternal.HandleMouseMoveImp(e.Button, new Point(e.X, e.Y));
                else if (this.dcInternal.dragTabPage != null)
                    this.dcInternal.HandleMouseMoveImp(MouseButtons.Left, new Point(e.X, e.Y));
            }           
        }

		protected override void OnDragOver(DragEventArgs drgevent)
		{
			int index = HitTestTabs( PointToClient(Cursor.Position) );
			if( index != -1 && index != SelectedIndex )
			{
				this.SelectedIndex = index;
			}
			base.OnDragOver (drgevent);
		}

		protected override void OnMouseUp( MouseEventArgs e )
		{
			DockTabPage page = this.TabPages[SelectedIndex] as DockTabPage;
			page.Closing += new TabPageAdvClosingEventHandler( page_Closing );

			base.OnMouseUp( e );
			this.dcInternal.HandleMouseUpImp( e.Button, new Point( e.X, e.Y ) );

			if( m_closingPage != null )
			{
				m_closingPage.dhcClient.CloseController();
				m_closingPage = null;
			}

			page.Closing -= new TabPageAdvClosingEventHandler( page_Closing );
		}

		private void page_Closing( object sender, TabPageAdvClosingEventArgs args )
		{
			m_closingPage = sender as DockTabPage;
			args.Cancel = true;
		}

		protected override void WndProc(ref Message msg)
		{
			if( (((msg.Msg == 0x0201 /*WM_LBUTTONDOWN*/) || (msg.Msg == 0x0202 /*WM_LBUTTONUP*/))
				&& (((int)msg.WParam == 0x0020 /*MK_XBUTTON1*/) || ((int)msg.WParam == 0x0040 /*MK_XBUTTON2*/)))
				|| ((msg.Msg == 0x020B /*WM_XBUTTONDOWN*/) || (msg.Msg == 0x020C /*WM_XBUTTONUP*/)) )
			{
				if((this.dcInternal != null) && (this.dcInternal.DockingManager.DesignMode == false) &&
					(this.dcInternal.DockingManager.DragProvider.DraggingControl != null)	)
					return;
			}

			base.WndProc(ref msg);

			if((this.dcInternal != null) && (this.dcInternal.DockingManager.DesignProcess == false))
			{
				if((msg.Msg == 0x0203) && ((int)msg.WParam == 0x0001) )	// WM_LBUTTONDBLCLK & MK_LBUTTON
				{
					int ptint = (int)msg.LParam;
					this.dcInternal.HandleDoubleClickImp(this.PointToClient(Cursor.Position));
				}
			}
		}

		public int GetTabHitIndex(Point pt)
		{
			return this.HitTestTabs(pt, false);
		}

		protected override void Dispose(bool bdisposing)
		{
			if((bdisposing == true) && (this.tdTab != null))
			{
				this.ImageList = null;
				if(this.dcInternal != null)
					this.dcInternal.DockingManager.ImageListChanged -= new EventHandler(this.dockingManager_ImageListChanged);

				this.tdTab.Dispose();
				this.tdTab = null;
			}
			base.Dispose(bdisposing);
		}

		public void UpdateRenderer()
		{
			TabStyle = GetRendererType();
			this.Invalidate();
		}

		internal Type GetRendererType()
		{
			Type rendererType = typeof( TabRenderer2D );
			switch( InternalController.DockingManager.Renderer.VisualStyle )
			{
				case VisualStyle.Default :
					if( dcInternal.DockingManager.ThemesEnabled && XPThemes.IsThemedOS &&
						XPThemes.IsThemeActive )
					{
						rendererType = typeof( DockTabThemedRenderer );
					}
					else
					{
						rendererType = typeof( TabRenderer2D );
					}
					break;
				case VisualStyle.Office2003 :
				case VisualStyle.OfficeXP :
					rendererType = typeof( TabRendererOffice2003 );
					break;
				case VisualStyle.VS2005:
                    rendererType = typeof(TabRendererDockingWhidbey);
					break;
				case VisualStyle.Metro:
                    rendererType = typeof(TabRendererDockingVS2012);
					break;
				case VisualStyle.Office2007:
				case VisualStyle.Office2007Outlook:
					rendererType = typeof( Renderers.DockTabRendererOffice2007 );
					 break;
			}
			return rendererType;
		}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class DockTabThemedRenderer : TabRenderer3D
	{
		public static new string TabStyleName
		{
			get{return "DockTabThemedRenderer";}
		}

		static DockTabThemedRenderer()
		{
			TabRendererFactory.RegisterTabType(TabStyleName, typeof(DockTabThemedRenderer), TabRenderer3D.TabPanelPropertyExtender );
		}

		public DockTabThemedRenderer(ITabControl parent, ITabPanelRenderer panelRenderer)
			: base(parent, panelRenderer)
		{
		}

		public override SizeF GetOverlapSize(SizeF tabSize)
		{
			return new SizeF(TabRenderer3D.OVERLAPX, 1);
		}

		protected override void DrawBackground(DrawTabEventArgs drawiteminfo)
		{
			Graphics gph = drawiteminfo.Graphics;
			Rectangle rctab = drawiteminfo.Bounds;
			DockTabControl tabctrl = this.TabControl.GetControl() as DockTabControl;
			if(drawiteminfo.Index == tabctrl.SelectedIndex)
			{
				// Draw the interior of the selected tab
				Rectangle rcseltabitem = new Rectangle(rctab.Left, rctab.Top-3, rctab.Width-1, rctab.Height+1);
				gph.SetClip(rcseltabitem);
				tabctrl.ThemeDraw.DrawThemeBackground(gph, ThemeParts.TABP_TOPTABITEM, ThemeStates.TTIS_SELECTED, rctab);
				gph.ResetClip();
			}
			else
			{
				// Draw the interior of the unselected tabs
				Rectangle rctabitem = new Rectangle(rctab.Left, rctab.Top-2, rctab.Width+1, rctab.Height);
				if(drawiteminfo.Index == tabctrl.SelectedIndex+1)
					rctabitem.X += 3;
				gph.SetClip(rctabitem);
				tabctrl.ThemeDraw.DrawThemeBackground(gph, ThemeParts.TABP_TABITEM, ThemeStates.TIS_NORMAL, rctab);
				gph.ResetClip();
			}
		}

		protected override void DrawBorders(DrawTabEventArgs drawiteminfo)
		{
			DockTabControl tabctrl = this.TabControl.GetControl() as DockTabControl;
			if((drawiteminfo.Index == tabctrl.SelectedIndex) || (drawiteminfo.State == DrawItemState.HotLight))
			{
				Graphics gph = drawiteminfo.Graphics;
				RectangleF curbounds = drawiteminfo.Bounds;
				Region oldClipRegion = gph.Clip;
				curbounds = AdjustBoundsAndGraphicsForAlignment(gph, curbounds, this.TabAlignment);
				PointF[] polygon = GetPolygonFromBounds(curbounds);

				Pen lightpen = new Pen(SystemColors.ControlLightLight);
				Pen darkpen = new Pen(SystemColors.ControlDarkDark);
				Pen graypen = new Pen(Color.DarkGray);

				gph.DrawLine(lightpen, polygon[0], polygon[1]);	// Left line
				gph.DrawLine(lightpen, polygon[1], polygon[2]);	// Top-left hatch
				gph.DrawLine(lightpen, polygon[2], polygon[3]);	// Top line
				gph.DrawLine(new Pen(SystemColors.ControlDarkDark), polygon[4], polygon[5]);	// Right line
				gph.DrawLine(graypen, new PointF(polygon[4].X-1, polygon[4].Y-1), new PointF(polygon[5].X-1, polygon[5].Y));	// right shade

				lightpen.Dispose();
				darkpen.Dispose();
				graypen.Dispose();

				// Draw the tab's theme highlight line
				Rectangle rctab = new Rectangle((int)curbounds.Left, (int)curbounds.Top, (int)curbounds.Width, (int)curbounds.Height);
				Rectangle rcseltabedge = new Rectangle(rctab.Left-2, rctab.Bottom-3, rctab.Width+4, 10);
				Point[] clippts = {
									  new Point(rctab.Left, rctab.Bottom-3), new Point(rctab.Right-1, rctab.Bottom-3),
									  new Point(rctab.Right-4, rctab.Bottom), new Point(rctab.Left+3, rctab.Bottom),
									  new Point(rctab.Left, rctab.Bottom-3)
								  };
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddPolygon(clippts);
                    using (Region leftcliprgn = new Region(path))
                    {
                        IntPtr hrgn = leftcliprgn.GetHrgn(gph);
                        IntPtr hdc = gph.GetHdc();
                        try
                        {
                            IntPtr hprevrgn = IntPtr.Zero;
                            NativeMethods.GetClipRgn(hdc, hprevrgn);
                            NativeMethods.SelectClipRgn(hdc, hrgn);
                            NativeMethods.RECT rectseltabedge = new NativeMethods.RECT(rcseltabedge);
                            NativeMethods.RECT rectclip = new NativeMethods.RECT(rcseltabedge);
                            NativeMethods.DrawThemeBackground(tabctrl.ThemeDraw.HTheme, hdc, ThemeParts.TABP_TOPTABITEM, ThemeStates.TTIS_SELECTED,
                                ref rectseltabedge, ref rectclip);
                            NativeMethods.SelectClipRgn(hdc, hprevrgn);
                        }
                        finally
                        {
                            gph.ReleaseHdc(hdc);                            
                        }
                    }
                }
				gph.SetClip(oldClipRegion, CombineMode.Replace);
			}
			else
			{
				base.DrawBorders(drawiteminfo);
			}
		}
	}
}
