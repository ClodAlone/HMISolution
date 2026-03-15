// <copyright file="HeatMapControl.cs" company="Syncfusion">
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
    using System.Windows.Media;
    using System.Xml;

    /// <summary>
    /// Control that lays out bound child items in rectangles whose area is based on
    /// their "weight" and whose color is based on their "color weight". Can also be
    /// bound to hierarchical data.
    /// </summary>
    /// <example>
    /// <code language="XAML">
    ///  &lt;syncfusion:HeatMapControl Name="heatMap"
    ///                               LowestWeightColor="ForestGreen"
    ///                               MedianWeightColor="YellowGreen"
    ///                               HighestWeightColor="IndianRed" Margin="20"&gt;
    ///       &lt;!--Adding HeatMapItem--&gt;
    ///              &lt;syncfusion:HeatMapItem Weight="1000" ColorWeight="400"
    /// Level="0"&gt;
    ///                     &lt;syncfusion:HeatMapItem Weight="500" Header="US:New York"
    /// ColorWeight="200" Level="1"&gt;
    ///                          &lt;syncfusion:HeatMapItem Header="NY : Albany"
    /// Weight="50" ColorWeight="15" Level="2" /&gt;
    ///                          &lt;syncfusion:HeatMapItem Header="NY : Buffalo"
    /// Weight="60" ColorWeight="20" Level="2" /&gt;
    ///                          &lt;syncfusion:HeatMapItem Header="NY : Rochester"
    /// Weight="55" ColorWeight="12" Level="2" /&gt;
    ///                     &lt;/syncfusion:HeatMapItem&gt;
    ///                     &lt;syncfusion:HeatMapItem Weight="200" Header="US:North
    /// Carolina" ColorWeight="100" Level="1" /&gt;
    ///                     &lt;syncfusion:HeatMapItem Weight="100"
    /// Header="US:Louisiana" ColorWeight="90" Level="1" /&gt;
    ///                     &lt;syncfusion:HeatMapItem Weight="280" Header="US:Florida"
    /// ColorWeight="65" Level="1" /&gt;
    ///              &lt;/syncfusion:HeatMapItem&gt;
    ///              &lt;syncfusion:HeatMapItem Weight="800" Header="Canada"
    /// ColorWeight="600" Level="0" /&gt;
    ///              &lt;syncfusion:HeatMapItem Weight="400" Header="Mexico"  Level="0"
    /// ColorWeight="200" /&gt;
    ///              &lt;syncfusion:HeatMapItem Weight="300" Header="Brazil" Level="0" 
    /// ColorWeight="100" /&gt;
    ///         &lt;/syncfusion:HeatMapControl&gt;
    ///         </code>
    /// </example>
    /// <seealso cref="HeatMapItem"/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class HeatMapControl : ItemsControl
    {
            /// <summary>
        ///  Identifies the TextIntersectAction dependency property.
            /// </summary>
            public static readonly DependencyProperty TextIntersectActionProperty
       = DependencyProperty.Register("TextIntersectAction", typeof(TextIntersectActions), typeof(HeatMapControl), new FrameworkPropertyMetadata(TextIntersectActions.Shrink, FrameworkPropertyMetadataOptions.AffectsRender));
            /// <summary>
            /// Get and Set TextIntersectActionProperty
            /// </summary>
            public TextIntersectActions TextIntersectAction
        {
            get { return (TextIntersectActions)this.GetValue(HeatMapControl.TextIntersectActionProperty); }
            set { this.SetValue(HeatMapControl.TextIntersectActionProperty, value); }
        }
        
        /// <summary>
            ///  Identifies the IsGradientBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty IsGradientBrushProperty
           = DependencyProperty.Register("IsGradientBrush", typeof(bool), typeof(HeatMapControl), new FrameworkPropertyMetadata(false));
        /// <summary>
        /// Get and Set IsGradientBrushProperty
        /// </summary>
        public bool IsGradientBrush
        {
            get { return (bool)this.GetValue(HeatMapControl.IsGradientBrushProperty); }
            set { this.SetValue(HeatMapControl.IsGradientBrushProperty, value); }
        }

        /// <summary>
        ///  Identifies the LowestWeightGradient dependency property.
        /// </summary>
        public static readonly DependencyProperty LowestWeightGradientProperty
           = DependencyProperty.Register("LowestWeightGradient", typeof(Brush), typeof(HeatMapControl), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));
        /// <summary>
        /// Get and Set LowestWeightGradientProperty
        /// </summary>
        public Brush LowestWeightGradient
        {
            get { return (Brush)this.GetValue(HeatMapControl.LowestWeightGradientProperty); }
            set { this.SetValue(HeatMapControl.LowestWeightGradientProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="MedianWeightColor"/> property.
        /// </summary>
        public static readonly DependencyProperty MedianWeightGradientProperty
            = DependencyProperty.Register("MedianWeightGradient", typeof(Brush), typeof(HeatMapControl), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));
        /// <summary>
        /// Get and Set MedianWeightGradientProperty
        /// </summary>
        public Brush MedianWeightGradient
        {
            get { return (Brush)this.GetValue(HeatMapControl.MedianWeightGradientProperty); }
            set { this.SetValue(HeatMapControl.MedianWeightGradientProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="HighestWeightColor"/> property.
        /// </summary>
        public static readonly DependencyProperty HighestWeightGradientProperty
            = DependencyProperty.Register("HighestWeightGradient", typeof(Brush), typeof(HeatMapControl), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));
        /// <summary>
        /// Get and Set HighestWeightGradientProperty
        /// </summary>
        public Brush HighestWeightGradient
        {
            get { return (Brush)this.GetValue(HeatMapControl.HighestWeightGradientProperty); }
            set { this.SetValue(HeatMapControl.HighestWeightGradientProperty, value); }
        }

        /// <summary>
        ///  Identifies the LabelFontSize dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelFontSizeProperty
       = DependencyProperty.Register("LabelFontSize", typeof(double), typeof(HeatMapControl), new FrameworkPropertyMetadata(12d, FrameworkPropertyMetadataOptions.AffectsRender));
        /// <summary>
        /// Get and Set LabelFontSizeProperty
        /// </summary>
        public double LabelFontSize
        {
            get { return (double)this.GetValue(HeatMapControl.LabelFontSizeProperty); }
            set { this.SetValue(HeatMapControl.LabelFontSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="WeightValuePath"/> property.
        /// </summary>
        public static readonly DependencyProperty WeightValuePathProperty
            = DependencyProperty.Register("WeightValuePath", typeof(string), typeof(HeatMapControl), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the <see cref="ColorValuePath"/> property.
        /// </summary>
        public static readonly DependencyProperty ColorValuePathProperty
            = DependencyProperty.Register("ColorValuePath", typeof(string), typeof(HeatMapControl), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.None));

        /// <summary>
        /// Identifies the <see cref="PreferredItemsPanelWidth"/> property.
        /// </summary>
        public static readonly DependencyProperty PreferredItemsPanelWidthProperty
            = DependencyProperty.Register("PreferredItemsPanelWidth", typeof(double), typeof(HeatMapControl), new FrameworkPropertyMetadata((double)300, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the <see cref="PreferredItemsPanelHeight"/> property.
        /// </summary>
        public static readonly DependencyProperty PreferredItemsPanelHeightProperty
            = DependencyProperty.Register("PreferredItemsPanelHeight", typeof(double), typeof(HeatMapControl), new FrameworkPropertyMetadata((double)300, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Identifies the <see cref="LowestWeightColor"/> property.
        /// </summary>
        public static readonly DependencyProperty LowestWeightColorProperty
            = DependencyProperty.Register("LowestWeightColor", typeof(Color), typeof(HeatMapControl), new FrameworkPropertyMetadata(Colors.Cornsilk, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="MedianWeightColor"/> property.
        /// </summary>
        public static readonly DependencyProperty MedianWeightColorProperty
            = DependencyProperty.Register("MedianWeightColor", typeof(Color), typeof(HeatMapControl), new FrameworkPropertyMetadata(Colors.Yellow, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies the <see cref="HighestWeightColor"/> property.
        /// </summary>
        public static readonly DependencyProperty HighestWeightColorProperty
            = DependencyProperty.Register("HighestWeightColor", typeof(Color), typeof(HeatMapControl), new FrameworkPropertyMetadata(Colors.Green, FrameworkPropertyMetadataOptions.AffectsRender));


        /// <summary>
        /// Identifies the <see cref="MedianWeight"/> property.
        /// </summary>
        public static readonly DependencyProperty MedianWeightProperty
            = DependencyProperty.Register("MedianWeight", typeof(int), typeof(HeatMapControl), new FrameworkPropertyMetadata(50, FrameworkPropertyMetadataOptions.AffectsRender, HeatMapControl.OnMedianWeightChanged));

        /// <summary>
        /// Identifies the <see cref="ColorCalculationLevel"/> property.
        /// </summary>
        public static readonly DependencyProperty ColorCalculationLevelProperty
            = DependencyProperty.Register("ColorCalculationLevel", typeof(int), typeof(HeatMapControl), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, HeatMapControl.OnColorCalculationLevelChanged));

        /// <summary>
        /// Identifies the <see cref="ItemsLayoutMode"/> property.
        /// </summary>
        public static DependencyProperty ItemsLayoutModeProperty
          = DependencyProperty.Register("ItemsLayoutMode", typeof(HeatMapLayoutMode), typeof(HeatMapControl), new FrameworkPropertyMetadata(HeatMapLayoutMode.Squarified, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Initializes static members of the <see cref="T:Syncfusion.Windows.Chart.HeatMapControl">HeatMapControl</see> class. 
        /// </summary>
        static HeatMapControl()
        {
            Syncfusion.Licensing.EnvironmentTest.ValidateLicense(typeof(HeatMapControl));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HeatMapControl), new FrameworkPropertyMetadata(typeof(HeatMapControl)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Chart.HeatMapControl">HeatMapControl</see> class. 
        /// </summary>
        public HeatMapControl()
        {
		    if (IsSecurityGranted)
            {
                ValidateLicense();
            }
            this.ColorWeightsInfo = new ColorWeightsInfo();
        }
		
		        /// <summary>
        /// Checks whether security permission can be granted. Read-only.
        /// </summary>
        internal static bool IsSecurityGranted
        {
            get
            {
                SecurityPermission perm = new SecurityPermission(PermissionState.Unrestricted);
                bool bResult = false;
                try
                {
                    perm.Demand();
                    bResult = true;
                }
                catch (Exception) { }
                return bResult;
            }
        }

        /// <summary>
        /// Checks whether license is valid.
        /// </summary>
        internal static void ValidateLicense()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
#if AllowUnsafeCode
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(HeatMapControl));
#endif
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
        }

        #region Properties
        /// <summary>
        /// Gets or sets a path to a value on the source object to serve as the "weight" of the object. 
        /// This is a dependency property. This is used for items at all levels. 
        /// Can be overriden for items at specific levels through the corresponding HeatMapItem's setting.
        /// </summary>
        public string WeightValuePath
        {
            get { return (string)this.GetValue(HeatMapControl.WeightValuePathProperty); }
            set { this.SetValue(HeatMapControl.WeightValuePathProperty, value); }
        }

        /// <summary>
        /// Gets or sets a path to a value on the source object to serve as the "color weight" of the object. 
        /// This is a dependency property. This is used for items at all levels. 
        /// Can be overriden for items at specific levels through the corresponding HeatMapItem's setting.
        /// </summary>
        public string ColorValuePath
        {
            get { return (string)this.GetValue(HeatMapControl.ColorValuePathProperty); }
            set { this.SetValue(HeatMapControl.ColorValuePathProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the preferred width you want to use for groups when bound to a grouped CollectionViewSource.
        /// Default is 300. This is a dependency property.
        /// </summary>
        /// <remarks>Normally, this control simply divides the available space between all the items. But, when bound
        /// to a CollectionViewSource with groups, the ItemsControl implementation requires you to specify
        /// a width and height for displaying a group. This property is used for that.</remarks>
        public double PreferredItemsPanelWidth
        {
            get { return (double)this.GetValue(HeatMapControl.PreferredItemsPanelWidthProperty); }
            set { this.SetValue(HeatMapControl.PreferredItemsPanelWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the preferred height you want to use for groups when bound to a grouped CollectionViewSource.
        /// Default is 300. This is a dependency property.
        /// </summary>
        /// <remarks>Normally, this control simply divides the available space between all the items. But, when bound
        /// to a CollectionViewSource with groups, the ItemsControl implementation requires you to specify
        /// a width and height for displaying a group. This property is used for that.</remarks>
        public double PreferredItemsPanelHeight
        {
            get { return (double)this.GetValue(HeatMapControl.PreferredItemsPanelHeightProperty); }
            set { this.SetValue(HeatMapControl.PreferredItemsPanelHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the color that will be used on the item with the lowest color weight. This is a dependency property.
        /// Default is Cornsilk.
        /// </summary>
        public Color LowestWeightColor
        {
            get { return (Color)this.GetValue(HeatMapControl.LowestWeightColorProperty); }
            set { this.SetValue(HeatMapControl.LowestWeightColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the color that will be used on the item with the highest color weight. This is a dependency property.
        /// Default is Green.
        /// </summary>
        public Color HighestWeightColor
        {
            get { return (Color)this.GetValue(HeatMapControl.HighestWeightColorProperty); }
            set { this.SetValue(HeatMapControl.HighestWeightColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the color that will be used on the item with the median color weight (see <see cref="MedianWeight"/>. 
        /// This is a dependency property. Default is Yellow.
        /// </summary>
        public Color MedianWeightColor
        {
            get { return (Color)this.GetValue(HeatMapControl.MedianWeightColorProperty); }
            set { this.SetValue(HeatMapControl.MedianWeightColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the items at a level (when bound to hierarchical data) for which the ColorWeight should be processed.
        /// Default is -1, indicating this will be processed for all leaf nodes in the hierarchy. 0 indicates the top level of items
        /// in the bound hierarchy and so on. If adding HeatMapItems manually, make sure to set their Level property appropriately.
        /// This is a dependency property.
        /// </summary>
        public int ColorCalculationLevel
        {
            get { return (int)this.GetValue(HeatMapControl.ColorCalculationLevelProperty); }
            set { this.SetValue(HeatMapControl.ColorCalculationLevelProperty, value); }
        }

        #region Protected Methods
        /// <summary>
        /// Get Container For Item Override
        /// </summary>
        /// <returns>The HeatMapItem</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new HeatMapItem(0);
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

        /// <summary>
        /// Called when OnItemsSourceChanged
        /// </summary>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            this.ColorWeightsInfo.LowestValue = Double.PositiveInfinity;
            this.ColorWeightsInfo.HighestValue = 0;
            base.OnItemsSourceChanged(oldValue, newValue);
        }

        #endregion

        /// <summary>
        /// The OnMedianWeightChanged method
        /// </summary>
        /// <param name="d">The DependencyObject d</param>
        /// <param name="args">The DependencyPropertyChangedEventArgs args</param>
        private static void OnMedianWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            int newvalue = (int)args.NewValue;
            if (newvalue < 0 || newvalue > 100)
            {
                throw new ArgumentOutOfRangeException("HeatMapConrol.MedianWeight should be within the range 0 to 100. Encountered values is: " + newvalue.ToString());
            }
        }

        /// <summary>
        /// The OnColorCalculationLevelChanged method
        /// </summary>
        /// <param name="d">The DependencyObject d</param>
        /// <param name="args">The DependencyPropertyChangedEventArgs args</param>
        private static void OnColorCalculationLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            HeatMapControl control = (HeatMapControl)d;
            if (control.IsVisible)
            {
                control.ColorWeightsInfo.LowestValue = Double.PositiveInfinity;
                control.ColorWeightsInfo.HighestValue = 0;
                control.ClearColorAvailability(control.Items);
                control.InvalidateVisual();
                control.InvalidateMeasure();
                control.InvalidateArrange();
                control.UpdateLayout();
            }
        }

        /// <summary>
        /// The ClearColorAvailability method
        /// </summary>
        /// <param name="items">The ItemCollection items</param>
        /// <remarks></remarks>
        /// <seealso cref="HeatMapControl"/>
        private void ClearColorAvailability(ItemCollection items)
        {
            foreach (HeatMapItem item in items)
            {
                item.IsColorInfoAvailable = false;
                this.ClearColorAvailability(item.Items);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the "median color weight" on which the <see cref="MedianWeightColor"/> will be applied. The
        /// valid values for this property are between 0 to 100. Default is 50. This is a dependency property.
        /// </summary>
        public int MedianWeight
        {
            get { return (int)this.GetValue(HeatMapControl.MedianWeightProperty); }
            set { this.SetValue(HeatMapControl.MedianWeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the mode in which items should be laid out. This setting will be applied at all levels. To customize this
        /// for specific level, check the corresponding HeatMapItem setting. Default value is HeatMapLayoutMode.Squarified.
        /// This is a dependency property.
        /// </summary>
        public HeatMapLayoutMode ItemsLayoutMode
        {
            get { return (HeatMapLayoutMode)this.GetValue(HeatMapControl.ItemsLayoutModeProperty); }
            set { this.SetValue(HeatMapControl.ItemsLayoutModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating the computed information about the low and high colors in the bound items.
        /// </summary>
        public ColorWeightsInfo ColorWeightsInfo
        {
            get;
            set;
        }
        #endregion
    }

   
}
