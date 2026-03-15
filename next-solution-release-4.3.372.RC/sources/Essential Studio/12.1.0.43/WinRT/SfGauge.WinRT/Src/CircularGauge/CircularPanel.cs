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

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using System.Threading.Tasks;
using Windows.UI;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
#endif

namespace Syncfusion.UI.Xaml.Gauges
{
    /// <summary>
    ///  CircularPanel is used to arrange the CircularScale elements.
    /// </summary>
    public class CircularPanel : Panel
    {
        double maxWidth = 0.0;
        double maxHeight = 0.0;
        int sizeCheck = 0;
        internal static T FindAnchestor<T>(DependencyObject current)
                 where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private void SetLabelPanelBindings()
        {
            CircularScale circularScale = FindAnchestor<CircularScale>(this);
            Binding sweepAngleBinding = new Binding() {Path = new PropertyPath("SweepAngle"), Source = circularScale};
            SetBinding(SweepAngleProperty, sweepAngleBinding);
            Binding startAngleBinding = new Binding() {Path = new PropertyPath("StartAngle"), Source = circularScale};
            SetBinding(StartAngleProperty, startAngleBinding);
            Binding angularOrientationBinding = new Binding()
                                                    {Path = new PropertyPath("SweepDirection"), Source = circularScale};
            SetBinding(SweepDirectionProperty, angularOrientationBinding);
            Binding angularSpaceBinding = new Binding()
                                              {Path = new PropertyPath("CircularLabelsAngularSpace"), Source = circularScale};
            SetBinding(AngularSpaceProperty, angularSpaceBinding);
            Binding maxWidthBinding = new Binding()
                                          {
                                              Path = new PropertyPath("ScaleMaxWidth"),
                                              Source = circularScale,
                                              Mode = BindingMode.TwoWay
                                          };
            SetBinding(ScaleMaxWidthProperty, maxWidthBinding);
        }



