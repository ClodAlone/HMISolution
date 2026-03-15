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
using System.Windows.Media.Imaging;

#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

#endif

namespace Syncfusion.UI.Xaml.Charts
{
    public class ImageAnnotation : SingleAnnotation
    {
        private Image _image;

        public ImageAnnotation()
            : base()
        {
            
        }

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
            DependencyProperty.Register("Angle", typeof(double), typeof(ImageAnnotation), new PropertyMetadata(0d, OnUpdatePropertyChanged));

        private static void OnUpdatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var annotation = sender as Annotation;
            if (annotation != null) annotation.UpdatePropertyChanged(args);
        }


#if NETFX_CORE
        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(string), typeof(ImageAnnotation), new PropertyMetadata(string.Empty,OnImageSourceChanged));
#else
        /// <summary>
        /// Gets or sets the image source.
        /// </summary>
        /// <value>
        /// The image source.
        /// </value>
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        /// <summary>
        /// The image source property
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageAnnotation), new PropertyMetadata(null,OnImageSourceChanged));
#endif

        private static void OnImageSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var _imageAnnotation = sender as ImageAnnotation;
            if (_imageAnnotation != null && _imageAnnotation.XAxis != null && _imageAnnotation.YAxis != null) _imageAnnotation.SetBindings();
        }       

        /// <summary>
        /// Gets or Sets the Y2 Property for the imageAnnotation
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
            DependencyProperty.Register("Y2", typeof(object), typeof(ImageAnnotation), new PropertyMetadata(null, OnHeightWidthChanged));

        internal double ImageWidth
        {
            get;
            set;
        }
        internal double ImageHeight
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or Sets the X2 Property for the imageAnnotation
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
            DependencyProperty.Register("X2", typeof(object), typeof(ImageAnnotation), new PropertyMetadata(null, OnHeightWidthChanged));


        private static void OnHeightWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var _imageAnnotation = sender as ImageAnnotation;
            if (_imageAnnotation != null && _imageAnnotation.XAxis != null && _imageAnnotation.YAxis != null) _imageAnnotation.HeightWidthChanged();
        }


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
            DependencyProperty.Register("HorizontalTextAlignment", typeof(HorizontalAlignment), typeof(ImageAnnotation), new PropertyMetadata(HorizontalAlignment.Center));


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
            DependencyProperty.Register("VerticalTextAlignment", typeof(VerticalAlignment), typeof(ImageAnnotation), new PropertyMetadata(VerticalAlignment.Bottom));

        internal void HeightWidthChanged()
        {
            switch (CoordinateUnit)
            {
                case CoordinateUnit.Axis:
                    this.ImageHeight = ConvertData(this.Y2, this.YAxis);
                    this.ImageWidth = ConvertData(this.X2, this.XAxis);
                    break;               
            }
            UpdateAnnotation();
        }

        internal override UIElement CreateAnnotation()
        {
            if (AnnotationElement != null && AnnotationElement.Children.Count == 0)
            {
                _image = new Image();
                SetBindings();
                if (ShowToolTip)
                    _image.Tag = this;
                AnnotationElement.Children.Add(_image);
                TextElementCanvas.Children.Add(TextElement);
                AnnotationElement.Children.Add(TextElementCanvas);
            }
            return AnnotationElement;

        }
        protected override void SetBindings()
        {
            base.SetBindings();
#if NETFX_CORE
            BitmapImage imageSource = new BitmapImage {UriSource = new Uri(ImageSource, UriKind.Absolute)};
            _image.Source = imageSource;
#else            
            _image.Source = ImageSource;
#endif
            _image.Stretch = Stretch.Fill;                        
               
        }
        public override void UpdateAnnotation()
        {
            if (_image != null && X1 != null && X2 != null && Y1 != null && Y2 != null)
            {
                Point textPosition = new Point();
                Size desiredSize;
                Rect heightAndWidthRect;
                RotateTransform rotate = new RotateTransform { Angle = this.Angle };
                Point positionedPoint = new Point(0, 0);
                switch (CoordinateUnit)
                {
                    case CoordinateUnit.Axis:
                        base.UpdateAnnotation();
                        if (XAxis != null && YAxis != null)
                        {
                            double xStart = XAxis.VisibleRange.Start;
                            double xEnd = XAxis.VisibleRange.End;
                            double yStart = YAxis.VisibleRange.Start;
                            double yEnd = YAxis.VisibleRange.End;
                            ImageHeight = ConvertData(Y2, YAxis);
                            ImageWidth = ConvertData(X2, XAxis);  
                            
                            Point point = (XAxis.Orientation == Orientation.Horizontal) ? new Point(this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1),
                                                    this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1)) : new Point(this.Chart.ValueToPointRelativeToAnnotation(YAxis, y1),
                                                    this.Chart.ValueToPointRelativeToAnnotation(XAxis, x1));
                            Point point2 = (XAxis.Orientation == Orientation.Horizontal) ? new Point(Chart.ValueToPointRelativeToAnnotation(XAxis, ImageWidth),
                                                     this.Chart.ValueToPointRelativeToAnnotation(YAxis, ImageHeight)) : new Point(this.Chart.ValueToPointRelativeToAnnotation(YAxis, ImageHeight),
                                                     Chart.ValueToPointRelativeToAnnotation(XAxis, ImageWidth));                            
                            point.Y = (double.IsNaN(point.Y)) ? 0 : point.Y;
                            point.X = (double.IsNaN(point.X)) ? 0 : point.X;
                            point2.Y = (double.IsNaN(point2.Y)) ? 0 : point2.Y;
                            point2.X = (double.IsNaN(point2.X)) ? 0 : point2.X;
                            heightAndWidthRect = new Rect(point, point2);
                            _image.Height = heightAndWidthRect.Height;
                            _image.Width = heightAndWidthRect.Width;
                            AnnotationElement.Height = heightAndWidthRect.Height;
                            AnnotationElement.Width = heightAndWidthRect.Width;        
                            Point ensurePoint = this.EnsurePoint(point, point2);
                            desiredSize = new Size(heightAndWidthRect.Width, heightAndWidthRect.Height);
                            positionedPoint = GetElementPosition(
                                new Size(heightAndWidthRect.Width, heightAndWidthRect.Height), ensurePoint);

                            TextElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));                                               
                            textPosition = GetTextPosition(desiredSize, new Point(0, 0), new Size(TextElement.DesiredSize.Width,TextElement.DesiredSize.Height));
                            Canvas.SetLeft(AnnotationElement, positionedPoint.X);
                            Canvas.SetTop(AnnotationElement, positionedPoint.Y);
                            Canvas.SetLeft(TextElement, textPosition.X);
                            Canvas.SetTop(TextElement, textPosition.Y);
                            Point centerPoint = new Point(positionedPoint.X + (AnnotationElement.Width / 2),
                                                          positionedPoint.Y + (AnnotationElement.Height / 2));
                            Rect rotated = this.RotateElement(this.Angle, AnnotationElement);
                            AnnotationElement.RenderTransformOrigin = new Point(0.5, 0.5);
                            AnnotationElement.RenderTransform = rotate;
                            if (this.Angle > 0)
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
                        _image.Height = heightAndWidthRect.Height;
                        _image.Width = heightAndWidthRect.Width;
                        AnnotationElement.Height = heightAndWidthRect.Height;
                        AnnotationElement.Width = heightAndWidthRect.Width;
                        positionedPoint = GetElementPosition(_image, elementPoint1);
                        desiredSize = new Size(_image.Width, _image.Height);
                        TextElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));         
                        textPosition = GetTextPosition(desiredSize, new Point(0, 0),
                                                       new Size(TextElement.DesiredSize.Width, TextElement.DesiredSize.Height));
                        Canvas.SetLeft(AnnotationElement, positionedPoint.X);
                        Canvas.SetTop(AnnotationElement, positionedPoint.Y);
                        Canvas.SetLeft(TextElement, textPosition.X);
                        Canvas.SetTop(TextElement, textPosition.Y);

                        AnnotationElement.RenderTransformOrigin = new Point(0.5, 0.5);
                        AnnotationElement.RenderTransform = rotate;
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
                default:
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
                default:
                    break;
            }
            return point;
        }

        protected Point GetTextPosition(Size desiredSize, Point originalPosition, Size textSize)
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
                default:
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
            ImageAnnotation imageAnnotation = new ImageAnnotation();
            imageAnnotation.Angle = this.Angle;
            imageAnnotation.HorizontalTextAlignment = this.HorizontalTextAlignment;
            imageAnnotation.ImageSource = this.ImageSource;
            imageAnnotation.VerticalTextAlignment = this.VerticalTextAlignment;
            imageAnnotation.X2 = this.X2;
            imageAnnotation.Y2 = this.Y2;
            return base.CloneAnnotation(imageAnnotation);
        }
    }
}
