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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    [CLSCompliant(false)]
    public class CarouselPanel : Canvas
    {

        private const double C_DEFAULT_ROTATION_SPEED = 200;
        private const double C_MINIMUM_ROTATION_SPEED = 1;
        private const double C_MAXIMUM_ROTATION_SPEED = 1000;
        private const double C_DEFAULT_FADE = 0.5;
        private const double C_MINIMUM_FADE = 0;
        private const double C_MAXIMUM_FADE = 1;
        private const double C_DEFAULT_SCALE = 0.5;
        private const double C_MINIMUM_SCALE = 0;
        private const double C_MAXIMUM_SCALE = 1;

        internal int currentIndex = 0;
        /// <summary>
        /// 
        /// </summary>
        protected DispatcherTimer _timer = new DispatcherTimer();
        internal double _rotationToGo = 0;
        private DateTime _previousTime;
        private DateTime _currentTime;
        /// <summary>
        /// 
        /// </summary>
        protected double X_SCALE = 0;
        /// <summary>
        /// 
        /// </summary>
        protected double Y_SCALE = 0;
        /// <summary>
        /// 
        /// </summary>
        internal double _currentRotation = 0;
        /// <summary>
        /// 
        /// </summary>
        private const double INTERNAL_SCALE_COEFFICIENT = 0.6;
        /// <summary>
        /// 
        /// </summary>
        protected double _targetRotation = 0;
        /// <summary>
        /// 
        /// </summary>
        private double rotationDiff = 0.0;

       /// <summary>
       /// 
       /// </summary>
        public CarouselPanel()
        {
            _timer.Tick += new EventHandler(TimerTick);
            _timer.Interval = TimeSpan.FromMilliseconds(10);
        }
        /// <summary>
        /// 
        /// </summary>
        public double ScaleFraction
        {
            get { return (double)GetValue(ScaleFractionProperty); }
            set { SetValue(ScaleFractionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleFraction.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ScaleFractionProperty =
            DependencyProperty.Register("ScaleFraction", typeof(double), typeof(CarouselPanel), new FrameworkPropertyMetadata(0.0,
                FrameworkPropertyMetadataOptions.AffectsArrange,
                new PropertyChangedCallback(OnScaleFractionChanged),
                new CoerceValueCallback(CoerceScaleFractions)));

        /// <summary>
        /// 
        /// </summary>
        public double OpacityFraction
        {
            get { return (double)GetValue(OpacityFractionProperty); }
            set { SetValue(OpacityFractionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpacityFraction.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OpacityFractionProperty =
            DependencyProperty.Register("OpacityFraction", typeof(double), typeof(CarouselPanel), new FrameworkPropertyMetadata(0.0,
                FrameworkPropertyMetadataOptions.AffectsArrange,
                new PropertyChangedCallback(OnOpacityFractionChanged),
                new CoerceValueCallback(CoerceOpacityFractions)));

        /// <summary>
        /// 
        /// </summary>
        public double RotationSpeed
        {
            get { return (double)GetValue(RotationSpeedProperty); }
            set { SetValue(RotationSpeedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RotationSpeed.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RotationSpeedProperty =
            DependencyProperty.Register("RotationSpeed", typeof(double), typeof(CarouselPanel), new UIPropertyMetadata(200.0, new PropertyChangedCallback(OnRotationSpeedChanged),
                new CoerceValueCallback(CoerceRotateSpeed)));

        /// <summary>
        /// 
        /// </summary>
        public double RadiusX
        {
            get { return (double)GetValue(RadiusXProperty); }
            set { SetValue(RadiusXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RadiusX.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RadiusXProperty =
            DependencyProperty.Register("RadiusX", typeof(double), typeof(CarouselPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// 
        /// </summary>
        public double RadiusY
        {
            get { return (double)GetValue(RadiusYProperty); }
            set { SetValue(RadiusYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RadiusY.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RadiusYProperty =
            DependencyProperty.Register("RadiusY", typeof(double), typeof(CarouselPanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange));


        private static object CoerceScaleFractions(DependencyObject sender, object obj)
        {
            return Math.Min(Math.Max((double)obj, C_MINIMUM_SCALE), C_MAXIMUM_SCALE);
        }

        private static void OnScaleFractionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }

        private static object CoerceOpacityFractions(DependencyObject sender, object obj)
        {
            return Math.Min(Math.Max((double)obj, C_MINIMUM_FADE), C_MAXIMUM_FADE);
        }

        private static void OnOpacityFractionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }

        private static object CoerceRotateSpeed(DependencyObject sender, object obj)
        {
            return Math.Min(Math.Max((double)obj, C_MINIMUM_ROTATION_SPEED), C_MAXIMUM_ROTATION_SPEED);
        }

        private static void OnRotationSpeedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {

        }

        private double RotationAmount
        {
            get
            {
                return (_currentTime - _previousTime).TotalSeconds * RotationSpeed;
            }
        }
    
       /// <summary>
       /// 
       /// </summary>
       /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            _previousTime = DateTime.Now;
            this.Loaded += new RoutedEventHandler(CarouselPanel_Loaded);
        }

        void CarouselPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (Children.Count > 0)
            {
                if (currentIndex >= 0)
                    MoveToItem(Children[currentIndex] as FrameworkElement);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                FrameworkElement element = Children[i] as FrameworkElement;
                element.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
       protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
                return finalSize;

            double angle = (90 + _currentRotation) * (Math.PI / 180);
            double itemspace = (360 / Children.Count);
            double angularspace = itemspace * (Math.PI / 180);

            double radiusx = RadiusX;
            double radiusy = RadiusY;

            for (int index = 0; index < Children.Count; index++)
            {
                FrameworkElement child = Children[index] as FrameworkElement;

                double elementWidthCenter = child.DesiredSize.Width / 2;
                double elementHeightCenter = child.DesiredSize.Height / 2;

                double degrees = 360 * ((double)index / (double)Children.Count) + _currentRotation;

                double left = radiusx * Math.Cos(angle);
                double top = radiusy * Math.Sin(angle);
                Point childPoint = new Point(left, top);
                Point actualChildPoint = new Point(finalSize.Width / 2 + childPoint.X - child.DesiredSize.Width / 2, finalSize.Height / 2 + childPoint.Y - child.DesiredSize.Height / 2);


                child.Arrange(new Rect(actualChildPoint.X, actualChildPoint.Y, child.DesiredSize.Width, child.DesiredSize.Height));
                ScaleTransform scale = child.RenderTransform as ScaleTransform;

                if (scale == null)
                {
                    scale = new ScaleTransform();
                    child.RenderTransform = scale;
                }

                scale.CenterX = elementWidthCenter;
                scale.CenterY = elementHeightCenter;
                scale.ScaleX = scale.ScaleY = GetScaledSize(degrees);
                Canvas.SetZIndex(child, GetZValue(degrees));

                SetOpacity(child, degrees);
                angle += angularspace;
            }
            return finalSize;
        }


        private double RadsConvert(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="degrees"></param>
        protected void SetOpacity(FrameworkElement element, double degrees)
        {
            element.Opacity = (1.0 - OpacityFraction) + OpacityFraction * GetCoefficient(degrees);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        protected int GetZValue(double degrees)
        {
            return (int)(360 * GetCoefficient(degrees));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        protected double GetScaledSize(double degrees)
        {
            return (1.0 - ScaleFraction) + ScaleFraction * GetCoefficient(degrees);
        }

        private double GetCoefficient(double degrees)
        {
            return 1.0 - Math.Cos(RadsConvert(degrees)) / 2 - 0.5;
        }

        private void MoveToItem(FrameworkElement element)
        {

            int targetIndex = Children.IndexOf(element);

            double degreesToRotate = GetFrontElementSpace(_currentRotation, targetIndex, Children.Count);
            _targetRotation = SetDegrees(_currentRotation - degreesToRotate);

            Rotate(degreesToRotate);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public void SelectElement(FrameworkElement element)
        {
            if (element != null)
            {
                _previousTime = DateTime.Now;
                double angularspace = (360 / Children.Count);
                rotationDiff = angularspace * (currentIndex - Children.IndexOf(element));
                MoveToItem(element);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public int GetSelecteItem(FrameworkElement element)
        {
            int index = 0;
            index = Children.IndexOf(element);
            return index;
        }
        internal double SetDegrees(double rawDegrees)
        {
            if (rawDegrees > 360)
                return rawDegrees - 360;

            if (rawDegrees < 0)
                return rawDegrees + 360;

            return rawDegrees;
        }


        internal void Rotate(double numberOfDegrees)
        {
            _rotationToGo = numberOfDegrees;
            if (!_timer.IsEnabled)
            {
                _timer.Start();
            }
        }


        internal static double GetFrontElementSpace(double currentRotation, int targetIndex, int totalNumberOfElements)
        {
            double rawDegrees = -(180.0 - (currentRotation + 360.0 * ((double)targetIndex / (double)totalNumberOfElements)));

            if (rawDegrees > 180)
                return -(360 - rawDegrees);

            return rawDegrees;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void TimerTick(object sender, EventArgs e)
        {
            _currentTime = DateTime.Now;

            if ((_rotationToGo < RotationAmount) && (_rotationToGo > -RotationAmount))
            {
                _rotationToGo = 0;

                if (_currentRotation != _targetRotation)
                {
                    _currentRotation = _targetRotation;
                }
                else
                {
                    _timer.Stop();
                    return;
                }
            }
            else if (_rotationToGo < 0)
            {
                _rotationToGo += RotationAmount;
                _currentRotation = SetDegrees(_currentRotation + RotationAmount);
            }
            else
            {
                _rotationToGo -= RotationAmount;
                _currentRotation = SetDegrees(_currentRotation - RotationAmount);
            }

            this.InvalidateArrange();

            _previousTime = _currentTime;
        }
    }
}
