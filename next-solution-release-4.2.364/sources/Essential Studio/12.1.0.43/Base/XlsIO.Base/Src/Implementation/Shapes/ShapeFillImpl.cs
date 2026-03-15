#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;
using System.Reflection;
using System.Resources;
using System.IO;
using System.Collections;

using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Drawing;

using FOPTE = Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing.MsofbtOPT.FOPTE;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using Syncfusion.XlsIO.Implementation.Exceptions;
using System.Drawing;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using Syncfusion.XlsIO.Implementation.Exceptions;
using System.Drawing;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

#endregion

namespace Syncfusion.XlsIO.Implementation.Shapes
{
  /// <summary>
  /// Represents fill format in shape object.
  /// </summary>
  public class ShapeFillImpl
    : CommonObject
    , IFill
    , IInternalFill
    , IGradient
  {
    #region Class constants
    /// <summary>
    /// Represents default vertical shading style.
    /// </summary>
    private const int DEF_SHAD_STYLE_VERTICAL = 166;
    /// <summary>
    /// Represents default vertical shading style in Excel 2007 generated xls files.
    /// </summary>
    private const int DEF_SHAD_STYLE_VERTICAL2007 = 90;
    /// <summary>
    /// Represents default diagonal up shading style.
    /// </summary>
    private const int DEF_SHAD_STYLE_DIAGONAL_UP = 121;
    /// <summary>
    /// Represents default diagonal down shading style.
    /// </summary>
    private const int DEF_SHAD_STYLE_DIAGONAL_DOWN = 211;
    /// <summary>
    /// Represents color constant.
    /// </summary>
    internal const int DEF_COLOR_CONSTANT = 8;
    /// <summary>
    /// Represents default shade variant array value.
    /// </summary>
    private static readonly byte[] DEF_VARIANT_FIRST_ARR = { 100, 0, 0, 0 };
    /// <summary>
    /// Represents default shade third variant array value.
    /// </summary>
    private static readonly byte[] DEF_VARIANT_THIRD_ARR = { 206, 255, 255, 255 };
    /// <summary>
    /// Represents default shade variant array value.
    /// </summary>
    private static readonly byte[] DEF_VARIANT_FOURTH_ARR = { 50, 0, 0, 0 };
    /// <summary>
    /// Represents default value for one color.
    /// </summary>
    private const int DEF_ONE_COLOR_STYLE_VALUE = ( int )0x4000000b;
    /// <summary>
    /// Represents additional data for center variants.
    /// </summary>
    private static readonly byte[] DEF_VARIANT_CENTER_ADD_DATA = { 0, 128, 0, 0 };
    /// <summary>
    /// Represents additional data for corner variants.
    /// </summary>
    private static readonly byte[] DEF_VARIANT_CORNER_ADD_DATA = { 0, 0, 1, 0 };
    /// <summary>
    /// Represents pattern prefix.
    /// </summary>
    public const string DEF_PATTERN_PREFIX = "Patt";
    /// <summary>
    /// Represents texture prefix.
    /// </summary>
    internal const string DEF_TEXTURE_PREFIX = "Text";
    /// <summary>
    /// Represents preset gradient prefix.
    /// </summary>
    private const string DEF_GRAD_PREFIX = "Grad";
    /// <summary>
    /// Represents pattern enum prefix.
    /// </summary>
    private const string DEF_PATTERN_ENUM_PREFIX = "Pat_";
    /// <summary>
    /// Represents value, that indicate that fill doesn't visible.
    /// </summary>
    private const byte DEF_NOT_VISIBLE_VALUE = 16;
    /// <summary>
    /// Represents array, that indicate, that current picture is bitmap.
    /// </summary>
    private static readonly byte[] DEF_BITMAP_INDEX = { 128, 122, 31, 240 };
    /// <summary>
    /// Represents default comment fill color.
    /// </summary>
    public static readonly Color DEF_COMENT_PARSE_COLOR = Color.FromArgb( 255, 255, 255, 222 );
    /// <summary>
    /// Represents index, that represents default comment color.
    /// </summary>
    public const int DEF_COMMENT_COLOR_INDEX = 80;
    /// <summary>
    /// Represents corner gradient style.
    /// </summary>
    private const int DEF_CORNER_STYLE = 5;
    /// <summary>
    /// Represents center gradient style.
    /// </summary>
    private const int DEF_CENTER_STYLE = 6;
    /// <summary>
    /// Represents default offset.
    /// </summary>
    private const int DEF_OFFSET = 25;
    /// <summary>
    /// Maximum value for such attributes like Alpha, Tint, Shade.
    /// </summary>
    internal const int MaxValue = 100000;
    /// <summary>
    /// Represents horizontal angle of fill.
    /// </summary>
    internal const int HorizontalAngle = 5400000;
    /// <summary>
    /// Represents vertical angle of fill.
    /// </summary>
    internal const int VerticalAngle = 0;
    /// <summary>
    /// Represents Diagonal up angle of fill.
    /// </summary>
    internal const int DiagonalUpAngle = 2700000;
    /// <summary>
    /// Represents Diagonal down angle of fill.
    /// </summary>
    internal const int DiagonalDownAngle = 18900000;
    /// <summary>
    /// Represents rectangular structure fill from center.
    /// </summary>
    internal static Rectangle RectangleFromCenter = Rectangle.FromLTRB( 50000, 50000, 50000, 50000 );
    /// <summary>
    /// Represents rectangular structure fill from corner.
    /// </summary>
    internal static Rectangle[] RectanglesCorner = new Rectangle[]
    {
      Rectangle.FromLTRB( 0, 0, 100000, 100000 ),
      Rectangle.FromLTRB( 100000, 0, 0, 100000 ),
      Rectangle.FromLTRB( 0, 100000, 100000, 0 ),
      Rectangle.FromLTRB( 100000, 100000, 0, 0 ),
    };
    #endregion

    #region Class members
    /// <summary>
    /// Dictionary to access to resource data. Key - string, resource id, value - byte[], resource data.
    /// </summary>
    private static Dictionary<string, byte[]> m_dicResources = new Dictionary<string, byte[]>();
    /// <summary>
    /// Represents fill type.
    /// </summary>
    protected ExcelFillType m_fillType = ExcelFillType.SolidColor;
    /// <summary>
    /// Represents shading style.
    /// </summary>
    private ExcelGradientStyle m_gradStyle;
    /// <summary>
    /// Represents current shading variants.
    /// </summary>
    private ExcelGradientVariants m_gradVariant = ExcelGradientVariants.ShadingVariants_2;
    /// <summary>
    /// Represents transparency to.
    /// </summary>
    private double m_transparencyTo;
    /// <summary>
    /// Represents transparency from.
    /// </summary>
    private double m_transparencyFrom;
    /// <summary>
    /// Represents gradient style.
    /// </summary>
    private ExcelGradientColor m_gradientColor = ExcelGradientColor.TwoColor;
    /// <summary>
    /// Represents gradient pattern.
    /// </summary>
    private ExcelGradientPattern m_gradPattern = ExcelGradientPattern.Pat_5_Percent;
    /// <summary>
    /// Represents gradient texture.
    /// </summary>
    private ExcelTexture m_gradTexture = ExcelTexture.Papyrus;
    /// <summary>
    /// Represents parent book.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Represents background color index.
    /// </summary>
    private ColorObject m_backColor = new ColorObject( ColorExtension.Black );
    /// <summary>
    /// Represents foreground color index.
    /// </summary>
    private ColorObject m_foreColor = new ColorObject( ColorExtension.Gray );
    /// <summary>
    /// Represents preset gradient.
    /// </summary>
    private ExcelGradientPreset m_presetGrad = ExcelGradientPreset.Grad_Early_Sunset;
    /// <summary>
    /// Represents user defined picture of texture.
    /// </summary>
    protected Image m_picture;
    /// <summary>
    /// Represents picture name.
    /// </summary>
    private string m_strPictureName;
    /// <summary>
    /// Represents if fill style is visible.
    /// </summary>
    private bool m_bVisible = true;
    /// <summary>
    /// Represents image index.
    /// </summary>
    private int m_imageIndex = -1;
    /// <summary>
    /// Represents gradient degree.
    /// </summary>
    private double m_gradDegree = 0.2;
    /// <summary>
    /// Represents picture data to parse.
    /// </summary>
    private FOPTE m_parsePictureData;
    /// <summary>
    /// Indicates if this instance of object is shape fill.
    /// </summary>
    protected bool m_bIsShapeFill = true;
    ///// <summary>
    ///// 
    ///// </summary>
    //private GradientStops m_gradientStops;
    private bool m_bTile;
    /// <summary>
    /// Represent the fillrectangle
    /// </summary>
    private Rectangle m_fillrect;
    /// <summary>
    /// Represent the source rectangle
    /// </summary>
    private Rectangle m_srcRect;
    /// <summary>
    /// It's define the alphamodfix value 
    /// </summary>
    private float m_amt;
    /// <summary>
    /// Preset gradient.
    /// </summary>
    private GradientStops m_preseredGradient;
    /// <summary>
    /// Indicates whether gradient is supported.
    /// </summary>
    private bool m_bSupportedGradient = true;
    /// <summary>
    /// It's define the texture attributes 
    /// </summary>
    private float m_textureVerticalScale;
    private float m_textureHorizontalScale;
    private float m_textureOffsetX;
    private float m_textureOffsetY;
    private string m_alignment;
    private string m_tileFlipping;
      
    #endregion

    #region Class static members
    /// <summary>
    /// Represents current assembly.
    /// </summary>
    private static Assembly s_asem = 
#if ( WINRT || WP )
        typeof( ShapeFillImpl ).GetTypeInfo().Assembly;
#else
        typeof( ShapeFillImpl ).Assembly;
#endif
    /// <summary>
    /// Represent array, that contain all preset gradient types.
    /// </summary>
    private static byte[] m_arrPreset = new byte[ 1320 ];
    /// <summary>
    /// This collection contains gradient stops collection for preset gradients.
    /// </summary>
    private static Dictionary<ExcelGradientPreset, byte[]> s_dicPresetStops;
    private PreservationLogger m_logger = new PreservationLogger();
    #endregion

    #region Class static contstructors
    /// <summary>
    /// Initialize all static members.
    /// </summary>
    static ShapeFillImpl()
    {
      int iIndex = 0;
      int iLen;
#if ( WINRT )
        ResourceHandler  resource=new ResourceHandler();
#endif
        for (int i = 1; i <= 24; i++)
        {
#if ( WINRT )
            byte[] arr = resource.PresetDictionary[DEF_GRAD_PREFIX + i.ToString()];
#else
            byte[] arr = GetResData( DEF_GRAD_PREFIX + i.ToString() );
#endif

          iLen = arr.Length;
          m_arrPreset[iIndex] = (byte)iLen;

          iIndex++;
          arr.CopyTo(m_arrPreset, iIndex);
          iIndex += iLen;
      }

    }
    #endregion

    #region Class static methods
    /// <summary>
    /// Gets picture from resources file by id.
    /// </summary>
    /// <param name="strID">Represents unique id in resource file.</param>
    /// <returns>Returns picture from resource file by unique id.</returns>
    public static byte[] GetResData( string strID )
    {
      byte[] result;
      
        //For WinRT Refer :http://msdn.microsoft.com/en-us/library/system.resources.resourcemanager(v=vs.110).aspx#WINRT
      if( !m_dicResources.TryGetValue( strID, out result ) )
      {
#if ( WINRT )
        ResourceManager resources = new ResourceManager("Syncfusion.XlsIO.Metro.TexturePatternGradient"
        , s_asem);
          string value=resources.GetString(strID);
          result=System.Text.Encoding.UTF8.GetBytes(value);
#elif ( WP )
          ResourceManager resources = new ResourceManager("Syncfusion.XlsIO.WP8.TexturePatternGradient", s_asem);
          result = (byte[])resources.GetObject(strID);
#else
          ResourceManager resources = new ResourceManager( "Syncfusion.XlsIO.TexturePatternGradient"
          , s_asem );
        result = ( byte[] )resources.GetObject( strID );
#endif
          m_dicResources[ strID ] = result;
      }

      return result;
    }
    /// <summary>
    /// Parses color.
    /// </summary>
    /// <param name="book">Represents parent book.</param>
    /// <param name="value">Color value to parse.</param>
    /// <returns>Extracted color.</returns>
    internal static Color ParseColor( WorkbookImpl book, byte[] value )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      Color color;

      if( value[ 5 ] == DEF_COLOR_CONSTANT )
      {
        color = ( value[ 2 ] == DEF_COMMENT_COLOR_INDEX )
          ? DEF_COMENT_PARSE_COLOR
          : book.GetPaletteColor( ( ExcelKnownColors )value[ 2 ] );
      }
      else
      {
        color = Color.FromArgb( 255, value[ 2 ], value[ 3 ], value[ 4 ] );
      }

      return color;
    }
    /// <summary>
    /// Parses color.
    /// </summary>
    /// <param name="book">Represents parent book.</param>
    /// <param name="value">Color value to parse.</param>
    internal static void ParseColor( WorkbookImpl book, byte[] value, ColorObject color )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      if( color == null )
        throw new ArgumentNullException( "color" );

