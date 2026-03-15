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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Diagnostics;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools.Renderers;
using System.Drawing.Text;
using Syncfusion.Windows.Forms.Tools.Win32API;

namespace Syncfusion.Windows.Forms.Tools
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class FloatingFormController : DockControllerBase
	{
		protected FloatingForm ctrlHost = null;
		protected internal DockControllerBase dcChild = null;
		protected int m_nImageIndex = -1;
        internal bool bClosingByMouse = false;
		public int ImageIndex
		{
			get { return this.m_nImageIndex; }
			set
			{
				if( this.m_nImageIndex != value )
				{
					this.m_nImageIndex = value;
                    this.ctrlHost.UpdateFormBorderStyle();
				}
			}
		}


		public override Control HostControl
		{
			get { return this.ctrlHost; }
		}

		public override Rectangle LayoutRect
		{
			get { return this.ctrlHost.ClientRectangle; }
			set
			{
				if(this.dcChild != null)
					this.dcChild.LayoutRect = value;
			}
		}

		internal override bool AllowFloating
		{
			get
			{
				bool allow = true;
				if( this.dcChild != null )
					allow = this.dcChild.AllowFloating;

				return allow;
			}
			set
			{}
		}

		public override bool Floating
		{
			get { return true; }
			set {}
		}

		public override int ChildCount
		{
			get	{ return (this.dcChild != null ) ? 1 : 0; }
		}

		public override int ChildHostCount
		{
			get
			{
				if( (this.dcChild != null) && (this.dcChild is DockHostController) )
					return 1;
				return 0;
			}
		}

		public override IEnumerator ChildHostEnumerator
		{
			get { return new IEnumWrapper(null); }
		}

		public override IEnumerator DCR
		{
			get	{ return this.dcChild.DCR; }
		}

		protected internal void ClearSharedFormReferences( DockControllerBase controller )
		{ 
			SizingController sc = controller as SizingController;

			if( sc != null )
			{
				foreach( DockControllerBase ctrl in sc.ChildControllers )
					ClearSharedFormReferences(ctrl);
			}
			else if( controller is DockStateControllerWrapper )
			{
				DockStateControllerWrapper dscw = controller as DockStateControllerWrapper;
				dscw.InternalControl.SharedForm = null;
			}
		}

		public FloatingFormController(DockingManager mgr, FloatingForm host) : base(mgr)
		{

            // Fix for 13244 
            if (mgr.HostControl is UserControl)
            {
                Form hostFrm = null;
                Form toplevelform = mgr.HostControl.TopLevelControl as Form;

                Control ctrl;
                if (mgr.HostControl.Parent != null)
                {
                    ctrl = mgr.HostControl.Parent;

                    while ((!(ctrl is Form)) && ctrl.Parent != null) //Iterating to get in to the form
                    {
                        ctrl = ctrl.Parent;
                    }
                    hostFrm = ctrl as Form;
                }

                if (hostFrm != null && toplevelform != null)
                {
                    if (hostFrm != toplevelform)
                    {
                        hostFrm.Resize += new EventHandler(hostFrm_Resize);
                    }
                }
            }

            // ---> upto this is for fix 13244

			this.ctrlHost = host;
			this.dockingMgr.AddFFController(this);

			// Subscribe to the host frame's resize event
			this.ctrlHost.Resize += new System.EventHandler(this.OnHostControlResize);
		}

        // Fix for 13244 
        private void hostFrm_Resize(object sender, EventArgs e)
        {
            MainFormController mfc = null;

            if(this.dockingMgr.alDockAreaControllers.Count >0)
                mfc = this.dockingMgr.alDockAreaControllers[0] as MainFormController;

            if (mfc != null)
            {
                Form hForm = sender as Form;
                if (hForm != null && hForm.WindowState == FormWindowState.Minimized)
                {
                    mfc.HideAllFloatingForms(true);
                }
                else if (hForm != null && hForm.WindowState != FormWindowState.Minimized)
                {
                    mfc.ShowAllFloatingForms(true);
                }
            }
        }
        // ---> upto this is for fix 13244

		public void OnHostControlResize(Object obj, EventArgs e)
		{
			LayoutRect = this.ctrlHost.ClientRectangle;
		}

		internal Direction CanResize()
		{
			Direction resize = Direction.Both;
			if( this.dcChild is DockStateControllerBase )
			{
				DockStateControllerBase dscb = this.dcChild as DockStateControllerBase;

				if( dscb.FreezeResize )
					resize = dscb.MustResize;
			}
			else if( this.dcChild is SizingController )
			{
				SizingController scChild = this.dcChild as SizingController;
				resize = scChild.CanResize;
			}

			return resize;
		}

		public override void AddChild(DockControllerBase dc, Syncfusion.Windows.Forms.Tools.DockingStyle db)
		{
			this.dcChild = dc;
			dc.DICurrent = new DockInfo(this, Syncfusion.Windows.Forms.Tools.DockingStyle.Fill, 0, 0, this.DICurrent.DP, dc.DICurrent.rcDockArea);
			dc.ParentController = this;
			// If the child is a DockHostController or DockTabController, then set the DockHost text as the Form caption.
			if( (dc is DockHostController) || ((dc is DockTabController) && (dc.HostControl != null)) )
				this.ctrlHost.Text = dc.HostControl.Text;
			else
			{
				this.ctrlHost.Text = " ";
			}
			NativeMethodsHelper.RedrawWindow( this.ctrlHost.Handle, NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE );
		}

		public override void InsertChild(DockControllerBase dc, int i, Syncfusion.Windows.Forms.Tools.DockingStyle db)
		{
			AddChild(dc, db);
		}

		public override void RemoveChild(DockControllerBase dc)
		{
			if( dc == this.dcChild )
			{
				this.dcChild = null;
				if( this.dockingMgr.HiddenFFControllers.Contains( this ) )
					this.dockingMgr.HiddenFFControllers.Remove( this );
				dc.ParentController = null;
			}
		}

		public override void ReplaceChild(DockControllerBase dccurrent, DockControllerBase dcnew)
		{
			if( dccurrent.Equals(this.dcChild) == false )
			{
				Debug.Assert(false, "Invalid Controller.\n");
				return;
			}
			Debug.Assert(dcnew != null);

			dcnew.DICurrent = new DockInfo(dccurrent.DICurrent);
			this.dcChild = dcnew;
			dcnew.ParentController = this;
			// If the replacement controller is a DockHostController or DockTabController, then set the DockHost text as the Form caption.
			if( (dcnew is DockHostController) || ((dcnew is DockTabController) && (dcnew.HostControl != null)) )
				this.ctrlHost.Text = dcnew.HostControl.Text;
			else
			{
				this.ctrlHost.Text = " ";
			}
		}

		public override DockControllerBase GetChildAt(int index)
		{
			return this.dcChild;
		}

		public override int GetChildHostIndex(DockControllerBase child)
		{
			return (this.dcChild.Equals(child)) ? 1 : -1;;
		}

		public override void AdjustLayout()
		{			
			if(this.dcChild != null)
			{
				this.dcChild.LayoutRect = this.LayoutRect;
			}
		}

		protected internal void RefreshFormCaption()
		{			
			if( this.dcChild is SizingController )
			{
				SizingController parentSizing = this.dcChild as SizingController;
				ArrayList controllers = parentSizing.GetDockControllers();

				if( controllers.Count == 1 )
				{					
					m_bHideChildCaptions = true;
					DockStateControllerBase baseController = controllers[0] 
						as DockStateControllerBase;
					SetCaptions(baseController);
				}
				else
				{
					this.HostControl.Text = String.Empty;
					NativeMethodsHelper.RedrawWindow(this.HostControl.Handle
						, NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE);
					m_bHideChildCaptions = false;
				}
			}
			else
			{
				m_bHideChildCaptions = true;

				SetCaptions(this.dcChild as DockStateControllerBase);
			}

            ctrlHost.SetCaption();
		}

		private void SetCaptions( DockStateControllerBase baseController )
		{
			if( baseController != null )
			{
				if( baseController is DockHostController )
				{
					DockHostController dhc = ( baseController as DockHostController );
					dhc.HideCaption = true;
					if( this.HostControl.Text != dhc.DockLabel )
					{
						this.HostControl.Text = dhc.DockLabel;
						NativeMethodsHelper.RedrawWindow(this.HostControl.Handle, NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE);
						dhc.AdjustLayout();
					}
				}
				else if( baseController is DockTabController )
				{
					DockHostController dhc = ( baseController as DockTabController ).HostController;

					foreach( DockTabPage page in ( baseController as DockTabController ).TabControl.TabPages )
						page.dhcClient.HideCaption = true;

					if( dhc != null )
					{
						dhc.HideCaption = true;
						this.HostControl.Text = dhc.DockLabel;
					}
					( baseController as DockTabController ).AdjustLayout();
				}
			}
		}

		private bool m_bHideChildCaptions = true;

		internal bool HideChildCaptions
		{
			get
			{
                return m_bHideChildCaptions;
			}
		}

		// Invoke close controller on child
		public override void CloseController()
		{
            if (this.dcChild != null)
            {
                if (this.dcChild is DockHostController)
                    (this.dcChild as DockHostController).bClosingByMouse = this.bClosingByMouse;
                else if (this.dcChild is DockTabController)
                    (this.dcChild as DockTabController).bClosingByMouse = this.bClosingByMouse;
                this.dcChild.CloseController();

            }
		}

		public override bool IsTargetController(Point ptscreen)
		{
			Rectangle rccaption = new Rectangle(this.ctrlHost.Location.X, this.ctrlHost.Location.Y, this.ctrlHost.Width, CaptionPainter.CaptionHeight+4);
			if( (this.ctrlHost.Visible == true) && (rccaption.Contains(ptscreen) == true) )
				return true;
			return false;
		}

		public override void GetDockInfo(Control ctrl, Point ptscreen, DockInfo di)
		{
			// Pass on the hit testing to the child controller. If this is a dockhostcontroller, then valid feedback
			// is returned. If the child is a sizing controller, then getdockinfo will be ignored
			if(this.dcChild != null)
			{
				DockStateControllerBase target = null;

				if(this.dcChild is DockHostController)
					this.dcChild.GetDockInfo(ctrl, ptscreen, di);
				else if(this.dcChild is DockTabController)
					(this.dcChild as DockTabController).HostController.GetDockInfo(ctrl, ptscreen, di);
				else if( this.dcChild is SizingController )
				{
					ArrayList dockControllers = ( this.dcChild as SizingController ).GetDockControllers();
					if( dockControllers.Count == 1 && dockControllers[0] != null )
						target = dockControllers[0] as DockStateControllerBase;

					if( target is DockHostController )
						target.GetDockInfo(ctrl, ptscreen, di);
					else if( target is DockTabController )
						( target as DockTabController ).HostController.GetDockInfo(ctrl, ptscreen, di);
				}
			}
		}

		public override bool QueryDropProceedWithDock(Control ctrldrop, Syncfusion.Windows.Forms.Tools.DockingStyle style)
		{
			if(this.dcChild != null)
			{
				DockStateControllerBase target = null;

				if(this.dcChild is DockHostController)
					return this.dcChild.QueryDropProceedWithDock(ctrldrop, style);
				else if(this.dcChild is DockTabController)
					return (this.dcChild as DockTabController).HostController.QueryDropProceedWithDock(ctrldrop, style);
				else if( this.dcChild is SizingController )
				{
					ArrayList dockControllers = ( this.dcChild as SizingController ).GetDockControllers();
					if( dockControllers.Count == 1 && dockControllers[0] != null )
						target = dockControllers[0] as DockStateControllerBase;

					if( target is DockHostController )
						return target.QueryDropProceedWithDock(ctrldrop, style);
					else if( target is DockTabController )
						return ( target as DockTabController ).HostController.QueryDropProceedWithDock(ctrldrop, style);
				}
			}
			return false;
		}
		public override ArrayList ChildControllers
		{
			get
			{
				ArrayList child = new ArrayList();
				child.Add( this.dcChild );
				return child;
			}
		}

		public void InvokeRedocking()
		{
			// Verify that the child controller is a sizing controller
			SizingController sc = this.dcChild as SizingController;
			Debug.Assert( (sc != null), "Error: Invalid cast.\n");
			bool freezed = this.dockingMgr.ForbidFreeze;
			this.dockingMgr.ForbidFreeze = true;

			// Undock the child controller from	the floating controller and dock it to the new feedback controller
			DockInfo dicurrent = this.dockInfoCurrent;			

			if( this.dcChild is SizingController )
			{
				DockStateControllerBase target = null;
				ArrayList dockControllers = ( this.dcChild as SizingController ).GetDockControllers();
				if( dockControllers.Count == 1 && dockControllers[0] != null )
				{	
					target = dockControllers[0] as DockStateControllerBase;
					target.RedockController(dicurrent, false);
				}
				else
					this.dcChild.RedockController(dicurrent, false);
			}
			else
				this.dcChild.RedockController(dicurrent, false);

			this.DICurrent = DockInfo.NullInfo;
			this.dockingMgr.ForbidFreeze = freezed;
			if( dicurrent.dController != null )
				dicurrent.dController.AdjustLayout();
		}

		public void InvokeDCRRedocking()
		{
			// Iterate the child controller hierarchy and issue a InvokePrevDockFloatTransition() call on each controller
			DoRecDCRRedocking(this.dcChild);
		}

		protected void DoRecDCRRedocking(DockControllerBase dc)
		{
			DockControllerBase dcchild = null;
			DockControllerBase dcnext = null;
			IEnumerator iechild = dc.ChildHostEnumerator;

			if(iechild == null)	// Does not have any children
			{
				dc.InvokePrevDockFloatTransition( true );
				return;
			}

			if( iechild.MoveNext() == true)
				dcchild = iechild.Current as DockControllerBase;
			while(dcchild != null)
			{
				if(iechild.MoveNext() == true)
				{
					dcnext = iechild.Current as DockControllerBase;
					// If the next child is a tabcontroller, then switch the two so that the tabcontroller is always
					// redocked first.
					if(dcnext is DockTabController)
					{
						DockControllerBase dctemp = dcnext;
						dcnext = dcchild;
						dcchild = dctemp;
					}
				}
				// Set the new target controller to be previously docked child's parent tabcontroller
				if( !(dcchild is DockStateControllerWrapper) )
					DoRecDCRRedocking(dcchild);
				dcchild = dcnext;
				dcnext = null;
			}
		}

		internal ControllerWrapper GetWrapper()
		{
			FloatingFormControllerWrapper ffcw = null;
			if( this.dcChild != null )
			{
				ffcw = new FloatingFormControllerWrapper();
				ffcw.LayoutRect = this.HostControl.Bounds;

				this.dcChild.AddWrapper( ffcw );
			}

			return ffcw;
		}

		public override bool AttemptDCRDocking(DockControllerBase dc, IEnumerator iedcr)
		{
			if(this.dcChild != null)
				return this.dcChild.AttemptDCRDocking(dc, iedcr);
			return false;
		}

		protected internal void HandleMouseDownImp(MouseButtons button, Point ptclient)
		{
			if(button == MouseButtons.Left)
			{
				if( !this.dockingMgr.DesignProcess && this.DockingManager.DragProvider is BorderDragProvider )
					this.ctrlHost.Capture = true;
				this.ctrlHost.RefreshRenderer();
				if( this.dcChild is DockHostController )
				{
					Point ptchildclient = this.dcChild.HostControl.PointToClient( this.ctrlHost.PointToScreen( ptclient ) );
					( this.dcChild as DockHostController ).HandleMouseDownImp( MouseButtons.Left, ptchildclient );
				}
				else if( this.dcChild is DockTabController )
				{
					Point ptchildclient = this.dcChild.HostControl.PointToClient( this.ctrlHost.PointToScreen( ptclient ) );
					DockTabController dtc = this.dcChild as DockTabController;
					dtc.HostController.HandleMouseDownImp( MouseButtons.Left, ptchildclient );
				}
				else
				{
					SizingController sc = this.dcChild as SizingController;
					ArrayList controls = sc.GetDockControllers();
					if( controls.Count == 1 )
					{
						DockControllerBase child = controls[0] as DockControllerBase;

						DockHostController dhc = child as DockHostController;
						if( dhc != null )
						{
							Point ptchildclient = dhc.HostControl.PointToClient( this.ctrlHost.PointToScreen( ptclient ) );
							dhc.HandleMouseDownImp( MouseButtons.Left, ptchildclient );
						}
						else
						{
							DockTabController dtc = child as DockTabController;
							if( dtc != null )
							{
								Point ptchildclient = dtc.HostControl.PointToClient( this.ctrlHost.PointToScreen( ptclient ) );
								dtc.HostController.HandleMouseDownImp( MouseButtons.Left, ptchildclient );
							}
						}
					}
					else
						this.dockingMgr.DragProvider.ProcessMouseDown( this, ctrlHost, ctrlHost.PointToScreen( ptclient ) );
				}
			}
		}

		protected internal void HandleMouseUpImp(MouseButtons button, Point ptclient)
		{
			if( button == MouseButtons.Left )
			{
				// If after the drag operation, the controller is null, then just move the frame. Else dock
				// the contents of this frame to the said controller
				if( dcChild != null )
				{
					if(this.dcChild is DockHostController)
					{
						Point ptchildclient = this.dcChild.HostControl.PointToClient(this.ctrlHost.PointToScreen(ptclient));
						(this.dcChild as DockHostController).HandleMouseUpImp(MouseButtons.Left, ptchildclient);
					}
					else if(this.dcChild is DockTabController)
					{
						Point ptchildclient = this.dcChild.HostControl.PointToClient(this.ctrlHost.PointToScreen(ptclient));
						DockTabController dtc = this.dcChild as DockTabController;
						dtc.HostController.HandleMouseUpImp(MouseButtons.Left, ptchildclient);
					}
					else
					{
						DockControllerBase targetCtrl = this.DICurrent.dController;
						Point dragPoint = Point.Empty;
						SizingController sc = this.dcChild as SizingController;
						ArrayList controls = sc.GetDockControllers();
						bool dragged = false;
						if( controls.Count == 1 )
						{
							DockControllerBase child = controls[0] as DockControllerBase;

							DockHostController dhc = child as DockHostController;
							if( dhc != null )
							{
								Point ptchildclient = dhc.HostControl.PointToClient( this.ctrlHost.PointToScreen( ptclient ) );
								dragPoint = dhc.DINew.rcDockArea.Location;
								dhc.HandleMouseUpImp( MouseButtons.Left, ptchildclient );
							}
							else
							{
								DockTabController dtc = child as DockTabController;
								if( dtc != null )
								{
									Point ptchildclient = dtc.HostControl.PointToClient( this.ctrlHost.PointToScreen( ptclient ) );
									dragPoint = dtc.HostController.DINew.rcDockArea.Location;
									dtc.HostController.HandleMouseUpImp( MouseButtons.Left, ptchildclient );
								}
							}
						}
						else
						{
							dragged = (this.dockingMgr.DragProvider.DraggingControl == this.HostControl);
							this.dockingMgr.DragProvider.ProcessMouseUp( this, ctrlHost, ctrlHost.PointToScreen( ptclient ) );
						}
						// Get hold of the controller with client focus and restore focus to that control after the redocking
						DockControllerBase dcfocus = GetChildControllerWithFocus(this);
						DockingManager.LockActivationEvents++;
						try
						{
							if( this.DICurrent.dController != null )
								this.InvokeRedocking();
							if((this.DockingManager.DesignProcess == false) && (dcfocus != null) && (this.ctrlHost.LastActiveControl != null))
								this.ctrlHost.LastActiveControl.Focus();
						}
						finally
						{
							DockingManager.LockActivationEvents--;
						}
						if( targetCtrl == null && dragged
							&& this.dockingMgr.DragProvider is BorderDragProvider )
						{
							this.ctrlHost.Capture = false;
							if( dragPoint == Point.Empty )
								dragPoint = this.DICurrent.rcDockArea.Location;

							this.ctrlHost.Location = dragPoint;
						}
					}
				}
			}

			if(button == MouseButtons.Right)
			{
				int nCaptionHeight = ( DockingManager.VisualStyle == VisualStyle.Default
					|| DockingManager.VisualStyle == VisualStyle.VS2005 )? 
					SystemInformation.ToolWindowCaptionHeight + 1/*border width*/:
					DockingManager.Renderer.CaptionWidth + DockingManager.Renderer.BorderWidth;
				if( ((ptclient.X >= 0) && (ptclient.X <= this.ctrlHost.Width)) && ((ptclient.Y >= -nCaptionHeight) && (ptclient.Y <= 0)) )
				{
					DockControllerBase dcfocus = this.GetChildControllerWithFocus(this);
					if( dcfocus == null )
					{
						SetChildControlFocus(this);
						dcfocus = GetChildControllerWithFocus(this);
					}
					if((dcfocus != null) && (dcfocus is DockHostController))
						this.dockingMgr.ShowMenu((dcfocus as DockHostController), this.ctrlHost.PointToScreen(ptclient));
				}
			}
		}

		protected internal void HandleMouseMoveImp(MouseButtons button, Point ptclient)
		{
			VirtualKeys vKey = WindowsAPI.GetSystemMetrics( SystemMetricsCodes.SM_SWAPBUTTON ) != 0 ? VirtualKeys.VK_RBUTTON : VirtualKeys.VK_LBUTTON;

			if( button == MouseButtons.Left && this.dockingMgr.DragProvider.DraggingControl != null
				&& ( WindowsAPI.GetKeyState( (int)VirtualKeys.VK_LBUTTON ) & 0x8000 ) == 0 )
			{
				this.dockingMgr.DragProvider.ForceStopDrag();
				button = MouseButtons.None;
				Point ptScreen = this.HostControl.PointToScreen( ptclient );
				NativeMethods.PostMessage( this.HostControl.Handle, NativeMethods.WM_LBUTTONUP, IntPtr.Zero
					, new IntPtr( NativeMethods.MAKELPARAM( ptScreen.X, ptScreen.Y ) ) );
			}

			if( button == MouseButtons.Left )
			{
				if( this.dcChild is DockHostController )
				{
					Point ptchildclient = this.dcChild.HostControl.PointToClient( this.ctrlHost.PointToScreen( ptclient ) );
					( this.dcChild as DockHostController ).HandleMouseMoveImp( MouseButtons.Left, ptchildclient );
				}
				else if( this.dcChild is DockTabController )
				{
					Point ptchildclient = this.dcChild.HostControl.PointToClient( this.ctrlHost.PointToScreen( ptclient ) );
					DockTabController dtc = this.dcChild as DockTabController;
					dtc.HostController.HandleMouseMoveImp( MouseButtons.Left, ptchildclient );
				}
				else
				{
					SizingController sc = this.dcChild as SizingController;
					ArrayList controls = sc.GetDockControllers();
					if( controls.Count == 1 )
					{
						DockControllerBase child = controls[0] as DockControllerBase;

						DockHostController dhc = child as DockHostController;
						if( dhc != null )
						{
							Point ptchildclient = dhc.HostControl.PointToClient( this.ctrlHost.PointToScreen( ptclient ) );
							dhc.HandleMouseMoveImp( MouseButtons.Left, ptchildclient );
						}
						else
						{ 
							DockTabController dtc = child as DockTabController;
							if( dtc != null )
							{
								Point ptchildclient = dtc.HostControl.PointToClient( this.ctrlHost.PointToScreen( ptclient ) );
								dtc.HostController.HandleMouseMoveImp( MouseButtons.Left, ptchildclient );
							}
						}
					}
					else
						this.dockingMgr.DragProvider.ProcessMouseMove( this, ctrlHost, ctrlHost.PointToScreen( ptclient ) );
				}
			}
		}

		protected internal void HandleDoubleClickImp(Point ptclient)
		{
			if( !dcChild.IsFloatOnly() )
			{
				int captionht = 
					( dockingMgr.VisualStyle == VisualStyle.Default
					|| dockingMgr.VisualStyle == VisualStyle.VS2005 )?
					SystemInformation.ToolWindowCaptionHeight : 
					dockingMgr.Renderer.CaptionWidth;
				if(XPThemes.IsThemedOS && XPThemes.IsThemeActive)
					captionht += 4;
				Control activeChild = null;
				DockHost activeDh = this.ctrlHost.ActiveControl as DockHost;
				if( activeDh != null )
					activeChild = activeDh.ActiveControl;
				if( ((ptclient.X >= 0) && (ptclient.X <= this.ctrlHost.Width)) && ((ptclient.Y >= (0-captionht)) && (ptclient.Y <= 0)) )
				{
					bool bprocessdoubleclick = true;
					if(this.dcChild is DockHostController)
					{
						// If the form has only one dockhost in it then initiate DragAllow
						// Fire the DragAllow event, and provide a chance to preempt the drag
						DragAllowEventArgs dragallow = new DragAllowEventArgs(this.dcChild.HostControl.Controls[0]);
						this.dockingMgr.FireDragAllowEvent(dragallow);
						bprocessdoubleclick = !dragallow.Cancel;
					}

					if(bprocessdoubleclick)
					{
						this.dockingMgr.DragProvider.ProcessDoubleClick();

						// Get hold of the controller with client focus and restore focus to that control after the redocking
						DockControllerBase dcfocus = GetChildControllerWithFocus(this);
						this.InvokeDCRRedocking();
						if(this.dockingMgr.DesignProcess == false)
						{
							if(dcfocus != null && dcfocus.HostControl.Controls.Count > 0)
							{
								if( activeChild != null )
									activeChild.Focus();
								else if(this.ctrlHost.LastActiveControl != null)
									this.ctrlHost.LastActiveControl.Focus();
								else
									dcfocus.HostControl.Controls[0].Focus();
							}
						}
						else if(this.dockingMgr.DesignMode == true)
						{
							this.dockingMgr.UpdateDesigner(); // Forcibly update the designer state
						}
					}
				}
			}
		}

        internal void ToggleFloatToDock(DockBehavior dock)
        {
            if (!dcChild.IsFloatOnly() && dock == DockBehavior.VS2010)
            {
                Control activeChild = null;
                DockHost activeDh = this.ctrlHost.ActiveControl as DockHost;
                if (activeDh != null)
                    activeChild = activeDh.ActiveControl;
                if (this.dockingMgr.DockBehavior == DockBehavior.VS2010)
                {
                    DockControllerBase dcfocus = GetChildControllerWithFocus(this);
                    this.InvokeDCRRedocking();
                    if (this.dockingMgr.DesignProcess == false)
                    {
                        if (dcfocus != null && dcfocus.HostControl.Controls.Count > 0)
                        {
                            if (activeChild != null)
                                activeChild.Focus();
                            else if (this.ctrlHost.LastActiveControl != null)
                                this.ctrlHost.LastActiveControl.Focus();
                            else
                                dcfocus.HostControl.Controls[0].Focus();
                        }
                    }
                }
            }
        }

		protected DockControllerBase GetChildControllerWithFocus(DockControllerBase dc)
		{
			if(dc.ChildCount <= 0)
			{
				if( dc.HostControl != null && dc.HostControl.ContainsFocus == true)
					return dc;
				return null;
			}

			for(int i = 0; i < dc.ChildCount; i++)
			{
				DockControllerBase dcfocus = GetChildControllerWithFocus(dc.GetChildAt(i));
				if(dcfocus != null)
					return dcfocus;
			}
			return null;
		}

		protected void SetChildControlFocus(DockControllerBase dc)
		{
			if( dc.ChildCount <= 0 && dc.HostControl != null )
			{
				dc.HostControl.Focus();
			}
			else
			{
				DockControllerBase focused = dc.GetChildAt(0);

				if( focused.HostControl == null
					&& this.dcChild is SizingController )
				{
					ArrayList dockControllers = ( this.dcChild as SizingController ).GetDockControllers();

					if( dockControllers.Count != 0 )
						focused = dockControllers[0] as DockControllerBase;
				}

				SetChildControlFocus(focused);
			}
		}

		public override Size MinimumSize
		{
			get
			{
				if(this.dcChild != null)
					return this.dcChild.MinimumSize;
				return Size.Empty;
			}
		}

		protected override void Dispose(bool bdisposing)
		{
			if((bdisposing == true) && (this.ctrlHost != null))
			{
				this.dockingMgr.RemoveFFController(this);

				if(this.dcChild != null)
				{
					this.dcChild.Dispose();
					this.dcChild = null;
				}

				this.ctrlHost.Resize -= new System.EventHandler(this.OnHostControlResize);
				this.ctrlHost.Dispose();
				this.ctrlHost = null;
			}
			base.Dispose(bdisposing);
		}

		public override void ApplyDockInfo()
		{
			base.ApplyDockInfo ();
		}
		internal override bool IsFloatOnly()
		{
			return dcChild.IsFloatOnly();
		}

		public override void DockAsMDIChild()
		{
			dcChild.DockAsMDIChild();			
		}

		public override void UpdateControl()
		{
			ctrlHost.UpdateControlBoxVisibility();
            ctrlHost.UpdateFormBorderStyle();
			NativeMethodsHelper.RedrawWindow( ctrlHost.Handle, NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_FRAME );
		}
	}

    public enum AutoHideStatus
    {
        Collapsed,
        Expanded
    }

	[ToolboxItem(false)]
	[Syncfusion.Documentation.DocumentationExclude()]
	public class FloatingForm : System.Windows.Forms.Form
		, IDraggable
		, IDockingManagerDesignerMouseHook
		, IDockable
	{
		protected FloatingFormController dcInternal = null;
		protected Rectangle rcDrag = Rectangle.Empty;
		internal int nDsgnrNCHit = 0;
		internal Point ptInitNCHit = Point.Empty;
		protected string formCaption = "";
		protected bool bActive = false;
        protected bool bCloseButtonVisibility = true;
        protected bool bAutoHideButtonVisibility = false;
		protected bool m_bUsed = true;
		protected DockStateControllerBase m_owner = null;
		private Control m_lastActiveControl = null;
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		private bool bAllowShowToolTip = false;
#endif

		private const int DEF_TEXT_IMAGE_INDENT = 4;
		private const int DEF_TEXT_BORDER_INDENT = 3;
		private bool m_bCancelMode = false;
		private bool m_bInSizeMove = false;
		private Point m_dragOffset = Point.Empty;
        private bool cancelVisibleChanging = false;
        //Floating Window Caption Height
        private int CaptionHeight = 29;
        private int MetroCaptionHeight = 24;
        //AutoHide Collapse Window Size
        internal Size AutoHideCollapseSize;
        internal Size FloatingWindowsSize;

		static protected int nNamingCount = 0;

		internal Point DragOffset
		{
			get
			{
				return m_dragOffset;
			}
			set
			{
				if( m_dragOffset != value )
				{
					m_dragOffset = value;
				}
			}
		}

		internal bool InSizeMove
		{
			get
			{
				return m_bInSizeMove;
			}
		}

		internal DockStateControllerBase FormOwner
		{
			get
			{
				return m_owner;
			}
			set
			{
				m_owner = value;
			}
		}

		internal bool Used
		{
			get
			{
				return m_bUsed;
			}
			set
			{
				if( m_bUsed != value )
				{
					m_bUsed = value;
				}
			}
		}

		public void Enable()
		{
			Enable( true );
		}

		public void Enable( bool show )
		{
			this.Visible = show;
			if( InternalController != null &&
				!InternalController.DockingManager.DesignMode )
				this.Enabled = true;
			Form ownerForm = InternalController.DockingManager.HostControl as Form;
			if (ownerForm == null)
			{
				ownerForm = InternalController.DockingManager.frmOwner;
			}
			this.Owner = null;
			this.Owner = ownerForm;
			this.InternalController.DockingManager.HiddenFFControllers.Remove( this.dcInternal );
			this.InternalController.DockingManager.AddFFController(this.dcInternal);
		}

		public void Disable()
		{
			if( InternalController != null &&
				!InternalController.DockingManager.DesignMode )
				this.Enabled = false;
			this.InternalController.DockingManager.RemoveFFController(this.dcInternal);
			if( !this.InternalController.DockingManager.HiddenFFControllers.Contains( this.dcInternal ) )
				this.InternalController.DockingManager.HiddenFFControllers.Add( this.dcInternal );
			this.Visible = false;
			this.StopToolTipTimer();
		}

		public Rectangle ImageRect
		{
			get
			{
				return GetImageRect( Bounds );
			}
		}

		protected Rectangle GetImageRect( RectangleF rcBoundsF )
		{
			return GetImageRect( new Rectangle( (int)rcBoundsF.Left, (int)rcBoundsF.Top, 
				(int)rcBoundsF.Width, (int)rcBoundsF.Height ) );
		}

		protected Rectangle GetImageRect( Rectangle rcBounds )
		{
			if( dcInternal.ImageIndex < 0 || dcInternal.DockingManager.ImageList == null ||
				dcInternal.ImageIndex >= dcInternal.DockingManager.ImageList.Images.Count || 
				!dcInternal.DockingManager.ShowCaptionImages )
				return Rectangle.Empty;
			int closeBtnWidth = ( ControlBox ) ? SystemInformation.ToolWindowCaptionButtonSize.Width : 0;
			if( Width <= closeBtnWidth + SystemInformation.SmallIconSize.Width 
				+ 2 * SystemInformation.FrameBorderSize.Width )
			{
				return Rectangle.Empty;
			}	

			// Check RightToLeft orientation of current FloatingForm, detect left and 
			// top location of its caption. Also calculate size of the icon.
			int nIconIndent = 1;
			int nIconSize = 
				(SystemInformation.ToolWindowCaptionHeight > SystemInformation.SmallIconSize.Height)?
				SystemInformation.SmallIconSize.Height:
				SystemInformation.ToolWindowCaptionHeight - nIconIndent;
			int nTop = SystemInformation.BorderSize.Height + (SystemInformation.ToolWindowCaptionHeight - nIconSize ) / 2 + nIconIndent;
			if( !XPThemes.IsThemedOS || !XPThemes.IsThemeActive )
			{
				nTop += SystemInformation.Border3DSize.Height;
			}
			int nLeft = ( (this.CreateParams.ExStyle & NativeMethods.WS_EX_LAYOUTRTL) == 0 ) ?  
				SystemInformation.FrameBorderSize.Width + nIconIndent:
				rcBounds.Width - SystemInformation.FrameBorderSize.Width - nIconSize - nIconIndent; 
			return new Rectangle( nLeft, nTop, nIconSize, nIconSize);
		}

		public bool CloseButtonVisibility
		{
			get
			{
				return this.bCloseButtonVisibility;
			}
			set
			{
				if( this.bCloseButtonVisibility != value )
				{
					this.bCloseButtonVisibility = value;
					PaintNCArea();
				}
			}
		}

        private AutoHideStatus m_AutoHideMode = AutoHideStatus.Expanded;
        /// <summary>
        /// Gets/Sets value to define AutoHideMode in Floating Form
        /// </summary>
        public AutoHideStatus AutoHideMode
        {
            get 
            { 
                return m_AutoHideMode; 
            }
            set 
            { 
                if(m_AutoHideMode != value)
                    m_AutoHideMode = value; 
            }
        }
        
        /// <summary>
        /// Gets/Sets value to display AutoHideButton in Floating Form
        /// </summary>
        public bool AutoHideButtonVisibility
        {
            get
            {
                return this.bAutoHideButtonVisibility;
            }
            set
            {
                if (this.bAutoHideButtonVisibility != value)
                {
                    this.bAutoHideButtonVisibility = value;
                    PaintNCArea();
                }
            }
        }

		public DockControllerBase InternalController
		{
			get { return this.dcInternal; }
			set
			{
				this.dcInternal = value as FloatingFormController;
			}
		}

		public DockInfo DragDockInfo
		{
			get { return this.dcInternal.DICurrent; }
			set { this.dcInternal.DICurrent = value; }
		}

		public Rectangle DragRectangle
		{
			get
			{
				return new Rectangle(this.Location.X, this.Location.Y, this.Bounds.Width, this.Bounds.Height);
			}

			set { }
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams CP = base.CreateParams;
				if( !base.DesignMode && dcInternal != null )
				{
					if( dcInternal.DockingManager.IsMirrored )
					{
						int version = NativeMethods.GetVersion();
						if( NativeMethods.LOWORD( version ) != 4 )
						{ // Ignore the following code when we are running under Windows NT 4.0 ( Workaround to bug 2181 ).
							CP.ExStyle |=
								NativeMethods.WS_EX_NOINHERITLAYOUT;
							if ( dcInternal.DockingManager.VisualStyle == VisualStyle.Default 
								|| dcInternal.DockingManager.VisualStyle == VisualStyle.VS2005 )
							{
								CP.ExStyle |= NativeMethods.WS_EX_LAYOUTRTL;
							}
							else
							{
								CP.ExStyle |= NativeMethods.WS_EX_RTLREADING;
							}
						}
					}

					CP.Style |= NativeMethods.WS_POPUP;
					
					// Set initial location outside the screen to prevent painting issues.
					CP.X = Screen.PrimaryScreen.Bounds.Width;
					CP.Y = Screen.PrimaryScreen.Bounds.Height;
				}
				return CP;
			}
		}

		internal Control LastActiveControl
		{
			get { return this.m_lastActiveControl; }
			set { this.m_lastActiveControl = value; }
		}

		// helper window for handling activation/deactivation events
		private NotifyNativeWindow nativeWindow = null;
		public FloatingForm(DockingManager dmgr)
		{
			this.dcInternal = new FloatingFormController(dmgr, this);
			m_internalTip = new ToolTip();

			m_internalTip.ReshowDelay = 500;
			m_internalTip.InitialDelay = c_initialDelay;
			m_timer = new Timer();
			m_timer.Interval = c_initialDelay;
			

			this.Visible = false;

			if(this.dcInternal.DockingManager.HostControl is Form)
				this.Owner = this.dcInternal.DockingManager.HostControl as Form;
			else
			{
				if(this.dcInternal.DockingManager.DesignProcess == false)
				{
					if(this.dcInternal.DockingManager.frmOwner != null)
						this.Owner = this.dcInternal.DockingManager.frmOwner;
				}
				else	// In design time - could be in design mode or in a hosted control
				{
					this.TopMost = true;
				}
			}
			if(this.Owner != null)
			{
				this.BackColor = this.Owner.BackColor;
				this.ForeColor = this.Owner.ForeColor;
			}

            UpdateFormBorderStyle();

			this.ShowInTaskbar = false;
			this.Size = new Size(0,0);	// Set 0 size to prevent flash

			this.CreateControl();
            this.CloseButtonVisibility = dmgr.CloseEnabled;
			FloatingForm.nNamingCount++;
			this.Name = String.Concat("FloatingForm_", FloatingForm.nNamingCount.ToString());

			nativeWindow = new NotifyNativeWindow( dcInternal.DockingManager,
				this );
			nativeWindow.AssignHandle( Handle );

			SubscribeToResizeEvent( true );

            AutoHideCollapseSize = new Size(this.dcInternal.HostControl.Width, 30);
		}

        /// <summary>
        /// Updates Form BorderStyles, Icon and caption text for Vista Aero Themed OS.
        /// </summary>
        public void UpdateFormBorderStyle()
        {
			if( Environment.OSVersion.Version.Major == 6 && this.Visible )
			{
                if (!IsDefaultRendering())
                {
                    if (NativeMethods.IsCompositionEnabled() && !m_dwmNCRenderingDisabled)
                    {
						NativeMethods.SetDwmNCRendering( this.Handle, false );
                    }
                }
                else
                {
                    if (NativeMethods.IsCompositionEnabled() && m_dwmNCRenderingDisabled)
                    {
						NativeMethods.SetDwmNCRendering( this.Handle, true );
                    }
                }
			}

            if (dcInternal.DockingManager.FreezeResizing && this.FormBorderStyle != FormBorderStyle.FixedToolWindow)
                this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            else if (!dcInternal.DockingManager.FreezeResizing && this.FormBorderStyle != FormBorderStyle.SizableToolWindow)
                this.FormBorderStyle = FormBorderStyle.SizableToolWindow;

			SetCaption();
        }

        /// <summary>
        /// Performs toggling auto hide state in floating forms
        /// </summary>
        internal void AutoHideToggleAnimation()
        {
            if (this.dcInternal != null && this.dcInternal.DockingManager != null && this.dcInternal.HostControl != null)
            {
                if (this.dcInternal.HostControl.Height > 31)
                {
                    if (this.dcInternal.DockingManager.VisualStyle == VisualStyle.Metro)
                        this.dcInternal.HostControl.Size = new Size(this.dcInternal.HostControl.Width, MetroCaptionHeight);
                    else
                        this.dcInternal.HostControl.Size = new Size(this.dcInternal.HostControl.Width, CaptionHeight);
                    this.AutoHideMode = AutoHideStatus.Collapsed;
                }
                else
                {
                    this.dcInternal.HostControl.Size = FloatingWindowsSize;
                    this.AutoHideMode = AutoHideStatus.Expanded;
                }
            }
        }

		internal void SetCaption()
		{
			if( IsDefaultRendering() )
			{
				if( HasIcon() )
				{
					Bitmap bmp = ( Bitmap )dcInternal.DockingManager.ImageList.Images[dcInternal.ImageIndex];
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
					this.ShowIcon = true;
#endif
					this.Icon = System.Drawing.Icon.FromHandle( bmp.GetHicon() );
				}
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				else
					this.ShowIcon = false;
#else
				this.Icon = null;
#endif

				if( base.Text != formCaption )
					base.Text = formCaption;
			}
		}

        //Check if a Icon must be drawn.
        private bool HasIcon()
        {
            if (dcInternal.ImageIndex < 0 || dcInternal.DockingManager.ImageList == null ||
                dcInternal.ImageIndex >= dcInternal.DockingManager.ImageList.Images.Count ||
                !dcInternal.DockingManager.ShowCaptionImages)
                return false;

            return true;
		}

		// Implementation of the IDraggable interface methods
		public bool IsSuitableDockTarget(DockControllerBase dc)
		{
			bool suitable = true;
			DockStateControllerBase baseCtrl = dc as DockStateControllerBase;

			if( baseCtrl != null && baseCtrl.AutoHideMode )
				suitable = false;

			// Disallow docking on all dockhosts housed in self
//			ContainerControl cntrtrl = dc.HostControl.GetContainerControl() as ContainerControl;
//			Form pf = cntrtrl.ParentForm;
//			if( (cntrtrl.Equals(this)) || ((pf != null) && (pf.Equals(this))) )
//				return false;
//			else
//				return true;

			if(dc.HostControl.Parent == this)
				suitable = false;
			return suitable;
		}

		public bool InitiateDrag(MouseAction action, Point ptscreen)
		{
			int captionht = ( dcInternal.DockingManager.VisualStyle == VisualStyle.Default 
				|| dcInternal.DockingManager.VisualStyle == VisualStyle.VS2005 )? 
				SystemInformation.ToolWindowCaptionHeight:
				renderer.CaptionWidth;
			if(XPThemes.IsThemedOS && XPThemes.IsThemeActive)
				captionht += 4;
			Point ptclient = this.PointToClient(ptscreen);
			if( ((ptclient.X >= 0) && (ptclient.X <= this.Width)) && ((ptclient.Y >= 0-captionht) && (ptclient.Y <= 0)) )
			{
				return true;
			}
			else
			{
				return false;
			}
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

			dcInternal.DockingManager.DragProvider.ForceStopDrag();

			if(this.Capture == true)
				this.Capture = false;
			this.dcInternal.DockingManager.DragProvider.TerminateDrag(this, Point.Empty);

			this.dcInternal.DICurrent = DockInfo.NullInfo;

			// Forcibly update the designer state
			if(this.dcInternal.DockingManager.DesignMode == true)
				this.dcInternal.DockingManager.UpdateDesigner();
		}

		public bool QueryDragProceedWithDock()
		{
			return true;
		}

        //Indicates if the Default//VS2005 style is set with Vista Aero Theme enabled.
        public bool IsDefaultRendering()
        {
            if (dcInternal == null)
                return false;
            if (dcInternal.DockingManager.IsDefaultRendering())
                return true;
            return false;
        }

		public override string Text
		{
			get
			{
				return " ";
			}
			set
			{
				if( this.formCaption != value )
				{
					this.formCaption = value;
					this.OnTextChanged( EventArgs.Empty );
				}
			}
		}

		protected Font GetCaptionFont()
		{
			return dcInternal.DockingManager.CaptionTextFont;
		}

		protected Rectangle GetTextRectFromBounds( RectangleF bounds, ref Rectangle imageRect )
		{
			imageRect = GetImageRect( bounds );

			int frameWidth = SystemInformation.FrameBorderSize.Width;
			int captBtnWidth = (CloseButtonVisibility) ?
				SystemInformation.ToolWindowCaptionButtonSize.Width : 
				0;

			Rectangle textRect = new Rectangle(
				0,
				SystemInformation.FrameBorderSize.Height,
				Convert.ToInt32(bounds.Width) - captBtnWidth,
				SystemInformation.ToolWindowCaptionHeight
				);

			textRect.Inflate( -( DEF_TEXT_BORDER_INDENT + frameWidth ), 0 );

			if( dcInternal.DockingManager.IsMirrored )
				textRect.Offset( captBtnWidth, 0 );

			if( imageRect != Rectangle.Empty )
			{
				if( dcInternal.DockingManager.ShowCaptionImages )
				{
					long windowStyles = NativeMethods.GetWindowLong( this.Handle, NativeMethods.GWL_EXSTYLE );
					textRect.Width -= ( imageRect.Width + DEF_TEXT_IMAGE_INDENT );

					if( ( windowStyles | NativeMethods.WS_EX_LAYOUTRTL ) != windowStyles )
					{
						// RightToLeft.No
						textRect.X += imageRect.Width + DEF_TEXT_IMAGE_INDENT;
					}
				}
				else
					imageRect = Rectangle.Empty;
			}

			return textRect;
		}

		protected internal void DrawWindowTextAndImage( ref Message msg )
		{
			IntPtr hdc = NativeMethods.GetWindowDC( msg.HWnd );
            try
            {
                if (hdc != IntPtr.Zero)
                {
                    // Painted text can't be mirrored so set layout to LTR
                    int version = NativeMethods.GetVersion();
                    if (NativeMethods.LOWORD(version) != 4)
                    { // Ignore this code when we are running under Windows NT 4.0 ( Workaround to bug 2181 ).
                        NativeMethods.SetLayout(hdc, 0);
                    }

                    using (Graphics g = Graphics.FromHdc(hdc))
                    {
                        using (SolidBrush brush = new SolidBrush(
                            (bActive) ?
                            SystemColors.ActiveCaptionText :
                            SystemColors.InactiveCaptionText
                            ))
                        {
                            StringFormat sf = new StringFormat();
                            sf.FormatFlags = StringFormatFlags.NoWrap;

                            if (this.dcInternal.DockingManager.IsMirrored)
                                sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

                            switch (dcInternal.DockingManager.DockLabelAlignment)
                            {
                                case DockLabelAlignmentStyle.Left:
                                    sf.Alignment = StringAlignment.Near;
                                    break;
                                case DockLabelAlignmentStyle.Center:
                                    sf.Alignment = StringAlignment.Center;
                                    break;
                                case DockLabelAlignmentStyle.Right:
                                    sf.Alignment = StringAlignment.Far;
                                    break;
                                default:
                                    sf.Alignment = StringAlignment.Near;
                                    break;
                            }
                            sf.LineAlignment = StringAlignment.Center;
                            sf.Trimming = StringTrimming.EllipsisCharacter;
                            Rectangle rcImage = Rectangle.Empty;

                            Rectangle rcBounds = GetTextRectFromBounds(g.VisibleClipBounds, ref rcImage);
                            Font sFont = GetCaptionFont();

                            if (rcBounds.Width > 0)
                            {
                                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                                g.DrawString(formCaption, sFont, brush, rcBounds, sf);
                            }


                            if (!rcImage.Equals(Rectangle.Empty) && dcInternal.DockingManager.ImageList != null
                                && dcInternal.DockingManager.ImageList.Images.Count > 0)
                            {
                                Image image = dcInternal.DockingManager.ImageList.Images[dcInternal.ImageIndex];
                                g.DrawImage(image, rcImage);
                            }
                            sf.Dispose();
                        }
                    }
                    msg.Result = (IntPtr)1;
                }
            }
            finally
            {
                NativeMethods.ReleaseDC(msg.HWnd, hdc);
            }
			dcInternal.DockingManager.ctrlLastPainted = this;
		}
		/// <summary>
		/// Defnes if to passby windows messages.
		/// </summary>
		private bool m_bPassbyHitTest = false;

		/// <summary>
		/// Gets/sets PassbyMessages value.
		/// </summary>
		internal bool PassbyHitTest
		{
			get
			{
				return m_bPassbyHitTest;
			}
			set
			{
				if( m_bPassbyHitTest != value )
				{
					m_bPassbyHitTest = value;
				}
			}
		}

        void EnsureDwmNCRendering(bool enabled)
        {
            if (NativeMethods.IsCompositionEnabled())
            {
                NativeMethods.SetDwmNCRendering(this.Handle, enabled);
            }
        }

		private ToolTip m_internalTip;
		private IntPtr m_lastHitTest;
		private Point m_dragPoint = Point.Empty;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		private bool m_toolTipVisible = false;
