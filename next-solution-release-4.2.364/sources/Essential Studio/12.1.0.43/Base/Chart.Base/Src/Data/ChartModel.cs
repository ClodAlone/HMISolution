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
using System.Diagnostics;
using System.ComponentModel;
using System.Collections;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// The ChartModel object acts as the central repository of data associated with and displayed by a ChartControl.
    /// There are three parts to the ChartModel. The first part is the <see cref="ChartSeriesCollection"/> instance that is held in the model.
    /// This collection holds all the <see cref="ChartSeries"/> instances that are registered with and displayed by the Chart. The Chart Model also
    /// holds a <see cref="ChartBaseStylesMap"/> instance. This collection maintains a collection of base styles that are registered with it.
    /// These base styles can be accessed and changed using this collection. Any changes made to base styles will automatically affect all style
    /// objects that depend on these base styles. Also, in the model is a <see cref="ChartColorModel"/> instance that provides access to several default
    /// color palettes for use by the ChartControl.
    /// </summary>
    public class ChartModel
    {
        #region Members
        private ChartBaseStylesMap m_baseStylesMap = new ChartBaseStylesMap();
        private ChartColorModel m_colorModel = new ChartColorModel();
        private ChartSeriesCollection m_series;
        private ChartIndexedValues m_intexed;
        
        private IChartAreaHost m_chart = null;
        private double m_minPointsDelta = double.NaN;
        private ChartSeries m_firstSeries = null;
        #endregion

        #region Proeprties
        /// <summary>
        /// Returns the <see cref="ChartColorModel"/> associated with this model.
        /// </summary>
        /// <value>The color model.</value>
        public ChartColorModel ColorModel
        {
            get
            {
                return m_colorModel;
            }
        }

        /// <summary>
        /// Collection of <see cref="ChartSeries"/> objects. Each series represents an underlying <see cref="IChartSeriesModel"/>.
        /// </summary>
        /// <value>The series.</value>
        public ChartSeriesCollection Series
        {
            get
            {
                return m_series;
            }
        }
        /// <summary>
        /// Gets the indexed values.
        /// </summary>
        /// <value>The indexed values.</value>
        public ChartIndexedValues IndexedValues
        {
            get { return m_intexed; }
        }

        /// <summary>
        /// Gets the chart.
        /// </summary>
        /// <value>The chart.</value>
        internal IChartAreaHost Chart
        {
            get
            {
                return m_chart;
            }
        }

        /// <summary>
        /// Gets the first series.
        /// </summary>
        /// <value>The first series.</value>
        internal ChartSeries FirstSeries
        {
            get
            {
                return m_firstSeries;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartModel"/> class.
        /// </summary>
        public ChartModel()
        {
            m_series = new ChartSeriesCollection(this);
            m_intexed = new ChartIndexedValues(this);
            m_series.Changed += new ChartSeriesCollectionChangedEventHandler(OnSeriesChanged);

        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the chart.
        /// </summary>
        /// <param name="chartHost">The chart host.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetChart(IChartAreaHost chartHost)
        {
            m_chart = chartHost;
        }

        /// <summary>
        /// Checks the series compatibility.
        /// </summary>
        /// <param name="chartArea">The chart area.</param>
        /// <param name="invertedSeriesIsCompatible">If set to <c>true</c> inverted series is compatible.</param>
        /// <returns>Returns whether inverted series is compatible or not.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool CheckSeriesCompatibility(ChartArea chartArea, bool invertedSeriesIsCompatible)
        {
            bool compatible = true;

            if (m_series.Count > 0)
            {
                bool radar = false;
                bool polar = false;

                m_firstSeries = null;

                foreach (ChartSeries series in m_series)
                {
                    if (m_firstSeries == null)
                    {
                        if (series.Visible)
                        {
                            m_firstSeries = series;

                            chartArea.RequireAxes = m_firstSeries.RequireAxes;
                            chartArea.RequireInvertedAxes = m_firstSeries.RequireInvertedAxes;
                            radar = (m_firstSeries.Type == ChartSeriesType.Radar);
                            polar = (m_firstSeries.Type == ChartSeriesType.Polar);
                            series.Compatible = true;
                        }
                    }
                    else
                    {
                        bool bNaAndIa = invertedSeriesIsCompatible ?
                            !(series.RequireAxes && series.RequireInvertedAxes) :
                            (series.RequireAxes != chartArea.RequireAxes || series.RequireInvertedAxes != chartArea.RequireInvertedAxes);

                        if (!((radar && (series.Type == ChartSeriesType.Radar))
                            || (polar && (series.Type == ChartSeriesType.Polar))) && (bNaAndIa))
                        {
                            compatible = false;
                            series.Compatible = false;
                        }
                        else
                        {
                            series.Compatible = true;
                        }
                    }
                }
            }

            return compatible;
        }

        /// <summary>
        /// Updates the Line series while button click and refresh the model.
        /// </summary>
        /// <param name="area">The area.</param>
        public void Refresh(ChartArea area)
        {
            foreach (ChartSeries series in this.Series)
            {
                if (ChartSeriesType.Line == series.Type)
                {
                    series.UpdateRenderer(ChartUpdateFlags.All);
                }
            }
            this.UpdateArea(area);
        }

        /// <summary>
        /// Updates the axes of chart area.
        /// </summary>
        /// <param name="area">The area.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UpdateArea(ChartArea area)
        {
            if (area.AxesType != ChartAreaAxesType.None)
            {
                foreach (ChartAxis axis in area.Axes)
                {
                    if (axis.RangeType == ChartAxisRangeType.Auto || axis.IsIndexed)
                    {
                        DoubleRange baseRange = DoubleRange.Empty;
                        ChartAxisRangePaddingType rangePadding = axis.RangePaddingType;

                        foreach (ChartSeries series in this.Series)
                        {
                            if (series.ActualXAxis == axis)
                            {
                                baseRange += series.Renderer.GetXDataMeasure();
                            }
                            else if (series.ActualYAxis == axis)
                            {
                                if (series.BaseStackingType == ChartSeriesBaseStackingType.FullStacked)
                                {
                                    rangePadding = ChartAxisRangePaddingType.None;
                                }

                                baseRange += series.Renderer.GetYDataMeasure();
                            }
                        }

                        if (baseRange.IsEmpty)
                        {
                            baseRange = new DoubleRange(0, 1);
                        }

                        axis.SetNiceRange(baseRange, rangePadding);
                    }

                    if (axis.BreakRanges.BreaksMode == ChartBreaksMode.Auto)
                    {
                        axis.BreakRanges.Compute(this.Series);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the <see cref="ChartBaseStylesMap"/> associated with this model.
        /// </summary>
        /// <returns>Return ChartBaseStylesMap.</returns>
        public ChartBaseStylesMap GetStylesMap()
        {
            return m_baseStylesMap;
        }

        #region Obselete methods
        /// <summary>
        /// Overloaded. Factory method for the creation of new series.
        /// </summary>
        /// <returns>Returns ChartSeries.</returns>
        [Obsolete("Use ChartSeries constructor.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ChartSeries NewSeries()
        {
            return new ChartSeries();
        }

        /// <summary>
        /// Factory method for the creation of new series.
        /// </summary>
        /// <param name="name">Unique name for the new series that is to be created.</param>
        /// <returns><see cref="ChartSeries"/>Returns ChartSeries</returns>
        [Obsolete("Use ChartSeries constructor.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ChartSeries NewSeries(string name)
        {
            return new ChartSeries(name);
        }

        /// <summary>
        /// Factory method for the creation of new series.
        /// </summary>
        /// <param name="name">Unique name for the new series that is to be created.</param>
        /// <param name="type">The type of the series that is to be created.</param>
        /// <returns><see cref="ChartSeries"/>Returns ChartSeries.</returns>
        [Obsolete("Use ChartSeries constructor.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ChartSeries NewSeries(string name, ChartSeriesType type)
        {
            return new ChartSeries(name, type);
        }
        #endregion

        #endregion

        #region Implementation
        /// <summary>
        /// Called when series is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Forms.Chart.ChartSeriesCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnSeriesChanged(object sender, ChartSeriesCollectionChangedEventArgs e)
        {
            m_minPointsDelta = double.NaN;
            m_firstSeries = null;
        }

        /// <summary>
        /// This method is used when series are rendered stacked. The value returned is a cumulative value of
        /// Y from all series that are below the series passed in in the contained <see cref="ChartSeriesCollection"/>.
        /// <seealso cref="ChartSeriesRenderer"/>
        /// </summary>
        /// <param name="chartArea">The <see cref="ChartArea"/>.</param>
        /// <param name="series">Instance of the ChartSeries.</param>
        /// <param name="pointIndex">The index value of the point.</param>
        /// <param name="isWithMe">If true the value form this series added too.</param>
        /// <returns>
        /// A sum of Y values from all series are below the series.
        /// </returns>
        internal double GetStackInfo(IChartArea chartArea, ChartSeries series, int pointIndex, bool isWithMe)
        {
            double y = isWithMe ? series.Points[pointIndex].YValues[series.PointFormats.YIndex] : 0;
            double x = series.Points[pointIndex].X;
            double all = series.ActualYAxis.CurrentOrigin;

            bool isFullStacking = series.BaseStackingType == ChartSeriesBaseStackingType.FullStacked;
            bool isEnd = false;

            if (series.BaseStackingType != ChartSeriesBaseStackingType.NotStacked)
            {
                for (int i = 0, c = m_series.VisibleCount; (i < c) && (!isEnd || isFullStacking); i++)
                {
                    ChartSeries curr = m_series.VisibleList[i] as ChartSeries;
                    isEnd = (!isEnd) ? (curr == series) : isEnd;
                    if ((curr.Type == series.Type) && (curr.StackingGroup == series.StackingGroup) && (curr.BaseStackingType == series.BaseStackingType))
                    {
                        for (int j = 0; j < curr.Points.Count; j++)
                        {
                            if ((!curr.Points[j].IsEmpty) && (curr.Points[j].X == x))
                            {
                                if (!isFullStacking || (series.Type == ChartSeriesType.StackingArea100))
                                {
                                    if (!isEnd)
                                    {
                                        if (isFullStacking || !(series.Type == ChartSeriesType.StackingBar || series.Type == ChartSeriesType.StackingColumn))
                                        {
                                            y += curr.Points[j].YValues[series.PointFormats.YIndex];
                                        }
                                        else if (((y <= 0) && (curr.Points[j].YValues[series.PointFormats.YIndex] <= 0)
                                            && series.Points[pointIndex].YValues[series.PointFormats.YIndex] < 0) ||
                                            ((y >= 0) && (curr.Points[j].YValues[series.PointFormats.YIndex] >= 0) &&
                                             series.Points[pointIndex].YValues[series.PointFormats.YIndex] > 0))
                                        {
                                            y += curr.Points[j].YValues[series.PointFormats.YIndex];
                                        }
                                    }
                                    if (isFullStacking)
                                        all += Math.Abs(curr.Points[j].YValues[series.PointFormats.YIndex]);
                                    else
                                        all += curr.Points[j].YValues[series.PointFormats.YIndex];
                                    break;
                                }
                                else
                                {
                                    if (!isEnd && ((curr.Points[j].YValues[series.PointFormats.YIndex] > 0 && y >= 0 && series.Points[pointIndex].YValues[series.PointFormats.YIndex] > 0) || (curr.Points[j].YValues[series.PointFormats.YIndex] < 0 && y <= 0 && series.Points[pointIndex].YValues[series.PointFormats.YIndex] < 0)))
                                        y += curr.Points[j].YValues[series.PointFormats.YIndex];

                                    all += Math.Abs(curr.Points[j].YValues[series.PointFormats.YIndex]);
                                    break;
                                }
                            }
                        }
                    }
                }

                if (isFullStacking)
                {
                    y = all == 0 ? 0 : chartArea.FullStackMax * y / all;
                }
            }

            return y;
        }

        /// <summary>
        /// Gets the side by side info.
        /// </summary>
        /// <param name="chartArea">The chart area.</param>
        /// <param name="series">The series.</param>
        /// <returns>Returns the DoubleRange value.</returns>
        internal DoubleRange GetSideBySideInfo(IChartArea chartArea, ChartSeries series)
        {
            int pos = 1;
            int all = 1;

            if (m_chart.ColumnDrawMode != ChartColumnDrawMode.ClusteredMode)
            {
                this.GetSideBySidePositions(series, out all, out pos);
            }

            double seriesSpacing = chartArea.SeriesParameters.SeriesSpacing;
            double pointSpacing = chartArea.SeriesParameters.PointSpacing - seriesSpacing;

            double width = this.GetMinPointsDelta(chartArea) * (1 - pointSpacing);
            double div = 1d / all;
            double loc = 0.5 - div * (pos - 1);

            DoubleRange res = series.ActualXAxis.Inversed ?
                new DoubleRange(loc - div, loc) : new DoubleRange(-loc, -loc + div);

            return DoubleRange.Scale(DoubleRange.Multiply(res, width), 1 - seriesSpacing);
        }

        /// <summary>
        /// Gets the side by side info.
        /// </summary>
        /// <param name="chartArea">The chart area.</param>
        /// <param name="series">The series.</param>
        /// <param name="seriesWidth">The seriesWidth.</param>
        /// <returns>Returns the DoubleRange.</returns>
        internal DoubleRange GetSideBySideInfo(IChartArea chartArea, ChartSeries series, double seriesWidth)
        {
            int pos = 1;
            int all = 1;

            if (m_chart.ColumnDrawMode != ChartColumnDrawMode.ClusteredMode)
            {
                this.GetSideBySidePositions(series, out all, out pos);
            }

            double width = seriesWidth * all;
            double div = width / all;
            double loc = 0.5 * width - div * (pos - 1);

            return new DoubleRange(-loc, div - loc);
        }

        /// <summary>
        /// Gets the minimal points delta.
        /// </summary>
        /// <param name="chartArea">The chart area.</param>
        /// <returns></returns>
        /// <value>The min points delta.</value>
        internal double GetMinPointsDelta(IChartArea chartArea)
        {
            if (double.IsNaN(m_minPointsDelta))
            {
                m_minPointsDelta = double.MaxValue;

                if (chartArea.Chart.Indexed)
                {
                    m_minPointsDelta = 1;
                }
                else
                {
                    foreach (ChartSeries series in m_series)
                    {
                        if (series.Visible)
                        {
                            double[] xValues = new double[series.Points.Count];

                            for (int i = 0; i < series.Points.Count; i++)
                            {
                                xValues[i] = series.Points[i].X;
                            }

                            Array.Sort(xValues);

                            for (int i = 1; i < xValues.Length; i++)
                            {
                                double delta = xValues[i] - xValues[i - 1];

                                if (delta != 0)
                                {
                                    m_minPointsDelta = Math.Min(m_minPointsDelta, delta);
                                }
                            }
                            if ((xValues.Length == 1) && (series.XAxis.Range.Interval < 1) &&( series.XAxis.RangeType == ChartAxisRangeType.Set))
                            {
                                double delta = series.XAxis.Range.Interval;
                                if (delta != 0)
                                {
                                    m_minPointsDelta = Math.Min(m_minPointsDelta, delta);
                                }
                            }
                        }
                    }
                }

                if (m_minPointsDelta == double.MaxValue)
                {
                    m_minPointsDelta = 1;
                }
            }

            return m_minPointsDelta;
        }
        

        /// <summary>
        /// Returns the value of side by side displacement.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="all">A sum of all sides.</param>
        /// <param name="pos">Position of side of a series.</param>
        private void GetSideBySidePositions(ChartSeries current, out int all, out int pos)
        {
            pos = -1;
            all = 0;

            Hashtable stackedSeriesTypes = new Hashtable(5);
            ArrayList stakckedList = new ArrayList();
            if (current.BaseType == ChartSeriesBaseType.SideBySide)
            {
                foreach (ChartSeries series in m_series.VisibleList)
                {
                    if (series.BaseType == ChartSeriesBaseType.SideBySide)
                    {
                        if (series.BaseStackingType == ChartSeriesBaseStackingType.NotStacked)
                        {
                            all++;

                            if (series == current)
                            {
                                pos = all;
                            }
                        }
                        else
                        {
                                bool isfound = false;
                                foreach (StackInfo info in stakckedList)
                                {
                                    if ((info.Type == series.Type) && (info.GroupName == series.StackingGroup))
                                    {
                                        if (series == current)
                                        {
                                            pos = info.All;
                                           
                                        }
                                        isfound = true;
                                    }
                                }
                                if(!isfound)
                                {
                                        all++;
                                        StackInfo newInfo = new StackInfo(series.Type, all, series.StackingGroup);
                                        stakckedList.Add(newInfo);
                                        if (series == current)
                                        {
                                            pos = all;
                                        }
                                        
                                    
                                }
                          
                        }
                    }
                }
            }

            if (all < 1)
            {
                all = 1;
                pos = 1;
            }
        }
        #endregion
    }
    internal class StackInfo
    {
        public ChartSeriesType Type;
        public int All;
        public string GroupName;

        public StackInfo(ChartSeriesType type, int all, string groupname)
        {
            this.Type = type;
            this.All = all;
            this.GroupName = groupname;
        }
    }

}