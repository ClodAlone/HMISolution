#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections.Specialized;
#if WINDOWS_PHONE
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public abstract class SparklineBase : Control
    {
        #region fields

        Rectangle rangeBand;

        SparklinePointsInfo info;

        bool isTemplateApplied, isMeasureed, needToAnimate = true;

        protected int DataCount { get; set; }

        internal bool IsIndexed { get; set; }

        internal List<double> yValues, xValues;

        protected List<double> EmptyPointIndexes { get; set; }

        internal double minYValue, maxYValue, deltaY, minXValue, maxXValue, deltaX, availableWidth, availableHeight;

        internal Grid RootPanel { get; set; }

        protected Canvas SegmentPresenter { get; set; }

        internal Canvas UtilityPresenter { get; set; }

        #endregion

        #region event

        public event SparklineMouseMoveHandler OnSparklineMouseMove;

        #endregion

        #region ctor

        public SparklineBase()
        {
            DefaultStyleKey = typeof(SparklineBase);
        }

        #endregion

        #region proeprty

        /// <summary>
        /// Gets or sets a value to animate the sparkline on loading and whenever ItemsSource change.
        /// </summary>
        public bool EnableAnimation
        {
            get { return (bool)GetValue(EnableAnimationProperty); }
            set { SetValue(EnableAnimationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableAnimation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableAnimationProperty =
            DependencyProperty.Register("EnableAnimation", typeof(bool), typeof(SparklineBase), new PropertyMetadata(false, OnEnableAnimationChanged));

        /// <summary>
        /// Gets or Sets the brush to paint the interior of the sparkline.
        /// </summary>
        public Brush Interior
        {
            get { return (Brush)GetValue(InteriorProperty); }
            set { SetValue(InteriorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Interior.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InteriorProperty =
            DependencyProperty.Register("Interior", typeof(Brush), typeof(SparklineBase), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2))));

        /// <summary>
        /// Gets or sets the range band brush.
        /// </summary>
        public Brush RangeBandBrush
        {
            get { return (Brush)GetValue(RangeBandBrushProperty); }
            set { SetValue(RangeBandBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeBandBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeBandBrushProperty =
            DependencyProperty.Register("RangeBandBrush", typeof(Brush), typeof(SparklineBase), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the maximum range band value in Y axis.
        /// </summary>
        public double BandRangeEnd
        {
            get { return (double)GetValue(BandRangeEndProperty); }
            set { SetValue(BandRangeEndProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BandRangeEnd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BandRangeEndProperty =
            DependencyProperty.Register("BandRangeEnd", typeof(double), typeof(SparklineBase), new PropertyMetadata(double.NaN, OnHighlightValueChanged));

        /// <summary>
        /// Gets or sets the minimum range band value in Y axis.
        /// </summary>
        public double BandRangeStart
        {
            get { return (double)GetValue(BandRangeStartProperty); }
            set { SetValue(BandRangeStartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BandRangeStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BandRangeStartProperty =
            DependencyProperty.Register("BandRangeStart", typeof(double), typeof(SparklineBase), new PropertyMetadata(double.NaN, OnHighlightValueChanged));

        /// <summary>
        /// Gets or sets the minimum value for Y axis.
        /// </summary>
        public double MinimumYValue
        {
            get { return (double)GetValue(MinimumYValueProperty); }
            set { SetValue(MinimumYValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinimumYValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumYValueProperty =
            DependencyProperty.Register("MinimumYValue", typeof(double), typeof(SparklineBase), new PropertyMetadata(double.NaN, OnYRangeChanged));

        /// <summary>
        /// Gets or sets the maximum value for Y axis.
        /// </summary>
        public double MaximumYValue
        {
            get { return (double)GetValue(MinimumXValueProperty); }
            set { SetValue(MinimumXValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinimumXValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumXValueProperty =
            DependencyProperty.Register("MaximumYValue", typeof(double), typeof(SparklineBase), new PropertyMetadata(double.NaN, OnYRangeChanged));

        /// <summary>
        /// Gets or Sets stroke thickness for segments
        /// </summary>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(SparklineBase), new PropertyMetadata(2d));

        /// <summary>
        /// Gets or Sets the brush to paint outline of the sparkline.
        /// </summary>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(SparklineBase), new PropertyMetadata(null));
      
        /// <summary>
        /// Gets or sets an IEnumerable source used to generate sparkline.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SparklineBase), new PropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>
        /// Gets or Sets the property path to retrieve y data from ItemsSource.
        /// </summary>
        public string YBindingPath
        {
            get { return (string)GetValue(YBindingPathProperty); }
            set
            {
                SetValue(YBindingPathProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for YBindingPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YBindingPathProperty =
            DependencyProperty.Register("YBindingPath", typeof(string), typeof(SparklineBase), new PropertyMetadata("", OnYBingingPathChanged));

        /// <summary>
        /// Gets or Sets a value that determines how to calculate value for empty point.
        /// </summary>
        public EmptyPointValues EmptyPointValue
        {
            get { return (EmptyPointValues)GetValue(EmptyPointValueProperty); }
            set { SetValue(EmptyPointValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EmptyPointValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EmptyPointValueProperty =
            DependencyProperty.Register("EmptyPointValue", typeof(EmptyPointValues), typeof(SparklineBase), new PropertyMetadata(EmptyPointValues.None, OnEmptyPointValueChanged));

        #endregion

        #region methods

        protected virtual void AnimateSegments(UIElementCollection elements) { }

        private static void OnYRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SparklineBase).UpdateMinMaxValues();
            (d as SparklineBase).UpdateArea();
        }

        private static void OnHighlightValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SparklineBase).UpdateRangeBand();
        }

        private static void OnEnableAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SparklineBase).OnEnableAnimationChanged();
        }

        private void OnEnableAnimationChanged()
        {
            if (EnableAnimation && SegmentPresenter != null)
                AnimateSegments(SegmentPresenter.Children);
        }

        private static void OnEmptyPointValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SparklineBase).UpdateArea();
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SparklineBase).OnItemsSourceChanged(e);
        }

        private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyCollectionChanged)
            {
                (e.OldValue as INotifyCollectionChanged).CollectionChanged -= OnDataCollectionChanged;
            }

            if (e.NewValue is INotifyCollectionChanged)
            {
                (e.NewValue as INotifyCollectionChanged).CollectionChanged += OnDataCollectionChanged;
            }
            if (isTemplateApplied)
                GeneratePoints(String.Empty);
            UpdateArea();
        }

        /// <summary>
        /// Creates the screen point from data point.
        /// </summary>
        public virtual Point TransformToVisible(double x, double y)
        {
            return new Point(Math.Round((availableWidth - (this.Padding.Left * 2)) * ((x - minXValue) / deltaX)), Math.Round((availableHeight - (Padding.Top * 2)) * (1 - ((y - minYValue) / deltaY))));
        }

        void OnDataCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                SetIndividualPoints(e.NewStartingIndex, e.NewItems[0], false, String.Empty);
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                yValues.RemoveAt(e.OldStartingIndex);
                xValues.RemoveAt(DataCount - 1);
                DataCount--;
                if (DataCount == 0)
                    Reset();
            }
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                SetIndividualPoints(e.NewStartingIndex, e.NewItems[0], true, String.Empty);
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Reset();
            }
            FindEmptyPoints();
            ValidateEmptyPoints(yValues);
            UpdateMinMaxValues();
            UpdateArea();
        }

        public virtual void Reset()
        {
            SegmentPresenter.Children.Clear();
            yValues.Clear();
            xValues.Clear();
            DataCount = 0;
        }

        private static void OnYBingingPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //(d as SparklineBase).GeneratePoints();
        }

        private static void OnXBindingPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Generate the points from the items source
        /// </summary>
        protected virtual void GeneratePoints(string xPath)
        {
            if (ItemsSource != null)
            {
                yValues = new List<double>();
                xValues = new List<double>();
                EmptyPointIndexes = new List<double>();
                double i = 0;
                double curY;
                IEnumerator enumerator = ItemsSource.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    if (string.IsNullOrEmpty(YBindingPath))
                    {
                        do
                        {
                            if (enumerator.Current is double)
                            {
                            curY = Convert.ToDouble(enumerator.Current);
                            yValues.Add(curY);
                            xValues.Add(i);
                            }
                            i++;
                        } while (enumerator.MoveNext());
                    }
                    else
                    {
                        var yPropertyInfo = enumerator.Current.GetType().GetTypeInfo().GetDeclaredProperty(YBindingPath);
                        if (yPropertyInfo != null)
                        {
                            IPropertyAccessor yPropertyAccessor = null;

                            yPropertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(yPropertyInfo);

                            Func<object, object> yGetMethod = yPropertyAccessor.GetMethod;

                            if (string.IsNullOrEmpty(xPath))
                            {
                                IsIndexed = true;
                                do
                                {
                                    curY = Convert.ToDouble(yGetMethod(enumerator.Current));
                                    xValues.Add(i);
                                    yValues.Add(curY);
                                    i++;
                                } while (enumerator.MoveNext());
                            }
                            else
                            {
                                IsIndexed = false;
                                IPropertyAccessor xPropertyAccessor = null;
                                var xPropertyInfo = enumerator.Current.GetType().GetTypeInfo().GetDeclaredProperty(xPath);
                                xPropertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(xPropertyInfo);
                                object xvalueType = xPropertyAccessor.GetValue(enumerator.Current);
                                Func<object, object> xGetMethod = xPropertyAccessor.GetMethod;
                                double curX;
                                if (xGetMethod(enumerator.Current) is DateTime)
                                {
                                    do
                                    {
                                        curX = ((DateTime)xGetMethod(enumerator.Current)).ToOADate();
                                        curY = Convert.ToDouble(yGetMethod(enumerator.Current));
                                        yValues.Add(curY);
                                        xValues.Add(curX);
                                    } while (enumerator.MoveNext());
                                }
                                else
                                {
                                    do
                                    {
                                        curX = Convert.ToDouble(xGetMethod(enumerator.Current));
                                        curY = Convert.ToDouble(yGetMethod(enumerator.Current));
                                        yValues.Add(curY);
                                        xValues.Add(curX);
                                    } while (enumerator.MoveNext());
                                }
                            }
                        }
                    }
                }
                DataCount = xValues.Count;
                FindEmptyPoints();
                ValidateEmptyPoints(yValues);
                UpdateMinMaxValues();
            }
        }

        internal void ValidateEmptyPoints(List<double> yValues)
        {
            switch (EmptyPointValue)
            {
                case EmptyPointValues.Average:
                    {
                        foreach (int index in EmptyPointIndexes)
                        {
                            if (index != -1)
                            {
                                if (index == 0)
                                    yValues[index] = (double.IsNaN(yValues[index + 1]) ? 0 : yValues[index + 1] / 2);
                                else if (index == DataCount - 1)
                                    yValues[index] = (double.IsNaN(yValues[index - 1]) ? 0 : yValues[index - 1] / 2);
                                else
                                    yValues[index] = ((double.IsNaN(yValues[index - 1]) ? 0 : yValues[index - 1]) + (double.IsNaN(yValues[index + 1]) ? 0 : yValues[index + 1] / 2));
                            }
                        }
                        break;
                    }
                case EmptyPointValues.Zero:
                    {
                        foreach (int index in EmptyPointIndexes)
                        {
                            if (index != -1)
                                yValues[index] = 0;
                        }
                        break;
                    }
                case EmptyPointValues.None:
                    {
                        foreach (int index in EmptyPointIndexes)
                        {
                            if (index != -1)
                                yValues[index] = double.NaN;
                        }
                        break;
                    }
            }
        }

        internal void FindEmptyPoints()
        {
            EmptyPointIndexes.Clear();
            EmptyPointIndexes.Add(-1);
            for (int i = 0; i < yValues.Count; i++)
            {
                if (double.IsNaN(yValues[i]))
                    EmptyPointIndexes.Add(i);
            }
        }

        protected virtual void UpdateMinMaxValues()
        {
            if ( xValues!=null && xValues.Count > 0)
            {
                minYValue = yValues.Where(p => !double.IsNaN(p)).Min();
                minYValue = double.IsNaN(MinimumYValue) ? minYValue : MinimumYValue;
                //minYValue = minYValue < 0 ? minYValue : 0;
                minXValue = xValues.Min();
                maxXValue = xValues.Max();
                maxYValue = yValues.Max();
                maxYValue = double.IsNaN(MaximumYValue) ? maxYValue : MaximumYValue;
                deltaY = maxYValue - minYValue;
                deltaX = maxXValue - minXValue;
                deltaY = deltaY == 0 ? 1 : deltaY;
                deltaX = deltaX == 0 ? 1 : deltaX;
            }
        }

        /// <summary>
        /// set the individual points to the existing collection
        /// </summary>
        protected virtual void SetIndividualPoints(int index, object obj, bool replace, string xPath)
        {
            if (obj != null)
            {
                if (string.IsNullOrEmpty(YBindingPath))
                {
                    if (replace && yValues.Count > index)
                    {
                        yValues[index] = Convert.ToDouble(obj);
                    }
                    else
                    {
                        yValues.Add(Convert.ToDouble(obj));
                        xValues.Add(DataCount);
                    }
                }
                else
                {
                    var yPropertyInfo = obj.GetType().GetTypeInfo().GetDeclaredProperty(YBindingPath);
                    if (yPropertyInfo != null)
                    {
                        IPropertyAccessor yPropertyAccessor = null;
                        if (yPropertyInfo != null)
                            yPropertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(yPropertyInfo);
                        Func<object, object> yGetMethod = yPropertyAccessor.GetMethod;
                        if (string.IsNullOrEmpty(xPath))
                        {
                            double yVal = Convert.ToDouble(yGetMethod(obj));
                            if (replace && yValues.Count > index)
                            {
                                yValues[index] = yVal;
                            }
                            else
                            {
                                yValues.Insert(index, yVal);
                                xValues.Add(DataCount);
                            }
                        }
                        else
                        {
                            var xPropertyInfo = obj.GetType().GetTypeInfo().GetDeclaredProperty(xPath);
                            IPropertyAccessor xPropertyAccessor = null;
                            if (xPropertyInfo != null)
                                xPropertyAccessor = FastReflectionCaches.PropertyAccessorCache.Get(xPropertyInfo);
                            Func<object, object> xGetMethod = xPropertyAccessor.GetMethod;
                            double yVal = Convert.ToDouble(yGetMethod(obj));
                            object xObj = xGetMethod(obj);
                            double xVal;
                            if (xObj is DateTime)
                                xVal = ((DateTime)xObj).ToOADate();
                            else
                                xVal = Convert.ToDouble(xObj);
                            if (replace && yValues.Count > index)
                            {
                                yValues[index] = (yVal);
                            }
                            else
                            {
                                xValues.Insert(index, xVal);
                                yValues.Insert(index, yVal);
                            }
                        }
                    }
                }
                DataCount++;
            }
        }

        /// <summary>
        /// To update the sparkline
        /// </summary>
        public void UpdateArea()
        {
            if (yValues != null && yValues.Count > 0 && isTemplateApplied && isMeasureed)
            {
                RenderSegments();
                if (!double.IsNaN(BandRangeStart) && !double.IsNaN(BandRangeEnd))
                    UpdateRangeBand();
                if (EnableAnimation && needToAnimate)
                    AnimateSegments(SegmentPresenter.Children);
                needToAnimate = false;
                this.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, availableWidth, availableHeight) };
            }
        }

        internal virtual void ClearUnUsedSegments(int dataCount)
        {
            int count = SegmentPresenter.Children.Count;
            if (count > dataCount)
            {
                for (int i = dataCount; i < count; i++)
                {
                    SegmentPresenter.Children.RemoveAt(dataCount);
                }
            }
        }

        /// <summary>
        /// To set the visual binding for the element
        /// </summary>
        internal virtual void SetBinding(Shape element)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("StrokeThickness");
            element.SetBinding(Shape.StrokeThicknessProperty, binding);
        }

        /// <summary>
        /// To render the visual segments
        /// </summary>
        protected abstract void RenderSegments();

#if WINDOWS_PHONE
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            RootPanel = this.GetTemplateChild("PART_RootPanel") as Grid;
            SegmentPresenter = new Canvas();
            RootPanel.Children.Add(SegmentPresenter);
            isTemplateApplied = true;
            GeneratePoints(String.Empty);
            UpdateArea();
            if (EnableAnimation && SegmentPresenter != null)
                AnimateSegments(SegmentPresenter.Children);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (availableHeight != availableSize.Height || availableWidth != availableSize.Width)
            {
                isMeasureed = true;
                availableWidth = double.IsInfinity(availableSize.Width) ? 100 : availableSize.Width;
                availableHeight = double.IsInfinity(availableSize.Height) ? 50 : availableSize.Height;
                UpdateArea();
            }
            return base.MeasureOverride(new Size(availableWidth, availableHeight));
        }

        private void UpdateRangeBand()
        {
            if (!double.IsNaN(BandRangeStart) && !double.IsNaN(BandRangeEnd) && RootPanel != null)
            {
                if (rangeBand == null)
                {
                    if (UtilityPresenter == null)
                    {
                        UtilityPresenter = new Canvas();
                        RootPanel.Children.Add(UtilityPresenter);
                    }
                    rangeBand = new Rectangle();
                    rangeBand.Opacity = 0.3;
                    UtilityPresenter.Children.Add(rangeBand);
                    BindRangeBandBrush(rangeBand);
                }
                Point point1 = TransformToVisible(minXValue, BandRangeStart);
                Point point2 = TransformToVisible(maxXValue, BandRangeEnd);
                Rect rect = new Rect(point1, point2);
                rangeBand.Width = rect.Width;
                rangeBand.SetValue(Canvas.LeftProperty, rect.X);
                rangeBand.Height = rect.Height;
                rangeBand.SetValue(Canvas.TopProperty, rect.Y);
            }
        }

        SparklineMouseMoveEventArgs args = new SparklineMouseMoveEventArgs();
#if !WINDOWS_PHONE
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
#else
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
#endif
        {
            if (OnSparklineMouseMove != null)
            {
#if WINDOWS_PHONE
                Point point = e.GetPosition(this);
                info = FindPoints(point.X, point.Y);
#else
                PointerPoint point = e.GetCurrentPoint(this);
                info = FindPoints(point.Position.X, point.Position.Y);
#endif
                if (e.OriginalSource is Rectangle)
                {
                    object[] values = ((e.OriginalSource as Shape).Tag as object[]);

                    if (values != null && values[1] is double && values[2] is double)
                        args.Value = new Point(Convert.ToDouble(values[1]), Convert.ToDouble(values[2]));
                    else
                        args.Value = info.Value;
                }
                else
                    args.Value = info.Value;
               
                args.Coordinate = info.Coordinate;               
                args.OriginalSource = e.OriginalSource;
                args.RootPanel = this.RootPanel;
                OnSparklineMouseMove(this, args);
            }
        }

        SparklinePointsInfo pointsInfo = new SparklinePointsInfo();
        internal SparklinePointsInfo FindPoints(double xVal, double yVal)
        {
            double factorX = xVal / availableWidth;
            double factorY = yVal / availableHeight;
            double x = double.NaN, y = double.NaN;

            if (!IsIndexed)
            {
                Point point = new Point(Math.Round(minXValue + maxXValue * factorX), Math.Round(minYValue + maxYValue * factorY));
                Point nearPoint = new Point(minXValue, minYValue);
                for (int i = 0; i < DataCount; i++)
                {
                    double x1 = xValues[i];
                    double y1 = yValues[i];

                    if (Math.Abs((point.X - x1)) <= Math.Abs((point.X - nearPoint.X)))
                    {
                        nearPoint = new Point(x1, y1);
                        x = xValues[i];
                        y = yValues[i];
                    }
                }
            }
            else
            {
                int count = xValues.Count - 1;
                x = Math.Round(minXValue + maxXValue * factorX);
                if (x <= maxXValue && x >= minXValue && xValues.Count > 0)
                {
                    y = yValues[(int)xValues[(int)x]];
                }
            }
            pointsInfo.Coordinate = TransformToVisible(x, y);
            pointsInfo.Value = new Point(x, y);
            return pointsInfo;
        }

        private void BindRangeBandBrush(Shape rangeBand)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("RangeBandBrush");
            rangeBand.SetBinding(Shape.FillProperty, binding);
        }

        internal void StyleBinding(FrameworkElement element, Style inputStyle, string propertyPath)
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath(propertyPath);
            element.SetBinding(Shape.StyleProperty, binding);
        }

        #endregion
    }

    public class SparklineMouseMoveEventArgs : EventArgs
    {
        public Point Value { get; set; }
        public Point Coordinate { get; set; }
        public object OriginalSource { get; set; }
        public Panel RootPanel { get; set; }
    }

    public delegate void SparklineMouseMoveHandler(object src, SparklineMouseMoveEventArgs args);
}
