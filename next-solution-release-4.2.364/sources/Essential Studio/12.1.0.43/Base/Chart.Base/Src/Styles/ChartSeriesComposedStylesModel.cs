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
using System.Collections;
using System.Diagnostics;

using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// The ChartSeriesComposedStylesModel class.
    /// </summary>
    internal class ChartSeriesComposedStylesModel : IChartSeriesComposedStylesModel
    {
        #region Constants
        private const int c_seriesIndex = -1;
        #endregion

        #region Members
        private IChartSeriesStylesModel m_chartSeries;
        private Hashtable m_cache = new Hashtable();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSeriesComposedStylesModel"/> class.
        /// </summary>
        /// <param name="chartSeries">The chart series.</param>
        public ChartSeriesComposedStylesModel(IChartSeriesStylesModel chartSeries)
        {
            m_chartSeries = chartSeries;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the style object that is common to the series (for which this model holds style information).
        /// </summary>
        /// <value></value>
        public ChartStyleInfo Style
        {
            get
            {
                return this.GetStyle(c_seriesIndex, false);
            }
        }

        /// <summary>
        /// The ChartStyleInfo indexer.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public ChartStyleInfo this[int index]
        {
            get
            {
                return this.GetStyle(index, false);
            }
        }
        #endregion

        #region Public methdos
        /// <summary>
        /// Overloaded. Returns an 'offline' version of the series style. Offline styles do not propagate changes made, back to the data store.
        /// </summary>
        /// <returns>Returns ChartStyleInfo object.</returns>
        public ChartStyleInfo GetOfflineStyle()
        {
            return this.GetStyle(c_seriesIndex, true);
        }

        /// <summary>
        /// Returns an 'offline' version of the point style. Offline styles do not propagate changes made, back to the data store.
        /// </summary>
        /// <param name="index">Index value of the style.</param>
        /// <returns>Returns ChartStyleInfo object.</returns>
        public ChartStyleInfo GetOfflineStyle(int index)
        {
            return this.GetStyle(index, true);
        }

        /// <summary>
        /// Looks up base style information for any <see cref="ChartStyleInfo"/> object.
        /// <seealso cref="ChartBaseStylesMap"/>
        /// </summary>
        /// <param name="chartStyleInfo">The style object for which base style information is to be retrieved.</param>
        /// <param name="index">The index value of the style.</param>
        /// <returns>Returns ChartStyleInfo array.</returns>
        public ChartStyleInfo[] GetBaseStyles(IStyleInfo chartStyleInfo, int index)
        {
            return m_chartSeries.GetBaseStyles(chartStyleInfo, index);
        }

        /// <summary>
        /// Changes the style stored at the specified index to be the same as the specified style. Affects the data store.
        /// </summary>
        /// <param name="style">Style object whose information is to be stored.</param>
        /// <param name="index">The index value of the style to be changed.</param>
        public void ChangeStyle(ChartStyleInfo style, int index)
        {
            if (index == c_seriesIndex)
            {
                m_chartSeries.ChangeStyle(style);
                m_cache.Clear();
            }
            else
            {
                m_chartSeries.ChangeStyleAt(style, index);
                m_cache.Remove(index);
            }

            style.Store.ResetChangedBits();
        }

        /// <summary>
        /// Changes the style.
        /// </summary>
        /// <param name="style">The style.</param>
        public void ChangeStyle(ChartStyleInfo style)
        {
            this.ChangeStyle(style, c_seriesIndex);
        }

        /// <summary>
        /// Removes any information that is cached.
        /// </summary>
        public void ResetCache()
        {
            m_cache.Clear();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="offLine">if set to <c>true</c> [off line].</param>
        /// <returns>Returns ChartStyleInfo object.</returns>
        private ChartStyleInfo GetStyle(int index, bool offLine)
        {
            if (m_cache.ContainsKey(index))
            {
                WeakReference wr = m_cache[index] as WeakReference;

                if (wr.Target != null)
                {
                    return wr.Target as ChartStyleInfo;
                }
            }

            ChartStyleInfo style = new ChartStyleInfo(new ChartStyleInfoIdentity(this, index, offLine));

            m_cache[index] = new WeakReference(style);

            return style;
        }
        #endregion
    }
}