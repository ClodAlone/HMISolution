#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Contains all node trasformation methods related with PinPoint position, PintPoint Offset, Size and Bounds.
    /// </summary>
    public interface IUnitIndependent
    {
        /// <summary>
        /// Gets the bounding rectangle.
        /// </summary>
        /// <param name="unit">The measure unit of return value.</param>
        /// <param name="bRelativeToModel">if set to <c>true</c> to get bounds in model coordinates, otherwise - <c>false</c>.</param>
        /// <returns>The <see cref="System.Drawing.RectangleF"/>.</returns>
        RectangleF GetBoundingRectangle(MeasureUnits unit, bool bRelativeToModel);

        /// <summary>
        /// Gets the node pin point.
        /// </summary>
        /// <param name="unit">The measure unit of return value.</param>
        /// <returns>The <see cref="System.Drawing.PointF"/>.</returns>
        PointF GetPinPoint(MeasureUnits unit);

        /// <summary>
        /// Gets the node size.
        /// </summary>
        /// <param name="unit">The measure unit of return value.</param>
        /// <returns>The <see cref="System.Drawing.SizeF"/>.</returns>
        SizeF GetSize(MeasureUnits unit);

        /// <summary>
        /// Gets the node pin point offset.
        /// </summary>
        /// <param name="unit">The measure unit of return value.</param>
        /// <returns>The <see cref="System.Drawing.SizeF"/>.</returns>
        SizeF GetPinPointOffset(MeasureUnits unit);

        /// <summary>
        /// Sets the node pin point.
        /// </summary>
        /// <param name="ptValue">The new value.</param>
        /// <param name="unit">The measure unit of ptValue parameter.</param>
        void SetPinPoint(PointF ptValue, MeasureUnits unit);

        /// <summary>
        /// Sets the node size.
        /// </summary>
        /// <param name="szValue">The new value.</param>
        /// <param name="unit">The measure unit of szValue parameter.</param>
        void SetSize(SizeF szValue, MeasureUnits unit);

        /// <summary>
        /// Sets the node pin point offset.
        /// </summary>
        /// <param name="szValue">The new value.</param>
        /// <param name="unit">The measure unit of szValue parameter.</param>
        void SetPinPointOffset(SizeF szValue, MeasureUnits unit);
    }
}
