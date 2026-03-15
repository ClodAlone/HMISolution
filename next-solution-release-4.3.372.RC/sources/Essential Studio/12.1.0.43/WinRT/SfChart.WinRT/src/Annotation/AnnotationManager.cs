#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Specialized;
using System;
using System.Linq;
using System.Collections.Generic;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows.Threading;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class AnnotationManager : DependencyObject
    {
        internal AnnotationResizer AnnotationResizer { get; set; }
        
        Annotation previouseSelectedAnnotation;

        double toolTipDuration = 0;

        internal ChartTooltip tooltip;

        DataTemplate defaultToolTipTemplate;

        private Annotation selectedAnnotation;

        private DispatcherTimer _timer;
        /// <summary>
        /// Gets or Sets the Selected Annotation
        /// </summary>
        public Annotation SelectedAnnotation
        {
            get { return selectedAnnotation; }
            set
            {
                if (selectedAnnotation != value)
                {
                    selectedAnnotation = value;
                    OnSelectionChanged();
                }
            }
        }

        private void OnSelectionChanged()
        {
            if ((SelectedAnnotation == null && AnnotationResizer != null) || (AnnotationResizer != null) || (previouseSelectedAnnotation is LineAnnotation && (previouseSelectedAnnotation as LineAnnotation).CanResize))
            {
                if (previouseSelectedAnnotation is LineAnnotation)
                {
                    HideLineResizer();
                    previouseSelectedAnnotation = null;
                }
                else
                {
                    AddOrRemoveAnnotations(AnnotationResizer, true);
                    AnnotationResizer = null;
                }
            }
            if (SelectedAnnotation != null)
            {
                OnAnnotationSelected();
                previouseSelectedAnnotation = SelectedAnnotation;
            }
        }

        public AnnotationManager()
        {
            
        }

        internal void HideLineResizer()
        {
            (previouseSelectedAnnotation as LineAnnotation).nearThumb.Visibility = Visibility.Collapsed;
            (previouseSelectedAnnotation as LineAnnotation).farThumb.Visibility = Visibility.Collapsed;
        }

        internal void OnAnnotationSelected()
        {
            if ((SelectedAnnotation is SolidShapeAnnotation && (SelectedAnnotation as ShapeAnnotation).CanResize))
            {
                AnnotationResizer = new AnnotationResizer();
                AnnotationResizer.X1 = SelectedAnnotation.X1;
                AnnotationResizer.Y1 = SelectedAnnotation.Y1;
                AnnotationResizer.X2 = (SelectedAnnotation as ShapeAnnotation).X2;
                AnnotationResizer.Y2 = (SelectedAnnotation as ShapeAnnotation).Y2;
                AnnotationResizer.XAxisName = SelectedAnnotation.XAxisName;
                AnnotationResizer.YAxisName = SelectedAnnotation.YAxisName;
                AnnotationResizer.XAxis = selectedAnnotation.XAxis;
                AnnotationResizer.YAxis = selectedAnnotation.YAxis;
                AnnotationResizer.CoordinateUnit = SelectedAnnotation.CoordinateUnit;
                AnnotationResizer.Angle = (SelectedAnnotation as SolidShapeAnnotation).Angle;
                AnnotationResizer.InternalHorizontalAlignment = SelectedAnnotation.InternalHorizontalAlignment;
                AnnotationResizer.InternalVerticalAlignment = SelectedAnnotation.InternalVerticalAlignment;
               
                if (SelectedAnnotation is SolidShapeAnnotation)
                AnnotationResizer.ResizingMode = (SelectedAnnotation as SolidShapeAnnotation).ResizingMode ;
                AddOrRemoveAnnotations(AnnotationResizer, false);
            }
            else if ((SelectedAnnotation is LineAnnotation && (SelectedAnnotation as LineAnnotation).CanResize))
            {
                (SelectedAnnotation as LineAnnotation).nearThumb.Visibility = Visibility.Visible;
                (SelectedAnnotation as LineAnnotation).farThumb.Visibility = Visibility.Visible;
                (SelectedAnnotation as LineAnnotation).UpdateAnnotation();
            }
        }

#if WPF || SILVERLIGHT_UNCOMMON
        Point currentPoint, previousPoint;
        bool isButtonPressed = false;
#endif
        SfChart chart;

        internal SfChart Chart
        {
            get
            {
                return chart;
            }
            set
            {
                if (chart != null)
                {
                    chart.SeriesBoundsChanged -= OnSeriesBoundsChanged;
                    chart.SizeChanged -= OnChartSizeChanged;
                    chart.Axes.CollectionChanged -= OnAxesCollectionChanged;
                    if(this.chart.ChartAnnotationCanvas.Children.Contains(tooltip))
                        this.chart.ChartAnnotationCanvas.Children.Remove(tooltip);
#if WINDOWS_PHONE
                    chart.MouseLeftButtonDown -= OnMouseLeftButtonDown;
                    chart.MouseLeftButtonUp -= OnMouseLeftButtonUp;
#if WPF || SILVERLIGHT_UNCOMMON
                    chart.MouseLeave -= OnMouseLeave;
                    chart.MouseMove -= OnMouseMove;
#endif
#if !SILVERLIGHT_UNCOMMON
                    chart.ManipulationDelta -= OnManipulationDelta;
#endif
#if WINDOWS_PHONE8 || WINDOWS_PHONE7
                    chart.Tap -= OnTap;
#endif
      
#else
                    chart.PointerPressed -= OnPointerPressed;
                    chart.ManipulationDelta -= OnManipulationDelta;
                    chart.ManipulationStarting -= ManipulationStarting;
                    chart.ManipulationCompleted -= ManipulationCompleted;
                    chart.PointerReleased -= OnPointerReleased;
#endif
                }                   
                chart = value;
                if (chart != null)
                {
                    tooltip = new ChartTooltip();
                    tooltip.IsHitTestVisible = false;
                    this.chart.ChartAnnotationCanvas.Children.Add(tooltip);
                    chart.SeriesBoundsChanged += OnSeriesBoundsChanged;
                    chart.SizeChanged += OnChartSizeChanged;
                    chart.Axes.CollectionChanged += OnAxesCollectionChanged;
#if WINDOWS_PHONE
                    chart.MouseLeftButtonDown += OnMouseLeftButtonDown;
                    chart.MouseLeftButtonUp += OnMouseLeftButtonUp;
#if WPF || SILVERLIGHT_UNCOMMON
                    chart.MouseLeave+=OnMouseLeave;
                    chart.MouseMove += OnMouseMove;
#endif
#if WINDOWS_PHONE8 || WINDOWS_PHONE7
                    chart.Tap += OnTap;
#endif
#if !SILVERLIGHT_UNCOMMON
                    chart.ManipulationDelta += OnManipulationDelta;
#endif

#else
                    chart.PointerMoved += OnPointerMoved;
                    chart.PointerPressed += OnPointerPressed;
                    chart.ManipulationDelta += OnManipulationDelta;
                    chart.ManipulationStarting += ManipulationStarting;
                    chart.ManipulationCompleted += ManipulationCompleted;
                    chart.PointerReleased += OnPointerReleased;
#endif
                }                    
            }
        }
     
#if WINDOWS_PHONE8 || WINDOWS_PHONE7
        private void OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowToolTip((Point)e.GetPosition(Chart.ChartAnnotationCanvas), e.OriginalSource);
        }
