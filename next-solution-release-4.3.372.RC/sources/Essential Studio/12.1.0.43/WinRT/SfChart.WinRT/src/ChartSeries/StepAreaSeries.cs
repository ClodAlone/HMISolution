#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Shapes;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// StepAreaSeries connects its data points,using a continuous line with its underlying areas being filled in.
    /// </summary>
    ///<seealso cref="AreaSegment"/>
    ///<seealso cref="SplineAreaSeries"/>
    ///<seealso cref="StackingAreaSeries"/>
    ///<seealso cref="RangeAreaSeries"/>
    [ClassReference(IsReviewed = false)]
    public class StepAreaSeries : XyDataSeries
    {
        #region ctor

        #endregion

        #region methods

        /// <summary>
        /// Creates the segments of StepAreaSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            var xValues = GetXValues();
            ClearUnUsedAdornments(DataCount);
            if (xValues.Count == 0) return;

            double xOffset = 0d;
            if (ActualXAxis is CategoryAxis &&
                (ActualXAxis as CategoryAxis).LabelPlacement == LabelPlacement.BetweenTicks)
                xOffset = 0.5d;
            var stepAreaPoints = new List<Point>
            {
                new Point((xValues[DataCount - 1] + xOffset), ActualXAxis.Origin),
                new Point(xValues[0] - xOffset, ActualXAxis.Origin)
            };

            for (int i = 0; i < DataCount; i++)
            {
                stepAreaPoints.Add(new Point(xValues[i] - xOffset, YValues[i]));
                if (i != DataCount - 1)
                {
                    stepAreaPoints.Add(new Point(xValues[i + 1] - xOffset, YValues[i]));
                }
            }
            if (xOffset > 0)
                stepAreaPoints.Add(new Point((xValues[DataCount - 1]) + xOffset, YValues[DataCount - 1]));

            if (Segments.Count == 0)
            {
                Segments.Add(new StepAreaSegment(stepAreaPoints, this));
            }
            else
            {
                Segments[0].SetData(stepAreaPoints);
            }

            if (AdornmentsInfo != null)
                AddAreaAdornments(YValues);
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new StepAreaSeries());
        }


        /// <summary>
        /// Timer Tick Handler for closing the Tooltip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, object e)
        {
            RemoveTooltip();
            timer.Stop();
        }

        internal override void RemoveTooltip()
        {
            var canvas = this.Area.GetAdorningCanvas();
            if (canvas.Children.Contains((this.Area.Tooltip as ChartTooltip)))
                canvas.Children.Remove(this.Area.Tooltip as ChartTooltip);
        }

        /// <summary>
        /// Event to show tooltip
        /// </summary>
        /// <param name="e"> Event Arguments</param>
#if WPF
        protected override void OnMouseMove(MouseEventArgs e)
#elif WINDOWS_PHONE && !SILVERLIGHT_UNCOMMON
        protected override void OnTap(GestureEventArgs e)
#elif SILVERLIGHT
        protected override void OnMouseMove(MouseEventArgs e)
#else
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
#endif
        {
            if (ShowTooltip)
            {
                if (e.OriginalSource as Shape == null || (e.OriginalSource as Shape).Tag == null)
                    return;
                timer.Start();
                timer.Interval = new TimeSpan(0, 0, 0, 1);
                var canvas = this.Area.GetAdorningCanvas();
#if !NETFX_CORE
                Point mousePos = e.GetPosition(canvas);
#else
                Point mousePos = e.GetCurrentPoint(canvas).Position;
#endif
                double xVal = 0;
                double yVal = 0;
                double stackedYValue = double.NaN;
                object data = null;
                int index = 0;
                if (this.Area.SeriesClipRect.Contains(mousePos))
                {
                    var point = new Point(mousePos.X - this.Area.SeriesClipRect.Left
                                      , mousePos.Y - this.Area.SeriesClipRect.Top);

                    this.FindNearestChartPoint(point, out xVal, out yVal, out stackedYValue);
                    if (double.IsNaN(xVal)) return;
                    index = this.GetXValues().IndexOf(xVal);
                    data = this.ActualData[index];
                }
                if (this.Area.Tooltip == null)
                    this.Area.Tooltip = new ChartTooltip();
                var chartTooltip = this.Area.Tooltip as ChartTooltip;
                ((e.OriginalSource as Shape).Tag as StepAreaSegment).Item = data;
                ((e.OriginalSource as Shape).Tag as StepAreaSegment).XData = xVal;
                ((e.OriginalSource as Shape).Tag as StepAreaSegment).YData = yVal;
                if (chartTooltip != null)
                {
                    if (canvas.Children.Count == 0 || (canvas.Children.Count > 0 && !IsTooltipAvailable(canvas)))
                    {
                        chartTooltip.Content = (e.OriginalSource as Shape).Tag;
                        if (chartTooltip.Content == null)
                            return;
                        canvas.Children.Add(chartTooltip);
                        chartTooltip.ContentTemplate = this.GetTooltipTemplate();
                        chartTooltip.LeftOffset = Position(mousePos, ref chartTooltip).X;
                        chartTooltip.TopOffset = Position(mousePos, ref chartTooltip).Y;
                        chartTooltip.Margin = ChartTooltip.GetTooltipMargin(this);

                        if (ChartTooltip.GetEnableAnimation(this))
                        {
                            SetDoubleAnimation(chartTooltip);
                            storyBoard.Children.Add(leftDoubleAnimation);
                            storyBoard.Children.Add(topDoubleAnimation);
                            Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                            Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
#if WPF || NETFX_CORE
                            _stopwatch.Start();
#endif
                        }
                        else
                        {
                            Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                            Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
#if WPF || NETFX_CORE
                            _stopwatch.Start();
#endif
                        }
                    }
                    else
                    {
                        foreach (var child in canvas.Children)
                        {
                            if (child is ChartTooltip)
                                chartTooltip = child as ChartTooltip;
                        }
                        chartTooltip.Content = (e.OriginalSource as Shape).Tag;
                        if (chartTooltip.Content == null)
                        {
                            RemoveTooltip();
                            return;
                        }
                        chartTooltip.LeftOffset = Position(mousePos, ref chartTooltip).X;
                        chartTooltip.TopOffset = Position(mousePos, ref chartTooltip).Y;
                        chartTooltip.Margin = ChartTooltip.GetTooltipMargin(this);
#if NETFX_CORE
                        if (_stopwatch.ElapsedMilliseconds > 100)
                        {
#endif
                        if (ChartTooltip.GetEnableAnimation(this))
                        {
#if WPF
                            if (_stopwatch.ElapsedMilliseconds > 100)
                            {
#endif

#if WPF || NETFX_CORE
                                _stopwatch.Restart();
#endif
                                if (leftDoubleAnimation == null || topDoubleAnimation == null || storyBoard == null)
                                {
                                    SetDoubleAnimation(chartTooltip);
                                }
                                else
                                {
                                    leftDoubleAnimation.To = chartTooltip.LeftOffset;
                                    topDoubleAnimation.To = chartTooltip.TopOffset;
                                    storyBoard.Begin();
                                }
#if WPF
                            }
#endif
                        }
                        else
                        {
#if WPF || NETFX_CORE
                            _stopwatch.Restart();
#endif
                            Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                            Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
                        }
#if NETFX_CORE
                        }
                        else if (EnableAnimation == false)
                        {
                            Canvas.SetLeft(chartTooltip, chartTooltip.LeftOffset);
                            Canvas.SetTop(chartTooltip, chartTooltip.TopOffset);
                        }
#endif
                    }
                }
            }
        }
        #endregion
    }
}
