// <copyright file="OuterRim.cs" company="Syncfusion">
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
#if WINDOWS_PHONE||WINDOWS_PHONE_7
using System.Windows.Controls;
using System.Windows;
using Syncfusion.WP.Controls.Navigation;
using System.Windows.Data;
using Syncfusion.WP.Converters;
using Syncfusion.WP.Primitives;

namespace Syncfusion.WP.Controls.Navigation
#elif SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using Syncfusion.Tools.Converters;
using Syncfusion.Tools.Primitives;

namespace Syncfusion.Tools.Controls.Navigation
#elif WPF
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using Syncfusion.Windows.Converters;
using Syncfusion.Windows.Primitives;
using System.Windows.Controls.Primitives;
namespace Syncfusion.Windows.Controls.Navigation
#else
using Syncfusion.UI.Xaml.Converters;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Syncfusion.UI.Xaml.Primitives;
using Windows.UI.Xaml;

namespace Syncfusion.UI.Xaml.Controls.Navigation
#endif
{
    /// <summary>
    /// Represents an OuterRim that contains the items the user can select from.
    /// <see cref="T:Syncfusion.UI.Xaml.Controls.Navigation.SfRadialSlider"/>
    /// </summary>
    /// <remarks>
    /// <para></para>
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
#if WINDOWS_PHONE_7
    public sealed class OuterRim : Syncfusion.WP.Primitives.HeaderedItemsControl
#elif WPF
    public sealed class OuterRim : System.Windows.Controls.HeaderedItemsControl
#else
    public sealed class OuterRim :HeaderedItemsControl
#endif
    {
        #region Variables

        internal SfRadialMenu radialMenu;

        internal OuterRimPanel rimPanel;
        /// <summary>
        /// Gets and sets the style for the item container
        /// </summary>
#if !WPF
#if WINRT
        public new Style ItemContainerStyle
#else
        public Style ItemContainerStyle
#endif
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemContainerStyle.  This enables animation, styling, binding, etc...
        /// </summary>
#if WPF||WINRT
        public new static readonly DependencyProperty ItemContainerStyleProperty =
            
#else
        public static readonly DependencyProperty ItemContainerStyleProperty =
#endif
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(OuterRim), new PropertyMetadata(null));
#endif
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Navigation.OuterRim"/> class.
        /// </summary>
        public OuterRim()
        {
            DefaultStyleKey = typeof(OuterRim);
            this.Loaded += OuterRim_Loaded;
        }


        #endregion

        #region Helper Methods
#if !WINRT
        void OuterRim_Loaded(object sender, System.Windows.RoutedEventArgs e)
#else
        void OuterRim_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
#endif
        {
            
        }

        
        #endregion

        #region Override Methods

        /// <summary>
        /// Returns an item if overrided
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is OuterRimItem;
        }

        /// <summary>
        /// Returns a dependency object if overrided
        /// </summary>
        /// <returns></returns>
#if !WINRT
        protected override System.Windows.DependencyObject GetContainerForItemOverride()
#else
        protected override Windows.UI.Xaml.DependencyObject GetContainerForItemOverride()
#endif
        {
            return new OuterRimItem();
        }
        /// <summary>
        /// Enables the container for overriding items
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
#if !WINRT
        protected override void PrepareContainerForItemOverride(System.Windows.DependencyObject element, object item)
#else
        protected override void PrepareContainerForItemOverride(Windows.UI.Xaml.DependencyObject element, object item)
