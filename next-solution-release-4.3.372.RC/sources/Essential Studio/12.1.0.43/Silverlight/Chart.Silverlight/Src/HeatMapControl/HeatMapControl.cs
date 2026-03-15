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
    using System.Windows.Markup;
    

    /// <summary>
    /// The Main Class of the HeatMap  Control
    /// </summary>
    public partial class HeatMapControl : ItemsControl
    {
        /// <summary>
        /// This property indicates PreferredItemsPanelWidth for HeatMapControl. 
        /// </summary>
        public static readonly DependencyProperty PreferredItemsPanelWidthProperty = DependencyProperty.Register("PreferredItemsPanelWidth", typeof(double), typeof(HeatMapControl), new PropertyMetadata((double)300));

        /// <summary>
        /// This property indicates PreferredItemsPanelHeight for HeatMapControl. 
        /// </summary>
        public static readonly DependencyProperty PreferredItemsPanelHeightProperty = DependencyProperty.Register("PreferredItemsPanelHeight", typeof(double), typeof(HeatMapControl), new PropertyMetadata((double)300));
        
        /// <summary>
        /// This property indicates WeightValuePath for HeatMapControl. This property is used when the control is bound to the source.  
        /// </summary>
        public static readonly DependencyProperty WeightValuePathProperty = DependencyProperty.Register("WeightValuePath", typeof(string), typeof(HeatMapControl), new PropertyMetadata(string.Empty));
        
        /// <summary>
        /// This property indicates ColorWeightValuePath for HeatMapControl. This property is used when the control is bound to the source 
        /// </summary>
        public static readonly DependencyProperty ColorWeightValuePathProperty = DependencyProperty.Register("ColorWeightValuePath", typeof(string), typeof(HeatMapControl), new PropertyMetadata(string.Empty));
        
        /// <summary>
        /// This property indicates Color for the lowest weight value. 
        /// </summary>
        public static readonly DependencyProperty LowestWeightColorProperty = DependencyProperty.Register("LowestWeightColor", typeof(Color), typeof(HeatMapControl), new PropertyMetadata(Colors.Cyan, new PropertyChangedCallback(OnLowestWeightColorChanged)));
        
        /// <summary>
        /// This property indicates color for median weight value.
        /// </summary>
        public static readonly DependencyProperty MedianWeightColorProperty = DependencyProperty.Register("MedianWeightColor", typeof(Color), typeof(HeatMapControl), new PropertyMetadata(Colors.Yellow, new PropertyChangedCallback(OnMedianWeightColorChanged)));
        
        /// <summary>
        /// This property indicates color for height weight value.
        /// </summary>
        public static readonly DependencyProperty HighestWeightColorProperty = DependencyProperty.Register("HighestWeightColor", typeof(Color), typeof(HeatMapControl), new PropertyMetadata(Colors.Green, new PropertyChangedCallback(OnHighestWeightColorChanged)));
        
        /// <summary>
        /// This property indicates MedianWeight for HeatMapControl. 
        /// </summary>
        public static readonly DependencyProperty MedianWeightProperty = DependencyProperty.Register("MedianWeight", typeof(int), typeof(HeatMapControl), new PropertyMetadata(50, new PropertyChangedCallback(OnMedianWeightChanged)));
        
        /// <summary>
        /// This property indicates ColorCalculationLevel for HeatMapControl. 
        /// </summary>
        public static readonly DependencyProperty ColorCalculationLevelProperty = DependencyProperty.Register("ColorCalculationLevel", typeof(int), typeof(HeatMapControl), new PropertyMetadata(-1, new PropertyChangedCallback(OnColorCalculationLevelChanged)));
        
        /// <summary>
        /// This property indicates the mode for itemslayout for HeatMapControl. 
        /// </summary>
        public static readonly DependencyProperty ItemsLayoutModeProperty = DependencyProperty.Register("ItemsLayoutMode", typeof(ItemsLayoutMode), typeof(HeatMapControl), new PropertyMetadata(ItemsLayoutMode.Squarified, new PropertyChangedCallback(OnItemsLayoutModeChanged)));
        
        /// <summary>
        /// This property indicates item container style for HeatMapControl. 
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(HeatMapControl), new PropertyMetadata(null, new PropertyChangedCallback(OnItemContainerStyleChanged)));
        
        /// <summary>
        /// This property indicates content for HeatMapControl. 
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(string), typeof(HeatMapControl), new PropertyMetadata(null, new PropertyChangedCallback(OnContentChanged)));
        
        /// <summary>
        /// This property indicates template for content of HeatMapControl. 
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(HeatMapControl), new PropertyMetadata(null, new PropertyChangedCallback(OnContentTemplateChanged)));

        /// <summary>
        /// Event that is raised when <see cref="LowestWeightColor"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback LowestWeightColorChanged;
        
        /// <summary>
        /// Event that is raised when <see cref="MedianWeightColor"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback MedianWeightColorChanged;

        /// <summary>
        /// Event that is raised when <see cref="HighestWeightColor"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback HighestWeightColorChanged;
        
        /// <summary>
        /// Event that is raised when <see cref="ColorCalculationLevel"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ColorCalculationLevelChanged;
        
        /// <summary>
        /// Event that is raised when <see cref="ItemsLayoutMode"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ItemsLayoutModeChanged;
        
        /// <summary>
        /// Event that is raised when <see cref="Content"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ContentChanged;

        /// <summary>
        /// Event that is raised when <see cref="ContentTemplate"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ContentTemplateChanged;

        /// <summary>
        /// Calls OnLowestWeightColorChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnLowestWeightColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMapControl obj = (HeatMapControl)d;
            obj.OnLowestWeightColorChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="LowestWeightColorChanged"/> event.
        /// </summary>
        protected virtual void OnLowestWeightColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LowestWeightColorChanged != null)
            {
                LowestWeightColorChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnMedianWeightColorChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnMedianWeightColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMapControl obj = (HeatMapControl)d;
            obj.OnMedianWeightColorChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="MedianWeightColorChanged"/> event.
        /// </summary>
        protected virtual void OnMedianWeightColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MedianWeightColorChanged != null)
            {
                MedianWeightColorChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnHighestWeightColorChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnHighestWeightColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMapControl obj = (HeatMapControl)d;
            obj.OnHighestWeightColorChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="HighestWeightColorChanged"/> event.
        /// </summary>
        protected virtual void OnHighestWeightColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HighestWeightColorChanged != null)
            {
                HighestWeightColorChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnMedianWeightChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnMedianWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int newvalue = (int)e.NewValue;
            if (newvalue < 0 || newvalue > 100)
            {
                throw new ArgumentOutOfRangeException("HeatMapConrol.MedianWeight should be within the range 0 to 100. Encountered values is: " + newvalue.ToString());
            }
        }

        /// <summary>
        /// Calls OnItemContainerStyleChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMapControl source = d as HeatMapControl;
            Style value = e.NewValue as Style;
            source.ItemContainerGenerator.UpdateItemContainerStyle(value);
            ////((HeatMapControl)d).OnItemContainerStyleChanged((Style)e.OldValue, (Style)e.NewValue);
        }

        /// <summary>
        /// Calls OnColorCalculationLevelChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnColorCalculationLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMapControl obj = (HeatMapControl)d;
            obj.OnColorCalculationLevelChanged(e);
        }

        /// <summary>
        /// Calls OnContentChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMapControl obj = (HeatMapControl)d;
            obj.OnContentChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ContentChanged"/> event.
        /// </summary>
        protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ContentChanged != null)
            {
                ContentChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnContentTemplateChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMapControl obj = (HeatMapControl)d;
            obj.OnContentTemplateChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ContentTemplateChanged"/> event.
        /// </summary>
        protected virtual void OnContentTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ContentTemplateChanged != null)
            {
                ContentTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ColorCalculationLevelChanged"/> event.
        /// </summary>
        protected virtual void OnColorCalculationLevelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ColorCalculationLevelChanged != null)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.ColorWeightsInfo.LowestValue = Double.PositiveInfinity;
                    this.ColorWeightsInfo.HighestValue = 0;
                    this.ClearColorAvailability(this.Items);
                    this.InvalidateMeasure();
                    this.InvalidateArrange();
                    this.UpdateLayout();
                }

                ColorCalculationLevelChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnItemContainerStyleChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        internal void OnItemContainerStyleChanged(Style oldItemContainerStyle, Style newItemContainerStyle)
        {
            ////globalcontainerstyle = newItemContainerStyle;
            ////UIElementCollection elements = null;
            ////if (elements != null)
            ////{
            ////    foreach (UIElement element in elements)
            ////    {
            ////        HeatMapItem item = element as HeatMapItem;
            ////        if (item != null)
            ////        {
            ////            item.Style = newItemContainerStyle;
            ////        }
            ////    }
            ////}
        }

        private void ClearColorAvailability(ItemCollection items)
        {
            foreach (HeatMapItem item in items)
            {
                item.IsColorInfoAvailable = false;
                this.ClearColorAvailability(item.Items);
            }
        }

        private static void OnItemsLayoutModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeatMapControl obj = (HeatMapControl)d;
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
        /// Gets or sets the value of the PreferredItemsPanelWidth dependency
        /// property.
        /// </summary>
        public double PreferredItemsPanelWidth
        {
            get
            {
                return (double)GetValue(PreferredItemsPanelWidthProperty);
            }

            set
            {
                SetValue(PreferredItemsPanelWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the PreferredItemsPanelHeight dependency
        /// property.
        /// </summary>
        public double PreferredItemsPanelHeight
        {
            get
            {
                return (double)GetValue(PreferredItemsPanelHeightProperty);
            }

            set
            {
                SetValue(PreferredItemsPanelHeightProperty, value);
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
        /// Gets or sets the value of the LowestWeightColor dependency
        /// property.
        /// </summary>
        public Color LowestWeightColor
        {
            get
            {
                return (Color)GetValue(LowestWeightColorProperty);
            }

            set
            {
                SetValue(LowestWeightColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the MedianWeightColor dependency
        /// property.
        /// </summary>
        public Color MedianWeightColor
        {
            get
            {
                return (Color)GetValue(MedianWeightColorProperty);
            }

            set
            {
                SetValue(MedianWeightColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the HighestWeightColor dependency
        /// property.
        /// </summary>
        public Color HighestWeightColor
        {
            get
            {
                return (Color)GetValue(HighestWeightColorProperty);
            }

            set
            {
                SetValue(HighestWeightColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the MedianWeight dependency
        /// property.
        /// </summary>
        public int MedianWeight
        {
            get
            {
                return (int)GetValue(MedianWeightProperty);
            }

            set
            {
                SetValue(MedianWeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ColorCalculationLevel dependency
        /// property.
        /// </summary>
        public int ColorCalculationLevel
        {
            get
            {
                return (int)GetValue(ColorCalculationLevelProperty);
            }

            set
            {
                SetValue(ColorCalculationLevelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the Content dependency
        /// property.
        /// </summary>
        public string Content
        {
            get
            {
                return (string)GetValue(ContentProperty);
            }

            set
            {
                SetValue(ContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the ContentTemplate dependency
        /// property.
        /// </summary>
        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ContentTemplateProperty);
            }

            set
            {
                SetValue(ContentTemplateProperty, value);
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
        /// Gets or sets the value of the ItemContainerStyle dependency
        /// property.
        /// </summary>
        public Style ItemContainerStyle
        {
            get
            {
                return (Style)base.GetValue(ItemContainerStyleProperty);
            }

            set
            {
                base.SetValue(ItemContainerStyleProperty, (DependencyObject)value);
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
        /// Gets the value of the ItemContainerGenerator dependency
        /// property.
        /// </summary>
        public new ItemContainerGeneratorAdv ItemContainerGenerator
        {
            get;
            private set;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public HeatMapControl()
        {
            this.DefaultStyleKey = typeof(HeatMapControl);
            this.ColorWeightsInfo = new ColorWeightsInfo();
            ItemContainerGenerator = new ItemContainerGeneratorAdv(this);
        }

        static HeatMapControl()
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
               // Syncfusion.Windows.Shared.LoadDependentAssemblies load = new Syncfusion.Windows.Shared.LoadDependentAssemblies();
               // load = null;
            }

        }

        /// <summary>
        /// Called when an internal process or application calls
        /// ApplyTemplate, which is used to build the current template's
        /// visual tree. 
        /// </summary>
        public override void OnApplyTemplate()
        {
            ItemContainerGenerator.ClearItems();
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Method returns the container
        /// </summary>
        /// <returns>Type : HeatMapItem</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            HeatMapItem hmi = new HeatMapItem();
            this.Visibility = Visibility.Visible;
            hmi.ColorWeightValuePath = this.ColorWeightValuePath;
            hmi.WeightValuePath = this.WeightValuePath;
            if (this.ItemContainerStyle != null)
            {
                hmi.Style = this.ItemContainerStyle;
            }

            return hmi;
        }

        /// <summary>
        /// Prepares the element that is used to display the given item.
        /// </summary>
        /// <param name="element">element is used to convert into HeatMapItem and passed into PrepareContainerForItemOverride function.</param>
        /// <param name="item">item is used to pass into PrepareContainerForItemOverride function.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            HeatMapItem item2 = (HeatMapItem)element;
            if ((this.ItemContainerStyle != null) && (item2.Style == null))
            {
                item2.Style = this.ItemContainerStyle;
            }

            
            ItemContainerGenerator.ApplyPropertiesTochild(element, item, this.ItemContainerStyle);
            base.PrepareContainerForItemOverride(element, item);
        }
                                         
        /// <summary>
        /// Method determines whether the item is its own container.
        /// </summary>
        /// <param name="item">item is used to convert as HeatMapItem</param>
        /// <returns>The element that is used to display the given item.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is HeatMapItem;
        }

        /// <summary>
        /// called when the items list is changed.
        /// </summary>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.ColorWeightsInfo.LowestValue = Double.PositiveInfinity;
            this.ColorWeightsInfo.HighestValue = 0;
            base.OnItemsChanged(e);
        }
    }
}