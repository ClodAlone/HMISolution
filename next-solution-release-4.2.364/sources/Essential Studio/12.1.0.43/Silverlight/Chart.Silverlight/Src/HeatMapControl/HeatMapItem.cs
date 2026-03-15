#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Data;
    using System.Collections.ObjectModel;
    using System.Xml;
    using System.Xml.Linq;
    using System.Collections;
    using System.Windows.Markup;
   

    /// <summary>
    /// Represents HeatMap item
    /// </summary>
    public partial class HeatMapItem : HeaderedItemsControl
    {
        /// <summary>
        /// Identifies the  Weight Dependency property for HeatMapControl. 
        /// </summary>
        public static readonly DependencyProperty WeightProperty = DependencyProperty.Register("Weight", typeof(double), typeof(HeatMapItem), new PropertyMetadata(double.NaN, new PropertyChangedCallback(OnWeightChanged)));

        /// <summary>
        /// Identifies ColorWeight Dependency property for HeatMapControl. 
        /// </summary>
        public static readonly DependencyProperty ColorWeightProperty = DependencyProperty.Register("ColorWeight", typeof(double), typeof(HeatMapItem), new PropertyMetadata(double.NaN, new PropertyChangedCallback(OnColorWeightChanged)));

        /// <summary>
        /// Identifies WeightValuePath Dependency property for HeatMapControl. 
        /// </summary>
        public static readonly DependencyProperty WeightValuePathProperty = DependencyProperty.Register("WeightValuePath", typeof(string), typeof(HeatMapItem), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnWeightValuePathChanged)));

        /// <summary>
        /// Identifies ColorWeightValuePath Dependency property for HeatMapControl. 
        /// </summary>
        public static readonly DependencyProperty ColorWeightValuePathProperty = DependencyProperty.Register("ColorWeightValuePath", typeof(string), typeof(HeatMapItem), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnColorWeightValuePathChanged)));

        /// <summary>
        /// Identifies whether the color information is available or not.
        /// </summary>
        public static readonly DependencyProperty IsColorInfoAvailableProperty = DependencyProperty.Register("IsColorInfoAvailable", typeof(bool), typeof(HeatMapItem), new PropertyMetadata(false, new PropertyChangedCallback(OnIsColorInfoAvailableChanged)));

        /// <summary>
        /// Identifies Level Dependency property for HeatMapControl. 
        /// </summary>
        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level", typeof(int), typeof(HeatMapItem), new PropertyMetadata(-1, new PropertyChangedCallback(OnLevelChanged)));

        /// <summary>
        /// Identifies ItemsLayoutMode Dependency property for HeatMapControl. 
        /// </summary>
        public static readonly DependencyProperty ItemsLayoutModeProperty = DependencyProperty.Register("ItemsLayoutMode", typeof(ItemsLayoutMode), typeof(HeatMapItem), new PropertyMetadata(ItemsLayoutMode.SliceAndDiceAuto, new PropertyChangedCallback(OnItemsLayoutModeChanged)));

        /// <summary>
        /// Event that is raised when <see cref="Weight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback WeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="ColorWeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ColorWeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsColorInfoAvailable"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsColorInfoAvailableChanged;

        /// <summary>
        /// Event that is raised when <see cref="Level"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback LevelChanged;

        /// <summary>
        /// Event that is raised when <see cref="ItemsLayoutMode"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemsLayoutModeChanged;

        /// <summary>
        /// Event that is raised when <see cref="Weight"/> property is changed.
        /// </summary>
        private static void OnWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMapItem obj = (HeatMapItem)d;
            obj.OnWeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="WeightChanged"/> event.
        /// </summary>
        protected virtual void OnWeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (WeightChanged != null)
            {
                WeightChanged(this, e);
            }
        }

        /// <summary>
        /// Event that is raised when <see cref="ColorWeight"/> property is changed.
        /// </summary>
        private static void OnColorWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMapItem obj = (HeatMapItem)d;
            ((HeatMapItem)d).UpdateBackground();
            obj.OnColorWeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ColorWeightChanged"/> event.
        /// </summary>
        protected virtual void OnColorWeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ColorWeightChanged != null)
            {
                ColorWeightChanged(this, e);
            }
        }

        /// <summary>
        /// Event that is raised when <see cref="WeightValuePath"/> property is changed.
        /// </summary>
        private static void OnWeightValuePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMapItem obj = (HeatMapItem)d;
            obj.OnWeightValuePathChanged(e);
        }

        /// <summary>
        /// Updates property value cache 
        /// </summary>
        protected virtual void OnWeightValuePathChanged(DependencyPropertyChangedEventArgs e)
        {
            ////if (WeightValuePathChanged != null)
            {
                this.SetupWeightBinding();
                ////WeightValuePathChanged(this, e);
            }
        }

        /// <summary>
        /// Event that is raised when <see cref="ColorWeightValuePath"/> property is changed.
        /// </summary>
        private static void OnColorWeightValuePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMapItem obj = (HeatMapItem)d;
            obj.OnColorWeightValuePathChanged(e);
        }

        /// <summary>
        /// Updates property value cache 
        /// </summary>
        protected virtual void OnColorWeightValuePathChanged(DependencyPropertyChangedEventArgs e)
        {
            this.SetupColorWeightBinding();
        }

        /// <summary>
        /// Event that is raised when <see cref="IsColorInfoAvailable"/> property is changed.
        /// </summary>
        private static void OnIsColorInfoAvailableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMapItem obj = (HeatMapItem)d;
            ((HeatMapItem)d).UpdateBackground();
            obj.OnIsColorInfoAvailableChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises<see cref="IsColorInfoAvailableChanged"/> event.
        /// </summary>
        protected virtual void OnIsColorInfoAvailableChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsColorInfoAvailableChanged != null)
            {
                IsColorInfoAvailableChanged(this, e);
            }
        }

        /// <summary>
        /// Event that is raised when <see cref="Level"/> property is changed.
        /// </summary>
        private static void OnLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMapItem obj = (HeatMapItem)d;
            obj.OnLevelChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="LevelChanged"/> event.
        /// </summary>
        protected virtual void OnLevelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LevelChanged != null)
            {
                LevelChanged(this, e);
            }
        }

        /// <summary>
        /// Event that is raised when <see cref="ItemsLayoutMode"/> property is changed.
        /// </summary>
        private static void OnItemsLayoutModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMapItem obj = (HeatMapItem)d;
            obj.OnItemsLayoutModeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ItemsLayoutModeChanged"/> event.
        /// </summary>
        protected virtual void OnItemsLayoutModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemsLayoutModeChanged != null)
            {
                ItemsLayoutModeChanged(this, e);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ColorWeightsInfo dependency
        /// property.
        /// </summary>
        public ColorWeightsInfo ColorWeightsInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of the Weight dependency
        /// property.
        /// </summary>
        public double Weight
        {
            get
            {
                return (double)GetValue(WeightProperty);
            }

            set
            {
                SetValue(WeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ColorWeight dependency
        /// property.
        /// </summary>
        public double ColorWeight
        {
            get
            {
                return (double)GetValue(ColorWeightProperty);
            }

            set
            {
                SetValue(ColorWeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the WeightValuePath dependency
        /// property.
        /// </summary>
        public string WeightValuePath
        {
            get
            {
                return (string)GetValue(WeightValuePathProperty);
            }

            set
            {
                SetValue(WeightValuePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ColorWeightValuePath dependency
        /// property.
        /// </summary>
        public string ColorWeightValuePath
        {
            get
            {
                return (string)GetValue(ColorWeightValuePathProperty);
            }

            set
            {
                SetValue(ColorWeightValuePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether of the IsColorInfoAvailable dependency
        /// property.
        /// </summary>
        public bool IsColorInfoAvailable
        {
            get
            {
                return (bool)GetValue(IsColorInfoAvailableProperty);
            }

            set
            {
                SetValue(IsColorInfoAvailableProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the Level dependency
        /// property.
        /// </summary>
        public int Level
        {
            get
            {
                return (int)GetValue(LevelProperty);
            }

            set
            {
                SetValue(LevelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ItemsLayoutMode dependency
        /// property.
        /// </summary>
        public ItemsLayoutMode ItemsLayoutMode
        {
            get
            {
                return (ItemsLayoutMode)GetValue(ItemsLayoutModeProperty);
            }

            set
            {
                SetValue(ItemsLayoutModeProperty, value);

                if (value == ItemsLayoutMode.Squarified)
                {
                    this.ItemsPanel = (ItemsPanelTemplate)XamlReader.Load(@"<ItemsPanelTemplate xmlns=""http://schemas.microsoft.com/client/2007"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:local=""clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight""> <local:SquarifiedHeatMapsPanel/> </ItemsPanelTemplate>");
                }
                else if (value == ItemsLayoutMode.SliceAndDiceHorizontal)
                {
                    this.ItemsPanel = (ItemsPanelTemplate)XamlReader.Load(@"<ItemsPanelTemplate xmlns=""http://schemas.microsoft.com/client/2007"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:local=""clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight""> <local:HorizontalSlicesPanel/> </ItemsPanelTemplate>");
                }
                else if (value == ItemsLayoutMode.SliceAndDiceVertical)
                {
                    this.ItemsPanel = (ItemsPanelTemplate)XamlReader.Load(@"<ItemsPanelTemplate xmlns=""http://schemas.microsoft.com/client/2007"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:local=""clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight""> <local:VerticalSlicesPanel/> </ItemsPanelTemplate>");
                }
                else if (value == ItemsLayoutMode.SliceAndDiceAuto)
                {
                    this.ItemsPanel = (ItemsPanelTemplate)XamlReader.Load(@"<ItemsPanelTemplate xmlns=""http://schemas.microsoft.com/client/2007"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:local=""clr-namespace:Syncfusion.Windows.Chart;assembly=Syncfusion.Chart.Silverlight""> <local:HeatMapsPanel/> </ItemsPanelTemplate>");
                }
            }
        }

        /// <summary>
        /// Gets the value of the PreferredItemsPanelWidth dependency
        /// property.
        /// </summary>
        private HeatMapControl ParentHeatMapControl
        {
            get
            {
                DependencyObject parent = this;
                while (parent != null && !(parent is HeatMapControl))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }

                return parent as HeatMapControl;
            }
        }

        /// <summary>
        /// Gets or sets the value of the PreferredItemsPanelWidth dependency
        /// property.
        /// </summary>
        public HeatMapItemMeasure ItemMeasure
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of the PreferredItemsPanelWidth dependency
        /// property.
        /// </summary>
        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            base.OnHeaderChanged(oldHeader, newHeader);
            this.SetupWeightBinding();
            this.SetupColorWeightBinding();
        }

        private void SetupWeightBinding()
        {
            this.SetupContentBinding(this.WeightValuePath, WeightProperty);
        }

        private void SetupColorWeightBinding()
        {
            this.SetupContentBinding(this.ColorWeightValuePath, ColorWeightProperty);
        }

        private void SetupContentBinding(string sourceProp, DependencyProperty destProp)
        {
            if (!string.IsNullOrEmpty(sourceProp) && this.Header != null)
            {
                this.DataContext = this.Header;
                Binding binding = new Binding();
                binding.Source = this.Header;
                binding.Path = new PropertyPath(sourceProp);
                this.SetBinding(destProp, binding);
            }
        }

        /// <summary>
        /// Method updates the Background color
        /// </summary>
        public void UpdateBackground()
        {
            Color clr = this.GetWeightedBGColor();
            this.Background = new SolidColorBrush(this.GetWeightedBGColor());
        }

        void control_HighestWeightColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Binding hightcolorBind = new Binding();
            hightcolorBind.Source = (HeatMapControl)d;
            hightcolorBind.Path = new PropertyPath("HighestWeightColor");
            hightcolorBind.Converter = new HeatMapItemColorConverter();
            hightcolorBind.ConverterParameter = this;
            hightcolorBind.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(this, HeatMapItem.BackgroundProperty, hightcolorBind);
        }

        void control_MedianWeightColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Binding middlecolorBind = new Binding();
            middlecolorBind.Source = (HeatMapControl)d;
            middlecolorBind.Path = new PropertyPath("MedianWeightColor");
            middlecolorBind.Converter = new HeatMapItemColorConverter();
            middlecolorBind.ConverterParameter = this;
            middlecolorBind.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(this, HeatMapItem.BackgroundProperty, middlecolorBind);
        }

        void control_LowestWeightColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Binding lowcolorBind = new Binding();
            lowcolorBind.Source = (HeatMapControl)d;
            lowcolorBind.Path = new PropertyPath("LowestWeightColor");
            lowcolorBind.Converter = new HeatMapItemColorConverter();
            lowcolorBind.ConverterParameter = this;
            lowcolorBind.Mode = BindingMode.OneWay;
            BindingOperations.SetBinding(this, HeatMapItem.BackgroundProperty, lowcolorBind);
        }

        internal Color GetWeightedBGColor()
        {
            HeatMapControl control = this.ParentHeatMapControl;
            if (this.IsColorInfoAvailable && control != null && this.ItemMeasure != null && this.ItemMeasure.ColorWeightsInfo != null)
            {
                // If this is null it means that in the current mode this item does not participate in color weight processing.
                if (!double.IsNaN(this.ColorWeight))
                {
                    ColorWeightsInfo clrWeightInfo = this.ItemMeasure.ColorWeightsInfo;
                    if (!double.IsPositiveInfinity(clrWeightInfo.LowestValue) && clrWeightInfo.HighestValue != 0)
                    {
                        return clrWeightInfo.GetColorFromValue(this.ColorWeight, control.LowestWeightColor, control.MedianWeightColor, control.HighestWeightColor, control.MedianWeight);
                    }
                }
            }

            return new Color();
        }

        ////Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HeatMapItem()
            : this(-1)
        {
            DefaultStyleKey = typeof(HeatMapItem);
            this.Margin = new Thickness(.5);
            Style style = this.Style;
            this.Loaded += new RoutedEventHandler(HeatMapItem_Loaded);
        }

        void HeatMapItem_Loaded(object sender, RoutedEventArgs e)
        {
            HeatMapControl control = this.ParentHeatMapControl;
            if (control != null)
            {
                control.LowestWeightColorChanged += new PropertyChangedCallback(control_LowestWeightColorChanged);
                control.MedianWeightColorChanged += new PropertyChangedCallback(control_MedianWeightColorChanged);
                control.HighestWeightColorChanged += new PropertyChangedCallback(control_HighestWeightColorChanged);
            }
        }

        /// <summary>
        /// constructor that sets the level.
        /// </summary>
        public HeatMapItem(int level)
        {
            if (level != -1)
            {
                this.Level = level;
            }

            Style style = this.Style;
        }

        /// <summary>
        /// Called when an internal process or application calls
        /// ApplyTemplate, which is used to build the current template's
        /// visual tree. 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        #region Relates to HeaderedItemsControl
        /// <summary>
        /// Returns the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new HeatMapItem();
        }

        /// <summary>
        /// Determines whether the element that is used to display the given item is same
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is HeatMapItem;
        }

        /// <summary>
        /// Called when the value of the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> property changes.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that contains the event data</param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
             base.OnItemsChanged(e);
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The container element used to display the specified item.</param><param name="item">The content to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
        }
        #endregion
    }

    /// <summary>
    /// Class implementation for HeatMapItemColorConverter
    /// </summary>
    public class HeatMapItemColorConverter : IValueConverter
    {

        #region IValueConverter Members

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        /// <param name="value">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            HeatMapItem item = parameter as HeatMapItem;
            if (item != null)
            {
                return new SolidColorBrush(item.GetWeightedBGColor());
            }

            return new Color();
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}