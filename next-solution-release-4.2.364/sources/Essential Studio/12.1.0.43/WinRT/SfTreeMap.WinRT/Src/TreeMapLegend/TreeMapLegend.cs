#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
using System.Windows;
#endif

namespace Syncfusion.UI.Xaml.TreeMap
{
    public class TreeMapLegend : ItemsControl
    {
        #region Constructor

        public TreeMapLegend()
        {
            DefaultStyleKey = typeof(TreeMapLegend);
            resourceDictionary = new ResourceDictionary
            {
#if WINRT
                Source = new Uri("ms-appx:///Syncfusion.SfTreeMap.WinRT/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#elif WPF
                Source = new Uri("/Syncfusion.SfTreeMap.WPF;component/Themes/Generic.xaml", UriKind.Relative)
#elif WINDOWS_PHONE8
                Source = new Uri("/Syncfusion.SfTreeMap.WP8;component/Themes/Generic.xaml", UriKind.Relative)
#elif WINDOWS_PHONE7
                Source = new Uri("/Syncfusion.SfTreeMap.WP7;component/Themes/Generic.xaml", UriKind.Relative)
#elif SILVERLIGHT
                Source = new Uri("/Syncfusion.SfTreeMap.Silverlight;component/Themes/Generic.xaml", UriKind.Relative)
#endif
            };
            SetLegendIcon();
        }

        #endregion

        #region Dependency Properties

        #region Header
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(TreeMapLegend), new PropertyMetadata(null));
        #endregion

        #region LegendPosition
        public TreeMapLegendPosition LegendPosition
        {
            get { return (TreeMapLegendPosition)GetValue(LegendPositionProperty); }
            set { SetValue(LegendPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendPositionProperty =
            DependencyProperty.Register("LegendPosition", typeof(TreeMapLegendPosition), typeof(TreeMapLegend), new PropertyMetadata(TreeMapLegendPosition.Top, OnLegendPositionChanged));

        private static void OnLegendPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapLegend)
            {
                var treeMapLegend = d as TreeMapLegend;
                treeMapLegend.Orientation = (treeMapLegend.LegendPosition == TreeMapLegendPosition.Left || treeMapLegend.LegendPosition == TreeMapLegendPosition.Right) ?
                                            Orientation.Vertical : Orientation.Horizontal;
                if (treeMapLegend.TreeMap != null)
                    treeMapLegend.TreeMap.SetLegendPosition();
            }
        }
        #endregion

        #region LegendItemWidth
        public double LegendItemWidth
        {
            get { return (double)GetValue(LegendItemWidthProperty); }
            set { SetValue(LegendItemWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendItemWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendItemWidthProperty =
            DependencyProperty.Register("LegendItemWidth", typeof(double), typeof(TreeMapLegend), new PropertyMetadata(0d));
        #endregion

        #region LegendItemHeight
        public double LegendItemHeight
        {
            get { return (double)GetValue(LegendItemHeightProperty); }
            set { SetValue(LegendItemHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendItemHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendItemHeightProperty =
            DependencyProperty.Register("LegendItemHeight", typeof(double), typeof(TreeMapLegend), new PropertyMetadata(0d));
        #endregion

        #region LegendItemMargin
        public Thickness LegendItemMargin
        {
            get { return (Thickness)GetValue(LegendItemMarginProperty); }
            set { SetValue(LegendItemMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendItemMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendItemMarginProperty =
            DependencyProperty.Register("LegendItemMargin", typeof(Thickness), typeof(TreeMapLegend), new PropertyMetadata(new Thickness(2)));
        #endregion

        #region LegendItemElementMargin
        public Thickness LegendItemElementMargin
        {
            get { return (Thickness)GetValue(LegendItemElementMarginProperty); }
            set { SetValue(LegendItemElementMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendItemElementMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendItemElementMarginProperty =
            DependencyProperty.Register("LegendItemElementMargin", typeof(Thickness), typeof(TreeMapLegend), new PropertyMetadata(new Thickness(1)));
        #endregion

        #region LegendIconWidth
        public double LegendIconWidth
        {
            get { return (double)GetValue(LegendIconWidthProperty); }
            set { SetValue(LegendIconWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendIconWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendIconWidthProperty =
            DependencyProperty.Register("LegendIconWidth", typeof(double), typeof(TreeMapLegend), new PropertyMetadata(15d));
        #endregion

        #region LegendIconHeight
        public double LegendIconHeight
        {
            get { return (double)GetValue(LegendIconHeightProperty); }
            set { SetValue(LegendIconHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendIconHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendIconHeightProperty =
            DependencyProperty.Register("LegendIconHeight", typeof(double), typeof(TreeMapLegend), new PropertyMetadata(15d));
        #endregion

        #region LegendIconStyle
        public TreeMapLegendIcon LegendIconStyle
        {
            get { return (TreeMapLegendIcon)GetValue(LegendIconStyleProperty); }
            set { SetValue(LegendIconStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendIconStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendIconStyleProperty =
            DependencyProperty.Register("LegendIconStyle", typeof(TreeMapLegendIcon), typeof(TreeMapLegend), new PropertyMetadata(TreeMapLegendIcon.Rectangle, OnLegendIconChanged));

        private static void OnLegendIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapLegend)
            {
                var treeMapLegend = (d as TreeMapLegend);
                treeMapLegend.SetLegendIcon();
                if (treeMapLegend.TreeMap != null && treeMapLegend.ItemsSource != null)
                    treeMapLegend.TreeMap.UpdateLegendItems();
            }
        }
        #endregion

        #region LegendIconTemplate
        public DataTemplate LegendIconTemplate
        {
            get { return (DataTemplate)GetValue(LegendIconTemplateProperty); }
            set { SetValue(LegendIconTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendIconTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendIconTemplateProperty =
            DependencyProperty.Register("LegendIconTemplate", typeof(DataTemplate), typeof(TreeMapLegend), new PropertyMetadata(null, OnLegendIconChanged));
        #endregion

        #endregion

        #region Internal Dependency Properties

        #region Orientation
        internal Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendOrientation.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(TreeMapLegend), new PropertyMetadata(Orientation.Horizontal));
        #endregion

        #region IconTemplate
        internal DataTemplate IconTemplate
        {
            get { return (DataTemplate)GetValue(IconTemplateProperty); }
            set { SetValue(IconTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconTemplate.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IconTemplateProperty =
            DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(TreeMapLegend), new PropertyMetadata(null));
        #endregion

        #endregion

        #region CLR Properties

        public SfTreeMap TreeMap { get; internal set; }

        #endregion

        #region Internal Members

        private readonly ResourceDictionary resourceDictionary;
        internal ContentPresenter legendHeaderPresenter;
        internal TreeMapLegendPanel legendPanel;

        #endregion

        #region Override Methods
#if WINRT
        protected override void OnApplyTemplate()
        {
#else
        public override void OnApplyTemplate()
        {
#endif
            legendHeaderPresenter = GetTemplateChild("PART_LegendHeader") as ContentPresenter;
            base.OnApplyTemplate();
        }

        #endregion

        #region Implementation
        private void SetLegendIcon()
        {
            if (resourceDictionary != null)
            {
                IconTemplate = LegendIconStyle == TreeMapLegendIcon.Custom ? LegendIconTemplate :
                               resourceDictionary[LegendIconStyle.ToString()] as DataTemplate;
            }
        }
        #endregion
    }
}
