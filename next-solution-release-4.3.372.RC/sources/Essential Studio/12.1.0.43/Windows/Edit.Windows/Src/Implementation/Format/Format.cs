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
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Permissions;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Utils;
using System.Globalization;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Formatting
{
	/// <summary>
	/// This class describes how each snippet will be printed: Font, Color, etc.
	/// </summary>
	[XmlRootAttribute( "format", IsNullable = false )]
	public class Format
		: ISnippetFormat
		, IBackgroundFormat
		, IBorderFormat
		, IXMLDataProvider
		, IDisposable
	{
		#region Constants
		/// <summary>
		/// Resource name.
		/// </summary>
		private const string DEF_RESOURCE = "Syncfusion.Windows.Forms.Edit.Images.UnderlineImage.bmp";
		/// <summary>
		/// Default Name of format.
		/// </summary>
		private const string DEF_NAME = "DefaultName";
		/// <summary>
		/// Number of pixels between two lines in double underlining.
		/// </summary>
		private const int SPACE_BETWEEN_DOUBLE_UNDERLINING = 3;
		#endregion

		#region Static Fields
		/// <summary>
		/// Static storage for all Formats of resource image.
		/// </summary>
		private static Image _imgWaveLine = null;
		#endregion

		#region Fields
		/// <summary>
		/// Format name.
		/// </summary>
		private string m_strName = string.Empty;
		/// <summary>
		/// BackGround hatch color.
		/// </summary>
		private Color m_clrFore = Control.DefaultForeColor;
		/// <summary>
		/// Snippet's font's color.
		/// </summary>
		private Color m_clrFont = Color.Empty;
		/// <summary>
		/// Color, used to strike out the text.
		/// </summary>
		private Color m_clrStrikeOut = Color.Empty;
		/// <summary>
		/// Specifies font familty to use.
		/// </summary>
		private FontFamily m_fontFamily;
		/// <summary>
		/// Specifies whether font is bolded.
		/// </summary>
		private bool m_bBold;
		/// <summary>
		/// Specifies whether font is italic.
		/// </summary>
		private bool m_bItalic;
		/// <summary>
		/// Specifies font size in points.
		/// </summary>
		private float m_fFontSize;
		/// <summary>
		/// Background color of snippet. If you want to draw rectangle over the snippet set Background Color.Empty value and Foreground property
		/// to needed rectangle border color. If both properties Background and Foreground set to not Empty value then for drawing used hatch brush
		/// according to BackStyle property value. If Foreground set to Color.Empty value then will be filled snippet rectangle by Background color.
		/// </summary>
		private Color m_clrBack = Color.Empty;
		/// <summary>
		/// Color of Line.
		/// </summary>
		private Color m_clrLine = Color.Black;
		/// <summary>
		/// Style of background brush. This property used only when Background and Foreground colors set to not Empty values.
		/// </summary>
		private HatchStyle m_HatchStyle;
		/// <summary>
		/// Weight of snippet text underline drawing.
		/// </summary>
		private UnderlineWeight m_UnderlineWeight;
		/// <summary>
		/// Style of snippet text underline drawing.
		/// </summary>
		private UnderlineStyle m_UnderlineStyle;
		/// <summary>
		/// Parent of the format.
		/// </summary>
		private FormatManager m_Parent;
		/// <summary>
		/// Snippet's font.
		/// </summary>
		private Font m_font;
        /// <summary>
        /// RightToLeft rendering.
        /// </summary>
        private bool rightToLeft = false;
        /// <summary>
        /// Text draw offset RightToLeft rendering.
        /// </summary>
        private int textDrawOffset = 0;
        /// <summary>
        /// StringFormat used to draw the string.
        /// </summary>
        private StringFormat stringFormat = GraphicsUtils.DefaultFormat;
		/// <summary>
		/// Brush, used to draw text.
		/// </summary>
		private Brush m_brushFont;
		/// <summary>
		/// Pen, used to draw rectangle arround text.
		/// </summary>
		private Pen m_penBackGround;
		/// <summary>
		/// Brush, used to draw background under text.
		/// </summary>
		private Brush m_brushBackGround;
		/// <summary>
		/// Brush, used to draw underlining.
		/// </summary>
		private Brush m_brushUnderline;
		/// <summary>
		/// Pen, used to draw underlining.
		/// </summary>
		private Pen m_penUnderline;
		/// <summary>
		/// Pen object used for drawing text strike out.
		/// </summary>
		private Pen m_penStrikeOut;
		/// <summary>
		/// List, used to keep dynamic formatting.
		/// </summary>
		private IList m_tempFormattingsList = new ArrayList();
		/// <summary>
		/// ID of the format.
		/// </summary>
		private int m_formatID;
		/// <summary>
		/// ID of the last format.
		/// </summary>
		private static int FormatLastID = 1;
		/// <summary>
		/// ImageAttributes, used for remapping of the colors.
		/// </summary>
		private ImageAttributes m_imgAttrWaveLineColorRemap = null;
		/// <summary>
		/// Specifies whether hatch style should be applied to background filling.
		/// </summary>
		private bool m_bUseHatchFill = false;
		/// <summary>
		/// Specifies whether custom control should be rendered instead of the text.
		/// </summary>
		private bool m_bUseCustomControl = false;
		/// <summary>
		/// Frame border color.
		/// </summary>
		private Color m_clrBorder = Color.Empty;
		/// <summary>
		/// Border line style. No border if 'None'.
		/// </summary>
		private FrameBorderStyle m_borderStyle = FrameBorderStyle.None;
		/// <summary>
		/// Border line weight.
		/// </summary>
		private BorderWeight m_borderWeight = BorderWeight.Thin;
		/// <summary>
		/// Static void array of additional formatings used for filling DynamicFormattings member when needed.
		/// </summary>
		private static AdditionalFormatting[] voidArray = new AdditionalFormatting[ 0 ];
		#endregion

		#region Static Properties
		/// <summary>
		/// Get wave image used for underline drawing.
		/// </summary>
		public static Image WaveImage
		{
			get
			{
				if( _imgWaveLine == null )
				{
					Stream imgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream( DEF_RESOURCE );
					_imgWaveLine = Image.FromStream( imgStream, false );
				}

				return _imgWaveLine;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets value indicating whether text is striked out.
		/// </summary>
		[XmlIgnore]
		public bool StrikeOut
		{
			get
			{
				return !m_clrStrikeOut.IsEmpty;
			}
		}
        /// <summary>
        /// Gets or sets the StringFormat for the Format.
        /// </summary>
        [XmlIgnore]
        public StringFormat StringFormat
        {
            get 
            { 
                return stringFormat;
            }
            set 
            { 
                stringFormat = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating RTL rendering.
        /// </summary>
        [XmlIgnore]
        public bool RightToLeft
        {
            get
            {
                return rightToLeft;
            }
            set
            {
                rightToLeft = value;
            }
        }
        /// <summary>
        /// Gets or sets the text draw offset value .
        /// </summary>
        [XmlIgnore]
        public int TextDrawOffset
        {
            get
            {
                return textDrawOffset;
            }
            set
            {
                textDrawOffset = value;
            }
        }
		/// <summary>
		/// Gets or sets currently used font family.
		/// </summary>
		[XmlIgnore]
		public FontFamily FontFamily
		{
			get
			{
				if( m_font != null )
				{
					return m_font.FontFamily;
				}

				return m_fontFamily;
			}
			set
			{
				if( FontFamily != value )
				{
					ResetFont();
					m_fontFamily = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets value that indicates whether font is bolded.
		/// </summary>
		[XmlIgnore]
		public bool FontBold
		{
			get
			{
				if( null != m_font )
				{
					return m_font.Bold;
				}

				return m_bBold;
			}
			set
			{
				if( FontBold != value )
				{
					ResetFont();
					m_bBold = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets value that indicates whether font is italic.
		/// </summary>
		[XmlIgnore]
		public bool FontItalic
		{
			get
			{
				if( null != m_font )
				{
					return m_font.Italic;
				}

				return m_bItalic;
			}
			set
			{
				if( FontItalic != value )
				{
					ResetFont();
					m_bItalic = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets font size.
		/// </summary>
		[XmlIgnore]
		public float FontSize
		{
			get
			{
				if( null != m_font )
				{
					return m_font.Size;
				}

				return m_fFontSize;
			}
			set
			{
				if( FontSize != value )
				{
					ResetFont();
					m_fFontSize = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets vaule indicating whether custom control should be used instead of rendering text.
		/// </summary>
		[XmlAttribute]
		[DefaultValue( false )]
		public bool UseCustomControl
		{
			get
			{
				return m_bUseCustomControl;
			}
			set
			{
				if( value != m_bUseCustomControl )
				{
					m_bUseCustomControl = value;
					RaiseChangedEvent();
				}
			}
		}
		/// <summary>
		/// Name of the format.
		/// </summary>
		[XmlAttribute( "name" )]
		public string Name
		{
			get
			{
				return m_strName;
			}
			set
			{
				if( value != m_strName )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_strName, value );
					m_strName = value;
					OnNameChanged( args );
					RaiseChangedEvent();
				}
			}
		}
		/// <summary>
		/// Manager of the format.
		/// </summary>
		[XmlIgnore()]
		public FormatManager Parent
		{
			get
			{
				return m_Parent;
			}
			set
			{
				m_Parent = value;
			}
		}
		/// <summary>
		/// Current format's font
		/// </summary>
		[XmlIgnore()]
		public Font Font
		{
			get
			{
				if( null == m_font )
				{
					FontStyle style = FontStyle.Regular;

					if( m_bBold )
					{
						style |= FontStyle.Bold;
					}
					if( m_bItalic )
					{
						style |= FontStyle.Italic;
					}

					m_font = new Font( m_fontFamily, m_fFontSize, style );
				}

				return m_font;
			}
			set
			{
				if( value != m_font )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_font, value );

					// release old font
					if( m_font != null )
					{
						m_font.Dispose();
					}

					m_font = value;
					OnFontChanged( args );

					RaiseChangedEvent();
				}
			}
		}
		/// <summary>
		/// Font presentation  for XmlSerialization.
		/// </summary>
		[XmlAttribute( "Font" )]
		public string XmlFont
		{
			get
			{
				TypeConverter cc = TypeDescriptor.GetConverter( this.Font );
				return ( string )cc.ConvertToInvariantString( this.Font );
			}
			set
			{
				TypeConverter cc = TypeDescriptor.GetConverter( this.Font );
				Font font = ( Font )cc.ConvertFromInvariantString( value );
				this.Font = font;
			}
		}
		/// <summary>
		/// BackGround hatch color.
		/// </summary>
		[XmlIgnore()]
		public Color ForeColor
		{
			get
			{
				return m_clrFore;
			}
			set
			{
				if( value != m_clrFore )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_clrFore, value );
					m_clrFore = value;
					OnForeColorChanged( args );

					RaiseChangedEvent();
				}
			}
		}
		/// <summary>
		/// Color presantation for XmlSerialization.
		/// </summary>
		[XmlAttribute( "ForeColor" )]
		public string XmlForeColor
		{
			get
			{
				TypeConverter cc = TypeDescriptor.GetConverter( this.ForeColor );
				string s = ( string )cc.ConvertToInvariantString( this.ForeColor );
				return s;
			}
			set
			{
				TypeConverter cc = TypeDescriptor.GetConverter( this.ForeColor );
				this.ForeColor = ( Color )cc.ConvertFromInvariantString( value );
			}
		}
		/// <summary>
		/// Color of the font.
		/// </summary>
		[XmlIgnore()]
		public Color FontColor
		{
			get
			{
				return m_clrFont;
			}
			set
			{
				if( value != m_clrFont )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_clrFont, value );
					m_clrFont = value;
					OnFontColorChanged( args );

					RaiseChangedEvent();
				}
			}
		}
		/// <summary>
		/// Color presantation for XmlSerialization
		/// </summary>
		[XmlAttribute( "FontColor" )
		 , DefaultValue( "Empty" )]
		public string XmlFontColor
		{
			get
			{
				TypeConverter cc = TypeDescriptor.GetConverter( this.FontColor );
				string s = ( string )cc.ConvertToInvariantString( this.FontColor );
				return s;
			}
			set
			{
				TypeConverter cc = TypeDescriptor.GetConverter( this.FontColor );
				this.FontColor = ( Color )cc.ConvertFromInvariantString( value );
			}
		}
		/// <summary>
		/// Background color of snippet. If you want to draw rectangle over the snippet set Background Color.Empty value and Foreground property to
		/// needed rectangle border color. If both properties Background and Foreground set to not Empty value then for drawing used hatch brush
		/// according to BackStyle property value. If Foreground set to Color.Empty value then will be filled snippet rectangle by Background color.
		/// </summary>
		[XmlIgnore()]
		public Color BackColor
		{
			get
			{
				return m_clrBack;
			}
			set
			{
				if( value != m_clrBack )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_clrBack, value );
					m_clrBack = value;
					OnBackColorChanged( args );

					RaiseChangedEvent();
				}
			}
		}
		/// <summary>
		/// BackColor presantation for XmlSerialization
		/// </summary>
		[XmlAttribute( "BackColor" )]
		[DefaultValue( "" )]
		public string XmlBackColor
		{
			get
			{
				TypeConverter cc = TypeDescriptor.GetConverter( this.BackColor );
				string s = ( string )cc.ConvertToInvariantString( this.BackColor );
				return s;
			}
			set
			{
				TypeConverter cc = TypeDescriptor.GetConverter( this.BackColor );
				this.BackColor = ( Color )cc.ConvertFromInvariantString( value );
			}
		}
		/// <summary>
		/// Style of background brush. This property used only when Background and Foreground colors set to not Empty values.
		/// </summary>
		[XmlAttribute( "style" )]
		[DefaultValue( HatchStyle.Horizontal )]
		public HatchStyle HatchStyle
		{
			get
			{
				return m_HatchStyle;
			}
			set
			{
				if( value != m_HatchStyle )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_HatchStyle, value );
					m_HatchStyle = value;
					OnHatchStyleChanged( args );

					RaiseChangedEvent();
				}
			}
		}
		/// <summary>
		/// Weight of snippet text underline drawing
		/// </summary>
		[XmlAttribute( "weight" )
		 , DefaultValue( typeof( UnderlineWeight ), "Thin" )]
		public UnderlineWeight UnderlineWeight
		{
			get
			{
				return m_UnderlineWeight;
			}
			set
			{
				if( value != m_UnderlineWeight )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_UnderlineWeight, value );
					m_UnderlineWeight = value;
					OnUnderlineWeightChanged( args );

					RaiseChangedEvent();
				}
			}
		}
		/// <summary>
		/// Style of snippet text underline drawing
		/// </summary>
		[XmlAttribute( "underline" )
		 , DefaultValue( typeof( UnderlineStyle ), "None" )]
		public UnderlineStyle UnderlineStyle
		{
			get
			{
				return m_UnderlineStyle;
			}
			set
			{
				if( value != m_UnderlineStyle )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_UnderlineStyle, value );
					m_UnderlineStyle = value;
					OnUnderlineStyleChanged( args );

					RaiseChangedEvent();
				}
			}
		}
		/// <summary>
		/// Gets or sets vaule that specifies whether hatch style settings should be applied on background filling or background should be solid.
		/// </summary>
		[XmlAttribute( "UseHatchFill" )]
		[DefaultValue( false )]
		public bool UseHatchFill
		{
			get
			{
				return m_bUseHatchFill;
			}
			set
			{
				m_bUseHatchFill = value;

				RaiseChangedEvent();
			}
		}
		/// <summary>
		/// Color of Line.
		/// </summary>
		[XmlIgnore]
		public Color LineColor
		{
			get
			{
				return m_clrLine;
			}
			set
			{
				if( value != m_clrLine )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_clrLine, value );
					m_clrLine = value;
					OnLineColorChanged( args );
					RaiseChangedEvent();
				}
			}
		}
		/// <summary>
		/// LineColor presantation for XmlSerialization
		/// </summary>
		[XmlAttribute( "LineColor" )]
		[DefaultValue( "Black" )]
		public string XmlLineColor
		{
			get
			{
				TypeConverter cc = TypeDescriptor.GetConverter( this.LineColor );
				string s = ( string )cc.ConvertToInvariantString( this.LineColor );
				return s;
			}
			set
			{
				TypeConverter cc = TypeDescriptor.GetConverter( this.LineColor );
				this.LineColor = ( Color )cc.ConvertFromInvariantString( value );
			}
		}
		/// <summary>
		/// Get line pen width
		/// </summary>
		[XmlIgnore]
		protected float LineWidth
		{
			get
			{
				return ( m_UnderlineWeight == UnderlineWeight.Thin || m_UnderlineWeight == UnderlineWeight.Double ) ? ( 1 ) : ( 1.3f );
			}
		}
		/// <summary>
		/// True - line has Double style
		/// </summary>
		protected bool IsDoubleLine
		{
			get
			{
				return ( m_UnderlineWeight == UnderlineWeight.Double || m_UnderlineWeight == UnderlineWeight.DoubleBold );
			}
		}
		/// <summary>
		/// Brush, used to draw text.
		/// </summary>
		[XmlIgnore]
		protected Brush TextBrush
		{
			get
			{
				if( m_brushFont == null )
				{
					m_brushFont = m_Parent.GetBrush( m_clrFont );
				}

				return m_brushFont;
			}
		}
		/// <summary>
		/// Pen used to draw rectangle arround text.
		/// </summary>
		[XmlIgnore]
		public Pen BackGroundPen
		{
			get
			{
				if( m_penBackGround == null )
				{
					m_penBackGround = m_Parent.GetPen( m_clrFore );
				}

				return m_penBackGround;
			}
		}
		/// <summary>
		/// Brush used to draw background under text.
		/// </summary>
		[XmlIgnore]
		internal Brush BackGroundBrush
		{
			get
			{
				if( m_brushBackGround == null )
				{
					if( !m_clrFore.IsEmpty && m_bUseHatchFill )
					{
						m_brushBackGround = m_Parent.GetBrush( m_HatchStyle, m_clrFore, m_clrBack );
					}
					else
					{
						m_brushBackGround = m_Parent.GetBrush( m_clrBack );
					}
				}

				return m_brushBackGround;
			}
		}
		/// <summary>
		/// Brush used for underlining.
		/// </summary>
		[XmlIgnore]
		protected Brush UnderlineBrush
		{
			get
			{
				if( ( m_imgAttrWaveLineColorRemap == null && m_UnderlineStyle == UnderlineStyle.Wave ) || m_brushUnderline == null )
				{
					SetUnderlineBrush();
				}

				return m_brushUnderline;
			}
		}
		/// <summary>
		/// Pen used for underlining.
		/// </summary>
		[XmlIgnore]
		protected Pen UnderlinePen
		{
			get
			{
				if( m_penUnderline == null )
				{
					SetUnderlinePen();
				}

				return m_penUnderline;
			}
		}
		/// <summary>
		/// Gets pen used for drawing text strike out.
		/// </summary>
		[XmlIgnore]
		protected Pen StrikeOutPen
		{
			get
			{
				if( null == m_penStrikeOut )
				{
					if( StrikeOut )
					{
						m_penStrikeOut = new Pen( m_clrStrikeOut );
					}
					else
					{
						m_penStrikeOut = null;
					}
				}

				return m_penStrikeOut;
			}
		}
		/// <summary>
		/// Gets or sets text strike out color.
		/// </summary>
		[XmlIgnore]
		public Color StrikeOutColor
		{
			get
			{
				return m_clrStrikeOut;
			}
			set
			{
				if( value != m_clrStrikeOut )
				{
					m_clrStrikeOut = value;

					ResetStrikeOutColor();
					RaiseChangedEvent();
				}
			}
		}
		/// <summary>
		/// Strike out color presantation for XmlSerialization.
		/// </summary>
		[XmlAttribute( "StrikeOutColor" )
		 , DefaultValue( "" )]
		public string XmlStrikeOutColor
		{
			get
			{
				TypeConverter cc = TypeDescriptor.GetConverter( this.StrikeOutColor );
				string s = ( string )cc.ConvertToInvariantString( this.StrikeOutColor );
				return s;
			}
			set
			{
				TypeConverter cc = TypeDescriptor.GetConverter( this.StrikeOutColor );
				this.StrikeOutColor = ( Color )cc.ConvertFromInvariantString( value );
			}
		}
		/// <summary>
		/// Frame border color.
		/// </summary>
		[XmlIgnore()]
		public Color BorderColor
		{
			[DebuggerStepThrough()]
			get
			{
				return m_clrBorder;
			}
			set
			{
				if( value != m_clrBorder )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_clrBorder, value );
					m_clrBorder = value;
					OnForeColorChanged( args );

					RaiseChangedEvent();
				}
			}
		}
		/// <summary>
		/// Color presantation for XmlSerialization.
		/// </summary>
		[XmlAttribute( "BorderColor" )]
		[DefaultValue( "" )]
		public string XmlBorderColor
		{
			get
			{
				TypeConverter cc = TypeDescriptor.GetConverter( this.BorderColor );
				string s = ( string )cc.ConvertToInvariantString( this.BorderColor );
				return s;
			}
			set
			{
				TypeConverter cc = TypeDescriptor.GetConverter( this.BorderColor );
				this.BorderColor = ( Color )cc.ConvertFromInvariantString( value );
			}
		}
		/// <summary>
		/// Weight of border line.
		/// </summary>
		[XmlAttribute( "BorderWeight" )
		 , DefaultValue( typeof( BorderWeight ), "Thin" )]
		public BorderWeight BorderWeight
		{
			[DebuggerStepThrough()]
			get
			{
				return m_borderWeight;
			}
			set
			{
				if( value != m_borderWeight )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_borderWeight, value );
					m_borderWeight = value;
					OnUnderlineStyleChanged( args );

					RaiseChangedEvent();
				}
			}
		}
		/// <summary>
		/// Style of border line.
		/// </summary>
		[XmlAttribute( "BorderStyle" )
		 , DefaultValue( typeof( FrameBorderStyle ), "None" )]
		public FrameBorderStyle BorderStyle
		{
			[DebuggerStepThrough()]
			get
			{
				return m_borderStyle;
			}
			set
			{
				if( value != m_borderStyle )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_borderStyle, value );
					m_borderStyle = value;
					OnUnderlineStyleChanged( args );

					RaiseChangedEvent();
				}
			}
		}
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Default Constructor.
		/// </summary>
		public Format()
			: this( DEF_NAME )
		{
		}
		/// <summary>
		/// Create obj. using format name all settings will be default.
		/// </summary>
		/// <param name="name">format name</param>
		public Format( string name )
			: this( name, Color.Black, Control.DefaultForeColor, Color.Empty, ( Font )Control.DefaultFont.Clone() )
		{
		}
		/// <summary>
		///  Creates obj. using name and Font
		/// </summary>
		/// <param name="name">name of this format</param>
		/// <param name="font">snippet's font at this format </param>
		public Format( string name, Font font )
			: this( name, Color.Black, Control.DefaultForeColor, Color.Transparent, font )
		{
		}
		/// <summary>
		/// create obj. using font and Colors
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="fontColor">snippet's font's color</param>
		/// <param name="foreColor">snippet's forecolor</param>
		/// <param name="backColor">snippet's background</param>
		/// <param name="font"> snippet's font's size</param>
		public Format( string name, Color fontColor, Color foreColor, Color backColor, Font font )
		{
			if( name == null ) throw new ArgumentNullException( "name" );
			if( name.Length == 0 ) throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_14 );
			if( font == null ) throw new ArgumentNullException( "font" );

			m_strName = name;
			m_clrFore = foreColor;
			m_clrBack = backColor;
			m_font = font;
			m_formatID = ++Format.FormatLastID;
		}
		/// <summary>
		/// create obj. using font[ + size ] and Colors
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="fontColor">snippet's font's color </param>
		/// <param name="foreColor"></param>
		/// <param name="backColor">snippet's background</param>
		/// <param name="fontName"></param>
		/// <param name="fontSize">snippet's font's size</param>
		public Format( string name, Color fontColor, Color foreColor, Color backColor, string fontName, float fontSize )
			: this( name, fontColor, foreColor, backColor, new Font( fontName, fontSize ) )
		{
		}
		/// <summary>
		/// create obj. using name and another existing format
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="inherit">Fornat to inherit.</param>
		public Format( string name, ISnippetFormat inherit )
			: this( name )
		{
			if( inherit == null ) throw new ArgumentNullException( "inherit" );

			this.ForeColor = inherit.ForeColor;
			this.BackColor = inherit.BackColor;
			this.Font = ( Font )inherit.Font.Clone();
			this.HatchStyle = inherit.HatchStyle;
			this.UnderlineStyle = inherit.UnderlineStyle;
			this.UnderlineWeight = inherit.UnderlineWeight;
		}
		/// <summary>
		/// Finalizer.
		/// </summary>
		~Format()
		{
			Dispose();
		}
		/// <summary>
		/// Disposes all used resources.
		/// </summary>
		public void Dispose()
		{
			if( m_font != null )
			{
				m_font.Dispose();
				m_font = null;
			}

			if( m_penUnderline != null )
			{
				m_penUnderline.Dispose();
				m_penUnderline = null;
			}

			if( m_brushUnderline != null )
			{
				m_brushUnderline.Dispose();
				m_brushUnderline = null;
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// someone changed Name
		/// </summary>
		[Category( "Property Changed" )]
		public event ValueChangedEventHandler NameChanged;
		/// <summary>
		/// someone changed FontName
		/// </summary>
		[Category( "Property Changed" )]
		public event ValueChangedEventHandler FontNameChanged;
		/// <summary>
		/// someone changed FontSize
		/// </summary>
		[Category( "Property Changed" )]
		public event ValueChangedEventHandler FontSizeChanged;
		/// <summary>
		/// someone changed ForeColor
		/// </summary>
		[Category( "Property Changed" )]
		public event ValueChangedEventHandler ForeColorChanged;
		/// <summary>
		/// someone changed ForeColor
		/// </summary>
		[Category( "Property Changed" )]
		public event ValueChangedEventHandler FontColorChanged;
		/// <summary>
		/// someone changed BackColor
		/// </summary>
		[Category( "Property Changed" )]
		public event ValueChangedEventHandler BackColorChanged;
		/// <summary>
		/// someone changed HatchStyle
		/// </summary>
		[Category( "Property Changed" )]
		public event ValueChangedEventHandler HatchStyleChanged;
		/// <summary>
		/// someone changed UnderlineWeight
		/// </summary>
		[Category( "Property Changed" )]
		public event ValueChangedEventHandler UnderlineWeightChanged;
		/// <summary>
		/// someone changed UnderlineStyle
		/// </summary>
		[Category( "Property Changed" )]
		public event ValueChangedEventHandler UnderlineStyleChanged;
		/// <summary>
		/// This event is raised by renderer when paint works started.
		/// user
		/// </summary>
		public event CustomSnippetDrawEventHandler OnCustomDraw;
		/// <summary>
		/// someone changed Font
		/// </summary>
		[Category( "Property Changed" )]
		public event ValueChangedEventHandler FontChanged;
		/// <summary>
		/// Event that is raised when line color is changed.
		/// </summary>
		[Category( "Property Changed" )]
		public event ValueChangedEventHandler LineColorChanged;
		/// <summary>
		/// Event that is raised when some property value is changed.
		/// </summary>
		[Category( "Behavior" )]
		public event EventHandler Changed;
		#endregion

		#region Event Raisers
		/// <summary>
		/// Raises Changed event.
		/// </summary>
		protected void RaiseChangedEvent()
		{
			if( Changed != null )
			{
				Changed( this, EventArgs.Empty );
			}
		}
		/// <summary>
		/// Raise property_changed event for Name
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseNameChanged( ValueChangedEventArgs args )
		{
			if( NameChanged != null )
			{
				NameChanged( this, args );
			}
		}
		/// <summary>
		/// raise  property_changed event for FontName
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseFontNameChanged( ValueChangedEventArgs args )
		{
			if( FontNameChanged != null )
			{
				FontNameChanged( this, args );
			}
		}
		/// <summary>
		/// raise  property_changed event for FontSize
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseFontSizeChanged( ValueChangedEventArgs args )
		{
			if( FontSizeChanged != null )
			{
				FontSizeChanged( this, args );
			}
		}
		/// <summary>
		/// raise  property_changed event for ForeColor
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseForeColorChanged( ValueChangedEventArgs args )
		{
			if( ForeColorChanged != null )
			{
				ForeColorChanged( this, args );
			}
		}
		/// <summary>
		/// raise  property_changed event for FontColor
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseFontColorChanged( ValueChangedEventArgs args )
		{
			if( FontColorChanged != null )
			{
				FontColorChanged( this, args );
			}
		}
		/// <summary>
		/// raise  property_changed event for BackColor
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseBackColorChanged( ValueChangedEventArgs args )
		{
			if( BackColorChanged != null )
			{
				BackColorChanged( this, args );
			}
		}
		/// <summary>
		/// raise  property_changed event for HatchStyle
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseHatchStyleChanged( ValueChangedEventArgs args )
		{
			if( HatchStyleChanged != null )
			{
				HatchStyleChanged( this, args );
			}
		}
		/// <summary>
		/// raise  property_changed event for
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseUnderlineWeightChanged( ValueChangedEventArgs args )
		{
			if( UnderlineWeightChanged != null )
			{
				UnderlineWeightChanged( this, args );
			}
		}
		/// <summary>
		/// raise  property_changed event for UnderlineStyle
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseUnderlineStyleChanged( ValueChangedEventArgs args )
		{
			if( UnderlineStyleChanged != null )
			{
				UnderlineStyleChanged( this, args );
			}
		}
		/// <summary>
		/// Method raise Custom draw event
		/// </summary>
		/// <param name="args">Parameters of custom draw event</param>
		/// <param>ValueChangedEventArgs</param>
		protected void RaiseOnCustomDraw( CustomSnippetDrawEventArgs args )
		{
			if( OnCustomDraw != null )
			{
				OnCustomDraw( this, args );
			}
		}
		/// <summary>
		/// raise  property_changed event for Font
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseFontChanged( ValueChangedEventArgs args )
		{
			if( FontChanged != null )
			{
				FontChanged( this, args );
			}
		}
		/// <summary>
		///	Raises LineColorChanged event.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseLineColorChanged( ValueChangedEventArgs args )
		{
			if( LineColorChanged != null )
			{
				LineColorChanged( this, args );
			}
		}
		#endregion

		#region Class overrides
		/// <summary>
		/// Raise to OnCustomDraw event to user with specified parameters
		/// </summary>
		/// <param name="args">CustomSnippetDrawEventArgs</param>
		public virtual void OnCustomDrawNeeds( CustomSnippetDrawEventArgs args )
		{
			RaiseOnCustomDraw( args );
		}
		/// <summary>
		/// call raiser for Name
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnNameChanged( ValueChangedEventArgs args )
		{
			RaiseNameChanged( args );
		}
		/// <summary>
		/// call raiser for FontName
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnFontNameChanged( ValueChangedEventArgs args )
		{
			RaiseFontNameChanged( args );
		}

		/// <summary>
		/// call raiser for FontSize
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnFontSizeChanged( ValueChangedEventArgs args )
		{
			RaiseFontSizeChanged( args );
		}

		/// <summary>
		/// call raiser for ForeColor
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnForeColorChanged( ValueChangedEventArgs args )
		{
			ResetForeColor();
			RaiseForeColorChanged( args );
		}
		/// <summary>
		/// call raiser for FontColor
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnFontColorChanged( ValueChangedEventArgs args )
		{
			ResetFontColor();
			RaiseFontColorChanged( args );
		}

		/// <summary>
		/// call raiser for BackColor
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnBackColorChanged( ValueChangedEventArgs args )
		{
			ResetBackColor();
			RaiseBackColorChanged( args );
		}

		/// <summary>
		/// call raiser for HatchStyle
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnHatchStyleChanged( ValueChangedEventArgs args )
		{
			ResetBackColor();
			RaiseHatchStyleChanged( args );
		}

		/// <summary>
		/// call raiser for UnderlineWeight
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnUnderlineWeightChanged( ValueChangedEventArgs args )
		{
			ResetUnderline();
			RaiseUnderlineWeightChanged( args );
		}

		/// <summary>
		/// call raiser for UnderlineStyle
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnUnderlineStyleChanged( ValueChangedEventArgs args )
		{
			ResetUnderline();
			RaiseUnderlineStyleChanged( args );
		}

		/// <summary>
		/// call raiser for Font
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnFontChanged( ValueChangedEventArgs args )
		{
			RaiseFontChanged( args );
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnLineColorChanged( ValueChangedEventArgs args )
		{
			ResetLineColor();
			RaiseLineColorChanged( args );
		}
		/// <summary>
		/// use this to see object like string
		/// </summary>
		/// <returns>object like string</returns>
		public override string ToString()
		{
			string strResult = string.Empty;
			strResult += "\n-----Format-----\n";
			strResult += "m_strName         : " + m_strName;
			strResult += "\nm_font            : " + m_font.ToString();
			strResult += "\nm_BackColor       : " + m_clrBack.ToString();
			strResult += "\nm_ForeColor       : " + m_clrFore.ToString();
			strResult += "\nm_HatchStyle      : " + m_HatchStyle.ToString();
			strResult += "\nm_UnderlineStyle  : " + m_UnderlineStyle.ToString();
			strResult += "\nm_UnderlineWeight : " + m_UnderlineWeight.ToString();
			return strResult;
		}

		#endregion

		#region Class Public Methods
		/// <summary>
		/// Sets underline brush.
		/// </summary>
		protected void SetUnderlineBrush()
		{
			if( m_brushUnderline != null )
				m_brushUnderline.Dispose();

			if( m_UnderlineStyle == UnderlineStyle.Wave )
			{
				Image imgTexture = Format.WaveImage;
				Rectangle imgRect = new Rectangle( Point.Empty, imgTexture.Size );

				ImageAttributes attr = new ImageAttributes();

				attr.SetWrapMode( WrapMode.Tile );
				ColorMap map1 = new ColorMap();
				ColorMap map2 = new ColorMap();
				map1.OldColor = Color.White;
				map1.NewColor = Color.Transparent;
				map2.OldColor = Color.Black;
				map2.NewColor = this.LineColor;

				ColorMap[] remap = new ColorMap[] { map1, map2 };
				attr.SetRemapTable( remap );

				m_imgAttrWaveLineColorRemap = attr;

				m_brushUnderline = null;
			}
			else
			{
				m_brushUnderline = new SolidBrush( LineColor );
			}
		}
		/// <summary>
		/// Sets underline pen.
		/// </summary>
		protected void SetUnderlinePen()
		{
			// dispose old pen
			if( m_penUnderline != null )
				m_penUnderline.Dispose();

			if( m_UnderlineStyle == UnderlineStyle.None ) return;

			if( m_UnderlineStyle != UnderlineStyle.Wave || this.UnderlineBrush == null )
			{
				m_penUnderline = new Pen( this.LineColor, this.LineWidth * 1.5f );

				switch( m_UnderlineStyle )
				{
					case UnderlineStyle.DashDot:
						m_penUnderline.DashStyle = DashStyle.DashDot;
						break;

					case UnderlineStyle.Dash:
						m_penUnderline.DashStyle = DashStyle.Dash;
						break;

					case UnderlineStyle.Dot:
						m_penUnderline.DashStyle = DashStyle.Dot;
						break;
				}
			}
			else
			{
				m_penUnderline = new Pen( this.UnderlineBrush, 1 );
			}
		}
		/// <summary>
		/// Draws underlines.
		/// </summary>
		/// <param name="g">Graphics object, underlines should be drawn on.</param>
		/// <param name="textRect">Rectangle with text, that should be underlined.</param>
		public void DrawUnderlines( Graphics g, RectangleF textRect )
		{
			DrawUnderlines( g, textRect.X, textRect.Bottom - 3, textRect.Width );
		}
		/// <summary>
		/// Draws underlines.
		/// </summary>
		/// <param name="g">Graphics object, underlines should be drawn on.</param>
		/// <param name="x">X coordinate of the underline start.</param>
		/// <param name="y">Y coordinate of the underline start.</param>
		/// <param name="width">Width of the underlining.</param>
		public void DrawUnderlines( Graphics g, float x, float y, float width )
		{
			if( g == null )
				throw new ArgumentNullException( "g" );

			if( m_UnderlineStyle == UnderlineStyle.None ) return;

			bool bSecondLineNeeded = ( m_UnderlineWeight == UnderlineWeight.Double ||
				m_UnderlineWeight == UnderlineWeight.DoubleBold );

			Point oldOrigin = g.RenderingOrigin;
			g.RenderingOrigin = new Point( ( int )x, ( int )y );

			DrawSingleUnderline( g, x, y, width );

			if( bSecondLineNeeded )
				DrawSingleUnderline( g, x, y + SPACE_BETWEEN_DOUBLE_UNDERLINING, width );

			g.RenderingOrigin = oldOrigin;
		}
		/// <summary>
		/// Draws single underlining line.
		/// </summary>
		/// <param name="g">Graphics object.</param>
		/// <param name="x">X coordinate of the underline start.</param>
		/// <param name="y">Y coordinate of the underline start.</param>
		/// <param name="width">Width of the underlining.</param>
		protected void DrawSingleUnderline( Graphics g, float x, float y, float width )
		{
			Pen underLinePen = UnderlinePen;

            if (this.RightToLeft)
                x = this.TextDrawOffset - x - width;

			if( m_UnderlineStyle != UnderlineStyle.Wave )
			{
				g.DrawLine( underLinePen, ( int )x, ( int )y, x + width, y );
			}
			else
			{
				Image imgTexture = Format.WaveImage;

				PointF[] points = new PointF[] 
          { 
            new PointF( x - 2, y ), 
            new PointF( x - 2 + width, y ), new PointF( x - 2, y + imgTexture.Height ) 
          };

				RectangleF rectF = new RectangleF( 0, 0, width, imgTexture.Height );

				g.DrawImage( imgTexture, points, rectF, GraphicsUnit.Pixel, m_imgAttrWaveLineColorRemap );
			}
		}
		/// <summary>
		/// Measures text using current font.
		/// </summary>
		/// <param name="g">Graphics.</param>
		/// <param name="text">Text.</param>
		/// <param name="bMeasureWholeWord">Indicates whether whole word should be measured.</param>
        /// /// <param name="bNativeGdi">bNativeGdi.</param>
        /// /// <param name="spaceBetweenLines">spaceBetweenLines.</param>
		/// <returns>TextInfo.</returns>
		[SecurityPermission( SecurityAction.Assert, Unrestricted = true, Flags = SecurityPermissionFlag.UnmanagedCode )]
		public Syncfusion.Windows.Forms.Edit.Utils.TextInfo MeasureText( Graphics g, string text, bool bMeasureWholeWord, bool bNativeGdi, int spaceBetweenLines )
		{
			if( g == null ) throw new ArgumentNullException( "g" );

			if( OnCustomDraw != null )
			{
				CustomSnippetDrawEventArgs e = new CustomSnippetDrawEventArgs( g, GraphicsUtils.LargeRectangle, this, text, true );

				try
				{
					OnCustomDraw( this, e );
				}
				catch( Exception ex )
				{
					Debug.WriteLine( ex.Message );
				}

				text = e.Text;

				if( e.SkipDefaultDrawing ) return e.MeasuringResult;
			}

			Syncfusion.Windows.Forms.Edit.Utils.TextInfo result =
				GraphicsUtils.MeasureString( text, m_font, bMeasureWholeWord, g, this.Parent.GetTabString(), bNativeGdi );
			result.Height += spaceBetweenLines;
			return result;
		}
		/// <summary>
		/// Return list with all given formatting.
		/// </summary>
		/// <param name="formatting">Array of additional formatting to fill.</param>
		/// <param name="textLength">Length of text.</param>
		/// <returns>List of formatting.</returns>
		public IList CompleteFormattingsList( ref AdditionalFormatting[] formatting, int textLength )
		{
			IList list = m_tempFormattingsList;
			list.Clear();
			int iLastIndex = 0;

			foreach( AdditionalFormatting formatCheck in formatting )
			{
				if( formatCheck.StartLetterIndex != iLastIndex )
				{
					AdditionalFormatting add = new AdditionalFormatting();
					add.StartLetterIndex = iLastIndex;
					add.EndLetterIndex = formatCheck.StartLetterIndex - 1;
					add.Format = this;
					list.Add( add );
				}

				list.Add( formatCheck );
				iLastIndex = formatCheck.EndLetterIndex + 1;
			}

			if( iLastIndex < textLength )
			{
				AdditionalFormatting add = new AdditionalFormatting();
				add.StartLetterIndex = iLastIndex;
				add.EndLetterIndex = textLength - 1;
				add.Format = this;
				list.Add( add );
			}

			return list;
		}
		/// <summary>
		/// Draws text and strikes it out if needed.
		/// </summary>
		/// <param name="g">Graphics object.</param>
		/// <param name="text">Text to draw.</param>
		/// <param name="rect">Rectangle, the text is to be drawn to.</param>
		/// <param name="autoScrollY">Y autoscroll position. Used for proper native drawing.</param>
		/// <param name="scale">Scale value for output. Used in printing for resolving printing problems related to native methods.</param>
		/// <param name="margins">Margin offsets for text output. Used in printing.</param>
        /// <param name="bNativeDrawing">bNativeDrawing.</param>
		protected void DrawTextAndStrikeOut( Graphics g, string text, Rectangle rect, int autoScrollY, float scale, Size margins, bool bNativeDrawing )
		{
			DrawTextAndStrikeOut( g, text, rect, autoScrollY, scale, margins, this.TextBrush, this.StrikeOutPen, bNativeDrawing );
		}
		/// <summary>
		/// Draws text and strikes it out if needed.
		/// </summary>
		/// <param name="g">Graphics object.</param>
		/// <param name="text">Text to draw.</param>
		/// <param name="rect">Rectangle, the text is to be drawn to.</param>
		/// <param name="autoScrollY">Y autoscroll position. Used for proper native drawing.</param>
		/// <param name="scale">Scale value for output. Used in printing for resolving printing problems related to native methods.</param>
		/// <param name="margins">Margin offsets for text output. Used in printing.</param>
		/// <param name="textBrush">Brush for text.</param>
		/// <param name="penStrikeOut">Pen to draw striking out.</param>
		/// <param name="bNativeDrawing">Indicates whether native drawing should be used.</param>
		[SecurityPermission( SecurityAction.Assert, Unrestricted = true, Flags = SecurityPermissionFlag.UnmanagedCode )]
		protected void DrawTextAndStrikeOut(
			Graphics g, string text, Rectangle rect, int autoScrollY, float scale, Size margins, Brush textBrush, Pen penStrikeOut, bool bNativeDrawing )
		{
			string textToDraw = text;

			if( Parent.ShowWhiteSpaces && Parent.ShowWhiteSpaceProperties.ShowTabs )
			{
				textToDraw = text.Replace( "\t", this.Parent.GetTabString() );
			}

			Font font = m_font;

			if( bNativeDrawing )
			{
				if( scale != 1 )
				{
					font = new Font( font.FontFamily, font.Size * scale, font.Style );
					rect.X = ( int )( rect.X * scale );
					rect.Y = ( int )( rect.Y * scale );
					rect.Width = ( int )( rect.Width * scale );
					rect.Height = ( int )( rect.Height * scale );
				}

				rect.Y += ( int )( autoScrollY * scale );
				rect.Y += ( int )( margins.Height * scale );
				rect.X += ( int )( margins.Width * scale );

				IntPtr hClip = g.Clip.GetHrgn( g );
				IntPtr hdc = g.GetHdc();
				IntPtr oldFont = GDIAppi.SelectObject( hdc, GraphicsUtils.GetFontHandle( font ) );
				int oldBKMode = GDIAppi.SetBkMode( hdc, 1/*TRANSPARENT*/ );
				IntPtr oldClip = new IntPtr();
				GDIAppi.GetClipRgn( hdc, oldClip );
				GDIAppi.SelectClipRgn( hdc, hClip );
				GDIAppi.DeleteObject( hClip );

				SolidBrush br = textBrush as SolidBrush;
				if( br != null )
				{
					int c = ( ( int )br.Color.R ) | ( ( int )br.Color.G << 8 ) | ( ( int )br.Color.B << 16 );
					GDIAppi.SetTextColor( hdc, c );
				}
				RECT rc = ( RECT )rect;
				GDIAppi.DrawText( hdc, textToDraw, textToDraw.Length, ref rc,
					DrawTextFormatFlags.DT_TOP | DrawTextFormatFlags.DT_NOPREFIX | DrawTextFormatFlags.DT_EXPANDTABS );
				GDIAppi.SelectObject( hdc, oldFont );
				GDIAppi.SetBkMode( hdc, oldBKMode );
				GDIAppi.SelectClipRgn( hdc, oldClip );
				GDIAppi.DeleteObject( oldClip );
				g.ReleaseHdc( hdc );
			}
			else
			{
				g.DrawString( textToDraw, font, textBrush, rect.X, rect.Y, this.StringFormat);
			}

			if( null != penStrikeOut )
			{
				float lineY = rect.Y + rect.Height / 2;
				float X2 = (this.RightToLeft) ? (rect.Left - rect.Width) : rect.Right;
				g.DrawLine( penStrikeOut, rect.X, lineY, X2, lineY );
			}
		}
		/// <summary>
		/// Draws text on specified Graphics object using current format settings.
		/// </summary>
		/// <param name="g">Graphics, the text is to be drawn on.</param>
		/// <param name="text">Text, to be drawn.</param>
		/// <param name="borderInfo">Info about bordering.</param>
		/// <param name="bNativeDrawing">Indicates whether native drawing should be used.</param>
        /// <param name="spaceBetweenLines">Space between lines.</param>
		public void DrawText( Graphics g, ref TextDrawInfo text, ref BorderInfo borderInfo, bool bNativeDrawing, int spaceBetweenLines )
		{
			DrawText( g, ref text, ref borderInfo, 0, 1, Size.Empty, bNativeDrawing, spaceBetweenLines );
		}
		/// <summary>
		/// Draws text on specified Graphics object using current format settings.
		/// </summary>
		/// <param name="g">Graphics, the text is to be drawn on.</param>
		/// <param name="text">Text, to be drawn.</param>
		/// <param name="borderInfo">Info about bordering.</param>
		/// <param name="autoScrollY">Y autoscroll position. Used for proper native drawing.</param>
		/// <param name="scale">Scale value for output. Used in printing for resolving printing problems related to native methods.</param>
		/// <param name="margins">Margin offsets for text output. Used in printing.</param>
		/// <param name="bNativeDrawing">Indicates whether native drawing should be used.</param>
		/// <param name="SpaceBetweenLines">Space between lines.</param>
		public void DrawText( Graphics g, ref TextDrawInfo text, ref BorderInfo borderInfo, int autoScrollY,
			float scale, Size margins, bool bNativeDrawing, int SpaceBetweenLines )
		{
			if( g == null ) throw new ArgumentNullException( "g" );

			// If user draws everything himself.
			if( OnCustomDraw != null )
			{
				CustomSnippetDrawEventArgs e = new CustomSnippetDrawEventArgs( g, text.DrawRectangle, this, text.Text, false );

				try
				{
					OnCustomDraw( this, e );
				}
				catch( Exception ex )
				{
					Debug.WriteLine( ex.Message );
				}

				text.Text = e.Text;

				if( e.SkipDefaultDrawing )
					return;
			}

			string textToDraw = text.Text;

			if( Parent.ShowWhiteSpaces && Parent.ShowWhiteSpaceProperties.ShowSpaces )
			{
				textToDraw = textToDraw.Replace( " ", Parent.ShowWhiteSpaceProperties.SpaceChar.ToString() );
			}

			if( m_clrFont.IsEmpty )
			{
				m_clrFont = SystemColors.WindowText;
			}

			//  Fill background.
			if( !m_clrBack.IsEmpty && text.DynamicFormattings == null )
			{
				g.FillRectangle( BackGroundBrush, text.DrawRectangle );
			}

			DrawUnderlines( g, text.DrawRectangle.X, text.DrawRectangle.Bottom - 2, text.DrawRectangle.Width );
			bool bNeedSpecialTextDraw = false;

			if( GraphicsUtils.DefaultFormat.LineAlignment != text.VerticalAlignment )
			{
				GraphicsUtils.DefaultFormat.LineAlignment = text.VerticalAlignment;
			}
			// If there are dynamic formatting or some borders should be drawn.
			if( text.DynamicFormattings != null || borderInfo.format != null || BorderStyle != FrameBorderStyle.None )
			{
				// Fill text.DynamicFormattings with nothing if needed.
				if( text.DynamicFormattings == null )
				{
					text.DynamicFormattings = voidArray;
				}
				Syncfusion.Windows.Forms.Edit.Utils.TextInfo info = MeasureText( g, textToDraw, false, bNativeDrawing, SpaceBetweenLines );

				if( info.Characters.Length != info.Length || info.Length != textToDraw.Length )
					throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_15 );

				IList formattingsList = CompleteFormattingsList( ref text.DynamicFormattings, info.Length );
				RectangleF rect = RectangleF.Empty;
				rect.Y = text.DrawRectangle.Y;
				rect.Height = text.DrawRectangle.Height;
				Region clipRegion = ( Region )g.Clip.Clone();

				// Cycle to test whether bNeedSpecialTextDraw should be set to True.
				for( int i = 0, iCount = formattingsList.Count; i < iCount; i++ )
				{
					AdditionalFormatting formatTest = ( AdditionalFormatting )formattingsList[ i ];
					bNeedSpecialTextDraw |= !formatTest.Format.FontColor.IsEmpty;
					bNeedSpecialTextDraw |= formatTest.Format.StrikeOut;
					bNeedSpecialTextDraw |= ( null != borderInfo.format || FrameBorderStyle.None != formatTest.Format.BorderStyle );
				}

				// Cycle over every sublexem.
				for( int i = 0, iCount = formattingsList.Count; i < iCount; i++ )
				{
					// Format of sublexem.
					AdditionalFormatting format = ( AdditionalFormatting )formattingsList[ i ];
					int partLength = format.EndLetterIndex - format.StartLetterIndex + 1;
					RectangleF rectRTL = RectangleF.Empty;

					if( partLength > 0 )
					{
						Format frm = format.Format as Format;
						rect.X = text.DrawRectangle.X + info.Characters[ format.StartLetterIndex ].CharLeft;

						rect.Width = text.DrawRectangle.X + info.Characters[ format.EndLetterIndex ].CharLeft
							+ info.Characters[ format.EndLetterIndex ].CharWidth - rect.X;

						rectRTL = ConvertToRTL(rect);

						if( bNeedSpecialTextDraw )
						{
							g.SetClip( clipRegion, CombineMode.Replace );
							g.SetClip( rectRTL, CombineMode.Intersect);
						}

						// Fill background.
						if( !frm.m_clrBack.IsEmpty )
							g.FillRectangle( frm.BackGroundBrush, rectRTL);
						else if( !this.m_clrBack.IsEmpty )
							g.FillRectangle( BackGroundBrush, rectRTL);

						if( bNeedSpecialTextDraw )
						{
							Brush brush = ( frm.FontColor.IsEmpty ) ? TextBrush : frm.TextBrush;
							Pen pen = ( frm.StrikeOut ) ? frm.StrikeOutPen : StrikeOutPen;
							// Draw string using format.
							DrawTextAndStrikeOut( g, textToDraw, text.DrawRectangle, autoScrollY, scale, margins, brush, pen, bNativeDrawing );
							g.SetClip( clipRegion, CombineMode.Replace );
							float oldRectHeight = rect.Height;
							rect.Height = text.TextHeight;
							DrawBorder( g, ref borderInfo, frm, ref rect );
							rect.Height = oldRectHeight;
						}
					}
				}

				if( bNeedSpecialTextDraw )
					g.SetClip( clipRegion, CombineMode.Replace );
			}

			if( !bNeedSpecialTextDraw )
			{
				DrawTextAndStrikeOut( g, textToDraw, text.DrawRectangle, autoScrollY, scale, margins, bNativeDrawing );
			}
		}
		/// <summary>
		/// Draws border if needed.
		/// </summary>
		/// <param name="g">Graphics object to draw border on.</param>
		/// <param name="borderInfo">Structure with info about border.</param>
		/// <param name="frm">Format of border.</param>
		/// <param name="rect">Rectengle to draw border around.</param>
		public void DrawBorder( Graphics g, ref BorderInfo borderInfo, Format frm, ref RectangleF rect )
		{
			// Drawing border if needed.
			bool bPrevHasBorder = ( null != borderInfo.format );
			bool bCurHasBorder = ( FrameBorderStyle.None != frm.BorderStyle );
			bool bPrevAndCurMatch =
				( bPrevHasBorder
				&& borderInfo.format.BorderStyle == frm.BorderStyle
				&& borderInfo.format.BorderColor == frm.BorderColor
				&& borderInfo.format.BorderWeight == frm.BorderWeight )
				||
				( !bPrevHasBorder && FrameBorderStyle.None == frm.BorderStyle );
			bool bDrawPreviousBorder = bPrevHasBorder && !bPrevAndCurMatch;
			bool bStartNewBorder = bDrawPreviousBorder || ( !bPrevHasBorder && bCurHasBorder );
			bool bContinueBorder = !bDrawPreviousBorder && bPrevHasBorder && bPrevAndCurMatch;
			bool bDrawCurrentBorder = ( bStartNewBorder || bContinueBorder ) && borderInfo.bFinish;

			RectangleF rectRTL = ConvertToRTL(borderInfo.Rect);

			if( bDrawPreviousBorder )
			{
				// Draw previous border.
				GraphicsUtils.DrawBorder(
					g, ref rectRTL, borderInfo.format.BorderStyle, borderInfo.format.BorderColor, borderInfo.format.BorderWeight );
			}

			// Border is not continued if it is already draw or it does not exist.
			if( bContinueBorder )
			{
				borderInfo.Rect.Width += rect.Width;
			}

			// If previous border is already drawn or does not exist, than maybe new one should be started.
			if( bStartNewBorder )
			{
				borderInfo.format = frm;
				borderInfo.Rect = rect;
			}

			rectRTL = ConvertToRTL(borderInfo.Rect);

			// If current border exists (continued or created) and it should be drawn, than lets draw it!
			if( bDrawCurrentBorder )
			{
				GraphicsUtils.DrawBorder( g,
					ref rectRTL,
					borderInfo.format.BorderStyle,
					borderInfo.format.BorderColor,
					borderInfo.format.BorderWeight );
			}

			// No border.
			if( !bCurHasBorder && borderInfo.format != null )
			{
				borderInfo.format = null;
			}
		}

		private RectangleF ConvertToRTL(RectangleF rect)
		{
			RectangleF rectRTL = rect;

			if (this.RightToLeft)
				rectRTL.X -= rect.Width + 2;

			return rectRTL;
		}
		/// <summary>
		/// Append it's data to given XML.
		/// </summary>
		/// <param name="parent">Parent element.</param>
		public void AppendToXML( XmlElement parent )
		{
			XmlElement element = parent.OwnerDocument.CreateElement( "format" );
			element.SetAttribute( "name", Name );
			element.SetAttribute( "ID", m_formatID.ToString() );
			element.SetAttribute( "color", XmlFontColor );
			element.SetAttribute( "bold", Font.Bold.ToString() );
			parent.AppendChild( element );
		}
		/// <summary>
		/// Append it's data to given XML.
		/// </summary>
		/// <param name="writer">Xml writer.</param>
		public void AppendToXML( XmlTextWriter writer )
		{
			Color fontColor = FontColor;
			writer.WriteStartElement( "format" );
			writer.WriteAttributeString( "name", Name );
			writer.WriteAttributeString( "ID", m_formatID.ToString() );
			writer.WriteAttributeString( "color", XmlFontColor );
			writer.WriteAttributeString( "colorRed", fontColor.R.ToString() );
			writer.WriteAttributeString( "colorGreen", fontColor.G.ToString() );
			writer.WriteAttributeString( "colorBlue", fontColor.B.ToString() );
			writer.WriteAttributeString( "bold", Font.Bold.ToString() );
			writer.WriteAttributeString( "italic", Font.Italic.ToString() );
			writer.WriteAttributeString( "font", Font.Name );
			writer.WriteAttributeString( "fontSize", Font.SizeInPoints.ToString( NumberFormatInfo.InvariantInfo ) );
			writer.WriteAttributeString( "underline", this.UnderlineStyle.ToString() );
			writer.WriteEndElement();
		}
		#endregion

		#region Class utility methods
		/// <summary>
		/// Sets font to null.
		/// </summary>
		private void ResetFont()
		{
			if( null != m_font )
			{
				m_font.Dispose();
				m_font = null;
			}
		}
		/// <summary>
		/// Disposes brush which depends from ForeColor.
		/// </summary>
		private void ResetForeColor()
		{
			if( m_penBackGround != null )
			{
				m_penBackGround = null;
			}
		}
		/// <summary>
		/// Disposes brush which depends from FontColor.
		/// </summary>
		private void ResetFontColor()
		{
			if( m_brushFont != null )
			{
				m_brushFont = null;
			}
		}
		/// <summary>
		/// Resets strike out pen.
		/// </summary>
		private void ResetStrikeOutColor()
		{
			if( m_penStrikeOut != null )
			{
				m_penStrikeOut.Dispose();
				m_penStrikeOut = null;
			}
		}
		/// <summary>
		/// Disposes brush which depends from BackColor.
		/// </summary>
		private void ResetBackColor()
		{
			if( m_brushBackGround != null )
			{
				m_brushBackGround = null;
			}
		}
		/// <summary>
		/// Disposes Underline Style and Weight.
		/// </summary>
		private void ResetUnderline()
		{
			if( m_penUnderline != null )
			{
				m_penUnderline.Dispose();
				m_penUnderline = null;
			}
		}
		/// <summary>
		/// Disposes Line color cache.
		/// </summary>
		private void ResetLineColor()
		{
			if( m_brushUnderline != null )
			{
				m_brushUnderline.Dispose();
				m_brushUnderline = null;
			}

			if( m_imgAttrWaveLineColorRemap != null )
			{
				m_imgAttrWaveLineColorRemap.Dispose();
				m_imgAttrWaveLineColorRemap = null;
			}
		}
		#endregion

		#region ShoulsSerialize Methods
		/// <summary>
		/// Should serialize Xml Fore Color.
		/// </summary>
		/// <returns></returns>
		internal bool ShouldSerializeXmlForeColor()
		{
			return m_clrFore != Control.DefaultForeColor;
		}
		#endregion
	}
}