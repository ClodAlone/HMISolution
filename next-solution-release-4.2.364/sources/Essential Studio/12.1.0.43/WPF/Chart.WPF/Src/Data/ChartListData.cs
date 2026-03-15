// <copyright file="ChartListData.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;   
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;   

    
    /// <summary>
    /// Represents Chart List data class. The ChartListData observable collection is used to add data points to chart series
    /// </summary>
    /// <seealso cref="ChartListData"/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    [TypeConverter(typeof(ChartListDataConverter))]
    public class ChartListData : ObservableCollection<IChartDataPoint>, IChartData
    {
        #region Members
        /// <summary>
        /// Initializes m_xValueType
        /// </summary>
        private ChartValueType m_xValueType = ChartValueType.Double;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the type of the X value.
        /// </summary>
        /// <value>The type of the X value.</value>
        public ChartValueType XValueType
        {
            get
            {
                return m_xValueType;
            }
        }

        /// <summary>
        /// Gets or Sets the type of the Chart X value
        /// </summary>
        public ChartValueType ChartXValueType
        {
            get;
            set;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the point.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <seealso cref="ChartListData"/>
        public void AddPoint(double x, double y)
        {
            this.Add(new ChartPoint(x, y));
        }

        /// <summary>
        /// Adds the point.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="yValues">The y values.</param>
        /// <seealso cref="ChartListData"/>
        public void AddPoint(double x, params double[] yValues)
        {
            this.Add(new ChartPoint(x, yValues));
        }

        /// <summary>
        /// Adds the point.
        /// </summary>
        /// <param name="xDate">The x date.</param>
        /// <param name="y">The y value.</param>
        /// <seealso cref="ChartListData"/>
        public void AddPoint(DateTime xDate, double y)
        {
            m_xValueType = ChartValueType.DateTime;
            this.Add(new ChartPoint(xDate.ToOADate(), y));
        }

        /// <summary>
        /// Adds the point.
        /// </summary>
        /// <param name="xDate">The x date.</param>
        /// <param name="yValues">The y values.</param>
        /// <seealso cref="ChartListData"/>
        public void AddPoint(DateTime xDate, params double[] yValues)
        {
            m_xValueType = ChartValueType.DateTime;
            this.Add(new ChartPoint(xDate.ToOADate(), yValues));
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {
            if (this.Items != null)
            {
                //    return;
                //}

                for (int i = 0; i < this.Items.Count; i++)
                {
                    this.Items[i] = null;
                }
            }
        }

        #endregion
    }
}