        /// <summary>
        /// Gets or sets ScaleMaxWidth of the CircularPanel which help to position the SfCircularGauge elements.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        [ClassReference(IsReviewed = false)]
        public double ScaleMaxWidth
        {
            get { return (double)GetValue(ScaleMaxWidthProperty); }
            set { SetValue(ScaleMaxWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleMaxWidthProperty =
            DependencyProperty.Register("ScaleMaxWidth", typeof(double), typeof(CircularPanel), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets SweepDirection of the CircularPanel which decides rendering direction of the CircularScale elements.
        /// </summary>
        /// <value>
        /// SweepDirection
        /// </value>
        [ClassReference(IsReviewed = false)]
        public SweepDirection SweepDirection
        {
            get { return (SweepDirection)GetValue(SweepDirectionProperty); }
            set { SetValue(SweepDirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SweepDirection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SweepDirectionProperty =
            DependencyProperty.Register("SweepDirection", typeof(SweepDirection), typeof(CircularPanel), new PropertyMetadata(SweepDirection.Clockwise));



        /// <summary>
        /// Gets or sets SweepAngle of the CircularPanel which decides the shape of the SfCircularGauge.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        [ClassReference(IsReviewed = false)]
        public double SweepAngle
        {
            get { return (double)GetValue(SweepAngleProperty); }
            set { SetValue(SweepAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SweepAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SweepAngleProperty =
            DependencyProperty.Register("SweepAngle", typeof(double), typeof(CircularPanel), new PropertyMetadata(360d));



        /// <summary>
        /// Gets or sets the StartAngle of the CircularPanel which decides the shape of the SfCircularGauge.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        [ClassReference(IsReviewed = false)]
        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(CircularPanel), new PropertyMetadata(0d));



        /// <summary>
        /// Gets or sets the AngularSpace between two children of the CircularPanel.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        [ClassReference(IsReviewed = false)]
        public double AngularSpace
        {
            get { return (double)GetValue(AngularSpaceProperty); }
            set { SetValue(AngularSpaceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AngularSpace.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AngularSpaceProperty =
            DependencyProperty.Register("AngularSpace", typeof(double), typeof(CircularPanel), new PropertyMetadata(0d));


#if WINRT

        internal object ItemsSource
        {
            get { return (object)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(CircularPanel), new PropertyMetadata(null));

        internal object Text
        {
            get { return (object)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(object), typeof(CircularPanel), new PropertyMetadata(null));



        internal Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Foreground.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(CircularPanel), new PropertyMetadata(new SolidColorBrush(Colors.White)));


        internal CircularScaleLabelCollection Labels
        {
            get { return (CircularScaleLabelCollection)GetValue(LabelsProperty); }
            set { SetValue(LabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Labels.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LabelsProperty =
            DependencyProperty.Register("Labels", typeof(CircularScaleLabelCollection), typeof(CircularPanel), new PropertyMetadata(null));

      #endif  

        protected override Size MeasureOverride(Size availableSize)
        {
#if WINRT
            this.Children.Clear();
            if (Labels != null)
            {
                foreach (var item in Labels)
                {
                    MyLabel Child1 = new MyLabel() { DataContext = item };

                    this.Children.Add(Child1);
                }
            }
     #endif       
            foreach (UIElement element in Children)
            {
                element.Measure(availableSize);
                maxWidth = Math.Max(maxWidth, element.DesiredSize.Width);
                maxHeight = Math.Max(maxHeight, element.DesiredSize.Height);
            }
            Binding maxWidthBinding = new Binding() { Path = new PropertyPath("ScaleMaxWidth"), Source = FindAnchestor<CircularScale>(this), Mode = BindingMode.TwoWay };
            SetBinding(ScaleMaxWidthProperty, maxWidthBinding);
            this.ScaleMaxWidth = maxWidth;
            SetLabelPanelBindings();
            return (availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int childCount = Children.Count - 1;
            int intersectCount = 0;
            double angle = StartAngle * (Math.PI /180);
            double angularSpace = AngularSpace * (Math.PI / 180);
            Rect checkIntersect = new Rect();
            foreach (UIElement element in Children)
            {
                double radiusx = (finalSize.Width / 2) - ((element.DesiredSize.Width / 2));
                double radiusy = (finalSize.Height / 2) - ((element.DesiredSize.Height / 2));
                Point point = new Point(Math.Cos(angle) * radiusx, Math.Sin(angle) * radiusy);
                Point actualChildPoint = new Point(finalSize.Width / 2 + point.X - element.DesiredSize.Width / 2, finalSize.Height / 2 + point.Y - element.DesiredSize.Height / 2);
                Rect eleRect = new Rect(actualChildPoint.X, actualChildPoint.Y, element.DesiredSize.Width, element.DesiredSize.Height);
                Rect checkRect = eleRect;
                checkRect.Intersect(checkIntersect);
                if (!checkRect.IsEmpty)
                {
                    intersectCount = intersectCount + 1;
                }
                element.Arrange(eleRect);
                checkIntersect = eleRect;
                if(this.SweepDirection == SweepDirection.Clockwise) 
                    angle += angularSpace;
                else
                    angle -= angularSpace;
            }
            CircularScale circularScale = FindAnchestor<CircularScale>(this);
            if (circularScale.LabelAutoSizeChange)
            {
                if (intersectCount > 2 && (sizeCheck == 0 || sizeCheck == 1) && circularScale.LabelFontSize > 3)
                {
                    circularScale.LabelFontSize = circularScale.LabelFontSize - 1;
                    circularScale.ResetScale();
                    sizeCheck = 1;
                }
                else if (intersectCount < 1 && (sizeCheck == 0 || sizeCheck == -1))
                {
                    circularScale.LabelFontSize = circularScale.LabelFontSize + 1;
                    circularScale.ResetScale();
                    sizeCheck = -1;
                }
                else
                {
                    sizeCheck = 0;
                }
            }
            return finalSize;
        }       

    }
#if WINRT
    public class MyLabel : Control
    {
        public MyLabel()
        {
            this.DefaultStyleKey = typeof(MyLabel);
        }
    }
#endif
}
