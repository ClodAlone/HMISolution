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
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.Devices.Input;
using Windows.UI.Input;
using System.Threading.Tasks;
using Windows.UI;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// ChartCrossHairBehavior enables viewing of informations related to Chart coordinates, at mouse over position or at touch contact point inside a Chart.
    /// </summary>
    /// <remarks>
    /// ChartCrossHairBehavior displays a vertical line, horizontal line and a popup like control displaying information about the data point
    /// at touch contact point or at mouse over position. You can also customize the look of cross hair and information displayed in a label.
    /// </remarks>
    public class ChartCrossHairBehavior: ChartBehavior
    {
        #region fields

        protected internal Point CurrentPoint;
        private Line verticalLine;
        private Line horizontalLine;
        #if !WINDOWS_PHONE
        private int fingerCount = 0;
        #endif
        private bool isActivated = false;
        private List<ContentControl> labelElements;
        private ObservableCollection<ChartPointInfo> pointInfos;
        private List<FrameworkElement> elements;
        private string labelXValue;
        private string labelYValue;
        #endregion

        #region properties
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
        /// Gets or Sets the alignment for the label appearing in horizontal axis.
        /// </summary>
        public ChartAlignment VerticalAxisLabelAlignment    
        {
            get { return (ChartAlignment)GetValue(VerticalAxisLabelAlignmentProperty); }
            set { SetValue(VerticalAxisLabelAlignmentProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VerticalAxisLabelAlignmentProperty =
            DependencyProperty.Register("VerticalAxisLabelAlignment", typeof(ChartAlignment), typeof(ChartCrossHairBehavior), new PropertyMetadata(ChartAlignment.Center));

        /// <summary>
        /// Gets or Sets the alignment for the label appearing in vertical axis.
        /// </summary>
        public ChartAlignment HorizontalAxisLabelAlignment
        {
            get { return (ChartAlignment)GetValue(HorizontalAxisLabelAlignmentProperty); }
            set { SetValue(HorizontalAxisLabelAlignmentProperty, value); }
        }

        
        /// <summary>
        ///  Using a DependencyProperty as the backing store for HorizontalAxisLabelAlignment.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HorizontalAxisLabelAlignmentProperty =
            DependencyProperty.Register("HorizontalAxisLabelAlignment", typeof(ChartAlignment), typeof(ChartCrossHairBehavior), new PropertyMetadata(ChartAlignment.Center));

        /// <summary>
        /// Gets the collection of ChartPointInfo
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
        ///Gets or Sets the style for horizontal line.
        /// </summary>
        public Style HorizontalLineStyle
        {
            get { return (Style)GetValue(HorizontalLineStyleProperty); }
            set { SetValue(HorizontalLineStyleProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for LineStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HorizontalLineStyleProperty =
            DependencyProperty.Register("HorizontalLineStyle", typeof(Style), typeof(ChartCrossHairBehavior), new PropertyMetadata(ChartDictionaries.GenericCommonDictionary["trackBallLineStyle"]));


        /// <summary>
        ///Gets or Sets the style for vertical line.
        /// </summary>
        public Style VerticalLineStyle
        {
            get { return (Style)GetValue(VerticalLineStyleProperty); }
            set { SetValue(VerticalLineStyleProperty, value); }
        }

       
        /// <summary>
        /// Using a DependencyProperty as the backing store for LineStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VerticalLineStyleProperty =
            DependencyProperty.Register("VerticalLineStyle", typeof(Style), typeof(ChartCrossHairBehavior), new PropertyMetadata(ChartDictionaries.GenericCommonDictionary["trackBallLineStyle"]));

        #endregion

        #region ctor

        /// <summary>
        /// Called when instance created for ChartCrossHairBehaviour
        /// </summary>
        public ChartCrossHairBehavior()
        {
            elements = new List<FrameworkElement>();
            verticalLine = new Line();
            horizontalLine = new Line();

            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("VerticalLineStyle");
            verticalLine.SetBinding(Line.StyleProperty, binding);


            horizontalLine = new Line(); binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("HorizontalLineStyle");
            horizontalLine.SetBinding(Line.StyleProperty, binding);

            labelElements = new List<ContentControl>();
            pointInfos = new ObservableCollection<ChartPointInfo>();
        }

        #endregion

        #region methods

        /// <summary>
        /// Method implementation for AttachElements
        /// </summary>
        protected override void AttachElements()
        {
            if (this.AdorningCanvas!=null &&!this.AdorningCanvas.Children.Contains(verticalLine))
            {
                this.AdorningCanvas.Children.Add(verticalLine);
                elements.Add(verticalLine);
            }

            if (this.AdorningCanvas != null && !this.AdorningCanvas.Children.Contains(horizontalLine))
            {
                this.AdorningCanvas.Children.Add(horizontalLine);
                elements.Add(horizontalLine);
            }

            IsActivated = false;
           
        }

        /// <summary>
        /// Method implementation for DetachElements
        /// </summary>
        internal protected override void DetachElements()
        {
            foreach (var element in elements)
            {
                this.AdorningCanvas.Children.Remove(element);
            }
            elements.Clear();
        }

        /// <summary>
        /// Called when Size Changed
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnSizeChanged(SizeChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(labelXValue) && !string.IsNullOrEmpty(labelYValue))
            {
                double y1 = this.ChartArea.ValueToPoint(this.ChartArea.InternalSecondaryAxis, (Convert.ToDouble(labelYValue)));
                double x1 = this.ChartArea.ValueToPoint(this.ChartArea.InternalPrimaryAxis, (Convert.ToDouble(labelXValue)));
                if (!double.IsNaN(y1) && !double.IsNaN(x1))
                {
                    foreach (ContentControl control in labelElements)
                    {
                        DetachElement(control);
                    }
                    this.labelElements.Clear();
                    this.pointInfos.Clear();
                    this.SetPosition(new Point(x1, y1));
                }
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
                if (this.ChartArea != null && this.ChartArea.VisibleSeries.Count>0 && this.ChartArea.VisibleSeries[0] is CartesianSeries && IsActivated)
                {
                    Point point = e.GetPosition(this.AdorningCanvas);

                    if (this.ChartArea.SeriesClipRect.Contains(point))
                    {
                        foreach (ContentControl control in labelElements)
                        {
                            DetachElement(control);
                        }

                        labelElements.Clear();

                        pointInfos.Clear();
                        SetPosition(point);
                    }
                }
        }

#endif

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
            IsActivated = false;
        }
#endif
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
        protected internal override void OnPointerMoved(PointerRoutedEventArgs e)
#endif
        {
#if WPF
            if (Mouse.LeftButton != MouseButtonState.Pressed)
                IsActivated = true;
#endif
#if SILVERLIGHT_UNCOMMON
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
#if !WINDOWS_PHONE
                CurrentPoint = new Point(pointer.Position.X, pointer.Position.Y);
#else
                CurrentPoint = e.GetPosition(this.AdorningCanvas);
#endif
                if (this.ChartArea.SeriesClipRect.Contains(CurrentPoint))
                {
                    foreach (ContentControl control in labelElements)
                    {
                        DetachElement(control);
                    }

                    labelElements.Clear();

                    pointInfos.Clear();
                    SetPosition(CurrentPoint);
                }
            }
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
        /// <summary>
        /// Called when chart layout updated from chart
        /// </summary>
        /// <param name="e"></param>
        protected internal override void OnLayoutUpdated()
        {
            if (IsActivated)
            {
                foreach (ContentControl control in labelElements)
                {
                    DetachElement(control);
                }

                labelElements.Clear();

                pointInfos.Clear();

                if (this.ChartArea.SeriesClipRect.Contains(CurrentPoint))
                {
                    SetPosition(CurrentPoint);
                }
                else
                {
                    foreach (var element in elements)
                    {
                        element.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
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
        }
        /// <summary>
        /// Method implementation for Set positions for given point
        /// </summary>
        /// <param name="point"></param>
        protected internal virtual void SetPosition(Point point)
        {
            if (AdorningCanvas == null || double.IsNaN(point.X) || double.IsNaN(point.Y)) return;
           
            var seriesLeft = ChartArea.SeriesClipRect.Left;
            var seriesTop = ChartArea.SeriesClipRect.Top;
            
            double x =point.X;
            double y = point.Y;

            foreach (var element in elements)
            {
                element.Visibility = Visibility.Visible;
            }

            verticalLine.X1 = verticalLine.X2 = x > this.ChartArea.SeriesClipRect.Right ? this.ChartArea.SeriesClipRect.Right : x;
            verticalLine.Y1 = seriesTop;
            verticalLine.Y2 = this.ChartArea.SeriesClipRect.Height + seriesTop;

            horizontalLine.Y1 = horizontalLine.Y2 = y;
            horizontalLine.X1 = seriesLeft;
            horizontalLine.X2 = seriesLeft + this.ChartArea.SeriesClipRect.Width;

            foreach (ChartAxis axis in ChartArea.Axes)
            {
                if ((axis.RenderedRect.Left <= point.X && axis.RenderedRect.Right >= point.X)
                    || axis.RenderedRect.Top <= point.Y && axis.RenderedRect.Bottom >= point.Y)
                {
                    double val = this.ChartArea.PointToValue(axis, new Point(point.X - seriesLeft, point.Y - seriesTop));
                    if (!double.IsNaN(val))
                    {
                        ChartPointInfo pointInfo = new ChartPointInfo();
                        if (axis.Orientation == Orientation.Horizontal)
                        {
                            if (ChartArea.VisibleSeries.Count > 0 && ChartArea.VisibleSeries[0].IsIndexed && !ChartArea.VisibleSeries[0].IsActualTransposed)
                            {
                                pointInfo.ValueX = axis.GetLabelContent((int)Math.Round(val)).ToString();
                                var x1 = this.ChartArea.ValueToPoint(axis, Math.Round(val));
                                x1 += seriesLeft;
                                pointInfo.X = verticalLine.X1 = verticalLine.X2 = x1 > this.ChartArea.SeriesClipRect.Right ? this.ChartArea.SeriesClipRect.Right : x1;
                            }
                            else if (ChartArea.VisibleSeries.Count > 0)
                            {
                                pointInfo.ValueX = axis.GetLabelContent(val).ToString();
                                pointInfo.X = point.X;
                            }
                            labelXValue = val.ToString();
                        }
                        else
                        {
                            if (ChartArea.VisibleSeries.Count > 0 && ChartArea.VisibleSeries[0].IsIndexed && ChartArea.VisibleSeries[0].IsActualTransposed)
                            {
                                pointInfo.ValueY = axis.GetLabelContent((int)Math.Round(val)).ToString();
                                var y1 = this.ChartArea.ValueToPoint(axis, Math.Round(val));
                                y1 += seriesTop;
                                pointInfo.Y = verticalLine.Y1 = verticalLine.Y2 = y1 > this.ChartArea.SeriesClipRect.Bottom ? this.ChartArea.SeriesClipRect.Bottom : y1;
                            }
                            else
                            {
                                pointInfo.ValueY = axis.GetLabelContent(Math.Round(val, 2)).ToString();
                                pointInfo.Y = point.Y;
                            }
                            labelYValue = val.ToString();
                        }

                        GenerateLabel(pointInfo, axis);
                    }
                }
            }

        }
        /// <summary>
        /// Method implementation for GenerateLabel for axis
        /// </summary>
        /// <param name="pointInfo"></param>
        /// <param name="axis"></param>
        protected virtual void GenerateLabel(ChartPointInfo pointInfo, ChartAxis axis)
        {
            if (axis.ShowTrackBallInfo)
            {
                if (axis.Orientation == Orientation.Vertical)
                {
                    pointInfo.X = axis.OpposedPosition?axis.ArrangeRect.Left:axis.ArrangeRect.Right;
                    AddLabel(pointInfo, VerticalAxisLabelAlignment,
                        GetChartAlignment(axis.OpposedPosition, ChartAlignment.Near), axis.TrackBallLabelTemplate);
                }
                else
                {
                    pointInfo.Y = axis.OpposedPosition ? axis.ArrangeRect.Bottom : axis.ArrangeRect.Top;
                    AddLabel(pointInfo, GetChartAlignment(axis.OpposedPosition, ChartAlignment.Far),
                        HorizontalAxisLabelAlignment, axis.TrackBallLabelTemplate);
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
        /// Method implementatin for 
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
        /// Method implementation for add labels for CrossHair
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
            control.ContentTemplate = template;
            AddElement(control);
            labelElements.Add(control);
            control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            ChartPointInfo pointInfo = obj as ChartPointInfo;

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
        /// <summary>
        /// Method implementation for Add elements in UIElement
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

        private void Activate(bool activate)
        {
            foreach (UIElement element in elements)
            {
                element.Visibility = activate ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        protected override DependencyObject CloneBehavior(DependencyObject obj)
        {
            return base.CloneBehavior(new ChartCrossHairBehavior() { CurrentPoint = this.CurrentPoint });
        }

        #endregion
    }
}
