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
using System.ComponentModel;
using System.Linq;
using System.Text;
#if !(WINDOWS_PHONE_7 || Silverlight4)
using System.Threading.Tasks;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using Syncfusion.WP.Controls.Navigation;
using System.Windows.Media;

namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
namespace Syncfusion.Windows.Controls.Navigation
#else
using Windows.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// Represents a control that allows the user to select a color by using a drop-down
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialMenu"/> control.
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class SfRadialColorItem : SfRadialMenuItem
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialMenu"/> class.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/>
        /// <seealso
        /// cref="N:Syncfusion.UI.Xaml.Controls.Navigation">Syncfusion.UI.Xaml.Controls.Navigation
        /// Namespace</seealso>
        [ClassReference(IsReviewed = false)]
        public SfRadialColorItem()
        {
            DefaultStyleKey = typeof (SfRadialColorItem);
            this.Loaded += RadialColorItem_Loaded;
        }

        void RadialColorItem_Loaded(object sender, RoutedEventArgs e)
        {
            this.HasItems = true;
#if !WPF
            if (this.radialMenu != null && this.radialMenu.PART_InnerRim != null)
                this.StrokeThickness = (this.radialMenu.PART_InnerRim.ActualWidth / 5);
#endif
#if WINRT || WINDOWS_PHONE||WINDOWS_PHONE_7
            if (!this.IsEnabled)
                VisualStateManager.GoToState(this, "Disabled", true);
#endif
        }

#if WPF
        public override void OnApplyTemplate()
        {
            if (this.radialMenu != null && this.radialMenu.PART_InnerRim != null)
                this.StrokeThickness = (this.radialMenu.PART_InnerRim.ActualWidth / 5);
            base.OnApplyTemplate();
        }
#endif
        /// <summary>
        /// Gets and sets the start point of the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialColorMenu"/> control
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Point StartPoint
        {
            get { return (Point)GetValue(StartPointProperty); }
            internal set { SetValue(StartPointProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StartPoint.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(Point), typeof(SfRadialColorItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets and sets the angle of the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialColorMenu"/> control in which the items are distributed equally
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
#if (WINDOWS_PHONE||WINDOWS_PHONE_7)
        public new double Angle
#else
        public double Angle
#endif
        {
            get { return (double)GetValue(AngleProperty); }
            internal set { SetValue(AngleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Angle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(SfRadialColorItem), new PropertyMetadata(0.0d));

        /// <summary>
        /// Returns a value when the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialColorMenu"/> control arc is large
        /// </summary> 
        /// <value>
        /// <c>true</c> if this instance is drop down open; otherwise, <c>false</c>.
        /// </value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsLargeArc
        {
            get { return (bool)GetValue(IsLargeArcProperty); }
            internal set { SetValue(IsLargeArcProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsLargeArc.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsLargeArcProperty =
            DependencyProperty.Register("IsLargeArc", typeof(bool), typeof(SfRadialColorItem), new PropertyMetadata(false));

        /// <summary>
        /// Gets and sets the thickness of the stroke for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialColorMenu"/> control
        /// </summary> 
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(SfRadialColorItem), new PropertyMetadata(25.0d));

        /// <summary>
        /// Gets and sets the point for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialColorMenu"/> control
        /// </summary> 
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Point Point
        {
            get { return (Point)GetValue(PointProperty); }
            internal set { SetValue(PointProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Point.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PointProperty =
            DependencyProperty.Register("Point", typeof(Point), typeof(SfRadialColorItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets and sets the size of the rim for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialColorMenu"/> control
        /// </summary> 
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Size RimSize
        {
            get { return (Size)GetValue(RimSizeProperty); }
            internal set { SetValue(RimSizeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RimSize.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RimSizeProperty =
            DependencyProperty.Register("RimSize", typeof(Size), typeof(SfRadialColorItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets and sets the point of the rim for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialColorMenu"/> control
        /// </summary> 
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Point RimPoint
        {
            get { return (Point)GetValue(RimPointProperty); }
            internal set { SetValue(RimPointProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RimPoint.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RimPointProperty =
            DependencyProperty.Register("RimPoint", typeof(Point), typeof(SfRadialColorItem), new PropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof (Color), typeof (SfRadialColorItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets and sets the color for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialColorMenu"/> control
        /// </summary> 
        public Color Color
        {
            get { return (Color) GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        /// <summary>
        /// Gets and sets the ActiveBrush of the rim for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialColorMenu"/> control
        /// </summary> 
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush RimActiveBrush
        {
            get { return (Brush)GetValue(RimActiveBrushProperty); }
            internal set { SetValue(RimActiveBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RimActiveBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RimActiveBrushProperty =
            DependencyProperty.Register("RimActiveBrush", typeof(Brush), typeof(SfRadialColorItem), new PropertyMetadata(null));


        /// <summary>
        /// Gets and sets the background of the rim for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialColorMenu"/> control
        /// </summary> 
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Brush RimBackground
        {
            get { return (Brush)GetValue(RimBackgroundProperty); }
            internal set { SetValue(RimBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RimBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RimBackgroundProperty =
            DependencyProperty.Register("RimBackground", typeof(Brush), typeof(SfRadialColorItem), new PropertyMetadata(null));

        /// <summary>
        /// Removes the focus from the selected item
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnManipulationCompleted(System.Windows.Input.ManipulationCompletedEventArgs e)
#else
        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if (this.radialMenu != null && this.radialMenu.DrillDownItem is SfRadialColorItem &&
                !(this.Parent is SfRadialColorItem))
            {
                this.radialMenu.GoBack();
            }
            ChangeParentColor();
#if !WINRT

#else
            base.OnPointerReleased(e);
#endif
        }

        internal void ChangeParentColor()
        {
            SfRadialColorItem rColorItem = this.Parent as SfRadialColorItem;
            if (rColorItem != null)
            {
                rColorItem.Color = this.Color;
            }
        }
    }
}
