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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows.Forms;

using Syncfusion.Collections;
using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Runtime.Serialization;
using Syncfusion.Windows.Forms.Tools.Design;
using Syncfusion.Windows.Forms.Tools.XPMenus;
using System.Collections.Generic;
#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary></summary>
	internal class KeyboardActivationHelper :
		IDisposable
	{
		#region Class members
		/// <summary></summary>
		private ArrayList m_mdiChildList;
		/// <summary></summary>
		private int m_currentActiveChildIndex;
		/// <summary></summary>
		private Form m_mdiParent;
		/// <summary></summary>
		private bool m_bIgnoreActivation = false;
		#endregion

		#region Class Initialize/Finalize methods

		/// <summary></summary>
		/// <param name="mdiParent"/>
		internal KeyboardActivationHelper( Form mdiParent )
		{
			m_mdiChildList = new ArrayList();
			//m_currentActiveChildIndex = 0;
			m_mdiParent = mdiParent;
		}

		/// <summary>
		/// Disposes this object.
		/// </summary>
		public void Dispose()
		{
			m_mdiChildList.Clear();
			m_mdiParent = null;
		}

		#endregion

		#region Class utility methods
		/// <summary></summary>
		/// <param name="mdiChild"/>
		internal void AddMdiChild( Form mdiChild )
		{
			if( !m_mdiChildList.Contains( mdiChild ) )
			{
				m_mdiChildList.Add( mdiChild );
			}

			if( mdiChild == m_mdiParent.ActiveMdiChild )
			{
				MoveChild( m_mdiChildList.IndexOf( mdiChild ), 0 );
			}
		}

		/// <summary></summary>
		/// <param name="mdiChild"/>
		internal void RemoveMdiChild( Form mdiChild )
		{
			if( m_currentActiveChildIndex != 0 )
			{
				StopTabBasedActivation();
			}
			m_mdiChildList.Remove( mdiChild );
		}

		/// <summary></summary>
		internal void StartTabBasedActivation()
		{
			if( m_currentActiveChildIndex != 0 )
			{
				StopTabBasedActivation();
			}

		//	m_currentActiveChildIndex = 0;
		}

        int executionsCount = 0;

		/// <summary></summary>
		/// <param name="shiftPressed"/>
		internal void ActivateNext( bool shiftPressed )
		{
			if( m_mdiChildList.Count == 0 || executionsCount > m_mdiChildList.Count)
			{
				return;
			}

			if( shiftPressed )
			{
				m_currentActiveChildIndex--;
			}
			else
			{
				m_currentActiveChildIndex++;
			}

			if( m_currentActiveChildIndex > m_mdiChildList.Count - 1 )
			{
				m_currentActiveChildIndex = 0;
			}
			else if( m_currentActiveChildIndex < 0 )
			{
				m_currentActiveChildIndex = m_mdiChildList.Count - 1;
			}

            Form f = m_mdiChildList[m_currentActiveChildIndex] as Form;
            if (!f.Enabled)
            {
                executionsCount++;
                this.ActivateNext(shiftPressed);
                return;
            }

            m_bIgnoreActivation = true;
            f.Activate();
			m_bIgnoreActivation = false;

            executionsCount = 0;
		}

		/// <summary></summary>
		internal void StopTabBasedActivation()
		{
			// Move the current active child to top of list;
			if( m_currentActiveChildIndex != 0
				&& m_currentActiveChildIndex < m_mdiChildList.Count )
			{
				MoveChild( m_currentActiveChildIndex, 0 );
			}
		}

		/// <summary></summary>
		/// <param name="mdiChild"/>
		internal void BringToFront( Form mdiChild )
		{
			if( m_bIgnoreActivation )
			{
				return;
			}

			if( m_currentActiveChildIndex != 0 )
			{
				//StopTabBasedActivation();
			}

			if( m_mdiChildList.Contains( mdiChild ) )
			{
				m_currentActiveChildIndex = m_mdiChildList.IndexOf(mdiChild);
			}
		}

		/// <summary></summary>
		/// <param name="from"/>
		/// <param name="to"/>
		private void MoveChild( int from, int to )
		{
			Form mdiChild = m_mdiChildList[ from ] as Form;
			m_mdiChildList.RemoveAt( from );
			m_mdiChildList.Insert( to, mdiChild );
		}
		#endregion
	}

	/// <summary></summary>
	[Serializable]
	public class UniquePageID
	{
		#region Class members
		/// <summary></summary>
		private string m_strTabName = null;
		#endregion

		#region Class properties
		/// <summary></summary>
		public string TabName
		{
			get
			{
				return m_strTabName;
			}
			set
			{
				if( value != m_strTabName )
				{
					m_strTabName = value;
				}
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary></summary>
		/// <param name="tabName"/>
		public UniquePageID( string tabName )
		{
			m_strTabName = tabName;
		}
		#endregion
	}

	/// <summary>
	/// Represents the class that manages the tabbed mdi.
	/// </summary>
	/// <remarks><para>The TabbedMDIManager enables a tabbed look and feel for the MDI children in its
	/// MDI client area popularized in Visual Studio.Net(r). This can be accomplished
	/// without having to make any changes to the MDI parent or the MDI child.</para><para>
	/// It also supports horizontal and vertical tab groups and supports dragging the mdi children
	/// into and away from the groups.
	/// </para><para>You can also merge items to the Context Menu provided by the TabbedMDIManager.</para><para>Note that when you attach a TabbedMDIManager to your MDI parent, you should use
	/// the TabbedMDIManager's <see cref="TabbedMDIManager.MdiChildren"/> property instead of the MDI parent's MDIChildren
	/// property. This is because the tabs manager, introduces additional mdi children into the
	/// mdi client that are not part of your application. For the same reason you should also use the <see cref="MdiListMenuItem"/> property
	/// to auto-insert mdi child windows into a <see cref="System.Windows.Forms.MenuItem"/> instead of the <see cref="System.Windows.Forms.MenuItem.MdiList"/> property.
	/// This however, is not necessary when you use XPMenus.</para><para>The TabbedMDIManager also automatically stores the user's preferences in the isolated storage with respect
	/// to the tab group alignment, number of tab groups and their sizes. Note that this persisted state is reapplied
	/// on the existing mdi children, the next time you call <see cref="AttachToMdiContainer"/>. So, this pattern requires 
	/// you to instantiate all the mdi children before you call AttachToMdiContainer in your app.</para><para>Take a look at <see cref="Syncfusion.Windows.Forms.Tools.TabbedMDIManager.MdiListMenuItem"/>
	/// for some sample codes that deals with issues like displaying the mdi children in the MDI Windows menu, etc.</para><para>
	/// You can optionally, programmatically control the number of tab groups and which tab group
	/// a form gets associated with using the <see cref="MoveActiveDocTo"/>, and other methods.
	/// </para><para>
	/// The <see cref="TabHost"/> is the form that hosts each tab group. The <see cref="MDITabPanel"/>
	/// is the tab control that is used to draw the tab group.
	/// </para></remarks>
	/// <example>
	/// Initializing the tabbed MDI layout is simple. In your Form Load handler:
	/// <code lang="C#">
	/// this.tabbedMDIManager = new TabbedMDIManager();
	/// this.tabbedMDIManager.AttachToMdiContainer(this);
	/// </code><code lang="VB">
	/// Me.tabbedMDIManager = New TabbedMDIManager()
	/// Me.tabbedMDIManager.AttachToMdiContainer(Me)
	/// </code></example>
	[
	Description( "Represents the class that manages the tabbed mdi." ),
	ToolboxItem( true ),
	ToolboxBitmap( typeof( TabbedMDIManager ), "ToolboxIcons.TabbedMDIManager.bmp" ),
	Designer( typeof( TabbedMDIManager.TabbedMDIManagerDesigner ) )
	]
	public class TabbedMDIManager :
		Component,
		IDisposable,
		IMessageFilter,
		IKeyboardProcHookClient,
		ITabbedMDIManager
	{
		#region Constants
		/// <summary></summary>
		private const string TAB_GROUP_INFO = "TabGroupInfo";
		/// <summary></summary>
		private const int DEF_IMAGE_SIZE = 16;
		/// <summary></summary>
		private const string DEF_STATE_ID_FORMAT = "{0}.{1}";
		/// <summary></summary>
		internal static readonly int SPLITTER_WIDTH = 5;
		/// <summary>
		/// Minimum TabHost width when Splitter is moving.
		/// </summary>
		private const int MINIMUM_TABHOST_WIDTH = 5;
		/// <summary>
		/// Minimum TabHost height when Splitter is moving.
		/// </summary>
		private const int MINIMUM_TABHOST_HEIGHT = 5;
		#endregion

		#region Class static members
		/// <summary></summary>
		private static Hashtable m_htManagers = new Hashtable();
		#endregion

		#region Fields
		/// <summary></summary>
		private bool themesEnabled = false;
		/// <summary></summary>
		private bool allowTabGroupCustomizing;
		/// <summary></summary>
		private Form mdiContainer;
		/// <summary></summary>
		private TabHost activeTabHost;
		/// <summary></summary>
		private ArrayListExt tabHostList;
		/// <summary></summary>
		private ArrayListExt splitterHostList;
		/// <summary></summary>
		internal MdiClient m_mdiClient;
		/// <summary></summary>
		private ImageList imageList;
		/// <summary></summary>
		internal int autoScaleBaseMdiClientDim = 0;
		/// <summary></summary>
		private int suspendCount = 0;
		/// <summary></summary>
		private ParentBarItem contextMenuItem;
		/// <summary></summary>
		private ParentBarItem defaultContextMenuItem;
		/// <summary></summary>
		private PopupMenu contextMenu;
		/// <summary></summary>
		private PopupMenusManager popupManager;
		/// <summary></summary>
		private bool showDragMenu = false;
		/// <summary></summary>
		internal KeyboardActivationHelper keyboardActivationHelper = null;
		/// <summary></summary>
		private MenuItem mdiListMenuItem;
		/// <summary></summary>
		private bool resetLayout = true;
		/// <summary></summary>
		private bool useIconsInTabs = true;
		/// <summary></summary>
		private Hashtable mdiChildrenTooltips = null;
		/// <summary></summary>
		private string id;
		/// <summary></summary>
		private bool causesFormValidation = false;
		/// <summary></summary>
		private Color closeButtonColor = Color.Black;
		/// <summary>
		/// Close button visible.
		/// </summary>
		private bool m_bCloseButtonVisible = true;
		/// <summary>
		/// Indicates whether the control is displayed.
		/// </summary>
		private bool m_bVisible = true;
		/// <summary>
		/// Indicates whether object was disposed.
		/// </summary>
		private bool bDisposed;
		/// <summary></summary>
		private BarItem closeMenuItem;
		/// <summary></summary>
		private BarItem cancelMenuItem;
		/// <summary></summary>
		private BarItem newHorzItem;
		/// <summary></summary>
		private BarItem newVertItem;
		/// <summary></summary>
		private BarItem movePrevItem;
		/// <summary></summary>
		private BarItem moveNextItem;
		/// <summary></summary>
		private bool closeOnMiddleButtonClick = false;
		/// <summary>
		/// The visibility of the drop down button.
		/// </summary>
		private bool m_bDropDownButtonVisible = false;
		/// <summary></summary>
		private bool m_bNeedUpdateHostedForm = true;
		// used to restore previous closeItem state, after DragMenu is shown.
		/// <summary></summary>
		private bool m_bOldCloseItemVisibility = false;
		/// <summary></summary>
		protected string m_strNewGroupName = null;
		/// <summary></summary>
		private Size m_imageSize = new Size( DEF_IMAGE_SIZE, DEF_IMAGE_SIZE );
		/// <summary></summary>
		private bool m_bIsDisposing = false;
		/// <summary></summary>
		private string m_strTabStyleName = TabRenderer2D.TabStyleName;
		/// <summary></summary>
		private bool m_bShowCloseButton = false;
		/// <summary></summary>
		private bool m_bShowCloseButtonForActiveTabOnly = false;
		/// <summary></summary>
		private Hashtable m_htPagesToSkip = new Hashtable();
		/// <summary></summary>
		private bool m_bShouldConsiderPageSkip = false;
		/// <summary></summary>
		private bool m_bSearchFromStart = false;
		/// <summary></summary>
		private ArrayList m_arrNativeWindowsToRelease = new ArrayList();
		/// <summary></summary>
		private bool dragging = false;
		/// <summary></summary>
		private Rectangle dropRect = Rectangle.Empty;
		/// <summary></summary>
		private ArrayList mdiChildMenuList = new ArrayList();
		/// <summary></summary>
		private Hashtable mdiChildrenByMenuItems = new Hashtable();
		/// <summary>
		/// Panel that contains layout data.
		/// </summary>
		private LayoutPanel m_lpPanel = null;
		/// <summary>
		/// Uses for backward compatibility.
		/// </summary>
		private bool m_bHorizontalAlignment = true;
		/// <summary></summary>
		internal bool shouldKeepImageIndex = false;
		/// <summary></summary>
		private bool allowMDIClientLocking = true;
		/// <summary>
		/// True - indicates that TabbedMDIManager mode is on, false - TabbedMDIManager mode is off.
		/// </summary>
		private bool m_bIsTabbedMDIModeOn = false;
		/// <summary>
		/// Reference on control to which we attach a TabbedMDIManager. </summary>
		/// </summary>
		private Form m_frmParent = null;
		/// <summary>
		/// Hash table of MDIChild forms and corresponding wrappers of SublassHWMD type,
		/// where MDIChild form is key and SublassHWMD instance is value.
		/// </summary>
		public Hashtable m_htChildForms = new Hashtable();
		/// <summary>
		/// Native window subclass for MDI container form.
		/// </summary>
		MdiParentNativeWindow m_mdiContainerSubclass;
        /// <summary>
        /// Indicates whether BeforeMDIChild event should be raised.
        /// </summary>
        private bool m_bShouldRaiseBeforeMDIChildEvent = true;
        private bool closed = false;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the visibility of the close button.
		/// </summary>
		[
		DefaultValue( true ),
		Description( "Gets or sets close button visible." )
		]
		public bool CloseButtonVisible
		{
			get
			{
				return this.m_bCloseButtonVisible;
			}
			set
			{
				if( this.m_bCloseButtonVisible != value )
				{
					this.m_bCloseButtonVisible = value;
					this.UpdateCloseAndDropDownButtons();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether tabs should be closed on middle button click.
		/// </summary>
		/// <value><c>true</c> if to be closed on middle button click; otherwise, <c>false</c>.
		/// </value>
		[
		DefaultValue( false ),
		Description( "Gets or sets a value indicating whether tabs should be closed on middle button click." )
		]
		public bool CloseOnMiddleButtonClick
		{
			get
			{
				return this.closeOnMiddleButtonClick;
			}
			set
			{
				if( this.closeOnMiddleButtonClick != value )
				{
					this.closeOnMiddleButtonClick = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the visibility of the drop down button.
		/// </summary>
		[
		DefaultValue( false ),
		Description( "Gets or sets the visibility of the drop down button." )
		]
		public bool DropDownButtonVisible
		{
			get
			{
				return m_bDropDownButtonVisible;
			}
			set
			{
				if( m_bDropDownButtonVisible != value )
				{
					m_bDropDownButtonVisible = value;
					this.UpdateCloseAndDropDownButtons();
				}
			}
		}

		/// <summary>
		/// Indicates whether the control is displayed.
		/// </summary>
		[
		DefaultValue( true ),
		Description( "Gets or sets a value indicating whether the control is displayed." )
		]
		public bool Visible
		{
			get
			{
				return this.m_bVisible;
			}
			set
			{
				if( this.m_bVisible != value )
				{
					this.m_bVisible = value;

					if( this.MdiClient != null )
					{
						this.MdiClient.PerformLayout();
					}
				}
			}
		}

		/// <summary>
		/// Gets or Sets update for HostedForm
		/// </summary>
        [DefaultValue(true)]
        [Description("Gets or Sets value indicating whether to update HostedForm")]
		public bool NeedUpdateHostedForm
		{
			get
			{
				return m_bNeedUpdateHostedForm;
			}
			set
			{
				if( value == m_bNeedUpdateHostedForm )
				{
					return;
				}

				m_bNeedUpdateHostedForm = value;
				OnNeedUpdateHostedFormChanged();
			}
		}
		/// <summary>
		/// Indicates whether the active child form will be validated before activating
		/// a new child form.
		/// </summary>
		/// <value>
		/// True to perform validation before switching the active child form; false otherwise. Default is false.
		/// </value>
		/// <remarks><para>
		/// The default mdi behavior is to let you switch child forms even if validation
		/// fails for the active form. This is the default behavior in the TabbedMDIManager as well.</para><para>
		/// When this property is turned on and if validation failed on the active form, 
		/// the user cannot click on a tab or use Ctrl+Tab keys to activate a new page.
		/// However, note that the active child form can still be changed programmatically.
		/// </para></remarks>
		[
		Description( "Specifies whether or not the active child form will be validated before activating a new child form." ),
		DefaultValue( false )
		]
		public bool CausesFormValidation
		{
			get
			{
				return this.causesFormValidation;
			}
			set
			{
				this.causesFormValidation = value;
			}
		}
		private PopupMenu ContextMenu
		{
			get
			{
				if (contextMenu == null)
				{
					this.contextMenu = new PopupMenu();

					InitContextMenu();
				}

				return contextMenu;
			}
			set { contextMenu = value; }
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		private void InitContextMenu()
		{
			// Init ContextMenu
			ParentBarItem defaultItem = new ParentBarItem();

			defaultItem.Items.Add(this.CloseItem);
			defaultItem.Items.Add(this.CancelItem);
			defaultItem.Items.Add(this.NewHorzGroupItem);
			defaultItem.BeginGroupAt(this.NewHorzGroupItem);
			defaultItem.Items.Add(this.NewVertGroupItem);
			defaultItem.Items.Add(this.MovePrevGroupItem);
			defaultItem.Items.Add(this.MoveNextGroupItem);
			defaultItem.BeginGroupAt(this.MovePrevGroupItem);

			this.defaultContextMenuItem = defaultItem;

			this.ContextMenuItem = new ParentBarItem(); // This will force the above defaultContextMenuItem to be Merged into the ContextMenu
		}
		/// <summary>
		/// Gets or sets the context menu that will be used along with the default tab context menu
		/// when the user right-clicks on a tab.
		/// </summary>
		/// <value>A ContextMenu instance.</value>
		/// <remarks><para>TabbedMDIManager uses our XP Menus classes for its context menu.</para><para>Using this you can add additional menu items to the context menu that pops up
		/// when a user clicks on a tab.</para><para>This menu will be merged with the default tab context menu. The items
		/// in the default tab menu have a merge order of 10. Using the appropriate merge
		/// order you can insert the custom items before or after the default items.</para></remarks>
		/// <example>
		/// This example adds a custom context menu to the tabbed mdi manager.
		/// <coderef file="\Tools\Samples\Tabbed MDI Package\TabbedMDI\cs\mainform.cs" name="TabbedMDIManager: Setting the Context Menu" lang="C#"><code lang="C#">
		///             // Append menus to the standard mdi tab context menu
		///             ParentBarItem contextMenuItem = new ParentBarItem();
		///             BarItem newDocItem = new BarItem();
		///             newDocItem.Click += new System.EventHandler(this.addDoc1_Click);
		///             newDocItem.Text = "Custom Item: Insert New Doc";
		///             newDocItem.MergeOrder = 30;
		///             contextMenuItem.Items.Add(newDocItem);
		///             BarItem exitItem = new BarItem();
		///             exitItem.Click += new System.EventHandler(this.FileExit_Clicked);
		///             exitItem.Text = "CustomItem: Exit";
		///             exitItem.MergeOrder = 30;
		///             contextMenuItem.Items.Add(exitItem);
		///             contextMenuItem.BeginGroupAt(newDocItem);
		///             // Items in this ParentBarItem will be merged with the standard context menu ParentBarItem of the mdi tab.
		///             tabbedMDIManager.ContextMenuItem = contextMenuItem;</code></coderef><coderef file="\Tools\Samples\Tabbed MDI Package\TabbedMDI\vb\mainform.vb" name="TabbedMDIManager: Setting the Context Menu" lang="VB"><code lang="VB">
		///            ' Append menus to the standard mdi tab context menu
		///            Dim contextMenuItem As ParentBarItem
		///            contextMenuItem = New ParentBarItem()
		///            Dim newDocItem As BarItem
		///            newDocItem = New BarItem()
		///            AddHandler newDocItem.Click, New System.EventHandler(AddressOf addDoc1_Click)
		///            newDocItem.Text = "Custom Item: Insert New Doc"
		///            newDocItem.MergeOrder = 30
		///            contextMenuItem.Items.Add(newDocItem)
		///            Dim exitItem As BarItem
		///            exitItem = New BarItem()
		///            AddHandler exitItem.Click, New System.EventHandler(AddressOf FileExit_Clicked)
		///            exitItem.Text = "CustomItem: Exit"
		///            exitItem.MergeOrder = 30
		///            contextMenuItem.Items.Add(exitItem)
		///            contextMenuItem.BeginGroupAt(newDocItem)
		///            ' Items in this ParentBarItem will be merged with the standard context menu ParentBarItem of the mdi tab.
		///            tabbedMDIManager.ContextMenuItem = contextMenuItem</code></coderef></example>
		[Description( "Represents the context menu that will be used along with the default tab context menu when the user right-clicks on a tab." )]
		public ParentBarItem ContextMenuItem
		{
			get
			{
				if (contextMenuItem == null)
				{
					InitContextMenu();
				}
				return contextMenuItem;
			}
			set
			{
				if( contextMenuItem != null )
				{
					contextMenuItem.Popup -= new EventHandler( ContextMenu_Popup );
					contextMenuItem.PopupClosed -= new EventHandler( ContextMenu_PopupClosed );
					contextMenuItem.BeforePopup -= new CancelEventHandler( ContextMenu_BeforePopup );
				}

				contextMenuItem = value;
                contextMenuItem.MergeItems( this.DefaultContextMenuItem );

				if( contextMenuItem != null )
				{
					contextMenuItem.Popup += new EventHandler( ContextMenu_Popup );
					contextMenuItem.PopupClosed += new EventHandler( ContextMenu_PopupClosed );
					contextMenuItem.BeforePopup += new CancelEventHandler( ContextMenu_BeforePopup );
				}

				ContextMenu.ParentBarItem = contextMenuItem;
			}
		}
        /// <summary>
        /// Gets the Defaultcontext menu 
        /// </summary>
        private ParentBarItem DefaultContextMenuItem
        {
            get
            {
                if (defaultContextMenuItem == null)
                {
                    InitContextMenu();
                }
                return defaultContextMenuItem;
            }
        }

		/// <summary>
		/// Returns the mdi children of the associated mdi parent.
		/// </summary>
		/// <value>An array of forms containing the mdi children.</value>
		/// <remarks>
		/// Use this property instead of accessing the mdi parent's MDIChildren property
		/// to get a list of mdi children. This is necessary because the TabbedMDIManager inserts
		/// additional mdi children that your application need not and should not access/modify.
		/// </remarks>
		/// <example><coderef file="\Tools\Samples\Tabbed MDI Package\TabbedMDI\cs\mainform.cs" name="TabbedMDIManager: Getting the mdi children" lang="C#"><code lang="C#">
		///             foreach(Form form in this.tabbedMDIManager.MdiChildren)
		///             {
		///                 children += form.Text + "\r\n";
		///             }</code></coderef><coderef file="\Tools\Samples\Tabbed MDI Package\TabbedMDI\vb\mainform.vb" name="TabbedMDIManager: Getting the mdi children" lang="VB"><code lang="VB">
		///            Dim form As Form
		///            For Each form In Me.tabbedMDIManager.MdiChildren
		///            ' Process form
		///            Next</code></coderef></example>
		[
		Description( "Represents the mdi children of the associated mdi parent." ),
		Browsable( false )
		]
		public Form[] MdiChildren
		{
			get
			{
				if( this.mdiContainer == null )
				{
					return new Form[] { };
				}

				ArrayList actualMdiChildren = new ArrayList();
				foreach( Form form in this.mdiContainer.MdiChildren )
				{
					if( form is SplitterHost || form is TabHost )
					{
						continue;
					}

					actualMdiChildren.Add( form );
				}
				Form[] forms = new Form[ actualMdiChildren.Count ];


				for( int i = 0; i < forms.Length; i++ )
				{
					Form form = actualMdiChildren[ i ] as Form;
					forms[ i ] = form;
				}
				return forms;
			}
		}

		/// <summary>
		/// Gets or sets the close button color.
		/// </summary>
		[Description( "Specifies the color of extreme right close button." )]
		[DefaultValue( typeof( Color ), "Black" )]
		public Color CloseButtonColor
		{
			get
			{
				return closeButtonColor;
			}
			set
			{
				closeButtonColor = value;
				MdiHostRefersh();
			}
		}

		/// <summary>
		/// Returns an array of <see cref="TabHost"/> instances that contains 
		/// the tab control(see <see cref="MDITabPanel"/>) used to draw a tab group.
		/// </summary>
		[
		Description( "Returns an array of TabHost instances that contains the tab control used to draw a tab group" ),
		Browsable( false )
		]
		public TabHost[] TabGroupHosts
		{
			get
			{
				TabHost[] hosts = new TabHost[ this.tabHostList.Count ];
				for( int i = 0; i < hosts.Length; i++ )
				{
					hosts[ i ] = tabHostList[ i ] as TabHost;
				}

				return hosts;
			}
		}

		/// <summary>
		/// Returns the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/> representing
		/// the "Close" menu item.
		/// </summary>
		[Description( "Represents the 'Close' menu item on the tab context menu." )]
		public virtual BarItem CloseItem
		{
			get
			{
				if( this.closeMenuItem == null )
				{
					this.closeMenuItem = new BarItem( SR.GetString( SR.CloseMenuItemText,this ), new EventHandler( this.ContextMenu_Close ) );
					this.closeMenuItem.MergeOrder = 10;
				}

				return this.closeMenuItem;
			}
		}
		/// <summary>
		/// Gets or sets the size of the image in mdi tabs.
		/// </summary>
		[Description( "Specifies the size of the image on tabs." )]
		public Size ImageSize
		{
			get
			{
				return m_imageSize;
			}
			set
			{
				if( value != m_imageSize )
				{
					m_imageSize = value;
					OnImageSizeChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the tabs to be painted as 2D, 3D(regular), WorkbookMode or other registered tab types.
		/// </summary>
		[
		Description( @"Indicates whether the tabs are painted as 2D, 3D(regular), WorkbookMode or other registered tab types." ),
		DefaultValue( typeof( TabRenderer2D ) ),
		Category( @"Appearance" ),
		Editor( typeof( TabStyleEditor ), typeof( UITypeEditor ) ),
		TypeConverter( typeof( TabStyleConverter ) ),
		]
		public virtual Type TabStyle
		{
			get
			{
				return ReflectionHelper.GetTabTypeFromName( m_strTabStyleName );
			}

			set
			{
				// Get the tabstyle from TabStyleName static property of the type (not the factory).
				this.m_strTabStyleName = ReflectionHelper.GetTabNameFromType( value );

				UpdateRenderers();
			}
		}

		/// <summary>
		/// Gets or Sets, show close button for tab only when mouse is over it.
		/// This property will work only if <see cref="TabbedMDIManager.ShowCloseButton"/> property is set to true.
		/// </summary>
		[
		DefaultValue( false ),
		Description( "Indicates, show close button for tab only when mouse is over it." ),
		Category( "Appearance" )
		]
		public bool ShowCloseButtonForActiveTabOnly
		{
			get
			{
				return m_bShowCloseButtonForActiveTabOnly;
			}
			set
			{
				if( value != m_bShowCloseButtonForActiveTabOnly )
				{
					m_bShowCloseButtonForActiveTabOnly = value;

					// Update all existing tab controls
					foreach( TabHost tabHost in this.tabHostList )
					{
						tabHost.MDITabPanel.ShowCloseButtonForActiveTabOnly = value;
					}
				}
			}
		}


        private bool showCloseButtonBackColor = false;
        /// <summary>
        /// Gets or Sets, show close button back color.
        /// </summary>
        [
        DefaultValue(false),
        Description("Indicates, show close button back color."),
        Category("Appearance")
        ]
        public  bool ShowCloseButtonBackColor
        {
            get
            {
                return showCloseButtonBackColor;
            }
            set
            {
                showCloseButtonBackColor = value;
            }
        }
        private Color closeButtonBackColor = Color.White;
        /// <summary>
        /// Gets or Sets, close button back color.
        /// </summary>
        [
        Description("Indicates, close button back color."),
        Category("Appearance")
        ]
        public Color CloseButtonBackColor
        {
            get
            {
                return closeButtonBackColor;
            }
            set
            {
                closeButtonBackColor = value;
            }
        }
        /// <summary>
        /// Collection of forms which needs to show Close button.
        /// </summary>
		internal   List<Form> childForms = new List<Form>();
		public void ShowCloseButtonForForm(Form form, bool showCloseButtonForForm)
		{
			if (showCloseButtonForForm)
				childForms.Add(form);
			else
				childForms.Remove(form);
			this.OnShowCloseButtonChanged();
		}
		/// <summary>
		/// Gets or Sets, show close button for individual tabs or not.
		/// </summary>
		[
		DefaultValue( false ),
		Description( "Specifies whether to show close button for individual tabs." )
		]
		public bool ShowCloseButton
		{
			get
			{
				return m_bShowCloseButton;
			}
			set
			{
				if( value != m_bShowCloseButton )
				{
					m_bShowCloseButton = value;

					OnShowCloseButtonChanged();
				}
			}
		}

		/// <summary>
		/// Returns the current mdi parent form managed.
		/// </summary>
		[Description( "Specifies the current mdi parent form managed." )]
		[Browsable( false )]
		public Form MdiParent
		{
			get
			{
				return mdiContainer;
			}
		}

		/// <summary>
		/// Indicates whether to use Icons in tabs.
		/// </summary>
		/// <value>True to use icons; false otherwise. Default is true.</value>
		/// <remarks>When true, the Tabs will get the Icon from the mdi child form's Icon property.
		/// </remarks>
		[
		DefaultValue( true ),
		Description( "Specifies whether to use Icons in tabs." )
		]
		public bool UseIconsInTabs
		{
			get
			{
				return this.useIconsInTabs;
			}
			set
			{
				if( this.useIconsInTabs != value )
				{
					this.useIconsInTabs = value;
					if( this.MdiClient != null )
					{
                        this.calcTabBounds();
						this.MdiClient.PerformLayout();
					}
				}
			}
		}
        private void calcTabBounds()
        {
            // Update all existing tab controls
            foreach (TabHost tabHost in this.tabHostList)
            {
                tabHost.MDITabPanel.ComputeTabPanelBoundsInternal();
            }
        }        
		/// <summary>
		/// Indicates whether the user can drag and drop tabs(child forms) from one tab group to another.
		/// </summary>
		/// <value>True to allow the user to customize the tab group settings; false otherwise.</value>
		/// <remarks><para>
		/// If this property is true, the user will be allowed to create new tab groups and move 
		/// tabs (child forms) between tab groups through the context menus and simple drag and drop.
		/// </para><para>
		/// If false, the creation and sizes of the tab groups can only be set programmatically.
		/// </para><para>
		/// This property also determines whether the tab group's settings are persisted for use
		/// across application instantiation. The state will be persisted only if this is set to true.
		/// </para></remarks>
		[
		DefaultValue( true ),
		Category( "Behavior" ),
		Description( "Specifies whether the user can drag and drop tabs(child forms) from one tab group to another." )
		]
		public bool AllowTabGroupCustomizing
		{
			get
			{
				return this.allowTabGroupCustomizing;
			}
			set
			{
				this.allowTabGroupCustomizing = value;
			}
		}
		/// <summary>
		/// Specifies whether the MDI Client will be locked when certain tasks are performed.
		/// </summary>
		[
		DefaultValue( true ),
		Category( "Behavior" ),
		Description( "Specifies whether the MDI Client will be locked when certain tasks are performed." ),
		Browsable( false )
		]
		public bool AllowMDIClientLocking
		{
			get
			{
				return this.allowMDIClientLocking;
			}
			set
			{
				this.allowMDIClientLocking = value;
			}
		}

		/// <summary>
		/// Gets or sets a unique ID to differentiate different instances of this class.
		/// </summary>
		/// <remarks>
		/// The runtime persisted information of this class will be scoped by this ID.
		/// </remarks>
		[
		DefaultValue( "" ),
		Category( "ID" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public string ID
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		/// <summary>
		/// Indicates whether the tabs should be drawn XP themed.
		/// </summary>
		/// <value>True to draw themed; false otherwise. Default is false.</value>
		[
		DefaultValue( false ),
		Category( "Appearance" ),
		Description( "Specifies whether the tabs should be drawn themed." )
		]
		public bool ThemesEnabled
		{
			get
			{
				return this.themesEnabled;
			}
			set
			{
				if( this.themesEnabled != value )
				{
					this.themesEnabled = value;

					UpdateRenderers();
				}
			}
		}

		/// <summary>
		/// Gets or sets the menu item to which the MDI Children list should be added.
		/// </summary>
		/// <value>The <see cref="System.Windows.Forms.MenuItem"/> to which the list should be added.</value>
		/// <remarks><para>Use this property instead of the <see cref="System.Windows.Forms.MenuItem.MdiList"/> property.</para><para>This is necessary because the tabbed mdi manager inserts additional
		/// mdi children that your user should not and need not be aware about.</para><para>
		/// Note that when you use XP Menus in Essential Tools as your mdi container's main-menu
		/// then this property need not be set. You should instead use the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.MdiListBarItem"/> 
		/// in XP Menus to represent the mdi child windows list. XP Menus framework automatically handles
		/// the case when the mdi child windows layout is managed by the TabbedMDIManager.
		/// </para></remarks>
		/// <example>
		/// The following example shows how to initialize the TabbedMDIManager with a form that
		/// is an mdi container. It also sets a menu to be an mdi list (using the TabbedMDIManager.MdiListMenuItem
		/// property) and adds custom entries to the default tab context menu.
		/// <coderef file="\Tools\Samples\Tabbed MDI Package\TabbedMDI\cs\mainform.cs" name="Initializing TabbedMDIManager" lang="C#"><code lang="C#">
		///        public MainForm() {
		///            //
		///            // Required for Windows Form Designer support
		///            //
		///            InitializeComponent();
		///             tabbedMDIManager = new TabbedMDIManager();
		///             //tabbedMDIManager.UseIconsInTabs = false;
		///            //Add Window Menu
		///            this.miWindow = mainMenu.MenuItems.Add("Window");
		///            miWindow.MergeOrder = 10;
		///            miWindow.MenuItems.Add("Cascade", new System.EventHandler(this.WindowCascade_Clicked));
		///            miWindow.MenuItems.Add("Tile Horizontal", new System.EventHandler(this.WindowTileH_Clicked));
		///            miWindow.MenuItems.Add("Tile Vertical", new System.EventHandler(this.WindowTileV_Clicked));
		///            miWindow.MenuItems.Add("MDI Tabbed", new System.EventHandler(this.TabbedWindows_Clicked));
		///       // Let the TabbedMDIManager insert the Mdi Child windows list
		///       this.tabbedMDIManager.MdiListMenuItem = miWindow;
		///       
		///             // Append menus to the standard mdi tab context menu
		///             ParentBarItem contextMenuItem = new ParentBarItem();
		///             BarItem newDocItem = new BarItem();
		///             newDocItem.Click += new System.EventHandler(this.addDoc1_Click);
		///             newDocItem.Text = "Custom Item: Insert New Doc";
		///             newDocItem.MergeOrder = 30;
		///             contextMenuItem.Items.Add(newDocItem);
		///             BarItem exitItem = new BarItem();
		///             exitItem.Click += new System.EventHandler(this.FileExit_Clicked);
		///             exitItem.Text = "CustomItem: Exit";
		///             exitItem.MergeOrder = 30;
		///             contextMenuItem.Items.Add(exitItem);
		///             contextMenuItem.BeginGroupAt(newDocItem);
		///             // Items in this ParentBarItem will be merged with the standard context menu ParentBarItem of the mdi tab.
		///             tabbedMDIManager.ContextMenuItem = contextMenuItem;
		///        }
		///         // Convenient way to toggle TabbedMDI mode.
		///         private bool TabbedMDIOn
		///         {
		///             get    {    return this.tabWindowsOn;    }
		///             set
		///             {
		///                 if(!(this.tabWindowsOn == value))
		///                 {
		///                     this.tabWindowsOn = value;
		///                     if(this.tabWindowsOn)
		///                     {
		///                         this.tabbedMDIManager.AttachToMdiContainer(this);
		///                     }
		///                     else
		///                     {
		///                         this.tabbedMDIManager.DetachFromMdiContainer(this, false); // false to not invoke the Cascade mode after detaching.
		///                     }
		///                 }
		///             }
		///         }
		///        //Add a document
		///        private void AddDocument(Form doc) {
		///            doc.MdiParent = this;
		///            doc.Show();
		///        }
		///         private void MainForm_Load(object sender, System.EventArgs e)
		///         {
		///             // Add 4 documents
		///             this.addDoc1_Click(this, EventArgs.Empty);
		///             this.addDoc1_Click(this, EventArgs.Empty);
		///             this.addDoc1_Click(this, EventArgs.Empty);
		///             this.addDoc1_Click(this, EventArgs.Empty);
		///             // Turn on MDI Tabbed Documents mode.
		///             // Call this after loading the mdi children to restore their previous state.
		///             this.TabbedMDIOn = true;
		///         }
		///         private int document1Count = 0 ;
		///         private void addDoc1_Click(object sender, System.EventArgs e)
		///         {
		///             document1Count++ ;
		///             Document1 doc = new Document1("DocumentOne " + document1Count.ToString());
		///             AddDocument(doc);
		///         }
		///        //Window-&gt;Cascade Menu item handler
		///        protected void WindowCascade_Clicked(object sender, System.EventArgs e) {
		///             this.TabbedMDIOn = false;
		///            this.LayoutMdi(MdiLayout.Cascade);
		///        }</code></coderef><coderef file="\Tools\Samples\Tabbed MDI Package\TabbedMDI\vb\mainform.vb" name="Initializing TabbedMDIManager" lang="VB"><code lang="VB">
		///        Public Sub New()
		///            MyBase.New()
		///            '
		///            ' Required for Windows Form Designer support
		///            '
		///            InitializeComponent()
		///            tabbedMDIManager = New TabbedMDIManager()
		///            'tabbedMDIManager.UseIconsInTabs = false;
		///            'Add Window Menu
		///            Me.miWindow = mainMenu.MenuItems.Add("Window")
		///            miWindow.MergeOrder = 10
		///            miWindow.MenuItems.Add("Cascade", New System.EventHandler(AddressOf WindowCascade_Clicked))
		///            miWindow.MenuItems.Add("Tile Horizontal", New System.EventHandler(AddressOf WindowTileH_Clicked))
		///            miWindow.MenuItems.Add("Tile Vertical", New System.EventHandler(AddressOf WindowTileV_Clicked))
		///            miWindow.MenuItems.Add("MDI Tabbed", New System.EventHandler(AddressOf TabbedWindows_Clicked))
		///            
		///            ' Let the TabbedMDIManager insert the Mdi Child windows list
		///        Me.tabbedMDIManager.MdiListMenuItem = miWindow
		///        
		///            ' Append menus to the standard mdi tab context menu
		///            Dim contextMenuItem As ParentBarItem
		///            contextMenuItem = New ParentBarItem()
		///            Dim newDocItem As BarItem
		///            newDocItem = New BarItem()
		///            AddHandler newDocItem.Click, New System.EventHandler(AddressOf addDoc1_Click)
		///            newDocItem.Text = "Custom Item: Insert New Doc"
		///            newDocItem.MergeOrder = 30
		///            contextMenuItem.Items.Add(newDocItem)
		///            Dim exitItem As BarItem
		///            exitItem = New BarItem()
		///            AddHandler exitItem.Click, New System.EventHandler(AddressOf FileExit_Clicked)
		///            exitItem.Text = "CustomItem: Exit"
		///            exitItem.MergeOrder = 30
		///            contextMenuItem.Items.Add(exitItem)
		///            contextMenuItem.BeginGroupAt(newDocItem)
		///            ' Items in this ParentBarItem will be merged with the standard context menu ParentBarItem of the mdi tab.
		///            tabbedMDIManager.ContextMenuItem = contextMenuItem
		///        End Sub
		///        ' Convenient way to toggle TabbedMDI mode.
		///        Property TabbedMDIOn() As Boolean
		///            Get
		///                Return Me.tabWindowsOn
		///            End Get
		///            Set(ByVal Value As Boolean)
		///                If (Not (Me.tabWindowsOn = Value)) Then
		///                    Me.tabWindowsOn = Value
		///                    If Me.tabWindowsOn Then
		///                        Me.tabbedMDIManager.AttachToMdiContainer(Me)
		///                    Else
		///                        Me.tabbedMDIManager.DetachFromMdiContainer(Me, False)
		///                    End If
		///                End If
		///            End Set
		///        End Property
		///        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
		///            MyBase.Dispose(disposing)
		///            If (Not (components) Is Nothing) Then
		///                components.Dispose()
		///            End If
		///        End Sub
		///        Private Sub AddDocument(ByVal doc As Form)
		///            doc.MdiParent = Me
		///            doc.Show()
		///        End Sub
		///        Private Sub MainForm_Load(ByVal sender As Object, ByVal e As EventArgs)
		///            ' Add 4 documents
		///            Me.addDoc1_Click(Me, EventArgs.Empty)
		///            Me.addDoc1_Click(Me, EventArgs.Empty)
		///            Me.addDoc1_Click(Me, EventArgs.Empty)
		///            Me.addDoc1_Click(Me, EventArgs.Empty)
		///            ' Turn on MDI Tabbed Documents mode.
		///            ' Call this after loading the mdi children to restore their previous state.
		///            Me.TabbedMDIOn = True
		///        End Sub
		///        Private Sub addDoc1_Click(ByVal sender As Object, ByVal e As EventArgs)
		///            document1Count = (document1Count + 1)
		///            Dim doc As Document1
		///            doc = New Document1(("DocumentOne " + document1Count.ToString))
		///            AddDocument(doc)
		///        End Sub
		///        ' Window-&gt;Cascade Menu item handler
		///        Protected Sub WindowCascade_Clicked(ByVal sender As Object, ByVal e As EventArgs)
		///            Me.TabbedMDIOn = False
		///            Me.LayoutMdi(MdiLayout.Cascade)
		///        End Sub</code></coderef></example>
		[Description( "Specifies the menu item to which the MDI Children list should be added." )]
		[DefaultValue( null )]
		public MenuItem MdiListMenuItem
		{
			get
			{
				return this.mdiListMenuItem;
			}
			set
			{
				if( this.mdiListMenuItem != value )
				{
					if( this.mdiListMenuItem != null )
					{
						this.DetachMdiListMenuItem( this.mdiListMenuItem );
					}

					this.mdiListMenuItem = value;

					if( this.mdiListMenuItem != null )
					{
						// If regular MdiList is enabled, disable it on the item.
						mdiListMenuItem.MdiList = false;
						this.AttachMdiListMenuItem( this.mdiListMenuItem );
					}
				}
			}
		}
		/// <summary></summary>
		internal ArrayListExt TabGroupHostsInternal
		{
			get
			{
				return tabHostList;
			}
		}

		/// <summary></summary>
		internal ArrayListExt SplitterHostsInternal
		{
			get
			{
				return splitterHostList;
			}
		}

        /// <summary></summary>
        internal bool ShouldRaiseBeforeMDIChildEvent
        {
            get { return m_bShouldRaiseBeforeMDIChildEvent; }
        }

		/// <summary>
		/// Returns the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/> representing
		/// the "Cancel" menu item.
		/// </summary>
		protected internal virtual BarItem CancelItem
		{
			get
			{
				if( this.cancelMenuItem == null )
				{
					this.cancelMenuItem = new BarItem( SR.GetString( SR.CancelMenuItemText, this ), new EventHandler( this.ContextMenu_Cancel ) );
					this.cancelMenuItem.MergeOrder = 10;
				}
				return this.cancelMenuItem;
			}
		}
		/// <summary>
		/// Returns the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/> representing
		/// the "Move Next" menu item.
		/// </summary>
		protected internal virtual BarItem MoveNextGroupItem
		{
			get
			{
				if( this.moveNextItem == null )
				{
					this.moveNextItem = new BarItem( SR.GetString( SR.MoveNextMenuItemText, this), new EventHandler( this.ContextMenu_MoveNext ) );
				}
				return this.moveNextItem;
			}
		}
		/// <summary>
		/// Returns the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/> representing
		/// the "Move Previous" menu item.
		/// </summary>
		protected internal virtual BarItem MovePrevGroupItem
		{
			get
			{
				if( this.movePrevItem == null )
				{
					this.movePrevItem = new BarItem( SR.GetString( SR.MovePrevMenuItemText, this), new EventHandler( this.ContextMenu_MovePrev ) );
				}
				return this.movePrevItem;
			}
		}
		/// <summary>
		/// Returns the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/> representing
		/// the "New Horizontal Tab Group" menu item.
		/// </summary>
		protected internal virtual BarItem NewHorzGroupItem
		{
			get
			{
				if( this.newHorzItem == null )
				{
					this.newHorzItem = new BarItem( SR.GetString( SR.NewHorzGroupMenuItemText, this) );
					this.newHorzItem.MergeOrder = 10;
					this.newHorzItem.Click += new EventHandler( this.ContextMenu_NewHorz );
				}

				return this.newHorzItem;
			}
		}
		/// <summary>
		/// Returns the <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.BarItem"/> representing
		/// the "New Vertical Tab Group" menu item.
		/// </summary>
		protected internal virtual BarItem NewVertGroupItem
		{
			get
			{
				if( this.newVertItem == null )
				{
					this.newVertItem = new BarItem( SR.GetString( SR.NewVertGroupMenuItemText, this), new EventHandler( this.ContextMenu_NewVert ) );
					this.newVertItem.MergeOrder = 10;
				}
				return this.newVertItem;
			}
		}

		/// <summary></summary>
		[DocumentationExclude()]
		protected virtual MdiClient MdiClient
		{
			get
			{
				return m_mdiClient;
			}
			set
			{
				if( this.m_mdiClient != value )
				{
					if( this.m_mdiClient != null )
					{
						if( resetLayout && this.m_mdiClient.IsHandleCreated )
						{
							NativeMethods.SendMessage( this.m_mdiClient.Handle, NativeMethods.WM_MDICASCADE, IntPtr.Zero, IntPtr.Zero );
						}

						m_mdiClient.ControlAdded -= new ControlEventHandler( MDIChild_Added );
						m_mdiClient.ControlRemoved -= new ControlEventHandler( MDIChild_Removed );
						m_mdiClient.Layout -= new LayoutEventHandler( MdiLayout );
					}

					m_mdiClient = value;

					if( m_mdiClient != null )
					{
						// Listen to the parent's child control colleciton changes
						m_mdiClient.ControlAdded += new ControlEventHandler( MDIChild_Added );
						m_mdiClient.ControlRemoved += new ControlEventHandler( MDIChild_Removed );
						m_mdiClient.Layout += new LayoutEventHandler( MdiLayout );
					}
				}
			}
		}
		/// <summary></summary>
		private TabHost ActiveTabHost
		{
			get
			{
				if( this.activeTabHost == null && this.tabHostList.Count > 0 )
				{
					this.ActiveTabHost = ( TabHost ) this.tabHostList[ 0 ];
				}

				return activeTabHost;
			}
			set
			{
				if( this.activeTabHost != value )
				{
					Font font = null;
					if( activeTabHost != null && activeTabHost.MDITabPanel != null )
					{
                        font = this.activeTabHost.MDITabPanel.ActiveTabFont;
				       //this.activeTabHost.MDITabPanel.ActiveTabFont = this.activeTabHost.MDITabPanel.Font;
					}

					activeTabHost = value;

					if( activeTabHost != null && activeTabHost.MDITabPanel != null && font!=null )
					{
						this.activeTabHost.MDITabPanel.ActiveTabFont = font;
					}
				}
			}
		}
		/// <summary>
		/// Returns whether tabStyle is Office2003 style or not
		/// </summary>
		protected bool IsOffice2003Style
		{
			get
			{
				return ( this.TabStyle == typeof( TabRendererOffice2003 ) );
			}
		}

		/// <summary></summary>
		[DocumentationExclude()]
		protected bool Dragging
		{
			get
			{
				return dragging;
			}
			set
			{
				if( dragging != value )
				{
					dragging = value;
					if( !dragging )
					{
						this.DropRect = Rectangle.Empty;
						Cursor.Current = Cursors.Default;
					}
				}
			}
		}
		/// <summary></summary>
		[DocumentationExclude()]
		protected Rectangle DropRect
		{
			set
			{
				if( this.dropRect != value )
				{
					if( this.dropRect != Rectangle.Empty )
					{
						DragRectDrawing.DrawDragFBRectangle
							( new Rectangle( dropRect.Left, dropRect.Top, 5, dropRect.Height ) );
						DragRectDrawing.DrawDragFBRectangle
							( new Rectangle( dropRect.Left, dropRect.Top, dropRect.Width, 5 ) );
						DragRectDrawing.DrawDragFBRectangle
							( new Rectangle( dropRect.Right - 6, dropRect.Top, 5, dropRect.Height ) );
						DragRectDrawing.DrawDragFBRectangle
							( new Rectangle( dropRect.Left, dropRect.Bottom - 6, dropRect.Width, 5 ) );
					}
					this.dropRect = value;
					if( this.dropRect != Rectangle.Empty )
					{
						DragRectDrawing.DrawDragFBRectangle
							( new Rectangle( dropRect.Left, dropRect.Top, 5, dropRect.Height ) );
						DragRectDrawing.DrawDragFBRectangle
							( new Rectangle( dropRect.Left, dropRect.Top, dropRect.Width, 5 ) );
						DragRectDrawing.DrawDragFBRectangle
							( new Rectangle( dropRect.Right - 6, dropRect.Top, 5, dropRect.Height ) );
						DragRectDrawing.DrawDragFBRectangle
							( new Rectangle( dropRect.Left, dropRect.Bottom - 6, dropRect.Width, 5 ) );
					}
				}
			}
		}

		/// <summary>
		/// Indicates whether to align the tab groups horizontally or vertically.
		/// </summary>
		/// <value>True indicates the tab groups should be aligned Horizontally;
		/// false indicates vertical alignment. Default is true.</value>
		[
		DefaultValue( true ),
		Localizable( true ),
        Description("Indicates whether to align the tab groups horizontally or vertically.")
		]
		public bool Horizontal
		{
			get
			{
				return m_bHorizontalAlignment;
			}
			set
			{
				if( m_bHorizontalAlignment != value )
				{
					m_bHorizontalAlignment = value;
					foreach( SplitterHost splitterHost in splitterHostList )
					{
						splitterHost.Horizontal = m_bHorizontalAlignment;
					}

					// Toggle alignment of LayoutPanel.
					if( m_lpPanel != null )
					{
						m_lpPanel.AlignmentToggle();
						m_lpPanel.DividePanelOnTwoParts();

						// Set TabHost's SplitterHosts bounds.
						ResetSplitterHostsBounds();
						m_lpPanel.SetSplitterHosts();
					}

					autoScaleBaseMdiClientDim = 0;
					if( MdiClient != null )
					{
						MdiClient.PerformLayout();
					}
				}
			}
		}
		/// <summary>
		/// Gets panel that contains layout data.
		/// </summary>
		internal LayoutPanel LayoutPanel
		{
			get
			{
				return m_lpPanel;
			}
		}
		/// <summary> Get or sets reference on form to which we attach a TabbedMDIManager. </summary>
		[
		Browsable( true ),
		Category( "Behavior" ),
		Description( "Reference on form to which we attach a TabbedMDIManager." )
		]
		public Form AttachedTo
		{
			get
			{
				return m_frmParent;
			}
			set
			{
				if( value != null && !value.IsMdiContainer )
				{
					value.IsMdiContainer = true;
				}

				if( m_frmParent != value )
				{
					DetachFromMdiContainer( m_frmParent, true );

					m_frmParent = value;

					AttachToMdiContainer( m_frmParent );
				}
			}
		}
		#endregion

		#region Class events
		/// <summary>
		/// Fired to let you provide a custom tab control.
		/// </summary>
		/// <remarks>The <b>TabControl</b> property of the event args will be null when
		/// this event is called. You can provide a custom <see cref="MDITabPanel"/> derived 
		/// class in this event's args.
		/// If you just have to set some properties on the tab control, then listen
		/// to the <see cref="TabControlAdded"/> event which will be called before
		/// creating the tab control.</remarks>
		[Description( "Fired to let user to provide a custom tab control through custom MDITabPanel." )]
		public event TabbedMDITabControlEventHandler TabControlAdding;
		/// <summary>
		/// Fired before drop down popup menu.
		/// </summary>
		[Description( "Occurs before the mdi list popupmenu appears." )]
		public event DropDownPopupEventHandler BeforeDropDownPopup;
		/// <summary>
		/// Fired to let you configure the tab control's 
		/// appearance and behavior.
		/// </summary>
		/// <remarks>
		/// If you have to provide a derived <see cref="MDITabPanel"/> instance
		/// to the TabbedMDIManager, use the <see cref="TabControlAdding"/> event.
		/// </remarks>
		[Description( "Occurs when tab panel is created and let user to configure the tab control's appearance and behavior." )]
		public event TabbedMDITabControlEventHandler TabControlAdded;
		/// <summary>
		/// Fired after a tab control in a tab group was removed.
		/// </summary>
		/// <remarks>
		/// You would typically listen to this event and unsubscribe to the tab control events that
		/// you previously subscribed to in the <see cref="TabControlAdded"/> handler.
		/// </remarks>
		[Description( "Occurs after a tab control in a tab group was removed." )]
		public event TabbedMDITabControlEventHandler TabControlRemoved;
		/// <summary>
		/// Fired to notify that the locked mdi client area is being unlocked.
		/// </summary>
		/// <remarks><para>
		/// Sometimes the <see cref="TabbedMDIManager"/> locks(prevents painting) the mdi client window for a short period 
		/// to avoid flicker as the tabbed mdi gets laid out.</para><para>This happens in this version when a new mdi child form
		/// gets added to the mdi parent form and gets shown. The mdi client gets locked when the mdi child gets
		/// added and gets unlocked a while (100 ms) after the mdi child gets activated. This avoids unseemly 
		/// flicker when the new mdi child gets activated.
		/// </para><para>
		/// Due to this locking you may not be able to perform certain operations in the <see cref="Form.MdiChildActivate"/>
		/// event like setting the focus on a child control in the new mdi child form (since the Form is locked along with the mdi client).
		/// In fact, calling the child form's <see cref="Control.CanFocus"/> property will return false when the mdi client is being locked.
		/// You should instead perform such operation in this event handler.
		/// </para></remarks>
		[Description( "Fired to notify that the locked mdi client area is being unlocked." )]
		public event EventHandler UnLockingMdiClient;
		/// <summary>Occurs before a MDI child is added to the TabbedMDIManager.</summary>
		[Description( "Occurs before a MDI child is added to the TabbedMDIManager." )]
		public event MDIChildAddCancelEventHandler BeforeMDIChildAdded;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary></summary>
		static TabbedMDIManager()
		{
			string strGuid = Guid.NewGuid().ToString();

#if SINGLE_DLL_BUILD
			AppStateSerializer.SetBindingInfo( "Syncfusion.Tools.Windows", typeof( TabbedMDIManager ).Assembly );
#else
      AppStateSerializer.SetBindingInfo( "Syncfusion.Tools.Frameworks", typeof( TabbedMDIManager ).Assembly );
      AppStateSerializer.SetTypeBindingInfo( "Syncfusion.Tools.Windows", typeof( TabGroupsStateInfo ).FullName, typeof( TabGroupsStateInfo ).Assembly );

#endif

		}

		/// <summary>
		/// Creates a new instance of the TabbedMDIManager.
		/// </summary>
		public TabbedMDIManager()
		{
			InitializeTabbedMDIManager();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="container"></param>
		public TabbedMDIManager( IContainer container )
		{
			container.Add( this );
			InitializeTabbedMDIManager();
		}
		/// <summary>
		/// Initializes TabbedMDIManager.
		/// </summary>
		private void InitializeTabbedMDIManager()
		{
			this.allowTabGroupCustomizing = true;
			tabHostList = new ArrayListExt();
			splitterHostList = new ArrayListExt();
			imageList = new ImageList();
			imageList.ColorDepth = ColorDepth.Depth32Bit;
			imageList.ImageSize = this.ImageSize;

			XPThemes.ThemeChanged += new EventHandler( OnThemeChanged );
			this.popupManager = new PopupMenusManager();
			this.mdiChildrenTooltips = new Hashtable();
		}

		/// <summary></summary>
		~TabbedMDIManager()
		{
			this.Dispose( false );
		}

		/// <summary></summary>
		//public void Dispose()
		//{
		//  this.Dispose( true );
		//  GC.SuppressFinalize( this );
		//}

		/// <summary></summary>
		protected bool IsDisposing
		{
			get
			{
				return m_bIsDisposing;
			}
		}

		/// <summary></summary>
		/// <param name="disposing"/>
		protected override void Dispose( bool disposing )
		{
			if( disposing && !bDisposed )
			{
				m_bIsDisposing = true;

				this.MdiListMenuItem = null;

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
				this.MdiListToolStripItem = null;
#endif

				if( this.mdiContainer != null && this.mdiContainer.IsHandleCreated )
				{
					this.DetachFromMdiContainer( this.mdiContainer, true, false );
				}

				this.mdiContainer = null;

				if( this.popupManager != null )
				{
					this.popupManager.Dispose();
					this.popupManager = null;
				}

				// Remove references to mdichildren in the tooltip-hash
				if( mdiChildrenTooltips != null )
				{
					foreach( Form mdiChild in this.mdiChildrenTooltips.Keys )
					{
						mdiChild.Disposed -= new EventHandler( this.MdiChildDisposed );
					}

					mdiChildrenTooltips.Clear();
					mdiChildrenTooltips = null;
				}
				if( mdiChildMenuList != null )
				{
					mdiChildMenuList.Clear();
					mdiChildMenuList = null;
				}
				if( tabHostList != null )
				{
					tabHostList.Clear();
					tabHostList = null;
				}
				if( splitterHostList != null )
				{
					splitterHostList.Clear();
					splitterHostList = null;
				}
				if( defaultContextMenuItem != null )
				{
					defaultContextMenuItem.Items.Dispose();
					defaultContextMenuItem.Dispose();
					defaultContextMenuItem = null;
				}
				if( contextMenu != null )
				{
					contextMenu.Dispose();
					contextMenu = null;
				}
				if( popupManager != null )
				{
					popupManager.Dispose();
					popupManager = null;
				}
				if( contextMenuItem != null )
				{
					contextMenuItem.Items.Dispose();
					contextMenuItem.Dispose();
					contextMenuItem = null;
				}
				if( this.imageList != null )
				{
					this.imageList.Images.Clear();
					this.imageList.Dispose();
					this.imageList = null;
				}

				for( int i = 0, len = m_arrNativeWindowsToRelease.Count; i < len; i++ )
				{
					NativeWindow wnd = m_arrNativeWindowsToRelease[ i ] as NativeWindow;
					try
					{
						wnd.ReleaseHandle();
					}
					catch( Exception ex )
					{
						Debug.WriteLine( ex.Message + Environment.NewLine + ex.StackTrace );
					}
				}

				XPThemes.ThemeChanged -= new EventHandler( OnThemeChanged );

				closeMenuItem = null;
				cancelMenuItem = null;

				if (newHorzItem != null)
				{
					newHorzItem.Click -= new EventHandler(this.ContextMenu_NewHorz);
					newHorzItem = null;
				}
				if (newVertItem!=null)
					newVertItem = null;

				if(movePrevItem!=null)
					movePrevItem = null;

				if(moveNextItem!=null)
					moveNextItem = null;

				if (m_mdiContainerSubclass != null)
					m_mdiContainerSubclass = null;

				bDisposed = true;

				m_bIsDisposing = false;

				base.Dispose( disposing );
			}
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Indicates whether the TabHost specified is still in use in the TabbedMDIManager.
		/// </summary>
		/// <param name="tabHost">The <see cref="TabHost"/> to validate.</param>
		/// <returns>True if the TabHost is still in use; false otherwise.</returns>
		public bool IsValidTabHost( TabHost tabHost )
		{
			return tabHostList.Contains( tabHost );
		}

		/// <summary>
		/// Sets the tooltip for the tab associated with the specified Form.
		/// </summary>
		/// <param name="mdiChild">The mdi child Form.</param>
		/// <param name="tooltip">The tooltip string.</param>
		public void SetTooltip( Form mdiChild, string tooltip )
		{
			if( mdiChildrenTooltips[ mdiChild ] == null
				|| mdiChildrenTooltips[ mdiChild ].ToString() != tooltip )
			{
				if( tooltip == String.Empty )
				{
					mdiChildrenTooltips.Remove( mdiChild );
					mdiChild.Disposed -= new EventHandler( MdiChildDisposed );
				}
				else
				{
					mdiChildrenTooltips[ mdiChild ] = tooltip;
					mdiChild.Disposed += new EventHandler( MdiChildDisposed );
				}

				TabHost tabHost = GetTabHostFromForm( mdiChild );
				if( tabHost != null )
				{
					tabHost.SetTooltip( mdiChild, tooltip );
				}
			}
		}

		/// <summary>
		/// Returns the tooltip specified for the form.
		/// </summary>
		/// <param name="mdiChild">The form whose tooltip is required.</param>
		/// <returns>The corresponding tooltip string.</returns>
		/// <remarks><para>This method returns the tooltip text set using a previous call to <see cref="SetTooltip"/>.</para></remarks>
		public string GetTooltip( Form mdiChild )
		{
			string sTooltip = String.Empty;

			if( this.mdiChildrenTooltips[ mdiChild ] != null )
			{
				sTooltip = this.mdiChildrenTooltips[ mdiChild ].ToString();
			}

			return sTooltip;
		}

		/// <summary>
		/// Indicates whether a new horizontal tab group can be created, off the active child form.
		/// </summary>
		/// <returns></returns>
		public bool CanCreateNewHorizontalGroup()
		{
			if( this.ActiveTabHost != null )
			{
				return this.CanCreateNewHorizontalGroup( this.ActiveTabHost.MDITabPanel );
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Creates a new horizontal tab group, moving the active child form to that group.
		/// </summary>
		/// <param name="groupName"> Tab group name. </param>
		public void CreateNewHorizontalGroup( string groupName )
		{
			if( !CanCreateNewHorizontalGroup() )
			{
				return;
			}

			m_strNewGroupName = groupName;
			MoveActiveTabToNewGroup( true );
			m_strNewGroupName = null;
		}

		/// <summary>
		/// Creates a new horizontal tab group, moving the active child form to that group.
		/// </summary>
		public void CreateNewHorizontalGroup()
		{
			if( !CanCreateNewHorizontalGroup() )
			{
				return;
			}

			MoveActiveTabToNewGroup( true );
		}

		/// <summary>
		/// Indicates whether a new vertical tab group can be created, off the active child form.
		/// </summary>
		/// <returns></returns>
		public bool CanCreateNewVerticalGroup()
		{
			if( ActiveTabHost != null )
			{
				return CanCreateNewVerticalGroup( ActiveTabHost.MDITabPanel );
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Creates a new vertical tab group, moving the active mdi child to that group.
		/// </summary>
		/// <param name="groupName"> Tab group name. </param>
		public void CreateNewVerticalGroup( string groupName )
		{
			if( !CanCreateNewVerticalGroup() )
			{
				return;
			}

			m_strNewGroupName = groupName;
			MoveActiveTabToNewGroup( false );
			m_strNewGroupName = null;
		}

		/// <summary>
		/// Creates a new vertical tab group, moving the active mdi child to that group.
		/// </summary>
		public void CreateNewVerticalGroup()
		{
			if( !CanCreateNewVerticalGroup() )
			{
				return;
			}

			MoveActiveTabToNewGroup( false );
		}
		/// <summary>
		/// Indicates whether the current active form can be moved to the previous tab group.
		/// </summary>
		/// <returns>True if possible; false otherwise.</returns>
		public bool CanMoveToPreviousTabGroup()
		{
			if( this.ActiveTabHost != null )
			{
				return this.CanMoveToPreviousTabGroup( this.ActiveTabHost.MDITabPanel );
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Moves the current active mdi child to its previous tab group, if any.
		/// </summary>
		public void MoveToPreviousTabGroup()
		{
			if( !this.CanMoveToPreviousTabGroup() )
			{
				return;
			}

			this.MoveActiveDocToAdjTabHost( false );
		}

		/// <summary>
		/// Indicates whether the current active form can be moved to the next tab group.
		/// </summary>
		/// <returns>True if possible; false otherwise.</returns>
		public bool CanMoveToNextTabGroup()
		{
			if( this.ActiveTabHost != null )
			{
				return this.CanMoveToNextTabGroup( this.ActiveTabHost.MDITabPanel );
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Moves the current active mdi child to its next tab group, if any.
		/// </summary>
		public void MoveToNextTabGroup()
		{
			if( !this.CanMoveToNextTabGroup() )
			{
				return;
			}

			this.MoveActiveDocToAdjTabHost( true );
		}
		/// <summary>
		/// Returns the <see cref="TabHost"/> given a mdi child form.
		/// </summary>
		/// <param name="mdiChild">A mdi child form instance.</param>
		/// <returns>A <see cref="TabHost"/> that hosts the mdi child Form.
		/// Will return null if the mdi child form is not found.
		/// </returns>
		public TabHost GetTabHostFromForm( Form mdiChild )
		{
			foreach( TabHost tabHost in tabHostList )
			{
				if( tabHost.MDITabPanel != null && tabHost.MDITabPanel.IsHosting( mdiChild ) )
				{
					return tabHost;
				}
			}
			return null;
		}
		/// <summary>
		/// Returns the <see cref="TabPageAdv"/> given a mdi child form.
		/// </summary>
		/// <param name="mdiChild">A mdi child Form instance.</param>
		/// <returns>A <see cref="TabPageAdv"/> that hosts the mdi child form.
		/// Will return null if the mdi child form is not found or not associated with a tab page.
		/// </returns>
		public TabPageAdv GetTabPageAdvFromForm( Form mdiChild )
		{
			foreach( TabHost tabHost in this.tabHostList )
			{
				if( tabHost != null && tabHost.MDITabPanel != null && tabHost.MDITabPanel.IsHosting( mdiChild ) )
				{
					return tabHost.MDITabPanel.GetTabPageAdvFromForm( mdiChild );
				}
			}
			return null;
		}

		/// <summary>
		/// Moves the active form to the specified tab group.
		/// </summary>
		/// <param name="tabHost">The <see cref="TabHost"/> representing the tab group.</param>
		/// <remarks>
		/// Use the <see cref="TabGroupHosts"/> property to get the current list of <see cref="TabHost"/>s.
		/// </remarks>
		/// <seealso cref="MoveDocTo"/>
		public void MoveActiveDocTo( TabHost pTabHost )
		{
			MoveActiveDocTo( pTabHost, mdiContainer.ActiveMdiChild );
		}
		/// <summary>
		/// Moves the active form to the specified tab group.
		/// </summary>
		/// <param name="tabHost">The <see cref="TabHost"/> representing the tab group.</param>
		/// <param name="pFrmChild"> A MDIChild form. </param>
		public void MoveActiveDocTo( TabHost pTabHost, Form pFrmChild )
		{
			if( this.mdiContainer.ActiveMdiChild == null )
			{
				return;
			}

			MoveDocTo( pFrmChild, pTabHost );
		}
		/// <summary>
		/// Moves a child form to the specified tab group.
		/// </summary>
		/// <param name="form">The child Form.</param>
		/// <param name="tabHost">The <see cref="TabHost"/> representing the tab group.</param>
		/// <remarks>
		/// Use the <see cref="TabGroupHosts"/> property to get the current list of <see cref="TabHost"/>s.
		/// </remarks>
		/// <seealso cref="MoveActiveDocTo"/>
		public void MoveDocTo( Form form, TabHost tabHost )
		{
			if( !this.tabHostList.Contains( tabHost ) )
			{
				throw new ArgumentException( "The TabHost specified is not valid", "tabHost" );
			}

			// Already hosting, return.
			if( tabHost.MDITabPanel.IsHosting( form ) )
			{
				return;
			}

			BeginUpdate( false );
			SuspendLayout();

			TabHost currentHost = this.GetTabHostFromForm( form );
			if( currentHost != null )
			{
				shouldKeepImageIndex = true;

				TabPageAdv tabPage = currentHost.MDITabPanel.GetTabPageAdvFromForm( form );
				MDIChildTabData prevTabData = new MDIChildTabData( tabPage.TabData as MDIChildTabData );

				// Remove the doc from its host
				bool tabsHostRemoved = false;
				RemoveDocFromTabHost( currentHost, form, ref tabsHostRemoved );
                OnActiveTabToNewGroupMoved(form, currentHost, tabHost);
				// Add it to the other tabHost.
				form.SuspendLayout();
				tabHost.AddMdiChild( form, prevTabData );

				if( !tabsHostRemoved )
				{
					SetNextTabHost( currentHost, tabHost );
					SetPreviousTabHost( tabHost, currentHost );
					AddTabHosts( tabHost );
				}

				shouldKeepImageIndex = false;
				form.ResumeLayout();
			}
			else
			{
				AddMdiChild( form, tabHost );
			}

			ResumeLayoutInternal();
			if( m_mdiClient != null )
			{
				m_mdiClient.PerformLayout();
			}

			EndUpdate( mdiContainer, m_mdiClient, false );
		}
		/// <summary>
		/// Sets next TabHost control for a current one.
		/// </summary>
		/// <param name="pCurrentHost"> Current TabHost control. </param>
		/// <param name="pNextHost"> TabHost control to add. </param>
		private void SetNextTabHost( TabHost pCurrentHost, TabHost pNextHost )
		{
			if( pCurrentHost != null )
			{
				if( !pCurrentHost.PreviousTabHost.Contains( pNextHost ) )
				{
					ArrayList hosts = pCurrentHost.NextTabHost;

					if (hosts.Contains(pNextHost))
					{
						hosts.Remove(pNextHost);
					}

					pNextHost.NextTabHost.Clear();
					pNextHost.NextTabHost.AddRange(hosts);

					hosts.Insert(0, pNextHost);
				}
			}
		}
		/// <summary>
		/// Sets previous TabHost control for a current one.
		/// </summary>
		/// <param name="pCurrentHost"> Current TabHost control. </param>
		/// <param name="pPreviousHost"> TabHost control to add. </param>
		private void SetPreviousTabHost( TabHost pCurrentHost, TabHost pPreviousHost )
		{
			if( pCurrentHost != null )
			{
				if( !pCurrentHost.NextTabHost.Contains( pPreviousHost ) )
				{
					if( pCurrentHost.PreviousTabHost.Contains( pPreviousHost ) )
					{
						pCurrentHost.PreviousTabHost.Remove( pPreviousHost );
					}

					pCurrentHost.PreviousTabHost.Insert( 0, pPreviousHost );
				}
			}
		}
		/// <summary>
		/// Adds TabHost controls to collections of TabHost controls, 
		/// which are next and previous TabHost controls for MDIChild form
		/// </summary>
		/// <param name="pHost"> Current TabHost control. </param>
		private void AddTabHosts( TabHost pHost )
		{
			ArrayList arrNextHosts = new ArrayList();
			ArrayList arrPreviousHosts = new ArrayList();
			bool bIsPrevious = true;

			if( m_lpPanel != null )
			{
				m_lpPanel.GetNextAndPreviousTabHosts( pHost, ref arrPreviousHosts, ref arrNextHosts, ref bIsPrevious );
			}

			foreach( TabHost thPreviousHost in arrPreviousHosts )
			{
				AddNextTabHosts( thPreviousHost, pHost );
				AddPreviousTabHosts( pHost, thPreviousHost );
			}

			foreach( TabHost thNextHost in arrNextHosts )
			{
				AddPreviousTabHosts( thNextHost, pHost );
				AddNextTabHosts( pHost, thNextHost );
			}
		}
		/// <summary>
		/// Adds TabHost control to collection of TabHost controls, 
		/// which are next TabHost controls for MDIChild form. 
		/// </summary>
		/// <param name="pHostToAdd"> TabHost control to which 
		/// current TabHost control must be added. </param>
		/// <param name="pCurrentHost"> Current TabHost control. </param>
		private void AddNextTabHosts( TabHost pHostToAdd, TabHost pCurrentHost )
		{
			if( !pHostToAdd.NextTabHost.Contains( pCurrentHost ) )
			{
				pHostToAdd.NextTabHost.Add( pCurrentHost );
			}
		}
		/// <summary>
		/// Adds TabHost control to collection of TabHost controls,
		/// which are previous TabHost controls for MDIChild form. 
		/// </summary>
		/// <param name="pHostToAdd"> TabHost control to which 
		/// current TabHost control must be added. </param>
		/// <param name="pCurrentHost"> Current TabHost control. </param>
		private void AddPreviousTabHosts( TabHost pHostToAdd, TabHost pCurrentHost )
		{
			if( !pHostToAdd.PreviousTabHost.Contains( pCurrentHost ) )
			{
				pHostToAdd.PreviousTabHost.Add( pCurrentHost );
			}
		}
		/// <summary>
		/// Consolidates the child forms in different tab groups into a single tab group.
		/// </summary>
		public void MakeSingleTabGroup()
		{
			if( this.TabGroupHosts == null || this.TabGroupHosts.Length == 0 )
			{
				return;
			}

			foreach( Form form in this.MdiChildren )
			{
				this.MoveDocTo( form, this.TabGroupHosts[ 0 ] );
			}
		}

		/// <summary>
        /// Removes the Tab host.
        /// </summary>
		/// <param name="tabHost"/>
		[DocumentationExclude()]
		public virtual void RemoveTabHost( TabHost tabHost )
		{
			LockLayout( false );
			SuspendLayout();

			// Remove SplitterHost.
			if( tabHostList.Contains( tabHost ) )
			{
				SplitterHost splitterHost = tabHost.SplitterHost;
				splitterHost.Close();
				splitterHost.Dispose();
				splitterHostList.Remove( splitterHost );
			}

			// If removed TabHost is active then make its inactive and set value to null.
			if( activeTabHost == tabHost )
			{
				ActiveTabHost = null;
			}

			TabbedMDITabControlEventArgs e = new TabbedMDITabControlEventArgs( tabHost.MDITabPanel );
			OnTabControlRemoving( e );

			tabHost.MDITabPanel.ImageList = null;
			tabHost.MDITabPanel.MouseMove -= new MouseEventHandler( Tab_MouseMove );
			tabHost.MDITabPanel.MouseUp -= new MouseEventHandler( Tab_MouseUp );
			popupManager.SetXPContextMenu( tabHost.MDITabPanel, null );

			DeleteTabHostFromLayoutPanel( tabHost );

			autoScaleBaseMdiClientDim = 0;
			tabHostList.Remove( tabHost );

			RemoveTabHostReferences( tabHost );
			RenameTabHosts();

			if( m_lpPanel != null )
			{
				m_lpPanel.SetUniqueNames();
			}

			if( tabHostList.Count == 0 )
			{
				m_lpPanel = null;
			}

			ResumeLayoutInternal();
         
			tabHost.Close();
			tabHost.Dispose();
            tabHost = null;           

			// As LayoutPanel has been changed, so do Layout.
			if( m_mdiClient != null )
			{
				m_mdiClient.PerformLayout();                
			}

			UnlockLayoutAsync( false );
		}
		/// <summary>
		/// Removes TabHost's reference from existent TabHosts.
		/// </summary>
		/// <param name="tabHost"> TabHost which is removed. </param>
		private void RemoveTabHostReferences( TabHost pTabHost )
		{
			foreach( TabHost host in tabHostList )
			{
				host.NextTabHost.Remove( pTabHost );
				host.PreviousTabHost.Remove( pTabHost );
			}
		}
		/// <summary>
		/// Method removes TabHost from LayoutPanel and modifies it.
		/// </summary>
		/// <param name="pTabHost"> TabHost to remove. </param>
		private void DeleteTabHostFromLayoutPanel( TabHost pTabHost )
		{
			// Modify LayoutPanel.
			if( m_lpPanel != null )
			{
				LayoutPanel panel = pTabHost.LayoutPanel;
				if( panel != null )
				{
					LayoutPanel previousPanel = panel.PreviousPanel;
					panel.FindAndDeleteTabHost( previousPanel, pTabHost );

					if( m_lpPanel.ComponentOne == null )
					{
						// Set m_lpPanel.ComponentTwo as head LayoutPanel.
						if( m_lpPanel.ComponentTwo is LayoutPanel )
						{
							LayoutPanel panelTwo = m_lpPanel.ComponentTwo as LayoutPanel;
							Point ptPreviousLocation = m_lpPanel.PanelOneLocation;
							m_lpPanel = panelTwo;
							m_lpPanel.Location = ptPreviousLocation;
							m_lpPanel.PreviousPanel = null;
						}
					}
					else if( m_lpPanel.ComponentTwo == null )
					{
						// Set m_lpPanel.ComponentOne as head LayoutPanel.
						if( m_lpPanel.ComponentOne is LayoutPanel )
						{
							LayoutPanel panelOne = m_lpPanel.ComponentOne as LayoutPanel;
							m_lpPanel = panelOne;
							m_lpPanel.PreviousPanel = null;
						}
					}

					// Remove nullable LayoutPanels.
					m_lpPanel.SetNullablePanels();
					// Divide LayoutPanel.
					m_lpPanel.DividePanelOnTwoParts();
					// Set TabHost's.
					m_lpPanel.SetTabHosts();

					// Set TabHost's SplitterHosts bounds.
					ResetSplitterHostsBounds();
					m_lpPanel.SetSplitterHosts();
				}
			}
		}
		/// <summary>
		/// Cancels the pending splitter operation.
		/// </summary>
		/// <returns>True if successful; false if nothing was canceled.</returns>
		public bool CancelSplitterOperation()
		{
			bool returnValue = false;
			foreach( SplitterHost splitterHost in this.splitterHostList )
			{
				if( splitterHost == null )
				{
					continue;
				}
				returnValue |= splitterHost.CancelOperation();
			}
			return returnValue;
		}

		/// <summary>
		/// Cancels a pending operation. Dragging the splitters, for example.
		/// </summary>
		/// <returns>True if successful; false if nothing was cancelled.</returns>
		public bool CancelOperation()
		{
			if( Dragging )
			{
				Dragging = false;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
        /// Suspends the layout.
        /// </summary>
		[DocumentationExclude()]
		public void SuspendLayout()
		{
			if( suspendCount < 0 )
			{
				suspendCount = 0;
			}
			suspendCount++;
			if( this.m_mdiClient != null )
			{
				m_mdiClient.SuspendLayout();
			}
		}

		/// <summary>
        /// Resets the layout to it's default value.
        /// </summary>
		[DocumentationExclude()]
		public void ResumeLayout()
		{
			this.ResumeLayoutInternal();
			if( this.suspendCount == 0 )
			{
				this.MdiLayout( null, new LayoutEventArgs( this.m_mdiClient, "Bounds" ) );
			}
		}

		/// <summary>
        /// Updates the active tab host.
        /// </summary>
		[DocumentationExclude()]
		public void UpdateActiveTabHost()
		{
			UpdateActiveTabHost( this.mdiContainer.ActiveMdiChild );
		}
		/// <summary></summary>
		[DocumentationExclude()]
		public void UpdateActiveTabHost( Form pFrmChild )
		{
			Form activeMdiChild = this.mdiContainer.ActiveMdiChild;

			if( pFrmChild != null )
			{
				this.keyboardActivationHelper.BringToFront( pFrmChild );

				// Find out the new active tabs host, by parsing through the tabsHosts.
				foreach( TabHost tabHost in this.tabHostList )
				{
					if( tabHost == null || tabHost.MDITabPanel == null )
					{
						continue;
					}
					if( tabHost.MDITabPanel.IsHosting( pFrmChild ) )
					{
						this.ActiveTabHost = tabHost;
						break;
					}
				}
				if( this.ActiveTabHost != null && this.ActiveTabHost.MDITabPanel != null )
				{
					TabPageAdv TabPageAdv = this.ActiveTabHost.MDITabPanel.GetTabPageAdvFromForm( pFrmChild );

					if( TabPageAdv != null )
					{
						this.ActiveTabHost.MDITabPanel.SelectedTab = TabPageAdv;
					}
					else
					{
						// Update the tab based on the next top-most form in the Z-order.
						this.ActiveTabHost.MDITabPanel.UpdateTabBasedOnFormOnFront();
					}
				}

                if( this.ActiveTabHost != null && this.ActiveTabHost.SplitterHost != null)
                {
                    Rectangle bounds  = this.ActiveTabHost.SplitterBounds;
                    NativeMethods.SetWindowPos( this.ActiveTabHost.SplitterHost.Handle, ( IntPtr ) NativeMethods.HWND_BOTTOM, bounds.X, bounds.Y, bounds.Width, bounds.Height, 0x0010 /*SWP_NOACTIVATE*/ );
                }
			}
		}
		/// <overload>
		/// Reads the tab group state information from a persistence medium.
		/// </overload>
		/// <summary>
		/// Reads the tab group states from the Isolated Storage.
		/// </summary>
		/// <returns>TRUE if the read is successful.</returns>
		/// <remarks><para>
		/// This method loads and applies the saved tab group states on the currently
		/// loaded child forms. Note that the loaded state information is not cached to be
		/// applied to child forms that might be created later. 
		/// </para><para>
		/// This method is automatically
		/// called by the <see cref="AttachToMdiContainer"/> method. You could optionally call this 
		/// method or it's other overloaded variants to load the tab group states at a different time.
		/// </para></remarks>
		public bool LoadTabGroupStates()
		{
			if( m_bIsTabbedMDIModeOn )
			{
				return LoadTabGroupStates( AppStateSerializer.GetSingleton() );
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Reads the previously serialized tab group states.
		/// </summary>
		/// <param name="mode"> A <see cref="SerializeMode"/> value.</param>
		/// <param name="persistpath">The name of the IsolatedStorage/INI/XML file or the 
		/// registry key containing the  persisted tab group information.</param>
		/// <returns>TRUE if the load is successful.</returns>
		/// <remarks><para>
		/// Reads the tab groups information from the specified persistent store and applies the new state. 
		/// This method has been provided only to allow a higher degree of control over the 
		/// serialization process. For normal state storage and retrieval it is advisable to 
		/// use the <see cref="SaveTabGroupStates()"/> and <see cref="LoadTabGroupStates()"/>
		/// methods.
		/// </para><para>
		/// This method will be removed in a future version. Please use the more flexible LoadTabGroupStates(AppStateSerializer) variant, instead.
		/// </para></remarks>
		[Obsolete( "This method will be removed in a future version. Please use the more flexible LoadTabGroupStates(AppStateSerializer) variant, instead.", false )]
		public virtual bool LoadTabGroupStates( SerializeMode mode, Object persistpath )
		{
			bool bretval = false;
			try
			{
				String strpersist = this.ID + "." + TAB_GROUP_INFO;

				TabGroupsStateInfo stateInfo = null;
				if( ( mode == SerializeMode.IsolatedStorage ) && ( persistpath == null ) )
				{
					AppStateSerializer serializer = AppStateSerializer.GetInstance();
					stateInfo = serializer.DeserializeObject( strpersist ) as TabGroupsStateInfo;
				}
				else // User-invoked
				{
					stateInfo = AppStateSerializer.DeserializeIsolatedObject( mode, persistpath, strpersist ) as TabGroupsStateInfo;
				}
				if( stateInfo != null )
				{
					ApplyDeserializedState( stateInfo );
					bretval = true;
				}
			}
			catch( Exception e )
			{
				Trace.Assert( false, "LoadTabGroupStates Failed.", e.Message + e.StackTrace );
			}

			return bretval;
		}

		/// <summary>
		/// Reads the previously serialized tab group states.
		/// </summary>
		/// <param name="serializer">A reference to the <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.</param>
		/// <returns>TRUE if the load is successful.</returns>
		/// <remarks><para>
		/// Reads the tab groups information from the specified persistent store and applies the new state. 
		/// This method has been provided only to allow a higher degree of control over the 
		/// serialization process. Note that the <see cref="LoadTabGroupStates()"/> and <see cref="SaveTabGroupStates()"/>
		/// methods get called automatically when you enable/disable tabbed mdi.
		/// </para><para>
		/// You could also cosider using the other overloaded variant <see cref="LoadTabGroupStates()"/>
		/// that loads the information from the Isolated Storage.
		/// </para></remarks>
		public virtual bool LoadTabGroupStates( AppStateSerializer serializer )
		{
			if( serializer == null )
			{
				return false;
			}

			bool bretval = false;
			try
			{
                String strpersist = ID + "." + TAB_GROUP_INFO;

                TabGroupsStateInfo stateInfo = null;
                stateInfo = serializer.DeserializeObject(strpersist) as TabGroupsStateInfo;
                if (stateInfo != null)
                {
                    ApplyDeserializedState(stateInfo);
                    bretval = true;
                }            
			}
			catch( Exception e )
			{
				Trace.Assert( false, "LoadTabGroupStates Failed.", e.Message );
			}

			return bretval;
		}

        private void RevertNonSavedChanges(TabGroupsStateInfo stateInfo)
        {
            int tabGroupCount = stateInfo.TabGroupCount;
            ArrayList TabPagesToBeMoved = new ArrayList();
            for (int i = 0; i < this.tabHostList.Count; i++)
            {
                TabHost tabHost = tabHostList[i] as TabHost;
                for (int j = 0; j < tabHost.MDITabPanel.TabPages.Count; j++)
                {
                    TabPageAdv page = tabHost.MDITabPanel.TabPages[j];
                    bool bExists = false;

                    for (int k = 0; k < tabGroupCount; k++)
                    {
                        Hashtable tabNames = stateInfo.GetTabNamesOfGroup(k);
                        if (tabNames != null)
                        {
                            string tabName = null;

                            // Check if the current tabs are in this group
                            foreach (UniquePageID tabID in tabNames.Keys)
                            {
                                tabName = tabID.TabName;
                                if (page.Text == tabName)
                                {
                                    bExists = true;
                                    break;
                                }
                            }

                            if(bExists)
                            {
                                break;
                            }
                        }
                    }
                    if (!bExists)
                    {
                        TabPagesToBeMoved.Add(page); //MoveDocumentTo(page.Text, 0);
                    }
                }
            }
            foreach (TabPageAdv page in TabPagesToBeMoved)
            {
                MoveDocumentTo(page.Text, 0);
            }
        }

		/// <summary>
        /// Applies deserialized state to the control.
        /// </summary>
		/// <param name="tabGroupStateInfo"/>
		[DocumentationExclude(),
		 EditorBrowsable( EditorBrowsableState.Advanced )]
		public virtual void ApplyDeserializedState( TabGroupsStateInfo tabGroupStateInfo )
		{
			if( tabGroupStateInfo == null )
			{
				return;
			}

			BeginUpdate( true );
			SuspendLayout();

			RevertNonSavedChanges( tabGroupStateInfo );

			ArrayList autoScaleBaseSizes = tabGroupStateInfo.MdiAutoScaleBaseSizes;

			// Retrieve one group at a time
			int tabGroupCount = tabGroupStateInfo.TabGroupCount;

			m_htPagesToSkip.Clear();
			m_bShouldConsiderPageSkip = true;

			for( int i = 0; i < tabGroupCount; i++ )
			{
				Hashtable tabNames = tabGroupStateInfo.GetTabNamesOfGroup( i );
				if( tabNames != null )
				{
					string tabName = null;

					// Check if the current tabs are in this group
					foreach( UniquePageID tabID in tabNames.Keys )
					{
						tabName = tabID.TabName;

						if( tabName == null )
						{
							continue;
						}

						m_bSearchFromStart = true;
						shouldKeepImageIndex = true;

						bool bMoveDocumentTo = MoveDocumentTo( tabName, i );

						shouldKeepImageIndex = false;
						m_bSearchFromStart = false;

						if( bMoveDocumentTo )
						{
							TabHost tabHost = this.tabHostList[i] as TabHost;
							string groupName = tabGroupStateInfo.GetGroupName( i );

							if( groupName != null )
							{
								SetTabHostForGroup( groupName, tabHost );
							}
						}
					}
				}
			}

			m_htPagesToSkip.Clear();
			m_bShouldConsiderPageSkip = false;
            //  restore pages order
            int tabHostCount = tabHostList.Count;
            for (int i = 0; i < tabHostCount; i++)
            {
                Hashtable tabNames = tabGroupStateInfo.GetTabNamesOfGroup(i);
                if (tabNames != null)
                {
                    SortedList tabIndexes = new SortedList();
                    foreach (UniquePageID pageID in tabNames.Keys)
                    {
                        tabIndexes.Add(tabNames[pageID], pageID.TabName);
                    }

                    TabHost tabHost = tabHostList[i] as TabHost;
                    if (tabHost == null)
                    {
                        continue;
                    }

                    TabPageAdvCollection pagesCollection = tabHost.MDITabPanel.TabPages;
                    TabPageAdv[] pageArray = (TabPageAdv[])pagesCollection.ToArray(typeof(TabPageAdv));

                    Hashtable htPages = new Hashtable();
                    TabPageAdv tabPage = null;
                    ArrayList arrPagesWithSameName = null;
                    string pageName = null;

                    for (int k = 0, len = pageArray.Length; k < len; k++)
                    {
                        tabPage = pageArray[k] as TabPageAdv;
                        pageName = tabPage.Text;
                        arrPagesWithSameName = htPages[pageName] as ArrayList;

                        if (arrPagesWithSameName == null)
                        {
                            arrPagesWithSameName = new ArrayList();
                            htPages[pageName] = arrPagesWithSameName;
                        }

                        arrPagesWithSameName.Add(tabPage);
                    }

                    pagesCollection.Clear();

                    for (int j = 0; j < tabIndexes.Count; j++)
                    {
                       pageName = tabIndexes[j].ToString();
                        arrPagesWithSameName = htPages[pageName] as ArrayList;
                        if (arrPagesWithSameName != null && arrPagesWithSameName.Count > 0)
                        {
                           pagesCollection.Add(arrPagesWithSameName[0]);
                            arrPagesWithSameName.RemoveAt(0);
                        }
                    }

                    foreach (DictionaryEntry entry in htPages)
                    {
                        arrPagesWithSameName = entry.Value as ArrayList;
                        if (arrPagesWithSameName != null && arrPagesWithSameName.Count > 0)
                        {
                            pagesCollection.AddRange(arrPagesWithSameName);
                        }
                    }

                    // restore selected page
                    int selectedIndex = tabGroupStateInfo.GetSelectedPagesOfGroup(i);
                    if (selectedIndex != -1 && selectedIndex < tabHost.MDITabPanel.TabPages.Count)
                    {
                        tabHost.MDITabPanel.SelectedIndex = selectedIndex;
                        if (selectedIndex == 0)
                            tabHost.MDITabPanel.UpdateSelectedTabIndex(selectedIndex);
                        tabHost.MDITabPanel.UpdateActiveTabFont();
                    }
                }
            }
		
			// Remove empty items in tabHostList
			for( int i = this.tabHostList.Count - 1; i >= 0; i-- )
			{
				if( tabHostList[i] == null )
				{
					tabHostList.RemoveAt( i );
				}
			}
			for( int i = splitterHostList.Count - 1; i >= 0; i-- )
			{
				if( splitterHostList[i] == null )
				{
					splitterHostList.RemoveAt( i );
				}
			}

			// Make sure this is done finally
			// Retrieve MDI Client's AutoScale base size
			autoScaleBaseMdiClientDim = tabGroupStateInfo.MdiClientAutoScaleBaseSize;

			DeserializeLayoutPanel( tabGroupStateInfo.LayoutPanel, tabGroupStateInfo );

			ResumeLayoutInternal();
			m_mdiClient.PerformLayout();

			EndUpdate( mdiContainer, m_mdiClient, true );
		}

		private static void RefineTabHostSiblings( ArrayList siblings )
		{
			for( int i = 0; i < siblings.Count; ++i )
			{
				TabHost th = (TabHost)siblings[i];

				if( th.MDITabPanel == null )
				{
					siblings.RemoveAt( i-- );
				}
			}
		}

		/// <summary>
		/// Method tries to deserialize layout information that contains in serialized 
		/// LayoutPanel class.
		/// </summary>
		/// <param name="pLayoutPanel"> The nullable panel for refilling needed layout info. </param>
		/// <param name="info"> The deserialization info. </param>
		private void DeserializeLayoutPanel( LayoutPanel pLayoutPanel, TabGroupsStateInfo info )
		{
			// Set Layout data.
			m_lpPanel = pLayoutPanel;

			if( m_lpPanel != null )
			{
				SetTabHosts( null, m_lpPanel );

				if( m_lpPanel.ComponentOne == null && m_lpPanel.ComponentTwo == null )
				{
					InitializeLayoutPanel();
				}
				else
				{
					// Set LayoutPanels for Hosts. 
					m_lpPanel.SetLayoutPanels();
					// Remove nullable LayoutPanels.
					m_lpPanel.SetNullablePanels();
					// Set LayoutPanel size.
					m_lpPanel.Size = m_mdiClient.DisplayRectangle.Size;
					// Divide panel.
					m_lpPanel.DividePanelOnTwoParts();
					// Set TabHost's SplitterHosts bounds.
					ResetSplitterHostsBounds();
					m_lpPanel.SetSplitterHosts();
				}
			}
			else
			{
				bool bHorizontal = info.Horizontal;
				int iSize = info.MdiClientAutoScaleBaseSize;
				int iTabHostsCount = tabHostList.Count;
				ArrayList lstBaseSizes = info.MdiAutoScaleBaseSizes;
				TabHost thCurrent = null;
				TabHost thNext = null;

				m_lpPanel = new LayoutPanel();

				if( iTabHostsCount == 1 )
				{
					InitializeLayoutPanel();
				}
				else if( iTabHostsCount > 1 )
				{
					thCurrent = ( TabHost ) tabHostList[ 0 ];
					thNext = ( TabHost ) tabHostList[ 1 ];

					m_lpPanel = new LayoutPanel( thCurrent, thNext, m_mdiClient.DisplayRectangle.Size, m_mdiClient.DisplayRectangle.Location, bHorizontal );
					m_lpPanel.Coefficient = ( float ) ( int ) lstBaseSizes[ 0 ] / iSize;

					for( int i = 2; i < iTabHostsCount; i++ )
					{
						thCurrent = ( TabHost ) tabHostList[ i - 1 ];
						thNext = ( TabHost ) tabHostList[ i ];

						LayoutPanel lpCurrent = thCurrent.LayoutPanel;

						lpCurrent.DivideTabHost( thCurrent, thNext, bHorizontal );
						lpCurrent.Coefficient = ( float ) ( int ) lstBaseSizes[ i - 1 ] / iSize;
					}
				}

				m_lpPanel.DividePanelOnTwoParts();

				// Set TabHost's SplitterHosts bounds.
				ResetSplitterHostsBounds();
				m_lpPanel.SetSplitterHosts();
			}
		}
		/// <summary>
		/// Gets TabHost with equal hame.
		/// </summary>
		/// <param name="name"> Name to find. </param>
		/// <returns></returns>
		private TabHost GetFromName( string name )
		{
			foreach( TabHost tabHost in tabHostList )
			{
				if( tabHost.Name == name )
				{
					return tabHost;
				}
			}

			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		private void SetTabHosts( LayoutPanel pPreviousPanel, LayoutPanel pPanel )
		{
			if( pPanel.ComponentOne is LayoutPanel )
			{
				LayoutPanel panelOne = pPanel.ComponentOne as LayoutPanel;
				SetTabHosts( pPanel, panelOne );
			}

			if( pPanel.ComponentTwo is LayoutPanel )
			{
				LayoutPanel panelTwo = pPanel.ComponentTwo as LayoutPanel;
				SetTabHosts( pPanel, panelTwo );
			}

			if( pPanel.TabHostOneName != String.Empty )
			{
				pPanel.ComponentOne = GetFromName( pPanel.TabHostOneName );
			}
			if( pPanel.TabHostTwoName != String.Empty )
			{
				pPanel.ComponentTwo = GetFromName( pPanel.TabHostTwoName );
			}

			// Set TabHost.
			pPanel.TabHost = GetFromName( pPanel.UniqueName );
			// Set Previous panel.
			pPanel.PreviousPanel = pPreviousPanel;
		}
		/// <summary>
		/// Locks the Host Form redrawing.
		/// </summary>
		public void LockHostFormUpdate()
		{
			if( this.mdiContainer != null && this.mdiContainer.IsHandleCreated )
			{
				NativeMethodsHelper.SuspendRedrawWindow( this.mdiContainer.Handle );
			}

			if( this.m_mdiClient != null && this.m_mdiClient.IsHandleCreated )
			{
				NativeMethodsHelper.SuspendRedrawWindow( this.m_mdiClient.Handle );
			}
		}

        /// <summary>
        /// Locks the MDI client redrawing.(MDI Client alone)
        /// </summary>
        public void LockMDIClientUpdate()
        {
            if (this.m_mdiClient != null && this.m_mdiClient.IsHandleCreated)
            {
                NativeMethodsHelper.SuspendRedrawWindow(this.m_mdiClient.Handle);
            }
        }

        /// <summary>
        /// Unlocks the MDI Client redrawing.
        /// </summary>
        public void UnLockMDIClientUpdate()
        {
            if (m_mdiClient != null && this.m_mdiClient.IsHandleCreated)
            {
                NativeMethodsHelper.ResumeRedrawWindow(this.m_mdiClient.Handle, true);
            }
            if (this.mdiContainer != null)
            {
                this.mdiContainer.Update();
            }
        }

		/// <summary>
		/// UnLocks the Host Form redrawing.
		/// </summary>
		public void UnlockHostFormUpdate()
		{
			if( m_mdiClient != null && this.m_mdiClient.IsHandleCreated )
			{
				NativeMethodsHelper.ResumeRedrawWindow( this.m_mdiClient.Handle, true );
			}

			if( this.mdiContainer != null && this.mdiContainer.IsHandleCreated )
			{
				NativeMethodsHelper.ResumeRedrawWindow( this.mdiContainer.Handle, true );
			}
		}

		/// <overload>
		/// Saves the current tab groups information into a persistence medium.
		/// </overload>
		/// <summary>
		/// Saves the current tab group states to Isolated Storage.
		/// </summary>
		/// <remarks><para>
		/// Calling this method saves the current tab group states in Isolated Storage.
		/// </para><para>
		/// This method is also called by the <see cref="DetachFromMdiContainer"/> method
		/// to save the tag group states while disabling tabbed mdi. You could call this or any
		/// of it's overloaded variants to explicitly save the state at any specific time.
		/// </para></remarks>
		public void SaveTabGroupStates()
		{
			if( m_bIsTabbedMDIModeOn )
			{
				SaveTabGroupStates( AppStateSerializer.GetSingleton() );
			}
		}

		/// <summary>
		/// Saves the current tab groups information to the specified persistence medium.
		/// </summary>
		/// <param name="mode"> A <see cref="SerializeMode"/> value.</param>
		/// <param name="persistpath"> Specifies the name of an IsolatedStorage/INI/XML file or a registry key to  
		/// which the persistence information will be written.</param>
		/// <remarks>
		/// Writes the mdi tab groups information to the persistence medium specified by the 
		/// <paramref name="mode"/> parameter and at the path specified by the <paramref name="persistpath"/> object.
		/// This method has been provided only to allow a higher degree of control over the 
		/// serialization process. For normal state storage and retrieval it is advisable to 
		/// use the <see cref="SaveTabGroupStates()"/> and <see cref="LoadTabGroupStates()"/> 
		/// methods.
		/// <para>
		/// This method will be removed in a future version. Please use the more flexible SaveTabGroupStates(AppStateSerializer) variant, instead.
		/// </para></remarks>
		[Obsolete( "This method will be removed in a future version. Please use the more flexible SaveTabGroupStates(AppStateSerializer) variant, instead.", false )]
		public virtual void SaveTabGroupStates( SerializeMode mode, Object persistpath )
		{
			// Call this early
			AppStateSerializer serializer = AppStateSerializer.GetSingleton();

			// Save the current tab group info.
			try
			{
				TabGroupsStateInfo stateInfo = null;
				if( this.AllowTabGroupCustomizing && tabHostList.Count > 0 )
				{
					stateInfo = new TabGroupsStateInfo( this );
				}

				String strpersist = this.ID + "." + TAB_GROUP_INFO;

				if( ( mode == SerializeMode.IsolatedStorage ) && ( persistpath == null ) ) // Default serialization to Isolated Storage
				{
					serializer.SerializeObject( strpersist, stateInfo );
				}
				else
				{
					AppStateSerializer.SerializeIsolatedObject( mode, persistpath, strpersist, stateInfo );
				}
			}
			catch( Exception e )
			{
				Trace.Assert( false, "SaveTabGroupStates Failed.", e.Message );
			}
		}

		/// <summary>
		/// Clears the state of the saved tab group.
		/// </summary>
		public void ClearSavedTabGroupState()
		{
			AppStateSerializer serializer = AppStateSerializer.GetSingleton();

			if( serializer != null )
			{
				String strpersist = string.Format( DEF_STATE_ID_FORMAT, ID, TAB_GROUP_INFO );

				serializer.RemoveInfoForObject( strpersist );
			}
		}

		/// <summary>
		/// Saves the current tab groups information to the specified persistence medium.
		/// </summary>
		/// <param name="serializer">A reference to the <see cref="Syncfusion.Runtime.Serialization.AppStateSerializer"/> instance.</param>
		/// <remarks><para>
		/// Writes the mdi tab groups information to the persistence medium.
		/// This method has been provided only to allow a higher degree of control over the 
		/// serialization process. Note that the <see cref="LoadTabGroupStates()"/> and <see cref="SaveTabGroupStates()"/>
		/// methods get called automatically when you enable/disable tabbed mdi.
		/// </para><para>
		/// You could also consider calling the other overloaded variant <see cref="SaveTabGroupStates(Syncfusion.Runtime.Serialization.AppStateSerializer)"/>
		/// that stores the tab group informtion in Isolated Storage.
		/// </para></remarks>
		public virtual void SaveTabGroupStates( AppStateSerializer serializer )
		{
			// Save the current tab group info.
			try
			{
				TabGroupsStateInfo stateInfo = null;
				if( tabHostList.Count > 0 )
				{
					stateInfo = new TabGroupsStateInfo( this );
				}

				String strpersist = ID + "." + TAB_GROUP_INFO;

				serializer.SerializeObject( strpersist, stateInfo );
			}
			catch( Exception e )
			{
				Trace.Assert( false, "SaveTabGroupStates Failed.", e.Message );
			}
		}

		/// <summary>
		/// Detaches an mdi parent from the TabbedMDIManager.
		/// </summary>
		/// <param name="mdiContainer">The mdi parent to be detached that was previously
		/// attached through AttachToMdiContainer.</param>
		/// <param name="setCascade">True indicates that it will layout mdi children in cascade mode after
		/// detaching itself; false otherwise.</param>
		/// <remarks><para>
		/// This will remove all references to the mdi parent and resume default mdi behavior.
		/// </para><para>
		/// This method will also save the current tab group state in Isolated Storage.
		/// </para></remarks>
		public virtual void DetachFromMdiContainer( Form pMdiContainer, bool setCascade, bool bLockMDIClient )
		{
            if (mdiContainer != pMdiContainer || mdiContainer == null)
            {
                return;
            }

            lock (typeof(TabbedMDIManager))
            {
                m_htManagers.Remove(pMdiContainer);
            }

            if (!m_bIsDisposing && !closed)
            {
                NeedUpdateHostedForm = true;
            }

            if (bLockMDIClient)
            {
                LockLayout(true);
            }

            MessageFilterEntryHelper.RemoveMessageFilter(this);

            if (mdiContainer.ContextMenu is ContextMenuPlaceHolder)
            {
                ContextMenuPlaceHolder cm = mdiContainer.ContextMenu as ContextMenuPlaceHolder;
                cm.TabbedMDIManager = null;
            }

            Cursor oldCursor = mdiContainer.Cursor;
            mdiContainer.Cursor = Cursors.WaitCursor;

            resetLayout = setCascade;

            if (null != keyboardActivationHelper)
            {
                keyboardActivationHelper.Dispose();
                keyboardActivationHelper = null;
            }

            SaveTabGroupStates();

            m_mdiContainerSubclass.ReleaseHandle();

            mdiContainer.Load -= new EventHandler(MDIContainer_Load);
            mdiContainer.MdiChildActivate -= new EventHandler(MDIChild_Activate);
            mdiContainer.RightToLeftChanged -= new EventHandler(MDIContainer_RightToLeftChanged);
            mdiContainer.HandleCreated -= new EventHandler(mdiContainer_HandleCreated);
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			mdiContainer.FormClosed -= new FormClosedEventHandler( mdiContainer_FormClosed );
#else
			mdiContainer.Closed -= new EventHandler( mdiContainer_FormClosed );
#endif

            foreach (Form mdiChild in this.mdiContainer.MdiChildren)
            {
                if (!(mdiChild is TabHost || mdiChild is SplitterHost))
                    mdiChild.VisibleChanged -= new EventHandler(MDIChild_VisibleChanged);

                this.NotifyTabbedMDIBindingToChildren(mdiChild, false);
            }

            Form tempRef = mdiContainer;

			// To make detaching faster:
			MdiClient.Layout -= new LayoutEventHandler( MdiLayout );
			for( int i = tabHostList.Count - 1; i >= 0; i-- )
			{
				RemoveTabHost( ( TabHost ) tabHostList[ i ] );
			}           
           
            tempRef.Cursor = oldCursor;

            if (bLockMDIClient)
            {
                UnlockLayoutAsync(true);
            }

            if (!closed)
            {
                foreach (Form frm in this.MdiChildren)
                    frm.Focus();
            }

            this.mdiContainer = null;
            this.MdiClient = null;
            this.imageList.Images.Clear();

            m_bIsTabbedMDIModeOn = false;
		}
		/// <summary>
		/// Detaches an mdi parent from the TabbedMDIManager.
		/// </summary>
		/// <param name="mdiContainer">The mdi parent to be detached that was previously
		/// attached through AttachToMdiContainer.</param>
		/// <param name="setCascade">True indicates that it will layout mdi children in cascade mode after
		/// detaching itself; false otherwise.</param>
		/// <remarks>
		/// <para>
		/// This will remove all references to the mdi parent and resume default mdi behavior.
		/// </para>
		/// <para>
		/// This method will also save the current tab group state in Isolated Storage.
		/// </para>
		/// </remarks>
		public virtual void DetachFromMdiContainer( Form mdiContainer, bool setCascade )
		{
			DetachFromMdiContainer( mdiContainer, setCascade, true );
		}
        /// <summary>This method is almost a stub, needed only for mouse hook subscribing.</summary>
		/// <returns></returns>
		/// <param name="m"/>
		[DocumentationExclude()]
		public virtual bool PreFilterMessage( ref Message m )
		{
			int msg = m.Msg;

			// Process Shortcuts first.
			if( msg == NativeMethods.WM_SYSKEYDOWN || msg == NativeMethods.WM_KEYDOWN )
			{
				// Always forward exceptions caught here to the Application class so that Application.ThreadException listeners will 
				// get to handle it.
				try
				{
					Keys keys = (Keys)m.WParam.ToInt32() | Control.ModifierKeys;

					if( ProcessCmdKey( keys, msg == 0x104 ) )
					{
						return true;
					}
				}
				catch( Exception e )
				{
					Application.OnThreadException( e );
				}
			}

			return false;
		}

		/// <summary>Checks if command key can be processed.</summary>
		/// <returns></returns>
		/// <param name="keyData"/>
		/// <param name="isSysKeyDown"/>
		[DocumentationExclude()]
		public virtual bool ProcessCmdKey( Keys keyData, bool isSysKeyDown )
		{
			if( this.mdiContainer == null )
			{
				return false;
			}

			bool returnVal = false;

			Form activeForm = Form.ActiveForm;
			if( activeForm == null )
			{
				return false;
			}

			if( activeForm != this.mdiContainer && activeForm.Owner != this.mdiContainer )
			{
				return false;
			}

			bool bValidated = false;

			if( keyData == Keys.Escape )
			{
				returnVal |= this.CancelOperation();
				returnVal |= this.CancelSplitterOperation();
			}

			else if( keyData == ( Keys.Control | Keys.F6 ) )
			{
				bValidated = ( !this.CausesFormValidation || ( this.MdiParent != null &&
					this.MdiParent.Validate() ) );
				if( bValidated )
				{
					this.CancelOperation();
					this.CancelSplitterOperation();

					this.keyboardActivationHelper.ActivateNext( false );
				}
				returnVal = true;
			}
			else if( keyData == ( Keys.Control | Keys.F6 | Keys.Shift ) )
			{
				bValidated = ( !this.CausesFormValidation || ( this.MdiParent != null &&
					this.MdiParent.Validate() ) );

				if( bValidated )
				{
					this.CancelOperation();
					this.CancelSplitterOperation();

					this.keyboardActivationHelper.ActivateNext( true );
				}
				returnVal = true;
			}
			else if( keyData == ( Keys.Tab | Keys.Control )
				|| keyData == ( Keys.Tab | Keys.Control | Keys.Shift ) )
			{
				bValidated = ( !this.CausesFormValidation || ( this.MdiParent != null &&
					this.MdiParent.Validate() ) );

				if( bValidated && null != activeTabHost
					&& activeTabHost.MDITabPanel.SwitchPagesForDialogKeys )
				{
					this.CancelOperation();
					this.CancelSplitterOperation();

					if( ( keyData & Keys.Shift ) > 0 )
					{
						this.keyboardActivationHelper.ActivateNext( true );
					}
					else
					{
						this.keyboardActivationHelper.ActivateNext( false );
					}
				}
				returnVal = true;
			}
			return returnVal;
		}

		/// <summary>
		/// This will be called after the controls and forms are done with processing the ProcessCmdKey.
		/// </summary>
		/// <returns></returns>
		/// <param name="msg"/>
		/// <param name="keyData"/>
		[DocumentationExclude()]
		public virtual bool ProcessCmdKey( ref Message msg, Keys keyData )
		{
			if( (Control.ModifierKeys & Keys.Control) == Keys.Control && (msg.WParam == (IntPtr)Keys.F4) && this.MdiParent != null )
			{
				if( (Control.ModifierKeys & Keys.Shift) != Keys.Shift && (Control.ModifierKeys & Keys.Alt) != Keys.Alt )
				{
					// first give the main-menu a shot at processing this one:
					if( this.MdiParent.Menu != null )
					{
						MethodInfo mInfo = typeof( MainMenu ).GetMethod( "ProcessCmdKey",
							BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic );
						if( mInfo != null )
						{
							bool retVal = ( bool ) mInfo.Invoke( this.MdiParent.Menu, new object[] { msg, keyData } );
							if( retVal )
							{
								return true;
							}
						}
					}

					if( this.MdiParent != null &&
						( !this.MdiParent.CausesValidation ||
						this.MdiParent.Validate() ) )
					{
						// Ctrl+F4 should close the active child form.
						this.CloseActiveChildForm();
						this.keyboardActivationHelper.StartTabBasedActivation();
					}
					return true;
				}
				else
				{
					return true;
				}
			}

			return false;
		}

		/// <summary></summary>
		/// <param name="form"></param>
		/// <returns></returns>
		public static TabbedMDIManager GetManagerForForm( Form form )
		{
			return ( TabbedMDIManager ) m_htManagers[ form ];
		}		

		/// <summary>
		/// Attaches a mdi parent to the TabbedMDIManager.
		/// </summary>
		/// <param name="mdiContainer">The mdi parent to attach to.</param>
		/// <remarks><para>
		/// This will attach the TabbedMDIManager to the mdi parent and invoke tabbed look-and-feel
		/// in the mdi client area. You should typically do this in the mdi container's constructor
		/// or in the Form Load event.
		/// </para><para>
		/// This method will also call <see cref="LoadTabGroupStates()"/> to load and apply
		/// the saved tab group states on the loaded child forms. Note that this loaded state
		/// will not be cached to be applied on child forms that might be loaded in a later stage.
		/// </para></remarks>
		public virtual void AttachToMdiContainer( Form pMdiContainer )
		{
			if( mdiContainer == pMdiContainer )
			{
				return;
			}

			lock( typeof( TabbedMDIManager ) )
			{
				m_htManagers[ pMdiContainer ] = this;
			}

			m_bIsTabbedMDIModeOn = true;

			// Mdi children cannot be in the maximized when using XP Menus, when this method is called.
			Form activeChild = pMdiContainer.ActiveMdiChild;
			if( activeChild != null && activeChild.WindowState == FormWindowState.Maximized )
			{
				pMdiContainer.LayoutMdi( System.Windows.Forms.MdiLayout.Cascade );
			}

			MessageFilterEntryHelper.AddMessageFilter( this, false );

			keyboardActivationHelper = new KeyboardActivationHelper( pMdiContainer );

			if( mdiContainer != null )
			{
				DetachFromMdiContainer( mdiContainer, true );
			}

			if( !pMdiContainer.IsMdiContainer )
			{
				throw new Exception( "You can attach only MdiContainers to TabbedMDIManager." );
			}

			mdiContainer = pMdiContainer;

			if( mdiContainer != null )
			{
				m_mdiContainerSubclass = new MdiParentNativeWindow( this );
				m_mdiContainerSubclass.AssignHandle( mdiContainer.Handle );
			}

			// Init ContextMenuPlaceHolder to listen for shortcut command keys.
			ContextMenuPlaceHolder cm = mdiContainer.ContextMenu as ContextMenuPlaceHolder;
			if( cm == null )
			{
				cm = new ContextMenuPlaceHolder();
			}
			cm.TabbedMDIManager = this;

			Cursor oldCursor = this.mdiContainer.Cursor;
			mdiContainer.Cursor = Cursors.WaitCursor;

			mdiContainer.MdiChildActivate += new EventHandler( MDIChild_Activate );
			mdiContainer.RightToLeftChanged += new EventHandler( MDIContainer_RightToLeftChanged );
            mdiContainer.Load += new EventHandler( MDIContainer_Load );

			// Get the MdiClient; There will be only one though
			foreach( Control control in mdiContainer.Controls )
			{
				if( control is MdiClient )
				{
					MdiClient = ( MdiClient ) control;
					break;
				}
			}

			if( mdiContainer.MdiChildren.Length > 0 )
			{
				BeginUpdate( false );

				TabHost tabHost = CreateTabHostInternal( tabHostList.Count );
				ActiveTabHost = tabHost;

				foreach( Form mdiChild in mdiContainer.MdiChildren )
				{
					AddMDIChildToTabbedMDI( mdiChild );
				}

				UpdateActiveTabHost();
				PostInitTabHost( tabHost );

				//this.InitFromStorage();
				LoadTabGroupStates();

				EndUpdateAsync( false );
			}

			mdiContainer.HandleCreated += new EventHandler( mdiContainer_HandleCreated );

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			mdiContainer.FormClosed += new FormClosedEventHandler( mdiContainer_FormClosed );
#else
      mdiContainer.Closed += new EventHandler( mdiContainer_FormClosed );
#endif

			MdiClient.ResumeLayout( true );
			MdiClient.PerformLayout();
			mdiContainer.Cursor = oldCursor;

			NeedUpdateHostedForm = false;
		}
		/// <summary>
		/// Method creates wrapper for MDIChild form handle,
		/// sets MessageFilter to filter received messages and
		/// adds values to hashtable.
		/// </summary>
		/// <param name="pMDIChild"> MDIChild form whose handle is to be wrappered. </param>
		internal void AddMDIChildToHash( Form pMDIChild )
		{
			MDIChildWindow hwnd;

			if( !m_htChildForms.Contains( pMDIChild ) )
			{
				hwnd = new MDIChildWindow( pMDIChild );
				hwnd.MessageFilter = hwnd;
				hwnd.SetIcon += new EventHandler( mdiChildNativeWnd_SetIcon );

				m_htChildForms[ pMDIChild ] = hwnd;
			}
			else
			{
				hwnd = m_htChildForms[ pMDIChild ] as MDIChildWindow;

				if( hwnd != null )
					hwnd.MessageFilter = hwnd;
			}
		}
		/// <summary>
		/// Method sets MessageFilter as null reference to prevent handling messages
		/// If MDIChild form is removed method also removes references of MDIChild form
		/// from hash table and message filter.
		/// </summary>
		/// <param name="pMDIChild"> MDIChild form which message handling must be stopped. </param>
		/// <param name="pIsRemoved"> True determines that MDIChild form is removed,
		/// otherwise TabbedMDIManager simply changes its state to detached. </param>
		internal void RemoveMDIChildFromHash( Form pMDIChild, bool bIsRemoved )
		{
			MDIChildWindow hwnd = m_htChildForms[ pMDIChild ] as MDIChildWindow;

			if( hwnd != null )
			{
				hwnd.MessageFilter = null;
				hwnd.UpdateRegion( true );

				if( bIsRemoved )
				{
					m_htChildForms.Remove( pMDIChild );
					hwnd.Dispose();
				}

				NativeMethods.SetWindowPos( pMDIChild.Handle, IntPtr.Zero, 0, 0, 0, 0,
					( int ) ( NativeMethods.SetWindowPosFlags.SWP_FRAMECHANGED | NativeMethods.SetWindowPosFlags.SWP_NOACTIVATE |
					NativeMethods.SetWindowPosFlags.SWP_NOMOVE | NativeMethods.SetWindowPosFlags.SWP_NOSIZE | NativeMethods.SetWindowPosFlags.SWP_NOZORDER ) );
			}
            this.childForms.Remove(pMDIChild);
		}
		/// <summary>
		/// Renames tabHosts. Begins renaming from "TabHost_1".
		/// </summary>
		private void RenameTabHosts()
		{
			// Generate a unique ID for the tab hosts.
			string tabHostName = "TabHost_1";

			foreach( TabHost tabHost in tabHostList )
			{
				tabHost.Name = tabHostName;
				tabHostName = IDGenerator.GetNextID( tabHostName );
			}
		}
		/// <summary>
		/// Lets you specify the weights for the tab groups when allocating 
		/// the available space between them.
		/// </summary>
		/// <param name="weights">An array of integers.</param>
		/// <remarks><para>
		/// An integer array with the same count as the current number of tab groups. The sum
		/// of these weights should be greater than 1.
		/// </para><para>
		/// Use <see cref="MaximizeTabGroup"/> to take a tab group take all the available area.
		/// </para></remarks>
		[Obsolete]
		public virtual void SetTabGroupWeights( int[] weights )
		{
		}
		/// <summary>
		/// Divides layout panels and allocates space for Tab hosts
		/// according to default coefficient of each panel.
		/// </summary>
		public void SetTabGroupWeights()
		{
			if( m_lpPanel != null )
			{
				m_lpPanel.SetCustomWeights();
				m_lpPanel.DividePanelOnTwoParts();

				if( m_mdiClient != null )
				{
					m_mdiClient.PerformLayout();
				}
			}
		}
		public void BalanceTDILayout()
		{
			if(m_lpPanel != null)
			{
				m_lpPanel.BalanceEqualWeights ();
				m_lpPanel.DividePanelOnTwoParts();
				if(m_mdiClient != null)
				{
					m_mdiClient.PerformLayout();
				}
			}
		}

        /// <summary>
        /// Divides layout panels and allocates space for Tab hosts
        /// equaly.
        /// </summary>
        public void AdjustTabGroupWeightsEqually()
        {
            if (m_lpPanel != null)
            {
                m_lpPanel.SetEqualWeights();
                m_lpPanel.DividePanelOnTwoParts();

                if (m_mdiClient != null)
                {
                    m_mdiClient.PerformLayout();
                }
            }
        }
		/// <summary>
		/// Call this method to make the tab group hosted in the specified <see cref="TabHost"/>
		/// occupy the maximum space.
		/// </summary>
		/// <param name="tabGroupHost">A <see cref="TabHost"/> instance.</param>
		public void MaximizeTabGroup( TabHost tabGroupHost )
		{
			if( m_lpPanel != null )
			{
				m_lpPanel.MaximizeTabGroup( tabGroupHost, m_lpPanel );
				m_lpPanel.DividePanelOnTwoParts();

				if( m_mdiClient != null )
				{
					m_mdiClient.PerformLayout();
				}
			}
		}
		/// <summary>
		/// Updates scroll offset of currently activated MDI child form.
		/// </summary>
		private void UpdateScrollOffset()
		{
			TabHost thActive = this.ActiveTabHost;

			if( thActive != null )
			{
				MDITabPanel panel = thActive.MDITabPanel;

				if( panel != null )
				{
					ITabPanelRenderer renderer = thActive.MDITabPanel.Renderer;

					if( renderer != null )
					{
						renderer.ValidateScrollOffset( true, false );
					}
				}
			}
		}
		/// <summary>
		/// Suspends MDIClient window and MDI container redrawing.
		/// </summary>
		/// <param name="bLockContainer"> Indicates if MDI container should be suspended for redrawing. </param>
		public virtual void BeginUpdate( bool bLockContainer )
		{
			if( m_mdiClient != null && m_mdiClient.IsHandleCreated )
			{
				if( bLockContainer )
				{
					if( mdiContainer != null && mdiContainer.IsHandleCreated )
					{
						NativeMethodsHelper.SuspendRedrawWindow( mdiContainer.Handle );
					}
				}

				NativeMethodsHelper.SuspendRedrawWindow( m_mdiClient.Handle );
			}
		}
		/// <summary>
		/// Redraws MDIClient window and MDI container.
		/// </summary>
		/// <param name="pMDIContainer"> MDI container</param>
		/// <param name="pMDIClient"> MDI client. </param>
		/// <param name="bLockContainer"> Indicates if MDI container should be redrawn. </param>
		public virtual void EndUpdate( Form pMDIContainer, MdiClient pMDIClient, bool bLockContainer )
		{
			if( pMDIClient != null && pMDIClient.IsHandleCreated )
			{
				NativeMethodsHelper.ResumeRedrawWindow( pMDIClient.Handle, true );

				if( bLockContainer )
				{
					if( pMDIContainer != null && pMDIContainer.IsHandleCreated )
					{
						NativeMethodsHelper.ResumeRedrawWindow( pMDIContainer.Handle, true );
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bLockContainer"></param>
		protected virtual void LockLayout( bool bLockContainer )
		{
			if( m_mdiClient != null && m_mdiClient.IsHandleCreated )
			{
				if( bLockContainer )
				{
					if( mdiContainer != null && mdiContainer.IsHandleCreated )
					{
						mdiContainer.SuspendLayout();
					}
				}
				m_mdiClient.SuspendLayout();
			}
		}
		/// <summary>
		/// Resumes layout for the MDIContainer and MDICLient.
		/// </summary>
		/// <param name="pMDIContainer">Specified MDIContainer.</param>
		/// <param name="pMDIClient">Specified MDIClient.</param>
		/// <param name="bLockContainer">Indicates whether resume layout will perform for MDIContainer.
        /// True - resume layout will be performed.</param>
		public virtual void UnlockLayout( Form pMDIContainer, MdiClient pMDIClient, bool bLockContainer )
		{
			if( pMDIClient != null && pMDIClient.IsHandleCreated )
			{
				pMDIClient.ResumeLayout( true );

				if( bLockContainer )
				{
					if( pMDIContainer != null && pMDIContainer.IsHandleCreated )
					{
						pMDIContainer.ResumeLayout( true );
					}
				}
			}
		}
		/// <summary>
		/// Suspends redrawing specified form.
		/// </summary>
		public virtual void BeginUpdateMDIChild( Form pMDIChild )
		{
			if( pMDIChild != null && pMDIChild.IsHandleCreated )
			{
				NativeMethodsHelper.SuspendRedrawWindow( pMDIChild.Handle );
			}
		}
		/// <summary>
        /// Resumes redrawing for the  specified form.
		/// </summary>
		public virtual void EndUpdateMDIChild( Form pMDIChild )
		{
			if( pMDIChild != null && pMDIChild.IsHandleCreated )
			{
				NativeMethodsHelper.ResumeRedrawWindow( pMDIChild.Handle, true );
			}
		}
		/// <summary>
        /// Asynchronously redraws MDIClient window and MDI container.
		/// </summary>
		public virtual void EndUpdateAsync( bool bLockContainer )
		{
			if( mdiContainer != null && !mdiContainer.IsDisposed && mdiContainer.IsHandleCreated )
			{
				m_mdiClient.BeginInvoke( new EndUpdateMDIClientDelegate( EndUpdate ), new object[] { mdiContainer, m_mdiClient, bLockContainer } );
			}
		}
		/// <summary>
        /// Resumes layout for the MDIContainer and MDICLient.
		/// </summary>
        /// <param name="bLockContainer">Indicates whether resume layout will perform for MDIContainer.
        /// True - resume layout will be performed.</param>
		public virtual void UnlockLayoutAsync( bool bLockContainer )
		{
			if( mdiContainer != null && !mdiContainer.IsDisposed && mdiContainer.IsHandleCreated )
			{
				m_mdiClient.BeginInvoke( new EndUpdateMDIClientDelegate( UnlockLayout ), new object[] { mdiContainer, m_mdiClient, bLockContainer } );
			}
		}
		/// <summary>
        /// Asynchronously resumes redrawing for the specified form.
		/// </summary>
		/// <param name="pMDIChild"></param>
		public virtual void EndUpdateMDIChildAsync( Form pMDIChild )
		{
			if( pMDIChild != null && !pMDIChild.IsDisposed && pMDIChild.IsHandleCreated )
			{
				pMDIChild.BeginInvoke( new EndUpdateMDIChildDelegate( EndUpdateMDIChild ), new object[] { pMDIChild } );
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Fires the <see cref="TabControlAdding"/> event.
		/// </summary>
		/// <param name="e">The event args.</param>
		protected virtual void OnTabControlAdding( TabbedMDITabControlEventArgs e )
		{
			if( this.TabControlAdding != null )
			{
				this.TabControlAdding( this, e );
			}
		}

		/// <summary>
		/// Fires BeforeDropDownPopup.
		/// </summary>
		/// <param name="e"></param>
		protected internal virtual void OnDropDownPopup( DropDownPopupEventArgs e )
		{
			if( this.BeforeDropDownPopup != null )
			{
				this.BeforeDropDownPopup( this, e );
			}
		}

		/// <summary>
		/// Overridden
		/// </summary>
		protected virtual void OnNeedUpdateHostedFormChanged()
		{
			if( mdiContainer == null )
			{
				return;
			}

			MdiSysMenuProvider mdiProvider = MdiSysMenuManager.GetProviderForForm( mdiContainer );
			if( mdiProvider != null )
			{
				mdiProvider.NeedUpdateHostedForm = m_bNeedUpdateHostedForm;
			}

		}

		/// <summary>
		/// Fires the <see cref="TabControlAdded"/> event.
		/// </summary>
		/// <param name="e">The event args.</param>
		protected virtual void OnTabControlAdded( TabbedMDITabControlEventArgs e )
		{
			if( this.TabControlAdded != null )
			{
				this.TabControlAdded( this, e );
			}
		}

		/// <summary>
		/// Fires the <see cref="OnTabControlRemoving"/> event.
		/// </summary>
		/// <param name="e">The event args.</param>
		protected virtual void OnTabControlRemoving( TabbedMDITabControlEventArgs e )
		{
			if( this.TabControlRemoved != null )
			{
				this.TabControlRemoved( this, e );
			}
		}

		/// <summary></summary>
		/// <param name="e"/>
		protected virtual void OnUnLockingMdiClient( EventArgs e )
		{
			if( this.UnLockingMdiClient != null )
			{
				this.UnLockingMdiClient( this, e );
			}
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		[DocumentationExclude()]
		protected virtual void ContextMenu_BeforePopup( object sender, CancelEventArgs e )
		{
			if( this.CausesFormValidation )
			{
				Control sourceControl = this.ContextMenu.GetPopupParentControl();
				if( sourceControl != null )
				{
					MDITabPanel tabControl = sourceControl as MDITabPanel;
					TabHost tabHost = tabControl.Parent as TabHost;
					if( ActiveTabHost != tabHost )
					{
						e.Cancel = true;
					}
				}
			}
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		[DocumentationExclude()]
		protected virtual void ContextMenu_PopupClosed( object sender, EventArgs e )
		{
			this.CloseItem.Visible = m_bOldCloseItemVisibility;
			ParentBarItem parentItem = ( ParentBarItem ) sender;
			if( !this.showDragMenu )
			{
				// If the corresponding form implementes the ITabbedMDIChildForm interface,
				// then notify it regarding the popup.
				Control sourceControl = this.ContextMenu.GetPopupParentControl();
				if( sourceControl != null )
				{
					MDITabPanel tabControl = sourceControl as MDITabPanel;
					if( tabControl != null )
					{
						Form form = ( Form ) tabControl.SelectedTab.Tag;
						if( form is ITabbedMDIChildForm )
						{
							ITabbedMDIChildForm childForm = form as ITabbedMDIChildForm;
							childForm.OnTabContextMenuClosed( parentItem );
						}
					}
				}
			}
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		[DocumentationExclude()]
		protected virtual void ContextMenu_Popup( object sender, EventArgs e )
		{
			ParentBarItem parentItem = ( ParentBarItem ) sender;
			MDITabPanel tab = this.ContextMenu.GetPopupParentControl() as MDITabPanel;
			foreach( BarItem barItem in parentItem.Items )
			{
				// If not shown in the context of the tabs, then hide the menus
				if( tab == null )
				{
					barItem.Visible = false;
					continue;
				}
				if( barItem == this.CloseItem )
				{
					if( tab.IsCloseButtonActive() )
					{
						barItem.Enabled = true;
					}
					else
					{
						barItem.Enabled = false;
					}

					m_bOldCloseItemVisibility = barItem.Visible;
					if( this.showDragMenu )
					{
						barItem.Visible = false;
					}
					//else barItem.Visible = true;
				}
				else if( barItem == this.CancelItem )
				{
					if( this.showDragMenu )
					{
						barItem.Visible = true;
					}
					else
					{
						barItem.Visible = false;
					}
				}
				else if( barItem == this.NewHorzGroupItem )
				{
					barItem.Visible = this.CanCreateNewHorizontalGroup( tab )
						&& this.AllowTabGroupCustomizing;
				}
				else if( barItem == this.NewVertGroupItem )
				{
					barItem.Visible = this.CanCreateNewVerticalGroup( tab )
						&& this.AllowTabGroupCustomizing;
				}
				else if( barItem == this.MovePrevGroupItem )
				{
					barItem.Visible = this.CanMoveToPreviousTabGroup( tab )
						&& this.AllowTabGroupCustomizing;
				}
				else if( barItem == this.MoveNextGroupItem )
				{
					barItem.Visible = this.CanMoveToNextTabGroup( tab )
						&& this.AllowTabGroupCustomizing;
				}
				else
				{
					// For items supplied by user, do not show them when in drag mode
					if( this.showDragMenu )
					{
						barItem.Visible = false;
					}
					else
					{
						barItem.Visible = true;
					}

				}
			}
			if( !this.showDragMenu )
			{
				// If the corresponding form implementes the ITabbedMDIChildForm interface,
				// then notify it regarding the popup.
				Form form = ( Form ) ActiveTabHost.MDITabPanel.SelectedTab.Tag;
				if( form is ITabbedMDIChildForm )
				{
					ITabbedMDIChildForm childForm = form as ITabbedMDIChildForm;
					childForm.OnTabContextMenuPopup( parentItem );
				}
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="tabPanel"/>
		[DocumentationExclude()]
		protected virtual bool CanCreateNewHorizontalGroup( MDITabPanel tabPanel )
		{
            if( this.ActiveTabHost == null || this.ActiveTabHost.MDITabPanel == null || tabPanel == null )
            {
                return false;
            }

			bool bCanAddToGroupedManager = tabPanel.TabPages.Count > 1;

            bool enoughSpace = true;
            TabPageAdv selectedTab = this.ActiveTabHost.MDITabPanel.SelectedTab;
            Form mdiChild = selectedTab.Tag as Form;
            if( mdiChild != null )
            {
                int height = mdiChild.Height - this.ActiveTabHost.BottomBorderHeight - 1;
            
                if( tabPanel.BorderVisible )
                {
                    height -= tabPanel.BorderWidth;
                }

                enoughSpace = tabPanel.IsVerticalAlignment || !tabPanel.IsVerticalAlignment && height > tabPanel.Height;
            }

            if( bCanAddToGroupedManager && enoughSpace )
			{
				// Verify if the selected tab is movable
				TabPageAdv tabPage = tabPanel.SelectedTab;
				if( tabPage != null )
				{
					Form form = tabPage.Tag as Form;
					if( form is ITabbedMDIChildForm )
					{
						if( !( ( ITabbedMDIChildForm ) form ).AllowUserDrag )
						{
							return false;
						}
					}
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="tabPanel"/>
		[DocumentationExclude()]
		protected virtual bool CanCreateNewVerticalGroup( MDITabPanel tabPanel )
		{
            if( this.ActiveTabHost == null || this.ActiveTabHost.MDITabPanel == null || tabPanel == null )
            {
                return false;
            }

			bool bCanAddToGroupedManager =  tabPanel.TabPages.Count > 1;

            bool enoughSpace = true;
            TabPageAdv selectedTab = this.ActiveTabHost.MDITabPanel.SelectedTab;
            Form mdiChild = selectedTab.Tag as Form;
            if( mdiChild != null )
            {
                int width = mdiChild.Width - this.ActiveTabHost.BottomBorderHeight - 1;

                if( tabPanel.BorderVisible )
                {
                    width -= tabPanel.BorderWidth;
                }

                enoughSpace = !tabPanel.IsVerticalAlignment || tabPanel.IsVerticalAlignment && width > tabPanel.Width;
            }

			if( bCanAddToGroupedManager && enoughSpace )
			{
				// Verify if the selected tab is movable
				TabPageAdv tabPage = tabPanel.SelectedTab;
				if( tabPage != null )
				{
					Form form = tabPage.Tag as Form;
					if( form is ITabbedMDIChildForm )
					{
						if( !( ( ITabbedMDIChildForm ) form ).AllowUserDrag )
						{
							return false;
						}
					}
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="tabPanel"/>
		[DocumentationExclude()]
		protected virtual bool CanMoveToPreviousTabGroup( MDITabPanel tabPanel )
		{
			TabHost thTabHost = tabPanel.Parent as TabHost;

			RefineTabHostSiblings( thTabHost.PreviousTabHost );

			if( thTabHost != null && thTabHost.PreviousTabHost.Count > 0 )
			{
				// Verify if the selected tab is movable
				TabPageAdv tabPage = tabPanel.SelectedTab;
				if( tabPage != null )
				{
					Form form = tabPage.Tag as Form;
					if( form is ITabbedMDIChildForm )
					{
						if( !((ITabbedMDIChildForm)form).AllowUserDrag )
						{
							return false;
						}
					}
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="tabPanel"/>
		[DocumentationExclude()]
		protected virtual bool CanMoveToNextTabGroup( MDITabPanel tabPanel )
		{
			TabHost thTabHost = tabPanel.Parent as TabHost;

			RefineTabHostSiblings( thTabHost.NextTabHost );

			if( thTabHost != null && thTabHost.NextTabHost.Count > 0 )
			{
				// Verify if the selected tab is movable
				TabPageAdv tabPage = tabPanel.SelectedTab;
				if( tabPage != null )
				{
					Form form = tabPage.Tag as Form;
					if( form is ITabbedMDIChildForm )
					{
						if( !((ITabbedMDIChildForm)form).AllowUserDrag )
						{
							return false;
						}
					}
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="tab"/>
		[DocumentationExclude()]
		protected TabHost GetTabHostFromTab( MDITabPanel tab )
		{
			if( tab.Parent is TabHost )
			{
				return tab.Parent as TabHost;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Overridden
		/// </summary>
		/// <param name="mdiChild"/>
		/// <param name="prevHost"/>
		/// <param name="newHost"/>
		[DocumentationExclude()]
		protected virtual void OnActiveTabToNewGroupMoved( Form mdiChild, TabHost prevHost, TabHost newHost )
		{
			// do nothing here
		}

		/// <summary></summary>
		[DocumentationExclude()]
		protected void UnLockMdiClient( Form mdiContainer, MdiClient mdiClient )
		{
			if( this.AllowMDIClientLocking )
			{
				if( mdiClient != null && mdiClient.IsHandleCreated )
				{
					NativeMethodsHelper.ResumeRedrawWindow( mdiClient.Handle, true );

					if( mdiContainer != null && mdiContainer.IsHandleCreated )
					{
						NativeMethodsHelper.ResumeRedrawWindow( mdiContainer.Handle, true );
					}
				}
			}
		}

		/// <summary></summary>
		[DocumentationExclude()]
		protected void UnLockMdiClientAsync()
		{
			if( null != this.mdiContainer && !mdiContainer.IsDisposed && this.mdiContainer.IsHandleCreated )
			{
				m_mdiClient.BeginInvoke( new UnlockMDIClientDelegate( UnLockMdiClient ),
					new object[] { mdiContainer, m_mdiClient } );
			}
		}

		private delegate void UnlockMDIClientDelegate( Form mdiContainer, MdiClient mdiClient );
		private delegate void UnlockMDIChildDelegate( Form mdiChild );

		/// <summary></summary>
		[DocumentationExclude()]
		protected void LockMdiClient()
		{
			if( this.AllowMDIClientLocking )
			{
				if( m_mdiClient != null && m_mdiClient.IsHandleCreated )
				{
					if( mdiContainer != null && mdiContainer.IsHandleCreated )
					{
						NativeMethodsHelper.SuspendRedrawWindow( mdiContainer.Handle );
					}

					NativeMethodsHelper.SuspendRedrawWindow( m_mdiClient.Handle );
				}
			}
		}

		/// <summary></summary>
		/// <param name="mdiChild"/>
		[DocumentationExclude()]
		protected void LockMDIChild( Form mdiChild )
		{
			if( null != mdiChild && mdiChild.IsHandleCreated )
			{
				NativeMethodsHelper.SuspendRedrawWindow( mdiChild.Handle );
			}
		}

		/// <summary></summary>
		/// <param name="mdiChild"/>
		[DocumentationExclude()]
		protected void UnlockMDIChild( Form mdiChild )
		{
			if( null != mdiChild && mdiChild.IsHandleCreated )
			{
				NativeMethodsHelper.ResumeRedrawWindow( mdiChild.Handle, true );
			}
		}

		/// <summary></summary>
		/// <param name="mdiChild"/>
		[DocumentationExclude()]
		protected void UnlockMDIChildAsync( Form mdiChild )
		{
			if( null != mdiChild && !mdiChild.IsDisposed && mdiChild.IsHandleCreated )
			{
				mdiChild.BeginInvoke( new UnlockMDIChildDelegate( UnlockMDIChild ), new object[] { mdiChild } );
			}
		}

		/// <summary></summary>
		[DocumentationExclude()]
		protected void MoveActiveTabToNewGroup( bool pHorizontal )
		{
			BeginUpdate( false );

			shouldKeepImageIndex = true;

			TabHost prevHost = this.ActiveTabHost;
			TabPageAdv selectedTab = prevHost.MDITabPanel.SelectedTab;
			MDIChildTabData prevTabData = new MDIChildTabData( selectedTab.TabData as MDIChildTabData );

			// Remove from current tabHost
			Form mdiChild = selectedTab.Tag as Form;
			mdiChild.SuspendLayout();

			prevHost.RemoveMdiChild( mdiChild, true );

			// Create a new tabHost and add to it
			TabHost tabHost = CreateTabHostInternal( tabHostList.Count );
			tabHost.AddMdiChild( mdiChild, prevTabData );

			shouldKeepImageIndex = false;

			PostInitTabHost( tabHost );

			SetNextTabHost(prevHost, tabHost);
			SetPreviousTabHost(tabHost, prevHost);

			OnActiveTabToNewGroupMoved(mdiChild, prevHost, tabHost);

			// Create LayoutPanel container to save layout info about TabHosts and SplitterHosts.
			if( m_lpPanel == null || m_lpPanel.ComponentOne == null || m_lpPanel.ComponentTwo == null )
			{
				m_lpPanel = new LayoutPanel( prevHost, tabHost, m_mdiClient.DisplayRectangle.Size, m_mdiClient.DisplayRectangle.Location, pHorizontal );
				m_lpPanel.DividePanelOnTwoParts();

				// Set TabHost's SplitterHosts bounds.
				ResetSplitterHostsBounds();
				m_lpPanel.SetSplitterHosts();
			}
			else
			{
				LayoutPanel panel = prevHost.LayoutPanel;

				if( panel != null )
				{
					panel.DivideTabHost( prevHost, tabHost, pHorizontal );

					// Set TabHost's SplitterHosts bounds.
					ResetSplitterHostsBounds();
					m_lpPanel.SetSplitterHosts();
				}
			}

			AddTabHosts( tabHost );

			mdiChild.ResumeLayout();
			m_mdiClient.PerformLayout();

			EndUpdateAsync( false );
		}
		/// <summary>
		/// Set TabHost's SplitterHosts bounds to Rectangle.Empty value.
		/// </summary>
		private void ResetSplitterHostsBounds()
		{
			for( int i = 0; i < tabHostList.Count; i++ )
			{
				TabHost tabHost = tabHostList[ i ] as TabHost;

				if( tabHost != null )
				{
					tabHost.SplitterBounds = Rectangle.Empty;
					if( tabHost.SplitterHost != null )
					{
						tabHost.SplitterHost.Bounds = Rectangle.Empty;
					}
				}
			}
		}
		/// <summary></summary>
		/// <returns></returns>
		/// <param name="tabsHostRemoved"/>
		[DocumentationExclude()]
		protected Form RemoveActiveDocFromTabHost( ref bool tabsHostRemoved )
		{
			Form mdiChild = ActiveTabHost.MDITabPanel.SelectedTab.Tag as Form;
			TabHost tabsHostToRemoveFrom = ActiveTabHost;

			RemoveDocFromTabHost( tabsHostToRemoveFrom, mdiChild, ref tabsHostRemoved );

			return mdiChild;
		}
		/// <summary></summary>
		/// <param name="tabHost"/>
		/// <param name="mdiChild"/>
		/// <param name="tabHostRemoved"/>
		[DocumentationExclude()]
		protected void RemoveDocFromTabHost( TabHost tabHost, Form mdiChild, ref bool tabHostRemoved )
		{
			mdiChild.SuspendLayout();

			tabHost.RemoveMdiChild( mdiChild, true );
			// Removing a tab could remove the tabHost too.
			if( this.tabHostList.IndexOf( tabHost ) == -1 )
			{
				tabHostRemoved = true;
			}

			mdiChild.ResumeLayout( false );
		}
		/// <summary></summary>
		/// <param name="next"/>
		[DocumentationExclude()]
		protected void MoveActiveDocToAdjTabHost( bool bNext )
		{
			TabHost thActiveTabHost = this.ActiveTabHost;

			if( ( !bNext && thActiveTabHost.PreviousTabHost.Count == 0 ) // Cannot move previous.
				|| ( bNext && thActiveTabHost.NextTabHost.Count == 0 ) )
			{
				return;
			}

			bool bTabsHostGotRemoved = false;
			Form mdiChild = null;

			BeginUpdate( true );
			SuspendLayout();

			shouldKeepImageIndex = true;

			TabPageAdv tpSelectedTab = thActiveTabHost.MDITabPanel.SelectedTab;
			MDIChildTabData tdTabData = new MDIChildTabData( tpSelectedTab.TabData as MDIChildTabData );

			// Remove from current tabHost
			mdiChild = this.RemoveActiveDocFromTabHost( ref bTabsHostGotRemoved );
			mdiChild.SuspendLayout();

			// Get the adjacent tabHost and add to it
			TabHost thTabHost = bNext ?
				thActiveTabHost.NextTabHost[ 0 ] as TabHost :
				thActiveTabHost.PreviousTabHost[ 0 ] as TabHost;

			thTabHost.AddMdiChild( mdiChild, tdTabData );

			shouldKeepImageIndex = false;

			OnActiveTabToNewGroupMoved( mdiChild, thActiveTabHost, thTabHost );

			if( !bTabsHostGotRemoved )
			{
				// If thActiveTabHost lays out down to just created TabHost then.
				SetNextTabHost( thActiveTabHost, thTabHost );
				SetPreviousTabHost( thTabHost, thActiveTabHost );
				AddTabHosts( thTabHost );
			}

			mdiChild.ResumeLayout();

			ResumeLayoutInternal();
			MdiClient.PerformLayout();
			EndUpdateAsync( true );
		}

		/// <summary>
		/// The event handler for the "New Horizontal Tab Group" menu item.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The event data.</param>
		protected void ContextMenu_NewHorz( object sender, EventArgs e )
		{
			this.CreateNewHorizontalGroup();
		}

		/// <summary>
		/// The event handler for the "Cancel" menu item.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The event data.</param>
		protected void ContextMenu_Cancel( object sender, EventArgs e )
		{
		}

		/// <summary>
		/// The event handler for the "Close" menu item.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The event data.</param>
		protected virtual void ContextMenu_Close( object sender, EventArgs e )
		{
			CloseActiveChildForm();
		}

		/// <summary>
		/// The event handler for the "New Vertical Tab Group" menu item.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The event data.</param>
		protected void ContextMenu_NewVert( object sender, EventArgs e )
		{
			this.CreateNewVerticalGroup();
		}

		/// <summary>
		/// The event handler for the "Move Previous" menu item.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The event data.</param>
		protected void ContextMenu_MovePrev( object sender, EventArgs e )
		{
			MoveActiveDocToAdjTabHost( false );
		}

		/// <summary>
		/// The event handler for the "Move Next" menu item.
		/// </summary>
		/// <param name="sender">The event source.</param>
		/// <param name="e">The event data.</param>
		protected void ContextMenu_MoveNext( object sender, EventArgs e )
		{
			MoveActiveDocToAdjTabHost( true );
		}

		/// <summary>
		/// Initializes the tab control representing a tab group.
		/// </summary>
		/// <param name="tabPanel">A <see cref="TabControlAdv"/> derived instance.</param>
		protected internal virtual void InitMDITabPanel( MDITabPanel tabPanel )
		{
			// MDITabPanel is a TabControlAdv derived class.
			tabPanel.Padding = new Point( 6, 0 );
			tabPanel.Size = new Size( 10, 19 );

			TabbedMDITabControlEventArgs e = new TabbedMDITabControlEventArgs( tabPanel );
			this.OnTabControlAdded( e );
		}

		/// <summary>
		/// Creates and returns an MDITabPanel.
		/// </summary>
		/// <returns>A reference to an MDITabPanel control.</returns>
		/// <remarks><para>
		/// MDITabPanel is a TabControlAdv derived class used internally by TabbedMDIManager. You
		/// can use this instance just as you would any TabControlAdv instance.
		/// </para><para>
		/// You can customize the tab being drawn by providing a custom MDITabPanel derived
		/// tab or modifying the properties of the MDITabPanel instance returned by the base class.
		/// </para></remarks>
		protected internal virtual MDITabPanel CreateMDITabPanel()
		{
			TabbedMDITabControlEventArgs e = new TabbedMDITabControlEventArgs( null );
			this.OnTabControlAdding( e );

			if( e.TabControl != null )
			{
				return e.TabControl;
			}

			return new MDITabPanel( this );
		}

		/// <summary></summary>
		/// <param name="control"/>
		/// <param name="interval"/>
		[DocumentationExclude(), Obsolete()]
		protected virtual void LockWindowTemporarily( Control control, int interval )
		{
		}

		/// <summary></summary>
		[Obsolete, DocumentationExclude()]
		protected virtual void UnLockWindows()
		{
		}

		/// <summary>
		/// Called when an mdi child form gets removed.
		/// </summary>
		/// <param name="form">The mdi child Form.</param>
		protected virtual void OnMdiChildRemoved( Form form )
		{
			// Don't know the corresponding tabshost, so parsing them all.
			foreach( TabHost tabHost in this.tabHostList )
			{
				if( tabHost != null && tabHost.MDITabPanel != null && tabHost.RemoveMdiChild( form, true ) )
				{
					break;
				}
			}
		}

		/// <summary>
		/// Raised when size of the image in the mdi tabs is changed.
		/// </summary>
		protected virtual void OnImageSizeChanged()
		{
			if( imageList != null )
			{
				imageList.ImageSize = m_imageSize;
			}
		}


		/// <summary></summary>
		/// <returns></returns>
		Form ITabbedMDIManager.GetMDIParent()
		{
			return this.MdiParent;
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="msg"/>
		/// <param name="keyData"/>
		bool ITabbedMDIManager.ProcessCmdKey( ref Message msg, Keys keyData )
		{
			return this.ProcessCmdKey( ref msg, keyData );
		}

		/// <summary>
		/// Called when ShowCloseButton property is changed
		/// </summary>
		protected virtual void OnShowCloseButtonChanged()
		{
			// Update all existing tab controls
			foreach( TabHost tabHost in this.tabHostList )
			{
				tabHost.MDITabPanel.ShowTabCloseButton = m_bShowCloseButton;
                tabHost.MDITabPanel.OnShowCloseButtonChanged();
			}
		}

		// Will create a new tabHost at the specified index in the tabHostList array
		// Will insert nulls to accomodate position.
		// Returns true if the tabName was founds, else false
		/// <summary></summary>
		/// <returns></returns>
		/// <param name="tabName"/>
		/// <param name="groupIndex"/>
		[DocumentationExclude()]
		protected virtual bool MoveDocumentTo( string tabName, int groupIndex )
		{
			if( tabHostList.Count == 0 )
				return false;

			// Find the corresponding document
			int tabHostIndex = ( m_bSearchFromStart ) ? 0 : tabHostList.Count - 1;
			bool shouldProcess = true;

			while( shouldProcess )
			{
				TabHost tabHost = tabHostList[ tabHostIndex ] as TabHost;

				if( tabHost != null )
				{
					foreach( TabPageAdv tabPage in tabHost.MDITabPanel.TabPages )
					{
						if( tabPage.Text == tabName && m_htPagesToSkip[ tabPage ] == null )
						{
							if( m_bShouldConsiderPageSkip )
							{
								m_htPagesToSkip[ tabPage ] = tabPage;
							}

							// Found the document, check if it needs to be moved
							if( tabHostIndex != groupIndex )
							{
								Form mdiChild = tabPage.Tag as Form;
								MDIChildTabData savedData = new MDIChildTabData( tabPage.TabData as MDIChildTabData );

								// Remove it from the current host
								tabHost.RemoveMdiChild( mdiChild, true );
								// Create the tabHost dest if necessary
								TabHost tabHostDest;
								bool newTabHost = true;

								if( groupIndex > tabHostList.Count - 1 || tabHostList[ groupIndex ] == null )
								{
									tabHostDest = CreateTabHostInternal( groupIndex );
								}
								else
								{
									tabHostDest = this.tabHostList[ groupIndex ] as TabHost;
									newTabHost = false;
								}

								// Add to destination
								tabHostDest.AddMdiChild( mdiChild, savedData );

								SetNextTabHost( tabHost, tabHostDest );
								SetPreviousTabHost( tabHostDest, tabHost );
								AddTabHosts( tabHost );

								if( newTabHost )
								{
									PostInitTabHost( tabHostDest );
								}
							}

							return true;
						}
					}
				}

				if( m_bSearchFromStart )
				{
					tabHostIndex++;
					shouldProcess = ( tabHostIndex < this.tabHostList.Count );
				}
				else
				{
					tabHostIndex--;
					shouldProcess = ( tabHostIndex >= 0 );
				}
			}

			return false;
		}
		// This will be called when hosted in a native app.
		/// <summary></summary>
		/// <returns></returns>
		/// <param name="wParam"/>
		/// <param name="lParam"/>
		bool IKeyboardProcHookClient.KeyboardHookProc( int wParam, int lParam )
		{
			// If key down
			if( ( lParam & 0x80000000 ) == 0 )
			{
				// Always forward exceptions caught here to the Application class so that Application.ThreadException listeners will 
				// get to handle it.
				try
				{
					Keys keys = ( Keys ) wParam;
					keys |= Control.ModifierKeys;
					if( this.ProcessCmdKey( keys, false ) )
					{
						return true;
					}
				}
				catch( Exception e )
				{
					Application.OnThreadException( e );
				}
			}

			return false;
		}

		/// <summary></summary>
		/// <param name="mdiListMenuItem"/>
		[DocumentationExclude()]
		protected virtual void DetachMdiListMenuItem( MenuItem mdiListMenuItem )
		{
			// To supress exception caused by bug in MenuItem.Popup event remove handler implementaion.
			try
			{
				mdiListMenuItem.Popup -= new EventHandler( this.MdiListItemPopup );
			}
			catch( Exception ex )
			{
				Debug.WriteLine( ex.Message + Environment.NewLine + ex.StackTrace );
			}
			this.RemoveMdiListItems( mdiListMenuItem );
		}

		/// <summary></summary>
		/// <param name="mdiListMenuItem"/>
		[DocumentationExclude()]
		protected virtual void AttachMdiListMenuItem( MenuItem mdiListMenuItem )
		{
			mdiListMenuItem.Popup += new EventHandler( this.MdiListItemPopup );
			this.AttachMdiListItems( mdiListMenuItem );
		}

		/// <summary></summary>
		[DocumentationExclude()]
		protected virtual Form[] GetMdiChildrenForMenu()
		{
			if( this.mdiContainer != null )
			{
				return MdiChildren;
			}
			else
			{
				return MdiListMenuItem.GetMainMenu().GetForm().MdiChildren;
			}
		}

		/// <summary></summary>
		[DocumentationExclude()]
		protected virtual void UpdateMdiList()
		{
			// If the MdiListMenuItem is on a context menu then this is supported only when
			// tabbed MDI is on.
			if( ( this.MdiListMenuItem.GetMainMenu() == null && this.mdiContainer == null ) || !this.GetMdiContainer().IsMdiContainer )
			{
				return;
			}

			// First, make them all invisible
			foreach( MenuItem item in this.mdiChildMenuList )
			{
				item.Visible = false;
				item.Checked = false;
			}

			Form[] mdiChildren = this.GetMdiChildrenForMenu();

			if( mdiChildren.Length == 0 )
			{
				return;
			}
			else
			{
				MenuItem separator = this.mdiChildMenuList[ 0 ] as MenuItem;
				separator.Visible = true;

				int index = 1;
				for( ; index < this.mdiChildMenuList.Count - 1
					&& index <= mdiChildren.Length; index++ )
				{
					// Init the menu items with the right caption and corresponding
					// entry in the hash table.
					Form mdiChild = mdiChildren[ index - 1 ];
					string caption = "&" + index.ToString() + " " + mdiChild.Text;

					MenuItem item = this.mdiChildMenuList[ index ] as MenuItem;
					item.Text = caption;
					item.Visible = true;

					if( this.GetMdiContainer().ActiveMdiChild == mdiChild )
					{
						item.Checked = true;
					}

					this.mdiChildrenByMenuItems[ item ] = mdiChild;
				}
				index--;
				// Include the "Windows..." entry if necessary
				if( mdiChildren.Length > index )
				{
					index++;
					MenuItem item = this.mdiChildMenuList[ index ] as MenuItem;
					item.Text = "&Windows...";
					item.Visible = true;

					this.mdiChildrenByMenuItems[ item ] = this.GetMdiContainer();
				}
			}
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="item"/>
		[DocumentationExclude()]
		protected virtual Form GetMdiChildFromMdiListMenuItem( MenuItem item )
		{
			return this.mdiChildrenByMenuItems[ item ] as Form;
		}

		/// <summary></summary>
		/// <param name="mdiListItem"/>
		[DocumentationExclude()]
		protected virtual void AttachMdiListItems( MenuItem mdiListItem )
		{
			if( this.mdiListMenuItem == null )
			{
				return;
			}

			MenuItem separator = new MenuItem( "-" );
			separator.Visible = false;
			mdiListItem.MenuItems.Add( separator );
			this.mdiChildrenByMenuItems[ separator ] = this;
			mdiChildMenuList.Add( separator );

			for( int i = 1; i <= 10; i++ )
			{
				// Just add an entry for the menu items; the text will be set
				// in Popup event handler.
				MenuItem item = new MenuItem();
				item.Visible = false;
				item.Click += new EventHandler( MdiListItemClicked );

				// Will use this array to set the caption
				mdiChildMenuList.Add( item );
				// Will use this hashtable in the click event handler
				this.mdiChildrenByMenuItems[ item ] = this.mdiContainer;
				// To insert it into the menu.
				mdiListItem.MenuItems.Add( item );
			}
		}

		/// <summary></summary>
		/// <param name="mdiListItem"/>
		[DocumentationExclude()]
		protected virtual void RemoveMdiListItems( MenuItem mdiListItem )
		{
			if( this.mdiListMenuItem == null )
			{
				return;
			}

			foreach( MenuItem item in this.mdiChildMenuList )
			{
				this.mdiListMenuItem.MenuItems.Remove( item );
			}

			this.mdiChildMenuList.Clear();
			this.mdiChildrenByMenuItems.Clear();
		}

		/// <summary>
		/// Adds a form to a tab host.
		/// </summary>
		/// <param name="mdiChild">The form to add.</param>
		/// <param name="tabHost">The destination <see cref="TabHost"/>.</param>
		protected virtual void AddMdiChild( Form mdiChild, TabHost tabHost )
		{
			if( mdiChild != null )
			{
				// Check MinimumSize and MaximumSize properties for default values.
				if( mdiChild.MinimumSize != Size.Empty )
				{
					throw new ArgumentException( "MDIChild form which is hosted in a TabbedMDIManager " +
						"must have default MinimumSize value ( 0;0 ), when TabbedMDiManager mode is on.",
						"MinimumSize" );
				}
				if( mdiChild.MaximumSize != Size.Empty )
				{
					throw new ArgumentException( "MDIChild form which is hosted in a TabbedMDIManager " +
						"must have default MaximumSize value ( 0;0 ), when TabbedMDiManager mode is on.",
						"MaximumSize" );
				}

				// The child cannot stay minimized when we do this.
				if( mdiChild.WindowState == FormWindowState.Minimized )
				{
					mdiChild.WindowState = FormWindowState.Normal;
				}

				using( MDIChildTabData tabData = new MDIChildTabData( tabHost.MDITabPanel.TabPanelData, mdiChild, this, null ) )
				{
					tabData.ToolTip = GetTooltip( mdiChild );

					if( mdiChild.Icon != null )
					{
						Image im = CorrectImageSize( DrawIconHelper.GetIconToDraw( mdiChild.Icon, mdiChild ), m_imageSize );
						ImageList.ImageCollection images = this.imageList.Images;

						images.Add( im );
						tabData.ImageIndex = images.Count - 1;
					}

					tabHost.AddMdiChild( mdiChild, tabData );
				}
			}
		}

		/// <summary></summary>
		[DocumentationExclude(), Obsolete()]
		protected void AdjustMdiChildSizes()
		{
		}

		/// <summary>
		/// Creates a <see cref="TabHost"/> to host a tab group (in a tab control).
		/// </summary>
		/// <returns>A new <see cref="TabHost"/> instance.</returns>
		protected virtual TabHost CreateTabHost()
		{
			return new TabHost( this );
		}

		/// <summary>
		/// Initializes the <see cref="TabHost"/> as soon as it gets created.
		/// </summary>
		/// <param name="tabHost">The <see cref="TabHost"/> to initialize.</param>
		/// <param name="tabGroupIndex">The tab group index which this tab host will represent.</param>
		protected virtual void InitTabHost( TabHost tabHost, int tabGroupIndex )
		{
			// Insert dummy objects; to accomodate inserting at tabHostIndex
			for( int i = tabGroupIndex - tabHostList.Count + 1; i > 0; i-- )
			{
				tabHostList.Add( null );
			}

			tabHostList[ tabGroupIndex ] = tabHost;

			tabHost.MDITabPanel.ImageList = imageList;
			tabHost.MDITabPanel.MouseMove += new MouseEventHandler( Tab_MouseMove );
			tabHost.MDITabPanel.MouseUp += new MouseEventHandler( Tab_MouseUp );
			popupManager.SetXPContextMenu( tabHost.MDITabPanel, ContextMenu);
			tabHost.MDITabPanel.ThemesEnabled = themesEnabled;

			// Also create a corresponding Splitter host
			for( int i = tabGroupIndex - splitterHostList.Count + 1; i > 0; i-- )
			{
				splitterHostList.Add( null );
			}

			SplitterHost splitterHost = new SplitterHost( this );
			splitterHostList[ tabGroupIndex ] = splitterHost;

			tabHost.ListenToSplitterHost( splitterHost );

			autoScaleBaseMdiClientDim = 0;
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="tabsHostSource"/>
		/// <param name="mousePos"/>
		[DocumentationExclude()]
		protected virtual Form GetTabHostUnder( MDITabPanel tabsHostSource, Point mousePos )
		{
			mousePos = tabsHostSource.PointToScreen( mousePos );
			foreach( Form mdiChild in this.mdiContainer.MdiChildren )
			{
				Rectangle bounds = mdiChild.ClientRectangle;
				if( mdiChild.Visible )
				{
					bounds = mdiChild.RectangleToScreen( bounds );
					if( bounds.Contains( mousePos ) )
					{
						return mdiChild;
					}
				}
			}
			return null;
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		[DocumentationExclude()]
		protected virtual void Tab_MouseUp( object sender, MouseEventArgs e )
		{
			if( Dragging )
			{
				Dragging = false;
				Form destForm = GetTabHostUnder( ( MDITabPanel ) sender, new Point( e.X, e.Y ) );

				if( destForm != ActiveTabHost )
				{
					if( destForm is TabHost )
					// Drag and drop possible, go ahead and do it.
					{
						this.MoveActiveDocTo( ( TabHost ) destForm );
					}
					else if( destForm != null )
					{
						// Show a context menu
						MDITabPanel tab = ( MDITabPanel ) sender;
						this.showDragMenu = true;
						this.ContextMenu.Show( tab, new Point( e.X, e.Y ) );
						this.showDragMenu = false;
					}
				}
			}
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		[DocumentationExclude()]
		protected virtual void Tab_MouseMove( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Left && ActiveTabHost != null
				&& AllowTabGroupCustomizing )
			{
				if( !Dragging )
				{
					// Check if the ActiveTabHost is where this mouse move happened
					// (this could be on an inactive TabHost if validation failed in the active form)
					Rectangle activeTabScreenRect = ActiveTabHost.MDITabPanel.RectangleToScreen( ActiveTabHost.MDITabPanel.ClientRectangle );
					if( activeTabScreenRect.Contains( Control.MousePosition ) )
					{
						// Try to start a drag
						int tabHit = ActiveTabHost.MDITabPanel.HitTestTabs( new Point( e.X, e.Y ), false );
						if( tabHit != -1 )
						{
							TabPageAdv tabPage = ActiveTabHost.MDITabPanel.TabPages[ tabHit ] as TabPageAdv;
							Form mdiChild = tabPage.Tag as Form;
							bool drag = true;
							if( mdiChild is ITabbedMDIChildForm )
							{
								drag = ( ( ITabbedMDIChildForm ) mdiChild ).AllowUserDrag;
							}
							Dragging = drag;
						}
					}
				}
				else
				{
					// In the middle of drag op, provide appropriate feedback
					bool dropRectFound = false;
					Form destForm = GetTabHostUnder( ( MDITabPanel ) sender, new Point( e.X, e.Y ) );
					if( destForm != this.ActiveTabHost )
					{
						if( destForm is TabHost )
						{
							TabHost tabHost = ( TabHost ) destForm;
							Form mdiChild = ( Form ) tabHost.MDITabPanel.SelectedTab.Tag;
							Rectangle bounds = mdiChild.ClientRectangle;
							bounds = mdiChild.RectangleToScreen( bounds );
							this.DropRect = bounds;
							dropRectFound = true;
							Cursor.Current = MDITabsDragCursors.DropCursor;
						}
						else
						{
							if( destForm != null )
							{
								Cursor.Current = MDITabsDragCursors.DropCursor;
							}
							else
							{
								Cursor.Current = DragCursors.NodropCursor;
							}
						}
					}
					else
					{
						Cursor.Current = Cursors.Default;
					}

					if( !dropRectFound )
					{
						this.DropRect = Rectangle.Empty;
					}
				}
			}
		}

		/// <summary></summary>
		/// <param name="tabHost"/>
		[DocumentationExclude()]
		protected virtual void PostInitTabHost( TabHost tabHost )
		{
            m_bShouldRaiseBeforeMDIChildEvent = false;

			tabHost.SuspendLayout();
			tabHost.MdiParent = mdiContainer;
			tabHost.ResumeLayout( false );
			tabHost.Visible = true;
			if( tabHost.IsHandleCreated )
			{
				NativeMethods.SetWindowPos( tabHost.Handle, ( IntPtr ) NativeMethods.HWND_BOTTOM, 0, 0, 0, 0, 0x0010 /*SWP_NOACTIVATE*/| 0x0001 /*SWP_NOSIZE*/ );
			}
			// Also init the splitter host
			SplitterHost splitterHost = tabHost.SplitterHost;
			splitterHost.SuspendLayout();
			splitterHost.MdiParent = mdiContainer;
			splitterHost.Visible = true;

			if( splitterHost.IsHandleCreated )
			{
				NativeMethods.SetWindowPos(splitterHost.Handle, (IntPtr)NativeMethods.HWND_BOTTOM, 0, 0, 0, 0, 0x0010 /*SWP_NOACTIVATE*/| 0x0001 /*SWP_NOSIZE*/ );
			}

			splitterHost.Size = tabHost.SplitterBounds.Size;
			splitterHost.ResumeLayout( false );

            m_bShouldRaiseBeforeMDIChildEvent = true;
		}

		/// <summary>
		/// Validates the active child form.
		/// </summary>
		/// <returns></returns>
		[DocumentationExclude()]
		protected internal virtual bool ValidateFocusedChildForm()
		{
			if( this.MdiParent.ActiveMdiChild != null )
			{
				return this.MdiParent.ActiveMdiChild.Validate();
			}
			else
			{
				return true;
			}
		}

		#endregion

		#region Class utility methods
		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void MdiListItemPopup( object sender, EventArgs e )
		{
			this.UpdateMdiList();
		}
		/// <summary></summary>
		/// <param name="mdiChild"/>
		/// <param name="attach"/>
		private void NotifyTabbedMDIBindingToChildren( Form mdiChild, bool attach )
		{
			if( mdiChild != null && mdiChild is ITabbedMDIChildForm )
			{
				if( attach )
				{
					( ( ITabbedMDIChildForm ) mdiChild ).OnAttachTabbedMDI( this );
				}
				else
				{
					( ( ITabbedMDIChildForm ) mdiChild ).OnDetachTabbedMDI( this );
				}
			}
		}
		/// <summary></summary>
		/// <returns></returns>
		internal MDITabPanel CreateMDITabPanelInternal()
		{
			MDITabPanel tabPanel = this.CreateMDITabPanel();
			tabPanel.TabStyle = this.TabStyle;
			this.InitMDITabPanel( tabPanel );
			return tabPanel;
		}
		/// <summary></summary>
		private void CloseActiveChildForm()
		{
			if( ActiveTabHost != null )
			{
				Form f = ( Form ) ActiveTabHost.MDITabPanel.SelectedTab.Tag;

				CloseChildForm( f );
			}
		}

		/// <summary></summary>
		internal void CloseChildForm( Form f )
		{

			f.Close();

			if( !f.IsMdiChild )
			{
            	TabPageAdv page = this.GetTabPageAdvFromForm(f);

				if( page != null )
				{
	                page.TabVisible = false;
				}
			}

            if (!(f is TabHost || f is SplitterHost))
                f.VisibleChanged -= new EventHandler(MDIChild_VisibleChanged);
		}

		/// <summary>
		/// Refreshes all MDI tab panel. 
		/// </summary>
		private void MdiHostRefersh()
		{
			foreach( TabHost tabhost in tabHostList )
			{
				tabhost.MDITabPanel.Refresh();
			}
		}
		/// <summary>
		/// Update close and drop down buttons.
		/// </summary>
		private void UpdateCloseAndDropDownButtons()
		{
			foreach( TabHost tabhost in tabHostList )
			{
				tabhost.MDITabPanel.UpdateCloseAndDropDownButtons();
			}
		}
		/// <summary></summary>
		/// <returns></returns>
		/// <param name="name"/>
		internal bool IsValidTabHostsName( string name )
		{
			foreach( TabHost tabHost in this.TabGroupHosts )
			{
				if( tabHost == null )
				{
					continue;
				}

				if( tabHost.Name == name )
				{
					return false;
				}
			}
			return true;
		}
		/// <summary></summary>
		private void UpdateRenderers()
		{
			if( this.IsOffice2003Style && this.ThemesEnabled )
			{
				this.ThemesEnabled = false;
			}

			ArrayList arrRenderers = null;

			// Update all existing tab controls
			foreach( TabHost tabHost in this.tabHostList )
			{
				tabHost.SuspendLayout();

				tabHost.MDITabPanel.ThemesEnabled = this.ThemesEnabled;
				tabHost.MDITabPanel.TabStyle = this.TabStyle;
				tabHost.MDITabPanel.ShowTabCloseButton = this.ShowCloseButton;
				tabHost.MDITabPanel.ShowCloseButtonForActiveTabOnly = this.ShowCloseButtonForActiveTabOnly;
                tabHost.MDITabPanel.ActiveTabFont = FontUtil.CreateFont( tabHost.MDITabPanel.ActiveTabFont, FontStyle.Regular );
				
                arrRenderers = tabHost.MDITabPanel.Renderer.Renderers;

				if( arrRenderers != null && arrRenderers.Count > 0 )
				{
					TabRendererBase renderer = null;

					for( int i = 0, len = arrRenderers.Count; i < len; i++ )
					{
						renderer = arrRenderers[ i ] as TabRendererBase;

						if( renderer != null )
						{
							renderer.ShowCloseButton = tabHost.MDITabPanel.ShouldDrawCloseButton( i );
						}
					}
				}

				LayoutPanel lpPanel = tabHost.LayoutPanel;

				if( lpPanel != null )
				{
					tabHost.PerformLayoutInternal( lpPanel.GetTabHostSize( tabHost ) );
				}

				tabHost.ResumeLayout();
			}

            if( this.ActiveTabHost != null && this.ActiveTabHost.MDITabPanel != null )
            {
                this.ActiveTabHost.MDITabPanel.ActiveTabFont = FontUtil.CreateFont( this.ActiveTabHost.MDITabPanel.ActiveTabFont, FontStyle.Bold );
            }
		}
		/// <summary></summary>
		/// <param name="groupName"/>
		/// <param name="tabHost"/>
		internal virtual void SetTabHostForGroup( string groupName, TabHost tabHost )
		{
		}
		/// <summary></summary>
		/// <returns></returns>
		private Form GetMdiContainer()
		{
			if( this.mdiContainer != null )
			{
				return this.mdiContainer;
			}
			else
			{
				return this.MdiListMenuItem.GetMainMenu().GetForm();
			}
		}
		/// <summary></summary>
		/// <returns></returns>
		/// <param name="image"/>
		/// <param name="destSize"/>
		private Image CorrectImageSize( Image image, Size destSize )
		{
			if( image == null )
			{
				throw new ArgumentNullException( "image" );
			}

			Bitmap bmPhoto = new Bitmap( destSize.Width, destSize.Height,
				PixelFormat.Format32bppArgb );
			bmPhoto.SetResolution( image.HorizontalResolution,
				image.VerticalResolution );

			Graphics grPhoto = Graphics.FromImage( bmPhoto );
			grPhoto.InterpolationMode =
				InterpolationMode.HighQualityBicubic;

			grPhoto.DrawImage( image,
				new Rectangle( Point.Empty, destSize ),
				new Rectangle( Point.Empty, image.Size ),
				GraphicsUnit.Pixel );

			grPhoto.Dispose();

			return bmPhoto;
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void mdiChildNativeWnd_SetIcon( object sender, EventArgs e )
		{
			MDIChildWindow subclass = sender as MDIChildWindow;

			if( subclass != null )
			{
				Form form = Control.FromHandle( subclass.Handle ) as Form;

				if( form != null )
				{
					SetMdiChildIcon( form, true );
				}
			}
		}

		/// <summary></summary>
		/// <param name="mdiChild"/>
		/// <param name="shouldInvalidate"/>
		private void SetMdiChildIcon( Form mdiChild, bool shouldInvalidate )
		{
			TabPageAdv tabPage = this.GetTabPageAdvFromForm( mdiChild );

			if( tabPage != null )
			{
				int imageIndex = tabPage.ImageIndex;

				if( imageList != null && imageIndex >= 0 && imageIndex < imageList.Images.Count )
				{
					Image im = CorrectImageSize( DrawIconHelper.GetIconToDraw( mdiChild.Icon, mdiChild ), m_imageSize );

					this.imageList.Images[ imageIndex ] = im;
				}

				if( shouldInvalidate )
				{
					if( tabPage.Parent != null )
					{
						tabPage.Parent.Invalidate();
					}
				}
			}
		}
		/// <summary></summary>
		private void ResumeLayoutInternal()
		{
			suspendCount--;
			if( suspendCount < 0 )
			{
				suspendCount = 0;
			}
			// Not checking for suspendCount for now. Otherwise, resulting in painting errors.
			if( this.m_mdiClient != null )
			{
				m_mdiClient.ResumeLayout( false );
			}
		}
		/// <summary></summary>
		/// <returns></returns>
		/// <param name="tabHostIndex"/>
		private TabHost CreateTabHostInternal( int tabHostIndex )
		{
			TabHost tabHost = CreateTabHost();

			InitTabHost( tabHost, tabHostIndex );

			return tabHost;
		}
		/// <summary></summary>
		/// <returns></returns>
		/// <param name="name"/>
		internal bool IsValidSplitterHostName( string name )
		{
			foreach( SplitterHost host in this.splitterHostList )
			{
				if( host.Name == name )
				{
					return false;
				}
			}
			return true;
		}
		/// <summary></summary>
		/// <returns></returns>
		/// <param name="tabHost"/>
		internal TabHost GetTabHostAfter( TabHost tabHost )
		{
			int tabsHostIndex = this.tabHostList.IndexOf( tabHost );
			if( tabsHostIndex == this.tabHostList.Count )
			{
				return null;
			}
			else
			{
				return ( TabHost ) this.tabHostList[ tabsHostIndex + 1 ];
			}
		}
		/// <summary>
		/// Get distances for splitter moving.
		/// </summary>
		/// <param name="splitterHost"> SplitterHost to move. </param>
		/// <param name="deltaPos"> Value splitterHost can move to right or up side. </param>
		/// <param name="deltaNeg"> Value splitterHost can move to left or down side. </param>
		internal void GetMaxDelta( SplitterHost pSplitterHost, ref int deltaPos, ref int deltaNeg )
		{
			deltaPos = 0;
			deltaNeg = 0;

			LayoutPanel lpPanel = pSplitterHost.LayoutPanel;

			if( m_lpPanel != null )
			{
				// Get LayoutPanel of current SplitterHost.
				TabHost thCurrent = lpPanel.TabHost;

				if( lpPanel.Horizontal )
				{
					int iDistanceToTop = 0;
					int iDistanceToDown = 0;

					lpPanel.GetDistanceToTop( ref iDistanceToTop, pSplitterHost.Top );
					lpPanel.GetDistanceToDown( ref iDistanceToDown, pSplitterHost.Bottom );

					deltaPos = iDistanceToDown - 1;
					deltaNeg = iDistanceToTop - 1;
				}
				else
				{
					int iDistanceToRight = 0;
					int iDistanceToLeft = 0;

					lpPanel.GetDistanceToRight( ref iDistanceToRight, pSplitterHost.Right );
					lpPanel.GetDistanceToLeft( ref iDistanceToLeft, pSplitterHost.Left );

					deltaPos = iDistanceToRight - 1;
					deltaNeg = iDistanceToLeft - 1;
				}

			}
		}
		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void MdiLayout( object sender, LayoutEventArgs e )
		{
			if( suspendCount > 0 )
			{
				return;
			}
			// Bounds is empty(usually when window is minimized), lets not bother laying out children.
			if (m_mdiClient.Width == 0 || m_mdiClient.Height == 0)
			{
				return;
			}
			BeginUpdate( false );
			SuspendLayout();

			if( m_lpPanel == null || ( m_lpPanel.ComponentOne == null && m_lpPanel.ComponentTwo == null ) )
			{
				InitializeLayoutPanel();
			}

			if( m_lpPanel != null )
			{
				if( m_lpPanel.Size != m_mdiClient.DisplayRectangle.Size )
				{
					m_lpPanel.Size = m_mdiClient.DisplayRectangle.Size;
					m_lpPanel.DividePanelOnTwoParts();
				}

				LayoutHosts( m_lpPanel );
			}

			ResumeLayoutInternal();
			EndUpdate( mdiContainer, m_mdiClient, false );
		}
		/// <summary>
		/// Creates LayoutPanel with one TabHost.
		/// </summary>
		private void InitializeLayoutPanel()
		{
			if( tabHostList.Count > 0 )
			{
				TabHost tabHost = tabHostList[ 0 ] as TabHost;

				if( tabHost != null )
				{
					m_lpPanel = new LayoutPanel( tabHost, m_mdiClient.DisplayRectangle.Size, m_mdiClient.DisplayRectangle.Location, false );
					m_lpPanel.InitializePanelOne( tabHost );

					tabHost.SplitterBounds = Rectangle.Empty;

					if( tabHost.SplitterHost != null )
					{
						tabHost.SplitterHost.Bounds = Rectangle.Empty;
						tabHost.SplitterHost.Visible = false;
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void LayoutHosts( LayoutPanel panel )
		{
			if( panel != null ) // Complex layout using.
			{
				if( panel.ComponentOne != null )
				{
					if( panel.ComponentOne is LayoutPanel )
					{
						LayoutHosts( panel.ComponentOne as LayoutPanel );
					}
					else // ComponentOne is TabHost.
					{
						TabHost host = panel.ComponentOne as TabHost;
						LayoutTabHost( host, panel.PanelOneLocation, panel.PanelOneSize );
						LayoutTabHostMdiChildren( host, panel.PanelOneLocation, panel.PanelOneSize );
					}
				}

				if( panel.ComponentTwo != null )
				{
					if( panel.ComponentTwo is LayoutPanel )
					{
						LayoutHosts( panel.ComponentTwo as LayoutPanel );
					}
					else // ComponentTwo is TabHost.
					{
						TabHost host = panel.ComponentTwo as TabHost;
						LayoutTabHost( host, panel.PanelTwoLocation, panel.PanelTwoSize );
						LayoutTabHostMdiChildren( host, panel.PanelTwoLocation, panel.PanelTwoSize );
					}
				}

				if( panel.ComponentOne != null && panel.ComponentTwo != null )
				{
					LayoutSplitterHost( panel.TabHost, panel.SplitterBounds, panel.Horizontal );
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tabHost"></param>
		/// <param name="location"></param>
		/// <param name="size"></param>
		private void LayoutTabHost( TabHost tabHost, Point location, Size size )
		{
			if( tabHost != null && tabHost.MDITabPanel != null )
			{
				tabHost.SuspendLayout();

				if( !m_bVisible )
				{
					size = Size.Empty;
				}

				tabHost.PerformLayoutInternal( size );

				switch( tabHost.Alignment )
				{
					case TabAlignment.Top:
						{
							tabHost.Width = size.Width;
							tabHost.Location = location;
                            if( this.Visible && tabHost.MDITabPanel.BorderVisible )
                            {
                                tabHost.MDITabPanel.Dock = DockStyle.Top;
                                tabHost.Height = size.Height;
                            }
                            else
                            {
                                tabHost.MDITabPanel.Dock = DockStyle.None;
                            }

							break;
						}
					case TabAlignment.Left:
						{
							tabHost.Height = size.Height;
							tabHost.Location = location;
                            if( this.Visible && tabHost.MDITabPanel.BorderVisible )
                            {
                                tabHost.MDITabPanel.Dock = DockStyle.Left;
                                tabHost.Width = size.Width;
                            }
                            else
                            {
                                tabHost.MDITabPanel.Dock = DockStyle.None;
                            }

							break;
						}
					case TabAlignment.Right:
						{
							tabHost.Height = size.Height;
                            if( this.Visible && tabHost.MDITabPanel.BorderVisible )
                            {
                                tabHost.Width = size.Width;
                                tabHost.MDITabPanel.Dock = DockStyle.Right;
                            }
                            else
                            {
                                location.X += ( size.Width - tabHost.Width );
                                tabHost.MDITabPanel.Dock = DockStyle.None;
                            }
							
							tabHost.Location = location;

							break;
						}
					case TabAlignment.Bottom:
						{
							tabHost.Width = size.Width;
                            if( this.Visible && tabHost.MDITabPanel.BorderVisible )
                            {
                                tabHost.Height = size.Height;
                                tabHost.MDITabPanel.Dock = DockStyle.Bottom;
                            }
                            else
                            {
                                location.Y += ( size.Height - tabHost.Height );
                                tabHost.MDITabPanel.Dock = DockStyle.None;
                            }
							tabHost.Location = location;

							break;
						}
				}
				
				tabHost.ResumeLayout( true );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tabHost"></param>
		/// <param name="location"></param>
		/// <param name="size"></param>
		private void LayoutTabHostMdiChildren( TabHost tabHost, Point location, Size size )
		{
			if( tabHost != null && tabHost.MDITabPanel != null )
			{
				// Position MDI children.
				foreach( TabPageAdv tabPage in tabHost.MDITabPanel.TabPages )
				{
					Form mdiChild = tabPage.Tag as Form;

					if( mdiChild != null )
					{
						mdiChild.SuspendLayout();

						// TabHost's MDI children width and height.
						int iHeight = 0, iWidth = 0;
						Size szTabHost = m_bVisible ? tabHost.Size : Size.Empty;

                        int borderWidth = 0;
                        int tabHostDist = 0;
                        if( tabHost.Alignment == TabAlignment.Left || tabHost.Alignment == TabAlignment.Right )
                        {
                            if( this.Visible && tabHost.MDITabPanel.BorderVisible )
                            {
                                borderWidth = tabHost.MDITabPanel.BorderWidth;
                                tabHostDist = tabHost.MDITabPanel.Width;
                            }
                            else
                            {
                                tabHostDist = tabHost.Width;
                            }
                        }
                        else
                        {
                            if( this.Visible && tabHost.MDITabPanel.BorderVisible )
                            {
                                borderWidth = tabHost.MDITabPanel.BorderWidth;
                                tabHostDist = tabHost.MDITabPanel.Height + 1;
                            }
                            else
                            {
                                tabHostDist = tabHost.Height;
                            }
                        }

						switch( tabHost.Alignment )
						{
							case TabAlignment.Top:
								{
									iWidth = size.Width;
                                    iHeight = tabHost.Visible ? size.Height - tabHostDist : size.Height;
									mdiChild.Size = new Size( iWidth - borderWidth * 2, iHeight - borderWidth );
									mdiChild.Location = new Point( location.X + borderWidth, tabHost.Visible ? location.Y + tabHostDist : location.Y );
                                    break;
								}

							case TabAlignment.Left:
								{
                                    iWidth = tabHost.Visible ? size.Width - tabHostDist : size.Width;
									iHeight = size.Height;
									mdiChild.Size = new Size( iWidth - borderWidth, iHeight - borderWidth * 2 );
									mdiChild.Location = new Point( tabHost.Visible ? location.X + tabHostDist : location.X, location.Y + borderWidth );
									break;
								}
							case TabAlignment.Right:
								{
                                    iWidth = tabHost.Visible ? size.Width - tabHostDist : size.Width;
									iHeight = size.Height;
									mdiChild.Size = new Size( iWidth - borderWidth, iHeight - borderWidth * 2 );
									mdiChild.Location = new Point( location.X + borderWidth, location.Y + borderWidth );
									break;
								}
							case TabAlignment.Bottom:
								{
									iWidth = size.Width;
                                    iHeight = tabHost.Visible ? size.Height - tabHostDist : size.Height;
									mdiChild.Size = new Size( iWidth - borderWidth * 2, iHeight - borderWidth );
									mdiChild.Location = new Point( location.X + borderWidth, location.Y + borderWidth );
									break;
								}
						}

						mdiChild.ResumeLayout( true );
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pTabHost"></param>
		/// <param name="pBounds"></param>
		private void LayoutSplitterHost( TabHost pTabHost, Rectangle pBounds, bool pHorizontal )
		{
			int iIndex = tabHostList.IndexOf( pTabHost );

			if( iIndex != -1 )
			{
				TabHost tabHost = tabHostList[ iIndex ] as TabHost;

				if( tabHost != null && tabHost.Visible )
				{
					SplitterHost splitterHost = tabHost.SplitterHost;

					if( splitterHost != null )
					{
						splitterHost.SuspendLayout();

						// Make SplitterHost instance visible and set its Size and Location.
						splitterHost.Visible = true;
						splitterHost.Horizontal = pHorizontal;
						splitterHost.Size = pBounds.Size;
						splitterHost.Location = pBounds.Location;
						splitterHost.Splitter.Size = pBounds.Size;

						splitterHost.ResumeLayout( true );
					}
				}
			}
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void UnlockMessageReceived( object sender, EventArgs e )
		{
			if( this.m_mdiClient != null )
			{
				EndUpdateAsync( true );
			}
		}
		#endregion

		#region Event handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MDIContainer_Load( object sender, EventArgs e )
        {
            if( this.ActiveTabHost != null )
            {
                Rectangle bounds  = this.ActiveTabHost.SplitterBounds;
                NativeMethods.SetWindowPos( this.ActiveTabHost.SplitterHost.Handle, ( IntPtr ) NativeMethods.HWND_BOTTOM, bounds.X, bounds.Y, bounds.Width, bounds.Height, 0x0010 /*SWP_NOACTIVATE*/ );
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MDIContainer_RightToLeftChanged( object sender, EventArgs e )
		{
			// Set IsMirrored property for all MDITabPanels according to RightToLeft value.
			if( mdiContainer != null )
			{
				foreach( TabHost tabHost in tabHostList )
				{
					if( tabHost != null )
					{
						if( tabHost.MDITabPanel != null )
						{
							//tabHost.MDITabPanel.IsMirrored = ( mdiContainer.RightToLeft == RightToLeft.Yes ) ? true : false;
						}
					}
				}
			}

			// Reset SplitterHost bounds.
			if( m_lpPanel != null )
			{
				TabHost thTabHost = m_lpPanel.GetTabHost( true );

				if( thTabHost != null )
				{
					if( thTabHost.SplitterHost != null )
					{
						thTabHost.SplitterHost.Visible = false;
					}
				}
			}
		}
		/// <summary></summary>
		/// <param name="ctrl"/>
		protected virtual bool OnBeforeMDIChildAdded( Control ctrl )
		{
			MDIChildAddCancelEventArgs e = new MDIChildAddCancelEventArgs( ctrl );

			if( BeforeMDIChildAdded != null )
			{
				BeforeMDIChildAdded( this, e );
			}

			return e.Cancel;
		}

        private void MDIChild_VisibleChanged(object sender, EventArgs e)
        {
            Form f = sender as Form;

            TabHost host = this.GetTabHostFromForm(f);
            if (host != null && host.MDITabPanel != null && host.MDITabPanel.TabPages.Count > 0)
            {
                int count = this.GetVisiblePagesCount(host);
                if (count == 0 && !f.Visible)
                {
                    host.Hide();
                }
                else if(count == 1 && f.Visible)
                {
                    host.Show();
                }
            }

            if (!f.Enabled)
            {
                this.UpdateActiveTabHost();

                if (this.mdiContainer.ActiveMdiChild != null)
                {
                    this.mdiContainer.ActiveMdiChild.BringToFront();
                }
            }
        }

        private void MdiChild_EnabledChanged(object sender, EventArgs e)
        {
            Form child = sender as Form;
            TabPageAdv page = this.GetTabPageAdvFromForm(child);
            if (page != null)
            {
                if (child.Enabled)
                {
                    page.TabEnabled = true;
                }
                else
                {
                    page.TabEnabled = false;
                }

                this.UpdateActiveTabHost();

                if (this.mdiContainer.ActiveMdiChild != null)
                {
                    this.mdiContainer.ActiveMdiChild.BringToFront();
                }
            }
        }

        private int GetVisiblePagesCount(TabHost host)
        {
            int i = 0;

            if(host != null && host.MDITabPanel != null)
            {
                foreach( TabPageAdv page in host.MDITabPanel.TabPages )
                {
                    if (page.TabVisible)
                    {
                        i++;
                    }
                }
            }

            return i;
        }

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void MDIChild_Added( object sender, ControlEventArgs e )
		{
			Form mdiChild = e.Control as Form;

			AddMDIChildToTabbedMDI( mdiChild );
		}

		protected void AddMDIChildToTabbedMDI( Form mdiChild )
		{
            bool eventFeedback = false;
            if (this.popupManager != null)
            {
                this.popupManager = new PopupMenusManager();
            }
            if( this.m_bShouldRaiseBeforeMDIChildEvent )
			{
                eventFeedback = this.OnBeforeMDIChildAdded( mdiChild );
            }

			if( eventFeedback == false )
			{
				if( mdiChild != null && !( mdiChild is TabHost || mdiChild is SplitterHost ) )
				{					
					BeginUpdate( false );
					BeginUpdateMDIChild( mdiChild );

					NotifyTabbedMDIBindingToChildren( mdiChild, true );

					if( ActiveTabHost != null )
					{
						AddMdiChild( mdiChild, ActiveTabHost );
					}
					else
					{
						TabHost thTabHost = CreateTabHostInternal( tabHostList.Count );
						AddMdiChild( mdiChild, thTabHost );
						PostInitTabHost( thTabHost );
					}

					OnActiveTabToNewGroupMoved( mdiChild, null, ActiveTabHost );
					OnMdiChildLoad( mdiChild );

					mdiChild.HandleDestroyed += new EventHandler( OnMDIChildHandleDestroyed );
                    mdiChild.VisibleChanged += new EventHandler(MDIChild_VisibleChanged);
                    mdiChild.EnabledChanged += new EventHandler(MdiChild_EnabledChanged);

					// This will set the default bounds for the newly added mdi child.
					MdiLayout( null, new LayoutEventArgs( mdiChild, "Bounds" ) );

					EndUpdateMDIChildAsync( mdiChild );
					EndUpdateAsync( false );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnMDIChildHandleDestroyed( object sender, EventArgs e )
		{
			RemoveMDIChildFromHash( sender as Form, true );
		}
		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void MDIChild_Removed( object sender, ControlEventArgs e )
		{
			Form mdiChild = e.Control as Form;

			if( mdiChild != null && !( mdiChild is TabHost || mdiChild is SplitterHost ) )
			{
				BeginUpdate( false );
				BeginUpdateMDIChild( mdiChild );

				OnMdiChildRemoved( mdiChild );

				EndUpdateMDIChildAsync( mdiChild );
				EndUpdateAsync( false );
			}

			if( mdiContainer != null && mdiContainer.MdiChildren.Length == 0 )
			{
				ModifyRedundantReferences();
                this.popupManager.Dispose();
			}

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
			if( m_mdiClient.MdiChildren.Length == 0 )
			{
				mdiContainer.ResumeLayout( true );
			}
#endif
		}
		/// <summary>
		/// Method modifies remaining redundant collections and properties.
		/// </summary>
		private void ModifyRedundantReferences()
		{
			tabHostList.Clear();
			splitterHostList.Clear();

			if( this.ActiveTabHost != null )
				this.ActiveTabHost = null;

			m_lpPanel = null;
		}
		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void MdiChildDisposed( object sender, EventArgs e )
		{
			Form mdiChild = sender as Form;
			this.mdiChildrenTooltips.Remove( mdiChild );
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void mdiContainer_HandleCreated( object sender, EventArgs e )
		{
			// Perform Mdi Layout here because of when TabbedMdi is attached in hosted form constructor,
			// MdiClient is not created yet, and because of this Layout maybe performed incorrect.
			if( m_mdiClient != null )
			{
				m_mdiClient.ResumeLayout( true );
			}
		}
        
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		/// <summary></summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mdiContainer_FormClosed( object sender, FormClosedEventArgs e )
		{
            closed = true;
			DetachFromMdiContainer( mdiContainer, false, false );
		}
#else
    /// <summary></summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void mdiContainer_FormClosed( object sender, EventArgs e )
    {
      DetachFromMdiContainer( mdiContainer, false, false );
    }
#endif

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void MdiListItemClicked( object sender, EventArgs e )
		{
			if( this.mdiChildrenByMenuItems[ sender ] != null )
			{
				Form form = this.mdiChildrenByMenuItems[ sender ] as Form;
				if( form.IsMdiContainer )
				{
					MdiWindowDialog dlg = new MdiWindowDialog();
					dlg.MaximizeBox = false;
					dlg.SetItems( form.ActiveMdiChild, this.GetMdiChildrenForMenu() );
					if( dlg.ShowDialog() == DialogResult.OK )
					{
						dlg.ActiveChildForm.Activate();
					}
				}
				else
				{
					form.Activate();
				}
			}
		}
		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void MDIChild_Activate( object sender, EventArgs e )
		{
			if( null != this.mdiContainer )
			{
				Form mdiChild = this.mdiContainer.ActiveMdiChild;

				if( null != mdiChild )
				{
					BeginUpdate( false );
					BeginUpdateMDIChild( mdiChild );

					UpdateActiveTabHost();
					UpdateScrollOffset();

					EndUpdateMDIChild( mdiChild );
					EndUpdate( mdiContainer, m_mdiClient, false );
				}
			}
		}
		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void OnMdiChildLoad( Form form )
		{
			AddMDIChildToHash( form );

			if( form != null )
			{
				// Perform Mdi Layout here because of when TabbedMdi is attached in hosted form constructor,
				// MdiClient is not created yet, and because of this Layout maybe performed incorrect.
				if( this.MdiClient != null )
				{
					this.MdiClient.ResumeLayout( true );
					this.MdiClient.PerformLayout();
				}
			}
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void OnThemeChanged( object sender, EventArgs e )
		{
			UpdateRenderers();
		}
		#endregion

		#region MdiListToolStripItem

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

		#region Fields
		private const int DEF_ITEMS_COUNT = 9;
		/// <summary>
		/// ToolStrip item to which MDIChildren menu items should be added or removed.
		/// </summary>
		private ToolStripMenuItem m_mdiListToolStripItem = null;
		/// <summary>
		/// List which contain menu items with MDIChild window reference.
		/// </summary>
		private ArrayList m_lstMDIMenuItems = new ArrayList();
		/// <summary>
		/// Container which is used to manipulate child windows.
		/// </summary>
		private Form m_frmWindowsListContainer = null;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the ToolStrip menu item to which 
		/// MDI Children list should be displayed.
		/// </summary>
		[Description( "Specifies the ToolStrip menu item to which the MDI Children list should be added." )]
		[DefaultValue( null )]
		public ToolStripMenuItem MdiListToolStripItem
		{
			get
			{
				return m_mdiListToolStripItem;
			}
			set
			{
				if( m_mdiListToolStripItem != value )
				{
					if( m_mdiListToolStripItem != null )
					{
						RemoveMDIChildrenFromToolStripMenu( m_mdiListToolStripItem );
						m_mdiListToolStripItem.DropDownOpening -= new EventHandler( MDIList_OnDropDownOpening );
						m_frmWindowsListContainer = null;
					}

					m_mdiListToolStripItem = value;

					if( m_mdiListToolStripItem != null )
					{
						ToolStrip ts = m_mdiListToolStripItem.GetCurrentParent();

						if( ts != null )
						{
							Form form = ts.Parent as Form;

							if( form != null && form.IsMdiContainer )
							{
								m_frmWindowsListContainer = form;
							}
						}

						m_mdiListToolStripItem.DropDownOpening += new EventHandler( MDIList_OnDropDownOpening );
					}
				}
			}
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// Activates MDIChild window selected by user.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItem_Click( object sender, EventArgs e )
		{
			ToolStripMenuItem tsMenuItem = sender as ToolStripMenuItem;

			if( tsMenuItem != null )
			{
				Form frmMDIChild = tsMenuItem.Tag as Form;

				if( frmMDIChild != null )
				{
					if( frmMDIChild.IsMdiContainer )
					{
						MdiWindowDialog dlgMDIWindow = new MdiWindowDialog();
						dlgMDIWindow.MaximizeBox = false;
						dlgMDIWindow.SetItems( frmMDIChild.ActiveMdiChild, GetMDIChildren() );

						if( dlgMDIWindow.ShowDialog() == DialogResult.OK )
						{
							dlgMDIWindow.ActiveChildForm.Activate();
						}
					}
					else
					{
						frmMDIChild.Activate();
					}
				}
			}
		}
		/// <summary>
		/// Updates dropdown items by MDIChildren list.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MDIList_OnDropDownOpening( object sender, EventArgs e )
		{
			RemoveMDIChildrenFromToolStripMenu( m_mdiListToolStripItem );
			AddMDIChildrenToToolStripMenu( m_mdiListToolStripItem );
		}
		#endregion

		#region Implementation
		/// <summary></summary>
		/// <param name="mdiListItem"></param>
		[DocumentationExclude(), Obsolete( "This method will be removed soon, use MdiListToolStripItem property." )]
		protected virtual void AttachMdiListToolStripItem( ToolStripMenuItem mdiListItem )
		{
			AddMDIChildrenToToolStripMenu( mdiListItem );
		}
		/// <summary>
		/// Removes menu items which belong to MDIChildren references from ToolStrip menu item  
		/// </summary>
		/// <param name="pMenuItem"> ToolStrip menu item from which menu items should be removed. </param>
		private void RemoveMDIChildrenFromToolStripMenu( ToolStripMenuItem pMenuItem )
		{
			if( pMenuItem != null )
			{
				foreach( ToolStripItem item in m_lstMDIMenuItems )
				{
					pMenuItem.DropDownItems.Remove( item );
				}
			}

			// Clear MDI children.
			m_lstMDIMenuItems.Clear();
		}
		/// <summary>
		/// Adds menu items with MDIChildren references to ToolStrip menu item. 
		/// </summary>
		/// <param name="pMenuItem"> ToolStrip menu item to which menu items should be added. </param></param>
		private void AddMDIChildrenToToolStripMenu( ToolStripMenuItem pMenuItem )
		{
			if( pMenuItem != null &&
				m_frmWindowsListContainer != null &&
				m_frmWindowsListContainer.IsMdiContainer )
			{
				Form[] frmMDIChildren = GetMDIChildren();
				int iMdiChildrenLength = frmMDIChildren.Length;

				// Add separator if needed.
				if( m_mdiListToolStripItem.DropDownItems.Count > 0 && iMdiChildrenLength > 0 )
				{
					ToolStripSeparator tsSeparator = new ToolStripSeparator();
					m_mdiListToolStripItem.DropDownItems.Add( tsSeparator );
					m_lstMDIMenuItems.Add( tsSeparator );
				}

				// Add MDI childs to list.
				int iChildrenLength = Math.Min( DEF_ITEMS_COUNT, iMdiChildrenLength );
				int iCaptionNumber = 1;

				for( int i = 0; i < iChildrenLength; i++ )
				{
					Form frmMDIChild = frmMDIChildren[ i ];
					if( !( frmMDIChild is TabHost ) && !( frmMDIChild is SplitterHost ) )
					{
						string sCaption = String.Format( "&{0} {1}", iCaptionNumber++, frmMDIChild.Text );

						ToolStripMenuItem item = new ToolStripMenuItem( sCaption, null, new EventHandler( MenuItem_Click ) );
						item.Checked = ( m_frmWindowsListContainer.ActiveMdiChild == frmMDIChild );
						item.Tag = frmMDIChild; // Save info about Form in Tag property.

						m_mdiListToolStripItem.DropDownItems.Add( item );
						m_lstMDIMenuItems.Add( item );
					}
				}

				// Add "Windows..." menu item if necessary.
				if( frmMDIChildren.Length > DEF_ITEMS_COUNT )
				{
					ToolStripMenuItem tsWindowsItem = new ToolStripMenuItem( "&Windows...", null, new EventHandler( MenuItem_Click ) );
					tsWindowsItem.Tag = m_frmWindowsListContainer;

					m_mdiListToolStripItem.DropDownItems.Add( tsWindowsItem );
					m_lstMDIMenuItems.Add( tsWindowsItem );
				}
			}
		}
		/// <summary>
		/// Gets MDIChildren from MDI container.
		/// </summary>
		private Form[] GetMDIChildren()
		{
			Form[] forms = new Form[] { };

			if( m_frmWindowsListContainer == null )
			{
				return forms;
			}

			ArrayList actualMdiChildren = new ArrayList();

			foreach( Form form in m_frmWindowsListContainer.MdiChildren )
			{
				if( !( form is SplitterHost ) && !( form is TabHost ) )
				{
					actualMdiChildren.Add( form );
				}
			}

			forms = ( Form[] ) actualMdiChildren.ToArray( typeof( Form ) );

			return forms;
		}
		#endregion

#endif
		#endregion

		#region Nested classes
		/// <summary></summary>
		public class MdiParentNativeWindow : NativeWindow
		{
			#region Data members
			/// <summary></summary>
			private TabbedMDIManager m_tabbedMDIMan = null;
			#endregion

			#region Contstruction
			/// <summary>
			/// Used to avoid misuse of this class (with null reference to <see cref="TabbedMDIManager"/>).
			/// </summary>
			private MdiParentNativeWindow()
			{
			}

			/// <summary>
			/// Constructs <see cref="MdiParentNativeWindow"/> class instance.
			/// </summary>
			/// <param name="tabbedMDI">Reference to valid <see cref="TabbedMDIManager"/> instance.</param>
			/// <remarks><see cref="tabbedMDI"/> has to be non-null.</remarks>
			public MdiParentNativeWindow( TabbedMDIManager tabbedMDI )
			{
				Debug.Assert( null != tabbedMDI, "TabbedMDIManager can't be null." );
				m_tabbedMDIMan = tabbedMDI;
			}
			#endregion

			#region Overrides
			/// <summary></summary>
			/// <param name="m"/>
			protected override void WndProc( ref Message m )
			{
				if( m.Msg == NativeMethods.WM_NEXTMENU )
				{
					return;
				}

				base.WndProc( ref m );
			}
			#endregion
		}

		#region Delegates
		/// <summary>
		/// 
		/// </summary>
		protected delegate void EndUpdateMDIClientDelegate( Form mdiContainer, MdiClient mdiClient, bool bLockContainer );
		/// <summary>
		/// 
		/// </summary>
		protected delegate void EndUpdateMDIChildDelegate( Form pMDIChild );
		#endregion

		#endregion

		#region Reset & ShouldSerialize
		/// <summary>
		/// Indicates whether the current value of AttachedTo is to be serialized.
		/// </summary>
		/// <returns></returns>
		private bool ShouldSerializeAttachedTo()
		{
			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pForm"></param>
		private void ResetAttachedTo()
		{
			Component component = this;

			if( component != null && component.Site != null )
			{
				IDesignerHost host = component.Site.GetService( typeof( IDesignerHost ) ) as IDesignerHost;

				if( host != null )
				{
					this.AttachedTo = host.RootComponent as Form;
				}
			}
			else
			{
				this.AttachedTo = null;
			}
		}
		#endregion

		#region *** TabbedMDIManagerDesigner
		/// <summary>
		/// 
		/// </summary>
		public class TabbedMDIManagerDesigner : ComponentDesigner
		{
			/// <summary>
			/// 
			/// </summary>
			/// <param name="component"></param>
			public override void Initialize( IComponent component )
			{
				base.Initialize( component );

				TabbedMDIManager mdiManager = component as TabbedMDIManager;

				if( mdiManager != null )
				{
					if( component != null && component.Site != null )
					{
						IDesignerHost host = component.Site.GetService( typeof( IDesignerHost ) ) as IDesignerHost;

						if( host != null )
						{
							mdiManager.AttachedTo = host.RootComponent as Form;
						}
					}
				}

			}
		}
		#endregion
	}

	/// <summary>
	/// This class is the parent form of a tabbed MDI tab control(tab group) managed by the 
	/// <see cref="TabbedMDIManager"/>.
	/// </summary>
	public class TabHost : Form, ITabHost
    {
        #region Constans
        private const int c_cornerCut = 3;
        #endregion

        #region Fields
        /// <summary></summary>
		private TabbedMDIManager m_MDIManager;
		/// <summary></summary>
		private SplitterHost m_SplitterHost;
		/// <summary></summary>
		private MDITabPanel mdiTabPanel;
		/// <summary></summary>
		private int m_borderHeight = 2;
		/// <summary> Uses for backward compatibility. </summary>
		private int m_iScaleBaseSize = 0;
		/// <summary>
		/// SplitterHost bounds.
		/// </summary>
		private Rectangle m_rcSplitterBounds;
		/// <summary>
		/// Indicates LayoutPanel in which TabHost instance is located.
		/// </summary>
		private LayoutPanel m_layoutPanel;
		/// <summary>
		/// Indicates a collection of next TabHost controls to which current MDIChild form can be moved.
		/// </summary>
		private ArrayList m_arrNextTabHost = new ArrayList();
		/// <summary>
		/// Indicates a collection of previous TabHost controls to which current MDIChild form can be moved.
		/// </summary>
		private ArrayList m_arrPreviousTabHost = new ArrayList();
		#endregion

		#region Properties
		/// <summary></summary>
		public TabAlignment Alignment
		{
			get
			{
				return MDITabPanel.Alignment;
			}
			set
			{
				if( value != MDITabPanel.Alignment )
				{
					MDITabPanel.Alignment = value;
					PerformLayoutInternal( this.LayoutPanel.GetTabHostSize( this ) );
				}
			}
		}

		/// <summary>
		/// Returns the <see cref="MDITabPanel"/> tab control within this form.
		/// </summary>
		public MDITabPanel MDITabPanel
		{
			get
			{
				return mdiTabPanel;
			}
		}

		/// <summary>
		/// Represents <see cref="TabHost"/>'s border height.
		/// </summary>
		[DefaultValue( 2 )]
		public int BottomBorderHeight
		{
			get
			{
				return m_borderHeight;
			}
			set
			{
				if( value < 0 )
				{
					throw new ArgumentException( "Border height shouldn't be negative" );
				}

				if( m_borderHeight != value )
				{
					m_borderHeight = value;
				}
			}
		}

		/// <summary>
		/// Represents <see cref="TabHost"/>'s border color.
		/// </summary>
		[DefaultValue( KnownColor.Control )]
		public Color BottomBorderColor
		{
			get
			{
				return BackColor;
			}
			set
			{
				if( BackColor != value )
				{
					BackColor = value;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal SplitterHost SplitterHost
		{
			get
			{
				return m_SplitterHost;
			}
			set
			{
				m_SplitterHost = value;
			}
		}
		/// <summary>
		/// Gets or sets SplitterHost bounds.
		/// </summary>
		internal Rectangle SplitterBounds
		{
			get
			{
				return m_rcSplitterBounds;
			}
			set
			{
				m_rcSplitterBounds = value;
			}
		}
		/// <summary>
		/// Gets or sets LayoutPanel in which TabHost instance is located.
		/// </summary>
		internal LayoutPanel LayoutPanel
		{
			get
			{
				return m_layoutPanel;
			}
			set
			{
				m_layoutPanel = value;
			}
		}
		/// <summary>
		/// Gets or sets a collection of next TabHost controls to which current MDIChild form can be moved.
		/// </summary>
		internal ArrayList NextTabHost
		{
			get
			{
				return m_arrNextTabHost;
			}
			set
			{
				m_arrNextTabHost = value;
			}
		}
		/// <summary>
		/// Gets or sets a collection of previous TabHost controls to which current MDIChild form can be moved.
		/// </summary>
		internal ArrayList PreviousTabHost
		{
			get
			{
				return m_arrPreviousTabHost;
			}
			set
			{
				m_arrPreviousTabHost = value;
			}
		}
		/// <summary>
		/// Returns the weight associated with this tab host 
		/// when allocating the available space between tab groups.
		/// </summary>
		public int MdiChildAutoScaleBaseDim
		{
			get
			{
				return m_iScaleBaseSize;
			}
		}

        /// <summary>
        /// Indicates whether host OS is Vista.
        /// </summary>
        internal bool IsVistaOS
        {
            get
            {
                return Environment.OSVersion.Version.Major >= 6;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsVS2008
        {
            get
            {
                return this.MDITabPanel != null && this.MDITabPanel.TabStyle == typeof( TabRendererVS2008 );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsDockingWhidbeyStyle
        {
            get
            {
                return this.MDITabPanel != null && this.MDITabPanel.TabStyle == typeof(TabRendererDockingWhidbey);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsOffice2007Style
		{
			get
			{
                return this.MDITabPanel != null && this.MDITabPanel.TabStyle == typeof(TabRendererOffice2007);
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Creates a new instance of the <see cref="TabHost"/> class.
		/// </summary>
		/// <param name="manager">The corresponding <see cref="TabbedMDIManager"/> instance.</param>
		/// <remarks><para>Meant to be used by the framework.</para></remarks>
		public TabHost( TabbedMDIManager manager )
			: base()
		{
			m_MDIManager = manager;

			mdiTabPanel = CreateMDITabPanel();

			FormBorderStyle = FormBorderStyle.None;
			ControlBox = false;
			HelpButton = false;
			MaximizeBox = false;
			MinimizeBox = false;
			ShowInTaskbar = false;
			SizeGripStyle = SizeGripStyle.Hide;

			Controls.Add( mdiTabPanel );
			Size = new Size( 10, mdiTabPanel.Size.Height + 2 );
			mdiTabPanel.Location = new Point( 0, 0 );

			SetStyle( ControlStyles.Selectable, false );
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
			// Generate a unique ID for the tab hosts.
			string tabHostName = "TabHost_1";
			while( !m_MDIManager.IsValidTabHostsName( tabHostName ) )
			{
				tabHostName = IDGenerator.GetNextID( tabHostName );
			}

			Name = tabHostName;
			mdiTabPanel.Name = Name + "_Tab";
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.Dispose"/>.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( m_SplitterHost != null )
				{
					m_SplitterHost.SplitterMoved -= new SplitterHostEventHandler( Splitter_Moved );
					m_SplitterHost = null;
				}

				if( mdiTabPanel != null )
				{
					SuspendChildActivation();
					mdiTabPanel.DetachMdiChildren();
					ResumeChildActivation();
					mdiTabPanel = null;
				}
			}

			base.Dispose( disposing );
		}

		#endregion

		#region Class overrides
		/// <summary></summary>
		[DocumentationExclude()]
		protected internal virtual void SuspendChildActivation()
		{
			if( this.MDITabPanel != null )
				this.MDITabPanel.SuspendChildActivation();
		}

		/// <summary></summary>
		[DocumentationExclude()]
		protected internal virtual void ResumeChildActivation()
		{
			if( this.MDITabPanel != null )
				this.MDITabPanel.ResumeChildActivation();
		}

		/// <summary>
		/// Creates a <see cref="MDITabPanel"/> for use within this Form.
		/// </summary>
		/// <returns>A <see cref="MDITabPanel"/> instance.</returns>
		/// <remarks><para>
		/// This method inturn uses <see cref="TabbedMDIManager.CreateMDITabPanel"/>
		/// to create the tab control.
		/// </para></remarks>
		protected virtual MDITabPanel CreateMDITabPanel()
		{
			return m_MDIManager.CreateMDITabPanelInternal();
		}

		/// <summary>
		/// Called to "host" an mdi child form within this tab group.
		/// </summary>
		/// <param name="mdiChild">The mdi child form to host.</param>
		/// <param name="prevTabData"/></param>
		protected internal virtual void AddMdiChild( Form mdiChild, ITabData prevTabData )
		{
			MDIChildTabData prevMDIChildTabData = prevTabData as MDIChildTabData;

			SuspendChildActivation();
			MDITabPanel.AddMdiChild( mdiChild, prevMDIChildTabData );
			m_MDIManager.keyboardActivationHelper.AddMdiChild( mdiChild );

			if( mdiChild is ITabbedMDIChildForm )
			{
				ITabbedMDIChildForm tmcf = mdiChild as ITabbedMDIChildForm;
				tmcf.OnMdiChildAddedToTabHost( this, MDITabPanel.TabPages.Count - 1 );
			}

			ResumeChildActivation();
			
			m_MDIManager.UpdateActiveTabHost();
			//m_MDIManager.UpdateActiveTabHost(mdiChild.Enabled ? mdiChild : m_MDIManager.MdiParent.ActiveMdiChild);
		}

		/// <summary>
		/// Specifies the tooltip for a contained form.
		/// </summary>
		/// <param name="mdiChild">The form.</param>
		/// <param name="tooltip">The tooltip.</param>
		/// <remarks><para>This tooltip will be set on the form's corresponding tab in its tab group.</para></remarks>
		protected internal virtual void SetTooltip( Form mdiChild, string tooltip )
		{
			TabPageAdv tabPage = MDITabPanel.GetTabPageAdvFromForm( mdiChild );

			if( tabPage != null )
			{
				tabPage.ToolTipText = tooltip;
			}
		}

		/// <summary>
		/// Removes the mdi child form from this tab group.
		/// </summary>
		/// <param name="mdiChild">The child form to remove.</param>
		/// <param name="removeFromHashtable">Internal flag.</param>
		/// <returns>True if the form was found and removed; false otherwise.</returns>
		protected internal virtual bool RemoveMdiChild( Form mdiChild, bool removeFromHashtable )
		{
			SuspendChildActivation();

			if( MDITabPanel.RemoveMdiChild( mdiChild, removeFromHashtable ) )
			{
				ResumeChildActivation();
				m_MDIManager.keyboardActivationHelper.RemoveMdiChild( mdiChild );

				if( MDITabPanel.TabPages.Count == 0 )
				{
					m_MDIManager.RemoveTabHost( this );
				}

				m_MDIManager.UpdateActiveTabHost();
				return true;
			}

			ResumeChildActivation();

			return false;
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.WndProc"/>.
		/// </summary>
		/// <param name="m"></param>
		protected override /*NativeWindow*/ void WndProc( ref Message m )
		{
			// Key for preventing focus being set on the Host forms.
			// Turning off ControlStyles.Selectable doesn't seem to do it.
			if( m.Msg == 0x0007 ) //WM_SETFOCUS 
			{
				return;
			}

			// I shouldn't get this message since I am not Selectable, but Windows ignores
			// that setting and sends this anyway.
			if( m.Msg == 0x0022 ) //WM_CHILDACTIVATE
			{
				// So that when the user mouse downs on this, it still stays on the bottom of the z-order.
				NativeMethods.SetWindowPos( this.Handle, ( IntPtr ) NativeMethods.HWND_BOTTOM, 0, 0, 0, 0,
					NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE );
				return;
			}

			base.WndProc( ref m );
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		[DocumentationExclude()]
		protected void Splitter_Moved( object sender, SplitterHostEventArgs e )
		{
			SplitterHost shCurrent = sender as SplitterHost;

			if( shCurrent != null )
			{
				LayoutPanel lpPanel = shCurrent.LayoutPanel;

				if( lpPanel != null )
				{
					lpPanel.SplitterMoved( e.Delta );
					lpPanel.DividePanelOnTwoParts();
				}
                this.PerformLayout();
                this.Parent.PerformLayout();
                this.Refresh();
			}
		}

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (this.SplitterHost != null && m_MDIManager != null && m_MDIManager.ShouldRaiseBeforeMDIChildEvent)
            {
                if (this.Visible)
                {
                    Rectangle rc  = this.SplitterBounds;
                    NativeMethods.SetWindowPos( this.SplitterHost.Handle, IntPtr.Zero, rc.X, rc.Y, rc.Width, rc.Height, 0x0054/*SWP_NOACTIVATE|SWP_NOZORDER|SWP_SHOWWINDOW*/ );
                }
                else
                {
                    this.SplitterHost.Hide();
                }
            }
        }

        protected override void OnPaintBackground( PaintEventArgs e )
        {
            base.OnPaintBackground( e );
        
            if( this.Visible && this.MDITabPanel != null && this.MDITabPanel.BorderVisible )
            {
                Graphics g = e.Graphics;
                Rectangle bounds = new Rectangle( 0, 0, this.Bounds.Width, this.Bounds.Height );
                int width = this.MDITabPanel.BorderWidth;

                Color borderColor = SystemColors.ControlDark;

                if( this.MDITabPanel.Renderer != null && this.MDITabPanel.Renderer.Renderers != null
                    && this.MDITabPanel.SelectedIndex >= 0 && this.MDITabPanel.SelectedIndex < this.MDITabPanel.Renderer.Renderers.Count )
                {
                    TabRendererBase tabPageRenderer = this.MDITabPanel.Renderer.Renderers[ this.MDITabPanel.SelectedIndex ] as TabRendererBase;

                    if( this.MDITabPanel.BorderColor != this.MDITabPanel.BackColor )
                    {
                        borderColor = this.MDITabPanel.BorderColor;
                    }
                    else if( tabPageRenderer != null )
                    {
                        borderColor = tabPageRenderer.TabBorderColor;
                    }
                }

                Rectangle topRect = new Rectangle( new Point( bounds.X, bounds.Y ), new Size( bounds.Right, width ) );
                Rectangle bottomRect = new Rectangle( bounds.Left, bounds.Bottom - width, bounds.Right, width );
                Rectangle leftRect = new Rectangle( new Point( bounds.X, bounds.Y ), new Size( width, bounds.Bottom - 1 ) );
                Rectangle rightRect = new Rectangle( bounds.Right - width, bounds.Top, width, bounds.Bottom - 1 );                

                using( SolidBrush brush = new SolidBrush( this.MDITabPanel.ActiveTabColor ) )
                {
                    g.FillRectangle( brush, leftRect );
                    g.FillRectangle( brush, bottomRect );
                    g.FillRectangle( brush, rightRect );
                    g.FillRectangle( brush, topRect );
                }

                Color lightBorderColor = WindowsXPThemeColors.TabControlAdvLightBorderColor;
                if( this.IsVistaOS )
                {
                    lightBorderColor = Color.White;
                }

                Point[] bottomLeftPoints = new Point[]
                                {
                                    new Point( bounds.Left, bounds.Bottom - c_cornerCut ),
                                    new Point( bounds.Left, bounds.Bottom ),
                                    new Point( bounds.Left + c_cornerCut, bounds.Bottom )
                                };

                Point[] bottomRightPoints = new Point[]
                                {
                                    new Point( bounds.Right, bounds.Bottom - c_cornerCut ),
                                    new Point( bounds.Right, bounds.Bottom ),
                                    new Point( bounds.Right - c_cornerCut, bounds.Bottom )
                                };

                Point[] topRightPoints = new Point[]
                                {
                                    new Point( bounds.Right, bounds.Top + c_cornerCut - 1 ),
                                    new Point( bounds.Right, bounds.Top ),
                                    new Point( bounds.Right - c_cornerCut + 1, bounds.Top )
                                };

               Point[] topLeftPoints = new Point[]
                                {
                                    new Point( bounds.Left, bounds.Top + c_cornerCut - 1 ),
                                    new Point( bounds.Left, bounds.Top ),
                                    new Point( bounds.Left + c_cornerCut - 1, bounds.Top )
                                };
                
                GraphicsPath path = new GraphicsPath();

                switch( this.Alignment )
                {
                    case TabAlignment.Top:
                        this.DrawTopAlignmentBorders( g, bounds, borderColor, lightBorderColor );
                        using( SolidBrush brush = new SolidBrush( Color.White ) )
                        {
                            path.AddLines( bottomLeftPoints );
                            g.FillPath( brush, path );
                            path.Reset();
                            path.AddLines( bottomRightPoints );
                            g.FillPath( brush, path );
                        }                            
                        break;
                    case TabAlignment.Left:
                        this.DrawLeftAlignmentBorders( g, bounds, borderColor, lightBorderColor );
                        using( SolidBrush brush = new SolidBrush( Color.White ) )
                        {
                            path.AddLines( bottomRightPoints );
                            g.FillPath( brush, path );
                            path.Reset();
                            path.AddLines( topRightPoints );
                            g.FillPath( brush, path );
                        } 
                        break;
                    case TabAlignment.Bottom:
                        bounds.Y++;
                        this.DrawBottomAlignmentBorders( g, bounds, borderColor, lightBorderColor );
                        using( SolidBrush brush = new SolidBrush( Color.White ) )
                        {
                            path.AddLines( topLeftPoints );
                            g.FillPath( brush, path );
                            path.Reset();
                            path.AddLines(topRightPoints);
                            g.FillPath( brush, path );
                        }
                        break;
                    case TabAlignment.Right:
                        bounds.X++;
                        this.DrawRightAlignmentBorders( g, bounds, borderColor, lightBorderColor );
                        using( SolidBrush brush = new SolidBrush( Color.White ) )
                        {
                            path.AddLines( bottomLeftPoints );
                            g.FillPath( brush, path );
                            path.Reset();
                            path.AddLines( topLeftPoints );
                            g.FillPath( brush, path );
                        }
                        break;

                }

                path.Dispose();

                using( Pen pen = new Pen( borderColor ) )
                {
                    Point point1 = Point.Empty;
                    Point point2 = Point.Empty;
 
                    if( this.Alignment == TabAlignment.Top )
                    {
                        point1 = new Point( bounds.Left, bounds.Top + this.MDITabPanel.Height );
                        point2 = new Point( bounds.Right, bounds.Top + this.MDITabPanel.Height );
                        if( this.IsVS2008 )
                        {
                            point1.X += this.MDITabPanel.BorderWidth;
                            point2.X -= this.MDITabPanel.BorderWidth;
                        }
                    }
                    else if( this.Alignment == TabAlignment.Bottom )
                    {
                        point1 = new Point( bounds.Left, bounds.Bottom - this.MDITabPanel.Height - 2 );
                        point2 = new Point( bounds.Right, bounds.Bottom - this.MDITabPanel.Height - 2 );
                        if( this.IsVS2008 )
                        {
                            point1.X += this.MDITabPanel.BorderWidth;
                            point2.X -= this.MDITabPanel.BorderWidth;
                        }
                    }


                    g.DrawLine( pen, point1, point2 );
                }
            }
        }
		#endregion

		#region Class Public Methods
		/// <summary></summary>
		/// <param name="splitterHost"/>
		[DocumentationExclude()]
		public void ListenToSplitterHost( SplitterHost splitterHost )
		{
			if( splitterHost == null )
			{
				throw new ArgumentNullException( "splitterHost" );
			}

			m_SplitterHost = splitterHost;
			m_SplitterHost.SplitterMoved += new SplitterHostEventHandler( Splitter_Moved );
		}

		#endregion

		#region Implementation
		/// <summary>
		/// Performs layout for TabHost's MDITabPanel control.
		/// </summary>
		/// <param name="pSize"> Preferred size for TabHost. </param>
		internal void PerformLayoutInternal( Size pSize )
		{
			if( mdiTabPanel != null )
			{
				Size szPreferred = mdiTabPanel.GetMDITabPanelPreferredSize().ToSize();

				if( mdiTabPanel.IsVerticalAlignment )
				{
					szPreferred = new Size( szPreferred.Height, szPreferred.Width );

					if( pSize.Width > szPreferred.Width + m_borderHeight )
					{
						this.Width = szPreferred.Width + m_borderHeight - 1;
						mdiTabPanel.Width = szPreferred.Width;
					}
					else
					{
						this.Width = pSize.Width;
						mdiTabPanel.Width = pSize.Width - m_borderHeight - 1;
					}

					mdiTabPanel.Height = pSize.Height;
				}
				else
				{
					if( pSize.Height > szPreferred.Height + m_borderHeight )
					{
						this.Height = szPreferred.Height + m_borderHeight;
						mdiTabPanel.Height = szPreferred.Height - 1;
					}
					else
					{
						this.Height = pSize.Height;
						mdiTabPanel.Height = pSize.Height - m_borderHeight - 1;
					}

					mdiTabPanel.Width = pSize.Width;
				}
			}
		}

        /// <summary>
        /// Draw borders for top tabAlignment.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        private void DrawTopAlignmentBorders( Graphics g, Rectangle bounds, Color borderColor, Color lightBorderColor )
        {
            if( this.IsVS2008 )
            {
                using( Pen pen = new Pen( lightBorderColor ) )
                using( GraphicsPath path = GetOuterTopAlignmentBordersPath( new Rectangle( bounds.X + 1, bounds.Y, bounds.Width - 2, bounds.Height - 1 ) ) )
                {
                    g.DrawPath( pen, path );
                }
            }

            using( Pen pen = new Pen( borderColor ) )
            {
                using( GraphicsPath path = GetOuterTopAlignmentBordersPath( bounds ) )
                {
                    g.DrawPath( pen, path );
                }
                using( GraphicsPath path = GetInnerTopAlignmentBordersPath( bounds ) )
                {
                    g.DrawPath( pen, path );
                }
            }
        }

        /// <summary>
        /// Draw borders for left tabAlignment.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        private void DrawLeftAlignmentBorders( Graphics g, Rectangle bounds, Color borderColor, Color lightBorderColor )
        {
            if( this.IsVS2008 )
            {
                using( Pen pen = new Pen( lightBorderColor ) )
                using( GraphicsPath path = GetOuterLeftAlignmentBordersPath( new Rectangle( bounds.X, bounds.Y + 1, bounds.Width - 1, bounds.Height - 2 ) ) )
                {
                    g.DrawPath( pen, path );
                }
            }

            using( Pen pen = new Pen( borderColor ) )
            {
                using( GraphicsPath path = GetOuterLeftAlignmentBordersPath( bounds ) )
                {
                    g.DrawPath( pen, path );
                }
                using( GraphicsPath path = GetInnerLeftAlignmentBordersPath( bounds ) )
                {
                    g.DrawPath( pen, path );
                }
            }
        }

        /// <summary>
        /// Draw borders for bottom tabAlignment.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        private void DrawBottomAlignmentBorders( Graphics g, Rectangle bounds, Color borderColor, Color lightBorderColor )
        {
            if( this.IsVS2008 )
            {
                using( Pen pen = new Pen( lightBorderColor ) )
                using( GraphicsPath path = GetOuterBottomAlignmentBordersPath( new Rectangle( bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 1 ) ) )
                {
                    g.DrawPath( pen, path );
                }
            }

            using( Pen pen = new Pen( borderColor ) )
            {
                using( GraphicsPath path = GetOuterBottomAlignmentBordersPath( bounds ) )
                {
                    g.DrawPath( pen, path );
                }
                using( GraphicsPath path = GetInnerBottomAlignmentBordersPath( bounds ) )
                {
                    g.DrawPath( pen, path );
                }
            }
        }

        /// <summary>
        /// Draw borders for right tabAlignment.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bounds"></param>
        private void DrawRightAlignmentBorders( Graphics g, Rectangle bounds, Color borderColor, Color lightBorderColor )
        {
            if( this.IsVS2008 )
            {
                using( Pen pen = new Pen( lightBorderColor ) )
                using( GraphicsPath path = GetOuterRightAlignmentBordersPath( new Rectangle( bounds.X + 1, bounds.Y + 1, bounds.Width - 1, bounds.Height - 2 ) ) )
                {
                    g.DrawPath( pen, path );
                }
            }

            using( Pen pen = new Pen( borderColor ) )
            {
                using( GraphicsPath path = GetOuterRightAlignmentBordersPath( bounds ) )
                {
                    g.DrawPath( pen, path );
                }
                using( GraphicsPath path = GetInnerRightAlignmentBordersPath( bounds ) )
                {
                    g.DrawPath( pen, path );
                }
            }
        }

        /// <summary>
        /// Gets outer borders for top tabAlignment.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private GraphicsPath GetOuterTopAlignmentBordersPath( Rectangle bounds )
        {
            GraphicsPath path = new GraphicsPath();
            if( !this.IsOffice2007Style && !this.IsDockingWhidbeyStyle )
            {
                path.AddLine( bounds.Left, bounds.Top, bounds.Right, bounds.Top );
            }
            path.AddLine( bounds.Left, bounds.Top, bounds.Left, bounds.Bottom );
            path.AddLine( bounds.Left, bounds.Bottom - c_cornerCut, bounds.Left + c_cornerCut, bounds.Bottom );
            path.AddLine( bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1 );
            path.AddLine( bounds.Right - c_cornerCut, bounds.Bottom - 1, bounds.Right, bounds.Bottom - c_cornerCut - 1 );
            path.AddLine( bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom );

            return path;
        }

        /// <summary>
        /// Gets inner borders for top tabAlignment.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private GraphicsPath GetInnerTopAlignmentBordersPath( Rectangle bounds )
        {
            GraphicsPath path = new GraphicsPath();
            int width = this.MDITabPanel.BorderWidth; ;

            path.AddLine( bounds.Left + width - 1, bounds.Top, bounds.Left + width - 1, bounds.Bottom - width );                
            path.AddLine( bounds.Left + width - 1, bounds.Bottom - width, bounds.Right - width, bounds.Bottom - width );                
            path.AddLine( bounds.Right - width, bounds.Top, bounds.Right - width, bounds.Bottom - width );

            return path;
        }

        /// <summary>
        /// Gets outer borders for left tabAlignment.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private GraphicsPath GetOuterLeftAlignmentBordersPath( Rectangle bounds )
        {
            GraphicsPath path = new GraphicsPath();
            if( !this.IsOffice2007Style && !this.IsDockingWhidbeyStyle )
            {
                path.AddLine( bounds.Left, bounds.Top, bounds.Left, bounds.Bottom );
            }
            path.AddLine( bounds.Left, bounds.Top, bounds.Right, bounds.Top );
            path.AddLine( bounds.Right - c_cornerCut, bounds.Top, bounds.Right, bounds.Top + c_cornerCut );
            path.AddLine( bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom );
            path.AddLine( bounds.Right, bounds.Bottom - c_cornerCut - 1, bounds.Right - c_cornerCut, bounds.Bottom - 1 );
            path.AddLine( bounds.Left, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1 );

            return path;
        }

        /// <summary>
        /// Gets inner borders for left tabAlignment.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private GraphicsPath GetInnerLeftAlignmentBordersPath( Rectangle bounds )
        {
            GraphicsPath path = new GraphicsPath();
            int width = this.MDITabPanel.BorderWidth;

            path.AddLine( bounds.Left, bounds.Top + width - 1, bounds.Right - width, bounds.Top + width - 1 );
            path.AddLine( bounds.Right - width, bounds.Top + width - 1, bounds.Right - width, bounds.Bottom - width );
            path.AddLine( bounds.Left, bounds.Bottom - width, bounds.Right - width, bounds.Bottom - width );

            return path;
        }

        /// <summary>
        /// Gets outer borders for bottom tabAlignment.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private GraphicsPath GetOuterBottomAlignmentBordersPath( Rectangle bounds )
        {
            GraphicsPath path = new GraphicsPath();

            if( !this.IsOffice2007Style && !this.IsDockingWhidbeyStyle )
            {
                path.AddLine( bounds.Right, bounds.Bottom - 2, bounds.Left, bounds.Bottom - 2 );
            }
            path.AddLine( bounds.Left, bounds.Top, bounds.Left, bounds.Bottom - 2 );
            path.AddLine( bounds.Left - 1, bounds.Top + c_cornerCut - 1, bounds.Left + c_cornerCut - 1, bounds.Top - 1 );
            path.AddLine( bounds.Left, bounds.Top - 1, bounds.Right , bounds.Top - 1 );
            path.AddLine( bounds.Right - c_cornerCut, bounds.Top - 1, bounds.Right , bounds.Top + c_cornerCut - 1 );
            path.AddLine( bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom - 2 );

            return path;
        }

        /// <summary>
        /// Gets inner borders for bottom tabAlignment.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private GraphicsPath GetInnerBottomAlignmentBordersPath( Rectangle bounds )
        {
            GraphicsPath path = new GraphicsPath();
            int width = this.MDITabPanel.BorderWidth;

            path.AddLine( bounds.Left + width - 1, bounds.Top + width - 1, bounds.Left + width - 1, bounds.Bottom - 2 );
            path.AddLine( bounds.Left + width - 1, bounds.Top + width - 2, bounds.Right - width, bounds.Top + width - 2 );
            path.AddLine( bounds.Right - width, bounds.Top + width - 1, bounds.Right - width, bounds.Bottom - 2 );

            return path;
        }

        /// <summary>
        /// Gets outer borders for right tabAlignment.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private GraphicsPath GetOuterRightAlignmentBordersPath( Rectangle bounds )
        {
            GraphicsPath path = new GraphicsPath();

            if( !this.IsOffice2007Style && !this.IsDockingWhidbeyStyle )
            {
                path.AddLine( bounds.Right - 2, bounds.Top, bounds.Right - 2, bounds.Bottom );
            }
            path.AddLine( bounds.Left, bounds.Bottom - 1, bounds.Right - 2, bounds.Bottom - 1 );
            path.AddLine( bounds.Left + c_cornerCut - 1, bounds.Bottom, bounds.Left - 1, bounds.Bottom - c_cornerCut );
            path.AddLine( bounds.Left - 1, bounds.Bottom, bounds.Left - 1, bounds.Top );
            path.AddLine( bounds.Left - 2, bounds.Top + c_cornerCut, bounds.Left + c_cornerCut - 2, bounds.Top );
            path.AddLine( bounds.Left, bounds.Top, bounds.Right - 2, bounds.Top );

            return path;
        }

        /// <summary>
        /// Gets inner borders for right tabAlignment.
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        private GraphicsPath GetInnerRightAlignmentBordersPath( Rectangle bounds )
        {
            GraphicsPath path = new GraphicsPath();
            int width = this.MDITabPanel.BorderWidth;

            path.AddLine( bounds.Right - 2, bounds.Bottom - width, bounds.Left + width - 1, bounds.Bottom - width );
            path.AddLine( bounds.Left + width - 2, bounds.Bottom - width, bounds.Left + width - 2, bounds.Top + width - 1 );
            path.AddLine( bounds.Left + width - 1, bounds.Top + width - 1, bounds.Right - 2, bounds.Top + width - 1 );

            return path;
        }

		#endregion

	}

	/// <summary></summary>
	public class DropDownPopupEventArgs : CancelEventArgs
	{
		#region Class members
		/// <summary></summary>
		private Point m_pLocation = Point.Empty;
		/// <summary></summary>
		private ParentBarItem m_ParentBarItem = null;
		#endregion

		#region Class properties
		/// <summary>
		/// Gets or sets popup menu location.
		/// </summary>
		public Point Location
		{
			get
			{
				return this.m_pLocation;
			}
			set
			{
				if( this.m_pLocation != value )
				{
					this.m_pLocation = value;
				}
			}
		}

		/// <summary>
		/// Gets X coordinates of popup menu location.
		/// </summary>
		public int X
		{
			get
			{
				return m_pLocation.X;
			}
		}

		/// <summary>
		/// Gets Y coordinates of popup menu location.
		/// </summary>
		public int Y
		{
			get
			{
				return m_pLocation.Y;
			}
		}

		/// <summary>
		/// Gets parent bar item of popup menu.
		/// </summary>
		public ParentBarItem ParentBarItem
		{
			get
			{
				return m_ParentBarItem;
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary></summary>
		/// <param name="parentBarItem"/>
		/// <param name="location"/>
		public DropDownPopupEventArgs( ParentBarItem parentBarItem, Point location )
			: base()
		{
			this.m_ParentBarItem = parentBarItem;
			this.m_pLocation = location;
		}
		#endregion
	}

	/// <summary>
	/// DropDownPopupEventArgs delegate.
	/// </summary>
	/// <returns></returns>
	/// <param name="sender"/>
	/// <param name="e"/>
	public delegate void DropDownPopupEventHandler( object sender, DropDownPopupEventArgs e );

	/// <summary></summary>
	[DocumentationExclude()]
	public class SplitterHostEventArgs : EventArgs
	{
		#region Class members
		/// <summary></summary>
		private int delta;
		#endregion

		#region Class properties
		/// <summary></summary>
		public int Delta
		{
			get
			{
				return this.delta;
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary></summary>
		/// <param name="delta"/>
		internal SplitterHostEventArgs( int delta )
			: base()
		{
			this.delta = delta;
		}
		#endregion
	}

	/// <summary></summary>
	/// <returns></returns>
	/// <param name="sender"/>
	/// <param name="e"/>
	[DocumentationExclude()]
	public delegate void SplitterHostEventHandler( object sender, SplitterHostEventArgs e );

	/// <summary></summary>
	[DocumentationExclude()]
	public class SplitterHost :
		Form,
		ISplitterHost
	{
		#region Class members
		/// <summary></summary>
		private int m_iMaxDeltaPos;
		/// <summary></summary>
		private int m_iMaxDeltaNeg;
		/// <summary></summary>
		private TabbedMDIManager m_mdiManager;
		/// <summary></summary>
		private Splitter splitter;
		/// <summary></summary>
		private int m_iStartPosition = -1;
		/// <summary> True indicates horizontal alignment for splitter, false - vertical one. </summary>
		private bool m_iHorizontalAlignment = true;
		/// <summary></summary>
		private int delta = Int32.MaxValue;
		/// <summary>
		/// Indicates LayoutPanel to which SplitterHost is belong to.
		/// </summary>
		private LayoutPanel m_lpLayoutPanel;
		#endregion

		#region Class properties
		/// <summary></summary>
		public Splitter Splitter
		{
			get
			{
				return splitter;
			}
		}

		/// <summary> </summary>
		public bool Horizontal
		{
			get
			{
				return m_iHorizontalAlignment;
			}
			set
			{
				if( m_iHorizontalAlignment != value )
				{
					m_iHorizontalAlignment = value;

					if( m_iHorizontalAlignment )
					{
						splitter.Dock = DockStyle.Top;
						Height = TabbedMDIManager.SPLITTER_WIDTH;
					}
					else
					{
						splitter.Dock = DockStyle.Left;
						Width = TabbedMDIManager.SPLITTER_WIDTH;
					}

					delta = Int32.MaxValue;
				}
			}
		}
		/// <summary></summary>
		private int Delta
		{
			set
			{
				if( delta != value )
				{
					if( delta != Int32.MaxValue )
					{
						DragRectDrawing.DrawDragFBRectangle( GetDragRect( delta ) );
					}
					delta = value;
					if( delta != Int32.MaxValue )
					{
						DragRectDrawing.DrawDragFBRectangle( GetDragRect( delta ) );
					}
				}
				if( delta == Int32.MaxValue )
				{
					m_iStartPosition = -1;
				}
			}
		}
		/// <summary>
		/// Gets or sets LayoutPanel to which SplitterHost is belong to.
		/// </summary>
		internal LayoutPanel LayoutPanel
		{
			get
			{
				return m_lpLayoutPanel;
			}
			set
			{
				if( m_lpLayoutPanel != value )
				{
					m_lpLayoutPanel = value;
				}
			}
		}
		#endregion

		#region Class events
		/// <summary></summary>
		public event SplitterHostEventHandler SplitterMoved;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary></summary>
		/// <param name="mdiManager"/>
		public SplitterHost( TabbedMDIManager mdiManager )
			: base()
		{
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
			this.m_mdiManager = mdiManager;
			this.FormBorderStyle = FormBorderStyle.None;
			this.ControlBox = false;
			this.HelpButton = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = SizeGripStyle.Hide;
			this.Size = new Size( TabbedMDIManager.SPLITTER_WIDTH, TabbedMDIManager.SPLITTER_WIDTH );

			this.splitter = new Splitter();
			this.splitter.Dock = DockStyle.Top;
			this.Splitter.Size = this.Size;
			this.splitter.TabIndex = 0;
			this.splitter.TabStop = false;
			this.splitter.MouseUp += new MouseEventHandler( Splitter_MouseUp );
			this.splitter.MouseDown += new MouseEventHandler( Splitter_MouseDown );
			this.splitter.MouseMove += new MouseEventHandler( Splitter_MouseMove );
			this.splitter.Paint += new PaintEventHandler( SplitterPaint );

			this.Controls.AddRange( new Control[] { this.splitter } );

			// Generate a uniuqe ID for the tab hosts.
			string splitterHostName = "SplitterHost_1";
			while( !m_mdiManager.IsValidTabHostsName( splitterHostName ) )
			{
				splitterHostName = IDGenerator.GetNextID( splitterHostName );
			}
			this.Name = splitterHostName;

			this.splitter.Name = this.Name + "_Splitter";
		}

		#endregion

		#region Class Public Methods
		/// <summary></summary>
		/// <returns></returns>
		public bool CancelOperation()
		{
			if( delta != Int32.MaxValue )
			{
				Delta = Int32.MaxValue;
				// To make the curson turn normal, toggle the Enabled state
				splitter.Enabled = false;
				splitter.Enabled = true;
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion

		#region Class utility methods
		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void SplitterPaint( object sender, PaintEventArgs e )
		{
			Rectangle bounds = splitter.ClientRectangle;
			if( Horizontal )
			{
				bounds.X -= 1;
				bounds.Height -= 1;
				bounds.Width += 1;
			}
			else
			{
				bounds.Y -= 1;
				bounds.Width -= 1;
				bounds.Height += 1;
			}

			using( Pen pen = new Pen( SystemColors.ControlDark, 1 ) )
			{
				e.Graphics.DrawRectangle( pen, bounds );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pDelta"></param>
		/// <returns></returns>
		private bool IsValidDelta( int pDelta )
		{
			if( pDelta > 0 )
			{
				return pDelta < m_iMaxDeltaPos;
			}
			else
			{
				pDelta = -pDelta;
				return pDelta < m_iMaxDeltaNeg;
			}
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void Splitter_MouseUp( object sender, MouseEventArgs e )
		{
			if( m_iStartPosition == -1 )
			{
				return;
			}

			int iDelta = GetCurrentDelta( e );
			Delta = Int32.MaxValue;

			OnSplitterMoved( iDelta );
            PerformLayout();
            this.splitter.Refresh();
            m_mdiManager.MdiParent.Refresh();
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void Splitter_MouseMove( object sender, MouseEventArgs e )
		{
			if( m_iStartPosition != -1 )
			{
				Delta = GetCurrentDelta( e );
			}
		}

		/// <summary></summary>
		/// <param name="sender"/>
		/// <param name="e"/>
		private void Splitter_MouseDown( object sender, MouseEventArgs e )
		{
			if( m_iHorizontalAlignment )
			{
				m_iStartPosition = e.Y;
			}
			else
			{
				m_iStartPosition = e.X;
			}

			m_mdiManager.GetMaxDelta( this, ref m_iMaxDeltaPos, ref m_iMaxDeltaNeg );
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="e"/>
		private int GetCurrentDelta( MouseEventArgs e )
		{
			int iDelta;

			if( m_iHorizontalAlignment )
			{
				iDelta = e.Y - m_iStartPosition;
			}
			else
			{
				iDelta = e.X - m_iStartPosition;
			}

			if( !IsValidDelta( iDelta ) )
			{
				if( iDelta > 0 )
				{
					iDelta = m_iMaxDeltaPos;
				}
				else
				{
					iDelta = -m_iMaxDeltaNeg;
				}
			}
			return iDelta;
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="delta"/>
		protected virtual Rectangle GetDragRect( int delta )
		{
			Rectangle dragRect = ClientRectangle;
			dragRect = RectangleToScreen( dragRect );
			if( Horizontal )
			{
				dragRect.Offset( 0, delta );
			}
			else
			{
				dragRect.Offset( delta, 0 );
			}
			return dragRect;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pDelta"></param>
		protected void OnSplitterMoved( int pDelta )
		{
			if( SplitterMoved != null )
			{
				SplitterMoved( this, new SplitterHostEventArgs( pDelta ) );
                foreach (Form frm in m_mdiManager.MdiChildren)
                {
                    frm.PerformLayout();
                }
			}
		}

		/// <summary></summary>
		/// <param name="m"/>
		protected override /*NativeWindow*/ void WndProc( ref Message m )
		{
			// Key for preventing focus being set on the Host forms.
			// Turning off ControlStyles.Selectable doesn't seem to do it.
			if( m.Msg == NativeMethods.WM_SETFOCUS )
			{
				return;
			}
			else if( m.Msg == NativeMethods.WM_CHILDACTIVATE )
			{
				// So that when the user mouse downs on this, it still stays on the bottom of the z-order.
				NativeMethods.SetWindowPos( this.Handle, ( IntPtr ) NativeMethods.HWND_BOTTOM,
					0, 0, 0, 0,
					NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE );

				return;
			}

			base.WndProc( ref m );
		}
		#endregion
	}

	/// <summary>
	/// Defines an interface through which a child <see cref="System.Windows.Forms.Form"/>
	/// will get notifications from a <see cref="TabbedMDIManager"/> regarding certain events.
	/// </summary>
	/// <remarks>
	/// <para>
	/// You should implement this interface when you want to customize certain features in the
	/// <b>TabbedMDIManager</b>. You can customize the context menu that pops up when the user
	/// right clicks on the tab, for example.
	/// </para>
	/// </remarks>
	public interface ITabbedMDIChildForm
	{
		/// <summary>
		/// Indicates whether the user can drag and drop the tab corresponding to this
		/// mdi child in the tabbed mdi.
		/// </summary>
		bool AllowUserDrag
		{
			get;
		}
		/// <summary>
		/// Called when a <see cref="TabbedMDIManager"/> is attached to the main form.
		/// </summary>
		/// <param name="manager">The <b>TabbedMDIManager</b>.</param>
		/// <remarks>
		/// This will also get called when a new child form implementing this interface is being created and
		/// added to an mdi parent bound to a <b>TabbedMDIManager</b>.
		/// </remarks>
		void OnAttachTabbedMDI( TabbedMDIManager manager );
		/// <summary>
		/// Called to indicate that the <b>TabbedMDIManager</b> is being detached from the mdi parent.
		/// </summary>
		/// <param name="manager">The <b>TabbedMDIManager</b>.</param>
		void OnDetachTabbedMDI( TabbedMDIManager manager );
		/// <summary>
		/// Called before the context menu is shown when the user right-clicks on a tab.
		/// </summary>
		/// <param name="contextMenuParentItem">The <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ParentBarItem"/> 
		/// representing the context menu.</param>
		void OnTabContextMenuPopup( ParentBarItem contextMenuParentItem );
		/// <summary>
		/// Called after the context menu is shown when the user right-clicks on a tab.
		/// </summary>
		/// <param name="contextMenuParentItem">The <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ParentBarItem"/> 
		/// representing the context menu.</param>
		void OnTabContextMenuClosed( ParentBarItem contextMenuParentItem );
		/// <summary>
		/// Called when the mdi child has been added to a new <see cref="TabHost"/>.
		/// </summary>
		/// <param name="tabHost">The <see cref="TabHost"/> to which the mdi child form was added to.</param>
		/// <param name="tabIndex">The tab index representing the mdi child form in the tab control.</param>
		/// <remarks>Use the <see cref="TabHost.MDITabPanel"/> property to get a reference to the tab control.
		/// </remarks>
		void OnMdiChildAddedToTabHost( TabHost tabHost, int tabIndex );
		/// <summary>
		/// Returns the custom text for the tab.
		/// </summary>
		/// <param name="validValueReturned">Set this "out" param to true
		/// if you intend to provide a custom tab text, or else set it to false.</param>
		/// <returns>A string value that will be the corresponding tab's text,
		/// if you intend to return a valid value; else return value will be ignored.</returns>
		string GetCustomTabText( out bool validValueReturned );
	}

	/// <summary></summary>
	[
	Serializable,
	DocumentationExclude()
	]
	public class TabGroupsStateInfo : ISerializable
	{
		#region Constants
		/// <summary></summary>
		private const string GROUP_NAMES_VS_TAB_PAGES = "GroupNamesVsTabPages";
		/// <summary></summary>
		private const string SELECTED_PAGES = "SelectedPages";
		/// <summary></summary>
		private const string GROUP_NAMES = "GroupNames";
		/// <summary></summary>
		private const string GROUP_BASE = "TabGroup";
		/// <summary></summary>
		private const string MDI_AUTO_SCALE_BASE_SIZES = "MdiAutoScaleBaseSizes";
		/// <summary></summary>
		private const string TAB_ALIGNMENT = "TabAlignment";
		/// <summary></summary>
		private const string MDI_CLIENT_AUTO_SCALE_BASE_SIZES = "MdiClientAutoScaleBaseSize";
		/// <summary></summary>
		private const string LAYOUT_PANEL = "LayoutPanel";
		#endregion

		#region Fields
		/// <summary></summary>
		private int m_iAutoScaleBaseMdiClientDim = 0;
		/// <summary>
		/// Array of hints.
		/// </summary>
		private ArrayList m_lstMdiAutoScaleBaseDim = new ArrayList();
		/// <summary>
		/// Alignment of TabHosts in TabbedMDIManager.
		/// </summary>
		private bool m_bHorizontalAlignment = true;
		/// <summary>
		/// Contains string Vs ArrayList
		/// </summary>
		private Hashtable m_htGroupNamesVsTabPages = new Hashtable();
		/// <summary>
		/// 
		/// </summary>
		private Hashtable m_htSelectedPages = new Hashtable();
		/// <summary>
		/// 
		/// </summary>
		private Hashtable m_htGroupNames = new Hashtable();
		/// <summary>
		/// TabbedMdiManager's LayoutPanel that contains all layout data.
		/// </summary>
		private LayoutPanel m_lpLayoutPanel;
		#endregion

		#region Properties
		/// <summary></summary>
		public int TabGroupCount
		{
			get
			{
				return m_htGroupNamesVsTabPages.Keys.Count;
			}
		}
		/// <summary></summary>
		public int MdiClientAutoScaleBaseSize
		{
			get
			{
				return m_iAutoScaleBaseMdiClientDim;
			}
		}
		/// <summary></summary>
		public ArrayList MdiAutoScaleBaseSizes
		{
			get
			{
				return m_lstMdiAutoScaleBaseDim;
			}
		}
		/// <summary></summary>
		public bool Horizontal
		{
			get
			{
				return m_bHorizontalAlignment;
			}
		}
		/// <summary>
		/// Gets or sets TabbedMdiManager's LayoutPanel that contains all layout data.
		/// </summary>
		internal LayoutPanel LayoutPanel
		{
			get
			{
				return m_lpLayoutPanel;
			}
			set
			{
				m_lpLayoutPanel = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary></summary>
		private TabGroupsStateInfo()
		{
		}
		/// <summary></summary>
		/// <param name="pManager"/>
		public TabGroupsStateInfo( TabbedMDIManager pManager )
		{
			// Get manager
			TabbedGroupedMDIManager groupedManager = pManager as TabbedGroupedMDIManager;

			// Arraylist of auto scale dims of tab hosts
			for( int i = 0; i < pManager.TabGroupHosts.Length; i++ )
			{
				TabHost tabHost = pManager.TabGroupHosts[ i ];

				// Set the current AutoScaleBaseSize of the group.
				m_lstMdiAutoScaleBaseDim.Add( tabHost.MdiChildAutoScaleBaseDim );

				// Prepare an array of Tab pages.
				Hashtable tabNames = new Hashtable();
				foreach( TabPageAdv tabPage in tabHost.MDITabPanel.TabPages )
				{
					TabPageAdvCollection tabCollection = tabHost.MDITabPanel.TabPages;
					tabNames.Add( new UniquePageID( tabPage.Text ), tabCollection.IndexOf( tabPage ) );
				}

				string groupName = GROUP_BASE + i.ToString();

				// Set group names.
				if( groupedManager != null )
				{
					string strGroupName = groupedManager.GetGroupNameOfHost( tabHost );

					if( strGroupName != null )
					{
						m_htGroupNames[ groupName ] = strGroupName;
					}
				}

				// Set accordance between group name and Hashtable with Tab pages names.
				m_htGroupNamesVsTabPages[ groupName ] = tabNames;
				// Set selected index 
				m_htSelectedPages[ groupName ] = tabHost.MDITabPanel.SelectedIndex;
			}

			// Set base dimensions.
			m_iAutoScaleBaseMdiClientDim = pManager.autoScaleBaseMdiClientDim;
			// Set alignment.
			m_bHorizontalAlignment = pManager.Horizontal;
			// Set layout panel.
			m_lpLayoutPanel = pManager.LayoutPanel;
		}

		#endregion

		#region Serialization and Deserialization.
		/// <summary>
		/// Private constructor called during the deserialization process
		/// </summary>
		/// <param name="info"> Serialization info. </param>
		/// <param name="context"> Streaming context. </param>
		private TabGroupsStateInfo( SerializationInfo info, StreamingContext context )
		{
			foreach( SerializationEntry entry in info )
			{
				if( entry.Name == MDI_AUTO_SCALE_BASE_SIZES )
				{
					// AutoScaleBaseSizes array.
					m_lstMdiAutoScaleBaseDim = info.GetValue( MDI_AUTO_SCALE_BASE_SIZES, typeof( ArrayList ) ) as ArrayList;
				}
				else if( entry.Name == MDI_CLIENT_AUTO_SCALE_BASE_SIZES )
				{
					// Get the MDI Client's AutoScale base size.
					m_iAutoScaleBaseMdiClientDim = info.GetInt32( MDI_CLIENT_AUTO_SCALE_BASE_SIZES );
				}
				else if( entry.Name == TAB_ALIGNMENT )
				{
					// Get alignment. 
					m_bHorizontalAlignment = info.GetBoolean( TAB_ALIGNMENT );
				}
				else if( entry.Name == GROUP_NAMES_VS_TAB_PAGES )
				{
					// Get accordance between group names and tab pages.
					m_htGroupNamesVsTabPages = info.GetValue( GROUP_NAMES_VS_TAB_PAGES, typeof( Hashtable ) ) as Hashtable;
				}
				else if( entry.Name == SELECTED_PAGES )
				{
					// Get selected pages.
					m_htSelectedPages = info.GetValue( SELECTED_PAGES, typeof( Hashtable ) ) as Hashtable;
				}
				else if( entry.Name == GROUP_NAMES )
				{
					// Store group names.
					m_htGroupNames = info.GetValue( GROUP_NAMES, typeof( Hashtable ) ) as Hashtable;
				}
				else if( entry.Name == LAYOUT_PANEL )
				{
					// Get info about layout panel.
					m_lpLayoutPanel = info.GetValue( LAYOUT_PANEL, typeof( LayoutPanel ) ) as LayoutPanel;
				}

				// Backward compatibility.
				if( entry.Name == "mdiAutoScaleBaseDim" )
				{
					// AutoScaleBaseSizes array.
					m_lstMdiAutoScaleBaseDim = info.GetValue( "mdiAutoScaleBaseDim", typeof( ArrayList ) ) as ArrayList;
				}
				else if( entry.Name == "autoScaleBaseMdiClientDim" )
				{
					// Get the MDI Client's AutoScale base size.
					m_iAutoScaleBaseMdiClientDim = info.GetInt32( "autoScaleBaseMdiClientDim" );
				}
				else if( entry.Name == "horizontalAlignment" )
				{
					// Get alignment. 
					m_bHorizontalAlignment = info.GetBoolean( "horizontalAlignment" );
				}
				else if( entry.Name == "groupNamesVsTabPages" )
				{
					// Get accordance between group names and tab pages.
					m_htGroupNamesVsTabPages = info.GetValue( "groupNamesVsTabPages", typeof( Hashtable ) ) as Hashtable;
				}
				else if( entry.Name == "selectedPages" )
				{
					// Get selected pages.
					m_htSelectedPages = info.GetValue( "selectedPages", typeof( Hashtable ) ) as Hashtable;
				}
				else if( entry.Name == "htGroupNames" )
				{
					// Store group names.
					m_htGroupNames = info.GetValue( "htGroupNames", typeof( Hashtable ) ) as Hashtable;
				}
			}
		}
		/// <summary>
		/// Gets data from TabbedMdiManager to save TabHosts and MdiClients states.
		/// </summary>
		/// <param name="info"> Serialization info. </param>
		/// <param name="context"> Streaming context. </param>
		public void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			// Store accordance between group names and tab pages.
			info.AddValue( GROUP_NAMES_VS_TAB_PAGES, m_htGroupNamesVsTabPages, typeof( Hashtable ) );
			// Store selected pages.
			info.AddValue( SELECTED_PAGES, m_htSelectedPages, typeof( Hashtable ) );
			// Store group names.
			info.AddValue( GROUP_NAMES, m_htGroupNames, typeof( Hashtable ) );
			// AutoScaleBaseSizes array.
			info.AddValue( MDI_AUTO_SCALE_BASE_SIZES, m_lstMdiAutoScaleBaseDim );
			// Store MDI Client's AutoScale base size.
			info.AddValue( MDI_CLIENT_AUTO_SCALE_BASE_SIZES, m_iAutoScaleBaseMdiClientDim );
			// Store alignment of TabHosts.
			info.AddValue( TAB_ALIGNMENT, m_bHorizontalAlignment );
			// Store info about layout panel.
			info.AddValue( LAYOUT_PANEL, m_lpLayoutPanel, typeof( LayoutPanel ) );
		}
		#endregion

		#region Implementation
		/// <summary></summary>
		/// <returns></returns>
		/// <param name="i"/>
		public string GetGroupName( int i )
		{
			string key = GROUP_BASE + i.ToString();

			string groupName = m_htGroupNames[ key ] as string;

			if( groupName == null )
			{
				groupName = key;
			}

			return groupName;
		}

		/// <summary></summary>
		/// <returns></returns>
		/// <param name="i"/>
		public Hashtable GetTabNamesOfGroup( int i )
		{
			string groupName = GROUP_BASE + i.ToString();

			return m_htGroupNamesVsTabPages[ groupName ] as Hashtable;
		}

		/// <summary>
		/// Gets index of group's selected page.
		/// </summary>
		/// <param name="i"> Indicates group name. </param>
		/// <returns></returns>
		public int GetSelectedPagesOfGroup( int i )
		{
			string groupName = GROUP_BASE + i.ToString();

			if( m_htSelectedPages != null && m_htSelectedPages.Contains( groupName ) )
			{
				return ( int ) m_htSelectedPages[ groupName ];
			}
			else
			{
				return -1;
			}
		}
		#endregion
	}

	/// <summary>
	/// The event args for the <see cref="TabbedMDIManager.TabControlAdding"/>, 
	/// <see cref="TabbedMDIManager.TabControlAdded"/> and <see cref="TabbedMDIManager.TabControlRemoved"/> events.
	/// </summary>
	public class TabbedMDITabControlEventArgs : EventArgs
	{
		#region Class members
		/// <summary></summary>
		private MDITabPanel _tabControl;
		#endregion

		#region Class properties
		/// <summary>
		/// Gets / sets the tab control instance that is being added or removed.
		/// </summary>
		/// <remove>You can set a custom instance only when the current value is null,
		/// otherwise an exception will be thrown.</remove>
		public MDITabPanel TabControl
		{
			get
			{
				return this._tabControl;
			}
			set
			{
				if( this._tabControl == null )
				{
					this._tabControl = value;
				}
				else
				{
					throw new ArgumentException( "The TabControl property is already set, you cannot reset it.", "value" );
				}
			}
		}
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary></summary>
		/// <param name="tabControl"/>
		public TabbedMDITabControlEventArgs( MDITabPanel tabControl )
		{
			this._tabControl = tabControl;
		}

		#endregion
	}

	/// <summary>
	/// Handles the <see cref="TabbedMDIManager.TabControlAdding"/>, 
	/// <see cref="TabbedMDIManager.TabControlAdded"/> and <see cref="TabbedMDIManager.TabControlRemoved"/> events.
	/// </summary>
	/// <returns></returns>
	/// <param name="sender"/>
	/// <param name="args"/>
	public delegate void TabbedMDITabControlEventHandler( object sender, TabbedMDITabControlEventArgs args );

	/// <summary>
	/// Delegate for the TabbedMDIManager.BeforeMDIChildAdded event.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public delegate void MDIChildAddCancelEventHandler( object sender, MDIChildAddCancelEventArgs args );

	/// <summary>
	/// Event data for the TabbedMDIManager.BeforeMDIChildAdded event.
	/// </summary>
	public class MDIChildAddCancelEventArgs : CancelEventArgs
	{
		private Control newControl = null;

		/// <summary>
		/// The control/form that is to be added to the TabbedMDIManager.
		/// </summary>
		public Control NewControl
		{
			get
			{
				return this.newControl;
			}
		}

		public MDIChildAddCancelEventArgs( Control ctrl )
		{
			this.newControl = ctrl;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	internal class MDIChildWindow:
		NativeWindow,
		IMessageFilter,
		IDisposable
	{
		#region Class events
		/// <summary></summary>
		public event EventHandler SetIcon;
		#endregion

		#region Initialization
		/// <summary></summary>
		/// <param name="mdiChild"></param>
		public MDIChildWindow( Form mdiChild )
		{
			if( mdiChild == null )
				throw new ArgumentNullException( "mdiChild" );

			m_mdiChild = mdiChild;

			if( m_mdiChild.IsHandleCreated )
			{
				AssignHandle();
			}
			else
			{
				m_mdiChild.HandleCreated += new EventHandler( mdiChild_HandleCreated );
			}
		}

		void mdiChild_HandleCreated( object sender, EventArgs e )
		{
			m_mdiChild.HandleCreated -= new EventHandler( mdiChild_HandleCreated );
			AssignHandle();
		}

		private void AssignHandle()
		{
			this.AssignHandle( m_mdiChild.Handle );
		}

		#endregion

		#region Implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pIsDetached"></param>
		internal void UpdateRegion( bool pIsDetached )
		{
			if( pIsDetached )
			{
				NativeMethods.SetWindowRgn( this.Handle, IntPtr.Zero, true );
			}
			else
			{
				NativeMethods.RECT rc = new NativeMethods.RECT();
				NativeMethods.GetWindowRect( (int)this.Handle, ref rc );
				IntPtr hRgn = NativeMethods.CreateRectRgn( 0, 0, rc.Width, rc.Height );

				if( hRgn != IntPtr.Zero )
				{
					NativeMethods.SetWindowRgn( this.Handle, hRgn, true );
					NativeMethods.DeleteObject( hRgn );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmNcActivate( ref Message m )
		{
			m.WParam = IntPtr.Zero;
			m.Result = (IntPtr)0;

			// This method draws the title bar or icon title in its inactive colors when wParam is FALSE.
			DefWndProc( ref m );

			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmNcCalcSize( ref Message m )
		{
            NativeMethods.RECT rc = ( NativeMethods.RECT ) m.GetLParam( typeof( NativeMethods.RECT ) );

			DefWndProc( ref m );

			Marshal.StructureToPtr( rc, m.LParam, false );

			// This will remove the window frame and caption items from the window, leaving only the client area displayed.
			m.Result = IntPtr.Zero;

			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		private bool OnWmSize( ref Message m )
		{
			UpdateRegion( false );

			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool OnWmSetFocus()
		{
			return (this.m_mdiChild.MdiParent != null && this.m_mdiChild != this.m_mdiChild.MdiParent.ActiveMdiChild);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool OnWmMouseActivate()
		{
			m_mdiChild.Focus();

			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool OnWmSetIcon()
		{
			if( SetIcon != null )
			{
				SetIcon( this, new EventArgs() );
			}

			return false;
		}
		#endregion

		#region IMessageFilter implementation
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public bool PreFilterMessage( ref Message m )
		{
			bool bResult = false;

			switch( m.Msg )
			{
				case (int)NativeMethods.WM_NCCALCSIZE:
					bResult = OnWmNcCalcSize( ref m );
					break;
				case (int)NativeMethods.WM_SIZE:
					bResult = OnWmSize( ref m );
					break;
				case (int)NativeMethods.WM_NCACTIVATE:
					bResult = OnWmNcActivate( ref m );
					break;
			}

			return bResult;
		}

		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		public IMessageFilter MessageFilter
		{
			get
			{
				return m_messageFilter;
			}
			set
			{
				m_messageFilter = value;
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc( ref Message m )
		{
			bool bResult = m_messageFilter != null && m_messageFilter.PreFilterMessage( ref m );

			if( !bResult )
			{
				switch( m.Msg )
				{
					case (int)NativeMethods.WM_MOUSEACTIVATE:
						bResult = OnWmMouseActivate();
						break;
					case (int)NativeMethods.WM_SETICON:
						bResult = OnWmSetIcon();
						break;
				}
			}

			if( !bResult )
			{
				base.WndProc( ref m );
			}
		}
		#endregion

		#region Fields
		/// <summary>
		/// 
		/// </summary>
		IMessageFilter m_messageFilter;
		/// <summary>
		/// 
		/// </summary>
		private Form m_mdiChild;
		#endregion

		#region Dispose pattern implementation

		~MDIChildWindow()
		{
			Dispose( false );
		}

		protected void Dispose( bool bDisposing )
		{
			if( bDisposing )
			{
				m_mdiChild = null;
				m_messageFilter = null;
			}

			this.ReleaseHandle();
		}

		public void Dispose()
		{
			GC.SuppressFinalize( this );
			Dispose( true );
		}

		#endregion
	}
}