#endif
        bool m_dwmNCRenderingDisabled = false;
		protected override void WndProc(ref Message msg)
		{
			if(this.dcInternal != null)
			{
				if(this.dcInternal.DockingManager.DesignProcess == false)
				{
					switch(msg.Msg)
					{
                        case NativeMethods.WM_DWMNCRENDERINGCHANGED:
                        {
                            if((int)msg.WParam > 0)
                                m_dwmNCRenderingDisabled = false;
                            else
                                m_dwmNCRenderingDisabled = true;
                            base.WndProc(ref msg);

							break;
                        }
						case NativeMethods.WM_NCCALCSIZE:
						{
							base.WndProc( ref msg );
							if( dcInternal.DockingManager.VisualStyle != VisualStyle.Default
								&& dcInternal.DockingManager.VisualStyle != VisualStyle.VS2005 )
							{
								ProcessNCCalcSize( msg );
                                if (dcInternal.DockingManager.NeedFloatFormRepaint)
                                {
                                    NativeMethods.RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero,
                                        NativeMethods.RDW_FRAME | NativeMethods.RDW_INVALIDATE);
                                }
							}
							break;
						}

						case NativeMethods.WM_ENTERSIZEMOVE:
							this.m_bInSizeMove = true;
							base.WndProc( ref msg );
							break;

						case NativeMethods.WM_MOVING:
						{
							base.WndProc( ref msg );

							if( !SystemInformation.DragFullWindows )
							{
								NativeMethods.RECT bounds =
									( NativeMethods.RECT )Marshal.PtrToStructure( msg.LParam, typeof( NativeMethods.RECT ) );
								this.Location = new Point( bounds.left, bounds.top );
							}

							break;
						}

						case NativeMethods.WM_ACTIVATE:
						{
							base.WndProc( ref msg );
							if( dcInternal.DockingManager.VisualStyle != VisualStyle.Default
								&& dcInternal.DockingManager.VisualStyle != VisualStyle.VS2005
								&& this.Visible )
							{
								renderer.ControlBounds = new Rectangle( Point.Empty, Size);
								PaintNCArea();
							}
							break;
						}

						case NativeMethods.WM_MOUSEACTIVATE:
							base.WndProc(ref msg);
							if( ActiveControl != null )
							{
								DockHost dHost = this.ActiveControl as DockHost;
								if( dHost != null )
									this.dcInternal.DockingManager.DHCInFocus = dHost.InternalController as DockHostController;
							}
							break;

						case NativeMethods.WM_SIZING:
							NativeMethods.RECT rectangle = 
								(NativeMethods.RECT)Marshal.PtrToStructure(msg.LParam, typeof(NativeMethods.RECT));
							Direction resize = this.dcInternal.CanResize();

							if( ( resize & Direction.Horizontal ) != Direction.Horizontal )
							{
								if( msg.WParam.ToInt32() == NativeMethods.WMSZ_LEFT
									|| msg.WParam.ToInt32() == NativeMethods.WMSZ_TOPLEFT 
									|| msg.WParam.ToInt32() == NativeMethods.WMSZ_BOTTOMLEFT )
									rectangle.left = this.Left;

								rectangle.Width = this.Width;
							}
							if( ( resize & Direction.Vertical ) != Direction.Vertical )
							{
								if( msg.WParam.ToInt32() == NativeMethods.WMSZ_TOP
									|| msg.WParam.ToInt32() == NativeMethods.WMSZ_TOPLEFT
									|| msg.WParam.ToInt32() == NativeMethods.WMSZ_TOPRIGHT )
									rectangle.top = this.Top;

								rectangle.Height = this.Height;
							}

							Marshal.StructureToPtr(rectangle, msg.LParam, true);
							break;

						case 0x0085: /*WM_NCPAINT */
						{
							if( dcInternal.DockingManager.VisualStyle == VisualStyle.Default
								|| dcInternal.DockingManager.VisualStyle == VisualStyle.VS2005 )
							{
								base.WndProc( ref msg );
                                if(!IsDefaultRendering())
									if( this.Text == " " )
										this.DrawWindowTextAndImage( ref msg );
							}
							else
							{
                                if (NativeMethods.IsDwmNCRenderingEnabled(this.Handle))
                                    base.WndProc(ref msg);
                                else
                                {
                                    renderer.ControlBounds = new Rectangle(Point.Empty, Size);
                                    PaintNCArea();
                                }
							}
							break;

						}
						case NativeMethods.WM_WINDOWPOSCHANGING:
							DockingManager dockMan = this.InternalController.DockingManager;

							if( dockMan.EscapeKeyPressed || m_bCancelMode )
							{
								m_bCancelMode = false;
								NativeMethods.WINDOWPOS windowpos = ( NativeMethods.WINDOWPOS )
									msg.GetLParam( typeof( NativeMethods.WINDOWPOS ) );
								if( windowpos.flags == NativeMethods.SWP_NOSIZE )
								{
									windowpos.flags |= NativeMethods.SWP_NOMOVE;
									Marshal.StructureToPtr( windowpos, msg.LParam, true );

									if( dockMan.DragProvider.DraggingControl != null )
										dockMan.DragProvider.DraggingControl.AbortDrag();
								}
								base.WndProc( ref msg );
							}
							break;
						case 0x0086: /*WM_NCACTIVATE*/
							if( this.Visible )
							{
								if( this.Controls.Count > 0 && this.ActiveControl != null )
									this.m_lastActiveControl = ( this.ActiveControl as DockHost ).ActiveControl;

								if( msg.WParam == IntPtr.Zero )
									this.bActive = false;
								else
									this.bActive = true;
								base.WndProc( ref msg );

								if( dcInternal.DockingManager.VisualStyle == VisualStyle.Default
								|| dcInternal.DockingManager.VisualStyle == VisualStyle.VS2005 )
								{
									if(!IsDefaultRendering())
										if( this.Text == " " )
											this.DrawWindowTextAndImage( ref msg );
								}
								else
								{
									renderer.ControlBounds = new Rectangle( Point.Empty, Size );
									PaintNCArea();
								}
							}
							else
								base.WndProc( ref msg );
							break;

						case NativeMethods.WM_DISPLAYCHANGE:
							UpdateFormPosition( NativeMethods.LOWORD( msg.LParam ), NativeMethods.HIWORD( msg.LParam ) );
							break;

						case NativeMethods.WM_NCHITTEST:
						case NativeMethods.WM_NCMOUSELEAVE:
							if( PassbyHitTest )								
							{
								msg.Result = (IntPtr) NativeMethods.HTTRANSPARENT;
								break;
							}
							
							base.WndProc( ref msg );
							if( dcInternal.DockingManager.VisualStyle == VisualStyle.Default
							|| dcInternal.DockingManager.VisualStyle == VisualStyle.VS2005 )
							{
								if (msg.Msg == NativeMethods.WM_NCHITTEST)
								{
									int hitCloseButton = NativeMethods.HTCLOSE;
									int hitCaption = NativeMethods.HTCAPTION;

									m_lastHitTest = msg.Result;
									if (msg.Result == new IntPtr(hitCloseButton))
									{
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
										if(bAllowShowToolTip)
#endif
										{
											msg.Result = new IntPtr(hitCaption);
											StartToolTipTimer();
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
											bAllowShowToolTip = false;
#endif
										}
									}
									else
									{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
										HideToolTip();
#else
										bAllowShowToolTip = true;
#endif
									}
								}
							}
							else
							{
								Point point = new Point( (System.Int16)NativeMethods.LOWORD( msg.LParam ),
									(System.Int16)NativeMethods.HIWORD( msg.LParam ) );
								point = PointToClient( point );
                                point.Offset(renderer.ThickBorderWidth,
                                    renderer.ThickBorderWidth + renderer.CaptionWidth);
                                RefreshRenderer();
								HitTestArea newArea = renderer.HitTest( MouseButtons.None, point );
                                if (newArea != hitArea ||
                                     dcInternal.DockingManager.Renderer.GetHitButtonIndex() != dcInternal.DockingManager.Renderer.GetHighlightedButtonIndex() && dcInternal.DockingManager.VisualStyle != VisualStyle.Metro)
                                {
                                    hitArea = newArea;
                                    HideToolTip();
                                    hitButton = this.renderer.GetHitButton();
                                    PaintNCArea();
                                }
                                else if (dcInternal.DockingManager.Renderer.GetHitButtonIndex() != dcInternal.DockingManager.Renderer.GetHighlightedButtonIndex())
                                {
                                    hitArea = newArea;
                                    hitButton = this.renderer.GetHitButton();
                                    PaintNCArea();
                                }
								if (newArea == HitTestArea.Button)
								{
									StartToolTipTimer();
								}
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
								else
								{
									HideToolTip();
								}
#endif
                                //CaptionButton button = this.renderer.GetHitButton();
                                //if( msg.Msg == NativeMethods.WM_NCHITTEST && newArea == HitTestArea.Button && 
                                //    button != null && button.Type == CaptionButtonType.Close )
                                //    msg.Result = new IntPtr(NativeMethods.HTCLOSE);
							}

							if (msg.Msg == NativeMethods.WM_NCMOUSELEAVE)
							{
								HideToolTip();
							}
							break;

						case 0x0010 /*WM_CLOSE*/:
                            this.dcInternal.bClosingByMouse = true;
                            this.dcInternal.CloseController();
                            this.dcInternal.bClosingByMouse = false;
                            if (this.dcInternal != null)
                            {
                                FloatingFormController controller = this.dcInternal as FloatingFormController;
                                if (controller != null && controller.HostControl != null)
                                {
                                    FloatingForm fForm = controller.HostControl as FloatingForm;                                    
                                    if (fForm != null && !cancelVisibleChanging)
                                    {
                                        if (controller.ChildControllers[0] is DockTabController)
                                        {
                                            DockTabController dtController = controller.ChildControllers[0] as DockTabController;
                                            DockTabControl dockTabControl = dtController.TabControl as DockTabControl;
                                            if(dockTabControl.TabPages.Count == 0)
                                                fForm.Disable();
                                        }
                                        else if (controller.ChildControllers[0] is DockHostController)
                                        {
                                            //do nothing.
                                        }
                                        else
                                            fForm.Disable();
                                    }
                                }
                            }
							break;

						case NativeMethods.WM_NCLBUTTONUP:
							if( dcInternal.DockingManager.VisualStyle != VisualStyle.Default
								&& dcInternal.DockingManager.VisualStyle != VisualStyle.VS2005 )
								if( this.hitArea == HitTestArea.Button )
								{
                                    if (hitButton.Type == CaptionButtonType.Close)
                                    {
                                        PaintNCArea();
                                        this.dcInternal.bClosingByMouse = true;
                                        dcInternal.CloseController();
                                        this.dcInternal.bClosingByMouse = false;
                                    }
                                    else
                                    {
                                        hitButton.FireClickEvent(new CancelEventArgs(false));
                                    }
								}
							m_dragPoint = Point.Empty;
							if( !SystemInformation.DragFullWindows )
								this.Capture = false;
							break;

						case 0x00A1 /*WM_NCLBUTTONDOWN*/:
							if( dcInternal.DockingManager.VisualStyle == VisualStyle.Default
								|| dcInternal.DockingManager.VisualStyle == VisualStyle.VS2005 
								)
							{
                                if(!IsDefaultRendering())
									msg.WParam = m_lastHitTest;
							}
							else
							{
								PaintNCArea();
							}
								
							if(  (dcInternal.DockingManager.VisualStyle == VisualStyle.Default)
								|| (dcInternal.DockingManager.VisualStyle == VisualStyle.VS2005)
								|| (( hitArea != HitTestArea.Button )) )
							{
								int wparam = (int)msg.WParam;
								if(wparam == 2/*HTCAPTION*/)
								{
									// Do custom mousedown processing
									Point ptscreen = Cursor.Position;
									m_dragPoint = ptscreen;

									DockingManager dockingManager = dcInternal.DockingManager;
									dockingManager.StopActivationEvents = true;
									try
									{
										if(this.ContainsFocus == false)
										{
											if(this.Controls.Count > 0 && this.Controls[ 0 ].Controls.Count > 0 && this.m_lastActiveControl != null )
											{												
												this.m_lastActiveControl.Focus();
											}
											else
												this.Focus();
										}
									}
									finally
									{
										dockingManager.StopActivationEvents = false;
									}
                                    this.RefreshRenderer();
									Control dragging = this.dcInternal.DockingManager.DragProvider.DraggingControl as Control;
									if( !this.Capture && dragging != this && !this.Controls.Contains(dragging) )
									{
										// Delegate the message to the controller that has the focus
										DockControllerBase dcchild = this.dcInternal.GetChildAt( 0 );
										if( dcchild is DockHostController )
										{
											DockHostController dhc = dcchild as DockHostController;
											dhc.HandleMouseDownImp( MouseButtons.Left, dhc.HostControl.PointToClient( ptscreen ) );
										}
										else if( dcchild is DockTabController )
										{
											DockTabController dtc = dcchild as DockTabController;
											dtc.HostController.HandleMouseDownImp( MouseButtons.Left, dtc.HostControl.PointToClient( ptscreen ) );
										}
										else
										{
											this.dcInternal.HandleMouseDownImp( MouseButtons.Left, this.PointToClient( ptscreen ) );
										}
									}
								}
							}

							if( msg.WParam.ToInt32() == NativeMethods.HTCAPTION && !SystemInformation.DragFullWindows )
							{
								if( m_dragOffset == Point.Empty )
								{
									int xPos = NativeMethods.LOWORD( msg.LParam );
									int yPos = NativeMethods.HIWORD( msg.LParam );
									m_dragPoint = new Point( xPos - this.Location.X, yPos - this.Location.Y );
								}
								else
									m_dragPoint = m_dragOffset;

								m_dragOffset = Point.Empty;
								this.Capture = true;
							}
							else
								base.WndProc( ref msg );

							break;

						case NativeMethods.WM_EXITSIZEMOVE:
							if( this.Visible && !this.dcInternal.DockingManager.EscapeKeyPressed )
							{
								Point location = Cursor.Position;
								this.HandleMouseUp( MouseButtons.Left, location );
							}
							base.WndProc( ref msg );
							this.m_bInSizeMove = false;
							break;

						case NativeMethods.WM_CANCELMODE:
							m_bCancelMode = true;
							base.WndProc( ref msg );
							break;

						case 0x00A3 /*WM_NCLBUTTONDBLCLK*/:
							if((int)msg.WParam == 2/*HTCAPTION*/)
							{
                                if (dcInternal.DockingManager.EnableDoubleClickOnCaption && hitArea != HitTestArea.Button)
                                {
                                    Point ptscreen = Cursor.Position;
                                    DockingManager dockingMgr = dcInternal.DockingManager;
                                    if (dockingMgr.DockBehavior == DockBehavior.VS2008)
                                    {
                                        dockingMgr.LockActivationEvents++;
                                        try
                                        {
                                            DockHost activeDh = this.ActiveControl as DockHost;
                                            if (activeDh != null)
                                                this.m_lastActiveControl = activeDh.ActiveControl;
                                            this.HandleDoubleClick(ptscreen);
                                        }
                                        finally
                                        {
                                            dockingMgr.LockActivationEvents--;
                                        }
                                    }
                                    else if (dockingMgr.DockBehavior == DockBehavior.VS2010)
                                    {
                                        MaximumToggleAnimation();
                                        this.GetPaintInfo();
                                    }
                                }
                                else if ((this.dcInternal != null) && (this.dcInternal.DockingManager.DesignProcess == false) && !(dcInternal.DockingManager.EnableDoubleClickOnCaption) && hitArea != HitTestArea.Button)
                                {
                                    if (dcInternal.DockingManager.ActiveControl != null)
                                    {
                                        dcInternal.DockingManager.RaiseCaptionDoubleClick(dcInternal.DockingManager.ActiveControl);
                                    }
                                }
								break;
							}
							goto default;
						case 0x00A4 /*WM_NCRBUTTONDOWN*/:
							return;
						case 0x00A5	/*WM_NCRBUTTONUP*/:
							if((int)msg.WParam == 2/*HTCAPTION*/)
							{
								this.dcInternal.HandleMouseUpImp(MouseButtons.Right, this.PointToClient(Cursor.Position));
								break;
							}
							goto default;
						case 0x020B /*WM_XBUTTONDOWN*/:
						case 0x020C /*WM_XBUTTONUP*/:
							return;
						default:
							base.WndProc(ref msg);
							break;
					}
				}
				else // (this.dcInternal.DockingManager.DesignProcess == true)
				{
					if( (msg.Msg == 0x0010/*WM_CLOSE*/) ||
						(((msg.Msg == 0x00A0/*WM_NCMOUSEMOVE*/)||(msg.Msg == 0x00A1/*WM_NCLBUTTONDOWN*/)||(msg.Msg == 0x00A2/*WM_NCLBUTTONUP*/)) && ((int)msg.WParam == 20/*HTCLOSE*/))
						|| (msg.Msg == 0x00A3/*WM_NCLBUTTONDBLCLK*/)
						)
					{
						return;
					}
					base.WndProc(ref msg);
				}
			}
			else
			{
				base.WndProc(ref msg);
			}
		}

        internal bool CancelVisibleChanging
        {
            get { return cancelVisibleChanging; }
            set { cancelVisibleChanging = value; }
        }

        private void RepaintChildren()
        {
            foreach (Control c in this.Controls)
            {
                c.Invalidate(true);
                c.Update();
            }
        }

		internal void RefreshRenderer()
		{
			if( dcInternal.DockingManager.ctrlLastPainted != this )
			{
				PaintDockControlArgs args = GetPaintInfo();
				renderer.RefreshPaintInfo( new Rectangle( Point.Empty, Size ), args );
                dcInternal.DockingManager.ctrlLastPainted = this;
			}
		}

		private readonly Point c_toolTipOffset = new Point(5, 25);
		private const int c_autopopDelay = 5000;
		private const int c_initialDelay = 1000;
		private Timer m_timer = null;
		private bool m_timerRunning = false;

		/// <summary>
		/// Shows close button's tool tip.
		/// </summary>
		private void StartToolTipTimer()
		{
			if( !m_timerRunning )
			{
				m_timer.Tick += new EventHandler( ShowTimerTick );
				m_timer.Start();
				m_timerRunning = true;
			}
		}

		private void ShowTimerTick( object sender, EventArgs e )
		{
			StopToolTipTimer();

			ShowToolTip();
		}

		private void StopToolTipTimer()
		{
			if( m_timer != null )
			{
				m_timer.Stop();
				m_timer.Tick -= new EventHandler( ShowTimerTick );
				m_timerRunning = false;
			}
		}

		private void ShowToolTip()
		{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			if( !m_toolTipVisible )
			{
				m_toolTipVisible = true;
				Point location = Point.Empty;

				if( this.dcInternal.DockingManager.EnableSuperToolTip )
				{
					location = c_toolTipOffset;
					location.Offset(Cursor.Position);
					ToolTipInfo tipInfo = null;
					CaptionButton captionButton = null;

                    //foreach( CaptionButton button in this.dcInternal.DockingManager.CaptionButtons )
                    //{
                    //    if( button.Type == CaptionButtonType.Close )
                    //    {
                    //        closeButton = button;
                    //        break;
                    //    }
                    //}

                    captionButton = this.renderer.GetHitButton();

					SuperToolTip dockingToolTip = this.dcInternal.DockingManager.SuperToolTip;
                    if (dockingToolTip != null && captionButton != null)
					{
                        dockingToolTip.ToolTipDuration = this.dcInternal.DockingManager.ToolTipInterval / 1000;
                        if (this.dcInternal.DockingManager.UseBalloonStyleToolTip)
                        {
                            dockingToolTip.Style = SuperToolTip.SuperToolTipStyle.Balloon;
                        }
                        else
                        {
                            dockingToolTip.Style = SuperToolTip.SuperToolTipStyle.Normal;
                        }

                        if (captionButton.SuperToolTipInfo == null)
                            captionButton.SuperToolTipInfo = new ToolTipInfo();

                        tipInfo = captionButton.SuperToolTipInfo;

						if( tipInfo.Body.Text == null && tipInfo.Footer.Text == null && tipInfo.Header.Text == null )
                            tipInfo.Body.Text = captionButton.ToolTip;

						dockingToolTip.Show( tipInfo, location, c_autopopDelay );
					}
				}
				else
				{
					if( this.InternalController.DockingManager != null )
					{
						location = this.PointToClient( Cursor.Position );
                        if (!this.dcInternal.DockingManager.UseBalloonStyleToolTip)
                        {
                            location.Offset(c_toolTipOffset.X * 2, c_toolTipOffset.Y * 2);
                        }
                        else
                        {
                            location.Offset(c_toolTipOffset.X, c_toolTipOffset.Y);
                        }
						string ToolTipText = string.Empty;
                        if (this.dcInternal.DockingManager.ShowToolTips)
                        {
                            CaptionButton button = this.renderer.GetHitButton();
                            if (button != null)
                            {
                                ToolTipText = button.ToolTip;
                            }
                            
                        }
                        if (m_internalTip != null)
                        {
                            m_internalTip.IsBalloon = this.dcInternal.DockingManager.UseBalloonStyleToolTip;
                            m_internalTip.AutoPopDelay = this.dcInternal.DockingManager.ToolTipInterval;
                            m_internalTip.Show(ToolTipText,
                                this,location, c_autopopDelay);
                        }
					}
				}
			}
#else
            if(	m_internalTip!=null)
			m_internalTip.SetToolTip(this, dcInternal.DockingManager.GetCloseButtonToolTip() );
#endif
        }

		protected override void OnMouseLeave( EventArgs e )
		{
			HideToolTip();
			base.OnMouseLeave( e );
		}

		/// <summary>
		/// Hide close button's tool tip.
		/// </summary>
		private void HideToolTip()
		{
			if( m_timerRunning )
			{
				m_timer.Stop();
				m_timer.Tick -= new EventHandler( ShowTimerTick );
				m_timerRunning = false;
			}
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			if (m_toolTipVisible)
			{
				m_toolTipVisible = false;
				if( this.dcInternal.DockingManager.EnableSuperToolTip )
				{
					SuperToolTip dockingToolTip = this.dcInternal.DockingManager.SuperToolTip;
					if( dockingToolTip != null )
						dockingToolTip.Hide();
				}
				else
				{
					m_internalTip.Hide(this);
				}
			}
#endif
		}

		private DockingManagerRenderer renderer
		{
			get { return dcInternal.DockingManager.Renderer; }
		}

		int borderWidth = 0;
		int captionWidth = 0;

		internal void ProcessNCCalcSize( Message m )
		{
			if ( m.WParam != IntPtr.Zero )
			{
				NativeMethods.NCCALCSIZE_PARAMS csp;
				borderWidth = SystemInformation.FrameBorderSize.Width;
				captionWidth = SystemInformation.ToolWindowCaptionHeight;
				csp = (NativeMethods.NCCALCSIZE_PARAMS) Marshal.PtrToStructure( m.LParam,
					typeof(NativeMethods.NCCALCSIZE_PARAMS));
				csp.rgrc0.left += ( renderer.ThickBorderWidth - borderWidth);
				csp.rgrc0.right -= (renderer.ThickBorderWidth - borderWidth);
				csp.rgrc0.top += (renderer.CaptionWidth - captionWidth)
					+ (renderer.ThickBorderWidth - borderWidth); 
				csp.rgrc0.bottom -= (renderer.ThickBorderWidth - borderWidth);

				Marshal.StructureToPtr( csp, m.LParam, false );
			}
		}

		internal PaintDockControlArgs GetPaintInfo()
		{
			CaptionButtonState closeButtonState = CaptionButtonState.Normal;

			if( hitArea == HitTestArea.Button )
				closeButtonState = GetActiveButtonState();
			this.dcInternal.DockingManager.UpdateFloatingFormsImages();
			Caption caption = new Caption();
			caption.CaptionState = bActive ? CaptionState.Active :
				CaptionState.Normal;
			caption.TextAlignment = dcInternal.DockingManager.DockLabelAlignment;
			caption.Font = dcInternal.DockingManager.CaptionTextFont;
			caption.Text = formCaption;
			int imageIndex = (dcInternal.DockingManager.ShowCaptionImages) ? dcInternal.ImageIndex : -1;

			CaptionButtonOptionsTable buttons = new CaptionButtonOptionsTable();
            //if( CloseButtonVisibility )
            //{
            //    CaptionButton button = new CaptionButton( CaptionButtonType.Close );
            //    if (dcInternal.DockingManager.CaptionButtons.Count > 0 && dcInternal.DockingManager.CaptionButtons[0].Type == CaptionButtonType.Close)
            //    {
            //        button.ImageIndex = dcInternal.DockingManager.CaptionButtons[0].ImageIndex;
            //    }
            //    CaptionButtonOptions options = new CaptionButtonOptions();
            //    buttons.Add( button, options );
            //}

            #region Custom Buttons

            if (Controls.Count > 0)
            {
                DockHost host = Controls[0] as DockHost;
                Control ctrl = host.Controls[0];

                CaptionButtonsCollection captionButtons = dcInternal.DockingManager.GetCustomCaptionButtons(ctrl);
                if (captionButtons != null)
                {
                    for (int i = 0; i < captionButtons.Count; i++)
                    {
                        CaptionButton button = captionButtons[i];
                        CaptionButtonOptions options = new CaptionButtonOptions();
                        switch (button.Type)
                        {
                            case CaptionButtonType.Close:
                                if (CloseButtonVisibility)
                                {
                                    buttons.Add(button, options);
                                }
                                break;
                            case CaptionButtonType.Pin:
                                if (AutoHideButtonVisibility)
                                {
                                    options.ModifiedView = (dcInternal.HostControl.Size.Height > 31) ? false : true;
                                    buttons.Add(button, options);
                                }
                                break;
                            case CaptionButtonType.Maximize:
                                if (this.dcInternal.DockingManager.DockBehavior == DockBehavior.VS2010)
                                {
                                    options.ModifiedView = (this.WindowState != FormWindowState.Maximized) ? false : true;
                                    buttons.Add(button, options);
                                }
                                break;
                            case CaptionButtonType.Menu:
                                if (this.dcInternal.DockingManager.DockBehavior == DockBehavior.VS2010)
                                {
                                    options.ModifiedView = true;
                                    buttons.Add(button, options);
                                }
                                break;
                            case CaptionButtonType.Custom:
                                if (dcInternal.DockingManager.ShowCustomButtonsInFloating)
                                {
                                    buttons.Add(captionButtons[i], options);
                                }
                                break;
                        }
                    }
                }
            }


            #endregion

            PaintDockControlArgs args = new PaintDockControlArgs( 
				caption, buttons, true, true, imageIndex, dcInternal.DockingManager.ImageList );
			ProvideGraphicsItemsEventArgs pgargs = null;

			if( this.dcInternal.HideChildCaptions )
			{
				Control dockedControl = null;

				foreach( Control floatChild in this.Controls )
				{
					DockHost dh = floatChild as DockHost;

					if( dh != null && dh.Controls.Count > 0 )
					{
						dockedControl = dh.Controls[0];

						if( this.dcInternal.DockingManager.alEnableDocking.Contains( dockedControl )
							&& dockedControl.Visible )
							break;
						else
							dockedControl = null;
					}
				}

				if( this.dcInternal.DockingManager.VisualStyle != VisualStyle.Default )
				{
                    if (dockedControl != null)
                    {
                        pgargs = new ProvideGraphicsItemsEventArgs(
                            dockedControl,
                            dcInternal.DockingManager.Renderer.CaptionBounds,
                            this.bActive);
                    }
				}
				else
					pgargs = new ProvideGraphicsItemsEventArgs( null, Rectangle.Empty, false );
			}
			else
				pgargs = new ProvideGraphicsItemsEventArgs( null, Rectangle.Empty, false );

			args.ProvideGraphicsItemsArgs = pgargs;

			args.DesignMode = dcInternal.DockingManager.DesignProcess;
			return args;
		}

		internal void PaintNCArea()
		{
			if( this.InternalController.DockingManager.VisualStyle == VisualStyle.Default 
				||this.InternalController.DockingManager.VisualStyle == VisualStyle.VS2005 )
				NativeMethodsHelper.RedrawWindow(this.Handle, NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_FRAME);
			else
			{
				IntPtr hdc = NativeMethods.GetWindowDC(Handle);
                try
                {
                    using (Graphics g = Graphics.FromHdc(hdc))
                    {
                        PaintDockControlArgs args = null;
                        if (this.Controls.Count > 0)
                        {
                            args = GetPaintInfo();
                            this.dcInternal.DockingManager.FireProvideGraphicsItemsEvent(args.ProvideGraphicsItemsArgs);
                            if (dcInternal.DockingManager.ctrlLastPainted != this)
                                renderer.ResetButtonsHitTest();
                            if (this.WindowState == FormWindowState.Maximized)
                            {
                                renderer.PaintDockedControl(g, new Rectangle(new Point(0,3), Size), args);
                            }
                            else
                                renderer.PaintDockedControl(g, new Rectangle(Point.Empty, Size), args);
                        }
                    }
                }
                finally
                {
                    NativeMethods.ReleaseDC(Handle, hdc);
                }				
				DockingManager dockMan = dcInternal.DockingManager;

				dockMan.ctrlLastPainted = this;
                if ((!NativeMethods.IsDwmNCRenderingEnabled(this.Handle) && Environment.OSVersion.Version.Major >= 6)
                    && (dockMan.VisualStyle != VisualStyle.VS2005 && dockMan.VisualStyle != VisualStyle.Default))
                { 
                    if(this.dcInternal.DockingManager.NeedFloatFormRepaint)
                        RepaintChildren(); 
                }
			}
		}

		private CaptionButtonState GetActiveButtonState()
		{
			return ( Control.MouseButtons | MouseButtons.Left ) == Control.MouseButtons	 ?
				CaptionButtonState.Pushed : CaptionButtonState.Active;
		}

        protected override void OnHandleDestroyed(EventArgs e)
        {
            m_internalTip.Dispose();
            base.OnHandleDestroyed(e);
        }

		protected override bool ProcessCmdKey(ref Message m, Keys keydata)
		{
			if((keydata == (Keys.Alt|Keys.F4)) && (this.ControlBox == false))
				return true;

			if (this.dcInternal.DockingManager.ForwardMenuShortcuts) 
			{

				Form hostform;
				if (this.dcInternal.DockingManager.HostForm != null)
				{
					hostform = this.dcInternal.DockingManager.HostForm;
				} 
				else 
				{
					hostform = this.dcInternal.DockingManager.HostControl.ParentForm;
				}

				Syncfusion.Windows.Forms.Tools.XPMenus.BarManager bm = null;


				if (hostform != null) 
				{
					if( hostform.Menu != null )
					{
						if ((keydata & Keys.Modifiers) == Keys.Alt) 
						{
							if (keydata == (Keys.Alt | Keys.Menu))
							{
								hostform.Focus();
							}
						}
					}
					else
					{
						bm = Syncfusion.Windows.Forms.Tools.XPMenus.BarManager.GetManagerFromForm(hostform);
						if(bm != null) 
						{
							bm = bm.MainFrameBarManager;
							Syncfusion.Windows.Forms.Tools.XPMenus.BarItem item = bm.GetBarItemFromShortcut(keydata);
							if (item != null && item.Enabled)
							{
								item.PerformClick();
								return true;
							}


							if ((keydata & Keys.Modifiers) == Keys.Alt) 
							{
								if (keydata == (Keys.Alt | Keys.Menu))
								{
									bm.ProcessCmdKey(ref m, keydata);
								}
							}
						}
					}
				}
			}

			return base.ProcessCmdKey(ref m, keydata);

		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad(e);
			this.MinimumSize = new Size(30,30);
		}

		protected override void OnControlAdded(ControlEventArgs e)
		{
			base.OnControlAdded(e);
			this.UpdateControlBoxVisibility();
            this.UpdateFormBorderStyle();
			DockHost child = e.Control as DockHost;

			if( child != null && child.Controls.Count > 0 )
			{
				Control childCtrl = child.Controls[0];
				if( childCtrl != null )
					childCtrl.GotFocus += new EventHandler( child_GotFocus );
			}

			if( InternalController.DockingManager.ApplyMinMaxExtents && this.AutoHideMode != AutoHideStatus.Collapsed)
				this.UpdateFormMinimumSize();

			RightToLeft rtlEffective = this.dcInternal.DockingManager.IsMirrored ?
				RightToLeft.Yes : RightToLeft.No;
			e.Control.RightToLeft = rtlEffective;
		}

		private void child_GotFocus( object sender, EventArgs e )
		{
			Control activeCtrl = sender as Control;

			if( activeCtrl != null )
				this.LastActiveControl = activeCtrl;
		}

		protected override void OnControlRemoved(ControlEventArgs e)
		{
			DockHost child = e.Control as DockHost;

			if( child != null && child.Controls.Count > 0 )
			{
				Control childCtrl = child.Controls[0];
				if( childCtrl != null )
					childCtrl.GotFocus -= new EventHandler( child_GotFocus );

				if (m_lastActiveControl == childCtrl)
					m_lastActiveControl = null;
			}

			base.OnControlRemoved(e);
			this.UpdateControlBoxVisibility();
            if(this.AutoHideMode != AutoHideStatus.Collapsed)
			    this.UpdateFormMinimumSize();
		}

		public void UpdateControlBoxVisibility()
		{
			bool bcontrolbox = false;
            bool bautohide = false;
			CaptionButtonsCollection captionButtons;
			foreach(Control child in this.Controls)
			{
				if(child.Controls.Count > 0 && child is DockHost)
				{
					DockHostController dhc = (child as DockHost).InternalController as DockHostController;
					captionButtons = dhc.DockingManager.GetCustomCaptionButtons(child.Controls[0]);
					if (captionButtons != null)
					{
						if (captionButtons.ContainsButtonType(CaptionButtonType.Close) && dhc.CloseButtonVisibility)
						{
							bcontrolbox = true;
						}
                        if (captionButtons.ContainsButtonType(CaptionButtonType.Pin) && this.AutoHideButtonVisibility)
                        {
                            bautohide = true;
                        }
					}
					else if (dhc.DockingManager.TargetManagers.Count > 0)
					{
						foreach (DockingManager dm in dhc.DockingManager.TargetManagers)
						{
							captionButtons = dm.GetCustomCaptionButtons(child.Controls[0]);
							if (captionButtons != null && captionButtons.ContainsButtonType(CaptionButtonType.Close) && dhc.CloseButtonVisibility)
							{
								bcontrolbox = true;
								break;
							}
                            if (captionButtons.ContainsButtonType(CaptionButtonType.Pin) && this.AutoHideButtonVisibility)
                            {
                                bautohide = true;
                                break;
                            }
						}
					}
				}
			}

			if( dcInternal.DockingManager.VisualStyle == VisualStyle.Default
				|| dcInternal.DockingManager.VisualStyle == VisualStyle.VS2005 )
				ControlBox = bcontrolbox;
			else
			{
				bool needUpdate = this.ControlBox != false;
				this.ControlBox = false;

				if( needUpdate )
					this.UpdateBounds();
			}

			CloseButtonVisibility = bcontrolbox;
            AutoHideButtonVisibility = bautohide;
		}

		protected internal void UpdateFormMinimumSize()
		{
			if((this.dcInternal != null) && (this.dcInternal.dcChild != null))
			{
				DockHostController dhc = this.dcInternal.dcChild as DockHostController;
				if( dhc != null && dhc.Closing )
					return;

				Size minsize = this.dcInternal.MinimumSize;
				Size setminsize = Size.Empty;
				if(minsize.Width != 0)
					setminsize.Width = minsize.Width;
				if(minsize.Height != 0)
					setminsize.Height = minsize.Height;

				// Pad the rcscreen value for the floating form's caption height and border widths
				Size bordersize = SystemInformation.FrameBorderSize;
				setminsize.Height += SystemInformation.ToolWindowCaptionHeight + (bordersize.Height*2) + 2;
				setminsize.Width += (bordersize.Width*2) + 2; // The DockHost borders add on 2 pixels

				this.MinimumSize = setminsize;
			}
		}

		protected void UpdateFormPosition( int nScreenWidth, int nScreenHeight )
		{
			int nOffsetX = 0;
			int nOffsetY = 0;
			if( Location.X + Size.Width > nScreenWidth )
			{
				nOffsetX = Location.X + Size.Width - nScreenWidth;
				if( nOffsetX > Location.X )
				{
					nOffsetX = Location.X;
				}
			}
			if( Location.Y + Size.Height > nScreenHeight )
			{
				nOffsetY = Location.Y + Size.Height - nScreenHeight;
				if( nOffsetY > Location.Y )
				{
					nOffsetY = Location.Y;
				}
			}
			if( nOffsetX > 0 || nOffsetY > 0 )
			{
				this.Location = new Point( Location.X - nOffsetX, Location.Y - nOffsetY );
			}
		}

		// Implementation of IDesignerMouseHook
		public void HandleMouseDown(MouseButtons button, Point ptscreen)
		{
			PaintDockControlArgs args = GetPaintInfo();
			renderer.RefreshPaintInfo( new Rectangle( Point.Empty, Size ), args );
			DockControllerBase dcchild = this.dcInternal.GetChildAt(0);
			if(dcchild is DockHostController)
			{
				DockHostController dhc = dcchild as DockHostController;
				(dhc.HostControl as DockHost).HandleMouseDown(button, ptscreen);
			}
			else if(dcchild is DockTabController)
			{
				DockTabController dtc = dcchild as DockTabController;
				(dtc.HostControl as DockHost).HandleMouseDown(button, ptscreen);
			}
            else if (dcchild is SizingController)
            {
				this.dcInternal.HandleMouseDownImp( button, this.PointToClient( ptscreen ) );
            }
            else
            {
                this.dcInternal.HandleMouseDownImp(button, this.PointToClient(ptscreen));
            }
		}

		public void HandleMouseMove(MouseButtons button, Point ptscreen)
		{
			if((this.nDsgnrNCHit >= 10) && (this.nDsgnrNCHit <= 17))
			{
				// If a design time resize has been in progress, then set the new window size
				Debug.Assert(this.dcInternal.DockingManager.DesignMode == true);
				Point ptcurrent = Cursor.Position;
				Rectangle rcbounds = this.Bounds;
				int ndeltaX = ptcurrent.X - this.ptInitNCHit.X;
				int ndeltaY = ptcurrent.Y - this.ptInitNCHit.Y;
				this.ptInitNCHit = ptcurrent;
				switch(this.nDsgnrNCHit)
				{
					case 10:	// HTLEFT
						rcbounds.X += ndeltaX;
						rcbounds.Width = (ndeltaX < 0) ? rcbounds.Width+Math.Abs(ndeltaX) : rcbounds.Width-Math.Abs(ndeltaX);
						break;
					case 11:	// HTRIGHT
						rcbounds.Width += ndeltaX;
						break;
					case 12:	// HTTOP
						rcbounds.Y += ndeltaY;
						rcbounds.Height = (ndeltaY < 0) ? rcbounds.Height+Math.Abs(ndeltaY) : rcbounds.Height-Math.Abs(ndeltaY);
						break;
					case 13:	// HTTOPLEFT
						rcbounds.X += ndeltaX;
						rcbounds.Width = (ndeltaX < 0) ? rcbounds.Width+Math.Abs(ndeltaX) : rcbounds.Width-Math.Abs(ndeltaX);
						rcbounds.Y += ndeltaY;
						rcbounds.Height = (ndeltaY < 0) ? rcbounds.Height+Math.Abs(ndeltaY) : rcbounds.Height-Math.Abs(ndeltaY);
						break;
					case 14:	// HTTOPRIGHT
						rcbounds.Width += ndeltaX;
						rcbounds.Y += ndeltaY;
						rcbounds.Height = (ndeltaY < 0) ? rcbounds.Height+Math.Abs(ndeltaY) : rcbounds.Height-Math.Abs(ndeltaY);
						break;
					case 15:	// HTBOTTOM
						rcbounds.Height += ndeltaY;
						break;
					case 16:	// HTBOTTOMLEFT
						rcbounds.X += ndeltaX;
						rcbounds.Width = (ndeltaX < 0) ? rcbounds.Width+Math.Abs(ndeltaX) : rcbounds.Width-Math.Abs(ndeltaX);
						rcbounds.Height += ndeltaY;
						break;
					case 17:	// HTBOTTOMRIGHT
						rcbounds.Width += ndeltaX;
						rcbounds.Height += ndeltaY;
						break;
				}
				this.Bounds = rcbounds;
			}
			else
			{
				DockControllerBase dcchild = this.dcInternal.GetChildAt(0);
				if(dcchild is DockHostController)
				{
					DockHostController dhc = dcchild as DockHostController;
					(dhc.HostControl as DockHost).HandleMouseMove(button, ptscreen);
				}
				else if(dcchild is DockTabController)
				{
					DockTabController dtc = dcchild as DockTabController;
					(dtc.HostControl as DockHost).HandleMouseMove(button, ptscreen);
				}
                else if( dcchild is SizingController )
                {
					this.dcInternal.HandleMouseMoveImp(button, this.PointToClient(ptscreen));
                }
				else
				{
					this.dcInternal.HandleMouseMoveImp(button, this.PointToClient(ptscreen));
				}
			}
		}

		public void HandleMouseUp(MouseButtons button, Point ptscreen)
		{
			if( dcInternal.DockingManager.VisualStyle != VisualStyle.Default
				&& dcInternal.DockingManager.VisualStyle != VisualStyle.VS2005 )
			{
				Control draggingControl = this.dcInternal.DockingManager.DragProvider.DraggingControl as Control;

				if( this.hitArea == HitTestArea.Button )
				{
					if( !this.Controls.Contains( draggingControl ) && draggingControl != this )
					{
						PaintNCArea();                       
                        if (hitButton != null)
                        {
                            if (hitButton.Type == CaptionButtonType.Close)
                            {
                                this.dcInternal.bClosingByMouse = true;
                                dcInternal.CloseController();
                                this.dcInternal.bClosingByMouse = false;
                                if (this.Owner != null)
                                    this.Owner.Focus();
                            }
                            if (hitButton.Type == CaptionButtonType.Maximize)
                            {
                                MaximumToggleAnimation();
                            }
                            if (hitButton.Type == CaptionButtonType.Restore)
                            {
                                MaximumToggleAnimation();
                            }
                            if (hitButton.Type == CaptionButtonType.Pin)
                            {
                                if(this.AutoHideMode == AutoHideStatus.Expanded)
                                    FloatingWindowsSize = this.Size;
                                this.dcInternal.HostControl.MinimumSize = new Size(0, 0 );
                                AutoHideToggleAnimation();
                            }
                            if (hitButton.Type == CaptionButtonType.Menu)
                            {
                                Point pt = this.PointToClient(Cursor.Position);
                                this.dcInternal.DockingManager.ShowMenu(this, this.dcInternal.HostControl.Controls[0].PointToScreen(pt));
                            }
                            else
                            {
                                hitButton.FireClickEvent(new CancelEventArgs(false));
                            }
                        }
						return;
					}
				}
			}

			if((this.nDsgnrNCHit >= 10) && (this.nDsgnrNCHit <= 17))
			{
				// A design-time resize has been progress. Reset state and update designer.
				this.nDsgnrNCHit = 0;
				this.ptInitNCHit = Point.Empty;
				this.dcInternal.DockingManager.UpdateDesigner();
			}
			else
				this.dcInternal.HandleMouseUpImp( button, this.PointToClient( ptscreen ) );
		}

		public void HandleDoubleClick(Point ptscreen)
		{
			if( this.Visible )
				this.dcInternal.HandleDoubleClickImp( this.PointToClient(ptscreen));
		}

        internal void InvokeToggleDockState(DockBehavior dock)
        {
            if (this.Visible && this.dcInternal != null)
                this.dcInternal.ToggleFloatToDock(dock);
        }

		public void HandleMouseLeave()
		{
			// No implementation
		}

		public void InitiateFloatingResize(Point ptscreen, int nchittest)
		{
			this.nDsgnrNCHit = nchittest;
			this.ptInitNCHit = ptscreen;
		}

		public bool GetDesignMode()
		{
			return this.dcInternal.DockingManager.DesignMode;
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if((this.dcInternal != null) && (this.dcInternal.DockingManager.DesignProcess == false)
				&& this.Visible )	// OnMouseUp gets sent after a doubleclick dispose
			{
				IDraggable idg = this.dcInternal.DockingManager.DragProvider.DraggingControl;
				this.dcInternal.HandleMouseUpImp(e.Button, new Point(e.X, e.Y));

				if((this.nDsgnrNCHit >= 10) && (this.nDsgnrNCHit <= 17))
				{
					this.nDsgnrNCHit = 0;
					this.ptInitNCHit = Point.Empty;
				}
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			Point ptscreen = new Point(e.X, e.Y);
			ptscreen = this.PointToScreen( ptscreen );
			if( e.Button == MouseButtons.Left &&
				!SystemInformation.DragFullWindows && this.dcInternal.DockingManager.DragProvider is WhidbeyDragProvider )
				this.Location = new Point( ptscreen.X - m_dragPoint.X, ptscreen.Y - m_dragPoint.Y );

			if((this.dcInternal != null) && (this.dcInternal.DockingManager.DesignProcess == false))
			{
				IDraggable idg = this.dcInternal.DockingManager.DragProvider.DraggingControl;
				if(idg == null)
					this.dcInternal.HandleMouseMoveImp(e.Button, new Point(e.X, e.Y));
				else if( idg == this || this.Controls.Contains( idg as Control ) )
					this.dcInternal.HandleMouseMoveImp(MouseButtons.Left, new Point(e.X, e.Y));
			}
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
            if( this.Visible )
                UpdateFormBorderStyle();
            if ((this.Visible == true) && (this.dcInternal.DockingManager.bLoadVisibility == false))
            {
                this.Visible = false;
                this.dcInternal = null;
            }
			base.OnVisibleChanged(e);
		}

		protected override void OnLocationChanged( EventArgs e )
		{
			Point cursorLocation = this.PointToClient( Cursor.Position );

			if( ( this.dcInternal != null ) && ( this.dcInternal.DockingManager.DesignProcess == false )
				&& this.Visible )
			{
				IDraggable idg = this.dcInternal.DockingManager.DragProvider.DraggingControl;

				if( idg == null || this.Controls.Contains( idg as Control ) || idg == this )
					this.dcInternal.HandleMouseMoveImp( MouseButtons.Left, cursorLocation );
			}

			base.OnLocationChanged( e );
		}

		protected override void Dispose(bool bdisposing)
		{
			this.LastActiveControl = null;
            SubscribeToResizeEvent(false);
			nativeWindow.ReleaseHandle();
         
			base.Dispose(bdisposing);
		}

		public override void Refresh()
		{}

		internal void RestoreStateOnRightToLeftUpdate()
		{
			/*
			 *  This workaround ensures that owned form will be visible after RTL changed in owner form
			 */
			Control ctrlHost = this;
			if (ctrlHost.Visible)
			{
				// Toggle visibility and toggle bounds
				ctrlHost.Visible = false;
				Rectangle rectBounds = ctrlHost.Bounds;
				// Respawn form outside of screen
				ctrlHost.Bounds = new Rectangle( new Point( 0, SystemInformation.VirtualScreen.Height ), rectBounds.Size );
				ctrlHost.Visible = true;
				ctrlHost.Bounds = rectBounds;
			}
		}

		internal void UpdateChildsRightToLeft( RightToLeft rtlEffective )
		{
			foreach (Control ctrlChildCtrl in this.Controls)
			{
				ctrlChildCtrl.RightToLeft = rtlEffective;
			}
		}

		public DockControllerBase GetController()
		{
			return InternalController;
		}

		private void FloatingForm_Resize(object sender, EventArgs e)
		{
			if( dcInternal.DockingManager.VisualStyle != VisualStyle.Default
				&& dcInternal.DockingManager.VisualStyle != VisualStyle.VS2005 && this.Controls.Count > 0)
			{
				renderer.ControlBounds = new Rectangle( Point.Empty, Size);
				PaintNCArea();
			}
		}

		private void SubscribeToResizeEvent( bool subscribe )
		{
			if( subscribe )
			{
				Resize += new EventHandler(FloatingForm_Resize);
			}
			else
			{
				Resize -= new EventHandler(FloatingForm_Resize);
			}
		}

        /// <summary>
        /// To update maximize and restore caption button based on windows state.
        /// </summary>
        private void MaximumToggleAnimation()
        {
            if (this.WindowState == FormWindowState.Normal)
                this.WindowState = FormWindowState.Maximized;
            else if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                if (this.AutoHideMode == AutoHideStatus.Collapsed)
                    this.dcInternal.HostControl.Height = this.CaptionHeight;
            }
        }

		private HitTestArea hitArea = HitTestArea.None;

        private CaptionButton hitButton = null;
	}
}
