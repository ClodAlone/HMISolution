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
using System.ComponentModel;
using System.Diagnostics;

namespace Syncfusion.Windows.Forms.Chart
{

	/// <summary>
	/// Interface that is to be implemented if you want ChartControl to be able to display your data. The default series store is a implementation of IChartSeriesModel. When you implement this interface,
	/// you can set it as the data underlying any <see cref="ChartSeries"/> object using the <see cref="ChartSeries.SeriesModelImpl"/>
	/// property.
	/// </summary>
	public interface IChartSeriesModel
	{
		/// <summary>
		/// Returns the number of points in this series.
		/// </summary>
		int Count { get; }
		/// <summary>
		/// Returns the X value of the series at the specified point index.
		/// </summary>
		/// <param name="xIndex">The index value of the point.</param>
		/// <returns>X value.</returns>
		double GetX(int xIndex);
		/// <summary>
		/// Returns the Y value of the series at the specified point index.
		/// </summary>
		/// <param name="xIndex">The index value of the point.</param>
		/// <returns>Y value.</returns>
		double[] GetY(int xIndex);
		/// <summary>
		/// Indicates whether a plottable value is present at the specified point index.
		/// </summary>
		/// <param name="xIndex" type="int">
		/// The index value of the point.
		/// </param>
		/// <returns>
		///     True, if there is a value present at this point index; false otherwise.
		/// </returns>
		bool GetEmpty(int xIndex);
		/// <summary>
		/// Event that should be raised by any implementation of this interface if data that it holds changes. This will cause the
		/// chart to be updated accordingly.
		/// </summary>
		event ListChangedEventHandler Changed;
	}

	/// <summary>
	/// Interface that is to be implemented if you want the ChartControl to be able to display your indexed data (X value is not needed). The ChartControl is totally agnostic
	/// about the data it displays. Even the default series store is an implementation of <see cref="IChartSeriesModel"/>. When you implement this interface,
	/// you can set it as the data underlying any <see cref="ChartSeries"/> object using the <see cref="ChartSeries.SeriesIndexedModelImpl"/>. When you
	/// use this model for a series, you have to set ChartControl's Indexed property to be True.
	///
	/// </summary>
	public interface IChartSeriesIndexedModel
	{
		/// <summary>
		/// Returns the number of points in this series.
		/// </summary>
		int Count { get; }
		/// <summary>
		/// Returns the Y value of the series at the specified point index. Indexed series do not have an X value.
		/// </summary>
		/// <param name="xIndex">The index value of the point.</param>
		/// <returns>Y value.</returns>
		double[] GetY(int xIndex);
		/// <summary>
		/// Indicates whether a plottable value is present at the specified point index.
		/// </summary>
		/// <param name="xIndex" type="int">
		/// The index value of the point.
		/// </param>
		/// <returns>
		///     True, if there is a value present at the specified point index; false otherwise.
		/// </returns>
		bool GetEmpty(int xIndex);
		/// <summary>
		/// Event that should be raised by any implementation of this interface if data that it holds changes. This will cause the
		/// chart to be updated accordingly.
		/// </summary>
		event ListChangedEventHandler Changed;
	}
}