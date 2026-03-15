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
using Syncfusion.Windows.Forms.Tools.Renderers;

namespace Syncfusion.Windows.Forms.Tools
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DragSplitterController : DockControllerBase, IResizable
	{
		protected DragSplitter ctrlHost = null;

		public override Control HostControl
		{
			get { return this.ctrlHost; }
		}

		public override Rectangle LayoutRect
		{
			get { return this.ctrlHost.Bounds; }
			set
			{
				this.ctrlHost.Bounds = value;

				// Set the control layoutrect for the current dockinfo. This should be equal to the splitter
				// control's bounds, but in screen coords. The DICurrent.rcDockArea rect will be used while dragging.
				if( this.ctrlHost.Parent != null )
					this.DICurrent.rcDockArea = this.ctrlHost.Parent.RectangleToScreen(value);
			}
		}

		public override bool Floating
		{
			get { return this.ParentController.Floating; }
			set {}
		}

		public override int ChildCount
		{
			get { return -1; }
		}

		public override int ChildHostCount
		{
			get { return 0;	}
		}

		public override IEnumerator DCR
		{
			get
			{
				return new IEnumWrapper(null);
			}
		}

		public DragSplitterController(DockingManager mgr, DragSplitter host) : base(mgr)
		{
			this.ctrlHost = host;
		}

		public override void AddChild(DockControllerBase dc, Syncfusion.Windows.Forms.Tools.DockingStyle db)
		{
			// No imp
		}

		public override void InsertChild(DockControllerBase dc, int i, Syncfusion.Windows.Forms.Tools.DockingStyle db)
		{
			// No imp
		}

		public override void RemoveChild(DockControllerBase dc)
		{
			// No imp
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
			// No imp
		}

		public override void GetDockInfo(Control ctrl, Point pt, DockInfo di)
		{
			SizingController parent = this.ParentController as SizingController;
			di.dController = parent;
			di.nPriority = parent.DICurrent.nPriority;

			// If the splitter's parent is hosted by the mainform controller, then the insertion index will be
			// the position after the splitter. On the other hand if the splitter is within a floating frame or a non-direct child
			// of the mainformcontroller, then the splitter index will be the insertion index.
			if(parent.ParentController is MainFormController)
				di.nDockIndex = this.dockInfoCurrent.nDockIndex+1;
			else
				di.nDockIndex = this.dockInfoCurrent.nDockIndex;
			di.dStyle = this.dockInfoCurrent.dStyle;
			di.DP = this.dockInfoCurrent.DP;

			DragSplitter splitter = this.HostControl as DragSplitter;
			int nindex = this.dockInfoCurrent.nDockIndex;

			DockControllerBase pane1 = null;
			DockControllerBase pane2 = null;
			if( (this.ParentController.DICurrent.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Right) ||
				(this.ParentController.DICurrent.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom) )
			{
				if( this.ParentController.ChildCount > (nindex+1) )
					pane1 = this.ParentController.GetChildAt(nindex+1);
				pane2 = this.ParentController.GetChildAt(nindex-1);
			}
			else
			{
				pane1 = this.ParentController.GetChildAt(nindex-1);
				if( this.ParentController.ChildCount > (nindex+1) )
					pane2 = this.ParentController.GetChildAt(nindex+1);
			}

			int ntotaldim = 0;
			int nreqdim = 0;
			if(this.dockInfoCurrent.DP == DockPreference.Horizontal)	// Vertical Splitter
			{
				// If pane1 or pane2 equals null, then the splitter is housed within the mainformcontroller.
				// In this case, make the form controller as the missing pane.
				if(pane1 == null)	// Syncfusion.Windows.Forms.Tools.DockingStyle.Right
				{
					Debug.Assert((this.dockInfoCurrent.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Right), "Error: Incorrect GetDockInfo call.\n");
					pane1 = this.dockingMgr.GetDockController(this.HostControl.GetContainerControl() as ContainerControl);
					ntotaldim = pane1.LayoutRect.Width;
					nreqdim = (ctrl.Width < ntotaldim/2) ? ctrl.Width : ntotaldim/2;
					// Trim drag rect to fit into the host window's clientrect
					if(pane1.LayoutRect.Left+nreqdim > splitter.Left)
						nreqdim = splitter.Left - pane1.LayoutRect.Left;
					di.rcDockArea = splitter.Parent.RectangleToScreen(new Rectangle(splitter.Left-nreqdim, splitter.Top, nreqdim, splitter.Height));
				}
				else if(pane2 == null)	// Syncfusion.Windows.Forms.Tools.DockingStyle.Left
				{
					Debug.Assert((this.dockInfoCurrent.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Left), "Error: Incorrect GetDockInfo call.\n");
					pane2 = this.dockingMgr.GetDockController(this.HostControl.GetContainerControl() as ContainerControl);
					ntotaldim = pane2.LayoutRect.Width;
					nreqdim = (ctrl.Width < ntotaldim/2) ? ctrl.Width : ntotaldim/2;
					// If the dragrect goes over the hostclient rect, then trim it to fit
					if(pane2.LayoutRect.Right-nreqdim < splitter.Right)
						nreqdim = pane2.LayoutRect.Right - splitter.Right;
					di.rcDockArea = splitter.Parent.RectangleToScreen(new Rectangle(splitter.Right, splitter.Top, nreqdim, splitter.Height));
				}
				else
				{
					// Use the two controlpane widths to calculate the prediction rect for the new dockhost
					ntotaldim = pane1.LayoutRect.Width + splitter.Width + pane2.LayoutRect.Width;
					nreqdim = (ctrl.Width < ntotaldim/2) ? ctrl.Width : ntotaldim/2;
					// Estimate the width/height that each child pane has to forgo to accomodate the new child
					int np1 = pane1.LayoutRect.Width -  (int)(((float)pane1.LayoutRect.Width/(float)ntotaldim)*((float)ntotaldim-nreqdim));
					int np2 = pane2.LayoutRect.Width - (int)(((float)pane2.LayoutRect.Width/(float)ntotaldim)*((float)ntotaldim-nreqdim));
					di.rcDockArea = splitter.Parent.RectangleToScreen(new Rectangle(splitter.Left-np1, splitter.Top, nreqdim, splitter.Height));
				}
			}
			else	// Horizontal Splitter
			{
				if(pane1 == null)
				{
					Debug.Assert((this.dockInfoCurrent.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom), "Error: Incorrect GetDockInfo call.\n");
					pane1 = this.dockingMgr.GetDockController(this.HostControl.GetContainerControl() as ContainerControl);
					ntotaldim = pane1.LayoutRect.Height;
					nreqdim = (ctrl.Height < ntotaldim/2) ? ctrl.Height : ntotaldim/2;
					if(pane1.LayoutRect.Top+nreqdim > splitter.Top)
						nreqdim = splitter.Top - pane1.LayoutRect.Top;
					di.rcDockArea = splitter.Parent.RectangleToScreen(new Rectangle(splitter.Left, splitter.Top-nreqdim, splitter.Width, nreqdim));
				}
				else if(pane2 == null)
				{
					Debug.Assert((this.dockInfoCurrent.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Top), "Error: Incorrect GetDockInfo call.\n");
					pane2 = this.dockingMgr.GetDockController(this.HostControl.GetContainerControl() as ContainerControl);
					ntotaldim = pane2.LayoutRect.Height;
					nreqdim = (ctrl.Height < ntotaldim/2) ? ctrl.Height : ntotaldim/2;
					if(pane2.LayoutRect.Bottom-nreqdim < splitter.Bottom)
						nreqdim = pane2.LayoutRect.Bottom - splitter.Bottom;
					di.rcDockArea = splitter.Parent.RectangleToScreen(new Rectangle(splitter.Left, splitter.Bottom, splitter.Width, nreqdim));
				}
				else
				{
					ntotaldim = pane1.LayoutRect.Height + splitter.Height + pane2.LayoutRect.Height;
					nreqdim = (ctrl.Height < ntotaldim/2) ? ctrl.Height : ntotaldim/2;
					int np1 = pane1.LayoutRect.Height - (int)(((float)pane1.LayoutRect.Height/(float)ntotaldim)*((float)ntotaldim-nreqdim));
					int np2 = pane2.LayoutRect.Height - (int)(((float)pane2.LayoutRect.Height/(float)ntotaldim)*((float)ntotaldim-nreqdim));
					di.rcDockArea = splitter.Parent.RectangleToScreen(new Rectangle(splitter.Left, splitter.Top-np1, splitter.Width, nreqdim));
				}
			}
		}

		protected internal override DockControllerBase QueryController( string uniqueName )
		{
			return null;
		}

		public override bool QueryDropProceedWithDock(Control ctrldrop, Syncfusion.Windows.Forms.Tools.DockingStyle style)
		{
			DockControllerBase pane1 = null;
			DockControllerBase pane2 = null;
			int nindex = this.dockInfoCurrent.nDockIndex;
			if( (this.ParentController.DICurrent.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Right) ||
				(this.ParentController.DICurrent.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom) )
			{
				if( this.ParentController.ChildCount > (nindex+1) )
					pane1 = this.ParentController.GetChildAt(nindex+1);
				pane2 = this.ParentController.GetChildAt(nindex-1);
			}
			else
			{
				pane1 = this.ParentController.GetChildAt(nindex-1);
				if( this.ParentController.ChildCount > (nindex+1) )
					pane2 = this.ParentController.GetChildAt(nindex+1);
			}

			if( (pane1 is DockStateControllerWrapper) || (pane1 != null) && (pane1.HostControl.Equals(ctrldrop)))
				return false;
			if( (pane2 is DockStateControllerWrapper) || (pane2 != null) && (pane2.HostControl.Equals(ctrldrop)))
				return false;
			if((pane1 == null) || (pane2 == null))	// Pass on to the mainframecontroller
			{
				DockControllerBase dcmainfrm = this.ParentController.ParentController;
				Debug.Assert(dcmainfrm.MainFormController == true);
				return dcmainfrm.QueryDropProceedWithDock(ctrldrop, style);
			}
			// No special cases. Just pass on to the first pane
			if(this.dockInfoCurrent.DP == DockPreference.Horizontal)	// Vertical Splitter
				return pane1.QueryDropProceedWithDock(ctrldrop, Syncfusion.Windows.Forms.Tools.DockingStyle.Right);
			else
				return pane1.QueryDropProceedWithDock(ctrldrop, Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom);
		}

		protected internal void HandleMouseDownImp(MouseButtons button, Point ptclient)
		{
			if((button == MouseButtons.Left) && (this.dockingMgr.FreezeResizing != true) && CanMove)
				this.dockingMgr.DragProvider.ProcessMouseDown( this, ctrlHost, ctrlHost.PointToScreen(ptclient));
		}

		internal bool CanMove
		{
			get
			{
				bool draggable = true;
				DockPreference pref = ( this.ParentController as SizingController ).DockingOrder;

				foreach( DockControllerBase dcb in this.ParentController.ChildControllers )
				{ 
					SizingController sc = dcb as SizingController;
					DockStateControllerBase dscb = dcb as DockStateControllerBase;

					if( sc != null )
					{
						Direction resize = sc.CanResize;
						if( pref == DockPreference.Vertical && (resize & Direction.Vertical) != Direction.Vertical)
							draggable = false;

						if( pref == DockPreference.Horizontal && ( resize & Direction.Horizontal ) != Direction.Horizontal )
							draggable = false;
					}

					if( dscb != null && dscb.FreezeResize )
						draggable = false;

					if( draggable == false )
						break;
				}

				return draggable;
			}
		}

		protected internal void HandleMouseUpImp(MouseButtons button, Point ptclient)
		{
			if( (button == MouseButtons.Left) && (this.dockingMgr.DragProvider.DraggingControl != null) )
			{
                if (this.ToplevelController is FloatingFormController)
                    this.ToplevelController.HostControl.Invalidate(true);

                this.DockingManager.dcHostForm.HostControl.Invalidate(true);
				this.ResizeTargetPanes();
				this.dockingMgr.DragProvider.ProcessMouseUp( this, ctrlHost, ctrlHost.PointToScreen(ptclient));
				this.ParentController.AdjustLayout();

				// Fire the splittermoved event
				this.ctrlHost.OnSplitterMoved(new SplitterEventArgs(this.ctrlHost.Left, this.ctrlHost.Top, ptclient.X, ptclient.Y));

				// Forcibly update the designer state
				if(this.dockingMgr.DesignMode == true)
					this.dockingMgr.UpdateDesigner();
			}
		}

		protected internal void HandleMouseMoveImp(MouseButtons button, Point ptclient)
		{
			if((button == MouseButtons.Left) && (this.dockingMgr.FreezeResizing != true) && CanMove)
			{
				this.dockingMgr.DragProvider.ProcessMouseMove(this, ctrlHost, ctrlHost.PointToScreen(ptclient));
				// Fire the splittermoving event
				this.ctrlHost.OnSplitterMoving(new SplitterEventArgs(this.ctrlHost.Left, this.ctrlHost.Top, ptclient.X, ptclient.Y));

				foreach( DockControllerBase dcb in this.ParentController.ChildControllers )
				{ 
					if( dcb.Maximized || dcb.Minimized != Minimization.None )
					{
						this.ParentController.Minimized = Minimization.None;

						foreach( DockControllerBase ctrl in this.ParentController.ChildControllers )
							ctrl.Maximized = false;

						break;
					}
				}
			}
		}

		// Based on the new splitter position, resize target controls
		public void ResizeTargetPanes()
		{
			int nindex = this.dockInfoCurrent.nDockIndex;

			DockControllerBase pane1 = null;
			DockControllerBase pane2 = null;
			if( (this.ParentController.ParentController.MainFormController == true) &&
				( (this.ParentController.DICurrent.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Right) ||
				(this.ParentController.DICurrent.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom) )	)
			{
				if( this.ParentController.ChildCount > (nindex+1) )
					pane1 = this.ParentController.GetChildAt(nindex+1);
				pane2 = this.ParentController.GetChildAt(nindex-1);
			}
			else
			{
				pane1 = this.ParentController.GetChildAt(nindex-1);
				if( this.ParentController.ChildCount > (nindex+1) )
					pane2 = this.ParentController.GetChildAt(nindex+1);
			}

			Rectangle rcdockclient = this.ctrlHost.Parent.RectangleToClient(this.dockInfoCurrent.rcDockArea);
			if(this.ctrlHost.Cursor == Cursors.VSplit)	//Vertical splitter
			{
				// Has the splitter infringed on the left pane?
				if(this.ctrlHost.Bounds.Left > rcdockclient.Left)	// Moved left
				{
					int deltax = this.ctrlHost.Bounds.Left - rcdockclient.Left;
					if(pane1 == null)
					{
						Rectangle rcPaneLayout = new Rectangle(pane2.LayoutRect.Left-deltax, pane2.LayoutRect.Top,
							pane2.LayoutRect.Width+deltax, pane2.LayoutRect.Height);

						pane2.LayoutRect = rcPaneLayout;
						pane2.DITransient.rcDockArea = rcPaneLayout;

						DockControllerBase pc = pane2.ParentController;
						pc.LayoutRect = new Rectangle(pc.LayoutRect.Left-deltax, pc.LayoutRect.Top,
							pc.LayoutRect.Width+deltax, pc.LayoutRect.Height);
						// Adjust layout of the mainformcontroller that hosts this splitter
						pc.ParentController.AdjustLayout();
					}
					else if(pane2 == null)
					{
						Rectangle rcPaneLayout = new Rectangle(pane1.LayoutRect.Left, pane1.LayoutRect.Top,
							pane1.LayoutRect.Width-deltax, pane1.LayoutRect.Height);

						pane1.LayoutRect = rcPaneLayout;
						pane1.DITransient.rcDockArea = rcPaneLayout;

						DockControllerBase pc = pane1.ParentController;
						pc.LayoutRect = new Rectangle(pc.LayoutRect.Left, pc.LayoutRect.Top,
							pc.LayoutRect.Width-deltax, pc.LayoutRect.Height);
						// Adjust layout of the mainformcontroller that hosts this splitter
						pc.ParentController.AdjustLayout();
					}
					else	// Splitter lies between 2 DockHostControllers. Set the transient rect value for the 2 dhcs
					{
						Rectangle rcPaneLayout1 = new Rectangle(pane1.LayoutRect.Left, pane1.LayoutRect.Top,
							pane1.LayoutRect.Width-deltax, pane1.LayoutRect.Height);

						pane1.LayoutRect = rcPaneLayout1;
						pane1.DITransient.rcDockArea = rcPaneLayout1;

						Rectangle rcPaneLayout2 = new Rectangle(pane2.LayoutRect.Left-deltax, pane2.LayoutRect.Top,
							pane2.LayoutRect.Width+deltax, pane2.LayoutRect.Height);

						pane2.LayoutRect = rcPaneLayout2;
						pane2.DITransient.rcDockArea = rcPaneLayout2;
					}
				}
				else if(rcdockclient.Right > this.ctrlHost.Bounds.Right)  // Moved right
				{
					int deltax = rcdockclient.Right - this.ctrlHost.Bounds.Right;
					if(pane1 == null)
					{
						Rectangle rcPaneLayout = new Rectangle(pane2.LayoutRect.Left+deltax, pane2.LayoutRect.Top,
							pane2.LayoutRect.Width-deltax, pane2.LayoutRect.Height);

						pane2.LayoutRect = rcPaneLayout;
						pane2.DITransient.rcDockArea = rcPaneLayout;

						DockControllerBase pc = pane2.ParentController;
						pc.LayoutRect = new Rectangle(pc.LayoutRect.Left+deltax, pc.LayoutRect.Top,
							pc.LayoutRect.Width-deltax, pc.LayoutRect.Height);
						pc.ParentController.AdjustLayout();
					}
					else if(pane2 == null)
					{
						Rectangle rcPaneLayout = new Rectangle( pane1.LayoutRect.Left, pane1.LayoutRect.Top,
							pane1.LayoutRect.Width+deltax, pane1.LayoutRect.Height );

						pane1.LayoutRect = rcPaneLayout;
						pane1.DITransient.rcDockArea = rcPaneLayout;

						DockControllerBase pc = pane1.ParentController;

						pc.LayoutRect = new Rectangle(pc.LayoutRect.Left, pc.LayoutRect.Top,
							pc.LayoutRect.Width+deltax, pc.LayoutRect.Height);

						pc.ParentController.AdjustLayout();
					}
					else
					{
						Rectangle rcPaneLayout1 = new Rectangle(pane1.LayoutRect.Left, pane1.LayoutRect.Top,
							pane1.LayoutRect.Width+deltax, pane1.LayoutRect.Height);

						pane1.LayoutRect = rcPaneLayout1;
						pane1.DITransient.rcDockArea = rcPaneLayout1;

						Rectangle rcPaneLayout2 = new Rectangle(pane2.LayoutRect.Left+deltax, pane2.LayoutRect.Top,
							pane2.LayoutRect.Width-deltax, pane2.LayoutRect.Height);

						pane2.LayoutRect = rcPaneLayout2;
						pane2.DITransient.rcDockArea = rcPaneLayout2;
					}
				}

			}
			else	// Horizontal splitter
			{
				// Has the splitter infringed on the top pane?
				if(this.ctrlHost.Bounds.Top > rcdockclient.Top)	// Moved up
				{
					int deltay = this.ctrlHost.Bounds.Top - rcdockclient.Top;
					if(pane1 == null)
					{
						Rectangle rcPaneLayout = new Rectangle(pane2.LayoutRect.Left, pane2.LayoutRect.Top-deltay,
							pane2.LayoutRect.Width, pane2.LayoutRect.Height+deltay);

						pane2.LayoutRect = rcPaneLayout;
						pane2.DITransient.rcDockArea = rcPaneLayout;

						DockControllerBase pc = pane2.ParentController;
						pc.LayoutRect = new Rectangle(pc.LayoutRect.Left, pc.LayoutRect.Top-deltay,
							pc.LayoutRect.Width, pc.LayoutRect.Height+deltay);
						pc.ParentController.AdjustLayout();
					}
					else if(pane2 == null)
					{
						Rectangle rcPaneLayout = new Rectangle(pane1.LayoutRect.Left, pane1.LayoutRect.Top,
							pane1.LayoutRect.Width, pane1.LayoutRect.Height-deltay);

						pane1.LayoutRect = rcPaneLayout;
						pane1.DITransient.rcDockArea = rcPaneLayout;

						DockControllerBase pc = pane1.ParentController;
						pc.LayoutRect = new Rectangle(pc.LayoutRect.Left, pc.LayoutRect.Top,
							pc.LayoutRect.Width, pc.LayoutRect.Height-deltay);
						pc.ParentController.AdjustLayout();
					}
					else // Splitter lies between 2 DockHostControllers. Set the transient rect value for the 2 dhcs
					{
						Rectangle rcPaneLayout1 = new Rectangle(pane1.LayoutRect.Left, pane1.LayoutRect.Top,
							pane1.LayoutRect.Width, pane1.LayoutRect.Height-deltay);

						pane1.LayoutRect = rcPaneLayout1;
						pane1.DITransient.rcDockArea = rcPaneLayout1;

						Rectangle rcPaneLayout2 = new Rectangle(pane2.LayoutRect.Left, pane2.LayoutRect.Top-deltay,
							pane2.LayoutRect.Width, pane2.LayoutRect.Height+deltay);

						pane2.LayoutRect = rcPaneLayout2;
						pane2.DITransient.rcDockArea = rcPaneLayout2;
					}
				}
				else if(rcdockclient.Bottom > this.ctrlHost.Bounds.Bottom) // Moved down
				{
					int deltay = rcdockclient.Bottom - this.ctrlHost.Bounds.Bottom;
					if(pane1 == null)
					{
						Rectangle rcPaneLayout = new Rectangle(pane2.LayoutRect.Left, pane2.LayoutRect.Top+deltay,
							pane2.LayoutRect.Width, pane2.LayoutRect.Height-deltay);

						pane2.LayoutRect = rcPaneLayout;
						pane2.DITransient.rcDockArea = rcPaneLayout;

						DockControllerBase pc = pane2.ParentController;
						pc.LayoutRect = new Rectangle(pc.LayoutRect.Left, pc.LayoutRect.Top+deltay,
							pc.LayoutRect.Width, pc.LayoutRect.Height-deltay);
						pc.ParentController.AdjustLayout();
					}
					else if(pane2 == null)
					{
						Rectangle rcPaneLayout = new Rectangle(pane1.LayoutRect.Left, pane1.LayoutRect.Top,
							pane1.LayoutRect.Width, pane1.LayoutRect.Height+deltay);

						pane1.LayoutRect = rcPaneLayout;
						pane1.DITransient.rcDockArea = rcPaneLayout;

						DockControllerBase pc = pane1.ParentController;
						pc.LayoutRect = new Rectangle(pc.LayoutRect.Left, pc.LayoutRect.Top,
							pc.LayoutRect.Width, pc.LayoutRect.Height+deltay);
						pc.ParentController.AdjustLayout();
					}
					else
					{
						Rectangle rcPaneLayout1 = new Rectangle(pane1.LayoutRect.Left, pane1.LayoutRect.Top,
							pane1.LayoutRect.Width, pane1.LayoutRect.Height+deltay);

						pane1.LayoutRect = rcPaneLayout1;
						pane1.DITransient.rcDockArea = rcPaneLayout1;

						Rectangle rcPaneLayout2 = new Rectangle(pane2.LayoutRect.Left, pane2.LayoutRect.Top+deltay,
							pane2.LayoutRect.Width, pane2.LayoutRect.Height-deltay);

						pane2.LayoutRect = rcPaneLayout2;
						pane2.DITransient.rcDockArea = rcPaneLayout2;
					}
				}
			}
		}

		public override Size MinimumSize
		{
			get
			{
				if(this.DICurrent.DP == DockPreference.Vertical)
					return new Size(0, 10);
				else
					return new Size(10, 0);
			}
		}

		protected override void Dispose(bool bdisposing)
		{
			if((bdisposing == true) && (this.ctrlHost != null))
			{
				this.ctrlHost.Dispose();
				this.ctrlHost = null;
				this.DockingManager.alDockAreaControllers.Remove( this );
			}
			base.Dispose(bdisposing);
		}

		internal override void AddWrapper(ControllerWrapper cw)
		{
			DragSplitterControllerWrapper dscw = new DragSplitterControllerWrapper();
			dscw.LayoutRect = LayoutRect;
			cw.Children.Add(dscw);
		}

		internal override void StoreControllers(ArrayList controllers)
		{
			base.StoreControllers (controllers);

			this.ParentController.RemoveChild(this);
			controllers.Add(this);
		}

		internal override bool IsEqual(ControllerWrapper cw)
		{
			return (cw is DragSplitterControllerWrapper);
		}

		internal override void ResizeControllers(ControllerWrapper cw)
		{
			DragSplitterControllerWrapper dscw = (DragSplitterControllerWrapper) cw;
			LayoutRect = dscw.LayoutRect;
			DITransient.rcDockArea = Rectangle.Empty;
		}

		internal override bool IsFloatOnly()
		{
			return false;
		}

		#region IResizable implementation
	
		public Size CalculateSize(Size parentSize, Size newParentSize)
		{
			if( DICurrent.DP == DockPreference.Horizontal )
			{
				return new Size( LayoutRect.Width, newParentSize.Height);
			}

			if( DICurrent.DP == DockPreference.Vertical )
			{
				return new Size( newParentSize.Width, LayoutRect.Height);
			}
			
			return LayoutRect.Size;
		}

		public bool IsVerticallyResizable()
		{
			return false;
		}

		public bool IsHorizontallyResizable()
		{
			return false;
		}

		#endregion

		public override void DockAsMDIChild()
		{
		}

		public override void UpdateControl()
		{
			ctrlHost.Invalidate();
		}
	}


	// Splitter class
	[
		ToolboxItem(false),
		DesignTimeVisible(false)
	]
	[Syncfusion.Documentation.DocumentationExclude()]
	public class DragSplitter : Control, IDraggable, IDockingManagerDesignerMouseHook
	{
		protected const int nSplitOffLt = 20;
		protected const int nCaptionHt = 16;

		protected DragSplitterController dcInternal = null;
		protected Rectangle rcDrag = Rectangle.Empty;

		protected static int nNamingCount = 0;

		// Events fired before and after the splitter is moved
		public event SplitterEventHandler DragSplitterMoving;
		public event SplitterEventHandler DragSplitterMoved;

		public DockControllerBase InternalController
		{
			get { return this.dcInternal; }
			set
			{
				this.dcInternal = value as DragSplitterController;
			}
		}

		public DockInfo DragDockInfo
		{
			get { return this.dcInternal.DICurrent;	}

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

		public override Cursor Cursor
		{
			get
			{
				Cursor current = base.Cursor;
				if( this.dcInternal.DockingManager.FreezeResizing || !this.dcInternal.CanMove )
					current = Cursors.Default;

				return current;
			}
			set
			{
				base.Cursor = value;
			}
		}

		public DragSplitter(DockingManager dmgr)
		{
			this.dcInternal = new DragSplitterController(dmgr, this);
			this.SetStyle(ControlStyles.Selectable, false);
			this.TabStop = false;
			this.CreateControl();
			this.Size = new Size(0,0);	// Set 0 size to prevent flash
			this.Visible = true;

			DragSplitter.nNamingCount++;
			this.Name = String.Concat("DragSplitter_", DragSplitter.nNamingCount.ToString());
		}

        public void UpdateCursor()
        {
            if (this.dcInternal != null && !this.dcInternal.DockingManager.FreezeResizing)
            {
                DockPreference dp = (this.dcInternal.ParentController as SizingController).DockingOrder;

                if (dp == DockPreference.Horizontal)
                {
                    if (this.Cursor != Cursors.VSplit)
                        this.Cursor = Cursors.VSplit;
                }
                else //DockPreference.Vertical
                {
                    if (this.Cursor != Cursors.HSplit)
                        this.Cursor = Cursors.HSplit;
                }
            }
        }

		// Implementation of the IDraggable interface methods
		public bool IsSuitableDockTarget(DockControllerBase dc)
		{
			return false;
		}

		public bool InitiateDrag(MouseAction action, Point ptscreen)
		{
			if( (action == MouseAction.LBtnDown) && (this.ClientRectangle.Contains(this.PointToClient(ptscreen)) == true) )
				return true;
			return false;
		}

		public DragAxis AllowedDragAxis(ref Point ptdrag, Point ptdelta)
		{
			Point ptclient = this.dcInternal.ParentController.HostControl.PointToClient(ptdrag);
			int nindex = this.dcInternal.DICurrent.nDockIndex;
			DockControllerBase pane1 = null;
			DockControllerBase pane2 = null;
			if( (this.dcInternal.ParentController.ParentController.MainFormController == true) &&
				( (this.dcInternal.ParentController.DICurrent.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Right) ||
				(this.dcInternal.ParentController.DICurrent.dStyle == Syncfusion.Windows.Forms.Tools.DockingStyle.Bottom) ) )
			{
				if( this.dcInternal.ParentController.ChildCount > (nindex+1) )
					pane1 = this.dcInternal.ParentController.GetChildAt(nindex+1);
				pane2 = this.dcInternal.ParentController.GetChildAt(nindex-1);
			}
			else
			{
				pane1 = this.dcInternal.ParentController.GetChildAt(nindex-1);
				if( this.dcInternal.ParentController.ChildCount > (nindex+1) )
					pane2 = this.dcInternal.ParentController.GetChildAt(nindex+1);
			}

			// If either pane is null, then the splitter is housed within the mainformcontroller.
			// In this case, make the form controller as the missing pane.
			if(pane1 == null)
				pane1 = this.dcInternal.DockingManager.GetDockController(this.GetContainerControl() as ContainerControl);
			else if(pane2 == null)
				pane2 = this.dcInternal.DockingManager.GetDockController(this.GetContainerControl() as ContainerControl);

			// Do not allow drag to proceed beyond the panes on either side
			if(this.Cursor == Cursors.VSplit)	//Vertical splitter
			{
				if( (ptclient.X > (pane1.LayoutRect.Left + nSplitOffLt)) &&
					(ptclient.X < (pane2.LayoutRect.Right - nSplitOffLt)) )
				{
					// Check Pane1 and Pane2 for min/max control settings
					int deltax = Math.Abs(ptdelta.X);

					// If either pane1 or pane2 is the MainFormController, then check with the docked controllers on the
					// adjancent borders for the min/max extents
					if(pane1.MainFormController == true)
					{
						if(ptclient.X < pane2.LayoutRect.Left)	// Pane2 is being increased in size
						{
							if((pane1 as MainFormController).AllowSplitterSizing(pane2.ParentController, -deltax) == false)
								return DragAxis.None;
						}
						else if(ptclient.X > pane2.LayoutRect.Left)	// Pane2 is being decreased in size
						{
							if((pane1 as MainFormController).AllowSplitterSizing(pane2.ParentController, deltax) == false)
								return DragAxis.None;
						}
					}
					else if(pane2.MainFormController == true)
					{
						if(ptclient.X > pane1.LayoutRect.Right)	// Pane1 is being increased in size
						{
							if((pane2 as MainFormController).AllowSplitterSizing(pane1.ParentController, -deltax) == false)
								return DragAxis.None;
						}
						else if(ptclient.X < pane1.LayoutRect.Right) // Pane1 is being decreased in size
						{
							if((pane2 as MainFormController).AllowSplitterSizing(pane1.ParentController, deltax) == false)
								return DragAxis.None;
						}
					}

					if((ptclient.X > pane1.LayoutRect.Left) && (ptclient.X < pane1.LayoutRect.Right))
					{
						// Pane1 is being reduced in size. Check for min extents
						if( pane1.MinimumSize.Width != 0 ) 
						{
							int ctrlWidth = pane1.LayoutRect.Width - 2;
							if( ctrlWidth - deltax < pane1.MinimumSize.Width )
							{
								ptdrag.Offset( pane1.MinimumSize.Width - ctrlWidth + deltax, 0);
							}
						}
					}
					
					if((ptclient.X + Width > pane2.LayoutRect.Left) && (ptclient.X + Width < pane2.LayoutRect.Right))
					{
						// Pane2 is being reduced in size. Check for min extents
						if( pane2.MinimumSize.Width != 0 )
						{
							int ctrlWidth = pane2.LayoutRect.Width - 2;
							if( ctrlWidth - deltax < pane2.MinimumSize.Width )
							{
								ptdrag.Offset( - pane2.MinimumSize.Width + ctrlWidth - deltax, 0);
							}
						}
					}
					return DragAxis.X;
				}
			}
			else	// Horizontal splitter
			{
                int m_Offset = 0;
                if ((ptclient.Y > pane1.LayoutRect.Top) && (ptclient.Y < pane1.LayoutRect.Bottom))
                {
                    // Pane1 is being reduced in size. set the offset value
                    try
                    {
                        if (pane1 is SizingController)
                        {
                            if ((pane1 as SizingController).GetDockControllers().Count > 1)
                            {
                                bool m_HasSizingController = false;
                                for (int i = 0; i < pane1.ChildControllers.Count; i++)
                                {
                                    if (pane1.ChildControllers[i] is SizingController)
                                    {
                                        m_HasSizingController = true;
                                        break;
                                    }
                                }
                                if (!m_HasSizingController)
                                {
                                    int m_MinHeight = SystemInformation.ToolWindowCaptionHeight;
                                    DockingManagerRenderer renderer = this.InternalController.DockingManager.Renderer;
                                    if (renderer != null && (this.InternalController.DockingManager.VisualStyle != VisualStyle.Default
                                        || !this.InternalController.DockingManager.ThemesEnabled))
                                        m_MinHeight = renderer.CaptionWidth + renderer.BorderWidth * 2 + 8; // 8 is for splitter width
                                    m_Offset = Math.Max(pane1.MinimumSize.Height, m_MinHeight);
                                }
                            }
                        }
                        else if (pane1 is DockTabController)
                        {
                            if ((pane1 as DockTabController).ChildControllers.Count > 1)
                            {
                                DockTabAlignmentStyle m_TabStyle = (pane1 as DockTabController).DockingManager.DockTabAlignment;
                                if (m_TabStyle == DockTabAlignmentStyle.Bottom || m_TabStyle == DockTabAlignmentStyle.Top)
                                {
                                    m_Offset = (pane1 as DockTabController).DockingManager.DockTabHeight + 2; // 2 is padding
                                }
                            }
                        }
                    }
                    catch { }
                }
            
				if( (ptclient.Y >= (pane1.LayoutRect.Top + (nCaptionHt + m_Offset))) &&
					(ptclient.Y < (pane2.LayoutRect.Bottom - nSplitOffLt)) )
				{
					// Check Pane1 and Pane2 for min/max control settings
					int deltay = Math.Abs(ptdelta.Y);

					// If either pane1 or pane2 is the MainFormController, then check with the docked controllers on the
					// adjancent borders for the min/max extents
					if(pane1.MainFormController == true)
					{
						if(ptclient.Y < pane2.LayoutRect.Top)	// Pane2 is being increased in size
						{
							if((pane1 as MainFormController).AllowSplitterSizing(pane2.ParentController, -deltay) == false)
								return DragAxis.None;
						}
						else if(ptclient.Y > pane2.LayoutRect.Top)	// Pane2 is being decreased in size
						{
							if((pane1 as MainFormController).AllowSplitterSizing(pane2.ParentController, deltay) == false)
								return DragAxis.None;
						}
					}
					else if(pane2.MainFormController == true)
					{
						if(ptclient.Y > pane1.LayoutRect.Bottom)	// Pane1 is being increased in size
						{
							if((pane2 as MainFormController).AllowSplitterSizing(pane1.ParentController, -deltay) == false)
								return DragAxis.None;
						}
						else if(ptclient.Y < pane1.LayoutRect.Bottom) // Pane1 is being decreased in size
						{
							if((pane2 as MainFormController).AllowSplitterSizing(pane1.ParentController, deltay) == false)
								return DragAxis.None;
						}
					}

					if((ptclient.Y > pane1.LayoutRect.Top) && (ptclient.Y < pane1.LayoutRect.Bottom))
					{
						// Pane1 is being reduced in size. Check for min extents
						if( pane1.MinimumSize.Height != 0 )
						{
							int ctrlHeight = pane1.LayoutRect.Height - (CaptionPainter.CaptionHeight + 5);
							if( ctrlHeight - deltay < pane1.MinimumSize.Height )
							{
								ptdrag.Offset(0, pane1.MinimumSize.Height - ctrlHeight + deltay);
							}
						}
					}
					if((ptclient.Y > pane2.LayoutRect.Top) && (ptclient.Y < pane2.LayoutRect.Bottom))
					{
						// Pane2 is being reduced in size. Check for min extents
						if( pane2.MinimumSize.Height != 0 ) 
						{
							int ctrlHeight = pane2.LayoutRect.Height - (CaptionPainter.CaptionHeight + 5);
							if( ctrlHeight - deltay < pane2.MinimumSize.Height )
							{
								ptdrag.Offset(0, - pane2.MinimumSize.Height + ctrlHeight - deltay);
							}
						}
					}
					return DragAxis.Y;
				}
			}

			return DragAxis.None;
		}

		public bool DrawHollow()
		{
			return false;
		}

		public void AbortDrag()
		{
			if(this.dcInternal.DockingManager.DragProvider.DraggingControl != this)
			{
				Debug.Assert(false, "Invalid call");
				return;
			}
			if(this.Capture == true)
				this.Capture = false;
			this.dcInternal.DockingManager.DragProvider.TerminateDrag(this, Point.Empty);

			// Forcibly update the designer state
			if(this.dcInternal.DockingManager.DesignMode == true)
				this.dcInternal.DockingManager.UpdateDesigner();
		}

		public bool QueryDragProceedWithDock()
		{
			return true;
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
		}

		public void HandleDoubleClick(Point ptscreen)
		{
			// No implementation
		}

		public void HandleMouseLeave()
		{
			// No implementation
		}

		public void InitiateFloatingResize(Point ptscreen, int nchittest)
		{
			Debug.Assert(false, "No implementation in DragSplitter.");
		}

		public bool GetDesignMode()
		{
			return this.dcInternal.DockingManager.DesignMode;
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);

            if ((this.dcInternal != null) && (this.dcInternal.DockingManager.DesignProcess == true) && (this.dcInternal.DockingManager.DesignMode == false)
                && (this.Cursor != Cursors.Default))
            {
                // Designtime contained environment. Set cursor to default
                this.Cursor = Cursors.Default;
            }
            else
                UpdateCursor();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if((this.dcInternal != null) && (this.dcInternal.DockingManager.DesignProcess == false))
				this.dcInternal.HandleMouseDownImp(e.Button, new Point(e.X, e.Y));
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			IDraggable idg = this.dcInternal.DockingManager.DragProvider.DraggingControl;
			if( (idg == null) || ((idg != null) && (idg == this)) )
				this.dcInternal.HandleMouseUpImp(e.Button, new Point(e.X, e.Y));
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if((this.dcInternal != null) && (this.dcInternal.DockingManager.DesignProcess == false))
			{
				IDraggable idg = this.dcInternal.DockingManager.DragProvider.DraggingControl;
				if( (idg == null) || ((idg != null) && (idg == this)) )
					this.dcInternal.HandleMouseMoveImp(e.Button, new Point(e.X, e.Y));
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if( dcInternal.DockingManager.Renderer.VisualStyle != VisualStyle.Default )
			{
				Orientation orientation = Orientation.Horizontal;
				if( dcInternal.DICurrent.DP == DockPreference.Horizontal )
					orientation = Orientation.Vertical;
				dcInternal.DockingManager.Renderer.PaintSplitter( e.Graphics, 
					ClientRectangle, orientation );	
			}
			base.OnPaint( e );
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);
			Invalidate();
		}

		protected internal virtual void OnSplitterMoving(SplitterEventArgs splitevent)
		{
			if(DragSplitterMoving != null)
				DragSplitterMoving(this, splitevent);
		}

		protected internal virtual void OnSplitterMoved(SplitterEventArgs splitevent)
		{
			if(DragSplitterMoved != null)
				DragSplitterMoved(this, splitevent);
		}

		protected override void Dispose(bool bdisposing)
		{
			if((bdisposing == true) && (this.dcInternal != null))
			{
				this.dcInternal = null;
			}
			base.Dispose(bdisposing);
		}
	}
}	