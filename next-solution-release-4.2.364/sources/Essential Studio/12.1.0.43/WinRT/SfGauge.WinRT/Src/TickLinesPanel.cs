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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Gauges
{
    /// <summary>
    ///  TickLinesPanel is used to arrange the TickLines.
    /// </summary>
    public class TickLinesPanel : Panel
    {
        double maxWidth = 0.0;
        double maxHeight = 0.0;

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

        private void SetTickPanelBindings()
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
                                              {Path = new PropertyPath("CircularTicksAngularSpace"), Source = circularScale};
            SetBinding(AngularSpaceProperty, angularSpaceBinding);

            
        }

        /// <summary>
        /// Gets or sets SweepDirection of the TickLinesPanel which decides the rendering direction of the ticks.
        /// </summary>
        /// <value>
        /// SweepDirection
        /// </value>
        public SweepDirection SweepDirection
        {
            get { return (SweepDirection)GetValue(SweepDirectionProperty); }
            set { SetValue(SweepDirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SweepDirection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SweepDirectionProperty =
            DependencyProperty.Register("SweepDirection", typeof(SweepDirection), typeof(TickLinesPanel), new PropertyMetadata(SweepDirection.Clockwise));



        /// <summary>
        /// Gets or sets the SweepAngle of the TickLinesPanel which help to position the ticks.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        public double SweepAngle
        {
            get { return (double)GetValue(SweepAngleProperty); }
            set { SetValue(SweepAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SweepAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SweepAngleProperty =
            DependencyProperty.Register("SweepAngle", typeof(double), typeof(TickLinesPanel), new PropertyMetadata(360d));


        /// <summary>
        /// Gets or sets the StartAngle of the TickLinesPanel which help to position the ticks.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartAngle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(TickLinesPanel), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the AngularSpace between two children of the TickLinesPanel.
        /// </summary>
        /// <value>
        /// double
        /// </value>
        public double AngularSpace
        {
            get { return (double)GetValue(AngularSpaceProperty); }
            set { SetValue(AngularSpaceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AngularSpace.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AngularSpaceProperty =
            DependencyProperty.Register("AngularSpace", typeof(double), typeof(TickLinesPanel), new PropertyMetadata(0d));

        
        protected override Size MeasureOverride(Size availableSize)
        {
            int childCount = SweepAngle == 360 ? Children.Count : Children.Count - 1;
            double angle = StartAngle;
            double angularSpace = AngularSpace;

            foreach (UIElement element in Children)
            {
                element.RenderTransformOrigin = new Point(0.5, 0.5);
                element.RenderTransform = new RotateTransform() { Angle = angle};
                element.Measure(availableSize);
                maxWidth = Math.Max(maxWidth, element.DesiredSize.Width);
                maxHeight = Math.Max(maxHeight, element.DesiredSize.Height);

                if (this.SweepDirection == SweepDirection.Clockwise)
                    angle += angularSpace;
                else
                    angle -= angularSpace;
            }
            SetTickPanelBindings();
            return (availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int childCount = SweepAngle == 360 ? Children.Count : Children.Count - 1;
            double angle = StartAngle * (Math.PI / 180);
            double angularSpace = AngularSpace * (Math.PI / 180);

            double radiusx = (finalSize.Width / 2) - ((maxWidth / 2));
            double radiusy = (finalSize.Height / 2) - ((maxHeight / 2));

            foreach (UIElement element in Children)
            {
                Point point = new Point(Math.Cos(angle) * radiusx, Math.Sin(angle) * radiusy);
                Point actualChildPoint = new Point(finalSize.Width / 2 + point.X - element.DesiredSize.Width / 2, finalSize.Height / 2 + point.Y - element.DesiredSize.Height / 2);

                element.Arrange(new Rect(actualChildPoint.X, actualChildPoint.Y, element.DesiredSize.Width, element.DesiredSize.Height));

                if (this.SweepDirection == SweepDirection.Clockwise)
                    angle += angularSpace;
                else
                    angle -= angularSpace;
            }

            return finalSize;
        }

    }
}