      //Color color;

      if( value[ 5 ] == DEF_COLOR_CONSTANT )
      {
        if( value[ 2 ] == DEF_COMMENT_COLOR_INDEX )
        {
          //color.//DEF_COMENT_PARSE_COLOR
          color.SetIndexed( ( ExcelKnownColors )DEF_COMMENT_COLOR_INDEX );
        }
        else
        {
          color.SetIndexed( ( ExcelKnownColors )( value[ 2 ] ) );
        }
        //book.GetPaletteColor( ( ExcelKnownColors )value[ 2 ] );
      }
      else if( value[ 5 ] == 16 ) // Unknown value
      {
        color.SetRGB( Color.FromArgb( 0, 0, 0, 0 ) );
      }
      else
      {
        //color = Color.FromArgb( 255, value[ 2 ], value[ 3 ], value[ 4 ] );
        color.SetRGB( Color.FromArgb( 255, value[ 2 ], value[ 3 ], value[ 4 ] ), book );
      }
    }
    /// <summary>
    /// Returns collection of gradient stops that stores specified preset color.
    /// </summary>
    /// <param name="preset">Gradient preset to return gradient stops collection for.</param>
    /// <returns>Corresponding gradient stops collection.</returns>
    public static GradientStops GetPresetGradientStops( ExcelGradientPreset preset )
    {
      byte[] arrData = GetPresetGradientStopsData( preset );
      GradientStops result = new GradientStops( arrData );
      return result;
    }
    /// <summary>
    /// Returns binary data for collection of gradient stops that stores specified preset color.
    /// </summary>
    /// <param name="preset">Gradient preset to return gradient stops collection for.</param>
    /// <returns>Corresponding gradient stops binary data.</returns>
    public static byte[] GetPresetGradientStopsData( ExcelGradientPreset preset )
    {
      if( s_dicPresetStops == null )
        FillPresetsGradientStops();

      byte[] arrData = s_dicPresetStops[ preset ];
      return arrData;
    }
    #endregion

    #region Class initalize methods
    /// <summary>
    /// Creates new instance of this class.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    public ShapeFillImpl( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();
      m_foreColor = new ColorObject( ColorExtension.White );//ExcelKnownColors.White );
      m_backColor = new ColorObject( ColorExtension.Empty );//ExcelKnownColors.None );
      m_backColor.AfterChange += ChangeVisible;
      m_foreColor.AfterChange += ChangeVisible;
    }
    /// <summary>
    /// Creates new instance of this class.
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    /// <param name="fillType">Fill type.</param>
    public ShapeFillImpl( IApplication application, object parent, ExcelFillType fillType )
      : this( application, parent )
    {
      m_fillType = fillType;
    }
    /// <summary>
    /// Creates new instance of this class. AutoShapeImplementation
    /// </summary>
    /// <param name="application">Represents current application.</param>
    /// <param name="parent">Represents parent object.</param>
    /// <param name="fillType">Fill type.</param>
    internal ShapeFillImpl(IApplication application, object parent, ExcelFillType fillType, PreservationLogger logger)
        : this(application, parent)
    {
        m_fillType = fillType;
        m_logger = logger;
    }
    /// <summary>
    /// Finds parent objects.
    /// </summary>
    private void FindParents()
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ApplicationException( "Cann't find parent book" );
    }
    #endregion

    #region Properties
    /// <summary>
    /// Returns collection of gradient stops that stores gradient fill settings. Read-only.
    /// </summary>
    public GradientStops GradientStops
    {
      get
      {
        GradientStops result = null;

        if( m_fillType == ExcelFillType.Gradient && m_bSupportedGradient )
        {
          if( m_gradientColor == ExcelGradientColor.Preset )
          {
            result = GetPresetGradientStops( m_presetGrad );
          }
          else if( m_gradientColor == ExcelGradientColor.OneColor )
          {
            result = new GradientStops();
            GradientStopImpl gradientStop = new GradientStopImpl( ForeColorObject, 0, MaxValue );
            result.Add( gradientStop );

            byte btDegree = ( byte )( m_gradDegree * byte.MaxValue );
            int iHalfByte = byte.MaxValue / 2;
            int iShade = ( btDegree <= iHalfByte ) ? btDegree * MaxValue / byte.MaxValue : -1;
            int iTint = ( btDegree > iHalfByte ) ? ( byte.MaxValue - btDegree ) * MaxValue / byte.MaxValue : -1;

            gradientStop = new GradientStopImpl( ForeColorObject, GradientStops.MaxPosition, MaxValue, iTint, iShade );
            result.Add( gradientStop );
          }
          else if( m_gradientColor == ExcelGradientColor.TwoColor )
          {
            result = new GradientStops();
            GradientStopImpl gradientStop = new GradientStopImpl( ForeColorObject, 0, MaxValue );
            result.Add( gradientStop );

            gradientStop = new GradientStopImpl( BackColorObject, GradientStops.MaxPosition, MaxValue );
            result.Add( gradientStop );
          }

          if( IsInverted( m_gradStyle, m_gradVariant ) )
            result.InvertGradientStops();
          
          if( IsDoubled( m_gradStyle, m_gradVariant ) )
            result.DoubleGradientStops();

          result.Angle = GradientAngle( m_gradStyle );
          result.FillToRect = GradientFillToRect( m_gradStyle, m_gradVariant );
          result.GradientType = GetGradientType( m_gradStyle );
        }

        return result;
      }
    }
    /// <summary>
    /// Represents whether picture is tiled or stretched.
    /// </summary>
    public bool Tile
    {
      get
      {
        return m_bTile;
      }
      set
      {
        m_bTile = value;
      }
    }
    public GradientStops PreservedGradient
    {
      get
      {
        return m_preseredGradient;
      }
      set
      {
        m_preseredGradient = value;
      }
    }
    /// <summary>
    /// Gets or Sets the fillrect values
    /// </summary>
    public Rectangle FillRect
    {
        get
        {
            return m_fillrect;
        }
        set
        {
           m_fillrect = value;
        }
    }
    /// <summary>
    /// Gets or Sets the sourcerect values
    /// </summary>
    public Rectangle SourceRect
    {
        get
        {
            return m_srcRect;
        }
        set
        {
            m_srcRect = value;
        }
    }    
    /// <summary>
    /// Gets parsed picture data.
    /// </summary>
    internal FOPTE ParsePictureData
    {
      get
      {
        return m_parsePictureData;
      }
    }
    public bool IsGradientSupported
    {
      get
      {
        return m_bSupportedGradient;
      }
      set
      {
        m_bSupportedGradient = value;
      }
    }
    #endregion

    #region IFill properties
    /// <summary>
    /// Represents shape fill type.
    /// </summary>
    public ExcelFillType FillType
    {
      get
      {
        return m_fillType;
      }
      set
      {
          if (FillType != value || Parent is Syncfusion.XlsIO.Implementation.Charts.ChartWallOrFloorImpl)
        {
          if( value == ExcelFillType.Picture )
            throw new ArgumentException( "For set picture type use UserPicture method." );

          if( value == ExcelFillType.Texture )
            m_gradTexture = ExcelTexture.Papyrus;

          if( value == ExcelFillType.Gradient )
            m_gradVariant = ExcelGradientVariants.ShadingVariants_1;

          m_fillType = value;
          ChangeVisible();
        }
      }
    }
    /// <summary>
    /// Represents gradient shading style.
    /// </summary>
    public ExcelGradientStyle GradientStyle
    {
      get
      {
        ValidateGradientType();

        return m_gradStyle;
      }
      set
      {
        ValidateGradientType();

        m_gradStyle = value;
        ChangeVisible();
      }
    }
    /// <summary>
    /// Represents current shading variant.
    /// </summary>
    public ExcelGradientVariants GradientVariant
    {
      get
      {
        ValidateGradientType();

        return m_gradVariant;
      }
      set
      {
        ValidateGradientType();

        bool bFlag = value == ExcelGradientVariants.ShadingVariants_3
          || value == ExcelGradientVariants.ShadingVariants_4;

        if( m_gradStyle == ExcelGradientStyle.From_Center && bFlag )
          throw new NotSupportedException( "This variant doesn't support center shading style." );

        m_gradVariant = value;
        ChangeVisible();
      }
    }
    /// <summary>
    /// Returns or sets the degree of transparency of the specified fill as
    ///  a value from 0.0 (opaque) through 1.0 (clear).
    /// </summary>
    public virtual double TransparencyTo
    {
      get
      {
        ValidateGradientType();

        return m_transparencyTo;
      }
      set
      {
        ValidateGradientType();

        if( value < 0 || value > 1 )
          throw new ArgumentOutOfRangeException( "TransparencyTo" );

        m_transparencyTo = value;
        ChangeVisible();
      }
    }
    /// <summary>
    /// Returns or sets the degree of transparency of the specified fill as
    ///  a value from 0.0 (opaque) through 1.0 (clear).
    /// </summary>
    public virtual double TransparencyFrom
    {
      get
      {
        return m_transparencyFrom;
      }
      set
      {
        if( value < 0 || value > 1 )
          throw new ArgumentOutOfRangeException( "TransparencyFrom" );

        m_transparencyFrom = value;
        ChangeVisible();
      }
    }
    /// <summary>
    /// It Gets or Sets the TransparencyColor value
    /// </summary>
    public float TransparencyColor
    {
        get
        {
            return m_amt;
        }
        set
        {
            if ((value>=0)&&(1>=value))
                m_amt = value;
            else
                throw new ArgumentException("The specified value is out of range");
        }
    }
    /// <summary>
    /// Returns the transparency level of the specified Solid color shaded fill as a floating-point
    /// value from 0.0 (opaque) through 1.0(transparent)
    /// </summary>
    /// <value></value>
    public double Transparency
    {
      get
      {
        ValidateSolidType();
        return m_transparencyFrom;
      }
      set
      {
        ValidateSolidType();

        if (value < 0 || value > 1)
          throw new ArgumentOutOfRangeException("Transparency is out of range");

        m_transparencyFrom = value;
      }
    }
    /// <summary>
    /// Represents gradient style.
    /// </summary>
    public ExcelGradientColor GradientColorType
    {
      get
      {
        ValidateGradientType();

        return m_gradientColor;
      }
      set
      {
        ValidateGradientType();

        m_gradientColor = value;
        ChangeVisible();
      }
    }
    /// <summary>
    /// Represents gradient pattern
    /// </summary>
    public ExcelGradientPattern Pattern
    {
      get
      {
        ValidatePatternType();

        return m_gradPattern;
      }
      set
      {
        m_fillType = ExcelFillType.Pattern;
        m_gradPattern = value;
        ChangeVisible();
      }
    }
    /// <summary>
    /// Represents gradient texture
    /// </summary>
    public ExcelTexture Texture
    {
      get
      {
        ValidateTextureType();

        return m_gradTexture;
      }
      set
      {
        if( m_gradTexture == ExcelTexture.User_Defined )
          throw new ArgumentException( "This method support only preset textured" );

        m_fillType = ExcelFillType.Texture;
        m_gradTexture = value;
        ChangeVisible();
      }
    }
    /// <summary>
    /// Represents background color index.
    /// </summary>
    public ExcelKnownColors BackColorIndex
    {
      get
      {
        return BackColorObject.GetIndexed( m_book );
      }
      set
      {
        BackColorObject.SetIndexed( value );
      }
    }
    /// <summary>
    /// Represents foreground color index.
    /// </summary>
    public ExcelKnownColors ForeColorIndex
    {
      get
      {
        return ForeColorObject.GetIndexed( m_book );
      }
      set
      {
        ForeColorObject.SetIndexed( value );
      }
    }
    /// <summary>
    /// Represents background color.
    /// </summary>
    public virtual Color BackColor
    {
      get
      {
        return BackColorObject.GetRGB( m_book );
      }
      set
      {
        BackColorObject.SetRGB( value, m_book );
      }
    }
    /// <summary>
    /// Represents foreground color.
    /// </summary>
    public virtual Color ForeColor
    {
      get
      {
        return ForeColorObject.GetRGB( m_book );
      }
      set
      {
        ForeColorObject.SetRGB( value, m_book );
      }
    }
    /// <summary>
    /// Represents background color.
    /// </summary>
    public virtual ColorObject BackColorObject
    {
      get
      {
        return m_backColor;
        // Change event -         ChangeVisible();
      }
    }
    /// <summary>
    /// Represents foreground color.
    /// </summary>
    public virtual ColorObject ForeColorObject
    {
      get
      {
        return m_foreColor;
        // Change event -         ChangeVisible();
      }
    }
    /// <summary>
    /// Represents preset gradient type.
    /// </summary>
    public ExcelGradientPreset PresetGradientType
    {
      get
      {
        ValidateGradientType();

        if( m_gradientColor != ExcelGradientColor.Preset )
          throw new NotSupportedException( "This property supported only if checked preset color type." );

        return m_presetGrad;
      }
      set
      {
        ValidateGradientType();

        m_gradientColor = ExcelGradientColor.Preset;
        m_presetGrad = value;
        ChangeVisible();
      }
    }
    /// <summary>
    /// Represents user defined picture or texture. Read-only.
    /// </summary>
    public Image Picture
    {
      get
      {
        ValidatePictureProperties();

        return m_picture;
      }
    }

    /// <summary>
    /// Returns user defined picture of texture name. Read-only.
    /// </summary>
    public string PictureName
    {
      get
      {
        ValidatePictureProperties();

        return m_strPictureName;
      }
    }
    /// <summary>
    /// Represents if fill style visible.
    /// </summary>
    public virtual bool Visible
    {
      get
      {
        return m_bVisible;
      }
      set
      {
        if( Visible != value )
        {
          m_bVisible = value;
        }
        this.m_logger.SetFlag(PreservedFlag.Fill);
      }
    }
    /// <summary>
    /// Returns the gradient degree of the specified one-color shaded fill as a floating-point
    ///  value from 0.0 (dark) through 1.0 (light)
    /// </summary>
    public double GradientDegree
    {
      get
      {
        ValidateGradientType();

        if( m_gradientColor != ExcelGradientColor.OneColor )
          throw new NotSupportedException( "This property supports only if checked one color gradient" );

        return m_gradDegree;
      }
      set
      {
        ValidateGradientType();

        if( m_gradientColor != ExcelGradientColor.OneColor )
          throw new NotSupportedException( "This property supports only if checked one color gradient" );

        if( value < 0 || value > 1 )
          throw new ArgumentOutOfRangeException( "Gradient degree is out of range." );

        m_gradDegree = value;
        ChangeVisible();
      }
    }
    /// <summary>
    /// Gets or Sets the TextureVerticalScale for specified fill
    /// </summary>
    public float TextureVerticalScale
    {
        get
        {
            return m_textureVerticalScale;
        }
        set
        {
            if (value < 21475)        
            m_textureVerticalScale = value;
            else
                throw new ArgumentException("The specified value is out of range");
        }
    }
    /// <summary>
    /// Gets or Sets the TextureHorizontalScale for specified fill
    /// </summary>
    public float TextureHorizontalScale
    {
        get
        {
            return m_textureHorizontalScale;
        }
        set
        {
            if (value < 21475)        
            m_textureHorizontalScale = value;
            else
                throw new ArgumentException("The specified value is out of range");
        }
    }
    /// <summary>
    /// Gets or Sets the offset X for the specified fill
    /// </summary>
    public float TextureOffsetX
    {
        get
        {
            return m_textureOffsetX;
        }
        set
        {
            if (value < 169056)
            m_textureOffsetX = value;
            else
                throw new ArgumentException("The specified value is out of range");
        }
    }
    /// <summary>
    /// Gets or Sets the offset Y for the specified fill
    /// </summary>
    public float TextureOffsetY
    {
        get
        {
            return m_textureOffsetY;
        }
        set
        {
            if (value < 169056)
            m_textureOffsetY = value;
            else
                throw new ArgumentException("The specified value is out of range");
        }
    }
    /// <summary>
    /// Define Alignment value
    /// </summary>
    public string Alignment
    {
        get
        {
          return m_alignment;  
        }
        set
        {
            m_alignment = value;
        }
    }
    /// <summary>
    /// Define TileFlipping value
    /// </summary>
    public string TileFlipping
    {
        get
        {
            return m_tileFlipping;;
        }
        set
        {
            m_tileFlipping = value;
        }
    }
      
    #endregion

    #region IFill methods
