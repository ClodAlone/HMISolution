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

namespace Syncfusion.XlsIO
{
    public interface IChartManualLayout
        : IParentApplication
    {
        /// <summary>
        /// Specifies whether to layout only the the plot area
        /// </summary>
        LayoutTargets LayoutTarget { get; set; }
        /// <summary>
        /// Specifies how to interpret the Left element for this manual layout
        /// </summary>
        LayoutModes LeftMode { get; set; }
        /// <summary>
        /// Specifies how to interpret the Top element for this manual layout.
        /// </summary>
        LayoutModes TopMode { get; set; }
        /// <summary>
        /// Specifies the x location (left) of the chart element as a fraction of the width of the chart. 
        /// If Left Mode is Factor, then the position is relative to the default position for the chart element.
        /// </summary>
        double Left { get; set; }
        /// <summary>
        /// Specifies the top of the chart element as a fraction of the height of the chart. 
        /// If Top Mode is Factor, then the position is relative to the default position for the chart element.
        /// </summary>
        double Top { get; set; }
        /// <summary>
        /// Specifies how to interpret the Width element for this manual layout.
        /// </summary>
        LayoutModes WidthMode { get; set; }
        /// <summary>
        /// Specifies how to interpret the Height element for this manual layout.
        /// </summary>
        LayoutModes HeightMode { get; set; }
        /// <summary>
        /// Specifies the width (if Width Mode is Factor) or right (if Width Mode is Edge) of the chart element
        /// as a fraction of the width of the chart.
        /// </summary>
        double Width { get; set; }
        /// <summary>
        /// Specifies the height (if Height Mode is Factor) or bottom (if Height Mode is edge) of the chart
        /// element as a fraction of the height of the chart.
        /// </summary>
        double Height { get; set; }
    }
}
