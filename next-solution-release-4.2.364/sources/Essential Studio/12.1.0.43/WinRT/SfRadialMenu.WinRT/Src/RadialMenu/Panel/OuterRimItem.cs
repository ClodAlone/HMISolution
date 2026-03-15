// <copyright file="OuterRimItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
#if !(WINDOWS_PHONE_7 || Silverlight4)
using System.Threading.Tasks;
#endif
#if WINRT||WINDOWS_PHONE
using Windows.UI;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using Syncfusion.WP.Controls.Navigation;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;
namespace Syncfusion.Windows.Controls.Navigation
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// Represents an OuterRimItem that enables the user to select from
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/> control.
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public sealed class OuterRimItem : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.OuterRimItem"/> class.
        /// </summary>
        internal OuterRimItem()
        {
            DefaultStyleKey = typeof(OuterRimItem);
            this.Loaded += OuterRimItem_Loaded;
        }

        void OuterRimItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (MenuItem is SfRadialColorItem)
            {
             if (MenuItem.Items.Count == 0)
             {
                 if(PART_Arrow != null)
                    Style = null;
             }
            }
#if WINRT || WINDOWS_PHONE||WINDOWS_PHONE_7||WPF
            if (MenuItem!=null &&!MenuItem.IsEnabled)
                VisualStateManager.GoToState(this, "Disabled", true);
