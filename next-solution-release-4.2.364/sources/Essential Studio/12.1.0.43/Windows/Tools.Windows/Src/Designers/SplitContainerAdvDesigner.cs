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

#region file using directives
using System;
using System.Text;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Diagnostics;
using System.ComponentModel.Design.Serialization;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using EnvDTE;

using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms.Tools.Events;
using Syncfusion.Runtime.InteropServices;
#endregion

namespace Syncfusion.Windows.Forms.Design
{
	public class SplitContainerAdvDesigner:
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		ParentControlDesigner
#else
		ControlDesigner
#endif
	{    
	    #region Class constants
		/// <summary>
		/// Splitter distance property name.
		/// </summary>
		private const string DEF_SPLITDISTANCE_PROP_NAME = "SplitterDistance";

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        private static string DEF_PANEL1_NAME = "Panel1";
        private static string DEF_PANEL2_NAME = "Panel2";
#else
		// these IDs are used to get appropriate menu items from VS2003 mainmenu.
		private const string DEF_MAINMENU_EDIT_COMMAND_GUID = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}";
		private const int DEF_MAINMENU_EDIT_CUT_COMMAND_ID = 16;
		private const int DEF_MAINMENU_EDIT_COPY_COMMAND_ID = 15;
		private const int DEF_MAINMENU_EDIT_PASTE_COMMAND_ID = 26;
		private const int DEF_MAINMENU_EDIT_DELETE_COMMAND_ID = 17;
		
		// Indexes are used to get appropriate menu items from VS2003 main menu.
		private const int DEF_MENUITEM_EDIT_INDEX = 2;
		private const int DEF_MENUITEM_COPY_INDEX = 4;
		private const int DEF_MENUITEM_PASTE_INDEX = 7;
		private const int DEF_MENUITEM_CUT_INDEX = 3;
		private const int DEF_MENUITEM_DELETE_INDEX = 10;

		private const string DEF_MAINMENUBAR_NAME = "MenuBar";
		
		/// <summary>
		/// Used to identify current running VS2003 IDE we are working with.
		/// </summary>
		private const string DEF_VS_IDE_ID_FORMAT = "!VisualStudio.DTE.7.1:{0}";
#endif
		#endregion
		//SmartTags added for .NET Framework 2.0        
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
						new SplitContainerAdvActionList(this.Component));
				}
				return actionLists;
			}
		}
#endif
		#region Class members
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// used to modify Currently running active Design Time Environment.
		/// </summary>
		private DTE m_currentDte = null;
		/// <summary>
		/// Indicates, enable all items in VS2003 main menu, or only some specified
		/// one's.
		/// </summary>
		private bool m_bEnableAll = false;
		/// <summary>
		/// Used to set enabled/disabled state in Edit menu item in VS2003 main menu.
		/// </summary>
		private Microsoft.Office.Core.CommandBarPopup m_editMenuItem = null;
#endif
		/// <summary>
		/// Split container, this designer is associated with.
		/// </summary>
		private SplitContainerAdv m_splitContainer = null;
		/// <summary>
		/// Used to handle components selection changed in VS003 designer
		/// to config correctly menu commands in VS2003 main menu, etc.
		/// </summary>
		private ISelectionService m_selectionService = null;
		/// <summary>
		/// Indicates, is designer active or not.
		/// </summary>
		private bool m_bIsActive = false;
		#endregion

		#region Class properties
		/// <summary>
		/// Gets selection service to handle components selection changed in 
		/// VS003 designer to config correctly menu commands in VS2003 main menu,
		/// etc.
		/// </summary>
		protected ISelectionService SelectionService
		{
			get
			{
				if( m_selectionService == null )
				{
					InitSelectionService();
				}

				return m_selectionService;
			}
		}

		/// <summary>
		/// Subscribes selection service events.
		/// </summary>
		private void InitSelectionService()
		{
			m_selectionService = ( ISelectionService )base.GetService( typeof( ISelectionService ) );
			if( m_selectionService != null )
			{
				m_selectionService.SelectionChanged += new EventHandler( SelectionChanged );
			}
		}

		/// <summary>
		/// Unsubscribes selection service events.
		/// </summary>
		private void UninitSelectionService()
		{
			if( m_selectionService != null )
			{
				m_selectionService.SelectionChanged -= new EventHandler( SelectionChanged );
				m_selectionService = null;
			}
		}


