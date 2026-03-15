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
using System.Text;
using System.Collections;
using System.Diagnostics;

using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;

using Syncfusion.XlsIO.Interfaces;

using FOPTE = Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing.MsofbtOPT.FOPTE;
using System.IO;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.XmlSerialization;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.Shapes
{
  /// <summary>
  /// Class used for Shape Implementation.
  /// </summary>
  public class ShapeImpl
    : CommonObject
    , IShape
    , IParentApplication
    , IDisposable
    , ICloneParent
    , INamedObject
  {
    #region Class constants
    /// <summary>
    /// Default foreground color.
    /// </summary>
    protected static readonly Color DEF_FORE_COLOR = ColorExtension.White;
    /// <summary>
    /// Default background color.
    /// </summary>
    protected static readonly Color DEF_BACK_COLOR = ColorExtension.White;
    /// <summary>
    /// Represents alpha constant, that represents index in ExcelKnownColors structures.
    /// </summary>
    private const int DEF_APLPHA_KNOWN_COLORS = 8;
    /// <summary>
    /// Value of SizeTextToFitShape option when it is set to false.
    /// </summary>
    protected const int DEF_SIZETEXTTOFITSHAPE_FALSE_VALUE = 0x080008;
    /// <summary>
    /// Value of SizeTextToFitShape option when it is set to true.
    /// </summary>
    protected const int DEF_SIZETEXTTOFITSHAPE_TRUE_VALUE  = 0x0A000A;
    /// <summary>
    /// Value of NoFillHitTest option.
    /// </summary>
    protected const int DEF_NOFILLHITTEST_VALUE = 1048592;
    /// <summary>
    /// Offset for full column.
    /// </summary>
    public const int DEF_FULL_COLUMN_OFFSET = 1024;
    /// <summary>
    /// Offset for full row.
    /// </summary>
    public const int DEF_FULL_ROW_OFFSET = 256;
    /// <summary>
    /// Represents default line weight.
    /// </summary>
    private const int DEF_LINE_WEIGHT = 9525;
    /// <summary>
    /// Represents default transparency mull.
    /// </summary>
    public const double DEF_TRANSPARENCY_MULL = 655.0;
    /// <summary>
    /// Represents default transparency mull multiplied by 100.
    /// </summary>
    public const double DEF_TRANSPARENCY_MULL_100 = DEF_TRANSPARENCY_MULL * 100;
    /// <summary>
    /// Represents default line weight mull.
    /// </summary>
    internal const double LineWieghtMultiplier = 12700.0;
    /// <summary>
    /// Represents default parent types.
    /// </summary>
    private static readonly Type[] DEF_PARENT_TYPES = new Type[]
    {
      typeof( ShapesCollection ),
      typeof( WorkbookImpl )
    };
    /// <summary>
    /// Fill options.
    /// </summary>
    private static readonly MsoOptions[] FillOptions = new MsoOptions[]
    {
      MsoOptions.FillType,
      MsoOptions.ForeColor,
      MsoOptions.BackColor,
      MsoOptions.NoFillHitTest
    };

    private static readonly MsoOptions[] LineOptions = new MsoOptions[]
    {
        MsoOptions.NoLineDrawDash,
        MsoOptions.LineStyle,
        MsoOptions.LineWeight,
        MsoOptions.LineDashStyle,
        MsoOptions.ContainRoundDot,
        MsoOptions.LineTransparency,
        MsoOptions.LineColor,
        MsoOptions.LineBackColor,
        MsoOptions.ContainLinePattern,
        MsoOptions.LinePattern,
        MsoOptions.LineStartArrow,
        MsoOptions.LineEndArrow,
        MsoOptions.StartArrowLength,
        MsoOptions.EndArrowLength,
        MsoOptions.StartArrowWidth,
        MsoOptions.EndArrowWidth,
     };
    #endregion

    #region Class members
    /// <summary>
    /// Indicates if this shape support shape color-line options.
    /// </summary>
    protected bool m_bSupportOptions;
    /// <summary>
    /// Name of the shape.
    /// </summary>
    private string              m_strName = string.Empty;
    /// <summary>
    /// Alternative text.
    /// </summary>
    private string              m_strAlternativeText = string.Empty;
    /// <summary>
    /// Shape's record.
    /// </summary>
    private MsoBase             m_record;
    /// <summary>
    /// Parent workbook for the shape.
    /// </summary>
    private WorkbookImpl        m_book;
    /// <summary>
    /// Type of the shape.
    /// </summary>
    private ExcelShapeType      m_shapeType;
    /// <summary>
    /// Shape record.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected MsofbtSp            m_shape;
    /// <summary>
    /// Client anchor record.
    /// </summary>
    private MsofbtClientAnchor  m_clientAnchor;
    /// <summary>
    /// Parent shapes collection.
    /// </summary>
    protected ShapeCollectionBase  m_shapes;
    /// <summary>
    /// OBJ record.
    /// </summary>
    private OBJRecord           m_object;
    /// <summary>
    /// Shape's options.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected MsofbtOPT         m_options;
    /// <summary>
    /// Absolute coordinates of the shape in pixels.
    /// </summary>
    private Rectangle           m_rectAbsolute = new Rectangle();
    /// <summary>
    /// Represents fill properties.
    /// </summary>
    private ShapeFillImpl       m_fill;
    /// <summary>
    /// Represents shape line format.
    /// </summary>
    private ShapeLineFormatImpl     m_lineFormat;
    /// <summary>
    /// Indicates is parse or serialize Line fill properties.
    /// </summary>
    protected bool m_bUpdateLineFill = true;
    /// <summary>
    /// This object contains not parsed xml data.
    /// </summary>
    private Stream m_xmlDataStream;
    /// <summary>
    /// This object contains not parsed type stream data.
    /// </summary>
    private Stream m_xmlTypeStream;
    /// <summary>
    /// Preserved relation to the image.
    /// </summary>
    private Relation m_imageRelation;
    /// <summary>
    /// Preserved relation id.
    /// </summary>
    private string m_strImageRelationId;
    /// <summary>
    /// Indicates whether we have to update absolute positions after setting TopRow,
    /// BottomRow, LeftColumn, RightColumn.
    /// </summary>
    private bool m_bUpdatePositions = true;
    /// <summary>
    /// Indicates whether shape is vml shape or not.
    /// </summary>
    private bool m_bVmlShape;
    /// <summary>
    /// Id of this shape.
    /// </summary>
    private int m_iShapeId;
    /// <summary>
    /// Tokens containing reference to the associated macro.
    /// </summary>
    private Ptg[] m_macroTokens;
    /// <summary>
    /// Visibility of the shape.
    /// </summary>
    private bool m_shapeVisibility = true;
    /// <summary>
    /// Represents Shadow 
    /// </summary>
    private ShadowImpl m_shadow;
    /// <summary>
    /// Represents the 3D features
    /// </summary>
    private ThreeDFormatImpl m_3D;

    private bool m_enableAlternateContent = false;
      /// <summary>
      /// Represens the Child Shapes in the Group.
      /// </summary>
    private List<ShapeImpl> m_childShapes;
    /// <summary>
    /// Child Client anchor record.
    /// </summary>
    private MsofbtChildAnchor m_childAnchor;
      /// <summary>
      /// Preserves the Style properties.
      /// </summary>
    private Dictionary<string,string> m_styleProperties;
    /// <summary>
    /// Preserves the Style properties as string value.
    /// </summary>
    private string m_preserveStyleString;
    private bool m_isHyperlink;
    private int m_shapeRotation = 0;
    /// <summary>
    /// Indicates the whether the shape has the borders.
    /// </summary>
    private bool m_bHasBorder;
    internal List<Stream> preservedShapeStreams;
    internal List<Stream> preservedCnxnShapeStreams;
    internal List<Stream> preservedInnerCnxnShapeStreams;
    internal List<Stream> preservedPictureStreams;
    /// <summary>
    /// Stream to preserve the Slicers graphic frame
    /// </summary>
    internal Stream m_graphicFrame;
    /// <summary>
    /// Anchor type
    /// </summary>
    private bool m_bIsAbsoluteAnchor;
    #endregion

    #region Class static methods
    /// <summary>
    /// Serialize FOPTE structure.
    /// </summary>
    /// <param name="options">Parent collection.</param>
    /// <param name="id">Structure id to serialize.</param>
    /// <param name="arr">Array of byte - main byte of structure.</param>
    [CLSCompliant( false )]
    public static void SerializeForte( IFopteOptionWrapper options, MsoOptions id, byte[] arr )
    {
      SerializeForte( options, id, arr, null, false );
    }
    /// <summary>
    /// Serialize FOPTE structure.
    /// </summary>
    /// <param name="options">Parent collection.</param>
    /// <param name="id">Structure id to serialize.</param>
    /// <param name="arr">Array of byte - main byte of structure.</param>
    /// <param name="addData">Represents additional data.</param>
    /// <param name="isValid">Represents if valid</param>
    [CLSCompliant( false )]
    public static void SerializeForte( IFopteOptionWrapper options, MsoOptions id, byte[] arr, byte[] addData, bool isValid )
    {
      if( arr == null )
        throw new ArgumentNullException( "arr" );

      SerializeForte( options, id, BitConverter.ToInt32( arr, 0 ), addData, isValid );
    }
    /// <summary>
    /// Serialize FOPTE structure.
    /// </summary>
    /// <param name="options">Parent collection.</param>
    /// <param name="id">Structure id to serialize.</param>
    /// <param name="value">Represents UInt value of structure.</param>
    [CLSCompliant( false )]
    public static void SerializeForte( IFopteOptionWrapper options, MsoOptions id, int value )
    {
      SerializeForte( options, id, value, null, false );
    }
    /// <summary>
    /// Serialize FOPTE structure.
    /// </summary>
    /// <param name="options">Parent collection.</param>
    /// <param name="id">Structure id to serialize.</param>
    /// <param name="value">Represents int value of structure.</param>
    /// <param name="addData">Represents additional data.</param>
    /// <param name="isValid">Represents if valid</param>
    [CLSCompliant( false )]
    public static void SerializeForte( IFopteOptionWrapper options, MsoOptions id, int value, byte[] addData, bool isValid )
    {
      if( options == null )
        throw new ArgumentNullException( "options" );

      FOPTE option = new FOPTE();
      option.Id = id;
      option.UInt32Value = ( uint )value;
      option.IsValid = isValid;
      option.IsComplex = false;

      if( addData != null )
      {
        option.IsComplex = true;
        option.AdditionalData = addData;
        option.UInt32Value = ( uint )addData.Length;
      }

      options.AddOptionSorted( option );
    }
    /// <summary>
    /// Indicates whether text frame should be autosized.
    /// </summary>
    private bool m_bAutoSize;
    internal bool IsEquationShape;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance of the shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    public ShapeImpl( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
      AttachEvents();
      m_bHasBorder = true;
      if( m_shapes.Worksheet == null )
        m_bUpdatePositions = false;

      m_clientAnchor = ( MsofbtClientAnchor )MsoFactory.GetRecord(
        MsoRecords.msofbtClientAnchor );//new MsofbtClientAnchor( null );
    }
    /// <summary>
    /// Initializes new instance of the shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    /// <param name="instance">Current object to clone.</param>
    public ShapeImpl( IApplication application, object parent, ShapeImpl instance )
      : this( application, parent )
    {
      m_bIsDisposed = instance.m_bIsDisposed;
      m_bSupportOptions = instance.m_bSupportOptions;
      m_rectAbsolute = instance.m_rectAbsolute;
      m_shapeType = instance.m_shapeType;
      m_strAlternativeText = instance.m_strAlternativeText;
      m_strName = instance.m_strName;

      MsoBase mso = instance.m_record;

      if( mso != null )
      {
        m_record = ( MsoBase )CloneUtils.CloneMsoBase( mso, null );
      }
      else
      {
        if( m_shape != null )
          m_shape = ( MsofbtSp )CloneUtils.CloneMsoBase( instance.m_shape, null );
      }

      UpdateRecord( instance.m_clientAnchor );
      
      m_object = ( OBJRecord )CloneUtils.CloneCloneable( instance.m_object );
    }
    /// <summary>
    /// Initializes new instance of the shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    /// <param name="records">Array of records that contains the record for new shape.</param>
    /// <param name="index">Index of the records for new shape.</param>
    [ CLSCompliant( false ) ]
    public ShapeImpl( IApplication application, object parent, MsoBase[] records, int index )
      : this( application, parent )
    {
      m_record = ( MsofbtSpContainer ) records[ index ];
      ParseRecord();
    }
    /// <summary>
    /// Initializes new instance of the shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    /// <param name="container">Shape container record that describes the new shape.</param>
    [ CLSCompliant( false ) ]
    public ShapeImpl( IApplication application, object parent, MsofbtSpContainer container )
      : this( application, parent, container, ExcelParseOptions.Default )
    {
    }
    /// <summary>
    /// Initializes new instance of the shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    /// <param name="container">Shape container record that describes the new shape.</param>
    /// <param name="options">Parse options.</param>
    [ CLSCompliant( false ) ]
    public ShapeImpl( IApplication application, object parent, MsofbtSpContainer container
      , ExcelParseOptions options )
      : this( application, parent )
    {
      m_record = container;

      ParseRecord( options );
      m_bSupportOptions = true;
    }
    /// <summary>
    /// Initializes new instance of the shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    /// <param name="shapeRecord">Record that describes the new shape.</param>
    [ CLSCompliant( false ) ]
    public ShapeImpl( IApplication application, object parent, MsoBase shapeRecord )
      : this( application, parent, shapeRecord, ExcelParseOptions.Default )
    {
    }
    /// <summary>
    /// Initializes new instance of the shape.
    /// </summary>
    /// <param name="application">Application object for the new shape.</param>
    /// <param name="parent">Parent object for the new shape.</param>
    /// <param name="shapeRecord">Record that describes the new shape.</param>
    /// <param name="options">Parse options.</param>
    [ CLSCompliant( false ) ]
    public ShapeImpl( IApplication application, object parent, MsoBase shapeRecord
      , ExcelParseOptions options )
      : this( application, parent )
    {
      m_record = shapeRecord;
    }
    #endregion

    #region Class Parse methods
    /// <summary>
    /// Creates default fill and line formatting options.
    /// </summary>
    protected virtual void CreateDefaultFillLineFormats()
    {
    }
    /// <summary>
    /// Parses shape container record.
    /// </summary>
    private void ParseRecord()
    {
      ParseRecord( ExcelParseOptions.Default );
    }
    /// <summary>
    /// Parses shape container record.
    /// </summary>
    /// <param name="options">Parse options.</param>
    private void ParseRecord( ExcelParseOptions options )
    {
      m_shapeType = ExcelShapeType.Unknown;
      MsofbtSpContainer container = m_record as MsofbtSpContainer;
      CreateDefaultFillLineFormats();

      if( container == null )
      {
        //throw new ArgumentNullException( "This is not a Shape container." );
        Debug.WriteLine( "This is not a Shape container.", "Warning" );
        return;
      }

      List<MsoBase> items = container.ItemsList;

      for( int i = 0, len = items.Count; i < len; i++ )
      {
        // Use 'as' to increase performance.
        MsoBase subRecord = items[ i ];

        switch( subRecord.MsoRecordType )
        {
          case MsoRecords.msofbtOPT:
            if( m_bUpdateLineFill )
            {
              ParseOptions( ( MsofbtOPT )subRecord );
            }
            else
            {
              MsofbtOPT opt = subRecord as MsofbtOPT;
              m_options = opt;
              ExtractNecessaryOptions( opt );
            }
            break;

          case MsoRecords.msofbtSp:
            ParseShape( ( MsofbtSp )subRecord );
            break;

          case MsoRecords.msofbtClientAnchor:
            ParseClientAnchor( ( MsofbtClientAnchor ) subRecord );
            break;

          case MsoRecords.msofbtSpgr:
            ParseShapeGroup( ( MsofbtSpgr )subRecord );
            break;

          case MsoRecords.msofbtSpgrContainer:
            ParseShapeGroupContainer( ( MsofbtSpgrContainer )subRecord );
            break;

          case MsoRecords.msofbtChildAnchor:
            ParseChildAnchor( ( MsofbtChildAnchor )subRecord );
            break;

          case MsoRecords.msofbtClientData:
            ParseClientData( ( MsofbtClientData )subRecord, options );
            break;

          default:
            // TODO: this is unknown shape record just save it
            ParseOtherRecords( subRecord, options );
            break;
        }
      }

      if( Id != 0 )
        m_iShapeId = Id;
    }

    /// <summary>
    /// Parses client data record.
    /// </summary>
    /// <param name="clientData">Record to parse.</param>
    /// <param name="options">Parse options.</param>
    [ CLSCompliant( false ) ]
    protected virtual void ParseClientData( MsofbtClientData clientData, ExcelParseOptions options )
    {
      m_object = clientData.ObjectRecord;

      List<ObjSubRecord> records = m_object.RecordsList;

      m_book.CurrentObjectId = Math.Max( m_book.CurrentObjectId
        , ( records[ 0 ] as ftCmo ).ID );

      for( int i = 1, len = records.Count; i < len; i++ )
      {
        ObjSubRecord subRecord = records[ i ];

        if( subRecord.Type == TObjSubRecordType.ftMacro )
        {
          ftMacro macro = ( ftMacro )subRecord;
          m_macroTokens = macro.Tokens;
          break;
        }
      }
    }

    /// <summary>
    /// Parses all unknown records (should be overridden in child classes).
    /// </summary>
    /// <param name="subRecord">Record to parse.</param>
    /// <param name="options">Parse options.</param>
    [CLSCompliant( false )]
    protected virtual void ParseOtherRecords( MsoBase subRecord, ExcelParseOptions options )
    {
    }

    /// <summary>
    /// Parses options.
    /// </summary>
    /// <param name="options">Options to parse.</param>
    private void ParseOptions( MsofbtOPT options )
    {
      if( options == null )
        throw new ArgumentNullException( "options" );

      m_options = options;
      FOPTE[] properties = options.Properties;

      for( int i = 0, len = properties.Length; i < len; i++ )
      {
        ParseOption( properties[ i ] );
      }
    }

    /// <summary>
    /// Parses fill options.
    /// </summary>
    /// <param name="option">Represents single fill option</param>
    /// <returns>Returns true if parsed correctly.</returns>
    private bool ParseFill( FOPTE option )
    {
      if( m_fill == null && Array.IndexOf<MsoOptions>( FillOptions, option.Id ) >= 0 )
        m_fill = new ShapeFillImpl( Application, this );

      if( m_fill == null )
        return false;

      return m_fill.ParseOption( option );
    }
    private bool ParseLineFormat( FOPTE option )
    {
      if( m_lineFormat == null && Array.IndexOf<MsoOptions>( LineOptions, option.Id ) >= 0 )
        m_lineFormat = new ShapeLineFormatImpl( Application, this );

      if( m_lineFormat == null )
        return false;

      return m_lineFormat.ParseOption( option );
    }
    /// <summary>
    /// Parses option record.
    /// </summary>
    /// <param name="option">Record to parse.</param>
    /// <returns>Value indicating fill option.</returns>
    [ CLSCompliant( false ) ]
    protected virtual bool ParseOption( FOPTE option )
    {
      if( ParseFill( option ) )
        return true;

      if( ParseLineFormat( option ) )//m_lineFormat.ParseOption( option ) )
        return true;

      switch( option.Id )
      {
        case MsoOptions.ShapeName:
          m_strName = ParseName( option );
          return true;

        case MsoOptions.SizeTextToFitShape:
          m_bAutoSize = option.UInt32Value == DEF_SIZETEXTTOFITSHAPE_TRUE_VALUE;
          return true;

        case MsoOptions.AlternativeText:
          m_strAlternativeText = ParseName( option );
          break;
      }

      return false;
    }
    /// <summary>
    /// Parses shape record.
    /// </summary>
    /// <param name="shapeRecord">Record to parse.</param>
    [ CLSCompliant( false ) ]
    protected virtual void ParseShape( MsofbtSp shapeRecord )
    {
      if( shapeRecord == null )
        throw new ArgumentNullException( "shapeRecord" );

      m_shape = ( MsofbtSp )shapeRecord.Clone();
    }

    /// <summary>
    /// Parses client anchor record.
    /// </summary>
    /// <param name="clientAnchor">Record to parse.</param>
    [ CLSCompliant( false ) ]
    public virtual void ParseClientAnchor( MsofbtClientAnchor clientAnchor )
    {
      // TODO: for the moment we don't know how to decode info about 
      // position that is store in client anchor record. 
      // We must set m_rectShape in this method.
      if( clientAnchor == null )
        throw new ArgumentNullException( "clientAnchor" );

      m_clientAnchor = ( MsofbtClientAnchor )clientAnchor;

      if( !m_clientAnchor.IsShortVersion )
      {
        EvaluateTopLeftPosition();
        UpdateHeight();
        UpdateWidth();
      }
    }

    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    protected virtual void SetParents()
    {
      m_shapes = FindParent( typeof( ShapeCollectionBase ), true ) as ShapeCollectionBase;

      if( m_shapes == null )
        throw new ArgumentNullException( "Can't find parent collection." );

      m_book = m_shapes.Workbook;

      //      m_shapes.Worksheet.ColumnWidthChanged +=
      //        new ValueChangedEventHandler( Worksheet_ColumnWidthChanged );
    }
    /// <summary>
    /// Raises events.
    /// </summary>
    protected void AttachEvents()
    {
      WorksheetImpl sheet = m_shapes.WorksheetBase as WorksheetImpl;

      if( sheet != null )
      {
        ( m_book.Styles[ RangeImpl.DEF_DEFAULT_STYLE ].Font as FontWrapper ).AfterChangeEvent
          //+= new ValueChangedEventHandler( NormalFont_OnAfterChange );
          += new EventHandler( NormalFont_OnAfterChange );

        sheet.ColumnWidthChanged +=
          new ValueChangedEventHandler( Worksheet_ColumnWidthChanged );

        sheet.RowHeightChanged +=
          new ValueChangedEventHandler( Worksheet_RowHeightChanged );
      }
    }
    /// <summary>
    /// Suppress events.
    /// </summary>
    protected void DetachEvents()
    {
      WorksheetImpl sheet = m_shapes.WorksheetBase as WorksheetImpl;

      if( sheet != null )
      {
          if (m_book.Styles != null)
          {
              if (m_book.Styles.Contains("Normal"))
              {
                  (m_book.Styles["Normal"].Font as FontWrapper).AfterChangeEvent
                      //-= new ValueChangedEventHandler( NormalFont_OnAfterChange );
                    -= new EventHandler(NormalFont_OnAfterChange);
              }
          }
        sheet.ColumnWidthChanged -=
          new ValueChangedEventHandler( Worksheet_ColumnWidthChanged );

        sheet.RowHeightChanged -=
          new ValueChangedEventHandler( Worksheet_RowHeightChanged );
      }
    }
    /// <summary>
    /// Parses shape group record.
    /// </summary>
    /// <param name="shapeGroup">Record to parse.</param>
    [ CLSCompliant( false ) ]
    protected virtual void ParseShapeGroup( MsofbtSpgr shapeGroup )
    {
      // TODO: Parse record that describes current group.
    }

    /// <summary>
    /// Parses shape group container.
    /// </summary>
    /// <param name="subRecord">Container to parse.</param>
    [ CLSCompliant( false ) ]
    protected virtual void ParseShapeGroupContainer( MsofbtSpgrContainer subRecord )
    {
      // TODO: some shapes were grouped - parse them.
    }

    /// <summary>
    /// Parses child anchor record.
    /// </summary>
    /// <param name="childAnchor">Record to parse.</param>
    [ CLSCompliant( false ) ]
    protected virtual void ParseChildAnchor( MsofbtChildAnchor childAnchor )
    {
      if( childAnchor == null )
        throw new ArgumentNullException( "childAnchor" );

       m_childAnchor = childAnchor;
    }

    /// <summary>
    /// Converts option value to color.
    /// </summary>
    /// <param name="option">Option to convert.</param>
    /// <returns>Corresponding color value.</returns>
    [ CLSCompliant( false ) ]
    protected Color GetColorValue( FOPTE option )
    {
      byte[] buffer = BitConverter.GetBytes( option.UInt32Value );

      if( buffer[ 3 ] == DEF_APLPHA_KNOWN_COLORS )
      {
        ExcelKnownColors color = ( ExcelKnownColors )buffer[ 0 ];

        return m_book.GetPaletteColor( color );
      }

      return Color.FromArgb( 0, buffer[ 0 ], buffer[ 1 ], buffer[ 2 ] );
    }
    /// <summary>
    /// Gets single byte by index for forte structure.
    /// </summary>
    /// <param name="option">Represents current FORTE structure.</param>
    /// <param name="iByteIndex">Byte index.</param>
    /// <returns>Returns single byte by index.</returns>
    private byte GetByte( FOPTE option, int iByteIndex )
    {
      byte[] buffer = BitConverter.GetBytes( option.UInt32Value );

      return buffer[ iByteIndex ];
    }
    /// <summary>
    /// Extracts shape name from option.
    /// </summary>
    /// <param name="option">Option that contains shape name.</param>
    /// <returns>Name extracted.</returns>
    [ CLSCompliant( false ) ]
    protected string ParseName( FOPTE option )
    {
      if( option == null )
        throw new ArgumentNullException( "option" );

      byte[] nameData = option.AdditionalData;
      string result = null;

      if( nameData == null )
      {
        result = null;
      }
      else
      {
        result = Encoding.Unicode.GetString( nameData, 0, nameData.Length );
        
        // Remove last zero character.
        result = result.Substring( 0, result.Length - 1 );
      }

      return result;
    }
    /// <summary>
    /// Extracts all necessary option.
    /// </summary>
    /// <param name="options">Represents option holder.</param>
    private void ExtractNecessaryOptions( MsofbtOPT options )
    {
      if( options == null )
        throw new ArgumentNullException( "options" );

      IList<FOPTE> list = options.PropertyList;

      for( int i = 0, iLen = list.Count; i < iLen; i++ )
      {
        FOPTE option = list[ i ];
        ExtractNecessaryOption( option );
      }
    }
    /// <summary>
    /// Extract necessary option.
    /// </summary>
    /// <param name="option">Option to extract.</param>
    /// <returns>Value indicating extracted option.</returns>
    [ CLSCompliant( false ) ]
    protected virtual bool ExtractNecessaryOption( FOPTE option )
    {
      if( option == null )
        throw new ArgumentNullException( "option" );

      switch( option.Id )
      {
        case MsoOptions.ShapeName:
          m_strName = ParseName( option );
          return true;

        case MsoOptions.SizeTextToFitShape:
          m_bAutoSize = option.UInt32Value == DEF_SIZETEXTTOFITSHAPE_TRUE_VALUE;
          return true;

        case MsoOptions.AlternativeText:
          m_strAlternativeText = ParseName( option );
          return true;
      }

      return false;
    }
    #endregion

    #region IShape Members
    /// <summary>
    /// Gets or sets the height of the shape.
    /// </summary>
    public virtual int    Height
    {
      get
      {
        return m_rectAbsolute.Height;
      }
      set
      {
        if( value < 0 )
          throw new ArgumentOutOfRangeException( "Height" );

        if( m_rectAbsolute.Height != value )
        {
          m_rectAbsolute.Height = value;

          UpdateBottomRow();
        }
      }
    }
    /// <summary>
    /// Gets Shape ID.
    /// </summary>
    public virtual int Id
    {
      get
      {
        MsofbtSpContainer container = m_record as MsofbtSpContainer;
        if( container != null )
        {
          MsofbtSp sp = container.ItemsList[ 0 ]as MsofbtSp;
          if( sp != null ) return ( int )sp.ShapeId;
        }
        // TODO:  Add ShapeImpl.Id getter implementation
        return 0;
      }
    }
    /// <summary>
    /// Gets the chart3 D properties.
    /// </summary>
    /// <value>The chart3 D properties.</value>
    public IThreeDFormat ThreeD
    {
        get
        {
            if (m_3D == null)
                m_3D = new ThreeDFormatImpl(Application, this);

            return m_3D;
        }
    }
    /// <summary>
    /// Gets the shadow properties.
    /// </summary>
    /// <value>The shadow properties.</value>
    public IShadow Shadow
    {
        get
        {

            if (m_shadow == null)
                m_shadow = new ShadowImpl(Application, this);
            //if (m_shadow.HasCustomShadowStyle == true)
            //    throw new NotSupportedException("It is not supported when Custom Shadow style is set to true");

            return m_shadow;
        }
    }
    /// <summary>
    /// Gets or set x coordinate of upper left corner of shape.
    /// </summary>
    public virtual int Left
    {
      get
      {
        return m_rectAbsolute.X;
      }
      set
      {
        if( m_rectAbsolute.X != value )
        {
          m_rectAbsolute.X = value;

          UpdateLeftColumn();
          UpdateRightColumn();
        }
      }
    }
    internal bool EnableAlternateContent
    {
        get
        {
            return m_enableAlternateContent;
        }
        set
        {
            m_enableAlternateContent = value;
        }
    }
    /// <summary>
    /// Gets or sets name of the shape.
    /// </summary>
    public virtual string Name
    {
      get
      {
        return m_strName;
      }
      set
      {
        m_strName = value;
      }
    }
    /// <summary>
    /// Gets or sets the Y coordinate of the shape
    /// </summary>
    public virtual int    Top
    {
      get
      {
        return m_rectAbsolute.Y;
      }
      set
      {
        if( m_rectAbsolute.Y != value )
        {
          m_rectAbsolute.Y = value;
          
          UpdateTopRow();
          UpdateBottomRow();
        }
      }
    }
    /// <summary>
    /// Gets or sets width of the shape.
    /// </summary>
    public virtual int    Width
    {
      get
      {
        return m_rectAbsolute.Width;
      }
      set
      {
        if( value < 0 )
          throw new ArgumentOutOfRangeException( "Width" );

        if( m_rectAbsolute.Width != value )
        {
          m_rectAbsolute.Width = value;
          
          UpdateRightColumn();
        }
      }
    }
    /// <summary>
    /// Gets or sets shape type
    /// </summary>
    public ExcelShapeType ShapeType
    {
      get
      {
        return m_shapeType;
      }
      set
      {
        m_shapeType = value;
      }
    }
    /// <summary>
    /// Gets or set a value indicating whether shape is visible
    /// </summary>
    public bool   IsShapeVisible
    {
      get
      {
        // TODO:  Add ShapeImpl.Visible getter implementation
        return m_shapeVisibility;
      }
      set
      {
        m_shapeVisibility = value;// TODO:  Add ShapeImpl.Visible setter implementation
      }
    }
    /// <summary>
    /// Gets or sets alternative text of the shape
    /// </summary>
    public virtual string AlternativeText
    {
      get
      {
        return m_strAlternativeText;
      }
      set
      {
        m_strAlternativeText = value;
      }
    }
    /// <summary>
    /// Gets or set a value indicating whether shape can move with cell
    /// </summary>
    public virtual bool   IsMoveWithCell
    {
      get
      {
        //return false;
        return ClientAnchor.IsMoveWithCell;
      }
      set
      {
        ClientAnchor.IsMoveWithCell = value;

        //        if( value )
        //        {
        //          WorksheetImpl sheet = Worksheet as WorksheetImpl;
        //
        //          if( sheet != null )
        //          {
        //            // Evaluate relative position of the TopLeft corner.
        //            m_pointTopLeft.X = ConvertWidthOffsetIntoPixels( 
        //              sheet.GetColumnWidthInPixels( LeftColumn )
        //              , LeftColumnOffset, false );
        //
        //            m_pointTopLeft.Y = ConvertHeightOffsetIntoPixels( 
        //              sheet.GetRowHeightInPixels( TopRow )
        //              , TopRowOffset, false );
        //          }
        //        }
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether shape can size with cell.
    /// </summary>
    public virtual bool  IsSizeWithCell
    {
      get
      {
        //return false;
        return ClientAnchor.IsSizeWithCell;
      }
      set
      {
        ClientAnchor.IsSizeWithCell = value;

        //        if( value )
        //        {
        //          WorksheetImpl sheet = Worksheet as WorksheetImpl;
        //
        //          if( sheet != null )
        //          {
        //            // Evaluate relative position of the TopLeft corner.
        //            m_pointBottomRight.X = ConvertWidthOffsetIntoPixels( 
        //              sheet.GetColumnWidthInPixels( RightColumn )
        //              , RightColumnOffset, false );
        //
        //            m_pointBottomRight.Y = ConvertHeightOffsetIntoPixels( 
        //              sheet.GetRowHeightInPixels( BottomRow )
        //              , BottomRowOffset, false );
        //          }
        //        }
      }
    }
    /// <summary>
    /// Represents fill properties. Read-only.
    /// </summary>
    public virtual IFill Fill
    {
      get
      {
        if( !m_bSupportOptions )
          throw new NotSupportedException( "This shape doesn't support fill properties." );

        if( !m_bUpdateLineFill )
        {
          ParseLineFill( m_options );
        }
        else if( m_fill == null )
        {
          m_fill = new ShapeFillImpl( Application, this );
        }

        return m_fill;
      }
    }
    /// <summary>
    /// Represents line format properties. Read-only.
    /// </summary>
    public virtual IShapeLineFormat Line
    {
      get
      {
        if( !m_bSupportOptions )
          throw new NotSupportedException( "This shape doesn't support line properties." );

        if( !m_bUpdateLineFill )
        {
          ParseLineFill( m_options );
        }
        else if( m_lineFormat == null )
        {
          m_lineFormat = new ShapeLineFormatImpl( Application, this );
        }

        return m_lineFormat;
      }
    }
    /// <summary>
    /// True if the size of the specified object is changed automatically
    /// to fit text within its boundaries. Read/write Boolean.
    /// </summary>
    public bool AutoSize
    {
      get
      {
        return m_bAutoSize;
      }
      set
      {
        m_bAutoSize = value;
      }
    }
    /// <summary>
    /// Gets / sets xml data stream.
    /// </summary>
    public Stream XmlDataStream
    {
      get
      {
        return m_xmlDataStream;
      }
      set
      {
        m_xmlDataStream = value;
      }
    }
    /// <summary>
    /// Gets or sets xml type stream.
    /// </summary>
    public Stream XmlTypeStream
    {
      get
      {
        return m_xmlTypeStream;
      }
      set
      {
        m_xmlTypeStream = value;
      }
    }
    /// <summary>
    /// Indicates whether this is vml shape or not.
    /// </summary>
    public bool VmlShape
    {
      get
      {
        return m_bVmlShape;
      }
      set
      {
        m_bVmlShape = value;
      }
    }
    /// <summary>
    /// Gets or sets macro-command that is called when action happens.
    /// </summary>
    public string OnAction
    {
      get
      {
        return ( m_macroTokens != null ) ? 
          m_book.FormulaUtil.ParsePtgArray( m_macroTokens ) :
          null;
      }
      set
      {
        m_macroTokens = ( value != null ) ?
          m_book.FormulaUtil.ParseString( value ) :
          null;
      }
    }
    public string ImageRelationId
    {
      get
      {
        return m_strImageRelationId;
      }
      set
      {
        m_strImageRelationId = value;
      }
    }
    public Relation ImageRelation
    {
      get
      {
        return m_imageRelation;
      }
      set
      {
        m_imageRelation = value;
      }
    }
    /// <summary>
    /// Returns or sets the rotation of the shape, in degrees.
    /// </summary>
    /// <value></value>
    public virtual int ShapeRotation
    {
        get
        {
            return m_shapeRotation;
        }
        set
        {
            if (value > 3600 && value < -3600)
                throw new ArgumentException("The rotation value should be between -3600 and 3600");
            m_shapeRotation = value;
        }
    }
    /// <summary>
    /// Returns a TextFrame object that contains the 
    /// alignment and anchoring properties for the specified shape. Read-only.
    /// </summary>
    public virtual ITextFrame TextFrame
    {
        get
        {
            throw new NotImplementedException("This property doesn't support in this class");
        }
    }
    #endregion

    #region IShape methods
    /// <summary>
    /// Removes this shape from the collection.
    /// </summary>
    public void Remove()
    {
      OnDelete();
      m_shapes.Remove( this );
    }
    /// <summary>
    /// Scales the shape.
    /// </summary>
    /// <param name="scaleWidth">Width scale in percent.</param>
    /// <param name="scaleHeight">Height scale in percent.</param>
    public void Scale( int scaleWidth, int scaleHeight )
    {
      if( scaleWidth < 0 )
        throw new ArgumentOutOfRangeException( "scaleWidth" );

      if( scaleHeight < 0 )
        throw new ArgumentOutOfRangeException( "scaleHeight" );

      Width = ( int )( Width * scaleWidth / 100.0 );
      Height = ( int )( Height * scaleHeight / 100.0 );
    }
    #endregion

    #region IDisposable Members
    /// <summary>
    /// Detaches events and disposes current object.
    /// </summary>
    protected override void OnDispose()
    {
      base.OnDispose();
      DetachEvents();
    }

    #endregion

    #region Class Serialization methods
    /// <summary>
    /// Serializes shape into shape group container.
    /// </summary>
    /// <param name="spgrContainer">Shape group container that will receive shape data.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( MsofbtSpgrContainer spgrContainer )
    {
        if(ChildShapes.Count>0)
            SerializeShape(spgrContainer,true);
        else
            SerializeShape( spgrContainer );
    }

    /// <summary>
    /// Serializes shape into shape group container.
    /// </summary>
    /// <param name="spgrContainer">Shape group container that will receive shape data.</param>
    [CLSCompliant(false)]
    public void Serialize(MsofbtSpgrContainer spgrContainer,bool isGroupShape)
    {
        SerializeShape(spgrContainer,isGroupShape);
    }
    /// <summary>
    /// Serializes shape record (MsofbtSp) into shape group container.
    /// </summary>
    /// <param name="spgrContainer">Shape group container that will receive shape data.</param>
    [CLSCompliant(false)]
    protected virtual void SerializeShape(MsofbtSpgrContainer spgrContainer)
    {
        if (m_record != null)
            spgrContainer.AddItem(m_record);
    }
    /// <summary>
    /// Serializes shape record (MsofbtSp) into shape group container.
    /// </summary>
    /// <param name="spgrContainer">Shape group container that will receive shape data.</param>
    [CLSCompliant(false)]
    protected virtual void SerializeShape(MsofbtSpgrContainer spgrContainer,bool isGroupShape)
    {
        List<ShapeImpl> childs = ChildShapes;
        if (childs.Count > 0)
        {
            Worksheet.TypedOptionButtons.PrepareForSerialization();
            MsofbtSpgrContainer groupContainer = (MsofbtSpgrContainer)MsoFactory.GetRecord(
       MsoRecords.msofbtSpgrContainer);
            spgrContainer.AddItem(groupContainer);
            foreach (ShapeImpl child in childs)
                child.Serialize(groupContainer,true);
        }
        else
        {
            if (m_record != null)
                spgrContainer.AddItem(m_record);
        }
    }
    /// <summary>
    /// Serializes mso options.
    /// </summary>
    /// <param name="container">Represents mso container.</param>
    private void SerializeMsoOptions( MsofbtSpContainer container )
    {
      if( container == null )
        throw new ArgumentNullException( "container" );

      if( m_options == null )
        m_options = CreateDefaultOptions();

      if( m_shapeType != ExcelShapeType.Unknown )
      {
        if( m_bUpdateLineFill )
          m_options = SerializeMsoOptions( m_options );

        List<MsoBase> arr = container.ItemsList;

        for( int i = 0, iLen = arr.Count; i < iLen; i++ )
        {
          if( arr[ i ] is MsofbtOPT )
          {
            arr[ i ] = m_options;
            break;
          }
        }
      }
    }
    /// <summary>
    /// Serialize mso options.
    /// </summary>
    /// <param name="opt">Represents option record.</param>
    /// <returns>Returns option record, initialized by option values.</returns>
    [ CLSCompliant( false ) ]
    protected MsofbtOPT SerializeMsoOptions( MsofbtOPT opt )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      if( m_fill != null )
      {
        UpdateFillFopte( opt );
        opt = ( MsofbtOPT )m_fill.Serialize( opt );
      }

      if( m_bSupportOptions && m_lineFormat != null )
        m_lineFormat.Serialize( opt );

      SerializeCommentShadow( opt );

      return opt;
    }
    /// <summary>
    /// Serialize transparency option.
    /// </summary>
    /// <param name="opt">Represents option storage.</param>
    /// <param name="value">Transparency value.</param>
    private void SerializeTransparency( MsofbtOPT opt, int value )
    {
      if( opt == null )
        throw new ArgumentNullException( "opt" );

      int iResult = 100 - value;

      SerializeForte( opt, MsoOptions.Transparency, ( int )( iResult * DEF_TRANSPARENCY_MULL ) );
    }
    /// <summary>
    /// Serializes shape's options.
    /// </summary>
    /// <param name="parent">Parent record for options record.</param>
    /// <returns>Options record.</returns>
    [ CLSCompliant( false ) ]
    protected virtual MsofbtOPT SerializeOptions( MsoBase parent )
    {
      if( m_options == null )
        m_options = CreateDefaultOptions();

      return m_options;
    }

    /// <summary>
    /// Serializes SizeTextToFitShape option.
    /// </summary>
    /// <param name="options">MsofbtOPT record to which text ID will be added.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If options argument is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    protected void SerializeSizeTextToFit( MsofbtOPT options )
    {
      int iValue = m_bAutoSize
        ? DEF_SIZETEXTTOFITSHAPE_TRUE_VALUE
        : DEF_SIZETEXTTOFITSHAPE_FALSE_VALUE;

      SerializeOptionSorted( options, MsoOptions.SizeTextToFitShape, ( uint )iValue );
    }
    /// <summary>
    /// Serializes HitTest option.
    /// </summary>
    /// <param name="options">MsofbtOPT record to which text ID will be added.</param>
    /// <exception cref="System.ArgumentNullException">
    /// If options argument is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    protected void SerializeHitTest( MsofbtOPT options )
    {
      SerializeOptionSorted( options, MsoOptions.NoFillHitTest, DEF_NOFILLHITTEST_VALUE );
    }

    /// <summary>
    /// Serialize shape's options.
    /// </summary>
    /// <param name="options">Options record.</param>
    /// <param name="id">Option ID.</param>
    /// <param name="value">Option value.</param>
    [ CLSCompliant( false ) ]
    protected void SerializeOption( MsofbtOPT options, MsoOptions id, uint value )
    {
      if( options == null )
        throw new ArgumentNullException( "options" );

      FOPTE option = new FOPTE();
      option.Id = id;
      option.UInt32Value = value;
      option.IsValid = false;
      option.IsComplex = false;
      options.AddOptionsOrReplace( option );
    }

    /// <summary>
    /// Serialize shape's options.
    /// </summary>
    /// <param name="options">Options record.</param>
    /// <param name="id">Option ID.</param>
    /// <param name="value">Option value.</param>
    /// <returns>Record with option.</returns>
    [ CLSCompliant( false ) ]
    protected FOPTE SerializeOption( MsofbtOPT options, MsoOptions id, int value )
    {
      if( options == null )
        throw new ArgumentNullException( "options" );

      FOPTE option = new FOPTE();
      option.Id = id;
      option.Int32Value = value;
      option.IsValid = false;
      option.IsComplex = false;
      options.AddOptionsOrReplace( option );
      return option;
    }

    /// <summary>
    /// Serialize shape's options sorted by option id.
    /// </summary>
    /// <param name="options">Options record.</param>
    /// <param name="id">Option ID.</param>
    /// <param name="value">Option value.</param>
    [ CLSCompliant( false ) ]
    protected void SerializeOptionSorted( MsofbtOPT options, MsoOptions id, uint value )
    {
      if( options == null )
        throw new ArgumentNullException( "options" );

      FOPTE option = new FOPTE();
      option.Id = id;
      option.UInt32Value = value;
      option.IsValid = false;
      option.IsComplex = false;
      options.AddOptionSorted( option );
    }

    /// <summary>
    /// Serializes shape name.
    /// </summary>
    /// <param name="options">Option holder.</param>
    [CLSCompliant( false )]
    protected void SerializeShapeName( MsofbtOPT options )
    {
      SerializeName( options, MsoOptions.ShapeName, m_strName );
    }
    /// <summary>
    /// Serializes shape name.
    /// </summary>
    /// <param name="options">Options object to store name in.</param>
    /// <param name="optionId">Option id to store name in.</param>
    /// <param name="name">Name to serialize.</param>
    [ CLSCompliant( false ) ]
    protected void SerializeName( MsofbtOPT options, MsoOptions optionId, string name )
    {
      if( name == null || name.Length == 0 )
        return;

      if( options == null )
        throw new ArgumentNullException( "options" );

      int iCount = name.Length;
      string strName = name;

      if( name[ iCount - 1 ] != '\0' )
      {
        strName += '\0';
      }

      byte[] data = Encoding.Unicode.GetBytes( strName );

      FOPTE option = new FOPTE();
      option.Id = optionId;
      option.UInt32Value = ( uint )data.Length;
      option.IsValid = true;
      option.IsComplex = true;
      option.AdditionalData = data;

      //options.AddOptionsOrReplace( option );
      options.AddOptionSorted( option );
    }
    /// <summary>
    /// Create default shape options.
    /// </summary>
    /// <returns>Record with option.</returns>
    [ CLSCompliant( false ) ]
    protected virtual MsofbtOPT CreateDefaultOptions()
    {
      MsofbtOPT result = ( MsofbtOPT )MsoFactory.GetRecord(
        MsoRecords.msofbtOPT );//new MsofbtOPT( m_shape );

      return result;
    }
    /// <summary>
    /// Sets all fill fopte structure to default value.
    /// </summary>
    /// <param name="option">Represents option holder.</param>
    private void UpdateFillFopte( MsofbtOPT option )
    {
      if( option == null )
        throw new ArgumentNullException( "opt" );

      for( int i = ChartGelFrameRecord.DEF_START_MSO_INDEX; i <= ChartGelFrameRecord.DEF_LAST_MSO_INDEX; i++ )
      {
        option.RemoveOption( i );
      }
    }
    /// <summary>
    /// Serialize comment shadow.
    /// </summary>
    /// <param name="option">Represents option holder.</param>
    [ CLSCompliant( false ) ]
    protected virtual void SerializeCommentShadow( MsofbtOPT option )
    {
    }
    #endregion

    #region Implementation properties

    /// <summary>
    ///  Stream to preserve the Slicers graphic frame
    /// </summary>
    internal Stream GraphicFrameStream
    {
        get
        {
            return m_graphicFrame;
        }
        set
        {
            m_graphicFrame = value;
        }
    }
    /// <summary>
    /// Indicates the whether the shape has the borders.
    /// </summary>
    internal bool HasBorder
    {
        get
        {
            return m_bHasBorder;
        }
        set
        {
            m_bHasBorder = value;
        }
    }
    /// <summary>
    /// Parent workbook. Read-only.
    /// </summary>
    public IWorkbook Workbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl ParentWorkbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Parent shapes collection. Read-only.
    /// </summary>
    public ShapeCollectionBase ParentShapes
    {
      get
      {
        return m_shapes;
      }
    }
    /// <summary>
    /// Parent worksheet. Read-only.
    /// </summary>
    public WorksheetBaseImpl Worksheet
    {
      get
      {
        return m_shapes.WorksheetBase;
      }
    }

    /// <summary>
    /// OBJ record. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public OBJRecord Obj
    {
      get
      {
        return m_object;
      }
    }
    /// <summary>
    /// Client anchor. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public MsofbtClientAnchor ClientAnchor
    {
      get
      {
        return m_clientAnchor;
      }
    }
    /// <summary>
    /// Top row for the shape.
    /// </summary>
    public virtual int TopRow
    {
      get
      {
        return ClientAnchor.TopRow + 1;
      }
      set
      {
        ClientAnchor.TopRow = value - 1;

        if( m_bUpdatePositions )
          OnTopRowChanged();
      }
    }
    /// <summary>
    /// Left column for the shape.
    /// </summary>
    public virtual int LeftColumn
    {
      get
      {
        return ClientAnchor.LeftColumn + 1;
      }
      set
      {
        ClientAnchor.LeftColumn = value - 1;

        if( m_bUpdatePositions )
          OnLeftColumnChange();
      }
    }
    /// <summary>
    /// Bottom row for the shape.
    /// </summary>
    public virtual int BottomRow
    {
      get
      {
        return ClientAnchor.BottomRow + 1;
      }
      set
      {
        ClientAnchor.BottomRow = value - 1;

        if( m_bUpdatePositions )
          UpdateHeight();
      }
    }
    /// <summary>
    /// Right column for the shape.
    /// </summary>
    public virtual int RightColumn
    {
      get
      {
        return ClientAnchor.RightColumn + 1;
      }
      set
      {
        ClientAnchor.RightColumn = value - 1;

        if( m_bUpdatePositions )
          UpdateWidth();
      }
    }
    
    /// <summary>
    /// Top row offset for the shape.
    /// </summary>
    public virtual int TopRowOffset
    {
      get
      {
        return ClientAnchor.TopOffset;
      }
      set
      {
        if( value < 0 )
          throw new ArgumentOutOfRangeException( "TopRowOffset" );

        ClientAnchor.TopOffset = value;
        OnTopRowChanged();
      }
    }
    /// <summary>
    /// Left column offset for the shape.
    /// </summary>
    public virtual int LeftColumnOffset
    {
      get
      {
        return ClientAnchor.LeftOffset;
      }
      set
      {
        if( value < 0 )
          throw new ArgumentOutOfRangeException( "LeftColumnOffset" );

        ClientAnchor.LeftOffset = value;
        OnLeftColumnChange();
      }
    }
    /// <summary>
    /// Bottom row offset for the shape.
    /// </summary>
    public virtual int BottomRowOffset
    {
      get
      {
        return ClientAnchor.BottomOffset;
      }
      set
      {
        if( value < 0 )
          throw new ArgumentOutOfRangeException( "BottomRowOffset" );

        ClientAnchor.BottomOffset = value;
        UpdateHeight();
      }
    }
    /// <summary>
    /// Right column offset for the shape.
    /// </summary>
    public virtual int RightColumnOffset
    {
      get
      {
        return ClientAnchor.RightOffset;
      }
      set
      {
        if( value < 0 )
          throw new ArgumentOutOfRangeException( "RightColumnOffset" );

        ClientAnchor.RightOffset = value;
        UpdateWidth();
      }
    }
    
    /// <summary>
    /// Gets or sets object id.
    /// </summary>
    [ CLSCompliant( false ) ]
    public uint OldObjId
    {
      get
      {
        if( m_object != null )
        {
          return ( m_object.RecordsList[ 0 ] as ftCmo ).ID;
        }

        return 0;
      }
      set
      {
        if( m_object != null )
        {
          ( m_object.RecordsList[ 0 ] as ftCmo ).ID = ( ushort )value;
        }
      }
    }
    /// <summary>
    /// Returns internal shapes record. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public MsoBase Record
    {
      get
      {
        return m_record;
      }
    }
    /// <summary>
    /// Returns inner sp record. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public MsofbtSp InnerSpRecord
    {
      get
      {
        return m_shape;
      }
    }
    /// <summary>
    /// Indicates whether it is short version of the shape
    /// (without obj and maybe some other properties).
    /// </summary>
    public bool IsShortVersion
    {
      get
      {
        return m_clientAnchor.IsShortVersion;
      }
      set
      {
        m_clientAnchor.IsShortVersion = value;
      }
    }
    /// <summary>
    /// Returns number of shapes contained by this shape (1 means no sub shapes). Read-only.
    /// </summary>
    public int ShapeCount
    {
      get
      {
        MsofbtSpgrContainer spgrContainer = m_record as MsofbtSpgrContainer;

        return ( spgrContainer != null ) ?
          spgrContainer.ItemsList.Count :
          1;
      }
    }
    /// <summary>
    /// Indicates whether we have to update absolute positions after setting TopRow,
    /// BottomRow, LeftColumn, RightColumn.
    /// </summary>
    public bool UpdatePositions
    {
      get
      {
        return m_bUpdatePositions;
      }
      set
      {
        m_bUpdatePositions = value;
      }
    }
    /// <summary>
    /// Returns instance value. Read-only.
    /// </summary>
    public virtual int Instance
    {
      get
      {
        return ( m_shape != null ) ?
          m_shape.Instance :
          -1;
      }
    }
    /// <summary>
    /// Indicates whether fill item was created. Read-only.
    /// </summary>
    public bool HasFill
    {
      get
      {
          return m_fill != null && m_fill.Visible;
      }
      internal set
      {
          if (!value)
              m_fill = null;
          else if (m_fill == null && value)
              m_fill = new ShapeFillImpl(this.Application, this.Parent);
      }
    }
    /// <summary>
    /// Indicates whether line item was created. Read-only.
    /// </summary>
    public bool HasLineFormat
    {
      get
      {
        return m_lineFormat != null && m_lineFormat.Visible;
      }
      internal set
      {
        if( !value )
          m_lineFormat = null;
      }
    }
    /// <summary>
    /// Gets / sets shape id.
    /// </summary>
    public int ShapeId
    {
      get
      {
        return m_iShapeId;
      }
      set
      {
        m_iShapeId = value;
      }
    }
    /// <summary>
    /// Returns internal shape record, creates new one if  necessary. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public MsofbtSp ShapeRecord
    {
      get
      {
        if( m_shape == null )
        {
          m_shape = ( MsofbtSp )MsoFactory.GetRecord( MsoRecords.msofbtSp );
        }

        return m_shape;
      }
    }
      /// <summary>
      /// Indicates whether this control is an ActiveX Control.
      /// </summary>
    internal bool IsActiveX
    {
        get
        {
            ftPioGrbit pioGrBit = (ftPioGrbit)Obj.FindSubRecord(TObjSubRecordType.ftPioGrbit);
            if(pioGrBit!=null)
                return pioGrBit.IsActiveX;
            return false;
        }
    }
      /// <summary>
      /// Preserves the Shape style properties.
      /// </summary>
    internal Dictionary<string,string> StyleProperties
    {
        get
        {
            if (m_styleProperties == null)
                return new Dictionary<string, string>();
            return m_styleProperties;
        }
        set
        {
            m_styleProperties = value;
        }
    }
    /// <summary>
    /// Preserves the Shape style properties as string value.
    /// </summary>
    internal string PreserveStyleString
    {
        get 
        { 
            return m_preserveStyleString;
        }
        set
        {
            m_preserveStyleString = value;
        }
    }
    internal bool IsHyperlink
    {
        get
        {
            return m_isHyperlink;
        }
        set
        {
            m_isHyperlink = value;
        }
    }
    /// <summary>
    /// We find the anchor type.
    /// </summary>
    internal bool IsAbsoluteAnchor
    {
        get
        {
            return m_bIsAbsoluteAnchor;
        }
        set
        {
            m_bIsAbsoluteAnchor = value;
        }
    }    
    #endregion
    
    #region Class methods
    /// <summary>
    /// Generates default shape name and sets it.
    /// </summary>
    public virtual void GenerateDefaultName()
    {
      this.Name = CollectionBaseEx<IShape>.GenerateDefaultName( m_shapes, "Shape " );
    }
    /// <summary>
    /// This method is called when removing shapes from the collection.
    /// </summary>
    protected virtual void OnDelete()
    {
    }
    /// <summary>
    /// Sets object with value.
    /// </summary>
    /// <param name="value">Value to be set.</param>
    [ CLSCompliant( false ) ]
    protected void SetObject( OBJRecord value )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      m_object = value;
    }
    /// <summary>
    /// Creates a clone of the current shape and adds it to the parent shapes collection.
    /// </summary>
    /// <param name="parent">New parent for the shape object.</param>
    /// <param name="hashNewNames">Dictionary with new worksheet names.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <param name="addToCollections">Indicates whether we should add created
    /// shape into all necessary parent collections.</param>
    /// <returns>A copy of the current shape.</returns>
    public virtual IShape Clone( object parent, Dictionary<string, string> hashNewNames,
      Dictionary<int, int> dicFontIndexes, bool addToCollections )
    {
      ShapeImpl result = ( ShapeImpl )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();
      result.CopyFrom( this, hashNewNames, dicFontIndexes );

      result.CloneLineFill( this );
      if (result.ShapeType == ExcelShapeType.AutoShape)
      {
          AutoShapeImpl autoShapeImpl = result as AutoShapeImpl;
          autoShapeImpl.ShapeExt.Worksheet = result.m_shapes.Worksheet;
          autoShapeImpl.ShapeExt.ClientAnchor.Worksheet = result.m_shapes.Worksheet;
      }

      if( addToCollections )
        result.m_shapes.AddShape( result );

      result.AttachEvents();
      result.OldObjId = 0;

      return result;
    }
    /// <summary>
    /// Creates a clone of the current shape.
    /// </summary>
    /// <param name="parent">New parent for the shape object.</param>
    /// <returns>A copy of the current shape.</returns>
    public object Clone( object parent )
    {
      return Clone( parent, null, null, true );
    }
    /// <summary>
    /// Copies settings from another shape object.
    /// </summary>
    /// <param name="shape">Shape to copy settings from.</param>
    /// <param name="hashNewNames">Dictionary with new worksheet names.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    public virtual void CopyFrom( ShapeImpl shape, Dictionary<string, string> hashNewNames,
      Dictionary<int, int> dicFontIndexes )
    {
      MsoBase record = shape.m_record;

      if( record != null )
        m_record = ( MsoBase )record.Clone();

      UpdateRecord( shape.ClientAnchor );
      m_iShapeId = shape.m_iShapeId;
    }
    /// <summary>
    /// Checks whether it is possible insert row or column into iIndex.
    /// </summary>
    /// <param name="iIndex">Index of row or column to insert.</param>
    /// <param name="iCount">Number of rows or columns to insert.</param>
    /// <param name="bRow">Indicates whether rows or columns are inserted.</param>
    /// <param name="iMaxIndex">Maximum possible index.</param>
    /// <returns>True if it is possible to insert row or column.</returns>
    public bool CanInsertRowColumn( int iIndex, int iCount, bool bRow, int iMaxIndex )
    {
      if( !IsMoveWithCell && !IsSizeWithCell ) return true;

      if( IsIndexLess( iIndex, bRow ) )
      {
        if( IsMoveWithCell )
        {
          return GetLowerBound( bRow ) + iCount >= 0
            && GetUpperBound( bRow ) + iCount <= iMaxIndex;
        }
      }
      else if( IsIndexMiddle( iIndex, bRow ) )
      {
        if( IsSizeWithCell )
        {
          return GetUpperBound( bRow ) + iCount <= iMaxIndex;
        }
      }

      return true;
    }
    /// <summary>
    /// Returns lower bound of the shape.
    /// </summary>
    /// <param name="bRow">Indicates whether lower row or lower column must be returned.</param>
    /// <returns>Lower bound of the shape.</returns>
    private int GetLowerBound( bool bRow )
    {
      return bRow ? TopRow : LeftColumn;
    }
    /// <summary>
    /// Returns upper bound of the shape.
    /// </summary>
    /// <param name="bRow">Indicates whether upper row or upper column must be returned.</param>
    /// <returns>Upper bound of the shape.</returns>
    private int GetUpperBound( bool bRow )
    {
      return bRow ? BottomRow : RightColumn;
    }
    /// <summary>
    /// Removes row or column.
    /// </summary>
    /// <param name="iIndex">Index of row or column to remove.</param>
    /// <param name="iCount">Number of rows or columns to remove.</param>
    /// <param name="bRow">Indicates whether rows or columns are removed.</param>
    public void RemoveRowColumn( int iIndex, int iCount, bool bRow )
    {
      int iLast = ( bRow ) ? BottomRow : RightColumn;
      bool bContinue = iIndex <= iLast;

      if( !IsMoveWithCell && !IsSizeWithCell )
      {
        UpdateNotSizeNotMoveShape( bRow, iIndex, -iCount );
        bContinue = false;
      }

      if( bContinue )
      {
        int iAbove = GetCountAbove( iIndex, iCount, bRow );

        if( iAbove > 0 )
          UpdateAboveRowColumnIndexes( -iAbove, bRow );

        iCount -= iAbove;
        bContinue = iCount > 0;
      }

      if( bContinue )
      {
        bool bIsFirst = IndicatesFirst( iIndex, iCount, bRow );

        if( bIsFirst )
        {
          UpdateFirstRowColumnIndexes( bRow, -1 );
          iCount --;

          bContinue = iCount > 0;
        }
      }

      if( bContinue )
      {
        int iInside = GetCountInside( iIndex, iCount, bRow );

        if( iInside > 0 )
          UpdateInsideRowColumnIndexes( -iInside, bRow );

        iCount -= iInside;
        bContinue = iCount > 0;
      }

      if( bContinue )
      {
        UpdateLastRowColumnIndex( bRow );
      }
    }
    /// <summary>
    /// This method should be called after rows or columns were inserted.
    /// </summary>
    /// <param name="iIndex">Index of row or column to insert.</param>
    /// <param name="iCount">Number of rows or columns to insert.</param>
    /// <param name="bRow">Indicates whether rows or columns are inserted.</param>
    public void InsertRowColumn( int iIndex, int iCount, bool bRow )
    {
      if( !IsSizeWithCell && !IsMoveWithCell )
      {
        UpdateNotSizeNotMoveShape( bRow, iIndex, iCount );
        return;
      }

      if( IsIndexLess( iIndex, bRow ) )
      {
        // Row is above the shape, or column is left to the shape.
        if( IsMoveWithCell )
        {
          IncreaseAndUpdateAll( iCount, bRow );
        }
      }
      else if( IsIndexMiddle( iIndex, bRow ) )
      {
        if( IsSizeWithCell )
        {
          IncreaseAndUpdateEnd( iCount, bRow );
        }
      }
    }
    /// <summary>
    /// Updates formulas after copy operation.
    /// </summary>
    /// <param name="iCurIndex">Current worksheet index.</param>
    /// <param name="iSourceIndex">Source worksheet index.</param>
    /// <param name="sourceRect">Source rectangle.</param>
    /// <param name="iDestIndex">Destination worksheet index.</param>
    /// <param name="destRect">Destination rectangle.</param>
    public virtual void UpdateFormula( int iCurIndex, int iSourceIndex,
      Rectangle sourceRect, int iDestIndex, Rectangle destRect )
    {
    }
    /// <summary>
    /// Sets shape name without updating parent shapes collection.
    /// </summary>
    /// <param name="strShapeName">Name to set.</param>
    public void SetName( string strShapeName )
    {
      if( strShapeName == null )
        throw new ArgumentNullException( "strShapeName" );

      if( strShapeName.Length == 0 )
        throw new ArgumentException( "strShapeName - string cannot be empty." );

      m_strName = strShapeName;
    }
    /// <summary>
    /// Registers shape in all required sub collections.
    /// </summary>
    public virtual void RegisterInSubCollection()
    {
    }
    /// <summary>
    /// Indicates is can copy current shape.
    /// </summary>
    /// <param name="sourceRec">Represents source range dimension.</param>
    /// <param name="destRec">Represents destination range dimension.</param>
    /// <param name="newPosition">Gets new position of shape.</param>
    /// <returns>Returns true if can copy; otherwise - false.</returns>
    public virtual bool CanCopyShapesOnRangeCopy( Rectangle sourceRec, Rectangle destRec, out Rectangle newPosition )
    {
      int iLeft = LeftColumn;
      int iTop = TopRow;
      //validate, that one dimension sheet is in source rec.
      bool bFlag = iLeft == RightColumn && ( iLeft > sourceRec.Right || iLeft < sourceRec.Left );
      bFlag = bFlag || ( iTop == BottomRow && ( iTop > sourceRec.Bottom || iTop < sourceRec.Top ) );

      bool result = IsMoveWithCell && !bFlag;
      newPosition = new Rectangle( 0, 0, 0 ,0 );

      if( result )
      {
        //calculate new top position of shape.
        newPosition.Y = iTop - sourceRec.Top + destRec.Top;
        // checks validation. In first case - indicates is shape in source range. in second - validate coordinate.
        result = sourceRec.Top - 1 <= iTop && newPosition.Top > 0;
      }

      if( result )
      {
        newPosition.X = iLeft - sourceRec.Left + destRec.Left;
        result = sourceRec.Left - 1 <= iLeft && newPosition.Left > 0;
      }

      if( result )
      {
        int iBottom = destRec.Bottom - ( sourceRec.Bottom - BottomRow );
        result = sourceRec.Bottom + 1 >= BottomRow && iBottom <= m_book.MaxRowCount;
        newPosition.Height = iBottom - newPosition.Y;
      }

      if( result )
      {
        int iRight = destRec.Right - ( sourceRec.Right - RightColumn );
        result = sourceRec.Right + 1 >= RightColumn && iRight <= m_book.MaxColumnCount;
        newPosition.Width = iRight - newPosition.X;
      }

      if( result && 
        ( m_book.Version !=ExcelVersion.Excel97to2003 ) &&
        m_xmlDataStream != null && m_xmlDataStream.Length > 0 )
      {
        result = false;
      }

      return result;
    }
    /// <summary>
    /// Copies / moves shape in range copy / move.
    /// </summary>
    /// <param name="sheet">Represents destination sheet.</param>
    /// <param name="destRec">Represents position of .</param>
    /// <param name="bIsCopy">Indicates is copy.</param>
    /// <returns>Returns copied moved shape.</returns>
    public virtual ShapeImpl CopyMoveShapeOnRangeCopyMove( WorksheetImpl sheet, Rectangle destRec, bool bIsCopy )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      ShapeImpl shapeToMove = this;
      ShapesCollection shapes = ( ShapesCollection )sheet.Shapes;

      if( bIsCopy )
      {
        shapeToMove = ( ShapeImpl )shapeToMove.Clone( shapes, null, null, true );
      }
      else
      {
        if( sheet != Worksheet )
        {
          ShapesCollection sourceColl = ( ( ShapesCollection )Worksheet.Shapes );

          sourceColl.Remove( this );
          shapes.AddShape( shapeToMove );
        }
      }

      int iHeight = Height;
      int iWidth = Width;
      shapeToMove.ClientAnchor.TopRow = destRec.Top - 1;
      shapeToMove.ClientAnchor.LeftColumn = destRec.Left - 1;

      if( IsSizeWithCell )
      {
        shapeToMove.ClientAnchor.BottomRow = destRec.Bottom - 1;
        shapeToMove.ClientAnchor.RightColumn = destRec.Right - 1;

        shapeToMove.UpdateWidth();
        shapeToMove.UpdateHeight();
      }
      else
      {
        shapeToMove.Height = iHeight;
        shapeToMove.Width = iWidth;
        shapeToMove.UpdateBottomRow();
        shapeToMove.UpdateRightColumn();
      }

      return shapeToMove;
    }
    /// <summary>
    /// Copy comments options.
    /// </summary>
    /// <param name="sourceShape">Represents source shape.</param>
    /// <param name="dicFontIndexes">Represents dictionary with font indexes.</param>
    public void CopyFillOptions( ShapeImpl sourceShape, IDictionary dicFontIndexes )
    {
      if( sourceShape.m_lineFormat != null )
        m_lineFormat = sourceShape.m_lineFormat.Clone( this );

      if( sourceShape.m_fill != null )
        m_fill = sourceShape.m_fill.Clone( this );
    }
    /// <summary>
    /// Prepares shape for serialization. We should fill all not prepared fields
    /// like m_shape in this method.
    /// </summary>
    public void PrepareForSerialization()
    {
      if( OldObjId == 0 )
      {
        m_book.CurrentObjectId++;
        OldObjId = ( uint )m_book.CurrentObjectId;
      }

      OnPrepareForSerialization();
      ShapeRecord.ShapeId = m_iShapeId;
    }
    /// <summary>
    /// This method is called inside of PrepareForSerialization to make shape-dependent preparations.
    /// </summary>
    protected virtual void OnPrepareForSerialization()
    {
      MsofbtSpContainer container = m_record as MsofbtSpContainer;

      if( container != null )
      {
        SerializeMsoOptions( container );
      }

      if( m_object != null )
        UpdateMacroInfo();
    }
    /// <summary>
    /// Updates macro information.
    /// </summary>
    private void UpdateMacroInfo()
    {
      List<ObjSubRecord> subRecords = m_object.RecordsList;
      ftMacro macro = null;

      for( int i = 0, len = subRecords.Count; i < len; i++ )
      {
        ObjSubRecord subRecord = subRecords[ i ];

        if( subRecord.Type == TObjSubRecordType.ftMacro )
        {
          macro = ( ftMacro )subRecord;

          if( m_macroTokens == null )
            subRecords.RemoveAt( i );

          break;
        }
      }

      if( macro == null && m_macroTokens != null )
      {
        macro = new ftMacro();
        subRecords.Insert( subRecords.Count - 2, macro );
      }

      if( m_macroTokens != null )
        macro.Tokens = m_macroTokens;
    }
    /// <summary>
    /// Sets instance value for the shape.
    /// </summary>
    /// <param name="instance">Instance to set.</param>
    internal void SetInstance( int instance )
    {
      ShapeRecord.Instance = instance;
    }
    /// <summary>
    /// Sets option with value.
    /// </summary>
    /// <param name="option">Represents shape option.</param>
    /// <param name="value">Represents value to be set.</param>
    public void SetOption( MsoOptions option, int value )
    {
      FOPTE fopte = new FOPTE();
      fopte.Id = option;
      fopte.Int32Value = value;
      ShapeOptions.AddOptionsOrReplace( fopte );
    }
    /// <summary>
    /// Gets shape options.
    /// </summary>
    private MsofbtOPT ShapeOptions
    {
      get
      {
        if( m_options == null )
          m_options = CreateDefaultOptions();

        return m_options;
      }
    }
    /// <summary>
    /// Represens the Child Shapes in the Group.
    /// </summary>
    internal List<ShapeImpl> ChildShapes
    {
        get
        {
            if (m_childShapes == null)
                m_childShapes = new List<ShapeImpl>();
            return m_childShapes;
        }
    }
    /// <summary>
    /// Child Client Anchor Record.
    /// </summary>
    internal MsofbtChildAnchor ChildAnchor
    {
        get
        {
            return m_childAnchor;
        }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Updates indexes to named ranges.
    /// </summary>
    /// <param name="arrNewIndex">New indexes.</param>
    public void UpdateNamedRangeIndexes( int[] arrNewIndex )
    {
      if( m_macroTokens != null )
      {
        m_book.FormulaUtil.UpdateNameIndex( m_macroTokens, arrNewIndex );
      }
    }
    /// <summary>
    /// Updates indexes to named ranges.
    /// </summary>
    /// <param name="dicNewIndex">New indexes.</param>
    public void UpdateNamedRangeIndexes( IDictionary<int, int> dicNewIndex )
    {
      if( m_macroTokens != null )
      {
        m_book.FormulaUtil.UpdateNameIndex( m_macroTokens, dicNewIndex );
      }
    }
    /// <summary>
    /// Updates left column and offset values correspondingly
    /// to X-coordinate changes.
    /// </summary>
    private void UpdateLeftColumn()
    {
      WorksheetImpl sheet = m_shapes.Worksheet;

      if( sheet == null )
      {
        ClientAnchor.LeftColumn = m_rectAbsolute.Left;
        ClientAnchor.LeftOffset = 0;
        return;
      }

      int iCurPos = 0;
      int iCurColumn = 0;
      
      while( iCurPos <= m_rectAbsolute.Left )
      {
        iCurColumn++;

        if( iCurColumn > m_book.MaxColumnCount )
        {
          ClientAnchor.LeftColumn = m_book.MaxColumnCount - 1;
          ClientAnchor.LeftOffset = DEF_FULL_COLUMN_OFFSET;
          return;
        }

        iCurPos += sheet.GetColumnWidthInPixels( iCurColumn );
      }

      //iCurColumn--;
      int iColWidth = sheet.GetColumnWidthInPixels( iCurColumn );
      iCurPos -= iColWidth;

      // Left column was evaluated; now we should evaluate offset.
      int iPixelsLeft = m_rectAbsolute.Left - iCurPos;
      
      ClientAnchor.LeftColumn = iCurColumn - 1;
      ClientAnchor.LeftOffset = ConvertPixelsIntoWidthOffset( iPixelsLeft, iColWidth );
    }
    /// <summary>
    /// Clears the Shape Offsets.
    /// </summary>
    /// <param name="clear">Clears the offset if True.</param>
    internal void ClearShapeOffset(bool clear)
    {
        if (clear)
        {
            ClientAnchor.LeftOffset = 0;
            ClientAnchor.RightOffset = 0;
            ClientAnchor.BottomOffset = 0;
            ClientAnchor.TopOffset = 0;
        }
    }
    protected internal void UpdateRightColumn(int iCount)
    {
        WorksheetImpl sheet = m_shapes.Worksheet;

        if (sheet == null)
        {
            m_rectAbsolute.Location = new Point(ClientAnchor.LeftColumn, m_rectAbsolute.Top);
            ClientAnchor.RightColumn = m_rectAbsolute.Left + m_rectAbsolute.Width;
            ClientAnchor.RightOffset = 0;
            return;
        }

        int iCurWidth = Width;
        int iCurLeftColumn = LeftColumn;
        int iCurLeftOffset = LeftColumnOffset;

        while (iCurWidth >= 0)
        {
            if (iCurLeftColumn > m_book.MaxColumnCount)
            {
                RightColumn = m_book.MaxColumnCount;
                RightColumnOffset = DEF_FULL_COLUMN_OFFSET;
                break;
            }

            int iLeftColWidth = sheet.GetColumnWidthInPixels(iCurLeftColumn+iCount);
            iLeftColWidth = iLeftColWidth > sheet.GetColumnWidthInPixels(iCurLeftColumn) ?
                            iLeftColWidth : sheet.GetColumnWidthInPixels(iCurLeftColumn);
            int iSpaceInLeftCell = iLeftColWidth - OffsetInPixels(iCurLeftColumn, iCurLeftOffset, true);

            if (iSpaceInLeftCell < 0) iSpaceInLeftCell = 0;

            if (iSpaceInLeftCell < 0)
                throw new ArgumentOutOfRangeException(
                  "Calculated value can't be less than zero, error in coordinates update.");

            // Check if whole shape can be placed into this column.
            if (iSpaceInLeftCell > iCurWidth)
            {
                RightColumn = iCurLeftColumn;
                RightColumnOffset = iCurLeftOffset + PixelsInOffset(iCurLeftColumn, iCurWidth, true);
                break;
            }
            else
            {
                iCurWidth -= iSpaceInLeftCell;
                iCurLeftColumn++;
                iCurLeftOffset = 0;
            }
        }  
    }
    /// <summary>
    /// Updates right column and offset values correspondingly
    /// to left column or offset or width changes.
    /// </summary>
    protected internal void UpdateRightColumn()
    {
        UpdateRightColumn(0);
    }
    /// <summary>
    /// Updates top row and offset values correspondingly
    /// to Y-coordinate changes.
    /// </summary>
    private void UpdateTopRow()
    {
      WorksheetImpl sheet = m_shapes.Worksheet;

      if( sheet == null )
      {
        ClientAnchor.TopRow = m_rectAbsolute.Y;
        ClientAnchor.TopOffset = 0;
        return;
      }

      int iCurPos = 0;
      int iCurRow = 0;
      
      while( iCurPos <= m_rectAbsolute.Top )
      {
        iCurRow++;
        iCurPos += sheet.GetRowHeightInPixels( iCurRow );
      }

      //iCurRow--;
      int iRowHeight = sheet.GetRowHeightInPixels( iCurRow );
      iCurPos -= iRowHeight;

      // Top row was evaluated, now we should evaluate offset.
      int iPixelsLeft = m_rectAbsolute.Top - iCurPos;
      
      ClientAnchor.TopRow = iCurRow - 1;
      ClientAnchor.TopOffset = ConvertPixelsIntoHeightOffset( iPixelsLeft, iRowHeight );
    }
    /// <summary>
    /// Updates bottom row of the shape.
    /// </summary>
    protected internal void UpdateBottomRow()
    {
      WorksheetImpl sheet = m_shapes.Worksheet;

      if( sheet == null )
      {
        m_rectAbsolute.Location = new Point( m_rectAbsolute.Left, ClientAnchor.TopRow );
        ClientAnchor.BottomRow = m_rectAbsolute.Top + m_rectAbsolute.Height;
        ClientAnchor.BottomOffset = 0;
        return;
      }

      int iCurHeight = Height;
      int iCurRow = TopRow;
      int iCurOffset = TopRowOffset;

      while( iCurHeight >= 0 )
      {
        if( iCurRow > m_book.MaxRowCount )
        {
          BottomRow = m_book.MaxRowCount;
          BottomRowOffset = DEF_FULL_ROW_OFFSET;
          break;
        }

        int iRowHeight = sheet.GetRowHeightInPixels( iCurRow );
        int iSpaceInCell = iRowHeight - OffsetInPixels( iCurRow, iCurOffset, false );

        if( iSpaceInCell < 0 )
          throw new ArgumentOutOfRangeException( 
            "Calculated value can't be less than zero, error in coordinates update." );

        // Check if whole shape can be placed into this column.
        if( iSpaceInCell > iCurHeight )
        {
          BottomRow = iCurRow;
          BottomRowOffset = iCurOffset + PixelsInOffset( iCurRow, iCurHeight, false );
          break;
        }
        else
        {
          iCurHeight -= iSpaceInCell;
          iCurRow++;
          iCurOffset = 0;
        }
      }
    }
    internal void UpdateAnchorPoints()
    {
        UpdateTopRow();
        UpdateBottomRow();
        UpdateLeftColumn();
        UpdateRightColumn();
    }
    /// <summary>
    /// Updates width of the shape.
    /// </summary>
    protected internal void UpdateWidth()
    {
      m_rectAbsolute.Width = GetWidth( LeftColumn, LeftColumnOffset
        , RightColumn, RightColumnOffset, false );
    }
    /// <summary>
    /// Updates Height of the shape.
    /// </summary>
    protected internal void UpdateHeight()
    {
      //      WorksheetImpl sheet = m_shapes.Worksheet;
      //
      //      if( sheet == null ) return;
      //
      //      int iTopRowHeight = sheet.GetRowHeightInPixels( TopRow );
      //      int iBottomRowHeight = sheet.GetRowHeightInPixels( BottomRow );
      //
      //      int iTopPixelsOffset = ( int )Math.Round( TopRowOffset * iTopRowHeight
      //        / ( double )DEF_FULL_ROW_OFFSET );
      //
      //      int iBottomPixelsOffset = ( int )Math.Round( BottomRowOffset * iBottomRowHeight
      //        / ( double )DEF_FULL_ROW_OFFSET );
      //
      //      int iHeight = iBottomPixelsOffset - iTopPixelsOffset;
      //
      //      for( int i = TopRow, last = BottomRow; i < last; i++ )
      //      {
      //        iHeight += sheet.GetRowHeightInPixels( i );
      //      }

      m_rectAbsolute.Height = GetHeight( TopRow, TopRowOffset, BottomRow,
        BottomRowOffset, false );//iHeight;
    }
    /// <summary>
    /// Converts offset value into pixels.
    /// </summary>
    /// <param name="iRowColumn">Width of row or column.</param>
    /// <param name="iOffset">Offset in row or column.</param>
    /// <param name="isXOffset">Indicates whether it is column offset.</param>
    /// <returns>Offset value in pixels.</returns>
    internal int OffsetInPixels( int iRowColumn, int iOffset, bool isXOffset )
    {
      WorksheetImpl sheet = m_shapes.Worksheet;

      if( sheet == null ) return 0;

      double dResult;

      if( isXOffset )
      {
        int iFullWidth = sheet.GetColumnWidthInPixels( iRowColumn );
        dResult = iOffset * iFullWidth / ( double )DEF_FULL_COLUMN_OFFSET;
        //return m_shapes.Worksheet.ColumnWidthToPixels( dColumnOffset );
      }
      else
      {
        double dFullHeight = sheet.GetRowHeightInPixels( iRowColumn );
        dResult = iOffset * dFullHeight / ( double )DEF_FULL_ROW_OFFSET;
      }

      return ( int )Math.Round( dResult );
    }
    /// <summary>
    /// Converts pixels into offset value.
    /// </summary>
    /// <param name="iCurRowColumn">Index to the current row or column.</param>
    /// <param name="iPixels">Size in pixels.</param>
    /// <param name="isXSize">Indicates whether iCurRowColumn is column index should.</param>
    /// <returns>Size of the row / column in pixels.</returns>
    internal int PixelsInOffset( int iCurRowColumn, int iPixels, bool isXSize )
    {
      WorksheetImpl sheet = m_shapes.Worksheet;

      if( sheet == null ) return 0;

      if( iPixels < 0 )
        throw new ArgumentOutOfRangeException( "IPixels", "Can't be less than zero." );

      if( isXSize )
      {
        int iTargetColWidth = sheet.GetColumnWidthInPixels( iCurRowColumn );
        if (iTargetColWidth != 0)
            return iPixels * DEF_FULL_COLUMN_OFFSET / iTargetColWidth;
        else
            return iTargetColWidth;
      }
      else
      {
        int iTargetRowHeight = sheet.GetRowHeightInPixels( iCurRowColumn );

        return ( iTargetRowHeight != 0 ) ?
          iPixels * DEF_FULL_ROW_OFFSET / iTargetRowHeight:
          DEF_FULL_ROW_OFFSET;
      }
    }
    /// <summary>
    /// Returns width of the area.
    /// </summary>
    /// <param name="iColumn1">Column index.</param>
    /// <param name="iOffset1">Column offset.</param>
    /// <param name="iColumn2">Second column index.</param>
    /// <param name="iOffset2">Offset in the second column.</param>
    /// <param name="bIsOffsetInPixels">Indicates whether offsets are in pixels.</param>
    /// <returns>Width in pixels of the specified area.</returns>
    private int GetWidth( int iColumn1, int iOffset1, int iColumn2, int iOffset2
      , bool bIsOffsetInPixels )
    {
      WorksheetImpl sheet = m_shapes.Worksheet;

      if( sheet == null ) return iColumn2 - iColumn1;

      if( iColumn1 < 1 || iColumn1 > m_book.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "iColumn1" );

      if( iOffset2 == 0 )
      {
        iColumn2--;
        iOffset2 = DEF_FULL_COLUMN_OFFSET;
      }

      if( iColumn1 > iColumn2 )
        return 0;

      if( iColumn2 < 1 || iColumn2 > m_book.MaxColumnCount )
        throw new ArgumentOutOfRangeException( "iColumn2" );

      if( iOffset1 < 0 )
        throw new ArgumentOutOfRangeException( "iOffset1" );

      if( iOffset2 < 0 )
        throw new ArgumentOutOfRangeException( "iOffset2" );

      if( iColumn1 == iColumn2 && iOffset1 > iOffset2 )
      {
        //System.Diagnostics.Debug.WriteLine( m_shapes.Worksheet.Name );
        //throw new ArgumentOutOfRangeException( "iOffset1 || iOffset2" );
        return 0;
        //iOffset2 = iOffset1;
      }

      int iLeftColumnWidth = sheet.GetColumnWidthInPixels( iColumn1 );
      int iRightColumnWidth = sheet.GetColumnWidthInPixels( iColumn2 );

      int iLeftPixelsOffset = ConvertWidthOffsetIntoPixels( iLeftColumnWidth
        , Math.Min( iOffset1, DEF_FULL_COLUMN_OFFSET ), bIsOffsetInPixels );
      
      int iRightPixelsOffset = ConvertWidthOffsetIntoPixels( iRightColumnWidth
        , Math.Min( iOffset2, DEF_FULL_COLUMN_OFFSET ), bIsOffsetInPixels );

      int iWidth = iRightPixelsOffset - iLeftPixelsOffset;

      for( int i = iColumn1; i < iColumn2; i++ )
      {
        iWidth += sheet.GetColumnWidthInPixels( i );
      }

      return iWidth;
    }

    /// <summary>
    /// Returns height of the specified area.
    /// </summary>
    /// <param name="iRow1">The first row.</param>
    /// <param name="iOffset1">Offset in the first row.</param>
    /// <param name="iRow2">The second row.</param>
    /// <param name="iOffset2">Offset in the second row.</param>
    /// <param name="bIsOffsetInPixels">Indicates whether offsets are in pixels.</param>
    /// <returns>Height in pixels of the specified area.</returns>
    private int GetHeight( int iRow1, int iOffset1, int iRow2, int iOffset2
      , bool bIsOffsetInPixels )
    {
      WorksheetImpl sheet = m_shapes.Worksheet;

      if( sheet == null ) return iRow2 - iRow1;

      if( iRow1 < 1 || iRow1 > m_book.MaxRowCount )
        throw new ArgumentOutOfRangeException( "iRow1" );

      if( iRow2 < 1 || iRow2 > m_book.MaxRowCount )
        throw new ArgumentOutOfRangeException( "iRow2" );

      if( iRow2 < iRow1 )
        return 0;

      if( iOffset1 < 0 )
        throw new ArgumentOutOfRangeException( "iOffset1" );

      if( iOffset2 < 0 )
        throw new ArgumentOutOfRangeException( "iOffset2" );

      if( iRow1 == iRow2 && iOffset1 > iOffset2 )
        return 0;
      //throw new ArgumentOutOfRangeException( "iOffset1 || iOffset2" );

      int iTopRowHeight = sheet.GetRowHeightInPixels( iRow1 );
      int iBottomRowHeight = sheet.GetRowHeightInPixels( iRow2 );

      int iTopPixelsOffset = ConvertHeightOffsetIntoPixels( iTopRowHeight
        , iOffset1, bIsOffsetInPixels );

      int iBottomPixelsOffset = ConvertHeightOffsetIntoPixels( iBottomRowHeight
        , iOffset2, bIsOffsetInPixels );

      int iHeight = iBottomPixelsOffset - iTopPixelsOffset;

      if( m_book.Loading )
      {
        iHeight += sheet.RowHeightHelper.GetTotal( iRow2 - 1 ) -
          sheet.RowHeightHelper.GetTotal( iRow1 - 1 );
      }
      else
      {
        for( int i = iRow1; i < iRow2; i++ )
        {
          iHeight += sheet.GetRowHeightInPixels( i );
        }
      }

      return iHeight;
    }
    /// <summary>
    /// Converts width offset into pixels and checks if it is out of range.
    /// </summary>
    /// <param name="iColWidth">Width of the target column.</param>
    /// <param name="iOffset">Offset in the target column.</param>
    /// <param name="bIsInPixels">
    /// Indicates whether offset is in pixels (True) or in the units
    /// relative to width of the column (False).
    /// </param>
    /// <returns>Offset in pixels.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If resulting offset is more that column width.
    /// </exception>
    private int ConvertWidthOffsetIntoPixels( int iColWidth, int iOffset, bool bIsInPixels )
    {
      int result = iOffset;

      if( !bIsInPixels )
      {
        result = ( int )Math.Round( iOffset * iColWidth
          / ( double )DEF_FULL_COLUMN_OFFSET );
      }

      return result;
    }
    /// <summary>
    /// Converts width offset into pixels and checks if it is out of range.
    /// </summary>
    /// <param name="iRowHeight">Height of the target row.</param>
    /// <param name="iOffset">Offset in the target row.</param>
    /// <param name="bIsOffsetInPixels">
    /// Indicates whether offset is in pixels (True) or in the units
    /// relative to height of the row (False).
    /// </param>
    /// <returns>Offset in pixels.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If resulting offset is more than row height.
    /// </exception>
    private static int ConvertHeightOffsetIntoPixels( int iRowHeight, int iOffset
      , bool bIsOffsetInPixels )
    {
      int iResult = iOffset;

      if( !bIsOffsetInPixels )
      {
        iResult = ( int )Math.Round( iOffset * iRowHeight
          / ( double )DEF_FULL_ROW_OFFSET );
      }

      //      if( iResult > iRowHeight )
      //        throw new ArgumentOutOfRangeException( "iOffset" );

      return iResult;
    }
    /// <summary>
    /// Converts pixels into column width offset.
    /// </summary>
    /// <param name="iPixels">Represents pixel.</param>
    /// <param name="iColWidth">Width of the target column.</param>
    /// <returns>Value in column width.</returns>
    private int ConvertPixelsIntoWidthOffset( int iPixels, int iColWidth )
    {
      //if( iPixels < 0 || iPixels > iColWidth )
      //  throw new ArgumentOutOfRangeException( "iPixels" );

      return iPixels * DEF_FULL_COLUMN_OFFSET / iColWidth;
    }
    /// <summary>
    /// Converts pixels into height offset.
    /// </summary>
    /// <param name="iPixels">Pixels to convert.</param>
    /// <param name="iRowHeight">Represents target row height.</param>
    /// <returns>Offset in terms of Row height.</returns>
    private int ConvertPixelsIntoHeightOffset( int iPixels, int iRowHeight )
    {
      if( iPixels < 0 || iPixels > iRowHeight )
        throw new ArgumentOutOfRangeException( "iPixels" );

      return iPixels * DEF_FULL_ROW_OFFSET / iRowHeight;
    }
    /// <summary>
    /// Evaluates top left position of the shape.
    /// </summary>
    public void EvaluateTopLeftPosition()
    {
      EvaluateLeftPosition();
      EvaluateTopPosition();
      //EvaluateRightPosition();
    }
    /// <summary>
    /// Evaluates left position.
    /// </summary>
    private void EvaluateLeftPosition()
    {
      m_rectAbsolute.X = GetWidth( 1, 0, LeftColumn, LeftColumnOffset, false );
    }
    /// <summary>
    /// Evaluates Right Position.
    /// </summary>
    private void EvaluateRightPosition()
    {
      m_rectAbsolute.Y = GetHeight( 1, 0, TopRow, TopRowOffset, false );
    }
    /// <summary>
    /// Evaluates top position.
    /// </summary>
    private void EvaluateTopPosition()
    {
      m_rectAbsolute.Y = GetHeight( 1, 0, TopRow, TopRowOffset, false );
    }
    /// <summary>
    /// Sets client anchor record.
    /// </summary>
    /// <param name="anchor">Represents anchor.</param>
    [ CLSCompliant( false ) ]
    protected void SetClientAnchor( MsofbtClientAnchor anchor )
    {
      if( anchor == null )
        throw new ArgumentOutOfRangeException( "anchor" );

      m_clientAnchor = anchor;
    }
    /// <summary>
    /// This method is called when left column was changed.
    /// </summary>
    private void OnLeftColumnChange()
    {
      if( IsSizeWithCell )
      {
        UpdateWidth();
      }
      else
      {
        UpdateRightColumn();
      }
    }
    /// <summary>
    /// This method is called when top row was changed.
    /// </summary>
    private void OnTopRowChanged()
    {
      EvaluateTopPosition();

      if( IsSizeWithCell )
      {
        UpdateHeight();
      }
      else
      {
        UpdateBottomRow();
      }
    }
    /// <summary>
    /// Indicates whether specified row / column index is before shape.
    /// </summary>
    /// <param name="iRowColumnIndex">Row / column index.</param>
    /// <param name="bIsRow">Indicates whether it is row or column index.</param>
    /// <returns>True if specified row / column index is before shape.</returns>
    private bool IsIndexLess( int iRowColumnIndex, bool bIsRow )
    {
      return bIsRow
        ? iRowColumnIndex <= TopRow
        : iRowColumnIndex <= LeftColumn;
    }
    /// <summary>
    /// Indicates whether specified row / column index is in the middle of the shape.
    /// </summary>
    /// <param name="iRowColumnIndex">Row / column index.</param>
    /// <param name="bIsRow">Indicates whether it is row or column index.</param>
    /// <returns>True if specified row / column index is before shape.</returns>
    private bool IsIndexMiddle( int iRowColumnIndex, bool bIsRow )
    {
      return bIsRow
        ? iRowColumnIndex <= BottomRow
        : iRowColumnIndex <= RightColumn;
    }
    /// <summary>
    /// Indicates whether it is last row or column index
    /// </summary>
    /// <param name="iRowColumnIndex">Row / column index.</param>
    /// <param name="bIsRow">Indicates whether it is row or column index.</param>
    /// <returns>Value indicating whether it is last row or column index.</returns>
    private bool IsIndexLast( int iRowColumnIndex, bool bIsRow )
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Increases and updates both begin and end of the shape.
    /// </summary>
    /// <param name="iCount">Number of inserted rows / columns.</param>
    /// <param name="bIsRow">Indicates whether rows where inserted.</param>
    private void IncreaseAndUpdateAll( int iCount, bool bIsRow )
    {
      if( bIsRow )
      {
        if( ClientAnchor.TopRow + iCount >= 0 )
        {
          ClientAnchor.TopRow += iCount;
        }
        else
        {
          ClientAnchor.TopOffset = 0;
        }

        ClientAnchor.BottomRow += iCount;
        EvaluateTopPosition();
      }
      else
      {
        if( ClientAnchor.LeftColumn + iCount >= 0 )
        {
          ClientAnchor.LeftColumn += iCount;
        }
        else
        {
          ClientAnchor.LeftOffset = 0;
        }

        ClientAnchor.RightColumn += iCount;
        EvaluateLeftPosition();
      }
    }
    /// <summary>
    /// Increases and updates end of the shape.
    /// </summary>
    /// <param name="iCount">Number of inserted rows / columns.</param>
    /// <param name="bIsRow">Indicates whether rows where inserted.</param>
    private void IncreaseAndUpdateEnd( int iCount, bool bIsRow )
    {
      if( bIsRow )
      {
        ClientAnchor.BottomRow += iCount;
        UpdateHeight();
      }
      else
      {
        ClientAnchor.RightColumn += iCount;
        UpdateWidth();
      }
    }
    /// <summary>
    /// Gets count of rows \ columns that is above shape.
    /// </summary>
    /// <param name="iIndex">Represents start index.</param>
    /// <param name="iCount">Represents count of rows \ columns.</param>
    /// <param name="bIsRow">Indicates is in row.</param>
    /// <returns>Returns count of rows \ columns above shape.</returns>
    private int GetCountAbove( int iIndex, int iCount, bool bIsRow )
    {
      int iFirst = ( bIsRow )
        ? TopRow
        : LeftColumn;

      if( iIndex < iFirst )
        return Math.Min( iCount, iFirst - iIndex );

      return 0;
    }
    /// <summary>
    /// Gets count of rows inside.
    /// </summary>
    /// <param name="iIndex">Represents first index.</param>
    /// <param name="iCount">Represents count.</param>
    /// <param name="bIsRow">Indicates is row or column.</param>
    /// <returns>Returns count of inside.</returns>
    private int GetCountInside( int iIndex, int iCount, bool bIsRow )
    {
      int iBottom = ( bIsRow ) ? BottomRow - 1 : RightColumn - 1;

      if( iIndex > iBottom )
        return 0;

      int iTop = ( bIsRow ) ? TopRow + 1 : LeftColumn + 1;
      int iBottom2 = iIndex + iCount - 1;

      int iResult = Math.Max( iTop, iIndex ) - Math.Min( iBottom, iBottom2 ) + 1;

      return ( iResult > 0 ) ? iResult : 0;
    }
    /// <summary>
    /// Indicates is when remove or insert use top\left row\column.
    /// </summary>
    /// <param name="iIndex">Represents start index of remove \ insert.</param>
    /// <param name="iCount">Represents count.</param>
    /// <param name="bIsRow">Indicates is in row.</param>
    /// <returns>If true - use first row\column; otherwise - false.</returns>
    private bool IndicatesFirst( int iIndex, int iCount, bool bIsRow )
    {
      int iFirst = ( bIsRow )
        ? TopRow
        : LeftColumn;

      int iLast = ( bIsRow )
        ? BottomRow
        : RightColumn;

      if( iFirst == iLast )
        return false;

      iFirst -= iIndex;

      return iFirst >= 0 && iFirst < iCount;
    }
    /// <summary>
    /// Updates row \ column indexes.
    /// </summary>
    /// <param name="iCount">Count to update.</param>
    /// <param name="bIsRow">Indicates is update row indexes or column.</param>
    private void UpdateAboveRowColumnIndexes( int iCount, bool bIsRow )
    {
      if( bIsRow )
      {
        ClientAnchor.TopRow += iCount;
        ClientAnchor.BottomRow += iCount;
        EvaluateTopPosition();
      }
      else
      {
        ClientAnchor.LeftColumn += iCount;
        ClientAnchor.RightColumn += iCount;
        EvaluateLeftPosition();
      }
    }
    /// <summary>
    /// Updates first row/column indexes.
    /// </summary>
    /// <param name="bIsRow">Indicates is in row.</param>
    /// <param name="iCount">Represents count.</param>
    private void UpdateFirstRowColumnIndexes( bool bIsRow, int iCount )
    {
      if( bIsRow )
      {
        ClientAnchor.TopOffset = 0;
        ClientAnchor.BottomRow += iCount;
        int iWidth = Width;
        EvaluateTopPosition();

        if( !IsSizeWithCell )
          Width = iWidth;
      }
      else
      {
        ClientAnchor.LeftOffset = 0;
        ClientAnchor.RightColumn += iCount;
        int iHeight = Height;
        EvaluateLeftPosition();

        if( !IsSizeWithCell )
          Height = iHeight;
      }

      if( !IsSizeWithCell )
        UpdateNotSizeNotMoveShape( bIsRow, 0, iCount );
    }
    /// <summary>
    /// Updates shape that include not size and not move flags.
    /// </summary>
    /// <param name="bRow">Indicates is row or column to update.</param>
    /// <param name="iIndex">Row or column index.</param>
    /// <param name="iCount">Number of inserted/removed rows/column.</param>
    protected virtual void UpdateNotSizeNotMoveShape( bool bRow, int iIndex, int iCount )
    {
      if( bRow )
      {
        UpdateTopRow();
        UpdateBottomRow();
      }
      else
      {
        UpdateLeftColumn();
        UpdateRightColumn();
      }
    }
    /// <summary>
    /// Updates inside row/column indexes.
    /// </summary>
    /// <param name="iCount">Represents count.</param>
    /// <param name="bRow">Indicates is in row.</param>
    private void UpdateInsideRowColumnIndexes( int iCount, bool bRow )
    {
      if( IsSizeWithCell )
      {
        if( bRow )
        {
          ClientAnchor.BottomRow += iCount;
          EvaluateTopPosition();
        }
        else
        {
          ClientAnchor.RightColumn += iCount;
          EvaluateRightPosition();
        }
      }
      else
      {
        UpdateNotSizeNotMoveShape( bRow, 0, iCount );
      }
    }
    /// <summary>
    /// Updates last row\column indexes.
    /// </summary>
    /// <param name="bRow">Indicates is row or column.</param>
    private void UpdateLastRowColumnIndex( bool bRow )
    {
      if( IsSizeWithCell )
      {
        if( bRow )
        {
          ClientAnchor.BottomOffset = 0;
          EvaluateTopPosition();
        }
        else
        {
          ClientAnchor.RightOffset = 0;
          EvaluateLeftPosition();
        }
      }
      else
      {
        UpdateNotSizeNotMoveShape( bRow, 0, 1 );
      }
    }
    /// <summary>
    /// Updates record.
    /// </summary>
    /// <param name="anchor">Represents client anchor.</param>
    private void UpdateRecord( MsofbtClientAnchor anchor )
    {
      if( anchor == null )
        throw new ArgumentNullException( "anchor" );

      MsofbtSpContainer container = m_record as MsofbtSpContainer;

      if( container == null )
      {
        m_clientAnchor = ( MsofbtClientAnchor )anchor.Clone();
      }
      else
      {
        IList list = container.ItemsList;

        for( int i = 0, iLen = list.Count; i < iLen; i++ )
        {
          MsoBase mso = list[ i ] as MsoBase;

          UpdateMso( mso );
        }
      }
    }
    /// <summary>
    /// Parses line and fill objects.
    /// </summary>
    /// <param name="options">Options holder.</param>
    private void ParseLineFill( MsofbtOPT options )
    {
      m_fill = new ShapeFillImpl( Application, this );
      m_fill.Visible = false;

      m_lineFormat = new ShapeLineFormatImpl( Application, this );
      m_lineFormat.Visible = false;

      m_bUpdateLineFill = true;

      if( options != null )
        ParseOptions( options );
    }
    /// <summary>
    /// Updates mso object.
    /// </summary>
    /// <param name="mso">Represents mso object to update.</param>
    /// <returns>Returns true if updated otherwise - false.</returns>
    [ CLSCompliant( false ) ]
    protected virtual bool UpdateMso( MsoBase mso )
    {
      if( mso == null )
        throw new ArgumentNullException( "mso" );

      if( mso is MsofbtClientAnchor )
      {
        m_clientAnchor = mso as MsofbtClientAnchor;

        return true;
      }

      if( mso is MsofbtClientData )
      {
        m_object = ( mso as MsofbtClientData ).ObjectRecord;

        return true;
      }

      if( mso is MsofbtOPT )
      {
        m_options = mso as MsofbtOPT;

        return true;
      }

      if( mso is MsofbtSp )
      {
        m_shape = mso as MsofbtSp;

        return true;
      }

      return false;
    }
    /// <summary>
    /// Clones line and fill objects.
    /// </summary>
    /// <param name="sourceShape">Shape to be cloned.</param>
    protected void CloneLineFill( ShapeImpl sourceShape )
    {
      if( sourceShape == null )
        throw new ArgumentNullException( "sourceShape" );

      if( m_bUpdateLineFill )
      {
        if( sourceShape.m_fill != null )
          m_fill = sourceShape.m_fill.Clone( this );

        if( sourceShape.m_lineFormat != null )
          m_lineFormat = sourceShape.m_lineFormat.Clone( this );
      }
    }
    #endregion

    #region Event handlers
    /// <summary>
    /// Column width change event.
    /// </summary>
    /// <param name="sender">Represents sender object</param>
    /// <param name="e">Represents value changed event.</param>
    private void Worksheet_ColumnWidthChanged( object sender, ValueChangedEventArgs e )
    {
      bool bFlag = m_clientAnchor == null || ( m_clientAnchor.LeftOffset == m_clientAnchor.RightOffset &&
        m_clientAnchor.RightOffset == m_clientAnchor.TopOffset && m_clientAnchor.TopOffset
        == m_clientAnchor.BottomOffset && 0 == m_clientAnchor.TopOffset );

      if( m_book.Loading || bFlag ) return;

      int iColumnIndex = ( int )e.oldValue;
      if( iColumnIndex > RightColumn ) return;

      if( iColumnIndex < LeftColumn )
      {
        if( IsMoveWithCell )
        {
          // Here we can optimize a little if we won't evaluate but just add offset.
          EvaluateLeftPosition();
        }
        else
        {
          UpdateLeftColumn();
          UpdateRightColumn();
        }
      }
      else if( iColumnIndex == LeftColumn )
      {
        // We have to make offsets in pixels relative to top-left
        // corner of this cell stay as it is (just update offsets)
        if( IsSizeWithCell )
        {
          UpdateLeftColumn();
          UpdateWidth();
        }
        else
        {
          UpdateLeftColumn();
          UpdateRightColumn();
        }
      }
      else if( iColumnIndex == RightColumn )
      {
        if( IsSizeWithCell )
        {
          LeaveRelativeBottomRightCorner();
        }
        else
        {
          UpdateRightColumn();
        }
      }
      else // More than left and less than right.
      {
        if( IsSizeWithCell )
        {
          UpdateWidth();
        }
        else
        {
          UpdateRightColumn();
        }
      }
    }
    /// <summary>
    /// Leaves bottom right corner of the shape.
    /// </summary>
    private void LeaveRelativeBottomRightCorner()
    {
      if( m_book.Loading ) return;

      WorksheetImpl sheet = m_shapes.Worksheet;

      if( sheet == null ) return;

      int iWidth = GetWidth( LeftColumn, LeftColumnOffset, RightColumn, RightColumnOffset, false );
      int iColSize = sheet.GetColumnWidthInPixels( RightColumn );

      RightColumnOffset += ConvertPixelsIntoWidthOffset( Math.Min( Width - iWidth, iColSize ), iColSize );

      UpdateWidth();
    }
    /// <summary>
    /// This method should be called after any change in font of "Normal" style.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void NormalFont_OnAfterChange( object sender, EventArgs e )
    {
      if( m_book.Loading ) return;

      if( !IsMoveWithCell )
      {
        UpdateLeftColumn();
        UpdateTopRow();
      }
      else
      {
        EvaluateTopLeftPosition();
      }

      if( !IsSizeWithCell )
      {
        UpdateRightColumn();
        UpdateBottomRow();
      }
      else
      {
        UpdateWidth();
        UpdateHeight();
      }
    }
    /// <summary>
    /// This method should be called after row height changes.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void Worksheet_RowHeightChanged( object sender, ValueChangedEventArgs e )
    {
      if( m_book.Loading ) return;

      int iRowIndex = ( int )e.oldValue;
      if( iRowIndex > BottomRow ) return;

      if( iRowIndex < TopRow )
      {
        if( IsMoveWithCell )
        {
          // Here we can optimize a little if we won't evaluate but just add offset.
          EvaluateTopPosition();
        }
        else
        {
          UpdateTopRow();
          UpdateBottomRow();
        }
      }
      else if( iRowIndex == TopRow )
      {
        // We have to make offsets in pixels relative to top-Top
        // corner of this cell stay as it is (just update offsets)
        if( IsSizeWithCell )
        {
          UpdateTopRow();
          UpdateHeight();
        }
        else
        {
          UpdateTopRow();
          UpdateBottomRow();
        }
      }
      else if( iRowIndex == BottomRow )
      {
        if( IsSizeWithCell )
        {
          LeaveRelativeBottomRightCorner();
        }
        else
        {
          UpdateBottomRow();
        }
      }
      else // More than top and less than bottom.
      {
        if( IsSizeWithCell )
        {
          UpdateHeight();
        }
        else
        {
          UpdateBottomRow();
        }
      }
    }
    /// <summary>
    /// Checks the Left Offset.
    /// </summary>
    internal void CheckLeftOffset()
    {
        int leftOffset = ClientAnchor.LeftOffset;
        int column = ClientAnchor.LeftColumn+1;
        int columnWidth = m_shapes.Worksheet.GetColumnWidthInPixels(column);
        int offsetInPixels = OffsetInPixels(column, leftOffset, true);
        if (columnWidth < offsetInPixels)
        {
            ClientAnchor.LeftColumn++;
            ClientAnchor.LeftOffset = offsetInPixels - column;
        }
    }
    #endregion
  }
}
