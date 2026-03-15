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
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Runtime.Serialization;

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	public interface IAmACustomizationForm
	{
	}
	internal class MainFormWndSubclass: NativeWindowSubclass
	{
		bool active = false;

		/// <summary>
		/// Gets or sets the Active control
		/// </summary>
		public bool Active
		{
			get { return this.active; }
			set
			{
				if( this.active != value )
				{
					this.active = value;
				}
			}
		}

		protected override void WndProc( ref Message m )
		{
			if( m.Msg == 0x21/*WM_MOUSEACTIVATE*/ && active )
			{
				if( NativeMethods.LOWORD( m.LParam.ToInt32() ) == 1/*HTCLIENT*/)
					m.Result = (IntPtr)3/*MA_NOACTIVATE*/;
				else
					m.Result = (IntPtr)4/*MA_NOACTIVATEANDEAT*/;
				return;
			}
			base.WndProc( ref m );
		}
	}

	[
		// This method not supported in this version.
	Syncfusion.Documentation.DocumentationExclude(),
	Browsable( false )]
	public class DockBarPaintEventArgs
	{
		private PaintEventArgs args;
		private CommandBarDockBorder border;
		private Rectangle bounds;

		public DockBarPaintEventArgs( PaintEventArgs args, CommandBarDockBorder border,
			Rectangle bounds )
		{
			this.args = args;
			this.border = border;
			this.bounds = bounds;
		}

		/// <summary>
		/// Gets the Command bar dock border
		/// </summary>
		public CommandBarDockBorder Border
		{
			get { return this.border; }
		}

		/// <summary>
		/// Gets the PaintEventArgs
		/// </summary>
		public PaintEventArgs PaintEventArgs
		{
			get { return this.args; }
		}

		/// <summary>
		/// Gets the control bounds
		/// </summary>
		public Rectangle Bounds
		{
			get { return this.bounds; }
		}
	}

	/// <summary>
	/// DockBarPaintEventHandler delegate
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	[
		// This method not supported in this version.
	Syncfusion.Documentation.DocumentationExclude(),
	Browsable( false )]
	public delegate void DockBarPaintEventHandler( object sender, DockBarPaintEventArgs args );

	/// <summary>
	/// UpdateType Enumerator
	/// </summary>
	[Serializable,
	Syncfusion.Documentation.DocumentationExclude()
	]
	public enum UpdateType
	{
		InsertBefore,
		InsertAfter,
		Remove,
		CustomParentItem,
		ModifiedPaintStyle,
		ModifiedGrouping,
		ModifiedText,
		RecentlyUsedItemClicked,
		CustomBarAdded,
		UsePartialMenus,
		LargeIcons,
		CustomBarInfoAdded,
        ChangedImage
	}
	[Serializable,
	Syncfusion.Documentation.DocumentationExclude()
	]
	public struct BarUpdateInfo
	{
		public BarItemID source;
		public BarItemID adjacentItem;
		public object destination; // BarID or BarItemID
		public UpdateType updateType;
		public object updateData;
		internal BarUpdateInfo( BarItemID source, BarItemID adjacentItem, object destination, UpdateType updateType,
			object updateData )
		{
			this.source = source;
			this.adjacentItem = adjacentItem;
			this.destination = destination;
			this.updateType = updateType;
			this.updateData = updateData;
		}
	}

	internal class FormNativeWindow: NativeWindow
	{
		/// <summary>
		/// Raised when ActiveWindowChanged
		/// </summary>
		public event EventHandler ActiveWindowChanged;

		private Form m_form = null;
		BarManager m_manager = null;

		public FormNativeWindow( Form form )
		{
			if( form == null )
				throw new ArgumentNullException( "form" );

			m_form = form;
			m_manager = BarManager.GetManagerFromForm( m_form );

			this.AssignHandle( form.Handle );
		}

		protected override void WndProc( ref Message m )
		{
			switch( m.Msg )
			{
				case NativeMethods.WM_ACTIVATEAPP:
				bool bDeactivated = ( m.WParam == IntPtr.Zero );

				if( ActiveWindowChanged != null && bDeactivated )
				{
					ActiveWindowChanged( this, EventArgs.Empty );
				}
				break;

				case NativeMethods.WM_NCACTIVATE:
				case NativeMethods.WM_ACTIVATE:
				Form form = Control.FromHandle( m.LParam ) as Form;

				if( m_manager != null && !m_manager.GetCommandBarManager().GetCommandBarController().ShouldHostFormGetFocus() &&
						form != null && !( form is IDontCallSetFocus ) )
				{
					return;
				}

				if( ActiveWindowChanged != null )
				{
					ActiveWindowChanged( this, EventArgs.Empty );
				}
				break;
			}

			base.WndProc( ref m );
		}
	}

	/// <summary>
	/// Handles the <b>BarControlBindingChanged</b> event of a <see cref="BarManager"/>.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="args">A <see cref="BarControlBindingChangedArgs"/> object that contains the event data.</param>
	/// <seealso cref="BarManager.BarControlBindingChanged"/>
	/// <seealso cref="BarControlBindingChangedArgs"/>
	public delegate void BarControlBindingChangedEventHanlder( object sender, BarControlBindingChangedArgs args );

	/// <summary>
	/// The MainFrameBarManager manages the application's main window's menus, tool bars 
	/// and the user customization capabilities in the XP Menus framework.
	/// </summary>
	/// <remarks>
	/// <para>Make sure to take a look at the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager"/>'s documentation before you start using 
	/// this derived class. </para>
	/// <para>You should associate an instance of this class with a form that will be the main form
	/// in an MDI scenario or the single top-level form in an SDI scenario.</para>
	/// <para>Note that in an MDI scenario, optionally you can provide the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.MainFrameBarManager"/> prior knowledge 
	/// of the all the child form types that it might parent, in order that the menus and tool bars
	/// provide a seamless interface to the user even though they are part of different child forms' BarManager.
	/// You do this using the MainFrameBarManager's <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.MainFrameBarManager.RegisterMdiChildTypes(Type[])"/> method.</para>
	/// <para>If you do not call the RegisterMdiChildTypes method, the child menus will be added/merged with the parent
	/// menus as and when the child forms are added to the mdi parent.</para>
	/// <para>When using this class in a SDI Form, it is recommended that you put all your controls in the form
	/// within a <see cref="System.Windows.Forms.Panel"/> with the <b>DockStyle.Fill</b> property set, 
	/// so that they resize as the menu's height changes. This is necessary because the menus occupy the client area of the form.
	/// This is not necessary if you are using this in an MDI Container (IsMdiContainer property set to true).</para>
	/// </remarks>
	/// <example>
	/// Take a look at our XPMenus samples under the Tools\Samples\Menus Package folder
	/// for usage example.
	/// </example>
	[
	TypeConverter(
		typeof( Syncfusion.Windows.Forms.Tools.Design.BarManagerConverter )
		),
	ToolboxItem( true ),
	ToolboxBitmap( typeof( CommandBar ), "ToolboxIcons.MainFrameBarManager.bmp" ),
	ToolboxItemFilter( "System.Windows.Forms" ),
	Description( "Manages an application's main window's menus, tool bars and user customization capabilities in XP Menus framework." )
	]
	public class MainFrameBarManager: BarManager, IMessageFilter
	//,IMouseHookHLProcClient,
	//IKeyboardProcHookClient
	{
		// Custom Categories:
		// 1001: Merged Items
		// 1000: New Menu Items.
		#region PRIVATE_MEMBERS
		private bool m_bActivateFormFromBar = false;
		private bool m_bRestoreFocus = false;
		private CustomizationPanel custPanel;
		private ArrayList updateInfoList;
		private const string CustomizedBarSettingsLabel = "CustomizedBarSettings";
		private const string BarItemsVisibilityInBarLabel = "BarItemsVisibilityInBar";
		private const string CustomBarsListLabel = "CustomBarsList";
		private const string CustomItemsListLabel = "CustomItemsList";
		private const string CustomItemsContainerListLabel = "CustomItemsContainerList";
		private Hashtable formsByTypeName;
		private Hashtable activatedForms;
		private Hashtable barItemVisibilityInBars;
		private Hashtable htBarNameVsMergedBars = new Hashtable();
		protected bool largeIcs = false;
		private Bar mainMenuBar = null;
		//private Bar replacedMainMenuBar = null;
		private bool dndCustomizing = false;
		private MainFormWndSubclass mainWndSubclass = null;
		private bool autoLoadToolBarPositions = true;
		private bool cachedCustomizingState = false;
		private bool themesEnabled = false;
		private bool autoPersistCustomization = true;
		/// <summary>
		/// Internal field, not meant to be used directly.
		/// </summary>
		[Documentation.DocumentationExclude()]
		public bool NewStyleChildTypeRegsitering = true;
		private ArrayList mergedBars = new ArrayList();
		private bool allowUserRenaming = true;
		//private ArrayList replacedBars = new ArrayList();
		//private Control controlLocked = null;

		private bool m_lockHostedFormForMDIChanges = false;
		protected internal bool bUpdateUserChangeIn = false;
		private bool m_bForceSaveLoadCustomData = false;
		private bool m_bNeedSaveCustomData = true;
		private bool m_bAutoLoadCustomData = false;
		private bool m_bAutoSaveCustomData = false;

		internal ArrayList customAddedBarsVsNames = new ArrayList();
		internal ArrayList customAddedItemsVsBarItemID = new ArrayList();
		private Hashtable customContainers = new Hashtable();
		private bool m_bCustomContainersLoaded = false;
		static bool s_isDevEnv = ( Application.ExecutablePath.ToLower().IndexOf( "devenv.exe" ) >= 0 );
        /// <summary>
        /// Default size of the control
        /// </summary>
        private static Size CTRLSIZE = default(Size);

        /// <summary>
        /// Default font style of the control
        /// </summary>
        private static Font FONTSTYLE = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

        /// <summary>
        /// Font which stored after changed in design
        /// </summary>
        private static Font USERFONTSTYLE = default(Font);

		/// <summary>
		/// Font for BarItems.
		/// </summary>
		private Font m_font = null;

		/// <summary>
		/// Gets or sets font for BarItems.
		/// </summary>
		[Category( "Appearance" ), Description( "The font of the BarItems." )]
		public Font Font
		{
			get
			{
				if( m_font == null )
				{
					m_font = this.Form.Font;
				}
				return m_font;
			}
			set
			{
				if( value != m_font )
				{
					m_font = value;

					OnFontChanged( new ProvideFontInfoEventArgs( m_font ) );
				}
			}
		}

		private Form latestActiveMdiChild = null;
		internal Form LatestActiveMdiChild
		{
			get { return this.latestActiveMdiChild; }
			set
			{
				if( this.latestActiveMdiChild != value )
				{
					this.commandBarManager.SuspendLayout();
					//this.commandBarManager.LockBars();

					if( this.latestActiveMdiChild != null )
						// Updating bars only if there is no new active mdi child (or the new mdi child doesn't have a BarManager)... 
						// if there is a new one, then UpdateBars will happen from 
						// within OnFormActivated.
						this.OnFormDeactivated( this.latestActiveMdiChild, value == null || !this.IsRegisteredType( value.GetType() ) );

					this.latestActiveMdiChild = value;

					if( this.latestActiveMdiChild != null )
					{
						this.OnFormActivated( this.latestActiveMdiChild );
					}

					//this.commandBarManager.UnLockBars();
					this.commandBarManager.ResumeLayout();
				}
			}
		}

		/// <summary>
		/// Gets or sets LockHostedFormForMDIChanges
		/// </summary>
		[Browsable( false ), DefaultValue( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public bool LockHostedFormForMDIChanges
		{
			get
			{
				return this.m_lockHostedFormForMDIChanges;
			}
			set
			{
				this.m_lockHostedFormForMDIChanges = value;
			}
		}

		/// <summary>
		/// Gets or sets the ActiveChildBarManager
		/// </summary>
		[
		Syncfusion.Documentation.DocumentationExclude(),
		Browsable( false )
		]
		public BarManager ActiveChildBarManager
		{
			get
			{
				if( this.LatestActiveMdiChild != null )
				{
					return this.GetManagerFrom( this.LatestActiveMdiChild );
				}
				return null;
			}
		}
		private Form currentActiveForm = null;
		internal event EventHandler ActiveFormChanged;
		// Returns the current active Form that has a BarManager associated with it.
		// Will return the MDIContainer if the active MDI child does not have a BarManager.
		internal Form ActiveForm
		{
			get { return this.currentActiveForm; }
			set
			{
				if( this.currentActiveForm != value )
				{
					this.currentActiveForm = value;
					if( this.ActiveFormChanged != null )
						this.ActiveFormChanged( this, EventArgs.Empty );
				}
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void InitCustomizationDialog()
		{
			if( this.customizationDlg == null )
			{
				this.customizationDlg = new BarCustomizationDialog( this );
				if( this.custPanel != null )
					this.customizationDlg.SetCustomizationPanel( this.custPanel );
			}
		}

		/// <summary>
		/// Name of the MDI Children form which needs save state.
		/// </summary>
		private string m_mdiChildrenFormName = String.Empty;

#if SyncfusionFramework2_0
		/// <summary>
		/// Automatic scaling for control.
		/// </summary>
		private bool m_bAutoScale = false;
#endif

		#endregion PRIVATE_MEMBERS

		#region BARITEM_VISIBILITY_PREF
		[Syncfusion.Documentation.DocumentationExclude()]
		public bool IsBarItemVisibilityPrefAvailable( BarItem item, Bar bar )
		{
			Form form = bar.Manager.Form;
			if( form == null )
				return false;

			string formTypeName = BarManager.GetFormTypeName( bar.Manager );
			BarID barId = new BarID( bar.BarName, formTypeName );
			string barIDAsString = barId.ToString();

			BarItemID barItemId = new BarItemID( item.ID, formTypeName );
			string barItemIDAsString = barItemId.ToString();

			Hashtable barVisibility = this.barItemVisibilityInBars[barIDAsString] as Hashtable;
			return ( barVisibility != null && barVisibility[barItemIDAsString] != null );
		}

		/// <summary>
		/// specifies the bar's child control's visibility.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="bar"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public bool ShouldDrawVisibleInBar( BarItem item, Bar bar )
		{
			BarID barId = new BarID( bar.BarName, BarManager.GetFormTypeName( bar.Manager ) );
			string barIDAsString = barId.ToString();

			BarItemID barItemId = new BarItemID( item.ID, BarManager.GetFormTypeName( bar.Manager ) );
			string barItemIDAsString = barItemId.ToString();

			Hashtable barVisibility = this.barItemVisibilityInBars[barIDAsString] as Hashtable;
			if( barVisibility != null && barVisibility[barItemIDAsString] != null )
				return (bool)barVisibility[barItemIDAsString];

			return item.Visible;
		}
		/// <summary>
		/// This method is called when the user hides a <see cref="BarItem"/> using the "Add or Remove Buttons"
		/// context menu of the toolbar.
		/// </summary>
		/// <param name="barItem">The <see cref="BarItem"/> that has been made visible or hidden in the specified bar.</param>
		/// <param name="bar">The corresponding <see cref="Bar"/>.</param>
		/// <param name="show">Indicates whether to show or hide the BarItem in the bar.</param>
		public virtual void SetUserVisibilityPreferenceInBar( BarItem barItem, Bar bar, bool show )
		{
			BarID barId = new BarID( bar.BarName, BarManager.GetFormTypeName( bar.Manager ) );
			string barIDAsString = barId.ToString();

			BarItemID barItemId = new BarItemID( barItem.ID, BarManager.GetFormTypeName( bar.Manager ) );
			string barItemIDAsString = barItemId.ToString();

			Hashtable barVisibility = this.barItemVisibilityInBars[barIDAsString] as Hashtable;

			if( show )
			{
				if( barVisibility != null && barVisibility[barItemIDAsString] != null )
				{
					if( !(bool)barVisibility[barItemIDAsString] )
						barVisibility.Remove( barItemIDAsString );
				}
				else if( !barItem.Visible )
				{
					if( barVisibility == null )
						this.barItemVisibilityInBars[barIDAsString]
							= barVisibility = new Hashtable();

					barVisibility[barItemIDAsString] = true;
				}
			}
			else
			{
				if( barVisibility != null && barVisibility[barItemIDAsString] != null )
				{
					if( (bool)barVisibility[barItemIDAsString] )
						barVisibility.Remove( barItemIDAsString );
				}
				else if( barItem.Visible )
				{
					if( barVisibility == null )
						this.barItemVisibilityInBars[barIDAsString]
							= barVisibility = new Hashtable();

					barVisibility[barItemIDAsString] = false;
				}
			}
		}
		#endregion BARITEM_VISIBILITY_PREF

		#region INITIALIZATION
		/// <summary>
		/// Overloaded. Creates a new instance of the MainFrameBarManager class.
		/// </summary>
		public MainFrameBarManager()
			: base()
		{

		}
		/// <summary>
		/// Creates a new instance of the MainFrameBarManager class and sets its Form property.
		/// </summary>
		/// <param name="form">The form to associate this manager with.</param>
		public MainFrameBarManager( Form form )
			: base( form )
		{

		}
		/// <summary>
		/// Creates a new instance of the MainFrameBarManager class and sets its Form property.
		/// </summary>
		/// <param name="container">The logical container.</param>
		/// <param name="form">The form to associate this manager with.</param>
		public MainFrameBarManager( IContainer container, Form form )
			: this( form )
		{
			if( container != null )
				container.Add( this );
            FONTSTYLE = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            USERFONTSTYLE = FONTSTYLE;
        }
		protected override void Init()
		{
			base.Init();
			this.updateInfoList = new ArrayList();
			this.formsByTypeName = new Hashtable();
			this.activatedForms = new Hashtable();

			this.barItemVisibilityInBars = new Hashtable();

			this.mainWndSubclass = new MainFormWndSubclass();
		}

		private FormNativeWindow m_mainFormNativeWnd = null;

		/// <summary>
		/// Gets or sets the Form. (overridden property)
		/// </summary>
		public override Form Form
		{
			get
			{
				return base.Form;
			}
			set
			{
				if( value != base.Form )
				{
					DeInitializeNativeWindow();
					base.Form = value;
					InitializeNativeWindow();
				}
			}
		}

		private void DeInitializeNativeWindow()
		{
			if( m_mainFormNativeWnd != null )
			{
				m_mainFormNativeWnd.ActiveWindowChanged -= new EventHandler( this.OnAppDeactivated );
				m_mainFormNativeWnd.ReleaseHandle();
				m_mainFormNativeWnd = null;
			}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			if( this.Form != null )
			{
				this.Form.Load -= new EventHandler( this.Form_Load );
			}
#endif
		}

		private void SubscribeFormNativeWnd()
		{
			m_mainFormNativeWnd = new FormNativeWindow( this.Form );
			m_mainFormNativeWnd.ActiveWindowChanged += new EventHandler( this.OnAppDeactivated );
		}

		private void InitializeNativeWindow()
		{
			if( this.Form != null )
			{
#if ( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				SubscribeFormNativeWnd();
#else
				if( this.Form.Visible )
				{
					SubscribeFormNativeWnd();
				}
				else
				{
					this.Form.Load += new EventHandler( this.Form_Load );
				}
#endif
			}
		}

		private void Form_Load( object sender, EventArgs e )
		{
			SubscribeFormNativeWnd();

			this.Form.Load -= new EventHandler( this.Form_Load );
		}

		/// <summary>
		/// Gets or sets the UserControl that will be used in the Customization dialog to allow the end user
		/// to customize the menu structure.
		/// </summary>
		/// <remarks>
		/// <para>Specifies the UserControl that will be used in the Customization dialog to allow the end user
		/// to customize the application's menu structure.</para>
		/// <para>This property allows you to provide a custom look for the customization dialog in
		/// your application. To do so, derive a class from the default CustomizationPanel class
		/// (the Visual Studio make this a snap using the "Add/Add Inherited Control..." menu item 
		/// in the Solution Explorer view, even providing your derived class with a design time 
		/// to work with), instantiate that class and assign that instance to this property, all 
		/// this from within your form's constructor.</para>
		/// </remarks>
		[
		Browsable( false ),
		DefaultValue( null )]
		public CustomizationPanel CustomizationPanel
		{
			get
			{
				return this.custPanel;
			}
			set
			{
				this.custPanel = value;
				if( this.customizationDlg != null )
					this.customizationDlg.SetCustomizationPanel( this.custPanel );
			}
		}
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				UnadviseForm();

				DeInitializeNativeWindow();

				if( this.mainWndSubclass != null )
				{
					mainWndSubclass.Active = false;
					mainWndSubclass = null;
				}

                if (this.customizationDlg != null)
                {
                    this.customizationDlg.Dispose();
                    this.customizationDlg = null;
                }

				foreach( ArrayList forms in this.formsByTypeName.Values )
				{
					foreach( Form form in forms )
						if( form.Text == "Dummy Form" && !form.IsDisposed )
						{
							// This snipped of code is used because of some ActiveX controls, used in child  forms,
							// are buggy and are not disposed correctly, if Window with it didn't shown,
							// for example: Esteem.Draw.eDraw control.
							try
							{
								foreach( Control control in form.Controls )
								{
									if( control.IsHandleCreated )
									{
										control.Visible = false;
									}
								}

								form.Dispose();
							}
							catch( Exception ex )
							{
								System.Diagnostics.Debug.WriteLine( ex.Message +
									Environment.NewLine + ex.StackTrace );
							}
						}
				}

				if( null != this.commandBarManager )
				{
					this.commandBarManager.Dispose();
					this.commandBarManager = null;
				}
			}
			base.Dispose( disposing );
		}
		#endregion INITIALIZATION

		#region PROPERTIES
		private int m_popupCloseTimer = 0;

		/// <summary>
		/// Gets or sets delay in milliseconds before the displayed DropDown on ToolBar gets closed.
		/// </summary>
		[DefaultValue( 0 )]
		[Description( "Indicates delay ( in milliseconds ) before the displayed DropDown on ToolBar gets closed." )]
		public int PopupCloseTimer
		{
			get
			{
				return m_popupCloseTimer;
			}
			set
			{
				if( value != m_popupCloseTimer )
				{
					m_popupCloseTimer = value;
				}
			}
		}

		internal bool RestoreFocus
		{
			get
			{
				return m_bRestoreFocus;
			}
			set
			{
				if( m_bRestoreFocus != value )
				{
					value = m_bRestoreFocus;
				}
			}
		}
		//		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		//		public override MemoryStream BarItemsCopy
		//		{
		//			get{return null;}
		//			set{}
		//		}
		/// <summary>
		/// Indicates whether the user-customization info is available in a persisted state due to a previous run of this application.
		/// </summary>
		/// <value>True if it available; False otherwise.</value>
		/// <remarks>
		/// If the application is run for the very first time then user-customization info will not be available
		/// and this method will return false. For subsequent runs, this method will return true if 
		/// the user-customization info persistence is turned on through the <see cref="AutoPersistCustomization"/> property,
		/// false otherwise.
		/// </remarks>
		[Browsable( false ), EditorBrowsable( EditorBrowsableState.Always ),
		Description( "Specifies whether or not user-customization info is available in a persisted state due to a previous run of this application." )]
		public bool IsUserCustomizationInfoPersisted
		{
			get
			{
				AppStateSerializer serializer = AppStateSerializer.GetSingleton();
				return this.IsUserCustomizationInfoAvailable( serializer );
			}
		}

		/// <summary>
		/// Called by the <see cref="IsUserCustomizationInfoPersisted"/> to determine if user-customization info is available 
		/// in the specified serializer.
		/// </summary>
		/// <param name="serializer">An <see cref="AppStateSerializer"/> instance.</param>
		/// <returns>True if available; false otherwise.</returns>
		protected virtual bool IsUserCustomizationInfoAvailable( AppStateSerializer serializer )
		{
			object tempList = null;
			// Retrieve alignment
			tempList = serializer.DeserializeObject( this.PersistenceID + ":" + CustomizedBarSettingsLabel );
			if( tempList != null )
				return true;

			// BarItems in Bars visibility
			tempList = serializer.DeserializeObject( this.PersistenceID + ":" + BarItemsVisibilityInBarLabel );
			if( tempList != null )
				return true;

			tempList = serializer.DeserializeObject( this.PersistenceID + ":" + CustomBarsListLabel );
			if( tempList != null )
				return true;

			tempList = serializer.DeserializeObject( this.PersistenceID + ":" + CustomItemsListLabel );
			if( tempList != null )
				return true;

			return false;
		}

		/// <summary>
		/// Indicates whether to use themes ("visual styles") to draw certain portions of the menus and toolbars.
		/// </summary>
		/// <value>True to turn on themes; false otherwise.</value>
		/// <remarks>
		/// <para>
		/// Setting this to true will include the BarStyle.RotateWhenVertical in all
		/// the bars currently in the manager.
		/// </para>
		/// </remarks>
		[DefaultValue( false ),
		Category( "Appearance" ),
		Description( "Specifies whether or not to use themes to draw certain portions of the menus and toolbars." )]
		public bool ThemesEnabled
		{
			get { return this.themesEnabled; }
			set
			{
				if( this.themesEnabled != value )
				{
					bool oldValue = this.themesEnabled;
					this.themesEnabled = value;

					// Ensure that the Bars have the rotate on vertical set.
					foreach( Bar bar in this.Bars )
					{
						if( ( bar.BarStyle & BarStyle.RotateWhenVertical ) <= 0 )
							bar.BarStyle |= BarStyle.RotateWhenVertical;
					}

					this.OnPropertyChanged( new SyncfusionPropertyChangedEventArgs
						( PropertyChangeEffect.NeedLayout, "ThemesEnabled", oldValue, this.themesEnabled ) );
				}
			}
		}

		/// <summary>
		/// Gets or sets the selected bar item. (overridden property)
		/// </summary>
		public override BarItem SelectedItem
		{
			get
			{
				if( base.SelectedItem != null )
					return base.SelectedItem;
				else
				{
					// else refer to the active manager.
					BarManager activeManager = this.ActiveChildBarManager;
					if( activeManager != null )
						return activeManager.SelectedItem;
					else
						return null;
				}
			}
			set { base.SelectedItem = value; }
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager.Customizing"/>.
		/// </summary>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		]
		public override bool Customizing
		{
			get
			{
				if( this.dndCustomizing || this.cachedCustomizingState )
					return true;
				else
					return base.Customizing;
			}
		}

		/// <summary>
		/// Gets the DndCustomizing behaviour
		/// </summary>
		[Browsable( false ),
		Syncfusion.Documentation.DocumentationExclude(),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		]
		public override bool DndCustomizing
		{
			get { return this.dndCustomizing; }
		}

		private bool resetCustomization = false;
		/// <summary>
		/// Indicates whether, on application shutdown all persisted information should be reset.
		/// </summary>
		/// <remarks>
		/// Note that setting this property will reset the persisted information only when application is shutdown
		/// and so the reset state will be seen only when the application is restarted (not as soon as this property
		/// is set).
		/// </remarks>
		[Browsable( false ), EditorBrowsable( EditorBrowsableState.Always ),
		Description( "Specifies that on application shutdown all persisted information should be reset." )]
		public virtual bool ResetCustomization
		{
			get { return this.resetCustomization; }
			set
			{
				if( this.resetCustomization != value )
				{
					this.resetCustomization = value;
				}
			}
		}

		/// <summary>
		/// Initializes the drag and drop customization.
		/// </summary>
		/// <param name="popupChild"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void StartDragAndDropCustomizing( IPopupChild popupChild )
		{
			if( !( this.EnableCustomizing | this.DesignMode ) )
				return;

			// If started via Alt + Click, then ignore next alt key up
			if( ( Control.ModifierKeys & Keys.Alt ) > 0 )
			{
				this.ignoreNextAltKeyUp = true;
			}

			this.dndCustomizing = true;
			PopupManager.SetCurrentPopupClient( popupChild, true, false/*!this.DesignMode*/);
			this.OnBeginCustomization( EventArgs.Empty );
		}

		/// <summary>
		/// Serves to end the drag and drop customization.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void EndDragAndDropCustomizing()
		{
			if( this.dndCustomizing )
			{
				this.dndCustomizing = false;
				this.OnCustomizationDone( EventArgs.Empty );
			}
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager.LargeIcons"/>.
		/// </summary>
		[
		Browsable( true ),
		DefaultValue( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )
		]
		public override bool LargeIcons
		{
			get { return this.largeIcs; }
			set
			{
				if( this.largeIcs != value )
				{
					bool oldValue = this.largeIcs;

					if( !this.DesignMode )
						this.RecordCommon( null, null, null, UpdateType.LargeIcons, value );
					else
						this.largeIcs = value;
					this.OnPropertyChanged( new SyncfusionPropertyChangedEventArgs
						( PropertyChangeEffect.NeedLayout, "LargeIcons", oldValue, this.largeIcs ) );
				}
			}
		}
		/// <summary>
		/// Returns a list of CommandBar instances that will be merged with the XP Menus framework.
		/// </summary>
		/// <remarks>
		/// Use this property to add generic command bars (containing custom controls) to the 
		/// XP Menus framework. During design-time you can do the same by invoking the "Add Detached Bar"
		/// verb in the MainFrameBarManager designer.
		/// </remarks>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
		Browsable( false )
		]
		public ArrayList DetachedCommandBars
		{
			get
			{
				if( this.commandBarManager == null )
					this.CreateCommandBarManager();

				if( this.commandBarManager != null )
					return this.commandBarManager.DetachedCommandBars;
				else
					return null;
			}
		}

		/// <summary>
		/// Indicates whether to automatically load the 
		/// persisted toolbar positions when the application is restarted.
		/// </summary>
		/// <value>True to automatically load the toolbar positions; false if not.</value>
		[
		DefaultValue( true ),
		Category( "Persistance" ),
		Description( "Specifies whether or not to automatically load the persisted toolbar positions when the application is restarted." )
		]
		public virtual bool AutoLoadToolBarPositions
		{
			get { return this.autoLoadToolBarPositions; }
			set
			{
				if( this.autoLoadToolBarPositions != value )
				{
					this.autoLoadToolBarPositions = value;
					if( this.commandBarManager != null )
						this.commandBarManager.LoadCommandBarState_Changed();
				}
			}
		}

		/// <summary>
		/// Indicates whether user should be allowed to change the names of the menu items during runtime.
		/// </summary>
		/// <value>True to allow renaming; false otherwise.</value>
		[
		Category( "Customization" ),
		Description( "Specifies whether user should be allowed to change the names of the menu items during runtime." ),
		DefaultValue( true )
		]
		public virtual bool AllowUserRenaming
		{
			get { return this.allowUserRenaming; }
			set
			{
				if( this.allowUserRenaming != value )
				{
					this.allowUserRenaming = value;
				}
			}
		}

		/// <summary>
		/// Indicates whether to persist and load user-customized info from or to 
		/// isolated storage.
		/// </summary>
		/// <value>True to persist; false otherwise.</value>
		/// <remarks>
		/// <para>
		/// If this property is set to false, persisted user-customized information will neither be loaded
		/// nor will new information be persisted in isolated storage. Existing information
		/// in the isolated storage will also be destroyed.
		/// </para>
		/// <para>To control persistence of toolbar positions, use the <see cref="AutoLoadToolBarPositions"/> property.</para>
		/// <para>Users can still customize their menus and toolbars with this flag off, but their
		/// changes will not be persisted when the application is closed. Use <see cref="BarManager.EnableCustomizing"/> to prevent users from customizing.</para>
		/// <para>
		/// To prevent loading of user-customized information, this property should be either set (to false) using the designer or
		/// set in your form's constructor before calling the <see cref="RegisterMdiChildTypes(Type[])"/> method.
		/// </para>
		/// </remarks>
		[
		Category( "Persistance" ),
		DefaultValue( true ),
		Description( "Indicates whether to persist and load user-customized info from or to isolated storage." )
		]
		public virtual bool AutoPersistCustomization
		{
			get { return this.autoPersistCustomization; }
			set
			{
				if( this.autoPersistCustomization != value )
				{
					this.autoPersistCustomization = value;
				}
			}
		}

		//		private void UpdateCachedCustomizationSettings(AppStateSerializer serializer, bool persistCustomization)
		//		{
		//			if(!persistCustomization)
		//			{
		//				if(!this.DesignMode
		//					&& (Application.ExecutablePath.IndexOf("devenv.exe") < 0))
		//				{
		//					serializer.SerializeObject(CustomizedBarSettingsLabel, null);
		//					serializer.SerializeObject(BarItemsVisibilityInBarLabel, null);
		//				}
		//			}
		//		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager.UsePartialMenus"/>.
		/// </summary>
		[DefaultValue( true ),
		Category( "Behavior" )]
		public override bool UsePartialMenus
		{
			get { return this.partialMenusMode; }
			set
			{
				if( this.partialMenusMode != value )
				{
					if( !this.DesignMode )
						this.RecordCommon( null, null, null, UpdateType.UsePartialMenus, value );
					else
						this.partialMenusMode = value;
				}
			}
		}

		/// <summary>
		/// Returns the list of ChildFrameBarManagers associated with this MainFrameBarManager.
		/// </summary>
		/// <remarks>
		/// Specifies the list of ChildFrameBarManagers associated with this 
		/// MainFrameBarManager. 
		/// <para>
		/// This list will be an instance of each form type specified in the RegisterMDIChildTypes call.</para>
		/// </remarks>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public ArrayList ChildManagers
		{
			get
			{
				// Prepare a list on the fly
				ArrayList managers = new ArrayList();
				foreach( ArrayList forms in this.formsByTypeName.Values )
					if( forms[0] != this.Form )
					{
						BarManager childManager = GetManagerFrom( forms[0] as Form );
						if( childManager != null )
							managers.Add( childManager );
					}
				return managers;
			}
		}

		/// <summary>
		/// Returns the main-menu toolbar in the form.
		/// </summary>
		/// <value>A <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.Bar"/> object. Can be null.</value>
		/// <remarks>
		/// <para>During design-time a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.Bar" /> is designated as the main-menu by including
		/// the IsMainMenu enum in the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.Bar.BarStyle"/> property.
		/// </para>
		/// <para>However, during runtime, a new <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.Bar"/> instance
		/// is created by merging the main-menus of the mdi container and all the mdi children. This merged
		/// main-menu <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.Bar"/> is exposed through this property. 
		/// You can modify insert items into the main-menu during runtime in code, if necessary, through this bar.
		/// </para>
		/// <para>Merging takes place when you call <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.MainFrameBarManager.RegisterMdiChildTypes(Type[])"/>
		/// or when the form gets activated. If you call this property before merging takes place then the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.Bar"/>
		/// in the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager.Bars"/> collection with the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.Bar.BarStyle"/>
		/// property containing the IsMainMenu enum will be returned. Will return null, if no such bar exists.
		/// </para>
		/// </remarks>
		[Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),]
		public Bar MainMenuBar
		{
			get
			{
				if( this.mainMenuBar != null )
					return this.mainMenuBar;
				else
				{
					foreach( Bar bar in this.Bars )
					{
						if( ( bar.BarStyle & BarStyle.IsMainMenu ) > 0 )
							return bar;
					}
				}
				return null;
			}
			set
			{
				Bar prevMainMenu = this.MainMenuBar;

				if( value != this.MainMenuBar )
				{
					CommandBarManager manager = this.GetCommandBarManager();
					if( manager != null )
					{
						manager.RemoveBars( this, true );
					}

					if( prevMainMenu != null )
					{
						prevMainMenu.BarStyle = prevMainMenu.BarStyle & ~BarStyle.IsMainMenu;
					}

					mainMenuBar = value;
					mainMenuBar.AllowHiding = false;
					mainMenuBar.BarStyle = mainMenuBar.BarStyle | BarStyle.IsMainMenu;

					if( manager != null )
					{
						manager.AttachBars( this );
					}
				}
			}
		}

		/// <summary>
		/// Indicates if activating menu should activate parent form.
		/// </summary>
		[DefaultValue( false ),
		Description( "Indicates if activating menu should activate parent form." )]
		public bool ActivateFormFromBar
		{
			get
			{
				return m_bActivateFormFromBar;
			}
			set
			{
				if( m_bActivateFormFromBar != value )
				{
					m_bActivateFormFromBar = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets name of the MDI Children form which needs save state.
		/// </summary>
		internal string MdiChildrenFormName
		{
			get
			{
				return m_mdiChildrenFormName;
			}
			set
			{
				if( value != m_mdiChildrenFormName )
				{
					m_mdiChildrenFormName = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether brought the CommandBar to the front of the z-order.
		/// </summary>
		[
		DefaultValue( false ),
		Description( "Indicates whether bring the CommandBar to the front of the z-order." ),
		Category( "Behavior" )
		]
		public bool InternalDocking
		{
			get
			{
				bool bIsInternal = false;
				CommandBarController controller = null;

				if( commandBarManager != null )
				{
					controller = commandBarManager.GetCommandBarController();

					if( controller != null )
					{
						bIsInternal = controller.InternalDocking;
					}
				}

				return bIsInternal;
			}
			set
			{
				CommandBarController controller = null;

				if( commandBarManager != null )
				{
					controller = commandBarManager.GetCommandBarController();

					if( controller != null )
					{
						controller.InternalDocking = value;
					}
				}
			}
		}

#if SyncfusionFramework2_0
		/// <summary>
		/// Gets or sets the automatic scaling for control. 
		/// </summary>
		[
		Category( "Behavior" ),
		DefaultValue( false ),
		Description( "Automatic scaling for CommandBars." )
		]
		public bool AutoScale
		{
			get
			{
				return m_bAutoScale;
			}
			set
			{
				if( value != m_bAutoScale )
				{
					m_bAutoScale = value;
				}
			}
		}
#endif

		#endregion PROPERTIES

		#region EVENTS

		/// <summary>
		/// Lets you specify a unique ID used to distinguish the persistence information of different instances of your Form type.
		/// </summary>
		/// <remarks>
		/// The default persistence logic assumes that there will be only a single MainFrameBarManager
		/// in an application. But that might not be the case if you have more than 1 MDI parent.
		/// In such cases, the persisted state of one MDI parent will get overridden by the other
		/// since the default logic doesn't distinguish these 2 different instances.
		/// </remarks>
		[Description( "Lets you specify a unique ID used to distinguish the persistence information of different instances of your Form type." )]
		public event ProvidePersistenceIDEventHandler ProvidePersisteceID;

		/// <summary>
		/// Raises the ProvidePersisteceID event.
		/// </summary>
		/// <param name="e">
		/// An ProvidePersistenceIDEventArgs object containing data pertaining to this event.
		/// </param>
		/// <remarks>
		/// The OnProvidePresistenceID method also allows derived classes to handle the event 
		/// without attaching a delegate. This is the preferred technique for 
		/// handling the event in a derived class. 
		/// <para>Notes to Inheritors:  When overriding OnProvidePresistenceID in a derived 
		/// class, be sure to call the base class's OnProvidePresistenceID method so that 
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnProvidePresistenceID( ProvidePersistenceIDEventArgs e )
		{
			if( this.ProvidePersisteceID != null )
				this.ProvidePersisteceID( this, e );
		}
		internal string PersistenceID
		{
			get
			{
				ProvidePersistenceIDEventArgs e = new ProvidePersistenceIDEventArgs( String.Empty );
				this.OnProvidePresistenceID( e );
				return e.PersistenceID;
			}
		}

		/// <summary>
		/// Occurs while the dock bar background is being drawn.
		/// </summary>
		[
			// This method not supported in this version.
		Syncfusion.Documentation.DocumentationExclude(),
		Browsable( false )]
		public event DockBarPaintEventHandler DrawDockBarBackground;

		[
			// This method not supported in this version.
		Syncfusion.Documentation.DocumentationExclude(),
		Browsable( false )]
		protected internal virtual bool OnDrawDockBarBackground( DockBarPaintEventArgs args )
		{
			if( this.DrawDockBarBackground != null )
			{
				this.DrawDockBarBackground( this, args );
				return true;
			}
			return false;
		}

		/// <summary>
		/// Occurs when <see cref="Font"/> is changed.
		/// </summary>
		[
		Description( "Occurs when Font property is changed." ),
		Category( "Property Changed" )
		]
		public event ProvideFontInfoEventHandler FontChanged;

		protected virtual void OnFontChanged( ProvideFontInfoEventArgs args )
		{
			RaiseFontChanged( args );

			if( args.Font != m_font )
			{
				m_font = args.Font;
			}
            if (!isScaling)
            {
                if (USERFONTSTYLE != this.Font)
                    USERFONTSTYLE = this.Font;
            }
		}

		private void RaiseFontChanged( ProvideFontInfoEventArgs args )
		{
			if( FontChanged != null )
			{
				FontChanged( this, args );
			}
		}

		#endregion EVENTS

		#region ACCESSING_LOADED_FORMS
		internal bool IsDummyManager( BarManager manager )
		{
			if( manager.Form != null
				&& manager.Form.Text == "Dummy Form" )
				return true;

			return false;
		}

		private ArrayList GetLoadedFormsOfType( string formTypeName )
		{
			ArrayList forms = this.formsByTypeName[formTypeName] as ArrayList;
			if( forms == null )
			{
				forms = new ArrayList();
				this.formsByTypeName[formTypeName] = forms;
			}
			return forms;
		}
		private Form GetDummyFormOfType( Type formType )
		{
			ArrayList forms = this.formsByTypeName[formType.FullName] as ArrayList;
			if( forms != null )
			{
				foreach( Form form in forms )
				{
					if( form.Text == "Dummy Form" )
						return form;
				}
			}
			return null;
		}
		#endregion ACCESSING_LOADED_FORMS

		#region UTIL_FUNCS
		private void InitDummyForm( Form form )
		{
			ArrayList forms = GetLoadedFormsOfType( BarManager.GetFormTypeName( form ) );
			forms.Add( form );

			form.Text = "Dummy Form";
			BarManager manager = GetManagerFrom( form );
			manager.mainBarManager = this;
			form.Visible = false;
		}

		private Form GetFormFromType( Type formType )
		{
			System.Reflection.ConstructorInfo constructorInfo;
			constructorInfo = formType.GetConstructor( BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new System.Type[] { }, null );
			if( constructorInfo != null )
				return constructorInfo.Invoke( new object[] { } ) as Form;
			else
				return null;
		}

		private Form GetActiveFormOfType( string formTypeName )
		{
			Form form = null;
			ArrayList forms = this.formsByTypeName[formTypeName] as ArrayList;

			if( null != forms && forms.Count > 0 )
			{
				form = forms[0] as Form;
			}

			return form;
		}

		/// <summary>
		/// Indicates the bar item's visibility.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public bool ShouldDrawVisible( BarItem item )
		{
			if( item.Manager == null ) return false;

			if( this.Customizing )
			{
				if( !this.IsItemCustomizable( item ) )
					return false;
				else
					return true;
			}

			if( item.Manager.Form == null || item.Manager.Form.Text == "Dummy Form" )
				return false;
			else
				return item.Visible;
		}

		/// <summary>
		/// Checks whether the bar item is customizable.
		/// </summary>
		/// <param name="item">
		/// Bar item to be checked.
		/// </param>
		/// <returns>True if the bar item is customizable</returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public bool IsItemCustomizable( BarItem item )
		{
			if( item.Customizable || this.DesignMode )
			{
				if( !this.DesignMode && item.Manager != null )
				{
					int catIndex = item.CategoryIndex;
					if( item.Manager.CategoriesToIgnoreInCustDialog.IndexOf( catIndex ) != -1 )
						return false;
				}
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Saves the current toolbars state information to the specified persistence medium,
		/// for toolbars which relate to form.
		/// </summary>
		protected void SaveBarStateForForm( Form form )
		{
			if( form != null && commandBarManager != null )
			{
				BarManager manager = this.GetManagerFrom( form );
				CommandBarControllerExt controller = commandBarManager.GetCommandBarController() as CommandBarControllerExt;

				if( manager != null && controller != null )
				{
					ArrayList savingCommandBars = GetCommandBarsForForm( form );

					// saves state to CommandBars
					if( savingCommandBars != null && savingCommandBars.Count > 0 )
					{
						foreach( CommandBar cbar in savingCommandBars )
						{
							controller.AppendCommandBarInfoForPersistance( cbar );
						}
					}
				}
			}
		}


		/// <summary>
		/// Gets a value indicating whether the <see cref="CommandBarExt"/> 
		/// relate to <see cref="MainFrameBarManager"/>.
		/// </summary>
		internal bool IsMainCommandBar( CommandBarExt cbar )
		{
			bool bMainCommandBar = false;

			if( cbar != null )
			{
				ArrayList commandBars = GetCommandBarsForForm( this.Form );

				if( commandBars != null && commandBars.Count > 0 && commandBars.Contains( cbar ) )
				{
					bMainCommandBar = true;
				}
			}

			return bMainCommandBar;
		}


		/// <summary>
		/// Gets CommandBars which relate to form.
		/// </summary>
		internal ArrayList GetCommandBarsForForm( Form form )
		{
			ArrayList commandBars = null;

			if( form != null && commandBarManager != null )
			{
				BarManager manager = this.GetManagerFrom( form );
				CommandBarController controller = commandBarManager.GetCommandBarController();

				if( manager != null && controller != null )
				{
					ArrayList mergedBars = manager.GetMergedBars();

					if( mergedBars != null && mergedBars.Count > 0 )
					{
						commandBars = new ArrayList();
						CommandBar commandBar = null;

						// gets CommandBars which must be saving its state
						foreach( Bar bar in mergedBars )
						{
							commandBar = commandBarManager.GetCommandBarFromBar( bar );

							if( commandBar != null )
							{
								commandBars.Add( commandBar );
							}
						}

						if( form == this.Form && this.DetachedCommandBars != null && this.DetachedCommandBars.Count > 0 )
						{
							foreach( CommandBar detachedCBar in this.DetachedCommandBars )
							{
								commandBars.Add( detachedCBar );
							}
						}
					}
				}
			}

			return commandBars;
		}


		/// <summary>
		/// Update the CommandBars's state for toolbars which relate to form.
		/// </summary>
		private void UpdateCommandBarState( Form form )
		{
			if( form != null )
			{
				ArrayList commandBars = GetCommandBarsForForm( form );

				if( commandBars != null && commandBars.Count > 0 )
				{
					CommandBarControllerExt controller = commandBarManager.GetCommandBarController() as CommandBarControllerExt;

					if( controller != null )
					{
						foreach( CommandBar cbar in commandBars )
						{
							controller.LoadCommandBarStateFromCache( cbar );
						}
					}
				}
			}
		}


		#endregion UTIL_FUNCS

		#region METHODS

		/// <summary>
		/// Checks whether the key can be processed for shortcut
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public override bool ProcessShortcut( Keys key )
		{
			bool returnVal = false;
			if( base.ProcessShortcut( key ) )
				returnVal = true;
			// We will instead process shortcut in the dockbar ProcessMnemonic override
			//			else
			//			{
			//				if((key & Keys.Alt) > 0
			//					&& Form.ActiveForm == this.Form)
			//					returnVal = this.commandBarManager.ProcessShortcut(key);
			//				else
			//					returnVal = false;
			//			}
			if( returnVal && key == Keys.F10 )
			{
				this.ignoreNextAltKeyUp = true;
			}
			return returnVal;
		}

		/// <summary>
		/// Processes the command key.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public override bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			// No need for a ShortcutMask ever since we use a dummy ContextMenu to listen to cmd keys.
			// Will not process "commands" if a popup is open.
			if( this.Customizing  /*(keyData & ShortcutMask) == 0 ||*/
				// This check will prevent the shortcuts from getting processed even if the
				// ActivePopupClient is a TooltipControl, for example. So, removing this.
				// This doesn't seem to be necessary anyway since the dropdown menu and the toolbars
				// eath the shortcut keys when they are active.
				//|| PopupManager.ActivePopupClient != null
				)
				return false;

			if( ( keyData == ( Keys.Menu | Keys.Alt ) || keyData == Keys.F10 )
				&& this.commandBarManager != null )
			{
				this.ProcessMenuKeyDown();
			}

			// Let the active child process this first.
			if( this.Form.IsMdiContainer )
			{
				Form activeChild = this.Form.ActiveMdiChild;
				if( activeChild != null )
				{
					BarManager manager = GetManagerFrom( activeChild );

					if( manager != null && manager.ProcessShortcut( keyData ) )
						return true;
				}
			}

			return this.ProcessShortcut( keyData );
		}

		/// <summary>
		/// Forces to execute shortcuts in menus when child forms are opened.
		/// In this case standard ProcessMnemonic doesn't raise.
		/// </summary>
		/// <param name="msg"></param>
		/// <returns>TRUE if processed; FALSE otherwise.</returns>
		private bool ProcessMnemonicInternal( ref Message msg )
		{
			bool result = false;

			Keys keyData = ( (Keys)( (int)msg.WParam ) ) | Control.ModifierKeys;
			char ch = (char)( (ushort)( (int)msg.WParam ) );

			if( null != this.Form && this.Form.IsMdiContainer )
			{
				Form activeChild = this.Form.ActiveMdiChild;

				// Try process shortcuts when child form is present.
				if( activeChild != null )
				{
					// Process shortcut when key is not Alt now but Alt key was recently pressed.
					if( ( keyData != ( Keys.Menu | Keys.Alt ) ) &&
						this.commandBarManager != null && this.processedAltKeyDown )
					{
						result = this.commandBarManager.ProcessShortcut( ch );
						this.processedAltKeyDown = false;
					}
				}
			}
			else
			{
                if (this.commandBarManager != null && this.ActiveForm.ContainsFocus)
                    result = this.commandBarManager.ProcessShortcut(ch);
			}

			return result;
		}

		private const string DEF_MNEMONIC_NAME = "ProcessMnemonic";

		[Syncfusion.Documentation.DocumentationExclude()]
		internal bool ignoreNextAltKeyUp = false;
		private bool processedAltKeyDown = false;

		private bool ShouldPreventMessagesToWndWhileCustomizing( IntPtr destWnd )
		{
			// Always forward exceptions caught here to the Application class so that Application.ThreadException listeners will 
			// get to handle it.
			try
			{
				// Let these messages be processed only if they belong to a Bar/Menu.
				Control dest = Control.FromHandle( destWnd );
				if( dest == null && !SystemInformationExt.IsDotNetApp )
					return false;
				// This is necessary because some .Net controls like ListView - when in listedit mode
				// will host native controls that will not be associated with a Control type.
				else if( dest == null )
					dest = PopupUtils.GetADotNetParentControl( destWnd );

				if( null != dest )
				{
					if( dest is MenuGrid
						|| dest.FindForm() is IAmACustomizationForm
						|| ( dest is BarControlInternal && !( dest is XPToolBar ) )
						|| dest is CommandBarExt || dest.Parent is MenuGrid
						// The resoning here is that if the PCC is showing then the parent should have been allowed to
						// recieve the lbutton messages and so we will allow the messages to be passed on to the popup as well.
						|| ( dest.FindForm() is PopupHost ) )
						return false;
					else
						return true;
				}
			}
			catch( Exception e )
			{
				Application.OnThreadException( e );
			}
			return false;
		}

		internal bool bNeedStartKeyboardNavigation = true;

		private void ProcessMenuKeyDown()
		{
			if( this.processedAltKeyDown )
				return;

			if( this.commandBarManager.IsKeyboardNavigationOn() )
			{
				bNeedStartKeyboardNavigation = false;

				this.commandBarManager.HintViaHotKeyPrefix = false;
				this.commandBarManager.StopKeyboardNavigationInMainMenu();
			}

			if( !this.Customizing )
			{
				Control control = Control.FromHandle( NativeMethods.GetFocus() );
				Form activeForm = ( control != null ) ? control.FindForm() : null;

				if( ( ( Form.ActiveForm == this.Form ) || ( activeForm != null && this.Form == activeForm.Owner ) ) && !ignoreNextAltKeyUp && bNeedStartKeyboardNavigation )
				{
					this.commandBarManager.HintViaHotKeyPrefix = true;
				}

				// Turn this on so that Alt Key up will be processed.
				this.processedAltKeyDown = true;
			}
		}

		private bool ProcessMenuKeyUp()
		{
			try
			{
                if( this.ignoreNextAltKeyUp )
				{
					this.commandBarManager.StopKeyboardNavigationInMainMenu();
					this.ignoreNextAltKeyUp = false;
					return true;
				}
				else
				{
					if( !this.Customizing )
					{
						Control control = Control.FromHandle( NativeMethods.GetFocus() );
						Form activeForm = ( control != null ) ? control.FindForm() : null;

						if( ( ( Form.ActiveForm == this.Form ) || ( activeForm != null && this.Form == activeForm.Owner ) )
							// Process this only if the alt key down was passed on to the menu - without
							// being processed by a control in it's ProcessCmdKey override.
							&& this.processedAltKeyDown && bNeedStartKeyboardNavigation )
						{
							this.ignoreNextAltKeyUp = true;
							this.commandBarManager.StartKeyboardNavigationInMainMenu();

							return true;
						}
					}
					else
						return true;
				}
				return false;
			}
			finally
			{
				// So that this will be reset the next time alt key is down.
				this.processedAltKeyDown = false;
				bNeedStartKeyboardNavigation = true;
			}
		}
		/// <summary>
		/// Checks whether the message being passed is a pre-filtered message.
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual bool PreFilterMessage( ref Message m )
		{
            // While customizing don't let messages be sent freely.
			if( !this.DesignMode )
			{
				if( this.cachedCustomizingState == true )
				{
					// MouseFirst - MouseLast
					// NC Mouse Messages
					// KeyFirst - KeyLast
					// WM_MOUSEACTIVATE
					if( m.Msg >= 0x200 && m.Msg <= 0x020D
						//|| m.Msg >= 0xa0 && m.Msg <= 0xad 
						//|| m.Msg >= 0x2a0 && m.Msg <= 0x2a2 
						|| m.Msg >= 0x100 && m.Msg <= 0x0108
						|| m.Msg == 0x007B // WM_CONTEXTMENU
						//|| m.Msg == 0x21
						)
					{
						return false;
					}
				}
			}

			if( ( Control.ModifierKeys & Keys.Alt ) == 0 && this.dndCustomizing )
			{
				this.EndDragAndDropCustomizing();
				this.ignoreNextAltKeyUp = false;
			}

			if( m.Msg == 260 /* WM_SYSKEYDOWN */)
			{
				// For .Net apps we process this in the ProcessCmdKey method.
				// Here we process this only for non-dotNet apps for which ProcessCmdKey is not fired.
				if( SystemInformationExt.IsDotNetApp == false )
				{
					Keys key = (Keys)m.WParam.ToInt32();
					// Don't do this for F10
					if( ( key == Keys.Menu )
						&& this.commandBarManager != null )
					{
						this.ProcessMenuKeyDown();
					}
				}
			}
			else if( m.Msg == 261 /* WM_SYSKEYUP */)
			{
				// This bit is zero if the WM_SYSKEYDOWN message is posted to the 
				// active window because no window has the keyboard focus.
				if( ( m.LParam.ToInt64() & 0x20000000 ) > 0 )
					return false;
				Keys key = (Keys)m.WParam.ToInt32();
				if( ( key == Keys.Menu || key == Keys.F10 )
					&& this.commandBarManager != null )
				{
					if( this.ProcessMenuKeyUp() )
						return true;
				}
			}
			else if( ( m.Msg == NativeMethods.WM_CHAR ) ||
				( m.Msg == NativeMethods.WM_SYSCHAR ) )
			{
				return ProcessMnemonicInternal( ref m );
			}

			else if( m.Msg == NativeMethods.WM_KEYDOWN )
			{
				Keys keyData = ( (Keys)( (int)m.WParam ) ) | Control.ModifierKeys;
				return ProcessShortcut( keyData );
			}

			return false;
		}


		//		bool IMouseHookHLProcClient.MouseHookProc(int msg, Point point, IntPtr hwnd, int wHitTestCode, int dwExtraInfo)
		//		{
		//			if(!this.DesignMode)
		//			{
		//				if(this.cachedCustomizingState == true)
		//				{
		//					return this.ShouldPreventMessagesToWndWhileCustomizing(hwnd);
		//				}
		//			}
		//			return false;
		//		}
		//		bool IKeyboardProcHookClient.KeyboardHookProc(int wParam, int lParam)
		//		{
		//			if(!this.DesignMode)
		//			{
		//				if(this.cachedCustomizingState == true)
		//				{
		//					IntPtr focusedWnd = Syncfusion.Runtime.InteropServices.NativeMethods.GetFocus();
		//					if(focusedWnd != IntPtr.Zero)
		//						return this.ShouldPreventMessagesToWndWhileCustomizing(focusedWnd);
		//				}
		//			}
		//			// If Alt is pressed:
		//			if(((int)lParam & 0x20000000) > 0)
		//			{
		//				Keys keys = (Keys)wParam;
		//
		//				if(((int)lParam & 0x80000000) == 0)
		//				{
		//					// Key is pressed
		//					// Don't do this for F10
		//					if((keys == Keys.Menu)
		//						&& this.commandBarManager != null)
		//					{
		//						this.ProcessMenuKeyDown();
		//					}
		//				}
		//				else
		//				{
		//					// Key is released
		//					if((keys == Keys.Menu || keys == Keys.F10)
		//						&& this.commandBarManager != null)
		//					{
		//						if(this.ProcessMenuKeyUp())
		//							return true;
		//					}
		//				}
		//			}
		//			return false;
		//		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager.OnDetachForm"/>.
		/// </summary>
		protected override void OnDetachForm()
		{
			//this.mainWndSubclass.ReleaseHandleCustom();
			this.ListeningToMdiChildrenAdded( false );
			base.OnDetachForm();

			UnadviseForm();
		}

		protected new void UnadviseForm()
		{
			Form parentForm = this.Form;

			if( null != parentForm )
			{
				if( !m_bFormClosed )
					this.FormClosed( this.Form, EventArgs.Empty );

				parentForm.MdiChildActivate -= new EventHandler( this.MdiChildActivated );
				parentForm.Activated -= new EventHandler( this.FormActivated );
				parentForm.Deactivate -= new EventHandler( this.FormDeactivate );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override void CreateCommandBarManager()
		{
			if( this.Form != null )
				this.commandBarManager = new CommandBarManager( this.Form, this );
		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager.EndInit"/>.
		/// </summary>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public override void EndInit()
		{
			if( !this.DesignMode )
			{
				this.updateInfoList = new ArrayList();
				this.customAddedBarsVsNames = new ArrayList();
				this.customAddedItemsVsBarItemID = new ArrayList();
				this.customContainers = new Hashtable();
				this.barItemVisibilityInBars = new Hashtable();

				if( this.AutoPersistCustomization )
				{
					this.LoadCustomizationInfo( AppStateSerializer.GetSingleton() );
					this.LoadBarStateInternal( AppStateSerializer.GetSingleton(), false );
				}
				else if( this.AutoLoadToolBarPositions )
				{
					this.LoadBarStateInternal( AppStateSerializer.GetSingleton(), false );
				}

				this.EnsureCenterScreen();

				if( !this.endInitCalled )
				{
					// Create the MergedBar main-menu even before attaching the commandbars.
					// This will improve performance.
					// This will not help if RegisterMdiChildTypes is called by the user, though.
					this.InitMergedBars();

					// Attach the bars before Form Load so that the commandbars will be 
					// created before Form_Load and there will be less flicker.
					this.AttachCommandBarsOfForm( this.Form, true, true );

					this.ListeningToMdiChildrenAdded( true );
				}
				else
				{
					// This will get the designer state in the derived form.
					this.commandBarManager.LoadDesignerState( this, false );
				}
			}
			// Should be called finally due to the above endInitCalled usage.
			base.EndInit();
		}
		// This method ensures that if a form has the CenterScreen property set, then it
		// does get centered. This needs to be forced because, WindowsForms uses the property
		// during window handle creation, which in the presence of XPMenus happens before the
		// CenterScreen property gets set in the client Form.
		private void EnsureCenterScreen()
		{
			if( this.Form != null && this.Form.IsHandleCreated
				&& this.Form.StartPosition == FormStartPosition.CenterScreen )
			{
				// Update start postion
				Type formType = typeof( Form );
				try
				{
					// Invoke the CenterToScreen method to force centering the form.
					formType.InvokeMember( "CenterToScreen",
						BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic
						, null, this.Form, new object[] { } );
				}
				catch { }
				{
					//Trace.WriteLine(e.Message);
				}
			}
		}

		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager.OnAttachForm"/>.
		/// </summary>
		protected override void OnAttachForm()
		{
			base.OnAttachForm();

			this.currentActiveForm = this.Form;
			this.Form.MdiChildActivate += new EventHandler( this.MdiChildActivated );
			this.Form.Activated += new EventHandler( this.FormActivated );
			this.Form.Deactivate += new EventHandler( this.FormDeactivate );
			
            this.BringFormToFront( this.Form );

			if( this.DesignMode )
				this.FormLoaded( null, EventArgs.Empty );
		}        

		internal static MdiClient GetMDIClient( BarManager barMan )
		{
			MdiClient mdiClient = null;

			if( null != barMan )
			{
				Form form = barMan.Form;

				if( null != form && form.IsMdiContainer )
				{
					foreach( Control child in form.Controls )
					{
						if( child is MdiClient )
						{
							mdiClient = child as MdiClient;
							break;
						}
					}
				}
			}

			return mdiClient;
		}

		private void ListeningToMdiChildrenAdded( bool subscribe )
		{
			// listen to MdiClient's ControlAdded
			MdiClient mdiClient = MainFrameBarManager.GetMDIClient( this );

			if( null != mdiClient )
			{
				if( subscribe )
				{
					mdiClient.ControlAdded += new ControlEventHandler( MdiChildAdded );
					mdiClient.ControlRemoved += new ControlEventHandler( MdiChildRemoved );
				}
				else
				{
					mdiClient.ControlAdded -= new ControlEventHandler( MdiChildAdded );
					mdiClient.ControlRemoved -= new ControlEventHandler( MdiChildRemoved );
				}
			}
		}

		private Hashtable htUnregisteredChildTypesVsInstanceCount = new Hashtable();
		private void MdiChildAdded( object sender, ControlEventArgs e )
		{
			Form childForm = e.Control as Form;
			this.OnMdiChildAdded( childForm );
		}

		private int GetChildFormsCountOfType( Type type )
		{
			int i = 0;
			foreach( Form child in this.Form.MdiChildren )
			{
				if( child.GetType() == type )
					i++;
			}
			return i;
		}

		private Hashtable m_htChildForms = new Hashtable();

		private void LockHostedFormRedraw()
		{
			if( this.LockHostedFormForMDIChanges && ( this.Form != null ) )
			{
				NativeMethodsHelper.SuspendRedrawWindow( this.Form.Handle );
			}
		}

		private void UnLockHostedFormRedraw()
		{
			if( this.LockHostedFormForMDIChanges && ( this.Form != null ) )
			{
				NativeMethodsHelper.ResumeRedrawWindow( this.Form.Handle, false );

				int redrawFlags = NativeMethods.RDW_ERASE |
					NativeMethods.RDW_INVALIDATE |
					NativeMethods.RDW_NOCHILDREN |
					NativeMethods.RDW_FRAME;

				Syncfusion.Runtime.InteropServices.NativeMethodsHelper.RedrawWindow( this.Form.Handle, redrawFlags );
			}
		}
		private void OnMdiChildAdded( Form childForm )
		{
			bool prevSaveCustomDataValue = m_bNeedSaveCustomData;
			m_bNeedSaveCustomData = false;

			BarManager manager = this.GetManagerFrom( childForm );

			if( manager != null )
			{
				if( this.AutoPersistCustomization && m_htChildForms.Count == 0 )
				{
					SaveBarStateForForm( this.Form );
				}

				m_htChildForms[childForm] = childForm;

				Type childType = childForm.GetType();

				// Check if this is a registered type.
				if( !this.IsRegisteredType( childType ) )
				{
					this.htUnregisteredChildTypesVsInstanceCount[childType.FullName] = this.GetChildFormsCountOfType( childType );
					Form dummyForm = BarManager.CreateDummyForm( childForm );
					this.RegisterMdiChildTypes( new Type[] { childType }, new Form[] { dummyForm } );
				}
				else
				{
					// Check if this was auto registered...
					// if so increase the instance count.
					if( this.IsAutoRegistered( childType ) )
					{
						int curCount = (int)this.htUnregisteredChildTypesVsInstanceCount[childType.FullName];
						this.htUnregisteredChildTypesVsInstanceCount[childType.FullName] = ++curCount;
					}
				}

				// Not waiting until the child form gets activated, b'cos it could
				// be already active (while re-registering some unregistered types).
				if( childForm.Visible && this.Form.ActiveMdiChild == childForm )
				{
					// If this form is already active, first reset the LatestActiveMdiChild
					// and then set it again, so that the menus will be refreshed.
					if( this.LatestActiveMdiChild == childForm )
						this.LatestActiveMdiChild = null;

					this.LatestActiveMdiChild = childForm;
				}
			}

			m_bNeedSaveCustomData = prevSaveCustomDataValue;
		}

		private void MdiChildRemoved( object sender, ControlEventArgs e )
		{
			this.OnMdiChildRemoved( e.Control as Form, false );
		}

		private void OnMdiChildRemoved( Form childForm, bool fromClose )
		{
			bool prevSaveCustomDataValue = m_bNeedSaveCustomData;
			m_bNeedSaveCustomData = false;

			if( m_htChildForms[childForm] != null )
			{
				m_htChildForms.Remove( childForm );
			}

			if( this.LatestActiveMdiChild == childForm )
			{
				this.MdiChildrenFormName = String.Empty;

				this.LatestActiveMdiChild = null;
			}

			this.activatedForms.Remove( childForm );

			BarManager manager = this.GetManagerFrom( childForm );

			if( manager != null )
			{
				LockHostedFormRedraw();

				if( this.IsRegisteredType( childForm.GetType() ) )
				{
					foreach( MergedBar bar in htBarNameVsMergedBars.Values )
					{
						if( this.htBarNameVsMergedBars.Contains( bar.BarName ) )
						{
							manager.RestoreOriginalBar( bar );
						}
					}

					this.GetLoadedFormsOfType( childForm.GetType().FullName ).Remove( childForm );

					Type childType = childForm.GetType();

					// Check if this is an auto-registered type.
					if( this.IsAutoRegistered( childType ) )
					{
						int curCount = (int)this.htUnregisteredChildTypesVsInstanceCount[childType.FullName];
						curCount--;
						// If no more instances, then unregister.
						if( curCount == 0 )
						{
							this.htUnregisteredChildTypesVsInstanceCount.Remove( childType.FullName );
							this.UnregisterMdiChildTypes( new Type[] { childType }, false );
						}
						else
							this.htUnregisteredChildTypesVsInstanceCount[childType.FullName] = curCount;
					}
					// Helps when a child form is being mdi-unparented with other child forms
					// of the same type still active. Then we should restore this form's
					// state to it's original.
					this.RestoreOriginalChildBars( manager );
				}

				manager.RemoveReferencesToForeignItems();

				UnLockHostedFormRedraw();
			}
			else
			{
				// Do this only if called from Form_Close not when called from MdiChildRemoved
				// Because if called from Form_Close then this means the type does not
				// have a BarManager associated with it, so it's ok to do this.
				if( fromClose )
					// Should not be necessary, but doing this for sanity.
					this.formsByTypeName.Remove( childForm.GetType().FullName );
			}

			m_bNeedSaveCustomData = prevSaveCustomDataValue;
		}

		/// <summary>
		/// Checks whether the child type is a registered type.
		/// </summary>
		/// <param name="childType"></param>
		/// <param name="checkOnlyManualRegistration"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public bool IsRegisteredType( Type childType, bool checkOnlyManualRegistration )
		{
			Form dummyForm = null;
			if( this.formsByTypeName[childType.FullName] != null )
				dummyForm = this.GetDummyFormOfType( childType );

			bool isRegisteredType = dummyForm != null;
			if( checkOnlyManualRegistration )
				isRegisteredType &= !this.htUnregisteredChildTypesVsInstanceCount.Contains( childType.FullName );

			return isRegisteredType;
		}
		private bool IsManuallyRegistered( Type childType )
		{
			return this.IsRegisteredType( childType, true );
		}
		private bool IsAutoRegistered( Type childType )
		{
			return this.IsRegisteredType( childType )
				&& !this.IsManuallyRegistered( childType );
		}
		private bool IsRegisteredType( Type childType )
		{
			return this.IsRegisteredType( childType, false );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void InitNewMenuCategory()
		{
			if( !this.DesignMode )
			{
				int index = this.Categories.Add( SR.GetString( SR.CustomMenu, this) );
				NewMenuItem item = new NewMenuItem();
				item.CategoryIndex = index;
				item.Text = SR.GetString( SR.NewMenu, this);
				item.Manager = this;
			}
		}

		/// <summary>
		/// Gets called when the form is loaded.
		/// </summary>
		/// <param name="sender"> control </param>
		/// <param name="e">Event arguments</param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public override void FormLoaded( object sender, EventArgs e )
		{
			if( this.Form == null )
				return;

			// Do this as late as possible and Dispose it as soon as possible.
			if( !this.DesignMode )
			{
				MessageFilterEntryHelper.AddMessageFilter( this, false, this.Form );
			}

			this.InitNewMenuCategory();
			base.FormLoaded( sender, e );
		}

		private void FormActivated( object sender, EventArgs e )
		{
			if( this.Form == null )
				return;

			this.OnFormActivated( this.Form );
		}
        
		private void FormDeactivate( object sender, EventArgs e )
		{            
			this.ignoreNextAltKeyUp = false;
            
		}

		private void MdiChildActivated( object sender, EventArgs e )
		{
			if( this.Form == null )
				return;

			if( PopupManager.ActivePopupClient != null )
				PopupManager.ActivePopupClient.HidePopup( PopupCloseType.Deactivated );

			this.Form.SuspendLayout();
			this.commandBarManager.SuspendLayout();

			this.LatestActiveMdiChild = this.Form.ActiveMdiChild;

			this.Form.ResumeLayout( false );
			this.commandBarManager.ResumeLayout();
		}
		/// <summary>
		/// Registers the MDI child form types that could be created
		/// during the course of an MDI application.
		/// </summary>
		/// <seealso cref="UnregisterMdiChildTypes(Type[], bool)"/>
		/// <param name="formTypes">An array of Form types.</param>
		/// <remarks>
		/// <para><b>Explicit MDI Merging:</b></para>
		/// <para>
		/// Calling this method merges the menus and toolbars of the child forms with that
		/// of the main form (in an MDI scenario), immediately. The toolbars from the child forms
		/// will then be available in the "Toolbars List" for user-customization. </para> 
		/// <para>You should call this in either your MDI Parent Form's
		/// constructor or Load event handler.</para>
		/// <para>This method is intended to support the following scenario. In an MDI app, 
		/// you might want the XPMenus framework to
		/// merge the child form types's menus and toolbars that the MDI Parent will be
		/// parented to during the course of the application, even before the child forms are visible. 
		/// Doing so will then present a seemless interface to the user with the
		/// toolbars and menus from all the different child form types being available
		/// for user-customization, all the time.</para>
		/// <para>Also a dummy instance of the child form types will be created at this time
		/// to retrieve the bar items, requiring the child types to include a default constructor.</para>
		/// <para>Take a look at our User's Guide (section MDI Merging) for more information
		/// on how the <see cref="BarItem.MergeType"/>, <see cref="BarItem.MergeOrder"/> 
		/// and the <see cref="BarItem.Text"/> properties of BarItems influence the merge behavior of
		/// those BarItems.</para>
		/// <para><b>Automatic MDI Merging:</b></para>
		/// <para>If you do not call this method explicitly with your child form types, 
		/// the framework will call it for you when a child form (with XPMenus) gets loaded  
		/// and then automatically unregister 
		/// (using the <see cref="UnregisterMdiChildTypes(Type[], bool)"/> method)
		/// the child form type when all the corresponding instances of the child type are closed.
		/// </para>
		/// <para><b>Delayed MDI Merging:</b></para>
		/// <para>Sometimes you will dynamically load new child form types into your 
		/// MDI. In such cases you could optionally call this method in a later stage after 
		/// you access these new types and before instances of such types get 
		/// parented by the MDI parent. If you do not call this method, the child's menus
		/// will still be merged (see above "Automatic MDI Merging"), but they will be removed 
		/// when the child form is closed.</para>
		/// <para>In such dynamic child-form loading scenarios, you might also not know whether
		/// your child forms are associated with a ChildFrameBarManager in order for you to register
		/// such types using this method. You can determine that using the <see cref="BarManager.GetManagerFromForm"/>
		/// method.</para>
		/// <para>Use the UnregisterMdiChildTypes method to "unmerge" toolbars and menus from one or 
		/// more child form types. Take a look at the method reference for more information.</para>
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// Will be thrown if either of the types in the array is not derived from
		/// the Form class, if either of the types do not have a default constructor,
		/// if the passed in types do not have a ChildFrameBarManager associated with them
		/// or if the form associated with this MainFrameBarManager
		/// is not an MdiContainer.
		/// </exception>
		public virtual void RegisterMdiChildTypes( Type[] formTypes )
		{
			this.RegisterMdiChildTypes( formTypes, null );
		}

		private void RegisterMdiChildTypes( Type[] formTypes, Form[] forms )
		{
			bool prevSaveCustomDataValue = m_bNeedSaveCustomData;
			m_bNeedSaveCustomData = false;

			if( !this.Form.IsMdiContainer )
				throw new ArgumentException( "MdiChild Types can be registered only if the MainFrameBarManager's Form is an MdiParent." );

			ArrayList newForms = new ArrayList();

			int i = -1;
			foreach( Type formType in formTypes )
			{
				i++;
				if( this.formsByTypeName[formType.FullName] != null )
				{
					// Not an unregistered type anymore!
					this.htUnregisteredChildTypesVsInstanceCount.Remove( formType.FullName );
					continue;
				}

				if( !formType.IsSubclassOf( typeof( Form ) ) )
					throw new ArgumentException( "The registered Form Type has to be a subclass of type System.Windows.Form." );

				Form dummyForm = null;
				if( NewStyleChildTypeRegsitering && forms != null && forms.Length > i )
				{
					dummyForm = forms[i];
				}
				else
				{
					// Create a new one from the type
					dummyForm = this.GetFormFromType( formType );

					if( dummyForm == null )
						throw new ArgumentException( "Cannot find a default constructor in the form type: " + formType.ToString() );
				}

				newForms.Add( dummyForm );

				BarManager manager = GetManagerFrom( dummyForm );
				if( manager == null )
					throw new ArgumentException( "Cannot find a ChildFrameBarManager associated with the form type: " + formType.ToString() );

				InitDummyForm( dummyForm );
			}

			if( newForms.Count > 0 )
			{
				foreach( Form form in newForms )
				{
					this.UpdateUserChangesIn( form );
					this.AttachCommandBarsOfForm( form, false, true );
				}

				// The main-menus from the child forms are merged with the parent's main menu.
				// And the Bars with the same name are also merged togather.
				Form cachedActiveMdiChild = this.LatestActiveMdiChild;

				bool firstCall = this.mergedBars.Count == 0;

				if( !firstCall )
				{
					this.commandBarManager.SuspendLayout();
					this.LockBars();
					this.LatestActiveMdiChild = null;

					this.UnInitMergedBars();
				}

				this.InitMergedBars();

				if( !firstCall )
				{
					this.UpdateUserChangesIn( this.Form );
					this.LatestActiveMdiChild = cachedActiveMdiChild;
				}

				this.AttachCommandBarsOfForm( this.Form, true, true );

				if( !firstCall )
				{
					this.UnLockBars();
					this.commandBarManager.ResumeLayout();
				}
			}

			m_bNeedSaveCustomData = prevSaveCustomDataValue;
		}

		/// <summary>
		/// Re-registers forms that have been unregistered with a call to
		/// <see cref="UnregisterMdiChildTypes(Type[], bool)"/>.
		/// </summary>
		/// <param name="childForm">The child form that needs to be re-registered.</param>
		/// <remarks>
		/// <p>
		/// Use this method in the context of "Automatic MDI Merging" (see <see cref="RegisterMdiChildTypes(Type[])"/>
		/// for more information). In that context, you can use this method, for example, to merge/unmerge the menus 
		/// of a child form while it is active. The steps to do so are as follows:
		/// </p>
		/// <list type="Number">
		/// <item><description>Add a show a new mdi child form to the mdi parent.</description></item>
		/// <item><description>The menus defined in the mdi child form (using a <see cref="ChildFrameBarManager"/>) will be merged automatically
		/// with that of the main form.</description></item>
		/// <item><description></description>Add and show one or more instances of the same child form type, if necessary.</item>
		/// <item><description>Call <see cref="UnregisterMdiChildTypes(Type[])"/> to unregister this child form type. Then the menus
		/// defined in these child forms will be unmerged from the mdi parent's menu structure.</description></item>
		/// <item><description>Call <b>ReRegisterMdiChild</b> one of these child forms to re-register this child form type. Then, 
		/// the menus of all these child forms will be merged with the mdi parent's menus, once again.</description></item>
		/// </list>
		/// </remarks>
		public void ReRegisterMdiChild( Form childForm )
		{
			if( childForm.MdiParent != this.Form )
				throw new ArgumentException( "The child form should be the main form's MdiChild.", "childForm" );

			if( !this.IsRegisteredType( childForm.GetType() ) )
			{
				// If registering a currently visible & active form, reset the prop. below, because
				// we don't want OnFormDeactivated to get called within this call stack after this type gets registered.
				if( this.LatestActiveMdiChild != null
					&& this.LatestActiveMdiChild.GetType() == childForm.GetType() )
					this.LatestActiveMdiChild = null;

				this.OnMdiChildAdded( childForm );

				foreach( Bar bar in this.Bars )
				{
					CommandBar cmdBar = this.GetBarControl( bar );

					if( null != cmdBar )
					{
						cmdBar.Refresh();
					}
				}
			}
		}

		/// <summary>
		/// Removes the merging of one or more child form types.
		/// </summary>
		/// <param name="formTypes">An array of form types.</param>
		/// <param name="bForceRedraw">Indicates whether main form is forcedly redrawn after unregistering types of MDI child forms.</param>
		/// <remarks>
		/// <para>
		/// This is an advanced method that lets you "unmerge" menus and toolbars from 
		/// the child types that were either manually-registered using the <see cref="RegisterMdiChildTypes(Type[])"/> 
		/// method or auto-registered when new instances of such types were created (see the <b>RegisterMdiChildTypes</b> 
		/// method for more information on manual and auto registration).
		/// </para>
		/// <para>You can "remerge" the child form's menus
		/// with a call to <see cref="ReRegisterMdiChild"/>.</para>
		/// </remarks>
		public virtual void UnregisterMdiChildTypes( Type[] formTypes, bool bForceRedraw )
		{
			if( !this.Form.IsMdiContainer )
				throw new ArgumentException( "MdiChild Types can be unregistered only if the MainFrameBarManager's Form is an MdiParent." );

			try
			{
				this.LockBars();
				Form cachedCurrentActiveChild = null;
				bool updatedBarsCalled = false;
				ArrayList dummyFormTobeDisposed = new ArrayList();
				ArrayList unregisteredTypes = new ArrayList();

				foreach( Type formType in formTypes )
				{
					if( formType == this.GetType() )
						throw new ArgumentException( "The form type to unregister cannot be the main frame form type.", "formTypes" );

					if( this.formsByTypeName[formType.FullName] == null )
						continue;

					ArrayList forms = GetLoadedFormsOfType( formType.FullName );

					if( this.LatestActiveMdiChild != null
						&& this.LatestActiveMdiChild.GetType() == formType )
					{
						cachedCurrentActiveChild = this.LatestActiveMdiChild;
						this.LatestActiveMdiChild = null;
					}

					unregisteredTypes.Add( formType );
					if( !updatedBarsCalled )
					{
						// Update all the bars so that they get refreshed by items in the "Dummy Form"
						this.UpdateAllBars( formType );
						updatedBarsCalled = true;
					}
					Form dummyForm = forms[0] as Form;

					this.commandBarManager.RemoveBars( this.GetManagerFrom( dummyForm ), true );

					this.RemoveReferencesToItemsInManager( this.GetManagerFrom( dummyForm ), formType );

					this.htUnregisteredChildTypesVsInstanceCount.Remove( formType.FullName );

					foreach( Form form in forms )
					{
						this.activatedForms.Remove( form );
						BarManager childManager = this.GetManagerFrom( form );
						childManager.mainBarManager = null;
					}

					// Dispose them later.
					//dummyForm.Dispose();
					dummyFormTobeDisposed.Add( dummyForm );
				}
				if( unregisteredTypes.Count > 0 )
				{
					this.UnInitMergedBars();

					// Make sure to clear the formsByTypeName hash after the call to UnInitMergedBars.
					foreach( Type formType in unregisteredTypes )
					{
						ArrayList forms = this.formsByTypeName[formType.FullName] as ArrayList;
						forms.Clear();
						this.formsByTypeName.Remove( formType.FullName );
					}

					// Dispose off the dummy forms.
					foreach( Form dummyForm in dummyFormTobeDisposed )
						dummyForm.Dispose();
					dummyFormTobeDisposed.Clear();

					this.InitMergedBars();

					this.AttachCommandBarsOfForm( this.Form, this.Form.Visible, true );
					this.UpdateUserChangesIn( this.Form );

					if( cachedCurrentActiveChild != null && cachedCurrentActiveChild.Visible )
						this.LatestActiveMdiChild = cachedCurrentActiveChild;
				}
				foreach( Form dummyForm in dummyFormTobeDisposed )
				{
					dummyForm.Dispose();
				}
			}
			finally
			{
				this.UnLockBars();
			}
		}

		/// <summary>
		/// Removes the merging of one or more child form types.
		/// </summary>
		/// <param name="formTypes">An array of form types.</param>
		/// <remarks>
		/// <para>
		/// This is an advanced method that lets you "unmerge" menus and toolbars from 
		/// the child types that were either manually-registered using the <see cref="RegisterMdiChildTypes(Type[])"/> 
		/// method or auto-registered when new instances of such types were created (see the <b>RegisterMdiChildTypes</b> 
		/// method for more information on manual and auto registration).
		/// </para>
		/// <para>You can "remerge" the child form's menus
		/// with a call to <see cref="ReRegisterMdiChild"/>.</para>
		/// </remarks>
		public void UnregisterMdiChildTypes( Type[] formTypes )
		{
			UnregisterMdiChildTypes( formTypes, true );
		}

		/// <summary>
		/// Removes all the references of the specific bar item.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="unregisteredTypeName"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual void RemoveReferencesToBarItem( BarItem item, string unregisteredTypeName )
		{
			base.RemoveReferencesToBarItem( item );

			ArrayList unregisteredFormsList = this.formsByTypeName[unregisteredTypeName] as ArrayList;
			// And also remove references in the child BarManagers.
			foreach( ArrayList formsList in this.formsByTypeName.Values )
			{
				// Exclude the type that is being unregistered.
				if( formsList == unregisteredFormsList )
					continue;
				foreach( Form form in formsList )
				{
					if( form == this.Form )
						break;
					BarManager manager = this.GetManagerFrom( form );
					manager.RemoveReferencesToBarItem( item );
				}
			}
		}

		/// <summary>
		/// Removes all the references to the items in the specified manager.
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="unregisteredType"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual void RemoveReferencesToItemsInManager( BarManager manager, Type unregisteredType )
		{
			foreach( BarItem item in manager.Items )
				this.RemoveReferencesToBarItem( item, unregisteredType.FullName );
		}

		private void LockBars()
		{
			this.Form.SuspendLayout();
			this.commandBarManager.LockBars();
		}

		private void UnLockBars()
		{
			this.commandBarManager.UnLockBars();
			this.Form.ResumeLayout();
		}

		#region MERGED_BARS
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void InitMergedBars()
		{
			this.mergedBars.Clear();
			//this.replacedBars.Clear();
			this.htBarNameVsMergedBars.Clear();

			// Same logic as here... but different criteria for merging (based on IsMainMenu style).
			this.InitMainMenuBar();

			if( !this.Form.IsMdiContainer )
				return;

			Hashtable barsByName = new Hashtable();

			// First pass: Create a map of mergeable bars (by their bar name).
			this.FillHashWithBarsInForm( this.Form, barsByName );
			foreach( ArrayList formsList in this.formsByTypeName.Values )
			{
				if( formsList.Count > 0 )
				{
					Form form = formsList[0] as Form;
					if( form == this.Form )
						continue;
					this.FillHashWithBarsInForm( form, barsByName );
				}
			}

			// Second Pass: Merge the mergeable bars:
			//this.Bars.SuspendEvents();
			foreach( ArrayList mergeableList in barsByName.Values )
			{
				// Nothing to merge.
				if( mergeableList.Count == 0 )
					continue;

				Bar[] mergeableBars = new Bar[mergeableList.Count];
				int i = -1;
				foreach( Bar bar in mergeableList )
				{
					i++;
					mergeableBars[i] = bar;
				}

				// Merge them togather!
				MergedBar mergedBar = new MergedBar( mergeableBars, this );

				// Remove the equivalent one from the parent Bar list.
				//				i = -1;
				//				int matchedIndex = -1;
				//				foreach(Bar bar in this.Bars)
				//				{
				//					i++;
				//					if(bar.BarName == mergedBar.BarName)
				//					{
				//						matchedIndex = i;
				//						break;
				//					}
				//				}
				//
				//				if(matchedIndex != -1)
				//				{
				//					this.replacedBars.Add(this.Bars[matchedIndex]);
				//					this.Bars.RemoveAt(matchedIndex);
				//					this.Bars.Insert(matchedIndex, mergedBar);
				//				}
				//				else
				//				{
				//					this.replacedBars.Add(null);
				//					this.Bars.Add(mergedBar);
				//				}
				this.mergedBars.Add( mergedBar );
				this.htBarNameVsMergedBars.Add( mergedBar.BarName, mergedBar );
			}
			//this.Bars.ResumeEvents(false);
		}

		private void FillHashWithBarsInForm( Form form, Hashtable hash )
		{
			BarManager manager = this.GetManagerFrom( form );
			foreach( Bar bar in manager.Bars )
			{
				// Don't merge mainmenus here.
				if( ( bar.BarStyle & BarStyle.IsMainMenu ) == 0 )
					this.InsertIntoHash( hash, bar.BarName, bar );
			}
		}

		private void InsertIntoHash( Hashtable hash, string key, Bar value )
		{
			if( hash[key] == null )
			{
				hash[key] = new ArrayList();
			}

			( (ArrayList)hash[key] ).Add( value );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void UnInitMergedBars()
		{
			if( this.mergedBars.Count == 0 )
				return;

			// Replace the merged bar with the original one.
			//int i = -1;
			foreach( MergedBar mergedBar in this.mergedBars )
			{
				//i++;
				//int oldIndex = this.Bars.IndexOf(mergedBar);
				//this.Bars.Remove(mergedBar);
				this.htBarNameVsMergedBars.Remove( mergedBar.BarName );

				if( mergedBar == this.mainMenuBar )
					this.mainMenuBar = null;

				mergedBar.Dispose();

				//				// Can be null when the merge is done only on Bars from child forms.
				//				if(this.replacedBars[i] != null)
				//				{
				//					Bar replacedBar = this.replacedBars[i] as Bar;
				//					this.Bars.Insert(oldIndex, replacedBar);
				//				}
			}

			this.mergedBars.Clear();
			this.htBarNameVsMergedBars.Clear();
			//this.replacedBars.Clear();
		}
		private void RestoreOriginalChildBars( BarManager manager )
		{
			foreach( Bar mergedBar in this.mergedBars )
			{
				manager.RestoreOriginalBar( mergedBar );
			}
		}

		internal void RestoreOriginalChildBars( Bar mergedBar, string formTypeName )
		{
			if( this.formsByTypeName[formTypeName] == null )
				return;

			ArrayList forms = this.GetLoadedFormsOfType( formTypeName );
			BarManager barMan = null;

			foreach( Form form in forms )
			{
				barMan = this.GetManagerFrom( form );
				if( barMan == null ) continue;

				barMan.RestoreOriginalBar( mergedBar );
			}
		}

		internal void ReplaceChildBarsWithMergedBar( Bar mergedBar, Bar bar )
		{
			ArrayList forms = this.GetLoadedFormsOfType( BarManager.GetFormTypeName( bar.Manager ) );
			foreach( Form form in forms )
			{
				this.GetManagerFrom( form ).ReplaceBarsWithMergedBar( mergedBar );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void InitMainMenuBar()
		{
			string sMainMenuBarName = String.Empty;
			ArrayList mainMenuBars = new ArrayList();

			// Look for a main menu bar
			// Its important that the mainmenu of the MainFrameBarManager gets inserted
			// first in this list.
			foreach( Bar bar in this.Bars )
			{
				if( ( bar.BarStyle & BarStyle.IsMainMenu ) > 0 )
				{
					mainMenuBars.Add( bar );
					sMainMenuBarName = bar.BarName;
					break;
				}
			}

			if( this.Form.IsMdiContainer )
			{
				// Prepare a list of MainMenu bars in the the registered child forms.
				foreach( ArrayList formsList in this.formsByTypeName.Values )
				{
					Form form = formsList[0] as Form;
					if( form == this.Form )
						continue;

					Bar childMainMenuBar = ( String.Empty == sMainMenuBarName ) ?
						this.GetChildMainMenuBar( form ) : this.GetChildMainMenuBar( form, sMainMenuBarName );

					if( childMainMenuBar != null )
						mainMenuBars.Add( childMainMenuBar );
				}

				if( mainMenuBars.Count > 0 )
				{
					Bar[] bars = new Bar[mainMenuBars.Count];
					for( int i = 0; i < mainMenuBars.Count; i++ )
						bars[i] = mainMenuBars[i] as Bar;

					// Merge them togather!
					this.mainMenuBar = new MergedBar( bars, this );
					this.mergedBars.Add( this.mainMenuBar );
					this.htBarNameVsMergedBars.Add( this.MainMenuBar.BarName, this.mainMenuBar );

					// Remove the existing MainMenu Bar
					//					int index = -1;
					//					int mainMenuIndex = -1;
					//					// Silently replace the existing main-menu bar with the merged one.
					//					this.Bars.SuspendEvents();
					//					foreach(Bar bar in this.Bars)
					//					{
					//						index++;
					//						if((bar.BarStyle & BarStyle.IsMainMenu) > 0)
					//						{
					//							mainMenuIndex = index;
					//							this.replacedBars.Add(bar);
					//							this.Bars.Remove(bar);
					//							break;
					//						}
					//					}
					//					if(mainMenuIndex == -1)
					//					{
					//						this.replacedBars.Add(null);
					//						mainMenuIndex = 0;
					//					}
					//					this.Bars.Insert(mainMenuIndex, this.mainMenuBar);
					//					this.Bars.ResumeEvents(false);
					//this.commandBarManager.LoadDesignerState(this);
				}
			}
			else
			{
                //if( mainMenuBars.Count > 0 )
                //    this.mainMenuBar = mainMenuBars[0] as Bar;
			}
			//			if(this.mainMenuBar != null)
			//				this.commandBarManager.AttachMainMenuBar(this.mainMenuBar);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual Bar GetChildMainMenuBar( Form form )
		{
			BarManager manager = GetManagerFrom( form );
			foreach( Bar bar in manager.Bars )
			{
				if( ( bar.BarStyle & BarStyle.IsMainMenu ) > 0 )
				{
					return bar;
				}
			}

			return null;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual Bar GetChildMainMenuBar( Form form, string mainMenuBarName )
		{
			BarManager barMan = GetManagerFrom( form );
			Bar mainBar = GetChildMainMenuBar( form );

			if( null == mainBar )
			{
				foreach( Bar bar in barMan.Bars )
				{
					if( mainMenuBarName == bar.BarName )
					{
						mainBar = bar;
						mainBar.BarStyle |= BarStyle.IsMainMenu;
						break;
					}
				}
			}

			return mainBar;
		}
		#endregion MERGED_BARS


		/// <summary>
		/// Resets the toolbar positions to the default designer set positions.
		/// </summary>
		/// <remarks>
		/// Calling this method will move the toolbars to the designer set positions. Note that
		/// this applies only the toolbars in the <see cref="MainFrameBarManager"/> not the ones
		/// in the <see cref="ChildFrameBarManager"/> (that manages a MDI Child). This is because
		/// the toolbars in an mdi child do not have any positional information associated with them.
		/// </remarks>
		public void LoadDesignerBarState()
		{
			if( this.commandBarManager != null )
				this.commandBarManager.LoadDesignerStateInternal( this, true );
		}

		/// <summary>
		/// Reads the previously serialized toolbar or menu states.
		/// </summary>		
		/// <param name="serializer">A reference to the <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.</param>
		/// <remarks>
		/// Reads the persisted information from the specified persistent store and applies the new state. 
		/// This is an advanced method provided to let you customize the serialization path. By default, the framework
		/// will automatically store the user customized menu and toolbar information in the Isolated Storage (this would however
		/// be controlled by the AutoPersistCustomization and AutoLoadToolBarPositions settings).
		/// </remarks>
		public void LoadBarState( AppStateSerializer serializer )
		{
			this.LoadBarStateInternal( serializer, true );
		}

		private void LoadBarStateInternal( AppStateSerializer serializer, bool lockBars )
		{
			if( ( this.DesignMode ) || s_isDevEnv )
				return;

			try
			{
				if( this.commandBarManager != null )
				{
					if( lockBars )
						this.commandBarManager.LockBars();
					this.commandBarManager.LoadCommandBarState( serializer );
				}
			}
			catch { }
			finally
			{
				if( this.commandBarManager != null && lockBars )
					this.commandBarManager.UnLockBars();
			}
		}

		/// <summary>
		/// Loads the user customized information from the specified persistence medium.
		/// </summary>
		/// <param name="serializer">A reference to the <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.</param>
		/// <remarks>
		/// <para>This method is not public because loading and saving of the user customized info 
		/// needs to be performed at a specific time and the framework doesn't support calling
		/// this method at any other time.</para>
		/// <para>
		/// To customize this persistence mechanism, please override this method and provide a
		/// custom <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.
		/// </para>
		/// </remarks>
		protected virtual void LoadCustomizationInfo( AppStateSerializer serializer )
		{
			if( ( this.DesignMode ) || s_isDevEnv )
				return;

			//this.UpdateCachedCustomizationSettings(serializer);

			try
			{
				object tempList = null;
				// Retrieve changes made to BarItems
				tempList = serializer.DeserializeObject( this.PersistenceID + ":" + CustomizedBarSettingsLabel );
				if( tempList != null )
					this.updateInfoList = tempList as ArrayList;

				// BarItems in Bars visibility
				tempList = serializer.DeserializeObject( this.PersistenceID + ":" + BarItemsVisibilityInBarLabel );
				if( tempList != null )
					this.barItemVisibilityInBars = tempList as Hashtable;

				tempList = serializer.DeserializeObject( this.PersistenceID + ":" + CustomBarsListLabel );
				if( tempList != null )
					this.customAddedBarsVsNames = tempList as ArrayList;

				tempList = serializer.DeserializeObject( this.PersistenceID + ":" + CustomItemsListLabel );
				if( tempList != null )
					this.customAddedItemsVsBarItemID = tempList as ArrayList;

				tempList = serializer.DeserializeObject( this.PersistenceID + ":" + CustomItemsContainerListLabel );
				if( tempList != null )
				{
					this.customContainers = tempList as Hashtable;
					m_bCustomContainersLoaded = true;
				}
			}
			catch { }
		}

		/// <summary>
		/// Saves the current toolbars or menus state information to the specified persistence medium.
		/// </summary>
		/// <param name="serializer">A reference to the <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.</param>
		/// <remarks>
		/// Saves the persisted information into the specified persistent store. 
		/// This is an advanced method that lets you save the customized state of menus and toolbars into a custom persistence location.
		/// By default, the framework will handle saving and loading this information automatically.
		/// </remarks>
		public void SaveBarState( AppStateSerializer serializer )
		{
			if( this.DesignMode )
				return;

			if( this.commandBarManager != null )
				this.commandBarManager.SaveCommandBarState( serializer );
		}

		/// <summary>
		/// Saves the user customized information to the specified persistence medium.
		/// </summary>
		/// <param name="serializer">A reference to the <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.</param>
		/// <remarks>
		/// <para>This method is not public because loading and saving of the user customized info 
		/// needs to be performed at a specific time and the framework doesn't support calling
		/// this method at any other time.</para>
		/// <para>
		/// To customize this persistence mechanism, please override this method and provide a
		/// custom <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.
		/// </para>
		/// </remarks>
		protected virtual void SaveCustomizationInfo( AppStateSerializer serializer )
		{
			if( this.DesignMode )
				return;

			//this.UpdateCachedCustomizationSettings(serializer, saveUserCustomizedInfo);

			if( this.AutoSaveCustomData )
			{
				foreach( BarUpdateInfo info in this.updateInfoList )
				{
					if( info.updateType == UpdateType.InsertAfter || info.updateType == UpdateType.InsertBefore )
					{
						ArrayList list = null;
						BarItemID id = info.source;

						if( !this.customContainers.Contains( id ) )
						{
							list = new ArrayList();
							list.Add( info.destination );

							this.customContainers.Add( id, list );
						}
						else
						{
							list = this.customContainers[id] as ArrayList;
							if( !list.Contains( info.destination ) )
								list.Add( info.destination );
						}
					}
				}
			}
			else
			{
				for( int i = 0; i < this.updateInfoList.Count; i++ )
				{
					BarUpdateInfo updateInfo = (BarUpdateInfo)this.updateInfoList[i];
					if( updateInfo.updateType == UpdateType.InsertAfter || updateInfo.updateType == UpdateType.InsertBefore )
					{
						if( !this.customAddedItemsVsBarItemID.Contains( updateInfo.source ) )
						{
							BarItem item = this.GetBarItemFromBarItemID( updateInfo.source );
							if( item != null )
							{
								this.RemoveReferencesToBarItem( item );
								i--;
							}
						}
					}
				}
			}

			serializer.SerializeObject( this.PersistenceID + ":" + CustomizedBarSettingsLabel, this.updateInfoList );
			serializer.SerializeObject( this.PersistenceID + ":" + BarItemsVisibilityInBarLabel, this.barItemVisibilityInBars );
			serializer.SerializeObject( this.PersistenceID + ":" + CustomBarsListLabel, this.customAddedBarsVsNames );
			serializer.SerializeObject( this.PersistenceID + ":" + CustomItemsListLabel, this.customAddedItemsVsBarItemID );

			if( customContainers.Count > 0 )
			{
				serializer.SerializeObject( this.PersistenceID + ":" + CustomItemsContainerListLabel, this.customContainers );
			}
		}

		/// <summary>
		/// Resets recently used items list.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual void ResetRecentlyUsedItemsList()
		{
			for( int i = this.updateInfoList.Count - 1; i >= 0; i-- )
			{
				BarUpdateInfo updateInfo = (BarUpdateInfo)this.updateInfoList[i];
				if( updateInfo.updateType == UpdateType.RecentlyUsedItemClicked )
					this.updateInfoList.RemoveAt( i );
			}
		}

		/// <summary>
		/// Resets the container.
		/// </summary>
		/// <param name="container"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual void ResetContainer( IBarItemContainer container )
		{
			if( !( container is Bar ) )
				return;

			Bar bar = container as Bar;
			BarID barID = new BarID( bar.BarName, BarManager.GetFormTypeName( bar.Manager ) );

			// Remove entries whose destination is this container.
			for( int i = this.updateInfoList.Count - 1; i >= 0; i-- )
			{
				BarUpdateInfo updateInfo = (BarUpdateInfo)this.updateInfoList[i];
				if( updateInfo.destination is BarID )
				{
					BarID destID = (BarID)updateInfo.destination;
					if( destID.containerName == barID.containerName
						&& destID.formTypeName == barID.formTypeName )
						this.updateInfoList.RemoveAt( i );
				}
			}

			for( int i = 0; i < container.Items.Count; i++ )
			{
				BarItem barItem = container.Items[i];

				this.ResetBarItem( barItem );
				// restore bar item visibility
				this.SetUserVisibilityPreferenceInBar( barItem, bar, true );
			}
		}

		/// <summary>
		/// Resets user customization done to this item.
		/// </summary>
		/// <param name="barItem">A <see cref="BarItem"/> instance.</param>
		/// <returns>True if the reset was done immediately; false if the reset will happen during next application load.</returns>
		/// <remarks>
		/// <para>Changes will be seen only when the application is restarted.</para>
		/// </remarks>
		public bool ResetBarItem( BarItem barItem )
		{
			if( barItem is CustomParentMenuItem )
			{
				this.RemoveCustomParentMenu( barItem as CustomParentMenuItem );
				return true;
			}

			BarItemID barItemID = BarItemID.FromBarItem( barItem );

			// Remove entries that indicate changes made to this item.
			for( int i = this.updateInfoList.Count - 1; i >= 0; i-- )
			{
				BarUpdateInfo updateInfo = (BarUpdateInfo)this.updateInfoList[i];

				if( updateInfo.updateType == UpdateType.ModifiedPaintStyle
                    || updateInfo.updateType == UpdateType.ModifiedText || updateInfo.updateType == UpdateType.ChangedImage)
				{
					if( updateInfo.destination is BarItemID ) // Fix for Defect # 2361.
					{
						BarItemID destination = (BarItemID)updateInfo.destination;
						if( destination.barItemID == barItemID.barItemID
							&& destination.formTypeName == barItemID.formTypeName )
							this.updateInfoList.RemoveAt( i );
					}
				}
			}
			return false;
		}

        /// <summary>
        /// Removes the previously added change image BarUpdateinfo.
        /// </summary>
        /// <param name="barItem">The bar item.</param>
        internal void RemovePreviouslyAddedChangeImageInfo(BarItem barItem)
        {
            BarItemID barItemID = BarItemID.FromBarItem(barItem);

            for (int i = this.updateInfoList.Count - 1; i >= 0; i--)
            {
                BarUpdateInfo updateInfo = (BarUpdateInfo)this.updateInfoList[i];

                if (updateInfo.updateType == UpdateType.ChangedImage)
                {
                    if (updateInfo.destination is BarItemID)
                    {
                        BarItemID destination = (BarItemID)updateInfo.destination;
                        if (destination.barItemID == barItemID.barItemID
                            && destination.formTypeName == barItemID.formTypeName)
                            this.updateInfoList.RemoveAt(i);
                    }
                }
            }
        }

		/// <summary>
		/// This method will be called when the user inserted an item into a <see cref="Bar"/> or <see cref="ParentBarItem"/>
		/// during runtime using customization.
		/// </summary>
		/// <param name="sourceItem">The <see cref="BarItem"/> that has been inserted into the container.</param>
		/// <param name="adjacentItem">The <see cref="BarItem"/> next to which the above item has been inserted.</param>
		/// <param name="destination">The <see cref="Bar"/> or <see cref="ParentBarItem"/> into which the soruceItem is being inserted.</param>
		/// <param name="insertBefore">Indicates whether the sourceItem is inserted before or after the adjacentItem.</param>
		public virtual void RecordInsert( BarItem sourceItem, BarItem adjacentItem,
			IBarItemContainer destination, bool insertBefore )
		{
			UpdateType updateType = insertBefore ? UpdateType.InsertBefore : UpdateType.InsertAfter;

			BarItem clone = null;
			ParentBarItem parentItem = sourceItem as ParentBarItem;
			if( parentItem != null )
			{
				clone = parentItem.Clone( true ) as ParentBarItem;
			}
			else
			{
				clone = sourceItem.Clone() as BarItem;
			}

			if( insertBefore && adjacentItem != null )
			{
				if( groupBeginersHash.Contains( adjacentItem ) )
					groupBeginersHash.Remove( adjacentItem );

				groupBeginersHash.Add( adjacentItem, destination.IsGroupBeginning( adjacentItem ) );
			}

			this.RecordCommon( sourceItem, adjacentItem, destination, updateType, clone );
		}

		/// <summary>
		/// This method will be called when the user removed a <see cref="BarItem"/> in a <see cref="Bar"/> 
		/// or <see cref="ParentBarItem"/> during runtime customization.
		/// </summary>
		/// <param name="sourceItem">The <see cref="BarItem"/> that was removed.</param>
		/// <param name="destination">The <see cref="Bar"/> or <see cref="ParentBarItem"/> from which the sourceItem was removed.</param>
		public virtual void RecordRemove( BarItem sourceItem, IBarItemContainer destination )
		{
			BarItem clone = null;
			ParentBarItem parentItem = sourceItem as ParentBarItem;
			if( parentItem != null )
			{
				clone = parentItem.Clone( true ) as ParentBarItem;
			}
			else
			{
				clone = sourceItem.Clone() as BarItem;
			}

			if( groupBeginersHash.Contains( sourceItem ) )
				groupBeginersHash.Remove( sourceItem );

			groupBeginersHash.Add( sourceItem, destination.IsGroupBeginning( sourceItem ) );

			int index = destination.Items.IndexOf( sourceItem );
			BarItem adjacentItem = index > 0 ? destination.Items[index - 1] : null;

			this.RecordCommon( sourceItem, adjacentItem, destination, UpdateType.Remove, clone );
		}

		private Hashtable groupBeginersHash = new Hashtable();

		[Syncfusion.Documentation.DocumentationExclude()]
		public void RecordModifiedPaintStyle( BarItem sourceItem, PaintStyle newStyle )
		{
			this.RecordCommon( null, null, sourceItem, UpdateType.ModifiedPaintStyle, newStyle );
		}

        [Syncfusion.Documentation.DocumentationExclude()]
        public void RecordChangedImage(BarItem sourceItem, ImageExt image)
        {
            this.RecordCommon(null, null, sourceItem, UpdateType.ChangedImage, image);
        }

		/// <summary>
		/// Records the bar caption.
		/// </summary>
		/// <param name="sourceItem">
		/// Source bar
		/// </param>
		/// <param name="strCaption">
		/// Caption
		/// </param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void RecordBarCaption( Bar sourceItem, string strCaption )
		{
			if( strCaption == null )
				throw new ArgumentNullException( "strCaption" );

			this.RecordCommon( null, null, sourceItem, UpdateType.ModifiedText, strCaption );
		}

		/// <summary>
		/// Records modified caption.
		/// </summary>
		/// <param name="sourceItem"></param>
		/// <param name="newCaption"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void RecordModifiedCaption( BarItem sourceItem, string newCaption )
		{
			this.RecordCommon( null, null, sourceItem, UpdateType.ModifiedText, newCaption );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public void RecordModifiedGrouping( BarItem sourceItem, bool isGroupBeginner, IBarItemContainer parentItem )
		{
			this.RecordCommon( sourceItem, null, parentItem, UpdateType.ModifiedGrouping, isGroupBeginner );
		}

		/// <summary>
		/// Records recently used item  clicked.
		/// </summary>
		/// <param name="sourceItem"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void RecordRecentlyUsedItemClicked( BarItem sourceItem )
		{
			this.RecordCommon( null, null, sourceItem, UpdateType.RecentlyUsedItemClicked, DateTime.Now );
		}

		/// <summary>
		/// Adds custom parent menu.
		/// </summary>
		/// <param name="id">
		/// unique id to be used for this menu</param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void RecordAddCustomParentMenu( string id )
		{
			this.RecordCommon( null, null, null, UpdateType.CustomParentItem, new BarItemID( id, "" ) );
		}

		/// <summary>
		/// Serves to remove the custom menu.
		/// </summary>
		/// <param name="item"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void RemoveCustomParentMenu( CustomParentMenuItem item )
		{
			// Removing only the creation entry
			// Corresponding Insert and Remove entries will fail gracefully.
			foreach( BarUpdateInfo updateInfo in this.updateInfoList )
			{
				if( updateInfo.updateType == UpdateType.CustomParentItem )
				{
					BarItemID barItemID = (BarItemID)updateInfo.updateData;
					if( barItemID.barItemID == item.ID )
					{
						this.updateInfoList.Remove( updateInfo );
						break;
					}
				}
			}
			this.RemoveReferencesToBarItem( item );
		}

		/// <summary>
		/// Adds the custom bar.
		/// </summary>
		/// <param name="barName">
		/// Bar Name
		/// </param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void RecordAddCustomBar( string barName )
		{
			this.RecordCommon( null, null, null, UpdateType.CustomBarAdded, barName );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		private void RecordAddCustomBarInfo( Bar bar )
		{
			Bar clone = bar.Clone( true ) as Bar;
			this.RecordCommon( null, null, null, UpdateType.CustomBarInfoAdded, clone );
		}

		/// <summary>
		/// Serves to remove the custom bar.
		/// </summary>
		/// <param name="bar"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void RemoveCustomBar( Bar bar )
		{
			// Parse through my updateInfo list and remove the entry corresponding to this one.
			foreach( BarUpdateInfo updateInfo in this.updateInfoList )
			{
				if( updateInfo.updateType == UpdateType.CustomBarAdded )
				{
					string barName = updateInfo.updateData as string;
					if( barName == bar.BarName )
					{
						this.updateInfoList.Remove( updateInfo );
						break;
					}
				}
				else if( updateInfo.updateType == UpdateType.CustomBarInfoAdded )
				{
					Bar updateBar = updateInfo.updateData as Bar;
					if( updateBar.BarName == bar.BarName )
					{
						this.updateInfoList.Remove( updateInfo );
						break;
					}
				}
			}

			if( this.Bars.Contains( bar ) )
				this.Bars.Remove( bar );

			if( this.mainMenuBar == bar )
			{
				this.mainMenuBar = null;
			}
		}

		/// <summary>
		/// Specifies whether the bar can be deleted by the user.
		/// </summary>
		/// <param name="bar">
		/// Bar to be checked
		/// </param>
		/// <returns> True if the bar can be deleted by user, else returns false. </returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public bool IsBarUserDeletable( Bar bar )
		{
			bool deletable = false;

			foreach( BarUpdateInfo updateInfo in this.updateInfoList )
			{
				if( updateInfo.updateType == UpdateType.CustomBarAdded )
				{
					string barName = updateInfo.updateData as string;
					if( barName == bar.BarName )
						deletable = true;
				}
				else if( updateInfo.updateType == UpdateType.CustomBarInfoAdded )
				{
					Bar updateBar = updateInfo.updateData as Bar;
					if( updateBar.BarName == bar.BarName )
						deletable = true;
				}
			}

			return deletable;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void RecordCommon( BarItem sourceItem, BarItem adjacentItem,
			object destinationItem, UpdateType updateType, object updateData )
		{
			this.OnBeforeRecording( sourceItem, adjacentItem,
				destinationItem, updateType, updateData );
			// Source can be null, in case of changing PaintStyle or Text
			BarItemID barItemID = BarItemID.Empty;
			if( sourceItem != null )
            {
                BarManager sourceManager = sourceItem.Manager;
                if (updateType == UpdateType.Remove && sourceItem.Manager == null)
                {
                    BarItem item = destinationItem as BarItem;
                    if (item != null && item.Manager != null)
                        sourceManager = item.Manager;
                    else
                        sourceManager = this;
                }
                barItemID = new BarItemID(sourceItem.ID, BarManager.GetFormTypeName(sourceManager));
            }
			// Adjacent Item can be null in case of Remove
			BarItemID adjacentItemID = BarItemID.Empty;
			if( adjacentItem != null && !( adjacentItem is StandAloneBarItem ) )
				adjacentItemID = new BarItemID( adjacentItem.ID, BarManager.GetFormTypeName( adjacentItem.Manager ) );

			// Destination
			object destinationID = null;
			if( destinationItem is Bar )
			{
				Bar bar = destinationItem as Bar;
				BarID barID = new BarID( bar.BarName, BarManager.GetFormTypeName( bar.Manager ) );
				destinationID = barID;
			}
			else if( destinationItem is BarItem )
			{
				BarItem item = destinationItem as BarItem;
				destinationID = new BarItemID( item.ID, BarManager.GetFormTypeName( item.Manager ) );
			}
			// Store them for later use
			this.StoreUpdateInfo( new BarUpdateInfo( barItemID, adjacentItemID, destinationID, updateType, updateData ) );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void OnBeforeRecording( BarItem sourceItem, BarItem adjacentItem,
			object destinationItem, UpdateType updateType, object updateData )
		{
			if( updateType == UpdateType.UsePartialMenus
				|| updateType == UpdateType.LargeIcons )
			{
				// Remove any existing entry.
				foreach( BarUpdateInfo info in this.updateInfoList )
				{
					if( ( info.updateType == UpdateType.UsePartialMenus && updateType == UpdateType.UsePartialMenus ) ||
						( info.updateType == UpdateType.LargeIcons && updateType == UpdateType.LargeIcons ) )
					{
						this.updateInfoList.Remove( info );
						break;
					}
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void StoreUpdateInfo( BarUpdateInfo updateInfo )
		{
			if( updateInfo.updateType == UpdateType.RecentlyUsedItemClicked )
				// Insert it in-front!
				this.updateInfoList.Insert( 0, updateInfo );
			else
			{
				if( updateInfo.updateType == UpdateType.CustomBarInfoAdded )
				{
					string barName = ( updateInfo.updateData as Bar ).BarName;
					foreach( BarUpdateInfo savedInfo in this.updateInfoList )
					{
						if( savedInfo.updateType == UpdateType.CustomBarInfoAdded )
						{
							string savedBarName = ( savedInfo.updateData as Bar ).BarName;
							if( barName == savedBarName )
								return;
						}
					}
				}

				this.updateInfoList.Add( updateInfo );
			}

			// Pares through all the loaded forms and update them as well
			foreach( ArrayList formsList in this.formsByTypeName.Values )
				foreach( Form form in formsList )
				{
					BarManager manager = GetManagerFrom( form );
					bool temp = true;
                    if(manager != null)
					    this.UpdateUserChangeIn( manager, updateInfo, ref temp );
				}
		}

		/// <summary>
		/// Performs the actions required while the form being closed.
		/// </summary>
		/// <param name="form"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual void OnFormClosed( Form form )
		{
			if( form == this.Form )
			{
				MessageFilterEntryHelper.RemoveMessageFilter( this );

				// Ensuring that any existing state in the serializer is removed.
				if( this.ResetCustomization )
				{
					AppStateSerializer.GetSingleton().SerializeObject( this.PersistenceID + ":" + CustomizedBarSettingsLabel, null );
					AppStateSerializer.GetSingleton().SerializeObject( this.PersistenceID + ":" + BarItemsVisibilityInBarLabel, null );
					AppStateSerializer.GetSingleton().SerializeObject( this.PersistenceID + ":" + CustomBarsListLabel, null );
					AppStateSerializer.GetSingleton().SerializeObject( this.PersistenceID + ":" + CustomItemsListLabel, null );
					AppStateSerializer.GetSingleton().SerializeObject( this.PersistenceID + ":" + CustomItemsContainerListLabel, null );
				}
				// If resetting, then force saving the null values.
				else if( this.AutoPersistCustomization )
				{
					SaveCustomizationInfo( AppStateSerializer.GetSingleton() );
					SaveBarState( AppStateSerializer.GetSingleton() );
				}
			}

			if( form != this.Form )
			{
				CommandBarManager cbManager = this.commandBarManager;

				if( cbManager != null )
				{
					cbManager.SuspendLayout();
				}

				OnMdiChildRemoved( form, true );

				// Do this only for child forms.
				// For main forms, if it were a dialog-form, it could be shown again with a call to ShowDialog.
				BarManager.htFormsVsBarManager.Remove( form );

				if( cbManager != null )
				{
					cbManager.ResumeLayout();
				}
			}

			this.activatedForms.Remove( form );
		}

		private void BringFormToFront( Form form )
		{
			ArrayList forms = this.GetLoadedFormsOfType( BarManager.GetFormTypeName( form ) );
			forms.Remove( form );
			// Move it to front
			forms.Insert( 0, form );
		}

		/// <summary>
		/// Performs the actions required while the form gets activated.
		/// </summary>
		/// <param name="form"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual void OnFormActivated( Form form )
		{
			bool prevSaveCustomDataValue = m_bNeedSaveCustomData;
			m_bNeedSaveCustomData = false;

			// Is it the first time this form gets activated?
			bool firstActivationForForm = this.activatedForms[form] == null;

			if( !form.Visible && !firstActivationForForm )
			{
				return;
			}

			if( this.Form != form && ( this.AutoPersistCustomization || this.AutoLoadToolBarPositions ) )
			{
				this.MdiChildrenFormName = form.Name;
			}

			if( this.mainMenuBar == null && this.mergedBars.Count == 0 )
			{
				this.commandBarManager.LockBars();

				this.InitMergedBars();

				this.commandBarManager.UnLockBars();
			}

			BarManager manager = GetManagerFrom( form );

			if( manager != null )
			{
				this.activatedForms[form] = 1;

				if( this.IsRegisteredType( form.GetType() ) )
				{
					this.BringFormToFront( form );

					// We used to do this in OnMdiChildAdded, but that is not good enough since once the child
					// is added it's type could be unregistered and registered at a later stage.
					// Remove merged toolbars.
					for( int i = manager.Bars.Count - 1; i >= 0; i-- )
					{
						Bar bar = manager.Bars[i];

						if( this.htBarNameVsMergedBars.Contains( bar.BarName ) )
							manager.ReplaceBarsWithMergedBar( this.htBarNameVsMergedBars[bar.BarName] as Bar );
					}
				}

				// Make sure its containers are updated for changes.
				if( firstActivationForForm )
				{
					this.UpdateUserChangesIn( form );
				}

				if( form != this.Form || firstActivationForForm )
				{
                    if (this.AutoLoadToolBarPositions)
                    {
                       // UpdateCommandBarState(this.Form);

                        if (this.Form != form)
                        {
                            UpdateCommandBarState(form);
                        }
                    }

					UpdateBars();
				}

				if( form != this.Form || this.Form.ActiveMdiChild == null )
				{
					this.ActiveForm = form;
				}

				if( this.IsRegisteredType( form.GetType() ) )
				{
					this.AttachCommandBarsOfForm( form, true, false );

					if( this.mergedBars.Count > 0 )
					{
						foreach( Bar bar in this.mergedBars )
							this.commandBarManager.RedrawBar( bar );
					}
				}
			}

			this.Form.ResumeLayout( true );

			m_bNeedSaveCustomData = prevSaveCustomDataValue;
		}


		/// <summary>
		/// Performs the actions required while the form gets deactivated.
		/// Note:
		/// updateBars lets you optionally delay the updation of the bars in OnFormActivated.
		/// </summary>
		/// <param name="form"></param>
		/// <param name="updateBars"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual void OnFormDeactivated( Form form, bool updateBars )
		{
			if( this.Form != form )
			{
				BarManager manager = GetManagerFrom( form );
				if( manager != null )
				{
					if( this.IsRegisteredType( form.GetType() ) )
					{                       
						if( this.AutoPersistCustomization )
						{
							this.MdiChildrenFormName = form.Name;
							SaveBarStateForForm( this.Form );
							SaveBarStateForForm( form );
							this.MdiChildrenFormName = String.Empty;
						}

						if( this.AutoLoadToolBarPositions && m_htChildForms.Count == 0 )
						{
							UpdateCommandBarState( this.Form );
						}

						// Move the dummy form of this type to the front
						ArrayList forms = this.GetLoadedFormsOfType( form.GetType().FullName );
						Form dummyForm = GetDummyFormOfType( form.GetType() );
						forms.Remove( dummyForm );
						forms.Insert( 0, dummyForm );

						if( updateBars )
						{
							UpdateBars();
						}

						// This could be moved within the above if. But, the merge logic (in MergedItems.cs)
						// expects the ActiveForm to be set to the Parent Form, before getting reset to a new Child Form.
						this.ActiveForm = this.Form;

						//this.UpdateMainMenuBar(this.Form);
						this.commandBarManager.HideBars( manager, false );

						this.AttachCommandBarsOfForm( dummyForm, false, false );
					}
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		internal protected override void OnBeginCustomization( EventArgs args )
		{
			this.cachedCustomizingState = true;
			//			No need for subclassing during design-time.
			//			If I do subclass and use AssingHandle, the Control and its Handle go out of sync
			//			when I later call ReleaseHandle.
			//			if(this.DesignMode)
			//				this.mainWndSubclass.AssignHandle(this.Form.Handle);
			//			else
			if( !this.DesignMode )
			{
				// Assign once.
				if( this.mainWndSubclass.Handle != this.Form.Handle )
					//this.mainWndSubclass.AssignHandleCustom(this.Form.Handle);
					this.mainWndSubclass.AssignHandleCustom( this.Form );
				// Activate it.
				this.mainWndSubclass.Active = true;
			}

			base.OnBeginCustomization( args );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		internal protected override void OnCustomizationDone( EventArgs args )
		{
			this.cachedCustomizingState = false;
			//			if(this.DesignMode)
			//				this.mainWndSubclass.ReleaseHandle();
			//			else

			if( !this.DesignMode )
				// Releasing the handle has problems.
				//this.mainWndSubclass.ReleaseHandleCustom();
				this.mainWndSubclass.Active = false;

			base.OnCustomizationDone( args );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void RefreshContainer( IBarItemContainer container )
		{
			for( int i = 0; i < container.Items.Count; i++ )
			{
				BarItem item = container.Items[i];

				if( item.IsDisposed ) continue;

				if( item.Manager == null && container.Manager != null && !container.Manager.Items.Contains( item ) )
				{
					container.Manager.Items.Add( item );
				}

				if( item.Manager == null )
					throw new NullReferenceException( "A BarItem (Text: " + item.Text + ") that is not part of a BarManager is found in the menus or toolbars. Make sure to add the BarItem to a BarManager before adding it to the menus or toolbars." );

				BarManager itemManager = item.Manager;
				Form itemForm = itemManager.Form;

				if( item is StandAloneBarItem || itemForm == null )
					continue;

				Form activeForm = this.GetActiveFormOfType( BarManager.GetFormTypeName( itemManager ) );

				if( null != activeForm && itemForm != activeForm )
				{
					BarItemID barItemId = new BarItemID( item.ID, BarManager.GetFormTypeName( itemForm ) );
					BarItem newItem = GetManagerFrom( activeForm ).GetBarItemFromBarItemID( barItemId );
					// newItem could be null, for example, when a child item was added to 
					// a child form but not in the dummy form!
					if( newItem != null )
					{
						//this.helper.ParentItem = container;
						// Replace the old item with the new item, keeping the groupings intact
						int curIndex = container.Items.IndexOf( item );
						BarItem oldItem = item;

						bool beginGroup = container.IsGroupBeginning( oldItem );

						if( beginGroup )
							container.RemoveGroupAt( oldItem );

						container.Items[curIndex] = newItem;

						if( beginGroup )
							container.BeginGroupAt( newItem );
					}
				}
			}
		}
		//		[Syncfusion.Documentation.DocumentationExclude()]
		//		protected virtual void UpdateMainMenuBar(Form activeForm)
		//		{
		//			if(this.mainMenuBar != null)
		//			{
		//				BarManager activeManager = GetManagerFrom(activeForm);
		//				foreach(BarItem item in this.mainMenuBar.Items)
		//				{
		//					if(item is MergedParentBarItem)
		//						this.UpdateMergedItems(item as MergedParentBarItem, activeManager);
		//				}
		//			}
		//		}
		//
		//		private void UpdateMergedItems(MergedParentBarItem mergedItem, BarManager activeManager)
		//		{
		//			mergedItem.ActiveManagerChanged(activeManager);
		//			foreach(BarItem item in mergedItem.Items)
		//			{
		//				if(item is MergedParentBarItem)
		//					this.UpdateMergedItems(item as MergedParentBarItem, activeManager);
		//			}
		//		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void UpdateBars()
		{
			foreach( object entry in this.formsByTypeName.Values )
			{
				ArrayList forms = entry as ArrayList;
				if( forms == null || forms.Count == 0 )
					continue;
				Form form = forms[0] as Form;

				BarManager manager = GetManagerFrom( form );
				if( manager == null || manager.Bars == null )
					continue;
				this.UpdateBarsInManager( manager );
			}
		}
		private void UpdateAllBars( Type unregisteringType )
		{
			ArrayList unregisteringTypeForms = this.formsByTypeName[unregisteringType.FullName] as ArrayList;

			foreach( object entry in this.formsByTypeName.Values )
			{
				ArrayList forms = entry as ArrayList;
				if( forms == null )
					continue;
				if( forms == unregisteringTypeForms )
					continue;

				foreach( Form form in forms )
				{
					BarManager manager = GetManagerFrom( form );
					if( manager == null || manager.Bars == null )
						continue;
					this.UpdateBarsInManager( manager );
				}
			}
		}

		private void UpdateBarsInManager( BarManager manager )
		{
			if( null == this.commandBarManager ) return;

			foreach( Bar bar in manager.Bars )
			{
				CommandBarExt cbe = this.commandBarManager.GetCommandBarFromBar( bar );
				if( cbe != null )
					cbe.SuspendRecalc();

				this.RefreshContainer( bar );

				if( cbe != null )
					cbe.ResumeRecalc( true );
			}

			for( int i = 0; i < manager.Items.Count; i++ )
			{
				ParentBarItem parent = manager.Items[i] as ParentBarItem;
				if( parent != null )
				{
					this.RefreshContainer( parent );
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void AddCustomBar( BarUpdateInfo updateInfo )
		{
			string barName = (string)updateInfo.updateData;

			foreach( Bar bar in this.Bars )
			{
				if( bar.BarName == barName )
				{
					return;
				}
			}

			Bar newBar = new Bar( this, barName );

			bool prevValue = m_bNeedSaveCustomData;
			m_bNeedSaveCustomData = false;

			this.Bars.Add( newBar );

			m_bNeedSaveCustomData = prevValue;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void AddCustomBarInfo( BarUpdateInfo updateInfo )
		{
			Bar savedBar = updateInfo.updateData as Bar;
			Bar barToAdd = savedBar.Clone( true ) as Bar;

			if( barToAdd != null )
			{
				foreach( Bar bar in this.Bars )
				{
					if( bar.BarName == barToAdd.BarName )
					{
						return;
					}
				}

				this.Bars.Add( barToAdd );
			}
		}

		internal bool ForceSaveLoadCustomData
		{
			get { return m_bForceSaveLoadCustomData; }
		}

		internal bool NeedSaveCustomData
		{
			get { return m_bNeedSaveCustomData; }
		}

		/// <summary>
		/// Saves the user customized information.
		/// </summary>
		/// <remarks>Saves data only when <see cref="AutoPersistCustomization"/> property is set to<c>false</c>.</remarks>
		public void SaveCustomData()
		{
			SaveCustomData( AppStateSerializer.GetSingleton() );
		}

		/// <summary>
		/// Saves the user customized information to the specified persistence medium.
		/// </summary>
		/// <remarks>Saves data only when <see cref="AutoPersistCustomization"/> property is set to<c>false</c>.</remarks>
		public void SaveCustomData( AppStateSerializer serializer )
		{
			if( !this.AutoPersistCustomization )
			{
				SaveCustomDataEx( serializer );	
			}			
		}

		/// <summary>
		/// Saves the user customized information to the specified persistence medium.
		/// </summary>
		protected void SaveCustomDataEx( AppStateSerializer serializer )
		{
			m_bForceSaveLoadCustomData = true;

			if( this.ResetCustomization )
			{
				serializer.SerializeObject( this.PersistenceID + ":" + CustomizedBarSettingsLabel, null );
				serializer.SerializeObject( this.PersistenceID + ":" + BarItemsVisibilityInBarLabel, null );
				serializer.SerializeObject( this.PersistenceID + ":" + CustomBarsListLabel, null );
				serializer.SerializeObject( this.PersistenceID + ":" + CustomItemsListLabel, null );
				serializer.SerializeObject( this.PersistenceID + ":" + CustomItemsContainerListLabel, null );
			}
			else
			{
				foreach( BarUpdateInfo updateInfo in this.updateInfoList )
				{
					if( updateInfo.updateType == UpdateType.CustomBarAdded ||
						updateInfo.updateType == UpdateType.CustomBarInfoAdded )
					{
						string barName = String.Empty;
						if( updateInfo.updateType == UpdateType.CustomBarAdded )
							barName = (string)updateInfo.updateData;
						else
							barName = ( updateInfo.updateData as Bar ).BarName;

						if( !this.customAddedBarsVsNames.Contains( barName ) )
							this.customAddedBarsVsNames.Add( barName );
					}
					else if( updateInfo.updateType == UpdateType.InsertAfter ||
						updateInfo.updateType == UpdateType.InsertBefore ||
						updateInfo.updateType == UpdateType.Remove )
					{
						BarItemID id = updateInfo.source;
						if( !this.customAddedItemsVsBarItemID.Contains( id ) )
							this.customAddedItemsVsBarItemID.Add( id );

						ArrayList list = null;

						if( !this.customContainers.Contains( id ) )
						{
							list = new ArrayList();
							list.Add( updateInfo.destination );

							this.customContainers.Add( id, list );
						}
						else
						{
							list = this.customContainers[id] as ArrayList;
							if( !list.Contains( updateInfo.destination ) )
								list.Add( updateInfo.destination );
						}
					}
				}

				m_htWrapper.Clear();
				foreach( CommandBar cbar in this.commandBarManager.GetCommandBarController().CommandBars )
				{
					CommandBarExtSerializer wrapper = new CommandBarExtSerializer();
					wrapper.GetCommandBarData( cbar );

					m_htWrapper.Add( cbar.Name, wrapper );
				}

				savedUpdateInfoList.Clear();
				savedUpdateInfoList.AddRange( this.updateInfoList );

				SaveCustomizationInfo( serializer );
				SaveBarState( serializer );
			}

			m_bForceSaveLoadCustomData = false;
		}

		private Hashtable m_htWrapper = new Hashtable();
		private ArrayList savedUpdateInfoList = new ArrayList();

		/// <summary>
		/// Loads the user customized information from the specified persistence medium.
		/// </summary>
		/// <param name="serializer">The state serializer.</param>
		/// <param name="loadBarState">if set to <c>true</c>, method also load bars' state.</param>
		/// <remarks>Loads data only when <see cref="AutoPersistCustomization"/> property is set to<c>false</c>.</remarks>
		public void LoadCustomData( AppStateSerializer serializer, bool loadBarsState )
		{
			if( !this.AutoPersistCustomization )
			{
				LoadCustomDataEx( serializer, loadBarsState );
			}
		}

		/// <summary>
		/// Loads the user customized information.
		/// </summary>
		/// <remarks>Loads data only when <see cref="AutoPersistCustomization"/> property is set to<c>false</c>.</remarks>
		public void LoadCustomData()
		{
			LoadCustomData( AppStateSerializer.GetSingleton() );
		}

		/// <summary>
		/// Loads the user customized information from the specified persistence medium.
		/// </summary>
		/// <param name="serializer">The state serializer.</param>
		/// <remarks>Loads bars' state.</remarks>
		/// <remarks>Loads data only when <see cref="AutoPersistCustomization"/> property is set to<c>false</c>.</remarks>
		public void LoadCustomData( AppStateSerializer serializer )
		{
			LoadCustomData( serializer, true );
		}

		/// <summary>
		/// Loads the user customized information from the specified persistence medium.
		/// </summary>
		/// <param name="serializer">The state serializer.</param>
		/// <param name="loadBarState">if set to <c>true</c>, method also load bars' state.</param>
		protected void LoadCustomDataEx( AppStateSerializer serializer, bool loadBarsState )
		{
			bool prevSaveCustomDataValue = m_bNeedSaveCustomData;
			m_bNeedSaveCustomData = false;
			m_bForceSaveLoadCustomData = true;

			this.RevertNonSavedChanges( serializer );

			this.updateInfoList = new ArrayList();
			this.customAddedBarsVsNames = new ArrayList();
			this.customAddedItemsVsBarItemID = new ArrayList();
			this.barItemVisibilityInBars = new Hashtable();

			this.LoadCustomizationInfo( serializer );
			//this.LoadBarStateInternal( serializer, false );

			foreach( ArrayList formsList in this.formsByTypeName.Values )
			{
				foreach( Form form in formsList )
				{
					BarManager manager = GetManagerFrom( form );
					this.UpdateUserChangesIn( form );
					this.AttachCommandBarsOfForm( form, true, false );
				}
			}

			if (loadBarsState)
			{
				this.LoadBarStateInternal(serializer, false);
			}
			foreach( CommandBar cbar in this.commandBarManager.GetCommandBarController().CommandBars )
			{
				CommandBarExtSerializer wrapper = m_htWrapper[cbar.Name] as CommandBarExtSerializer;

				if( cbar.Visible && wrapper != null &&
						( cbar.nMaxLength != wrapper.nMaxLength ||
						cbar.FloatBounds.Location != wrapper.ptFloat ||
						cbar.FloatBounds.Size != wrapper.szFloat ) )
				{
					if( cbar.Floating )
					{
						cbar.rcFloat = new Rectangle( wrapper.ptFloat, wrapper.szFloat );
						CommandBarForm cbform = cbar.Parent as CommandBarForm;
						if( cbform != null && cbar.FloatBounds.Location != cbform.Location )
							cbform.Location = cbar.FloatBounds.Location;
					}

					cbar.bRedockNeeded = true;
					cbar.bRecalcNeeded = true;

					CommandBarDockState prevSate = cbar.cbDockStateT;
					cbar.cbDockStateT = cbar.cbarDockState;

					cbar.RedockIfNeeded();
					cbar.RecalcIfNeeded();

					cbar.cbDockStateT = prevSate;
				}
			}

			m_bForceSaveLoadCustomData = false;
			m_bNeedSaveCustomData = prevSaveCustomDataValue;
		}

		/// <summary>
		/// Loads the user customized information from the specified persistence medium.
		/// </summary>
		/// <param name="serializer">The state serializer.</param>
		/// <remarks>Loads bars' state.</remarks>
		protected void LoadCustomDataEx( AppStateSerializer serializer )
		{
			LoadCustomDataEx( serializer, true );
		}

		private void RevertNonSavedChanges( AppStateSerializer serializer )
		{
			bool prevValue = m_bNeedSaveCustomData;
			m_bNeedSaveCustomData = false;

			object tempList = null;
			ArrayList savedBars = new ArrayList(), savedItems = new ArrayList();
			Hashtable savedContainers = new Hashtable();

			tempList = serializer.DeserializeObject( this.PersistenceID + ":" + CustomBarsListLabel );
			if( tempList != null )
				savedBars = tempList as ArrayList;

			tempList = serializer.DeserializeObject( this.PersistenceID + ":" + CustomItemsListLabel );
			if( tempList != null )
				savedItems = tempList as ArrayList;

			tempList = serializer.DeserializeObject( this.PersistenceID + ":" + CustomItemsContainerListLabel );
			if( tempList != null )
				savedContainers = tempList as Hashtable;

			for( int i = this.updateInfoList.Count - 1; i >= 0; i-- )
			{
				BarUpdateInfo updateInfo = (BarUpdateInfo)this.updateInfoList[i];

				if( updateInfo.updateType == UpdateType.CustomBarAdded ||
					updateInfo.updateType == UpdateType.CustomBarInfoAdded )
				{
					string barName = String.Empty;
					if( updateInfo.updateType == UpdateType.CustomBarAdded )
						barName = (string)updateInfo.updateData;
					else
						barName = ( updateInfo.updateData as Bar ).BarName;

					if( !savedBars.Contains( barName ) )
					{
						for( int j = 0; j < this.Bars.Count; j++ )
						{
							if( barName == this.Bars[j].BarName )
							{
								this.Bars.RemoveAt( j );
								i++;
								break;
							}
						}
					}
				}
				else if( updateInfo.updateType == UpdateType.InsertAfter ||
					updateInfo.updateType == UpdateType.InsertBefore ||
					updateInfo.updateType == UpdateType.Remove )
				{
					ArrayList list = savedContainers[updateInfo.source] as ArrayList;
					BarItemID id = updateInfo.source;

					if( !savedItems.Contains( id ) || ( list != null && !list.Contains( updateInfo.destination ) ) )
					{
						for( int j = 0; j < this.Items.Count; j++ )
						{
							BarItem item = this.Items[j];
							BarItemID itemId = new BarItemID( item.ID, BarManager.GetFormTypeName( item.Manager ) );
							if( id == itemId )
							{
								BarManager manager = item.Manager;
								IBarItemContainer destinationParent = this.GetDestinationContainer( manager, updateInfo.destination );

								BarItem hitItem = manager.GetBarItemFromBarItemID( updateInfo.adjacentItem );
								if( hitItem == null )
									hitItem = this.SearchBarItem( updateInfo.adjacentItem );

								if( updateInfo.updateType == UpdateType.Remove )
								{
									if( destinationParent.Items.Contains( item ) )
									{
										int index = destinationParent.Items.IndexOf( item );
										BarItem nextItem = index < ( destinationParent.Items.Count - 1 ) ? destinationParent.Items[index + 1] : null;

										destinationParent.Items.Remove( item );

										if( nextItem != null && groupBeginersHash[nextItem] != null && (bool)groupBeginersHash[nextItem] )
											destinationParent.BeginGroupAt( nextItem );
									}

									int insertIndex = destinationParent.Items.IndexOf( hitItem );
									destinationParent.Items.Insert( insertIndex != -1 ? insertIndex + 1 : 0, item );
                                  
                                    if (groupBeginersHash[item] != null && (bool)groupBeginersHash[item] )
										destinationParent.BeginGroupAt( item );
								}
								else
								{
									bool needRemoveItem = true;
									foreach( BarUpdateInfo info in this.updateInfoList )
									{
										if( info.source == updateInfo.source &&
											info.updateType == UpdateType.Remove )
										{
											needRemoveItem = false;

											IBarItemContainer infoContainer = GetDestinationContainer( item.Manager, info.destination );

											if( infoContainer != destinationParent && destinationParent.Items.Contains( item ) )
											{
												int index = destinationParent.Items.IndexOf( item );
												BarItem nextItem = index < ( destinationParent.Items.Count - 1 ) ? destinationParent.Items[index + 1] : null;

												destinationParent.Items.Remove( item );

												if( nextItem != null && groupBeginersHash[nextItem] != null && (bool)groupBeginersHash[nextItem] )
													destinationParent.BeginGroupAt( nextItem );
											}

											break;
										}
									}

									if( needRemoveItem )
									{
										if( item.DesignTimeCreated )
										{
											if( destinationParent.Items.Contains( item ) )
											{
												int index = destinationParent.Items.IndexOf( item );
												BarItem nextItem = index < ( destinationParent.Items.Count - 1 ) ? destinationParent.Items[index + 1] : null;

												destinationParent.Items.Remove( item );

												if( nextItem != null && groupBeginersHash[nextItem] != null && (bool)groupBeginersHash[nextItem] )
													destinationParent.BeginGroupAt( nextItem );
											}
										}
										else
										{
											this.RemoveReferencesToBarItem( item );
											i++;
											break;
										}
									}
								}

								break;
							}
						}
					}
				}
				else if( updateInfo.updateType == UpdateType.LargeIcons ||
					updateInfo.updateType == UpdateType.UsePartialMenus )
				{
					bool founded = false;
					foreach( BarUpdateInfo info in savedUpdateInfoList )
					{
						if( ( updateInfo.updateType == UpdateType.LargeIcons && info.updateType == UpdateType.LargeIcons ) ||
							( updateInfo.updateType == UpdateType.UsePartialMenus && info.updateType == UpdateType.UsePartialMenus ) )
						{
							founded = true;

							if( updateInfo.updateType == UpdateType.LargeIcons )
							{
								this.LargeIcons = (bool)info.updateData;
							}
							else
							{
								this.UsePartialMenus = (bool)info.updateData;
							}

							break;
						}
					}

					if( !founded )
					{
						if( updateInfo.updateType == UpdateType.LargeIcons )
						{
							this.LargeIcons = !this.LargeIcons;
						}
						else
						{
							this.UsePartialMenus = !this.UsePartialMenus;
						}
					}
				}
			}

			m_bNeedSaveCustomData = prevValue;
		}

		private IBarItemContainer GetDestinationContainer( BarManager manager, object destination )// BarID or BarItemID
		{
			IBarItemContainer destinationParent = null;

			if( destination is BarID )
			{
				Bar bar = manager.GetBarFromBarID( (BarID)destination );
				if( bar == null )
				{
					foreach( BarUpdateInfo info in this.updateInfoList )
					{
						if( info.updateType == UpdateType.CustomBarInfoAdded )
						{
							Bar customBar = info.updateData as Bar;
							if( customBar.BarName == ( (BarID)destination ).containerName )
							{
								bar = customBar;
								break;
							}
						}
					}
				}

				if( bar != null )
				{
					destinationParent = bar;
				}
			}
			else if( destination is BarItemID )
			{
				BarItem dest = manager.GetBarItemFromBarItemID( (BarItemID)destination );
				destinationParent = dest as ParentBarItem;
			}

			return destinationParent;
		}

		protected override void BarListCollectionChanged( object sender, CollectionChangeEventArgs args )
		{
			base.BarListCollectionChanged( sender, args );

			if( !this.DesignMode && this.endInitCalled && m_bNeedSaveCustomData )
			{
				Bar bar = args.Element as Bar;
				string cbName = CommandBarExt.SynthesizeNameFromBar( bar );

				if( this.MdiChildrenFormName != String.Empty )
				{
					cbName += CommandBarControllerExt.DEF_SPACER + this.MdiChildrenFormName;
				}

				if( !m_bForceSaveLoadCustomData )
				{
					CommandBarController cmdBarController = this.commandBarManager.GetCommandBarController();
					CBCtrlrSerializationWrapper serWrapper = cmdBarController.wpprCBController;

					if( serWrapper != null && serWrapper.IsInitialized( cbName ) )
					{
						serWrapper.RemoveCommandBarData( cbName );
					}
				}

				if( args.Action == CollectionChangeAction.Add )
				{
					if( !this.customAddedBarsVsNames.Contains( bar.BarName ) && this.AutoSaveCustomData )
					{
						this.customAddedBarsVsNames.Add( bar.BarName );
					}

					this.RecordAddCustomBarInfo( bar );
				}
				else if( args.Action == CollectionChangeAction.Remove )
				{
					if( this.customAddedBarsVsNames.Contains( bar.BarName ) && this.AutoSaveCustomData )
					{
						this.customAddedBarsVsNames.Remove( bar.BarName );
					}

					this.RemoveCustomBar( bar );
				}
			}
		}

		/// <summary>
		/// Indicates whether to automatically load user's customization data when form is activating.
		/// </summary>
		[DefaultValue( false )]
		[Description( "Indicates whether to automatically load user's customization data when form is activating." )]
		public bool AutoLoadCustomData
		{
			get { return m_bAutoLoadCustomData; }
			set { m_bAutoLoadCustomData = value; }
		}

		/// <summary>
		/// Indicates whether to automatically save user's customization data when form is closing.
		/// </summary>
		[DefaultValue( false )]
		[Description( "Indicates whether to automatically save user's customization data when form is closing." )]
		public bool AutoSaveCustomData
		{
			get { return m_bAutoSaveCustomData; }
			set { m_bAutoSaveCustomData = value; }
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void UpdateUserChangeIn( BarManager manager, BarUpdateInfo updateInfo, ref bool keepUpdateInfo )
		{
			if( updateInfo.updateType == UpdateType.CustomBarAdded )
			{
				if( manager == this ) // Custom bars can be added only to the MainFrameBarManager
					this.AddCustomBar( updateInfo );
				return;
			}
			else if( updateInfo.updateType == UpdateType.CustomBarInfoAdded )
			{
				string barName = ( updateInfo.updateData as Bar ).BarName;
				if( manager == this && this.customAddedBarsVsNames.Contains( barName ) &&
					( this.AutoLoadCustomData || m_bForceSaveLoadCustomData ) )
					this.AddCustomBarInfo( updateInfo );
				return;
			}
			else if( updateInfo.updateType == UpdateType.CustomParentItem )
			{
				if( manager == this )// Custom menus can be added only to the MainFrameBarManager
				{
					BarItemID itemId = (BarItemID)updateInfo.updateData;
					// This was already called by a different RegisterMdiChildTypes.
					if( !this.Items.IsValidItemID( null, itemId.barItemID ) )
						return;
					CustomParentMenuItem menuItem = new CustomParentMenuItem();
					menuItem.Text = SR.GetString( SR.NewMenu );
					// Last category.
					menuItem.ID = itemId.barItemID;
					menuItem.CategoryIndex = 1000;
					menuItem.Manager = this;
				}
				return;
			}
			else if( updateInfo.updateType == UpdateType.UsePartialMenus )
			{
				if( manager == this )
					this.partialMenusMode = (bool)updateInfo.updateData;
				return;
			}
			else if( updateInfo.updateType == UpdateType.LargeIcons )
			{
				if( manager == this )
				{
					bool oldValue = this.largeIcs;
					this.largeIcs = (bool)updateInfo.updateData;
					this.OnPropertyChanged( new SyncfusionPropertyChangedEventArgs
						( PropertyChangeEffect.NeedLayout, "LargeIcons", oldValue, this.largeIcs ) );
				}
				return;
			}
			else if( updateInfo.updateType == UpdateType.InsertAfter ||
				updateInfo.updateType == UpdateType.InsertBefore ||
				updateInfo.updateType == UpdateType.Remove )
			{
				if( manager == this )
				{
					BarItem source = this.GetBarItemFromUpdateInfo( updateInfo, true );
					if( source == null || !source.DesignTimeCreated )
					{
						ArrayList list = this.customContainers[updateInfo.source] as ArrayList;

						if( ( !this.Customizing || this.CustomizationHelper.ParentItem != null ) &&
							( m_bCustomContainersLoaded &&
								!( this.customAddedItemsVsBarItemID.Contains( updateInfo.source ) &&
								( this.AutoLoadCustomData || m_bForceSaveLoadCustomData ) &&
								list != null && list.Contains( updateInfo.destination ) ) ) )
						{
							return;
						}
					}
				}
			}

			BarItem sourceItem = this.GetBarItemFromUpdateInfo( updateInfo, true );
			BarItem adjacentItem = this.GetBarItemFromUpdateInfo( updateInfo, false );

			IBarItemContainer destinationParent = null;
			BarItem destinationItem = null;
			if( updateInfo.destination is BarID )
			{
				Bar bar = manager.GetBarFromBarID( (BarID)updateInfo.destination );
				if( bar == null )
				{
					foreach( BarUpdateInfo info in this.updateInfoList )
					{
						if( info.updateType == UpdateType.CustomBarInfoAdded )
						{
							Bar customBar = info.updateData as Bar;
							if( customBar.BarName == ( (BarID)updateInfo.destination ).containerName )
							{
								bar = customBar;
								break;
							}
						}
					}
				}

				if( bar != null )
				{
					bar.LoadBarInfo( updateInfo );
					destinationParent = bar;
				}
			}
			else if( updateInfo.destination is BarItemID )
			{
				BarItem item = manager.GetBarItemFromBarItemID( (BarItemID)updateInfo.destination );
				destinationParent = item as ParentBarItem;
				if( destinationParent == null
					|| this.IsUpdateTypeForItemProperty( updateInfo.updateType ) )
				{
					destinationItem = item;
					destinationParent = null;
				}
			}

			if( destinationParent != null )
				manager.UpdateContainer( destinationParent, sourceItem, adjacentItem, updateInfo.updateType, updateInfo.updateData, ref keepUpdateInfo );
			else if( destinationItem != null )
				manager.UpdateItem( destinationItem, updateInfo.updateType, updateInfo.updateData, ref keepUpdateInfo );
		}

		private BarItem GetBarItemFromUpdateInfo( BarUpdateInfo updateInfo, bool source )
		{
			BarItem itemToFind;
			if( source )
			{
				itemToFind = this.GetBarItemFromBarItemID( updateInfo.source );
				if( itemToFind == null )
					itemToFind = this.SearchBarItem( updateInfo.source );

				if( itemToFind == null )
				{
					BarItem item = updateInfo.updateData as BarItem;
					if( item != null )
					{
						itemToFind = item.Clone() as BarItem;

						this.bUpdateUserChangeIn = true;

						this.Items.SuspendEvents();
						itemToFind.Manager = this;
						this.Items.ResumeEvents( false );

						this.bUpdateUserChangeIn = false;
					}
				}
			}
			else //adjacent Item
			{
				itemToFind = this.GetBarItemFromBarItemID( updateInfo.adjacentItem );
				if( itemToFind == null )
					itemToFind = this.SearchBarItem( updateInfo.adjacentItem );
			}

			return itemToFind;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void UpdateUserChangesIn( Form form )
		{
			if( !this.EnableCustomizing && !this.DesignMode )
				return;

			try
			{
				BarManager manager = GetManagerFrom( form );

				bool keepUpdateInfo = true;
				int count = this.updateInfoList.Count;
				for( int i = 0; i < count; i++ )
				{
					BarUpdateInfo updateInfo = (BarUpdateInfo)this.updateInfoList[i];
					keepUpdateInfo = true;

					this.UpdateUserChangeIn( manager, updateInfo, ref keepUpdateInfo );

					if( !keepUpdateInfo )
					{
						this.updateInfoList.RemoveAt( i );
						count--;
						i--;
					}
				}
			}
			catch { }
			{
				//Trace.WriteLine("Error updating default state with customizied state: " + e.Message);
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal BarItem SearchBarItem( BarItemID barItemID )
		{
			if( barItemID.barItemID == String.Empty )
				return null;

			BarItem barItem = null;

			foreach( ArrayList forms in this.formsByTypeName.Values )
			{
				Form activeForm = forms[0] as Form; // The first form is either the active form or the dummy form
				BarManager manager = GetManagerFrom( activeForm );

				if( manager != null )
				{
					barItem = manager.GetBarItemFromBarItemID( barItemID );
					if( barItem != null )
						break;
				}
			}
			return barItem;
		}
		#endregion METHODS

		#region CLONING
		//		object ICloneable.Clone() 
		//		{
		//			return this.Clone();
		//		}
		/// <summary>
		/// Creates a clone of this MainFrameBarManager instance.
		/// </summary>
		/// <returns>An object that has similar properties to this MainFrameBarManager.</returns>
		/// <remarks>
		/// Creates a new instance of MainFrameBarManager and calls the <see cref="MainFrameBarManager.CopyTo"/> method to copy over properties.
		/// </remarks>
		//		public object Clone()
		//		{
		//			// Make sure to include BeginInit/EndInit here.
		//			MainFrameBarManager	newManager = new MainFrameBarManager();
		//			this.CopyTo(newManager);
		//			return newManager;
		//		}

		public override void CopyTo( BarManager barManager )
		{
			MainFrameBarManager mainFrameBarManager = barManager as MainFrameBarManager;
			base.CopyTo( barManager );
			mainFrameBarManager.AutoLoadToolBarPositions = this.AutoLoadToolBarPositions;
			mainFrameBarManager.AutoPersistCustomization = this.AutoPersistCustomization;
			mainFrameBarManager.ThemesEnabled = this.ThemesEnabled;
		}
		#endregion

		#region Overrides
		internal override MergedParentBarItem[] GetMergedEquivalents( ParentBarItem originalParent )
		{
			ArrayList mergedEquivalents = new ArrayList();
			MergedParentBarItem[] baseMergedEquivalents = base.GetMergedEquivalents( originalParent );

			if( baseMergedEquivalents != null )
			{
				mergedEquivalents.AddRange( baseMergedEquivalents );
			}

			foreach( ChildFrameBarManager childBarMan in this.ChildManagers )
			{
				MergedParentBarItem[] childMergedEquivalents = childBarMan.GetMergedEquivalents( originalParent );

				if( childMergedEquivalents != null && childMergedEquivalents.Length > 0 )
				{
					mergedEquivalents.AddRange( childMergedEquivalents );
				}
			}

			return (MergedParentBarItem[])mergedEquivalents.ToArray( typeof( MergedParentBarItem ) );
		}
		#endregion Overrides

		/// <summary>
		/// Overloaded. Converts specified point in screen coordinates to point in
		/// hosted form client coordinates, excluding Command Bars Bounds.
		/// </summary>
		public Point PointToClient( int x, int y )
		{
			return this.PointToClient( new Point( x, y ) );
		}

		/// <summary>
		/// Converts specified point in screen coordinates to point in
		/// hosted formclient coordinates, excluding Command Bars Bounds.
		/// </summary>
		public Point PointToClient( Point p )
		{
			Point clientPoint = ( this.Form == null ) ? new Point( int.MinValue, int.MinValue ) : Form.PointToClient( p );

			if( this.commandBarManager != null )
			{
				CommandBarController controller = this.commandBarManager.GetCommandBarController();

				if( controller != null )
				{
					if( controller.CommandDockBarT != null )
					{
						clientPoint.Offset( 0, -controller.CommandDockBarT.Height );
					}

					if( controller.CommandDockBarL != null )
					{
						clientPoint.Offset( -controller.CommandDockBarL.Width, 0 );
					}
				}
			}

			return clientPoint;
		}

		private void OnAppDeactivated( object sender, EventArgs e )
		{
			if( this.commandBarManager != null )
			{
				this.commandBarManager.HintViaHotKeyPrefix = false;
                if (this.commandBarManager.IsKeyboardNavigationOn())
                    this.ProcessMenuKeyUp();
			}
		}


		/// <summary>
		/// Synchronously suspends redrawing of dock bars.
		/// </summary>
		public void SuspendDockBarsRedrawSync()
		{
			NativeMethodsHelper.SuspendRedrawWindow( this.Form.Handle );
		}

		/// <summary>
		/// Synchronously resumes redrawing of dock bars.
		/// </summary>
		public void ResumeDockBarsRedrawSync()
		{
			NativeMethodsHelper.ResumeRedrawWindow( this.Form.Handle, true );
		}

		protected override void BarItemsCollectionChanged( object sender, CollectionChangeEventArgs args )
		{
			base.BarItemsCollectionChanged( sender, args );

			BarItem item = args.Element as BarItem;

			if( args.Action != CollectionChangeAction.Refresh
				&& !this.DesignMode && this.endInitCalled && !( item is NewMenuItem ) && m_bNeedSaveCustomData )
			{
				switch( args.Action )
				{
					case CollectionChangeAction.Add:
					{
						RecordInsertEx( item, false );
						break;
					}

					case CollectionChangeAction.Remove:
					{
						BarItemID id = new BarItemID( item.ID, BarManager.GetFormTypeName( this ) );

						if( this.customAddedItemsVsBarItemID.Contains( id ) && this.AutoSaveCustomData )
							this.customAddedBarsVsNames.Remove( id );

						//if( this.Items.IndexOf( item ) != -1 )
                        if (item != null && this.helper.ParentItem != null && this.helper.ParentItem.Items != null)
						{
							this.RecordRemove( item, this.helper.ParentItem );
						}

						break;
					}
				}
			}
		}

		internal void RecordInsertEx( BarItem item, bool overrideInfo )
		{
			BarItemID id = new BarItemID( item.ID, BarManager.GetFormTypeName( item.Manager ) );
            bool insertBefore = false;

			if( !this.customAddedItemsVsBarItemID.Contains( id ) && this.AutoSaveCustomData )
			{
				this.customAddedItemsVsBarItemID.Add( id );
			}

			BarItem adjacentItem = null;

			if( this.helper.ParentItem != null && this.helper.ParentItem.Items != null )
			{
				int index = this.helper.ParentItem.Items.IndexOf( item );

				if( index < 0 )
				{
					index = this.helper.ParentItem.Items.Count;
				}

				else if( index > 0 )
				{
					adjacentItem = this.helper.ParentItem.Items[--index];
				}
                else if (index == 0 && this.helper.ParentItem.Items.Count > 1)
                {
                    insertBefore = true;
                }
			}

			if( overrideInfo )
			{
				if( helper.ParentItem != null )
				{
					BarItemID barItemID = BarItemID.Empty;
					if( item != null )
						barItemID = new BarItemID( item.ID, BarManager.GetFormTypeName( item.Manager ) );

					for( int i = 0; i < this.updateInfoList.Count; i++ )
					{
						BarUpdateInfo info = (BarUpdateInfo)this.updateInfoList[i];
						if( info.source == barItemID && info.destination == null &&
							( info.updateType == UpdateType.InsertAfter || info.updateType == UpdateType.InsertBefore ) )
						{
							object destinationID = null;
							IBarItemContainer destinationItem = helper.ParentItem;
							if( destinationItem is Bar )
							{
								Bar bar = destinationItem as Bar;
								BarID barID = new BarID( bar.BarName, BarManager.GetFormTypeName( bar.Manager ) );
								destinationID = barID;
							}
							else if( destinationItem is BarItem )
							{
								BarItem destItem = destinationItem as BarItem;
								destinationID = new BarItemID( destItem.ID, BarManager.GetFormTypeName( destItem.Manager ) );
							}

							if( adjacentItem != null )
							{
								info.adjacentItem = new BarItemID( adjacentItem.ID, BarManager.GetFormTypeName( adjacentItem.Manager ) );
							}

							BarUpdateInfo newinfo = new BarUpdateInfo( info.source, info.adjacentItem, destinationID,
								info.updateType, info.updateData );

							this.updateInfoList.RemoveAt( i );
							this.updateInfoList.Add( newinfo );

							break;
						}
					}
				}
			}
			else
			{
				this.RecordInsert( item, adjacentItem, this.helper.ParentItem, insertBefore);
			}
		}
	}

	/// <summary>
	/// The ChildFrameBarManager manages the menus and tool bars of a child window (in an MDI
	/// scenario), in the XP Menus framework. 
	/// </summary>
	/// <remarks>
	/// <para>Make sure to take a look at the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager"/> class's documentation before you start using 
	/// this derived class. </para>
	/// <para>Always associate an instance of this class with a form that will be parented to another form (making it
	/// a child Form in an MDI scenario).</para>
	/// <para>Note that in an MDI scenario, optionally you can provide the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.MainFrameBarManager"/> prior knowledge 
	/// of the all the child form types that it might parent, in order that the menus and tool bars
	/// provide a seamless interface to the user even though they are part of different child forms' BarManager.
	/// You do this via the MainFrameBarManager's <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.MainFrameBarManager.RegisterMdiChildTypes(Type[])"/> method.</para>
	/// <para>If you do not call the RegisterMdiChildTypes method, the child menus will be added/merged with the parent
	/// menus as and when the child forms are added to the mdi parent.</para>
	/// </remarks>
	/// <example>
	/// Take a look at our XPMenus samples under the Tools\Samples\Menus Package folder
	/// for usage example.
	/// </example>
	[
	TypeConverter( typeof( Syncfusion.Windows.Forms.Tools.Design.BarManagerConverter ) ),
	ToolboxItem( true ),
	ToolboxBitmap( typeof( CommandBar ), "ToolboxIcons.ChildFrameBarManager.bmp" ),
	Serializable(),
	ToolboxItemFilter( "System.Windows.Forms" ),
	Description( "Manages menus and tool bars of a child window (in an MDI scenario), in XP Menus framework." )
	]
	public class ChildFrameBarManager: BarManager, ICloneable, ISerializable
	{
		/// <summary>
		/// Overloaded. Creates a new instance of the ChildFrameBarManager class.
		/// </summary>
		/// <param name="form">The form to which this instance will be associated.</param>
		public ChildFrameBarManager( Form form )
			: base( form )
		{

		}

		/// <summary>
		/// Creates a new instance of the ChildFrameBarManager class.
		/// </summary>
		/// <param name="container">The logical container parenting this instance.</param>
		/// <param name="form">The form to which this instance will be associated.</param>
		public ChildFrameBarManager( IContainer container, Form form )
			: this( form )
		{
			if( container != null )
				container.Add( this );
		}

		/// <summary>
		/// Creates an instance of the ChildFrameBarManager and sets it's defaults.
		/// </summary>
		public ChildFrameBarManager()
			: base()
		{

		}
		#region CLONING
		object ICloneable.Clone()
		{
			return this.Clone();
		}
		/// <summary>
		/// Creates a clone of this ChildFrameBarManager instance.
		/// </summary>
		/// <returns>An object that has similar properties to this ChildFrameBarManager.</returns>
		/// <remarks>
		/// Creates a new instance of ChildFrameBarManager and calls the <see cref="BarManager.CopyTo"/> method to copy over properties.
		/// </remarks>
		public object Clone()
		{
			ChildFrameBarManager newManager = new ChildFrameBarManager();
			this.CopyTo( newManager );
			this.OnAfterClone( new BarManagerClonedEventArgs( newManager ) );
			return newManager;
		}
		#endregion
		protected ChildFrameBarManager( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{

			// No need for an Init, because Init is already overridable in the base class.
		}

		/// <summary>
		/// Gets the Object data. (overridden method)
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public override void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			base.GetObjectData( info, context );
		}

		/// <summary>
		/// Indicates whether the component should draw right-to-left for RTL languages. (overridden property)
		/// </summary>
		public override RightToLeft RightToLeft
		{
			get
			{
				RightToLeft eRTL = RightToLeft.No;
				BarManager mainMgr = this.MainFrameBarManager;

				if( null != mainMgr && this != mainMgr )
				{
					eRTL = mainMgr.RightToLeft;
				}
				else
				{
					eRTL = base.RightToLeft;
				}

				return eRTL;
			}
		}
	}

	/// <summary>
	/// Bar changed event handler delegate
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void BarChangedEventHandler( object sender, BarChangedEventArgs e );

	public class BarChangedEventArgs: EventArgs
	{
		#region Class members
		private static BarChangedEventArgs _empty = null;
		private Bar m_bar = null;
		#endregion

		#region Class properties
		/// <summary>
		/// Returns the Bar that was closed. 
		/// </summary>
		public Bar Bar
		{
			get
			{
				return m_bar;
			}
		}

		public static new BarChangedEventArgs Empty
		{
			get
			{
				return _empty;
			}
		}

		#endregion

		#region Class Initialize/Finalize methods
		static BarChangedEventArgs()
		{
			_empty = new BarChangedEventArgs();
		}

		private BarChangedEventArgs()
		{
			m_bar = null;
		}

		public BarChangedEventArgs( Bar bar )
		{
			if( bar == null )
				throw new ArgumentNullException( "bar" );

			m_bar = bar;
		}
		#endregion

	}
	/// <summary>
	/// The BarManager manages a form's menus and tool bars and lets it participate in
	/// the user-customization feature.
	/// </summary>
	/// <seealso cref="Syncfusion.Windows.Forms.Tools.XPMenus.ChildFrameBarManager"/>
	/// <seealso cref="Syncfusion.Windows.Forms.Tools.XPMenus.MainFrameBarManager"/>
	/// <remarks>
	/// <para>The BarManager should contain a reference to all the BarItems (<see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager.Items"/> property)
	/// that you intend to use in the associated form's menu structure and the tool bars.
	/// The BarItems should have a unique ID.
	/// The BarManager also has a list of tool bars (<see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager.Bars"/> property) associated with the form.
	/// There is a "Customize" verb associated with the BarManager's designer which will allow
	/// you to design your form's menu structure and tool bars visually during 
	/// design-time without writing a single line of code. Use the "Activate Menus" verbs in the
	/// VS2003 designer in case the menus/toolbars don't respond to mouse clicks in the designer.
	/// </para>
	/// <para>The BarManager also allows you to set certain global settings like enabling 
	/// user-customization, enable partial menus, enable large icon mode for tool bars, etc.</para>
	/// <para>You don't normally use this class directly. You would instead use one of the
	/// following derived classes based on your form type. Use a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.MainFrameBarManager"/>
	/// (for the main window in an MDI and
	/// SDI scenario) or a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ChildFrameBarManager"/>(for the child windows in an MDI scenario).
	/// Since the BarManager is a component derived class, you can simple drag and drop 
	/// one of the above managers on your form during design-time to bind it to your form.</para>
	/// <para>When initializing one of the derived classes in code, make sure to call <see cref="BeginInit"/>
	/// and <see cref="EndInit"/>.</para>
	/// <para>
	/// Note: A BarManager (ChildFrameBarManager or MainFrameBarManager) cannot be simultaneously used with a 
	/// <see cref="Syncfusion.Windows.Forms.Tools.CommandBarController"/>.
	/// Use the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.MainFrameBarManager.DetachedCommandBars"/> property to
	/// add generic tool bars to your form.
	/// </para>
	/// </remarks>
	/// <example>
	/// Take a look at our XPMenus samples under the Tools\Samples\Menus Package folder
	/// for usage example.
	/// </example>
	[
	ToolboxItem( false ),
	Designer(
		typeof( Syncfusion.Windows.Forms.Tools.Design.BarManagerDesigner ),
		typeof( System.ComponentModel.Design.IDesigner ) ),
	Serializable()
	]
	public class BarManager: Component, ISupportInitialize, IBarItemsRepository,
		ISerializable, IDeserializationCallback, IIgnoreWorkingArea,IVisualStyle 
	{
		#region PRIVATE_MEMBERS
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		private bool m_bCustomDrag = false;
#endif

		/// <summary>
		/// Indicates whether to show BarItem highlighted when mouse is moves over it.
		/// </summary>
		private bool m_bShowHighlightRectangle = true;
		/// <summary>
		/// Indicates whether to show shadow for images.
		/// </summary>
		private bool m_bShowDropShadow = true;
		/// <summary>
		/// Indicates whether to show shadows for Popups.
		/// </summary>
		private bool m_bShowShadow = true;
		private bool enableCustomizing = true;
		internal bool useHooksForMenus = false;
		private string formName = String.Empty;
		internal string formFullName = String.Empty;
		private Bars barList;
		private ArrayList categories;

		private ImageList m_imageList;
		private ImageListAdv m_imageListAdv;

		private ImageList m_largeImageList;
		private ImageListAdv m_largeImageListAdv;

		private ImageList m_disabledImgList = null;
		private ImageListAdv m_disabledImgListAdv = null;

		private ImageList m_disabledLargeImgList = null;
		private ImageListAdv m_disabledLargeImgListAdv = null;

		private ImageList m_highlightImgList = null;
		private ImageListAdv m_highlightImgListAdv = null;

		private ImageList m_highlightLargeImgList = null;
		private ImageListAdv m_highlightLargeImgListAdv = null;

		private ImageList m_pressedImageList = null;
		private ImageListAdv m_pressedImageListAdv = null;

		private ImageList m_pressedLargeImageList = null;
		private ImageListAdv m_pressedLargeImageListAdv = null;

		private BarItems barItems;
		private BarItem customizingItem = null;
		private BarItem selectedItem = null;
		private Form parentForm;
		protected bool expandSubMenusAfterDelay = true;
		protected BarCustomizationDialog customizationDlg;
		internal MainFrameBarManager mainBarManager = null;
		internal CustomizationDndHelper helper;
		protected Hashtable shortcuts;
		//private bool lockToolBars = false;
		protected internal CommandBarManager commandBarManager;
		internal static string SyncfusionTransientItemID = "SyncfusionTransientItem";
		/// <summary>
		/// Specifies in milliseconds, the time after which an expanded partial menu will revert back to the
		/// collapsed state.
		/// </summary>
		/// <value>Specify the time in milliseconds. Default is 5000.</value>
		public static int PartialMenusExpandedStateResetDelay = 5000;
		/*protected static Keys ShortcutMask =
			Keys.Control | Keys.Shift | Keys.Alt | Keys.Delete | Keys.F1 | Keys.F2
			| Keys.F3 | Keys.F4 | Keys.F5 | Keys.F6 | Keys.F7 | Keys.F8 | Keys.F9 | Keys.F10
			| Keys.F11 | Keys.F12 | Keys.F13 | Keys.F13 | Keys.F14 | Keys.F15 | Keys.F16 | Keys.F17 | Keys.F18 
			| Keys.F19 | Keys.F20 | Keys.F21 | Keys.F22 | Keys.F23 | Keys.F24 | Keys.Insert;*/
		internal MemoryStream barPosInfo = null;
		protected internal IBarManagerDesigner designer = null;
		private int recentlyUsedItemResetDelay = 100;
		protected bool partialMenusMode = true;
		private IntList categoriesToIgnoreInCustDialog;
		private bool showItemsInCustomizationDialog = true;
		protected bool endInitCalled = false;
		internal static Hashtable htFormsVsBarManager = new Hashtable();
		private string currentInitializingBaseFormType = String.Empty;
		// Will be used by type converters and custom serializers.
		protected bool insertContainerWhileSerializing = true;
		internal IContainer components;
		private ArrayList deserializedItems;
		private ArrayList deserializedBars;
		private VisualStyle style = VisualStyle.OfficeXP;
		private bool _uiUpdateMFCStyle = false;
		private bool bBarItemActiveFormCheckOverride = false;
		static bool s_isDevEnv = ( Application.ExecutablePath.ToLower().IndexOf( "devenv.exe" ) >= 0 );
		static bool CaretHidden = false;
        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

        /// <summary>
        /// Default font style of the control
        /// </summary>
        private static Font FONTSTYLE = default(Font);

        /// <summary>
        /// Font which stored after changed in design
        /// </summary>
        private static Font USERFONTSTYLE = default(Font);

		#endregion PRIVATE_MEMBERS
		#region INITIALIZATION
		static BarManager()
		{
#if SINGLE_DLL_BUILD
			AppStateSerializer.SetBindingInfo( "Syncfusion.Tools.Windows", typeof( BarManager ).Assembly );
#else
			AppStateSerializer.SetBindingInfo("Syncfusion.Tools.Frameworks", typeof(BarManager).Assembly);
			// To support backward compatibility
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(BarManager).FullName, typeof(BarManager).Assembly);
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(BarUpdateInfo).FullName, typeof(BarUpdateInfo).Assembly);
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(BarItemID).FullName, typeof(BarItemID).Assembly);
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(BarID).FullName, typeof(BarID).Assembly);
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(PaintStyle).FullName, typeof(PaintStyle).Assembly);
			AppStateSerializer.SetTypeBindingInfo("Syncfusion.Tools.Windows", typeof(UpdateType).FullName, typeof(UpdateType).Assembly);
#endif
			XPMenuGridFactory.InitMenus();
		}

		/// <summary>
		/// Creates an instance of the BarManager class and sets its default properties.
		/// </summary>
		public BarManager()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				new Syncfusion.Core.Licensing.LicensedComponent( typeof( BarManager ) );
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}

			this.Init();
		}

		private MenuActivationControl m_subclass;

		public BarManager( Form form )
			: this()
		{
			this.Form = form;

			SetFormProperty( true, form );
		}

		public BarManager( IContainer container, Form form )
			: this( form )
		{
			if( container != null )
			{
				container.Add( this );
			}

		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void Init()
		{
			this.components = new System.ComponentModel.Container();
			this.shortcuts = new Hashtable();
			this.barList = new Bars();
			this.barList.manager = this;
			this.barList.CollectionChanged
				+= new CollectionChangeEventHandler( this.BarListCollectionChanged );
			this.barList.ItemPropertyChanged
				+= new SyncfusionPropertyChangedEventHandler( this.BarPropertyChanged );

			this.barItems = new BarItems( this );
			this.barItems.CollectionChanged
				+= new CollectionChangeEventHandler( this.BarItemsCollectionChanged );
			this.barItems.ItemPropertyChanged
				+= new SyncfusionPropertyChangedEventHandler( this.BarItemPropertyChanged );

			this.categories = new ArrayList();
			this.helper = new CustomizationDndHelper( null );
			this.categoriesToIgnoreInCustDialog = new IntList();
            FONTSTYLE = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            USERFONTSTYLE = FONTSTYLE;
		}

		/// <summary>
		/// Begins the initialization of a <see cref="BarManager"/> that is used 
		/// on a form.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The Visual Studio .NET design environment uses this method to start 
		/// the initialization of a component that is used on a form or used by 
		/// another component. The <see cref="EndInit"/> method ends the initialization. 
		/// Using the BeginInit and EndInit methods prevents the control from 
		/// being used before it is fully initialized.
		/// </para>
		/// </remarks>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public virtual void BeginInit()
		{
		}

		/// <summary>
		/// Ends the initialization of a <see cref="BarManager"/> that is used 
		/// on a form.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The Visual Studio .NET design environment uses this method to end 
		/// the initialization of a component that is used on a form or used by 
		/// another component. The <see cref="BeginInit"/> method starts the 
		/// initialization. Using the BeginInit and EndInit methods prevents the 
		/// control from being used before it is fully initialized.
		/// </para>
		/// </remarks>
		[EditorBrowsable( EditorBrowsableState.Never )]
		public virtual void EndInit()
		{
			this.endInitCalled = true;
			if( this.DesignMode && s_isDevEnv )
			{
				if( this.Form != null )
					this.CurrentBaseFormType = BarManager.GetFormTypeName( this );
			}
			else
			{
				if( this.Form != null )
					this.Form.Layout += new LayoutEventHandler( this.Form_Layout );
			}
		}

		protected internal CustomizationDndHelper CustomizationHelper
		{
			get { return helper; }
		}

		protected internal new object GetService( Type serviceType )
		{
			return base.GetService( serviceType );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void InitCustomizationDialog()
		{
			if( this.customizationDlg == null )
			{
				this.customizationDlg = new BarCustomizationDialog( this );
			}
		}
		/// <summary>
		/// Overridden. See <see cref="System.ComponentModel.Component.Dispose(bool)"/>.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				Application.Idle -= new EventHandler( this.OnIdle );

				this.SelectedItem = null;
				if( this.customizationDlg != null )
				{
					// Don't call Close, the dlg cancels close and throws events by default on Close.
					this.customizationDlg.Dispose();
					this.customizationDlg = null;
				}

				if( this.commandBarManager != null )
				{
					this.commandBarManager.Dispose();
					this.commandBarManager = null;
				}

				if( !this.DesignMode && this.MainFrameBarManager != this
					// The MainFrameBarManager is null when opening a derived Form with a private manager in the base Form
					&& this.MainFrameBarManager != null )
				{
					this.MainFrameBarManager.CustomizingItemChanged -=
						new EventHandler( this.MainBarManager_CustomizingItemChanged );
					this.MainFrameBarManager.PropertyChanged -=
						new SyncfusionPropertyChangedEventHandler( this.MainBarManager_PropertyChanged );
				}

				if( this.barList != null )
				{
					this.barList.CollectionChanged
						-= new CollectionChangeEventHandler( this.BarListCollectionChanged );
					this.barList.ItemPropertyChanged
						-= new SyncfusionPropertyChangedEventHandler( this.BarPropertyChanged );

					for( int iBar = 0; iBar < this.barList.Count; ++iBar )
					{
						Bar bar = this.barList[iBar];

						if( null != bar )
						{
							bar.Dispose();
						}
					}

					this.barList.Dispose();
					this.barList = null;
				}

				if( this.barItems != null )
				{
					this.barItems.CollectionChanged -= new CollectionChangeEventHandler( this.BarItemsCollectionChanged );
					this.barItems.ItemPropertyChanged
						-= new SyncfusionPropertyChangedEventHandler( this.BarItemPropertyChanged );

					BarItem item = null;

					for( int i = 0; i < this.Items.Count; i++ )
					{
						item = this.Items[i];

						if( item != null )
						{
							item.Dispose();
						}
					}

					this.barItems.Dispose();
					this.barItems = null;
				}

				this.Form = null;
			}

			base.Dispose( disposing );
		}
		#endregion INITIALIZATION
		#region PROPERTIES
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		protected internal bool CustomDrag
		{
			get
			{
				return m_bCustomDrag;
			}
			set
			{
				if( value != m_bCustomDrag )
				{
					m_bCustomDrag = value;

					if( CustomDragChanged != null )
					{
						CustomDragChanged( this, EventArgs.Empty );
					}
				}
			}
		}
#endif
		/// <summary>
		/// Indicates whether to highlight BarItem when mouse moves over it.
		/// </summary>
		[DefaultValue( true ),
	   Description( "Indicates whether to highlight BarItem when mouse moves over it." )]
		public bool ShowHighlightRectangle
		{
			get
			{
				return m_bShowHighlightRectangle;
			}
			set
			{
				if( value != m_bShowHighlightRectangle )
				{
					m_bShowHighlightRectangle = value;
				}
			}
		}
		/// <summary>
		/// Indicates whether to show shadow for BarItem's images.
		/// </summary>
		[DefaultValue( true ),
	   Description( "Indicates whether to show shadow for BarItem's images." )]
		public bool ShowDropShadow
		{
			get
			{
				return m_bShowDropShadow;
			}
			set
			{
				if( value != m_bShowDropShadow )
				{
					m_bShowDropShadow = value;
				}
			}
		}
		/// <summary>
		///Metrocolor
		/// </summary>
        private Color metroColor = ColorTranslator.FromHtml("#43C9E8");
		/// <summary>
		///Gets or Sets the metrocolor
		/// </summary>
        public Color MetroColor
        {
            get 
            { 
                return metroColor; 
            }
            set 
            { 
                if(metroColor!=value)
                    metroColor = value;
                if (this.commandBarManager != null)
                {
                    CommandBarController cbcontroller = this.commandBarManager.GetCommandBarController();
                    if (cbcontroller != null)
                        cbcontroller.MetroColor = value;
                }
            }
        }
		/// <summary>
		/// Indicates whether to show shadows for Popups.
		/// </summary>
		[DefaultValue( true ),
		Description( "Indicates whether to show shadows for Popups." )]
		public bool ShowShadow
		{
			get
			{
				return m_bShowShadow;
			}
			set
			{
				m_bShowShadow = value;
				XPMenuGridFactory.ReleaseAllGrids();
			}
		}
		/// <summary>
		/// Indicates whether the <see cref="BarItem.UpdateUI"/> events for the BarItems
		/// should be fired mfc style when the mouse moves over it or before it gets shown in a dropdown menu.
		/// </summary>
		/// <value>True to fire the UpdateUI event; false otherwise. Default is false.</value>
		[Description( "Specifies whether the UpdateUI event for the BarItems should be fired in a MFC style fashion." ),
		DefaultValue( false ),
		Category( "Behavior" )
		]
		public bool UpdateUIMFCStyle
		{
			get
			{
				return this._uiUpdateMFCStyle;
			}
			set
			{
				if( this._uiUpdateMFCStyle != value )
				{
					this._uiUpdateMFCStyle = value;
					if( this._uiUpdateMFCStyle == false )
						Application.Idle -= new EventHandler( this.OnIdle );
					else
						Application.Idle += new EventHandler( this.OnIdle );
				}
			}
		}

		/// <summary>
		/// Indicates whether the BarItems in this BarManager should appear in the
		/// Customization dialog that the user invokes.
		/// </summary>
		/// <value>True to indicate they should appear; false otherwise. Default is true.</value>
		/// <remarks>
		/// If the value is true, some of the BarItems will still not appear if their 
		/// corresponding categories are excluded in the CategoriesToIgnoreInCustDialog list.
		/// If the value is false, then none of the BarItems will appear in the dialog irrespective
		/// of the settings in the CategoriesToIgnoreInCustDialog property.
		/// </remarks>
		[
		DefaultValue( true ),
		Localizable( true ),
		Description( "Specifies whether or not the BarItems in this BarManager should appear in the Customization dialog that the user invokes." ),
		Category( "Behavior" )
		]
		public bool ShowItemsInCustomizationDialog
		{
			get { return this.showItemsInCustomizationDialog; }
			set { this.showItemsInCustomizationDialog = value; }
		}

		/// <summary>
		/// Gets or sets the delay in days after which an item's recently used setting will be reset.
		/// </summary>
		/// <remarks>
		/// Specifies the delay after which an item's recently used setting will be reset.
		/// Default value is 100 days.
		/// When a user selects an item in a partial menus enabled submenu and if the item 
		/// is not a recently used item, the item will be marked as recently used (its IsRecentlyUsedItem property will 
		/// be true) for the time-interval specified by this property. After this 
		/// time-interval the property will be reset to false.
		/// </remarks>
		[
		Browsable( true ),
		DefaultValue( 100 ),
		Category( "Behavior" ),
		Localizable( true ),
		Description( "Specifies the delay in days after which an item's recently used setting will be reset." )
		]
		public virtual int PartialMenusResetDelay
		{
			get { return this.recentlyUsedItemResetDelay; }
			set { this.recentlyUsedItemResetDelay = value; }
		}

		/// <summary>
		/// Enables or disables Partial Menus mode in submenus.
		/// </summary>
		/// <remarks>
		/// Indicates whether the Partial Menus mode should be enabled or disabled globally in all the parent items 
		/// associated with this BarManager. Default is true.
		/// </remarks>
		[DefaultValue( true ),
		Category( "Behavior" ),
		Localizable( true ),
		Description( "Enables or disables Partial Menus mode in submenus." )]
		public virtual bool UsePartialMenus
		{
			get { return this.partialMenusMode; }
			set
			{
				this.partialMenusMode = value;
			}
		}
		/// <summary>
		/// Enables or disables customization of menus and toolbars during run-time.
		/// </summary>
		/// <remarks>
		/// <para>Indicates whether customization of menus and tool bars is allowed during run-time.
		/// This property is true by default.</para>
		/// <para>If this property is set to false and if the Customize method gets called, then
		/// it will not start the customization mode.</para>
		/// <para>
		/// If this property is true and the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.MainFrameBarManager.AutoPersistCustomization"/> is
		/// set to false, then users will be able to customize their menus, but the customized state
		/// will not be persisted for use across application instantiations.
		/// </para>
		/// </remarks>
		[DefaultValue( true ),
		Localizable( true ),
		Category( "Persistance" ),
		Description( "Specifies whether or not customization by the user is allowed during run-time." )
		]
		public bool EnableCustomizing
		{
			get
			{
				return this.enableCustomizing;
			}
			set
			{
				this.enableCustomizing = value;
			}
		}
		/// <summary>
		/// Enables or disables automatic expansion of the partial menus into full menus after a delay.
		/// </summary>
		/// <remarks>
		/// Indicates whether the partial menus should be expanded automatically.
		/// This is true by default.
		/// <para>If this property is false the user will have to click on the expander button to 
		/// view the full menu. If true the menu will expand automatically after 5 seconds.</para>
		/// </remarks>
		[
		DefaultValue( true ),
		Category( "Behavior" ),
		Localizable( true ),
		Description( "Enable automatic expansion of the partial menus into full menus after a delay." )
		]
		public virtual bool ExpandPartialMenusAfterDelay
		{
			get { return this.expandSubMenusAfterDelay; }
			set
			{
				if( this.expandSubMenusAfterDelay != value )
				{
					this.expandSubMenusAfterDelay = value;
				}
			}
		}
		/// <summary>
		/// Used to store the positional information of the tool bars by the designer.
		/// </summary>
		/// <remarks>
		/// <para>This property is used by the BarManager's designer to store positional information of the
		/// contained tool bars. In the case of the MainFrameBarManager, this information 
		/// will be used to initialize the corresponding tool bars during runtime and design
		/// time. In the case of the ChildFrameBarManager, this information will be used to
		/// position the tool bars only during design-time.</para>
		/// <para>The structure of the MemoryStream is internal to this library and hence do not
		/// try to set/get this property.</para>
		/// </remarks>
		[
		Browsable( false ),
		EditorBrowsable( EditorBrowsableState.Never ),
		DefaultValue( null )
		]
		public MemoryStream BarPositionInfo
		{
			get
			{
				if( this.commandBarManager == null )
					return null;
				else
					return this.commandBarManager.GetBarPositionInfo();
			}
			set
			{
				if( this.barPosInfo != null )
					this.barPosInfo.Close();

				this.barPosInfo = value;
			}
		}
		//		[
		//			Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		//		DefaultValue(null)
		//		]
		//		public virtual MemoryStream BarItemsCopy
		//		{
		//			get
		//			{
		//				MemoryStream ms = new MemoryStream();
		//				AppStateSerializer assr = new AppStateSerializer(SerializeMode.BinaryFmtStream, ms);
		//				assr.SerializeObject("BarManager", this, false);
		//				assr.PersistNow();
		////				AppStateSerializer assr = new AppStateSerializer(SerializeMode.XMLFile, "c:\\myxmlfile.xml");
		////				assr.SerializeObject("BarManager", this, false);
		////				assr.PersistNow();
		//				return ms;
		//			}
		//			set
		//			{
		//
		//			}
		//		}
		/// <summary>
		/// Gets or sets a user-friendly name that will be used to refer to a form type in the Customization dialog.
		/// </summary>
		/// <remarks>
		/// Specifies the user-friendly name that will be used to refer to a form type in 
		/// the Customization dialog. In an MDI scenario the Customization dialog categorizes the
		/// tool bars and bar items based on their BarManager/Form type. And its recommended
		/// to provide a user-friendly name for these BarManager/Form types. If this property is
		/// null or empty, then the form type will be used to mark these BarManagers/Forms.
		/// </remarks>
		[Category( "Data" ),
		Localizable( true ),
		Description( "A user-friendly name that will be used to refer to a Form type in the customization dialog." )]
		public string FormName
		{
			get
			{
				if( this.formName != String.Empty )
					return this.formName;
				else if( this.Form != null )
					return this.Form.Name;
				else
					return String.Empty;
			}
			set { this.formName = value; }
		}
		[EditorBrowsable( EditorBrowsableState.Never )]
		protected bool ShouldSerializeFormName()
		{
			if( this.Form != null && FormName == this.Form.Name )
				return false;
			else return this.formName != String.Empty;
		}
		[EditorBrowsable( EditorBrowsableState.Never )]
		protected void ResetFormName()
		{
			this.formName = String.Empty;
		}

		/// <summary>
		/// Gets the customization dialog.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude(),
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),]
		public BarCustomizationDialog CustomizationDialog
		{
			get
			{
				if( !this.DesignMode && !( this is MainFrameBarManager ) && this.MainFrameBarManager != null )
					return this.MainFrameBarManager.CustomizationDialog;

				this.InitCustomizationDialog();
				return this.customizationDlg;
			}
		}

		/// <summary>
		/// Indicates whether the customization dialog is created or not.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude(),
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),]
		public bool IsCustomizationDialogCreated
		{
			get
			{
				if( !this.DesignMode && !( this is MainFrameBarManager ) && this.MainFrameBarManager != null )
					return this.MainFrameBarManager.IsCustomizationDialogCreated;

				return customizationDlg != null;
			}
		}

		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Syncfusion.Documentation.DocumentationExclude()
		]
		protected internal new bool DesignMode
		{
			get { return base.DesignMode; }
		}
		/// <summary>
		/// Indicates whether the user is currently customizing the menus.
		/// </summary>
		/// <remarks>
		/// You will typically not have to use this property. Advanced users while extending
		/// the BarManager framework might find this property useful.
		/// </remarks>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Description( "Indicates whether the user is currently customizing the menus." )
		]
		public virtual bool Customizing
		{
			get
			{
				if( !this.DesignMode && !( this is MainFrameBarManager ) && this.MainFrameBarManager != null )
					return this.MainFrameBarManager.Customizing;

				// Can use customizationDlg since the above check will take into account the MainFrameBarManager.
				if( this.DesignMode ||
					( this.customizationDlg != null
					&& this.customizationDlg.Visible ) )
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Indicates whether the DndCustomizing is true or false. (virtual property)
		/// </summary>
		[Browsable( false ),
		Syncfusion.Documentation.DocumentationExclude(),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		]
		public virtual bool DndCustomizing
		{
			get
			{
				if( this.MainFrameBarManager != null )
					return this.MainFrameBarManager.DndCustomizing;
				else
					return false;
			}
		}


		/// <summary>
		/// Reflects the RightToLeft setting of the form this BarManager is attached to.
		/// </summary>
		/// <value>
		/// One of the <see cref="RightToLeft"/> values. The default is <see cref="RightToLeft.No"/>.
		/// </value>
		[
		Category( "Appearance" ),
		Description( "Indicates whether the menus and toolbars should draw right-to-left." )
		]
		public virtual RightToLeft RightToLeft
		{
			get
			{
				return ( null != this.parentForm ) ? this.parentForm.RightToLeft : RightToLeft.No;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool IsRTL
		{
			get
			{
				return ( RightToLeft.Yes == this.RightToLeft );
			}
		}

		/// <summary>
		/// Gets or sets the item that is currently selected by the user through the mouse or keyboard.
		/// </summary>
		[
		Browsable( false ), EditorBrowsable( EditorBrowsableState.Always ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public virtual BarItem SelectedItem
		{
			get
			{
				return this.selectedItem;
			}
			set
			{
				if( this.selectedItem != value )
				{
					this.selectedItem = value;
					this.OnSelectedItemChanged( EventArgs.Empty );
				}
			}
		}
		/// <summary>
		/// Gets or sets the item that is currently being selected or dragged by the user
		/// during customization.
		/// </summary>
		/// <remarks>
		/// <para>You will typically not have to use this property. Advanced users while extending
		/// the BarManager framework might find this property useful.</para>
		/// <para>Changing this property's value will throw the CustomizingItemChanged event.</para>
		/// </remarks>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public virtual BarItem CustomizingItem
		{
			get
			{
				MainFrameBarManager mainBarManager = this.MainFrameBarManager;
				if( mainBarManager != null && mainBarManager != this )
					return mainBarManager.CustomizingItem;
				else
					return this.customizingItem;
			}
			set
			{
				if( this.CustomizingItem != value )
				{
					MainFrameBarManager mainBarManager = this.MainFrameBarManager;
					if( mainBarManager != null && mainBarManager != this )
						mainBarManager.CustomizingItem = value;
					else
					{
						this.customizingItem = value;
						this.OnCustomizingItemChanged( EventArgs.Empty );
					}
				}
			}
		}
		/// <summary>
		/// Returns the list of bars representing the toolbars for the corresponding form.
		/// </summary>
		/// <remarks>
		/// Specifies the list of tool bars associated with this BarManager.
		/// The BarManager's designer takes care of filling this list with one entry
		/// for each tool bar.
		/// </remarks>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
		Category( "Data" ),
		Editor( typeof( CustDlgEditor ), typeof( System.Drawing.Design.UITypeEditor ) ),
		Description( "Collection of Bars, representing the toolbars for the corresponding form." ),
		]
		public Bars Bars
		{
			get { return this.barList; }
		}
		/// <summary>
		/// Returns the list of categories defined in this BarManager, under which 
		/// the BarItems will be grouped.
		/// </summary>
		/// <remarks>
		/// <para>The entries in this list are strings representing the category names.</para>
		/// <para>The CategoryID property of the BarItems in this BarManager's Items list 
		/// is an index into this Categories list. Also, the items in the CategoriesToIgnoreInCustDialog
		/// list are indices into this Categories list.</para>
		/// <para>The BarItems when added to a BarManager are identified by the framework based
		/// on their CategoryID and Text property. When these two properties combined is not 
		/// unique for a BarItem within the BarManager then an exception will be thrown during runtime.</para>
		/// </remarks>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
		Category( "Data" ),
		Editor( typeof( CustDlgEditor ), typeof( System.Drawing.Design.UITypeEditor ) ),
		Localizable( true ),
		Description( "The list of categories defined in this BarManager, under which the BarItems will be grouped." )
		]
		public ArrayList Categories
		{
			get { return this.categories; }
		}
		/// <summary>
		/// Returns the list of BarItems associated with this BarManager.
		/// </summary>
		/// <remarks>
		/// Specifies the list of BarItems associated with this BarManager.
		/// <para>
		/// Every item that needs to be part of the form's menu structure/tool bar should be
		/// included in this list. When included in this list, the BarItem's CategoryID and Text
		/// property, combined, should be unique.
		/// </para>
		/// </remarks>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
		Description( "The list of BarItems associated with this BarManager." ),
		Category( "Data" )
		]
		public BarItems Items
		{
			get { return this.barItems; }
		}

		#region ImageList

		/// <summary>
		/// Gets or sets the central repository of images to which the BarItems refer to when in small icons mode.
		/// </summary>
		/// <remarks>
		/// Specifies the ImageList where the images that will be used by the BarItems are 
		/// stored (when in small icons mode). 
		/// <para>The BarItem's ImageIndex property is usually an index into this ImageList.
		/// However, if the BarItem has its own ImageList then its ImageIndex property will be
		/// an index into that ImageList.</para>
		/// </remarks>
		[Category( "Appearance" ), Description( "The ImageList that the BarItems refer to when in small icons mode." )]
		[DefaultValue( null )]
		public ImageList ImageList
		{
			get
			{
				return m_imageList;
			}
			set
			{
				if( m_imageList != value )
				{
					m_imageList = value;

					if( m_imageList != null )
					{
						m_imageListAdv = null;
					}

					OnPropertyChanged( new SyncfusionPropertyChangedEventArgs( PropertyChangeEffect.NeedLayout, "ImageList", null, null ) );
				}
			}
		}
		/// <summary>
		/// Gets or sets the central repository of images to which the BarItems refer to when in small icons mode.
		/// </summary>
		[Category( "Appearance" ), Description( "The ImageList that the BarItems refer to when in small icons mode." )]
		[DefaultValue( null )]
		public ImageListAdv ImageListAdv
		{
			get
			{
				return m_imageListAdv;
			}
			set
			{
				if( m_imageListAdv != value )
				{
					m_imageListAdv = value;

					if( m_imageListAdv != null )
					{
						m_imageList = null;
					}

					OnPropertyChanged( new SyncfusionPropertyChangedEventArgs( PropertyChangeEffect.NeedLayout, "ImageListAdv", null, null ) );
				}
			}
		}

		#endregion

		#region LargeImageList

		/// <summary>
		/// Gets or sets the central repository of images to which the BarItems refer to when in large icons mode.
		/// </summary>
		/// <remarks>
		/// Specifies the ImageList where the images that will be used by the BarItems are 
		/// stored (when in large icons mode). 
		/// <para>The BarItem's ImageIndex property is usually an index into this ImageList.
		/// However, if the BarItem has its own ImageList then its ImageIndex property will be
		/// an index into that ImageList.</para>
		/// </remarks>
		[Category( "Appearance" ), Description( "The ImageList to which the BarItems refer to when in large icons mode." )]
		[DefaultValue( null )]
		public ImageList LargeImageList
		{
			get
			{
				return this.m_largeImageList;
			}
			set
			{
				if( this.m_largeImageList != value )
				{
					this.m_largeImageList = value;

					if( m_largeImageList != null )
					{
						m_largeImageListAdv = null;
					}
					this.OnPropertyChanged( new SyncfusionPropertyChangedEventArgs( PropertyChangeEffect.NeedLayout, "LargeImageList", null, null ) );
				}
			}
		}
		/// <summary>
		/// The ImageListAdv to which the BarItems refer to when in large icons mode.
		/// </summary>
		[Category( "Appearance" ), Description( "The ImageListAdv to which the BarItems refer to when in large icons mode." )]
		[DefaultValue( null )]
		public ImageListAdv LargeImageListAdv
		{
			get
			{
				return this.m_largeImageListAdv;
			}
			set
			{
				if( this.m_largeImageListAdv != value )
				{
					this.m_largeImageListAdv = value;

					if( m_largeImageListAdv != null )
					{
						m_largeImageList = null;
					}
					this.OnPropertyChanged( new SyncfusionPropertyChangedEventArgs( PropertyChangeEffect.NeedLayout, "LargeImageListAdv", null, null ) );
				}
			}
		}

		#endregion

		#region DisabledImageList

		/// <summary>
		/// Gets or sets the ImageList to which the BarItems refer to when disabled.
		/// </summary>
		[Category( "Appearance" ), Description( "The ImageList to which the BarItems refer to when disabled." )]
		[DefaultValue( null )]
		public virtual ImageList DisabledImageList
		{
			get
			{
				return m_disabledImgList;
			}
			set
			{
				if( value != m_disabledImgList )
				{
					m_disabledImgList = value;

					if( m_disabledImgList != null )
					{
						m_disabledImgListAdv = null;
					}
				}
			}
		}
		/// <summary>
		/// The ImageListAdv to which the BarItems refer to when disabled.
		/// </summary>
		[Category( "Appearance" ), Description( "The ImageListAdv to which the BarItems refer to when disabled." )]
		[DefaultValue( null )]
		public virtual ImageListAdv DisabledImageListAdv
		{
			get
			{
				return m_disabledImgListAdv;
			}
			set
			{
				if( value != m_disabledImgListAdv )
				{
					m_disabledImgListAdv = value;

					if( m_disabledImgListAdv != null )
					{
						m_disabledImgList = null;
					}
				}
			}
		}

		#endregion

		#region DisabledLargeImageList
		/// <summary>
		/// Gets or sets the ImageList to which the BarItems refer to when disabled and using LargeIcons mode.
		/// </summary>
		[Category( "Appearance" ), Description( "The ImageList to which the BarItems refer to when disabled, and using LargeIcons mode." )]
		[DefaultValue( null )]
		public virtual ImageList DisabledLargeImageList
		{
			get
			{
				return this.m_disabledLargeImgList;
			}
			set
			{
				if( value != m_disabledLargeImgList )
				{
					m_disabledLargeImgList = value;

					if( m_disabledLargeImgList != null )
					{
						m_disabledLargeImgListAdv = null;
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets the ImageListAdv to which the BarItems refer to when disabled and using LargeIcons mode.
		/// </summary>
		[Category( "Appearance" ), Description( "The ImageListAdv to which the BarItems refer to when disabled, and using LargeIcons mode." )]
		[DefaultValue( null )]
		public virtual ImageListAdv DisabledLargeImageListAdv
		{
			get
			{
				return this.m_disabledLargeImgListAdv;
			}
			set
			{
				if( value != m_disabledLargeImgListAdv )
				{
					m_disabledLargeImgListAdv = value;

					if( m_disabledLargeImgListAdv != null )
					{
						m_disabledLargeImgList = null;
					}
				}
			}
		}
		#endregion

		#region HighlightImageList
		/// <summary>
		/// Gets or sets the ImageList to which BarItems refer to when highlighted.
		/// </summary>
		[Category( "Appearance" ), Description( "Gets or sets the ImageList to which BarItems refer to when highlighted." )]
		[DefaultValue( null )]
		public virtual ImageList HighlightImageList
		{
			get
			{
				return m_highlightImgList;
			}
			set
			{
				if( value != m_highlightImgList )
				{
					m_highlightImgList = value;

					if( m_highlightImgList != null )
					{
						m_highlightImgListAdv = null;
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets the ImageListAdv to which BarItems refer to when highlighted.
		/// </summary>
		[Category( "Appearance" ), Description( "Gets or sets the ImageListAdv to which BarItems refer to when highlighted." )]
		[DefaultValue( null )]
		public virtual ImageListAdv HighlightImageListAdv
		{
			get
			{
				return m_highlightImgListAdv;
			}
			set
			{
				if( value != m_highlightImgListAdv )
				{
					m_highlightImgListAdv = value;

					if( m_highlightImgListAdv != null )
					{
						m_highlightImgList = null;
					}
				}
			}
		}

		#endregion

		#region HighlightLargeImageList
		/// <summary>
		/// Gets or sets the ImageList to which BarItems refer to when highlighted.
		/// </summary>
		[Category( "Appearance" ), Description( "Gets or sets the ImageList to which BarItems refer to when highlighted." )]
		[DefaultValue( null )]
		public virtual ImageList HighlightLargeImageList
		{
			get
			{
				return m_highlightLargeImgList;
			}
			set
			{
				if( value != m_highlightLargeImgList )
				{
					m_highlightLargeImgList = value;

					if( m_highlightLargeImgList != null )
					{
						m_highlightLargeImgListAdv = null;
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets the ImageListAdv to which BarItems refer to when highlighted.
		/// </summary>
		[Category( "Appearance" ), Description( "Gets or sets the ImageListAdv to which BarItems refer to when highlighted." )]
		[DefaultValue( null )]
		public virtual ImageListAdv HighlightLargeImageListAdv
		{
			get
			{
				return m_highlightLargeImgListAdv;
			}
			set
			{
				if( value != m_highlightLargeImgListAdv )
				{
					m_highlightLargeImgListAdv = value;

					if( m_highlightLargeImgListAdv != null )
					{
						m_highlightLargeImgList = null;
					}
				}
			}
		}
		#endregion

		#region PressedImageList
		/// <summary>
		/// Gets or sets the ImageList to which BarItems refer to when pressed.
		/// </summary>
		[Category( "Appearance" ), Description( "Gets or sets the ImageList to which BarItems refer to when pressed." )]
		[DefaultValue( null )]
		public virtual ImageList PressedImageList
		{
			get
			{
				return m_pressedImageList;
			}
			set
			{
				if( value != m_pressedImageList )
				{
					m_pressedImageList = value;

					if( m_pressedImageList != null )
					{
						m_pressedImageListAdv = null;
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets the ImageListAdv to which BarItems refer to when pressed.
		/// </summary>
		[Category( "Appearance" ), Description( "Gets or sets the ImageListAdv to which BarItems refer to when pressed." )]
		[DefaultValue( null )]
		public virtual ImageListAdv PressedImageListAdv
		{
			get
			{
				return m_pressedImageListAdv;
			}
			set
			{
				if( value != m_pressedImageListAdv )
				{
					m_pressedImageListAdv = value;

					if( m_pressedImageListAdv != null )
					{
						m_pressedImageList = null;
					}
				}
			}
		}
		#endregion

		#region PressedLargeImageList
		/// <summary>
		/// Gets or sets the ImageList to which BarItems refer to when pressed.
		/// </summary>
		[Category( "Appearance" ), Description( "Gets or sets the ImageList to which BarItems refer to when pressed." )]
		[DefaultValue( null )]
		public virtual ImageList PressedLargeImageList
		{
			get
			{
				return m_pressedLargeImageList;
			}
			set
			{
				if( value != m_pressedLargeImageList )
				{
					m_pressedLargeImageList = value;

					if( m_pressedLargeImageList != null )
					{
						m_pressedLargeImageListAdv = null;
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets the ImageListAdv to which BarItems refer to when pressed.
		/// </summary>
		[Category( "Appearance" ), Description( "Gets or sets the ImageListAdv to which BarItems refer to when pressed." )]
		[DefaultValue( null )]
		public virtual ImageListAdv PressedLargeImageListAdv
		{
			get
			{
				return m_pressedLargeImageListAdv;
			}
			set
			{
				if( value != m_pressedLargeImageListAdv )
				{
					m_pressedLargeImageListAdv = value;

					if( m_pressedLargeImageListAdv != null )
					{
						m_pressedLargeImageList = null;
					}
				}
			}
		}
		#endregion

		bool IBarItemsRepository.ClearItemsAfterImport
		{
			get { return true; }
		}

		/// <summary>
		/// Returns the categories that should not be shown in the Customization Dialog.
		/// </summary>
		/// <value>The IntList instance containing a list of category indices.</value>
		/// <remarks>
		/// The indices in this list refer to an item in the Categories list.
		/// <para>Note that this list will be consulted only when the ShowItemsInCustomizationDialog
		/// property is true.</para>
		/// </remarks>
		[
		Category( "Behavior" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
		Localizable( true ),
		Description( "Specifies the categories that should not be shown in the Customization Dialog." ),
		Browsable( false )
		]
		public IntList CategoriesToIgnoreInCustDialog
		{
			get { return this.categoriesToIgnoreInCustDialog; }
		}

		protected bool ShouldSerializeCategoriesToIgnoreInCustDialog
		{
			get
			{
				return this.categoriesToIgnoreInCustDialog.Count > 0;
			}
		}

		/// <summary>
		/// Enables or disables LargIcons mode for items in the tool bar.
		/// </summary>
		/// <remarks>
		/// Indicates whether the images in the tool bar should be drawn large or small.
		/// The default value is false.
		/// <para>When in small icons mode, the images are of the size specified by the BarItem's
		/// ImageList's ImageSize property. When in large icons mode, the images are of the size
		/// specified by the BarItem's LargeImageList's ImageSize property. 
		/// </para>
		/// </remarks>
		[
		DefaultValue( false ),
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Category( "Appearance" ),
		Localizable( true ),
		Description( "Enables or disables LargIcons mode for items in the tool bar." )
		]
		public virtual bool LargeIcons
		{
			get
			{
				if( this.MainFrameBarManager != null && this.MainFrameBarManager != this )
					return this.MainFrameBarManager.LargeIcons;
				else
					return false;
			}
			set
			{
				if( this.MainFrameBarManager != null && this.MainFrameBarManager != this )
					this.MainFrameBarManager.LargeIcons = value;
			}
		}
		/// <summary>
		/// Gets or sets the form to which this BarManager is associated with.
		/// </summary>
		/// <remarks>
		/// Specifies the form to which this BarManager is associated with.
		/// <para>BarManagers should be associated with a form in order for the form to be 
		/// adorned with menus and tool bars.</para>
		/// </remarks>
		[Category( "Data" ),
		Description( "The Form to which this BarManager is associated with." ),
		Browsable( false )
		]
		public virtual Form Form
		{
			get { return this.parentForm; }
			set
			{
				if( this.parentForm != value )
				{
					if( this.parentForm != null )
						this.OnDetachForm();
					this.parentForm = value;
					if( this.parentForm != null )
						this.OnAttachForm();
				}
			}
		}
		/// <summary>
		/// Returns the MainFrameBarManager if this is a ChildFrameBarManager.
		/// </summary>
		/// <remarks>
		/// Specifies the MainFrameBarManager if this is a ChildFrameBarManager.
		/// If this is a MainFrameBarManager then the property will return this.
		/// If this BarManager is not associated with an mdi child form then this property will
		/// return null.
		/// </remarks>
		[Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),]
		public MainFrameBarManager MainFrameBarManager
		{
			get
			{
				if( this.mainBarManager != null )
					return this.mainBarManager;
				else if( this is MainFrameBarManager )
					return this as MainFrameBarManager;
				else if( this.Form != null && this.Form.MdiParent != null )
					return GetManagerFrom( this.Form.MdiParent ) as MainFrameBarManager;
				else
					return null;
			}
		}

		/// <summary>
		/// Gets or sets the current base form type.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude(),
		Browsable( false )
		]
		public string CurrentBaseFormType
		{
			get
			{
				return this.currentInitializingBaseFormType;
			}
			set { this.currentInitializingBaseFormType = value; }
		}
		/// <summary>
		/// Gets or sets the visual style of the toolbars and main-menus in this <see cref="MainFrameBarManager"/>.
		/// </summary>
		/// <value>A <see cref="VisualStyle"/> value. Default is VisualStyle.OfficeXP.</value>
		/// <remarks>Note that this setting will be ignored when 
		/// <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.MainFrameBarManager.ThemesEnabled"/> is turned on and themes are 
		/// available in the OS. Also note that setting <b>VisualStyle.Default</b>
		/// will result in the same effect as <b>VisualStyle.OfficeXP</b>.</remarks>
		[
		DefaultValue( VisualStyle.OfficeXP ),
		Category( "Appearance" ),
		Description( "Specifies the visual style of the toolbars and main-menus." ),
		RefreshProperties( RefreshProperties.All )
		]
		public virtual VisualStyle Style
		{
			get
			{
				if( this.MainFrameBarManager != null && this.MainFrameBarManager != this )
					return this.MainFrameBarManager.Style;
				else
					return this.style;
			}
			set
			{
				if( this.style != value )
				{
					VisualStyle old = this.style;
					this.style = value;
					if( this.commandBarManager != null )
					{
						CommandBarController cbcontroller = this.commandBarManager.GetCommandBarController();
						if( cbcontroller != null )
							cbcontroller.Style = value;
					}
					this.OnPropertyChanged( new SyncfusionPropertyChangedEventArgs( PropertyChangeEffect.NeedLayout, "Style", old, this.style ) );
				}
			}
		}
        public bool isScaling = false;

        bool _touchMode = false;
        /// <summary>
        /// Gets or sets value to enable or disable the Touchmode to the controls.
        /// </summary>
        /// <remarks>Scale factor will be updated automatically if scalefactor is equal to 1</remarks>
        [Browsable(true),DefaultValue(false),
        Category("Layout"), Description("Gets or sets value to enable or disable the Touchmode to the controls."),
    ]
        public bool EnableTouchMode
        {
            get
            {

                    return _touchMode;
            }
            set
            {
                if (_touchMode != value)
                {
                    _touchMode = value;
                    if (_touchMode)
                        ApplyScaleToControl(1.5F);
                    else
                        ApplyScaleToControl(1);
                }
            }
        }
        private bool ShouldSerializeEnableTouchMode()
        {
            return EnableTouchMode != false;
        }

        /// <summary></summary>
        private void ResetEnableTouchMode()
        {
            EnableTouchMode = false;
        }

        /// <summary>
        /// Scale the control based on the scale factor passed in the argument.
        /// </summary>
        /// <param name="scaleFactor">value to scale the factor based upon.</param>
        public void ApplyScaleToControl(float scaleFactor)
        {
            isScaling = true;
            if (this.MainFrameBarManager != null)
            {
                if (FONTSTYLE.Name == USERFONTSTYLE.Name)
                    this.MainFrameBarManager.Font = new Font(FONTSTYLE.FontFamily, FONTSTYLE.Size * scaleFactor);
                else
                    this.MainFrameBarManager.Font = new Font(USERFONTSTYLE.FontFamily, FONTSTYLE.Size * scaleFactor);
            }
            isScaling = false;
        }

        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string vStyle;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return vStyle;
            }
            set
            {
                vStyle = value;

                if (value == "Office2007Blue")
                    Office2007Theme = Office2007Theme.Blue;
                else if (value == "Office2007Silver")
                    Office2007Theme = Office2007Theme.Silver;
                else if (value == "Office2007Black")
                    Office2007Theme = Office2007Theme.Black;
                if (value == "Office2010Blue")
                    Office2010Theme = Office2010Theme.Blue;
                else if (value == "Office2010Silver")
                    Office2010Theme = Office2010Theme.Silver;
                else if (value == "Office2010Black")
                    Office2010Theme = Office2010Theme.Black;
                else if (value == "Managed")
                {
                    if (this.Style == VisualStyle.Office2010)
                        Office2010Theme = Office2010Theme.Managed;
                    else
                        Office2007Theme = Office2007Theme.Managed;
                }
            }
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

				if( commandBarManager != null )
				{
					theme = commandBarManager.Office2007Theme;
				}

				return theme;
			}
			set
			{
				if( commandBarManager != null )
				{
					commandBarManager.Office2007Theme = value;
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

                if (commandBarManager != null)
                {
                    theme = commandBarManager.Office2010Theme;
                }

                return theme;
            }
            set
            {
                if (commandBarManager != null)
                {
                    commandBarManager.Office2010Theme = value;
                }
            }
        }
		/// <summary>
		/// Indicates whether the BarItems should check for ActiveForm before displaying tooltip. Workaround for using from MFC applications.
		/// </summary>
		[Description( "Specifies whether the BarItems should check for ActiveForm before displaying tooltip." ),
		DefaultValue( false ),
		Category( "Behavior" )
		]
		public bool BarItemActiveFormCheckOverride
		{
			get
			{
				return this.bBarItemActiveFormCheckOverride;
			}
			set
			{
				if( this.bBarItemActiveFormCheckOverride != value )
				{
					this.bBarItemActiveFormCheckOverride = value;
				}
			}
		}
		#endregion PROPERTIES
		#region INTERNAL
		private void OnIdle( object sender, EventArgs e )
		{
			if( commandBarManager == null ) return;

			// Call UpdateUI on all the BarItems in the visible Bars in Application.Idle.
			foreach( Bar bar in this.Bars )
			{
				if( this.commandBarManager.IsBarVisible( bar ) )
				{
					foreach( BarItem item in bar.Items )
					{
						if( item.Visible )
							item.PerformUpdateUI();
					}
				}
			}
		}
		[Browsable( false )]
		internal void SetUseHooksForMenus( bool useHooks )
		{
			this.useHooksForMenus = useHooks;
		}

		/// <summary>
		/// Initializes from designer.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void InitFromDesigner()
		{
			this.SetUseHooksForMenus( true );
			this.CustomizationDialog.DesignerHost = this.GetService( typeof( IDesignerHost ) ) as IDesignerHost;
		}

		protected virtual void BarListCollectionChanged( object sender, CollectionChangeEventArgs args )
		{
			this.SetDirtyOnDesigner();
		}

		protected virtual void BarItemsCollectionChanged( object sender, CollectionChangeEventArgs args )
		{
			BarItem barItem = args.Element as BarItem;

			if( null != barItem )
			{
				if( args.Action == CollectionChangeAction.Add )
				{
					if( barItem.Shortcut != Shortcut.None )
					{
						this.shortcuts[(int)barItem.Shortcut] = barItem;
					}

					IIgnoreWorkingArea iwa = barItem as IIgnoreWorkingArea;

					if( iwa != null )
					{
						iwa.IgnoreWorkingArea = this.IgnoreWorkingArea;
					}

					barItem.Manager = this;
				}
				else if( args.Action == CollectionChangeAction.Remove )
				{
					if( barItem.Shortcut != Shortcut.None )
					{
						this.shortcuts.Remove( (int)barItem.Shortcut );
					}
					barItem.Manager = null;
				}
			}
		}

		private void BarPropertyChanged( object sender, SyncfusionPropertyChangedEventArgs e )
		{
			this.SetDirtyOnDesigner();
		}

		private void BarItemPropertyChanged( object sender, SyncfusionPropertyChangedEventArgs e )
		{
			if( e.PropertyName == "Shortcut" )
			{
				//this.BarItemsCollectionChanged(this.barItems, new CollectionChangeEventArgs(CollectionChangeAction.Remove, sender));
				//this.BarItemsCollectionChanged(this.barItems, new CollectionChangeEventArgs(CollectionChangeAction.Add, sender));
				if( (Shortcut)e.OldValue != Shortcut.None )
				{
					this.shortcuts.Remove( (int)e.OldValue );
				}
				if( (Shortcut)e.NewValue != Shortcut.None )
				{
					this.shortcuts[(int)e.NewValue] = sender;
				}
			}
			this.SetDirtyOnDesigner();
		}
		private void SetDirtyOnDesigner()
		{
			if( this.DesignMode && this.Designer != null )
			{
				this.Designer.SetDirty();
			}
		}
		internal IBarManagerDesigner Designer
		{
			get
			{
				IDesignerHost host = this.GetService( typeof( IDesignerHost ) ) as IDesignerHost;
				if( host != null )
					return host.GetDesigner( this ) as IBarManagerDesigner;

				return null;
			}
		}

		/// <summary>
		/// Returns the bar associated with the bar ID.
		/// </summary>
		/// <param name="barID"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		[EditorBrowsable( EditorBrowsableState.Never )]
		public Bar GetBarFromBarID( BarID barID )
		{
			if( barID.formTypeName != BarManager.GetFormTypeName( this ) )
				return null;

			foreach( Bar bar in this.Bars )
			{
				if( bar.BarName == barID.containerName )
					return bar;
			}

			return null;
		}

		/// <summary>
		/// Returns the Bar from the Bar name.
		/// </summary>
		/// <param name="barName">Name of the bar</param>
		/// <returns>Bar object</returns>
		public Bar GetBarFromBarName( string barName )
		{
			foreach( Bar bar in this.Bars )
			{
				if( bar.BarName == barName )
					return bar;
			}
			return null;
		}

		/// <summary>
		/// Returns the bar associated with the bar item ID.
		/// </summary>
		/// <param name="barItemID"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		[EditorBrowsable( EditorBrowsableState.Never )]
		public BarItem GetBarItemFromBarItemID( BarItemID barItemID )
		{
			if( barItemID.formTypeName != BarManager.GetFormTypeName( this ) )
				return null;

			return this.Items.FindItem( barItemID.barItemID );
		}

		internal static string GetFormTypeName( Form form )
		{
			return BarManager.GetFormTypeName( BarManager.GetManagerFromForm( form ) );
		}
		internal static string GetFormTypeName( BarManager manager )
		{
			if( manager != null )
				return manager.formFullName;
			else
				return String.Empty;
		}
		internal static Form CreateDummyForm( Form form )
		{
			BarManager src = BarManager.GetManagerFromForm( form );
			BarManager cloned = null;
			cloned = ( (ChildFrameBarManager)src ).Clone() as ChildFrameBarManager;

			DummyForm df = new DummyForm( cloned );
            cloned.formFullName = form.GetType().FullName;
			return df;
		}
		/// <overload>
		/// Returns the merged equivalent, if any, of a <see cref="Bar"/> or <see cref="ParentBarItem"/> when in MDI mode.
		/// </overload>
		/// <summary>
		/// Returns the merged ParentBarItem that has replaced the original in the menu structure.
		/// </summary>
		/// <param name="originalParent">The original <see cref="ParentBarItem"/>.</param>
		/// <param name="defaultReturnValue">The default return value if a merged ParentBarItem was not found.</param>
		/// <returns>A ParentBarItem if a merged equivalent was found; defaultReturnValue otherwise.</returns>
		/// <remarks>
		/// <para>Often in an MDI scenario, ParentBarItems created during design-time in the MDI Parent
		/// and MDI children are merged together (if satisfying the merge criteria)
		/// into a new ParentBarItem which will replace the original
		/// in the menu structure.</para>
		/// <para>Once replaced changes made to the original ParentBarItem, like adding a new
		/// <see cref="BarItem"/> will not be reflected in the merged one. So, you should access
		/// the merged item directly to add child BarItems, for example. This method lets you do the same.</para>
		/// </remarks>
		/// <genoverload/>
		public ParentBarItem GetMergedEquivalent( ParentBarItem originalParent, ParentBarItem defaultReturnValue )
		{
			MainFrameBarManager mainBarMgr = this.MainFrameBarManager;
			ParentBarItem mergedItem = defaultReturnValue;

			if( mainBarMgr != this && mainBarMgr != null )
			{
				mergedItem = mainBarMgr.GetMergedEquivalent( originalParent, defaultReturnValue );
			}
			else
			{
				string sOrigText = originalParent.Text;

				if( sOrigText != String.Empty )
				{
					mergedItem = this.Items.FindItem( sOrigText, 1001 ) as ParentBarItem;
				}
			}

			return mergedItem != null ? mergedItem : defaultReturnValue;
		}

		internal virtual MergedParentBarItem[] GetMergedEquivalents( ParentBarItem originalParent )
		{
			MergedParentBarItem[] mergedParentBarItems = null;
			string sOrigText = originalParent.Text;

			if( sOrigText != String.Empty )
			{
				Hashtable mergedEquivalents = new Hashtable();
				BarItem mergedItem = null;

				foreach( Bar bar in this.Bars )
				{
					mergedItem = bar.Items.FindItem( sOrigText, 1001 );

					if( null != mergedItem && !mergedEquivalents.ContainsKey( mergedItem ) )
					{
						mergedEquivalents.Add( mergedItem, mergedItem );
					}
				}

				mergedItem = this.Items.FindItem( sOrigText, 1001 );

				if( null != mergedItem && !mergedEquivalents.ContainsKey( mergedItem ) )
				{
					mergedEquivalents.Add( mergedItem, mergedItem );
				}

				mergedParentBarItems = new MergedParentBarItem[mergedEquivalents.Count];
				mergedEquivalents.Values.CopyTo( mergedParentBarItems, 0 );
			}

			return mergedParentBarItems;
		}

		/// <summary>
		/// Returns the merged bar that has replaced the original in the menu structure.
		/// </summary>
		/// <param name="originalBar">The original <see cref="Bar"/>.</param>
		/// <param name="defaultReturnValue">The default return value if a merged Bar was not found.</param>
		/// <returns>A bar if a merged equivalent was found; defaultReturnValue otherwise.</returns>
		/// <remarks>
		/// <para>Often in an MDI scenario, bars created during design-time in the MDI Parent
		/// and MDI children are merged together (if satisfying the merge criteria)
		/// into a new bar which will replace the original
		/// in the menu structure.</para>
		/// <para>Once replaced changes made to the original bar, like adding a new
		/// <see cref="BarItem"/> will not be reflected in the merged one. So, you should access
		/// the merged bar directly to add child BarItems, for example. This method lets you do the same.</para>
		/// </remarks>
		/// <genoverload/>
		public Bar GetMergedEquivalent( Bar originalBar, Bar defaultReturnValue )
		{
			MainFrameBarManager main = this.MainFrameBarManager;
			if( main != this && main != null )
				return main.GetMergedEquivalent( originalBar, defaultReturnValue );

			Bar mergedBar = this.GetBarFromBarID( new BarID( originalBar.BarName, BarManager.GetFormTypeName( this ) ) );

			if( mergedBar == null )
				return defaultReturnValue;
			else
				return mergedBar;
		}

		/// <summary>
		/// Updates the container
		/// </summary>
		/// <param name="parentItem">Parent control </param>
		/// <param name="source">Source control</param>
		/// <param name="adjacentItem">Adjacent bar item</param>
		/// <param name="updateType"> Update type</param>
		/// <param name="updateData">Update data of type object</param>
		/// <param name="keepUpdateInfo">true if update info need to be kept</param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void UpdateContainer( IBarItemContainer parentItem, BarItem source,
			BarItem adjacentItem, UpdateType updateType, object updateData, ref bool keepUpdateInfo )
		{
			if( source == null )
			{
				// Not removing the UpdateInfo, since it could involve an unregistered Form, that could get registered later.
				//keepUpdateInfo = false;
				return;
			}

			helper.ParentItem = parentItem;
			switch( updateType )
			{
				case UpdateType.InsertAfter:
				helper.InsertItem( adjacentItem, source, false, false );
				break;
				case UpdateType.InsertBefore:
				helper.InsertItem( adjacentItem, source, true, false );
				break;
				case UpdateType.Remove:
				helper.RemoveItem( adjacentItem, source, false );
				break;
				case UpdateType.ModifiedGrouping:
				bool beginGroup = (bool)updateData;
				if( beginGroup )
					parentItem.BeginGroupAt( source );
				else
					parentItem.RemoveGroupAt( source );
				break;
			}
			helper.ParentItem = null;
		}

		internal bool IsUpdateTypeForItemProperty( UpdateType updateType )
		{
			// Based on the "Switch" in the below UpdateItem method.
			switch( updateType )
			{
				case UpdateType.ModifiedPaintStyle:
				case UpdateType.ModifiedText:
				case UpdateType.RecentlyUsedItemClicked:
				return true;
			}
			return false;
		}

		/// <summary>
		/// Removes all references to <see cref="BarItems"/> that are not parented by this 
		/// <see cref="BarManager"/> and are in the bars and ParentBarItems
		/// of this BarManager.
		/// </summary>
		public virtual void RemoveReferencesToForeignItems()
		{
			for( int i = this.Bars.Count - 1; i >= 0; i-- )
			{
				Bar bar = this.Bars[i];
				BarItems barItems = bar.Items;

				if( null != barItems )
				{
					barItems.SuspendEvents();

					for( int j = barItems.Count - 1; j >= 0; j-- )
					{
						BarItem item = barItems[j];

						if( item.Manager != this )
						{
							barItems.RemoveAt( j );
						}
					}

					barItems.ResumeEvents( true );
				}
			}

			for( int i = this.Items.Count - 1; i >= 0; i-- )
			{
				BarItem pitem = this.Items[i];

				if( pitem is ParentBarItem )
				{
					ParentBarItem parentItem = pitem as ParentBarItem;
					BarItems barItems = parentItem.Items;

					if( null != barItems )
					{
						barItems.SuspendEvents();

						for( int j = barItems.Count - 1; j >= 0; j-- )
						{
							BarItem item = barItems[j];

							if( item.Manager != this )
							{
								barItems.RemoveAt( j );
							}
						}

						barItems.ResumeEvents( true );
					}
				}
			}
		}

		/// <summary>
		/// Removes all the references for the bar item specified.
		/// </summary>
		/// <param name="item"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual void RemoveReferencesToBarItem( BarItem item )
		{
			this.Items.Remove( item );
			// Remove any references to this item elsewhere
			foreach( Bar bar in this.Bars )
			{
				bar.RemoveItem( item );
			}
			foreach( BarItem barItem in this.Items )
			{
				ParentBarItem parentItem = barItem as ParentBarItem;
				if( parentItem != null )
				{
					parentItem.RemoveItem( item );
				}
			}
		}

		/// <summary>
		/// Updates the bar items.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="updateType"></param>
		/// <param name="updateData"></param>
		/// <param name="keepUpdateInfo"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public void UpdateItem( BarItem item, UpdateType updateType, object updateData
			, ref bool keepUpdateInfo )
		{
			// Update the above IsUpdateTypeForItemProperty when this switch gets modified.
			switch( updateType )
			{
				case UpdateType.ModifiedPaintStyle:
				item.PaintStyle = (PaintStyle)updateData;
				break;
				case UpdateType.ModifiedText:
				item.Text = (string)updateData;
				break;
				case UpdateType.RecentlyUsedItemClicked:
				if( item.IsRecentlyUsedItem || ( (DateTime)updateData ) < DateTime.Now - TimeSpan.FromDays( this.MainFrameBarManager.PartialMenusResetDelay ) )
					keepUpdateInfo = false;
				else
					item.IsRecentlyUsedItem = true;
				break;
                case UpdateType.ChangedImage:
                    item.Image = updateData as ImageExt;
                    break;
			}
		}

		private Hashtable replacedBarsByMerge = new Hashtable();

		internal void ReplaceBarsWithMergedBar( Bar mergedBar )
		{
			if( this.Bars == null )
				return;

			int curIndex = -1;
			foreach( Bar bar in this.Bars )
			{
				if( bar.BarName == mergedBar.BarName )
				{
					this.replacedBarsByMerge[mergedBar] = bar;
					curIndex = this.Bars.IndexOf( bar );
					this.Bars.Remove( bar );
					break;
				}
			}
			if( this is MainFrameBarManager )
			{
				if( curIndex != -1 )
					this.Bars.Insert( curIndex, mergedBar );
				else
					this.Bars.Add( mergedBar );
			}
		}
		internal void RestoreOriginalBar( Bar mergedBar )
		{
			if( this.Bars == null )
				return;

			Bar original = this.replacedBarsByMerge[mergedBar] as Bar;
			if( original != null )
			{
				int curIndex = this.Bars.IndexOf( mergedBar );
				this.Bars.Remove( mergedBar );
				if( curIndex != -1 )
					this.Bars.Insert( curIndex, original );
				else
					this.Bars.Add( original );
			}
			else
				this.Bars.Remove( mergedBar );

			this.replacedBarsByMerge.Remove( mergedBar );
		}
		internal void ReplaceBars( Bar oldBar, Bar newBar )
		{
			if( this.Bars == null )
				return;
			//this.Bars.SuspendEvents();
			int curIndex = this.Bars.IndexOf( oldBar );
			if( curIndex != -1 )
			{
				this.Bars.RemoveAt( curIndex );
				this.Bars.Insert( curIndex, newBar );
			}
			else
				this.Bars.Add( newBar );
			//this.Bars.ResumeEvents(false);
		}
		/// <summary>
		/// Called when the attached Form is being removed.
		/// </summary>
		protected virtual void OnDetachForm()
		{
			this.CleanupOnFormClose();

			UnadviseForm();

			if( this.commandBarManager != null )
			{
				this.commandBarManager.Dispose();
				this.commandBarManager = null;
			}

			if( this.Form.ContextMenu is ContextMenuPlaceHolder )
			{
				ContextMenuPlaceHolder cm = this.Form.ContextMenu as ContextMenuPlaceHolder;
				cm.MainMenuForm = null;
			}

			if( null != m_subclass )
			{
				m_subclass.ReleaseHandleCustom();

				m_subclass.AppDeactivate -= new MenuActivationControl.EventHandler( MenuActivationControl_AppDeactivate );
				m_subclass.AppActivate -= new MenuActivationControl.EventHandler( MenuActivationControl_AppActivate );

				m_subclass = null;
			}
		}

		protected void UnadviseForm()
		{
			if( Form != null )
			{
				this.Form.Deactivate -= new EventHandler( Form_Deactivate );
				this.Form.Load -= new EventHandler( this.FormLoaded );
				this.Form.Closed -= new EventHandler( this.FormClosed );
				this.Form.Disposed -= new EventHandler( this.FormDisposed );
				this.Form.RightToLeftChanged -= new EventHandler( this.FormRightToLeftChanged );
				this.Form.Layout -= new LayoutEventHandler( this.Form_Layout );
			}
		}

		/// <summary>
		/// Called when a form is set using the Form property.
		/// </summary>
		protected virtual void OnAttachForm()
		{
			//			if(!this.DesignMode)
			//			{
			//				PropertyDescriptor propDescriptor = (PropertyDescriptor)TypeDescriptor.GetProperties((object)this.Form)["BarManager"];
			//				if(propDescriptor == null)
			//					throw new ArgumentException("A \"BarManager\" property of type BarManager could not be found in the attached Form. Make sure to implement such a property in your Form.");
			//			}

			BarManager.htFormsVsBarManager[this.Form] = this;

			this.Form.Deactivate += new EventHandler( Form_Deactivate );
			this.Form.Load += new EventHandler( this.FormLoaded );
			this.Form.Closed += new EventHandler( this.FormClosed );
			this.Form.Disposed += new EventHandler( this.FormDisposed );
			this.Form.RightToLeftChanged += new EventHandler( this.FormRightToLeftChanged );
			this.formFullName = this.Form.GetType().FullName;
			if( this.commandBarManager == null )
				this.CreateCommandBarManager();

			// Make sure to do this only in runtime. Otherwise VS.Net chokes when using Enterprise Template Projects.
			if( !( this.DesignMode || s_isDevEnv ) )
			{
				// Init ContextMenuPlaceHolder to listen for shortcut command keys.
				ContextMenuPlaceHolder cm = this.Form.ContextMenu as ContextMenuPlaceHolder;
				if( cm == null )
					cm = new ContextMenuPlaceHolder();

				cm.MainMenuForm = this.Form;
			}

			if( null == m_subclass )
			{
				m_subclass = new MenuActivationControl();
			}

			m_subclass.AppDeactivate += new MenuActivationControl.EventHandler( MenuActivationControl_AppDeactivate );
			m_subclass.AppActivate += new MenuActivationControl.EventHandler( MenuActivationControl_AppActivate );
			m_subclass.AssignHandleCustom( this.Form );
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void CreateCommandBarManager()
		{
			if( this.DesignMode && this.Form != null )
				this.commandBarManager = new CommandBarManager( this.Form, this );
		}

		/// <summary>
		/// Returns the CommandBarManager.
		/// </summary>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public CommandBarManager GetCommandBarManager()
		{
			if( this.commandBarManager == null )
			{
				if( this.DesignMode || this.MainFrameBarManager == this )
					this.CreateCommandBarManager();
				else if( this.MainFrameBarManager != null )
					return this.MainFrameBarManager.GetCommandBarManager();
			}

			return this.commandBarManager;
		}
		/// <summary>
		/// Returns the <b>CommandBar</b> associated with the <b>Bar</b> object.
		/// </summary>
		/// <seealso cref="BarControlBindingChanged"/>
		/// <param name="bar">The <see cref="Bar"/> instance whose associated CommandBar is needed.</param>
		/// <returns>The <see cref="CommandBar"/> that the bar is currently bound to. Can be null.</returns>
		/// <remarks>
		/// <para>For an MDI app, this binding is very transient as the bar gets bound and unbound frequently
		/// as the active child form changes. Hence do not cache the CommandBar returned by this method.</para>
		/// <para>A <b>far</b> is usually bound to a <b>CommandBar</b> whenever the <b>form</b> containing the corresponding 
		/// <b>BarManager</b> is active. The method will return null if the <b>bar</b> is not bound to a 
		/// <b>CommandBar</b> at the time this method is called.</para>
		/// <para>
		/// As an alternative, consider listening to the <see cref="BarControlBindingChanged"/>
		/// event which occurs whenever the <b>bar</b> is bound/unbound to a <see cref="CommandBar"/>.
		/// </para>
		/// </remarks>
		public CommandBar GetBarControl( Bar bar )
		{
			if( this.commandBarManager != null )
			{
				return this.commandBarManager.GetCommandBarFromBar( bar );
			}
			return null;
		}

		protected bool m_DesignerLoaded = false;

		/// <summary>
		/// Refreshes the command bars after the designer has loaded.
		/// </summary>
		/// <param name="saveCommandBarState"></param>
		[Syncfusion.Documentation.DocumentationExclude(),
		EditorBrowsable( EditorBrowsableState.Never )]
		public virtual void RefreshCommandBarsAfterDesignerLoad( bool saveCommandBarState )
		{
			// Called by the designer to create the command bars afresh.
			// This is necessary to allow using bar managers in base Forms.
			// The derived form's design-time requires the controls to be created
			// in the "context" of the designer.

			// This will save the current state in barPostionInfo.
			// The bars will then be restored in LoadDesignerState

			if( !m_DesignerLoaded )
			{
				if( saveCommandBarState )
					this.barPosInfo = this.BarPositionInfo;

				// Clear any existing command bars
				this.commandBarManager.RemoveBars( this, saveCommandBarState );

				// Add them again
				this.AttachCommandBarsOfForm( this.Form, true, false );

				this.commandBarManager.GetCommandBarController().ResetDockBarZOrder();

				m_DesignerLoaded = true;
			}
		}

		protected bool m_bXPMenuActive = true;

		internal bool XPMenuActive
		{
			get
			{
				return m_bXPMenuActive;
			}
			set
			{
				if( m_bXPMenuActive != value )
				{
					m_bXPMenuActive = value;

					if( null != this.commandBarManager )
					{
						CommandBarController control = this.commandBarManager.GetCommandBarController();

						foreach( CommandBar cmdBar in control.CommandBars )
						{
							CommandBarExt barExt = cmdBar as CommandBarExt;

							if( null != barExt )
							{
								if( barExt.BarControl != null )
									barExt.BarControl.XPMenuActive = value;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Attaches the command bars of the form
		/// </summary>
		/// <param name="form"></param>
		/// <param name="show"></param>
		/// <param name="hideImmediately"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		[EditorBrowsable( EditorBrowsableState.Never )]
		public virtual void AttachCommandBarsOfForm( Form form, bool show, bool hideImmediately )
		{
			BarManager manager = GetManagerFrom( form );

			this.commandBarManager.AttachBars( manager );
			if( show )
				this.commandBarManager.ShowBars( manager );
			else
				this.commandBarManager.HideBars( manager, hideImmediately );
		}

		private void MainBarManager_CustomizingItemChanged( object sender, EventArgs args )
		{
			this.OnCustomizingItemChanged( args );
		}
		private void MainBarManager_PropertyChanged( object sender, SyncfusionPropertyChangedEventArgs args )
		{
			this.OnPropertyChanged( args );
		}

		/// <summary>
		/// Gets called when the form is loaded. (virtual method)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		[EditorBrowsable( EditorBrowsableState.Never )]
		public virtual void FormLoaded( object sender, EventArgs e )
		{
			if( this.Form == null )
				return;

			if( !this.DesignMode && this.MainFrameBarManager != this
				// The MainFrameBarManager is null when opening a derived Form with a private manager in the base Form
				&& this.MainFrameBarManager != null )
			{
				this.MainFrameBarManager.CustomizingItemChanged +=
					new EventHandler( this.MainBarManager_CustomizingItemChanged );
				this.MainFrameBarManager.PropertyChanged +=
					new SyncfusionPropertyChangedEventHandler( this.MainBarManager_PropertyChanged );
			}

			// Calling CreateCommandBarManager again to make sure the command bar manager is created if in design mode.
			// Noticed that the DesignMode gets set very late when opening a derived Form in the designer
			// with the base Form containing the BarManager.
			if( this.commandBarManager == null )
				this.CreateCommandBarManager();

			if( this.commandBarManager != null )
				this.commandBarManager.ShowMainMenusAutomatically = this.DesignMode | !this.Form.IsMdiContainer;
		}

		protected bool m_bFormClosed = false;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void FormClosed( object sender, EventArgs e )
		{
			if( this.Form == null )
				return;

			MainFrameBarManager mainManager = this.MainFrameBarManager;
			if( mainManager != null )
			{
				if( mainManager != this )
				{
					mainManager.CustomizingItemChanged -=
						new EventHandler( this.MainBarManager_CustomizingItemChanged );
					mainManager.PropertyChanged -=
						new SyncfusionPropertyChangedEventHandler( this.MainBarManager_PropertyChanged );
				}

				mainManager.OnFormClosed( sender as Form );
			}

			m_bFormClosed = true;
		}
		protected void Form_Layout( object sender, LayoutEventArgs levent )
		{
			this.Form.Layout -= new LayoutEventHandler( this.Form_Layout );
			// To ensure that the command bars stay on top.
			if( this.commandBarManager != null )
				this.commandBarManager.GetCommandBarController().ResetDockBarZOrder();
		}
		internal void CleanupOnFormClose()
		{
			BarManager.htFormsVsBarManager.Remove( this.Form );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void FormDisposed( object sender, EventArgs e )
		{
			this.Dispose();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		private void FormRightToLeftChanged( object sender, EventArgs e )
		{
			XPMenuGridFactory.ReleaseAllGrids();
			this.OnPropertyChanged(
				new SyncfusionPropertyChangedEventArgs( PropertyChangeEffect.NeedLayout,
				"RightToLeft", null, ( (Form)sender ).RightToLeft ) );
		}
		#endregion INTERNAL
		#region METHODS
		/// <summary>
		/// Occurs when the floating bar gets closed
		/// </summary>
		[Description( "Occurs when the floating bar gets closed" )]
		public event BarChangedEventHandler FloatingFormClosed;


		/// <summary>
		/// Occurs when reset BarItem is clicked.
		/// </summary>
		[Description( "Occurs when reset BarItem is clicked." )]
		public event BarItemClickedEventHandler ResetBarItemClicked;

		/// <summary>
		/// Occurs when bar visibility is changed by the user.
		/// </summary>
		[Description( "Occurs when bar visibility is changed by the user." )]
		public event BarChangedEventHandler UserChangedBarVisibility;

		protected internal virtual void OnFloatingFormClosed( Bar bar )
		{
			if( bar == null )
				throw new ArgumentNullException( "bar" );

			if( FloatingFormClosed != null )
			{
				FloatingFormClosed( this, new BarChangedEventArgs( bar ) );
			}
		}

		protected internal virtual void OnResetBarItem( BarItem itemToReset )
		{
			if( itemToReset == null )
				throw new ArgumentNullException( "itemToReset" );

			if( ResetBarItemClicked != null )
			{
				ResetBarItemClicked( this, new BarItemClickedEventArgs( itemToReset ) );
			}

		}

		protected internal virtual void OnUserChangedBarVisibility( Bar bar )
		{
			if( UserChangedBarVisibility != null )
			{
				UserChangedBarVisibility( this, new BarChangedEventArgs( bar ) );
			}
		}

		/// <summary>
		/// Looks for bindable properties in each Baritem associated with manager and create a new PD entry for it.
		/// It then creates a brand new PDC combining the PDs of the originalList and the new entries and returns the new PDC.
		/// </summary>
		/// <seealso cref="DataBindingUtils.AppendBindableProperties"/>
		/// <param name="originalList">The original list.</param>
		/// <returns>A collection of property descriptors.</returns>
		public PropertyDescriptorCollection AppendBindableBarItemProperties( PropertyDescriptorCollection originalList )
		{
			return DataBindingUtils.AppendBindableProperties( originalList, this.Items );
		}

		internal bool IsDummyManager()
		{
			if( this.MainFrameBarManager != null )
				return this.MainFrameBarManager.IsDummyManager( this );

			return false;
		}
		/// <summary>
		/// Imports <see cref="BarItem"/>s from one BarManager to another. Not to be confused with "MDI Merging".
		/// </summary>
		/// <param name="sourceRepository">The source <see cref="IBarItemsRepository"/> from which to import bar items.</param>
		/// <remarks>
		/// <para>
		/// Note that the BarManager class implements the IBarItemsRepository interface, due to which you can 
		/// import items from an instance of BarManager. 
		/// </para>
		/// <para>
		/// This method will move all <b>BarItems</b> from the source <b>BarManager</b> to the 
		/// destination <b>BarManager</b>. The <b>BarItems</b> will be moved into their corresponding 
		/// categories in the destination <b>BarManager</b>. If there is a clash in Text value 
		/// of these items (no 2 items within the same category can have the same Text 
		/// value), a new category will be created and the clashing item from the source 
		/// <b>BarManager</b> will be moved to that new category.
		/// </para>
		/// </remarks>
		public void ImportBarItems( IBarItemsRepository sourceRepository )
		{
			BarItemsImporter importer = this.CreateImporter();
			importer.ImportItems( sourceRepository, this );
		}
		protected BarItemsImporter CreateImporter()
		{
			return new BarItemsImporter();
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual BarManager GetManagerFrom( Form form )
		{
			if( this.Form == form )
				return this;

			return GetManagerFromForm( form );
		}

		/// <summary>
		/// Looks for a BarManager associated with the specified form in a global hashtable and if found
		/// returns it.
		/// </summary>
		/// <param name="form">A form that has a BarManager associated with it.</param>
		/// <returns>The associated BarManager; Null if not found.</returns>
		/// <remarks>
		/// A BarManager gets associated with a form when you create an instance of the BarManager passing in the
		/// form in it's constructor.
		/// </remarks>
		public static BarManager GetManagerFromForm( Form form )
		{
			BarManager manager = ( form == null ) ? null : BarManager.htFormsVsBarManager[form] as BarManager;

			//			PropertyDescriptor propertyDescriptor = (PropertyDescriptor)TypeDescriptor.GetProperties((object)form)["BarManager"];
			//			if(propertyDescriptor != null)
			//				manager = (BarManager)propertyDescriptor.GetValue((object)form);

			return manager;
		}

		/// <summary>
		/// Processes the command key
		/// </summary>
		/// <param name="msg">
		/// reference Message.
		/// </param>
		/// <param name="keyData">
		/// Key data of type Keys
		/// </param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			if( this.Customizing  /*(keyData & ShortcutMask) == 0 ||*/
				// This check will prevent the shortcuts from getting processed even if the
				// ActivePopupClient is a TooltipControl, for example. So, removing this.
				// This doesn't seem to be necessary anyway since the dropdown menu and the toolbars
				// eath the shortcut keys when they are active.
				//|| PopupManager.ActivePopupClient != null
				)
				return false;

			return this.ProcessShortcut( keyData );
		}

		/// <summary>
		/// Checks whether the key can be processed for shortcut
		/// </summary>
		/// <param name="key"> Key </param>
		/// <returns>true if can process</returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual bool ProcessShortcut( Keys key )
		{
			if( this.shortcuts[(int)key] != null )
			{
				Form activeForm = Form.ActiveForm;

				// Process the shortcut only when the corresponding Form is active.
				if( this.Form != null && activeForm != null &&
					( ( Form.ActiveForm == this.Form ) ||
					( activeForm.IsMdiContainer && activeForm.ActiveMdiChild == this.Form ) ||
					( this.Form.Parent != null && this.Form.Parent.FindForm() == activeForm ) ) )
				{
					BarItem item = this.shortcuts[(int)key] as BarItem;
					if( this.UpdateUIMFCStyle )
						item.PerformUpdateUI();

					if( item.Enabled )
					{
						item.SetShortCutProcessing( true );
						item.PerformClick();
						item.SetShortCutProcessing( false );

						return true;
					}
				}
			}

			return false;
		}
		/// <summary>
		/// Starts or stops the Customization mode and opens the Customization dialog.
		/// </summary>
		/// <param name="start">True to start customization; false to stop.</param>
		/// <remarks>
		/// Call this method to start Customization mode by opening the
		/// Customization dialog. If EnableCustomizing is not true then
		/// this method will return immediately without starting customization.
		/// </remarks>
		public void Customize( bool start )
		{
			if( start && !( this.EnableCustomizing | this.DesignMode ) )
				return;

			if( start )
			{
				if( !this.Customizing )
					this.Customize( null );
			}
			else
			{
				if( this.Customizing && this.IsCustomizationDialogCreated
					&& this.CustomizationDialog.Visible )
					this.CustomizationDialog.Close();
				this.HidePopups();
			}
		}
		/// <summary>
		/// Starts or stops the Customization mode and opens the Customization dialog.
		/// </summary>
		/// <param name="designerHost"> designerHost is of type IDesignerHost. </param> 
		[Syncfusion.Documentation.DocumentationExclude()]
		public void Customize( IDesignerHost designerHost )
		{
			if( !( this.EnableCustomizing | this.DesignMode ) )
				return;

			if( this.CustomizationDialog.Visible )
				return;

			this.CustomizationDialog.DesignerHost = designerHost;
			this.CustomizationDialog.Owner = this.Form;
			this.CustomizationDialog.RightToLeft = this.RightToLeft;

			// Calling the Being event before refreshing the customization dialog.
			this.OnBeginCustomization( EventArgs.Empty );

			this.CustomizationDialog.Show();
		}

		internal void HidePopups()
		{
			if( ShouldHidePopup )
			{
				if( null != PopupManager.ActivePopupClient )
				{
					PopupManager.ActivePopupClient.HidePopup( PopupCloseType.Deactivated );
				}
			}
		}

		/// <summary>
		/// Makes the caret visible on the screen at the caret's current position.
		/// </summary>
		internal static void ShowCaret()
		{
			NativeMethods.ShowCaret(NativeMethods.GetFocus());
			CaretHidden = false;
		}

		/// <summary>
		/// Removes the caret from the screen.
		/// </summary>
		internal static void HideCaret()
		{
			if (!CaretHidden)
			{
				NativeMethods.HideCaret(NativeMethods.GetFocus());
				CaretHidden = true;
			}
		}

		/// <summary>
		/// Returns whether the control can start dragging.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		[EditorBrowsable( EditorBrowsableState.Never )]
		public bool CanStartDragging( BarItem item )
		{
			return true;
		}
		/// <summary>
		/// Returns a list of BarItems under the specified category ID.
		/// </summary>
		/// <param name="categoryIndex">An index into the Categories list.</param>
		/// <returns>A BarItems list containing BarItem objects that has the specified category ID.</returns>
		public BarItems GetItemsInCategory( int categoryIndex )
		{
			BarItems barItems = new BarItems();
			foreach( BarItem item in this.Items )
			{
				if( item.CategoryIndex == categoryIndex )
					barItems.Add( item );
			}
			return barItems;
		}

		/// <summary>
		/// Gets merged bars.
		/// </summary>
		internal ArrayList GetMergedBars()
		{
			ArrayList mergetBars = new ArrayList();

			foreach( Bar bar in replacedBarsByMerge.Keys )
			{
				mergetBars.Add( bar );
			}

			return mergetBars;
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

		protected internal BarControlInternal GetBarControlForDragDrop()
		{
			BarControlInternal foundControl = null;

			if( commandBarManager != null )
			{
				CommandBarController controller = this.commandBarManager.GetCommandBarController();
				if( controller != null )
				{
					foundControl = GetBarControlForDragDrop( controller.CommandDockBarB );

					if( foundControl == null ) foundControl = GetBarControlForDragDrop( controller.CommandDockBarL );

					if( foundControl == null ) foundControl = GetBarControlForDragDrop( controller.CommandDockBarR );

					if( foundControl == null ) foundControl = GetBarControlForDragDrop( controller.CommandDockBarT );
				}
			}

			if( foundControl == null )
			{
				foundControl = FindBarControlForDragDrop();
			}

			return foundControl;
		}

		private BarControlInternal FindBarControlForDragDrop()
		{
			if( m_arrDragDropControls != null && m_arrDragDropControls.Count > 0 )
			{
				BarControlInternal barControl = null;
				Point mousePos = Control.MousePosition;

				for( int i = 0, len = m_arrDragDropControls.Count; i < len; i++ )
				{
					barControl = m_arrDragDropControls[i] as BarControlInternal;

					if( barControl != null )
					{
						Rectangle bounds = barControl.ClientRectangle;
						bounds.Location = barControl.PointToScreen( Point.Empty );

						if( bounds.Contains( mousePos ) )
						{
							return barControl;
						}
					}
				}
			}

			return null;
		}

		private ArrayList m_arrDragDropControls = new ArrayList();

		protected internal void RegisterDragDropControl( BarControlInternal barControl )
		{
			if( barControl == null ) throw new NullReferenceException( "barControl" );

			if( !m_arrDragDropControls.Contains( barControl ) )
			{
				m_arrDragDropControls.Add( barControl );
			}
		}

		protected internal void UnRegisterDragDropControl( BarControlInternal barControl )
		{
			if( barControl == null ) throw new NullReferenceException( "barControl" );

			if( !m_arrDragDropControls.Contains( barControl ) )
			{
				m_arrDragDropControls.Add( barControl );
			}
		}

		protected internal BarControlInternal GetBarControlForDragDrop( CommandDockBar dockBar )
		{
			if( dockBar == null )
				throw new ArgumentNullException( "dockBar" );

			if( dockBar.Controls.Count > 0 )
			{
				Point mousePos = Control.MousePosition;
				BarControlInternal barControl = null;

				foreach( CommandBar commandBar in dockBar.Controls )
				{
					if( commandBar != null && commandBar.Controls.Count > 0 )
					{
						for( int i = 0, len = commandBar.Controls.Count; i < len; i++ )
						{
							barControl = commandBar.Controls[i] as BarControlInternal;

							if( barControl != null )
							{
								Rectangle bounds = barControl.ClientRectangle;
								bounds.Location = barControl.PointToScreen( Point.Empty );

								if( bounds.Contains( mousePos ) )
								{
									return barControl;
								}
							}
						}
					}
				}
			}

			return null;
		}
#endif
		/// <summary>
		/// Sets form's property named "IsUsedWithBarManager" if 
		/// form is of Office2007Form type.
		/// </summary>
		/// <param name="bIsUsedWithBarManager"> The new value for the property. </param>
		/// <param name="form"> The form whose property value will be set. </param>
		private void SetFormProperty( bool bIsUsedWithBarManager, Form form )
		{
			Office2007Form frmOffice2007 = form as Office2007Form;

			if( frmOffice2007 != null )
			{
				Type type = typeof( Office2007Form );
				PropertyInfo piInfo = type.GetProperty( "IsUsedWithBarManager", BindingFlags.Instance | BindingFlags.NonPublic );

				if( piInfo != null )
				{
					piInfo.SetValue( frmOffice2007, bIsUsedWithBarManager, null );
				}
			}
		}
		#endregion METHODS
		#region EVENTS
		/// <summary>
		/// Occurs when a <see cref="Bar"/> object is bound or unbound to a <b>Control</b>.
		/// </summary>
		/// 
		/// <seealso cref="BarControlBindingChangedArgs"/>
		/// <seealso cref="BarManager.OnBarControlBindingChanged"/>
		/// <remarks>
		/// <para>The <see cref="Bar"/> component (representing a toolbar and a main-menu) gets associated
		/// with a <see cref="CommandBar"/> control during runtime. This binding is static
		/// in an SDI app, but usually very transient in an MDI application. In an MDI, a <b>bar</b>
		/// will frequently get bound and unbound to controls as the active child form changes.</para>
		/// <para>The <see cref="Bar"/> object itself exposes some
		/// styles to affect the look-and-feel of the toolbars through it's <see cref="Bar.BarStyle"/> property.
		/// However, you might want to access the <see cref="CommandBar"/> control hosting
		/// the <see cref="Bar"/> object for some advanced customization of the toolbar.</para>
		/// <para>You can do so by handling this event which is fired whenever the
		/// <b>bar</b> object gets bound/unbound to a Control. For the toolbars and the main-menu,
		/// this control is an instance of the <see cref="CommandBar"/> class and hence can be cast to it.
		/// </para>
		/// <para>
		/// You could also use the <see cref="GetBarControl"/> method to get the <see cref="CommandBar"/>
		/// currently bound to a <see cref="Bar"/>.
		/// </para>
		/// </remarks>
		[Description( "Occurs when Bar object is bound or unbound to a control." )]
		public event BarControlBindingChangedEventHanlder BarControlBindingChanged;

		/// <summary>
		/// Raises the <see cref="BarControlBindingChanged"/> event.
		/// </summary>
		/// <param name="args">A <see cref="BarControlBindingChangedArgs"/> that contains the event data.</param>
		/// <seealso cref="BarManager.BarControlBindingChanged"/>
		/// <remarks>
		/// The OnBarControlBindingChanged method also allows derived classes to handle the event 
		/// without attaching a delegate. This is the preferred technique for 
		/// handling the event in a derived class. 
		/// <para>Notes to Inheritors:  When overriding OnBarControlBindingChanged in a derived 
		/// class, be sure to call the base class's OnBarControlBindingChanged method so that 
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected internal virtual void OnBarControlBindingChanged( BarControlBindingChangedArgs args )
		{
			if( this.BarControlBindingChanged != null )
			{
				this.BarControlBindingChanged( this, args );
			}
		}

		/// <summary>
		/// Raises the CustomizingItemChanged event.
		/// </summary>
		/// <param name="args">
		/// An <see cref="EventArgs"/> object containing data pertaining to this event.
		/// </param>
		/// <remarks>
		/// The OnCustomizingItemChanged method also allows derived classes to handle the event 
		/// without attaching a delegate. This is the preferred technique for 
		/// handling the event in a derived class. 
		/// <para>Notes to Inheritors:  When overriding OnCustomizingItemChanged in a derived 
		/// class, be sure to call the base class's OnCustomizingItemChanged method so that 
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnCustomizingItemChanged( EventArgs args )
		{
			if( this.CustomizingItemChanged != null )
			{
				this.CustomizingItemChanged( this, args );
			}
		}
		/// <summary>
		/// Raises the SelectedItemChanged event.
		/// </summary>
		/// <param name="args">
		/// An <see cref="EventArgs"/> object containing data pertaining to this event.
		/// </param>
		/// <remarks>
		/// The OnSelectedItemChanged method also allows derived classes to handle the event 
		/// without attaching a delegate. This is the preferred technique for 
		/// handling the event in a derived class. 
		/// <para>Notes to Inheritors:  When overriding OnSelectedItemChanged in a derived 
		/// class, be sure to call the base class's OnSelectedItemChanged method so that 
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnSelectedItemChanged( EventArgs args )
		{
			if( this.SelectedItemChanged != null )
			{
				this.SelectedItemChanged( this, args );
			}
		}
		/// <summary>
		/// Raises the PropertyChanged event.
		/// </summary>
		/// <param name="args">
		/// An <see cref="EventArgs"/> object containing data pertaining to this event.
		/// </param>
		/// <remarks>
		/// The OnPropertyChanged method also allows derived classes to handle the event 
		/// without attaching a delegate. This is the preferred technique for 
		/// handling the event in a derived class. 
		/// <para>Notes to Inheritors:  When overriding OnPropertyChanged in a derived 
		/// class, be sure to call the base class's OnPropertyChanged method so that 
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnPropertyChanged( SyncfusionPropertyChangedEventArgs args )
		{
			if( this.PropertyChanged != null )
			{
				this.PropertyChanged( this, args );
			}
		}
		/// <summary>
		/// Raises the CustomizationBegin event.
		/// </summary>
		/// <param name="args">
		/// An EventArgs object containing data pertaining to this event.
		/// </param>
		/// <remarks>
		/// The OnBeginCustomization method also allows derived classes to handle the event 
		/// without attaching a delegate. This is the preferred technique for 
		/// handling the event in a derived class. 
		/// <para>Notes to Inheritors:  When overriding OnBeginCustomization in a derived 
		/// class, be sure to call the base class's OnBeginCustomization method so that 
		/// registered delegates receive the event.</para>
		/// </remarks>
		internal protected virtual void OnBeginCustomization( EventArgs args )
		{
			if( this.CustomizationBegin != null )
			{
				this.CustomizationBegin( this, args );
			}
		}
		/// <summary>
		/// Raises the CustomizationDone event.
		/// </summary>
		/// <param name="args">
		/// An EventArgs object containing data pertaining to this event.
		/// </param>
		/// <remarks>
		/// The OnCustomizationDone method also allows derived classes to handle the event 
		/// without attaching a delegate. This is the preferred technique for 
		/// handling the event in a derived class. 
		/// <para>Notes to Inheritors:  When overriding OnCustomizationDone in a derived 
		/// class, be sure to call the base class's OnCustomizationDone method so that 
		/// registered delegates receive the event.</para>
		/// </remarks>
		internal protected virtual void OnCustomizationDone( EventArgs args )
		{
			// At the end of customization, reset all popups
			this.HidePopups();

			if( this.CustomizationDone != null )
			{
				this.CustomizationDone( this, args );
			}
		}

		/// <summary>
		/// Raises the ItemClicked event.
		/// </summary>
		/// <param name="args">A <see cref="BarItemClickedEventArgs"/> object containing data pertaining to this event.</param>
		/// <remarks>
		/// The OnItemClicked method also allows derived classes to handle the event 
		/// without attaching a delegate. This is the preferred technique for 
		/// handling the event in a derived class. 
		/// <para>Notes to Inheritors:  When overriding OnItemClicked in a derived 
		/// class, be sure to call the base class's OnItemClicked method so that 
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected internal virtual void OnItemClicked( BarItemClickedEventArgs args )
		{
			if( this.ItemClicked != null )
			{
				this.ItemClicked( this, args );
			}
		}

		/// <summary>
		/// Raises the AfterClone event.
		/// </summary>
		/// <param name="args">A <see cref="BarItemClickedEventArgs"/> object containing data pertaining to this event.</param>
		/// <remarks>
		/// The OnAfterClone method also allows derived classes to handle the event 
		/// without attaching a delegate. This is the preferred technique for 
		/// handling the event in a derived class. 
		/// <para>Notes to Inheritors:  When overriding OnAfterClone in a derived 
		/// class, be sure to call the base class's OnAfterClone method so that 
		/// registered delegates receive the event.</para>
		/// </remarks>
		/// <example>
		/// <code lang="C#">
		/// // Listen to the AfterClone event of the ChildFrameBarManager and
		/// // set the cloned BarManager's ImageList and LargeImageList.
		/// // This is necessary because cloning an ImageList (il1.ImageStream = il2.ImageStream;) seems to be broken in 1.1.
		/// private void childFrameBarManager_AfterClone(object sender, Syncfusion.Windows.Forms.Tools.XPMenus.BarManagerClonedEventArgs args)
		/// {
		/// 	System.Resources.ResourceManager resources = new System.Resources.ResourceManager(this.GetType());
		///	
		/// 	// Load the images from the resource and set it to the ImageLists. 
		/// 	// You can typically copy the right-hand side portion of the following statements from the 
		/// 	// designer generated code in the InitializeComponent method.
		/// 	// imgList16 and imgList24 are the names of the ImageList instances in this Form.
		/// 	args.ClonedBarManager.ImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList16.ImageStream")));
		///		args.ClonedBarManager.LargeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList24.ImageStream")));
		/// }
		/// </code>
		/// <code lang="VB">
		/// ' Listen to the AfterClone event of the ChildFrameBarManager and
		/// ' set the cloned BarManager's ImageList and LargeImageList.
		/// ' This is necessary because cloning an ImageList (il1.ImageStream = il2.ImageStream;) seems to be broken in 1.1.
		/// Private  Sub childFrameBarManager_AfterClone(ByVal sender As Object, ByVal args As Syncfusion.Windows.Forms.Tools.XPMenus.BarManagerClonedEventArgs)
		/// Dim resources As System.Resources.ResourceManager =  New System.Resources.ResourceManager(Me.GetType()) 
		/// 
		///		' Load the images from the resource and set it to the ImageLists. 
		///		' You can typically copy the right-hand side portion of the following statements from the 
		///		' designer generated code in the InitializeComponent method.
		///		args.ClonedBarManager.ImageList.ImageStream = (CType((resources.GetObject("imgList16.ImageStream")), System.Windows.Forms.ImageListStreamer))
		///		args.ClonedBarManager.LargeImageList.ImageStream = (CType((resources.GetObject("imgList24.ImageStream")), System.Windows.Forms.ImageListStreamer))
		///	End Sub
		///	</code>
		/// </example>
		protected internal virtual void OnAfterClone( BarManagerClonedEventArgs args )
		{
			if( this.AfterClone != null )
			{
				this.AfterClone( this, args );
			}
		}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		internal event EventHandler CustomDragChanged;
#endif

		/// <summary>
		/// Occurs when the BarManager's CustomizingItem property has changed.
		/// </summary>
		/// <remarks>
		/// <para>This event will also be raised when it's associated
		/// MainBarManager's CustomizingItem property changes.</para></remarks>
		[Description( "Occurs when the BarManager's CustomizingItem property has changed." ),
		Category( "Customization" )]
		public event EventHandler CustomizingItemChanged;

		/// <summary>
		/// Occurs when a new <see cref="BarItem"/> is selected or unselected
		/// by the user.
		/// </summary>
		/// <remarks>
		/// This is a good event to listen to if you want to, for example,
		/// show a selected item's tooltip in the status bar. Use the <see cref="SelectedItem"/>
		/// property to get a reference to the selected <see cref="BarItem"/>.
		/// </remarks>
		[Description( "Occurs when a new BarItem is selected or unselected by the user." ),
		Category( "UI" )]
		public event EventHandler SelectedItemChanged;
		/// <summary>
		/// Occurs when the BarManager's LargeIcons or ThemesEnabled property has changed.
		/// </summary>
		/// <remarks>
		/// <para>This event will also be raised when it's associated
		/// MainBarManager's LargeIcons or ThemesEnabled property changes.</para></remarks>
		[Description( "Occurs when the BarManager's LargeIcons or ThemesEnabled property has changed." ),
		Category( "Appearance" )]
		public event SyncfusionPropertyChangedEventHandler PropertyChanged;
		/// <summary>
		/// Occurs when the Customization dialog is about to be shown.
		/// </summary>
		[Description( "Occurs when the Customization dialog is about to be shown." ),
		Category( "Customization" )]
		public event EventHandler CustomizationBegin;
		/// <summary>
		/// Occurs when the Customization dialog has been closed.
		/// </summary>
		[Description( "Occurs when the Customization dialog has been closed." ),
		Category( "Customization" )]
		public event EventHandler CustomizationDone;
		/// <summary>
		/// Occurs when one of the items in this BarManager was clicked.
		/// </summary>
		[Description( "Occurs when one of the items in this BarManager was clicked." ),
		Category( "UI" )]
		public event BarItemClickedEventHandler ItemClicked;

		/// <summary>
		/// Occurs after a clone of this BarManager was created in an MDI scenario.
		/// </summary>
		/// <remarks><p>This is usually called for a <see cref="ChildFrameBarManager"/>
		/// when merging happens in an MDI scenario.</p>
		/// <p>
		/// This event is provided to workaround a 1.1 framework bug which prevents us from cloning an 
		/// ImageList (The code "imageList.ImageStream = anotherImageList.ImageStream" doesn't work in 1.1).
		/// </p>
		/// </remarks>
		[Category( "Misc" ),
		Description( "Occurs after a clone of this BarManager was created in a MDI scenario." )]
		public event BarManagerClonedEventHandler AfterClone;
		#endregion EVENTS

		#region SERIALIZATION
		protected BarManager( SerializationInfo info, StreamingContext context )
			: this()
		{
			foreach( SerializationEntry entry in info )
			{
				switch( entry.Name )
				{
					case "ShowItemsInCustomizationDialog":
					// When using SoapFormatter, the primitive types will be stored as strings
					if( entry.Value is string )
						// This is faster than calling info.GetBool("ShowItemsInCustomizationDialog");
						this.ShowItemsInCustomizationDialog = (bool)Convert.ChangeType( entry.Value, typeof( bool ) );
					else
						this.ShowItemsInCustomizationDialog = (bool)entry.Value;
					break;
					case "PartialMenusResetDelay":
					// When using SoapFormatter, the primitive types will be stored as strings
					if( entry.Value is string )
						// This is faster than calling info.GetInt32("PartialMenusResetDelay");
						this.PartialMenusResetDelay = (int)Convert.ChangeType( entry.Value, typeof( int ) );
					else
						this.PartialMenusResetDelay = (int)entry.Value;
					break;
					case "UsePartialMenus":
					// When using SoapFormatter, the primitive types will be stored as strings
					if( entry.Value is string )
						// This is faster than calling info.GetInt32("UsePartialMenus");
						this.UsePartialMenus = (bool)Convert.ChangeType( entry.Value, typeof( bool ) );
					else
						this.UsePartialMenus = (bool)entry.Value;
					break;
					case "EnableCustomizing":
					// When using SoapFormatter, the primitive types will be stored as strings
					if( entry.Value is string )
						// This is faster than calling info.GetInt32("EnableCustomizing");
						this.EnableCustomizing = (bool)Convert.ChangeType( entry.Value, typeof( bool ) );
					else
						this.EnableCustomizing = (bool)entry.Value;
					break;
					case "ExpandPartialMenusAfterDelay":
					// When using SoapFormatter, the primitive types will be stored as strings
					if( entry.Value is string )
						// This is faster than calling info.GetInt32("ExpandPartialMenusAfterDelay");
						this.ExpandPartialMenusAfterDelay = (bool)Convert.ChangeType( entry.Value, typeof( bool ) );
					else
						this.ExpandPartialMenusAfterDelay = (bool)entry.Value;
					break;
					case "FormName":
					this.FormName = (string)entry.Value;
					break;
					case "LargeIcons":
					// When using SoapFormatter, the primitive types will be stored as strings
					if( entry.Value is string )
						// This is faster than calling info.GetInt32("LargeIcons");
						this.LargeIcons = (bool)Convert.ChangeType( entry.Value, typeof( bool ) );
					else
						this.LargeIcons = (bool)entry.Value;
					break;
					case "Style":
					this.Style = (VisualStyle)entry.Value;
					break;
					case "Categories":
					ArrayList al = (ArrayList)entry.Value;
					foreach( string s in al )
						this.Categories.Add( s );
					break;
					case "CategoriesToIgnoreInCustDialog":
					ArrayList il = (ArrayList)entry.Value;
					foreach( int i in il )
						this.CategoriesToIgnoreInCustDialog.Add( i );
					break;
					case "ImageList":
					this.ImageList = new ImageList( this.components );
					this.ImageList.ImageStream = (ImageListStreamer)entry.Value;
					break;
					case "ImageListAdv":
					this.ImageListAdv = entry.Value as ImageListAdv;
					break;
					case "LargeImageList":
					this.LargeImageList = new ImageList( this.components );
					this.LargeImageList.ImageStream = (ImageListStreamer)entry.Value;
					break;
					case "LargeImageListAdv":
					this.LargeImageListAdv = entry.Value as ImageListAdv;
					break;
					case "Items":
					this.deserializedItems = entry.Value as ArrayList;
					break;
					case "Bars":
					this.deserializedBars = (ArrayList)entry.Value;
					break;
				}
			}

			//this.ShowItemsInCustomizationDialog = info.GetBoolean("ShowItemsInCustomizationDialog");
			//this.PartialMenusResetDelay = info.GetInt32("PartialMenusResetDelay");
			//this.UsePartialMenus = info.GetBoolean("UsePartialMenus");
			//this.EnableCustomizing = info.GetBoolean("EnableCustomizing");
			//this.ExpandPartialMenusAfterDelay = info.GetBoolean("ExpandPartialMenusAfterDelay");
			//this.FormName = info.GetString("FormName");
			//this.LargeIcons = info.GetBoolean("LargeIcons");

			//			ArrayList al = (ArrayList)info.GetValue("Categories", typeof(ArrayList));
			//			foreach(string s in al)
			//				this.Categories.Add(s);


			//			ArrayList il = (ArrayList)info.GetValue("CategoriesToIgnoreInCustDialog", typeof(ArrayList));
			//			foreach(int i in il)
			//				this.CategoriesToIgnoreInCustDialog.Add(i);

			//			object temp = info.GetValue("ImageList", typeof(ImageListStreamer));
			//			if(temp != null)
			//			{
			//				this.ImageList = new ImageList(this.components);
			//				this.ImageList.ImageStream = (ImageListStreamer)info.GetValue("ImageList", typeof(ImageListStreamer));
			//			}
			//
			//			temp = info.GetValue("LargeImageList", typeof(ImageListStreamer));
			//			if(temp != null)
			//			{
			//				this.LargeImageList = new ImageList(this.components);
			//				this.LargeImageList.ImageStream = (ImageListStreamer)info.GetValue("LargeImageList", typeof(ImageListStreamer));
			//			}

			//			this.deserializedItems = (ArrayList)info.GetValue("Items", typeof(ArrayList));

			//			this.deserializedBars = (ArrayList)info.GetValue("Bars", typeof(ArrayList));
		}
		void IDeserializationCallback.OnDeserialization( object sender )
		{
			foreach( BarItem item in this.deserializedItems )
				this.Items.Add( item );

			foreach( Bar bar in this.deserializedBars )
				this.Bars.Add( bar );
		}

		/// <summary>
		/// Assists the serializer to perform serialization
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public virtual void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue( "ShowItemsInCustomizationDialog", this.ShowItemsInCustomizationDialog );
			info.AddValue( "PartialMenusResetDelay", this.PartialMenusResetDelay );
			info.AddValue( "UsePartialMenus", this.UsePartialMenus );
			info.AddValue( "EnableCustomizing", this.EnableCustomizing );
			info.AddValue( "ExpandPartialMenusAfterDelay", this.ExpandPartialMenusAfterDelay );
			info.AddValue( "FormName", this.FormName );
			info.AddValue( "LargeIcons", this.LargeIcons );
			// Use my value (not inherited value)
			info.AddValue( "Style", this.style );

			info.AddValue( "Categories", Categories );

			ArrayList temp = new ArrayList( CategoriesToIgnoreInCustDialog );

			info.AddValue( "CategoriesToIgnoreInCustDialog", temp );

			if( this.ImageList != null )
			{
				info.AddValue( "ImageList", this.ImageList.ImageStream );
			}
			else
			{
				info.AddValue( "ImageList", null );
			}

			info.AddValue( "ImageListAdv", this.ImageListAdv );

			if( this.LargeImageList != null )
			{
				info.AddValue( "LargeImageList", this.LargeImageList.ImageStream );
			}
			else
			{
				info.AddValue( "LargeImageList", null );
			}

			info.AddValue( "LargeImageListAdv", this.LargeImageListAdv );

			temp = new ArrayList( this.Items );
			info.AddValue( "Items", temp );

			temp = new ArrayList( this.Bars );
			info.AddValue( "Bars", temp );
		}
		#endregion SERIALIZATION
		#region CLONING
		internal static bool preventCopyingItemsInContainers = false;
        internal static bool DesignerBarClone = false;
		/// <summary>
		/// Copies the properties of this BarManager into the specified BarManager.
		/// </summary>
		/// <param name="barManager">The BarManager where the values should be copied to.</param>
		/// <remarks>
		/// 
		/// </remarks>
		public virtual void CopyTo( BarManager barManager )
		{
			BarManager.preventCopyingItemsInContainers = true;
			barManager.ShowItemsInCustomizationDialog = this.ShowItemsInCustomizationDialog;
			barManager.PartialMenusResetDelay = this.PartialMenusResetDelay;
			barManager.UsePartialMenus = this.UsePartialMenus;
			barManager.EnableCustomizing = this.EnableCustomizing;
			barManager.ExpandPartialMenusAfterDelay = this.ExpandPartialMenusAfterDelay;
			barManager.FormName = this.FormName;
			barManager.LargeIcons = this.LargeIcons;
			barManager.Style = this.style;

			foreach( string category in this.Categories )
				barManager.Categories.Add( category );

			foreach( int ignoreIndex in this.CategoriesToIgnoreInCustDialog )
				barManager.CategoriesToIgnoreInCustDialog.Add( ignoreIndex );

			if( this.ImageList != null )
			{
				barManager.ImageList = new ImageList();

				barManager.ImageList.ImageStream = this.ImageList.ImageStream;

				barManager.components.Add( barManager.ImageList );
			}

			if( this.ImageListAdv != null )
			{
				barManager.ImageListAdv = (ImageListAdv)this.ImageListAdv.Clone();
				barManager.components.Add( barManager.ImageListAdv );
			}

			if( this.LargeImageList != null )
			{
				barManager.LargeImageList = new ImageList();

				barManager.LargeImageList.ImageStream = this.LargeImageList.ImageStream;

				barManager.components.Add( barManager.LargeImageList );
			}

			if( this.LargeImageListAdv != null )
			{
				barManager.LargeImageListAdv = (ImageListAdv)this.LargeImageListAdv.Clone();
				barManager.components.Add( barManager.LargeImageListAdv );
			}

			// First pass, clone everything. ParentBarItems will be copied over without their children
			foreach( BarItem item in this.Items )
			{
				if( !( item is NewMenuItem ) )
					barManager.Items.Add( item.Clone() as BarItem );
			}
			int i = -1;
			foreach( BarItem item in this.Items )
			{
				i++;
				if( item is ParentBarItem )
				{
					ParentBarItem cloned = barManager.Items[i] as ParentBarItem;
					ParentBarItem parentBarItem = ( (ParentBarItem)item );
					foreach( BarItem child in parentBarItem.Items )
					{
						BarItem clonedChild = barManager.Items.FindItem( child.ID );
						cloned.Items.Add( clonedChild );
						// Now we are ready to copy over the grouping info.
						if( parentBarItem.IsGroupBeginning( child ) )
							cloned.BeginGroupAt( clonedChild );
					}
					//Do not add this again as the cloned ParentBarItem is already there in the list
					//barManager.Items.Add(cloned);
				}
			}
			foreach( Bar bar in this.Bars )
			{
				Bar barCopy = bar.Clone() as Bar;
				foreach( BarItem child in bar.Items )
				{
					BarItem clonedChild = barManager.Items.FindItem( child.ID );
					barCopy.Items.Add( clonedChild );
					// Now we are ready to copy over the grouping info.
					if( bar.IsGroupBeginning( child ) )
						barCopy.BeginGroupAt( clonedChild );
				}
				barManager.Bars.Add( barCopy );
			}
			BarManager.preventCopyingItemsInContainers = false;
		}
		#endregion
		#region IBarItemsRepository Imp
		// Workaround for Everett beta. Without this, the ImageList property will not deserialize properly (in the designer).
		ImageList IBarItemsRepository.ImageList
		{
			get { return this.ImageList; }
		}
		ImageList IBarItemsRepository.LargeImageList
		{
			get { return this.LargeImageList; }
		}

		BarItems IBarItemsRepository.Items
		{
			get { return this.Items; }
		}
		Bars IBarItemsRepository.Bars
		{
			get
			{ return this.Bars; }
		}
		ArrayList IBarItemsRepository.Categories
		{
			get { return this.Categories; }
		}
		IntList IBarItemsRepository.CategoriesToIgnoreInCustDialog
		{ get { return this.CategoriesToIgnoreInCustDialog; } }
		#endregion
		#region MENU_NAVIGATION
		private static bool navigatingMenus = false;
		internal static void OnStartingMenuNavigation( Form form, Control menuControl )
		{
			if( navigatingMenus )
				return;
			navigatingMenus = true;
			FireMenuStart( form );
		}
		internal static void OnStoppingMenuNavigation( Form form, Control menuControl )
		{
			if( !navigatingMenus )
				return;

			navigatingMenus = false;
			FireMenuComplete( form );
		}
		private static void FireMenuStart( Form form )
		{
			if( form != null )
			{
				// No need to do this, as we fire the Focus change when a new menu item is selected in the toolbar.
				//				Control.ControlAccessibleObject cao = menuControl.AccessibilityObject as Control.ControlAccessibleObject;
				//				if(cao != null)
				//					cao.NotifyClients(AccessibleEvents.Focus);
                            
				MethodInfo mInfo = typeof( Form ).GetMethod( "OnMenuStart",
					BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic );
				if( mInfo != null )
				{
					mInfo.Invoke( form, new object[] { new MenuNavigationEventArgs() } );
				}

				HideCaret();
			}
		}
		private static void FireMenuComplete( Form form )
		{
			Control curFocusControl = Control.FromHandle( NativeMethods.GetFocus() );
			if( curFocusControl != null )
			{
				Control.ControlAccessibleObject cao = curFocusControl.AccessibilityObject as Control.ControlAccessibleObject;
				if( cao != null )
					cao.NotifyClients( AccessibleEvents.Focus );
			}

			MethodInfo mInfo = typeof( Form ).GetMethod( "OnMenuComplete",
				BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic );
			if( mInfo != null )
			{
				mInfo.Invoke( form, new object[] { new MenuNavigationEventArgs() } );
			}

			ShowCaret();
		}
		#endregion MENU_NAVIGATION

		/// <summary>
		/// Indicates whether bar manager is in process of initializing, i.e. <see cref="BarManager.BeginInit"/> was called first time.
		/// </summary>
		internal bool Initializing
		{
			get
			{
				return !this.endInitCalled;
			}
		}

		private bool m_bShouldHidePopup = false;
		/// <summary>
		/// Indicates whethr to hide Popups on form deactivation.
		/// Default is true.
		/// </summary>
		internal bool ShouldHidePopup
		{
			get
			{
				return m_bShouldHidePopup;
			}
			set
			{
				if( value != m_bShouldHidePopup )
				{
					m_bShouldHidePopup = value;
				}
			}
		}

		private void Form_Deactivate( object sender, EventArgs e )
		{
			if( PopupManager.ActivePopupClient != null )
			{
				// Don't hide popupcontrolcontainer if user clicked on a control within it.
				if( PopupManager.ActivePopupClient is PopupControlContainer )
				{
					// ConfirmDeactivate checks whether the user clicked inside a child control of this PopupControlContainer.
					// If this is not the case (e.g. user clicked on another window on the desktop) then the popup gets hidden.
					( (PopupControlContainer)PopupManager.ActivePopupClient ).ConfirmDeactivate();
				}
				else
				{
					MenuGrid grid = PopupManager.ActivePopupClient as MenuGrid;
					if( grid == null || !grid.IsMouseDownProcessing )
					{
						PopupManager.ActivePopupClient.HidePopup( PopupCloseType.Deactivated );
					}
				}
			}
			// this.HidePopups();
		}
		/// <summary>
		/// Returns the current working bar.
		/// </summary>
		/// <param name="item">BarItem contains in searched Bar.</param>
		/// <returns>Parental Bar of specified BarItem.</returns>
		protected Bar GetBarFromBarItem( BarItem item, ref Queue pbiQueue )
		{
			Bar currentBar = null;

			foreach( Bar b in this.Bars )
			{
				if( FindBarItemInItems( b.Items, item, ref pbiQueue ) )
				{
					currentBar = b;
					break;
				}
			}

			return currentBar;
		}

		protected bool FindBarItemInItems( BarItems items, BarItem item, ref Queue pbiQueue )
		{
			bool bResult = false;

			foreach( BarItem bi in items )
			{
				if( bi == item )
				{
					bResult = true;
					break;
				}
				else
				{
					ParentBarItem pbi = bi as ParentBarItem;

					if( null != pbi )
					{
						pbiQueue.Enqueue( pbi );

						bResult = FindBarItemInItems( pbi.Items, item, ref pbiQueue );

						if( bResult )
						{
							break;
						}
						else
						{
							pbiQueue.Dequeue();
						}
					}
				}
			}

			return bResult;
		}

		/// <summary>
		/// Returns the bar renderer from bar.
		/// </summary>
		/// <param name="bar">Bar whose renderer should be returned.</param>
		/// <returns>Bar renderer.</returns>
		internal BarRenderer GetBarRendererFromBar( Bar bar )
		{
			BarRenderer render = null;

			if( this.commandBarManager != null )
			{
				CommandBarExt commandExt = this.commandBarManager.GetCommandBarFromBar( bar );

				if( commandExt != null )
				{
					render = commandExt.BarControl.barRenderer;
				}
			}

			return render;
		}

		/// <summary>
		/// Bar to currently work with.
		/// </summary>
		private Bar workingBar = null;

		/// <summary>
		/// Shows popup of specified item.
		/// </summary>
		/// <param name="item">Parent bar item whose popup must be shown.</param>
		public void ShowPopup( ParentBarItem item )
		{
			Queue pbiQueue = new Queue();
			workingBar = GetBarFromBarItem( item, ref pbiQueue );
			BarRenderer renderer = GetBarRendererFromBar( workingBar );

			if( renderer != null )
			{
				if( pbiQueue.Count > 0 )
				{
					pbiQueue.Enqueue( item );
					item = pbiQueue.Dequeue() as ParentBarItem;
				}

				renderer.SetCurrentTrackItem( workingBar.Items.IndexOf( item ), true, true, pbiQueue );
			}
		}

		/// <summary>
		/// Hides currently open popup.
		/// </summary>
		public void HidePopup()
		{
			if( workingBar != null )
			{
				BarRenderer renderer = GetBarRendererFromBar( workingBar );

				if( renderer != null )
					renderer.SetCurrentTrackItem( -1, false, false );

				workingBar = null;
			}
		}

		#region Event handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void MenuActivationControl_AppDeactivate( object sender, EventArgs e )
		{
			XPMenuActive = false;
			IPopupChild popupClient = PopupManager.ActivePopupClient;

			if( null != popupClient )
			{
				popupClient.HidePopup( PopupCloseType.Deactivated );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void MenuActivationControl_AppActivate( object sender, EventArgs e )
		{
			XPMenuActive = true;
		}
		#endregion

		#region IIgnoreWorkingArea implementation

		private bool m_bIgnoreWorkingArea = false;

		/// <summary>
		/// Indicates whether to ignore working area of the display before showing popups.
		/// </summary>
		[DefaultValue( false )]
		[Description( "Indicates whether to ignore working area of the display before showing popups." )]
		public bool IgnoreWorkingArea
		{
			get
			{
				return m_bIgnoreWorkingArea;
			}
			set
			{
				if( m_bIgnoreWorkingArea != value )
				{
					m_bIgnoreWorkingArea = value;
					foreach( BarItem bi in this.Items )
					{
						IIgnoreWorkingArea iwa = bi as IIgnoreWorkingArea;
						if( iwa != null )
						{
							iwa.IgnoreWorkingArea = value;
						}

					}
				}
			}
		}

		#endregion

		/// <summary>
		/// Retrieves BarItem object by associated keyboard shortcut.
		/// </summary>
		/// <param name="key">Keyboard shortcut.</param>
		/// <returns>BarItem object.</returns>
		public BarItem GetBarItemFromShortcut( Keys key )
		{
			return ( shortcuts[(int)key] as BarItem );
		}
	}



	/// <summary>
	/// The event args received in the <see cref="System.Windows.Forms.Form.MenuStart"/>
	/// and <see cref="System.Windows.Forms.Form.MenuComplete"/> events.
	/// </summary>
	/// <remarks>
	/// <para>This is the type of the EventArgs received in the <b>>Form.MenuStart</b> and <b>Form.MenuComplete</b>
	/// events when the XPMenus framework is used in the form to show the main-menu and context menus.</para>
	/// <para>
	/// Currently this type doesn't include any properties, it just lets you distinguish between
	/// the .NET menus and the XPMenus in the <b>Form.MenuStart</b> and <b>Form.MenuComplete</b> events.
	/// </para>
	/// </remarks>
	public class MenuNavigationEventArgs: EventArgs
	{
	}

	public class BarsSerializer:
		CodeDomSerializer
	{
		public override object Serialize( IDesignerSerializationManager manager, object value )
		{
			CodeDomSerializer baseClassSerializer =
				(CodeDomSerializer)manager.GetSerializer( value.GetType().BaseType, typeof( CodeDomSerializer ) );
			Bars bars = (Bars)value;

			if( bars.Count > 0 )
			{
				bars.SuspendEvents();

				for( int i = 0; i < bars.Count; ++i )
				{
					MergedBar mergedBar = bars[i] as MergedBar;

					if( null != mergedBar )
					{
						bars[i] = mergedBar.MergedBarsCollection[0];
					}
				}

				bars.ResumeEvents( false );
			}

			return baseClassSerializer.Serialize( manager, value );
		}

		public override object Deserialize( IDesignerSerializationManager manager, object codeObject )
		{
			CodeDomSerializer baseClassSerializer =
				(CodeDomSerializer)manager.GetSerializer( codeObject.GetType().BaseType, typeof( CodeDomSerializer ) );

			return baseClassSerializer.Deserialize( manager, codeObject );
		}
	}

	/// <summary>
	/// Holds a list of <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.Bar"/> instances.
	/// </summary>
	/// <remarks>
	/// Used by a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager"/> class to hold a list of bar instances representing the tool bars.
	/// </remarks>
	/// <example>
	/// Take a look at our XPMenus samples under the Tools\Samples\Menus Package folder
	/// for usage example.
	/// </example>
	[DesignerSerializer( typeof( BarsSerializer ), typeof( CodeDomSerializer ) )]
	public class Bars:
		ArrayListExt,
		IDisposable
	{
		#region Fields
		private bool validating = false;
		internal BarManager manager;
		#endregion

		#region Constructors
		/// <summary>
		/// Creates a new instance of the Bars class.
		/// </summary>
		public Bars() { }
		#endregion

		#region Properties
		/// <summary>
		/// Returns a bar instance at the specified index.
		/// </summary>
		/// <param name="index">The index where a bar is searched for.</param>
		/// <returns>The bar at the specified index.</returns>
		public new Bar this[int index]
		{
			get { return (Bar)base[index]; }
			set { base[index] = value; }
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Collections.ArrayListExt.Add"/>.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public override int Add( object value )
		{
			if( manager != null && manager.Designer != null && manager.Designer.DesignerHost.GetDesigner( value as IComponent ) == null )
			{
				// Insert the component into designtime.
				Bar incomingBar = (Bar)value;

				Bar bar = (Bar)this.manager.Designer.DesignerHost.CreateComponent( typeof( Bar ) );
				bar.Manager = incomingBar.Manager;
				bar.BarName = incomingBar.BarName;
				bar.BarStyle = incomingBar.BarStyle;
				foreach( BarItem item in incomingBar.Items )
					bar.Items.Add( item );
				foreach( int i in incomingBar.SeparatorIndices )
					bar.SeparatorIndices.Add( i );

				value = bar;
			}

			return base.Add( value );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		public override void Remove( object obj )
		{
			if( base.Contains( obj ) )
			{
				base.Remove( obj );
			}
			else RemoveMergedBar( obj as Bar );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool Contains( object item )
		{
			Bar barToFind = item as Bar;

			if( barToFind != null && this.IsBarUsedForMerge( barToFind ) )
			{
				return true;
			}

			return base.Contains( item );
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Collections.ArrayListExt.AddHandlers"/>.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		protected override void AddHandlers( object item )
		{
			if( !( item is Bar ) )
				throw new ArrayTypeMismatchException();

			// Assert that there are no other items with the same caption
			Bar bar = item as Bar;
			ValidateBarStyles( bar );

			// Reparent the Bar to the manager. Required, when ImportBarItems is used to
			// import Bars from one manager to another.
			if( this.manager != null && bar.Manager != this.manager )
				bar.Manager = this.manager;

			base.AddHandlers( item );
		}
		/// <summary>
		/// Overridden. See <see cref="Syncfusion.Collections.ArrayListExt.OnItemPropertyChanged"/>.
		/// </summary>
		protected override void OnItemPropertyChanged( object sender, SyncfusionPropertyChangedEventArgs e )
		{
			this.ValidateBarStyles( (Bar)sender );
			base.OnItemPropertyChanged( sender, e );
		}
		/// <summary>
		/// 
		/// </summary>
		public virtual void Dispose()
		{
			this.Dispose( true );
			GC.SuppressFinalize( this );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose( bool disposing )
		{
			this.Clear();

			this.manager = null;
		}
		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="newBar"></param>
		private void ValidateBarStyles( Bar newBar )
		{
			// To avoid recursion
			if( validating )
				return;

			// Force RotateWhenVertical when themes are on.
			if( this.manager != null && this.manager is MainFrameBarManager
				&& ( (MainFrameBarManager)this.manager ).ThemesEnabled )
			{
				if( ( newBar.BarStyle & BarStyle.RotateWhenVertical ) <= 0 )
					newBar.BarStyle |= BarStyle.RotateWhenVertical;
			}

			this.validating = true;
			bool newMainMenuBar = ( newBar.BarStyle & BarStyle.IsMainMenu ) > 0;
			bool newStatusBar = ( newBar.BarStyle & BarStyle.IsStatusBar ) > 0;
			if( newStatusBar && newMainMenuBar )
			{
				newBar.BarStyle &= ~BarStyle.IsStatusBar;
				newStatusBar = false;
			}
			for( int i = 0; i < this.Count; i++ )
			{
				Bar bar = this[i];
				if( bar != newBar )
				{
					if( bar.BarName == newBar.BarName //&& bar.GetType() == newBar.GetType()
						// In design-mode names will be initialized later
						&& !( newBar.BarName == String.Empty && newBar.DesignMode ) )
					{
						this.validating = false;

						if( this.IsBarUsedForMerge( newBar ) )
						{
							this.Remove( newBar );
							return;
						}
						else
						{
							throw new Exception( "Bar name " + newBar.BarName + " is not unique." );
						}
					}
					// More than 1 main-menu, prevent new main-menu.
					if( newMainMenuBar && ( bar.BarStyle & BarStyle.IsMainMenu ) > 0 )
						newBar.BarStyle &= ~BarStyle.IsMainMenu;
					// More than 1 status bar, prevent new statusbar.
					if( newStatusBar && ( bar.BarStyle & BarStyle.IsStatusBar ) > 0 )
						newBar.BarStyle &= ~BarStyle.IsStatusBar;
				}
			}
			this.validating = false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="barToFind"></param>
		/// <returns></returns>
		private bool IsBarUsedForMerge( Bar barToFind )
		{
			bool used = false;

			foreach( Bar bar in this )
			{
				MergedBar mBar = bar as MergedBar;
				if( mBar != null )
				{
					if( mBar.mergedBars != null && mBar.mergedBars.Length > 0 )
					{
						for( int i = 0; i < mBar.mergedBars.Length; i++ )
						{
							if( mBar.mergedBars[i] == barToFind )
							{
								used = true;
								break;
							}
						}

						if( used )
							break;
					}
				}
			}

			return used;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bar"></param>
		private void RemoveMergedBar( Bar bar )
		{
			if( bar != null )
			{
				for( int i = 0; i < this.Count; i++ )
				{
					MergedBar mergedBar = this[i] as MergedBar;
					if( mergedBar != null )
					{
						Bar[] mergedBars = mergedBar.mergedBars;

						if( mergedBars != null )
						{
							ArrayList list = new ArrayList( mergedBars );
							if( list.Contains( bar ) )
							{
								if( list.Count > 1 )
								{
									ArrayList barItems = new ArrayList( bar.Items );

									foreach( BarItem barItem in barItems )
									{
										mergedBar.RemoveItem( barItem );
									}

									list.Remove( bar );

									mergedBar.mergedBars = (Bar[])list.ToArray( typeof( Bar ) );
								}
								else base.Remove( mergedBar );
							}
						}
					}
				}
			}
		}
		#endregion
	}

	public interface IBarItemsRepository
	{
		BarItems Items { get; }
		Bars Bars { get; }
		ArrayList Categories { get; }
		IntList CategoriesToIgnoreInCustDialog { get; }
		ImageList ImageList { get; }
		ImageList LargeImageList { get; }
		bool ClearItemsAfterImport { get; }
	}
	public class BarItemsImporter
	{
		protected string miscCategoryRootName = SR.GetString( SR.ShortMiscelaneousText );
		public BarItemsImporter()
		{
		}
		/// <summary>
		/// Move the BarItems from the source to the destination.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		public virtual void ImportItems( IBarItemsRepository source, IBarItemsRepository destination )
		{
			// Move BarItems from the source to this one.

			// First, create a hash of bar items by cateogry name
			Hashtable htItemsByCategory = new Hashtable();
			foreach( BarItem item in source.Items )
			{
				string catName = (string)source.Categories[item.CategoryIndex];
				ArrayList items = htItemsByCategory[catName] as ArrayList;
				if( items == null )
				{
					items = new ArrayList();
					htItemsByCategory[catName] = items;
				}
				items.Add( item );
			}

			// Second, add images from the source to the destination
			// If the destination doesn't have an ImageList then ignore all Images in Source.
			int destImageListCount = 0;
			if( destination.ImageList != null )
				destImageListCount = destination.ImageList.Images.Count;

			bool addedImages = false;

			// Small ImageList...
			if( source.ImageList != null && destination.ImageList != null )
			{
				foreach( Image image in source.ImageList.Images )
				{
					destination.ImageList.Images.Add( image, source.ImageList.TransparentColor );
				}
				addedImages = true;
			}

			// Large ImageList...
			if( source.LargeImageList != null && destination.LargeImageList != null )
			{
				foreach( Image image in source.LargeImageList.Images )
				{
					destination.LargeImageList.Images.Add( image, source.LargeImageList.TransparentColor );
				}
				addedImages = true;
			}

			// Third, move items into the destination, by category
			foreach( string category in htItemsByCategory.Keys )
			{
				int catIndex = this.GetDestinationCategoryIndex( destination, category );

				ArrayList list = htItemsByCategory[category] as ArrayList;
				foreach( BarItem item in list )
				{
					string newID = item.ID;
					// Can add this item to destination?
					while( !destination.Items.IsValidItemID( null, newID ) )
					{
						// Item clashes with an existing one (in ID).
						newID = IDGenerator.GetNextID( newID );
					}
					// So that I can go ahead and change the ID without any problems.
					item.Manager = null;

					item.CategoryIndex = catIndex;

					item.ID = newID;

					if( item.ImageIndex != -1 && addedImages )
						item.ImageIndex += destImageListCount;

					destination.Items.Add( item );
				}
			}

			// Fourth, Add or Merge Bars
			bool mainMenuFound = false;
			foreach( Bar bar in source.Bars )
			{
				// Check if main-menu
				if( !mainMenuFound && ( ( bar.BarStyle & BarStyle.IsMainMenu ) > 0 ) )
				{
					mainMenuFound = true;
					Bar destMainMenu = this.GetMainMenu( destination );
					if( destMainMenu != null )
					{
						// Merge with the existing main-menu
						destMainMenu.MergeItems( bar );
						continue;
					}
					// else let it be added or merged with a toolbar.
				}

				Bar destBar = this.GetBarFromName( bar.BarName, destination );

				if( destBar != null )
				{
					// Bar with same name exists, so merge:
					destBar.MergeItems( bar );
				}
				else
				{
					// No bars to merge with, so just add.
					destination.Bars.Add( bar );
				}
			}
			source.Bars.Clear();
		}
		/// <summary>
		/// Returns the Bar object using the BarName.
		/// </summary>
		/// <param name="barName"></param>
		/// <param name="repository"></param>
		/// <returns></returns>
		protected virtual Bar GetBarFromName( string barName, IBarItemsRepository repository )
		{
			foreach( Bar bar in repository.Bars )
			{
				if( bar.BarName == barName )
					return bar;
			}
			return null;
		}
		/// <summary>
		/// Returns the MainMenu in the BarManager
		/// </summary>
		/// <param name="repository"></param>
		/// <returns></returns>
		protected virtual Bar GetMainMenu( IBarItemsRepository repository )
		{
			foreach( Bar bar in repository.Bars )
			{
				if( ( bar.BarStyle & BarStyle.IsMainMenu ) > 0 )
					return bar;
			}
			return null;
		}
		/// <summary>
		/// Returns the Category Index of a particular Category.If the Category is not available it creates a new Category and returns the Index of the new Category.
		/// </summary>
		/// <param name="destination"></param>
		/// <param name="category"></param>
		/// <returns></returns>
		protected virtual int GetDestinationCategoryIndex( IBarItemsRepository destination, string category )
		{
			int i = -1;
			string srcCategory = category.ToLower();
			foreach( string cat in destination.Categories )
			{
				i++;
				string destCategory = cat.ToLower();
				if( srcCategory == destCategory )
					return i;
			}
			// Didn't find a match there, create a new category
			destination.Categories.Add( category );
			return destination.Categories.Count - 1;
		}
	}

	/// <summary>
	/// Provides data for the <see cref="BarManager.BarControlBindingChanged"/> event.
	/// </summary>
	/// <seealso cref="BarManager.BarControlBindingChanged"/>
	/// <seealso cref="BarManager.OnBarControlBindingChanged"/>
	/// <remarks>
	/// A <b>BarControlBindingChangedArgs</b> specifies which <see cref="Bar"/> object
	/// is being bound/unbound to a <b>control</b> as specified by the <see cref="BarControlBindingChangeType"/>.
	/// </remarks>
	public class BarControlBindingChangedArgs: EventArgs
	{
		private Control barControl;
		private BarControlBindingChangeType changeType;
		private Bar bar;

		public BarControlBindingChangedArgs( Bar bar, Control barControl, BarControlBindingChangeType changeType )
		{
			this.bar = bar;
			this.barControl = barControl;
			this.changeType = changeType;
		}
		/// <summary>
		/// Returns the Bar whose binding was changed.
		/// </summary>
		public Bar Bar
		{
			get { return this.bar; }
		}
		public Control BarControl
		{
			get { return this.barControl; }
		}
		/// <summary>
		/// Returns the nature of change in Control binding.
		/// </summary>
		public BarControlBindingChangeType ChangeType
		{
			get { return this.changeType; }
		}
	}
	/// <summary>
	/// Specifies the nature of change in control binding.
	/// </summary>
	public enum BarControlBindingChangeType
	{
		/// <summary>
		/// The <see cref="Bar"/> component is being unbound from a toolbar control.
		/// </summary>
		Unparenting,
		/// <summary>
		/// The <see cref="Bar"/> component is being bound to a control.
		/// </summary>
		Parented
	}
	internal class DummyForm: Form
	{
		BarManager manager;
		public DummyForm( BarManager manager )
		{
			this.manager = manager;
			manager.Form = this;
			this.ControlBox = false;
			this.FormBorderStyle = FormBorderStyle.None;
			this.Location = new Point( -1000, -1000 );
			this.ShowInTaskbar = false;
			this.Size = new Size( 0, 0 );
			this.StartPosition = FormStartPosition.Manual;
			this.TopLevel = false;
		}
		public BarManager BarManager
		{
			get { return this.manager; }
		}
	}
	/// <summary>
	/// Handles the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager.AfterClone"/> event of the BarManager component in XP Menus framework.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="args">A <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManagerClonedEventArgs"/> that contains the event data.</param>
	public delegate void BarManagerClonedEventHandler( object sender, BarManagerClonedEventArgs args );
	/// <summary>
	/// Provides data for the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarManager.AfterClone"/> event.
	/// </summary>
	public class BarManagerClonedEventArgs: EventArgs
	{
		private BarManager clonedmanager;
		/// <summary>
		/// Creates a new instance of the BarManagerClonedEventArgs type.
		/// </summary>
		/// <param name="clonedmanager">The manager that was cloned.</param>
		public BarManagerClonedEventArgs( BarManager clonedmanager )
		{
			this.clonedmanager = clonedmanager;
		}

		/// <summary>
		/// Returns the BarManager that was just cloned.
		/// </summary>
		public BarManager ClonedBarManager
		{
			get { return this.clonedmanager; }
		}
	}
}
