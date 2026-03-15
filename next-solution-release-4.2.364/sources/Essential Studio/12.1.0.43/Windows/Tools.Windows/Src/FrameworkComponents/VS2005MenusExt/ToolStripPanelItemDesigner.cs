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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using EnvDTE;

using Syncfusion.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	#region ToolStripPanelItemDesigner
	class ToolStripPanelItemDesigner: IDesigner
	{
		#region IDesigner implementation

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		IComponent IDesigner.Component
		{
			get { return m_component; }
		}
		/// <summary>
		/// 
		/// </summary>
		DesignerVerbCollection IDesigner.Verbs
		{
			get { return null; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="component"></param>
		void IDesigner.Initialize( IComponent component )
		{
			m_component = component;

			if( component != null )
			{
				ToolStripExService tsSvc = ToolStripExService.Get( component.Site );
				if( tsSvc != null )
				{
					if( tsSvc.Designers != null )
					{
						Type type = Type.GetType( "System.Windows.Forms.Design.ToolStripItemDesigner, System.Design" );
						if( type != null )
						{
							ComponentDesigner designer = Activator.CreateInstance( type ) as ComponentDesigner;
							if( designer != null )
							{
								tsSvc.Designers[component] = designer;
								designer.Initialize( component );
							}
						}
					}
					tsSvc.AddItem( component as ToolStripPanelItem );
				}
				
				UpdateSerializationService( component );

				InitDTE();
			}
		}
		/// <summary>
		/// 
		/// </summary>
		void IDesigner.DoDefaultAction()
		{
		}
		#endregion

		#endregion

		#region IDisposable implementation
		/// <summary>
		/// 
		/// </summary>
		void IDisposable.Dispose()
		{
			m_component = null;
		}
		#endregion

		#region Implementation

		public static void UpdateSerializationService( IComponent component )
		{
			if( component.Site != null )
			{
				IServiceProvider serviceProvider = (IServiceProvider)component.Site;

				UpdateSerializationService( serviceProvider );
			}
		}

		public static void UpdateSerializationService( IServiceProvider serviceProvider )
		{
			IServiceContainer services = serviceProvider.GetService( typeof( IServiceContainer ) ) as IServiceContainer;

			if( services != null )
			{
				Type tService = typeof( ComponentSerializationService );
				ComponentSerializationService currentService = services.GetService( tService ) as ComponentSerializationService;

				if( currentService != null && !( currentService is PanelItemSerializationService ) )
				{
					services.RemoveService( tService );
					services.AddService( tService, new PanelItemSerializationService( currentService ) );
				}
			}
		}

		#endregion

		#region Fields
		protected IComponent m_component = null;
		#endregion

		#region ENVDTE

		#region Implementation

		static void InitDTE()
		{
			if (s_dte == null)
			{
				s_dte = GetCurrentDTE();

				if (s_dte != null)
				{
					s_dteEvents = s_dte.Events.DTEEvents;
					s_winEvents = s_dte.Events.get_WindowEvents(null);

					s_dteEvents.OnBeginShutdown += new _dispDTEEvents_OnBeginShutdownEventHandler(s_dteEvents_OnBeginShutdown);
					s_winEvents.WindowCreated += new _dispWindowEvents_WindowCreatedEventHandler(winEvents_WindowCreated);
					s_winEvents.WindowActivated += new _dispWindowEvents_WindowActivatedEventHandler(winEvents_WindowActivated);
				}
			}
		}

		private static void s_dteEvents_OnBeginShutdown()
		{
			s_dteEvents.OnBeginShutdown -= new _dispDTEEvents_OnBeginShutdownEventHandler(s_dteEvents_OnBeginShutdown);
			s_winEvents.WindowCreated -= new _dispWindowEvents_WindowCreatedEventHandler(winEvents_WindowCreated);
			s_winEvents.WindowActivated -= new _dispWindowEvents_WindowActivatedEventHandler(winEvents_WindowActivated);

			s_dte = null;
			s_dteEvents = null;
			s_winEvents = null;
		}

		private static void winEvents_WindowActivated(Window GotFocus, Window LostFocus)
		{
			ActivateDesignSerializationService(GotFocus);
		}

		private static void winEvents_WindowCreated(Window Window)
		{
			ActivateDesignSerializationService(Window);
		}

		private static void ActivateDesignSerializationService(Window Window)
		{
			IServiceProvider svcProvider = Window.Object as IServiceProvider;

			if (svcProvider != null)
			{
				ToolStripPanelItemDesigner.UpdateSerializationService(svcProvider);
			}
		}

		static private DTE GetCurrentDTE()
		{
			IRunningObjectTable runningObjectTable = null;
			IEnumMoniker monikerEnumerator = null;
			IBindCtx ctx = null;
			IMoniker[] monikers = new IMoniker[1];
			string runningObjectName = null;
			object runningObjectVal = null;

			System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
			string sPID = currentProcess.Id.ToString();
			NativeMethods.GetRunningObjectTable(0, out runningObjectTable);

			runningObjectTable.EnumRunning(out monikerEnumerator);
			monikerEnumerator.Reset();

			DTE dte = null;

			while (monikerEnumerator.Next(1, monikers, IntPtr.Zero) == 0)
			{
				NativeMethods.CreateBindCtx(0, out ctx);

				monikers[0].GetDisplayName(ctx, null, out runningObjectName);
				runningObjectTable.GetObject(monikers[0], out runningObjectVal);

				if (runningObjectName.StartsWith("!VisualStudio.DTE.") && runningObjectName.EndsWith(sPID))
				{
					dte = runningObjectVal as DTE;
					break;
				}
				else
				{
					SolutionClass sln = runningObjectVal as SolutionClass;

					if (sln != null)
					{
						int hMainWnd = sln.DTE.MainWindow.HWnd;
						uint pid;
						GetWindowThreadProcessId((IntPtr)hMainWnd, out pid);

						if (pid == currentProcess.Id)
						{
							dte = sln.DTE;
							break;
						}
					}
				}
			}

			return dte;
		}

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);

		#endregion

		#region Fields

		static DTE s_dte;
		static WindowEvents s_winEvents;
		static DTEEvents s_dteEvents;

		#endregion

		#endregion

		#region Nested classes
		class PanelItemSerializationService: ComponentSerializationService
		{
			#region Constructors
			public PanelItemSerializationService( ComponentSerializationService baseService )
				: base()
			{
				m_baseService = baseService;
			}
			#endregion

			#region Overrides
			public override SerializationStore CreateStore()
			{
				return m_baseService.CreateStore();
			}
			public override ICollection Deserialize( SerializationStore store )
			{
				return m_baseService.Deserialize( store );
			}
			public override ICollection Deserialize( SerializationStore store, IContainer container )
			{
				ICollection collection = m_baseService.Deserialize( store, container );
				return RemovePanelItems( collection );
			}
			public override void DeserializeTo( SerializationStore store, IContainer container, bool validateRecycledTypes, bool applyDefaults )
			{
				m_baseService.DeserializeTo( store, container, validateRecycledTypes, applyDefaults );
			}
			public override SerializationStore LoadStore( System.IO.Stream stream )
			{
				return m_baseService.LoadStore( stream );
			}
			public override void Serialize( SerializationStore store, object value )
			{
				m_baseService.Serialize( store, value );

				ToolStripPanelItem panelItem = value as ToolStripPanelItem;
				if( panelItem != null )
				{
					SerializePanelItems( store, panelItem.Items );
				}
			}
			public override void SerializeAbsolute( SerializationStore store, object value )
			{
				m_baseService.SerializeAbsolute( store, value );
			}
			public override void SerializeMember( SerializationStore store, object owningObject, MemberDescriptor member )
			{
				m_baseService.SerializeMember( store, owningObject, member );
			}
			public override void SerializeMemberAbsolute( SerializationStore store, object owningObject, MemberDescriptor member )
			{
				m_baseService.SerializeMemberAbsolute( store, owningObject, member );
			}
			#endregion

			#region Implementation
			/// <summary>
			/// 
			/// </summary>
			/// <param name="store"></param>
			/// <param name="items"></param>
			void SerializePanelItems( SerializationStore store, ICollection items )
			{
				foreach( ToolStripItem item in items )
				{
					m_baseService.Serialize( store, item );

					ToolStripPanelItem panelItem = item as ToolStripPanelItem;
					if( panelItem != null )
					{
						SerializePanelItems( store, panelItem.Items );
					}
					else
					{
						ToolStripDropDownItem dropDownItem = item as ToolStripDropDownItem;

						if( dropDownItem != null && dropDownItem.DropDown.IsAutoGenerated )
						{
							SerializePanelItems( store, dropDownItem.DropDownItems );
						}
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="items"></param>
			ICollection RemovePanelItems( ICollection items )
			{
				ArrayList result = new ArrayList( items );
				ICollection panelItems = GetPanelItems( items );

				foreach( ToolStripPanelItem panelItem in panelItems )
				{
					RemovePanelItems( result, panelItem.Items );
				}

				return result;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="items"></param>
			/// <param name="panelItems"></param>
			void RemovePanelItems( ArrayList items, ICollection panelItems )
			{
				foreach( ToolStripItem item in panelItems )
				{
					items.Remove( item );

					ToolStripDropDownItem dropDownItem = item as ToolStripDropDownItem;

					if( dropDownItem != null && dropDownItem.DropDown.IsAutoGenerated )
					{
						RemovePanelItems( items, dropDownItem.DropDownItems );
					}
				}
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="items"></param>
			/// <returns></returns>
			ICollection GetPanelItems( ICollection items )
			{
				ArrayList panelItems = new ArrayList();

				foreach( object obj in items )
				{
					if( obj is ToolStripPanelItem )
					{
						panelItems.Add( obj );
					}
				}
				return panelItems;
			}
			#endregion

			#region Fields
			ComponentSerializationService m_baseService;
			#endregion
		}
		#endregion
	}
	#endregion
}
#endif
