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

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Rectangle = System.Drawing.Rectangle;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// The interface that the <see cref="ChartArea"/> type implements.
	/// </summary>
	public interface IChartArea
	{
		//int BorderWidth { get; set; } 
		/// <summary>
		/// Gets or sets the border color of the rectangular area occupied by this ChartArea.
		/// </summary>
		Color BorderColor { get; set; }

		/// <summary>
		/// Gets or sets the width of the rectangular area occupied by this ChartArea.    
		/// </summary>
		int Width { get; set; }
		/// <summary>
		/// Gets or sets the height of the rectangular area that is occupied by this ChartArea.    
		/// </summary>
		int Height { get; set; }

		/// <summary>
		/// Returns the bounds associated with this ChartArea.    
		/// </summary>
		Rectangle Bounds { get; set; }
		/// <summary>
		/// Gets or sets the ClientRectangle associated with this ChartArea.    
		/// </summary>
		Rectangle ClientRectangle { get; }

		/// <summary>
		/// Returns the X axis offset value used when rendering in 3D mode.   
		/// </summary>
		float OffsetX { get; }
		/// <summary>
		/// Returns the Y axis offset value used when rendering in 3D mode.   
		/// </summary>
		float OffsetY { get; }

		/// <summary>
		/// Gets or sets the size of the rectangular area occupied by the ChartArea.    
		/// </summary>
		Size Size { get; set; }
		/// <summary>
		/// Gets or sets the location of the rectangular area occupied by this ChartArea.
		/// </summary>
		Point Location { get; set; }

		/// <summary>
		/// Indicates if the ChartArea is to be rendered in 3D. Default value is false.    
		/// </summary>
		bool Series3D { get; set; }
		/// <summary>
		/// Indicates if the ChartArea is to be rendered in 3D. Default value is false.    
		/// </summary>
		bool RealSeries3D { get; set; }

		/// <summary>
		/// Gets or sets the perception of depth that is to be used when the ChartArea is rendered in 3D.   
		/// </summary>
		float Depth { get; set; }
		/// <summary>
		/// Gets or sets the rotational angle that is to be used when the ChartArea is rendered in 3D.   
		/// </summary>
		float Rotation { get; set; }
		/// <summary>
		/// Gets or sets the tilt that is to be used when the ChartArea is rendered in 3D.   
		/// </summary>
		float Tilt { get; set; }
		/// <summary>
		/// Gets or sets the turn that is to be used when the ChartArea is rendered in Real 3D only.
		/// </summary>
		float Turn { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether area should scale automatically in 3D mode.
		/// </summary>
		/// <value><c>true</c> if area should scale automatically; otherwise, <c>false</c>.</value>
		bool AutoScale { get; set; }
		/// <summary>
		/// Gets or sets the color with which the ChartArea is to be filled initially before any rendering takes place.
		/// </summary>
		BrushInfo BackInterior { get; set; }
		/// <summary>
		/// Gets or sets the image that is to be used as the background for this ChartArea.    
		/// </summary>
		Image BackImage { get; set; }
		/// <summary>
		/// If set to true, the legend will show the series text (for Pie Chart).
		/// </summary>
		bool DivideArea { get; set; }

        /// <summary>
        /// If set to true, multiple pie chart series will be rendered in the same chart area.
        /// </summary>        
        bool MultiplePies { get; set; }

		/// <summary>
		/// Indicates if the ChartArea requires axes to be rendered (for the Chart types being rendered).
		/// </summary>
		bool RequireAxes { get; set; }

        /// <summary>
        /// Indicates to change the appearance of chart.
        /// </summary>
        bool LegacyAppearance { get; set; }
	
		/// <summary>
		/// Indicates if Chart requires Inverted Axes.
		/// </summary>
		bool RequireInvertedAxes { get; set; }

		/// <summary>
		/// Collection of axes associated with this chart. You can add and remove axes from this collection.
		/// Primary X and Y axes may not be removed.
		/// </summary>
		ChartAxisCollection Axes { get; }

		/// <summary>
		/// Gets or sets the spacing between different axes on the same side of the ChartArea. This spacing is useful when you display multiple
		/// axes side by side.
		/// </summary>
		SizeF AxisSpacing { get; set; }

		/// <summary>
		/// The primary X axis.
		/// </summary>
		ChartAxis PrimaryXAxis { get; }
		/// <summary>
		/// The primary Y axis.
		/// </summary>
		ChartAxis PrimaryYAxis { get; }

		/// <summary>
		/// Gets or sets the minimum size of this ChartArea.
		/// </summary>
		SizeF MinSize { get; set; }

		/// <summary>
		/// 
		/// </summary>
		float Scale3DCoeficient { get; set; }

		/// <summary>
		/// Returns the margins that will be deduced from the rectangular area that represents the ChartArea.
		/// Negative values are supported.
		/// </summary>
		ChartMargins ChartAreaMargins { get; set; }

		/// <summary>
		/// Returns the center point of this ChartArea.     
		/// </summary>
		PointF Center { get; }
		/// <summary>
		/// Returns the radius of the Radar chart occupied by this ChartArea.
		/// </summary>
		float Radius { get; }

		/// <summary>
		/// Returns the actual rectangular bounds used for rendering.   
		/// </summary>
		Rectangle RenderBounds { get; }

		/// <summary>
		/// Gets or sets the ToolTip text associated with this ChartArea.   
		/// </summary>
		string ChartAreaToolTip { get; set; }

		/// <summary>
		/// Returns the margins of ChartArea (excluding label width and height).
		/// </summary>
		ChartMargins ChartPlotAreaMargins
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the mode of drawing the edge labels.
		/// </summary>
		ChartSetMode AdjustPlotAreaMargins
		{
			get;
			set;
		}
		/// <summary>
		/// 
		/// </summary>
		ChartAxesInfoBar AxesInfoBar { get; }
		/// <internalonly/>
		void Draw(PaintEventArgs e);
		/// <internalonly/>
		void Draw(PaintEventArgs e, ChartPaintFlags flags);
		/// <internalonly/>
		void DrawZoomingRange(Graphics g);
		/// <summary>
		/// Calculates the size of ChartArea.
		/// </summary>
		/// <param name="rect"></param>
		void CalculateSizes(Rectangle rect);
		/// <summary>
		/// Returns the chartpoint value at this real point.
		/// </summary>
		/// <returns></returns>
		ChartPoint GetValueByPoint(Point pt);

		/// <summary>
		/// Gets the real point value at this chart point.
		/// </summary>
		/// <returns></returns>
		Point GetPointByValue(ChartPoint cpt);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		Point CorrectionFrom(Point pt);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		PointF CorrectionFrom(PointF pt);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		Point CorrectionTo(Point pt);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		PointF CorrectionTo(PointF pt);

		/// <summary>
		/// Gets the interactive cursors.
		/// </summary>
		/// <value>The interactive cursors.</value>
		[Browsable(false)]
		ChartAreaCursorCollection InteractiveCursors { get; }

		/// <summary>
		/// Collection of custom points associated with this ChartArea. Custom points can be used to add labels to chart points.    
		/// </summary>
		/// <remarks>
		/// <seealso cref="ChartCustomPointCollection"/>
		/// </remarks>
		ChartCustomPointCollection CustomPoints { get; }

		/// <summary>
		/// Returns the x axis of associated with this chartseries.
		/// </summary>
		/// <param name="ser"></param>
		/// <returns></returns>
		ChartAxis GetXAxis(ChartSeries ser);
		/// <summary>
		/// Returns the y axis associated with this chartseries.
		/// </summary>
		/// <param name="ser"></param>
		/// <returns></returns>
		ChartAxis GetYAxis(ChartSeries ser);

		/// <summary>
		/// Gets the chart regions.
		/// </summary>
		/// <value>The chart regions.</value>
		IList ChartRegions { get;}
		/// <summary>
		/// Gets transformation for real 3d mode.
		/// </summary>
		Transform3D Transform3D { get;}
		/// <summary>
		/// Gets the 3D mode settings.
		/// </summary>
		/// <value>The 3D mode settings.</value>
		Graphics3DState Settings3D { get; }

		/// <summary>
		/// Gets or sets the maximal value of full stracking charts.
		/// </summary>
		/// <value>The maximal value of full stracking charts.</value>
		double FullStackMax { get; set; }

		/// <summary>
		/// Gets the chart.
		/// </summary>
		/// <value>The chart.</value>
		IChartAreaHost Chart { get; }

		/// <summary>
		/// Gets or sets the X axes layout mode.
		/// </summary>
		/// <value>The X axes layout mode.</value>
		ChartAxesLayoutMode XAxesLayoutMode
		{
			get;
			set;
		}
		/// <summary>
		/// Gets or sets the Y axes layout mode.
		/// </summary>
		/// <value>The Y axes layout mode.</value>
		ChartAxesLayoutMode YAxesLayoutMode
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the series rendering parameters.
		/// </summary>
		/// <value>The series parameters.</value>
		ChartSeriesParameters SeriesParameters { get; }
	}
}