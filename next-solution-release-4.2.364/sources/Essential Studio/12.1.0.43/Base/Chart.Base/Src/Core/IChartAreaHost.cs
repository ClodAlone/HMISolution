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

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using Syncfusion.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Chart
{

	/// <summary>
	/// 
	/// </summary>
	/// <internalonly/>
  [Syncfusion.Documentation.DocumentationExclude()]
  public interface IChartAreaHost
  {
    /// <internalonly/>
    Graphics GetGraphics();

    /// <internalonly/>
    void OnChartFormatAxisLabel(ChartAxis axis, ChartFormatAxisLabelEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="update"></param>
    void Redraw(bool update);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="img"></param>
    void Draw( Image img );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    /// <param name="sz"></param>
    void Draw( Graphics g, Size sz );
    /// <summary>
    /// 
    /// </summary>
    /// <param name="img"></param>
    /// <param name="sz"></param>
    void Draw( Image img, Size sz );
    /// <summary>
    /// Saves the chart as an image in the specified format.
    /// </summary>
    /// <param name="filename"></param>
    void SaveImage( string filename );

    /// <summary>
    /// Event that will be raised when Chart has completed laying out of axes, legend
    /// </summary>
    event EventHandler LayoutCompleted;
    /// <summary>
    /// 
    /// </summary>
    event PaintEventHandler ChartAreaPaint;

		/// <summary>
		/// Gets a value indicating whether it's design time.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if it's design time; otherwise, <c>false</c>.
		/// </value>
		bool IsDesignTime { get; }

    /// <internalonly/>
    void Refresh();

    /// <internalonly/>
    void SeriesChanged( object sender, ChartSeriesCollectionChangedEventArgs e );

    /// <internalonly/>
    int ActiveIndex{get;}
    /// <internalonly/>
    bool AutoHighlight  {get;set;}
    /// <internalonly/>
    bool SeriesHighlight { get; set; }
    /// <internalonly/>
    bool HighlightSymbol { get; set; }
    /// <internalonly/>
    int SeriesHighlightIndex { get; set; }
    /// <internalonly/>
    IChartArea GetChartArea();

    /// <internalonly/>
		ChartRegionCollection ChartRegions { get;}

    /// <internalonly/>
    Point ClickPoint{get;}

    /// <summary>
    /// 
    /// </summary>
    ChartColumnDrawMode ColumnDrawMode{get;set;}

    /// <internalonly/>
    ChartColumnWidthMode ColumnWidthMode{get;set;}

    /// <internalonly/>
    int ColumnFixedWidth{get;set;}
    /// <internalonly/>
    bool CompatibleSeries  {get;set;}
    /// <internalonly/>
    int ElementsSpacing {get;set;}
    /// <internalonly/>
    bool EnableXZooming{get;set;}
    /// <internalonly/>
    bool ImprovePerformance { get; set; }

    /// <internalonly/>
    bool EnableYZooming{get;set;}
    /// <internalonly/>
    bool Indexed  {get;set;}
    /// <internalonly/>
    bool AllowGapForEmptyPoints { get;set;}
    /// <internalonly/>
	ChartIndexedValues IndexValues { get;}
    /// <internalonly/>
    ChartDock LegendPosition {get;set;}
    /// <internalonly/>
    ChartModel Model  {get;set;}
    /// <internalonly/>
    Point MouseDownPosition{get;}
    /// <internalonly/>
    bool NeedRegionUpdate {get;set;}
    /// <internalonly/>
    bool CalcRegions {get;set;}
    /// <internalonly/>
    bool Radar  {get;}
    /// <internalonly/>
    bool Polar  {get;}
    /// <internalonly/>
    ChartRadarAxisStyle RadarStyle  {get;set;}
    /// <internalonly/>
    bool RequireAxes  {get;set;}
    /// <internalonly/>
    bool RequireInvertedAxes  {get;set;}
    /// <internalonly/>
    ChartSeriesCollection Series { get;}
    /// <internalonly/>
    bool Series3D  {get;set;}
     /// <internalonly/>
	bool Style3D { get; set; }
    /// <internalonly/>
    SmoothingMode SmoothingMode  {get;set;}
    /// <internalonly/>
    TextRenderingHint TextRenderingHint  {get;set;}

    /// <internalonly/>
    float Spacing  {get;set;}
    /// <internalonly/>
    float SpacingBetweenSeries  {get;set;}
		
    /// <internalonly/>
    double ZoomFactorX{get;set;}

    /// <internalonly/>
    double ZoomFactorY{get;set;}

    /// <internalonly/>
    ChartZooming Zooming{get;}
    
    /// <internalonly/>
    double ZoomOutIncrement{get;set;}

    /// <internalonly/>
    double ZoomPositionX{get;set;}

    /// <internalonly/>
    double ZoomPositionY{get;set;}

		/// <internalonly/>
		bool InteractiveCursorMouseDown { get; }

		/// <summary>
		/// Gets or sets a value indicating whether is real 3D mode.
		/// </summary>
		/// <value><c>true</c> if is real 3D mode; otherwise, <c>false</c>.</value>
    	bool RealMode3D{get;set;}

		/// <summary>
		/// Gets or sets a value indicating whether [drop series points].
		/// </summary>
		/// <value><c>true</c> if [drop series points]; otherwise, <c>false</c>.</value>
    	bool DropSeriesPoints{get;set;}

        /// <summary>
        /// Gets or sets a value indicating whether [improve performance].
        /// </summary>
        /// <value><c>true</c> if [improve performance]; otherwise, <c>false</c>.</value>
        bool NeedPerformance { get;set;}
		/// <summary>
		/// Gets or sets a value indicating whether legend is shown.
		/// </summary>
		/// <value><c>true</c> if legend is shown; otherwise, <c>false</c>.</value>
		bool ShowLegend { get;set;} 
		/// <summary>
		/// Gets or sets the interior of axis grid.
		/// </summary>
		/// <value>The chart interior.</value>
    	BrushInfo ChartInterior { get;set; }
    	/// <summary>
    	/// Gets or sets the color with which the ChartArea is to be filled initially before any rendering takes place.
    	/// </summary>
    	BrushInfo BackInterior { get; set; }
		/// <summary>
		/// Gets or sets the font.
		/// </summary>
		/// <value>The font.</value>
		Font Font { get; set; }
		/// <summary>
		/// Gets or sets the color of the fore.
		/// </summary>
		/// <value>The color of the fore.</value>
		Color ForeColor { get; set; }
  }
}
