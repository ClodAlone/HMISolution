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
using System.Diagnostics;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

#else
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// StepLineSeries displays its data points using line segments.
    /// </summary>
    /// <seealso cref="StepAreaSeries"/>    
    [ClassReference(IsReviewed = false)]
    public class StepLineSeries : XyDataSeries
    {
        #region ctor

        #endregion

        #region Properties

        /// <summary>
        /// Gets or Sets the DataTemplate for line segment.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public DataTemplate CustomTemplate
        {
            get { return (DataTemplate)GetValue(CustomTemplateProperty); }
            set { SetValue(CustomTemplateProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CustomTemplateProperty =
            DependencyProperty.Register("CustomTemplate", typeof(DataTemplate), typeof(StepLineSeries), new PropertyMetadata(null));


        #endregion

        #region methods

        /// <summary>
        /// Creates the segments of StepLineSeries.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            List<double> xValues = GetXValues();

            if (xValues == null) return;
            ClearUnUsedStepLineSegment(DataCount);
            ClearUnUsedAdornments(DataCount);
            double xOffset = 0d;
            if (ActualXAxis is CategoryAxis &&
                (ActualXAxis as CategoryAxis).LabelPlacement == LabelPlacement.BetweenTicks)
                xOffset = 0.5d;
            for (int i = 0; i < DataCount; i++)
            {

                int index = i + 1;
                Point point1, point2, stepPoint;

                if (AdornmentsInfo != null )
                {
                    if (i < Adornments.Count)
                    {
                        Adornments[i].SetData(xValues[i], YValues[i], xValues[i], YValues[i]);
                        Adornments[i].Item = ActualData[i];
                    }
                    else
                    {
                        Adornments.Add(this.CreateAdornment(this, xValues[i], YValues[i], xValues[i], YValues[i]));
                        Adornments[i].Item = ActualData[i];
                    }
                    
                }
                
                if (index < DataCount)
                {
                    point1 = new Point(xValues[i] - xOffset, YValues[i]);
                    point2 = new Point(xValues[index] - xOffset, YValues[i]);
                    stepPoint = new Point(xValues[index] - xOffset, YValues[index]);
                }
                else
                {
                    if(!(xOffset > 0)) continue;
                    point1 = new Point(xValues[i] - xOffset, YValues[i]);
                    point2 = new Point(xValues[i] + xOffset, YValues[i]);
                    stepPoint = new Point(xValues[i] - xOffset, YValues[i]);
                }
                if (i < Segments.Count)
                {
                    Segments[i].SetData(new List<Point> {point1, stepPoint, point2});
                    Segments[i].Item = ActualData[i];
                }
                else
                {
                    Segments.Add(new StepLineSegment(point1, stepPoint, point2, this) { Item = ActualData[i] });
                    
                }
            }

            if (ShowEmptyPoints)
                UpdateEmptyPointSegments(xValues);
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
                timer.Interval = new TimeSpan(0, 0, 0, 2);
                var canvas = this.Area.GetAdorningCanvas();
#if !NETFX_CORE
                Point mousePos = e.GetPosition(canvas);
#else
                Point mousePos = e.GetCurrentPoint(canvas).Position;
#endif
                if (this.Area.Tooltip == null)
                    this.Area.Tooltip = new ChartTooltip();
                var chartTooltip = this.Area.Tooltip as ChartTooltip;
                if (chartTooltip != null)
                {

                    if (canvas.Children.Count == 0 || (canvas.Children.Count > 0 && !IsTooltipAvailable(canvas)))
                    {
                        double xVal = 0;
                        double yVal = 0;
                        double stackValue = double.NaN;
                        var point = new Point(mousePos.X - this.Area.SeriesClipRect.Left, mousePos.Y - this.Area.SeriesClipRect.Top);

                        FindNearestChartPoint(point, out xVal, out yVal, out stackValue);
                        if (double.IsNaN(xVal)) return;
                        var lineSegment = ((Shape)e.OriginalSource).Tag as StepLineSegment;
                        (((Shape)e.OriginalSource).Tag as StepLineSegment).YData = yVal == lineSegment.Y1Value
                                                                                    ? lineSegment.Y1Value
                                                                                    : lineSegment.Y2Value;
                        chartTooltip.Content = ((Shape)e.OriginalSource).Tag;
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
                        }
                    }
                    else
                    {
                        foreach (var child in canvas.Children)
                        {
                            if (child is ChartTooltip)
                                chartTooltip = child as ChartTooltip;
                        }
                        double xVal = 0;
                        double yVal = 0;
                        double stackValue = double.NaN;
                        var point = new Point(mousePos.X - this.Area.SeriesClipRect.Left, mousePos.Y - this.Area.SeriesClipRect.Top);

                        FindNearestChartPoint(point, out xVal, out yVal, out stackValue);
                        if (double.IsNaN(xVal)) return;
                        var lineSegment = ((Shape)e.OriginalSource).Tag as StepLineSegment;
                        (((Shape)e.OriginalSource).Tag as StepLineSegment).YData = yVal == lineSegment.Y1Value
                                                                                    ? lineSegment.Y1Value
                                                                                    : lineSegment.Y2Value;

                        chartTooltip.Content = ((Shape)e.OriginalSource).Tag;
                        chartTooltip.ContentTemplate = this.GetTooltipTemplate();
                        chartTooltip.LeftOffset = Position(mousePos, ref chartTooltip).X;
                        chartTooltip.TopOffset = Position(mousePos, ref chartTooltip).Y;
                        chartTooltip.Margin = ChartTooltip.GetTooltipMargin(this);
#if NETFX_CORE
                        if (_stopwatch.ElapsedMilliseconds > 100)
                        {
#endif
                        if (ChartTooltip.GetEnableAnimation(this)
#if WPF 
 && _stopwatch.ElapsedMilliseconds > 100
#endif
)
                        {

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
                        }
                        else
                        {
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

        protected override void SetDoubleAnimation(ChartTooltip chartTooltip)
        {
            storyBoard = new Storyboard();
            leftDoubleAnimation = new DoubleAnimation
            {
                To = chartTooltip.LeftOffset,
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200)),
                EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(leftDoubleAnimation, chartTooltip);
#if !NETFX_CORE
            Storyboard.SetTargetProperty(leftDoubleAnimation, new PropertyPath("(Canvas.Left)"));
#else
                            Storyboard.SetTargetProperty(leftDoubleAnimation, "(Canvas.Left)");
#endif
            topDoubleAnimation = new DoubleAnimation
            {
                To = chartTooltip.TopOffset,
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200)),
                EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(topDoubleAnimation, chartTooltip);
#if !NETFX_CORE
            Storyboard.SetTargetProperty(topDoubleAnimation, new PropertyPath("(Canvas.Top)"));
#else
                            Storyboard.SetTargetProperty(topDoubleAnimation, "(Canvas.Top)");
#endif
        }

        /// <summary>
        /// Removes the unused segemnets
        /// </summary>
        [ClassReference(IsReviewed = false)]
        private void ClearUnUsedStepLineSegment(int startIndex)
        {
            if (this.Segments.Count >= startIndex && this.Segments.Count!=0)
            {
                int count = this.Segments.Count;

                for (int i = startIndex; i <= count; i++)
                {
                    if (i == count)
                        this.Segments.RemoveAt(startIndex - 1);
                    else
                        this.Segments.RemoveAt(startIndex);
                }
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new StepLineSeries() { CustomTemplate = this.CustomTemplate });
        }

        #endregion
    }
}
