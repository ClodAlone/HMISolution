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
using System.Drawing.Drawing2D;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;

using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.XmlSerializersCreator;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Formatting
{
	/// <summary>
	/// Operates by formats.
	/// </summary>
	public class FormatManager
		: EventBaseCollection
		, IFormatManager
		, IObjectInitialize
		, IXmlSerializable
		, IXMLDataProvider
		, IDisposable
	{
		#region Class Constants
		/// <summary>
		/// Default name of the format.
		/// </summary>
		private const string DEF_DEFAULT_FORMAT_NAME = "Default";
		/// <summary>
		/// Default name of the font.
		/// </summary>
		private const string DEF_FONT_NAME = "Courier New";
		/// <summary>
		/// Default Format.
		/// </summary>
		private Format defFormat = new Format( DEF_DEFAULT_FORMAT_NAME,
			Color.Black, Control.DefaultForeColor,
			Color.Empty, new Font( DEF_FONT_NAME, 10 ) );
		/// <summary>
		/// 
		/// </summary>
		internal const int TAB_SIZE = 2;
		#endregion

		#region Class Members
		/// <summary>
		/// Stores name-to-format object.
		/// </summary>
		protected Hashtable m_hashNames = new Hashtable();
		/// <summary>
		/// This is calculated value and it hold maximum heigh of line according to
		/// known to object formats.
		/// </summary>
		protected int m_iMaxLineHeight = 0;
		/// <summary>
		/// This is calculated value an it hold minimal size of line according to
		/// known for object formats
		/// </summary>
		protected int m_iMinLineHeight = 100;
		/// <summary>
		/// This is calculated value and it hold maximum char width according to
		/// known for object formats.
		/// </summary>
		protected int m_iMaxCharWidth = 0;
		/// <summary>
		/// This is calcualted value and it hold minimum char width according to
		/// formats known by object.
		/// </summary>
		protected int m_iMinCharWidth = 100;
		/// <summary>
		/// Hash of the preallocated brushes.
		/// Key - Style, Value - hashtable ( Key - Color, Value - Brush )
		/// </summary>
		private Hashtable m_BrushesHash = new Hashtable();
		/// <summary>
		/// Hash of the preallocated solid brushes.
		/// Key - Color, Value - Brush
		/// </summary>
		private Hashtable m_SolidBrushesHash = new Hashtable();
		/// <summary>
		/// Hashtable of the pens.
		/// Key - color. Value - Pen.
		/// </summary>
		private Hashtable m_PensHash = new Hashtable();
		/// <summary>
		/// Hashtable of standart format names.
		/// </summary>
		private Hashtable m_standartFormatNames = new Hashtable();
		/// <summary>
		/// Hashtable with formats used for striking out text.
		/// </summary>
		private Hashtable m_strikeOutFormats = new Hashtable();
		/// <summary>
		/// Count of space characters, the tab 
		/// character is to be replaced with.
		/// </summary>
		private int m_iSpacesInTab = TAB_SIZE;
		/// <summary>
		/// Text( array of spaces ), the tab 
		/// character is to be replaced with.
		/// </summary>
		private string m_textInsteadTab = "  ";
		/// <summary>
		/// Idicates whether Show white space mode is on.
		/// </summary>
		private bool m_bShowWhiteSpaces;
		/// <summary>
		/// Hashtable used to keep all created border formats.
		/// </summary>
		private Hashtable m_borderFormats = new Hashtable();
		/// <summary>
		/// Hashtable used to keep all created text color formats.
		/// </summary>
		private Hashtable m_textColorFormats = new Hashtable();
		/// <summary>
		/// Hashtable used to keep all created background color formats.
		/// </summary>
		private Hashtable m_backgroundColorFormats = new Hashtable();
		/// <summary>
		/// Hashtable used to keep all created background and text color formats.
		/// </summary>
		private Hashtable m_backgroundAndTextColorFormats = new Hashtable();
		/// <summary>
		/// Index of newly created border format used for its naming.
		/// </summary>
		private int m_borderIndex = 0;
		/// <summary>
		/// Index of newly created text color format used for its naming.
		/// </summary>
		private int m_textColorIndex = 0;
		/// <summary>
		/// Index of newly created background color format used for its naming.
		/// </summary>
		private int m_backgroundColorIndex = 0;
		/// <summary>
		/// Index of newly created background and text color format used for its naming.
		/// </summary>
		private int m_backgroundAndTextColorIndex = 0;
		/// <summary>
		/// Properties of White space mode.
		/// </summary>
		private ShowWhiteSpaceProperties m_whitespaceProps = null;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets format by name.
		/// </summary>
		[XmlIgnore()]
		public ISnippetFormat this[ string name ]
		{
			get
			{
				if( !m_hashNames.Contains( name ) )
				{
					if( name != DEF_DEFAULT_FORMAT_NAME )
					{
						defFormat.Parent = this;
						return defFormat;
					}
					else
						return this[ DEF_DEFAULT_FORMAT_NAME ];
				}

				ISnippetFormat format = ( ISnippetFormat )m_hashNames[ name ];
				return ( format == null ) ? defFormat : format;
			}
		}
		/// <summary>
		/// Gets format by FormatType.
		/// </summary>
		[XmlIgnore()]
		public ISnippetFormat this[ FormatType type ]
		{
			get
			{
				string name = ( string )m_standartFormatNames[ type ];

				return this[ name ];
			}
		}
		/// <summary>
		/// Gets format by index.
		/// </summary>
		[XmlIgnore()]
		public ISnippetFormat this[ int index ]
		{
			get
			{
				return ( ISnippetFormat )InnerList[ index ];
			}
		}
		/// <summary>
		/// This is calculated value and it hold maximum heigh of line according to
		/// known to object formats.
		/// </summary>
		[XmlIgnore]
		public int MaxLineHeight
		{
			get
			{
				return m_iMaxLineHeight;
			}
		}
		/// <summary>
		/// This is calculated value an it hold minimal size of line according to
		/// known for object formats
		/// </summary>
		[XmlIgnore]
		public int MinLineHeight
		{
			get
			{
				return m_iMinLineHeight;
			}
		}
		/// <summary>
		/// This is calculated value and it hold maximum char width according to
		/// known for object formats.
		/// </summary>
		[XmlIgnore]
		public int MaxCharWidth
		{
			get
			{
				return m_iMaxCharWidth;
			}
		}
		/// <summary>
		/// This is calcualted value and it hold minimum char width according to
		/// formats known by object.
		/// </summary>
		[XmlIgnore]
		public int MinCharWidth
		{
			get
			{
				return m_iMinCharWidth;
			}
		}
		/// <summary>
		/// Gets or sets count of space characters, the tab 
		/// character is to be replaced with.
		/// </summary>
		public int SpacesInTab
		{
			get
			{
				return m_iSpacesInTab;
			}
			set
			{
				if( value != m_iSpacesInTab )
				{

					if( value < 1 )
						throw new ArgumentOutOfRangeException( "SpacesInTab", value, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_5 );

					m_iSpacesInTab = value;
					m_textInsteadTab = new string( ' ', m_iSpacesInTab );
				}
			}
		}
		/// <summary>
		/// Gets text( array of spaces ), the tab 
		/// character is to be replaced with.
		/// </summary>
		public string TabReplaceString
		{
			get
			{
				return m_textInsteadTab;
			}
		}
		/// <summary>
		/// Gets or sets sing of showing whitespaces as bullets.
		/// </summary>
		public bool ShowWhiteSpaces
		{
			get
			{
				return m_bShowWhiteSpaces;
			}
			set
			{
				if( value != m_bShowWhiteSpaces )
				{
					m_bShowWhiteSpaces = value;
				}
			}
		}
		/// <summary>
		/// Properties of Show white spaces mode.
		/// </summary>
		public ShowWhiteSpaceProperties ShowWhiteSpaceProperties
		{
			get
			{
				return m_whitespaceProps;
			}
			set
			{
				m_whitespaceProps = value;
			}
		}
		#endregion

		#region Class Events
		/// <summary>
		/// Event that is raised when some format has been changed.
		/// </summary>
		public event EventHandler FormatChanged;
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Adds format to collection.
		/// </summary>
		/// <param name="format">Format to add.</param>
		public void Add( ISnippetFormat format )
		{
			if( format == null )
				throw new ArgumentNullException( "format" );

			this.List.Add( format );
			( format as Format ).Parent = this;

			m_hashNames[ format.Name ] = format;
		}
		/// <summary>
		/// Creates new format and adds it to collection.
		/// </summary>
		/// <param name="formatName">Name for format.</param>
		/// <returns>New format.</returns>
		public ISnippetFormat Add( string formatName )
		{
			if( formatName == null )
				throw new ArgumentNullException( "formatName" );

			if( formatName.Length == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_6 );

			Format format = new Format( formatName );
			this.List.Add( format );
			format.Parent = this;

			m_hashNames[ formatName ] = format;

			return format;
		}
		/// <summary>
		/// Adds new format to collection.
		/// </summary>
		/// <param name="formatName">Name of format.</param>
		/// <param name="source">Source format.</param>
		/// <returns>New format.</returns>
		public ISnippetFormat Add( string formatName, ISnippetFormat source )
		{
			if( formatName == null )
				throw new ArgumentNullException( "formatName" );

			if( formatName.Length == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_6 );

			if( source == null )
				throw new ArgumentNullException( "source" );

			Format format = new Format( formatName, source );
			this.List.Add( format );
			format.Parent = this;

			m_hashNames[ formatName ] = format;

			return format;
		}

		/// <summary>
		/// Adds new format to collection.
		/// </summary>
		/// <param name="formatName">Name of format.</param>
		/// <param name="sourceName">Name of source format.</param>
		/// <returns>New format.</returns>
		public ISnippetFormat Add( string formatName, string sourceName )
		{
			if( formatName == null )
				throw new ArgumentNullException( "formatName" );

			if( formatName.Length == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_6 );

			if( sourceName == null )
				throw new ArgumentNullException( "sourceName" );

			if( sourceName.Length == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_7 );

			ISnippetFormat source = ( ISnippetFormat )m_hashNames[ sourceName ];
			Format format = new Format( formatName, source );

			this.List.Add( format );
			format.Parent = this;

			m_hashNames[ formatName ] = format;

			return format;
		}
		/// <summary>
		/// Removes format from the collection.
		/// </summary>
		/// <param name="format">Format to remove.</param>
		public void Remove( ISnippetFormat format )
		{
			if( format == null )
				throw new ArgumentNullException( "format" );

			this.List.Remove( format );

			m_hashNames.Remove( format.Name );
		}
		/// <summary>
		/// Removes format from the collection.
		/// </summary>
		/// <param name="formatName">Name of format to remove.</param>
		public void Remove( string formatName )
		{
			if( formatName == null )
				throw new ArgumentNullException( "formatName" );

			if( formatName.Length == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_6 );

			this.List.Remove( m_hashNames[ formatName ] );

			m_hashNames.Remove( formatName );
		}
		/// <summary>
		/// Creates or get's from the cache solid brush.
		/// </summary>
		/// <param name="color">Color of the needed brush.</param>
		/// <returns>Solid brush.</returns>
		public Brush GetBrush( Color color )
		{
			if( !m_SolidBrushesHash.Contains( color ) )
			{
				return ( Brush )( m_SolidBrushesHash[ color ] = new SolidBrush( color ) );
			}

			return ( Brush )m_SolidBrushesHash[ color ];
		}
		/// <summary>
		/// Creates or gets from the cache brush with a needed hatch style.
		/// </summary>
		/// <param name="style">HatchStyle of the needed brush.</param>
		/// <param name="foreColor">Color of the needed brush.</param>
		/// <param name="backColor">Background color.</param>
		/// <returns>Brush. You do not have to dispose it later, it will be done automatically.</returns>
		public Brush GetBrush( HatchStyle style, Color foreColor, Color backColor )
		{
			Hashtable subBrushes = null;

			if( !m_BrushesHash.Contains( style ) )
				m_BrushesHash[ style ] = subBrushes = new Hashtable();
			else
				subBrushes = ( Hashtable )m_BrushesHash[ style ];

			if( !subBrushes.Contains( backColor ) )
				subBrushes = ( Hashtable )( subBrushes[ backColor ] = new Hashtable() );
			else
				subBrushes = ( Hashtable )subBrushes[ backColor ];

			if( !subBrushes.Contains( foreColor ) )
			{
				return ( Brush )( subBrushes[ foreColor ] = new HatchBrush( style,
					foreColor, backColor ) );
			}

			return ( Brush )subBrushes[ foreColor ];
		}
		/// <summary>
		/// Creates or gets from the cache pen with a needed color.
		/// </summary>
		/// <param name="penColor">Color of the needed pen.</param>
		/// <returns>Pen. You do not have to dispose this pen later,
		/// it will be done automatically.</returns>
		public Pen GetPen( Color penColor )
		{
			if( !m_PensHash.Contains( penColor ) )
				return ( Pen )( m_PensHash[ penColor ] = new Pen( penColor ) );

			return ( Pen )m_PensHash[ penColor ];
		}
		/// <summary>
		/// Append its data to given XML.
		/// </summary>
		/// <param name="parent">Parent element.</param>
		public void AppendToXML( XmlElement parent )
		{
			XmlElement element = parent.OwnerDocument.CreateElement( "formats" );

			foreach( IXMLDataProvider data in m_hashNames.Values )
			{
				data.AppendToXML( element );
			}

			parent.AppendChild( element );
		}
		/// <summary>
		/// Append its data to given XML.
		/// </summary>
		/// <param name="writer">Parent element.</param>
		public void AppendToXML( XmlTextWriter writer )
		{
			writer.WriteStartElement( "formats" );

			foreach( IXMLDataProvider data in m_hashNames.Values )
			{
				data.AppendToXML( writer );
			}

			writer.WriteEndElement();
		}
		/// <summary>
		/// Gets format used for striking out 
		/// </summary>
		/// <param name="color">Color to use.</param>
		/// <returns>Format.</returns>
		public Format GetStrikeOutFormat( Color color )
		{
			if( m_strikeOutFormats.Contains( color ) )
				return ( Format )m_strikeOutFormats[ color ];

			Format format = ( Format )Add( "StrikeOutFormat" + m_strikeOutFormats.Count.ToString() );
			format.BackColor = Color.Empty;
			format.FontColor = Color.Empty;
			format.LineColor = Color.Empty;
			format.UnderlineStyle = UnderlineStyle.None;
			format.UseHatchFill = false;
			format.StrikeOutColor = color;
			m_strikeOutFormats.Add( color, format );

			return format;
		}
		/// <summary>
		/// Creates new format object with given border parameters or retrieves it from hashtable (if it's already created).
		/// </summary>
		/// <param name="style">Style of border.</param>
		/// <param name="color">Color of border.</param>
		/// <param name="weight">Weight of border line.</param>
		/// <returns>Created or retrieved format.</returns>
		public ISnippetFormat GetBorderFormat( FrameBorderStyle style, Color color, BorderWeight weight )
		{
			// Result format.
			ISnippetFormat format;

			// Second level hashtable.
			Hashtable htColor = m_borderFormats[ style ] as Hashtable;
			if( null == htColor )
			{
				htColor = new Hashtable();
				m_borderFormats.Add( style, htColor );
			}

			// Third level hashtable.
			Hashtable htWeight = htColor[ color ] as Hashtable;
			if( null == htWeight )
			{
				htWeight = new Hashtable();
				htColor.Add( color, htWeight );
			}

			format = htWeight[ weight ] as ISnippetFormat;
			// If format with given parameters already exists.
			if( null != format ) return format;

			format = Add( string.Format( "Border format #{0}", m_borderIndex ) );
			format.BorderStyle = style;
			format.BorderColor = color;
			format.BorderWeight = weight;
			m_borderIndex++;

			htWeight.Add( weight, format );
			return format;
		}
		/// <summary>
		/// Creates new format object with given text color or retrieves it from hashtable (if it's already created).
		/// </summary>
		/// <param name="color">Color of text.</param>
		/// <returns>Created or retrieved format.</returns>
		public ISnippetFormat GetTextColorFormat( Color color )
		{
			// Result format.
			ISnippetFormat format = m_textColorFormats[ color ] as ISnippetFormat;

			// If format with given parameters already exists.
			if( null != format ) return format;

			format = Add( string.Format( "Text color format #{0}", m_textColorIndex ) );
			format.FontColor = color;
			m_textColorIndex++;

			m_textColorFormats.Add( color, format );
			return format;
		}
		/// <summary>
		/// Creates new format object with given background color or retrieves it from hashtable (if it's already created).
		/// </summary>
		/// <param name="color">Color of text.</param>
		/// <returns>Created or retrieved format.</returns>
		public ISnippetFormat GetBackgroundColorFormat( Color color )
		{
			// Result format.
			ISnippetFormat format = m_backgroundColorFormats[ color ] as ISnippetFormat;

			// If format with given parameters already exists.
			if( null != format ) return format;

			format = Add( string.Format( "Background color format #{0}", m_backgroundColorIndex ) );
			format.BackColor = color;
			m_backgroundColorIndex++;

			m_backgroundColorFormats.Add( color, format );
			return format;
		}
		/// <summary>
		/// Creates new format object with given background and text color or retrieves it from hashtable (if it's already created).
		/// </summary>
		/// <param name="backColor">Color of text background.</param>
		/// <param name="textColor">Color of text.</param>
		/// <returns>Created or retrieved format.</returns>
		public ISnippetFormat GetBackgroundAndTextColorFormat( Color backColor, Color textColor )
		{
			// Result format.
			ISnippetFormat format;

			// Second level hashtable.
			Hashtable htText = m_backgroundAndTextColorFormats[ backColor ] as Hashtable;

			if( null == htText )
			{
				htText = new Hashtable();
				m_backgroundAndTextColorFormats.Add( backColor, htText );
			}

			format = htText[ textColor ] as ISnippetFormat;

			// If format with given parameters already exists.
			if( null != format )
			{
				return format;
			}
			else
			{
				format = Add( string.Format( "Border format #{0}", m_backgroundAndTextColorIndex ) );
				format.BackColor = backColor;
				format.FontColor = textColor;
				m_backgroundAndTextColorIndex++;
			}

			htText.Add( textColor, format );
			return format;
		}
		/// <summary>
		/// Gets tab string with legth set to SpacesInTab.
		/// </summary>
		/// <returns>Tab string with legth set to SpacesInTab.</returns>
		public string GetTabString()
		{
			string result;

			if( this.ShowWhiteSpaces && this.ShowWhiteSpaceProperties.ShowTabs )
			{
				result = this.ShowWhiteSpaceProperties.TabString;

				int symbsToAdd = this.SpacesInTab - result.Length;

				if( symbsToAdd < 0 )
				{
					result = result.Substring( 0, this.SpacesInTab );
				}
				else
				{
					result += new string( ' ', symbsToAdd );
				}
			}
			else
			{
				result = this.TabReplaceString;
			}

			return result;
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Adds Changed event handling.
		/// </summary>
		/// <param name="index">Zero-base index of in collection.</param>
		/// <param name="value">Format that has been inserted.</param>
		protected override void OnInsertComplete( int index, object value )
		{
			base.OnInsertComplete( index, value );

			if( value == null ) return;

			Format format = ( Format )value;
			format.Changed += new EventHandler( format_Changed );
			OnFormatChanged( format );
		}
		/// <summary>
		/// Removes Changed event handling.
		/// </summary>
		/// <param name="index">Zero-base index of in collection.</param>
		/// <param name="value">Format that has been removed.</param>
		protected override void OnRemoveComplete( int index, object value )
		{
			base.OnRemoveComplete( index, value );

			if( value == null ) return;

			Format format = ( Format )value;
			format.Changed -= new EventHandler( format_Changed );
			OnFormatChanged( format );
		}
		#endregion

		#region Class Virtual Methods
		/// <summary>
		/// Raises FormatChanged event.
		/// </summary>
		/// <param name="format">Format that has been changed.</param>
		protected virtual void OnFormatChanged( Format format )
		{
			if( FormatChanged != null )
			{
				FormatChanged( format, EventArgs.Empty );
			}

			RaiseOnChangedEvent();
		}
		#endregion

		#region IObjectInitialize Members
		/// <summary>
		/// Initializes object.
		/// Rebuilds hashes.
		/// </summary>
		public virtual void Initialize()
		{
			RebuildHashes();

			m_standartFormatNames.Clear();

			string[] names = Enum.GetNames( typeof( FormatType ) );
			Array values = Enum.GetValues( typeof( FormatType ) );

			for( int i = 0; i < names.Length; i++ )
			{
				m_standartFormatNames[ values.GetValue( i ) ] = names[ i ];
			}
		}
		/// <summary>
		/// Rebuilds hash of the name:format pairs.
		/// </summary>
		protected virtual void RebuildHashes()
		{
			m_hashNames.Clear();

			foreach( ISnippetFormat format in InnerList )
			{
                if (!m_hashNames.ContainsKey(format.Name))
                    m_hashNames.Add(format.Name, format);
			}

			if( !m_hashNames.ContainsKey( DEF_DEFAULT_FORMAT_NAME ) )
				m_hashNames.Add( DEF_DEFAULT_FORMAT_NAME, defFormat );
		}
		/// <summary>
		/// Recalcualtes Min/Max line heights.
		/// </summary>
		protected virtual void Recalculation()
		{
			m_iMaxLineHeight = 0;
			m_iMinLineHeight = int.MaxValue;

			foreach( Format format in m_hashNames.Values )
			{
				if( format.Font.Height > m_iMaxLineHeight )
					m_iMaxLineHeight = ( int )format.Font.Height;
				if( format.Font.Height < m_iMinLineHeight )
					m_iMinLineHeight = ( int )format.Font.Height;
			}
		}
		#endregion

		#region IXmlSerializable Members
		/// <summary>
		/// I need this method for custom serialiation
		/// to write my own complex object.
		/// </summary>
		/// <param name="writer">XmlWriter.</param>
		virtual public void WriteXml( XmlWriter writer )
		{
			if( writer == null )
				throw new ArgumentNullException( "writer" );

			XmlSerializer valueSer = new System.Xml.Serialization.XmlSerializer( typeof( Format ) );

			writer.WriteStartElement( "formats" );

			foreach( Format format in base.InnerList )
			{
				valueSer.Serialize( writer, format );
			}

			writer.WriteEndElement();
		}
		/// <summary>
		/// Gets XSD shema for serialization.
		/// </summary>
		/// <returns>XmlSchema.</returns>
		virtual public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}
		/// <summary>
		/// i need this method for custom serialiation
		/// to read my own complex object
		/// </summary>
		/// <param name="reader">XmlReader.</param>
		virtual public void ReadXml( XmlReader reader )
		{
			if( reader == null )
				throw new ArgumentNullException( "reader" );

			XmlSerializer valueSer = SerializersManager.GetSerializer( typeof( Format ) );
			if( !reader.IsEmptyElement )
			{
				reader.ReadStartElement( "formats" );
				while( reader.NodeType != XmlNodeType.EndElement )
				{
					Format format = ( Format )valueSer.Deserialize( reader );
					format.Parent = this;
					base.List.Add( format );
					reader.MoveToContent();
				}
				reader.ReadEndElement();

				RebuildHashes();
				Recalculation();
			}
		}
		#endregion

		#region IDisposable Members
		/// <summary>
		/// Disposes all used resources: Brushes, Fonts.
		/// </summary>
		public void Dispose()
		{
			foreach( IDisposable penDispose in m_PensHash.Values )
			{
				penDispose.Dispose();
			}

			m_PensHash.Clear();

			foreach( IDisposable solidBrushDispose in m_SolidBrushesHash.Values )
			{
				solidBrushDispose.Dispose();
			}

			m_SolidBrushesHash.Clear();

			foreach( Hashtable brushes in m_BrushesHash.Values )
			{
				foreach( object brush_OR_table in brushes.Values )
				{
					foreach( Brush brush in ( brush_OR_table as Hashtable ).Values )
					{
						brush.Dispose();
					}
				}
			}

			m_BrushesHash.Clear();
		}
		#endregion

		#region Class Event Handlers
		/// <summary>
		/// Calls OnFormatChanged method.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void format_Changed( object sender, EventArgs e )
		{
			OnFormatChanged( sender as Format );
		}
		#endregion
	}
}