#endif

       

 #if WPF || SILVERLIGHT_UNCOMMON
        void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
           isButtonPressed = false;
        }
#endif
#if WINDOWS_PHONE
#if WPF || SILVERLIGHT_UNCOMMON
        void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
                ShowToolTip((Point)e.GetPosition(Chart.ChartAnnotationCanvas), e.OriginalSource);
                currentPoint = (Point)e.GetPosition(Chart.ChartAnnotationCanvas);
                TranslateTransform transform = ChartMath.Translate(previousPoint, currentPoint);
                if (isButtonPressed)
                {
                    if (SelectedAnnotation != null && SelectedAnnotation.XAxis.Orientation == Orientation.Vertical && SelectedAnnotation.CoordinateUnit == CoordinateUnit.Axis)
                        AnnotationDrag(-transform.Y, -transform.X);
                    else
                        AnnotationDrag(transform.X, transform.Y);
                }
                previousPoint = currentPoint;

        }
#endif

        void OnManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (SelectedAnnotation != null && SelectedAnnotation.XAxis.Orientation == Orientation.Vertical && SelectedAnnotation.CoordinateUnit == CoordinateUnit.Axis)
                AnnotationDrag(-e.DeltaManipulation.Translation.Y, -e.DeltaManipulation.Translation.X);
            else
                AnnotationDrag(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);
        }

        void OnMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MouseUp();
