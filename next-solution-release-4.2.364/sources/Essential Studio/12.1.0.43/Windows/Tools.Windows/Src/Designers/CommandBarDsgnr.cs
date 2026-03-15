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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
#if SyncfusionFramework2_0
using System.Windows.Forms.Design.Behavior;
#endif

using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <exclude/>
	public class CommandBarsCollectionEditor : CollectionEditor
	{
		public CommandBarsCollectionEditor(Type type) : base(type)
		{
		}

		protected override void DestroyInstance(object instance)
		{
			CommandBar cbar = instance as CommandBar;
			if(cbar.Parent != null)	// Visible docked or floating CommandBars
			{
				if(cbar.Floating == true)
				{
					CommandBarForm cbarform = cbar.Parent as CommandBarForm;
					cbarform.Visible = false;
					cbarform.Controls.Remove(cbar);
					cbarform.Close();
				}
				else
				{
					ICommandBarDesignerInvoke icbinvoke = cbar as ICommandBarDesignerInvoke;
					CommandDockBar cdbparent = icbinvoke.GetCommandDockBarParent();
					Trace.Assert(cdbparent != null);
					if(icbinvoke.CommandBarBaseVisibility == true)
					{
						icbinvoke.CommandBarBaseVisibility = false;
						cdbparent.RemoveCommandBar(cbar);
					}
					else
					{
						cdbparent.Controls.Remove(cbar);
					}
				}
			}

			base.DestroyInstance(instance);
		}
	}

	public interface IProvideController
	{
		CommandBarController Controller{get;}
	}
    /// <exclude/>
	public sealed class CBControllerDesigner : ComponentDesigner, IProvideController, IGetMsgProcListener, ICallWndProcListener
	{
		private IntPtr VSMainForm = IntPtr.Zero;
		private static Control LButtonDownControl = null;
		private static Point LButtonDownPoint = Point.Empty;
		private static bool bKeyBdEvent = false;

		private DesignerVerb dvAddCommandBar = null;
		private DesignerVerb dvAddControlBar = null;
		private DesignerVerb dvRemoveCommandBar = null;
		private DesignerVerbCollection dvcVerbs = null;
		internal CommandBarController cbController = null;
		
		private CommandDockBarSerializationProvider cbSerProvider = null;

		public override DesignerVerbCollection Verbs
		{
			get
			{
				if( this.cbController.GetCommandBarsList().Count > 0 )
					this.dvRemoveCommandBar.Enabled = true;
				else
					this.dvRemoveCommandBar.Enabled = false;
				return this.dvcVerbs;
			}
		}

		public CommandBarController Controller
		{
			get{return this.Component as CommandBarController;}
		}

		public CBControllerDesigner()
		{

		}

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);

			IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
			ComponentCollection clln = idh.Container.Components;
			foreach(IComponent obj in clln)
			{
				// If an instance of the CommandBarController already exists for this designerhost,
				// then throw an exception.
				if( (obj is CommandBarController) && (obj != component) )
					throw new ApplicationException("Only one instance of the CommandBarController can exist on a form.");
			}

			this.cbController = component as CommandBarController;
			Form hostform = idh.RootComponent as Form;
			if(hostform == null)
				throw( new ApplicationException("A CommandBar host must be a top-level form."));
			this.cbController.HostForm = hostform;
			if(idh.Loading == false)
			{
				ICBControllerDesignerInvoke icbcd = this.cbController as ICBControllerDesignerInvoke;
				icbcd.InitializeCBController();
			}

			this.dvAddCommandBar = new DesignerVerb("Add CommandBar", new EventHandler(this.OnAddCommandBar));
			this.dvAddControlBar = new DesignerVerb("Add ControlBar", new EventHandler(this.OnAddControlBar));
			this.dvRemoveCommandBar = new DesignerVerb("Remove CommandBar", new EventHandler(this.OnRemoveCommandBar));
			this.dvRemoveCommandBar.Enabled = false;
			DesignerVerb[] dvarray = new DesignerVerb[] { this.dvAddCommandBar, this.dvAddControlBar, this.dvRemoveCommandBar };
			this.dvcVerbs = new DesignerVerbCollection(dvarray);

			IComponentChangeService iccs = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			iccs.ComponentChanged += new ComponentChangedEventHandler(this.IComponentChangeService_ComponentChanged);
			iccs.ComponentRemoved += new ComponentEventHandler(this.IComponentChangeService_ComponentRemoved);

			idh.Activated += new System.EventHandler(this.IDesignerHost_Activated);
			idh.Deactivated += new System.EventHandler(this.IDesignerHost_Deactivated);
			idh.LoadComplete += new System.EventHandler(this.IDesignerHost_LoadComplete);

			// Custom Serialization provider
			this.cbSerProvider = new CommandDockBarSerializationProvider();
			IDesignerSerializationManager idsm = (IDesignerSerializationManager)this.GetService(typeof(IDesignerSerializationManager));
			if(idsm != null)
				idsm.AddSerializationProvider(this.cbSerProvider);

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			IDesignerSerializationManager serManager
				= (IDesignerSerializationManager)this.GetService(typeof(IDesignerSerializationManager));
			// This is possible when within WinRes, for example.
			if(serManager != null)
				serManager.SerializationComplete += new EventHandler(this.OnSerializationComplete);
