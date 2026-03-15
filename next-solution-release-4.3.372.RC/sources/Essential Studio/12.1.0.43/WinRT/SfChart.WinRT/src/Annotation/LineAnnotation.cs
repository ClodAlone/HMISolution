#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINDOWS_PHONE
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class LineAnnotation : ShapeAnnotation
    {
        const double minimumSize = 10;
        protected internal double HorizontalChange, VerticalChange;
        internal Canvas LineCanvas;
        protected ArrowLine arrowLine;
        protected bool isAxis = false;
        bool isRotated;
        bool IsVertical 
        { 
            get 
            {
                return !(this is HorizontalLineAnnotation);
            } 
        }

        bool IsHorizontal 
        {  
            get 
            {
                return !(this is VerticalLineAnnotation); 
            }  
        }
        
        double ActualX1
        {
            get
            {
                if (isAxis)
                    return Chart.ValueToPointRelativeToAnnotation(XAxis, this.x1);
                return Convert.ToDouble(X1);
            }
            set { X1 = value; }
        }

        private double ActualX2
        {
            get
            {
                if (isAxis)
                    return Chart.ValueToPointRelativeToAnnotation(XAxis, this.x2);
                return Convert.ToDouble(X2);
            }
            set { X2 = value; }
        }

        private double ActualY1
        {
            get
            {
                if (isAxis)
                    return Chart.ValueToPointRelativeToAnnotation(YAxis, this.y1);
                return Convert.ToDouble(Y1);
            }
            set { Y1 = value; }
        }

        private double ActualY2
        {
            get
            {
                if (isAxis)
                    return Chart.ValueToPointRelativeToAnnotation(YAxis, this.y2);
                return Convert.ToDouble(Y2);
            }
            set { Y2 = value; }
        }

        /// <summary>
        /// Gets or sets the ShowLine
        /// </summary>
        public bool ShowLine
        {
            get { return (bool)GetValue(ShowLineProperty); }
            set { SetValue(ShowLineProperty, value); }
        }

        /// <summary>
        /// The ShowLine property
        /// </summary>
        public static readonly DependencyProperty ShowLineProperty =
            DependencyProperty.Register("ShowLine", typeof(bool), typeof(LineAnnotation), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the line cap
        /// </summary>
        public  LineCap LineCap
        {
            get { return (LineCap)GetValue(LineAnnotionCapProperty); }
            set { SetValue(LineAnnotionCapProperty, value); }
        }

        /// <summary>
        /// The line cap alignment property
        /// </summary>
        public static readonly DependencyProperty LineAnnotionCapProperty =
            DependencyProperty.Register("LineCap", typeof(LineCap), typeof(LineAnnotation), new PropertyMetadata(LineCap.None));
        
        private bool isWithCap { get { return (LineCap== LineCap.Arrow);} }

        internal override UIElement CreateAnnotation()
        {
            isAxis = (CoordinateUnit == Charts.CoordinateUnit.Axis);
            if (AnnotationElement != null && AnnotationElement.Children.Count == 0)
            {
                LineCanvas = new Canvas();
                if (isWithCap)
                {
                    shape = new Path();
                    arrowLine = new ArrowLine();
                }
                else
                    shape = new Line();
                if (ShowLine)
                {
                    shape.Tag = this;
                    if (CanResize)
                        AddThumb();
                    LineCanvas.Children.Add(TextElement);
                    LineCanvas.Children.Add(shape);
                }
                SetBindings();
                AnnotationElement.Children.Add(LineCanvas);
            }
            return AnnotationElement;
        }

        internal void AddThumb()
        {
            nearThumb = new Thumb();
            farThumb = new Thumb();
            nearThumb.Style = ChartDictionaries.GenericCommonDictionary["roundthumbstyle"] as Style;
            farThumb.Style = ChartDictionaries.GenericCommonDictionary["roundthumbstyle"] as Style;
            nearThumb.Visibility = Visibility.Collapsed;
            farThumb.Visibility = Visibility.Collapsed;
#if SILVERLIGHT_UNCOMMON || WPF 
            nearThumb.Margin = new Thickness(-5);
            farThumb.Margin = new Thickness(-5);
#else
            nearThumb.Margin = new Thickness(-10);
            farThumb.Margin = new Thickness(-10);
#endif
            LineCanvas.Children.Add(nearThumb);
            LineCanvas.Children.Add(farThumb);
            nearThumb.DragDelta += OnNearThumbDragDelta;
            farThumb.DragDelta += OnFarThumbDragDelta;
            Canvas.SetZIndex(nearThumb, 1);
            Canvas.SetZIndex(farThumb, 1);
#if WPF
            nearThumb.DragCompleted += OnDragCompleted;
            farThumb.DragCompleted += OnDragCompleted;
#endif
        }

#if WPF
        protected virtual void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.IsResizing = false;
        }
#endif

        void OnFarThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            this.IsResizing = true;
            isRotated = (XAxis.Orientation == Orientation.Vertical && isAxis);
            HorizontalChange = isRotated ? e.VerticalChange : e.HorizontalChange;
            VerticalChange = isRotated ? e.HorizontalChange : e.VerticalChange;
            if (!isRotated)
            {
                if(IsHorizontal)
                    ActualX1 = isAxis ? Chart.PointToAnnotationValue(XAxis, new Point(ActualX1 + HorizontalChange, 0)) : ActualX1 + HorizontalChange;
                if(IsVertical)
                    ActualY1 = isAxis ? Chart.PointToAnnotationValue(YAxis, new Point(0, ActualY1 + VerticalChange)) : ActualY1 + VerticalChange;
            }
            else
            {
                if (IsVertical)
                    ActualX1 = isAxis ? Chart.PointToAnnotationValue(XAxis, new Point(0, ActualX1 + HorizontalChange)) : ActualX1 + HorizontalChange;
                if (IsHorizontal)
                    ActualY1 = isAxis ? Chart.PointToAnnotationValue(YAxis, new Point(ActualY1 + VerticalChange, 0)) : ActualY1 + VerticalChange;
            }
        }

        void OnNearThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            this.IsResizing = true;
            isRotated = (XAxis.Orientation == Orientation.Vertical && isAxis);
            HorizontalChange = isRotated ? e.VerticalChange : e.HorizontalChange;
            VerticalChange = isRotated ? e.HorizontalChange : e.VerticalChange;
            if (!isRotated)
            {
                if(IsHorizontal)
                    ActualX2 = isAxis ? Chart.PointToAnnotationValue(XAxis, new Point(ActualX2 + HorizontalChange, 0)) : ActualX2 + HorizontalChange;
                if(IsVertical)
                    ActualY2 = isAxis ? Chart.PointToAnnotationValue(YAxis, new Point(0, ActualY2 + VerticalChange)) : ActualY2 + VerticalChange;
            }
            else
            {
                if(IsVertical)
                    ActualX2 = isAxis ? Chart.PointToAnnotationValue(XAxis, new Point(0, ActualX2 + HorizontalChange)) : ActualX2 + HorizontalChange;
                if(IsHorizontal)
                    ActualY2 = isAxis ? Chart.PointToAnnotationValue(YAxis, new Point(ActualY2 + VerticalChange, 0)) : ActualY2 + VerticalChange;
            }
        }

        protected override DependencyObject CloneAnnotation(Annotation annotation)
        {
            return base.CloneAnnotation(new LineAnnotation());
        }

        public override void UpdateAnnotation()
        {
            if (shape != null && X1 != null && X2 != null && Y1 != null && Y2 != null)
            {
                switch (CoordinateUnit)
                {
                    case CoordinateUnit.Axis:
                        if (XAxis != null && YAxis != null && ShowLine)
                        {
                            SetData();
                            y2 = ConvertData(Y2, YAxis);
                            x2 = ConvertData(X2, XAxis);
                            Point point = (XAxis.Orientation == Orientation.Horizontal) ? new Point(this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1),
                                                        this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1)) : new Point(this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1),
                                                        this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1));
                            Point point2 = (XAxis.Orientation == Orientation.Horizontal) ? new Point(Chart.ValueToPointRelativeToAnnotation(XAxis, x2),
                                                         this.Chart.ValueToPointRelativeToAnnotation(YAxis, y2)) : new Point(this.Chart.ValueToPointRelativeToAnnotation(YAxis, y2),
                                                         Chart.ValueToPointRelativeToAnnotation(XAxis, x2));
                            DrawLine(point, point2, shape);
                        }
                        break;
                    case CoordinateUnit.Pixel:
                        if (ShowLine)
                        {
                            Point elementPoint1 = new Point(Convert.ToDouble(X1), Convert.ToDouble(Y1));
                            Point elementPoint2 = new Point(Convert.ToDouble(X2), Convert.ToDouble(Y2));
                            DrawLine(elementPoint1, elementPoint2, shape);
                        }
                        break;
                }

            }
        }

        protected void DrawLine(Point point, Point point2, Shape shape)
        {
            Size desiredSize;
            Point ensurePoint;
            Rect heightAndWidthRect;
            Point positionedPoint = new Point(0, 0);
            Point centerPoint = new Point(0, 0);
            double height = 0, width = 0;
            Path path = null;
            Line line = null;
            if (isWithCap)
                path = shape as Path;
            else
                line = shape as Line;
        
            point.Y = (double.IsNaN(point.Y)) ? 0 : point.Y;
            point.X = (double.IsNaN(point.X)) ? 0 : point.X;
            point2.Y = (double.IsNaN(point2.Y)) ? 0 : point2.Y;
            point2.X = (double.IsNaN(point2.X)) ? 0 : point2.X;
            heightAndWidthRect = new Rect(point, point2);
            ensurePoint = this.EnsurePoint(point, point2);
            desiredSize = new Size(heightAndWidthRect.Width, heightAndWidthRect.Height);
            positionedPoint = GetElementPosition(new Size(heightAndWidthRect.Width, heightAndWidthRect.Height), ensurePoint);
            if (isWithCap)
            {
                arrowLine.X1 = point.X;
                arrowLine.Y1 = point.Y;
                arrowLine.X2 = point2.X;
                arrowLine.Y2 = point2.Y;
                path.Data = arrowLine.GetGeometry() as PathGeometry;
            }
            else
            {
                line.X1 = point.X;
                line.Y1 = point.Y;
                line.X2 = point2.X;
                line.Y2 = point2.Y;
            }
            if (CanResize && farThumb!=null && nearThumb != null)
            {
                Canvas.SetLeft(farThumb, point.X);
                Canvas.SetTop(farThumb, point.Y);
                Canvas.SetLeft(nearThumb, point2.X);
                Canvas.SetTop(nearThumb, point2.Y);
            }
            AnnotationElement.Width = heightAndWidthRect.Width;
            AnnotationElement.Height = heightAndWidthRect.Height;
            centerPoint = new Point(positionedPoint.X + (heightAndWidthRect.Width / 2),
                                          positionedPoint.Y + (heightAndWidthRect.Height / 2));
            height = heightAndWidthRect.Height < minimumSize ? minimumSize : heightAndWidthRect.Height;
            width = heightAndWidthRect.Width < minimumSize ? minimumSize : heightAndWidthRect.Width;

            RotatedRect = new Rect(centerPoint.X - (width / 2),
                                           centerPoint.Y - (height / 2), width, height);
            if (TextElement.Content != null)
                SetTextElementPosition(point, point2, desiredSize, positionedPoint, TextElement);
            // This is to avoid line visibility collapse while dragging.
            if (LineCanvas != null && IsDragging)
                LineCanvas.UpdateLayout();
            
        }

        RotateTransform rotate ;
        protected void SetTextElementPosition(Point point, Point point2, Size desiredSize, Point positionedPoint, ContentControl TextElement)
        {
            double slope;
            rotate = new RotateTransform();
            TextElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            
            slope = (point2.Y - point.Y) / (point2.X - point.X);
            rotate.Angle = double.IsNaN(slope) ? 0 : Math.Atan(slope) * (180 / Math.PI);
            if (this is VerticalLineAnnotation)
            {
                switch (this.HorizontalTextAlignment)
                {
                    case HorizontalAlignment.Center:
                        TextElement.RenderTransformOrigin = new Point(0, 0.5);
                        break;
                    case HorizontalAlignment.Left:
                        TextElement.RenderTransformOrigin = new Point(0.7, 0.5);
                        break;
                    case HorizontalAlignment.Right:
                        TextElement.RenderTransformOrigin = new Point(0.2, 0.5);
                        break;
                }
            }
            else
                TextElement.RenderTransformOrigin = new Point(0.5, 0.5);
            TextElement.RenderTransform = rotate;
            Point textPosition = GetTextPosition(desiredSize, positionedPoint, new Size(TextElement.DesiredSize.Width, TextElement.DesiredSize.Height));
            Canvas.SetLeft(TextElement, textPosition.X);
            Canvas.SetTop(TextElement, textPosition.Y);
        }
        protected override Point GetTextPosition(Size desiredSize, Point originalPosition, Size textSize)
        {
            Point point = originalPosition;
            HorizontalAlignment horizontalAlignment = this.HorizontalTextAlignment;
            VerticalAlignment verticalAlignment = this.VerticalTextAlignment;
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    point.X += (desiredSize.Width / 2);
                    if (Math.Abs(rotate.Angle) < 80)
                    point.X -= (textSize.Width/2);
                    break;
                case HorizontalAlignment.Left:
                    point.X -= (textSize.Width);
                    break;
                case HorizontalAlignment.Right:
                    point.X += (desiredSize.Width);
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Center:
                    point.Y += (desiredSize.Height / 2);
                    point.Y -= (textSize.Height / 2);
                    break;
                case VerticalAlignment.Bottom:
                    point.Y += (desiredSize.Height / 2);
                    point.Y -= (textSize.Height / 8);
                    TextElement.Margin = new Thickness(0, minimumSize, 0, 0);
                    break;
                case VerticalAlignment.Top:
                    point.Y += (desiredSize.Height / 2);
                    point.Y -= (textSize.Height);
                    TextElement.Margin = new Thickness(0, 0, 0, minimumSize);
                    break;
            }
            return point;
        }
        protected override void SetBindings()
        {
            base.SetBindings();
            if (this.LineCap == Charts.LineCap.Arrow)
            {
                Binding strokeBinding = new Binding { Source = this, Path = new PropertyPath("Stroke") };
                shape.SetBinding(Path.FillProperty, strokeBinding);
            }
        }

    }
}
