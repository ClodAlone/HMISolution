#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

#region file using directives
using System;
using System.Collections;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Implementation.Collections;

using LinkIndex = Syncfusion.XlsIO.Parser.Biff_Records.Charts.ChartAIRecord.LinkIndex;
using ReferenceType = Syncfusion.XlsIO.Parser.Biff_Records.Charts.ChartAIRecord.ReferenceType;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.Exceptions;
using System.IO;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Class used for Chart Axis implementation.
  /// </summary>
  public abstract class ChartAxisImpl
    : CommonObject
    , IChartAxis
  {
    #region Class constants
    /// <summary>
    /// Represents default number format index;
    /// </summary>
    protected const int DEF_NUMBER_FORMAT_INDEX = -1;
    /// <summary>
    /// Represents index to general format.
    /// </summary>
    private const int DEF_GENERAL_FORMAT = 0;
    #endregion

    #region Class members
    /// <summary>
    /// Type of the axis.
    /// </summary>
    private ExcelAxisType m_axisType;
    /// <summary>
    /// True if this is the primary axis; false if secondary.
    /// </summary>
    private bool m_bPrimary;
    /// <summary>
    /// Text area for the axis title.
    /// </summary>
    private ChartTextAreaImpl m_titleArea;
    /// <summary>
    /// Chart tick record for this axis.
    /// </summary>
    private ChartTickRecord m_chartTick;
    /// <summary>
    /// Represents Shadow
    /// </summary>
    private ShadowImpl m_shadow;
    /// <summary>
    /// Indicates whether line format is present.
    /// </summary>
    private bool m_bLineFormat;
    /// <summary>
    /// Font used for axis drawing.
    /// </summary>
    private FontWrapper m_font;
	/// <summary>
    /// Font used to store the default font used with axis drawing.
    /// </summary>
    private FontWrapper old_Font;
    /// <summary>
    /// Represents major grid.
    /// </summary>
    private ChartGridLineImpl m_majorGrid;
    /// <summary>
    /// Represents minor grid.
    /// </summary>
    private ChartGridLineImpl m_minorGrid;
    /// <summary>
    /// Represents if axis has major gridlines.
    /// </summary>
    private bool m_bHasMajor;
    /// <summary>
    /// Represents if axis has minor gridlines.
    /// </summary>
    private bool m_bHasMinor;
    /// <summary>
    /// Represents parent axis.
    /// </summary>
    private ChartParentAxisImpl m_parentAxis;
    /// <summary>
    /// Represents format index.
    /// </summary>
    private int m_iNumberFormat = DEF_NUMBER_FORMAT_INDEX;
    /// <summary>
    /// Represents border axis.
    /// </summary>
    private ChartBorderImpl m_border;
    /// <summary>
    /// Represents text direction.
    /// </summary>
    private ExcelAxisTextDirection m_textDirection = ExcelAxisTextDirection.Context;
    /// <summary>
    /// Axis id.
    /// </summary>
    private int m_iAxisId;
    /// <summary>
    /// Inidicates whether axis was deleted.
    /// </summary>
    private bool m_bDeleted;
    /// <summary>
    /// Indicates whether tick label spacing value is automatically evaluated.
    /// </summary>
    private bool m_bAutoTickLabelSpacing = true;
    /// <summary>
    /// Indicates whether TickMarkSpacing has automatic value.
    /// </summary>
    private bool m_bAutoTickMarkSpacing;
    /// <summary>
    /// Stores label alignment setting.
    /// </summary>
    internal string LabelAlign;
    /// <summary>
    /// Represents the 3D features
    /// </summary>
    private ThreeDFormatImpl m_3D;
    private ChartAxisPos? m_axisPos;
    private bool m_sourceLinked;
    private Stream m_textStream;
    /// <summary>
    /// Represents the TextArea Paragraph 
    /// </summary>
    private ChartParagraphType m_paraType;
      /// <summary>
      /// Represents the frame format of Axis
      /// </summary>
    private ChartFrameFormatImpl m_axisFormat;
    private bool m_IsDefaultTextSettings;
    private bool m_isChartFont;
    #endregion

    #region Class constructors
    /// <summary>
    /// Creates axis object.
    /// </summary>
    /// <param name="application">Application object for the axis.</param>
    /// <param name="parent">Parent object for the axis.</param>
    public ChartAxisImpl( IApplication application, object parent )
      : base( application, parent )
    {
      InitializeVariables();
    }
    /// <summary>
    /// Creates primary axis of specified type.
    /// </summary>
    /// <param name="application">Application object for the axis.</param>
    /// <param name="parent">Parent object for the axis.</param>
    /// <param name="axisType">Type of the new axis.</param>
    public ChartAxisImpl( IApplication application, object parent, ExcelAxisType axisType )
      : this( application, parent, axisType, true )
    {
    }
    /// <summary>
    /// Creates axis of specified type and specified IsPrimary value.
    /// </summary>
    /// <param name="application">Application object for the axis.</param>
    /// <param name="parent">Parent object for the axis.</param>
    /// <param name="axisType">Type of the new axis.</param>
    /// <param name="bIsPrimary">
    /// True if primary axis should be created; otherwise False.
    /// </param>
    public ChartAxisImpl( IApplication application, object parent,
      ExcelAxisType axisType, bool bIsPrimary )
      : base( application, parent )
    {
      m_axisType = axisType;
      m_bPrimary = bIsPrimary;
      InitializeVariables();
    }
    /// <summary>
    /// Extracts primary axis from the array of BiffRecords.
    /// </summary>
    /// <param name="application">Application object for the axis.</param>
    /// <param name="parent">Parent object for the axis.</param>
    /// <param name="data">Array of BiffRecords with axis data.</param>
    /// <param name="iPos">
    /// Position of the first axis record in the data array.
    /// </param>
    [ CLSCompliant( false ) ]
    public ChartAxisImpl( IApplication application, object parent
      , IList<BiffRecordRaw> data, ref int iPos )
      : this( application, parent, data, ref iPos, true )
    {
    }
    /// <summary>
    /// Extracts axis from the array of BiffRecords.
    /// </summary>
    /// <param name="application">Application object for the axis.</param>
    /// <param name="parent">Parent object for the axis.</param>
    /// <param name="data">Array of BiffRecords with axis data.</param>
    /// <param name="iPos">
    /// Position of the first axis record in the data array.
    /// </param>
    /// <param name="isPrimary">
    /// True if it is primary axis; otherwise False.
    /// </param>
    [ CLSCompliant( false ) ]
    public ChartAxisImpl( IApplication application, object parent
      , IList<BiffRecordRaw> data, ref int iPos, bool isPrimary )
      : this( application, parent )
    {
      Parse( data, ref iPos, isPrimary );
    }
    /// <summary>
    /// Finds parent objects.
    /// </summary>
    private void SetParents()
    {
      m_parentAxis = ( ChartParentAxisImpl )FindParent( typeof( ChartParentAxisImpl ) );
      
      if( m_parentAxis == null )
        throw new ArgumentNullException( "There is no parent axis." );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Type of the axis.
    /// </summary>
    public ExcelAxisType AxisType
    {
      get
      {
        return m_axisType;
      }
      set
      {
        m_axisType = value;
      }
    }
    /// <summary>
    /// True if this is primary axis; False if secondary.
    /// </summary>
    public bool IsPrimary
    {
      get
      {
        return m_bPrimary;
      }
      set
      {
        m_bPrimary = value;
      }
    }
    /// <summary>
    /// Title of the axis.
    /// </summary>
    public string Title
    {
      get
      {
        if( m_titleArea == null )
          return null;

        return m_titleArea.Text;
      }
      set
      {
        TitleArea.Text = value;
      }
    }
    /// <summary>
    /// Text rotation angle. Should be integer value between -90 and 90.
    /// </summary>
    public int TextRotationAngle
    {
      get
      {
        ExcelVersion version = ParentWorkbook.Version;
        bool bNewVersion = version !=ExcelVersion.Excel97to2003;
        return ( bNewVersion ) ?
          -m_chartTick.RotationAngle :
          m_chartTick.RotationAngle;
      }
        set
        {
            if (value < -90 || value > 90)
                m_chartTick.RotationAngle = 0;
            else
            {

                ExcelVersion version = ParentWorkbook.Version;
                bool bNewVersion = version != ExcelVersion.Excel97to2003;

                m_chartTick.RotationAngle = (short)(bNewVersion ?
                  -value :
                  value);
                m_chartTick.IsAutoRotation = false;
            }
        }
    }
    /// <summary>
    /// Gets value indicating whether text rotation angle is autoselected.
    /// </summary>
    public bool IsAutoTextRotation
    {
      get
      {
        return m_chartTick.IsAutoRotation;
      }
    }
    /// <summary>
    /// Returns text area for the axis title. Read-only.
    /// </summary>
    public IChartTextArea TitleArea
    {
      get
      {
        if( m_titleArea == null )
        {
          m_titleArea = new ChartTextAreaImpl( Application, this, TextLinkType );
          m_titleArea.Bold = true;
          m_titleArea.Size = ChartAxisParser.DefaultFontSize;
        }

        return m_titleArea;
      }
    }
    /// <summary>
    /// Returns font used for axis text displaying. Read-only.
    /// </summary>
    public IFont Font
    {
      get
      {
        if( m_font == null )
        {
          FontImpl font = ( FontImpl )ParentWorkbook.InnerFonts[ 0 ];
          m_font = new FontWrapper( font );
        }

        if (!IsChartFont)
        {
            IsDefaultTextSettings = false;
            m_paraType = ChartParagraphType.CustomDefault;
        }

        return m_font;
      }
      set
      {
          if (value!=m_font)
          {
              if(!IsChartFont)
              IsDefaultTextSettings = false;
              m_font = (FontWrapper)value;
              m_isChartFont = false;		  
          }
       }
    }
    /// <summary>
    /// Gets or Sets the value indicating whether the aixs font settings are applied from the chart default font settings. 
    /// </summary>
    internal bool IsChartFont
    {
        get
        {
            return m_isChartFont;
        }
        set
        {
            m_isChartFont = value;
        }
        
    }
    /// <summary>
    /// Returns major gridLines. Read-only.
    /// </summary>
    public IChartGridLine MajorGridLines
    {
      get
      {
        return m_majorGrid;
      }
    }
    /// <summary>
    /// Returns minor gridLines. Read-only.
    /// </summary>
    public IChartGridLine MinorGridLines
    {
      get
      {
        return m_minorGrid;
      }
    }
    /// <summary>
    /// Gets or sets if axis has minor gridlines.
    /// </summary>
    public bool HasMinorGridLines
    {
      get
      {
        return m_bHasMinor;
      }
      set
      {
        if( value != HasMinorGridLines )
        {
          ChartImpl chart = m_parentAxis.ParentChart;

          if( !chart.TypeChanging && !chart.CheckForSupportGridLine() )
            throw new ApplicationException( "This chart type does not support gridlines." );

          m_bHasMinor = value;
          m_minorGrid = value ? new ChartGridLineImpl( Application, this
            , ExcelAxisLineIdentifier.MinorGridLine ) : null;
        }
      }
    }
    /// <summary>
    /// Gets or sets if axis has major gridlines.
    /// </summary>
    public bool HasMajorGridLines
    {
      get
      {
        return m_bHasMajor;
      }
      set
      {
        if( value != HasMajorGridLines )
        {
          ChartImpl chart = m_parentAxis.ParentChart;

          if( !chart.TypeChanging && !chart.CheckForSupportGridLine() )
            throw new ApplicationException( "This chart type does not support gridlines." );

          m_bHasMajor = value;
          m_majorGrid = value ? new ChartGridLineImpl( Application, this
            , ExcelAxisLineIdentifier.MajorGridLine ) : null;
        }
      }
    }
    public bool isNumber
    {
        get
        {
            return m_iNumberFormat != -1;
        }        
    }
    /// <summary>
    /// Returns chart parent axis. Read-only.
    /// </summary>
    protected ChartParentAxisImpl ParentAxis
    {
      get
      {
        return m_parentAxis;
      }
    }
    /// <summary>
    /// Gets or sets format index.
    /// </summary>
    public int NumberFormatIndex
    {
      get
      {
        return m_iNumberFormat;
      }
      set
      {
        m_iNumberFormat = value;
      }
    }
    /// <summary>
    /// Gets or sets number format string.
    /// </summary>
    public string NumberFormat
    {
      get
      {
        int index = m_iNumberFormat;
        FormatsCollection formatColl = ParentWorkbook.InnerFormats;

        if( m_iNumberFormat == DEF_NUMBER_FORMAT_INDEX || !formatColl.Contains( m_iNumberFormat ) )
          index = DEF_GENERAL_FORMAT;

        FormatImpl format = formatColl[ index ];

        return format.FormatString;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "formatString" );

        if( value.Length == 0 )
          throw new ArgumentException( "value - string cannot be empty" );

        m_iNumberFormat = ParentWorkbook.InnerFormats.FindOrCreateFormat( value );
      }
    }
    /// <summary>
    /// Represents minor tick marks.
    /// </summary>
    public ExcelTickMark MinorTickMark
    {
      get
      {
        return m_chartTick.MinorMark;
      }
      set
      {
        m_chartTick.MinorMark = value;
      }
    }
    /// <summary>
    /// Represents major tick marks.
    /// </summary>
    public ExcelTickMark MajorTickMark
    {
      get
      {
        return m_chartTick.MajorMark;
      }
      set
      {
        m_chartTick.MajorMark = value;
      }
    }
    /// <summary>
    /// Represents chart border. Read-only.
    /// </summary>
    public IChartBorder Border
    {
      get
      {
        if( m_border == null )
          m_border = new ChartBorderImpl( Application, this );

        return m_border;
      }
    }
    
    /// <summary>
    /// Represents tick label position.
    /// </summary>
    public ExcelTickLabelPosition TickLabelPosition
    {
      get
      {
        return m_chartTick.LabelPos;
      }
      set
      {
        m_chartTick.LabelPos = value;
      }
    }
    /// <summary>
    /// Indicates is axis is visible.
    /// </summary>
    public bool Visible
    {
      get
      {
        return !Deleted;
      }
      set
      {
        if( value != Visible )
        {
          Border.LinePattern = ( value )
            ? ExcelChartLinePattern.Solid
            : ExcelChartLinePattern.None;

          Deleted = !value;
        }
      }
    }
    /// <summary>
    /// Represents alignment for the tick label.
    /// </summary>
    public ExcelAxisTextDirection Alignment
    {
      get
      {
        return m_textDirection;
      }
      set
      {
        m_textDirection = value;
      }
    }
    /// <summary>
    /// True if plots data points from last to first.
    /// </summary>
    public bool IsReversed
    {
      get
      {
        return ReversePlotOrder;
      }
      set
      {
        ReversePlotOrder = value;
      }
    }
    /// <summary>
    /// True if plots data points from last to first.
    /// </summary>
    public abstract bool ReversePlotOrder { get; set; }
    /// <summary>
    /// Returns axis id.
    /// </summary>
    public int AxisId
    {
      get
      {
        return m_iAxisId;
      }
      internal set
      {
        m_iAxisId = value;
      }
    }
    /// <summary>
    /// Returns parent chart object. Read-only.
    /// </summary>
    public ChartImpl ParentChart
    {
      get
      {
        return m_parentAxis.m_parentChart;
      }
    }
    /// <summary>
    /// Gets or sets value indicating whether axis was deleted.
    /// </summary>
    public bool Deleted
    {
      get
      {
        return m_bDeleted;
      }
      set
      {
        m_bDeleted = value;
      }
    }
    public bool AutoTickLabelSpacing
    {
      get
      {
        return m_bAutoTickLabelSpacing;
      }
      set
      {
        m_bAutoTickLabelSpacing = value;
      }
    }
    public bool AutoTickMarkSpacing
    {
      get
      {
        return m_bAutoTickMarkSpacing;
      }
      set
      {
        m_bAutoTickMarkSpacing = value;
      }
    }
    /// <summary>
    /// Represents the Shadow.Read-only
    /// </summary>
    public IShadow Shadow
    {
      get
      {
        if (m_shadow == null)
          m_shadow = new ShadowImpl( Application, this );

        return m_shadow;
      }
    }
    /// <summary>
    /// Represents Shadow Propertes.Read-Only
    /// </summary>
    public IShadow ShadowProperties
    {
      get
      {
         return Shadow;
      }
    }

    /// <summary>
    /// This property indicates whether the shadow object has been created 
    /// </summary>
    public bool HasShadowProperties
    {
      get
      {
         return m_shadow != null;
      }
      internal set
      {
        if (value)
        {
          IShadow shadow = Shadow;
        }
        else
        {
          m_shadow = null;
        }
      }
    }

        /// <summary>
        /// Gets the chart3 D options.
        /// </summary>
        /// <value>The chart3 D options.</value>
        public IThreeDFormat Chart3DOptions
        {
            get
            {
                if (m_3D == null)
                    m_3D = new ThreeDFormatImpl(Application, this);

                return m_3D;
            }
        }
        /// <summary>
        /// Gets the chart3 D properties.
        /// </summary>
        /// <value>The chart3 D properties.</value>
        public IThreeDFormat Chart3DProperties
        {
            get
            {
                return Chart3DOptions;
            }
        }

        /// <summary>
        /// This property Indicates whether the Shadow object has been created(which includes the 3D properties)
        /// </summary>
        public bool Has3dProperties
        {
            get
            {
                return m_3D != null;
            }
            internal set
            {
                if (value)
                {
                    IThreeDFormat Threed = Chart3DOptions;
                }
                else
                {
                    m_3D = null;
                }
            }
        }

    internal ChartAxisPos? AxisPosition
    {
      get
      {
        return m_axisPos;
      }
      set
      {
        m_axisPos = value;
      }
    }
    internal bool IsSourceLinked
    {
        get
        {
            return m_sourceLinked;
        }
        set
        {
            m_sourceLinked = value;
        }
    }
    internal Stream TextStream
    {
        get
        {
            return m_textStream;
        }
        set
        {
            m_textStream = value;
        }
    }
    /// <summary>
    /// Return frame format of Axis. Read-only.
    /// </summary>
    public IChartFrameFormat FrameFormat
    {
        get
        {
            if (m_axisFormat == null)
            {
                InitFrameFormat();
            }
            return m_axisFormat;
        }
    }
      /// <summary>
      /// Indicates wheather the axis has title
      /// </summary>
    public bool HasAxisTitle
    {
        get
        {
            return m_titleArea != null;
        }
    }
    /// <summary>
    /// Represents the TextArea Paragraph 
    /// </summary>
    public ChartParagraphType ParagraphType
    {
        get
        {
            return m_paraType;
        }
        set
        {
            m_paraType = value;
        }
    }
    /// <summary>
    /// Represents the Default Text Settings
    /// </summary>
    internal bool IsDefaultTextSettings
    {
         get
         {
            return m_IsDefaultTextSettings;
         }
         set
         {
            m_IsDefaultTextSettings = value;     
             
         }
     }

    #endregion

    #region Class protected properties
    /// <summary>
    /// Creates title area. Read-only.
    /// </summary>
    protected abstract ExcelObjectTextLink TextLinkType { get; }
    /// <summary>
    /// Returns parent workbook. Read-only.
    /// </summary>
    protected WorkbookImpl ParentWorkbook
    {
      get
      {
        return m_parentAxis.m_parentChart.InnerWorkbook;
      }
    }
    #endregion

    #region Parse
    /// <summary>
    /// Extracts axis from the array of BiffRecords.
    /// </summary>
    /// <param name="data">Array of BiffRecords with axis data.</param>
    /// <param name="iPos">
    /// Position of the first axis record in the data array.
    /// </param>
    /// <param name="isPrimary">
    /// True if it is primary axis; otherwise False.
    /// </param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// If specified record is not ChartAxis record
    /// or next record is not Begin record.
    /// </exception>
    [ CLSCompliant( false ) ]
    protected void Parse( IList<BiffRecordRaw> data, ref int iPos, bool isPrimary )
    {
      m_bPrimary = isPrimary;
      ParagraphType = ChartParagraphType.CustomDefault;
      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartAxis );

      ChartAxisRecord axis = (ChartAxisRecord)record;
      iPos++;

      record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.Begin );

      while( record.TypeCode != TBIFFRecord.End )
      {
        switch( record.TypeCode )
        {
          case TBIFFRecord.ChartAxisLineFormat:
            ParseChartAxisLineFormat( data, ref iPos );
            break;

          case TBIFFRecord.ChartIfmt:
            ParseIfmt( record as ChartIfmtRecord );
            iPos++;
            break;

          case TBIFFRecord.ChartTick:
            ParseTickRecord( ( ChartTickRecord )data[ iPos ] );
            iPos++;
            break;

          case TBIFFRecord.ChartFontx:
            ChartFontxRecord fontx = ( ChartFontxRecord )data[ iPos ];
            ParseFontXRecord( fontx );
            iPos++;
            break;

          default:
            ParseData( record, data, ref iPos );
            iPos++;
            break;
        }

        record = ( BiffRecordRaw )data[ iPos ];
      }

      // Move after EndRecord
      iPos++;

      // TODO: change this switch

      switch( axis.AxisType )
      {
        case ChartAxisRecord.ChartAxisType.CategoryAxis:
          m_axisType = ExcelAxisType.Category;
          break;

        case ChartAxisRecord.ChartAxisType.ValueAxis:
          m_axisType = ExcelAxisType.Value;
          break;

        case ChartAxisRecord.ChartAxisType.SeriesAxis:
          m_axisType = ExcelAxisType.Serie;
          break;
      }
    }
    /// <summary>
    /// Parses axis line format.
    /// </summary>
    /// <param name="data">Array of BiffRecords with axis data.</param>
    /// <param name="iPos">
    /// Position of the ChartAxisLineFormat record in the data array.
    /// </param>
    private void ParseChartAxisLineFormat( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      ChartAxisLineFormatRecord axisLine = ( ChartAxisLineFormatRecord )data[ iPos ];

      switch( axisLine.LineIdentifier )
      {
        case ExcelAxisLineIdentifier.MajorGridLine:
          m_bHasMajor = true;
          m_majorGrid = new ChartGridLineImpl( Application, this, data, ref iPos );
          break;

        case ExcelAxisLineIdentifier.MinorGridLine:
          m_bHasMinor = true;
          m_minorGrid = new ChartGridLineImpl( Application, this, data, ref iPos );
          break;

        case ExcelAxisLineIdentifier.WallsOrFloor:
          ParseWallsOrFloor( data, ref iPos );
          break;

        case ExcelAxisLineIdentifier.AxisLineItself:
          iPos++;
          if (data[iPos].TypeCode == TBIFFRecord.ChartLineFormat)
            m_border = new ChartBorderImpl( Application, this, data, ref iPos );
          break;

        default:
          throw new NotSupportedException( "Unknown line identifier." );
      }
    }
    /// <summary>
    /// Parses ChartFontxRecord.
    /// </summary>
    /// <param name="fontx">Record to parse.</param>
    [ CLSCompliant( false ) ]
    protected void ParseFontXRecord( ChartFontxRecord fontx )
    {
      if( fontx == null )
        throw new ArgumentNullException( "fontx" );

      int iIndex = fontx.FontIndex;
      WorkbookImpl book = ParentWorkbook;
      FontImpl font = ( FontImpl )book.InnerFonts[ iIndex ];
      m_font = new FontWrapper( font );
    }
    /// <summary>
    /// Parses walls or floor.
    /// </summary>
    /// <param name="data">Record storage.</param>
    /// <param name="iPos">Position in storage.</param>
    protected abstract void ParseWallsOrFloor( IList<BiffRecordRaw> data, ref int iPos );
    /// <summary>
    /// Parses chart ifmt record.
    /// </summary>
    /// <param name="record">Record to parse.</param>
    [ CLSCompliant( false ) ]
    protected void ParseIfmt( ChartIfmtRecord record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      NumberFormatIndex = record.FormatIndex;
    }
    /// <summary>
    /// Parses data.
    /// </summary>
    /// <param name="record">Represents record data.</param>
    /// <param name="data">Represents records storage.</param>
    /// <param name="iPos">Represents position in storage.</param>
    [ CLSCompliant( false ) ]
    protected virtual void ParseData( BiffRecordRaw record, IList<BiffRecordRaw> data, ref int iPos )
    {}
    /// <summary>
    /// Parses chart tick record.
    /// </summary>
    /// <param name="chartTick">Represents chart tick record.</param>
    private void ParseTickRecord( ChartTickRecord chartTick )
    {
      if( chartTick == null )
        throw new ArgumentNullException( "chartTick" );

      m_chartTick = chartTick;
      m_textDirection = ExcelAxisTextDirection.Context;

      if( chartTick.IsLeftToRight )
      {
        m_textDirection = ExcelAxisTextDirection.LeftToRight;
      }
      else if( chartTick.IsRightToLeft )
      {
        m_textDirection = ExcelAxisTextDirection.RightToLeft;
      }
    }
    #endregion

    #region Serialization
    /// <summary>
    /// Serializes axis.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive all records.</param>
    [ CLSCompliant( false ) ]
    public virtual void Serialize( OffsetArrayList records )
    {
      throw new NotSupportedException( "This method should not be called." );
    }
    /// <summary>
    /// Serializes title of the axis.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive all records.</param>
    [ CLSCompliant( false ) ]
    public void SerializeAxisTitle( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_titleArea != null )
        m_titleArea.Serialize( records );
    }
    /// <summary>
    /// Saves font.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive all records.</param>
    [ CLSCompliant( false ) ]
    protected void SerializeFont( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_font != null )
      {
        ChartFontxRecord fontx = ( ChartFontxRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.ChartFontx );
        fontx.FontIndex = ( ushort )m_font.Index;

        records.Add( fontx );
      }
    }
    /// <summary>
    /// Serializes Grid lines.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive all records.</param>
    [ CLSCompliant( false ) ]
    protected void SerializeGridLines( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_majorGrid != null )
        m_majorGrid.Serialize( records );

      if( m_minorGrid != null )
        m_minorGrid.Serialize( records );
    }
    /// <summary>
    /// Serializes number format.
    /// </summary>
    /// <param name="records">Record storage.</param>
    [ CLSCompliant( false ) ]
    protected void SerializeNumberFormat( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( NumberFormatIndex != DEF_NUMBER_FORMAT_INDEX )
      {
        ChartIfmtRecord Ifmt = ( ChartIfmtRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.ChartIfmt );

        Ifmt.FormatIndex = ( ushort )NumberFormatIndex;
        records.Add( Ifmt );
      }
    }
    /// <summary>
    /// Serializes axis border.
    /// </summary>
    /// <param name="records">Record storage.</param>
    [ CLSCompliant( false ) ]
    protected void SerializeAxisBorder( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( m_border == null )
        return;

      ChartAxisLineFormatRecord lineFormat = ( ChartAxisLineFormatRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartAxisLineFormat );

      lineFormat.LineIdentifier = ExcelAxisLineIdentifier.AxisLineItself;

      records.Add( lineFormat );

      m_border.Serialize( records );
    }
    /// <summary>
    /// Serializes tick record.
    /// </summary>
    /// <param name="records">Record storage.</param>
    [ CLSCompliant( false ) ]
    protected void SerializeTickRecord( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      ChartTickRecord record = ( ChartTickRecord )m_chartTick.Clone();

      record.IsLeftToRight = false;
      record.IsRightToLeft = false;

      if( Alignment == ExcelAxisTextDirection.LeftToRight )
      {
        record.IsLeftToRight = true;
      }
      else if( Alignment == ExcelAxisTextDirection.RightToLeft )
      {
        record.IsRightToLeft = true;
      }

      records.Add( record );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Sets some important variables to the initial state.
    /// </summary>
    protected virtual void InitializeVariables()
    {
      SetParents();
      m_paraType = ChartParagraphType.Default;
      InitializeTickRecord();
    }
    /// <summary>
    /// Initializes internal tick record.
    /// </summary>
    private void InitializeTickRecord()
    {
      m_chartTick = ( ChartTickRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartTick );
      m_chartTick.MajorMark = ExcelTickMark.TickMark_Outside;
      m_chartTick.LabelPos = ExcelTickLabelPosition.TickLabelPosition_NextToAxis;
      m_chartTick.IsAutoTextColor = true;
    }
    /// <summary>
    /// Sets title area.
    /// </summary>
    /// <param name="titleArea">Title area to set.</param>
    protected internal void SetTitleArea( ChartTextAreaImpl titleArea )
    {
      if( titleArea == null )
        throw new ArgumentNullException( "titleArea" );

      m_titleArea = titleArea;
    }
    /// <summary>
    /// Clone current object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="dicFontIndexes">Dictionary with new indexes.</param>
    /// <param name="dicNewSheetNames">Dictionary with new worksheet names.</param>
    /// <returns>Returns cloned object.</returns>
    public virtual ChartAxisImpl Clone( object parent, Dictionary<int, int> dicFontIndexes,
      Dictionary<string, string> dicNewSheetNames )
    {
      ChartAxisImpl result = ( ChartAxisImpl )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();

      result.NumberFormat = NumberFormat;
      result.m_axisType = m_axisType;
      result.m_bIsDisposed = m_bIsDisposed;
      result.m_bLineFormat = m_bLineFormat;
      result.m_bPrimary = m_bPrimary;

      if( m_chartTick != null )
      {
        result.m_chartTick = ( ChartTickRecord )m_chartTick.Clone();
      }

      if( m_border != null )
        result.m_border = m_border.Clone( result );

      if( m_titleArea != null )
      {
        result.m_titleArea = ( ChartTextAreaImpl )m_titleArea.Clone( result, dicFontIndexes, dicNewSheetNames );
      }

      if( m_majorGrid != null )
        result.m_majorGrid = ( ChartGridLineImpl )m_majorGrid.Clone( result );

      if( m_minorGrid != null )
        result.m_minorGrid = ( ChartGridLineImpl )m_minorGrid.Clone( result );

      if( m_font != null )
      {
        result.m_font = m_font.Clone( result.ParentWorkbook, this, dicFontIndexes );
      }

      return result;
    }
    /// <summary>
    /// Clones the current object.
    /// </summary>      
    public ChartAxisImpl Clone(FontWrapper font)
    {        
       ChartAxisImpl result = this.MemberwiseClone() as ChartAxisImpl;

        result.Font =(IFont) font.Clone(result);
        return result;      
    }
    /// Sets axis title.
    /// </summary>
    /// <param name="text">Title to set.</param>
    public void SetTitle( ChartTextAreaImpl text )
    {
      m_titleArea = text;
    }
    /// <summary>
    /// Updates surface tick record.
    /// </summary>
    /// <param name="value">Represents value type.</param>
    public void UpdateTickRecord( ExcelTickLabelPosition value )
    {
      m_chartTick.LabelPos = value;
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      m_titleArea.MarkUsedReferences( usedItems );
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      m_titleArea.UpdateReferenceIndexes( arrUpdatedIndexes );
    }
   /// <summary>
    /// Initializes frame format.
    /// </summary>
    protected void InitFrameFormat()
    {
        m_axisFormat = CreateFrameFormat();
        ChartFrameRecord frameRecord = m_axisFormat.FrameRecord;
        frameRecord.AutoSize = true;

        m_axisFormat.Border.LinePattern = ExcelChartLinePattern.None;
        m_axisFormat.Border.AutoFormat = false;

        m_axisFormat.Interior.UseAutomaticFormat = false;
        //m_frame.Fill.FillType = ExcelFillType.Pattern;
        m_axisFormat.Interior.Pattern = ExcelPattern.None;
    }
    /// <summary>
    /// Creates frame format.
    /// </summary>
    /// <returns>Newly created frame format.</returns>
    protected virtual ChartFrameFormatImpl CreateFrameFormat()
    {
        return new ChartFrameFormatImpl(Application, this);
    }
    #endregion
  }
}
