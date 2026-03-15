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
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;

using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools
{
	internal class MainWindowSubclass : NativeWindowSubclass
	{
		#region Class overrides
		protected override void WndProc(ref Message m)
		{
			if( m.Msg == NativeMethods.WM_CLOSE )
			{
				RaiseCloseEvent( m );
			}

			base.WndProc(ref m);

			if( m.Msg == NativeMethods.WM_MDIACTIVATE )
			{
				RaiseMdiActivateEvent( m );
			}
			else if( m.Msg == NativeMethods.WM_ACTIVATE )
			{
				if( m.WParam == new IntPtr( 0 ) )
				{
					RaiseDeactivateMessage( m );
				}
			}
		}
		#endregion

		#region Class events implementation
		public delegate void MessageEventHandler(object sender, Message m);

		public event MessageEventHandler OnMdiActivate;
		public event MessageEventHandler OnDeactivate;
        public event MessageEventHandler OnClose;

		private void RaiseMdiActivateEvent(Message m)
		{
			if (OnMdiActivate != null)
				OnMdiActivate(this, m);
		}
		private void RaiseDeactivateMessage(Message m)
		{
			if (OnDeactivate != null)
				OnDeactivate(this, m);
		}
        private void RaiseCloseEvent(Message m)
        {
            if (OnClose != null)
                OnClose(this, m);
        }

		#endregion
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class MainFormController : DockControllerBase
	{
		protected ContainerControl ctrlHost = null;
		protected Rectangle rcLayout = Rectangle.Empty;
		protected ArrayList alChildren = new ArrayList();
		protected ArrayList alVisibleFloatingForms = new ArrayList();
		protected DockControllerBase dcPriority = null;
		protected internal Rectangle rcClientPool = Rectangle.Empty;
		protected Control ctrlFormClient = null;
		protected Form frmTopLevelSubscribed = null;

		protected internal AHTabControl ahTabCtrlL = null;
		protected internal AHTabControl ahTabCtrlT = null;
		protected internal AHTabControl ahTabCtrlR = null;
		protected internal AHTabControl ahTabCtrlB = null;
		protected bool m_bInLayout = false;

		public bool bOverlapSizing = true;

		private bool m_EventSubscribed = false;

		protected FocusHolder fhCtrl = null;

		public override Control HostControl
		{
			get { return this.ctrlHost; }
		}

		public DockControllerBase PriorityController
		{
			get { return this.dcPriority; }
			set { this.dcPriority = value; }
		}

		public override Rectangle LayoutRect
		{
			get { return this.rcLayout; }
			set
			{
				if(this.rcLayout != value)
				{
					this.rcLayout = value;
					AdjustLayout();
				}
			}
		}

		public override bool Floating
		{
			get { return false; }
			set {}
		}

		public override IEnumerator ChildEnumerator
		{
			get
			{
				return alChildren.GetEnumerator();
			}
		}

		public override int ChildCount
		{
			get	{ return this.alChildren.Count; }
		}

		public override int ChildHostCount
		{
			get
			{
				int i = 0;
				foreach(DockControllerBase dc in this.alChildren)
				{
					if(dc is DockHostController)
						i++;
				}
				return i;
			}
		}

		public override IEnumerator DCR
		{
			get { return new IEnumWrapper(null); }
		}

		public FocusHolder FocusHolderControl
		{
			get { return this.fhCtrl; }
		}

		public MainFormController(DockingManager mgr, ContainerControl host) : base(mgr)
		{
			this.ctrlHost = host;
			InitializeComponent();
			AdjustLayoutDockArea();
		}

		protected void InitializeComponent()
		{
			// Subscribe to the form's events
			this.ctrlHost.Paint += new System.Windows.Forms.PaintEventHandler(this.HostControl_Paint);
			this.ctrlHost.VisibleChanged += new System.EventHandler(this.HostControl_VisibleChanged);
			this.ctrlHost.ParentChanged += new EventHandler(this.HostControl_ParentChanged);
			this.ctrlHost.Layout += new LayoutEventHandler( HostControl_Layout );

			// Initialize the layout rectangles
			if( this.ctrlHost.AutoScroll )
			{
				this.rcLayout = this.ctrlHost.ClientRectangle;
				this.rcClientPool = this.ctrlHost.ClientRectangle;
			}
			else
			{
				this.rcLayout = this.ctrlHost.DisplayRectangle;
				this.rcClientPool = this.ctrlHost.DisplayRectangle;
			}

			// Create the autohide tabs
			this.ahTabCtrlL = new AHTabControl(this.dockingMgr, Syncfusion.Windows.Forms.Tools.DockingStyle.Left);
			this.ahTabCtrlL.Alignment = System.Windows.Forms.TabAlignment.Right;
			this.ahTabCtrlL.RotateText180WhenLeftAligned = true;
			this.ahTabCtrlL.VerticalAlignment = TabVerticalAlignment.Top;

			this.ahTabCtrlT = new AHTabControl(this.dockingMgr, Syncfusion.Windows.Forms.Tools.DockingStyle.Top);
			this.ahTabCtrlT.Alignment = System.Windows.Forms.TabAlignment.Bottom;
			this.ahTabCtrlT.RotateText180WhenLeftAligned = true;

			this.ahTabCtrlR = new AHTabControl(this.dockingMgr, Syncfusion.Windows.Forms.Tools.DockingStyle.Right);
			this.ahTabCtrlR.Alignment = System.Windows.Forms.TabAlignment.Left;
			this.ahTabCtrlR.RotateText180WhenLeftAligned = true;
			this.ahTabCtrlR.VerticalAlignment = TabVerticalAlignment.Top;

			this.ahTabCtrlB = new AHTabControl(this.dockingMgr, Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom);
			this.ahTabCtrlB.Alignment = System.Windows.Forms.TabAlignment.Top;
			this.ahTabCtrlB.RotateText180WhenLeftAligned = true;

			this.fhCtrl = new FocusHolder();

			if(this.dockingMgr.DesignProcess == false)
			{
				this.ctrlHost.Controls.AddRange( new Control[] { this.ahTabCtrlL, this.ahTabCtrlT,
																   this.ahTabCtrlR, this.ahTabCtrlB } );
				this.ctrlHost.Controls.Add(this.fhCtrl);
			}

			this.ctrlHost.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.HostControl_ChildControlAdded);
			this.ctrlHost.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.HostControl_ChildControlRemoved);
			foreach(Control ctrl in this.ctrlHost.Controls)
			{
				if( ((ctrl is DockHost)==false) && ((ctrl is DragSplitter)==false)
					&& ((ctrl is AHTabControl)==false) && ((ctrl is FocusHolder)==false) )
				{
					ctrl.DockChanged += new EventHandler(this.ChildControl_DockChanged);
					ctrl.SizeChanged += new EventHandler(this.ChildControl_SizeChanged);
					ctrl.VisibleChanged += new EventHandler(this.ChildControl_VisibleChanged);
				}
			}

			if(this.ctrlHost is Form)
				(this.ctrlHost as Form).MdiChildActivate += new EventHandler(this.MainForm_MdiChildActivate);
		}

		// Invoked by the docking manager when the form first loads
		public void UpdateFormClientSetting()
		{
			if(this.ctrlFormClient == null)
			{
				// If the hostform is an mdicontainer or a regular form containing a DockingClientPanel
				// instance, then get a reference to the mdiclient or the DockingClientPanel instance.
				foreach(Control ctrl in this.ctrlHost.Controls)
				{
					if(ctrl is MdiClient)
					{
						this.ctrlFormClient = ctrl;
						this.ctrlFormClient.Anchor = AnchorStyles.Top|AnchorStyles.Left;
						break;
					}
					else if(ctrl is DockingClientPanel)
					{
						this.ctrlFormClient = ctrl;
                        if(this.DockingManager.DesignProcess)
                            this.ctrlFormClient.SuspendLayout();
						this.ctrlFormClient.Anchor = AnchorStyles.Top|AnchorStyles.Left;
						if (this.DockingManager.DesignProcess)
						this.ctrlFormClient.ResumeLayout(false);
						break;
					}
				}
			}
		}

		internal override void RemoveDockController()
		{
		}

		protected void HostControl_Layout( object sender, LayoutEventArgs e )
		{
			if( m_bInLayout )
				return;

			m_bInLayout = true;
			AdjustLayoutDockArea();
			m_bInLayout = false;
		}

		private void HostControl_Resize( Object obj, EventArgs e )
		{
			if( dockingMgr.ahTabAnimate != null && dockingMgr.ahTabAnimate.AnimationState != AutoHideAnimationState.None )
				AdjustLayoutDockArea();
			if( dockingMgr.DesignMode == true )
				dockingMgr.SaveToStream();
		}

		protected void HostControl_Paint(Object obj, PaintEventArgs e)
        {
            if( this.dockingMgr.Office2007Theme == Office2007Theme.Managed
                   && (this.dockingMgr.Renderer.VisualStyle == VisualStyle.Office2007 || this.dockingMgr.Renderer.VisualStyle == VisualStyle.Office2007Outlook) )
                this.dockingMgr.Renderer.RefreshOffice2007Theme( this.dockingMgr.Office2007Theme );

            if ((this.ctrlFormClient == null) && (e.ClipRectangle.IntersectsWith(this.rcClientPool)))
                DrawDockEdgeBorders(e.Graphics);

			if( !m_EventSubscribed )
			{
				ctrlHost.Resize += new System.EventHandler(this.HostControl_Resize);
				m_EventSubscribed = true;
			}
		}

		protected void HostControl_VisibleChanged( object sender, EventArgs e )
		{
			if( this.HostControl != null )
			{
				bool chainVisible = true;
				Control parent = this.HostControl;
				while( parent != null )
				{
					if( !parent.Visible )
					{
						chainVisible = false;
						break;
					}

					parent = parent.Parent;
				}

				if( chainVisible )
					ShowAllFloatingForms();
				else
					HideAllFloatingForms();
			}
		}

        protected void HostControl_ParentChanged(object sender, EventArgs e)
        {
            if( ctrlHost != null )
            {
                Form toplevelform = this.ctrlHost.TopLevelControl as Form;
                if( toplevelform != null )
                {
                    if( toplevelform.IsMdiContainer )
                    {
                        this.frmTopLevelSubscribed = this.ctrlHost.TopLevelControl as Form;
                        frmTopLevelSubscribed.Resize += new EventHandler( TopLevelControl_Resize );
                    }
                }
                else
                {
                    if( frmTopLevelSubscribed != null )
                    {
                        frmTopLevelSubscribed.Resize -= new EventHandler( TopLevelControl_Resize );
                    }
                }
            }
        }

		protected void TopLevelControl_Resize( object sender, EventArgs e )
		{
			Form frm = sender as Form;
			if( frm != null && frm.WindowState == FormWindowState.Minimized )
			{
				HideAllFloatingForms(true);
			}
			else
			{
				Form hostFrm = this.ctrlHost as Form;
                // Fix for 13244 
                if (hostFrm == null) 
                {
                    if (this.ctrlHost is UserControl) // In case of Nested Docking or control docked into usercontrol.
                    {
                        Control ctrl = this.ctrlHost.Parent;
                        if (ctrl != null)
                        {
                            while ((!(ctrl is Form)) && ctrl.Parent != null)
                                ctrl = ctrl.Parent;

                            hostFrm = ctrl as Form;
                        }
                    }
                }
                // ---> upto this is for fix 13244
                if (hostFrm != null && hostFrm.WindowState != FormWindowState.Minimized)
                    ShowAllFloatingForms(true);
			}
		}

		protected void HostControl_ChildControlAdded(Object obj, ControlEventArgs e)
		{
			// Converting the form to MDIContainer at designtime messes up the DockHost state.
			// Reloading the document provides an easy workaround.
			if(e.Control is MdiClient)
			{
				if(this.dockingMgr.DesignMode == true)
				{
					if(this.ctrlFormClient != null)
						throw new ApplicationException("Error - An MdiContainer form should not contain a DockingClientPanel control.");
					if(this.dockingMgr.DockToFill == true)
					{
						this.dockingMgr.DockToFill = false;
						MessageBox.Show("The DockingManager.DockToFill property has been reset as this is an invalid option when the host form is an MDIContainer.",
							"Essential Tools DockingManager", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					IDesignerLoaderService idls = this.dockingMgr.GetServiceFromMgr(typeof(IDesignerLoaderService)) as IDesignerLoaderService;
					if(idls != null)
						idls.Reload();
				}
				else
				{
					this.UpdateFormClientSetting();
					this.AdjustLayoutDockArea();
				}
			}
			else if(e.Control is DockingClientPanel)
			{
				if(this.dockingMgr.DesignMode == true)
				{
					if(this.ctrlFormClient != null)
						throw new ApplicationException("Error - The form should contain only one DockingClientPanel instance.");
					if(this.dockingMgr.DockToFill == true)
					{
						this.dockingMgr.DockToFill = false;
						MessageBox.Show("The DockingManager.DockToFill property has been reset as this is an invalid option when using the DockingClientPanel control.",
							"Essential Tools Docking Windows", MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					this.UpdateFormClientSetting();
				}
				else
				{
					this.UpdateFormClientSetting();
					this.AdjustLayoutDockArea();
				}
			}

			if( (e.Control != null) && ((e.Control is DockHost)==false) && ((e.Control is DragSplitter)==false)
				&& ((e.Control is AHTabControl)==false) && ((e.Control is FocusHolder)==false) )
			{
				if(e.Control.Dock != DockStyle.None)
					this.AdjustLayoutDockArea();
				e.Control.DockChanged += new EventHandler(this.ChildControl_DockChanged);
				e.Control.SizeChanged += new EventHandler(this.ChildControl_SizeChanged);
				e.Control.VisibleChanged += new EventHandler(this.ChildControl_VisibleChanged);
			}
		}

		protected void HostControl_ChildControlRemoved(Object obj, ControlEventArgs e)
		{
			if((e.Control is MdiClient) || (e.Control is DockingClientPanel))
				this.ctrlFormClient = null;

			if( (e.Control != null) && ((e.Control is DockHost)==false) && ((e.Control is DragSplitter) == false)
				&& ((e.Control is AHTabControl)==false) && ((e.Control is FocusHolder)==false) )
			{
				e.Control.DockChanged -= new EventHandler(this.ChildControl_DockChanged);
				e.Control.SizeChanged -= new EventHandler(this.ChildControl_SizeChanged);
				e.Control.VisibleChanged -= new EventHandler(this.ChildControl_VisibleChanged);
			}
		}

		protected void MainForm_MdiChildActivate(object sender, EventArgs e)
		{
			// Workaround for .NET 1.1 MDI focus bug - Unparenting/hiding a control with focus from the
			// mainform(as happens when a docked control is floated or an autohidden window is restored),
			// and subsequently setting focus to the mainform by clicking anywhere on it incorrectly activates
			// the first created mdichild form rather than the topmost(the most recently active) mdichild.
			// Setting the recently activated childform to be TabIndex 0 helps avoid the problem.
			Form hostform = this.ctrlHost as Form;
			if((hostform.IsMdiContainer == true) && (hostform.ActiveMdiChild != null))
			{
				hostform.ActiveMdiChild.TabIndex = 0;
				hostform.ActiveMdiChild.Focus();
			}
		}

		protected void ChildControl_DockChanged(object sender, EventArgs e)
		{
			this.AdjustLayoutDockArea();
		}

		protected void ChildControl_SizeChanged(object sender, EventArgs e)
		{
			Control ctrl = sender as Control;
			if((ctrl != null) && (ctrl.Dock != DockStyle.None))
				this.AdjustLayoutDockArea();
		}

		protected void ChildControl_VisibleChanged(object sender, EventArgs e)
		{
			Control ctrl = sender as Control;
			if((ctrl != null) )
			{
				if( ( ctrl.Dock != DockStyle.None ) )
				{
					if( ctrl.Disposing && this.dockingMgr.DesignMode )
						ctrl.Disposed += new EventHandler( ctrl_Disposed );
					else
						this.AdjustLayoutDockArea();
				}
			}
		}

		protected void ctrl_Disposed( object sender, EventArgs e )
		{
			this.AdjustLayoutDockArea();
			Control ctrl = sender as Control;
			if( ctrl != null )
				ctrl.Disposed -= new EventHandler( ctrl_Disposed );
		}

		public override bool IsTargetController(Point ptscreen)
		{
			if( (this.dockingMgr.DesignMode == true) || (this.ctrlHost.Parent != null) )
				return this.ctrlHost.Parent.RectangleToScreen(this.ctrlHost.Bounds).Contains(ptscreen) ? true : false;
			else	// The form must be a top-level floating form.
				return this.ctrlHost.Bounds.Contains(ptscreen) ? true : false;
		}

		public DockControllerBase GetBorderController(Syncfusion.Windows.Forms.Tools.DockingStyle border)
		{
			foreach(DockControllerBase dc in this.alChildren)
			{
				if(dc.DICurrent.dStyle == border)
					return dc;
			}
			return null;
		}

		public AHTabControl GetAHTabControl(Syncfusion.Windows.Forms.Tools.DockingStyle border)
		{
			switch(border)
			{
				case Syncfusion.Windows.Forms.Tools.DockingStyle.Left:
					return this.ahTabCtrlL;
				case Syncfusion.Windows.Forms.Tools.DockingStyle.Top:
					return this.ahTabCtrlT;
				case Syncfusion.Windows.Forms.Tools.DockingStyle.Right:
					return this.ahTabCtrlR;
				case Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom:
					return this.ahTabCtrlB;
				default:
					return null;
			}
		}

		public override ArrayList ChildControllers
		{
			get
			{
				return this.alChildren;
			}
		}

		protected internal override DockControllerBase QueryController( string uniqueName )
		{
			DockControllerBase dcb = null;

			foreach( DockControllerBase baseCtrl in DockingManager.DockAreaControllers )
			{
				if( baseCtrl is DockHostController )
				{
					dcb = baseCtrl.QueryController(uniqueName);

					if( dcb != null )
						break;
				}
			}

			return dcb;
		}

		public DockInfo GetDockInfoForController(DockControllerBase dctarget)
		{
			DockInfo dinfo = DockInfo.NullInfo;
			foreach(DockControllerBase dcchild in this.alChildren)
			{
				if(RecGetControllerInfo(dcchild, dctarget) == true)
				{
					dinfo.dController = this;
					dinfo.dStyle = dcchild.DICurrent.dStyle;
					dinfo.nPriority = this.alChildren.IndexOf(dcchild);
					dinfo.nDockIndex = dcchild.DICurrent.nDockIndex;
					dinfo.DP = dcchild.DICurrent.DP;
					if(this.dockingMgr.DockToFill == false)
						dinfo.rcDockArea = dcchild.LayoutRect;
					else
						dinfo.rcDockArea = dctarget.LayoutRect;
					return dinfo;
				}
			}

			foreach(DockControllerBase dockControllerBase in DockingManager.DockAreaControllers )
			{
				if( dockControllerBase == dctarget)
				{
					dinfo.dController = this;
					dinfo.dStyle = dctarget.DICurrent.dStyle;
					dinfo.nPriority = 0;
					dinfo.nDockIndex = dctarget.DICurrent.nDockIndex;
					dinfo.DP = dctarget.DICurrent.DP;
					dinfo.rcDockArea = dctarget.LayoutRect;
					return dinfo;
				}
			}

			Debug.Assert(false, "Error: Invalid Controller.");
			return dinfo;
		}

		protected bool RecGetControllerInfo(DockControllerBase dc, DockControllerBase target)
		{
			if(dc == target)
				return true;

			if(dc.ChildCount <= 0)
			{
				if(dc.Equals(target) == true)
					return true;
				return false;
			}

			for(int i = 0; i < dc.ChildCount; i++)
			{
				if(RecGetControllerInfo(dc.GetChildAt(i), target) == true)
					return true;
			}
			return false;
		}

		public override void InvokeDocking(DockControllerBase dc)
		{
			DockStateControllerBase dhc = dc as DockStateControllerBase;
			Debug.Assert((dhc!=null), "Error: Invalid dock controller.\n");

			if((this.dockingMgr.DockToFill == true) && (this.ChildCount > 0))
			{
				// DockToFill mode where the MainFormController already has a child SizingController with another
				// child dockhost/sizing controller. In this case create a new SizingController for the given dockstyle,
				// add dc and the existing child as children of the new SizingController, and make the new
				// SizingController a child of the MainFormController's SizingController.
				this.dockingMgr.DockToNewSizingControllerFillMode(dhc);
			}
			else
			{
				this.dockingMgr.DockToFormController(dhc);
			}

			if( dc.DockingManager != this.dockingMgr )
				this.dockingMgr.ImportControl( dc );
		}

		public override bool AttemptDCRDocking(DockControllerBase ctrl, IEnumerator iedcr)
		{
			DockHostController dhc = ctrl as DockHostController;
			Debug.Assert((dhc != null), "Error: Invalid DockHostController.\n");
			
			// Run through the formcontroller's child list and see if there are any controller's with
			// a relationship with this dockhost
			foreach(DockControllerBase dc in this.alChildren)
			{
				if(dc.AttemptDCRDocking(dhc, iedcr) == true)
					return true;
			}

			if((this.dockingMgr.DockToFill == true) && (this.ChildCount > 0))
			{
				// DockToFill mode where the MainFormController already has a child SizingController with another
				// child dockhost/sizing controller. In this case create a new SizingController for the given dockstyle,
				// add dc and the existing child as children of the new SizingController, and make the new
				// SizingController a child of the MainFormController's SizingController.
				if((dhc.DINew.dStyle == DockingStyle.Left) || (dhc.DINew.dStyle == DockingStyle.Right))
				{
					if(dhc.DINew.rcDockArea.Width > (this.rcLayout.Width * 0.5))
						dhc.DINew.rcDockArea.Width = (int)(this.rcLayout.Width * 0.5);
				}
				else if((dhc.DINew.dStyle == DockingStyle.Top) || (dhc.DINew.dStyle == DockingStyle.Bottom))
				{
					if(dhc.DINew.rcDockArea.Height > (this.rcLayout.Height * 0.5))
						dhc.DINew.rcDockArea.Height = (int)(this.rcLayout.Height * 0.5);
				}

				this.dockingMgr.DockToNewSizingControllerFillMode(dhc);
			}
			else
				this.dockingMgr.DockToFormController(dhc);
			return true;
		}
		
		public override void InvokeDCRDocking(DockControllerBase dc, DCRelationship dcr)
		{
			DockHostController dhc = dc as DockHostController;
			Debug.Assert((dhc != null), "Error: Invalid Controller.\n");

			if((this.dockingMgr.DockToFill == true) && (this.ChildCount > 0))
			{
				// DockToFill mode where the MainFormController already has a child SizingController with another
				// child dockhost/sizing controller. In this case create a new SizingController for the given dockstyle,
				// add dc and the existing child as children of the new SizingController, and make the new
				// SizingController a child of the MainFormController's SizingController.
				this.dockingMgr.DockToNewSizingControllerFillMode(dhc);
			}
			else
				this.dockingMgr.DockToFormController(dhc);
		}

		public override void AddChild(DockControllerBase dc, Syncfusion.Windows.Forms.Tools.DockingStyle db)
		{
			this.InsertChild(dc, this.alChildren.Count, db);
		}

		public override void InsertChild(DockControllerBase dc, int index, Syncfusion.Windows.Forms.Tools.DockingStyle db)
		{
			if(index > this.alChildren.Count)
				index = this.alChildren.Count;
			if(this.alChildren.Contains(dc) == false)
				this.alChildren.Insert(index, dc);

			DockPreference dp = ((db==Syncfusion.Windows.Forms.Tools.DockingStyle.Left)||(db==Syncfusion.Windows.Forms.Tools.DockingStyle.Right)) ? DockPreference.Horizontal : DockPreference.Vertical;
			dc.DICurrent = new DockInfo(this, db, 0, 0, dp, Rectangle.Empty);
			dc.DICurrent.nPriority = this.alChildren.IndexOf(dc);
			dc.ParentController = this;

			UpdatePriorityIndices();
		}

		public override void RemoveChild(DockControllerBase dc)
		{
			try
			{
				this.alChildren.Remove(dc);
			}
			catch(ArgumentException e)
			{
				Trace.Write(e.Message);
				return;
			}
			dc.ParentController = null;
			UpdatePriorityIndices();
		}

		public override DockControllerBase GetChildAt(int index)
		{
			return this.alChildren[index] as DockControllerBase;
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

		public override void GetDockInfo(Control ctrl, Point pt, DockInfo di)
		{
			Point ptclient = HostControl.PointToClient(pt);

			// Determine whether the form outer boundaries are hit. Priority 0 area. Insert at head of the list
			Rectangle[] rcborders = new Rectangle[4];

			// If any of the AH tabs are present, then these also constitute the priority 0 zone.
			// Compute a new layoutrect, that takes into account the presence of the AHTabControl
			Rectangle[] rcahborders = new Rectangle[4];			// Temporary array for storing ahtabborders
			Rectangle rcahlayout = this.LayoutRect;
			int nAHTabCtrlWidth = this.dockingMgr.AutoHideTabHeight;
			if(this.ahTabCtrlL.TabCount > 0)
			{
				rcahborders[0] = this.ahTabCtrlL.Bounds;
				rcahlayout.X += nAHTabCtrlWidth;
				rcahlayout.Width -= nAHTabCtrlWidth;
			}
			if(this.ahTabCtrlT.TabCount > 0)
			{
				rcahborders[1] = this.ahTabCtrlT.Bounds;
				rcahlayout.Y += nAHTabCtrlWidth;
				rcahlayout.Height -= nAHTabCtrlWidth;
			}
			if(this.ahTabCtrlR.TabCount > 0)
			{
				rcahborders[2] = this.ahTabCtrlR.Bounds;
				rcahlayout.Width -= nAHTabCtrlWidth;
			}
			if(this.ahTabCtrlB.TabCount > 0)
			{
				rcahborders[3] = this.ahTabCtrlB.Bounds;
				rcahlayout.Height -= nAHTabCtrlWidth;
			}

			if((this.dockingMgr.DockToFill == true) && (this.alChildren.Count > 0))
			{
				// In the DockToFill mode, the main FormController usually contains only 1 child DockHostController,
				// and this DockHostController provides the feedback dockinfo.
				// An exception is made when the control being dragged is a tab that is a part of
				// the tabbedgroup on the form. This special case prevents a lock-out esp., when DisallowFloating is set.
				bool bresetreturn = true;
				DockControllerBase sc = this.alChildren[0] as DockControllerBase;
				if(sc.ChildCount == 1)
				{
					DockControllerBase dcb = sc.GetChildAt(0);
					if(dcb is DockTabController)
					{
						DockTabController dtc = dcb as DockTabController;
						// The control being dragged should be the tab's HostController, but should
						// not be present in the tab itself. This happens only when a tab is being dragged
						// off the tabgroup. Dragging the tabgroup itself will fail the second condition.
						if(dtc.HostControl == ctrl)
						{
							foreach(DockTabPage tabpage in dtc.TabControl.TabPages)
							{
								if(tabpage.dhcClient.HostControl == ctrl)
								{
									di.dController = null;
									return;
								}
							}
							bresetreturn = false;
						}
					}
				}
				if(bresetreturn)
				{
					di.dController = null;
					return;
				}
			}

			// For the 0 hitzone, Provide an outer rectangle with a 10pxl outer offset from the display rectangle
			ComputeLRTBBorders(Rectangle.Inflate(this.LayoutRect,10,10), 10, ref rcborders);
			HitTestBorderRect(rcahlayout, ref rcborders, ptclient, di);
			if(di.dController != null)
			{
				di.nPriority = 0;
				return;
			}

			// In the DockToFill mode use only the first hitrect pass.
			if((this.dockingMgr.DockToFill == true) && (this.alChildren.Count > 0))
			{
				di.dController = null;
				return;
			}

			HitTestBorderRect(rcahlayout, ref rcahborders, ptclient, di);
			if(di.dController != null)
			{
				di.nPriority = 0;
				return;
			}

			// If not, allow second pass to an inner rect that is composed out of the client pool rect.
			// If the clientpool rect's border is the same as the form's border, then offset the inner hit test rect
			// by the outer dockboundary. Priority -1 area. Will be added to the end of the list
			int left = (this.rcClientPool.Left == LayoutRect.Left) ? LayoutRect.Left+this.nDockBoundary : this.rcClientPool.Left;
			int top = (this.rcClientPool.Top == LayoutRect.Top) ? LayoutRect.Top+this.nDockBoundary : this.rcClientPool.Top;
			int right = (this.rcClientPool.Right == LayoutRect.Right) ? LayoutRect.Right-this.nDockBoundary : this.rcClientPool.Right;
			int bottom = (this.rcClientPool.Bottom == LayoutRect.Bottom) ? LayoutRect.Bottom-this.nDockBoundary : this.rcClientPool.Bottom;
			Rectangle rcinner = new Rectangle(left, top, right-left, bottom-top);
			ComputeLRTBBorders(this.rcClientPool, this.nDockBoundary, ref rcborders);
			HitTestBorderRect(this.rcClientPool, ref rcborders, ptclient, di);
			if(di.dController != null)
				di.nPriority = -1;
		}

		public override bool QueryDropProceedWithDock(Control ctrldrop, Syncfusion.Windows.Forms.Tools.DockingStyle style)
		{
			DockAllowEventArgs arg = new DockAllowEventArgs(ctrldrop.Controls[0], this.ctrlHost, style);
			this.dockingMgr.FireDockAllowEvent(arg);
			return !arg.Cancel;
		}
		public override void AdjustLayout()
		{
			this.ctrlHost.Layout -= new LayoutEventHandler(HostControl_Layout);
			if(this.dockingMgr.IsLayoutSuspended == true || this.dockingMgr.bWaitOnLayoutEvent == true)
				return;

			// Perform the overlap reduction sizing only if the HostForm is Visible.
			if(this.ctrlHost.Visible == false)
				this.bOverlapSizing = false;
			else
				this.bOverlapSizing = true;

			// Start the layout adjustment with the clientpool equal to the form's clientrect. After resizing each controller
			// in the list, offset the rcClientPool rect by the width/height of the controller's layout. For the next
			// controller in the list, use the client rect as the origin coords for the layout. This ensures relative
			// layout among controllers taking into account preferential positioning.
			Rectangle rcold = this.rcClientPool;
			this.rcClientPool = this.LayoutRect;

			int nAHTabCtrlWidth = this.dockingMgr.AutoHideTabHeight;
			// Step 1 - Layout the autohide panes
			{
				// Display an autohide pane, only if it contains children
				if( !this.ahTabCtrlL.IsDisposed &&
					(this.ahTabCtrlL.TabPages.Count > 0) && (this.ahTabCtrlL.Width >= 0))
				{
					this.ahTabCtrlL.LeadSpace = false;
					if(this.ahTabCtrlT.TabCount > 0)	// If top tab is visible, then set the empty left offset for the left top.
					{
						this.ahTabCtrlL.LeadSpace = true;
						if(this.ahTabCtrlL.EmptyLeft < nAHTabCtrlWidth)
							this.ahTabCtrlL.AddToEmptyLeft(nAHTabCtrlWidth);
						this.ahTabCtrlL.OnTabPanelBoundsAffected();	// Forces a layout recalc
					}
					else if(this.ahTabCtrlL.EmptyLeft > nAHTabCtrlWidth)
					{
						this.ahTabCtrlL.AddToEmptyLeft(-nAHTabCtrlWidth);
						this.ahTabCtrlL.OnTabPanelBoundsAffected();
					}
					if(this.ahTabCtrlB.TabPages.Count > 0)
						this.ahTabCtrlL.TrailSpace = true;
					else
						this.ahTabCtrlL.TrailSpace = false;

					Rectangle rcbounds = new Rectangle(this.rcClientPool.Left, this.rcClientPool.Top,
						nAHTabCtrlWidth, this.rcClientPool.Height);

					this.ahTabCtrlL.Bounds = rcbounds;
					this.rcClientPool.X += rcbounds.Width;
					this.rcClientPool.Width -= rcbounds.Width;
				}
				if( !this.ahTabCtrlT.IsDisposed &&
					(this.ahTabCtrlT.TabPages.Count > 0) && (this.ahTabCtrlT.Height >= 0))
				{
					// Also set the lead and trail space triggers based on the presence/absence of adjacent tabcontrols.
					this.ahTabCtrlT.LeadSpace = false;
					if(this.ahTabCtrlR.TabPages.Count > 0)
						this.ahTabCtrlT.TrailSpace = true;
					else
						this.ahTabCtrlT.TrailSpace = false;

					Rectangle rcbounds = new Rectangle(this.rcClientPool.Left, this.rcClientPool.Top,
													this.rcClientPool.Width, nAHTabCtrlWidth);
					this.ahTabCtrlT.Bounds = rcbounds;
					this.rcClientPool.Y += rcbounds.Height;
					this.rcClientPool.Height -= rcbounds.Height;
				}
				if( !this.ahTabCtrlR.IsDisposed &&
					(this.ahTabCtrlR.TabPages.Count > 0) && (this.ahTabCtrlR.Width >= 0))
				{
					this.ahTabCtrlR.LeadSpace = false;
					this.ahTabCtrlR.TrailSpace = false;

					Rectangle rcbounds = new Rectangle(this.rcClientPool.Right-nAHTabCtrlWidth, this.rcClientPool.Top,
						nAHTabCtrlWidth, ((this.ahTabCtrlB.TabPages.Count>0)?this.rcClientPool.Height-nAHTabCtrlWidth:this.rcClientPool.Height));
					this.ahTabCtrlR.Bounds = rcbounds;
					this.rcClientPool.Width -= rcbounds.Width;
				}
				if( !this.ahTabCtrlB.IsDisposed &&
					(this.ahTabCtrlB.TabPages.Count > 0) && (this.ahTabCtrlB.Height >= 0))
				{
					this.ahTabCtrlB.LeadSpace = false;
					int nfillx = 0;
					if(this.ahTabCtrlR.TabPages.Count > 0)
					{
						this.ahTabCtrlB.TrailSpace = true;
						nfillx = nAHTabCtrlWidth;
					}
					else
						this.ahTabCtrlB.TrailSpace = false;

					Rectangle rcbounds = new Rectangle(this.rcClientPool.Left, this.rcClientPool.Bottom-nAHTabCtrlWidth,
													this.rcClientPool.Width+nfillx, nAHTabCtrlWidth);
					this.ahTabCtrlB.Bounds = rcbounds;
					this.rcClientPool.Height -= rcbounds.Height;
				}
			}

			if(this.dockingMgr.DockToFill == false)
			{				
				DockControllerBase dc = null;
				// Step 2 - Layout all docked windows starting with priority 0
				for( int i = 0; i < this.alChildren.Count; i++ )
				{
					dc = alChildren[ i ] as DockControllerBase;
					if( dc == null ) continue;

					int nwidth = dc.LayoutRect.Width;
					int nheight = dc.LayoutRect.Height;
					switch(dc.DICurrent.dStyle)
					{
						case Syncfusion.Windows.Forms.Tools.DockingStyle.Left:
							if( (this.bOverlapSizing == true) && (nwidth > (this.rcClientPool.Width-5)) )
								nwidth = this.rcClientPool.Width-5;
							if( nwidth > 10 || ( dc as SizingController ).IsEmpty )
							{
								dc.LayoutRect = new Rectangle(this.rcClientPool.Left, this.rcClientPool.Top,
									nwidth, this.rcClientPool.Height);
								this.rcClientPool.X += dc.LayoutRect.Width;
								this.rcClientPool.Width -= dc.LayoutRect.Width;
							}							
							break;
						case Syncfusion.Windows.Forms.Tools.DockingStyle.Top:
							if( (this.bOverlapSizing == true) && (nheight > (this.rcClientPool.Height-5)) )
								nheight = this.rcClientPool.Height-5;
							if(nheight > 10 || (dc as SizingController).IsEmpty )
							{
								dc.LayoutRect = new Rectangle(this.rcClientPool.Left, this.rcClientPool.Top,
									this.rcClientPool.Width, nheight);
								this.rcClientPool.Y += dc.LayoutRect.Height;
								this.rcClientPool.Height -= dc.LayoutRect.Height;
							}
							break;
						case Syncfusion.Windows.Forms.Tools.DockingStyle.Right:
							if( (this.bOverlapSizing == true) && (nwidth > (this.rcClientPool.Width-5)) )
								nwidth = this.rcClientPool.Width-5;
							if( nwidth > 10 || ( dc as SizingController ).IsEmpty )
							{
								dc.LayoutRect = new Rectangle(this.rcClientPool.Right-nwidth, this.rcClientPool.Top,
									nwidth, this.rcClientPool.Height);
								this.rcClientPool.Width -= dc.LayoutRect.Width;
							}
							break;
						case Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom:
							if( (this.bOverlapSizing == true) && (nheight > (this.rcClientPool.Height-5)) )
								nheight = this.rcClientPool.Height-5;
							if( nheight > 10 || ( dc as SizingController ).IsEmpty )
							{
								dc.LayoutRect = new Rectangle(this.rcClientPool.Left, this.rcClientPool.Bottom-nheight,
									this.rcClientPool.Width, nheight);
								this.rcClientPool.Height -= dc.LayoutRect.Height;
							}
							break;
					}
				}
			}
			else
			{
				foreach(DockControllerBase dc in this.alChildren)
					dc.LayoutRect = this.rcClientPool;
			}

			// Step 3 - Draw docking inner border
			// Invalidate the left-over rect and allow the paint handler to draw the borders
			if( (this.rcClientPool.Width > rcold.Width) || (this.rcClientPool.Height > rcold.Height) )
			{
				Syncfusion.Runtime.InteropServices.NativeMethods.RECT rcsub = new Syncfusion.Runtime.InteropServices.NativeMethods.RECT(0,0,0,0);
				Syncfusion.Runtime.InteropServices.NativeMethods.RECT rc1 = Syncfusion.Runtime.InteropServices.NativeMethods.RECT.FromXYWH(rcClientPool.X, rcClientPool.Y, rcClientPool.Width, rcClientPool.Height);
				Syncfusion.Runtime.InteropServices.NativeMethods.RECT rc2 = Syncfusion.Runtime.InteropServices.NativeMethods.RECT.FromXYWH(rcold.X, rcold.Y, rcold.Width, rcold.Height);
				if(Syncfusion.Runtime.InteropServices.NativeMethods.SubtractRect(ref rcsub, ref rc1, ref rc2) == true)
				{
					Rectangle rcclip = new Rectangle(rcsub.left, rcsub.top, rcsub.right-rcsub.left, rcsub.bottom-rcsub.top);
					rcclip.Inflate(1,1);
					this.ctrlHost.Invalidate(rcclip);
				}
			}
			else if(this.ctrlFormClient == null)
			{
				if((this.ctrlHost.Visible == true) && (this.ctrlHost.IsHandleCreated == true))
				{
					// Draw the inner dock edge borders
					Graphics gph = this.ctrlHost.CreateGraphics();
					Region rgnborder = new Region(rcold);
					rgnborder.Exclude(Rectangle.Inflate(rcold,-2,-2));
					this.ctrlHost.Invalidate(rgnborder,false);
					this.DrawDockEdgeBorders(gph);
					gph.Dispose();
                    rgnborder.Dispose();
				}
				else
				{
					// Invalidate the inner dock edge borders
					Region rgnborder = new Region(rcold);
					rgnborder.Exclude(Rectangle.Inflate(rcold,-2,-2));
					this.ctrlHost.Invalidate(rgnborder,false);
                    rgnborder.Dispose();
				}
			}

			bool drawingSuspendNeeded = false;
			// Step 4 - Layout the MDIClient window, if present
			// If this form is an MDI container, then size the mdiclient window to the leftover clientrect
			if(this.ctrlFormClient != null)
			{
				if((this.ctrlFormClient is DockingClientPanel) && ((DockingClientPanel)this.ctrlFormClient).SizeToFit == false)
					return;

				if( (this.ctrlFormClient.Size != this.rcClientPool.Size)
					|| (this.ctrlFormClient.Location != this.rcClientPool.Location) )
				{
					// Workaround for MdiClient quirk
					// If MdiClient.Anchor is changed, all minimized and hidden MdiChildren are shown
					bool[] mdiChildrenVisibility = null;
					
					// The workaround is only necessary when the Host is an MdiContainer
					if (this.HostControl is Form && (this.HostControl as Form).IsMdiContainer)
					{
						mdiChildrenVisibility = new bool[(this.HostControl as Form).MdiChildren.Length];

						int childNumber = 0;
						foreach (Form childForm in (this.HostControl as Form).MdiChildren)
						{
							mdiChildrenVisibility[childNumber++] = childForm.Visible;
							if(childForm.WindowState == FormWindowState.Minimized && childForm.Visible == false)
							{
								drawingSuspendNeeded = true;
							}
						}
					}

					if(drawingSuspendNeeded == true)
					{
						Control mdiClient = this.DockingManager.GetMdiClient();
						if(mdiClient != null)
							NativeMethodsHelper.SuspendRedrawWindow(mdiClient.Handle);
					}

					this.ctrlFormClient.Anchor = AnchorStyles.None;
                    this.ctrlFormClient.Bounds = this.rcClientPool;
					this.ctrlFormClient.Anchor = AnchorStyles.Left|AnchorStyles.Top;

					if(drawingSuspendNeeded == true)
					{
						drawingSuspendNeeded = false;

						if (this.HostControl is Form && (this.HostControl as Form).IsMdiContainer)
						{
							int childNumber = 0;
							foreach (Form childForm in (this.HostControl as Form).MdiChildren)
							{
								// Don't bother if we don't specficially need the workaround
								if (childForm.WindowState == FormWindowState.Minimized && childForm.Visible != mdiChildrenVisibility[childNumber]) 
								{
									childForm.Visible = mdiChildrenVisibility[childNumber++];
								} 
								else 
								{
									childNumber++;
								}
							}

							Control mdiClient = this.DockingManager.GetMdiClient();
							if(mdiClient != null)
								NativeMethodsHelper.ResumeRedrawWindow(mdiClient.Handle, true);
						}
					}
				}
			}
			this.ctrlHost.Layout += new LayoutEventHandler(HostControl_Layout);
		}

		public virtual void AdjustLayoutDockArea()
		{
			if(this.dockingMgr == null)
				return;
			if(this.dockingMgr.DesignMode == true)
			{
				// If any of the Essential Tools codedomserializers have temporarily removed docking relevant
				// children from the main form, then return without adjusting layout.
				ArrayList dclist = this.dockingMgr.DockAreaControllers;
				foreach(DockControllerBase dcbase in dclist)
				{
					if(
						((dcbase is DragSplitterController) || (dcbase is DockHostController) || (dcbase is DockTabController))
						&& (dcbase.HostControl != null) && (dcbase.HostControl.Parent == null)
						&& (((dcbase is DockHostController) && (dcbase.HostControl.Controls.Count == 0))==false)
						)
					{
						return;
					}
				}
			}

			Form hostform = this.ctrlHost.FindForm() as Form;

			if( hostform != null )
			{
				if( hostform.IsMdiChild )
				{
					if( (hostform.WindowState == FormWindowState.Minimized && hostform.MdiParent.ActiveControl == hostform)
						|| hostform.MdiParent.WindowState == FormWindowState.Minimized )
					{
						this.HideAllFloatingForms();
					}
					else
					{
						if( this.alVisibleFloatingForms.Count > 0 && hostform.Visible )
							this.ShowAllFloatingForms();
					}
				}
				if( (hostform.WindowState == FormWindowState.Minimized) || (hostform.IsMdiChild && (hostform.MdiParent.WindowState == FormWindowState.Minimized)) )
					return;
			}

			Rectangle rctemp = this.ctrlHost.DisplayRectangle;
			// Run through the childcontrol's and offset the layout rect by the width/height occupied by docked controls
			foreach(Control ctrl in this.ctrlHost.Controls)
			{
				if((this.ctrlHost.Visible == true) && (ctrl.Visible == false))
					continue;
                
				switch(ctrl.Dock)
				{
					case DockStyle.Left:
						rctemp = new Rectangle(rctemp.Left+ctrl.Bounds.Width, rctemp.Top, rctemp.Width-ctrl.Bounds.Width, rctemp.Height);
						break;
					case DockStyle.Top:
						rctemp = new Rectangle(rctemp.Left, rctemp.Top+ctrl.Bounds.Height,	rctemp.Width, rctemp.Height-ctrl.Bounds.Height);
						break;
					case DockStyle.Right:
						rctemp = new Rectangle(rctemp.Left, rctemp.Top, rctemp.Width-ctrl.Bounds.Width, rctemp.Height);
						break;
					case DockStyle.Bottom:
						rctemp = new Rectangle(rctemp.Left, rctemp.Top, rctemp.Width, rctemp.Height-ctrl.Bounds.Height);
						break;
					default:	// DockStyle.None || DockStyle.Fill
						break;
				}
			}
			// Perform the new layout placement
			this.LayoutRect = rctemp;
		}

		// Hides all visible floating forms
		protected internal void HideAllFloatingForms()
		{
            HideAllFloatingForms(false);
        }

        protected internal void HideAllFloatingForms(bool fireVisibilityChange)
        {
			// Fill alVisibleFloatingForms with controls that contain these forms, then hide 'em
			for( int i = 0; i < this.dockingMgr.alEnableDocking.Count; i++ )
			{
				Control ctrl = this.dockingMgr.alEnableDocking[i] as Control;
				if( ctrl != null )
				{
					DockHost dh = ctrl.Parent as DockHost;
					if( dh != null )
					{
						FloatingForm ffrm = dh.Parent as FloatingForm;
						if (ffrm != null)
						{
							this.alVisibleFloatingForms.Add(ffrm);
							this.DockingManager.bFiredVisibilityChangedEvent = fireVisibilityChange;
							this.DockingManager.bFiredVisibilityChangingEvent = fireVisibilityChange;
							ffrm.Disable();
							this.DockingManager.bFiredVisibilityChangingEvent = this.DockingManager.HoldEvents;
							this.DockingManager.bFiredVisibilityChangedEvent = this.DockingManager.HoldEvents;
						}
					}
				}
			}
		}

		// Shows all floating forms that were visible before HideAllFloatingForms call
		protected internal void ShowAllFloatingForms()
		{
            ShowAllFloatingForms(false);
        }

		protected internal void ShowAllFloatingForms(bool fireVisibilityChange)
        {
			for( int i = 0; i < this.alVisibleFloatingForms.Count; i++ )
			{
				FloatingForm ffrm = this.alVisibleFloatingForms[i] as FloatingForm;
				if( ffrm != null )
				{
					this.DockingManager.bFiredVisibilityChangedEvent = fireVisibilityChange;
					this.DockingManager.bFiredVisibilityChangingEvent = fireVisibilityChange;
					ffrm.Enable();
					this.DockingManager.bFiredVisibilityChangingEvent = this.DockingManager.HoldEvents;
					this.DockingManager.bFiredVisibilityChangedEvent = this.DockingManager.HoldEvents;
				}
			}
			this.alVisibleFloatingForms.Clear();
		}

		public override void Refresh()
		{
			AdjustLayout();
		}

		protected internal Syncfusion.Windows.Forms.Tools.DockingStyle GetControllerBorder(DockControllerBase dctarget)
		{
			if( DockingManager.DockToFill )
			{
				DockStateControllerBase stateController = dctarget as DockStateControllerBase;
				DockingStyle dockingStyle = stateController.DockEdge;
				if( (dockingStyle == DockingStyle.Fill) || ( dockingStyle == DockingStyle.Tabbed) )
					dockingStyle = stateController.DINew.dStyle;

				if( dockingStyle == DockingStyle.Fill )
				{
					switch( stateController.DICurrent.DP )
					{
						case DockPreference.Horizontal:
							if( stateController.DICurrent.nDockIndex == 0 )
								dockingStyle = DockingStyle.Left;
							else
								dockingStyle = DockingStyle.Right;
							break;
						case DockPreference.Vertical:
							if( stateController.DICurrent.nDockIndex == 0 )
								dockingStyle = DockingStyle.Top;
							else
								dockingStyle = DockingStyle.Bottom;
							break;
					}
				}
				return dockingStyle;
			}
			else
			{
				DockInfo dinfo = DockInfo.NullInfo;
				foreach(DockControllerBase dcchild in this.alChildren)
				{
					if(RecIsChildOfController(dcchild, dctarget) == true)
						return dcchild.DICurrent.dStyle;
				}
				if( dctarget.DICurrent.dStyle != DockingStyle.Fill )
				{
					return dctarget.DICurrent.dStyle;
				}
				else
				{
					DockHostController hostController = dctarget as DockHostController;
					if( hostController != null )
					{
						return hostController.DINew.dStyle;
					}

					throw new ApplicationException( "Can not obtain controller''s style." );
				}
			}
		}


		protected bool RecIsChildOfController(DockControllerBase dc, DockControllerBase target)
		{
			IEnumerator iechild = dc.ChildHostEnumerator;

            if (dc is DockTabController && target is DockTabController)
            {
                if(dc.Equals(target))
                    return dc.Equals(target);
            }
			if( dc is DockTabController )
				iechild = dc.ChildControllers.GetEnumerator();

			if(iechild == null)	// Does not have any children
				return dc.Equals(target);

			while(iechild.MoveNext() == true)
			{
				if( RecIsChildOfController(iechild.Current as DockControllerBase, target) == true )
					return true;
			}
			return false;
		}

		// Draw the outer border for the splitter control
		public void OnAHSplitterPaint(Object obj, PaintEventArgs e)
		{
			DragSplitter splitter = obj as DragSplitter;
			if(splitter == null)
			{
				Debug.Assert(false, "Error: Invalid Paint Handler Invocation.\n");
				return;
			}
			Graphics gph = splitter.CreateGraphics();
			switch(splitter.InternalController.DICurrent.dStyle)
			{
				case Syncfusion.Windows.Forms.Tools.DockingStyle.Left:
					ControlPaint.DrawBorder3D(gph, splitter.ClientRectangle, Border3DStyle.Raised, Border3DSide.Right);
					break;
				case Syncfusion.Windows.Forms.Tools.DockingStyle.Top:
					ControlPaint.DrawBorder3D(gph, splitter.ClientRectangle, Border3DStyle.Raised, Border3DSide.Bottom);
					break;
				case Syncfusion.Windows.Forms.Tools.DockingStyle.Right:
					ControlPaint.DrawBorder3D(gph, splitter.ClientRectangle, Border3DStyle.Raised, Border3DSide.Left);
					break;
				case Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom:
					ControlPaint.DrawBorder3D(gph, splitter.ClientRectangle, Border3DStyle.Raised, Border3DSide.Top);
					break;
			}
			gph.Dispose();
		}

		public void EnterAutoHideMode(DockStateControllerBase ddcbase, bool animate)
		{
			this.dockingMgr.LockHostFormUpdate();
			bool freezed = this.dockingMgr.ForbidFreeze;
			this.dockingMgr.ForbidFreeze = true;
			Syncfusion.Windows.Forms.Tools.DockingStyle border = GetControllerBorder(ddcbase);

			DockTabController tabParent = ddcbase as DockTabController;

			if( tabParent != null && this.DockingManager.DockToFill)
			{
				foreach( DockTabPage page in tabParent.TabControl.TabPages )
				{
					page.dhcClient.DockEdge = border;
				}
			}

			// Store the border in the DINew info. This border info is used while exiting the autohide mode.
			Rectangle rclayout = ddcbase.LayoutRect;
			if(this.dockingMgr.DockToFill == true)
			{
				// Control bounds should not be > 1/2 the width/height of the host form
				if((border == DockingStyle.Top) || (border == DockingStyle.Bottom))
				{
					int maxheight = this.rcLayout.Height;
					if(this.ahTabCtrlT.Visible == true)
						maxheight += this.dockingMgr.AutoHideTabHeight;
					if(this.ahTabCtrlB.Visible == true)
						maxheight += this.dockingMgr.AutoHideTabHeight;
					if(rclayout.Height > (maxheight*0.5))
						rclayout.Height = (int)(maxheight*0.5);
				}
				else	// DockingStyle.Left||DockingStyle.Right
				{
					int maxwidth = this.rcLayout.Width;
					if(this.ahTabCtrlL.Visible == true)
						maxwidth  += this.dockingMgr.AutoHideTabHeight;
					if(this.ahTabCtrlR.Visible == true)
						maxwidth += this.dockingMgr.AutoHideTabHeight;
					if(rclayout.Width > (maxwidth*0.5))
						rclayout.Width = (int)(maxwidth*0.5);
				}
			}
			
			Rectangle rctransient = Rectangle.Empty;
			if( animate )
			{
				ddcbase.DINew = new DockInfo(null, border, -1, -1, DockPreference.All, rclayout);
				this.dockingMgr.UndockFromController(ddcbase);
				if( ddcbase.DITransient.rcDockArea.Size == Size.Empty )
					rctransient = rclayout;
				else
					rctransient = ddcbase.DITransient.rcDockArea;	// The DITransient gets reset during the undocking process
			}

			// Undocking the dockhostcontroller will remove it from the parent form. Re-add it.
			this.ctrlHost.Controls.Add(ddcbase.HostControl);
			// Create a new sizingcontroller and add this to the mainformcontroller. The sizingcontroller will
			// host the dockcontroller
			DockPreference dp = ((border == Syncfusion.Windows.Forms.Tools.DockingStyle.Left)||(border==Syncfusion.Windows.Forms.Tools.DockingStyle.Right)) ? DockPreference.Horizontal : DockPreference.Vertical;
			SizingController sc = new SizingController(this.dockingMgr, this.HostControl, dp);
			sc.DICurrent = new DockInfo(this, border, -1, -1, dp, Rectangle.Empty);
			sc.ParentController = this;
			sc.AddChild(ddcbase, border);

			// Subscribe to the sizingcontroller's splitter's paint event
			DockControllerBase splitterdc = sc.GetChildAt(1);
			splitterdc.HostControl.Paint += new PaintEventHandler(this.OnAHSplitterPaint);

			GetAHTabControl(border).AddTab(ddcbase, true);

			if( animate )
			{
				ddcbase.DITransient.rcDockArea = rctransient;
				this.ctrlHost.Focus();
			}

			if(this.dockingMgr.DockToFill)
				this.AdjustLayoutDockArea();

			this.dockingMgr.ForbidFreeze = freezed;
			this.dockingMgr.UnlockHostFormUpdate();
		}

		public void LoadInAutoHideMode(DockStateControllerBase ddcbase)
		{
			Syncfusion.Windows.Forms.Tools.DockingStyle border = ddcbase.DICurrent.dStyle;
			// Store the border in the DINew info. This border info is used while exiting the autohide mode.
			Rectangle rclayout = ddcbase.DICurrent.rcDockArea;
			ddcbase.DINew = new DockInfo(null, border, -1, -1, DockPreference.All, rclayout);
			Rectangle rctransient = ddcbase.DITransient.rcDockArea;	// The DITransient gets reset during the undocking process

			// Undocking the dockhostcontroller will remove it from the parent form. Re-add it.
			this.ctrlHost.Controls.Add(ddcbase.HostControl);
			// Create a new sizingcontroller and add this to the mainformcontroller. The sizingcontroller will
			// host the dockcontroller
			DockPreference dp = ((border == Syncfusion.Windows.Forms.Tools.DockingStyle.Left)||(border==Syncfusion.Windows.Forms.Tools.DockingStyle.Right)) ? DockPreference.Horizontal : DockPreference.Vertical;
			SizingController sc = new SizingController(this.dockingMgr, this.HostControl, dp);
			sc.DICurrent = new DockInfo(this, border, -1, -1, dp, Rectangle.Empty);
			sc.ParentController = this;
			sc.AddChild(ddcbase, border);

			// Subscribe to the sizingcontroller's splitter's paint event
			DockControllerBase splitterdc = sc.GetChildAt(1);
			splitterdc.HostControl.Paint += new PaintEventHandler(this.OnAHSplitterPaint);

			this.GetAHTabControl(border).AddTab(ddcbase, false);
			ddcbase.DITransient.rcDockArea = rctransient;
			ddcbase.HostControl.Visible = false;

			if(this.dockingMgr.DockToFill)
				this.AdjustLayoutDockArea();
		}

        private void UpdatePreviousAutoHideIndex(DockStateControllerBase ddcbase)
        {
            int index = ddcbase.AutoHideIndex;

            foreach (DockControllerBase db in ddcbase.DockingManager.alDockAreaControllers)
            {
                if (db is DockHostController && !(db as DockStateControllerBase).Equals(ddcbase))
                {
                    int queueIndex = (db as DockStateControllerBase).PreviousAutoHideIndex;
                    if ( queueIndex!= -1 && queueIndex == index)
                    {
                        index++;
                    }
                }
            }
            ddcbase.PreviousAutoHideIndex = index;
        }

		public void ExitAutoHideMode(DockStateControllerBase ddcbase, bool bcloseonexit)
		{
			this.dockingMgr.LockHostFormUpdate();
			DockingStyle border = GetControllerBorder(ddcbase);
			AHTabControl ahbordertab = GetAHTabControl(border);
			Debug.Assert((ahbordertab != null), "Error: Invalid ExitAutoHide call.\n");
                        
            UpdatePreviousAutoHideIndex(ddcbase);

			ahbordertab.RemoveTab(ddcbase);

			// Remove the controller from the parent sizingcontroller.
			if(ddcbase.ParentController is SizingController)
			{
				DockControllerBase splitterdc = ddcbase.ParentController.GetChildAt(1);
				splitterdc.HostControl.Paint -= new PaintEventHandler(this.OnAHSplitterPaint);
				DockControllerBase parent = ddcbase.ParentController;
				ddcbase.ParentController.RemoveChild(ddcbase);
				ddcbase.LayoutRect = ddcbase.DINew.rcDockArea;
				DockingStyle style = ddcbase.DINew.dStyle;
				Minimization minimized = Minimization.None;
				if( ddcbase.InternalDockWrapper != null )
				{
					minimized = ddcbase.InternalDockWrapper.Minimized;
					ddcbase.InternalDockWrapper.ControlSize = Size.Empty;
				}

				if( ddcbase is DockHostController )
					( ddcbase as DockHostController ).TransitToPrevDock();
				else
				{
					DockTabController dtc = ddcbase as DockTabController;
					dtc.TransitToPrevDock();

					if( minimized != Minimization.None && dtc.HostController != null)
					{
						dtc.Minimized = minimized;
						dtc.TabControl.Visible = false;
					}
				}

				ddcbase.DINew.dStyle = style;
			}
			else
			{

				// bcloseonexit is true if the dockhostcontroller was closed when in the AH mode.
				// Do not redock the controller.
				if( bcloseonexit == false )
				{
					if( ddcbase is DockTabController )
					{
						DockTabController dtc = ddcbase as DockTabController;
						DockHostController selectedController = dtc.SelectedController;
						dtc.HostController = null;
						dtc.PauseActivation = true;
						DockTabPage firstPage = dtc.TabControl.TabPages[0] as DockTabPage;
						DockHostController controller = firstPage.dhcClient;
						if( ddcbase.InternalDockWrapper != null )
						{
							SizingController sc = ddcbase.InternalDockWrapper.ParentController as SizingController;
							ddcbase.InternalDockWrapper.ParentController.ReplaceChild(ddcbase.InternalDockWrapper, ddcbase);							
						}
						else
						{
							foreach( DockTabPage page in dtc.TabControl.TabPages )
							{
								page.dhcClient.DockTab = null;
								page.dhcClient.DINew = new DockInfo(ddcbase.DITransient);
								if( this.dockingMgr.DockToFill == false )
								{
									if( page.dhcClient.DINew.dController.AttemptDCRDocking(page.dhcClient, page.dhcClient.DCR) == false )
									{
										if( page.dhcClient.DINew.dController is SizingController )
										{
											this.dockingMgr.DockToSizingController(page.dhcClient);
										}
										else
										{
											page.dhcClient.DINew.dController = this;
											page.dhcClient.DINew.nPriority = 0;
											page.dhcClient.DINew.nDockIndex = 0;
											if( ( page.dhcClient.DINew.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Left ) || ( page.dhcClient.DINew.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Right ) )
												page.dhcClient.DINew.DP = DockPreference.Horizontal;
											else if( ( page.dhcClient.DINew.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Top ) || ( page.dhcClient.DINew.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom ) )
												page.dhcClient.DINew.DP = DockPreference.Vertical;
											this.dockingMgr.DockToFormController(page.dhcClient);
										}
									}
								}
								else	// DockToFill
								{
									if( !page.dhcClient.DINew.dController.AttemptDCRDocking(page.dhcClient, page.dhcClient.DCR) )
									{
										page.dhcClient.DINew.dController = this;
										this.dockingMgr.DockToFormController(page.dhcClient);
									}
								}
								if( page.dhcClient.HostControl.Visible == false )
									page.dhcClient.HostControl.Visible = true;
								dtc.TabControl.TabPages.Remove(page);
								if( ( this.dockingMgr.DockToFill || ( ( this.HostControl is Form ) ? ( ( Form )this.HostControl ).IsMdiContainer : false ) && ( dtc.TabControl.TabPages.Count > 0 ) ) )
								{
									// Set the ddcbase.DITransient's dController to be equal to the recently docked dockhostcontroller.
									// This is done so that AttemptDCRDocking is invoked for the remaining dockhostcontrollers
									// in the tabcontrol
									if( !( ddcbase.DITransient.dController is MainFormController ) )
										ddcbase.DITransient.dController = this.alChildren[0] as DockControllerBase;
								}
							}
							this.dockingMgr.UndockFromController(dtc);
							dtc.TabControl.Visible = false;
							dtc.TabControl.Dispose();
						}

						//restore active tab page
						if( selectedController != null )
						{
							DockTabController parentController = selectedController.ParentController as DockTabController;
							if( parentController != null )
							{
								parentController.SelectedController = selectedController;
							}
						}

					}
					else
					{
						DockInfo dockInfo = new DockInfo(ddcbase.DITransient);
						if( dockInfo.rcDockArea == Rectangle.Empty )
						{
							dockInfo.rcDockArea = ddcbase.DINew.rcDockArea;
						}

						ddcbase.DINew = dockInfo;
						if( ddcbase is DockHostController )
							( ddcbase as DockHostController ).InternalTransitToPrevState(false);
					}
				}
			}
			this.dockingMgr.UnlockHostFormUpdate();
			if(this.dockingMgr.DockToFill)
				this.AdjustLayoutDockArea();
		}

		protected virtual void HitTestBorderRect(Rectangle rcfull, ref Rectangle[] borders, Point pt, DockInfo di)
		{
			di.dController = this;
			di.nDockIndex = 0;

			int nwidth = 0;
			int nheight = 0;
			if(this.dockingMgr.DockToFill == true)
			{
				if(this.alChildren.Count == 0)
				{
					nwidth = rcfull.Width;
					nheight = rcfull.Height;
				}
				else
				{
					nwidth = rcfull.Width/2;
					nheight = rcfull.Height/2;
				}
			}
			else
			{
				nwidth = (di.rcDockArea.Width < rcfull.Width/2) ? di.rcDockArea.Width : rcfull.Width/2;
				nheight = (di.rcDockArea.Height < rcfull.Height/2) ? di.rcDockArea.Height : rcfull.Height/2;
				// Test for min width/height
				nwidth = (nwidth < 20) ? 20 : nwidth;
				nheight = (nheight < 20) ? 20 : nheight;
			}

			if( borders[0].Contains(pt) )
			{
				if((this.dockingMgr.DockToFill == true) && (this.dockingMgr.dsDockFillAHBorder != DockingStyle.Fill))
					di.dStyle = this.dockingMgr.dsDockFillAHBorder;
				else
					di.dStyle = Syncfusion.Windows.Forms.Tools.DockingStyle.Left;
				di.rcDockArea = HostControl.RectangleToScreen(new Rectangle(rcfull.Left,rcfull.Top,nwidth,rcfull.Height));
			}
			else if( borders[1].Contains(pt) )
			{
				if((this.dockingMgr.DockToFill == true) && (this.dockingMgr.dsDockFillAHBorder != DockingStyle.Fill))
					di.dStyle = this.dockingMgr.dsDockFillAHBorder;
				else
					di.dStyle = Syncfusion.Windows.Forms.Tools.DockingStyle.Top;
				di.rcDockArea = HostControl.RectangleToScreen(new Rectangle(rcfull.Left,rcfull.Top,rcfull.Width, nheight));
			}
			else if( borders[2].Contains(pt) )
			{
				if((this.dockingMgr.DockToFill == true) && (this.dockingMgr.dsDockFillAHBorder != DockingStyle.Fill))
					di.dStyle = this.dockingMgr.dsDockFillAHBorder;
				else
					di.dStyle = Syncfusion.Windows.Forms.Tools.DockingStyle.Right;
				di.rcDockArea = HostControl.RectangleToScreen(new Rectangle(rcfull.Right-nwidth,rcfull.Top,nwidth,rcfull.Height));
			}
			else if( borders[3].Contains(pt) )
			{
				if((this.dockingMgr.DockToFill == true) && (this.dockingMgr.dsDockFillAHBorder != DockingStyle.Fill))
					di.dStyle = this.dockingMgr.dsDockFillAHBorder;
				else
					di.dStyle = Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom;
				di.rcDockArea = HostControl.RectangleToScreen(new Rectangle(rcfull.Left,rcfull.Bottom-nheight,rcfull.Width, nheight));
			}
			else
			{
				// Nothing matches, reset the controller to null
				di.dController = null;
			}
			if(di.dController != null)
			{
				if((di.dStyle == DockingStyle.Left) || (di.dStyle == DockingStyle.Right))
					di.DP = DockPreference.Horizontal;
				else
					di.DP = DockPreference.Vertical;
			}
		}

		protected void UpdatePriorityIndices()
		{
			int index = 0;
			foreach(DockControllerBase dcb in this.alChildren)
			{
				dcb.DICurrent.nDockIndex = index++;
			}
		}

		protected virtual void DrawDockEdgeBorders(Graphics gph)
		{
			if(this.dockingMgr.HostFormClientBorder == true)
			{
				// Paint a single border along the edges of the form that have windows docked onto them.
				Color clrborder = SystemColors.ControlDark;
				if(this.ctrlHost.BackColor != SystemColors.Control)
					clrborder = ControlPaint.Dark(this.ctrlHost.BackColor);
				Pen pnline = new Pen(clrborder, 1);

                bool enableRTLLayout = true;
                if (this.dockingMgr.HostControl is Form && this.dockingMgr.HostForm != null)
                {
                    enableRTLLayout = this.dockingMgr.HostForm.RightToLeftLayout;
                }

                if (enableRTLLayout && this.dockingMgr.RightToLeft == RightToLeft.Yes && !(this.dockingMgr.HostForm is RibbonForm)
                    && (Environment.OSVersion.Version.Major >= 6 || (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.ServicePack == "Service Pack 2")))
                {
                    if (this.rcClientPool.Left != this.ctrlHost.DisplayRectangle.Left)
                        gph.DrawLine(pnline, new Point(this.rcClientPool.Right - this.rcClientPool.Left - 1, this.rcClientPool.Bottom), new Point(this.rcClientPool.Right - this.rcClientPool.Left - 1, this.rcClientPool.Top));
                    if (this.rcClientPool.Right != this.ctrlHost.DisplayRectangle.Right)
                        gph.DrawLine(pnline, new Point(this.ctrlHost.DisplayRectangle.Right - this.rcClientPool.Right + 1, this.rcClientPool.Bottom), new Point(this.ctrlHost.DisplayRectangle.Right - this.rcClientPool.Right + 1, this.rcClientPool.Top));
                }
                else
                {
                    if (this.rcClientPool.Left != this.ctrlHost.DisplayRectangle.Left)
                        gph.DrawLine(pnline, new Point(this.rcClientPool.Left, this.rcClientPool.Bottom), new Point(this.rcClientPool.Left, this.rcClientPool.Top));
                    if (this.rcClientPool.Right != this.ctrlHost.DisplayRectangle.Right)
                        gph.DrawLine(pnline, new Point(this.rcClientPool.Right - 1, this.rcClientPool.Top), new Point(this.rcClientPool.Right - 1, this.rcClientPool.Bottom - 1));
                }

                if (this.rcClientPool.Top != this.ctrlHost.DisplayRectangle.Top)
                    gph.DrawLine(pnline, new Point(this.rcClientPool.Left, this.rcClientPool.Top), new Point(this.rcClientPool.Right - 1, this.rcClientPool.Top));                    
                if (this.rcClientPool.Bottom != this.ctrlHost.DisplayRectangle.Bottom)
                    gph.DrawLine(pnline, new Point(this.rcClientPool.Left, this.rcClientPool.Bottom - 1), new Point(this.rcClientPool.Right - 1, this.rcClientPool.Bottom - 1));                    
                
				pnline.Dispose();
			}
		}

		public bool AllowSplitterSizing(DockControllerBase dcbase, int deltasize)
		{
			if(this.alChildren.Contains(dcbase) == false)
				return true;	// AutoHide controller. Return true

			int dcbaseindex = this.alChildren.IndexOf(dcbase);
			if(dcbaseindex+1 == this.alChildren.Count)	// Lowest priority. Size change will not affect other controllers.
				return true;

			ArrayList afterlist = this.alChildren.GetRange(dcbaseindex+1, this.alChildren.Count-(dcbaseindex+1));

			if(dcbase.DICurrent.DP == DockPreference.Horizontal)
			{
				// dcbase is docked along the left or right border of the form and has a vertical splitter
				// Check with all Horizontal controllers that lie after it in the list to see whether the deltasize
				// change can be allowed.
				foreach(DockControllerBase dc in afterlist)
				{
					if(dc.DICurrent.DP == DockPreference.Horizontal)
						continue;
					if((dc.MinimumSize.Width != 0) && (deltasize < 0)) // Decrease in available MainForm LayoutRect
					{
						if((dc.LayoutRect.Width + deltasize) < dc.MinimumSize.Width)
							return false;
					}
				}
			}
			else	// DockPreference.Vertical
			{
				// dcbase is docked along the top or bottom border of the form and has a horizontal splitter.
				// Check with all Horizontal controllers that lie after it in the list to see whether the deltasize
				// change can be allowed.
				foreach(DockControllerBase dc in afterlist)
				{
					if(dc.DICurrent.DP == DockPreference.Vertical)
						continue;
					if((dc.MinimumSize.Height != 0) && (deltasize < 0)) // Decrease in available MainForm LayoutRect
					{
						if((dc.LayoutRect.Height + deltasize) < dc.MinimumSize.Height)
							return false;
					}
				}
			}
			return true;
		}

		protected override void Dispose(bool bdisposing)
		{
			if((bdisposing == true) && (this.ctrlHost != null))
			{
				if( m_EventSubscribed )
				{
					ctrlHost.Resize -= new System.EventHandler(this.HostControl_Resize);
					m_EventSubscribed = false;
				}

				Form toplevelform = this.ctrlHost.TopLevelControl as Form;
				if( frmTopLevelSubscribed != null )
				{
					frmTopLevelSubscribed.Resize -= new System.EventHandler( TopLevelControl_Resize );
					frmTopLevelSubscribed = null;
				}

				this.ctrlHost.VisibleChanged -= new System.EventHandler(this.HostControl_VisibleChanged);
				this.ctrlHost.Paint -= new System.Windows.Forms.PaintEventHandler(this.HostControl_Paint);
				this.ctrlHost.ControlAdded -= new System.Windows.Forms.ControlEventHandler(this.HostControl_ChildControlAdded);
				this.ctrlHost.ControlRemoved -= new System.Windows.Forms.ControlEventHandler(this.HostControl_ChildControlRemoved);
				this.ctrlHost.ParentChanged -= new EventHandler(this.HostControl_ParentChanged);
				this.ctrlHost.Layout -= new LayoutEventHandler(HostControl_Layout);
				fhCtrl.Dispose();
				fhCtrl = null;

				Form HostForm = this.ctrlHost as Form;
				if( HostForm != null )
					HostForm.MdiChildActivate -= new System.EventHandler( this.MainForm_MdiChildActivate );

				foreach(Control ctrl in this.ctrlHost.Controls)
				{
					if(((ctrl is DockHost)==false) && ((ctrl is DragSplitter)==false))
					{
						ctrl.DockChanged -= new EventHandler(this.ChildControl_DockChanged);
						ctrl.SizeChanged -= new EventHandler(this.ChildControl_SizeChanged);
						ctrl.VisibleChanged -= new EventHandler(this.ChildControl_VisibleChanged);
					}
				}

				if(this.dockingMgr.DesignProcess == false)
				{
					this.ctrlHost.Controls.Remove(this.ahTabCtrlL);
					this.ctrlHost.Controls.Remove(this.ahTabCtrlT);
					this.ctrlHost.Controls.Remove(this.ahTabCtrlR);
					this.ctrlHost.Controls.Remove(this.ahTabCtrlB);
					this.ctrlHost.Controls.Remove(this.fhCtrl);
				}

				foreach(DockControllerBase dcbase in this.alChildren)
					dcbase.Dispose();
				this.ahTabCtrlL.Dispose();
				this.ahTabCtrlL = null;
				this.ahTabCtrlT.Dispose();
				this.ahTabCtrlT = null;
				this.ahTabCtrlR.Dispose();
				this.ahTabCtrlR = null;
				this.ahTabCtrlB.Dispose();
				this.ahTabCtrlB = null;
				this.alChildren.Clear();
				this.alChildren = null;
				this.dcPriority = null;

				this.ctrlFormClient = null;
				this.ctrlHost = null;
				this.dockingMgr = null;
			}
			base.Dispose(bdisposing);
		}
		
		internal ControllerWrapper GetWrapper()
		{
			MainFormControllerWrapper mfcw = new MainFormControllerWrapper();
			
			foreach(DockControllerBase dcb in alChildren)
			{
				dcb.AddWrapper(mfcw);
			}
			
			return mfcw;
		}

		internal override void StoreControllers(ArrayList controllers)
		{
			base.StoreControllers (controllers);

			Array array = this.alChildren.ToArray(typeof(DockControllerBase));
			
			foreach(DockControllerBase dcb in array)
			{
				dcb.StoreControllers(controllers);
			}
		}

		internal override void ApplyWrapper(ControllerWrapper cw)
		{
			base.ApplyWrapper (cw);

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
						Debug.Assert(false, "Can't find controller");					
					}
				}
			}
		}
		
		internal override void ResizeControllers(ControllerWrapper cw)
		{
			for( int i = 0; i < ChildCount; i++ )
			{
				(alChildren[i] as DockControllerBase).
					ResizeControllers((ControllerWrapper) cw.Children[i]);
			}
		}

		internal override bool IsFloatOnly()
		{
			return false;
		}

		public override void DockAsMDIChild()
		{
		}

		public override void UpdateControl()
		{
			ahTabCtrlB.UpdateRenderer();
			ahTabCtrlL.UpdateRenderer();
			ahTabCtrlR.UpdateRenderer();
			ahTabCtrlT.UpdateRenderer();
		}
	}


	/// <summary>
	/// Represents a Panel derived control for use with the Essential Tools Docking Windows framework.
	/// </summary>
	/// <remarks>
	/// The DockingClientPanel is a subclass of the <see cref="System.Windows.Forms.Panel"/> control and implements
	/// a docking layout aware container that may be used for hosting the non-dockable controls on the Form or ContainerControl
	/// that houses the <see cref="DockingManager"/>. By virtue of it being aware of the docking layout, the DockingClientPanel's
	/// bounds are automatically repositioned or resized when the container's client area changes during the course
	/// of docking/undocking operations. Controls placed on the DockingClientPanel can thus avail of it's static boundary for
	/// implementing any required layout management.
	/// <p>
	/// NOTE: The DockingClientPanel should not be used in MDIContainer forms as the equivalent functionality is provided by
	/// the MDIClient window.
	/// </p>
	/// </remarks>
	/// <seealso cref="DockingManager"/>
	[
	ToolboxBitmap(typeof(DockingClientPanel), "ToolboxIcons.dockingpanel.bmp"), Docking(DockingBehavior.Never),
	Description("Represents a Panel derived control to use with Essential Tools Docking Windows framework.")
	]
	public class DockingClientPanel : Panel
	{
		private bool bSizeToFit = false;
		private BorderStyle borderStyle = BorderStyle.FixedSingle;

		/// <summary>
		/// Gets or sets a value indicating whether the container enables the user to
		/// scroll to any controls placed outside of its visible boundaries.
		/// </summary>
		/// <value>A boolean value.</value>
		[		
		LocalizableAttribute(true),
		DefaultValue(true),
		]
		public override bool AutoScroll
		{
		    get	{ return base.AutoScroll; }
		    set	{ base.AutoScroll = value; }
		} 
		/// <summary>
		/// Indicates whether the control is sized to fill
		/// the form's client area.
		/// </summary>
		/// <value>A boolean value. During runtime this is always TRUE.</value>
		[
		Description("Gets or sets a value indicating whether the control is sized to fill the form's client area."),
		Category("Syncfusion Docking"),
		DefaultValue(false)
		]
		public bool SizeToFit
		{
			get { return this.bSizeToFit; }
			set
			{
				if(this.DesignMode == true)	// SizeToFit is always set during runtime
				{
					if(this.bSizeToFit != value)
					{
						this.bSizeToFit = value;
						if((value == true) && (this.Parent != null))	// Force parent to resize
						{
							this.Parent.Width -= 1;
							this.Parent.Width += 1;
						}
					}
				}
			}
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
                if (this.Parent != null)
                {
                    this.Parent.Controls.Remove(this);
                }
				for (int i = 0; i < this.Controls.Count; i++)
				{
					((Control)this.Controls[i]).Dispose();
				} 
				this.Controls.Clear();
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Gets or sets the border style of the <see cref="DockingClientPanel"/> control.
		/// </summary>
		/// <value>A <see cref="System.Windows.Forms.BorderStyle"/> value. The default is BorderStyle.FixedSingle.</value>
		[
		Description("The border style of the control."),
		Category("Syncfusion Docking"),
		DefaultValue(BorderStyle.FixedSingle)
		]
		public new BorderStyle BorderStyle
		{
			get { return this.borderStyle; }
			set
			{
				if(this.BorderStyle != value)
				{
					if (!Enum.IsDefined(typeof(System.Windows.Forms.BorderStyle), value))
						throw new InvalidEnumArgumentException("value", ((int)(value)), typeof(BorderStyle));
					if(value == BorderStyle.Fixed3D)
					{
						this.borderStyle = value;
						base.BorderStyle = BorderStyle.Fixed3D;
						this.UpdateStyles();
					}
					else if(this.borderStyle == BorderStyle.Fixed3D)
					{
						this.borderStyle = value;
						this.UpdateStyles();
					}
					else
					{
						this.borderStyle = value;
						this.Invalidate(false);
					}
				}
			}
		}

        /// <summary>
        /// Defines the edges of the container to which a certain control is bound. 
        /// When a control is anchored to an edge, the distance between the control's  
        /// closest edge and the specified edge will remain constant.
        /// </summary>
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public override AnchorStyles Anchor
		{
			get { return base.Anchor; }
			set { base.Anchor = value; }
		}

		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public override DockStyle Dock
		{
			get { return base.Dock; }
			set	{ /*Ignore*/ }
		}


		/// <summary>
		/// Creates an instance of the <see cref="DockingClientPanel"/> class.
		/// </summary>
		public DockingClientPanel()
		{
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(DockingClientPanel));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
			base.Dock = DockStyle.None;
			base.Anchor = AnchorStyles.Top|AnchorStyles.Left;
			this.AutoScroll = true;
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnParentChanged"/>.
		/// </summary>
		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);

			if(this.Parent != null)
			{
				if(this.DesignMode == true)
				{
					if((this.Parent is Form) && ((this.Parent as Form).IsMdiContainer == true))
						throw new ApplicationException("Error - A DockingClientPanel control should not be added to an MDIContainer form.");
				}
				else
					this.bSizeToFit = true;
			}
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.SetBoundsCore"/>.
		/// </summary>
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			if(this.borderStyle == BorderStyle.FixedSingle)
			{
				Region rgnborder = new Region(this.ClientRectangle);
				rgnborder.Exclude(Rectangle.Inflate(this.ClientRectangle,-2,-2));
				this.Invalidate(rgnborder,false);

				base.SetBoundsCore(x,y,width,height,specified);

				rgnborder = new Region(this.ClientRectangle);
				rgnborder.Exclude(Rectangle.Inflate(this.ClientRectangle,-2,-2));
				this.Invalidate(rgnborder,false);
                rgnborder.Dispose();
			}
			else
				base.SetBoundsCore(x,y,width,height,specified);
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnPaint"/>.
		/// </summary>
		protected override void OnPaint(PaintEventArgs e)
		{
			if(this.borderStyle == BorderStyle.FixedSingle)
			{
				Color clrborder = SystemColors.ControlDark;
				if(this.BackColor != SystemColors.Control)
					clrborder = ControlPaint.Dark(this.BackColor);
				Pen pnborder = new Pen(clrborder, 1);
				Graphics gph = e.Graphics;
				Rectangle rcbounds = this.ClientRectangle;
				gph.DrawLine(pnborder, new Point(rcbounds.Left,rcbounds.Bottom), new Point(rcbounds.Left,rcbounds.Top));
				gph.DrawLine(pnborder, new Point(rcbounds.Left,rcbounds.Top), new Point(rcbounds.Right-1,rcbounds.Top));
				gph.DrawLine(pnborder, new Point(rcbounds.Right-1,rcbounds.Top), new Point(rcbounds.Right-1,rcbounds.Bottom-1));
				gph.DrawLine(pnborder, new Point(rcbounds.Left,rcbounds.Bottom-1), new Point(rcbounds.Right-1,rcbounds.Bottom-1));
				pnborder.Dispose();
			}

			base.OnPaint(e);
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		protected override Point ScrollToControl( Control activeControl )
		{
			this.Invalidate();
			return base.ScrollToControl( activeControl );
		}
#endif

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.WndProc"/>.
		/// </summary>
		protected override void WndProc(ref Message m)
		{
			if(((m.Msg == 0x0114/*WM_HSCROLL*/) || (m.Msg == 0x0115/*WM_VSCROLL*/) || m.Msg == NativeMethods.WM_MOUSEWHEEL )
				&& (this.borderStyle == BorderStyle.FixedSingle))
			{
				Region rgnborder = new Region(this.ClientRectangle);
				rgnborder.Exclude(Rectangle.Inflate(this.ClientRectangle,-2,-2));
				this.Invalidate(rgnborder,false);

				base.WndProc(ref m);

				rgnborder = new Region(this.ClientRectangle);
				rgnborder.Exclude(Rectangle.Inflate(this.ClientRectangle,-2,-2));
				this.Invalidate(rgnborder,false);
                rgnborder.Dispose();
			}
			else
				base.WndProc(ref m);
		}
	}

	[ToolboxItem(false)]
	[Syncfusion.Documentation.DocumentationExclude()]
	public class FocusHolder : Control
	{
		protected Control m_lastActive = null;
		protected bool m_lockFocus = false;
		protected Control m_hostControl = null;

		internal bool LockFocus
		{
			get
			{
				return m_lockFocus;
			}
			set
			{
				if( m_lockFocus != value )
				{
					m_lockFocus = value;
				}
			}
		}

		internal Control LastActiveControl
		{
			get
			{
				return m_lastActive;
			}
			set
			{
				if( m_lastActive != value )
				{
					m_lastActive = value;
				}
			}
		}

		public FocusHolder()
		{
			this.SetStyle(ControlStyles.Selectable, true);
			this.TabIndex = 0;
			this.Visible = false;
			this.Size = new Size(1,1);
		}

		internal void Enable()
		{
			if( m_hostControl != null )
				m_hostControl.Controls.Add(this);
		}

		internal void Disable()
		{
			if( this.Parent != null )
			{
				m_hostControl = this.Parent;
				this.Parent.Controls.Remove(this);
			}
		}

		protected override void WndProc( ref Message m )
		{
			if( m.Msg == NativeMethods.WM_KILLFOCUS )
			{
				Control ctrl = Control.FromHandle(m.WParam);

				if( ctrl is DragTargetForm || m_lockFocus )
				{
					m.Result = new IntPtr(1);
					return;
				}
			}
			base.WndProc(ref m);
		}
	}
}