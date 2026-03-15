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

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
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

        #region Dependency Properties

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

        #if WINRT

        internal double HalfLength
        {
            get { return (double)GetValue(HalfLengthProperty); }
            set { SetValue(HalfLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HalfLength.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HalfLengthProperty =
            DependencyProperty.Register("HalfLength", typeof(double), typeof(TickLinesPanel), new PropertyMetadata(5d));



        internal double Length
        {
            get { return (double)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Length.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register("Length", typeof(double), typeof(TickLinesPanel), new PropertyMetadata(10d));




        internal Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(TickLinesPanel), new PropertyMetadata(new SolidColorBrush(Colors.White)));



        internal double TickStrokeThickness
        {
            get { return (double)GetValue(TickStrokeThicknessProperty); }
            set { SetValue(TickStrokeThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickStrokeThickness.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TickStrokeThicknessProperty =
            DependencyProperty.Register("TickStrokeThickness", typeof(double), typeof(TickLinesPanel), new PropertyMetadata(1d));


        internal CircularScaleTickCollection Ticks
        {
            get { return (CircularScaleTickCollection)GetValue(TicksProperty); }
            set { SetValue(TicksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Ticks.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TicksProperty =
            DependencyProperty.Register("Ticks", typeof(CircularScaleTickCollection), typeof(TickLinesPanel), new PropertyMetadata(null));



        internal object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(TickLinesPanel), new PropertyMetadata(null));

#endif


        #endregion

        protected override Size MeasureOverride(Size availableSize)
        {
#if WINRT
            this.Children.Clear();
   SetTickPanelBindings();
            double angle = StartAngle;
            double angularSpace = AngularSpace;
            foreach (var item in (IEnumerable<object>)ItemsSource)
            {
                CircularTicks Child1 = new CircularTicks() { DataContext=item, Tick =(CircularScaleTick)item, HalfLength = this.HalfLength, Stroke= this.Stroke, TickStrokeThickness = this.TickStrokeThickness, TicksMargin = this.Margin};
                UIElement element = Child1;
                element.RenderTransformOrigin = new Point(0.5, 0.5);
                element.RenderTransform = new RotateTransform() { Angle = angle };
                this.Children.Add(element);
                if (this.SweepDirection == SweepDirection.Clockwise)
                    angle += angularSpace;
                else
                    angle -= angularSpace;
            }
            if (Children.Count >= 2)
            {
                Children[0].Measure(availableSize);
                Children[1].Measure(availableSize);
                maxWidth = Math.Max(maxWidth, Children[0].DesiredSize.Width);
                maxWidth = Math.Max(maxWidth, Children[1].DesiredSize.Width);
                maxHeight = Math.Max(maxHeight, Children[0].DesiredSize.Height);
                maxHeight = Math.Max(maxHeight, Children[1].DesiredSize.Height);

            } 
#else

 int childCount = SweepAngle == 360 ? Children.Count : Children.Count - 1;
            double angle = StartAngle;
            double angularSpace = AngularSpace;

            foreach (UIElement element in Children)
            {
                element.RenderTransformOrigin = new Point(0.5, 0.5);
                element.RenderTransform = new RotateTransform() { Angle = angle};                
                if (this.SweepDirection == SweepDirection.Clockwise)
                    angle += angularSpace;
                else
                    angle -= angularSpace;
            }
            if (Children.Count >= 2)
            {
                Children[0].Measure(availableSize);
                Children[1].Measure(availableSize);
                maxWidth = Math.Max(maxWidth, Children[0].DesiredSize.Width);
                maxWidth = Math.Max(maxWidth, Children[1].DesiredSize.Width);
                maxHeight = Math.Max(maxHeight, Children[0].DesiredSize.Height);
                maxHeight = Math.Max(maxHeight, Children[1].DesiredSize.Height);
            }
            SetTickPanelBindings();

#endif
          
            return (availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double angle = StartAngle * (Math.PI / 180);
            double angularSpace = AngularSpace * (Math.PI / 180);

            double radiusx = (finalSize.Width / 2) - ((maxWidth / 2));
            double radiusy = (finalSize.Height / 2) - ((maxHeight / 2));
            Size smalltick = new Size(0, 0);
            Size largetick = new Size(0, 0);

            if (Children.Count > 1)
            {
                smalltick = Children[1].DesiredSize;
                largetick = Children[0].DesiredSize;
            }
            
           
            Size commonsize = largetick;

            foreach (UIElement element in Children)
            {
#if WINRT
                if (element is CircularTicks)
                {
                    if (((element as CircularTicks).Tick as CircularScaleTick).Ticktype == TickType.Major)
                    {
                        commonsize = largetick;
                    }
                    else
                    {
                        commonsize = smalltick;
                    }
                }
#else

       if (element is ContentPresenter)
                {
                    if (((element as ContentPresenter).Content as CircularScaleTick).Ticktype == TickType.Major)
                    {
                        commonsize = largetick;
                    }
                    else
                    {
                        commonsize = smalltick;
                    }
                }
#endif
                    Point point = new Point(Math.Cos(angle) * radiusx, Math.Sin(angle) * radiusy);
                    Point actualChildPoint = new Point(finalSize.Width / 2 + point.X - commonsize.Width / 2, finalSize.Height / 2 + point.Y - commonsize.Height / 2);

                    element.Arrange(new Rect(actualChildPoint.X, actualChildPoint.Y, commonsize.Width, commonsize.Height));

                    if (this.SweepDirection == SweepDirection.Clockwise)
                        angle += angularSpace;
                    else
                        angle -= angularSpace;
                
            }

            return finalSize;
        }

    }

    #if WINRT
    public class CircularTicks : Control
    {
        internal CircularTicks()
       {
           this.DefaultStyleKey = typeof(CircularTicks);
       }

        internal Thickness TicksMargin
        {
            get { return (Thickness)GetValue(TicksMarginProperty); }
            set { SetValue(TicksMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TicksMargin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TicksMarginProperty =
            DependencyProperty.Register("TicksMargin", typeof(Thickness), typeof(CircularTicks), new PropertyMetadata(new Thickness(0)));


        internal CircularScaleTick Tick
       {
           get { return (CircularScaleTick)GetValue(TickProperty); }
           set { SetValue(TickProperty, value); }
       }

       // Using a DependencyProperty as the backing store for Tick.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TickProperty =
           DependencyProperty.Register("Tick", typeof(CircularScaleTick), typeof(CircularTicks), new PropertyMetadata(null));


        internal double HalfLength
       {
           get { return (double)GetValue(HalfLengthProperty); }
           set { SetValue(HalfLengthProperty, value); }
       }

       // Using a DependencyProperty as the backing store for HalfLength.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HalfLengthProperty =
           DependencyProperty.Register("HalfLength", typeof(double), typeof(CircularTicks), new PropertyMetadata(5d));


        internal double Length
       {
           get { return (double)GetValue(LengthProperty); }
           set { SetValue(LengthProperty, value); }
       }

       // Using a DependencyProperty as the backing store for Length.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LengthProperty =
           DependencyProperty.Register("Length", typeof(double), typeof(CircularTicks), new PropertyMetadata(10d));


        internal Brush Stroke
       {
           get { return (Brush)GetValue(StrokeProperty); }
           set { SetValue(StrokeProperty, value); }
       }

       // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty StrokeProperty =
           DependencyProperty.Register("Stroke", typeof(Brush), typeof(CircularTicks), new PropertyMetadata(new SolidColorBrush(Colors.White)));



        internal double TickStrokeThickness
       {
           get { return (double)GetValue(TickStrokeThicknessProperty); }
           set { SetValue(TickStrokeThicknessProperty, value); }
       }

       // Using a DependencyProperty as the backing store for TickStrokeThickness.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TickStrokeThicknessProperty =
           DependencyProperty.Register("TickStrokeThickness", typeof(double), typeof(CircularTicks), new PropertyMetadata(1d));


       internal CircularScaleTickCollection Ticks
       {
           get { return (CircularScaleTickCollection)GetValue(TicksProperty); }
           set { SetValue(TicksProperty, value); }
       }

       // Using a DependencyProperty as the backing store for Ticks.  This enables animation, styling, binding, etc...
       internal static readonly DependencyProperty TicksProperty =
           DependencyProperty.Register("Ticks", typeof(CircularScaleTickCollection), typeof(CircularTicks), new PropertyMetadata(null));
    }
  #endif
}
