#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
#if WINRT
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
#if WPF
using System.Data;
#endif
#endif

namespace Syncfusion.UI.Xaml.TreeMap
{
    public class SfTreeMap : Control, IDisposable
    {
        #region Constructor

        public SfTreeMap()
        {
            DefaultStyleKey = typeof(SfTreeMap);
            LeafColorMapping = new UniColorMapping();
            SizeChanged += SfTreeMap_SizeChanged;
            Loaded += SfTreeMap_Loaded;
            Levels = new ObservableCollection<TreeMapLevel>();
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
        }

        #endregion

        #region Dependency Properties

        #region ItemsLayoutMode
        public TreeMapLayoutMode ItemsLayoutMode
        {
            get { return (TreeMapLayoutMode)GetValue(ItemsLayoutModeProperty); }
            set { SetValue(ItemsLayoutModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsLayoutMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsLayoutModeProperty =
            DependencyProperty.Register("ItemsLayoutMode", typeof(TreeMapLayoutMode), typeof(SfTreeMap), new PropertyMetadata(TreeMapLayoutMode.Squarified, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfTreeMap)
            {
                (d as SfTreeMap).UpdateTreeMapItems();
            }
        }
        #endregion

        #region Levels
        public ObservableCollection<TreeMapLevel> Levels
        {
            get { return (ObservableCollection<TreeMapLevel>)GetValue(LevelsProperty); }
            set { SetValue(LevelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Levels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LevelsProperty =
            DependencyProperty.Register("Levels", typeof(ObservableCollection<TreeMapLevel>), typeof(SfTreeMap), new PropertyMetadata(null));
        #endregion

        #region LeafColorMapping
        public ColorMapping LeafColorMapping
        {
            get { return (ColorMapping)GetValue(LeafColorMappingProperty); }
            set { SetValue(LeafColorMappingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeafColorMapping.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeafColorMappingProperty =
            DependencyProperty.Register("LeafColorMapping", typeof(ColorMapping), typeof(SfTreeMap), new PropertyMetadata(null, OnLeafColorMappingChanged));

        private static void OnLeafColorMappingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfTreeMap)
            {
                SfTreeMap sfTreeMap = d as SfTreeMap;
                if (sfTreeMap.LeafNodes != null && sfTreeMap.LeafNodes.Count > 0)
                {
                    sfTreeMap.LeafColorMapping.EvaluateColorMapping(sfTreeMap.LeafNodes);
                }
            }
        }
        #endregion

        #region WeightValuePath
        public string WeightValuePath
        {
            get { return (string)GetValue(WeightValuePathProperty); }
            set { SetValue(WeightValuePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WeightValuePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WeightValuePathProperty =
            DependencyProperty.Register("WeightValuePath", typeof(string), typeof(SfTreeMap), new PropertyMetadata(null, OnPropertyChanged));
        #endregion

        #region ColorValuePath
        public string ColorValuePath
        {
            get { return (string)GetValue(ColorValuePathProperty); }
            set { SetValue(ColorValuePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorValuePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorValuePathProperty =
            DependencyProperty.Register("ColorValuePath", typeof(string), typeof(SfTreeMap), new PropertyMetadata(null, OnPropertyChanged));
        #endregion

        #region LeafLabelPath
        public string LeafLabelPath
        {
            get { return (string)GetValue(LeafLabelPathProperty); }
            set { SetValue(LeafLabelPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeafLabelPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeafLabelPathProperty =
            DependencyProperty.Register("LeafLabelPath", typeof(string), typeof(SfTreeMap), new PropertyMetadata(null, OnPropertyChanged));
        #endregion

        #region BorderBrush
        public new Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderBrush.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(SfTreeMap), new PropertyMetadata(new SolidColorBrush(Colors.White)));
        #endregion

        #region BorderThickness
        public new double BorderThickness
        {
            get { return (double)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BorderThickness.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register("BorderThickness", typeof(double), typeof(SfTreeMap), new PropertyMetadata(2d));
        #endregion

        #region ItemsSource
        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(SfTreeMap), new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfTreeMap)
            {
                SfTreeMap treeMap = d as SfTreeMap;

                if (treeMap.ItemsSource != null)
                {
                    if (treeMap.ItemsSource is IEnumerable)
                    {
                        var itemsSource = treeMap.ItemsSource as IEnumerable;

                        if (itemsSource is INotifyCollectionChanged)
                            (itemsSource as INotifyCollectionChanged).CollectionChanged += treeMap.SfTreeMap_CollectionChanged;

                        foreach (object item in itemsSource)
                        {
                            var propertyChanged = item as INotifyPropertyChanged;
                            if (propertyChanged != null)
                            {
                                propertyChanged.PropertyChanged += treeMap.SfTreeMap_PropertyChanged;
                            }
                        }
                    }
#if WPF
                    else if (treeMap.ItemsSource is DataTable)
                    {
                        var itemsSource = treeMap.ItemsSource as DataTable;
                        treeMap.UpdateTreeMapItems();
                    }
#endif
                    OnPropertyChanged(d, e);
                }
            }
        }

        #endregion

        #region LeafTemplate
        public DataTemplate LeafTemplate
        {
            get { return (DataTemplate)GetValue(LeafTemplateProperty); }
            set { SetValue(LeafTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeafTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeafTemplateProperty =
            DependencyProperty.Register("LeafTemplate", typeof(DataTemplate), typeof(SfTreeMap), new PropertyMetadata(null, OnPropertyChanged));
        #endregion

        #region LeafNodes
        public List<TreeMapLeafNode> LeafNodes
        {
            get { return (List<TreeMapLeafNode>)GetValue(LeafNodesProperty); }
            internal set { SetValue(LeafNodesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeafNodes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeafNodesProperty =
            DependencyProperty.Register("LeafNodes", typeof(List<TreeMapLeafNode>), typeof(SfTreeMap), new PropertyMetadata(null));
        #endregion

        #region ShowToolTip
        public bool ShowToolTip
        {
            get { return (bool)GetValue(ShowToolTipProperty); }
            set { SetValue(ShowToolTipProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowToolTipProperty =
            DependencyProperty.Register("ShowToolTip", typeof(bool), typeof(SfTreeMap), new PropertyMetadata(false));
        #endregion

        #region ToolTipTemplate
        public DataTemplate ToolTipTemplate
        {
            get { return (DataTemplate)GetValue(ToolTipTemplateProperty); }
            set { SetValue(ToolTipTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ToolTipTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolTipTemplateProperty =
            DependencyProperty.Register("ToolTipTemplate", typeof(DataTemplate), typeof(SfTreeMap), new PropertyMetadata(null));
        #endregion

        #region HighlightBorderBrush
        public Brush HighlightBorderBrush
        {
            get { return (Brush)GetValue(HighlightBorderBrushProperty); }
            set { SetValue(HighlightBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightBorderBrushProperty =
            DependencyProperty.Register("HighlightBorderBrush", typeof(Brush), typeof(SfTreeMap), new PropertyMetadata(new SolidColorBrush(Colors.Yellow)));
        #endregion

        #region HighlightBorderThickness
        public double HighlightBorderThickness
        {
            get { return (double)GetValue(HighlightBorderThicknessProperty); }
            set { SetValue(HighlightBorderThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightBorderThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightBorderThicknessProperty =
            DependencyProperty.Register("HighlightBorderThickness", typeof(double), typeof(SfTreeMap), new PropertyMetadata(2d));
        #endregion

        #region HighlightOnSelection
        public bool HighlightOnSelection
        {
            get { return (bool)GetValue(HighlightOnSelectionProperty); }
            set { SetValue(HighlightOnSelectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightOnSelection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightOnSelectionProperty =
            DependencyProperty.Register("HighlightOnSelection", typeof(bool), typeof(SfTreeMap), new PropertyMetadata(false));
        #endregion

        #region Legend
        public TreeMapLegend Legend
        {
            get { return (TreeMapLegend)GetValue(LegendProperty); }
            set { SetValue(LegendProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Legend.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendProperty =
            DependencyProperty.Register("Legend", typeof(TreeMapLegend), typeof(SfTreeMap), new PropertyMetadata(null, OnLegendChanged));

        private static void OnLegendChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SfTreeMap)
            {
                var treeMapLegend = e.NewValue as TreeMapLegend;
                if (treeMapLegend != null)
                {
                    SfTreeMap treeMap = (d as SfTreeMap);
                    treeMapLegend.TreeMap = treeMap;
                    treeMap.UpdateTreeMapItems();
                }
            }
        }
        #endregion

        #region SelectedItem
        public TreeMapLeafNode SelectedItem
        {
            get { return (TreeMapLeafNode)GetValue(SelectedItemProperty); }
            internal set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(TreeMapLeafNode), typeof(SfTreeMap), new PropertyMetadata(null));
        #endregion

#if !WINDOWS_PHONE

        #region EnableDrillDown
        /// <summary>
        /// Gets or sets a value to indicate whether the drill down feature should be enabled.
        /// </summary>
        public bool EnableDrillDown
        {
            get { return (bool)GetValue(EnableDrillDownProperty); }
            set { SetValue(EnableDrillDownProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableDrillDown.  This enables animation, styling, binding, etc...
        /// </summary>

        public static readonly DependencyProperty EnableDrillDownProperty =
            DependencyProperty.Register("EnableDrillDown", typeof(bool), typeof(SfTreeMap), new PropertyMetadata(false, OnPropertyChanged));
        #endregion

        #region DrillDownHeaderHeight
        public double DrillDownHeaderHeight
        {
            get { return (double)GetValue(DrillDownHeaderHeightProperty); }
            set { SetValue(DrillDownHeaderHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrillDownHeaderHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrillDownHeaderHeightProperty =
            DependencyProperty.Register("DrillDownHeaderHeight", typeof(double), typeof(SfTreeMap), new PropertyMetadata(40d));
        #endregion

        #region DrillDownHeaderTemplate
        public DataTemplate DrillDownHeaderTemplate
        {
            get { return (DataTemplate)GetValue(DrillDownHeaderTemplateProperty); }
            set { SetValue(DrillDownHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrillDownHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrillDownHeaderTemplateProperty =
            DependencyProperty.Register("DrillDownHeaderTemplate", typeof(DataTemplate), typeof(SfTreeMap), new PropertyMetadata(null));
        #endregion

        #region DrillDownSelectionStroke
        public Brush DrillDownSelectionStroke
        {
            get { return (Brush)GetValue(DrillDownSelectionStrokeProperty); }
            set { SetValue(DrillDownSelectionStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrillDownSelectionStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrillDownSelectionStrokeProperty =
            DependencyProperty.Register("DrillDownSelectionStroke", typeof(Brush), typeof(SfTreeMap), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 00, 00, 00))));
        #endregion

        #region DrillDownHeader
        internal string DrillDownHeader
        {
            get { return (string)GetValue(DrillDownHeaderProperty); }
            set { SetValue(DrillDownHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DrillDownHeader.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DrillDownHeaderProperty =
            DependencyProperty.Register("DrillDownHeader", typeof(string), typeof(SfTreeMap), new PropertyMetadata(string.Empty));
        #endregion

        #region CustomToolTipTemplate
        internal ControlTemplate CustomToolTipTemplate
        {
            get { return (ControlTemplate)GetValue(CustomToolTipTemplateProperty); }
            set { SetValue(CustomToolTipTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomToolTipTemplate.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CustomToolTipTemplateProperty =
            DependencyProperty.Register("CustomToolTipTemplate", typeof(ControlTemplate), typeof(SfTreeMap), new PropertyMetadata(null));
        #endregion

#endif

        #endregion

        #region Private Members

        Size availableSize;
        TreeMapEngine engine;
        List<TreeMapLeafNode> wholeItems = new List<TreeMapLeafNode>();
        Grid treeMapGrid;
        ResourceDictionary resourceDictionary;

        #endregion

        #region Internal Members

#if !WPF
#if WINRT || SILVERLIGHT && !WINDOWS_PHONE
        internal Point MousePosition;
#endif
        internal Popup treeMapPopup;
#else
        internal ToolTip treeMapTooltip;
#endif
        internal bool isLoaded;
        internal TreeMapItem RootTreeMapItem;
#if !WINDOWS_PHONE
        internal Size rootTreeMapItemSize;
        internal ContentPresenter drillDownHeaderPresenter;
        internal bool? isDrilledIn;
        internal TreeMapItem drilledTreeMapItem;
        internal bool isResized;
#endif

        #endregion

        #region Override Methods

#if WINRT
        protected override void OnApplyTemplate()
        {
#else
        public override void OnApplyTemplate()
        {
#endif
            treeMapGrid = GetTemplateChild("PART_TreeMap") as Grid;
            base.OnApplyTemplate();
        }


        #endregion

        #region Implementation

        private void GenerateTreeMapItems()
        {
            RootTreeMapItem = new TreeMapItem { isRootTreeMapItem = true, TreeMap = this, GroupingLevel = Levels[0] };
            wholeItems = new List<TreeMapLeafNode>();
            var valueField = new TreeMapValueField
            {
                Name = WeightValuePath,
                FieldName = WeightValuePath
            };

            var colorField = new TreeMapValueField
            {
                Name = ColorValuePath,
                FieldName = ColorValuePath
            };

            engine = new TreeMapEngine
            {
                ValueField = valueField,
                ColorField = colorField,
                DataSource = ItemsSource
            };

            List<TreeMapItem> rootItems = null, subItems = null;
            int levelsCount = Levels.Count;

            #region Nested Data Collection TreeMapItems
            if (Levels[0].GetType() == typeof(TreeMapHierarchicalLevel))
            {
                for (int i = 0; i < levelsCount; i++)
                {
                    if (i == 0)
                    {
                        RootTreeMapItem.GroupingLevel = Levels[i];
                        int itemsCount;
                        if (Levels[i].GroupingPath == null && i == levelsCount - 1)
                        {
                            wholeItems = engine.GetTreeMapLeafNodes(WeightValuePath, ColorValuePath, LeafLabelPath);
                            RootTreeMapItem.LeafNodes = wholeItems;
                            if (rootItems != null)
                            {
                                itemsCount = rootItems.Count;
                                for (int j = itemsCount - 1; j >= 0; j--)
                                {
                                    rootItems[j].TreeMap = this;
                                    rootItems[j].HeaderSize = Levels[i].HeaderHeight;
                                    rootItems[j].HeaderTemplate = Levels[i].HeaderTemplate;
                                    rootItems[j].LabelTemplate = Levels[i].LabelTemplate;
                                }
                            }
                        }
                        else
                        {
                            rootItems = engine.GetTreeMapItems(WeightValuePath, ColorValuePath, Levels[i].LevelLabelPath, Levels[i].LevelHeaderPath);
                            RootTreeMapItem.ChildTreeMapItems = rootItems;
                            subItems = rootItems.ToList();
                            itemsCount = rootItems.Count;
                            for (int j = itemsCount - 1; j >= 0; j--)
                            {
                                rootItems[j].TreeMap = this;
                                rootItems[j].ParentTreeMapItem = RootTreeMapItem;
                            }
                        }
                    }
                    if (subItems != null)
                    {
                        var cloneItems = subItems.ToList();
                        subItems.Clear();
                        var levelEngine = new TreeMapEngine
                        {
                            ValueField = valueField,
                            ColorField = colorField,
                        };
                        if (i > 0 && Levels[i - 1].DataSource.Count > 0)
                        {
                            engine.DataSource = Levels[i - 1].DataSource;
                        }

                        int cloneItemsCount = cloneItems.Count;
                        for (int j = 0; j < cloneItemsCount; j++)
                        {
                            object levelItemsSource = null;
#if WPF
                            if (cloneItems[j].Data is DataRowView)
                                levelItemsSource = engine.GetValue(Levels[i].GroupingPath, (cloneItems[j].Data as DataRowView).Row);
                            else
#endif
                                levelItemsSource = engine.GetValue(Levels[i].GroupingPath, cloneItems[j].Data);
                            levelEngine.DataSource = levelItemsSource;
                            int levelItemsSourceCount = 0;
                            if (levelEngine.DataSourceList != null)
                            {
                                IEnumerator iterator = levelEngine.DataSourceList.GetEnumerator();
                                while (iterator.MoveNext())
                                {
                                    Levels[i].DataSource.Add(iterator.Current);
                                    levelItemsSourceCount++;
                                }
                            }
                            if (i != levelsCount - 1)
                            {
                                var treeItems = levelEngine.GetTreeMapItems(WeightValuePath, ColorValuePath, Levels[i + 1].LevelLabelPath, Levels[i + 1].LevelHeaderPath);
                                int treeItemsCount = treeItems.Count;
                                cloneItems[j].ChildTreeMapItems = treeItems;
                                cloneItems[j].GroupingLevel = Levels[i + 1];
                                cloneItems[j].HeaderSize = Levels[i].HeaderHeight;
                                cloneItems[j].HeaderTemplate = Levels[i].HeaderTemplate;
                                cloneItems[j].LabelTemplate = Levels[i].LabelTemplate;
                                for (int k = treeItemsCount - 1; k >= 0; k--)
                                {
                                    treeItems[k].TreeMap = this;
                                    treeItems[k].ParentTreeMapItem = cloneItems[j];
                                }
                                subItems.AddRange(treeItems);
                            }
                            else
                            {
                                List<TreeMapLeafNode> leafNodes;
                                if (levelItemsSourceCount == 0)
                                {
                                    levelEngine.DataSource = new ObservableCollection<object> { cloneItems[j].Data };
                                    leafNodes = levelEngine.GetTreeMapLeafNodes(WeightValuePath, ColorValuePath, LeafLabelPath);
                                }
                                else
                                    leafNodes = levelEngine.GetTreeMapLeafNodes(WeightValuePath, ColorValuePath, LeafLabelPath);
                                cloneItems[j].LeafNodes = leafNodes;
                                cloneItems[j].HeaderSize = Levels[i].HeaderHeight;
                                cloneItems[j].HeaderTemplate = Levels[i].HeaderTemplate;
                                cloneItems[j].LabelTemplate = Levels[i].LabelTemplate;
                                wholeItems.AddRange(leafNodes);
                            }
                        }
                        Levels[i].TreeMapItems = cloneItems;
                    }
                }
                LeafColorMapping.EvaluateColorMapping(wholeItems);
            }
            #endregion

            #region Normal Data Collection TreeMapItems
            else
            {
                #region Grouping tree map items
                for (int i = 0; i < levelsCount; i++)
                {
                    if (Levels[i].GroupingPath != null)
                    {
                        if (subItems != null)
                        {
                            var cloneItems = subItems.ToList();
                            subItems.Clear();

                            int cloneItemsCount = cloneItems.Count;
                            for (int j = 0; j < cloneItemsCount; j++)
                            {
                                engine = new TreeMapEngine
                                {
                                    ValueField = valueField,
                                    ColorField = colorField,
                                    DataSource = cloneItems[j].SubItemsList
                                };
                                var treeItems = engine.GetGroupItem(Levels[i].GroupingPath);
                                cloneItems[j].ChildTreeMapItems = treeItems;
                                cloneItems[j].GroupingLevel = Levels[i];
                                int treeItemsCount = treeItems.Count;
                                for (int k = treeItemsCount - 1; k >= 0; k--)
                                {
                                    treeItems[k].TreeMap = this;
                                    treeItems[k].ParentTreeMapItem = cloneItems[j];
                                    treeItems[k].HeaderSize = Levels[i].HeaderHeight;
                                    treeItems[k].HeaderTemplate = Levels[i].HeaderTemplate;
                                    treeItems[k].LabelTemplate = Levels[i].LabelTemplate;
                                }
                                subItems.AddRange(treeItems);
                            }
                            Levels[i].TreeMapItems = subItems.ToList();
                        }
                        else
                        {
                            rootItems = engine.GetGroupItem(Levels[i].GroupingPath);
                            RootTreeMapItem.GroupingLevel = Levels[i];
                            int rootItemsCount = rootItems.Count;
                            for (int j = rootItemsCount - 1; j >= 0; j--)
                            {
                                rootItems[j].TreeMap = this;
                                rootItems[j].ParentTreeMapItem = RootTreeMapItem;
                                rootItems[j].HeaderSize = Levels[i].HeaderHeight;
                                rootItems[j].HeaderTemplate = Levels[i].HeaderTemplate;
                                rootItems[j].LabelTemplate = Levels[i].LabelTemplate;
                            }
                            subItems = rootItems.ToList();
                            Levels[i].TreeMapItems = rootItems;
                        }
                    }
                }
                #endregion

                if (subItems != null)
                {
                    RootTreeMapItem.ChildTreeMapItems = rootItems;
                    int subItemsCount = subItems.Count;
                    for (int i = 0; i < subItemsCount; i++)
                    {
                        engine = new TreeMapEngine
                        {
                            ValueField = valueField,
                            ColorField = colorField,
                            DataSource = subItems[i].SubItemsList,
                        };
                        var leafNodes = engine.GetTreeMapLeafNodes(WeightValuePath, ColorValuePath, LeafLabelPath);
                        subItems[i].LeafNodes = leafNodes;
                        subItems[i].GroupingLevel = Levels[levelsCount - 1];
                        wholeItems.AddRange(leafNodes);
                    }
                    LeafColorMapping.EvaluateColorMapping(wholeItems);
                }
                else
                {
                    wholeItems = engine.GetTreeMapLeafNodes(WeightValuePath, ColorValuePath, LeafLabelPath);
                    LeafColorMapping.EvaluateColorMapping(wholeItems);
                    RootTreeMapItem.LeafNodes = wholeItems;
                }
            }
            #endregion

            LeafNodes = wholeItems;

            #region DrillDownHeader & Legend

#if !WINDOWS_PHONE
            CreateDrillDownHeader();
#endif
            CreateLegend();

            #endregion

            #region ToolTip

#if !WPF
            DataTemplate toolTipTemplate = ToolTipTemplate != null ? ToolTipTemplate : (resourceDictionary["ToolTipTemplate"] as DataTemplate);
            treeMapPopup = new Popup
            {
                Child = new ContentControl { Content = toolTipTemplate.LoadContent(), IsHitTestVisible = false },
                IsHitTestVisible = false
            };
            Binding toolTipVisibilityBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath("ShowToolTip"),
                Converter = new BoolToVisibilityConverter(),
            };
            treeMapPopup.SetBinding(Popup.VisibilityProperty, toolTipVisibilityBinding);
            treeMapGrid.Children.Add(treeMapPopup);
#endif

            #endregion
        }

#if !WINDOWS_PHONE
        private void CreateDrillDownHeader()
        {
            if (EnableDrillDown && RootTreeMapItem.ChildTreeMapItems != null)
            {
                drillDownHeaderPresenter = new ContentPresenter
                {
                    Width = availableSize.Width,
                    VerticalAlignment = VerticalAlignment.Top,
                };
                Binding heightBinding = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("DrillDownHeaderHeight")
                };
                drillDownHeaderPresenter.SetBinding(ContentPresenter.HeightProperty, heightBinding);
                Binding visibilityBinding = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("EnableDrillDown"),
                    Converter = new BoolToVisibilityConverter()
                };
                drillDownHeaderPresenter.SetBinding(ContentPresenter.VisibilityProperty, visibilityBinding);
                Binding contentTemplateBinding = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("DrillDownHeaderTemplate")
                };
                drillDownHeaderPresenter.SetBinding(ContentPresenter.ContentTemplateProperty, contentTemplateBinding);
                Binding dataContextBinding = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("DrillDownHeader")
                };
                drillDownHeaderPresenter.SetBinding(ContentPresenter.ContentProperty, dataContextBinding);
#if WINRT
                drillDownHeaderPresenter.PointerPressed += drillDownHeaderPresenter_PointerPressed;
#elif WPF
                drillDownHeaderPresenter.MouseDown += drillDownHeaderPresenter_MouseDown;
#else
                drillDownHeaderPresenter.MouseLeftButtonDown += drillDownHeaderPresenter_MouseDown;
#endif

                if (Legend == null)
                {
                    treeMapGrid.RowDefinitions.Clear();
                    treeMapGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    treeMapGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                    Grid.SetRow(RootTreeMapItem, 1);
                }
                treeMapGrid.Children.Add(drillDownHeaderPresenter);
            }
        }
#endif

        internal void UpdateTreeMapItems()
        {
            if (isLoaded && treeMapGrid != null && ItemsSource != null && Levels != null && Levels.Count > 0 && availableSize != new Size())
            {
                treeMapGrid.Children.Clear();
                treeMapGrid.RowDefinitions.Clear();
                treeMapGrid.ColumnDefinitions.Clear();
#if !WINDOWS_PHONE
                if (EnableDrillDown && isDrilledIn != null && drilledTreeMapItem != null)
                {
                    isResized = true;
                    CreateDrillDownHeader();
                    CreateLegend();
                    drilledTreeMapItem.DrillDownAndDrillUpTreeMapItem(true);
                }
                else
#endif
                    GenerateTreeMapItems();
                treeMapGrid.Children.Add(RootTreeMapItem);
            }
        }

        private void CreateLegend()
        {
            if (Legend != null)
            {
                if (wholeItems.Count > 0 && LeafColorMapping is RangeBrushColorMapping)
                {
                    var legendItems = new ObservableCollection<TreeMapLegendItem>();
                    var colorMapping = LeafColorMapping as RangeBrushColorMapping;
                    for (int i = colorMapping.Brushes.Count - 1; i >= 0; i--)
                    {
                        legendItems.Add(new TreeMapLegendItem
                        {
                            Legend = Legend,
                            Fill = new SolidColorBrush(colorMapping.Brushes[i].Color),
                            Label = colorMapping.Brushes[i].LegendLabel ?? colorMapping.Brushes[i].From.ToString(CultureInfo.InvariantCulture) + "-" + colorMapping.Brushes[i].To.ToString(CultureInfo.InvariantCulture),
                            IconTemplate = Legend.IconTemplate,
                            Icon = Legend.IconTemplate != null ? Legend.IconTemplate.LoadContent() : null
                        });
                    }
                    Legend.ItemsSource = legendItems;
                }
                treeMapGrid.Children.Add(Legend);
                SetLegendPosition();
            }
        }

        internal void SetLegendPosition()
        {
            if (treeMapGrid == null || Legend == null)
                return;
            if (Legend.LegendPosition == TreeMapLegendPosition.Top || Legend.LegendPosition == TreeMapLegendPosition.Bottom)
            {
                treeMapGrid.RowDefinitions.Clear();
                treeMapGrid.ColumnDefinitions.Clear();
                treeMapGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) });

                if (Legend.LegendPosition == TreeMapLegendPosition.Top)
                {
                    treeMapGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
#if !WINDOWS_PHONE
                    treeMapGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
#endif
                    treeMapGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Star) });
                }
                else
                {
#if !WINDOWS_PHONE
                    treeMapGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
#endif
                    treeMapGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Star) });
                    treeMapGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                }
#if !WINDOWS_PHONE
                Grid.SetRow(Legend, Legend.LegendPosition == TreeMapLegendPosition.Top ? 0 : 2);
                Grid.SetRow(RootTreeMapItem, Legend.LegendPosition == TreeMapLegendPosition.Top ? 2 : 1);
                if (drillDownHeaderPresenter != null)
                    Grid.SetRow(drillDownHeaderPresenter, Legend.LegendPosition == TreeMapLegendPosition.Top ? 1 : 0);
#else
				Grid.SetRow(Legend, Legend.LegendPosition == TreeMapLegendPosition.Top ? 0 : 1);
                Grid.SetRow(RootTreeMapItem, Legend.LegendPosition == TreeMapLegendPosition.Top ? 1 : 0);
#endif
            }
            else
            {
                treeMapGrid.RowDefinitions.Clear();
                treeMapGrid.ColumnDefinitions.Clear();
#if !WINDOWS_PHONE
                treeMapGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
#endif
                treeMapGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Star) });

                if (Legend.LegendPosition == TreeMapLegendPosition.Left)
                {
                    treeMapGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    treeMapGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) });
                }
                else
                {
                    treeMapGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) });
                    treeMapGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                }
                Grid.SetColumn(Legend, Legend.LegendPosition == TreeMapLegendPosition.Left ? 0 : 1);
                Grid.SetColumn(RootTreeMapItem, Legend.LegendPosition == TreeMapLegendPosition.Left ? 1 : 0);
#if !WINDOWS_PHONE
                if (drillDownHeaderPresenter != null)
                {
                    Grid.SetColumn(drillDownHeaderPresenter, Legend.LegendPosition == TreeMapLegendPosition.Left ? 1 : 0);
                    Grid.SetRow(drillDownHeaderPresenter, 0);
                    Grid.SetRow(RootTreeMapItem, 1);
                    Grid.SetRowSpan(Legend, 2);
                }
#endif
            }

        }

        internal void UpdateLegendItems()
        {
            var legendItems = Legend.ItemsSource as ObservableCollection<TreeMapLegendItem>;
            if (legendItems != null)
            {
                int itemsCount = legendItems.Count;
                for (int i = itemsCount - 1; i >= 0; i--)
                {
                    legendItems[i].IconTemplate = Legend.IconTemplate;
                    legendItems[i].Icon = Legend.IconTemplate != null ? Legend.IconTemplate.LoadContent() : null;
                }
            }
        }

        #endregion

        #region Events

        private void SfTreeMap_Loaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;
#if !WINDOWS_PHONE
            if (EnableDrillDown && RootTreeMapItem != null && RootTreeMapItem.ChildTreeMapItems != null && Levels.Count > 0)
            {
                foreach (TreeMapItem treeMapItem in Levels[0].TreeMapItems)
                {
                    treeMapItem.CreateDrillDownOverlay(treeMapItem.TreeMapItemGrid);
                }
            }
#endif
        }

        private void SfTreeMap_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            availableSize = e.NewSize;
#if !WINDOWS_PHONE
            rootTreeMapItemSize = e.NewSize;
            if (ActualWidth != 0 && !Double.IsInfinity(ActualWidth) && ActualHeight != 0 && !Double.IsInfinity(ActualHeight))
                Clip = new RectangleGeometry { Rect = new Rect(0, 0, ActualWidth, ActualHeight) };
#endif
            treeMapGrid.Children.Clear();
#if WPF
            isLoaded = true;
#endif
            if (ItemsSource != null && Levels != null && Levels.Count > 0)
            {
#if WPF
                if ((ItemsSource is IEnumerable && (ItemsSource as IEnumerable).GetEnumerator().MoveNext()) ||
                    (ItemsSource is DataTable && (ItemsSource as DataTable).Rows.Count > 0))
#else
                if (ItemsSource is IEnumerable && (ItemsSource as IEnumerable).GetEnumerator().MoveNext())
#endif
                {
#if !WINDOWS_PHONE
                    if (EnableDrillDown && isDrilledIn != null && drilledTreeMapItem != null)
                    {
                        isResized = true;
                        CreateDrillDownHeader();
                        rootTreeMapItemSize.Height -= DrillDownHeaderHeight;
                        CreateLegend();
                        if (Legend != null)
                        {
                            if (Legend.LegendPosition == TreeMapLegendPosition.Left || Legend.LegendPosition == TreeMapLegendPosition.Right)
                                rootTreeMapItemSize.Width -= Legend.ActualWidth;
                            if (Legend.LegendPosition == TreeMapLegendPosition.Top || Legend.LegendPosition == TreeMapLegendPosition.Bottom)
                                rootTreeMapItemSize.Height -= Legend.ActualHeight;
                        }
                        drilledTreeMapItem.DrillDownAndDrillUpTreeMapItem(true);
                        if (RootTreeMapItem != null && RootTreeMapItem.TreeMapItemGrid != null)
                            RootTreeMapItem.TreeMapItemGrid.Background = null;
                    }
                    else
#endif
                        GenerateTreeMapItems();

                    treeMapGrid.Children.Add(RootTreeMapItem);
                }
            }
        }

        private void SfTreeMap_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if ((ItemsSource as IEnumerable).GetEnumerator().MoveNext())
                UpdateTreeMapItems();
        }

        private void SfTreeMap_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateTreeMapItems();
        }

#if !WINDOWS_PHONE
#if WINRT
        private void drillDownHeaderPresenter_PointerPressed(object sender, PointerRoutedEventArgs e)
#else
        private void drillDownHeaderPresenter_MouseDown(object sender, MouseEventArgs e)
#endif
        {
            if ((sender as ContentPresenter).Tag is TreeMapItem)
            {
                TreeMapItem treeMapItem = ((sender as ContentPresenter).Tag as TreeMapItem);
                if (isDrilledIn != null)
                {
                    isResized = false;
                    treeMapItem.CreateDrillDownStoryboard();
                    treeMapItem.DrillDownAndDrillUpTreeMapItem(false);
                    drillDownHeaderPresenter.Tag = treeMapItem.isRootTreeMapItem ? null : treeMapItem.ParentTreeMapItem;
                }
            }
        }
#endif

        #endregion

        #region IDisposable Method

        public void Dispose()
        {
            SizeChanged -= SfTreeMap_SizeChanged;
            Loaded -= SfTreeMap_Loaded;
#if !WINDOWS_PHONE
            if (drillDownHeaderPresenter != null)
            {
#if WINRT
                drillDownHeaderPresenter.PointerPressed -= drillDownHeaderPresenter_PointerPressed;
#elif WPF
                drillDownHeaderPresenter.MouseDown -= drillDownHeaderPresenter_MouseDown;
#else
                drillDownHeaderPresenter.MouseLeftButtonDown -= drillDownHeaderPresenter_MouseDown;
#endif
                drillDownHeaderPresenter = null;
            }
#endif

            if (RootTreeMapItem != null)
            {
                RootTreeMapItem.Dispose();
                RootTreeMapItem = null;
            }
            if (Levels != null)
            {
                Levels.Clear();
                Levels = null;
            }
            if (LeafNodes != null)
            {
                LeafNodes.Clear();
                LeafNodes = null;
            }
            if (wholeItems != null)
            {
                wholeItems.Clear();
                wholeItems = null;
            }
            if (Legend != null)
                Legend.ItemsSource = null;
#if WPF
            treeMapTooltip = null;
#else
            treeMapPopup = null;
#endif
            engine = null;
            resourceDictionary = null;
            if (LeafColorMapping != null)
            {
                if (LeafColorMapping is RangeBrushColorMapping)
                {
                    (LeafColorMapping as RangeBrushColorMapping).Brushes.Clear();
                    (LeafColorMapping as RangeBrushColorMapping).Brushes = null;
                }
                else if (LeafColorMapping is PaletteColorMapping)
                {
                    (LeafColorMapping as PaletteColorMapping).Colors.Clear();
                    (LeafColorMapping as PaletteColorMapping).Colors = null;
                }
                LeafColorMapping = null;
            }
            if (ItemsSource != null)
            {
                if (ItemsSource is INotifyCollectionChanged)
                {
                    (ItemsSource as INotifyCollectionChanged).CollectionChanged -= SfTreeMap_CollectionChanged;
                }
                if (ItemsSource is IEnumerable)
                {
                    foreach (object item in (ItemsSource as IEnumerable))
                    {
                        var propertyChanged = item as INotifyPropertyChanged;
                        if (propertyChanged != null)
                        {
                            propertyChanged.PropertyChanged -= SfTreeMap_PropertyChanged;
                        }
                    }
                }
                ItemsSource = null;
            }

            if (treeMapGrid != null)
            {
                treeMapGrid.Children.Clear();
                treeMapGrid = null;
            }
            GC.SuppressFinalize(this);
            GC.Collect();
        }

        #endregion
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
#if WINRT
        public object Convert(object value, System.Type targetType, object parameter, string language)
#else
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
#endif
        {
            if (value != null)
            {
                return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

#if WINRT
        public object ConvertBack(object value, System.Type targetType, object parameter, string language)
#else
        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
#endif
        {
            throw new NotImplementedException();
        }
    }

    public class VisibilityConverter : IValueConverter
    {
#if WINRT
        public object Convert(object value, System.Type targetType, object parameter, string language)
#else
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
#endif
        {
            return (value != null) ? Visibility.Visible : Visibility.Collapsed;
        }

#if WINRT
        public object ConvertBack(object value, System.Type targetType, object parameter, string language)
#else
        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
#endif
        {
            throw new NotImplementedException();
        }
    }
}