#if WPF || SILVERLIGHT_UNCOMMON
            isButtonPressed = false;
#endif
        }
        
        void OnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MouseDown(e.GetPosition(Chart.ChartAnnotationCanvas), e.GetPosition(Chart.SeriesAnnotationCanvas));
#if WPF || SILVERLIGHT_UNCOMMON
            isButtonPressed = true;
#endif
        }

#else
        private void OnPointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
             ShowToolTip((Point)e.GetCurrentPoint(Chart.ChartAnnotationCanvas).Position, e.OriginalSource);
        }
        
        void OnPointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            MouseUp();
        }

        void ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            UnHoldPanning(true);
        }

        void ManipulationStarting(object sender, Windows.UI.Xaml.Input.ManipulationStartingRoutedEventArgs e)
        {
            UnHoldPanning(false);
        }

        void OnManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (!e.IsInertial)
            {
                if (SelectedAnnotation != null && SelectedAnnotation.XAxis.Orientation == Orientation.Vertical && SelectedAnnotation.CoordinateUnit == CoordinateUnit.Axis)
                    AnnotationDrag(-e.Delta.Translation.Y, -e.Delta.Translation.X);
                else
                    AnnotationDrag(e.Delta.Translation.X, e.Delta.Translation.Y);
            }
        }

        void OnPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            MouseDown(e.GetCurrentPoint(Chart.ChartAnnotationCanvas).Position, e.GetCurrentPoint(Chart.SeriesAnnotationCanvas).Position);
        }

