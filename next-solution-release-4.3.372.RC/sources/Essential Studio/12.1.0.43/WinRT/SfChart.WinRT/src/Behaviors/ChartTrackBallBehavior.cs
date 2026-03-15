#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Globalization;
#else
using Windows.UI;
using Windows.UI.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using Windows.Devices.Input;
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// ChartTrackBallBehavior enables tracking of data points nearer to mouse over position or at touch contact point in a Chart.
    /// </summary>
    /// <remarks>
    /// ChartTrackBallBehavior displays a vertical line,a tracker ball symbol and a popup like control displaying information about the data point, at mouse move positions/ at touch contact positions over a <see cref="ChartSeriesBase"/>.
    /// </remarks>
    public class ChartTrackBallBehavior : ChartBehavior
    {
        #region fields

        private bool isActivated;

        private bool isReversed;
        #if !WINDOWS_PHONE
        private int fingerCount = 0;
        #endif
        private Line line;

        private List<FrameworkElement> elements;

        private ObservableCollection<ChartPointInfo> pointInfos;

        private List<ChartTrackBallControl> trackBalls;

        internal string labelXValue;
        internal string labelYValue;
#if WPF || SILVERLIGHT_UNCOMMON
        private bool isLeftButtonPressed;
#endif
#if WINDOWS_PHONE
        bool isTrackBallUpdateDispatched = false;
        bool isTrackBallUpdateDispatching = false;
#else
        IAsyncAction updateTrackBallAction;
#endif

        internal Point CurrentPoint { get; set; }

        #endregion

        #region properties

        /// <summary>
        /// Gets the collection of ChartPointInfo.
        /// </summary>
        public ObservableCollection<ChartPointInfo> PointInfos
        {
            get
            {
                return pointInfos;
            }
            internal set
            {
                pointInfos = value;
            }
        }
        /// <summary>
        /// Get or Set IsActivated
        /// </summary>
        protected internal bool IsActivated
        {
            get
            {
                return isActivated;
            }
            set
            {
                isActivated = value;
                Activate(isActivated);
            }
        }
        
        /// <summary>
        /// Gets or Sets the alignment for the label appearing in axis.
        /// </summary>
        public ChartAlignment AxisLabelAlignment
        {
            get { return (ChartAlignment)GetValue(AxisLabelAlignmentProperty); }
            set { SetValue(AxisLabelAlignmentProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for AxisLabelAlignment.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AxisLabelAlignmentProperty =
            DependencyProperty.Register("AxisLabelAlignment", typeof(ChartAlignment), typeof(ChartTrackBallBehavior), new PropertyMetadata(ChartAlignment.Center));

        /// <summary>
        /// Gets or Sets the EnableAnimation
        /// </summary>
        private bool EnableAnimation
        {
            get { return (bool)GetValue(EnableAnimationProperty); }
            set { SetValue(EnableAnimationProperty, value); }
        }

       
      /// <summary>
        /// Using a DependencyProperty as the backing store for EnableAnimation.  This enables animation, styling, binding, etc...
      /// </summary>
        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(ChartTrackBallBehavior ), new PropertyMetadata(false));

        /// <summary>
        /// Gets or Sets the line style
        /// </summary>
        public Style LineStyle
        {
            get { return (Style)GetValue(LineStyleProperty); }
            set { SetValue(LineStyleProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for LineStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LineStyleProperty =
            DependencyProperty.Register("LineStyle", typeof(Style), typeof(ChartTrackBallBehavior), new PropertyMetadata(ChartDictionaries.GenericCommonDictionary["trackBallLineStyle"]));
        
        /// <summary>
        /// Gets or Sets a value indicating whether to show/hide line.
        /// </summary>
        public bool ShowLine
        {
            get { return (bool)GetValue(ShowLineProperty); }
            set { SetValue(ShowLineProperty, value); }
        }

       
        /// <summary>
        ///  Using a DependencyProperty as the backing store for ShowLine.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowLineProperty =
            DependencyProperty.Register("ShowLine", typeof(bool), typeof(ChartTrackBallBehavior), new PropertyMetadata(true,OnShowLinePropertyChanged));
        private static void OnShowLinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ChartTrackBallBehavior).OnShowLinePropertyChanged(e);
        }

        private void OnShowLinePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                if ((bool)e.NewValue)
                    AttachElements();
                else if (this.AdorningCanvas != null)
                {
                    DetachElement(line);
                    elements.Remove(line);
                }
            }
        }

        
        /// <summary>
        /// Gets or Sets vertical alignment for label.
        /// </summary>
        public ChartAlignment LabelVerticalAlignment
        {
            get { return (ChartAlignment)GetValue(LabelVerticalAlignmentProperty); }
            set { SetValue(LabelVerticalAlignmentProperty, value); }
        }

      
        /// <summary>
        /// Using a DependencyProperty as the backing store for VerticalAlignment.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelVerticalAlignmentProperty =
            DependencyProperty.Register("LabelVerticalAlignment", typeof(ChartAlignment), typeof(ChartTrackBallBehavior), new PropertyMetadata(ChartAlignment.Center));
        
        /// <summary>
        /// Gets or Sets horizontal alignment for label.
        /// </summary>
        public ChartAlignment LabelHorizontalAlignment
        {
            get { return (ChartAlignment)GetValue(LabelHorizontalAlignmentProperty); }
            set { SetValue(LabelHorizontalAlignmentProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for HorizontalAlignment.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LabelHorizontalAlignmentProperty =
            DependencyProperty.Register("LabelHorizontalAlignment", typeof(ChartAlignment), typeof(ChartTrackBallBehavior), new PropertyMetadata(ChartAlignment.Far));

        /// <summary>
        /// Gets  or Sets the style for ChartTrackBallControl.
        /// </summary>
        public Style ChartTrackBallStyle
        {
            get { return (Style)GetValue(ChartTrackBallStyleProperty); }
            set { SetValue(ChartTrackBallStyleProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for SymbolTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ChartTrackBallStyleProperty =
            DependencyProperty.Register("ChartTrackBallStyle", typeof(Style), typeof(ChartTrackBallBehavior), null);
        
        private List<ContentControl> labelElements;

        private List<ContentControl> axisLabelElements;

        private Dictionary<ChartAxis, ChartPointInfo> axisLabels;

        #endregion

        #region ctor

        /// <summary>
        /// Called when instance created for ChartrackBallBehaviour
        /// </summary>
        public ChartTrackBallBehavior()
        {
            elements = new List<FrameworkElement>();
            pointInfos = new ObservableCollection<ChartPointInfo>();
            line = new Line();
            Binding binding = new Binding();
            binding.Path = new PropertyPath("LineStyle");
            binding.Source = this;
            line.SetBinding(Line.StyleProperty, binding);
            labelElements = new List<ContentControl>();
            axisLabelElements = new List<ContentControl>();
            axisLabels = new Dictionary<ChartAxis, ChartPointInfo>();

            trackBalls = new List<ChartTrackBallControl>();
        }

        #endregion

        #region methods

        /// <summary>
        /// Method implementation for AttachElements
        /// </summary>
        protected override void AttachElements()
        {
            if (this.AdorningCanvas != null && !AdorningCanvas.Children.Contains(line) && this.ShowLine)
            {
                AdorningCanvas.Children.Add(line);
                elements.Add(line);
            }
        }

        /// <summary>
        /// Method implementation for DetachElements
        /// </summary>
        internal protected override void DetachElements()
        {
            if (this.AdorningCanvas != null && AdorningCanvas.Children.Contains(line))
            {
                AdorningCanvas.Children.Remove(line);
                elements.Remove(line);
            }

            foreach (var element in elements)
            {
                AdorningCanvas.Children.Remove(element);
            }
        }

        /// <summary>
        /// Called when Size Changed
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnSizeChanged(SizeChangedEventArgs e)
        {
            double y1 = this.ChartArea.ValueToLogPoint(this.ChartArea.InternalSecondaryAxis, (Convert.ToDouble(labelYValue)));
            double x1 = this.ChartArea.ValueToLogPoint(this.ChartArea.InternalPrimaryAxis, (Convert.ToDouble(labelXValue)));
            if (!double.IsNaN(y1) && !double.IsNaN(x1))
            {
                //foreach (ContentControl control in labelElements)
                //{
                //    if (this.AdorningCanvas.Children.Contains(control))
                //        this.AdorningCanvas.Children.Remove(control);
                //}

                //foreach (ContentControl control in axisLabelElements)
                //{
                //    if (this.AdorningCanvas.Children.Contains(control))
                //        this.AdorningCanvas.Children.Remove(control);
                //}

                //foreach (Control control in trackBalls)
                //{
                //    if (this.AdorningCanvas.Children.Contains(control))
                //        this.AdorningCanvas.Children.Remove(control);
                //}
                //PointInfos.Clear();

                //labelElements.Clear();

                //axisLabelElements.Clear();

                //trackBalls.Clear();

                //axisLabels.Clear();

                ClearItems();

                CurrentPoint = new Point(x1, y1);
                //OnPointerPositionChanged();
                ScheduleTrackBallUpdate();
            }
        }
#if !WPF

#if WINDOWS_PHONE
        /// <summary>
        /// Called when Hold the pointer in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnHold(System.Windows.Input.GestureEventArgs e)
#else
        /// <summary>
        /// Called when Holding the Focus in UIElement
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnHolding(HoldingRoutedEventArgs e)
#endif
        {
            IsActivated = true;
#if !WINDOWS_PHONE
            if (e.PointerDeviceType == PointerDeviceType.Touch)
#endif
                ChartArea.HoldUpdate = true;

                if (this.ChartArea != null && IsActivated)
                {
                    Point point = e.GetPosition(this.AdorningCanvas);

                    if (ChartArea.SeriesClipRect.Contains(point))
                    {
                        point = new Point(point.X - ChartArea.SeriesClipRect.Left
                        , point.Y - ChartArea.SeriesClipRect.Top);

                        CurrentPoint = point;
                        OnPointerPositionChanged();
                    }
                }
        }      

#endif
        /// <summary>
        /// Method implementation for Clearitems in ChartTrackBallbehaviour
        /// </summary>
        protected void ClearItems()
        {
            foreach (ContentControl control in labelElements)
            {
                if (this.AdorningCanvas.Children.Contains(control))
                    this.AdorningCanvas.Children.Remove(control);
            }

            foreach (ContentControl control in axisLabelElements)
            {
                if (this.AdorningCanvas.Children.Contains(control))
                    this.AdorningCanvas.Children.Remove(control);
            }

            foreach (Control control in trackBalls)
            {
                if (this.AdorningCanvas.Children.Contains(control))
                    this.AdorningCanvas.Children.Remove(control);
            }

            foreach (ChartTrackBallControl control in trackBalls)
            {
                control.Series = null;
            }

            PointInfos.Clear();

            elements.Clear();

            labelElements.Clear();

            axisLabelElements.Clear();

            trackBalls.Clear();

            axisLabels.Clear();

            line.ClearUIValues();
        }
#if !WINDOWS_PHONE

        /// <summary>
        /// Called when Pointer pressed in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
                IsActivated = false;
            else
                fingerCount++;
        }
#endif

#if WPF || SILVERLIGHT_UNCOMMON
        protected internal override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            isLeftButtonPressed = true;
            IsActivated = false;
        }
#endif
        /// <summary>
        /// Called when layout updated in chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnLayoutUpdated()
        {
            if (IsActivated)
                ScheduleTrackBallUpdate();
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Called when MouseMove in chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnMouseMove(MouseEventArgs e)
#else
        /// <summary>
        /// Called when Pointer moved in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal sealed override void OnPointerMoved(PointerRoutedEventArgs e)
#endif
        {
#if WPF || SILVERLIGHT_UNCOMMON
            if (!isLeftButtonPressed)
                IsActivated = true; 
#endif

#if !WINDOWS_PHONE

            PointerPoint pointer = e.GetCurrentPoint(this.AdorningCanvas);
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Touch)
                if (fingerCount > 1) return;
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse && 
                !pointer.Properties.IsLeftButtonPressed)
                IsActivated = true;
#endif

            if (this.ChartArea != null && this.ChartArea.AreaType == ChartAreaType.CartesianAxes && IsActivated)
            {
#if WINDOWS_PHONE
                Point point = e.GetPosition(this.AdorningCanvas);
#else
                Point point = new Point(pointer.Position.X, pointer.Position.Y);
#endif

                if (ChartArea.SeriesClipRect.Contains(point))
                {
                   

                    point = new Point(point.X - ChartArea.SeriesClipRect.Left
                    , point.Y - ChartArea.SeriesClipRect.Top);

                    CurrentPoint = point;
                    //OnPointerPositionChanged();
                    ScheduleTrackBallUpdate();
                }
            }
        }

        internal void ScheduleTrackBallUpdate()
        {
#if WINDOWS_PHONE
            if (!isTrackBallUpdateDispatched && !isTrackBallUpdateDispatching)
            {
#if WPF 
                isTrackBallUpdateDispatching = true;
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(OnPointerPositionChanged));
#else
                Dispatcher.BeginInvoke(OnPointerPositionChanged);
#endif
                isTrackBallUpdateDispatched = true;
            }
#else
            if (updateTrackBallAction == null)
            {
                updateTrackBallAction = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, OnPointerPositionChanged);
            }
#endif
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Called when MouseLeave from Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnMouseLeave(MouseEventArgs e)
        {
            if (IsActivated)
            {
                IsActivated = false;
                ChartArea.HoldUpdate = false;
            }
        }
#endif

#if WINDOWS_PHONE
        /// <summary>
        /// Called when OnMouse
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
#else
        /// <summary>
        /// Called when Pointer Released in Chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnPointerReleased(PointerRoutedEventArgs e)
#endif
        {
#if !WINDOWS_PHONE     
            if (e.Pointer.PointerDeviceType != PointerDeviceType.Mouse)
            {
                if (IsActivated)
                {
                    IsActivated = false;
                    ChartArea.HoldUpdate = false;
                }
                fingerCount--;
            }
#else
            if (IsActivated)
            {
                IsActivated = false;
                ChartArea.HoldUpdate = false;
            }
#endif

#if WPF || SILVERLIGHT_UNCOMMON
            isLeftButtonPressed = false;
#endif
        }
        /// <summary>
        /// Called when Pointer position Changed
        /// </summary>
        /// <param name="point"></param>
        protected virtual void OnPointerPositionChanged(Point point)
        {
            CurrentPoint = point;
            OnPointerPositionChanged();
        }
        /// <summary>
        ///Called when Pointer position Changed in Chart
        /// </summary>
        protected internal virtual void OnPointerPositionChanged()
        {
#if WINDOWS_PHONE
            isTrackBallUpdateDispatched = false;
#else
            updateTrackBallAction =null;
#endif

            if (!IsActivated)
            {
                return;
            }

            Point point = CurrentPoint;
            int index = 0;
            double leastX = 0;

            IEnumerable<IGrouping<ChartAxis, ChartSeriesBase>> groupedSeries = this.ChartArea.VisibleSeries.GroupBy<ChartSeriesBase, ChartAxis>((series) => series.ActualXAxis);
            ClearItems();
            foreach (IGrouping<ChartAxis, ChartSeriesBase> group in groupedSeries)
            {
                ChartAxis axis = group.Key;
                if (axis == null)
                    continue;
                double leastXPoint = 0, leastYPoint = 0;
                double leastIndex = 0;
                double leastXVal = 0;
                foreach (ChartSeriesBase series in group)
                {
                    if (series.IsActualTransposed)
                    {
                        isReversed = true;
                    }
                    else
                    {
                        isReversed = false;
                    }
                    if(series.DataCount==1 && !series.IsSideBySide)
                    {
                        return;
                    }
                    if (series.DataCount > 0)
                    {
                        double xVal = 0;
                        double yVal = 0;
                        double stackedYValue = double.NaN;
                        double x = 0;
                        double y = 0;
                        bool isStackedSeries = series is StackingSeriesBase;
                        series.FindNearestChartPoint(point, out xVal, out yVal, out stackedYValue);
                        x = this.ChartArea.ValueToLogPoint(series.ActualXAxis, xVal);
                        y = this.ChartArea.ValueToLogPoint(series.ActualYAxis, (isStackedSeries ? stackedYValue : yVal));
                        if (!double.IsNaN(x) && !double.IsNaN(y))
                        {
                            if (index == 0)
                                leastX = x;
                            if (leastIndex == 0)
                            {
                                leastYPoint = y;
                                leastXPoint = x;
                            }
                            if (Math.Abs(leastX - point.X) > Math.Abs(leastX - x))
                            {
                                leastX = x;
                            }

                            if (Math.Abs(leastXPoint - point.X) > Math.Abs(leastXPoint - x))
                            {
                                leastXPoint = x;
                                leastXVal = xVal;
                            }

                            if (Math.Abs(leastYPoint - point.Y) > Math.Abs(leastYPoint - y))
                            {
                                leastYPoint = y;
                            }

                            Rect rect = new Rect(this.ChartArea.SeriesClipRect.Left - 1, this.ChartArea.SeriesClipRect.Top - 1,
                                this.ChartArea.SeriesClipRect.Width + 2, this.ChartArea.SeriesClipRect.Height + 2);
                            if (isReversed)
                            {
                                if (!rect.Contains(new Point(leastYPoint+this.ChartArea.SeriesClipRect.Left
                                    , x + this.ChartArea.SeriesClipRect.Top)))
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                if (!rect.Contains(new Point(leastXPoint + this.ChartArea.SeriesClipRect.Left
                                    , y + this.ChartArea.SeriesClipRect.Top)))
                                {
                                    continue;
                                }
                            }
                            ChartPointInfo pointInfo = new ChartPointInfo();
                            pointInfo.X = x + this.ChartArea.SeriesClipRect.Left;
                            pointInfo.Y = y + this.ChartArea.SeriesClipRect.Top;
                            pointInfo.Series = series;
                            if (series.IsIndexed)
                            {
                                pointInfo.ValueX = series.ActualXAxis.GetLabelContent((int)xVal).ToString();
                                labelXValue = xVal.ToString();
                                pointInfo.SeriesValues.Add(pointInfo.ValueX);
                            }
                            else
                            {
                                pointInfo.ValueX = series.ActualXAxis.GetLabelContent(xVal).ToString();
                                labelXValue = xVal.ToString();
                                pointInfo.SeriesValues.Add(pointInfo.ValueX);
                            }
                            pointInfo.ValueY = yVal.ToString();
                            labelYValue = yVal.ToString();
                            if (series is FinancialSeriesBase && !isReversed)
                            {
                                if (series.ActualSeriesYValues.Count() > 0 && series.ActualSeriesYValues[0].Contains(yVal))
                                {
                                    int indexofy = series.ActualSeriesYValues[0].IndexOf(yVal);
                                    for (int i = 0; i < series.ActualSeriesYValues.Count(); i++)
                                    {
                                        pointInfo.SeriesValues.Add(series.ActualSeriesYValues[i][indexofy].ToString());
                                    }
                                }

                            }
                            else if (series is RangeSeriesBase && !isReversed)
                            {
                                if (series.ActualSeriesYValues.Count() > 0 && series.ActualSeriesYValues[0].Contains(yVal))
                                {
                                    int indexofy = series.ActualSeriesYValues[0].IndexOf(yVal);
                                    for (int i = 0; i < series.ActualSeriesYValues.Count(); i++)
                                    {
                                        pointInfo.SeriesValues.Add(series.ActualSeriesYValues[i][indexofy].ToString());
                                    }
                                }
                            }
                            else if (series is BubbleSeries && !isReversed)
                            {
                                pointInfo.SeriesValues.Add(yVal.ToString());
                                pointInfo.SeriesValues.Add((series.Segments[0] as BubbleSegment).Size.ToString());
                            }
                            else if (isReversed)
                            {

                                pointInfo.SeriesValues.Add(yVal.ToString());
                                pointInfo.X = this.ChartArea.SeriesClipRect.Height - (x + this.ChartArea.SeriesClipRect.Top);
                                pointInfo.Y = y + this.ChartArea.SeriesClipRect.Left;
                            }
                            else
                            {
                                pointInfo.SeriesValues.Add(yVal.ToString());
                            }
                            PointInfos.Add(pointInfo);
                            index++;
                            leastIndex++;
                        }
                    }
                }
                if (PointInfos.Count == 0)
                    continue;
                ChartPointInfo ptInfo = new ChartPointInfo();
                if (isReversed)
                {
                    if (axis.Orientation == Orientation.Vertical)
                    {
                        if (ChartArea.VisibleSeries.Count > 0 && ChartArea.VisibleSeries[0].IsIndexed)
                        {
                            ptInfo.ValueX = axis.GetLabelContent((int)leastXVal).ToString();
                        }
                        else
                        {
                            ptInfo.ValueX = axis.GetLabelContent(leastXVal).ToString();
                        }

                        ptInfo.X = this.ChartArea.SeriesClipRect.Height - (leastXPoint + this.ChartArea.SeriesClipRect.Top);
                    }
                    else
                    {
                        ptInfo.ValueY = axis.GetLabelContent(leastXVal).ToString();
                        ptInfo.Y = point.Y;
                    }
                }
                else
                {
                    if (axis.Orientation == Orientation.Horizontal)
                    {
                        if (ChartArea.VisibleSeries.Count > 0 && ChartArea.VisibleSeries[0].IsIndexed)
                        {
                            ptInfo.ValueX = axis.GetLabelContent((int)leastXVal).ToString();
                        }
                        else
                        {
                            ptInfo.ValueX = axis.GetLabelContent(leastXVal).ToString();
                        }

                        ptInfo.X = leastXPoint + this.ChartArea.SeriesClipRect.Left;
                    }
                    else
                    {
                        ptInfo.ValueY = axis.GetLabelContent(leastXVal).ToString();
                        ptInfo.Y = point.Y;
                    }
                }
                axisLabels.Add(axis, ptInfo);
        
            }
            GenerateAxisLabels();
            GenerateLabels();
            GenerateTrackBalls();
            double xPos = leastX + this.ChartArea.SeriesClipRect.Left;
            if (isReversed && pointInfos.Count>0)
            {
                xPos = leastX + this.ChartArea.SeriesClipRect.Top;
                line.X1 = this.ChartArea.SeriesClipRect.Left;
                line.X2 = this.ChartArea.SeriesClipRect.Left + this.ChartArea.SeriesClipRect.Width;
                line.Y2 = line.Y1 = xPos;
            }
            else if (pointInfos.Count > 0)
            {
                line.Y1 = this.ChartArea.SeriesClipRect.Top;
                line.Y2 = this.ChartArea.SeriesClipRect.Top + this.ChartArea.SeriesClipRect.Height;
                line.X1 = line.X2 = xPos;
            }
            isTrackBallUpdateDispatching = false;
        }

        private void GenerateAxisLabels()
        {
            foreach (KeyValuePair<ChartAxis, ChartPointInfo> keyVal in axisLabels)
            {
                ChartAxis axis = keyVal.Key;
                ChartPointInfo pointInfo = keyVal.Value;
                if (axis.ShowTrackBallInfo)
                {
                    if (isReversed)
                    {
                        if (axis.Orientation == Orientation.Vertical)
                        {
                            pointInfo.Y = axis.OpposedPosition ? axis.ArrangeRect.Right : axis.ArrangeRect.Left;
                            AddLabel(pointInfo, GetChartAlignment(axis.OpposedPosition, ChartAlignment.Far),
                            AxisLabelAlignment, axis.TrackBallLabelTemplate);
                        }
                        else
                        {
                            pointInfo.X = axis.OpposedPosition ? axis.ArrangeRect.Left : axis.ArrangeRect.Right;
                            AddLabel(pointInfo, AxisLabelAlignment,
                                GetChartAlignment(axis.OpposedPosition, ChartAlignment.Near), axis.TrackBallLabelTemplate);
                        }
                    }
                    else
                    {
                        if (axis.Orientation == Orientation.Vertical)
                        {
                            pointInfo.X = axis.OpposedPosition ? axis.ArrangeRect.Left : axis.ArrangeRect.Right;
                            AddLabel(pointInfo, AxisLabelAlignment,
                                GetChartAlignment(axis.OpposedPosition, ChartAlignment.Near), axis.TrackBallLabelTemplate);
                        }
                        else
                        {
                            pointInfo.Y = axis.OpposedPosition ? axis.ArrangeRect.Bottom : axis.ArrangeRect.Top;
                            AddLabel(pointInfo, GetChartAlignment(axis.OpposedPosition, ChartAlignment.Far),
                                AxisLabelAlignment, axis.TrackBallLabelTemplate);
                        }
                    }
                }
            }
        }

        private ChartAlignment GetChartAlignment(bool isOpposed, ChartAlignment alignment)
        {
            if (isOpposed)
            {
                if (alignment == ChartAlignment.Near)
                    return ChartAlignment.Far;
                else if (alignment == ChartAlignment.Far)
                    return ChartAlignment.Near;
                else
                    return ChartAlignment.Center;
            }
            else
                return alignment;
        }
        /// <summary>
        /// Method implementation for GenerateLabels 
        /// </summary>
        protected virtual void GenerateLabels()
        {
            foreach (ChartPointInfo pointInfo in PointInfos)
            {
                AddLabel(pointInfo, LabelVerticalAlignment, LabelHorizontalAlignment, pointInfo.Series.TrackBallLabelTemplate);
            }
        }
        /// <summary>
        /// Mathod implementation for Add labels in Chart
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="verticalAlignment"></param>
        /// <param name="horizontalAlignment"></param>
        /// <param name="template"></param>
        protected void AddLabel(ChartPointInfo obj, ChartAlignment verticalAlignment, ChartAlignment horizontalAlignment, DataTemplate template)
        {
            if (obj != null && template != null)
            {
                AddLabel(obj, verticalAlignment, horizontalAlignment, template, obj.X, obj.Y);
            }
        }
        /// <summary>
        /// Method implementation for AddLabels in Chart
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="verticalAlignemnt"></param>
        /// <param name="horizontalAlignment"></param>
        /// <param name="template"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected virtual void AddLabel(object obj, ChartAlignment verticalAlignemnt, ChartAlignment horizontalAlignment
            , DataTemplate template, double x, double y)
        {
            if (template == null)
                return;
            ContentControl control = new ContentControl();
            control.Content = obj;
            control.IsHitTestVisible = false;
            control.ContentTemplate = template;
            AddElement(control);
            labelElements.Add(control);
            control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            if (isReversed)
            {
                AlignElement(control, horizontalAlignment, verticalAlignemnt, y, ChartArea.SeriesClipRect.Height - x);
            }
            else
            {
                AlignElement(control, verticalAlignemnt, horizontalAlignment, x, y);
            }
        }

        private void AlignElement(Control control, ChartAlignment verticalAlignemnt, ChartAlignment horizontalAlignment,
            double x, double y)
        {
            if (control != null && !double.IsInfinity(x)
                && !double.IsInfinity(y) && !double.IsNaN(x) && !double.IsNaN(y))
            {

                if (horizontalAlignment == ChartAlignment.Near)
                {
                    x = x - control.DesiredSize.Width;
                }
                else if (horizontalAlignment == ChartAlignment.Center)
                {
                    x = x - control.DesiredSize.Width / 2;
                }

                if (verticalAlignemnt == ChartAlignment.Near)
                {
                    y = y - control.DesiredSize.Height;
                }
                else if (verticalAlignemnt == ChartAlignment.Center)
                {
                    y = y - control.DesiredSize.Height / 2;
                }

                Canvas.SetLeft(control, x);
                Canvas.SetTop(control, y);
            }
        }
        /// <summary>
        /// Method implementation for Add Trackball to Corresponding chartpoint
        /// </summary>
        /// <param name="pointInfo"></param>
        protected virtual void AddTrackBall(ChartPointInfo pointInfo)
        {
            ChartTrackBallControl control = new ChartTrackBallControl(pointInfo.Series);

            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("ChartTrackBallStyle");
            control.SetBinding(ChartTrackBallControl.StyleProperty, binding);

            trackBalls.Add(control);
            AddElement(control);
            control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            if (isReversed)
            {
                AlignElement(control, ChartAlignment.Center, ChartAlignment.Center, pointInfo.Y, ChartArea.SeriesClipRect.Height - pointInfo.X);
            }
            else
            {
                AlignElement(control, ChartAlignment.Center, ChartAlignment.Center, pointInfo.X, pointInfo.Y);
            }
        }
        /// <summary>
        /// Method implementation for generate TrackBalls
        /// </summary>
        protected virtual void GenerateTrackBalls()
        {
            foreach (ChartPointInfo pointInfo in PointInfos)
            {
                AddTrackBall(pointInfo);
            }
        }
        /// <summary>
        /// Method implementation for Add UIElements 
        /// </summary>
        /// <param name="element"></param>
        protected void AddElement(UIElement element)
        {
            if (!this.AdorningCanvas.Children.Contains(element))
            {
                this.AdorningCanvas.Children.Add(element);
                elements.Add(element as FrameworkElement);
            }
        }

        private void SetinterpolatedPositions(Point point)
        {
            //int index = 0;
            //foreach (ChartSeries series in this.ChartArea.Series)
            //{
            //    ChartPoint[] chartPoint = series.FindNearestChartPoints(point);
            //    double xstart = this.ChartArea.ValueToPoint(series.XAxis, series.XAxis.VisibleRange.Start);
            //    double yend = this.ChartArea.ValueToPoint(series.YAxis, series.YAxis.VisibleRange.End);
            //    double valx = this.ChartArea.PointToValue(this.ChartArea.PrimaryAxis, point);
            //    double YValue = FindYValue(chartPoint[0], chartPoint[2], valx, series.IsIndexed);
            //    double OffsetY = this.ChartArea.ValueToPoint(series.YAxis, YValue) - yend;
            //    double OffsetX = this.ChartArea.ValueToPoint(series.XAxis, valx) - xstart;
            //    GenerateDataPointTrackers(index, OffsetX, OffsetY, series);
            //    FrameworkElement element = GetElement(index, series);
            //    Canvas.SetLeft(element, OffsetX - element.ActualWidth / 2);
            //    Canvas.SetTop(element, OffsetY - element.ActualHeight / 2);
            //    index++;
            //}
        }

        //internal double FindYValue(ChartPoint point1, ChartPoint point2, double valX, bool isIndexed)
        //{
        //    if (isIndexed)
        //    {
        //        double yVal = 0;

        //        double m = (point2.Y - point1.Y) / (point2.Index - point1.Index); // slop = y2-y1/x2-x1

        //        double b = point2.Y - (m * point2.Index);  //y - y1 = m (x-x1)  y = mx +b 

        //        yVal = (m * valX) + b;

        //        return yVal;
        //    }
        //    else
        //    {
        //        double yVal = 0;

        //        double m = (point2.Y - point1.Y) / (point2.X - point1.X); // slop = y2-y1/x2-x1

        //        double b = point2.Y - (m * point2.X);  //y - y1 = m (x-x1)  y = mx +b 

        //        yVal = (m * valX) + b;

        //        return yVal;
        //    }

        //}

        private void Activate(bool activate)
        {
            foreach (UIElement element in elements)
            {
                element.Visibility = activate ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        protected override DependencyObject CloneBehavior(DependencyObject obj)
        {
            return base.CloneBehavior(new ChartTrackBallBehavior() { CurrentPoint = this.CurrentPoint });
        }

        #endregion

    }

    /// <summary>
    /// Sets the fill color for the track ball control.
    /// </summary>
    public class ChartTrackBallColorConverter : IValueConverter
    {
#if WINDOWS_PHONE
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="value">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#else
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
        {
            if (value is ChartTrackBallControl)
            {
                ChartTrackBallControl trackBall = value as ChartTrackBallControl;
                if (trackBall.Background != null)
                    return trackBall.Background;
                if (trackBall.Series != null)
                {
                    return trackBall.Series.GetInteriorColor(1);
                }
            }
            return null;
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#else
        /// <summary>
        ///  Modifies the target data before passing it to the source object. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Defines the control Template for the trackball
    /// </summary>
    public class ChartTrackBallControl : Control
    {
        /// <summary>
        /// Called when instance created for ChartTrackBall
        /// </summary>
        /// <param name="series"></param>
        public ChartTrackBallControl(ChartSeriesBase series)
        {
            this.Series = series;
            DefaultStyleKey = typeof(ChartTrackBallControl);
        }
        /// <summary>
        /// Get or Set Series property
        /// </summary>
        public ChartSeriesBase Series
        {
            get { return (ChartSeriesBase)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for Series.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register("Series", typeof(ChartSeriesBase), typeof(ChartTrackBallControl), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set strokeproperty
        /// </summary>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(ChartTrackBallControl), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set StrokeThickness property
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ChartTrackBallControl), new PropertyMetadata(1d));
    }
}
