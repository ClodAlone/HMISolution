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
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Text;
using System.Diagnostics;

using System.Collections;
using System.Drawing.Drawing2D;
using System.Data;
using System.Reflection;
using Microsoft.Win32;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.CodeDom;
using System.IO;

using Syncfusion;
using Syncfusion.ComponentModel;
using Syncfusion.Collections;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms.Tools.XPMenus;
using Syncfusion.ComponentModel.Design.Serialization;
using Syncfusion.Windows.Forms.Design;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System.Runtime.InteropServices.ComTypes;
using EnvDTE;
using System.Windows.Forms.Design.Behavior;
#endif

namespace Syncfusion.Windows.Forms.Tools.Design
{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
	internal class EnvDte
	{
		#region Imported methods
		// these methods are used in getting currently running VS2003 instance.
		[DllImport("ole32.dll")]
		private static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);
		[DllImport("ole32.dll")]
		private static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);
		#endregion

		private const string DEF_VS_IDE_ID_FORMAT = "!VisualStudio.DTE.8.0:{0}";

		public static DTE GetDesignTimeEnvironment()
		{
			string progID = "VisualStudio.DTE.8.0";

			DTE result = (DTE)Marshal.GetActiveObject(progID);
			return result;
		}


		/// <summary>
		/// Gets current Dte from running object table( ROT ).
		/// </summary>
		public static DTE GetCurrentDTE()
		{
			IntPtr numFetched = IntPtr.Zero;
			IRunningObjectTable runningObjectTable = null;
			IEnumMoniker monikerEnumerator = null;
			IBindCtx ctx = null;
			IMoniker[] monikers = new IMoniker[1];
			string runningObjectName = null;
			object runningObjectVal = null;

			System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();

			// get current dte Name
			string dteName = string.Format(DEF_VS_IDE_ID_FORMAT, currentProcess.Id);

			GetRunningObjectTable(0, out runningObjectTable);
			runningObjectTable.EnumRunning(out monikerEnumerator);
			monikerEnumerator.Reset();

			// iterate through ROT and search for DTE, and with Name
			while (monikerEnumerator.Next(1, monikers, numFetched) == 0)
			{
				CreateBindCtx(0, out ctx);

				monikers[0].GetDisplayName(ctx, null, out runningObjectName);

				// found!
				if (string.Compare(runningObjectName, dteName, true) == 0)
				{
					runningObjectTable.GetObject(monikers[0], out runningObjectVal);
					break;
				}
			}

			DTE dte = runningObjectVal as DTE;

			return dte;
		}
	}
