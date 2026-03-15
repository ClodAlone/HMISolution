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
using System.Drawing;
using System.Drawing.Text;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Resources;
using System.ComponentModel.Design;
using Syncfusion.Drawing;
using System.Drawing.Drawing2D;
using Syncfusion.Runtime.InteropServices;
using System.Drawing.Design;

namespace Syncfusion.Windows.Forms.Tools
{

	// Form based class that provides the drawing surface for displaying a customized 
	// ToolTip with an icon. Used in the SmallIcon - FullItemSelect - ButtonView mode.
	[
	DesignTimeVisible( false ),
	ToolboxItem( false ),
	Syncfusion.Documentation.DocumentationExclude()
	]
	public class ToolTipForm: Form
	{
		#region Internal Classes
		/// <summary>
		/// Class for subclassing parent form and prevent it from blinking.
		/// </summary>
		internal class ParentFormSubClass
			: NativeWindow
		{
			public bool bCatchMessage = false;
			/// <summary>
			/// Catches WM_NCACTIVATE message if needed.
			/// </summary>
			/// <param name="m"></param>
			protected override void WndProc( ref Message m )
			{
				if( m.Msg  == NativeMethods.WM_NCACTIVATE && bCatchMessage )
				{
					m.Result = IntPtr.Zero;
					return;
				}

				base.WndProc( ref m );
			}
		}
		#endregion

		/// <summary>
		/// Specifies whether OnLoad was called.
		/// </summary>
		private bool m_bOnloadCalled = false;

		/// <summary>
		/// Specifies whether form is visible.
		/// </summary>
		private bool m_bVisible = false;

		private ParentFormSubClass m_parentSubClass = new ParentFormSubClass();
		private GroupView ctrlHost = null;

		public ToolTipForm( GroupView ctrl )
		{
			this.ctrlHost = ctrl;
			this.SetStyle( ControlStyles.Selectable, false );
			this.TabStop = false;
			this.BackColor = ctrl.BackColor;
			this.FormBorderStyle = FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ShowInTaskbar = false;
			//this.TopMost = true;
			this.Owner = this.ParentForm;
			this.Visible = false;
		}

		/// <summary>
		/// Indicates whether form must be inactive.
		/// </summary>
		private bool m_bInactive = true;

		protected override void WndProc( ref Message m )
		{
			if( m.Msg == NativeMethods.WM_MOUSEACTIVATE && m_bInactive )
			{
				m.Result = (IntPtr)3 /*MA_NOACTIVATE*/;
				return;
			}

			base.WndProc( ref m );
		}

		protected override void OnClosed( EventArgs e )
		{
			base.OnClosed( e );

			if( IntPtr.Zero != m_parentSubClass.Handle )
			{
				m_parentSubClass.bCatchMessage = false;
			}

			m_bVisible = false;
		}

		/// <summary>
		/// Gets or sets bool specifying whether form is visible.
		/// </summary>
		public new bool Visible
		{
			get
			{
				return m_bVisible;
			}
			set
			{
				if( m_bInactive )
				{
					SetVisibleCore( value );
				}
				else
				{
					base.SetVisibleCore( value );
					m_bVisible = value;
				}
			}
		}

		protected override void SetVisibleCore( bool value )
		{
			if( m_bVisible == value ) return;

			if( value )
			{
				if( !m_bOnloadCalled )
				{
					this.OnLoad( EventArgs.Empty );
					m_bOnloadCalled = true;
				}

				if( null != ctrlHost.TopLevelControl )
				{
					if( IntPtr.Zero == m_parentSubClass.Handle )
					{
						m_parentSubClass.AssignHandle( ctrlHost.TopLevelControl.Handle );
					}

					m_parentSubClass.bCatchMessage = true;
				}
			}
			else
			{
				if( IntPtr.Zero != m_parentSubClass.Handle )
				{
					m_parentSubClass.bCatchMessage = false;
				}
			}

			if( ( this.IsHandleCreated && this.Handle != IntPtr.Zero ) || value )
			{
				// show_nactivate = 8, 0 = hide
				NativeMethods.ShowWindow( this.Handle, value ? 8 : 0 );
			}

			if( m_bVisible != value )
			{
				this.OnVisibleChanged( EventArgs.Empty );

				if( this.Parent != null )
				{
					this.Parent.PerformLayout( this, "Visible" );
				}
			}

			m_bVisible = value;
		}


		protected override void OnMouseDown( MouseEventArgs e )
		{
			base.OnMouseDown( e );

			this.Visible = false;
			Point ptclient = this.ctrlHost.PointToClient( this.PointToScreen( new Point( e.X, e.Y ) ) );
			if( e.Button == MouseButtons.Left )
			{
				Syncfusion.Runtime.InteropServices.NativeMethods.SendMessage( this.ctrlHost.Handle, 0x0201/*WM_LBUTTONDOWN*/,
					(IntPtr)0x0001/*MK_LBUTTON*/, (IntPtr)Syncfusion.Runtime.InteropServices.NativeMethods.MAKELPARAM( ptclient.X, ptclient.Y ) );
			}
			else if( e.Button == MouseButtons.Right )
			{
				Syncfusion.Runtime.InteropServices.NativeMethods.SendMessage( this.ctrlHost.Handle, 0x0204/*WM_RBUTTONDOWN*/,
					(IntPtr)0x0002/*MK_RBUTTON*/, (IntPtr)Syncfusion.Runtime.InteropServices.NativeMethods.MAKELPARAM( ptclient.X, ptclient.Y ) );
			}

			this.ctrlHost.ToolTipLastMouseDownedTime = DateTime.Now;
			this.ctrlHost.ToolTipMouseButtonsDowned = e.Button;
			this.ctrlHost.ToolTipLastMouseDownedPoint = ptclient;
		}

		protected override void OnMouseUp( MouseEventArgs e )
		{
			base.OnMouseUp( e );

			Point ptclient = this.ctrlHost.PointToClient( this.PointToScreen( new Point( e.X, e.Y ) ) );
			if( e.Button == MouseButtons.Left )
			{
				Syncfusion.Runtime.InteropServices.NativeMethods.SendMessage( this.ctrlHost.Handle, 0x0202/*WM_LBUTTONUP*/,
					(IntPtr)0x0001/*MK_LBUTTON*/, (IntPtr)Syncfusion.Runtime.InteropServices.NativeMethods.MAKELPARAM( ptclient.X, ptclient.Y ) );
			}
			else if( e.Button == MouseButtons.Right )
			{
				Syncfusion.Runtime.InteropServices.NativeMethods.SendMessage( this.ctrlHost.Handle, 0x0205/*WM_RBUTTONUP*/,
					(IntPtr)0x0002/*MK_RBUTTON*/, (IntPtr)Syncfusion.Runtime.InteropServices.NativeMethods.MAKELPARAM( ptclient.X, ptclient.Y ) );
			}
		}

		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			Point ptscreen = this.PointToScreen( new Point( e.X, e.Y ) );
			if( this.ctrlHost.RectangleToScreen( this.ctrlHost.ClientRectangle ).Contains( ptscreen ) == false )
			{
				this.Visible = false;
				this.ctrlHost.nHighlightedItem = -1;
			}
		}

