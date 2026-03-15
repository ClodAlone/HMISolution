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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel.Design;

using Syncfusion.Collections;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms.Tools.Design;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Win32;
using Syncfusion.Windows.Forms;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Tools.XPMenus
{
	#region *** PopupMenusManager
	/// <summary>
	/// Provides the extended "XPContextMenu" property and manages the activation of Context Menus
	/// (<see cref="Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu"/>) in the XP Menus framework.
	/// </summary>
	/// <remarks><para>Using this class you can easily associate a control
	/// with a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu"/>.</para>
	/// <para>This class provides an extended property "XPContextMenu" through which you can associate
	/// a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu"/> for any control on your form.
	/// Once set, the PopupMenusManager will automatically
	/// show the PopupMenu when the user right-clicks on the control.</para>
	/// <para>Note that when this component is disposed, it will not dispose any of the associated
	/// control or PopupMenus.</para>
	/// </remarks>
	/// <example>
	/// <para>Take a look at our XPToolbarsAndContextMenus sample under the Tools\Samples\Menus Package\ folder
	/// for usage example.</para>
	/// <para>To associate a control with a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu"/>
	/// use the SetXPContextMenu method in code as follows:</para>
	/// <code lang="C#">
	/// this.popupMenusManager1.SetXPContextMenu(this.richTextBox1, this.popupMenu2);
	/// </code>
	/// <code lang="VB">
	/// Me.popupMenusManager1.SetXPContextMenu(Me.richTextBox1, Me.popupMenu2)
	/// </code>
	/// </example>
	[
	ProvideProperty( "XPContextMenu", typeof( Control ) ),
	ToolboxBitmap( typeof( CommandBar ), "ToolboxIcons.PopupMenusManager.bmp" ),
	ToolboxItemFilter( "System.Windows.Forms" ),
	Description( "Manages the activation of Popup Menus in a control." )
	]
	public class PopupMenusManager
		: Component
		, System.ComponentModel.IExtenderProvider
		, IIgnoreWorkingArea
	{
		#region Fields
		private Hashtable contextMenus;
		private Hashtable cmPlaceHoldersByControl;
		private bool disposed = false;
		/// <summary>
		/// Ignore working area when menu begin popup.
		/// </summary>
		private bool m_bIgnoreWorkingArea = false;
		private Control parentForm = null;
		#endregion

		#region Properties
		/// <summary>
		/// Specifies whether to ignore the working area.
		/// </summary>
		[DefaultValue( false )]
        [Description("Specifies whether to ignore the working area.")]
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
					if( this.contextMenus != null && this.contextMenus.Count > 0 )
					{
						foreach( object obj in this.contextMenus.Keys )
						{
							PopupMenu pm = this.contextMenus[ obj ] as PopupMenu;
							if( pm != null )
							{
								pm.IgnoreWorkingArea = value;
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// Gets or sets the ParentForm for responding when parent form is a modal dialog.
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		Browsable( false )
		]
		public Control ParentForm
		{
			get
			{
				if( this.parentForm == null )
				{
					if( this.DesignMode == true )
					{
						IDesignerHost idhost = this.Site.Container as IDesignerHost;
						Form mainfrm = idhost.RootComponent as Form;
						this.parentForm = mainfrm;
					}
				}
				return this.parentForm;
			}
			set
			{
				UnWireParentForm( this.parentForm );
				this.parentForm = value;
				WireParentForm( this.parentForm );
			}
		}
		#endregion

		#region Initialization And Finalization
		/// <summary>
		/// Overloaded. Creates a new instance of the PopupMenusManager class.
		/// </summary>
		public PopupMenusManager()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				new Syncfusion.Core.Licensing.LicensedComponent( typeof( PopupMenusManager ) );
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}
			this.contextMenus = new Hashtable();
			this.cmPlaceHoldersByControl = new Hashtable();
		}
		/// <summary>
		/// Creates a new instance of the PopupMenusManager class and adds itself to the specified container.
		/// </summary>
		/// <param name="container">The container into which to add.</param>
		/// <remarks>This constructor is used at design-time to add a component to the form's
		/// IContainer field so that it gets disposed when the form gets disposed.</remarks>
		public PopupMenusManager( IContainer container )
			: this()
		{
			if( container != null )
				container.Add( this );
		}
		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Component"></see> and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( this.contextMenus != null )
				{
					// First copy to a list and then remove them.
					ArrayList controls = new ArrayList();
					foreach( Control control in this.contextMenus.Keys )
						controls.Add( control );

					foreach( Control control in controls )
						this.DetachContextMenu( control );
				}
				this.contextMenus = null;
				this.disposed = true;
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Associates a PopupMenu with a control.
		/// </summary>
		/// <param name="control">The control to associate with.</param>
		/// <param name="contextMenu">The PopupMenu to associate. Null to remove any association with the control.</param>
		/// <remarks>The PopupMenusManager will automatically show the PopupMenu when the user
		/// right clicks on the control.</remarks>
		public void SetXPContextMenu( Control control, PopupMenu contextMenu )
		{
			if( ( this.contextMenus[ control ] != null && this.contextMenus[ control ] != contextMenu )
				|| contextMenu == null )
				this.DetachContextMenu( control );

            if (control != null && contextMenu != null)
            {
                this.DetachContextMenu(control);
                this.AttachContextMenu(control, contextMenu); 
            }
		}
		/// <summary>
		/// Returns the associated PopupMenu of the control.
		/// </summary>
		/// <param name="control">The control whose PopupMenu is required.</param>
		[
		DefaultValue( null ),
		Category( "XP Menus" )
		]
		public PopupMenu GetXPContextMenu( Control control )
		{
			return this.contextMenus[ control ] as PopupMenu;
		}
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		/// <param name="contextMenu"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void AttachContextMenu( Control control, PopupMenu contextMenu )
		{
			if( this.disposed )
				return;

			Trace.Assert( contextMenu != null && control != null );
			if( !this.contextMenus.Contains( control ) )
			{
				if( !this.DesignMode )
				{
					ContextMenuPlaceHolder cm = control.ContextMenu as ContextMenuPlaceHolder;

					if( cm == null )
					{
						Form parentForm = control.FindForm();
						if( parentForm != null )
						{
							cm = parentForm.ContextMenu as ContextMenuPlaceHolder;
						}
					}


					// Create a new one.
					if( cm == null )
					{
						cm = new ContextMenuPlaceHolder();
					}

					cm.InitContextMenuSettings( this, control );
					this.cmPlaceHoldersByControl[ control ] = cm;
				}
			}

			contextMenu.IgnoreWorkingArea = this.IgnoreWorkingArea;
			this.contextMenus[ control ] = contextMenu;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DetachContextMenu( Control control )
		{
			if( this.disposed )
				return;

			// Set it to null, don't Remove.
			this.contextMenus.Remove( control );
			if( this.cmPlaceHoldersByControl.Contains( control ) )
			{
				ContextMenuPlaceHolder cm = this.cmPlaceHoldersByControl[ control ] as ContextMenuPlaceHolder;
				cm.ReleaseContextMenuSettings();
				this.cmPlaceHoldersByControl.Remove( control );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parentForm"></param>
		protected void WireParentForm( Control parentForm )
		{
			if( parentForm != null )
				parentForm.MouseUp += new MouseEventHandler( OnParentFormMouseUp );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parentForm"></param>
		protected void UnWireParentForm( Control parentForm )
		{
			if( parentForm != null )
				parentForm.MouseUp -= new MouseEventHandler( OnParentFormMouseUp );
		}
		/// <summary>
		/// Indicates whether the ParentForm property is to be serialized.
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeParentForm()
		{
			if( this.parentForm == null )
				return false;
			else
				return true;
		}
		/// <summary>
		/// 
		/// </summary>
		protected void ResetParentForm()
		{
			this.parentForm = null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		/// <param name="pt"></param>
		/// <param name="popupMenu"></param>
		/// <returns></returns>
		internal bool OnContextMenu( Control control, Point pt, PopupMenu popupMenu )
		{
			bool needShowPopup = true;

			if( this.disposed || popupMenu == null )
			{
				needShowPopup = false;
			}
			else
			{
				Form form = control.FindForm();

				if( form != null )
				{
					BarManager barManager = MainFrameBarManager.GetManagerFromForm( form );

					if( barManager != null )
					{
						MainFrameBarManager manager = barManager.MainFrameBarManager;

						Control controlBar = control.GetChildAtPoint( pt );

						if( controlBar != null && manager != null && controlBar.Visible
							&& manager.DetachedCommandBars != null
							&& manager.DetachedCommandBars.Contains( controlBar )
							&& this.contextMenus[ controlBar ] != popupMenu )
						{
							needShowPopup = false;
						}
					}
				}

				if( needShowPopup )
				{
					popupMenu.Show( control, pt );
				}
			}

			return needShowPopup;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		/// <param name="pt"></param>
		/// <returns></returns>
		internal bool OnContextMenu( Control control, Point pt )
		{
			if( this.disposed )
				return false;

			PopupMenu popupMenu = this.contextMenus[ control ] as PopupMenu;
			if( popupMenu != null )
			{
				return this.OnContextMenu( control, pt, popupMenu );
			}
			return false;
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnParentFormMouseUp( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Right )
			{
				OnContextMenu( this.ParentForm, new Point( e.X, e.Y ) );
			}
		}
		#endregion

		#region IExtenderProvider Members
		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		bool IExtenderProvider.CanExtend( object target )
		{
			if( target is Control )
				return true;
			else
				return false;
		}
		#endregion
	}
	#endregion

	#region *** PopupMenu
	/// <summary>
	/// The PopupMenu class lets you create XP like context menus in the XP Menus framework.
	/// </summary>
	/// <remarks>
	/// <para>The PopupMenu class works in conjunction with a <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.ParentBarItem"/>.</para>
	/// <para>You should first associate a ParentBarItem with a PopupMenu
	/// (using its <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu.ParentBarItem"/> property)
	/// and fill the ParentBarItem with BarItems that you want displayed in the popup.
	/// Then use the PopupMenu's <see cref="Syncfusion.Windows.Forms.Tools.XPMenus.PopupMenu.Show"/> method to popup a menu at any location.
	/// </para>
	/// <para>Note that the ParentBarItem attached to this PopupMenu need not be associated with
	/// any BarManager. However, if the ParentBarItem is part of a BarManager(which itself is already
	/// in the form's designer) then you can design the PopupMenu during design time
	/// (through the PopupMenu designer's "Customize" verb) by simply dragging and
	/// dropping items from the BarManager into the PopupMenu.
	/// </para>
	/// </remarks>
	/// <example>
	/// <para>Take a look at our samples under the Tools\Samples\Menus Package folder
	/// for usage example. The XPMenus sample will illustrate how to use the PopupMenu in the
	/// presence of a BarManager. The XPToolbarsAndContextMenus sample will illustrate how to
	/// use the PopupMenu in the absence of a BarManager, at stand alone.</para>
	/// <code lang="C#">
	/// // Create and initialize a ParentBarItem
	/// this.editBarItem = new Syncfusion.Windows.Forms.Tools.ParentBarItem();
	/// this.editMenu.Text = "Edit";
	/// this.editMenu.Items.AddRange(new BarItem[]
	///         {   this.cutItem,
	///             this.copyItem,});
	/// // Associate the ParentBarItem with the PopupMenu            
	/// this.popupMenu1 = new Syncfusion.Windows.Forms.Tools.PopupMenu();
	/// this.popupMenu1.ParentBarItem = this. editBarItem;
	/// 
	/// // Then associate it with a RichTextBox
	/// this.popupMenusManager = new PopupMenusManager();
	/// this.popupMenusManager.SetXPContextMenu(this.richTextBox1, this.popupMenu1);
	/// </code>
	/// <code lang="VB">
	///		' Create and initialize a ParentBarItem
	///     Me.editBarItem = New Syncfusion.Windows.Forms.Tools.ParentBarItem()
	///     Me.editMenu.Text = "Edit"
	///     Me.editMenu.Items.AddRange(New BarItem() {Me.cutItem, Me.copyItem})
	///     
	///     ' Associate the ParentBarItem with the PopupMenu
	///     Me.popupMenu1 = New Syncfusion.Windows.Forms.Tools.PopupMenu()
	///     Me.popupMenu1.ParentBarItem = Me.editBarItem
	/// 
	///		' Then associate it with a RichTextBox
	///		Me.popupMenusManager = New PopupMenusManager()
	///		Me.popupMenusManager.SetXPContextMenu(Me.richTextBox1, Me.popupMenu1)
	/// </code>
	/// </example>
	[
	Designer(
		typeof( Syncfusion.Windows.Forms.Tools.Design.PopupMenuDesigner ),
		typeof( System.ComponentModel.Design.IDesigner ) ),
	ToolboxBitmap( typeof( CommandBar ), "ToolboxIcons.PopupMenu.bmp" ),
	ToolboxItemFilter( "System.Windows.Forms" ),
	Description( "Represents a PopupMenu which lets you create XP like context menus." )
	]
	public class PopupMenu
		: Component
		, IPopupParent
		, IMessageFilter
		, IIgnoreWorkingArea
	{
		#region Fields
		protected IPopupChild childMenuUI;
		internal Form designTimeForm = null;
		private ParentBarItem parentBarItem;
		private Control menuParentControl;
		private Point curPopupLoc = Point.Empty;
		private Rectangle curPopupParentRect = Rectangle.Empty;
		private IPopupParent popupParent;
		private bool returnFromShowAfterMenuClose = true;
		private Form cachedParentForm = null;
		/// <summary>
		/// Ignore working area when menu begin popup.
		/// </summary>
		private bool m_bIgnoreWorkingArea = false;
		#endregion

		#region Properties
		/// <summary>
		/// Returns the control that is displaying the PopupMenu.
		/// </summary>
		/// <value>The control that is displaying the context
		///  menu. If no control has displayed the context menu, the property returns a 
		///  null reference (Nothing in Visual Basic).</value>
		///  <remarks><para>This property enables you to determine which control currently 
		///  displays the context menu defined in this PopupMenu. If the context menu is 
		///  not currently displayed, you can use this property to determine which 
		///  control last displayed the context menu. You can use this property in the 
		///  <see cref="ParentBarItem.Popup"/> event to ensure that the control displays the proper menu items. 
		///  You can also use this property to pass a reference to the control to a 
		///  method that performs the tasks associated with a menu command displayed 
		///  in the popup menu. Since the Form class inherits from control, you can 
		///  also use this property if the context menu is associated with a form.</para>
		///  <para>You can use this property in the <see cref="BeforePopup"/> event
		///  to figure out in which control the context menu was shown.</para>
		///  <para>This can also be used when a shortcut key triggers a <see cref="BarItem"/>.<see cref="Click"/> event to
		///  determine in which control's context the shortcut was processed, if a PopupMenu
		///  is associated with multiple controls.</para>
		///  </remarks>
		[Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
		public Control SourceControl
		{
			get { return this.menuParentControl; }
			set
			{
				if( this.IsShowing() && value != null )
					Trace.Assert( false, "Cannot set PopupMenu.SourceControl value when the Popup is droppedown." );

				this.menuParentControl = value;
			}
		}
		/// <summary>
		/// Gets or sets ignore working area when menu begin popup.
		/// </summary>
		[DefaultValue( false )]
		[Description( "Gets or sets ignore working area when menu begin popup." )]
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
				}
			}
		}
		/// <summary>
		/// Indicates whether the PopupMenu should be displayed within the WM_CONTEXTMENU message processing stack or 
		/// if the PopupMenu should be shown after a short delay (using a timer). Use the Asynchronous (timer based) 
		/// approach if you intend to dispose the underlying SourceControl from within a Popup event.
		/// </summary>
		[Browsable( true ), ReadOnly( true )]
		[Description( "Indicates whether the PopupMenu should be displayed within the WM_CONTEXTMENU message processing stack or if the PopupMenu should be shown after a short delay (using a timer)." )]
		public bool SynchronousPopup
		{
			get { return this.returnFromShowAfterMenuClose; }
			set { this.returnFromShowAfterMenuClose = value; }
		}
		/// <summary>
		/// Gets or sets the ParentBarItem that specifies the items in the dropdown menu.
		/// </summary>
		/// <remarks>
		/// The ParentBarItem that specifies the items in the dropdown menu. 
		/// The default value is null.
		/// <para>
		/// If this property is null, then calling Show on the PoupMenu will not 
		/// have any effect (will Assert in debug mode).</para>
		/// </remarks>
		[DefaultValue( null ),
			Description( "Indicates the ParentBarItem that specifies the items in the dropdown menu." )]
		public ParentBarItem ParentBarItem
		{
			get { return this.parentBarItem; }
			set
			{
				if( this.parentBarItem != value )
				{
					if( this.parentBarItem != null )
						this.Hide();

					this.parentBarItem = value;
					if( this.ParentBarItemChanged != null )
					{
						this.ParentBarItemChanged( this, EventArgs.Empty );
					}
				}
			}
		}
		/// <summary>
		/// Sets the design time form.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		public Form DesignTimeForm
		{
			set { this.designTimeForm = value; }
		}
		/// <summary>
		/// 
		/// </summary>
		internal IPopupParent PopupParent
		{
			get
			{
				return popupParent;
			}
		}
		#endregion

		#region Initialization And Finalization
		/// <summary>
		/// 
		/// </summary>
		static PopupMenu()
		{
			XPMenuGridFactory.InitMenus();
		}
		/// <summary>
		/// Overloaded. Creates a new instance of the PopupMenu class.
		/// </summary>
		public PopupMenu()
			: base()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				new Syncfusion.Core.Licensing.LicensedComponent( typeof( PopupMenu ) );
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}
			//this.parentBarItem = new ParentBarItem();
		}
		/// <summary>
		/// Creates a new instance of the Popupmenu class and adds itself to the specified container.
		/// </summary>
		/// <param name="container">The container into which to add.</param>
		/// <remarks>This constructor is used at design-time to add a component to the form's
		/// IContainer field so that it gets disposed when the form gets disposed.</remarks>
		public PopupMenu( IContainer container )
			: this()
		{
			if( container != null )
				container.Add( this );

			MessageFilterEntryHelper.AddMessageFilter( this, false );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			MessageFilterEntryHelper.RemoveMessageFilter( this );
			this.SourceControl = null;
			this.ParentBarItem = null;

			base.Dispose( disposing );
		}
		#endregion

		#region Public Methods
		/// <summary>
        /// Indicates whether a specified control is part of the popup hierarchy.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="askParent"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual bool IsRelatedControl( Control control, bool askParent )
		{
			if( this.popupParent != null )
				return this.popupParent.IsRelatedControl( control, askParent );

			if( this.designTimeForm != null )
			{
				if( control == this.designTimeForm || this.designTimeForm.Contains( control ) )
					return true;
			}
			return false;
		}
		/// <summary>
		/// Indicates whether the Popup is currently open 
		/// and dropped down.
		/// </summary>
		/// <returns>True if the Popup is open; false otherwise.</returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public bool IsShowing()
		{
			if( this.childMenuUI != null )
				return this.childMenuUI.IsShowing();
			else
				return false;
		}
		/// <overload>
		/// Pops up a menu at the specified location within the control.
		/// </overload>
		/// <summary>
		/// Pops up a menu beside the specified rectangle in the control.
		/// </summary>
		/// <param name="control">The parent control for this PopupMenu.</param>
		/// <param name="besideRect">The rectangular region in client co-ordinates of the control
		/// to which the popup will be docked.</param>
		/// <remarks>
		/// Use this version of Show to popup the menu around a rectangle
		/// like in a combo box rather than around a point. The Popup
		/// drop-down position will be determined based on the available
		/// screen area and the docking rectangle beside which to popup.
		/// <para>In NT4.0 this method will return immediately and the menu will be shown asynchronously.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">Will be thrown if control is null.</exception>
		/// <exception cref="Exception">Will be thrown if the control's handle is not created or if
		/// the control is not visible.</exception>
		public void Show( Control control, Rectangle besideRect )
		{
			if( control == null )
				throw new ArgumentNullException( "control", "Control cannot be null" );

			if( !control.IsHandleCreated || !control.Visible )
				throw new Exception( "Invalid Control specified as ContextMenu Parent" );

			if( this.IsShowing() )
				return;

			this.curPopupParentRect = control.RectangleToScreen( besideRect );

			this.menuParentControl = control;

			Form parentForm = control.FindForm();

			// Similar code portion in the 2 overloaded methods.
			if( parentForm is PopupHost )
			{
				PopupControlContainer pcc = null;
				// Find the PopupControlContainer
				foreach( Control c in control.FindForm().Controls )
				{
					if( c is PopupControlContainer )
					{
						pcc = c as PopupControlContainer;
						break;
					}
				}
				// Need to do this only if the PopupControlContainer is currently showing
				if( pcc != null && pcc.IsShowing() )
				{
					this.ShowPopupMenuWithPCCAsParent( Point.Empty, pcc );
					return;
				}
			}

			this.ShowChildrenUI( Point.Empty, this );
		}
		/// <summary>
		/// Pops up a menu beside the specified rectangle in the control.
		/// </summary>
		/// <param name="control">The parent control for this PopupMenu.</param>
		/// <param name="pos">The point in the control's client co-ordinates
		/// at which the popup will be dropped-down.</param>
		/// <remarks>
		/// <para>Use this version of Show to popup the menu at a specified point.
		/// The Popup drop-down alignment will be determined based on the available
		/// screen area and the point beside which to popup.</para>
		/// <para>In NT4.0 this method will return immediately and the menu will be shown asynchronously.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">Will be thrown if control is null.</exception>
		/// <exception cref="Exception">Will be thrown if the control's handle is not created or if
		/// the control is not visible.</exception>
		public void Show( Control control, Point pos )
		{
			if( control == null )
				throw new ArgumentNullException( "control", "Control cannot be null" );

			if( !control.IsHandleCreated )
				throw new Exception( "Invalid Control specified as ContextMenu Parent" );

			if( this.IsShowing()||!control.Visible )
				return;

			pos = control.PointToScreen( pos );
			this.curPopupLoc = pos;

			this.menuParentControl = control;

			Form parentForm = control.FindForm();

			// Similar code portion in the 2 overloaded methods.
			if( parentForm is PopupHost )
			{
				PopupControlContainer pcc = null;
				// Find the PopupControlContainer
				foreach( Control c in control.FindForm().Controls )
				{
					if( c is PopupControlContainer )
					{
						pcc = c as PopupControlContainer;
						break;
					}
				}
				// Need to do this only if the PopupControlContainer is currently showing
				if( pcc != null && pcc.IsShowing() )
				{
					this.ShowPopupMenuWithPCCAsParent( pos, pcc );
					return;
				}
			}

			this.ShowChildrenUI( pos, this );
		}
		/// <summary>
		/// Hides a Popup if it is being displayed.
		/// </summary>
		public void Hide()
		{
			if( this.childMenuUI != null )
			{
				this.childMenuUI.HidePopup( PopupCloseType.Canceled );
			}
		}
		/// <summary>
		/// Shows the children UI.
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="parentUI"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual IPopupChild ShowChildrenUI( Point pos, IPopupParent parentUI )
		{
			this.cachedParentForm = null;

			if( this.parentBarItem == null )
				return null;

			// Fire a cancelable BeforePopup that will give the user the mouse position.
			CancelMouseEventArgs e = new CancelMouseEventArgs( new MouseEventArgs( Control.MouseButtons, 1, pos.X, pos.Y, 1 ) );
			this.OnBeforePopup( e );
			if( e.Cancel )
				return null;

			if( parentUI != this )
				this.popupParent = parentUI;

			// The menuGrid will release itself based on settings
			MenuGrid menuGrid = this.MenuGrid;

			menuGrid.IgnoreWorkingArea = m_bIgnoreWorkingArea;

			this.childMenuUI = menuGrid;

			if( PopupManager.ActivePopupClient != null )
			{
				PopupManager.ActivePopupClient.HidePopup( PopupCloseType.Deactivated );
			}
			//Show the menu
			bool needShowPopup = false;
			foreach (BarItem item in this.ParentBarItem.Items)
			{
				if (item.Visible)
				{
					needShowPopup = true;
					break;
				}
			}
			if (needShowPopup)
				menuGrid.Show( this.parentBarItem, pos, this, false );

			if( Popup != null )
			{
				Popup( this, EventArgs.Empty );
			}

			if( !menuGrid.IsShowing() )
			{
				this.ChildClosing( menuGrid, PopupCloseType.Canceled );
				return null;
			}

			if( this.menuParentControl != null )
			{
				// Fire the MenuStart event
				cachedParentForm = this.SourceControl.FindForm();
				BarManager.OnStartingMenuNavigation( cachedParentForm, this.menuParentControl );
			}

			if( this.returnFromShowAfterMenuClose && ( ( this.parentBarItem.Manager == null ) || !this.parentBarItem.Manager.Customizing ) )
			{
				Application.DoEvents();
				NativeMethods.MSG msg = new NativeMethods.MSG();
				while( this.childMenuUI != null )
				{
					NativeMethods.MsgWaitForMultipleObjects( 0, new IntPtr[ 0 ], false, uint.MaxValue, 0x4ff );
					bool flag = NativeMethods.PeekMessage( ref msg, IntPtr.Zero, 0x204, 0x204, 0 );
					if( !flag )
					{
						flag = NativeMethods.PeekMessage( ref msg, IntPtr.Zero, 0x201, 0x201, 0 );
					}
					if( !flag )
					{
						flag = NativeMethods.PeekMessage( ref msg, IntPtr.Zero, 0xa1, 0xa1, 0 );
					}
					if( !flag )
					{
						flag = NativeMethods.PeekMessage( ref msg, IntPtr.Zero, 0xa4, 0xa4, 0 );
					}
					if( flag )
					{
						Control control = Control.FromHandle( msg.hwnd );
						if( ( PopupManager.ActivePopupClient == null ) ||
							!PopupManager.ActivePopupClient.IsRelatedControl( control, true ) )
						{
							this.Hide();
							break;
						}
					}
                    if (!flag)
                    {
                        if (this.GetPopupParentControl() != null && this.GetPopupParentControl().TopLevelControl != null 
                           && !(this.GetPopupParentControl().TopLevelControl is FloatingForm) && !(this.GetPopupParentControl().TopLevelControl.ContainsFocus) && !(this is CustomizingPopupMenu))
                        {
                            if (this.childMenuUI != null && this.childMenuUI.IsShowing())
                            {
                                this.Hide();
                            }
                        }
                    }
                    Application.DoEvents();
				}
			}

			return menuGrid;
		}

		/// <summary>
		/// Gets the available menu grid.
		/// </summary>
		protected virtual MenuGrid MenuGrid
		{
			get
			{
				return XPMenuGridFactory.GetMenuGridToDeploy();
			}
		}
		/// <summary>
		/// Returns the parent control.
		/// </summary>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual Control GetPopupParentControl()
		{
			if( this.popupParent != null )
				return this.popupParent.GetPopupParentControl();
			else
				return this.menuParentControl;
		}
		/// <overload>
		/// <para>Processes a shortcut key.</para>
		/// <para>The underlying <see cref="ParentBarItem"/> is allowed to process the shorcut which 
		/// will result in a <see cref="BarItem"/>'s <see cref="Click"/> event getting 
		/// fired, if one with the specified shortcut is found.</para>
		/// </overload>
		/// <summary>
		/// Processes the shortcut key.
		/// </summary>
		/// <param name="key">The shortcut key.</param>
		/// <returns>True if processed; false otherwise.</returns>
		/// <remarks>In this overload, if the shortcut was processed, the PopupMenu's <see cref="SourceControl"/> property
		/// will be set to null when the <see cref="BarItem"/>'s <see cref="Click"/> event is fired.
		/// </remarks>
		public bool ProcessShortcut( Keys key )
		{
			return this.ProcessShortcut( key, null );
		}
		/// <summary>
		/// Processes the shortcut key.
		/// </summary>
		/// <param name="key">The shorcut key.</param>
		/// <param name="control">The control in whose context the shortcut should be processed.</param>
		/// <returns>True if processed; false otherwise.</returns>
		/// <remarks>In this overload, if the shortcut was processed, the PopupMenu's <see cref="SourceControl"/> property
		/// will be set to the specified control when the <see cref="BarItem"/>'s <see cref="Click"/> event is fired.
		/// </remarks>
		public bool ProcessShortcut( Keys key, Control control )
		{
			if( this.parentBarItem != null )
			{
				BarItem item = this.parentBarItem.FindProcessableItemWithShortcut( key );
				if( item != null )
				{
					if( !IsShowing() )
					{
						this.SourceControl = control;
					}
					item.PerformClick();
					return true;
				}
				else
					return false;
			}
			else
				return false;
		}
		/// <summary>
		/// Child closing method.
		/// </summary>
		/// <param name="childUI"></param>
		/// <param name="popupCloseType"></param>
		[Syncfusion.Documentation.DocumentationExclude()]
		public virtual void ChildClosing( IPopupChild childUI, PopupCloseType popupCloseType )
		{
			if( this.popupParent != null )
				this.popupParent.ChildClosing( childUI, popupCloseType );

			// Fire MenuComplete
			if( cachedParentForm != null )
				BarManager.OnStoppingMenuNavigation( cachedParentForm, this.menuParentControl );

			if (this.Collapse != null)
				this.Collapse(this, EventArgs.Empty);

			// Caching the latest Control that showed the menu even after close up.
			this.cachedParentForm = null;
			this.childMenuUI = null;
			this.curPopupParentRect = Rectangle.Empty;
			this.curPopupLoc = Point.Empty;
			this.popupParent = null;
		}
		/// <summary>
		/// Serves to get the border overlap cue.
		/// </summary>
		/// <param name="rAlignment"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public Point[] GetBorderOverlapCue( PopupRelativeAlignment rAlignment )
		{
			if( this.popupParent != null )
				return this.popupParent.GetBorderOverlapCue( rAlignment );

			if( this.curPopupParentRect == Rectangle.Empty )
				return null;
			else
				return PopupUtils.ComputeDefaultBorderOverlapCue( rAlignment, this.curPopupParentRect );
		}
		/// <summary>
		/// Serves to get the location for pop-up alignment.
		/// </summary>
		/// <param name="prevAlign"></param>
		/// <param name="newAlign"></param>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		public Point GetLocationForPopupAlignment( PopupRelativeAlignment prevAlign, out PopupRelativeAlignment newAlign )
		{
			Point pos = Point.Empty;

			if( this.popupParent != null )
			{
				pos = this.popupParent.GetLocationForPopupAlignment( prevAlign, out newAlign );
			}
			else
			{
				bool bRTL = ( this as IPopupParent ).IsRightToLeft;

				if( this.curPopupParentRect.IsEmpty )
				{
					newAlign = bRTL ? PopupRelativeAlignment.BottomRight : PopupRelativeAlignment.Default;
					pos = this.curPopupLoc;
				}
				else
				{
					pos = PopupUtils.ComputeDefaultTopBottomAlignment( prevAlign, out newAlign,
						this.curPopupParentRect, bRTL );
				}
			}

			return pos;
		}
		/// <summary>
		/// Processes mouse messages.
		/// </summary>
		/// <param name="hWnd"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		private bool ProcessMouseMessage( IntPtr hWnd, IntPtr wParam, IntPtr lParam )
		{
			Control control = Control.FromHandle( hWnd );

			if( PopupManager.ActivePopupClient == null ||
				!PopupManager.ActivePopupClient.IsRelatedControl( control, true ) )
			{
				if( this.IsShowing() )
				{
					MenuGrid grid = this.childMenuUI as MenuGrid;
					if( grid != null )
					{
						grid.IsMouseMessage = true;
					}

					this.Hide();

					if( grid != null )
					{
						grid.IsMouseMessage = false;
					}

					return true;
				}
			}

			return false;
		}
		#endregion

		#region Events
		/// <summary>
		/// Fired when the ParentBarItem property changes.
		/// </summary>
		[Description( "Occurs when the ParentBarItem property changes." )]
		public event EventHandler ParentBarItemChanged;
		/// <summary>
		/// Fired right before the popup menu gets displayed with the position.
		/// </summary>
		/// <remarks>
		/// This event gets fired before the underlying <see cref="ParentBarItem"/>'s <b>BeforePopup</b>
		/// event gets fired. The difference is that this event also provides the mouse position
		/// of the context menu. Both events are cancellable.
		/// </remarks>
		[Description( "Occurs before the popupmenu gets displayed." )]
		public event CancelMouseEventHandler BeforePopup;
		/// <summary>
		/// Occurs when menu is popped up.
		/// </summary>
        [Description("Occurs when menu is popped up.")]
		public event EventHandler Popup;
		/// <summary>
		/// Occurs when menu is collapsed.
		/// </summary>
        [Description("Occurs when menu is collapsed.")]
		public event EventHandler Collapse;
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual Control GetParentControl()
		{
			return this.menuParentControl;
		}
		/// <summary>
		/// Raises the BeforePopup event.
		/// </summary>
		/// <param name="e">A CancelMouseEventArgs that contains the event data.</param>
		/// <remarks>
		/// The OnBeforePopup method also allows derived classes to handle the event 
		/// without attaching a delegate. This is the preferred technique for 
		/// handling the event in a derived class. 
		/// <para>Notes to Inheritors:  When overriding OnBeforePopup in a derived 
		/// class, be sure to call the base class's OnBeforePopup method so that 
		/// registered delegates receive the event.</para>
		/// </remarks>
		protected virtual void OnBeforePopup( CancelMouseEventArgs e )
		{
			if( this.BeforePopup != null )
				this.BeforePopup( this, e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		internal bool ShouldHidePopupOnDeactivate()
		{
			bool hide = true;

			if( this.childMenuUI != null )
			{
				MenuGrid grid = this.childMenuUI as MenuGrid;
				if( grid != null && grid.HighlightRange != GridRangeInfo.Empty )
				{
					BarItem itemClicked = grid.ParentItem.Items[ grid.HighlightRange.Top - 1 ] as BarItem;
					if(grid.IsItemDropDownStyle( itemClicked )||itemClicked is DropDownBarItem )
						hide = false;
				}
			}

			return hide;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="pcc"></param>
		/// <returns></returns>
		internal IPopupChild ShowPopupMenuWithPCCAsParent( Point pos, PopupControlContainer pcc )
		{
			if( this.parentBarItem == null )
				return null;

			// The menuGrid will release itself based on settings
			MenuGrid menuGrid = XPMenuGridFactory.GetMenuGridToDeploy();

			pcc.CurrentPopupChild = menuGrid;

			//Show the menu
			menuGrid.Show( this.parentBarItem, pos, pcc, false );

			if( !menuGrid.IsShowing() )
			{
				this.ChildClosing( menuGrid, PopupCloseType.Canceled );
				return null;
			}

			return menuGrid;
		}
		#endregion

		#region IPopupParent Members
		/// <summary>
		/// 
		/// </summary>
		bool IPopupParent.IsRightToLeft
		{
			get
			{
				bool bRTL = false;

				if( null != this.popupParent )
				{
					bRTL = this.popupParent.IsRightToLeft;
				}
				else if( null != this.SourceControl )
				{
					bRTL = ( RightToLeft.Yes == this.SourceControl.RightToLeft );
				}

				return bRTL;
			}
		}
		#endregion

		#region IMessageFilter Members
		/// <summary>
		/// Pre processing mouse messages.
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		[DocumentationExclude()]
		public bool PreFilterMessage( ref Message m )
		{
			if( m.Msg == NativeMethods.WM_LBUTTONDOWN || m.Msg == NativeMethods.WM_RBUTTONDOWN )
			{
				bool processed = ProcessMouseMessage( m.HWnd, m.WParam, m.LParam );

				return processed;
			}

			return false;
		}
		#endregion
	}
	#endregion

	#region *** ContextMenuPlaceHolder
	/// <summary>
	/// Serves to listen to the command keys and popup events.
	/// </summary>
	[ToolboxItem( false )]
	public class ContextMenuPlaceHolder
		: ContextMenu
	{
		#region Fields
		/// <summary>
		/// manager != null means the PopupMenusManager is using this instance.
		/// </summary>
		private PopupMenusManager manager;
		private Control control;
		/// <summary>
		/// form and control above will be the same instance.
		/// form != null means the BarManager is using this instance.
		/// </summary>
		private Form form;
		/// <summary>
		/// When this is != null the tabbedMDIManager is expecting the ProcessCmdKey calls
		/// </summary>
		private ITabbedMDIManager tabbedMDIManager;
		/// <summary>
		/// 
		/// </summary>
		private static bool IsNT4 = false;
		/// <summary>
		/// The position where the context menu should be shown.
		/// </summary>
		private Point cachedMousePoisiton = Point.Empty;
		private ArrayList m_arrToolBars = new ArrayList();
		#endregion

		#region Properties
		/// <summary>
        /// Gets the PopMenusManager
        /// </summary>
        public PopupMenusManager Manager
        {
            get
            {
                return manager;
            }
        }
		
        /// <summary>
		/// Gets or sets the main menu form.
		/// </summary>
		public Form MainMenuForm
		{
			get { return this.form; }
			set
			{
				if( this.form != value )
				{
					if( this.form != null && value != null )
						throw new ArgumentException( "Reusing ContextMenuPlaceHolder for different Forms. Improper usage, please contact vendor." );

					this.form = value;

					if( this.form != null )
					{
						this.control = this.form;
						this.control.ContextMenu = this;
					}
					else if( this.IsFreeOfReferences() )
						this.Dispose();
				}
			}
		}
		/// <summary>
		/// Gets or sets the tabbed MDI manager.
		/// </summary>
		public ITabbedMDIManager TabbedMDIManager
		{
			get { return this.tabbedMDIManager; }
			set
			{
				if( this.tabbedMDIManager != value )
				{
					if( this.tabbedMDIManager != null && value != null )
						throw new ArgumentException( "Reusing ContextMenuPlaceHolder for different TabbedMDIManager. Improper usage, please contact vendor." );

					this.tabbedMDIManager = value;

					if( this.tabbedMDIManager != null )
					{
						this.control = this.tabbedMDIManager.GetMDIParent();
						this.control.ContextMenu = this;
					}
					else if( this.IsFreeOfReferences() )
						this.Dispose();
				}
			}
		}
		/// <summary>
		/// Adds the tool bar to an array list.
		/// </summary>
		public void AddToolBar( XPToolBar toolBar )
		{
			if( toolBar == null )
				throw new ArgumentNullException( "toolBar" );

			if( !m_arrToolBars.Contains( toolBar ) )
			{
				m_arrToolBars.Add( toolBar );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="toolBar"></param>
		public void RemoveToolBar( XPToolBar toolBar )
		{
			if( toolBar == null )
				throw new ArgumentNullException( "toolBar" );

			if( m_arrToolBars.Contains( toolBar ) )
			{
				m_arrToolBars.Remove( toolBar );
			}
		}
		#endregion

		#region Initialization And Finalization
		/// <summary>
		/// 
		/// </summary>
		static ContextMenuPlaceHolder()
		{
			IsNT4 = Environment.OSVersion.Platform == PlatformID.Win32NT &&
				Environment.OSVersion.Version.Major <= 4;
		}
		/// <summary>
		/// 
		/// </summary>
		public ContextMenuPlaceHolder()
		{
		}
		/// <summary>
		/// Disposes of the resources, other than memory, used by the <see cref="T:System.Windows.Forms.Menu"></see>.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( this.control != null && this.control.ContextMenu == this )
					this.control.ContextMenu = null;

				this.control = null;
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="manager"></param>
		/// <param name="control"></param>
		public void InitContextMenuSettings( PopupMenusManager manager, Control control )
		{
			this.manager = manager;
			this.control = control;
			this.control.ContextMenu = this;
		}
		/// <summary>
		/// 
		/// </summary>
		public void ReleaseContextMenuSettings()
		{
			this.manager = null;
			if( this.IsFreeOfReferences() )
				this.Dispose();
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns></returns>
		protected override bool ProcessCmdKey( ref System.Windows.Forms.Message msg, Keys keyData )
		{
			bool processed = false;
			if( this.manager != null )
			{
				PopupMenu pm = this.manager.GetXPContextMenu( this.control );
				// Check whether the parent is visible
				if( pm != null )
				{
					if( ( ( keyData & Keys.F10 ) == Keys.F10 ) &&
							( ( Control.ModifierKeys & Keys.Shift ) == Keys.Shift ) &&
							( ( keyData & ~Keys.F10 ) == ( Keys.ShiftKey | Keys.Shift ) ) &&
							!pm.IsShowing() )
					{
						bool old = pm.SynchronousPopup;
						pm.SynchronousPopup = false;
						this.ShowPopupMenu();
						pm.SynchronousPopup = old;

						processed = true;
					}
					else if( this.control.Visible )
					{
						// Check if we need to do this: keys |= Control.ModifierKeys;
						processed = pm.ProcessShortcut( keyData, this.control );
					}
				}
			}
			if( !processed && this.MainMenuForm != null )
			{
				BarManager bm = BarManager.GetManagerFromForm( this.MainMenuForm );
				if( bm != null )
					processed = bm.ProcessCmdKey( ref msg, keyData );
			}
			if( !processed && this.TabbedMDIManager != null )
			{
				processed = this.TabbedMDIManager.ProcessCmdKey( ref msg, keyData );
			}

			if( !processed && m_arrToolBars.Count > 0 )
			{
				foreach( XPToolBar toolBar in m_arrToolBars )
				{
					toolBar.ProcessShortcut( keyData );
				}
			}

			if( !processed )
			{
				processed = base.ProcessCmdKey( ref msg, keyData );
			}

			return processed;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPopup( EventArgs e )
		{
			// manager will be null when this is a "MainMenuForm context menu".
			if( this.manager != null )
			{
				// Buggy RichTextBox doesn't fire the WM_UNINITMENUPOPUP message!
				PopupMenu popupMenu = this.manager.GetXPContextMenu( this.control );
				if( popupMenu != null )
				{
					bool old = popupMenu.SynchronousPopup;
					popupMenu.SynchronousPopup = false;
					this.ShowPopupMenu();
					popupMenu.SynchronousPopup = old;
				}
			}
            else if (this.MainMenuForm != null && this.MainMenuForm.ContextMenuStrip != null)
            {
                this.MainMenuForm.ContextMenuStrip.Show(Cursor.Position);
            }
		}
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool IsFreeOfReferences()
		{
			return this.form == null && this.manager == null && this.tabbedMDIManager == null && this.m_arrToolBars.Count == 0;
		}
		/// <summary>
		/// Displays the popup menu.
		/// </summary>
		private void ShowPopupMenu()
		{
			if( this.manager != null )
			{
				if( this.SourceControl == null )
					this.manager.OnContextMenu( this.control, this.GetLocation( this.control )/*, false*/);
				else if( this.control == this.SourceControl )
					this.manager.OnContextMenu( this.SourceControl, this.GetLocation( this.SourceControl )/*, false*/);
				else
				{
					PopupMenu popup = null;
					bool bShouldShowMenu = this.IsChildControl( this.SourceControl, this.control );

					if( bShouldShowMenu )
					{
						// this could happen for this.control's child controls (typically, like in NumericUpDown)
						popup = this.manager.GetXPContextMenu( this.control );
					}
					else
					{
						popup = this.manager.GetXPContextMenu( this.SourceControl );

						bShouldShowMenu = ( null != popup );
					}

					if( bShouldShowMenu )
					{
						this.manager.OnContextMenu( this.SourceControl, this.GetLocation( this.SourceControl ), popup );
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="childControl"></param>
		/// <param name="parentControl"></param>
		/// <returns></returns>
		private bool IsChildControl( Control childControl, Control parentControl )
		{
			if( childControl == null )
				throw new ArgumentNullException( "childControl" );

			if( parentControl == null )
				throw new ArgumentNullException( "parentControl" );

			bool isChild = false;
			Control parent = childControl.Parent;

			while( parent != null )
			{
				if( parent == parentControl )
				{
					isChild = true;
					break;
				}

				parent = parent.Parent;
			}

			return isChild;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		private Point GetLocation( Control control )
		{
			// If the menu was invoked via a Shift+F10
			bool menuKeyDown =
				Syncfusion.Runtime.InteropServices.NativeMethods.GetKeyState( ( int )Keys.ShiftKey ) < 0;

			menuKeyDown &=
				Syncfusion.Runtime.InteropServices.NativeMethods.GetKeyState( ( int )Keys.F10 ) < 0;

			if( menuKeyDown )
			{
				if( control is IProvideCustomContextMenuPositionalInformation )
				{
					IProvideCustomContextMenuPositionalInformation customInfo = control as IProvideCustomContextMenuPositionalInformation;
					return customInfo.GetMenuPositionForKeyboardInvoke();
				}
				else if( control is TreeView )
				{
					TreeView tv = control as TreeView;
					if( tv.SelectedNode != null )
					{
						Point ptx = new Point( tv.SelectedNode.Bounds.X, tv.SelectedNode.Bounds.Y + tv.SelectedNode.Bounds.Height / 2 );
						return ptx;
					}
				}
			}
			// By default, use the cursor position.
			Point pt = control.PointToClient( Control.MousePosition );
			if( !control.ClientRectangle.Contains( pt ) )
			{
				pt = new Point( control.ClientRectangle.Width / 2, control.ClientRectangle.Height / 2 );
			}
			return pt;
		}
		#endregion
	}
	#endregion
}
