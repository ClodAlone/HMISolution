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
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Threading;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using Syncfusion.Windows.Forms.Localization;
using System.Windows.Forms.Design;
using Syncfusion.Runtime.InteropServices;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System.Windows.Forms.Design.Behavior;
#endif

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <exclude/>
	[Documentation.DocumentationExclude()]
	public class LMDesigner : ComponentDesigner
	{
		#region Constants
		private const int POINTER_WIDTH = 3;
		#endregion

		#region Overrides
		// Flag to avoid multiple Reloads
		//		private bool designerLoaded = false;
		// Capture the control over which the extender provider is dropped.
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);

			LayoutManager lmThis = (LayoutManager)component;

			IDesignerHost iDesignerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));

			lmThis.DesignerHost = iDesignerHost;

			Control containerControl = null;

			if(!iDesignerHost.Loading)
			{
				ISelectionService service = (ISelectionService)base.GetService(typeof(ISelectionService));

				// Set the selected control as the container control
				if(service != null)
				{
					ICollection c = service.GetSelectedComponents();
					if(c != null)
					{
						IEnumerator e = c.GetEnumerator();
						if(e != null)
						{
							// Will be false when Designer View gets drawn the first time.
							if(e.MoveNext())
							{
								IComponent comp = (IComponent)e.Current;
								if(comp != null && comp is Control)
								{
									Control control = (Control)comp;
									if(control != null && lmThis != null)
									{
										if(MessageBox.Show("Do you want to make " + control.Name + " the LayoutManager's ContainerControl?", "LayoutManager Designer", MessageBoxButtons.YesNo) == DialogResult.Yes)
											containerControl = control;
									}
								}
							}
						}
					}
				}
			}
			// Check to see if more than 1 layout manager set over the container.
			IExtenderListService extService =
				(IExtenderListService)base.GetService(typeof(IExtenderListService));
			if(extService != null && containerControl != null)
			{
				IExtenderProvider[] providers = extService.GetExtenderProviders();
				foreach(IExtenderProvider provider in providers)
				{
					LayoutManager lm = provider as LayoutManager;
					if(lm != null && lmThis != lm && lm.ContainerControl != null)
					{
						if(lm.ContainerControl == containerControl)
						{
							MessageBox.Show("Specified container is already bound to a Layout Manager. Try a different container.", "LayoutManager Designer Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							containerControl = null;
						}
					}
				}
			}
			if(!iDesignerHost.Loading && containerControl != null)
				lmThis.ContainerControl = containerControl;

			// Listen to Componenet added event
			IComponentChangeService iComponentChangeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
			if (iComponentChangeService != null)
			{
				iComponentChangeService.ComponentAdded += new ComponentEventHandler(this.Designer_ComponentAdded);
				iComponentChangeService.ComponentRemoved += new ComponentEventHandler(this.Designer_ComponentRemoved);
			}

			// Listen for any more changes to the ContainerControl property
			lmThis.ContainerControlChanged += new EventHandler(this.LM_ContainerControlChanged);

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			// Listen for Serialization Complete
			IDesignerSerializationManager manager = (IDesignerSerializationManager )GetService(typeof(IDesignerSerializationManager) );
			if(manager != null)
				manager.SerializationComplete += new EventHandler(this.Designer_SerializationComplete);
#endif
			Attach();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				IDesignerHost iDesignerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));
				if (iDesignerHost != null)
				{
					// Unsubscribe the Control.ParentChanged handler
					foreach(IComponent comp in iDesignerHost.Container.Components)
					{
						if((comp is Control) && ((comp is Form) == false))
							(comp as Control).ParentChanged -= new EventHandler(this.OnControlParentChanged);
					}
				}

				IComponentChangeService iComponentChangeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
				if (iComponentChangeService != null)
				{
					iComponentChangeService.ComponentAdded -= new ComponentEventHandler(this.Designer_ComponentAdded);
					iComponentChangeService.ComponentRemoved -= new ComponentEventHandler(this.Designer_ComponentRemoved);
				}
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				IDesignerSerializationManager manager = (IDesignerSerializationManager )GetService(typeof(IDesignerSerializationManager) );
				if(manager != null)
					manager.SerializationComplete -= new EventHandler(this.Designer_SerializationComplete);
