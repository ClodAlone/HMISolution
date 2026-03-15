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
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls.Primitives;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public abstract class ShapeAnnotation : SingleAnnotation
    {

        public ShapeAnnotation()
            : base()
        {            
        }

        internal bool IsDragging { get; set; }

        internal Thumb nearThumb, farThumb;

        protected double x2;

        protected double y2;

        internal Shape shape;

        internal Resizer ResizerControl { get; set; }

        /// <summary>
        /// Gets or sets the horizontal text alignment.
        /// </summary>
        /// <value>
        /// The horizontal text alignment.
        /// </value>
        public HorizontalAlignment HorizontalTextAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalTextAlignmentProperty); }
            set { SetValue(HorizontalTextAlignmentProperty, value); }
        }

        /// <summary>
        /// The horizontal text alignment property
        /// </summary>
        public static readonly DependencyProperty HorizontalTextAlignmentProperty =
            DependencyProperty.Register("HorizontalTextAlignment", typeof(HorizontalAlignment), typeof(ShapeAnnotation), new PropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// Gets or Sets the Annotation Dragging Path
        /// </summary>
        public AxisMode DraggingMode
        {
            get { return (AxisMode)GetValue(DraggingModeProperty); }
            set { SetValue(DraggingModeProperty, value); }
        }

        /// <summary>
        /// The dragging path property
        /// </summary>
        public static readonly DependencyProperty DraggingModeProperty =
            DependencyProperty.Register("DraggingMode", typeof(AxisMode), typeof(ShapeAnnotation), new PropertyMetadata(AxisMode.All));

        /// <summary>
        /// Gets or Sets the CanDrag for Annotation
        /// </summary>
        public bool CanDrag
        {
            get { return (bool)GetValue(CanDragProperty); }
            set { SetValue(CanDragProperty, value); }
        }

        /// <summary>
        /// The CanDrag property
        /// </summary>
        public static readonly DependencyProperty CanResizeProperty =
            DependencyProperty.Register("CanResize", typeof(bool), typeof(ShapeAnnotation), new PropertyMetadata(false, OnCanResizeChanged));

        /// <summary>
        /// Gets or Sets the CanDrag for Annotation
        /// </summary>
        public bool CanResize
        {
            get { return (bool)GetValue(CanResizeProperty); }
            set { SetValue(CanResizeProperty, value); }
        }

        /// <summary>
        /// The CanDrag property
        /// </summary>
        public static readonly DependencyProperty CanDragProperty =
            DependencyProperty.Register("CanDrag", typeof(bool), typeof(ShapeAnnotation), new PropertyMetadata(false));

        private static void OnCanResizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && (d as ShapeAnnotation).Chart != null)
                (d as ShapeAnnotation).UpdateResizer((bool)e.NewValue);
        }

        private void UpdateResizer(bool value)
        {
            if (value && Chart.AnnotationManager.SelectedAnnotation != null)
            {
                Chart.AnnotationManager.OnAnnotationSelected();
            }
            else
            {
                if (Chart.AnnotationManager.AnnotationResizer != null)
                {
                    Chart.AnnotationManager.AddOrRemoveAnnotations(Chart.AnnotationManager.AnnotationResizer, true);
                    Chart.AnnotationManager.AnnotationResizer = null;
                }
                else if (Chart.AnnotationManager.SelectedAnnotation is LineAnnotation)
                {
                    Chart.AnnotationManager.HideLineResizer();
                }
            }
        }

        /// <summary>
        /// Gets or sets the vertical text alignment.
        /// </summary>
        /// <value>
        /// The vertical text alignment.
        /// </value>
        public VerticalAlignment VerticalTextAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalTextAlignmentProperty); }
            set { SetValue(VerticalTextAlignmentProperty, value); }
        }

        /// <summary>
        /// The vertical text alignment property
        /// </summary>
        public static readonly DependencyProperty VerticalTextAlignmentProperty =
            DependencyProperty.Register("VerticalTextAlignment", typeof(VerticalAlignment), typeof(ShapeAnnotation), new PropertyMetadata(VerticalAlignment.Bottom));


        /// <summary>
        /// Gets or sets the fill.
        /// </summary>
        /// <value>
        /// The fill.
        /// </value>
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        /// <summary>
        /// The fill property
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(ShapeAnnotation), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(50, 30, 144, 255))));

        /// <summary>
        /// Gets or Sets the Y2 Property for the ShapeAnnotation
        /// </summary>
        public object Y2
        {
            get { return (object)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        /// <summary>
        /// The y2 property
        /// </summary>
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2", typeof(object), typeof(ShapeAnnotation), new PropertyMetadata(null, OnHeightWidthChanged));

        
        /// <summary>
        /// Gets or Sets the X2 Property for the ShapeAnnotation
        /// </summary>
        public object X2
        {
            get { return (object)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }

        /// <summary>
        /// The x2 property
        /// </summary>
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(object), typeof(ShapeAnnotation), new PropertyMetadata(null, OnHeightWidthChanged));


        private static void OnHeightWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue==null || (args.NewValue!=null && !args.NewValue.ToString().Equals(args.OldValue.ToString())))
            {
                var shapeAnnotation = sender as ShapeAnnotation;
                if (shapeAnnotation != null && shapeAnnotation.XAxis != null && shapeAnnotation.YAxis != null) 
                    shapeAnnotation.HeightWidthChanged();
            }
        }

        internal void HeightWidthChanged()
        {
            switch (CoordinateUnit)
            {
                case CoordinateUnit.Axis:
                    this.y2 = ConvertData(this.Y2, this.YAxis);
                    this.x2 = ConvertData(this.X2, this.XAxis);
                    break;
            }
            if (this is AnnotationResizer)
            {
                (Chart.AnnotationManager.SelectedAnnotation as ShapeAnnotation).X2 = this.X2;
                (Chart.AnnotationManager.SelectedAnnotation as ShapeAnnotation).Y2 = this.Y2;
            }
            UpdateAnnotation();
        }

        /// <summary>
        /// Gets or sets the stroke thickness.
        /// </summary>
        /// <value>
        /// The stroke thickness.
        /// </value>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// The stroke thickness property
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ShapeAnnotation), new PropertyMetadata(1d));


        /// <summary>
        /// Gets or sets the stroke.
        /// </summary>
        /// <value>
        /// The stroke.
        /// </value>
        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// The stroke property
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(ShapeAnnotation), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255,30, 144, 255))));

        /// <summary>
        /// The stroke dash array property
        /// </summary>
        public static readonly DependencyProperty StrokeDashArrayProperty =
            DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(ShapeAnnotation), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the stroke dash array.
        /// </summary>
        /// <value>
        /// The stroke dash array.
        /// </value>
        public DoubleCollection StrokeDashArray
        {
            get { return (DoubleCollection)GetValue(StrokeDashArrayProperty); }
            set { SetValue(StrokeDashArrayProperty, value); }
        }

        /// <summary>
        /// The stroke dash cap property
        /// </summary>
        public static readonly DependencyProperty StrokeDashCapProperty =
            DependencyProperty.Register("StrokeDashCap", typeof(PenLineCap), typeof(ShapeAnnotation), new PropertyMetadata(PenLineCap.Flat));

        /// <summary>
        /// Gets or sets the stroke dash cap.
        /// </summary>
        /// <value>
        /// The stroke dash cap.
        /// </value>
        public PenLineCap StrokeDashCap
        {
            get { return (PenLineCap)GetValue(StrokeDashCapProperty); }
            set { SetValue(StrokeDashCapProperty, value); }
        }

        /// <summary>
        /// The stroke dash offset property
        /// </summary>
        public static readonly DependencyProperty StrokeDashOffsetProperty =
            DependencyProperty.Register("StrokeDashOffset", typeof(double), typeof(ShapeAnnotation), new PropertyMetadata(Shape.StrokeDashOffsetProperty.GetMetadata(typeof(Shape)).DefaultValue));

        /// <summary>
        /// Gets or sets the stroke dash offset.
        /// </summary>
        /// <value>
        /// The stroke dash offset.
        /// </value>
        public double StrokeDashOffset
        {
            get { return (double)GetValue(StrokeDashOffsetProperty); }
            set { SetValue(StrokeDashOffsetProperty, value); }
        }

        /// <summary>
        /// The stroke endline dash cap property
        /// </summary>
        public static readonly DependencyProperty StrokeEndLineCapProperty =
            DependencyProperty.Register("StrokeEndLineCap", typeof(PenLineCap), typeof(ShapeAnnotation), new PropertyMetadata(PenLineCap.Flat));

        /// <summary>
        /// Gets or sets the stroke endline dash cap.
        /// </summary>
        /// <value>
        /// The stroke endline dash cap.
        /// </value>
        public PenLineCap StrokeEndLineCap
        {
            get { return (PenLineCap)GetValue(StrokeEndLineCapProperty); }
            set { SetValue(StrokeEndLineCapProperty, value); }
        }

        /// <summary>
        /// The stroke line join property
        /// </summary>
        public static readonly DependencyProperty StrokeLineJoinProperty =
            DependencyProperty.Register("StrokeLineJoin", typeof(PenLineJoin), typeof(ShapeAnnotation), new PropertyMetadata(Shape.StrokeLineJoinProperty.GetMetadata(typeof(Shape)).DefaultValue));

        /// <summary>
        /// Gets or sets the stroke line join.
        /// </summary>
        /// <value>
        /// The stroke line join.
        /// </value>
        public PenLineJoin StrokeLineJoin
        {
            get { return (PenLineJoin)GetValue(StrokeLineJoinProperty); }
            set { SetValue(StrokeLineJoinProperty, value); }
        }

        /// <summary>
        /// The stroke miter limit property
        /// </summary>
        public static readonly DependencyProperty StrokeMiterLimitProperty =
            DependencyProperty.Register("StrokeMiterLimit", typeof(double), typeof(ShapeAnnotation), new PropertyMetadata(Shape.StrokeMiterLimitProperty.GetMetadata(typeof(Shape)).DefaultValue));

        /// <summary>
        /// Gets or sets the stroke miter limit.
        /// </summary>
        /// <value>
        /// The stroke miter limit.
        /// </value>
        public double StrokeMiterLimit
        {
            get { return (double)GetValue(StrokeMiterLimitProperty); }
            set { SetValue(StrokeMiterLimitProperty, value); }
        }

        /// <summary>
        /// The stroke start line cap property
        /// </summary>
        public static readonly DependencyProperty StrokeStartLineCapProperty =
            DependencyProperty.Register("StrokeStartLineCap", typeof(PenLineCap), typeof(ShapeAnnotation), new PropertyMetadata(Shape.StrokeStartLineCapProperty.GetMetadata(typeof(Shape)).DefaultValue));


        /// <summary>
        /// Gets or sets the stroke start line cap.
        /// </summary>
        /// <value>
        /// The stroke start line cap.
        /// </value>
        public PenLineCap StrokeStartLineCap
        {
            get { return (PenLineCap)GetValue(StrokeStartLineCapProperty); }
            set { SetValue(StrokeStartLineCapProperty, value); }
        }

        protected override void SetBindings()
        {
            base.SetBindings();
            Binding fillBinding = new Binding { Source = this, Path = new PropertyPath("Fill") };
            shape.SetBinding(Shape.FillProperty, fillBinding);

            Binding opacityBinding = new Binding { Source = this, Path = new PropertyPath("Opacity") };
            shape.SetBinding(Shape.OpacityProperty, opacityBinding);

            Binding strokeBinding = new Binding { Source = this, Path = new PropertyPath("Stroke") };
            shape.SetBinding(Shape.StrokeProperty, strokeBinding);

            Binding strokeThicknessBinding = new Binding { Source = this, Path = new PropertyPath("StrokeThickness") };
            shape.SetBinding(Shape.StrokeThicknessProperty, strokeThicknessBinding);

            Binding strokeDashArray = new Binding { Source = this, Path = new PropertyPath("StrokeDashArray") };
            shape.SetBinding(Shape.StrokeDashArrayProperty, strokeDashArray);

            Binding strokeDashCapBinding = new Binding { Source = this, Path = new PropertyPath("StrokeDashCap") };
            shape.SetBinding(Shape.StrokeDashCapProperty, strokeDashCapBinding);

            Binding strokeDashOffsetBinding = new Binding { Source = this, Path = new PropertyPath("StrokeDashOffset") };
            shape.SetBinding(Shape.StrokeDashOffsetProperty, strokeDashOffsetBinding);

            Binding strokeEndLineCapBinding = new Binding { Source = this, Path = new PropertyPath("StrokeEndLineCap") };
            shape.SetBinding(Shape.StrokeEndLineCapProperty, strokeEndLineCapBinding);

            Binding strokeLineJoinBinding = new Binding { Source = this, Path = new PropertyPath("StrokeLineJoin") };
            shape.SetBinding(Shape.StrokeLineJoinProperty, strokeLineJoinBinding);

            Binding strokeMiterBinding = new Binding { Source = this, Path = new PropertyPath("StrokeMiterLimit") };
            shape.SetBinding(Shape.StrokeMiterLimitProperty, strokeMiterBinding);

            Binding strokeStartLineCapBinding = new Binding { Source = this, Path = new PropertyPath("StrokeStartLineCap") };
            shape.SetBinding(Shape.StrokeStartLineCapProperty, strokeStartLineCapBinding);


        }

        public override void UpdateAnnotation()
        {
            if ((shape != null || ResizerControl != null) && X1 != null && Y1 != null && X2 != null && Y2 != null)
            {
                Point textPosition = new Point();
                Size desiredSize;
                Point centerPoint;
                Rect rotated;
                Rect heightAndWidthRect;
                double angle=(this as SolidShapeAnnotation).Angle;
                RotateTransform rotate = new RotateTransform { Angle = angle };
                Point positionedPoint = new Point(0, 0);
                Point ensurePoint;
                switch (CoordinateUnit)
                {
                    case CoordinateUnit.Axis:
                        base.UpdateAnnotation();
                        if (XAxis != null && YAxis != null)
                        {
                            y2 = ConvertData(Y2, YAxis);
                            x2 = ConvertData(X2, XAxis);
                            Point point = (XAxis.Orientation == Orientation.Horizontal) ? new Point(this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1),
                                                    this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1)) : new Point(this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1),
                                                    this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1));
                            Point point2 = (XAxis.Orientation == Orientation.Horizontal) ? new Point(Chart.ValueToPointRelativeToAnnotation(XAxis, x2),
                                                     this.Chart.ValueToPointRelativeToAnnotation(YAxis, y2)) : new Point(this.Chart.ValueToPointRelativeToAnnotation(YAxis, y2),
                                                     Chart.ValueToPointRelativeToAnnotation(XAxis, x2));

                            point.Y = (double.IsNaN(point.Y)) ? 0 : point.Y;
                            point.X = (double.IsNaN(point.X)) ? 0 : point.X;
                            point2.Y = (double.IsNaN(point2.Y)) ? 0 : point2.Y;
                            point2.X = (double.IsNaN(point2.X)) ? 0 : point2.X;
                            
                            heightAndWidthRect = new Rect(point, point2);
                            if (shape != null)
                            {
                                shape.Height = heightAndWidthRect.Height;
                                shape.Width = heightAndWidthRect.Width;
                            }
                            else
                            {
                                ResizerControl.Height = heightAndWidthRect.Height;
                                ResizerControl.Width = heightAndWidthRect.Width;
                            }
                            AnnotationElement.Height = heightAndWidthRect.Height;
                            AnnotationElement.Width = heightAndWidthRect.Width;
                            ensurePoint = this.EnsurePoint(point, point2);
                            desiredSize = new Size(heightAndWidthRect.Width, heightAndWidthRect.Height);
                            positionedPoint = GetElementPosition(
                                new Size(heightAndWidthRect.Width, heightAndWidthRect.Height), ensurePoint);
                            TextElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                            textPosition = GetTextPosition(desiredSize, new Point(0, 0), new Size(TextElement.DesiredSize.Width, TextElement.DesiredSize.Height));
                            Canvas.SetLeft(AnnotationElement, positionedPoint.X);
                            Canvas.SetTop(AnnotationElement, positionedPoint.Y);
                            Canvas.SetLeft(TextElement, textPosition.X);
                            Canvas.SetTop(TextElement, textPosition.Y);
                            centerPoint = new Point(positionedPoint.X + (AnnotationElement.Width / 2),
                                                          positionedPoint.Y + (AnnotationElement.Height / 2));
                            rotated = this.RotateElement(angle, AnnotationElement, new Size(this.AnnotationElement.Width, this.AnnotationElement.Height));
                            AnnotationElement.RenderTransformOrigin = new Point(0.5, 0.5);
                            AnnotationElement.RenderTransform = rotate;

                            if (angle > 0)
                                RotatedRect = new Rect(centerPoint.X - (rotated.Width / 2),
                                                       centerPoint.Y - (rotated.Height / 2), rotated.Width, rotated.Height);
                            else
                                RotatedRect = new Rect(centerPoint.X - (AnnotationElement.Width / 2),
                                                       centerPoint.Y - (AnnotationElement.Height / 2),
                                                       AnnotationElement.Width, AnnotationElement.Height);
                         
                        }
                        break;
                    case CoordinateUnit.Pixel:
                        Point elementPoint1 = new Point(Convert.ToDouble(X1), Convert.ToDouble(Y1));
                        Point elementPoint2 = new Point(Convert.ToDouble(X2), Convert.ToDouble(Y2));
                        heightAndWidthRect = new Rect(elementPoint1, elementPoint2);
                        if (shape != null)
                        {
                            shape.Height = heightAndWidthRect.Height;
                            shape.Width = heightAndWidthRect.Width;
                        }
                        else
                        {
                            ResizerControl.Height = heightAndWidthRect.Height;
                            ResizerControl.Width = heightAndWidthRect.Width;
                        }
                        AnnotationElement.Height = heightAndWidthRect.Height;
                        AnnotationElement.Width = heightAndWidthRect.Width;
                        ensurePoint = this.EnsurePoint(elementPoint1, elementPoint2);
                        positionedPoint = shape != null ? GetElementPosition(shape, ensurePoint) : GetElementPosition(ResizerControl, ensurePoint); ;
                        desiredSize = (shape != null) ? new Size(shape.Width, shape.Height) : new Size(ResizerControl.Width, ResizerControl.Height);
                        TextElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        textPosition = GetTextPosition(desiredSize, new Point(0, 0),
                                                       new Size(TextElement.DesiredSize.Width, TextElement.DesiredSize.Height));
                        Canvas.SetLeft(AnnotationElement, positionedPoint.X);
                        Canvas.SetTop(AnnotationElement, positionedPoint.Y);
                        
                        Canvas.SetLeft(TextElement, textPosition.X);
                        Canvas.SetTop(TextElement, textPosition.Y);
                        centerPoint = new Point(positionedPoint.X + (AnnotationElement.Width / 2),
                                                         positionedPoint.Y + (AnnotationElement.Height / 2));
                        rotated = this.RotateElement(angle, AnnotationElement, new Size(this.AnnotationElement.Width, this.AnnotationElement.Height));
                        AnnotationElement.RenderTransformOrigin = new Point(0.5, 0.5);
                        AnnotationElement.RenderTransform = rotate;

                        if (angle > 0)
                            RotatedRect = new Rect(centerPoint.X - (rotated.Width / 2),
                                                   centerPoint.Y - (rotated.Height / 2), rotated.Width, rotated.Height);
                        else
                            RotatedRect = new Rect(centerPoint.X - (AnnotationElement.Width / 2),
                                                   centerPoint.Y - (AnnotationElement.Height / 2),
                                                   AnnotationElement.Width, AnnotationElement.Height);
                        break;
                }


            }
        }
        protected Point GetTextPosition(Size desiredSize, Point originalPosition)
        {
            Point point = originalPosition;
            HorizontalAlignment horizontalAlignment = this.HorizontalTextAlignment;
            VerticalAlignment verticalAlignment = this.VerticalTextAlignment;
            Size textSize = new Size(TextElement.ActualWidth, TextElement.ActualHeight);
            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    point.X += (desiredSize.Width / 2);
                    point.X -= (textSize.Width / 2);
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
                case VerticalAlignment.Bottom:
                    point.Y += (desiredSize.Height);
                    break;
                case VerticalAlignment.Center:
                    point.Y += (desiredSize.Height / 2);
                    point.Y -= (textSize.Height / 2);
                    break;
                case VerticalAlignment.Top:
                    point.Y -= (textSize.Height);
                    break;
                
            }
            return point;
        }

        protected virtual Point GetTextPosition(Size desiredSize, Point originalPosition, Size textSize)
        {
            Point point = originalPosition;
            HorizontalAlignment horizontalAlignment = this.HorizontalTextAlignment;
            VerticalAlignment verticalAlignment = this.VerticalTextAlignment;

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                    point.X += (desiredSize.Width / 2);
                    point.X -= (textSize.Width / 2);
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
                case VerticalAlignment.Bottom:
                    point.Y += (desiredSize.Height);
                    break;
                case VerticalAlignment.Center:
                    point.Y += (desiredSize.Height / 2);
                    point.Y -= (textSize.Height / 2);
                    break;
                case VerticalAlignment.Top:
                    point.Y -= (textSize.Height);
                    break;
            }
            return point;
        }

        protected override DependencyObject CloneAnnotation(Annotation annotation)
        {
            ShapeAnnotation shapeAnnotation = annotation as ShapeAnnotation;
            if (shapeAnnotation is SolidShapeAnnotation)
                (shapeAnnotation as SolidShapeAnnotation).Angle = (this as SolidShapeAnnotation).Angle;
            shapeAnnotation.Fill = this.Fill;
            shapeAnnotation.HorizontalTextAlignment = this.HorizontalTextAlignment;
            shapeAnnotation.Stroke = this.Stroke;
            shapeAnnotation.StrokeDashArray = this.StrokeDashArray;
            shapeAnnotation.StrokeDashCap = this.StrokeDashCap;
            shapeAnnotation.StrokeDashOffset = this.StrokeDashOffset;
            shapeAnnotation.StrokeEndLineCap = this.StrokeEndLineCap;
            shapeAnnotation.StrokeLineJoin = this.StrokeLineJoin;
            shapeAnnotation.StrokeMiterLimit = this.StrokeMiterLimit;
            shapeAnnotation.StrokeStartLineCap = this.StrokeStartLineCap;
            shapeAnnotation.StrokeThickness = this.StrokeThickness;
            shapeAnnotation.VerticalTextAlignment = this.VerticalTextAlignment;
            shapeAnnotation.X2 = this.X2;
            shapeAnnotation.Y2 = this.Y2;
            return base.CloneAnnotation(shapeAnnotation);
        }
    }
}