#endif

	/// <summary>
	/// Summary description for BarManagerDesigner.
	/// </summary>
	public class BarManagerDesigner : ComponentDesigner, IListenForMainFormVisibilityChange,
		IProvideController, IBarManagerDesigner, ICallWndProcListener, IGetMsgProcListener
	{		
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		private Adorner m_adorner = null;
#endif
		private IntPtr VSMainForm = IntPtr.Zero;
		private static Control LButtonDownControl = null;
		private static Point LButtonDownPoint = Point.Empty;
		private static bool bKeyBdEvent = false;

		private bool lastKnownCustState = false;
		private bool dontListenForCustomizationDone = false;
		private bool designerActive = true;
		private bool serializationBegun = false;
		//private IDesignerSerializationManager curSerManager = null;

        private BarManager m_manager;
		private DesignerVerbCollection verbs;
		private IComponentChangeService iComponentChangeService;
		private ISelectionService iSelectionService;
		internal IDesignerHost iDesignerHost;
		protected CBSerializationProvider cbSerProvider = null;
		protected CommandDockBarSerializationProvider m_cdbSerProvider = null;
		private ContainerInsertingSerializationProvider bmSerProvider = null;
		private SerializationListener serListener = null;
		internal IntPtr hVSMDIClient;
		private bool ininitialize = false;

        internal bool Active
        {
            get { return this.designerActive; }
        }

		public BarManagerDesigner()
		{
		}

		IntPtr IBarManagerDesigner.MdiClientWnd
		{
			get{return this.hVSMDIClient;}
		}


#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

		DesignerActionListCollection actionLists;

		public override DesignerActionListCollection ActionLists
		{
			get
			{
				if (null == actionLists)
				{
					actionLists = new DesignerActionListCollection();
					actionLists.Add(
						new BarManagerActionList(this.Component));
				}
				return actionLists;
			}
		}
#endif

		public override void Initialize(IComponent component)
		{
			this.ininitialize = true;
			base.Initialize(component);
			iComponentChangeService = (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
			iComponentChangeService.ComponentRemoving += new ComponentEventHandler
				(this.OnComponentRemoving);
			iComponentChangeService.ComponentRemoved += new ComponentEventHandler
				(this.OnComponentRemoved);
			iComponentChangeService.ComponentAdded += new ComponentEventHandler
				(this.OnComponentAdded);
			iComponentChangeService.ComponentChanged += new ComponentChangedEventHandler
				(this.OnComponentChanged);

			iDesignerHost = (IDesignerHost)this.GetService(typeof(IDesignerHost));

            m_manager = this.Component as BarManager;

			if(!this.VerifySingleInstance(false))
			{

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				if( m_manager != null )
				{
					m_manager.CustomDragChanged += new EventHandler( OnCustomDragChanged );
				}

				InitializeDragDrop();
#endif
				iDesignerHost.Activated += new System.EventHandler(this.OnActivated);
				iDesignerHost.Deactivated += new System.EventHandler(this.OnDeactivated);

				iDesignerHost.LoadComplete += new EventHandler(this.OnLoadComplete);

				if(iDesignerHost.Loading)
					this.SetupManager();
				else
					this.OnLoadComplete(null, EventArgs.Empty);

				IntPtr vsform = Syncfusion.Runtime.InteropServices.NativeMethods.GetForegroundWindow();
				if(vsform != IntPtr.Zero)
				{
					Syncfusion.Runtime.InteropServices.NativeMethods.EnumChildWindowsCallBack cb = new Syncfusion.Runtime.InteropServices.NativeMethods.EnumChildWindowsCallBack(this.EnumChildWindows_Callback);
					Syncfusion.Runtime.InteropServices.NativeMethods.EnumChildWindows(vsform, cb, (IntPtr)0);
				}

				iSelectionService = (ISelectionService)this.GetService(typeof(ISelectionService));
				iSelectionService.SelectionChanged += new EventHandler(this.SelectionChanged);

				IDesignerSerializationManager serManager
					= (IDesignerSerializationManager)this.GetService(typeof(IDesignerSerializationManager));
				// This is possible when within WinRes, for example.
				if(serManager != null &&
					// In 2.0, we cannot listen to this event unless the manager is currently serializing.
					(Syncfusion.Runtime.InteropServices.RuntimeEnvironment.MajorRuntimeVersion  < 2 || iDesignerHost.Loading))
					serManager.SerializationComplete += new EventHandler(this.OnSerializationComplete);

				iDesignerHost.TransactionClosed +=
					new DesignerTransactionCloseEventHandler(this.DesingerTransClosed);
				iDesignerHost.TransactionOpened +=
					new EventHandler(this.DesingerTransOpened);

				// Custom Serialization provider
				m_cdbSerProvider = new CommandDockBarSerializationProvider();
				this.cbSerProvider = new CBSerializationProvider();
				this.bmSerProvider = new ContainerInsertingSerializationProvider(typeof(BarManager), true);
				IDesignerSerializationManager idsm = (IDesignerSerializationManager)this.GetService(typeof(IDesignerSerializationManager));
				if(idsm != null)
				{
					idsm.AddSerializationProvider(m_cdbSerProvider);
					idsm.AddSerializationProvider(this.cbSerProvider);
					idsm.AddSerializationProvider(this.bmSerProvider);

					// Required only for 1.1 and above
					if(Syncfusion.Runtime.InteropServices.RuntimeEnvironment.MajorRuntimeVersion > 1
						|| (Syncfusion.Runtime.InteropServices.RuntimeEnvironment.MajorRuntimeVersion == 1 && Syncfusion.Runtime.InteropServices.RuntimeEnvironment.MinorRuntimeVersion > 0))
					{
						this.serListener = new SerializationListener(idsm);
						this.serListener.SerializationBegin += new EventHandler(this.SerializationBegin);
					}
				}
			}
			this.ininitialize = false;

			// Calling GetAncestor seems to break the designer sometimes (when opening a derived form
			// a tab control in VI mode doesn't respond to mouse down for tab switching!)
			// So, moving this to OnSerializationComplete.
			//this.VSMainForm = Syncfusion.Runtime.InteropServices.NativeMethods.GetAncestor(dmgr.Form.Handle, 3);
			DesignerHooks.AddGetMsgProcListener(this);
			DesignerHooks.AddCallWndProcListener(this);
		}

		public bool EnumChildWindows_Callback(IntPtr hwnd, IntPtr lparam)
		{
			if(hwnd != IntPtr.Zero)
			{
				StringBuilder strbuilder = new StringBuilder(100);
				Syncfusion.Runtime.InteropServices.NativeMethods.GetClassName(hwnd, strbuilder, 100);
				if(strbuilder.ToString() == "MDIClient")
				{
					this.hVSMDIClient = hwnd;
					return false;
				}
			}
			return true;
		}

		protected virtual bool VerifySingleInstance(bool destroyIfMultiple)
		{
			// Parse through all the components in the component collection of the designer
			ComponentCollection componentCollection = iDesignerHost.Container.Components;

			bool foundMultipleEntries = false;
			foreach(IComponent component in componentCollection)
			{
				if(component is BarManager && component != this.Component)
				{
					foundMultipleEntries = true;
					break;
				}
			}
			if(foundMultipleEntries && destroyIfMultiple)
			{
				MessageBox.Show("Cannot have more than one instance of ChildFrameBarManager or MainFrameBarManager. Deleting this new instance.", "Multiple BarManager instances warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
				iDesignerHost.DestroyComponent(this.Component);
			}
			return foundMultipleEntries;
		}

		public CommandBarController Controller
		{
			get
			{
				if(m_manager != null)
                    return m_manager.GetCommandBarManager().GetCommandBarController();
				else
					return null;
			}
		}

		object IBarManagerDesigner.GetService(Type serviceType)
		{
			return base.GetService(serviceType);
		}

		private void DesingerTransClosed(object sender, DesignerTransactionCloseEventArgs args)
		{

		}
		private void DesingerTransOpened(object sender, EventArgs args)
		{

		}
		private void SelectionChanged(object sender, EventArgs e)
		{
			if(m_manager != null)
			{
				BarItem item = iSelectionService.PrimarySelection as BarItem;

                if (m_manager.Items.Contains(item))
                    m_manager.CustomizingItem = item;
			}
		}

		private void Manager_CustomizingItem_Changed(object sender, EventArgs args)
		{
            BarItem item = m_manager.CustomizingItem;
			if(item != null && item.DesignMode)
				this.iSelectionService.
					SetSelectedComponents( new Object[1] { item }, SelectionTypes.Replace);
		}
		protected void CustomizationDone(object sender, EventArgs e)
		{
			if(!dontListenForCustomizationDone)
			{
				this.lastKnownCustState = false;
			}
		}

		// IMessageProcListener implementation
		public void GetMsgProc(int nCode, IntPtr wparam, IntPtr lparam)
		{
			Message msg = (Message)(Marshal.PtrToStructure(lparam, typeof(Message)));
			if( (msg.Msg == 0x0201 /*WM_LBUTTONDOWN*/) || (msg.Msg == 0x0203 /*WM_LBUTTONDBLCLK*/)
				|| (msg.Msg == 0x0200 /*WM_MOUSEMOVE*/) || (msg.Msg == 0x0202 /*WM_LBUTTONUP*/)	|| (msg.Msg==NativeMethods.WM_RBUTTONDOWN))
			{
				Control ctrlhit = Control.FromHandle(msg.HWnd);
				if(ctrlhit != null)
				{
					ICommandBarDesignerMouseHook idmh = ctrlhit as ICommandBarDesignerMouseHook;
                    if (ctrlhit is BarControlInternal && idmh == null)
                    {
                        BarControlInternal barControl = ctrlhit as BarControlInternal;
                        if (msg.Msg == NativeMethods.WM_RBUTTONDOWN || msg.Msg == NativeMethods.WM_LBUTTONDOWN)
                        {
                            barControl.CallWndProc(nCode, wparam, lparam);
                        }
                    }
					if(idmh != null)
					{
						Point ptscreen = Cursor.Position;
						if(msg.Msg == 0x0200 /*WM_MOUSEMOVE*/)
						{
							if(msg.WParam == (IntPtr)0x0001 /*MK_LBUTTON*/)
							{
								// A drag is about to commence. Synthesize the 'ESC' keyboard event to preempt designer band drawing.
								if(
									(BarManagerDesigner.LButtonDownControl != null) &&
									( ((ptscreen.X >= BarManagerDesigner.LButtonDownPoint.X-1) && (ptscreen.Y >= BarManagerDesigner.LButtonDownPoint.Y-1) &&
									(ptscreen.X <= BarManagerDesigner.LButtonDownPoint.X+1) && (ptscreen.Y <= BarManagerDesigner.LButtonDownPoint.Y+1))
									|| (BarManagerDesigner.bKeyBdEvent == true) )
									)
								{
									Syncfusion.Runtime.InteropServices.NativeMethods.keybd_event((byte)0x1B /*VK_ESCAPE*/, (byte)0, 0, IntPtr.Zero);
									Syncfusion.Runtime.InteropServices.NativeMethods.keybd_event((byte)0x1B /*VK_ESCAPE*/, (byte)0, 0x0002/*KEYEVENTF_KEYUP*/, IntPtr.Zero);
									if(BarManagerDesigner.bKeyBdEvent == true)
										BarManagerDesigner.bKeyBdEvent = false;
								}
								idmh.HandleMouseMove(MouseButtons.Left, ptscreen);
							}
							else
							{
								idmh.HandleMouseMove(MouseButtons.None, ptscreen);
							}
						}
						else if((msg.Msg == 0x0201 /*WM_LBUTTONDOWN*/) && (BarManagerDesigner.bKeyBdEvent == false))
						{
							// CommandBarExt uses native message processing
							if( (ctrlhit is CommandBarExt) ||
								((ctrlhit is CommandBarForm) && (ctrlhit.Controls.Count > 0) && (ctrlhit.Controls[0] is CommandBarExt)) )
								return;

							idmh.HandleMouseDown(MouseButtons.Left, ptscreen);
							BarManagerDesigner.LButtonDownControl = ctrlhit;
							BarManagerDesigner.LButtonDownPoint = ptscreen;
							BarManagerDesigner.bKeyBdEvent = true;
							if(Syncfusion.Runtime.InteropServices.NativeMethods.GetCapture() != ctrlhit.Handle)
								Syncfusion.Runtime.InteropServices.NativeMethods.SetCapture(ctrlhit.Handle);
						}
						else if(msg.Msg == 0x0202 /*WM_LBUTTONUP*/)
						{
							// CommandBarExt uses native message processing
							if( (ctrlhit is CommandBarExt) ||
								((ctrlhit is CommandBarForm) && (ctrlhit.Controls.Count > 0) && (ctrlhit.Controls[0] is CommandBarExt)) )
								return;

							idmh.HandleMouseUp(MouseButtons.Left, ptscreen);
							if(BarManagerDesigner.LButtonDownControl != null)
							{
								BarManagerDesigner.LButtonDownControl = null;
								BarManagerDesigner.LButtonDownPoint = Point.Empty;
								BarManagerDesigner.bKeyBdEvent = false;
								Syncfusion.Runtime.InteropServices.NativeMethods.ReleaseCapture();
							}
						}
						else if(msg.Msg == 0x0203 /*WM_LBUTTONDBLCLK*/)
						{
							// CommandBarExt uses native message processing
							if( (ctrlhit is CommandBarExt) ||
								((ctrlhit is CommandBarForm) && (ctrlhit.Controls.Count > 0) && (ctrlhit.Controls[0] is CommandBarExt)) )
								return;

							idmh.HandleDoubleClick(ptscreen);
						}
					}
				}
			}
		}


		private const int DEF_COLLAPSED = 30000;

		// ICallWndProcListener Implementation
		public void CallWndProc(int nCode, IntPtr wparam, IntPtr lparam)
		{
			Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT cwp =
				(Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT)(Marshal.PtrToStructure(lparam, typeof(Syncfusion.Runtime.InteropServices.NativeMethods.CWPSTRUCT)));
			if(nCode >= 0)
			{
				if((cwp.message == 0x0215 /*WM_CAPTURECHANGED*/) && (BarManagerDesigner.LButtonDownControl != null))
				{
					if(cwp.hwnd == BarManagerDesigner.LButtonDownControl.Handle)
						Syncfusion.Runtime.InteropServices.NativeMethods.SetCapture(BarManagerDesigner.LButtonDownControl.Handle);
				}

				if( cwp.message == NativeMethods.WM_NCCALCSIZE )
				{
					if( cwp.wParam != IntPtr.Zero )
					{
						NativeMethods.NCCALCSIZE_PARAMS param = 
							( NativeMethods.NCCALCSIZE_PARAMS )Marshal.PtrToStructure(
							cwp.lParam, typeof( NativeMethods.NCCALCSIZE_PARAMS ) );

						if( Math.Abs( param.rgrc0.left ) > DEF_COLLAPSED &&
							Math.Abs( param.rgrc1.left ) < DEF_COLLAPSED )
						{
							this.OnChangedVisibility( false );
						}
						else if( Math.Abs( param.rgrc0.left ) < DEF_COLLAPSED &&
							Math.Abs( param.rgrc1.left ) > DEF_COLLAPSED )
						{
							this.OnChangedVisibility( true );
						}
					}
				}

				if((cwp.message == 0x0018/*WM_SHOWWINDOW*/) || (cwp.message == 0x001C/*WM_ACTIVATEAPP*/))
				{
					if (cwp.hwnd == this.VSMainForm)
					{
						if (cwp.wParam != IntPtr.Zero )
						{
							if (cwp.lParam != IntPtr.Zero)
							{
								this.OnChangedVisibility(true);
							}
						}
						else
						{
							this.OnChangedVisibility(false);
						}
					}
				}
			}
		}

		public void OnChangedVisibility(bool visible)
		{
			if(m_manager == null)
				return;

			if(visible)
			{
				if(this.lastKnownCustState == true && this.designerActive)
					m_manager.Customize((System.ComponentModel.Design.IDesignerHost)this.GetService(typeof(System.ComponentModel.Design.IDesignerHost)));
			}
			else
			{
				if(m_manager.Customizing)
				{
					this.dontListenForCustomizationDone = true;
					m_manager.Customize(false);
					this.dontListenForCustomizationDone = false;

                    bool prevValue = m_manager.ShouldHidePopup;

					m_manager.ShouldHidePopup = true;
					m_manager.HidePopups();
					m_manager.ShouldHidePopup = prevValue;
				}
			}

			if(this.designerActive)
				m_manager.GetCommandBarManager().UpdateDesignTimeVisibility(visible);
		}

		protected void OnActivated(Object sender, EventArgs e)
		{
			designerActive = true;
			this.OnChangedVisibility(true);

			DesignerHooks.AddGetMsgProcListener(this);
			DesignerHooks.AddCallWndProcListener(this);
		}

		protected void OnDeactivated(Object sender, EventArgs e)
		{
			this.OnChangedVisibility(false);
			designerActive = false;

			DesignerHooks.RemoveGetMsgProcListener(this);
			DesignerHooks.RemoveCallWndProcListener(this);
		}

		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				BarManagerDesigner.bKeyBdEvent = false;
				BarManagerDesigner.LButtonDownControl = null;
				BarManagerDesigner.LButtonDownPoint = Point.Empty;
				DesignerHooks.RemoveGetMsgProcListener(this);
				DesignerHooks.RemoveCallWndProcListener(this);
				this.VSMainForm = IntPtr.Zero;

				if(this.iComponentChangeService != null)
				{
					iComponentChangeService.ComponentRemoving -= new ComponentEventHandler
						(this.OnComponentRemoving);
					iComponentChangeService.ComponentRemoving -= new ComponentEventHandler
						(this.OnComponentRemoved);
					iComponentChangeService.ComponentAdded -= new ComponentEventHandler
						(this.OnComponentAdded);
					iComponentChangeService.ComponentChanged -= new ComponentChangedEventHandler
						(this.OnComponentChanged);
				}

				if(this.iDesignerHost != null)
				{
					iDesignerHost.Activated -= new System.EventHandler(this.OnActivated);
					iDesignerHost.Deactivated -= new System.EventHandler(this.OnDeactivated);

					iDesignerHost.LoadComplete -= new EventHandler(this.OnLoadComplete);
				}

				if(this.iSelectionService != null)
					iSelectionService.SelectionChanged -= new EventHandler(this.SelectionChanged);

				IDesignerSerializationManager serManager
					= (IDesignerSerializationManager)this.GetService(typeof(IDesignerSerializationManager));
				if(serManager != null)
				{
					serManager.SerializationComplete -= new EventHandler(this.OnSerializationComplete);
					serManager.RemoveSerializationProvider(m_cdbSerProvider);
					serManager.RemoveSerializationProvider(this.cbSerProvider);
					serManager.RemoveSerializationProvider(this.bmSerProvider);
					if(this.serListener != null)
						serManager.RemoveSerializationProvider(this.serListener);
				}
			}

			base.Dispose(disposing);
		}

		private Form GetHostForm()
		{
			Form hostform = iDesignerHost.RootComponent as Form;
			if(hostform == null)
				throw( new ApplicationException("A BarManager can only be dropped on a Form Designer"));

			return hostform;
		}

		public override void DoDefaultAction()
		{
			if(Syncfusion.Runtime.InteropServices.RuntimeEnvironment.MajorRuntimeVersion > 1
				|| (Syncfusion.Runtime.InteropServices.RuntimeEnvironment.MajorRuntimeVersion == 1 && Syncfusion.Runtime.InteropServices.RuntimeEnvironment.MinorRuntimeVersion > 0))
			{
				this.OnSerializationComplete(this, EventArgs.Empty);
			}
			else
				base.DoDefaultAction();
		}

		private void SerializationBegin(object sender, EventArgs e)
		{
			if(this.serializationBegun)
				return;

			IDesignerSerializationManager dsm = (IDesignerSerializationManager )GetService(typeof(IDesignerSerializationManager) );
			if(dsm != null)
			{
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				dsm.SerializationComplete += new EventHandler( this.OnSerializationComplete );
#else
				DesignerSerializationManager dsmInstance = dsm as DesignerSerializationManager;

				if( null != dsmInstance )
				{
					dsmInstance.SessionCreated += new EventHandler( dsm_SessionCreated );
				}
#endif
			}

			this.serializationBegun = true;
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		private void dsm_SessionCreated( object sender, EventArgs e )
		{
			DesignerSerializationManager dsmInstance = (DesignerSerializationManager)sender;
			IDesignerSerializationManager dsm = (IDesignerSerializationManager)dsmInstance;

			dsmInstance.SessionCreated -= new EventHandler( dsm_SessionCreated );
			dsm.SerializationComplete += new EventHandler( this.OnSerializationComplete );
		}
#endif

		private void OnLoadComplete(object sender, EventArgs e)
		{
			if(m_manager.Form == null)
				this.SetupManager();
            if (!m_manager.Form.Visible)
			{
                m_manager.Form.VisibleChanged += new EventHandler(BarManagerForm_VisibleChanged);
			}
			else
			{
				m_manager.FormLoaded(null, EventArgs.Empty);
				m_manager.RefreshCommandBarsAfterDesignerLoad(false);
			}
		}

		protected void BarManagerForm_VisibleChanged(Object sender, EventArgs e)
		{
            Trace.Assert(m_manager.Form != null);
            if (m_manager.Form.Visible)
			{
                m_manager.Form.VisibleChanged -= new EventHandler(BarManagerForm_VisibleChanged);
                m_manager.FormLoaded(null, EventArgs.Empty);
                m_manager.RefreshCommandBarsAfterDesignerLoad(false);
			}
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		
		private void InitializeDragDrop()
		{
			IDesignerHost host = base.GetService( typeof( IDesignerHost ) ) as IDesignerHost;
			if( host != null )
			{
				BehaviorService BehaviorService = host.GetService( typeof( BehaviorService ) ) as BehaviorService;
				if( BehaviorService != null )
				{
					m_adorner = new Adorner();
					m_adorner.Enabled = false;

					DragDropBehavior dragDropBehavior = new DragDropBehavior();

					dragDropBehavior.DragDrop += new DragEventHandler( OnDragDrop );
					dragDropBehavior.DragOver += new DragEventHandler( OnDragOver );
					dragDropBehavior.DragLeave += new EventHandler( OnDragLeave );
					dragDropBehavior.MouseUp += new EventHandler( OnMouseUp );
					dragDropBehavior.MouseDown += new EventHandler( OnMouseDown );
					dragDropBehavior.MouseMove += new EventHandler( OnMouseMove );

					DragDropGlyph glyph = new DragDropGlyph( dragDropBehavior );
					m_adorner.Glyphs.Add( glyph );
					BehaviorService.Adorners.Add ( m_adorner );
				}
			}
		}

		private void OnCustomDragChanged( object sender, EventArgs e )
		{
            m_adorner.Enabled = (m_manager != null && m_manager.CustomDrag);
		}

		private void OnMouseMove( object sender, EventArgs e )
        {
            if (m_manager != null )
            {
                if (m_manager.CustomizingItem == null)
                {
                    m_manager.CustomDrag = false;
                }
            }
        }

        private void OnMouseDown( object sender, EventArgs e )
        {
            if( m_adorner.Enabled )
            {
                m_adorner.Enabled = false;

                if (m_manager != null)
                {
                    m_manager.CustomDrag = false;
                }
            }
        }

        private void OnMouseUp( object sender, EventArgs e )
        {
            if (m_manager != null)
            {
                BarControlInternal barControl = m_manager.GetBarControlForDragDrop();

                m_manager.CustomDrag = (m_manager.CustomDrag && barControl != null);
            }
        }

        private BarControlInternal lastEnteredBarControl = null;

        private void OnDragOver( object sender, DragEventArgs e )
        {
            if (m_manager != null)
            {
                BarControlInternal barControl = m_manager.GetBarControlForDragDrop();                

                if( lastEnteredBarControl != null && lastEnteredBarControl != barControl )
                {
                    lastEnteredBarControl.ProcessDragLeave( e );
                }

                if( barControl != null )
                {
                    barControl.ProcessDragOver( e );
                }

                lastEnteredBarControl = barControl;
            }
        }

		private void OnDragLeave( object sender, EventArgs e )
		{
            if (m_manager != null)
			{
                BarControlInternal barControl = m_manager.GetBarControlForDragDrop();

				if( barControl != null )
				{
					barControl.ProcessDragLeave( e );
				}
			}
		}

        private void OnDragDrop( object sender, DragEventArgs e )
        {
            if (m_manager != null)
            {
                BarControlInternal barControl = m_manager.GetBarControlForDragDrop();
                if( barControl != null )
                {
                    barControl.ProcessDragDrop( e );
                }

                m_manager.CustomDrag = false;
            }
        }
#endif

		protected virtual void SetupManager()
		{
            if (m_manager != null)
			{
                m_manager.Form = this.GetHostForm();

                m_manager.CustomizationDone += new EventHandler(this.CustomizationDone);
                m_manager.InitFromDesigner();

                m_manager.CustomizingItemChanged += new EventHandler(Manager_CustomizingItem_Changed);
			}
		}

		private void OnSerializationComplete(object sender, EventArgs args)
		{
			this.serializationBegun = false;

            if (null != m_manager && m_manager.Form == null)
			{
				IDesignerHost host = base.GetService( typeof( IDesignerHost ) ) as IDesignerHost;
				if( host != null )
				{
					m_manager.Form = host.RootComponent as Form;
				}
			}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            System.Diagnostics.Process currentProc = System.Diagnostics.Process.GetCurrentProcess();
            this.VSMainForm = currentProc.MainWindowHandle;
#else
			this.VSMainForm = Syncfusion.Runtime.InteropServices.NativeMethods.GetAncestor(m_manager.Form.Handle, 3);
#endif

			// Required for 1.1 and above
			if(Syncfusion.Runtime.InteropServices.RuntimeEnvironment.MajorRuntimeVersion > 1
				|| (Syncfusion.Runtime.InteropServices.RuntimeEnvironment.MajorRuntimeVersion == 1 && Syncfusion.Runtime.InteropServices.RuntimeEnvironment.MinorRuntimeVersion > 0))
			{
				if(!this.iDesignerHost.Loading && m_manager != null)
				{
                    m_manager.RefreshCommandBarsAfterDesignerLoad(true);
				}
			}
		}

		private void OnComponentChanged(object sender, ComponentChangedEventArgs args)
		{
			if( null != m_manager && args.Component == m_manager.Form &&
                args.Member != null && args.Member.Name == "IsMdiContainer")
			{
				IDesignerHost host = (IDesignerHost)sender;
				ReloadDesigner();
			}
		}

		private void OnComponentAdded(object sender, ComponentEventArgs args)
		{
			if(args.Component == this.Component)
			{
				this.VerifySingleInstance(true);
				return;
/*
				// TODO: Include this after fixing the TODO in InsertDockFilledPanel
				if(this.VerifySingleInstance(true) && this.Component is MainFrameBarManager)
				{
					Form hostForm = this.GetHostForm();
					if(hostForm != null)
					{
						if(!this.IsDockFilledContainerControlAvailable(hostForm))
						{
							string strInitMessage = "The MainFrameBarManager is initializing and is about to add toolbar docking areas to "
								+ hostForm.Name + ".\r\n"
								+ "It is recommended, for non-MDI forms only, that a container control with its 'Dock' property set to 'Fill' be added"
								+ "to the Form and that all child Controls be re-parented to it. This is not required but will help ensure proper"
								+ "positioning of child controls as the size of the dock area changes"
								+ "due to toolbar docking.\r\n"
								+ "Would you like to automatically add a Panel container control now.";
							DialogResult result = MessageBox.Show(strInitMessage, "MainFrameBarManager initialization", MessageBoxButtons.YesNo,
								MessageBoxIcon.Information);
							if(result == DialogResult.Yes)
							{
								// Insert a Panel between the child controls and and the Form.
								this.InsertDockFilledPanel(hostForm);
							}
						}
					}
				}
				*/
			}
		}
		private void InsertDockFilledPanel(Form form)
		{
			Panel panel = (Panel)this.iDesignerHost.CreateComponent(typeof(Panel));

			// Get the child controls array:
			Control[] childControls = new Control[form.Controls.Count];
			int i = 0;
			foreach(Control control in form.Controls)
				childControls[i++] = control;

			// Reparent the child controls to the Panel
			foreach(Control control in childControls)
			{
				// TODO: Prevent reparenting controls that are not visible to the designer (like the dock area).
				control.Parent = panel;
			}

			panel.Dock = DockStyle.Fill;

			form.Controls.Add(panel);
		}
		private bool IsDockFilledContainerControlAvailable(Form form)
		{
			foreach(Control control in form.Controls)
			{
				if(control is ContainerControl)
				{
					if(control.Dock == DockStyle.Fill)
						return true;
				}
			}
			return false;
		}

		private void OnComponentRemoved( object sender, ComponentEventArgs args )
		{
			CommandBar cmdBar = args.Component as CommandBar;
			MainFrameBarManager manager = this.Component as MainFrameBarManager;

			if( null != cmdBar && null != manager )
			{
				// Without this subsequent Menu Shows don't get Paint message!
				XPMenuGridFactory.ReleaseAllGrids();

				ArrayList detachedCmdBars = manager.DetachedCommandBars;

				if( detachedCmdBars.Count > 0 && detachedCmdBars.Contains(cmdBar) )
				{
					detachedCmdBars.Remove( args.Component );

					MessageBox.Show( "The CommandBar is removed from the \"DetachedCommandBars\" list. Undoing this delete will not add it back to the list. If you need to undo this delete, close the designer and undo in the corresponding source code file, if it is open.",
						"BarManager Designer Information" );
				}
			}
		}

		private void OnComponentRemoving(object sender, ComponentEventArgs args)
		{
			if(this.Component == null)
				return;

			if(args.Component is BarItem)
			{
				// Without this subsequent Menu Shows don't get Paint message!
				XPMenuGridFactory.ReleaseAllGrids();
				BarItem item = (BarItem)args.Component;
                m_manager.CustomizationDialog.RemoveReferencesToItem(item);

				if( !item.IsDisposing )
				{
					MessageBox.Show("All references to the deleted bar item in the CommandBars and Menus have been deleted. Undoing this Delete will not restore these references. However, closing the designer and the source file without saving the changes will restore the original settings.",
						"BarManager Designer Information");
				}
			}
			else if(args.Component is Bar)
			{
				// Without this subsequent Menu Shows don't get Paint message!
				XPMenuGridFactory.ReleaseAllGrids();
				Bar bar = (Bar)args.Component;
                m_manager.CustomizationDialog.RemoveReferencesToItem(bar);
				MessageBox.Show("The Bar has been deleted from the BarManager. Undoing this Delete will not restore the Bar. However, closing the designer and the source file without saving the changes will restore the original settings.",
					"BarManager Designer Information");
			}

			if((args.Component.Equals(this.Component)))
			{
				IDesignerSerializationManager idsm = (IDesignerSerializationManager)this.GetService(typeof(IDesignerSerializationManager));
				if(idsm != null)
				{
					idsm.RemoveSerializationProvider(m_cdbSerProvider);
					idsm.RemoveSerializationProvider(this.cbSerProvider);
					idsm.RemoveSerializationProvider(this.bmSerProvider);
				}
			}
		}

		private void ReloadDesigner()
		{
			IDesignerHost iDesignerHost = (IDesignerHost)this.GetService( typeof( IDesignerHost ) );
			if( iDesignerHost != null && !iDesignerHost.Loading )
			{
				IDesignerLoaderService iDesignerLoaderService = (IDesignerLoaderService)this.GetService( typeof( IDesignerLoaderService ) );
				if( iDesignerLoaderService != null )
				{
					IDesignerSerializationManager manager = (IDesignerSerializationManager)GetService( typeof( IDesignerSerializationManager ) );
					if( manager != null )
					{
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
						manager.SerializationComplete += new EventHandler( this.OnSerializationComplete );
#else
						DesignerSerializationManager dsmInstance = manager as DesignerSerializationManager;

						if( null != dsmInstance )
						{
							dsmInstance.SessionCreated += new EventHandler( dsm_SessionCreated );
						}
#endif
						bool successful = iDesignerLoaderService.Reload();
					}
				}
			}
		}

		public override DesignerVerbCollection Verbs
		{
			get
			{
				if (this.verbs == null)
				{
					this.verbs = new DesignerVerbCollection();
					this.verbs.Add(new DesignerVerb("Customize...",new EventHandler(this.OnCustomize)));
					this.verbs.Add(new DesignerVerb("Activate Menus",new EventHandler(this.OnActivateMenus)));
					if(this.Component is MainFrameBarManager)
					{
						this.verbs.Add(new DesignerVerb("Add Detached CommandBar", new EventHandler(this.OnAddDetachedCommandBar)));
						this.verbs.Add(new DesignerVerb("Add Detached ControlBar", new EventHandler(this.OnAddDetachedControlBar)));
					}
				}
				return this.verbs;
			}
		}

		private void OnAddDetachedCommandBar(object sender, EventArgs e)
		{
			CommandBar newCommandBar = (CommandBar)this.iDesignerHost.CreateComponent(typeof(CommandBar));

			MainFrameBarManager manager = this.Component as MainFrameBarManager;
			manager.DetachedCommandBars.Add(newCommandBar);
		}
		private void OnAddDetachedControlBar(object sender, EventArgs e)
		{
			ControlBar newControlBar = (ControlBar)this.iDesignerHost.CreateComponent(typeof(ControlBar));

			MainFrameBarManager manager = this.Component as MainFrameBarManager;
			manager.DetachedCommandBars.Add(newControlBar);
		}
		private void OnActivateMenus(object sender, EventArgs eevent)
		{
			this.OnSerializationComplete(this, EventArgs.Empty);
		}
		private void OnCustomize(object sender, EventArgs eevent)
		{
            if (m_manager.Form == null)
			{
				MessageBox.Show("Please assign a Form to the BarManager before Customizing it.");
				return;
			}

			// Subclass the VS Mainform
//			if(this.vsMainForm.Handle == IntPtr.Zero)
//			{
//				IntPtr hwndvsform = NativeMethods.GetAncestor(manager.Form.Handle, 3);
//				if(hwndvsform != IntPtr.Zero)
//					this.vsMainForm.AssignHandleCustom(hwndvsform);
//			}

			this.lastKnownCustState = true;
            m_manager.Customize((System.ComponentModel.Design.IDesignerHost)this.GetService(typeof(System.ComponentModel.Design.IDesignerHost)));

			if(Syncfusion.Runtime.InteropServices.RuntimeEnvironment.MajorRuntimeVersion > 1
				|| (Syncfusion.Runtime.InteropServices.RuntimeEnvironment.MajorRuntimeVersion == 1 && Syncfusion.Runtime.InteropServices.RuntimeEnvironment.MinorRuntimeVersion > 0))
			{
				this.OnSerializationComplete(this, EventArgs.Empty);
			}
			// Force it dirty for now
			//TODO: Not a good way since calling Changing and Changed simultaneouly
			//			this.SetDirty();
		}
		IDesignerHost IBarManagerDesigner.DesignerHost
		{
			get
			{
				return this.GetService(typeof(IDesignerHost)) as IDesignerHost;
			}
		}

		void IBarManagerDesigner.SetDirty()
		{
			System.ComponentModel.Design.IDesignerHost iDesignerHost;
			iDesignerHost = (System.ComponentModel.Design.IDesignerHost)this.GetService(typeof(System.ComponentModel.Design.IDesignerHost));

			if(this.ininitialize || iDesignerHost.Loading)
				return;

			System.ComponentModel.MemberDescriptor memberDescriptor;

			//			System.ComponentModel.Design.DesignerTransaction designerTransaction;
			//			System.ComponentModel.Design.CheckoutException checkoutException;

			memberDescriptor = (MemberDescriptor)TypeDescriptor.GetProperties((object)this.Component)[(string)"Bars"];
			if (iDesignerHost != null)
			{
				this.RaiseComponentChanged(memberDescriptor,null,null);

				//				designerTransaction = null;
				//				// Raise the RaiseComponentChanging and RaiseComponentChanged events
				//				try
				//				{
				//					try
				//					{
				//						designerTransaction = iDesignerHost.CreateTransaction(String.Concat((string)@"BarManager Property",this.Component.Site.Name));
				//						this.RaiseComponentChanging(memberDescriptor);
				//					}
				//					catch(System.ComponentModel.Design.CheckoutException exception)
				//					{
				//						checkoutException = exception;
				//						if (checkoutException == CheckoutException.Canceled)
				//							throw checkoutException;
				//					}
				//					this.RaiseComponentChanged(memberDescriptor,null,null);
				//				}
				//				finally
				//				{
				//					if (designerTransaction != null)
				//						designerTransaction.Commit();
				//				}
			}
		}
	}

	public class XPToolBarSerializationProvider : IDesignerSerializationProvider
	{
		BarControlCodeDomSerializer serializer;
		public XPToolBarSerializationProvider()
		{
			this.serializer = new BarControlCodeDomSerializer();
		}

		public virtual object GetSerializer(IDesignerSerializationManager manager, object currentSerializer,
			Type objectType, Type serializerType)
		{
			if(this.serializer.RemovedManager == false)
			{
				if( (objectType != null) && ((objectType == typeof(XPToolBar))
					|| (objectType.IsSubclassOf(typeof(XPToolBar)))) )
					return this.serializer;
			}
			return null;
		}

		~XPToolBarSerializationProvider()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				this.serializer = null;
			}
		}
	}

	public class BarControlCodeDomSerializer : CodeDomSerializer
	{
		private bool removedManager = false;

		public bool RemovedManager
		{
			get{return this.removedManager;}
		}

		public BarControlCodeDomSerializer()
		{
		}

		public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
		{
			CodeDomSerializer baseClassSerializer =
				(CodeDomSerializer)manager.GetSerializer(typeof(Control), typeof(CodeDomSerializer));

			return baseClassSerializer.Deserialize(manager, codeObject);
		}

		public override object Serialize(IDesignerSerializationManager manager, object obj)
		{
			if(this.removedManager == false && obj is XPToolBar)
			{
				XPToolBar barControl = obj as XPToolBar;
				this.removedManager = true;
				barControl.BeforeCodeDomSerialize();

				CodeDomSerializer baseclassserializer =
					(CodeDomSerializer)manager.GetSerializer(typeof(Control), typeof(CodeDomSerializer));
				Object codeobject = baseclassserializer.Serialize(manager, obj);

				barControl.AfterCodeDomSerialize();

				this.removedManager = false;
				return codeobject;
			}
			else
			{
				CodeDomSerializer baseclassserializer =
					(CodeDomSerializer)manager.GetSerializer(typeof(Control), typeof(CodeDomSerializer));
				return baseclassserializer.Serialize(manager, obj);
			}
		}
	}

	public class BarManagerCollectionEditor : CollectionEditor
	{
		public BarManagerCollectionEditor(Type type)
			:base(type)
		{
		}
		public override object EditValue(
			ITypeDescriptorContext context, IServiceProvider provider,
			object value)
		{
			BarManager manager = null;
			if(context.Instance is BarManager)
				manager = context.Instance as BarManager;
			else if(value is BarItems)
				manager = ((BarItems)value).Manager;
			else if(value is IntList && context.Instance is ParentBarItem)
			{
				manager = ((ParentBarItem)context.Instance).Manager;
			}

			if(manager != null)
			{
				IDesignerHost host = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host != null)
					manager.Customize(host);
				return value;
			}
			else
				return base.EditValue(context, provider, value);
		}
	}

	public class BarItemsCollectionEditor : BarManagerCollectionEditor
	{
		private Type[] types;
		// The base class has its own version of this property
		private CollectionForm collectionForm;

		private PropertyGridPopupMenu pgMenu;

		public BarItemsCollectionEditor(Type type)
			: base(type)
		{
			types = new Type[]{typeof(BarItem), typeof(ParentBarItem), typeof(DropDownBarItem), typeof(ComboBoxBarItem),
                typeof(ListBarItem), typeof(StaticBarItem), typeof(MdiListBarItem), typeof(ToolbarListBarItem), typeof(TextBoxBarItem)};
		}

		protected override Type[] CreateNewItemTypes()
		{
			return types;
		}

		public override object EditValue(
			ITypeDescriptorContext context, IServiceProvider provider,
			object value)
		{
			if(this.collectionForm != null && this.collectionForm.Visible)
			{
				BarItemsCollectionEditor editor = new BarItemsCollectionEditor(this.CollectionType);
				return editor.EditValue(context, provider, value);
			}
			else
			{
				return base.EditValue(context, provider, value);
			}
		}

		protected override CollectionForm CreateCollectionForm()
		{
			this.collectionForm = base.CreateCollectionForm();

			PropertyGrid pg = WinFormsUtils.GetPropertyGridInControl(this.collectionForm);

			if(pg != null)
				this.pgMenu = new PropertyGridPopupMenu(pg);

			return this.collectionForm;
		} // end of method CreateCollectionForm
	}
	public class BarItemDesigner : ComponentDesigner, IAllowMakeDirty
	{
		void IAllowMakeDirty.SetDirty()
		{
			System.ComponentModel.MemberDescriptor memberDescriptor;
			System.ComponentModel.Design.IDesignerHost iDesignerHost;

			memberDescriptor = (MemberDescriptor)TypeDescriptor.GetProperties((object)this.Component)[(string)"Items"];
			iDesignerHost = (System.ComponentModel.Design.IDesignerHost)this.GetService(typeof(System.ComponentModel.Design.IDesignerHost));
			if (iDesignerHost != null)
				this.RaiseComponentChanged(memberDescriptor,null,null);
		}
	}

	public class SerializationListener : IDesignerSerializationProvider
	{
		private IDesignerSerializationManager idsm;
		public SerializationListener(IDesignerSerializationManager idsm)
		{
			this.idsm = idsm;
			this.idsm.AddSerializationProvider(this);
		}

		public event EventHandler SerializationBegin;

		public virtual object GetSerializer(IDesignerSerializationManager manager, object currentSerializer,
			Type objectType, Type serializerType)
		{
			if(this.SerializationBegin != null)
				this.SerializationBegin(this, EventArgs.Empty);

			return null;
		}
	}
}
