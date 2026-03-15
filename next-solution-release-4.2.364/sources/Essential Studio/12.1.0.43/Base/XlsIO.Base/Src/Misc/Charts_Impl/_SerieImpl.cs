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
using System.Collections;

using Syncfusion.XlsIO.Interfaces.Charts;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
  /// <summary>
  /// Summary description for Series.
  /// </summary>
  public class SerieImpl
    : CommonObject
    , IChartSerie
  {
    #region Class Members
    /// <summary>
    /// 
    /// </summary>
    private LineStyleImpl m_Border;
    /// <summary>
    /// 
    /// </summary>
    private AreaImpl m_Area;
    /// <summary>
    /// 
    /// </summary>
    private SerieDataLabelsImpl m_DataLabels;
    /// <summary>
    /// 
    /// </summary>
    private ErrorBarsImpl m_YErrorBars;
    /// <summary>
    /// 
    /// </summary>
    private ErrorBarsImpl m_XErrorBars;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bPlotOnPrimary;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bVaryColors;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bSeriesLines;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bSizeIsArea;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bShowNegativeBubbles;
    /// <summary>
    /// 
    /// </summary>
    private int  m_iOverlap;
    /// <summary>
    /// 
    /// </summary>
    private int  m_iGapWidth;
    /// <summary>
    /// 
    /// </summary>
    private int  m_iSplitSeriesBy;
    /// <summary>
    /// 
    /// </summary>
    private int  m_iInSecondPlot;
    /// <summary>
    /// 
    /// </summary>
    private int  m_iSecondPlotSize;
    /// <summary>
    /// 
    /// </summary>
    private int  m_iBubbleSizeScale;
    /// <summary>
    /// 
    /// </summary>
    private ChartSeriesRecord.DataType m_xDataType;
    /// <summary>
    /// 
    /// </summary>
    private ChartSeriesRecord.DataType m_yDataType;
    /// <summary>
    /// 
    /// </summary>
    private ChartSeriesRecord.DataType m_bubbleDataType;
    /// <summary>
    /// 
    /// </summary>
    private int m_iCategoriesCount;
    /// <summary>
    /// 
    /// </summary>
    private int m_iValuesCount;
    /// <summary>
    /// 
    /// </summary>
    private int m_iBubbleSeriesCount;
    /// <summary>
    /// 
    /// </summary>
    private Dictionary<object, object> m_hashAi = new Dictionary<object, object>();
    /// <summary>
    /// 
    /// </summary>
    private ChartDataFormatRecord m_dataFormat;
    /// <summary>
    /// 
    /// </summary>
    private Chart3DDataFormatRecord m_3dDataFormat;
    /// <summary>
    /// 
    /// </summary>
    private int m_iChartGroup;
    #endregion

    #region Class constructors
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public SerieImpl( IApplication application, object parent )
      : base( application, parent )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    public SerieImpl( IApplication application, object parent, BiffRecordRaw[] data
      , ref int iPos )
      : this( application, parent )
    {
      Parse( data, ref iPos );
    }
    #endregion

    #region IChartSerie Members
    /// <summary>
    /// 
    /// </summary>
    public ILineStyle Border
    {
      get
      {
        return m_Border;
      }
      set
      {
        // TODO:  Add SerieImpl.Border setter implementation
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IArea Area
    {
      get
      {
        return m_Area;
      }
      set
      {
        // TODO:  Add SerieImpl.Area setter implementation
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public ISerieDataLabels DataLabels
    {
      get
      {
        return m_DataLabels;
      }
      set
      {
        // TODO:  Add SerieImpl.DataLabels setter implementation
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IChartErrorBars YErrorBars
    {
      get
      {
        return m_YErrorBars;
      }
      set
      {
        // TODO:  Add SerieImpl.YErrorBars setter implementation
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IChartErrorBars XErrorBars
    {
      get
      {
        return m_XErrorBars;
      }
      set
      {
        // TODO:  Add SerieImpl.XErrorBars setter implementation
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsPlotOnPrimary
    {
      get
      {
        return m_bPlotOnPrimary;
      }
      set
      {
        m_bPlotOnPrimary = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Overlap
    {
      get
      {
        return m_iOverlap;
      }
      set
      {
        m_iOverlap = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int GapWidth
    {
      get
      {
        return m_iGapWidth;
      }
      set
      {
        m_iGapWidth = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsVaryColors
    {
      get
      {
        return m_bVaryColors;
      }
      set
      {
        m_bVaryColors = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int SplitSeriesBy
    {
      get
      {
        return m_iSplitSeriesBy;
      }
      set
      {
        m_iSplitSeriesBy = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int InSecondPlot
    {
      get
      {
        return m_iInSecondPlot;
      }
      set
      {
        m_iInSecondPlot = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int SecondPlotSize
    {
      get
      {
        return m_iSecondPlotSize;
      }
      set
      {
        m_iSecondPlotSize = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsSeriesLines
    {
      get
      {
        return m_bSeriesLines;
      }
      set
      {
        m_bSeriesLines = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsSizeIsArea
    {
      get
      {
        return m_bSizeIsArea;
      }
      set
      {
        m_bSizeIsArea = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int BubbleSizeScale
    {
      get
      {
        return m_iBubbleSizeScale;
      }
      set
      {
        m_iBubbleSizeScale = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool ShowNegativeBubbles
    {
      get
      {
        return m_bShowNegativeBubbles;
      }
      set
      {
        m_bShowNegativeBubbles = value;
      }
    }

    #endregion

    #region Parse methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    private void Parse( BiffRecordRaw[] data, ref int iPos )
    {
      if( data[ iPos ].TypeCode != TBIFFRecord.ChartSeries )
        throw new ArgumentOutOfRangeException( "ChartSeries record was expected" );

      ParseSeriesRecord( (ChartSeriesRecord) data[ iPos ] );
      iPos++;

      if( data[ iPos ].TypeCode != TBIFFRecord.Begin )
        throw new ArgumentOutOfRangeException( "Begin record was expected" );
      
      iPos++;
      
      // TODO: implement
      while( data[ iPos ].TypeCode != TBIFFRecord.End )
      {
        switch( data[ iPos ].TypeCode )
        {
          case TBIFFRecord.ChartAI:
            ParseAIRecord( data, ref iPos );
            break;

          case TBIFFRecord.ChartDataFormat:
            ParseDataFormat( data, ref iPos );
            break;

          case TBIFFRecord.ChartSertocrt:
            ParseSertoCrt( data, ref iPos );
            break;

          default:
            iPos++;
            break;
        }
      }

      // lets move after EndRecord
      iPos++;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="series"></param>
    private void ParseSeriesRecord( ChartSeriesRecord series )
    {
      m_xDataType = series.StdX;
      m_yDataType = series.StdY;
      m_bubbleDataType = series.StdY;

      m_iValuesCount = series.ValuesCount;
      m_iCategoriesCount = series.CategoriesCount;
      m_iBubbleSeriesCount = series.BubbleSeriesCount;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    private void ParseAIRecord( BiffRecordRaw[] data, ref int iPos )
    {
      if( data[ iPos ].TypeCode != TBIFFRecord.ChartAI )
        throw new ArgumentOutOfRangeException( "ChartAI record was expected" );
      
      ChartAIRecord newAi = (ChartAIRecord) data[ iPos ];
      
      if( m_hashAi.Contains( newAi.IndexIdentifier ) )
        throw new ArgumentException( "AI record with such IndexIdentifier was already read" );

      m_hashAi.Add( newAi.IndexIdentifier, newAi );

      iPos++;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    private void ParseDataFormat( BiffRecordRaw[] data, ref int iPos )
    {
      if( data[ iPos ].TypeCode != TBIFFRecord.ChartDataFormat )
        throw new ArgumentOutOfRangeException( "ChartDataFormat record was expected" );

      m_dataFormat = (ChartDataFormatRecord) data[ iPos ].Clone();
      iPos++;

      if( data[ iPos ].TypeCode != TBIFFRecord.Begin )
        throw new ArgumentOutOfRangeException( "Begin record was expected" );

      iPos++;

      while( data[ iPos ].TypeCode != TBIFFRecord.End )
      {
        switch( data[ iPos ].TypeCode )
        {
          case TBIFFRecord.Chart3DDataFormat:
            Parse3DDataFormat( data, ref iPos );
            break;

          default:
            iPos++;
        }
      }
      
      // Let's move after EndRecord
      iPos++;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    private void Parse3DDataFormat( BiffRecordRaw[] data, ref int iPos )
    {
      if( data[ iPos ].TypeCode != TBIFFRecord.Chart3DDataFormat )
        throw new ArgumentOutOfRangeException( "Chart3DDataFormat record was expected" );

      m_3dDataFormat = (Chart3DDataFormatRecord) data[ iPos ].Clone();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    private void ParseSertoCrt( BiffRecordRaw[] data, ref int iPos )
    {
      data[ iPos ].CheckTypeCode( TBIFFRecord.ChartSertocrt );
      m_iChartGroup = ( (ChartSertocrtRecord) data[ iPos ] ).ChartGroup;
      iPos++;
    }
    #endregion
  }
}
