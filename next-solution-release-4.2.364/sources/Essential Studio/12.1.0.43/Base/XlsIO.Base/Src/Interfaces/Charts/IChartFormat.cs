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

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
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
#endregion

namespace Syncfusion.XlsIO
{
	/// <summary>
	/// Summary description for IChartFormat.
	/// </summary>
	public interface IChartFormat
	{
    #region Interface properties
    /// <summary>
    /// Vary color for each data point.
    /// </summary>
    [ Obsolete( "Please, use IsVaryColor instead of this property. Sorry for inconvenience." ) ]
    bool IsVeryColor
    {
      get;
      set;
    }
    /// <summary>
    /// Vary color for each data point.
    /// </summary>
    bool IsVaryColor { get; set; }
//    /// <summary>
//    /// Returns data format. Read-only.
//    /// </summary>
//    //IChartSerieDataFormat SerieDataFormat { get; }
    /// <summary>
    /// Space between bars ( -100 : 100 ).
    /// </summary>
    int Overlap { get; set; }
    /// <summary>
    /// Space between categories (percent of bar width), default = 50.
    /// </summary>
    int GapWidth { get; set; }
    /// <summary>
    /// Angle of the first pie slice expressed in degrees. ( 0 - 360 )
    /// </summary>
    int FirstSliceAngle { get; set; }
    /// <summary>
    /// Size of center hole in a doughnut chart (as a percentage).( 10 - 90 )
    /// </summary>
    int DoughnutHoleSize { get; set; }
    /// <summary>
    /// Percent of largest bubble compared to chart in general. ( 0 - 300 )
    /// </summary>
    int BubbleScale { get; set; }
    /// <summary>
    /// Returns or sets what the bubble size represents on a bubble chart.
    /// </summary>
    ExcelBubbleSize SizeRepresents { get; set; }
    /// <summary>
    /// True to show negative bubbles.
    /// </summary>
    bool ShowNegativeBubbles { get; set; }
    /// <summary>
    /// True if a radar chart has axis labels. Applies only to radar charts.
    /// </summary>
    bool HasRadarAxisLabels { get; set; }
    /// <summary>
    /// Returns or sets the way the two sections of either a pie of pie chart or a bar
    /// of pie chart are split.
    /// </summary>
    ExcelSplitType SplitType { get; set; }
    /// <summary>
    /// Returns or sets the threshold value separating the two sections of either a pie of pie chart or a bar of pie chart.
    /// </summary>
    int SplitValue { get; set; }
    /// <summary>
    /// Returns or sets the size of the secondary section of either a pie of pie chart or a bar of pie chart,
    /// as a percentage of the size of the primary pie. ( 5 - 200 )
    /// </summary>
    int PieSecondSize { get; set; }
    /// <summary>
    /// Returns object that represents first drop bar.
    /// </summary>
    IChartDropBar FirstDropBar { get; }
    /// <summary>
    /// Returns object that represents second drop bar.
    /// </summary>
    IChartDropBar SecondDropBar { get; }
    /// <summary>
    /// Represents series line properties. ( For pie of pie or pie of bar chart types only. ) Read-only.
    /// </summary>
    IChartBorder PieSeriesLine { get; }
    #endregion
	}
}
