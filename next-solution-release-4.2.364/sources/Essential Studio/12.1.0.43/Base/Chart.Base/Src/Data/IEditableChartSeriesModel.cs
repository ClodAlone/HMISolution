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

namespace Syncfusion.Windows.Forms.Chart
{

	/// <summary>
	/// Interface to be implemented if you want ChartPoint to be able to change your data through code. Additionally, in a future version this interface will
	/// allow the chart to edit data. If you wish to just display data as a series in the chart, please refer to the simpler <see cref="IChartSeriesModel"/>
	/// interface.
	/// </summary>
	public interface IEditableChartSeriesModel : IChartSeriesModel
	{

		/// <summary>
		/// Adds data to the end of the data representation.
		/// </summary>
		/// <param name="x">X value.</param>
		/// <param name="y">Y value.</param>
		void Add( double x, double[] y );

		/// <summary>
		/// Adds data to the end of the data representation.
		/// </summary>
		/// <param name="x">X value.</param>
		/// <param name="y">Y value.</param>
		/// <param name="isEmpty">if set to <c>true</c> the point is empty.</param>
		void Add(double x, double[] y, bool isEmpty);

		/// <summary>
		/// Inserts a value in the data at the specified index.
		/// </summary>
		/// <param name="xIndex">Index value where the insertion is to be made.</param>
		/// <param name="x">The X value.</param>
		/// <param name="yValues">The associated Y values.</param>
		void Insert( int xIndex, double x, double[] yValues );

		/// <summary>
		/// Changes the X value of the data point at the specified index.
		/// </summary>
		/// <param name="xIndex">Index value where the data is to be changed.</param>
		/// <param name="value">New X value.</param>
		void SetX( int xIndex, double value );

		/// <summary>
		/// Changes the Y value of the data point at the specified index.
		/// </summary>
		/// <param name="xIndex">Index value where data is to be changed.</param>
		/// <param name="yValues">New Y values.</param>
		void SetY( int xIndex, double[] yValues );

		/// <summary>
		/// Sets the empty state indicating if the value at the specified point index is to be plotted. If this
		/// value is set to True, then it is treated as not present and is not plotted.
		/// </summary>
		/// <param name="xIndex" type="int">
		/// Index value where the empty state indicator is to be stored.
		/// </param>
		/// <param name="isEmpty" type="bool">
		/// Empty state indicator.
		/// </param>
		void SetEmpty( int xIndex, bool isEmpty );

		/// <summary>
		/// Removes the data point at the specified index.
		/// </summary>
		/// <param name="xIndex">Index value where data is to be removed.</param>
		void Remove( int xIndex );

		/// <summary>
		/// Clears all data points in this datasource.
		/// </summary>
		void Clear();
	}
}