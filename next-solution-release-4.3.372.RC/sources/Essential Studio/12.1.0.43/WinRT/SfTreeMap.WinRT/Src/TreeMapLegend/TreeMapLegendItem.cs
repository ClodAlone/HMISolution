#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.ComponentModel;
#if WINRT
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
#endif

namespace Syncfusion.UI.Xaml.TreeMap
{
    public class TreeMapLegendItem : DependencyObject, INotifyPropertyChanged
    {
        #region Dependency Properties

        #region Fill
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            internal set { SetValue(FillProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Fill.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(TreeMapLegendItem), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
        #endregion

        #region Label
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            internal set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(TreeMapLegendItem), new PropertyMetadata(null));
        #endregion

        #region IconWidth
        public double IconWidth
        {
            get { return (double)GetValue(IconWidthProperty); }
            internal set { SetValue(IconWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register("IconWidth", typeof(double), typeof(TreeMapLegendItem), new PropertyMetadata(15d, OnIconWidthChanged));

        private static void OnIconWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapLegendItem)
                (d as TreeMapLegendItem).OnPropertyChanged("IconWidth");
        }
        #endregion

        #region IconHeight
        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            internal set { SetValue(IconHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register("IconHeight", typeof(double), typeof(TreeMapLegendItem), new PropertyMetadata(15d, OnIconHeightChanged));

        private static void OnIconHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapLegendItem)
                (d as TreeMapLegendItem).OnPropertyChanged("IconHeight");
        }

        #endregion

        #region ItemMargin
        public Thickness ItemMargin
        {
            get { return (Thickness)GetValue(ItemMarginProperty); }
            internal set { SetValue(ItemMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemMarginProperty =
            DependencyProperty.Register("ItemMargin", typeof(Thickness), typeof(TreeMapLegendItem), new PropertyMetadata(new Thickness(2), OnItemMarginChanged));

        private static void OnItemMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapLegendItem)
                (d as TreeMapLegendItem).OnPropertyChanged("ItemMargin");
        }
        #endregion

        #region ElementMargin
        public Thickness ElementMargin
        {
            get { return (Thickness)GetValue(ElementMarginProperty); }
            internal set { SetValue(ElementMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ElementMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ElementMarginProperty =
            DependencyProperty.Register("ElementMargin", typeof(Thickness), typeof(TreeMapLegendItem), new PropertyMetadata(new Thickness(1), OnElementMarginChanged));

        private static void OnElementMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapLegendItem)
            {
                (d as TreeMapLegendItem).OnPropertyChanged("ElementMargin");
            }
        }
        #endregion

        #region Icon
        public object Icon
        {
            get { return GetValue(IconProperty); }
            internal set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(TreeMapLegendItem), new PropertyMetadata(null));
        #endregion

        #region IconTemplate
        public object IconTemplate
        {
            get { return GetValue(IconTemplateProperty); }
            internal set { SetValue(IconTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconTemplateProperty =
            DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(TreeMapLegendItem), new PropertyMetadata(null, OnIconTemplateChanged));

        private static void OnIconTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapLegendItem)
                (d as TreeMapLegendItem).OnPropertyChanged("IconTemplate");
        }
        #endregion

        #endregion

        #region CLR Properties

        #region Legend
        private TreeMapLegend legend;
        public TreeMapLegend Legend
        {
            get { return legend; }
            internal set
            {
                legend = value;
                if (legend != null)
                {
                    var iconWidthBinding = new Binding { Source = legend, Path = new PropertyPath("LegendIconWidth") };
                    BindingOperations.SetBinding(this, IconWidthProperty, iconWidthBinding);

                    var iconHeightBinding = new Binding { Source = legend, Path = new PropertyPath("LegendIconHeight") };
                    BindingOperations.SetBinding(this, IconHeightProperty, iconHeightBinding);

                    var iconTemplateBinding = new Binding { Source = legend, Path = new PropertyPath("IconTemplate") };
                    BindingOperations.SetBinding(this, IconTemplateProperty, iconTemplateBinding);

                    var itemMarginBinding = new Binding { Source = legend, Path = new PropertyPath("LegendItemMargin") };
                    BindingOperations.SetBinding(this, ItemMarginProperty, itemMarginBinding);

                    var elementMarginBinding = new Binding { Source = legend, Path = new PropertyPath("LegendItemElementMargin") };
                    BindingOperations.SetBinding(this, ElementMarginProperty, elementMarginBinding);
                }
            }
        }
        #endregion

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
