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
#else
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class TextAnnotation : SingleAnnotation
    {
        /// <summary>
        /// Gets or Sets the Rotation Angle for Annotation
        /// </summary>
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        /// <summary>
        /// The angle property
        /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(SingleAnnotation), new PropertyMetadata(0d, OnUpdatePropertyChanged));

        private static void OnUpdatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var annotation = sender as Annotation;
            if (annotation != null) annotation.UpdatePropertyChanged(args);
        }

        internal override UIElement CreateAnnotation()
        {
            if (AnnotationElement != null && AnnotationElement.Children.Count == 0)
            {
                TextElement = new ContentControl();
                SetBindings();
                TextElement.Tag = this;
                AnnotationElement.Children.Add(TextElement);
            }
            return AnnotationElement;
        }        

        public override void UpdateAnnotation()
        {
            
            if (AnnotationElement != null && TextElement!=null)
            {
                RotateTransform rotate = new RotateTransform { Angle = this.Angle };
                Rect rotated;
                Point centerPoint;
                switch (CoordinateUnit)
                {
                    case CoordinateUnit.Axis:
                        base.UpdateAnnotation();
                        if (XAxis != null && YAxis != null)
                        {
                            Point point = (XAxis.Orientation == Orientation.Horizontal) ? new Point(Chart.ValueToPointRelativeToAnnotation(XAxis, x1),
                                                    Chart.ValueToPointRelativeToAnnotation(YAxis, y1)) : new Point(Chart.ValueToPointRelativeToAnnotation(YAxis, y1),
                                                    Chart.ValueToPointRelativeToAnnotation(XAxis, x1));

                            point.Y = (double.IsNaN(point.Y)) ? 0 : point.Y;
                            point.X = (double.IsNaN(point.X)) ? 0 : point.X;
                            TextElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                            Size TextSize = TextElement.DesiredSize;
                            Point positionedPoint = GetElementPosition(TextSize, point);
                            centerPoint = new Point(positionedPoint.X + (TextSize.Width/2),
                                                          positionedPoint.Y + (TextSize.Height/2));
                            rotated = this.RotateElement(this.Angle, TextElement, TextSize);
                            AnnotationElement.Height = TextSize.Height;
                            AnnotationElement.Width = TextSize.Width;
                            
                            Canvas.SetLeft(AnnotationElement, positionedPoint.X);
                            Canvas.SetTop(AnnotationElement, positionedPoint.Y);

                            AnnotationElement.RenderTransformOrigin = new Point(0.5, 0.5);
                            AnnotationElement.RenderTransform = rotate;

                            if (this.Angle > 0)
                                RotatedRect = new Rect(centerPoint.X - (rotated.Width/2),
                                                       centerPoint.Y - (rotated.Height/2), rotated.Width, rotated.Height);
                            else
                                RotatedRect = new Rect(centerPoint.X - (TextSize.Width/2),
                                                       centerPoint.Y - (TextSize.Height/2),
                                                       TextSize.Width, TextSize.Height);
                            
                        }
                        break;
                    case CoordinateUnit.Pixel:
                        TextElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        Size textSize = TextElement.DesiredSize;
                        AnnotationElement.Height = textSize.Height;
                        AnnotationElement.Width = textSize.Width;
                        Point positionedPixel = GetElementPosition(textSize,
                                                                   new Point(Convert.ToDouble(X1), Convert.ToDouble(Y1)));
                        centerPoint = new Point(positionedPixel.X + (textSize.Width / 2),
                                                         positionedPixel.Y + (textSize.Height / 2));
                        Canvas.SetLeft(AnnotationElement, positionedPixel.X);
                        Canvas.SetTop(AnnotationElement, positionedPixel.Y);
                        AnnotationElement.RenderTransformOrigin = new Point(0.5, 0.5);
                        AnnotationElement.RenderTransform = rotate;
                        rotated = this.RotateElement(this.Angle, TextElement, textSize);
                        if (this.Angle > 0)
                           RotatedRect = new Rect(centerPoint.X - (rotated.Width / 2),
                                                    centerPoint.Y - (rotated.Height / 2), rotated.Width, rotated.Height);
                        else
                            RotatedRect = new Rect(centerPoint.X - (textSize.Width / 2),
                                                    centerPoint.Y - (textSize.Height / 2),
                                                    textSize.Width, textSize.Height);
                        break;
                }
            }

        }

              

        public override UIElement GetRenderedAnnotation()
        {
            return AnnotationElement;
        }

        protected override DependencyObject CloneAnnotation(Annotation annotation)
        {
            TextAnnotation textAnnotation = new TextAnnotation();
            textAnnotation.Angle = this.Angle;
            return base.CloneAnnotation(textAnnotation);
        }
    }
}
