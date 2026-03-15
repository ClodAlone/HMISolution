#region Copyright
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
//
#endregion Copyright

#region file using directives
using System;
using System.Collections;

#if ( WINRT )
using Windows.UI;
#endif
#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;


#endif

using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Parser;
using System.IO;
using System.Collections.Generic;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Represents ChartLegend.
  /// </summary>
  [ CLSCompliant( false ) ]
	public class ChartLegendImpl
    : CommonObject
    , IChartLegend
	{
    #region Class constants
    /// <summary>
    /// Represents top-left position in pos record.
    /// </summary>
    private const int DEF_POSITION = 5;
    #endregion

    #region Class members
    /// <summary>
    /// Represents chart legend record.
    /// </summary>
    private ChartLegendRecord m_serieLegend;
    /// <summary>
    /// Represents chart pos record.
    /// </summary>
    private ChartPosRecord m_pos;
    /// <summary>
    /// Attached label layout
    /// </summary>
    private ChartAttachedLabelLayoutRecord m_attachedLabelLayout;
    /// <summary>
    /// Represents chart text record and sub records.
    /// </summary>
    private ChartTextAreaImpl m_text;
    /// <summary>
    /// Represents legend frame format.
    /// </summary>
    private ChartFrameFormatImpl m_frame;
    /// <summary>
    /// Show legend without overlapping. Default is True.
    /// </summary>
    private bool m_includeInLayout = true;
    /// <summary>
    /// Represents parent chart.
    /// </summary>
    private ChartImpl m_parentChart;
    /// <summary>
    /// Represents collection of legend entries.
    /// </summary>
    private ChartLegendEntriesColl m_collEntries;
    /// <summary>
    /// Represents Excel 2007 layout data
    /// </summary>
    private IChartLayout m_layout;
    /// <summary>
    /// Represents the TextArea Paragraph 
    /// </summary>
    private ChartParagraphType m_paraType;
    /// <summary>
    /// Represents the legend text properties stream
    /// </summary>
    private UnknownRecord m_legendTextPropsStream;
    /// <summary>
    /// Represents the default ChartTextArea settings
    /// </summary>
    private bool m_IsDefaultTextSettings;
    /// <summary>
    /// Represents the default ChartTextArea settings
    /// </summary>
    private bool m_IsChartTextArea;
    #endregion

    #region Class initialize methods
    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="application">Current application.</param>
    /// <param name="parent">Parent object.</param>
		public ChartLegendImpl( IApplication application, object parent )
      : base( application, parent )
		{
      m_serieLegend = ( ChartLegendRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartLegend );

      m_pos = ( ChartPosRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartPos );
      m_pos.TopLeft = DEF_POSITION;

      m_text = new ChartTextAreaImpl( application, this );
      m_collEntries = new ChartLegendEntriesColl( application, parent );

      SetParents();
      m_paraType = ChartParagraphType.Default;
		}
    /// <summary>
    /// Finds all parent objects.
    /// </summary>
    private void SetParents()
    {
      m_parentChart = ( ChartImpl )FindParent( typeof( ChartImpl ) );

      if( m_parentChart == null )
        throw new ApplicationException( "Can't find parent object." );
    }
    #endregion

    #region Parse methods
    /// <summary>
    /// Parsing current object.
    /// </summary>
    /// <param name="data">Records offset.</param>
    /// <param name="iPos">Position in offset.</param>
    public void Parse( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartLegend );
      m_serieLegend = ( ChartLegendRecord )data[ iPos ];

      iPos = iPos + 2;

      int count = 1;

      while( count != 0 )
      {
        record = ( BiffRecordRaw )data[ iPos ];

        switch( record.TypeCode )
        {
          case TBIFFRecord.Begin:
            iPos = BiffRecordRaw.SkipBeginEndBlock( data, iPos );
            break;

          case TBIFFRecord.End:
            count--;
            break;

          case TBIFFRecord.ChartPos:
            m_pos = ( ChartPosRecord )record;
            break;

          case TBIFFRecord.ChartAttachedLabelLayout:
            if (m_attachedLabelLayout == null)
                m_attachedLabelLayout = (Layout.ManualLayout as ChartManualLayoutImpl).AttachedLabelLayout;
            m_attachedLabelLayout = (ChartAttachedLabelLayoutRecord)record;
            break;

          case TBIFFRecord.ChartText:
            m_text = new ChartTextAreaImpl( Application, this );
            iPos =  m_text.Parse( data, iPos ) -  1;
            IsChartTextArea = true;
            m_text.ParagraphType = ChartParagraphType.CustomDefault;
            break;

          case TBIFFRecord.ChartFrame:
            m_frame = new ChartFrameFormatImpl( Application, this, false );
            m_frame.Parse( data, ref iPos );
            iPos--;
            break;

          case TBIFFRecord.ChartTextPropsStream:
            m_legendTextPropsStream = (UnknownRecord)data[iPos];
            break;

//          default:
//            throw new ArgumentException();
        }

        iPos++;
      }
    }
    #endregion

    #region Serialize methods
    /// <summary>
    /// Serialize current object.
    /// </summary>
    /// <param name="records">Represents record to serialize</param>
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      records.Add( ( ChartLegendRecord )m_serieLegend.Clone() );

      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );

      if( m_pos != null )
        records.Add( ( ChartPosRecord )m_pos.Clone() );

      if( m_text != null )
        m_text.Serialize( records, true );

      if( m_frame != null )
        m_frame.Serialize( records );

      if (m_attachedLabelLayout != null)
          SerializeRecord(records, m_attachedLabelLayout);

      //TODO: Need to add support for TextPropsStream records
      if (m_legendTextPropsStream != null)
          records.Add(m_legendTextPropsStream);

      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.End ) );
    }
    /// <summary>
    /// Saves single record into list of biff records.
    /// </summary>
    /// <param name="records">OffsetArrayList that will receive necessary records.</param>
    /// <param name="record">Record to serialize.</param>
    [CLSCompliant(false)]
    protected virtual void SerializeRecord(IList<IBiffStorage> records, BiffRecordRaw record)
    {
        if (records == null)
            throw new ArgumentNullException("records");

        if (record != null)
            records.Add((BiffRecordRaw)record.Clone());
    }
    #endregion

    #region IChartLegend properties
    /// <summary>
    /// Represents chart frame format.
    /// </summary>
    public IChartFrameFormat FrameFormat
    {
      get
      {
        if( m_frame == null )
        {
          m_frame = new ChartFrameFormatImpl( Application, this, true, false, true );
          m_frame.Interior.UseAutomaticFormat = true;

          ExcelVersion version = m_parentChart.Workbook.Version;

          if( version !=ExcelVersion.Excel97to2003 )
            m_frame.HasLineProperties = false;
        }

        return m_frame;
      }
    }
    /// <summary>
    /// Return text area of legend.
    /// </summary>
    public IChartTextArea TextArea
    {
      get
      {
        if( m_text == null )
          m_text = new ChartTextAreaImpl( Application, this );

        if (!IsChartTextArea)
        {
            IsDefaultTextSettings = false;
            m_text.ParagraphType = ChartParagraphType.CustomDefault;
        }

        return m_text;
      }
    }
    /// <summary>
    /// Show legend without overlapping. Default is True.
    /// </summary>
    public bool IncludeInLayout
    {
      get
      {
        return m_includeInLayout;
      }
      set
      {
        if (m_parentChart.Workbook.Version == ExcelVersion.Excel97to2003)
            throw new ArgumentException("This property is not supported for the current workbook version");
            
        if(value != m_includeInLayout)
           m_includeInLayout = value;
      }
    }
    /// <summary>
    /// X-position of upper-left corner. 1/4000 of chart plot.
    /// </summary>
    public int X
    {
      get
      {
        return LegendRecord.X;
      }
      set
      {
        SetCustomPosition();
        LegendRecord.X = value;
        PositionRecord.X1 = value;
      }
    }
    /// <summary>
    /// Y-position of upper-left corner. 1/4000 of chart plot.
    /// </summary>
    public int Y
    {
      get
      {
        return LegendRecord.Y;
      }
      set
      {
        SetCustomPosition();
        LegendRecord.Y = value;
        PositionRecord.Y1 = value;
      }
    }
    /// <summary>
    /// Type:
    /// 0 = bottom
    /// 1 = corner
    /// 2 = top
    /// 3 = right
    /// 4 = left
    /// 7 = not docked or inside the plot area
    /// </summary>
    public ExcelLegendPosition Position
    {
      get
      {
        return LegendRecord.Position;
      }
      set
      {
        if( value == ExcelLegendPosition.NotDocked )
        {
          SetCustomPosition();

          return;
        }

        SetDefPosition();

        IsVerticalLegend = !( value == ExcelLegendPosition.Bottom || value == ExcelLegendPosition.Top );

        LegendRecord.Position = value;
      }
    }
    /// <summary>
    /// True if vertical legend (a single column of entries);
    /// False if horizontal legend (multiple columns of entries).
    /// Manual-sized legends always have this bit set to False.
    /// </summary>
    public bool IsVerticalLegend
    {
      get
      {
        return LegendRecord.IsVerticalLegend;
      }
      set
      {
        LegendRecord.IsVerticalLegend = value;
      }
    }
    /// <summary>
    /// Represents legend entries collection. Read-only.
    /// </summary>
    public IChartLegendEntries LegendEntries
    {
      get
      {
        return m_collEntries;
      }
    }
    /// <summary>
    /// Represents the Default ChartTextArea Settings
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
    /// <summary>
    /// Gets or Sets the value indicating whether the ChartTextArea settings are applied from the chart default text settings. 
    /// </summary>
    internal bool IsChartTextArea
    {
        get
        {
            return m_IsChartTextArea;
        }
        set
        {
            m_IsChartTextArea = value;
        }

    }
    #endregion

    #region Class public properties
    /// <summary>
    /// X-size.
    /// </summary>
    public int Width
    {
      get
      {
        return LegendRecord.Width;
      }
      set
      {
        LegendRecord.Width = value;
      }
    }
    /// <summary>
    /// Y-size.
    /// </summary>
    public int Height
    {
      get
      {
        return LegendRecord.Height;
      }
      set
      {
        LegendRecord.Height = value;
      }
    }
    /// <summary>
    /// True if chart contains data table.
    /// </summary>
    public bool ContainsDataTable
    {
      get
      {
        return LegendRecord.ContainsDataTable;
      }
      set
      {
        LegendRecord.ContainsDataTable = value;
      }
    }
    /// <summary>
    /// Spacing:
    /// 0 = close
    /// 1 = medium
    /// 2 = open
    /// </summary>
    public ExcelLegendSpacing Spacing
    {
      get
      {
        return LegendRecord.Spacing;
      }
      set
      {
        LegendRecord.Spacing = value;
      }
    }
    /// <summary>
    /// Automatic positioning (True if legend is docked).
    /// </summary>
    public bool AutoPosition
    {
      get
      {
        return LegendRecord.AutoPosition;
      }
      set
      {
        LegendRecord.AutoPosition = value;
      }
    }
    /// <summary>
    /// Automatic series distribution (True in Microsoft Excel 5.0).
    /// </summary>
    public bool AutoSeries
    {
      get
      {
        return LegendRecord.AutoSeries;
      }
      set
      {
        LegendRecord.AutoSeries = value;
      }
    }
    /// <summary>
    /// X positioning is automatic.
    /// </summary>
    public bool AutoPositionX
    {
      get
      {
        return LegendRecord.AutoPositionX;
      }
      set
      {
        LegendRecord.AutoPositionX = value;
      }
    }
    /// <summary>
    /// Y positioning is automatic.
    /// </summary>
    public bool AutoPositionY
    {
      get
      {
        return LegendRecord.AutoPositionY;
      }
      set
      {
        LegendRecord.AutoPositionY = value;
      }
    }
    /// <summary>
    /// Gets / sets Excel 2007 layout data
    /// </summary>
    public IChartLayout Layout
    {
        get
        {
            if (m_layout == null)
                m_layout = new ChartLayoutImpl(Application, this, m_parentChart);

            return m_layout;
        }
        set
        {
            m_layout = value;
        }
    }
    /// <summary>
    /// Represents the Legend Paragraph 
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
    #endregion

    #region Class members
    /// <summary>
    /// Returns legend record. Read-only.
    /// </summary>
    private ChartLegendRecord LegendRecord
    {
      get
      {
        if( m_serieLegend == null )
        {
          m_serieLegend = ( ChartLegendRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.ChartLegend );
        }

        return m_serieLegend;
      }
    }
    /// <summary>
    /// Returns position record. Read-only.
    /// </summary>
    private ChartPosRecord PositionRecord
    {
      get
      {
        if( m_pos == null )
          m_pos = ( ChartPosRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartPos );

        return m_pos;
      }
    }
    /// <summary>
    /// Clones current object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="dicFontIndexes">Array with new font indexes.</param>
    /// <param name="dicNewSheetNames">Dictionary with new worksheet names.</param>
    /// <returns>Returns cloned object.</returns>
    public ChartLegendImpl Clone( object parent, Dictionary<int, int> dicFontIndexes,
      Dictionary<string, string> dicNewSheetNames )
    {
      ChartLegendImpl result = ( ChartLegendImpl )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();

      result.m_serieLegend = ( ChartLegendRecord )CloneUtils.CloneCloneable( m_serieLegend );
      result.m_pos = ( ChartPosRecord )CloneUtils.CloneCloneable( m_pos );

      result.m_collEntries = ( ChartLegendEntriesColl )m_collEntries.Clone( result,
        dicFontIndexes, dicNewSheetNames );

      if( m_frame != null )
        result.m_frame = m_frame.Clone( result );

      if( m_text != null )
        result.m_text = ( ChartTextAreaImpl )m_text.Clone( result, dicFontIndexes,
          dicNewSheetNames );

      return result;
    }
    #endregion

    #region IChartLegend method
    /// <summary>
    /// Clears chart legend.
    /// </summary>
    public void Clear()
    {
      m_frame.Clear();
      m_collEntries.Clear();
      m_pos = ( ChartPosRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartPos );
      m_serieLegend = ( ChartLegendRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartLegend );
    }
    /// <summary>
    /// Deletes chart legend.
    /// </summary>
    public void Delete()
    {
      m_parentChart.HasLegend = false;
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// Sets to default position
    /// </summary>
    private void SetDefPosition()
    {
      AutoPosition = true;
      AutoPositionX = true;
      AutoPositionY = true;

      LegendRecord.X = 0;
      LegendRecord.Y = 0;
      LegendRecord.Width = 0;
      LegendRecord.Height = 0;
      PositionRecord.X1 = 0;
      PositionRecord.X2 = 0;
      PositionRecord.Y1 = 0;
      PositionRecord.Y2 = 0;

      m_parentChart.ChartProperties.IsAlwaysAutoPlotArea = false;
    }
    /// <summary>
    /// Sets legend to custom position.
    /// </summary>
    private void SetCustomPosition()
    {
      AutoPosition = false;
      AutoPositionX = false;
      AutoPositionY = false;

      LegendRecord.Position = ExcelLegendPosition.NotDocked;
      m_parentChart.ChartProperties.IsAlwaysAutoPlotArea = true;
    }
    #endregion
	}
}
