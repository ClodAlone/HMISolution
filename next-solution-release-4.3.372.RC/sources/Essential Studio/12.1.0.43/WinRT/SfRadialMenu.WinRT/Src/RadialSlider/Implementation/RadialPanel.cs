#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.ComponentModel;

namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.ComponentModel;
namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.ComponentModel;
namespace Syncfusion.Windows.Controls.Navigation
#else
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// Represents a panel that allows the user to select an item in
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class RadialPanel : Panel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialPanel"/> class.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/>
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Navigation">Syncfusion.UI.Xaml.Controls.Navigation
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public RadialPanel()
        {           
            Loaded += RadialPanel_Loaded;
        }

        #endregion

        RadialList _parentItemsControl;
        
        void RadialPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _parentItemsControl = ItemsControl.GetItemsOwner(this) as RadialList;
            if (_parentItemsControl != null)
            {
                Binding radiusBinding = new Binding();
                radiusBinding.Source = _parentItemsControl;
                radiusBinding.Path = new PropertyPath("Radius");
                SetBinding(RadialPanel.RadiusProperty, radiusBinding);

                SfRadialSlider sfRadialSlider = _parentItemsControl.ListHost as SfRadialSlider;
                if (sfRadialSlider != null)
                {
                    Binding startAngleBinding = new Binding();
                    startAngleBinding.Source = sfRadialSlider;
                    startAngleBinding.Path = new PropertyPath("StartAngle");
                    SetBinding(RadialPanel.StartAngleProperty, startAngleBinding);

                    Binding endAngleBinding = new Binding();
                    endAngleBinding.Source = sfRadialSlider;
                    endAngleBinding.Path = new PropertyPath("EndAngle");
                    SetBinding(RadialPanel.EndAngleProperty, endAngleBinding);
                  
                }

            }          

        }

        #region Dependency Properties

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Getsor sets the start angle of the <see 
        /// cref="Syncfusion.WP.Controls.Navigation.RadialPanel"/>
        /// </summary>
#else
        /// <summary>
        /// Getsor sets the start angle of the <see 
        /// cref="Syncfusion.UI.Xaml.Controls.Navigation.RadialPanel"/>
        /// </summary>
#endif
        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TickFrequency.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(RadialPanel), new PropertyMetadata(0d));

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Getsor sets the end angle of the <see 
        /// cref="Syncfusion.WP.Controls.Navigation.RadialPanel"/>
        /// </summary>
#else
        /// <summary>
        /// Getsor sets the end angle of the <see 
        /// cref="Syncfusion.UI.Xaml.Controls.Navigation.RadialPanel"/>
        /// </summary>
#endif
        public double EndAngle
        {
            get { return (double)GetValue(EndAngleProperty); }
            set { SetValue(EndAngleProperty, value); }
        }
        
        /// <summary>
        /// Using a DependencyProperty as the backing store for TickFrequency.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register("EndAngle", typeof(double), typeof(RadialPanel), new PropertyMetadata(360d));

#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Getsor sets the radius of the <see 
        /// cref="Syncfusion.WP.Controls.Navigation.RadialPanel"/>
        /// </summary>
#else
        /// <summary>
        /// Getsor sets the radius of the <see 
        /// cref="Syncfusion.UI.Xaml.Controls.Navigation.RadialPanel"/>
        /// </summary>
#endif
        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Radius.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(RadialPanel), new PropertyMetadata(0d,OnRadiusChanged));
        
#if WINDOWS_PHONE || WINDOWS_PHONE_7
        /// <summary>
        /// Getsor sets the rotate items of the <see 
        /// cref="Syncfusion.WP.Controls.Navigation.RadialPanel"/>
        /// </summary>
#else
        /// <summary>
        /// Getsor sets the rotate items of the <see 
        /// cref="Syncfusion.UI.Xaml.Controls.Navigation.RadialPanel"/>
        /// </summary>
#endif
        public bool RotateItems
        {
            get { return (bool)GetValue(RotateItemsProperty); }
            set { SetValue(RotateItemsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RotateItems.  This enables animation, styling, binding, etc...
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is drop down open; otherwise, <c>false</c>.
        /// </value>
        public static readonly DependencyProperty RotateItemsProperty =
            DependencyProperty.Register("RotateItems", typeof(bool), typeof(RadialPanel), new PropertyMetadata(true,OnRotateItemsChanged));

        #endregion

        #region CallBack Methods

        private static void OnRadiusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadialPanel radialPanel = (RadialPanel)sender;
            radialPanel.UpdateItemsLayout(radialPanel.ActualWidth, radialPanel.ActualHeight);          
        }

        private static void OnRotateItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadialPanel radialPanel = (RadialPanel)sender;
            radialPanel.UpdateItemsLayout(radialPanel.ActualWidth, radialPanel.ActualHeight);
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Sets the size of the <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialPanel"/>
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            double maxWidth = 0d;
            double maxHeight = 0d;
           
            foreach (UIElement element in Children)
            {
                element.Measure(availableSize);
                maxWidth = Math.Max(maxWidth, element.DesiredSize.Width);
                maxHeight = Math.Max(maxHeight, element.DesiredSize.Height);
            }

            double elementWidth = double.IsPositiveInfinity(availableSize.Width) ?
                (2.0 * Radius) + maxHeight : availableSize.Width;
            double elementHeight = double.IsPositiveInfinity(availableSize.Height) ?
                (2.0 * Radius) + maxHeight : availableSize.Height;

            return new Size(elementWidth, elementHeight);
        }

        /// <summary>
        /// Sets the size of the <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.RadialPanel"/>
        /// on override
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {          
            UpdateItemsLayout(finalSize.Width, finalSize.Height);
            return finalSize;
        }

        #endregion

        #region Helper Methods

        private void UpdateItemsLayout(double availableWidth,double availableHeight)
        {
            int i = 0;
            int childrenCount = Children.Count;


            if (StartAngle != EndAngle)
                childrenCount = Children.Count - 1;

            if (StartAngle == 0d && EndAngle == 360d)
                childrenCount = Children.Count;
            if (StartAngle == 360d && EndAngle == 0d)
                childrenCount = Children.Count;
            double angularSpace = Math.Abs(StartAngle - EndAngle)/childrenCount;

            foreach (UIElement element in Children)
            {
                double width = element.DesiredSize.Width/2.0;
                double height = element.DesiredSize.Height/2.0 ;

                double angle = 0d;
               
                if (element is RadialLabel && (element as RadialLabel).Angle > 0d)
                {
                    angle = (element as RadialLabel).Angle;
                }
                else if (element is RadialTick && (element as RadialTick).Angle > 0d)
                {
                    angle = (element as RadialTick).Angle;
                }
                else
                {
					if(StartAngle > EndAngle)
                        angle  = StartAngle-(angularSpace * i++);
                    else
                    {
                        angle = StartAngle +(angularSpace * i++);
                    }
                }

                if (RotateItems)
                {
                    RotateTransform transform = new RotateTransform();
                    transform.CenterX = width;
                    transform.CenterY = height;
                    transform.Angle = angle;
                    element.RenderTransform = transform;
                }

                double x = Radius * Math.Cos((Math.PI * angle) / 180.0);
                double y = Radius * Math.Sin((Math.PI * angle) / 180.0);

                element.Arrange(new Rect((x + (availableWidth / 2.0)) - width, (y + (availableHeight / 2.0)) - height,
                    element.DesiredSize.Width,
                    element.DesiredSize.Height));
            }
        }

        #endregion
    }
}