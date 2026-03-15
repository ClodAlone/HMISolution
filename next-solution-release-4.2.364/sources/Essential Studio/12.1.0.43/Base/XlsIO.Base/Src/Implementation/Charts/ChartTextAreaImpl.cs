#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

#region file using directives
using System;
using System.IO;
using System.Collections;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;

using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Interfaces.Charts;
using System.Collections.Generic;
#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif
#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Represents text area in the chart.
  /// </summary>
  public class ChartTextAreaImpl
    : CommonObject
    , IChartTextArea
    , IChartDataLabels
    , ISerializable
    , IInternalChartTextArea
  {
    #region Class members
    /// <summary>
    /// Text area font.
    /// </summary>
    private FontWrapper m_font;
    /// <summary>
    /// Chart text record.
    /// </summary>
    internal ChartTextRecord m_chartText = ( ChartTextRecord )
      BiffRecordFactory.GetRecord( TBIFFRecord.ChartText );
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Text frame.
    /// </summary>
    private ChartFrameFormatImpl m_frame;
    /// <summary>
    /// Text.
    /// </summary>
    private string m_strText;
    /// <summary>
    /// Object link.
    /// </summary>
    private ChartObjectLinkRecord m_link = ( ChartObjectLinkRecord )
      BiffRecordFactory.GetRecord( TBIFFRecord.ChartObjectLink );
    /// <summary>
    /// Data labels record.
    /// </summary>
    private ChartDataLabelsRecord m_dataLabels = ( ChartDataLabelsRecord )
      BiffRecordFactory.GetRecord( TBIFFRecord.ChartDataLabels );
    /// <summary>
    /// Chart ai record.
    /// </summary>
    private ChartAIRecord m_chartAi;
    /// <summary>
    /// Chart Al Runs record represents the rich text format
    /// </summary>
    private ChartAlrunsRecord m_chartAlRuns;
    /// <summary>
    /// Represents chart position record.
    /// </summary>
    private ChartPosRecord m_pos;
    /// <summary>
    /// Attached label layout
    /// </summary>
    private ChartAttachedLabelLayoutRecord m_attachedLabelLayout;
    /// <summary>
    /// Indicates if current text assign to trend object.
    /// </summary>
    private bool m_bIsTrend;
    /// <summary>
    /// Represents Excel 2007 layout data
    /// </summary>
    private IChartLayout m_layout;
    /// <summary>
    /// Represents the TextArea Paragraph 
    /// </summary>
    private ChartParagraphType m_paraType;
    /// <summary>
    /// Stream containing Overlay settings in Excel 2007 and above.
    /// </summary>
    private Stream m_overlayStream;
    /// <summary>
    /// Indicats whether to show text properties or not
    /// </summary>
    private bool m_bShowTextProperties = true;
    /// <summary>
    /// Indicats whether to show text size properties or not
    /// </summary>
    private bool m_bShowSizeProperties;
    /// <summary>
    /// Indicats whether to show text bold properties or not
    /// </summary>
    private bool m_bShowBoldProperties;
    private bool m_bIsFormula;
    /// <summary>
    /// Represents RTF string.
    /// </summary>
    protected IChartRichTextString m_rtfString;
    /// <summary>
    /// Represents Vertical Text Rotation.
    /// </summary>
    private Excel2007TextRotation m_TextRotation = Excel2007TextRotation.horz;
    #endregion

    #region Class static methods
    /// <summary>
    /// Unwraps specified record.
    /// </summary>
    /// <param name="record">Record to unwrap.</param>
    /// <returns>Unwrapped record.</returns>
    [ CLSCompliant( false ) ]
    public static BiffRecordRaw UnwrapRecord( BiffRecordRaw record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      if( record.TypeCode == TBIFFRecord.ChartWrapper )
      {
        ChartWrapperRecord wrapper = ( ChartWrapperRecord )record;
        return wrapper.Record;
      }

      return record;
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates objects sets its Application and Parent properties to specified values.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    public ChartTextAreaImpl( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();

      ChartImpl chart = ( ChartImpl )FindParent( typeof( ChartImpl ) );
      SetFontIndex( chart.DefaultTextIndex );

      m_chartText.IsAutoMode = true;
      m_chartText.IsGenerated = false;
      m_chartText.IsAutoText = false;
      m_chartText.IsAutoColor = true;
      m_chartText.HorzAlign = ExcelChartHorzAlignment.Center;
      m_chartText.VertAlign = ExcelChartVertAlignment.Center;
      m_link.LinkObject = ExcelObjectTextLink.DataLabel;

      m_chartAi = ( ChartAIRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartAI );
      m_chartAi.Reference = ChartAIRecord.ReferenceType.EnteredDirectly;

      m_chartAlRuns = (ChartAlrunsRecord)BiffRecordFactory.GetRecord(TBIFFRecord.ChartAlruns);

      m_pos = ( ChartPosRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartPos );
      m_pos.TopLeft = 2;
      m_pos.BottomRight = 2;
      m_paraType = ChartParagraphType.Default;
    }
    /// <summary>
    /// Creates objects sets its Application and Parent properties to specified values.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="textLink">Text link.</param>
    [ CLSCompliant( false ) ]
    public ChartTextAreaImpl( IApplication application, object parent, ExcelObjectTextLink textLink )
      : this( application, parent )
    {
      m_link.LinkObject = textLink;
    }
    /// <summary>
    /// Creates objects sets its Application and Parent properties to specified
    /// values and parses object data.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="data">Array with object's records.</param>
    /// <param name="iPos">Position of the first object's record in the data array.</param>
    public ChartTextAreaImpl( IApplication application, object parent, IList<BiffRecordRaw> data, ref int iPos )
      : this( application, parent )
    {
      iPos = Parse( data, iPos );
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void SetParents()
    {
      m_book = FindParent( typeof( WorkbookImpl ) ) as WorkbookImpl;

      if( m_book == null )
        throw new ArgumentNullException( "Can't find parent workbook." );
    }
    #endregion

    #region IFont Members
    /// <summary>
    /// True if the font is bold. Read / write Boolean.
    /// </summary>
    public bool Bold
    {
      get
      {
        return m_font.Bold;
      }
      set
      {
        m_font.Bold = value;
      }
    }
    /// <summary>
    /// Returns or sets the primary color of the object. Read / write ExcelKnownColors.
    /// </summary>
    public ExcelKnownColors Color
    {
      get
      {
        return m_font.Color;//m_chartText.ColorIndex;
      }
      set
      {
        if( m_chartText.ColorIndex != value )
        {
          m_chartText.ColorIndex = value;
          m_font.Color = value;
          m_chartText.IsAutoColor = false;
        }
      }
    }
    /// <summary>
    /// Gets / sets font color. If there is at least one free color, 
    /// define a new color; if not, search for the closest one in 
    /// workbook palette.
    /// </summary>
    public Color RGBColor
    {
      get
      {
        return m_font.RGBColor;//m_book.GetPaletteColor( Color );
      }
      set
      {
        //Color = m_book.GetNearestColor( value );
        m_font.RGBColor = value;
      }
    }
    /// <summary>
    /// True if the font style is italic. Read / write Boolean.
    /// </summary>
    public bool Italic
    {
      get
      {
        return m_font.Italic;
      }
      set
      {
        m_font.Italic = value;
      }
    }
    /// <summary>
    /// True if the font is an outline font. Read / write Boolean.
    /// </summary>
    public bool MacOSOutlineFont
    {
      get
      {
        return m_font.MacOSOutlineFont;
      }
      set
      {
        m_font.MacOSOutlineFont = value;
      }
    }
    /// <summary>
    /// True if the font is a shadow font or if the object has
    /// a shadow. Read / write Boolean.
    /// </summary>
    public bool MacOSShadow
    {
      get
      {
        return m_font.MacOSShadow;
      }
      set
      {
        m_font.MacOSShadow = value;
      }
    }
    /// <summary>
    /// Returns or sets the size of the font. Read / write Variant.
    /// </summary>
    public double Size
    {
      get
      {
        return m_font.Size;
      }
      set
      {
        m_font.Size = value;
        if (!this.ParentWorkbook.Loading)
        {
            ShowSizeProperties = true;
        }
      }
    }
    /// <summary>
    /// True if the font is struck through with a horizontal line.
    /// Read / write Boolean
    /// </summary>
    public bool Strikethrough
    {
      get
      {
        return m_font.Strikethrough;
      }
      set
      {
        m_font.Strikethrough = value;
      }
    }
    /// <summary>
    /// Gets or sets the offset value of superscript and subscript
    /// </summary>
    internal int Baseline
    {
        get
        {
            return m_font.Baseline;
        }
        set
        {
            m_font.Baseline = value;
        }
    }
    /// <summary>
    /// True if the font is formatted as subscript.
    /// False by default. Read / write Boolean.
    /// </summary>
    public bool Subscript
    {
      get
      {
        return m_font.Subscript;
      }
      set
      {
        m_font.Subscript = value;
      }
    }
    /// <summary>
    /// True if the font is formatted as superscript. False by default.
    /// Read/write Boolean
    /// </summary>
    public bool Superscript
    {
      get
      {
        return m_font.Superscript;
      }
      set
      {
        m_font.Superscript = value;
      }
    }
    /// <summary>
    /// Returns or sets the type of underline applied to the font. Can
    /// be one of the following ExcelUnderlineStyle constants.
    /// Read / write ExcelUnderline.
    /// </summary>
    public ExcelUnderline Underline
    {
      get
      {
        return m_font.Underline;
      }
      set
      {
        m_font.Underline = value;
      }
    }
    /// <summary>
    /// Returns or sets the font name. Read / write string.
    /// </summary>
    public string FontName
    {
      get
      {
        return m_font.FontName;
      }
      set
      {
        m_font.FontName = value;
      }
    }
    /// <summary>
    /// Gets / sets font vertical alignment.
    /// </summary>
    public ExcelFontVertialAlignment VerticalAlignment
    {
      get
      {
        return m_font.VerticalAlignment;
      }
      set
      {
        m_font.VerticalAlignment = value;
      }
    }
    /// <summary>
    /// Generates .Net font object corresponding to the current font.
    /// </summary>
    /// <returns>Generated .Net font.</returns>
    public Font GenerateNativeFont()
    {
      return m_font.GenerateNativeFont();
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Text.
    /// </summary>
    public string Text
    {
      get
      {
        return m_strText;
      }
      set
      {
        m_strText = value;
        
        if( m_bIsTrend || m_link.LinkObject == ExcelObjectTextLink.DisplayUnit )
          m_chartText.IsAutoText = false;

        m_chartText.IsDeleted = value == null;

        if (m_chartAlRuns != null && m_chartAlRuns.Runs != null && m_chartAlRuns.Runs.Length > 0)
            m_chartAlRuns = null;

        if (m_chartAi != null)
        {
            if (this.IsFormula)
            {
                m_chartAi.ParsedExpression = GetNameTokens();
                m_chartAi.Reference = ChartAIRecord.ReferenceType.Worksheet;
            }
        }
      }
    }
    /// <summary>
    /// Gets rich text.
    /// </summary>
    public IChartRichTextString RichText
    {
        get
        {
            CheckDisposed();

            if (m_rtfString == null)
            {
                CreateRichTextString();
            }

            return m_rtfString;
        }
    }
    /// <summary>
    /// Return frame format. Read-only.
    /// </summary>
    public IChartFrameFormat FrameFormat
    {
      get
      {
        if( m_frame == null )
        {
          InitFrameFormat();
        }

        return m_frame;
      }
    }
    /// <summary>
    /// Gets object link record. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ChartObjectLinkRecord ObjectLink
    {
      get
      {
        return m_link;
      }
    }
    /// <summary>
    /// Text rotation angle.
    /// </summary>
    public int TextRotationAngle
    {
      get
      {        
          if (m_chartText.TextRotation > 90)
              return m_chartText.TextRotation - 90;
          else
              return -m_chartText.TextRotation;
      }
      set
      {        
          if (value > 0)
              m_chartText.TextRotation = (short)((short)90 + (short)value);
          else
              m_chartText.TextRotation = ((short)-value);
      }
    }
    /// <summary>
    /// Gets value indicating whether TextRotation was changed. Read-only.
    /// </summary>
    public bool HasTextRotation
    {
      get
      {
        return m_chartText.TextRotationOrNull != null;
      }
    }
    /// <summary>
    /// Represents Vertical Text Rotation.
    /// </summary>
    internal Excel2007TextRotation TextRotation
    {
        get
        {
            return m_TextRotation;
        }
        set
        {
            m_TextRotation = value;
        }
    }
    /// <summary>
    /// Returns chart text record. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ChartTextRecord TextRecord
    {
      get
      {
        return m_chartText;
      }
    }
    /// <summary>
    /// Gets / sets number format for the text area.
    /// </summary>
    public string NumberFormat
    {
      get
      {
        int iIndex = NumberFormatIndex;
        FormatImpl format = ( FormatImpl )m_book.InnerFormats[ iIndex ];
        return format.FormatString;
      }
      set
      {
        int iIndex = m_book.InnerFormats.FindOrCreateFormat( value );
        ChartAI.NumberFormatIndex = ( ushort )iIndex;
      }
    }
    /// <summary>
    /// Gets index to the number format. Read-only.
    /// </summary>
    public int NumberFormatIndex
    {
      get
      {
        return ( m_chartAi != null ) ? ( int )m_chartAi.NumberFormatIndex : 0;
      }
    }
    /// <summary>
    /// Returns ChartAIRecord for the text area (creates it if necessary). Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ChartAIRecord ChartAI
    {
      get
      {
        if( m_chartAi == null )
        {
          m_chartAi = ( ChartAIRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartAI );
        }

        return m_chartAi;
      }
    }
    /// <summary>
    /// Return ChartAlRunsRecord for the text area (creates if necessary). Read-only.
    /// </summary>
    [CLSCompliant(false)]
    public ChartAlrunsRecord ChartAlRuns
    {
        get
        {
            if (m_chartAlRuns == null)
                m_chartAlRuns = ( ChartAlrunsRecord )BiffRecordFactory.GetRecord(TBIFFRecord.ChartAlruns);

            return m_chartAlRuns;
        }
    }
    /// <summary>
    /// Returns true if contain dataLabels otherwise false. Read-only.
    /// </summary>
    public bool ContainDataLabels
    {
      get
      {
        return !( m_dataLabels == null );
      }
    }
    /// <summary>
    /// Display mode of the background.
    /// </summary>
    public ExcelChartBackgroundMode BackgroundMode
    {
      get
      {
        return m_chartText.BackgroundMode;
      }
      set
      {
        m_chartText.BackgroundMode = value;
        IsAutoMode = false;
      }
    }
    /// <summary>
    /// True if background is set to automatic.
    /// </summary>
    public bool IsAutoMode
    {
      get
      {
        return m_chartText.IsAutoMode;
      }
      set
      {
        m_chartText.IsAutoMode = value;
      }
    }
    /// <summary>
    /// Indicates if current text assign to trend object.
    /// </summary>
    public bool IsTrend
    {
      get
      {
        return m_bIsTrend;
      }
      set
      {
        m_bIsTrend = value;

        if( m_strText == null || m_strText.Length == 0 )
          m_chartText.IsAutoText = true;
      }
    }
    /// <summary>
    /// Indicates whether color has automatic color. Read-only.
    /// </summary>
    public bool IsAutoColor
    {
      get
      {
        return m_chartText.IsAutoColor;
      }
    }
    /// <summary>
    /// Gets or sets Excel 2007 layout data
    /// </summary>
    public IChartLayout Layout
    {
        get
        {
            if (m_layout == null)
                m_layout = new ChartLayoutImpl(Application, this, this.Parent);

            return m_layout;
        }
        set
        {
            m_layout = value;
        }
    }
    /// <summary>
    /// Gets or sets the overlay stream.
    /// </summary>
    /// <value>The overlay stream.</value>
    internal Stream OverlayStream
    {
        get
        {
            return m_overlayStream;
        }
        set
        {
            m_overlayStream = value;
        }
    }
    /// <summary>
    /// Returns parent workbook object.
    /// </summary>
    public WorkbookImpl ParentWorkbook
    {
      get
      {
        return m_book;
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
    /// Gets or sets a value indicating whether to show text properties or not
    /// </summary>
    internal bool ShowTextProperties
    {
        get
        {
            return m_bShowTextProperties;
        }
        set
        {
            m_bShowTextProperties = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether to show text size properties or not
    /// </summary>
    internal bool ShowSizeProperties
    {
        get
        {
            return m_bShowSizeProperties;
        }
        set
        {
            m_bShowSizeProperties = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether to show text bold properties or not
    /// </summary>
    internal bool ShowBoldProperties
    {
        get
        {
            return m_bShowBoldProperties;
        }
        set
        {
            m_bShowBoldProperties = value;
        }
    }
    #endregion

    #region Class parse methods
    /// <summary>
    /// Parses chart text area.
    /// </summary>
    /// <param name="data">BiffRecords array to parse.</param>
    /// <param name="iPos">Starting data position.</param>
    /// <returns>Position after parsing.</returns>
    [ CLSCompliant( false ) ]
    public int Parse( IList<BiffRecordRaw> data, int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      if( iPos < 0 || iPos >= data.Count )
        throw new ArgumentOutOfRangeException( "iPos", "Value cannot be less than 0 and greater than data.Length" );

      m_chartText = ( ChartTextRecord )UnwrapRecord( ( BiffRecordRaw )data[ iPos ] );
      iPos++;

      BiffRecordRaw raw = ( BiffRecordRaw )UnwrapRecord( ( BiffRecordRaw )data[ iPos ] );
      iPos++;
      raw.CheckTypeCode( TBIFFRecord.Begin );

      m_dataLabels = null;

      while( raw.TypeCode != TBIFFRecord.End )
      {
        raw = ( BiffRecordRaw )data[ iPos ];
        raw = UnwrapRecord( raw );
        iPos++;
        iPos = ParseRecord( raw, data, iPos );
      }

      return iPos;
    }
    /// <summary>
    /// Parses ChartFontxRecord.
    /// </summary>
    /// <param name="fontx">Record to parse.</param>
    private void ParseFontx( ChartFontxRecord fontx )
    {
      if( fontx == null )
        throw new ArgumentNullException( "fontx" );

      SetFontIndex( fontx.FontIndex );
    }
    /// <summary>
    /// Parses single record.
    /// </summary>
    /// <param name="record">Record to parse.</param>
    /// <param name="data">Data array with records to parse (used for complex object parsing).</param>
    /// <param name="iPos">Position in data array after current record.</param>
    /// <returns>Position after record parsing.</returns>
    [ CLSCompliant( false ) ]
    protected int ParseRecord( BiffRecordRaw record, IList<BiffRecordRaw> data, int iPos )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      //m_dataLabels = null;

      switch( record.TypeCode )
      {
        case TBIFFRecord.ChartFontx:
          ParseFontx( ( ChartFontxRecord )record );
          this.ParagraphType = ChartParagraphType.CustomDefault;
          break;

        case TBIFFRecord.ChartAlruns:
          m_chartAlRuns = (ChartAlrunsRecord)record;
          break;

        case TBIFFRecord.ChartAI:
          m_chartAi = ( ChartAIRecord )record;
          if (m_chartAi != null && m_chartAi.FormulaSize > 0)
              IsFormula = true;
          break;

        case TBIFFRecord.ChartSeriesText:
          ChartSeriesTextRecord seriesText = ( ChartSeriesTextRecord )record;
          if (IsFormula && m_chartAi != null && m_chartAi.ParsedExpression != null)
              m_strText = m_book.FormulaUtil.ParsePtgArray(m_chartAi.ParsedExpression);
          else
              m_strText = seriesText.Text;
          break;

        case TBIFFRecord.ChartFrame:
          iPos--;
          InitFrameFormat();
          m_frame.Parse( data, ref iPos );
          break;

        case TBIFFRecord.ChartObjectLink:
          m_link = ( ChartObjectLinkRecord )record;
          break;

        case TBIFFRecord.ChartDataLabels:
          m_dataLabels = ( ChartDataLabelsRecord )record;
          break;

        case TBIFFRecord.ChartPos:
          m_pos = ( ChartPosRecord )record;
          break;

        case TBIFFRecord.ChartAttachedLabelLayout:
          if (m_attachedLabelLayout == null) 
            m_attachedLabelLayout = (Layout.ManualLayout as ChartManualLayoutImpl).AttachedLabelLayout;
          m_attachedLabelLayout = (ChartAttachedLabelLayoutRecord)record;
          break;

          //          case TBIFFRecord.End:
          //            break;
          //
          //          default:
          //            ParseOtherRecords( raw );
          //            break;
      }

      m_font.ColorObject.SetIndexed( m_chartText.ColorIndex );
      return iPos;
    }
    #endregion

    #region Class serialization methods
    /// <summary>
    /// Indicates whether object should be serialized. Read-only.
    /// </summary>
    protected virtual bool ShouldSerialize
    {
      get
      {
        return HasText ||
          m_bIsTrend ||
          m_link.LinkObject == ExcelObjectTextLink.DataLabel ||
          m_link.LinkObject == ExcelObjectTextLink.DisplayUnit ||
          m_chartText.IsDeleted ||
          !m_chartText.IsAutoColor ||
          !m_chartText.IsAutoMode ||
          !m_chartText.IsAutoText;
      }
    }
    /// <summary>
    /// Indicates whether text area contains text.
    /// </summary>
    public bool HasText
    {
      get
      {
        return ( m_strText != null );
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is formula.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is formula; otherwise, <c>false</c>.
    /// </value>
    internal bool IsFormula
    {
        get
        {
            return m_bIsFormula;
        }
        set
        {
             m_bIsFormula=value;
        }
    }
    /// <summary>
    /// Saves chart text area into OffsetArrayList.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all chart's records.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    /// When specified OffsetArrayList is NULL.
    /// </exception>
    [ CLSCompliant( false ) ]
    public virtual void Serialize( IList<IBiffStorage> records )
    {
      Serialize( records, false );
    }
    /// <summary>
    /// Saves chart text area into OffsetArrayList.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all chart's records.
    /// </param>
    /// <param name="bIsLegendEntry">If true - serialize as legend entry text; otherwise false.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    /// When specified OffsetArrayList is NULL.
    /// </exception>
    [CLSCompliant( false )]
    public void Serialize( IList<IBiffStorage> records, bool bIsLegendEntry )
    {
      Serialize( records, bIsLegendEntry, true );
    }
    [ CLSCompliant( false ) ]
    public void Serialize( IList<IBiffStorage> records, bool bIsLegendEntry, bool bSerializeFontX )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( !ShouldSerialize ) return;

      if( m_bIsTrend )
        UpdateAsTrend();

      m_chartText.ColorIndex = m_font.ColorObject.GetIndexed( m_book );

      SerializeRecord( records, m_chartText );
      SerializeRecord( records, BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );
      bool bIsDataLabel = m_link.LinkObject == ExcelObjectTextLink.DataLabel;

      if( !bIsDataLabel )
        m_chartText.DataLabelPlacement = ExcelDataLabelPosition.Automatic;

      if( m_pos != null )
        SerializeRecord( records, m_pos );

      if( bSerializeFontX )
        SerializeFontx( records );
      
      //Number of rich text format runs. Must be >= 3 and <= 256
      if ((m_chartAlRuns != null) && (m_chartAlRuns.Runs.Length >= 3 && m_chartAlRuns.Runs.Length <= 256))
        SerializeRecord(records, m_chartAlRuns);

      SerializeRecord( records, m_chartAi );

      if( bIsLegendEntry )
      {
        SerializeRecord( records, BiffRecordFactory.GetRecord( TBIFFRecord.End ) );

        return;
      }

      if( m_strText != null && m_strText.Length > 0 )
      {
        ChartSeriesTextRecord seriesText = ( ChartSeriesTextRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.ChartSeriesText );
        seriesText.Text = m_strText;

        SerializeRecord( records, seriesText );
      }

      if( m_frame != null ) m_frame.Serialize( records );

      SerializeRecord( records, m_link );

      if (m_attachedLabelLayout != null)
          SerializeRecord(records, m_attachedLabelLayout);

      if( !m_bIsTrend && bIsDataLabel && m_dataLabels != null )
      {
        SerializeRecord( records, m_dataLabels );
      }

      SerializeRecord( records, BiffRecordFactory.GetRecord( TBIFFRecord.End ) );
    }

    /// <summary>
    /// Saves fontx record.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive necessary records.</param>
    private void SerializeFontx( IList<IBiffStorage> records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      FontWrapper font = ( FontWrapper )m_font;
      int index = font.Wrapped.Index;

      if( index > 0 )
      {
        ChartFontxRecord fontx = ( ChartFontxRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartFontx );
        fontx.FontIndex = ( ushort )index;
        SerializeRecord( records, fontx );
      }
    }
    /// <summary>
    /// Saves single record into list of biff records.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive necessary records.</param>
    /// <param name="record">Record to serialize.</param>
    [ CLSCompliant( false ) ]
    protected virtual void SerializeRecord( IList<IBiffStorage> records, BiffRecordRaw record )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      if( record != null )
        //throw new ArgumentNullException( "record" );
        records.Add( ( BiffRecordRaw )record.Clone() );
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Creates frame format.
    /// </summary>
    /// <returns>Newly created frame format.</returns>
    protected virtual ChartFrameFormatImpl CreateFrameFormat()
    {
      return new ChartFrameFormatImpl( Application, this );
    }
    /// <summary>
    /// Initializes frame format.
    /// </summary>
    protected void InitFrameFormat()
    {
      m_frame = CreateFrameFormat();
      ChartFrameRecord frameRecord = m_frame.FrameRecord;
      frameRecord.AutoSize = true;

      m_frame.Border.LinePattern = ExcelChartLinePattern.None;
      m_frame.Border.AutoFormat = false;

      m_frame.Interior.UseAutomaticFormat = false;
      //m_frame.Fill.FillType = ExcelFillType.Pattern;
      m_frame.Interior.Pattern = ExcelPattern.None;
    }
    /// <summary>
    /// Sets internal font according to the font index.
    /// </summary>
    /// <param name="index">Font index to set.</param>
    internal void SetFontIndex( int index )
    {
      DetachEvents();

      FontImpl fontToWrap = ( FontImpl )m_book.InnerFonts[ index ];

      if( m_font == null )
      {
        //m_font = new FontWrapper( Application, this );
        m_font = new FontWrapper();
      }

      ( m_font as FontWrapper ).Wrapped = fontToWrap;

      AttachEvents();
    }
    /// <summary>
    /// Creates data labels object if necessary.
    /// </summary>
    private void CreateDataLabels()
    {
        if (m_dataLabels == null)
        {
            m_dataLabels = new ChartDataLabelsRecord();
        }
    }
    /// <summary>
    /// Clone current record.
    /// </summary>
    /// <param name="parent">Parent object for create new instance.</param>
    /// <param name="dicFontIndexes">Dictionary with new font indexes.</param>
    /// <param name="dicNewSheetNames">Dictionary with new worksheet names.</param>
    /// <returns>Returns clone of current object.</returns>
    public object Clone( object parent, Dictionary<int, int> dicFontIndexes, Dictionary<string, string> dicNewSheetNames )
    {
      ChartTextAreaImpl result = ( ChartTextAreaImpl )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();
      
      result.m_bIsDisposed = m_bIsDisposed;

      result.m_chartText = ( ChartTextRecord )CloneUtils.CloneCloneable( m_chartText );

      if( m_font != null )
      {
        result.m_font = ( ( FontWrapper )m_font ).Clone( result.m_book, result, dicFontIndexes );
      }

      if( m_frame != null )
      {
        result.m_frame = m_frame.Clone( result );
      }

      result.m_link = ( ChartObjectLinkRecord )CloneUtils.CloneCloneable( m_link );

      if( m_chartAi != null )
      {
        result.m_chartAi = ( ChartAIRecord )m_chartAi.Clone();
        result.NumberFormat = NumberFormat;

        Ptg[] arrTokens = result.m_chartAi.ParsedExpression;
        int iTokenCount = ( arrTokens != null ) ?
          arrTokens.Length :
          0;

        for( int i = 0; i < iTokenCount; i++ )
        {
          ISheetReference token = arrTokens[ i ] as ISheetReference;

          if( token != null )
          {
            int iRefIndex = token.RefIndex;
            int iNewRefIndex = iRefIndex;

            if( m_book.IsExternalReference( iRefIndex ) )
            {
            }
            else
            {
              string strSheetName = m_book.GetSheetNameByReference( iRefIndex );

              if( dicNewSheetNames != null )
              {
                if( dicNewSheetNames.ContainsKey( strSheetName ) )
                {
                  strSheetName = dicNewSheetNames[ strSheetName ];
                }
              }

              iNewRefIndex = result.m_book.AddSheetReference( strSheetName );
            }

            token.RefIndex = ( ushort )iNewRefIndex;
          }
        }
      }

      result.m_strText = m_strText;

      return result;
    }
    /// <summary>
    /// Creates object that is copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for create new instance.</param>
    /// <returns>Returns clone of current object.</returns>
    public object Clone( object parent )
    {
      return Clone( parent, null, null );
    }
    /// <summary>
    /// Updates Series index.
    /// </summary>
    /// <param name="iNewIndex">Index to set.</param>
    public void UpdateSerieIndex( int iNewIndex )
    {
      m_link.SeriesNumber = ( ushort )iNewIndex;
    }
    /// <summary>
    /// Updates record for serialize as trend line data label.
    /// </summary>
    public void UpdateAsTrend()
    {
      ObjectLink.DataPointNumber = ChartSerieImpl.DEF_FORMAT_ALLPOINTS_INDEX;
      m_chartText.IsShowLabel = true;
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      if( m_chartAi != null )
        FormulaUtil.MarkUsedReferences( m_chartAi.ParsedExpression, usedItems );
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      if( m_chartAi != null )
      {
        Ptg[] tokens = m_chartAi.ParsedExpression;

        if( FormulaUtil.UpdateReferenceIndexes( tokens, arrUpdatedIndexes ) )
          m_chartAi.ParsedExpression = tokens;
      }
    }
    /// <summary>
    /// Attaches all necessary events.
    /// </summary>
    private void AttachEvents()
    {
      if( m_font != null )
        m_font.ColorObject.AfterChange += ColorChangeEventHandler;
    }
    /// <summary>
    /// Detaches all events.
    /// </summary>
    private void DetachEvents()
    {
      if( m_font != null )
        m_font.ColorObject.AfterChange -= ColorChangeEventHandler;
    }
    /// <summary>
    /// Event handler for color change.
    /// </summary>
    private void ColorChangeEventHandler()
    {
      m_chartText.IsAutoColor = false;
    }
    public int FontIndex
    {
      get
      {
        return ( m_font != null ) ? m_font.Index : 0;
      }
    }
    private Ptg[] GetNameTokens()
    {
        Ptg[] tokens = null;
        string formula = m_strText;

        if (formula != null)
        {
            if (formula[0] == '=')
                formula = UtilityMethods.RemoveFirstCharUnsafe(formula);
            tokens = m_book.FormulaUtil.ParseString(formula);
        }

        return tokens;
    }
    #endregion

    #region IChartDataLabels properties
    /// <summary>
    /// Indicates whether series name is in data labels.
    /// </summary>
    public bool IsSeriesName
    {
      get
      {
        return ( m_dataLabels != null ) ? m_dataLabels.IsSeriesName : false;
      }
      set
      {
        m_dataLabels.IsSeriesName = value;
      }
    }
    /// <summary>
    /// Indicates whether category name is in data labels.
    /// </summary>
    public bool IsCategoryName
    {
      get
      {
        return ( m_dataLabels != null ) ? m_dataLabels.IsCategoryName : false;
      }
      set
      {
        m_dataLabels.IsCategoryName = value;
        //TextRecord.IsShowLabel = true;
      }
    }
    /// <summary>
    /// Indicates whether value is in data labels.
    /// </summary>
    public bool IsValue
    {
      get
      {
        return ( m_dataLabels != null ) ? m_dataLabels.IsValue : false;
      }
      set
      {
        if (m_dataLabels == null) CreateDataLabels();
        m_dataLabels.IsValue = value;
        TextRecord.IsShowValue = value;
      }
    }
    /// <summary>
    /// Indicates whether percentage is in data labels.
    /// </summary>
    public bool IsPercentage
    {
      get
      {
        return ( m_dataLabels != null ) ? m_dataLabels.IsPercentage : false;
      }
      set
      {
        m_dataLabels.IsPercentage = value;
        TextRecord.IsShowPercent = value;
      }
    }
    /// <summary>
    /// Indicates whether bubble size is in data labels.
    /// </summary>
    public bool IsBubbleSize
    {
      get
      {
        return ( m_dataLabels != null ) ? m_dataLabels.IsBubbleSize : false;
      }
      set
      {
        m_dataLabels.IsBubbleSize = value;
        //TextRecord.IsShowBubbleSizes = value;
      }
    }
    /// <summary>
    /// Indicates whether Leader Lines is in data labels.
    /// </summary>
    public bool ShowLeaderLines
    {
        get
        {
          throw new NotSupportedException("Liner lines are not supported.");
        }
        set
        {
            
        }
    }
    /// <summary>
    /// Delimiter.
    /// </summary>
    public string Delimiter
    {
      get
      {
        return ( m_dataLabels != null ) ? m_dataLabels.Delimiter : null;
      }
      set
      {
        m_dataLabels.Delimiter = value;
      }
    }
    /// <summary>
    /// Indicates whether legend key is in data labels.
    /// </summary>
    public bool IsLegendKey
    {
      get
      {
        return TextRecord.IsShowKey;
      }
      set
      {
        TextRecord.IsShowKey = value;
      }
    }
    /// <summary>
    /// Represents data labels position.
    /// </summary>
    public ExcelDataLabelPosition Position
    {
      get
      {
        return m_chartText.DataLabelPlacement;
      }
      set
      {
        if( value == ExcelDataLabelPosition.Moved )
          throw new NotSupportedException( "This flag doesnot support." );

        m_chartText.DataLabelPlacement = value;
      }
    }
    /// <summary>
    /// Return attached label layout record. Read-only
    /// </summary>
    public ChartAttachedLabelLayoutRecord AttachedLabelLayout
    {
        get
        {
            if (m_attachedLabelLayout == null)
            {
                m_attachedLabelLayout = (ChartAttachedLabelLayoutRecord)
                    BiffRecordFactory.GetRecord(TBIFFRecord.ChartAttachedLabelLayout);
            }
            return m_attachedLabelLayout;
        }
    }
    /// <summary>
    /// Gets or sets value indicating whether to show category label and value as percentage.
    /// </summary>
    public bool IsShowLabelPercent
    {
      get
      {
        return TextRecord.IsShowLabelPercent;
      }
      set
      {
        TextRecord.IsShowLabelPercent = value;
      }
    }
    #endregion

    #region IOptimizedUpdate members
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public void BeginUpdate()
    {
      // TODO: implement BeginUpdate if necessary
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public void EndUpdate()
    {
      // TODO: implement EndUpdate if necessary
    }
    #endregion

    #region IInternalChartTextArea Members
    /// <summary>
    /// Returns textarea's color object. Read-only.
    /// </summary>
    public ColorObject ColorObject
    {
      get
      {
        return m_font.ColorObject;
      }
    }
    /// <summary>
    /// Returns font index. Read-only.
    /// </summary>
    public int Index
    {
      get
      {
        return m_font.Index;
      }
    }
    /// <summary>
    /// Returns FontImpl for current font. Read-only.
    /// </summary>
    public FontImpl Font
    {
      get
      {
        return m_font.Font;
      }
    }

    #endregion

    #region Rich-Text methods
    /// <summary>
    /// Creates rich text string.
    /// </summary>
    protected void CreateRichTextString()
    {
        m_rtfString = new ChartRichTextString(Application, this);
    }
    #endregion


  }
}
