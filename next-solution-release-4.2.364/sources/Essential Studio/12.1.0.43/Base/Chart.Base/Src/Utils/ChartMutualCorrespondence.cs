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
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// ChartIndexedValues collects and sorts the X values of series.
    /// </summary>
    /// <internalonly/>
    [DocumentationExclude()]
    public sealed class ChartIndexedValues
    {
        #region Members
        private ChartModel m_chartModel = null;
        private ArrayList m_indexedValues = new ArrayList();
        private bool m_needUpdate = true;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the count of indexed values.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                this.EnsureValuesUpdated();
                return m_indexedValues.Count;
            }
        }

        /// <summary>
        /// Gets the <see cref="System.Double"/> at the specified index.
        /// </summary>
        /// <value></value>
        public double this[int index]
        {
            get
            {
                this.EnsureValuesUpdated();
                return (double)m_indexedValues[index];
            }
        }
        #endregion

        #region Constrcutor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartIndexedValues"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        internal ChartIndexedValues(ChartModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            m_chartModel = model;
            m_chartModel.Series.Changed += new ChartSeriesCollectionChangedEventHandler(OnSeriesChanged);
        }
        #endregion

        #region Publci methods
        /// <summary>
        /// Gets the indexed value by real value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Returns the Index.</returns>
        public double GetIndex(double value)
        {
            this.EnsureValuesUpdated();

            double result = m_indexedValues.IndexOf(value);
            int count = m_indexedValues.Count;

            if (count > 0)
            {
                if (result < 0)
                {
                    int index1 = -1;
                    int index2 = -1;

                    double[] values = m_indexedValues.ToArray(typeof(double)) as double[];

                    ChartMath.GetTwoClosestPoints(values, value, out index1, out index2);

                    double val1 = values[index1];
                    double val2 = values[index2];

                    if (val2 == val1) val2++;

                    result = index1 + ((value - val1) / (val2 - val1));
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the real value by indexed value.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Returns double value for the given index.</returns>
        public double GetValue(double index)
        {
            this.EnsureValuesUpdated();

            double result = -1;
            int count = m_indexedValues.Count;

            if (count > 0)
            {
                if (index % 1 == 0 && index > -1 && index < m_indexedValues.Count)
                {
                    result = (double)m_indexedValues[(int)index];
                }
                else
                {
                    int index1 = (int)ChartMath.MinMax(ChartMath.Round(index, 1, false), 0, count - 1);
                    int index2 = (int)ChartMath.MinMax(ChartMath.Round(index, 1, true), 0, count - 1);

                    if (index1 == index2 && count > 1)
                    {
                        if (index1 == 0)
                        {
                            index2++;
                        }
                        else
                        {
                            index1--;
                        }
                    }

                    double val1 = (double)m_indexedValues[index1];
                    double val2 = (double)m_indexedValues[index2];

                    if (val2 == val1) val2++;

                    result = val1 + (val2 - val1) * (index - index1);
                }
            }

            return result;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates this values.
        /// </summary>
        private void EnsureValuesUpdated()
        {
            if (m_needUpdate)
            {
                m_indexedValues.Clear();

                if (m_chartModel != null)
                {
                    foreach (ChartSeries series in m_chartModel.Series)
                    {
                        foreach (ChartPoint point in series.Points)
                        {
                            if (m_chartModel.Chart.AllowGapForEmptyPoints)
                            {
                                if (!m_indexedValues.Contains(point.X))
                                {
                                    m_indexedValues.Add(point.X);
                                }
                            }
                            else
                            {
                                if (!m_indexedValues.Contains(point.X) && (!point.IsEmpty))
                                {
                                    m_indexedValues.Add(point.X);
                                }
                            }
                        }
                    }
                }

                m_indexedValues.Sort();
                m_needUpdate = false;
            }
        }

        /// <summary>
        /// Called when series changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Forms.Chart.ChartSeriesCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnSeriesChanged(object sender, ChartSeriesCollectionChangedEventArgs e)
        {
            m_needUpdate = true;
        }
        #endregion
    }
}
