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
using System.Reflection;
using System.Diagnostics;
using Microsoft.Win32;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms.Design;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System.Windows.Forms.Design.Behavior;
#endif
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.CodeDom;
using System.IO;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <exclude/>
	/// <summary>
	/// Summary description for DockingManagerDesigner.
	/// </summary>
	public sealed class DockingManagerDesigner : ComponentDesigner, IDockingManagerDesignerComponentInvoke, IGetMsgProcListener, ICallWndProcListener
	{
		private static Control LButtonDownControl = null;
		private static Point LButtonDownPoint = Point.Empty;
		private static bool bKeyBdEvent = false;

		internal bool bReloading = false;

		private DMgrSerializationProvider dmSerProvider = null;
		internal bool bRemoved = false;
		private ArrayList alRemoved = new ArrayList();
		private DockPropertiesExtender propertyExtender =  null;
		private bool m_bEscapeFired = false;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		private bool m_bAdornersVisible = true;
#endif

		public DockingManagerDesigner()
		{
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

		DesignerActionListCollection actionLists;

		public override DesignerActionListCollection ActionLists
		{
			get
			{
				if( null == actionLists )
				{
					actionLists = new DesignerActionListCollection();
					actionLists.Add(
						new DockingManagerActionList( this.Component ) );
				}
				return actionLists;
			}
		}

		private bool bAllowAdornerSwitching = true;
		private BehaviorService behavior = null;
		private Hashtable nativeWindows = new Hashtable();

#endif
		private IntPtr VSMainForm
		{
			get
			{
				DockingManager dmgr = Component as DockingManager;
				return Syncfusion.Runtime.InteropServices.NativeMethods.GetAncestor(dmgr.HostControl.Handle, 3);
			}
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

	public override void  InitializeNewComponent(IDictionary defaultValues)
    {
 	     base.InitializeNewComponent(defaultValues);
         InitDMCaptionButtons();
    }

#else

    public override void OnSetComponentDefaults()
    {
        base.OnSetComponentDefaults();
        InitDMCaptionButtons();
    }

#endif
        
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);

			IDesignerHost idhost = component.Site.Container as IDesignerHost;
			ComponentCollection clln = idhost.Container.Components;
			foreach(IComponent obj in clln)
			{
				// If an instance of the docking manager already exists for this designerhost,
				// then throw an exception.
				if( (obj is DockingManager) && (obj != component) )
					throw new ApplicationException("Only one instance of the DockingManager can exist on a form.");
			}

			idhost.LoadComplete += new System.EventHandler(this.IDesignerHost_LoadComplete);
			ContainerControl hostcontrol = idhost.RootComponent as ContainerControl;
			
			Trace.Assert(hostcontrol != null);

			// Subscribe to the designerhosts activated/deactivated events
			idhost.Activated += new System.EventHandler(this.IDesignerHost_Activated);
			idhost.Deactivated += new System.EventHandler(this.IDesignerHost_Deactivated);		

			DockingManager dmgr = component as DockingManager;
			dmgr.HostControl = hostcontrol;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			dmgr.OnEnabledDocking += new DockingManager.EnabledDockingEventHandler(dmgr_OnEnabledDocking);
			behavior = GetService( typeof( BehaviorService ) ) as BehaviorService;
#endif

			IComponentChangeService iccs = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(iccs != null)
			{
				iccs.ComponentAdded += new ComponentEventHandler(this.IComponentChangeService_ComponentAdded);
				iccs.ComponentRename += new ComponentRenameEventHandler(this.IComponentChangeService_ComponentRename);
				iccs.ComponentRemoving += new ComponentEventHandler(this.IComponentChangeService_ComponentRemoving);
			}

			ISelectionService iss = this.GetService(typeof(ISelectionService)) as ISelectionService;
			if(iss != null)
			{
				iss.SelectionChanging += new EventHandler(this.ISelectionService_SelectionChanging);
				iss.SelectionChanged += new EventHandler(this.ISelectionService_SelectionChanged);
			}

			// Designer support for form inheritance requires that the memorystream be recreated for each inherited form.
			// Failing to do this will cause the CodeDomSerializer to not serialize the MemoryStream in inherited forms.
			if( dmgr.DockLayoutStream != null && dmgr.DockLayoutStream.Length > 0 )
			{
				byte[] array = dmgr.DockLayoutStream.ToArray();
				dmgr.DockLayoutStream = new MemoryStream();
				dmgr.DockLayoutStream.Write( array, 0, array.Length );
			}
			else
				dmgr.DockLayoutStream = new MemoryStream();

			// Redock all inherited docking windows. DockHosts need to be recreated within the designer.
			IDockingManagerDesignerInvoke iinvoke = dmgr as IDockingManagerDesignerInvoke;
			ArrayList alenabled = iinvoke.GetEnableDockingList();
			ArrayList alinherited = iinvoke.GetInheritedControlsList();
			foreach(Control ctrl in alenabled)
				alinherited.Add(ctrl);

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			try
			{
				bAllowAdornerSwitching = false;
#endif
				foreach( Control ctrl in alinherited )
					dmgr.SetEnableDocking( ctrl, false );
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			}
			finally
			{
				bAllowAdornerSwitching = true;
			}
#endif

			alenabled.AddRange(alinherited);

			// Custom serialization provider
			this.dmSerProvider = new DMgrSerializationProvider(this);
			IDesignerSerializationManager idsm = (IDesignerSerializationManager)this.GetService(typeof(IDesignerSerializationManager));
			if(idsm != null)
			{
				idsm.AddSerializationProvider(this.dmSerProvider);
			}

			this.propertyExtender = new DockPropertiesExtender(this);
			IExtenderProviderService ieps = this.GetService(typeof(IExtenderProviderService)) as IExtenderProviderService;
			ieps.AddExtenderProvider(this.propertyExtender);

			// Calling GetAncestor seems to break the designer sometimes (when opening a derived form
			// a tab control in VI mode doesn't respond to mouse down for tab switching!)
			// So, moving this to IDesignerSerializationManager_SerializationComplete.
			//this.VSMainForm = Syncfusion.Runtime.InteropServices.NativeMethods.GetAncestor(dmgr.HostControl.Handle, 3);

			DesignerHooks.AddGetMsgProcListener(this);
			DesignerHooks.AddCallWndProcListener(this);
		}
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		private void dmgr_OnEnabledDocking( Control ctrl, bool enabledDocking )
		{
			if( enabledDocking )
			{
				HideAdorners();
				AssignHandle( ctrl );
			}
			else
			{
				ShowAdorners();
				ReleaseHandle( ctrl );
			}
		}

		private void AssignHandle( Control control )
		{
			if( !nativeWindows.Contains( control ) )
			{
				ControlNativeWindow window = new ControlNativeWindow();
				window.AssignHandle( control.Handle );
				nativeWindows[ control ] = window;
			}
		}

		private void ReleaseHandle( Control control )
		{
			if( nativeWindows.Contains( control ) )
			{
				ControlNativeWindow window = nativeWindows[ control ] as ControlNativeWindow;
				window.ReleaseHandle();
				nativeWindows.Remove( control );
			}
		}
#endif

		private void InitDMCaptionButtons()
		{
			DockingManager dm = this.Component as DockingManager;
			if (dm != null)
			{
				dm.CaptionButtons = dm.GetDefaultCaptionButtons();
			}
		}
		bool bLoadComplete = false;
		private void IDesignerHost_LoadComplete(Object sender, EventArgs e)
		{
			bLoadComplete = true;
			DockingManager dmgr = this.Component as DockingManager;
			dmgr.DesignDockStateLoad = true;
			IDockingManagerDesignerInvoke iinvoke = dmgr as IDockingManagerDesignerInvoke;
			iinvoke.GetHostFormController().UpdateFormClientSetting();
			iinvoke.GetHostFormController().AdjustLayoutDockArea();

			// When loading persisted designer state, hold off hooking/subclassing till designer activation
			Hashtable httext = iinvoke.GetTextTable();
			Hashtable hticon = iinvoke.GetIconTable();
            Hashtable htmIcon = iinvoke.GetmIconTable();
			Hashtable htDockAbility = iinvoke.GetDockAbilityTable();
			Hashtable htOuterDockAbility = iinvoke.GetOuterDockAbilityTable();
			ArrayList alFreezeRisize = iinvoke.GetFreezeResizeControllers();
			this.bReloading = true;
			foreach(Control ctrl in iinvoke.GetEnableDockingList())
			{
				dmgr.SetEnableDocking(ctrl, true);
				if(httext.Contains(ctrl) == true)
					dmgr.SetDockLabel(ctrl, (String)httext[ctrl]);
                if (hticon.Contains(ctrl) == true)
                {
                    dmgr.SetDockIcon(ctrl, (int)hticon[ctrl]);
                }
                if (htmIcon.Contains(ctrl) == true)
                {
                    dmgr.SetMDIChildIcon(ctrl, (int)htmIcon[ctrl]);
                }
				if(htDockAbility.Contains(ctrl) == true)
					dmgr.SetDockAbility(ctrl, (DockAbility)htDockAbility[ctrl]);
				if(htOuterDockAbility.Contains(ctrl) == true)
					dmgr.SetOuterDockAbility(ctrl, (DockAbility)htOuterDockAbility[ctrl]);
				if(alFreezeRisize.Contains(ctrl))
					dmgr.SetFreezeResize(ctrl, true);
			}
			if (!dmgr.DesignMode)
				dmgr.UpdateDesigner(); 
			// If a designer persistence state exists, then apply this state to the layout manager.
			if((dmgr.DockLayoutStream != null) && (dmgr.DockLayoutStream.Length > 0))
			{
				dmgr.DockLayoutStream.Seek(0, SeekOrigin.Begin);
				if(iinvoke.LoadFromStream(dmgr.DockLayoutStream) == true)
                {
					iinvoke.ApplyDHCFloatOnlySettings();
                    if(dmgr.DesignMode)
                    dmgr.LoadDockState();
                }
				else
				{
					MessageBox.Show("The DockingManager designer has failed to load the dockstate. Please restart Visual Studio.NET and reload the project.", "Syncfusion - Essential Tools", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			bAllowAdornerSwitching = true;
			ISelectionService iss = this.GetService(typeof(ISelectionService)) as ISelectionService;
			if( iss != null )
			{
				Control control = iss.PrimarySelection as Control;
				if( control != null )
				{
					if( iinvoke.GetEnableDockingList().Contains( control ) )
					{
						HideAdorners();
					}
					else
					{
						if( control == dmgr.HostControl )
						{
							ShowAdorners();
						}
					}
				}
			}
#endif
			this.bReloading = false;
			dmgr.DesignDockStateLoad = false;
		}

		private void ISelectionService_SelectionChanging(Object sender, EventArgs e)
		{
			ISelectionService iss = this.GetService(typeof(ISelectionService)) as ISelectionService;
			if(iss != null)
			{
				DockingManager dmgr = this.Component as DockingManager;
				if((iss.PrimarySelection is DockHost) || (iss.PrimarySelection is FloatingForm))
				{
					// The designerhost automatically sets selection to the parent when a control is deleted.
					// We workaround this problem by explicitly selecting the design form.
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
					iss.SetSelectedComponents(new Control[] { dmgr.HostControl }, SelectionTypes.MouseDown);
#else
					iss.SetSelectedComponents( new Control[] { dmgr.HostControl }, SelectionTypes.Primary );
#endif					
				}
				else
				{
					IDockingManagerDesignerInvoke iinvoke = dmgr as IDockingManagerDesignerInvoke;
					DockHostController dhcinfocus = iinvoke.GetDHCInFocus();
					Control cont = iss.PrimarySelection as Control;
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
					if( !iinvoke.GetEnableDockingList().Contains(cont) )
					{
						ShowAdorners();
					}
#endif
					if((dhcinfocus != null) && (dhcinfocus.HostControl.Controls[0] != iss.PrimarySelection))
					{
						DockHost dhost = dhcinfocus.HostControl as DockHost;
						if(dhcinfocus.HideCaption == false)
						{
							if( dmgr.VisualStyle != VisualStyle.Default )
							{
								dhost.Invalidate( new Rectangle( dmgr.Renderer.ThinBorderWidth, 
									dmgr.Renderer.ThinBorderWidth,
									dhost.Bounds.Width - 2 * dmgr.Renderer.ThinBorderWidth,
									dmgr.Renderer.CaptionWidth ) );
							}
							else
							{
								dhost.Invalidate( dhost.TitleBar.CaptionRect );
							}
						}
						iinvoke.SetDHCInFocus(null);
					}
				}
			}
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		private void ShowAdorners()
		{
			if( bAllowAdornerSwitching )
			{
				if( !m_bAdornersVisible )
				{
					foreach( Adorner ad in behavior.Adorners )
					{
						if( !IsControlBodyGlyphsAdorner( ad ) )
						{
							ad.Enabled = true;
						}
					}
					m_bAdornersVisible = true;
				}
			}
		}

		private void HideAdorners()
		{
			if( bAllowAdornerSwitching )
			{
				if( m_bAdornersVisible )
				{
					foreach( Adorner ad in behavior.Adorners )
					{
						if( !IsControlBodyGlyphsAdorner( ad ) )
						{
							ad.Enabled = false;
						}
					}
					m_bAdornersVisible = false;
				}
			}
		}

		private bool IsControlBodyGlyphsAdorner( Adorner adorner )
		{
			bool result = false;
			foreach( Glyph glyph in adorner.Glyphs )
			{
				if( glyph is ControlBodyGlyph )
				{
					result = true;
					break;
				}
			}

			return result;
		}
#endif

        private void ISelectionService_SelectionChanged(Object sender, EventArgs e)
		{
			ISelectionService iss = this.GetService(typeof(ISelectionService)) as ISelectionService;
			if(iss != null)
			{
				DockingManager dmgr = this.Component as DockingManager;
				// If there was any focused DockHost, repaint it's caption.
				if( dmgr.dhLastActive != null )
				{
					if( dmgr.VisualStyle != VisualStyle.Default )
					{
						dmgr.dhLastActive.Invalidate( new Rectangle( dmgr.Renderer.ThinBorderWidth, 
							dmgr.Renderer.ThinBorderWidth,
							dmgr.dhLastActive.Bounds.Width - 2 * dmgr.Renderer.ThinBorderWidth,
							dmgr.Renderer.CaptionWidth ) );
						dmgr.dhLastActive = null;
					}
				}
				IDockingManagerDesignerInvoke iinvoke = dmgr as IDockingManagerDesignerInvoke;
				ArrayList alenabled = iinvoke.GetEnableDockingList();
				foreach(Control ctrl in alenabled)
				{
					if(iss.GetComponentSelected(ctrl) == true)
					{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
						HideAdorners();
#endif

						DockHost dhost = ctrl.Parent as DockHost;
						if((dhost.InternalController as DockHostController).HideCaption == false)
						{
							if( dmgr.VisualStyle != VisualStyle.Default )
							{
								dhost.Invalidate( new Rectangle( dmgr.Renderer.ThinBorderWidth, 
									dmgr.Renderer.ThinBorderWidth,
									dhost.Bounds.Width - 2 * dmgr.Renderer.ThinBorderWidth,
									dmgr.Renderer.CaptionWidth ) );
							}
							else
							{
								dhost.Invalidate( dhost.TitleBar.CaptionRect );
							}
						}
						break;
					}
				}
			}
		}

		private void IComponentChangeService_ComponentAdded(Object sender, ComponentEventArgs e)
		{
			DockingManager dmgr = this.Component as DockingManager;
			if((e.Component is Control) && (e.Component != dmgr.HostControl))
			{
				(e.Component as Control).ParentChanged += new EventHandler(this.OnControlParentChanged);
			}
			else if(e.Component == this.Component)
			{
				// For all existing controls, subscribe to the Control.ParentChanged handler
				IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
				foreach(IComponent obj in idh.Container.Components)
				{
					if((obj is Control) && (obj != dmgr.HostControl))
					{
						Control ctrl = obj as Control;
						ctrl.ParentChanged += new EventHandler(this.OnControlParentChanged);
						TypeDescriptor.Refresh(ctrl);
					}
				}
			}
		}

		private void IComponentChangeService_ComponentRename(object sender, ComponentRenameEventArgs e)
		{
			if (bLoadComplete)
			{
				DockingManager dmgr = this.Component as DockingManager;

				if(dmgr.DesignMode == true)
					dmgr.UpdateDesigner();
			}
		}

		private void IComponentChangeService_ComponentRemoving(Object sender, ComponentEventArgs e)
		{
			DockingManager dmgr = this.Component as DockingManager;
			if((e.Component is Control) && (e.Component != dmgr.HostControl))
			{
				Control ctrl = e.Component as Control;
				ctrl.ParentChanged -= new EventHandler(this.OnControlParentChanged);
				IDockingManagerDesignerInvoke iinvoke = dmgr as IDockingManagerDesignerInvoke;
				Array enabledarray = iinvoke.GetEnableDockingList().ToArray(typeof(Control));
				if(Array.IndexOf(enabledarray, ctrl) >= 0)
					dmgr.SetEnableDocking(ctrl, false);
			}
		}

		private void OnControlParentChanged(object sender, EventArgs e)
		{
			Control ctrl = sender as Control;
			DockingManager dmgr = this.Component as DockingManager;

			if( dmgr != null )
			{
				IDockingManagerDesignerInvoke iinvoke = dmgr as IDockingManagerDesignerInvoke;
				Array enabledarray = iinvoke.GetEnableDockingList().ToArray( typeof( Control ) );
				if( (Array.IndexOf( enabledarray, ctrl ) >= 0) && (ctrl.Parent != null) && ((ctrl.Parent is DockHost) == false) )
				{
					// A dock-enabled control has been reparented through direct assignment by some other control/component.
					// Get the orphaned DockHostController, temporarily parent the control to it and then disable the control
					// as a docking window and reparent it to the original parent.
					ctrl.ParentChanged -= new EventHandler( this.OnControlParentChanged );
					Control parent = ctrl.Parent;
					Array dhcarray = iinvoke.GetControllerList().ToArray( typeof( DockControllerBase ) );
					foreach( DockControllerBase dcbase in dhcarray )
					{
						if( dcbase is DockHostController )
						{
							DockHostController dhc = dcbase as DockHostController;
							if( dhc.HostControl.Controls.Count == 0 )
							{
								dhc.HostControl.Controls.Add( ctrl );
								break;
							}
						}
					}
					dmgr.SetEnableDocking( ctrl, false );	// SetEnableDocking will parent the control to the Form
					ctrl.Parent = parent;
					ctrl.ParentChanged += new EventHandler( this.OnControlParentChanged );
				}
			
				TypeDescriptor.Refresh( ctrl );
			}
		}

		private void IDesignerHost_Activated(Object sender, EventArgs e)
		{
			if(this.bReloading == true)
				this.bReloading = false;
			// Show all the floating forms, and set the msgproc and wndproc hooks
			ToggleFloatingState(true);

			DesignerHooks.AddGetMsgProcListener(this);
			DesignerHooks.AddCallWndProcListener(this);
		}


		private void IDesignerHost_Deactivated(Object sender, EventArgs e)
		{
			// Hide all design time floating forms, and remove the msgproc and wndproc hooks
			ToggleFloatingState(false);
      
			DesignerHooks.RemoveGetMsgProcListener(this);
			DesignerHooks.RemoveCallWndProcListener(this);
		}

		public void ToggleFloatingState(bool bactivate)
		{
			DockingManager dmgr = this.Component as DockingManager;
			IDockingManagerDesignerInvoke iinvoke = dmgr as IDockingManagerDesignerInvoke;
			ArrayList fflist = iinvoke.GetFFControllerList();
			foreach(DockControllerBase dcb in fflist)
				dcb.HostControl.Visible = bactivate;
		}

		// IGetMsgProcListener implementation
		public void GetMsgProc(int nCode, IntPtr wparam, IntPtr lparam)
		{
			Message msg = (Message)(Marshal.PtrToStructure(lparam, typeof(Message)));
			if((msg.Msg == 0x0201 /*WM_LBUTTONDOWN*/) || (msg.Msg == 0x0203 /*WM_LBUTTONDBLCLK*/)
				|| (msg.Msg == 0x0200 /*WM_MOUSEMOVE*/) || (msg.Msg == 0x0202 /*WM_LBUTTONUP*/)
				|| (msg.Msg == 0x00A1 /*WM_NCLBUTTONDOWN*/) || (msg.Msg == 0x00A3/*WM_NCLBUTTONDBLCLK*/))
			{
				Control ctrlhit = Control.FromHandle(msg.HWnd);
				if(ctrlhit != null)
				{
					IDockingManagerDesignerMouseHook idmh = ctrlhit as IDockingManagerDesignerMouseHook;
					if((idmh != null) && (idmh.GetDesignMode() == true))
					{
						Point ptscreen = Cursor.Position;
						if(msg.Msg == 0x0200 /*WM_MOUSEMOVE*/)
						{
							if(msg.WParam == (IntPtr)0x0001 /*MK_LBUTTON*/)
							{
								// A drag is about to commence. Synthesize the 'ESC' keyboard event to preempt designer band drawing.
								if( !m_bEscapeFired &&
									( DockingManagerDesigner.LButtonDownControl != null ) &&
									( Math.Abs( ptscreen.X - DockingManagerDesigner.LButtonDownPoint.X ) >= 1
									|| Math.Abs( ptscreen.Y - DockingManagerDesigner.LButtonDownPoint.Y ) >= 1
									|| DockingManagerDesigner.bKeyBdEvent == true )	)
								{
									Syncfusion.Runtime.InteropServices.NativeMethods.keybd_event( ( byte )0x1B /*VK_ESCAPE*/, ( byte )0, 0, IntPtr.Zero );
									Syncfusion.Runtime.InteropServices.NativeMethods.keybd_event( ( byte )0x1B /*VK_ESCAPE*/, ( byte )0, 0x0002/*KEYEVENTF_KEYUP*/, IntPtr.Zero );
									DockingManagerDesigner.bKeyBdEvent = false;
									m_bEscapeFired = true;
								}
								idmh.HandleMouseMove(MouseButtons.Left, ptscreen);
							}
							else
							{
								if(ctrlhit.Cursor != Cursor.Current)
									Cursor.Current = ctrlhit.Cursor;
								idmh.HandleMouseMove(MouseButtons.None, ptscreen);
							}
						}
						else if( ( msg.Msg == 0x0201 /*WM_LBUTTONDOWN*/) )
						{
							idmh.HandleMouseDown( MouseButtons.Left, ptscreen );
							DockingManagerDesigner.LButtonDownControl = ctrlhit;
							DockingManagerDesigner.LButtonDownPoint = ptscreen;
							m_bEscapeFired = false;
							if( Syncfusion.Runtime.InteropServices.NativeMethods.GetCapture() != ctrlhit.Handle )
								Syncfusion.Runtime.InteropServices.NativeMethods.SetCapture( ctrlhit.Handle );
						}
						else if( msg.Msg == NativeMethods.WM_NCLBUTTONUP )
						{
							idmh.HandleMouseDown( MouseButtons.Left, ptscreen );
							DockingManagerDesigner.LButtonDownControl = null;
							DockingManagerDesigner.LButtonDownPoint = Point.Empty;
							DockingManagerDesigner.bKeyBdEvent = false;
							Syncfusion.Runtime.InteropServices.NativeMethods.ReleaseCapture();
						}
						else if( msg.Msg == 0x0202 /*WM_LBUTTONUP*/)
						{
							idmh.HandleMouseUp( MouseButtons.Left, ptscreen );
							if( DockingManagerDesigner.LButtonDownControl != null )
							{
								DockingManagerDesigner.LButtonDownControl = null;
								DockingManagerDesigner.LButtonDownPoint = Point.Empty;
								DockingManagerDesigner.bKeyBdEvent = false;
								Syncfusion.Runtime.InteropServices.NativeMethods.ReleaseCapture();
							}
						}
						else if(msg.Msg == 0x0203 /*WM_LBUTTONDBLCLK*/)
						{
							idmh.HandleDoubleClick(ptscreen);
						}
						else if(msg.Msg == 0x00A1 /*WM_NCLBUTTONDOWN*/)
						{
							int htval = msg.WParam.ToInt32();
							if((htval == 2/*HTCAPTION*/) && (DockingManagerDesigner.bKeyBdEvent == false))
							{
								idmh.HandleMouseDown(MouseButtons.Left, ptscreen);
								DockingManagerDesigner.LButtonDownControl = ctrlhit;
								DockingManagerDesigner.LButtonDownPoint = ptscreen;
								DockingManagerDesigner.bKeyBdEvent = true;
								m_bEscapeFired = false;
								if(Syncfusion.Runtime.InteropServices.NativeMethods.GetCapture() != ctrlhit.Handle)
									Syncfusion.Runtime.InteropServices.NativeMethods.SetCapture(ctrlhit.Handle);
							}
							else if((htval >= 10/*HTSIZEFIRST*/) && (htval <= 17/*HTSIZELAST*/))
							{
								idmh.InitiateFloatingResize(ptscreen, htval);
								DockingManagerDesigner.LButtonDownControl = ctrlhit;
								DockingManagerDesigner.LButtonDownPoint = ptscreen;
								m_bEscapeFired = false;
								if(Syncfusion.Runtime.InteropServices.NativeMethods.GetCapture() != ctrlhit.Handle)
									Syncfusion.Runtime.InteropServices.NativeMethods.SetCapture(ctrlhit.Handle);
							}
						}
						else	// WM_NCLBUTTONDBLCLK
						{
							int htval = msg.WParam.ToInt32();
							if(htval == 2 /*HTCAPTION*/)
							{
								idmh.HandleDoubleClick(ptscreen);
							}
						}
					}
					else if((idmh == null) && (msg.Msg == 0x0201/*WM_LBUTTONDOWN*/))
					{
						idmh = ctrlhit.TopLevelControl as IDockingManagerDesignerMouseHook;
						if((idmh != null) && (idmh.GetDesignMode() == true))
						{
							// A child of a floating dockhost was hit. Set this control as selected.
							ISelectionService iss = this.GetService(typeof(ISelectionService)) as ISelectionService;
							if((iss != null) && (iss.PrimarySelection is Control))
							{
								Control selctrl = iss.PrimarySelection as Control;
								if( (selctrl == null) || (selctrl.Equals( ctrlhit ) == false) )
								{
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
									iss.SetSelectedComponents(new Control[] { ctrlhit }, SelectionTypes.MouseDown);
#else
									iss.SetSelectedComponents( new Control[] { ctrlhit }, SelectionTypes.Primary );
#endif
								}
							}
						}
					}
				}
			}
		}

		// ICallWndProcListener implementation
		public void CallWndProc(int nCode, IntPtr wparam, IntPtr lparam)
		{
			Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT cwp =
				(Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT)(Marshal.PtrToStructure(lparam, typeof(Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT)));

			if(nCode >= 0)
			{
				if((cwp.message == 0x0215 /*WM_CAPTURECHANGED*/) && (DockingManagerDesigner.LButtonDownControl != null))
				{
					if(cwp.hwnd == DockingManagerDesigner.LButtonDownControl.Handle)
						Syncfusion.Runtime.InteropServices.NativeMethods.SetCapture(DockingManagerDesigner.LButtonDownControl.Handle);
				}

				if( cwp.message == NativeMethods.WM_ACTIVATE && cwp.hwnd == this.VSMainForm )
				{
					int hiword = NativeMethods.HIWORD(cwp.wParam);
					int loword = NativeMethods.LOWORD(cwp.wParam);

					if( hiword != 0 )
						hostFormMinimized = true;

					if( loword == 0 )
						activateApp = false;
					else
					{
						DockingManager dm = this.Component as DockingManager;
						bool canShow = true;

						if( dm != null && !dm.bLoadVisibility )
							canShow = false;

						activateApp = true;
						if( !hostFormMinimized && canShow )
							ToggleFloatingState(true);
					}
				}

				if( cwp.message == NativeMethods.WM_ACTIVATEAPP && cwp.hwnd == this.VSMainForm )
				{
					if( cwp.wParam == IntPtr.Zero )
					{
						this.ToggleFloatingState(false);
						DesignerHooks.RemoveGetMsgProcListener(this);
					}
					else
					{
						if( !hostFormMinimized && activateApp )
							this.ToggleFloatingState(true);
						DesignerHooks.AddGetMsgProcListener(this);
					}
				}

				if(cwp.message == NativeMethods.WM_SIZE)
				{
					if( cwp.wParam == new IntPtr(1)/*SIZE_MINIMIZED*/ )
					{	
						hostFormMinimized = true;
					}
					else
					{
						DockingManager dm = this.Component as DockingManager;

						if( dm != null && dm.bLoadVisibility )
							this.ToggleFloatingState(true);

						hostFormMinimized = false;
						activateApp = true;
					}
				}
				
				if(cwp.message == 0x31A /*WM_THEMECHANGED*/)
				{
					DockingManager dmgr = this.Component as DockingManager;
					if(dmgr.VisualStyle != VisualStyle.Default)
					{
						dmgr.Renderer.RefreshColors();
						if (dmgr.HostControl != null)
						{
							Graphics g = Graphics.FromHwnd(dmgr.HostControl.Handle);
							dmgr.UpdateFonts( g.DpiY );
						}
						NativeMethodsHelper.RedrawWindow(cwp.hwnd, NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_FRAME);
					}
				}
			}
		}
		private bool hostFormMinimized = false;
		private bool activateApp = true;

		public void RemoveUnserializableChildControls()
		{
			// Remove the child controls, the dockbars, that are not serializable.
			if(this.bRemoved == false)
			{
				DockingManager dockingmgr = this.Component as DockingManager;
				Control[] ctrlarray = new Control[dockingmgr.HostControl.Controls.Count];
				int i = 0;
				foreach(Control ctrl in dockingmgr.HostControl.Controls)
					ctrlarray[i++] = ctrl;
				foreach(Control ctrl in ctrlarray)
				{
					if((ctrl is DockHost) || (ctrl is DragSplitter) || (ctrl is AHTabControl))
					{
						this.alRemoved.Add(ctrl);
						dockingmgr.HostControl.Controls.Remove(ctrl);
					}
				}
				this.bRemoved = true;
			}
		}

		public void ReaddUnserializableChildControls()
		{
			// Readd the dockbars that were removed during the Form.Controls serialization
			if(this.bRemoved == true)
			{
				DockingManager dockingmgr = this.Component as DockingManager;
				foreach(Control ctrl in this.alRemoved)
					dockingmgr.HostControl.Controls.Add(ctrl);
				this.alRemoved.Clear();
				this.bRemoved = false;
			}
		}

		public void RaiseComponentChanged()
		{
			base.RaiseComponentChanged(null,null,null);
		}

		protected override void PostFilterProperties(IDictionary properties)
		{
			// The DockingManager.PersistState property is valid only when the HostControl is a Form
			DockingManager dockingmgr = this.Component as DockingManager;
			if((dockingmgr != null) && (dockingmgr.HostControl != null)
				&& ((dockingmgr.HostControl is Form) == false) )
			{
				if(properties.Contains("PersistState"))
					properties.Remove("PersistState");
			}
			base.PostFilterProperties(properties);
		}

		protected override void Dispose(bool bdisposing)
		{
			DesignerHooks.RemoveCallWndProcListener(this);
			DesignerHooks.RemoveGetMsgProcListener(this);

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			foreach( ControlNativeWindow window in nativeWindows.Values )
			{
				window.ReleaseHandle();
			}
			nativeWindows.Clear();
#endif

			IDesignerSerializationManager idsm = (IDesignerSerializationManager)this.GetService(typeof(IDesignerSerializationManager));
			if(idsm != null)
			{
				idsm.RemoveSerializationProvider(this.dmSerProvider);
				this.dmSerProvider.Dispose();
			}

			IExtenderProviderService ieps = this.GetService(typeof(IExtenderProviderService)) as IExtenderProviderService;
			if(ieps != null)
				ieps.RemoveExtenderProvider(this.propertyExtender);

			IDesignerHost idh = this.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(idh != null)
			{
				DockingManager dmgr = this.Component as DockingManager;

				// Unsubscribe the Control.ParentChanged handler
				foreach(IComponent obj in idh.Container.Components)
				{
					if((obj is Control) && (obj != dmgr.HostControl))
						(obj as Control).ParentChanged -= new EventHandler(this.OnControlParentChanged);
				}

				if(idh.Loading == false)	// Ignore when unloading the document
				{
					// Restore all controls to their non-docked state before removing the docking manager
					IDockingManagerDesignerInvoke iinvoke = dmgr as IDockingManagerDesignerInvoke;
					if(dmgr.DockLayoutStream != null)
					{
						dmgr.DockLayoutStream.Close();
						dmgr.DockLayoutStream = null;
					}
					Array dhcarray = iinvoke.GetControllerList().ToArray(typeof(DockControllerBase));
					foreach(DockControllerBase dcbase in dhcarray)
					{
						DockHostController dhc = dcbase as DockHostController;
						if(dhc != null)
							dmgr.SetEnableDocking(dhc.HostControl.Controls[0], false);
					}
				}
				idh.LoadComplete -= new System.EventHandler(this.IDesignerHost_LoadComplete);
				idh.Activated -= new System.EventHandler(this.IDesignerHost_Activated);
				idh.Deactivated -= new System.EventHandler(this.IDesignerHost_Deactivated);
			}

			IComponentChangeService iccs = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(iccs != null)
			{
				iccs.ComponentAdded -= new ComponentEventHandler(this.IComponentChangeService_ComponentAdded);
				iccs.ComponentRemoving -= new ComponentEventHandler(this.IComponentChangeService_ComponentRemoving);
				iccs.ComponentRename -= new ComponentRenameEventHandler(this.IComponentChangeService_ComponentRename);
			}

			ISelectionService iss = this.GetService(typeof(ISelectionService)) as ISelectionService;
			if(iss != null)
			{
				iss.SelectionChanging -= new EventHandler(this.ISelectionService_SelectionChanging);
				iss.SelectionChanged -= new EventHandler(this.ISelectionService_SelectionChanged);
			}

			base.Dispose(bdisposing);
		}
	}

    /// <exclude/>
	public sealed class DMgrSerializationProvider : IDesignerSerializationProvider
	{
		private DockingManagerDesigner notifyDesigner;
		private DMFormCollectionCodeDomSerializer cdsFormCollection;

		public DMgrSerializationProvider(DockingManagerDesigner dsgnr)
		{
			this.notifyDesigner = dsgnr;
			this.cdsFormCollection = new DMFormCollectionCodeDomSerializer(dsgnr);
		}

		public object GetSerializer(IDesignerSerializationManager manager, object currentSerializer,
			Type objectType, Type serializerType)
		{
			// If the non-designer controls are present, then return the custom formcollection
			// codedomserializer. However, if codedom serializer has already been
			// provided and the controls have been removed from the form, return null.
			if((this.notifyDesigner != null) && (this.notifyDesigner.bRemoved == false))
			{
				if( objectType != null ) 
				{
					if( (objectType == typeof(Control.ControlCollection))
						|| (objectType.IsSubclassOf(typeof(Control.ControlCollection))))
					{
						return this.cdsFormCollection;
					}
					else if ((objectType == typeof(CaptionButtonsCollection)))
					{
						return new CaptionButtonsCodeDomSerializer();
					}
					#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
					else if( (objectType == typeof(System.Windows.Forms.Keys)) )
					{
						return new KeysCodeDomSerializer();
					}
					#endif
				}
			}

			return null;
		}

		public void Dispose()
		{
			if(this.cdsFormCollection != null)
				this.cdsFormCollection.Dispose();
			this.cdsFormCollection = null;
			this.notifyDesigner = null;
		}
	}
    /// <exclude/>
	public sealed class DMFormCollectionCodeDomSerializer : CodeDomSerializer
	{
		private DockingManagerDesigner notifyDesigner;
		private DockingManager dockingManager;

		public DMFormCollectionCodeDomSerializer(DockingManagerDesigner dsgnr)
		{
			this.notifyDesigner = dsgnr;
			dockingManager = this.notifyDesigner.Component as DockingManager;
		}

		public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
		{
			CodeDomSerializer baseClassSerializer =
				(CodeDomSerializer)manager.GetSerializer(typeof(Control.ControlCollection), typeof(CodeDomSerializer));
			return baseClassSerializer.Deserialize(manager, codeObject);
		}

		public override object Serialize(IDesignerSerializationManager manager, object obj)
		{
			if( ( this.notifyDesigner != null ) && ( this.notifyDesigner.bRemoved == false ) )
			{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				bool bSerializeRibbonControls = false;

				if( obj is Control.ControlCollection )
				{
					Control.ControlCollection collControls = obj as Control.ControlCollection;

					bSerializeRibbonControls = ( collControls.Owner is RibbonControlAdv || collControls.Owner is RibbonControlAdvHeader );
				}
				else if( !( obj is RibbonControlAdv || obj is RibbonControlAdvHeader ) )
				{
					bSerializeRibbonControls = true;
				}

				if( bSerializeRibbonControls )
				{
					return null;
				}
#endif

				if( this.dockingManager.HostControl.Handle != IntPtr.Zero )
				{
					this.dockingManager.HostControl.SuspendLayout();
					Syncfusion.Runtime.InteropServices.NativeMethodsHelper.LockWindowUpdate( this.dockingManager.HostControl.Handle );
				}

				this.notifyDesigner.RemoveUnserializableChildControls();

				CodeDomSerializer baseclassserializer =
					( CodeDomSerializer ) manager.GetSerializer( typeof( Control.ControlCollection ), typeof( CodeDomSerializer ) );
				Object codeobject = baseclassserializer.Serialize( manager, obj );

				this.notifyDesigner.ReaddUnserializableChildControls();

				if( this.dockingManager.HostControl.Handle != IntPtr.Zero )
				{
					Syncfusion.Runtime.InteropServices.NativeMethodsHelper.LockWindowUpdate( IntPtr.Zero );
					this.dockingManager.HostControl.ResumeLayout();
				}

				return codeobject;
			}
			else
			{
				CodeDomSerializer baseclassserializer =
					( CodeDomSerializer ) manager.GetSerializer( typeof( Control.ControlCollection ), typeof( CodeDomSerializer ) );
				return baseclassserializer.Serialize( manager, obj );
			}
		}

		public void Dispose()
		{
			this.notifyDesigner = null;
			this.dockingManager = null;
		}
	}
    /// <exclude/>
	[
	ProvideProperty("DockLabel", typeof(Control)),
	ProvideProperty("DockIcon", typeof(Control)),
    ProvideProperty("MDIChildIcon", typeof(Control)),
	ProvideProperty("FloatOnly", typeof(Control)),
	ProvideProperty("AutoHideOnLoad", typeof(Control)),
	ProvideProperty("HiddenOnLoad", typeof(Control)),
	ProvideProperty("DockAbility", typeof(Control)),
	ProvideProperty("OuterDockAbility", typeof(Control)),
	ProvideProperty("CustomCaptionButtons", typeof(Control)),
	ProvideProperty("FreezeResize", typeof(Control)),
	ProvideProperty("AllowFloating", typeof(Control))
	]
	public sealed class DockPropertiesExtender : IExtenderProvider
	{
		public DockingManager dockingMgr = null;
		public DockingManagerDesigner dockingMgrDesigner = null;
		public DockPropertiesExtender(DockingManagerDesigner designer)
		{
			this.dockingMgrDesigner = designer;
			this.dockingMgr = designer.Component as DockingManager;
		}

		public bool CanExtend(object target)
		{
			bool bval = this.dockingMgr.CanExtend(target);
			if(bval == true)
				bval = this.dockingMgr.GetEnableDocking(target as Control);
			return bval;
		}
        [
        Category("Syncfusion Docking"),        
        RefreshProperties(RefreshProperties.All),
        Description("Enables docking.")
        ]
        public void SetEnableDocking(Control ctrl, bool value)
        {
            if (this.dockingMgrDesigner.bReloading == false)
            {
                this.dockingMgr.SetEnableDocking(ctrl, value);
                this.dockingMgr.UpdateDesigner();
            }
        }
        [
        Category("Syncfusion Docking"),
        DefaultValue(true),
        RefreshProperties(RefreshProperties.All),
        Description("gets docking")
        ]
        public bool GetEnableDocking(Control ctrl)
        {
            return this.dockingMgr.GetEnableDocking(ctrl);
        }

		[
		Category( "Syncfusion Docking" ),
		DefaultValue( true ),
		RefreshProperties( RefreshProperties.All ),
		Description( "Sets if the control can be transited to floating state." )
		]
		public void SetAllowFloating( Control ctrl, bool allowFloat )
		{
			if( this.dockingMgrDesigner.bReloading == false )
			{
				this.dockingMgr.SetAllowFloating( ctrl, allowFloat );
				this.dockingMgr.UpdateDesigner();
			}
		}

		[
		Category( "Syncfusion Docking" ),
		DefaultValue( true ),
		RefreshProperties( RefreshProperties.All ),
		Description( "TRUE if the control can be transited to floating state." )
		]
		public bool GetAllowFloating( Control ctrl )
		{
			return this.dockingMgr.GetAllowFloating( ctrl );
		}

		[
		Category("Syncfusion Docking"),
		DefaultValue(""),
		RefreshProperties(RefreshProperties.All),
		Description("The text displayed by the docking window caption.")
		]
		public void SetDockLabel(Control ctrl, String strtext)
		{
			if(this.dockingMgrDesigner.bReloading == false)
			{
				this.dockingMgr.SetDockLabel(ctrl, strtext);
				this.dockingMgr.UpdateDesigner();
			}
		}

		[
		Category("Syncfusion Docking"),
		DefaultValue(""),
		RefreshProperties(RefreshProperties.All),
		Description("The text displayed by the docking window caption.")
		]
		public String GetDockLabel(Control ctrl)
		{
			return this.dockingMgr.GetDockLabel(ctrl);
		}

		[
		Category("Syncfusion Docking"),
		DefaultValue(false),
		RefreshProperties(RefreshProperties.All),
		Description("Specifies whether the docking window should not be resized.")
		]
		public void SetFreezeResize( Control ctrl, bool freeze )
		{
			this.dockingMgr.SetFreezeResize(ctrl, freeze);
		}

		[
		Category("Syncfusion Docking"),
		DefaultValue(false),
		RefreshProperties(RefreshProperties.All),
		Description("Specifies whether the docking window should not be resized.")
		]
		public bool GetFreezeResize( Control ctrl )
		{
			return this.dockingMgr.GetFreezeResize(ctrl);
		}

		[
		Category("Syncfusion Docking"),
		DefaultValue(-1),
		RefreshProperties(RefreshProperties.All),
		Description("The index of the image associated with this docking window.")
		]
		public void SetDockIcon(Control ctrl, int index)
		{
			if(this.dockingMgrDesigner.bReloading == false)
				this.dockingMgr.SetDockIcon(ctrl, index);
		}

		[
		Category("Syncfusion Docking"),
		DefaultValue(-1),
		RefreshProperties(RefreshProperties.All),
		Description("The index of the image associated with this docking window.")
		]
		public int GetDockIcon(Control ctrl)
		{
			return this.dockingMgr.GetDockIcon(ctrl);
		}

        [
        Category("Syncfusion Docking"),
        DefaultValue(-1),
        RefreshProperties(RefreshProperties.All),
        Description("The index of the image associated with this docking window at MDI Child state.")
        ]
        public int GetMDIChildIcon(Control ctrl)
        {
            return this.dockingMgr.GetMDIChildIcon(ctrl);
        }

        [
         Category("Syncfusion Docking"),
         DefaultValue(-1),
         RefreshProperties(RefreshProperties.All),
         Description("The index of the image associated with this docking window at MDI Child state.")
         ]
        public void SetMDIChildIcon(Control ctrl, int index)
        {
            
            if (this.dockingMgrDesigner.bReloading == false)
                this.dockingMgr.SetMDIChildIcon(ctrl, index);
        }


		[
		Category("Syncfusion Docking"),
		DefaultValue(false),
		RefreshProperties(RefreshProperties.All),
		Description("Makes the docking window a float-only control.")
		]
		public void SetFloatOnly(Control ctrl, bool bfloat)
		{
			if(this.dockingMgrDesigner.bReloading == false)
				this.dockingMgr.SetFloatOnly(ctrl, bfloat);
		}

		[
		Category("Syncfusion Docking"),
		DefaultValue(false),
		RefreshProperties(RefreshProperties.All),
		Description("Makes the docking window a float-only control.")
		]
		public bool GetFloatOnly(Control ctrl)
		{
			return this.dockingMgr.GetFloatOnly(ctrl);
		}

		[
		Category("Syncfusion Docking"),
		DefaultValue(false),
		RefreshProperties(RefreshProperties.All),
		EditorBrowsable(EditorBrowsableState.Never),
		Description("Specifies whether the docking window should be in the autohide mode on application startup.")
		]
		public void SetAutoHideOnLoad(Control ctrl, bool bautohide)
		{
			if(this.dockingMgrDesigner.bReloading == false)
				this.dockingMgr.SetAutoHideOnLoad(ctrl, bautohide);
		}

		[
		Category("Syncfusion Docking"),
		DefaultValue(false),
		RefreshProperties(RefreshProperties.All),
		EditorBrowsable(EditorBrowsableState.Never),
		Description("Specifies whether the docking window should be in the autohide mode on application startup.")
		]
		public bool GetAutoHideOnLoad(Control ctrl)
		{
			return this.dockingMgr.GetAutoHideOnLoad(ctrl);
		}

		[
		Category("Syncfusion Docking"),
		DefaultValue(false),
		RefreshProperties(RefreshProperties.All),
		EditorBrowsable(EditorBrowsableState.Never),
		Description("Specifies whether the docking window should be hidden on application startup.")
		]
		public void SetHiddenOnLoad(Control ctrl, bool bhidden)
		{
			if(this.dockingMgrDesigner.bReloading == false)
				this.dockingMgr.SetHiddenOnLoad(ctrl, bhidden);
		}

		[
		Category("Syncfusion Docking"),
		DefaultValue(false),
		RefreshProperties(RefreshProperties.All),
		EditorBrowsable(EditorBrowsableState.Never),
		Description("Specifies whether the docking window should be hidden on application startup.")
		]
		public bool GetHiddenOnLoad(Control ctrl)
		{
			return this.dockingMgr.GetHiddenOnLoad(ctrl);
		}

		[
		Category("Syncfusion Docking"),
		DefaultValue("All"),
		RefreshProperties(RefreshProperties.All),
		Description("Indicates where user can dock in this control using drag providers (Whidbey and VS2005 drag providers only).")
		]
		public void SetDockAbility(Control ctrl, string strAbility)
		{
            DockAbility ability = (DockAbility)Enum.Parse( typeof(DockAbility), strAbility );
			this.dockingMgr.SetDockAbility(ctrl, ability);
			this.dockingMgr.UpdateDesigner();
		}

		[
		Category("Syncfusion Docking"),
		DefaultValue("All"),
		Editor(typeof(DockAbilityEditor), typeof(System.Drawing.Design.UITypeEditor)),
		RefreshProperties(RefreshProperties.All),
		Description("Indicates where user can dock in this control using drag providers (Whidbey and VS2005 drag providers only).")
		]
		public string GetDockAbility(Control ctrl)
		{
			return this.dockingMgr.GetDockAbility(ctrl).ToString();
		}

		[
		Category("Syncfusion Docking"),
		DefaultValue("All"),
		RefreshProperties(RefreshProperties.All),
		Description("Indicates where user can dock this control using drag providers (Whidbey and VS2005 drag providers only).")
		]
		public void SetOuterDockAbility(Control ctrl, string strAbility)
		{
            DockAbility ability = (DockAbility)Enum.Parse( typeof(DockAbility), strAbility );
			this.dockingMgr.SetOuterDockAbility(ctrl, ability);
			this.dockingMgr.UpdateDesigner();
		}

		[
		Category("Syncfusion Docking"),
		DefaultValue("All"),
		Editor(typeof(DockAbilityEditor), typeof(System.Drawing.Design.UITypeEditor)),
		RefreshProperties(RefreshProperties.All),
		Description("Indicates where user can dock this control using drag providers (Whidbey and VS2005 drag providers only).")
		]
		public string GetOuterDockAbility(Control ctrl)
		{
			return this.dockingMgr.GetOuterDockAbility(ctrl).ToString();
		}

		[
		Category("Syncfusion Docking"),
		Description("Contains custom caption buttons collection for each docked control.")
		]
		public void SetCustomCaptionButtons(Control ctrl, CaptionButtonsCollection buttons)
		{
			this.dockingMgr.SetCustomCaptionButtons( ctrl, buttons );
		}

		[
		Category("Syncfusion Docking"),
		Description("Contains custom caption buttons collection for each docked control.")
		]
		public CaptionButtonsCollection GetCustomCaptionButtons(Control ctrl)
		{
			return this.dockingMgr.GetCustomCaptionButtons(ctrl);
		}
	}

	#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
	internal sealed class KeysCodeDomSerializer	: CodeDomSerializer
	{
		public override object Deserialize(IDesignerSerializationManager manager, object codeobject)
		{
			CodeDomSerializer baseclassserializer = (CodeDomSerializer)manager.
				GetSerializer(typeof(DockingManager).BaseType, typeof(CodeDomSerializer));
			return baseclassserializer.Deserialize(manager, codeobject);
		}

		public override object Serialize(IDesignerSerializationManager manager, object value)
		{
			return new CodeCastExpression( typeof(Keys), 
				new CodeSnippetExpression( "System.Enum.Parse(typeof(System.Windows.Forms.Keys), \"" + 
				((System.Windows.Forms.Keys) value).ToString() + "\")" ));
		}
	}
	#endif
    /// <exclude/>
	public sealed class CaptionButtonsCodeDomSerializer : CodeDomSerializer
	{
		public override object Deserialize(IDesignerSerializationManager manager, object codeobject)
		{
			CodeDomSerializer baseclassserializer = (CodeDomSerializer)manager.
				GetSerializer(typeof(DockingManager).BaseType, typeof(CodeDomSerializer));
			return baseclassserializer.Deserialize(manager, codeobject);
		}


		public override object Serialize(IDesignerSerializationManager manager, object value)
		{
			CaptionButtonsCollection cbCollection = value as CaptionButtonsCollection;
			if (cbCollection != null && cbCollection.NeedSerializeToolTipInfo())
			{
				CodeDomSerializer baseClassSerializer = (CodeDomSerializer)manager.
				GetSerializer(typeof(CaptionButtonsCollection).BaseType, typeof(CodeDomSerializer));

				return baseClassSerializer.Serialize(manager, value);
			}
			else
			{
				return new CodeStatementCollection();
			}
		}

	}
    /// <exclude/>
	public sealed class DockingManagerCDS : CodeDomSerializer
	{
		public override object Deserialize(IDesignerSerializationManager manager, object codeobject)
		{
			CodeDomSerializer baseclassserializer = (CodeDomSerializer)manager.
				GetSerializer(typeof(DockingManager).BaseType, typeof(CodeDomSerializer));
			return baseclassserializer.Deserialize(manager, codeobject);
		}

		public override object Serialize(IDesignerSerializationManager manager, object value)
		{
			CodeDomSerializer baseClassSerializer = (CodeDomSerializer)manager.
				GetSerializer(typeof(DockingManager).BaseType, typeof(CodeDomSerializer));

			object codeobject = baseClassSerializer.Serialize(manager, value);

			// Custom serialization of the Extended Properties provided by the external DockPropertiesExtender
			// property provider component.
			if(codeobject is CodeStatementCollection)
			{
				DockingManager dmgr = value as DockingManager;
				IDockingManagerDesignerInvoke iinvoke = dmgr as IDockingManagerDesignerInvoke;
				CodeStatementCollection statements = (CodeStatementCollection)codeobject;

				// Check if caption buttons were already serialized.
				bool bCaptionButtonsCreated = false;
				for (int i = 0; i < statements.Count; i++)
				{
					CodeVariableDeclarationStatement cvds = statements[i] as CodeVariableDeclarationStatement;
					if (cvds != null)
					{
						if (cvds.Type.BaseType == "CaptionButton")
						{
							bCaptionButtonsCreated = true;
							break;
						}
					}
				}

				// Serialize caption buttons collection.
				CodeFieldReferenceExpression cfrDM =
						new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
						TypeDescriptor.GetComponentName(dmgr));
				CodePropertyReferenceExpression cprTarget =
						new CodePropertyReferenceExpression(cfrDM, "CaptionButtons");
				if (!bCaptionButtonsCreated && dmgr.CaptionButtons != null && dmgr.CaptionButtons != dmgr.GetDefaultCaptionButtons() && !dmgr.CaptionButtons.NeedSerializeToolTipInfo())
				{
					if (dmgr.CaptionButtons.Count > 0)
					{
						// Add to collection CaptionButtons. Need in VS2003 only.
						foreach (CaptionButton button in dmgr.CaptionButtons)
						{
							CodeMethodInvokeExpression cmiexp = SerializeCaptionButton(button, cprTarget);
							statements.Add(cmiexp);
						}
					}
				}

				// Serialize extended properties.

				ArrayList alcontrollers = iinvoke.GetControllerList();
				ArrayList alinherited = iinvoke.GetInheritedControlsList();
				foreach(DockControllerBase dcbase in alcontrollers)
				{
					if(dcbase is DockHostController)
					{
						DockHostController dhc = dcbase as DockHostController;
						Control clientcontrol = dhc.HostControl.Controls[0];

						// DockLabel extended property
						String strlabel = dmgr.GetDockLabel(clientcontrol);
						if((strlabel != String.Empty) || (alinherited.Contains(clientcontrol)))
						{
							// Target CodeExpression
							CodeFieldReferenceExpression cfrtarget =
								new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName(dmgr));

							// Parameters CodeExpression
							CodeExpression[] ceparams = new CodeExpression[2];
							ceparams[0] = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName(clientcontrol));
							ceparams[1] = new CodePrimitiveExpression(strlabel);

							// Method Invocation
							CodeMethodInvokeExpression cmiexp = new CodeMethodInvokeExpression(cfrtarget, "SetDockLabel", ceparams);

							statements.Add(cmiexp);
						}

						// DockIcon extended property
						int nicon = dmgr.GetDockIcon(clientcontrol);
                        int mIcon = dmgr.GetMDIChildIcon(clientcontrol);
                        if ((nicon != -1) || (alinherited.Contains(clientcontrol)) || (mIcon != -1))
						{
							// Target CodeExpression
							CodeFieldReferenceExpression cfrtarget =
								new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName(dmgr));
                            if (nicon != -1)
                            {
                                // Parameters CodeExpression
                                CodeExpression[] ceparams = new CodeExpression[2];
                                ceparams[0] = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                                    TypeDescriptor.GetComponentName(clientcontrol));
                                ceparams[1] = new CodePrimitiveExpression(nicon);

                                // Method Invocation
                                CodeMethodInvokeExpression cmiexp = new CodeMethodInvokeExpression(cfrtarget, "SetDockIcon", ceparams);
                                
                                statements.Add(cmiexp);
                            }

                            if (mIcon != -1)
                            {
                                // Parameters CodeExpression
                                CodeExpression[] ceparam = new CodeExpression[2];
                                ceparam[0] = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                                    TypeDescriptor.GetComponentName(clientcontrol));
                                ceparam[1] = new CodePrimitiveExpression(mIcon);

                                // Method Invocation
                                CodeMethodInvokeExpression  cmiexp = new CodeMethodInvokeExpression(cfrtarget, "SetMDIChildIcon", ceparam);
                                statements.Add(cmiexp);
                            }							
						}

						// FloatOnly extended property
						bool bfloatonly = dmgr.GetFloatOnly(clientcontrol);
						if((bfloatonly != false) || (alinherited.Contains(clientcontrol)))
						{
							// Target CodeExpression
							CodeFieldReferenceExpression cfrtarget =
								new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName(dmgr));

							// Parameters CodeExpression
							CodeExpression[] ceparams = new CodeExpression[2];
							ceparams[0] = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName(clientcontrol));
							ceparams[1] = new CodePrimitiveExpression(bfloatonly);

							// Method Invocation
							CodeMethodInvokeExpression cmiexp = new CodeMethodInvokeExpression(cfrtarget, "SetFloatOnly", ceparams);

							statements.Add(cmiexp);
						}

						// AllowFloating extended property
						bool ballowfloating = dmgr.GetAllowFloating( clientcontrol );
						if( ( ballowfloating != true ) || ( alinherited.Contains( clientcontrol ) ) )
						{
							// Target CodeExpression
							CodeFieldReferenceExpression cfrtarget =
								new CodeFieldReferenceExpression( new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName( dmgr ) );

							// Parameters CodeExpression
							CodeExpression[] ceparams = new CodeExpression[2];
							ceparams[0] = new CodeFieldReferenceExpression( new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName( clientcontrol ) );
							ceparams[1] = new CodePrimitiveExpression( ballowfloating );

							// Method Invocation
							CodeMethodInvokeExpression cmiexp = new CodeMethodInvokeExpression( cfrtarget, "SetAllowFloating", ceparams );

							statements.Add( cmiexp );
						}
                        // EnableDocking extended property
                        bool benableDocking = dmgr.GetEnableDocking(clientcontrol);
                        if ((benableDocking == true) || (alinherited.Contains(clientcontrol)))
                        {
                            // Target CodeExpression
                            CodeFieldReferenceExpression cfrtarget =
                                new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                                TypeDescriptor.GetComponentName(dmgr));

                            // Parameters CodeExpression
                            CodeExpression[] ceparams = new CodeExpression[2];
                            ceparams[0] = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
                                TypeDescriptor.GetComponentName(clientcontrol));
                            ceparams[1] = new CodePrimitiveExpression(benableDocking);

                            // Method Invocation
                            CodeMethodInvokeExpression cmiexp = new CodeMethodInvokeExpression(cfrtarget, "SetEnableDocking", ceparams);

                            statements.Add(cmiexp);
                        }
						// AutoHideOnLoad extended property
						bool bautohide = dmgr.GetAutoHideOnLoad(clientcontrol);
						if((bautohide != false) || (alinherited.Contains(clientcontrol)))
						{
							// Target CodeExpression
							CodeFieldReferenceExpression cfrtarget =
								new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName(dmgr));

							// Parameters CodeExpression
							CodeExpression[] ceparams = new CodeExpression[2];
							ceparams[0] = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName(clientcontrol));
							ceparams[1] = new CodePrimitiveExpression(bautohide);

							// Method Invocation
							CodeMethodInvokeExpression cmiexp = new CodeMethodInvokeExpression(cfrtarget, "SetAutoHideOnLoad", ceparams);

							statements.Add(cmiexp);
						}

						// DockVisibilityOnLoad extended property
						bool bhidden = dmgr.GetHiddenOnLoad(clientcontrol);
						if((bhidden == true) || (alinherited.Contains(clientcontrol)))
						{
							// Target CodeExpression
							CodeFieldReferenceExpression cfrtarget =
								new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName(dmgr));

							// Parameters CodeExpression
							CodeExpression[] ceparams = new CodeExpression[2];
							ceparams[0] = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName(clientcontrol));
							ceparams[1] = new CodePrimitiveExpression(bhidden);

							// Method Invocation
							CodeMethodInvokeExpression cmiexp = new CodeMethodInvokeExpression(cfrtarget, "SetHiddenOnLoad", ceparams);

							statements.Add(cmiexp);
						}

						// DockVisibilityOnLoad extended property
						bool freezeresize = dmgr.GetFreezeResize(clientcontrol);
						if( ( freezeresize == true ) || ( alinherited.Contains(clientcontrol) ) )
						{
							// Target CodeExpression
							CodeFieldReferenceExpression cfrtarget =
								new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName(dmgr));

							// Parameters CodeExpression
							CodeExpression[] ceparams = new CodeExpression[2];
							ceparams[0] = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName(clientcontrol));
							ceparams[1] = new CodePrimitiveExpression(freezeresize);

							// Method Invocation
							CodeMethodInvokeExpression cmiexp = new CodeMethodInvokeExpression(cfrtarget, "SetFreezeResize", ceparams);

							statements.Add(cmiexp);
						}

						// DockAbility extended property
						DockAbility ability = dmgr.GetDockAbility(clientcontrol);
						if( ability != DockAbility.All || (alinherited.Contains(clientcontrol)) )
						{
							// Target CodeExpression
							CodeFieldReferenceExpression cfrtarget =
								new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName(dmgr));

							// Parameters CodeExpression
							CodeExpression[] ceparams = new CodeExpression[2];
							ceparams[0] = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName(clientcontrol));
                            ceparams[1] = new CodePrimitiveExpression( ability.ToString() );

							// Method Invocation
							CodeMethodInvokeExpression cmiexp = new CodeMethodInvokeExpression(cfrtarget, "SetDockAbility", ceparams);

							statements.Add(cmiexp);
						}

						// OuterDockAbility extended property
						DockAbility outerAbility = dmgr.GetOuterDockAbility(clientcontrol);
						if( outerAbility != DockAbility.All || (alinherited.Contains(clientcontrol)) )
						{
							// Target CodeExpression
							CodeFieldReferenceExpression cfrtarget =
								new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName(dmgr));

							// Parameters CodeExpression
							CodeExpression[] ceparams = new CodeExpression[2];
							ceparams[0] = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName(clientcontrol));
                            ceparams[1] = new CodePrimitiveExpression( outerAbility.ToString() );

							// Method Invocation
							CodeMethodInvokeExpression cmiexp = new CodeMethodInvokeExpression(cfrtarget, "SetOuterDockAbility", ceparams);

							statements.Add(cmiexp);
						}

						// CustomCaptionButtons extended property
						if ( dmgr.CaptionButtons != null )
						{
							string ccbName = "ccb" + TypeDescriptor.GetComponentName(clientcontrol);
							CodeExpression[] ceTmpCollectionParams = { };
							CodeObjectCreateExpression cocTmpCollection = new CodeObjectCreateExpression( typeof( CaptionButtonsCollection ), ceTmpCollectionParams );
							CodeVariableDeclarationStatement cvdsCollection = new CodeVariableDeclarationStatement(typeof(CaptionButtonsCollection), ccbName, cocTmpCollection);
							statements.Add(cvdsCollection);
							CodeVariableReferenceExpression cvreCollection = new CodeVariableReferenceExpression(ccbName);

							CaptionButtonsCollection cbCtrl = dmgr.GetCustomCaptionButtons( clientcontrol );
							if (cbCtrl != null)
							{
								CaptionButtonsCollection cbTemp = cbCtrl.Clone();
								cbTemp.ExcludeCommonButtonsWith(dmgr.CaptionButtons);
                                if (dmgr.CaptionButtons.Count == 0)
                                {
                                    dmgr.m_IsCaptionButtonsCleared = true;
                                    dmgr.SaveCaptionButtionsClearedState();
                                }

								CodeExpression[] ceMergeParams = new CodeExpression[2];
								ceMergeParams[0] = cprTarget;
								ceMergeParams[1] = new CodePrimitiveExpression(false);
								CodeMethodInvokeExpression cmieMerge = new CodeMethodInvokeExpression(cvreCollection, "MergeWith", ceMergeParams);
								statements.Add(cmieMerge);

								for (int i = 0; i < cbTemp.Count; i++)
								{
									CodeMethodInvokeExpression cmiexp = SerializeCaptionButton(cbTemp[i], cvreCollection);
									statements.Add(cmiexp);
								}

								CodeExpression[] ceSetCollection = new CodeExpression[2];
								ceSetCollection[0] = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName(clientcontrol));
								ceSetCollection[1] = cvreCollection;
								CodeFieldReferenceExpression cfrtarget =
								new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
								TypeDescriptor.GetComponentName(dmgr));
								CodeMethodInvokeExpression cmieSetCollection = new CodeMethodInvokeExpression( cfrtarget, "SetCustomCaptionButtons", ceSetCollection );
								statements.Add(cmieSetCollection);
							}
						}						
					}
				}
            }
			return codeobject;
		}

		private CodeMethodInvokeExpression SerializeCaptionButton(CaptionButton button, CodeExpression ceTarget)
		{
			CodeExpression[] ceparams = new CodeExpression[1];
			CodeFieldReferenceExpression cfreType = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(CaptionButtonType)), button.Type.ToString());
			CodePrimitiveExpression cpeName = new CodePrimitiveExpression(button.Name);
			CodeExpression[] ceConstructorParams;
			if (CaptionButton.HasDefaultValue(button))
			{
				ceConstructorParams = new CodeExpression[2];
				ceConstructorParams[0] = cfreType;
				ceConstructorParams[1] = cpeName;
			}
			else
			{
				ceConstructorParams = new CodeExpression[5];
				ceConstructorParams[0] = cfreType;
				ceConstructorParams[1] = cpeName;
				ceConstructorParams[2] = new CodePrimitiveExpression(button.ImageIndex);
				Color clr = button.TransparentImageColor;
				if (clr == Color.Transparent)
				{
					ceConstructorParams[3] = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Color)), "Transparent");
				}
				else
				{
					CodeExpression[] clrparams = new CodeExpression[4];
					clrparams[0] = new CodePrimitiveExpression(clr.A);
					clrparams[1] = new CodePrimitiveExpression(clr.R);
					clrparams[2] = new CodePrimitiveExpression(clr.G);
					clrparams[3] = new CodePrimitiveExpression(clr.B);
					CodeTypeReferenceExpression ctreColor = new CodeTypeReferenceExpression(typeof(Color));
					ceConstructorParams[3] = new CodeMethodInvokeExpression(ctreColor, "FromArgb", clrparams);
				}
				ceConstructorParams[4] = new CodePrimitiveExpression(button.ToolTip);
			}
			CodeObjectCreateExpression cocButton = new CodeObjectCreateExpression(typeof(CaptionButton), ceConstructorParams);
			ceparams[0] = cocButton;

			// Method Invocation
			CodeMethodInvokeExpression cmiexp = new CodeMethodInvokeExpression(ceTarget, "Add", ceparams);
			return cmiexp;
		}
	}


	public class DockingManagerTypeConverter : TypeConverter
	{
		public DockingManagerTypeConverter()
		{
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(typeof(Syncfusion.Windows.Forms.Tools.DockingManager), attributes);
			if((context != null) && (context.Instance is DockingManager))
			{
				try
				{
					ArrayList props = new ArrayList();
					for(int i=0;i<pdc.Count;i++)
					{
						//if(pdc[i].DisplayName!="AllowSelection" && pds[i].DisplayName != "Culture" && pds[i].DisplayName != "SizeToFit")
						if(pdc[i].DisplayName != "PersistState")
						{
							props.Add(pdc[i]);
						}
					}
					pdc = new PropertyDescriptorCollection(props.ToArray(typeof(PropertyDescriptor)) as PropertyDescriptor[]);
				}
				catch{}
			}
			return pdc;
		}
	}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
	internal class ControlNativeWindow : NativeWindow
	{
		protected override void WndProc( ref Message m )
		{
			if( ( m.Msg != NativeMethods.WM_MOUSEMOVE ) || ( ( ( int )m.WParam ) != NativeMethods.MK_LBUTTON ) )
			{
				base.WndProc( ref m );
			}
		}
	}
#endif
}
