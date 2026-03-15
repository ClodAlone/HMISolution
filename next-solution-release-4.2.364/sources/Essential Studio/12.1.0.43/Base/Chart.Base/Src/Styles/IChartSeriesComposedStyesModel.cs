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
    ///     This interface represents the 'composed' state of styles for a single series. When composed, styles have all their attributes
    ///     initialized from their base styles and any other styles that forms a part of their inheritance structure. Composed styles are used
    ///     by the chart directly.
    /// </summary>
    /// <remarks>
    ///
    /// </remarks>
    public interface IChartSeriesComposedStylesModel
    {
        /// <summary>
        ///  Returns the style object that is common to the series (for which this model holds style information).
        /// </summary>
        ChartStyleInfo Style { get; }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Forms.Chart.ChartStyleInfo"/> at the specified index.
        /// </summary>
        /// <value></value>
        ChartStyleInfo this[int index]
        {
            get; 
        }

        /// <summary>
        ///     Overloaded. Returns an 'offline' version of the series style. Offline styles do not propagate changes made, back to the data store.
        /// </summary>
        ChartStyleInfo GetOfflineStyle();

        /// <summary>
        /// Returns an 'offline' version of the point style. Offline styles do not propagate changes made, back to the data store.
        /// </summary>
        /// <param name="index">Index value of the style.</param>
        /// <returns> Returns ChartStyleInfo object.</returns>
        ChartStyleInfo GetOfflineStyle(int index);

        /// <summary>
        ///     Removes any information that is cached.
        /// </summary>
        void ResetCache();

        /// <summary>
        /// Looks up base style information for any <see cref="ChartStyleInfo"/> object.
        /// <seealso cref="ChartBaseStylesMap"/>
        /// </summary>
        /// <param name="chartStyleInfo">The style object for which base style information is to be retrieved.</param>
        /// <param name="index">The index value of the style.</param>
        /// <returns>Returns ChartStyleInfo array. </returns>
        ChartStyleInfo[] GetBaseStyles(IStyleInfo chartStyleInfo, int index);

        /// <summary>
        ///   Changes the style stored at the specified index to be the same as the specified style. Affects the data store.
        /// </summary>
        /// <param name="style" type="Syncfusion.Windows.Forms.Chart.ChartStyleInfo">
        ///     <para>
        ///     Style object whose information is to be stored.
        ///     </para>
        /// </param>
        /// <param name="index" type="int">
        ///     <para>
        ///     The index value of the style to be changed.
        ///     </para>
        /// </param>
        void ChangeStyle(ChartStyleInfo style, int index);
    }
}