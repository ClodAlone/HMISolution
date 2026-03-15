#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
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

namespace Syncfusion.XlsIO
{
  public interface ISparklineGroup : IList<ISparklines>
  {
    #region Properties

    /// <summary>
    /// Indicates whether to show the sparkline horizontal axis. The horizontal axis appears if the sparkline has data that crosses the zero axis.
    /// </summary>
    /// <value><c>true</c> if [display axis]; otherwise, <c>false</c>.</value>
    bool DisplayAxis {get; set; }
    /// <summary>
    /// Indicates whether to show data in hidden rows and columns.
    /// </summary>
    /// <value><c>true</c> if [display hidden RC]; otherwise, <c>false</c>.</value>
    bool DisplayHiddenRC { get; set; }
    /// <summary>
    /// Indicates whether the plot data is right to left. 
    /// </summary>
    /// <value><c>true</c> if [plot right to left]; otherwise, <c>false</c>.</value>
    bool PlotRightToLeft { get; set; }
    /// <summary>
    /// Indicates whether to highlight the first point of data in the sparkline group. 
    /// </summary>
    /// <value><c>true</c> if [show first point]; otherwise, <c>false</c>.</value>
    bool ShowFirstPoint { get; set; }
    /// <summary>
    /// Indicates whether to highlight the last point of data in the sparkline group. 
    /// </summary>
    /// <value><c>true</c> if [show last point]; otherwise, <c>false</c>.</value>
    bool ShowLastPoint {get; set; }
    /// <summary>
    /// Indicates whether to highlight the lowest points of data in the sparkline group.
    /// </summary>
    /// <value><c>true</c> if [show low point]; otherwise, <c>false</c>.</value>
    bool ShowLowPoint {get; set; }
    /// <summary>
    ///Indicates whether to highlight the highest points of data in the sparkline group. 
    /// </summary>
    /// <value><c>true</c> if [show high point]; otherwise, <c>false</c>.</value>
    bool ShowHighPoint {get; set; }
    /// <summary>
    /// Indicates whether to highlight the negative values on the sparkline group with a different color or marker.
    /// </summary>
    /// <value><c>true</c> if [show negative point]; otherwise, <c>false</c>.</value>
    bool ShowNegativePoint {get; set; }
    /// <summary>
    /// Indicates whether to highlight each point in each line sparkline in the sparkline group.  
    /// </summary>
    /// <value><c>true</c> if [show markers]; otherwise, <c>false</c>.</value>
    /// <exception cref=" NotSupportedException">If Sparklinetype is not equal to Line</exception>
    bool ShowMarkers {get; set; }
    /// <summary>
    /// The VerticalAxisMaximum property represents the Vertical Axis maximum options.
    /// </summary>
    /// <value>The VerticalAxisMaximum property gets/sets the m_verticalMaximum member.</value>
    ISparklineVerticalAxis VerticalAxisMaximum {get; set; }
    /// <summary>
    /// The VerticalAxisMinimum property represents the Vertical Axis minimum options.
    /// </summary>
    /// <value>The VerticalAxisMinimum property gets/sets the m_verticalMinimum member.</value>
    ISparklineVerticalAxis VerticalAxisMinimum {get; set; }
    /// <summary>
    /// Indicates the sparkline type of the sparkline group.
    /// </summary>
    /// <value>The SparklineType property gets/sets the m_sparklineType member.</value>
    SparklineType SparklineType {get; set; }

    /// <summary>
    /// The HorizontalDateAxis property represents the horizontal axis type as Dateaxis.
    /// </summary>
    /// <value><c>true</c> if [horizontal date axis]; otherwise, <c>false</c>.</value>
    bool HorizontalDateAxis {get; set; }
    /// <summary>
    /// Indicates how to display empty cells.
    /// </summary>
    /// <value>The DisplayEmptyCellsAs property gets/sets the m_displayEmptyCellsAs data member.</value>
    SparklineEmptyCells DisplayEmptyCellsAs {get; set; }
    /// <summary>
    /// Represents the range that contains the date values for the sparkline data.
    /// </summary>
    /// <value>The HorizontalDateAxisRange property gets/sets the m_horizontalDateAxisRange data member.</value>
    ///<exception cref=" ArgumentOutOfRangeException">
    ///if<paramref name="Value.Rows.Length"/>is not equal to 1
    ///if<paramref name="value.Column.Length"/>is not equal to 1
    ///if<paramref name="HorizontalDateAxis"/>is not true
    ///</exception>
    IRange HorizontalDateAxisRange {get; set; }
    /// <summary>
    /// Gets and sets the color of the horizontal axis in the sparkline group.
    /// </summary>
    /// <value>The color of the axis.</value>
    Color AxisColor {get; set; }
    /// <summary>
    /// Gets and sets the color of the first point of data in the sparkline group. 
    /// </summary>
    /// <value>The first color of the point.</value>
    Color FirstPointColor {get; set; }
    /// <summary>
    /// Gets and sets the color of the highest points of data in the sparkline group. 
    /// </summary>
    /// <value>The color of the high point.</value>
    Color HighPointColor {get; set; }
    /// <summary>
    /// Gets and sets the color of the last point of data in the sparkline group.
    /// </summary>
    /// <value>The last color of the point.</value>
    Color LastPointColor {get; set; }
    /// <summary>
    /// Gets and sets the line weight in each line sparkline in the sparkline group, in the unit of points. 
    /// </summary>
    /// <value>The line weight value should be between 0 and 1584.</value>
    /// <exception cref=" ArgumentOutOfRangeException">if the value is not between 0 and 1584</exception>
    double LineWeight {get; set; }
    /// <summary>
    /// Gets and sets the color of the lowest points of data in the sparkline group.
    /// </summary>
    /// <value>The color of the low point.</value>
    Color LowPointColor {get; set; }
    /// <summary>
    ///Gets and sets the color of points in each line sparkline in the sparkline group.
    /// </summary>
    /// <value>The color of the markers.</value>
    Color MarkersColor {get; set; }
    /// <summary>
    /// Gets and sets the color of the negative values on the sparkline group.
    /// </summary> {get; set; }
    /// <value>The color of the negative point.</value>
    Color NegativePointColor {get; set; }
    /// <summary>
    /// Gets and sets the color of the sparklines in the sparkline group. 
    /// </summary>
    /// <value>The color of the sparkline.</value>
    Color SparklineColor {get; set; }

    #endregion

    #region Implementation

    /// <summary>
    /// Adds Sparklines instance.
    /// </summary>
    /// <example>
    /// This Example Demostrated how to Add the Sparklines
    /// <code>
    /// ExcelEngine engine=new Excelengine();
    /// 
    /// IApplication app= engine.Excel;
    /// 
    /// app.DefaultVersion= ExcelVersion.Excel2010;
    /// 
    /// IWorkbook wkBook= app.Workbooks.Create(2); 
    /// IWorkSheet sheet= wkBook.Worksheets[0];
    /// 
    /// SparklineGroup spGroup= sheet.SparklineGroups.Add();        
    /// 
    /// //It returns the Sparklines object
    /// Sparklines spLines= spGroup.Add();
    /// 
    /// wkBook.SaveAs("Sample.xlsx");
    /// wkBook.Close();
    /// 
    /// </code>
    /// </example>
    /// <returns>Sparklines object</returns>
    ISparklines Add();

    #endregion

  }
}