#endif
        private void MouseDown(Point pixelPos, Point axisPos)
        {
            Annotation annotation = null;
            var annotations = Annotations.Where(axis => axis.CoordinateUnit == CoordinateUnit.Axis).ToList();
            if (annotations.Any())
                annotation = GetSelectedAnnotation(annotations, axisPos);
            annotations = Annotations.Where(pixel => pixel.CoordinateUnit == CoordinateUnit.Pixel).ToList();
            if (annotations.Any() && annotation == null)
                annotation = GetSelectedAnnotation(annotations, pixelPos);
            SelectedAnnotation = annotation;
#if WINDOWS_PHONE
            UnHoldPanning(false);
#endif
        }

        private void UnHoldPanning(bool value)
        {
            if (SelectedAnnotation is ShapeAnnotation)
            {
                var shapeAnnotation = SelectedAnnotation as ShapeAnnotation;
                if (shapeAnnotation.CanDrag || shapeAnnotation.CanResize)
                {
                    var eumerator = chart.Behaviors.OfType<ChartZoomPanBehavior>();
                    foreach (var behavior in eumerator)
                    {
                        behavior.InternalEnablePanning = value;
                        behavior.InternalEnableSelectionZooming = value;
                    }
                }
            }
        }

        private void MouseUp()
        {
            if (AnnotationResizer != null)
                AnnotationResizer.IsResizing = false;
            else if (SelectedAnnotation is LineAnnotation)
                (SelectedAnnotation as LineAnnotation).IsResizing = false;
#if WINDOWS_PHONE
            UnHoldPanning(true);
#endif
        }
        
        /// <summary>
        /// Enables the tool tip in the visual.
        /// </summary>
        private void ShowToolTip(Point currentPoint, object source)
        {
            FrameworkElement parent = source as FrameworkElement;
            if (parent == null) return;
            Annotation annotation = null;
            if (source is Shape)
                annotation = (source as Shape).Tag as Annotation;
            else if (source is Image)
                annotation = (source as Image).Tag as Annotation;
            else if (CheckBounds(currentPoint) != null)
            {
                parent = VisualTreeHelper.GetParent(parent as UIElement) as FrameworkElement;
                while (parent != null)
                {
                    if (parent.Tag is Annotation)
                    {
                        annotation = parent.Tag as Annotation;
                        break;
                    }
                    parent = VisualTreeHelper.GetParent(parent as UIElement) as FrameworkElement;
                }
            }
            
            if (annotation != null && annotation.ShowToolTip)
            {
                if(defaultToolTipTemplate == null)
                    defaultToolTipTemplate = ChartDictionaries.GenericCommonDictionary["AnnotationTooltipTemplate"] as DataTemplate;

                tooltip.Visibility = Visibility.Visible;
                toolTipDuration = annotation.ToolTipShowDuration;
                if(!double.IsNaN(toolTipDuration))
                    ResetTimer();
                Point actualPosition = GetToolTipPosition(currentPoint, annotation);
                tooltip.ContentTemplate = annotation.ToolTipTemplate == null ? defaultToolTipTemplate : annotation.ToolTipTemplate;
                tooltip.Content = annotation.ToolTipContent;
                Canvas.SetLeft(tooltip, actualPosition.X);
                Canvas.SetTop(tooltip, actualPosition.Y);
                Canvas.SetZIndex(tooltip, 1);
            }
            else
                tooltip.Visibility = Visibility.Collapsed;
        }

        private object CheckBounds(Point currentPoint)
        {
            Annotation selectedAxisAnnotation = null, selectedPixelAnnotation = null;
            var annotations = Annotations.Where(axis => axis.CoordinateUnit == CoordinateUnit.Axis).Where(annotation => annotation is TextAnnotation);
            if (annotations.Count() > 0)
            {
                foreach (Annotation annotation in annotations)
                {
                    if (annotation.RotatedRect.Contains(currentPoint))
                        selectedAxisAnnotation= annotation;
                }
            }

            annotations = Annotations.Where(pixel => pixel.CoordinateUnit == CoordinateUnit.Pixel).Where(annotation => annotation is TextAnnotation);
            if (annotations.Count() > 0)
            {
                foreach (Annotation annotation in annotations)
                {
                    if (annotation.RotatedRect.Contains(currentPoint))
                        selectedPixelAnnotation = annotation;
                } 
            }
            return (selectedPixelAnnotation != null ? selectedPixelAnnotation : selectedAxisAnnotation);
        }

        private void ResetTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Interval = TimeSpan.FromMilliseconds(toolTipDuration);
                _timer.Start();
            }
            else
            {
                _timer = new DispatcherTimer();
                _timer.Tick += OnTimeout;
                _timer.Start();
            }
        }

        private void OnTimeout(object sender, object e)
        {
            if (tooltip != null)
            {
                tooltip.Visibility = Visibility.Collapsed;
            }
        }
        
        /// <summary>
        /// Generate the position of the tooltip according the tooltip placement.
        /// </summary>
        private Point GetToolTipPosition(Point currentPosition, Annotation annotation)
        {
            tooltip.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            switch (annotation.ToolTipPlacement)
            {
                case ToolTipLabelPlacement.Left:
                    currentPosition.X -= tooltip.DesiredSize.Width;
                    break;
                case ToolTipLabelPlacement.Top:
                    currentPosition.Y -= tooltip.DesiredSize.Height;
                    break;
                case ToolTipLabelPlacement.Bottom:
                    currentPosition.Y += tooltip.DesiredSize.Height;
                    break;
                case ToolTipLabelPlacement.Right:
                    tooltip.Margin = new Thickness(10, 0, 0, 0);
                    break;
            }
            return currentPosition;
        }

        

        private Annotation GetSelectedAnnotation(IEnumerable<Annotation> annotations, Point currentPosition)
        {
            Annotation selectedAnnotation = null;
            foreach (Annotation annotation in annotations)
            {
                annotation.IsSelected = annotation.RotatedRect.Contains(currentPosition) && annotation is ShapeAnnotation;
                if (annotation.IsSelected)
                    selectedAnnotation = annotation;
                else if (annotation is VerticalLineAnnotation && (annotation as VerticalLineAnnotation).AxisMarkerObject!=null 
                    &&(annotation as VerticalLineAnnotation).AxisMarkerObject.RotatedRect.Contains(currentPosition))
                    selectedAnnotation = (annotation as VerticalLineAnnotation).AxisMarkerObject;
                else if (annotation is HorizontalLineAnnotation && (annotation as HorizontalLineAnnotation).AxisMarkerObject!=null 
                    && (annotation as HorizontalLineAnnotation).AxisMarkerObject.RotatedRect.Contains(currentPosition))
                    selectedAnnotation = (annotation as HorizontalLineAnnotation).AxisMarkerObject;
            }

            return selectedAnnotation;
        }

        void AnnotationDrag(double xTranslate, double yTranslate)
        {
            if (SelectedAnnotation != null)
            {
                ShapeAnnotation selectedAnnotation = (SelectedAnnotation as ShapeAnnotation);
                bool isDraggable = (selectedAnnotation.CanResize && AnnotationResizer != null) ? !AnnotationResizer.IsResizing
                    : (selectedAnnotation is LineAnnotation) ? !(selectedAnnotation as LineAnnotation).IsResizing : true;
                object x1, x2, y1, y2;
                if (selectedAnnotation.CanDrag && isDraggable)
                {
                    selectedAnnotation.IsDragging = true;
                    bool isXAxis = selectedAnnotation.DraggingMode == AxisMode.Horizontal;
                    bool isYAxis = selectedAnnotation.DraggingMode == AxisMode.Vertical;
                    bool isAll = selectedAnnotation.DraggingMode == AxisMode.All;
                    if (selectedAnnotation.CoordinateUnit == CoordinateUnit.Pixel)
                    {
#if WINDOWS_PHONE8 || WINDOWS_PHONE7
                        if (selectedAnnotation is SolidShapeAnnotation && (selectedAnnotation as SolidShapeAnnotation).Angle > 0)
                        {
                            x1 = (isAll || isXAxis) ? Convert.ToDouble(selectedAnnotation.X1) - xTranslate : selectedAnnotation.X1;
                            x2 = (isAll || isXAxis) ? Convert.ToDouble(selectedAnnotation.X2) - xTranslate : selectedAnnotation.X2;
                            y1 = (isAll || isYAxis) ? Convert.ToDouble(selectedAnnotation.Y1) - yTranslate : selectedAnnotation.Y1;
                            y2 = (isAll || isYAxis) ? Convert.ToDouble(selectedAnnotation.Y2) - yTranslate : selectedAnnotation.Y2;
                        }
                        else
                        {
#endif
                        x1 = (isAll || isXAxis) ? Convert.ToDouble(selectedAnnotation.X1) + xTranslate : selectedAnnotation.X1;
                        x2 = (isAll || isXAxis) ? Convert.ToDouble(selectedAnnotation.X2) + xTranslate : selectedAnnotation.X2;
                        y1 = (isAll || isYAxis) ? Convert.ToDouble(selectedAnnotation.Y1) + yTranslate : selectedAnnotation.Y1;
                        y2 = (isAll || isYAxis) ? Convert.ToDouble(selectedAnnotation.Y2) + yTranslate : selectedAnnotation.Y2;
#if WINDOWS_PHONE8 || WINDOWS_PHONE7
                        }
#endif
                    }
                    else
                    {
                        xTranslate = selectedAnnotation.XAxis.IsInversed ? -xTranslate : xTranslate;
                        yTranslate = selectedAnnotation.YAxis.IsInversed ? -yTranslate : yTranslate;
                        double xAxisChange = selectedAnnotation.XAxis.PixelToCoefficientValue(xTranslate);
                        double yAxisChange = selectedAnnotation.YAxis.PixelToCoefficientValue(yTranslate);
#if WINDOWS_PHONE8 || WINDOWS_PHONE7
                        if (selectedAnnotation is SolidShapeAnnotation && (selectedAnnotation as SolidShapeAnnotation).Angle > 0)
                        {
                            x2 = (isAll || isXAxis) ? selectedAnnotation.ConvertToObject(CalculatePointValue(selectedAnnotation.X2, xAxisChange, true, true), selectedAnnotation.XAxis) : selectedAnnotation.X2;
                            x1 = (isAll || isXAxis) ? selectedAnnotation.ConvertToObject(CalculatePointValue(selectedAnnotation.X1, xAxisChange, true, true), selectedAnnotation.XAxis) : selectedAnnotation.X1;
                            y2 = (isAll || isYAxis) ? selectedAnnotation.ConvertToObject(CalculatePointValue(selectedAnnotation.Y2, yAxisChange, false, true), selectedAnnotation.YAxis) : selectedAnnotation.Y2;
                            y1 = (isAll || isYAxis) ? selectedAnnotation.ConvertToObject(CalculatePointValue(selectedAnnotation.Y1, yAxisChange, false, true), selectedAnnotation.YAxis) : selectedAnnotation.Y1;
                        }
                        else
                        {
#endif
                        x2 = (isAll || isXAxis) ? selectedAnnotation.ConvertToObject(CalculatePointValue(selectedAnnotation.X2, xAxisChange, true, false), selectedAnnotation.XAxis) : selectedAnnotation.X2;
                        x1 = (isAll || isXAxis) ? selectedAnnotation.ConvertToObject(CalculatePointValue(selectedAnnotation.X1, xAxisChange, true, false), selectedAnnotation.XAxis) : selectedAnnotation.X1;
                        y2 = (isAll || isYAxis) ? selectedAnnotation.ConvertToObject(CalculatePointValue(selectedAnnotation.Y2, yAxisChange, false, false), selectedAnnotation.YAxis) : selectedAnnotation.Y2;
                        y1 = (isAll || isYAxis) ? selectedAnnotation.ConvertToObject(CalculatePointValue(selectedAnnotation.Y1, yAxisChange, false, false), selectedAnnotation.YAxis) : selectedAnnotation.Y1;
#if WINDOWS_PHONE8 || WINDOWS_PHONE7
                        }
#endif
                    }
                    if (AnnotationResizer != null && !AnnotationResizer.IsResizing)
                    {
                        selectedAnnotation.X1 = AnnotationResizer.X1 = x1;
                        selectedAnnotation.X2 = AnnotationResizer.X2 = x2;
                        selectedAnnotation.Y1 = AnnotationResizer.Y1 = y1;
                        selectedAnnotation.Y2 = AnnotationResizer.Y2 = y2;
                    }
                    else
                    {
                        selectedAnnotation.X1 = x1;
                        selectedAnnotation.X2 = x2;
                        selectedAnnotation.Y1 = y1;
                        selectedAnnotation.Y2 = y2;
                    }
                    if (selectedAnnotation is AxisMarker)
                    {
                        if ((selectedAnnotation as AxisMarker).ParentAnnotation is VerticalLineAnnotation)
                        {
                            if ((selectedAnnotation as AxisMarker).ParentAnnotation.XAxis.Orientation == Orientation.Horizontal)
                                (selectedAnnotation as AxisMarker).ParentAnnotation.X1 = selectedAnnotation.X1;
                            else
                                (selectedAnnotation as AxisMarker).ParentAnnotation.Y1 = selectedAnnotation.Y1;
                        }
                        else
                        {
                            if ((selectedAnnotation as AxisMarker).ParentAnnotation.XAxis.Orientation == Orientation.Vertical)
                                (selectedAnnotation as AxisMarker).ParentAnnotation.X1 = selectedAnnotation.X1;
                            else
                                (selectedAnnotation as AxisMarker).ParentAnnotation.Y1 = selectedAnnotation.Y1;
                        }
                    }
                    if  (AnnotationResizer!=null)
                        AnnotationResizer.MapActualValueToPixels();
                    selectedAnnotation.IsDragging = false;
                }

            }
        }

        double CalculatePointValue(object value, double change, bool isXAxis, bool isAngleInPhone)
        {
            ShapeAnnotation selectedAnnotation = (SelectedAnnotation is ShapeAnnotation) ? (SelectedAnnotation as ShapeAnnotation) : null;
            change = isAngleInPhone ? change * -1 : change;
            if (isXAxis)
                 return selectedAnnotation.ConvertData(value, selectedAnnotation.XAxis) + change;
            return selectedAnnotation.ConvertData(value, selectedAnnotation.YAxis) - change;
        }


        void OnAxesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var annotation in Annotations)
            {
                annotation.SetAxisFromName();
            }
        }

        void OnChartSizeChanged(object sender, SizeChangedEventArgs e)
        {
#if WPF
            this.chart.ChartAnnotationCanvas.ClipToBounds = true;
#else
            this.chart.ChartAnnotationCanvas.Clip = new RectangleGeometry() { Rect = new Rect(new Point(0,0), e.NewSize) };                       
#endif
        }

        void OnSeriesBoundsChanged(object sender, ChartSeriesBoundsEventArgs e)
        {
            this.chart.SeriesAnnotationCanvas.Clip = new RectangleGeometry() { Rect = this.chart.SeriesClipRect };
        }

        private AnnotationCollection annotations;

        internal AnnotationCollection Annotations
        {
            get
            {
                return annotations;
            }
            set
            {
                if (annotations != value)
                {
                    if (annotations != null)
                    {
                        annotations.CollectionChanged -= OnAnnotationsCollectionChanged;
                        foreach (var oldAnnotation in annotations)
                        {
                                AddOrRemoveAnnotations(oldAnnotation,true);
                        }
                    }
                    
                    annotations = value;
                    if (annotations != null)
                        annotations.CollectionChanged += OnAnnotationsCollectionChanged;
                   
                    if (annotations != null)
                        AddAnnotations();
                }
                if(chart!=null)
                {
                    foreach (ChartAxis axis in chart.Axes)
                    {
                    axis.AxisBoundsChanged -= OnAxisBoundsChanged;
                    axis.VisibleRangeChanged -= OnAxisVisibleRangeChanged;
                    }
                   foreach (ChartAxis axis in chart.Axes)
                   {
                    axis.AxisBoundsChanged += OnAxisBoundsChanged;
                    axis.VisibleRangeChanged += OnAxisVisibleRangeChanged;
                   }
                }
            }
        }

        void OnAxisVisibleRangeChanged(object sender, VisibleRangeChangedEventArgs e)
        {
            foreach (Annotation annotation in this.Annotations)
            {
                if (annotation.XAxis == sender || annotation.YAxis == sender)
                    annotation.UpdateAnnotation();
            }
            if (AnnotationResizer != null && (AnnotationResizer.XAxis == sender || AnnotationResizer.YAxis == sender))
            {
                AnnotationResizer.UpdateAnnotation();
                AnnotationResizer.MapActualValueToPixels();
            }
        }

        void OnAxisBoundsChanged(object sender, ChartAxisBoundsEventArgs e)
        {
            foreach (Annotation annotation in this.Annotations)
            {
                if (annotation.XAxis == sender || annotation.YAxis == sender)
                    annotation.UpdateAnnotation();
            }
            if (AnnotationResizer != null && (AnnotationResizer.XAxis == sender || AnnotationResizer.YAxis == sender))
            {
                AnnotationResizer.UpdateAnnotation();
                AnnotationResizer.MapActualValueToPixels();
            }
        }

        void AddAnnotations()
        {
            
            foreach (Annotation annotation in this.Annotations)
            {
                UIElement annotationElement = annotation.CreateAnnotation();
                annotation.Chart = chart;
                if (annotation.Visibility != Visibility.Collapsed)
                {
                    if (annotationElement != null && !(annotation is AxisMarker))
                    {
                        switch (annotation.CoordinateUnit)
                        {
                            case CoordinateUnit.Axis:
                                this.chart.SeriesAnnotationCanvas.Children.Add(annotation);
                                this.chart.SeriesAnnotationCanvas.Children.Add(annotationElement);
                                break;

                            case CoordinateUnit.Pixel:
                                this.chart.ChartAnnotationCanvas.Children.Add(annotation);
                                this.chart.ChartAnnotationCanvas.Children.Add(annotationElement);
                                break;
                        }
                        annotation.IsAddedToVisualTree = true;
                    }
                    else
                    {
                        this.chart.ChartAnnotationCanvas.Children.Add(annotationElement);
                        annotation.IsAddedToVisualTree = true;
                    }
                    annotation.UpdateAnnotation();
                }
            }
        }

        internal void AddOrRemoveAnnotations(Annotation annotation, bool isRemoval)
        {
            annotation.Chart = chart;
            UIElement annotationElement = null;
            if (annotation.IsVisbilityChanged)
                annotationElement = annotation.GetRenderedAnnotation();
            else
                annotationElement = !isRemoval ? annotation.CreateAnnotation() : annotation.GetRenderedAnnotation();
            annotation.IsVisbilityChanged = false;
            if (annotationElement != null && !(annotation is AxisMarker))
            {
                switch (annotation.CoordinateUnit)
                {
                      case CoordinateUnit.Axis:
                        if (this.chart.SeriesAnnotationCanvas.Children.Contains(annotationElement) && isRemoval)
                        {
                            this.chart.SeriesAnnotationCanvas.Children.Remove(annotation);
                            RemoveAxisMarker(annotation);
                            this.chart.SeriesAnnotationCanvas.Children.Remove(annotationElement);
                            annotation.IsAddedToVisualTree = false;
                        }
                        else
                        {
                            this.chart.SeriesAnnotationCanvas.Children.Add(annotation);
                            this.chart.SeriesAnnotationCanvas.Children.Add(annotationElement);
                            annotation.IsAddedToVisualTree = true;
                            annotation.UpdateAnnotation();
                        }
                            break;
                      case CoordinateUnit.Pixel:
                            if (this.chart.ChartAnnotationCanvas.Children.Contains(annotationElement) && isRemoval)
                            {
                                this.chart.ChartAnnotationCanvas.Children.Remove(annotation);
                                this.chart.ChartAnnotationCanvas.Children.Remove(annotationElement);
                                annotation.IsAddedToVisualTree = false;
                            }
                            else
                            {
                                this.chart.ChartAnnotationCanvas.Children.Add(annotation);
                                this.chart.ChartAnnotationCanvas.Children.Add(annotationElement);
                                annotation.IsAddedToVisualTree = true;
                                annotation.UpdateAnnotation();
                        }
                        break;
                }
            }
            else
            {
                if (this.chart.ChartAnnotationCanvas.Children.Contains((annotation as AxisMarker).MarkerCanvas) && isRemoval)
                {
                    this.chart.ChartAnnotationCanvas.Children.Remove((annotation as AxisMarker).MarkerCanvas);
                    annotation.IsAddedToVisualTree = false;
                }
                else if((annotation as AxisMarker).ParentAnnotation.Visibility!= Visibility.Collapsed)
                {
                    this.chart.ChartAnnotationCanvas.Children.Add(annotationElement);
                    annotation.IsAddedToVisualTree = true;
                    annotation.UpdateAnnotation();
                }
            }
        }

        private void RemoveAxisMarker(Annotation annotation)
        {
            if (annotation is VerticalLineAnnotation && (annotation as VerticalLineAnnotation).ShowAxisLabel)
                this.chart.ChartAnnotationCanvas.Children.Remove((annotation as VerticalLineAnnotation).AxisMarkerObject.MarkerCanvas);
            else if (annotation is HorizontalLineAnnotation && (annotation as HorizontalLineAnnotation).ShowAxisLabel)
                this.chart.ChartAnnotationCanvas.Children.Remove((annotation as HorizontalLineAnnotation).AxisMarkerObject.MarkerCanvas);
        }

        void OnAnnotationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.chart != null && this.chart.ChartAnnotationCanvas != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (Annotation annotation in e.NewItems)
                        {
                            AddOrRemoveAnnotations(annotation, false);
                        }
                        break;
                    
                    case NotifyCollectionChangedAction.Remove:
                        foreach (Annotation annotation in e.OldItems)
                        {
                            AddOrRemoveAnnotations(annotation, true);
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        foreach (Annotation annotation in e.OldItems)
                        {
                            AddOrRemoveAnnotations(annotation, true);
                        }
                        foreach (Annotation annotation in e.NewItems)
                        {
                            AddOrRemoveAnnotations(annotation, false);
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        this.chart.ChartAnnotationCanvas.Children.Clear();
                        this.chart.SeriesAnnotationCanvas.Children.Clear();
                        foreach (Annotation annotation in Annotations)
                        {
                            annotation.IsAddedToVisualTree = false; 
                        }
                        break;
                   
                }
            }
        }

    }
}