#if !(WINRT )
    /// <summary>
    /// Sets user defined picture.
    /// </summary>
    /// <param name="path">Path to image.</param>
    public void UserPicture( string path )
    {
      if( path == null || path.Length == 0 )
        throw new ArgumentException( "Path canot be null or empty." );

      if( !File.Exists( path ) )
        throw new FileNotFoundException( "File represents by current path doesn't exist." );

      string strName = Path.GetFileNameWithoutExtension( path );

      UserPicture( Image.FromFile( path ), strName );
    }
#endif
    /// <summary>
    /// Sets user defined picture.
    /// </summary>
    ///<param name="im">Represents user defined image.</param>
    ///<param name="name">Represents name of user defined image.</param>
    public void UserPicture( Image im, string name )
    {
      if( name == null || name.Length == 0 )
        throw new ArgumentException( "name canot be null or empty." );

      if( im == null )
        throw new ArgumentNullException( "im" );

      m_fillType = ExcelFillType.Picture;
      m_picture = im;
      m_strPictureName = name;

      ChangeVisible();

      m_imageIndex = SetPictureToBse( im, name );
    }
#if !(WINRT )
    /// <summary>
    /// Sets user defined texture.
    /// </summary>
    /// <param name="path">Path to image.</param>
    public void UserTexture( string path )
    {
      if( path == null || path.Length == 0 )
        throw new ArgumentException( "path canot be null or empty." );

      if( !File.Exists( path ) )
        throw new FileNotFoundException( "File represents by current path doesn't exist." );

      string strName = Path.GetFileNameWithoutExtension( path );

      UserTexture( Image.FromFile( path ), strName );
    }
