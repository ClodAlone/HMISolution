// <copyright file="HeatMapItem.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Xml;

    /// <summary>
    /// Represents an item hosted within a <see cref="HeatMapControl"/>. You can add child items to this type as well. When
    /// a HeatMapControl's ItemsSource property is set, these types are automatically created for each bound item.
    /// </summary>
    /// <example>
    /// <code language="XAML">
    ///  &lt;syncfusion:HeatMapControl Name="heatMap"
    ///                               LowestWeightColor="ForestGreen"
    ///                               MedianWeightColor="YellowGreen"
    ///                               HighestWeightColor="IndianRed" Margin="20"&gt;
    ///       &lt;!--Adding HeatMapItem--&gt;
    ///              &lt;syncfusion:HeatMapItem Weight="1000" ColorWeight="400"
    /// Level="0"/&gt;
    ///                     &lt;syncfusion:HeatMapItem Weight="500" Header="US:New York"
    /// ColorWeight="200" Level="1"/&gt;   
    ///         &lt;/syncfusion:HeatMapControl&gt;
    ///         </code>
    /// </example>
    /// <seealso cref="HeatMapItem"/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    [TemplatePart(Name = HeatMapItem.HeaderPartName, Type = typeof(FrameworkElement)), StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(HeatMapItem))]
    public class HeatMapItem : HeaderedItemsControl
    {
        /// <summary>
        /// Identifies the <see cref="Weight"/> property.
        /// </summary>
        public static readonly DependencyProperty WeightProperty = DependencyProperty.Register("Weight", typeof(double), typeof(HeatMapItem), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        ///  Identifies the ItemVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemVisibilityProperty = DependencyProperty.Register("ItemVisibility", typeof(Visibility), typeof(HeatMapItem), new FrameworkPropertyMetadata(Visibility.Visible, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        /// <summary>
        /// Identifies the <see cref="ColorWeight"/> property.
        /// </summary>
        public static readonly DependencyProperty ColorWeightProperty = DependencyProperty.Register("ColorWeight", typeof(double), typeof(HeatMapItem), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Identifies the <see cref="WeightValuePath"/> property.
        /// </summary>
        public static readonly DependencyProperty WeightValuePathProperty = DependencyProperty.Register("WeightValuePath", typeof(string), typeof(HeatMapItem), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, HeatMapItem.OnWeightValuePathChanged));

        /// <summary>
        /// Identifies the <see cref="ColorWeightValuePath"/> property.
        /// </summary>
        public static readonly DependencyProperty ColorWeightValuePathProperty = DependencyProperty.Register("ColorWeightValuePath", typeof(string), typeof(HeatMapItem), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange, HeatMapItem.OnColorWeightValuePathChanged));

        /// <summary>
        /// Identifies the <see cref="IsColorInfoAvailable"/> property.
        /// </summary>
        public static readonly DependencyProperty IsColorInfoAvailableProperty = DependencyProperty.Register("IsColorInfoAvailable", typeof(bool), typeof(HeatMapItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None));

        /// <summary>
        /// Identifies the <see cref="Level"/> property.
        /// </summary>
        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register("Level", typeof(int), typeof(HeatMapItem), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.None));

        /// <summary>
        /// Identifies the <see cref="ItemsLayoutMode"/> property.
        /// </summary>
        public static DependencyProperty ItemsLayoutModeProperty = DependencyProperty.Register("ItemsLayoutMode", typeof(HeatMapLayoutMode), typeof(HeatMapItem), new FrameworkPropertyMetadata(HeatMapLayoutMode.Squarified, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Initializes constant HeaderPartName
        /// </summary>
        private const string HeaderPartName = "PART_Header";

        /// <summary>
        /// Initializes static members of the <see cref="T:Syncfusion.Windows.Chart.HeatMapItem">HeatMapItem</see> class. 
        /// </summary>
        /// <remarks></remarks>
        static HeatMapItem()
        {
            Syncfusion.Licensing.EnvironmentTest.ValidateLicense(typeof(HeatMapItem));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HeatMapItem), new FrameworkPropertyMetadata(typeof(HeatMapItem)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Chart.HeatMapItem">HeatMapItem</see> class. 
        /// </summary>
        public HeatMapItem()
            : this(-1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Chart.HeatMapItem">HeatMapItem</see> class. Constructor used to specify the level property of this item.
        /// </summary>
        /// <param name="level">Specifies the level property.</param>
        public HeatMapItem(int level)
        {
            if (level != -1)
            {
                this.Level = level;
            }
        }
        internal Brush GetWeightedBGGradient()
        {          
            HeatMapControl control = this.ParentHeatMapControl;
            if (this.IsColorInfoAvailable && control != null && this.ItemMeasure != null

                && this.ItemMeasure.ColorWeightsInfo != null)
            {
                if (!double.IsNaN(this.ColorWeight))
                {
                    ColorWeightsInfo clrWeightInfo = this.ItemMeasure.ColorWeightsInfo;
                    if (!double.IsPositiveInfinity(clrWeightInfo.LowestValue)
                        && clrWeightInfo.HighestValue != 0)
                    {
                        if (control.IsGradientBrush == true)
                        {
                            
                           return clrWeightInfo.GetGradientFromValue(
                                   this.ColorWeight, control.LowestWeightGradient, control.MedianWeightGradient, control.HighestWeightGradient, control.MedianWeight);

                        }

                    }
                }
            }
            return new LinearGradientBrush();

        }
        /// <summary>
        /// The GetWeightedBGColor method
        /// </summary>
        /// <returns>The color value</returns>
        internal Color GetWeightedBGColor()
        {
            HeatMapControl control = this.ParentHeatMapControl;
            if (this.IsColorInfoAvailable && control != null && this.ItemMeasure != null

                && this.ItemMeasure.ColorWeightsInfo != null)
            {
                if (!double.IsNaN(this.ColorWeight))
                {
                    ColorWeightsInfo clrWeightInfo = this.ItemMeasure.ColorWeightsInfo;
                    if (!double.IsPositiveInfinity(clrWeightInfo.LowestValue)
                        && clrWeightInfo.HighestValue != 0)
                    {
                        if (control.IsGradientBrush == false)
                        {
                            return clrWeightInfo.GetColorFromValue(
                                   this.ColorWeight, control.LowestWeightColor, control.MedianWeightColor, control.HighestWeightColor, control.MedianWeight);
                        }                       
                        
                    }
                }
            }

            return new Color();
        }

        /// <summary>
        /// The OnHeaderChanged method
        /// </summary>
        /// <param name="oldHeader">The old header</param>
        /// <param name="newHeader">The new header</param>
        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            base.OnHeaderChanged(oldHeader, newHeader);
            this.SetupWeightBinding();
            this.SetupColorWeightBinding();
        }
        #region Hierarchy realted
        /// <summary>
        /// Method to Get Container For Item Override
        /// </summary>
        /// <returns>The HeatMapItem</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new HeatMapItem(this.Level + 1);
        }

        /// <summary>
        /// Method to check whether Is Item Its Own Container Override
        /// </summary>
        /// <param name="item">The object item</param>
        /// <returns>The HeatMapItem</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is HeatMapItem;
        }

        #endregion

        /// <summary>
        /// The OnWeightValuePathChanged method
        /// </summary>
        /// <param name="d">The DependencyObject d</param>
        /// <param name="args">The DependencyPropertyChangedEvent arguments</param>
        private static void OnWeightValuePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((HeatMapItem)d).SetupWeightBinding();
        }

        /// <summary>
        /// The OnColorWeightValuePathChanged method
        /// </summary>
        /// <param name="d">The DependencyObject d</param>
        /// <param name="args">The DependencyPropertyChangedEvent arguments</param>
        private static void OnColorWeightValuePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((HeatMapItem)d).SetupColorWeightBinding();
        }

        /// <summary>
        /// The SetupWeightBinding method
        /// </summary>
        private void SetupWeightBinding()
        {
            BindingOperations.ClearBinding(this, WeightProperty);
            this.SetupContentBinding(this.WeightValuePath, WeightProperty);
        }

        /// <summary>
        /// The SetupColorWeightBinding method
        /// </summary>
        private void SetupColorWeightBinding()
        {
            BindingOperations.ClearBinding(this, ColorWeightProperty);
            this.SetupContentBinding(this.ColorWeightValuePath, ColorWeightProperty);
        }

        /// <summary>
        /// The SetupContentBinding method
        /// </summary>
        /// <param name="sourceProp">The source property</param>
        /// <param name="destProp">The destination property</param>
        private void SetupContentBinding(string sourceProp, DependencyProperty destProp)
        {
            if (!string.IsNullOrEmpty(sourceProp)
                    && this.Header != null)
            {
                Binding binding = new Binding();
                binding.Source = this.Header;
                if (this.Header is XmlNode)
                {
                    binding.XPath = sourceProp;
                }
                else
                {
                    binding.Path = new PropertyPath(sourceProp);
                }

                BindingOperations.SetBinding(this, destProp, binding);
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies the level within the hierarchy. This value will be automatically generated if and only if this control is data bound.
        /// The top most level is 0.
        /// </summary>
        public int Level
        {
            get { return (int)this.GetValue(HeatMapItem.LevelProperty); }
            set { this.SetValue(HeatMapItem.LevelProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that specifies the property name in the bound items from which to retrieve the weight info for that item.
        /// By default, this value is derived from the corresponding setting in the parent HeatMapControl.
        /// </summary>
        public string WeightValuePath
        {
            get { return (string)this.GetValue(HeatMapItem.WeightValuePathProperty); }
            set { this.SetValue(HeatMapItem.WeightValuePathProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that specifies the property name in the bound items from which to retrieve the weight info for that item.
        /// By default, this value is derived from the corresponding setting in the parent HeatMapControl.
        /// </summary>
        public string ColorWeightValuePath
        {
            get { return (string)this.GetValue(HeatMapItem.ColorWeightValuePathProperty); }
            set { this.SetValue(HeatMapItem.ColorWeightValuePathProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether enough color info is available to determine the background color for this item. This is typically
        /// used in your custom HeatMapItem style definitions.
        /// </summary>
        public bool IsColorInfoAvailable
        {
            get { return (bool)this.GetValue(HeatMapItem.IsColorInfoAvailableProperty); }
            set { this.SetValue(HeatMapItem.IsColorInfoAvailableProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the layout mode for the child items. By default, this value is derived from the 
        /// corresponding setting in the parent HeatMapControl.
        /// </summary>
        public HeatMapLayoutMode ItemsLayoutMode
        {
            get { return (HeatMapLayoutMode)this.GetValue(HeatMapItem.ItemsLayoutModeProperty); }
            set { this.SetValue(HeatMapItem.ItemsLayoutModeProperty, value); }
        }

        /// <summary>
        /// Get and Set ItemVisibilityProperty
        /// </summary>
        public Visibility ItemVisibility
        {
            get { return (Visibility)this.GetValue(HeatMapItem.ItemVisibilityProperty); }
            set { this.SetValue(HeatMapItem.ItemVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the weight of the item based on which the area it occupies in the resultant layout is determined.
        /// This is usually set indirectly via the WeightValuePath property
        /// </summary>
        public double Weight
        {
            get
            {
                return (double)this.GetValue(HeatMapItem.WeightProperty);
            }

            set
            {
                if (this.ParentHeatMapControl != null && !string.IsNullOrEmpty(this.ParentHeatMapControl.WeightValuePath))
                {
                    throw new ArgumentException("Cannot set HeatMapItem.Weight property when the parent HeatMapControl has it's WeightValuePath property set.");
                }

                this.SetValue(HeatMapItem.WeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the background color weight of the item based on which the background of the item is determined.
        /// This is usually set indirectly via the ColorWeightValuePath property
        /// </summary>
        public double ColorWeight
        {
            get
            {
                return (double)this.GetValue(HeatMapItem.ColorWeightProperty);
            }

            set
            {
                if (this.ParentHeatMapControl != null && !string.IsNullOrEmpty(this.ParentHeatMapControl.ColorValuePath))
                {
                    throw new ArgumentException("Cannot set HeatMapItem.ColorWeight property when the parent HeatMapControl has it's ColorValuePath property set.");
                }

                this.SetValue(HeatMapItem.ColorWeightProperty, value);
            }
        }

        /// <summary>
        /// Gets the Parent HeatMapControl
        /// </summary>  
        internal HeatMapControl ParentHeatMapControl
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
        /// Gets or sets a value indicating more info about the position and size of the item.
        /// </summary>
        /// <remarks>
        /// This property can be accessed in the ItemTemplates as shown below.
        /// <example>
        /// First setup an ItemTemplate as follows
        /// <code>
        ///             &lt;DataTemplate x:Key="itemDataTemplate"&gt;
        ///               &lt;Grid Background="Wheat"&gt;
        ///                 &lt;TextBlock  TextWrapping="Wrap" Text="{Binding RelativeSource={RelativeSource TemplatedParent}, Converter={StaticResource myConverter}}" /&gt;
        ///               &lt;/Grid>
        ///             &lt;/DataTemplate&gt;
        /// </code>
        /// Then in your custom converter:
        /// <code>
        ///     public class MyConverter : IValueConverter
        ///     {
        ///         object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        ///         {
        ///             ContentPresenter cp = value as ContentPresenter;
        ///             HeatMapItem item = cp.TemplatedParent as HeatMapItem;
        ///             // Show the Area occupied by the item:
        ///             return "Area is: " + item.ItemMeasure.AreaByWeight.ToString();
        ///         }
        ///     }
        /// </code>
        /// </example>
        /// </remarks>
        public HeatMapItemMeasure ItemMeasure { get; set; }
    }

    /// <summary>
    /// Represents MyItemContentProvider
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    internal class MyItemContentProvider : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        /// <summary>
        /// The Convert method
        /// </summary>
        /// <param name="values">The values</param>
        /// <param name="targetType">The targetType</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="culture">The culture</param>
        /// <returns>Returns the value</returns>
        ///  <seealso cref="MyItemContentProvider"/>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool hasItems = (bool)values[1];
            if (hasItems)
            {
                return null;
            }
            else
            {
                return values[0];
            }
        }

        /// <summary>
        /// The ConvertBack method
        /// </summary>
        /// <param name="value">The object value</param>
        /// <param name="targetTypes">The targetTypes</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="culture">The culture</param>
        /// <returns>Returns the value</returns>
        /// <seealso cref="MyItemContentProvider"/>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Represents MyBGColorConverter
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif

   
    internal class MyBGColorConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        /// <summary>
        /// The Convert method
        /// </summary>
        /// <param name="values">The object values</param>
        /// <param name="targetType">The targetType</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="culture">The culture</param>
        /// <returns>Returns the value</returns>
        /// <seealso cref="MyBGColorConverter"/>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            HeatMapItem item = values[0] as HeatMapItem;
            HeatMapControl heatmap = item.ParentHeatMapControl as HeatMapControl;
           
            if (heatmap != null)
            {
                if (heatmap.IsGradientBrush == false)
                {
                    return new SolidColorBrush(item.GetWeightedBGColor());
                }
                else
                {

                    return item.GetWeightedBGGradient();
                }
            }
            return values;
        }

        /// <summary>
        /// The ConvertBack method
        /// </summary>
        /// <param name="value">The object value</param>
        /// <param name="targetTypes">The targetTypes</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="culture">The culture</param>
        /// <returns>Returns the value</returns>
        /// <seealso cref="MyBGColorConverter"/>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
