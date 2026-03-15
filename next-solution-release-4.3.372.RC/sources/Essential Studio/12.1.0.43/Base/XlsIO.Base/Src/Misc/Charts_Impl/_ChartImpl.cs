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
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
	/// <summary>
	/// Summary description for ChartImpl.
	/// </summary>
	public class ChartImpl : CommonObject, IChart
	{
    #region Class members
    /// <summary>
    /// 
    /// </summary>
    private ExcelChartType m_chartType = ExcelChartType.Column_Clustered;
    /// <summary>
    /// 
    /// </summary>
    private AxisImpl m_categoryAxis;
    /// <summary>
    /// 
    /// </summary>
    private AxisImpl m_valueAxis;
    /// <summary>
    /// 
    /// </summary>
    private AxisImpl m_serieAxis;
    /// <summary>
    /// 
    /// </summary>
    private AxisImpl m_secondaryCategoryAxis;
    /// <summary>
    /// 
    /// </summary>
    private AxisImpl m_secondaryValueAxis;
    /// <summary>
    /// 
    /// </summary>
    private IRange m_dataRange;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bSeriesInRows;
    /// <summary>
    /// 
    /// </summary>
    private ArrayList m_series = new ArrayList();
    /// <summary>
    /// 
    /// </summary>
    private LegendImpl m_legend;
    /// <summary>
    /// 
    /// </summary>
    private ChartAreaImpl m_chartArea;
    /// <summary>
    /// 
    /// </summary>
    private ChartTitleImpl m_title;
    /// <summary>
    /// 
    /// </summary>
    private WallsImpl m_walls;
    /// <summary>
    /// 
    /// </summary>
    private WallsImpl m_floor;
    /// <summary>
    /// 
    /// </summary>
    private PlotAreaImpl m_plotArea;
    /// <summary>
    /// 
    /// </summary>
    private ChartPageSetupImpl m_pageSetup;
    /// <summary>
    /// 
    /// </summary>
    private double m_dXPos;
    /// <summary>
    /// 
    /// </summary>
    private double m_dYPos;
    /// <summary>
    /// 
    /// </summary>
    private double m_dWidth;
    /// <summary>
    /// 
    /// </summary>
    private double m_dHeight;
    /// <summary>
    /// 
    /// </summary>
    private uint m_uiHorzFontGrowth;
    /// <summary>
    /// 
    /// </summary>
    private uint m_uiVertFontGrowth;
    #endregion

    #region Class constructors
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
		public ChartImpl( IApplication application, object parent )
      : base( application, parent )
		{
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    public ChartImpl( IApplication application, object parent, BiffRecordRaw[] data,
      ref int iPos )
      : this( application, parent )
    {
      Parse( data, ref iPos );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="application"></param>
    /// <param name="parent"></param>
    /// <param name="reader"></param>
    public ChartImpl( IApplication application, object parent, BiffReader reader )
      : this( application, parent )
    {
      Parse( reader );
    }
    #endregion

    #region IChart Members
    /// <summary>
    /// 
    /// </summary>
    public IAxis CategoryAxis
    {
      get
      {
        return m_categoryAxis;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IAxis ValueAxis
    {
      get
      {
        return m_valueAxis;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IAxis SerieAxis
    {
      get
      {
        return m_serieAxis;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IAxis SecondaryCategoryAxis
    {
      get
      {
        return m_secondaryCategoryAxis;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IAxis SecondaryValueAxis
    {
      get
      {
        return m_secondaryValueAxis;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public ExcelChartType ChartType
    {
      get
      {
        return m_chartType;
      }
      set
      {
        // TODO:  Add ChartImpl.ChartType setter implementation
        m_chartType = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IRange DataRange
    {
      get
      {
        return m_dataRange;
      }
      set
      {
        // TODO:  Add ChartImpl.DataRange setter implementation
        m_dataRange = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsSeriesInRows
    {
      get
      {
        return m_bSeriesInRows;
      }
      set
      {
        // TODO: implementation
        if( m_bSeriesInRows != value )
        {
          m_bSeriesInRows = value;
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IChartSerie[] Series
    {
      get
      {
        return (IChartSerie[]) m_series.ToArray( typeof( IChartSerie ) );
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IChartLegend Legend
    {
      get
      {
        return m_legend;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IChartArea ChartArea
    {
      get
      {
        return m_chartArea;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IChartTitle ChartTitle
    {
      get
      {
        return m_title;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IChartWalls Walls
    {
      get
      {
        return m_walls;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IChartWalls Floor
    {
      get
      {
        return m_floor;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IChartPlotArea PlotArea
    {
      get
      {
        return m_plotArea;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public IChartPageSetup PageSetup
    {
      get
      {
        return m_pageSetup;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public double XPos
    {
      get
      {
        return m_dXPos;
      }
      set
      {
        m_dXPos = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public double YPos
    {
      get
      {
        return m_dYPos;
      }
      set
      {
        m_dYPos = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public double Width
    {
      get
      {
        return m_dWidth;
      }
      set
      {
        m_dWidth = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public double Height
    {
      get
      {
        return m_dHeight;
      }
      set
      {
        m_dHeight = value;
      }
    }
    #endregion

    #region Parse methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    private void Parse( BiffReader reader )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    private void Parse( BiffRecordRaw[] data, ref int iPos )
    {
      if( data[ iPos ].TypeCode != TBIFFRecord.BOF )
        throw new ArgumentOutOfRangeException( "BOF Record was expected" );

      iPos++;

      while( true )
      {
        switch( data[ iPos ].TypeCode )
        {
          case TBIFFRecord.EOF:
            iPos++;
            return;

          case TBIFFRecord.Header:
            m_pageSetup = new ChartPageSetupImpl( Application, this, data, ref iPos );
            break;

          case TBIFFRecord.ChartFbi:
            ParseFonts( data, ref iPos );
            break;

          case TBIFFRecord.Chart:
            ParseChart( data, ref iPos );
            break;

          case TBIFFRecord.Dimensions:
            ParseDimensions( (DimensionsRecord) data[ iPos++ ] );
            break;
            
          case TBIFFRecord.ChartSiIndex:
            ParseSiIndex( data, ref iPos );
            break;

          case TBIFFRecord.WindowTwo:
            ParseWindowTwo( (WindowTwoRecord) data[ iPos++ ] );
            break;

          case TBIFFRecord.WindowZoom:
            ParseWindowZoom( (WindowZoomRecord) data[ iPos++ ] );
            break;

          default:
            iPos++;
            break;
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    private void ParseFonts( BiffRecordRaw[] data, ref int iPos )
    {
      if( data[ iPos ].TypeCode != TBIFFRecord.ChartFbi )
        throw new ArgumentOutOfRangeException( "ChartFbi record was expected" );

      // TODO: implement - Now skipping those records
      while( data[ iPos ].TypeCode == TBIFFRecord.ChartFbi ) iPos++;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    private void ParseChart( BiffRecordRaw[] data, ref int iPos )
    {
      if( data[ iPos ].TypeCode != TBIFFRecord.Chart )
        throw new ArgumentOutOfRangeException( "Chart record was expected" );
      
      FillDataFromChartRecord( (ChartRecord) data[ iPos ] );
      iPos++;

      if( data[ iPos ].TypeCode != TBIFFRecord.Begin )
        throw new ArgumentNullException( "Begin record was expected" );

      iPos++;

      while( data[ iPos ].TypeCode != TBIFFRecord.End )
      {
        switch( data[ iPos ].TypeCode )
        {
          case TBIFFRecord.ChartPlotGrowth:
            ParsePlotGrowth( (ChartPlotGrowthRecord) data[ iPos++ ] );
            break;

          case TBIFFRecord.ChartSeries:
            ParseSeries( data, ref iPos );
            break;
            
          case TBIFFRecord.ChartShtprops:
            ParseSheetProperties( data, ref iPos );
            break;

          case TBIFFRecord.ChartDefaultText:
            ParseDefaultText( data, ref iPos );
            break;

          case TBIFFRecord.ChartText:
            ParseText( data, ref iPos );
            break;

          case TBIFFRecord.ChartAxesUsed:
            ParseAxesUsed( data, ref iPos );
            break;

          case TBIFFRecord.ChartAxisParent:
            ParseAxisParent( data, ref iPos );
            break;

          default:
            iPos++;
            break;
        }
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="chart"></param>
    private void FillDataFromChartRecord( ChartRecord chart )
    {
      XPos = FixedPointToDouble( chart.X );
      YPos = FixedPointToDouble( chart.Y );
      Width = FixedPointToDouble( chart.XSize );
      Height = FixedPointToDouble( chart.YSize );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="plotGrowth"></param>
    private void ParsePlotGrowth( ChartPlotGrowthRecord plotGrowth )
    {
      m_uiHorzFontGrowth = plotGrowth.HorzGrowth;
      m_uiVertFontGrowth = plotGrowth.VertGrowth;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dimensions"></param>
    private void ParseDimensions( DimensionsRecord dimensions )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="windowTwo"></param>
    private void ParseWindowTwo( WindowTwoRecord windowTwo )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="windowZoom"></param>
    private void ParseWindowZoom( WindowZoomRecord windowZoom )
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    private void ParseSiIndex( BiffRecordRaw[] data, ref int iPos )
    {
      if( data[ iPos ].TypeCode != TBIFFRecord.ChartSiIndex )
        throw new ArgumentOutOfRangeException( "ChartSiIndex record was expected" );

      int count = 0;

      // TODO: we are skipping this records for the moment
      while( count < 3 )
      {
        if( data[ iPos ].TypeCode == TBIFFRecord.ChartSiIndex ) count++;

        iPos++;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    private void ParseSeries( BiffRecordRaw[] data, ref int iPos )
    {
      SerieImpl newSerie = new SerieImpl( Application, Parent, data, ref iPos );
      m_series.Add( newSerie );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    private void ParseSheetProperties( BiffRecordRaw[] data, ref int iPos )
    {
      if( data[ iPos ].TypeCode != TBIFFRecord.ChartShtprops )
        throw new ArgumentOutOfRangeException( "Chart Sheet Properties record was expected" );

      iPos++;
      // TODO: implement
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    private void ParseDefaultText( BiffRecordRaw[] data, ref int iPos )
    {
      if( data[ iPos ].TypeCode != TBIFFRecord.ChartDefaultText )
        throw new ArgumentOutOfRangeException( "ChartDefaultText record was expected" );
      
      iPos++;

      // TODO : imlement
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    private void ParseText( BiffRecordRaw[] data, ref int iPos )
    {
      if( data[ iPos ].TypeCode != TBIFFRecord.ChartText )
        throw new ArgumentOutOfRangeException( "ChartText record was expected" );
      
      iPos++;

      if( data[ iPos ].TypeCode != TBIFFRecord.Begin )
        throw new ArgumentOutOfRangeException( "Begin record was expected" );
      
      iPos++;

      // TODO : implement
      while( data[ iPos ].TypeCode != TBIFFRecord.End )
      {
        iPos++;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    private void ParseAxesUsed( BiffRecordRaw[] data, ref int iPos )
    {
      if( data[ iPos ].TypeCode != TBIFFRecord.ChartAxesUsed )
        throw new ArgumentOutOfRangeException( "ChartAxesUsed record was expected" );
      
      iPos++;

      // TODO: implement
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    private void ParseAxisParent( BiffRecordRaw[] data, ref int iPos )
    {
      if( data[ iPos ].TypeCode != TBIFFRecord.ChartAxisParent )
        throw new ArgumentOutOfRangeException( "ChartAxesParent record was expected" );
      
      iPos++;

      // TODO: implement
    }
    #endregion

    #region Serialize methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="records"></param>
    public void Serialize( OffsetArrayList records )
    {
    }
    #endregion

    #region Class helper methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int DoubleToFixedPoint( double value )
    {
      ushort number = (ushort) value;
      double fraction = (value - number) * 100000;
      
      if( fraction > ushort.MaxValue ) fraction /= 10;

      byte[] bytes1 = BitConverter.GetBytes( number );
      byte[] bytes2 = BitConverter.GetBytes( (ushort) fraction );
      byte[] buffer = new byte[4];
      
      bytes1.CopyTo( buffer, 0 );
      bytes2.CopyTo( buffer, 2 );

      return BitConverter.ToInt32( buffer, 0 );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static double FixedPointToDouble( int value )
    {
      byte[] bytes = BitConverter.GetBytes( value );
      
      // fraction part of the number
      int fraction = BitConverter.ToUInt16( bytes, 0 );

      // integer part of the double value
      int number = BitConverter.ToUInt16( bytes, 2 );

      // number of digits in the fraction part
      int count = (int) Math.Log10( fraction ) + 1;

      double result = Math.Abs( number ) + fraction / Math.Pow( 10, count );

      result *= Math.Sign( number );
      return result;
    }
    #endregion
  }
}
