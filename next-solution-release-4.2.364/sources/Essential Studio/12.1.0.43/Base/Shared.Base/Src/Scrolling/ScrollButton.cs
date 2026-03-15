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
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Security;
using System.Security.Permissions;

namespace Syncfusion.Windows.Forms
{

	[Syncfusion.Documentation.DocumentationExclude()]
	public class ThemedScrollButtonDrawing: ThemedControlDrawing
	{
		public ThemedScrollButtonDrawing()
			: base( ThemedControls.SCROLLBAR )
		{
		}

		public ThemedScrollButtonDrawing( IComponent component )
			: base( ThemedControls.SCROLLBAR, component )
		{
		}

		// ButtonState.Normal will be drawn as "hot" button and ButtonState.Flat will be drawn as "normal".
		public void DrawScrollButton( Graphics g, Rectangle rect, ButtonID buttonID,
			ScrollButtonAppearance appearance, ButtonState btnState )
		{
			// Get part id.
			int part = ThemeParts.SBP_ARROWBTN;
			int state = 0;
			if( buttonID == ButtonID.Up )
			{
				if( appearance == ScrollButtonAppearance.Horizontal )
				{
					if( ( btnState & ButtonState.Inactive ) > 0 )
						state = ThemeStates.ABS_LEFTDISABLED;
					else if( ( btnState & ButtonState.Pushed ) > 0 )
						state = ThemeStates.ABS_LEFTPRESSED;
					else if( ( btnState & ButtonState.Flat ) > 0 )
						state = ThemeStates.ABS_LEFTNORMAL;
					else
						state = ThemeStates.ABS_LEFTHOT;
				}
				else
				{
					if( ( btnState & ButtonState.Inactive ) > 0 )
						state = ThemeStates.ABS_UPDISABLED;
					else if( ( btnState & ButtonState.Pushed ) > 0 )
						state = ThemeStates.ABS_UPPRESSED;
					else if( ( btnState & ButtonState.Flat ) > 0 )
						state = ThemeStates.ABS_UPNORMAL;
					else
						state = ThemeStates.ABS_UPHOT;
				}
			}
			else
			{
				if( appearance == ScrollButtonAppearance.Horizontal )
				{
					if( ( btnState & ButtonState.Inactive ) > 0 )
						state = ThemeStates.ABS_RIGHTDISABLED;
					else if( ( btnState & ButtonState.Pushed ) > 0 )
						state = ThemeStates.ABS_RIGHTPRESSED;
					else if( ( btnState & ButtonState.Flat ) > 0 )
						state = ThemeStates.ABS_RIGHTNORMAL;
					else
						state = ThemeStates.ABS_RIGHTHOT;
				}
				else
				{
					if( ( btnState & ButtonState.Inactive ) > 0 )
						state = ThemeStates.ABS_DOWNDISABLED;
					else if( ( btnState & ButtonState.Pushed ) > 0 )
						state = ThemeStates.ABS_DOWNPRESSED;
					else if( ( btnState & ButtonState.Flat ) > 0 )
						state = ThemeStates.ABS_DOWNNORMAL;
					else
						state = ThemeStates.ABS_DOWNHOT;
				}
			}

			this.DrawThemeBackground( g, part, state, rect );
		}
	}

	[Syncfusion.Documentation.DocumentationExclude()]
	internal class ScrollImages
	{
		[ThreadStatic]
		internal static Bitmap _downFilled;
		[ThreadStatic]
		internal static Bitmap _downOut;
		[ThreadStatic]
		internal static Bitmap _leftFilled;
		[ThreadStatic]
		internal static Bitmap _leftOut;
		[ThreadStatic]
		internal static Bitmap _upFilled;
		[ThreadStatic]
		internal static Bitmap _upOut;
		[ThreadStatic]
		internal static Bitmap _rightFilled;
		[ThreadStatic]
		internal static Bitmap _rightOut;
        [ThreadStatic]
        internal static Bitmap _vs2010LeftFilled;
        [ThreadStatic]
        internal static Bitmap _vs2010RightFilled;
        [ThreadStatic]
        internal static Bitmap _vs2010DownFilled;
        [ThreadStatic]
        internal static Bitmap _vs2010UpFilled;
        [ThreadStatic]
        internal static Bitmap _vs2010LeftOut;
        [ThreadStatic]
        internal static Bitmap _vs2010RightOut;
        [ThreadStatic]
        internal static Bitmap _vs2010UpOut;
        [ThreadStatic]
        internal static Bitmap _vs2010DownOut;