#endif
				Detach();
			}
			base.Dispose(disposing);
		}
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		private Control InsertBefore
		{
			get
			{
				Control insertBefore = null;

				if (m_insertIndex >= 0)
				{
					Control control = this.Control;
					if (control != null)
					{
						IList controls = control.Controls;

						int index = m_insertIndex;

						for (int i = 0; i < controls.Count; i++)
						{
							if (!this.DragComponents.Contains(controls[i]))
							{
								if (i == index)
								{
									insertBefore = controls[i] as Control;
									break;
								}
							}
							else index++;
						}
					}
				}

				return insertBefore;
			}
		}
		/// <summary>
		/// Managed control
		/// </summary>
		private Control Control
		{
			get
			{
				LayoutManager lm = this.Component as LayoutManager;
				if (lm != null)
				{
					return lm.ContainerControl;
				}
				return null;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private bool IsRightToLeft
		{
			get
			{
				Control c = this.Control;
				if (c != null)
				{
					return (c.RightToLeft == RightToLeft.Yes);
				}
				return false;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private bool IsVerticalLayout
		{
			get
			{
				FlowLayout fl = this.Component as FlowLayout;
				if (fl != null)
				{
					return fl.LayoutMode == FlowLayoutMode.Vertical;
				}
				return false;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private ArrayList DragComponents
		{
			get
			{
				ArrayList componets = new ArrayList();

				ISelectionService selSvc = GetService(typeof(ISelectionService)) as ISelectionService;

				if(selSvc!=null)
				{
					componets.AddRange(selSvc.GetSelectedComponents());
				}

				return componets;
			}
		}
		#endregion

		#region Event handlers
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		// This will reset ExtenderProvider dependencies.
		private void Designer_SerializationComplete(object sender, EventArgs e)
		{
			//			this.designerLoaded = true;
			// This call to AddExtenderProvider is necessary to FORCE the designer to call CanExtend for ALL the Controls.
			// This is necessary because any calls to CanExtend before SerializationComplete might not have the corresponding Control parented yet.
			IExtenderProviderService eps = (IExtenderProviderService)GetService(typeof(IExtenderProviderService));
			// When this is called by ReloadDesigner Controls that have been already
			// extended end up getting a duplicate property.
			eps.AddExtenderProvider((IExtenderProvider)this.Component);
		}
#endif

		private void LM_ContainerControlChanged(object sender, EventArgs e)
		{
			IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
			foreach(IComponent comp in idh.Container.Components)
			{
				if((comp is Control) && ((comp is Form) == false))
				{
					Control ctrl = comp as Control;
					TypeDescriptor.Refresh(ctrl);
				}
			}
		}
		public void OnControlParentChanged(object sender, EventArgs e)
		{
			Control ctrl = sender as Control;
			TypeDescriptor.Refresh(ctrl);
		}
		private void Designer_ComponentAdded(object sender, ComponentEventArgs e)
		{
			if((e.Component is Control) && ((e.Component is Form)==false))
			{
				(e.Component as Control).ParentChanged += new EventHandler(this.OnControlParentChanged);
			}
			else if(e.Component == this.Component)
			{
				// For all existing controls, subscribe to the Control.ParentChanged handler
				IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
				foreach(IComponent comp in idh.Container.Components)
				{
					if((comp is Control) && ((comp is Form) == false))
					{
						Control ctrl = comp as Control;
						ctrl.ParentChanged += new EventHandler(this.OnControlParentChanged);
					}
				}
			}
		}
		private void Designer_ComponentRemoved(object sender, ComponentEventArgs e)
		{
			if((e.Component is Control) && ((e.Component is Form)==false))
				(e.Component as Control).ParentChanged -= new EventHandler(this.OnControlParentChanged);
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pt"></param>
		/// <param name="c"></param>
		/// <returns></returns>
		private bool GetIsInsertBefore(Point pt, Control c)
		{
			bool bResult = false;

			if (this.IsRightToLeft)
			{
				if (this.IsVerticalLayout)
				{
					bResult = (pt.X >= c.Left) && (pt.X > c.Right || pt.Y <= c.Bottom);
				}
				else
				{
					bResult = (pt.Y <= c.Bottom) && (pt.Y < c.Top || pt.X >= c.Left);
				}
			}
			else
			{
				if (this.IsVerticalLayout)
				{
					bResult = (pt.X <= c.Right) && (pt.X < c.Left || pt.Y <= c.Bottom);
				}
				else
				{
					bResult = (pt.Y <= c.Bottom) && (pt.Y < c.Top || pt.X <= c.Right);
				}
			}
			return bResult;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ptMouse"></param>
		private void SetInsertIndex(Point ptMouse)
		{
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			int index = GetInsertIndex(ptMouse);
			if (m_insertIndex != index)
			{
				DrawPointer();

				m_insertIndex = index;
				SetPointerRect();

				DrawPointer();
			}
#else
			int index = GetInsertIndex(ptMouse);
			if (m_insertIndex != index)
			{
				InvalidatePointer();

				m_insertIndex = index;

				SetPointerRect();
			}
			DrawPointer();
#endif
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ptMouse"></param>
		/// <returns></returns>
		private int GetInsertIndex(Point ptMouse)
		{
			int index = -1;

			Control control = this.Control;
			if (control != null)
			{
				Point pt = control.PointToClient(ptMouse);
				if (control.ClientRectangle.Contains(pt))
				{
					for (int i = 0, d = 0; i < control.Controls.Count; i++)
					{
						Control c = control.Controls[i];
						if (!this.DragComponents.Contains(c))
						{
							if( GetIsInsertBefore(pt, c) )
							{
								index = i - d;
								break;
							}
						}
						else d++;
					}
				}
			}
			return index;
		}
		/// <summary>
		/// Updates bounds of the instertion pointer
		/// </summary>
		private void SetPointerRect()
		{
			m_rcPointer = Rectangle.Empty;

			Control insertBefore = this.InsertBefore;
			if (insertBefore != null)
			{
				Rectangle rc = insertBefore.Bounds;

				m_rcPointer.X = this.IsRightToLeft ? rc.Right : rc.X - POINTER_WIDTH;
				m_rcPointer.Y = rc.Y - 1;
				m_rcPointer.Width = POINTER_WIDTH;
				m_rcPointer.Height = rc.Height + 2;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void InvalidatePointer()
		{
			if (!m_rcPointer.IsEmpty)
			{
				Control control = this.Control;
				if (control != null)
				{
					control.Invalidate(m_rcPointer, true);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void DrawPointer()
		{
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			if (!m_rcPointer.IsEmpty)
			{
				Control c = this.Control;
				if(c!=null)
				{
					Color clr = c.BackColor;
					Rectangle rc = c.RectangleToScreen(m_rcPointer);

					ControlPaint.DrawReversibleLine(new Point(rc.X, rc.Y), new Point(rc.Right, rc.Y),clr);
					ControlPaint.DrawReversibleLine(new Point(rc.X+1, rc.Y+1), new Point(rc.X+1, rc.Bottom),clr);
					ControlPaint.DrawReversibleLine(new Point(rc.X, rc.Bottom), new Point(rc.Right, rc.Bottom),clr);
				}
			}
#else
			if (!m_rcPointer.IsEmpty)
			{
				Control c = this.Control;
				if (c != null)
				{
					IntPtr hWnd = c.Handle;

					IntPtr hdc = NativeMethods.GetDCEx(hWnd, IntPtr.Zero, NativeMethods.DCX_CACHE);
					if (hdc != IntPtr.Zero)
					{
						using (Graphics g = Graphics.FromHdc(hdc))
						{
							Pen p = c.BackColor.GetBrightness() > 0.5f ? Pens.Black : Pens.White;
							
							g.DrawLine(p, m_rcPointer.X, m_rcPointer.Y, m_rcPointer.Right-1, m_rcPointer.Y);
							g.DrawLine(p, m_rcPointer.X + 1, m_rcPointer.Y, m_rcPointer.X + 1, m_rcPointer.Bottom - 1);
							g.DrawLine(p, m_rcPointer.X, m_rcPointer.Bottom - 1, m_rcPointer.Right - 1, m_rcPointer.Bottom - 1);
						}
						NativeMethods.ReleaseDC(hWnd, hdc);
					}
				}
			}
#endif
		}
		/// <summary>
		/// 
		/// </summary>
		private void InsertControls()
		{
			Control control = this.Control;
			
			if (control != null)
			{
				control.SuspendLayout();

				Control.ControlCollection controls = control.Controls;

				Control insertBefore = this.InsertBefore;

				ArrayList dragComponents = this.DragComponents;

				if (insertBefore != null )
				{
					for (int i = dragComponents.Count - 1; i >= 0; i--)
					{
						Control c = dragComponents[i] as Control;
						
						if (c != null)
						{
							int oldIndex = controls.IndexOf(c);
							
							if (oldIndex >= 0)
							{
								int newIndex = controls.IndexOf(insertBefore);

								controls.SetChildIndex(c, oldIndex < newIndex ? newIndex - 1 : newIndex);

								insertBefore = c;
							}
						}
					}
				}
				else
				{
					for (int i = 0; i < dragComponents.Count; i++)
					{
						controls.Add(dragComponents[i] as Control);
					}
				} 

				control.ResumeLayout();

				// Refresh selections
				ISelectionService selSvc = GetService(typeof(ISelectionService)) as ISelectionService;
				if (selSvc != null)
				{
					selSvc.SetSelectedComponents(new object[] {}, SelectionTypes.Replace);
					selSvc.SetSelectedComponents(dragComponents);
				}
			}
		}
		#endregion

		#region Fields

		Point m_lastMouse = Point.Empty;
		Rectangle m_rcPointer = Rectangle.Empty;

		int m_insertIndex;
		
		#endregion

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		void Attach()
		{
			LayoutManager lm = this.Component as LayoutManager;
			if(lm!=null)
			{
				lm.ContainerControlChanged+=new EventHandler(OnContainerControlChanged);
				AttachControl();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		void Detach()
		{
			LayoutManager lm = this.Component as LayoutManager;
			if(lm!=null)
			{
				lm.ContainerControlChanged-=new EventHandler(OnContainerControlChanged);
				DetachControl();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		void AttachControl()
		{
			Control c = this.Control;
			if(c!=null)
			{
				c.DragEnter+=new DragEventHandler(OnControlDragEnter);
				c.DragOver+=new DragEventHandler(OnControlDragOver);
				c.DragDrop+=new DragEventHandler(OnControlDragDrop);
				c.DragLeave+=new EventHandler(OnControlDragLeave);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		void DetachControl()
		{
			Control c = this.Control;
			if(c!=null)
			{
				c.DragEnter-=new DragEventHandler(OnControlDragEnter);
				c.DragOver-=new DragEventHandler(OnControlDragOver);
				c.DragDrop-=new DragEventHandler(OnControlDragDrop);
				c.DragLeave-=new EventHandler(OnControlDragLeave);
			}
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnContainerControlChanged(object sender, EventArgs e)
		{
			AttachControl();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnControlDragEnter(object sender, DragEventArgs e)
		{
			m_rcPointer = Rectangle.Empty;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnControlDragOver(object sender, DragEventArgs e)
		{
			if (m_lastMouse.X != e.X || m_lastMouse.Y != e.Y)
			{
				m_lastMouse = new Point(e.X, e.Y);
				SetInsertIndex(m_lastMouse);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnControlDragDrop(object sender, DragEventArgs e)
		{
			DrawPointer();

			if( (e.AllowedEffect & (DragDropEffects.Copy|DragDropEffects.Move))!=DragDropEffects.None )
			{
				InsertControls();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnControlDragLeave(object sender, EventArgs e)
		{
			if(!m_lastMouse.IsEmpty)
			{
				m_lastMouse = Point.Empty;
				SetInsertIndex(m_lastMouse);
			}
		}
		#endregion
#else
		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		void Attach()
		{
			m_behaviorSvc = GetService(typeof(BehaviorService)) as BehaviorService;
			if (m_behaviorSvc != null)
			{
				m_behavior = new LayoutManagerBehavior(m_behaviorSvc);

				m_behaviorSvc.BeginDrag += new BehaviorDragDropEventHandler(OnBehaviorSvcBeginDrag);
				m_behaviorSvc.EndDrag += new BehaviorDragDropEventHandler(OnBehaviorSvcEndDrag);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		void Detach()
		{
			if (m_behaviorSvc != null)
			{
				m_behaviorSvc.BeginDrag -= new BehaviorDragDropEventHandler(OnBehaviorSvcBeginDrag);
				m_behaviorSvc.EndDrag -= new BehaviorDragDropEventHandler(OnBehaviorSvcEndDrag);

				m_behavior = null;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="glyph"></param>
		/// <returns></returns>
		bool IsControlGlyph(ComponentGlyph glyph)
		{
			bool bResult = false;

			if (glyph != null)
			{
				Control c = this.Control;

				bResult = (c != null && c == glyph.RelatedComponent);
			}
			
			return bResult;
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnBehaviorSvcBeginDrag(object sender, BehaviorDragDropEventArgs e)
		{
			this.DragComponents.Clear();
			this.DragComponents.AddRange(e.DragComponents);

			m_behavior.DragOver += new DragEventHandler(OnBehaviorDragOver);
			m_behavior.DragDrop += new DragEventHandler(OnBehaviorDragDrop);

			m_behaviorSvc.PushBehavior(m_behavior);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnBehaviorSvcEndDrag(object sender, BehaviorDragDropEventArgs e)
		{
			this.DragComponents.Clear();

			m_behavior.DragOver -= new DragEventHandler(OnBehaviorDragOver);
			m_behavior.DragDrop -= new DragEventHandler(OnBehaviorDragDrop);

			m_behaviorSvc.PopBehavior(m_behavior);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnBehaviorDragOver(object sender, DragEventArgs e)
		{
			if (m_lastMouse.X != e.X || m_lastMouse.Y != e.Y)
			{
				Point pt = Point.Empty;

				m_lastMouse = new Point(e.X, e.Y);

				if( IsControlGlyph(sender as ComponentGlyph))
				{
					pt = m_lastMouse;
				}
				
				SetInsertIndex(pt);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnBehaviorDragDrop(object sender, DragEventArgs e)
		{
			if( (e.AllowedEffect & (DragDropEffects.Copy|DragDropEffects.Move))!=DragDropEffects.None )
			{
				if( IsControlGlyph(sender as ComponentGlyph))
				{
					InsertControls();
				}
			}
		}
		#endregion

		#region Fields
		BehaviorService m_behaviorSvc;
		LayoutManagerBehavior m_behavior;
		#endregion

		#region LayoutManagerBehavior
		class LayoutManagerBehavior : Behavior
		{
			#region Constructor
			/// <summary>
			/// 
			/// </summary>
			/// <param name="behaviorSvc"></param>
			public LayoutManagerBehavior(BehaviorService behaviorSvc)
				: base(true, behaviorSvc)
			{
			}
			#endregion

			#region Overrides
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="e"></param>
			public override void OnDragOver(Glyph g, DragEventArgs e)
			{
				if (this.DragOver != null)
				{
					this.DragOver(g, e);
				}

				base.OnDragOver(g, e);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="g"></param>
			/// <param name="e"></param>
			public override void OnDragDrop(Glyph g, DragEventArgs e)
			{
				base.OnDragDrop(g, e);

				if (this.DragDrop != null)
				{
					this.DragDrop(g, e);
				}
			}
			#endregion

			#region Events
			/// <summary>
			/// 
			/// </summary>
			public event DragEventHandler DragOver;
			/// <summary>
			/// 
			/// </summary>
			public event DragEventHandler DragDrop;
			#endregion
		}
		#endregion
#endif
	}

	[Documentation.DocumentationExclude()]
	public class CardLayoutDesigner : LMDesigner
	{
		private DesignerVerbCollection verbs;

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);

			if(component is ISupportInitialize)
			{
				((ISupportInitialize)component).BeginInit();
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				IDesignerSerializationManager manager = (IDesignerSerializationManager )GetService(typeof(IDesignerSerializationManager) );
				if(manager != null)
					manager.SerializationComplete += new EventHandler(this.Designer_SerializationComplete);
#else
                // Trying to get VS main form, all exceptions will be suppressed.
                this.Designer_SerializationComplete(null, EventArgs.Empty); // Added 
#endif
			}
		}

		private void Designer_SerializationComplete(object sender, EventArgs e)
		{
			if(this.Component is ISupportInitialize)
			{
				((ISupportInitialize)this.Component).EndInit();
			}

			IDesignerSerializationManager manager = (IDesignerSerializationManager )GetService(typeof(IDesignerSerializationManager) );
			if(manager != null)
				manager.SerializationComplete -= new EventHandler(this.Designer_SerializationComplete);
		}

		public override DesignerVerbCollection Verbs
		{
			get
			{
				if (this.verbs == null)
				{
					this.verbs = new DesignerVerbCollection();
					this.verbs.Add(new DesignerVerb("First Card",new EventHandler(this.OnFirstPage)));
					this.verbs.Add(new DesignerVerb("Last Card",new EventHandler(this.OnLastPage)));
					this.verbs.Add(new DesignerVerb("Next Card",new EventHandler(this.OnNextPage)));
					this.verbs.Add(new DesignerVerb("Previous Card",new EventHandler(this.OnPreviousPage)));
				}
				return this.verbs;
			}
		}

		private void OnFirstPage(object sender, EventArgs e)
		{
			CardLayout cl = this.Component as CardLayout;
			if(cl != null)
				cl.First();
		}

		private void OnLastPage(object sender, EventArgs e)
		{
			CardLayout cl = this.Component as CardLayout;
			if(cl != null)
				cl.Last();
		}

		private void OnPreviousPage(object sender, EventArgs e)
		{
			CardLayout cl = this.Component as CardLayout;
			if(cl != null)
				cl.Previous();
		}

		private void OnNextPage(object sender, EventArgs e)
		{
			CardLayout cl = this.Component as CardLayout;
			if(cl != null)
				cl.Next();
		}
	}
}