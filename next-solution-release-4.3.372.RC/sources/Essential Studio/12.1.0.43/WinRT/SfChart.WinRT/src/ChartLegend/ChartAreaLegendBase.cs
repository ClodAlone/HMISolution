#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Collections.Specialized;
using System.Windows.Media;
#else
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Markup;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Specialized;
using Windows.UI.Xaml.Input;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public partial class ChartBase
    {
        internal IList<UIElement> LegendCollection;

        internal ObservableCollection<LegendItem> LegendItems = new ObservableCollection<LegendItem>();

        /// <summary>
        /// Gets or sets the legend.
        /// </summary>
        /// <value>
        /// The legend.
        /// </value>
        public object Legend
        {
            get { return (object)GetValue(LegendProperty); }
            set { SetValue(LegendProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Legend.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendProperty =
            DependencyProperty.Register("Legend", typeof(object), typeof(ChartBase), new PropertyMetadata(null, OnLegendPropertyChanged));

        /// <summary>
        /// Updates the legend arrange rect.
        /// </summary>
        /// <param name="legend">The legend.</param>
        internal void UpdateLegendArrangeRect(ChartLegend legend)
        {
            if (legend == null && RootPanelDesiredSize == null) return;
            var elemSize = legend.DesiredSize;
            if (Legend is IList && AreaType == ChartAreaType.CartesianAxes)
            {
                if (legend.XAxis == null || legend.YAxis == null) return;

                if (legend.InternalDockPosition == ChartDock.Floating)
                {
                        var xAxis = legend.XAxis;
                        var yAxis = legend.YAxis;
                        var rect = new Rect(xAxis.ArrangeRect.X, yAxis.ArrangeRect.Y, xAxis.ArrangeRect.Width, yAxis.ArrangeRect.Height);
                        UpdateLegendInside(legend, rect);                   
                }
            }
            else
            {
                var actualRect = AreaType == ChartAreaType.None ? new Rect(0, 0, RootPanelDesiredSize.Value.Width, RootPanelDesiredSize.Value.Height) : SeriesClipRect;
                switch (legend.InternalDockPosition)
                {
                    case ChartDock.Top:
                        legend.ArrangeRect = new Rect(actualRect.Left, actualRect.Top - AxisThickness.Top, actualRect.Width, elemSize.Height);
                        break;
                    case ChartDock.Left:
                        legend.ArrangeRect = new Rect(actualRect.Left - AxisThickness.Left, actualRect.Top, elemSize.Width, actualRect.Height);
                        break;
                    case ChartDock.Right:
                        legend.ArrangeRect = new Rect(actualRect.Width + actualRect.Left + AxisThickness.Right, 0, elemSize.Width, actualRect.Height);
                        break;
                    case ChartDock.Bottom:
                        legend.ArrangeRect = new Rect(actualRect.Left, actualRect.Bottom + AxisThickness.Bottom, actualRect.Width, elemSize.Height);
                        break;
                    case ChartDock.Floating:
                        {
                            UpdateLegendInside(legend, AreaType == ChartAreaType.CartesianAxes ? seriesClipRect : actualRect);
                            break;
                        }
                }
            }
        }

        private static void OnLegendPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var area = d as ChartBase;
            if (area != null)
            {
                area.OnLegendPropertyChanged(e);
            }
        }

        private void OnLegendPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateLegend(e.NewValue, true);
        }

        /// <summary>
        /// Updates the legend arrange rect.
        /// </summary>
        internal void UpdateLegendArrangeRect()
        {
            if (LegendCollection == null) return;
            foreach (var legend in LegendCollection.OfType<ChartLegend>())
            {
                UpdateLegendArrangeRect(legend);
            }
            var axisLayout = ChartAxisLayoutPanel as ChartCartesianAxisLayoutPanel;
            if (axisLayout != null)
                axisLayout.UpdateLegendsArrangeRect();
        }
        private void UpdateLegendInside(ChartLegend legend, Rect rect)
        {
            double x = 0d, y = 0d, width = 0d, height = 0d;
            switch (legend.DockPosition)
            {
                case ChartDock.Bottom:
                    y = rect.Y + rect.Height - legend.DesiredSize.Height;
                    x = GetHorizontalLegendAlignment(legend.HorizontalAlignment, rect, legend.DesiredSize);
                    width = rect.Width;
                    height = legend.DesiredSize.Height;
                    break;
                case ChartDock.Top:
                    y = rect.Y;
                    x = GetHorizontalLegendAlignment(legend.HorizontalAlignment, rect, legend.DesiredSize);
                    width = rect.Width;
                    height = legend.DesiredSize.Height;
                    break;
                case ChartDock.Right:
                    y = GetVerticalLegendAlignment(legend.VerticalAlignment, rect, legend.DesiredSize);
                    x = rect.X + rect.Width - legend.DesiredSize.Width;
                    width = legend.DesiredSize.Width;
                    height = rect.Height;
                    break;
                case ChartDock.Left:
                    y = GetVerticalLegendAlignment(legend.VerticalAlignment, rect, legend.DesiredSize);
                    x = rect.X;
                    width = legend.DesiredSize.Width;
                    height = rect.Height;
                    break;
                case ChartDock.Floating:
                    x = rect.Left;
                    y = rect.Top;
                    width = legend.DesiredSize.Width;
                    height = legend.DesiredSize.Height;
                    break;
            }
            legend.ArrangeRect = new Rect(x, y, width, height);
        }

        private double GetHorizontalLegendAlignment(HorizontalAlignment alignment, Rect rect, Size desiredSize)
        {
            var left = rect.Left;
            if (alignment == HorizontalAlignment.Center)
            {
                left = left + (rect.Width / 2) - desiredSize.Width / 2;
            }
            else if (alignment == HorizontalAlignment.Right)
            {
                left = left + rect.Width - desiredSize.Width;
            }
            return left;
        }

        private double GetVerticalLegendAlignment(VerticalAlignment alignment, Rect rect, Size desiredSize)
        {
            var top = rect.Top;
            if (alignment == VerticalAlignment.Center)
            {
                top = top + (rect.Height / 2) - desiredSize.Height / 2;
            }
            else if (alignment == VerticalAlignment.Bottom)
            {
                top = top + rect.Height - desiredSize.Height;
            }
            return top;
        }

        internal void LayoutLegends()
        {
            for (int i = 0; i < RowDefinitions.Count; i++)
            {
                int leftIndex = 0, rightIndex = 0, currLeftPos = 0, currRightPos = 0;
                RowDefinitions[i].Legends.Clear();

                foreach (ChartLegend item in LegendCollection)
                {
                    if (GetActualRow(item) == i && item.LegendPosition != LegendPosition.Inside)
                    {
                        if (item.DockPosition == ChartDock.Left)
                        {
                            RowDefinitions[i].Legends.Add(item);
                            if (leftIndex < currLeftPos)
                                leftIndex++;
                            item.RowColumnIndex = leftIndex;
                            currLeftPos++;
                        }
                        else if (item.DockPosition == ChartDock.Right)
                        {
                            RowDefinitions[i].Legends.Add(item);
                            if (rightIndex < currRightPos)
                                rightIndex++;
                            item.RowColumnIndex = rightIndex;
                            currRightPos++;
                        }
                        if (ChartDockPanel.GetDock(item) != item.InternalDockPosition)
                            ChartDockPanel.SetDock(item, item.InternalDockPosition);
                    }
                }
            }

            for (int i = 0; i < ColumnDefinitions.Count; i++)
            {
                int topIndex = 0, bottomIndex = 0, currTopIndex = 0, currBottomIndex = 0;
                ColumnDefinitions[i].Legends.Clear();

                foreach (ChartLegend item in LegendCollection)
                {
                    if (GetActualColumn(item) == i && item.LegendPosition != LegendPosition.Inside)
                    {
                        if (item.DockPosition == ChartDock.Top)
                        {
                            if (topIndex < currTopIndex)
                                topIndex++;
                            item.RowColumnIndex = topIndex;
                            currTopIndex++;
                            ColumnDefinitions[i].Legends.Add(item);
                        }
                        else if (item.DockPosition == ChartDock.Bottom)
                        {
                            if (bottomIndex < currBottomIndex)
                                bottomIndex++;
                            item.RowColumnIndex = bottomIndex;
                            currBottomIndex++;
                            ColumnDefinitions[i].Legends.Add(item);
                        }
                    }
                    if (ChartDockPanel.GetDock(item) != item.InternalDockPosition)
                        ChartDockPanel.SetDock(item, item.InternalDockPosition);
                }
            }
        }

        internal void UpdateLegend(object newLegend, bool isCollectionChanged)
        {
            try
            {
                if (ChartDockPanel == null) return;
                if (LegendCollection != null && isCollectionChanged)
                {
                    foreach (var item in LegendCollection.Where(item => ChartDockPanel.Children.Contains(item)))
                    {
                        ChartDockPanel.Children.Remove(item);
                    }
                }

                if (newLegend == null) return;

                LegendCollection = new List<UIElement>();

                var legendColl = newLegend as ChartLegendCollection;

                if (legendColl != null)
                {
                    legendColl.CollectionChanged -= LegendCollectionChanged;
                    legendColl.CollectionChanged += LegendCollectionChanged;

                    foreach (var item in legendColl)
                    {
                        LegendCollection.Add(item);
                    }
                    LayoutLegends();
                }
                else
                {
                    LegendCollection.Add(newLegend as UIElement);
                }
                
                foreach (var item in LegendCollection)
                {
                    if (!ChartDockPanel.Children.Contains(item))
                    {
                        ChartDockPanel.Children.Add(item);
                    }
                    SetLegendItemsSource(item as ChartLegend);
                }
            }
            catch
            {
            }
        }

        internal void SetLegendItemsSource(ChartLegend legend)
        {
            if (legend == null || ActualSeries == null) return;
            LegendItems = new ObservableCollection<LegendItem>();
            legend.ChartArea = this;
            if (VisibleSeries.Count == 1 && ((VisibleSeries[0] is AccumulationSeriesBase || VisibleSeries[0] is CircularSeriesBase3D)))
            {
                var k = 0;
                foreach (var item in VisibleSeries[0].Segments.Select(segment => new LegendItem
                {
                    Index = k,
                    Series = ActualSeries[0],
                    Item = segment.Item,
                    Legend = legend,
                    Label = ActualSeries[0].GetActualXValue(k).ToString()
                }))
                {
                    LegendItems.Add(item);
                    k++;
                }
                legend.ItemsSource = LegendItems;
            }
            else
            {
                var technicalIndicators = new List<ChartSeriesBase>();

                if (this is SfChart)
                {
                    foreach (ChartSeries indicator in (this as SfChart).TechnicalIndicators)
                    {
                        technicalIndicators.Add(indicator as ChartSeriesBase);
                    }
                }

                List<ChartSeriesBase> actualLegendSeries = this is SfChart ? ActualSeries.Union(technicalIndicators).ToList() : ActualSeries;
                if (AreaType == ChartAreaType.PolarAxes || AreaType == ChartAreaType.CartesianAxes && Legend is ChartLegendCollection)
                    actualLegendSeries = (from actualSeries in actualLegendSeries
                                          where
                                              GetActualRow(legend) == GetActualRow(actualSeries.IsActualTransposed ? actualSeries.ActualXAxis : actualSeries.ActualYAxis) &&
                                              GetActualColumn(legend) == GetActualColumn(actualSeries.IsActualTransposed ? actualSeries.ActualYAxis : actualSeries.ActualXAxis)
                                          select actualSeries).ToList();

                if (actualLegendSeries.Count > 0 && actualLegendSeries[0] is ISupportAxes)
                {
                    legend.XAxis = actualLegendSeries[0].IsActualTransposed ? (actualLegendSeries[0] as ISupportAxes).ActualYAxis : (actualLegendSeries[0] as ISupportAxes).ActualXAxis;
                    legend.YAxis = actualLegendSeries[0].IsActualTransposed ? (actualLegendSeries[0] as ISupportAxes).ActualXAxis : (actualLegendSeries[0] as ISupportAxes).ActualYAxis;
                }

                IEnumerable<ChartSeriesBase> legendSeries;
                switch (AreaType)
                {
                    case ChartAreaType.CartesianAxes:

                        legendSeries = from series in actualLegendSeries
                                       where series is ISupportAxes2D || series is ISupportAxes3D
                                       select series;
                        break;
                    case ChartAreaType.PolarAxes:
                        legendSeries = from series in actualLegendSeries
                                       where series is PolarRadarSeriesBase
                                       select series;
                        break;
                    default:
                        legendSeries = from series in actualLegendSeries
                                       where series is AccumulationSeriesBase || series is CircularSeriesBase3D
                                       select series;
                        break;
                }
                var chartSeries = legendSeries as ChartSeries[] ?? legendSeries.ToArray();
                foreach (var item in chartSeries.Where(series => series.VisibilityOnLegend == Visibility.Visible).Select(series => new LegendItem { Legend = legend, Series = series }))
                {
                    LegendItems.Add(item);
                }
                foreach (var item in chartSeries)
                {
                    var cartesian = item as CartesianSeries;
                    if (cartesian != null)
                        foreach (
                            var trend in
                                cartesian.Trendlines.Where(
                                    trendline => trendline.VisibilityOnLegend == Visibility.Visible)
                                    .Select(trendline => new LegendItem {Legend = legend, Trendline = trendline}))
                            LegendItems.Add(trend);
                }
                legend.ItemsSource = LegendItems;
            }
        }

        internal void LegendCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var newLegend = e.NewItems[0] as ChartLegend;
                        SetLegendItemsSource(newLegend);
                        LegendCollection.Add(newLegend);
                        ChartDockPanel.Children.Add(newLegend);
                        LayoutLegends();
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        var removedLegend = e.OldItems[0] as ChartLegend;
                        if (LegendCollection.Contains(removedLegend))
                            LegendCollection.Remove(removedLegend);
                        if (ChartDockPanel.Children.Contains(removedLegend))
                            ChartDockPanel.Children.Remove(removedLegend);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var item in LegendCollection)
                    {
                        ChartDockPanel.Children.Remove(item);
                    }
                    LegendCollection.Clear();
                    break;
            }
        }

    }
}