		protected override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );

			if( this.RectangleToScreen( this.ClientRectangle ).Contains( Cursor.Position ) == false )
			{
				this.Visible = false;
				this.ctrlHost.nHighlightedItem = -1;
			}
		}

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			if( this.ctrlHost != null )
			{
				this.ctrlHost.DrawItemHighlight( e.Graphics, this.ctrlHost.nHighlightedItem,
					new Rectangle( 0, 0, this.Width-1, this.Height-1 ), GroupView.ItemState.Highlight );
			}
		}
	}

	/// <summary>
	/// Represents an item in the <see cref="GroupView"/> control.
	/// </summary>
	/// <remarks>
	/// Each item in a GroupView control is an instance of the GroupViewItem type. The 
	/// collection of GroupViewItems in the control can be accessed through the 
	/// <see cref="Syncfusion.Windows.Forms.Tools.GroupView.GroupViewItems"/> property.
	/// </remarks>
	[
	TypeConverter( typeof( Syncfusion.Windows.Forms.Tools.Design.GroupViewItemConverter ) ),
	ToolboxItem( false ),
	DefaultProperty( "Text" )
	]
	public class GroupViewItem
	{
		private string m_tooltipText = string.Empty;
		/// <summary>
		/// Gets / sets the <see cref="GroupViewItem"/>'s tooltiptext.
		/// </summary>
		/// <value>A string value.</value>
		[Localizable( true )]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public string ToolTipText
		{
			get
			{
				return m_tooltipText;
			}
			set
			{
				if( m_tooltipText != value )
				{
					m_tooltipText = value;
					this.OnPropertyChanged( new PropertyChangedEventArgs( "ToolTipText" ) );
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected String strText = String.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nImage = -1;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bEnabled = true;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected object objTag = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal GroupView groupViewCtrl = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		public const String GroupViewItemName = "GroupViewItem";

		/// <summary>
		/// Indicates whether the <see cref="GroupViewItem"/> is visible.
		/// </summary>
		private bool m_bVisible =  true;

		/// <summary>
		/// Item bounds.
		/// </summary>
		private Rectangle m_bounds = Rectangle.Empty;

		/// <summary>
		/// Gets or sets item bounds.
		/// </summary>
		internal Rectangle Bounds
		{
			get
			{
				return m_bounds;
			}
			set
			{
                
				m_bounds = value;
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="GroupViewItem"/> is visible.
		/// </summary>
		/// <value>False if the item is hidden. The default is True.</value>
		[
		Category( "Behavior" ),
		DefaultValue( true ),
		Description( "Indicates whether the GroupViewItem is visible." )
		]
		public bool Visible
		{
			get
			{
				return m_bVisible;
			}
			set
			{
				if( value != m_bVisible )
				{
					m_bVisible = value;
					PropertyChangedEventArgs e = new PropertyChangedEventArgs( GroupView.DEF_PROPERTY_NAME_VISIBLE );
					OnPropertyChanged( e );
				}
			}
		}

		[
		Browsable( false ),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Gets / sets the <see cref="GroupViewItem"/>'s text.
		/// </summary>
		/// <value>A String value.</value>
		[Localizable( true )]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public String Text
		{
			get { return this.strText; }
			set
			{
				if( this.strText != value )
				{
					this.strText = value;
					this.OnPropertyChanged( new PropertyChangedEventArgs( "Text" ) );
				}
			}
		}

		/// <summary>
		/// Gets / sets the <see cref="GroupViewItem"/>'s image index.
		/// </summary>
		/// <value>An integer value.</value>
		/// <remarks> A zero-based index into the <see cref="GroupView"/> control's 
		/// <see cref="Syncfusion.Windows.Forms.Tools.GroupView.LargeImageList"/> and <see cref="Syncfusion.Windows.Forms.Tools.GroupView.SmallImageList"/> property values.
		/// </remarks>
		[Localizable( true )]
		public int ImageIndex
		{
			get { return this.nImage; }
			set
			{
				if( this.nImage != value )
				{
					this.nImage = value;
					this.OnPropertyChanged( new PropertyChangedEventArgs( "ImageIndex" ) );
				}
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="GroupViewItem"/> is enabled / disabled.
		/// </summary>
		/// <value>False if the item is disabled. The default is True.</value>		
		[Localizable( true )]
		public bool Enabled
		{
			get { return this.bEnabled; }
			set
			{
				if( this.bEnabled != value )
				{
					this.bEnabled = value;
					this.OnPropertyChanged( new PropertyChangedEventArgs( "Enabled" ) );
				}
			}
		}

		/// <summary>
		/// Gets / sets an object that contains data about the <see cref="GroupViewItem"/>.
		/// </summary>
		/// <value>
		/// An <see cref="System.Object"/> value that contains data about the GroupViewItem. 
		/// The default is a NULL reference (Nothing in Visual Basic).
		/// </value>
		/// <remarks>
		/// Any type derived from the Object class can be assigned 
		/// to this property. If the Tag property is set through 
		/// the Windows Forms designer, only text may be assigned.
		/// </remarks>
		[
		DefaultValue( null ),
		TypeConverter( typeof( System.ComponentModel.StringConverter ) ),
		Description( "Gets / sets the object that contains data about the GroupViewItem." ),
		Category( "Data" )
		]
		public object Tag
		{
			get { return this.objTag; }
			set { this.objTag = value; }
		}

		/// <summary>
		/// Returns the GroupView control that the item is assigned to.
		/// </summary>
		/// <value>
		/// A <see cref="GroupView"/> that represents the parent GroupView control that the <see cref="GroupViewItem"/> is assigned to.
		/// </value>
		[
		Browsable( false )
		]
		public GroupView GroupView
		{
			get { return this.groupViewCtrl; }
		}

		/// <summary>
		/// Overloaded. Creates an instance of the <see cref="GroupViewItem"/> class.
		/// </summary>
		public GroupViewItem()
		{
			this.strText = GroupViewItem.GroupViewItemName;
			m_tooltipText = GroupViewItem.GroupViewItemName;
		}

		/// <summary>
		/// Creates an instance of the <see cref="GroupViewItem"/> class with the specified attributes.
		/// </summary>
		/// <param name="name">A String value representing the GroupViewItem text.</param>
		/// <param name="image">An integer value representing a zero-based index into the <see cref="GroupView"/> 
		/// control's small and large imagelists.</param>
		public GroupViewItem( String name, int image )
		{
			this.strText = name;
			this.nImage = image;
			m_tooltipText = name;
		}

		/// <summary>
		/// Creates an instance of the <see cref="GroupViewItem"/> class with the specified attributes.
		/// </summary>
		/// <param name="name">A String value representing the GroupViewItem text.</param>
		/// <param name="image">An integer value representing a zero-based index into the <see cref="GroupView"/> 
		/// control's small and large imagelists.</param>
		/// <param name="enabled">A boolean value representing the item's enabled / disabled state.</param>
		public GroupViewItem( String name, int image, bool enabled )
		{
			this.strText = name;
			this.nImage = image;
			this.bEnabled = enabled;
			m_tooltipText = name;
		}

		/// <summary>
		/// Creates an instance of the <see cref="GroupViewItem"/> class with the specified attributes.
		/// </summary>
		/// <param name="name">A String value representing the GroupViewItem text.</param>
		/// <param name="image">An integer value representing a zero-based index into the <see cref="GroupView"/> 
		/// control's small and large imagelists.</param>
		/// <param name="tagObject">An Object value that contains data about the GroupViewItem.</param>
		public GroupViewItem( String name, int image, Object tagObject )
		{
			this.strText = name;
			this.nImage = image;
			this.objTag = tagObject;
			m_tooltipText = name;
		}

		/// <summary>
		/// Creates an instance of the <see cref="GroupViewItem"/> class with the specified attributes.
		/// </summary>
		/// <param name="name">A String value representing the GroupViewItem text.</param>
		/// <param name="image">An integer value representing a zero-based index into the <see cref="GroupView"/> 
		/// control's small and large imagelists.</param>
		/// <param name="enabled">A boolean value representing the item's enabled / disabled state.</param>
		/// <param name="tagObject">An Object value that contains data about the GroupViewItem.</param>
		public GroupViewItem( String name, int image, bool enabled, Object tagObject )
		{
			this.strText = name;
			this.nImage = image;
			this.bEnabled = enabled;
			this.objTag = tagObject;
			m_tooltipText = name;
		}
		/// <summary>
		/// Creates an instance of the <see cref="GroupViewItem"/> class with the specified attributes.
		/// </summary>
		/// <param name="name">A String value representing the GroupViewItem text.</param>
		/// <param name="image">An integer value representing a zero-based index into the <see cref="GroupView"/> 
		/// control's small and large imagelists.</param>
		/// <param name="enabled">A boolean value representing the item's enabled / disabled state.</param>
		/// <param name="tagObject">An Object value that contains data about the GroupViewItem.</param>
		/// <param name="tooltipText">A String value representing the GroupViewItem tooltiptext.</param>
		public GroupViewItem( String name, int image, bool enabled, Object tagObject, string tooltipText ) :
			this( name, image, enabled, tagObject )
		{
			m_tooltipText = tooltipText;
		}
		/// <summary>
		/// Creates an instance of the <see cref="GroupViewItem"/> class with the specified attributes.
		/// </summary>
		/// <param name="name">A String value representing the GroupViewItem text.</param>
		/// <param name="image">An integer value representing a zero-based index into the <see cref="GroupView"/> 
		/// control's small and large imagelists.</param>
		/// <param name="tagObject">An Object value that contains data about the GroupViewItem.</param>
		/// <param name="bVisible">A boolean value representing the item's show/hide the GroupViewItems.</param>
		public GroupViewItem( String name, int image, bool enabled, Object tagObject, bool bVisible ) :
			this( name, image, enabled, tagObject )
		{
			m_bVisible = bVisible;
		}

		public GroupViewItem( String name, int image, bool enabled, Object tagObject, string tooltipText, bool bVisible )
		{
			this.strText = name;
			this.nImage = image;
			this.bEnabled = enabled;
			this.objTag = tagObject;
			m_tooltipText = tooltipText;
			m_bVisible = bVisible;
		}

		protected void OnPropertyChanged( PropertyChangedEventArgs args )
		{
			if( this.PropertyChanged != null )
			{
				PropertyChanged( this, args );
			}
		}
	}


	/// <summary>
	/// Represents a control that can display a list of items.
	/// </summary>
	/// <remarks>
	/// <p>
	/// The GroupView control implements a list type control that can display a set of items where 
	/// each item is represented by an image and a descriptor. Items are implemented as 
	/// instances of the <see cref="GroupViewItem"/> class. The collection of GroupViewItems in the 
	/// control can be accessed through the <see cref="GroupView.GroupViewItems"/> property that returns
	/// a reference to the <see cref="GroupView.GroupViewItemCollection"/> object maintained
	/// by the control.
	/// </p>
	/// <p>
	/// The GroupView control is capable of displaying items with large or small icons in various
	/// combinable styles such as the default selectable style, button-type selection, full-item select
	/// and an icon-only flowview mode. All styles are available in the regular 3D or a FlatLook mode.
	/// The control also implements an IntegratedScrolling option that allows scrolling to 
	/// be delegated to its parent container.
	/// </p>
	/// <p>
	/// The GroupView control can be used in conjunction with the Essential Tools <see cref="GroupBar"/> control
	/// to implement composite controls such as the Microsoft OutlookBar and the Visual Studio.NET toolbox window. 
	/// The latter scenario makes use of the IntegratedScrolling feature supported by the two controls.
	/// </p>
	/// </remarks>
	/// <seealso cref="GroupBar"/>
	/// <example>
	/// The sample code shows how to create a GroupView control and initialize the Behavior and Appearance properties 
	/// to resemble the Visual Studio.NET toolbox window. A handler for the GroupView.GroupViewItemSelected event 
	/// is also shown.
	/// 
	/// <coderef file="Tools\Samples\GroupBar Package\GroupViewDemo\CS\GroupViewDemoForm.cs" name="GroupView" lang="C#"><code lang="C#">
	///		private void InitializeGroupView()
	///		{
	///			// Create the GroupView control.
	///			this.gvcWinForms = new Syncfusion.Windows.Forms.Tools.GroupView();
	///
	///			// Set the large and small ImageLists.
	///			this.gvcWinForms.LargeImageList = this.ilGroupBarLarge;
	///			this.gvcWinForms.SmallImageList = this.ilGroupBarSmall;
	///			
	///			// Set the GroupView properties to display as a VS.NET tool box type window.
	///			this.gvcWinForms.SmallImageView = true;
	///			this.gvcWinForms.HighlightText = true;
	///			this.gvcWinForms.ButtonView = true;
	///			this.gvcWinForms.FlowView = false;
	///			this.gvcWinForms.FlatLook = false;
	///			this.gvcWinForms.TextWrap = false;
	///
	///			this.gvcWinForms.ImageSpacing = 2;
	///			this.gvcWinForms.ItemXSpacing = 8;
	///			this.gvcWinForms.ItemYSpacing = 1;
	///
	///			this.gvcWinForms.BackColor = SystemColors.Control;
	///			this.gvcWinForms.ForeColor = SystemColors.ControlText;
	///
	///			this.gvcWinForms.HighlightItemColor = SystemColors.Control;
	///			this.gvcWinForms.SelectingItemColor = ControlPaint.Light(SystemColors.ControlLight);
	///			this.gvcWinForms.SelectedItemColor = ControlPaint.Light(SystemColors.ControlLight);
	///			this.gvcWinForms.SelectedHighlightItemColor = SystemColors.Control;
	///
	///			this.gvcWinForms.SelectingTextColor = SystemColors.ControlText;
	///			this.gvcWinForms.SelectedHighlightTextColor = SystemColors.ControlText;																				 
	///		
	///			// Create and add the GroupViewItem objects.
	///			this.gvcWinForms.GroupViewItems.AddRange(
	///				new Syncfusion.Windows.Forms.Tools.GroupViewItem[] {
	///																	   new Syncfusion.Windows.Forms.Tools.GroupViewItem("Pointer", 11),
	///																	   new Syncfusion.Windows.Forms.Tools.GroupViewItem("Label", 12),
	///																	   new Syncfusion.Windows.Forms.Tools.GroupViewItem("LinkLabel", 13)});
	///																	   
	///			// Provide a handler for the GroupView.GroupViewItemSelected event.
	///			this.gvcWinForms.GroupViewItemSelected += new System.EventHandler(this.gvcWinForms_GroupViewItemSelected);																	   
	///		}
	///		
	///		// GroupView.GroupViewItemSelected event handler.
	///		private void gvcWinForms_GroupViewItemSelected(object sender, System.EventArgs e)
	///		{
	///			MessageBox.Show(String.Concat("Selected Item Index = ", this.gvcWinForms.SelectedItem.ToString())); 
	///		}</code></coderef>
	/// 
	/// 
	/// <coderef file="Tools\Samples\GroupBar Package\GroupViewDemo\VB\GroupViewDemoForm.vb" name="GroupView" lang="VB"><code lang="VB">
	///        Private Sub InitializeGroupView()
	///
	///				' Create the GroupView control
	///				Me.gvcWinForms = New Syncfusion.Windows.Forms.Tools.GroupView()
	///
	///				' Set the large and small ImageLists
	///				Me.gvcWinForms.LargeImageList = Me.ilGroupBarLarge
	///				Me.gvcWinForms.SmallImageList = Me.ilGroupBarSmall
	///
	///				' Set the GroupView properties to display as a VS.NET Toolbox type window
	///				Me.gvcWinForms.SmallImageView = True
	///				Me.gvcWinForms.HighlightText = True
	///				Me.gvcWinForms.ButtonView = True
	///				Me.gvcWinForms.FlowView = False
	///				Me.gvcWinForms.FlatLook = False
	///				Me.gvcWinForms.TextWrap = False
	///
	///				Me.gvcWinForms.ImageSpacing = 2
	///				Me.gvcWinForms.ItemXSpacing = 8
	///				Me.gvcWinForms.ItemYSpacing = 1
	///
	///				Me.gvcWinForms.BackColor = SystemColors.Control
	///				Me.gvcWinForms.ForeColor = SystemColors.ControlText
	///
	///				Me.gvcWinForms.HighlightItemColor = SystemColors.Control
	///				Me.gvcWinForms.SelectingItemColor = ControlPaint.Light(SystemColors.ControlLight)
	///				Me.gvcWinForms.SelectedItemColor = ControlPaint.Light(SystemColors.ControlLight)
	///				Me.gvcWinForms.SelectedHighlightItemColor = SystemColors.Control
	///
	///				Me.gvcWinForms.SelectingTextColor = SystemColors.ControlText
	///				Me.gvcWinForms.SelectedHighlightTextColor = SystemColors.ControlText																				 
	///
	///             ' Create and add the GroupViewItem objects.
	///             Me.gvcWinForms.GroupViewItems.AddRange(New Syncfusion.Windows.Forms.Tools.GroupViewItem() {New Syncfusion.Windows.Forms.Tools.GroupViewItem("Pointer", 11), New Syncfusion.Windows.Forms.Tools.GroupViewItem("Label", 12), New Syncfusion.Windows.Forms.Tools.GroupViewItem("LinkLabel", 13)})
	///
	///             ' Handle the GroupView.GroupViewItemSelected event.
	///             AddHandler Me.gvcWinForms.GroupViewItemSelected, New System.EventHandler(AddressOf gvcWinForms_GroupViewItemSelected)
	///
	///        End Sub
	///
	///        ' GroupView.GroupViewItemSelected event handler.
	///        Private Sub gvcWinForms_GroupViewItemSelected(ByVal sender As Object, ByVal e As System.EventArgs)
	///
	///             MessageBox.Show([String].Concat("Selected Item Index = ", Me.gvcWinForms.SelectedItem.ToString()))
	///
	///        End Sub 'gvcWinForms_GroupViewItemSelected</code></coderef>
	///
	/// </example>	
	[
	ToolboxBitmap( typeof( Syncfusion.Windows.Forms.PopupControlContainer ), "ToolboxIcons.groupviewcontrol.bmp" ),
	Designer( typeof( Syncfusion.Windows.Forms.Tools.Design.GroupViewDesigner ),
		typeof( System.ComponentModel.Design.IDesigner ) ),
	DefaultProperty( "GroupViewItems" ),
	DefaultEvent( "GroupViewItemSelected" ),
	Description( "Represents a list type control that can display a list of items." )
	]
	public class GroupView: Control, IIntegratedScrollClient, IGroupViewDesignerInvoke
	{
		/// <summary>
		/// Indicates whether the <see cref="Syncfusion.Windows.Forms.Tools.GroupViewItem.ToolTip"/> is enabled / disabled.
		/// </summary>
		/// <value>True if the ToolTip is enabled. The default is False.</value>	
		private bool m_bShowTooltips = false;

		/// <summary>
		/// Indicates whether the items being selected.
		/// </summary>
		private bool m_bSelecting = false;

		/// <summary>
		/// Indicates whether FlowView value was changed during renaming.
		/// </summary>
		private bool m_bFlowViewChanged = false;

		/// <summary>
		/// Gets or sets whether tooltips for GroupViewItems should be shown or not.
		/// Use <see cref="Syncfusion.Windows.Forms.Tools.GroupViewItem.ToolTip"/> property to get\set ToolTip text.
		/// </summary>
		[DefaultValue( false )]
		[Description( "Gets or sets whether tooltips for GroupViewItems should be shown or not." )]
		public bool ShowToolTips
		{
			get
			{
				return m_bShowTooltips;
			}
			set
			{
				if( value!= m_bShowTooltips )
				{
					m_bShowTooltips = value;
				}
			}
		}

		#region Tooltip implementation

		private static readonly Point DEF_TOOLTIP_OFFSET  = new Point( 13, 15 );
		private const int DEF_TOOLTIP_TIMER_INTERVAL = 200;
		private const int DEF_TOOLTIP_INITIAL_TIMER_INTERVAL = 1000;

		/// <summary>
		/// Store index of <see cref="GroupViewItem"/> which was hitted before current
		/// </summary>
		private int m_prevHitItemIndex = -1;
		/// <summary>
		/// Indicate whether <see cref="GroupViewItem"/> is under mouse pointer
		/// </summary>
		private bool m_bIsGroupViewItemSelecting = false;
		/// <summary>
		/// Indicate  whether <see cref="Syncfusion.Windows.Forms.Tools.GroupViewItem.ToolTip"/> is showing for the first time
		/// </summary>
		/// <value>False if the ToolTip isn't showing. The default is True.</value>	
		private bool m_bShouldShowToolTipFirstTime = true;

		private Timer m_toolTipTimer = null;
		private ToolTipAdv m_toolTip = null;

		/// <summary>
		/// Initialize Timer
		/// </summary>
		private void InitTimer()
		{
			m_toolTipTimer = new Timer();
			m_toolTipTimer.Tick += new EventHandler( OnToolTipTimerTick );
		}

		/// <summary>
		/// Initialize ToolTip
		/// </summary>
		private void InitToolTip()
		{
			if( m_toolTip == null )
			{
				m_toolTip = new ToolTipAdv( this );

				m_toolTip.BackColor = SystemColors.Info;
				m_toolTip.BorderStyle = BorderStyle.FixedSingle;
			}
		}

		private void OnToolTipTimerTick( object sender, EventArgs e )
		{
            if ( !this.IsDisposed )
            {
                Point showPos = this.PointToClient( Control.MousePosition );

                GroupViewItem hotItem = GetItemAt( showPos );

                // Show tooltip only when mouse is over Tab to show tooltip for
                ShowToolTip( ( hotItem == null )? null : hotItem.ToolTipText );

                m_toolTipTimer.Stop();
            }
		}

		/// <summary>
		/// Shows ToolTip
		/// </summary>
		/// <param name="text">A string value representing ToolTip's text</param>
		protected void ShowToolTip( string text )
		{
			if( m_toolTip != null )
			{
				Point showPos = this.PointToClient( Control.MousePosition );

				// Hide tooltip
				if( text == null || text == string.Empty || !this.ShowToolTips )
				{
					if( m_toolTip.Visible )
					{
						m_toolTip.HidePopup();
					}
				}

				// Show tooltip
				else
				{
					showPos.Offset( DEF_TOOLTIP_OFFSET.X, DEF_TOOLTIP_OFFSET.Y );
					m_toolTip.Text = text;
					m_toolTip.ShowPopup( PointToScreen( showPos ) );
				}
			}
		}
		/// <summary>
		/// Get <see cref="GroupViewItem"/> which is under mouse pointer
		/// </summary>
		/// <param name="pt">Represent mouse pointer </param>
		public GroupViewItem GetItemAt( Point pt )
		{
			GroupViewItem foundItem = null;
			GroupViewItem item = null;

			if( m_arrVisibleItems != null && m_arrVisibleItems.Count > 0 )
			{
				for( int i = m_arrVisibleItems.Count - 1; i >=0; i-- )
				{
					item = (GroupViewItem)m_arrVisibleItems[i];

					if( item != null && item.Bounds.Contains( pt ) )
					{
						foundItem = item;

						//int k = GroupViewItems.IndexOf( item );

						break;
					}
				}
			}

			return foundItem;
		}


		/// <summary>
		/// Start showing tooltips
		/// </summary>
		/// <param name="interval">Representint interval before showing ToolTip</param>
		protected void StartShowingToolTip( int interval )
		{
			if( interval < 0 )
			{
				throw new ArgumentException( "interval" );
			}

			if( m_toolTipTimer == null )
			{
				InitTimer();
			}

			if( m_toolTipTimer != null )
			{
				m_toolTipTimer.Interval = interval;
				m_toolTipTimer.Start();
			}
		}

		/// <summary>
		/// Stops showing tooltips
		/// </summary>
		protected void StopShowingToolTip()
		{
			if( m_toolTipTimer != null )
			{
				ShowToolTip( null );
				m_bShouldShowToolTipFirstTime = !m_bIsGroupViewItemSelecting;
				m_toolTipTimer.Interval = DEF_TOOLTIP_INITIAL_TIMER_INTERVAL;
				m_toolTipTimer.Stop();
			}
		}

		#endregion
        #region For Touch

        bool isScaling = false;
        /// <summary>
        /// Gets/Sets Control size before touch enabled
        /// </summary>
        [Browsable(false)]
        public Size BeforeTouchSize
        {
            get
            {
                return CTRLSIZE;
            }
            set
            {
                CTRLSIZE = value;
            }
        }
        bool _touchMode = false;
        /// <summary>
        ///Gets or Sets the touchmode
        /// </summary>
        [DefaultValue(false)]
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
        ///Applies the scaling
        /// </summary>
        public void ApplyScaleToControl(float scaleFactor)
        {
            this.SuspendLayout();
            isScaling = true;
            this.Size = new Size((int)(CTRLSIZE.Width * scaleFactor), (int)(CTRLSIZE.Height * scaleFactor));
            isScaling = false;
            this.ResumeLayout();
            this.Invalidate();
        }
        /// <summary>
        ///Font changed
        /// </summary>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
        }
        #endregion

		/// <summary>
		/// ToolTip last mouse buttons downed.
		/// </summary>
		private MouseButtons m_mbToolTipMouseButtonsDowned = MouseButtons.None;
		/// <summary>
		/// ToolTip last mouse downed time.
		/// </summary>
		private DateTime m_dtToolTipLastMouseDownedTime = DateTime.MinValue;
		/// <summary>
		/// ToolTip last mouse downed point.
		/// </summary>
		private Point m_dtToolTipLastMouseDownedPoint = Point.Empty;

		/// <summary>
		/// Indicates whether control should show GroupViewItem text in FlowView mode.
		/// </summary>
		private bool m_bShowFlowViewText = false;

		/// <summary>
		/// Stores <see cref="GroupViewItem.Text"/> length in FlowView mode.
		/// </summary>
		private int m_flowViewTextLength = 80;

		/// <summary>
		/// GroupViev orientation.
		/// </summary>
		private GroupViewOrientation m_orientation = GroupViewOrientation.Vertical;

		/// <summary>
		/// Gets or sets GroupViev orientation.
		/// </summary>
		[DefaultValue( GroupViewOrientation.Vertical )]
		[Category( "Appearance" )]
		[Description( "Gets or sets GroupViev orientation." )]
		public GroupViewOrientation Orientation
		{
			get
			{
				return m_orientation;
			}
			set
			{
				if( m_orientation != value )
				{
					m_orientation = value;
					CalculateItemsPerRow();
					this.Invalidate();
				}
			}
		}

		private bool Horizontal
		{
			get
			{
				return ( m_orientation == GroupViewOrientation.Horizontal );
			}
		}

		/// <summary>
		/// Gets or sets ToolTip last mouse buttons downed.
		/// </summary>
		internal MouseButtons ToolTipMouseButtonsDowned
		{
			get
			{
				return m_mbToolTipMouseButtonsDowned;
			}
			set
			{
				m_mbToolTipMouseButtonsDowned = value;
			}
		}

		/// <summary>
		/// Gets or sets ToolTip last mouse downed time.
		/// </summary>
		internal DateTime ToolTipLastMouseDownedTime
		{
			get
			{
				return m_dtToolTipLastMouseDownedTime;
			}
			set
			{
				m_dtToolTipLastMouseDownedTime = value;
			}
		}

		/// <summary>
		/// Gets or sets ToolTip last mouse downed point.
		/// </summary>
		internal Point ToolTipLastMouseDownedPoint
		{
			get
			{
				return m_dtToolTipLastMouseDownedPoint;
			}
			set
			{
				m_dtToolTipLastMouseDownedPoint = value;
			}
		}

		protected new virtual void Layout()
		{
			m_itemTextHeight = -1;

			Rectangle rect = Rectangle.Empty;

			for( int i = this.nTopIndex; i < VisibleItems.Count; i++ )
			{
				GroupViewItem gvi = (GroupViewItem)VisibleItems[i];

				if( !this.bFlowView )
				{
					this.SetNonFlowItemBounds( i, ref rect );
				}
				else
				{
					this.SetFlowItemBounds( i, ref rect );
				}

				Rectangle correctRect = rect;

				if( this.Horizontal )
				{
					int temp = correctRect.Width;
					correctRect.Width = correctRect.Height;
					correctRect.Height = temp;

					temp = correctRect.X;
					correctRect.X = correctRect.Y;
					correctRect.Y = temp;

					if( this.GetIsMirrored() && !this.bFlowView )
					{
						correctRect.X += this.ClientRectangle.Width - ( correctRect.X * 2 + correctRect.Width );
					}
				}

				gvi.Bounds = correctRect;

				if( this.bFlowView == false )
					rect.Y = rect.Bottom;
			}
		}

		protected override void WndProc( ref Message m )
		{
			if(
				( m.Msg == NativeMethods.WM_LBUTTONDOWN
				&& ToolTipMouseButtonsDowned == MouseButtons.Left )
				|| ( m.Msg == NativeMethods.WM_RBUTTONDOWN
				&& ToolTipMouseButtonsDowned == MouseButtons.Right )
				)
			{
				Point mousePoint = new Point( NativeMethods.LOWORD( m.LParam ), NativeMethods.HIWORD( m.LParam ) );

				if( mousePoint == ToolTipLastMouseDownedPoint )
				{
					TimeSpan subtractTime = DateTime.Now.Subtract( ToolTipLastMouseDownedTime );
					bool needFireDoubleClick = ( subtractTime.TotalMilliseconds <= SystemInformation.DoubleClickTime );

					if( needFireDoubleClick )
					{
						ToolTipLastMouseDownedTime = DateTime.MinValue;

						if( ToolTipMouseButtonsDowned == MouseButtons.Left )
						{
							m.Msg = NativeMethods.WM_LBUTTONDBLCLK;
						}
						else if( ToolTipMouseButtonsDowned == MouseButtons.Right )
						{
							m.Msg = NativeMethods.WM_RBUTTONDBLCLK;
						}
					}
				}
			}

			base.WndProc( ref m );
		}

		static GroupView()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				new Syncfusion.Core.Licensing.LicensedComponent( typeof( GroupView ) );
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}
		}


		#region GroupViewItemCollection

		/// <summary>
		/// The collection of <see cref="GroupViewItem"/> objects present in the GroupView control.
		/// </summary>	
		/// <remarks>
		/// Each item in a <see cref="GroupView"/> control is an instance of a <see cref="GroupViewItem"/> 
		/// type and the collection is represented by the GroupViewItemCollection class. 
		/// The GroupViewItemCollection class implements the IList and ICollection interfaces.
		/// <see cref="GroupView.GroupViewItems"/>
		/// </remarks>		
		[
			Description( "The collection of GroupViewItems in the GroupView control." ),
			DefaultProperty( "Item" )
			]
		public class GroupViewItemCollection: CollectionBase
		{
			[Syncfusion.Documentation.DocumentationExclude()]
			protected GroupView gViewHost;
			[Syncfusion.Documentation.DocumentationExclude()]
			protected bool bLockUpdate = false;

			/// <summary>
			/// Occurs when a <see cref="GroupViewItemCollection"/> is changed.
			/// </summary>
			public event CollectionChangeEventHandler CollectionChanged;

			private void RaiseCollectionChanged( CollectionChangeEventArgs args )
			{
				if( CollectionChanged != null )
				{
					CollectionChanged( this, args );
				}
			}

			/// <summary>
			/// Raises the collection changed event.
			/// </summary>
			protected void OnCollectioChanged( CollectionChangeEventArgs args )
			{
				RaiseCollectionChanged( args );
			}

			/// <summary>
			/// Gets / sets a <see cref="GroupViewItem"/> in the collection.
			/// </summary>
			/// <param name="index">The zero-based index of the GroupViewItem to get or set.</param>
			public GroupViewItem this[int index]
			{
				get { return (GroupViewItem)( this.List[index] ); }

				set { Insert( index, value ); }
			}

			/// <summary>
			/// Creates a new instance of the <see cref="GroupViewItemCollection"/> class.
			/// </summary>
			/// <param name="ctrl">The <see cref="GroupView"/> control that contains this collection.</param>
			public GroupViewItemCollection( GroupView ctrl )
			{
				this.gViewHost = ctrl;
			}

			/// <summary>
			/// Adds the GroupViewItem to the collection.
			/// </summary>
			/// <param name="item">The <see cref="GroupViewItem"/> to be added.</param>
			/// <returns>The zero-based index of the new item within the collection.</returns>
			public int Add( GroupViewItem item )
			{
				return this.List.Add( item );
			}

			/// <summary>
			/// Inserts the <see cref="GroupViewItem"/> into the collection at the specified index.
			/// </summary>
			/// <param name="index">The zero-based index at which the item is to be inserted.</param>
			/// <param name="item">The <see cref="GroupViewItem"/> to be inserted.</param>
			public void Insert( int index, GroupViewItem item )
			{
				this.List.Insert( index, item );
			}

			/// <summary>
			/// Removes the GroupViewItem from the collection.
			/// </summary>
			/// <param name="item">The <see cref="GroupViewItem"/> to be removed.</param>
			public void Remove( GroupViewItem item )
			{
				if( this.List.Contains( item ) == false )
					throw new ApplicationException( "GroupViewItem does not exist." );

				int index = this.IndexOf( item );

				this.gViewHost.nHighlightedItem = -1;
				this.gViewHost.nContextMenuItem = -1;
				if( ( index < this.gViewHost.nSelectedItem ) || ( ( index == this.gViewHost.nSelectedItem ) && ( index+1 == this.Count ) ) )
					this.gViewHost.nSelectedItem--;

				this.List.Remove( item );

				item.PropertyChanged -= new PropertyChangedEventHandler( gViewHost.PropChangedHandler );

				gViewHost.Invalidate( false );
			}

			/// <summary>
			/// Adds an array of GroupViewItems to the <see cref="GroupView"/> control's <see cref="GroupView.GroupViewItems"/> collection.
			/// </summary>
			/// <param name="items">An array of <see cref="GroupViewItem"/> objects.</param>
			public void AddRange( GroupViewItem[] items )
			{
				this.bLockUpdate = true;
				foreach( GroupViewItem item in items )
					this.Add( item );
				this.bLockUpdate = false;
				this.gViewHost.Invalidate( false );
			}

			/// <summary>
			/// Indicates whether the specified GroupViewItem is present in the collection.
			/// </summary>
			/// <param name="item">The <see cref="GroupViewItem"/> to locate in the collection.</param>
			/// <returns>True if the item is present; False otherwise.</returns>
			public bool Contains( GroupViewItem item )
			{
				return this.List.Contains( item );
			}

			[
			EditorBrowsable( EditorBrowsableState.Never ),
			Syncfusion.Documentation.DocumentationExclude()
			]
			public void CopyTo( GroupViewItem[] array, int index )
			{
				this.List.CopyTo( array, index );
			}

			/// <summary>
			/// Returns the zero-based index of the GroupViewItem.
			/// </summary>
			/// <param name="item">The <see cref="GroupViewItem"/> to locate in the collection.</param>
			/// <returns>The zero-based index of the item; -1 if the item cannot be found.</returns>
			public int IndexOf( GroupViewItem item )
			{
				return this.List.IndexOf( item );
			}

			[Syncfusion.Documentation.DocumentationExclude()]
			protected override void OnInsert( int index, object value )
			{
				GroupViewItem item = value as GroupViewItem;
				if( ( this.gViewHost.DesignMode == true ) && ( item.Text == GroupViewItem.GroupViewItemName ) )
				{
					item.Text = String.Concat( "GroupViewItem", this.Count.ToString() );
					item.ToolTipText = item.Text;
				}
				base.OnInsert( index, value );
			}

			[Syncfusion.Documentation.DocumentationExclude()]
			protected override void OnInsertComplete( int index, object value )
			{
				base.OnInsertComplete( index, value );

				GroupViewItem item = value as GroupViewItem;
				item.groupViewCtrl = this.gViewHost;
				item.PropertyChanged += new PropertyChangedEventHandler( gViewHost.PropChangedHandler );

				this.gViewHost.nHighlightedItem = -1;
				this.gViewHost.nContextMenuItem = -1;
				if( this.gViewHost.DesignMode == true )
				{
					if( this.gViewHost.ButtonView == true )
						this.gViewHost.nSelectedItem = 0;
					else
						this.gViewHost.nSelectedItem = -1;
				}

				if( this.bLockUpdate == false )
					gViewHost.Invalidate( false );

				CollectionChangeEventArgs args = ( ( value as GroupViewItem ) == null ) ?
					new CollectionChangeEventArgs( CollectionChangeAction.Remove, null ) :
					new CollectionChangeEventArgs( CollectionChangeAction.Remove, value as GroupViewItem );
				OnCollectioChanged( args );
			}

			protected override void OnRemoveComplete( int index, object value )
			{
				base.OnRemoveComplete( index, value );

				CollectionChangeEventArgs args = ( ( value as GroupViewItem ) == null ) ?
					new CollectionChangeEventArgs( CollectionChangeAction.Remove, null ) :
					new CollectionChangeEventArgs( CollectionChangeAction.Remove, value as GroupViewItem );
				OnCollectioChanged( args );
			}

			protected override void OnClearComplete()
			{
				base.OnClearComplete();

				CollectionChangeEventArgs args = new CollectionChangeEventArgs( CollectionChangeAction.Remove, null );
				OnCollectioChanged( args );
			}
		}

		#endregion	//GroupViewItemCollection

		[Syncfusion.Documentation.DocumentationExclude()]
		public enum ItemState
		{
			Normal=0,
			Highlight,
			Selected,
			Selecting
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected enum DragDropInsert
		{
			Upper=0,
			Middle,
			Lower
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public enum ScrollButtonState
		{
			Normal=0,
			UpScrollHot=1,
			UpScrollPressed,
			DownScrollHot,
			DownScrollPressed
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal int nHighlightedItem = -1;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Rectangle rcHighlightedItem = Rectangle.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal int nSelectedItem = -1;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Rectangle rcSelectedItem = Rectangle.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nContextMenuItem = -1;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nPrevSelected = -1;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Rectangle rcPrevSelected = Rectangle.Empty;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nItemsPerRow = 0;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bDownScrlButton = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bUpScrlButton = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nTopIndex = 0;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Timer tmrScrolling = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ScrollButtonState scrllBtnState = ScrollButtonState.Normal;

		/// <summary>
		/// Specifies the name of the custom <see cref="System.Windows.Forms.DataFormats.Format"/> type used for <see cref="GroupViewItem"/> drag-and-drop.
		/// </summary>
		public const String GroupViewFormatName = "GroupViewItemDataFormat";

		/// <summary>
		/// Name visible property for PropertyChangedEventArgs
		/// </summary>
		internal static readonly string DEF_PROPERTY_NAME_VISIBLE = "Visible";

		/// <summary>
		/// ArrayList visible items.
		/// </summary>
		private ArrayList m_arrVisibleItems = null;

		/// <summary>
		/// Gets ArrayList visible items.
		/// </summary>
		private ArrayList VisibleItems
		{
			get
			{
				if( m_arrVisibleItems == null )
				{
					m_arrVisibleItems = new ArrayList();
				}

				return m_arrVisibleItems;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected static DataFormats.Format dfGroupViewItem = DataFormats.GetFormat( GroupView.GroupViewFormatName );

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bAllowDragDrop = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nDragItem = -1;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nDropItem = -1;
		[Syncfusion.Documentation.DocumentationExclude()]
		static protected Point ptDragStart = new Point( -1, -1 );

		// Appearance attributes.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Brush brBackGround = new SolidBrush( Control.DefaultBackColor );

		[Syncfusion.Documentation.DocumentationExclude()]
		protected Brush brHighlightItem = new SolidBrush( Control.DefaultBackColor );
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bHighlightItemSet = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Brush brSelectedItem = new SolidBrush( Control.DefaultBackColor );
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bSelectedItemSet = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Brush brSelectedHighlightItem = new SolidBrush( Control.DefaultBackColor );
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bSelectedHighlightItemSet = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Brush brSelectingItem = new SolidBrush( Control.DefaultBackColor );
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bSelectingItemSet = false;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected Color clrHighlightText = Control.DefaultForeColor;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bHighlightTextSet = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Color clrSelectedText = Control.DefaultForeColor;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bSelectedTextSet = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Color clrSelectedHighlightText = Control.DefaultForeColor;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bSelectedHighlightTextSet = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Color clrSelectingText = Control.DefaultForeColor;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bSelectingTextSet = false;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected ImageList ilLarge = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ImageList ilSmall = null;

		// GroupViewItems collection.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected GroupViewItemCollection cllnGroupViewItems = null;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected RenameTextBox ctrlTextBox = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nRenameItem = -1;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ToolTipForm ctrlToolTip = null;

		// Appearance attributes.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bSmallImageView = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bButtonView = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bIntegratedScrolling = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bFlowView = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bTextWrap = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bFlatLook = false;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bHighlightImage = true;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bHighlightText = true;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bTextUnderline = false;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bClipSelectionBounds = false;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nTextSpacing = 8;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nImageSpacing = 2;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nItemYSpacing = 5;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nItemXSpacing = 8;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected Point ptHighlightImageOffset = Point.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Point ptHighlightTextOffset = Point.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Point ptSelectedImageOffset = Point.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Point ptSelectedTextOffset = Point.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Point ptSelectedHighlightImageOffset = Point.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Point ptSelectedHighlightTextOffset = Point.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Point ptSelectingImageOffset = Point.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Point ptSelectingTextOffset = Point.Empty;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected const int nScrllBttnOffset = 21; // right offset inlcuding scroll button width

		[Syncfusion.Documentation.DocumentationExclude()]
		protected BorderStyle bdrStyle = BorderStyle.Fixed3D;
		// XP Themes Support.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bThemesEnabled = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ThemedControlDrawing tdToolbar = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ThemedControlDrawing tdScrollBar = null;

		[Syncfusion.Documentation.DocumentationExclude()]
		private const int c_nScrollButtonWidth = 16;
		[Syncfusion.Documentation.DocumentationExclude()]
		private const int c_nScrollButtonHeight = 16;
		[Syncfusion.Documentation.DocumentationExclude()]
		private const int c_nScrollButtonOffsetX = nScrllBttnOffset - c_nScrollButtonWidth;
		[Syncfusion.Documentation.DocumentationExclude()]
		private const int c_nScrollButtonOffsetY = nScrllBttnOffset - c_nScrollButtonHeight;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected readonly Size defSmallImgSize = new Size( 16, 16 );
		[Syncfusion.Documentation.DocumentationExclude()]
		protected readonly Size defLargeImgSize = new Size( 32, 32 );

		/// <summary>
		/// Occurs when a <see cref="GroupViewItem"/> in the <see cref="GroupView"/> 
		/// control is selected.
		/// </summary>
		/// <remarks>
		/// Use the <see cref="GroupView.SelectedItem"/> property to get the index of the newly 
		/// selected item.
		/// </remarks>
		[
		Description( "Event fired when an item in the GroupView control is selected." ),
		Category( "Behavior" )
		]
		public event System.EventHandler GroupViewItemSelected;

		/// <summary>
		/// Occurs when a <see cref="GroupViewItem"/> in the <see cref="GroupView"/> control is highlighted.
		/// </summary>
		/// <remarks>
		/// Use the <see cref="GroupView.HighlightedItem"/> property to get the index of the newly 
		/// selected item.
		/// </remarks>
		[
		Description( "Event fired when an item in the GroupView control is highlighted." ),
		Category( "Behavior" )
		]
		public event System.EventHandler GroupViewItemHighlighted;

		/// <summary>
		/// Occurs after the items in a <see cref="GroupView"/> control have been reordered by a 
		/// drag-and-drop operation.
		/// </summary>
		[
		Description( "Event fired after the GroupView control items have been reordered by a drag-and-drop operation." ),
		Category( "Behavior" )
		]
		public event System.EventHandler GroupViewItemsReordered;

		/// <summary>
		/// Occurs after a <see cref="GroupViewItem"/> has been renamed by an in-place edit operation. 
		/// </summary>
		/// <remarks>
		/// See <see cref="GroupItemRenamedEventArgs"/> and <see cref="GroupItemRenamedEventHandler"/>.
		/// </remarks>
		/// <seealso cref="GroupView.InplaceRenameItem"/>.
		[
		Description( "Event fired after an in-place rename operation." ),
		Category( "Behavior" )
		]
		public event GroupItemRenamedEventHandler GroupViewItemRenamed;

		/// <summary>
		/// Occurs when the right mouse button is clicked over the <see cref="GroupView"/> control.
		/// </summary>
		/// <remarks>The <see cref="GroupView.ContextMenuItem"/> property will provide the index 
		/// of the <see cref="GroupViewItem"/> over which the mouse was clicked.</remarks>
		[
		Description( "Event fired when the right mouse button is clicked over the control." ),
		Category( "Behavior" )
		]
		public event EventHandler ShowContextMenu;

        /// <summary>
        /// Default size of the control
        /// </summary>
        private Size CTRLSIZE = default(Size);

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual Rectangle UpScrlBtnRect
		{
			get
			{
				if( this.bIntegratedScrolling == false )
				{
					if( this.Horizontal )
					{
						if( this.GetIsMirrored() )
						{
							return new Rectangle( ClientRectangle.Right - nScrllBttnOffset,
								ClientRectangle.Bottom - ( c_nScrollButtonHeight + c_nScrollButtonOffsetY ),
								c_nScrollButtonWidth, c_nScrollButtonHeight );
						}
						else
						{
							return new Rectangle( c_nScrollButtonOffsetX,
								ClientRectangle.Bottom - ( c_nScrollButtonHeight + c_nScrollButtonOffsetY ),
								c_nScrollButtonWidth, c_nScrollButtonHeight );
						}
					}
					else
					{
						int nLeft = GetScrollButtonLeft();
						return new Rectangle( nLeft, ClientRectangle.Top + c_nScrollButtonOffsetY,
							c_nScrollButtonWidth, c_nScrollButtonHeight );
					}
				}
				else
					return Rectangle.Empty;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual Rectangle DownScrlBtnRect
		{
			get
			{
				if( this.bIntegratedScrolling == false )
				{
					if( this.Horizontal )
					{
						if( this.GetIsMirrored() )
						{
							return new Rectangle( c_nScrollButtonOffsetX,
								ClientRectangle.Bottom - ( c_nScrollButtonHeight + c_nScrollButtonOffsetY ),
								c_nScrollButtonWidth, c_nScrollButtonHeight );
						}
						else
						{
							return new Rectangle( ClientRectangle.Right - nScrllBttnOffset,
								ClientRectangle.Bottom - ( c_nScrollButtonHeight + c_nScrollButtonOffsetY ),
								c_nScrollButtonWidth, c_nScrollButtonHeight );
						}
					}
					else
					{
						int nLeft = GetScrollButtonLeft();
						return new Rectangle( nLeft, ClientRectangle.Bottom - ( c_nScrollButtonHeight + c_nScrollButtonOffsetY ),
							c_nScrollButtonWidth, c_nScrollButtonHeight );
					}
				}
				else
					return Rectangle.Empty;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		private int GetScrollButtonLeft()
		{
			bool bIsMirrored = GetIsMirrored();
			int nLeft = bIsMirrored ? c_nScrollButtonOffsetX : ClientRectangle.Right - nScrllBttnOffset;
			return nLeft;
		}

		/// <summary>
		/// Gets / sets the collection of <see cref="GroupViewItem"/> objects in the control.
		/// </summary>
		/// <value>An instance of the <see cref="GroupView.GroupViewItemCollection"/> type.</value>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
		Browsable( true ),
		Description( "The GroupViewItems present in the control." ),
		Category( "GroupViewItems" )
		]
		public GroupView.GroupViewItemCollection GroupViewItems
		{
			get { return this.cllnGroupViewItems; }
			set { this.cllnGroupViewItems = value; }
		}

		/// <summary>
		/// Gets or sets the height between the highlighted edge of a <see cref="GroupViewItem"/> and the image.
		/// </summary>
		/// <remarks>
		/// In <see cref="GroupView.FlowView"/> mode, the ImageSpacing value also 
		/// dictates the horizontal distance between the highlighted edge and the image.
		/// </remarks>		
		/// <value>An integer value.</value>
		[
		Description( "Gets or sets the height between the highlighted edge of a GroupViewItem and the image." ),
		Category( "Appearance - Spacing" ),
		DefaultValue( 2 ),
		Localizable( true )
		]
		public int ImageSpacing
		{
			get { return this.nImageSpacing; }

			set
			{
				if( this.nImageSpacing != value )
				{
					this.nImageSpacing = value;
					if( ( this.bFlowView == true ) && ( this.Visible == true ) && ( this.cllnGroupViewItems.Count > 0 ) )
						this.CalculateItemsPerRow();
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets / sets the distance between the <see cref="GroupViewItem"/> image and the text.
		/// </summary>
		/// <remarks>
		/// In large icon mode, this attribute represents the vertical distance between the GroupViewItem image and the text, 
		/// while in small icon mode, it represents the horizontal distance between the two.
		/// </remarks>
		/// <value>An integer value.</value>
		[
		Description( "Gets / sets the distance between the GroupViewItem image and the text." ),
		Category( "Appearance - Spacing" ),
		DefaultValue( 8 ),
		Localizable( true )
		]
		public int TextSpacing
		{
			get { return this.nTextSpacing; }

			set
			{
				if( this.nTextSpacing != value )
				{
					this.nTextSpacing = value;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets / sets the horizontal distance between a <see cref="GroupViewItem"/> and the control's left border.
		/// </summary>
		/// <value>An integer value.</value>
		/// <remarks>
		/// This attribute is valid only when <see cref="GroupView.SmallImageView"/> is True. In large image view, the items are drawn centered. 
		/// </remarks>
		[
		Description( "Gets / sets the horizontal distance between a GroupViewItem and the control's left border." ),
		Category( "Appearance - Spacing" ),
		DefaultValue( 8 ),
		Localizable( true )
		]
		public int ItemXSpacing
		{
			get { return this.nItemXSpacing; }

			set
			{
				if( this.nItemXSpacing != value )
				{
					this.nItemXSpacing = value;
					if( ( this.bFlowView == true ) && ( this.Visible == true ) && ( this.cllnGroupViewItems.Count > 0 ) )
						this.CalculateItemsPerRow();
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets / sets the height between adjacent <see cref="GroupViewItem"/>s.
		/// </summary>
		/// <value>An integer value.</value>
		[
		Description( "Gets / sets the height between adjacent GroupViewItems." ),
		Category( "Appearance - Spacing" ),
		DefaultValue( 5 ),
		Localizable( true )
		]
		public int ItemYSpacing
		{
			get { return this.nItemYSpacing; }

			set
			{
				if( this.nItemYSpacing != value )
				{
					this.nItemYSpacing = value;
					if( ( this.bFlowView == true ) && ( this.Visible == true ) && ( this.cllnGroupViewItems.Count > 0 ) )
						this.CalculateItemsPerRow();
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="GroupViewItem"/>'s text length in FlowView mode.
		/// </summary>
		/// <value>An integer value.</value>
		[
		Description( "Gets or sets the GroupViewItem's text length in FlowView mode." ),
		Category( "Appearance" ),
		DefaultValue( 80 )
		]
		public int FlowViewItemTextLength
		{
			get { return m_flowViewTextLength; }
			set
			{
				if( m_flowViewTextLength != value )
				{
					m_flowViewTextLength = value;

					if( ( this.Visible == true ) && ( this.VisibleItems.Count > 0 ) && ( this.bFlowView == true ) )
						this.CalculateItemsPerRow();

					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the distance by which the <see cref="GroupViewItem"/> image is offset when the mouse is moved over it.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Point"/> value.</value>
		[
		Description( "Gets or sets the image offset for the highlighted GroupViewItem." ),
		Category( "Behavior - Offsets" ),
		Localizable( true )
		]
		public Point HighlightImageOffset
		{
			get { return this.ptHighlightImageOffset; }

			set
			{
				if( this.ptHighlightImageOffset != value )
					this.ptHighlightImageOffset = value;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeHighlightImageOffset()
		{
			return ( this.ptHighlightImageOffset != Point.Empty );
		}

		/// <summary>
		/// Resets the <see cref="GroupView.HighlightImageOffset"/> property to its default value.
		/// </summary>
		public void ResetHighlightImageOffset()
		{
			this.ptHighlightImageOffset = Point.Empty;
		}

		/// <summary>
		/// Gets or sets the distance by which the <see cref="GroupViewItem"/> text is offset when the mouse is moved over it.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Point"/> value.</value>
		[
		Description( "Gets or sets the text offset for the highlighted GroupViewItem." ),
		Category( "Behavior - Offsets" ),
		Localizable( true )
		]
		public Point HighlightTextOffset
		{
			get { return this.ptHighlightTextOffset; }

			set
			{
				if( this.ptHighlightTextOffset != value )
					this.ptHighlightTextOffset = value;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeHighlightTextOffset()
		{
			return ( this.ptHighlightTextOffset != Point.Empty );
		}

		/// <summary>
		/// Resets the <see cref="GroupView.HighlightTextOffset"/> property to its default value.
		/// </summary>
		public void ResetHighlightTextOffset()
		{
			this.ptHighlightTextOffset = Point.Empty;
		}

		/// <summary>
		/// Gets / sets the distance by which the <see cref="GroupViewItem"/> image is offset when it is selected.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Point"/> value.</value>
		[
		Description( "Gets / sets the image offset for the selected GroupViewItem." ),
		Category( "Behavior - Offsets" ),
		Localizable( true )
		]
		public Point SelectedImageOffset
		{
			get { return this.ptSelectedImageOffset; }

			set
			{
				if( this.ptSelectedImageOffset != value )
					this.ptSelectedImageOffset = value;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeSelectedImageOffset()
		{
			return ( this.ptSelectedImageOffset != Point.Empty );
		}

		/// <summary>
		/// Resets the <see cref="GroupView.SelectedImageOffset"/> property to its default value.
		/// </summary>
		public void ResetSelectedImageOffset()
		{
			this.ptSelectedImageOffset = Point.Empty;
		}

		/// <summary>
		/// Gets / sets the distance by which the <see cref="GroupViewItem"/> text is offset when it is selected.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Point"/> value.</value>
		[
		Description( "Gets / sets the text offset for the selected GroupViewItem." ),
		Category( "Behavior - Offsets" ),
		Localizable( true )
		]
		public Point SelectedTextOffset
		{
			get { return this.ptSelectedTextOffset; }

			set
			{
				if( this.ptSelectedTextOffset != value )
					this.ptSelectedTextOffset = value;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeSelectedTextOffset()
		{
			return ( this.ptSelectedTextOffset != Point.Empty );
		}

		/// <summary>
		/// Resets the <see cref="GroupView.SelectedTextOffset"/> property to its default value.
		/// </summary>
		public void ResetSelectedTextOffset()
		{
			this.ptSelectedTextOffset = Point.Empty;
		}

		/// <summary>
		/// Gets / sets the distance by which the selected <see cref="GroupViewItem"/> image is offset when the mouse is moved over it.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Point"/> value.</value>
		[
		Description( "Gets / sets the image offset when the mouse is moved over the selected GroupViewItem." ),
		Category( "Behavior - Offsets" ),
		Localizable( true )
		]
		public Point SelectedHighlightImageOffset
		{
			get { return this.ptSelectedHighlightImageOffset; }

			set
			{
				if( this.ptSelectedHighlightImageOffset != value )
					this.ptSelectedHighlightImageOffset = value;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeSelectedHighlightImageOffset()
		{
			return ( this.ptSelectedHighlightImageOffset != Point.Empty );
		}

		/// <summary>
		/// Resets the <see cref="GroupView.SelectedHighlightImageOffset"/> property to its default value.
		/// </summary>
		public void ResetSelectedHighlightImageOffset()
		{
			this.ptSelectedHighlightImageOffset = Point.Empty;
		}

		/// <summary>
		/// Gets / sets the distance by which the selected <see cref="GroupViewItem"/> text is offset when the mouse is moved over it.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Point"/> value.</value>
		[
		Description( "Gets / sets the text offset when the mouse is moved over a selected GroupViewItem." ),
		Category( "Behavior - Offsets" ),
		Localizable( true )
		]
		public Point SelectedHighlightTextOffset
		{
			get { return this.ptSelectedHighlightTextOffset; }

			set
			{
				if( this.ptSelectedHighlightTextOffset != value )
					this.ptSelectedHighlightTextOffset = value;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeSelectedHighlightTextOffset()
		{
			return ( this.ptSelectedHighlightTextOffset != Point.Empty );
		}

		/// <summary>
		/// Resets the <see cref="GroupView.SelectedHighlightTextOffset"/> property to its default value.
		/// </summary>
		public void ResetSelectedHighlightTextOffset()
		{
			this.ptSelectedHighlightTextOffset = Point.Empty;
		}

		/// <summary>
		/// Gets / sets the distance by which the <see cref="GroupViewItem"/> image is offset when it is being selected.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Point"/> value.</value>
		[
		Description( "Gets / sets the image offset for the GroupViewItem being selected." ),
		Category( "Behavior - Offsets" ),
		Localizable( true )
		]
		public Point SelectingImageOffset
		{
			get { return this.ptSelectingImageOffset; }

			set
			{
				if( this.ptSelectingImageOffset != value )
					this.ptSelectingImageOffset = value;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeSelectingImageOffset()
		{
			return ( this.ptSelectingImageOffset != Point.Empty );
		}

		/// <summary>
		/// Resets the <see cref="GroupView.SelectingImageOffset"/> property to its default value.
		/// </summary>
		public void ResetSelectingImageOffset()
		{
			this.ptSelectingImageOffset = Point.Empty;
		}

		/// <summary>
		/// Gets / sets the distance by which the <see cref="GroupViewItem"/> text is offset when it is being selected.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Point"/> value.</value>
		[
		Description( "Gets / sets the text offset for the GroupViewItem being selected." ),
		Category( "Behavior - Offsets" ),
		Localizable( true )
		]
		public Point SelectingTextOffset
		{
			get { return this.ptSelectingTextOffset; }

			set
			{
				if( this.ptSelectingTextOffset != value )
					this.ptSelectingTextOffset = value;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeSelectingTextOffset()
		{
			return ( this.ptSelectingTextOffset != Point.Empty );
		}

		/// <summary>
		/// Resets the <see cref="GroupView.SelectingTextOffset"/> property to its default value.
		/// </summary>
		public void ResetSelectingTextOffset()
		{
			this.ptSelectingTextOffset = Point.Empty;
		}

		/// <summary>
		/// Gets / sets the image list containing the large (32x32) images.
		/// </summary>
		/// <remarks>
		/// <seealso cref="GroupView.SmallImageList"/>
		/// </remarks>
		/// <value>An ImageList type.</value>
		[
		Description( "The image list containing the large (32x32) images associated with the control." ),
		Category( "Behavior" ), DefaultValue(null)
		]
		public ImageList LargeImageList
		{
			get
			{
				return this.ilLarge;
			}
			set
			{
				this.ilLarge = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Gets / sets the image list containing the small (16x16) images.
		/// </summary>
		/// <value>An ImageList type.</value>
		/// <remarks>
		/// <seealso cref="GroupView.LargeImageList"/>
		/// </remarks>
		[
		Description( "The image list containing the small (16x16) images associated with the control." ),
        Category("Behavior"), DefaultValue(null)
		]
		public ImageList SmallImageList
		{
			get
			{
				return this.ilSmall;
			}
			set
			{
				this.ilSmall = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Indicates whether the control displays items using the images in the <see cref="GroupView.SmallImageList"/>.
		/// </summary>
		/// <value>True if the small image mode is set. The default is False.</value>
		[
		Description( "Indicates whether the control should use small images for the items." ),
		Category( "Appearance" ),
		DefaultValue( false )
		]
		public bool SmallImageView
		{
			get { return bSmallImageView; }
			set
			{
				if( bSmallImageView != value )
				{
					this.bSmallImageView = value;
					// Icon size has changed. Reset scroll positions to default.
					this.nTopIndex = 0;
					this.bDownScrlButton = false;
					this.bUpScrlButton = false;
					this.nHighlightedItem = -1;
					if( ( this.bFlowView == true ) && ( this.Visible == true ) && ( this.cllnGroupViewItems.Count > 0 ) )
						this.CalculateItemsPerRow();
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Indicates whether items are drawn with a button-type flat look upon selection.
		/// </summary>
		/// <value>True if the button-type selection is set. The default is False.</value>
		[
		Description( "Indicates whether the control should use the button-type selection mode." ),
		Category( "Appearance" ),
		DefaultValue( false )
		]
		public bool ButtonView
		{
			get { return this.bButtonView; }
			set
			{
				if( this.bButtonView != value )
				{
					this.bButtonView = value;
					if( value == true )
					{
						if( this.nSelectedItem == -1 )
							this.nSelectedItem = 0;
					}
					else
					{
						this.nSelectedItem = -1;
					}
					this.nHighlightedItem = -1;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Indicates whether the flow view display mode is set.
		/// </summary>
		/// <remarks>
		/// In the default display mode, the <see cref="GroupView"/> control items are arranged top-down in list form. 
		/// In the flow mode, however, item images are arranged side by side for the full width of the control. 
		/// Resizing the control will wrap the images. Text will not be displayed in this mode, 
		/// unless the ShowFlowViewItemText property set to True.
		/// </remarks>
		/// <value>True if flow view is set. The default is False.</value>
		[
		Description( "Indicates whether the control should use the flow type display." ),
		Category( "Appearance" ),
		DefaultValue( false )
		]
		public bool FlowView
		{
			get
			{
				return this.bFlowView;
			}
			set
			{
				if( this.bFlowView != value )
				{
					if( value == true )
					{
						if( ( this.Visible == true ) && ( this.cllnGroupViewItems.Count > 0 ) )
							this.CalculateItemsPerRow();
					}
					this.bFlowView = value;
					this.Invalidate();
				}
			}
		}


		/// <summary>
		/// Indicates whether control should show GroupViewItem text in FlowView mode.
		/// </summary>
		/// <remarks>
		/// <value>True if should show text. The default is False.</value>
		/// </remarks>
		[
		Description( "Indicates whether control should show GroupViewItem text in FlowView mode." ),
		Category( "Appearance" ),
		DefaultValue( false )
		]
		public bool ShowFlowViewItemText
		{
			get
			{
				return m_bShowFlowViewText;
			}
			set
			{
				if( m_bShowFlowViewText != value )
				{
					m_bShowFlowViewText = value;

					if( ( this.Visible == true ) && ( this.VisibleItems.Count > 0 ) && ( this.bFlowView == true ) )
						this.CalculateItemsPerRow();

					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Indicates whether scrolling should be delegated to the <see cref="GroupView"/> control's parent.
		/// </summary>
		/// <remarks>
		/// This option is primarily intended for use with the Syncfusion <see cref="GroupBar"/> control. 
		/// When this option is set, the GroupView control delegates scrolling behavior to the 
		/// parent GroupBar control. When IntegratedScrolling is set to False, the control provides its own scroll buttons.
		/// </remarks>
		/// <value>True to enable integrated scrolling. The default is False.</value>
		/// <seealso cref="GroupBar.IntegratedScrolling"/>
		[
		Description( "Indicates whether the control should use the IntegratedScrolling option with the parent control containing the scroll buttons." ),
		Category( "Behavior" ),
		DefaultValue( false )
		]
		public bool IntegratedScrolling
		{
			get { return this.bIntegratedScrolling; }
			set
			{
				if( this.bIntegratedScrolling != value )
				{
					this.bIntegratedScrolling = value;
					if( ( this.bFlowView == true ) && ( this.Visible == true ) && ( this.cllnGroupViewItems.Count > 0 ) )
						this.CalculateItemsPerRow();
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets / sets the index of the currently selected <see cref="GroupViewItem"/>.
		/// </summary>
		/// <value>The zero-based index of the selected item. -1 if a selected item is not available.</value>
		[
		Description( "Gets / sets the index of the currently selected GroupBarItem." ),
		Category( "Behavior" ),
		DefaultValue( -1 )
		]
		public int SelectedItem
		{
			get
			{
				int visibleIndex = this.SelectedItemInternal;

				if( visibleIndex >= 0 && visibleIndex < this.VisibleItems.Count )
				{
					GroupViewItem item = this.VisibleItems[visibleIndex] as GroupViewItem;
					if( item != null )
					{
						return this.cllnGroupViewItems.IndexOf( item );
					}
				}

				return -1;
			}
			set
			{
				if( value >= 0 )
				{
					if( value < this.cllnGroupViewItems.Count )
					{
						GroupViewItem item = this.cllnGroupViewItems[value];

						int visibleIndex = this.VisibleItems.IndexOf( item );
						if( visibleIndex >= 0 )
						{
							this.SelectedItemInternal = visibleIndex;
						}
					}
				}
				else this.SelectedItemInternal = -1;
			}
		}
		internal int SelectedItemInternal
		{
			get { return this.nSelectedItem; }
			set
			{
				if( this.nSelectedItem != value )
				{
					this.nSelectedItem = value;
					if( ( this.ButtonView == true ) && ( this.Visible == true ) )
						this.Invalidate();
					this.OnGroupViewItemSelected( EventArgs.Empty );
				}
			}
		}

		/// <summary>
		/// Returns the <see cref="GroupViewItem"/> over which the mouse cursor is hovering.
		/// </summary>
		/// <value>The zero-based index of the item under the cursor. -1 if no item is being highlighted.</value>
		[
		Description( "Returns the index of the GroupViewItem currently under the mouse cursor." ),
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public int HighlightedItem
		{
			get { return this.nHighlightedItem; }
		}

		/// <summary>
		/// Returns the index of <see cref="GroupViewItem"/> that triggered the <see cref="GroupView.ShowContextMenu"/> event.
		/// </summary>
		/// <value>The zero-based index of the item.</value>
		[
		Description( "Returns the index of the GroupViewItem for which the ContextMenu was last displayed." ),
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public int ContextMenuItem
		{
			get { return this.nContextMenuItem; }
		}

		/// <summary>
		/// Gets / sets a value indicating whether the <see cref="GroupViewItem"/> image is highlighted when the mouse is moved over it.
		/// </summary>
		/// <value>False if image highlighting is disabled. The default is True.</value>
		[
		Description( "Indicates whether the control should highlight the image when the mouse is moved over the GroupViewItem." ),
		Category( "Behavior" ),
		DefaultValue( true )
		]
		public bool HighlightImage
		{
			get { return this.bHighlightImage; }
			set
			{
				if( this.bHighlightImage != value )
				{
					this.bHighlightImage = value;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="GroupViewItem"/> text is highlighted when the mouse is moved over it.
		/// </summary>
		/// <value>False if text highlighting is disabled. The default is True.</value>
		[
		Description( "Indicates whether the control should highlight the text when the mouse is moved over the GroupViewItem." ),
		Category( "Behavior" ),
		DefaultValue( true )
		]
		public bool HighlightText
		{
			get { return this.bHighlightText; }
			set
			{
				if( this.bHighlightText != value )
				{
					this.bHighlightText = value;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="GroupViewItem"/> text is underlined when the mouse is moved over it.
		/// </summary>
		/// <value>True if text underlining is enabled. The default is False.</value>
		[
		Description( "Indicates whether the control should underline the text when the mouse is moved over the GroupViewItem." ),
		Category( "Behavior" ),
		DefaultValue( false )
		]
		public bool TextUnderline
		{
			get { return this.bTextUnderline; }
			set
			{
				if( this.bTextUnderline != value )
				{
					this.bTextUnderline = value;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the color used for drawing the background of a <see cref="GroupViewItem"/> when the mouse is moved over it.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Color"/> value.</value>
		[
		Description( "The color used to draw the background of a GroupViewItem when the mouse cursor is moved over it." ),
		Category( "Appearance" )
		]
		public Color HighlightItemColor
		{
			get
			{
				if( this.brHighlightItem is SolidBrush )
					return ( this.brHighlightItem as SolidBrush ).Color;
				else return this.BackColor;
			}
			set
			{
				if( this.HighlightItemColor != value )
				{
					this.brHighlightItem = new SolidBrush( value );
					this.bHighlightItemSet = true;
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeHighlightItemColor()
		{
			return this.bHighlightItemSet;
		}

		/// <summary>
		/// Resets the <see cref="GroupView.HighlightItemColor"/> property to its default value.
		/// </summary>
		public void ResetHighlightItemColor()
		{
			this.HighlightItemColor = this.BackColor;
			this.bHighlightItemSet = false;
		}

		/// <summary>
		/// Gets / sets the color used for drawing the background of the selected <see cref="GroupViewItem"/>.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Color"/> value.</value>
		[
		Description( "The color used to draw the background of the selected GroupViewItem." ),
		Category( "Appearance" )
		]
		public Color SelectedItemColor
		{
			get
			{
				if( this.brSelectedItem is SolidBrush )
					return ( this.brSelectedItem as SolidBrush ).Color;
				else return this.BackColor;
			}
			set
			{
				if( this.SelectedItemColor != value )
				{
					this.brSelectedItem = new SolidBrush( value );
					this.bSelectedItemSet = true;
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeSelectedItemColor()
		{
			return this.bSelectedItemSet;
		}

		/// <summary>
		/// Resets the <see cref="GroupView.SelectedItemColor"/> property to its default value.
		/// </summary>
		public void ResetSelectedItemColor()
		{
			this.SelectedItemColor = this.BackColor;
			this.bSelectedItemSet = false;
		}

		/// <summary>
		/// Gets / sets the color used for drawing the background of the selected <see cref="GroupViewItem"/> when the mouse is moved over it.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Color"/> value.</value>
		[
		Description( "The color used to draw the background of the selected GroupViewItem when the mouse cursor is moved over it." ),
		Category( "Appearance" )
		]
		public Color SelectedHighlightItemColor
		{
			get
			{
				if( this.brSelectedHighlightItem is SolidBrush )
					return ( this.brSelectedHighlightItem as SolidBrush ).Color;
				else return this.BackColor;
			}
			set
			{
				if( this.SelectedHighlightItemColor != value )
				{
					this.brSelectedHighlightItem = new SolidBrush( value );
					this.bSelectedHighlightItemSet = true;
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeSelectedHighlightItemColor()
		{
			return this.bSelectedHighlightItemSet;
		}

		/// <summary>
		/// Resets the <see cref="GroupView.SelectedHighlightItemColor"/> property to its default value.
		/// </summary>
		public void ResetSelectedHighlightItemColor()
		{
			this.SelectedHighlightItemColor = this.BackColor;
			this.bSelectedHighlightItemSet = false;
		}

		/// <summary>
		/// Gets / sets the color used for drawing the background of the <see cref="GroupViewItem"/> being selected.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Color"/> value.</value>
		[
		Description( "The color used to draw the background of the GroupViewItem being selected." ),
		Category( "Appearance" )
		]
		public Color SelectingItemColor
		{
			get
			{
				if( this.brSelectingItem is SolidBrush )
					return ( this.brSelectingItem as SolidBrush ).Color;
				else return this.BackColor;
			}
			set
			{
				if( this.SelectingItemColor != value )
				{
					this.brSelectingItem = new SolidBrush( value );
					this.bSelectingItemSet = true;
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeSelectingItemColor()
		{
			return this.bSelectingItemSet;
		}

		/// <summary>
		/// Resets the <see cref="GroupView.SelectingItemColor"/> property to its default value.
		/// </summary>
		public void ResetSelectingItemColor()
		{
			this.SelectingItemColor = this.BackColor;
			this.bSelectingItemSet = false;
		}

		/// <summary>
		/// Gets or sets the color used for drawing the <see cref="GroupViewItem"/> text when the mouse is moved over it.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Color"/> value.</value>
		[
		Description( "The color used to draw the text of a GroupViewItem when the mouse moves over it." ),
		Category( "Appearance" )
		]
		public Color HighlightTextColor
		{
			get { return this.clrHighlightText; }
			set
			{
				if( this.clrHighlightText != value )
				{
					this.clrHighlightText = value;
					this.bHighlightTextSet = true;
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeHighlightTextColor()
		{
			return this.bHighlightTextSet;
		}

		/// <summary>
		/// Resets the <see cref="GroupView.HighlightTextColor"/> property to its default value.
		/// </summary>
		public void ResetHighlightTextColor()
		{
			this.HighlightTextColor = this.ForeColor;
			this.bHighlightTextSet = false;
		}

		/// <summary>
		/// Gets / sets the color used for drawing the selected <see cref="GroupViewItem"/> text.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Color"/> value.</value>
		[
		Description( "The color used to draw the text of a selected GroupViewItem." ),
		Category( "Appearance" )
		]
		public Color SelectedTextColor
		{
			get { return this.clrSelectedText; }
			set
			{
				if( this.clrSelectedText != value )
				{
					this.clrSelectedText = value;
					this.bSelectedTextSet = true;
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeSelectedTextColor()
		{
			return this.bSelectedTextSet;
		}

		/// <summary>
		/// Resets the <see cref="GroupView.SelectedTextColor"/> property to its default value.
		/// </summary>
		public void ResetSelectedTextColor()
		{
			this.SelectedTextColor = this.ForeColor;
			this.bSelectedTextSet = false;
		}

		/// <summary>
		/// Gets / sets the color used for drawing the selected <see cref="GroupViewItem"/> text when the mouse is moved over it.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Color"/> value.</value>
		[
		Description( "The color used to draw the text of a selected GroupViewItem when the mouse moves over it." ),
		Category( "Appearance" )
		]
		public Color SelectedHighlightTextColor
		{
			get { return this.clrSelectedHighlightText; }
			set
			{
				if( this.clrSelectedHighlightText != value )
				{
					this.clrSelectedHighlightText = value;
					this.bSelectedHighlightTextSet = true;
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeSelectedHighlightTextColor()
		{
			return this.bSelectedHighlightTextSet;
		}

		/// <summary>
		/// Resets the <see cref="GroupView.SelectedHighlightTextColor"/> property to its default value.
		/// </summary>
		public void ResetSelectedHighlightTextColor()
		{
			this.SelectedHighlightTextColor = this.ForeColor;
			this.bSelectedHighlightTextSet = false;
		}

		/// <summary>
		/// Gets / sets the color used for drawing the <see cref="GroupViewItem"/> text while it is being selected.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Color"/> value.</value>
		[
		Description( "The color used to draw the text of the GroupViewItem being selected." ),
		Category( "Appearance" )
		]
		public Color SelectingTextColor
		{
			get { return this.clrSelectingText; }
			set
			{
				if( this.clrSelectingText != value )
				{
					this.clrSelectingText = value;
					this.bSelectingTextSet = true;
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeSelectingTextColor()
		{
			return this.bSelectingTextSet;
		}

		/// <summary>
		/// Resets the <see cref="GroupView.SelectingTextColor"/> property to its default value.
		/// </summary>
		public void ResetSelectingTextColor()
		{
			this.SelectingTextColor = this.ForeColor;
			this.bSelectingTextSet = false;
		}

		/// <summary>
		/// Gets or sets the brush used for drawing the <see cref="GroupView"/> control background. 
		/// </summary>
		/// <value> A <see cref="System.Drawing.Brush"/> value.</value>
		[Browsable( false )]
		public Brush BackGroundBrush
		{
			get { return this.brBackGround; }
			set
			{
				if( this.brBackGround != value )
				{
					this.brBackGround = value;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the brush used for drawing the background of the selected <see cref="GroupViewItem"/>. 
		/// </summary>
		/// <value> A <see cref="System.Drawing.Brush"/> value.</value>
		[Browsable( false )]
		public Brush SelectedItemBrush
		{
			get { return this.brSelectedItem; }
			set
			{
				if( this.brSelectedItem != value )
				{
					this.brSelectedItem = value;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the brush used for drawing the background of the selected <see cref="GroupViewItem"/> 
		/// when the mouse is moved over it. 
		/// </summary>
		/// <value> A <see cref="System.Drawing.Brush"/> value.</value>
		[Browsable( false )]
		public Brush SelectedItemHighlightBrush
		{
			get { return this.brSelectedHighlightItem; }
			set
			{
				if( this.brSelectedHighlightItem != value )
				{
					this.brSelectedHighlightItem = value;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the brush used for drawing the background of the highlighted <see cref="GroupViewItem"/>. 
		/// </summary>
		/// <value> A <see cref="System.Drawing.Brush"/> value.</value>
		[Browsable( false )]
		public Brush HighlightItemBrush
		{
			get { return this.brHighlightItem; }
			set
			{
				if( this.brHighlightItem != value )
				{
					this.brHighlightItem = value;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets or sets the brush used for drawing the background of the <see cref="GroupViewItem"/> being selected. 
		/// </summary>
		/// <value> A <see cref="System.Drawing.Brush"/> value.</value>
		[Browsable( false )]
		public Brush SelectingItemBrush
		{
			get { return this.brSelectingItem; }
			set
			{
				if( this.brSelectingItem != value )
				{
					this.brSelectingItem = value;
					this.Invalidate();
				}
			}
		}
         
		/// <summary>
		/// Gets or sets the border style of the <see cref="GroupView"/> control.
		/// </summary>
		/// <value>A <see cref="System.Windows.Forms.BorderStyle"/> value. The default is BorderStyle.Fixed3D.</value>
		[
		Description( "The border style of the control." ),
		Category( "Appearance" ),
		DefaultValue( BorderStyle.Fixed3D )
		]
		public BorderStyle BorderStyle
		{
			get { return this.bdrStyle; }

			set
			{
				if( this.bdrStyle != value )
				{
					if( !Enum.IsDefined( typeof( System.Windows.Forms.BorderStyle ), value ) )
						throw new InvalidEnumArgumentException( "value", ( (int)( value ) ), typeof( BorderStyle ) );
					this.bdrStyle = value;
					UpdateStyles();
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected override CreateParams CreateParams
		{
			get
			{
				System.Windows.Forms.CreateParams cp = base.CreateParams;
				switch( this.bdrStyle )
				{
					case BorderStyle.Fixed3D:
					cp.ExStyle |= 0x200; // WS_EX_DLGFRAME
					break;
					case BorderStyle.FixedSingle:
					cp.Style |= 0x800000; // WS_BORDER
					break;
				}
				return cp;
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="GroupViewItem"/> text should be wrapped.
		/// </summary>
		/// <remarks>
		/// Text wrapping is available only with the large icon display mode.
		/// </remarks>
		/// <value>True to turn on wrapping. The default is False.</value>
		[
		Description( "Gets / sets a value indicating whether the GroupViewItem text should be wrapped." ),
		Category( "Behavior" ),
		DefaultValue( false )
		]
		public bool TextWrap
		{
			get { return this.bTextWrap; }
			set
			{
				if( this.bTextWrap != value )
				{
					this.bTextWrap = value;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="GroupView"/> control is displayed with a flat look.
		/// </summary>		
		/// <value>True to display in flat mode. The default is False.</value>
		[
		Description( "Gets or sets a value indicating whether the control is displayed with a flat look." ),
		Category( "Appearance" ),
		DefaultValue( false )
		]
		public bool FlatLook
		{
			get { return this.bFlatLook; }
			set
			{
				if( this.bFlatLook != value )
				{
					this.bFlatLook = value;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Indicates whether XP Themes (visual styles) should be used for drawing the control.
		/// </summary>
		/// <value>True to turn on themes; the default is False.</value>
		[
		DefaultValue( false ),
		Category( @"Appearance" ),
		Description( "Specifies whether XP Themes (visual styles) should be used for drawing the control." )
		]
		public bool ThemesEnabled
		{
			get { return this.bThemesEnabled; }
			set
			{
				if( this.bThemesEnabled != value )
				{
					this.bThemesEnabled = value;
					this.Invalidate();
				}
			}
		}
        /// <summary>
        /// Gets a value indicating whether the control supports drop.
        /// </summary>
		[
		Browsable( false ),
		EditorBrowsable( EditorBrowsableState.Never ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public override bool AllowDrop
		{
			get { return base.AllowDrop; }
			set { /* Ignore */	}
		}

		/// <summary>
		/// Indicates whether the control supports drag-and-drop of <see cref="GroupViewItem"/> objects.
		/// </summary>
		[
		Description( "Gets or sets a value indicating whether the control supports drag-and-drop." ),
		Category( "Behavior" ),
		DefaultValue( false ),
		RefreshProperties( RefreshProperties.All )
		]
		public bool AllowDragDrop
		{
			get { return this.bAllowDragDrop; }
			set
			{
				if( this.bAllowDragDrop != value )
				{
					this.bAllowDragDrop = value;
					if( !value && AllowDragAnyObject )
					{
						AllowDragAnyObject = false;
					}
				}
			}
		}

		/// <summary>
		/// Allow drag any object.
		/// </summary>
		private bool m_bAllowDragAnyObject = false;

		/// <summary>
		/// Gets or sets allow drag any object. For use this property AllowDragDrop must be true.
		/// </summary>
		[
		Description( "Gets or sets allow drag any object. For use this property AllowDragDrop must be true." ),
		Category( "Behavior" ),
		DefaultValue( false ),
		RefreshProperties( RefreshProperties.All )
		]
		public bool AllowDragAnyObject
		{
			get
			{
				return this.m_bAllowDragAnyObject;
			}
			set
			{
				if( this.m_bAllowDragAnyObject != value )
				{
					this.m_bAllowDragAnyObject = value;

					if( value && !AllowDragDrop )
					{
						AllowDragDrop = true;
					}
				}
			}
		}

		/// <summary>
		/// Indicates whether the selection bounds of a <see cref="GroupViewItem"/> are clipped around its image and text.
		/// </summary>
		/// <value>A Boolean value. The default is False.</value>
		[
		Description( "Gets or sets a value indicating whether the selection bounds of a GroupViewItem are clipped." ),
		Category( "Behavior" ),
		DefaultValue( false )
		]
		public bool ClipSelectionBounds
		{
			get { return this.bClipSelectionBounds; }
			set { this.bClipSelectionBounds = value; }
		}

		[
		Browsable( false ),
		EditorBrowsable( EditorBrowsableState.Never ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public int TimerScrollInterval
		{
			get { return this.tmrScrolling.Interval; }
			set
			{
				if( this.tmrScrolling.Interval != value )
					this.tmrScrolling.Interval = value;
			}
		}

		/// <summary>
		/// Gets the Height of the Item's Text.
		/// </summary>
		private int ItemTextHeight
		{
			get
			{
				if( m_itemTextHeight < 0 )
				{
					m_itemTextHeight = 0;

					if( this.ShowFlowViewItemText )
					{
						Rectangle rcClient = this.ClientRectangle;

						for( int i = 0; i<this.VisibleItems.Count; i++ )
						{
							int itemTextHeight = Size.Ceiling( this.MeasureText( i, rcClient ) ).Height;

							if( m_itemTextHeight < itemTextHeight )
							{
								m_itemTextHeight = itemTextHeight;
							}
						}
					}
				}
				return m_itemTextHeight;
			}
		}

		private int m_itemTextHeight = -1;


		/// <summary>
		/// Creates a new instance of the <see cref="GroupView"/> control.
		/// </summary>
		public GroupView()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				new Syncfusion.Core.Licensing.LicensedComponent( typeof( GroupView ) );
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}
			this.SetStyle( ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | WhidbeyCompatibleControlStyles.DoubleBuffer, true );
			base.AllowDrop = true;
			this.cllnGroupViewItems = new GroupViewItemCollection( this );
			this.cllnGroupViewItems.CollectionChanged += new CollectionChangeEventHandler( cllnGroupViewItems_CollectionChanged );
			this.ContextMenu = null;

			this.tmrScrolling = new Timer();
			this.tmrScrolling.Interval = 300;
			this.tmrScrolling.Tick += new EventHandler( this.ScrollTimerEventHandler );

			this.ctrlToolTip = new ToolTipForm( this );

			if( XPThemes.IsThemedOS )
			{
				this.tdToolbar = new ThemedControlDrawing( ThemedControls.TOOLBAR );
				this.tdScrollBar = new ThemedControlDrawing( ThemedControls.SCROLLBAR );
			}
            CTRLSIZE = new Size(151,123);
			InitToolTip();
		}

		/// <summary>
		/// Starts an in-place edit of the specified <see cref="GroupViewItem"/> text. 
		/// </summary>
		/// <remarks>
		/// Invoking this method will create an editable text box and and populate it with 
		/// the item text. Editing the textbox contents and selecting ENTER will update the item text. 
		/// Selecting ESC will cancel the edit.
		/// </remarks>
		/// <param name="nindex">The zero-based index of the item to be renamed.</param>
		/// <seealso cref="GroupView.CancelInplaceRenameItem"/>
		public virtual void InplaceRenameItem( int nindex )
		{
			if( this.Visible == false )
				return;

			if( nindex < 0 || nindex >= this.cllnGroupViewItems.Count )
			{
				Debug.Assert( false, "Invalid Item Index\n" );
				return;
			}

			// Create the edit control for the first invocation of EditBar.
			if( this.ctrlTextBox == null )
			{
				ctrlTextBox = new RenameTextBox();
				this.Controls.Add( ctrlTextBox );
				ctrlTextBox.RenameComplete += new RenameCompleteEventHandler( this.GroupView_RenameComplete );
				ctrlTextBox.TextChanged += new System.EventHandler( this.GroupView_TextChanged );
			}

			this.nRenameItem = nindex;

			if( this.bFlowView == true )
			{
				this.FlowView = false;
				m_bFlowViewChanged = true;
			}

			int ninflate = 1;
			if( this.bSmallImageView == false )
			{
				ctrlTextBox.TextAlign = HorizontalAlignment.Center;
				ctrlTextBox.Multiline = false;
				ninflate = 2;
			}
			else
			{
				ctrlTextBox.TextAlign = HorizontalAlignment.Left;
				ctrlTextBox.Multiline = false;
			}
			// Get the client rect of the BarObject for nbar and position the edit control at that location.
			BringItemIntoView( nindex );
			Rectangle rcitem = GetGroupViewItemBounds( nRenameItem, "Text" );
			rcitem.Inflate( ninflate, ninflate );

			this.ctrlTextBox.BeginRename( rcitem, ( (GroupViewItem)this.cllnGroupViewItems[nRenameItem] ).Text );
		}

		/// <summary>
		/// Cancels an inplace renaming that is in progress.
		/// </summary>
		/// <seealso cref="GroupView.InplaceRenameItem"/>
		public void CancelInplaceRenameItem()
		{
			if( nRenameItem >= 0 )
				this.ctrlTextBox.EndRename( true );
		}

		/// <summary>
		/// Brings the specified <see cref="GroupViewItem"/> into the visible area of the <see cref="GroupView"/> control.
		/// </summary>
		/// <param name="nindex">The zero-based index of the item.</param>
		public void BringItemIntoView( int nindex )
		{
			if( nindex < 0 )
				return;

			// If no items are hidden, return.
			if( this.bDownScrlButton == false && this.bUpScrlButton == false )
				return;

			int nfullitems = this.CalculateItemsPerPage();

			if( ( nindex >= this.nTopIndex ) && ( nindex < this.nTopIndex+nfullitems ) )
				return;	// Item is visible

			if( nindex < this.nTopIndex )	// Do a downscroll to bring nindex into view.
			{
				if( this.bFlowView == false )
				{
					int ntemp = this.nTopIndex;
					for( int i=nindex; i < ntemp; i++ )
						DoScroll( false, false );
				}
				else
				{
					do
					{
						DoScroll( false, false );
						if( this.nTopIndex <= nindex )
							break;
					} while( true );
				}
			}
			else	// Do an upscroll.
			{
				if( this.bFlowView == false )
				{
					// If nindex comes into view, then stop scrolling.
					int ntemp = this.nTopIndex+nfullitems;
					for( int i=nindex; i>=ntemp; i-- )
						DoScroll( true, false );
				}
				else
				{
					do
					{
						DoScroll( true, false );
						if( ( this.nTopIndex+nfullitems ) > nindex )
							break;
					} while( true );
				}
			}
		}

		[
		EditorBrowsable( EditorBrowsableState.Never ),
		Syncfusion.Documentation.DocumentationExclude()
		]
		internal bool IsItemVisible( int nindex )
		{
			if( ( nindex < 0 ) || ( nindex >= this.VisibleItems.Count ) )
			{
				Debug.Assert( false, "Invalid Index" );
				return false;
			}

			// If no items are hidden, return.
			if( this.bDownScrlButton == false && this.bUpScrlButton == false )
				return true;

			if( ( nindex >= this.nTopIndex ) && ( nindex < this.nTopIndex+this.CalculateItemsPerPage() ) )
				return true;	// Item is visible.

			return false;
		}

		// Implementation of the IIntegratedScrollClient interface.		
		bool IIntegratedScrollClient.IsUpScrollButtonEnabled()
		{
			if( this.bIntegratedScrolling == false )
				return false;
			return this.bUpScrlButton;
		}

		bool IIntegratedScrollClient.IsDownScrollButtonEnabled()
		{
			if( this.bIntegratedScrolling == false )
				return false;
			return this.bDownScrlButton;
		}

		void IIntegratedScrollClient.UpScrollButtonPressed()
		{
			this.DoScroll( false, false );
		}

		void IIntegratedScrollClient.DownScrollButtonPressed()
		{
			this.DoScroll( true, false );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void PropChangedHandler( Object obj, PropertyChangedEventArgs args )
		{
			GroupViewItem item = obj as GroupViewItem;
			if( item == null )
				return;

			if( args.PropertyName == DEF_PROPERTY_NAME_VISIBLE )
			{
				RecalculateVisibleItem();
			}

			this.Invalidate();
		}

		// Event handler for the RenameComplete event fired by the RenameTextBox.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void GroupView_RenameComplete( Object sender, RenameCompleteEventArgs arg )
		{
			String name = arg.NewName;
			int ntemp = nRenameItem;

			if( ntemp >= 0 && ntemp < this.cllnGroupViewItems.Count )
			{
				nRenameItem = -1;
				GroupViewItem item = (GroupViewItem)this.cllnGroupViewItems[ntemp];
				String strold = item.Text;
				if( ( name != null ) && ( item.Text.Equals( name ) == false ) )
					item.Text = name;

				OnGroupViewItemRenamed( new GroupItemRenamedEventArgs( ntemp, strold, name ) );

				this.Invalidate( GetGroupViewItemBounds( ntemp, "Text" ) );
				this.Update();
			}
			if( m_bFlowViewChanged )
			{
				this.FlowView = true;
				m_bFlowViewChanged = false;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void GroupView_TextChanged( object sender, System.EventArgs e )
		{
			if( nRenameItem == -1 )
				return;

			// Resize edit control till the max allowed client rect is reached. Scroll beyond that.
			String text = this.ctrlTextBox.Text;
			Graphics gph = this.CreateGraphics();
			SizeF sztxt = gph.MeasureString( text, this.ctrlTextBox.Font );
			sztxt.Width += ( 2 * this.ctrlTextBox.Font.Size );	// Provide space for atleast 2 key presses.
			sztxt.Width = ( sztxt.Width > 25 ) ? sztxt.Width : 25;
			gph.Dispose();

			Rectangle rcctrl = this.ctrlTextBox.Bounds;
			if( this.bSmallImageView == false )
			{
				int nclientwidth = this.ClientRectangle.Width-( nScrllBttnOffset*2 );
				if( rcctrl.Width <= nclientwidth )
				{
					rcctrl.X = nScrllBttnOffset + ( nclientwidth -(int)sztxt.Width )/2;
					rcctrl.Width = (int)sztxt.Width;
				}
			}
			else
			{
				int nclientwidth = this.ClientRectangle.Width - ( this.nItemXSpacing+nScrllBttnOffset );
				if( rcctrl.Width < nclientwidth )
				{
					rcctrl.Width = (int)sztxt.Width;
				}
			}
			this.ctrlTextBox.SetBounds( rcctrl.X, 0, rcctrl.Width, 0, BoundsSpecified.X|BoundsSpecified.Width );
		}

		/// <summary>
		/// Raises the <see cref="GroupView.GroupViewItemSelected"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="System.EventArgs"/> value that contains the event data.</param>
		protected virtual void OnGroupViewItemSelected( EventArgs arg )
		{
			if( this.GroupViewItemSelected != null )
			{
				GroupViewItemSelected( this, arg );
			}
		}

		/// <summary>
		/// Raises the <see cref="GroupView.GroupViewItemHighlighted"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="System.EventArgs"/> value that contains the event data.</param>
		protected virtual void OnGroupViewItemHighlighted( EventArgs arg )
		{
			if( this.GroupViewItemHighlighted != null )
			{
				GroupViewItemHighlighted( this, arg );
			}
		}

		/// <summary>
		/// Raises the <see cref="GroupView.GroupViewItemRenamed"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="GroupItemRenamedEventArgs"/> value that contains the event data.</param>
		protected virtual void OnGroupViewItemRenamed( GroupItemRenamedEventArgs arg )
		{
			if( this.GroupViewItemRenamed != null )
			{
				GroupViewItemRenamed( this, arg );
			}
		}

		/// <summary>
		/// Raises the <see cref="GroupView.GroupViewItemsReordered"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="System.EventArgs"/> value that contains the event data.</param>
		protected virtual void OnGroupViewItemsReordered( EventArgs arg )
		{
			if( this.GroupViewItemsReordered != null )
			{
				GroupViewItemsReordered( this, arg );
			}
		}

		/// <summary>
		/// Raises the <see cref="GroupView.ShowContextMenu"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="System.EventArgs"/> value that contains the event data.</param>
		protected virtual void OnShowContextMenu( EventArgs arg )
		{
			if( this.ShowContextMenu != null )
			{
				ShowContextMenu( this, arg );
			}
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnBackColorChanged"/>.
		/// </summary>
		protected override void OnBackColorChanged( EventArgs e )
		{
			base.OnBackColorChanged( e );

			this.brBackGround = new SolidBrush( this.BackColor );
			if( this.ctrlToolTip != null )
				this.ctrlToolTip.BackColor = this.BackColor;

			if( this.bHighlightItemSet == false )
				this.ResetHighlightItemColor();
			if( this.bSelectedItemSet == false )
				this.ResetSelectedItemColor();
			if( this.bSelectedHighlightItemSet == false )
				this.ResetSelectedHighlightItemColor();
			if( this.bSelectingItemSet == false )
				this.ResetSelectingItemColor();
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnForeColorChanged"/>.
		/// </summary>
		protected override void OnForeColorChanged( EventArgs e )
		{
			base.OnForeColorChanged( e );

			if( this.bHighlightTextSet == false )
				this.ResetHighlightTextColor();
			if( this.bSelectedTextSet == false )
				this.ResetSelectedTextColor();
			if( this.bSelectedHighlightTextSet == false )
				this.ResetSelectedHighlightTextColor();
			if( this.bSelectingTextSet == false )
				this.ResetSelectingTextColor();
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnPaint"/>.
		/// </summary>
		protected override void OnPaint( PaintEventArgs e )
		{
			this.Layout();

			DrawAllItems( e.Graphics );
			base.OnPaint( e );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnPaintBackground"/>.
		/// </summary>
		protected override void OnPaintBackground( PaintEventArgs e )
		{
			if( this.BackgroundImage == null )
				e.Graphics.FillRectangle( this.brBackGround, e.ClipRectangle );
			else
				base.OnPaintBackground( e );
		}
        /// <summary>
        ///Size changed
        /// </summary>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (!EnableTouchMode && this.DesignMode)
            {
                CTRLSIZE = this.Size;
            }
        }
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnVisibleChanged"/>.
		/// </summary>
		protected override void OnVisibleChanged( EventArgs e )
		{
			base.OnVisibleChanged( e );

			if( ( this.Visible == true ) && ( this.VisibleItems.Count > 0 ) && ( this.bFlowView == true ) )
				this.CalculateItemsPerRow();
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnDragEnter"/>.
		/// </summary>
		protected override void OnDragEnter( DragEventArgs drgevent )
		{
			if( ( this.bAllowDragDrop == false ) || 
				( !m_bAllowDragAnyObject && ( drgevent.Data.GetDataPresent( GroupViewFormatName ) == false ) ) )
			{
				drgevent.Effect = DragDropEffects.None;
			}

			base.OnDragEnter( drgevent );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnDragOver"/>.
		/// </summary>
		protected override void OnDragOver( DragEventArgs drgevent )
		{
			if( ( this.bAllowDragDrop == false ) ||
				( !m_bAllowDragAnyObject && ( drgevent.Data.GetDataPresent( GroupViewFormatName ) == false ) ) )
			{
				drgevent.Effect = DragDropEffects.None;
			}
			else
			{
				Point ptclient = PointToClient( new Point( drgevent.X, drgevent.Y ) );
				int nhit = ProcessMouseOver( ptclient );
				if( nhit >= 0 )
				{
					// If the cursor is over a partially displayed item, scroll up.
					if( this.bDownScrlButton == true )
					{
						Rectangle rchititem = this.rcHighlightedItem;
						if( this.bSmallImageView == false )
							rchititem.Height += nItemYSpacing + this.Font.Height + nItemYSpacing;
						if( ( rchititem.Bottom > ClientRectangle.Bottom ) && ( ptclient.Y > ( ClientRectangle.Bottom-25 ) ) )
						{
							// Erase any drag inserts that may be present.
							if( this.nDropItem != -1 )
								ProcessDragDrop( MouseActions.Leave, ptclient );
							DoScroll( true, false );
						}
					}
				}
				// In the buttonselect mode, the rect inbetween items is just 1 pxl. So we'll allow 
				// an insertion to take place even when the drag cursor is over a valid item.			
				if( nhit == -1 )
				{
					ProcessDragDrop( MouseActions.Drag, ptclient );
					if( this.nDropItem != -1 )
					{
						if( drgevent.KeyState == 9 ) // CTRL Key
							drgevent.Effect = DragDropEffects.Copy;
						else
							drgevent.Effect = DragDropEffects.Move;
					}
					else
						drgevent.Effect = DragDropEffects.None;
				}
				else
				{
					// If a drop position arrow exists, then erase it.
					if( this.nDropItem != -1 )
						ProcessDragDrop( MouseActions.Leave, ptclient );
					drgevent.Effect = DragDropEffects.None;
				}
			}

			base.OnDragOver( drgevent );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnDragLeave"/>.
		/// </summary>
		protected override void OnDragLeave( EventArgs e )
		{
			// Erase any drop arrows and reset the drop index.
			if( this.nDropItem != -1 )
				ProcessDragDrop( MouseActions.Leave, new Point( -1, -1 ) );

			base.OnDragLeave( e );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnDragDrop"/>.
		/// </summary>
		protected override void OnDragDrop( DragEventArgs drgevent )
		{
			IDataObject dataobject = drgevent.Data;
			if( ( m_bAllowDragAnyObject || ( dataobject.GetDataPresent( GroupViewFormatName ) == true ) )
				&& ( this.nDropItem >= 0 ) )
			{
				int ndropitem = this.nDropItem;
				ProcessDragDrop( MouseActions.Leave, new Point( drgevent.X, drgevent.Y ) );

				GroupViewItem item = dataobject.GetData( GroupViewFormatName ) as GroupViewItem;
				// Check for validity of the drop item.
				if( ( item == null ) || ( item.Text == null ) )
				{
					string text = drgevent.Data.GetData( typeof( string ) ) as string;

					if( text != null )
					{
						item = new GroupViewItem();
						item.Text = text;
					}
					else
					{
						Debug.Assert( false, "Invalid Drop Item." );
						drgevent.Effect = DragDropEffects.None;
						base.OnDragDrop( drgevent );
						return;
					}
				}

				// Is the drop target the same as the drag target?
				if( this.cllnGroupViewItems.Contains( item ) == true )
				{
					// Is the drop position different from the item's current position?
					if( item.Equals( this.cllnGroupViewItems[( ( ndropitem==this.cllnGroupViewItems.Count )?ndropitem-1:ndropitem )] ) || 
						( ( ndropitem-1 >= 0 ) && item.Equals( this.cllnGroupViewItems[ndropitem-1] ) ) )
					{
						// Send same position Notification.
						drgevent.Effect = DragDropEffects.None;
						base.OnDragDrop( drgevent );
						this.nSelectedItem = this.VisibleItems.IndexOf( item );
						return;
					}
					// Create a new item that is a clone of the drag item.
					GroupViewItem newitem = new GroupViewItem( item.Text, item.ImageIndex, item.Tag );
					this.cllnGroupViewItems.Insert( ndropitem, newitem );
					if( this.bButtonView == true )
						this.nSelectedItem = this.VisibleItems.IndexOf( newitem );
					this.Focus();
					// No redraw here. The control redraws itself when the DoDragDrop() call returns.
				}
				else	// Ok, the drop target is different.
				{
					// Create a new GroupViewItem with text equal to the drag item text and referencing the new images.
					GroupViewItem newitem = new GroupViewItem( item.Text, item.ImageIndex, item.Tag );
					this.cllnGroupViewItems.Insert( ndropitem, newitem );
					if( this.bButtonView == true )
						this.nSelectedItem = this.VisibleItems.IndexOf( newitem );

					// Redraw the droptarget.
					this.Invalidate();
					this.Update();

					// Fire the reordered notification.
					OnGroupViewItemsReordered( EventArgs.Empty );
					this.Focus();
				}
				base.OnDragDrop( drgevent );	// Return Effect set by OnDragOver override.						
				return;
			}
			drgevent.Effect = DragDropEffects.None;
			base.OnDragDrop( drgevent );
		}

		private int GetItemCountInPage()
		{
			int fullitems = 0;
			if( !this.bFlowView )
			{
				Rectangle rect = Rectangle.Empty;

				while( rect.Bottom < ( ( this.Horizontal ) ? this.ClientRectangle.Right : this.ClientRectangle.Bottom ) )
				{
					this.SetNonFlowItemBounds( fullitems, ref rect );

					fullitems++;

					if( fullitems == this.VisibleItems.Count )
						break;

					rect.Y = rect.Bottom;
				}
			}
			else
			{
				int itemheight = this.nImageSpacing + this.nImageSpacing + 1;

				itemheight += ( this.bSmallImageView ) ? this.defSmallImgSize.Height : this.defLargeImgSize.Height;

				if( this.ShowFlowViewItemText )
				{
					itemheight += this.ItemTextHeight + nTextSpacing;
				}

				fullitems = ( ClientRectangle.Height / ( itemheight + 1 ) ) * this.nItemsPerRow;
			}
			return fullitems;
		}

		private const int c_iMouseWheelDeltaStep = 120;

		/// <summary>
		/// Called by the tab control when mouse hovers on the control.
		/// </summary>
		/// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
		protected override void OnMouseHover( EventArgs e )
		{
			if( m_bShouldShowToolTipFirstTime && this.ShowToolTips )
			{
				m_bShouldShowToolTipFirstTime = false;
                StartShowingToolTip(DEF_TOOLTIP_TIMER_INTERVAL);
			}
		}

		protected override void OnLostFocus( EventArgs e )
		{
			base.OnLostFocus( e );

			m_bIsGroupViewItemSelecting = false;
		}


		protected override void OnMouseWheel( MouseEventArgs e )
		{
			base.OnMouseWheel( e );

			int numberOfItemsToMove = e.Delta * SystemInformation.MouseWheelScrollLines / c_iMouseWheelDeltaStep;

			bool needScroll = ( numberOfItemsToMove > 0 ) ? bUpScrlButton : bDownScrlButton;

			if( needScroll )
			{
				if( this.bFlowView )
				{
					numberOfItemsToMove *= this.nItemsPerRow;
				}
				nTopIndex -= numberOfItemsToMove;

				int nItemsPerPage = GetItemCountInPage();

				if( nTopIndex >= VisibleItems.Count - nItemsPerPage )
				{
					nTopIndex = VisibleItems.Count - nItemsPerPage;
				}

				if( nTopIndex < 0 )
				{
					nTopIndex = 0;
				}

				this.Invalidate();
				this.Update();

				if( this.bIntegratedScrolling )
				{
					IIntegratedScrollContainer scrollContainer = this.Parent as IIntegratedScrollContainer;

					if( scrollContainer != null )
					{
						scrollContainer.InvalidateUpScrollButton();
						scrollContainer.InvalidateDownScrollButton();
					}
				}
				StopShowingToolTip();
			}
		}


		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseDown"/>.
		/// </summary>
		protected override void OnMouseDown( MouseEventArgs e )
		{
			base.OnMouseDown( e );
			this.HandleMouseDown( e.Button, new Point( e.X, e.Y ) );
		}

        protected override void OnClick(EventArgs e)
        {
            this.suppressMouseHover = true;
            base.OnClick(e);
        }

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void HandleMouseDown( MouseButtons button, Point pt )
		{
			if( this.ContainsFocus == false )
				this.Focus();

			if( ( button == MouseButtons.Left ) && 
				( ( this.bDownScrlButton && this.DownScrlBtnRect.Contains( pt ) ) || ( this.bUpScrlButton & this.UpScrlBtnRect.Contains( pt ) ) ) )
				ScrollButtonsMouseHandler( MouseActions.LBtnDown, pt );

			if( this.DesignMode == true )
				return;

			int nindex = ProcessMouseOver( pt );

			if( nindex != -1 )
			{
				StopShowingToolTip();
				m_bShouldShowToolTipFirstTime = false;
			}

			// If an LBtnDown occurred over the current hovering item, then draw it in the pressed state.
			if( button == MouseButtons.Left )
			{
				m_bSelecting = true;
				this.Invalidate( Rectangle.Inflate( this.rcHighlightedItem, 1, 1 ) );

				nPrevSelected = nSelectedItem;
				rcPrevSelected = rcSelectedItem;

				if( nindex != -1 )
					nSelectedItem = nindex;

				if( nindex >= 0 && nindex < m_arrVisibleItems.Count )
					rcSelectedItem = ( (GroupViewItem)m_arrVisibleItems[nindex] ).Bounds;
				else
					rcSelectedItem = Rectangle.Empty;

				if( !this.bFlatLook )
					this.Invalidate( Rectangle.Inflate( this.rcPrevSelected, 1, 1 ) );
				this.Update();

				if( this.bAllowDragDrop == true )
				{
					this.nDragItem = nindex;
					GroupView.ptDragStart = pt;
				}
			}
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseMove"/>.
		/// </summary>
		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			Point pt = new Point( e.X, e.Y );

			if( ( this.tmrScrolling != null ) && ( this.tmrScrolling.Enabled == true ) )
			{
				if( ( this.bDownScrlButton || this.bUpScrlButton ) && 
					( ( DownScrlBtnRect.Contains( pt )==false )&&( UpScrlBtnRect.Contains( pt )==false ) ) )
				{
					this.tmrScrolling.Stop();
					ScrollButtonsMouseHandler( MouseActions.Leave, pt );
				}
			}
			else if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.bThemesEnabled )
			{
				if( ( this.bDownScrlButton && this.DownScrlBtnRect.Contains( pt ) ) || ( this.bUpScrlButton & this.UpScrlBtnRect.Contains( pt ) ) )
					this.ScrollButtonsMouseHandler( MouseActions.Move, pt );
				else if( this.scrllBtnState != ScrollButtonState.Normal )
					this.ScrollButtonsMouseHandler( MouseActions.Leave, pt );
			}

			// Get the index of the item over which the mouse is moving.
			int nindex = ProcessMouseOver( pt );

			int newHitItemIndex = nindex;

			if( newHitItemIndex != m_prevHitItemIndex )
			{
				ShowToolTip(null);

				// Set the new tooltip
				if( newHitItemIndex != -1 )
				{
					if( this.ShowToolTips && !m_bShouldShowToolTipFirstTime )
					{
						// Start showing new tooltip
						StartShowingToolTip( DEF_TOOLTIP_TIMER_INTERVAL );
					}
				}

				// Update cache
				m_prevHitItemIndex = newHitItemIndex;
			}

			//  If mouse was moved with LBtnDown while over an item, initiate a drag operation.
			if( ( this.bAllowDragDrop == true ) && ( e.Button == MouseButtons.Left ) && 
				( nindex >= 0 ) && ( nindex == this.nDragItem ) )
			{
				// If the mouse has been moved outside a 2pxl rect from the buttondown pt, commence the drag.
				Rectangle rc = new Rectangle( GroupView.ptDragStart.X-2, GroupView.ptDragStart.Y-2, 4, 4 );
				if( rc.Contains( pt ) == false )
				{
					// If in flatlook mode, erase the previous selected rect.
					if( ( this.bFlatLook == true ) && ( this.nPrevSelected >= 0 ) && ( this.nPrevSelected != nindex ) )
					{
						this.Invalidate( Rectangle.Inflate( this.rcPrevSelected, 1, 1 ) );
						this.Update();
						this.nPrevSelected = -1;
						this.rcPrevSelected = Rectangle.Empty;
					}
					if( this.bButtonView == false )
						this.nSelectedItem = -1;
					GroupViewItem item = (GroupViewItem)VisibleItems[nindex];
					DataObject dtaobject = new DataObject( GroupViewFormatName, item );
					DragDropEffects dde = DoDragDrop( dtaobject, DragDropEffects.All );

					// If the drag operation was successful, remove the GroupViewItem instance from 
					// the list and redraw.
					if( dde == DragDropEffects.Move )
					{
						int nremitem = this.VisibleItems.IndexOf( item );
						this.cllnGroupViewItems.Remove( item );
						if( ( this.bButtonView == true ) && 
							( ( this.nSelectedItem < 0 ) || ( this.nSelectedItem >= this.VisibleItems.Count ) ) )
						{
							if( this.VisibleItems.Count > 0 )
								this.nSelectedItem = this.VisibleItems.Count-1;
							else
								this.nSelectedItem = 0;
						}
					}
					this.Invalidate();
					this.Update();

					// Fire the reordered notification.
					if( ( dde == DragDropEffects.Move ) || ( dde == DragDropEffects.Copy ) )
						OnGroupViewItemsReordered( EventArgs.Empty );

					GroupView.ptDragStart = new Point( -1, -1 );
					this.nDragItem = -1;
					this.nDropItem = -1;
				}
			}
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseUp"/>.
		/// </summary>
		protected override void OnMouseUp( MouseEventArgs e )
		{
			base.OnMouseUp( e );
			this.HandleMouseUp( e.Button, new Point( e.X, e.Y ) );
            this.suppressMouseHover = false;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void HandleMouseUp( MouseButtons button, Point pt )
		{
			if( ( this.tmrScrolling != null ) && ( this.tmrScrolling.Enabled == true ) )
			{
				this.tmrScrolling.Stop();
				// It's possible that the cursor hurriedly dragged out the control during a scroll operation.
				// If so, redraw the scroll buttons
				if( ( this.ClientRectangle.Contains( pt ) == false ) && ( this.bDownScrlButton || this.bUpScrlButton ) )
					this.ScrollButtonsMouseHandler( MouseActions.Leave, pt );
			}

			switch( button )
			{
				case MouseButtons.Left:
				if( ( this.bDownScrlButton & this.DownScrlBtnRect.Contains( pt ) ) || ( this.bUpScrlButton & this.UpScrlBtnRect.Contains( pt ) ) )
					this.ScrollButtonsMouseHandler( MouseActions.LBtnUp, pt );

				if( this.DesignMode == true )
					return;

				int nindex = ProcessMouseOver( pt );
				bool raiseEvent;

				if( nindex != -1 )
				{
					raiseEvent = ( nindex != nPrevSelected );

					if( this.bFlatLook )
					{
						if( this.rcPrevSelected != Rectangle.Empty )
							this.Invalidate( Rectangle.Inflate( this.rcPrevSelected, 1, 1 ) );
						else if( nindex != nSelectedItem && this.rcSelectedItem != Rectangle.Empty )
							this.Invalidate( Rectangle.Inflate( this.rcSelectedItem, 1, 1 ) );
					}

					nPrevSelected = nSelectedItem;
					rcPrevSelected = rcSelectedItem;
					nSelectedItem = nindex;
					rcSelectedItem = ( (GroupViewItem)m_arrVisibleItems[nindex] ).Bounds;

					// If a previous selection is visible, first erase it.				
					if( this.nPrevSelected >= 0 )
					{
						this.Invalidate( Rectangle.Inflate( this.rcPrevSelected, 1, 1 ) );
						this.Update();
					}
				}
				else
				{
					//when index == -1 then selected item is previous and no SelectedItem should fire
					raiseEvent = false;

					nSelectedItem = nPrevSelected;
					rcSelectedItem = rcPrevSelected;
				}

				m_bSelecting = false;

				// Draw hover state if nHighlightedItem is a valid index.
				this.Invalidate( Rectangle.Inflate( this.rcSelectedItem, 1, 1 ) );
				this.Update();

				// Fire GroupViewItemSelected notification.
				if( this.nSelectedItem != -1 && raiseEvent )
				{
					this.OnGroupViewItemSelected( EventArgs.Empty );
				}


				break;
				case MouseButtons.Right:
				int nhitindex = ProcessMouseOver( pt );
				this.nContextMenuItem = this.nHighlightedItem;
				OnShowContextMenu( EventArgs.Empty );
				break;
				default:
				break;
			}
			if( ( GroupView.ptDragStart.X != -1 ) || ( GroupView.ptDragStart.Y != -1 ) )
			{
				this.nDragItem = -1;
				this.nDropItem = -1;
				GroupView.ptDragStart = new Point( -1, -1 );
			}
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseLeave"/>.
		/// </summary>
		protected override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );

			Point pt = new Point( -1, -1 );

			if( ( this.tmrScrolling != null ) && ( this.tmrScrolling.Enabled == true ) )
				this.tmrScrolling.Stop();

			// If the tooltip control is visible, then treat the mouse as though it is moving 
			// over this control.
			if( this.ctrlToolTip.Visible == true )
				pt = this.PointToClient( Cursor.Position );
			ProcessMouseOver( pt );

			if( this.bFlatLook && this.rcPrevSelected != Rectangle.Empty )
				this.Invalidate( Rectangle.Inflate( this.rcPrevSelected, 1, 1 ) );

			StopShowingToolTip();
		}

		protected override bool ProcessCmdKey( ref Message msg, Keys keydata )
		{
			if( ( msg.Msg == 0x0100/*WM_KEYDOWN*/) && ( this.GroupViewItems.Count > 0 ) && ( this.bButtonView == true ) )
			{
				Keys correctedKeydata = keydata;

				if( this.Horizontal && ( correctedKeydata & Keys.Control ) == 0 )
				{
					bool bIsMirrored = this.GetIsMirrored();
					if( correctedKeydata == Keys.Left )
					{
						correctedKeydata = bIsMirrored ?  Keys.Down : Keys.Up;
					}
					else if( correctedKeydata == Keys.Right )
					{
						correctedKeydata = bIsMirrored ? Keys.Up : Keys.Down;
					}
					else if( correctedKeydata == Keys.Up )
					{
						correctedKeydata = Keys.Left;
					}
					else if( correctedKeydata == Keys.Down )
					{
						correctedKeydata = Keys.Right;
					}
				}

				switch( correctedKeydata )
				{
					case Keys.Up:
					if( ( correctedKeydata & Keys.Control ) == 0 )	// GroupBar parent processes Ctrl + Up/Down keys.
					{
						if( this.bFlowView == false )
						{
							if( this.nSelectedItem > 0 )
							{
								this.BringItemIntoView( this.nSelectedItem-1 );
								this.SelectedItemInternal -= 1;
							}
						}
						else
						{
							int newitem = this.nSelectedItem - this.nItemsPerRow;
							if( newitem >= 0 )
							{
								this.BringItemIntoView( newitem );
								this.SelectedItemInternal = newitem;
							}
						}
						return true;
					}
					break;
					case Keys.Down:
					if( ( correctedKeydata & Keys.Control ) == 0 )	// GroupBar parent processes of Ctrl + Up/Down keys.
					{
						if( this.bFlowView == false )
						{
							if( this.nSelectedItem < this.GroupViewItems.Count-1 )
							{
								this.BringItemIntoView( this.nSelectedItem+1 );
								this.SelectedItemInternal += 1;
							}
						}
						else
						{
							int newitem = this.nSelectedItem + this.nItemsPerRow;
							if( newitem < this.GroupViewItems.Count )
							{
								this.BringItemIntoView( newitem );
								this.SelectedItemInternal = newitem;
							}
						}
						return true;
					}
					break;
					case Keys.Left:
					if( this.bFlowView == true )
					{
						if( this.nSelectedItem > 0 )
						{
							this.BringItemIntoView( this.nSelectedItem-1 );
							this.SelectedItemInternal -= 1;
						}
						return true;
					}
					break;
					case Keys.Right:
					if( this.bFlowView == true )
					{
						if( this.nSelectedItem < this.GroupViewItems.Count-1 )
						{
							this.BringItemIntoView( this.nSelectedItem+1 );
							this.SelectedItemInternal += 1;
						}
						return true;
					}
					break;
					case Keys.PageUp:
					int pgupitemsperpage = this.CalculateItemsPerPage();
					int topitem = this.nSelectedItem - pgupitemsperpage;
					if( topitem < 0 )
						topitem = 0;
					this.BringItemIntoView( topitem );
					this.SelectedItemInternal = topitem;
					return true;
					case Keys.PageDown:
					int pgdownitemsperpage = this.CalculateItemsPerPage();
					int bottomitem = this.nSelectedItem + pgdownitemsperpage;
					if( bottomitem >= this.GroupViewItems.Count )
						bottomitem = this.GroupViewItems.Count-1;
					this.BringItemIntoView( bottomitem );
					this.SelectedItemInternal = bottomitem;
					return true;
					case Keys.Home:
					this.BringItemIntoView( 0 );
					this.SelectedItemInternal = 0;
					return true;
					case Keys.End:
					int lastitem = this.GroupViewItems.Count-1;
					this.BringItemIntoView( lastitem );
					this.SelectedItemInternal = lastitem;
					return true;
				}
			}
			return base.ProcessCmdKey( ref msg, keydata );
		}


		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.SetBoundsCore"/>.
		/// </summary>
		protected override void SetBoundsCore( int x, int y, int width, int height, BoundsSpecified specified )
		{
			Rectangle rcold = this.ClientRectangle;
			base.SetBoundsCore( x, y, width, height, specified );
			// Redraw the client area.
			if( this.Horizontal )
			{
				if( rcold.Height != this.ClientRectangle.Height )
				{
					if( this.bFlowView == true )
						CalculateItemsPerRow();
					this.Invalidate();
					return;
				}
			}
			if( rcold.Width != this.ClientRectangle.Width )
			{
				if( this.bFlowView == true )
				{
					CalculateItemsPerRow();

					int nrowsreqd = (int)Math.Ceiling( (float)this.VisibleItems.Count / (float)this.nItemsPerRow );
					int nrowheight = 0;

					if( this.bSmallImageView == false )
						nrowheight = this.nImageSpacing + 32 + this.nImageSpacing + 1;
					else
						nrowheight = this.nImageSpacing + 16 + this.nImageSpacing + 1;

					if( this.ShowFlowViewItemText )
					{
						SizeF strSize = this.MeasureText( 0, this.ClientRectangle );

						nrowheight += Size.Ceiling( strSize ).Height + nTextSpacing;
					}

					if( this.nTopIndex != 0 )
					{
						int itemsCount = 0;
						int visibleRows = 0;

						do
						{
							itemsCount = this.VisibleItems.Count - this.nTopIndex;
							visibleRows = (int)Math.Ceiling( (float)itemsCount / (float)this.nItemsPerRow );

							if( visibleRows * nrowheight < this.ClientRectangle.Height - nrowheight )
							{
								this.nTopIndex--;
							}
						}
						while( this.nTopIndex != 0 && visibleRows * nrowheight < this.ClientRectangle.Height - nrowheight );
					}
				}

				this.Invalidate();
				return;
			}
			// Redraw the scroll bars for changes in height.
			Rectangle rcclient = this.ClientRectangle;
			if( rcold.Height != rcclient.Height )
			{
				Rectangle rcscroll = new Rectangle( GetScrollButtonLeft(),
					rcclient.Top, nScrllBttnOffset, rcclient.Height );
				this.Invalidate( rcscroll );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void CalculateItemsPerRow()
		{
			// Recalculate the nItemsPerRow value.
			int width = this.ClientRectangle.Width;

			if( this.bIntegratedScrolling == false )
				width -= nScrllBttnOffset;

			int nitemwidth = 2 * this.nImageSpacing + this.nItemXSpacing + 1;

			if( !this.ShowFlowViewItemText )
			{
				nitemwidth += this.bSmallImageView ? 16 : 32;

				this.nItemsPerRow = (int)Math.Floor( (float)( ( this.Horizontal ) ? this.ClientRectangle.Height : width
					+ this.nItemXSpacing ) / (float)nitemwidth );
			}
			else
			{
				if( this.FlowViewItemTextLength < defLargeImgSize.Width )
					nitemwidth += defLargeImgSize.Width;
				else
					nitemwidth += this.FlowViewItemTextLength;

				this.nItemsPerRow = (int)Math.Floor( (float)( ( this.Horizontal ) ? this.ClientRectangle.Height : width
					+ this.nItemXSpacing ) / (float)nitemwidth );
			}

			if( this.nItemsPerRow <= 0 )
				this.nItemsPerRow = 1;
		}

		// Calculates the items that can fit into the control's client rectangle - used for Accessibility.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int CalculateItemsPerPage()
		{
			int nfullitems = 0;	// Represents the items that can fit into the control's client rectangle.
			if( this.bFlowView == false )
			{
				Rectangle rcitem = Rectangle.Empty;
				for( int i=this.nTopIndex; i<this.VisibleItems.Count; i++ )
				{
					this.SetNonFlowItemBounds( i, ref rcitem );
					if( rcitem.Bottom > 
						( ( this.Horizontal ) ? this.ClientRectangle.Right : this.ClientRectangle.Bottom ) )
						break;
					nfullitems++;
					rcitem.Y = rcitem.Bottom;
				}
			}
			else
			{
				int nitemheight = 0;
				if( this.bSmallImageView == false )
					nitemheight = this.nImageSpacing+32+this.nImageSpacing+1;
				else
					nitemheight = this.nImageSpacing+16+this.nImageSpacing+1;
				nfullitems = ( ClientRectangle.Height/( nitemheight+1 ) ) * this.nItemsPerRow;
			}
			return nfullitems;
		}

		/// <summary>
		/// Overloaded. Returns the GroupViewItem at the specified point in client coordinates.
		/// </summary>
		/// <param name="x">X - coordinate of the item.</param>
		/// <param name="y">Y- coordinate of the item.</param>
		/// <returns>GroupViewItem, whose area contains specified point; null, if nothing is found.</returns>
		public GroupViewItem PointToItem( int x, int y )
		{
			return PointToItem( new Point( x, y ) );
		}

		/// <summary>
		/// Returns GroupViewItem at the specified point in client coordinates.
		/// </summary>
		/// <param name="pt">Point where the GroupViewItem is located.</param>
		/// <returns>GroupViewItem, whose area contains specified point; Null, if nothing is found.</returns>
		public GroupViewItem PointToItem( Point pt )
		{
			GroupViewItem foundItem = null;

			if( VisibleItems != null && VisibleItems.Count > 0 )
			{
				int foundItemIndex = ProcessMouseOver( pt );
				if( foundItemIndex >= 0 && foundItemIndex < VisibleItems.Count )
				{
					foundItem = VisibleItems[foundItemIndex] as GroupViewItem;
				}
			}

			return foundItem;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DrawAllItems( Graphics gph )
		{
			Rectangle rc = Rectangle.Empty;

			for( int i = this.nTopIndex; i < VisibleItems.Count; i++ )
			{
				rc = ( (GroupViewItem)VisibleItems[i] ).Bounds;
				bool drawed = false;

				ItemState state = ItemState.Normal;

				if( this.nHighlightedItem == i )
				{
					this.rcHighlightedItem = rc;
					state = ( m_bSelecting ) ? ItemState.Selecting : ItemState.Highlight;
					this.DrawItemHighlight( gph, this.nHighlightedItem, this.rcHighlightedItem, state );
					drawed = true;
				}

				if( !drawed && this.nSelectedItem == i )
				{
					this.rcSelectedItem = rc;
					this.DrawItemHighlight( gph, this.nSelectedItem, this.rcSelectedItem, ItemState.Selected );
					drawed = true;
				}

				if( !drawed )
				{
					this.DrawItem( gph, i, rc, state );
				}

				if( rc.Y >= this.ClientRectangle.Bottom )
					break;
			}

			if( this.bFlowView == false )
				this.DrawNonFlowScrollButtons( gph );
			else
				this.DrawFlowScrollButtons( gph );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DrawItem( Graphics gph, int nindex, Rectangle rc, ItemState state )
		{
			if( ( rc.Width <= 0 ) || ( rc.Height <= 0 ) )
				return;

			if( ( nindex < 0 ) || ( nindex >= this.VisibleItems.Count ) )
			{
				Debug.Assert( false, "Invalid index." );
				return;
			}
            
			if( this.Horizontal )
			{
				int temp = rc.Width;
				rc.Width = rc.Height;
				rc.Height = temp;

				temp = rc.X;
				rc.X = rc.Y;
				rc.Y = temp;
			}

			// Get the icon from the image list and draw it.
			GroupViewItem item = (GroupViewItem)VisibleItems[nindex];
			Rectangle rcicon = Rectangle.Empty;

			bool bIsMirrored = GetIsMirrored();

			ImageList ilImgList;
			Size sizeImgList;
			int nIconLeft = 0, nIconTop = 0;
            if (this.bSmallImageView == false)
            {
                ilImgList = this.ilLarge;
                sizeImgList = (ilImgList != null) ? ilImgList.ImageSize : defLargeImgSize;

                // Center the icon within the hilight rect.
                if (this.bFlowView == false)
                {
                    nIconLeft = rc.X + (rc.Width - sizeImgList.Width) / 2;
                    if (!bIsMirrored)
                        nIconLeft += 1;
                    nIconTop = rc.Top + this.nImageSpacing;
                }
                else
                {
                    nIconLeft = rc.X + (rc.Width - sizeImgList.Width) / 2;

                    if (!this.ShowFlowViewItemText)
                        nIconTop = rc.Top + (rc.Height - sizeImgList.Height) / 2;
                    else
                        nIconTop = rc.Top+ this.nImageSpacing;
                }
            }
            else
            {
                ilImgList = this.ilSmall;
                sizeImgList = (ilImgList != null) ? ilImgList.ImageSize : defSmallImgSize;

                if (this.bFlowView == false)
                {
                    int nAddSpace = this.nItemXSpacing + 1;
                    nIconLeft = (bIsMirrored && !this.Horizontal) ? rc.Right - sizeImgList.Width - nAddSpace : rc.X + nAddSpace;
                    nIconTop = rc.Top + (rc.Height - sizeImgList.Height) / 2;
                }
                else
                {
                    nIconLeft = rc.X + (rc.Width - sizeImgList.Width) / 2;

                    if (!this.ShowFlowViewItemText)
                        nIconTop = rc.Top + (rc.Height - sizeImgList.Height) / 2;
                    else
                        nIconTop = rc.Top + this.nImageSpacing;
                }
            }

			rcicon = new Rectangle( nIconLeft, nIconTop, sizeImgList.Width, sizeImgList.Height );

			Point ptOffset = Point.Empty;

			if( ( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.bThemesEnabled ) == false )
			{
				if( ( nindex == this.nHighlightedItem ) && ( nindex == this.nSelectedItem ) )
				{
					ptOffset = this.ptSelectingImageOffset;
				}
				else if( nindex == this.nSelectedItem )
				{
					ptOffset = this.ptSelectedImageOffset;
				}
				else if( nindex == this.nHighlightedItem )
				{
					ptOffset = ( nindex == this.nSelectedItem ) ? 
						this.ptSelectedHighlightImageOffset : this.ptHighlightImageOffset;
				}


				bool bNeedFillBack = ( this.BackgroundImage == null && ilImgList != null 
					&& item.ImageIndex >= 0	&& item.ImageIndex < ilImgList.Images.Count 
					&& this.bFlatLook == true );

				if( bNeedFillBack )
				{
					DrawingUtils.DrawImageViaImageList( gph, ilImgList, item.ImageIndex, rcicon );

				}
			}

			if( bIsMirrored && !this.Horizontal ) ptOffset.X = -ptOffset.X;
			rcicon.Offset( ptOffset );

			if( ( ilImgList != null ) && ( item.ImageIndex >= 0 ) && ( item.ImageIndex < ilImgList.Images.Count ) )
			{
				if( this.Horizontal )
				{
					int temp = rcicon.Width;
					rcicon.Width = rcicon.Height;
					rcicon.Height = temp;

					temp = rcicon.X;
					rcicon.X = rcicon.Y;
					rcicon.Y = temp;
				}

				bool itemEnabled = item.Enabled && this.Enabled;
				if( itemEnabled == true )
				{
					DrawingUtils.DrawImageViaImageList( gph, ilImgList, item.ImageIndex, rcicon );
				}
				else
					ControlPaint.DrawImageDisabled( gph, ilImgList.Images[item.ImageIndex], rcicon.X, rcicon.Y, SystemColors.Control );
			}

			if( !this.bFlowView || this.ShowFlowViewItemText )
			{
				Rectangle rctext = this.GetAdjustedTextBounds( nindex, rc );

				ptOffset = Point.Empty;

				if( state == ItemState.Highlight )
				{
					ptOffset = ( nindex == this.nSelectedItem ) ? this.ptSelectedHighlightTextOffset : this.ptHighlightTextOffset;
				}
				else if( state == ItemState.Selecting )
					ptOffset = this.ptSelectingTextOffset;
				else if( state == ItemState.Selected )
					ptOffset = this.ptSelectedTextOffset;

				if( bIsMirrored && !this.Horizontal ) ptOffset.X = -ptOffset.X;
				rctext.Offset( ptOffset );

				this.DrawText( gph, nindex, rctext, state );
			}
		}

		/// <summary>
		/// Draws the highlighting of GroupViewitem.
		/// </summary>
		/// <param name="gph">A <see cref="System.Drawing.Graphics"/> object.</param>
		/// <param name="nindex">The index of the groupViewItem.</param>
		/// <param name="rcitem">A <see cref="System.Drawing.Rectangle"/> value specifying the GroupViewItem bounds.</param>
		/// <param name="state">The state of the item.</param>
		protected internal virtual void DrawItemHighlight( Graphics gph, int nindex, Rectangle rcitem, ItemState state )
		{
			if( ( rcitem.Width <= 0 ) || ( rcitem.Height <= 0 ) )
				return;

			if( ( nindex < 0 ) || ( nindex >= this.VisibleItems.Count ) )
			{
				Debug.Assert( false, "Invalid index." );
				return;
			}

			PaintEventArgs pe = new PaintEventArgs( gph, Rectangle.Inflate( rcitem, 1, 1 ) );

			if( ( this.bHighlightImage == true ) || ( this.bHighlightText == true ) )
			{
				bool bIsMirrored = GetIsMirrored();

				if( null != this.tdToolbar )
				{
					this.tdToolbar.DrawMirrored = bIsMirrored;
				}

				Rectangle rchighlight = this.GetAdjustedHighlightBounds( nindex, rcitem );
				Rectangle rcitemfill = new Rectangle( rchighlight.Left + 1, rchighlight.Top+1,
					rchighlight.Width - 1, rchighlight.Height-1 );
				pe = new PaintEventArgs( gph, rcitemfill );

				Pen lightpen = null;
				Pen darkpen = null;
				Color fillColor = Color.Empty;
				if( ( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.bThemesEnabled ) == false )
				{
					if( this.bFlatLook == true )
					{
						if( this.brHighlightItem is SolidBrush )
						{
							Color clrborder = ControlPaint.Dark( ( this.brHighlightItem as SolidBrush ).Color );
							lightpen = new Pen( clrborder, 1 );
							darkpen = new Pen( clrborder, 1 );
						}
						else
						{
							lightpen = new Pen( SystemColors.Highlight, 1 );
							darkpen = new Pen( SystemColors.Highlight, 1 );
						}
					}
					else
					{
						lightpen = new Pen( SystemColors.ControlLightLight, 1 );
						darkpen = new Pen( SystemColors.ControlDarkDark, 1 );
					}
				}

				switch( state )
				{
					case ItemState.Highlight:	// Draw the hovering effect.
					if( nindex == this.nSelectedItem )
					{
						if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.bThemesEnabled )
						{
							this.tdToolbar.DrawThemeBackground( gph, ThemeParts.TP_BUTTON, ThemeStates.TS_HOTCHECKED, rcitemfill );
						}
						else
						{
							gph.DrawLine( bIsMirrored ? lightpen : darkpen,
								new Point( rchighlight.Left, rchighlight.Bottom ), new Point( rchighlight.Left, rchighlight.Top ) );
							gph.DrawLine( darkpen, new Point( rchighlight.Left, rchighlight.Top ), new Point( rchighlight.Right, rchighlight.Top ) );
							gph.DrawLine( bIsMirrored ? darkpen : lightpen,
								new Point( rchighlight.Right, rchighlight.Top ), new Point( rchighlight.Right, rchighlight.Bottom ) );
							gph.DrawLine( lightpen, new Point( rchighlight.Left, rchighlight.Bottom ), new Point( rchighlight.Right, rchighlight.Bottom ) );
							fillColor = this.SelectedHighlightItemColor;
						}
					}
					else
					{
						if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.bThemesEnabled )
						{
							this.tdToolbar.DrawThemeBackground( gph, ThemeParts.TP_BUTTON, ThemeStates.TS_HOT, rcitemfill );
						}
						else
						{
							gph.DrawLine( bIsMirrored ? darkpen : lightpen,
								new Point( rchighlight.Left, rchighlight.Bottom ), new Point( rchighlight.Left, rchighlight.Top ) );
							gph.DrawLine( lightpen, new Point( rchighlight.Left, rchighlight.Top ), new Point( rchighlight.Right, rchighlight.Top ) );
							gph.DrawLine( bIsMirrored ? lightpen : darkpen,
								new Point( rchighlight.Right, rchighlight.Top ), new Point( rchighlight.Right, rchighlight.Bottom ) );
							gph.DrawLine( darkpen, new Point( rchighlight.Left, rchighlight.Bottom ), new Point( rchighlight.Right, rchighlight.Bottom ) );
							fillColor = this.HighlightItemColor;
						}
					}
					break;
					case ItemState.Selected:	// Draw the selected effect.
					if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.bThemesEnabled )
					{
						this.tdToolbar.DrawThemeBackground( gph, ThemeParts.TP_BUTTON, ThemeStates.TS_CHECKED, rcitemfill );
					}
					else
					{
						gph.DrawLine( bIsMirrored ? lightpen : darkpen,
							new Point( rchighlight.Left, rchighlight.Bottom ), new Point( rchighlight.Left, rchighlight.Top ) );
						gph.DrawLine( darkpen, new Point( rchighlight.Left, rchighlight.Top ), new Point( rchighlight.Right, rchighlight.Top ) );
						gph.DrawLine( bIsMirrored ? darkpen : lightpen,
							new Point( rchighlight.Right, rchighlight.Top ), new Point( rchighlight.Right, rchighlight.Bottom ) );
						gph.DrawLine( lightpen, new Point( rchighlight.Left, rchighlight.Bottom ), new Point( rchighlight.Right, rchighlight.Bottom ) );
						fillColor = this.SelectedItemColor;
					}
					break;
					case ItemState.Selecting:	// Draw the in-selection effect.
					if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.bThemesEnabled )
					{
						this.tdToolbar.DrawThemeBackground( gph, ThemeParts.TP_BUTTON, ThemeStates.TS_PRESSED, rcitemfill );
					}
					else
					{
						gph.DrawLine( bIsMirrored ? lightpen : darkpen,
							new Point( rchighlight.Left, rchighlight.Bottom ), new Point( rchighlight.Left, rchighlight.Top ) );
						gph.DrawLine( darkpen, new Point( rchighlight.Left, rchighlight.Top ), new Point( rchighlight.Right, rchighlight.Top ) );
						gph.DrawLine( bIsMirrored ? darkpen : lightpen,
							new Point( rchighlight.Right, rchighlight.Top ), new Point( rchighlight.Right, rchighlight.Bottom ) );
						gph.DrawLine( lightpen, new Point( rchighlight.Left, rchighlight.Bottom ), new Point( rchighlight.Right, rchighlight.Bottom ) );
						fillColor = this.SelectingItemColor;
					}
					break;
					default:	// case ItemState.ImageOnly
					break;
				}
				if( ( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.bThemesEnabled ) == false )
				{
					lightpen.Dispose();
					darkpen.Dispose();
				}
				if( !fillColor.IsEmpty )
				{
					rchighlight.Offset( 1, 1 );
					rchighlight.Height--;
					rchighlight.Width--;
                    using(Brush brush=new SolidBrush( fillColor ))
                        gph.FillRectangle(brush, rchighlight);
				}
			}

			this.DrawItem( gph, nindex, rcitem, state );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DrawText( Graphics gph, int nindex, Rectangle rc, ItemState state )
		{
			if( ( rc.Width <= 0 ) || ( rc.Height <= 0 ) )
				return;

			if( ( nindex < 0 ) || ( nindex >= this.VisibleItems.Count ) )
			{
				Debug.Assert( false, "Invalid index." );
				return;
			}

			GroupViewItem item = (GroupViewItem)this.VisibleItems[nindex];
			// Do not draw the string if this item is being edited.
			if( nindex != this.nRenameItem )
			{
				if( ( this.bTextWrap == true ) && ( this.bSmallImageView == true ) )
				{
					rc.Y += this.nImageSpacing;
					rc.Height -= this.nImageSpacing;
				}

				Matrix oldMatrix = gph.Transform;
				if( this.Horizontal )
				{
					int temp = rc.X;
					rc.X = rc.Y;
					rc.Y = temp;

					Matrix newMatrix = new Matrix();

					newMatrix.RotateAt( 90, new PointF( rc.X + rc.Height / 2, rc.Y + rc.Height / 2 ) );

					gph.Transform = newMatrix;
				}

				bool itemEnabled = item.Enabled && this.Enabled;
				if( itemEnabled == true )
				{
					Font textfont = Syncfusion.Drawing.FontUtil.CreateFont( this.Font,
						( nindex == this.nHighlightedItem ) && ( this.bTextUnderline == true ) ? FontStyle.Underline : this.Font.Style );

					Brush textbrush;
					if( ( nindex == this.nHighlightedItem ) && ( nindex == this.nSelectedItem ) )
					{
						if( state == ItemState.Selecting )
							textbrush = new SolidBrush( this.clrSelectingText );
						else
							textbrush = new SolidBrush( this.clrSelectedHighlightText );
					}
					else if( nindex == this.nSelectedItem )
						textbrush = new SolidBrush( this.clrSelectedText );
					else if( nindex == this.nHighlightedItem )
						textbrush = new SolidBrush( this.clrHighlightText );
					else
						textbrush = new SolidBrush( this.ForeColor );

					StringFormat sf = new StringFormat( StringFormatFlags.LineLimit );
					if( ( this.ctrlToolTip.Visible == true ) && ( nindex == this.nHighlightedItem ) )
					{
						sf.Alignment = StringAlignment.Near;
						sf.FormatFlags |= StringFormatFlags.NoWrap;
						sf.Trimming = StringTrimming.None;
						sf.LineAlignment = StringAlignment.Center;
					}
					else
					{
						if( this.bSmallImageView == true )
						{
							sf.Alignment = StringAlignment.Near;
							SizeF szfulltext = gph.MeasureString( item.Text, textfont );
						}
						else
							sf.Alignment = StringAlignment.Center;
						if( this.bTextWrap == false )
						{
							sf.Trimming = StringTrimming.EllipsisCharacter;
							sf.FormatFlags |= StringFormatFlags.NoWrap;
							sf.LineAlignment = StringAlignment.Center;
						}
						else
						{
							sf.Trimming = StringTrimming.EllipsisWord;
						}
					}

					if( GetIsMirrored() && !this.Horizontal )
					{
						sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
					}

					gph.TextRenderingHint = TextRenderingHint.SystemDefault;

					gph.DrawString( item.Text, textfont, textbrush, rc, sf );
                    textbrush.Dispose();
					textfont.Dispose();
					sf.Dispose();
				}
				else // Disabled text.
				{
					StringFormat sf = new StringFormat( StringFormatFlags.LineLimit );
					sf.Trimming = StringTrimming.EllipsisWord;
					if( this.bSmallImageView == true )
						sf.Alignment = StringAlignment.Near;
					else
						sf.Alignment = StringAlignment.Center;
					if( this.bTextWrap == false )
					{
						sf.FormatFlags |= StringFormatFlags.NoWrap;
						sf.LineAlignment = StringAlignment.Center;
					}

					if( GetIsMirrored() && !this.Horizontal )
					{
						sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
					}
					ControlPaint.DrawStringDisabled( gph, item.Text, this.Font, SystemColors.Control, rc, sf );
					sf.Dispose();
				}

				gph.Transform = oldMatrix;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void SetNonFlowItemBounds( int nindex, ref Rectangle rchilight )
		{
			bool bIsMirrored = GetIsMirrored();
			Rectangle rect = this.ClientRectangle;

			if( this.Horizontal )
			{
				int temp = rect.Width;
				rect.Width = rect.Height;
				rect.Height = temp;
			}

			GroupViewItem item = this.VisibleItems[nindex] as GroupViewItem;
			if( this.bSmallImageView == false )
			{
				rchilight.Y += this.nItemYSpacing;
				if( this.bIntegratedScrolling == false )
				{
					int nAddSpace = 1;
					rchilight.X = rect.Left + nScrllBttnOffset + (/*bIsMirrored ? nAddSpace :*/ 0 );
					rchilight.Width = rect.Width - ( 2*nScrllBttnOffset + nAddSpace );
				}
				else
				{
					rchilight.X = rect.Left+1;
					rchilight.Width = rect.Width-2;
				}

				SizeF szlabel = this.MeasureText( nindex, rchilight );
                if ((this.ilLarge != null) && (item.ImageIndex >= 0) && (item.ImageIndex < this.ilLarge.Images.Count))
                {
                    rchilight.Height = this.nImageSpacing + 32 + this.nTextSpacing + (int)szlabel.Height + this.nImageSpacing;
                }
                else
                    rchilight.Height = this.nImageSpacing + (int)szlabel.Height + this.nImageSpacing;
			}
			else	// SmallIcon View.
			{
				rchilight.Y += this.nItemYSpacing;
				if( this.bIntegratedScrolling == false )
				{
					rchilight.Width = rect.Width - ( 2+nScrllBttnOffset );
					int nAddSpace = 1;
					if( bIsMirrored && !this.Horizontal )
					{
						rchilight.X = rect.Right - rchilight.Width - nAddSpace;
					}
					else
					{
						rchilight.X = rect.Left + nAddSpace;
					}
				}
				else
				{
					rchilight.X = rect.Left+1;
					rchilight.Width = rect.Width-2;
				}

				Rectangle rctext = Rectangle.Empty;
				rctext.Width = rect.Width - ( 2+this.nItemXSpacing );
				if( this.bIntegratedScrolling == false )
					rctext.Width -= nScrllBttnOffset;

				int nAddTextSpace = 1 + this.nItemXSpacing;
				if( bIsMirrored && !this.Horizontal )
				{
					rctext.X = rect.Right - rctext.Width - nAddTextSpace;
				}
				else
				{
					rctext.X = rect.Left + nAddTextSpace;
				}

				if( ( this.ilSmall != null ) && ( item.ImageIndex >= 0 ) && ( item.ImageIndex < this.ilSmall.Images.Count ) )
				{
					rctext.Width -= 16+this.nTextSpacing;
					SizeF szlabel = this.MeasureText( nindex, rctext );
					int ntextheight = (int)szlabel.Height;
					rchilight.Height = this.nImageSpacing + ( ( 16>ntextheight )?16:ntextheight ) + this.nImageSpacing;
				}
				else
				{
					SizeF szlabel = this.MeasureText( nindex, rctext );
					int ntextheight = (int)szlabel.Height;
					rchilight.Height = this.nImageSpacing + ntextheight + this.nImageSpacing;
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void SetFlowItemBounds( int nindex, ref Rectangle rchighlight )
		{
			if( ( nindex < 0 ) || ( nindex >= this.VisibleItems.Count ) )
			{
				Debug.Assert( false, "Error - Invalid item." );
				return;
			}

			nindex -= this.nTopIndex;
			int nrowindex = (int)Math.Floor( (float)nindex / (float)this.nItemsPerRow );
			int ncolindex = nindex - ( nrowindex * this.nItemsPerRow );

			int ncx = 2 * this.nImageSpacing + this.nItemXSpacing + 1;
			int ncy = 2 * this.nImageSpacing + this.nItemYSpacing + 1;

			if( this.Horizontal )
			{
				int temp = nrowindex;
				nrowindex = ncolindex;
				ncolindex = temp;
			}

			if( !this.ShowFlowViewItemText )
			{
				if( this.bSmallImageView == false )
				{
					ncx += 32;
					ncy += 32;
				}
				else
				{
					ncx += 16;
					ncy += 16;
				}
			}
			else
			{
				int imageWidth = 0;
				if( this.bSmallImageView == false )
				{
					imageWidth = 32;
					ncy += 32;
				}
				else
				{
					imageWidth = 16;
					ncy += 16;
				}

				if( this.FlowViewItemTextLength < imageWidth )
					ncx += imageWidth;
				else
					ncx += this.FlowViewItemTextLength;

				ncy += this.ItemTextHeight + nTextSpacing;
			}

			rchighlight.Width = ncx - this.nItemXSpacing - 1;
			rchighlight.Height = ncy - this.nItemYSpacing - 1;

			int nXOffset = 0;

			if( this.Horizontal )
				nXOffset = 1 + ( ncolindex * ncy );
			else
				nXOffset = 1 + ( ncolindex * ncx );

			bool bIsMirrored = GetIsMirrored();
			if( bIsMirrored )
			{
				rchighlight.X = this.ClientRectangle.Right - nXOffset - rchighlight.Width;
			}
			else
			{
				rchighlight.X = nXOffset;
			}

			if( this.Horizontal )
				rchighlight.Y = 1 + ( nrowindex * ncx );
			else
				rchighlight.Y = 1 + ( nrowindex * ncy );

			if( this.Horizontal )
			{
				int temp = rchighlight.X;
				rchighlight.X = rchighlight.Y;
				rchighlight.Y = temp;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected Rectangle GetAdjustedHighlightBounds( int nindex, Rectangle rchighlight )
		{
			if( !( this.bFlowView && !this.ShowFlowViewItemText && this.HighlightImage ) )
			{
				if( this.Horizontal )
				{
					int temp = rchighlight.Width;
					rchighlight.Width = rchighlight.Height;
					rchighlight.Height = temp;

					temp = rchighlight.X;
					rchighlight.X = rchighlight.Y;
					rchighlight.Y = temp;
				}

				ImageList imgList = ( this.bSmallImageView ) ? this.ilSmall : this.ilLarge;
				GroupViewItem item = this.VisibleItems[nindex] as GroupViewItem;
				bool IsImgShown = ( imgList != null && item.ImageIndex >= 0 && item.ImageIndex < imgList.Images.Count )
											|| ( this.FlowView && !this.ShowFlowViewItemText );

				bool IsTextShown = !this.FlowView || this.ShowFlowViewItemText;
				bool bIsMirrored = GetIsMirrored() && !this.Horizontal;
				Size imgSize = ( imgList != null ) ? imgList.ImageSize :
					( bSmallImageView ) ? defSmallImgSize : defLargeImgSize;

				if( ( !this.bHighlightImage && !IsTextShown ) || ( !this.bHighlightText && !IsImgShown )
					|| ( !this.bHighlightImage && !this.bHighlightText ) )
				{
					rchighlight = Rectangle.Empty;
				}
				else if( !this.HighlightText )
				{
					if( !this.bSmallImageView )
					{
						rchighlight.Y += this.nImageSpacing - 2;

						if( !bFlowView )
						{
							if( this.Horizontal )
								rchighlight.X = ( this.ClientRectangle.Height - imgSize.Width ) / 2 - 2;
							else
								rchighlight.X = ( this.ClientRectangle.Width - imgSize.Width ) / 2 - 2;
						}
						else if( this.ShowFlowViewItemText )
							rchighlight.X += ( rchighlight.Width - imgSize.Width ) / 2 - 2;

						rchighlight.Width = imgSize.Width + 4;
						rchighlight.Height = imgSize.Height + 4;
					}
					else
					{
						if( !bFlowView )
						{
							int nWidth = imgSize.Width + 4;

							int nAddSpace = this.nItemXSpacing;
							if( bIsMirrored )
							{
								rchighlight.X = ( rchighlight.Right - nWidth - nAddSpace );
							}
							else
							{
								rchighlight.X += nAddSpace;
							}
							rchighlight.Width = nWidth;

							rchighlight.Y += ( rchighlight.Height - imgSize.Height ) / 2 - 2;
							rchighlight.Height = imgSize.Height + 4;
						}
						else
						{
							if( this.ShowFlowViewItemText )
							{
								rchighlight.Y += this.nImageSpacing - 2;

								if( !bFlowView )
									rchighlight.X = ( this.ClientRectangle.Width - imgSize.Width ) / 2 - 2;
								else if( this.ShowFlowViewItemText )
									rchighlight.X += ( rchighlight.Width - imgSize.Width ) / 2 - 2;

								rchighlight.Width = imgSize.Width + 4;
								rchighlight.Height = imgSize.Height + 4;
							}
						}
					}
				}
				else if( !this.HighlightImage && ( IsImgShown || ( this.FlowView && !this.ShowFlowViewItemText ) ) )
				{
					if( !this.bSmallImageView )
					{
						int nimage = this.nImageSpacing + imgSize.Width + 2;
						rchighlight.Y += nimage;
						rchighlight.Height -= nimage;
					}
					else
					{
						if( !bFlowView )
						{
							int nIcrement = this.nItemXSpacing + imgSize.Height + 4;
							rchighlight.Width -= nIcrement;

							if( !bIsMirrored )
								rchighlight.X += nIcrement;
						}
						else
						{
							int nimage = this.nImageSpacing + imgSize.Width + 2;
							rchighlight.Y += nimage;
							rchighlight.Height -= nimage;
						}
					}
				}


				if( this.Horizontal )
				{
					int temp = rchighlight.Width;
					rchighlight.Width = rchighlight.Height;
					rchighlight.Height = temp;

					temp = rchighlight.X;
					rchighlight.X = rchighlight.Y;
					rchighlight.Y = temp;
				}
			}
			return rchighlight;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual Rectangle GetAdjustedTextBounds( int nindex, Rectangle rcItem )
		{
			Rectangle rctext = rcItem;

			GroupViewItem item = (GroupViewItem)VisibleItems[nindex];
			if( this.bSmallImageView == false )
			{
				if( ( this.ilLarge != null ) && ( item.ImageIndex >= 0 ) && ( item.ImageIndex < this.ilLarge.Images.Count ) )
				{
                    int nimage = this.nImageSpacing + defLargeImgSize.Width + this.nTextSpacing;
                    
					rctext.Y += nimage;
					rctext.Height -= nimage;
				}
				else
				{
					rctext.X += 2;	// Provide a 2 pxl offset from the highlight edge.
					rctext.Width -= 4;
				}
			}
			else
			{
				bool bIsMirrored = GetIsMirrored();

				if( !bFlowView )
				{
					int nWidthDecrement;
					if( ( this.ilSmall != null ) && ( item.ImageIndex >= 0 ) && ( item.ImageIndex < this.ilSmall.Images.Count ) )
						nWidthDecrement = this.nItemXSpacing + defSmallImgSize.Width +
							this.nTextSpacing;
					else
						nWidthDecrement = this.nItemXSpacing;

					if( !bIsMirrored || this.Horizontal )
					{
						rctext.X = nWidthDecrement;
					}
					rctext.Width -= nWidthDecrement;

					if( ( this.ctrlToolTip.Visible == true ) && ( nindex == this.nHighlightedItem ) )
						rctext.X += bIsMirrored ? 1 : -1;
				}
				else
				{
					if( ( this.ilSmall != null ) && ( item.ImageIndex >= 0 ) && ( item.ImageIndex < this.ilSmall.Images.Count ) )
					{
						int nimage = this.nImageSpacing + defSmallImgSize.Width +
							this.nTextSpacing;
						rctext.Y += nimage;
						rctext.Height -= nimage;
					}
					else
					{
						rctext.X += 2;	// Provide a 2 pxl offset from the highlight edge.
						rctext.Width -= 4;
					}
				}
			}
			return rctext;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected SizeF MeasureText( int nindex, Rectangle rc )
		{
			if( this.VisibleItems.Count == 0 || nindex > this.VisibleItems.Count )
				return SizeF.Empty;

			GroupViewItem item = (GroupViewItem)this.VisibleItems[nindex];
			if( item.Text == "" )
				return new Size( 0, 0 );

			Font textfont;
			if( ( nindex == this.nHighlightedItem ) && ( this.bTextUnderline == true ) )
				textfont = Syncfusion.Drawing.FontUtil.CreateFont( this.Font, FontStyle.Underline );
			else
				textfont = Syncfusion.Drawing.FontUtil.CreateFont( this.Font, this.Font.Style );

			rc.Height = 0;
			Graphics gph = this.CreateGraphics();
			gph.TextRenderingHint = TextRenderingHint.AntiAlias;

			StringFormatFlags sffFormatFlags = StringFormatFlags.LineLimit;
			if( GetIsMirrored() )
			{
				sffFormatFlags |= StringFormatFlags.DirectionRightToLeft;
			}

			StringFormat sf = new StringFormat( sffFormatFlags );
			sf.Alignment = StringAlignment.Near;
			if( this.bTextWrap == false )
			{
				sf.Trimming = StringTrimming.EllipsisCharacter;
				sf.FormatFlags |= StringFormatFlags.NoWrap;
				sf.LineAlignment = StringAlignment.Center;
			}
			else
			{
				sf.Trimming = StringTrimming.EllipsisWord;
			}
			SizeF textsize = gph.MeasureString( item.Text, textfont, rc.Width, sf );
			if(this.FlowView )
				textsize = gph.MeasureString( item.Text, textfont, this.FlowViewItemTextLength , sf );
			textfont.Dispose();
			gph.Dispose();
			sf.Dispose();

			return textsize;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DrawDragDropNonFlowInsert( int inspos, DragDropInsert insert, bool berase )
		{
			Point[] ptsarrayl = new Point[3];
			Point[] ptsarrayr = new Point[3];
			if( this.Horizontal )
			{
				ptsarrayl[0] = new Point( inspos, this.ClientRectangle.Left+10 );
			}
			else
			{
				ptsarrayl[0] = new Point( this.ClientRectangle.Left+10, inspos );
			}

			int nrightedge = ( this.Horizontal ) ? this.ClientRectangle.Bottom : this.ClientRectangle.Right;
			if( this.Horizontal )
			{
				ptsarrayr[0] = new Point( inspos, nrightedge-10 );
			}
			else
			{
				ptsarrayr[0] = new Point( nrightedge-10, inspos );
			}
			switch( insert )
			{
				case DragDropInsert.Upper:
				if( this.Horizontal )
				{
					ptsarrayl[1] = new Point( inspos+10, this.ClientRectangle.Left+5 );
					ptsarrayl[2] = new Point( inspos, this.ClientRectangle.Left+5 );
					ptsarrayr[1] = new Point( inspos+10, nrightedge-5 );
					ptsarrayr[2] = new Point( inspos, nrightedge-5 );
				}
				else
				{
					ptsarrayl[1] = new Point( this.ClientRectangle.Left+5, inspos+10 );
					ptsarrayl[2] = new Point( this.ClientRectangle.Left+5, inspos );
					ptsarrayr[1] = new Point( nrightedge-5, inspos+10 );
					ptsarrayr[2] = new Point( nrightedge-5, inspos );
				}
				break;
				case DragDropInsert.Middle:
				if( this.Horizontal )
				{
					ptsarrayl[1] = new Point( inspos+5, this.ClientRectangle.Left+5 );
					ptsarrayl[2] = new Point( inspos-5, this.ClientRectangle.Left+5 );
					ptsarrayr[1] = new Point( inspos + 5, nrightedge - 5 );
					ptsarrayr[2] = new Point( inspos - 5, nrightedge - 5 );
				}
				else
				{
					ptsarrayl[1] = new Point( this.ClientRectangle.Left+5, inspos+5 );
					ptsarrayl[2] = new Point( this.ClientRectangle.Left+5, inspos-5 );
					ptsarrayr[1] = new Point( nrightedge-5, inspos+5 );
					ptsarrayr[2] = new Point( nrightedge-5, inspos-5 );
				}
				break;
				case DragDropInsert.Lower:
				if( this.Horizontal )
				{
					ptsarrayl[1] = new Point( inspos+1, this.ClientRectangle.Left+5 );
					ptsarrayl[2] = new Point( inspos-10, this.ClientRectangle.Left+5 );
					ptsarrayr[1] = new Point( inspos+1, nrightedge-5 );
					ptsarrayr[2] = new Point( inspos-10, nrightedge-5 );
				}
				else
				{
					ptsarrayl[1] = new Point( this.ClientRectangle.Left+5, inspos+1 );
					ptsarrayl[2] = new Point( this.ClientRectangle.Left+5, inspos-10 );
					ptsarrayr[1] = new Point( nrightedge-5, inspos+1 );
					ptsarrayr[2] = new Point( nrightedge-5, inspos-10 );
				}
				break;
				default:
				break;
			}

			Graphics gph = this.CreateGraphics();
			Brush br = null;
			if( berase == true )
			{
				// Exclude the scroll button clip rect from the gph object.
				if( this.bIntegratedScrolling == false )
				{
					if( this.bUpScrlButton )
						gph.ExcludeClip( this.UpScrlBtnRect );
					if( this.bDownScrlButton )
						gph.ExcludeClip( this.DownScrlBtnRect );
				}
				br = this.brBackGround;

				Rectangle rcfill;
				if( this.Horizontal )
				{
					rcfill = new Rectangle( ptsarrayl[0].X, ptsarrayl[0].Y, 1, ptsarrayr[0].Y-ptsarrayl[0].Y + 1 );
				}
				else
				{
					rcfill = new Rectangle( ptsarrayl[0].X, ptsarrayl[0].Y, ptsarrayr[0].X-ptsarrayl[0].X, 1 );
				}

				this.Invalidate( rcfill );

				if( this.Horizontal )
				{
					this.Invalidate( new Rectangle( ptsarrayl[2], new Size( 11, 5 ) ) );
					this.Invalidate( new Rectangle( new Point( ptsarrayr[2].X, ptsarrayr[0].Y ), new Size( 11, 5 ) ) );
				}
				else
				{
					this.Invalidate( new Rectangle( ptsarrayl[2], new Size( 5, 11 ) ) );
					this.Invalidate( new Rectangle( new Point( ptsarrayr[0].X, ptsarrayr[2].Y ), new Size( 5, 11 ) ) );
				}
			}
			else
			{
				gph.DrawLine( Pens.Black, ptsarrayl[0], ptsarrayr[0] );
				br = Brushes.Black;
			}

			if( !berase )
			{
				gph.FillPolygon( br, ptsarrayl );
				gph.FillPolygon( br, ptsarrayr );
			}

			if( this.bSmallImageView && ( this.nHighlightedItem == 0 ) && ( berase == true ) 
				&& ( insert == DragDropInsert.Upper ) )
			{
				Rectangle rchover = this.rcHighlightedItem;
				DrawItem( gph, this.nHighlightedItem, rchover, ItemState.Highlight );
			}

			// In the buttonview mode, redraw the selection rect, if erase insert causes an overdraw.
			if( ( berase == true ) && ( this.bButtonView == true ) && ( this.VisibleItems.Count > this.nSelectedItem ) )
			{
				if( this.Horizontal )
				{
					if( ( inspos >= this.rcSelectedItem.Left-5 ) && ( inspos <= this.rcSelectedItem.Left+2 ) )
					{
						Rectangle redrawRect = 
							new Rectangle( this.rcSelectedItem.Left, this.rcSelectedItem.Top-1, 2, this.rcSelectedItem.Height );
						this.Invalidate( redrawRect );
					}
					if( ( inspos >= this.rcSelectedItem.Right-2 ) && ( inspos <= this.rcSelectedItem.Right+5 ) )
					{
						Rectangle redrawRect 
							= new Rectangle( this.rcSelectedItem.Right, this.rcSelectedItem.Top-1, 2, this.rcSelectedItem.Height );
						this.Invalidate( redrawRect );
					}
				}
				else
				{
					if( ( inspos >= this.rcSelectedItem.Top-5 ) && ( inspos <= this.rcSelectedItem.Top+2 ) )
						this.Invalidate( new Rectangle( this.rcSelectedItem.Left, this.rcSelectedItem.Top-1, this.rcSelectedItem.Width, 2 ) );
					if( ( inspos >= this.rcSelectedItem.Bottom-2 ) && ( inspos <= this.rcSelectedItem.Bottom+5 ) )
						this.Invalidate( new Rectangle( this.rcSelectedItem.Left, this.rcSelectedItem.Bottom-1, this.rcSelectedItem.Width, 2 ) );
				}
			}

			gph.Dispose();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DrawDragDropFlowInsert( int posX, int posY, DragDropInsert insert, bool berase )
		{
			Point[] ptsarrayl = new Point[3];
			Point[] ptsarrayr = new Point[3];

			Rectangle itemBounds = Rectangle.Empty;
			if( this.VisibleItems.Count > 0 )
				itemBounds = ( this.VisibleItems[0] as GroupViewItem ).Bounds;

			if( this.Horizontal )
			{
				ptsarrayl[0] = new Point( posX + 5, posY );
			}
			else
			{
				ptsarrayl[0] = new Point( posX, posY + 5 );
			}

			if( this.Horizontal )
			{
				ptsarrayr[0] = new Point( posX + itemBounds.Width - 5, posY );
			}
			else
			{
				ptsarrayr[0] = new Point( posX, posY + itemBounds.Height - 6 );
			}

			switch( insert )
			{
				case DragDropInsert.Upper:
				if( this.Horizontal )
				{
					ptsarrayl[1] = new Point( posX, posY + 10 );
					ptsarrayl[2] = new Point( posX, posY );
					ptsarrayr[1] = new Point( posX + itemBounds.Width, posY + 10 );
					ptsarrayr[2] = new Point( posX + itemBounds.Width, posY );
				}
				else
				{
					ptsarrayl[1] = new Point( posX, posY );
					ptsarrayl[2] = new Point( posX + 9, posY );
					ptsarrayr[1] = new Point( posX, posY + itemBounds.Height );
					ptsarrayr[2] = new Point( posX + 9, posY + itemBounds.Height );
				}
				break;
				case DragDropInsert.Middle:
				if( this.Horizontal )
				{
					ptsarrayl[1] = new Point( posX, posY + 5 );
					ptsarrayl[2] = new Point( posX, posY - 5 );
					ptsarrayr[1] = new Point( posX + itemBounds.Width, posY + 5 );
					ptsarrayr[2] = new Point( posX + itemBounds.Width, posY - 5 );
				}
				else
				{
					ptsarrayl[1] = new Point( posX - 4, posY );
					ptsarrayl[2] = new Point( posX + 5, posY );
					ptsarrayr[1] = new Point( posX - 5, posY + itemBounds.Height );
					ptsarrayr[2] = new Point( posX + 5, posY + itemBounds.Height );
				}
				break;
				case DragDropInsert.Lower:
				if( this.Horizontal )
				{
					ptsarrayl[1] = new Point( posX, posY + 1 );
					ptsarrayl[2] = new Point( posX, posY - 10 );
					ptsarrayr[1] = new Point( posX + itemBounds.Width, posY + 1 );
					ptsarrayr[2] = new Point( posX + itemBounds.Width, posY - 10 );
				}
				else
				{
					ptsarrayl[1] = new Point( posX - 10, posY );
					ptsarrayl[2] = new Point( posX + 1, posY );
					ptsarrayr[1] = new Point( posX - 10, posY + itemBounds.Height );
					ptsarrayr[2] = new Point( posX + 1, posY + itemBounds.Height );
				}
				break;
				default:
				break;
			}

			Graphics gph = this.CreateGraphics();
			Brush br = null;

			if( berase )
			{
				// Exclude the scroll button clip rect from the gph object.
				if( this.bIntegratedScrolling == false )
				{
					if( this.bUpScrlButton )
						gph.ExcludeClip( this.UpScrlBtnRect );
					if( this.bDownScrlButton )
						gph.ExcludeClip( this.DownScrlBtnRect );
				}
				br = this.brBackGround;

				Rectangle rcfill;
				if( this.Horizontal )
				{
					rcfill = new Rectangle( ptsarrayl[0].X, ptsarrayl[0].Y, ptsarrayr[0].X - ptsarrayl[0].X, 1 );
				}
				else
				{
					rcfill = new Rectangle( ptsarrayl[0].X, ptsarrayl[0].Y, 1, ptsarrayr[0].Y - ptsarrayl[0].Y + 1 );
				}

				this.Invalidate( rcfill );

				if( this.Horizontal )
				{
					this.Invalidate( new Rectangle( ptsarrayl[2], new Size( 5, 11 ) ) );
					this.Invalidate( new Rectangle( new Point( ptsarrayr[0].X, ptsarrayr[2].Y ), new Size( 5, 11 ) ) );
				}
				else
				{
					this.Invalidate( new Rectangle( ptsarrayl[1], new Size( 11, 5 ) ) );
					this.Invalidate( new Rectangle( new Point( ptsarrayr[1].X, ptsarrayr[0].Y + 1 ), new Size( 11, 5 ) ) );
				}
			}
			else
			{
				gph.DrawLine( Pens.Black, ptsarrayl[0], ptsarrayr[0] );
				br = Brushes.Black;
			}

			if( !berase )
			{
				gph.FillPolygon( br, ptsarrayl );
				gph.FillPolygon( br, ptsarrayr );
			}

			if( this.bSmallImageView && ( this.nHighlightedItem == 0 ) && ( berase == true ) 
				&& ( insert == DragDropInsert.Upper ) )
			{
				Rectangle rchover = this.rcHighlightedItem;
				DrawItem( gph, this.nHighlightedItem, rchover, ItemState.Highlight );
			}

			gph.Dispose();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DrawNonFlowScrollButtons( Graphics gfxDC )
		{
			int[] itemheights = new int[this.VisibleItems.Count];
			Rectangle rcitem = Rectangle.Empty;
			for( int i=0; i<this.VisibleItems.Count; i++ )
			{
				GroupViewItem item = this.VisibleItems[i] as GroupViewItem;
				if( this.bSmallImageView == false )
				{
					rcitem.Y += this.nItemYSpacing;
					if( this.bIntegratedScrolling == false )
					{
						rcitem.X = this.ClientRectangle.Left + nScrllBttnOffset;
						rcitem.Width = this.ClientRectangle.Width - ( 2*nScrllBttnOffset+1 );
					}
					else
					{
						rcitem.X = this.ClientRectangle.Left+1;
						rcitem.Width = this.ClientRectangle.Width-2;
					}
					SizeF szlabel = this.MeasureText( i, rcitem );
					if( ( this.ilLarge != null ) && ( item.ImageIndex >= 0 ) && ( item.ImageIndex < this.ilLarge.Images.Count ) )
						rcitem.Height = this.nImageSpacing + 32 + this.nTextSpacing + (int)szlabel.Height + this.nImageSpacing;
					else
						rcitem.Height = this.nImageSpacing + (int)szlabel.Height + this.nImageSpacing;
				}
				else	// SmallIcon View.
				{
					rcitem.Y += this.nItemYSpacing;
					rcitem.X = this.ClientRectangle.Left+1+this.nItemXSpacing;
					rcitem.Width = this.ClientRectangle.Width - ( 2+this.nItemXSpacing );
					if( this.bIntegratedScrolling == false )
						rcitem.Width -= nScrllBttnOffset;
					if( ( this.ilSmall != null ) && ( item.ImageIndex >= 0 ) && ( item.ImageIndex < this.ilSmall.Images.Count ) )
					{
						rcitem.Width -= 16+this.nTextSpacing;
						SizeF szlabel = this.MeasureText( i, rcitem );
						int ntextheight = (int)szlabel.Height;
						rcitem.Height = this.nImageSpacing + ( ( 16>ntextheight )?16:ntextheight ) + this.nImageSpacing;
					}
					else
					{
						SizeF szlabel = this.MeasureText( i, rcitem );
						int ntextheight = (int)szlabel.Height;
						rcitem.Height = this.nImageSpacing + ntextheight + this.nImageSpacing;
					}
				}

				itemheights[i] = rcitem.Bottom;
				rcitem.Y = rcitem.Bottom;
			}

			// If the last item's bottom is greater than the client rectangle height, scroll
			// buttons will be required.
			if( ( this.VisibleItems.Count > 0 ) && ( itemheights[this.VisibleItems.Count-1] > 
				( ( this.Horizontal ) ? this.ClientRectangle.Width : this.ClientRectangle.Height ) ) )	// Ok, scroll buttons are needed. 
			{
				bool bIsMirrored = GetIsMirrored();

				// If top index is greater than 0, draw the top button.
				if( this.nTopIndex > 0 )
				{
					this.bUpScrlButton = true;
					if( this.bIntegratedScrolling == false )
					{
						using( CMirroredDrawer mdUpDrawer = new CMirroredDrawer( gfxDC, this.UpScrlBtnRect, bIsMirrored ) )
						{
							Graphics gfxVirt = mdUpDrawer.VirtualGfx;
							Rectangle rectVirt = mdUpDrawer.VirtualBounds;

							if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.bThemesEnabled )
							{
								int nStateID;

								if( this.scrllBtnState == ScrollButtonState.UpScrollPressed )
								{
									if( this.Horizontal )
									{
										nStateID = ( bIsMirrored ) ? ThemeStates.ABS_RIGHTPRESSED : ThemeStates.ABS_LEFTPRESSED;
									}
									else
									{
										nStateID = ThemeStates.ABS_UPPRESSED;
									}
								}
								else if( this.scrllBtnState == ScrollButtonState.UpScrollHot )
								{
									if( this.Horizontal )
									{
										nStateID = ( bIsMirrored ) ? ThemeStates.ABS_RIGHTHOT : ThemeStates.ABS_LEFTHOT;
									}
									else
									{
										nStateID = ThemeStates.ABS_UPHOT;
									}
								}
								else
								{
									if( this.Horizontal )
									{
										nStateID = ( bIsMirrored ) ? ThemeStates.ABS_RIGHTNORMAL : ThemeStates.ABS_LEFTNORMAL;
									}
									else
									{
										nStateID = ThemeStates.ABS_UPNORMAL;
									}
								}

								this.tdScrollBar.DrawThemeBackground( gfxVirt, ThemeParts.SBP_ARROWBTN, nStateID, rectVirt );
							}
							else
							{
								ButtonState bsBtnState;
								if( this.scrllBtnState == ScrollButtonState.UpScrollPressed )
								{
									bsBtnState = ( this.bFlatLook ) ? ButtonState.Pushed|ButtonState.Flat : ButtonState.Pushed;
								}
								else
								{
									bsBtnState = ( this.bFlatLook ) ? ButtonState.Normal|ButtonState.Flat : ButtonState.Normal;
								}
								ControlPaint.DrawScrollButton( gfxVirt, rectVirt,
									( this.Horizontal ) ? ScrollButton.Left : ScrollButton.Up, bsBtnState );
							}
						}
					}
					else
					{
						if( this.Parent is IIntegratedScrollContainer )
						{
							IIntegratedScrollContainer iisc = this.Parent as IIntegratedScrollContainer;
							iisc.InvalidateUpScrollButton();
						}
					}
				}
				else if( this.bUpScrlButton == true )
				{
					this.bUpScrlButton = false;
					if( ( this.scrllBtnState == ScrollButtonState.UpScrollHot ) || ( this.scrllBtnState == ScrollButtonState.UpScrollPressed ) )
						this.scrllBtnState = ScrollButtonState.Normal;
					if( ( this.tmrScrolling != null ) && ( this.tmrScrolling.Enabled ) )
						this.tmrScrolling.Stop();
				}

				// After accounting for the top index, if remaining items do not fill the client rect, draw the bottom button.
				bool bdownscrollreqd = false;
				if( this.nTopIndex > 0 && (VisibleItems.Count)>=nTopIndex )
					bdownscrollreqd = ( ( itemheights[this.VisibleItems.Count-1] - itemheights[this.nTopIndex-1] ) >
						( ( this.Horizontal ) ? this.ClientRectangle.Width : this.ClientRectangle.Height ) );
				else
					bdownscrollreqd = ( itemheights[this.VisibleItems.Count-1] >
						( ( this.Horizontal ) ? this.ClientRectangle.Width : this.ClientRectangle.Height ) );
				if( bdownscrollreqd == true )
				{
					this.bDownScrlButton = true;
					if( this.bIntegratedScrolling == false )
					{
						using( CMirroredDrawer mdUpDrawer = new CMirroredDrawer( gfxDC, this.DownScrlBtnRect, bIsMirrored ) )
						{
							Graphics gfxVirt = mdUpDrawer.VirtualGfx;
							Rectangle rectVirt = mdUpDrawer.VirtualBounds;

							if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.bThemesEnabled )
							{
								int nStateID;
								if( this.scrllBtnState == ScrollButtonState.DownScrollPressed )
								{
									if( this.Horizontal )
									{
										nStateID = ( bIsMirrored ) ? ThemeStates.ABS_LEFTPRESSED : ThemeStates.ABS_RIGHTPRESSED;
									}
									else
									{
										nStateID = ThemeStates.ABS_DOWNPRESSED;
									}
								}
								else if( this.scrllBtnState == ScrollButtonState.DownScrollHot )
								{
									if( this.Horizontal )
									{
										nStateID = ( bIsMirrored ) ? ThemeStates.ABS_LEFTHOT : ThemeStates.ABS_RIGHTHOT;
									}
									else
									{
										nStateID = ThemeStates.ABS_DOWNHOT;
									}
								}
								else
								{
									if( this.Horizontal )
									{
										nStateID = ( bIsMirrored ) ? ThemeStates.ABS_LEFTNORMAL : ThemeStates.ABS_RIGHTNORMAL;
									}
									else
									{
										nStateID = ThemeStates.ABS_DOWNNORMAL;
									}
								}
								this.tdScrollBar.DrawThemeBackground( gfxVirt, ThemeParts.SBP_ARROWBTN, nStateID, rectVirt );
							}
							else
							{
								ButtonState bsBtnState;
								if( this.scrllBtnState == ScrollButtonState.DownScrollPressed )
								{
									bsBtnState = ( this.bFlatLook ) ? ButtonState.Pushed|ButtonState.Flat : ButtonState.Pushed;
								}
								else
								{
									bsBtnState = ( this.bFlatLook ) ? ButtonState.Normal|ButtonState.Flat : ButtonState.Normal;
								}
								ControlPaint.DrawScrollButton( gfxVirt, rectVirt,
									( this.Horizontal ) ? ScrollButton.Right : ScrollButton.Down, bsBtnState );
							}
						}
					}
					else
					{
						if( this.Parent is IIntegratedScrollContainer )
						{
							IIntegratedScrollContainer iisc = this.Parent as IIntegratedScrollContainer;
							iisc.InvalidateDownScrollButton();
						}
					}
				}
				else if( this.bDownScrlButton == true )	// No more items left to scroll. If the scroll timer is enabled, disable it.				
				{
					this.bDownScrlButton = false;
					if( ( this.scrllBtnState == ScrollButtonState.DownScrollHot ) || ( this.scrllBtnState == ScrollButtonState.DownScrollPressed ) )
						this.scrllBtnState = ScrollButtonState.Normal;
					if( ( this.tmrScrolling != null ) && ( this.tmrScrolling.Enabled ) )
						this.tmrScrolling.Stop();
				}
			}
			else if( ( this.bUpScrlButton == true ) || ( this.bDownScrlButton == true ) )
			{
				this.bUpScrlButton = false;
				this.bDownScrlButton = false;
				this.scrllBtnState = ScrollButtonState.Normal;
				// Reset top index and redraw.
				if( this.nTopIndex > 0 )
				{
					this.nTopIndex = 0;
					Invalidate();
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DrawFlowScrollButtons( Graphics gph )
		{
			// If the client rectangle height is less than the product of nrowsreqd and 
			// nrowheight, then scroll bars are needed.
			int nrowsreqd = (int)Math.Ceiling( (float)this.VisibleItems.Count/(float)this.nItemsPerRow );
			int nrowheight = 0;

			bool bIsMirrored = this.GetIsMirrored();

			if( this.bSmallImageView == false )
				nrowheight = this.nImageSpacing+32+this.nImageSpacing+1;
			else
				nrowheight = this.nImageSpacing+16+this.nImageSpacing+1;

			if( this.ShowFlowViewItemText )
			{
				SizeF strSize = this.MeasureText( 0, this.ClientRectangle );

				nrowheight += Size.Ceiling( strSize ).Height + nTextSpacing;
			}

			int totalHeight = ( nrowheight+this.nItemYSpacing ) * nrowsreqd - this.nItemYSpacing;
			
			if (this.ClientRectangle.Height < totalHeight)
			{
				// If top index is greater than 0, draw the top button.
				if( this.nTopIndex > 0 )
				{
					this.bUpScrlButton = true;
					if( this.bIntegratedScrolling == false )
					{
						using( CMirroredDrawer mdUpDrawer = new CMirroredDrawer( gph, this.UpScrlBtnRect, bIsMirrored ) )
						{
							Graphics gfxVirt = mdUpDrawer.VirtualGfx;
							Rectangle rectVirt = mdUpDrawer.VirtualBounds;

							if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.bThemesEnabled )
							{
								int stateID;

								if( this.scrllBtnState == ScrollButtonState.UpScrollPressed )
								{
									if( this.Horizontal )
									{
										stateID = ( bIsMirrored ) ? ThemeStates.ABS_RIGHTPRESSED : ThemeStates.ABS_LEFTPRESSED;
									}
									else
									{
										stateID = ThemeStates.ABS_UPPRESSED;
									}
								}
								else if( this.scrllBtnState == ScrollButtonState.UpScrollHot )
								{
									if( this.Horizontal )
									{
										stateID = ( bIsMirrored ) ? ThemeStates.ABS_RIGHTHOT : ThemeStates.ABS_LEFTHOT;
									}
									else
									{
										stateID = ThemeStates.ABS_UPHOT;
									}
								}
								else
								{
									if( this.Horizontal )
									{
										stateID = ( bIsMirrored ) ? ThemeStates.ABS_RIGHTNORMAL : ThemeStates.ABS_LEFTNORMAL;
									}
									else
									{
										stateID = ThemeStates.ABS_UPNORMAL;
									}
								}

								this.tdScrollBar.DrawThemeBackground( gfxVirt, ThemeParts.SBP_ARROWBTN, stateID, rectVirt );
							}
							else
							{
								if( this.scrllBtnState == ScrollButtonState.UpScrollPressed )
								{
									ButtonState pushed = ( this.bFlatLook == true ) ? ButtonState.Pushed|ButtonState.Flat : ButtonState.Pushed;
									ControlPaint.DrawScrollButton( gfxVirt, rectVirt,
										( this.Horizontal ) ? ScrollButton.Left : ScrollButton.Up, pushed );
								}
								else
								{
									ButtonState normal = ( this.bFlatLook == true ) ? ButtonState.Normal|ButtonState.Flat : ButtonState.Normal;
									ControlPaint.DrawScrollButton( gfxVirt, rectVirt,
										( this.Horizontal ) ? ScrollButton.Left : ScrollButton.Up, normal );
								}
							}
						}
					}
					else
					{
						if( this.Parent is IIntegratedScrollContainer )
						{
							IIntegratedScrollContainer iisc = this.Parent as IIntegratedScrollContainer;
							iisc.InvalidateUpScrollButton();
						}
					}
				}
				else if( this.bUpScrlButton == true )
				{
					this.bUpScrlButton = false;
					if( ( this.scrllBtnState == ScrollButtonState.UpScrollHot ) || ( this.scrllBtnState == ScrollButtonState.UpScrollPressed ) )
						this.scrllBtnState = ScrollButtonState.Normal;
					if( ( this.tmrScrolling != null ) && ( this.tmrScrolling.Enabled ) )
						this.tmrScrolling.Stop();
				}
				// After accounting for the top row, recalculate the rows required and if the 
				// client rect is still insufficient, then provide the lower scrollbar.
				nrowsreqd = (int)Math.Ceiling( (float)( this.VisibleItems.Count-this.nTopIndex )/(float)this.nItemsPerRow );

				int nreqdrows = (int)Math.Ceiling( (float)this.VisibleItems.Count/(float)this.nItemsPerRow );
				int nlastrowindex = ( nreqdrows-1 )*this.nItemsPerRow;

				if( ( this.VisibleItems[nlastrowindex] as GroupViewItem ).Bounds.Bottom > this.ClientRectangle.Height )
					nrowsreqd++;

				if( this.ClientRectangle.Height < ( nrowheight*nrowsreqd + 1 ) )
				{
					this.bDownScrlButton = true;
					if( this.bIntegratedScrolling == false )
					{
						using( CMirroredDrawer mdDownDrawer = new CMirroredDrawer( gph, this.DownScrlBtnRect, bIsMirrored ) )
						{
							Graphics gfxVirt = mdDownDrawer.VirtualGfx;
							Rectangle rectVirt = mdDownDrawer.VirtualBounds;

							if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.bThemesEnabled )
							{
								int stateID;

								if( this.scrllBtnState == ScrollButtonState.DownScrollPressed )
								{
									if( this.Horizontal )
									{
										stateID = ( bIsMirrored ) ? ThemeStates.ABS_LEFTPRESSED : ThemeStates.ABS_RIGHTPRESSED;
									}
									else
									{
										stateID = ThemeStates.ABS_DOWNPRESSED;
									}
								}
								else if( this.scrllBtnState == ScrollButtonState.DownScrollHot )
								{
									if( this.Horizontal )
									{
										stateID = ( bIsMirrored ) ? ThemeStates.ABS_LEFTHOT : ThemeStates.ABS_RIGHTHOT;
									}
									else
									{
										stateID = ThemeStates.ABS_DOWNHOT;
									}
								}
								else
								{
									if( this.Horizontal )
									{
										stateID = ( bIsMirrored ) ? ThemeStates.ABS_LEFTNORMAL : ThemeStates.ABS_RIGHTNORMAL;
									}
									else
									{
										stateID = ThemeStates.ABS_DOWNNORMAL;
									}
								}

								this.tdScrollBar.DrawThemeBackground( gfxVirt, ThemeParts.SBP_ARROWBTN, stateID, rectVirt );
							}
							else
							{
								if( this.scrllBtnState == ScrollButtonState.DownScrollPressed )
								{
									ButtonState pushed = ( this.bFlatLook == true ) ? ButtonState.Pushed|ButtonState.Flat : ButtonState.Pushed;
									ControlPaint.DrawScrollButton( gfxVirt, rectVirt,
										( this.Horizontal ) ? ScrollButton.Right : ScrollButton.Down, pushed );
								}
								else
								{
									ButtonState normal = ( this.bFlatLook == true ) ? ButtonState.Normal|ButtonState.Flat : ButtonState.Normal;
									ControlPaint.DrawScrollButton( gfxVirt, rectVirt,
										( this.Horizontal ) ? ScrollButton.Right : ScrollButton.Down, normal );
								}
							}
						}
					}
					else if( this.Parent is IIntegratedScrollContainer )
					{
						IIntegratedScrollContainer iisc = this.Parent as IIntegratedScrollContainer;
						iisc.InvalidateDownScrollButton();
					}
				}
				else if( this.bDownScrlButton == true )	// No more items left to scroll. If the scroll timer is enabled, disable it.				
				{
					this.bDownScrlButton = false;
					if( ( this.scrllBtnState == ScrollButtonState.DownScrollHot ) || ( this.scrllBtnState == ScrollButtonState.DownScrollPressed ) )
						this.scrllBtnState = ScrollButtonState.Normal;
					if( ( this.tmrScrolling != null ) && ( this.tmrScrolling.Enabled ) )
						this.tmrScrolling.Stop();
				}
			}
			else
			{
				this.bUpScrlButton = false;
				this.bDownScrlButton = false;
				this.scrllBtnState = ScrollButtonState.Normal;
				// A resizing might have caused the client rect to become sufficiently large to accomodate all items.
				// Reset top index and redraw all items.
				if( this.nTopIndex >= this.nItemsPerRow )
				{
					this.nTopIndex = 0;
					Invalidate();
				}
			}
		}

        private bool suppressMouseHover = false;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual int ProcessMouseOver( Point pt )
		{
			int nindex = -1;
			if( ( pt.X != -1 ) && ( pt.Y != -1 ) )
			{
				Rectangle rcitem = Rectangle.Empty;
				for( nindex = nTopIndex; nindex<VisibleItems.Count; nindex++ )
				{
					GroupViewItem item = (GroupViewItem)VisibleItems[nindex];

					rcitem = item.Bounds;

					if( this.IsMouseOverItem( nindex, rcitem, pt ) == true )
					{
						if( GroupView.ptDragStart != new Point( -1, -1 ) )
							return nindex;

						if( nindex != this.nHighlightedItem && !this.suppressMouseHover)
						{
							if( this.nHighlightedItem >= 0 )
							{
								// Reset current hover state to normal before proceeding.
								if( this.ctrlToolTip.Visible == true )
									this.ctrlToolTip.Visible = false;
								int nprevhilight = this.nHighlightedItem;
								this.nHighlightedItem = -1;
								this.Invalidate( Rectangle.Inflate( this.rcHighlightedItem, 1, 1 ) );
								this.rcHighlightedItem = Rectangle.Empty;
							}

							if( item.Enabled == false )
							{
								if( this.ctrlToolTip.Visible == true )
									this.ctrlToolTip.Visible = false;

								return -1;
							}

							this.rcHighlightedItem = rcitem;
							this.nHighlightedItem = nindex;

							// Fire GroupViewItemHighlighted notification.
							if( this.nHighlightedItem != -1 )
							{
								this.OnGroupViewItemHighlighted( EventArgs.Empty );
							}

							if( ( this.bFlowView == false ) && ( this.bHighlightText == true ) && ( this.bTextWrap == false ) )
							{
								// If the control bounds are insufficient to display the full item, 
								// then popup the ToolTip form and draw the item on the form.
								Graphics gphtemp = this.CreateGraphics();
								SizeF sztext = gphtemp.MeasureString( item.Text, this.Font );
								gphtemp.Dispose();

								int nwidth = ( this.Horizontal ) ? this.Height : this.Width;
								if( this.bIntegratedScrolling == false )
									nwidth -= nScrllBttnOffset;

								int nreqlen = 0;
								if( this.bSmallImageView == true )
								{
									if( item.ImageIndex >= 0 )
										nreqlen = this.nItemXSpacing + 16 + this.nTextSpacing + (int)Math.Ceiling( sztext.Width )+2;
									else
										nreqlen = this.nItemXSpacing + (int)Math.Ceiling( sztext.Width )+2;
								}
								else
								{
									// For LargeImageView the flyout ToolTips are displayed only when the GroupViewItem is text-only.
									if( this.bIntegratedScrolling == false )
										nwidth -= nScrllBttnOffset;
									if( item.ImageIndex < 0 )
										nreqlen = (int)Math.Ceiling( sztext.Width )+2;
									else
										nreqlen = nwidth-1;
								}

								int nWidthDiff = nreqlen - nwidth;
								if( nWidthDiff >= 0 )
								{
									Point ptTooltip = this.rcHighlightedItem.Location;
									if( GetIsMirrored() && !this.Horizontal )
									{
										int nClientDiff = this.Width - ClientRectangle.Width;
										ptTooltip.X -= ( nWidthDiff + nClientDiff + 2 );
									}

									Point ptscreen = this.PointToScreen( ptTooltip /*this.rcHighlightedItem.Location*/ );
									Syncfusion.Runtime.InteropServices.NativeMethods.MoveWindow( this.ctrlToolTip.Handle,
										ptscreen.X, ptscreen.Y, 0, 0, false );

									if( this.Horizontal )
									{
										this.ctrlToolTip.Size = new Size( this.rcHighlightedItem.Width+1, nreqlen+1 );
									}
									else
									{
										this.ctrlToolTip.Size = new Size( nreqlen+1, this.rcHighlightedItem.Height+1 );
									}
									Syncfusion.Runtime.InteropServices.NativeMethods.SetWindowPos( this.ctrlToolTip.Handle, (IntPtr)( -1 ),
										0, 0, 0, 0, 0x0001|0x0002|0x0010|0x0040 ); //  SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE|SWP_SHOWWINDOW
									this.ctrlToolTip.Visible = true;

									this.ctrlToolTip.Refresh();
								}
								else
								{
									this.Invalidate( Rectangle.Inflate( this.rcHighlightedItem, 1, 1 ) );
									this.Update();
								}
							}
							else
							{
								this.Invalidate( Rectangle.Inflate( this.rcHighlightedItem, 1, 1 ) );
								this.Update();
							}
						}
						return nindex;
					}
					if( this.bFlowView == false )
						rcitem.Y = rcitem.Bottom;
				}
			}

			// No item was hit. If any item is currently in the up state, then restore it
			// to normal state and return a negative index.
			if( this.nHighlightedItem >= 0 )
			{
				if( this.ctrlToolTip.Visible == true )
					this.ctrlToolTip.Visible = false;

				int nprevhilight = this.nHighlightedItem;
				this.nHighlightedItem = -1;
				this.Invalidate( Rectangle.Inflate( this.rcHighlightedItem, 1, 1 ) );
				this.rcHighlightedItem = Rectangle.Empty;
				if( ( this.bButtonView == false ) && ( this.nSelectedItem != -1 ) )
					this.nSelectedItem = -1;
			}
			nindex = -1;

			return nindex;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool IsMouseOverItem( int nindex, Rectangle rchighlight, Point ptmouse )
		{
			if( ( this.bFlowView == true ) || ( this.bClipSelectionBounds == false ) )
			{
				return rchighlight.Contains( ptmouse );
			}
			else	// Selection bounds is clipped.
			{
				bool bIsMirrored = GetIsMirrored();

				GroupViewItem item = this.VisibleItems[nindex] as GroupViewItem;
				bool bimage = this.bHighlightImage;
				bool btext = this.bHighlightText;
				this.bHighlightImage = true;
				this.bHighlightText = false;
				Rectangle rcimage = this.GetAdjustedHighlightBounds( nindex, rchighlight );
				this.bHighlightImage = bimage;
				this.bHighlightText = btext;
				Rectangle rctext = this.GetAdjustedTextBounds( nindex, rchighlight );
				SizeF sztext = this.MeasureText( nindex, rctext );
				if( this.bSmallImageView == false )	// Center the text rect.
					rctext.X = (int)this.Width/2 - (int)sztext.Width/2;

				int nWidthDiff = rctext.Width - (int)sztext.Width;
				rctext.Width = (int)sztext.Width;
				if( this.bSmallImageView && GetIsMirrored() && nWidthDiff > 0 )
				{
					rctext.X += nWidthDiff;
				}

				if( item.ImageIndex >= 0 )
				{
					if( this.bSmallImageView == false )
					{
						Rectangle rcinter = new Rectangle( rcimage.Left, rcimage.Bottom, rcimage.Width, rctext.Top-rcimage.Bottom );
						rcimage.Inflate( 1, 0 );
						rcinter.Inflate( 1, 0 );
						rctext.Inflate( 1, 0 );
						return ( rcimage.Contains( ptmouse ) || rcinter.Contains( ptmouse ) || rctext.Contains( ptmouse ) );
					}
					else
					{
						Rectangle rcitem = new Rectangle(
							bIsMirrored ? rctext.Left : rchighlight.Left, rchighlight.Top,
							bIsMirrored ? rchighlight.Right - rctext.Left : rctext.Right-rchighlight.Left,
							rchighlight.Height );
						rcitem.Inflate( 1, 0 );
						return rcitem.Contains( ptmouse );
					}
				}
				else	// Text-only.
				{
					rctext.Inflate( 1, 0 );
					return rctext.Contains( ptmouse );
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void ScrollButtonsMouseHandler( MouseActions action, Point pt )
		{
			ButtonState normal = ( this.bFlatLook == true ) ? ButtonState.Normal|ButtonState.Flat : ButtonState.Normal;
			ButtonState pushed = ( this.bFlatLook == true ) ? ButtonState.Pushed|ButtonState.Flat : ButtonState.Pushed;
			if( action == MouseActions.Leave )
			{
				this.scrllBtnState = ScrollButtonState.Normal;
				if( this.bDownScrlButton == true )
				{
					this.Invalidate( this.DownScrlBtnRect );
					this.Update();
				}
				if( this.bUpScrlButton == true )
				{
					this.Invalidate( this.UpScrlBtnRect );
					this.Update();
				}
			}

			// Did the mouse click occur over one of the scroll buttons?
			if( ( this.bDownScrlButton == true ) && ( this.DownScrlBtnRect.Contains( pt ) ) )
			{
				if( action == MouseActions.LBtnDown )
				{
					this.scrllBtnState = ScrollButtonState.DownScrollPressed;
					this.Invalidate( this.DownScrlBtnRect );
					this.Update();
					this.DoScroll( true, true );
				}
				else if( action == MouseActions.Move )
				{
					this.scrllBtnState = ScrollButtonState.DownScrollHot;
					this.Invalidate( this.DownScrlBtnRect );
					this.Update();
				}
				else
				{
					this.scrllBtnState = ScrollButtonState.Normal;
					this.Invalidate( this.DownScrlBtnRect );
					this.Update();
				}
			}
			else if( ( this.bUpScrlButton == true ) && ( this.UpScrlBtnRect.Contains( pt ) ) )
			{
				if( action == MouseActions.LBtnDown )
				{
					this.scrllBtnState = ScrollButtonState.UpScrollPressed;
					this.Invalidate( this.UpScrlBtnRect );
					this.Update();
					this.DoScroll( false, true );
				}
				else if( action == MouseActions.Move )
				{
					this.scrllBtnState = ScrollButtonState.UpScrollHot;
					this.Invalidate( this.UpScrlBtnRect );
					this.Update();
				}
				else
				{
					this.scrllBtnState = ScrollButtonState.Normal;
					this.Invalidate( this.UpScrlBtnRect );
					this.Update();
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void ProcessDragDrop( MouseActions action, Point pt )
		{
			Rectangle rcitem = Rectangle.Empty;
			bool bIsMirrored = this.GetIsMirrored();

			if( action == MouseActions.Leave )
			{
				// Erase previous insert lines.
				if( bFlowView )
				{
					if( this.Horizontal )
					{
						if( !bIsMirrored )
							DrawDragDropFlowInsert( rcitem.X, rcitem.Y + rcitem.Height + this.nItemYSpacing / 2, DragDropInsert.Upper, true );
						else
							DrawDragDropFlowInsert( rcitem.X, rcitem.Y + rcitem.Height + this.nItemYSpacing / 2, DragDropInsert.Lower, true );
					}
					else
					{
						if( !bIsMirrored )
							DrawDragDropFlowInsert( rcitem.Right + this.nItemXSpacing / 2, rcitem.Y, DragDropInsert.Upper, true );
						else
							DrawDragDropFlowInsert( rcitem.Left - this.nItemXSpacing / 2, rcitem.Y, DragDropInsert.Lower, true );
					}
				}
				else
				{
					if( this.Horizontal )
					{
						Rectangle correctRect = rcitem;
						correctRect.Y += this.ClientRectangle.Width -
							( correctRect.Y * 2 + correctRect.Height * 2 + this.nItemYSpacing );

						DrawDragDropNonFlowInsert( correctRect.Bottom + this.nItemYSpacing/2, DragDropInsert.Lower, true );
					}
					else
					{
						DrawDragDropNonFlowInsert( rcitem.Bottom + this.nItemYSpacing/2, DragDropInsert.Upper, true );
					}
				}

				for( int i = nTopIndex; i < VisibleItems.Count; i++ )
				{
					if( bFlowView )
						this.SetFlowItemBounds( i, ref rcitem );
					else
						this.SetNonFlowItemBounds( i, ref rcitem );

					if( i == VisibleItems.Count-1 )
						break;

					Rectangle correctRect = rcitem;

					if( bFlowView )
					{
						if( this.Horizontal )
						{
							if( !bIsMirrored )
								DrawDragDropFlowInsert( correctRect.Y, correctRect.Right + this.nItemXSpacing / 2, DragDropInsert.Middle, true );
							else
								DrawDragDropFlowInsert( correctRect.Y, correctRect.Right + this.nItemXSpacing / 2, DragDropInsert.Middle, true );
						}
						else
						{
							if( !bIsMirrored )
								DrawDragDropFlowInsert( correctRect.Right + this.nItemXSpacing / 2, correctRect.Y, DragDropInsert.Middle, true );
							else
								DrawDragDropFlowInsert( correctRect.Left - this.nItemXSpacing / 2, correctRect.Y, DragDropInsert.Middle, true );
						}
					}
					else
					{
						if( this.Horizontal && bIsMirrored )
						{
							correctRect.Y += this.ClientRectangle.Width - 
								( correctRect.Y * 2 + correctRect.Height * 2 + this.nItemYSpacing );
						}

						DrawDragDropNonFlowInsert( correctRect.Bottom + this.nItemYSpacing/2, DragDropInsert.Middle, true );
						rcitem.Y = rcitem.Bottom;
					}
				}

				if( bFlowView )
				{
					if( this.Horizontal )
					{
						DrawDragDropFlowInsert( rcitem.Y, rcitem.Right + this.nItemYSpacing / 2, DragDropInsert.Lower, true );
					}
					else
					{
						if( !bIsMirrored )
							DrawDragDropFlowInsert( rcitem.Right + this.nItemXSpacing / 2, rcitem.Y, DragDropInsert.Lower, true );
						else
							DrawDragDropFlowInsert( rcitem.Left - this.nItemXSpacing / 2, rcitem.Y, DragDropInsert.Upper, true );
					}
				}
				else
				{
					if( this.Horizontal && bIsMirrored )
					{
						Rectangle correctRect = rcitem;
						correctRect.Y += this.ClientRectangle.Width - ( correctRect.Y * 2 + correctRect.Height * 2 );

						DrawDragDropNonFlowInsert( correctRect.Bottom - 7, DragDropInsert.Upper, true );
					}
					else
					{
						DrawDragDropNonFlowInsert( rcitem.Bottom + 7, DragDropInsert.Lower, true );
					}
				}

				this.nDropItem = -1;
				this.nHighlightedItem = -1;

				return;
			}

			if( this.ClientRectangle.Contains( pt ) == false )
			{
				this.nDropItem = -1;
				this.nHighlightedItem = -1;
				return;
			}

			if( this.Horizontal )
			{
				int temp = pt.Y;
				pt.Y = pt.X;
				pt.X = temp;
			}

			// Is cursor above the first visible item?
			bool needDownScroll = false;

			if( bFlowView )
			{
				if( this.Horizontal )
				{
					needDownScroll = pt.Y < this.ClientRectangle.Right && ( pt.Y > this.ClientRectangle.Right - this.nImageSpacing );
				}
				else
				{
					if( !bIsMirrored )
						needDownScroll = pt.Y > ClientRectangle.Top && pt.Y < ( ClientRectangle.Top + this.nImageSpacing );
					else
						needDownScroll = pt.Y > ClientRectangle.Left && pt.Y < this.nImageSpacing;
				}
			}
			else
			{
				if( this.Horizontal && bIsMirrored )
				{
					needDownScroll = ( ( pt.Y < this.ClientRectangle.Right ) && ( pt.Y > this.ClientRectangle.Right - this.nImageSpacing ) );
				}
				else
				{
					needDownScroll = ( ( pt.Y > rcitem.Bottom ) && ( pt.Y < this.nImageSpacing ) );
				}
			}

			if( needDownScroll )
			{
				// Scroll up if the top index is not equal to 0.
				if( this.nTopIndex > 0 )
				{
					this.nDropItem = -1;
					DoScroll( false, false );
					return;
				}

				Rectangle correctRect = rcitem;
				nDropItem = this.nTopIndex;

				if( !bFlowView )
				{
					if( this.Horizontal && bIsMirrored && !this.bFlowView )
					{
						correctRect.Y += this.ClientRectangle.Width -
							( correctRect.Y * 2 + correctRect.Height * 2 + this.nItemYSpacing );
					}

					DrawDragDropNonFlowInsert( correctRect.Bottom + this.nItemYSpacing/2,
						( bIsMirrored && this.Horizontal ) ? DragDropInsert.Lower : DragDropInsert.Upper, false );
				}

				return;
			}

			if( this.bDownScrlButton )
			{
				bool needUpScroll = false;

				if( this.Horizontal )
				{
					if( bIsMirrored )
					{
						needUpScroll = ( pt.Y < ( this.ClientRectangle.Left + 25 ) );
					}
					else
					{
						needUpScroll = ( pt.Y > ( this.ClientRectangle.Right - 25 ) );
					}
				}
				else
				{
					needUpScroll = pt.Y > ( this.ClientRectangle.Bottom - 25 );
				}

				if( needUpScroll )
				{
					this.nDropItem = -1;
					DoScroll( true, false );
					return;
				}
			}

			// Determine the item for the current cursor coords.
			for( int i = this.nTopIndex; i < this.VisibleItems.Count; i++ )
			{
				if( bFlowView )
					SetFlowItemBounds( i, ref rcitem );
				else
					SetNonFlowItemBounds( i, ref rcitem );

				if( i == this.VisibleItems.Count - 1 )
					break;

				// If the cursor is between two items, draw the middle insert line.
				Rectangle correctRect = rcitem;

				if( bFlowView )
				{
					int countUnvisibleItem = cllnGroupViewItems.Count - VisibleItems.Count;
					this.nDropItem = countUnvisibleItem + i + 1;

					if( !bIsMirrored )
					{
						if( pt.Y > correctRect.Y && pt.Y < correctRect.Bottom && 
							pt.X > correctRect.Right && pt.X < ( correctRect.Right + this.nItemXSpacing + this.nImageSpacing ) )
						{
							if( this.Horizontal )
								DrawDragDropFlowInsert( correctRect.Y, correctRect.Right + this.nItemXSpacing / 2, DragDropInsert.Middle, false );
							else
								DrawDragDropFlowInsert( correctRect.Right + this.nItemXSpacing / 2, correctRect.Y, DragDropInsert.Middle, false );

							return;
						}
					}
					else
					{
						if( this.Horizontal )
						{
							if( pt.X > correctRect.Right && pt.X < ( correctRect.Right + this.nItemXSpacing ) &&
								pt.Y > correctRect.Top && pt.Y < correctRect.Bottom )
							{
								DrawDragDropFlowInsert( correctRect.Top, correctRect.Right + this.nItemXSpacing / 2, DragDropInsert.Middle, false );
								return;
							}
						}
						else
						{
							if( pt.Y > correctRect.Y && pt.Y < correctRect.Bottom && 
								pt.X < correctRect.Left && pt.X > ( correctRect.Left - this.nItemXSpacing ) )
							{
								DrawDragDropFlowInsert( correctRect.Left - this.nItemXSpacing / 2, correctRect.Y, DragDropInsert.Middle, false );
								return;
							}
						}
					}
				}
				else
				{
					if( this.Horizontal && bIsMirrored )
					{
						correctRect.Y += this.ClientRectangle.Width -
							( correctRect.Y * 2 + correctRect.Height * 2 + this.nItemYSpacing );
					}

					if( ( pt.Y > correctRect.Bottom ) && ( pt.Y < ( correctRect.Bottom + this.nItemYSpacing+this.nImageSpacing ) ) )
					{
						int countUnvisibleItem = cllnGroupViewItems.Count - VisibleItems.Count;
						this.nDropItem = countUnvisibleItem + i + 1;
						DrawDragDropNonFlowInsert( correctRect.Bottom + this.nItemYSpacing/2, DragDropInsert.Middle, false );
						return;
					}

					rcitem.Y = rcitem.Bottom;
				}
			}

			if( this.Horizontal && bIsMirrored && !this.bFlowView )
			{
				rcitem.Y += this.ClientRectangle.Width - ( rcitem.Y * 2 + rcitem.Height * 2 );
			}

			// Draw the lower insert line.
			bool bNeedDrawLower = false;

			if( bFlowView )
			{
				if( this.Horizontal )
				{
					if( bIsMirrored )
					{
						bNeedDrawLower = ( pt.X > rcitem.Right && pt.X < ClientRectangle.Bottom ) ||
							( pt.Y < rcitem.Top && pt.Y > ClientRectangle.Left );
					}
					else
					{
						bNeedDrawLower = ( pt.X > rcitem.X && pt.X < this.ClientRectangle.Bottom ) ||
							( pt.Y > rcitem.Bottom && pt.Y < this.ClientRectangle.Width );
					}
				}
				else
				{
					if( bIsMirrored )
					{
						bNeedDrawLower = ( pt.Y > rcitem.Bottom && pt.Y < ClientRectangle.Bottom ) ||
							( pt.X < rcitem.Left && pt.X > ClientRectangle.Left && pt.Y > rcitem.Y );
					}
					else
					{
						bNeedDrawLower = ( pt.Y > rcitem.Bottom && pt.Y < ClientRectangle.Bottom ) ||
							( pt.X > rcitem.Right && pt.X < ClientRectangle.Right && pt.Y > rcitem.Y );
					}
				}
			}
			else
			{
				if( this.Horizontal )
				{
					if( bIsMirrored )
					{
						bNeedDrawLower = ( pt.Y > ClientRectangle.Left ) && ( pt.Y < rcitem.Bottom );
					}
					else
					{
						bNeedDrawLower = ( pt.Y > rcitem.Bottom ) && ( pt.Y < ClientRectangle.Right );
					}
				}
				else
				{
					bNeedDrawLower = ( pt.Y > rcitem.Bottom ) && ( pt.Y < ClientRectangle.Bottom );
				}
			}

			if( bNeedDrawLower )
			{
				this.nDropItem = cllnGroupViewItems.Count;

				if( bFlowView )
				{
					if( this.Horizontal )
					{
						DrawDragDropFlowInsert( rcitem.Y, rcitem.X + rcitem.Height + this.nItemYSpacing / 2, DragDropInsert.Lower, false );
					}
					else
					{
						if( this.VisibleItems.Count > 0 )
						{
							if( !bIsMirrored )
								DrawDragDropFlowInsert( rcitem.Right + this.nItemXSpacing / 2, rcitem.Y, DragDropInsert.Lower, false );
							else
								DrawDragDropFlowInsert( rcitem.Left - this.nItemXSpacing / 2, rcitem.Y, DragDropInsert.Upper, false );
						}
					}
				}
				else
				{
					if( this.Horizontal && bIsMirrored )
					{
						DrawDragDropNonFlowInsert( rcitem.Bottom - 7, DragDropInsert.Upper, false );
					}
					else
					{
						DrawDragDropNonFlowInsert( rcitem.Bottom + 7, DragDropInsert.Lower, false );
					}
				}

				return;
			}

			if( this.Horizontal )
			{
				int temp = pt.Y;
				pt.Y = pt.X;
				pt.X = temp;
			}

			// Moved out, erase insert lines.
			if( this.nDropItem != -1 )
			{
				ProcessDragDrop( MouseActions.Leave, pt );
				this.nDropItem = -1;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void ScrollTimerEventHandler( Object obj, EventArgs args )
		{
			// Determine the nature of the scroll event, by examining the cursor coords.
			Point ptCur = PointToClient( Cursor.Position );
			if( UpScrlBtnRect.Contains( ptCur ) )
				DoScroll( false, true );
			else
				DoScroll( true, true );
		}

		// Each call to DoScroll will move the items up or down by one scroll index.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DoScroll( bool bscrllup, bool bsettimer )
		{
			if( this.bFlowView == false )
			{
				if( bscrllup )
					this.nTopIndex = ( this.nTopIndex==this.VisibleItems.Count ) ? this.nTopIndex : this.nTopIndex+1;
				else
					this.nTopIndex = ( this.nTopIndex==0 ) ? 0 : this.nTopIndex-1;
			}
			else
			{
				int nreqdrows = (int)Math.Ceiling( (float)this.VisibleItems.Count/(float)this.nItemsPerRow );
				int nlastrowindex = ( nreqdrows-1 )*this.nItemsPerRow;
				if( bscrllup )
					this.nTopIndex = ( this.nTopIndex== nlastrowindex ) ? this.nTopIndex : this.nTopIndex+this.nItemsPerRow;
				else
					this.nTopIndex = ( this.nTopIndex==0 ) ? 0 : this.nTopIndex-this.nItemsPerRow;
				if( this.nTopIndex < 0 )
					this.nTopIndex = 0;
				if( this.nTopIndex > nlastrowindex )
					this.nTopIndex = nlastrowindex;
			}

			if( bsettimer == true )
			{
				if( this.tmrScrolling.Enabled == false )
					this.tmrScrolling.Start();
			}

			// Reset hover state, if necessary.
			if( this.nHighlightedItem >= 0 )
			{
				this.nHighlightedItem = -1;
				this.rcHighlightedItem = Rectangle.Empty;
			}

			this.Invalidate();
			this.Update();

			if( ( this.bIntegratedScrolling == true ) && ( this.Parent is IIntegratedScrollContainer ) )
			{
				IIntegratedScrollContainer iisc = this.Parent as IIntegratedScrollContainer;
				iisc.InvalidateUpScrollButton();
				iisc.InvalidateDownScrollButton();
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal Rectangle GetGroupViewItemBounds( int nindex, String type )
		{
			Rectangle rcitem = Rectangle.Empty;
			Rectangle rctext = Rectangle.Empty;
			for( int i = this.nTopIndex; i<this.VisibleItems.Count; i++ )
			{
				if( this.bFlowView == false )
				{
					this.SetNonFlowItemBounds( i, ref rcitem );

					rctext = this.GetAdjustedTextBounds( i, rcitem );
					SizeF szlabel = this.MeasureText( i, rctext );

					GroupViewItem item = this.VisibleItems[i] as GroupViewItem;

					if( this.bSmallImageView == false )
						rctext.X += ( rctext.Width - (int)szlabel.Width )/2;
					else
					{
						if( ( this.ilSmall != null ) && ( item.ImageIndex >= 0 ) && ( item.ImageIndex < this.ilSmall.Images.Count ) )
							rctext.Height = ( 16>Font.Height )?16:Font.Height;
						else
							rctext.Height = Font.Height;
					}
					rctext.Width = (int)szlabel.Width;
					if( i == nindex )
					{
						if( type == "Icon" )
							return rcitem;
						else if( type == "Text" )
							return rctext;
						else if( type == "FullItem" )
						{
							if( item.ImageIndex == -1 )
								return rctext;
							else if( item.Text == String.Empty )
								return rcitem;
							if( this.bSmallImageView )
								return new Rectangle( rcitem.Left, rcitem.Top, rctext.Right-rcitem.Left, rctext.Height );
							else
								return new Rectangle( rctext.Left, rcitem.Top, rctext.Width, rctext.Bottom-rcitem.Top );
						}
						else
						{
							Debug.Assert( false, "Invalid Type." );
							return Rectangle.Empty;
						}
					}
					if( this.bSmallImageView == true )
					{
						if( ( this.ilSmall != null ) && ( item.ImageIndex >= 0 ) && ( item.ImageIndex < this.ilSmall.Images.Count ) )
							rctext.Height = this.nImageSpacing + ( ( 16>Font.Height )?16:Font.Height ) + this.nImageSpacing;
						else
							rctext.Height = this.nImageSpacing + Font.Height + this.nImageSpacing;
					}
					rcitem = rctext;
					rcitem.Y = rcitem.Bottom;
				}
				else
				{
					this.SetFlowItemBounds( i, ref rcitem );
					if( i == nindex )
					{
						if( ( type == "Icon" ) || ( type == "FullItem" ) )
							return rcitem;
						else
						{
							Debug.Assert( false, "Invalid Type." );
							return Rectangle.Empty;
						}
					}
				}
			}
			return Rectangle.Empty;
		}

		// Private implementation of the IGroupViewDesignerInvoke interface.
		void IGroupViewDesignerInvoke.HandleMouseDown( MouseButtons button, Point pt )
		{
			this.HandleMouseDown( button, pt );
		}

		void IGroupViewDesignerInvoke.HandleMouseUp( MouseButtons button, Point pt )
		{
			this.HandleMouseUp( button, pt );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.Dispose"/>.
		/// </summary>
		protected override void Dispose( bool bdispose )
		{
			if( bdispose )
			{
				if( this.ctrlToolTip != null )
				{
					if( this.DesignMode == false )
						this.cllnGroupViewItems.Clear();
					this.ctrlToolTip.Dispose();

					if( this.ctrlTextBox != null )
					{
						this.ctrlTextBox.Dispose();
						this.ctrlToolTip = null;
					}
					if( this.tdToolbar != null )
					{
						this.tdToolbar.Dispose();
						this.tdToolbar = null;
					}
					if( this.tdScrollBar != null )
					{
						this.tdScrollBar.Dispose();
						this.tdScrollBar = null;
					}
					if( m_toolTip != null )
					{
						m_toolTip.Dispose();
						m_toolTip = null;
					}
				}
			}
			base.Dispose( bdispose );
		}

		// Accessibility Implementation
		[Syncfusion.Documentation.DocumentationExclude()]
		protected GroupViewItemAccessibleObjectsIndexer itemAccessibleObjects = null;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal GroupViewItemAccessibleObjectsIndexer gviAccessibleObjects
		{
			get
			{
				if( itemAccessibleObjects == null )
					itemAccessibleObjects = new GroupViewItemAccessibleObjectsIndexer( this );
				return itemAccessibleObjects;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal GroupViewItemAccessibleObject CreateGroupViewItemAccessibilityInstance( int index )
		{
			return new GroupViewItemAccessibleObject( this.GroupViewItems[index] );
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			return new GroupViewControlAccessibleObject( this );
		}

		// Helper function that performs HitTesting.
		[Syncfusion.Documentation.DocumentationExclude()]
		public int GetItemUnderPoint( Point pt )
		{
			int nindex = -1;
			if( ( pt.X != -1 ) && ( pt.Y != -1 ) )
			{
				Rectangle rcitem = Rectangle.Empty;
				for( nindex = this.nTopIndex; nindex<this.VisibleItems.Count; nindex++ )
				{
					GroupViewItem item = (GroupViewItem)this.VisibleItems[nindex];
					if( this.bFlowView == false )
						this.SetNonFlowItemBounds( nindex, ref rcitem );
					else
						this.SetFlowItemBounds( nindex, ref rcitem );

					if( this.IsMouseOverItem( nindex, rcitem, pt ) == true )
						return nindex;
					if( this.bFlowView == false )
						rcitem.Y = rcitem.Bottom;
				}
			}
			return -1;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void FireGroupViewItemSelectedEvent( int nindex )
		{
			if( ( nindex >= this.nTopIndex ) && ( nindex <= this.CalculateItemsPerPage() ) )
				this.OnGroupViewItemSelected( EventArgs.Empty );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool GetIsMirrored()
		{
			return RightToLeft.Yes == RightToLeft;
		}

		private void cllnGroupViewItems_CollectionChanged( object sender, CollectionChangeEventArgs e )
		{
			RecalculateVisibleItem();
		}

		private void RecalculateVisibleItem()
		{
			VisibleItems.Clear();

			if( cllnGroupViewItems != null && cllnGroupViewItems.Count > 0 )
			{
				foreach( GroupViewItem item in cllnGroupViewItems )
				{
					if( item.Visible )
					{
						VisibleItems.Add( item );
					}
				}
			}
		}
	}


	/// <summary>
	/// ControlAccessibleObject derived class that implements the Accessibility object for the GroupView control.
	/// </summary>
	public class GroupViewControlAccessibleObject: Control.ControlAccessibleObject
	{
		protected GroupView ctrlGroupView;

		public GroupViewControlAccessibleObject( GroupView gvctrl )
			: base( gvctrl )
		{
			this.ctrlGroupView = gvctrl;
		}

		// Gets the role for the GroupView. This is used by accessibility programs.
		public override AccessibleRole Role
		{
			get { return AccessibleRole.List; }
		}

		public override string Name
		{
			get { return this.ctrlGroupView.AccessibleName; }
		}

		public override Rectangle Bounds
		{
			get { return this.ctrlGroupView.RectangleToScreen( this.ctrlGroupView.ClientRectangle ); }
		}

		public override string Description
		{
			get { return this.ctrlGroupView.AccessibleDescription; }
		}

		public override string Help
		{
			get { return String.Empty; }
		}

		public override AccessibleObject Parent
		{
			get { return this.ctrlGroupView.Parent.AccessibilityObject; }
		}

		// Gets the state for the GroupView. This is used by accessibility programs.
		public override AccessibleStates State
		{
			get { return ( this.ctrlGroupView.Visible ? AccessibleStates.None : AccessibleStates.Invisible ); }
		}

		// The GroupViewItem objects are "child" controls in terms of accessibility so 
		// return the number of GroupViewItems.
		public override int GetChildCount()
		{
			return this.ctrlGroupView.GroupViewItems.Count;
		}

		// Gets the Accessibility object of the GroupViewItem identified by index.
		public override AccessibleObject GetChild( int index )
		{
			if( index < this.ctrlGroupView.GroupViewItems.Count )
				return this.ctrlGroupView.gviAccessibleObjects[index];
			return null;
		}

		public override string Value
		{
			get { return this.ctrlGroupView.Text; }
			set { this.ctrlGroupView.Text = value; }
		}

		// This function is used by the GroupViewItemAccessibleObject.Navigate function.
		public AccessibleObject NavigateFromChild( GroupViewItemAccessibleObject child, AccessibleNavigation navdir )
		{
			int index = child.Index;

			switch( navdir )
			{
				case AccessibleNavigation.FirstChild:
				index = 0;
				break;

				case AccessibleNavigation.LastChild:
				index = this.ctrlGroupView.GroupViewItems.Count-1;
				break;

				case AccessibleNavigation.Previous:
				case AccessibleNavigation.Up:
				case AccessibleNavigation.Left:
				if( index > 0 )
					index--;
				break;
				case AccessibleNavigation.Next:
				case AccessibleNavigation.Down:
				case AccessibleNavigation.Right:
				if( index < this.ctrlGroupView.GroupViewItems.Count )
					index++;
				break;
			}

			return this.GetChild( index );
		}

		// This function is used by the GroupViewItemAccessibleObject.Select function.
		public void SelectChild( GroupViewItemAccessibleObject child, AccessibleSelection selection )
		{
			if( ( selection & AccessibleSelection.TakeSelection ) != 0 )
			{
				if( this.ctrlGroupView.ButtonView == true )
					this.ctrlGroupView.SelectedItemInternal = child.Index;
				else
				{
					this.ctrlGroupView.nHighlightedItem = child.Index;
					this.ctrlGroupView.nSelectedItem = child.Index;
					this.ctrlGroupView.FireGroupViewItemSelectedEvent( child.Index );
					this.ctrlGroupView.nHighlightedItem = -1;
					this.ctrlGroupView.nSelectedItem = -1;
				}
			}
		}

		public override AccessibleObject GetFocused()
		{
			if( ( this.ctrlGroupView.Focused ) && ( this.ctrlGroupView.ButtonView ) )
				return this.GetSelected();
			else
				return base.GetFocused();
		}

		public override AccessibleObject GetSelected()
		{
			int nitemselected = this.ctrlGroupView.SelectedItemInternal;
			if( nitemselected != -1 )
				return this.GetChild( nitemselected );
			return base.GetSelected();
		}

		public override AccessibleObject HitTest( int x, int y )
		{
			Point pt = this.ctrlGroupView.PointToClient( new Point( x, y ) );
			int nitemhit = this.ctrlGroupView.GetItemUnderPoint( pt );
			if( nitemhit != -1 )
				return this.ctrlGroupView.gviAccessibleObjects[nitemhit];
			return base.HitTest( x, y );
		}

		public override AccessibleObject Navigate( AccessibleNavigation navdir )
		{
			if( this.ctrlGroupView.SelectedItemInternal != -1 )
			{
				if( ( navdir == AccessibleNavigation.Down ) || ( navdir == AccessibleNavigation.Next ) || ( navdir == AccessibleNavigation.Right ) )
				{
					int nextitem = this.ctrlGroupView.SelectedItemInternal+1;
					if( nextitem < this.GetChildCount() )
						return this.GetChild( nextitem );
				}
				else if( ( navdir == AccessibleNavigation.Up ) || ( navdir == AccessibleNavigation.Previous ) || ( navdir == AccessibleNavigation.Left ) )
				{
					int previtem = this.ctrlGroupView.SelectedItemInternal-1;
					if( previtem >= 0 )
						return this.GetChild( previtem );
				}
			}
			return base.Navigate( navdir );
		}
	}


	public class GroupViewItemAccessibleObject: AccessibleObject
	{
		GroupView ctrlGroupView;
		GroupViewItem itemGroupView;
		int nIndex;

		public GroupViewItemAccessibleObject( GroupViewItem gvitem )
		{
			this.itemGroupView = gvitem;
			this.ctrlGroupView = gvitem.GroupView;
			this.nIndex = this.ctrlGroupView.GroupViewItems.IndexOf( gvitem );
		}

		public int Index
		{
			get { return this.nIndex; }
		}

		public GroupViewControlAccessibleObject GroupViewAccessibilityObject
		{
			get { return this.ctrlGroupView.AccessibilityObject as GroupViewControlAccessibleObject; }
		}

		public override void Select( AccessibleSelection flags )
		{
			if( ( flags & AccessibleSelection.TakeFocus ) != 0 )
			{
				if( !this.ctrlGroupView.Focused )
					this.ctrlGroupView.Focus();
			}
			if( ( flags & AccessibleSelection.TakeSelection ) != 0 )
				this.GroupViewAccessibilityObject.SelectChild( this, flags );
		}

		public override AccessibleObject Navigate( AccessibleNavigation navdir )
		{
			return this.GroupViewAccessibilityObject.NavigateFromChild( this, navdir );
		}

		public override void DoDefaultAction()
		{
			this.Select( AccessibleSelection.TakeSelection );
		}

		public override AccessibleObject GetFocused()
		{
			return this.GroupViewAccessibilityObject.GetFocused();
		}

		public override AccessibleStates State
		{
			get
			{

				AccessibleStates accessiblestates = AccessibleStates.None;
				if( this.ctrlGroupView.IsItemVisible( this.nIndex ) == true )
				{
					if( this.ctrlGroupView.AllowDragDrop )
						accessiblestates = AccessibleStates.Moveable;
					if( this.ctrlGroupView.ButtonView )
						accessiblestates |= AccessibleStates.Selectable;
					if( this.ctrlGroupView.HighlightedItem == this.nIndex )
						accessiblestates |= AccessibleStates.HotTracked;
					if( this.ctrlGroupView.SelectedItemInternal == this.nIndex )
						accessiblestates |= AccessibleStates.Selected;
				}
				else
				{
					accessiblestates |= AccessibleStates.Offscreen|AccessibleStates.Invisible;
				}

				return accessiblestates;
			}
		}

		public override AccessibleRole Role
		{
			get { return AccessibleRole.ListItem; }
		}

		public override AccessibleObject Parent
		{
			get { return this.ctrlGroupView.AccessibilityObject; }
		}

		public override string Name
		{
			get { return this.ctrlGroupView.GroupViewItems[this.nIndex].Text; }
		}

		public override string DefaultAction
		{
			get { return "Click"; }
		}

		public override Rectangle Bounds
		{
			get { return this.ctrlGroupView.RectangleToScreen( this.ctrlGroupView.GetGroupViewItemBounds( this.nIndex, "FullItem" ) ); }
		}

		public override string Description
		{
			get { return this.ctrlGroupView.GroupViewItems[this.nIndex].Text; }
		}
	}


	public class GroupViewItemAccessibleObjectsIndexer
	{
		ArrayList data;
		GroupView ctrlGroupView;

		public GroupViewItemAccessibleObjectsIndexer( GroupView gvctrl )
		{
			this.ctrlGroupView = gvctrl;
			this.data = new ArrayList();
		}

		public GroupViewItemAccessibleObject GetItem( int index )
		{
			// Returns NULL if GroupViewItem is not found.
			return index < data.Count ? data[index] as GroupViewItemAccessibleObject : null;
		}

		public void SetItem( int index, GroupViewItemAccessibleObject accobj )
		{
			if( index >= data.Count )
			{
				object[] newItems = new object[index-data.Count+1];
				data.AddRange( newItems );
			}
			data[index] = accobj;
		}

		public void ResetItem( int index )
		{
			if( index < data.Count )
				data[index] = null;
		}

		[Browsable( false )]
		public GroupViewItemAccessibleObject this[int index]
		{
			get
			{
				GroupViewItemAccessibleObject accObj = GetItem( index );
				if( accObj == null )
				{
					accObj = this.ctrlGroupView.CreateGroupViewItemAccessibilityInstance( index );
					// Save weak reference to object.
					SetItem( index, accObj );
				}
				return accObj;
			}
		}
	}

	public enum GroupViewOrientation
	{
		Vertical,
		Horizontal
	}
}