#endif
        }

        #endregion

        #region Variables

        internal FrameworkElement PART_Arrow;

        internal FrameworkElement PART_PointerOverRim;


        internal OuterRim outerRim;

        internal SfRadialMenuItem MenuItem;


        #endregion

        //private RadialMenuItem menuitem;

        //public RadialMenuItem MenuItem
        //{
        //    get
        //    {
        //        return menuitem;
        //    }

        //    internal set
        //    {
        //        menuitem = value;
        //    }
        //}

        #region Dependency Properties
        /// <summary>
        /// Returns a value if the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.OuterRimItem"/> has any items
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is drop down open; otherwise, <c>false</c>.
        /// </value>
        public bool HasItems
        {
            get { return (bool)GetValue(HasItemsProperty); }
            internal set { SetValue(HasItemsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HasItems.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasItemsProperty =
            DependencyProperty.Register("HasItems", typeof(bool), typeof(OuterRimItem), new PropertyMetadata(false));

        /// <summary>
        /// Gets and sets the start point of the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.OuterRimItem"/> control
        /// </summary>
        public Point StartPoint
        {
            get { return (Point)GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StartPoint.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(Point), typeof(OuterRimItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets and sets the angle of the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.OuterRimItem"/> control in which the items are distributed equally
        /// </summary>
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Angle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(OuterRimItem), new PropertyMetadata(0.0d));

        /// <summary>
        /// Returns a value when the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.OuterRimItem"/> control arc is large
        /// </summary> 
        /// <value>
        /// <c>true</c> if this instance is drop down open; otherwise, <c>false</c>.
        /// </value>
        public bool IsLargeArc
        {
            get { return (bool)GetValue(IsLargeArcProperty); }
            set { SetValue(IsLargeArcProperty, value); }
        }

        ///<summary>
        /// Using a DependencyProperty as the backing store for IsLargeArc.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsLargeArcProperty =
            DependencyProperty.Register("IsLargeArc", typeof(bool), typeof(OuterRimItem), new PropertyMetadata(false));

        /// <summary>
        /// Gets and sets the thickness of the stroke for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialColorMenu"/> control
        /// </summary> 
        /// <value>
        /// The default value is 25.0
        /// </value>
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        ///<summary>
        /// Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(OuterRimItem), new PropertyMetadata(25.0d));

        /// <summary>
        /// Gets and sets the ArrowPoint for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialColorMenu"/> control
        /// </summary> 
        public Point ArrowPoint
        {
            get { return (Point)GetValue(ArrowPointProperty); }
            set { SetValue(ArrowPointProperty, value); }
        }

        ///<summary>
        /// Using a DependencyProperty as the backing store for ArrowPoint.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ArrowPointProperty =
            DependencyProperty.Register("ArrowPoint", typeof(Point), typeof(OuterRimItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets and sets the Point for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialColorMenu"/> control
        /// </summary> 
        public Point Point
        {
            get { return (Point)GetValue(PointProperty); }
            set { SetValue(PointProperty, value); }
        }

        ///<summary>
        /// Using a DependencyProperty as the backing store for Point.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PointProperty =
            DependencyProperty.Register("Point", typeof(Point), typeof(OuterRimItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets and sets the size of the rim for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialColorMenu"/> control
        /// </summary> 
        public Size RimSize
        {
            get { return (Size)GetValue(RimSizeProperty); }
            set { SetValue(RimSizeProperty, value); }
        }

        ///<summary>
        /// Using a DependencyProperty as the backing store for RimSize.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RimSizeProperty =
            DependencyProperty.Register("RimSize", typeof(Size), typeof(OuterRimItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets and sets the point of the rim for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialColorMenu"/> control
        /// </summary> 
        public Point RimPoint
        {
            get { return (Point)GetValue(RimPointProperty); }
            set { SetValue(RimPointProperty, value); }
        }

        ///<summary>
        /// Using a DependencyProperty as the backing store for RimPoint.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RimPointProperty =
            DependencyProperty.Register("RimPoint", typeof(Point), typeof(OuterRimItem), new PropertyMetadata(null));

        
        /// <summary>
        /// Returns a value when the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialColorMenu"/> control is checked
        /// </summary> 
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        ///<summary>
        /// Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(OuterRimItem), new PropertyMetadata(false));

        ///<summary>
        /// Using a DependencyProperty as the backing store for IsCheckable.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCheckableProperty =
            DependencyProperty.Register("IsCheckable", typeof (bool), typeof (OuterRimItem), new PropertyMetadata(default(bool)));

        /// <summary>
        /// Returns a value when the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialColorMenu"/> control is checkable
        /// </summary> 
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsCheckable
        {
            get { return (bool) GetValue(IsCheckableProperty); }
            set { SetValue(IsCheckableProperty, value); }
        }

        /// <summary>
        /// Returns a brush color for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.OuterRimItem"/> control
        /// </summary> 
        /// <value>
        /// The default value is null.
        /// </value>
        public Brush RimActiveBrush
        {
            get { return (Brush)GetValue(RimActiveBrushProperty); }
            set { SetValue(RimActiveBrushProperty, value); }
        }

        ///<summary>
        /// Using a DependencyProperty as the backing store for RimActiveBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RimActiveBrushProperty =
            DependencyProperty.Register("RimActiveBrush", typeof(Brush), typeof(OuterRimItem), new PropertyMetadata(null));


        /// <summary>
        /// Returns a brush color for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.OuterRimItem"/> control
        /// </summary> 
        /// <value>
        /// The default value is null.
        /// </value>
        public Brush RimHoverBrush
        {
            get { return (Brush)GetValue(RimHoverBrushProperty); }
            set { SetValue(RimHoverBrushProperty, value); }
        }

        ///<summary>
        /// Using a DependencyProperty as the backing store for RimHoverBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RimHoverBrushProperty =
            DependencyProperty.Register("RimHoverBrush", typeof(Brush), typeof(OuterRimItem), new PropertyMetadata(null));


        /// <summary>
        /// Returns a brush color for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.OuterRimItem"/> control
        /// </summary> 
        /// <value>
        /// The default value is null.
        /// </value>
        public Brush RimBackground
        {
            get { return (Brush)GetValue(RimBackgroundProperty); }
            set { SetValue(RimBackgroundProperty, value); }
        }

        ///<summary>
        /// Using a DependencyProperty as the backing store for RimBackground.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RimBackgroundProperty =
            DependencyProperty.Register("RimBackground", typeof(Brush), typeof(OuterRimItem), new PropertyMetadata(null));

#if WPF
        /// <summary>
        /// Returns a value when the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.OuterRimItem"/> control is a color item
        /// </summary> 
        /// <value>
        /// <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsColorItem
        {
            get { return (bool)GetValue(IsColorItemProperty); }
            set { SetValue(IsColorItemProperty, value); }
        }
        
        ///<summary>
        /// Using a DependencyProperty as the backing store for IsColorItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsColorItemProperty =
            DependencyProperty.Register("IsColorItem", typeof(bool), typeof(OuterRimItem), new PropertyMetadata(true));     
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7

        /// <summary>
        /// Returns a brush color for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.OuterRimItem"/> control
        /// </summary> 
        /// <value>
        /// The default value is null.
        /// </value>
        public Brush OuterRimStroke
        {
            get { return (Brush)GetValue(OuterRimStrokeProperty); }
            set { SetValue(OuterRimStrokeProperty, value); }
        }
        
        /// <summary>
        /// Returns the stroke thickness for the <see 
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.OuterRimItem"/> control
        /// </summary> 
        /// <value>
        /// The default value is 2.0.
        /// </value>
        ///<summary>
        /// Using a DependencyProperty as the backing store for OuterRimFill.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OuterRimStrokeProperty =
            DependencyProperty.Register("OuterRimStroke", typeof(Brush), typeof(OuterRimItem), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the stroke thickness for the outer rim
        /// </summary>
        public double OuterRimStrokeThickness
        {
            get { return (double)GetValue(OuterRimStrokeThicknessProperty); }
            set { SetValue(OuterRimStrokeThicknessProperty, value); }
        }
        
        ///<summary>
        /// Using a DependencyProperty as the backing store for OuterRimStrokeThickness.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OuterRimStrokeThicknessProperty =
            DependencyProperty.Register("OuterRimStrokeThickness", typeof(double), typeof(OuterRimItem), new PropertyMetadata(2.0d));

      
       

#endif


        #endregion

        #region Helper Methods

        private void TimelineCompleted(object sender, object args)
        {
           AnimateItems();
        }
        private void AnimateItems()
        {
            if (MenuItem is SfRadialColorItem)
            {
                MenuItem.radialMenu.DrillDownItem = this.MenuItem;
                MenuItem.radialMenu.AnimateIn(0, 1, 0.0001,0.0001, EasingMode.EaseOut, null);
                MenuItem.radialMenu.PART_InnerColorItemRim.Visibility = Visibility.Visible;
                if (MenuItem.radialMenu.previousDrillDowItem != null)
                {
                    IList<object> items=new List<object>();
                    foreach (var item in MenuItem.radialMenu.previousDrillDowItem.Items)
                    {
                        if (item is SfRadialColorItem)
                            items.Add(item);
                    }
                    MenuItem.radialMenu.PART_InnerColorItemRim.ItemsSource = items;
                }
                MenuItem.radialMenu.PART_InnerColorItemRim.UpdateLayout();
                MenuItem.radialMenu.AnimateInColorItem(1, 0.5,0.05,EasingMode.EaseIn, null);

            }
            else
            {
                MenuItem.radialMenu.DrillDownItem = this.MenuItem;
                MenuItem.radialMenu.AnimateIn(0, 1,0.2,0.2,EasingMode.EaseOut, null);
            }
        }

        #endregion
        
        #region Override Methods

        /// <summary>
        /// Initializes all the variables of the class
        /// </summary>
#if !WINRT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            PART_Arrow = GetTemplateChild("PART_Arrow") as FrameworkElement;
            PART_PointerOverRim = GetTemplateChild("PART_PointerOverRim") as FrameworkElement;
          
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Sets the focus to the control when invoked
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
#else
        protected override void OnPointerEntered(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if (MenuItem!=null && MenuItem.IsEnabled)
            {
                if (MenuItem is SfRadialColorItem)
                {
#if !WPF
                    VisualStateManager.GoToState(this, "ColorPointerOver", true);
#endif
                }
                else
                {
                    if (MenuItem.HasItems)
#if WPF
                    IsColorItem = false;
#else
                        VisualStateManager.GoToState(this, "PointerOver", true);
#endif
                }
            }
            else
            {
#if WINRT || WINDOWS_PHONE||WINDOWS_PHONE_7||WPF
                VisualStateManager.GoToState(this, "Disabled", true);
#endif
            }
#if !WINRT
            base.OnMouseEnter(e);
#else
            base.OnPointerEntered(e);
#endif
        }
        /// <summary>
        /// Releases the focus from the control when invoked.
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
#else
        protected override void OnPointerExited(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if (MenuItem != null)
            {
                if (MenuItem.IsEnabled)
                {
                    if (!IsCheckable || !IsChecked)
                        VisualStateManager.GoToState(this, "Normal", true);
                }
                else
                {
#if WINRT || WINDOWS_PHONE||WINDOWS_PHONE_7||WPF
                    VisualStateManager.GoToState(this, "Disabled", true);
#endif
                }
            }
#if !WINRT
            base.OnMouseLeave(e);           
#else
            base.OnPointerExited(e);
#endif
        }

        /// <summary>
        /// Enables the animations when invoked
        /// </summary>
        /// <param name="e"></param>
#if !WINRT
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
#else
        protected override void OnPointerReleased(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            if (MenuItem != null && MenuItem.radialMenu != null && !(MenuItem.radialMenu.EnableFreeRotation && MenuItem.radialMenu.manipulationStarted))
            {
                if (MenuItem != null && MenuItem.HasItems)
                {
                    if (MenuItem is SfRadialColorItem)
                    {
                        (MenuItem as SfRadialColorItem).ChangeParentColor();
#if WINRT||WPF
                        if (MenuItem.Items.Count > 0&& MenuItem.IsEnabled) 
#else
                        if (MenuItem.Children.Count > 0)
#endif
                            AnimateItems();
                    }
                       
                    else if (MenuItem is SfRadialMenuItem && MenuItem.IsEnabled)
                    {
#if !WINRT
                        MenuItem.radialMenu.AnimateIn(1, 0,0.2,0.2,System.Windows.Media.Animation.EasingMode.EaseIn, TimelineCompleted);
#else
                         MenuItem.radialMenu.AnimateIn(1, 0,0.2,0.2,Windows.UI.Xaml.Media.Animation.EasingMode.EaseIn, TimelineCompleted);
#endif
                    }
                }
            }
#if !WINRT
            base.OnMouseLeftButtonUp(e);
#else
            base.OnPointerReleased(e);
#endif
        }

        #endregion
        
    }
}
