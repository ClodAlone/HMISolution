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

using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    ///    This interface represents the complete style information for a series in the chart.
    /// </summary>
    public interface IChartSeriesStylesModel
    {
        /// <summary>
        ///     Gets the series style information.
        /// </summary>
        ChartStyleInfo Style { get; }

        /// <summary>
        ///     Returns the style information at the specified index. This is the actual style information and not composed style information.
        /// </summary>
        /// <param name="index" type="int">
        ///     <para>
        ///     The index value of the point for which style information is needed.
        ///     </para>
        /// </param>
        /// <returns>
        ///     Style information at the specified index.
        /// </returns>
        ChartStyleInfo GetStyleAt(int index);

        /// <summary>
        ///     Changes style information at the specified index.
        /// </summary>
        /// <param name="style" type="Syncfusion.Windows.Forms.Chart.ChartStyleInfo">
        ///     <para>
        ///      Style whose attributes are to be stored.
        ///     </para>
        /// </param>
        /// <param name="index" type="int">
        ///     <para>
        ///      Index value where they need to be stored.
        ///     </para>
        /// </param>
        void ChangeStyleAt(ChartStyleInfo style, int index);

        /// <summary>
        ///     Changes series style information.
        /// </summary>
        /// <param name="style" type="Syncfusion.Windows.Forms.Chart.ChartStyleInfo">
        ///     <para>
        ///     Style whose attributes are to be stored in the series style.
        ///     </para>
        /// </param>
        void ChangeStyle(ChartStyleInfo style);

        /// <summary>
        ///     Accesses base style information for the specified style.
        /// </summary>
        /// <param name="chartStyleInfo" type="Syncfusion.Styles.IStyleInfo">
        ///     <para>
        ///      Style for which base style information is needed.
        ///     </para>
        /// </param>
        /// <param name="index" type="int">
        ///     <para>
        ///     Index value where the style is stored.
        ///     </para>
        /// </param>
        ChartStyleInfo[] GetBaseStyles(IStyleInfo chartStyleInfo, int index);

        /// <summary>
        ///     Completely composed styles can be accessed using the interface returned by this property.
        ///     Composed styles have all information initialized from base styles and any other styles along their
        ///     inheritance hierarchy.
        /// </summary>
        IChartSeriesComposedStylesModel ComposedStyles { get; }

        /// <summary>
        /// Event that is raised when style information is changed.
        /// </summary>
        event ChartStyleChangedEventHandler Changed;
    }
}