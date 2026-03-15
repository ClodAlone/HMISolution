#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Diagnostics;

using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using System.Collections.Generic;
#if ( WINRT )
using Windows.UI;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation.Charts
{
	/// <summary>
	/// Represent the ChartChartFormatRecord.
	/// </summary>
	public class ChartFormatImpl
    : CommonObject
    , IChartFormat
    , ICloneParent
	{
    #region Class constants
    /// <summary>
    /// Id for bar stacked chart type.
    /// </summary>
    public const int DEF_BAR_STACKED = -65436;
    /// <summary>
    /// Represents default series number.
    /// </summary>
    private const int DEF_SERIES_NUMBER = 65533;
    #endregion

    #region Class members
    /// <summary>
    /// Represents the ChartChartFormatRecord
    /// </summary>
    private ChartChartFormatRecord m_chartChartFormat;
    /// <summary>
    /// Represents the chart format of some series.
    /// </summary>
    private BiffRecordRaw m_serieFormat;
    /// <summary>
    /// Represents chart 3d record.
    /// </summary>
    private Chart3DRecord m_chart3D;
    /// <summary>
    /// Represents format link record.
    /// </summary>
    private ChartFormatLinkRecord m_formatLink;
    /// <summary>
    /// Represents data labels record.
    /// </summary>
    private ChartDataLabelsRecord m_dataLabels;
    /// <summary>
    /// Represents chart chart line record.
    /// </summary>
    private ChartChartLineRecord m_chartChartLine;
    /// <summary>
    /// Represents data format record and subrecords.
    /// </summary>
    private ChartSerieDataFormatImpl m_dataFormat = null;
    /// <summary>
    /// Represents first drop bar record and subrecords.
    /// </summary>
    private ChartDropBarImpl m_firstDropBar = null;
    /// <summary>
    /// Represents second drop bar record and subrecords.
    /// </summary>
    private ChartDropBarImpl m_secondDropBar = null;
    /// <summary>
    /// Represents series list record.
    /// </summary>
    private ChartSeriesListRecord m_seriesList;
    /// <summary>
    /// Parent chart object.
    /// </summary>
    private ChartImpl m_chart;
    /// <summary>
    /// Parent axis object.
    /// </summary>
    private ChartParentAxisImpl m_parentAxis;
    /// <summary>
    /// Represents chart Series line properties object.
    /// </summary>
    private ChartBorderImpl m_serieLine;
    #endregion

    #region IChartFormat properties
    /// <summary>
    /// Vary color for each data point.
    /// </summary>
    public bool IsVeryColor
    {
      get
      {
        return IsVaryColor;
      }
      set
      {
        IsVaryColor = value;
      }
    }
    /// <summary>
    /// Vary color for each data point.
    /// </summary>
    public bool IsVaryColor
    {
      get
      {
        return ChartChartFormatRecord.IsVaryColor;
      }
      set
      {
        ChartChartFormatRecord.IsVaryColor = value;
      }
    }

    /// <summary>
    /// Returns data format. Read-only.
    /// </summary>
    public IChartSerieDataFormat SerieDataFormat
    {
      get
      {
        return DataFormat;
      }
    }
    /// <summary>
    /// Space between bars ( -100 : 100 ).
    /// </summary>
    public int Overlap
    {
      get
      {
        if( m_chart.IsChart3D )
          throw new NotSupportedException( "This property is not supported in 3d chart types" );

        return BarRecord.Overlap;
      }
      set
      {
        if( !m_chart.ParentWorkbook.Loading && m_chart.IsChart3D )
          throw new NotSupportedException( "This property is not supported in 3d chart types" );

        if( !m_chart.ParentWorkbook.Loading && value < -100 || value > 100 )
          throw new ArgumentOutOfRangeException( "value" );

        BarRecord.Overlap = value;
      }
    }
    /// <summary>
    /// Space between categories (percent of bar width), default = 50.
    /// </summary>
    public int GapWidth
    {
      get
      {
        return ( m_serieFormat.TypeCode == TBIFFRecord.ChartBar )
          ? BarRecord.CategoriesSpace
          : BoppopRecord.Gap;
      }
      set
      {
        if( m_serieFormat.TypeCode == TBIFFRecord.ChartBar )
        {
          if( value < 0 || value > 500 )
            throw new ArgumentOutOfRangeException( "GapWidth" );

          BarRecord.CategoriesSpace = ( ushort )value;
        }
        else
        {
          if( value < 5 || value > 200 )
            throw new ArgumentOutOfRangeException( "GapWidth" );

          BoppopRecord.Gap = ( ushort )value;
        }
      }
    }
    /// <summary>
    /// True for horizontal bars (bar chart).
    /// False for vertical bars (column chart).
    /// </summary>
    public bool   IsHorizontalBar
    {
      get
      {
        return BarRecord.IsHorizontalBar;
      }
      set
      {
        BarRecord.IsHorizontalBar = value;
      }
    }
    /// <summary>
    /// Stack the displayed values.
    /// </summary>
    public bool   StackValuesBar
    {
      get
      {
        return BarRecord.StackValues;
      }
      set
      {
        BarRecord.StackValues = value;
      }
    }
    /// <summary>
    /// Each category is displayed as a percentage.
    /// </summary>
    public bool   ShowAsPercentsBar
    {
      get
      {
        return BarRecord.ShowAsPercents;
      }
      set
      {
        BarRecord.ShowAsPercents = value;
      }
    }
    /// <summary>
    /// True if this bar has a shadow; otherwise False.
    /// </summary>
    public bool   HasShadowBar
    {
      get
      {
        return BarRecord.HasShadow;
      }
      set
      {
        BarRecord.HasShadow = value;
      }
    }

    /// <summary>
    /// Stack the displayed values.
    /// </summary>
    public bool StackValuesLine
    {
      get
      {
        return LineRecord.StackValues;
      }
      set
      {
        LineRecord.StackValues = value;
      }
    }
    /// <summary>
    /// Each category is broken down as a percentage.
    /// </summary>
    public bool ShowAsPercentsLine
    {
      get
      {
        return LineRecord.ShowAsPercents;
      }
      set
      {
        LineRecord.ShowAsPercents = value;
      }
    }
    /// <summary>
    /// True if this line has a shadow.
    /// </summary>
    public bool HasShadowLine
    {
      get
      {
        return LineRecord.HasShadow;
      }
      set
      {
        LineRecord.HasShadow = value;
      }
    }
    /// <summary>
    /// Angle of the first pie slice expressed in degrees.
    /// </summary>
    public int FirstSliceAngle
    {
      get
      {
        return PieRecord.StartAngle;
      }
      set
      {
        if( value < 0 || value > 360 )
          throw new ArgumentOutOfRangeException( "StartAngle" );

        PieRecord.StartAngle = ( ushort )value;
      }
    }
    /// <summary>
    /// 0 = True pie chart
    /// Non-zero = size of center hole in a donut chart (as a percentage).
    /// </summary>
    public int DoughnutHoleSize
    {
      get
      {
        return PieRecord.DonutHoleSize;
      }
      set
      {
        if( value < 10 || value > 90 )
          throw new ArgumentOutOfRangeException( "DonutHoleSize" );

        if( !m_chart.TypeChanging )
        {
          ExcelChartType type = m_chart.ChartType;

          if( type != ExcelChartType.Doughnut && type != ExcelChartType.Doughnut_Exploded )
            throw new NotSupportedException( "This property is supported only in doughnut chart types" );
        }

        PieRecord.DonutHoleSize = ( ushort )value;
      }
    }
    /// <summary>
    /// True if this pie has a shadow.
    /// </summary>
    public bool HasShadowPie
    {
      get
      {
        return PieRecord.HasShadow;
      }
      set
      {
        PieRecord.HasShadow = value;
      }
    }
    /// <summary>
    /// True to show leader lines to data labels.
    /// </summary>
    public bool ShowLeaderLines
    {
      get
      {
          if ( PieRecord != null )
              return PieRecord.ShowLeaderLines;
          else if( BoppopRecord != null)
              return BoppopRecord.ShowLeaderLines;
          else
              return true;
      }
      set
      {
          if (PieRecord != null)
              PieRecord.ShowLeaderLines = value;
          else if (BoppopRecord != null)
              BoppopRecord.ShowLeaderLines = value;

      }
    }
    /// <summary>
    /// Percent of largest bubble compared to chart in general.( 0 - 300 )
    /// </summary>
    public int BubbleScale
    {
      get
      {
        return ScatterRecord.BubleSizeRation;
      }
      set
      {
        if( value < 0 || value > 300 )
          throw new ArgumentOutOfRangeException( "BubleSizeScale" );

        if( !m_chart.TypeChanging )
        {
          ExcelChartType type = m_chart.ChartType;

          if( type != ExcelChartType.Bubble && type != ExcelChartType.Bubble_3D )
            throw new NotSupportedException( "This property supported only in bubble chart types." );
        }

        ScatterRecord.BubleSizeRation = ( ushort )value;
      }
    }
    /// <summary>
    /// Returns or sets what the bubble size represents on a bubble chart.
    /// </summary>
    public ExcelBubbleSize SizeRepresents
    {
      get
      {
        return ScatterRecord.BubleSize;
      }
      set
      {
        if( !m_chart.TypeChanging )
        {
          ExcelChartType type = m_chart.ChartType;

          if( type != ExcelChartType.Bubble && type != ExcelChartType.Bubble_3D )
            throw new NotSupportedException( "This property is supported only in bubble chart types." );
        }

        ScatterRecord.BubleSize = value;
      }
    }
    /// <summary>
    /// True if this a bubble series.
    /// </summary>
    public bool IsBubbles
    {
      get
      {
        return ScatterRecord.IsBubbles;
      }
      set
      {
        ScatterRecord.IsBubbles = value;
      }
    }
    /// <summary>
    /// True to show negative bubbles.
    /// </summary>
    public bool ShowNegativeBubbles
    {
      get
      {
        return ScatterRecord.IsShowNegBubbles;
      }
      set
      {
        ExcelChartType type = m_chart.ChartType;

        if( type != ExcelChartType.Bubble && type != ExcelChartType.Bubble_3D )
          throw new NotSupportedException( "This property is supported only in bubble chart types." );

        ScatterRecord.IsShowNegBubbles = value;
      }
    }
    /// <summary>
    /// True if bubble series has a shadow.
    /// </summary>
    public bool HasShadowScatter
    {
      get
      {
        return ScatterRecord.HasShadow;
      }
      set
      {
        ScatterRecord.HasShadow = value;
      }
    }
    /// <summary>
    /// Series in this group are stacked.
    /// </summary>
    public bool IsStacked
    {
      get
      {
        return AreaRecord.IsStacked;
      }
      set
      {
        AreaRecord.IsStacked = value;
      }
    }
    /// <summary>
    /// Each category is broken down as a percentage.
    /// </summary>
    public bool IsCategoryBrokenDown
    {
      get
      {
        return AreaRecord.IsCategoryBrokenDown;
      }
      set
      {
        AreaRecord.IsCategoryBrokenDown = value;
      }
    }
    /// <summary>
    /// This area has a shadow.
    /// </summary>
    public bool IsAreaShadowed
    {
      get
      {
        return AreaRecord.IsAreaShadowed;
      }
      set
      {
        AreaRecord.IsAreaShadowed = value;
      }
    }
    /// <summary>
    /// True if chart contains color fill for surface.
    /// </summary>
    public bool IsFillSurface
    {
      get
      {
        return SurfaceRecord.IsFillSurface;
      }
      set
      {
        SurfaceRecord.IsFillSurface = value;
      }
    }
    /// <summary>
    /// True if this surface chart has shading.
    /// </summary>
    public bool Is3DPhongShade
    {
      get
      {
        return SurfaceRecord.Is3DPhongShade;
      }
      set
      {
        SurfaceRecord.Is3DPhongShade = value;
      }
    }

    /// <summary>
    /// True if this radar series has a shadow.
    /// </summary>
    public bool HasShadowRadar
    {
      get
      {
        return RadarRecord.HasShadow;
      }
      set
      {
        RadarRecord.HasShadow = value;
      }
    }
    /// <summary>
    /// True if a radar chart has axis labels. Applies only to radar charts.
    /// </summary>
    public bool HasRadarAxisLabels
    {
      get
      {
        return ( m_serieFormat.TypeCode == TBIFFRecord.ChartRadarArea )
          ? RadarRecord.IsRadarAxisLabel
          : RadarAreaRecord.IsRadarAxisLabel;
      }
      set
      {
        if( m_serieFormat.TypeCode == TBIFFRecord.ChartRadarArea )
        {
          RadarAreaRecord.IsRadarAxisLabel = value;
        }
        else
        {
          RadarRecord.IsRadarAxisLabel = value;
        }
      }
    }
    /// <summary>
    /// 0 = normal pie chart
    /// 1 = pie of pie chart
    /// 2 = bar of pie chart
    /// </summary>
    public ExcelPieType PieChartType
    {
      get
      {
        return BoppopRecord.PieChartType;
      }
      set
      {
        BoppopRecord.PieChartType = value;
      }
    }
    /// <summary>
    /// True to use default split value; otherwise False.
    /// </summary>
    public bool   UseDefaultSplitValue
    {
      get
      {
        return BoppopRecord.UseDefaultSplitValue;
      }
      set
      {
        BoppopRecord.UseDefaultSplitValue = value;
      }
    }
    /// <summary>
    /// Returns or sets the way the two sections of either a pie
    /// of pie chart or a bar of pie chart are split.
    /// </summary>
    public ExcelSplitType SplitType
    {
      get
      {
        return BoppopRecord.ChartSplitType;
      }
      set
      {
        BoppopRecord.ChartSplitType = value;
      }
    }
    /// <summary>
    /// Returns or sets the threshold value separating the two sections of either a pie of pie chart or a bar of pie chart.
    /// </summary>
    public int SplitValue
    {
      get
      {
        return BoppopRecord.SplitPosition;
      }
      set
      {
        if( SplitType == ExcelSplitType.Percent )
        {
          BoppopRecord.SplitPercent = ( ushort )value;
        }
        else
        {
          BoppopRecord.SplitPosition = ( ushort )value;
        }

        UseDefaultSplitValue = false;
      }
    }
    /// <summary>
    /// For split = 2, what percentage should go to the other pie / bar.
    /// </summary>
    public int SplitPercent
    {
      get
      {
        return BoppopRecord.SplitPercent;
      }
      set
      {
        BoppopRecord.SplitPercent = ( ushort )value;
      }
    }
    /// <summary>
    /// Returns or sets the size of the secondary section of either a pie of pie chart or 
    /// a bar of pie chart, as a percentage of the size of the primary pie.
    /// </summary>
    public int PieSecondSize
    {
      get
      {
        return BoppopRecord.Pie2Size;
      }
      set
      {
        if( value < 5 || value > 200 )
          throw new ArgumentOutOfRangeException( "PieSecondSize" );

        BoppopRecord.Pie2Size = ( ushort )value;
      }
    }
    /// <summary>
    /// Space between the first pie and the second.
    /// </summary>
    public int Gap
    {
      get
      {
        return BoppopRecord.Gap;
      }
      set
      {
        BoppopRecord.Gap = ( ushort )value;
      }
    }
    /// <summary>
    /// For split = 1, what values should go to the other pie / bar.
    /// </summary>
    public int    NumSplitValue
    {
      get
      {
        return BoppopRecord.NumSplitValue;
      }
      set
      {
        BoppopRecord.NumSplitValue = value;
      }
    }
    /// <summary>
    /// 1 = the second pie / bar has a shadow.
    /// </summary>
    public bool   HasShadowBoppop
    {
      get
      {
        return BoppopRecord.HasShadow;
      }
      set
      {
        BoppopRecord.HasShadow = value;
      }
    }

    /// <summary>
    /// If true Series has name.
    /// </summary>
    public bool IsSeriesName
    {
      get
      {
        return DataLabelsRecord.IsSeriesName;
      }
      set
      {
        DataLabelsRecord.IsSeriesName = value;
      }
    }
    
    /// <summary>
    /// If true category has name.
    /// </summary>
    public bool IsCategoryName
    {
      get
      {
        return DataLabelsRecord.IsCategoryName;
      }
      set
      {
        DataLabelsRecord.IsCategoryName = value;
      }
    }

    /// <summary>
    /// If true has value.
    /// </summary>
    public bool IsValue
    {
      get
      {
        return DataLabelsRecord.IsValue;
      }
      set
      {
        DataLabelsRecord.IsValue = value;
      }
    }

    /// <summary>
    /// If true has percentage.
    /// </summary>
    public bool IsPercentage
    {
      get
      {
        return DataLabelsRecord.IsPercentage;
      }
      set
      {
        DataLabelsRecord.IsPercentage = value;
      }
    }

    /// <summary>
    /// If true bubble has size.
    /// </summary>
    public bool IsBubbleSize
    {
      get
      {
        return DataLabelsRecord.IsBubbleSize;
      }
      set
      {
        DataLabelsRecord.IsBubbleSize = value;
      }
    }

    /// <summary>
    /// Returns delimiter length.
    /// </summary>
    public int DelimiterLength
    {
      get
      {
        return DataLabelsRecord.DelimiterLength;
      }
    }

    /// <summary>
    /// Represents delimiter.
    /// </summary>
    public string Delimiter
    {
      get
      {
        return DataLabelsRecord.Delimiter;
      }
      set
      {
        DataLabelsRecord.Delimiter = value;
      }
    }
    /// <summary>
    /// Drop lines / hi-lo lines:
    /// 0 = drop lines
    /// 1 = hi-lo lines
    /// 2 = series lines (the lines that connect the columns in a stacked column chart)
    /// </summary>
    public ExcelDropLineStyle LineStyle
    {
      get
      {
        if( m_chartChartLine == null )
          m_chartChartLine = ( ChartChartLineRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.ChartChartLine );

        return m_chartChartLine.LineStyle;
      }
      set
      {
        if( m_chartChartLine == null )
          m_chartChartLine = ( ChartChartLineRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.ChartChartLine );

        m_chartChartLine.LineStyle = value;
      }
    }
    /// <summary>
    /// Returns object that represents first drop bar (up bar).
    /// </summary>
    public IChartDropBar FirstDropBar
    {
      get
      {
        if( m_firstDropBar == null )
          m_firstDropBar = new ChartDropBarImpl( Application, this );

        return m_firstDropBar;
      }
    }
    /// <summary>
    /// Returns object that represents second drop bar (down bar).
    /// </summary>
    public IChartDropBar SecondDropBar
    {
      get
      {
        if( m_secondDropBar == null )
          m_secondDropBar = new ChartDropBarImpl( Application, this );

        return m_secondDropBar;
      }
    }
    /// <summary>
    /// Represents series line properties. ( For pie of pie or pie of bar chart types only. ) Read-only.
    /// </summary>
    public IChartBorder PieSeriesLine
    {
      get
      {
        if( m_serieFormat.TypeCode != TBIFFRecord.ChartBoppop )
          throw new ArgumentNullException( "This property is not supported in this chart type" );

        if( m_serieLine == null )
          m_serieLine = new ChartBorderImpl( Application, this );

        return m_serieLine;
      }
    }
    #endregion

    #region Class 3DDataFormat properties
    /// <summary>
    /// Returns or sets the rotation of the 3-D chart view
    /// (the rotation of the plot area around the z-axis, in degrees).(0 to 360 degrees).
    /// </summary>
    public int Rotation
    {
      get
      {
        return Chart3DRecord.RotationAngle;
      }
      set
      {
        if( value < 0 || value > 360 )
          throw new ArgumentOutOfRangeException( "Rotation" );

        Chart3DRecord.RotationAngle = ( ushort )value;
      }
    }
    /// <summary>
    /// Indicates whether rotation has default value.
    /// </summary>
    public bool IsDefaultRotation
    {
      get
      {
        return Chart3DRecord.IsDefaultRotation;
      }
    }
    /// <summary>
    /// Returns or sets the elevation of the 3-D chart view, in degrees (�90 to +90 degrees).
    /// </summary>
    public int  Elevation
    {
      get
      {
        return Chart3DRecord.ElevationAngle;
      }
      set
      {
        if( value < -90 || value > 90 )
          throw new ArgumentOutOfRangeException( "Elevation" );

        Chart3DRecord.ElevationAngle = ( short )value;
      }
    }
    /// <summary>
    /// Indicates whether elevation has default value.
    /// </summary>
    public bool IsDefaultElevation
    {
      get
      {
        return Chart3DRecord.IsDefaultElevation;
      }
    }
    /// <summary>
    /// Returns or sets the perspective for the 3-D chart view.( 0 - 100 )
    /// </summary>
    public int Perspective
    {
      get
      {
        return Chart3DRecord.DistanceFromEye;
      }
      set
      {
        if( value < 0 || value > 100 )
          throw new ArgumentOutOfRangeException( "Elevation" );

        Chart3DRecord.DistanceFromEye = ( ushort )value;        
      }
    }
    /// <summary>
    /// Returns or sets the height of a 3-D chart as a percentage of the chart width
    /// (between 5 and 500 percent).
    /// </summary>
    public int HeightPercent
    {
      get
      {
        return Chart3DRecord.Height;
      }
      set
      {
        if( value < 5 || value > 500 )
          throw new ArgumentOutOfRangeException( "Elevation" );

        Chart3DRecord.Height = ( ushort )value;
      }
    }
    /// <summary>
    /// Returns or sets the depth of a 3-D chart as a percentage of the chart width
    /// (between 20 and 2000 percent).
    /// </summary>
    public int DepthPercent
    {
      get
      {
        return Chart3DRecord.Depth;
      }
      set
      {
        if( value < 20 || value > 2000 )
          throw new ArgumentOutOfRangeException( "DepthPercent" );

        Chart3DRecord.Depth = ( ushort )value;
      }
    }
    /// <summary>
    /// Returns or sets the distance between the data series in a 3-D chart, as a percentage of the marker width.( 0 - 500 )
    /// </summary>
    public int GapDepth
    {
      get
      {
        return Chart3DRecord.SeriesSpace;
      }
      set
      {
        if( value < 0 || value > 500 )
          throw new ArgumentOutOfRangeException( "GapDepth" );

        Chart3DRecord.SeriesSpace = ( ushort )value;
      }
    }
    /// <summary>
    /// True if the chart axes are at right angles, independent of chart rotation or elevation.
    /// </summary>
    public bool   RightAngleAxes
    {
      get
      {
        return !Chart3DRecord.IsPerspective;
      }
      set
      {
        Chart3DRecord.IsPerspective = !value;
      }
    }
    /// <summary>
    /// 3D columns are clustered or stacked.
    /// </summary>
    public bool   IsClustered
    {
      get
      {
        return Chart3DRecord.IsClustered;
      }
      set
      {
        Chart3DRecord.IsClustered = value;
      }
    }
    /// <summary>
    /// True if Microsoft Excel scales a 3-D chart so that it's closer in size to the equivalent 2-D chart..
    /// </summary>
    public bool   AutoScaling
    {
      get
      {
        return Chart3DRecord.IsAutoScaled;
      }
      set
      {
        Chart3DRecord.IsAutoScaled = value;
      }
    }
    /// <summary>
    /// True if gridlines are drawn two-dimensionally on a 3-D chart.
    /// </summary>
    public bool   WallsAndGridlines2D
    {
      get
      {
        return Chart3DRecord.Is2DWalls;
      }
      set
      {
        Chart3DRecord.Is2DWalls = value;
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Try to convert m_serieFormat to chartBarRecord.
    /// If can  - returns chartBarRecord; otherwise rise exception. Read-only.
    /// </summary>
    private ChartBarRecord BarRecord
    {
      get
      {
        if( m_serieFormat.TypeCode != TBIFFRecord.ChartBar )
          throw new NotSupportedException( "This property is not suported in current chart type." );

        return m_serieFormat as ChartBarRecord;
      }
    }
    /// <summary>
    /// Try to convert m_serieFormat to chartLineRecord.
    /// If can  - returns chartLineRecord; otherwise rise exception. Read-only.
    /// </summary>
    private ChartLineRecord LineRecord
    {
      get
      {
        if( m_serieFormat.TypeCode != TBIFFRecord.ChartLine )
          throw new NotSupportedException( "This property is not suported in current chart type." );

        return m_serieFormat as ChartLineRecord;
      }
    }
    /// <summary>
    /// Try to convert m_serieFormat to chartPieRecord.
    /// If can  - returns chartPieRecord; otherwise rise exception. Read-only.
    /// </summary>
    private ChartPieRecord PieRecord
    {
      get
      {
          if (m_serieFormat.TypeCode == TBIFFRecord.ChartBoppop)
              return null;
          else if( m_serieFormat.TypeCode != TBIFFRecord.ChartPie && !(m_chart.Workbook as WorkbookImpl).Loading)
          throw new NotSupportedException( "This property is not suported in current chart type." );

        return m_serieFormat as ChartPieRecord;
      }
    }
    /// <summary>
    /// Try to convert m_serieFormat to chartScatterRecord.
    /// If can  - returns chartScatterRecord; otherwise rise exception. Read-only.
    /// </summary>
    private ChartScatterRecord ScatterRecord
    {
      get
      {
        if( m_serieFormat.TypeCode != TBIFFRecord.ChartScatter )
          throw new NotSupportedException( "This property is not suported in current chart type." );

        return m_serieFormat as ChartScatterRecord;
      }
    }
    /// <summary>
    /// Try to convert m_serieFormat to chartAreaRecord.
    /// If can  - returns chartAreaRecord; otherwise rise exception. Read-only.
    /// </summary>
    private ChartAreaRecord AreaRecord
    {
      get
      {
        if( m_serieFormat.TypeCode != TBIFFRecord.ChartArea )
          throw new NotSupportedException( "This property is not suported in current chart type." );

        return m_serieFormat as ChartAreaRecord;
      }
    }
    /// <summary>
    /// Try to convert m_serieFormat to chartSurfaceRecord.
    /// If can  - returns chartSurfaceRecord; otherwise rise exception. Read-only.
    /// </summary>
    private ChartSurfaceRecord SurfaceRecord
    {
      get
      {
        if( m_serieFormat.TypeCode != TBIFFRecord.ChartSurface )
          throw new NotSupportedException( "This property is not suported in current chart type." );

        return m_serieFormat as ChartSurfaceRecord;
      }
    }
    /// <summary>
    /// Try to convert m_serieFormat to chartRadarRecord.
    /// If can  - returns chartRadarRecord; otherwise rise exception. Read-only.
    /// </summary>
    private ChartRadarRecord RadarRecord
    {
      get
      {
        if( m_serieFormat.TypeCode != TBIFFRecord.ChartRadar )
          throw new NotSupportedException( "This property is not suported in current chart type." );

        return m_serieFormat as ChartRadarRecord;
      }
    }
    /// <summary>
    /// Try to convert m_serieFormat to chartRadarAreaRecord.
    /// If can  - returns chartRadarAreaRecord; otherwise rise exception. Read-only.
    /// </summary>
    private ChartRadarAreaRecord RadarAreaRecord
    {
      get
      {
        if( m_serieFormat.TypeCode != TBIFFRecord.ChartRadarArea )
          throw new NotSupportedException( "This property is not suported in current chart type." );

        return m_serieFormat as ChartRadarAreaRecord;
      }
    }
    /// <summary>
    /// Try to convert m_serieFormat to chartBoppopRecord.
    /// If can  - returns chartBoppopRecord; otherwise rise exception. Read-only.
    /// </summary>
    private ChartBoppopRecord BoppopRecord
    {
      get
      {
        if( m_serieFormat.TypeCode != TBIFFRecord.ChartBoppop )
          throw new NotSupportedException( "This property is not suported in current chart type." );

        return m_serieFormat as ChartBoppopRecord;
      }
    }
    /// <summary>
    /// Returns data labels record. Read-only.
    /// </summary>
    private ChartDataLabelsRecord DataLabelsRecord
    {
      get
      {
        if( m_dataLabels == null )
          m_dataLabels = ( ChartDataLabelsRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.ChartDataLabels );

        return m_dataLabels;
      }
    }
    /// <summary>
    /// Returns chart data format. Read-only.
    /// </summary>
    private ChartSerieDataFormatImpl DataFormat
    {
      get
      {
        if( m_dataFormat == null )
          m_dataFormat = new ChartSerieDataFormatImpl( Application, this );

        return m_dataFormat;
      }
    }
    /// <summary>
    /// Returns chart chart format. Read-only.
    /// </summary>
    private ChartChartFormatRecord ChartChartFormatRecord
    {
      get
      {
        if( m_chartChartFormat == null )
          m_chartChartFormat = ( ChartChartFormatRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.ChartChartFormat );

        return m_chartChartFormat;
      }
    }
    /// <summary>
    /// Returns Chart3d record. Read-only.
    /// </summary>
    private Chart3DRecord Chart3DRecord
    {
      get
      {
        if( m_chart3D == null )
          m_chart3D = ( Chart3DRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.Chart3D );

        return m_chart3D;
      }
    }
    /// <summary>
    /// If true then belong to primary axis; otherwise to secondary axis.
    /// </summary>
    public bool IsPrimaryAxis
    {
      get
      {
        return m_parentAxis.IsPrimary;
      }
    }
    /// <summary>
    /// If true - format contains chart chart line record. Read-only.
    /// </summary>
    public bool IsChartChartLine
    {
      get
      {
        return m_chartChartLine != null;
      }
    }
    /// <summary>
    /// If true - format contains series line. Read-only.
    /// </summary>
    public bool IsChartLineFormat
    {
      get
      {
        return m_serieLine != null;
      }
    }
    /// <summary>
    /// If true - format contains drop bar record. Read-only.
    /// </summary>
    public bool IsDropBar
    {
      get
      {
        return m_firstDropBar != null;
      }
    }
    /// <summary>
    /// Returns record that represents Series format. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public BiffRecordRaw SerieFormat
    {
      get
      {
        if( m_serieFormat == null )
          throw new ArgumentNullException( "m_serieFormat" );

        return m_serieFormat;
      }
    }
    /// <summary>
    /// Drawing order (0 = bottom of the z-order).
    /// </summary>
    public int DrawingZOrder
    {
      get
      {
        return ChartChartFormatRecord.DrawingZOrder;
      }
      set
      {
        int iOrder = ChartChartFormatRecord.DrawingZOrder;

        if( iOrder != value )
        {
          ChartChartFormatRecord.DrawingZOrder = ( ushort )value;
        }
      }
    }
    /// <summary>
    /// Represents type code of major format record type. Read-only.
    /// </summary>
    public TBIFFRecord FormatRecordType
    {
      get
      {
        return m_serieFormat.TypeCode;
      }
    }
    /// <summary>
    /// If true - Series 3D; otherwise Series 2D. Read-only.
    /// </summary>
    public bool Is3D
    {
      get
      {
        return m_chart3D != null;
      }
    }
    /// <summary>
    /// Returns dataformat or null; Read-only.
    /// </summary>
    public ChartSerieDataFormatImpl DataFormatOrNull
    {
      get
      {
        return m_dataFormat;
      }
    }
    /// <summary>
    /// If true - format has marker; otherwise false. Read-only.
    /// </summary>
    public bool IsMarker
    {
      get
      {
        return m_dataFormat == null || m_dataFormat.IsMarker;
      }
    }
    /// <summary>
    /// If true - format has line; otherwise false. Read-only.
    /// </summary>
    public bool IsLine
    {
      get
      {
        return m_dataFormat == null || m_dataFormat.IsLine;
      }
    }
    /// <summary>
    /// If true - format has smoothed line; otherwise false. Read-only.
    /// </summary>
    public bool IsSmoothed
    {
      get
      {
        return m_dataFormat != null && m_dataFormat.IsSmoothed;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Create new instance.
    /// </summary>
    /// <param name="application">Current application.</param>
    /// <param name="parent">Parent object.</param>
		public ChartFormatImpl( IApplication application, object parent )
      : base( application, parent )
		{
      SetParents();

      m_chartChartFormat = ( ChartChartFormatRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartChartFormat );

      m_serieFormat = BiffRecordFactory.GetRecord( TBIFFRecord.ChartBar );
		}
    /// <summary>
    /// Finds parent objects.
    /// </summary>
    public void SetParents()
    {
      m_parentAxis = ( ChartParentAxisImpl )FindParent( typeof( ChartParentAxisImpl ) );

      if( m_parentAxis == null )
        throw new ArgumentNullException( "Cannot find parent axis object." );

      m_chart = m_parentAxis.m_parentChart;
    }
    #endregion

    #region Parse methods
    /// <summary>
    /// Parsing chart chart format.
    /// </summary>
    /// <param name="data">Records offset.</param>
    /// <param name="iPos">Position in offset.</param>
    [ CLSCompliant( false ) ]
    public void Parse( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartChartFormat );

      m_chartChartFormat = ( ChartChartFormatRecord )data[ iPos ];
      iPos++;

      record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.Begin );
      iPos++;

      int count = 1;
      int iDropCount = 0;

      while( count != 0 )
      {
        record = ( BiffRecordRaw )data[ iPos ];

        switch( record.TypeCode )
        {
          case TBIFFRecord.Begin:
            iPos = BiffRecordRaw.SkipBeginEndBlock( data, iPos ) - 1; // there would be ++ at the end of while loop
            break;

          case TBIFFRecord.End:
            count--;
            break;

          case TBIFFRecord.ChartBar:
          case TBIFFRecord.ChartLine:
          case TBIFFRecord.ChartPie:
          case TBIFFRecord.ChartArea:
          case TBIFFRecord.ChartScatter:
          case TBIFFRecord.ChartSurface:
          case TBIFFRecord.ChartRadar:
          case TBIFFRecord.ChartRadarArea:
          case TBIFFRecord.ChartBoppop:
            m_serieFormat = record;
            break;

          case TBIFFRecord.Chart3D:
            m_chart3D = ( Chart3DRecord )record;
            break;

          case TBIFFRecord.ChartFormatLink:
            m_formatLink = ( ChartFormatLinkRecord )record;
            break;

          case ( TBIFFRecord )2129:
          case ( TBIFFRecord )2128:
          case ( TBIFFRecord )2130:
          case ( TBIFFRecord )2131:
          case ( TBIFFRecord )2132:
          case ( TBIFFRecord )2133:
          case ( TBIFFRecord )2154:
            break;

          case TBIFFRecord.ChartLegend:
            m_chart.ParseLegend( data, ref iPos );
            iPos --;
            break;

          case TBIFFRecord.ChartDataFormat:
            m_dataFormat = new ChartSerieDataFormatImpl( Application, this );
            iPos = m_dataFormat.Parse( data, iPos ) - 1;
            break;

          case TBIFFRecord.ChartDataLabels:
            m_dataLabels = ( ChartDataLabelsRecord )record;
            break;

          case TBIFFRecord.ChartChartLine:
            m_chartChartLine = ( ChartChartLineRecord )record;
            break;

          case TBIFFRecord.ChartLineFormat:
            m_serieLine = new ChartBorderImpl( Application, this, ( ChartLineFormatRecord )record );
            break;

          case TBIFFRecord.ChartDropBar:
            if( iDropCount > 1 )
              throw new ParseException( "Find more then two ChartBarRecords." );

            ChartDropBarImpl dropBar = new ChartDropBarImpl( Application, this );
            dropBar.Parse( data, ref iPos );

            if( iDropCount == 0 )
            {
              m_firstDropBar = dropBar;
            }
            else
            {
              m_secondDropBar = dropBar;
            }

            iDropCount++;
            break;

          case TBIFFRecord.ChartSeriesList:
            m_seriesList = ( ChartSeriesListRecord )record;
            break;

          default:
            //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, record.TypeCode, "Unaccepted record" );
            break;
        }

        iPos++;
      }
    }

    #endregion

    #region Serialize methods
    /// <summary>
    /// Serialize current object.
    /// </summary>
    /// <param name="records">Records offset.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      records.Add( ( BiffRecordRaw )m_chartChartFormat.Clone() );
      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );

      records.Add( ( BiffRecordRaw )m_serieFormat.Clone() );

      if( m_formatLink != null )
        records.Add( ( BiffRecordRaw )m_formatLink.Clone() );

      if( m_seriesList != null )
        records.Add( ( BiffRecordRaw )m_seriesList.Clone() );

      if( m_chart3D != null )
      {
        records.Add( ( Chart3DRecord )m_chart3D.Clone() );
      }

      if( DrawingZOrder == 0 )
        m_chart.SerializeLegend( records );

      if( m_firstDropBar != null )
        m_firstDropBar.Serialize( records );

      if( m_secondDropBar != null )
        m_secondDropBar.Serialize( records );

      if( m_chartChartLine != null ) 
        records.Add( ( BiffRecordRaw )m_chartChartLine.Clone() );

      if( m_serieLine != null )
        m_serieLine.Serialize( records );

      if( m_dataFormat != null )
      {
        m_dataFormat.Serialize( records );
      }

      if( m_dataLabels != null )
      {
        records.Add( ( ChartDataLabelsRecord )m_dataLabels.Clone() );
      }

      records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.End ) );
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Gets start Series type.
    /// </summary>
    /// <param name="type">Type to extract.</param>
    /// <returns>Returns start Series type.</returns>
    public static string GetStartSerieType( ExcelChartType type )
    {
      if( type == ExcelChartType.PieOfPie )
        return ChartImpl.START_PIE;

      string strType = type.ToString();
      int pos = strType.IndexOf( '_' );

      if( pos == -1 )
        return strType;

      return strType.Substring( 0, pos );
    }
    /// <summary>
    /// Changed chart type.
    /// </summary>
    /// <param name="type">Type to change.</param>
    /// <param name="isSeriesCreation">Indicates whether we are in the process of series creation.</param>
    public void ChangeChartType( ExcelChartType type, bool isSeriesCreation )
    {
      ChangeSerieType( type, isSeriesCreation );
    }
    /// <summary>
    /// Sets null for some records.
    /// </summary>
    private void SetNullForAllRecords()
    {
      m_chart3D = null;
      m_chartChartLine = null;
      m_serieLine = null;
      m_dataFormat = null;
      m_dataLabels = null;
      m_firstDropBar = null;
      m_secondDropBar = null;
      m_seriesList = null;
    }
    /// <summary>
    /// Changes for chart stock.
    /// </summary>
    private void ChangeChartStockLine()
    {
      ExcelChartType destType = m_chart.DestinationType;
      m_chart.DestinationType = ExcelChartType.Line;

      m_serieFormat = BiffRecordFactory.GetRecord( TBIFFRecord.ChartLine );

      m_serieLine = new ChartBorderImpl( Application, this );

      m_serieLine.LineWeight = ExcelChartLineWeight.Hairline;
      m_serieLine.ColorIndex = ChartWallOrFloorImpl.DEF_CATEGORY_BACKGROUND_COLOR_INDEX;
      LineStyle = ExcelDropLineStyle.HiLow;

      IChartBorder border = SerieDataFormat.LineProperties;
      border.LineWeight = ExcelChartLineWeight.Hairline;
      border.LinePattern = ExcelChartLinePattern.None;
      border.ColorIndex = ChartWallOrFloorImpl.DEF_CATEGORY_BACKGROUND_COLOR_INDEX;
      m_dataFormat.SeriesNumber = DEF_SERIES_NUMBER;
      m_dataFormat.MarkerStyle = ExcelChartMarkerType.None;
      m_dataFormat.MarkerForegroundColorIndex = ChartWallOrFloorImpl.DEF_VALUE_BACKGROUND_COLOR_INDEX;
      m_dataFormat.MarkerBackgroundColorIndex = ChartWallOrFloorImpl.DEF_VALUE_BACKGROUND_COLOR_INDEX;
      m_dataFormat.IsAutoMarker = false;

      m_chart.PrimaryCategoryAxis.IsBetween = true;

      IChartGridLine gridline = m_chart.PrimaryValueAxis.MajorGridLines;

      m_chart.DestinationType = destType;
    }
    /// <summary>
    /// Changes type for stock high low close type.
    /// </summary>
    public void ChangeChartStockHigh_Low_CloseType()
    {
      ExcelChartType destType = m_chart.DestinationType;
      m_chart.DestinationType = ExcelChartType.Line;

      ChangeChartStockLine();

      ChartDataPointImpl dataPoint = ( ChartDataPointImpl )
        m_chart.Series[ 2 ].DataPoints.DefaultDataPoint;

      dataPoint.ChangeChartStockHigh_Low_CloseType();

      m_chart.DestinationType = destType;
    }
    /// <summary>
    /// Changes type for stock open high low close type.
    /// </summary>
    public void ChangeChartStockOpen_High_Low_CloseType()
    {
      ChangeChartStockLine();

      FirstDropBar.Gap = 150;
      IChartBorder border = m_firstDropBar.LineProperties;
      border.LinePattern = ExcelChartLinePattern.Solid;
      border.LineWeight = ExcelChartLineWeight.Hairline;

      IChartInterior interior = m_firstDropBar.Interior;
      interior.Pattern = ExcelPattern.Solid;
      border.ColorIndex = ChartWallOrFloorImpl.DEF_CATEGORY_BACKGROUND_COLOR_INDEX;
      border.AutoFormat = true;
      //m_firstDropBar.ForegroundColor = 16777215;
      interior.ForegroundColorIndex = ExcelKnownColors.WhiteCustom;
      interior.BackgroundColorIndex = ExcelKnownColors.Custom0;

      m_secondDropBar = m_firstDropBar.Clone( this );
      //m_secondDropBar.ForegroundColor = 0;
      interior = m_secondDropBar.Interior;
      interior.ForegroundColorIndex = ExcelKnownColors.Custom0;
      interior.BackgroundColorIndex = ExcelKnownColors.WhiteCustom;
    }
    /// <summary>
    /// Change type for stock volume high low close type in primary axis.
    /// </summary>
    public void ChangeChartStockVolume_High_Low_CloseTypeFirst()
    {
      m_serieFormat = BiffRecordFactory.GetRecord( TBIFFRecord.ChartBar );
      IsVaryColor = false;
    }
    /// <summary>
    /// Change type for stock volume high low close type in secondary axis.
    /// </summary>
    public void ChangeChartStockVolume_High_Low_CloseTypeSecond()
    {
      ChangeChartStockLine();

      ushort[] arr = new ushort[ 4 ]{ 1, 2, 3, 4 };

      m_seriesList = ( ChartSeriesListRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartSeriesList );

      m_seriesList.Series = arr;
      m_chart.SecondaryParentAxis.UpdateSecondaryAxis( true );

      for( int i = 1; i < 4; i++ )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )m_chart.Series[ i ];

        serie.ChartGroup = 1;

        if( i == 3 )
        {
          ChartDataPointImpl dataPoint =
            ( ChartDataPointImpl )serie.DataPoints.DefaultDataPoint;

          dataPoint.ChangeChartStockVolume_High_Low_CloseType();
        }
      }
    }
    /// <summary>
    /// Change type for stock volume open high low close type.
    /// </summary>
    public void ChangeChartStockVolume_Open_High_Low_CloseType()
    {
      ChangeChartStockOpen_High_Low_CloseType();

      FirstDropBar.Gap = 100;
      SecondDropBar.Gap = 100;

      //m_firstDropBar.ForegroundColor = 16777215;
      IChartInterior interior = m_firstDropBar.Interior;
      interior.ForegroundColorIndex = ExcelKnownColors.WhiteCustom;
      interior.BackgroundColorIndex = ExcelKnownColors.Custom0;

      ExcelChartType destType = m_chart.DestinationType;
      m_chart.DestinationType = ExcelChartType.Line;
      //SerieDataFormat.AreaProperties.Pattern = 0;
      ( ( ChartSerieDataFormatImpl )SerieDataFormat ).SeriesNumber = 65533;
      m_chart.DestinationType = destType;

      SecondDropBar.Interior.BackgroundColor = Color.FromArgb( 0, 255, 255, 255 );

      m_seriesList = ( ChartSeriesListRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartSeriesList );

      ushort[] arr = new ushort[ 5 ]{ 1, 2, 3, 4, 5 };
      m_seriesList.Series = arr;

      for( int i = 1; i < 5; i++ )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )m_chart.Series[ i ];

        serie.ChartGroup = 1;
      }
    }
    /// <summary>
    /// Changes Series type.
    /// </summary>
    /// <param name="type">Type to change.</param>
    /// <param name="isSeriesCreation">Indicates whether we are in the process of series creation.</param>
    public void ChangeSerieType( ExcelChartType type, bool isSeriesCreation )
    {
      switch( type )
      {
        case ExcelChartType.Doughnut:
        case ExcelChartType.Doughnut_Exploded:
          ChangeSerieDoughnut( type );
          break;

        case ExcelChartType.Surface_NoColor_3D:
        case ExcelChartType.Surface_3D:
        case ExcelChartType.Surface_Contour:
        case ExcelChartType.Surface_NoColor_Contour:
          ChangeSerieSurface( type, isSeriesCreation );
          break;

        case ExcelChartType.Column_Clustered:
        case ExcelChartType.Column_Clustered_3D:
        case ExcelChartType.Column_3D:
        case ExcelChartType.Bar_Clustered:
        case ExcelChartType.Bar_Clustered_3D:
          ChangeSerieBarClustered( type );
          break;

        case ExcelChartType.Radar_Markers:
        case ExcelChartType.Radar:
        case ExcelChartType.Radar_Filled:
          ChangeSerieRadar( type );
          break;

        case ExcelChartType.Column_Stacked:
        case ExcelChartType.Column_Stacked_100:
        case ExcelChartType.Column_Stacked_100_3D:
        case ExcelChartType.Column_Stacked_3D:
        case ExcelChartType.Bar_Stacked:
        case ExcelChartType.Bar_Stacked_100:
        case ExcelChartType.Bar_Stacked_100_3D:
        case ExcelChartType.Bar_Stacked_3D:
          ChangeSerieBarStacked( type );
          break;

        case ExcelChartType.Line_Markers:
        case ExcelChartType.Line_3D:
        case ExcelChartType.Line_Markers_Stacked:
        case ExcelChartType.Line_Markers_Stacked_100:
        case ExcelChartType.Line:
        case ExcelChartType.Line_Stacked:
        case ExcelChartType.Line_Stacked_100:
          ChangeSerieLine( type );
          break;

        case ExcelChartType.Pie:
        case ExcelChartType.Pie_3D:
        case ExcelChartType.Pie_Bar:
        case ExcelChartType.PieOfPie:
        case ExcelChartType.Pie_Exploded:
        case ExcelChartType.Pie_Exploded_3D:
          ChangeSeriePie( type );
          break;

        case ExcelChartType.Area:
        case ExcelChartType.Area_Stacked:
        case ExcelChartType.Area_Stacked_100:
        case ExcelChartType.Area_Stacked_100_3D:
        case ExcelChartType.Area_Stacked_3D:
        case ExcelChartType.Area_3D:
          ChangeSerieArea( type );
          break;

        case ExcelChartType.Scatter_Line_Markers:
        case ExcelChartType.Scatter_Line:
        case ExcelChartType.Scatter_SmoothedLine:
        case ExcelChartType.Scatter_SmoothedLine_Markers:
        case ExcelChartType.Scatter_Markers:
          ChangeSerieScatter( type );
          break;

        case ExcelChartType.Bubble:
        case ExcelChartType.Bubble_3D:
          ChangeSerieBuble( type, isSeriesCreation );
          break;

        case ExcelChartType.Cone_Bar_Clustered:
        case ExcelChartType.Cone_Bar_Stacked:
        case ExcelChartType.Cone_Bar_Stacked_100:
        case ExcelChartType.Cone_Clustered:
        case ExcelChartType.Cone_Clustered_3D:
        case ExcelChartType.Cone_Stacked:
        case ExcelChartType.Cone_Stacked_100:
        case ExcelChartType.Pyramid_Bar_Clustered:
        case ExcelChartType.Pyramid_Bar_Stacked:
        case ExcelChartType.Pyramid_Bar_Stacked_100:
        case ExcelChartType.Pyramid_Clustered:
        case ExcelChartType.Pyramid_Clustered_3D:
        case ExcelChartType.Pyramid_Stacked:
        case ExcelChartType.Pyramid_Stacked_100:
        case ExcelChartType.Cylinder_Bar_Clustered:
        case ExcelChartType.Cylinder_Bar_Stacked:
        case ExcelChartType.Cylinder_Bar_Stacked_100:
        case ExcelChartType.Cylinder_Clustered:
        case ExcelChartType.Cylinder_Clustered_3D:
        case ExcelChartType.Cylinder_Stacked:
        case ExcelChartType.Cylinder_Stacked_100:
          ChangeSerieConeCylinderPyramyd( type );
          break;

        default:
          throw new NotSupportedException( "Cannot change serie type." );
      }

      string strStartType = ChartFormatImpl.GetStartSerieType( type );

      if( !m_chart.ParentWorkbook.Loading )
      {
        m_chart.PrimaryCategoryAxis.IsBetween =
          !( strStartType == "Area" || strStartType == "Surface" );
      }
    }
    /// <summary>
    /// Change Series type as one kind of doughnut.
    /// </summary>
    /// <param name="type">Type to change.</param>
    private void ChangeSerieDoughnut( ExcelChartType type )
    {
      SetNullForAllRecords();

      m_serieFormat = ( ChartPieRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartPie );

      m_chartChartFormat.IsVaryColor = true;
      DoughnutHoleSize = 50;

      if( type == ExcelChartType.Doughnut_Exploded )
      {
        SerieDataFormat.Percent = 25;
      }
    }
    /// <summary>
    /// Change Series type to one kind of bubble.
    /// </summary>
    /// <param name="type">Type to change.</param>
    /// <param name="isSeriesCreation">Indicates whether we are in the process of series creation.</param>
    private void ChangeSerieBuble( ExcelChartType type, bool isSeriesCreation )
    {
      if( m_chart.Series.Count < 2 && !isSeriesCreation )
        throw new ArgumentException( "Cannot change chart type." );

      SetNullForAllRecords();

      m_serieFormat = ( ChartScatterRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartScatter );

      SizeRepresents = ExcelBubbleSize.Area;
      BubbleScale = 100;
      IsBubbles = true;

      if( type == ExcelChartType.Bubble_3D )
        SerieDataFormat.Is3DBubbles = true;

      if( !isSeriesCreation )
        UpdateBubbleSeries( m_chart.Series );
    }
    /// <summary>
    /// Updates bubble series.
    /// </summary>
    /// <param name="series">Represents series collection.</param>
    private void UpdateBubbleSeries( IChartSeries series )
    {
      if( series == null )
        throw new ArgumentNullException( "series" );

      int iOldCount = m_chart.Series.Count;

      for( int i = 0; i < series.Count - 1; i = i + 2 )
      {
        ChartSerieImpl serie = ( ChartSerieImpl )series[ i ];
        IChartSerie serieSize = series[ i + 1 ];

        serie.Bubbles = serieSize.Values;
        serie.Index = i;
        serie.Number = i;

        series.RemoveAt( i + 1 );

        i--;
      }

      if( iOldCount % 2 != 0 )
      {
        IChartSerie firstSerie = series[ 0 ];
        IRange range = firstSerie.Values;

        int iPointCount = ( firstSerie.Values != null )
          ? Math.Max( range.LastRow - range.Row + 1, range.LastColumn - range.Column + 1 )
          : firstSerie.EnteredDirectlyValues.Length;

        object[] arrBubbles = new object[ iPointCount ];

        for( int i = 0; i < iPointCount; i++ )
        {
          arrBubbles[ i ] = 0;
        }

        int iIndex = series.Count - 1;
        ChartSerieImpl lastSerie = ( ChartSerieImpl )series[ series.Count - 1 ];

        lastSerie.EnteredDirectlyBubbles = arrBubbles;
        lastSerie.Index = iIndex;
        lastSerie.Number = iIndex;
      }
    }
    /// <summary>
    /// Change Series type as one kind of surface.
    /// </summary>
    /// <param name="type">Type to change.</param>
    /// <param name="isSeriesCreation">Indicates whether we are in the process of series creation.</param>
    private void ChangeSerieSurface( ExcelChartType type, bool isSeriesCreation )
    {
      if( m_chart.Series.Count < 2 && !isSeriesCreation )
        throw new ArgumentException( "Cannot change type. Chart cannot contain less then 2 series." );

      SetNullForAllRecords();

      m_serieFormat = ( ChartSurfaceRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartSurface );

      RightAngleAxes = false;

      if( type == ExcelChartType.Surface_3D
        || type == ExcelChartType.Surface_Contour )
        IsFillSurface = true;

      if( type == ExcelChartType.Surface_NoColor_Contour
        || type == ExcelChartType.Surface_Contour )
      {
        Rotation = 0;
        Elevation = 90;
        Perspective = 0;
        IsVaryColor = false;
      }
    }
    /// <summary>
    /// Change Series type as one kind of radar.
    /// </summary>
    /// <param name="type">Type to change.</param>
    private void ChangeSerieRadar( ExcelChartType type )
    {
      SetNullForAllRecords();

      if( type == ExcelChartType.Radar_Filled )
      {
        ChartRadarAreaRecord radar = ( ChartRadarAreaRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartRadarArea );

        radar.IsRadarAxisLabel = true;
        m_serieFormat = radar;

        IsCategoryName = true;
      }
      else
      {
        ChartRadarRecord radar = ( ChartRadarRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartRadar );

        radar.IsRadarAxisLabel = true;
        m_serieFormat = radar;

        if( type == ExcelChartType.Radar )
        {
          HasRadarAxisLabels = true;

          ( ( ChartSerieDataFormatImpl )SerieDataFormat ).ChangeRadarDataFormat( type );
        }
      }
    }
    /// <summary>
    /// Change Series type as one kind of bar clustered.
    /// </summary>
    /// <param name="type">Type to change.</param>
    private void ChangeSerieBarClustered( ExcelChartType type )
    {
      SetNullForAllRecords();

      m_serieFormat = ( ChartBarRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartBar );

      if( type == ExcelChartType.Column_Clustered_3D
        || type == ExcelChartType.Bar_Clustered_3D )
        IsClustered = true;

      if( type == ExcelChartType.Column_3D )
      {
        RightAngleAxes = false;

        return;
      }

      string strType = type.ToString();

      if( strType.IndexOf( "Bar_" ) >= 0 )
      {
        IsHorizontalBar = true;
      }
    }
    /// <summary>
    /// Change Series type as one kind of bar stacked.
    /// </summary>
    /// <param name="type">Type to change.</param>
    private void ChangeSerieBarStacked( ExcelChartType type )
    {
      ChangeSerieBarClustered( type );

      StackValuesBar = true;
      BarRecord.Overlap = DEF_BAR_STACKED;
      if (m_chart != null)
          m_chart.OverLap = DEF_BAR_STACKED;

      if( type == ExcelChartType.Column_Stacked_100
        || type == ExcelChartType.Bar_Stacked_100 )
      {
        ShowAsPercentsBar = true;

        return;
      }

      switch( type )
      {
        case ExcelChartType.Column_Stacked_3D:
        case ExcelChartType.Bar_Stacked_3D:
          m_chart3D = ( Chart3DRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.Chart3D );
          break;

        case ExcelChartType.Column_Stacked_100_3D:
        case ExcelChartType.Bar_Stacked_100_3D:
          m_chart3D = ( Chart3DRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.Chart3D );

          ShowAsPercentsBar = true;
          break;
      }
    }
    /// <summary>
    /// Change Series type as one kind of line.
    /// </summary>
    /// <param name="type">Type to change.</param>
    private void ChangeSerieLine( ExcelChartType type )
    {
      if( !m_chart.ParentWorkbook.Loading )
        SetNullForAllRecords();

      m_serieFormat = ( ChartLineRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartLine );

      if( type == ExcelChartType.Line_Markers )
        return;

      if( type == ExcelChartType.Line_3D )
      {
        m_chart3D = ( Chart3DRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.Chart3D );

        RightAngleAxes = false;

        return;
      }

      if( type == ExcelChartType.Line_Markers_Stacked
        || type == ExcelChartType.Line_Stacked )
      {
        StackValuesLine = true;
      }

      if( type == ExcelChartType.Line_Markers_Stacked_100
        || type == ExcelChartType.Line_Stacked_100 )
      {
        StackValuesLine = true;
        ShowAsPercentsLine = true;
      }

      if( type == ExcelChartType.Line
        || type == ExcelChartType.Line_Stacked
        || type == ExcelChartType.Line_Stacked_100 )
      {
        ( ( ChartSerieDataFormatImpl )SerieDataFormat ).ChangeLineDataFormat( type );
      }
    }
    /// <summary>
    /// Change Series type as one kind of pie.
    /// </summary>
    /// <param name="type">Type to change.</param>
    private void ChangeSeriePie( ExcelChartType type )
    {
      SetNullForAllRecords();

      IsVaryColor = true;

      m_serieFormat = ( ChartPieRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartPie );

      if( type == ExcelChartType.Pie_3D
        || type == ExcelChartType.Pie_Exploded_3D )
      {
        m_chart3D = ( Chart3DRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.Chart3D );
      }

      if( type == ExcelChartType.Pie_Exploded
         || type == ExcelChartType.Pie_Exploded_3D )
      {
        SerieDataFormat.Percent = 25;
      }

      if( type == ExcelChartType.Pie_Bar
        || type == ExcelChartType.PieOfPie )
      {
        m_serieFormat = ( ChartBoppopRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.ChartBoppop );

        UseDefaultSplitValue = true;
        PieChartType = ExcelPieType.Bar;
        PieSecondSize = 75;
        Gap = 100;
        LineStyle = ExcelDropLineStyle.Series;
        
        m_serieLine = new ChartBorderImpl( Application, this );

        if( type == ExcelChartType.PieOfPie )
          PieChartType = ExcelPieType.Pie;
      }
    }
    /// <summary>
    /// Change Series type as one kind of area.
    /// </summary>
    /// <param name="type">Type to change.</param>
    private void ChangeSerieArea( ExcelChartType type )
    {
      SetNullForAllRecords();

      m_serieFormat = ( ChartAreaRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartArea );

      if( type == ExcelChartType.Area_3D
        || type == ExcelChartType.Area_Stacked_3D
        || type == ExcelChartType.Area_Stacked_100_3D )
      {
        m_chart3D = ( Chart3DRecord )
          BiffRecordFactory.GetRecord( TBIFFRecord.Chart3D );
      }

      if( type == ExcelChartType.Area_Stacked
        || type == ExcelChartType.Area_Stacked_3D )
      {
        IsStacked = true;
      }

      if( type == ExcelChartType.Area_Stacked_100
        || type == ExcelChartType.Area_Stacked_100_3D )
      {
        IsStacked = true;
        IsCategoryBrokenDown = true;
      }

      if( type == ExcelChartType.Area_3D )
        RightAngleAxes = false;
    }
    /// <summary>
    /// Change Series type as one kind of scatter.
    /// </summary>
    /// <param name="type">Type to change.</param>
    private void ChangeSerieScatter( ExcelChartType type )
    {
      SetNullForAllRecords();

      m_serieFormat = ( ChartScatterRecord )
        BiffRecordFactory.GetRecord( TBIFFRecord.ChartScatter );

      SizeRepresents = ExcelBubbleSize.Area;
      BubbleScale = 100;

      if( type == ExcelChartType.Scatter_Line_Markers )
        return;

      ( ( ChartSerieDataFormatImpl )SerieDataFormat ).ChangeScatterDataFormat( type );
    }
    /// <summary>
    /// Changes Series type for Cylinder or Pyramid, Cone.
    /// </summary>
    /// <param name="type">Type to change.</param>
    private void ChangeSerieConeCylinderPyramyd( ExcelChartType type )
    {
      switch( type )
      {
        case ExcelChartType.Cone_Bar_Clustered:
        case ExcelChartType.Pyramid_Bar_Clustered:
        case ExcelChartType.Cylinder_Bar_Clustered:
          ChangeSerieBarClustered( ExcelChartType.Bar_Clustered_3D );
          break;

        case ExcelChartType.Cone_Clustered:
        case ExcelChartType.Pyramid_Clustered:
        case ExcelChartType.Cylinder_Clustered:
          ChangeSerieBarClustered( ExcelChartType.Column_Clustered_3D );
          break;

        case ExcelChartType.Cone_Clustered_3D:
        case ExcelChartType.Pyramid_Clustered_3D:
        case ExcelChartType.Cylinder_Clustered_3D:
          ChangeSerieBarClustered( ExcelChartType.Column_3D );
          break;

        case ExcelChartType.Cone_Bar_Stacked:
        case ExcelChartType.Pyramid_Bar_Stacked:
        case ExcelChartType.Cylinder_Bar_Stacked:
          ChangeSerieBarStacked( ExcelChartType.Bar_Stacked_3D );
          break;

        case ExcelChartType.Cone_Stacked:
        case ExcelChartType.Pyramid_Stacked:
        case ExcelChartType.Cylinder_Stacked:
          ChangeSerieBarStacked( ExcelChartType.Column_Stacked_3D );
          break;

        case ExcelChartType.Cone_Bar_Stacked_100:
        case ExcelChartType.Pyramid_Bar_Stacked_100:
        case ExcelChartType.Cylinder_Bar_Stacked_100:
          ChangeSerieBarStacked( ExcelChartType.Bar_Stacked_100_3D );
          break;

        case ExcelChartType.Cone_Stacked_100:
        case ExcelChartType.Pyramid_Stacked_100:
        case ExcelChartType.Cylinder_Stacked_100:
          ChangeSerieBarStacked( ExcelChartType.Column_Stacked_100_3D );
          break;
      }

      ExcelBaseFormat baseFormat;
      ExcelTopFormat topFormat;

      switch( type )
      {
        case ExcelChartType.Cone_Bar_Clustered:
        case ExcelChartType.Cone_Bar_Stacked:
        case ExcelChartType.Cone_Bar_Stacked_100:
        case ExcelChartType.Cone_Clustered:
        case ExcelChartType.Cone_Clustered_3D:
        case ExcelChartType.Cone_Stacked:
        case ExcelChartType.Cone_Stacked_100:
          baseFormat = ExcelBaseFormat.Circle;
          topFormat = ExcelTopFormat.Sharp;
          break;

        case ExcelChartType.Pyramid_Bar_Clustered:
        case ExcelChartType.Pyramid_Bar_Stacked:
        case ExcelChartType.Pyramid_Bar_Stacked_100:
        case ExcelChartType.Pyramid_Clustered:
        case ExcelChartType.Pyramid_Clustered_3D:
        case ExcelChartType.Pyramid_Stacked:
        case ExcelChartType.Pyramid_Stacked_100:
          baseFormat = ExcelBaseFormat.Rectangle;
          topFormat = ExcelTopFormat.Sharp;
          break;

        case ExcelChartType.Cylinder_Bar_Clustered:
        case ExcelChartType.Cylinder_Bar_Stacked:
        case ExcelChartType.Cylinder_Bar_Stacked_100:
        case ExcelChartType.Cylinder_Clustered:
        case ExcelChartType.Cylinder_Clustered_3D:
        case ExcelChartType.Cylinder_Stacked:
        case ExcelChartType.Cylinder_Stacked_100:
          baseFormat = ExcelBaseFormat.Circle;
          topFormat = ExcelTopFormat.Straight;
          break;

        default:
          throw new ArgumentException( "type" );
      }

      ( ( ChartSeriesCollection )m_chart.Series ).UpdateDataPointForCylConePurChartType( 
        baseFormat, topFormat );

      SerieDataFormat.BarShapeBase = baseFormat;
      SerieDataFormat.BarShapeTop = topFormat;
    }
    /// <summary>
    /// Clones current object.
    /// </summary>
    /// <param name="parent">Parent object for clone.</param>
    /// <returns>Returns just cloned object.</returns>
    public object Clone( object parent )
    {
      ChartFormatImpl format = ( ChartFormatImpl )MemberwiseClone();

      format.SetParent( parent );
      format.SetParents();

      format.m_chartChartFormat = ( ChartChartFormatRecord )CloneUtils.CloneCloneable( m_chartChartFormat );
      format.m_serieFormat = ( BiffRecordRaw )CloneUtils.CloneCloneable( m_serieFormat );
      format.m_chart3D = ( Chart3DRecord )CloneUtils.CloneCloneable( m_chart3D );
      format.m_formatLink = ( ChartFormatLinkRecord )CloneUtils.CloneCloneable( m_formatLink );
      format.m_dataLabels = ( ChartDataLabelsRecord )CloneUtils.CloneCloneable( m_dataLabels );
      format.m_chartChartLine = ( ChartChartLineRecord )CloneUtils.CloneCloneable( m_chartChartLine );
      
      if( m_serieLine != null )
        format.m_serieLine = m_serieLine.Clone( format );

      format.m_seriesList = ( ChartSeriesListRecord )CloneUtils.CloneCloneable( m_seriesList );

      if( m_firstDropBar != null )
        format.m_firstDropBar = m_firstDropBar.Clone( format );

      if( m_secondDropBar != null )
        format.m_secondDropBar = m_secondDropBar.Clone( format );

      if( m_dataFormat != null )
        format.m_dataFormat = m_dataFormat.Clone( format );

      return format;
    }
    public static bool operator==( ChartFormatImpl format1, ChartFormatImpl format2 )
    {
      if( Object.Equals( format1, null ) && Object.Equals( format2, null ) )
        return true;

      if( Object.Equals( format1, null ) || Object.Equals( format2, null ) )
        return false;

      if( format1.m_serieFormat.TypeCode != format2.m_serieFormat.TypeCode )
        return false;

      int iStoreSize1 = format1.m_serieFormat.GetStoreSize( ExcelVersion.Excel97to2003 );
      int iStoreSize2 = format1.m_serieFormat.GetStoreSize( ExcelVersion.Excel97to2003 );

      if( iStoreSize1 != iStoreSize2 )
        return false;

      ByteArrayDataProvider array = new ByteArrayDataProvider( new byte[ iStoreSize1 ] );
      format1.m_serieFormat.InfillInternalData( array, 0, ExcelVersion.Excel97to2003 );

      ByteArrayDataProvider array2 = new ByteArrayDataProvider( new byte[ iStoreSize2 ] );
      format2.m_serieFormat.InfillInternalData( array2, 0, ExcelVersion.Excel97to2003 );


      return BiffRecordRaw.CompareArrays( array.InternalBuffer, array2.InternalBuffer ) &&
        format1.m_chartChartFormat.EqualsWithoutOrder( format2.m_chartChartFormat ) &&
        format1.m_chart3D == format2.m_chart3D &&
        format1.m_seriesList == format2.m_seriesList &&
        format1.m_chartChartLine == format2.m_chartChartLine &&
        format1.m_dataLabels == format2.m_dataLabels;
        //format1.m_formatLink == format2.m_formatLink;
        //format1.m_firstDropBar == format2.m_firstDropBar &&
        //format2.m_secondDropBar == format2.m_secondDropBar;
/*
    ChartSerieDataFormatImpl m_dataFormat = null;
    ChartDropBarImpl m_firstDropBar = null;
    ChartDropBarImpl m_secondDropBar = null;
    ChartBorderImpl m_serieLine;
*/

    }
    public static bool operator !=( ChartFormatImpl format1, ChartFormatImpl format2 )
    {
      return !( format1 == format2 );
    }
    #endregion
	}
}
