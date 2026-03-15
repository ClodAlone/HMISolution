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
    /// Indexer helper class to access individual point styles.
    /// </summary>
    public class ChartStyleInfoIndexer
    {
        #region Members
        private IChartSeriesStylesModel m_chartSeries;
        #endregion

        #region Properties
        /// <summary>
        /// Returns the ChartStyleInfo object at the specified index.
        /// </summary>
        public ChartStyleInfo this[int index]
        {
            get
            {
                return m_chartSeries.ComposedStyles[index];
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartStyleInfoIndexer"/> class.
        /// </summary>
        /// <param name="chartSeries">The chart series.</param>
        internal ChartStyleInfoIndexer(IChartSeriesStylesModel chartSeries)
        {
            m_chartSeries = chartSeries;
        }
        #endregion
    }
}