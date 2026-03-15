#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows.Media;

namespace Syncfusion.Windows.Controls.Cells
{
    /// <summary>
    /// NoHitTestDrawingVisual is a visual object that can be used to
    /// render vector graphics on the screen. The content is persisted by the system. 
    /// Use this derived object instead of the base class DrawingVisual when
    /// you do not want the object to participate in hit-testing, as for example
    /// when you want to draw a solid background rectangle over selected cells in a grid 
    /// but still want the hit-test to be passed down to the underlying cells when
    /// the user moves the mouse over the rectangle.
    /// </summary>
    public class NoHitTestDrawingVisual : DrawingVisual
    {
        /// <summary>
        /// Determines whether a geometry value is within the bounds of the visual object.
        /// </summary>
        /// <param name="hitTestParameters">A value of type <see cref="System.Windows.Media.GeometryHitTestParameters"/> that specifies the <see cref="System.Windows.Media.Geometry"/> to hit test against.</param>
        /// <returns>
        /// A value of type <see cref="System.Windows.Media.GeometryHitTestResult"/>.
        /// </returns>
        protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
        {
            return null;
        }

        /// <summary>
        /// Determines whether a point coordinate value is within the bounds of the <see cref="System.Windows.Media.DrawingVisual"/> object.
        /// </summary>
        /// <param name="hitTestParameters">A value of type <see cref="System.Windows.Media.PointHitTestParameters"/> that specifies the <see cref="System.Windows.Point"/> to hit test against.</param>
        /// <returns>
        /// A value of type <see cref="System.Windows.Media.HitTestResult"/>, representing the <see cref="System.Windows.Media.Visual"/> returned from a hit test.
        /// </returns>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return null;
        }

    }
}
