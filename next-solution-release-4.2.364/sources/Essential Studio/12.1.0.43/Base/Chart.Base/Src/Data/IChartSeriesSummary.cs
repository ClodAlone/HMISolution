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

using System.Diagnostics;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{

	/// <summary>
	/// Contains summary information for implementing a class. In the current version, this
	/// interface is implemented by the <see cref="ChartAxis"/> class.
	/// </summary>
	public interface IChartSeriesSummary
	{
		/// <summary>
		/// Refreshes summary information.
		/// </summary>
		void Refresh();

		/// <summary>
		/// Returns the maximum X value.
		/// </summary>
		/// <value></value>
		double MaxX { get; }
		/// <summary>
		/// Returns the maximum Y value.
		/// </summary>
		/// <value></value>
		double MaxY { get; }
		/// <summary>
		/// Returns the minimum X value.
		/// </summary>
		/// <value></value>
		double MinX { get; }
		/// <summary>
		/// Returns the minimum Y value.
		/// </summary>
		/// <value></value>
		double MinY { get; }

		/// <summary>
		/// Gets the Y percentage.
		/// </summary>
		/// <param name="pointIndex">Index of the point.</param>
		/// <returns></returns>
		/// <remarks>Percentages computes for positive values only.</remarks>
		double GetYPercentage(int pointIndex);
		/// <summary>
		/// Gets the Y percentage.
		/// </summary>
		/// <param name="pointIndex">Index of the point.</param>
		/// <param name="yIndex">Index of the y.</param>
		/// <returns></returns>
		/// <remarks>Percentages computes for positive values only.</remarks>
		double GetYPercentage(int pointIndex, int yIndex);

		/// <summary>
		/// Finds point by specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>Found point or null.</returns>
		ChartPoint FindValue(double value);
		/// <summary>
		/// Finds point by specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="useValue">The use value.</param>
		/// <returns>Found point or null.</returns>
		ChartPoint FindValue(double value, string useValue);
		/// <summary>
		/// Finds point by specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="useValue">Which point value to use (X, Y1, Y2,...).</param>
		/// <param name="index">Index to start looking from. Returns index of found point or -1.</param>
		/// <returns>Found point or null.</returns>
		ChartPoint FindValue(double value, string useValue, ref int index);
		/// <summary>
		/// Finds point by specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="useValue">Which point value to use (X, Y1, Y2,...).</param>
		/// <param name="index">Index to start looking from. Returns index of found point or -1.</param>
		/// <param name="endIndex">The Index where the search is end.</param>
		/// <returns>Found point or null.</returns>
		ChartPoint FindValue(double value, string useValue, ref int index, int endIndex);

		/// <summary>
		/// Finds point with minimal value.
		/// </summary>
		/// <returns>Found point or null.</returns>
		ChartPoint FindMinValue();
		/// <summary>
		/// Finds point with minimal value.
		/// </summary>
		/// <param name="useValue">The use value.</param>
		/// <returns>Found point or null.</returns>
		ChartPoint FindMinValue(string useValue);
		/// <summary>
		/// Finds point with minimal value.
		/// </summary>
		/// <param name="useValue">Which point value to use (X, Y1, Y2,...).</param>
		/// <param name="index">Index to start looking from. Returns index of found point or -1.</param>
		/// <returns>Found point or null.</returns>
		ChartPoint FindMinValue(string useValue, ref int index);
		/// <summary>
		/// Finds point with minimal value.
		/// </summary>
		/// <param name="useValue">Which point value to use (X, Y1, Y2,...).</param>
		/// <param name="index">Index to start looking from. Returns index of found point or -1.</param>
        /// <param name="endIndex">The Index where the search is end..</param>
		/// <returns>Found point or null.</returns>
		ChartPoint FindMinValue(string useValue, ref int index, int endIndex);

		/// <summary>
		/// Finds point with maximal value.
		/// </summary>
		/// <returns>Found point or null.</returns>
		ChartPoint FindMaxValue();
		/// <summary>
		/// Finds point with maximal value.
		/// </summary>
		/// <param name="useValue">The use value.</param>
		/// <returns>Found point or null.</returns>
		ChartPoint FindMaxValue(string useValue);
		/// <summary>
		/// Finds point with maximal value.
		/// </summary>
		/// <param name="useValue">Which point value to use (X, Y1, Y2,...).</param>
		/// <param name="index">Index to start looking from. Returns index of found point or -1.</param>
		/// <returns>Found point or null.</returns>
		ChartPoint FindMaxValue(string useValue, ref int index);
		/// <summary>
		/// Finds point with maximal value.
		/// </summary>
		/// <param name="useValue">Which point value to use (X, Y1, Y2,...).</param>
		/// <param name="index">Index to start looking from. Returns index of found point or -1.</param>
        /// <param name="endIndex">The Index where the search is end.</param>
		/// <returns>Found point or null.</returns>       
		ChartPoint FindMaxValue(string useValue, ref int index, int endIndex);

		/// <internalonly/>
		[Syncfusion.Documentation.DocumentationExclude()]
		IChartSeriesModel ModelImpl { get; set; }
	}
}