#else
			// Trying to get VS main form, all exceptions will be suppressed.
			OnSerializationComplete( null, EventArgs.Empty ); // Added 
#endif

			// Calling GetAncestor seems to break the designer sometimes (when opening a derived form
			// a tab control in VI mode doesn't respond to mouse down for tab switching!)
			// So, moving this to OnSerializationComplete.
			// this.VSMainForm = Syncfusion.Runtime.InteropServices.NativeMethods.GetAncestor(this.cbController.HostForm.Handle, 3);

			DesignerHooks.AddGetMsgProcListener(this);
			DesignerHooks.AddCallWndProcListener(this);
		}

		private void OnSerializationComplete(object sender, EventArgs args)
		{
			try
			{
				this.VSMainForm = Syncfusion.Runtime.InteropServices.NativeMethods.GetAncestor( this.cbController.HostForm.Handle, 3 );
			}
			catch( Exception e )
			{
				Debug.WriteLine( "Can not get VSMainForm. Exception: " + e.Message );
			}
		}

		public void ToggleFloatingFormVisibility(bool bvisible)
		{
			foreach(CommandBar cbar in this.cbController.CommandBars)
			{
				if(cbar.DockState == CommandBarDockState.Float)
				{
					if( cbar.Parent != null )
					{
						if( bvisible == true )
						{
							Syncfusion.Runtime.InteropServices.NativeMethods.ShowWindow( cbar.Parent.Handle, 4/*SW_SHOWNOACTIVATE*/);
							cbar.Parent.Visible = bvisible;
						}
						else
							cbar.Parent.Visible = false;
					}
				}
			}
		}

		// IMessageProcListener implementation
		public void GetMsgProc(int nCode, IntPtr wparam, IntPtr lparam)
		{
			Message msg = (Message)(Marshal.PtrToStructure(lparam, typeof(Message)));
			if( (msg.Msg == 0x0201 /*WM_LBUTTONDOWN*/) || (msg.Msg == 0x0203 /*WM_LBUTTONDBLCLK*/)
				|| (msg.Msg == 0x0200 /*WM_MOUSEMOVE*/) || (msg.Msg == 0x0202 /*WM_LBUTTONUP*/)	)
			{
				Control ctrlhit = Control.FromHandle(msg.HWnd);
				if(ctrlhit != null)
				{
					ICommandBarDesignerMouseHook idmh = ctrlhit as ICommandBarDesignerMouseHook;
					if(idmh != null)
					{
						Point ptscreen = Cursor.Position;
						if(msg.Msg == 0x0200 /*WM_MOUSEMOVE*/)
						{
							if(msg.WParam == (IntPtr)0x0001 /*MK_LBUTTON*/)
							{
								// A drag is about to commence. Synthesize the 'ESC' keyboard event to preempt designer band drawing.
								if(
									(CBControllerDesigner.LButtonDownControl != null) &&
									( ((ptscreen.X >= CBControllerDesigner.LButtonDownPoint.X-1) && (ptscreen.Y >= CBControllerDesigner.LButtonDownPoint.Y-1) &&
									(ptscreen.X <= CBControllerDesigner.LButtonDownPoint.X+1) && (ptscreen.Y <= CBControllerDesigner.LButtonDownPoint.Y+1))
									|| (CBControllerDesigner.bKeyBdEvent == true) )
									)
								{
									Syncfusion.Runtime.InteropServices.NativeMethods.keybd_event((byte)0x1B /*VK_ESCAPE*/, (byte)0, 0, IntPtr.Zero);
									Syncfusion.Runtime.InteropServices.NativeMethods.keybd_event((byte)0x1B /*VK_ESCAPE*/, (byte)0, 0x0002/*KEYEVENTF_KEYUP*/, IntPtr.Zero);
									if(CBControllerDesigner.bKeyBdEvent == true)
										CBControllerDesigner.bKeyBdEvent = false;
								}
								idmh.HandleMouseMove(MouseButtons.Left, ptscreen);
							}
							else
							{
								idmh.HandleMouseMove(MouseButtons.None, ptscreen);
							}
						}
						else if((msg.Msg == 0x0201 /*WM_LBUTTONDOWN*/) && (CBControllerDesigner.bKeyBdEvent == false))
						{
							idmh.HandleMouseDown(MouseButtons.Left, ptscreen);
							CBControllerDesigner.LButtonDownControl = ctrlhit;
							CBControllerDesigner.LButtonDownPoint = ptscreen;
							CBControllerDesigner.bKeyBdEvent = true;
							if(Syncfusion.Runtime.InteropServices.NativeMethods.GetCapture() != ctrlhit.Handle)
								Syncfusion.Runtime.InteropServices.NativeMethods.SetCapture(ctrlhit.Handle);
						}
						else if(msg.Msg == 0x0202 /*WM_LBUTTONUP*/)
						{
							idmh.HandleMouseUp(MouseButtons.Left, ptscreen);
							if(CBControllerDesigner.LButtonDownControl != null)
							{
								CBControllerDesigner.LButtonDownControl = null;
								CBControllerDesigner.LButtonDownPoint = Point.Empty;
								CBControllerDesigner.bKeyBdEvent = false;
								Syncfusion.Runtime.InteropServices.NativeMethods.ReleaseCapture();
							}
						}
						else if(msg.Msg == 0x0203 /*WM_LBUTTONDBLCLK*/)
						{
							idmh.HandleDoubleClick(ptscreen);
						}
					}
				}
			}
		}

		public void CallWndProc(int nCode, IntPtr wparam, IntPtr lparam)
		{
			Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT cwp =
				(Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT)(Marshal.PtrToStructure(lparam, typeof(Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT)));
			if(nCode >= 0)
			{
				if((cwp.message == 0x0215 /*WM_CAPTURECHANGED*/) && (CBControllerDesigner.LButtonDownControl != null))
				{
					if(cwp.hwnd == CBControllerDesigner.LButtonDownControl.Handle)
						Syncfusion.Runtime.InteropServices.NativeMethods.SetCapture(CBControllerDesigner.LButtonDownControl.Handle);
				}

				if((cwp.message == 0x0018/*WM_SHOWWINDOW*/) || (cwp.message == 0x001C/*WM_ACTIVATEAPP*/))
				{
					if(cwp.hwnd == this.VSMainForm)
					{
						if(cwp.wParam.ToInt32() == 1)
							this.ToggleFloatingFormVisibility(true);
						else
							this.ToggleFloatingFormVisibility(false);
					}
				}
			}
		}

		public void OnAddCommandBar(object sender, EventArgs e)
		{
			IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
			CommandBarController.CommandBarsCollection cbarlistold = this.cbController.CommandBars;
			CommandBar cbar = (CommandBar)idh.CreateComponent(typeof(CommandBar));
			cbar.Text = TypeDescriptor.GetComponentName(cbar);
			this.cbController.CommandBars.Add(cbar);

			// Raise the component changed event, so that the bar collection can be repersisted.
			this.RaiseComponentChanged(TypeDescriptor.GetProperties(this.cbController)["CommandBars"], cbarlistold, this.cbController.CommandBars);
		}

		public void OnAddControlBar(object sender, EventArgs e)
		{
			IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
			CommandBarController.CommandBarsCollection cbarlistold = this.cbController.CommandBars;
			ControlBar cbar = (ControlBar)idh.CreateComponent(typeof(ControlBar));
			cbar.Text = TypeDescriptor.GetComponentName(cbar);
			this.cbController.CommandBars.Add(cbar);

			// Raise the component changed event, so that the bar collection can be repersisted.
			this.RaiseComponentChanged(TypeDescriptor.GetProperties(this.cbController)["CommandBars"], cbarlistold, this.cbController.CommandBars);

		}

		public void OnRemoveCommandBar(object sender, EventArgs e)
		{
			IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
			CommandBarController.CommandBarsCollection cbarlistold = this.cbController.CommandBars;
			if(cbarlistold.Count > 0)
			{
				CommandBar cbar = cbarlistold[cbarlistold.Count-1];
				this.cbController.CommandBars.Remove(cbar);
				idh.DestroyComponent(cbar);

				this.RaiseComponentChanged(TypeDescriptor.GetProperties(this.cbController)["CommandBars"], cbarlistold, this.cbController.CommandBars);
			}
		}

		private void IComponentChangeService_ComponentChanged(object sender, ComponentChangedEventArgs e)
		{
			if(this.dvcVerbs != null && this.dvcVerbs.Count > 0)
			{
				if(this.cbController.CommandBars.Count > 0)
					this.dvRemoveCommandBar.Enabled = true;
				else
					this.dvRemoveCommandBar.Enabled = false;
			}
		}

		private void IComponentChangeService_ComponentRemoved(Object sender, ComponentEventArgs e)
		{
			if(e.Component is CommandBar)
			{
				CommandBar cbar = e.Component as CommandBar;
				if(this.cbController.CommandBars.Contains(cbar) == true)
				{
					if(cbar.Floating == true)
					{
						if(cbar.Parent != null)	//cbar.cdbParent
						{
							CommandBarForm cbarform = cbar.Parent as CommandBarForm;
							cbarform.Visible = false;
							cbarform.Controls.Remove(cbar);
							// Do a PostMessage to the floating form and in that msg handler reset designer selection
							// to the main form so that the selectionservice is not left hanging with an invalid reference
							// to the floating form. Dispose off the floating form after setting the selection.
							// This needs to be done in a PostMessage as doing it here fails to set selection to the form
							// and causes a whole bunch of related problems.
							Syncfusion.Runtime.InteropServices.NativeMethods.PostMessage(cbarform.Handle, (int)Syncfusion.Runtime.InteropServices.NativeMethods.WM_CBAR_SELANDDISP,
								IntPtr.Zero, IntPtr.Zero);
						}
					}
					this.cbController.CommandBars.Remove(cbar);
				}
			}
		}

		private void IDesignerHost_Activated(Object sender, EventArgs e)
		{
			// Show all floating forms
			ToggleFloatingFormVisibility(true);

			DesignerHooks.AddGetMsgProcListener(this);
			DesignerHooks.AddCallWndProcListener(this);
		}

		private void IDesignerHost_Deactivated(Object sender, EventArgs e)
		{
			// Hide all floating forms
			ToggleFloatingFormVisibility(false);

			DesignerHooks.RemoveGetMsgProcListener(this);
			DesignerHooks.RemoveCallWndProcListener(this);
		}

		private void IDesignerHost_LoadComplete(Object sender, EventArgs e)
		{
			this.cbController.ResetDockBarZOrder();
		}

		protected override void Dispose(bool disposing)
		{
			DesignerHooks.RemoveGetMsgProcListener(this);
			DesignerHooks.RemoveCallWndProcListener(this);

			this.VSMainForm = IntPtr.Zero;

			IDesignerSerializationManager idsm = (IDesignerSerializationManager)this.GetService(typeof(IDesignerSerializationManager));
			if(idsm != null)
			{
				idsm.RemoveSerializationProvider(this.cbSerProvider);
			}

			IComponentChangeService iccs = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(iccs != null)
			{
				iccs.ComponentChanged -= new ComponentChangedEventHandler(this.IComponentChangeService_ComponentChanged);
				iccs.ComponentRemoved -= new ComponentEventHandler(this.IComponentChangeService_ComponentRemoved);
			}

			IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(idh != null)
			{
				idh.Activated -= new System.EventHandler(this.IDesignerHost_Activated);
				idh.Deactivated -= new System.EventHandler(this.IDesignerHost_Deactivated);
				idh.LoadComplete -= new System.EventHandler(this.IDesignerHost_LoadComplete);
			}

			base.Dispose(disposing);
		}
	}

    /// <exclude/>
	/// <summary>
	/// Summary description for CommandBar Designer.
	/// </summary>
	public sealed class CommandBarDesigner : ParentControlDesigner, ICommandBarDesignerComponentInvoke
	{
		private bool iamSelected = false;
		protected override bool DrawGrid
		{
			get { return false; }
			set {;}
		}

		protected override bool EnableDragRect
		{
			get { return false; }
		}
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		public override SelectionRules SelectionRules
		{
			get 
			{
				return SelectionRules.Visible;
			}
		}
#endif

		public CommandBarDesigner()
		{
		}

		public override /*ParentControlDesigner*/ void Initialize(IComponent component)
		{
			base.Initialize(component);
			ISelectionService iSelectionService = (ISelectionService)this.GetService(typeof(ISelectionService));
			if (iSelectionService != null)
				iSelectionService.SelectionChanged += new EventHandler(this.OnSelectionChanged);
		}
		private void OnSelectionChanged(object sender, EventArgs e)
		{
			bool oldSelectedState = this.iamSelected;
			// To find out the current selection (can be more than 1) do this:
			System.ComponentModel.Design.ISelectionService iSelectionService;
			System.Collections.ICollection selectedComponents;

			this.iamSelected = false;
			iSelectionService = (ISelectionService)this.GetService(typeof(ISelectionService));

			if (iSelectionService != null)
			{
				selectedComponents = iSelectionService.GetSelectedComponents();
				foreach(object selectedComponent in selectedComponents)
				{
					if(selectedComponent == this.Component)
						this.iamSelected = true;
				}
			}
			if(iamSelected != oldSelectedState)
				this.Control.Invalidate();
 		}


		protected override void Dispose(bool disposing)
		{
			ISelectionService iSelectionService = (ISelectionService)this.GetService(typeof(ISelectionService));
			if (iSelectionService != null)
				iSelectionService.SelectionChanged -= new EventHandler(this.OnSelectionChanged);
			base.Dispose(disposing);
		}

		protected override /*ParentControlDesigner*/ void OnPaintAdornments(PaintEventArgs pe)
		{
			if(iamSelected)
				DrawingUtils.DrawDesignTimeBorder(pe.Graphics, this.Control);
			base.OnPaintAdornments(pe);
		}

//		// Forward mouse move messages to the commandbar
//		protected override bool GetHitTest(Point point)
//		{
//			CommandBar cbar = this.Control as CommandBar;
//			(cbar as ICommandBarDesignerInvoke).HandleMouseMove(cbar.PointToClient(point));
//			return true;
//		}
//
//		protected override void OnMouseDragMove(int x, int y)
//		{
//			CommandBar cbar = this.Control as CommandBar;
//			(cbar as ICommandBarDesignerInvoke).HandleMouseMove(cbar.PointToClient(new Point(x,y)));
//		}

		protected override void OnDragDrop(DragEventArgs de)
		{
			base.OnDragDrop(de);

			CommandBar cbar = this.Control as CommandBar;
			if(cbar.Controls.Count > 0)
			{
				(cbar as ICommandBarDesignerInvoke).SetChildControlBounds();
				ISelectionService selsvc = (ISelectionService)this.GetService(typeof(ISelectionService));
				Object[] objarray = new Object[1];
				objarray[0] = cbar;
				selsvc.SetSelectedComponents(objarray);
				objarray[0] = cbar.Controls[0];
				selsvc.SetSelectedComponents(objarray);
			}
		}

		protected override void PreFilterProperties(IDictionary properties)
		{
			base.PreFilterProperties(properties);

			String[] strcolln = new String[6];
			strcolln[0] = "RightToLeft";
			strcolln[1] = "ContextMenu";
			strcolln[2] = "ImeMode";
			strcolln[3] = "Dock";
			strcolln[4] = "Anchor";
			strcolln[5] = "CausesValidation";
			RemovePropertyBrowsable(this.Control, strcolln, properties);
		}

		static private void RemovePropertyBrowsable(Control control, String[] strcolln, IDictionary properties)
		{
			foreach(String property in strcolln)
			{
				PropertyDescriptor prop = (PropertyDescriptor)properties[property];
				if( (prop != null) && (prop.IsBrowsable == true) )
				{
					AttributeCollection mac = prop.Attributes;
					bool bnondef = false;
					foreach(Attribute mematt in mac)
					{
						// Is Browsable a default attribute? If so, break.
						if(mematt as BrowsableAttribute != null)
						{
							bnondef = true;
							break;
						}
					}
					int ncount = (bnondef == true) ? mac.Count : mac.Count + 1;
					Attribute[] arrmematt = new Attribute[ncount];
					mac.CopyTo(arrmematt, 0);
					if(bnondef == true)
						arrmematt[Array.IndexOf(arrmematt, BrowsableAttribute.Yes)] = BrowsableAttribute.No;
					else
						arrmematt[ncount-1] = BrowsableAttribute.No;
					properties[property] = TypeDescriptor.CreateProperty(control.GetType(), prop, arrmematt);
				}
			}
		}

		// Implementation of the ICommandBarDesignerComponentInvoke interface
		public void RaiseComponentChanged()
		{
			try
			{
				base.RaiseComponentChanged( null, null, null );
			}
			catch( Exception ex )
			{
				IDesignerHost host = (IDesignerHost)this.GetService( typeof( IDesignerHost ) );

				if( host != null )
				{
					IUIService ui = (IUIService)host.GetService( typeof( IUIService ) );

					if( ui != null )
					{
						ui.ShowError( ex );
					}
				}
			}
		}

#if SyncfusionFramework2_0
        public override GlyphCollection GetGlyphs( GlyphSelectionType selectionType )
        {
            selectionType = System.Windows.Forms.Design.Behavior.GlyphSelectionType.NotSelected;
            return base.GetGlyphs( selectionType );
        }
#endif
	}
    /// <exclude/>
	public sealed class CommandDockBarSerializationProvider : IDesignerSerializationProvider
	{
		private CommandDockBarSerializer serializer;

		public CommandDockBarSerializationProvider()
		{
			serializer = new CommandDockBarSerializer();
		}

		public object GetSerializer(IDesignerSerializationManager manager, object currentSerializer,
			Type objectType, Type serializerType)
		{
			if( (objectType != null) && ((objectType == typeof(CommandDockBar))
				|| (objectType.IsSubclassOf(typeof(CommandDockBar)))) )
			{
				return serializer;
			}
			
			return null;
		}
	}
    /// <exclude/>
	public sealed class CBSerializationProvider : IDesignerSerializationProvider
	{
		public CBSerializationProvider()
		{
		}

		public object GetSerializer(IDesignerSerializationManager manager, object currentSerializer,
			Type objectType, Type serializerType)
		{
			return null;
		}

		public void Dispose()
		{
		}
	}
    /// <exclude/>
	public sealed class CommandDockBarSerializer : CodeDomSerializer
	{
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
		{
			CodeDomSerializer baseClassSerializer =
				(CodeDomSerializer)manager.GetSerializer(typeof(CommandDockBar), typeof(CodeDomSerializer));
			return baseClassSerializer.Deserialize(manager, codeObject);
		}

		public override object Serialize(IDesignerSerializationManager manager, object obj)
		{
			return null;
		}
	}
}
