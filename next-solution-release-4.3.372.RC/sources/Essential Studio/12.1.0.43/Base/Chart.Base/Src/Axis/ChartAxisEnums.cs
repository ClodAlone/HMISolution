#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Specifies the padding that will be applied when calculating the axis range.
    /// </summary>
    public enum ChartAxisRangePaddingType
    {
        /// <summary>
        /// No padding will be applied to the Axis range.
        /// </summary>
        None,

        /// <summary>
        /// Padding will be calculated when the axis range is computed.
        /// </summary>
        Calculate
    }

    /// <summary>
    /// Specifies the location type of the axis.
    /// </summary>
    public enum ChartAxisLocationType
    {
        /// <summary>
        /// Axis will be placed automatically by control to prevent overlapping with labels.
        /// </summary>
        Auto,

        /// <summary>
        /// Axis thickness will be calculated and axis will placed automatically by control to prevent 
        /// labels cutting by sides of control. During this process one coordinate of axis location 
        /// is preserved (x coordinate for horizontal axis and y for vertical axis).
        /// </summary>
        AntiLabelCut,

        /// <summary>
        /// The user will have ability to set axis location manually.
        /// </summary>
        Set
    }

    /// <summary>
    /// Specifies the drawing mode of the labels.
    /// </summary>
    public enum ChartAxisEdgeLabelsDrawingMode
    {
        /// <summary>
        /// Labels at the sides of the axis will be positioned at the center of tick label.
        /// </summary>
        Center,

        /// <summary>
        /// Labels will be aligned with current axis edges.
        /// </summary>
        Shift,

        /// <summary>
        /// Labels will be positioned in way to prevent clipping.
        /// </summary>
        ClippingProtection
    }

    /// <summary>
    /// Specifies the drawing mode of the tick lines.
    /// </summary>
    public enum ChartAxisTickDrawingOperationMode
    {
        /// <summary>
        /// In this mode, the interval always remains unchanged during zooming.
        /// </summary>
        IntervalFixed,

        /// <summary>
        /// In this mode, only number of intervals matters, and it is kept constant 
        /// during zooming. Also there is always full number of intervals on axis.
        /// </summary>
        NumberOfIntervalsFixed
    }

    /// <summary>
    /// Specifies the drawing mode of the grid lines.
    /// </summary>
    public enum ChartAxisGridDrawingMode
    {
        /// <summary>
        /// Grid lines are drawed with equal interval.
        /// </summary>
        Default,

        /// <summary>
        /// Grid lines are drawed at grouping labels margins.
        /// </summary>
        GroupingLabelsPositions
    }

    /// <summary>
    /// Specifies the parameter types of <see cref="IChartAxisLabelModel.GetLabelAt"/> method.
    /// </summary>
    public enum ChartCustomLabelsParameter
    {
        /// <summary>
        /// The parameter of <see cref="IChartAxisLabelModel.GetLabelAt"/> method is index of labels.
        /// </summary>
        Index,

        /// <summary>
        /// The parameter of <see cref="IChartAxisLabelModel.GetLabelAt"/> method is position of labels.
        /// </summary>
        Position
    }
}
