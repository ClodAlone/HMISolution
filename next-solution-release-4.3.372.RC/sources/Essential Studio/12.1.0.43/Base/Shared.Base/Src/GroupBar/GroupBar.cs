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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Windows.Forms.Tools.Design;
using System.Drawing.Design;


namespace Syncfusion.Windows.Forms.Tools
{
	[Syncfusion.Documentation.DocumentationExclude()]
	public enum GroupLayout
	{
		Vertical=1,
		Horizontal
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	public enum ButtonsState
	{
		None=0,
		GroupBarItemHot=1,
		GroupBarItemPushed,
		ScrollUpPushed,
		ScrollDownPushed,
		ScrollUpHot,
		ScrollDownHot,
		DropDownButtonHot,
		DropDownButtonPushed
	}

	public enum IconRenderingMode
	{
		AlphaBlended,
		Default
	}

	/// <summary>
	/// Specifies the colors used for drawing the <see cref="GroupBar"/> control's client rectangle borders.
	/// </summary>
	/// <remarks>
	/// The BorderColors structure is used by the <see cref="GroupBarItem"/> objects in a <see cref="GroupBar"/> 
	/// to specify the set of colors used for drawing the borders around the client control. The 
	/// BorderColors value is set through the <see cref="GroupBarItem.ClientBorderColors"/> property.
	/// </remarks>
	[
	StructLayout( LayoutKind.Sequential ),
	TypeConverter( typeof( Syncfusion.Windows.Forms.Tools.Design.BorderColorsConverter ) )
	]
	public struct BorderColors
	{
		private Color clrLeft;
		private Color clrTop;
		private Color clrRight;
		private Color clrBottom;

		/// <summary>
		/// Gets / sets the color used to draw the left border.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Color"/> value.</value>
		public Color Left
		{
			get { return this.clrLeft; }
			set { this.clrLeft = value; }
		}

		/// <summary>
		/// Gets / sets the color used to draw the top border.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Color"/> value.</value>
		public Color Top
		{
			get { return this.clrTop; }
			set { this.clrTop = value; }
		}

		/// <summary>
		/// Gets / sets the color used to draw the right border.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Color"/> value.</value>
		public Color Right
		{
			get { return this.clrRight; }
			set { this.clrRight = value; }
		}

		/// <summary>
		/// Gets / sets the color used to draw the bottom border.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Color"/> value.</value>
		public Color Bottom
		{
			get { return this.clrBottom; }
			set { this.clrBottom = value; }
		}

		/// <summary>
		/// Represents a <see cref="BorderColors"/> instance with empty color values.
		/// </summary>
		public static BorderColors Empty
		{
			get { return new BorderColors( Color.Empty, Color.Empty, Color.Empty, Color.Empty ); }
		}

		/// <summary>
		/// Represents a <see cref="BorderColors"/> instance with the default color values.
		/// </summary>
		public static BorderColors Default
		{
			get
			{
				return new BorderColors( SystemColors.ControlDarkDark, SystemColors.ControlDarkDark,
					SystemColors.Control, SystemColors.Control );
			}
		}

		/// <summary>
		/// Creates a new instance of the <see cref="BorderColors"/> class with the specified colors.
		/// </summary>
		/// <param name="left">Left border color.</param>
		/// <param name="top">Top border color.</param>
		/// <param name="right">Right border color.</param>
		/// <param name="bottom">Bottom border color.</param>
		public BorderColors( Color left, Color top, Color right, Color bottom )
		{
			this.clrLeft = left;
			this.clrTop = top;
			this.clrRight = right;
			this.clrBottom = bottom;
		}
	}

	#region GroupBarItem

	/// <summary>
	/// Represents an item in the <see cref="GroupBar"/> control.
	/// </summary>
	/// <remarks>
	/// The GroupBar control is composed of a number of selectable groups or items each of which is 
	/// associated with a client control. Each of these items is an instance of the GroupBarItem type. 
	/// The collection of items present in the GroupBar can be accessed through the control's 
	/// <see cref="Syncfusion.Windows.Forms.Tools.GroupBar.GroupBarItems"/> property.
	/// </remarks>
	[
	DesignTimeVisible( false ),
	ToolboxItem( false ),
	DefaultProperty( "Client" )
	]
	public class GroupBarItem: Component, ICustomTypeDescriptor
	{
		#region Fields

		// Behavior attributes:
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Control wndClient = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bEnabled = true;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bVisible = true;

		// Appearance attributes:
		[Syncfusion.Documentation.DocumentationExclude()]
		protected String szText = String.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Icon iItemIcon = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Image iItemImage = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Font ftText = null; // new Font("Tahoma", 8);
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bFontSet = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Color clrForeGround = Color.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Brush brBackground = new SolidBrush( SystemColors.Control );
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bBackColorSet = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected BorderColors bdrColors = BorderColors.Default;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bBorderColorsSet = false;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected object objTag = null;

		// Host GroupBar reference:
		[Syncfusion.Documentation.DocumentationExclude()]
		protected GroupBar groupBarCtrl = null;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bInNavigationPane = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Icon iNavPaneIcon = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool imageYchanged = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool textDraw = true;
		[Syncfusion.Documentation.DocumentationExclude()]
		private bool m_bLargeImageMode = false;
		/// <summary>
		/// The image representing the item in the GroupBar's navigation pane.
		/// </summary>
		private Image iNavPaneImage = null;
		#endregion

		[
		Browsable( false ),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void GroupBar_FontChanged( Object sender, EventArgs e )
		{
            if (this.groupBarCtrl == null)
                return;
			Debug.Assert( sender == this.groupBarCtrl );
			if( this.bFontSet == false )
				this.ResetFont();
		}

		#region Properties

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void GroupBar_BackColorChanged( Object sender, EventArgs e )
		{
			Debug.Assert( sender == this.groupBarCtrl );
			if( this.bBackColorSet == false )
				this.ResetBackColor();
			if( this.bBorderColorsSet == false )
				this.ResetClientBorderColors();
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public bool IsTextVisible
		{
			get
			{
				return this.textDraw;
			}
			set
			{
				if( this.textDraw != value )
					this.textDraw = value;
			}
		}

		/// <summary>
		/// Indicates whether all types of images can be used or not
		/// </summary>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		DefaultValue( false ),
		Category( "Appearance" ),
		RefreshProperties( RefreshProperties.All )
		]
		public bool LargeImageMode
		{
			get
			{
				return m_bLargeImageMode;
			}
			set
			{
				m_bLargeImageMode = value;
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public bool YChanged
		{
			get
			{
				return this.imageYchanged;
			}
			set
			{
				this.imageYchanged = value;
			}
		}
		/// <summary>
		/// Gets / sets the text displayed on the <see cref="GroupBarItem"/>.
		/// </summary>
		/// <value>A String value.</value>
		[
		Description( "The text displayed in the GroupBarItem." ),
		Category( "Appearance" ),
		Localizable( true ),
        Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))
		]
		public String Text
		{
			get { return szText; }
			set
			{
				if( this.szText != value )
				{
					this.szText = value;
					this.OnPropertyChanged( new PropertyChangedEventArgs( "Text" ) );
				}
			}
		}

		/// <summary>
		/// Padding provides spacing between the icons (images) and the text in the <see cref="GroupBarItem"/>.
		/// </summary>
		private int m_padding = 0;

		/// <summary>
		/// Gets or sets padding for <see cref="GroupBarItem"/>.
		/// </summary>
		[DefaultValue( 0 )]
		[Description( "Gets or sets padding for GroupBarItem" )]
		[Category( "Appearance" )]
		public int Padding
		{
			get
			{
				return m_padding;
			}
			set
			{
				if( m_padding != value )
				{
					if( value < 0 )
						throw new ArgumentException( "Value cannot be negative." );

					m_padding = value;

					this.OnPropertyChanged( new PropertyChangedEventArgs( "Padding" ) );
				}
			}
		}

		public bool ShouldSerializePadding()
		{
			if( m_padding == 0 )
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Gets / sets the client control associated with the <see cref="GroupBarItem"/>.
		/// </summary>
		/// <value>A <see cref="System.Windows.Forms.Control"/> instance.</value>
		[
		Description( "The client control associated with this item." ),
		Category( "Behavior" )
		]
		public Control Client
		{
			get { return this.wndClient; }
			set
			{
				if( this.wndClient != value )
				{
					Control client = this.wndClient;
					this.wndClient = null;

					if( client != null )
					{
						this.groupBarCtrl.Controls.Remove( client );
					}

					if( value != null )
					{
						if( this.groupBarCtrl == null )	// During document loading
						{
							if( value.IsHandleCreated == false )
								value.CreateControl();
							value.Visible = false;
							this.wndClient = value;
						}
						else

							// If the control is one of the parents of the GroupBar or the GroupBar itself, then ignore the assignment						
							if( (Syncfusion.Runtime.InteropServices.NativeMethods.IsChild( value.Handle, this.groupBarCtrl.Handle ) == false) && 
                            (value != this.groupBarCtrl) )
							{
								if( value.IsHandleCreated == false )
									value.CreateControl();
								value.Visible = false;
								this.wndClient = value;
								if( this.groupBarCtrl.Controls.Contains( value ) == false )
								{
									this.groupBarCtrl.IsClientSetting = true;
									this.groupBarCtrl.Controls.Add( value );
									this.groupBarCtrl.IsClientSetting = false;

									this.groupBarCtrl.RecalculateGroupBarLayout();
									this.groupBarCtrl.SetActiveClientBounds();
								}
							}
					}
					else
						this.OnPropertyChanged( new PropertyChangedEventArgs( "Client" ) );
				}
			}
		}

		/// <summary>
		/// Gets / sets the image displayed on the <see cref="GroupBarItem"/>. 
		/// </summary>
		/// <value>An <see cref="System.Drawing.Icon"/> value.</value>
		[
		Description( "The image displayed on the GroupBarItem." ),
		Category( "Appearance" ),
		DefaultValue( null ),
		Localizable( true )
		]
		public Image Image
		{
			get { return iItemImage; }
			set
			{
				if( this.iItemImage != value )
				{
					this.iItemImage = value;
					this.OnPropertyChanged( new PropertyChangedEventArgs( "Image" ) );
				}
			}
		}
		/// <summary>
		/// Gets / sets the icon displayed on the <see cref="GroupBarItem"/>. 
		/// </summary>
		/// <value>An <see cref="System.Drawing.Icon"/> value.</value>
		[
		Description( "The icon displayed on the GroupBarItem." ),
		Category( "Appearance" ),
		DefaultValue( null ),
		Localizable( true )
		]
		public Icon Icon
		{
			get { return iItemIcon; }
			set
			{
				if( this.iItemIcon != value )
				{
					this.iItemIcon = value;
					this.OnPropertyChanged( new PropertyChangedEventArgs( "Icon" ) );
				}
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="GroupBarItem"/> is enabled / disabled.
		/// </summary>
		/// <value>False if the item is disabled. The default is True.</value>
		[
		Description( "Indicates whether the item is enabled / disabled." ),
		Category( "Behavior" ),
		DefaultValue( true ),
		Localizable( true )
		]
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
		/// Indicates whether the <see cref="GroupBarItem"/> is visible.
		/// </summary>
		/// <value>False if the item is hidden. The default is True.</value>
		[
		Description( "Indicates whether the item is visible." ),
		Category( "Behavior" ),
		DefaultValue( true ),
		Localizable( true )
		]
		public bool Visible
		{
			get { return this.bVisible; }
			set
			{
				if( this.bVisible != value )
				{
					this.bVisible = value;
					this.OnPropertyChanged( new PropertyChangedEventArgs( "Visible" ) );
				}
			}
		}

		/// <summary>
		/// Gets / sets the font used for drawing the <see cref="GroupBarItem"/> text.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Font"/> value.</value>
		[
		Description( "The font used to display the text in the GroupBarItem." ),
		Category( "Appearance" ),
		Localizable( true )
		]
		public Font Font
		{
			get { return this.ftText; }
			set
			{
				if( this.ftText != value )
				{
					this.ftText = value;
					this.bFontSet = true;
					this.OnPropertyChanged( new PropertyChangedEventArgs( "Font" ) );
				}
			}
		}

		protected bool ShouldSerializeFont()
		{
			return this.bFontSet;
		}

		/// <summary>
		/// Resets the <see cref="Font"/> property to its default value.
		/// </summary>
		public void ResetFont()
		{
			this.Font = this.groupBarCtrl.Font;
			this.bFontSet = false;
		}

		/// <summary>
		/// Gets / sets the foreground color used to paint the text in the <see cref="GroupBarItem"/>.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Color"/> value.</value>
		[
		Description( "The foreground color used to paint the text in the GroupBarItem." ),
		Category( "Appearance" )
		]
		public Color ForeColor
		{
			get
			{
				if (clrForeGround.IsEmpty)
				{
					if (groupBarCtrl != null)
					{
						return groupBarCtrl.ForeColor;
					}
					return SystemColors.ControlText;
				}
				return this.clrForeGround;
			}
			set
			{
				if( this.clrForeGround != value )
				{
					this.clrForeGround = value;
					this.OnPropertyChanged( new PropertyChangedEventArgs( "ForeColor" ) );
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected bool ShouldSerializeForeColor()
		{
			return !this.clrForeGround.IsEmpty;
		}
		/// <summary>
		/// Resets the <see cref="ForeColor"/> property to its default value.
		/// </summary>
		public void ResetForeColor()
		{
			this.ForeColor = Color.Empty;
		}

		/// <summary>
		/// Gets / sets the color used to fill the <see cref="GroupBarItem"/> background.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Color"/> value.</value>
		[
		Description( "The background color used to fill the GroupBarItem." ),
		Category( "Appearance" )
		]
		public Color BackColor
		{
			get
			{
				if( this.brBackground is SolidBrush )
					return (this.brBackground as SolidBrush).Color;
				else return this.groupBarCtrl.BackColor; //return SystemColors.Control;
			}
			set
			{
				if( this.brBackground is SolidBrush )
				{
					if( (this.brBackground as SolidBrush).Color != value )
					{
						this.brBackground = new SolidBrush( value );
						this.bBackColorSet = true;
						this.OnPropertyChanged( new PropertyChangedEventArgs( "BackColor" ) );
					}
				}
				else
				{
					this.brBackground = new SolidBrush( value );
					this.bBackColorSet = true;
					this.OnPropertyChanged( new PropertyChangedEventArgs( "BackColor" ) );
				}
			}
		}

		protected bool ShouldSerializeBackColor()
		{
			return this.bBackColorSet;
		}

		/// <summary>
		/// Resets the <see cref="BackColor"/> property to its default value.
		/// </summary>
		public void ResetBackColor()
		{
			this.BackColor = this.groupBarCtrl.BackColor;
			this.bBackColorSet = false;
		}

		/// <summary>
		/// Gets / sets the brush used for painting the <see cref="GroupBarItem"/> background.
		/// </summary>
		/// <value>A <see cref="System.Drawing.Brush"/> value.</value>
		[
		Browsable( false )
		]
		public Brush BackgroundBrush
		{
			get { return this.brBackground; }
			set
			{
				if( this.brBackground != value )
				{
					this.brBackground = value;
					this.bBackColorSet = true;
					this.OnPropertyChanged( new PropertyChangedEventArgs( "BackgroundBrush" ) );
				}
			}
		}

		/// <summary>
		/// Gets / sets the colors used to draw the borders around the <see cref="GroupBarItem"/>'s client 
		/// control.
		/// </summary>
		/// <value>A <see cref="BorderColors"/> value.</value>
		[
		Description( "The colors used to draw the borders around the client control." ),
		Category( "Appearance" )
		]
		public BorderColors ClientBorderColors
		{
			get { return this.bdrColors; }
			set
			{
				if( (this.bdrColors.Left != value.Left) || (this.bdrColors.Top != value.Top) || 
					(this.bdrColors.Right != value.Right) || (this.bdrColors.Bottom != value.Bottom) )
				{
					this.bdrColors = value;
					this.bBorderColorsSet = true;
					this.OnPropertyChanged( new PropertyChangedEventArgs( "ClientBorderColors" ) );
				}
			}
		}

		protected bool ShouldSerializeClientBorderColors()
		{
			return this.bBorderColorsSet;
		}

		/// <summary>
		/// Resets the <see cref="ClientBorderColors"/> property to its default value.
		/// </summary>
		public void ResetClientBorderColors()
		{
			Color border = this.groupBarCtrl.BackColor;
			Color borderdarkdark = ControlPaint.DarkDark( this.groupBarCtrl.BackColor );
			this.ClientBorderColors = new BorderColors( borderdarkdark, borderdarkdark, border, border );
			this.bBorderColorsSet = false;
		}

		/// <summary>
		/// Gets / sets the object that contains data about the <see cref="GroupBarItem"/>.
		/// </summary>
		/// <value>
		/// A <see cref="System.Object"/> value that contains data about the control. 
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
		Description( "Gets / sets the object that contains data about the item." ),
		Category( "Data" )
		]
		public object Tag
		{
			get { return this.objTag; }
			set { this.objTag = value; }
		}

		/// <summary>
		/// Returns the GroupBar control that the item is assigned to.
		/// </summary>
		/// <value>
		/// A <see cref="GroupBar"/> that represents the parent GroupBar control that the <see cref="GroupBarItem"/> is assigned to.
		/// </value>
		[
		Browsable( false )
		]
		public GroupBar GroupBar
		{
			get { return this.groupBarCtrl; }
		}

		/// <summary>
		/// Indicates whether the <see cref="GroupBarItem"/> should be added to the navigation pane.
		/// </summary>
		/// <remarks>
		/// This property is valid only when the <see cref="GroupBar"/> control is in the stacked mode.
		/// <seealso cref="Syncfusion.Windows.Forms.Tools.GroupBar.StackedMode"/>
		/// </remarks>
		/// <value>TRUE if the item should be added to the navigation pane. The default is FALSE.</value>
		[
		Description( "Indicates whether the item should be added to the GroupBar's navigation pane." ),
		Category( "Stacked Mode" ),
		DefaultValue( false ),
		Localizable( true )
		]
		public bool InNavigationPane
		{
			get { return this.bInNavigationPane; }
			set
			{
				if( this.bInNavigationPane != value )
				{
					if( value == true )
					{
						if( (this.groupBarCtrl != null) && (this.groupBarCtrl.StackedMode == false) )
							throw (new ApplicationException( "This property can be set only when the GroupBar is in the StackedMode." ));
					}

					this.bInNavigationPane = value;
					this.OnPropertyChanged( new PropertyChangedEventArgs( "InNavigationPane" ) );
				}
			}
		}

		/// <summary>
		/// Gets / sets the icon representing the <see cref="GroupBarItem"/> in the navigation pane. 
		/// </summary>
		/// <remarks>
		/// This property is valid only when the <see cref="GroupBar"/> control is in the stacked mode.
		/// <seealso cref="Syncfusion.Windows.Forms.Tools.GroupBar.StackedMode"/>
		/// </remarks>
		/// <value>An <see cref="System.Drawing.Icon"/> value.</value>
		[
		Description( "The icon representing the item in the GroupBar's navigation pane." ),
		Category( "Stacked Mode" ),
		DefaultValue( null ),
		Localizable( true )
		]
		public Icon NavigationPaneIcon
		{
			get { return iNavPaneIcon; }
			set
			{
				if( this.iNavPaneIcon != value )
				{
					this.iNavPaneIcon = value;
					this.OnPropertyChanged( new PropertyChangedEventArgs( "NavigationPaneIcon" ) );
				}
			}
		}

		/// <summary>
		/// Gets / sets image representing the item in the GroupBar's navigation pane.
		/// </summary>
		[
			Description( "Gets or sets image representing the item in the GroupBar's navigation pane." ),
			Category( "Stacked Mode" ),
			DefaultValue( null ),
			Localizable( true )
		]
		public Image NavigationPaneImage
		{
			get
			{
				return iNavPaneImage;
			}
			set
			{
				if( this.iNavPaneImage != value )
				{
					this.iNavPaneImage = value;
					this.OnPropertyChanged( new PropertyChangedEventArgs( "NavigationPaneImage" ) );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal Color ForeColorInternal
		{
			get
			{
				return clrForeGround;
			}
		}

		#endregion

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void SetGroupBarControlReference( GroupBar ctrl )
		{
			this.groupBarCtrl = ctrl;
			if( this.groupBarCtrl != null )
			{
				if( this.bFontSet == false )
					this.ftText = this.groupBarCtrl.Font;
				if( this.bBackColorSet == false )
					this.BackColor = this.groupBarCtrl.BackColor;
				this.groupBarCtrl.FontChanged += new EventHandler( this.GroupBar_FontChanged );
				this.groupBarCtrl.BackColorChanged += new EventHandler( this.GroupBar_BackColorChanged );
			}
		}

		/// <summary>
		/// Creates an instance of the <see cref="GroupBarItem"/> class.
		/// </summary>
		public GroupBarItem()
		{
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void OnPropertyChanged( PropertyChangedEventArgs args )
		{
			if( this.PropertyChanged != null )
			{
				PropertyChanged( this, args );
			}
		}

		/// <summary>
		/// Overridden. See <see cref="System.ComponentModel.Component.Dispose(bool)"/>.
		/// </summary>
		protected override void Dispose( bool bdispose )
		{
			if( bdispose )
			{
				this.wndClient = null;
				this.groupBarCtrl = null;
			}
			base.Dispose( bdispose );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal bool InDesignMode
		{
			get { return this.DesignMode; }
		}

		// ICustomTypeDescriptor Implementation - Required for showing/hiding the Stacked Mode related properties depending on 
		// whether the GroupBar is in the StackedMode
		#region ICustomTypeDescriptor Implementation

		[Syncfusion.Documentation.DocumentationExclude()]
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties( Attribute[] attributes )
		{
			PropertyDescriptorCollection baseprops = TypeDescriptor.GetProperties( this, attributes, true );

			// Remove the InNavigationPane and NavigationPaneIcon properties when not in StackedMode.
			bool bcontainsproperties = false;
			bool bLargeImageMode = false;
			string strSkipProperty = "";

			foreach( PropertyDescriptor propdesc in baseprops )
			{
				if( propdesc.Category == "Stacked Mode" )
				{
					bcontainsproperties = true;
				}
				if( propdesc.Category == "Appearance" )
				{
					if( this.groupBarCtrl != null 
						&& !this.groupBarCtrl.IsInitializing 
						&& propdesc.Name == "LargeImageMode" )
					{
						bLargeImageMode = (bool)propdesc.GetValue( this );
						if( bLargeImageMode )
						{
							strSkipProperty = "Icon";
						}
						else
						{
							strSkipProperty = "Image";
						}
					}
				}
			}

			int i = 0;
			ArrayList arrayNewProp = new ArrayList();
			if( (bcontainsproperties) && (this.groupBarCtrl != null) && (!this.groupBarCtrl.StackedMode) )
			{
				for( i = 0; i < baseprops.Count; i++ )
				{
					PropertyDescriptor prop = baseprops[i];
					if( prop.Name == strSkipProperty || prop.Category == "Stacked Mode" )
					{
						if( prop.Name == strSkipProperty )
						{
							ResetValue( prop );
						}
						continue;
					}
					arrayNewProp.Add( prop );
				}
			}
			else
			{
				for( i = 0; i < baseprops.Count; i++ )
				{
					PropertyDescriptor prop = baseprops[i];
					if( prop.Name == strSkipProperty )
					{
						ResetValue( prop );
						i++;
						prop = baseprops[i];
					}
					arrayNewProp.Add( prop );
				}
			}

			i = 0;
			PropertyDescriptor[] arrayNewProperties = new PropertyDescriptor[arrayNewProp.Count];
			foreach( PropertyDescriptor prop in arrayNewProp )
			{
				arrayNewProperties[i++] = prop;
			}

			return new PropertyDescriptorCollection( arrayNewProperties );
		}

		private void ResetValue( PropertyDescriptor prop )
		{
			if( (prop.GetValue( this )) != null )
				prop.ResetValue( this );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes( this, true );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName( this, true );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		string ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName( this, true );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter( this, true );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent( this, true );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents( System.Attribute[] attributes )
		{
			return TypeDescriptor.GetEvents( this, attributes, true );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents( this, true );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty( this, true );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return TypeDescriptor.GetProperties( this, true );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		object ICustomTypeDescriptor.GetEditor( System.Type editorBaseType )
		{
			return TypeDescriptor.GetEditor( this, editorBaseType, true );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		object ICustomTypeDescriptor.GetPropertyOwner( System.ComponentModel.PropertyDescriptor pd )
		{
			return this;
		}

		#endregion	// ICustomTypeDescriptor

	}

	#endregion	// GroupBarItem

	/// <summary>
	/// Specifies an alignment value for the <see cref="GroupBarItem"/> text.
	/// </summary>
	/// <remarks>
	/// The TextAlignment enum is used for specifying a value for the <see cref="GroupBar"/> 
	/// control's <see cref="GroupBar.TextAlign"/> property.
	/// </remarks>
	public enum TextAlignment
	{
		/// <summary>
		/// The text is aligned to the left.
		/// </summary>
		Left=1,

		/// <summary>
		/// The text is horizontally centered.
		/// </summary>
		Center,

		/// <summary>
		/// The text is aligned to the right.
		/// </summary>
		Right
	}

	/// <summary>
	/// Provides data for the <see cref="GroupBar.ProvideGroupBarItemBrush"/> event.
	/// </summary>
	/// <remarks>The <see cref="GroupBar"/> control uses the <see cref="GroupBar.ProvideGroupBarItemBrush"/> event to 
	/// obtain a custom brush from the application to draw the background region of a <see cref="GroupBarItem"/>. 	
	/// <seealso cref="ProvideGroupBarItemBrushEventHandler"/>	
	/// </remarks>	
	public class ProvideGroupBarItemBrushEventArgs: EventArgs
	{
		private int nIndex;
		private Brush brBackground;
		private Rectangle rcBounds;

		/// <summary>
		/// Creates an instance of the ProvideBrushEventArgs class.
		/// </summary>
		/// <param name="bounds">The bounds for which a brush is requested.</param>
		public ProvideGroupBarItemBrushEventArgs( int item, Rectangle bounds )
		{
			this.nIndex = item;
			this.rcBounds = bounds;
		}

		/// <summary>
		/// Returns the index of the <see cref="GroupBarItem"/> being drawn.
		/// </summary>
		/// <value>An Integer value.</value>
		public int Item
		{
			get { return this.nIndex; }
		}

		/// <summary>
		/// Returns the bounds for which a brush is requested.
		/// </summary>
		/// <value>The Rectangle specifying the bounds.</value>
		public Rectangle Bounds
		{
			get { return this.rcBounds; }
		}

		/// <summary>
		/// Gets / sets the brush that will be used to draw the specified bounds.
		/// </summary>
		/// <value>A brush object.</value>
		/// <remarks>The event handler should set this property for it
		/// to be used while drawing the specified bounds.</remarks>
		public Brush BackgroundBrush
		{
			get { return this.brBackground; }
			set { this.brBackground = value; }
		}
	}

	/// <summary>
	/// Represents the method that will handle the <see cref="GroupBar.ProvideGroupBarItemBrush"/> event 
	/// in the <see cref="Syncfusion.Windows.Forms.Tools.GroupBar"/> control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="args">A <see cref="Syncfusion.Windows.Forms.Tools.ProvideGroupBarItemBrushEventArgs"/> that contains the event data.</param>
	public delegate void ProvideGroupBarItemBrushEventHandler( object sender, ProvideGroupBarItemBrushEventArgs args );


	/// <summary>
	/// Provides data for the <see cref="GroupBar.GroupBarItemAdded"/> and <see cref="GroupBar.GroupBarItemRemoved"/> events.
	/// </summary>
	/// <remarks>The <see cref="GroupBar"/> control uses the <see cref="GroupBar.GroupBarItemAdded"/> and <see cref="GroupBar.GroupBarItemRemoved"/> 
	/// events to notify users of a change in its <see cref="GroupBar.GroupBarItems"/> collection.
	/// <seealso cref="GroupBarItemEventHandler"/>	
	/// </remarks>	
	public class GroupBarItemEventArgs: EventArgs
	{
		private GroupBarItem gbItem;

		/// <summary>
		/// Returns the GroupBarItem used by this event.
		/// </summary>
		/// <value>The <see cref="GroupBarItem"/> object used by the event.</value>
		public GroupBarItem Item
		{
			get { return this.gbItem; }
		}

		/// <summary>
		/// Creates a new instance of the <see cref="GroupBarItemEventArgs"/> class.
		/// </summary>
		/// <param name="item">The <see cref="GroupBarItem"/> to store in this event.</param>
		public GroupBarItemEventArgs( GroupBarItem item )
		{
			this.gbItem = item;
		}
	}

	/// <summary>
	/// Represents the method that will handle the <see cref="GroupBar.GroupBarItemAdded"/> and <see cref="GroupBar.GroupBarItemRemoved"/> 
	/// events in the <see cref="Syncfusion.Windows.Forms.Tools.GroupBar"/> control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="args">A <see cref="GroupBarItemEventArgs"/> that contains the event data.</param>	
	public delegate void GroupBarItemEventHandler( object sender, GroupBarItemEventArgs args );


	/// <summary>
	/// Provides data for the <see cref="GroupBar.NavigationPaneDropDownClick"/> event.
	/// </summary>
	/// <remarks>The <see cref="GroupBar"/> control uses the <see cref="GroupBar.NavigationPaneDropDownClick"/> event to 
	/// allow users to cancel or change the context menu displayed when the drop-down button is clicked.
	/// <seealso cref="NavigationPaneDropDownClickEventHandler"/>	
	/// </remarks>	
	public class NavigationPaneDropDownClickEventArgs: EventArgs
	{
		private IContextMenuProvider menuProvider;

		/// <summary>
		/// Returns the menu provider object used by <see cref="GroupBar"/> for creating its context menu.
		/// </summary>
		/// <value>The <see cref="Syncfusion.Windows.Forms.IContextMenuProvider"/> object.</value>
		public IContextMenuProvider ContextMenuProvider
		{
			get { return this.menuProvider; }
		}

		/// <summary>
		/// Creates a new instance of the <see cref="NavigationPaneDropDownClickEventArgs"/> class.
		/// </summary>
		/// <param name="provider">The <see cref="Syncfusion.Windows.Forms.IContextMenuProvider"/> implementing the context menu.</param>
		public NavigationPaneDropDownClickEventArgs( IContextMenuProvider provider )
		{
			this.menuProvider = provider;
		}
	}

	/// <summary>
	/// Represents the method that will handle the <see cref="GroupBar.NavigationPaneDropDownClick"/> event in the 
	/// <see cref="Syncfusion.Windows.Forms.Tools.GroupBar"/> control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="args">A <see cref="NavigationPaneDropDownClickEventArgs"/> that contains the event data.</param>	
	public delegate void NavigationPaneDropDownClickEventHandler( object sender, NavigationPaneDropDownClickEventArgs args );

	/// <summary>
	/// Provides data for the <see cref="GroupBar.GroupBarItemSelectionChanging"/> event.
	/// </summary>
	/// <remarks>The <see cref="GroupBar"/> control uses the <see cref="GroupBar.GroupBarItemSelectionChanging"/> event to 
	/// allow users to cancel bar item selection.
	/// <seealso cref="GroupBarItemSelectionChangingEventHandler"/>	
	/// </remarks>	
	public class GroupBarItemSelectionChangingEventArgs:
		CancelEventArgs
	{
		private int m_nNewSelected = -1;
		private int m_nOldSelected = -1;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GroupBarItemSelectionChangingEventArgs"/> class.
		/// </summary>
		/// <param name="nNewSelected">The newly selected item index.</param>
		/// <param name="nOldSelected">The old selected item index.</param>
		public GroupBarItemSelectionChangingEventArgs( int nNewSelected, int nOldSelected )
		{
			this.m_nNewSelected = nNewSelected;
			this.m_nOldSelected = nOldSelected;
		}

		/// <summary>
		/// Returns the newly selected index.
		/// </summary>
		public int NewSelected
		{
			get
			{
				return m_nNewSelected;
			}
		}

		/// <summary>
		/// Returns the previously selected index.
		/// </summary>
		public int OldSelected
		{
			get
			{
				return m_nOldSelected;
			}
		}
	}

	/// <summary>
	/// Represents the method that will handle the <see cref="GroupBar.GroupBarItemSelectionChanging"/> event in the 
	/// <see cref="Syncfusion.Windows.Forms.Tools.GroupBar"/> control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="args">A <see cref="GroupBarItemSelectionChangingEventArgs"/> that contains the event data.</param>	
	public delegate void GroupBarItemSelectionChangingEventHandler( object sender, GroupBarItemSelectionChangingEventArgs args );


	/// <summary>
	/// Displays a set of related controls as selectable groups or tabs.
	/// </summary>
	/// <remarks>
	/// <p>
	/// The GroupBar class implements a container that can serve as a host for other controls.
	/// The control is functionally similar to the Windows Forms <see cref="System.Windows.Forms.TabControl"/>
	/// and provides a tab user-interface that will display only one control, the <see cref="GroupBar.SelectedItem"/>, 
	/// at any given time.
	/// </p>
	/// <p>
	/// Each control in the GroupBar is associated with a <see cref="GroupBarItem"/> and the
	/// various GroupBarItems are arranged in a vertical stack. Clicking on a GroupBarItem will make 
	/// it the current selected item and the client control tied to that item will be displayed 
	/// within the GroupBar's client region. The collection of GroupBarItems in the control is 
	/// implemented as an instance of the <see cref="GroupBar.GroupBarItemCollection"/> type and 
	/// can be accessed and manipulated through the <see cref="GroupBar.GroupBarItems"/> property.
	/// </p>
	/// <p>
	/// The GroupBar can be used in combination with the Syncfusion <see cref="GroupView"/> control
	/// to implement composite controls with user interfaces similar to the Outlook Bar in
	/// Microsoft Outlook and the toolbox window present in the Visual Studio.NET development environment.
	/// </p>
	/// </remarks>	
	/// <seealso cref="GroupView"/>
	/// <example>
	/// The sample code shows how to create a GroupBar, create and add two GroupBarBarItems, and 
	/// assign client controls to each of the GroupBarItems. 
	/// <coderef file="Tools\Samples\GroupBar Package\GroupBarDemo\CS\GroupBarForm.cs" name="GroupBar" lang="C#"><code lang="C#">
	///		private void InitializeGroupBar()
	///		{
	///			// Create the GroupBar control.
	///			this.gbOutlook = new Syncfusion.Windows.Forms.Tools.GroupBar();
	///
	///			// Create and initialize the GroupBarItems that belong to this GroupBar.
	///			this.gbiPersonal = new Syncfusion.Windows.Forms.Tools.GroupBarItem();			
	///			// Assign the gvcPersonal client control to this GroupBarItem.
	///			this.gbiPersonal.Client = this.gvcPersonal;
	///			this.gbiPersonal.Text = "Personal";
	///
	///			this.gbiWork = new Syncfusion.Windows.Forms.Tools.GroupBarItem();
	///			// Assign the gvcWork client control to this GroupBarItem.
	///			this.gbiWork.Client = this.gvcWork;
	///			this.gbiWork.Text = "Work";		
	///			
	///			// Add the GroupBarItems to the GroupBar.
	///			this.gbOutlook.GroupBarItems.Add(this.gbiPersonal);
	///			this.gbOutlook.GroupBarItems.Add(this.gbiWork);
	///
	///			// Set the GroupBar's initially selected index.
	///			this.gbOutlook.SelectedItem = 1;
	///		}</code></coderef>
	///		
	///		
	/// <coderef file="Tools\Samples\GroupBar Package\GroupBarDemo\VB\GroupBarForm.vb" name="GroupBar" lang="VB"><code lang="VB">
	///        Private Sub InitializeGroupBar()
	///
	///            ' Create the GroupBar control.
	///            Me.gbOutlook = New Syncfusion.Windows.Forms.Tools.GroupBar()
	///
	///            ' Create and initialize the GroupBarItems that belong to this GroupBar.
	///            Me.gbiPersonal = New Syncfusion.Windows.Forms.Tools.GroupBarItem()
	///            ' Assign the gvcPersonal client control to this GroupBarItem.
	///            Me.gbiPersonal.Client = Me.gvcPersonal
	///            Me.gbiPersonal.Text = "Personal"
	///
	///            Me.gbiWork = New Syncfusion.Windows.Forms.Tools.GroupBarItem()
	///            ' Assign the gvcWork client control to this GroupBarItem.
	///            Me.gbiWork.Client = Me.gvcWork
	///            Me.gbiWork.Text = "Work"
	///
	///            ' Add the GroupBarItems to the GroupBar.
	///            Me.gbOutlook.GroupBarItems.Add(Me.gbiPersonal)
	///            Me.gbOutlook.GroupBarItems.Add(Me.gbiWork)
	///
	///            ' Set the GroupBar's initially selected index.
	///            Me.gbOutlook.SelectedItem = 1
	///
	///        End Sub</code></coderef>
	/// 
	/// </example>	
	[
	ToolboxBitmap( typeof( Syncfusion.Windows.Forms.PopupControlContainer ), "ToolboxIcons.groupbar.bmp" ),
	Designer( typeof( GroupBarDesigner ), typeof( IDesigner ) ),
	DefaultProperty( "GroupBarItems" ),
	DefaultEvent( "GroupBarItemSelected" ),
	Description( "Displays a set of related controls as selectable groups or tabs." )
	]
	public class GroupBar:
		Control,
		IIntegratedScrollContainer,
		IGroupBarDesignerInvoke,
		ISupportInitialize,
        IVisualStyle 
	{
		static GroupBar()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
				new Syncfusion.Core.Licensing.LicensedComponent( typeof( GroupBar ) );
			}
			finally
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler( AssemblyInfo.AssemblyResolver );
			}

			InitResources();
		}

		private static void InitResources()
		{
			Type thisType = typeof( Syncfusion.Windows.Forms.Tools.GroupBar );
			Assembly asm = thisType.Assembly;
			ResourceManager resmgr = new ResourceManager( s_resPath + ".GroupBar", asm );

			s_ilNavPaneItems = new ImageList();

			s_ilNavPaneItems.Images.Add( resmgr.GetObject( "UpArrow" ) as Icon );
			s_ilNavPaneItems.Images.Add( resmgr.GetObject( "DownArrow" ) as Icon );

			s_defaultCollapseImage = new Bitmap( asm.GetManifestResourceStream( s_resPath + ".CollapseButton.png" ) );
			s_defaultExpandImage = new Bitmap( asm.GetManifestResourceStream( s_resPath + ".ExpandButton.png" ) );


		}

        protected override Size DefaultSize
        {
            get
            {
                return new Size(220, 300);
            }
        }

		#region GroupBarItemCollection

		/// <summary>
		/// The collection of <see cref="GroupBarItem"/> objects in the <see cref="GroupBar"/> control.
		/// </summary>	
		/// <remarks>
		/// Each group in the GroupBar is an instance of the GroupBarItem type and 
		/// the collection of these groups is represented by an instance of the 
		/// GroupBarItemCollection class. GroupBarItems may be added or removed using the 
		/// IList and ICollection interface methods implemented by the GroupBarItemCollection. 
		/// <see cref="GroupBar.GroupBarItems"/>					
		/// </remarks>
		[
		Description( "The GroupBarItem collection in the GroupBar." ),
		DefaultProperty( "Item" )
		]
		public class GroupBarItemCollection: CollectionBase
		{
			[Syncfusion.Documentation.DocumentationExclude()]
			protected GroupBar groupBarHost;
			[Syncfusion.Documentation.DocumentationExclude()]
			protected bool bLockUpdate = false;

			/// <summary>
			/// Gets / sets a <see cref="GroupBarItem"/> in the collection.
			/// </summary>
			/// <param name="index">The zero-based index of the GroupBarItem to get / set.</param>
			public GroupBarItem this[int index]
			{
				get { return (GroupBarItem)(this.List[index]); }

				set { Insert( index, value ); }
			}

			/// <summary>
			/// Creates an instance of the GroupBarItemCollection class.
			/// </summary>
			/// <param name="groupbar">The <see cref="GroupBar"/> control that contains this collection.</param>
			public GroupBarItemCollection( GroupBar groupbar )
			{
				this.groupBarHost = groupbar;
			}

			/// <summary>
			/// Adds the <see cref="GroupBarItem"/> to the collection.
			/// </summary>
			/// <param name="item">The <see cref="GroupBarItem"/> to be added.</param>
			/// <returns>The index of the new item within the collection.</returns>
			public int Add( GroupBarItem item )
			{
				return this.List.Add( item );
			}

			/// <summary>
			/// Inserts the <see cref="GroupBarItem"/> into the collection at the specified index.
			/// </summary>
			/// <param name="index">The zero-based index at which the item is to be inserted.</param>
			/// <param name="item">The <see cref="GroupBarItem"/> to be inserted.</param>
			public void Insert( int index, GroupBarItem item )
			{
				this.List.Insert( index, item );

				// Update the active group index
				//	if(index <= groupBarHost.nSelectedItem)
				//		groupBarHost.nSelectedItem++;				
			}

			/// <summary>
			/// Removes the <see cref="GroupBarItem"/> specified by the index parameter.
			/// </summary>
			/// <param name="index">A zero-based index specifying the GroupBarItem to be removed.</param>
			new public void RemoveAt( int index )
			{
				if( (index < 0) || (index >= this.List.Count) )
					throw new ArgumentOutOfRangeException( "The specified index is invalid." );
				this.Remove( this[index] );
			}

			/// <summary>
			/// Removes the <see cref="GroupBarItem"/> from the collection.
			/// </summary>
			/// <param name="item">The <see cref="GroupBarItem"/> to be removed.</param>
			public void Remove( GroupBarItem item )
			{
				if( this.List.Contains( item ) == false )
					throw new ApplicationException( "GroupBarItem does not exist." );

				this.groupBarHost.NotifyGroupBarOfItemRemoval( item );
				item.PropertyChanged -= new PropertyChangedEventHandler( groupBarHost.PropChangedHandler );

				this.List.Remove( item );

				groupBarHost.RecalculateGroupBarLayout();
				groupBarHost.SetActiveClientBounds();
				groupBarHost.Invalidate( false );

				this.groupBarHost.FireGroupBarItemRemovedEvent( item );
			}

			/// <summary>
			/// Adds an array of GroupBarItems to the <see cref="GroupBar"/> control's <see cref="GroupBar.GroupBarItems"/> collection.
			/// </summary>
			/// <param name="items">An array of <see cref="GroupBarItem"/> objects.</param>
			public void AddRange( GroupBarItem[] items )
			{
				this.bLockUpdate = true;
				foreach( GroupBarItem item in items )
					this.Add( item );
				this.bLockUpdate = false;
				this.groupBarHost.RecalculateGroupBarLayout();
				this.groupBarHost.SetActiveClientBounds();
				this.groupBarHost.Invalidate( false );
			}

			/// <summary>
			/// Indicates whether the specified <see cref="GroupBarItem"/> is present in the collection.
			/// </summary>
			/// <param name="item">The <see cref="GroupBarItem"/> to locate in the collection.</param>
			/// <returns>True if the item is present; False otherwise.</returns>
			public bool Contains( GroupBarItem item )
			{
				return this.List.Contains( item );
			}

			/// <summary>
			/// Returns the zero-based index of the <see cref="GroupBarItem"/> in the collection.
			/// </summary>
			/// <param name="item">The <see cref="GroupBarItem"/> to locate in the collection.</param>
			/// <returns>The zero-based index of the item; -1 if the item is not present.</returns>
			public int IndexOf( GroupBarItem item )
			{
				return this.List.IndexOf( item );
			}

			[
			EditorBrowsable( EditorBrowsableState.Never ),
			Syncfusion.Documentation.DocumentationExclude()
			]
			public void CopyTo( GroupBarItem[] array, int index )
			{
				this.List.CopyTo( array, index );
			}

			[Syncfusion.Documentation.DocumentationExclude()]
			protected override void OnInsert( int index, object value )
			{
				GroupBarItem item = value as GroupBarItem;
				if( (this.groupBarHost.DesignMode == true) && (item.Text == String.Empty) )
					item.Text = String.Concat( "GroupBarItem", this.Count.ToString() );

				base.OnInsert( index, value );
			}

			[Syncfusion.Documentation.DocumentationExclude()]
			protected override void OnInsertComplete( int index, object value )
			{
				base.OnInsertComplete( index, value );

				GroupBarItem item = value as GroupBarItem;
				if( item.GroupBar != this.groupBarHost )
				{
					item.SetGroupBarControlReference( this.groupBarHost );
					if( item.InDesignMode == true )
						TypeDescriptor.Refresh( item );
				}
				if( item.Client != null )
				{
					if( item.Client.IsHandleCreated == false )
						item.Client.CreateControl();

					// Temporarily setting GroupBarHost.nSelectedItem to a non-valid index prevents the GroupBar.OnControlAdded 
					// method from assigning this control the selected GroupBarItem. 
					int nsel = this.groupBarHost.nSelectedItem;
					this.groupBarHost.nSelectedItem = -1;
					if( (item.Client.Parent == null) || (item.Client.Parent.Equals( groupBarHost )==false) )
						groupBarHost.Controls.Add( item.Client );
					this.groupBarHost.nSelectedItem = nsel;
				}
				item.PropertyChanged += new PropertyChangedEventHandler( groupBarHost.PropChangedHandler );

				this.groupBarHost.nHighlightItem = -1;
				this.groupBarHost.nContextMenuItem = -1;
				if( this.groupBarHost.DesignMode == true )
					this.groupBarHost.nSelectedItem = this.Count-1;
				else if( groupBarHost.nSelectedItem == -1 )
					this.groupBarHost.nSelectedItem = 0;

				if( this.bLockUpdate == false )
				{
					groupBarHost.RecalculateGroupBarLayout();
					groupBarHost.SetActiveClientBounds();
					groupBarHost.Invalidate( false );
				}

				this.groupBarHost.FireGroupBarItemAddedEvent( item );
			}

			protected override void OnClearComplete()
			{
				base.OnClearComplete();

				groupBarHost.nSelectedItem = -1;
				groupBarHost.RecalculateGroupBarLayout();
				groupBarHost.SetActiveClientBounds();
				groupBarHost.Invalidate( false );
			}
		}

		/// <summary>
		/// Collection of visible <see cref="GroupBarItem"/>
		/// </summary>
		public class VisibleGroupBarItemsCollection: ICollection
		{
			#region Members
			/// <summary>
			/// Visible group bar items array list.
			/// </summary>
			private ArrayList m_barItems;
			#endregion Members

			#region Indexer
			public GroupBarItem this[int index]
			{
				get
				{
					if( index < 0 || index >= m_barItems.Count )
					{
						throw new ArgumentOutOfRangeException( "index" );
					}
					return (GroupBarItem)m_barItems[index];
				}
			}
			#endregion

			#region ICollection implementation
			internal VisibleGroupBarItemsCollection( ArrayList barItems )
			{
				if( null == barItems )
					throw new ArgumentNullException( "barItems" );

				m_barItems = barItems;
			}

			/// <summary>
			/// Copies the elements of the <see cref="T:System.Collections.ICollection"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
			/// </summary>
			public void CopyTo( Array array, int index )
			{
				m_barItems.CopyTo( array, index );
			}

			/// <summary>
			/// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"></see>.
			/// </summary>
			public int Count
			{
				get
				{
					return m_barItems.Count;
				}
			}

			/// <summary>
			/// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.
			/// </summary>
			public object SyncRoot
			{
				get
				{
					return m_barItems.SyncRoot;
				}
			}

			/// <summary>
			/// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe).
			/// </summary>
			public bool IsSynchronized
			{
				get
				{
					return m_barItems.IsSynchronized;
				}
			}

			/// <summary>
			/// Returns the enumerator that iterates through the item collection.
			/// </summary>
			public IEnumerator GetEnumerator()
			{
				return m_barItems.GetEnumerator();
			}
			#endregion ICollection implementation
		}

		#endregion	// GroupBarItemCollection

		#region Fields

		[Syncfusion.Documentation.DocumentationExclude()]
		private readonly static string s_resPath = "Syncfusion.Windows.Forms.GroupBar";
		[Syncfusion.Documentation.DocumentationExclude()]
		protected const int nTxtOffset = 4;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected static int nStartTick = 0;
		/// <summary>
		/// Indicates whether to show the chevron button on the Navigation Panel 
		/// in the stacked GroupBar.
		/// </summary>
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bShowChevron = true;

        [Syncfusion.Documentation.DocumentationExclude()]
        private bool showNavigationPane = true;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool m_bShowImageInHeader = false;
		// Property fields.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected TextAlignment nAlignment = TextAlignment.Center;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected GroupLayout nLayout = GroupLayout.Vertical;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bBarHighlight = true;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bAnimation = true;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bIntegratedScrolling = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected BorderStyle bdrStyle = BorderStyle.Fixed3D;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bFlatLook = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected VisualStyle vStyle = VisualStyle.OfficeXP;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Office2007Theme m_office2007Theme = Office2007Theme.Blue;
        [Syncfusion.Documentation.DocumentationExclude()]
        protected Office2010Theme m_office2010Theme = Office2010Theme.Blue;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Cursor gDefaultCursor = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Cursor itemCursor = Cursors.Arrow;

		// Stacked View attributes.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bStackedMode = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal ArrayList alNavPaneItems = new ArrayList();
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nSplitterHeight = 6;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nHeaderHeight = 26;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nNavigationPaneHeight = 32;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nNavigationButtonWidth = 22;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nDropdownButtonWidth = 20;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Point ptSplitterDragStart = new Point( -1, -1 );
		[Syncfusion.Documentation.DocumentationExclude()]
		static protected String s_sDropDownToolTip;
		[Syncfusion.Documentation.DocumentationExclude()]
		static protected ImageList s_ilNavPaneItems;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Font hdrFont;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Color hdrForeColor = Color.White;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Color prev_hdrForeColor = Color.White;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Color hdrBackColor = SystemColors.ControlDark;
        [Syncfusion.Documentation.DocumentationExclude()]
        protected Color prev_hdrBackColor = Color.FromArgb(22,165,220);
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nItemHeight = 22;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nScrollThumbWidth = 18;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected const int c_nIconSize = 16;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected Rectangle rcClient = Rectangle.Empty;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nSelectedItem = -1;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nHighlightItem = -1;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nContextMenuItem = -1;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected ButtonsState eButtonsState = ButtonsState.None;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bHighlight = false;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected Rectangle rcScrollUp = Rectangle.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Rectangle rcScrollDown = Rectangle.Empty;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected Timer tmrScrolling = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ButtonsState eInScroll = ButtonsState.None;

		// Used for the dynamic renaming of items.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected RenameTextBox ctrlRename = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected int nRenameItem = -1;

		// GroupBarItems collection.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected GroupBarItemCollection cllnGroupBarItems = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal ArrayList activeItemsList = new ArrayList();

		// XP Themes Support.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bThemesEnabled = false;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ThemedControlDrawing tdButton = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		protected ThemedControlDrawing tdScrollBar = null;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected ToolTip compToolTip = null;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool bDrawClientBorder = false;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected IContextMenuProvider menuProvider;
		/// <summary>
		/// Collection of visible group bar items.
		/// </summary>
		private VisibleGroupBarItemsCollection m_visibleGroupBarItems = null;
		private bool m_bDrawingHeader = false;

		/// <summary>
		/// Colors for Office2007 visual style.
		/// </summary>
		private Office2007Colors m_office2007ColorTable = null;

        /// <summary>
        /// Colors for Office2010 visual style.
        /// </summary>
        private Office2010Colors m_office2010ColorTable = null;
		[Syncfusion.Documentation.DocumentationExclude()]
		private IconRenderingMode m_IconRenderingMode;


        /// <summary>
        /// Default size of the control
        /// </summary>
        private static Size CTRLSIZE = default(Size);
        /// <summary>
        /// Default size of the collapsebutton
        /// </summary>
        private Size CbuttonSIZE = default(Size);
        /// <summary>
        /// Chevron button size
        /// </summary>
        private Size CHEVRONSize = default(Size);
        /// <summary>
        ///Header font
        /// </summary>
        private Font HeaderFONT = default(Font);
        /// <summary>
        ///barItem height
        /// </summary>
        private int GBIheight = default(int);
        /// <summary>
        ///Header height
        /// </summary>
        private int GHeaderHeight = default(int);
        /// <summary>
        ///Collapsedwidth
        /// </summary>
        private int GBCWidth = default(int);
        /// <summary>
        ///Collapsed font
        /// </summary>
        private Font GBCFont = default(Font);
		private Color m_borderColor = Color.Empty;
		#endregion

		#region Events

		/// <summary>
		/// Occurs when a <see cref="GroupBarItem"/> in the <see cref="GroupBar"/> control 
		/// is selected.
		/// </summary>
		/// <remarks>
		/// Use the <see cref="GroupBar.SelectedItem"/> property to get the index of the newly 
		/// selected item.
		/// </remarks>
		[
		Description( "Event fired when an item in the GroupBar control is selected." ),
		Category( "Behavior" )
		]
		public event System.EventHandler GroupBarItemSelected;

		/// <summary>
		/// Occurs when a <see cref="GroupBarItem"/> in the <see cref="GroupBar"/> control is being selected.
		/// </summary>
		[
		Description( "Event fired when an item in the GroupBar control is being selected." ),
		Category( "Behavior" )
		]
		public event GroupBarItemSelectionChangingEventHandler GroupBarItemSelectionChanging;

		/// <summary>
		/// Occurs after a <see cref="GroupBarItem"/> has been renamed by an in-place edit operation. 
		/// </summary>
		/// See <see cref="GroupItemRenamedEventArgs"/> and <see cref="GroupItemRenamedEventHandler"/>.
		/// <seealso cref="GroupBar.InplaceRenameItem"/>.
		[
		Description( "Event fired after an in-place rename operation." ),
		Category( "Behavior" )
		]
		public event GroupItemRenamedEventHandler GroupBarItemRenamed;


		/// <summary>
		/// Occurs after a <see cref="GroupBarItem"/> has been added to the <see cref="GroupBar.GroupBarItems"/> collection. 
		/// </summary>
		/// See <see cref="GroupBarItemEventArgs"/> and <see cref="GroupBarItemEventHandler"/>.
		[
		Description( "Event fired after a GroupBarItem has been added to the GroupBar control." ),
		Category( "Behavior" )
		]
		[Browsable( false )]
		public event GroupBarItemEventHandler GroupBarItemAdded;


		/// <summary>
		/// Occurs after a <see cref="GroupBarItem"/> has been removed from the <see cref="GroupBar.GroupBarItems"/> collection. 
		/// </summary>
		/// See <see cref="GroupBarItemEventArgs"/> and <see cref="GroupBarItemEventHandler"/>.
		[
		Description( "Event fired after a GroupBarItem has been removed from the GroupBar control." ),
		Category( "Behavior" )
		]
		[Browsable( false )]
		public event GroupBarItemEventHandler GroupBarItemRemoved;


		/// <summary>
		/// Occurs when the right mouse button is clicked over the <see cref="GroupBar"/> control.
		/// </summary>
		/// <remarks>The <see cref="GroupBar.ContextMenuItem"/> property will provide the index 
		/// of the <see cref="GroupBarItem"/> over which the mouse was clicked.</remarks>
		[
		Description( "Event fired when the right mouse button is clicked over the control." ),
		Category( "Behavior" )
		]
		public event EventHandler ShowContextMenu;

		/// <summary>
		/// Occurs when a <see cref="GroupBarItem"/> is about to be drawn. 
		/// </summary>
		/// <remarks>
		/// Handle this event to provide a custom brush for painting the GroupBarItem background.
		/// </remarks>
		/// See <see cref="ProvideGroupBarItemBrushEventArgs"/> and <see cref="ProvideGroupBarItemBrushEventHandler"/>.	
		[
		Description( "Event occurs when a GroupBarItem is about to be drawn." ),
		Category( "Appearance" )
		]
		public event ProvideGroupBarItemBrushEventHandler ProvideGroupBarItemBrush;


		/// <summary>
		/// Occurs when the user clicks on the <see cref="GroupBar"/> control's navigation pane drop-down button. 
		/// </summary>
		/// <remarks>
		/// This GroupBar control displays the navigation pane only when the <see cref="GroupBar.StackedMode"/> property is set.
		/// </remarks>
		/// See <see cref="NavigationPaneDropDownClickEventArgs"/> and <see cref="NavigationPaneDropDownClickEventHandler"/>.
		[
		Description( "Event fired when the GroupBar's navigation pane drop-down button is clicked." ),
		Category( "Behavior" )
		]
		public event NavigationPaneDropDownClickEventHandler NavigationPaneDropDownClick;

		#endregion

		#region Consts
		const int c_nOffset = 1;
		//const int c_nPressedOffset = 3;
		//const int c_nPressedGroupBarItemBorders = 5;
		const int c_nGroupBarItemBorders = 3;
		const int c_nPressedOffset = 1;
		const int c_nPressedGroupBarItemBorders = 3;
		#endregion Consts


		#region Properties

		/// <summary>        
		/// Specifies the type of rendering done to icons
		/// </summary>
		[
		Description( "Indicates the type of rendering done to icons." ),
		Category( "Behavior" ),
		DefaultValue( IconRenderingMode.AlphaBlended )
		]
		public IconRenderingMode IconRenderingMode
		{
			get
			{
				return m_IconRenderingMode;
			}
			set
			{
				m_IconRenderingMode = value;
			}
		}

		/// <summary>
		/// Returns the collection of visible group bar items.
		/// </summary>
		[Browsable( false )]
		public VisibleGroupBarItemsCollection VisibleGroupBarItems
		{
			get
			{
				if( m_visibleGroupBarItems == null )
				{
					m_visibleGroupBarItems = new VisibleGroupBarItemsCollection( activeItemsList );
				}
				return m_visibleGroupBarItems;
			}
		}

		/// <summary>
		/// Returns the collection of <see cref="GroupBarItem"/>s in the control.
		/// </summary>
		/// <value>An instance of the <see cref="GroupBar.GroupBarItemCollection"/> type.</value>
		[
		DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
		Description( "The GroupBarItems present in the control." ),
		Category( "GroupBarItems" )
		]
		public GroupBar.GroupBarItemCollection GroupBarItems
		{
			get
			{
				return this.cllnGroupBarItems;
			}
		}

		/// <summary>
		/// Gets / sets the alignment of the text displayed on the <see cref="GroupBarItem"/>.
		/// </summary>
		/// <value>A <see cref="TextAlignment"/> value. The default value is TextAlignment.Center.</value>
		[
		Description( "Specifies the horizontal alignment of the GroupBarItem text." ),
		Category( "Appearance" ),
		DefaultValue( TextAlignment.Center ),
		Localizable( true )
		]
		public TextAlignment TextAlign
		{
			get { return nAlignment; }
			set
			{
				if( this.nAlignment != value )
				{
					this.nAlignment = value;
					this.Invalidate();
				}
			}
		}
        private bool applyDefaultVisualStyleColor = true;
        /// <summary>
        /// Indicates whether applying the default forecolor for GroupBar as per the visual style
        /// </summary>
        /// <value>True if highlighting is enabled. The default is True.</value>
        [
        Description("Indicates whether applying the default forecolor as per the visual style."),
        Category("Behavior"),
        DefaultValue(true)
        ]
        public bool ApplyDefaultVisualStyleColor
        {
            get { return applyDefaultVisualStyleColor; }
            set
            {
                if (applyDefaultVisualStyleColor != value)
                    this.applyDefaultVisualStyleColor = value;
            }
        }
		/// <summary>
		/// Indicates whether moving the mouse cursor over a <see cref="GroupBarItem"/> will highlight it.
		/// </summary>
		/// <value>True if highlighting is enabled. The default is True.</value>
		[
		Description( "Indicates whether moving the cursor over a GroupBarItem will highlight it." ),
		Category( "Behavior" ),
		DefaultValue( true )
		]
		public bool BarHighlight
		{
			get { return bBarHighlight; }
			set { this.bBarHighlight = value; }
		}

		/// <summary>
		/// Indicates whether switching between different <see cref="GroupBarItem"/>s is animated.
		/// </summary>
		/// <value>True if animated selection is enabled. The default is True.</value>
		[
		Description( "Indicates whether animated selection is enabled." ),
		Category( "Behavior" ),
		DefaultValue( true )
		]
		public bool AnimatedSelection
		{
			get { return bAnimation; }
			set
			{
				if( this.bAnimation != value )
				{
					if( (value == true) && (this.bStackedMode == true) )
						return;
					this.bAnimation = value;
				}
			}
		}

		/// <summary>
		/// Gets / sets an integer that represents the index of the current selected <see cref="GroupBarItem"/>.
		/// </summary>
		/// <value>An integer value that specifies the zero-based index of the GroupBarItem.</value>
		[
		Description( "The 0-based index of the current selected GroupBarItem." ),
		Category( "Behavior" ),
		DefaultValue( -1 )
		]
		public int SelectedItem
		{
			get
			{
				if (IndexOnVisibleItems)
					return this.nSelectedItem;
				else
					return GetItemIndex(this.nSelectedItem);
			}
			set
			{
				if( GroupExists( value ) )
				{
					this.SelectItem( value, this.nSelectedItem != value ? this.nSelectedItem : -1 );
				}
			}
		}

		/// <summary>
		/// Returns the index of the <see cref="GroupBarItem"/> that is currently under the mouse cursor.
		/// </summary>
		/// <value>An integer value that specifies the zero-based index of the GroupBarItem.</value>
		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public int HighlightItem
		{
			get { return this.nHighlightItem; }
		}

		/// <summary>
		/// Returns the index of the <see cref="GroupBarItem"/> that triggered the <see cref="GroupBar.ShowContextMenu"/> event.
		/// </summary>
		/// <value>The zero-based index of the item.</value>
		[
		Description( "Returns the index of the GroupBarItem that triggered the ShowContextMenu event." ),
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		public int ContextMenuItem
		{
			get { return this.nContextMenuItem; }
		}


		/// <summary>
		/// Gets / sets the <see cref="GroupBarItem"/> height. 
		/// </summary>
		/// <value>An integer value that specifies the item height.</value>
		[
		Description( "Determines the height for the GroupBarItems." ),
		Category( "Behavior" ),
		DefaultValue( 22 ),
		Localizable( true )
		]
		public int GroupBarItemHeight
		{
			get { return this.nItemHeight; }
			set
			{
				if( this.nItemHeight != value )
				{
					this.nItemHeight = value;
					// Force a resize
					this.Size = new Size( this.Width-1, this.Height-1 );
					this.Size = new Size( this.Width+1, this.Height+1 );
				}
			}
		}

		/// <summary>
		/// Indicates whether integrated scroll buttons are to be used.
		/// </summary>
		/// <remarks>This option is primarily intended for use with the Syncfusion <see cref="GroupView"/> control. 
		/// When this flag is set, the <see cref="GroupBar"/> provides the scroll buttons for the current 
		/// selected GroupView control and interacts with it to achieve seamless scrolling behavior.
		/// </remarks>
		/// <value>True to enable integrated scrolling. The default is False.</value>
		/// <seealso cref="GroupView.IntegratedScrolling"/>
		[
		Description( "Indicates whether the GroupBar should provide the scroll buttons for the client controls." ),
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
					if( value == true )
					{
						if( this.bStackedMode == true )
							return;
						this.nAlignment = TextAlignment.Left;
					}
					this.bIntegratedScrolling = value;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets / sets the border style of the <see cref="GroupBar"/> control.
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
						throw new InvalidEnumArgumentException( "value", ((int)(value)), typeof( BorderStyle ) );
					this.bdrStyle = value;

					this.RecalculateGroupBarLayout();
					this.SetActiveClientBounds();
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
                    this.ForeColor = Color.Black;
                    this.hdrForeColor = Color.Black;
                    this.BackColor = SystemColors.Control;
					this.bThemesEnabled = value;
                    this.OnStyleChanged();
					this.Invalidate( false );
				}
			}
		}

		[
		Browsable( false ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
		]
		private bool UseThemedDrawing
		{
			get
			{
				return (this.ThemesEnabled && XPThemes.IsAppThemed);
			}
		}

		/// <summary>
		/// Indicates whether the <see cref="GroupBar"/> control is displayed with a flat look.
		/// </summary>		
		/// <value>True to display in flat mode. The default is False.</value>
		[
		Description( "Gets / sets a value indicating whether the control is displayed with a flat look." ),
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
		/// Indicates the style to be used for drawing the <see cref="GroupBar"/> control.
		/// </summary>		
		/// <value>A <see cref="Syncfusion.Windows.Forms.VisualStyle"/> value; the default is VisualStyle.OfficeXP.</value>
		[
		Description( "Gets or sets a value indicating the style used for drawing the control." ),
		Category( "Appearance" ),
		DefaultValue( VisualStyle.OfficeXP ),
		TypeConverter( typeof( DefaultVisualStyleEnumFilter ) )
		]
		public VisualStyle VisualStyle
		{
			get { return this.vStyle; }

			set
			{
				if( this.vStyle != value )
				{
                    if (this.vStyle == VisualStyle.Metro)
                    {
                        this.ForeColor = Color.Black;
                        this.HeaderForeColor = this.prev_hdrForeColor;
                    }
                    else if (this.vStyle == VisualStyle.Office2007 || this.vStyle == VisualStyle.Office2010)
                    {
                        this.ForeColor = Color.Black;
                        this.Font = FontUtil.CreateFont(this.Font, FontStyle.Regular);
                    }
					this.vStyle = value;
					this.OnStyleChanged();
				}
			}
		}

		private void OnStyleChanged()
		{
            if (this.vStyle == VisualStyle.Office2010)
            {
                if (Office2010Theme != Office2010Theme.Black)
                {
                    m_collapseImage = new Bitmap(typeof(GroupBar).Assembly.GetManifestResourceStream(s_resPath + ".CollapseButton2010.png"));
                    m_expandImage = new Bitmap(typeof(GroupBar).Assembly.GetManifestResourceStream(s_resPath + ".ExpandButton2010.png"));
                }
                else
                {
                    m_collapseImage = new Bitmap(typeof(GroupBar).Assembly.GetManifestResourceStream(s_resPath + ".CollapseBlack2010.png"));
                    m_expandImage = new Bitmap(typeof(GroupBar).Assembly.GetManifestResourceStream(s_resPath + ".ExpandBlack2010.png"));
                }               
            }
            else if(this.vStyle == VisualStyle.Metro)
            {
                m_collapseImage = new Bitmap(typeof(GroupBar).Assembly.GetManifestResourceStream(s_resPath + ".CollapseButtonMetro.png"));
                m_expandImage = new Bitmap(typeof(GroupBar).Assembly.GetManifestResourceStream(s_resPath + ".ExpandButtonMetro.png"));
            }
            else
            {
                m_collapseImage = new Bitmap(typeof(GroupBar).Assembly.GetManifestResourceStream(s_resPath + ".CollapseButton.png"));
                m_expandImage = new Bitmap(typeof(GroupBar).Assembly.GetManifestResourceStream(s_resPath + ".ExpandButton.png"));
            }
			if( this.vStyle == VisualStyle.Office2007 )
			{
				m_office2007ColorTable = Office2007Colors.GetColorTable( m_office2007Theme );

				if( this.DesignMode )
					this.BorderStyle = BorderStyle.FixedSingle;

				this.Font = FontUtil.CreateFont( this.Font, FontStyle.Bold );
				this.ForeColor = m_office2007ColorTable.GroupBarItemTextColor;
                if (!this.ApplyDefaultVisualStyleColor)
                {
                    foreach (Control ctrl in this.Controls)
                    {
                        ctrl.ForeColor = m_office2007ColorTable.GroupBarItemTextColor;
                    }
                }
				this.hdrForeColor = m_office2007ColorTable.GroupBarHeaderTextColor;
                this.BorderColor = m_office2007ColorTable.GroupBarBorderColor;
			}
            else if (this.vStyle == VisualStyle.Office2010)
            {
                m_office2010ColorTable = Office2010Colors.GetColorTable(m_office2010Theme);

                if (this.DesignMode)
                    this.BorderStyle = BorderStyle.FixedSingle;

                this.Font = FontUtil.CreateFont(this.Font, FontStyle.Bold);
                this.ForeColor = m_office2010ColorTable.GroupBarItemTextColor;
                if (!this.ApplyDefaultVisualStyleColor)
                {
                    foreach (Control ctrl in this.Controls)
                    {
                        ctrl.ForeColor = m_office2010ColorTable.GroupBarItemTextColor;
                    }
                }
                this.hdrForeColor = m_office2010ColorTable.GroupBarHeaderTextColor;                
            }
            else if (this.vStyle == VisualStyle.Metro && !ThemesEnabled)
            {
                if (!this.ApplyDefaultVisualStyleColor)
                {
                    foreach (Control ctrl in this.Controls)
                    {
                        ctrl.ForeColor = SystemColors.ControlText;
                    }
                }
                this.FlatLook = true;
                this.BorderStyle = BorderStyle.None;
                this.HeaderBackColor = ColorTranslator.FromHtml("#16A5DC");
                this.BorderColor = Color.White;
                this.BackColor = ColorTranslator.FromHtml("#EBEBEB");
                this.HeaderForeColor = Color.White;
                this.ForeColor = Color.White;
                this.HeaderForeColor = Color.White;
            }
            else if (vStyle == VisualStyle.Default)
            {
                if (!this.ApplyDefaultVisualStyleColor)
                {
                    foreach (Control ctrl in this.Controls)
                    {
                        ctrl.ForeColor = SystemColors.ControlText;
                    }
                }
                this.HeaderBackColor = SystemColors.Control;
                this.HeaderForeColor = Color.Black;
                this.ForeColor = Color.Black;
                this.BackColor = Color.FromArgb(235,235,235);
                this.FlatLook = false;
                this.BorderStyle = BorderStyle.Fixed3D;
                this.BorderColor = SystemColors.ControlDark;
            }
            else
            {
                if (!this.ApplyDefaultVisualStyleColor)
                {
                    foreach (Control ctrl in this.Controls)
                    {
                        ctrl.ForeColor = SystemColors.ControlText;
                    }
                }
                this.HeaderBackColor = SystemColors.Control;
                this.HeaderForeColor = SystemColors.ControlText;
				this.FlatLook = false;
				this.BorderStyle = BorderStyle.Fixed3D;
				this.BorderColor = SystemColors.ControlDark;
			}
			this.Invalidate();
		}

        /// <summary>
        /// Get or Set of Skin Manager Interface
        /// </summary>
        private string style;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return style;
            }
            set
            {
                style = value;

                if (value == "Office2007Blue")
                {
                    VisualStyle = VisualStyle.Office2007;
                    Office2007Theme = Office2007Theme.Blue;
                }
                else if (value == "Office2007Silver")
                {
                    VisualStyle = VisualStyle.Office2007;
                    Office2007Theme = Office2007Theme.Silver;
                }
                else if (value == "Office2007Black")
                {
                    VisualStyle = VisualStyle.Office2007;
                    Office2007Theme = Office2007Theme.Black;
                }
                else if (value == "Office2010Blue")
                {
                    VisualStyle = VisualStyle.Office2010;
                    Office2010Theme = Office2010Theme.Blue;
                }
                else if (value == "Office2010Silver")
                {
                    VisualStyle = VisualStyle.Office2010;
                    Office2010Theme = Office2010Theme.Silver;
                }
                else if (value == "Office2010Black")
                {
                    VisualStyle = VisualStyle.Office2010;
                    Office2010Theme = Office2010Theme.Black;
                }
                else if (value == "Managed")
                {
                    VisualStyle = VisualStyle.Office2007;
                    Office2007Theme = Office2007Theme.Managed;
                }
                else if (value == "Office2003")
                    VisualStyle = VisualStyle.Office2003;
                else if (value == "Metro")
                    VisualStyle = VisualStyle.Metro;
                else
                    VisualStyle = VisualStyle.Default;
            }
        }
		/// <summary>
		/// Indicates the Office2007 theme used for drawing the control.
		/// </summary>
		[
		Description( "Gets / sets a value indicating the Office2007 theme used for drawing the control." ),
		Category( "Appearance" ),
		DefaultValue( Office2007Theme.Blue )
		]
		public Office2007Theme Office2007Theme
		{
			get { return m_office2007Theme; }
			set
			{
				if( m_office2007Theme != value )
				{
					m_office2007Theme = value;
					OnOffice2007ThemeChanged();
				}
			}
		}

		private void OnOffice2007ThemeChanged()
		{
			if( this.VisualStyle == VisualStyle.Office2007 )
			{
				m_office2007ColorTable = Office2007Colors.GetColorTable( m_office2007Theme );
			}

			this.Invalidate();
		}
        /// <summary>
        /// Indicates the Office2010 theme used for drawing the control.
        /// </summary>
        [
        Description("Gets / sets a value indicating the Office2010 theme used for drawing the control."),
        Category("Appearance"),
        DefaultValue(Office2010Theme.Blue)
        ]
        public Office2010Theme Office2010Theme
        {
            get { return m_office2010Theme; }
            set
            {
                if (m_office2010Theme != value)
                {
                    m_office2010Theme = value;
                    OnOffice2010ThemeChanged();
                }
            }
        }

        private void OnOffice2010ThemeChanged()
        {
            if (this.VisualStyle == VisualStyle.Office2010)
            {
                m_office2010ColorTable = Office2010Colors.GetColorTable(m_office2010Theme);
                if (Office2010Theme != Office2010Theme.Black)
                {
                    m_collapseImage = new Bitmap(typeof(GroupBar).Assembly.GetManifestResourceStream(s_resPath + ".CollapseButton.png"));
                    m_expandImage = new Bitmap(typeof(GroupBar).Assembly.GetManifestResourceStream(s_resPath + ".ExpandButton.png"));
                }
                else
                {
                    m_collapseImage = new Bitmap(typeof(GroupBar).Assembly.GetManifestResourceStream(s_resPath + ".CollapseBlack2010.png"));
                    m_expandImage = new Bitmap(typeof(GroupBar).Assembly.GetManifestResourceStream(s_resPath + ".ExpandBlack2010.png"));
                }
            }

            this.Invalidate();
        }
		/// <summary>
		/// Gets / sets the cursor that is displayed when the mouse pointer is over the control.
		/// </summary>
		/// <value>A <see cref="System.Windows.Forms.Cursor"/> object.</value>
		[
		Description( "Gets / sets the cursor that is displayed when the mouse pointer is over the control." ),
		Category( "Appearance" )
		]
		public new Cursor Cursor
		{
			get { return base.Cursor; }

			set
			{
				if( base.Cursor != value )
				{
					base.Cursor = value;
					this.gDefaultCursor = value;
				}
			}
		}

		/// <summary>
		/// Gets / sets the cursor that is displayed when the mouse pointer is over the <see cref="GroupBarItem"/>s.
		/// </summary>		
		/// <value>A <see cref="System.Windows.Forms.Cursor"/> object.</value>
		[
		Description( "Gets / sets the cursor that is displayed when the mouse pointer is over the GroupBarItems." ),
		Category( "Appearance" )
		]
		public Cursor GroupBarItemCursor
		{
			get { return this.itemCursor; }

			set
			{
				if( this.itemCursor != value )
					this.itemCursor = value;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool ShouldSerializeGroupBarItemCursor()
		{
			return (this.itemCursor != Cursors.Arrow);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public void ResetGroupBarItemCursor()
		{
			this.itemCursor = Cursors.Arrow;
		}

		/// <summary>
		/// Indicates whether the <see cref="GroupBarItem"/>s are displayed in a stack.
		/// </summary>
		/// <remarks>
		/// <p>When the <see cref="GroupBar.StackedMode"/> property is set, the GroupBarItems are stacked at 
		/// the bottom of the <see cref="GroupBar"/> control on top of a navigation pane. The stack size can be increased 
		/// or decreased by moving items to and from the navigation pane.</p>
		/// NOTE: The StackedMode interface is similar to the Navigation Pane in Microsoft Outlook 2003.
		/// </remarks>		
		/// <value>True to set the stacked mode. The default is False.</value>
		[
		Description( "Gets / sets a value indicating whether GroupBarItems are stacked." ),
		Category( "Behavior" ),
		DefaultValue( false )
		]
		public bool StackedMode
		{
			get
			{
				return this.bStackedMode;
			}
			set
			{
				if( this.bStackedMode != value )
				{
					if( !value )
					{
						this.Collapsed = false;
					}

					this.bStackedMode = value;

					if( this.DesignMode == true )
						TypeDescriptor.Refresh( this );

					if( value == true )
					{
						if( this.bIntegratedScrolling == true )
							this.bIntegratedScrolling = false;
						if( this.bAnimation == true )
							this.bAnimation = false;
					}

					this.RecalculateGroupBarLayout();
					this.SetActiveClientBounds();
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets / sets the height of the <see cref="GroupBar"/> header. 
		/// </summary>
		/// <remarks>
		/// The GroupBar header is shown only when the <see cref="GroupBar.StackedMode"/> property is set.
		/// </remarks>
		/// <value>An integer value that specifies the header height.</value>
		[
		Description( "Determines the height of the GroupBar header." ),
		Category( "Stacked Mode" ),
		DefaultValue( 26 ),
		Localizable( true )
		]
		public int HeaderHeight
		{
			get { return this.nHeaderHeight; }
			set
			{
				if( this.nHeaderHeight != value )
				{
					this.nHeaderHeight = value;

					this.RecalculateGroupBarLayout();
					this.SetActiveClientBounds();
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets / sets the font of the text displayed in the <see cref="GroupBar"/> header. 
		/// </summary>
		/// <remarks>
		/// The GroupBar header is shown only when the <see cref="GroupBar.StackedMode"/> property is set.
		/// </remarks>
		/// <value>A <see cref="System.Drawing.Font"/> value.</value>
		[
		Description( "Determines the font of the text displayed in the GroupBar header." ),
		Category( "Stacked Mode" ),
		Localizable( true )
		]
		public Font HeaderFont
		{
			get { return this.hdrFont; }
			set
			{
				if( this.hdrFont != value )
				{
					this.hdrFont = value;
					Rectangle rcbounds = this.GetBoundedRectangle();
					this.Invalidate( new Rectangle( rcbounds.Left, rcbounds.Top, rcbounds.Width, this.nHeaderHeight ) );
				}
			}
		}

		protected bool ShouldSerializeHeaderFont()
		{
			if( (this.hdrFont.FontFamily.Name == "Arial") && (this.hdrFont.Size == 12) && (this.hdrFont.Style == FontStyle.Bold) )
				return false;
			return true;
		}

		/// <summary>
		/// Resets the <see cref="GroupBar.HeaderFont"/> property to its default value.
		/// </summary>
		public void ResetHeaderFont()
		{
			this.hdrFont = new Font( "Arial", 12, FontStyle.Bold );
		}

		/// <summary>
		/// Gets / sets the forecolor for the <see cref="GroupBar"/> header. 
		/// </summary>
		/// <remarks>
		/// The GroupBar header is shown only when the <see cref="GroupBar.StackedMode"/> property is set.
		/// </remarks>
		/// <value>A <see cref="System.Drawing.Color"/> value.</value>
		[
		Description( "Determines the forecolor for the GroupBar header." ),
		Category( "Stacked Mode" ),
		Localizable( true )
		]
		public Color HeaderForeColor
		{
			get { return this.hdrForeColor; }
			set
			{
				if( this.hdrForeColor != value )
				{
					this.hdrForeColor = value;
					this.prev_hdrForeColor = value;

					Rectangle rcbounds = this.GetBoundedRectangle();
					this.Invalidate( new Rectangle( rcbounds.Left, rcbounds.Top, rcbounds.Width, this.nHeaderHeight ) );
				}
			}
		}

		protected bool ShouldSerializeHeaderForeColor()
		{
			if( this.hdrForeColor == Color.White )
				return false;
			return true;
		}

		/// <summary>
		/// Resets the <see cref="GroupBar.HeaderForeColor"/> property to its default value.
		/// </summary>
		public void ResetHeaderForeColor()
		{
			this.hdrForeColor = Color.White;
		}

		/// <summary>
		/// Gets / sets the backcolor for the <see cref="GroupBar"/> header. 
		/// </summary>
		/// <remarks>
		/// The GroupBar header is shown only when the <see cref="GroupBar.StackedMode"/> property is set.
		/// </remarks>
		/// <value>A <see cref="System.Drawing.Color"/> value.</value>
		[
		Description( "Determines the background color for the GroupBar header." ),
		Category( "Stacked Mode" ),
		Localizable( true )
		]
		public Color HeaderBackColor
		{
			get { return this.hdrBackColor; }
			set
			{
				if( this.hdrBackColor != value )
				{
					this.hdrBackColor = value;
                    this.prev_hdrBackColor = value;
					Rectangle rcbounds = this.GetBoundedRectangle();
					this.Invalidate( new Rectangle( rcbounds.Left, rcbounds.Top, rcbounds.Width, this.nHeaderHeight ) );
				}
			}
		}

		protected bool ShouldSerializeHeaderBackColor()
		{
            if (this.hdrBackColor == ColorTranslator.FromHtml("#16A5DC"))
				return false;
			return true;
		}

		/// <summary>
		/// Resets the <see cref="GroupBar.HeaderBackColor"/> property to its default value.
		/// </summary>
		public void ResetHeaderBackColor()
		{
            this.hdrBackColor = ColorTranslator.FromHtml("#16A5DC");
		}

		/// <summary>
		/// Gets / sets the height of the <see cref="GroupBar"/> navigation pane. 
		/// </summary>
		/// <remarks>
		/// The navigation pane is shown only when the <see cref="GroupBar.StackedMode"/> property is set.
		/// </remarks>
		/// <value>An integer value that specifies the height.</value>
		[
		Description( "Determines the height of the GroupBar navigation pane." ),
		Category( "Stacked Mode" ),
		DefaultValue( 32 ),
		Localizable( true )
		]
		public int NavigationPaneHeight
		{
			get { return this.nNavigationPaneHeight; }
			set
			{
				if( this.nNavigationPaneHeight != value )
				{
					this.nNavigationPaneHeight = value;

					this.RecalculateGroupBarLayout();
					this.SetActiveClientBounds();
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Gets / sets the width of the <see cref="GroupBarItem"/>s shown in the navigation pane. 
		/// </summary>
		/// <remarks>
		/// The navigation pane is shown only when the <see cref="GroupBar.StackedMode"/> property is set.
		/// </remarks>
		/// <value>An integer value that specifies the GroupBarItem width.</value>
		[
		Description( "Determines the width of the items in the GroupBar navigation pane." ),
		Category( "Stacked Mode" ),
		DefaultValue( 22 ),
		Localizable( true )
		]
		public int NavigationPaneButtonWidth
		{
			get { return this.nNavigationButtonWidth; }
			set
			{
				if( this.nNavigationButtonWidth != value )
				{
					this.nNavigationButtonWidth = value;
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// Indicates whether a border is drawn around the <see cref="GroupBar"/>'s client window. 
		/// </summary>
		/// <remarks>
		/// The border colors for each <see cref="GroupBarItem"/> can be individually specified using the 
		/// <see cref="GroupBarItem.ClientBorderColors"/> property.
		/// </remarks>
		/// <value>A boolean value; the default is False.</value>
		[
		Description( "Determines whether a border is drawn around the GroupBar's client window." ),
		Category( "Appearance" ),
		DefaultValue( false ),
		Localizable( true )
		]
		public bool DrawClientBorder
		{
			get { return this.bDrawClientBorder; }
			set
			{
				if( this.bDrawClientBorder != value )
				{
					this.bDrawClientBorder = value;

					this.RecalculateGroupBarLayout();
					this.SetActiveClientBounds();
					this.Invalidate();
				}
			}
		}

		[
		Browsable( false ),
		EditorBrowsable( EditorBrowsableState.Never ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public override Image BackgroundImage
		{
			get { return null; }
			set { }
		}
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
		[
		Browsable( false ),
		EditorBrowsable( EditorBrowsableState.Never ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public override ImageLayout BackgroundImageLayout
		{
			get { return ImageLayout.None; }
			set { }
		}
#endif
		/// <summary>
		/// Gets / sets the menu provider object that will implement the <see cref="GroupBar"/>'s contextmenu. 
		/// </summary>
		/// <remarks>
		/// The GroupBar control automatically initializes this property depending on the presence of the Syncfusion Essential Tools library.
		/// If Essential Tools is available, then the menu provider object will be an instance of the <see cref="Syncfusion.Windows.Forms.Tools.GroupBar.ContextMenuProvider"/> 
		/// type. If not, the <see cref="Syncfusion.Windows.Forms.StandardMenusProvider"/> class is used for implementing the standard .NET context menu.
		/// <p>The GroupBar's automatic initialization should suffice for most applications and you should explicitly set this property 
		/// only when you want to override the default menu provider assignment.</p>
		/// </remarks>
		/// <value>A <see cref="Syncfusion.Windows.Forms.IContextMenuProvider"/> implementation; the default is <see cref="Syncfusion.Windows.Forms.StandardMenusProvider"/>.</value>
		[
		Description( "Specifies the context menu provider for the GroupBar control." ),
		Category( "Behavior" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Browsable( false )
		]
		public IContextMenuProvider ContextMenuProvider
		{
			get { return this.menuProvider; }

			set
			{
				if( this.menuProvider != value )
				{
					this.menuProvider = value;
				}
			}
		}

		[
		Browsable( false ),
		EditorBrowsable( EditorBrowsableState.Never ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
		Syncfusion.Documentation.DocumentationExclude()
		]
		public int ScrollTimerInterval
		{
			get { return this.tmrScrolling.Interval; }
			set
			{
				if( this.tmrScrolling.Interval != value )
					this.tmrScrolling.Interval = value;
			}
		}

		/// <summary>
		/// Indicates whether the Chevron button on the  
		/// Navigation Panel is shown in the Stacked GroupBar.
		/// </summary>
		/// <remarks>
		/// The navigation pane is shown only when the <see cref="GroupBar.StackedMode"/> property is set.
		/// </remarks>
		[
		Browsable( true ),
		Description( "Gets / sets value indicating if Chevron button on the Navigation Panel is shown in the Stacked GroupBar or not." ),
		Category( "Stacked Mode" ),
		DefaultValue( true ),
		Localizable( true ),
		]
		public bool ShowChevron
		{
			get
			{
				return bShowChevron;
			}
			set
			{
				if( bShowChevron != value )
				{
					bShowChevron = value;
					Invalidate();
				}
			}
		}
        [
        Browsable(true),
        Description("Shows and Hides the GroupBar navigation pane."),
        Category("Stacked Mode"),
        DefaultValue(true),
        ]
        public bool ShowNavigationPane
        {
            get
            {
                return this.showNavigationPane;
            }
            set
            {
                if (this.showNavigationPane != value)
                {
                    this.showNavigationPane = value;
                    Invalidate();
                }
            }
        }
        bool bPopupRTL = false;
        [
        Browsable(false),
        Description("The GroupBar navigation pane popup RTL support."),
        Category("Stacked Mode"),
        DefaultValue(false),
        ]
        public bool PopupRightToLeft
        {
            get
            {
                return this.bPopupRTL;
            }
            set
            {
                if (this.bPopupRTL != value)
                {
                    this.bPopupRTL = value;
                    Invalidate();
                }
            }
        }
		/// <summary>
		/// Indicates whether the selected item's image is shown in header in the Stacked GroupBar.
		/// </summary>
		[
		Description( "Gets / sets value indicating if selected item's image is shown in header in the Stacked GroupBar or not." ),
		Category( "Stacked Mode" ),
		DefaultValue( false ),
		Localizable( true ),
		]
		public bool ShowItemImageInHeader
		{
			get
			{
				return m_bShowImageInHeader;
			}
			set
			{
				if( m_bShowImageInHeader != value )
				{
					m_bShowImageInHeader = value;
					Invalidate();
				}
			}
		}
		/// <summary>
		/// Indicates the color of the 2D border.
		/// </summary>
		[
		Category( "Appearance" ),
		Description("Gets/sets the color of the 2D border.")
		]
		public Color BorderColor
		{
			get
			{
				Color color;

				if (m_borderColor == Color.Empty)
				{
					if (this.vStyle == VisualStyle.Office2003)
					{
						color = Office2003Colors.CommandBarDropDownColorDark;
					}
					else if (this.vStyle == VisualStyle.Office2007)
					{
						color = m_office2007ColorTable.GroupBarBorderColor;
					}
                    else if (this.vStyle == VisualStyle.Office2010)
                    {
                        color = m_office2010ColorTable.GroupBarBorderColor;
                    }
					else
					{
						color = SystemColors.ControlDark;
					}
				}
				else color = m_borderColor;

				return color;
			}
			set
			{
				if (m_borderColor != value)
				{
					m_borderColor = value;
					Invalidate();
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private bool ShouldSerializeBorderColor()
		{
			return m_borderColor != Color.Empty;
		}
		/// <summary>
		/// 
		/// </summary>
		private void ResetBorderColor()
		{
			this.BorderColor = Color.Empty;
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
            c_nCollapseButtonHeight= c_nCollapseButtonWidth = (int)(CbuttonSIZE.Width * scaleFactor);
            this.GroupBarItemHeight = (int)(GBIheight * scaleFactor);
            this.HeaderHeight = (int)(GHeaderHeight * scaleFactor);
            this.CollapsedWidth = (int)(GBCWidth *scaleFactor);
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
        /// <summary></summary>
        /// <param name="e"/>
        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
            if (!EnableTouchMode && this.DesignMode)
            {
                CTRLSIZE = this.Size;
            }
        }
        #endregion
		/// <summary>
		/// Creates a new instance of the <see cref="GroupBar"/> class.
		/// </summary>
		public GroupBar()
		{
            s_sDropDownToolTip = SR.GetString(SR.GroupBarDropDownToolTip, this);
            s_sExpandButtonToolTip = SR.GetString(SR.GroupBarExpandButtonToolTip, this);
            s_sMinimizeButtonToolTip = SR.GetString(SR.GroupBarMinimizeButtonToolTip, this);
            s_sNavigationPaneTooltip = SR.GetString(SR.GroupBarNavigationPaneTooltip, this);
            s_sAddRemoveButton = SR.GetString(SR.GroupBarAddRemoveButton, this);
			this.SetStyle( ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | WhidbeyCompatibleControlStyles.DoubleBuffer, true );
			this.cllnGroupBarItems = new GroupBarItemCollection( this );

			this.AllowDrop = true;
			this.ContextMenu = null;

			this.tmrScrolling = new Timer();
			this.tmrScrolling.Interval = 200;
			this.tmrScrolling.Tick += new EventHandler( this.ScrollTimerEventHandler );

			if( XPThemes.IsThemedOS )
			{
				this.tdButton = new ThemedControlDrawing( ThemedControls.BUTTON );
				this.tdScrollBar = new ThemedControlDrawing( ThemedControls.SCROLLBAR );
			}

			this.compToolTip = new ToolTip();
			this.compToolTip.SetToolTip( this, String.Empty );

			this.menuProvider = Syncfusion.Windows.Forms.MenuProviderFactory.CreateContextMenuProvider();

			this.hdrFont = new Font( "Arial", 12, FontStyle.Bold );

			this.gDefaultCursor = Cursors.Default;

			m_sfmtTextFormat.LineAlignment = StringAlignment.Center;
			m_sfmtTextFormat.Trimming = StringTrimming.EllipsisCharacter;
			m_sfmtTextFormat.FormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoWrap;
            CbuttonSIZE = new Size(18, 18);
            CTRLSIZE = this.Size;
            GBIheight = this.GroupBarItemHeight;
            GHeaderHeight = this.HeaderHeight;
            HeaderFONT = this.HeaderFont;
            GBCWidth = this.CollapsedWidth;
		}

		/// <summary>
		/// Starts an in-place edit of the specified <see cref="GroupBarItem"/> text. 
		/// </summary>
		/// <remarks>Invoking this method will create an editable text box and and populates it with 
		/// the item text. Editing the text box contents and selecting ENTER will update the GroupBarItem text. 
		/// Selecting ESC will cancel the edit.</remarks>
		/// <param name="nindex">The zero-based index of the item to be renamed.</param>
		/// <seealso cref="GroupBar.CancelInplaceRenameItem"/>
		public virtual void InplaceRenameItem( int nindex )
		{
			if( GroupExists( nindex ) == false )
				return;

			GroupBarItem group = this.activeItemsList[nindex] as GroupBarItem;
			if( group.Enabled == false )
				return;

			// Create the edit control and an instance of the message filter during the first invocation of InplaceRenameItem.
			if( this.ctrlRename == null )
			{
				this.ctrlRename = new RenameTextBox();
				this.Controls.Add( this.ctrlRename );
				this.ctrlRename.RenameComplete += new RenameCompleteEventHandler( this.GroupBar_RenameComplete );
			}
			this.nRenameItem = nindex;
			this.ctrlRename.BeginRename( GetGroupBarItemBounds( nindex ), group.Text );
		}

		/// <summary>
		/// Cancels an in-place edit that is in progress.
		/// </summary>		
		/// <seealso cref="GroupBar.InplaceRenameItem"/>		
		public virtual void CancelInplaceRenameItem()
		{
			if( (this.ctrlRename != null) && (this.ctrlRename.Visible == true) )
				this.ctrlRename.EndRename( true );
		}

		int IGroupBarDesignerInvoke.GetGroupAtLocation( Point pt )
		{
			if( this.ClientRectangle.Contains( pt ) == false )
				return -1;

			return this.GetItemFromPoint( pt );
		}

		private const int DEF_MINITEMS_COUNT = 2;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void NotifyGroupBarOfItemRemoval( GroupBarItem item )
		{
			this.nHighlightItem = -1;
			this.nContextMenuItem = -1;

			int nindex = this.activeItemsList.IndexOf( item );
			if( nindex == this.nSelectedItem )
			{
				// If nindex is the current active group, then activate the next / previous group.
				int newselected = this.nSelectedItem;
				if( nindex-1 >= 0 )
					newselected = nindex-1;
				else if( nindex+1 < this.activeItemsList.Count && 
					this.activeItemsList.Count > DEF_MINITEMS_COUNT )
					newselected = nindex+1;
				if( newselected == this.nSelectedItem )
				{
					// This is the last group. Just hide the client object.					
					if( item.Client != null )
						item.Client.Visible = false;
				}
				else
					this.SelectItem( newselected, this.nSelectedItem );
			}
			else if( nindex < this.nSelectedItem )
			{
				this.nSelectedItem--;
			}

			if( this.alNavPaneItems.Contains( item ) == true )
				this.alNavPaneItems.Remove( item );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool GroupExists( int ngroup )
		{
			return (ngroup >= 0)&&(ngroup < this.activeItemsList.Count);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void PropChangedHandler( Object obj, PropertyChangedEventArgs args )
		{
			GroupBarItem item = obj as GroupBarItem;
			if( item == null )
				return;

			if( args.PropertyName == "InNavigationPane" )
			{
				this.nHighlightItem = -1;
				this.nContextMenuItem = -1;

				this.RecalculateGroupBarLayout();
				this.SetActiveClientBounds();
				this.Invalidate( true );
			}
			else if( (this.DesignMode == false) && (args.PropertyName == "Visible") )
			{
				this.nHighlightItem = -1;
				this.nContextMenuItem = -1;

				if( item.Visible == false )
				{
					int nindex = this.activeItemsList.IndexOf( item );
					if( nindex == this.nSelectedItem )
					{
						if( nindex-1 >= 0 )
						{
							nindex--;
							this.SelectItem( nindex, this.nSelectedItem );
						}
						else // (nindex == this.nSelectedItem) The nSelectedItem value will remain the same. Just hide the client window.
						{
							if( item.Client != null )
								item.Client.Visible = false;
						}
					}
					else if( nindex < this.nSelectedItem )
					{
						this.nSelectedItem--;
					}

					if( this.alNavPaneItems.Contains( item ) == true )
						this.alNavPaneItems.Remove( item );
				}
				this.RecalculateGroupBarLayout();
				this.SetActiveClientBounds();
				this.Invalidate( true );
			}
			else
			{
				this.Invalidate( this.GetGroupBarItemBounds( this.activeItemsList.IndexOf( item ) ), false );
			}
		}

		// Event handler for the RenameComplete event fired by the RenameTextBox.		
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void GroupBar_RenameComplete( Object sender, RenameCompleteEventArgs arg )
		{
			String name = arg.NewName;
			GroupBarItem group = this.activeItemsList[nRenameItem] as GroupBarItem;
			String strold = group.Text;
			if( (name != null) && (strold.Equals( name ) == false) )
				group.Text = name;
			this.Invalidate();
			OnGroupBarItemRenamed( new GroupItemRenamedEventArgs( nRenameItem, strold, name ) );

			nRenameItem = -1;
		}

		/// <summary>
		/// Raises the <see cref="GroupBar.GroupBarItemSelected"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="System.EventArgs"/> value that contains the event data.</param>
		protected virtual void OnGroupBarItemSelected( EventArgs arg )
		{
			if( this.GroupBarItemSelected != null )
			{
				this.GroupBarItemSelected( this, arg );
			}
		}

		/// <summary>
		/// Raises the <see cref="GroupBar.GroupBarItemSelectionChanging"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="GroupBarItemSelectionChangingEventArgs"/> value that contains the event data.</param>
		protected virtual void OnGroupBarItemSelectionChanging( GroupBarItemSelectionChangingEventArgs arg )
		{
			if( this.GroupBarItemSelectionChanging != null )
			{
				this.GroupBarItemSelectionChanging( this, arg );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void FireGroupBarItemAddedEvent( GroupBarItem item )
		{
			this.OnGroupBarItemAdded( new GroupBarItemEventArgs( item ) );
		}

		/// <summary>
		/// Raises the <see cref="GroupBar.GroupBarItemAdded"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="GroupBarItemEventArgs"/> value that contains the event data.</param>
		protected virtual void OnGroupBarItemAdded( GroupBarItemEventArgs arg )
		{
			if( this.GroupBarItemAdded != null )
			{
				this.GroupBarItemAdded( this, arg );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void FireGroupBarItemRemovedEvent( GroupBarItem item )
		{
			this.OnGroupBarItemRemoved( new GroupBarItemEventArgs( item ) );
		}

		/// <summary>
		/// Raises the <see cref="GroupBar.GroupBarItemRemoved"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="GroupBarItemEventArgs"/> value that contains the event data.</param>
		protected virtual void OnGroupBarItemRemoved( GroupBarItemEventArgs arg )
		{
			if( this.GroupBarItemRemoved != null )
			{
				this.GroupBarItemRemoved( this, arg );
			}
		}

		/// <summary>
		/// Raises the <see cref="GroupBar.GroupBarItemRenamed"/> event.
		/// </summary>
		/// <param name="arg">An <see cref="GroupItemRenamedEventArgs"/> value that contains the event data.</param>
		protected virtual void OnGroupBarItemRenamed( GroupItemRenamedEventArgs arg )
		{
			if( this.GroupBarItemRenamed != null )
			{
				this.GroupBarItemRenamed( this, arg );
			}
		}

		/// <summary>
		/// Raises the <see cref="GroupBar.ShowContextMenu"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="System.EventArgs"/> value that contains the event data.</param>
		protected virtual void OnShowContextMenu( EventArgs arg )
		{
			if( this.ShowContextMenu != null )
			{
				this.ShowContextMenu( this, arg );
			}
		}

		/// <summary>
		/// Raises the <see cref="GroupBar.ProvideGroupBarItemBrush"/> event.
		/// </summary>
		/// <param name="arg">A <see cref="ProvideGroupBarItemBrushEventArgs"/> value that contains the event data.</param>
		protected virtual void OnProvideGroupBarItemBrush( ProvideGroupBarItemBrushEventArgs arg )
		{
			if( this.ProvideGroupBarItemBrush != null )
			{
				this.ProvideGroupBarItemBrush( this, arg );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual int SelectItem( int newselected, int oldselected )
		{
			if( newselected == oldselected )
				return oldselected;

			GroupBarItemSelectionChangingEventArgs args = null ;
			if (this.IndexOnVisibleItems)
				args = new GroupBarItemSelectionChangingEventArgs(newselected, oldselected);
			else
				args = new GroupBarItemSelectionChangingEventArgs(
					this.GetItemIndex(newselected),
					this.GetItemIndex(oldselected)
					);

			this.OnGroupBarItemSelectionChanging( args );

			if( args.Cancel )
			{
				return oldselected;
			}

			if( !this.Collapsed )
			{
				SelectItemExpanded( newselected, oldselected );
			}

			GroupBarItem prevSelected = this.SelItem;

			this.nSelectedItem = newselected;
			if( GroupExists( oldselected ) && (this.bAnimation == true) )
			{
				ActivateSelection( newselected, oldselected );
			}
			else
			{
				if( GroupExists( oldselected ) )
				{
					GroupBarItem group = this.activeItemsList[oldselected] as GroupBarItem;
					if( group.Client != null )
						group.Client.Visible = false;
				}
				RecalculateGroupBarLayout();
				SetActiveClientBounds();
				this.Refresh();
			}

			this.OnGroupBarItemSelected( EventArgs.Empty );

			if( this.Collapsed && this.ItemPopupOpened )
			{
				if( this.PopupAutoClose )
				{
					HidePopup( prevSelected );
				}
				else
				{
					SelectItemCollapsed( prevSelected );
				}
			}

			return oldselected;
		}

		private void SelectItemCollapsed( GroupBarItem prevSelected )
		{
			if( m_bAllowItemPopup )
			{
				ShowItemPopup( prevSelected );

				m_bAllowItemPopup = false;
			}
		}

		private void SelectItemExpanded( int newselected, int oldselected )
		{
			Control ctrl = (this.activeItemsList[newselected] as GroupBarItem).Client;
			if( (ctrl != null) && (ctrl.IsHandleCreated == false) )
				ctrl.CreateControl();
			if( (ctrl != null) && ((ctrl.Parent == null) || (ctrl.Parent.Equals( this )==false)) )
				this.Controls.Add( ctrl );

			// Invalidate the rectangles occupied by the selected and the next group bars.
			this.Invalidate( this.GetGroupBarItemBounds( newselected ), false );

			if( this.GroupExists( oldselected ) )
			{
				this.Invalidate( this.GetGroupBarItemBounds( oldselected ), false );
			}

			if( this.bIntegratedScrolling )
			{
				if( GroupExists( oldselected + 1 ) )
				{
					this.Invalidate( this.GetGroupBarItemBounds( oldselected + 1 ), false );
				}
				if( GroupExists( newselected + 1 ) )
				{
					this.Invalidate( this.GetGroupBarItemBounds( newselected + 1 ), false );
				}
			}

			// If the lastgroup is the one being deactivated, then reduce the this.rcclient 
			// height by the control's barheight that was previously deducted from the client 
			// rect for drawing the bottom scrollbarthumb.
			if( (this.bIntegratedScrolling == true) && (oldselected+1 == this.activeItemsList.Count) )
				this.rcClient.Height += this.nItemHeight;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void ActivateSelection( int newselected, int oldselected )
		{
			int ndisplacestep = 5;
			Debug.Assert( (newselected >= 0) && (newselected < this.activeItemsList.Count) );

			Debug.Assert( (oldselected >= 0) && (oldselected < this.activeItemsList.Count) );
			GroupBarItem newgroup = this.activeItemsList[newselected] as GroupBarItem;
			GroupBarItem oldgroup = this.activeItemsList[oldselected] as GroupBarItem;

			int oldpane = this.rcClient.Height;
			int newpane = 0;

			int nstep = ndisplacestep;
			if( nstep<=0 )
				nstep = 1;
			int amttochange = oldpane/nstep;
			if( amttochange == 0 )
				amttochange = 1;

			bool bscrlldown = false;
			int nold, nnew;
			if( newselected < oldselected )
			{
				bscrlldown = true;
				nold = newselected;
				nnew = oldselected;
			}
			else
			{
				nold = oldselected;
				nnew = newselected;
			}

			int ncnt = this.activeItemsList.Count;
			// Allocate an array of rects equal to the number of bars and init the array with the current group rects.
			Rectangle[] rcbars = new Rectangle[ncnt];
			// Temporarily reset the selected index.
			this.nSelectedItem = oldselected;
			for( int i=0; i<ncnt; i++ )
				rcbars[i] = GetGroupBarItemBounds( i );
			this.nSelectedItem = newselected;

			int ntop = rcbars[nold].Bottom;
			int nbottom = 0;
			if( this.bIntegratedScrolling == false )
			{
				nbottom = (nnew+1 < ncnt) ? rcbars[nnew+1].Top : this.Bounds.Height;
			}
			else
			{
				if( nnew+1 < ncnt ) // Other than the lastbar, the resizing is the same for non-integ scrolling.
					nbottom = rcbars[nnew+1].Top;
				else
				{
					// When the lastbar is active, the scrollbar is drawn at the bottom of the 
					// groupbar's client. 
					if( nnew == newselected )
						nbottom = this.Bounds.Height-this.nItemHeight;
					else
						nbottom = this.Bounds.Height;
				}
			}

			object prevNewGroupValue = null;
			object prevOldGroupValue = null;

			if( oldgroup.Client != null )
			{
				Control client = oldgroup.Client;

				Type clientType = client.GetType();
				PropertyInfo borderStyleProp = client.GetType().GetProperty( "BorderStyle" );

				if( borderStyleProp != null )
				{
					prevOldGroupValue = borderStyleProp.GetValue( client, new object[] { } );
					client.GetType().GetProperty( "BorderStyle" ).SetValue( client, BorderStyle.None, new object[] { } );

					client.Refresh();
				}
			}

			if( newgroup.Client != null )
			{
				Control client = newgroup.Client;

				Type clientType = client.GetType();
				PropertyInfo borderStyleProp = client.GetType().GetProperty( "BorderStyle" );

				if( borderStyleProp != null )
				{
					prevNewGroupValue = borderStyleProp.GetValue( client, new object[] { } );
					client.GetType().GetProperty( "BorderStyle" ).SetValue( client, BorderStyle.None, new object[] { } );

					client.Refresh();
				}
			}

			int nexttick = Environment.TickCount + 30;
			while( (oldpane - amttochange) >= 1 )
			{
				// Check if it is time to animate.
				if( Environment.TickCount < nexttick )
					continue;

				// Adjust the pane sizes.
				oldpane -= amttochange;
				newpane += amttochange;
				// Compenstate for the last partial animation cycle.
				if( oldpane <= amttochange )
					amttochange += oldpane;

				for( int i=nold+1; i<=nnew; i++ )
				{
					if( bscrlldown )
						rcbars[i].Y += amttochange;
					else
						rcbars[i].Y -= amttochange;
					rcbars[i].Height = nItemHeight;
				}
				Rectangle rctoppane = Rectangle.FromLTRB( rcClient.Left, ntop, rcClient.Right, rcbars[nold+1].Top );
				Rectangle rcbottompane = Rectangle.FromLTRB( rcClient.Left, rcbars[nnew].Bottom, rcClient.Right, nbottom );

				SetActiveClientBounds( oldgroup, rcbottompane, rctoppane, bscrlldown );
				SetActiveClientBounds( newgroup, rctoppane, rcbottompane, bscrlldown );
				if( (newgroup.Client != null) && (newgroup.Client.Visible == false) )
					newgroup.Client.Visible = !this.Collapsed;

				// Redraw the client and all bars affected by the transition.
				for( int j=nold+1; j<=nnew; j++ )
				{
					Rectangle rcdraw = rcbars[j];
					if( (this.bIntegratedScrolling == true) && 
						((j == this.nSelectedItem) || (j == this.nSelectedItem+1)) )
					{
						CorrectRectForScrollThumbWidth( ref rcdraw );
					}

					this.Invalidate( rcdraw, false );
				}
				if( (newgroup != null) && (newgroup.Client != null) )
				{
					Rectangle rcnew = newgroup.Client.Bounds;
					this.Invalidate( new Rectangle( rcnew.Left-1, rcnew.Top-1, rcnew.Width+2, rcnew.Height+1 ) );
				}
				nexttick = Environment.TickCount + 30;
			}
			this.RecalculateGroupBarLayout();
			this.SetActiveClientBounds();

			if( oldgroup.Client != null )
			{
				Control client = oldgroup.Client;
				PropertyInfo propInfo = client.GetType().GetProperty( "BorderStyle" );

				if( propInfo != null )
					propInfo.SetValue( client, prevOldGroupValue, new object[] { } );
			}

			if( newgroup.Client != null )
			{
				Control client = newgroup.Client;
				PropertyInfo propInfo = client.GetType().GetProperty( "BorderStyle" );

				if( propInfo != null )
					propInfo.SetValue( client, prevNewGroupValue, new object[] { } );
			}

			if( oldgroup.Client != null )
				oldgroup.Client.Visible = false;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void SetActiveClientBounds( GroupBarItem group, Rectangle rc1, Rectangle rc2, bool bscrlldown )
		{
			Control client = group.Client;

			if( bscrlldown )
			{
				if( client != null )	// Client is available; resize it to fit the new rect.
				{
					client.Bounds = new Rectangle( rc1.Left, rc1.Top, rc1.Width, rc1.Height );
					client.Update();
				}
				else	// If the group does not have a client, just invoke a paint background for the previously occupied rect.
				{
					this.Invalidate( rc1, false );
				}
			}
			else
			{
				if( client != null )
				{
					client.Bounds = new Rectangle( rc2.Left, rc2.Top, rc2.Width, rc2.Height );
					client.Update();
				}
				else
				{
					this.Invalidate( rc2, false );
				}
			}
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnSystemColorsChanged"/>.
		/// </summary>
		protected override void OnSystemColorsChanged( EventArgs e )
		{
			base.OnSystemColorsChanged( e );
			MenuColors.SysColorsChanged( false );
			Office2003Colors.SysColorsChanged( false );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal Rectangle GetBoundedRectangle()
		{
			Rectangle rcclient = this.ClientRectangle;
			if( this.BorderStyle == BorderStyle.Fixed3D )
				rcclient.Inflate( -2, -2 );
			else if( this.BorderStyle == BorderStyle.FixedSingle )
				rcclient.Inflate( -1, -1 );
			return rcclient;
		}


		#region Renderring

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnPaint"/>.
		/// </summary>
		protected override void OnPaint( PaintEventArgs e )
		{
			MenuColors.UpdateMenuColors();
			Office2003Colors.UpdateMenuColors();

			if( this.bStackedMode == false )
				this.DrawRegularGroupBar( e.Graphics );
			else
				this.DrawStackedGroupBar( e.Graphics );

			if( this.bDrawClientBorder == true )
			{
				this.DrawSelectedGroupClientBorder( e.Graphics, new Rectangle( this.rcClient.Left-1, this.rcClient.Top-1,
					this.rcClient.Width+2, this.rcClient.Height+2 ) );
			}

			base.OnPaint( e );
		}

		/// <summary>
		/// Draws the group bar control.
		/// </summary>
		/// <param name="gph">A <see cref="System.Drawing.Graphics"/> object.</param>
		protected virtual void DrawRegularGroupBar( Graphics gph )
		{
			// Draw a border around the control bounds.
			Rectangle rcClient = this.ClientRectangle;

			bool bIsMirrored = GetIsMirrored();
			if( this.bdrStyle == BorderStyle.Fixed3D )
			{
				using (CMirroredDrawer md3DBorder = new CMirroredDrawer(gph, rcClient, bIsMirrored))
				{
					Rectangle rc = rcClient;
					
					if (bIsMirrored)
					{
						rc.X += rc.Width;
					}

					ControlPaint.DrawBorder3D(md3DBorder.VirtualGfx, rc, Border3DStyle.Sunken);
				}
			}
			else if( this.bdrStyle == BorderStyle.FixedSingle )
			{
				Color bordercolor = this.BorderColor;
				ControlPaint.DrawBorder( gph, this.ClientRectangle, bordercolor, ButtonBorderStyle.Solid );
			}

			// If the list is empty, return.
			if( this.activeItemsList.Count <= 0 )
				return;

			Rectangle rcbounds = this.GetBoundedRectangle();
			// Run through the list of group objects and draw each one.
			Rectangle rcdraw = new Rectangle( rcbounds.Left, rcbounds.Top, rcbounds.Width, this.nItemHeight );
			int itemsCount = this.activeItemsList.Count;
			for( int i = 0; i < itemsCount; i++ )
			{
				Rectangle rcbar = rcdraw;
				if ((this.bIntegratedScrolling == true) &&
					((i == this.nSelectedItem) || (i == this.nSelectedItem + 1)))
				{
					CorrectRectForScrollThumbWidth(ref rcbar);
				}

				GroupBarItem item = this.activeItemsList[i] as GroupBarItem;
				this.DrawGroupBarItem( gph, i, rcbar );
				if (item.LargeImageMode)
				{
					this.DrawGroupBarImage( gph, i, rcbar );
				}
				else
				{
					this.DrawGroupBarIcon( gph, i, rcbar );
				}
				this.DrawGroupBarText( gph, i, rcbar );

				// Set the rcdraw bounds for the next GroupBarItem.
				if( i != nSelectedItem || this.rcClient.Height <= 0 )
				{
					rcdraw.Y += rcdraw.Height;
				}
				else
				{
					rcdraw.Y = this.rcClient.Bottom;
				}
				rcdraw.Height = this.nItemHeight;
			}
			 //If IntegratedScrolling is set and the last button is selected, then draw the 
			 //scroll button at the GroupBar bottom.
			if( (this.bIntegratedScrolling == true) && (this.nSelectedItem == this.activeItemsList.Count-1) )
			{
				Rectangle rcscroll = new Rectangle(
					bIsMirrored ? rcbounds.Left : (rcbounds.Right-this.nScrollThumbWidth),
					rcbounds.Bottom-this.nItemHeight, this.nScrollThumbWidth, this.nItemHeight );
				this.DrawScrollThumb( gph, false, rcscroll );
			}
		}

		/// <summary>
		/// Draws the stacked group bar.
		/// </summary>
		/// <param name="gph">The GPH.</param>
		protected virtual void DrawStackedGroupBar( Graphics gph )
		{
			Rectangle rcbounds = this.GetBoundedRectangle();

			if( rcbounds.Width != 0 && rcbounds.Height != 0 )
			{
				DrawStackedGroupBar( gph, rcbounds );
			}
		}

		private void DrawStackedGroupBar( Graphics gph, Rectangle rcbounds )
		{
			Rectangle rcsplitter = new Rectangle( rcbounds.Left, this.rcClient.Bottom, rcbounds.Width, this.nSplitterHeight );
			Brush splitterbrush = GetSplitterBrush(rcsplitter);
			Color bordercolor = this.BorderColor;

			bool bIsMirrored = GetIsMirrored();
			if( this.bdrStyle == BorderStyle.Fixed3D )
			{
				using( CMirroredDrawer md3DBorder = new CMirroredDrawer( gph, rcClient, bIsMirrored ) )
				{
					Graphics virtGph = md3DBorder.VirtualGfx;
					IntPtr hDC = IntPtr.Zero;

					try
					{
						hDC = virtGph.GetHdc();

						NativeMethods.DrawEdgeBorder border = NativeMethods.DrawEdgeBorder.BDR_SUNKEN;
						NativeMethods.DrawEdgeFlags flags = NativeMethods.DrawEdgeFlags.BF_ALL;
						NativeMethods.RECT rect = new NativeMethods.RECT( md3DBorder.VirtualBounds );

						NativeMethods.DrawEdge( hDC, ref rect, border, flags );
					}
					finally
					{
						if( hDC != IntPtr.Zero )
						{
							virtGph.ReleaseHdc( hDC );
						}
					}
				}
			}
			else
			{
				ControlPaint.DrawBorder( gph, this.ClientRectangle, bordercolor, ButtonBorderStyle.Solid );
			}

			// Draw the GroupBar header.
			if( this.activeItemsList.Count != 0 )
			{
				this.DrawHeader( gph );
			}

			// Draw the stack sizing splitter.
			gph.FillRectangle( splitterbrush, rcsplitter );

			if( this.vStyle == VisualStyle.Office2007 )
			{
				SolidBrush whitebrush = new SolidBrush( Color.White );
				SolidBrush darkrush = new SolidBrush( m_office2007ColorTable.GroupBarBorderColor );
				SolidBrush alphawhite = new SolidBrush( Color.FromArgb( 125, Color.White ) );
				using( CMirroredDrawer mdGripper = new CMirroredDrawer( gph, rcsplitter, bIsMirrored ) )
				{
					Graphics gfxVirt = mdGripper.VirtualGfx;
					Rectangle rectGripper = mdGripper.VirtualBounds;

					Point ptgripper = new Point( (rectGripper.Width / 2) - 9, rectGripper.Top + 2 );
					for( int i = 0; i < 5; i++ )
					{
						ptgripper.Offset( 1, 1 );
						gfxVirt.FillRectangle( whitebrush, ptgripper.X, ptgripper.Y, 2, 2 );
						ptgripper.Offset( -1, -1 );
						gfxVirt.FillRectangle( darkrush, ptgripper.X, ptgripper.Y, 2, 2 );
						ptgripper.Offset( 1, 1 );
						gfxVirt.FillRectangle( alphawhite, ptgripper.X, ptgripper.Y, 2, 2 );
						ptgripper.Offset( 3, -1 );
					}

					//Draw splitter up border.
					Pen spPen = new Pen( m_office2007ColorTable.GroupBarBorderColor, 1 );
					Pen p1=new Pen( Color.White, 1 );
                    if( bIsMirrored )
					{
						gfxVirt.DrawLine( spPen, rectGripper.Left, rectGripper.Top, rectGripper.Right, rectGripper.Top );
						gfxVirt.DrawLine( p1, rectGripper.Left, rectGripper.Top + 1, rectGripper.Right, rectGripper.Top + 1 );
					}
					else
					{
						gfxVirt.DrawLine( spPen, rectGripper.Left, rectGripper.Top, rectGripper.Right - 1, rectGripper.Top );
						gfxVirt.DrawLine( p1, rectGripper.Left, rectGripper.Top + 1, rectGripper.Right - 1, rectGripper.Top + 1 );
					}

					spPen.Dispose();
                    p1.Dispose();
				}
				whitebrush.Dispose();
				darkrush.Dispose();
				alphawhite.Dispose();
			}
            else if (this.vStyle == VisualStyle.Office2010)
            {
                SolidBrush whitebrush = new SolidBrush(Color.White);
                SolidBrush darkrush = new SolidBrush(m_office2010ColorTable.GroupBarBorderColor);
                SolidBrush alphawhite = new SolidBrush(Color.FromArgb(125, Color.White));
                using (CMirroredDrawer mdGripper = new CMirroredDrawer(gph, rcsplitter, bIsMirrored))
                {
                    Graphics gfxVirt = mdGripper.VirtualGfx;
                    Rectangle rectGripper = mdGripper.VirtualBounds;

                    //Draw splitter up border.
                    Pen spPen = new Pen(m_office2010ColorTable.GroupBarBorderColor, 1);
                    Pen p1 = new Pen(Color.White, 1);
                    if (bIsMirrored)
                    {
                        gfxVirt.DrawLine(spPen, rectGripper.Left, rectGripper.Top, rectGripper.Right, rectGripper.Top);
                        gfxVirt.DrawLine(p1, rectGripper.Left, rectGripper.Top + 1, rectGripper.Right, rectGripper.Top + 1);
                    }
                    else
                    {
                        gfxVirt.DrawLine(spPen, rectGripper.Left+4, rectGripper.Top + 1, rectGripper.Right - 6, rectGripper.Top + 1);
                        gfxVirt.DrawLine(spPen, rectGripper.Left+4, rectGripper.Top + 5, rectGripper.Right - 6, rectGripper.Top + 5);
                        gfxVirt.DrawLine(spPen, rectGripper.Left+4, rectGripper.Top + 1, rectGripper.Left+4, rectGripper.Top + 5);
                        gfxVirt.DrawLine(spPen, rectGripper.Right - 6, rectGripper.Top + 1, rectGripper.Right - 6, rectGripper.Top + 5);
                    }

                    spPen.Dispose();
                    p1.Dispose();
                }
                whitebrush.Dispose();
                darkrush.Dispose();
                alphawhite.Dispose();
            }
			else
			{
				SolidBrush whitebrush = new SolidBrush( Color.White );
				SolidBrush blackbrush = new SolidBrush( Color.Black );
				SolidBrush alphawhite = new SolidBrush( Color.FromArgb( 125, Color.White ) );
				using( CMirroredDrawer mdGripper = new CMirroredDrawer( gph, rcsplitter, bIsMirrored ) )
				{
					Graphics gfxVirt = mdGripper.VirtualGfx;
					Rectangle rectGripper = mdGripper.VirtualBounds;

					Point ptgripper = new Point( (rectGripper.Width / 2) - 17, rectGripper.Top + 2 );
					for( int i = 0; i < 9; i++ )
					{
						ptgripper.Offset( 1, 1 );
						gfxVirt.FillRectangle( whitebrush, ptgripper.X, ptgripper.Y, 2, 2 );
						ptgripper.Offset( -1, -1 );
						gfxVirt.FillRectangle( blackbrush, ptgripper.X, ptgripper.Y, 2, 2 );
						ptgripper.Offset( 1, 1 );
						gfxVirt.FillRectangle( alphawhite, ptgripper.X, ptgripper.Y, 2, 2 );
						ptgripper.Offset( 3, -1 );
					}
				}
				whitebrush.Dispose();
				blackbrush.Dispose();
				alphawhite.Dispose();
			}
			splitterbrush.Dispose();

			if( this.Collapsed && this.activeItemsList.Count > 0 )
			{
				DrawCollapsedClientArea( gph, bIsMirrored, bordercolor );
			}

			// Draw GroupBarItems.
			Rectangle rcdraw = new Rectangle( rcbounds.Left, rcsplitter.Bottom, rcbounds.Width, this.nItemHeight );
			for( int i=0; i<this.activeItemsList.Count; i++ )
			{
				if( this.alNavPaneItems.Count > 0 )
				{
					// Ignore navigation pane items. These are drawn when the navigation pane is rendered.
					if( this.alNavPaneItems.Contains( this.activeItemsList[i] ) )
						continue;
				}

				GroupBarItem item = this.activeItemsList[i] as GroupBarItem;
				this.DrawGroupBarItem( gph, i, rcdraw );

				if( item.LargeImageMode )
				{
					this.DrawGroupBarImage( gph, i, rcdraw );
				}
				else
				{
					this.DrawGroupBarIcon( gph, i, rcdraw );
				}

				if( !this.Collapsed )
				{
					this.DrawGroupBarText( gph, i, rcdraw );
				}

				rcdraw.Y = rcdraw.Bottom;
			}

            if(ShowNavigationPane)
			    this.DrawNavigationPane( gph );

		}

		internal Brush GetSplitterBrush( Rectangle rcSplitter )
		{
			Brush brush;

			if( this.vStyle == VisualStyle.Office2003 )
			{
				brush = new LinearGradientBrush( rcSplitter, Office2003Colors.CommandBarDropDownColorLight,
					Office2003Colors.CommandBarDropDownColorDark, LinearGradientMode.Vertical );
			}
			else if( this.vStyle == VisualStyle.Office2007 )
			{
				brush = new LinearGradientBrush( rcSplitter, m_office2007ColorTable.GroupBarSplitterColorLight,
					m_office2007ColorTable.GroupBarSplitterColorDark, LinearGradientMode.Vertical );
			}
            else if (this.vStyle == VisualStyle.Office2010)
            {
                brush = new LinearGradientBrush(rcSplitter, m_office2010ColorTable.GroupBarSplitterColorLight,
                    m_office2010ColorTable.GroupBarSplitterColorDark, LinearGradientMode.Vertical);
            }
			else
			{
				brush = new SolidBrush( SystemColors.ControlDark );
			}

			return brush;
		}

		/// <summary>
		/// Draws the Header for <see cref="GroupBar"/>.
		/// </summary>
		/// <remarks>The <see cref="GroupBar"/> control calls this method to paint the specified GroupBar Header.
		/// Override this method to customize the GroupBar Header drawing.
		/// </remarks>
		/// <param name="gph">A <see cref="System.Drawing.Graphics"/> object.</param>
		protected virtual void DrawHeader( Graphics gph )
		{
			m_bDrawingHeader = true;

			Rectangle rcbounds = this.GetBoundedRectangle();
			Rectangle rcheader = new Rectangle( rcbounds.Left, rcbounds.Top, rcbounds.Width, this.nHeaderHeight );

            if (rcheader.Width > 0 && rcheader.Height > 0)
            {
                bool bIsMirrored = this.GetIsMirrored();

                GroupBarItem item = (this.GroupExists(nSelectedItem)) ? this.activeItemsList[this.nSelectedItem] as GroupBarItem : null;
                Brush headerbrush = GetHeaderBrush(rcheader);

                gph.FillRectangle(headerbrush, rcheader);
                if(this.VisualStyle==VisualStyle.Office2010)
                    gph.DrawLine(new Pen(this.BorderColor), rcheader.Left, rcheader.Bottom, rcheader.Right, rcheader.Bottom);
                
                headerbrush.Dispose();

                DrawHeaderLines(gph, bIsMirrored, rcheader);

                if (this.Collapsed)
                {
                    DrawHeaderCollapsed(gph, bIsMirrored, rcheader);
                }
                else if (item != null)
                {
                    DrawHeaderExpanded(gph, bIsMirrored, rcheader, item);
                }

                if (this.AllowCollapse)
                {
                    DrawCollapseButton(gph, bIsMirrored);
                }
            }
			
            m_bDrawingHeader = false;
		}

		private void DrawHeaderLines( Graphics gph, bool bIsMirrored, Rectangle rcheader )
		{
            Pen p=new Pen( Color.White );
			if( this.vStyle == VisualStyle.Office2007 && this.BorderStyle == BorderStyle.FixedSingle )
			{
				gph.DrawLine( p, rcheader.Left, rcheader.Top, rcheader.Right - 1, rcheader.Top );

				if( bIsMirrored )
				{
					gph.DrawLine(p, rcheader.Right - 1, rcheader.Bottom - 1, rcheader.Right - 1, rcheader.Top );
				}
				else
				{
					gph.DrawLine( p, rcheader.Left, rcheader.Bottom - 1, rcheader.Left, rcheader.Top );
				}
			}
            p.Dispose();
		}

		private void DrawHeaderExpanded( Graphics gph, bool bIsMirrored, Rectangle rcheader, GroupBarItem item )
		{
			if( this.ShowItemImageInHeader && (item.Icon != null || item.Image != null) )
			{
				if( item.LargeImageMode )
				{
					this.DrawGroupBarImage( gph, this.nSelectedItem, rcheader );
				}
				else
				{
					this.DrawGroupBarIcon( gph, this.nSelectedItem, rcheader );
				}

				if( this.vStyle != VisualStyle.Office2003 )
				{
					rcheader.Offset( bIsMirrored ? -1 : 1, 1 );
				}

				Rectangle rctext = CalcTextRect( gph, item, rcheader );
				
                if(this.TextAlign != TextAlignment.Center)
				    rctext.Width = rcheader.Right - rctext.Left;

				this.DrawHeaderText( gph, item, rctext );
			}
			else
			{
				this.DrawHeaderText( gph, item, rcheader );
			}
		}

		private Brush GetHeaderBrush( Rectangle rcHeader )
		{
			Brush headerBrush = new SolidBrush( this.hdrBackColor );

			if( this.vStyle == VisualStyle.Office2003 )
			{
				headerBrush = new LinearGradientBrush( rcHeader, Office2003Colors.GroupBarHeaderColorLight,
					Office2003Colors.GroupBarHeaderColorDark, LinearGradientMode.Vertical );
			}
			else if( this.vStyle == VisualStyle.Office2007 )
			{
                headerBrush = new LinearGradientBrush(rcHeader, m_office2007ColorTable.GroupBarHeaderColorLight,
					m_office2007ColorTable.GroupBarHeaderColorDark, LinearGradientMode.Vertical );
			}
            else if (this.vStyle == VisualStyle.Office2010)
            {
                headerBrush = new LinearGradientBrush(rcHeader, m_office2010ColorTable.GroupBarHeaderColorLight,
                    m_office2010ColorTable.GroupBarHeaderColorDark, LinearGradientMode.Vertical);
            }

			return headerBrush;
		}

		private void DrawHeaderText( Graphics gph, GroupBarItem item, Rectangle rect )
		{
			if( GroupExists( nSelectedItem ) )
			{
				if( this.AllowCollapse )
				{
					int nCollapseButtonWidth = this.CollapseButtonBounds.Width;

					rect.Width -= nCollapseButtonWidth;

					if( this.RightToLeft == RightToLeft.Yes )
					{
						rect.X += nCollapseButtonWidth;
					}
				}

				TextRenderer.DrawText( gph, item.Text, this.hdrFont, rect, this.hdrForeColor, this.HeaderTextFormat );
			}
		}

		/// <summary>
		/// Draws the <see cref="GroupBarItem"/> object.
		/// </summary>
		/// <remarks>The <see cref="GroupBar"/> control calls this method to paint the specified GroupBarItem.
		/// Override this method to customize the GroupBar drawing.
		/// </remarks>
		/// <param name="gfxTarget">A <see cref="System.Drawing.Graphics"/> object.</param>
		/// <param name="nindex">The zero-based index of the <see cref="GroupBarItem"/> to be drawn.</param>
		/// <param name="rcbar">A <see cref="System.Drawing.Rectangle"/> value specifying the GroupBarItem bounds.</param>
		protected virtual void DrawGroupBarItem( Graphics gfxTarget, int nindex, Rectangle rcbar )
		{
			if( (rcbar.Width <= 0) || (rcbar.Height <= 0) )
				return;

			GroupBarItem item = this.activeItemsList[nindex] as GroupBarItem;
			Rectangle rcdraw = rcbar;

			bool bpressed = false;
			if( ((nindex == this.nHighlightItem) && (this.eButtonsState == ButtonsState.GroupBarItemPushed))
				|| ((this.bStackedMode == true) && (nindex == this.nSelectedItem)) )
				bpressed = true;

			ButtonState btnstate = ButtonState.Normal;
			if( bpressed )
				btnstate = ButtonState.Pushed;
			if( item.Enabled == false )
				btnstate = ButtonState.Inactive;

			bool bIsMirrorred = GetIsMirrored();

			if( this.bIntegratedScrolling )
			{
				Rectangle rcscroll = new Rectangle(
					bIsMirrorred ? rcdraw.Left - this.nScrollThumbWidth : rcdraw.Right,
					rcdraw.Top, this.nScrollThumbWidth, this.nItemHeight );
				if( nindex == this.nSelectedItem )
					this.DrawScrollThumb( gfxTarget, true, rcscroll );
				else if( nindex == this.nSelectedItem+1 )
					this.DrawScrollThumb( gfxTarget, false, rcscroll );
			}

			CMirroredDrawer mdDrawer = new CMirroredDrawer( gfxTarget, rcdraw, bIsMirrorred );
			Graphics gfx = mdDrawer.VirtualGfx;
			Rectangle rectVirt = mdDrawer.VirtualBounds;

			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.UseThemedDrawing )
			{
				int pbstate = (btnstate == ButtonState.Pushed) ? ThemeStates.PBS_PRESSED : ThemeStates.PBS_NORMAL;

				// WORKAROUND: theme backgrounds contain transparent pixels which lose alpha-channel 
				// info after bitmap drawing.
				// Mirroring is turned off due to RTL theme background is almost identical to LTR
				this.tdButton.DrawMirrored = false; // bIsMirrorred;

				this.tdButton.DrawThemeBackground( gfxTarget, ThemeParts.BP_PUSHBUTTON, pbstate, rcdraw );

				// Draw the highlighted button.
				if( (this.bBarHighlight == true) && (this.bHighlight == true) && (this.eButtonsState == ButtonsState.None) 
					&& (this.nHighlightItem >=0) && (item.Equals( this.activeItemsList[this.nHighlightItem] )) )
				{
					this.tdButton.DrawThemeBackground( gfxTarget, ThemeParts.BP_PUSHBUTTON, ThemeStates.PBS_HOT, rcdraw );
				}
			}
			else if( this.vStyle == VisualStyle.Office2003 )
			{
				// Fill button rectangle with the suitable gradient colors.
				LinearGradientBrush lgb = null;
				Color color1, color2;

				if( this.nSelectedItem == nindex )
				{
					if( (this.bHighlight == true) && (this.nHighlightItem == nindex) )
					{
						color1 = Office2003Colors.GroupBarSelectedHighlightColorDark;
						color2 = Office2003Colors.GroupBarSelectedHighlightColorLight;
					}
					else
					{
						color1 = Office2003Colors.GroupBarSelectedColorLight;
						color2 = Office2003Colors.GroupBarSelectedColorDark;
					}
				}
				else
				{
					if( (this.bHighlight == true) && (this.nHighlightItem == nindex) )
					{
						if( this.eButtonsState == ButtonsState.GroupBarItemPushed )
						{
							color1 = Office2003Colors.GroupBarSelectedHighlightColorDark;
							color2 = Office2003Colors.GroupBarSelectedHighlightColorLight;
						}
						else
						{
							color1 = Office2003Colors.GroupBarHighlightColorLight;
							color2 = Office2003Colors.GroupBarHighlightColorDark;
						}
					}
					else
					{
						color1 = Office2003Colors.MenuMarginColorLight;
						color2 = Office2003Colors.MenuMarginColorDark;
					}
				}

				lgb = new LinearGradientBrush( rectVirt, color1, color2, LinearGradientMode.Vertical );
				gfx.FillRectangle( lgb, rectVirt );
				lgb.Dispose();

				// Draw the item border.
				Pen borderpen = new Pen( Office2003Colors.CommandBarDropDownColorDark, 1 );
				gfx.DrawLine( borderpen, new Point( rectVirt.Left, rectVirt.Top ), new Point( rectVirt.Right-1, rectVirt.Top ) );
				gfx.DrawLine( borderpen, new Point( rectVirt.Left, rectVirt.Bottom ), new Point( rectVirt.Right-1, rectVirt.Bottom ) );
				borderpen.Dispose();
			}
			else if( this.vStyle == VisualStyle.Office2007 )
			{
				// Fill button rectangle with the suitable gradient colors.
				LinearGradientBrush lgb = null;
				Color color1, color2;
				Color colorTop1 = m_office2007ColorTable.GroupBarSelectedTopColorLight;
				Color colorTop2 = m_office2007ColorTable.GroupBarSelectedTopColorDark;
				Blend blend = new Blend();

				if( this.nSelectedItem == nindex )
				{
					if( (this.bHighlight == true) && (this.nHighlightItem == nindex) )
					{
						color1 = m_office2007ColorTable.GroupBarSelectedHighlightColorLight;
						color2 = m_office2007ColorTable.GroupBarSelectedHighlightColorDark;
					}
					else
					{
						color1 = m_office2007ColorTable.GroupBarSelectedColorLight;
						color2 = m_office2007ColorTable.GroupBarSelectedColorDark;
					}
				}
				else
				{
					if( (this.bHighlight == true) && (this.nHighlightItem == nindex) )
					{
						if( this.eButtonsState == ButtonsState.GroupBarItemPushed )
						{
							color1 = m_office2007ColorTable.GroupBarSelectedHighlightColorLight;
							color2 = m_office2007ColorTable.GroupBarSelectedHighlightColorDark;
						}
						else
						{
							color1 = m_office2007ColorTable.GroupBarHighlightColorLight;
							color2 = m_office2007ColorTable.GroupBarHighlightColorDark;
						}
					}
					else
					{
						color1 = m_office2007ColorTable.GroupBarItemColorLight;
						color2 = m_office2007ColorTable.GroupBarItemColorDark;
					}
				}

				if( this.nSelectedItem == nindex && this.nHighlightItem != nindex )
				{
					Color[] colors = new Color[] { colorTop1, colorTop2, color2, color1 };

					lgb = new LinearGradientBrush( rectVirt, Color.Empty, Color.Empty, LinearGradientMode.Vertical );
					lgb.InterpolationColors = this.GetColorBlend( colors );
				}
				else
				{
					blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };

					if( this.nSelectedItem == nindex && this.nHighlightItem == nindex )
						blend.Factors = new float[] { 0.2F, 0.6F, 1.0F, 0.0F };
					else
						blend.Factors = new float[] { 0.0F, 0.5F, 1.0F, 0.5F };

					lgb = new LinearGradientBrush( rectVirt, color1, color2, LinearGradientMode.Vertical );
					lgb.Blend = blend;
				}

				gfx.FillRectangle( lgb, rectVirt );

				lgb.Dispose();

				// Draw the item border.
				Pen borderpen = new Pen( m_office2007ColorTable.GroupBarBorderColor, 1 );
				if( bIsMirrorred )
				{
					gfx.DrawLine( borderpen, new Point( rectVirt.Left, rectVirt.Top ), new Point( rectVirt.Right, rectVirt.Top ) );
					gfx.DrawLine( borderpen, new Point( rectVirt.Left, rectVirt.Bottom ), new Point( rectVirt.Right, rectVirt.Bottom ) );
				}
				else
				{
					gfx.DrawLine( borderpen, new Point( rectVirt.Left, rectVirt.Top ), new Point( rectVirt.Right - 1, rectVirt.Top ) );
					gfx.DrawLine( borderpen, new Point( rectVirt.Left, rectVirt.Bottom ), new Point( rectVirt.Right - 1, rectVirt.Bottom ) );
				}

				if( this.IntegratedScrolling && 
					((nindex == this.nSelectedItem) || (nindex == this.nSelectedItem+1)) )
					gfx.DrawLine( borderpen, new Point( rectVirt.Right, rectVirt.Top ), new Point( rectVirt.Right, rectVirt.Bottom ) );

				borderpen.Dispose();
			}
            else if (this.vStyle == VisualStyle.Office2010)
            {
                Rectangle rcdraw1 = new Rectangle(rcdraw.X + 2, rcdraw.Y + 2, rcdraw.Width - 4, rcdraw.Height - 4);
                mdDrawer = new CMirroredDrawer(gfxTarget, rcdraw1, bIsMirrorred);
                gfx = mdDrawer.VirtualGfx;
                rectVirt = mdDrawer.VirtualBounds;
                // Fill button rectangle with the suitable gradient colors.
                LinearGradientBrush lgb = null;
                Color color1, color2;
                Color colorTop1 = m_office2010ColorTable.GroupBarSelectedTopColorLight;
                Color colorTop2 = m_office2010ColorTable.GroupBarSelectedTopColorDark;
                Blend blend = new Blend();
                if (this.nSelectedItem == nindex)
                {
                    if ((this.bHighlight == true) && (this.nHighlightItem == nindex))
                    {
                        color1 = m_office2010ColorTable.GroupBarSelectedHighlightColorLight;
                        color2 = m_office2010ColorTable.GroupBarSelectedHighlightColorDark;
                    }
                    else
                    {
                        color1 = m_office2010ColorTable.GroupBarSelectedColorLight;
                        color2 = m_office2010ColorTable.GroupBarSelectedColorDark;
                    }
                }
                else
                {
                    if ((this.bHighlight == true) && (this.nHighlightItem == nindex))
                    {
                        if (this.eButtonsState == ButtonsState.GroupBarItemPushed)
                        {
                            color1 = m_office2010ColorTable.GroupBarSelectedHighlightColorLight;
                            color2 = m_office2010ColorTable.GroupBarSelectedHighlightColorDark;
                        }
                        else
                        {
                            color1 = m_office2010ColorTable.GroupBarHighlightColorLight;
                            color2 = m_office2010ColorTable.GroupBarHighlightColorDark;
                        }
                    }
                    else
                    {
                        color1 = m_office2010ColorTable.GroupBarItemColorLight;
                        color2 = m_office2010ColorTable.GroupBarItemColorDark;
                    }
                }
                
                lgb = new LinearGradientBrush(rectVirt, Color.Empty, Color.Empty, LinearGradientMode.Vertical);
                gfx.FillRectangle(lgb, rectVirt);
                if (this.nSelectedItem == nindex && this.nHighlightItem != nindex)
                {
                    Color[] colors = new Color[] { colorTop1, colorTop2, color1, color1 };

                    lgb = new LinearGradientBrush(rectVirt, m_office2010ColorTable.GroupBarBorderColor, Color.Empty, LinearGradientMode.Vertical);
                    lgb.InterpolationColors = this.GetColorBlend(colors);
                    gfx.FillRectangle(lgb, rectVirt);
                    Pen borderpen = new Pen(m_office2010ColorTable.GroupBarBorderColor, 1);
                    if (bIsMirrorred)
                    {
                        gfx.DrawLine(borderpen, new Point(rectVirt.Left, rectVirt.Top), new Point(rectVirt.Right, rectVirt.Top));
                        gfx.DrawLine(borderpen, new Point(rectVirt.Left, rectVirt.Bottom), new Point(rectVirt.Right, rectVirt.Bottom));
                    }
                    else
                    {
                        gfx.DrawLine(borderpen, new Point(rectVirt.Left, rectVirt.Top - 1), new Point(rectVirt.Right - 1, rectVirt.Top - 1));
                        gfx.DrawLine(borderpen, new Point(rectVirt.Left, rectVirt.Bottom), new Point(rectVirt.Right - 1, rectVirt.Bottom));
                        gfx.DrawLine(borderpen, new Point(rectVirt.Left - 1, rectVirt.Top - 1), new Point(rectVirt.Left - 1, rectVirt.Bottom));
                        gfx.DrawLine(borderpen, new Point(rectVirt.Right, rectVirt.Top - 1), new Point(rectVirt.Right, rectVirt.Bottom));
                    }

                    if (this.IntegratedScrolling &&
                        ((nindex == this.nSelectedItem) || (nindex == this.nSelectedItem + 1)))
                        gfx.DrawLine(borderpen, new Point(rectVirt.Right, rectVirt.Top), new Point(rectVirt.Right, rectVirt.Bottom));

                    borderpen.Dispose();
                }
                if ((this.bHighlight == true) && (this.nHighlightItem == nindex))
                {
                    blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };

                    if (this.nSelectedItem == nindex && this.nHighlightItem == nindex)
                        blend.Factors = new float[] { 0.2F, 0.6F, 1.0F, 0.0F };
                    else
                        blend.Factors = new float[] { 0.0F, 0.5F, 1.0F, 0.5F };

                    Pen borderpen = new Pen(m_office2010ColorTable.GroupBarBorderColor, 1);
                    if (bIsMirrorred)
                    {
                        gfx.DrawLine(borderpen, new Point(rectVirt.Left, rectVirt.Top), new Point(rectVirt.Right, rectVirt.Top));
                        gfx.DrawLine(borderpen, new Point(rectVirt.Left, rectVirt.Bottom), new Point(rectVirt.Right, rectVirt.Bottom));
                    }
                    else
                    {
                        gfx.DrawLine(borderpen, new Point(rectVirt.Left, rectVirt.Top - 1), new Point(rectVirt.Right - 1, rectVirt.Top - 1));
                        gfx.DrawLine(borderpen, new Point(rectVirt.Left, rectVirt.Bottom), new Point(rectVirt.Right - 1, rectVirt.Bottom));
                        gfx.DrawLine(borderpen, new Point(rectVirt.Left - 1, rectVirt.Top - 1), new Point(rectVirt.Left - 1, rectVirt.Bottom));
                        gfx.DrawLine(borderpen, new Point(rectVirt.Right, rectVirt.Top - 1), new Point(rectVirt.Right, rectVirt.Bottom));
                    }

                    if (this.IntegratedScrolling &&
                        ((nindex == this.nSelectedItem) || (nindex == this.nSelectedItem + 1)))
                        gfx.DrawLine(borderpen, new Point(rectVirt.Right, rectVirt.Top), new Point(rectVirt.Right, rectVirt.Bottom));

                    borderpen.Dispose();
                    lgb = new LinearGradientBrush(rectVirt, color1, color2, LinearGradientMode.Vertical);
                    lgb.Blend = blend;
                    gfx.FillRectangle(lgb, rectVirt);

                }
                lgb.Dispose();
                //// Draw the item border.
                //Pen borderpen1 = new Pen(m_office2010ColorTable.GroupBarBorderColor, 1);
                //gfx.DrawLine(borderpen1, new Point(rectVirt.Left, rectVirt.Top), new Point(rectVirt.Right - 1, rectVirt.Top));
                //gfx.DrawLine(borderpen1, new Point(rectVirt.Left, rectVirt.Bottom), new Point(rectVirt.Right - 1, rectVirt.Bottom));
                //borderpen1.Dispose();
            }
			else if (vStyle == VisualStyle.Metro)
			{
				SolidBrush brush = new SolidBrush(this.HeaderBackColor);
				gfx.FillRectangle(brush, rectVirt);
				brush.Dispose();
				Pen pen = new Pen(this.BorderColor);
				Rectangle r = new Rectangle();
				r = rectVirt;
				r.Width = r.Width - 1;
				r.Height = r.Height - 1;
				gfx.DrawRectangle(pen, r);
				pen.Dispose();
			}
			else
			{
				ControlPaint.DrawButton( gfx, rectVirt, btnstate|(this.bFlatLook ? ButtonState.Flat : 0) );
				if( this.bIntegratedScrolling == true )
				{
					// Draw a line on the bottom & right of items to provide a flush look in the IntegratedScrolling mode.
					if( this.nSelectedItem == nindex )
					{
						ControlPaint.DrawBorder( gfx, rectVirt, Color.Empty, 0, ButtonBorderStyle.None,
							Color.Empty, 0, ButtonBorderStyle.None, SystemColors.Control, 1, ButtonBorderStyle.Solid,
							Color.Empty, 0, ButtonBorderStyle.None );
                        using(Pen pen=new Pen( item.ClientBorderColors.Top, 1 ))
                            gfx.DrawLine(pen,
							new Point( rectVirt.Left, rectVirt.Bottom-1 ), new Point( rectVirt.Right-2, rectVirt.Bottom-1 ) );
					}
					else
					{
						ControlPaint.DrawBorder( gfx, rectVirt, Color.Empty, 0, ButtonBorderStyle.None,
						Color.Empty, 0, ButtonBorderStyle.None, SystemColors.Control, 1, ButtonBorderStyle.Solid,
						SystemColors.Control, 1, ButtonBorderStyle.Solid );
					}
				}

				// If the GroupBarItem has a preset background brush or if the application provides one through 
				// the ProvideGroupBarItemBrush event, then use that brush for rendering the background.
				Brush bkgrndbrush = item.BackgroundBrush;
				ProvideGroupBarItemBrushEventArgs args = new ProvideGroupBarItemBrushEventArgs( nindex, rcdraw );
				this.OnProvideGroupBarItemBrush( args );
				if( args.BackgroundBrush != null )
					bkgrndbrush = args.BackgroundBrush;

				if( bkgrndbrush != null )
				{
					Rectangle rcbkgrnd = rectVirt;
					if( bpressed )
					{
						rcbkgrnd.Offset( 3, 3 );
						rcbkgrnd.Width -= 5;
						rcbkgrnd.Height -= 5;
					}
					else
					{
						rcbkgrnd.Offset( 1, 1 );
						rcbkgrnd.Width -= 3;
						rcbkgrnd.Height -= 3;
					}
					gfx.FillRectangle( bkgrndbrush, rcbkgrnd );
					if( args.BackgroundBrush != null )
						args.BackgroundBrush.Dispose();
				}

				// Draw the highlighted region of the button.
				if( (this.bBarHighlight == true) && (this.bHighlight == true)
					&& (this.nHighlightItem >= 0) && (item.Equals( this.activeItemsList[this.nHighlightItem] )) )
				{
					if( this.bStackedMode == true )
					{
						rcdraw.Y += 1;
						rcdraw.Height -= 2;
						rcdraw.Width -= 1;
					}
					else
					{
						rcdraw.Height -= 1;
						rcdraw.Width -= 1;
					}
					ControlPaint.DrawBorder3D( gfx, rectVirt, Border3DStyle.Raised, Border3DSide.Bottom | Border3DSide.Right );
				}
			}

			mdDrawer.Dispose();
		}

		private ColorBlend GetColorBlend( Color[] colors )
		{
			ColorBlend blend = new ColorBlend( colors.Length );
			blend.Colors = colors;
			blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };
			return blend;
		}

		/// <summary>
		/// Draws a border around the selected client control.
		/// </summary>
		/// <remarks>The <see cref="GroupBar"/> control calls this method to paint a border around the 
		/// the current selected client control. Override this method to customize the GroupBar drawing.
		/// </remarks>
		/// <param name="gph">A <see cref="System.Drawing.Graphics"/> object.</param>
		/// <param name="rcclient">A <see cref="System.Drawing.Rectangle"/> value specifying the client control bounds.</param>
		protected virtual void DrawSelectedGroupClientBorder( Graphics gph, Rectangle rcclient )
		{
			if( (rcclient.Width <= 0) || (rcclient.Height <= 0) )
				return;

			if( !this.GroupExists( nSelectedItem ) )
			{
				Debug.Assert( false, "Invalid index" );
				return;
			}
			GroupBarItem item = this.activeItemsList[this.nSelectedItem] as GroupBarItem;
            Pen pen = new Pen(item.ClientBorderColors.Left, 1);
			gph.DrawLine( pen, new Point( rcclient.Left, rcclient.Bottom-1 ), new Point( rcclient.Left, rcclient.Top ) );
            pen=new Pen( item.ClientBorderColors.Top, 1 );
			gph.DrawLine( pen, new Point( rcclient.Left, rcclient.Top ), new Point( rcclient.Right-1, rcclient.Top ) );
            pen= new Pen( item.ClientBorderColors.Right, 1 );
			gph.DrawLine(pen, new Point( rcclient.Right-1, rcclient.Top ), new Point( rcclient.Right-1, rcclient.Bottom-1 ) );
            pen=new Pen( item.ClientBorderColors.Bottom, 1 );
            gph.DrawLine(pen, new Point(rcclient.Left, rcclient.Bottom - 1), new Point(rcclient.Right - 2, rcclient.Bottom - 1));
            pen.Dispose();
		}


		private void OffsetIconBar( bool bPressed, ref Rectangle rcbar )
		{
			if (bPressed)
			{
				rcbar.X += c_nPressedOffset;
				rcbar.Y += c_nPressedOffset;
				rcbar.Width -= c_nPressedGroupBarItemBorders;
				rcbar.Height -= c_nPressedGroupBarItemBorders;
			}
			else
			{
				rcbar.X += c_nOffset;
				rcbar.Y += c_nOffset;
				rcbar.Width -= c_nGroupBarItemBorders;
				rcbar.Height -= c_nGroupBarItemBorders;
			}
		}
		/// <summary>
		/// Draws the <see cref="GroupBarItem"/> icon.
		/// </summary>
		/// <remarks>
		/// The <see cref="GroupBar"/> control calls this method to paint the specified 
		/// GroupBarItem's icon. Override this method to customize the GroupBar drawing.
		/// </remarks>
		/// <param name="gph">A <see cref="System.Drawing.Graphics"/> object.</param>
		/// <param name="nindex">The zero-based index of the <see cref="GroupBarItem"/> for which the icon is drawn.</param>
		/// <param name="rcbar">A <see cref="System.Drawing.Rectangle"/> value specifying the GroupBarItem bounds.</param>
		protected virtual void DrawGroupBarImage( Graphics gph, int nindex, Rectangle rcbar )
		{
			GroupBarItem item = this.activeItemsList[nindex] as GroupBarItem;
			if( (item.Image == null) || (rcbar.Width <= 0) || (rcbar.Height <= 0) )
				return;

			bool bIsMirrored = GetIsMirrored();
			bool bPressed = ((nindex == this.nHighlightItem) && (this.eButtonsState == ButtonsState.GroupBarItemPushed))
				|| ((this.bStackedMode == true) && (nindex == this.nSelectedItem));

			this.OffsetIconBar( bPressed, ref rcbar );

			Rectangle rc = rcbar;

			if (bPressed && (this.vStyle != VisualStyle.Office2003) && (this.vStyle != VisualStyle.Office2007))
			{
				rc.Offset( bIsMirrored ? -1 : 1, 1 );
			}

			int ImageWidth = 0;
			int ImageHeight = 0;
			float ScaleRatio = 1.0f;
			if( this.bStackedMode )
			{
				ImageHeight = rcbar.Height - nTxtOffset;
				ScaleRatio = (float)ImageHeight / (float)item.Image.Height;
				ImageWidth = (int)(ScaleRatio * item.Image.Width);
			}
			else
			{
				ImageWidth = item.Image.Width;
				ImageHeight = item.Image.Height;
			}

			Rectangle destRect = rc;
			RectangleF srcRect = new Rectangle();

			destRect = this.CalcTextRect( gph, item, rc );

			LocateImageDestination( item, rc, bIsMirrored, ImageWidth, ref destRect );

			AppendImageSourceRectLogic( rc, ImageWidth, item, ScaleRatio, bIsMirrored,
				rcbar, ImageHeight, item.Image.Width, item.Image.Height,
				item.YChanged, gph, ref srcRect, ref destRect );

			bool itemEnabled = item.Enabled && this.Enabled;
			this.DrawImageStateHelper( gph, item.Image, destRect, srcRect, itemEnabled );
		}

		#region Image Renderring Helpers

		private void AppendImageSourceRectLogic( Rectangle rc, int ImageWidth, GroupBarItem item, float ScaleRatio, bool bIsMirrored, Rectangle rcbar, int ImageHeight, int realImageWidth, int realImageHeight, bool yChanged, Graphics gph, ref RectangleF srcRect, ref Rectangle destRect )
		{
			AppendWidthLogic( rc, ImageWidth, ScaleRatio, bIsMirrored, realImageWidth, item, rcbar, ref srcRect, ref destRect );

			AppendHeightLogic( ImageHeight, gph, realImageHeight, item, rcbar, rc, ref srcRect, ref destRect );
		}

		private void AppendHeightLogic( int ImageHeight, Graphics gph, int realImageHeight, GroupBarItem item, Rectangle rcbar, Rectangle rc, ref RectangleF srcRect, ref Rectangle destRect )
		{
			if( destRect.Height < ImageHeight )
			{
				srcRect.Height = destRect.Height;
				srcRect.Y = (realImageHeight / 2) - (destRect.Height / 2);
			}
			else
			{
				srcRect.Height = realImageHeight;
				destRect.Height = ImageHeight;
				if( item.YChanged )
				{
					int imgAndTextHeight = realImageHeight + TextRenderer.MeasureText(item.Text, item.Font, destRect.Size, this.ItemTextFormat).Height;
					destRect.Y = rcbar.Y + ((rcbar.Height - imgAndTextHeight) / 2);
				}
				else
				{
                    if(item.LargeImageMode)
                        destRect.Y = rc.Y + ((rc.Height / 2)-(ImageHeight / 2));
                    else
                        destRect.Y = rc.Y + nTxtOffset / 2;
				}
			}
		}

		private void AppendWidthLogic( Rectangle rc, int ImageWidth, float ScaleRatio, bool bIsMirrored, int realImageWidth, GroupBarItem item, Rectangle rcbar, ref RectangleF srcRect, ref Rectangle destRect )
		{
			if( rc.Width < ImageWidth )
			{
				srcRect.Width = (rc.Width / ScaleRatio);
				destRect.Width = rc.Width;

				switch( this.nAlignment )
				{
					case TextAlignment.Center:
                        srcRect.X = ((item.Image.Width / 2) - (srcRect.Width / 2));
                        break;
					case TextAlignment.Right:
						if( bIsMirrored )
						{
							srcRect.X = realImageWidth - srcRect.Width;
						}
						break;
					case TextAlignment.Left:
						if( !bIsMirrored )
						{
							destRect.X -= realImageWidth;
							if( destRect.X < rc.X )
								destRect.X = rc.X;
						}
						else
						{
							srcRect.X = realImageWidth - srcRect.Width;
						}

						break;
				}
			}
			else
			{
				srcRect.Width = realImageWidth;
				destRect.Width = ImageWidth;
				if( item.YChanged )
				{
					switch( this.nAlignment )
					{
						case TextAlignment.Center:
							destRect.X = ((rc.Width / 2) - (ImageWidth / 2)) + rcbar.X;
							break;
						case TextAlignment.Right:
							if( bIsMirrored )
							{
								destRect.X = rc.X;
							}
							else
							{
								destRect.X = (rc.Width - ImageWidth) + rcbar.X;
							}
							break;
						case TextAlignment.Left:
							if( bIsMirrored )
							{
								destRect.X = (rc.X + rc.Width) - ImageWidth;
							}
							else
							{
								destRect.X = rcbar.X;
							}
							break;
					}
				}
			}
		}

		private void LocateImageDestination( GroupBarItem item, Rectangle rc, bool bIsMirrored, int ImageWidth, ref Rectangle destRect )
		{
			if( this.Collapsed )
			{
				destRect.X = rc.X + (rc.Width - ImageWidth) / 2;
			}
			else
			{
				LocateImageDestinationExpnaded( item, rc, bIsMirrored, ImageWidth, ref destRect );
			}
		}

		private void LocateImageDestinationExpnaded( GroupBarItem item, Rectangle rc, bool bIsMirrored, int ImageWidth, ref Rectangle destRect )
		{
			if( item.YChanged )
			{
				switch( this.nAlignment )
				{
					case TextAlignment.Center:
						destRect.X -= nTxtOffset;
						break;
					case TextAlignment.Right:
						destRect.X = rc.X;
						break;
					case TextAlignment.Left:
						if( bIsMirrored )
						{
							destRect.X = (rc.X + rc.Width) - ImageWidth;
						}
						else
						{
							destRect.X -= nTxtOffset;
						}
						break;
				}
				if( destRect.X < rc.X )
					destRect.X = rc.X;
				if( bIsMirrored )
					destRect.X += 1;
			}
			else
			{
				if( bIsMirrored )
				{
					if( item.IsTextVisible )
					{
						destRect.X = destRect.Right + item.Padding;
					}
					else
					{
						destRect.X = rc.X;
					}
				}

				// For all aligns		
				if( !bIsMirrored )
				{
					destRect.X -= (ImageWidth + item.Padding);
				}
				if( destRect.X < rc.X )
					destRect.X = rc.X;
			}
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void DrawImageStateHelper( Graphics gph, Image image, Rectangle destRect, RectangleF srcRect, bool bEnabled )
		{
			if( !bEnabled )
			{
				DrawingUtils.DrawGrayedImage( gph, image, destRect, srcRect );
			}
			else
			{
				gph.DrawImage( image, destRect );
			}
		}


		#endregion

		#region Icon Renderring
		/// <summary>
		/// Draws the <see cref="GroupBarItem"/> icon.
		/// </summary>
		/// <remarks>
		/// The <see cref="GroupBar"/> control calls this method to paint the specified 
		/// GroupBarItem's icon. Override this method to customize the GroupBar drawing.
		/// </remarks>
		/// <param name="gph">A <see cref="System.Drawing.Graphics"/> object.</param>
		/// <param name="nindex">The zero-based index of the <see cref="GroupBarItem"/> for which the icon is drawn.</param>
		/// <param name="rcbar">A <see cref="System.Drawing.Rectangle"/> value specifying the GroupBarItem bounds.</param>
		protected virtual void DrawGroupBarIcon( Graphics gph, int nindex, Rectangle rcbar )
		{
			GroupBarItem item = this.activeItemsList[nindex] as GroupBarItem;
			if( (item.Icon == null) || (rcbar.Width <= 0) || (rcbar.Height <= 0) )
				return;

			bool bIsMirrored = GetIsMirrored();
			bool bPressed = ((nindex == this.nHighlightItem) && (this.eButtonsState == ButtonsState.GroupBarItemPushed))
				|| (nindex == this.nSelectedItem);

			this.OffsetIconBar( bPressed, ref rcbar );

			Rectangle rc = rcbar;

			if (bPressed && (this.vStyle != VisualStyle.Office2003) && (this.vStyle != VisualStyle.Office2007))
			{
				rc.Offset( bIsMirrored ? -1 : 1, 1 );
			}

			int ImageWidth = 0;
			int ImageHeight = 0;
			float ScaleRatio = 0;

			ImageHeight = rcbar.Height - nTxtOffset;
			ScaleRatio = (float)ImageHeight / (float)item.Icon.Height;
			ImageWidth = (int)(ScaleRatio * item.Icon.Width);

			Rectangle destRect = rc;
			RectangleF srcRect = new Rectangle();

			destRect = this.CalcTextRect( gph, item, destRect );

			LocateImageDestination( item, rc, bIsMirrored, ImageWidth, ref destRect );

			AppendImageSourceRectLogic( rc, ImageWidth, item, ScaleRatio, bIsMirrored,
				rcbar, ImageHeight, item.Icon.Width, item.Icon.Height,
				item.YChanged, gph, ref srcRect, ref destRect );

			bool itemEnabled = item.Enabled && this.Enabled;
			if( m_IconRenderingMode == IconRenderingMode.AlphaBlended )
			{
				DrawIconStateHelper( gph, item.Icon, destRect, itemEnabled, item );
			}
			else
			{
				DrawImageStateHelper( gph, item.Icon.ToBitmap(), destRect, srcRect, item.Enabled );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected Rectangle CalcTextRectForIcon( Graphics gph, GroupBarItem group, Rectangle rcbar )
		{
			Rectangle rctext = rcbar;
			int nTextWidth = TextRenderer.MeasureText(group.Text, group.Font, rcbar.Size, this.ItemTextFormat).Width;

			int nIconSize = c_nIconSize;

			if( this.bStackedMode )
				nIconSize = rcbar.Height - nTxtOffset;

			switch( this.nAlignment )
			{
				case TextAlignment.Left:
					rctext.X = rcbar.X + nTxtOffset;
					if( group.Icon != null )
					{
						rctext.X += nIconSize + nTxtOffset; // offset + icon width
					}
					break;
				case TextAlignment.Center:
					rctext.X = rcbar.X + (rcbar.Width - nTextWidth)/2;
					if( group.Icon != null )
					{
						rctext.X += (nIconSize + nTxtOffset)/2;
					}
					break;
				case TextAlignment.Right:
					rctext.X = rcbar.X + (rcbar.Width - nTxtOffset - nTextWidth);
					break;
				default:
					break;

			}
			rctext.Width = nTextWidth;

			bool bIsMirrored = GetIsMirrored();
			if( bIsMirrored )
			{
				int nLeftOffset = rctext.Left - rcbar.Left;
				rctext.X = rcbar.Right - (nLeftOffset + rctext.Width);
			}

			return rctext;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected void DrawIconStateHelper( Graphics gph, Icon icon, Rectangle rcicon, bool benabled, GroupBarItem item )
		{
			if( !benabled )
			{
				IntPtr hDC = gph.GetHdc();
				IntPtr hIcon;

				if( icon.Width != rcicon.Width )
				{
					hIcon = Syncfusion.Runtime.InteropServices.NativeMethods.CopyImage( icon.Handle, 1/*IMAGE_ICON*/,
						rcicon.Width, rcicon.Height, 0x0004/*LR_COPYRETURNORG*/);
				}
				else
				{
					hIcon = icon.Handle;
				}

				NativeMethods.DrawState( hDC, IntPtr.Zero, null, hIcon, IntPtr.Zero,
					rcicon.Left, rcicon.Top, rcicon.Width, rcicon.Height, 0x0003|0x0020 ); //DST_ICON|DSS_DISABLED
				NativeMethods.DestroyIcon( hIcon );

				gph.ReleaseHdc( hDC );
			}
			else
			{
				if( icon.Width == icon.Height )
				{
					using( Image drawImage = DrawIconHelper.GetIconToDraw( icon, item ) )
					{
						gph.DrawImage( drawImage, rcicon );
					}
				}
				else
				{
					IntPtr hDC = gph.GetHdc();
					IntPtr hIcon = Syncfusion.Runtime.InteropServices.NativeMethods.CopyImage( icon.Handle, 1/*IMAGE_ICON*/,
						rcicon.Width, rcicon.Height, 0x0004/*LR_COPYRETURNORG*/);

					NativeMethods.DrawState( hDC, IntPtr.Zero, null, hIcon, IntPtr.Zero,
						rcicon.Left, rcicon.Top, rcicon.Width, rcicon.Height, 0x0003 ); //DST_ICON
					NativeMethods.DestroyIcon( hIcon );

					gph.ReleaseHdc( hDC );
				}
			}
		}

		#endregion Icon Renderring

		#endregion

		[Syncfusion.Documentation.DocumentationExclude()]
		protected readonly StringFormat m_sfmtTextFormat = new StringFormat( StringFormat.GenericTypographic );
		[Syncfusion.Documentation.DocumentationExclude()]
		protected readonly StringFormat m_sfmtHeaderTextFormat = new StringFormat( StringFormat.GenericDefault );

		/// <summary>
		/// Returns the String format for text drawing.
		/// </summary>
		protected StringFormat TextStringFormat
		{
			get
			{
				StringFormat sfmt = new StringFormat(m_sfmtTextFormat);
				// LTR and RTL should use this flag for MeasureString to avoid manual width correction
				// by adding 1 pixel.
				sfmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

				if (GetIsMirrored())
				{
					sfmt.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
				}

				switch (this.nAlignment)
				{
					case TextAlignment.Left:
						sfmt.Alignment = StringAlignment.Near;
						break;

					case TextAlignment.Center:
						sfmt.Alignment = StringAlignment.Center;
						break;

					case TextAlignment.Right:
						sfmt.Alignment = StringAlignment.Far;
						break;
				}

				return sfmt;
			}
		}

		/// <summary>
		/// Returns the string format object for header drawing.
		/// </summary>
		protected StringFormat HeaderStringFormat
		{
			get
			{
				StringFormat result = new StringFormat(m_sfmtHeaderTextFormat);

				// Original typographic string format.
				using (StringFormat original = TextStringFormat)
				{
					// Copy all attributes from original format.
					result.Alignment = original.Alignment;
					result.FormatFlags = original.FormatFlags;
					result.HotkeyPrefix = original.HotkeyPrefix;
					result.LineAlignment = original.LineAlignment;
					result.Trimming = original.Trimming;
				}

				return result;
			}
		}

		private TextFormatFlags ItemTextFormat
		{
			get
			{
				TextFormatFlags format = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

				if (GetIsMirrored())
				{
					format |= TextFormatFlags.RightToLeft;
				}

				switch (this.nAlignment)
				{
					case TextAlignment.Left:
						format |= TextFormatFlags.Left;
						break;

					case TextAlignment.Center:
						format |= TextFormatFlags.HorizontalCenter;
						break;

					case TextAlignment.Right:
						format |= TextFormatFlags.Right;
						break;
				}

				return format;
			}
		}

		private TextFormatFlags HeaderTextFormat
		{
			get
			{
				return this.ItemTextFormat;
			}
		}

		/// <summary>
		/// Draws the <see cref="GroupBarItem"/> text.
		/// </summary>
		/// <remarks>
		/// The <see cref="GroupBar"/> control calls this method to paint the specified 
		/// GroupBarItem's text. Override this method to customize the GroupBar drawing.
		/// </remarks>
		/// <param name="gph">A <see cref="System.Drawing.Graphics"/> object.</param>
		/// <param name="nindex">The zero-based index of the <see cref="GroupBarItem"/> for which the text is drawn.</param>
		/// <param name="rcbar">A <see cref="System.Drawing.Rectangle"/> value specifying the GroupBarItem bounds.</param>
		protected virtual void DrawGroupBarText( Graphics gph, int nindex, Rectangle rcbar )
		{
			GroupBarItem item = this.activeItemsList[nindex] as GroupBarItem;
			if( (rcbar.Width <= 0) || (rcbar.Height <= 0) )
				return;

			bool bIsMirrored = GetIsMirrored();

			bool bPressed = ((nindex == this.nHighlightItem) && (this.eButtonsState == ButtonsState.GroupBarItemPushed))
				|| ((this.bStackedMode == true) && (nindex == this.nSelectedItem));

			if( bPressed && item.LargeImageMode )
			{
				rcbar.X += c_nPressedOffset;
				rcbar.Y += c_nPressedOffset;
				rcbar.Width -= c_nPressedGroupBarItemBorders;
				rcbar.Height -= c_nPressedGroupBarItemBorders;
			}
			else
			{
				rcbar.X += c_nOffset;
				rcbar.Y += c_nOffset;
				rcbar.Width -= c_nGroupBarItemBorders;
				rcbar.Height -= c_nGroupBarItemBorders;
			}

			Rectangle rc = rcbar;

			if( (bPressed == true) && 
				(this.vStyle != VisualStyle.Office2003 && this.vStyle != VisualStyle.Office2007) )
			{
				rc.Offset( bIsMirrored ? -1 : 1, 1 );
			}

			Rectangle rctext;
			rctext = CalcTextRect( gph, item, rc );

			if( bIsMirrored )
			{
				if( rctext.X < 0 )
					rctext.X = rc.X;
			}

			bool itemEnabled = item.Enabled && this.Enabled;
			
			if( itemEnabled )
			{
				Color clr = item.ForeColorInternal;

				if (clr.IsEmpty)
				{
					switch (this.vStyle)
					{
						case VisualStyle.Office2003:
							{
								if (this.bHighlight && (nindex == this.nHighlightItem) && ((nindex == this.nSelectedItem) || (this.eButtonsState == ButtonsState.GroupBarItemPushed)))
								{
									clr = Office2003Colors.GroupBarItemTextSelectedHighlightColor;
								}
								else
								{
									clr = Office2003Colors.GroupBarItemTextColor;
								}
							}
							break;
						case VisualStyle.Office2007:
							{
								if (nindex == this.nSelectedItem)
								{
									clr = Color.Black;
								}
								else
								{
									clr = m_office2007ColorTable.GroupBarItemTextColor;
								}
							}
							break;
                        case VisualStyle.Office2010:
                            {
                                if (nindex == this.nSelectedItem)
                                {
                                    clr = Color.Black;
                                }
                                else
                                {
                                    clr = m_office2010ColorTable.GroupBarItemTextColor;
                                }
                            }
                            break;
						default:
							clr = this.ForeColor;
							break;
					}
				}

				if (item.IsTextVisible)
				{
					TextRenderer.DrawText(gph, item.Text, item.Font, rctext, clr, this.ItemTextFormat);
				}
			}
			else
			{
				if (item.IsTextVisible)
				{
					RenderingHelper.DrawStringDisabled(gph, item.Text, item.Font, SystemColors.Control, rctext, this.ItemTextFormat);
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DrawScrollThumb( Graphics gfxTarget, bool bscrollup, Rectangle rectScroll )
		{
			ButtonState btnstate = ButtonState.Normal;
			if( bscrollup == true )
			{
				if( this.IsUpScrollButtonEnabled() == false )
					btnstate = ButtonState.Inactive;
				else if( this.eButtonsState == ButtonsState.ScrollUpPushed )
					btnstate = ButtonState.Pushed;
				this.rcScrollUp = rectScroll;
			}
			else
			{
				if( this.IsDownScrollButtonEnabled() == false )
					btnstate = ButtonState.Inactive;
				else if( this.eButtonsState == ButtonsState.ScrollDownPushed )
					btnstate = ButtonState.Pushed;
				this.rcScrollDown = rectScroll;
			}
			if( (this.activeItemsList[this.nSelectedItem] as GroupBarItem).Enabled == false )
				btnstate = ButtonState.Inactive;

			if( (this.tmrScrolling != null) && (this.tmrScrolling.Enabled == true) 
				&& (btnstate == ButtonState.Inactive) )
			{
				this.tmrScrolling.Stop();
				this.eInScroll = ButtonsState.None;
			}

			bool bIsMirrored = GetIsMirrored();

			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.UseThemedDrawing )
			{
				int abstate = 0;
				if( bscrollup == true )
				{
					if( this.eButtonsState == ButtonsState.ScrollUpPushed )
						abstate = ThemeStates.ABS_UPPRESSED;
					else if( this.eButtonsState == ButtonsState.ScrollUpHot )
						abstate = ThemeStates.ABS_UPHOT;
					else
						abstate = ThemeStates.ABS_UPNORMAL;
					if( btnstate == ButtonState.Inactive )
						abstate = ThemeStates.ABS_UPDISABLED;
				}
				else
				{
					if( this.eButtonsState == ButtonsState.ScrollDownPushed )
						abstate = ThemeStates.ABS_DOWNPRESSED;
					else if( this.eButtonsState == ButtonsState.ScrollDownHot )
						abstate = ThemeStates.ABS_DOWNHOT;
					else
						abstate = ThemeStates.ABS_DOWNNORMAL;
					if( btnstate == ButtonState.Inactive )
						abstate = ThemeStates.ABS_DOWNDISABLED;
				}
				Rectangle rcscrolltheme = new Rectangle( rectScroll.Left, rectScroll.Top+1,
					rectScroll.Width, rectScroll.Height-2 );
				this.tdScrollBar.DrawMirrored = bIsMirrored;
				this.tdScrollBar.DrawThemeBackground( gfxTarget, ThemeParts.SBP_ARROWBTN, abstate, rcscrolltheme );
			}
			else if( this.vStyle == VisualStyle.Office2007 )
			{
				using( Brush brush = GetBackgroundBrush( rectScroll, btnstate ) )
				{
					gfxTarget.FillRectangle( brush, rectScroll );
				}

				Point[] ptsdropdown;

				Point loc = new Point( rectScroll.X + rectScroll.Width / 2 - 4, rectScroll.Y + rectScroll.Height / 2 - 2 );
				Size size = new Size( 9, 5 );

				Rectangle rcArrow = new Rectangle( loc, size );

				if( bscrollup == true )
				{
					ptsdropdown = new Point[] { 
												new Point(rcArrow.Left + 4, rcArrow.Top),
												new Point(rcArrow.Left + 4, rcArrow.Top + 1),
												new Point(rcArrow.Left + 5, rcArrow.Top + 1),
												new Point(rcArrow.Left + 5, rcArrow.Top + 2),
												new Point(rcArrow.Left + 6, rcArrow.Top + 2),
												new Point(rcArrow.Left + 6, rcArrow.Top + 3),
												new Point(rcArrow.Left + 7, rcArrow.Top + 3),
												new Point(rcArrow.Left + 7, rcArrow.Top + 4),
												new Point(rcArrow.Left + 8, rcArrow.Top + 4),
												new Point(rcArrow.Left + 8, rcArrow.Top + 5),												
												new Point(rcArrow.Left + 9, rcArrow.Top + 5),
												new Point(rcArrow.Left, rcArrow.Top + 6),
												new Point(rcArrow.Left, rcArrow.Top + 5),
												new Point(rcArrow.Left + 1, rcArrow.Top + 5),
												new Point(rcArrow.Left + 1, rcArrow.Top + 4),
												new Point(rcArrow.Left + 2, rcArrow.Top + 4),
												new Point(rcArrow.Left + 2, rcArrow.Top + 3),
												new Point(rcArrow.Left + 3, rcArrow.Top + 3),
												new Point(rcArrow.Left + 3, rcArrow.Top + 2),
												new Point(rcArrow.Left + 4, rcArrow.Top + 2)
											};
				}
				else
				{
					ptsdropdown = new Point[] { 
												  new Point(rcArrow.Left + 9, rcArrow.Top),
												  new Point(rcArrow.Left + 9, rcArrow.Top + 1),
												  new Point(rcArrow.Left + 8, rcArrow.Top + 1),
												  new Point(rcArrow.Left + 8, rcArrow.Top + 2),
												  new Point(rcArrow.Left + 7, rcArrow.Top + 2),
												  new Point(rcArrow.Left + 7, rcArrow.Top + 3),
												  new Point(rcArrow.Left + 6, rcArrow.Top + 3),
												  new Point(rcArrow.Left + 6, rcArrow.Top + 4),
												  new Point(rcArrow.Left + 5, rcArrow.Top + 4),
												  new Point(rcArrow.Left + 5, rcArrow.Top + 5),
												  new Point(rcArrow.Left + 4, rcArrow.Top + 5),
												  new Point(rcArrow.Left + 4, rcArrow.Top + 4),
												  new Point(rcArrow.Left + 3, rcArrow.Top + 4),
												  new Point(rcArrow.Left + 3, rcArrow.Top + 3),
												  new Point(rcArrow.Left + 2, rcArrow.Top + 3),
												  new Point(rcArrow.Left + 2, rcArrow.Top + 2),
												  new Point(rcArrow.Left + 1, rcArrow.Top + 2),
												  new Point(rcArrow.Left + 1, rcArrow.Top + 1),
												  new Point(rcArrow.Left, rcArrow.Top)				  
											  };
				}

				GraphicsPath path = new GraphicsPath();
				path.AddLines( ptsdropdown );

				Brush fillBrush;

				if( btnstate == ButtonState.Inactive )
					fillBrush = new SolidBrush( SystemColors.ControlDark );
				else
					fillBrush = new LinearGradientBrush( rcArrow, Color.FromArgb( 107, 125, 165 ), Color.FromArgb( 66, 73, 99 ), LinearGradientMode.Vertical );
				using (Region region = new Region(path))
					gfxTarget.FillRegion(fillBrush, region);
				path.Dispose();
				using (Pen pen = new Pen(m_office2007ColorTable.GroupBarBorderColor))
				{
					if (bscrollup == true)
						gfxTarget.DrawLine(pen, new Point(rectScroll.Left, rectScroll.Top), new Point(rectScroll.Right, rectScroll.Top));
					else
						gfxTarget.DrawLine(pen , new Point(rectScroll.Left, rectScroll.Bottom), new Point(rectScroll.Right, rectScroll.Bottom));
				}
				fillBrush.Dispose();
			}
			else
			{
				ScrollButton scrlbtn = (bscrollup == true) ? ScrollButton.Up : ScrollButton.Down;

				using( CMirroredDrawer mdDrawer = new CMirroredDrawer( gfxTarget, rectScroll, bIsMirrored ) )
				{
					Graphics gfxVirt = mdDrawer.VirtualGfx;
					Rectangle rectVirt = mdDrawer.VirtualBounds;

					ControlPaint.DrawScrollButton( gfxVirt, rectVirt, scrlbtn, btnstate|(this.bFlatLook ? ButtonState.Flat : 0) );

					Color clrright = SystemColors.Control;
					if( this.bdrStyle == BorderStyle.Fixed3D )
						clrright = SystemColors.ControlLightLight;
					else if( this.bdrStyle == BorderStyle.FixedSingle )
						clrright = SystemColors.ControlDark;

					GroupBarItem item = this.activeItemsList[this.nSelectedItem] as GroupBarItem;
					Color clrbottom = SystemColors.Control;

					if( bscrollup )
					{
						if( (this.bdrStyle != BorderStyle.None) && (this.nSelectedItem == 0) )
						{
							ControlPaint.DrawBorder( gfxVirt, rectVirt,
								Color.Empty, 0, ButtonBorderStyle.None,
								SystemColors.ControlDark, 1, ButtonBorderStyle.Solid,
								clrright, 1, ButtonBorderStyle.Solid,
								item.ClientBorderColors.Top, 1, ButtonBorderStyle.Solid );
						}
						else
						{
							ControlPaint.DrawBorder( gfxVirt, rectVirt,
								Color.Empty, 0, ButtonBorderStyle.None,
								Color.Empty, 0, ButtonBorderStyle.None,
								clrright, 1, ButtonBorderStyle.Solid,
								item.ClientBorderColors.Top, 1, ButtonBorderStyle.Solid );
						}
					}
					else	// bscrolldown
					{
						if( (this.nSelectedItem+1 == this.activeItemsList.Count) || (this.nSelectedItem+1 == this.activeItemsList.Count-1) )
						{
							// The bottom scroll button.
							if( this.bdrStyle == BorderStyle.Fixed3D )
								clrbottom = SystemColors.ControlLightLight;
							else if( this.bdrStyle == BorderStyle.FixedSingle )
								clrbottom = SystemColors.ControlDark;

							ControlPaint.DrawBorder( gfxVirt, rectVirt,
								Color.Empty, 0, ButtonBorderStyle.None,
								item.ClientBorderColors.Bottom, 1, ButtonBorderStyle.Solid,
								clrright, 1, ButtonBorderStyle.Solid,
								clrbottom, 1, ButtonBorderStyle.Solid );
						}
						else
						{
							ControlPaint.DrawBorder( gfxVirt, rectVirt,
								Color.Empty, 0, ButtonBorderStyle.None,
								item.ClientBorderColors.Bottom, 1, ButtonBorderStyle.Solid,
								clrright, 1, ButtonBorderStyle.Solid,
								clrbottom, 1, ButtonBorderStyle.Solid );
						}
					}
				}
			}
		}

		private Brush GetBackgroundBrush( Rectangle rect, ButtonState state )
		{
			LinearGradientBrush lgb = null;
			Color color1 = Color.Empty, color2 = Color.Empty;

			if( this.vStyle == VisualStyle.Office2007 )
			{
				Blend blend = new Blend();

				if( state != ButtonState.Inactive && (this.eButtonsState == ButtonsState.ScrollDownHot ||
					this.eButtonsState == ButtonsState.ScrollUpHot) )
				{
					color1 = m_office2007ColorTable.GroupBarHighlightColorLight;
					color2 = m_office2007ColorTable.GroupBarHighlightColorDark;
				}
				else if( state == ButtonState.Pushed )
				{
					color1 = m_office2007ColorTable.GroupBarSelectedColorLight;
					color2 = m_office2007ColorTable.GroupBarSelectedColorDark;
				}
				else if( state == ButtonState.Normal )
				{
					color1 = m_office2007ColorTable.GroupBarItemColorLight;
					color2 = m_office2007ColorTable.GroupBarItemColorDark;
				}
				else if( state == ButtonState.Inactive )
				{
					color1 = Color.FromArgb( 236, 233, 216 );
					color2 = color1;
				}

				blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };

				blend.Factors = new float[] { 0.0F, 0.5F, 1.0F, 0.5F };

				lgb = new LinearGradientBrush( rect, color1, color2, LinearGradientMode.Vertical );
				lgb.Blend = blend;
			}
            else if (this.vStyle == VisualStyle.Office2010)
            {
                Blend blend = new Blend();

                if (state != ButtonState.Inactive && (this.eButtonsState == ButtonsState.ScrollDownHot ||
                    this.eButtonsState == ButtonsState.ScrollUpHot))
                {
                    color1 = m_office2010ColorTable.GroupBarHighlightColorLight;
                    color2 = m_office2010ColorTable.GroupBarHighlightColorDark;
                }
                else if (state == ButtonState.Pushed)
                {
                    color1 = m_office2010ColorTable.GroupBarSelectedColorLight;
                    color2 = m_office2010ColorTable.GroupBarSelectedColorDark;
                }
                else if (state == ButtonState.Normal)
                {
                    color1 = m_office2010ColorTable.GroupBarItemColorLight;
                    color2 = m_office2010ColorTable.GroupBarItemColorDark;
                }
                else if (state == ButtonState.Inactive)
                {
                    color1 = Color.FromArgb(236, 233, 216);
                    color2 = color1;
                }

                blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };

                blend.Factors = new float[] { 0.0F, 0.5F, 1.0F, 0.5F };

                lgb = new LinearGradientBrush(rect, color1, color2, LinearGradientMode.Vertical);
                lgb.Blend = blend;
            }

			return lgb;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DrawNavigationPane( Graphics gph )
		{
			Rectangle rcbounds = this.GetBoundedRectangle();

			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.UseThemedDrawing )
			{
				DrawNavigationPaneThemed( gph, rcbounds );
			}
			else if( this.vStyle == VisualStyle.Office2003 )
			{
				DrawNavigationPaneOffice2003( gph, rcbounds );
			}
			else if( this.vStyle == VisualStyle.Office2007 )
			{
				DrawNavigationPaneOffice2007( gph, rcbounds );
			}
            else if (this.vStyle == VisualStyle.Office2010)
            {
                DrawNavigationPaneOffice2010(gph, rcbounds);
            }
			else	// OfficeXP style
			{
				DrawNavigationPaneOfficeXP( gph, rcbounds );
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DrawNavigationPaneThemed( Graphics gph, Rectangle rcbounds )
		{
			Rectangle rcddbutton = GetDropDownButtonRectangle();

			bool bIsMirrored = GetIsMirrored();

			// WORKAROUND: theme backgrounds contain transparent pixels which lose alpha-channel 
			// info after bitmap drawing.
			// Mirroring is turned off due to RTL theme background is almost identical to LTR.
			this.tdButton.DrawMirrored = false;	//bIsMirrored;

			// Draw the drop-down button	if needed.
			if( this.ShowChevron )
			{
				if( this.eButtonsState == ButtonsState.DropDownButtonHot )
				{
					this.tdButton.DrawThemeBackground( gph, ThemeParts.BP_PUSHBUTTON, ThemeStates.PBS_HOT, rcddbutton );
					this.DrawDropdownButton( gph, rcddbutton, bIsMirrored );
				}
				else if( this.eButtonsState == ButtonsState.DropDownButtonPushed )
				{
					this.tdButton.DrawThemeBackground( gph, ThemeParts.BP_PUSHBUTTON, ThemeStates.PBS_PRESSED, rcddbutton );
					rcddbutton.Offset( bIsMirrored ? -1 : 1, 1 );
					this.DrawDropdownButton( gph, rcddbutton, bIsMirrored );
					rcddbutton.Offset( bIsMirrored ? 1 : -1, -1 );
				}
				else
				{
					this.tdButton.DrawThemeBackground( gph, ThemeParts.BP_PUSHBUTTON, ThemeStates.PBS_NORMAL, rcddbutton );
					this.DrawDropdownButton( gph, rcddbutton, bIsMirrored );
				}
			}

			// Draw the navigation pane GroupBarItems.
			Rectangle rcbutton = rcddbutton;
			rcbutton.Width = this.nNavigationButtonWidth;
			if( bIsMirrored )
			{
				// Offset to the right for width of drop-down button and n-1 navbar button widths.
				int shiftStart = (this.ShowChevron) ? rcddbutton.Width : 0;
				rcbutton.X += shiftStart + this.nNavigationButtonWidth * (this.alNavPaneItems.Count - 1);
			}
			else
			{
				// Offset to the left for witdh of drop-down button and n-1 nav bar button widths.
				int startPoint = (this.ShowChevron) ? rcbutton.X : rcbounds.Right;
				rcbutton.X = startPoint - this.nNavigationButtonWidth * this.alNavPaneItems.Count;
			}

			if( !this.Collapsed )
			{
				foreach( GroupBarItem item in this.alNavPaneItems )
				{
					if( ((item.LargeImageMode && (item.NavigationPaneImage != null || item.Image != null)))
					|| ((!item.LargeImageMode && (item.NavigationPaneIcon != null || item.Icon != null))) )
					{
						Rectangle rcicon = new Rectangle(
							rcbutton.Left+(rcbutton.Width - c_nIconSize)/2, rcbutton.Top+((rcbutton.Height - c_nIconSize)/2),
							c_nIconSize, c_nIconSize );
						int itemindex = this.activeItemsList.IndexOf( item );

						if( (this.bHighlight == true) && (itemindex == this.nHighlightItem) )
							this.tdButton.DrawThemeBackground( gph, ThemeParts.BP_PUSHBUTTON, ThemeStates.PBS_HOT, rcbutton );
						else if( itemindex == this.nSelectedItem )
							this.tdButton.DrawThemeBackground( gph, ThemeParts.BP_PUSHBUTTON, ThemeStates.PBS_PRESSED, rcbutton );
						else
							this.tdButton.DrawThemeBackground( gph, ThemeParts.BP_PUSHBUTTON, ThemeStates.PBS_NORMAL, rcbutton );

						this.DrawNevigationItemIcon( gph, item, rcbutton );
					}

					// Shift next button bounds...
					rcbutton.X += bIsMirrored ? 
					// to the left for RTL
					-this.nNavigationButtonWidth : 
					// to the right for LTR
					this.nNavigationButtonWidth;
				}
			}
		}

		/// <summary>
		/// Draws icon\image in navigation panel.
		/// </summary>
		/// <param name="gph"></param>
		/// <param name="item"></param>
		/// <param name="rcbutton"></param>		
		protected virtual void DrawNevigationItemIcon( Graphics gph, GroupBarItem item, Rectangle rcbutton )
		{
			Rectangle rc = new Rectangle(
				rcbutton.Left+(rcbutton.Width-c_nIconSize)/2,
				rcbutton.Top+(rcbutton.Height-c_nIconSize)/2, c_nIconSize, c_nIconSize );

			if( item.LargeImageMode )
			{
				Image image = null;
				if( item.NavigationPaneImage != null )
				{
					image = item.NavigationPaneImage;
				}
				else
				{
					image = item.Image;
				}
				if( image != null )
				{
					RectangleF srcRect = new RectangleF( 0, 0, image.Width, image.Height );
					this.DrawImageStateHelper( gph, image, rc, srcRect, item.Enabled );
				}
			}
			else
			{
				Icon icon = null;
				if( item.NavigationPaneIcon != null )
				{
					icon = item.NavigationPaneIcon;
				}
				else
				{
					icon = item.Icon;
				}
				if( icon != null )
				{
					if( m_IconRenderingMode == IconRenderingMode.AlphaBlended )
					{
						DrawIconStateHelper( gph, icon, rc, item.Enabled, item );
					}
					else if( m_IconRenderingMode == IconRenderingMode.Default )
					{
                        using (Bitmap bMap = icon.ToBitmap())
                        {
                            DrawImageStateHelper(gph, bMap, rc, new RectangleF(0, 0, bMap.Width, bMap.Height), item.Enabled);
                        }
					}
				}
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DrawNavigationPaneOffice2003( Graphics gph, Rectangle rcbounds )
		{
			Rectangle rcpane = new Rectangle( rcbounds.Left, rcbounds.Bottom - this.nNavigationPaneHeight, rcbounds.Width, this.nNavigationPaneHeight );

			bool bIsMirrored = GetIsMirrored();

            if (rcpane.Width > 0 && rcpane.Height > 0)
            {
                // Fill the navigation pane with the gradient colors.
                LinearGradientBrush panegradient = new LinearGradientBrush(rcpane, Office2003Colors.MenuMarginColorLight,
                    Office2003Colors.MenuMarginColorDark, LinearGradientMode.Vertical);
                gph.FillRectangle(panegradient, rcpane);
                panegradient.Dispose();
            }

			Rectangle rcddbutton = GetDropDownButtonRectangle();

			// Draw chevron button if needed.
			if( this.ShowChevron )
			{
				// Fill button rectangle with the suitable gradient colors.
				if( this.eButtonsState == ButtonsState.DropDownButtonHot )
				{
					LinearGradientBrush lgb = new LinearGradientBrush( rcddbutton, Office2003Colors.GroupBarHighlightColorLight,
						Office2003Colors.GroupBarHighlightColorDark, LinearGradientMode.Vertical );
					gph.FillRectangle( lgb, rcddbutton );
					lgb.Dispose();
				}
				else if( this.eButtonsState == ButtonsState.DropDownButtonPushed )
				{
					LinearGradientBrush lgb = new LinearGradientBrush( rcddbutton, Office2003Colors.GroupBarSelectedHighlightColorDark,
						Office2003Colors.GroupBarSelectedHighlightColorLight, LinearGradientMode.Vertical );
					gph.FillRectangle( lgb, rcddbutton );
					lgb.Dispose();
				}

				this.DrawDropdownButton( gph, rcddbutton, bIsMirrored );
			}

			// Draw the navigation pane GroupBarItems.			
			Rectangle rcbutton = rcddbutton;
			rcbutton.Width = this.nNavigationButtonWidth;
			if( bIsMirrored )
			{
				// Offset to the right for width of drop-down button and n-1 nav bar button widths.
				int shiftStart = (this.ShowChevron) ? rcddbutton.Width : 0;
				rcbutton.X += shiftStart + this.nNavigationButtonWidth * (this.alNavPaneItems.Count - 1);
			}
			else
			{
				// Offset to the left for witdh of drop-down button and n-1 nav bar button widths.
				int startPoint = (this.ShowChevron) ? rcbutton.X : rcbounds.Right;
				rcbutton.X = startPoint - this.nNavigationButtonWidth * this.alNavPaneItems.Count;
			}

			if( !this.Collapsed )
			{
				foreach( GroupBarItem item in this.alNavPaneItems )
				{
					int itemindex = this.activeItemsList.IndexOf( item );
					if( itemindex == this.nSelectedItem )
					{
						LinearGradientBrush lgb = null;
						if( (this.bHighlight == true) && (itemindex == this.nHighlightItem) )
						{
							lgb = new LinearGradientBrush( rcbutton, Office2003Colors.GroupBarSelectedHighlightColorDark,
								Office2003Colors.GroupBarSelectedHighlightColorLight, LinearGradientMode.Vertical );
						}
						else
						{
							lgb = new LinearGradientBrush( rcbutton, Office2003Colors.GroupBarSelectedColorLight,
								Office2003Colors.GroupBarSelectedColorDark, LinearGradientMode.Vertical );
						}
						gph.FillRectangle( lgb, rcbutton );
						lgb.Dispose();
					}
					else if( (this.bHighlight == true) && (itemindex == this.nHighlightItem) )
					{
						LinearGradientBrush lgb = null;
						if( this.eButtonsState == ButtonsState.GroupBarItemPushed )
						{
							lgb = new LinearGradientBrush( rcbutton, Office2003Colors.GroupBarSelectedHighlightColorDark,
								Office2003Colors.GroupBarSelectedHighlightColorLight, LinearGradientMode.Vertical );
						}
						else
						{
							lgb = new LinearGradientBrush( rcbutton, Office2003Colors.GroupBarHighlightColorLight,
								Office2003Colors.GroupBarHighlightColorDark, LinearGradientMode.Vertical );
						}
						gph.FillRectangle( lgb, rcbutton );
						lgb.Dispose();
					}

					// Draw icon.
					DrawNevigationItemIcon( gph, item, rcbutton );


					// Shift next button bounds...
					rcbutton.X += bIsMirrored ? 
					// to the left for RTL.
					-this.nNavigationButtonWidth : 
					// to the right for LTR.
					this.nNavigationButtonWidth;
				}
			}

			// Draw a border along the top edge of the pane.
			Pen borderpen = new Pen( Office2003Colors.CommandBarDropDownColorDark, 1 );
			gph.DrawLine( borderpen, new Point( rcpane.Left, rcpane.Top ), new Point( rcpane.Right-1, rcpane.Top ) );
			borderpen.Dispose();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DrawNavigationPaneOffice2007( Graphics gph, Rectangle rcbounds )
		{
			Rectangle rcpane = new Rectangle( rcbounds.Left, rcbounds.Bottom - this.nNavigationPaneHeight, rcbounds.Width, this.nNavigationPaneHeight );

			bool bIsMirrored = GetIsMirrored();

            if (rcpane.Width > 0 && rcpane.Height > 0)
            {
                // Fill the navigation pane with the gradient colors.
                LinearGradientBrush panegradient = new LinearGradientBrush(rcpane, m_office2007ColorTable.GroupBarItemColorLight,
                    m_office2007ColorTable.GroupBarItemColorDark, LinearGradientMode.Vertical);

                Blend blend = new Blend();
                blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };
                blend.Factors = new float[] { 0.0F, 0.6F, 1.0F, 0.6F };

                panegradient.Blend = blend;

                gph.FillRectangle(panegradient, rcpane);
                panegradient.Dispose();

                Rectangle rcddbutton = GetDropDownButtonRectangle();

                // Draw chevron button if needed.
                if (this.ShowChevron)
                {
                    // Fill button rectangle with the suitable gradient colors.
                    if (this.eButtonsState == ButtonsState.DropDownButtonHot)
                    {
                        LinearGradientBrush lgb = new LinearGradientBrush(rcddbutton, m_office2007ColorTable.GroupBarHighlightColorLight,
                            m_office2007ColorTable.GroupBarHighlightColorDark, LinearGradientMode.Vertical);

                        blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };
                        blend.Factors = new float[] { 0.0F, 0.6F, 1.0F, 0.6F };

                        lgb.Blend = blend;

                        gph.FillRectangle(lgb, rcddbutton);
                        lgb.Dispose();
                    }
                    else if (this.eButtonsState == ButtonsState.DropDownButtonPushed)
                    {
                        LinearGradientBrush lgb = new LinearGradientBrush(rcddbutton, m_office2007ColorTable.GroupBarSelectedHighlightColorLight,
                            m_office2007ColorTable.GroupBarSelectedHighlightColorDark, LinearGradientMode.Vertical);

                        blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };
                        blend.Factors = new float[] { 0.2F, 0.6F, 1.0F, 0.0F };

                        lgb.Blend = blend;

                        gph.FillRectangle(lgb, rcddbutton);
                        lgb.Dispose();
                    }

                    this.DrawDropdownButton(gph, rcddbutton, bIsMirrored);
                }

                // Draw the navigation pane GroupBarItems.			
                Rectangle rcbutton = rcddbutton;
                rcbutton.Width = this.nNavigationButtonWidth;
                if (bIsMirrored)
                {
                    // Offset to the right for width of drop-down button and n-1 nav bar button widths.
                    int shiftStart = (this.ShowChevron) ? rcddbutton.Width : 0;
                    rcbutton.X += shiftStart + this.nNavigationButtonWidth * (this.alNavPaneItems.Count - 1);
                }
                else
                {
                    // Offset to the left for witdh of drop-down button and n-1 nav bar button widths.
                    int startPoint = (this.ShowChevron) ? rcbutton.X : rcbounds.Right;
                    rcbutton.X = startPoint - this.nNavigationButtonWidth * this.alNavPaneItems.Count;
                }

                if (!this.Collapsed)
                {
                    foreach (GroupBarItem item in this.alNavPaneItems)
                    {
                        int itemindex = this.activeItemsList.IndexOf(item);
                        if (itemindex == this.nSelectedItem)
                        {
                            LinearGradientBrush lgb = null;
                            if ((this.bHighlight == true) && (itemindex == this.nHighlightItem))
                            {
                                lgb = new LinearGradientBrush(rcbutton, m_office2007ColorTable.GroupBarSelectedHighlightColorLight,
                                    m_office2007ColorTable.GroupBarSelectedHighlightColorDark, LinearGradientMode.Vertical);
                            }
                            else
                            {
                                lgb = new LinearGradientBrush(rcbutton, m_office2007ColorTable.GroupBarSelectedColorLight,
                                    m_office2007ColorTable.GroupBarSelectedColorDark, LinearGradientMode.Vertical);
                            }

                            blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };
                            blend.Factors = new float[] { 0.2F, 0.6F, 1.0F, 0.0F };

                            lgb.Blend = blend;

                            gph.FillRectangle(lgb, rcbutton);
                            lgb.Dispose();
                        }
                        else if ((this.bHighlight == true) && (itemindex == this.nHighlightItem))
                        {
                            LinearGradientBrush lgb = null;
                            if (this.eButtonsState == ButtonsState.GroupBarItemPushed)
                            {
                                lgb = new LinearGradientBrush(rcbutton, m_office2007ColorTable.GroupBarSelectedHighlightColorLight,
                                    m_office2007ColorTable.GroupBarSelectedHighlightColorDark, LinearGradientMode.Vertical);

                                blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };
                                blend.Factors = new float[] { 0.2F, 0.6F, 1.0F, 0.0F };
                            }
                            else
                            {
                                lgb = new LinearGradientBrush(rcbutton, m_office2007ColorTable.GroupBarHighlightColorLight,
                                    m_office2007ColorTable.GroupBarHighlightColorDark, LinearGradientMode.Vertical);

                                blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };
                                blend.Factors = new float[] { 0.0F, 0.6F, 1.0F, 0.6F };
                            }
                            lgb.Blend = blend;

                            gph.FillRectangle(lgb, rcbutton);
                            lgb.Dispose();
                        }

                        // Draw icon.
                        DrawNevigationItemIcon(gph, item, rcbutton);

                        // Shift next button bounds...
                        rcbutton.X += bIsMirrored ?
                            // to the left for RTL.
                        -this.nNavigationButtonWidth :
                            // to the right for LTR.
                        this.nNavigationButtonWidth;
                    }
                }

                // Draw a border along the top edge of the pane.
                Pen borderpen = new Pen(m_office2007ColorTable.GroupBarBorderColor, 1);
                gph.DrawLine(borderpen, new Point(rcpane.Left, rcpane.Top), new Point(rcpane.Right - 1, rcpane.Top));
                borderpen.Dispose();
            }
		}

        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual void DrawNavigationPaneOffice2010(Graphics gph, Rectangle rcbounds)
        {
            Rectangle rcpane = new Rectangle(rcbounds.Left, rcbounds.Bottom - this.nNavigationPaneHeight, rcbounds.Width, this.nNavigationPaneHeight);

            bool bIsMirrored = GetIsMirrored();

            if (rcpane.Width > 0 && rcpane.Height > 0)
            {
                // Fill the navigation pane with the gradient colors.
                LinearGradientBrush panegradient = new LinearGradientBrush(rcpane, m_office2010ColorTable.GroupBarItemColorLight,
                    m_office2010ColorTable.GroupBarItemColorDark, LinearGradientMode.Vertical);

                Blend blend = new Blend();
                blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };
                blend.Factors = new float[] { 0.0F, 0.6F, 1.0F, 0.6F };

                panegradient.Blend = blend;

                gph.FillRectangle(panegradient, rcpane);
                panegradient.Dispose();

                Rectangle rcddbutton = GetDropDownButtonRectangle();

                // Draw chevron button if needed.
                if (this.ShowChevron)
                {
                    // Fill button rectangle with the suitable gradient colors.
                    if (this.eButtonsState == ButtonsState.DropDownButtonHot)
                    {
                        LinearGradientBrush lgb = new LinearGradientBrush(rcddbutton, m_office2010ColorTable.GroupBarHighlightColorLight,
                            m_office2010ColorTable.GroupBarHighlightColorDark, LinearGradientMode.Vertical);

                        blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };
                        blend.Factors = new float[] { 0.0F, 0.6F, 1.0F, 0.6F };

                        lgb.Blend = blend;

                        gph.FillRectangle(lgb, rcddbutton);
                        lgb.Dispose();
                    }
                    else if (this.eButtonsState == ButtonsState.DropDownButtonPushed)
                    {
                        LinearGradientBrush lgb = new LinearGradientBrush(rcddbutton, m_office2010ColorTable.GroupBarSelectedHighlightColorLight,
                            m_office2010ColorTable.GroupBarSelectedHighlightColorDark, LinearGradientMode.Vertical);

                        blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };
                        blend.Factors = new float[] { 0.2F, 0.6F, 1.0F, 0.0F };

                        lgb.Blend = blend;

                        gph.FillRectangle(lgb, rcddbutton);
                        lgb.Dispose();
                    }

                    this.DrawDropdownButton(gph, rcddbutton, bIsMirrored);
                }

                // Draw the navigation pane GroupBarItems.			
                Rectangle rcbutton = rcddbutton;
                rcbutton.Width = this.nNavigationButtonWidth;
                if (bIsMirrored)
                {
                    // Offset to the right for width of drop-down button and n-1 nav bar button widths.
                    int shiftStart = (this.ShowChevron) ? rcddbutton.Width : 0;
                    rcbutton.X += shiftStart + this.nNavigationButtonWidth * (this.alNavPaneItems.Count - 1);
                }
                else
                {
                    // Offset to the left for witdh of drop-down button and n-1 nav bar button widths.
                    int startPoint = (this.ShowChevron) ? rcbutton.X : rcbounds.Right;
                    rcbutton.X = startPoint - this.nNavigationButtonWidth * this.alNavPaneItems.Count;
                }

                if (!this.Collapsed)
                {
                    foreach (GroupBarItem item in this.alNavPaneItems)
                    {
                        int itemindex = this.activeItemsList.IndexOf(item);
                        if (itemindex == this.nSelectedItem)
                        {
                            LinearGradientBrush lgb = null;
                            if ((this.bHighlight == true) && (itemindex == this.nHighlightItem))
                            {
                                lgb = new LinearGradientBrush(rcbutton, m_office2010ColorTable.GroupBarSelectedHighlightColorLight,
                                    m_office2010ColorTable.GroupBarSelectedHighlightColorDark, LinearGradientMode.Vertical);
                            }
                            else
                            {
                                lgb = new LinearGradientBrush(rcbutton, m_office2010ColorTable.GroupBarSelectedColorLight,
                                    m_office2010ColorTable.GroupBarSelectedColorDark, LinearGradientMode.Vertical);
                            }

                            blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };
                            blend.Factors = new float[] { 0.2F, 0.6F, 1.0F, 0.0F };

                            lgb.Blend = blend;

                            gph.FillRectangle(lgb, rcbutton);
                            lgb.Dispose();
                        }
                        else if ((this.bHighlight == true) && (itemindex == this.nHighlightItem))
                        {
                            LinearGradientBrush lgb = null;
                            if (this.eButtonsState == ButtonsState.GroupBarItemPushed)
                            {
                                lgb = new LinearGradientBrush(rcbutton, m_office2010ColorTable.GroupBarSelectedHighlightColorLight,
                                    m_office2010ColorTable.GroupBarSelectedHighlightColorDark, LinearGradientMode.Vertical);

                                blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };
                                blend.Factors = new float[] { 0.2F, 0.6F, 1.0F, 0.0F };
                            }
                            else
                            {
                                lgb = new LinearGradientBrush(rcbutton, m_office2010ColorTable.GroupBarHighlightColorLight,
                                    m_office2010ColorTable.GroupBarHighlightColorDark, LinearGradientMode.Vertical);

                                blend.Positions = new float[] { 0.0F, 0.4F, 0.4F + 0.001F, 1.0F };
                                blend.Factors = new float[] { 0.0F, 0.6F, 1.0F, 0.6F };
                            }
                            lgb.Blend = blend;

                            gph.FillRectangle(lgb, rcbutton);
                            lgb.Dispose();
                        }

                        // Draw icon.
                        DrawNevigationItemIcon(gph, item, rcbutton);

                        // Shift next button bounds...
                        rcbutton.X += bIsMirrored ?
                            // to the left for RTL.
                        -this.nNavigationButtonWidth :
                            // to the right for LTR.
                        this.nNavigationButtonWidth;
                    }
                }
               
            }
        }
		[Syncfusion.Documentation.DocumentationExclude()]
		protected virtual void DrawNavigationPaneOfficeXP( Graphics gph, Rectangle rcbounds )
		{
			Rectangle rcddbutton = GetDropDownButtonRectangle();

			ButtonState bsButtonState = (ButtonsState.DropDownButtonPushed == this.eButtonsState ?
				ButtonState.Pushed : 
				ButtonState.Normal);

			bool bIsMirrored = GetIsMirrored();

			// Draw Chevron if needed.
			if( this.ShowChevron )
			{
				using( CMirroredDrawer mdButton = new CMirroredDrawer( gph, rcddbutton, bIsMirrored ) )
				{
					Graphics gfxVirt = mdButton.VirtualGfx;
					Rectangle rectVirt = mdButton.VirtualBounds;

                    if (rectVirt.Width > 0 && rectVirt.Height > 0)
                    {
                        if (this.VisualStyle == VisualStyle.Metro)
                        {
                            SolidBrush brush = new SolidBrush(Color.White);
                            gph.FillRectangle(brush, rectVirt);
                            brush.Dispose();
                        }
                        else
                        ControlPaint.DrawButton(gfxVirt, rectVirt, bsButtonState | (this.bFlatLook ? ButtonState.Flat : 0));

                        // If the drop-down button is being highlighted or is pushed, then draw the highlight state.
                        if ((this.eButtonsState == ButtonsState.DropDownButtonHot) || (this.eButtonsState == ButtonsState.DropDownButtonPushed))
                        {
                            Rectangle rchighlight = new Rectangle(rectVirt.Left, rectVirt.Top, rectVirt.Width - 1, rectVirt.Height - 1);
                            if (this.VisualStyle == VisualStyle.Metro)
                            {
                                SolidBrush brush = new SolidBrush(this.HeaderBackColor);
                                gph.FillRectangle(brush, rchighlight);
                                brush.Dispose();
                            }
                            else
                            ControlPaint.DrawBorder3D(gfxVirt, rchighlight, Border3DStyle.Raised, Border3DSide.Bottom | Border3DSide.Right);
                        }
                    }
				}

				// Draw the drop-down button.
				if( this.eButtonsState == ButtonsState.DropDownButtonPushed && this.VisualStyle != VisualStyle.Metro )
				{
					rcddbutton.Offset( bIsMirrored ? -1 : 1, 1 );
					this.DrawDropdownButton( gph, rcddbutton, bIsMirrored );
					rcddbutton.Offset( bIsMirrored ? 1 : -1, -1 );
				}
				else
				{
					this.DrawDropdownButton( gph, rcddbutton, bIsMirrored );
				}
			}

			// Draw the navigation pane GroupBarItems.			
			Rectangle rcbutton = rcddbutton;
			rcbutton.Width = this.nNavigationButtonWidth;
			if( bIsMirrored )
			{
				// Offset to the right for width of drop-down button and n-1 nav bar button widths.
				int shiftStart = (this.ShowChevron) ? rcddbutton.Width : 0;
				rcbutton.X += shiftStart + this.nNavigationButtonWidth * (this.alNavPaneItems.Count - 1);
			}
			else
			{
				// Offset to the left for width of drop-down button and n-1 nav bar button widths.
				int startPoint = (this.ShowChevron) ? rcbutton.X : rcbounds.Right;
				rcbutton.X = startPoint - this.nNavigationButtonWidth * this.alNavPaneItems.Count;
			}

            if ((this.bBarHighlight == true) && (this.bHighlight == true) && (this.nHighlightItem >= 0))
            {
                if (this.alNavPaneItems.Contains(this.activeItemsList[this.nHighlightItem]))
                {
                    Rectangle rcitem = this.GetGroupBarItemBounds(this.nHighlightItem);
                    rcitem.Width -= 1;
                    rcitem.Height -= 1;
                    if (bIsMirrored)
                    {
                        rcitem.X += 1;
                    }

                    using (CMirroredDrawer mdItem = new CMirroredDrawer(gph, rcitem, bIsMirrored))
                    {
                        if (this.VisualStyle == VisualStyle.Metro)
                        {
                            SolidBrush brush = new SolidBrush(this.HeaderBackColor);
                            gph.FillRectangle(brush, mdItem.VirtualBounds);
                            brush.Dispose();
                        }
                        else
                        {
                            ControlPaint.DrawBorder3D(mdItem.VirtualGfx, mdItem.VirtualBounds,
                            Border3DStyle.Raised, Border3DSide.Bottom | Border3DSide.Right);
                        }
                    }
                }
            }
			if( !this.Collapsed )
			{ 
				foreach( GroupBarItem item in this.alNavPaneItems )
				{
					if( ((item.LargeImageMode && (item.NavigationPaneImage != null || item.Image != null)))
					|| ((!item.LargeImageMode && (item.NavigationPaneIcon != null || item.Icon != null))) )
					{
						int itemindex = this.activeItemsList.IndexOf( item );
						bool bSelectedItem = ((this.nHighlightItem == itemindex && this.eButtonsState == ButtonsState.GroupBarItemPushed)
						|| this.nSelectedItem == itemindex);

						ButtonState bsButton = bSelectedItem ? ButtonState.Pushed : ButtonState.Normal;
						if( this.bFlatLook == true )
							bsButton |= ButtonState.Flat;
						if( item.Enabled == false )
							bsButton |= ButtonState.Inactive;

						using( CMirroredDrawer mdItem = new CMirroredDrawer( gph, rcbutton, bIsMirrored ) )
						{
							if (this.VisualStyle != VisualStyle.Metro) 
							ControlPaint.DrawButton( mdItem.VirtualGfx, mdItem.VirtualBounds, bsButton );
						}
						this.DrawNevigationItemIcon( gph, item, rcbutton );
					}

					// Shift next button bounds...
					rcbutton.X += bIsMirrored ? 
					// to the left for RTL.
					-this.nNavigationButtonWidth : 
					// to the right for LTR.
					this.nNavigationButtonWidth;
				}
			}

		}

		/// <summary>
		/// Draws the drop down button.
		/// </summary>
		/// <param name="gph">A <see cref="System.Drawing.Graphics"/> object.</param>
		/// <param name="rcbutton">A <see cref="System.Drawing.Rectangle"/> value specifying the GroupBarItem bounds.</param>
		/// <param name="bIsMirrored">If set to <c>true</c> button is mirrored for RTL.</param>
		protected virtual void DrawDropdownButton( Graphics gph, Rectangle rcbutton, bool bIsMirrored )
		{
			if( !this.ShowChevron ) return;

			using( CMirroredDrawer mdMirrorDraw = new CMirroredDrawer( gph, rcbutton, bIsMirrored ) )
			{
				Graphics gphVirt = mdMirrorDraw.VirtualGfx;
				Rectangle rectVirt = mdMirrorDraw.VirtualBounds;
                if (EnableTouchMode)
                {
                    rectVirt.X = mdMirrorDraw.VirtualBounds.X;// -(mdMirrorDraw.VirtualBounds.Width);
                    //rectVirt.Width = mdMirrorDraw.VirtualBounds.Width * (int)ScaleFactor;
                }
                if (this.vStyle != VisualStyle.Office2007 && this.vStyle != VisualStyle.Office2010)
				{
					// Draw the chevron.
					Rectangle rcchevron = new Rectangle( rectVirt.Left + 6, rectVirt.Bottom - 23, 4, 5 );
					int nxpos = rcchevron.Left;
                    if(EnableTouchMode)
                        rcchevron = new Rectangle(rectVirt.Left + 6, rectVirt.Bottom - 23, 4 * (int)(1.5F), 5);
					Pen pen = new Pen(Color.Black );
					SolidBrush brush = new SolidBrush(Color.Black);
					if (this.VisualStyle == VisualStyle.Metro && this.eButtonsState == ButtonsState.DropDownButtonHot)
					{
						pen = new Pen(Color.White);
						brush = new SolidBrush(Color.White);  
					}
					for( int i = 0; i < 2; i++ )
					{
						gphVirt.DrawLine(pen, new Point(nxpos, rcchevron.Top), new Point(nxpos + 1, rcchevron.Top));
						gphVirt.DrawLine(pen, new Point(nxpos + 1, rcchevron.Top + 1), new Point(nxpos + 2, rcchevron.Top + 1));
						gphVirt.DrawLine(pen, new Point(nxpos + 2, rcchevron.Top + 2), new Point(nxpos + 3, rcchevron.Top + 2));
						gphVirt.DrawLine(pen, new Point(nxpos + 1, rcchevron.Top + 3), new Point(nxpos + 2, rcchevron.Top + 3));
						gphVirt.DrawLine(pen, new Point(nxpos, rcchevron.Top + 4), new Point(nxpos + 1, rcchevron.Top + 4));
						nxpos = rcchevron.Left + 4;
					}

					// Draw the drop-down arrow.
					Point[] ptsdropdown;
					Rectangle rcdropdown = new Rectangle( rectVirt.Left + 7, rectVirt.Bottom - 12, 5, 3 );
					ptsdropdown = new Point[] { 
											new Point(rcdropdown.Left, rcdropdown.Top),
											new Point(rcdropdown.Right, rcdropdown.Top),
											new Point(rcdropdown.Left+2, rcdropdown.Bottom),
											new Point(rcdropdown.Left, rcdropdown.Top) };
					GraphicsPath path = new GraphicsPath();
					path.AddLines( ptsdropdown );
                    Region r = new Region(path);
					gphVirt.FillRegion( brush, r );
					pen.Dispose();
					brush.Dispose();
                    path.Dispose();
                    r.Dispose();
                    brush.Dispose();
				}
				else
				{
					Point[] ptsbackground;
					Rectangle rcbackground = new Rectangle( rectVirt.Left + 8, rectVirt.Bottom - 18, 5, 4 );

					if( !bIsMirrored )
					{
						ptsbackground = new Point[] { 
											new Point(rcbackground.Left, rcbackground.Top),
											new Point(rcbackground.Right, rcbackground.Top),
											new Point(rcbackground.Right, rcbackground.Top + 1),
											new Point(rcbackground.Left + 2, rcbackground.Bottom),
											new Point(rcbackground.Left, rcbackground.Top + 1),
											new Point(rcbackground.Left, rcbackground.Top) };
					}
					else
					{
						ptsbackground = new Point[] {
											new Point(rcbackground.Left, rcbackground.Top),
											new Point(rcbackground.Right, rcbackground.Top),
											new Point(rcbackground.Right, rcbackground.Top + 1),
											new Point(rcbackground.Left + 3, rcbackground.Bottom),
											new Point(rcbackground.Left, rcbackground.Top + 1),
											new Point(rcbackground.Left, rcbackground.Top) };
					}

					GraphicsPath backgroundPath = new GraphicsPath();
					backgroundPath.AddLines( ptsbackground );
                    Region r2=new Region( backgroundPath );
                    using(Brush brush = new SolidBrush( Color.White ))
                        gphVirt.FillRegion(brush, r2);
                    backgroundPath.Dispose();
                    r2.Dispose();
					Point[] ptsdropdown;
					Rectangle rcdropdown = new Rectangle( rectVirt.Left + 8, rectVirt.Bottom - 18, 5, 3 );

					if( !bIsMirrored )
					{
						ptsdropdown = new Point[] { 
											new Point(rcdropdown.Left, rcdropdown.Top),
											new Point(rcdropdown.Right, rcdropdown.Top),
											new Point(rcdropdown.Left + 2, rcdropdown.Bottom),
											new Point(rcdropdown.Left, rcdropdown.Top) };
					}
					else
					{
						ptsdropdown = new Point[] { 
											new Point(rcdropdown.Left, rcdropdown.Top),
											new Point(rcdropdown.Right, rcdropdown.Top),
											new Point(rcdropdown.Left + 3, rcdropdown.Bottom),
											new Point(rcdropdown.Left, rcdropdown.Top) };
					}

					GraphicsPath dropdownPath = new GraphicsPath();
					dropdownPath.AddLines( ptsdropdown );

					SolidBrush fillBrush = null;

					if( this.m_office2007Theme == Office2007Theme.Blue )
						fillBrush = new SolidBrush( Color.FromArgb( 66, 113, 181 ) );
					else if( this.m_office2007Theme == Office2007Theme.Black )
						fillBrush = new SolidBrush( Color.FromArgb( 49, 52, 49 ) );
					else
						fillBrush = new SolidBrush( Color.FromArgb( 99, 105, 115 ) );
                    Region r1=new Region( dropdownPath );
					gphVirt.FillRegion( fillBrush, r1 );
                    dropdownPath.Dispose();
                    r1.Dispose();
                    fillBrush.Dispose();
				}
			}
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.SetBoundsCore"/>.
		/// </summary>
		protected override void SetBoundsCore( int x, int y, int width, int height, BoundsSpecified specified )
		{
			if( (specified & BoundsSpecified.Width) != 0 && this.Collapsed )
			{
				width = this.CollapsedWidth;
			}

			base.SetBoundsCore( x, y, width, height, specified );

			this.RecalculateGroupBarLayout();
			this.SetActiveClientBounds();
			this.Invalidate( this.ClientRectangle, false );
		}

		private bool m_bIsClientSetting = false;

		/// <summary>
		/// Indicates, if process of setting client for <see cref="GroupBarItem"/> is in progress.
		/// </summary>
		protected internal bool IsClientSetting
		{
			get
			{
				return m_bIsClientSetting;
			}
			set
			{
				if( value != m_bIsClientSetting )
				{
					m_bIsClientSetting = value;
				}
			}
		}
		/// <summary>
		/// Gets or Sets a value indicating whether selection logic includes visible items alone for SelectedItem calculation.
		/// </summary>
		private bool indexOnVisibleItems = true;

		/// <summary>
		/// Gets or Sets a value indicating whether selection logic includes visible items alone for SelectedItem calculation.
		/// </summary>
		public bool IndexOnVisibleItems
		{
			get { return indexOnVisibleItems; }
			set { indexOnVisibleItems = value; }
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnControlAdded"/>.
		/// </summary>
		protected override void OnControlAdded( ControlEventArgs e )
		{
			base.OnControlAdded( e );

			if( !IsClientSetting && this.DesignMode == true )
			{
				if( this.GroupExists( nSelectedItem ) && 
					((this.activeItemsList[this.nSelectedItem] as GroupBarItem).Client == null) )
				{
					(this.activeItemsList[this.nSelectedItem] as GroupBarItem).Client = e.Control;
					this.RecalculateGroupBarLayout();
					this.SetActiveClientBounds();
				}
			}
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnControlRemoved"/>.
		/// </summary>
		protected override void OnControlRemoved( ControlEventArgs e )
		{
			base.OnControlRemoved( e );

			if( this.DesignMode == true )
			{
				if( this.GroupExists( nSelectedItem ) &&
					((this.activeItemsList[this.nSelectedItem] as GroupBarItem).Client != null) &&
					((this.activeItemsList[this.nSelectedItem] as GroupBarItem).Client == e.Control) )
				{
					(this.activeItemsList[this.nSelectedItem] as GroupBarItem).Client = null;
				}
			}
		}

		// When displaying the control for the first time, do a reactivation of the active group if a client control is present.
		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnVisibleChanged"/>.
		/// </summary>
		protected override void OnVisibleChanged( EventArgs e )
		{
			base.OnVisibleChanged( e );

			// When displaying for the first time, reactivate the active index if the group has a non-visible client.
			if( (this.Visible == true) && this.GroupExists( nSelectedItem ) )
			{
				GroupBarItem group = this.activeItemsList[this.nSelectedItem] as GroupBarItem;
				if( group.Client != null && group.Client.Visible == false )
					this.SelectedItem = this.nSelectedItem;
			}
		}

		protected override bool ProcessCmdKey( ref Message msg, Keys keydata )
		{
			if( (msg.Msg == 0x0100/*WM_KEYDOWN*/) && (this.activeItemsList.Count > 0) && ((keydata & Keys.Control) != 0) )
			{
				switch( keydata & (~Keys.Control) )
				{
					case Keys.Up:
						if( this.nSelectedItem > 0 )
						{
							this.SelectedItem -= 1;
							Control client = (this.activeItemsList[this.SelectedItem] as GroupBarItem).Client;
							if( client != null )
								client.Focus();
							return true;
						}
                        else if (this.nSelectedItem == 0)
                        {
                            this.SelectedItem = this.activeItemsList.Count - 1;
                            Control client = (this.activeItemsList[this.SelectedItem] as GroupBarItem).Client;
                            if (client != null)
                                client.Focus();
                            if(!(this.Parent is GroupBar))
                                return true;
                        }
						break;
					case Keys.Down:
						if( this.nSelectedItem < this.activeItemsList.Count-1 )
						{
							this.SelectedItem += 1;
							Control client = (this.activeItemsList[this.SelectedItem] as GroupBarItem).Client;
							if( client != null )
								client.Focus();
							return true;
						}
                        else if(this.nSelectedItem == this.activeItemsList.Count - 1)
                        {
                            this.SelectedItem = 0;
                            Control client = (this.activeItemsList[this.SelectedItem] as GroupBarItem).Client;
                            if (client != null)
                                client.Focus();
                            if (!(this.Parent is GroupBar))
                                return true;
                        }
						break;
				}
			}
			return base.ProcessCmdKey( ref msg, keydata );
		}



		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseDown"/>.
		/// </summary>
		protected override void OnMouseDown( MouseEventArgs e )
		{
			base.OnMouseDown( e );

			Point pt = new Point( e.X, e.Y );
			switch( e.Button )
			{
				case MouseButtons.Left:
					bool bscrlhandled = (this as IIntegratedScrollContainer).HandleScrollButtonDown( pt );
					if( bscrlhandled == false )
						this.HandleMouseLButtonDown( pt );
					break;
				case MouseButtons.Right:
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseUp"/>.
		/// </summary>
		protected override void OnMouseUp( MouseEventArgs e )
		{
			base.OnMouseUp( e );

			Point pt = new Point( e.X, e.Y );
			switch( e.Button )
			{
				case MouseButtons.Left:
					{
						bool bscrlhandled = (this as IIntegratedScrollContainer).HandleScrollButtonUp( pt );
						if( bscrlhandled == false )
						{
							bool bpressed = (this.eButtonsState == ButtonsState.GroupBarItemPushed) ? true : false;
							int nitem = this.HandleMouseLButtonUp( pt );
							// Activate only if the group was in the pressed state previous to the mouse up message.
							if( bpressed == false )
								break;
							if( (nitem >= 0) && (nitem <= this.activeItemsList.Count) )
							{
								if( nitem != this.nSelectedItem )
								{
									int nprevactive = this.nSelectedItem;

									m_bAllowItemPopup = true;

									this.SelectItem( nitem, nprevactive );

									this.bHighlight = false;
									this.eButtonsState = ButtonsState.None;
								}
							}
						}
					}
					break;
				case MouseButtons.Right:	//Fire the context menu event.
					{
						int nitem = this.GetItemFromPoint( pt );
						if( nitem >= 0 )
						{
                            this.nHighlightItem = this.GroupBarItems.IndexOf(this.activeItemsList[this.HighlightItem] as GroupBarItem);
							this.nContextMenuItem = this.nHighlightItem;
							this.OnShowContextMenu( EventArgs.Empty );
						}
					}
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseMove"/>.
		/// </summary>
		protected override void OnMouseMove( MouseEventArgs e )
		{
			base.OnMouseMove( e );

			Point pt = new Point( e.X, e.Y );

			if( ((this.eButtonsState == ButtonsState.ScrollUpPushed)||(this.eButtonsState == ButtonsState.ScrollUpHot)) 
				&& (this.rcScrollUp.Contains( pt ) == false) )
			{
				if( (this.tmrScrolling != null) && (this.tmrScrolling.Enabled == true) )
					this.tmrScrolling.Stop();
				this.SetScrollButtonState( true, ButtonsState.None );
			}
			else if( ((this.eButtonsState == ButtonsState.ScrollDownPushed)||(this.eButtonsState == ButtonsState.ScrollDownHot)) 
				&& (this.rcScrollDown.Contains( pt ) == false) )
			{
				if( (this.tmrScrolling != null) && (this.tmrScrolling.Enabled == true) )
					this.tmrScrolling.Stop();
				this.SetScrollButtonState( false, ButtonsState.None );
			}
			else if( this.rcScrollUp.Contains( pt ) == true )
			{
				if( this.eInScroll == ButtonsState.ScrollUpPushed )
				{
					this.SetScrollButtonState( true, ButtonsState.ScrollUpPushed );
					this.DoScroll( true );
				}
				else if( (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.UseThemedDrawing ||
					this.vStyle == VisualStyle.Office2007) && 
                    (this.eInScroll != ButtonsState.ScrollUpHot) )
				{
					this.SetScrollButtonState( true, ButtonsState.ScrollUpHot );
				}
			}
			else if( this.rcScrollDown.Contains( pt ) == true )
			{
				if( this.eInScroll == ButtonsState.ScrollDownPushed )
				{
					this.SetScrollButtonState( false, ButtonsState.ScrollDownPushed );
					this.DoScroll( false );
				}
				else if( (XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.UseThemedDrawing ||
					this.vStyle == VisualStyle.Office2007) && 
                    (this.eInScroll != ButtonsState.ScrollDownHot) )
				{
					this.SetScrollButtonState( false, ButtonsState.ScrollDownHot );
				}
			}
			else
			{
				this.HandleMouseMove( new Point( e.X, e.Y ) );
			}
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnMouseLeave"/>.
		/// </summary>
		protected override void OnMouseLeave( EventArgs e )
		{
			base.OnMouseLeave( e );

			if( (this.tmrScrolling != null) && (this.tmrScrolling.Enabled == true) )
				this.tmrScrolling.Stop();

			if( (this.eButtonsState == ButtonsState.ScrollUpPushed) || (this.eButtonsState == ButtonsState.ScrollUpHot) )
				this.SetScrollButtonState( true, ButtonsState.None );
			if( (this.eButtonsState == ButtonsState.ScrollDownPushed) || (this.eButtonsState == ButtonsState.ScrollDownHot) )
				this.SetScrollButtonState( false, ButtonsState.None );

			this.HandleMouseMove( this.PointToClient( Cursor.Position ) );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnDragOver"/>.
		/// </summary>
		protected override void OnDragOver( DragEventArgs e )
		{
			if( e.KeyState == 9 )	//CTRL key
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.Move;

			Point ptclient = PointToClient( new Point( e.X, e.Y ) );
			int nitem = this.GetItemFromPoint( ptclient );
			if( (nitem != -1) && (nitem != this.nSelectedItem) )
			{
				if( nStartTick == 0 )
					nStartTick = Environment.TickCount;

				// Allow a 3/4 second delay before the activation.
				if( Environment.TickCount  > (nStartTick+750) )
				{
					this.SelectedItem = nitem;
					GroupBar.nStartTick = 0;
				}
			}
			else
				nStartTick = 0;

			base.OnDragOver( e );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.OnDragLeave"/>.
		/// </summary>
		protected override void OnDragLeave( EventArgs e )
		{
			if( nStartTick != 0 )
				nStartTick = 0;

			base.OnDragLeave( e );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="virtualSelctedItem">Index of selected item is VisibleGroupBarItems</param>
		/// <returns>Index of selected item is GroupBarItems collection</returns>
		protected int GetItemIndex(int virtualSelctedItem)
		{
			if (virtualSelctedItem >= 0 && virtualSelctedItem < this.VisibleGroupBarItems.Count)
				return this.GroupBarItems.IndexOf(this.VisibleGroupBarItems[virtualSelctedItem]);
			else
				return -1;
		}

		/// <summary>
		/// Gets the GroupBarItem from the corresponding mouse point.
		/// </summary>
		protected int GetItemFromPoint( Point pt )
		{
			for( int i=0; i<this.activeItemsList.Count; i++ )
			{
				Rectangle rcbar = GetGroupBarItemBounds( i );
				Rectangle rchitrect = rcbar;
				if( (this.bIntegratedScrolling == true) && 
					((i == this.nSelectedItem) || (i == this.nSelectedItem+1)) )
				{
					CorrectRectForScrollThumbWidth( ref rchitrect );
				}
				if( rchitrect.Contains( pt ) )
				{
					if( i == this.nRenameItem )
						return -1;
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Overloaded. Returns the GroupBarItem at the specified point in client coordinates.
		/// </summary>
		/// <param name="x">X - coordinate of the item.</param>
		/// <param name="y">Y - coordinate of the item.</param>
		/// <returns>GroupBarItem, whose area contains the specified point; Null, if nothing is found.</returns>
		public GroupBarItem PointToItem( int x, int y )
		{
			return PointToItem( new Point( x, y ) );
		}

		/// <summary>
		/// Returns GroupBarItem at specified point in client coordinates.
		/// </summary>
		/// <param name="pt">Point to search GroupBarItem at.</param>
		/// <returns>GroupBarItem, whose area contains the specified point; Null, if nothing is found.</returns>
		public GroupBarItem PointToItem( Point pt )
		{
			GroupBarItem foundItem = null;

			if( ClientRectangle.Contains( pt ) && activeItemsList != null && activeItemsList.Count > 0 )
			{
				for( int i = 0, len = activeItemsList.Count; i < len; i++ )
				{
					Rectangle itemRect = GetGroupBarItemBounds( i );
					if( this.bIntegratedScrolling && 
						((i == this.nSelectedItem) || (i == this.nSelectedItem + 1)) )
					{
						CorrectRectForScrollThumbWidth( ref itemRect );
					}

					GroupBarItem groupItem = this.activeItemsList[i] as GroupBarItem;
					if( groupItem != null )
					{
						if( itemRect.Contains( pt ) )
						{
							foundItem = groupItem;
							break;
						}
						else
						{
							Control client = groupItem.Client;
							if( client != null && this.SelectedItem == i && 
								client.Bounds.Contains( pt ) )
							{
								foundItem = groupItem;
								break;
							}
						}
					}
				}
			}

			return foundItem;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal int HandleMouseLButtonDown( Point pt )
		{
			Rectangle rcbounds = this.GetBoundedRectangle();
			if( this.bStackedMode == true )
			{
				// If the LButtonDown occurs over the splitter then capture the mouse.
				Rectangle rcsplitter = new Rectangle( 1, this.rcClient.Bottom, rcbounds.Width-2, this.nSplitterHeight );
				if( rcsplitter.Contains( pt ) )
				{
					this.ptSplitterDragStart = this.PointToScreen( pt );
					this.Capture = true;
				}

				Rectangle rcddbutton = GetDropDownButtonRectangle();

				if( rcddbutton.Contains( pt ) && this.ShowChevron )
				{
					if( this.eButtonsState != ButtonsState.DropDownButtonPushed )
					{
						this.eButtonsState = ButtonsState.DropDownButtonPushed;
						this.Invalidate( Rectangle.Inflate( rcddbutton, 2, 2 ), false );
					}
					return -1;
				}
				else if( this.Collapsed && this.rcClient.Contains( pt ) )
				{
					m_collapsedClientAreaState = ButtonsState.DropDownButtonPushed;

					this.Capture = true;

					Invalidate( this.rcClient );

					return -1;
				}
				else if( this.AllowCollapse && this.activeItemsList.Count > 0 )
				{
					Rectangle bounds = this.CollapseButtonBounds;

					if( bounds.Contains( pt ) )
					{
						m_collapseButtonState = ButtonsState.DropDownButtonPushed;

						Invalidate( bounds );

						return -1;
					}
				}
			}

			for( int i=0; i<this.activeItemsList.Count; i++ )
			{
				Rectangle rcbar = GetGroupBarItemBounds( i );
				Rectangle rchitrect = rcbar;
				if( (this.bIntegratedScrolling == true) && 
					((i == this.nSelectedItem) || (i == this.nSelectedItem+1)) )
				{
					CorrectRectForScrollThumbWidth( ref rchitrect );
				}
				if( rchitrect.Contains( pt ) )
				{
					GroupBarItem group = this.activeItemsList[i] as GroupBarItem;
					if( i == this.nRenameItem )
						return -1;
					this.nHighlightItem = i;
					if( group.Enabled == false )
						return -1;

					// Draw buttons with pressed look.
					this.eButtonsState = ButtonsState.GroupBarItemPushed;
					if( this.bStackedMode == false )
					{
						this.bHighlight = false;
						this.Invalidate( rchitrect, false );
					}
					else
					{
						this.Invalidate( rchitrect, false );
						this.Refresh();
					}
					return i;
				}
			}

			return -1;
		}
		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal int HandleMouseLButtonUp( Point pt )
		{
			Rectangle rcbounds = this.GetBoundedRectangle();

			// Restore pressed / highlight state if out of the group rect.
			if( this.nHighlightItem >= 0 )
			{
                Debug.Assert((this.nHighlightItem >= 0) && (this.nHighlightItem < this.activeItemsList.Count));
				Rectangle rcbar = this.GetGroupBarItemBounds( this.nHighlightItem );
				Rectangle rchitrect = rcbar;
				if( (this.bIntegratedScrolling == true) && 
					((nHighlightItem == this.nSelectedItem) || (nHighlightItem == this.nSelectedItem+1)) )
				{
					CorrectRectForScrollThumbWidth( ref rchitrect );
				}

				if( this.eButtonsState == ButtonsState.GroupBarItemPushed )
				{
					if( this.bStackedMode == false )	// In StackedMode the selected item is always displays with a pushed state.
					{
						this.eButtonsState = ButtonsState.None;
						this.Invalidate( Rectangle.Inflate( rchitrect, 1, 1 ), false );
					}
				}
			}

			if( this.bStackedMode == true )
			{
				// If a splitter drag has been in progress, then end drag and reset the cursor.
				if( this.ptSplitterDragStart != new Point( -1, -1 ) )
				{
					this.Capture = false;
					this.ptSplitterDragStart = new Point( -1, -1 );
					Rectangle rcsplitter = new Rectangle( 0, this.rcClient.Bottom, rcbounds.Width, this.nSplitterHeight );
					if( (rcsplitter.Contains( pt ) == false) && (base.Cursor == Cursors.SizeNS) )
						base.Cursor = this.gDefaultCursor;
				}

				// Check whether the mouse is over the hidden items button and if True, then depend on 
				// the mouse action to perform either a highlight or a button click action.
				Rectangle rcddbutton = GetDropDownButtonRectangle();

				if( rcddbutton.Contains( pt ) && this.ShowChevron )
				{
					if( this.eButtonsState == ButtonsState.DropDownButtonPushed )
					{
						this.eButtonsState = ButtonsState.DropDownButtonHot;
						this.Invalidate( Rectangle.Inflate( rcddbutton, 2, 2 ), false );
					}

					// Display the drop-down context menu.
					this.ShowDropDownMenu();
					return -1;
				}
				else if( (this.eButtonsState == ButtonsState.DropDownButtonHot) || (this.eButtonsState == ButtonsState.DropDownButtonPushed) )
				{
					this.eButtonsState = ButtonsState.None;
					this.Invalidate( Rectangle.Inflate( rcddbutton, 2, 2 ), false );
				}

				if( this.Collapsed && this.Capture && this.rcClient.Contains( pt ) )
				{
					this.Capture = false;
					base.Cursor = this.gDefaultCursor;

					m_collapsedClientAreaState = ButtonsState.DropDownButtonHot;

					HandleCollapsedClientAreaClick();
					Invalidate( this.rcClient );

					return -1;
				}

				if( this.AllowCollapse && this.activeItemsList.Count > 0 )
				{
					Rectangle bounds = this.CollapseButtonBounds;

					if( bounds.Contains( pt ) )
					{
						if( m_collapseButtonState == ButtonsState.DropDownButtonPushed )
						{
							HandleCollapseButtonClick();
						}

						m_collapseButtonState = ButtonsState.DropDownButtonHot;

						Invalidate( bounds );

						return -1;
					}
				}

			}

			for( int i=0; i<this.activeItemsList.Count; i++ )
			{
				Rectangle rcbar = GetGroupBarItemBounds( i );
				Rectangle rchitrect = rcbar;
				if( (this.bIntegratedScrolling == true) && 
					((i == this.nSelectedItem) || (i == this.nSelectedItem+1)) )
				{
					CorrectRectForScrollThumbWidth( ref rchitrect );
				}
				if( rchitrect.Contains( pt ) )
				{
					if( i == this.nRenameItem )
						return -1;
					this.nHighlightItem = i;
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Creates and displays the drop-down button context menu.
		/// </summary>
		protected internal void ShowDropDownMenu()
		{
			IContextMenuProvider menu = this.ContextMenuProvider;
			menu.InitializeContextMenu();
			if( ((XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.UseThemedDrawing) == false) 
				&& (this.vStyle == VisualStyle.Office2003) )
			{
				menu.SetVisualStyle( VisualStyle.Office2003 );
			}
			else if( ((XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.UseThemedDrawing) == false)
				&& (this.vStyle == VisualStyle.Office2007) )
			{
				menu.SetVisualStyle( VisualStyle.Office2007Outlook );
			}

			this.InitializeNavigationButtonMenu( menu );
			NavigationPaneDropDownClickEventArgs args = new NavigationPaneDropDownClickEventArgs( menu );
			this.OnNavigationPaneButtonClick( args );
			Rectangle rcddbutton = this.GetDropDownButtonRectangle();

            if (menu.GetItemsCount()!= 0 && menu.NeedAddRemoveButtons())
				InitAddRemoveNavMenu( menu );

			if( this.Collapsed )
			{
				InitCollapsedNavMenu( menu );				
			}

            if (menu.GetItemsCount() != 0)
				menu.ShowContextMenu( this, new Point( rcddbutton.Right, rcddbutton.Top+(rcddbutton.Height/2) ) );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal int HandleMouseMove( Point pt )
		{
			Rectangle rcbounds = this.GetBoundedRectangle();

			// Restore pressed / highlight state if out of the group rect.
			if( this.nHighlightItem >= 0 )
			{
				Debug.Assert( (this.nHighlightItem >= 0) && (this.nHighlightItem < this.GroupBarItems.Count) );
				Rectangle rcbar = this.GetGroupBarItemBounds( this.nHighlightItem );
				Rectangle rchitrect = rcbar;
				if( (this.bIntegratedScrolling == true) && 
					((nHighlightItem == this.nSelectedItem) || (nHighlightItem == this.nSelectedItem+1)) )
				{
					CorrectRectForScrollThumbWidth( ref rchitrect );
				}

				if( !rchitrect.Contains( pt ) )
				{
					this.bHighlight = false;
					this.nHighlightItem = -1;
					this.eButtonsState = ButtonsState.None;
					if( (base.Cursor == this.itemCursor) && (this.gDefaultCursor != null) )
						base.Cursor = this.gDefaultCursor;
					this.Invalidate( Rectangle.Inflate( rchitrect, 1, 1 ), false );
				}
			}

			if( this.bStackedMode == true )
			{
				if( this.ptSplitterDragStart == new Point( -1, -1 ) )
				{
					// If the mouse is over the GroupBar header or the stacksizing splitter, set the appropriate cursor.
					Rectangle rcsplitter = new Rectangle( 1, this.rcClient.Bottom, rcbounds.Width-1, this.nSplitterHeight );
					if( rcsplitter.Contains( pt ) )
					{
						if( base.Cursor != Cursors.SizeNS )
							base.Cursor = Cursors.SizeNS;
					}
					else
					{
						if( (base.Cursor == Cursors.SizeNS) && (this.gDefaultCursor != null) )
							base.Cursor = this.gDefaultCursor;
					}
				}
				else
				{
					// If the mouse has moved a vertical distance of more than nItemHeight and the navigation pane contains 
					// item, then transfer it from the pane to the GroupBarItem stack, else move items from the stack to the 
					// navigation pane.
					Point ptscreen = this.PointToScreen( pt );
					if( ptscreen.Y < (this.ptSplitterDragStart.Y-this.nItemHeight) )
					{
						if( this.alNavPaneItems.Count > 0 )
						{
							(this.alNavPaneItems[0] as GroupBarItem).InNavigationPane = false;
							this.Refresh();
							this.ptSplitterDragStart = ptscreen;
						}
					}
					else if( ptscreen.Y > (this.ptSplitterDragStart.Y+this.nItemHeight) )
					{
						if( this.activeItemsList.Count > this.alNavPaneItems.Count )
						{
							(this.activeItemsList[(this.activeItemsList.Count-this.alNavPaneItems.Count)-1] as GroupBarItem).InNavigationPane = true;
							this.Refresh();
							this.ptSplitterDragStart = ptscreen;
						}
					}
					return -1;
				}

				// Check whether the mouse is over the navigation panel drop-down button and if True, then depend on 
				// the mouse action to perform either a highlight or a button click action.
				Rectangle rcddbutton = GetDropDownButtonRectangle();

				if( rcddbutton.Contains( pt ) && this.ShowChevron )
				{
					if( base.Cursor != this.itemCursor )
						base.Cursor = this.itemCursor;                    
					if( !this.compToolTip.Active )
					{
						this.compToolTip.SetToolTip( this, m_sGroupBarDropDownToolTip );
						this.compToolTip.Active = true;
					}

					if( (this.eButtonsState != ButtonsState.DropDownButtonHot) && (this.eButtonsState != ButtonsState.DropDownButtonPushed) )
					{
						this.eButtonsState = ButtonsState.DropDownButtonHot;
						this.Invalidate( Rectangle.Inflate( rcddbutton, 2, 2 ), false );
					}
					return -1;
				}
				else
				{
					if( this.Collapsed && this.activeItemsList.Count > 0 )
					{
						if( this.rcClient.Contains( pt ) )
						{
							base.Cursor = Cursors.Hand;

							if( m_collapsedClientAreaState == ButtonsState.None && !this.compToolTip.Active )
							{
								this.compToolTip.SetToolTip( this, this.NavigationPaneTooltip );
								this.compToolTip.Active = true;
							}

							if( this.Capture )
							{
								if( m_collapsedClientAreaState != ButtonsState.None )
								{
									m_collapsedClientAreaState = ButtonsState.DropDownButtonPushed;
									Invalidate( this.rcClient );
								}
							}
							else
							{
								if( m_collapsedClientAreaState != ButtonsState.DropDownButtonHot )
								{
									m_collapsedClientAreaState = ButtonsState.DropDownButtonHot;
									Invalidate( this.rcClient );
								}
							}

							return -1;
						}
						else
						{
							if( this.Capture )
							{
								if( m_collapsedClientAreaState != ButtonsState.None )
								{
									m_collapsedClientAreaState = ButtonsState.DropDownButtonHot;
									Invalidate( this.rcClient );
								}
							}
							else
							{
								m_collapsedClientAreaState = ButtonsState.None;
								Invalidate( this.rcClient );
							}
						}
					}

					if( this.AllowCollapse && this.activeItemsList.Count > 0 )
					{
						Rectangle bounds = this.CollapseButtonBounds;

						if( bounds.Contains( pt ) )
						{
							if( m_collapseButtonState == ButtonsState.None )
							{
								m_collapseButtonState = ButtonsState.DropDownButtonHot;

								if( !this.compToolTip.Active )
								{
									this.compToolTip.SetToolTip( this, this.CollapseButtonTooltip );
									this.compToolTip.Active = true;
								}
							}

							Invalidate( bounds );

							return -1;
						}
						else
						{
							if( m_collapseButtonState != ButtonsState.None )
							{
								m_collapseButtonState = ButtonsState.None;

								Invalidate( bounds );
							}
						}
					}

					if( (base.Cursor == this.itemCursor) && (this.gDefaultCursor != null) )
						base.Cursor = this.gDefaultCursor;

					if( (this.eButtonsState == ButtonsState.DropDownButtonHot) || (this.eButtonsState == ButtonsState.DropDownButtonPushed) )
					{
						this.eButtonsState = ButtonsState.None;
						this.Invalidate( Rectangle.Inflate( rcddbutton, 2, 2 ), false );
					}
				}
			}

			bool bToolTipSet = false;

			for( int i=0; i < this.activeItemsList.Count; i++ )
			{
				Rectangle rcbar = GetGroupBarItemBounds( i );
				Rectangle rchitrect = rcbar;
				if( (this.bIntegratedScrolling == true) && 
					((i == this.nSelectedItem) || (i == this.nSelectedItem+1)) )
				{
					CorrectRectForScrollThumbWidth( ref rchitrect );
				}
				if( rchitrect.Contains( pt ) )
				{
					if( i == this.nRenameItem )
						return -1;

					if( base.Cursor != this.itemCursor )
						base.Cursor = this.itemCursor;

					GroupBarItem item = this.activeItemsList[i] as GroupBarItem;
					if( (this.alNavPaneItems.Contains( item ) || this.Collapsed) &&
						(!this.compToolTip.Active || item.Text != this.compToolTip.GetToolTip( this )) )
					{
						this.compToolTip.SetToolTip( this, item.Text );
						this.compToolTip.Active = true;
						bToolTipSet = true;
					}

					this.nHighlightItem = i;

					if( item.Enabled == false )
						return -1;

					if( (this.bBarHighlight == true) && (this.eButtonsState == ButtonsState.None)
						&& (this.bHighlight == false) )
					{
						// Do the highlight drawing.
						this.bHighlight = true;
						this.Invalidate( Rectangle.Inflate( rchitrect, 1, 1 ), false );
					}

					if( !bToolTipSet && item.Text != this.compToolTip.GetToolTip( this ) )
					{
						this.compToolTip.Active = false;
					}

					return i;
				}
			}

			this.compToolTip.Active = false;

			return -1;
		}

		/// <summary>
		/// Initializes the navigation button menu.
		/// </summary>
		/// <param name="menu">The context menu provider used to create the menus.</param>
		protected virtual void InitializeNavigationButtonMenu( IContextMenuProvider menu )
		{
			String morebuttonsitem = SR.GetString( SR.MoreButtonsItemsText,this );
			menu.AddContextMenuItem( String.Empty, morebuttonsitem, new EventHandler( this.OnMoreButtons_Click ) );
			menu.SetContextMenuItemImage( morebuttonsitem, s_ilNavPaneItems, 0 );
			if( this.alNavPaneItems.Count == 0 )
				menu.SetContextMenuItemEnabled( morebuttonsitem, false );

			String fewerbuttonsitem = SR.GetString(SR.FewerButtonsItemsText, this);
			menu.AddContextMenuItem( String.Empty, fewerbuttonsitem, new EventHandler( this.OnFewerButtons_Click ) );
			menu.SetContextMenuItemImage( fewerbuttonsitem, s_ilNavPaneItems, 1 );
			if( this.alNavPaneItems.Count == this.activeItemsList.Count )
				menu.SetContextMenuItemEnabled( fewerbuttonsitem, false );
		}

		/// <summary>
		/// Raises the navigation pane button click event.
		/// </summary>
		/// <param name="e">An <see cref="T:Syncfusion.Windows.Forms.Tools.NavigationPaneDropDownClickEventArgs"/> that contains the event data.</param>
		protected void OnNavigationPaneButtonClick( NavigationPaneDropDownClickEventArgs e )
		{
			if( this.NavigationPaneDropDownClick != null )
				this.NavigationPaneDropDownClick( this, e );
		}

		/// <summary>
		/// Called when more buttons menuitem is clicked.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected void OnMoreButtons_Click( Object sender, EventArgs e )
		{
			// Remove the first item in the alNavPaneItems list and set this with a stacked or visible item.
			if( this.alNavPaneItems.Count > 0 )
			{
				GroupBarItem item = this.alNavPaneItems[0] as GroupBarItem;
				item.InNavigationPane = false;
			}
		}

		/// <summary>
		/// Called when fewer buttons menuitem is clicked.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected void OnFewerButtons_Click( Object sender, EventArgs e )
		{
			// Compose a list of the stacked GroupBarItems and move the last one in the list to the navigation pane.
			ArrayList stackeditems = new ArrayList();
			foreach( GroupBarItem item in this.activeItemsList )
			{
				if( this.alNavPaneItems.Contains( item ) == false )
					stackeditems.Add( item );
			}
			if( stackeditems.Count > 0 )
			{
				GroupBarItem item = stackeditems[stackeditems.Count-1] as GroupBarItem;
				item.InNavigationPane = true;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal Rectangle GetGroupBarItemBounds( int nitem )
		{
			Rectangle rcbounds = this.GetBoundedRectangle();
			Rectangle rcbar = Rectangle.Empty;
			if( !this.bStackedMode )
			{
				rcbar = new Rectangle( rcbounds.Left, rcbounds.Top, rcbounds.Width, this.nItemHeight );
				for( int i=0; i<this.activeItemsList.Count; i++ )
				{
					if( i == nitem )
						break;
					if( i != nSelectedItem || this.rcClient.Height <= 0 )
					{
						rcbar.Y += rcbar.Height;
					}
					else
					{
						rcbar.Y = this.rcClient.Bottom;
					}
					rcbar.Height = this.nItemHeight;
				}
			}
			else
			{
				if( this.GroupExists( nitem ) )
				{
					GroupBarItem item = this.activeItemsList[nitem] as GroupBarItem;
					if( this.alNavPaneItems.Contains( item ) == false )
					{
						// If the stacked item's index is higher than that of a nav pane item, reduce the index used for calculating
						// the vertical offset by that value.
						int nvertoffset = nitem;
						foreach( GroupBarItem navpaneitem in this.alNavPaneItems )
						{
							if( this.activeItemsList.IndexOf( navpaneitem ) < nitem )
								nvertoffset--;
						}
						rcbar = new Rectangle( rcbounds.Left, this.rcClient.Bottom+this.nSplitterHeight+(this.nItemHeight*nvertoffset), rcbounds.Width, this.nItemHeight );
					}
					else	// The item is being shown in the navigation pane.
					{
						int leftof = this.alNavPaneItems.Count - this.alNavPaneItems.IndexOf( item );
						rcbar = GetDropDownButtonRectangle();
						if( GetIsMirrored() )
						{
							int shiftVal = (this.ShowChevron) ? rcbar.Width : 0;
							rcbar.X += shiftVal + (leftof - 1) * this.nNavigationButtonWidth;
						}
						else
						{
							int startPoint = (this.ShowChevron) ? rcbar.X : rcbounds.Right;
							rcbar.X = startPoint - leftof * this.nNavigationButtonWidth;
						}
						rcbar.Width = this.nNavigationButtonWidth;
					}
				}
			}
			return rcbar;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void SetScrollButtonState( bool bupscroll, ButtonsState state )
		{
			this.eButtonsState = state;
			if( bupscroll == true )
				this.Invalidate( this.rcScrollUp );
			else
				this.Invalidate( this.rcScrollDown );
			this.Update();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool IsUpScrollButtonEnabled()
		{
			if( (this.activeItemsList.Count == 0) || (this.nSelectedItem < 0) )
				return false;
			Control client = (this.activeItemsList[this.nSelectedItem] as GroupBarItem).Client;
			if( (client == null) || ((client is IIntegratedScrollClient) == false) )
				return false;
			IIntegratedScrollClient iisc = client as IIntegratedScrollClient;
			return iisc.IsUpScrollButtonEnabled();
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected bool IsDownScrollButtonEnabled()
		{
			if( (this.activeItemsList.Count == 0) || (this.nSelectedItem < 0) )
				return false;
			Control client = (this.activeItemsList[this.nSelectedItem] as GroupBarItem).Client;
			if( (client == null) || ((client is IIntegratedScrollClient) == false) )
				return false;
			IIntegratedScrollClient iisc = client as IIntegratedScrollClient;
			return iisc.IsDownScrollButtonEnabled();
		}

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		internal bool GetIsMirrored()
		{
			return RightToLeft.Yes == this.RightToLeft;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void DoScroll( bool bupscrllbttn )
		{
			if( this.tmrScrolling.Enabled == false )
			{
				this.tmrScrolling.Start();
				this.eInScroll = bupscrllbttn ? ButtonsState.ScrollUpPushed : ButtonsState.ScrollDownPushed;
			}

			Control client = (this.activeItemsList[this.nSelectedItem] as GroupBarItem).Client;
			if( (client == null) || ((client is IIntegratedScrollClient) == false) )
				return;
			IIntegratedScrollClient iisc = client as IIntegratedScrollClient;

			if( bupscrllbttn == true )
				iisc.UpScrollButtonPressed();
			else
				iisc.DownScrollButtonPressed();
		}

		// Private implementation of the IIntegratedScrollContainer interface.
		void IIntegratedScrollContainer.InvalidateUpScrollButton()
		{
			if( this.bIntegratedScrolling == true )
				this.Invalidate( this.rcScrollUp, false );
		}

		void IIntegratedScrollContainer.InvalidateDownScrollButton()
		{
			if( this.bIntegratedScrolling == true )
				this.Invalidate( this.rcScrollDown, false );
		}

		bool IIntegratedScrollContainer.HandleScrollButtonDown( Point pt )
		{
			if( this.rcScrollUp.Contains( pt ) && this.IsUpScrollButtonEnabled()
				&& ((this.activeItemsList[this.nSelectedItem] as GroupBarItem).Enabled == true) )
			{
				this.SetScrollButtonState( true, ButtonsState.ScrollUpPushed );
				this.DoScroll( true );
				return true;
			}
			else if( this.rcScrollDown.Contains( pt ) && this.IsDownScrollButtonEnabled()
				&& ((this.activeItemsList[this.nSelectedItem] as GroupBarItem).Enabled == true) )
			{
				this.SetScrollButtonState( false, ButtonsState.ScrollDownPushed );
				this.DoScroll( false );
				return true;
			}
			return false;
		}

		bool IIntegratedScrollContainer.HandleScrollButtonUp( Point pt )
		{
			if( (this.tmrScrolling != null) && (this.tmrScrolling.Enabled == true) )
			{
				this.tmrScrolling.Stop();
				this.eInScroll = ButtonsState.None;
			}
			if( this.eButtonsState == ButtonsState.ScrollUpPushed )
			{
				if( this.rcScrollUp.Contains( pt ) )
					this.SetScrollButtonState( true, ButtonsState.None );
				return true;
			}
			else if( this.eButtonsState == ButtonsState.ScrollDownPushed )
			{
				if( this.rcScrollDown.Contains( pt ) )
					this.SetScrollButtonState( false, ButtonsState.None );
				return true;
			}
			return false;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected Rectangle CalcTextRect( Graphics gph, GroupBarItem group, Rectangle rcbar )
		{
            int padding = 5; // Add this to ensure the text appears properly
			Rectangle rctext = rcbar;

			TextFormatFlags tf = m_bDrawingHeader ? this.HeaderTextFormat : this.ItemTextFormat;

			int destWidth = rcbar.Width;
			if( destWidth <= 0 )
			{
				group.IsTextVisible = false;
			}
			else
			{
				group.IsTextVisible = true;
			}

			int ImageWidth = 0;
			int ImageHeight = 0;

			int TextAndImageHeight = 0;

			bool largeImageMode = group.LargeImageMode;

			if( (group.Image != null && largeImageMode) ||
				(group.Icon != null && !largeImageMode) )
			{
				Size imageSize = (largeImageMode) ? group.Image.Size : group.Icon.Size;
				ImageWidth = imageSize.Width;
				ImageHeight = imageSize.Height;

                if (!largeImageMode)
                {
                    ImageHeight = rcbar.Height - nTxtOffset;
                    float ScaleRatio = (float)ImageHeight / (float)imageSize.Height;
                    ImageWidth = (int)(ScaleRatio * imageSize.Width);
                }
              
				if( !m_bDrawingHeader )
					TextAndImageHeight = TextRenderer.MeasureText(group.Text, group.Font, rcbar.Size, tf).Height + ImageHeight;
				else
					TextAndImageHeight = TextRenderer.MeasureText(group.Text, this.hdrFont, rcbar.Size, tf).Height + ImageHeight;

				if( (!group.YChanged) && (TextAndImageHeight >= rcbar.Height) )
				{
					destWidth = rcbar.Width - ImageWidth - group.Padding;
				}

				if( destWidth <= 0 )
				{
					group.IsTextVisible = false;
				}
				else
				{
					group.IsTextVisible = true;
				}
			}

			Size szlabel = Size.Empty;
			
			if( !m_bDrawingHeader )
				szlabel = TextRenderer.MeasureText(group.Text, group.Font, rcbar.Size, tf);
			else
				szlabel = TextRenderer.MeasureText(group.Text, this.hdrFont, rcbar.Size, tf);

			AppendAlignment(rcbar, group, szlabel.Width, ImageWidth, szlabel, ImageHeight, ref rctext);

            
            rctext.Width += padding; // Add this to ensure the text appears properly for Corbel

			return rctext;
		}

		private Rectangle AppendAlignment( Rectangle rcbar, GroupBarItem group, int nTextWidth, int ImageWidth, SizeF szlabel, int ImageHeight, ref Rectangle rctext )
		{
			bool bTextWrapSpacePresent = ((nTextWidth + ImageWidth + group.Padding) > rcbar.Width) && ((szlabel.Height + ImageHeight) < rcbar.Height) && (group.IsTextVisible);
			bool isImageNull = (group.LargeImageMode) ? group.Image == null : group.Icon == null;

			switch( this.nAlignment )
			{
				case TextAlignment.Left:
					rctext.X = rcbar.X + nTxtOffset + group.Padding;

					if( !isImageNull )
					{
						if( bTextWrapSpacePresent )
						{
							rctext.Y += ImageHeight / 2;
							group.YChanged = true;
						}
						else
						{
							rctext.X += ImageWidth;
							group.YChanged = false;
						}
					}
					break;
				case TextAlignment.Center:
					rctext.X = rcbar.X + (rcbar.Width - nTextWidth + group.Padding) / 2;

					if( !isImageNull )
					{
						if( bTextWrapSpacePresent )
						{
							rctext.Y += ImageHeight / 2;
							group.YChanged = true;
						}
						else
						{
							rctext.X += ImageWidth / 2;
							group.YChanged = false;
						}
					}
					break;
				case TextAlignment.Right:
					rctext.X = rcbar.X + (rcbar.Width - nTextWidth);
					if( !isImageNull )
					{
						if( bTextWrapSpacePresent )
						{
							rctext.Y += ImageHeight / 2;
							group.YChanged = true;
						}
						else
						{
							if( rctext.X < 0 )
								rctext.X = rcbar.X;
							group.YChanged = false;
						}
					}
					break;
			}

			if( group.IsTextVisible )
				rctext.Width = nTextWidth;

			bool bIsMirrored = GetIsMirrored();
			if( bIsMirrored )
			{
				int nLeftOffset = rctext.Left - rcbar.Left;
				rctext.X = rcbar.Right - (nLeftOffset + rctext.Width);
			}

			return rctext;
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected void ScrollTimerEventHandler( Object obj, EventArgs args )
		{
			if( this.eButtonsState == ButtonsState.ScrollUpPushed )
				DoScroll( true );
			else if( this.eButtonsState == ButtonsState.ScrollDownPushed )
				DoScroll( false );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void RecalculateGroupBarLayout()
		{
			this.activeItemsList.Clear();

			this.rcClient = Rectangle.Empty;
			Rectangle rcbounds = this.GetBoundedRectangle();

			foreach( GroupBarItem item in this.cllnGroupBarItems )
			{
				if( this.DesignMode == false )
				{
					if( item.Visible == true )
						this.activeItemsList.Add( item );
				}
				else
				{
					this.activeItemsList.Add( item );
				}
			}

			if( this.bStackedMode == false )
			{
				this.rcClient = new Rectangle( rcbounds.Left, rcbounds.Top, rcbounds.Width, rcbounds.Height );
				for( int i=0; i<this.activeItemsList.Count; i++ )
				{
					this.rcClient.Y += this.nItemHeight;
					if( i == nSelectedItem )
						break;
				}

				if( this.bIntegratedScrolling == false )
				{
					this.rcClient.Height = rcbounds.Height - (this.activeItemsList.Count*this.nItemHeight);
				}
				else
				{
					this.rcClient.Width -= 1;
					if( this.nSelectedItem == this.activeItemsList.Count-1 )	// Last group is selected
						this.rcClient.Height = rcbounds.Height - ((this.activeItemsList.Count+1)*this.nItemHeight);
					else
						this.rcClient.Height = rcbounds.Height - (this.activeItemsList.Count*this.nItemHeight);
				}
			}
			else
			{
				// Run through the GroupBarItems collection and make sure that those items that have the InNavigationPane property 
				// set are present in the GroupBar's alNavPaneItems and those that do not are not present in it.
				foreach( GroupBarItem item in this.activeItemsList )
				{
					if( item.InNavigationPane == true )
					{
						if( this.alNavPaneItems.Contains( item ) == false )
						{
							// Insert the item into the navpane list in the same order that it exists in the GroupBarItems collection.
							int insertpos = 0;
							foreach( GroupBarItem navitem in this.alNavPaneItems )
							{
								if( this.activeItemsList.IndexOf( navitem ) > this.activeItemsList.IndexOf( item ) )
									break;
								insertpos++;
							}
							this.alNavPaneItems.Insert( insertpos, item );
						}
					}
					else
					{
						if( this.alNavPaneItems.Contains( item ) == true )
							this.alNavPaneItems.Remove( item );
					}
				}


                int nitemheight = this.nHeaderHeight + this.nSplitterHeight + (this.nItemHeight * (this.activeItemsList.Count - this.alNavPaneItems.Count)) +
                    (this.ShowNavigationPane ? this.nNavigationPaneHeight : 0);
                this.rcClient = new Rectangle( rcbounds.Left, rcbounds.Top+this.nHeaderHeight, rcbounds.Width, rcbounds.Height-nitemheight );
			}

			if( this.bDrawClientBorder == true )
				this.rcClient.Inflate( -1, -1 );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void SetActiveClientBounds()
		{
			if( !this.GroupExists( nSelectedItem ) )
			{
				return;
			}
			GroupBarItem selecteditem = this.activeItemsList[this.nSelectedItem] as GroupBarItem;
			// Check if any GroupBarItem clients are currently visible, and if so, hide it first.
			foreach( GroupBarItem item in this.activeItemsList )
			{
                if ((item != selecteditem) && (item.Client != null) && (item.Client.Visible == true))
                {
                    if (!this.Focused)
                    {
                       this.Focus();                       
                    }                    
                    item.Client.Visible = false;                    
                }
			}
			if( selecteditem.Client == null )
				return;
			selecteditem.Client.BringToFront();
			selecteditem.Client.Visible = !this.Collapsed;
			selecteditem.Client.Bounds = new Rectangle( this.rcClient.Left, this.rcClient.Top, this.rcClient.Width, this.rcClient.Height );
		}

		/// <summary>
		/// Overridden. See <see cref="System.Windows.Forms.Control.Dispose"/>.
		/// </summary>
		protected override void Dispose( bool bdispose )
		{
			if( bdispose )
			{
				if( this.DesignMode == false )
				{
					if( this.cllnGroupBarItems.Count > 0 )
					{
						GroupBarItem[] items = new GroupBarItem[this.cllnGroupBarItems.Count];
						this.cllnGroupBarItems.CopyTo( items, 0 );
						this.cllnGroupBarItems.Clear();
						foreach( GroupBarItem item in items )
							item.Dispose();
					}
				}
				if( this.tdButton != null )
				{
					this.tdButton.Dispose();
					this.tdButton = null;
				}
				if( this.tdScrollBar != null )
				{
					this.tdScrollBar.Dispose();
					this.tdScrollBar = null;
				}
				if( this.compToolTip != null )
				{
					this.compToolTip.Dispose();
					this.compToolTip = null;
				}

				if( null != m_itemPopup )
				{
					m_itemPopup.OpenedChanged -= new EventHandler( ItemPopupOpenedChanged );
					m_itemPopup.BeforeClose -= new MouseClickCancelEventHandler( ItemPopupBeforeClose );

					m_itemPopup.Dispose();
					m_itemPopup = null;
				}

				if( null != m_ilNavMenu )
				{
					m_ilNavMenu.Dispose();
					m_ilNavMenu = null;
				}
                if (s_ilNavPaneItems != null)
                {
                    for (int i = 0; i < s_ilNavPaneItems.Images.Count; i++)
                        s_ilNavPaneItems.Images[i].Dispose();
                }
			}

			base.Dispose( bdispose );
		}

		// Accessibility Implementation.
		[Syncfusion.Documentation.DocumentationExclude()]
		protected GroupBarItemAccessibleObjectsIndexer gbiAccesibleObjects = null;

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal GroupBarItemAccessibleObjectsIndexer ItemAccessibleObjects
		{
			get
			{
				if( gbiAccesibleObjects == null )
					gbiAccesibleObjects = new GroupBarItemAccessibleObjectsIndexer( this );
				return gbiAccesibleObjects;
			}
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal GroupBarItemAccessibleObject CreateGroupBarItemAccessibilityInstance( int index )
		{
			return new GroupBarItemAccessibleObject( this.activeItemsList[index] as GroupBarItem );
		}

		protected override AccessibleObject CreateAccessibilityInstance()
		{
			return new GroupBarControlAccessibleObject( this );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal Rectangle GetNavPaneRectangle()
		{
			Rectangle rcbounds = this.GetBoundedRectangle();
			return new Rectangle( rcbounds.Left, rcbounds.Bottom-1-this.nNavigationPaneHeight,
				rcbounds.Width, this.nNavigationPaneHeight );
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		public Rectangle GetDropDownButtonRectangle()
		{
			Rectangle rcpane = this.GetBoundedRectangle();
			rcpane.Y = rcpane.Bottom - this.nNavigationPaneHeight;
			rcpane.Height = this.nNavigationPaneHeight;
            int DropWidth = this.nDropdownButtonWidth;
            if (EnableTouchMode)
            {
                DropWidth = nDropdownButtonWidth * (int)(1.5F);
            }
			return new Rectangle(
                GetIsMirrored() ? rcpane.Left : rcpane.Right - DropWidth,
                rcpane.Top, DropWidth, rcpane.Height);
		}

		[Syncfusion.Documentation.DocumentationExclude()]
		protected internal void CorrectRectForScrollThumbWidth( ref Rectangle rRect )
		{
			rRect.Width -= this.nScrollThumbWidth;

			if( GetIsMirrored() )
			{
				rRect.X += this.nScrollThumbWidth;
			}
		}

		#region ISupportInitialize

		private bool m_bInitializing = false;

		/// <summary>
		/// Signals the object that initialization is starting.
		/// </summary>
		public void BeginInit()
		{
			this.m_bInitializing = true;
		}

		/// <summary>
		/// Signals the object that initialization is complete.
		/// </summary>
		public void EndInit()
		{
			this.m_bInitializing = false;

			int nExpandedWidth = m_nExpandedWidth;

			this.Collapsed = m_bCollapsedInitValue;

			if( nExpandedWidth > 0 )
			{
				m_nExpandedWidth = nExpandedWidth;
			}
		}

		#endregion

		/// <summary>
		/// Get is control initializing.
		/// </summary>
		[Browsable( false )]
		public bool IsInitializing
		{
			get
			{
				return this.m_bInitializing;
			}
		}

		#region Sliding feature

		#region Constants

		private const int c_nDefaultCollapsedWidth = 32;
		private static readonly string c_sDefaultCollapsedText = "Navigation Pane";
        /// <summary>
        ///
        /// </summary>
		private  int c_nCollapseButtonWidth = 18;
        /// <summary>
        ///
        /// </summary>
		private  int c_nCollapseButtonHeight = 18;
		private const int c_nCollapseButtonPadRight = 5;
		private const int c_nCollapseButtonPadTop = c_nCollapseButtonPadRight;
		private const int c_nMinCollapsedWidth = 20;

		#endregion

		#region Fields

		/// <summary>
		/// Indicates whether <see cref="GroupBar"/> is collapsed.
		/// </summary>
		private bool m_bCollapsed;

		/// <summary>
		/// Stores <see cref="m_bCollapsed"/> value while <see cref="GroupBar"/> initialization.
		/// </summary>
		private bool m_bCollapsedInitValue;

		/// <summary>
		/// Indicates whether <see cref="GroupBar"/> can be collapsed.
		/// </summary>		
		private bool m_bAllowCollapse;

		/// <summary>
		/// Width of the collapsed <see cref="GroupBar"/>.
		/// </summary>
		private int m_nCollapsedWidth = c_nDefaultCollapsedWidth;

		/// <summary>
		/// Width of the expanded <see cref="GroupBar"/>.
		/// </summary>
		private int m_nExpandedWidth;

		/// <summary>
		/// Item popup's client heigth.
		/// </summary>
		private int m_nItemPopupHeigth = 0;

		/// <summary>
		/// Text shown in collapsed client area of GroupBar.
		/// </summary>
		private string m_sCollapsedText = c_sDefaultCollapsedText;

		/// <summary>
		/// Collapse button's state.
		/// </summary>
		private ButtonsState m_collapseButtonState = ButtonsState.None;

		/// <summary>
		/// Collapsed client area's state.
		/// </summary>
		private ButtonsState m_collapsedClientAreaState = ButtonsState.None;

		/// <summary>
		/// Collapse button's image in expanded state.
		/// </summary>
		private Image m_collapseImage = s_defaultCollapseImage;

		/// <summary>
		/// Collapse button's image in collapsed state.
		/// </summary>
		private Image m_expandImage = s_defaultExpandImage;

		/// <summary>
		/// Collapse button's tooltip, when control is expanded.
		/// </summary>
		private string m_sMinimizeButtonToolTip = s_sMinimizeButtonToolTip;

		/// <summary>
		/// Group Bar  button's tooltip.
		/// </summary>
		private string m_sExpandButtonToolTip = s_sExpandButtonToolTip;

		/// <summary>
        /// GroupBarDropDownTooltip.
        /// </summary>
        private string m_sGroupBarDropDownToolTip = s_sDropDownToolTip;


		/// <summary>
		/// Navigation pane's tooltip.
		/// </summary>
		private string m_sNavigationPaneTooltip = s_sNavigationPaneTooltip;

		/// <summary>
		/// <see cref="GroupBarItem"/>'s popup.
		/// </summary>
		private GroupBarItemPopup m_itemPopup;

		/// <summary>
		/// Indicates whether item popup is allowed to be opened.
		/// </summary>
		private bool m_bAllowItemPopup = false;

		/// <summary>
		/// Image list for navigation menu in collapsed mode.
		/// </summary>
		private ImageList m_ilNavMenu;

		/// <summary>
		/// Image list for navigation menu for add/remove sub menu.
		/// </summary>
		private ImageList m_ilAddRemove;

		/// <summary>
		/// Size of the popup for GroupBarItem client.
		/// </summary>
		private Size m_popupClientSize = Size.Empty;

		/// <summary>
		/// <see cref="GroupBarItem"/> item to its popup size mapping.
		/// </summary>
		private Hashtable m_htPopupSize;

		/// <summary>
		/// Indicates whether to show <see cref="GroupBarItem"/> popup's gripper.
		/// </summary>
		private bool m_bShowPopupGripper = false;

		/// <summary>
		/// Popup's resize mode.
		/// </summary>
		private PopupResizeMode m_popupResizeMode = PopupResizeMode.Both;

		/// <summary>
		/// Indicates whether popup is shown to the left of <see cref="GroupBar"/>.
		/// </summary>
		private bool m_bPopupLocationFlippedX = false;

		/// <summary>
		/// Indicates whether popup is shown to the top of <see cref="GroupBar"/> 
		/// </summary>
		private bool m_bPopupLocationFlippedY = false;

		/// <summary>
		/// Indicates whether popup is closed after clicking on item.
		/// </summary>
		private bool m_bPopupAutoClose = false;

		#region Static

		/// <summary>
		/// <see cref="StringFormat"/> for drawing <see cref="CollapsedText"/>.
		/// </summary>
		private static StringFormat s_sfCollapsedText;

		/// <summary>
		/// Default collapse button's image in expanded state.
		/// </summary>
		private static Image s_defaultCollapseImage;

		/// <summary>
		/// Default collapse button's image in collapsed state.
		/// </summary>
		private static Image s_defaultExpandImage;

		/// <summary>
		/// Group Bar  button's tooltip.
		/// </summary>
		private static string s_sExpandButtonToolTip;

		/// <summary>
		/// Collapse button's tooltip, when <see cref="GroupBar"/> is expanded.
		/// </summary>
		private static string s_sMinimizeButtonToolTip;

		/// <summary>
		/// Collapsed client area's tooltip.
		/// </summary>
		private static string s_sNavigationPaneTooltip;

		/// <summary>
		/// Caption for add/remove parent menu item in navgation menu.
		/// </summary>
		private static string s_sAddRemoveButton;

		#endregion

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GroupBar"/> is collapsed.
		/// </summary>
		/// <remarks>
		/// Works only <see cref="GroupBar"/> is in stacked mode.
		/// </remarks>
		/// <seealso cref="GroupBar.AllowCollapse"/>
		[DefaultValue( false )]
		[Category( "Appearance" )]
		[Description( "Gets or sets a value indicating whether this GroupBar is collapsed." )]
		public bool Collapsed
		{
			get
			{
				return  m_bInitializing ? m_bCollapsedInitValue : m_bCollapsed;
			}
			set
			{
				if( m_bInitializing && m_bCollapsedInitValue != value )
				{
					m_bCollapsedInitValue = value;
				}
				else if( m_bCollapsed != value )
				{
					if( value )
					{
						if( !this.StackedMode )
						{
							throw new ArgumentException( "GroupBar can be collapsed only in stacked mode." );
						}

						if( m_nExpandedWidth <= 0 )
						{
							m_nExpandedWidth = this.Width;
						}

						if( this.AllowCollapse && OnStateChanging() )
						{
							m_bCollapsed = true;

							OnStateChanged();
						}
					}
					else if( OnStateChanging() )
					{
						m_bCollapsed = false;

						OnStateChanged();
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether <see cref="GroupBar"/> can be collapsed.
		/// </summary>
		/// <seealso cref="GroupBar.Collapsed"/>
		[DefaultValue( false )]
		[Category( "Behavior" )]
		[Description( "Gets or sets a value indicating whether GroupBar can be collapsed." )]
		public bool AllowCollapse
		{
			get
			{
				return m_bAllowCollapse;
			}
			set
			{
				if( m_bAllowCollapse != value )
				{
					if( !value )
					{
						this.Collapsed = false;
					}

					m_bAllowCollapse = value;

					if( this.activeItemsList.Count > 0 )
					{
						Invalidate( this.CollapseButtonBounds );
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the width of the collapsed <see cref="GroupBar"/>.
		/// </summary>
		[DefaultValue( GroupBar.c_nDefaultCollapsedWidth )]
		[Category( "Appearance" )]
		[Description( "Gets or sets the width of the collapsed GroupBar." )]
		public int CollapsedWidth
		{
			get
			{
				return m_nCollapsedWidth;
			}
			set
			{
				if( m_nCollapsedWidth != value )
				{
					if( value < c_nMinCollapsedWidth )
					{
						throw new ArgumentOutOfRangeException( "CollapsedWidth can't be less then 20." );
					}

					m_nCollapsedWidth = value;

					if( this.Collapsed )
					{
						this.Width = m_nCollapsedWidth;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the text shown in collapsed client area of <see cref="GroupBar"/>.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Gets or sets the text shown in collapsed client area of GroupBar." )]
		public string CollapsedText
		{
			get
			{
				return m_sCollapsedText;
			}
			set
			{
				if( m_sCollapsedText != value )
				{
					m_sCollapsedText = value;

					if( this.Collapsed )
					{
						this.Invalidate( this.rcClient );
					}
				}
			}
		}

		/// <summary>
		/// Resets the <see cref="CollapsedText"/> property to its default value.
		/// </summary>
		protected void ResetCollapsedText()
		{
			this.CollapsedText = c_sDefaultCollapsedText;
		}
        /// <summary>
        /// Resets the <see cref="ApplyDefaultVisualStyleColor"/> property to its default value.
        /// </summary>
        protected void ResetApplyDefaultVisualStyleColor()
        {
            this.ApplyDefaultVisualStyleColor = true;
        }
		/// <summary>
		/// Indicates whether <see cref="CollapsedText"/> property should be serialized.
		/// </summary>
		protected bool ShouldSerializeCollapsedText()
		{
			return (this.CollapsedText != c_sDefaultCollapsedText);
		}
        /// <summary>
        /// Indicates whether <see cref="ApplyDefaultVisualStyleColor"/> property should be serialized.
        /// </summary>
        protected bool ShoulSerailizeApplyDefaultVisualStyleColor()
        {
            return ApplyDefaultVisualStyleColor != true;
        }
		/// <summary>
		/// Gets or sets the image of the collapse button in expanded state.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Gets or sets the image of the collapse button in expanded state." )]
		public Image CollapseImage
		{
			get
			{
				return m_collapseImage;
			}
			set
			{
				if( m_collapseImage != value )
				{
					m_collapseImage = value;

					InvalidateCollapseButton();
				}
			}
		}

		/// <summary>
		/// Resets the <see cref="CollapseImage"/> property to its default value.
		/// </summary>
		protected void ResetCollapseImage()
		{
			this.CollapseImage = s_defaultCollapseImage;
		}

		/// <summary>
		/// Indicates whether <see cref="CollapseImage"/> property should be serialized.
		/// </summary>
		protected bool ShouldSerializeCollapseImage()
		{
			return this.CollapseImage != s_defaultCollapseImage;
		}

		private void InvalidateCollapseButton()
		{
			if( this.AllowCollapse && this.activeItemsList.Count > 0 )
			{
				Invalidate( this.CollapseButtonBounds );
			}
		}

		/// <summary>
		/// Gets or sets the image of the collapse button in collapsed state.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Gets or sets the image of the collapse button." )]
		public Image ExpandImage
		{
			get
			{
				return m_expandImage;
			}
			set
			{
				if( m_expandImage != value )
				{
					m_expandImage = value;

					InvalidateCollapseButton();
				}
			}
		}

		/// <summary>
		/// Resets the <see cref="CollapseImage"/> property to its default value.
		/// </summary>
		protected void ResetExpandImage()
		{
			this.ExpandImage = s_defaultExpandImage;
		}

		/// <summary>
		/// Indicates whether <see cref="CollapseImage"/> property should be serialized.
		/// </summary>
		protected bool ShouldSerializeExpandImage()
		{
			return this.ExpandImage != s_defaultExpandImage;
		}

		/// <summary>
		/// Gets or sets the initial size of the popup for <see cref="GroupBarItem"/> client.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Gets or sets the initial size of the popup for GroupBarItem client." )]
		[DefaultValue( "0,0" )]
		public Size PopupClientSize
		{
			get
			{
				return m_popupClientSize;
			}
			set
			{
				m_popupClientSize = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to show <see cref="GroupBarItem"/> popup's gripper.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Gets or sets a value indicating whether to show GroupBarItem popup's gripper." )]
		[DefaultValue( false )]
		public bool ShowPopupGripper
		{
			get
			{
				return m_bShowPopupGripper;
			}
			set
			{
				m_bShowPopupGripper = value;
			}
		}

		/// <summary>
		/// Gets or sets the popup's resize mode.
		/// </summary>
		[Category( "Appearance" )]
		[Description( "Gets or sets the popup's resize mode." )]
		[DefaultValue( PopupResizeMode.Both )]
		public PopupResizeMode PopupResizeMode
		{
			get
			{
				return m_popupResizeMode;
			}
			set
			{
				m_popupResizeMode = value;
			}
		}

		#region Tooltips

		/// <summary>
		/// Gets or sets the tooltip for collapse button, when control is expanded.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Gets or sets the tooltip for collapse button, when control is expanded." )]
		public string MinimizeButtonToolTip
		{
			get
			{
				return m_sMinimizeButtonToolTip;
			}
			set
			{
				m_sMinimizeButtonToolTip = value;
			}
		}

		/// <summary>
		/// Resets the <see cref="MinimizeButtonToolTip"/> property to its default value.
		/// </summary>
		protected void ResetMinimizeButtonToolTip()
		{
			this.MinimizeButtonToolTip = s_sMinimizeButtonToolTip;
		}

		/// <summary>
		/// Indicates whether <see cref="MinimizeButtonToolTip"/> property should be serialized.
		/// </summary>
		protected bool ShouldSerializeMinimizeButtonToolTip()
		{
			return this.MinimizeButtonToolTip != s_sMinimizeButtonToolTip;
		}

		/// <summary>
		/// Gets or sets the tooltip for collapse button, when control is collapsed.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Gets or sets the tooltip for collapse button, when control is collapsed." )]
		public string ExpandButtonToolTip
		{
			get
			{
				return m_sExpandButtonToolTip;
			}
			set
			{
				m_sExpandButtonToolTip = value;
			}
		}

        /// <summary>
        /// Indicates whether <see cref="ExpandButtonToolTip"/> property should be serialized.
        /// </summary>
        protected bool ShouldSerializeExpandButtonToolTip()
        {
            return this.ExpandButtonToolTip != s_sExpandButtonToolTip;
        }

        /// <summary>
        /// Resets the <see cref="ExpandButtonToolTip"/> property to its default value.
        /// </summary>
        protected void ResetExpandButtonToolTip()
        {
            this.ExpandButtonToolTip = s_sExpandButtonToolTip;
        }
       
        /// <summary>
        /// Gets or sets the tooltip for GroupBarDropDownToolTip.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets the tooltip for GroupBar button.")]
        public string GroupBarDropDownToolTip
        {
            get
            {
                return m_sGroupBarDropDownToolTip;
            }
            set
            {
                m_sGroupBarDropDownToolTip = value;
            }
        }

        /// <summary>
        /// Resets the <see cref="GroupBarDropDownToolTip"/> property to its default value.
        /// </summary>
        public void ResetGroupBarDropDownToolTip()
        {
            GroupBarDropDownToolTip = s_sDropDownToolTip;
        }

        /// <summary>
        /// Indicates whether <see cref="GroupBarDropDownToolTip"/> property should be serialized.
        /// </summary>
        protected bool ShouldSerializeGroupBarDropDownToolTip()
        {
            return this.GroupBarDropDownToolTip != s_sDropDownToolTip;
        }

		/// <summary>
		/// Gets or sets the navigation pane's tooltip.
		/// </summary>
		[Category( "Behavior" )]
		[Description( "Gets or sets the navigation pane's tooltip." )]
		public string NavigationPaneTooltip
		{
			get
			{
				return m_sNavigationPaneTooltip;
			}
			set
			{
				m_sNavigationPaneTooltip = value;
			}
		}

		/// <summary>
		/// Resets the <see cref="NavigationPaneTooltip"/> property to its default value.
		/// </summary>
		protected void ResetNavigationPaneTooltip()
		{
			this.NavigationPaneTooltip = s_sNavigationPaneTooltip;
		}

		/// <summary>
		/// Indicates whether <see cref="MinimizeButtonToolTip"/> property should be serialized.
		/// </summary>
		protected bool ShouldSerializeNavigationPaneTooltip()
		{
			return this.NavigationPaneTooltip != s_sNavigationPaneTooltip;
		}

		private string CollapseButtonTooltip
		{
			get
			{
				return this.Collapsed ? this.ExpandButtonToolTip : this.MinimizeButtonToolTip;
			}
		}

		#endregion

		#region Popup

		private bool ItemPopupOpened
		{
			get
			{
				return this.ItemPopup.Opened;
			}
		}

		private GroupBarItemPopup ItemPopup
		{
			get
			{
				if( null == m_itemPopup )
				{
					m_itemPopup = new GroupBarItemPopup( this );

					m_itemPopup.OpenedChanged += new EventHandler( ItemPopupOpenedChanged );
					m_itemPopup.BeforeClose += new MouseClickCancelEventHandler( ItemPopupBeforeClose );
				}

				return m_itemPopup;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether popup is closed after clicking on item.
		/// </summary>
		[Description( "Indicates whether popup is closed after clicking on item." )]
		[Category( "Behavior" )]
		[DefaultValue( false )]
		public bool PopupAutoClose
		{
			get
			{
				return m_bPopupAutoClose;
			}
			set
			{
				m_bPopupAutoClose = value;
			}
		}

		[Browsable( false )]
		[DefaultValue( 0 )]
		[EditorBrowsable( EditorBrowsableState.Never )]
		public int ExpandedWidth
		{
		    get
		    {
		        return m_nExpandedWidth;
		    }
			set
			{
				m_nExpandedWidth = value;
			}
		}

		#endregion

		#endregion

		#region Events

		/// <summary>
		/// Occurs when <see cref="GroupBar.Collapsed"/> property is changed.
		/// </summary>
		[Description( "Occurs when Collapsed property is changed." )]
		public event EventHandler StateChanged;

		/// <summary>
		/// Occurs when <see cref="GroupBar.Collapsed"/> property is about to be changed.
		/// </summary>
		/// <remarks>Can cancel state changing.</remarks>
		[Description( "Occurs when Collapsed property is about to be changed." )]
		public event CancelEventHandler StateChanging;

		/// <summary>
		/// Provides data for the <see cref="GroupBar.BeforePopup"/> event.
		/// </summary>
		/// <remarks>The collapsed <see cref="GroupBar"/> control uses the <see cref="GroupBar.BeforePopup"/> events to notify users,
		/// that <see cref="GroupBarItem"/>'s popup is about to be shown.
		/// <seealso cref="BeforePopupEventHandler"/>	
		/// </remarks>
		public class BeforePopupEventArgs:
			CancelEventArgs
		{
			#region Fields

			/// <summary>
			/// Item that popup.
			/// </summary>
			private GroupBarItem m_item = null;

			/// <summary>
			/// Popup's bounds.
			/// </summary>
			private Rectangle m_popupBounds = Rectangle.Empty;

			/// <summary>
			/// Indicates whether popup is shown to the left of <see cref="GroupBar"/>.
			/// </summary>
			private bool m_bFlippedX;

			/// <summary>
			/// Indicates whether popup is shown to the top of <see cref="GroupBar"/> 
			/// </summary>
			private bool m_bFlippedY;
			
			#endregion			

			#region Construction

			/// <summary>
			/// Initializes a new instance of the <see cref="BeforePopupEventArgs"/> class.
			/// </summary>
			/// <param name="item">The <see cref="GroupBarItem"/> item.</param>
			/// <param name="popupBounds">The popup's bounds.</param>
			/// <param name="cancel"><c>true</c> to cancel the event; otherwise, <c>false</c>.</param>
			public BeforePopupEventArgs( GroupBarItem item, Rectangle popupBounds, bool cancel, bool flippedX, bool flippedY ):
				base( cancel )
			{
				m_item = item;
				m_popupBounds = popupBounds;
				m_bFlippedX = flippedX;
				m_bFlippedY = flippedY;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="BeforePopupEventArgs"/> class.
			/// </summary>
			/// <param name="item">The <see cref="GroupBarItem"/> item.</param>
			/// <param name="popupBounds">The popup's bounds.</param>
			public BeforePopupEventArgs( GroupBarItem item, Rectangle popupBounds, bool flippedX, bool flippedY ):
				this( item, popupBounds, false, flippedX, flippedY )
			{
			}

			#endregion

			#region Properties

			/// <summary>
			/// The <see cref="GroupBarItem"/> that popups.
			/// </summary>
			public GroupBarItem Item
			{
				get
				{
					return m_item;
				}
			}

			/// <summary>
			/// Gets or sets the popup's bounds.
			/// </summary>
			public Rectangle PopupBounds
			{
				get
				{
					return m_popupBounds;
				}
				set
				{
					m_popupBounds = value;
				}
			}

			/// <summary>
			/// Indicates whether popup is shown to the left of <see cref="GroupBar"/>.
			/// </summary>
			public bool FlippedX
			{
				get { return m_bFlippedX; }
			}

			/// <summary>
			/// Indicates whether popup is shown to the top of <see cref="GroupBar"/> 
			/// </summary>
			public bool FlippedY
			{
				get { return m_bFlippedY; }
			}	

			#endregion
		}

		/// <summary>
		/// Represents the method that will handle the <see cref="GroupBar.BeforePopup"/> event in the <see cref="GroupBar"/> control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="args">A <see cref="BeforePopupEventArgs"/> that contains the event data.</param>	
		public delegate void BeforePopupEventHandler( object sender, BeforePopupEventArgs args );

		/// <summary>
		/// Occurs when before <see cref="GroupBarItem"/>'s popup is shown.
		/// </summary>
		/// <remarks>Can cancel popup showing.</remarks>
		[Description( "Occurs when before GroupBarItem's popup is shown." )]
		public event BeforePopupEventHandler BeforePopup;

		#endregion

		#region Implementation

		#region State manipulation

		private GroupBarItem SelectedGroupBarItem
		{
			get
			{
				GroupBarItem selectedItem = null;

				if( this.SelectedItem >= 0 && this.SelectedItem < this.activeItemsList.Count )
				{
					selectedItem = this.activeItemsList[this.SelectedItem] as GroupBarItem;
				}

				return selectedItem;
			}
		}

		internal GroupBarItem SelItem
		{
			get
			{
				GroupBarItem selectedItem = null;

				if( this.SelectedItem >= 0 && this.SelectedItem < this.activeItemsList.Count )
				{
					selectedItem = this.activeItemsList[this.SelectedItem] as GroupBarItem;
				}

				return selectedItem;
			}
		}

		internal Control SelectedItemClient
		{
			get
			{
				GroupBarItem selectedItem = this.SelItem;
				Control client = null;

				if( selectedItem != null )
				{
					client = selectedItem.Client;
				}

				return client;
			}
		}

		private bool OnStateChanging()
		{
			bool bResult = true;

			if( null != this.StateChanging )
			{
				CancelEventArgs e = new CancelEventArgs();

				this.StateChanging( this, e );

				bResult = !e.Cancel;
			}

			return bResult;
		}

		private void OnStateChanged()
		{
			if( this.Collapsed )
			{
				m_nExpandedWidth = this.Width;

				this.Width = m_nCollapsedWidth;
			}
			else
			{
				this.Width = m_nExpandedWidth;
			}

			Control client = this.SelectedItemClient;

			if( null != client )
			{
				client.Visible = !this.Collapsed;
			}

			if( null != this.StateChanged )
			{
				this.StateChanged( this, EventArgs.Empty );
			}
		}

		#endregion

		#region Drawing

		private void DrawCollapsedClientArea( Graphics gph, bool bIsMirrored, Color bordercolor )
		{
			DrawCollapsedClientAreaBackground( gph );

			if( m_collapsedClientAreaState == ButtonsState.None )
			{
				Rectangle client = this.rcClient;
				client.Height--;
				client.Y++;

				DrawHeaderLines( gph, bIsMirrored, client );
			}

			using( Pen pen = new Pen( bordercolor ) )
			{
				gph.DrawLine( pen, rcClient.Left, rcClient.Top, rcClient.Right, rcClient.Top );
			}

			DrawCollapsedClientAreaText( gph, bIsMirrored );
		}

		private void DrawCollapsedClientAreaBackground( Graphics gph )
		{
			Color bgColor;

			if( this.ItemPopupOpened )
			{
				bgColor = Office2007BlueColors.Default.FloatHighlightButtonBorderColor;
			}
			else
			{
				switch( m_collapsedClientAreaState )
				{
					case ButtonsState.None:
						bgColor = this.ClientAreaBackground;
						break;

                    case ButtonsState.DropDownButtonHot:
                        {
                            if (this.VisualStyle == VisualStyle.Office2010)
                                bgColor = m_office2010ColorTable.GroupBarHighlightColorLight;
                            else if (this.VisualStyle == VisualStyle.Metro)
                                bgColor = ControlPaint.LightLight(HeaderBackColor);
                            else
                                bgColor = Office2007BlueColors.Default.FloatHighlightButtonColor;
                        }
                        break;

                    case ButtonsState.DropDownButtonPushed:
                        {
                            if (this.VisualStyle == VisualStyle.Office2010)
                                bgColor = m_office2010ColorTable.GroupBarHighlightColorDark;
                            else if (this.VisualStyle == VisualStyle.Metro)
                                bgColor = ControlPaint.Light(HeaderBackColor);
                            else
                                bgColor = Office2007BlueColors.Default.FloatPressCloseButtonColor;
                        }
                        break;

					default:
						return;
				}
			}

            if (this.VisualStyle == VisualStyle.Office2010)
            {
                using (Brush brush = new SolidBrush(bgColor))
                {
                    Pen spPen = new Pen(this.BorderColor, 1);
                    gph.FillRectangle(brush, this.rcClient);
                    gph.DrawRectangle(spPen, new Rectangle(this.rcClient.X + 1, this.rcClient.Y - 1, this.rcClient.Width - 1, this.rcClient.Height - 1));
                    spPen.Dispose();
                }
            }
            else
            {
                using (Brush brush = new SolidBrush(bgColor))
                {
                    gph.FillRectangle(brush, this.rcClient);
                }
            }
		}

		internal Color ClientAreaBackground
		{
			get
			{
				Color color;

				if( this.vStyle == VisualStyle.Office2003 )
				{
					color = Office2003Colors.GroupBarHeaderColorDark;
				}
				else if( this.vStyle == VisualStyle.Office2007 )
				{
					color = m_office2007ColorTable.GroupBarClientAreaBackground;
				}
                else if (this.vStyle == VisualStyle.Office2010)
                {
                    color = m_office2010ColorTable.GroupBarClientAreaBackground;
                }
				else
				{
					color = this.HeaderBackColor;
				}

				return color; 
			}
		}

		private void DrawCollapsedClientAreaText( Graphics gph, bool bIsMirrored )
		{
			Color color = this.VisualStyle != VisualStyle.Office2007 ?
				this.HeaderForeColor : m_office2007ColorTable.XPTaskBarBoxForeColor;

            Font font = new Font("Segoe UI", 12, FontStyle.Bold);
            if (EnableTouchMode)
                font = new Font("Segoe UI", 12 + 3, FontStyle.Bold);
            using (Brush brush = new SolidBrush(color))
            {
                int angle = 0;
                if (bIsMirrored)
                {
                    if (!PopupRightToLeft)
                        angle = 0;
                    else
                        angle = 180;
                }
                else
                {
                    if (!PopupRightToLeft)
                        angle = 180;
                    else
                        angle = 0;
                }
                RotatePaint.DrawRotatedString(gph, this.CollapsedText, font, brush, this.rcClient, this.CollapsedTextSF,
                    angle);

			}
		}

		private StringFormat CollapsedTextSF
		{
			get
			{
				if( null == s_sfCollapsedText )
				{
					StringFormat sf = new StringFormat( StringFormat.GenericTypographic );
					sf.Alignment = StringAlignment.Center;
					sf.LineAlignment = StringAlignment.Center;
					sf.FormatFlags = StringFormatFlags.DirectionVertical | StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
					sf.Trimming = StringTrimming.EllipsisCharacter;

					s_sfCollapsedText = sf;
				}

				if( this.RightToLeft == RightToLeft.Yes )
				{
					s_sfCollapsedText.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
				}
				else
				{
					s_sfCollapsedText.FormatFlags &= ~StringFormatFlags.DirectionRightToLeft;
				}

				return s_sfCollapsedText;
			}
		}

		private Rectangle CollapseButtonBounds
		{
			get
			{
				Size size = new Size( c_nCollapseButtonWidth, c_nCollapseButtonHeight );
				int x = (this.RightToLeft != RightToLeft.Yes) ?
					(this.Width - size.Width - c_nCollapseButtonPadRight) : c_nCollapseButtonPadRight;
				Point location = new Point( x, c_nCollapseButtonPadTop );

				return new Rectangle( location, size );
			}
		}

		private Image _CollapseImage
		{
			get
			{
				return (this.RightToLeft != RightToLeft.Yes) ? m_collapseImage : m_expandImage;
			}
		}

		private Image _ExpandImage
		{
			get
			{
				return (this.RightToLeft == RightToLeft.Yes) ? m_collapseImage : m_expandImage;
			}
		}

		private void DrawHeaderCollapsed( Graphics gph, bool bIsMirrored, Rectangle rcheader )
		{
		}

		private void DrawCollapseButton( Graphics gph, bool bIsMirrored )
		{
			DrawCollapseButtonBackground( gph, bIsMirrored );

			Image image = this.Collapsed ? this._ExpandImage : this._CollapseImage;

			if( image != null )
			{
				Rectangle bounds = this.CollapseButtonBounds;
				Size imgSize = image.Size;
				Size deflateSize = Size.Empty;

				if( imgSize.Width < bounds.Width )
				{
					deflateSize.Width = (imgSize.Width - bounds.Width) / 2;
				}
				if( imgSize.Height < bounds.Height )
				{
					deflateSize.Height = (imgSize.Height - bounds.Height) / 2;
				}

				if( deflateSize != Size.Empty )
				{
					bounds.Inflate( deflateSize );
				}

				gph.DrawImage( image, bounds );
			}
		}

		private void DrawCollapseButtonBackground( Graphics gph, bool bIsMirrored )
		{
			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.UseThemedDrawing )
			{
				DrawCollapseButtonThemed( gph );
			}
			else
			{
				switch( this.vStyle )
				{
                    case VisualStyle.Office2010:
					case VisualStyle.Office2007:
					case VisualStyle.Office2003:
						DrawCollapseButtonOffice200X( gph );
						break;

					default:
						DrawCollapseButtonOfficeXP( gph, bIsMirrored );
						break;
				}
			}
		}

		private void DrawCollapseButtonOfficeXP( Graphics gph, bool bIsMirrored )
		{
			Rectangle bounds = this.CollapseButtonBounds;

			using( CMirroredDrawer mdButton = new CMirroredDrawer( gph, bounds, bIsMirrored ) )
			{
				Graphics gfxVirt = mdButton.VirtualGfx;
				Rectangle rectVirt = mdButton.VirtualBounds;
				ButtonState bsButtonState = (ButtonsState.DropDownButtonPushed == m_collapseButtonState ?
					ButtonState.Pushed : ButtonState.Normal);

				ControlPaint.DrawButton( gfxVirt, rectVirt, bsButtonState | (this.bFlatLook ? ButtonState.Flat : 0) );

				if( m_collapseButtonState != ButtonsState.None )
				{
					Rectangle rchighlight = new Rectangle( rectVirt.Left, rectVirt.Top, rectVirt.Width-1, rectVirt.Height-1 );
                    if (this.VisualStyle == VisualStyle.Metro)
					    ControlPaint.DrawBorder3D( gfxVirt, rchighlight, Border3DStyle.SunkenOuter, Border3DSide.Bottom|Border3DSide.Right );
                    else
                        ControlPaint.DrawBorder3D(gfxVirt, rchighlight, Border3DStyle.Raised, Border3DSide.Bottom | Border3DSide.Right);
				}
			}
		}

		private void DrawCollapseButtonOffice200X( Graphics gph )
		{
			Rectangle bounds = this.CollapseButtonBounds;
			LinearGradientBrush lgb = null;

			if( m_collapseButtonState == ButtonsState.DropDownButtonPushed )
			{
                if (this.VisualStyle == VisualStyle.Office2010)
                    lgb = new LinearGradientBrush(bounds, m_office2010ColorTable.GroupBarHighlightColorDark,
                    m_office2010ColorTable.GroupBarHighlightColorLight, LinearGradientMode.Vertical);
                else
                    lgb = new LinearGradientBrush(bounds, Office2007BlueColors.Default.GroupBarHighlightColorDark,
                Office2007BlueColors.Default.GroupBarHighlightColorLight, LinearGradientMode.Vertical);
			}
			else if( m_collapseButtonState == ButtonsState.DropDownButtonHot )
			{
                if (this.VisualStyle == VisualStyle.Office2010)
                    lgb = new LinearGradientBrush(bounds, m_office2010ColorTable.GroupBarHighlightColorLight,
                    m_office2010ColorTable.GroupBarHighlightColorDark, LinearGradientMode.Vertical);
                else
				    lgb = new LinearGradientBrush( bounds, Office2007BlueColors.Default.GroupBarHighlightColorLight,
					Office2007BlueColors.Default.GroupBarHighlightColorDark, LinearGradientMode.Vertical );
			}

			if( null != lgb )
			{
				lgb.WrapMode = WrapMode.TileFlipX;

				gph.FillRectangle( lgb, bounds );

				lgb.Dispose();
			}
		}

		private void DrawCollapseButtonThemed( Graphics gph )
		{
			Rectangle bounds = this.CollapseButtonBounds;

			switch( m_collapseButtonState )
			{
				case ButtonsState.DropDownButtonHot:
					this.tdButton.DrawThemeBackground( gph, ThemeParts.BP_PUSHBUTTON, ThemeStates.PBS_HOT, bounds );
					break;

				case ButtonsState.DropDownButtonPushed:
					this.tdButton.DrawThemeBackground( gph, ThemeParts.BP_PUSHBUTTON, ThemeStates.PBS_PRESSED, bounds );
					break;

				default:
					this.tdButton.DrawThemeBackground( gph, ThemeParts.BP_PUSHBUTTON, ThemeStates.PBS_NORMAL, bounds );
					break;
			}
		}

		#endregion

		private void HandleCollapsedClientAreaClick()
		{
			ShowItemPopup();
		}

		private Rectangle GetPopupBounds( GroupBarItem item	)
		{
			int nBorderInc = 2 * GroupBarItemPopup.BorderWidth;

			if( m_nItemPopupHeigth <= 0 )
			{
				m_nItemPopupHeigth = this.rcClient.Height;
			}

			Size size = GetPopupSize( item );

			size.Height += nBorderInc;

			if( this.ShowPopupGripper )
			{
				size.Height += GroupBarItemPopup.GripperHeight - GroupBarItemPopup.BorderWidth;
			}

			size.Width += nBorderInc;

			Point location = this.Location;

			location = PointToScreen( Point.Empty );

			if( this.RightToLeft != RightToLeft.Yes )
			{
                if (!PopupRightToLeft)
                {
                    location.Offset(this.Width - 1, this.HeaderHeight + 1);	// 1 horz pixel overlap over GroupBar; 1 vert pixel for bottom header line.
                    m_bPopupLocationFlippedX = false;
                }
                else
                {
                    location.Offset(1 - size.Width, this.HeaderHeight + 1);
                    m_bPopupLocationFlippedX = true;
                }
			}
			else
			{
                if (!PopupRightToLeft)
                {
                    location.Offset(1 - size.Width, this.HeaderHeight + 1);	// 1 horz pixel overlap over GroupBar; 1 vert pixel for bottom header line.
                    m_bPopupLocationFlippedX = true;
                }
                else
                {
                    location.Offset(this.Width - 1, this.HeaderHeight + 1);
                    m_bPopupLocationFlippedX = false;
                }
			}

			m_bPopupLocationFlippedY = false;

			Rectangle popupBounds = new Rectangle( location, size );
			Rectangle workArea = Screen.GetWorkingArea( this );

			popupBounds.Intersect( workArea );

			if( popupBounds.Width < 3 * GroupBarItemPopup.BorderWidth )
			{
				int nOffset = size.Width + this.Width;

				if( this.RightToLeft != RightToLeft.Yes )
				{
					location.X -= nOffset;
					m_bPopupLocationFlippedX = true;
				}
				else
				{
					location.X += nOffset;
					m_bPopupLocationFlippedX = false;
				}

				popupBounds = new Rectangle( location, size );
			}

			popupBounds.Intersect( workArea );

			if( popupBounds.Height < 3 * GroupBarItemPopup.BorderWidth )
			{
				location.Y -= size.Height;
				m_bPopupLocationFlippedY = true;

				popupBounds = new Rectangle( location, size );
			}

			popupBounds.Intersect( workArea );

			return popupBounds;
		}

		private Size GetPopupSize( GroupBarItem item )
		{
			Size size;

			if( m_htPopupSize != null && m_htPopupSize.Contains( item ) )
			{
				size = (Size)m_htPopupSize[item];
			}
			else if( this.PopupClientSize != Size.Empty )
			{
				size = this.PopupClientSize;
			}
			else
			{
				size = new Size( m_nExpandedWidth, m_nItemPopupHeigth );
			}

			return size;
		}

		private void ShowItemPopup()
		{
			ShowItemPopup( null );
		}

        /// <summary>
        /// Shows the item popup when the Group bar is Collapsed
        /// </summary>
        /// <param name="barItem">
        /// GroupBar Item for which Popup to be shown
        /// </param>
		public void ShowItemPopup( GroupBarItem barItem )
		{
			if( this.DesignMode || null == this.SelectedItemClient )
			{
				HidePopup( barItem );
				return;
			}

			if( null != barItem && this.ItemPopupOpened )
			{
				HideItemClient( barItem );
			}

			GroupBarItem item = this.SelectedGroupBarItem;

			if( null != item )
			{
				if( !this.ItemPopupOpened )
				{
					BeforePopupEventArgs args = OnBeforePopup( item );

					if( !args.Cancel )
					{
						this.ItemPopup.Show( true, args );
					}
				}
				else
				{
					if( null != barItem )
					{
						TryShowItemClient( item );
					}
					else
					{
						this.ItemPopup.Hide();
					}
				}
			}
		}

		private void HidePopup( GroupBarItem prevSelected )
		{
			if( this.ItemPopupOpened )
			{
				if( null != prevSelected )
				{
					HideItemClient( prevSelected );
				}

				this.ItemPopup.Hide();
			}
		}

		/// <summary>
		/// Hides the <see cref="GroupBarItem"/>'s popup.
		/// </summary>
		public void HidePopup()
		{
			HidePopup( null );
		}

		private void TryShowItemClient( GroupBarItem item )
		{
			BeforePopupEventArgs args = OnBeforePopup( item );

			if( !args.Cancel )
			{
				ShowItemClient( item, args.PopupBounds );
			}
		}

		private BeforePopupEventArgs OnBeforePopup( GroupBarItem item )
		{
			Rectangle popupBounds = GetPopupBounds( item );
			BeforePopupEventArgs args = new BeforePopupEventArgs( item, popupBounds, m_bPopupLocationFlippedX, m_bPopupLocationFlippedY );

			if( null != this.BeforePopup )
			{
				this.BeforePopup( this, args );
			}

			return args;
		}

		private void HandleCollapseButtonClick()
		{
			this.Collapsed = !this.Collapsed;
		}

		private void ItemPopupOpenedChanged( object sender, EventArgs e )
		{
			GroupBarItem item = this.SelItem;

			if( this.ItemPopupOpened )
			{
				ShowItemClient( item, Rectangle.Empty );
			}
			else
			{
				HideItemClient( item );
				StorePopupSize( item );			
			}

			Invalidate( this.rcClient );
		}

		private void StorePopupSize( GroupBarItem item )
		{
			if( null == m_htPopupSize )
			{
				m_htPopupSize = new Hashtable();
			}

			Size size = this.ItemPopup.ClientSize;

			if( !size.IsEmpty )
			{
				m_htPopupSize[item] = this.ItemPopup.ClientSize;
			}
		}

		private void HideItemClient( GroupBarItem item )
		{
			if( this.ItemPopupOpened )
			{
				StorePopupSize( item );
			}

			Control client = item.Client;

			if( null != client )
			{
				client.Visible = false;
				client.Dock = DockStyle.None;
				client.Parent = this;
			}
		}

		private void ShowItemClient( GroupBarItem item, Rectangle popupBounds )
		{
			Control client = item.Client;

			if( null != client )
			{
				client.Parent = this.ItemPopup;
				client.Dock = DockStyle.Fill;
				client.Visible = true;

				this.ItemPopup.Bounds = !popupBounds.IsEmpty ? popupBounds : GetPopupBounds( item );
			}
		}

		void ItemPopupBeforeClose( object sender, MouseClickCancelEventArgs e )
		{
			if( e.MouseClickPoint != Point.Empty )
			{
				Point pt = this.PointToClient( e.MouseClickPoint );

				if( this.ItemPopupOpened )
				{
					e.Cancel = this.rcClient.Contains( pt );

					if( !e.Cancel )
					{
						for( int i=0; i < this.activeItemsList.Count; i++ )
						{
							Rectangle rcbar = GetGroupBarItemBounds( i );

							if( this.bIntegratedScrolling && ( i == this.nSelectedItem || i == (this.nSelectedItem + 1) ) )
							{
								CorrectRectForScrollThumbWidth( ref rcbar );
							}

                            if (this.ItemPopup.ContainsFocus || rcbar.Contains(pt))
							{
								e.Cancel = !this.PopupAutoClose;
								break;
							}
						}
					}
				}
			}

			if( !e.Cancel )
			{
				GroupBarItem item = this.SelItem;

				StorePopupSize( item );

				Size size = GetPopupSize( item );

				m_nExpandedWidth = size.Width;
				m_nItemPopupHeigth = size.Height;				

				HideItemClient( this.SelItem );
			}
		}

		private void InitCollapsedNavMenu( IContextMenuProvider menu )
		{
			ArrayList items = this.alNavPaneItems;

			if( items.Count > 0 )
			{
				AddContextMenuItems( menu, items, null, ref m_ilNavMenu, true );
			}
		}

		private void AddContextMenuItems( IContextMenuProvider menu, IList items, string parent, ref ImageList ilNavMenu, bool bAddSeparator )
		{
			if( null == ilNavMenu )
			{
				ilNavMenu = new ImageList();
			}
			else
			{
				ilNavMenu.Images.Clear();
			}

			ImageList.ImageCollection images = ilNavMenu.Images;
			bool bSeparatorAdded = false;

			foreach( GroupBarItem item in items )
			{
				Image image = null;

				if( item.LargeImageMode )
				{
					image = item.Image;
				}
				else if( null != item.Icon )
				{
					image = ImageListAdv.IconToImageAlphaCorrect( item.Icon );
				}

				if( null != image )
				{
					images.Add( image );
				}

				string text = item.Text;

				if( parent != null && parent.Length > 0 )
				{
					text = text.Insert( 0, " " );

					menu.AddContextMenuItem( parent, text, new EventHandler( NavMenuItemHandler ) );

					if( item.Visible )
					{
						menu.SetContextMenuItemChecked( text, true );
					}
				}
				else
				{
					menu.AddContextMenuItem( text, new EventHandler( NavMenuItemHandler ) );
				}

				if( null != image )
				{
					menu.SetContextMenuItemImage( text, ilNavMenu, (images.Count - 1) );
				}

				if( bAddSeparator && !bSeparatorAdded )
				{
					menu.SetContextMenuItemSeparator( item.Text, true );
					bSeparatorAdded = true;
				}
			}
		}

		private void InitAddRemoveNavMenu( IContextMenuProvider menu )
		{
			menu.AddContextMenuItem( s_sAddRemoveButton, null );

			AddContextMenuItems( menu, this.GroupBarItems, s_sAddRemoveButton, ref m_ilAddRemove, false );
		}

		private void NavMenuItemHandler( object sender, EventArgs e )
		{
			ContextMenuItem menuItem = (ContextMenuItem)sender;
			string sText = menuItem.ContextMenuItemText;
			GroupBarItem itemHit = FindItemByText( sText );
			
			if( null != itemHit )
			{
				SelectItem( this.GroupBarItems.IndexOf( itemHit ), this.nSelectedItem );
			}
			else if( sText.StartsWith( " " ) )
			{
				string sItemText = sText.Substring( 1 );
				itemHit = FindItemByText( sItemText );

				if( null != itemHit )
				{
					IContextMenuProvider menu = menuItem.ContextMenuProvider;

					itemHit.Visible = !menu.GetContextMenuItemChecked( sText ); 
				}
			}
		}

		private GroupBarItem FindItemByText( string sText )
		{
			GroupBarItem itemHit = null;

			foreach( GroupBarItem item in this.GroupBarItems )
			{
				if( item.Text == sText )
				{
					itemHit = item;
					break;
				}
			}
			return itemHit;
		}

		#endregion

		#region Message handlers
		
		private void OnCaptureChanged()
		{
			if( this.Collapsed )
			{
				m_collapsedClientAreaState = ButtonsState.None;
				base.Cursor = this.gDefaultCursor;

				Invalidate( this.rcClient );
			}
		}

		#endregion

		#region Overrides

		protected override void WndProc( ref Message m )
		{
			switch( m.Msg )
			{
				case NativeMethods.WM_CAPTURECHANGED:
					OnCaptureChanged();
					break;
			}

			base.WndProc( ref m );
		}

		#endregion

		#endregion
	}

	/// <summary>
	/// ControlAccessibleObject derived class that implements the Accessibility object for the GroupBar control.
	/// </summary>
	public class GroupBarControlAccessibleObject: Control.ControlAccessibleObject
	{
		protected GroupBar ctrlGroupBar;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GroupBarControlAccessibleObject"/> class.
		/// </summary>
		public GroupBarControlAccessibleObject( GroupBar gbctrl )
			: base( gbctrl )
		{
			this.ctrlGroupBar = gbctrl;
		}

		// Gets the role for the Group. This is used by accessibility programs.
		public override AccessibleRole Role
		{
			get { return AccessibleRole.Grouping; }
		}

		public override string Name
		{
			get { return this.ctrlGroupBar.AccessibleName; }
		}

		public override Rectangle Bounds
		{
			get { return this.ctrlGroupBar.RectangleToScreen( this.ctrlGroupBar.ClientRectangle ); }
		}

		public override string Description
		{
			get { return this.ctrlGroupBar.AccessibleDescription; }
		}

		public override string Help
		{
			get { return String.Empty; }
		}

		// Gets the state for the GroupView. This is used by accessibility programs.
		public override AccessibleStates State
		{
			get { return AccessibleStates.None; }
		}

		public override string Value
		{
			get { return this.ctrlGroupBar.Text; }
			set { this.ctrlGroupBar.Text = value; }
		}

		public override int GetChildCount()
		{
			// Return the number.
			if( this.ctrlGroupBar.StackedMode == false )
				return this.ctrlGroupBar.activeItemsList.Count+1;	// GroupBarItems + selected client.
			else
				return this.ctrlGroupBar.activeItemsList.Count+2;	// GroupBarItems + selected client = drop-down button.
		}

		// Gets the Accessibility object of the GroupBarItem identified by index.
		public override AccessibleObject GetChild( int index )
		{
			if( index >= 0 )
			{
				int itemcount = this.ctrlGroupBar.activeItemsList.Count;
				if( index < itemcount )
				{
					return this.ctrlGroupBar.ItemAccessibleObjects[index];
				}
				else
				{
					if( this.ctrlGroupBar.StackedMode == false )
					{
						if( index == itemcount )
							return this.ctrlGroupBar.GroupBarItems[ctrlGroupBar.SelectedItem].Client.AccessibilityObject;
					}
					else
					{
						if( index == itemcount )
							return new DropDownButtonAccessibleItem( this.ctrlGroupBar );
						else if( index == itemcount+1 )
							return this.ctrlGroupBar.GroupBarItems[ctrlGroupBar.SelectedItem].Client.AccessibilityObject;
					}
				}
			}
			return null;
		}

		// This function is used by the GroupBarItemAccessibleObject.Navigate function.
		public AccessibleObject NavigateFromChild( GroupBarItemAccessibleObject child, AccessibleNavigation navdir )
		{
			int index = child.Index;
			switch( navdir )
			{
				case AccessibleNavigation.FirstChild:
					index = 0;
					break;

				case AccessibleNavigation.LastChild:
					index = this.ctrlGroupBar.activeItemsList.Count-1;
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
					if( index < this.ctrlGroupBar.activeItemsList.Count )
						index++;
					else if( this.ctrlGroupBar.StackedMode == true )
						return new DropDownButtonAccessibleItem( this.ctrlGroupBar );
					break;
			}

			return this.GetChild( index );
		}

		// This function is used by the GroupBarItemAccessibleObject.Select function.
		public void SelectChild( GroupBarItemAccessibleObject child, AccessibleSelection selection )
		{
			if( (selection & AccessibleSelection.TakeSelection) != 0 )
				this.ctrlGroupBar.SelectedItem = child.Index;
		}

		public override AccessibleObject GetSelected()
		{
			int nitemselected = this.ctrlGroupBar.SelectedItem;
			if( nitemselected != -1 )
				return this.GetChild( nitemselected );
			return base.GetSelected();
		}

		public override AccessibleObject HitTest( int x, int y )
		{
			Point pt = this.ctrlGroupBar.PointToClient( new Point( x, y ) );
			if( this.ctrlGroupBar.StackedMode == true )
			{
				if( this.ctrlGroupBar.GetDropDownButtonRectangle().Contains( pt ) == true )
					return new DropDownButtonAccessibleItem( this.ctrlGroupBar );
			}
			int nitemcount = this.ctrlGroupBar.activeItemsList.Count;
			for( int i=0; i<nitemcount; i++ )
			{
				if( this.ctrlGroupBar.GetGroupBarItemBounds( i ).Contains( pt ) )
					return this.ctrlGroupBar.ItemAccessibleObjects[i];
			}
			return base.HitTest( x, y );
		}
	}

	public class GroupBarItemAccessibleObject: AccessibleObject
	{
		GroupBar ctrlGroupBar;
		GroupBarItem itemGroupBar;
		int nIndex;

		public GroupBarItemAccessibleObject( GroupBarItem gbitem )
		{
			this.itemGroupBar = gbitem;
			this.ctrlGroupBar = gbitem.GroupBar;
			this.nIndex = this.ctrlGroupBar.activeItemsList.IndexOf( gbitem );
		}

		public int Index
		{
			get { return this.nIndex; }
		}

		public GroupBarControlAccessibleObject GroupBarAccessibilityObject
		{
			get { return this.ctrlGroupBar.AccessibilityObject as GroupBarControlAccessibleObject; }
		}

		public override AccessibleStates State
		{
			get
			{
				AccessibleStates accessiblestates = AccessibleStates.Selectable;
				if( this.ctrlGroupBar.SelectedItem == this.nIndex )
					accessiblestates |= AccessibleStates.Selected;
				return accessiblestates;
			}
		}

		public override AccessibleRole Role
		{
			get { return AccessibleRole.ListItem; }
		}

		public override AccessibleObject Parent
		{
			get { return this.ctrlGroupBar.AccessibilityObject; }
		}

		public override string Name
		{
			get { return (this.ctrlGroupBar.activeItemsList[this.nIndex] as GroupBarItem).Text; }
		}

		public override string DefaultAction
		{
			get { return "Select"; }
		}

		public override Rectangle Bounds
		{
			get { return this.ctrlGroupBar.RectangleToScreen( this.ctrlGroupBar.GetGroupBarItemBounds( this.nIndex ) ); }
		}

		public override string Description
		{
			get { return (this.ctrlGroupBar.activeItemsList[this.nIndex] as GroupBarItem).Text; }
		}

		public override void Select( AccessibleSelection flags )
		{
			if( (flags & AccessibleSelection.TakeFocus) != 0 )
			{
				if( !this.ctrlGroupBar.Focused )
					this.ctrlGroupBar.Focus();
			}
			if( (flags & AccessibleSelection.TakeSelection) != 0 )
				this.GroupBarAccessibilityObject.SelectChild( this, flags );
		}

		public override AccessibleObject Navigate( AccessibleNavigation navdir )
		{
			return this.GroupBarAccessibilityObject.NavigateFromChild( this, navdir );
		}

		public override void DoDefaultAction()
		{
			this.Select( AccessibleSelection.TakeSelection );
		}

		public override AccessibleObject GetFocused()
		{
			return this.GroupBarAccessibilityObject.GetFocused();
		}
	}

	public class GroupBarItemAccessibleObjectsIndexer
	{
		ArrayList data;
		GroupBar ctrlGroupBar;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:GroupBarItemAccessibleObjectsIndexer"/> class.
		/// </summary>
		public GroupBarItemAccessibleObjectsIndexer( GroupBar gbctrl )
		{
			this.ctrlGroupBar = gbctrl;
			this.data = new ArrayList();
		}

		public GroupBarItemAccessibleObject GetItem( int index )
		{
			// Returns NULL if GroupBarItem is not found.
			return index < data.Count ? data[index] as GroupBarItemAccessibleObject : null;
		}

		public void SetItem( int index, GroupBarItemAccessibleObject accobj )
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

		public GroupBarItemAccessibleObject this[int index]
		{
			get
			{
				GroupBarItemAccessibleObject accObj = GetItem( index );
				if( accObj == null )
				{
					accObj = this.ctrlGroupBar.CreateGroupBarItemAccessibilityInstance( index );
					// Save weak reference to object.
					SetItem( index, accObj );
				}
				return accObj;
			}
		}
	}

	public class DropDownButtonAccessibleItem: AccessibleObject
	{
		protected GroupBar groupBarControl;
		public DropDownButtonAccessibleItem( GroupBar grpbarctrl )
		{
			this.groupBarControl = grpbarctrl;
		}

		public override AccessibleStates State
		{
			get { return AccessibleStates.Default; }
		}

		public override AccessibleRole Role
		{
			get { return AccessibleRole.PushButton; }
		}

		public override AccessibleObject Parent
		{
			get { return this.groupBarControl.AccessibilityObject; }
		}

		public override string Name
		{
			get { return "Configure button"; }
		}

		public override string DefaultAction
		{
			get { return "Select"; }
		}

		public override Rectangle Bounds
		{
			get { return this.groupBarControl.RectangleToScreen( this.groupBarControl.GetDropDownButtonRectangle() ); }
		}

		public override string Description
		{
			get { return "Configure the Navigation Pane buttons and options."; }
		}

		public override AccessibleObject Navigate( AccessibleNavigation navdir )
		{
			if( navdir == AccessibleNavigation.Left )
			{
				if( this.groupBarControl.alNavPaneItems.Count > 0 )
				{
					GroupBarItem item = this.groupBarControl.alNavPaneItems[this.groupBarControl.alNavPaneItems.Count-1] as GroupBarItem;
					return this.groupBarControl.ItemAccessibleObjects[this.groupBarControl.GroupBarItems.IndexOf( item )];
				}
			}
			else if( navdir == AccessibleNavigation.Previous )
			{
				// Get the last visible item.
				for( int i=this.groupBarControl.GroupBarItems.Count-1; i>=0; i-- )
				{
					GroupBarItem item = this.groupBarControl.GroupBarItems[i];
					if( item.Visible == true )
						return this.groupBarControl.ItemAccessibleObjects[i];
				}
			}
			else if( navdir == AccessibleNavigation.Up )
			{
				// Run through the GroupBar's items collection in reverse order and return the first item that is not 
				// in the navigation pane.				
				for( int i=this.groupBarControl.GroupBarItems.Count-1; i>=0; i-- )
				{
					GroupBarItem item = this.groupBarControl.GroupBarItems[i];
					if( (item.Visible == true) && (item.InNavigationPane == false) )
						return this.groupBarControl.ItemAccessibleObjects[i];
				}
			}
			return null;
		}

		public override void DoDefaultAction()
		{
			this.groupBarControl.ShowDropDownMenu();
		}
	}
}