#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// Subscribes selection service events.
		/// </summary>
		private void InitLoaderService()
		{
            IDesignerSerializationManager serializationManager = 
				base.GetService( typeof( IDesignerSerializationManager ) ) as IDesignerSerializationManager;

			if( serializationManager != null )
			{
				serializationManager.SerializationComplete += new EventHandler( SerializationComplete );
			}
		}
#endif
		#endregion
		
		#region Class overrides
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// Indicates, that this container can be parent only for
		/// split panel object.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		public override bool CanParent( Control control )
		{
			return( control as SplitPanelAdv != null );
		}
#endif
		/// <summary>
		/// Initializes designer.
		/// </summary>
		/// <param name="component"></param>
		public override void Initialize( IComponent component )
		{
			if( component == null ) throw new NullReferenceException( "component" );

			base.Initialize( component );
            
			m_splitContainer = component as SplitContainerAdv;
            
			if( m_splitContainer != null )
			{
				// subsribe for mouse hooks, if not subscribed yet,
				// because if mouse messages are not processed by control at design-time
				if( !m_bIsActive )
				{
					m_bIsActive = true;
					MessageFilterEntryHelper.AddMessageFilter( m_splitContainer, false );
				}

				m_splitContainer.SplitterMoved += new SplitterMoveEventHandler( SplitterMoved );

				// subscribe services
				InitDesignerHost();
				InitSelectionService();

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				InitLoaderService();
#endif

				// Enable split panels design
				EnablePanelsDesign();

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				// get edit main menu item from currently running VS2003.
				InitEditMenuItem();

				SubscribeEditMenuCommands();
                				
				ReloadDesigner();
#endif
			}
		}

		/// <summary>
		/// Unsubscribe DesignerHost events.
		/// </summary>
		private void UnInitDesignerHost()
		{
			IDesignerHost host = (IDesignerHost)base.GetService( typeof( IDesignerHost ) );

			if( host != null )
			{
				host.Activated -= new EventHandler( DesignerActivated );
				host.Deactivated -= new EventHandler( DesignerDeactivated );
			}
		}

		/// <summary>
		/// Subscribe DesignerHost events.
		/// </summary>
		private void InitDesignerHost()
		{
			IDesignerHost host = (IDesignerHost)base.GetService( typeof( IDesignerHost ) );

			if( host != null )
			{
				host.Activated += new EventHandler( DesignerActivated );
				host.Deactivated += new EventHandler( DesignerDeactivated );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			// unsubscribe services
			UnInitDesignerHost();
			UninitSelectionService();
			
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			// restore Edit menu items enabled state to their initial state.
			m_bEnableAll = true;
			SetEditMenuItemsEnabled();
			m_bEnableAll = false;
#endif

			if( m_splitContainer != null )
			{
				// because of .Net controls doesn't receive mouse messages at design-time,
				// unsubscribe container for mouse hooks, when leaving designer
				MessageFilterEntryHelper.RemoveMessageFilter( m_splitContainer );
			
				m_splitContainer = null;
			}

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			m_editMenuItem = null;
#endif
            			

			base.Dispose( disposing );
		}

		/// <summary>
		/// Shows context menu.
		/// </summary>
		protected override void OnContextMenu( int x, int y )
		{

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			// disable such menu items as Cut, Copy, Delete, etc.
			DisableContextMenuCommands();
#endif

			base.OnContextMenu( x, y );
		}
		#endregion

		#region Class utility methods
		/// <summary>
		/// Ensure that references to all split containers have been created
		/// </summary>
		private void EnsureReferences()
		{
			IDesignerHost host = (IDesignerHost)base.GetService( typeof( IDesignerHost ) );

			if( host != null )
			{
				IReferenceService refferenceService = host.GetService( typeof( IReferenceService ) ) as IReferenceService;

				refferenceService.GetReferences( typeof( SplitContainerAdv ) );
			}
		}

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// Reloads designer.
		/// </summary>
		/// <returns> True, if succeed, otherwise - false. </returns>
		private bool ReloadDesigner()
		{
			bool succeeed = false;

			IDesignerLoaderService loaderService = 
				base.GetService( typeof( IDesignerLoaderService ) ) as IDesignerLoaderService;
			
			if( loaderService!= null )
			{
				succeeed = loaderService.Reload();
			}
            
			return succeeed;
		}
		/// <summary>
		/// Disables such menu items as Cut, Copy, Delete, etc. in context menu.
		/// </summary>
		private void DisableContextMenuCommands()
		{			
			IMenuCommandService menuService = ( IMenuCommandService )base.GetService( typeof( IMenuCommandService ) );
			if( menuService != null )
			{
				DisabeContextMenuCommand( menuService, StandardCommands.Cut );
				DisabeContextMenuCommand( menuService, StandardCommands.Copy );
				DisabeContextMenuCommand( menuService, StandardCommands.Delete );
			}
		}
		/// <summary>
		/// Gets current Dte from running object table( ROT ).
		/// </summary>
		private DTE GetCurrentDTE()
		{
			int numFetched = 0;
			UCOMIRunningObjectTable runningObjectTable = null;
			UCOMIEnumMoniker monikerEnumerator = null;
			UCOMIBindCtx ctx = null;
			UCOMIMoniker[ ] monikers = new UCOMIMoniker[1];
			string runningObjectName = null;
			object runningObjectVal = null;

			System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();

			// get current dte Name
			string dteName = string.Format( DEF_VS_IDE_ID_FORMAT, currentProcess.Id );

			NativeMethods.GetRunningObjectTable( 0, out runningObjectTable );
			runningObjectTable.EnumRunning( out monikerEnumerator );
			monikerEnumerator.Reset();

			// iterate through ROT and search for DTE, and with Name
			while( monikerEnumerator.Next( 1, monikers, out numFetched ) == 0 )
			{
				NativeMethods.CreateBindCtx( 0, out ctx );

				monikers[ 0 ].GetDisplayName( ctx, null, out runningObjectName );

				// found!
				if( string.Compare( runningObjectName, dteName, true ) == 0 )
				{
					runningObjectTable.GetObject( monikers[ 0 ], out runningObjectVal );
					break;
				}
			}

			DTE dte = runningObjectVal as DTE;

			return dte;
		}

		/// <summary>
		/// Initializes Edit menu item from currently running VS2003 main menu.
		/// </summary>
		private void InitEditMenuItem()
		{
			if( m_currentDte == null )
			{
				m_currentDte = GetCurrentDTE();
			}

			// search for edit menu item by it's id
			if( m_editMenuItem == null &&  m_currentDte != null )
			{
				// get main menu command bar
				Microsoft.Office.Core.CommandBar mainMenuBar = m_currentDte.CommandBars[ DEF_MAINMENUBAR_NAME ];
				if( mainMenuBar != null )
				{
					m_editMenuItem = mainMenuBar.Controls[ DEF_MENUITEM_EDIT_INDEX ] as Microsoft.Office.Core.CommandBarPopup;
				}
			}
		}

		/// <summary>
		/// Subscribes edit main menu items to handle their processing.
		/// </summary>
		private void SubscribeEditMenuCommands()
		{
			m_currentDte.Events.get_CommandEvents( DEF_MAINMENU_EDIT_COMMAND_GUID,
				DEF_MAINMENU_EDIT_CUT_COMMAND_ID ).BeforeExecute += 
				new _dispCommandEvents_BeforeExecuteEventHandler( BeforeExecuteMenuCommand );

			m_currentDte.Events.get_CommandEvents( DEF_MAINMENU_EDIT_COMMAND_GUID,
				DEF_MAINMENU_EDIT_COPY_COMMAND_ID ).BeforeExecute += 
				new _dispCommandEvents_BeforeExecuteEventHandler( BeforeExecuteMenuCommand );

			m_currentDte.Events.get_CommandEvents( DEF_MAINMENU_EDIT_COMMAND_GUID,
				DEF_MAINMENU_EDIT_PASTE_COMMAND_ID ).BeforeExecute += 
				new _dispCommandEvents_BeforeExecuteEventHandler( BeforeExecuteMenuCommand );

			m_currentDte.Events.get_CommandEvents( DEF_MAINMENU_EDIT_COMMAND_GUID,
				DEF_MAINMENU_EDIT_DELETE_COMMAND_ID ).BeforeExecute += 
				new _dispCommandEvents_BeforeExecuteEventHandler( BeforeExecuteMenuCommand );
		}

		/// <summary>
		/// Sets some menu items in VS2003 Edit main menu state to specified value.
		/// </summary>
		private void SetEditMenuItemsEnabled()
		{
			InitEditMenuItem();

			ArrayList arrItemsToDisable = new ArrayList();
			ArrayList arrItemsToEnable = new ArrayList();

			// get, which items needed to be enabled and which - disabled
			if( SelectionService != null && !m_bEnableAll )
			{
				if( ( SelectionService.PrimarySelection as SplitContainerAdv ) != null )
				{
					arrItemsToDisable.Add( DEF_MENUITEM_PASTE_INDEX );

					arrItemsToEnable.Add( DEF_MENUITEM_COPY_INDEX );
					arrItemsToEnable.Add( DEF_MENUITEM_CUT_INDEX );
					arrItemsToEnable.Add( DEF_MENUITEM_DELETE_INDEX );
				}
				else if( ( SelectionService.PrimarySelection as SplitPanelAdv ) != null )
				{
					arrItemsToEnable.Add( DEF_MENUITEM_PASTE_INDEX );

					arrItemsToDisable.Add( DEF_MENUITEM_COPY_INDEX );
					arrItemsToDisable.Add( DEF_MENUITEM_CUT_INDEX );
					arrItemsToDisable.Add( DEF_MENUITEM_DELETE_INDEX );
				}
				else
				{
					arrItemsToEnable.Add( DEF_MENUITEM_PASTE_INDEX );
					arrItemsToEnable.Add( DEF_MENUITEM_COPY_INDEX );
					arrItemsToEnable.Add( DEF_MENUITEM_CUT_INDEX );
					arrItemsToEnable.Add( DEF_MENUITEM_DELETE_INDEX );
				}
			}
			else
			{
				arrItemsToEnable.Add( DEF_MENUITEM_PASTE_INDEX );
				arrItemsToEnable.Add( DEF_MENUITEM_COPY_INDEX );
				arrItemsToEnable.Add( DEF_MENUITEM_PASTE_INDEX );
				arrItemsToEnable.Add( DEF_MENUITEM_PASTE_INDEX );
			}

			// set items state
			if( m_editMenuItem != null )
			{
				Microsoft.Office.Core.CommandBarControl item = null;
				for( int i = 0, len = arrItemsToDisable.Count; i < len; i++ )
				{
					item = m_editMenuItem.Controls[ (int)arrItemsToDisable[ i ] ];
					if( item != null && item.Enabled )
					{
						item.Enabled = false;
					}
				}

				for( int i = 0, len = arrItemsToEnable.Count; i < len; i++ )
				{
					item = m_editMenuItem.Controls[ (int)arrItemsToEnable[ i ] ];
					if( item != null && !item.Enabled )
					{
						item.Enabled = true;
					}
				}
			}
		}

		/// <summary>
		/// Disables specified menu item in ContextMenu at design-time.
		/// </summary>
		/// <param name="menuService"> Service, used to find menu items </param>
		/// <param name="commandID"> command to disable </param>
		protected void DisabeContextMenuCommand( IMenuCommandService menuService, CommandID commandID )
		{
			if( menuService == null )
				throw new ArgumentNullException( "menuService" );

			if( commandID == null )
				throw new ArgumentNullException( "commandID" );

			MenuCommand command = menuService.FindCommand( commandID );
			if( command != null && command.Enabled )
			{
				command.Enabled = false;
			}
		}
    
		/// <summary>
		/// Enable design-time behaviour for child control.
		/// </summary>
		/// <param name="childControl"></param>
		/// <param name="name"> Contrlol's name, with which it will be acessable at design-time. </param>
		/// <returns> True, if design-time behaviour is enabled succesfully. Otherwise - false. </returns>
		protected virtual bool EnableDesignMode( Control childControl, string name )
		{
			if( childControl == null ) throw new ArgumentNullException( "childControl" );
			if (name == null) throw new ArgumentNullException( "name" );

			IDesignerHost host = (IDesignerHost)base.GetService( typeof( IDesignerHost ) );

			if( host != null )
			{
				ComponentCollection components = host.Container.Components;

				if( components != null && components.Count > 0 )
				{
					for( int i = 0, len = components.Count; i < len; i++ )
					{
						if( components[ i ].Equals( childControl ) )
						{
							return true;
						}
					}
				}

				host.Container.Add( childControl, name );

				return true;
			}

			return false;
		}
#endif
		/// <summary>
		/// update specified split container properties to notify designer that they are changed
		/// and needed to be serialized.
		/// </summary>
		private void UpdateProperties()
		{
			PropertyDescriptorCollection propertiesDescription = 
				TypeDescriptor.GetProperties( m_splitContainer );
						
			// update splitter distance property.
			PropertyDescriptor splitDistancePropDescriptor = propertiesDescription[ DEF_SPLITDISTANCE_PROP_NAME ];
			if( splitDistancePropDescriptor != null )
			{
				base.RaiseComponentChanging( splitDistancePropDescriptor );
				base.RaiseComponentChanged( splitDistancePropDescriptor, null, null );
			}
		}

		/// <summary>
		/// Enables container's split panels design-time behaviour.
		/// </summary>
		private void EnablePanelsDesign()
		{
			if( m_splitContainer != null )
			{
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				this.EnableDesignMode( m_splitContainer.Panel1, DEF_PANEL1_NAME );
				this.EnableDesignMode( m_splitContainer.Panel2, DEF_PANEL2_NAME );
#else
				this.EnableDesignMode( m_splitContainer.Panel1, m_splitContainer.Panel1.UniqueName );
				this.EnableDesignMode( m_splitContainer.Panel2, m_splitContainer.Panel2.UniqueName );
#endif
			}
		}

		#endregion

		#region Class event handlers
		/// <summary>
		/// Additional logic on designer activation is implemented here.
		/// </summary>
		private void DesignerActivated( object sender, EventArgs e )
		{
			if( !m_bIsActive )
			{
				m_bIsActive = true;

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				// set aprropriate main menu items enabled/disabled, when 
				// designer is activated, according to what component is selected.
				SetEditMenuItemsEnabled();
#endif

				// because of .Net controls doesn't receive mouse messages at design-time,
				// subscribe container for mouse hooks when designer is activated.
				if( m_splitContainer != null )
				{
					MessageFilterEntryHelper.AddMessageFilter( m_splitContainer, false );
				}
			}

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			// designer needed to be reloaded due to VS2003 deserialization problems
			ReloadDesigner();
#endif
		}

		/// <summary>
		/// Additional logic on designer deactivation is implemented here.
		/// </summary>
		private void DesignerDeactivated( object sender, EventArgs e )
		{

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			// set aprropriate main menu items enabled, when
			// designer is deactivated
			m_bEnableAll = true;
			SetEditMenuItemsEnabled();
			m_bEnableAll = false;
#endif

			// because of .Net controls doesn't receive mouse messages at design-time,
			// unsubscribe container for mouse hooks, when leaving designer
			MessageFilterEntryHelper.RemoveMessageFilter( m_splitContainer );

			m_bIsActive = false;
		}

		/// <summary>
		/// Additional logic for selected control changing is implemented here.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SelectionChanged( object sender, EventArgs e )
		{
			// set selected split panel to draw highlited
			if( m_splitContainer != null )
			{
				if( SelectionService.PrimarySelection == m_splitContainer.Panel1 )
				{
					m_splitContainer.SelectedPanel = m_splitContainer.Panel1;
				}
				else if( SelectionService.PrimarySelection == m_splitContainer.Panel2 )
				{
					m_splitContainer.SelectedPanel = m_splitContainer.Panel2;
				}
				else
				{
					m_splitContainer.SelectedPanel = null;
				}
			}

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			// set enabled state for edit main menu items in VS2003;
			SetEditMenuItemsEnabled();
#endif
		}


		private void SplitterMoved( object sender, SplitterMoveEventArgs e )
		{
			// update specified split container properties to notify designer that they are changed
			// and needed to be serialized.
			UpdateProperties();
		}		

#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary>
		/// Cancel specified commands excecution in VS2003 designer,
		/// if split container or split panel are selected.
		/// </summary>
		private void BeforeExecuteMenuCommand( string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault )
		{
			if( SelectionService != null && m_bIsActive )
			{
				// split container is selected, cancel paste operation
				if( ( SelectionService.PrimarySelection as SplitContainerAdv ) != null )
				{
					CancelDefault = ( ID == DEF_MAINMENU_EDIT_PASTE_COMMAND_ID );
				}

				// split panel is selected, cancel all operations except paste
				else if( ( SelectionService.PrimarySelection as SplitPanelAdv ) != null )
				{
					CancelDefault = ( ID != DEF_MAINMENU_EDIT_PASTE_COMMAND_ID );
				}
			}            
		}

		private void SerializationComplete( object sender, EventArgs e )
		{
			EnsureReferences();

			// get edit main menu item from currently running VS2003.
			InitEditMenuItem();

			SubscribeEditMenuCommands();
		}
#endif
		#endregion		
	}
}
