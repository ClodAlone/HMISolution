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
    /// This event is raised by the <see cref="ChartSeries"/> class when series class has changed.
    /// </summary>
    /// <param name="source" type="object">
    ///     <para>
    ///     Event source.
    ///     </para>
    /// </param>
    /// <param name="args" type="Syncfusion.Windows.Forms.Chart.ChartDataChangedEventArgs">
    ///     <para>
    ///     Event arguments.
    ///     </para>
    /// </param>
    public delegate void ChartSeriesChangedEventHandler(object source, ChartSeriesChangedEventArgs args);

    /// <summary>
    /// This interface represents the minimum Y value, maximum Y value and the X value at any point in a series.
    /// This interface is used to compute summary information such as overall series minimum and maximum values for
    /// rendering the chart. In most cases, you have to simply loop through the Y values at an index and return the minimum
    /// and maximum values for that point.
    /// </summary>
    internal interface IChartPointMinMax
    {
        /// <summary>
        /// Gets the X.
        /// </summary>
        /// <value>The X.</value>
        double X { get; }

        /// <summary>
        /// Gets the min.
        /// </summary>
        /// <value>The min.</value>
        double Min { get; }

        /// <summary>
        /// Gets the max.
        /// </summary>
        /// <value>The max.</value>
        double Max { get; }

        /// <summary>
        /// Gets the Y.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="yIndex">Index of the y.</param>
        /// <returns>Returns the Y value.</returns>
        double GetY(int index, int yIndex);
    }
}