		public static Bitmap downFilled
		{
			get
			{
				if( _downFilled == null )
				{
					_downFilled = GetBitmap( "Syncfusion.Windows.Forms.Images.DownFilled.bmp" );
					_downFilled.MakeTransparent( Color.White );
				}
				return _downFilled;
			}
		}
		public static Bitmap downOut
		{
			get
			{
				if( _downOut == null )
				{
					_downOut = GetBitmap( "Syncfusion.Windows.Forms.Images.DownOut.bmp" );
					_downOut.MakeTransparent( Color.White );
				}
				return _downOut;
			}
		}
		public static Bitmap leftFilled
		{
			get
			{
				if( _leftFilled == null )
				{
					_leftFilled = GetBitmap( "Syncfusion.Windows.Forms.Images.LeftFilled.bmp" );
					_leftFilled.MakeTransparent( Color.White );
				}
				return _leftFilled;
			}
		}
		public static Bitmap leftOut
		{
			get
			{
				if( _leftOut == null )
				{
					_leftOut = GetBitmap( "Syncfusion.Windows.Forms.Images.LeftOut.bmp" );
					_leftOut.MakeTransparent( Color.White );
				}
				return _leftOut;
			}
		}
		public static Bitmap upFilled
		{
			get
			{
				if( _upFilled == null )
				{
					_upFilled = GetBitmap( "Syncfusion.Windows.Forms.Images.UpFilled.bmp" );
					_upFilled.MakeTransparent( Color.White );
				}
				return _upFilled;
			}
		}
		public static Bitmap upOut
		{
			get
			{
				if( _upOut == null )
				{
					_upOut = GetBitmap( "Syncfusion.Windows.Forms.Images.UpOut.bmp" );
					_upOut.MakeTransparent( Color.White );
				}
				return _upOut;
			}
		}
		public static Bitmap rightFilled
		{
			get
			{
				if( _rightFilled == null )
				{
					_rightFilled = GetBitmap( "Syncfusion.Windows.Forms.Images.RightFilled.bmp" );
					_rightFilled.MakeTransparent( Color.White );
				}
				return _rightFilled;
			}
		}
		public static Bitmap rightOut
		{
			get
			{
				if( _rightOut == null )
				{
					_rightOut = GetBitmap( "Syncfusion.Windows.Forms.Images.RightOut.bmp" );
					_rightOut.MakeTransparent( Color.White );
				}
				return _rightOut;
			}
		}
        public static Bitmap VS2010RightOut
        {
            get
            {
                if (_vs2010RightOut == null)
                {
                    _vs2010RightOut = GetBitmap("Syncfusion.Windows.Forms.Images.VS2010RightOut.bmp");
                    _vs2010RightOut.MakeTransparent(Color.Black);
                }
                return _vs2010RightOut;
            }
        }
        public static Bitmap VS2010RightFilled
        {
            get
            {
                if (_vs2010RightFilled == null)
                {
                    _vs2010RightFilled = GetBitmap("Syncfusion.Windows.Forms.Images.VS2010RightFilled.bmp");
                    _vs2010RightFilled.MakeTransparent(Color.Black);
                }
                return _vs2010RightFilled;
            }
        }
        public static Bitmap VS2010LeftFilled
        {
            get
            {
                if (_vs2010LeftFilled == null)
                {
                    _vs2010LeftFilled = GetBitmap("Syncfusion.Windows.Forms.Images.VS2010LeftFilled.bmp");
                    _vs2010LeftFilled.MakeTransparent(Color.Black);
                }
                return _vs2010LeftFilled;
            }
        }
        public static Bitmap VS2010LeftOut
        {
            get
            {
                if (_vs2010LeftOut == null)
                {
                    _vs2010LeftOut = GetBitmap("Syncfusion.Windows.Forms.Images.VS2010LeftOut.bmp");
                    _vs2010LeftOut.MakeTransparent(Color.Black);
                }
                return _vs2010LeftOut;
            }
        }
        public static Bitmap VS2010DownFilled
        {
            get
            {
                if (_vs2010DownFilled == null)
                {
                    _vs2010DownFilled = GetBitmap("Syncfusion.Windows.Forms.Images.VS2010DownFilled.bmp");
                    _vs2010DownFilled.MakeTransparent(Color.Black);
                }
                return _vs2010DownFilled;
            }
        }
        public static Bitmap VS2010DownOut
        {
            get
            {
                if (_vs2010DownOut == null)
                {
                    _vs2010DownOut = GetBitmap("Syncfusion.Windows.Forms.Images.VS2010DownOut.bmp");
                    _vs2010DownOut.MakeTransparent(Color.Black);
                }
                return _vs2010DownOut;
            }
        }
        public static Bitmap VS2010UpFilled
        {
            get
            {
                if (_vs2010UpFilled == null)
                {
                    _vs2010UpFilled = GetBitmap("Syncfusion.Windows.Forms.Images.VS2010UpFilled.bmp");
                    _vs2010UpFilled.MakeTransparent(Color.Black);
                }
                return _vs2010UpFilled;
            }
        }
        public static Bitmap VS2010UpOut
        {
            get
            {
                if (_vs2010UpOut == null)
                {
                    _vs2010UpOut = GetBitmap("Syncfusion.Windows.Forms.Images.VS2010UpOut.bmp");
                    _vs2010UpOut.MakeTransparent(Color.Black);
                }
                return _vs2010UpOut;
            }
        }
		static Bitmap GetBitmap( string resourceName )
		{
			Type type = typeof( ScrollImages );
			Assembly assembly = type.Module.Assembly;
			string[] resourceNames = assembly.GetManifestResourceNames();
			Stream stream = assembly.GetManifestResourceStream( resourceName );//cursorNS + cursorName
			return new Bitmap( stream );
		}
	}

