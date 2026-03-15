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

namespace Syncfusion.Windows.Forms.Tools
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class SizingController : DockControllerBase, IResizable
	{
		// The priority is set while creating the controller. This cannot be changed
		protected ArrayList alChildren = new ArrayList();
		protected DockPreference dpOrder = DockPreference.Horizontal;

		protected Control ctrlHost = null;
		protected DockControllerBase dcPriority = null;
		protected Rectangle rcLayout = Rectangle.Empty;

		protected DCRelationship currentRelationship = null;
		protected ArrayList alDCRShared = new ArrayList();

		protected IEnumerator ieSizing = null;

		public override ArrayList ChildControllers
		{
			get
			{
				return this.alChildren;
			}
		}

		internal FloatingForm InternalForm
		{
			get
			{
				DockStateControllerBase baseController = null;
				ArrayList controls = this.GetDockControllers();

				if( controls.Count > 0 )
					baseController = controls[0] as DockStateControllerBase;

				if( baseController != null )
					return baseController.InternalForm;
				else
					return null;
			}
			set
			{
				DockStateControllerBase baseController = null;
				ArrayList controls = this.GetDockControllers();

				if( controls.Count > 0 )
					baseController = controls[0] as DockStateControllerBase;

				if( baseController != null )
					baseController.InternalForm = value;
			}
		}

		protected internal ArrayList GetDockControllers()
		{
			ArrayList dockControllers = new ArrayList();

			foreach( DockControllerBase controller in this.alChildren )
			{
				if( controller is SizingController )
					dockControllers.AddRange( ( controller as SizingController ).GetDockControllers() );
				else
				{
					if( (controller is DockHostController || controller is DockTabController) &&
						!controller.Deleting )
						dockControllers.Add( controller );
				}
			}

			if( dockControllers.Contains(null) )
				dockControllers.Remove(null);

			return dockControllers;
		}

		protected internal ArrayList GetResizableDockControllers()
		{
			ArrayList dockControllers = new ArrayList();

			foreach( DockControllerBase controller in this.alChildren )
			{
				if( controller is SizingController )
					dockControllers.AddRange(( controller as SizingController ).GetResizableDockControllers());
				else
				{
					if( ( controller is DockHostController || controller is DockTabController ) &&
						!controller.Deleting && !(controller as DockStateControllerBase).FreezeResize)
						dockControllers.Add(controller);
				}
			}

			if( dockControllers.Contains(null) )
				dockControllers.Remove(null);

			return dockControllers;
		}

		public DockControllerBase PriorityController
		{
			set { this.dcPriority = value; }
			get { return this.dcPriority; }
		}

		public override Control HostControl
		{
			get { return this.ctrlHost; }
		}

		public override Rectangle LayoutRect
		{
			get { return this.rcLayout; }
			set
			{
				if( this.Minimized == Minimization.Vertical )
					value.Size = new Size(value.Size.Width, RefreshMinimizedHeight());
				if( this.Minimized == Minimization.Horizontal )
					value.Size = new Size(RefreshMinimizedHeight(), value.Size.Height);
				Direction resize = this.CanResize;

				if( resize != Direction.Both && !this.IsHorizontallyResizable() )
					value.Size = new Size(this.LayoutRect.Width, value.Height);
				if( resize != Direction.Both && !this.IsVerticallyResizable() )
					value.Size = new Size(value.Width, this.LayoutRect.Height);

				if( value.Width < 0 )
					value.Width = 0;
				if( value.Height < 0 )
					value.Height = 0;

				if(this.rcLayout != value)
				{
					this.rcLayout = value;
				}
				if( (alChildren.Count > 0) && (!DockingManager.LockUpdates) )
				{
					this.AdjustLayout();
				}
			}
		}

		private int RefreshMinimizedHeight()
		{
			int height = 0;

			foreach( DockControllerBase child in this.alChildren )
			{
				if( this.Minimized == Minimization.Vertical )
				{
					if( this.DockingOrder == DockPreference.Vertical )
						height += child.LayoutRect.Height;
					else
						if( child.LayoutRect.Height > height )
							height = child.LayoutRect.Height;
				}
				else if( this.Minimized == Minimization.Horizontal )
				{
					if( this.DockingOrder == DockPreference.Horizontal )
						height += child.LayoutRect.Width;
					else
						if( child.LayoutRect.Width > height )
							height = child.LayoutRect.Width;
				}
			}

			return height;
		}

		public override DockInfo DICurrent
		{
			get { return this.dockInfoCurrent; }

			// The DockPreference order never changes for sizingcontroller
			set
			{
				this.dockInfoCurrent.dStyle = value.dStyle;
				this.dockInfoCurrent.dController = value.dController;
				this.dockInfoCurrent.nDockIndex = value.nDockIndex;
				this.dockInfoCurrent.DP = this.dpOrder;
				this.dockInfoCurrent.rcDockArea = value.rcDockArea;
			}
		}

		// Travel up the hierarchy till you reach a windowed controller
		public override bool Floating
		{
			get
            {
                bool floating = false;
                if( this.ParentController != null)
                    floating = this.ParentController.Floating;
                return floating;
            }
			set {}
		}

		public override int ChildCount
		{
			get { return this.alChildren.Count; }
		}

		public override int ChildHostCount
		{
			get
			{
				int i = 0;
				foreach(DockControllerBase dc in this.alChildren)
				{
					if((dc is DragSplitterController) == false)
						i++;
				}
				return i;
			}
		}

		public override IEnumerator DCR
		{
			get
			{
				return new IEnumWrapper(this.currentRelationship);
			}
		}

		public override IEnumerator ChildEnumerator
		{
			get { return this.alChildren.GetEnumerator(); }
		}

		public override DCRelationship DCRCurrent
		{
			get { return this.currentRelationship; }
			set { this.currentRelationship = value; }
		}

		public override IEnumerator ChildHostEnumerator
		{
			get
			{
				//Form an array that contains all the non-splitter children and return an enumerator for the collection
				DockControllerBase[] archildren = new DockControllerBase[this.ChildHostCount];
				int i = 0;
				foreach(DockControllerBase dc in this.alChildren)
				{
					if((dc is DragSplitterController) == false)
						archildren[i++] = dc;
				}
				return archildren.GetEnumerator();;
			}
		}

		public SizingController(DockingManager mgr, Control ctrl, DockPreference dp) : base(mgr)
		{
			this.ctrlHost = ctrl;
			this.dpOrder = dp;
		}

		public void AddToDCRSharedList(DCRelationship dcr, bool baddtochildren)
		{
			foreach(DCRelationship dcrexist in this.alDCRShared)
			{
				if( dcrexist.nRelation == dcr.nRelation )
					return;
			}
			this.alDCRShared.Add(dcr);

			if(baddtochildren == true)
			{
				foreach(DockControllerBase dcchild in this.alChildren)
					dcchild.AddToDCR(new DCRelationship(dcr.nRelation, true, dcr.DP, dcr.nIndex));
			}
		}

		// Call CloseController on this controller's immediate children. This will invoke recursion.
		public override void CloseController()
		{
			for( int i = 0; i < this.ChildCount; i++ )
			{
				DockControllerBase dc = this.alChildren[i] as DockControllerBase;
				if( !( dc is DragSplitterController ) 
					&& !(dc is DockStateControllerWrapper))
					dc.CloseController();

			}			
		}

		public override void InvokeDocking(DockControllerBase dc)
		{
			DockStateControllerBase dhc = dc as DockStateControllerBase;
			Debug.Assert((dhc != null), "Error: Invalid Controller.\n");

			this.dockingMgr.DockToSizingController(dhc);
			// Set the DCR for the new dockhost
			if(this.currentRelationship == null)
				this.currentRelationship = new DCRelationship(this.GetHashCode(), false, this.dockInfoCurrent.DP, -1);
			dhc.DCRCurrent = new DCRelationship(this.currentRelationship.nRelation, true, this.dockInfoCurrent.DP, this.GetChildHostIndex(dhc));
			// Update the current DCR for all child dockhostcontrollers/tabcontrollers of this parent sizingcontroller
			foreach(DockControllerBase dcbase in this.alChildren)
			{
				DockStateControllerBase ddcbase = dcbase as DockStateControllerBase;
				if( (ddcbase != null) && (ddcbase != dhc) )
				{
					DCRelationship dcrupdate = new DCRelationship( this.currentRelationship.nRelation, true, this.dockInfoCurrent.DP,
																		this.GetChildHostIndex(ddcbase) );
					ddcbase.UpdateDCRIndex(dcrupdate);
				}
			}

			if(dc.DockingManager != this.dockingMgr)
				this.dockingMgr.ImportControl( dc );
		}

		public override bool QueryRelationship(DCRelationship dcr)
		{
			if( (this.currentRelationship != null) && (this.currentRelationship.nRelation == dcr.nRelation) &&
				(this.currentRelationship.DP == dcr.DP) && (this.currentRelationship.bChild != dcr.bChild) )
			{
				return true;
			}
			if(this.alDCRShared.Count > 0)
			{
				foreach(DCRelationship dcrel in this.alDCRShared)
				{
					if( (dcrel.nRelation == dcr.nRelation) && (dcrel.DP == dcr.DP) && (dcrel.bChild == dcr.bChild) )
						return true;
				}
			}
			return false;
		}

		protected internal void GetChildDockHosts(ref ArrayList alDockHosts)
		{
			foreach( DockControllerBase dcb in alChildren )
			{
				SizingController sc = dcb as SizingController;
				if( sc != null )
				{
					sc.GetChildDockHosts( ref alDockHosts );
				}
				else
				{
					DockTabController dtc = dcb as DockTabController;
					if( dtc != null )
					{
						foreach( DockTabPage page in dtc.TabControl.TabPages )
						{
							alDockHosts.Add( page.dhcClient.HostControl );
						}
					}
					else
					{
						DockHostController dhc = dcb as DockHostController;
						if( dhc != null )
						{
							alDockHosts.Add( dhc.HostControl );
						}
					}
				}
			}
		}

		public override bool AttemptDCRDocking(DockControllerBase ctrl, IEnumerator iedcr)
		 {
			 DockHostController host = ctrl as DockHostController;
			 Debug.Assert((host != null), "Error: Invalid DockHostController.\n");

			 // Before docking to the controller specified in the newdockinfo, run through that controller's children
			 // to see if the dockcontroller enjoys a higher relationship with any of the children. If this is true,
			 // then dock to that child, using the relation
			 ControllerDCRPair cdcrp = null;
			 iedcr.Reset();
			 while(iedcr.MoveNext() == true)
			 {
				 cdcrp = IterChildControllers(this, iedcr.Current as DCRelationship);
				 if(cdcrp != null)
				 {
					 host.DINew.dController = cdcrp.Controller;
					 host.DINew.DP = cdcrp.DCR.DP;
					 host.DINew.nDockIndex = cdcrp.DCR.nIndex;
					 host.DINew.dController.InvokeDCRDocking(host, cdcrp.DCR);
					 return true;
				 }
			 }

			if(this.ParentController.MainFormController == false)
			{
				// See if the dockhost has a relation with any of this controller's siblings
				IEnumerator iesiblings = this.ParentController.ChildHostEnumerator;
				while(iesiblings.MoveNext() == true)
				{
					DockControllerBase dcsibling = iesiblings.Current as DockControllerBase;
					if(dcsibling != this)
					{
						if(dcsibling.AttemptDCRDocking(ctrl, iedcr) == true)
							return true;
					}
				}
			}
			return false;
		 }


		public override void InvokeDCRDocking(DockControllerBase dc, DCRelationship dcr)
		{
			DockHostController dhc = dc as DockHostController;
			Debug.Assert((dhc != null), "Error: Invalid Controller.\n");

			// If the new hostcontroller enjoys a parent-child direct relation with this sizing controller, then
			// insert the host into the sizingcontroller's child list
			if( (this.currentRelationship.nRelation == dcr.nRelation) && (this.currentRelationship.DP == dcr.DP)
						&& (dcr.bChild == true) )
			{
				// When inserting into an existing sizing controller, the DCR index, stored in the dockhost's DINew
				// is just a hint and is not the exact insertion point. Use the DCR index to calculate the actual
				// insertion point within the controller.
				int ninsindex = 0;
				foreach(DockControllerBase dcb in this.alChildren)
				{
					IEnumerator iedcr = dcb.DCR;
					DCRelationship dcrchild = null;
					while(iedcr.MoveNext() == true)	// Get the child relation that matches the current relation
					{
						dcrchild = iedcr.Current as DCRelationship;
						if(dcrchild.nRelation == dcr.nRelation)
							break;
					}
					// dcrchild will be null for splitters
					if( (dcrchild == null) || (dcrchild.nIndex < dhc.DINew.nDockIndex) )
						ninsindex++;
					else break;
				}
				dhc.DINew.nDockIndex = ninsindex;
				// If the rcDockArea is larger than the median size that can be accomodated to all the childhosts within
				// this sizing controller, then resize it to the median
				int nchildcnt = this.ChildHostCount+1;
				if( (dhc.DINew.DP == DockPreference.Horizontal) && (dhc.DINew.rcDockArea.Width >= this.LayoutRect.Width/nchildcnt) )
					dhc.DINew.rcDockArea = new Rectangle(0, 0, this.LayoutRect.Width/nchildcnt, this.LayoutRect.Height);
				else if( (dhc.DINew.DP == DockPreference.Vertical) && (dhc.DINew.rcDockArea.Height >= this.LayoutRect.Height/nchildcnt) )
					dhc.DINew.rcDockArea = new Rectangle(0, 0, this.LayoutRect.Width, this.LayoutRect.Height/nchildcnt);
				this.dockingMgr.DockToSizingController(dhc);
			}
			else
			{
				if(dcr.nIndex == 0)
					dhc.DINew.dStyle = (dhc.DINew.DP == DockPreference.Horizontal) ? Syncfusion.Windows.Forms.Tools.DockingStyle.Left : Syncfusion.Windows.Forms.Tools.DockingStyle.Top;
				else
					dhc.DINew.dStyle = (dhc.DINew.DP == DockPreference.Horizontal) ? Syncfusion.Windows.Forms.Tools.DockingStyle.Right : Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom;

				int nmaxwidth = this.LayoutRect.Width - (this.LayoutRect.Width/this.dockingMgr.MaxRedockFactor);
				int nmaxheight = this.LayoutRect.Height - (this.LayoutRect.Height/this.dockingMgr.MaxRedockFactor);
				if( (dhc.DINew.DP == DockPreference.Horizontal) && (dhc.DINew.rcDockArea.Width > nmaxwidth) )
					dhc.DINew.rcDockArea = new Rectangle(0, 0, nmaxwidth, this.LayoutRect.Height);
				else if( (dhc.DINew.DP == DockPreference.Vertical) && (dhc.DINew.rcDockArea.Height > nmaxheight) )
					dhc.DINew.rcDockArea = new Rectangle(0, 0, this.LayoutRect.Width, nmaxheight);

				this.dockingMgr.DockToNewSizingController(dhc);

				// Update the dcrs for the new parent sizing controller
				SizingController sc = this.ParentController as SizingController;
				Debug.Assert((sc != null), "Error: Invalid Cast.\n");
				sc.DCRCurrent = new DCRelationship(dcr.nRelation, false, sc.DICurrent.DP, -1);

				// If the new sizing controller's two children, ie., this dockhost and this sizingcontroller have common relations
				// then add them to the sizing controller's DCRShared list.
				foreach(DCRelationship dcrchild1 in this.alDCRShared)
				{
					if(dcrchild1.nRelation == sc.DCRCurrent.nRelation)
						continue;
					IEnumerator iedcrchild2 = dhc.DCR;
					while(iedcrchild2.MoveNext() == true)
					{
						DCRelationship dcrchild2 = iedcrchild2.Current as DCRelationship;
						if( (dcrchild1.nRelation == dcrchild2.nRelation) && (dcrchild1.DP == dcrchild2.DP) &&
							(dcrchild1.bChild == dcrchild2.bChild) )
						{
							sc.AddToDCRSharedList(new DCRelationship(dcrchild1.nRelation, true, dcrchild1.DP, -1), false);
							break;
						}
					}
				}
			}
		}

		public override void AddChild(DockControllerBase dcinternal, Syncfusion.Windows.Forms.Tools.DockingStyle db)
		{
			if( (db == Syncfusion.Windows.Forms.Tools.DockingStyle.Left) || (db == Syncfusion.Windows.Forms.Tools.DockingStyle.Top)
				|| db == DockingStyle.Fill)
				InsertChild(dcinternal, this.alChildren.Count, db);
			else
				InsertChild(dcinternal, 0, db);
		}

        private void RemoveRespectiveWrapper(DockControllerBase dhc)
        {
            if (dhc is DockHostController)
            {
                ArrayList wrappers = new ArrayList();
                foreach (DockControllerBase ctrlBase in this.alChildren)
                {
                    if (ctrlBase is DockStateControllerWrapper)
                        wrappers.Add(ctrlBase);
                }
                foreach (DockStateControllerWrapper wrapper in wrappers)
                {
                    if (wrapper.InternalControl == dhc)
                        this.alChildren.Remove(wrapper);
                }
            }
        }

		public override void InsertChild(DockControllerBase dc, int index, Syncfusion.Windows.Forms.Tools.DockingStyle db)
		{
            //Remove respective wrappers if any, Which is no longer needed. This especially in the case where the control has been floated
            // and Load state is forced.
            this.RemoveRespectiveWrapper(dc);

			this.ieSizing = null;
			if(index > this.alChildren.Count)
				index = this.alChildren.Count;
			if(this.alChildren.Contains(dc) == false)
			{
				bool insertSplitter = this.GetDockControllers().Count > 0;
                
                // In case if the controller to be inserted is Sizing controller and if the sizing controller doesn't hold any 
                //valid DHC and just holds wrappers, then no need to insert splitter
                if (insertSplitter && dc is SizingController && (dc as SizingController).GetDockControllers().Count == 0 && db == DockingStyle.Fill)
                {
                    insertSplitter = false;
                }

				this.alChildren.Insert(index, dc);
				dc.ParentController = this;

				if( insertSplitter || (this.ParentController == this.ToplevelController 
					&& (!this.IsEmpty || this.dockingMgr.ForbidWrapperLogic)))
					InsertSplitter(dc, db);
			}

			foreach( DockControllerBase baseCtrlr in this.alChildren )
			{
				if( baseCtrlr is DockStateControllerWrapper )
				{
					this.HideRelatedSplitter(baseCtrlr);
					break;
				}
			}

			//Set the child controller's parent controller and current dockinfo
			int nindex = this.alChildren.IndexOf(dc);
			dc.DICurrent = new DockInfo(this, db, 0, nindex, this.dpOrder, Rectangle.Empty);

			UpdateControlList();
		}

		private void InsertSplitter(DockControllerBase dc, DockingStyle db)
		{
			int index = this.alChildren.IndexOf(dc);
			DragSplitter ds = null;
			int nsplindex = 0;
	
			// In the case of hosting within a floating frame, insert a splitter only when two or more dockhosts are
			// docked to the frame.
			if((this.ParentController is MainFormController) == false)
			{
				// Whenever a windowed child controller is being added to the list, create and insert a drag splitter
				// between the two children
				if( this.alChildren.Count == 2 )
				{
					ds = CreateNewSplitter();
					// If the controller that immediately precedes this new controller in the list is a splitter, then
					// insert the new splitter one position after the new controller. Else before it.
					if(index > 0)
					{
						DockControllerBase predc = this.alChildren[index-1] as DockControllerBase;
						if(predc is DragSplitterController)
							nsplindex = index+1;
						else
							nsplindex = index;
					}
					else
						nsplindex = 1;
				}
			}
			else	// Docking directly to the mainformcontroller.
			{
				nsplindex = index+1;
				if(this.dockingMgr.DockToFill == false)
				{
					ds = CreateNewSplitter();
				}
				else
				{
					bool bcreatesplitter = true;
					// In the DockToFill mode, do not create a splitter if the child being inserted
					// is the trailing child in the mainformcontroller.
					if(index == this.alChildren.Count-1)
					{
						if(index == 0)
							bcreatesplitter = false;
						else
							nsplindex = index;
					}
					// Make an exemption for the autohidden mode.
					if(((dc is DockStateControllerBase) && (dc as DockStateControllerBase).bInAutoHide))
						bcreatesplitter = true;
					if(bcreatesplitter)
						ds = CreateNewSplitter();
				}
			}
	
			if(ds != null)
			{
				this.alChildren.Insert(nsplindex, ds.InternalController);
				ds.InternalController.DICurrent = new DockInfo(null, db, 0, nsplindex, this.dpOrder,
				                                               this.ctrlHost.RectangleToScreen(ds.InternalController.LayoutRect));
				// Add the splitter controller to the docking manager's list of dock targets
				this.dockingMgr.AddController(ds.InternalController);
			}
		}

		protected DragSplitter CreateNewSplitter()
		{
			DragSplitter ds = new DragSplitter(this.dockingMgr);
			this.ctrlHost.Controls.Add(ds);
			ds.InternalController.ParentController = this;
			if(this.dpOrder == DockPreference.Horizontal)
			{
				if(this.dockingMgr.FreezeResizing != true)
					ds.Cursor = Cursors.VSplit;
				ds.InternalController.LayoutRect = new Rectangle(0, 0, 
					dockingMgr.SplitterWidth, rcLayout.Height);
			}
			else	// DockPreference.Vertical
			{
				if(this.dockingMgr.FreezeResizing != true)
					ds.Cursor = Cursors.HSplit;
				ds.InternalController.LayoutRect = new Rectangle(0, 0, 
					rcLayout.Width, dockingMgr.SplitterWidth);
			}
			return ds;
		}

		public override void RemoveChild(DockControllerBase dcinternal)
		{
			this.ieSizing = null;
			if(this.alChildren.Count > 1)
			{
				// Whenever a dockhost child is being removed, the splitter adjacent to this control should be removed
				// If this is the last child, then remove the splitter controller that lies prior to this in the list.
				// Else remove the splitter controller that follows this dockhost.
				DragSplitterController dsc = GetNeighborSplitter(dcinternal);

				if(dsc != null)
				{
					// Remove splitter controller from docking manager's list
					this.dockingMgr.RemoveController(dsc);
					this.alChildren.Remove(dsc);					
					DragSplitter splitter = dsc.HostControl as DragSplitter;
					splitter.Visible = false;					
					this.ctrlHost.Controls.Remove(splitter);
					splitter.Dispose();
				}
			}

			try
			{
				this.alChildren.Remove(dcinternal);
				dcinternal.ParentController = null;
			}
			catch(ArgumentException e)
			{
				Debug.Assert(false, e.Message);
				return;
			}

            SizingController parent = this.ParentController as SizingController;

            bool suitable = true;

            if( parent != null && parent.ChildControllers.Count > 2 && parent.ChildControllers[2] is DockTabController )
                suitable = false;

            if (parent != null && this.alChildren.Count == 0 && !(dcinternal is DockTabController || dcinternal.ParentController == null) && suitable)
            {
                parent.HideRelatedSplitter(this);
            }

            UpdateControlList();
		}

		private DragSplitterController GetNeighborSplitter(DockControllerBase dcinternal)
		{
			DragSplitterController dsc = null;
			int index = this.alChildren.IndexOf(dcinternal);

			if( this.alChildren.Count != 1 )
			{
				if(this.ParentController is MainFormController)
				{
					if(this.dockingMgr.DockToFill == false)
						index++;
					else
					{
						// In the DockToFill mode, the last dockcontrollerBase will not be a splittercontroller
						if( ( index + 1 ) <= ( this.alChildren.Count - 1 ) )
							index++;
						else
							index--;
					}
				}
				else
				{
					if( index == this.alChildren.Count - 1 )
						index--;
					else
						index++;
				}

				if( index + 1 <= this.alChildren.Count )
					dsc = this.alChildren[index] as DragSplitterController;
			}

			return dsc;
		}


		public bool IsEmpty
		{
			get
			{
				bool empty = true;
				foreach( DockControllerBase ctrl in this.alChildren )
				{
					if( ctrl is DockHostController ||
						ctrl is DockTabController )
						empty = false;

					if( ctrl is SizingController)
						if( !( ctrl as SizingController ).IsEmpty )
							empty = false;
				}

				return empty;
			}
		}

		public override void ReplaceChild(DockControllerBase dccurrent, DockControllerBase dcnew)
		{
			this.ieSizing = null;
			int ncurrindex = this.alChildren.IndexOf(dccurrent);
			if( ncurrindex < 0 )
			{
				Debug.Assert(false, "Invalid Controller.\n");
				return;
			}
			Debug.Assert(dcnew != null);
			bool empty = this.IsEmpty;

			dcnew.DICurrent = new DockInfo(dccurrent.DICurrent);
			this.alChildren[ncurrindex] = dcnew;
			dcnew.ParentController = this;

			SizingController parentSizing = this.ParentController as SizingController;

			while( parentSizing != null && parentSizing.IsEmpty )
				parentSizing = parentSizing.ParentController as SizingController;

			ToggleSplitterVisibility(dccurrent, dcnew, empty);

			if( parentSizing != null )
			{
				if( !empty && this.IsEmpty )
					parentSizing.AdjustLayout();
			}

			if( !( dcnew is DockStateControllerWrapper ) )
			{
				if( this.LayoutRect.Height != 0 && this.LayoutRect.Width != 0 )
				{
					foreach( DockControllerBase dcbase in this.alChildren )
						dcbase.DITransient.rcDockArea = dcbase.LayoutRect;
				}
				else
				    dcnew.DITransient.rcDockArea = dcnew.LayoutRect;
			}
		}

		internal protected void ToggleSplitterVisibility( DockControllerBase dccurrent, DockControllerBase dcnew, bool empty )
		{
			if( dcnew is DockStateControllerWrapper &&
				!( dccurrent is DockStateControllerWrapper ) )
			{
				HideRelatedSplitter(dcnew);

				if( this.IsEmpty )
				{
					SizingController scParent = this.ParentController as SizingController;
					SizingController scChild = this;

					while( scParent != null )
					{
						if( !(scParent.ParentController is SizingController) ||
							!scParent.IsEmpty )
							break;

						scChild = scParent;
						scParent = scParent.ParentController as SizingController;
					}

					if( scParent != null )
						scParent.HideRelatedSplitter(scChild);
				}
			}

			if( ( dcnew is DockHostController || dcnew is DockTabController )
				&& ( dccurrent is DockStateControllerWrapper ) )
			{
				if( this.ParentController is MainFormController || !empty )
					ShowRelatedSplitter(dcnew);

				SizingController parentSc = this;
				SizingController childSc = null;

				if( this.GetDockControllers().Count > 1 )
					parentSc = null;

				while( parentSc != null )
				{
					SizingController next = parentSc.ParentController as SizingController;

					if( next == null )
					{
						if( DockingManager.DockToFill || this.Floating )
							parentSc = null;
						break;
					}

					childSc = parentSc;
					parentSc = next;

					if( next.GetDockControllers().Count > 1 )
						break;
				}

				if( parentSc != null )
					parentSc.ShowRelatedSplitter(childSc);
			}
		}

		internal void ShowRelatedSplitter( DockControllerBase dcnew )
		{			
			DragSplitterController dsc = this.GetNeighborSplitter(dcnew);

			if( dsc != null )
			{
				if( this.dpOrder == DockPreference.Vertical )
				{
					if( dsc.LayoutRect.Height == 0 )
					{
						dsc.LayoutRect = new Rectangle(dsc.LayoutRect.Location,
							new Size(this.LayoutRect.Width, this.DockingManager.SplitterWidth));
						this.rcLayout.Height += DockingManager.SplitterWidth;
					}
				}
				else
				{
					if( dsc.LayoutRect.Width == 0 )
					{
						dsc.LayoutRect = new Rectangle(dsc.LayoutRect.Location,
							new Size(this.DockingManager.SplitterWidth, this.LayoutRect.Height));
						this.rcLayout.Width += DockingManager.SplitterWidth;
					}
				}
			}
			else
			{
				if( dcnew != null )
					InsertSplitter(dcnew, dcnew.DICurrent.dStyle);
			}
		}

		internal void HideRelatedSplitter( DockControllerBase dcnew )
		{			
			DragSplitterController dsc = this.GetNeighborSplitter(dcnew);

			if( dsc != null )
			{
				this.dockingMgr.RemoveController( dsc );
				this.alChildren.Remove( dsc );
				DragSplitter splitter = dsc.HostControl as DragSplitter;
				splitter.Visible = false;
				this.ctrlHost.Controls.Remove( splitter );
				splitter.Dispose();
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
				foreach( DockControllerBase child in this.alChildren )
					child.Minimized = value;

				base.Minimized = value;

				if( value != Minimization.None )
				{
					int height = RefreshMinimizedHeight();
					
					if( value == Minimization.Vertical )
						this.rcLayout.Height = height;
					else
						this.rcLayout.Width = height;

					this.DITransient.rcDockArea = this.rcLayout;
				}

				bool containMaximized = false;
				foreach( DockControllerBase child in this.alChildren )
					if( child.Maximized )
						containMaximized = true;

				if( !containMaximized )
					foreach( DockControllerBase child in this.alChildren )
					{
						if( child is DockTabController
							|| child is DockHostController )
							child.Minimized = Minimization.None;
					}
			}
		}

		public override DockControllerBase GetChildAt(int index)
		{
			try
			{
				return this.alChildren[index] as DockControllerBase;
			}
			catch(ArgumentException e)
			{
				Trace.Write(e.Message);
				return null;
			}
		}

		public override int GetChildHostIndex(DockControllerBase child)
		{
			int i = -1;
			if(this.alChildren.Contains(child) == false)
				return i;

			foreach(DockControllerBase dc in this.alChildren)
			{
				if((dc is DragSplitterController) == false)
					i++;
				if(dc.Equals(child))
					break;
			}
			return i;
		}

		protected void UpdateControlList()
		{
			int index = 0;
			foreach(DockControllerBase dcb in this.alChildren)
			{
				dcb.DICurrent.nDockIndex = index++;
                if(!this.DockingManager.m_bLoadingDockState)
                    dcb.DITransient.rcDockArea = Rectangle.Empty;
            }
		}

		public override void GetDockInfo(Control ctrl, Point pt, DockInfo di)
		{
			// No imp
		}

		public override bool QueryDropProceedWithDock(Control ctrldrop, Syncfusion.Windows.Forms.Tools.DockingStyle style)
		{
			Debug.Assert(this.alChildren.Count > 0);
			foreach(DockControllerBase dcb in this.alChildren)
			{
				if((dcb.HostControl != ctrldrop) && ((dcb is DragSplitterController)==false))
					return dcb.QueryDropProceedWithDock(ctrldrop, style);
			}
			return true;
		}

		public override void UpdateControl()
		{

		}

		class LayoutResizer
		{
			public LayoutResizer( SizingController controller)
			{
				m_Controller = controller;
			}
			
			public void Calculate()
			{
				m_Location = m_Controller.LayoutRect.Location;
				Size parentSize = m_Controller.GetRedockSize();
				Size currSize = m_Controller.LayoutRect.Size;

				for( int i = 0 ; i < m_Controller.alChildren.Count ; i++ )
				{
					Rectangle rectangle = new Rectangle( m_Location,
						( m_Controller.alChildren[ i ] as IResizable ).CalculateSize( parentSize, m_Controller.LayoutRect.Size ) );

					if( m_Controller.DockingOrder == DockPreference.Vertical )
					{
						rectangle.Width = currSize.Width;
					}
					else
					{
						rectangle.Height = currSize.Height;
					}

					rectangles.Add( rectangle );
					m_Location.Offset(
						Preference == DockPreference.Horizontal ? rectangle.Width : 0,
						Preference == DockPreference.Vertical ? rectangle.Height : 0);
				}

				ProcessRemainderPixels();
				UpdateSplitterPositions();

			}

			private int ChildCount
			{
				get { return m_Controller.ChildCount; }
			}

			private DockPreference Preference
			{
				get { return m_Controller.DICurrent.DP; }
			}

			private Point m_Location = Point.Empty;

			public Rectangle this [ int index ]
			{
				get
				{
					return rectangles[ index ];
				}
			}

			private void ProcessRemainderPixels()
			{
				Point pixelLeft = new Point(
					Preference == DockPreference.Horizontal ? 
					m_Controller.LayoutRect.X + m_Controller.LayoutRect.Width - m_Location.X: 0,
					Preference == DockPreference.Vertical ? 
					m_Controller.LayoutRect.Y + m_Controller.LayoutRect.Height - m_Location.Y: 0 );
				
				int nChildCount = m_Controller.alChildren.Count;
				if( pixelLeft != Point.Empty )
				{
					IResizable lastController = (IResizable) m_Controller.alChildren[nChildCount - 1];
					Rectangle lastRect = rectangles[nChildCount - 1];
					if( lastController.IsHorizontallyResizable() )
					{
						Rectangle rect = rectangles[nChildCount - 1];
						lastRect.Inflate( pixelLeft.X, 0 );
					}
					else
					{
						if( nChildCount > 1 )
						{
							Rectangle prevRect = rectangles[nChildCount - 2];
							prevRect.Inflate(pixelLeft.X, 0);
							lastRect.Offset(pixelLeft.X, 0);
						}
					}
					if( lastController.IsVerticallyResizable() )
					{
						Rectangle rect = rectangles[nChildCount - 1];
						lastRect.Inflate(0, pixelLeft.Y);
					}
					else
					{
						if( nChildCount > 1 )
						{
							Rectangle prevRect = rectangles[nChildCount - 2];
							prevRect.Inflate(0, pixelLeft.Y);
							lastRect.Offset(0, pixelLeft.Y);
						}
					}
				}
			}

			private void UpdateSplitterPositions()
			{
				if( m_Controller.alChildren[ChildCount - 1] is DragSplitterController && 
					(m_Controller.DICurrent.dStyle == DockingStyle.Right || 
					m_Controller.DICurrent.dStyle == DockingStyle.Bottom) )
				{
					Rectangle lastRect = rectangles[ChildCount - 1];
					Rectangle prevRect = rectangles[ChildCount - 2];
				
					m_Location.X = lastRect.X - prevRect.X;
					m_Location.Y = lastRect.Y - prevRect.Y;
				
					lastRect.X -= m_Location.X;
					lastRect.Y -= m_Location.Y;
				
					if( Preference == DockPreference.Horizontal )
						prevRect.X = lastRect.X + lastRect.Width;
					
					if( Preference == DockPreference.Vertical )
						prevRect.Y = lastRect.Y + lastRect.Height;
				
					rectangles[ChildCount - 1] = lastRect;
					rectangles[ChildCount - 2] = prevRect;
				}
			}

			private SizingController m_Controller;
			private RectangleCollection rectangles = new RectangleCollection();
		}
		public DockPreference DockingOrder
		{
			get
			{
				return dpOrder;
			}
		}
		
		public override void AdjustLayout()
		{
			foreach(DockControllerBase controller in this.alChildren)
			{
				if(controller.DITransient.rcDockArea == Rectangle.Empty ||(controller is DockTabController && this.DockingManager.m_bLoadingDockState) )
					controller.DITransient.rcDockArea = controller.LayoutRect;
			}

			for( int i = 0; i < alChildren.Count; i++ )
			{
				DockStateControllerWrapper controllerWrapper = alChildren[i] as DockStateControllerWrapper;
				if( controllerWrapper != null )
					controllerWrapper.RefreshSize();
			}

			LayoutResizer layoutResizer = new LayoutResizer(this);
			layoutResizer.Calculate();
			
			for( int i = 0; i < alChildren.Count; i++ )
			{
				DockControllerBase controller = (DockControllerBase) alChildren[i];
				controller.LayoutRect = layoutResizer[i];
			}

			if(this.dcPriority != null)
			{
				foreach( DockControllerBase controller in alChildren )
				{
					bool resetTransient = true;

					if( controller.LayoutRect.Width == 0 || controller.LayoutRect.Height == 0 )
					{
						SizingController sc = controller as SizingController;
						if( ( sc != null && !sc.IsEmpty )
							|| ( controller is DockStateControllerBase && !( controller is DockStateControllerWrapper ) ) )
							resetTransient = false;
					}

					if( resetTransient )
						controller.DITransient.rcDockArea = Rectangle.Empty;
				}
			}

			this.MustResize = Direction.None;
		}

		internal override bool Maximized
		{
			get
			{
				return base.Maximized;
			}
			set
			{
				foreach( DockControllerBase ctrl in this.alChildren )
					ctrl.Maximized = value;

				base.Maximized = value;
			}
		}
		
		// Assume uniform distribution ie., a dock operation. Splitter resize will use a non-uniform layout
		// distribution where individual dock sizes are used for the layout
		protected void AdjustLRLayout()
		{
			int nchildcount = this.alChildren.Count;
			int nreqdim = 0;
			// Estimate the total required width for all controls.
			foreach(DockControllerBase dc in this.alChildren)
			{
				// The insertion/splitter resizing size is stored in dc.DITransient and this is
				// used to preserve the child ratios during resizing.
				if(dc.DITransient.rcDockArea == Rectangle.Empty)
					dc.DITransient.rcDockArea = dc.LayoutRect;
				nreqdim +=  dc.DITransient.rcDockArea.Width;
			}

			// If a space constraint exists, then it is necessary to resize all hosted controls managed by this controller
			if((nchildcount > 0) /*&& (nreqdim != this.LayoutRect.Width)*/)
			{
				if(nchildcount > 1)
				{
					// Ok, there is a space crunch here...
					// The priority control is, well, given top priority while the remaining controls
					// are squished into the available space based on a percentage of their current dimension.
					int navlwidth = this.LayoutRect.Width;
					if(this.dcPriority != null)
					{
						navlwidth -= this.dcPriority.LayoutRect.Width;
						nreqdim -= this.dcPriority.LayoutRect.Width;
					}
					int ndiffwidths = Math.Abs(navlwidth-nreqdim);

					if(this.ieSizing == null)
					{
						this.ieSizing = this.alChildren.GetEnumerator();
						this.ieSizing.MoveNext();
					}

					int nincrement = 0;
					DockControllerBase dcstart = null;
					bool bfirstpass = true;
					int noverrun = 0;
					while(true)	// Starting from the dc following the ieSizing.Current dc, run through all controllers once.
					{
						if(this.ieSizing.MoveNext() == false)
						{
							this.ieSizing.Reset();
							this.ieSizing.MoveNext();
						}
						while((this.ieSizing.Current is DragSplitterController) == true)
						{
							if(this.ieSizing.MoveNext() == false)
							{
								this.ieSizing.Reset();
								this.ieSizing.MoveNext();
							}
						}
						DockControllerBase dccurrent = this.ieSizing.Current as DockControllerBase;
						if(dcstart == null)
							dcstart = dccurrent;
						else if(dcstart == dccurrent)
						{
							if((bfirstpass == true) && (noverrun != 0))
								bfirstpass = false;
							else
								break;
						}

						if((this.dcPriority == null) || (dccurrent.HostControl.Equals(this.dcPriority.HostControl) == false))
						{
							int nwidth = (int)(((float)dccurrent.DITransient.rcDockArea.Width/(float)nreqdim) * navlwidth) + noverrun;

							if((dccurrent.MinimumSize.Width != 0) && (nwidth <= dccurrent.MinimumSize.Width)
								&& (dccurrent.LayoutRect.Width >= dccurrent.MinimumSize.Width))
							{
								dccurrent.LayoutRect = new Rectangle(dccurrent.LayoutRect.X, dccurrent.LayoutRect.Y,
									dccurrent.MinimumSize.Width, dccurrent.LayoutRect.Height);
								noverrun += (nwidth - dccurrent.LayoutRect.Width);
							}
							else
							{
								noverrun = 0;
								nincrement += Math.Abs(dccurrent.LayoutRect.Width - nwidth);
								dccurrent.LayoutRect = new Rectangle(dccurrent.LayoutRect.X, dccurrent.LayoutRect.Y,
									nwidth, dccurrent.LayoutRect.Height);
							}
						}
						if(nincrement == ndiffwidths)	// Break if delta widths has been met
							break;
						if((bfirstpass == false) && (noverrun == 0))
							break;
					}

					// If after the sizing operation, the dc widths fall short of the total available width,
					// then push off the remaining width to the last controller.
					int nnewdim = 0;
					foreach(DockControllerBase dc in this.alChildren)
						nnewdim +=  dc.LayoutRect.Width;
					if(nnewdim != this.LayoutRect.Width)
					{
						int nremainingwidth = this.LayoutRect.Width-nnewdim;

						DockControllerBase dcadjustsize = this.alChildren[nchildcount-1] as DockControllerBase;
						if(dcadjustsize is DragSplitterController)
							dcadjustsize = this.alChildren[nchildcount-2] as DockControllerBase;
						Debug.Assert((dcadjustsize != null), "Error: Invalid Cast.\n");

						int nadjustedwidth = dcadjustsize.LayoutRect.Width + nremainingwidth;
						if((dcadjustsize.MinimumSize.Width != 0) && (nadjustedwidth < dcadjustsize.MinimumSize.Width))
						{
							// Try to find a DockController that can accept 'nadjustedwidth' without affecting it's min/max extents
							foreach(DockControllerBase dcbase in this.alChildren)
							{
								if(dcbase == dcadjustsize)
									break;
								if(dcbase is DragSplitterController)
									continue;
								int ndcbwidth = dcbase.LayoutRect.Width + nremainingwidth;
								if((dcbase.MinimumSize.Width == 0) || (ndcbwidth >= dcbase.MinimumSize.Width))
								{
									dcadjustsize = dcbase;
									nadjustedwidth = ndcbwidth;
									break;
								}
							}
						}

						dcadjustsize.LayoutRect = new Rectangle(dcadjustsize.LayoutRect.X, dcadjustsize.LayoutRect.Y,
							nadjustedwidth, dcadjustsize.LayoutRect.Height);
					}
				}
				else
				{
					DockControllerBase dcchild = this.alChildren[0] as DockControllerBase;
					dcchild.LayoutRect = new Rectangle(dcchild.LayoutRect.X, dcchild.LayoutRect.Y,
						this.LayoutRect.Width, dcchild.LayoutRect.Height);
				}
			}

			int noffset = 0;
			for(int i=0; i<nchildcount; i++)		// Reposition all controllers
			{
				DockControllerBase dc = this.alChildren[i] as DockControllerBase;
				if(this.ParentController.MainFormController == true)
				{
					if( this.dockInfoCurrent.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Right)
						dc.LayoutRect = new Rectangle(LayoutRect.Right-noffset-dc.LayoutRect.Width, LayoutRect.Top, dc.LayoutRect.Width, LayoutRect.Height);
					else
						dc.LayoutRect = new Rectangle(LayoutRect.Left+noffset, LayoutRect.Top, dc.LayoutRect.Width, LayoutRect.Height);
				}
				else
				{
					dc.LayoutRect = new Rectangle(LayoutRect.Left+noffset, LayoutRect.Top, dc.LayoutRect.Width, LayoutRect.Height);
				}
				noffset += dc.LayoutRect.Width;
			}

			if(this.dcPriority != null)
			{
				// If the priority controller exits, then redo the transient rect calculation for
				// all controllers during the next layout call as the current layout adjustment
				// would have altered bounds to favor the priority controller
				foreach(DockControllerBase dc in this.alChildren)
					dc.DITransient.rcDockArea = Rectangle.Empty;
			}
		}

		protected void AdjustTBLayout()
		{
			int nchildcount = this.alChildren.Count;
			int nreqdim = 0;
			// Estimate the total required width for all controls.
			foreach(DockControllerBase dc in this.alChildren)
			{
				if(dc.DITransient.rcDockArea == Rectangle.Empty)
					dc.DITransient.rcDockArea = dc.LayoutRect;
				nreqdim +=  dc.DITransient.rcDockArea.Height;
			}

			// If a space constraint exists, then it is necessary to resize all hosted controls managed by this controller
			if((nchildcount > 0) /*&& (nreqdim != this.LayoutRect.Height)*/)
			{
				if(nchildcount > 1)
				{
					// Ok, there is a space crunch here...
					// The priority control is, well, given top priority while the remaining controls
					// are squished into the available space based on a percentage of their current dimension.
					int navlheight = this.LayoutRect.Height;
					if(this.dcPriority != null)
					{
						navlheight -= this.dcPriority.LayoutRect.Height;
						nreqdim -= this.dcPriority.LayoutRect.Height;
					}
					int ndiffheights = Math.Abs(navlheight-nreqdim);

					if(this.ieSizing == null)
					{
						this.ieSizing = this.alChildren.GetEnumerator();
						this.ieSizing.MoveNext();
					}

					int nincrement = 0;
					DockControllerBase dcstart = null;
					bool bfirstpass = true;
					int noverrun = 0;
					while(true)	// Starting from the dc following the ieSizing.Current dc, run through all controllers once.
					{
						if(this.ieSizing.MoveNext() == false)
						{
							this.ieSizing.Reset();
							this.ieSizing.MoveNext();
						}
						while((this.ieSizing.Current is DragSplitterController) == true)
						{
							if(this.ieSizing.MoveNext() == false)
							{
								this.ieSizing.Reset();
								this.ieSizing.MoveNext();
							}
						}
						DockControllerBase dccurrent = this.ieSizing.Current as DockControllerBase;
						if(dcstart == null)
							dcstart = dccurrent;
						else if(dcstart == dccurrent)
						{
							if((bfirstpass == true) && (noverrun != 0))
								bfirstpass = false;
							else
								break;
						}

						if((this.dcPriority == null) || (dccurrent.HostControl.Equals(this.dcPriority.HostControl) == false))
						{
							int nheight = (int)(((float)dccurrent.DITransient.rcDockArea.Height/(float)nreqdim) * navlheight);

							if((dccurrent.MinimumSize.Height != 0) && (nheight <= dccurrent.MinimumSize.Height)
								&& (dccurrent.LayoutRect.Height >= dccurrent.MinimumSize.Height))
							{
								dccurrent.LayoutRect = new Rectangle(dccurrent.LayoutRect.X, dccurrent.LayoutRect.Y,
									dccurrent.LayoutRect.Width, dccurrent.MinimumSize.Height);
								noverrun += (nheight - dccurrent.LayoutRect.Height);
							}
							else
							{
								noverrun = 0;
								nincrement += Math.Abs(dccurrent.LayoutRect.Height - nheight);
								dccurrent.LayoutRect = new Rectangle(dccurrent.LayoutRect.X, dccurrent.LayoutRect.Y,
									dccurrent.LayoutRect.Width, nheight);
							}
						}
						if(nincrement == ndiffheights)	// Break if delta heights has been met
							break;
						if((bfirstpass == false) && (noverrun == 0))
							break;
					}

					// If after the sizing operation, the dc heights fall short of the total available height,
					// then push off the remaining height to the last controller.
					int nnewdim = 0;
					foreach(DockControllerBase dc in this.alChildren)
						nnewdim +=  dc.LayoutRect.Height;
					if(nnewdim != this.LayoutRect.Height)
					{
						DockControllerBase dcadjustsize = this.alChildren[nchildcount-1] as DockControllerBase;
						if(dcadjustsize is DragSplitterController)
							dcadjustsize = this.alChildren[nchildcount-2] as DockControllerBase;
						Debug.Assert((dcadjustsize != null), "Error: Invalid Cast.\n");

						int nremainingheight = this.LayoutRect.Height - nnewdim;
						int nadjustedheight = dcadjustsize.LayoutRect.Height + nremainingheight;
						if((dcadjustsize.MinimumSize.Height != 0) && (nadjustedheight < dcadjustsize.MinimumSize.Height))
						{
							// Try to find a DockController that can accept 'nadjustedheight' without affecting it's min/max extents
							foreach(DockControllerBase dcbase in this.alChildren)
							{
								if(dcbase == dcadjustsize)
									break;
								if(dcbase is DragSplitterController)
									continue;
								int ndcbheight = dcbase.LayoutRect.Height + nremainingheight;
								if((dcbase.MinimumSize.Height == 0) || (ndcbheight >= dcbase.MinimumSize.Height))
								{
									dcadjustsize = dcbase;
									nadjustedheight = ndcbheight;
									break;
								}
							}
						}

						dcadjustsize.LayoutRect = new Rectangle(dcadjustsize.LayoutRect.X, dcadjustsize.LayoutRect.Y,
								dcadjustsize.LayoutRect.Width, nadjustedheight);
					}
				}
				else
				{
					DockControllerBase dcchild = this.alChildren[0] as DockControllerBase;
					dcchild.LayoutRect = new Rectangle(dcchild.LayoutRect.X, dcchild.LayoutRect.Y,
						dcchild.LayoutRect.Width, this.LayoutRect.Height);
				}
			}

			int noffset = 0;
			for(int i=0; i<nchildcount; i++)		// Reposition all controllers
			{
				DockControllerBase dc = this.alChildren[i] as DockControllerBase;
				if(this.ParentController.MainFormController == true)
				{
					if( this.dockInfoCurrent.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom)
						dc.LayoutRect = new Rectangle(LayoutRect.Left, LayoutRect.Bottom-noffset-dc.LayoutRect.Height, LayoutRect.Width, dc.LayoutRect.Height);
					else
						dc.LayoutRect = new Rectangle(LayoutRect.Left, LayoutRect.Top+noffset, LayoutRect.Width, dc.LayoutRect.Height);
				}
				else
				{
					dc.LayoutRect = new Rectangle(LayoutRect.Left, LayoutRect.Top+noffset, LayoutRect.Width, dc.LayoutRect.Height);
				}
				noffset += dc.LayoutRect.Height;
			}

			if(this.dcPriority != null)
			{
				// If the priority controller exits, then redo the transient rect calculation for
				// all controllers during the next layout call as the current layout adjustment
				// would have altered bounds to favor the priority controller
				foreach(DockControllerBase dc in this.alChildren)
					dc.DITransient.rcDockArea = Rectangle.Empty;
			}
		}

		public override DockControllerBase RedockController(DockInfo di, bool bforcenew)
		{
			if( this.IsEmpty )
				return null;

			// If the indicated preference is that of a tabbed docking, then invoke a tabbedredocking
			if(di.DP == DockPreference.Tabbed)
			{
				return DoRecTabbedRedocking(this, di);
			}

			// Store the dockrects for each of the child controllers in an rect array and use this array to
			// restore child controller sizes during the redocking
			Rectangle[] rclayout = new Rectangle[this.ChildHostCount];
			int nchildindex = 0;
			for(int i=0; i < this.alChildren.Count; i++)
				rclayout[nchildindex++] = (this.alChildren[i++] as DockControllerBase).LayoutRect;

			// Iterate the child list and dock each child to the new controller while preserving the existing
			// relative sizes and positions.
			nchildindex = 0;
			DockControllerBase dc = this.alChildren[0] as DockControllerBase;
			DockControllerBase dcnext = null;
			DockInfo ditemp = new DockInfo(di);
			for( int i = 0; i < 2; i++ )
			{
				if( this.alChildren.Count == 2 )
					dcnext = this.alChildren[ 1 ] as DockControllerBase;
				else if( this.alChildren.Count > 2 )
					dcnext = this.alChildren[ 2 ] as DockControllerBase;
				else
					dcnext = null;
				if( dc == null )
					break;

				DockControllerBase dctarget = dc.RedockController(ditemp, bforcenew);
				if( i == 1 )
					if( dctarget == null )
						return null;
					else
						return dctarget.ParentController;

				// The target controller is either a sizingController or a dockhostcontroller. In either case,
				// attempt to preserve the current sizing/positioning information.
				if(dctarget is SizingController)
					bforcenew = true;
				else
					bforcenew = false;

				nchildindex++;
				// If this sizingcontroller has only two children, excl splitters, then while docking the second child
				// use the first docked child to provide suitable border dock feedback info. However, if there are 3
				// or more children, then while redocking the third or higher child, use the position info and the previous
				// docked child's parent as the feedback controller and dock onto it. This will be similar to a dynamic
				// docking onto the splitter.

				// Size the next child as a percentage of the change in dimensions of the recently redocked previous child.
				if( dctarget != null )
				{
					Rectangle rcnextchild = Rectangle.Empty;

					if( !( ( dc is SizingController ) && ( dc as SizingController ).IsEmpty ) )
					{
						float deltaw = ( float )dctarget.LayoutRect.Width / ( float )( rclayout[nchildindex - 1].Width + rclayout[nchildindex].Width );
						float deltah = ( float )dctarget.LayoutRect.Height / ( float )( rclayout[nchildindex - 1].Height + rclayout[nchildindex].Height );
						rcnextchild = new Rectangle(rclayout[nchildindex].Left, rclayout[nchildindex].Top,
							( int )( rclayout[nchildindex].Width * deltaw ), ( int )( rclayout[nchildindex].Height * deltah ));
					}
					if( nchildindex < 2 )
					{
						Syncfusion.Windows.Forms.Tools.DockingStyle border = ( this.DICurrent.DP == DockPreference.Horizontal ) ? Syncfusion.Windows.Forms.Tools.DockingStyle.Right : Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom;
						ditemp = new DockInfo(dctarget, border, this.DICurrent.nPriority, nchildindex * 2 + 1,
							this.DICurrent.DP, rcnextchild);
					}
					else
					{
						ditemp = new DockInfo(dctarget.ParentController, dcnext.DICurrent.dStyle, this.DICurrent.nPriority,
							nchildindex * 2 + 1, this.DICurrent.DP, rcnextchild);
					}
				}
				dc = dcnext;
			}
			return null;
		}

		// A tabbed redocking is very simple. Run through all the children within this sizing controller and dock
		// them to the dockhost
		protected DockControllerBase DoRecTabbedRedocking(DockControllerBase dc, DockInfo di)
		{
			DockControllerBase dcchild = null;
			DockControllerBase dcnext = null;
			IEnumerator iechild = dc.ChildHostEnumerator;

			if(iechild == null)	// Does not have any children
				return dc.RedockController(di, false);

			if( iechild.MoveNext() == true)
				dcchild = iechild.Current as DockControllerBase;
			while(dcchild != null)
			{
				if(iechild.MoveNext() == true)
					dcnext = iechild.Current as DockControllerBase;
				// Set the new target controller to be previously docked child's parent tabcontroller
				di.dController = DoRecTabbedRedocking(dcchild, di);
				dcchild = dcnext;
				dcnext = null;
			}
			return di.dController;	// Returns a reference to the host tabcontroller
		}

		public override Size MinimumSize
		{
			get
			{
				Size minsize = Size.Empty;
				if(this.dpOrder == DockPreference.Horizontal)
				{
					foreach(DockControllerBase dcbase in this.alChildren)
					{
						Size dcsize = dcbase.MinimumSize;
						minsize.Width += dcsize.Width;
						if(dcsize.Height > minsize.Height)
							minsize.Height = dcsize.Height;
					}
				}
				else
				{
					foreach(DockControllerBase dcbase in this.alChildren)
					{
						Size dcsize = dcbase.MinimumSize;
						minsize.Height += dcsize.Height;
						if(dcsize.Width > minsize.Width)
							minsize.Width = dcsize.Width;
					}
				}
				return minsize;
			}
		}

		protected override void Dispose(bool bdisposing)
		{
			if((bdisposing == true) && (this.alChildren.Count > 0))
			{
				foreach(DockControllerBase dcbase in this.alChildren)
					dcbase.Dispose();
				this.alChildren.Clear();
			}
			base.Dispose(bdisposing);
		}

		internal override void AddWrapper(ControllerWrapper cw)
		{
			SizingControllerWrapper scw = new SizingControllerWrapper();
			scw.LayoutRect = LayoutRect;
			scw.Orientation = dpOrder;
			scw.TransientRect = this.DITransient.rcDockArea;
			scw.Style = DICurrent.dStyle;
			scw.Priority = DICurrent.nPriority;

			bool skipNextSizingController = false;
			foreach(DockControllerBase dcb in alChildren)
			{
				DockHostController dhc = dcb as DockHostController;
				if( (dhc != null) && (dhc.bInMDIMode == true) )
					skipNextSizingController = true;
				else
					if( skipNextSizingController == true )
						skipNextSizingController = false;
					else
						dcb.AddWrapper(scw);
			}
			
			if( scw.Children.Count > 0 )
				cw.Children.Add(scw);
		}	

		internal override void StoreControllers(ArrayList controllers)
		{
			base.StoreControllers (controllers);

			ArrayList array = new ArrayList( this.alChildren.ToArray(typeof(DockControllerBase)));			

			foreach( DockControllerBase dcb in array )
			{
				dcb.StoreControllers(controllers);
			}
			
			if( this.ParentController != null )
				this.ParentController.RemoveChild(this);
			controllers.Add(this);
		}

		internal override void ApplyWrapper(ControllerWrapper cw)
		{
			base.ApplyWrapper (cw);

			SizingControllerWrapper scw = (SizingControllerWrapper) cw;

			foreach( ControllerWrapper wrapper in cw.Children )
			{
				if( !(wrapper is DragSplitterControllerWrapper) ) 
					{
					DockControllerBase dcb = DockingManager.LocateController(wrapper);
					if( dcb != null ) 
					{
						InsertChild(dcb, ChildCount, dcb.DICurrent.dStyle);
						dcb.ApplyWrapper(wrapper);
					}
					else
					{
						// If tab group has only one page available, 
						// replace it with dock host of this page
						DockTabControllerWrapper tabWrapper = wrapper as DockTabControllerWrapper;
						if( tabWrapper != null )
						{
							DockHostController hostController = null;   
							int count = 0;
							foreach( String controlName in tabWrapper.Controls )
							{
								foreach( DockControllerBase controller in dockingMgr.alDockAreaControllers )
								{
									DockHostController dockHostController = controller as DockHostController;
									if( dockHostController != null )
									{
										string unName = controlName;
										if( unName.StartsWith( "DockHost_" ) )
											unName = unName.Remove( 0, "DockHost_".Length );
										if( dockHostController.UniqueName == unName )
										{
											hostController = dockHostController;
											count++;
										}
									}
								}
							}

							if( count == 1 )
							{
								DockingStyle dockingStyle = hostController.DICurrent.dStyle;
								InsertChild(hostController, ChildCount, dockingStyle );
								hostController.ApplyWrapper(wrapper);
							}
					}
				}
			}
		}

			if( alChildren.Count == 0 )
			{
				ParentController.RemoveChild( this );
			}
		}

		internal override bool IsEqual(ControllerWrapper cw)
		{
			if( cw is SizingControllerWrapper )
			{
				if( (cw as SizingControllerWrapper).Orientation == dpOrder )
				{
					return true;
				}
			}

			return false;
		}

		internal Direction CanResize
		{
			get
			{
				Direction resizeDirection = Direction.Both;
				ArrayList directions = new ArrayList();

				foreach( DockControllerBase dcb in this.alChildren )
				{
					if( dcb is SizingController )
					{
						SizingController sc = dcb as SizingController;

						if( !sc.IsEmpty )
							directions.Add( sc.CanResize );
					}
					else if( dcb is DockStateControllerBase && !(dcb is DockStateControllerWrapper))
					{
						DockStateControllerBase dscb = dcb as DockStateControllerBase;

						if( dscb.FreezeResize )
							directions.Add(dscb.MustResize);
						else
							directions.Add(Direction.Both);
					}
				}

				if( this.IsEmpty )
					resizeDirection = Direction.Both;
				else
				{
					resizeDirection = Direction.None;
					bool vertical = false;
					bool horizontal = false;

					if( directions.Count == 2 )
					{
						for( int i = 0; i < directions.Count; i++ )
						{
							Direction dir = (Direction)directions[i];

							if( this.DockingOrder == DockPreference.Horizontal )
							{
								if( dir == Direction.Both || dir == Direction.Horizontal )
									horizontal = true;

								if( dir == Direction.Both || dir == Direction.Vertical )
								{
									if( i == 0 )
										vertical = true;
								}
								else
									if( i == 1 )
										vertical = false;
							}
							else
							{
								if( dir == Direction.Both || dir == Direction.Vertical )
									vertical = true;

								if( dir == Direction.Both || dir == Direction.Horizontal )
								{
									if( i == 0 )
										horizontal = true;
								}
								else
									if( i == 1 )
										horizontal = false;
							}
						}

						if( horizontal )
							resizeDirection = resizeDirection | Direction.Horizontal;
						if( vertical )
							resizeDirection = resizeDirection | Direction.Vertical;
					}
					else if( directions.Count == 1 )
						resizeDirection = (Direction)directions[0];
				}

				if( this.ParentController == this.ToplevelController && !this.Floating)
					resizeDirection = CorrectResizeDirection(resizeDirection);

				return resizeDirection;
			}
		}

		private Direction CorrectResizeDirection( Direction resizeDirection )
		{
			if( this.DICurrent.dStyle == DockingStyle.Bottom || this.DICurrent.dStyle == DockingStyle.Top )
			{
				if( resizeDirection != Direction.Both && resizeDirection != Direction.Horizontal )
					this.MustResize = Direction.Horizontal;

				resizeDirection = resizeDirection | Direction.Horizontal;
			}
			else if( this.DICurrent.dStyle == DockingStyle.Left || this.DICurrent.dStyle == DockingStyle.Right )
			{
				if( resizeDirection != Direction.Both && resizeDirection != Direction.Vertical )
					this.MustResize = Direction.Vertical;

				resizeDirection = resizeDirection | Direction.Vertical;
			}
			else if( this.DICurrent.dStyle == DockingStyle.Fill )
			{
				if( resizeDirection == Direction.Vertical )
					this.MustResize = Direction.Horizontal;
				else if( resizeDirection == Direction.Horizontal )
					this.MustResize = Direction.Vertical;
				else if( resizeDirection == Direction.None )
					this.MustResize = Direction.Both;

				resizeDirection = Direction.Both;
			}

			return resizeDirection;
		}

		internal override Direction MustResize
		{
			get
			{
				return Direction.Both;
			}
			set
			{
				foreach( DockControllerBase dcb in this.alChildren )
					dcb.MustResize = value;
			}
		}
		
		internal override void ResizeControllers(ControllerWrapper cw)
		{
			SizingControllerWrapper scw = (SizingControllerWrapper) cw;
			
			rcLayout = scw.LayoutRect;
			DITransient.rcDockArea = Rectangle.Empty;
			dpOrder = scw.Orientation;
			DICurrent.dStyle = scw.Style;
			
			int itemCount =  Math.Min(ChildCount, cw.Children.Count);

			for( int i = 0; i < itemCount; i++ )
			{
				(alChildren[i] as DockControllerBase).
					ResizeControllers((ControllerWrapper) cw.Children[i]);
			}
		}

		private Size GetRedockSize()
		{
			DockControllerBase child = (DockControllerBase) alChildren[0];
			Size childSize = child.DITransient.rcDockArea.Size;
			int nWidth = 0, nHeight = 0;
			
			if( PriorityController != null )
			{
				foreach( DockControllerBase controller in alChildren )
				{
					IResizable resizable = (IResizable) controller;
					if( resizable.IsHorizontallyResizable() )
						nWidth += controller.LayoutRect.Width;
					if( resizable.IsVerticallyResizable() )
						nHeight += controller.LayoutRect.Height;
				}
			}
			else
			{
				foreach( DockControllerBase controller in alChildren )
				{
					IResizable resizable = (IResizable) controller;
					if( resizable.IsHorizontallyResizable() )
						nWidth += controller.DITransient.rcDockArea.Width;
					if( resizable.IsVerticallyResizable() )
						nHeight += controller.DITransient.rcDockArea.Height;
				}
			}

			if( this.PriorityController != null )
			{
				nWidth -= PriorityController.LayoutRect.Width;
				nHeight -= PriorityController.LayoutRect.Height;
			}
			
			nWidth = (DICurrent.DP == DockPreference.Vertical ? childSize.Width : nWidth);
			nHeight = (DICurrent.DP == DockPreference.Horizontal ? childSize.Height : nHeight);
			
			return new Size( nWidth, nHeight );
		}
		
		internal override bool IsFloatOnly()
		{
			foreach( DockControllerBase controller in alChildren )
			{
				if( controller.IsFloatOnly() )
					return true;
			}

			return false;
		}

		#region IResizable implementation
	
		public Size CalculateSize(Size parentSize, Size newParentSize)
		{
			SizingController sc = this.ParentController as SizingController;
			if( this.IsEmpty && sc != null)
			{
				Size emptySize = new Size();

				if( sc.DockingOrder == DockPreference.Vertical )
				{
					emptySize = new Size(sc.LayoutRect.Width, 0);
				}
				else
				{
					emptySize = new Size( 0, sc.LayoutRect.Height );
				}
				if( emptySize != this.LayoutRect.Size )
				{
					sc.HideRelatedSplitter(this);
					this.rcLayout.Size = emptySize;
					this.ParentController.AdjustLayout();
				}

				return emptySize;
			}
			else
				return ControllerSizeCalculator.CalculateSize(this, parentSize, newParentSize);
		}

		public bool IsVerticallyResizable()
		{
			Direction resizeDirection = this.CanResize;

			return Minimized == Minimization.None && !this.IsEmpty &&
				( resizeDirection == Direction.Vertical || resizeDirection == Direction.Both );
		}

		public bool IsHorizontallyResizable()
		{
			Direction resizeDirection = this.CanResize;

			return Minimized == Minimization.None && !this.IsEmpty &&
				( resizeDirection == Direction.Horizontal || resizeDirection == Direction.Both );
		}

		#endregion

		public override void DockAsMDIChild()
		{
			Array dcarray = alChildren.ToArray( typeof(DockControllerBase) );
			foreach( DockControllerBase controller in dcarray )
			{
				controller.DockAsMDIChild();
			}
		}
	}
}






