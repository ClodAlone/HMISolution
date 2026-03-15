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

//#define MEASURE_PERFORMANCE

#region file using directives
using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;

using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Shapes;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using System.IO;
using System.Collections.Generic;
using System.Xml;

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
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
using Syncfusion.Compression.Zip;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation
{
	/// <summary>
	/// Base class for all worksheet objects (objects that have own tab in the workbook)
	/// like worksheets and charts.
	/// </summary>
  public abstract class WorksheetBaseImpl
    : CommonObject
    , INamedObject
    , IParseable
    , ITabSheet
    , ICloneParent
  {
    #region Class constants
    /// <summary>
    /// Records that do not belong to mso part of the worksheet.
    /// </summary>
    private static readonly TBIFFRecord[] DEF_NOTMSORECORDS = new TBIFFRecord[]
    {
      TBIFFRecord.PivotViewDefinition,
      TBIFFRecord.Note,
      TBIFFRecord.WindowTwo,
      ( TBIFFRecord )2128,
      ( TBIFFRecord )2150,
      ( TBIFFRecord )237,
      TBIFFRecord.ChartUnits,
      TBIFFRecord.ChartChart,
      TBIFFRecord.DCON,
    };
    /// <summary>
    /// Maximum length of the password.
    /// </summary>
    public const int DEF_MAX_PASSWORDLEN = 255;
    /// <summary>
    /// Default password hash value.
    /// </summary>
    private const ushort DEF_PASSWORD_CONST = 0xCE4B;
    /// <summary>
    /// Min column index.
    /// </summary>
    [ CLSCompliant( false ) ]
    public const int DEF_MIN_COLUMN_INDEX = int.MaxValue;
    /// <summary>
    /// Min row index.
    /// </summary>
    public const int DEF_MIN_ROW_INDEX = -1;
    /// <summary>
    /// Default tab color.
    /// </summary>
    public const ExcelKnownColors DEF_DEFAULT_TAB_COLOR = ( ExcelKnownColors )(-1);
    /// <summary>
    /// Default tab color.
    /// </summary>
    private static readonly Color DEF_DEFAULT_TAB_COLOR_RGB = ColorExtension.Empty;
    /// <summary>
    /// Maximum one-based index of the row.
    /// </summary>
    [Obsolete( "This constant is obsolete and will be removed soon. Please, use MaxRowCount property of the IWorkbook interface. Sorry for inconvenience." )]
    public const int DEF_MAX_ROW_ONE_INDEX = 65536;
    /// <summary>
    /// Maximum one-based index of the column.
    /// </summary>
    [ Obsolete( "This constant is obsolete and will be removed soon. Please, use MaxColumnCount property of the IWorkbook interface. Sorry for inconvenience." ) ]
    public const int DEF_MAX_COLUMN_ONE_INDEX = 256;
    /// <summary>
    /// Maximum allowed worksheet name length.
    /// </summary>
    private const int MaxSheetNameLength = 31;
    #endregion

    #region Class members
    /// <summary>
    /// Represents a boolean value to parse worksheets on demand
    /// </summary>
    private bool m_bParseOnDemand;
    /// <summary>
    /// Represents to parse sheet on demand
    /// </summary>
    private bool m_bParseDataOnDemand;
    /// <summary>
    /// Parent workbook.
    /// </summary>
    protected WorkbookImpl m_book;
    /// <summary>
    /// Worksheet's name.
    /// </summary>
    private string m_strName = string.Empty;
    /// <summary>
    /// Indicates whether worksheet was changed and need saving.
    /// </summary>
    private bool m_bChanged = true;
    /// <summary>
    /// Real worksheet's index in the workbook.
    /// </summary>
    private int m_iRealIndex;
    /// <summary>
    /// Starting index for Mso record.
    /// </summary>
    protected int m_iMsoStartIndex = -1;
    /// <summary>
    /// Current index of the mso drawing object.
    /// </summary>
    private int m_iCurMsoIndex;
    /// <summary>
    /// Parse options.
    /// </summary>
    protected ExcelParseOptions m_parseOptions;
    /// <summary>
    /// List with all mso drawing records in the worksheet.
    /// </summary>
    private List<BiffRecordRaw>       m_arrMSODrawings;
    /// <summary>
    /// List in which all records which belong to worksheet are stored.
    /// </summary>
    protected List<BiffRecordRaw>       m_arrRecords = new List<BiffRecordRaw>();
    /// <summary>
    /// Collection with all shapes in the worksheet.
    /// </summary>
    private ShapesCollection m_shapes;
    /// <summary>
    /// Charts collection.
    /// </summary>
    private WorksheetChartsCollection m_charts;
    /// <summary>
    /// Collection of all pictures in the worksheet.
    /// </summary>
    private PicturesCollection m_pictures;
    /// <summary>
    /// True - indicates that record extracted from stream can be used, by this
    /// implementation, otherwise False.
    /// </summary>
    private bool            m_bIsSupported = true;
    /// <summary>
    /// Zoom of the current window.
    /// </summary>
    private int m_iZoom = 100;
    /// <summary>
    /// Represents sheet protection record.
    /// </summary>
    private SheetProtectionRecord m_sheetProtection;
    /// <summary>
    /// Represents Range Protection Record
    /// </summary>
    protected RangeProtectionRecord m_rangeProtectionRecord;
    /// <summary>
    /// Describes password entered by user to protect worksheet's data.
    /// </summary>
    private PasswordRecord m_password;
    /// <summary>
    /// Code name.
    /// </summary>
    protected string          m_strCodeName;
    /// <summary>
    /// Indicates whether worksheet was parsed.
    /// </summary>
    private bool m_bParsed = true;
    /// <summary>
    /// Indicates whether object is currently being parsed.
    /// </summary>
    private bool m_bParsing;
    /// <summary>
    /// Indicates whether to skip parsing.
    /// </summary>
    private bool m_bSkipParsing;
    /// <summary>
    /// Window two record.
    /// </summary>
    private WindowTwoRecord m_windowTwo;
    /// <summary>
    /// Pagelayout view record.
    /// </summary>
    private PageLayoutView m_layout;
    /// <summary>
    /// One field from Worksheet dimension records.
    /// </summary>
    [ CLSCompliant( false ) ]
    protected int             m_iFirstColumn = DEF_MIN_COLUMN_INDEX;
    /// <summary>
    /// One field from Worksheet dimension records.
    /// </summary>
    protected int             m_iLastColumn = DEF_MIN_COLUMN_INDEX;
    /// <summary>
    /// One field from Worksheet dimension records.
    /// </summary>
    protected int             m_iFirstRow = DEF_MIN_ROW_INDEX;
    /// <summary>
    /// One field from Worksheet dimension records
    /// </summary>
    protected int             m_iLastRow = DEF_MIN_ROW_INDEX;
    /// <summary>
    /// Index of the tab color.
    /// </summary>
    private ColorObject m_tabColor;
    /// <summary>
    /// Contains images used in header and footer.
    /// </summary>
    private HeaderFooterShapeCollection m_headerFooterShapes;
    /// <summary>
    /// Worksheet index.
    /// </summary>
    private int             m_iIndex;
    ///// <summary>
    ///// Indicates is current sheet is protected.
    ///// </summary>
    //private bool            m_bIsProtected;
    /// <summary>
    /// Represents sheet protection for support old version. Using for parsing.
    /// </summary>
    private ExcelSheetProtection m_parseProtection = ( ExcelSheetProtection )( -1 );
    /// <summary>
    /// BOF record that will be serialized.
    /// </summary>
    internal BOFRecord m_bof = ( BOFRecord )BiffRecordFactory.GetRecord( TBIFFRecord.BOF );
    /// <summary>
    /// Indicates whether to keep record inside of internal storage.
    /// </summary>
    protected bool KeepRecord = false;
    /// <summary>
    /// Visibility of worksheet.
    /// </summary>
    private WorksheetVisibility m_visiblity = WorksheetVisibility.Visible;
    /// <summary>
    /// Data holder for 2007 Excel.
    /// </summary>
    protected internal WorksheetDataHolder m_dataHolder;
    /// <summary>
    /// Indicates whether worksheet contains some unknown vml shapes.
    /// </summary>
    private bool m_bUnknownVmlShapes;
    /// <summary>
    /// Collection with all textboxes.
    /// </summary>
    private TextBoxCollection m_textBoxes;
    /// <summary>
    /// Collection with all checkboxes.
    /// </summary>
    private CheckBoxCollection m_checkBoxes;
    /// <summary>
    /// Collection with all OptionButton.
    /// </summary>
    private OptionButtonCollection m_optionButtons;
    /// <summary>
    /// Collection with all comboboxes.
    /// </summary>
    private ComboBoxCollection m_comboBoxes;
    private bool m_bTransitionEvaluation;
    /// <summary>
    /// Parse MSO Drawings if worksheet is parsed on demand
    /// </summary>
    internal bool m_bParseMSODrawings;
    /// <summary>
    /// Custom height
    /// </summary>
    protected bool m_isCustomHeight = false;
    /// <summary>
    /// 
    /// </summary>
    BiffRecordRaw m_previousRecord;
    /// <summary>
    /// Collection of worksheet's error indicators.
    /// </summary>
    protected ErrorIndicatorsCollection m_errorIndicators;
    /// <summary>
    /// Indicate whether sheet contain tab color rgb
    /// </summary>
    private bool m_bTabColorRGB;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance and sets its application and parent properties.
    /// </summary>
    /// <param name="application">Application object for the new instance.</param>
    /// <param name="parent">Parent object for the new instance.</param>
    public WorksheetBaseImpl( IApplication application, object parent )
      : base( application, parent )
    {
      FindParents();
      InitializeCollections();
    }
    /// <summary>
    /// Initializes new instance and sets its application and parent properties.
    /// </summary>
    /// <param name="application">Application object for the new instance.</param>
    /// <param name="parent">Parent object for the new instance.</param>
    /// <param name="reader">BiffReader to extract data from.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="bSkipParsing">Indicates whether to skip parsing.</param>
    /// <param name="hashNewXFormatIndexes">
    /// Dictionary with new extended format indexes for ignore styles mode.
    /// </param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    [ CLSCompliant( false ) ]
    public WorksheetBaseImpl( IApplication application, object parent, BiffReader reader,
      ExcelParseOptions options, bool bSkipParsing, Dictionary<int, int> hashNewXFormatIndexes, IDecryptor decryptor )
      : this( application, parent )
    {
      KeepRecord = true;
      Parse( reader, options, bSkipParsing, hashNewXFormatIndexes, decryptor );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns or sets the name of the object. Read / write String.
    /// </summary>
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
          int iLength = value.Length;
          if( value[ 0 ] == '\'' || value[ iLength - 1 ] == '\'' )
          {
            throw new ArgumentOutOfRangeException( "Apostrophe can't be used as first and/or last character of the worksheet's name." );
          }

          if( value.Length > MaxSheetNameLength )
          {
            value = value.Substring( 0, MaxSheetNameLength );
            value = GenerateUniqueName( GetName, value );
          }

          ValueChangedEventArgs args = new ValueChangedEventArgs( m_strName, value, "Name" );
          m_strName = value;
          OnNameChanged( args );
          // TODO: add event handler with following code.
          //m_book.InnerNamesColection.IsWorkbookNamesChanged = true;
        }
      }
    }
    /// <summary>
    /// Indicates whether worksheet was saved.
    /// </summary>
    public bool IsSaved
    {
      get
      {
        return !m_bChanged;
      }
      set
      {
        m_bChanged = !value;
      }
    }
    /// <summary>
    /// Returns comments collection for this worksheet. Read-only.
    /// </summary>
    protected internal CommentsCollection InnerComments
    {
      get
      {
        return m_shapes.InnerComments;
      }
    }
    /// <summary>
    /// Returns pictures collection. Read-only.
    /// </summary>
    protected internal PicturesCollection InnerPictures
    {
      get
      {
        if( m_pictures == null )
          m_pictures = new PicturesCollection( Application, this );

        return m_pictures;
      }
    }
    /// <summary>
    /// Returns embedded charts collection. Read-only.
    /// </summary>
    protected internal WorksheetChartsCollection InnerCharts
    {
      get
      {
        CheckParseOnDemand();

        if( m_charts == null )
          m_charts = new WorksheetChartsCollection( Application, this );

        return m_charts;
      }
    }
    /// <summary>
    /// Returns shapes collection. Read-only.
    /// </summary>
    protected internal ShapesCollection InnerShapes
    {
      [ DebuggerStepThrough ]
      get
      {
        CheckParseOnDemand();

        return m_shapes;
      }
    }
    /// <summary>
    /// Shapes collection.
    /// </summary>
    public IShapes      Shapes
    {
      get
      {
          if(m_shapes == null)
              CheckParseOnDemand();

        return m_shapes;
      }
    }
    /// <summary>
    /// Returns InnerShapes base collection.
    /// </summary>
    public ShapeCollectionBase      InnerShapesBase
    {
      get
      {
          if (m_shapes == null)
              CheckParseOnDemand();

          return m_shapes;
      }
      internal set
      {
        m_shapes = ( ShapesCollection )value;
      }
    }
    /// <summary>
    /// Header / footer shapes collection.
    /// </summary>
    public HeaderFooterShapeCollection      HeaderFooterShapes
    {
      get
      {
        CheckParseOnDemand();

        if( m_headerFooterShapes == null )
          m_headerFooterShapes = new HeaderFooterShapeCollection( Application, this );

        return m_headerFooterShapes;
      }
    }
    /// <summary>
    /// Header / footer shapes collection.
    /// </summary>
    public HeaderFooterShapeCollection InnerHeaderFooterShapes
    {
      get
      {
        CheckParseOnDemand();

        return m_headerFooterShapes;
      }
      internal set
      {
        m_headerFooterShapes = value;
      }
    }
    /// <summary>
    /// Returns comments collection for this worksheet. Read-only.
    /// </summary>
    public IComments    Comments
    {
      get
      {
        CheckParseOnDemand();

        return m_shapes.Comments;
      }
    }
    /// <summary>
    /// Returns charts collection. Read-only.
    /// </summary>
    public IChartShapes Charts
    {
      get
      {
        CheckParseOnDemand();

        if( m_charts == null )
          m_charts = new WorksheetChartsCollection( Application, this );

        return m_charts;
      }
    }
    /// <summary>
    /// Pictures collection. Read-only.
    /// </summary>
    public IPictures    Pictures
    {
      get
      {
        CheckParseOnDemand();

        if( m_pictures == null )
          m_pictures = new PicturesCollection( Application, this );

        return m_pictures;
      }
    }
    /// <summary>
    /// Name used by macros to access workbook items. Read-only.
    /// </summary>
    public string       CodeName
    {
      get
      {
        return ( m_strCodeName != null )
          ? m_strCodeName
          : m_strName;
      }
      internal set
      {
        m_strCodeName = value;
      }
    }
    /// <summary>
    /// Sheet window settings.
    /// </summary>
    [ CLSCompliant( false ) ]
    public WindowTwoRecord  WindowTwo
    {
      get
      {
        if( m_windowTwo == null )
        {
          m_windowTwo = ( WindowTwoRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.WindowTwo );
        }
        if (BOF != null && BOF.Type == BOFRecord.TType.TYPE_CHART)
            m_windowTwo.OriginalLength = WindowTwoRecord.DEF_MAX_CHART_SHEET_SIZE;
        return m_windowTwo;
      }
    }
    /// <summary>
    /// Indicates is current sheet is protected.
    /// </summary>
    public virtual bool    ProtectContents
    {
      get
      {
        return ( InnerProtection & ExcelSheetProtection.Content ) != 0;
        //return m_bIsProtected;
      }
      internal set
      {
        //m_bIsProtected = value;
        if( value )
        {
          InnerProtection |= ExcelSheetProtection.Content;
        }
        else
        {
          InnerProtection &= ~ExcelSheetProtection.Content;
        }
      }
    }
    /// <summary>
    /// True if objects are protected. Read-only.
    /// </summary>
    public virtual bool ProtectDrawingObjects
    {
      get
      {
        return ProtectContents && ( InnerProtection & ExcelSheetProtection.Objects ) == 0;
      }
    }
    /// <summary>
    /// True if the scenarios of the current sheet are protected. Read-only.
    /// </summary>
    public virtual bool ProtectScenarios
    {
      get
      {
        return ProtectContents && ( InnerProtection & ExcelSheetProtection.Scenarios ) == 0;
      }
    }
    /// <summary>
    /// Gets a value indicating whether worksheet is protected with password.
    /// </summary>
    public bool IsPasswordProtected
    {
      get
      {
        CheckParseOnDemand();

        if (m_parseProtection.ToString() != "-1" && !(Workbook as WorkbookImpl).Saving)
            return true;
        else
            return ( m_password != null && m_password.IsPassword != 0 );
      }
    }
    /// <summary>
    /// Indicates whether object was parsed.
    /// </summary>
    public bool IsParsed
    {
      get
      {
        return m_bParsed;
      }
      set
      {
        m_bParsed = value;
      }
    }
    /// <summary>
    /// Indicates whether object is currently being parsed.
    /// </summary>
    public bool IsParsing
    {
      get
      {
        return m_bParsing;
      }
      set
      {
        m_bParsing = value;
      }
    }
    /// <summary>
    /// Indicates whether worksheet was opened in skip parsing mode. Read-only.
    /// </summary>
    public bool IsSkipParsing
    {
      get
      {
        return m_bSkipParsing;
      }
    }
    /// <summary>
    /// Indicates whether worksheet type is supported. Read-only.
    /// </summary>
    public bool IsSupported
    {
      get
      {
        return m_bIsSupported;
      }
      protected set
      {
        m_bIsSupported = value;
      }
    }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    public WorkbookImpl ParentWorkbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Gets / sets one-based index of the first row of the worksheet.
    /// </summary>
    public virtual int FirstRow
    {
      get
      {
        ParseData();
        return m_iFirstRow;
      }
      set
      {
        m_iFirstRow = value;
      }
    }

    /// <summary>
    /// Gets / sets one-based index of the first column of the worksheet.
    /// </summary>
    [ CLSCompliant( false ) ]
    public virtual int FirstColumn
    {
      get
      {
        ParseData();
        return m_iFirstColumn;
      }
      set
      {
        m_iFirstColumn = value;
      }
    }
    /// <summary>
    /// Gets / sets one-based index of the last row of the worksheet.
    /// </summary>
    public virtual int LastRow
    {
      get
      {
        ParseData();
        return m_iLastRow;
      }
      set
      {
        m_iLastRow = value;
      }
    }

    /// <summary>
    /// Gets / sets one-based index of the last column of the worksheet.
    /// </summary>
    public virtual int LastColumn
    {
      get
      {
        ParseData();
        return m_iLastColumn;
      }
      set
      {
        m_iLastColumn = value;
      }
    }
    /// <summary>
    /// Zoom factor of document. Value must be in range from 10 to 400.
    /// </summary>
    public int Zoom
    {
      get
      {
        //return m_windowZoom.Zoom;
        return m_iZoom;
      }
      set
      {
        if( value < 10 || value > 400 )
          throw new ArgumentOutOfRangeException( "Zoom", "Zoom must be in range from 10 till 400." );

        //m_windowZoom.Zoom = value;
        m_iZoom = value;
      }
    }
    /// <summary>
    /// Tab color.
    /// </summary>
    public virtual ColorObject TabColorObject
    {
      get
      {
        if( m_tabColor == null )
          m_tabColor = new ColorObject( DEF_DEFAULT_TAB_COLOR );

        return m_tabColor;
      }
    }
    /// <summary>
    /// Tab color.
    /// </summary>
    public virtual ExcelKnownColors TabColor
    {
      get
      {
        return ( m_tabColor != null ) ?
          m_tabColor.GetIndexed( m_book ) :
          DEF_DEFAULT_TAB_COLOR;
      }
      set
      {
        if( m_tabColor == null )
          m_tabColor = new ColorObject( ExcelKnownColors.None );

        m_tabColor.SetIndexed( value );
      }
    }
    /// <summary>
    /// Tab color.
    /// </summary>
    public virtual Color TabColorRGB
    {
      get
      {
        return ( m_tabColor != null ) ?
          m_tabColor.GetRGB( m_book ) :
          DEF_DEFAULT_TAB_COLOR_RGB;
      }
      set
      {
        if( m_tabColor == null )
          m_tabColor = new ColorObject( ExcelKnownColors.None );

        m_bTabColorRGB = true;
        m_tabColor.SetRGB( value, m_book );
      }
    }
    /// <summary>
    /// Grid line color.
    /// </summary>
    public ExcelKnownColors GridLineColor
    {
      get
      {
        return ( ExcelKnownColors )m_windowTwo.HeaderColor;
      }
      set
      {
        WindowTwo.IsDefaultHeader = false;
        WindowTwo.HeaderColor = ( int )value;
      }
    }
    /// <summary>
    /// Indicates whether gridline color has default value.
    /// </summary>
    public bool DefaultGridlineColor
    {
      get
      {
        return WindowTwo.IsDefaultHeader;
      }
      set
      {
        WindowTwo.IsDefaultHeader = value;
      }
    }
    /// <summary>
    /// Get parent workbook of current worksheet.
    /// </summary>
    public IWorkbook Workbook
    {
      get
      {
        return m_book;
      }
    }
    /// <summary>
    /// Indicates whether worksheet is displayed right to left.
    /// </summary>
    public bool IsRightToLeft
    {
      get
      {
        return WindowTwo.IsArabic;
      }
      set
      {
        WindowTwo.IsArabic = value;
      }
    }
    /// <summary>
    /// Return page setup. Read-only.
    /// </summary>
    public abstract PageSetupBaseImpl PageSetupBase { get; }
    /// <summary>
    /// Indicates whether tab of this sheet is selected. Read-only.
    /// </summary>
    public bool IsSelected
    {
      get
      {
        return WindowTwo.IsSelected;
      }
    }
    /// <summary>
    /// Returns the index number of the object within the collection of
    /// similar objects. Read-only.
    /// </summary>
    public int          Index
    {
      get
      {
        return m_iIndex;
      }
      set
      {
        m_iIndex = value;
      }
    }
    /// <summary>
    /// Gets protected options. Read-only. For sets protection options use "Protect" method.
    /// </summary>
    public virtual ExcelSheetProtection Protection
    {
      get
      {
        return ( m_sheetProtection != null )
          ? ( ExcelSheetProtection )m_sheetProtection.ProtectedOptions
          : ExcelSheetProtection.None;
      }
    }
    /// <summary>
    /// Gets protected options. Read-only. For sets protection options use "Protect" method.
    /// </summary>
    protected internal virtual ExcelSheetProtection InnerProtection
    {
      get
      {
        return ( m_sheetProtection != null )
          ? ( ExcelSheetProtection )m_sheetProtection.ProtectedOptions
          : UnprotectedOptions;
      }
      internal set
      {
        m_sheetProtection.ProtectedOptions = ( int )value;
      }
    }
    protected virtual ExcelSheetProtection UnprotectedOptions
    {
      get
      {
        return ExcelSheetProtection.None;
      }
    }
    /// <summary>
    /// Returns internal BOF record. Read-only.
    /// </summary>
    internal BOFRecord BOF
    {
      get
      {
        return m_bof;
      }
    }
    /// <summary>
    /// Controls end user visibility of worksheet.
    /// </summary>
    public WorksheetVisibility Visibility
    {
      get
      {
        return m_visiblity;
      }
      set
      {
        if( Visibility != value )
        {
            WorksheetVisibility oldVisibilty = Visibility;
            m_visiblity = value;

          if( !m_book.Loading && value != WorksheetVisibility.Visible )
          {
            int iIndex = RealIndex;
            WorkbookObjectsCollection objects = m_book.Objects;

            for( int i = iIndex + 1, iLen = objects.Count; i < iLen; i++ )
            {
              if( FindUnhided( objects, i ) )
                return;
            }

            for( int i = iIndex - 1; i >= 0; i-- )
            {
              if( FindUnhided( objects, i ) )
                return;
            }
            m_visiblity = oldVisibilty;
            throw new NotSupportedException( "A workbook must contain at least one visible worksheet." );
          }
        }
      }
    }
    /// <summary>
    /// Gets / sets worksheet data holder.
    /// </summary>
    internal WorksheetDataHolder DataHolder
    {
      get
      {
        return m_dataHolder;
      }
      set
      {
        m_dataHolder = value;

        if( value != null )
        {
          m_bParsed = false;
        }
      }
    }
    /// <summary>
    /// Gets/sets top visible row of the worksheet.
    /// </summary>
    public int TopVisibleRow
    {
      get
      {
        return WindowTwo.TopRow + 1;
      }
      set
      {
        if( value <= 0 )
          throw new ArgumentOutOfRangeException();

        WindowTwo.TopRow = ( ushort )( value - 1 );
      }
    }
    /// <summary>
    /// Gets/sets left visible column of the worksheet.
    /// </summary>
    public int LeftVisibleColumn
    {
      get
      {
        return WindowTwo.LeftColumn + 1;
      }
      set
      {
        if( value <= 0 )
          throw new ArgumentOutOfRangeException();

        WindowTwo.LeftColumn = ( ushort )( value - 1 );
      }
    }
    /// <summary>
    /// Describes password entered by user to protect worksheet's data.
    /// </summary>
    internal PasswordRecord Password
    {
      get
      {
        return m_password;
      }
    }
    /// <summary>
    /// Indicates whether worksheet contains some unknown vml shapes.
    /// </summary>
    public bool UnknownVmlShapes
    {
      get
      {
        return m_bUnknownVmlShapes;
      }
      set
      {
        m_bUnknownVmlShapes = value;
      }
    }
    /// <summary>
    /// Returns inner textboxes collection. Read-only.
    /// </summary>
    public TextBoxCollection TypedTextBoxes
    {
      get
      {
        if( m_textBoxes == null )
          m_textBoxes = new TextBoxCollection( Application, this );

        return m_textBoxes;
      }
    }
    /// <summary>
    /// Returns inner textboxes collection. Read-only.
    /// </summary>
    internal TextBoxCollection InnerTextBoxes
    {
      get
      {
        CheckParseOnDemand();

        return m_textBoxes;
      }
    }
    /// <summary>
    /// Returns inner textboxes collection. Read-only.
    /// </summary>
    public ITextBoxes TextBoxes
    {
      get
      {
        return TypedTextBoxes;
      }
    }
    /// <summary>
    /// Returns inner checkboxes collection. Read-only.
    /// </summary>
    public CheckBoxCollection TypedCheckBoxes
    {
      get
      {
        CheckParseOnDemand();

        if( m_checkBoxes == null )
          m_checkBoxes = new CheckBoxCollection( Application, this );

        return m_checkBoxes;
      }
    }
    /// <summary>
    /// Returns inner checkboxes collection. Read-only.
    /// </summary>
    public OptionButtonCollection TypedOptionButtons
    {
        get
        {
            if (m_optionButtons == null)
                m_optionButtons = new OptionButtonCollection(Application, this);

            return m_optionButtons;
        }
    }
    /// <summary>
    /// Returns inner comboboxes collection. Read-only.
    /// </summary>
    public ComboBoxCollection TypedComboBoxes
    {
      get
      {
        CheckParseOnDemand();

        if( m_comboBoxes == null )
          m_comboBoxes = new ComboBoxCollection( Application, this );

        return m_comboBoxes;
      }
    }
    /// <summary>
    /// Returns checkboxes collection for this worksheet. Read-only.
    /// </summary>
    protected internal CheckBoxCollection InnerCheckBoxes
    {
      get
      {
        return m_checkBoxes;
      }
    }
    /// <summary>
    /// Returns inner checkboxes collection. Read-only.
    /// </summary>
    public ICheckBoxes CheckBoxes
    {
      get
      {
        return TypedCheckBoxes;
      }
    }
    /// <summary>
    /// Returns inner checkboxes collection. Read-only.
    /// </summary>
    public IOptionButtons  OptionButtons
    {
        get
        {
            return TypedOptionButtons;
        }
    }
    /// <summary>
    /// Returns collection with all comboboxes inside this worksheet. Read-only.
    /// </summary>
    public IComboBoxes ComboBoxes
    {
      get
      {
        return TypedComboBoxes;
      }
    }
    /// <summary>
    /// Indicates whether tabsheet contains any picture. Read-only.
    /// </summary>
    public bool HasPictures
    {
      get
      {
        return m_pictures != null && m_pictures.Count > 0;
      }
    }
    /// <summary>
    /// Indicates whether worksheet has vml shapes. Read-only.
    /// </summary>
    public bool HasVmlShapes
    {
      get
      {
        return UnknownVmlShapes ||
          m_checkBoxes != null && m_checkBoxes.Count > 0 ||
          InnerComments != null && InnerComments.Count > 0 ||
		  m_optionButtons != null && m_optionButtons.Count > 0 ||
          FindVmlShape();
      }
    }

    private bool FindVmlShape()
    {
      for( int i = 0, len = m_shapes.Count; i < len; i++ )
      {
        if( ( m_shapes[ i ] as ShapeImpl ).VmlShape )
          return true;
      }

      return false;
    }
    /// <summary>
    /// Returns number of known vml shapes. Read-only.
    /// </summary>
    public int VmlShapesCount
    {
      get
      {
        int iResult = 0;

        for( int i = 0, len = m_shapes.Count; i < len; i++ )
        {
          if( ( m_shapes[ i ] as ShapeImpl ).VmlShape )
            iResult++;
        }

        return iResult;
        //int iCheckBoxCount = ( m_checkBoxes != null ) ? m_checkBoxes.Count : 0;
        //int iCommentsCount = ( InnerComments != null ) ? InnerComments.Count : 0;
        //int iComboBoxCount = ( m_comboBoxes != null ) ? m_comboBoxes.Count : 0;
        //return iCheckBoxCount + iCommentsCount + iComboBoxCount;
      }
    }
    /// <summary>
    /// Gets default protection options for the worksheet.
    /// </summary>
    protected abstract ExcelSheetProtection DefaultProtectionOptions
    {
      get;
    }
    /// <summary>
    /// Indicates whether Protection property is direct (specified items are
    /// protected) or indirect (specified items are unprotected).
    /// </summary>
    private bool ProtectionMeaningDirect
    {
      get
      {
        return ( DefaultProtectionOptions & ExcelSheetProtection.Content ) != 0;
      }
    }
    /// <summary>
    /// Inidicates whether protection should be serialized.
    /// </summary>
    protected virtual bool ContainsProtection
    {
      get
      {
        return m_sheetProtection != null &&
          ( m_sheetProtection.ProtectedOptions != ( int )( ExcelSheetProtection.LockedCells | ExcelSheetProtection.UnLockedCells ) );
      }
    }
    /// <summary>
    /// Returns sheet protection record.
    /// </summary>
    protected SheetProtectionRecord SheetProtection
    {
      get
      {
        return m_sheetProtection;
      }
    }
    public bool IsTransitionEvaluation
    {
      get
      {
        return m_bTransitionEvaluation;
      }
      set
      {
        m_bTransitionEvaluation = value;
      }
    }
    /// <summary>
    /// Gets or Sets a boolean value to parse worksheets on demand
    /// </summary>
    public bool ParseOnDemand
    {
        get
        {
            return m_bParseOnDemand;
        }
        set
        {
            m_bParseOnDemand = value;
        }
    }
    /// <summary>
    /// Gets or sets the boolean value to load worksheets on demand
    /// </summary>
    virtual internal bool ParseDataOnDemand
    {
        get
        {
            return m_bParseDataOnDemand;
        }
        set
        {
            m_bParseDataOnDemand = value;
        }
    }
    /// <summary>
    /// Indicate whether sheet contain tab color rgb
    /// </summary>
    internal bool HasTabColorRGB
    {
        get 
        {
            return m_bTabColorRGB;
        }
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Clears NameChanged event.
    /// </summary>
    internal void ClearEvents()
    {
      NameChanged = null;
    }
    /// <summary>
    /// Returns true if visible sheet found. Also sets active sheet index and display tab.
    /// </summary>
    /// <param name="objects">Workbook objects collection </param>
    /// <param name="iIndex">Object index.</param>
    /// <returns>True if visible sheet found</returns>
    private bool FindUnhided( WorkbookObjectsCollection objects, int iIndex )
    {
      WorksheetBaseImpl sheet = ( WorksheetBaseImpl )objects[ iIndex ];

      if( sheet.Visibility == WorksheetVisibility.Visible )
      {
        m_book.ActiveSheetIndex = iIndex;
        m_book.DisplayedTab = iIndex;

        return true;
      }

      return false;
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    protected virtual void FindParents()
    {
      object result = FindParent( typeof( WorkbookImpl ) );

      if( result == null )
        throw new ApplicationException( "Worksheet must be a member of Workbook object tree" );

      m_book = ( WorkbookImpl )result;
      m_iRealIndex = m_book.ObjectCount;
    }
    /// <summary>
    /// This method is called when Name of the worksheet was changed.
    /// </summary>
    /// <param name="args">Event arguments.</param>
    protected virtual void OnNameChanged( ValueChangedEventArgs args )
    {
      RaiseNameChangedEvent( args );
      SetChanged();
    }
    /// <summary>
    /// This method raises NameChanged event.
    /// </summary>
    /// <param name="args">Event arguments.</param>
    protected void RaiseNameChangedEvent( ValueChangedEventArgs args )
    {
      if( NameChanged != null )
      {
        NameChanged( this, args );
      }
    }
    /// <summary>
    /// This method should be called after any changes in the worksheet.
    /// Sets Saved property of the parent workbook to false.
    /// </summary>
    public void SetChanged()
    {
      if( m_book.Loading ) return;

      m_book.Saved = false;
      m_book.IsCellModified = true;
      IsSaved = false;
    }
    /// <summary>
    /// Initializes all required collections.
    /// </summary>
    protected virtual void InitializeCollections()
    {
      m_shapes = new ShapesCollection( Application, this );
      //m_headerFooterShapes = new HeaderFooterShapeCollection( Application, this );
      //m_arrMSODrawings = new List<BiffRecordRaw>();
      //m_charts = new WorksheetChartsCollection( Application, this );
      m_pictures = new PicturesCollection( Application, this );
    }
    /// <summary>
    /// Clear all internal collections.
    /// </summary>
    /// <param name="flags">Allows to avoid clearing of some properties.</param>
    protected virtual void ClearAll( ExcelWorksheetCopyFlags flags )
    {
      if( m_arrMSODrawings != null ) m_arrMSODrawings.Clear();
      if(m_shapes!=null) m_shapes.Clear();
      if( m_charts != null ) m_charts.Clear();
      if( m_pictures != null ) m_pictures.Clear();
    }
    /// <summary>
    /// Makes the current sheet the active sheet. Equivalent to clicking the
    /// sheet's tab in MS Excel.
    /// </summary>
    public virtual void Activate()
    {
      if( m_book.WindowOne.SelectedTab != RealIndex || m_book.Loading )
      {
        AppImplementation.SetActiveWorksheet( this );
        m_book.SetActiveWorksheet( this );
        m_book.InnerWorksheetGroup.Select( this );
      }
    }
    /// <summary>
    /// Selects current tab sheet.
    /// </summary>
    public virtual void Select()
    {
      Activate();
    }
    /// <summary>
    /// Unselects current tab sheet if possible.
    /// </summary>
    public void Unselect()
    {
      Unselect( true );
    }
    /// <summary>
    /// Unselects current tab sheet if possible.
    /// </summary>
    /// <param name="bCheckNumber">Indicates whether to allow unselect last sheet.</param>
    public void Unselect( bool bCheckNumber )
    {
      WindowOneRecord windowOne = m_book.WindowOne;

      if( WindowTwo.IsSelected )
      {
        if( bCheckNumber && windowOne.NumSelectedTabs > 1 || !bCheckNumber )
        {
          WindowTwo.IsSelected = false;
          m_book.WindowOne.NumSelectedTabs--;

          if( bCheckNumber ) m_book.InnerWorksheetGroup.Remove( this );
        }
      }
    }
    /// <summary>
    /// Protects worksheet with password.
    /// </summary>
    /// <param name="password">Protection password.</param>
    /// <exception cref="System.ApplicationException">
    /// If worksheet is already protected.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// If specified password is null.
    /// </exception>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If length of the password is more than 15 symbols.
    /// </exception>
    public void Protect( string password )
    {
      Protect( password, DefaultProtectionOptions );
    }
    /// <summary>
    /// Protects current worksheet.
    /// </summary>
    /// <param name="password">Represents password to protect.</param>
    /// <param name="options">Represents params to protect.</param>
    public void Protect( string password, ExcelSheetProtection options )
    {
      if( IsPasswordProtected )
        throw new ApplicationException( "Sheet is already protected, before use unprotect method" );

      if( password == null )
        throw new ArgumentNullException( "password" );

      if (password.Length > DEF_MAX_PASSWORDLEN)
          throw new ArgumentOutOfRangeException("Length of the password can't be more than "
            + DEF_MAX_PASSWORDLEN);

      ushort usPassword = ( password.Length > 0 )
        ? GetPasswordHash( password )
        : ( ushort )1;

      Protect( usPassword, options );
    }
    /// <summary>
    /// Prepares protection options before setting protection.
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual ExcelSheetProtection PrepareProtectionOptions( ExcelSheetProtection options )
    {
      return options;
    }
    /// <summary>
    /// Unprotects this worksheet without password.
    /// </summary>
    public void Unprotect()
    {
      m_password = null;
      //m_bIsProtected = false;
      m_sheetProtection = null;
    }
    /// <summary>
    /// Unprotects this worksheet using specified password.
    /// </summary>
    /// <param name="password">Password to unprotect.</param>
    public void Unprotect( string password )
    {
      if( m_password == null && m_sheetProtection == null )//!m_bIsProtected )
        return;

      if( password == null )
        throw new ArgumentNullException( "password" );

      if( password.Length > DEF_MAX_PASSWORDLEN )
        throw new ArgumentOutOfRangeException( "Length of the password can't be more than "
          + DEF_MAX_PASSWORDLEN );

      if( !IsPasswordProtected || m_password.IsPassword == GetPasswordHash( password ) || (m_password.IsPassword == 1 && password == "") )
      {
        m_password = null;
        //m_bIsProtected = false;
        m_sheetProtection = null;
      }
      else
      {
        throw new ArgumentException( "Wrong password" );
      }

      //m_bIsProtected = false;
    }
    /// <summary>
    /// This method is called after RealIndex property change.
    /// </summary>
    /// <param name="iOldIndex">Old index.</param>
    protected virtual void OnRealIndexChanged( int iOldIndex )
    {
    }
    /// <summary>
    /// Adds sheet tab to the selected tab.
    /// </summary>
    public void SelectTab()
    {
      if( !WindowTwo.IsSelected )
      {
        WindowTwo.IsSelected = true;
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
      m_shapes.UpdateFormula( iCurIndex, iSourceIndex, sourceRect, iDestIndex, destRect );
    }
    /// <summary>
    /// This method should be called immediately after extended format removal.
    /// </summary>
    /// <param name="dictFormats">Dictionary with updated extended formats.</param>
    public virtual void UpdateExtendedFormatIndex( Dictionary<int, int> dictFormats )
    {
    }
    /// <summary>
    /// Creates copy of the current object.
    /// </summary>
    /// <param name="parent">Parent object for the new object.</param>
    /// <returns>Copy of the current object.</returns>
    public virtual object Clone( object parent )
    {
      return Clone( parent, true );
    }
    /// <summary>
    /// Creates copy of the current object.
    /// </summary>
    /// <param name="parent">Parent object for the new object.</param>
    /// <param name="cloneShapes">Indicates whether we should clone shapes or not.</param>
    /// <returns>Copy of the current object.</returns>
    public virtual object Clone( object parent, bool cloneShapes )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      // NOTE: Don't forget to add all new members here.

      WorksheetBaseImpl result = ( WorksheetBaseImpl )MemberwiseClone();
      result.SetParent( parent );
      result.FindParents();

      //result.m_windowZoom = ( WindowZoomRecord )CloneUtils.CloneCloneable( m_windowZoom );
      result.m_password = ( PasswordRecord )CloneUtils.CloneCloneable( m_password );
      result.m_windowTwo = ( WindowTwoRecord )CloneUtils.CloneCloneable( m_windowTwo );
      result.m_bof = ( BOFRecord )CloneUtils.CloneCloneable( m_bof );

      result.m_arrMSODrawings = CloneUtils.CloneCloneable( m_arrMSODrawings );
      result.m_arrRecords = CloneUtils.CloneCloneable( m_arrRecords );

      if( m_charts != null )
      {
        result.m_charts = new WorksheetChartsCollection( Application, this );
      }

      if( m_pictures != null )
      {
        result.m_pictures = new PicturesCollection( Application, this );
      }

      if( cloneShapes )
        CloneShapes( result );

      // NOTE: we should copy shapes object only, because otherwise shapes
      // and charts (pictures) collection would contain different references.
      //result.m_charts = ( WorksheetChartsCollection )m_charts.Clone( result );
      //result.m_pictures = ( PicturesCollection )m_pictures.Clone( result );
      result.m_headerFooterShapes = ( HeaderFooterShapeCollection )
        CloneUtils.CloneCloneable( ( ICloneParent )m_headerFooterShapes, result );

#if !SILVERLIGHT && !WINRT && !WP
      if( m_dataHolder != null )
        result.m_dataHolder = m_dataHolder.Clone( result.m_book.DataHolder );
#endif

      return result;
    }
    /// <summary>
    /// Create copy of the shapes collection inside specified worksheet object.
    /// </summary>
    /// <param name="result">Object to put shapes into.</param>
    public void CloneShapes( WorksheetBaseImpl result )
    {
      result.m_shapes = ( ShapesCollection )m_shapes.Clone( result );
    }
    /// <summary>
    /// Updates style indexes.
    /// </summary>
    /// <param name="styleIndexes">Array with changed style indexes.</param>
    protected internal virtual void UpdateStyleIndexes( int[] styleIndexes )
    {
      if( styleIndexes == null )
        throw new ArgumentNullException( "styleIndexes" );
    }
    /// <summary>
    /// Protects worksheet.
    /// </summary>
    /// <param name="password">Password hash to use for protection.</param>
    /// <param name="options">Protection options.</param>
    internal void Protect( ushort password, ExcelSheetProtection options )
    {
      if( m_password == null )
      {
        m_password = ( PasswordRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.Password );
      }

      options = PrepareProtectionOptions( options );
      m_password.IsPassword = password;
      
      //m_bIsProtected = true;
      m_sheetProtection = ( SheetProtectionRecord )BiffRecordFactory.GetRecord( TBIFFRecord.SheetProtection );
      m_sheetProtection.ProtectedOptions = ( ushort )options;
      m_sheetProtection.ContainProtection = true;
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public abstract void MarkUsedReferences( bool[] usedItems );
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public abstract void UpdateReferenceIndexes( int[] arrUpdatedIndexes );
    #endregion

    #region Class parse methods
    /// <summary>
    /// Method extracts biff records belonging to the worksheet.
    /// </summary>
    /// <param name="reader">BiffReader that contains worksheet's records.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="bSkipParsing">Indicates whether to skip parsing.</param>
    /// <param name="hashNewXFormatIndexes">
    /// Dictionary with new extended format indexes for ignore styles mode.
    /// </param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    [ CLSCompliant( false ) ]
    protected internal void Parse( BiffReader reader, ExcelParseOptions options,
      bool bSkipParsing, Dictionary<int, int> hashNewXFormatIndexes, IDecryptor decryptor )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      // This condition enables parse worksheet on demand support for binary file format
      if (options == ExcelParseOptions.ParseWorksheetsOnDemand)
      {
          this.ParseOnDemand = true;
      }

      int iBOFCounter = 0;
      bool bSkipStyles = false;//( ( options & ExcelParseOptions.SkipStyles ) != 0 );

#if MEASURE_PERFORMANCE
      //TimeSpan getRecordTime = new TimeSpan( 0 );
#endif

      do
      {
              iBOFCounter = ParseNextRecord(reader, iBOFCounter, options,
              bSkipStyles, hashNewXFormatIndexes, decryptor);
              if (iBOFCounter == 0) break;
      }
          while (!reader.IsEOF);

      PrepareProtection();

#if MEASURE_PERFORMANCE
//      Console.WriteLine( "Time to get records: {0}", getRecordTime );
#endif
      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "Extracted: " + m_arrRecords.Count, "Parse Worksheet" );

      m_bParsed = false;
      m_bSkipParsing = bSkipParsing;
      IsSaved = true;
    }
    /// <summary>
    /// Prepares protection variables.
    /// </summary>
    protected void PrepareProtection()
    {
      if( m_sheetProtection == null &&
        m_parseProtection != ExcelSheetProtection.None &&
        m_parseProtection != ( ExcelSheetProtection )( -1 ) )
      {
        m_sheetProtection = ( SheetProtectionRecord )BiffRecordFactory.GetRecord( TBIFFRecord.SheetProtection );
        m_sheetProtection.ProtectedOptions = ( int )m_parseProtection;
        m_sheetProtection.ContainProtection = true;
      }
    }
    /// <summary>
    /// Extracts next record from reader and parses it.
    /// </summary>
    /// <param name="reader">Reader to extract record from.</param>
    /// <param name="iBOFCounter">Number of BOF records without closing EOF record.</param>
    /// <param name="bSkipStyles">Indicates whether styles information must be skipped.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="hashNewXFormatIndexes">
    /// Dictionary with new extended format indexes for ignore styles mode.
    /// </param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    /// <returns>Updated number of BOF records without closing EOF record.</returns>
    [ CLSCompliant( false ) ]
    protected virtual int ParseNextRecord( BiffReader reader, int iBOFCounter, ExcelParseOptions options,
      bool bSkipStyles, Dictionary<int, int> hashNewXFormatIndexes, IDecryptor decryptor )
    {
#if MEASURE_PERFORMANCE
//      DateTime start = DateTime.Now;
#endif
      BiffRecordRaw raw = reader.GetRecord( decryptor );

#if MEASURE_PERFORMANCE
//      getRecordTime += DateTime.Now - start;
#endif

      if (ParseOnDemand)
      {
          switch (raw.TypeCode)
          {
              case TBIFFRecord.BOF:
                  iBOFCounter++;

                  if (iBOFCounter == 1)
                  {
                      m_bof = (BOFRecord)raw;
                      BOFRecord.TType sheetType = m_bof.Type;
                      m_bIsSupported = (sheetType == BOFRecord.TType.TYPE_WORKSHEET
                        || sheetType == BOFRecord.TType.TYPE_CHART);
                  }
                  if (KeepRecord) m_arrRecords.Add(raw);
                  break;

              case TBIFFRecord.EOF:
                  iBOFCounter--;
                  if (KeepRecord) m_arrRecords.Add(raw);
                  break;

              default:
                  if (KeepRecord) m_arrRecords.Add(raw);
                  break;
          }
      }
      else
      {
          if (bSkipStyles)
          {
              if (!m_book.ModifyRecordToSkipStyle(raw))
                  return iBOFCounter;
              //m_arrRecords.Add( raw );
          }


          if (KeepRecord) m_arrRecords.Add(raw);

          // NOTE: Here we must add code which set worksheet properties
          // or configure worksheet behaviour.
          if (iBOFCounter <= 1)
          {
              switch (raw.TypeCode)
              {
                  case TBIFFRecord.BOF:
                      iBOFCounter++;

                      if (iBOFCounter == 1)
                      {
                          m_bof = (BOFRecord)raw;
                          BOFRecord.TType sheetType = m_bof.Type;
                          m_bIsSupported = (sheetType == BOFRecord.TType.TYPE_WORKSHEET
                            || sheetType == BOFRecord.TType.TYPE_CHART);
                      }
                      break;

                  // reserve space for cells according to Dimensions record info
                  case TBIFFRecord.Dimensions:
                      ParseDimensions((DimensionsRecord)raw);
                      break;

                  case TBIFFRecord.WindowTwo:
                      ParseWindowTwo((WindowTwoRecord)raw);
                      break;
                  case TBIFFRecord.PageLayoutView :
                      ParsePageLayoutView((PageLayoutView)raw );
                      break ;
                  case TBIFFRecord.WindowZoom:
                      ParseWindowZoom((WindowZoomRecord)raw);
                      break;

                  case TBIFFRecord.Protect:
                      ParseProtect((ProtectRecord)raw);
                      break;

                  case TBIFFRecord.WindowProtect:
                      bool bProtected = ((WindowProtectRecord)raw).IsProtected;

                      if (bProtected && !Workbook.IsWindowProtection && !Workbook.IsCellProtection)
                          Workbook.Protect(true, false);
                      break;

                  case TBIFFRecord.ScenProtect:
                      ParseScenProtect((ScenProtectRecord)raw);
                      break;

                  case TBIFFRecord.ObjectProtect:
                      ParseObjectProtect((ObjectProtectRecord)raw);
                      break;

                  case TBIFFRecord.Password:
                      ParsePassword((PasswordRecord)raw);
                      break;

                  case TBIFFRecord.SheetProtection:
                      SheetProtectionRecord protect = (SheetProtectionRecord)raw;

                      if (m_book.Loading || (m_parseProtection != (ExcelSheetProtection)(-1) && protect.ContainProtection &&
                         (ProtectionMeaningDirect && (m_parseProtection & ExcelSheetProtection.Content) != 0 ||
                         !ProtectionMeaningDirect && (m_parseProtection & ExcelSheetProtection.Content) == 0)))
                      {
                          m_sheetProtection = protect;
                      }
                      break;

                  case TBIFFRecord.RangeProtection:
                      m_previousRecord = raw;
                      m_rangeProtectionRecord = (RangeProtectionRecord)raw;
                      break;

                  case TBIFFRecord.ContinueFrt:
                      if (m_previousRecord == (RangeProtectionRecord)m_previousRecord)
                      {
                          byte[] data = new byte[8221];
                          reader.DataProvider.ReadArray(0, data);
                          RangeProtectionRecord rangeProtectRecord =  (m_previousRecord as RangeProtectionRecord);
                          if (rangeProtectRecord.m_continueRecords == null)
                              rangeProtectRecord.m_continueRecords = new List<UnknownRecord>();
                          rangeProtectRecord.m_continueRecords.Add((UnknownRecord)raw);
                          if( raw.Length < data.Length )
                          {
                              int iOptionOffset = 8065;
                              ExcelIgnoreError m_ignoreOpt = (ExcelIgnoreError)reader.DataProvider.ReadUInt16(iOptionOffset);

                              if (m_ignoreOpt != ExcelIgnoreError.None)
                              {
                                  rangeProtectRecord.ErrorIndicator = new ErrorIndicatorImpl(m_ignoreOpt);
                                  m_errorIndicators.Add(rangeProtectRecord.ErrorIndicator);
                              }
                          }
                      }
                      break;

                  case TBIFFRecord.CodeName:
                      m_strCodeName = ((CodeNameRecord)raw).CodeName;
                      break;

                  case TBIFFRecord.MSODrawing:
                      if (!KeepRecord)
                      {
                          KeepRecord = true;
                          m_arrRecords.Add(raw);
                      }

                      if (m_iMsoStartIndex < 0) m_iMsoStartIndex = m_arrRecords.Count - 1;
                      break;

                  case TBIFFRecord.EOF:
                      iBOFCounter--;
                      break;

                  //case ( TBIFFRecord )2150:
                  case TBIFFRecord.HeaderFooterImage:
                      HeaderFooterImageRecord headerFooter = (HeaderFooterImageRecord)raw;
                      HeaderFooterShapes.ParseMsoStructures(headerFooter.StructuresList, options);
                      break;

                  case TBIFFRecord.SheetLayout:
                      ParseSheetLayout((SheetLayoutRecord)raw);
                      break;

                  case TBIFFRecord.DefaultRowHeight:
                      this.m_isCustomHeight = ((DefaultRowHeightRecord) raw).CustomHeight;
                      break;

                  default:
                      ParseRecord(raw, bSkipStyles, hashNewXFormatIndexes);
                      break;
              }
          }
          else if (raw.TypeCode == TBIFFRecord.EOF)
          {
              iBOFCounter--;
          }
          else if (raw.TypeCode == TBIFFRecord.BOF)
          {
              iBOFCounter++;
          }
      }

      return iBOFCounter;
    }
    /// <summary>
    /// Parses Protect record.
    /// </summary>
    /// <param name="protectRecord">Record to parse.</param>
    [ CLSCompliant( false ) ]
    protected void ParseProtect( ProtectRecord protectRecord )
    {
      if( protectRecord.IsProtected )
      {
        m_parseProtection = DefaultProtectionOptions;

        if( ProtectionMeaningDirect )
        {
          m_parseProtection |= ExcelSheetProtection.Content;
        }
        else
        {
          m_parseProtection &= ~ExcelSheetProtection.Content;
        }
      }
    }
    /// <summary>
    /// Parses Password record.
    /// </summary>
    /// <param name="passwordRecord">Record to parse.</param>
    [CLSCompliant( false )]
    protected void ParsePassword( PasswordRecord passwordRecord )
    {
      m_password = passwordRecord;
    }
    /// <summary>
    /// Parse ObjectProtect record.
    /// </summary>
    /// <param name="objectProtect">Record to parse.</param>
    [CLSCompliant( false )]
    protected void ParseObjectProtect( ObjectProtectRecord objectProtect )
    {
      if( objectProtect.IsProtected )
      {
        if( ProtectionMeaningDirect )
        {
          m_parseProtection |= ExcelSheetProtection.Objects;
        }
        else
        {
          m_parseProtection &= ~ExcelSheetProtection.Objects;
        }
      }
    }
    /// <summary>
    /// Parse ScenProtect record.
    /// </summary>
    /// <param name="scenProtect">Record to parse.</param>
    [CLSCompliant( false )]
    protected void ParseScenProtect( ScenProtectRecord scenProtect )
    {
      if( scenProtect.IsProtected )
      {
        if( ProtectionMeaningDirect )
        {
          m_parseProtection |= ExcelSheetProtection.Scenarios;
        }
        else
        {
          m_parseProtection &= ~ExcelSheetProtection.Scenarios;
        }
      }
    }
    /// <summary>
    /// Prepares variables to worksheet parsing.
    /// </summary>
    /// <param name="options">Parse options.</param>
    /// <param name="bSkipParsing">Indicates whether to skip parsing.</param>
    protected virtual void PrepareVariables( ExcelParseOptions options, bool bSkipParsing )
    {
      m_arrRecords.Clear();
      m_iMsoStartIndex  = -1;
      m_parseOptions = options;
    }
    /// <summary>
    /// Parse pagelayoutview
    /// </summary>
    /// <param name="layout">Record to parse.</param>
    [CLSCompliant(false)]
    private void ParsePageLayoutView(PageLayoutView layout)
    {
        if (layout == null)
            throw new ArgumentNullException("windowTwo");
        WorksheetImpl sheet = this as WorksheetImpl;
        if (layout.LayoutView)
            sheet.View = SheetView.PageLayout;
        m_layout = layout;
    }
    /// <summary>
    /// Parse WindowTwo record.
    /// </summary>
    /// <param name="windowTwo">Record to parse.</param>
    [ CLSCompliant( false ) ]
    protected virtual void ParseWindowTwo( WindowTwoRecord windowTwo )
    {
      if( windowTwo == null )
        throw new ArgumentNullException( "windowTwo" );

      m_windowTwo = windowTwo;

      if( m_windowTwo.IsSelected )
      {
        m_book.WorksheetGroup.Add( this );

//        m_book.WindowOne.NumSelectedTabs--;
//
//        if( m_windowTwo.IsPaged && m_book.WindowOne.SelectedTab == RealIndex )
//        {
//          m_windowTwo.IsSelected = false;
//          m_book.WindowOne.SelectedTab = ( ushort )( RealIndex + 1 );
//          Activate();
//        }
//        else
//        {
//          m_windowTwo.IsSelected = false;
//          Select();
//        }
      }
    }

    /// <summary>
    /// Parses single record.
    /// </summary>
    /// <param name="raw">Record to parse.</param>
    /// <param name="bIgnoreStyles">Indicates whether to ignore styles.</param>
    /// <param name="hashNewXFormatIndexes">
    /// Dictionary with new extended format indexes for ignore styles mode.
    /// </param>
    [ CLSCompliant( false ) ]
    protected virtual void ParseRecord( BiffRecordRaw raw, bool bIgnoreStyles,
      Dictionary<int, int> hashNewXFormatIndexes )
    {
    }
    /// <summary>
    /// Parses dimensions record.
    /// </summary>
    /// <param name="dimensions">Record to parse.</param>
    [ CLSCompliant( false ) ]
    protected virtual void ParseDimensions( DimensionsRecord dimensions )
    {
      if( dimensions == null )
        throw new ArgumentNullException( "dimensions" );

      if( dimensions.LastColumn == 0 && dimensions.LastRow == 0 )
      {
        m_iFirstColumn = DEF_MIN_COLUMN_INDEX;
        m_iFirstRow     = DEF_MIN_ROW_INDEX;
        m_iLastColumn  = DEF_MIN_COLUMN_INDEX;
        m_iLastRow      = DEF_MIN_ROW_INDEX;
      }
      else
      {
        m_iFirstColumn = dimensions.FirstColumn + 1;
        m_iFirstRow     = dimensions.FirstRow + 1;
        m_iLastColumn  = Math.Min( dimensions.LastColumn, m_book.MaxColumnCount );

        if( m_iLastColumn == 0 ) m_iLastColumn = 1;

        m_iLastRow = Math.Min( dimensions.LastRow, m_book.MaxRowCount );

        if( m_iLastRow == 0 ) m_iLastRow = 1;
      }
    }
    /// <summary>
    /// Parses WindowZoom record.
    /// </summary>
    /// <param name="windowZoom">Record to parse.</param>
    [ CLSCompliant( false ) ]
    protected virtual void ParseWindowZoom( WindowZoomRecord windowZoom )
    {
      if( windowZoom == null )
        throw new ArgumentNullException( "windowZoom" );

      //m_windowZoom = windowZoom;
      m_iZoom = windowZoom.Zoom;
    }
    /// <summary>
    /// Parses sheet layout record.
    /// </summary>
    /// <param name="sheetLayout">Record to parse.</param>
    [ CLSCompliant( false ) ]
    protected void ParseSheetLayout( SheetLayoutRecord sheetLayout )
    {
      if( sheetLayout == null )
        throw new ArgumentNullException( "sheetLayout" );

      TabColor = ( ExcelKnownColors )sheetLayout.ColorIndex;
    }
    #endregion

    #region Class serialization methods
    /// <summary>
    ///  Serializes records.
    /// </summary>
    /// <param name="records">Records to be serialized.</param>
    [ CLSCompliant( false ) ]
    public virtual void Serialize( OffsetArrayList records )
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// Saves all shapes.
    /// </summary>
    /// <param name="records">List to save records into.</param>
    [ CLSCompliant( false ) ]
    protected virtual void SerializeMsoDrawings( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_shapes != null && m_shapes.Count > 0
        && ( Application.SkipOnSave & SkipExtRecords.Drawings ) != SkipExtRecords.Drawings )
      {
        m_shapes.Serialize( records );
      }
    }
    /// <summary>
    /// Saves protection block.
    /// </summary>
    /// <param name="records">List to save records into.</param>
    [ CLSCompliant( false ) ]
    protected virtual void SerializeProtection( OffsetArrayList records, bool bContentNotNecessary )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( bContentNotNecessary || ProtectContents )
      {
        if( ProtectContents )
        {
          ProtectRecord protect = ( ProtectRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Protect );
          protect.IsProtected = true;
          records.Add( protect );
        }

        if( ProtectScenarios )
        {
          ScenProtectRecord scenProtect = ( ScenProtectRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ScenProtect );
          scenProtect.IsProtected = true;
          records.Add( scenProtect );
        }

        if( ProtectDrawingObjects )
        {
          ObjectProtectRecord objProtect = ( ObjectProtectRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ObjectProtect );
          objProtect.IsProtected = true;
          records.Add( objProtect );
        }
      }

      if (m_password != null && m_password.IsPassword != 1)
        records.Add( m_password );
    }
    /// <summary>
    /// Serialize sheet protection.
    /// </summary>
    /// <param name="records">Represents record storage.</param>
    [ CLSCompliant( false ) ]
    protected void SerializeSheetProtection( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_sheetProtection != null && ContainsProtection )
      {
        SheetProtectionRecord protection = ( SheetProtectionRecord )m_sheetProtection.Clone();
        // Remove this flag since it is not stored inside those options, but inside ProtectRecord.
        protection.ProtectedOptions &= ~( int )ExcelSheetProtection.Content;
        records.Add( protection );
      }
      //else
      //{
      //  throw new ApplicationException( "Unexpected situation." );
      //}
    }
    /// <summary>
    /// Serializes header / footer pictures.
    /// </summary>
    /// <param name="records">List to save records into.</param>
    [ CLSCompliant( false ) ]
    protected virtual void SerializeHeaderFooterPictures( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_headerFooterShapes != null && m_headerFooterShapes.Count > 0 )
      {
        m_headerFooterShapes.Serialize( records );
      }
    }
    /// <summary>
    /// Serializes WindowTwo record.
    /// </summary>
    /// <param name="records">List to save records into.</param>
    [ CLSCompliant( false ) ]
    protected virtual void SerializeWindowTwo( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

//      if( this != m_book.ActiveSheet )
//      {
//        WindowTwo.IsPaged = false;
//        WindowTwo.IsSelected = false;
//      }
//      else
      if( this == m_book.ActiveSheet )
      {
        WindowTwo.IsPaged = true;
        WindowTwo.IsSelected = true;
      }

      records.Add( WindowTwo );
    }
    /// <summary>
    /// Serializes page layout view
    /// </summary>
    /// <param name="records">List to save records into.</param>
    [CLSCompliant(false)]
    protected virtual void SerializePageLayoutView(OffsetArrayList records)
    {
        if (records == null)
            throw new ArgumentNullException("records");
        WorksheetImpl sheet = this as WorksheetImpl;
        if (m_layout != null)
        {
            if (sheet.View == SheetView.PageLayout)
            {
                m_layout.LayoutView = true;
                records.Add(m_layout);
            }
        }
    }
    /// <summary>
    /// Serializes macros support.
    /// </summary>
    /// <param name="records">List to save records into.</param>
    [ CLSCompliant( false ) ]
    protected virtual void SerializeMacrosSupport( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( ( Application.SkipOnSave & SkipExtRecords.Macros ) != SkipExtRecords.Macros )
      {
        if( m_strCodeName != null && m_book.HasMacros )
        {
          CodeNameRecord code = ( CodeNameRecord )BiffRecordFactory.GetRecord( TBIFFRecord.CodeName );
          code.CodeName = m_strCodeName;
          records.Add( code );
        }
      }
    }
    /// <summary>
    /// Serializes WindowZoom.
    /// </summary>
    /// <param name="records">List to save records into.</param>
    [ CLSCompliant( false ) ]
    protected void SerializeWindowZoom( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      WindowZoomRecord zoom = ( WindowZoomRecord )BiffRecordFactory.GetRecord( TBIFFRecord.WindowZoom );
      zoom.Zoom = m_iZoom;
      records.Add( zoom );
      //records.Add( m_windowZoom );
    }
    /// <summary>
    /// Serializes sheet layout record.
    /// </summary>
    /// <param name="records">List to save records into.</param>
    [ CLSCompliant( false ) ]
    protected void SerializeSheetLayout( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_tabColor != null )
      {
        SheetLayoutRecord sheetLayout = ( SheetLayoutRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.SheetLayout );

        sheetLayout.ColorIndex = ( int )m_tabColor.GetIndexed( m_book );
        records.Add( sheetLayout );
      }
    }
    #endregion

    #region Class Static Methods
    /// <summary>
    /// Returns hash value for the password string.
    /// </summary>
    /// <param name="password">Password to hash.</param>
    /// <returns>Hash value for the password string.</returns>
    [ CLSCompliant( false ) ]
    public static ushort GetPasswordHash( string password )
    {
      if( password == null )
        return 0;//throw new ArgumentNullException( "password" );

      ushort usHash = 0;

      for( int iCharIndex = 0, len = password.Length; iCharIndex < len; iCharIndex++ )
      {
        bool[] bits = GetCharBits15( password[ iCharIndex ] );
        bits = RotateBits( bits, iCharIndex + 1 );
        ushort curNumber = GetUInt16FromBits( bits );
        usHash ^= curNumber;
      }

      return ( ushort ) ( usHash ^ password.Length ^ DEF_PASSWORD_CONST );
    }
    /// <summary>
    /// Converts character to 15 bits sequence
    /// </summary>
    /// <param name="charToConvert">Character to convert.</param>
    /// <returns>Array of values indicating 15 bit sequence.</returns>
    private static bool[] GetCharBits15( char charToConvert )
    {
      bool[] arrResult = new bool[ 15 ];
      ushort usSource = Convert.ToUInt16( charToConvert );
      ushort curBit = 1;

      for( int i = 0; i < 15; i++ )
      {
        arrResult[ i ] = ( ( usSource & curBit ) == curBit );
        curBit <<= 1;
      }

      return arrResult;
    }
    /// <summary>
    /// Converts bits array to UInt16 value.
    /// </summary>
    /// <param name="bits">Array to convert.</param>
    /// <returns>Converted UInt16 value.</returns>
    private static ushort GetUInt16FromBits( bool[] bits )
    {
      if( bits == null )
        throw new ArgumentNullException( "bits" );

      if( bits.Length > 16 )
        throw new ArgumentOutOfRangeException( "There can't be more than 16 bits" );

//      bool[] arrResult = new bool[ 15 ];
      ushort usResult = 0;
      ushort curBit = 1;

      for( int i = 0, len = bits.Length; i < len; i++ )
      {
        if( bits[ i ] ) usResult += curBit;
        curBit <<= 1;
      }

      return usResult;
    }
    /// <summary>
    /// Rotates (cyclic shift) bits in the array specified number of times
    /// </summary>
    /// <param name="bits">Array to rotate</param>
    /// <param name="count">Number of times to rotate</param>
    /// <returns>Rotated array.</returns>
    private static bool[] RotateBits( bool[] bits, int count )
    {
      if( bits == null )
        throw new ArgumentNullException( "bits" );

      if( bits.Length == 0 )
        return bits;

      if( count < 0 )
        throw new ArgumentOutOfRangeException( "Count can't be less than zero" );

      bool[] arrResult = new bool[ bits.Length ];

      for( int i = 0, len = bits.Length; i < len; i++ )
      {
        int newPos = ( i + count ) % len;

        arrResult[ newPos ] = bits[ i ];
      }

      return arrResult;
    }
    /// <summary>
    /// Rounds value.
    /// </summary>
    /// <param name="value">Value to be rounded.</param>
    /// <param name="degree">Represents degree used to round the given value.</param>
    /// <returns>Rounded value.</returns>
    public static int Round( int value, int degree )
    {
      if( degree == 0 )
        throw new ArgumentOutOfRangeException( "degree can't be 0" );

      int mod = value % degree;

      return value - mod + degree;
    }

    #endregion

    #region Class events
    /// <summary>
    /// This event is raised when name of the worksheet is changed.
    /// </summary>
