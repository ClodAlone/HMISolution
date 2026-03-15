#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region	file using directives

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections;
using System.Globalization;

#endregion

namespace Syncfusion.Windows.Forms.Tools
{
	/// <summary></summary>
	[
		ToolboxItem(false),
		ToolboxItemFilter( "System.Windows.Forms" ),
		DefaultEvent( "Click" ),
		DefaultProperty( "UserName" ),
		Designer( typeof( ControlDesigner ) )
	]
	public class XpUserEntry
		: ContainerControl
		, ISupportInitialize
	{
		#region	Class constants

		public const string ImageIndexEditor = "System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

		private const int DEF_CORNER_RADIUS = 3;
		private const int DEF_BUTTON_SPACE = 5;
		private const int DEF_MIN_PASSWORDBOX_WIDTH = 50;
		private const int DEF_RECT_INFLATE = 4;
 
		private const int INDENT_LEFT_FORM = 5;
		private const int INDENT_TOP_FORM = INDENT_LEFT_FORM;
		private const int INDENT_RIGHT_FORM = INDENT_LEFT_FORM;

		private const int INDENT_LEFT_PIC = 4;
		private const int INDENT_TOP_PIC = 4;
		private const int INDENT_BOTTOM_PIC = 3;
		private const int INDENT_RIGHT_PIC = 3;

		private const int INDENT_LEFT_RECT = 10;

		private const int INDENT_BOTTOM_USER_NAME = 5;
		private const int INDENT_LEFT_PASSWORDBOX = 5;

		private const int INDENT_LEFT_TEXT = INDENT_LEFT_RECT + INDENT_LEFT_FORM +
			INDENT_LEFT_PIC + INDENT_RIGHT_PIC;

		private static Color m_colorShadowDark = Color.FromArgb( 255, 0, 0, 0 );
		private static Color m_colorShadowLight = Color.FromArgb( 0, 0, 0, 0 );

		#endregion

		#region	Class members
		
		/// <summary>
		/// String format outputing text.
		/// </summary>
		protected StringFormat m_textStringFormat = null;
		
		///	<summary>
		///	This component style.
		///	</summary>
		private XpUserStyle m_style;
		
		///	<summary>
		///	Skip all evants if QuietMode runing.
		///	</summary>
		private bool m_bSkipEvents;
		
		///	<summary>
		///	User icon size thumbnail.
		///	</summary>
		private Size m_ThumbnailSize = new Size(48,48);
		
		///	<summary>
		///	User icon as default mode.
		///	</summary>
		private Image m_DefaultIcon;
		
		///	<summary>
		///	User icon as select mode.
		///	</summary>
		private Image m_SelectIcon;

		/// <summary>
		/// Order of the drawing button. 
		/// </summary>
		private int m_indexButtonDrawing = 0;
		
		///	<summary>
		///	User name.
		///	</summary>
		private string m_strUserName = string.Empty;
		
		///	<summary>
		///	User help.
		///	</summary>
		private string m_strHelpString = string.Empty;
		
		///	<summary>
		///	Active mode as mouse overhead component.
		///	</summary>
		private bool m_bActive = false;
		
		///	<summary>
		///	Select mode as user password enter.
		///	</summary>
		private bool m_bSelect = false;
		
		///	<summary>
		///	GraphicsPath for rectangle in user icon.
		///	</summary>
		private GraphicsPath m_rectIcon;

		///	<summary>
		///	GraphicsPath for rectangle in user TextBox.
		///	</summary>
		private GraphicsPath m_rectTextBox;

		///	<summary>
		///	Images List. image can be choosed from this	list and be	displayed as
		///	menu item icon.
		///	</summary>
		private ImageList m_imgList;
		
		///	<summary>
		///	Image index	from image list.
		///	</summary>
		private int m_iImageIndex = -1;
		
		///	<summary>
		///	Image index	from image list.
		///	</summary>
		private int m_iSelectedImageIndex = -1;
		
		/// <summary>
		/// Regions user icon.
		/// </summary>
		private Region m_rgIconRect;
		
		/// <summary>
		/// Neeeded recalculate layout graphics element.
		/// </summary>
		private bool m_bNeedRelayout = true;
		
		/// <summary>
		/// Regions for user name.
		/// </summary>
		private RectangleF m_rectUserName;
		
		/// <summary>
		/// Regions for user help.
		/// </summary>
		private RectangleF m_rectUserHelp;
		
		/// <summary>
		/// Collection of Buttons
		/// </summary>
		private XPUserEntryButtonsCollection m_buttons = null;

		/// <summary>
		/// Button to enter password.
		/// </summary>
		private XPUserEntryButton m_ButtonEnter;
		
		/// <summary>
		///	TextBox for password enter.
		/// </summary>
		protected TextBox m_textBox;
		
		/// <summary>
		/// TextBox layout.
		/// </summary>
		private Rectangle m_rectPasswordBox = new Rectangle();
		
		/// <summary>
		///	Icon button password enter.
		/// </summary>
		private Image m_ButtonEnterIcon;
		
		/// <summary>
		///	Text Rendering Hint.
		/// </summary>
		private TextRenderingHint m_textRenderingHint = TextRenderingHint.SystemDefault;

		/// <summary>
		///	Auto hide text box as control lost focus.
		/// </summary>
		private bool m_autoHideTextBox = true; 

		/// <summary>
		///	Auto reset password as text box show.
		/// </summary>
		private bool m_autoResetPassword = true;

		#endregion

		#region	Class properties

		///	<summary>
		///	Get or set component style.
		///	</summary>
		[ 
			DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
			Category( "Appearance" ),
			Description( "Get or set component style." )
		]
		public XpUserStyle Style
		{
			get
			{
				return m_style;
			}
			set
			{
				if( value != m_style )
				{
					// detach event	first
					if( m_style != null )
					{
						m_style.OnChanged -= new EventHandler( style_OnChanged );
					}
					m_style = ( value == null ) ? new XpUserStyle() : value;

					m_style.OnChanged += new EventHandler( style_OnChanged );
				}
			}
		}

		///	<summary>
		///	Get or set user icon size.
		///	</summary>
		[ 
			Category( "Layout" ),
			Description( "Get or set user icon size." )
		]	
		public Size ThumbnailSize
		{
			get
			{
				return m_ThumbnailSize;
			}
			set
			{
				if( value != m_ThumbnailSize )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_ThumbnailSize, value );
					m_ThumbnailSize = value;
					OnThumbnailSizeChanged( args );
				}
			}
		}


		///	<summary>
		///	Get or set image list user icons.
		///	</summary>
		[ DefaultValue( null )
			, Category( "Appearance" )
			,Description(" Get or set image list user icons." )
		]
		public ImageList ImageList
		{
			get
			{
				return m_imgList;
			}
			set
			{
				if( value != m_imgList )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_imgList, value );
					m_imgList = value;
					OnImageListChanged( args );
				}
			}
		}

		
		///	<summary>
		/// Get or set user icon index from image list.
		///	</summary>
		[ DefaultValue( -1 )
			, Category( "Appearance" )
			, Editor( ImageIndexEditor, typeof( UITypeEditor ) )
			, TypeConverter( "System.Windows.Forms.ImageIndexConverter" ) 
			, Description( "Get or set user icon index from image list." )
		]
		public int DefaultImageIndex
		{
			get
			{
				return m_iImageIndex;
			}
			set
			{
				ValueChangedEventArgs args = new ValueChangedEventArgs( m_iImageIndex, value );
				m_iImageIndex = value;
				OnDefaultImageIndexChanged( args );
			}
		}


		///	<summary>
		///	Get or set user icon index from image list.
		///	</summary>
		[ DefaultValue( -1 )
			, Category( "Appearance" )
			, Editor( ImageIndexEditor, typeof( UITypeEditor ) )
			, TypeConverter( "System.Windows.Forms.ImageIndexConverter" ) 
			, Description( "Get or set user icon index from image list." )
		]
		public int SelectedImageIndex
		{
			get
			{
				return m_iSelectedImageIndex;
			}
			set
			{
				m_iSelectedImageIndex = value;
			}
		}

		
		///	<summary>
		///	Get or set default user icon.
		///	</summary>
		[ 
			DefaultValue( null ),
			Category( "Appearance" )
			, Description( "Get or set default user icon." )
		]
		public Image DefaultIcon
		{
			get
			{
				return m_DefaultIcon;
			}
			set
			{
				if( value != m_DefaultIcon )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_DefaultIcon, value );
					m_bNeedRelayout = true;
					m_DefaultIcon = value;
					OnDefaultIconChanged( args );
				}
			}
		}


		/// <summary>
		/// Get or set select user icon.
		/// </summary>
		[ 
			DefaultValue( null ),
			Category( "Appearance" )
			, Description( "Get or set select user icon." )
		]
		public Image SelectIcon
		{
			get
			{
				return m_SelectIcon;
			}
			set
			{
				if( value != m_SelectIcon )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_SelectIcon, value );
					m_SelectIcon = value;
					OnSelectIconChanged( args );
				}
			}
		}

		/// <summary>
		/// Get or set user icon as select mode.
		/// </summary>
		[ 
			DefaultValue( null ),
			Category( "Appearance" )
			, Description( "Get or set user icon as select mode." )
		]
		public Image ButtonEnterIcon
		{
			get
			{
				return m_ButtonEnterIcon;
			}
			set
			{
				if( value != m_ButtonEnterIcon )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_ButtonEnterIcon, value );
					m_ButtonEnterIcon = value;
					OnButtonEnterIconChanged( args );
				}
			}
		}


		///	<summary>
		///	Get or set user name.
		///	</summary>
		[ 
			DefaultValue( "" ),
			Category( "Appearance" )
			, Description( "Get or set user name." )
		]
		public string UserName
		{
			get
			{
				return m_strUserName;
			}
			set
			{
				if( value != m_strUserName )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_strUserName, value );
					m_strUserName = value;
					OnUserNameChanged( args );
				}
			}
		}


		///	<summary>
		///	Get or set user help.
		///	</summary>
		[ 
			DefaultValue( "" ),
			Category( "Appearance" )
			, Description( "Get or set user help." )
		]
		public string HelpString
		{
			get
			{
				return m_strHelpString;
			}
			set
			{
				if( value != m_strHelpString )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_strHelpString, value );
					m_strHelpString = value;
					OnHelpStringChanged( args );
				}
			}
		}


		///	<summary>
		///	True - do not raise	any	events,	otherwise False.
		///	</summary>
		[ Description( "True - do not raise	any	events,	otherwise False." ) ]
		protected internal bool QuietMode
		{
			get { return m_bSkipEvents; }
			set
			{
				if( value != m_bSkipEvents )
				{
					m_bSkipEvents = value;
					OnQuietModeChanged();
				}
			}
		}


		///	<summary>
		/// Get active mode as mouse overhead component.
		///	</summary>
		[ Description( "Get active mode as mouse overhead component." ) ]
		protected bool IsActive
		{
			get { return m_bActive; }
		}


		///	<summary>
		/// Get select mode.
		///	</summary>
		[ Description( "Get select mode." ) ]
		protected bool IsSelected
		{
			get { return m_bSelect; }
		}


		/// <summary>
		/// Get password box.
		/// </summary>
		[ 
			Browsable( false ) 
			, Description( "Get password box." )
		]
		public TextBox TextBox
		{
			get
			{
				if( null == m_textBox )
				{
					ConstructTextBox();
				}

				return m_textBox;
			}
		}


		/// <summary>
		///	Get or set Text Rendering Hint.
		/// </summary>
		[
			Browsable(true),
			DefaultValue(TextRenderingHint.SystemDefault),
			Category( "Appearance" )
			, Description( "Get or set Text Rendering Hint." )
		]
		public TextRenderingHint TextRenderingHint
		{
			get
			{
				return m_textRenderingHint;
			}
			set
			{
				if( value != m_textRenderingHint )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_textRenderingHint, value );
					m_textRenderingHint = value;
					OnTextRenderingHintChanged( args );
				}
			}
		}


		/// <summary>
		///	Get or set auto hide text box as control lost focus.
		/// </summary>
		[ 
			Browsable( true ),
			DefaultValue( true ),
			Category( "Appearance" )
			, Description( "Get or set auto hide text box as control lost focus." )
		]
		public bool AutoHideTextBox
		{
			get
			{
				return m_autoHideTextBox;
			}
			set
			{
				if( value != m_autoHideTextBox )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_autoHideTextBox, value );
					m_autoHideTextBox = value;
					OnAutoHideTextBoxChanged( args );
				}
			}
		}


		/// <summary>
		///	Get or set auto reset password as text box show.
		/// </summary>
		[ 
			Browsable( true ),
			DefaultValue( true ),
			Category( "Appearance" )
			, Description( "Get or set auto reset password as text box show." )
		]
		public bool AutoResetPassword
		{
			get
			{
				return m_autoResetPassword;
			}
			set
			{
				if( value != m_autoResetPassword )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_autoResetPassword, value );
					m_autoResetPassword = value;
					OnAutoResetPasswordChanged( args );
				}
			}
		}

		/// <summary>
		/// Collection of buttons.
		/// </summary>
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Description("The XPUserEntryButtons present in the control."),
		Category("XPUserEntryButtons")
		]
		public XPUserEntryButtonsCollection Buttons
		{
			get { return m_buttons; }
			set
			{ 
				if( m_buttons != value )
				{
					m_buttons = value;
				}
			 }
		}
		
		#endregion

		#region	Class events

		///	<summary>
		///	Occurs when quiet mode changed.
		///	</summary>
		public event EventHandler QuietModeChanged;

		///	<summary>
		///	Occurs when thumbnail size changed.
		///	</summary>
		[ Category( "Property Changed" ) ]
		public event ValueChangedEventHandler ThumbnailSizeChanged;

		///	<summary>
		///	Occurs when default icon changed.
		///	</summary>
		[ Category( "Property Changed" ) ]
		public event ValueChangedEventHandler DefaultIconChanged;

		///	<summary>
		///	Occurs when select icon changed.
		///	</summary>
		[ Category( "Property Changed" ) ]
		public event ValueChangedEventHandler SelectIconChanged;

		///	<summary>
		///	Occurs when user name changed.
		///	</summary>
		[ Category( "Property Changed" ) ]
		public event ValueChangedEventHandler UserNameChanged;

		///	<summary>
		///	Occurs when help string changed.
		///	</summary>
		[ Category( "Property Changed" ) ]
		public event ValueChangedEventHandler HelpStringChanged;

		///	<summary>
		///	Occurs when image list changed.
		///	</summary>
		[ Category( "Property Changed" ) ]
		public event ValueChangedEventHandler ImageListChanged;

		/// <summary>
		/// Occurs when default image index changed.
		/// </summary>
		[ Category( "Property Changed" ) ]
		public event ValueChangedEventHandler DefaultImageIndexChanged;

		/// <summary>
		///	Occurs when password box changed.
		/// </summary>
		[ Category( "Property Changed" ) ]
		public event ValueChangedEventHandler PasswordBoxChanged;

		/// <summary>
		///	Occurs when button enter icon changed.
		/// </summary>
		[ Category( "Property Changed" ) ]
		public event ValueChangedEventHandler ButtonEnterIconChanged;

		/// <summary>
		/// Occurs when when user enter password.
		/// </summary>
		public event PasswordEnterEventHandler PasswordEnter;

		/// <summary>
		/// Occurs when text rendering hint changed.
		/// </summary>
		[ Category( "Property Changed" ) ]
		public event ValueChangedEventHandler TextRenderingHintChanged;

		/// <summary>
		///	Occurs when auto hide text box changed.
		/// </summary>
		[ Category( "Property Changed" ) ]
		public event ValueChangedEventHandler AutoHideTextBoxChanged;

		/// <summary>
		///	Occurs when auto reset password changed.
		/// </summary>
		[ Category( "Property Changed" ) ]
		public event ValueChangedEventHandler AutoResetPasswordChanged;
		#endregion

		#region	Class initialize/finalize methods
		///	<summary>
		///	Constructor this component.
		///	</summary>
		public XpUserEntry()
		{
			ControlStyles styleTrue = ControlStyles.AllPaintingInWmPaint |
				ControlStyles.ResizeRedraw |
				ControlStyles.UserPaint |
				ControlStyles.UserMouse |
				ControlStyles.Selectable |
				ControlStyles.DoubleBuffer |
				ControlStyles.SupportsTransparentBackColor;

			SetStyle( styleTrue, true );

			m_textStringFormat = new StringFormat( StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap );
			m_textStringFormat.Trimming = StringTrimming.EllipsisCharacter;

			m_buttons = new XPUserEntryButtonsCollection(this);

			// TODO: introduce virtual method for creating TextBox
			// PaswordBox

			// XPStyle
			m_style = new XpUserStyle();
			m_style.OnChanged += new EventHandler( style_OnChanged );

			base.Size = this.Size;
		}

		///	<summary>
		///
		///	</summary>
		~XpUserEntry()
		{
			this.Dispose( !this.IsDisposed );
		}

		///	<summary>
		/// Begin initialize.
		///	</summary>
		public virtual void BeginInit()
		{
			this.QuietMode = true;
		}

		///	<summary>
		///	Begin initialize.
		///	</summary>
		public virtual void EndInit()
		{
			this.QuietMode = false;

			if( ThumbnailSize == Size.Empty )
			{
				OnThumbnailSizeChanged( null );
			}
		}

		///	<override/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (this.TextBox != null)
				{
					m_style.OnChanged -= new EventHandler( style_OnChanged );
					this.m_textBox.KeyPress -= new KeyPressEventHandler( this.OnPasswordKeyPress );	
					this.m_textBox.Leave -= new EventHandler( this.OnPaswordBoxLeave );
				}
			}

			base.Dispose( disposing );
		}

		#endregion

		#region	Class event	raisers

		protected void RaiseQuietModeChangedEvent()
		{
			if( QuietModeChanged != null )
			{
				QuietModeChanged( this, EventArgs.Empty );
			}
		}

		protected void RaiseThumbnailSizeChanged( ValueChangedEventArgs args )
		{
			if( ThumbnailSizeChanged != null && !this.QuietMode )
			{
				ThumbnailSizeChanged( this, args );
			}
		}

		protected void RaiseDefaultIconChanged( ValueChangedEventArgs args )
		{
			if( DefaultIconChanged != null && !this.QuietMode )
			{
				DefaultIconChanged( this, args );
			}
		}

		protected void RaiseSelectIconChanged( ValueChangedEventArgs args )
		{
			if( SelectIconChanged != null && !this.QuietMode )
			{
				SelectIconChanged( this, args );
			}
		}

		protected void RaiseUserNameChanged( ValueChangedEventArgs args )
		{
			if( UserNameChanged != null && !this.QuietMode )
			{
				UserNameChanged( this, args );
			}
		}

		protected void RaiseHelpStringChanged( ValueChangedEventArgs args )
		{
			if( HelpStringChanged != null && !this.QuietMode )
			{
				HelpStringChanged( this, args );
			}
		}

		protected void RaiseImageListChanged( ValueChangedEventArgs args )
		{
			if( ImageListChanged != null && !this.QuietMode )
			{
				ImageListChanged( this, args );
			}
		}

		protected void RaiseDefaultImageIndex( ValueChangedEventArgs args )
		{
			if( DefaultImageIndexChanged != null && !this.QuietMode )
			{
				DefaultImageIndexChanged( this, args );
			}
		}

		protected void RaisePasswordBoxChanged( ValueChangedEventArgs args )
		{
			if( PasswordBoxChanged != null && !this.QuietMode )
			{
				PasswordBoxChanged( this, args );
			}
		}

		protected void RaiseButtonEnterIconChanged( ValueChangedEventArgs args )
		{
			if( ButtonEnterIconChanged != null && !this.QuietMode )
			{
				ButtonEnterIconChanged( this, args );
			}
		}

		protected void RaiseTextRenderingHintChanged( ValueChangedEventArgs args )
		{
			if( TextRenderingHintChanged != null && !this.QuietMode )
			{
				TextRenderingHintChanged( this, args );
			}
		}		

		protected void RaiseAutoHideTextBoxChanged( ValueChangedEventArgs args )
		{
			if( AutoHideTextBoxChanged != null && !this.QuietMode )
			{
				AutoHideTextBoxChanged( this, args );
			}
		}	
		
		protected void RaiseAutoResetPasswordChanged( ValueChangedEventArgs args )
		{
			if( AutoResetPasswordChanged != null && !this.QuietMode )
			{
				AutoResetPasswordChanged( this, args );
			}
		}			
		
		#endregion

		#region	Serialization helper methods
		protected bool ShouldSerializeThumbnailSize()
		{
			return ( this.ThumbnailSize != new  Size(48,48) );
		}

		#endregion

		#region	Class overrides
		
		///	<override/>
		protected override void OnResize( EventArgs e )
		{
			m_bNeedRelayout = true;
			base.OnResize( e );
		}

		///	<override/>
		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );
			
			Graphics g = e.Graphics;
			
			//Set Graphics Mode
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.TextRenderingHint = this.TextRenderingHint;

			// Recalcilate Layout
			if( m_bNeedRelayout )
			{
				CalculateLayout( g );
				m_bNeedRelayout = false;
			}

			// draw	background
			this.Style.DrawControlBackground( g, this.ClientRectangle );

			// draw shadow
			if( this.Style.DrawShadow && this.Style.ShadowSize != 0 )
			{
				DrawShadow( g );
			}

			// Draw	Rect
			this.Style.DrawImageRect( g, m_rectIcon, this.IsActive || this.IsSelected );

			// Draw	Icon
			if( this.IconForDrawing != null )
			{
				Region rgOld = g.Clip;
				g.Clip = m_rgIconRect;
				g.DrawImage( IconForDrawing, m_rgIconRect.GetBounds( g ) );
				g.Clip = rgOld;
			}
			
			Color curColor = this.IsActive ? this.Style.UserNameColorActive : this.Style.UserNameColorDefault;

			using( SolidBrush brush = new SolidBrush( curColor ) )
			{
				// Draw	UserName
				g.DrawString( this.UserName, 
					( this.IsActive ) ? this.Style.ActiveFont : this.Style.Font, 
					brush, m_rectUserName, m_textStringFormat );

				// Draw	HelpString
				brush.Color = ( this.IsActive ) ? 
					this.Style.UserHelpColorActive : this.Style.UserHelpColorDefault;
				
				if( !this.IsSelected )
				{
					g.DrawString( this.HelpString,
						( this.IsActive ) ? this.Style.ActiveUserHelpFont : this.Style.DefaultUserHelpFont,
						brush, m_rectUserHelp, m_textStringFormat );
				}
				
				// Set PasswordBox Layout
				if( this.IsSelected )
				{
					Rectangle rect2 = m_rectPasswordBox;
					rect2.Inflate( DEF_RECT_INFLATE, DEF_RECT_INFLATE );
			
					CalculateButtonsLayout( this.ThumbnailSize.Width, rect2 );

					m_ButtonEnter.Draw( g );
					
					m_rectTextBox = GetRoundRect( rect2.X, rect2.Y, rect2.Width, rect2.Height, DEF_CORNER_RADIUS );					
					g.FillPath( new SolidBrush( TextBox.BackColor ), m_rectTextBox );	
																			
					this.TextBox.Bounds = m_rectPasswordBox;  

					if( m_buttons != null && m_buttons.Count > 0 )
					{
						for( int i = 0, len = m_buttons.Count; i < len; i++ )
						{
							if( this.RightToLeft == RightToLeft.Yes )
							{
								m_buttons[ i ].Bounds = MirrorRectangle( m_buttons[ i ].Bounds );
							}
							
							if( m_buttons[ i ].Visible )
							{
								m_buttons[ i ].Draw(g);
							}
						}
					}
				}
			}
		}
		
		///	<override/>
		protected override void OnMouseLeave( EventArgs e )
		{
			m_bActive = false;
			m_bNeedRelayout = true;
			this.Invalidate();

			base.OnMouseLeave( e );
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if( this.IsSelected )
			{
				if( HitTest( e.X, e.Y ) != HitAreas.ButtonEnter )
				{
					if( m_ButtonEnter != null &&
						( m_ButtonEnter.State == XPUserEntryButtonState.Highlighted ||
					    m_ButtonEnter.State == XPUserEntryButtonState.Pushed ) )
					{
						m_ButtonEnter.State = XPUserEntryButtonState.Normal;
						Invalidate( m_ButtonEnter.Bounds );
					}

					for( int i = 0, len = m_buttons.Count; i < len; i++ )
						if( m_buttons[ i ].State == XPUserEntryButtonState.Highlighted || 
							m_buttons[ i ].State == XPUserEntryButtonState.Pushed )
						{
							m_buttons[ i ].State = XPUserEntryButtonState.Normal;
							Invalidate( m_buttons[ i ].Bounds );
						}
				}
				else
				{
					if( m_ButtonEnter.Bounds.Contains( e.X, e.Y ) &&
						m_ButtonEnter.State != XPUserEntryButtonState.Pushed )
					{
						m_ButtonEnter.State = XPUserEntryButtonState.Highlighted;
						Invalidate( m_ButtonEnter.Bounds );
					}

					for( int i = 0, len = m_buttons.Count; i < len; i++ )
					{
						if( m_buttons[ i ].Enable &&
							m_buttons[ i ].Bounds.Contains( e.X, e.Y ) && 
							m_buttons[ i ].State != XPUserEntryButtonState.Pushed )
						{
							m_buttons[ i ].State = XPUserEntryButtonState.Highlighted;
							Invalidate(m_buttons[ i ].Bounds);
						}
					}
				}
			}

			base.OnMouseMove (e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if( m_ButtonEnter != null && m_ButtonEnter.Bounds.Contains( e.X, e.Y ) )
			{
				m_ButtonEnter.State = XPUserEntryButtonState.Pushed;

				Invalidate( m_ButtonEnter.Bounds );
			}
			else if( HitAddedButtons( e.X, e.Y ) )
			{
				for( int i = 0, len = m_buttons.Count; i < len; i++ )
					if( m_buttons[ i ].Enable &&
						m_buttons[ i ].Bounds.Contains( e.X, e.Y ) )
					{
						m_buttons[ i ].State = XPUserEntryButtonState.Pushed;

						Invalidate( m_buttons[ i ].Bounds );
					}
			}

			base.OnMouseDown (e);
		}
 
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if( m_ButtonEnter != null && m_ButtonEnter.Bounds.Contains( e.X, e.Y ) 
				&& m_ButtonEnter.State == XPUserEntryButtonState.Pushed )
			{
				m_ButtonEnter.State = XPUserEntryButtonState.Normal;

				Invalidate( m_ButtonEnter.Bounds );
			}
			else if( HitAddedButtons( e.X, e.Y ) )
			{
				for( int i = 0, len = m_buttons.Count; i < len; i++ )
					if( m_buttons[ i ].Bounds.Contains( e.X, e.Y ) &&
						m_buttons[ i ].State == XPUserEntryButtonState.Pushed )
					{
						m_buttons[ i ].State = XPUserEntryButtonState.Normal;

						Invalidate( m_buttons[ i ].Bounds );
					}
			}

			base.OnMouseUp (e);
		}

		///	<override/>
		protected override void OnMouseEnter( EventArgs e )
		{
			m_bActive = true;
			m_bNeedRelayout = true;
			this.Invalidate();

			base.OnMouseEnter( e );
		}

		
		///	<override/>
		protected override void OnGotFocus( EventArgs e )
		{
			TextBox textBox = this.TextBox;	// Ensure that TextBox is created

			if( !textBox.Visible )
			{
				if( AutoResetPassword )
				{
					textBox.Text = "";
				}
				textBox.Show();
			}

			textBox.Focus();

			m_bSelect = true;
			m_bNeedRelayout = true;

			Invalidate();

			base.OnGotFocus( e );
		}

		
		///	<override/>
		protected override void OnLostFocus( EventArgs e )
		{
			m_bNeedRelayout = true;
			Invalidate();

			base.OnLostFocus( e );
		}
		
    
		///	<override/>
		protected override void OnRightToLeftChanged(EventArgs e)
		{
			m_bNeedRelayout = true;

			if( RightToLeft.Yes == this.RightToLeft )
			{
				m_textStringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
			}
			else
			{
				m_textStringFormat.FormatFlags &= ~StringFormatFlags.DirectionRightToLeft;
			}

			base.OnRightToLeftChanged(e);
		}

		
		protected void OnPaswordBoxLeave( object sender, EventArgs e )
		{
			if( this.ActiveControl != this && AutoHideTextBox )
			{
				this.TextBox.Hide();
				m_bSelect = false;
			}
			m_bNeedRelayout = true;
			Invalidate();
		}

	
		
		///	<summary>
		///	Occurs when quiet mode changed.
		///	</summary>
		protected virtual void OnQuietModeChanged()
		{
			RaiseQuietModeChangedEvent();
		}

		
		/// <summary> 
		/// Occurs when thumbnail size changed.
		/// </summary>
		/// <param name="args"></param>
		protected virtual void OnThumbnailSizeChanged( ValueChangedEventArgs args )
		{
			RaiseThumbnailSizeChanged( args );

			m_bNeedRelayout = true;
			Invalidate();
		}

		
		///	<summary>
		///	Occurs when default icon changed.
		///	</summary>
		///	<param name="args"/>
		protected virtual void OnDefaultIconChanged( ValueChangedEventArgs args )
		{
			RaiseDefaultIconChanged( args );
			m_bNeedRelayout = true;
			Invalidate();
		}

		
		///	<summary>
		///	Occurs when select icon changed.
		///	</summary>
		///	<param name="args"/>
		protected virtual void OnSelectIconChanged( ValueChangedEventArgs args )
		{
			RaiseSelectIconChanged( args );
			m_bNeedRelayout = true;
			Invalidate();
		}

		
		///	<summary>
		///	Occurs when user name changed.
		///	</summary>
		///	<param name="args"/>
		protected virtual void OnUserNameChanged( ValueChangedEventArgs args )
		{
			RaiseUserNameChanged( args );
			m_bNeedRelayout = true;
			Invalidate();
		}

		
		///	<summary>
		///	Occurs when help string changed.
		///	</summary>
		///	<param name="args"/>
		protected virtual void OnHelpStringChanged( ValueChangedEventArgs args )
		{
			RaiseHelpStringChanged( args );
			m_bNeedRelayout = true;
			Invalidate();
		}
		
	
		///	<summary>
		///	Occurs when image list changed.
		///	</summary>
		///	<param name="args"></param>
		protected virtual void OnImageListChanged( ValueChangedEventArgs args )
		{
			RaiseImageListChanged( args );
			m_bNeedRelayout = true;
			Invalidate();
		}

		
		///	<summary>
		///	Occurs when default image index changed.
		///	</summary>
		///	<param name="args"></param>
		protected virtual void OnDefaultImageIndexChanged( ValueChangedEventArgs args )
		{
			RaiseDefaultImageIndex( args );
			m_bNeedRelayout = true;
			Invalidate();
		}

		
		/// <summary>
		///	Occurs when password box changed.
		/// </summary>
		protected virtual void OnPasswordBoxChanged( ValueChangedEventArgs args )
		{
			RaisePasswordBoxChanged( args );
		}

		
		/// <summary>
		///	Occurs when button enter icon changed.
		/// </summary>
		protected virtual void OnButtonEnterIconChanged( ValueChangedEventArgs args )
		{
			RaiseButtonEnterIconChanged( args );
			m_bNeedRelayout = true;
			Invalidate();
		}

		
		///	<override/>
		protected override void OnClick( EventArgs e )
		{
			Point mp = this.PointToClient( Control.MousePosition );

			if( m_ButtonEnter != null && m_ButtonEnter.Bounds.Contains( mp ) && this.IsSelected )
			{
				PasswordEnterEventArgs pwdEnterArgs = new PasswordEnterEventArgs( this.UserName, this.m_textBox.Text );

				OnPasswordEnter( pwdEnterArgs );
			}

			base.OnClick( e );
		}

		
		protected void OnPasswordKeyPress( object sender, KeyPressEventArgs e )
		{
			Keys key = (Keys)Convert.ToByte(e.KeyChar);

			if( Keys.Enter == key )
			{	
				OnPasswordEnter( new PasswordEnterEventArgs( this.UserName, this.m_textBox.Text ) );
			}
			else if( Keys.Escape == key )
			{
				this.CloseInputArea();
			}
		}

		
		/// <summary>
		/// Occurs when user enter password.
		/// </summary>
		protected virtual void OnPasswordEnter( PasswordEnterEventArgs e )
		{
			if( PasswordEnter != null )
			{
				PasswordEnter( this, e );
				if( !e.Cancel )
				{
					this.CloseInputArea();
				}
			}
		}

		public void CloseInputArea()
		{
			this.TextBox.Hide();
			this.m_bSelect = false;
			m_bNeedRelayout = true;

			Invalidate();
		}
		
		/// <summary>
		/// Occurs when component style changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void style_OnChanged( object sender, EventArgs e )
		{
			m_bNeedRelayout = true;
			Invalidate();
		}

		
		/// <summary>
		///	Occurs when text rendering hint changed.
		/// </summary>
		protected virtual void OnTextRenderingHintChanged( ValueChangedEventArgs args )
		{
			m_bNeedRelayout = true;
			this.Invalidate();

			RaiseTextRenderingHintChanged( args );
		}
		

		/// <summary>
		/// Occurs when text box create.
		/// </summary>
		protected virtual void OnTextBoxCreate()
		{
			this.m_textBox = new TextBox();
			this.m_textBox.Name = "PasswordBox";
			this.m_textBox.PasswordChar = '\u2022';
			this.m_textBox.BorderStyle = BorderStyle.None;
		}


		/// <summary>
		/// Occurs when	text box initialize.
		/// </summary>
		protected virtual void OnTextBoxInitialize()
		{
			this.m_textBox.Leave += new EventHandler( this.OnPaswordBoxLeave );
			this.m_textBox.KeyPress += new KeyPressEventHandler( this.OnPasswordKeyPress );
		}


		/// <summary>
		/// Text box constructor.
		/// </summary>
		private void ConstructTextBox()
		{
			OnTextBoxCreate();

			this.Controls.Add( this.TextBox );

			OnTextBoxInitialize();
		}


		/// <summary>
		///	Occurs when auto hide text box changed.
		/// </summary>
		protected virtual void OnAutoHideTextBoxChanged( ValueChangedEventArgs args )
		{
			RaiseAutoHideTextBoxChanged( args );
			m_bNeedRelayout = true;
			Invalidate();
		}


		/// <summary>
		///	Occurs when auto reset password changed.
		/// </summary>
		protected virtual void OnAutoResetPasswordChanged( ValueChangedEventArgs args )
		{
			RaiseAutoResetPasswordChanged( args );
		}


		[ Browsable( false ) ]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}

		#endregion

		#region	Class utility methods

		///	<summary>
		///	Draw rounded rectangle.
		///	</summary>
		///	<param name="x">X - coordinate of rectangle.</param>
		///	<param name="y">Y - coordinate of rectangle.</param>
		///	<param name="w">Width rectangle.</param>
		///	<param name="h">Heigth rectangle.</param>
		///	<param name="r">Radius evening-out.</param>
		///	<returns>GraphicsPath rounded rectangle.</returns>
		private GraphicsPath GetRoundRect( int x, int y, int w, int h, int r )
		{
			GraphicsPath gp = new GraphicsPath();
			
			if( r > 0 )
			{
				gp.AddArc( x, y, r * 2, r * 2, 180, 90 );
				gp.AddLine( x + r, y, x + w - r, y );
				gp.AddArc( x + w - r * 2, y, r * 2, r * 2, 270, 90 );
				gp.AddLine( x + w, y + r, x + w, y + h - r );
				gp.AddArc( x + w - r * 2, y + h - r * 2, r * 2, r * 2, 0, 90 );
				gp.AddLine( x + w - r, y + h, x + r, y + h );
				gp.AddArc( x, y + h - r * 2, r * 2, r * 2, 90, 90 );
				gp.AddLine( x, y + h - r, x, y + r );
				gp.CloseFigure();
			}
			else
			{
				gp.AddRectangle( new Rectangle( x, y, w, h ) );
			}

			return gp;
		}


		///	<summary>
		///	Calculate layout graphics element.
		///	</summary>
		private void CalculateLayout( Graphics gph )
		{
			int thumbWidth = this.ThumbnailSize.Width;
			int thumbHeight = this.ThumbnailSize.Height;

			// Calculate rect size and draw GraphicsPath
			m_rectIcon = GetRoundRect( INDENT_LEFT_FORM,
			                       INDENT_TOP_FORM,
			                       thumbWidth + INDENT_LEFT_PIC + INDENT_RIGHT_PIC,
			                       thumbHeight + INDENT_TOP_PIC + INDENT_BOTTOM_PIC,
			                       Style.RectRadius );
			
			// Calculate Icon rect and draw GraphicsPath
			GraphicsPath grIconRect = GetRoundRect(
				INDENT_LEFT_FORM + INDENT_LEFT_PIC,
				INDENT_TOP_FORM + INDENT_TOP_PIC,
				thumbWidth, thumbHeight, Style.IconRadius );

			// clean resources
			if( m_rgIconRect != null )
			{
				m_rgIconRect.Dispose();
			}

			m_rgIconRect = new Region( grIconRect );

			grIconRect.Dispose();

			int textAreaWidth = this.Width - ( INDENT_LEFT_FORM + thumbWidth + INDENT_RIGHT_FORM + INDENT_RIGHT_PIC );

			SizeF m_sizeUserName = CalculateUserNameLayout( gph, textAreaWidth, thumbWidth );

			CalculateTextBoxLayout( thumbWidth, m_sizeUserName, gph );

			CalculateUserHelpLayout( gph, textAreaWidth, thumbWidth, m_sizeUserName );

			if( this.RightToLeft == RightToLeft.Yes )
			{
				MirrorAll( gph );
			}
		}


		/// <summary>
		/// Calculate user name layout.
		/// </summary>
		/// <param name="gph"></param>
		/// <param name="textAreaWidth"></param>
		/// <param name="thumbWidth"></param>
		/// <returns></returns>
		private SizeF CalculateUserNameLayout( Graphics gph, int textAreaWidth, int thumbWidth )
		{
			// Calculate UserName Size text
			SizeF m_sizeUserName = gph.MeasureString( this.UserName,
				( this.IsActive ) ? this.Style.ActiveFont : this.Style.Font, textAreaWidth, m_textStringFormat );
	
			// Calculate UserName Layout
			m_rectUserName = new RectangleF(
				thumbWidth + INDENT_LEFT_TEXT, INDENT_TOP_FORM,
				Math.Min(m_sizeUserName.Width, this.Width - ( thumbWidth + INDENT_LEFT_TEXT + INDENT_RIGHT_FORM ) ),
				m_sizeUserName.Height );
			return m_sizeUserName;
		}
		

		/// <summary>
		/// Calculate user help layout.
		/// </summary>
		/// <param name="gph"></param>
		/// <param name="textAreaWidth"></param>
		/// <param name="thumbWidth"></param>
		/// <param name="m_sizeUserName"></param>
		private void CalculateUserHelpLayout( Graphics gph, int textAreaWidth, int thumbWidth, SizeF m_sizeUserName )
		{
			// Calculate UserHelp Size text
			SizeF sizeHelp = gph.MeasureString( this.HelpString, 
				( this.IsActive ) ? this.Style.ActiveUserHelpFont : this.Style.DefaultUserHelpFont,
				textAreaWidth, m_textStringFormat );
	
			// Calculate UserHelp Layout
			m_rectUserHelp = new RectangleF(
				thumbWidth + INDENT_LEFT_TEXT,
				m_sizeUserName.Height + INDENT_BOTTOM_USER_NAME,
				Math.Min(sizeHelp.Width, this.Width - ( thumbWidth + INDENT_LEFT_TEXT + INDENT_RIGHT_FORM ) ),
				sizeHelp.Height );
		}


		/// <summary>
		/// Calculate text box layout.
		/// </summary>
		/// <param name="thumbWidth"></param>
		/// <param name="m_sizeUserName"></param>
		private void CalculateTextBoxLayout( int thumbWidth, SizeF m_sizeUserName, Graphics g )
		{
			if( this.IsSelected )
			{
				m_rectPasswordBox.Location = new Point( thumbWidth + INDENT_LEFT_TEXT,
					( int ) m_sizeUserName.Height + INDENT_BOTTOM_USER_NAME + INDENT_BOTTOM_USER_NAME / 2 );
			
				int buttonEnterWidth = this.TextBox.Height;

				m_rectPasswordBox.Height = buttonEnterWidth;

				if( m_ButtonEnter == null )
				{
					m_ButtonEnter = new XPUserEntryButton( "ButtonEnter", ButtonEnterIcon, XPUserEntryButtonType.PasswordEnter, m_buttons.Count, true, true );
					
					m_ButtonEnter.Bounds = new Rectangle( Point.Empty, new Size( buttonEnterWidth + DEF_RECT_INFLATE * 2, buttonEnterWidth + DEF_RECT_INFLATE * 2) );
				}

				if( m_buttons.Count == 0 )
				{
					m_rectPasswordBox.Width = this.Width - ( thumbWidth + INDENT_LEFT_TEXT + INDENT_RIGHT_FORM +
						INDENT_LEFT_PASSWORDBOX + m_ButtonEnter.Bounds.Width + DEF_RECT_INFLATE );
				}
				else
				{
					m_rectPasswordBox.Width = this.Width - ( thumbWidth + INDENT_LEFT_TEXT + INDENT_RIGHT_FORM +
						INDENT_LEFT_PASSWORDBOX + DEF_RECT_INFLATE + ( m_buttons.Count + 1 ) * ( buttonEnterWidth + DEF_RECT_INFLATE * 2 ) + 
						m_buttons.Count * DEF_BUTTON_SPACE );
				}

				if( m_rectPasswordBox.Width < DEF_MIN_PASSWORDBOX_WIDTH )
					m_rectPasswordBox.Width = DEF_MIN_PASSWORDBOX_WIDTH;
		
				CalculateButtonsLayout( thumbWidth, m_rectPasswordBox );
			}
		}

		/// <summary>
		/// Calculate buttons layout.
		/// </summary>
		/// <param name="thumbWidth"></param>
		/// <param name="rectPasswordBox"></param>
		private void CalculateButtonsLayout( int thumbWidth, Rectangle rectPasswordBox )
		{
			if( m_ButtonEnter == null )
			{
				m_ButtonEnter = new XPUserEntryButton( "EnterButton", ButtonEnterIcon, XPUserEntryButtonType.PasswordEnter, m_buttons.Count, true, true );
			}
			
			Rectangle buttonRect = Rectangle.Empty;

			if( this.RightToLeft == RightToLeft.Yes )
			{
				buttonRect = new Rectangle( rectPasswordBox.Left - rectPasswordBox.Height - DEF_BUTTON_SPACE, rectPasswordBox.Top, rectPasswordBox.Height, rectPasswordBox.Height );
			}
			else 
			{
				buttonRect = new Rectangle( rectPasswordBox.Right + INDENT_LEFT_PASSWORDBOX, rectPasswordBox.Top, rectPasswordBox.Height, rectPasswordBox.Height );
			}

			m_ButtonEnter.Bounds = buttonRect;


			if( m_buttons != null && m_buttons.Count > 0 )
			{
				int savedWidth = thumbWidth + INDENT_LEFT_TEXT + m_rectPasswordBox.Width + DEF_RECT_INFLATE + INDENT_LEFT_PASSWORDBOX + m_ButtonEnter.Bounds.Width;

				for( int i = 0, len = m_buttons.Count; i < len; i++ )
				{					
					Rectangle rect = new Rectangle( 
						savedWidth + DEF_BUTTON_SPACE + (DEF_BUTTON_SPACE + rectPasswordBox.Height ) * i, rectPasswordBox.Top,
						rectPasswordBox.Height, rectPasswordBox.Height );					

					m_buttons[ FindNextButton( m_indexButtonDrawing ) ].Bounds = rect;					
				}

				m_indexButtonDrawing = 0;
			}
		}

		private int FindNextButton( int index )
		{
			while( true )
			{
				for( int i = 0, len = m_buttons.Count; i < len; i++ )
				{
					if( m_buttons[ i ].Index == index )
					{
						m_indexButtonDrawing = index + 1;
						return i;
					}
				}
			
				index++;
			}			
		}

		/// <summary>
		/// Get user icon for drawing this time.
		/// </summary>
		private Image IconForDrawing
		{
			get
			{
				Image icon = ( this.IsSelected ) ? this.SelectIcon : this.DefaultIcon;

				if( icon == null )
				{
					int index = ( m_bSelect ) ? this.SelectedImageIndex : this.DefaultImageIndex;
					if( index >= 0 && index < ImageList.Images.Count )
					{
						icon = ImageList.Images[ index ];
					}
				}

				return icon;
			}
		}

		
		/// <summary>
		/// Draw icon shadow.
		/// </summary>
		/// <param name="g">Graphics for paint.</param>
		private void DrawShadow( Graphics g )
		{
			int shadowLength = this.Style.ShadowSize;
			int rtl = 1;
			if( this.RightToLeft == RightToLeft.Yes )
			{
				rtl = -1;
			}

			g.TranslateTransform( shadowLength * rtl, shadowLength );
	
			using( PathGradientBrush pgb = new PathGradientBrush( m_rectIcon ) )
			{
				RectangleF rc = m_rectIcon.GetBounds();
				pgb.CenterPoint = new PointF( rc.Left + rc.Width / 2, rc.Top + rc.Height / 2 );
				pgb.CenterColor = m_colorShadowDark;
				Blend bb = new Blend( 1 );
				bb.Factors = new float[] { 10f };
				pgb.Blend = bb;
				pgb.SurroundColors = new Color[] { m_colorShadowLight };
		
				g.FillPath( pgb, m_rectIcon );
			}
			g.TranslateTransform( -shadowLength * rtl, -shadowLength );
		}
		

		/// <summary>
		/// Retrieves object at the specified screen coordinates.
		/// </summary>
		/// <param name="x">The horizontal screen coordinate.</param>
		/// <param name="y">The vertical screen coordinate.</param>
		/// <returns>Retrieves the child object at the specified screen coordinates.</returns>
		public HitAreas HitTest( int x, int y )
		{
			HitAreas hitArea = HitAreas.None;

			if( m_rectIcon != null )
			{
				if( m_rectIcon.IsVisible( x, y ) )
				{
					hitArea = HitAreas.Icon;
				}
				else if ( m_rectUserName.Contains( x, y ) )
				{
					hitArea = HitAreas.UserName;
				}
				else if ( !this.IsSelected && m_rectUserHelp.Contains( x, y ) )
				{
					hitArea = HitAreas.HelpString;
				}
				else if ( this.IsSelected && m_rectPasswordBox.Contains( x, y ) )
				{
					hitArea = HitAreas.TextBox;
				}
				else if( this.IsSelected && m_ButtonEnter != null && m_ButtonEnter.Bounds.Contains( x, y ) )
				{
					hitArea = HitAreas.ButtonEnter;
				}
				else if ( this.IsSelected && m_buttons.Count != 0 && HitAddedButtons( x, y ) )
				{
					hitArea = HitAreas.ButtonEnter;
				}
			}

			return hitArea;
		}
		
		private bool HitAddedButtons( int x, int y )
		{
			if( m_buttons != null && m_buttons.Count > 0 )
			{
				for( int i = 0, len = m_buttons.Count; i < len; i++ )
				{
					if( m_buttons[i].Bounds.Contains( x, y ) ) return true;
				}
			}
			return false;
		}
		

		/// <summary>
		/// Retrieves object at the specified screen coordinates.
		/// </summary>
		/// <param name="pt">Screen coordinate</param>
		/// <returns>Retrieves object at the specified screen coordinates.</returns>
		public HitAreas HitTest( Point pt )
		{
			return HitTest( pt.X, pt.Y );
		}

		
		/// <summary>
		/// Mirror RectangleF for RigthToLeft representation.
		/// </summary>
		/// <param name="rect">RectangleF as mirror.</param>
		private RectangleF MirrorRectangleF( RectangleF rect )
		{
			PointF location = new PointF( this.Width - ( rect.Width + rect.Left ), rect.Top );

			return new RectangleF( location, rect.Size );
		}
		
		private Rectangle MirrorRectangle( Rectangle rect )
		{			
			Point location = new Point( this.Width - ( rect.Width + rect.Left ), rect.Top );

			return new Rectangle( location, rect.Size );
		}

		/// <summary>
		/// Mirror Region for RigthToLeft representation.
		/// </summary>
		/// <param name="rg">Region as mirror.</param>
		/// <param name="g"></param>
		private void MirrorRegion( Region rg, Graphics g )
		{
			Matrix mtr = new Matrix();
			mtr.Translate( this.Width - ( rg.GetBounds(g).Left*2 + rg.GetBounds(g).Width ) ,0 );
			rg.Transform( mtr );
		}

		/// <summary>
		/// Mirror GraphicsPath for RigthToLeft representation.
		/// </summary>
		/// <param name="gp">GraphicsPath as mirror.</param>
		private void MirrorGraphicsPath( ref GraphicsPath gp )
		{
			Matrix mtr = new Matrix();
			mtr.Translate( this.Width - ( gp.GetBounds().Left*2 + gp.GetBounds().Width ) ,0 );
			gp.Transform( mtr );
		}		

		/// <summary>
		/// Mirror all graphics element for RigthToLeft representation.
		/// </summary>
		/// <param name="g"></param>
		private void MirrorAll( Graphics g )
		{
			m_rectUserHelp = MirrorRectangleF( m_rectUserHelp );
			m_rectUserName = MirrorRectangleF( m_rectUserName );
			
			if( m_ButtonEnter != null )
			{
				m_ButtonEnter.Bounds = MirrorRectangle( m_ButtonEnter.Bounds );
			}			
			
			if( m_buttons != null && m_buttons.Count > 0 )
			{
				for( int i = 0, len = m_buttons.Count; i < len; i++ )
				{
					m_buttons[ i ].Bounds = MirrorRectangle( m_buttons[ i ].Bounds );
				}
			}

			MirrorRegion( m_rgIconRect, g );
			MirrorGraphicsPath( ref m_rectIcon );

			// Mirror PasswordBox
			m_rectPasswordBox.X = this.Width - ( m_rectPasswordBox.Width + m_rectPasswordBox.Left );
		}
		
		#endregion
	}
	
	[
	TypeConverter( typeof(XPUserEntryButtonTypeConverter) )
	, Serializable
	]
	public class XPUserEntryButton
	{
		#region Class constants
		
		/// <summary>
		/// Button name.
		/// </summary>
		private const string BUTTONNAME = "Button";
		
		/// <summary>
		/// Backgroundcolor color of the highlighted button.
		/// </summary>
		private static readonly Color c_cBackgroundHot = Color.FromArgb( 255, 238, 194 );
		
		/// <summary>
		/// Backgroundcolor color of the pressed button.
		/// </summary>
		private static readonly Color c_cBackgroundPressed = Color.FromArgb( 242, 210, 101 );

		#endregion

		#region Class members

		/// <summary>
		/// Represent bounds of the button.
		/// </summary>
		private Rectangle m_bounds;

		/// <summary>
		/// Transparent color of the button's image. 
		/// </summary>
		private Color m_cTransparentImageColor;

		/// <summary>
		/// Index of the button.
		/// </summary>
		private int m_buttonIndex = -1;

		/// <summary>
		/// Shows type of the button.
		/// </summary>
		private XPUserEntryButtonType m_type = XPUserEntryButtonType.Custom;

		/// <summary>
		/// Indicate whether the button is enabled. 
		/// Default value is true.
		/// </summary>
		private bool m_bEnable = true;

		/// <summary>
		/// Indicate whether the button is Visible. 
		/// Default value is true.
		/// </summary>
		private bool m_bVisible = true;

		/// <summary>
		/// Name of the button.
		/// </summary>
		private string m_name = String.Empty;

		/// <summary>
		/// Represent button's image.
		/// Default value is null. 
		/// </summary>
		private Image m_image = null;

		/// <summary>
		/// Indicate button's state.
		/// </summary>
		private XPUserEntryButtonState m_state = XPUserEntryButtonState.Normal;
		
		#endregion

		#region Class initialize/finalize methods
		
		public XPUserEntryButton() : this( XPUserEntryButton.BUTTONNAME )
		{					
		}

		public XPUserEntryButton(string name )
		{
			m_name = name;			
		}
		
		public XPUserEntryButton(string name, int index ) : this( name )
		{		
			m_buttonIndex = index;
		}
		
		public XPUserEntryButton( string name, Image image, XPUserEntryButtonType type, int index, bool enabled, bool visible ) : this( name, index )
		{
			m_image = image;
			m_type = type;
			m_bEnable = enabled;
			m_bVisible = visible;
		}

		#endregion		
		
		#region Class Properties		
		
		/// <summary>
		/// Gets or set button bounds.
		/// </summary>
		protected internal Rectangle Bounds
		{
			get{ return m_bounds; }
			set
			{
				if( m_bounds != value )
					m_bounds = value;
			}
		}
		
		
		/// <summary>
		/// Gets or sets transparent image color.
		/// </summary>
		[Description("Gets or sets transparent image color.")]
		public Color TransparentImageColor
		{
			get
			{
				return m_cTransparentImageColor;
			}
			set
			{
				if( m_cTransparentImageColor != value )
				{
					m_cTransparentImageColor = value;
				}
			}
		}
		
		
		/// <summary>
		/// Gets or sets index of the button.
		/// </summary>
		[DefaultValue(null)]
		public int Index
		{
			get { return m_buttonIndex; }
			set 
			{
				if( value != m_buttonIndex )
					m_buttonIndex = value;
			}
		}			 


		/// <summary>
		/// Gets or sets type of the button.
		/// </summary>
		[DefaultValue(XPUserEntryButtonType.Custom)]
		public XPUserEntryButtonType PredefinedType
		{
			get { return m_type; }
			set
			{
				if( m_type != value )
					m_type = value;
			}
		}


		/// <summary>
		/// Gets or sets if button is enabled.
		/// </summary>
		[DefaultValue(true)
		, Category("Behavior")]
		public bool Enable
		{
			get
			{
				return m_bEnable; 
			}
			set
			{
				if( m_bEnable != value )
					m_bEnable = value;
			}
		}


		/// <summary>
		/// Gets or sets if button is Visible.
		/// </summary>
		[DefaultValue(true)
		, Category("Behavior")]
		public bool Visible
		{
			get
			{ 
				return m_bVisible; 
			}			
			set
			{
				if( m_bVisible != value )
					m_bVisible = value;
			}
		}


		/// <summary>
		/// Gets or sets button's name.
		/// </summary>
		[DefaultValue("")]
		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				if( m_name != value )
					m_name = value;
			}
		}
		

		/// <summary>
		/// Gets or sets image of the button. 
		/// </summary>
		[DefaultValue(null)
		, Category("Appearance")]
		public Image Image
		{
			get
			{
				return m_image; 
			}
			set
			{
				if( m_image != value )
					m_image = value;
			}
		}


		/// <summary>
		/// Gets or sets button' state.
		/// </summary>	
		[DefaultValue(XPUserEntryButtonState.Normal)]
		protected internal XPUserEntryButtonState State
		{
			get
			{
				return m_state;
			}
			set
			{
				if( m_state != value )
					m_state = value;
			}
		}		

		#endregion

		#region Class methods

		public void Draw(Graphics g)
		{
			if( this.Image != null )
			{
				DrawImage( g );
			}
			else
			{
				if( this.State != XPUserEntryButtonState.Normal )
				{
					if( this.State == XPUserEntryButtonState.Highlighted )
					{						
						g.FillRectangle( new SolidBrush( c_cBackgroundHot ), this.Bounds );					
					}
					else if( this.State == XPUserEntryButtonState.Pushed )
					{
						RectangleF rect = this.Bounds;
						rect.Height--;
						rect.Width--;

						g.FillRectangle( new SolidBrush( c_cBackgroundPressed ), rect );
					}

					DrawBorder( g );
				}
			}
		}

		private void DrawImage( Graphics g )
		{
			Bitmap bm = null;

			bm = new Bitmap( this.Image );
			bm.MakeTransparent( this.TransparentImageColor );

			if( this.Enable )
			{
				if( this.State == XPUserEntryButtonState.Highlighted )
				{
					g.DrawImage( bm, this.Bounds );

					g.DrawRectangle( Pens.White, this.Bounds );
				}
				else if( this.State == XPUserEntryButtonState.Pushed )
				{				
					g.DrawImage( bm, this.Bounds );
					
					RectangleF rectF = this.Bounds;
					rectF.Height--;
					rectF.Width--;
					
					Rectangle rect = Rectangle.Ceiling( rectF );

					g.DrawLine( Pens.White, new Point( rect.Right, rect.Top ), new Point( rect.Right, rect.Bottom ) );
					g.DrawLine( Pens.White, new Point( rect.Right, rect.Bottom ), new Point( rect.Left, rect.Bottom ) );					
				}
				else 
				{
					g.DrawImage( bm, this.Bounds );
				}
			}
			else
			{
				RectangleF imageRect = new RectangleF( Point.Empty, this.Image.Size );

				DrawGreyImage( g, bm, this.Bounds, imageRect );
			}
		}

		/// <summary>
		/// Draws grayed image.
		/// </summary>
		private void DrawGreyImage( Graphics g, Image image, Rectangle destRect, RectangleF srcRect )
		{
			ImageAttributes imageAtributes = new ImageAttributes();
			ColorMatrix colorMatrix = new ColorMatrix();

			colorMatrix.Matrix00 = 1/3f;
			colorMatrix.Matrix01 = 1/3f;
			colorMatrix.Matrix02 = 1/3f;
			colorMatrix.Matrix10 = 1/3f;
			colorMatrix.Matrix11 = 1/3f;
			colorMatrix.Matrix12 = 1/3f;
			colorMatrix.Matrix20 = 1/3f;
			colorMatrix.Matrix21 = 1/3f;
			colorMatrix.Matrix22 = 1/3f;
		
			imageAtributes.SetColorMatrix( colorMatrix );

			g.DrawImage( image, destRect, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, imageAtributes );
		
			imageAtributes.Dispose();
		}
		
		private void DrawBorder( Graphics g )
		{
			RectangleF rect = this.Bounds;			
			rect.Width--;
			rect.Height--;
					
			g.DrawRectangle( Pens.Black, Rectangle.Ceiling( rect ) );
		}
		
		#endregion
	}


	[Serializable]
	public enum XPUserEntryButtonState
	{
		Pushed,
		Highlighted,
		Normal
	}


	[Serializable]
	public enum XPUserEntryButtonType
	{
		PasswordEnter,
		Custom
	}


	public class XPUserEntryButtonTypeConverter : TypeConverter
	{
		public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
		{
			XPUserEntryButton button = ( XPUserEntryButton )value;

			if( ( destinationType == typeof( System.ComponentModel.Design.Serialization.InstanceDescriptor ) ) )
			{
				if( button != null )
				{
					if( button.Image != null )
					{
						System.Reflection.ConstructorInfo ci = typeof( XPUserEntryButton ).GetConstructor(
							new Type[]{ typeof(string), typeof(Image), typeof(XPUserEntryButtonType), typeof(int), typeof(bool), typeof(bool) } );

						return new System.ComponentModel.Design.Serialization.InstanceDescriptor( ci, 
							new object[]{ button.Name, button.Image, button.PredefinedType, button.Index, button.Enable, button.Visible } );						
					}
					else if( button.Index != -1 )
					{
						System.Reflection.ConstructorInfo ci = typeof( XPUserEntryButton ).GetConstructor(
							new Type[]{ typeof(string), typeof( int ) } );

						return new System.ComponentModel.Design.Serialization.InstanceDescriptor( ci, 
							new object[]{ button.Name, button.Index } );
					}
					else 
					{
						System.Reflection.ConstructorInfo ci = typeof( XPUserEntryButton ).GetConstructor(
							new Type[]{ typeof(string) } );

						return new System.ComponentModel.Design.Serialization.InstanceDescriptor( ci, 
							new object[]{ button.Name } );
					}
				}
			}
			return base.ConvertTo( context, culture, value, destinationType );
		}
		
		public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType)
		{
			if(destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
				return true;

			return base.CanConvertTo( context, destinationType );
		}
	}

	
	[Serializable]
	public class XPUserEntryButtonsCollection : CollectionBase
	{
		#region Members

		private XpUserEntry m_xpUserEntry;
		
		#endregion

		#region Events

		public event EventHandler CollectionChanged;
		
		#endregion

		protected void OnCollectionChanged()
		{
			if( this.CollectionChanged != null )
			{
				this.CollectionChanged( this, EventArgs.Empty );
			}
		}


		#region Constructors

		internal XPUserEntryButtonsCollection( XpUserEntry xpUserEntry )
		{
			if( xpUserEntry == null )
			{
				throw new NullReferenceException("XPUserEntry can't be NULL");
			}

			m_xpUserEntry = xpUserEntry;
		}

		#endregion

		#region Indexer

		public XPUserEntryButton this[ int index ]
		{
			get
			{
				return (XPUserEntryButton)this.List[ index ];
			}
			set
			{
				if( index < 0 || index >= this.List.Count )
				{
					throw new IndexOutOfRangeException( "index" );
				}
				if( value == null )
				{
					throw new NullReferenceException("value can't be NULL");
				}

				if( this.List[ index ] != value )
				{
					this.List[ index ] = value;
				}
			}
		}
		#endregion

		#region Methods

		public void Add( XPUserEntryButton button )
		{
			if( button == null )
			{
				throw new NullReferenceException( "Button can't be NULL" );
			}
			
			this.List.Add( button );
		}

		public bool Contains( XPUserEntryButton button )
		{
			if( button == null )
			{
				throw new NullReferenceException( "Button can't be NULL" );
			}

			return this.List.Contains( button );
		}

		public void Remove( XPUserEntryButton button )
		{
			if( button == null )
			{
				throw new NullReferenceException( "Button can't be NULL" );
			}

			if( !this.Contains( button ) )
			{
				throw new NullReferenceException( "button is't include in collection" );
			}

			this.List.Remove( button );
		}

		public int IndexOf( XPUserEntryButton button )
		{
			if( button == null )
			{
				throw new NullReferenceException( "Button can't be NULL" );
			}

			return this.List.IndexOf( button );
		}

		public void Insert( int index, XPUserEntryButton button )
		{
			if( button == null )
			{
				throw new NullReferenceException( "button can't be NULL" );
			}

			if( index < 0 || index >= this.List.Count )
			{
				throw new IndexOutOfRangeException( "index" );
			}

			this.List.Insert( index, button );
		}	

		#endregion

		#region Overrides
			
		protected override void OnInsert(int index, object value)
		{
			base.OnInsert (index, value);

			XPUserEntryButton button = (XPUserEntryButton)value;

			if( button.Name == "Button" )
			{
				button.Name = String.Concat("Button", this.Count.ToString());	
			}
			
			if( button.Index == -1 )
			{
				button.Index = this.Count;
			}

			this.OnCollectionChanged();			
		}

		#endregion 
	}
}