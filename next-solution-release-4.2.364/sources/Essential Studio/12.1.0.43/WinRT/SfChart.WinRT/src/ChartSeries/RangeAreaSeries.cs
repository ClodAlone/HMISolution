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
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    ///<summary>
    ///RangeAreaSeries displays data points as a set of continous lines with the areas between the high value and low value are filled in.
    ///</summary>
    ///<seealso cref="RangeAreaSegment"/>
    ///<seealso cref="RangeColumnSeries"/>
    ///<seealso cref="AreaSeries"/>
    ///<seealso cref="SplineAreaSeries"/>
    [ClassReference(IsReviewed = false)]
    public class RangeAreaSeries : RangeSeriesBase
    {
        #region properties

        internal override bool IsMultipleYPathRequired
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or Sets high value interior
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush HighValueInterior
        {
            get { return (Brush)GetValue(HighValueInteriorProperty); }
            set { SetValue(HighValueInteriorProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for HighValueInterior.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HighValueInteriorProperty =
            DependencyProperty.Register("HighValueInterior", typeof(Brush), typeof(RangeAreaSeries), new PropertyMetadata(null, OnHighValueChanged));

        /// <summary>
        /// Gets or Sets low value interior
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush LowValueInterior
        {
            get { return (Brush)GetValue(LowValueInteriorProperty); }
            set { SetValue(LowValueInteriorProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for LowValueInterior.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LowValueInteriorProperty =
            DependencyProperty.Register("LowValueInterior", typeof(Brush), typeof(RangeAreaSeries), new PropertyMetadata(null, OnLowValueChanged));

        #endregion

        #region constructor

        #endregion

        #region methods

        private static void OnHighValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeAreaSeries rangeAreaSeries = d as RangeAreaSeries;

            foreach (ChartSegment segment in rangeAreaSeries.Segments)
            {
                (segment as RangeAreaSegment).HighValueInterior = rangeAreaSeries.HighValueInterior;
            }
        }

        private static void OnLowValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeAreaSeries rangeAreaSeries = d as RangeAreaSeries;

            foreach (ChartSegment segment in rangeAreaSeries.Segments)
            {
                (segment as RangeAreaSegment).LowValueInterior = rangeAreaSeries.LowValueInterior;
            }
        }       

        /// <summary>
        /// Creates the segments of RangeAreaSeries
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public override void CreateSegments()
        {
            Point point1;
            Point point2;
            Point point3;
            Point point4;

            Point? crossPoint;
            List<Point> segPoints = new List<Point>();

            List<double> xValues = GetXValues();
            Segments.Clear();

            if (AdornmentsInfo != null)
            {
                if (AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.TopAndBottom)
                    ClearUnUsedAdornments(this.DataCount * 2);
                else
                    ClearUnUsedAdornments(this.DataCount);
            }

            if (xValues != null)
            {
                segPoints.Add(new Point(xValues[0], LowValues[0]));
                segPoints.Add(new Point(xValues[0], HighValues[0]));

                int i;
                for (i = 0; i < DataCount - 1; i++)
                {
                    point1 = new Point(xValues[i], LowValues[i]);
                    point2 = new Point(xValues[i + 1], LowValues[i + 1]);
                    point3 = new Point(xValues[i], HighValues[i]);
                    point4 = new Point(xValues[i + 1], HighValues[i + 1]);
                    crossPoint = GetCrossPoint(point1, point2, point3, point4);
                    if (crossPoint != null)
                    {
                        segPoints.Add(crossPoint.Value);
                        Segments.Add(new RangeAreaSegment(segPoints, (LowValues[i] > HighValues[i]), this)
                            {
                                High = HighValues[i],
                                Low = LowValues[i],
                                Item = ActualData[i]
                            });
                        segPoints = new List<Point>();
                        segPoints.Add(crossPoint.Value);
                    }
                    segPoints.Add(point2);
                    segPoints.Add(point4);
                }

                Segments.Add(new RangeAreaSegment(segPoints, (LowValues[i] > HighValues[i]), this)
                    {
                        High = HighValues[i],
                        Low = LowValues[i],
                        Item = ActualData[i]
                    });
            }
            if (AdornmentsInfo != null)
                AddAdornments(xValues, HighValues, LowValues);
        }

        private void AddAdornments(List<double> xValues, IList<double> HighValues, IList<double> LowValues)
        {
            double adornX = 0d, adornHigh = 0d, adornLow = 0d;

            for (int i = 0; i < xValues.Count; i++)
            {
                if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Top)
                {
                    adornX = xValues[i];
                    adornHigh = HighValues[i] < LowValues[i] ? LowValues[i] : HighValues[i];
                    if (i < Adornments.Count)
                    {
                        Adornments[i].SetData(adornX, adornHigh, adornX, adornHigh);
                    }
                    else
                    {
                        Adornments.Add(this.CreateAdornment(this, adornX, adornHigh, adornX, adornHigh));
                    }
                    Adornments[i].Item = ActualData[i];
                }
                else if (this.AdornmentsInfo.AdornmentsPosition == AdornmentsPosition.Bottom)
                {
                    adornX = xValues[i];
                    adornLow = LowValues[i] < HighValues[i] ? LowValues[i] : HighValues[i];
                    if (i < Adornments.Count)
                    {
                        Adornments[i].SetData(adornX, adornLow, adornX, adornLow);
                    }
                    else
                    {
                        Adornments.Add(this.CreateAdornment(this, adornX, adornLow, adornX, adornLow));
                    }
                    Adornments[i].Item = ActualData[i];
                }
                else
                {
                    adornX = xValues[i];
                    adornHigh = HighValues[i];
                    adornLow = LowValues[i];
                    if (i < Adornments.Count / 2)
                    {
                        int j = 2 * i;
                        Adornments[j++].SetData(adornX, adornHigh, adornX, adornHigh);
                        Adornments[j].SetData(adornX, adornLow, adornX, adornLow);
                    }
                    else
                    {
                        Adornments.Add(this.CreateAdornment(this, adornX, adornHigh, adornX, adornHigh));
                        Adornments.Add(this.CreateAdornment(this, adornX, adornLow, adornX, adornLow));
                    }
                    int k = 2 * i;
                    Adornments[k++].Item = ActualData[i];
                    Adornments[k].Item = ActualData[i];
                }
            }
        }

        /// <summary>
        /// Gets the cross point.
        /// </summary>
        /// <param name="p11">The P11 value.</param>
        /// <param name="p12">The P12 value.</param>
        /// <param name="p21">The P21 value.</param>
        /// <param name="p22">The P22 value.</param>
        /// <returns>The CrossPoint</returns>
        protected Point? GetCrossPoint(Point p11, Point p12, Point p21, Point p22)
        {
            Point pt = new Point();
            double z = (p12.Y - p11.Y) * (p21.X - p22.X) - (p21.Y - p22.Y) * (p12.X - p11.X);
            double ca = (p12.Y - p11.Y) * (p21.X - p11.X) - (p21.Y - p11.Y) * (p12.X - p11.X);
            double cb = (p21.Y - p11.Y) * (p21.X - p22.X) - (p21.Y - p22.Y) * (p21.X - p11.X);

            if ((z == 0) && (ca == 0) && (cb == 0))
            {
                return null;
            }

            double ua = ca / z;
            double ub = cb / z;

            pt.X = p11.X + (p12.X - p11.X) * ub;
            pt.Y = p11.Y + (p12.Y - p11.Y) * ub;

            if ((0 <= ua) && (ua <= 1) && (0 <= ub) && (ub <= 1))
            {
                return pt;
            }
            else
            {
                return null;
            }
        }

        protected override DependencyObject CloneSeries(DependencyObject obj)
        {
            return base.CloneSeries(new RangeAreaSeries() { HighValueInterior = this.HighValueInterior, LowValueInterior = this.LowValueInterior });
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
                ((e.OriginalSource as Shape).Tag as RangeAreaSegment).Item = data;
                if (this.ActualSeriesYValues.Count() > 1)
                {
                    (((Shape) e.OriginalSource).Tag as RangeAreaSegment).High = this.ActualSeriesYValues[0][index];
                    (((Shape) e.OriginalSource).Tag as RangeAreaSegment).Low = this.ActualSeriesYValues[1][index];
                }
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