#endif
        {
            if(ItemContainerStyle!=null)
                (element as FrameworkElement).Style = ItemContainerStyle;

            if (radialMenu != null)
            {
                OuterRimItem rimitem = element as OuterRimItem;
                rimitem.outerRim = this;

                Binding thicknessbinding = new Binding();
                thicknessbinding.Source = radialMenu;

#if !WINRT
                thicknessbinding.Path = new System.Windows.PropertyPath("StrokeThickness");
#else
                thicknessbinding.Path = new Windows.UI.Xaml.PropertyPath("StrokeThickness");
#endif
                rimitem.SetBinding(OuterRimItem.StrokeThicknessProperty, thicknessbinding);

                Binding rimActiveBrushBinding = new Binding();
                if (item is SfRadialColorItem)
                {
                    rimActiveBrushBinding.Source = (item as SfRadialColorItem);
                    rimActiveBrushBinding.Converter = new ColorToBrushConverter();
#if !WINRT
                    rimActiveBrushBinding.Path = new System.Windows.PropertyPath("Color");
#else
                    rimActiveBrushBinding.Path = new Windows.UI.Xaml.PropertyPath("Color");
#endif
                }
                else
                {

                    rimActiveBrushBinding.Source = radialMenu;
#if !WINRT
                    rimActiveBrushBinding.Path = new System.Windows.PropertyPath("RimActiveBrush");
#else
                    rimActiveBrushBinding.Path = new Windows.UI.Xaml.PropertyPath("RimActiveBrush");
#endif
                  

                }
               

                rimitem.SetBinding(OuterRimItem.RimActiveBrushProperty, rimActiveBrushBinding);


                Binding isCheckedBinding = new Binding();
                isCheckedBinding.Source = item;
#if !WINRT
                isCheckedBinding.Path = new System.Windows.PropertyPath("IsChecked");
#else
                isCheckedBinding.Path = new Windows.UI.Xaml.PropertyPath("IsChecked");
#endif
                rimitem.SetBinding(OuterRimItem.IsCheckedProperty, isCheckedBinding);
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                Binding rimHoverBrushBinding = new Binding();
                rimHoverBrushBinding.Source = radialMenu;
#if WPFSILVERLIGHT
                rimHoverBrushBinding.Path =new System.Windows.PropertyPath("RimHoverBrush");
                rimHoverBrushBinding.Path = new System.Windows.PropertyPath("RimHoverBrush");
#else
                rimHoverBrushBinding.Path = new Windows.UI.Xaml.PropertyPath("RimHoverBrush");
                rimHoverBrushBinding.Path = new Windows.UI.Xaml.PropertyPath("RimHoverBrush");
#endif
                rimitem.SetBinding(OuterRimItem.RimHoverBrushProperty, rimHoverBrushBinding);
#endif
                Binding rimBackgroundBinding = new Binding();
                rimBackgroundBinding.Source = radialMenu;
#if !WINRT
                rimBackgroundBinding.Path = new System.Windows.PropertyPath("RimBackground");
#else
                rimBackgroundBinding.Path = new Windows.UI.Xaml.PropertyPath("RimBackground");
#endif
                rimitem.SetBinding(OuterRimItem.RimBackgroundProperty, rimBackgroundBinding);
#if WINDOWS_PHONE||WINDOWS_PHONE_7
                Binding rimOuterRimStrokeThicknessBinding = new Binding();
                rimOuterRimStrokeThicknessBinding.Source = radialMenu;
                rimOuterRimStrokeThicknessBinding.Path = new System.Windows.PropertyPath("OuterRimStrokeThickness");
                rimitem.SetBinding(OuterRimItem.OuterRimStrokeThicknessProperty, rimOuterRimStrokeThicknessBinding);

                Binding rimOuterRimStrokeBinding = new Binding();
                rimOuterRimStrokeBinding.Source = radialMenu;
                rimOuterRimStrokeBinding.Path = new System.Windows.PropertyPath("OuterRimStroke");
                rimitem.SetBinding(OuterRimItem.OuterRimStrokeProperty, rimOuterRimStrokeBinding);
#endif

                if (item is SfRadialMenuItem)
                {
                    rimitem.DataContext = item;
                    (item as SfRadialMenuItem).checkableRimItem = rimitem;
                }
#if !(WINDOWS_PHONE||WINDOWS_PHONE_7)
                rimitem.Style = ItemContainerStyle;
#endif
            }
            base.PrepareContainerForItemOverride(element, item);
        }

        #endregion
    }
}
