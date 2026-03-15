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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// The ToolTipController that controls tips visibility, position and style.
    /// </summary>
    /// <internalonly/>
    [DocumentationExclude()]
    public sealed class ChartFancyToolTipController : IDisposable
    {
        #region Members
        private IChartAreaHost m_chart = null;
        private Control m_chartHost = null;

        private bool m_canCheck = false;
        private Point m_mousePoint;
        private bool m_canViewToolTip = false;

        private Dictionary<ChartSeries, ChartFancyToolTip> m_toolTips = new Dictionary<ChartSeries, ChartFancyToolTip>();
        #endregion

        #region Events
        /// <summary>
        /// Raised when the text format is changed.
        /// </summary>
        public event ChartTextFormatEventHendler TextFormat;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartFancyToolTipController"/> class.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public ChartFancyToolTipController(IChartAreaHost chart)
            : this(chart, chart as Control)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartFancyToolTipController"/> class.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="host">The host.</param>
        public ChartFancyToolTipController(IChartAreaHost chart, Control host)
        {
            if (chart == null)
                throw new ArgumentNullException("chart");
            if (host == null)
                throw new ArgumentNullException("host");

            m_chart = chart;
            m_chartHost = host;

            m_chartHost.Invalidated += new InvalidateEventHandler(OnChartHostInvalidated);
            m_chartHost.Paint += new PaintEventHandler(OnChartHostPaint);
            m_chartHost.MouseMove += new MouseEventHandler(OnMouseMove);
            m_chartHost.MouseLeave += new EventHandler(OnMouseLeave);

            m_chart.Series.Changed += new ChartSeriesCollectionChangedEventHandler(OnSeriesListChanged);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_chartHost != null)
            {
                m_chartHost.Invalidated -= new InvalidateEventHandler(OnChartHostInvalidated);
                m_chartHost.Paint -= new PaintEventHandler(OnChartHostPaint);
                m_chartHost.MouseMove -= new MouseEventHandler(OnMouseMove);
                m_chartHost.MouseLeave -= new EventHandler(OnMouseLeave);
                m_chartHost = null;
            }

            if (m_chart != null)
            {
                m_chart.Series.Changed -= new ChartSeriesCollectionChangedEventHandler(OnSeriesListChanged);
                m_chart = null;
            }

            foreach (IDisposable cftt in m_toolTips.Values)
            {
                cftt.Dispose();
            }

            GC.SuppressFinalize(this);
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Views the tool tips.
        /// </summary>
        private void RefreshToolTips(Point pt)
        {
            if (m_chart.GetChartArea().RequireAxes)
            {
                bool canViewToolTip = m_canViewToolTip;
                double xValue = m_chart.GetChartArea().GetValueByPoint(pt).X;
                ChartRegion rgn = m_chart.ChartRegions.HitTest(pt);
                if (rgn != null)
                {
                    int pointIndex = rgn.PointIndex;
                    int seriesIndex = rgn.SeriesIndex;

                    if (pointIndex != -1 && seriesIndex != -1)
                    {
                        ChartSeries series = m_chart.Series[seriesIndex];
                        foreach (ChartSeries ser in m_chart.Series)
                        {
                            if (ser != series)
                            {
                                ChartFancyToolTip toolTip1 = this.GetFancyToolTip(ser);
                                if (toolTip1 != null)
                                    toolTip1.Hide();
                            }
                        }
                        ChartFancyToolTip toolTip = this.GetFancyToolTip(series);
                        if (toolTip != null)
                        {
                            if (canViewToolTip && series.Visible && series.FancyToolTip.Visible)
                            {
                                PointF target = series.Renderer.GetCharacterPoint(pointIndex);

                                if (m_chart.GetChartArea().Bounds.Contains(Point.Round(target)))
                                {
                                    toolTip.Text = this.GetTextForToolTip(series, pointIndex);
                                    toolTip.SymbolColor = series.GetOfflineStyle().Interior.BackColor;
                                    toolTip.Show(target);

                                }
                                else
                                {
                                    toolTip.Hide();
                                }
                            }
                            else
                            {
                                toolTip.Hide();
                            }
                        }
                    }
                    else
                    {
                        foreach (ChartSeries series in m_chart.Series)
                        {
                            ChartFancyToolTip toolTip = this.GetFancyToolTip(series);
                            if (toolTip != null)
                                toolTip.Hide();
                        }
                    }
                }
                

            }
            else
            {
                foreach (ChartFancyToolTip toolTip in m_toolTips.Values)
                {
                    toolTip.Hide();
                }
            }
        }

        /// <summary>
        /// Gets the nearly point by X.
        /// </summary>
        /// <param name="x">The x-coord.</param>
        /// <param name="series">The series.</param>
        /// <returns>Nearest point to x-coord in given series</returns>
        private int GetNearlyPointByX(double x, ChartSeries series)
        {
            int res = -1;
            double delta = double.MaxValue;

            for (int i = 0, end = series.Points.Count; i < end; i++)
            {
                double nDelta = Math.Abs(series.Points[i].X - x);

                if (nDelta < delta)
                {
                    delta = nDelta;
                    res = i;
                }
            }

            return res;
        }

        /// <summary>
        /// Gets the text for display on tool tip.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="ptIndex">The point to get tool tip.</param>
        /// <returns>Returns text for tooltip.</returns>
        private string GetTextForToolTip(ChartSeries series, int ptIndex)
        {
            ChartPoint pt = series.Points[ptIndex];
            string result = series.Name + " [ " + pt.X + ", " + pt.YValues[0] + " ]";

            if (this.TextFormat != null)
            {
                ChartTextFormatEventArgs args = new ChartTextFormatEventArgs(series, pt);

                args.Text = result;

                this.TextFormat(this, args);

                if (args.Handled)
                {
                    result = args.Text;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the fancy tool tip.
        /// </summary>
        /// <param name="series">The series to get tool tip.</param>
        /// <returns>Tool tip assosiated with series.</returns>
        private ChartFancyToolTip GetFancyToolTip(ChartSeries series)
        {
            if (m_toolTips.ContainsKey(series))
            {
                return m_toolTips[series];
            }
            else if (series.Visible && series.FancyToolTip.Visible)
            {
                ChartFancyToolTip result = new ChartFancyToolTip(series.FancyToolTip);
                result.Init(m_chartHost, this);
                m_toolTips.Add(series, result);

                return result;
            }

            return null;
        }
        #endregion

        #region Event hendlers
        /// <summary>
        /// Called when mouse is moved.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        internal void OnMouseMove(object sender, MouseEventArgs e)
        {
            m_mousePoint = new Point(e.X, e.Y);

            ChartRegion rgn = m_chart.ChartRegions.HitTest(m_mousePoint);

            m_canViewToolTip = rgn != null && rgn.IsChartPoint;

            if (m_canCheck)
            {
                this.RefreshToolTips(m_mousePoint);
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the Parent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnMouseLeave(object sender, EventArgs e)
        {
            if (m_canCheck)
            {
                this.RefreshToolTips(m_mousePoint);
            }
        }

        /// <summary>
        /// Handles the Changed event of the Series control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Forms.Chart.ChartSeriesCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnSeriesListChanged(object sender, ChartSeriesCollectionChangedEventArgs e)
        {
            if (e.ChangeType != ChartSeriesCollectionChangeType.Added)
            {
                List<ChartSeries> removedSeries = new List<ChartSeries>();

                foreach (ChartSeries series in m_toolTips.Keys)
                {
                    if (m_chart.Model.Series.Contains(series))
                    {
                        removedSeries.Add(series);
                    }
                }

                foreach (ChartSeries series in removedSeries)
                {
                    ChartFancyToolTip toolTip = m_toolTips[series];
                    m_toolTips.Remove(series);

                    if (toolTip != null)
                    {
                        (toolTip as IDisposable).Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Invalidated event of the Parent control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.InvalidateEventArgs"/> instance containing the event data.</param>
        private void OnChartHostInvalidated(object sender, InvalidateEventArgs e)
        {
            m_canCheck = false;
        }

        /// <summary>
        /// Called when chart host is painting.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        private void OnChartHostPaint(object sender, PaintEventArgs e)
        {
            m_canCheck = true;
        }
        #endregion
    }

    /// <summary>
    /// Represents the method that handles the <see cref="ChartFancyToolTipController.TextFormat"/> event.
    /// </summary>
    /// <internalonly/>
    [DocumentationExclude()]
    public delegate void ChartTextFormatEventHendler(object sender, ChartTextFormatEventArgs e);

    /// <summary>
    /// Represents arguments of <see cref="ChartTextFormatEventHendler"/> handler.
    /// </summary>
    /// <internalonly/>
    [DocumentationExclude()]
    public sealed class ChartTextFormatEventArgs : HandledEventArgs
    {
        #region Members
        private ChartSeries m_series;
        private ChartPoint m_point;
        private string m_text;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the series.
        /// </summary>
        /// <value>The series.</value>
        public ChartSeries Series
        {
            get
            {
                return m_series;
            }
        }

        /// <summary>
        /// Gets the point.
        /// </summary>
        /// <value>The point.</value>
        public ChartPoint Point
        {
            get
            {
                return m_point;
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return m_text;
            }
            set
            {
                m_text = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartTextFormatEventArgs"/> class.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="point">The point.</param>
        public ChartTextFormatEventArgs(ChartSeries series, ChartPoint point)
            : base(true)
        {
            m_series = series;
            m_point = point;
        }
        #endregion
    }
}

