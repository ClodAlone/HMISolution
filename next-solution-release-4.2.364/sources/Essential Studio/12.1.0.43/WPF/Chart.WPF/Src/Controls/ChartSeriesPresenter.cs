// <copyright file="ChartSeriesPresenter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using Syncfusion.Windows.Shared;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation;

    /// <summary>
    /// Represents ChartSeriesPresenter
    /// </summary>
    /// <remarks>Class instance is created automatically by WPF Chart building system.</remarks>
    /// <seealso cref="ChartSeriesPresenter"/>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartSeriesPresenter : FrameworkElement, IDisposable
    {
        private class ChartSeriesPresenterAutomationPeer : FrameworkElementAutomationPeer
        {
            public ChartSeriesPresenterAutomationPeer(ChartSeriesPresenter control)
                : base(control)
            {
            }

            protected override string GetClassNameCore()
            {
                return "ChartSeriesPresenter";
            }
            protected override string GetAutomationIdCore()
            {
                return this.MyOwner.Series.Name;
            }
            protected override AutomationControlType GetAutomationControlTypeCore()
            {
                return AutomationControlType.Custom;
            }

            public override object GetPattern(PatternInterface patternInterface)
            {
                return this;
            }


            private ChartSeriesPresenter MyOwner
            {
                get
                {
                    return (ChartSeriesPresenter)base.Owner;
                }
            }
        }

        /// <summary>
        /// Returns class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementations for the Windows Presentation Foundation (WPF) infrastructure.
        /// </summary>
        /// <returns>
        /// The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementation.
        /// </returns>
        protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer()
        {
            return new ChartSeriesPresenterAutomationPeer(this);
        }
        /// <summary>
        /// Class implementation for IndicatorPresenter
        /// </summary>
        public class IndicatorPresenter : ContentPresenter
        {
            #region Members
            /// <summary>
            /// Initializes m_segment
            /// </summary>
            private ChartTechnicalIndicator m_indicator;

            internal PointCollection BeyondVisiblePoints = null;

            #endregion

            #region Properties
            /// <summary>
            /// Gets the segment.
            /// </summary>
            /// <value>The segment.</value>
            public ChartTechnicalIndicator Indicator
            {
                get { return m_indicator; }
            }
            #endregion
            /// <summary>
            /// Called when instance created for Indicatorpresenter
            /// </summary>
            /// <param name="indicator"></param>
            public IndicatorPresenter(ChartTechnicalIndicator indicator)
            {
                m_indicator = indicator;

                indicator.Series.PropertyChanged += new DependencyPropertyChangedEventHandler(Series_PropertyChanged);
                this.ContentTemplateSelector = ChartTypeTemplateSelector.Default;
                BindingUtils.SetBinding(this, indicator.Series, ContentPresenter.ContentTemplateProperty, ChartSeries.TemplateProperty);
                if (m_indicator.Series != null && m_indicator.Series.Area != null)
                {
                    m_indicator.Series.Area.isIndicatorEnabled = true;
                }
            }

            void Series_PropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                ChartSeries series = sender as ChartSeries;
                if (series != null && e.Property.ToString() == "FastTypePen")
                {
                    InvalidateVisual();
                }
            }

            void indicator_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                InvalidateVisual();
            }

            /// <summary>
            /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing. 
            /// </summary>
            /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
            protected override void OnRender(DrawingContext drawingContext)
            {
                Random r1 = new Random();
                ChartSeries cachedSeries = null;
                if (this.ActualWidth != 0 && this.ActualHeight != 0)
                {
                    ChartTechnicalIndicator indic = this.Indicator;
                    IChartTransformer transformer = null;

                    ChartSeries series = indic.Series;
                    //if (cachedSeries != series)
                    {
                        cachedSeries = series;
                        transformer = ChartTransform.CreateCartesian(new Rect(new Point(0, 0), new Size(this.ActualWidth, this.ActualHeight)), series);
                    }
                    if (indic.MACDArea != null && indic.IndicatorType != IndicatorTypes.MACD)
                    {
                        (indic.Series.Area.Parent as Chart).Areas.Remove(indic.MACDArea);
                        indic.MACDArea = null;
                    }
                    if (indic.stocasticsArea != null && indic.IndicatorType != IndicatorTypes.Stochastics && indic.Series.Area.SyncChartArea!= null)
                    {
                        (indic.Series.Area.Parent as Chart).Areas.Remove(indic.stocasticsArea);
                        indic.stocasticsArea = null;
                    }
                    if (indic.accumulationArea != null && indic.IndicatorType != IndicatorTypes.AccumulationDistribution)
                    {
                        (indic.Series.Area.Parent as Chart).Areas.Remove(indic.accumulationArea);
                        indic.accumulationArea = null;
                    }
                    if (indic.rsiArea != null && indic.IndicatorType != IndicatorTypes.RelativeStrengthIndex)
                    {
                        (indic.Series.Area.Parent as Chart).Areas.Remove(indic.rsiArea);
                        indic.rsiArea = null;
                    }
                    if (indic.momentumArea != null && indic.IndicatorType != IndicatorTypes.Momentum)
                    {
                        (indic.Series.Area.Parent as Chart).Areas.Remove(indic.momentumArea);
                        indic.momentumArea = null;
                    }
                    if (indic.avtArea != null && indic.IndicatorType != IndicatorTypes.AverageTrueRange)
                    {
                        (indic.Series.Area.Parent as Chart).Areas.Remove(indic.avtArea);
                        indic.avtArea = null;
                    }
                    PointCollection rangePoints = new PointCollection();
                    switch (indic.IndicatorType)
                    {
                        case IndicatorTypes.BollingerBands:
                            if (indic.BollingerIndicator != null && indic.BollingerIndicator.LowerPoints != null && indic.BollingerIndicator.UpperPoints != null)
                            {
                                int startPoint = (ChartBollingerBand.GetBollingerMovingAverage(indic)) - 1;
                                for (int i = startPoint, j = startPoint, k = startPoint; i < indic.BollingerIndicator.UpperPoints.Count - 1 && j < indic.BollingerIndicator.LowerPoints.Count - 1 && k < indic.BollingerIndicator.SignalPoints.Count - 1; i++, j++, k++)
                                {
                                    if (indic.BollingerIndicator.UpperPoints[i].Y > indic.Series.YAxis.m_visibleRange.End)
                                    {
                                        if (indic.BollingerIndicator.MaxYValue < indic.BollingerIndicator.UpperPoints[i].Y)
                                        {
                                            indic.BollingerIndicator.MaxYValue = indic.BollingerIndicator.UpperPoints[i].Y;
                                        }
                                        else if (indic.BollingerIndicator.MaxYValue == 0)
                                        {
                                            indic.BollingerIndicator.MaxYValue = indic.BollingerIndicator.UpperPoints[i].Y;
                                        }
                                    }
                                    if (indic.BollingerIndicator.LowerPoints[i].Y < indic.Series.YAxis.m_visibleRange.Start)
                                    {
                                        if (indic.BollingerIndicator.MinYValue == 0)
                                        {
                                            indic.BollingerIndicator.MinYValue = indic.BollingerIndicator.LowerPoints[i].Y;
                                        }
                                        if (indic.BollingerIndicator.MinYValue > indic.BollingerIndicator.LowerPoints[i].Y)
                                        {
                                            indic.BollingerIndicator.MinYValue = indic.BollingerIndicator.LowerPoints[i].Y;
                                        }
                                    }

                                    Point pt1 = transformer.TransformToVisible(indic.BollingerIndicator.UpperPoints[i].X, indic.BollingerIndicator.UpperPoints[i].Y);
                                    Point pt2 = transformer.TransformToVisible(indic.BollingerIndicator.UpperPoints[i + 1].X, indic.BollingerIndicator.UpperPoints[i + 1].Y);
                                    drawingContext.DrawLine(new Pen(ChartBollingerBand.GetUpperLineColor(indic), 1), pt1, pt2);

                                    Point pt11 = transformer.TransformToVisible(indic.BollingerIndicator.LowerPoints[j].X, indic.BollingerIndicator.LowerPoints[j].Y);
                                    Point pt21 = transformer.TransformToVisible(indic.BollingerIndicator.LowerPoints[j + 1].X, indic.BollingerIndicator.LowerPoints[j + 1].Y);
                                    drawingContext.DrawLine(new Pen(ChartBollingerBand.GetLowerLineColor(indic), 1), pt11, pt21);

                                    Point pt111 = transformer.TransformToVisible(indic.BollingerIndicator.SignalPoints[k].X, indic.BollingerIndicator.SignalPoints[k].Y);
                                    Point pt211 = transformer.TransformToVisible(indic.BollingerIndicator.SignalPoints[k + 1].X, indic.BollingerIndicator.SignalPoints[k + 1].Y);
                                    drawingContext.DrawLine(new Pen(ChartBollingerBand.GetSignalLineColor(indic), 1) {DashStyle= DashStyles.Dash }, pt111, pt211);
                                }
                            }
                            break;
                        case IndicatorTypes.MACD:
                            if (indic.MACDIndicator != null && indic.MACDIndicator.DivergencePoints != null && indic.MACDIndicator.ConvergencePoints != null && indic.MACDIndicator.CenterPoints != null)
                            {
                                if (indic.MACDArea == null)
                                {
                                    if (indic.Series.Area.SyncChartArea == null)
                                    {
                                        ChartArea area = new ChartArea();
                                        area.Width = indic.Series.Area.ActualWidth;
                                        area.ElementMargin = indic.Series.Area.ElementMargin;
                                        area.Margin = indic.Series.Area.Margin;
                                        ChartAxis axis = indic.Series.XAxis;
                                        area.Series.Add(new ChartSeries() { DataSource = indic.MACDIndicator.DivergencePoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartMACD.GetDivergenceLineColor(indic), ToolTip = "MACD Indicator" });
                                        area.Series.Add(new ChartSeries() { DataSource = indic.MACDIndicator.ConvergencePoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartMACD.GetConvergenceLineColor(indic), ToolTip = "MACD Indicator" });
                                        area.Series.Add(new ChartSeries() { DataSource = indic.MACDIndicator.CenterPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartMACD.GetSignalLineInterior(indic), ToolTip = "MACD Indicator" });
                                        area.PrimaryAxis = indic.Series.XAxis;
                                        ChartArea.SetShowGridLines(area.PrimaryAxis, false);
                                        indic.MACDArea = area;
                                        (indic.Series.Area.Parent as Chart).Areas.Add(area);
                                    }
                                }
                                else
                                {
                                    if (indic.Series.Area.SyncChartArea == null)
                                    {
                                        foreach (ChartArea area in (indic.Series.Area.Parent as Chart).Areas)
                                        {
                                            if (area == indic.MACDArea)
                                            {
                                                area.Width = indic.Series.Area.ActualWidth;
                                                area.ElementMargin = indic.Series.Area.ElementMargin;
                                                area.Margin = indic.Series.Area.Margin;
                                                area.Series[0] = new ChartSeries() { DataSource = indic.MACDIndicator.DivergencePoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartMACD.GetDivergenceLineColor(indic), ToolTip = "MACD Indicator" };
                                                area.Series[1] = new ChartSeries() { DataSource = indic.MACDIndicator.ConvergencePoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartMACD.GetConvergenceLineColor(indic), ToolTip = "MACD Indicator" };
                                                area.Series[2] = new ChartSeries() { DataSource = indic.MACDIndicator.CenterPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartMACD.GetSignalLineInterior(indic), ToolTip = "MACD Indicator" };
                                                area.PrimaryAxis = indic.Series.XAxis;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        SyncChartAreas sarea = indic.Series.Area.SyncChartArea as SyncChartAreas;
                                        foreach (ChartArea area in sarea.Areas)
                                        {
                                            if (area == indic.MACDArea)
                                            {
                                                //area.Width = sarea.ActualWidth;
                                               // area.ElementMargin = indic.Series.Area.ElementMargin;
                                                //area.Margin = indic.Series.Area.Margin;
                                                area.Series[0].Interior = ChartMACD.GetDivergenceLineColor(indic);
                                                area.Series[1].Interior = ChartMACD.GetConvergenceLineColor(indic);
                                                area.Series[2].Interior = ChartMACD.GetSignalLineInterior(indic);
                                               // area.Margin = new Thickness(sarea.Areas[0].AxesThickness.Left - area.AxesThickness.Left, sarea.Areas[0].AxesThickness.Top - area.AxesThickness.Top, sarea.Areas[0].AxesThickness.Right - area.AxesThickness.Right, sarea.Areas[0].AxesThickness.Bottom - area.AxesThickness.Bottom);
                                                area.Series[0].DataSource = indic.MACDIndicator.DivergencePoints;
                                                area.Series[1].DataSource = indic.MACDIndicator.ConvergencePoints;
                                                area.Series[2].DataSource = indic.MACDIndicator.CenterPoints;
                                                //area.Series[0].Invalidate();
                                                //area.Series[1].Invalidate();
                                                //area.Series[2].Invalidate();
                                                area.PrimaryAxis.ZoomFactor = indic.Series.Area.PrimaryAxis.ZoomFactor;
                                            }
                                        }

                                        
                                    }
                                }

                            }
                            break;
                        case IndicatorTypes.ExponentialAverage:
                            if (indic.ExponentialIndicator != null)
                            {
                                for (int k = 0; k < indic.ExponentialIndicator.ExponentialSignalPoints.Count - 1; k++)
                                {
                                    if (indic.ExponentialIndicator.ExponentialSignalPoints[k].X >= indic.Series.XAxis.VisibleRange.Start && indic.ExponentialIndicator.ExponentialSignalPoints[k].X <= indic.Series.XAxis.VisibleRange.End && indic.ExponentialIndicator.ExponentialSignalPoints[k].Y > indic.Series.YAxis.VisibleRange.Start && indic.ExponentialIndicator.ExponentialSignalPoints[k].Y < indic.Series.YAxis.VisibleRange.End)
                                    {
                                        Point pt111 = transformer.TransformToVisible(indic.ExponentialIndicator.ExponentialSignalPoints[k].X, indic.ExponentialIndicator.ExponentialSignalPoints[k].Y);
                                        Point pt211 = transformer.TransformToVisible(indic.ExponentialIndicator.ExponentialSignalPoints[k + 1].X, indic.ExponentialIndicator.ExponentialSignalPoints[k + 1].Y);
                                        drawingContext.DrawLine(new Pen(ChartExponentialAverage.GetSignalLineInterior(indic), 1), pt111, pt211);
                                    }

                                }
                            }
                            break;
                        case IndicatorTypes.SimpleAverage:
                            if (indic.simpleAverage != null)
                            {
                                for (int i = 0; i < indic.simpleAverage.SimpleAverageSignalPoints.Count - 1; i++)
                                {
                                    if (indic.simpleAverage.SimpleAverageSignalPoints[i].X >= indic.Series.XAxis.VisibleRange.Start && indic.simpleAverage.SimpleAverageSignalPoints[i].X <= indic.Series.XAxis.VisibleRange.End && indic.simpleAverage.SimpleAverageSignalPoints[i].Y > indic.Series.YAxis.VisibleRange.Start && indic.simpleAverage.SimpleAverageSignalPoints[i].Y < indic.Series.YAxis.VisibleRange.End)
                                    {
                                        Point pt111 = transformer.TransformToVisible(indic.simpleAverage.SimpleAverageSignalPoints[i].X, indic.simpleAverage.SimpleAverageSignalPoints[i].Y);
                                        Point pt211 = transformer.TransformToVisible(indic.simpleAverage.SimpleAverageSignalPoints[i + 1].X, indic.simpleAverage.SimpleAverageSignalPoints[i + 1].Y);
                                        drawingContext.DrawLine(new Pen(ChartSimpleAverage.GetSignalLineInterior(indic), 1), pt111, pt211);
                                    }

                                }
                            }
                            break;
                        case IndicatorTypes.TriangularAverage:
                            if (indic.triangularIndicator != null)
                            {
                                for (int i = 0; i < indic.triangularIndicator.TriangularIndicatorPoints.Count - 1; i++)
                                {
                                    if (indic.triangularIndicator.TriangularIndicatorPoints[i].X >= indic.Series.XAxis.VisibleRange.Start && indic.triangularIndicator.TriangularIndicatorPoints[i].X <= indic.Series.XAxis.VisibleRange.End && indic.triangularIndicator.TriangularIndicatorPoints[i].Y > indic.Series.YAxis.VisibleRange.Start && indic.triangularIndicator.TriangularIndicatorPoints[i].Y < indic.Series.YAxis.VisibleRange.End)
                                    {
                                        Point pt111 = transformer.TransformToVisible(indic.triangularIndicator.TriangularIndicatorPoints[i].X, indic.triangularIndicator.TriangularIndicatorPoints[i].Y);
                                        Point pt211 = transformer.TransformToVisible(indic.triangularIndicator.TriangularIndicatorPoints[i + 1].X, indic.triangularIndicator.TriangularIndicatorPoints[i + 1].Y);
                                        drawingContext.DrawLine(new Pen(ChartTriangularAverage.GetSignalLineColor(indic), 1), pt111, pt211);
                                    }
                                }
                            }
                            break;
                        case IndicatorTypes.Stochastics:
                            if (indic.stochasticsIndicator != null && indic.stochasticsIndicator.UpperLinePoitns != null && indic.stochasticsIndicator.LowerLinePoints != null && indic.stochasticsIndicator.StochasticsPoints != null)
                            {
                                if (indic.stocasticsArea == null)
                                {
                                    if (indic.Series.Area.SyncChartArea == null)
                                    {
                                        ChartArea area = new ChartArea();
                                        area.Width = indic.Series.Area.ActualWidth;
                                        area.ElementMargin = indic.Series.Area.ElementMargin;
                                        area.Margin = indic.Series.Area.Margin;
                                        area.Series.Add(new ChartSeries() { DataSource = indic.stochasticsIndicator.LowerLinePoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartStochastics.GetLowerLineColor(indic), ToolTip = "Stochastics Indicator" });
                                        area.Series.Add(new ChartSeries() { DataSource = indic.stochasticsIndicator.UpperLinePoitns, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartStochastics.GetUpperLineColor(indic), ToolTip = "Stochastics Indicator" });
                                        area.Series.Add(new ChartSeries() { DataSource = indic.stochasticsIndicator.StochasticsPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartStochastics.GetSignalLineColor(indic), ToolTip = "Stochastics Indicator" });
                                        area.PrimaryAxis = indic.Series.XAxis;
                                        ChartArea.SetShowGridLines(area.PrimaryAxis, false);
                                        indic.stocasticsArea = area;                                        
                                        (indic.Series.Area.Parent as Chart).Areas.Add(area);
                                    }
                                }
                                else
                                {
                                    if (indic.Series.Area.SyncChartArea == null)
                                    {
                                        foreach (ChartArea area in (indic.Series.Area.Parent as Chart).Areas)
                                        {
                                            if (area == indic.stocasticsArea)
                                            {
                                                area.Series[0] = new ChartSeries() { DataSource = indic.stochasticsIndicator.LowerLinePoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartStochastics.GetLowerLineColor(indic), ToolTip = "Stochastics Indicator" };
                                                area.Series[1] = new ChartSeries() { DataSource = indic.stochasticsIndicator.UpperLinePoitns, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartStochastics.GetUpperLineColor(indic), ToolTip = "Stochastics Indicator" };
                                                area.Series[2] = new ChartSeries() { DataSource = indic.stochasticsIndicator.StochasticsPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartStochastics.GetSignalLineColor(indic), ToolTip = "Stochastics Indicator" };
                                                area.PrimaryAxis = indic.Series.XAxis;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        SyncChartAreas sarea = indic.Series.Area.SyncChartArea as SyncChartAreas;
                                        foreach (ChartArea area in sarea.Areas)
                                        {
                                            if (area == indic.stocasticsArea)
                                            {
                                               // area.Width = sarea.ActualWidth;
                                                //area.ElementMargin = indic.Series.Area.ElementMargin;
                                                //area.Margin = indic.Series.Area.Margin;
                                                area.Series[0].Interior = ChartStochastics.GetLowerLineColor(indic);
                                                area.Series[1].Interior = ChartStochastics.GetUpperLineColor(indic); 
                                                area.Series[2].Interior = ChartStochastics.GetSignalLineColor(indic);
                                                //area.Margin = new Thickness(sarea.Areas[0].AxesThickness.Left - area.AxesThickness.Left, sarea.Areas[0].AxesThickness.Top - area.AxesThickness.Top, sarea.Areas[0].AxesThickness.Right - area.AxesThickness.Right, sarea.Areas[0].AxesThickness.Bottom - area.AxesThickness.Bottom);
                                                area.Series[0].DataSource = indic.stochasticsIndicator.LowerLinePoints;
                                                area.Series[1].DataSource = indic.stochasticsIndicator.UpperLinePoitns;
                                                area.Series[2].DataSource = indic.stochasticsIndicator.StochasticsPoints;
                                                //area.Series[0].Invalidate();
                                                //area.Series[1].Invalidate();
                                                //area.Series[2].Invalidate();
                                                area.PrimaryAxis.ZoomFactor = indic.Series.Area.PrimaryAxis.ZoomFactor;
                                            }

                                        }
                                    }
                                }
                            }
                            break;
                        case IndicatorTypes.AccumulationDistribution:
                            if (indic.accumulationIndicator != null && indic.accumulationIndicator.AccumulationPoints != null)
                            {
                                if (indic.accumulationArea == null)
                                {
                                    if (indic.Series.Area.SyncChartArea == null)
                                    {
                                        ChartArea area = new ChartArea();
                                        area.Width = indic.Series.Area.ActualWidth;
                                        area.ElementMargin = indic.Series.Area.ElementMargin;
                                        area.Margin = indic.Series.Area.Margin;
                                        area.Series.Add(new ChartSeries() { DataSource = indic.accumulationIndicator.AccumulationPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartAccumulationDistribution.GetSignalLineColor(indic), ToolTip = "Accumulation Distribution" });
                                        area.PrimaryAxis = indic.Series.XAxis;
                                        ChartArea.SetShowGridLines(area.PrimaryAxis, false);
                                        indic.accumulationArea = area;
                                        (indic.Series.Area.Parent as Chart).Areas.Add(area);
                                    }
                                }
                                else
                                {
                                    if (indic.Series.Area.SyncChartArea == null)
                                    {
                                        foreach (ChartArea area in (indic.Series.Area.Parent as Chart).Areas)
                                        {
                                            if (area == indic.accumulationArea)
                                            {
                                                area.Series[0] = new ChartSeries() { DataSource = indic.accumulationIndicator.AccumulationPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = ChartAccumulationDistribution.GetSignalLineColor(indic), ToolTip = "Accumulation Distribution" };
                                                area.PrimaryAxis = indic.Series.XAxis;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        SyncChartAreas sarea = indic.Series.Area.SyncChartArea as SyncChartAreas;
                                        foreach (ChartArea area in sarea.Areas)
                                        {
                                            if (area == indic.accumulationArea)
                                            {
                                                area.Width = sarea.ActualWidth;
                                                //area.ElementMargin = indic.Series.Area.ElementMargin;
                                                //area.Margin = indic.Series.Area.Margin;
                                                area.Series[0].Interior = ChartAccumulationDistribution.GetSignalLineColor(indic);
                                                //area.Margin = new Thickness(sarea.Areas[0].AxesThickness.Left - area.AxesThickness.Left, sarea.Areas[0].AxesThickness.Top - area.AxesThickness.Top, sarea.Areas[0].AxesThickness.Right - area.AxesThickness.Right, sarea.Areas[0].AxesThickness.Bottom - area.AxesThickness.Bottom);
                                                area.Series[0].DataSource = indic.accumulationIndicator.AccumulationPoints;
                                                //area.Series[0].Invalidate();
                                                area.PrimaryAxis.ZoomFactor = indic.Series.Area.PrimaryAxis.ZoomFactor;
                                            }
                                        }                                        
                                    }
                                }
                            }

                            break;
                        case IndicatorTypes.RelativeStrengthIndex:
                            if (indic.rsiIndicator != null && indic.rsiIndicator.RSIpoints != null && indic.rsiIndicator.lowerPoints != null && indic.rsiIndicator.upperPoints != null)
                            {
                                if (indic.rsiArea == null)
                                {
                                    if (indic.Series.Area.SyncChartArea == null)
                                    {
                                        ChartArea area = new ChartArea();
                                        area.Width = indic.Series.Area.ActualWidth;
                                        area.ElementMargin = indic.Series.Area.ElementMargin;
                                        area.Margin = indic.Series.Area.Margin;
                                        ChartAxis axis = indic.Series.XAxis;
                                        area.Series.Add(new ChartSeries() { DataSource = indic.rsiIndicator.upperPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.Maroon, ToolTip = "Relative Strength Index" });
                                        area.Series.Add(new ChartSeries() { DataSource = indic.rsiIndicator.lowerPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.DarkGreen, ToolTip = "Relative Strength Index" });
                                        area.Series.Add(new ChartSeries() { DataSource = indic.rsiIndicator.RSIpoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.Navy, ToolTip = "Relative Strength Index" });
                                        area.PrimaryAxis = indic.Series.XAxis;
                                        ChartArea.SetShowGridLines(area.PrimaryAxis, false);
                                        indic.rsiArea = area;
                                        (indic.Series.Area.Parent as Chart).Areas.Add(area);
                                    }
                                }
                                else
                                {
                                    if (indic.Series.Area.SyncChartArea == null)
                                    {
                                        foreach (ChartArea area in (indic.Series.Area.Parent as Chart).Areas)
                                        {
                                            if (area == indic.rsiArea)
                                            {
                                                area.Series[0] = new ChartSeries() { DataSource = indic.rsiIndicator.upperPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.Maroon, ToolTip = "Relative Strength Index" };
                                                area.Series[1] = new ChartSeries() { DataSource = indic.rsiIndicator.lowerPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.DarkGreen, ToolTip = "Relative Strength Index" };
                                                area.Series[2] = new ChartSeries() { DataSource = indic.rsiIndicator.RSIpoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.Navy, ToolTip = "Relative Strength Index" };
                                                area.PrimaryAxis = indic.Series.XAxis;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        SyncChartAreas sarea = indic.Series.Area.SyncChartArea as SyncChartAreas;
                                        foreach (ChartArea area in sarea.Areas)
                                        {
                                            if (area == indic.rsiArea)
                                            {
                                              //  area.Width = sarea.ActualWidth;
                                               // area.ElementMargin = indic.Series.Area.ElementMargin;
                                               // area.Margin = indic.Series.Area.Margin;
                                                area.Series[0].Interior = Brushes.Orange;
                                                area.Series[1].Interior = Brushes.DarkGreen;
                                                area.Series[2].Interior = Brushes.Aqua;
                                            //    area.Margin = new Thickness(sarea.Areas[0].AxesThickness.Left - area.AxesThickness.Left, sarea.Areas[0].AxesThickness.Top - area.AxesThickness.Top, sarea.Areas[0].AxesThickness.Right - area.AxesThickness.Right, sarea.Areas[0].AxesThickness.Bottom - area.AxesThickness.Bottom);
                                                area.Series[0].DataSource = indic.rsiIndicator.upperPoints;
                                                area.Series[1].DataSource = indic.rsiIndicator.lowerPoints;
                                                area.Series[2].DataSource = indic.rsiIndicator.RSIpoints;
                                                //area.Series[0].Invalidate();
                                                //area.Series[1].Invalidate();
                                                //area.Series[2].Invalidate();
                                                area.PrimaryAxis.ZoomFactor = indic.Series.Area.PrimaryAxis.ZoomFactor;
                                            }

                                        }
                                    }
                                }
                            }
                            break;
                        case IndicatorTypes.Momentum:
                            if (indic.momentumIndicator != null && indic.momentumIndicator.Momentumpoints != null && indic.momentumIndicator.lowerPoints != null)
                            {
                                if (indic.momentumArea == null)
                                {
                                    if (indic.Series.Area.SyncChartArea == null)
                                    {
                                        ChartArea area = new ChartArea();
                                        area.Width = indic.Series.Area.ActualWidth;
                                        area.ElementMargin = indic.Series.Area.ElementMargin;
                                        area.Margin = indic.Series.Area.Margin;
                                        ChartAxis axis = indic.Series.XAxis;
                                        area.Series.Add(new ChartSeries() { DataSource = indic.momentumIndicator.Momentumpoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.Maroon, ToolTip = "Momentum Indicator" });
                                        area.Series.Add(new ChartSeries() { DataSource = indic.momentumIndicator.lowerPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.DarkGreen, ToolTip = "Momentum Indicator" });
                                        area.PrimaryAxis = indic.Series.XAxis;
                                        ChartArea.SetShowGridLines(area.PrimaryAxis, false);
                                        indic.momentumArea = area;
                                        (indic.Series.Area.Parent as Chart).Areas.Add(area);
                                    }
                                }
                                else
                                {
                                    if (indic.Series.Area.SyncChartArea == null)
                                    {
                                        foreach (ChartArea area in (indic.Series.Area.Parent as Chart).Areas)
                                        {
                                            if (area == indic.momentumArea)
                                            {
                                                area.Series[0] = new ChartSeries() { DataSource = indic.momentumIndicator.Momentumpoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.DarkOrange, ToolTip = "Momentum Indicator" };
                                                area.Series[1] = new ChartSeries() { DataSource = indic.momentumIndicator.lowerPoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.DarkViolet, ToolTip = "Momentum Indicator" };
                                                area.PrimaryAxis = indic.Series.XAxis;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        SyncChartAreas sarea = indic.Series.Area.SyncChartArea as SyncChartAreas;
                                        foreach (ChartArea area in sarea.Areas)
                                        {
                                            if (area == indic.momentumArea)
                                            {
                                                //area.Width = sarea.ActualWidth;
                                               // area.ElementMargin = indic.Series.Area.ElementMargin;
                                               // area.Margin = indic.Series.Area.Margin;
                                                area.Series[0].Interior = Brushes.DarkOrange;
                                                area.Series[1].Interior = Brushes.DarkViolet;
                                                //area.Margin = new Thickness(sarea.Areas[0].AxesThickness.Left - area.AxesThickness.Left, sarea.Areas[0].AxesThickness.Top - area.AxesThickness.Top, sarea.Areas[0].AxesThickness.Right - area.AxesThickness.Right, sarea.Areas[0].AxesThickness.Bottom - area.AxesThickness.Bottom);
                                                area.Series[0].DataSource = indic.momentumIndicator.Momentumpoints;
                                                area.Series[1].DataSource = indic.momentumIndicator.lowerPoints;
                                                //area.Series[0].Invalidate();
                                                //area.Series[1].Invalidate();
                                                area.PrimaryAxis.ZoomFactor = indic.Series.Area.PrimaryAxis.ZoomFactor;
                                            }
                                        }                                        
                                        
                                    }
                                }
                            }
                            break;
                        case IndicatorTypes.AverageTrueRange:
                            if (indic.avtIndicator != null && indic.avtIndicator.ATRpoints != null)
                            {
                                if (indic.avtArea == null)
                                {
                                    if (indic.Series.Area.SyncChartArea == null)
                                    {
                                        ChartArea area = new ChartArea();
                                        area.Width = indic.Series.Area.ActualWidth;
                                        area.ElementMargin = indic.Series.Area.ElementMargin;
                                        area.Margin = indic.Series.Area.Margin;
                                        ChartAxis axis = indic.Series.XAxis;
                                        area.Series.Add(new ChartSeries() { DataSource = indic.avtIndicator.ATRpoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.Maroon, ToolTip = "Average True Range" });

                                        area.PrimaryAxis = indic.Series.XAxis;
                                        ChartArea.SetShowGridLines(area.PrimaryAxis, false);
                                        indic.avtArea = area;
                                        (indic.Series.Area.Parent as Chart).Areas.Add(area);
                                    }
                                }
                                else
                                {
                                    if (indic.Series.Area.SyncChartArea == null)
                                    {
                                        foreach (ChartArea area in (indic.Series.Area.Parent as Chart).Areas)
                                        {
                                            if (area == indic.avtArea)
                                            {
                                                area.Series[0] = new ChartSeries() { DataSource = indic.avtIndicator.ATRpoints, BindingPathX = "X", BindingPathsY = new string[] { "Y" }, Type = ChartTypes.FastLine, Interior = Brushes.Aqua, ToolTip = "Average True Range" };
                                                area.PrimaryAxis = indic.Series.XAxis;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        SyncChartAreas sarea = indic.Series.Area.SyncChartArea as SyncChartAreas;
                                        foreach (ChartArea area in sarea.Areas)
                                        {
                                            if (area == indic.avtArea)
                                            {
                                                //area.Width = sarea.ActualWidth;
                                                //area.ElementMargin = indic.Series.Area.ElementMargin;
                                                //area.Margin = indic.Series.Area.Margin;
                                                area.Series[0].Interior = Brushes.Aqua;
                                                //area.Margin = new Thickness(sarea.Areas[0].AxesThickness.Left - area.AxesThickness.Left, sarea.Areas[0].AxesThickness.Top - area.AxesThickness.Top, sarea.Areas[0].AxesThickness.Right - area.AxesThickness.Right, sarea.Areas[0].AxesThickness.Bottom - area.AxesThickness.Bottom);
                                                area.Series[0].DataSource = indic.avtIndicator.ATRpoints;
                                                //area.Series[0].Invalidate();
                                                area.PrimaryAxis.ZoomFactor = indic.Series.Area.PrimaryAxis.ZoomFactor;
                                            }

                                        }
                                    }
                                }
                            }
                            break;
                    }
                    indic.Series.Area.UpdateArea();
                    indic.VisiblePoints = null;

                }
                base.OnRender(drawingContext);
            }
        }

        #region Internal types
        /// <summary>
        /// Represents ChartSegmentPresenter class
        /// </summary>
        /// <example>
        /// The below given code illustrates how Chartsegment could be selected when the
        /// user mouse downs on a data point segment.
        /// <code language="C#">
        /// private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        /// {
        ///     // Get the corresponding ChartSegment
        ///     ChartSegment seg =
        /// ((ChartSeriesPresenter.ChartSegmentPresenter)(((Canvas)sender).TemplatedParent)).Segment;
        ///     // Get the corresponding bound CollectionView
        ///     CollectionView cv = seg.Series.DataSource as CollectionView;
        ///     // Set the CurrentItem in the CollectionView
        ///     cv.MoveCurrentToPosition(seg.CorrespondingPoints[0].Index);
        /// }
        /// </code>
        /// </example>
        public class ChartSegmentPresenter : ContentPresenter, IDisposable
        {
            #region Members
            /// <summary>
            /// Initializes m_segment
            /// </summary>
            private ChartSegment m_segment;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the segment.
            /// </summary>
            /// <value>The segment.</value>
            public ChartSegment Segment
            {
                get { return m_segment; }
            }
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="ChartSegmentPresenter"/> class.
            /// </summary>
            /// <param name="segment">The segment.</param>
            public ChartSegmentPresenter(ChartSegment segment)
            {
                m_segment = segment;

                this.Content = segment;
                this.DataContext = segment;
                this.ContentTemplateSelector = ChartTypeTemplateSelector.Default;

                BindingUtils.SetBinding(this, segment.Series, ContentPresenter.ContentTemplateProperty, ChartSeries.TemplateProperty);
            }
            #endregion

            #region IDisposable Members
            /// <summary>
            /// Clean up any resources being used.
            /// </summary>
            public void Dispose()
            {
                if (!(this.m_segment is ChartFastLineSegment) && this.m_segment!=null)
                {
                    this.m_segment.Dispose();
                    this.m_segment = null;
                    this.Content = null;
                }
                //this.DataContext = null;
                
            }

            #endregion
        }
        #endregion

        #region Dependency properties

        /// <summary>
        /// Using a DependencyProperty as the backing store for Series.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(ChartSeries), typeof(ChartSeriesPresenter), new PropertyMetadata(null, new PropertyChangedCallback(OnSeriesChanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for XAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty XAxisProperty =
          DependencyProperty.Register("XAxis", typeof(ChartAxis), typeof(ChartSeriesPresenter), new UIPropertyMetadata(null, new PropertyChangedCallback(OnAxeschanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for YAxis.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty YAxisProperty =
          DependencyProperty.Register("YAxis", typeof(ChartAxis), typeof(ChartSeriesPresenter), new UIPropertyMetadata(null, new PropertyChangedCallback(OnAxeschanged)));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Object.  This enables animation, styling, binding, etc...
        /// </summary> 
        protected static readonly DependencyProperty ObjectProperty =
          DependencyProperty.RegisterAttached("Object", typeof(object), typeof(ChartSeriesPresenter));

        #endregion

        #region Members
        /// <summary>
        /// Initializes m_elements
        /// </summary>
        private List<ChartSegmentPresenter> m_elements;

        private List<IndicatorPresenter> m_indicators;

        internal IndicatorPresenter m_IndicatorPresenter = null;

        /// <summary>
        /// Initializes m_visibleElements
        /// </summary>
        internal UIElementCollection m_visibleElements;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the X axis.
        /// </summary>
        /// <value>The X axis.</value>
        public ChartAxis XAxis
        {
            get { return (ChartAxis)GetValue(XAxisProperty); }
            set { SetValue(XAxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Y axis.
        /// </summary>
        /// <value>The Y axis.</value>
        public ChartAxis YAxis
        {
            get { return (ChartAxis)GetValue(YAxisProperty); }
            set { SetValue(YAxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the series.
        /// </summary>
        /// <value>The series.</value>
        public ChartSeries Series
        {
            get { return (ChartSeries)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        /// <summary>
        /// Gets the Total size of Chart series render.
        /// </summary>
        public Size TotalSize
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the VisualChildrenCount
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return m_visibleElements.Count;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSeriesPresenter"/> class.
        /// </summary>
        public ChartSeriesPresenter()
        {
            m_elements = new List<ChartSegmentPresenter>();
            m_visibleElements = new UIElementCollection(this, this);
            m_indicators = new List<IndicatorPresenter>();
        }
        #endregion
        internal bool Issizechanged = true;
        private bool IsAddIndicator = true;
        private int indicpos = -1;
        private int IncrementValue = 0;
        private int indicator_count = 0;
        internal IChartTransformer temptransformer;
        internal bool rangechanged = false;
        #region Implementation
        /// <summary>
        /// Arranges the content of a <see cref="T:System.Windows.Controls.Canvas"></see> element.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"></see> that represents the arranged size of this <see cref="T:System.Windows.Controls.Canvas"></see> element and its descendants.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            IChartTransformer transformer = this.CreateTransformer(new Rect(finalSize));

            this.InvalidateChildrenVisibility();
            foreach (UIElement element in m_visibleElements)
            {
                if (element is ChartSegmentPresenter)
                {
                    ChartSegmentPresenter segmentPresenter = (ChartSegmentPresenter)element;
                    if (segmentPresenter.Segment.Series.Type == ChartTypes.FastScatter || segmentPresenter.Segment.Series.Type == ChartTypes.Scatter || segmentPresenter.Segment.Series.Type == ChartTypes.Bubble)
                    {
                        //Requires when updating huge datapoints
                        segmentPresenter.Segment.Update(transformer);
                        segmentPresenter.Arrange(new Rect(finalSize));
                    }
                    else
                    {
                        if (segmentPresenter.Segment is ChartFastLineSegment)
                        {
                            if (temptransformer == null)
                            {
                                Issizechanged = true;
                                temptransformer = transformer;                              
                                segmentPresenter.Arrange(new Rect(finalSize));
                                segmentPresenter.Segment.Update(transformer);
                            }
                            else
                            {
                                //Commented for giving emptypoint support.
                                //Since the width of this presenter layout flicker  more or less to 5 on realtime update, so to avoid the rerendering of layout for this small change below condition is added.
                                //if ((temptransformer.Viewport.Width - transformer.Viewport.Width < 5 && temptransformer.Viewport.Width - transformer.Viewport.Width > -5) && (temptransformer.Viewport.Height == transformer.Viewport.Height) && !rangechanged)
                                //{
                                //    Issizechanged = false;
                                //    segmentPresenter.Arrange(new Rect(finalSize));
                                //    segmentPresenter.Segment.Update(transformer);
                                //}
                                //else
                                {
                                    
                                        Issizechanged = true;
                                        temptransformer = transformer;
                                   
                                    segmentPresenter.Arrange(new Rect(finalSize));
                                    segmentPresenter.Segment.Update(transformer);
                                    rangechanged = false;
                                }
                            }
                        }
                        else
                        {
                            if (this.Series.Area.m_scrolling)
                            {
                                segmentPresenter.Segment.Update(transformer);
                                segmentPresenter.Arrange(new Rect(finalSize));
                                
                            }
                            else
                            {
                                segmentPresenter.Arrange(new Rect(finalSize));
                                segmentPresenter.Segment.Update(transformer);
                            }
                        }
                    }
                }
                else
                {
                    element.Arrange(new Rect(finalSize));
                    IsAddIndicator = true;
                }
            }
           
            if (this.Series != null && this.Series.Animation != null && m_visibleElements.Count != 0 && this.Series.Animation.Storyboard.Children.Count == 0 && this.Series.EnableAnimation == true)
            {
                this.Series.Animation.AnimateSeries();
            }

            if (this.Series != null)
            {
                if (this.Series.Area != null && this.Series.Area.InteractiveCursors.Count != 0)
                {
                    foreach (InteractiveCursor ic in this.Series.Area.InteractiveCursors)
                    {
                        ic.SetValueForInteractiveCursor(false);
                    }
                }
            }           
            this.IsHitTestVisible = this.Series.IsHitTestVisible;
            return finalSize;
        }

        /// <summary>
        /// Measures the child elements of a <see cref="T:System.Windows.Controls.Canvas"></see> in anticipation of arranging them during the <see cref="M:System.Windows.Controls.Canvas.ArrangeOverride(System.Windows.Size)"></see> pass.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"></see> that represents the size that is required to arrange child content.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            IChartTransformer transformer = this.CreateTransformer(new Rect(availableSize));
            ////Scope below provides value of real co-ordinates of series midpoint.
            IChartData data = Series.Data;
            if (data != null && Series != null)
            {
                if (Series.ChartType != null && Series.XAxis != null && Series.YAxis != null)
                {
                    if (data != null && data.Count > 0 && Series.ChartType.AxesType == ChartAxesType.CartesianAxes)
                    {
                        ////Retrieving middle point of series.
                        IChartDataPoint point = data[(int)(data.Count / 2)];
                        Point centerPoint = new Point();
                        ////Getting X and Y real point co-ordinates.
                        centerPoint.X = Series.XAxis.ValueToCoefficient(point.X) * transformer.Viewport.Width;
                        centerPoint.Y = (1 - Series.YAxis.ValueToCoefficient(point.Y)) * transformer.Viewport.Height;
                        ////Setting readonly CenterPoint property.
                        Series.SetValue(ChartSeries.CenterPointPropertyKey, centerPoint);
                    }
                }
            }

            this.TotalSize = availableSize;
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"></see>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is raised.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return m_visibleElements[index];
        }

        /// <summary>
        /// Called when [segments changed].
        /// </summary>
        /// <param name="dObj">The d obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSeriesChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs args)
        {
            ChartSeriesPresenter seriesPresenter = dObj as ChartSeriesPresenter;

            if (seriesPresenter != null)
            {
                if (args.OldValue is ChartSeries)
                {
                    (args.OldValue as ChartSeries).Segments.CollectionChanged -= new NotifyCollectionChangedEventHandler(seriesPresenter.ChartSegmentsChanged);
                }

                ChartSeries series = args.NewValue as ChartSeries;
                if (series != null && series.Segments != null)
                {
                    (args.NewValue as ChartSeries).Segments.CollectionChanged += new NotifyCollectionChangedEventHandler(seriesPresenter.ChartSegmentsChanged);
                }
                if (series != null && series.Indicators != null)
                {
                    (args.NewValue as ChartSeries).Indicators.Items.CollectionChanged += new NotifyCollectionChangedEventHandler(seriesPresenter.Items_CollectionChanged);
                }

                seriesPresenter.ResetSegments();
            }
        }

        /// <summary>
        /// Called when axes is changed.
        /// </summary>
        /// <param name="dObj">The d obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAxeschanged(DependencyObject dObj, DependencyPropertyChangedEventArgs args)
        {
            ChartSeriesPresenter canvas = dObj as ChartSeriesPresenter;

            if (canvas != null)
            {
                if (args.OldValue != null)
                {
                    (args.OldValue as ChartAxis).Changed -= new EventHandler(canvas.OnVisibleRangeChanged);
                }

                if (args.NewValue != null)
                {
                    (args.NewValue as ChartAxis).Changed += new EventHandler(canvas.OnVisibleRangeChanged);
                }
            }
        }

        /// <summary>
        /// Invalidates the children visibility.
        /// </summary>
        private void InvalidateChildrenVisibility()
        {
            foreach (ChartSegmentPresenter segmentPresenter in m_elements)
            {
                bool contains = m_visibleElements.Contains(segmentPresenter);

                if (IsSegmentVisible(segmentPresenter.Segment))
                {
                    if (!contains)
                    {
                        m_visibleElements.Add(segmentPresenter);
                    }
                    else
                    {
                        segmentPresenter.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (contains)
                    {
                        // m_visibleElements.Remove(segmentPresenter);
                        segmentPresenter.Visibility = Visibility.Collapsed;
                    }
                }
            }
            foreach (IndicatorPresenter presenter in m_indicators)
            {
                if (!m_visibleElements.Contains(presenter))
                {
                    m_visibleElements.Add(presenter);
                }
            }
        }

        /// <summary>
        /// Determines whether segment is visible on area.
        /// </summary>
        /// <param name="segment">The segment.</param>
        /// <returns>
        ///   <c>true</c> if segment is  visible; otherwise, <c>false</c>.
        /// </returns>
        private bool IsSegmentVisible(ChartSegment segment)
        {
            DoubleRange XMeasure = DoubleRange.Empty, YMeasure = DoubleRange.Empty;
            if (segment != null && XAxis != null && YAxis != null)
            {
                if (segment.Series != null && XAxis.VisibleRange != null && YAxis.VisibleRange != null)
                {
                    if (segment.Series.Area != null && segment.YDataMeasure != null && segment.XDataMeasure != null)
                    {
                        ////return (segment.Series.Area.AreaType != ChartAxesType.CartesianAxes) || (XAxis.VisibleRange.Intersects(segment.XDataMeasure) && YAxis.VisibleRange.Intersects(segment.YDataMeasure));
                        //Since FastLine is drawn as a single segment so cheking of XDataMeasure for FastLine is Incorrect when range is specified externally. Because for fast line only the AxisXRange/AxisYRange get modified not XDataMeasure. 
                        DoubleRange fastlineXrange= DoubleRange.Empty;
                        DoubleRange fastlineYrange= DoubleRange.Empty;
                        if (XAxis.IsAutoSetRange)
                        {
                            fastlineXrange = segment.XDataMeasure;
                        }
                        else
                        {
                            fastlineXrange = segment.Series.ActualXAxis.InternalRange;
                        }
                        if (YAxis.IsAutoSetRange)
                        {
                            fastlineYrange = segment.YDataMeasure;
                        }
                        else
                        {
                            fastlineYrange = segment.Series.ActualYAxis.InternalRange;
                        }
                        XMeasure = XAxis.IsLogarithmic ? new DoubleRange(Math.Log(segment.XDataMeasure.Start, XAxis.LogarithmicBase), Math.Log(segment.XDataMeasure.End, XAxis.LogarithmicBase)) : segment is ChartFastLineSegment ? new DoubleRange(fastlineXrange.Start, fastlineXrange.End) : new DoubleRange(segment.XDataMeasure.Start, segment.XDataMeasure.End);
                        YMeasure = YAxis.IsLogarithmic ? new DoubleRange(Math.Log(segment.YDataMeasure.Start, YAxis.LogarithmicBase), Math.Log(segment.YDataMeasure.End, YAxis.LogarithmicBase)) : segment is ChartFastLineSegment ? new DoubleRange(fastlineYrange.Start, fastlineYrange.End) : new DoubleRange(segment.YDataMeasure.Start, segment.YDataMeasure.End);

                        return (segment.Series.Area.AreaType != ChartAxesType.CartesianAxes) || (XAxis.VisibleRange.Intersects(XMeasure) && YAxis.VisibleRange.Intersects(YMeasure));
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Creates the transformer.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <returns>The Transformer</returns>
        internal IChartTransformer CreateTransformer(Rect viewport)
        {

            ChartArea area = Series.Area;
            if (ChartFastSeriesPresenter.GetPen(Series) == null)
            {
                //ChartFastSeriesPresenter.SetPen(this, pn);
                //if (Series.Type != ChartTypes.FastLine)
                //{
                //    Series.FastTypePen = new Pen(Series.Stroke, Series.StrokeThickness);
                //}
                if (Series.Type == ChartTypes.FastLine)
                {
                    Series.FastTypePen = new Pen(Series.Interior, Series.StrokeThickness);
                }
                else
                {
                    Series.FastTypePen = new Pen(Series.Stroke, Series.StrokeThickness);
                }
            }
            Series.Presenter = this;
            if (area != null)
            {
                return ChartTransform.CreateTransformer(area.AreaType, viewport, this.Series);
            }
            else
            {
                return null;
            }
        }

        internal void AddIndicators(ChartTechnicalIndicator indicator)
        {
            if (IsAddIndicator == true || IncrementValue < indicator_count)
            {
            IndicatorPresenter presenter = new IndicatorPresenter(indicator);
            presenter.SetValue(ObjectProperty, indicator);
            m_indicators.Add(presenter);
            this.m_IndicatorPresenter = presenter;
            IncrementValue++;
            IsAddIndicator = false;
            }
        }

        /// <summary>
        /// Adds the container.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="segment">The segment.</param>
        /// <seealso cref="ChartSeriesPresenter"/>
        private void AddSegment(int index, ChartSegment segment)
        {
            ChartSegmentPresenter container = new ChartSegmentPresenter(segment as ChartSegment);
            container.SetValue(ObjectProperty, segment);
            m_elements.Insert(index, container);
        }

        internal void RemoveIndicators(object indicator,int i)
        {
            //for (int i = 0; i < m_indicators.Count; i++)
            {
                if (m_indicators[i].Indicator.IndicatorType == (indicator as ChartTechnicalIndicator).IndicatorType)
                {
                    m_visibleElements.Remove(m_indicators[i]);
                    m_indicators.RemoveAt(i);
                    indicpos--;
                }
            }
        }

        /// <summary>
        /// Removes the container.
        /// </summary>
        /// <param name="segment">The segment.</param>
        internal void RemoveSegment(object segment)
        {
            for (int i = 0; i < m_elements.Count; i++)
            {
                if (m_elements[i].GetValue(ObjectProperty) == segment)
                {
                    if (m_elements[i] is ChartSegmentPresenter)
                    {
                        (m_elements[i] as ChartSegmentPresenter).Dispose();
                    }

                    m_elements[i].ClearValue(ObjectProperty);
                    m_visibleElements.Remove(m_elements[i]);
                    m_elements.RemoveAt(i);
                    break;
                }
            }
        }


        /// <summary>
        /// Resets the segments.
        /// </summary>
        private void ResetSegments()
        {
            this.ClearSegments();
            this.ClearIdnicator();
            if (this.Series.Segments != null)
            {
                for (int i = 0, ci = this.Series.Segments.Count; i < ci; i++)
                {
                    this.AddSegment(i, this.Series.Segments[i]);
                }
            }
            if (Series.Indicators != null)
            {
                for (int i = 0; i < this.Series.Indicators.Items.Count; i++)
                {
                    this.Series.Indicators.Items[i].Series = this.Series;
                    this.Series.Indicators.Items[i].VisiblePoints = this.Series.Data;
                    this.AddIndicators(this.Series.Indicators.Items[i]);
                }
            }
        }
        private void ClearIdnicator()
        {
            m_indicators.Clear();
            m_visibleElements.Clear();
        }
        /// <summary>
        /// Clears the segments.
        /// </summary>
        /// <seealso cref="ChartSeriesPresenter"/>
        private void ClearSegments()
        {
            foreach (UIElement element in m_elements)
            {
                if (element is ChartSegmentPresenter)
                {
                    (element as ChartSegmentPresenter).Dispose();
                }

                element.ClearValue(ObjectProperty);
            }

            m_elements.Clear();
            m_visibleElements.Clear();
        }


        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.ClearIdnicator();
                indicpos = -1;
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (object obj in e.OldItems)
                    {
                        this.RemoveIndicators(obj,e.OldStartingIndex);
                    }
                }
                if (e.NewItems != null)
                {
                    //for (int i = 0; i < this.Series.Indicators.Count; i++)
                    {
                        indicpos++;
                        IsAddIndicator = true;
                        if (this.Series.Indicators.Items[indicpos].Series == null || this.Series.Indicators.Items[indicpos].VisiblePoints == null)
                        {
                            this.Series.Indicators.Items[indicpos].Series = this.Series;
                            this.Series.Indicators.Items[indicpos].VisiblePoints = this.Series.Data;
                        }
                        this.AddIndicators(this.Series.Indicators.Items[indicpos]);
                        this.m_IndicatorPresenter.InvalidateVisual();
                        this.InvalidateMeasure();
                    }
                }
            }
        }



        /// <summary>
        /// Charts the segments changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void ChartSegmentsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.ClearSegments();
                this.ClearIdnicator();
                if (this.Series.Indicators != null)
                {
                    indicator_count = this.Series.Indicators.Items.Count;
                    IncrementValue = 0;
                }
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (object obj in e.OldItems)
                    {
                        this.RemoveSegment(obj);
                    }
                }

                if (e.NewItems != null)
                {
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        this.AddSegment(e.NewStartingIndex + i, e.NewItems[i] as ChartSegment);
                       
                        if (Series.Indicators != null && e.NewItems.Count - 1 == i)
                        {
                            for (int j = 0; j < this.Series.Indicators.Items.Count; j++)
                            {
                                    this.Series.Indicators.Items[j].Series = this.Series;
                                    this.Series.Indicators.Items[j].VisiblePoints = this.Series.Data;
                                    this.AddIndicators(this.Series.Indicators.Items[j]);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when visible range is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnVisibleRangeChanged(object sender, EventArgs e)
        {
            this.InvalidateArrange();
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == FlowDirectionProperty)
            {
                ////Setting flow direction for all segment presenters due to they may not be in visual tree.
                foreach (ChartSegmentPresenter segmentPresenter in m_elements)
                {
                    segmentPresenter.FlowDirection = (FlowDirection)e.NewValue;
                }
            }    
            string inter= string.Empty;
            if (this.Series != null && this.Series.Interior != null)
            {
                inter = this.Series.Interior.ToString();
            }
            if (this.Series != null)
            {
                AutomationProperties.SetItemStatus(this, string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + string.Empty + ";" + this.Series.Type.ToString() + ";" + this.Series.IsIndexed.ToString() + ";" + inter + ";");
            }
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            m_elements.Clear();
            m_indicators.Clear();
            //foreach (ChartSegmentPresenter val in m_elements)
            //{
            //    val.Dispose();
            //}
            m_elements = null;
            m_indicators = null;
            m_visibleElements.Clear();
            m_visibleElements = null;
        }

        #endregion
    }
}
