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

#region File Using
using System;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using Microsoft.Win32;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Diagnostics;

using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.ComponentModel;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Design;
using Syncfusion.Collections;
using Syncfusion.Win32;
using Syncfusion.Runtime.Serialization;
#endregion

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	internal class ResetToolBarBarItem : BarItem
	{
		private CommandBarExt curBar;
		private BarManager manager;

		public ResetToolBarBarItem( BarManager manager )
		{
			this.manager = manager;
		}

		public CommandBarExt CurrentBar
		{
			get { return this.curBar; }
			set { this.curBar = value; }
		}

		protected override void OnItemClicked( EventArgs args )
		{
			this.curBar.PopupMenu.Hide();

			if( manager.MainFrameBarManager == null )
				return;

			manager.MainFrameBarManager.ResetContainer( curBar.Bar );

			MessageBox.Show( SR.GetString( SR.SuccesfulToolbarResetMessage, manager.MainFrameBarManager), SR.GetString( SR.SuccesfulResetMessageBoxTitle, manager.MainFrameBarManager) );
		}
	}

	internal class ButtonVisibilityToggleBarItem : BarItem
	{
		BarItem barItem;
		public ButtonVisibilityToggleBarItem( BarItem barItem, Bar bar )
		{
			this.barItem = barItem;
			this.Tag = bar;
		}

		public override string Text
		{
			get { return this.barItem.Text; }
			set { }
		}

		public override int ImageIndex
		{
			get { return this.barItem.ImageIndex; }
			set { }
		}

		public override ImageList ImageList
		{
			get { return this.barItem.ImageList; }
		}

		public override ImageListAdv ImageListAdv
		{
			get { return this.barItem.ImageListAdv; }
		}

		public override Shortcut Shortcut
		{
			get { return this.barItem.Shortcut; }
			set { }
		}

		public override bool Checked
		{
			get
			{
				if( this.barItem.Manager.MainFrameBarManager.IsBarItemVisibilityPrefAvailable( this.barItem, ( Bar )this.Tag ) )
					return this.barItem.Manager.MainFrameBarManager.ShouldDrawVisibleInBar( this.barItem, ( Bar )this.Tag );
				else
					return this.barItem.Visible;
			}
			set { }
		}

		protected override void OnItemClicked( EventArgs args )
		{
			// Toggle the visibility of the button
			this.barItem.Manager.MainFrameBarManager.SetUserVisibilityPreferenceInBar( this.barItem, ( Bar )this.Tag, !this.Checked );

			// So the bar will redraw with the new visibility.
			this.barItem.OnPropertyChanged( new SyncfusionPropertyChangedEventArgs( PropertyChangeEffect.NeedLayout, "Visible", !this.Checked, this.Checked ) );

			// And finally inform that the Checked property has changed
			this.OnPropertyChanged( new SyncfusionPropertyChangedEventArgs( PropertyChangeEffect.NeedLayout, "Checked", !this.Checked, this.Checked ) );
		}
	}
	[Syncfusion.Documentation.DocumentationExclude(),
	ToolboxItem( false )]
	public class CommandBarControllerExt : CommandBarController
	{
		private MainFrameBarManager manager;
		private CommandBarManager cbarManager;
		internal const string DEF_SPACER = "_";
		protected override CommandDockBar CreateCommandDockBar()
		{
			CommandDockBar newBar = new CommandDockBarExt( this );
			return newBar;
		}
		public CommandBarControllerExt( CommandBarManager cbarManager )
		{
			this.cbarManager = cbarManager;
		}
		public CommandBarManager CBarManager
		{
			get { return this.cbarManager; }
		}
		public MainFrameBarManager Manager
		{
			get { return this.manager; }
			set { this.manager = value; }
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal override string PersistenceID
		{
			get
			{
				if( this.manager != null )
					return this.manager.PersistenceID;
				else
					return String.Empty;
			}
		}

		public void AppendCommandBarInfoForPersistance( CommandBar cbar )
		{
			if( cbar != null )
			{
				if( this.wpprCBController == null )
				{
					this.CreateCachedPersistanceData();
				}

				if( !( cbar is CommandBarExt ) && manager != null && manager.ActiveChildBarManager != null )
				{
					string commandBarName = cbar.Name + DEF_SPACER + manager.MdiChildrenFormName;
					this.wpprCBController.AddCommandBarData( cbar, commandBarName );
				}
				else
				{
					this.wpprCBController.AddCommandBarData( cbar );
				}
			}
		}

		protected override void ReadDeserializedData( CommandBar commandBar )
		{
			if( !( commandBar is CommandBarExt ) && manager != null && manager.ActiveChildBarManager != null )
			{
				if( String.Empty != manager.MdiChildrenFormName )
				{
					string cBarName = commandBar.Name + DEF_SPACER + manager.MdiChildrenFormName;

					this.wpprCBController.ReadDeserializedData( commandBar, cBarName );
				}
				else
				{
					this.wpprCBController.ReadDeserializedData( commandBar );
				}
			}
			else
			{
				base.ReadDeserializedData( commandBar );
			}
		}

		protected override CBCtrlrSerializationWrapper GetPersistanceData()
		{
			if( this.wpprCBController == null )
				this.CreateCachedPersistanceData();

			// Will save the state of the current command bars, keeping the state of the
			// deleted command bars.
			this.wpprCBController.InitSerailizationData( this );
			return this.wpprCBController;
		}
		private void CreateCachedPersistanceData()
		{
			this.wpprCBController = base.GetPersistanceData();
		}

		protected override void Dispose( bool disposing )
		{
			this.manager = null;
			this.cbarManager = null;

			base.Dispose( disposing );
		}

		/// <summary>
		/// Indicating whether the state for CommandBars from the Form loaded from isolated storage.
		/// </summary>
		public bool IsLoaded( Form form )
		{
			bool bLoaded = false;

			if( form != null )
			{
				ArrayList arr = manager.GetCommandBarsForForm( form );

				if( arr != null && arr.Count > 0 )
				{
					bool bInitialized = false;
					string commandBarName = String.Empty;

					foreach( CommandBar commandBar in arr )
					{
						if( !( commandBar is CommandBarExt ) && manager != null && manager.ActiveChildBarManager != null )
						{
							commandBarName = commandBar.Name + DEF_SPACER + manager.MdiChildrenFormName;
							bInitialized = this.wpprCBController.IsInitialized( commandBarName );
						}
						else
						{
							bInitialized = this.wpprCBController.IsInitialized( commandBar );
						}

						if( !bInitialized )
						{
							break;
						}
					}

					bLoaded = bInitialized;
				}
			}

			return bLoaded;
		}
	}

	[
	Syncfusion.Documentation.DocumentationExclude(),
	ToolboxItem( false )
	]
	public class CommandDockBarExt : CommandDockBar
	{
		private CommandBarControllerExt controllerExt;
        static bool s_isDevEnv = (Application.ExecutablePath.ToLower().IndexOf("devenv.exe") >= 0);
		public CommandDockBarExt( CommandBarControllerExt controller )
			: base( controller )
		{
			this.controllerExt = controller;
			// Set the tab index to be the max so that, the other controls will first
			// get a crack at mnemonic processing.
			// But, don't do this for design-mode. Otherwise controls addded next will
			// get a TabIndex with a very large TabIndex
			// This will still set a new sibling's TabIndex to the next value during runtime.
			if( controller == null ||
				!( controller.CBarManager.BarManager.DesignMode
					|| ( s_isDevEnv ) )
			)
				this.TabIndex = 10000; // Some high value
		}

		protected override void OnPaintBackground( PaintEventArgs e )
		{
			bool customBackground = false;
			if( this.controllerExt.Manager != null )
			{
				customBackground =
					this.controllerExt.Manager.OnDrawDockBarBackground( new DockBarPaintEventArgs( e, this.DockBorder, this.ClientRectangle ) );
			}
			if( !customBackground )
				base.OnPaintBackground( e );
		}
		private CommandBarDockBorder DockBorder
		{
			get
			{
				if( this.Dock == DockStyle.Left )
					return CommandBarDockBorder.Left;
				else if( this.Dock == DockStyle.Top )
					return CommandBarDockBorder.Top;
				else if( this.Dock == DockStyle.Bottom )
					return CommandBarDockBorder.Bottom;
				else if( this.Dock == DockStyle.Right )
					return CommandBarDockBorder.Right;
				else
					return CommandBarDockBorder.None;
			}
		}
		protected override void WndProc( ref Message m )
		{
			if( m.Msg == 0x0007 /*WM_SETFOCUS*/)
			{
				// Without this, the framework will set focus to one of the Controls in the
				// command bar when there is nothing else in the Form.
				this.ActiveControl = null;
			}
			base.WndProc( ref m );
		}

		protected override void ResetTabIndex()
		{
			// Don't call the base class; we want to preserve the Int32.MaxValue
		}

		protected override bool ProcessMnemonic( char charCode )
		{
			bool processed = false;
			if( this.DockBorder == CommandBarDockBorder.Top )
			{
				// Support this only if Alt key is pressed or if keyboard navigation is on.
				if( ( Control.ModifierKeys & Keys.Alt ) > 0 )
					processed = this.controllerExt.CBarManager.ProcessShortcut( charCode );
			}
			if( !processed )
				return base.ProcessMnemonic( charCode );
			else
				return true;
		}

		public CommandBarExt GetMainMenuBar()
		{
			foreach( Control child in this.Controls )
			{
				if( child is CommandBarExt )
				{
					if( ( ( ( CommandBarExt )child ).Bar.BarStyle & BarStyle.IsMainMenu ) > 0 )
						return child as CommandBarExt;
				}
			}
			return null;
		}

		/// Clean up any resources being used.
		protected override void Dispose( bool disposing )
		{
			this.controllerExt = null;
			base.Dispose( disposing );
		}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public class CommandBarManager : IDisposable
	{
		#region PRIVATE_MEMBERS
		private Form form;
		private BarManager manager;
		private BarItem customizeItem;
		private Hashtable commandbarsByBarId;
		private Hashtable barsByFormType;
		private CommandBarControllerExt controller;
		private bool showMainMenusAutomatically = false;
		private Bar mainMenuBar;
		private CommandBarExt mainMenuCommandBar;
		private CommandBarExt selectedCommandBar;
		private System.Windows.Forms.Timer hideTimer;
		private ArrayList barsToHideInManagers;
		private bool loadedFromIsolatedStore = false;
		// Items to be serialized
		private ArrayListExt detachedCommandBars;
		private Hashtable barIdVisibilityPref;
		private const string BarVisibilitySettingsLabel = "BarVisibilitySettings";
		private PopupMenusManager popupMenusManager;
		private ToolbarListPopupMenu toolbarListPopup;
		/// <summary>
		/// Contains the name of the children form for which loaded CommandBar state.
		/// </summary>
		private Hashtable m_htLoadedForm = new Hashtable();
		#endregion PRIVATE_MEMBERS

		static CommandBarManager()
		{
#if SINGLE_DLL_BUILD
			AppStateSerializer.SetBindingInfo( "Syncfusion.Tools.Windows", typeof( CommandBarManager ).Assembly );
#else
			AppStateSerializer.SetBindingInfo("Syncfusion.Tools.Frameworks", typeof(CommandBarManager).Assembly);
			// To support backward compatibility
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(CommandBarManager).FullName, typeof(CommandBarManager).Assembly);
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(CommandBarExtSerializer).FullName, typeof(CommandBarExtSerializer).Assembly);
#endif
		}
		public CommandBarManager( Form form, BarManager manager )
		{
			this.form = form;
			this.manager = manager;

			this.popupMenusManager = this.CreateInternalPopupMenusManager();
			this.toolbarListPopup = this.CreateToolbarListPopup();

			this.controller = new CommandBarControllerExt( this );
			this.controller.PersistState = false;
			this.controller.CommandBarSerializer = typeof( CommandBarExtSerializer );
			this.controller.DockBarStateChanged += new CommandBarController.DockBarStateChangedHandler( this.DockBarStateChanged );
			this.controller.HostForm = this.form;
			this.controller.InitializeCBController();
			this.controller.bDisableButtons = manager.DesignMode;
			this.controller.ThemesEnabled = this.ThemesEnabled;
			this.controller.Style = manager.Style;
			this.commandbarsByBarId = new Hashtable();
			this.barsByFormType = new Hashtable();
			this.barIdVisibilityPref = new Hashtable();
			this.hideTimer = new System.Windows.Forms.Timer();
			this.hideTimer.Interval = 100;
			this.hideTimer.Tick += new EventHandler( this.HideBarsEvent );

			// Customize item
			this.customizeItem = new BarItem( SR.GetString( SR.CustomizeMenu, this) );
			this.customizeItem.Click += new EventHandler( this.Customize_Clicked );

			this.manager.CustomizationBegin += new EventHandler( this.CusomizationBegin );
			this.manager.CustomizationDone += new EventHandler( this.CusomizationDone );
			this.manager.PropertyChanged
				+= new SyncfusionPropertyChangedEventHandler( this.Property_Changed );

			this.barsToHideInManagers = new ArrayList();

			this.detachedCommandBars = new ArrayListExt();
			this.detachedCommandBars.CollectionChanged += new CollectionChangeEventHandler( this.DetachedBars_CollectionChanged );

			if( ( this.manager is MainFrameBarManager ) && !this.manager.DesignMode )
				this.controller.Manager = this.manager as MainFrameBarManager;
		}

		public PopupMenu DockBarContextMenu
		{
			get { return this.toolbarListPopup; }
		}

		protected virtual PopupMenusManager CreateInternalPopupMenusManager()
		{
			return new PopupMenusManager();
		}

		protected virtual ToolbarListPopupMenu CreateToolbarListPopup()
		{
			return new ToolbarListPopupMenu( this );
		}
		private bool ThemesEnabled
		{
			get
			{
				if( this.manager != null && this.manager.MainFrameBarManager != null )
					return this.manager.MainFrameBarManager.ThemesEnabled;
				else
					return false;
			}
		}

		internal bool Customizing
		{
			get
			{
				if( this.manager != null )
				{
					return this.manager.Customizing;
				}
				return false;
			}
		}
		private void CusomizationBegin( object sender, EventArgs e )
		{
			this.controller.bDisableButtons = true;
		}
		private void CusomizationDone( object sender, EventArgs e )
		{
			this.controller.bDisableButtons = manager.DesignMode;
		}

		#region DETACHED_COMMANDBARS
		public CommandBarController GetCommandBarController()
		{
			return this.controller;
		}
		internal ArrayListExt DetachedCommandBars
		{
			get { return this.detachedCommandBars; }
		}
		private void DetachedBars_CollectionChanged( object sender, CollectionChangeEventArgs e )
		{
			// Add it to the commandbar controller.
			if( e.Action == CollectionChangeAction.Add )
			{
				Trace.Assert( e.Element is CommandBar );

				this.controller.CommandBars.Add( e.Element as CommandBar );
			}
			// Remove it from the commandbar controller.
			else if( e.Action == CollectionChangeAction.Remove )
				this.controller.CommandBars.Remove( e.Element as CommandBar );
		}
		#endregion DETACHED_COMMANDBARS
		public BarManager BarManager
		{
			get { return this.manager; }
		}

		private void Property_Changed( object sender, SyncfusionPropertyChangedEventArgs args )
		{
			switch( args.PropertyName )
			{
				case "ThemesEnabled":
					{
						if( this.controller != null )
							this.controller.ThemesEnabled = this.ThemesEnabled;

						bool bVal = ( bool )args.NewValue;

						foreach( CommandBarExt cbar in this.commandbarsByBarId.Values )
						{
							cbar.BarControl.ThemesEnabled = bVal;
						}
						break;
					}

				case "Style":
					{
						if( this.controller != null )
							this.controller.Style = manager.Style;
						break;
					}

				case "RightToLeft":
					{
						RightToLeft eRTL = ( null != this.manager ) ? this.manager.RightToLeft :
							( ( null != this.form ) ? this.form.RightToLeft : RightToLeft.No );

						foreach( CommandBar cbBar in this.DetachedCommandBars )
						{
							cbBar.RightToLeft = eRTL;
						}
						break;
					}
			}

			if( args.PropertyChangeEffect == PropertyChangeEffect.NeedLayout )
			{
				this.RecalcLayout();
			}
		}

		public void RecalcLayout()
		{
			foreach( CommandBarExt cbar in this.commandbarsByBarId.Values )
			{
				bool forceRecalc = cbar.NeedLayout;
				bool dontRecalc = !cbar.NeedLayout;
				cbar.RecalcBarLayout( forceRecalc, dontRecalc );
			}

			this.controller.RecalcLayout( CommandBarDockBorder.Left | CommandBarDockBorder.Top | CommandBarDockBorder.Right | CommandBarDockBorder.Bottom );
		}

		~CommandBarManager()
		{
			this.Dispose( false );
		}

		public virtual void Dispose()
		{
			this.hideTimer.Tick -= new EventHandler( this.HideBarsEvent );
			this.Dispose( true );
			GC.SuppressFinalize( this );
		}

		protected virtual void Dispose( bool disposing )
		{
			if( disposing )
			{
				this.CleanupOnFormClose();

				if( this.mainMenuCommandBar != null )
					mainMenuCommandBar.Dispose();

				if( this.selectedCommandBar != null )
					this.selectedCommandBar.Dispose();

				if( this.toolbarListPopup != null )
				{
					this.toolbarListPopup.Dispose();
					this.toolbarListPopup = null;
				}

				if( this.controller != null )
				{
					this.controller.Dispose();
					this.controller = null;
				}

				if( this.manager != null )
				{
					this.manager.Bars.CollectionChanged -= new CollectionChangeEventHandler( this.BarCollectionChanged );
					this.manager.Bars.ItemPropertyChanged -= new Syncfusion.ComponentModel.SyncfusionPropertyChangedEventHandler
						( this.BarPropertyChanged );

					this.manager.CustomizationBegin -= new EventHandler( this.CusomizationBegin );
					this.manager.CustomizationDone -= new EventHandler( this.CusomizationDone );
					this.manager.PropertyChanged -= new SyncfusionPropertyChangedEventHandler( this.Property_Changed );
					this.manager = null;
				}

				if( this.popupMenusManager != null )
				{
					this.popupMenusManager.Dispose();
					this.popupMenusManager = null;
				}

				if( null != this.customizeItem )
				{
					this.customizeItem.Click -= new EventHandler( this.Customize_Clicked );
					this.customizeItem.Dispose();
					this.customizeItem = null;
				}

				foreach( CommandBarExt commandBar in this.commandbarsByBarId )
				{
					this.StopListeningForMousePress( commandBar );
					commandBar.Dispose();
				}

				this.commandbarsByBarId.Clear();
				this.barsByFormType.Clear();

				this.detachedCommandBars.CollectionChanged -= new CollectionChangeEventHandler( this.DetachedBars_CollectionChanged );
				this.detachedCommandBars.Clear();

				this.barsToHideInManagers.Clear();

				this.form = null;
			}
		}

		public void CleanupOnFormClose()
		{
			this.commandbarsByBarId.Clear();
		}

		public void SaveCommandBarState( AppStateSerializer serializer )
		{
			serializer.SerializeObject( this.PersistenceID + ":" + BarVisibilitySettingsLabel, this.barIdVisibilityPref );

			// Load the Bar positions
			try
			{
				this.controller.SaveCommandBarState( serializer );
			}
			catch { }
			{
				//Trace.WriteLine("Error serializing Toolbar positions: " + e.Message);
			}
		}
		internal string PersistenceID
		{
			get
			{
				if( this.manager != null && this.manager is MainFrameBarManager )
					return ( ( MainFrameBarManager )this.manager ).PersistenceID;
				else
					return String.Empty;
			}
		}

		public void LockBars()
		{
			this.controller.FreezeLayout = true;
		}

		public void UnLockBars()
		{
			//NativeMethodsHelper.SetRedrawWindow( this.form.Handle, false, false );
			//this.controller.RedockAndRecalcAll();
			this.controller.FreezeLayout = false;

            if (this.controller.alDelayLoadBars.Count > 0)
            {
                for (int i = 0; i < this.controller.alDelayLoadBars.Count; i++)
                {
                    CommandBar cbar = this.controller.alDelayLoadBars[i] as CommandBar;
                    if (cbar.Visible)
                    {
                        CommandBarDockState border = cbar.cdbParent.GetDockBorder();
                        cbar.cbarDockState = CommandBarDockState.None;
                        cbar.FireCommandBarStateChanging(new CommandBarStateChangingEventArgs(border));
                        cbar.cbarDockState = border;

                        cbar.cdbParent.IsInitializing = true;
                        cbar.cdbParent.AddCommandBar(cbar, false);
                        cbar.cdbParent.IsInitializing = false;

                        cbar.FireCommandBarStateChanged(EventArgs.Empty);
                    }
                }

                foreach (CommandBar cbar in this.controller.alDelayLoadBars)
                {
                    cbar.cdbParent.ReplaceBarLocation(cbar);
                }

                this.controller.alDelayLoadBars.Clear();
            }

			// Calling RecalcLayout since RecalcBarLayout would have returned without recalculating.
			// Possible performance improvement.
			this.RecalcLayout();

			//NativeMethodsHelper.SetRedrawWindow( this.form.Handle, true, true );
		}

		public void LoadCommandBarState( AppStateSerializer serializer )
		{
			object tempObject = null;
			tempObject = serializer.DeserializeObject( this.PersistenceID + ":" + BarVisibilitySettingsLabel );

			if( tempObject != null )
				this.barIdVisibilityPref = tempObject as Hashtable;

			// Load the Bar positions
			try
			{
				this.loadedFromIsolatedStore = this.controller.LoadCommandBarState( serializer );
			}
			catch { }
			{
				//Trace.WriteLine("Error reading persisted Toolbar state: " + e.Message);
			}
			// Needed if this method was called after Load time to apply a custom saved state.
			this.UpdateCommandBarsVisibility();
		}

		private void UpdateCommandBarsVisibility()
		{
			// Refresh the visibility of all the cbars currently available
			foreach( CommandBar cbar in this.controller.CommandBars )
			{
				if( cbar is CommandBarExt )
				{
					CommandBarExt cbarext = cbar as CommandBarExt;
					Bar bar = cbarext.Bar;
					if( bar != null )
					{
						this.ShowOrHideBar( bar, bar.Manager, true );
					}
				}
			}
		}

		public void LoadCommandBarState_Changed()
		{
			if( this.manager.MainFrameBarManager != null
				&& !this.manager.MainFrameBarManager.AutoLoadToolBarPositions )
			{
				this.loadedFromIsolatedStore = false;
				this.controller.LoseCachedCommandBarState();
			}
		}

		public bool ShowMainMenusAutomatically
		{
			get { return this.showMainMenusAutomatically; }
			set { this.showMainMenusAutomatically = value; }
		}

		//public bool ProcessShortcut(Keys key)
		public bool ProcessShortcut( char c )
		{
			bool processed = false;

			if( ( Control.ModifierKeys & Keys.Alt ) != Keys.None )
			{
				if( this.mainMenuCommandBar != null )
					processed = this.mainMenuCommandBar.ProcessShortcut( c );

				if( !processed )
				{
					foreach( CommandBarExt cbar in this.commandbarsByBarId.Values )
					{
						if( cbar.Visible )
							processed |= cbar.ProcessShortcut( c );
						if( processed )
							break;
					}
				}
			}
			return processed;
		}

		public void AttachMainMenuBar( Bar mainMenuBar )
		{
			if( this.mainMenuBar == mainMenuBar )
				return;

			// Wire the existing main-menu commandbar to the new main-menu bar.
			// This is necessary because we do not want to recreate the MdiSysMenuProvider.

			if( this.mainMenuBar != null )
			{
				// Remove the existing mapping and set up a new mapping.
				BarID oldBarID = new BarID( this.mainMenuBar.BarName, BarManager.GetFormTypeName( this.mainMenuBar.Manager ) );
				string oldBarIDAsString = oldBarID.ToString();

				this.commandbarsByBarId.Remove( oldBarIDAsString );
			}

			this.mainMenuBar = mainMenuBar;

			if( mainMenuBar == null ) mainMenuCommandBar = null;

			if( this.mainMenuBar != null )
			{
				BarID newBarID = new BarID( mainMenuBar.BarName, BarManager.GetFormTypeName( mainMenuBar.Manager ) );
				string newBarIDAsString = newBarID.ToString();

				if( this.mainMenuCommandBar == null )
				{
					// Create a new one.
					this.mainMenuCommandBar = this.BindBarInternal( mainMenuBar );
				}
				else
				{
					// Or use the existiing one.
					this.commandbarsByBarId[ newBarIDAsString ] = mainMenuCommandBar;
					// Call this for further settings.
					this.mainMenuCommandBar = this.BindBarInternal( mainMenuBar );
				}
			}
		}

		public void UpdateDesignTimeVisibility( bool visible )
		{
			// Hide all floating bars
			foreach( CommandBar cbar in this.controller.CommandBars )
			{
				if( ( cbar.DockState == CommandBarDockState.Float ) && ( cbar.Visible != false )
					&& cbar.Parent != null )
				{
					cbar.Parent.Visible = visible;
				}
			}
		}

		internal void SuspendLayout()
		{
			this.controller.SuspendLayout();
		}

		internal void ResumeLayout()
		{
			this.controller.ResumeLayout();
		}

		#region KEYBOARD_NAV_RELATED
		public void StartKeyboardNavigationInMainMenu()
		{
			if( this.mainMenuCommandBar != null )
			{
				IntPtr wndHandle = NativeMethods.WindowFromPoint( Control.MousePosition.X, Control.MousePosition.Y );
				Control mouseOverControl = Control.FromHandle( wndHandle );

				if( ( mouseOverControl != null && mouseOverControl != this.mainMenuCommandBar )
					|| wndHandle != IntPtr.Zero )
				{
					NativeMethods.SendMessage( wndHandle,
						NativeMethods.WM_MOUSELEAVE, 0, 0 );
				}

				// Need to fake this SetCapture so that MouseEnter/Leave notifications 
				// will work properly in the control with the latest MouseMove. 
				// Bug in .Net.

				if( this.mainMenuCommandBar.BarControl.IsKeyboardNavigationOn() )
				{
					this.mainMenuCommandBar.BarControl.StopKeyboardBasedNavigation();
					( this.BarManager as MainFrameBarManager ).ignoreNextAltKeyUp = false;
				}
				else
				{
					this.mainMenuCommandBar.BarControl.StartKeyboardBasedNavigation();
				}
			}
		}
		internal void MoveMenuNavigation( CommandBarExt cbe, bool forward )
		{
			ArrayList al = new ArrayList();

			// Prepare a list first.
			foreach( CommandBarExt cbar in this.commandbarsByBarId.Values )
			{
				al.Add( cbar );
			}

			if( forward )
			{
				int i = -1;
				foreach( CommandBarExt cbar in al )
				{
					i++;
					// Found the current one.
					if( cbar == cbe )
					{
						int foundIndex = i + 1;
						if( foundIndex >= al.Count )
							foundIndex = 0;

						// Skip the ones that cannot be navigatable (like the statusbar)
						CommandBarExt next = null;
						for( int n = foundIndex; n != i; n++ )
						{
							CommandBarExt c = al[ n ] as CommandBarExt;
							if( c.BarControl.CanStartKeybardBasedNavigation() )
							{
								next = c;
								break;
							}
							if( n == al.Count - 1 )
								n = -1;
						}
						if( next != null && next != cbe )
						{
							next.BarControl.StartKeyboardBasedNavigation();
						}
					}
				}
			}
			else /*Backward*/
			{
				int i = -1;
				foreach( CommandBarExt cbar in al )
				{
					i++;
					// Found the current one.
					if( cbar == cbe )
					{
						int foundIndex = i - 1;
						if( foundIndex < 0 )
							foundIndex = al.Count - 1;

						// Skip the ones that cannot be navigatable (like the statusbar)
						CommandBarExt next = null;
						for( int n = foundIndex; n != i; n-- )
						{
							CommandBarExt c = al[ n ] as CommandBarExt;
							if( c.BarControl.CanStartKeybardBasedNavigation() )
							{
								next = c;
								break;
							}
							if( n == 0 )
								n = al.Count;
						}
						if( next != null && next != cbe )
						{
							next.BarControl.StartKeyboardBasedNavigation();
						}
					}
				}
			}
		}
		public bool HintViaHotKeyPrefix
		{
			set
			{
				if( this.mainMenuCommandBar != null )
				{
					BarControlInternal bci = this.mainMenuCommandBar.BarControl;

					if( null != bci )
					{
						bci.HintViaHotKeyPrefix = value;
					}
				}
				foreach( CommandBarExt cbar in this.commandbarsByBarId.Values )
				{
					BarControlInternal bci = cbar.BarControl;

					if( null != bci )
					{
						bci.HintViaHotKeyPrefix = value;
					}
				}
			}
		}

		public bool IsKeyboardNavigationOn()
		{
			// Parse through all the command bars and see if any of it has keyboard nav on.
			bool keyboardNavOn = false;
			if( this.mainMenuCommandBar != null )
				keyboardNavOn = this.mainMenuCommandBar.BarControl.IsKeyboardNavigationOn();
			if( !keyboardNavOn )
			{
				foreach( CommandBarExt commandBar in this.commandbarsByBarId.Values )
				{
                    if (commandBar.BarControl != null && commandBar.BarControl.IsKeyboardNavigationOn())
					{
						keyboardNavOn = true;
						break;
					}
				}
			}

			return keyboardNavOn;
		}

		public void StopKeyboardNavigationInMainMenu()
		{
			// First find the commandbar with the focus
			BarControlInternal barControlWithFocus = null;
			if( this.mainMenuCommandBar != null && this.mainMenuCommandBar.BarControl.IsKeyboardNavigationOn() )
				barControlWithFocus = this.mainMenuCommandBar.BarControl;
			else
			{
				foreach( CommandBarExt commandBar in this.commandbarsByBarId.Values )
				{
					if( commandBar.BarControl.IsKeyboardNavigationOn() )
					{
						barControlWithFocus = commandBar.BarControl;
						break;
					}
				}
			}
			if( barControlWithFocus != null )
				barControlWithFocus.StopKeyboardBasedNavigation();
		}
		#endregion KEYBOARD_NAV_RELATED

		#region BAR_VISIBILITY
		private bool IsActiveManager( BarManager manager )
		{
			if( manager.DesignMode )
				return true;
			else
				return manager.MainFrameBarManager.Form.ActiveMdiChild == manager.Form;
		}

		private void Customize_Clicked( object sender, EventArgs e )
		{
			this.StartCustomize();
		}
		private void StartCustomize()
		{
			this.manager.Customize( true );
		}

		private void UserChangedBarVisibility( object sender, EventArgs e )
		{
			BarItem clickedItem = ( BarItem )sender;
			if( clickedItem.Tag != null )
			{
				CommandBarExt commandBar = clickedItem.Tag as CommandBarExt;
				bool newVisibility = !clickedItem.Checked;

				// Get BarID
				Bar bar = commandBar.Bar;
				this.SetBarVisibility( bar, newVisibility );
				clickedItem.Checked = commandBar.Visible;

				if( manager != null && bar != null )
				{
					this.manager.OnUserChangedBarVisibility( bar );
				}
			}
		}
		public void SetBarVisibility( Bar bar, bool visible )
		{
			BarID barID = new BarID( bar.BarName, BarManager.GetFormTypeName( bar.Manager ) );
			string barIDAsString = barID.ToString();

			CommandBarExt commandBar = this.commandbarsByBarId[ barIDAsString ] as CommandBarExt;

			if( bar.Manager.DesignMode )
			{
				if( visible )
					bar.BarStyle |= BarStyle.Visible;
				else
					bar.BarStyle &= ~BarStyle.Visible;

				commandBar.OnBarChanged();
			}
			else
			{
				bool activeManager = this.IsActiveManager( bar.Manager );
				activeManager |= ( bar.Manager is MainFrameBarManager );

				bool visibilityChanged = false;
				// If user turns on an invisible Bar, in the activeManager then 
				// just make the Bar Visible.
				if( visible == true && activeManager )
				{
					if( ( bar.BarStyle & BarStyle.Visible ) <= 0 )
					{
						visibilityChanged = true;
						bar.BarStyle |= BarStyle.Visible;
					}
				}

				// Store the User's preference
				if( visible == false || !activeManager
					|| visibilityChanged )
					this.barIdVisibilityPref[ barIDAsString ] = visible;

				if( ( visible == false && !activeManager )
					|| ( visible == true && activeManager && !visibilityChanged ) )
					this.barIdVisibilityPref.Remove( barIDAsString );

				// Update the command bar.
				if( commandBar != null )
					commandBar.Visible = visible;
			}

			if( controller != null )
			{
				controller.LayoutDockBarsExceptBar( commandBar.Parent as
					CommandDockBar );
			}

		}

		public bool IsBarVisible( Bar bar )
		{
			CommandBarExt commandBar = this.GetCommandBarFromBar( bar );
			if( commandBar != null )
				return commandBar.Visible;
			else
				return false;
		}

		private void HideBarsEvent( object sender, EventArgs e )
		{
			if( this.barsToHideInManagers.Count > 0 )
			{
				foreach( BarManager manager in this.barsToHideInManagers )
					this.ShowOrHideBars( manager, false );
				this.barsToHideInManagers.Clear();
			}
			this.hideTimer.Stop();
		}
		protected virtual void ShowOrHideBars( BarManager manager, bool show )
		{
			Bars bars = manager.Bars;
			if( bars == null )
				return;

			// Override the Bar Visibility requirements with the user's preference
			foreach( Bar bar in bars )
			{
				this.ShowOrHideBar( bar, manager, show );
			}
		}

		private void ShowOrHideBar( Bar bar, BarManager manager, bool show )
		{
			bool showBar = show;
			if( !bar.Manager.DesignMode )
				showBar &= ( ( bar.BarStyle & BarStyle.Visible ) > 0 | ( bar.BarStyle & BarStyle.IsMainMenu ) > 0 );

			BarID barID = new BarID( bar.BarName, BarManager.GetFormTypeName( bar.Manager ) );
			string barIDAsString = barID.ToString();

			if( this.IsUserPrefAvailable( barIDAsString ) )
				showBar = this.GetUserVisibilityPrefOfBar( barIDAsString );
			if( bar is MergedBar )
			{
				MergedBar mb = bar as MergedBar;
				if( !mb.CurrentVisibleStateDueToMerge )
					showBar = false;
			}

			CommandBarExt commandBar = this.commandbarsByBarId[ barIDAsString ] as CommandBarExt;
			if( commandBar != null )
			{
				if( showBar )
					commandBar.Visible = true;
				else
					commandBar.Visible = false;
			}
		}

		public void ShowBars( BarManager manager )
		{
			if( this.barsToHideInManagers.Count > 0 )
			{
				foreach( BarManager tobeHiddenManager in this.barsToHideInManagers )
				{
					if( tobeHiddenManager.Form.GetType() == manager.Form.GetType() )
					{
						this.barsToHideInManagers.Remove( tobeHiddenManager );
						break;
					}
				}
			}

			this.ShowOrHideBars( manager, true );
		}
		public void HideBars( BarManager manager, bool hideImmediately )
		{
			foreach( BarManager tobeHiddenManager in this.barsToHideInManagers )
			{
				if( tobeHiddenManager.Form.GetType() == manager.Form.GetType() )
				{
					this.barsToHideInManagers.Remove( tobeHiddenManager );
					break;
				}
			}
			this.barsToHideInManagers.Add( manager );

			// Don't hide it immediately, wait a while
			//if(hideImmediately)
			this.HideBarsEvent( this, EventArgs.Empty );
			//else
			//	this.hideTimer.Start();
		}

		public void RedrawBar( Bar bar )
		{
			if( bar == null )
				return;

			if( bar == this.mainMenuBar )
				this.mainMenuCommandBar.OnBarBoundsChanged();
			else
			{
				CommandBarExt commandBar = this.GetCommandBarFromBar( bar );
				if( commandBar != null )
					commandBar.OnBarBoundsChanged();
			}

		}

		public bool GetUserVisibilityPrefOfBar( Bar bar )
		{
			BarID barID = new BarID( bar.BarName, BarManager.GetFormTypeName( bar.Manager ) );
			string barIDAsString = barID.ToString();

			return this.GetUserVisibilityPrefOfBar( barIDAsString );
		}

		private bool GetUserVisibilityPrefOfBar( string barIdAsString )
		{
			if( this.barIdVisibilityPref[ barIdAsString ] != null )
				return ( bool )this.barIdVisibilityPref[ barIdAsString ];
			return true;
		}
		public bool IsUserPrefAvailable( Bar bar )
		{
			BarID barID = new BarID( bar.BarName, BarManager.GetFormTypeName( bar.Manager ) );
			string barIDAsString = barID.ToString();

			return this.IsUserPrefAvailable( barIDAsString );
		}

		private bool IsUserPrefAvailable( string barIdAsString )
		{
			return this.barIdVisibilityPref[ barIdAsString ] != null;
		}
		#endregion BAR_VISIBILITY
		#region BARS_CONTEXT_MENU
		protected internal virtual void StartListeningForMousePress( Control control )
		{
			//control.MouseDown += new MouseEventHandler(this.MousePressed);
			this.popupMenusManager.SetXPContextMenu( control, this.toolbarListPopup );
		}
		protected internal virtual void StopListeningForMousePress( Control control )
		{
			if( control != null && this.popupMenusManager != null )
			{
				//control.MouseDown -= new MouseEventHandler(this.MousePressed);
				this.popupMenusManager.SetXPContextMenu( control, null );
			}
		}
		//		private void MousePressed(object sender, MouseEventArgs e)
		//		{
		//			if(e.Button == MouseButtons.Right && !this.manager.Customizing)
		//			{
		//				PopupMenu popup = this.CreateBarVisibilityMenu();
		//				// Show only if there is atleast one toolbar.
		//				if(popup.ParentBarItem.Items.Count > 0)
		//					popup.Show(sender as Control, new Point(e.X, e.Y));
		//			}
		//		}
		//		protected virtual PopupMenu CreateBarVisibilityMenu()
		//		{
		//			PopupMenu popup = new PopupMenu();
		//			ParentBarItem parent = new ParentBarItem();
		//			parent.Manager = new BarManager(); // Some dummy bar manager.
		//			popup.ParentBarItem = parent;
		//
		//			this.PrepareToolbarListItem(parent);
		//			
		//			return popup;
		//		}
		/// <summary>
		/// This method will be called to fill the ParentBarItem that represents the menu
		/// that drops down when right-clicking in the toolbar area.
		/// </summary>
		/// <param name="parent">The <see cref="ParentBarItem"/> that represents the dropdown menu.</param>
		public virtual void PrepareToolbarListItem( ParentBarItem parent )
		{
			parent.Items.Clear();
			parent.Style = this.BarManager.Style;
			foreach( CommandBarExt commandBar in this.commandbarsByBarId.Values )
			{
				if( commandBar.Bar != this.mainMenuBar && commandBar.Bar.AllowHiding )
				{
					bool bShouldShow = true;

					MergedBar bar = commandBar.Bar as MergedBar;
					if( bar != null )
					{
						bShouldShow = bar.CurrentVisibleStateDueToMerge;
					}

					if( !bShouldShow ) continue;

					string strText = commandBar.Text;
					if( commandBar.Bar != null )
					{
						strText = ( commandBar.Bar.Caption == null ) ? commandBar.Text :
									commandBar.Bar.Caption;
					}

					BarItem item = new BarItem( strText );
					item.Tag = commandBar;
					item.Checked = commandBar.Visible;
					item.MergeOrder = commandBar.Bar.MenuItemMergeOrder;
					item.Click += new EventHandler( this.UserChangedBarVisibility );
					// Insert items according to MergeOrder
					parent.Items.Insert( parent.Items.FindMergePosition( item.MergeOrder ), item );
				}
			}
			// Customize item
			if( this.BarManager.EnableCustomizing | this.BarManager.DesignMode )
			{
				parent.Items.Add( this.customizeItem );
				parent.BeginGroupAt( this.customizeItem );
			}
		}

		private void DockBarStateChanged( object sender, CommandBarController.DockBarStateEventArgs args )
		{
			if( args.Created )
			{
				this.StartListeningForMousePress( args.CommandDockBar );
				args.CommandDockBar.DoubleClick += new EventHandler( this.DockBarDoubleClick );
			}
			else
			{
				this.StopListeningForMousePress( args.CommandDockBar );
				args.CommandDockBar.DoubleClick -= new EventHandler( this.DockBarDoubleClick );
			}
		}
		private void DockBarDoubleClick( object sender, EventArgs e )
		{
			this.StartCustomize();
		}

		#endregion BARS_CONTEXT_MENU
		public void SetDesigntimeSelectedBar( Bar bar )
		{
			if( !this.manager.DesignMode )
				return;

			if( bar == null )
				this.SelectedCommandbar = null;
			else
			{
				CommandBarExt commandBar = this.GetCommandBarFromBar( bar );

				if( commandBar != null )
					this.SelectedCommandbar = commandBar;
			}
		}
		public CommandBarExt SelectedCommandbar
		{
			get { return this.selectedCommandBar; }
			set
			{
				if( this.selectedCommandBar != value )
				{
					this.selectedCommandBar = value;
					foreach( CommandBarExt commandBar in this.commandbarsByBarId.Values )
						commandBar.Refresh();
				}
			}
		}

		public void AttachBars( BarManager manager )
		{
			Bars bars = manager.Bars;
			string formtype = BarManager.GetFormTypeName( manager );
			if( this.barsByFormType[ formtype ] == bars )
				return;

			if( this.barsByFormType[ formtype ] != null )
				this.ReleaseBars( this.barsByFormType[ formtype ] as Bars );

			this.barsByFormType[ formtype ] = bars;

			// Listen to changes in this collection
			bars.CollectionChanged += new CollectionChangeEventHandler( this.BarCollectionChanged );

			bars.ItemPropertyChanged += new Syncfusion.ComponentModel.SyncfusionPropertyChangedEventHandler
				( this.BarPropertyChanged );

			this.LockBars();	// Locking/Unlocking ensures row order is maintained.
			foreach( Bar bar in bars )
				this.BindBar( bar );
			this.UnLockBars();

			if( manager.MainFrameBarManager != null
				&& manager.MainFrameBarManager.AutoLoadToolBarPositions )
			{
				this.LoadDesignerState( manager, false );
			}
		}

		public void LoadDesignerState( BarManager manager, bool loadImmediately )
		{
			if( ( manager is MainFrameBarManager && !this.loadedFromIsolatedStore )
					|| manager.DesignMode )
			{
				this.LoadDesignerStateInternal( manager, loadImmediately );
			}

			ChildFrameBarManager childManager = manager as ChildFrameBarManager;

			if( childManager != null && !IsLoadedChildFrameBarManager( childManager )
					&& !manager.DesignMode )
			{
				LoadChildState( childManager );
			}
		}

		/// <summary>
		/// Indicating whether the state for CommandBars of the ChildFrameBarManager loaded.
		/// </summary>
		private bool IsLoadedChildFrameBarManager( ChildFrameBarManager childManger )
		{
			bool bLoaded = false;

			if( childManger != null )
			{
				bLoaded = m_htLoadedForm.ContainsValue( childManger.FormName );

				if( !bLoaded )
				{
					bLoaded = controller.IsLoaded( childManger.Form );
				}
			}

			return bLoaded;
		}

		/// <summary>
		/// Loads state for CommandBars of the ChildFrameBarManager
		/// </summary>
		private void LoadChildState( ChildFrameBarManager manager )
		{
			if( manager != null )
			{
				MemoryStream barPosInfo = manager.barPosInfo;

				if( barPosInfo != null && barPosInfo.CanRead )
				{
					this.controller.SetChildStartLocation();
					m_htLoadedForm.Add( manager.FormName.GetHashCode(), manager.FormName );
				}
			}
		}

		internal void LoadDesignerStateInternal( BarManager manager, bool loadImmediately )
		{
			MemoryStream barPosInfo = manager.barPosInfo;
			bool loadedFromStream = false;

			if( barPosInfo != null && barPosInfo.CanRead )
			{
				loadedFromStream = true;
				barPosInfo.Position = 0;
				// Don't need to reset anymore as uniqeness is now provided by the MainFrameBarManager.PersistId
				// rather than in the bar names.
				//				if(!manager.DesignMode)
				//					this.ResetBarNamesTemporarily(true, manager);
				try
				{
					this.controller.LoadFromStream( barPosInfo );
				}
				catch { loadedFromStream = false; }
				//				if(!manager.DesignMode)
				//					this.ResetBarNamesTemporarily(false, manager);
			}
			if( manager.DesignMode && !loadedFromStream )
				this.SetDefaultBarPositions( manager.Bars );

			if( loadImmediately )
			{
				foreach( Bar bar in manager.Bars )
				{
					CommandBarExt cb = this.GetCommandBarFromBar( bar );
					if( cb != null )
						this.controller.LoadCommandBarStateFromCache( cb );
				}
			}
		}

		private void SetDefaultBarPositions( Bars bars )
		{
			if( bars.manager == null || bars.manager.Designer == null )
				return;

			int curTop = 0;
			NativeMethods.POINT mdiclientStartPos = new NativeMethods.POINT( 0, 0 );
			NativeMethods.ClientToScreen( this.manager.Designer.MdiClientWnd, ref mdiclientStartPos );
			curTop = mdiclientStartPos.Y;
			foreach( Bar bar in bars )
			{
				CommandBarExt commandBar = this.GetCommandBarFromBar( bar );
                if (commandBar != null)
                {
                    commandBar.RedockIfNeeded();

                    if (commandBar.DockState == CommandBarDockState.Float)
                    {
                        Rectangle rcFloat = commandBar.rcFloat;
                        if (rcFloat.Top == 0)
                            commandBar.TopLevelControl.Location = new Point(mdiclientStartPos.X, curTop);

                        curTop += rcFloat.Height;
                    }
                }
			}
		}

		// Toggle between the actual form type and "System.Windows.Forms.Form"(which is what
		// the type is in design mode.
		//		private void ResetBarNamesTemporarily(bool reset, BarManager manager)
		//		{
		//			string formtype = BarManager.GetFormTypeName(manager);
		//			Bars bars = this.barsByFormType[formtype] as Bars;
		//
		//			foreach(Bar bar in bars)
		//			{
		//				BarID barID = new BarID(bar.BarName, BarManager.GetFormTypeName(bar.Manager));
		//				string barIDAsString = barID.ToString();
		//				CommandBarExt commandBar = this.commandbarsByBarId[barIDAsString] as CommandBarExt;
		//
		//				if(reset)
		//				{
		//					barIDAsString = new BarID(bar.BarName, manager.CurrentBaseFormType).ToString();
		//				}
		//
		//				// This could be null because Mainmenu would not have been initialized yet.
		//				// which means we would lose positional info of main menus, which is OK.
		//				if(commandBar != null)
		//					commandBar.Name = barIDAsString; 
		//			}
		//		}

		public MemoryStream GetBarPositionInfo()
		{
			MemoryStream stream = new MemoryStream();
			this.controller.SaveToStream( stream );

			return stream;
		}

		public void RemoveBars( BarManager manager, bool saveCommandBarState )
		{
			this.HideBars( manager, true );

			this.ReleaseBars( manager.Bars );

			string formtype = BarManager.GetFormTypeName( manager );
			this.barsByFormType.Remove( formtype );

			foreach( Bar bar in manager.Bars )
			{
				if( saveCommandBarState )
					this.controller.AppendCommandBarInfoForPersistance( this.GetCommandBarFromBar( bar ) );
				this.UnbindBar( bar );
			}
		}

		public CommandBarExt GetCommandBarFromBar( Bar bar )
		{
			BarID barID = new BarID( bar.BarName, BarManager.GetFormTypeName( bar.Manager ) );
			string barIDAsString = barID.ToString();

			return this.commandbarsByBarId[ barIDAsString ] as CommandBarExt;
		}

		public void ReleaseBars( Bars bars )
		{
			bars.CollectionChanged -= new CollectionChangeEventHandler( this.BarCollectionChanged );

			bars.ItemPropertyChanged -= new Syncfusion.ComponentModel.SyncfusionPropertyChangedEventHandler
				( this.BarPropertyChanged );
		}

		private void BarCollectionChanged( object sender, CollectionChangeEventArgs args )
		{
			if( args.Action == CollectionChangeAction.Add )
			{
				this.BindBar( args.Element as Bar );
				this.SetDefaultBarPositions( sender as Bars );
			}
			else if( args.Action == CollectionChangeAction.Remove && null != this.controller )
			{
				this.UnbindBar( args.Element as Bar );
			}
		}

		private void BarPropertyChanged( object sender, SyncfusionPropertyChangedEventArgs e )
		{
			if(
				// Do this for runtime as well since users might change the BarName for localization purpose, for example
				/*this.manager.DesignMode &&*/
				e.PropertyName == "BarName" )
			{
				Bar bar = sender as Bar;
				BarID oldBarID = new BarID( ( string )e.OldValue, BarManager.GetFormTypeName( bar.Manager ) );
				CommandBarExt cmdBar = this.commandbarsByBarId[ oldBarID.ToString() ] as CommandBarExt;
				this.commandbarsByBarId.Remove( oldBarID.ToString() );

				if( cmdBar != null )
				{
					BarID newBarID = new BarID( ( string )e.NewValue, BarManager.GetFormTypeName( bar.Manager ) );
					this.commandbarsByBarId[ newBarID.ToString() ] = cmdBar;
				}
			}
		}

		protected internal void UnbindBar( Bar bar )
		{
			CommandBarExt commandBar = null;
			BarID barID;
			string barIDAsString = null;

			if( this.mainMenuBar == bar )
			{
				// If in DesignMode, recreate afresh, else reuse during runtime.
				if( this.BarManager.DesignMode )
				{
					// Necessary to recreate in DesignMode. Otherwise the designer the Bar becomes unselectable.
					this.mainMenuBar = null;
					this.mainMenuCommandBar = null;
				}
				else
				{
					barID = new BarID( bar.BarName, BarManager.GetFormTypeName( bar.Manager ) );
					barIDAsString = barID.ToString();
					commandBar = this.commandbarsByBarId[ barIDAsString ] as CommandBarExt;

					this.AttachMainMenuBar( null );
				}
			}

			barID = new BarID( bar.BarName, BarManager.GetFormTypeName( bar.Manager ) );
			barIDAsString = barID.ToString();

			// We used to remove the entry here. But, not doing so now because
			// the next time the same bar gets bound, we want it's visibility state to be restored.
			// this.barIdVisibilityPref.Remove(barIDAsString);

			if( commandBar == null )
			{
				commandBar = this.commandbarsByBarId[ barIDAsString ] as CommandBarExt;
			}

			if( commandBar != null )
			{
				this.StopListeningForMousePress( commandBar );
				this.StopListeningForMousePress( commandBar.BarControl );

				commandBar.Visible = false;
				// This is necessary so that the bar renderer will be cleared (otherwise, it's still listening to events, etc.)
				commandBar.Bar = null;
				this.controller.CommandBars.Remove( commandBar );
				this.commandbarsByBarId.Remove( barIDAsString );
				commandBar.Dispose();
			}
		}

		protected void BindBar( Bar bar )
		{
			// If this is called for the main-frame go ahead and bind it to CommandBar, 
			// the merged main-menu will subsequently replace this temporary main-menu bar.
			if( !this.showMainMenusAutomatically && ( bar.BarStyle & BarStyle.IsMainMenu ) > 0
				&& !( bar.Manager is MainFrameBarManager ) )
				return;

			if( ( bar.BarStyle & BarStyle.IsMainMenu ) > 0 )
				this.AttachMainMenuBar( bar );
			else
				this.BindBarInternal( bar );
		}
		protected internal virtual void InitQuickCustomizePopup( CommandBarExt commandBar )
		{
			ParentBarItem parentItem = new ParentBarItem();
			BarManager barMan;// Dummy bar manager.

			MainFrameBarManager managerInfo = this.BarManager as MainFrameBarManager;
			if( managerInfo != null )
			{
				//Dummy Form. Need this to draw items visible.
				barMan = new MainFrameBarManager( new Form() );
				( barMan as MainFrameBarManager ).ThemesEnabled = managerInfo.ThemesEnabled;
			}
			else
			{
				barMan = new BarManager();
			}

			barMan.Style = this.BarManager.Style;
			parentItem.Manager = barMan;
			parentItem.CloseOnClick = false;
			commandBar.PopupMenu.ParentBarItem = parentItem;

			// Add the invisible items to the popup
			BarItems invisibleItems = commandBar.BarControl.barRenderer.InvisibleBarItems;

			foreach( BarItem item in invisibleItems )
				parentItem.Items.Add( item );

			// The Add Or Remove Buttons option
			if( ( commandBar.Bar.BarStyle & BarStyle.AllowQuickCustomizing ) > 0 )
			{
				ParentBarItem addOrRemoveButtonsItem
					= this.GetAddOrRemoveButtonsParentItem( parentItem.Manager, commandBar );

				parentItem.Items.Add( addOrRemoveButtonsItem );
			}
		}

		protected virtual ParentBarItem GetAddOrRemoveButtonsParentItem( BarManager manager, CommandBarExt commandBar )
		{
			ParentBarItem parentItem = new ParentBarItem( SR.GetString( SR.AddOrRemoveButtons,manager ) );
			parentItem.Manager = manager;
			parentItem.Manager.Style = this.BarManager.Style;

			MainFrameBarManager itemManager = parentItem.Manager as MainFrameBarManager;
			MainFrameBarManager managerInfo = this.BarManager as MainFrameBarManager;
			if( itemManager != null && managerInfo != null )
			{
				itemManager.ThemesEnabled = managerInfo.ThemesEnabled;
			}

			parentItem.Items.Add( this.GetButtonsList( commandBar, parentItem.Manager ) );

			// Customize item
			if( this.BarManager.EnableCustomizing | this.BarManager.DesignMode )
			{
				parentItem.Items.Add( this.customizeItem );
				parentItem.BeginGroupAt( this.customizeItem );
			}

			return parentItem;
		}

		protected virtual ParentBarItem GetButtonsList( CommandBarExt commandBar, BarManager manager )
		{
			ParentBarItem parentItem = new ParentBarItem( commandBar.Text );
			parentItem.CloseOnClick = false;
			parentItem.Manager = manager;
			parentItem.Manager.Style = this.BarManager.Style;

			// This could be a merged bar.
			IMergedContainer mergedContainer = commandBar.Bar as IMergedContainer;
			// Add the list of buttons to the parent
			foreach( BarItem item in commandBar.Bar.Items )
			{
				if( item.Visible == false )
					continue;

				// If this item was hidden by merge than don't bother with that item.
				if( mergedContainer != null )
				{
					if( mergedContainer.IsItemHiddenByMerge( item ) )
						continue;
				}
				parentItem.Items.Add( new ButtonVisibilityToggleBarItem( item, commandBar.Bar ) );
			}

			if( this.BarManager.EnableCustomizing | this.BarManager.DesignMode )
			{
				ResetToolBarBarItem resetToolbarItem = new ResetToolBarBarItem( this.manager );
				resetToolbarItem.Text = SR.GetString( SR.ResetToolBarMenu, this.BarManager);
				resetToolbarItem.CurrentBar = commandBar;

				parentItem.Items.Add( resetToolbarItem );

				parentItem.BeginGroupAt( resetToolbarItem );
			}
			return parentItem;
		}

		protected virtual BarControlInternal CreateBarControl()
		{
			return new BarControlInternal();
		}

		protected virtual CommandBarExt CreateCommandBar()
		{
			return new CommandBarExt( this );
		}

		protected CommandBarExt BindBarInternal( Bar bar )
		{
			if( this.manager == null ) return null;

			BarID barID = new BarID( bar.BarName, BarManager.GetFormTypeName( bar.Manager ) );
			string barIDAsString = barID.ToString();

			CommandBarExt commandBar = this.commandbarsByBarId[ barIDAsString ] as CommandBarExt;
			if( commandBar == null )
			{
				// If one doesn't exist for this already, create a new one
				commandBar = this.CreateCommandBar();

				commandBar.SuspendLayout();

				this.StartListeningForMousePress( commandBar );
				this.commandbarsByBarId[ barIDAsString ] = commandBar;                
                
                this.controller.CommandBars.Add( commandBar );
                if (!(this.manager is MainFrameBarManager))
                {
                    if (manager.Designer != null)
                    {
                        Syncfusion.Windows.Forms.Tools.Design.BarManagerDesigner designer = this.manager.Designer.DesignerHost.GetDesigner(this.manager) as Syncfusion.Windows.Forms.Tools.Design.BarManagerDesigner;
                        if (designer != null && !designer.Active)
                            commandBar.Controller.bLoadVisibility = false;
                    }

					commandBar.DisableDocking = true;
					commandBar.DockState = CommandBarDockState.Float;
				}
                else
                {
                    commandBar.DockState = CommandBarDockState.Top;
                }

				// Don't use the BarId since the Name will be queried by testing tools.
				// Also it's not necessary now as uniqueness is provided by the PersistID property.
				//commandBar.Name = barIDAsString;

				//this.manager.InitCustomizationDialog();

				// Create the BarControl and add it to the CommandBar
				BarControlInternal barControl = this.CreateBarControl();
				this.StartListeningForMousePress( barControl );

				commandBar.AttachBarControl( barControl );
				commandBar.Bar = bar;                

				barControl = null;

				if( !this.manager.DesignMode
					&& this.manager.MainFrameBarManager != null
					&& this.manager.MainFrameBarManager.AutoLoadToolBarPositions )
				{
					this.controller.LoadCommandBarStateFromCache( commandBar );
				}

				commandBar.ResumeLayout();
			}
			else
			{
				commandBar.Bar = bar;
			}
			return commandBar;
		}

		/// <summary>
		/// Gets or sets colorschemes for Office2007 visual style.
		/// </summary>
		[
		Description( "Colorschemes for Office2007 visual style." ),
		Category( "Appearance" ),
		DefaultValue( Office2007Theme.Blue )
		]
		public Office2007Theme Office2007Theme
		{
			get
			{
				Office2007Theme theme = Office2007Theme.Blue;

				if( controller != null )
				{
					theme = controller.Office2007Theme;
				}

				return theme;
			}
			set
			{
				if( controller != null )
				{
					controller.Office2007Theme = value;
				}
			}
		}
        /// <summary>
        /// Gets or sets colorschemes for Office2010 visual style.
        /// </summary>
        [
        Description("Colorschemes for Office2010 visual style."),
        Category("Appearance"),
        DefaultValue(Office2010Theme.Blue)
        ]
        public Office2010Theme Office2010Theme
        {
            get
            {
                Office2010Theme theme = Office2010Theme.Blue;

                if (controller != null)
                {
                    theme = controller.Office2010Theme;
                }

                return theme;
            }
            set
            {
                if (controller != null)
                {
                    controller.Office2010Theme = value;
                }
            }
        }
	}


	[Syncfusion.Documentation.DocumentationExclude()]
	public interface IBarHost
	{
		void OnBarBoundsChanged();
		void MoveMenuNavigation( bool forward );
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public class CommandBarExt : CommandBar, IBarHost, ICanCancel
	{
        internal const int DEF_HEIGHT_ADJUST = 2;
		private int suspendRecalc = 0;
		private BarControlInternal barControl;
		private CommandBarManager manager;
		private bool updateAfterThemeChange = false;
		private Timer repaintTimer = null;
		private bool needRepaint = false;
		private bool needLayout = false;
		private int gripperWidth = 14;
		private Point clickPoint;
		private bool parentResizing = false;
		private Size parentSize = Size.Empty;
		private Size originalParentSize = Size.Empty;
		private CancelListener cancelListener = null;
		private ThemedStatusBarDrawing themedDrawing = null;

		#region FOCUS_TRANSFER_LOGIC
		#endregion FOCUS_TRANSFER_LOGIC

		public CommandBarExt( CommandBarManager manager )
		{
			this.manager = manager;
			this.SetStyle( ControlStyles.ResizeRedraw, false );
			this.ShowDockModeText = false;

			BarManager barMan = this.manager.BarManager;
			MainFrameBarManager mainFrameBarMan = barMan.MainFrameBarManager;

			if( null != mainFrameBarMan )
				mainFrameBarMan.PropertyChanged
					+= new SyncfusionPropertyChangedEventHandler( Manager_PropertyChanged );

			this.repaintTimer = new Timer();
			this.repaintTimer.Interval = 100;
			this.repaintTimer.Tick += new EventHandler( this.Repaint_Tick );

			if( XPThemes.IsThemedOS )
				this.themedDrawing = new ThemedStatusBarDrawing();

			this.RightToLeft = null != mainFrameBarMan ? mainFrameBarMan.RightToLeft : RightToLeft.Inherit;
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				this.repaintTimer.Tick -= new EventHandler( this.Repaint_Tick );

				this.Bar = null;
				BarManager barMan = null;
				if( null != this.manager )
					barMan = this.manager.BarManager;

				if( null != barMan )
				{
					MainFrameBarManager mainBarMan = barMan.MainFrameBarManager;

					if( null != mainBarMan )
					{
						mainBarMan.PropertyChanged -= new SyncfusionPropertyChangedEventHandler( Manager_PropertyChanged );
					}
				}

				this.manager = null;

				if( this.themedDrawing != null )
				{
					this.themedDrawing.Dispose();
					this.themedDrawing = null;
				}

				DetachCurBarControl();
				if (this.barControl != null)
					this.barControl.Dispose();
					this.barControl = null;
			}
			base.Dispose( disposing );
		}

		protected override void OnPaintBackground( PaintEventArgs pevent )
		{
			bool customBackground = false;
			if( this.Bar != null )
			{
				customBackground = this.Bar.OnDrawBackground( pevent );
			}
			if( !customBackground )
				base.OnPaintBackground( pevent );

			if( this.StatusBarOn && this.IsLeadingStatusBar() )
			{
				int x = this.Width - this.gripperWidth;
				if( this.ThemesActive )
					this.themedDrawing.DrawStatusGripper( pevent.Graphics, new Rectangle( x, 0, this.gripperWidth, this.Height ) );
				else
					ControlPaint.DrawSizeGrip( pevent.Graphics, Color.Transparent, x, 0, this.gripperWidth, Height );
			}
		}

		#region STATUSBAR_LOGIC
		protected override void OnSizeChanged( EventArgs e )
		{
			base.OnSizeChanged( e );
			// Necessary if drawing gripper
			if( this.StatusBarOn )
				this.Invalidate();

			UpdateDropdownButton( this.Bar );
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			if( Bar != null && Bar.Manager != null && Bar.Manager.Form != null )
			{
				this.Bar.Manager.Form.Activate();
			}
           
			base.OnMouseDown( e );
			if( CommandBar.IsDragging && this.Bar != null && this.ThemesActive )
				this.repaintTimer.Start();
			else if( e.Button == MouseButtons.Left && this.manager != null && !this.manager.BarManager.DesignMode )
			{
				// Is mouse down on resize gripper
				if( this.StatusBarOn && this.IsLeadingStatusBar() )
				{
					Form parentForm = this.FindForm();
					if( parentForm.WindowState != FormWindowState.Maximized )
					{
						this.clickPoint = new Point( e.X, e.Y );
						Rectangle rc = new Rectangle( Width - this.gripperWidth, 0, gripperWidth, Height );
						//						if(PointToScreen(new Point(Width,Height))==Parent.PointToScreen(new Point(Parent.ClientRectangle.Width,Parent.ClientRectangle.Height))
						if( rc.Contains( clickPoint ) )
						{
							this.ParentResizing = true;
						}
					}
				}
			}
		}
		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );
			if( this.OccupyFullRow && !this.manager.BarManager.DesignMode )
			{
				if( this.StatusBarOn && this.IsLeadingStatusBar() )
				{
					Form parentForm = this.FindForm();
					if( parentForm.WindowState != FormWindowState.Maximized )
					{
						Rectangle rc = new Rectangle( Width - this.gripperWidth, 0, this.gripperWidth, Height );
						if( rc.Contains( new Point( e.X, e.Y ) ) )
						{
							Cursor.Current = Cursors.SizeNWSE;
						}
						else
						{
							Cursor.Current = Cursors.Default;
						}
						if( this.ParentResizing )
						{
							this.parentSize = new Size( parentSize.Width + e.X - clickPoint.X, parentSize.Height + e.Y - clickPoint.Y );
							parentForm.Size = parentSize;
							clickPoint = new Point( e.X, clickPoint.Y );
							parentForm.Update();
						}
					}
				}

			}
		}
		void ICanCancel.CancelOperation()
		{
			if( this.ParentResizing )
			{
				Size originalSize = this.originalParentSize;
				this.ParentResizing = false;
				this.FindForm().Size = originalSize;
			}
		}
		private bool StatusBarOn
		{
			get
			{
				if( this.Bar != null
					&& ( this.Bar.BarStyle & BarStyle.IsStatusBar ) > 0
					&& !this.DisableDocking )
				{
					Form form = this.FindForm();
					if( form != null && form.WindowState == FormWindowState.Maximized )
						return false;

					return true;
				}
				else
					return false;
			}
		}
		// This helps to know when there are more than one status bars.
		private bool IsLeadingStatusBar()
		{
			Form form = this.FindForm();
			if( form == null )
				return false;

			Rectangle myScrnBounds = this.RectangleToScreen( this.ClientRectangle );
			Rectangle formScrnBounds = form.RectangleToScreen( form.ClientRectangle );
			if( myScrnBounds.Bottom == formScrnBounds.Bottom
				&& myScrnBounds.Right == formScrnBounds.Right )
			{
				return true;
			}
			return false;
		}
		protected override bool ShouldDrawThemed()
		{
			return base.ShouldDrawThemed() && !this.StatusBarOn;// && !this.IsLeadingStatusBar();
		}
		private bool ParentResizing
		{
			get { return this.parentResizing; }
			set
			{
				if( this.parentResizing != value )
				{
					this.parentResizing = value;
					if( this.parentResizing )
					{
						this.Capture = true;
						Cursor.Current = Cursors.SizeNWSE;
						this.parentSize = this.FindForm().Size;
						this.originalParentSize = this.parentSize;
						this.cancelListener = new CancelListener( this );
					}
					else
					{
						this.Capture = false;
						Cursor.Current = Cursors.Default;
						this.parentSize = Size.Empty;
						this.originalParentSize = Size.Empty;
						this.cancelListener.Release();
						this.cancelListener = null;
					}
				}
			}
		}
		#endregion STATUSBAR_LOGIC
		protected override void OnCommandBarUserClosed( EventArgs arg )
		{
			base.OnCommandBarUserClosed( arg );
			this.manager.SetBarVisibility( this.Bar, false );

			if( manager != null && manager.BarManager != null )
			{
				this.manager.BarManager.OnFloatingFormClosed( this.Bar );
			}
		}
		protected override void OnLocationChanged( EventArgs e )
		{
			if( this.needRepaint )
			{
				this.Invalidate( true );
				this.needRepaint = false;
			}
			base.OnLocationChanged( e );
		}

		private void Repaint_Tick( object sender, EventArgs e )
		{
			if( Control.MouseButtons != MouseButtons.Left )
				this.repaintTimer.Stop();
			if( this.DockState != CommandBarDockState.Float
				&& this.Bar != null && this.ThemesActive )
			{
				this.needRepaint = true;
			}
		}

		protected override void OnPopupClosed( object sender, EventArgs e )
		{
			MainFrameBarManager mainBarMan = GetMainManager();
			if( mainBarMan != null )
			{
				mainBarMan.ShouldHidePopup = false;
			}

			base.OnPopupClosed( sender, e );
		}

		private MainFrameBarManager GetMainManager()
		{
			if( manager == null ) return null;

			MainFrameBarManager barManager = manager.BarManager as MainFrameBarManager;
			if( barManager != null ) return barManager;

			ChildFrameBarManager childMan = manager.BarManager as ChildFrameBarManager;
			if( childMan != null )
			{
				barManager = childMan.MainFrameBarManager;
			}

			return barManager;
		}
		protected override bool ShowDropDown()
		{
			this.manager.InitQuickCustomizePopup( this );
			MainFrameBarManager mainManager = this.GetMainManager();
			if( mainManager != null )
				mainManager.ShouldHidePopup = true;

			return base.ShowDropDown();
		}

		public BarControlInternal BarControl
		{
			get { return this.barControl; }
		}
		public Bar Bar
		{
			get
			{
				if( this.barControl != null )
					return this.barControl.Bar;
				else
					return null;
			}
			set
			{
				if( null != this.barControl && this.barControl.Bar != value )
				{
					if( Bar != null && Bar.Manager != null && Bar.Manager.Form != null )
					{
						this.Bar.Manager.Form.Deactivate -= new EventHandler( Form_Deactivate );
					}

					if( this.barControl.Bar != null )
					{
						Bar bar = this.barControl.Bar;
						bar.CaptionChanged -= new TextChangedEventHandler(this.bar_CaptionChanged);
						bar.PropertyChanged -= new SyncfusionPropertyChangedEventHandler( this.BarPropertyChanged );

						if( bar.Manager != null && !bar.Manager.IsDummyManager() )
							bar.Manager.OnBarControlBindingChanged( new BarControlBindingChangedArgs( bar, this, BarControlBindingChangeType.Unparenting ) );
					}
					this.barControl.Bar = value;

					if( this.barControl.Bar != null )
						this.barControl.Bar.PropertyChanged += new SyncfusionPropertyChangedEventHandler( this.BarPropertyChanged );

					if( Bar != null && Bar.Manager != null && Bar.Manager.Form != null )
					{
						this.Bar.Manager.Form.Deactivate += new EventHandler( Form_Deactivate );
					}

					this.OnBarChanged();

					if( this.barControl.Bar != null )
					{
						Bar bar = this.barControl.Bar;
						if( bar.Manager != null && !bar.Manager.IsDummyManager() )
							bar.Manager.OnBarControlBindingChanged( new BarControlBindingChangedArgs( bar, this, BarControlBindingChangeType.Parented ) );
						this.SynthesizeName();
					}
				}
			}
		}

		private void SynthesizeName()
		{
			if( this.Bar == null )
				return;
			
			this.Name = SynthesizeNameFromBar(this.Bar);
		}

        internal static string SynthesizeNameFromBar(Bar bar)
        {
            string newName = "ToolbarHost_" + bar.BarName;
            newName = newName.Replace(' ', '_');

            return newName;
        }

		/// <summary>
		/// Gets name of the MDI Children form which needs save state.
		/// </summary>
		internal string MdiChildrenFormName
		{
			get
			{
				string name = String.Empty;

				if( manager != null && manager.BarManager != null && manager.BarManager.MainFrameBarManager != null )
				{
					name = manager.BarManager.MainFrameBarManager.MdiChildrenFormName;
				}

				return name;
			}
		}


		/// <summary>
		/// Gets a value indicates whether the <see cref="CommandBarExt"/> 
		/// relate to <see cref="MainFrameBarManager"/>.
		/// </summary>
		internal bool IsMainCommandBar()
		{
			bool bMainCommandBar = false;

			if( manager != null && manager.BarManager != null
				&& manager.BarManager.MainFrameBarManager != null )
			{
				bMainCommandBar = manager.BarManager.MainFrameBarManager.IsMainCommandBar( this );
			}

			return bMainCommandBar;
		}


		public bool ProcessShortcut( Char c )
		{
			if( this.barControl != null )
				return this.barControl.ProcessShortcut( c );
			else
				return false;
		}

		private void BarPropertyChanged( object sender, SyncfusionPropertyChangedEventArgs args )
		{
			if( args.PropertyName == "BarStyle" )
				this.OnBarChanged();

			else if( args.PropertyName == "BarName" )
			{
				this.Text = this.Bar.BarName;
				this.Refresh();
				this.SynthesizeName();
			}
		}
		private new Color DefaultBackColor
		{
			get
			{
				if( this.Bar != null
					&& ( this.Bar.BarStyle & BarStyle.IsMainMenu ) > 0
					&& this.Bar.Manager is MainFrameBarManager )
					return MenuColors.MainMenuBackColor;
				else if( this.Bar != null && ( this.Bar.BarStyle & BarStyle.IsStatusBar ) > 0 )
				{
					return MenuColors.StatusBarBackColor;
				}
				else
				{
					return MenuColors.CommandBarBackColor;
				}
			}
		}

		protected override void OnFontChanged( EventArgs e )
		{
			base.OnFontChanged( e );
			this.OnBarBoundsChanged();
		}

		public void OnBarChanged()
		{
			if( this.Bar == null )
			{
				if( this.barControl != null )
					this.barControl.ActLikeMainMenu( null, false );
				return;
			}

			Bar bar = this.Bar;
			bar.CaptionChanged -= new TextChangedEventHandler( bar_CaptionChanged );
			bar.CaptionChanged += new TextChangedEventHandler( bar_CaptionChanged );

			this.Text = ( bar.Caption == null ) ? bar.BarName : bar.Caption;

			this.DockModeWrapping = ( bar.BarStyle & BarStyle.MultiLine ) > 0;
			this.FloatModeWrapping = true;
			this.OccupyFullRow = ( bar.BarStyle & BarStyle.UseWholeRow ) > 0;
			this.HideGripper = ( bar.BarStyle & BarStyle.DrawDragBorder ) == 0;
			UpdateDropdownButton( bar );
			this.HideCloseButton = bar.AllowHiding == false;

			bool showBar = true;
			BarManager barMan = this.manager.BarManager;

			if( null == barMan || !barMan.DesignMode )
			{
				showBar = BarStyle.Visible == ( bar.BarStyle & BarStyle.Visible );
			}

			// Get user's preference, if any:
			if( this.manager.IsUserPrefAvailable( bar ) )
				showBar = this.manager.GetUserVisibilityPrefOfBar( bar );
			if( bar is MergedBar )
			{
				MergedBar mb = bar as MergedBar;
				if( !mb.CurrentVisibleStateDueToMerge )
					showBar = false;
			}
			this.Visible = showBar;

			if( ( bar.BarStyle & BarStyle.IsMainMenu ) > 0 )
			{
				if( bar.Manager is MainFrameBarManager )
				{
					this.barControl.ActLikeMainMenu( bar.Manager.Form, null != barMan && barMan.DesignMode );
					//this.AlwaysLeadingEdge = true;
					this.OccupyFullRow = true;
					//this.DisableFloating = true;
				}
				this.AlwaysTrailingEdge = false;
				this.Visible = true;
				//this.HideGripper = true;
				this.HideChevron = true;
				this.HideDropDownButton = true;

				// THis is not necessary because Top is the default setting.
				// .. and this is not correct because CommandBars that were previously floated while acting as main-menu
				// will now be reset to dock to Top.
				//this.DockState = CommandBarDockState.Top;
			}
			else if( ( bar.BarStyle & BarStyle.IsStatusBar ) > 0 )
			{
				if( bar.Manager is MainFrameBarManager )
					this.barControl.ActLikeMainMenu( null, false );

				this.AlwaysLeadingEdge = false;
				this.AlwaysTrailingEdge = true;
				this.OccupyFullRow = true;
				this.DisableFloating = true;

				this.Visible = true;
				this.HideGripper = true;
				this.HideChevron = true;
				this.HideDropDownButton = true;

				this.DockState = CommandBarDockState.Bottom;
			}
			else
			{
				if( bar.Manager is MainFrameBarManager )
					this.barControl.ActLikeMainMenu( null, false );
				this.AlwaysLeadingEdge = false;
				this.AlwaysTrailingEdge = false;

				this.UpdateFloatModeRenderer( false );
				this.HideChevron = false;
			}

			this.InitBackColor();

			if( this.DockState == CommandBarDockState.Float )
				this.UpdateRenderer( false );
			else
				this.UpdateRenderer( true );
		}

		private void UpdateDropdownButton( Bar bar )
		{
			this.HideDropDownButton = !( this.AllowQuickCustomizing || this.IsChevronVisible );
			Invalidate();
		}

		protected bool IsMainMenu
		{
			get
			{
				bool bResult = false;
				Bar bar = this.Bar;

				if( bar != null )
				{
					bResult = (this.Bar.BarStyle & BarStyle.IsMainMenu) != 0;
				}

				return bResult;
			}
		}

		protected internal override bool AllowQuickCustomizing
		{
			get
			{
				bool bResult = false;
				Bar bar = this.Bar;
			
				if( bar != null )
				{
					bResult = (this.Bar.BarStyle & BarStyle.AllowQuickCustomizing) != 0
						&& (this.Bar.BarStyle & BarStyle.IsStatusBar) == 0;
				}

				return bResult && !this.IsMainMenu;
			}
		}

		protected internal override bool HasExternalPopupMenu
		{
			get
			{
				return false;
			}
		}

		private bool ThemesActive
		{
			get
			{
				if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.Bar != null
					&& this.Bar.Manager != null && this.Bar.Manager.MainFrameBarManager != null
					&& this.Bar.Manager.MainFrameBarManager.ThemesEnabled )
					return true;
				else
					return false;
			}
		}
		public override void MenuColorsChanged( object sender, EventArgs e )
		{
			this.InitBackColor();
			base.MenuColorsChanged( sender, e );
		}

		private void InitBackColor()
		{
			if( this.ThemesActive && this.Controller.Style != VisualStyle.Office2007
                && this.Controller.Style != VisualStyle.Office2007Outlook && this.Controller.Style != VisualStyle.Office2010)
			{
				this.ResetBackColor();
			}
			else
			{
				if( null != this.Controller && this.Controller.Style != VisualStyle.Office2003
					&& this.Controller.Style != VisualStyle.VS2005
					&& this.Controller.Style != VisualStyle.Office2007
                    && this.Controller.Style != VisualStyle.Office2010
					&& this.Controller.Style != VisualStyle.Office2007Outlook )
				{
					this.BackColor = this.DefaultBackColor;
					this.bBackColorSet = false;
				}
				else
				{
					// Also the Draw** overrides below will help with the bg drawing for main-menus
					if( this.Floating )
					{
						this.BackColor = this.Parent.BackColor;
					}
					else
					{
						this.BackColor = Color.Transparent;
					}

					this.bBackColorSet = false;
				}
			}
		}

		protected override void OnParentChanged( EventArgs e )
		{
			base.OnParentChanged( e );

			if( this.Parent != null )
				this.InitBackColor();
		}

		private void Manager_PropertyChanged( object sender, SyncfusionPropertyChangedEventArgs args )
		{
			switch( args.PropertyName )
			{
				case "ThemesEnabled":
				case "LargeIcons":
					{
                        this.bUpdateOffsets = true;

						this.InitBackColor();
						this.Invalidate( true );
						break;
					}

				case "RightToLeft":
					{
						this.RightToLeft = ( RightToLeft )args.NewValue;
						this.SetBoundsCore( this.Location.X, this.Location.Y, this.Width, this.Height, BoundsSpecified.All );
						this.Invalidate( true );
						break;
					}
			}
		}

		protected override void WndProc( ref Message m )
		{
			// remove WM_MOUSEACTIVATE messages 
			// to keep focus at floating form window

			if( m.Msg == NativeMethods.WM_MOUSEACTIVATE )
			{
				if( this.FindForm() != null )
				{
					m.Result = ( IntPtr )NativeMethods.MA_NOACTIVATE;
					return;
				}
			}

			if( m.Msg == 0x000f && this.updateAfterThemeChange )
			{
				this.updateAfterThemeChange = false;
				this.InitBackColor();
			}
			base.WndProc( ref m );
			if( m.Msg == 0x031A/*WM_THEMECHANGED*/)
			{
				this.updateAfterThemeChange = true;
			}
		}

		private void UpdateFloatModeRenderer( bool forceMulitiline )
		{
			if( forceMulitiline || ( this.Floating && !( this.BarControl.barRenderer is MultilineBarRenderer ) ) )
			{
				this.BarControl.forceMultiline = true;
				this.BarControl.RendererChanged( null );
				this.BarControl.forceMultiline = false;
			}
		}

		private void UpdateDockModeRenderer()
		{
			if( ( this.BarControl.Bar.BarStyle & BarStyle.MultiLine ) == 0 )
			{
				MultilineBarRenderer mlbr = this.BarControl.barRenderer as MultilineBarRenderer;

				if( mlbr != null )
				{
					this.BarControl.RendererChanged( null );
				}
			}
		}

		public void AttachBarControl( BarControlInternal barControl )
		{
			if( this.barControl != barControl )
				this.DetachCurBarControl();

			barControl.BarHost = this;
			barControl.Style = this.manager.GetCommandBarController().Style;
            if(barControl.Style==VisualStyle.Metro)
				barControl.MetroColor = this.manager.GetCommandBarController().MetroColor;
			this.Controls.Add( barControl );

			this.barControl = barControl;
			this.barControl.MouseUp += new MouseEventHandler( this.BarControl_MouseUp );
		}
		public void DetachCurBarControl()
		{
			if( this.barControl != null )
			{
				this.Controls.Remove( this.barControl );
				this.barControl.MouseUp -= new MouseEventHandler( this.BarControl_MouseUp );
			}
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );
			// Draw selected rect if in design mode.
			if( this.manager.SelectedCommandbar == this )
			{
				RectangleF bounds = this.ClientRectangle;
				e.Graphics.DrawRectangle( new Pen( SystemColors.ControlText, 2 ), Rectangle.Ceiling( bounds ) );
			}
		}

		// Always reference designer interfaces in a separate method which
		// you shouldn't call during runtime. So that the designer dlls don't get loaded.
		private void OnMouseUpDuringDesignTime( bool dragging )
		{
			IBarManagerDesigner designer = this.manager.BarManager.Designer;
			if( dragging )
				designer.SetDirty();
			if( this.Bar != null )
			{
				ISelectionService selService = designer.GetService( typeof( ISelectionService ) ) as ISelectionService;
				selService.SetSelectedComponents( new Object[ 1 ] { this.Bar } );
			}
		}
		protected override void OnMouseUp( MouseEventArgs e )
		{
			this.repaintTimer.Stop();
			bool dragging = CommandBar.IsDragging;
			base.OnMouseUp( e );
			if( this.manager.BarManager.DesignMode )
			{
				this.OnMouseUpDuringDesignTime( dragging );
			}
			this.ParentResizing = false;
		}
		private void BarControl_MouseUp( object sender, MouseEventArgs e )
		{
			if( this.Bar != null && this.Bar.DesignMode )
			{
				if( this.BarControl.barRenderer.HitTestBarItems( new PointF( e.X, e.Y ) ) == -1 )
				{
					IBarManagerDesigner designer = ( this.manager.BarManager.GetService( typeof( IDesignerHost ) ) as IDesignerHost ).GetDesigner( this.manager.BarManager ) as IBarManagerDesigner;
					if( designer != null )
					{
						ISelectionService selService = designer.GetService( typeof( ISelectionService ) ) as ISelectionService;
						selService.SetSelectedComponents( new Object[ 1 ] { this.Bar } );
					}
				}
			}
		}

		private void FloatingFormMouseUp( object sender, MouseEventArgs e )
		{
			bool dragging = CommandBarForm.Sizing;
			if( dragging && this.manager.BarManager.DesignMode )
			{
				IBarManagerDesigner designer = ( this.manager.BarManager.GetService( typeof( IDesignerHost ) ) as IDesignerHost ).GetDesigner( this.manager.BarManager ) as IBarManagerDesigner;
				designer.SetDirty();
			}
		}
		// While docking recalc after Changed event. While floating recalc before Changed event (in EnterFloatMode below)
		// This is necessary since the CommandBar framework expects the new sizes to be provided even before the Changed event.
		protected override void OnCommandBarStateChanged( EventArgs args )
		{
			if( this.DockState != CommandBarDockState.Float )
			{
				this.UpdateRenderer( true );
			}

			base.OnCommandBarStateChanged( args );
		}

		private void UpdateRenderer( bool todockmode )
		{
			if( this.Bar == null || this.BarControl == null )
				return;

			this.SuspendRecalc();

			if( todockmode )
			{
				// Changing to docked state
				this.UpdateDockModeRenderer();
				this.BarControl.Alignment = this.DockState;
			}
			else
			{
				this.UpdateFloatModeRenderer( true );
				// When this method is called from EnterFloatMode, my DockState has
				// not been updated yet to Float, so setting it explicity.
				this.BarControl.Alignment = CommandBarDockState.Float;
			}

			// Change the alignment within the SuspendRecalce, otherwise there will
			// be unnecessary multiple recalcs.
			this.ResumeRecalc( false );

			this.RecalcBarLayout( true, false );
		}

		// Updating renderer on floating.
		protected internal override void EnterFloatMode( Point ptscreen )
		{
			this.UpdateRenderer( false );

			base.EnterFloatMode( ptscreen );
		}

		public void SuspendRecalc()
		{
			this.suspendRecalc++;
		}

		public void ResumeRecalc( bool recalcIfNecessary )
		{
			this.suspendRecalc--;
			if( recalcIfNecessary && this.suspendRecalc == 0
				&& this.needLayout )
				this.RecalcBarLayout( false, false );
		}

		public bool NeedLayout
		{
			get { return this.needLayout; }
		}
		protected internal virtual void RecalcBarLayout( bool forceRecalc, bool dontRecalc )
		{
			if( this.barControl == null /*|| this.Parent == null*/)// Need Parent check?
				return;

			// CommandBar cannot handle layout calls when frozen.
			CommandBarController cbController = this.manager.GetCommandBarController();

			if( null != cbController && cbController.FreezeLayout )
			{
				if( forceRecalc && !dontRecalc )
					this.needLayout = true;
				return;
			}

			if( this.suspendRecalc > 0 )
			{
				this.needLayout = true;
				return;
			}

			this.needLayout = false;

			SizeF minSize = new SizeF( 0, 0 );

			UpdateDropdownButton( this.Bar );

			// Calculate the Min Length
			this.barControl.GetPreferredSize( ref minSize );
			int prevMinLength = this.MinLength;
			int newMinLength = this.CalcCommandBarMaxLength( ( int )Math.Ceiling( minSize.Width ) );
			if( prevMinLength != newMinLength )
				this.MinLength = newMinLength;

			// Calculate the Max Length
			SizeF maxSize = new SizeF( 0, 0 );
			maxSize = new SizeF( Int32.MaxValue, Int32.MaxValue );
			this.barControl.GetPreferredSize( ref maxSize );
			int prevMaxLength = this.MaxLength;
			int newMaxLength = this.CalcCommandBarMaxLength( ( int )Math.Ceiling( maxSize.Width ) );
			if( newMaxLength != prevMaxLength )
				this.MaxLength = newMaxLength;

			// Min height is when the width is max.
			int newMinHeight = ( int )maxSize.Height + CommandBarExt.DEF_HEIGHT_ADJUST;
			if( newMinHeight != this.MinHeight )
            {
                this.MinHeight = newMinHeight;
                if (this.cbController != null)
                    this.cbController.RecalcLayout(this);
            }

			if( prevMaxLength != this.MaxLength
				|| prevMinLength != this.MinLength || forceRecalc
				)
			{
				this.DetermineIntermediateHeights( Size.Ceiling( minSize ), Size.Ceiling( maxSize ) );
				if( !dontRecalc )
					this.barControl.PerformLayout();
			}
			//			else
			//				this.barControl.PerformLayout();
		}

		public override int CalcCommandBarMaxLength( int nctrlwidth )
		{
			int maxLength = base.CalcCommandBarMaxLength( nctrlwidth );

			if( this.StatusBarOn )
				maxLength += this.gripperWidth;

			return maxLength;
		}

		protected override void AdjustChildControlBounds( ref Rectangle childBounds )
		{
			base.AdjustChildControlBounds( ref childBounds );

			if( this.StatusBarOn )
			{
				childBounds.Width -= this.gripperWidth;
			}
		}

		public void OnBarBoundsChanged()
		{
            Rectangle bounds = this.Bounds;
			this.RecalcBarLayout( false, false );

			CommandDockBar commandDockBar = this.Parent as CommandDockBar;
			if( commandDockBar != null )
			{
                if ((this.Height == bounds.Height && (this.cbarDockState == CommandBarDockState.Top || this.cbarDockState == CommandBarDockState.Bottom)) ||
                     (this.Width == bounds.Width && (this.cbarDockState == CommandBarDockState.Left || this.cbarDockState == CommandBarDockState.Right)))
                {
                    this.bRedockNeededInternal = false;
                }

                if (this.bRedockNeededInternal)
                {
                    this.RedockIfNeeded();
                    this.bRedockNeededInternal = false;
                }

				commandDockBar.LayoutDockBar();
			}
		}

		public void MoveMenuNavigation( bool forward )
		{
			this.manager.MoveMenuNavigation( this, forward );
		}

		protected internal override Size GetDockWrapSize( Size szbounds )
		{
			Size szMax = this.CalcChildControlBounds( szbounds ).Size;
			SizeF size = szMax;
			if( ( this.barControl.Alignment == CommandBarDockState.Left || this.barControl.Alignment == CommandBarDockState.Right ) )
				size = new SizeF( size.Height, size.Width );
			// making space for gripper
			else if( this.StatusBarOn )
			{
				size.Width -= this.gripperWidth;
			}

			this.barControl.GetPreferredSize( ref size );

			if( ( this.barControl.Alignment == CommandBarDockState.Left || this.barControl.Alignment == CommandBarDockState.Right ) )
				size = new SizeF( size.Height, size.Width );

			return this.GetAdjustedSize( szMax, Size.Ceiling( size ) );
		}

		private ArrayList interSizes = new ArrayList();
		// minSize has the largest height, maxSize has the shortest
		protected virtual void DetermineIntermediateHeights( SizeF minSize, SizeF maxSize )
		{
			interSizes.Clear();

			interSizes.Add( minSize );
			interSizes.Add( maxSize );

			if( this.BarControl.barRenderer is SingleLineBarRenderer )
				return;

			// Start inserting between min and max
			int insertPos = 1;
			SizeF curSize = minSize;
			while( curSize.Height != maxSize.Height )
			{
				curSize.Width += 10;
				SizeF prefSize = curSize;
				this.barControl.GetPreferredSize( ref prefSize );
				if( prefSize.Height < curSize.Height )
				{
					if( prefSize.Height != maxSize.Height )
						interSizes.Insert( insertPos++, prefSize );
					curSize = prefSize;
				}
			}
		}

		protected internal override Size GetFloatWrapSize( Size szbounds, CommandBarResizeType rtsize )
		{
			Size szMax = this.CalcChildControlBounds( szbounds ).Size;
			SizeF szNew = szMax;

			if( rtsize == CommandBarResizeType.Bottom || rtsize == CommandBarResizeType.Top )
			{
				bool expanding = szbounds.Height > this.Bounds.Height;

				SizeF szMatch = SizeF.Empty;
				for( int i = this.interSizes.Count - 1; i >= 0; i-- )
				{
					SizeF sizeF = ( SizeF )this.interSizes[ i ];
					if( sizeF.Height > szNew.Height )
					{
						if( i < this.interSizes.Count - 1 )
							i++;
						szMatch = ( SizeF )this.interSizes[ i ];
						break;
					}
				}
				if( szMatch == SizeF.Empty )
					szMatch = ( SizeF )this.interSizes[ 0 ];

				szNew = szMatch;
			}
			else
			{
				SizeF minSize = (SizeF)this.interSizes[ 0 ];
				if( szNew.Width < minSize.Width )
					szNew.Width = minSize.Width;
				this.barControl.GetPreferredSize( ref szNew );
			}

			return this.GetAdjustedSize( szMax, Size.Ceiling( szNew ) );
		}
		protected override CommandBarForm CreateFloatingForm()
		{
			CommandBarForm form = base.CreateFloatingForm();
			this.manager.StartListeningForMousePress( form );
			form.MouseUp += new MouseEventHandler( FloatingFormMouseUp );
			if( this.Bar != null )
			{
				form.Name = "commandBarForm" + this.Bar.BarName;
			}
			return form;
		}

		private void Form_Deactivate( object sender, EventArgs e )
		{
			base.HideDropDown();
		}

		private void bar_CaptionChanged( object sender, TextChangedEventArgs e )
		{
			this.Text = ( e.Text == null ) ? Bar.BarName : e.Text;
		}

		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				if( !value && this.Floating )
				{
					CommandBarForm frmFloating = this.Parent as CommandBarForm;

					if( frmFloating != null )
					{
						this.rcFloat = frmFloating.Bounds;
					}
				}

				base.Visible = value;
			}
		}

		protected internal override Size CalculateFloatingSize()
		{
			if( this.Bar != null && ( this.Bar.BarStyle & BarStyle.IsMainMenu ) != 0 )
			{
				this.rcFloat.Size = Size.Empty;
			}				

			return base.CalculateFloatingSize();
		}
	}

	[Serializable,
	Syncfusion.Documentation.DocumentationExclude()
	]
	public class CommandBarExtSerializer : ISerializable, ICommandBarSerializer
	{
		protected internal int nMaxLength;
		protected internal int nMinLength;
		protected internal CommandBarDockState cbDockState;
		protected internal int nRowOffsetDir;
		protected internal int nRCIndex;
        protected internal int nRCCountDrag;
		protected internal Point ptFloat;
        protected internal Size szFloat;

		public CommandBarExtSerializer()
		{
		}

		public void GetCommandBarData( CommandBar cbar )
		{
			this.nMaxLength = cbar.nMaxLength;
			this.nMinLength = cbar.nMinLength;
			this.cbDockState = cbar.cbarDockState;
			this.nRowOffsetDir = cbar.nRowOffsetDir;
			this.nRCIndex = cbar.nRCIndex;
            this.nRCCountDrag = cbar.nRCCountDrag;
			this.ptFloat = cbar.FloatBounds.Location;
            this.szFloat = cbar.FloatBounds.Size;
		}

		public void SetCommandBarData( CommandBar cbar )
		{
			cbar.nMaxLength = this.nMaxLength;
			cbar.nMinLength = this.nMinLength;
			cbar.cbarDockState = this.cbDockState;
			cbar.nRowOffsetDir = this.nRowOffsetDir;
			cbar.nRowOffsetInDir = cbar.nRowOffsetDir;
			cbar.nRCIndex = this.nRCIndex;
            cbar.nRCCountDrag = this.nRCCountDrag;
			cbar.rcFloat = new Rectangle(this.ptFloat, this.szFloat);
		}

		// Private constructor called during the deserialization process
		protected CommandBarExtSerializer( SerializationInfo info, StreamingContext context )
		{
			this.nMaxLength = info.GetInt32( "MaxLength" );
			this.nMinLength = info.GetInt32( "MinLength" );

			this.cbDockState = ( CommandBarDockState )info.GetValue( "CommandBarDockState", typeof( CommandBarDockState ) );
			this.nRowOffsetDir = info.GetInt32( "RowOffsetDir" );
			this.nRCIndex = info.GetInt32( "RCIndex" );
			this.ptFloat = ( Point )info.GetValue( "FloatLocation", typeof( Point ) );
            
            try
            {
                //Need to skip RCCoundDrag - if it isn't included into serialization info.
                this.nRCCountDrag = info.GetInt32("RCCountDrag");
                this.szFloat = (Size)info.GetValue("FloatSize", typeof(Size));
            }
            catch(SerializationException)
            {
                this.nRCCountDrag = 0;
                this.szFloat = Size.Empty;
            }
        }

		// ISerializable implementation
		public void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue( "MaxLength", this.nMaxLength );
			info.AddValue( "MinLength", this.nMinLength );

			info.AddValue( "CommandBarDockState", this.cbDockState );
			info.AddValue( "RowOffsetDir", this.nRowOffsetDir );
			info.AddValue( "RCIndex", this.nRCIndex );
            info.AddValue( "RCCountDrag", this.nRCCountDrag );
			info.AddValue( "FloatLocation", this.ptFloat );
            info.AddValue( "FloatSize", this.szFloat );
		}
	}
	[Syncfusion.Documentation.DocumentationExclude(),
	ToolboxItem( false )]
	public class ToolbarListPopupMenu : PopupMenu
	{
		CommandBarManager cbm;
		public ToolbarListPopupMenu( CommandBarManager cbm )
		{
			this.cbm = cbm;

			ParentBarItem parentBarItem = new ParentBarItem();
			parentBarItem.Manager = new BarManager(); // Some dummy bar manager.
			this.ParentBarItem = parentBarItem;
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( this.ParentBarItem != null )
				{
					this.ParentBarItem.Dispose();
					this.ParentBarItem = null;
				}
			}
			base.Dispose( disposing );
		}
		public override IPopupChild ShowChildrenUI( Point pos, IPopupParent parentUI )
		{
			if( !this.cbm.Customizing )
			{
				this.ParentBarItem.Items.Clear();
				this.ParentBarItem.SeparatorIndices.Clear();
				this.cbm.PrepareToolbarListItem( this.ParentBarItem );
				foreach( BarItem bitem in this.ParentBarItem.Items )
					bitem.CustomTextFont = cbm.BarManager.MainFrameBarManager.Font;
				// Show only if there is atleast one toolbar.
				if( this.ParentBarItem.Items.Count > 0 )
					base.ShowChildrenUI( pos, parentUI );
			}
			return null;
		}

		/// <summary>
		/// Indicates whether to show shadows for Popups.
		/// </summary>
		public bool ShowShadow
		{
			get
			{
				bool bShowShadow = false;

				if( cbm != null && cbm.BarManager != null )
				{
					bShowShadow = cbm.BarManager.ShowShadow;
				}

				return bShowShadow;
			}
		}
	}
}