	[ToolboxItem( false ),
	Syncfusion.Documentation.DocumentationExclude()]
	public class ScrollButtons:
		ThemedControl
	{
		private const float DEF_SELECTIONCOLOR_PERCENT = 0.3f;
		private const float DEF_BACKCOLOR_PERCENT = 1.3f;

		// Fields
		private bool m_bIsReverseGradient = false;
		private ButtonID pushed = 0;
		private ButtonID captured = 0;
		private Timer timer = new Timer();
		private int timerInterval;
		private ButtonID tracking = ButtonID.None;
		private bool vsLikeScrollButton = false;
		private ThemedScrollButtonDrawing themedDrawing = null;
		private VisualStyle m_style = VisualStyle.Default;
		private Brush m_backgroundBrush = null;
		private int m_gradientOffset = 0;
		private Office2007Theme m_Office2007ColorScheme = Office2007Theme.Blue;
		protected Office2007Colors m_office2007ColorTable = null;
        private Office2010Theme m_Office2010ColorScheme = Office2010Theme.Blue;
        protected Office2010Colors m_office2010ColorTable = null;
		protected Color m_arrowColor = Color.FromArgb( 0, 21, 110 );

		// Properties
		#region PROPERTIES
		public Brush BackGroundBrush
		{
			get
			{
				return m_backgroundBrush;
			}
			set
			{
				if( m_backgroundBrush != value )
				{
					m_backgroundBrush = value;
				}
			}
		}

		public ButtonID Tracking
		{
			get
			{
				return tracking;
			}

			set
			{
				if( value != tracking )
				{
					tracking = value;
				}
			}
		}

		public bool IsReverseGradient
		{
			get
			{
				return m_bIsReverseGradient;
			}
			set
			{
				if( value != m_bIsReverseGradient )
				{
					m_bIsReverseGradient = value;
				}
			}
		}

		public VisualStyle Style
		{
			get
			{
				return m_style;
			}
			set
			{
				if( value != m_style )
				{
					m_style = value;

					if( m_style == VisualStyle.Office2007 )
					{
						m_office2007ColorTable = Office2007Colors.GetColorTable( m_Office2007ColorScheme );
					}
                    else if (m_style == VisualStyle.Office2010)
                    {
                        m_office2010ColorTable = Office2010Colors.GetColorTable(m_Office2010ColorScheme);
                    }
					Invalidate();
				}
			}
		}

		private ScrollButtonAppearance scrollAppearence = ScrollButtonAppearance.Horizontal;
		public ScrollButtonAppearance ScrollButtonAppearance
		{
			get { return scrollAppearence; }
			set
			{
				if( scrollAppearence != value )
				{
					scrollAppearence = value;
					OnAlignmentChanged();
				}
			}
		}
		private ButtonState buttonState;
		public ButtonState ButtonState
		{
			get { return buttonState; }
			set { buttonState = value; }
		}

		private bool maxButtonActive = true;
		public bool MaxButtonActive
		{
			get { return this.maxButtonActive; }
			set
			{
				if( this.maxButtonActive != value )
				{
					this.maxButtonActive = value;
					if( !this.maxButtonActive )
						this.EndButtonPress();

					this.Invalidate();
				}
			}
		}
		private bool minButtonActive = true;
		public bool MinButtonActive
		{
			get { return this.minButtonActive; }
			set
			{
				if( this.minButtonActive != value )
				{
					this.minButtonActive = value;
					if( !this.minButtonActive )
						this.EndButtonPress();

					this.Invalidate();
				}
			}
		}
		[
		DefaultValue( false )
		]
		public bool VSLikeButton
		{
			get { return this.vsLikeScrollButton; }
			set
			{
				this.vsLikeScrollButton = value;
				this.Invalidate();
			}
		}

		private bool m_bSelected = false;

		/// <summary>
		/// Gets / sets ScrollButton highlighted state.
		/// </summary>
		public bool Selected
		{
			get
			{
				return m_bSelected;
			}
			set
			{
				m_bSelected = value;
			}
		}

		[
		Browsable( true ),
		Category( "Appearance" ),
		Description( "Specifies color scheme for the control." ),
		DefaultValue( Office2007Theme.Blue )
		]
		public Office2007Theme Office2007ColorScheme
		{
			get
			{
				return m_Office2007ColorScheme;
			}
			set
			{
				if( m_Office2007ColorScheme != value )
				{
					m_Office2007ColorScheme = value;

					if( this.Style == VisualStyle.Office2007 )
					{
						m_office2007ColorTable = Office2007Colors.GetColorTable( m_Office2007ColorScheme );

						this.Invalidate();
					}
				}
			}
		}
        [
        Browsable(true),
        Category("Appearance"),
        Description("Specifies color scheme for the control."),
        DefaultValue(Office2010Theme.Blue)
        ]
        public Office2010Theme Office2010ColorScheme
        {
            get
            {
                return m_Office2010ColorScheme;
            }
            set
            {
                if (m_Office2010ColorScheme != value)
                {
                    m_Office2010ColorScheme = value;

                    if (this.Style == VisualStyle.Office2010)
                    {
                        m_office2010ColorTable = Office2010Colors.GetColorTable(m_Office2010ColorScheme);

                        this.Invalidate();
                    }
                }
            }
        }
		#endregion PROPERTIES

		protected virtual void OnAlignmentChanged()
		{

		}

		/// <override/>
		//		protected override void OnThemeChanged(EventArgs e)
		//		{
		//			base.OnThemeChanged(e);
		//			this.Invalidate();
		//		}
		// Constructors
		public ScrollButtons()
		{
			//			this.SetStyle((ControlStyles.FixedHeight|ControlStyles.FixedWidth)/*~(WhidbeyCompatibleControlStyles.DoubleBuffer|ControlStyles.EnableNotifyMessage|ControlStyles.CacheText|ControlStyles.AllPaintingInWmPaint|ControlStyles.StandardDoubleClick|ControlStyles.SupportsTransparentBackColor|ControlStyles.UserMouse|ControlStyles.Selectable|ControlStyles.StandardClick|ControlStyles.ResizeRedraw|ControlStyles.Opaque|ControlStyles.UserPaint|ControlStyles.ContainerControl)*/,true);
			this.SetStyle( ControlStyles.Selectable, false );
			this.timer.Tick += new EventHandler( this.TimerHandler );

		}

		/// <override/>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				m_backgroundBrush = null;
			}
			this.timer.Tick -= new EventHandler( this.TimerHandler );
			base.Dispose( disposing );
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			if (XPThemes.IsThemedOS)
			{
				this.themedDrawing = new ThemedScrollButtonDrawing(this);
			}
		}
		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (themedDrawing != null)
			{
				this.themedDrawing.Dispose();
				this.themedDrawing = null;
			}

			base.OnHandleDestroyed(e);
		}

		// Properties

		// Events
		public event UpDownEventHandler UpDown;

		// Methods

		protected virtual void OnUpDown( UpDownEventArgs upevent )
		{
			if( this.UpDown != null )
				this.UpDown( (object)this, upevent );
		}

		/// <override/>
		protected override /*Control*/ void OnPaint( PaintEventArgs e )
		{
			if( this.vsLikeScrollButton )
				this.OnPaint2D( e );
			else
				this.OnPaint3D( e );
		}

		/// <summary>
		/// Blend 30% of menu selection color.
		/// </summary>
		private static Color BlendColor( Color selectionColor, Color backColor )
		{
			Color blendedColor = Color.FromArgb(
				(int)( ( DEF_SELECTIONCOLOR_PERCENT * selectionColor.R + backColor.R ) / DEF_BACKCOLOR_PERCENT ),
				(int)( ( DEF_SELECTIONCOLOR_PERCENT * selectionColor.G + backColor.G ) / DEF_BACKCOLOR_PERCENT ),
				(int)( ( DEF_SELECTIONCOLOR_PERCENT * selectionColor.B + backColor.B ) / DEF_BACKCOLOR_PERCENT ) );

			return blendedColor;
		}

		/// <summary>
		/// Gets / sets for correctly GradientBrush creation, so that background
		/// is same as Parent's background.
		/// </summary>
		public int GradientInflateOffset
		{
			get
			{
				return m_gradientOffset;
			}
			set
			{
				if( value != m_gradientOffset )
				{
					m_gradientOffset = value;
				}
			}
		}

		protected virtual void OnPaint2D( PaintEventArgs e )
		{
			int n0;
			System.Drawing.Size size1;
			size1 = this.ClientSize;
			n0 = ( size1.Height / 2 );

			// Background			

			Graphics g = e.Graphics;

			if( m_backgroundBrush != null )
			{
				g.FillRectangle( m_backgroundBrush, this.ClientRectangle );
			}
			ScrollButton scroll1, scroll2;

			if( this.ScrollButtonAppearance == ScrollButtonAppearance.Horizontal )
			{
				scroll1 = ScrollButton.Left;
				scroll2 = ScrollButton.Right;
			}
			else
			{
				scroll1 = ScrollButton.Up;
				scroll2 = ScrollButton.Down;
			}

			if( this.tracking == (ButtonID)1 )
			{
				if( this.minButtonActive )
				{
					this.DrawScrollButton( g, this.GetButtonRect( ButtonID.Up ), scroll1, ButtonState.Checked | this.buttonState );
				}
				else
				{
					this.DrawScrollButton( g, this.GetButtonRect( ButtonID.Up ), scroll1, ButtonState.Inactive | this.buttonState );
				}
			}
			else if( this.tracking == (ButtonID)2 )
			{
				if( this.maxButtonActive )
				{
					this.DrawScrollButton( g, this.GetButtonRect( ButtonID.Down ), scroll2, ButtonState.Checked | this.buttonState );
				}
				else
				{
					this.DrawScrollButton( g, this.GetButtonRect( ButtonID.Down ), scroll2, ButtonState.Inactive | this.buttonState );
				}
			}

			// Draw buttons
			if( this.pushed == (ButtonID)1 )
			{
				this.DrawScrollButton( g, this.GetButtonRect( ButtonID.Up ), scroll1, ButtonState.Pushed | this.buttonState );
			}
			else
			{
				if( this.minButtonActive )
				{
					this.DrawScrollButton( g, this.GetButtonRect( ButtonID.Up ), scroll1, this.buttonState );
				}
				else
				{
					this.DrawScrollButton( g, this.GetButtonRect( ButtonID.Up ), scroll1, this.buttonState | ButtonState.Inactive );
				}
			}

			if( this.pushed == (ButtonID)2 )
			{
				this.DrawScrollButton( g, this.GetButtonRect( ButtonID.Down ), scroll2, ButtonState.Pushed | this.buttonState );
			}
			else
			{
				if( this.maxButtonActive )
				{
					this.DrawScrollButton( g, this.GetButtonRect( ButtonID.Down ), scroll2, this.buttonState );
				}
				else
				{
					this.DrawScrollButton( g, this.GetButtonRect( ButtonID.Down ), scroll2, this.buttonState | ButtonState.Inactive );
				}
			}
		}

		protected virtual void DrawOffice2007ScrollButtonBackground( Graphics g, Rectangle rect, ButtonState buttonState )
		{
			Brush brush;

			if( this.Enabled )
			{
				Blend blend = new Blend();
				Color color1 = Color.Empty, color2 = Color.Empty;

				if( ( buttonState & ButtonState.Pushed ) > 0 )
				{
					color1 = m_office2007ColorTable.DataTimePickerDropDownSelectedLightColor;
					color2 = m_office2007ColorTable.DataTimePickerDropDownSelectedDarkColor;
				}
				else if( buttonState == ButtonState.Checked )
				{
					color1 = m_office2007ColorTable.DataTimePickerDropDownHighLightLightColor;
					color2 = m_office2007ColorTable.DataTimePickerDropDownHighLightDarkColor;
				}
				else if( this.Selected )
				{
					color1 = m_office2007ColorTable.DataTimePickerDropDownLightColor;
					color2 = m_office2007ColorTable.DataTimePickerDropDownDarkColor;
				}

				blend.Positions = new float[] { 0.0F, 0.45F, 0.45F + 0.001F, 1.0F };

				if( buttonState == ButtonState.Pushed )
				{
					blend.Factors = new float[] { 0.0F, 0.8F, 1.0F, 0.4F };
				}
				else
				{
					blend.Factors = new float[] { 0.0F, 0.5F, 1.0F, 0.0F };
				}

				brush = new LinearGradientBrush( rect, color1, color2, LinearGradientMode.Vertical );
				( brush as LinearGradientBrush ).Blend = blend;
			}
			else
			{
				brush = new SolidBrush( this.BackColor );
			}

			g.FillRectangle( brush, rect );
            brush.Dispose();
		}

        protected virtual void DrawOffice2010ScrollButtonBackground(Graphics g, Rectangle rect, ButtonState buttonState)
        {
            Brush brush;

            if (this.Enabled)
            {
                Blend blend = new Blend();
                Color color1 = Color.Empty, color2 = Color.Empty;

                if ((buttonState & ButtonState.Pushed) > 0)
                {
                    color1 = m_office2010ColorTable.DataTimePickerDropDownSelectedLightColor;
                    color2 = m_office2010ColorTable.DataTimePickerDropDownSelectedDarkColor;
                }
                else if (buttonState == ButtonState.Checked)
                {
                    color1 = m_office2010ColorTable.DataTimePickerDropDownHighLightLightColor;
                    color2 = m_office2010ColorTable.DataTimePickerDropDownHighLightDarkColor;
                }
                else if (this.Selected)
                {
                    color1 = m_office2010ColorTable.DataTimePickerDropDownLightColor;
                    color2 = m_office2010ColorTable.DataTimePickerDropDownDarkColor;
                }

                blend.Positions = new float[] { 0.0F, 0.45F, 0.45F + 0.001F, 1.0F };

                if (buttonState == ButtonState.Pushed)
                {
                    blend.Factors = new float[] { 0.0F, 0.8F, 1.0F, 0.4F };
                }
                else
                {
                    blend.Factors = new float[] { 0.0F, 0.5F, 1.0F, 0.0F };
                }

                brush = new LinearGradientBrush(rect, color1, color2, LinearGradientMode.Vertical);
                (brush as LinearGradientBrush).Blend = blend;
            }
            else
            {
                brush = new SolidBrush(this.BackColor);
            }

            g.FillRectangle(brush, rect);
            brush.Dispose();
        }

		protected virtual void DrawOffice2007ScrollButtonArrow( Graphics g, Rectangle rect, ScrollButton scroll, ButtonState buttonState )
		{
			if( !this.VSLikeButton )
			{
				using( GraphicsPath path = new GraphicsPath() )
				{
					Point[] points = new Point[] { };

					if( scroll == ScrollButton.Up )
					{
						points = new Point[] {
                                                new Point( rect.Width / 2 - 2, rect.Height / 2 + 2 ),
                                                new Point( rect.Width / 2, rect.Height / 2 - 1 ),
                                                new Point( rect.Width / 2 + 2, rect.Height / 2 + 2 )
                                                };
					}
					else if( scroll == ScrollButton.Down )
					{
						points = new Point[] {
                                                new Point( rect.Width / 2 - 1, rect.Height + rect.Height / 2 ),
                                                new Point( rect.Width / 2, rect.Height + rect.Height / 2 + 2 ),
                                                new Point( rect.Width / 2 + 2, rect.Height + rect.Height / 2 )
                                                };
					}
					else if( scroll == ScrollButton.Left )
					{
						points = new Point[] {
                                                new Point( rect.Width / 2 + 2, rect.Height / 2 - 2 ),
                                                new Point( rect.Width / 2, rect.Height / 2 ),
                                                new Point( rect.Width / 2 + 2, rect.Height / 2 + 2)
                                                };
					}
					else
					{
						points = new Point[] {
                                                new Point( rect.Width + rect.Width / 2, rect.Height / 2 - 2 ),
                                                new Point( rect.Width + rect.Width / 2 + 2, rect.Height / 2 ),
                                                new Point( rect.Width + rect.Width / 2, rect.Height / 2 + 2 )
                                                };
					}

					path.AddLines( points );

					using( Brush brush = new SolidBrush( m_arrowColor ) )
					{
						g.FillPath( brush, path );
					}
				}
			}
			else
			{
				this.DrawVSLikeButtonArrow( g, rect, scroll, Office2007Colors.GetColorTable( Office2007ColorScheme ).TabScrollArrowColor, buttonState );
			}
		}
        protected virtual void DrawOffice2010ScrollButtonArrow(Graphics g, Rectangle rect, ScrollButton scroll, ButtonState buttonState)
        {
            if (!this.VSLikeButton)
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    Point[] points = new Point[] { };

                    if (scroll == ScrollButton.Up)
                    {
                        points = new Point[] {
                                                new Point( rect.Width / 2 - 2, rect.Height / 2 + 2 ),
                                                new Point( rect.Width / 2, rect.Height / 2 - 1 ),
                                                new Point( rect.Width / 2 + 2, rect.Height / 2 + 2 )
                                                };
                    }
                    else if (scroll == ScrollButton.Down)
                    {
                        points = new Point[] {
                                                new Point( rect.Width / 2 - 1, rect.Height + rect.Height / 2 ),
                                                new Point( rect.Width / 2, rect.Height + rect.Height / 2 + 2 ),
                                                new Point( rect.Width / 2 + 2, rect.Height + rect.Height / 2 )
                                                };
                    }
                    else if (scroll == ScrollButton.Left)
                    {
                        points = new Point[] {
                                                new Point( rect.Width / 2 + 2, rect.Height / 2 - 2 ),
                                                new Point( rect.Width / 2, rect.Height / 2 ),
                                                new Point( rect.Width / 2 + 2, rect.Height / 2 + 2)
                                                };
                    }
                    else
                    {
                        points = new Point[] {
                                                new Point( rect.Width + rect.Width / 2, rect.Height / 2 - 2 ),
                                                new Point( rect.Width + rect.Width / 2 + 2, rect.Height / 2 ),
                                                new Point( rect.Width + rect.Width / 2, rect.Height / 2 + 2 )
                                                };
                    }

                    path.AddLines(points);

                    using (Brush brush = new SolidBrush(m_arrowColor))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
            else
            {
                this.DrawVSLikeButtonArrow(g, rect, scroll, Office2010Colors.GetColorTable(Office2010ColorScheme).TabScrollArrowColor, buttonState);
            }
        }

		protected virtual void DrawVSLikeButtonArrow( Graphics g, Rectangle rect, System.Windows.Forms.ScrollButton scroll, Color arrowColor, ButtonState buttonState )
		{
			if( ( buttonState & ButtonState.Inactive ) == 0 )
			{
				using( GraphicsPath path = this.GetNormalArrowPath( rect, ( scroll == ScrollButton.Up || scroll == ScrollButton.Left ) ) )
				{
					using( SolidBrush brush = new SolidBrush( arrowColor ) )
					{
						g.FillPath( brush, path );
					}
				}
			}
			else
			{
				using( GraphicsPath path = this.GetInactiveArrowPath( rect, ( scroll == ScrollButton.Up || scroll == ScrollButton.Left ) ) )
				{
					using( Pen pen = new Pen( arrowColor ) )
					{
						g.DrawPath( pen, path );
					}
				}
			}
		}

		protected virtual void DrawScrollButton( Graphics g, Rectangle rect, ScrollButton scroll, ButtonState buttonState )
		{
			bool drawBorder = false;
			if( !this.Enabled )
			{
				buttonState |= ButtonState.Inactive;
			}

			if( this.Style == VisualStyle.Office2007 )
			{
				DrawOffice2007ScrollButtonBackground( g, rect, buttonState );
			}
            else if (this.Style == VisualStyle.Office2010)
            {
                DrawOffice2010ScrollButtonBackground(g, rect, buttonState);
            }

			if( ( buttonState & ButtonState.Pushed ) > 0 )
			{

				if( this.Style == VisualStyle.Office2007 )
				{
                    this.DrawOffice2007Border(g, rect, scroll, buttonState);                    
				}
                else if (this.Style == VisualStyle.Office2010)
                {
                    this.DrawOffice2010Border(g, rect, scroll, buttonState);
                }
				else
				{
                    using (Pen pen = new Pen(SystemColors.ControlDarkDark))
                    {
                        g.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom - 1);
                        g.DrawLine(pen, rect.Left, rect.Top, rect.Right - 1, rect.Top);
                    }
				}
			}
			else if( tracking != ButtonID.None && ( buttonState & ButtonState.Inactive ) == 0 )
			{
				switch( tracking )
				{
					case ButtonID.Down:
					if( scroll == ScrollButton.Right || scroll == ScrollButton.Down )
						drawBorder = true;
					break;
					case ButtonID.Up:
					if( scroll == ScrollButton.Up || scroll == ScrollButton.Left )
						drawBorder = true;
					break;
				}

				if( drawBorder && this.Style != VisualStyle.Metro)
				{
					if( this.Style == VisualStyle.Office2007 )
					{
                        this.DrawOffice2007Border(g, rect, scroll, buttonState);
                    }
                    else if (this.Style == VisualStyle.Office2010)
                    {
                        this.DrawOffice2010Border(g, rect, scroll, buttonState);
                    }
                    else if (this.Style == VisualStyle.VS2010)                        
                    {
                        Color startColor = Color.FromArgb(111, 119, 118);
                        Color endColor = Color.FromArgb(79, 95, 116);
                        using (Brush brush = new SolidBrush(Color.FromArgb(255, 252, 244)))
                        {  
                            g.FillRectangle(brush, rect);
                        }
                        using (Pen pen = new Pen(Color.FromArgb(229, 195, 101)))
                        {
                            Rectangle rectangle = new Rectangle(rect.X, rect.Y, rect.Width -1, rect.Height - 1);
                            g.DrawRectangle(pen, rectangle);
                        }
                    }
                    else
                    {
                        Pen pen1 = new Pen(SystemColors.ControlLightLight);
                        Pen pen2 = new Pen(SystemColors.ControlDarkDark);
                        g.DrawLine(pen1, rect.Left, rect.Top, rect.Left, rect.Bottom - 1);
                        g.DrawLine(pen1, rect.Left, rect.Top, rect.Right - 1, rect.Top);
                        g.DrawLine(pen2, rect.Right - 1, rect.Top + 1, rect.Right - 1, rect.Bottom - 1);
                        g.DrawLine(pen2, rect.Right - 1, rect.Bottom - 1, rect.Left, rect.Bottom - 1);
                        pen1.Dispose();
                        pen2.Dispose();
                    }
				}
			}
			else if( this.Selected && this.Style == VisualStyle.Office2007 )
			{
				this.DrawOffice2007Border( g, rect, scroll, buttonState );
			}
            else if( this.Selected && this.Style == VisualStyle.Office2010 )
			{
				this.DrawOffice2010Border( g, rect, scroll, buttonState );
			}

            if (this.Style != VisualStyle.Office2007 && this.Style != VisualStyle.Office2010)
			{
				if (this.Style == VisualStyle.Metro && drawBorder)
				{
					SolidBrush brush = new SolidBrush(ColorTranslator.FromHtml("#119EDA"));
					g.FillRectangle(brush, rect);
					brush.Dispose();
				}

				switch( scroll )
				{
					case ScrollButton.Left:
					rect = new Rectangle( rect.Left + ( rect.Width - 6 ) / 2, rect.Top + ( rect.Height - 11 ) / 2, 6, 11 );
                    if (this.Style == VisualStyle.VS2010)
                    {
                        Bitmap bitMap;
                        if (this.tracking == ButtonID.Up && (buttonState & ButtonState.Inactive) == 0)
                        {
                            bitMap = (buttonState & ButtonState.Inactive) == 0 ? ScrollImages.leftFilled : ScrollImages.leftOut;
                        }
                        else
                        {
                            bitMap = (buttonState & ButtonState.Inactive) == 0 ? ScrollImages.VS2010LeftFilled : ScrollImages.VS2010LeftOut;
                        }
                        this.DrawScrollImage(g, rect, bitMap);
                    }
                    else
                    {
                        this.DrawScrollImage(g, rect,
                            (buttonState & ButtonState.Inactive) == 0 ? ScrollImages.leftFilled : ScrollImages.leftOut);
                    }                    
					//					this.DrawTriangle(g, 
					//									new Point[]{new Point(rect.Left, rect.Top + (int)Math.Floor((double)rect.Height/2)), 
					//														new Point(rect.Right - 1, rect.Top),
					//														new Point(rect.Right - 1, rect.Bottom - 1)},
					//									(buttonState & ButtonState.Inactive) == 0);
					break;
					case ScrollButton.Right:
					rect = new Rectangle( rect.Right - 7 - ( rect.Width - 6 ) / 2, rect.Top + ( rect.Height - 11 ) / 2, 6, 11 );
                    
                    if (this.Style == VisualStyle.VS2010)
                    {
                        Bitmap bitMap;
                        if (this.tracking == ButtonID.Down && (buttonState & ButtonState.Inactive) == 0)
                        {
                            bitMap = (buttonState & ButtonState.Inactive) == 0 ? ScrollImages.rightFilled : ScrollImages.rightOut;
                        }
                        else
                        {
                            bitMap = (buttonState & ButtonState.Inactive) == 0 ? ScrollImages.VS2010RightFilled : ScrollImages.VS2010RightOut;
                        }
                        this.DrawScrollImage(g, rect, bitMap);
                    }
                    else
                    {
                        this.DrawScrollImage(g, rect,
                            (buttonState & ButtonState.Inactive) == 0 ? ScrollImages.rightFilled : ScrollImages.rightOut);
                    }
                    

					break;
					case ScrollButton.Up:
					rect = new Rectangle( rect.Left + ( rect.Width - 11 ) / 2, rect.Top + ( rect.Height - 6 ) / 2, 11, 6 );
                    if (this.Style == VisualStyle.VS2010)
                    {
                        Bitmap bitMap;
                        if (this.tracking == ButtonID.Up && (buttonState & ButtonState.Inactive) == 0)
                        {
                            bitMap = (buttonState & ButtonState.Inactive) == 0 ? ScrollImages.upFilled : ScrollImages.upOut;
                        }
                        else
                        {
                            bitMap = (buttonState & ButtonState.Inactive) == 0 ? ScrollImages.VS2010UpFilled : ScrollImages.VS2010UpOut;
                        }
                        this.DrawScrollImage(g, rect, bitMap);
                    }
                    else
                    {
                        this.DrawScrollImage(g, rect,
                            (buttonState & ButtonState.Inactive) == 0 ? ScrollImages.upFilled : ScrollImages.upOut);
                    }
					break;
					case ScrollButton.Down:
					rect = new Rectangle( rect.Left + ( rect.Width - 11 ) / 2, rect.Bottom - 7 - ( rect.Height - 6 ) / 2, 11, 6 );
                    if (this.Style == VisualStyle.VS2010)
                    {
                        Bitmap bitMap;
                        if (this.tracking == ButtonID.Down && (buttonState & ButtonState.Inactive) == 0)
                        {
                            bitMap = (buttonState & ButtonState.Inactive) == 0 ? ScrollImages.downFilled : ScrollImages.downOut;
                        }
                        else
                        {
                            bitMap = (buttonState & ButtonState.Inactive) == 0 ? ScrollImages.VS2010DownFilled : ScrollImages.VS2010DownOut;
                        }
                        this.DrawScrollImage(g, rect, bitMap);
                    }
                    else
                    {
                        this.DrawScrollImage(g, rect,
                            (buttonState & ButtonState.Inactive) == 0 ? ScrollImages.downFilled : ScrollImages.downOut);
                    }
					break;
				}
			}
			else if(this.Style == VisualStyle.Office2007)
			{
				DrawOffice2007ScrollButtonArrow( g, rect, scroll, buttonState );
			}
            else if (this.Style == VisualStyle.Office2010)
            {
                DrawOffice2010ScrollButtonArrow(g, rect, scroll, buttonState);
            }
		}

		private GraphicsPath GetNormalArrowPath( Rectangle scrollRect, bool upScroll )
		{
			Point[] ptsdropdown;
			Point loc;
			Size size;
			Rectangle rcArrow;

			if( this.ScrollButtonAppearance == ScrollButtonAppearance.Vertical )
			{
				loc = new Point( scrollRect.X + scrollRect.Width / 2 - 4, scrollRect.Y + scrollRect.Height / 2 - 2 );
				size = new Size( 9, 5 );

				rcArrow = new Rectangle( loc, size );

				if( upScroll )
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
			}
			else
			{
				loc = new Point( scrollRect.X + scrollRect.Width / 2 - 2, scrollRect.Y + scrollRect.Height / 2 - 4 );
				size = new Size( 5, 9 );

				rcArrow = new Rectangle( loc, size );

				if( upScroll )
				{
					ptsdropdown = new Point[] { 
												new Point(rcArrow.Left, rcArrow.Top + 4),
												new Point(rcArrow.Left + 1, rcArrow.Top + 4),
												new Point(rcArrow.Left + 1, rcArrow.Top + 5),
												new Point(rcArrow.Left + 2, rcArrow.Top + 5),
												new Point(rcArrow.Left + 2, rcArrow.Top + 6),
												new Point(rcArrow.Left + 3, rcArrow.Top + 6),
												new Point(rcArrow.Left + 3, rcArrow.Top + 7),
												new Point(rcArrow.Left + 4, rcArrow.Top + 7),
												new Point(rcArrow.Left + 4, rcArrow.Top + 8),
												new Point(rcArrow.Left + 5, rcArrow.Top + 8),												
												new Point(rcArrow.Left + 5, rcArrow.Top + 9),
												new Point(rcArrow.Left + 6, rcArrow.Top),
												new Point(rcArrow.Left + 5, rcArrow.Top),
												new Point(rcArrow.Left + 5, rcArrow.Top + 1),
												new Point(rcArrow.Left + 4, rcArrow.Top + 1),
												new Point(rcArrow.Left + 4, rcArrow.Top + 2),
												new Point(rcArrow.Left + 3, rcArrow.Top + 2),
												new Point(rcArrow.Left + 3, rcArrow.Top + 3),
												new Point(rcArrow.Left + 2, rcArrow.Top + 3),
												new Point(rcArrow.Left + 2, rcArrow.Top + 4)
											};
				}
				else
				{
					ptsdropdown = new Point[] { 
												  new Point(rcArrow.Left, rcArrow.Top + 9),
												  new Point(rcArrow.Left + 1, rcArrow.Top + 9),
												  new Point(rcArrow.Left + 1, rcArrow.Top + 8),
												  new Point(rcArrow.Left + 2, rcArrow.Top + 8),
												  new Point(rcArrow.Left + 2, rcArrow.Top + 7),
												  new Point(rcArrow.Left + 3, rcArrow.Top + 7),
												  new Point(rcArrow.Left + 3, rcArrow.Top + 6),
												  new Point(rcArrow.Left + 4, rcArrow.Top + 6),
												  new Point(rcArrow.Left + 4, rcArrow.Top + 5),
												  new Point(rcArrow.Left + 5, rcArrow.Top + 5),
												  new Point(rcArrow.Left + 5, rcArrow.Top + 4),
												  new Point(rcArrow.Left + 4, rcArrow.Top + 4),
												  new Point(rcArrow.Left + 4, rcArrow.Top + 3),
												  new Point(rcArrow.Left + 3, rcArrow.Top + 3),
												  new Point(rcArrow.Left + 3, rcArrow.Top + 2),
												  new Point(rcArrow.Left + 2, rcArrow.Top + 2),
												  new Point(rcArrow.Left + 2, rcArrow.Top + 1),
												  new Point(rcArrow.Left + 1, rcArrow.Top + 1),
												  new Point(rcArrow.Left + 1, rcArrow.Top),
												  new Point(rcArrow.Left, rcArrow.Top)	  
											  };
				}
			}
			GraphicsPath path = new GraphicsPath();
			path.AddLines( ptsdropdown );

			return path;
		}

		private GraphicsPath GetInactiveArrowPath( Rectangle scrollRect, bool upScroll )
		{
			Point[] ptsdropdown;
			Point loc;
			Size size;
			Rectangle rcArrow;

			if( this.ScrollButtonAppearance == ScrollButtonAppearance.Vertical )
			{

				loc = new Point( scrollRect.X + scrollRect.Width / 2 - 4, scrollRect.Y + scrollRect.Height / 2 - 2 );
				size = new Size( 9, 5 );

				rcArrow = new Rectangle( loc, size );

				if( upScroll )
				{
					ptsdropdown = new Point[] { 
												new Point( rcArrow.Left + 4, rcArrow.Top + 1 ),																							
												new Point( rcArrow.Left + 8, rcArrow.Top + 5 ),
												new Point( rcArrow.Left, rcArrow.Top + 5 ),
                                                new Point( rcArrow.Left + 4, rcArrow.Top + 1 )
											  };
				}
				else
				{
					ptsdropdown = new Point[] { 
				  								new Point( rcArrow.Left + 4, rcArrow.Top + 4 ),																							
												new Point( rcArrow.Left + 8, rcArrow.Top ),
												new Point( rcArrow.Left, rcArrow.Top ),
                                                new Point( rcArrow.Left + 4, rcArrow.Top + 4 )
											  };
				}
			}
			else
			{
				loc = new Point( scrollRect.X + scrollRect.Width / 2 - 2, scrollRect.Y + scrollRect.Height / 2 - 4 );
				size = new Size( 5, 9 );

				rcArrow = new Rectangle( loc, size );

				if( upScroll )
				{
					ptsdropdown = new Point[] { 
												new Point( rcArrow.Left + 1, rcArrow.Top + 4 ),
												new Point( rcArrow.Left + 5, rcArrow.Top ),
                                                new Point( rcArrow.Left + 5, rcArrow.Top + 8 ),
                                                new Point( rcArrow.Left + 1, rcArrow.Top + 4 )
											  };
				}
				else
				{
					ptsdropdown = new Point[] { 
												new Point( rcArrow.Left + 4, rcArrow.Top + 4 ),
												new Point( rcArrow.Left, rcArrow.Top ),
                                                new Point( rcArrow.Left, rcArrow.Top + 8 ),
                                                new Point( rcArrow.Left + 4, rcArrow.Top + 4 )  
											  };
				}
			}
			GraphicsPath path = new GraphicsPath();
			path.AddLines( ptsdropdown );

			return path;
		}

		private void DrawOffice2007Border( Graphics g, Rectangle rect, ScrollButton scroll, ButtonState state )
		{
			Pen pen = new Pen( Color.White );

			if( ( state & ButtonState.Pushed ) > 0 )
				pen = new Pen( m_office2007ColorTable.DataTimePickerSelectedBorderColor );
			else if( ( tracking != ButtonID.None || this.Parent.Focused ) && ( state & ButtonState.Inactive ) == 0 )
				pen = new Pen( m_office2007ColorTable.DataTimePickerHighLightedBorderColor );
			else if( this.Selected )
				pen = new Pen( m_office2007ColorTable.DataTimePickerBorderColor );

			if( scroll == ScrollButton.Up )
			{
				if( this.Dock == DockStyle.Right )
				{
					g.DrawLine( pen, rect.Left, rect.Top, rect.Left, rect.Bottom - 1 );
					g.DrawLine( pen, rect.Left, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1 );
				}
				else if( this.Dock == DockStyle.Left )
				{
					g.DrawLine( pen, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom - 1 );
					g.DrawLine( pen, rect.Left, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1 );
				}
			}
			else if( scroll == ScrollButton.Down )
			{
				if( this.Dock == DockStyle.Right )
				{
					g.DrawLine( pen, rect.Left, rect.Top + 1, rect.Left, rect.Bottom );
					g.DrawLine( pen, rect.Left, rect.Top + 1, rect.Right - 1, rect.Top + 1 );
				}
				else if( this.Dock == DockStyle.Left )
				{
					g.DrawLine( pen, rect.Right - 1, rect.Top + 1, rect.Right - 1, rect.Bottom );
					g.DrawLine( pen, rect.Left, rect.Top + 1, rect.Right - 1, rect.Top + 1 );
				}
			}
            pen.Dispose();
		}

        private void DrawOffice2010Border(Graphics g, Rectangle rect, ScrollButton scroll, ButtonState state)
        {
            Pen pen = new Pen(Color.White);

            if ((state & ButtonState.Pushed) > 0)
                pen = new Pen(m_office2010ColorTable.DataTimePickerSelectedBorderColor);
            else if ((tracking != ButtonID.None || this.Parent.Focused) && (state & ButtonState.Inactive) == 0)
                pen = new Pen(m_office2010ColorTable.DataTimePickerHighLightedBorderColor);
            else if (this.Selected)
                pen = new Pen(m_office2010ColorTable.DataTimePickerBorderColor);

            if (scroll == ScrollButton.Up)
            {
                if (this.Dock == DockStyle.Right)
                {
                    g.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom - 1);
                    g.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1);
                }
                else if (this.Dock == DockStyle.Left)
                {
                    g.DrawLine(pen, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom - 1);
                    g.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1);
                }
            }
            else if (scroll == ScrollButton.Down)
            {
                if (this.Dock == DockStyle.Right)
                {
                    g.DrawLine(pen, rect.Left, rect.Top + 1, rect.Left, rect.Bottom);
                    g.DrawLine(pen, rect.Left, rect.Top + 1, rect.Right - 1, rect.Top + 1);
                }
                else if (this.Dock == DockStyle.Left)
                {
                    g.DrawLine(pen, rect.Right - 1, rect.Top + 1, rect.Right - 1, rect.Bottom);
                    g.DrawLine(pen, rect.Left, rect.Top + 1, rect.Right - 1, rect.Top + 1);
                }
            }
            pen.Dispose();
        }

		protected void DrawScrollImage( Graphics g, Rectangle rect, Image image )
		{
			g.DrawImage( image, rect );
		}
		protected void DrawTriangle( Graphics g, Point[] bounds, bool fill )
		{
			if( fill )
			{
				byte[] type = new byte[] { (byte)PathPointType.Start, (byte)PathPointType.Line, (byte)PathPointType.Line };
				GraphicsPath path = new GraphicsPath( bounds, type );
                using (Brush brush = new SolidBrush(SystemColors.ControlDarkDark))
                    g.FillPath(brush, path);
                path.Dispose();
				//				g.FillPolygon(new SolidBrush(SystemColors.ControlDarkDark), bounds, System.Drawing.Drawing2D.FillMode.Alternate);
			}
			else
			{
                using (Pen pen = new Pen(SystemColors.ControlDarkDark))
                {
                    g.DrawLine(pen, bounds[0], bounds[1]);
                    g.DrawLine(pen, bounds[1], bounds[2]);
                    g.DrawLine(pen, bounds[2], bounds[0]);
                }
				//				g.DrawPolygon(new Pen(SystemColors.ControlDarkDark), bounds);
			}
		}

		protected virtual void OnPaint3D( PaintEventArgs e )
		{
			int n0;
			System.Drawing.Size size1;
			size1 = this.ClientSize;
			n0 = ( size1.Height / 2 );

			Graphics g = e.Graphics;
            using (Brush brush = new SolidBrush(this.BackColor))
                g.FillRectangle(brush, this.ClientRectangle);

			ScrollButton scroll1, scroll2;

			if( this.ScrollButtonAppearance == ScrollButtonAppearance.Horizontal )
			{
				scroll1 = ScrollButton.Left;
				scroll2 = ScrollButton.Right;
			}
			else
			{
				scroll1 = ScrollButton.Up;
				scroll2 = ScrollButton.Down;
			}

			ButtonState btnState1, btnState2;


			if( this.pushed == (ButtonID)1 )
				btnState1 = ButtonState.Pushed | this.buttonState;
			else
			{
				if( this.minButtonActive )
					btnState1 = this.buttonState;
				else
					btnState1 = this.buttonState | ButtonState.Inactive;
			}

			if( this.pushed == (ButtonID)2 )
				btnState2 = ButtonState.Pushed | this.buttonState;
			else
			{
				if( this.maxButtonActive )
					btnState2 = this.buttonState;
				else
					btnState2 = this.buttonState | ButtonState.Inactive;
			}

			if( this.Enabled == false )
			{
				btnState1 |= ButtonState.Inactive;
				btnState2 |= ButtonState.Inactive;
			}

			if( XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.ThemesEnabled )
			{
				if( this.tracking != ButtonID.None )
				{
					if( tracking == (ButtonID)2 )
						this.UpdateButtonStateForThemedDrawing( ref btnState2 );
					else
						this.UpdateButtonStateForThemedDrawing( ref btnState1 );
				}
				this.themedDrawing.DrawScrollButton( e.Graphics, this.GetButtonRect( ButtonID.Up ), ButtonID.Up,
					this.ScrollButtonAppearance, btnState1 );
				this.themedDrawing.DrawScrollButton( e.Graphics, this.GetButtonRect( ButtonID.Down ), ButtonID.Down,
					this.ScrollButtonAppearance, btnState2 );
			}
            else if (this.Style == VisualStyle.Office2010 || this.Style == VisualStyle.Office2010)
			{
				this.DrawScrollButton( g, this.GetButtonRect( ButtonID.Up ), scroll1, btnState1 );
				this.DrawScrollButton( g, this.GetButtonRect( ButtonID.Down ), scroll2, btnState2 );
			}
			else
			{
				ControlPaint.DrawScrollButton( g, this.GetButtonRect( ButtonID.Up ), scroll1, btnState1 );
				ControlPaint.DrawScrollButton( g, this.GetButtonRect( ButtonID.Down ), scroll2, btnState2 );
			}
		}

		private void UpdateButtonStateForThemedDrawing( ref ButtonState state )
		{
			state &= ~ButtonState.Flat;
			state |= ButtonState.Normal;
		}

		/// <override/>
		protected override /*Control*/ void OnMouseUp( MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Left )
				this.EndButtonPress();
		}

		protected virtual Rectangle GetButtonRect( ButtonID buttonID )
		{
			Rectangle buttonRect = Rectangle.Empty;
			buttonRect = this.ClientRectangle;

			if( this.ScrollButtonAppearance == ScrollButtonAppearance.Horizontal )
			{
				if( buttonID == ButtonID.Up )
					buttonRect.Width = buttonRect.Width / 2;
				else
					buttonRect = new Rectangle( buttonRect.Left + buttonRect.Width / 2,
						buttonRect.Top, buttonRect.Width / 2, buttonRect.Height );
			}
			else
			{
				if( buttonID == ButtonID.Up )
					buttonRect.Height = buttonRect.Height / 2;
				else
					buttonRect = new Rectangle( buttonRect.Left,
						buttonRect.Top + buttonRect.Height / 2, buttonRect.Width, buttonRect.Height / 2 );
			}
			return buttonRect;
		}

		/// <override/>
		protected override /*Control*/ void OnMouseMove( MouseEventArgs e )
		{
			System.Drawing.Rectangle buttonRect;
			if( this.Capture )
			{
				buttonRect = this.GetButtonRect( this.captured );

				if( ( buttonRect.Contains( e.X, e.Y ) ) )
				{
					if( this.pushed == this.captured ) return;
					this.StartTimer();
					this.pushed = this.captured;
				}
				else
				{
					if( this.pushed == 0 ) return;
					this.StopTimer();
					this.pushed = 0;
				}
				this.Invalidate();
			}
			else
			{
				ButtonID oldTracking = this.tracking;
				if( this.GetButtonRect( ButtonID.Up ).Contains( e.X, e.Y ) )
					this.tracking = ButtonID.Up;
				else if( this.GetButtonRect( ButtonID.Down ).Contains( e.X, e.Y ) )
					this.tracking = ButtonID.Down;
				else
					this.tracking = ButtonID.None;

				if( oldTracking != this.tracking )
				{
					this.Invalidate();
				}
			}
		}

		/// <override/>
		protected override /*Control*/ void OnMouseLeave( EventArgs e )
		{
			this.tracking = ButtonID.None;
			this.Invalidate();
		}

		/// <override/>
		protected override /*Control*/ void OnMouseDown( MouseEventArgs e )
		{
			this.tracking = ButtonID.None;
			if( e.Button == MouseButtons.Left )
			{
				this.BeginButtonPress( e );
				this.Invalidate();
			}
		}


		private void BeginButtonPress( MouseEventArgs e )
		{
			Rectangle rect1;
			rect1 = GetButtonRect( ButtonID.Up );
			Point mousePosition = new Point( e.X, e.Y );
			bool bCapture = false;

			if( rect1.Contains( mousePosition ) )
			{
				if( this.minButtonActive )
				{
					this.captured = ButtonID.Up;
					this.pushed = ButtonID.Up;
					bCapture = true;
				}
			}
			else
			{
				if( this.maxButtonActive )
				{
					this.captured = ButtonID.Down;
					this.pushed = ButtonID.Down;
					bCapture = true;
				}
			}

			if( bCapture )
			{
				this.Invalidate();
				this.Capture = true;
				this.OnUpDown( new UpDownEventArgs( (int)this.pushed ) );
				this.StartTimer();
			}
		}


		private void EndButtonPress()
		{
			this.pushed = 0;
			this.captured = 0;
			this.StopTimer();
			this.Capture = false;
			this.Invalidate();
		}


		protected void StartTimer()
		{
			this.timerInterval = 0x1f4;
			this.timer.Interval = this.timerInterval;
			this.timer.Start();
		}


		protected void StopTimer()
		{
			this.timer.Stop();
		}

		private void TimerHandler( object source, EventArgs args )
		{
			if( !( this.Capture ) )
			{
				this.EndButtonPress();
				return;
			}

			this.OnUpDown( new UpDownEventArgs( (int)this.pushed ) );
			this.timerInterval = ( this.timerInterval * 7 );
			this.timerInterval = ( this.timerInterval / 10 );

			if( this.timerInterval < 1 )
				this.timerInterval = 1;
			else
				this.timer.Interval = this.timerInterval;
		}
	}

	[Serializable,
	Syncfusion.Documentation.DocumentationExclude()
	]
	public enum ButtonID
	{
		None=0 /*0x0000*/,
		Up=1 /*0x0001*/,
		Down=2 /*0x0002*/,
	}
	[Syncfusion.Documentation.DocumentationExclude()]
	public enum ScrollButtonAppearance { Vertical, Horizontal };
}
