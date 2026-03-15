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
using System.Linq;
#if WINRT
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Collections;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Animation;
#if WPF
using System.Data; 
#endif
#endif

namespace Syncfusion.UI.Xaml.TreeMap
{
    public class TreeMapItem : Control, IDisposable
    {
        #region Constructor

        public TreeMapItem()
        {
            DefaultStyleKey = typeof(TreeMapItem);
            SubItemsList = new ObservableCollection<object>();
            TreeMapItemCanvas = new Canvas();
#if !WPF
            timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 30) }; 
#endif
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

        #region Weight
        internal double Weight
        {
            get { return (double)GetValue(WeightProperty); }
            set { SetValue(WeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Weight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty WeightProperty =
            DependencyProperty.Register("Weight", typeof(double), typeof(TreeMapItem), new PropertyMetadata(0d));
        #endregion

        #region AreaByWeight
        internal double AreaByWeight
        {
            get { return (double)GetValue(AreaByWeightProperty); }
            set { SetValue(AreaByWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AreaByWeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AreaByWeightProperty =
            DependencyProperty.Register("AreaByWeight", typeof(double), typeof(TreeMapItem), new PropertyMetadata(0d));
        #endregion

        #region LeftPosition
        internal double LeftPosition
        {
            get { return (double)GetValue(LeftPositionProperty); }
            set { SetValue(LeftPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftPosition.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LeftPositionProperty =
            DependencyProperty.Register("LeftPosition", typeof(double), typeof(TreeMapItem), new PropertyMetadata(0d));
        #endregion

        #region TopPosition
        internal double TopPosition
        {
            get { return (double)GetValue(TopPositionProperty); }
            set { SetValue(TopPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TopPosition.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TopPositionProperty =
            DependencyProperty.Register("TopPosition", typeof(double), typeof(TreeMapItem), new PropertyMetadata(0d));
        #endregion

        #region ItemHeight
        internal double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemHeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(TreeMapItem), new PropertyMetadata(0d));
        #endregion

        #region ItemWidth
        internal double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(TreeMapItem), new PropertyMetadata(0d));
        #endregion

        #region ColorWeight
        internal double ColorWeight
        {
            get { return (double)GetValue(ColorWeightProperty); }
            set { SetValue(ColorWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorWeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ColorWeightProperty =
            DependencyProperty.Register("ColorWeight", typeof(double), typeof(TreeMapItem), new PropertyMetadata(0d));
        #endregion

        #region HeaderSize
        internal double HeaderSize
        {
            get { return (double)GetValue(HeaderSizeProperty); }
            set { SetValue(HeaderSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderSize.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HeaderSizeProperty =
            DependencyProperty.Register("HeaderSize", typeof(double), typeof(TreeMapItem), new PropertyMetadata(0d));
        #endregion

        #region Header
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            internal set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(TreeMapItem), new PropertyMetadata(null));
        #endregion

        #region HeaderTemplate
        internal DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(TreeMapItem), new PropertyMetadata(null));
        #endregion

        #region HeaderColor
        internal Brush HeaderColor
        {
            get { return (Brush)GetValue(HeaderColorProperty); }
            set { SetValue(HeaderColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderColor.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HeaderColorProperty =
            DependencyProperty.Register("HeaderColor", typeof(Brush), typeof(TreeMapItem), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
        #endregion

        #region Label
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            internal set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(TreeMapItem), new PropertyMetadata(null));
        #endregion

        #region LabelTemplate
        internal DataTemplate LabelTemplate
        {
            get { return (DataTemplate)GetValue(LabelTemplateProperty); }
            set { SetValue(LabelTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelTemplate.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LabelTemplateProperty =
            DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(TreeMapItem), new PropertyMetadata(null, OnLabelTemplateChanged));

        private static void OnLabelTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapItem)
            {
                var treeMapItem = (d as TreeMapItem);
                if (treeMapItem.labelPresenter != null)
                {
                    treeMapItem.labelPresenter.Content = (treeMapItem.LabelTemplate == null) ? null : treeMapItem.Label;
                    treeMapItem.labelPresenter.ContentTemplate = treeMapItem.LabelTemplate;
                }
            }
        }
        #endregion

        #region ShowLabels
        public bool ShowLabels
        {
            get { return (bool)GetValue(ShowLabelsProperty); }
            internal set { SetValue(ShowLabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowLabels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowLabelsProperty =
            DependencyProperty.Register("ShowLabels", typeof(bool), typeof(TreeMapItem), new PropertyMetadata(false));
        #endregion

        #region SubItemsList
        internal ObservableCollection<object> SubItemsList
        {
            get { return (ObservableCollection<object>)GetValue(SubItemsListProperty); }
            set { SetValue(SubItemsListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TreeList.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty SubItemsListProperty =
            DependencyProperty.Register("SubItemsList", typeof(ObservableCollection<object>), typeof(TreeMapItem), new PropertyMetadata(null));
        #endregion

        #region ChildTreeMapItems
        public List<TreeMapItem> ChildTreeMapItems
        {
            get { return (List<TreeMapItem>)GetValue(ChildTreeMapItemsProperty); }
            internal set { SetValue(ChildTreeMapItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChildTreeMapItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildTreeMapItemsProperty =
            DependencyProperty.Register("ChildTreeMapItems", typeof(List<TreeMapItem>), typeof(TreeMapItem), new PropertyMetadata(null));
        #endregion

        #region LeafNodes
        public List<TreeMapLeafNode> LeafNodes
        {
            get { return (List<TreeMapLeafNode>)GetValue(LeafNodesProperty); }
            internal set { SetValue(LeafNodesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeafNodes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeafNodesProperty =
            DependencyProperty.Register("LeafNodes", typeof(List<TreeMapLeafNode>), typeof(TreeMapItem), new PropertyMetadata(null));
        #endregion

        #region Data
        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            internal set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(TreeMapItem), new PropertyMetadata(null));
        #endregion

        #region TreeMap
        internal SfTreeMap TreeMap
        {
            get { return (SfTreeMap)GetValue(TreeMapProperty); }
            set { SetValue(TreeMapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TreeMap.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TreeMapProperty =
            DependencyProperty.Register("TreeMap", typeof(SfTreeMap), typeof(TreeMapItem), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Private Members

        ContentPresenter headerPresenter, labelPresenter;
        Rect AvailableArea;
        ResourceDictionary resourceDictionary;
#if !WINDOWS_PHONE
        Grid drillDownOverlay;
        Storyboard drillDownStoryboard;
        DoubleAnimation drillDownScaleXAnimation, drillDownScaleYAnimation;
        DoubleAnimation drillDownWidthAnimation, drillDownHeightAnimation;
        bool isParentDrilled;
        double actualWidth, actualHeight;
        bool isLayoutUpdated;
#endif
#if !WPF
        DispatcherTimer timer;
        FrameworkElement currentTreeMapNode;
#endif

        #endregion

        #region Internal Members

        internal bool isRootTreeMapItem;
        internal TreeMapLevel GroupingLevel;
        internal TreeMapItem ParentTreeMapItem;
        internal Canvas TreeMapItemCanvas;
        internal Grid TreeMapItemGrid;
#if !WINDOWS_PHONE
        internal bool isDrilledItem;
#endif

        #endregion

        #region CLR Properties

        #region TreeMapLevel
        private TreeMapLevel treeMapLevel;
        public TreeMapLevel TreeMapLevel
        {
            get
            {
                return treeMapLevel;
            }
            internal set
            {
                treeMapLevel = value;
                if (treeMapLevel != null)
                {
                    var showLabelsBinding = new Binding { Path = new PropertyPath("ShowLabels"), Source = treeMapLevel };
                    BindingOperations.SetBinding(this, ShowLabelsProperty, showLabelsBinding);
                }
            }
        }
        #endregion

        #endregion

#if WINRT
        protected override void OnApplyTemplate()
        {
#else
        public override void OnApplyTemplate()
        {
#endif
            TreeMapItemGrid = GetTemplateChild("PART_TreeMapItemGrid") as Grid;
#if !WPF
            if (TreeMapItemCanvas != null && !isRootTreeMapItem)
            {
                Binding widthBinding = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("ItemWidth"),
                    Mode = BindingMode.OneWay
                };
                TreeMapItemCanvas.SetBinding(Canvas.WidthProperty, widthBinding);
                Binding heightBinding = new Binding
                {
                    Source = this,
                    Path = new PropertyPath("ItemHeight"),
                    Mode = BindingMode.OneWay
                };
                TreeMapItemCanvas.SetBinding(Canvas.HeightProperty, heightBinding);
            }
#endif
            base.OnApplyTemplate();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
#if !WINDOWS_PHONE
            if (TreeMap != null && (TreeMap.drilledTreeMapItem == null || (TreeMap.drilledTreeMapItem != null && isDrilledItem)))
#else
			if (TreeMap != null)
#endif
            {
                bool isSizeCalculated = false;
                if (TreeMap.ItemsLayoutMode != TreeMapLayoutMode.Squarified)
                {
                    if (isRootTreeMapItem)
                    {
                        isSizeCalculated = true;
#if !WINDOWS_PHONE
                        if (TreeMap.EnableDrillDown && TreeMap.isDrilledIn == null)
                            TreeMap.DrillDownHeader = GetDrillDownHeader();
#endif
                        if (ChildTreeMapItems != null)
                            CalculateSliceAndDiceItemsSize(ChildTreeMapItems, availableSize, GroupingLevel.GroupingGap, 0d);
                        else if (LeafNodes != null)
                            CalculateSliceAndDiceLeafNodesSize(LeafNodes, availableSize, 0d);
                    }
                    else
                    {
                        isSizeCalculated = true;
                        if (ChildTreeMapItems != null)
                            CalculateSliceAndDiceItemsSize(ChildTreeMapItems, availableSize, GroupingLevel.GroupingGap, HeaderSize);
                        else if (LeafNodes != null)
                            CalculateSliceAndDiceLeafNodesSize(LeafNodes, availableSize, HeaderSize);
                    }
                }
                else
                {
                    if (isRootTreeMapItem)
                    {
                        isSizeCalculated = true;
#if !WINDOWS_PHONE
                        if (TreeMap.EnableDrillDown && TreeMap.isDrilledIn == null)
                            TreeMap.DrillDownHeader = GetDrillDownHeader();
#endif
                        if (ChildTreeMapItems != null)
                            CalculateSquarifiedItemsSize(ChildTreeMapItems, availableSize, GroupingLevel.GroupingGap, 0d);
                        else if (LeafNodes != null)
                            CalculateSquarifiedLeafNodesSize(LeafNodes, availableSize, 0d);
                    }
                    else
                    {
                        isSizeCalculated = true;
                        if (ChildTreeMapItems != null)
                            CalculateSquarifiedItemsSize(ChildTreeMapItems, availableSize, GroupingLevel.GroupingGap, HeaderSize);
                        else if (LeafNodes != null)
                            CalculateSquarifiedLeafNodesSize(LeafNodes, availableSize, HeaderSize);
                    }
                }

                if (TreeMapItemGrid != null && TreeMapItemGrid.Children.Count > 0)
                    TreeMapItemGrid.Children.Clear();
                if (TreeMapItemCanvas != null && TreeMapItemCanvas.Children.Count > 0)
                    TreeMapItemCanvas.Children.Clear();

                if (isSizeCalculated)
                {
                    if (ChildTreeMapItems != null && ChildTreeMapItems.Count > 0)
                    {
                        IEnumerable<TreeMapItem> items = from item in ChildTreeMapItems
                                                         where ((item.ItemWidth > 0 && item.ItemHeight > 0) && (!(Double.IsInfinity(item.LeftPosition))) && (!(Double.IsInfinity(item.TopPosition))))
                                                         select item;
                        IEnumerator iterator = items.GetEnumerator();
#if !WINDOWS_PHONE
                        if (!TreeMap.EnableDrillDown || (TreeMap.EnableDrillDown && (isParentDrilled || (TreeMap.Levels.IndexOf(GroupingLevel) <= 1))))
                        {
                            if (isParentDrilled)
                                TreeMap.LeafColorMapping.EvaluateColorMapping(items.ToList(), false);
#endif
                            while (iterator.MoveNext())
                            {
                                TreeMapItem childTreeMapItem = iterator.Current as TreeMapItem;
#if !WINDOWS_PHONE
                                if (TreeMap.EnableDrillDown)
                                    childTreeMapItem.isParentDrilled = isRootTreeMapItem;
#endif
                                AddItemsInTreeMapLayout(childTreeMapItem, null);
                            }
#if !WINDOWS_PHONE
                        }
#endif
                    }
#if !WINDOWS_PHONE
                    else if (LeafNodes != null && ((TreeMap.EnableDrillDown && isParentDrilled) || !TreeMap.EnableDrillDown))
#else
                    else if (LeafNodes != null)
#endif
                    {
                        IEnumerable<TreeMapLeafNode> items = from item in LeafNodes
                                                             where ((item.Width > 0 && item.Height > 0) && (!(Double.IsInfinity(item.LeftPosition))) && (!(Double.IsInfinity(item.TopPosition))))
                                                             select item;
                        IEnumerator iterator = items.GetEnumerator();
                        while (iterator.MoveNext())
                        {
                            AddItemsInTreeMapLayout(null, iterator.Current as TreeMapLeafNode);
                        }
                    }
                }
                if (TreeMapItemCanvas != null)
                {
                    TreeMapItemGrid.Children.Clear();
                    TreeMapItemGrid.Children.Add(TreeMapItemCanvas);
                }
#if !WINDOWS_PHONE
                if (TreeMap.Levels.Count > 0 && (!TreeMap.EnableDrillDown || (TreeMap.EnableDrillDown && isParentDrilled)))
#endif
                    AddHeadersAndLabels(TreeMapItemGrid);
#if !WINDOWS_PHONE
                if (TreeMap.isLoaded && TreeMap.EnableDrillDown)
                    CreateDrillDownOverlay(TreeMapItemGrid);
#endif
            }
            return availableSize;
        }

        double AspectRatio(double x, double y)
        {
            return (x > y) ? (x / y) : (y / x);
        }

        Orientation GetOrientation()
        {
            if (TreeMap != null)
            {
                if (TreeMap.ItemsLayoutMode == TreeMapLayoutMode.SliceAndDiceHorizontal)
                    return Orientation.Horizontal;
                if (TreeMap.ItemsLayoutMode == TreeMapLayoutMode.SliceAndDiceVertical)
                    return Orientation.Vertical;
            }
            return AvailableArea.Width > AvailableArea.Height ? Orientation.Horizontal : Orientation.Vertical;
        }

        double ShorterSideLength
        {
            get
            {
                return Math.Min(AvailableArea.Width, AvailableArea.Height);
            }
        }

#if !WPF
        void timer_Tick(object sender, object e)
        {
            if (TreeMap != null && TreeMap.ShowToolTip && TreeMap.treeMapPopup != null)
                TreeMap.treeMapPopup.IsOpen = false;
        }
#endif

        #region SquarifiedLayout

        void CalculateSquarifiedItemsSize(List<TreeMapItem> TreeMapItemList, Size AvailableSize, double Gap, double HeaderHeight)
        {
            double headerSize = HeaderHeight < AvailableSize.Height ? HeaderHeight : 0;
            double totalweight = TreeMapItemList.Sum(x => x.Weight);
            AvailableSize = new Size(AvailableSize.Width, AvailableSize.Height - headerSize);
#if !WINDOWS_PHONE
            if (isRootTreeMapItem)
                TreeMap.rootTreeMapItemSize = AvailableSize;
#endif
            AvailableArea = new Rect(0, 0, AvailableSize.Width, AvailableSize.Height);
            int itemsCount = TreeMapItemList.Count;

            for (int i = itemsCount - 1; i >= 0; i--)
            {
                TreeMapItemList[i].AreaByWeight = (AvailableSize.Height * AvailableSize.Width) * TreeMapItemList[i].Weight / totalweight;
            }

            var OrderedTreeMapItemList = new ObservableCollection<TreeMapItem>(TreeMapItemList.OrderByDescending(x => x.AreaByWeight));

            double GroupMaxAspectRatio = 0d, curX = 0, curY = 0;
            int j;

            for (int i = 0; i < itemsCount; i = j)
            {
                var firstTreemapItem = OrderedTreeMapItemList[i];
                double GroupTotalWeight = 0d;
                j = i;

                for (; j < itemsCount; j++)
                {
                    var lastTreemapItem = OrderedTreeMapItemList[j];
                    GroupTotalWeight += lastTreemapItem.AreaByWeight;
                    double GroupWidth = GroupTotalWeight / ShorterSideLength;
                    double firstitemheight = firstTreemapItem.AreaByWeight / GroupWidth;
                    double lastitemheight = lastTreemapItem.AreaByWeight / GroupWidth;
                    if (j == 0)
                        GroupMaxAspectRatio = AspectRatio(GroupWidth, ShorterSideLength);
                    double TempAspectRatio = Math.Max(AspectRatio(firstitemheight, GroupWidth), AspectRatio(lastitemheight, GroupWidth));
                    if (GroupTotalWeight.Equals(lastTreemapItem.AreaByWeight) || TempAspectRatio < GroupMaxAspectRatio)
                    {
                        GroupMaxAspectRatio = TempAspectRatio;
                    }
                    else
                    {
                        GroupTotalWeight -= lastTreemapItem.AreaByWeight;
                        GroupWidth = GroupTotalWeight / ShorterSideLength;
                        GroupMaxAspectRatio = Math.Max(AspectRatio(firstitemheight, GroupWidth), AspectRatio(lastitemheight, GroupWidth));
                        break;
                    }
                }

                Orientation orientation = GetOrientation();
                var currentRect = new Rect();

                for (int k = i; k < j; k++)
                {
                    var item = OrderedTreeMapItemList[k];
                    double areaSum = GroupTotalWeight;
                    if (k == i)
                    {
                        currentRect = (orientation == Orientation.Horizontal) ? new Rect(AvailableArea.X, AvailableArea.Y, areaSum / AvailableArea.Height, AvailableArea.Height) :
                                                                                new Rect(AvailableArea.X, AvailableArea.Y, AvailableArea.Width, areaSum / AvailableArea.Width);
                        AvailableArea = (orientation == Orientation.Horizontal) ? new Rect(AvailableArea.X + currentRect.Width, AvailableArea.Y, Math.Max(0, AvailableArea.Width - currentRect.Width), AvailableArea.Height) :
                                                                                  new Rect(AvailableArea.X, AvailableArea.Y + currentRect.Height, AvailableArea.Width, Math.Max(0, AvailableArea.Height - currentRect.Height));
                        curX = currentRect.X;
                        curY = currentRect.Y;
                    }

                    Rect rect;
                    if (OrderedTreeMapItemList.IndexOf(item) != itemsCount - 1)
                    {
                        if (currentRect.Height <= Gap || currentRect.Width <= Gap)
                            Gap = 0;
                        rect = (orientation == Orientation.Horizontal) ? new Rect(0, 0, currentRect.Width - Gap, item.AreaByWeight / currentRect.Width) :
                                                                             new Rect(0, 0, item.AreaByWeight / currentRect.Height, currentRect.Height - Gap);
                        if (j - k != 1)
                        {
                            if (rect.Height <= Gap || rect.Width <= Gap)
                                Gap = 0;
                            rect = (orientation == Orientation.Horizontal) ? new Rect(0, 0, rect.Width, rect.Height - Gap) :
                                                                             new Rect(0, 0, rect.Width - Gap, rect.Height);
                        }
                    }
                    else
                    {
                        rect = (orientation == Orientation.Horizontal) ? new Rect(0, 0, currentRect.Width, item.AreaByWeight / currentRect.Width) :
                                                                         new Rect(0, 0, item.AreaByWeight / currentRect.Height, currentRect.Height);
                    }

                    item.ItemWidth = rect.Width;
                    item.ItemHeight = rect.Height;
                    item.LeftPosition = curX;
                    item.TopPosition = curY;

                    if (orientation == Orientation.Horizontal)
                    {
                        if (j - k != 1)
                            curY = curY + rect.Height + Gap;
                        else
                            curY += rect.Height;
                    }
                    else
                    {
                        if (j - k != 1)
                            curX = curX + rect.Width + Gap;
                        else
                            curX += rect.Width;
                    }
                }
            }
        }

        void CalculateSquarifiedLeafNodesSize(List<TreeMapLeafNode> TreeMapItemList, Size AvailableSize, double HeaderHeight)
        {
            double headerSize = HeaderHeight < AvailableSize.Height ? HeaderHeight : 0;
            double totalweight = TreeMapItemList.Sum(x => x.Weight);
            AvailableSize = new Size(AvailableSize.Width, AvailableSize.Height - headerSize);
            AvailableArea = new Rect(0, 0, AvailableSize.Width, AvailableSize.Height);
            int itemsCount = TreeMapItemList.Count;

            for (int i = itemsCount - 1; i >= 0; i--)
            {
                TreeMapItemList[i].AreaByWeight = (AvailableSize.Height * AvailableSize.Width) * TreeMapItemList[i].Weight / totalweight;
            }

            var OrderedTreeMapItemList = new ObservableCollection<TreeMapLeafNode>(TreeMapItemList.OrderByDescending(x => x.AreaByWeight));

            double GroupMaxAspectRatio = 0d, curX = 0, curY = 0;
            int j;

            for (int i = 0; i < itemsCount; i = j)
            {
                var firstTreemapItem = OrderedTreeMapItemList[i];
                double GroupTotalWeight = 0d;
                j = i;

                for (; j < itemsCount; j++)
                {
                    var lastTreemapItem = OrderedTreeMapItemList[j];
                    GroupTotalWeight += lastTreemapItem.AreaByWeight;
                    double GroupWidth = GroupTotalWeight / ShorterSideLength;
                    double firstitemheight = firstTreemapItem.AreaByWeight / GroupWidth;
                    double lastitemheight = lastTreemapItem.AreaByWeight / GroupWidth;
                    if (j == 0)
                        GroupMaxAspectRatio = AspectRatio(GroupWidth, ShorterSideLength);
                    double TempAspectRatio = Math.Max(AspectRatio(firstitemheight, GroupWidth), AspectRatio(lastitemheight, GroupWidth));
                    if (GroupTotalWeight.Equals(lastTreemapItem.AreaByWeight) || TempAspectRatio < GroupMaxAspectRatio)
                    {
                        GroupMaxAspectRatio = TempAspectRatio;
                    }
                    else
                    {
                        GroupTotalWeight -= lastTreemapItem.AreaByWeight;
                        GroupWidth = GroupTotalWeight / ShorterSideLength;
                        GroupMaxAspectRatio = Math.Max(AspectRatio(firstitemheight, GroupWidth), AspectRatio(lastitemheight, GroupWidth));
                        break;
                    }
                }

                Orientation orientation = GetOrientation();
                var currentRect = new Rect();

                for (int k = i; k < j; k++)
                {
                    var item = OrderedTreeMapItemList[k];
                    double areaSum = GroupTotalWeight;

                    if (k == i)
                    {
                        currentRect = (orientation == Orientation.Horizontal) ? new Rect(AvailableArea.X, AvailableArea.Y, areaSum / AvailableArea.Height, AvailableArea.Height) :
                                                                                new Rect(AvailableArea.X, AvailableArea.Y, AvailableArea.Width, areaSum / AvailableArea.Width);
                        AvailableArea = (orientation == Orientation.Horizontal) ? new Rect(AvailableArea.X + currentRect.Width, AvailableArea.Y, Math.Max(0, AvailableArea.Width - currentRect.Width), AvailableArea.Height) :
                                                                                  new Rect(AvailableArea.X, AvailableArea.Y + currentRect.Height, AvailableArea.Width, Math.Max(0, AvailableArea.Height - currentRect.Height));
                        curX = currentRect.X;
                        curY = currentRect.Y;
                    }

                    item.LeftPosition = curX;
                    item.TopPosition = curY;

                    if (orientation == Orientation.Horizontal)
                    {
                        item.Width = currentRect.Width;
                        item.Height = item.AreaByWeight / currentRect.Width;
                        curY += item.Height;
                    }
                    else
                    {
                        item.Width = item.AreaByWeight / currentRect.Height;
                        item.Height = currentRect.Height;
                        curX += item.Width;
                    }
                }
            }
        }

        #endregion

        #region Slice And Dice Layout

        void CalculateSliceAndDiceItemsSize(List<TreeMapItem> TreeMapItemList, Size AvailableSize, double Gap, double HeaderHeight)
        {
            double headerSize = HeaderHeight < AvailableSize.Height ? HeaderHeight : 0;
            AvailableSize = new Size(AvailableSize.Width, AvailableSize.Height - headerSize);
#if !WINDOWS_PHONE
            if (isRootTreeMapItem)
                TreeMap.rootTreeMapItemSize = AvailableSize;
#endif
            AvailableArea = new Rect(0, 0, AvailableSize.Width, AvailableSize.Height);
            double totalWeight = TreeMapItemList.Sum(x => x.Weight);
            double parentArea = AvailableSize.Height * AvailableSize.Width;
            int itemsCount = TreeMapItemList.Count;
            double gap;

            Orientation orientation = GetOrientation();

            if (orientation == Orientation.Horizontal)
            {
                double parentHeight = AvailableSize.Height;
                double allottedWidth = 0;
                for (int i = 0; i < itemsCount; i++)
                {
                    double childarea = (parentArea / totalWeight) * TreeMapItemList[i].Weight;
                    double childWidth = childarea / parentHeight;
                    gap = (childWidth > Gap) ? Gap : 0;

                    if (allottedWidth <= AvailableSize.Width)
                    {
                        TreeMapItemList[i].ItemWidth = (i != itemsCount - 1) ? childWidth - gap : childWidth;
                        TreeMapItemList[i].ItemHeight = parentHeight;
                        TreeMapItemList[i].LeftPosition = allottedWidth;
                        TreeMapItemList[i].TopPosition = 0;
                        allottedWidth += childWidth;
                    }
                }
            }
            else
            {
                double parentWidth = AvailableSize.Width;
                double allottedHeight = 0;

                for (int i = 0; i < itemsCount; i++)
                {
                    double childarea = (parentArea / totalWeight) * TreeMapItemList[i].Weight;
                    double childHeight = childarea / parentWidth;
                    gap = (childHeight > Gap) ? Gap : 0;
                    if (allottedHeight <= AvailableSize.Height)
                    {
                        TreeMapItemList[i].ItemWidth = parentWidth;
                        TreeMapItemList[i].ItemHeight = (i != itemsCount - 1) ? childHeight - gap : childHeight;
                        TreeMapItemList[i].TopPosition = allottedHeight;
                        TreeMapItemList[i].LeftPosition = 0;
                        allottedHeight += childHeight;
                    }
                }
            }
        }

        void CalculateSliceAndDiceLeafNodesSize(List<TreeMapLeafNode> TreeMapItemList, Size AvailableSize, double HeaderHeight)
        {
            double headerSize = HeaderHeight < AvailableSize.Height ? HeaderHeight : 0;
            AvailableSize = new Size(AvailableSize.Width, AvailableSize.Height - headerSize);
            AvailableArea = new Rect(0, 0, AvailableSize.Width, AvailableSize.Height);
            double totalWeight = TreeMapItemList.Sum(x => x.Weight);
            double parentArea = AvailableSize.Height * AvailableSize.Width;
            int itemsCount = TreeMapItemList.Count;
            Orientation orientation = GetOrientation();

            if (orientation == Orientation.Horizontal)
            {
                double parentHeight = AvailableSize.Height;
                double allottedWidth = 0;

                for (int i = 0; i < itemsCount; i++)
                {
                    TreeMapLeafNode treemapitem = TreeMapItemList[i];

                    double childarea = (parentArea / totalWeight) * treemapitem.Weight;
                    double childWidth = childarea / parentHeight;
                    if (allottedWidth <= AvailableSize.Width)
                    {
                        TreeMapItemList[i].Width = childWidth;
                        TreeMapItemList[i].Height = parentHeight;
                        TreeMapItemList[i].LeftPosition = allottedWidth;
                        TreeMapItemList[i].TopPosition = 0;
                        allottedWidth += childWidth;
                    }
                }
            }
            else
            {
                double parentWidth = AvailableSize.Width;
                double allottedHeight = 0;

                for (int i = 0; i < itemsCount; i++)
                {
                    TreeMapLeafNode treemapitem = TreeMapItemList[i];

                    double childarea = (parentArea / totalWeight) * treemapitem.Weight;
                    double childHeight = childarea / parentWidth;
                    if (allottedHeight <= AvailableSize.Height)
                    {
                        TreeMapItemList[i].Width = parentWidth;
                        TreeMapItemList[i].Height = childHeight;
                        TreeMapItemList[i].TopPosition = allottedHeight;
                        TreeMapItemList[i].LeftPosition = 0;
                        allottedHeight += childHeight;
                    }
                }
            }
        }

        #endregion

        void AddItemsInTreeMapLayout(TreeMapItem treeMapItem, TreeMapLeafNode leafNode)
        {
            if (leafNode != null)
            {
                FrameworkElement treeMapNode;
                if (TreeMap.LeafTemplate == null)
                {
                    treeMapNode = new Grid();
                    Rectangle rect = new Rectangle();

                    Binding fillBinding = new Binding();
                    fillBinding.Source = leafNode;
                    fillBinding.Path = new PropertyPath("MappedColor");
                    rect.SetBinding(Rectangle.FillProperty, fillBinding);

                    Binding opacityBinding = new Binding();
                    opacityBinding.Source = leafNode;
                    opacityBinding.Path = new PropertyPath("MappedColor.Opacity");
                    rect.SetBinding(Rectangle.OpacityProperty, opacityBinding);

                    Binding strokeBinding = new Binding();
                    strokeBinding.Source = TreeMap;
                    strokeBinding.Path = new PropertyPath("BorderBrush");
                    rect.SetBinding(Rectangle.StrokeProperty, strokeBinding);

                    Binding strokeThicknessBinding = new Binding();
                    strokeThicknessBinding.Source = TreeMap;
                    strokeThicknessBinding.Path = new PropertyPath("BorderThickness");
                    rect.SetBinding(Rectangle.StrokeThicknessProperty, strokeThicknessBinding);

                    (treeMapNode as Grid).Children.Add(rect);
#if !WINDOWS_PHONE
                    if (leafNode.Label != null && ((TreeMap.EnableDrillDown && !isParentDrilled) || !TreeMap.EnableDrillDown))
#else
                    if (leafNode.Label != null)
#endif
                    {
                        (treeMapNode as Grid).Children.Add(new TextBlock
                        {
                            Text = leafNode.Label,
                            FontSize = 13,
                            FontFamily = new FontFamily("Segoe UI"),
                            Foreground = new SolidColorBrush(Colors.Black),
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        });
                    }
                }
                else
                {
#if SILVERLIGHT
                    var child = new ContentPresenter { Content = TreeMap.LeafTemplate.LoadContent() };
                    treeMapNode = new Border { Child = child, DataContext = leafNode };
#else
                    var child = new ContentPresenter { ContentTemplate = TreeMap.LeafTemplate };
                    var binding = new Binding { Source = leafNode };
                    child.SetBinding(ContentPresenter.ContentProperty, binding);
                    treeMapNode = new Border { Child = child };
                    leafNode.TreeMapNode = treeMapNode;
#endif
                }

                treeMapNode.Height = leafNode.Height;
                treeMapNode.Width = leafNode.Width;

#if WPF
                #region ToolTip

                ToolTip treemapNodeTooltip = new ToolTip();
                treemapNodeTooltip.Template = TreeMap.CustomToolTipTemplate;

                var contentBinding = new Binding { Source = leafNode };
                treemapNodeTooltip.SetBinding(ContentControl.ContentProperty, contentBinding);

                var contentTemplateBinding = new Binding { Source = TreeMap, Path = new PropertyPath("ToolTipTemplate") };
                treemapNodeTooltip.SetBinding(ContentControl.ContentTemplateProperty, contentTemplateBinding);

                Binding toolTipVisibilityBinding = new Binding { Source = TreeMap, Path = new PropertyPath("ShowToolTip"), Converter = new BoolToVisibilityConverter() };
                treemapNodeTooltip.SetBinding(ContentControl.VisibilityProperty, toolTipVisibilityBinding);
                treeMapNode.ToolTip = treemapNodeTooltip;

                #endregion
#endif

                Canvas.SetLeft(treeMapNode, leafNode.LeftPosition);
                Canvas.SetTop(treeMapNode, leafNode.TopPosition);
                TreeMapItemCanvas.Children.Add(treeMapNode);

                if (TreeMap.ShowToolTip || TreeMap.HighlightOnSelection)
                {
                    leafNode.TreeMapNode = treeMapNode;
                    leafNode.ParentNode = this;
                    treeMapNode.DataContext = leafNode;
                    leafNode.TreeMap = TreeMap;

                    if (TreeMap.ShowToolTip)
                    {
#if WINRT
                        treeMapNode.PointerEntered += treeMapNode_PointerEntered;
                        treeMapNode.PointerExited += treeMapNode_PointerExited;
#elif WINDOWS_PHONE
                        treeMapNode.MouseEnter += treeMapNode_MouseEnter;
                        treeMapNode.MouseLeave += treeMapNode_MouseLeave;
#elif !WPF
                        treeMapNode.MouseEnter += treeMapNode_MouseEnter;
                        treeMapNode.MouseLeave += treeMapNode_MouseLeave;
#endif
                    }
                    else
                    {
#if WINRT
                        treeMapNode.PointerPressed += treeMapNode_PointerPressed;
#elif WINDOWS_PHONE
                        treeMapNode.MouseEnter += treeMapNode_MouseEnter;
                        treeMapNode.MouseLeave += treeMapNode_MouseLeave;
#elif SILVERLIGHT
                        treeMapNode.MouseLeftButtonDown += treeMapNode_MouseDown;
#endif
                    }
                }
            }
            else if (treeMapItem != null && TreeMapItemCanvas != null)
            {
                treeMapItem.Width = treeMapItem.ItemWidth;
                treeMapItem.Height = treeMapItem.ItemHeight;
#if !WINDOWS_PHONE
                if (TreeMap.EnableDrillDown && isParentDrilled)
                {
                    treeMapItem.BorderBrush = TreeMap.BorderBrush;
                    treeMapItem.BorderThickness = new Thickness(TreeMap.BorderThickness);
                }
#endif
                Canvas.SetLeft(treeMapItem, treeMapItem.LeftPosition);
                Canvas.SetTop(treeMapItem, treeMapItem.TopPosition);
                TreeMapItemCanvas.Children.Add(treeMapItem);
            }
        }

        void AddHeadersAndLabels(Grid itemGrid)
        {
            #region Header
            if (!HeaderSize.Equals(0d))
            {
                if (TreeMap.Levels[0].GetType() == typeof(TreeMapFlatLevel))
                {
                    if (HeaderTemplate != null)
                    {
#if SILVERLIGHT
                        var binding = new Binding { Source = new TreeMapSubItem { Header = Header, MappedColor = HeaderColor } };
#else
                        var binding = new Binding { Source = new { Header = Header, MappedColor = HeaderColor } };
#endif
                        headerPresenter = new ContentPresenter
                        {
                            Height = HeaderSize,
                            ContentTemplate = HeaderTemplate,
                        };
                        headerPresenter.SetBinding(ContentPresenter.ContentProperty, binding);
                    }
                    else
                    {
                        Border content = new Border
                            {
                                Child = Header != null
                                ? new TextBlock
                                {
                                    Text = Header,
                                    FontFamily = new FontFamily("Segoe UI"),
                                    Foreground = new SolidColorBrush(Colors.White),
                                    FontSize = HeaderSize > 3 ? HeaderSize - 3 : HeaderSize,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Center
                                }
                                : null
                            };
                        Binding backgroundBinding = new Binding();
                        backgroundBinding.Source = this;
                        backgroundBinding.Path = new PropertyPath("HeaderColor");
                        content.SetBinding(Border.BackgroundProperty, backgroundBinding);

                        Binding opacityBinding = new Binding();
                        opacityBinding.Source = this;
                        opacityBinding.Path = new PropertyPath("HeaderColor.Opacity");
                        content.SetBinding(Border.OpacityProperty, opacityBinding);

                        headerPresenter = new ContentPresenter
                        {
                            Height = HeaderSize,
                            Width = ItemWidth,
                            Content = content
                        };
                    }
                }
                else
                {
                    if (HeaderTemplate != null)
                    {
#if SILVERLIGHT
                        var binding = new Binding { Source = new TreeMapSubItem { Data = Data, MappedColor = HeaderColor } };
#else
                        var binding = new Binding { Source = new { Data = Data, MappedColor = HeaderColor } };
#endif
                        headerPresenter = new ContentPresenter
                        {
                            Height = HeaderSize,
                            ContentTemplate = HeaderTemplate,
                        };
                        headerPresenter.SetBinding(ContentPresenter.ContentProperty, binding);
                    }
                    else
                    {
                        headerPresenter = new ContentPresenter
                        {
                            Height = HeaderSize,
                            Width = ItemWidth,
                            Content = new Border
                            {
                                Background = HeaderColor,
                                Child = Header != null
                                ? new TextBlock
                                {
                                    Text = Header,
                                    FontFamily = new FontFamily("Segoe UI"),
                                    Foreground = new SolidColorBrush(Colors.White),
                                    FontSize = HeaderSize > 3 ? HeaderSize - 3 : HeaderSize,
                                    HorizontalAlignment = HorizontalAlignment.Center,
                                    VerticalAlignment = VerticalAlignment.Center
                                }
                                : null
                            }
                        };
                    }
                }
            }
            #endregion

            if (ShowLabels)
            {
                if (TreeMap.Levels[0].GetType() == typeof(TreeMapFlatLevel))
                {
                    if (LabelTemplate != null)
                    {
#if SILVERLIGHT
                        var binding = new Binding { Source = new TreeMapSubItem { Label = Label } };
#else
                        var binding = new Binding { Source = new { Label = Label } };
#endif
                        labelPresenter = new ContentPresenter
                        {
                            IsHitTestVisible = false,
                            ContentTemplate = LabelTemplate,
                        };
                        labelPresenter.SetBinding(ContentPresenter.ContentProperty, binding);
                    }
                    else
                    {
                        labelPresenter = new ContentPresenter
                        {
                            IsHitTestVisible = false,
                            Content = Label != null
                            ? new TextBlock
                            {
                                Text = Label,
                                FontFamily = new FontFamily("Segoe UI"),
                                Foreground = new SolidColorBrush(Colors.Black),
                                FontSize = 13,
                                Margin = new Thickness(5, 5, 0, 0),
                                Width = ItemWidth > 5 ? ItemWidth - 5 : ItemWidth,
                                Height = ItemHeight > 5 ? ItemHeight - 5 : ItemHeight,
                                TextWrapping = TextWrapping.Wrap,
                            }
                            : null
                        };
                    }
                }
                else
                {
                    if (LabelTemplate != null)
                    {
#if SILVERLIGHT
                        var binding = new Binding { Source = new TreeMapSubItem { Data = Data } };
#else
                        var binding = new Binding { Source = new { Data = Data } };
#endif
                        labelPresenter = new ContentPresenter
                        {
                            IsHitTestVisible = false,
                            ContentTemplate = LabelTemplate,
                        };
                        labelPresenter.SetBinding(ContentPresenter.ContentProperty, binding);
                    }
                    else
                    {
                        labelPresenter = new ContentPresenter
                        {
                            IsHitTestVisible = false,
                            Content = Label != null
                            ? new TextBlock
                            {
                                Text = Label,
                                FontFamily = new FontFamily("Segoe UI"),
                                Foreground = new SolidColorBrush(Colors.Black),
                                FontSize = 13,
                                Margin = new Thickness(5, 5, 0, 0),
                                Width = ItemWidth > 5 ? ItemWidth - 5 : ItemWidth,
                                Height = ItemHeight > 5 ? ItemHeight - 5 : ItemHeight,
                                TextWrapping = TextWrapping.Wrap,
                            }
                            : null
                        };
                    }
                }
            }

            if (itemGrid != null)
            {
                int labelRow = 0;
                if (headerPresenter != null)
                {
                    itemGrid.RowDefinitions.Insert(0, new RowDefinition { Height = new GridLength(HeaderSize) });
                    itemGrid.Children.Add(headerPresenter);
                    Grid.SetRow(TreeMapItemCanvas, 1);
                    labelRow = 1;
                }
                if (labelPresenter != null)
                {
                    var visibilityBinding = new Binding
                    {
                        Source = this,
                        Path = new PropertyPath("ShowLabels"),
                        Converter = new BoolToVisibilityConverter()
                    };
                    labelPresenter.SetBinding(ContentPresenter.VisibilityProperty, visibilityBinding);
                    itemGrid.Children.Add(labelPresenter);
                    Grid.SetRow(labelPresenter, labelRow);
                }
            }
        }

#if !WINDOWS_PHONE
        internal void CreateDrillDownOverlay(Grid itemGrid)
        {
            if (drillDownOverlay != null)
            {
                itemGrid.Children.Remove(drillDownOverlay);
            }
            drillDownOverlay = new Grid
            {
                DataContext = this,
                Background = TreeMap.DrillDownSelectionStroke,
                Opacity = 0,
                Height = ItemHeight,
                Width = ItemWidth,
            };
            if (itemGrid != null)
            {
                itemGrid.Children.Add(drillDownOverlay);
                Grid.SetRowSpan(drillDownOverlay, itemGrid.RowDefinitions.Count);
            }
#if WINRT
            drillDownOverlay.PointerEntered += DrillDownOverlay_PointerEntered;
            drillDownOverlay.PointerExited += DrillDownOverlay_PointerExited;
            drillDownOverlay.PointerPressed += DrillDownOverlay_PointerPressed;
#else
            drillDownOverlay.MouseEnter += drillDownOverlay_MouseEnter;
            drillDownOverlay.MouseLeave += drillDownOverlay_MouseLeave;
#if WPF
            drillDownOverlay.MouseDown += drillDownOverlay_MouseDown;
#else
            drillDownOverlay.MouseLeftButtonDown += drillDownOverlay_MouseDown;
#endif
#endif
            CreateDrillDownStoryboard();

#if WPF
            #region ToolTip

            ToolTip treemapItemTooltip = new ToolTip();
            treemapItemTooltip.Template = TreeMap.CustomToolTipTemplate;

            var contentBinding = new Binding { Source = new { Header = Header, Label = Label, Weight = Weight, ColorWeight = ColorWeight, TreeMap = TreeMap } };
            treemapItemTooltip.SetBinding(ContentControl.ContentProperty, contentBinding);

            var contentTemplateBinding = new Binding { Source = TreeMap, Path = new PropertyPath("ToolTipTemplate") };
            treemapItemTooltip.SetBinding(ContentControl.ContentTemplateProperty, contentTemplateBinding);

            Binding toolTipVisibilityBinding = new Binding { Source = TreeMap, Path = new PropertyPath("ShowToolTip"), Converter = new BoolToVisibilityConverter() };
            treemapItemTooltip.SetBinding(ContentControl.VisibilityProperty, toolTipVisibilityBinding);
            ToolTip = treemapItemTooltip;

            #endregion
#endif
        }

        internal void DrillDownAndDrillUpTreeMapItem(bool isDrilledDown)
        {
            if (TreeMap != null)
            {
#if !WPF
                if (TreeMap.treeMapPopup != null)
                    TreeMap.treeMapPopup.IsOpen = false;
#endif
                TreeMap.isDrilledIn = isDrilledDown;
                TreeMap.drilledTreeMapItem = this;
                Size availableSize = TreeMap.rootTreeMapItemSize;
                if (isDrilledDown)
                {
                    actualWidth = ActualWidth;
                    actualHeight = ActualHeight;
                }
                ItemWidth = availableSize.Width;
                ItemHeight = availableSize.Height;
                if (TreeMap.ItemsLayoutMode != TreeMapLayoutMode.Squarified)
                {
                    if (ChildTreeMapItems != null)
                    {
                        CalculateSliceAndDiceItemsSize(ChildTreeMapItems, availableSize, GroupingLevel.GroupingGap, 0d);
                    }
                    else if (LeafNodes != null)
                        CalculateSliceAndDiceLeafNodesSize(LeafNodes, availableSize, 0d);
                }
                else
                {
                    if (ChildTreeMapItems != null)
                    {
                        CalculateSquarifiedItemsSize(ChildTreeMapItems, availableSize, GroupingLevel.GroupingGap, 0d);
                    }
                    else if (LeafNodes != null)
                    {
                        CalculateSquarifiedLeafNodesSize(LeafNodes, availableSize, 0d);
                    }
                }

                if (TreeMapItemCanvas != null && TreeMapItemCanvas.Children.Count > 0)
                    TreeMapItemCanvas.Children.Clear();
                if (TreeMapItemGrid.Children.Count > 0)
                    TreeMapItemGrid.Children.Clear();

                if (ChildTreeMapItems != null && ChildTreeMapItems.Count > 0)
                {
                    IEnumerable<TreeMapItem> items = from item in ChildTreeMapItems
                                                     where ((item.ItemWidth > 0 && item.ItemHeight > 0) && (!(Double.IsInfinity(item.LeftPosition))) && (!(Double.IsInfinity(item.TopPosition))))
                                                     select item;
                    IEnumerator iterator = items.GetEnumerator();
                    while (iterator.MoveNext())
                    {
                        TreeMapItem childTreeMapItem = iterator.Current as TreeMapItem;
                        childTreeMapItem.isParentDrilled = true;
                        AddItemsInTreeMapLayout(childTreeMapItem, null);
                        childTreeMapItem.isDrilledItem = true;
                    }
                }
                else if (LeafNodes != null)
                {
                    isParentDrilled = false;
                    IEnumerable<TreeMapLeafNode> items = from item in LeafNodes
                                                         where ((item.Width > 0 && item.Height > 0) && (!(Double.IsInfinity(item.LeftPosition))) && (!(Double.IsInfinity(item.TopPosition))))
                                                         select item;
                    IEnumerator iterator = items.GetEnumerator();
                    while (iterator.MoveNext())
                    {
                        TreeMapLeafNode leafNode = iterator.Current as TreeMapLeafNode;
                        AddItemsInTreeMapLayout(null, leafNode);
                    }
                }

                if (TreeMap.RootTreeMapItem != null && TreeMap.RootTreeMapItem.TreeMapItemGrid != null)
                {
                    var treeMapItemBackground = Background;
                    TreeMap.LeafColorMapping.EvaluateColorMapping(new List<TreeMapItem> { this }, false);
                    TreeMap.RootTreeMapItem.TreeMapItemGrid.Background = Background;
                    Background = treeMapItemBackground;
                    TreeMap.RootTreeMapItem.TreeMapItemGrid.Children.Clear();
                    TreeMap.RootTreeMapItem.TreeMapItemGrid.Children.Add(TreeMapItemCanvas);
                    isLayoutUpdated = false;
                    TreeMapItemCanvas.LayoutUpdated += DrillDownCanvas_LayoutUpdated;
                }
                if (TreeMap.drillDownHeaderPresenter != null && isDrilledDown)
                {
                    TreeMap.drillDownHeaderPresenter.Tag = ParentTreeMapItem;
                }
                if (!TreeMap.isResized)
                    TreeMap.DrillDownHeader = GetDrillDownHeader();
            }
        }

        void DrillDownCanvas_LayoutUpdated(object sender, object e)
        {
            if (!isLayoutUpdated && TreeMap != null && TreeMap.isDrilledIn != null && !TreeMap.isResized)
            {
                if ((bool)TreeMap.isDrilledIn)
                {
                    if (actualHeight != ItemHeight)
                    {
#if !WPF
                        TreeMapItemCanvas.RenderTransformOrigin = new Point(0, 0);
                        if (TopPosition != 0)
                        {
                            drillDownHeightAnimation.Duration = TimeSpan.FromMilliseconds(500);
                            drillDownHeightAnimation.From = actualHeight;
                            drillDownHeightAnimation.To = ItemHeight;
                            drillDownStoryboard.Children.Add(drillDownHeightAnimation);

                        }
                        else
#else
                        TreeMapItemCanvas.RenderTransformOrigin = new Point(0.5, 0.5);
#endif
                        {
                            drillDownScaleYAnimation.Duration = TimeSpan.FromMilliseconds(500);
                            drillDownScaleYAnimation.From = actualHeight / ItemHeight;
                            drillDownScaleYAnimation.To = 1;
                            drillDownStoryboard.Children.Add(drillDownScaleYAnimation);
                        }
                    }
                    if (actualWidth != ItemWidth)
                    {
#if !WPF
                        TreeMapItemCanvas.RenderTransformOrigin = new Point(0, 0);
                        if (LeftPosition != 0)
                        {
                            drillDownWidthAnimation.Duration = TimeSpan.FromMilliseconds(500);
                            drillDownWidthAnimation.From = actualWidth;
                            drillDownWidthAnimation.To = ItemWidth;
                            drillDownStoryboard.Children.Add(drillDownWidthAnimation);
                        }
                        else
#else
                        TreeMapItemCanvas.RenderTransformOrigin = new Point(0.5, 0.5);
#endif
                        {
                            drillDownScaleXAnimation.Duration = TimeSpan.FromMilliseconds(500);
                            drillDownScaleXAnimation.From = actualWidth / ItemWidth;
                            drillDownScaleXAnimation.To = 1;
                            drillDownStoryboard.Children.Add(drillDownScaleXAnimation);
                        }
                    }
                }
                else
                {
                    #region Zoom Out Animation

                    //TreeMapItemCanvas.RenderTransformOrigin = new Point(0.5, 0.5);

                    //drillDownScaleXAnimation.Duration = TimeSpan.FromMilliseconds(500);
                    //drillDownScaleXAnimation.From = 0.7;
                    //drillDownScaleXAnimation.FillBehavior = FillBehavior.HoldEnd;
                    //drillDownScaleXAnimation.To = 1;
                    //drillDownStoryboard.Children.Add(drillDownScaleXAnimation);

                    //drillDownScaleYAnimation.Duration = TimeSpan.FromMilliseconds(500);
                    //drillDownScaleYAnimation.From = 0.7;
                    //drillDownScaleYAnimation.FillBehavior = FillBehavior.HoldEnd;
                    //drillDownScaleYAnimation.To = 1;
                    //drillDownStoryboard.Children.Add(drillDownScaleYAnimation); 

                    #endregion

                    #region Vertical Animation

                    TreeMapItemCanvas.RenderTransformOrigin = new Point(0, 0);
                    drillDownScaleYAnimation.Duration = TimeSpan.FromMilliseconds(300);
                    drillDownScaleYAnimation.From = 0.6;
                    drillDownScaleYAnimation.FillBehavior = FillBehavior.HoldEnd;
                    drillDownScaleYAnimation.To = 1;
                    drillDownStoryboard.Children.Add(drillDownScaleYAnimation);

                    #endregion

                    #region Height Animation

                    //drillDownHeightAnimation.Duration = TimeSpan.FromMilliseconds(500);
                    //drillDownHeightAnimation.From = ItemHeight / 3;
                    //drillDownHeightAnimation.To = ItemHeight;
                    //drillDownStoryboard.Children.Add(drillDownHeightAnimation);

                    #endregion
                }
                if (drillDownStoryboard != null)
                    drillDownStoryboard.Begin();
            }
            isLayoutUpdated = true;
        }

        void DrillDownAnimation_Completed(object sender, object e)
        {
            if (TreeMap != null)
            {
                if (TreeMap.RootTreeMapItem != null && TreeMap.RootTreeMapItem.TreeMapItemGrid != null)
                    TreeMap.RootTreeMapItem.TreeMapItemGrid.Background = null;
            }
#if !WPF
            if (ChildTreeMapItems != null && TreeMap != null && TreeMap.isDrilledIn != null && (bool)TreeMap.isDrilledIn)
            {
                foreach (TreeMapItem item in ChildTreeMapItems)
                {
                    Rect itemRect = new Rect(new Point(item.LeftPosition, item.TopPosition), new Size(item.ItemWidth, item.ItemHeight));
                    Rect rect = new Rect(TreeMap.MousePosition, new Size());
                    rect.Intersect(itemRect);
                    if (!rect.IsEmpty)
                    {
                        item.drillDownOverlay.Opacity = 0.15;
                        if (TreeMap.treeMapPopup != null)
                        {
                            TreeMap.treeMapPopup.IsOpen = true;
                            SetPopupPosition(TreeMap.MousePosition);
                        }
                    }
                    else
                        item.drillDownOverlay.Opacity = 0;
                }
            }
#endif
        }

        internal void CreateDrillDownStoryboard()
        {
            if (TreeMapItemCanvas != null)
            {
                TreeMapItemCanvas.RenderTransform = new ScaleTransform();

                drillDownStoryboard = new Storyboard();

                #region ScaleXAnimation

                drillDownScaleXAnimation = new DoubleAnimation();
                drillDownScaleXAnimation.Completed += DrillDownAnimation_Completed;
                Storyboard.SetTarget(drillDownScaleXAnimation, TreeMapItemCanvas);
#if WINRT
                drillDownScaleXAnimation.EnableDependentAnimation = true;
                Storyboard.SetTargetProperty(drillDownScaleXAnimation, "(UIElement.RenderTransform).ScaleTransform.ScaleX");
#else
                Storyboard.SetTargetName(drillDownScaleXAnimation, TreeMapItemCanvas.Name);
                Storyboard.SetTargetProperty(drillDownScaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
#endif

                #endregion

                #region ScaleYAnimation

                drillDownScaleYAnimation = new DoubleAnimation();
                drillDownScaleYAnimation.Completed += DrillDownAnimation_Completed;
                Storyboard.SetTarget(drillDownScaleYAnimation, TreeMapItemCanvas);
#if WINRT
                drillDownScaleYAnimation.EnableDependentAnimation = true;
                Storyboard.SetTargetProperty(drillDownScaleYAnimation, "(UIElement.RenderTransform).ScaleTransform.ScaleY");
#else
                Storyboard.SetTargetName(drillDownScaleYAnimation, TreeMapItemCanvas.Name);
                Storyboard.SetTargetProperty(drillDownScaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
#endif

                #endregion

                #region WidthAnimation

                drillDownWidthAnimation = new DoubleAnimation();
                drillDownWidthAnimation.Completed += DrillDownAnimation_Completed;
                Storyboard.SetTarget(drillDownWidthAnimation, TreeMapItemCanvas);
#if WINRT
                drillDownWidthAnimation.EnableDependentAnimation = true;
                Storyboard.SetTargetProperty(drillDownWidthAnimation, "Width");
#else
                Storyboard.SetTargetProperty(drillDownWidthAnimation, new PropertyPath(Canvas.WidthProperty));
#endif

                #endregion

                #region HeightAnimation

                drillDownHeightAnimation = new DoubleAnimation();
                drillDownHeightAnimation.Completed += DrillDownAnimation_Completed;
                Storyboard.SetTarget(drillDownHeightAnimation, TreeMapItemCanvas);
#if WINRT
                drillDownHeightAnimation.EnableDependentAnimation = true;
                Storyboard.SetTargetProperty(drillDownHeightAnimation, "Height");
#else
                Storyboard.SetTargetProperty(drillDownHeightAnimation, new PropertyPath(Canvas.HeightProperty));
#endif
                #endregion
            }
        }

        private string GetDrillDownHeader()
        {
            int levels = TreeMap.Levels.Count;
            if (levels > 0)
            {
                if (LeafNodes != null)
                {
                    TreeMap.Levels[levels - 1].drillDownHeader = TreeMap.Levels[levels - 1].drillDownHeader +
                        (Header != null ? "." + Header : "");
                    return TreeMap.Levels[levels - 1].drillDownHeader;
                }
                else
                {
                    int index = TreeMap.Levels.IndexOf(GroupingLevel);
                    if (index == 0)
                    {
#if WPF
                        if (TreeMap.ItemsSource is DataTable)
                            TreeMap.Levels[index].drillDownHeader = (TreeMap.ItemsSource as DataTable).TableName;
                        else
#endif
                            TreeMap.Levels[index].drillDownHeader = TreeMap.ItemsSource.GetType() != null ? TreeMap.ItemsSource.GetType().Name : "";
                    }
                    else
                    {
                        string levelHeader = TreeMap.Levels[index - 1].drillDownHeader + (!String.IsNullOrEmpty(TreeMap.Levels[index - 1].drillDownHeader) ? "." : "");
                        TreeMap.Levels[index].drillDownHeader = levelHeader + (!String.IsNullOrEmpty(Header) ? Header : "");
                    }
                    return TreeMap.Levels[index].drillDownHeader;
                }
            }
            return "";
        }
#endif

#if !WPF
        private void ClearToolTip(object currentTreeMapNode)
        {
            if (TreeMap != null && TreeMap.ShowToolTip && TreeMap.treeMapPopup != null)
            {
                if (currentTreeMapNode != null)
                    currentTreeMapNode = null;
                TreeMap.treeMapPopup.IsOpen = false;
                timer.Stop();
            }
        }

        private void SetPopupPosition(Point mousePoint)
        {
            Popup popup = TreeMap.treeMapPopup;
            if (popup != null)
            {
                popup.IsOpen = TreeMap.ShowToolTip;
                timer.Start();
                timer.Tick += timer_Tick;
                FrameworkElement popupContent = (TreeMap.treeMapPopup.Child as ContentControl).Content as FrameworkElement;
#if WINRT
                double posX = mousePoint.X;
                double posY = mousePoint.Y;
                if (posX < 0)
                    posX = 0;
                else if (popupContent != null && posX + popupContent.Width > TreeMap.ActualWidth)
                    posX = TreeMap.ActualWidth - popupContent.Width;
                if (posY < 0)
                    posY = 0;
                else if (popupContent != null && posY + popupContent.Height > TreeMap.ActualHeight)
                    posY = TreeMap.ActualHeight - popupContent.Height;

                if (popup.HorizontalOffset != posX)
                    popup.HorizontalOffset = posX;
                if (popup.VerticalOffset != posY)
                    popup.VerticalOffset = posY; 
#else
                double posX = mousePoint.X;
                double posY = mousePoint.Y;
                if (posX < 0)
                    posX = 0;
                else if (popupContent != null && posX + popupContent.Width > TreeMap.ActualWidth)
                    posX = TreeMap.ActualWidth - popupContent.Width;
                if (posY < 0)
                    posY = 0;
                else if (popupContent != null && posY + popupContent.Height > TreeMap.ActualHeight)
                    posY = TreeMap.ActualHeight - popupContent.Height;

                if (popup.HorizontalOffset != posX)
                    popup.HorizontalOffset = posX;
                if (popup.VerticalOffset != posY)
                    popup.VerticalOffset = posY;
#endif
            }
        }
#endif

#if WINRT
        void treeMapNode_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (TreeMap != null && TreeMap.ShowToolTip)
            {
                Popup popup = TreeMap.treeMapPopup;
                if (popup != null)
                {
                    popup.IsOpen = TreeMap.ShowToolTip;
                    timer.Start();
                    timer.Tick += timer_Tick;
                    var frameworkElement = sender as FrameworkElement;

                    if (frameworkElement != null)
                    {
                        currentTreeMapNode = frameworkElement;
                        popup.DataContext = frameworkElement.DataContext;
                        FrameworkElement popupContent = (popup.Child as ContentControl).Content as FrameworkElement;
                        GeneralTransform gt = frameworkElement.TransformToVisual(TreeMap);
                        PointerPoint pointerPoint = e.GetCurrentPoint(frameworkElement);
                        Point screenPoint = gt.TransformPoint(new Point(pointerPoint.Position.X, pointerPoint.Position.Y));

                        double posX = screenPoint.X;
                        double posY = screenPoint.Y;
                        if (posX < 0)
                            posX = 0;
                        else if (popupContent != null && posX + popupContent.Width > TreeMap.ActualWidth)
                            posX = TreeMap.ActualWidth - popupContent.Width;
                        if (posY < 0)
                            posY = 0;
                        else if (popupContent != null && posY + popupContent.Height > TreeMap.ActualHeight)
                            posY = TreeMap.ActualHeight - popupContent.Height;

                        if (popup.HorizontalOffset != posX)
                            popup.HorizontalOffset = posX;
                        if (popup.VerticalOffset != posY)
                            popup.VerticalOffset = posY;
                    }
                }
            }
        }

        void treeMapNode_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ClearToolTip(currentTreeMapNode);
        }

        void treeMapNode_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (TreeMap != null && TreeMap.HighlightOnSelection)
            {
                var frameworkElement = sender as FrameworkElement;
                if (frameworkElement != null)
                {
                    var treeMapNode = (TreeMapLeafNode)frameworkElement.DataContext;
                    if (treeMapNode != null)
                        treeMapNode.IsSelected = true;
                    if (TreeMap.SelectedItem != null)
                        TreeMap.SelectedItem.IsSelected = false;
                    TreeMap.SelectedItem = treeMapNode;
                }
            }
        }

        void DrillDownOverlay_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            TreeMap.MousePosition = e.GetCurrentPoint(TreeMap.RootTreeMapItem).Position;
            if ((sender as Grid).DataContext is TreeMapItem)
                ((sender as Grid).DataContext as TreeMapItem).DrillDownAndDrillUpTreeMapItem(true);
        }

        void DrillDownOverlay_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if ((sender as Grid).DataContext is TreeMapItem)
            {
                drillDownOverlay.Opacity = 0.15;
                TreeMap.MousePosition = e.GetCurrentPoint(TreeMap.RootTreeMapItem).Position;
                if (TreeMap != null && TreeMap.ShowToolTip)
                {
                    var frameworkElement = sender as FrameworkElement;
                    if (frameworkElement != null)
                    {
                        TreeMap.treeMapPopup.DataContext = (TreeMapItem)((sender as Grid).DataContext);
                        GeneralTransform gt = frameworkElement.TransformToVisual(TreeMap);
                        PointerPoint pointerPoint = e.GetCurrentPoint(frameworkElement);
                        Point screenPoint = gt.TransformPoint(new Point(pointerPoint.Position.X, pointerPoint.Position.Y));
                        SetPopupPosition(screenPoint);
                    }
                }
            }
        }

        void DrillDownOverlay_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if ((sender as Grid).DataContext is TreeMapItem)
            {
                drillDownOverlay.Opacity = 0;
                ClearToolTip(null);
            }
        }
#elif WINDOWS_PHONE
        void treeMapNode_MouseEnter(object sender, MouseEventArgs e)
        {
            if (TreeMap != null)
            {
                if (TreeMap.ShowToolTip)
                {
                    Popup popup = TreeMap.treeMapPopup;
                    if (popup != null)
                    {
                        popup.IsOpen = TreeMap.ShowToolTip;
                        timer.Start();
                        timer.Tick += timer_Tick;
                        var frameworkElement = sender as FrameworkElement;

                        if (frameworkElement != null && currentTreeMapNode != frameworkElement)
                        {
                            popup.DataContext = frameworkElement.DataContext;
                            FrameworkElement popupContent = (popup.Child as ContentControl).Content as FrameworkElement;
                            GeneralTransform gt = frameworkElement.TransformToVisual(TreeMap);
                            Point pointerPoint = e.GetPosition(frameworkElement);
                            Point screenPoint = gt.Transform(pointerPoint);

                            double posX = screenPoint.X;
                            double posY = screenPoint.Y;
                            if (posX < 0)
                                posX = 0;
                            else if (popupContent != null && posX + popupContent.Width > TreeMap.ActualWidth)
                                posX = TreeMap.ActualWidth - popupContent.Width;
                            if (posY < 0)
                                posY = 0;
                            else if (popupContent != null && posY + popupContent.Height > TreeMap.ActualHeight)
                                posY = TreeMap.ActualHeight - popupContent.Height;

                            if (popup.HorizontalOffset != posX)
                                popup.HorizontalOffset = posX;
                            if (popup.VerticalOffset != posY)
                                popup.VerticalOffset = posY;
                        }
                    }
                }
                if (TreeMap.HighlightOnSelection)
                {
                    var frameworkElement = sender as FrameworkElement;
                    if (frameworkElement != null)
                    {
                        var leafNode = (TreeMapLeafNode)frameworkElement.DataContext;
                        leafNode.IsSelected = true;
                        if (TreeMap.SelectedItem != null)
                            TreeMap.SelectedItem.IsSelected = false;
                        TreeMap.SelectedItem = leafNode;
                    }
                }
            }
        }

        void treeMapNode_MouseLeave(object sender, MouseEventArgs e)
        {
            if (TreeMap != null && TreeMap.treeMapPopup != null)
            {
                currentTreeMapNode = null;
                TreeMap.treeMapPopup.IsOpen = false;
                timer.Stop();
            }
        }
#else
#if !WPF
        void treeMapNode_MouseEnter(object sender, MouseEventArgs e)
        {
            if (TreeMap != null)
            {
                Popup popup = TreeMap.treeMapPopup;
                if (popup != null)
                {
                    popup.IsOpen = TreeMap.ShowToolTip;
                    timer.Start();
                    timer.Tick += timer_Tick;
                    var frameworkElement = sender as FrameworkElement;

                    if (frameworkElement != null && currentTreeMapNode != frameworkElement)
                    {
                        currentTreeMapNode = frameworkElement;
                        popup.DataContext = frameworkElement.DataContext;
                        FrameworkElement popupContent = (popup.Child as ContentControl).Content as FrameworkElement;

                        Point mousePoint = e.GetPosition(null);
                        double posX = mousePoint.X;
                        double posY = mousePoint.Y;
                        if (posX < 0)
                            posX = 0;
                        else if (popupContent != null && posX + popupContent.Width > TreeMap.ActualWidth)
                            posX = TreeMap.ActualWidth - popupContent.Width;
                        if (posY < 0)
                            posY = 0;
                        else if (popupContent != null && posY + popupContent.Height > TreeMap.ActualHeight)
                            posY = TreeMap.ActualHeight - popupContent.Height;

                        if (popup.HorizontalOffset != posX)
                            popup.HorizontalOffset = posX;
                        if (popup.VerticalOffset != posY)
                            popup.VerticalOffset = posY;
                    }
                }
            }
        }

        void treeMapNode_MouseLeave(object sender, MouseEventArgs e)
        {
            if (TreeMap != null && TreeMap.treeMapPopup != null)
            {
                currentTreeMapNode = null;
                TreeMap.treeMapPopup.IsOpen = false;
                timer.Stop();
            }
        }

        void treeMapNode_MouseDown(object sender, MouseEventArgs e)
        {
            if (TreeMap != null && TreeMap.HighlightOnSelection)
            {
                var frameworkElement = sender as FrameworkElement;
                if (frameworkElement != null)
                {
                    var leafNode = (TreeMapLeafNode)frameworkElement.DataContext;
                    leafNode.IsSelected = true;
                    if (TreeMap.SelectedItem != null)
                        TreeMap.SelectedItem.IsSelected = false;
                    TreeMap.SelectedItem = leafNode;
                }
            }
        }
#endif

        void drillDownOverlay_MouseEnter(object sender, MouseEventArgs e)
        {
            if ((sender as Grid).DataContext is TreeMapItem)
            {
#if !WPF
                if (TreeMap != null)
                {
                    TreeMap.MousePosition = e.GetPosition(TreeMap.RootTreeMapItem);
                    if (TreeMap.ShowToolTip)
                    {
                        var frameworkElement = sender as FrameworkElement;
                        if (frameworkElement != null)
                        {
                            TreeMap.treeMapPopup.DataContext = (TreeMapItem)((sender as Grid).DataContext);
                            GeneralTransform gt = frameworkElement.TransformToVisual(TreeMap);
                            Point mousePoint = e.GetPosition(null);
                            SetPopupPosition(mousePoint);
                        }
                    }
                }
#endif
                drillDownOverlay.Opacity = 0.15;
            }
        }

        void drillDownOverlay_MouseLeave(object sender, MouseEventArgs e)
        {
            if ((sender as Grid).DataContext is TreeMapItem)
            {
                drillDownOverlay.Opacity = 0;
            }
        }

        void drillDownOverlay_MouseDown(object sender, MouseEventArgs e)
        {
            if ((sender as Grid).DataContext is TreeMapItem)
            {
                if (TreeMap != null)
                {
                    TreeMap.isResized = false;
#if !WPF
                    TreeMap.MousePosition = e.GetPosition(TreeMap.RootTreeMapItem); 
#endif
                }
                ((sender as Grid).DataContext as TreeMapItem).DrillDownAndDrillUpTreeMapItem(true);
            }
        }
#endif

        #region IDisposable Method

        public void Dispose()
        {
            if (LeafNodes != null)
            {
                foreach (TreeMapLeafNode leafNode in LeafNodes)
                {
                    var treeMapNode = leafNode.TreeMapNode as FrameworkElement;
                    if (treeMapNode != null)
                    {
#if WINRT
                        treeMapNode.PointerEntered -= treeMapNode_PointerEntered;
                        treeMapNode.PointerExited -= treeMapNode_PointerExited;
                        treeMapNode.PointerPressed -= treeMapNode_PointerPressed;
#elif !WPF
                        treeMapNode.MouseEnter -= treeMapNode_MouseEnter;
                        treeMapNode.MouseLeave -= treeMapNode_MouseLeave;
#elif SILVERLIGHT && !WINDOWS_PHONE
                        treeMapNode.MouseLeftButtonDown -= treeMapNode_MouseDown;
#endif
                    }
                    treeMapNode = null;
                }
                LeafNodes.Clear();
                LeafNodes = null;
            }
#if !WINDOWS_PHONE
            if (drillDownOverlay != null)
            {
#if WINRT
                drillDownOverlay.PointerPressed -= DrillDownOverlay_PointerPressed;
                drillDownOverlay.PointerEntered -= DrillDownOverlay_PointerEntered;
                drillDownOverlay.PointerExited -= DrillDownOverlay_PointerExited;
#else
                drillDownOverlay.MouseEnter -= drillDownOverlay_MouseEnter;
                drillDownOverlay.MouseLeave -= drillDownOverlay_MouseLeave;
#if WPF
                drillDownOverlay.MouseDown -= drillDownOverlay_MouseDown;
#else
                drillDownOverlay.MouseLeftButtonDown += drillDownOverlay_MouseDown;
#endif
#endif
            }
            if (drillDownStoryboard != null)
            {
                drillDownStoryboard.Stop();
                drillDownStoryboard.Children.Clear();
                drillDownStoryboard = null;
            }
#endif
            if (SubItemsList != null)
            {
                SubItemsList.Clear();
                SubItemsList = null;
            }
#if !WPF
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= timer_Tick;
                timer = null;
            }
#endif
            if (ChildTreeMapItems != null)
            {
                foreach (TreeMapItem childTreeMapItem in ChildTreeMapItems)
                {
                    childTreeMapItem.Dispose();
                }
                ChildTreeMapItems.Clear();
                ChildTreeMapItems = null;
            }

            TreeMap = null;
            TreeMapLevel = null;
            GroupingLevel = null;
            resourceDictionary = null;
            if (TreeMapItemCanvas != null)
                TreeMapItemCanvas.Children.Clear();
            if (TreeMapItemGrid != null)
                TreeMapItemGrid.Children.Clear();
            GC.SuppressFinalize(this);
            GC.Collect();
        }

        #endregion
    }

    public class TreeMapSubItem : DependencyObject
    {
        #region Data
        internal object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(TreeMapSubItem), new PropertyMetadata(null));
        #endregion

        #region Header
        internal string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(TreeMapSubItem), new PropertyMetadata(null));
        #endregion

        #region Label
        internal string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(TreeMapSubItem), new PropertyMetadata(null));
        #endregion

        #region MappedColor
        internal Brush MappedColor
        {
            get { return (Brush)GetValue(MappedColorProperty); }
            set { SetValue(MappedColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MappedColor.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MappedColorProperty =
            DependencyProperty.Register("MappedColor", typeof(Brush), typeof(TreeMapSubItem), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
        #endregion
    }
}