#if !(WINRT )
    [ Category( "Property Changed" ) ]
#endif
    public event ValueChangedEventHandler NameChanged;
    #endregion

    #region INamedObject members
    /// <summary>
    /// Index of the worksheet in the workbook
    /// (not necessary in Worksheets collection)
    /// </summary>
    public int RealIndex
    {
      get
      {
        return m_iRealIndex;
      }
      set
      {
        if( m_iRealIndex != value )
        {
          int iOldIndex = m_iRealIndex;
          m_iRealIndex = value;
          OnRealIndexChanged( iOldIndex );
        }
      }
    }
    /// <summary>
    /// Returns index in the parent ITabSheets collection. Read-only.
    /// </summary>
    int ITabSheet.TabIndex
    {
      get
      {
        return m_iRealIndex;
      }
    }
    #endregion

    #region IParseable Members
    /// <summary>
    /// Parses internal records.
    /// </summary>
    public virtual void Parse()
    {
#if MEASURE_PERFORMANCE
      DateTime start = DateTime.Now;
#endif
      IsParsed = false;
      ParseData();

      bool bParsed = IsParsed;

      IsParsing = true;
      IsParsed = false;

      ExtractMSODrawing( m_iMsoStartIndex, m_parseOptions );

      IsParsing = false;
      IsParsed = bParsed;

      if( IsSupported && IsParsed )
      {
        m_arrRecords.Clear();
      }

#if MEASURE_PERFORMANCE
      TimeSpan parseTime = DateTime.Now - start;
      Console.WriteLine( "Time to parse {0}, {1}", Name, parseTime );
#endif
    }
    /// <summary>
    /// Parses worksheet's data.
    /// </summary>
    protected internal void ParseData()
    {
      ParseData( null );
    }
    /// <summary>
    /// Parses worksheet's data.
    /// </summary>
    protected internal abstract void ParseData( Dictionary<int, int> dictUpdatedSSTIndexes );
    /// <summary>
    /// Extracts MsoDrawing records from internal array.
    /// </summary>
    /// <param name="startIndex">Index to the first MsoDrawing record.</param>
    /// <param name="options">Parse options.</param>
    protected void ExtractMSODrawing( int startIndex, ExcelParseOptions options )
    {
      if( m_arrMSODrawings != null )
      {
        m_arrMSODrawings.Clear();
      }
      else
      {
        m_arrMSODrawings = new List<BiffRecordRaw>();
      }

      if( startIndex < 0 ) return;

      int i = startIndex;
      int iLen = m_arrRecords.Count;
      int iLevel = 0;

      for( ; i < iLen; i++ )
      {
        TBIFFRecord code = ( m_arrRecords[ i ] as BiffRecordRaw ).TypeCode;

        if( iLevel == 0 && Array.IndexOf( DEF_NOTMSORECORDS, code ) != -1 )
        {
          break;
        }

        switch( code )
        {
          case TBIFFRecord.BOF:
            iLevel++;
            goto default;

          case TBIFFRecord.EOF:
            iLevel--;
            goto default;

          case TBIFFRecord.TextObject:
            TextObjectRecord textObject = ( TextObjectRecord )m_arrRecords[ i ];
            m_arrMSODrawings.Add( textObject );

            if( textObject.TextLen > 0 )
            {
              i++;
              int iTextLen = textObject.TextLen;

              while( iTextLen > 0 )
              {
                ContinueRecord continueRecord = ( ContinueRecord  )m_arrRecords[ i ];
                byte[] arrData = continueRecord.Data;
                bool bUnicode = arrData[ 0 ] != 0;
                int iStringDataLen = arrData.Length - 1;
                iTextLen -= bUnicode ? iStringDataLen / 2 : iStringDataLen;
                m_arrMSODrawings.Add( m_arrRecords[ i ] );
                i++;
              }

              int iFormattingLen = textObject.FormattingRunsLen;
              // TODO: evaluate text and formatting length.
              while( iFormattingLen > 0 )
              {
                ContinueRecord continueRecord = ( ContinueRecord )m_arrRecords[ i ];
                iFormattingLen -= continueRecord.Length;
                m_arrMSODrawings.Add( continueRecord );
                i++;
              }

              i--;
            }
            break;

          case TBIFFRecord.Continue:
            //throw new NotImplementedException();
            MSODrawingRecord mso = BiffRecordFactory.GetRecord( TBIFFRecord.MSODrawing )
              as MSODrawingRecord;

            ContinueRecord cont = m_arrRecords[ i ] as ContinueRecord;

            mso.m_data = new byte[ cont.m_data.Length ];
            cont.m_data.CopyTo( mso.m_data, 0 );
            mso.RecordLength = cont.Length;
            m_arrMSODrawings.Add( mso );
            break;

          default:
            m_arrMSODrawings.Add( m_arrRecords[ i ] );
            break;
        }
      }

      List<MsoBase> structures = CombineMsoDrawings();
      m_shapes.ParseMsoStructures( structures, options );
      m_shapes.RegenerateComboBoxNames();
    }

    /// <summary>
    /// Combines all MsoBase records into one array.
    /// </summary>
    /// <returns>Array of records.</returns>
    private List<MsoBase> CombineMsoDrawings()
    {
      List<byte[]> arrCombined = new List<byte[]>();
      List<MsoBase> arrStructures = new List<MsoBase>();
      int iLevel = 0;
      int iCombinedLength = 0;

      if( m_arrMSODrawings.Count > 0 )
      {
        for( int i = 0, len = m_arrMSODrawings.Count; i < len; i++ )
        {
          // Use 'as' to increase performance.
          BiffRecordRaw record = m_arrMSODrawings[ i ];

          if( iLevel == 0 && record is MSODrawingRecord )
          {
            byte[] arrData = record.Data;
            iCombinedLength += arrData.Length;
            arrCombined.Add( arrData );
          }
          else if( record.TypeCode == TBIFFRecord.BOF )
          {
            iLevel++;
          }
          else if( record.TypeCode == TBIFFRecord.EOF )
          {
            iLevel--;
          }
        }

        byte[] buffer = CombineArrays( iCombinedLength, arrCombined );
        MemoryStream stream = new MemoryStream( buffer );

        while( stream.Position < iCombinedLength )
        {
          MsoBase record = MsoFactory.CreateMsoRecord( null, stream,
            new GetNextMsoDrawingData( GetNextMsoData ) );
          arrStructures.Add( record );
        }

      }

      return arrStructures;
    }

    /// <summary>
    /// Extracts client data for mso records.
    /// </summary>
    /// <returns>Array of records.</returns>
    private BiffRecordRaw[] GetNextMsoData()
    {
      List<BiffRecordRaw> result = new List<BiffRecordRaw>();
      bool bResultStarted = false;
      int iLevel = 0;

      while( !bResultStarted )
      {
        if( m_iCurMsoIndex >= m_arrMSODrawings.Count )
          throw new ApplicationException( "Can't find data for MSODrawing" );

        if( !(m_arrMSODrawings[ m_iCurMsoIndex ] is MSODrawingRecord ) )
        {
          bResultStarted = true;
        }
        else
        {
          m_iCurMsoIndex++;
        }
      }

      while( bResultStarted && m_iCurMsoIndex < m_arrMSODrawings.Count )
      {
        if( iLevel == 0 && m_arrMSODrawings[ m_iCurMsoIndex ] is MSODrawingRecord )
        {
          //bResultStarted = false;
          break;
        }
        else
        {
          result.Add( m_arrMSODrawings[ m_iCurMsoIndex ] );
        }

        if( m_arrMSODrawings[ m_iCurMsoIndex ] is BOFRecord )
        {
          iLevel++;
        }
        else if( m_arrMSODrawings[ m_iCurMsoIndex ] is EOFRecord )
        {
          iLevel--;
        }

        m_iCurMsoIndex++;
      }

      return result.ToArray();
    }
    /// <summary>
    /// Combines several byte arrays into one.
    /// </summary>
    /// <param name="iCombinedLength">Size of the combined data.</param>
    /// <param name="arrCombined">List that contains byte arrays to combine.</param>
    /// <returns>Combined array.</returns>
    public static byte[] CombineArrays( int iCombinedLength, List<byte[]> arrCombined )
    {
      if( arrCombined == null || arrCombined.Count == 0 )
        return new byte[ 0 ];

      int iLength = arrCombined.Count;

      byte[] arrResult = new byte[ iCombinedLength ];
      int iOffset = 0;

      for( int i = 0; i < iLength; i++ )
      {
        byte[] arrCurrent = arrCombined[ i ];
        int iPartLength = arrCurrent.Length;
        Buffer.BlockCopy( arrCurrent, 0, arrResult, iOffset, iPartLength );
        iOffset += iPartLength;
      }

      return arrResult;
    }
    #endregion

    #region Class copy methods
    /// <summary>
    /// Copies all data from another worksheet.
    /// </summary>
    /// <param name="worksheet">Parent worksheet.</param>
    /// <param name="hashStyleNames">Dictionary with style names.</param>
    /// <param name="hashWorksheetNames">Dictionary with new worksheet names.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <param name="flags">Copy flags.</param>
    /// <param name="hashExtFormatIndexes">
    /// Dictionary with extended format indexes, key - old index, value - new index.
    /// </param>
    public void CopyFrom( WorksheetBaseImpl worksheet, Dictionary<string, string> hashStyleNames,
      Dictionary<string, string> hashWorksheetNames, Dictionary<int, int> dicFontIndexes, ExcelWorksheetCopyFlags flags,
      Dictionary<int, int> hashExtFormatIndexes )
    {
      if( ( flags & ExcelWorksheetCopyFlags.ClearBefore ) != 0 )
        ClearAll( flags );

      if( ( flags & ExcelWorksheetCopyFlags.CopyOptions ) != 0 )
        CopyOptions( worksheet );

      if( ( flags & ExcelWorksheetCopyFlags.CopyShapes ) != 0 )
        CopyShapes( worksheet, hashWorksheetNames, dicFontIndexes );

      if( ( flags & ExcelWorksheetCopyFlags.CopyPageSetup ) != 0 )
        CopyHeaderFooterImages( worksheet, hashWorksheetNames, dicFontIndexes );
    }
    /// <summary>
    /// Copies preserved header/footer images.
    /// </summary>
    /// <param name="sourceSheet">Source worksheet.</param>
    /// <param name="hashNewNames">Hash with new WorkSheet names.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    protected void CopyHeaderFooterImages( WorksheetBaseImpl sourceSheet,
      Dictionary<string, string> hashNewNames, IDictionary dicFontIndexes )
    {
      PageSetupBaseImpl setup = PageSetupBase;
      PageSetupBaseImpl sourceSetup = sourceSheet.PageSetupBase;

      /*m_headerFooterShapes = ( HeaderFooterShapeCollection )*/
        CloneUtils.CloneCloneable( ( ICloneParent )sourceSheet.m_headerFooterShapes, this );
    }
    /// <summary>
    /// Copies all shapes from a source worksheet.
    /// </summary>
    /// <param name="sourceSheet">Source worksheet.</param>
    /// <param name="hashNewNames">Hash with new WorkSheet names.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    protected void CopyShapes( WorksheetBaseImpl sourceSheet,
      Dictionary<string, string> hashNewNames, Dictionary<int, int> dicFontIndexes )
    {
      ShapeCollectionBase sourceShapes = sourceSheet.Shapes as ShapeCollectionBase;

      for( int i = 0, len = sourceShapes.Count; i < len; i++ )
      {
        ShapeImpl shape = ( ShapeImpl )sourceShapes[ i ];

        m_shapes.AddCopy( shape, hashNewNames, dicFontIndexes );
      }

      //m_shapes.LastId = ( m_shapes.LastId == 0 ) ?
      //  sourceShapes.LastId :
      //  Math.Max( sourceShapes.LastId, m_shapes.LastId );

      //m_shapes.StartId = ( m_shapes.StartId == 0 ) ?
      //  sourceShapes.StartId :
      //  Math.Min( sourceShapes.StartId, m_shapes.StartId );
    }
    /// <summary>
    /// Copies different sheet options.
    /// </summary>
    /// <param name="sourceSheet">Source sheet.</param>
    protected virtual void CopyOptions( WorksheetBaseImpl sourceSheet )
    {
      //m_bIsProtected = sourceSheet.ProtectContents;

      m_sheetProtection = ( SheetProtectionRecord )CloneUtils.CloneCloneable( sourceSheet.m_sheetProtection );

      if( sourceSheet.m_password != null )
      {
        m_password = ( PasswordRecord )sourceSheet.m_password.Clone();
      }

      //m_windowZoom = ( WindowZoomRecord )sourceSheet.m_windowZoom.Clone();
      m_iZoom = sourceSheet.m_iZoom;

      if( sourceSheet.m_windowTwo != null )
      {
        m_windowTwo = ( WindowTwoRecord )sourceSheet.m_windowTwo.Clone();
        m_windowTwo.IsSelected = false;
        m_windowTwo.IsPaged = false;
      }

      CopyTabColor( sourceSheet );

      string strSourceCodeName = sourceSheet.m_strCodeName;

      //creates default name.
      if( strSourceCodeName != null && strSourceCodeName.Length > 0 )
      {
        m_strCodeName = GenerateUniqueName( ( NameGetter )GetCodeName, strSourceCodeName );
      }
    }
    /// <summary>
    /// Generates unique name for the parent collection
    /// </summary>
    /// <param name="getName">Name getter.</param>
    /// <param name="sourceCodeName">Proposed name</param>
    /// <returns>Generated unique name.</returns>
    private string GenerateUniqueName( NameGetter getName, string sourceCodeName )
    {
      Dictionary<string, object> hashNames = new Dictionary<string, object>();
      ITabSheets tabSheets = m_book.TabSheets;

      // Step 1. Chech whether proposed name exist in the collection
      bool bUnique = true;
      for( int i = 0, len = tabSheets.Count; i < len; i++ )
      {
          if (this.Index != i)
          {
              string name = getName(tabSheets[i]);

              if (name == sourceCodeName)
              {
                  bUnique = false;
              }

              hashNames.Add(name, null);
          }
      }
      // If no, then done,
      // If yes, then try to modify name, but not exceed total length
      if( !bUnique )
      {
        int counter = 0;
        string proposedName = sourceCodeName;

        while( hashNames.ContainsKey( proposedName ) )
        {
          counter++;
          proposedName = sourceCodeName + "_" + counter;

          if( proposedName.Length > MaxSheetNameLength )
          {
            counter = 0;
            proposedName = sourceCodeName = sourceCodeName.Remove( sourceCodeName.Length - 1 );
          }
        }

        sourceCodeName = proposedName;
      }

      return sourceCodeName;
    }
    /// <summary>
    /// Delegate to be used for getting name information from the worksheet.
    /// </summary>
    /// <param name="tabSheet">Worksheet to get name from.</param>
    /// <returns>Extracted name.</returns>
    private delegate string NameGetter( ITabSheet tabSheet );
    /// <summary>
    /// Gets code name of the tabsheet.
    /// </summary>
    /// <param name="tabSheet">Tabsheet to get name from.</param>
    /// <returns>Code name of the tabsheet.</returns>
    private string GetCodeName( ITabSheet tabSheet )
    {
      return tabSheet.CodeName;
    }
    /// <summary>
    /// Gets name of the tabsheet.
    /// </summary>
    /// <param name="tabSheet">Tabsheet to get name from.</param>
    /// <returns>Name of the tabsheet.</returns>
    private string GetName( ITabSheet tabSheet )
    {
      return tabSheet.Name;
    }
    /// <summary>
    /// Copies tab color from another worksheet.
    /// </summary>
    /// <param name="sourceSheet">Worksheet to copy tab color from.</param>
    private void CopyTabColor( WorksheetBaseImpl sourceSheet )
    {
      if( sourceSheet == null )
        throw new ArgumentNullException( "sourceSheet" );

      if( sourceSheet.m_tabColor == null )
      {
        m_tabColor = sourceSheet.m_tabColor;
      }
      else
      {
        if( m_tabColor == null )
          m_tabColor = new ColorObject( ExcelKnownColors.None );

        m_tabColor.CopyFrom( sourceSheet.m_tabColor, true );
      }
    }
    /// <summary>
    /// Checks the worksheet for parse on demand load
    /// </summary>
    internal void CheckParseOnDemand()
    {
        bool bOldThrow = m_book.ThrowOnUnknownNames;
        m_book.ThrowOnUnknownNames = false;
        if (m_dataHolder != null && ParseDataOnDemand)
            m_dataHolder.ParseWorksheetData((this as WorksheetImpl), null, ParseDataOnDemand);
        else if (m_dataHolder == null && ParseOnDemand && !IsParsed && (this.Parent is Syncfusion.XlsIO.Implementation.Collections.WorksheetsCollection))
        {
            m_book.Loading = true;
            m_bParseMSODrawings = true;
            ParseData(null);
            m_bParseMSODrawings = false;
            m_book.Loading = false;
        }
        m_book.ThrowOnUnknownNames = bOldThrow;
    }
    #endregion

      #region Dispose
    public override void Dispose()
    {
        base.Dispose();
        if (m_pictures != null)
        {
            m_pictures.Clear();
            m_pictures = null;
  }
        if (m_shapes != null)
        {
            m_shapes.Clear();
            m_shapes = null;
        }
        if (NameChanged != null)
        {
            NameChanged = null;
        }
    }
      #endregion
  }
}