#endif
    /// <summary>
    /// Sets user defined texture.
    /// </summary>
    ///<param name="im">Represents user defined texture.</param>
    ///<param name="name">Represents name of user defined texture.</param>
    public void UserTexture( Image im, string name )
    {
      if( name == null || name.Length == 0 )
        throw new ArgumentException( "name canot be null or empty." );

      if( im == null )
        throw new ArgumentNullException( "im" );

      m_fillType = ExcelFillType.Texture;
      m_gradTexture = ExcelTexture.User_Defined;
      m_picture = im;
      m_strPictureName = name;

      ChangeVisible();

      m_imageIndex = SetPictureToBse( im, name );
    }

    /// <summary>
    /// Sets the specified fill to a pattern.
    /// </summary>
    /// <param name="pattern">Pattern to set.</param>
    public void Patterned( ExcelGradientPattern pattern )
    {
      Pattern = pattern;

      ChangeVisible();
    }
    /// <summary>
    /// Sets the specified fill to a preset gradient.
    /// </summary>
    /// <param name="grad">Represents preset gradient type.</param>
    public void PresetGradient( ExcelGradientPreset grad )
    {
      PresetGradient( grad, ExcelGradientStyle.Horizontal );
    }
    /// <summary>
    /// Sets the specified fill to a preset gradient.
    /// </summary>
    /// <param name="grad">Represents preset gradient type.</param>
    /// <param name="shadStyle">Represents gradient style, for preset gradient.</param>
    public void PresetGradient( ExcelGradientPreset grad, ExcelGradientStyle shadStyle )
    {
      PresetGradient( grad, shadStyle, ExcelGradientVariants.ShadingVariants_1 );
    }
    /// <summary>
    /// Sets the specified fill to a preset gradient.
    /// </summary>
    /// <param name="grad">Represents preset gradient type.</param>
    /// <param name="shadStyle">Represents gradient style, for preset gradient.</param>
    /// <param name="shadVar">Represents gradient variant for preset gradient.</param>
    public void PresetGradient( ExcelGradientPreset grad, ExcelGradientStyle shadStyle
      , ExcelGradientVariants shadVar )
    {
      if( shadStyle == ExcelGradientStyle.From_Center && ( int )shadVar > 2 )
        throw new ArgumentException( "From centr style support only var_1 or var_2" );

      m_fillType = ExcelFillType.Gradient;
      m_gradientColor = ExcelGradientColor.Preset;
      m_presetGrad = grad;
      m_gradStyle = shadStyle;
      m_gradVariant = shadVar;
      m_bSupportedGradient = true;

      ChangeVisible();
    }
    /// <summary>
    /// Sets the specified fill format to a preset texture.
    /// </summary>
    /// <param name="texture">Represents texture to set.</param>
    public void PresetTextured( ExcelTexture texture )
    {
      Texture = texture;

      ChangeVisible();
    }
    /// <summary>
    /// Sets the specified fill to a two-color gradient.
    /// </summary>
    public void TwoColorGradient()
    {
      TwoColorGradient( ExcelGradientStyle.Horizontal );
    }
    /// <summary>
    /// Sets the specified fill to a two-color gradient.
    /// </summary>
    /// <param name="style">Represents shading shading style.</param>
    public void TwoColorGradient( ExcelGradientStyle style )
    {
      TwoColorGradient( style, ExcelGradientVariants.ShadingVariants_1 );
    }
    /// <summary>
    /// Sets the specified fill to a two-color gradient.
    /// </summary>
    /// <param name="style">Represents shading shading style.</param>
    /// <param name="variant">Represents shading variant.</param>
    public void TwoColorGradient( ExcelGradientStyle style, ExcelGradientVariants variant )
    {
      if( style == ExcelGradientStyle.From_Center && ( int )variant > 2 )
        throw new ArgumentException( "From centr style support only var_1 or var_2" );

      m_fillType = ExcelFillType.Gradient;
      m_gradientColor = ExcelGradientColor.TwoColor;
      m_gradStyle = style;
      m_gradVariant = variant;
      m_bSupportedGradient = true;

      ChangeVisible();
    }
    /// <summary>
    /// Sets the specified fill to a one-color gradient.
    /// </summary>
    public void OneColorGradient()
    {
      OneColorGradient( ExcelGradientStyle.Horizontal );
    }
    /// <summary>
    /// Sets the specified fill to a one-color gradient.
    /// </summary>
    /// <param name="style">Represents shading shading style.</param>
    public void OneColorGradient( ExcelGradientStyle style )
    {
      OneColorGradient( style, ExcelGradientVariants.ShadingVariants_1 );
    }
    /// <summary>
    /// Sets the specified fill to a one-color gradient.
    /// </summary>
    /// <param name="style">Represents shading shading style.</param>
    /// <param name="variant">Represents shading variant.</param>
    public void OneColorGradient( ExcelGradientStyle style, ExcelGradientVariants variant )
    {
      if( style == ExcelGradientStyle.From_Center && ( int )variant > 2 )
        throw new ArgumentException( "From centr style support only var_1 or var_2" );

      m_fillType = ExcelFillType.Gradient;
      m_gradientColor = ExcelGradientColor.OneColor;
      m_gradStyle = style;
      m_gradVariant = variant;
      m_bSupportedGradient = true;

      ChangeVisible();
    }
    /// <summary>
    /// Sets the specified fill to a uniform color.
    /// </summary>
    public void Solid()
    {
      m_fillType = ExcelFillType.SolidColor;

      ChangeVisible();
    }
    #endregion

    #region IGradient methods
    /// <summary>
    /// Compares with shape fill impl.
    /// </summary>
    /// <param name="twin">Shape fill to compare with.</param>
    /// <returns>Zero if shape fills are equal.</returns>
    public int CompareTo( IGradient twin )
    {
      const byte btOne = 1;
      const byte btZero = 0;

      if( twin == null )
        return btOne;

      int result = ( m_gradStyle == twin.GradientStyle ) ? btZero : btOne;

      if( result != 0 ) return result;

      result = ( m_gradVariant == twin.GradientVariant ) ? btZero : btOne;

      if( result != 0 ) return result;

      result = ( m_backColor == twin.BackColorObject ) ? btZero : btOne;

      if( result != 0 ) return result;

      result = ( m_foreColor == twin.ForeColorObject ) ? btZero : btOne;

      if( result != 0 ) return result;

      return result;
    }
    #endregion

    #region Class parse methods
    /// <summary>
    /// Parses fill options.
    /// </summary>
    /// <param name="option">Record to parse.</param>
    /// <returns>Value indicating fill option.</returns>
    [ CLSCompliant( false ) ]
    public bool ParseOption( FOPTE option )
    {
      Color color;

      switch( option.Id )
      {
        case MsoOptions.FillType:
          ParseFillType( option.UInt32Value );
          return true;

        case MsoOptions.BackColor:
          /*color = */ParseColor( m_book, option.MainData, BackColorObject );
          //BackColorObject.SetRGB( color, m_book );
          m_gradDegree = ParseGradientDegree( option.MainData );
          return true;

        case MsoOptions.ForeColor:
          /*color = */ParseColor( m_book, option.MainData, ForeColorObject );
          //ForeColorObject.SetRGB( color, m_book );
          return true;

        case MsoOptions.ShadStyle:
          ParseShadingStyle( option.MainData );
          return true;

        case MsoOptions.ShadVariant:
          ParseShadingVariant( option.MainData[ 2 ] );
          return true;

        case MsoOptions.PattTextName:
          byte[] additionalData = option.AdditionalData;

          if( !option.IsValid && additionalData != null && additionalData.Length > 0 )
          {
            ParsePattTextName( additionalData );
          }
          return true;

        case MsoOptions.PatternTexture:
          if( m_fillType == ExcelFillType.Picture || m_fillType == ExcelFillType.Texture )
            m_parsePictureData = option;
          return true;

        case MsoOptions.GradientColorType:
          ParseGradientColor( option.UInt32Value );
          return true;

        case MsoOptions.PresetGradientData:
          ParsePresetGradient( option.AdditionalData );
          return true;

        case MsoOptions.ShadingStyleCorner_1:
          if( m_gradStyle == ExcelGradientStyle.From_Corner && option.MainData[ 4 ] == 1 )
            m_gradVariant = ExcelGradientVariants.ShadingVariants_2;
          return true;

        case MsoOptions.ShadingStyleCorner_2:
          ParseCornerVariants( option.MainData[ 4 ] );
          return true;

        case MsoOptions.NoFillHitTest:
          ParseVisible( option.MainData );
          return true;

        case MsoOptions.GradientTransparency:
          m_transparencyTo = ShapeLineFormatImpl.ParseTransparency( option.UInt32Value );
          return true;

        case MsoOptions.Transparency:
          m_transparencyFrom = ShapeLineFormatImpl.ParseTransparency( option.UInt32Value );
          return true;
      }

      return false;
    }
    /// <summary>
    /// Parses fill type.
    /// </summary>
    /// <param name="value">Represents fill type value.</param>
    private void ParseFillType( uint value )
    {
      m_fillType = ( ExcelFillType )value;

      if( value == DEF_CORNER_STYLE )
      {
        m_fillType = ExcelFillType.Gradient;
        m_gradStyle = ExcelGradientStyle.From_Corner;
        m_gradVariant = ExcelGradientVariants.ShadingVariants_1;
      }

      if( value == DEF_CENTER_STYLE )
      {
        m_fillType = ExcelFillType.Gradient;
        m_gradStyle = ExcelGradientStyle.From_Center;
      }
    }
    /// <summary>
    /// Parses shading style.
    /// </summary>
    /// <param name="arr">Represents option value byte array.</param>
    private void ParseShadingStyle( byte[] arr )
    {
      if( arr == null )
        throw new ArgumentNullException( "arr" );

      if( m_gradStyle == ExcelGradientStyle.From_Center || m_gradStyle
        == ExcelGradientStyle.From_Corner )
      {
        return;
      }

      byte bVal = arr[ 4 ];

      switch( bVal )
      {
        case DEF_SHAD_STYLE_VERTICAL:
        case DEF_SHAD_STYLE_VERTICAL2007:
          m_gradStyle = ExcelGradientStyle.Vertical;
          break;

        case DEF_SHAD_STYLE_DIAGONAL_UP:
          m_gradStyle = ExcelGradientStyle.Diagonl_Up;
          break;

        case DEF_SHAD_STYLE_DIAGONAL_DOWN:
          m_gradStyle = ExcelGradientStyle.Diagonl_Down;
          m_gradVariant = ExcelGradientVariants.ShadingVariants_1;
          break;
      }
    }
    /// <summary>
    /// Parse shading variant.
    /// </summary>
    /// <param name="value">Represents variant value.</param>
    private void ParseShadingVariant( byte value )
    {
      if( m_gradStyle == ExcelGradientStyle.From_Corner )
        return;

      if( m_gradStyle == ExcelGradientStyle.From_Center )
      {
        m_gradVariant = ( value == 100 )
          ? ExcelGradientVariants.ShadingVariants_1
          : ExcelGradientVariants.ShadingVariants_2;

        return;
      }

      switch( value )
      {
        case 100:
          m_gradVariant = ( m_gradStyle == ExcelGradientStyle.Diagonl_Down )
            ? ExcelGradientVariants.ShadingVariants_2
            : ExcelGradientVariants.ShadingVariants_1;
          break;

        case 50:
          m_gradVariant = ( m_gradStyle == ExcelGradientStyle.Horizontal )
            ? ExcelGradientVariants.ShadingVariants_3
            : ExcelGradientVariants.ShadingVariants_4;
          break;

        case 206:
          m_gradVariant = ( m_gradStyle == ExcelGradientStyle.Horizontal )
            ? ExcelGradientVariants.ShadingVariants_4
            : ExcelGradientVariants.ShadingVariants_3;
          break;

        default:
          m_gradVariant = ( m_gradStyle == ExcelGradientStyle.Diagonl_Down )
            ? ExcelGradientVariants.ShadingVariants_1
            : ExcelGradientVariants.ShadingVariants_2;
          break;
      }
    }
    /// <summary>
    /// Parses pattern or texture name.
    /// </summary>
    /// <param name="addData">Represents addition data, that contain name.</param>
    private void ParsePattTextName( byte[] addData )
    {
      bool bIsPattern = m_fillType == ExcelFillType.Pattern;
      bool bIsPicture = m_fillType == ExcelFillType.Picture;
      bool bIsTexture = m_fillType == ExcelFillType.Texture;

      if( !bIsPattern && !bIsTexture && !bIsPicture )
        return;

      if( addData == null )
        throw new ArgumentNullException( "addData" );

      string strName = "";

      for( int i = 0, iLen = addData.Length - 2; i < iLen; i = i + 2 )
      {
        strName += ( char )addData[ i ];
      }

      if( bIsPicture )
      {
        ParsePictureOrUserDefinedTexture( strName, true );

        return;
      }

      if( bIsTexture && strName[ 0 ] >= '0' && strName[ 0 ] <= '9' )
      {
        ParsePictureOrUserDefinedTexture( strName, false );

        return;
      }

      string updatedName = strName.Replace( ' ', '_' );

      if( bIsPattern )
      {
        updatedName = DEF_PATTERN_ENUM_PREFIX + updatedName;

        try
        {
          m_gradPattern = ( ExcelGradientPattern )Enum.Parse( typeof( ExcelGradientPattern )
            , updatedName, true );
        }
        catch
        {
          m_gradPattern = ExcelGradientPattern.Pat_5_Percent;
        }

        return;
      }

      try
      {
        m_gradTexture = ( ExcelTexture )Enum.Parse( typeof( ExcelTexture ), updatedName, true );
      }
      catch
      {
        ParsePictureOrUserDefinedTexture( strName, false );
      }
    }
    /// <summary>
    /// Parses gradient color.
    /// </summary>
    /// <param name="value">Represents gradient color value to parse.</param>
    private void ParseGradientColor( uint value )
    {
      if( value == 0 )
      {
        m_gradientColor = ExcelGradientColor.Preset;

        return;
      }

      if( value == DEF_ONE_COLOR_STYLE_VALUE )
      {
        m_gradientColor = ExcelGradientColor.OneColor;

        return;
      }

      m_gradientColor = ExcelGradientColor.TwoColor;
    }
    /// <summary>
    /// Parses preset gradient type.
    /// </summary>
    /// <param name="value">Represents data to parse.</param>
    private void ParsePresetGradient( byte[] value )
    {
      if( value == null || m_fillType != ExcelFillType.Gradient )
        return;

      int iType = 1;
      int iIndex = 0;
      int iValueCount = value.Length;
      bool bFounded = false;

      while( iType < DEF_OFFSET && !bFounded )
      {
        int iLen = m_arrPreset[ iIndex ];
        iIndex++;

        if( iLen == iValueCount )
        {
          for( int i = 0; i < iLen; i++ )
          {
            if( value[ i ] != m_arrPreset[ iIndex + i ] )
              break;

            if( iLen - i == 1 )
              bFounded = true;
          }
        }

        iIndex += iLen;
        iType++;
      }

      if( bFounded )
        m_presetGrad = ( ExcelGradientPreset )( iType - 1 );
    }
    /// <summary>
    /// Parses picture or user defined texture.
    /// </summary>
    /// <param name="strName">Represents name of picture.</param>
    /// <param name="bIsPicture">If true - parses picture otherwise user defined texture.</param>
    private void ParsePictureOrUserDefinedTexture( string strName, bool bIsPicture )
    {
      if( strName == null || strName.Length == 0 )
        throw new ArgumentException( "strName canot be null or empty." );

      m_strPictureName = strName;

      ParsePictureOrUserDefinedTexture( bIsPicture );
    }
    protected void ParsePictureOrUserDefinedTexture( bool bIsPicture )
    {
#if !SILVERLIGHT && !WINRT && !WP

      if( !bIsPicture )
        m_gradTexture = ExcelTexture.User_Defined;

      byte[] arr = m_parsePictureData.AdditionalData;

      if( arr == null || arr.Length == 0 )
      {
        m_imageIndex = ( int )m_parsePictureData.UInt32Value;
        MsofbtBSE picture = m_book.ShapesData.GetPicture( m_imageIndex );

        m_picture = picture.PictureRecord.Picture;
      }
      else
      {
        byte[] arrPicture = new byte[ arr.Length - DEF_OFFSET ];

        Array.Copy( arr, DEF_OFFSET, arrPicture, 0, arrPicture.Length );
        MemoryStream ms = new MemoryStream();

        UpdateBitMapHederToStream( ms, arr );
        ms.Write( arrPicture, 0, arrPicture.Length );

        m_picture = ApplicationImpl.CreateImage( ms );
      }

      m_parsePictureData = null;
#endif
    }
    /// <summary>
    /// Updates bitmap header to stream.
    /// </summary>
    /// <param name="ms">Represents memory stream.</param>
    /// <param name="arr">Represents bitmap data.</param>
    public static void UpdateBitMapHederToStream( MemoryStream ms, byte[] arr )
    {
      if( ms == null )
        throw new ArgumentNullException( "ms" );

      if( arr == null )
        throw new ArgumentNullException( "arr" );

      if( !BiffRecordRaw.CompareArrays( arr, 0, DEF_BITMAP_INDEX, 0, DEF_BITMAP_INDEX.Length ) )
      {
        return;
      }

      int iSize = arr.Length + MsoBitmapPicture.DEF_DIB_HEADER_SIZE - DEF_OFFSET;
      uint uiColCount = BitConverter.ToUInt32( arr, DEF_OFFSET );
      uint uiDib = BitConverter.ToUInt32( arr, MsoBitmapPicture.DEF_COLOR_USED_OFFSET + DEF_OFFSET );

      MsoBitmapPicture.AddBitMapHeaderToStream( ms, iSize, uiColCount, uiDib );
    }
    /// <summary>
    /// Parses visible property.
    /// </summary>
    /// <param name="data">Represents data value.</param>
    private void ParseVisible( byte[] data )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      byte value = ( byte )( data[ 2 ] & DEF_NOT_VISIBLE_VALUE );

      if( value > 0 )
      {
        m_bVisible = true;

        return;
      }

      value = ( byte )( data[ 4 ] & DEF_NOT_VISIBLE_VALUE );
      // Used fifth bit in third byte and first byte of data

      if( data[ 3 ] == 0 && data[ 5 ] == 0 && value > 0 )
      {
        m_bVisible = false;
      }
      else
      {
        m_bVisible = true;
      }
    }
    #endregion

    #region Class serialize methods
    /// <summary>
    /// Serialize fill properties.
    /// </summary>
    /// <param name="opt">Option holder.</param>
    /// <returns>Returns updated option holder.</returns>
    [ CLSCompliant( false ) ]
    public IFopteOptionWrapper Serialize( IFopteOptionWrapper opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      opt = SerializeFillType( opt );
      opt = SerializeTransparency( opt );
      opt = SerializeVisible( opt );

      ShapeLineFormatImpl.SerializeColor( opt, ForeColorObject, m_book, MsoOptions.ForeColor );
      ShapeLineFormatImpl.SerializeColor( opt, BackColorObject, m_book, MsoOptions.BackColor );

      switch( m_fillType )
      {
        case ExcelFillType.Gradient:
          return SerializeGradient( opt );

        case ExcelFillType.Texture:
        case ExcelFillType.Pattern:
          return SerializePatternTexture( opt );

        case ExcelFillType.Picture:
          return SerializePicture( opt );

        case ExcelFillType.SolidColor:
          return SerializeSolidColor( opt );

        case ExcelFillType.UnknownGradient:
          return opt; ;
      }

      throw new ApplicationException( "Unknown fill type" );
    }
    /// <summary>
    /// Serialize gradient as gradient style.
    /// </summary>
    /// <param name="opt">Option holder.</param>
    /// <returns>Returns updated option holder.</returns>
    private IFopteOptionWrapper SerializeGradient( IFopteOptionWrapper opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      opt = SerializeShadVariant( opt );
      opt = SerializeGradientStyle( opt );
      
      if( m_gradStyle != ExcelGradientStyle.Horizontal )
        opt = SerializeShadStyle( opt );

      if( m_gradientColor == ExcelGradientColor.Preset )
        opt = SerializeGradientPreset( opt );

      return opt;
    }
    /// <summary>
    /// Serialize gradient as pattern style.
    /// </summary>
    /// <param name="opt">Option holder.</param>
    /// <returns>Returns updated option holder.</returns>
    private IFopteOptionWrapper SerializePatternTexture( IFopteOptionWrapper opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      bool bisPattern = m_fillType == ExcelFillType.Pattern;

      if( !bisPattern && m_gradTexture == ExcelTexture.User_Defined )
        return SerializePicture( opt );


      int iIndex;
      string strName;
      string strStartResName;

      if( bisPattern )
      {
        iIndex = ( int )m_gradPattern;
        strName = m_gradPattern.ToString();
        strStartResName = DEF_PATTERN_PREFIX;
      }
      else
      {
        iIndex = ( int )m_gradTexture;
        strName = m_gradTexture.ToString();
        strStartResName = DEF_TEXTURE_PREFIX;
      }
#if ( WINRT )
        ResourceHandler resource = new ResourceHandler();
        byte[] addData;
        if(bisPattern)      
        addData = resource.PatternArray[strStartResName + iIndex.ToString()];
        else
        addData = resource.TextureArray[strStartResName + iIndex.ToString()];
#else
      byte[] addData = GetResData( strStartResName + iIndex.ToString() );
#endif
      ShapeImpl.SerializeForte( opt, MsoOptions.PatternTexture, 0, addData, true );

      if( bisPattern )
      {
        strName = strName.Substring( DEF_PATTERN_ENUM_PREFIX.Length );
        strName = strName.Replace( "_Percent", "%" );
      }

      strName = strName.Replace( '_', ' ' );
      byte[] add = ConvertNameToByteArray( strName );

      ShapeImpl.SerializeForte( opt, MsoOptions.PattTextName, 0, add, true );

      return opt;
    }
    /// <summary>
    /// Serialize gradient as picture style.
    /// </summary>
    /// <param name="opt">Option holder.</param>
    /// <returns>Returns updated option holder.</returns>
    private IFopteOptionWrapper SerializePicture( IFopteOptionWrapper opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      opt = SetPicture( opt );

      if( !string.IsNullOrEmpty( m_strPictureName ) )
      {
        byte[] result = ConvertNameToByteArray( m_strPictureName );
        ShapeImpl.SerializeForte( opt, MsoOptions.PattTextName, 0, result, true );
      }

      return opt;
    }
    /// <summary>
    /// Serialize gradient as solid color style.
    /// </summary>
    /// <param name="opt">Option holder.</param>
    /// <returns>Returns updated option holder.</returns>
    private IFopteOptionWrapper SerializeSolidColor( IFopteOptionWrapper opt )
    {
      return opt;
    }
    /// <summary>
    /// Serialize fill type.
    /// </summary>
    /// <param name="opt">Options holder.</param>
    /// <returns>Returns updated options holder.</returns>
    private IFopteOptionWrapper SerializeFillType( IFopteOptionWrapper opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      int iVal = ( int )m_fillType;

      if( m_fillType == ExcelFillType.Gradient && m_gradStyle == ExcelGradientStyle.From_Corner )
        iVal = DEF_CORNER_STYLE;

      if( m_fillType == ExcelFillType.Gradient && m_gradStyle == ExcelGradientStyle.From_Center )
        iVal = DEF_CENTER_STYLE;

      ShapeImpl.SerializeForte( opt, MsoOptions.FillType, iVal );

      return opt;
    }
    /// <summary>
    /// Serialize shading style.
    /// </summary>
    /// <param name="opt">Represents fopte option.</param>
    /// <returns>Serialized option.</returns>
    private IFopteOptionWrapper SerializeShadStyle( IFopteOptionWrapper opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      byte[] arr = { 0, 0, 0, 255 };

      switch( m_gradStyle )
      {
        case ExcelGradientStyle.Vertical:
          arr[ 2 ] = DEF_SHAD_STYLE_VERTICAL;
          break;

        case ExcelGradientStyle.Diagonl_Up:
          arr[ 2 ] = DEF_SHAD_STYLE_DIAGONAL_UP;
          break;

        case ExcelGradientStyle.Diagonl_Down:
          arr[ 2 ] = DEF_SHAD_STYLE_DIAGONAL_DOWN;
          break;

        default:
          return opt;
      }

      ShapeImpl.SerializeForte( opt, MsoOptions.ShadStyle, arr );

      return opt;
    }
    /// <summary>
    /// Serialize shading variant.
    /// </summary>
    /// <param name="opt">Options holder.</param>
    /// <returns>Returns updated options holder.</returns>
    private IFopteOptionWrapper SerializeShadVariant( IFopteOptionWrapper opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      if( m_gradStyle == ExcelGradientStyle.From_Corner )
        return SerializeShadVariantCorner( opt );

      if( m_gradStyle == ExcelGradientStyle.From_Center )
        return SerializeShadVariantCenter( opt );

      bool bFlag = false;

      if( ( int )m_gradVariant < 3 )
      {
        bFlag = ( m_gradStyle == ExcelGradientStyle.Diagonl_Down )
          ? !( m_gradVariant == ExcelGradientVariants.ShadingVariants_2 )
          : m_gradVariant == ExcelGradientVariants.ShadingVariants_2;

        if( !bFlag )
          ShapeImpl.SerializeForte( opt, MsoOptions.ShadVariant, DEF_VARIANT_FIRST_ARR );

        return opt;
      }

      bFlag = ( m_gradStyle == ExcelGradientStyle.Horizontal )
        ? !( m_gradVariant == ExcelGradientVariants.ShadingVariants_3 )
        : m_gradVariant == ExcelGradientVariants.ShadingVariants_3;

      if( bFlag )
      {
        ShapeImpl.SerializeForte( opt, MsoOptions.ShadVariant, DEF_VARIANT_THIRD_ARR );
      }
      else
      {
        ShapeImpl.SerializeForte( opt, MsoOptions.ShadVariant, DEF_VARIANT_FOURTH_ARR );
      }

      return opt;
    }
    /// <summary>
    /// Serialize center shad variants.
    /// </summary>
    /// <param name="opt">Represents options holder.</param>
    /// <returns>Returns updated option holder.</returns>
    private IFopteOptionWrapper SerializeShadVariantCenter( IFopteOptionWrapper opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      if( m_gradVariant == ExcelGradientVariants.ShadingVariants_1 )
        ShapeImpl.SerializeForte( opt, MsoOptions.ShadVariant, DEF_VARIANT_FIRST_ARR );

      ShapeImpl.SerializeForte( opt, MsoOptions.ShadingStyleCorner_1 , DEF_VARIANT_CENTER_ADD_DATA );
      ShapeImpl.SerializeForte( opt, MsoOptions.ShadingStyleCorner_2, DEF_VARIANT_CENTER_ADD_DATA );
      ShapeImpl.SerializeForte( opt, MsoOptions.ShadingStyleCorner_3, DEF_VARIANT_CENTER_ADD_DATA );
      ShapeImpl.SerializeForte( opt, MsoOptions.ShadingStyleCorner_4, DEF_VARIANT_CENTER_ADD_DATA );

      return opt;
    }
    /// <summary>
    /// Serialize corner shad variants.
    /// </summary>
    /// <param name="opt">Represents options holder.</param>
    /// <returns>Returns updated option holder.</returns>
    private IFopteOptionWrapper SerializeShadVariantCorner( IFopteOptionWrapper opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      ShapeImpl.SerializeForte( opt, MsoOptions.ShadVariant, DEF_VARIANT_FIRST_ARR );

      if( m_gradVariant == ExcelGradientVariants.ShadingVariants_1 )
        return opt;

      if( m_gradVariant == ExcelGradientVariants.ShadingVariants_2
        || m_gradVariant == ExcelGradientVariants.ShadingVariants_4 )
      {
        ShapeImpl.SerializeForte( opt, MsoOptions.ShadingStyleCorner_1, DEF_VARIANT_CORNER_ADD_DATA );
      }

      if( m_gradVariant == ExcelGradientVariants.ShadingVariants_3
        || m_gradVariant == ExcelGradientVariants.ShadingVariants_4 )
      {
        ShapeImpl.SerializeForte( opt, MsoOptions.ShadingStyleCorner_2, DEF_VARIANT_CORNER_ADD_DATA );
      }

      if( m_gradVariant == ExcelGradientVariants.ShadingVariants_2
        || m_gradVariant == ExcelGradientVariants.ShadingVariants_4 )
      {
        ShapeImpl.SerializeForte( opt, MsoOptions.ShadingStyleCorner_3, DEF_VARIANT_CORNER_ADD_DATA );
      }

      if( m_gradVariant == ExcelGradientVariants.ShadingVariants_3
        || m_gradVariant == ExcelGradientVariants.ShadingVariants_4 )
      {
        ShapeImpl.SerializeForte( opt, MsoOptions.ShadingStyleCorner_4, DEF_VARIANT_CORNER_ADD_DATA );
      }

      return opt;
    }
    /// <summary>
    /// Serialize gradient style.
    /// </summary>
    /// <param name="opt">Represents options holder.</param>
    /// <returns>Returns updated options holder.</returns>
    private IFopteOptionWrapper SerializeGradientStyle( IFopteOptionWrapper opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      if( m_gradientColor != ExcelGradientColor.OneColor )
        return opt;

      ShapeImpl.SerializeForte( opt, MsoOptions.GradientColorType, DEF_ONE_COLOR_STYLE_VALUE );
      SerializeGradientDegree( opt );

      return opt;
    }
    /// <summary>
    /// Serialize gradient preset type.
    /// </summary>
    /// <param name="opt">Represents option holder.</param>
    /// <returns>Returns updated option holder.</returns>
    private IFopteOptionWrapper SerializeGradientPreset( IFopteOptionWrapper opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      int iIndex = ( int )m_presetGrad;
#if ( WINRT )
        ResourceHandler resource = new ResourceHandler();
        byte[] arr = resource.PresetDictionary[DEF_GRAD_PREFIX + iIndex.ToString()];
        
#else        
      byte[] arr = GetResData( DEF_GRAD_PREFIX + iIndex.ToString() );     
              
#endif
        ShapeImpl.SerializeForte(opt, MsoOptions.PresetGradientData, 0, arr, true);
        ShapeImpl.SerializeForte(opt, MsoOptions.GradientColorType, 0);
      return opt;
    }
    /// <summary>
    /// Serialize visible.
    /// </summary>
    /// <param name="opt">Represents option holder.</param>
    /// <returns>Returns updated visible.</returns>
    private IFopteOptionWrapper SerializeVisible( IFopteOptionWrapper opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      //byte[] visibleData = { 0, 0, DEF_NOT_VISIBLE_VALUE, 0 };
      byte[] visibleData = { 0, 0, 31, 0 };

      if( m_bVisible )
          visibleData[0] = 28;

      ShapeImpl.SerializeForte( opt, MsoOptions.NoFillHitTest, visibleData );

      return opt;
    }
    /// <summary>
    /// Serialize gradient degree to biff stream
    /// </summary>
    /// <param name="opt">Represents option holder.</param>
    /// <returns>Returns updated option holder.</returns>
    private IFopteOptionWrapper SerializeGradientDegree( IFopteOptionWrapper opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      byte[] arr = { 240, 1, 0, DEF_COLOR_CONSTANT * 2 };
      double val = m_gradDegree;

      if( val >= 0.5 )
      {
        arr[ 1 ] = 2;
        val = 1 - val;
      }

      arr[ 2 ] = ( byte )( val * byte.MaxValue * 2 );

      ShapeImpl.SerializeForte( opt, MsoOptions.BackColor, arr );

      return opt;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Indicates if fill type is gradient. If not throw NotSupportedException.
    /// </summary>
    private void ValidateGradientType()
    {
      if( m_fillType != ExcelFillType.Gradient )
        throw new NotSupportedException( "This property can be set only when Gradient Style is selected." );
    }
    /// <summary>
    /// Indicates if fill type is user defined texture or picture. If not throw NotSupportedException.
    /// </summary>
    private void ValidatePictureProperties()
    {
      bool bFlag = m_fillType == ExcelFillType.Texture
        && m_gradTexture == ExcelTexture.User_Defined;

      if( m_fillType != ExcelFillType.Picture && !bFlag )
        throw new NotSupportedException( "This property support only if defined user texture of picture" );
    }
    /// <summary>
    /// Indicates if fill type is pattern. If not throw NotSupportedException.
    /// </summary>
    private void ValidatePatternType()
    {
      if( m_fillType != ExcelFillType.Pattern )
        throw new NotSupportedException( "This property suports only if chacked pattern style." );
    }
    /// <summary>
    /// Indicates if fill type is texture. If not throw NotSupportedException.
    /// </summary>
    private void ValidateTextureType()
    {
      if( m_fillType != ExcelFillType.Texture )
        throw new NotSupportedException( "This property suports only if chacked texture style." );
    }
    /// <summary>
    /// Validates the type of the solid.
    /// </summary>
    private void ValidateSolidType()
    {
      if( m_fillType != ExcelFillType.SolidColor )
        throw new NotSupportedException( "This property supports only if Checked Solid style." );
    }
    /// <summary>
    /// Convert name to byte array.
    /// </summary>
    /// <param name="strName">Represents name to convert.</param>
    /// <returns>Returns converted name in byte array.</returns>
    private byte[] ConvertNameToByteArray( string strName )
    {
      if( strName == null || strName.Length == 0 )
        throw new ArgumentException( "strName canot be null or empty." );

      int iLen = strName.Length;
      byte[] result = new byte[ iLen * 2 + 2 ];

      for( int i = 0; i < iLen; i++ )
      {
        char ch = strName[ i ];

        if( char.IsUpper( ch ) && i > 0 )
          ch = char.ToLower( ch );

        result[ 2 * i ] = ( byte )ch;
        result[ 2 * i + 1 ] = 0;
      }

      result[ 2 * iLen ] = 0;
      result[ 2 * iLen + 1 ] = 0;

      return result;
    }
    /// <summary>
    /// Parses gradient degree.
    /// </summary>
    /// <param name="value">Represents value to parse.</param>
    /// <returns>Returns parsed gradient degree.</returns>
    private double ParseGradientDegree( byte[] value )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      double degree = m_gradDegree;

      if( value[ 5 ] == DEF_COLOR_CONSTANT * 2 )
      {
        degree = value[ 4 ] * 0.5 / byte.MaxValue;

        if( value[ 3 ] == 2 )
          degree = 1 - degree;
      }

      return degree;
    }
    /// <summary>
    /// Parses corner variants.
    /// </summary>
    /// <param name="value">Represents value to parse.</param>
    private void ParseCornerVariants( byte value )
    {
      if( m_gradStyle != ExcelGradientStyle.From_Corner || value != 1 )
        return;

      m_gradVariant = ( m_gradVariant == ExcelGradientVariants.ShadingVariants_1 )
        ? ExcelGradientVariants.ShadingVariants_3
        : ExcelGradientVariants.ShadingVariants_4;
    }
    /// <summary>
    /// Fills internal collection with preset gradients default data.
    /// </summary>
    private static void FillPresetsGradientStops()
    {
      s_dicPresetStops = new Dictionary<ExcelGradientPreset, byte[]>();
      ExcelGradientPreset[] arrPresets = //( ExcelGradientPreset[] )Enum.GetValues( typeof( ExcelGradientPreset ) );
        new ExcelGradientPreset[]
      {
        ExcelGradientPreset.Grad_Early_Sunset,
        ExcelGradientPreset.Grad_Late_Sunset,
        ExcelGradientPreset.Grad_Nightfall,
        ExcelGradientPreset.Grad_Daybreak,
        ExcelGradientPreset.Grad_Horizon,
        ExcelGradientPreset.Grad_Desert,
        ExcelGradientPreset.Grad_Ocean,
        ExcelGradientPreset.Grad_Calm_Water,
        ExcelGradientPreset.Grad_Fire,
        ExcelGradientPreset.Grad_Fog,
        ExcelGradientPreset.Grad_Moss,
        ExcelGradientPreset.Grad_Peacock,
        ExcelGradientPreset.Grad_Wheat,
        ExcelGradientPreset.Grad_Parchment,
        ExcelGradientPreset.Grad_Mahogany,
        ExcelGradientPreset.Grad_Rainbow,
        ExcelGradientPreset.Grad_RainbowII,
        ExcelGradientPreset.Grad_Gold,
        ExcelGradientPreset.Grad_GoldII,
        ExcelGradientPreset.Grad_Brass,
        ExcelGradientPreset.Grad_Chrome,
        ExcelGradientPreset.Grad_ChromeII,
        ExcelGradientPreset.Grad_Silver,
        ExcelGradientPreset.Grad_Sapphire,
      };

      ResourceManager manager = new ResourceManager( "Syncfusion.XlsIO.PresetGradients", s_asem );

      for( int i = 0, len = arrPresets.Length; i < len; i++ )
      {
        ExcelGradientPreset preset = arrPresets[ i ];
#if ( WINRT )
          string value=manager.GetString(preset.ToString());
          byte[] arrData=System.Text.Encoding.UTF8.GetBytes(value);
#else
        byte[] arrData = ( byte[] )manager.GetObject( preset.ToString() );
#endif
        s_dicPresetStops.Add( preset, arrData );
      }
    }
    #endregion

    #region Class overrided methods
    /// <summary>
    /// Sets picture to option storage.
    /// </summary>
    /// <param name="opt">Represents option storage.</param>
    /// <returns>Returns updated option storage.</returns>
    [ CLSCompliant( false ) ]
    protected virtual IFopteOptionWrapper SetPicture( IFopteOptionWrapper opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      ShapeImpl.SerializeForte( opt, MsoOptions.PatternTexture, m_imageIndex, null, true );

      return opt;
    }
    /// <summary>
    /// Sets picture to bse collection.
    /// </summary>
    /// <param name="im">Image to set.</param>
    /// <param name="strName">Represent name of image.</param>
    /// <returns>Return index of image.</returns>
    protected virtual int SetPictureToBse( Image im, string strName )
    {
      if( im == null )
        throw new ArgumentNullException( "im" );

      WorkbookShapeDataImpl shapeData = m_book.ShapesData;

      if( m_imageIndex >= 0 )
      {
        MsofbtBSE bse = shapeData.GetPicture( m_imageIndex );

        if( bse != null && bse.RefCount <= 1 )
            shapeData.RemovePicture( ( uint )m_imageIndex-1, true );
      }

      return shapeData.AddPicture( im, ExcelImageFormat.Original, strName );
    }
    /// <summary>
    /// Serialize transparency to option holder.
    /// </summary>
    /// <param name="opt">Represents option holder.</param>
    /// <returns>Returns updated option holder.</returns>
    [ CLSCompliant( false ) ]
    protected virtual IFopteOptionWrapper SerializeTransparency( IFopteOptionWrapper opt )
    {
      ShapeLineFormatImpl.SerializeTransparency( opt, MsoOptions.GradientTransparency
        , m_transparencyTo );

      ShapeLineFormatImpl.SerializeTransparency( opt, MsoOptions.Transparency, m_transparencyFrom );

      return opt;
    }
    /// <summary>
    /// If need changes visible.
    /// </summary>
    protected virtual void ChangeVisible()
    {
      Visible = true;
    }
    #endregion

    #region Clone method
    /// <summary>
    /// Clones current object.
    /// </summary>
    /// <param name="parent">Represents parent object.</param>
    /// <returns>Returns cloned methods.</returns>
    public virtual ShapeFillImpl Clone( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      ShapeFillImpl result = ( ShapeFillImpl )MemberwiseClone();

      result.SetParent( parent );
      result.FindParents();

#if !SILVERLIGHT && !WINRT && !WP
      result.m_picture = ( Image )CloneUtils.CloneCloneable( m_picture );
#endif

      return result;
    }
    /// <summary>
    /// Copies filling settings from.
    /// </summary>
    /// <param name="fill">Object to copy settings from.</param>
    public void CopyFrom( ShapeFillImpl fill )
    {
      m_fillType = fill.m_fillType;
      m_gradStyle = fill.m_gradStyle;
      m_gradVariant = fill.m_gradVariant;
      m_transparencyTo = fill.m_transparencyTo;
      m_transparencyFrom = fill.m_transparencyFrom;
      m_gradientColor = fill.m_gradientColor;
      m_gradPattern = fill.m_gradPattern;
      m_gradTexture = fill.m_gradTexture;
      m_book = fill.m_book;
      m_backColor.CopyFrom( fill.m_backColor, false );
      m_foreColor.CopyFrom( fill.m_foreColor, false );

      m_presetGrad = fill.m_presetGrad;

#if !SILVERLIGHT && !WINRT && !WP
      m_picture = ( Image )fill.m_picture.Clone();
#endif

      m_strPictureName = fill.m_strPictureName;
      m_bVisible = fill.m_bVisible;
      m_imageIndex = fill.m_imageIndex;
      m_gradDegree = fill.m_gradDegree;
      m_parsePictureData = ( FOPTE )fill.m_parsePictureData.Clone();
      m_bIsShapeFill = fill.m_bIsShapeFill;
    }
    #endregion

    #region Excel2007 conversion methods
    /// <summary>
    /// Returns true if colors should be placed in inverted order.
    /// </summary>
    /// <param name="gradientStyle">Gradient style.</param>
    /// <param name="variant">Gradient variant.</param>
    /// <returns>True if colors should be placed in inverted order.</returns>
    public static bool IsInverted( ExcelGradientStyle gradientStyle, ExcelGradientVariants variant )
    {
      bool bResult = false;

      switch( gradientStyle )
      {
        case ExcelGradientStyle.Horizontal:
        case ExcelGradientStyle.Vertical:
        case ExcelGradientStyle.Diagonl_Up:
        case ExcelGradientStyle.From_Center:
          bResult = StandardInverted( variant );
          break;

        case ExcelGradientStyle.Diagonl_Down:
          bResult = DiagonalDownInverted( variant );
          break;

        case ExcelGradientStyle.From_Corner:
          bResult = false;
          break;
      }

      return bResult;
    }
    /// <summary>
    /// Evaluates whether colors should be placed in inverted order for DiagonalDown gradient style.
    /// </summary>
    /// <param name="variant">Gradient variant to check.</param>
    /// <returns>True if colors should be placed in inverted order.</returns>
    private static bool DiagonalDownInverted( ExcelGradientVariants variant )
    {
      bool bResult = ( variant == ExcelGradientVariants.ShadingVariants_1 || variant == ExcelGradientVariants.ShadingVariants_4 ) ?
        true :
        false;

      return bResult;
    }
    /// <summary>
    /// Evaluates whether colors should be placed in inverted order for the most
    /// of gradient styles (Horizontal, Vertical, DiagonalUp and FromCenter).
    /// </summary>
    /// <param name="variant">Represents gradient variant.</param>
    /// <returns>Value indicating whether colors placed in inverted order.</returns>
    private static bool StandardInverted( ExcelGradientVariants variant )
    {
      bool bResult = ( variant == ExcelGradientVariants.ShadingVariants_2 || variant == ExcelGradientVariants.ShadingVariants_4 ) ?
        true :
        false;

      return bResult;
    }
    /// <summary>
    /// Detects whether color sequence should be doubled or not.
    /// </summary>
    /// <param name="gradientStyle">Gradient style.</param>
    /// <param name="variant">Gradient variant.</param>
    /// <returns>True if color sequence should be doubled.</returns>
    public static bool IsDoubled( ExcelGradientStyle gradientStyle, ExcelGradientVariants variant )
    {
      bool bResult = false;

      switch( gradientStyle )
      {
        case ExcelGradientStyle.Horizontal:
        case ExcelGradientStyle.Vertical:
        case ExcelGradientStyle.Diagonl_Up:
        case ExcelGradientStyle.From_Center:
        case ExcelGradientStyle.Diagonl_Down:
          bResult = StandardDoubled( variant );
          break;

        case ExcelGradientStyle.From_Corner:
          bResult = false;
          break;
      }

      return bResult;
    }
    /// <summary>
    /// Detects whether color sequence should be doubled for the most of gradient
    /// styles (all except From_Corner).
    /// </summary>
    /// <param name="variant">Gradient variant.</param>
    /// <returns>True if color sequence should be doubled.</returns>
    private static bool StandardDoubled( ExcelGradientVariants variant )
    {
      bool bResult = ( variant == ExcelGradientVariants.ShadingVariants_3 || variant == ExcelGradientVariants.ShadingVariants_4 ) ?
        true :
        false;

      return bResult;
    }
    /// <summary>
    /// Evaluates gradient angle based on gradient style.
    /// </summary>
    /// <param name="gradientStyle">Gradient style.</param>
    /// <returns>Gradient angle, or -1 when gradient style is not linear gradient.</returns>
    private static int GradientAngle( ExcelGradientStyle gradientStyle )
    {
      int iResult = -1;

      switch( gradientStyle )
      {
        case ExcelGradientStyle.Horizontal:
          iResult = HorizontalAngle;
          break;

        case ExcelGradientStyle.Vertical:
          iResult = VerticalAngle;
          break;

        case ExcelGradientStyle.Diagonl_Up:
          iResult = DiagonalUpAngle;
          break;

        case ExcelGradientStyle.Diagonl_Down:
          iResult = DiagonalDownAngle;
          break;
      }

      return iResult;
    }
    /// <summary>
    /// Evaluates fillToRect value for gradient.
    /// </summary>
    /// <param name="gradientStyle">Gradient style.</param>
    /// <param name="variant">Gradient variant.</param>
    /// <returns>Value of the fillToRect rectangle or Rectangle.Empty if gradient style doesn't need it.</returns>
    private static Rectangle GradientFillToRect( ExcelGradientStyle gradientStyle, ExcelGradientVariants variant )
    {
      Rectangle result = Rectangle.Empty;

      if( gradientStyle == ExcelGradientStyle.From_Corner )
      {
        result = RectanglesCorner[ ( int )variant ];
      }
      else if( gradientStyle == ExcelGradientStyle.From_Center )
      {
        result = RectangleFromCenter;
      }

      return result;
    }
    /// <summary>
    /// Returns gradient type value based on gradient style.
    /// </summary>
    /// <param name="gradStyle">Gradient style.</param>
    /// <returns>Gradient type value that correspond to the specified gradient style.</returns>
    private static GradientType GetGradientType( ExcelGradientStyle gradStyle )
    {
      switch( gradStyle )
      {
        case ExcelGradientStyle.Horizontal:
        case ExcelGradientStyle.Vertical:
        case ExcelGradientStyle.Diagonl_Down:
        case ExcelGradientStyle.Diagonl_Up:
          return GradientType.Liniar;

        case ExcelGradientStyle.From_Corner:
        case ExcelGradientStyle.From_Center:
          return GradientType.Rect;

        default:
          throw new ArgumentOutOfRangeException();
      }
    }
    #endregion

    internal void Clear()
    {
        
        if (m_backColor != null)
            m_backColor.Dispose();
        if (m_foreColor != null)
            m_foreColor.Dispose();
        if (m_picture != null)
            m_picture.Dispose();
        if (m_preseredGradient != null)
            m_preseredGradient.Dispose();
        
        m_backColor = null;
        m_foreColor = null;
        m_picture = null;
        m_parsePictureData = null;
    }
  }
}
