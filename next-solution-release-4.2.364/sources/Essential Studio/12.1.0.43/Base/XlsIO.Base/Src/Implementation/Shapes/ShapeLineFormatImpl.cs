#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;

using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Drawing;

using FOPTE = Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing.MsofbtOPT.FOPTE;
using System.IO;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
using Syncfusion.XlsIO.Implementation.Exceptions;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.Shapes
{
  /// <summary>
  /// Class used for Shapes Line Format.
  /// </summary>
  public class ShapeLineFormatImpl
    : CommonObject
    , IShapeLineFormat
  {
    #region Class constants
    /// <summary>
    /// Represents default line weight mull.
    /// </summary>
    private const double DEF_LINE_WEIGHT_MULL = 12700.0;
    /// <summary>
    /// Represents max line weight.
    /// </summary>
    private const int DEF_LINE_MAX_WEIGHT = 1584;
    /// <summary>
    /// Represents length of helper parse array.
    /// </summary>
    private const int DEF_PARSE_ARR_LENGTH = 5088;
    #endregion

    #region Class static mebers
    /// <summary>
    /// Represents helper byte array to parse pattern.
    /// </summary>
    private static byte[] m_parsePattArray = new byte[ DEF_PARSE_ARR_LENGTH ];
    #endregion

    #region Class members
    /// <summary>
    /// Represents weight of shape line.
    /// </summary>
    private double m_weight = 0.75;
    /// <summary>
    /// Represents foreground color.
    /// </summary>
    private Color m_foreColor = ColorExtension.Black;
    /// <summary>
    /// Represents foreground color.
    /// </summary>
    private Color m_backColor = ColorExtension.White;
    /// <summary>
    /// Represents parent book.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Represents begin arrow style.
    /// </summary>
    private ExcelShapeArrowStyle m_beginArrowStyle;
    /// <summary>
    /// Represents end arrow style.
    /// </summary>
    private ExcelShapeArrowStyle m_endArrowStyle;
    /// <summary>
    /// Represents begin arrow length.
    /// </summary>
    private ExcelShapeArrowLength m_beginArrowLength = ExcelShapeArrowLength.ArrowHeadMedium;
    /// <summary>
    /// Represents end arrow length.
    /// </summary>
    private ExcelShapeArrowLength m_endArrowLength = ExcelShapeArrowLength.ArrowHeadMedium;
    /// <summary>
    /// Represents begin arrow width.
    /// </summary>
    private ExcelShapeArrowWidth m_beginArrowWidth = ExcelShapeArrowWidth.ArrowHeadMedium;
    /// <summary>
    /// Represents end arrow width.
    /// </summary>
    private ExcelShapeArrowWidth m_endArrowWidth = ExcelShapeArrowWidth.ArrowHeadMedium;
    /// <summary>
    /// Represents dash style.
    /// </summary>
    private ExcelShapeDashLineStyle m_dashStyle = ExcelShapeDashLineStyle.Solid;
    /// <summary>
    /// Represents line style.
    /// </summary>
    private ExcelShapeLineStyle m_style = ExcelShapeLineStyle.Line_Single;
    /// <summary>
    /// Represents line transparency.
    /// </summary>
    private double m_transparency = 0;
    /// <summary>
    /// Represents is line format visible.
    /// </summary>
    private bool m_visible = true;
    /// <summary>
    /// Represents line pattern.
    /// </summary>
    private ExcelGradientPattern m_pattern = ExcelGradientPattern.Pat_5_Percent;
    /// <summary>
    /// Indicate if line format contain pattern.
    /// </summary>
    private bool m_bContainPattern;
    /// <summary>
    /// Indicates whether border join is round.
    /// </summary>
    private bool m_bRound;
    private PreservationLogger m_logger;
    #endregion

    #region Class initialize method
    /// <summary>
    /// Initialize new static members.
    /// </summary>
    static ShapeLineFormatImpl()
    {
      int iIndex = 0;
#if ( WINRT )
      ResourceHandler resource = new ResourceHandler();
#endif
      for (int i = 1; i < 49; i++)
      {
#if ( WINRT )
          byte[] arr = resource.PatternArray[ (ShapeFillImpl.DEF_PATTERN_PREFIX + i.ToString())];
#else
          byte[] arr = ShapeFillImpl.GetResData( ShapeFillImpl.DEF_PATTERN_PREFIX + i.ToString() );
#endif
          m_parsePattArray[iIndex] = (byte)arr.Length;
          iIndex++;

          arr.CopyTo(m_parsePattArray, iIndex);
          iIndex += arr.Length;
      }

    }
    /// <summary>
    /// Creates new instance of object.
    /// </summary>
    /// <param name="application">Current application.</param>
    /// <param name="parent">Parent object.</param>
    public ShapeLineFormatImpl( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();
      m_logger = new PreservationLogger();
    }
    /// <summary>
    /// Creates new instance of object. Autoshape Implementation
    /// </summary>
    /// <param name="application">Current application.</param>
    /// <param name="parent">Parent object.</param>
    internal ShapeLineFormatImpl(IApplication application, object parent,PreservationLogger logger)
        : base(application, parent)
    {
        FindParents();
        m_logger = logger;
    }
    /// <summary>
    /// Finds all parent objects.
    /// </summary>
    private void FindParents()
    {
      m_book = ( WorkbookImpl )FindParent( typeof( WorkbookImpl ) );

      if( m_book == null )
        throw new ApplicationException( "Cann't find parent object." );
    }
    #endregion

    #region Class static methods
    /// <summary>
    /// Parses transparency.
    /// </summary>
    /// <param name="value">Transparency value to parse.</param>
    /// <returns>Returns parsed value.</returns>
    internal static double ParseTransparency( uint value )
    {
      return ( ShapeImpl.DEF_TRANSPARENCY_MULL_100 - value ) / ShapeImpl.DEF_TRANSPARENCY_MULL_100;
    }
    /// <summary>
    /// Serialize transparency.
    /// </summary>
    /// <param name="opt">Represents option holder.</param>
    /// <param name="id">Represents transparency id.</param>
    /// <param name="value">Represents transparency value.</param>
    internal static void SerializeTransparency( IFopteOptionWrapper opt, MsoOptions id, double value )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      //if( value < 0 || value > 1 )
      //  throw new ArgumentOutOfRangeException( "value" );

      int iResult = ( int )( ( 100 - value * 100 )* ShapeImpl.DEF_TRANSPARENCY_MULL );
      ShapeImpl.SerializeForte( opt, id, iResult );
    }
    /// <summary>
    /// Serialize color to stream.
    /// </summary>
    /// <param name="opt">Represents option holder.</param>
    /// <param name="col">Represents color to serialize.</param>
    /// <param name="id">Represents option id to serialize.</param>
    internal static void SerializeColor( IFopteOptionWrapper opt, ColorObject color, WorkbookImpl book,
      MsoOptions id )
    {
      if( opt == null ) 
        throw new ArgumentNullException( "opt" );

      byte[] arr;
      bool bSerialize = true;

      if( color.ColorType == ColorType.Indexed )
      {
        arr = new byte[] { ( byte )color.Value, 0, 0, ShapeFillImpl.DEF_COLOR_CONSTANT };
      }
      else
      {
        Color value = color.GetRGB( book );
        arr = new byte[] { value.R, value.G, value.B, 2 };

        if( value.A == 0 && value.R == 0 && value.G == 0 && value.B == 0 )
          bSerialize = false;
      }

      if( bSerialize )
        ShapeImpl.SerializeForte( opt, id, arr );
    }
    #endregion

    #region IShapeLineFormat properties
    /// <summary>
    /// Represents weight of the line in pts.( 0 - 1584 )
    /// </summary>
    public double Weight
    {
      get
      {
        return m_weight;
      }
      set
      {
        if( value < 0 || value > DEF_LINE_MAX_WEIGHT )
          throw new ArgumentOutOfRangeException( "Weight" );

        m_weight = value;
        Visible = true;
      }
    }
    /// <summary>
    /// Represents foreground color.
    /// </summary>
    public Color  ForeColor
    {
      get
      {
        return m_foreColor;
      }
      set
      {
        m_foreColor = value;
        Visible = true;
      }
    }
    /// <summary>
    /// Represents background color.
    /// </summary>
    public Color  BackColor
    {
      get
      {
        return m_backColor;
      }
      set
      {
        m_backColor = value;
        Visible = true;
      }
    }
    /// <summary>
    /// Represents foreground color index.
    /// </summary>
    public ExcelKnownColors ForeColorIndex
    {
      get
      {
        return m_book.GetNearestColor( m_foreColor );
      }
      set
      {
        ForeColor = m_book.GetPaletteColor( value );
      }
    }
    /// <summary>
    /// Represents background color index.
    /// </summary>
    public ExcelKnownColors BackColorIndex
    {
      get
      {
        return m_book.GetNearestColor( m_backColor );
      }
      set
      {
        BackColor = m_book.GetPaletteColor( value );
      }
    }
    /// <summary>
    /// Represents begin arrow head style.
    /// </summary>
    public ExcelShapeArrowStyle BeginArrowHeadStyle
    {
      get
      {
        return m_beginArrowStyle;
      }
      set
      {
        m_beginArrowStyle = value;
        Visible = true;
      }
    }
    /// <summary>
    /// Represents end arrow head style.
    /// </summary>
    public ExcelShapeArrowStyle EndArrowHeadStyle
    {
      get
      {
        return m_endArrowStyle;
        }
      set
      {
        m_endArrowStyle = value;
        Visible = true;
      }
    }
    /// <summary>
    /// Represents begin arrow head length.
    /// </summary>
    public ExcelShapeArrowLength BeginArrowheadLength
    {
      get
      {
        return m_beginArrowLength;
      }
      set
      {
        m_beginArrowLength = value;
        Visible = true;
      }
    }
    /// <summary>
    /// Represents end arrow head length.
    /// </summary>
    public ExcelShapeArrowLength EndArrowheadLength
    {
      get
      {
        return m_endArrowLength;
      }
      set
      {
        m_endArrowLength = value;
        Visible = true;
      }
    }
    /// <summary>
    /// Represents begin arrow head width.
    /// </summary>
    public ExcelShapeArrowWidth BeginArrowheadWidth
    {
      get
      {
        return m_beginArrowWidth;
      }
      set
      {
        m_beginArrowWidth = value;
        Visible = true;
      }
    }
    /// <summary>
    /// Represents end arrow head width.
    /// </summary>
    public ExcelShapeArrowWidth EndArrowheadWidth
    {
      get
      {
        return m_endArrowWidth;
      }
      set
      {
        m_endArrowWidth = value;
        Visible = true;
      }
    }
    /// <summary>
    /// Represents the dash style for the specified line.
    /// </summary>
    public ExcelShapeDashLineStyle DashStyle
    {
      get
      {
        return m_dashStyle;
      }
      set
      {
        m_dashStyle = value;
        Visible = true;
      }
    }
    /// <summary>
    /// Represents line style.
    /// </summary>
    public ExcelShapeLineStyle Style
    {
      get
      {
        return m_style;
      }
      set
      {
        m_style = value;
        Visible = true;
      }
    }
    /// <summary>
    /// Represents line transparency.
    /// </summary>
    public double Transparency
    {
      get
      {
        return m_transparency;
      }
      set
      {
        if( value < 0 || value > 1 )
          throw new ArgumentOutOfRangeException( "value" );

        m_transparency = value;
        Visible = true;
      }
    }
    /// <summary>
    /// Represents if line format is visible.
    /// </summary>
    public bool Visible
    {
      get
      {
        return m_visible;
      }
      set
      {
        m_visible = value;
        this.m_logger.SetFlag(PreservedFlag.Line);
      }
    }
    /// <summary>
    /// Represents line pattern.
    /// </summary>
    public ExcelGradientPattern Pattern
    {
      get
      {
        if( !m_bContainPattern )
          throw new NotSupportedException( "Doesn't checked patterned style." );

        return m_pattern;
      }
      set
      {
        m_pattern = value;

        HasPattern = true;
        Visible = true;
      }
    }
    /// <summary>
    /// Indicates if current line format contain pattern.
    /// </summary>
    public bool HasPattern
    {
      get
      {
        return m_bContainPattern;
      }
      set
      {
        if( HasPattern != value )
        {
          m_bContainPattern = value;
          Visible = true;
        }
      }
    }
    #endregion

    #region Properties
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl Workbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Indicates whether join is round.
    /// </summary>
    public bool IsRound
    {
      get
      {
        return m_bRound;
      }
      set
      {
        m_bRound = value;
      }
    }
    #endregion

    #region Class parse methods
    /// <summary>
    /// Parses fill options.
    /// </summary>
    /// <param name="option">Record to parse.</param>
    /// <returns>Value indicating extracted option.</returns>
    [ CLSCompliant( false ) ]
    public bool ParseOption( FOPTE option )
    {
      if( option == null )
        throw new ArgumentNullException( "option" );

      if( ParseArrowsPropertys( option ) )
        return true;
      
      switch( option.Id )
      {
        case MsoOptions.NoLineDrawDash:
          ParseVisible( option.MainData );
          return true;

        case MsoOptions.LineStyle:
          m_style = ( ExcelShapeLineStyle )( option.UInt32Value + 1 );
          return true;

        case MsoOptions.LineWeight:
          m_weight = ( double )( option.UInt32Value / DEF_LINE_WEIGHT_MULL );
          return true;

        case MsoOptions.LineDashStyle:
          m_dashStyle = ( ExcelShapeDashLineStyle )option.UInt32Value;
          return true;

        case MsoOptions.ContainRoundDot:
          m_dashStyle = ExcelShapeDashLineStyle.Dotted_Round;
          return true;

        case MsoOptions.LineTransparency:
          m_transparency = ParseTransparency( option.UInt32Value );
          return true;

        case MsoOptions.LineColor:
          m_foreColor = ShapeFillImpl.ParseColor( m_book, option.MainData );
          return true;

        case MsoOptions.LineBackColor:
          m_backColor = ShapeFillImpl.ParseColor( m_book, option.MainData );
          return true;

        case MsoOptions.ContainLinePattern:
          m_bContainPattern = true;
          return true;

        case MsoOptions.LinePattern:
          m_pattern = ParsePattern( option );
          return true;
      }

      return false;
    }
    /// <summary>
    /// Parses arrows options
    /// </summary>
    /// <param name="option">Represents option to parse.</param>
    /// <returns>Returns true if parsed; otherwise false.</returns>
    private bool ParseArrowsPropertys( FOPTE option )
    {
      if( option == null )
        throw new ArgumentNullException( "option" );

      switch( option.Id )
      {
        case MsoOptions.LineStartArrow:
          m_beginArrowStyle = ( ExcelShapeArrowStyle )option.UInt32Value;
          return true;

        case MsoOptions.LineEndArrow:
          m_endArrowStyle = ( ExcelShapeArrowStyle )option.UInt32Value;
          return true;

        case MsoOptions.StartArrowLength:
          m_beginArrowLength = ( ExcelShapeArrowLength )option.UInt32Value;
          return true;

        case MsoOptions.EndArrowLength:
          m_endArrowLength = ( ExcelShapeArrowLength )option.UInt32Value;
          return true;

        case MsoOptions.StartArrowWidth:
          m_beginArrowWidth = ( ExcelShapeArrowWidth )option.UInt32Value;
          return true;

        case MsoOptions.EndArrowWidth:
          m_endArrowWidth = ( ExcelShapeArrowWidth )option.UInt32Value;
          return true;
      }

      return false;
    }
    /// <summary>
    /// Parses current pattern.
    /// </summary>
    /// <param name="option">Represents pattern option.</param>
    /// <returns>Returns parsed pattern value.</returns>
    private ExcelGradientPattern ParsePattern( FOPTE option )
    {
      if( option == null )
        throw new ArgumentNullException( "option" );

      byte[] arr;

      if( option.AdditionalData == null || option.AdditionalData.Length == 0 )
      {
        if( option.UInt32Value > 0 )
        {
          MsofbtBSE data = m_book.ShapesData.GetPicture( ( int )option.UInt32Value );

          MemoryStream stream = new MemoryStream( data.Length );
          data.InfillInternalData( stream, 0, null, null );
          //byte[] pattData = data.m_data;
          arr = new byte[ data.Length - 36 ];

          //Array.Copy( pattData, 36, arr, 0, arr.Length );
          stream.Position = 36;
          stream.Read( arr, 0, arr.Length );
        }
        else
        {
          return ExcelGradientPattern.Pat_5_Percent;
        }
      }
      else
      {
        arr = option.AdditionalData;
      }

      return GetPattern( arr );
    }
    /// <summary>
    /// Parse visible property.
    /// </summary>
    /// <param name="data">Represents visible data.</param>
    private void ParseVisible( byte[] data )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      byte value = data[ 4 ];
      byte firstByte = data[ 2 ];

      // TODO: add flags based on the documentation.
      if( ( firstByte < 8 || firstByte > 16 ) && data[ 3 ] == 0 && data[ 5 ] == 0
        && ( value >= 8 && value <= 16 || value == 24 ) || value == 24 && firstByte == 16 )
      {
        m_visible = false;
      }
      else
      {
        m_visible = true;
      }
    }
    #endregion

    #region Class serialize methods
    /// <summary>
    /// Serialize shape line format as biff format.
    /// </summary>
    /// <param name="opt">Represents option holder.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( MsofbtOPT opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      if( m_visible )
        ShapeImpl.SerializeForte( opt, MsoOptions.LineWeight, ( int )( m_weight * DEF_LINE_WEIGHT_MULL ) );

      SerializeVisible( opt );

      if( m_visible )
      {
        SerializeColor( opt, m_foreColor, m_book, MsoOptions.LineColor );
        SerializeColor( opt, m_backColor, m_book, MsoOptions.LineBackColor );
        SerializeArrowProperties( opt );
        SerializeDashStyle( opt );
        SerializeLineStyle( opt );
        SerializeTransparency( opt, MsoOptions.LineTransparency, m_transparency );
        SerializePattern( opt );
      }
    }
    /// <summary>
    /// Serialize arrow line options.
    /// </summary>
    /// <param name="opt">Represents options storage.</param>
    /// <returns>Returns updated option storage.</returns>
    private void SerializeArrowProperties( MsofbtOPT opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );
      
      ShapeImpl.SerializeForte( opt, MsoOptions.LineStartArrow, ( int )m_beginArrowStyle );
      ShapeImpl.SerializeForte( opt, MsoOptions.LineEndArrow, ( int )m_endArrowStyle );
      ShapeImpl.SerializeForte( opt, MsoOptions.StartArrowLength, ( int )m_beginArrowLength );
      ShapeImpl.SerializeForte( opt, MsoOptions.EndArrowLength, ( int )m_endArrowLength );
      ShapeImpl.SerializeForte( opt, MsoOptions.StartArrowWidth, ( int )m_beginArrowWidth );
      ShapeImpl.SerializeForte( opt, MsoOptions.EndArrowWidth, ( int )m_endArrowWidth );
    }
    /// <summary>
    /// Serialize dash style as biff recorded structure.
    /// </summary>
    /// <param name="opt">Represents option holder.</param>
    private void SerializeDashStyle( MsofbtOPT opt )
    {
      ExcelShapeDashLineStyle style = m_dashStyle;

      if( opt == null )
        throw new ArgumentNullException( "opt" );

      if( style == ExcelShapeDashLineStyle.Dotted_Round )
      {
        style = ExcelShapeDashLineStyle.Dotted;
        ShapeImpl.SerializeForte( opt, MsoOptions.ContainRoundDot, 0 );
      }
      else
      {
        opt.RemoveOption( ( int )MsoOptions.ContainRoundDot );
      }

      ShapeImpl.SerializeForte( opt, MsoOptions.LineDashStyle, ( int )style );
    }
    /// <summary>
    /// Serialize line style as biff recorded structure.
    /// </summary>
    /// <param name="opt">Represents option holder.</param>
    private void SerializeLineStyle( MsofbtOPT opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      int value = ( int )m_style;

      ShapeImpl.SerializeForte( opt, MsoOptions.LineStyle, value - 1 );
    }
    /// <summary>
    /// Serialize visible property.
    /// </summary>
    /// <param name="opt">Represents option holder.</param>
    private void SerializeVisible( MsofbtOPT opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      byte[] arr = { 8, 0, 8, 0 };

      if( !m_visible )
      {
        arr[ 0 ] = 0;
        arr[ 2 ] = 24;
      }
      
      ShapeImpl.SerializeForte( opt, MsoOptions.NoLineDrawDash, arr );
    }
    /// <summary>
    /// Serialize pattern to mso option.
    /// </summary>
    /// <param name="opt">Represents option holder.</param>
    private void SerializePattern( MsofbtOPT opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      string strIndex = ( ( int )m_pattern ).ToString();
#if ( WINRT )
        ResourceHandler resource=new ResourceHandler();
        byte[] arr=resource.PatternArray[ShapeFillImpl.DEF_PATTERN_PREFIX + strIndex];
#else
      byte[] arr = ShapeFillImpl.GetResData( ShapeFillImpl.DEF_PATTERN_PREFIX + strIndex );
#endif
      if( m_bContainPattern )
      {
        ShapeImpl.SerializeForte( opt, MsoOptions.LinePattern, 0, arr, true );
        ShapeImpl.SerializeForte( opt, MsoOptions.ContainLinePattern, 1 );
      }
      else
      {
        opt.RemoveOption( ( int )MsoOptions.LinePattern );
        opt.RemoveOption( ( int )MsoOptions.ContainLinePattern );
      }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Gets pattern by pattern data.
    /// </summary>
    /// <param name="arr">Represents patt data.</param>
    /// <returns>Returns parsed data.</returns>
    private ExcelGradientPattern GetPattern( byte[] arr )
    {
      if( arr == null )
        throw new ArgumentNullException( "arr" );

      int iPatIndex = 1;
      int iArrIndex = 0;
      int iLen = arr.Length;
      int i;

      while( iArrIndex < m_parsePattArray.Length )
      {
        i = 0;
        iArrIndex++;

        if( m_parsePattArray[ iArrIndex - 1 ] == iLen )
        {
          for( ; i < iLen; i++ )
          {
            if( arr[ i ] != m_parsePattArray[ iArrIndex + i ] )
              break;
          }

          if( i == iLen )
            return ( ExcelGradientPattern )iPatIndex;
        }


        iArrIndex += m_parsePattArray[ iArrIndex - 1 ];
        iPatIndex++;
      }

      return ExcelGradientPattern.Pat_5_Percent;
    }
    #endregion

    #region Clone method
    /// <summary>
    /// Clones current object.
    /// </summary>
    /// <param name="parent">Represents parent object for new instance.</param>
    /// <returns>Returns cloned object.</returns>
    public ShapeLineFormatImpl Clone( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      ShapeLineFormatImpl result = ( ShapeLineFormatImpl )MemberwiseClone();

      result.SetParent( parent );
      result.FindParents();

      return result;
    }
    #endregion
  }
}
