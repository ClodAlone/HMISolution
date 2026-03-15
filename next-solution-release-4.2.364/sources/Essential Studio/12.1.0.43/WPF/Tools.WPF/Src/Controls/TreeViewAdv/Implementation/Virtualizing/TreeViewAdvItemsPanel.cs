#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.

#endregion Copyright Syncfusion Inc. 2001 - 2009

#region file using

using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    /// <summary>
    /// /// <summary>
    /// An class that provides functionality for virtualizing <see cref="TreeViewItemAdv"/>.
    /// </summary>
    /// <list type="table">
    /// 	<listheader>
    /// 		<term>Help Page</term>
    /// 		<description>Syntax</description>
    /// 	</listheader>
    /// 	<example>
    /// 		<list type="table">
    /// 			<listheader>
    /// 				<description>C#</description>
    /// 			</listheader>
    /// 			<example><code>public class TreeViewAdvItemsPanel : <see cref="FakeItemsPanel"/></code></example>
    /// 		</list>
    /// 		<para/>
    /// 		<list type="table">
    /// 			<listheader>
    /// 				<description>XAML Object Element Usage</description>
    /// 			</listheader>
    /// 			<example>
    /// 				<code language="XAML">
    /// You cannot use this managed class in XAML.
    /// </code>
    /// 			</example>
    /// 		</list>
    /// 	</example>
    /// </list>
    /// <remarks>
    /// An class that provides functionality
    /// for virtualizing <see cref="TreeViewItemAdv"/> and shows fake items.
    /// </remarks>
    /// </summary>
    public class TreeViewAdvItemsPanel : FakeItemsPanel
    {
        #region Constants

        /// <summary>
        /// A brush that describes the foreground color for fake items.
        /// </summary>
        private readonly Brush c_fakeForeground = Brushes.Red;

        /// <summary>
        /// Previous treeview element
        /// </summary>
        private TreeViewItemAdv prevElement;

        /// <summary>
        /// Offset X for vertical line.
        /// </summary>
        private const double c_verticalLineXOffset = 9;

        /// <summary>
        /// Offset Y for vertical line.
        /// </summary>
        private const double c_verticalLineYOffset = 2;

        #endregion Constants

        #region Members

        /// <summary>
        /// determines actual height of TreeViewItemAdv
        /// </summary>
        internal double actualheight = 20.0;

        /// <summary>
        /// determines previous offset
        /// </summary>
        internal static double m_previousoffset = 0.0;

        /// <summary>
        /// determines scrollowner handling
        /// </summary>
        internal static bool m_scrollmovemanually = false;

        /// <summary>
        /// Guide lines for line.
        /// </summary>
        private static readonly GuidelineSet c_guidelines;

        #endregion Members

        #region Properties

        /// <summary>
        /// Gets or sets the Pen used to draw of the line.
        /// </summary>
        /// <value>
        /// Type: <see cref="Pen"/>
        /// The Pen used to to draw of the line.
        /// </value>
        /// <example>
        ///
        /// <para/>This example shows how to set LinePen property in XAML.
        /// <code language="XAML">
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="Pen"/>
        public Pen LinePen
        {
            get
            {
                return (Pen)GetValue(LinePenProperty);
            }
            set
            {
                SetValue(LinePenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether show line.
        /// </summary>
        /// <value>
        /// Type: <see cref="Bool"/>
        /// Value indicating whether show line.
        /// </value>
        /// <example>
        ///
        /// <para/>This example shows how to set IsShowLine property in XAML.
        /// <code language="XAML">
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="Bool"/>
        public bool IsShowLine
        {
            get
            {
                return (bool)GetValue(IsShowLineProperty);
            }
            set
            {
                SetValue(IsShowLineProperty, value);
            }
        }

        /// <summary>
        /// Indicates whether the control need re-render.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// For re-render control need change property.
        /// </value>
        public bool InvalidateRenderRequest
        {
            get
            {
                return (bool)GetValue(InvalidateRenderRequestProperty);
            }
            set
            {
                SetValue(InvalidateRenderRequestProperty, value);
            }
        }

        /// <summary>
        /// Gets value whether indicates that need draw vertical line.
        /// </summary>
        protected virtual bool NeedDrawVerticalLine
        {
            get
            {
                bool needDraw = true;

                if (InternalChildren.Count == 0)
                {
                    needDraw = false;
                }

                return needDraw;
            }
        }

        /// <summary>
        /// Indicates whethe need update index.
        /// </summary>
        protected virtual bool IsNeedUpdateIndex
        {
            get
            {
                bool value = true;

                if (TreeViewAdv.IsDragging)
                {
                    value = false;
                }

                return value;
            }
        }

        #endregion Properties

        #region Dependency property

        /// <summary>
        /// Identifies TreeViewAdvItemsPanel. InvalidateRenderRequest dependency property.
        /// </summary>
        public static readonly DependencyProperty InvalidateRenderRequestProperty =
            DependencyProperty.Register("InvalidateRenderRequest", typeof(bool), typeof(TreeViewAdvItemsPanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies TreeViewAdvItemsPanel. LinePen dependency property.
        /// </summary>
        public static readonly DependencyProperty LinePenProperty =
            DependencyProperty.Register("LinePen", typeof(Pen), typeof(TreeViewAdvItemsPanel), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies TreeViewAdvItemsPanel. IsShowLine dependency property.
        /// </summary>
        public static readonly DependencyProperty IsShowLineProperty =
            DependencyProperty.Register("IsShowLine", typeof(bool), typeof(TreeViewAdvItemsPanel), new UIPropertyMetadata(true));

        #endregion Dependency property

        #region Initialization

        /// <summary>
        /// Static constructor.
        /// </summary>
        static TreeViewAdvItemsPanel()
        {
            c_guidelines = new GuidelineSet();
            c_guidelines.GuidelinesX.Add(0.5);
            c_guidelines.GuidelinesY.Add(0.5);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TreeViewAdvItemsPanel()
        {
            Loaded += new RoutedEventHandler(TreeViewAdvItemsPanel_Loaded);
            
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Raises the Initialized event. This method is invoked whenever IsInitialized
        /// is set to true internally.
        /// </summary>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            m_ownerTreeView = TreeViewAdv.GetTreeViewFromChildren(ParentItemsControl);
        }

        /// <summary>
        /// Shows fake items.
        /// </summary>
        /// <paparam name="element">Element for which will show fake items.</paparam>
        /// <param name="bIsTop">Indicates whether fake items will show over element;
        /// otherwise fake items will show under element.</param>
        public override void ShowFakeItems(UIElement element, bool bIsTop)
        {
            base.ShowFakeItems(element, bIsTop);
            InvalidateRenderRequest = !InvalidateRenderRequest;
        }

        /// <summary>
        /// Hide fake items.
        /// </summary>
        public override void HideFakeItems()
        {
            base.HideFakeItems();
            InvalidateRenderRequest = !InvalidateRenderRequest;
        }

        /// <summary>
        /// Represents the item
        /// </summary>
        internal object m_Item = null;

        /// <summary>
        /// Gets type of the fake element.
        /// </summary>
        protected override Type GetFakeElementType()
        {
            return typeof(TreeViewItemAdv);
        }

        /// <summary>
        /// Called when the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> collection that is associated with the <see cref="T:System.Windows.Controls.ItemsControl"/> for this <see cref="T:System.Windows.Controls.Panel"/> changes.
        /// </summary>
        /// <param name="sender">The <see cref="T:System.Object"/> that raised the event.</param>
        /// <param name="args">Provides data for the <see cref="E:System.Windows.Controls.ItemContainerGenerator.ItemsChanged"/> event.</param>
        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            HideFakeItems();

            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    try
                    {
                        m_Item = GetInternalItem(args.OldPosition.Index);
                        RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                    }
                    //SU I78477
                    //catch (Exception e)
                    catch (Exception)
                    //EU I78477
                    {
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    {
                        try
                        {
                            m_Item = GetInternalItem(args.OldPosition.Index);
                            RemoveInternalChildRange(args.OldPosition.Index, args.ItemUICount);
                            InsertInternalChild(args.Position.Index, (UIElement)sender);
                        }
                        //SU I78477
                        //catch (Exception e)
                        catch (Exception)
                        //EU I78477
                        {
                        }
                    }

                    break;
            }

            base.OnItemsChanged(sender, args);
            
        }

        internal object GetInternalItems()
        {
            return m_Item;
        }

        /// <summary>
        /// Gets container for fake element.
        /// </summary>
        protected override UIElement GetContainerForFakeElement(object element)
        {
            UIElement container = base.GetContainerForFakeElement(element);

            if (IsNeedCreateContainer(element))
            {
                TreeViewItemAdv item = new TreeViewItemAdv();
                item.Header = element;
                container = item;
            }

            Brush fakeForeground = c_fakeForeground;
            TreeViewAdv tree = TreeViewAdv.GetTreeViewFromChildren(this.ParentItemsControl);

            if (tree != null)
            {
                fakeForeground = tree.FakeItemForeground;
            }

            if (container != null && container is TreeViewItemAdv)
            {
                ((TreeViewItemAdv)container).Foreground = fakeForeground;
                ((TreeViewItemAdv)container).IsFakeItem = true;
            }

            return container;
        }

        /// <summary>
        /// Iterates the expanded items.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <returns></returns>
        internal double IterateExpandedItems(TreeViewItemAdv tree)
        {
            double height = 0.0, measuredheight = 0.0;
            if (tree.ExpandedItems.Count > 0)
            {
                TreeViewItemAdv m_previoustree = null;
                foreach (TreeViewItemAdv tree1 in tree.ExpandedItems)
                {
                    if (tree1.ParentTreeViewItem != null)
                    {
                        if (tree1.Items.Count > 0)
                            measuredheight = IterateExpandedItems(tree1);
                        if (m_previoustree == null)
                        {
                            if (tree1.ParentTreeViewItem.ItemContainerGenerator.IndexFromContainer(tree1) != -1)
                            {
                                height += tree1.ParentTreeViewItem.ItemContainerGenerator.IndexFromContainer(tree1) * actualheight + measuredheight;
                            }
                            else
                            {
                                int indx = tree1.ParentTreeViewItem.Items.IndexOf(tree1);
                                if (indx != -1)
                                {
                                    height += indx * actualheight + measuredheight;
                                }
                            }
                        }
                        else
                        {
                            int treeindx = tree1.ParentTreeViewItem.ItemContainerGenerator.IndexFromContainer(tree1);
                            int previoustreeindx = m_previoustree.ParentTreeViewItem.ItemContainerGenerator.IndexFromContainer(m_previoustree);
                            if (treeindx != -1 && previoustreeindx != -1)
                            {
                                if (treeindx > previoustreeindx)
                                {
                                    height += (treeindx - previoustreeindx) * actualheight + measuredheight;
                                }
                                else
                                {
                                    height += (previoustreeindx - treeindx) * actualheight + measuredheight;
                                }
                            }
                            else
                            {
                                if (treeindx == -1)
                                {
                                    treeindx = tree1.ParentTreeViewItem.Items.IndexOf(tree1);
                                }
                                if (previoustreeindx == -1)
                                {
                                    previoustreeindx = m_previoustree.ParentTreeViewItem.Items.IndexOf(m_previoustree);
                                }
                                if (treeindx > previoustreeindx)
                                {
                                    height += (treeindx - previoustreeindx) * actualheight + measuredheight;
                                }
                                else
                                {
                                    height += (previoustreeindx - treeindx) * actualheight + measuredheight;
                                }
                            }
                        }
                        m_previoustree = tree1;
                    }
                }
                if (m_previoustree != null)
                {
                    int indx = m_previoustree.ParentTreeViewItem.ItemContainerGenerator.IndexFromContainer(m_previoustree);
                    if (indx != -1)
                    {
                        if (indx < tree.Items.Count - 1)
                        {
                            height += (tree.Items.Count - 1 - indx) * actualheight;
                        }
                    }
                    else
                    {
                        indx = m_previoustree.ParentTreeViewItem.Items.IndexOf(m_previoustree);
                        if (indx < tree.Items.Count - 1)
                        {
                            height += (tree.Items.Count - 1 - indx) * actualheight;
                        }
                    }
                }
                else
                {
                    height = tree.Items.Count * actualheight;
                }
            }
            else
            {
                height = tree.Items.Count * actualheight;
            }
            return height;
        }

        /// <summary>
        /// Update visible item index.
        /// </summary>
        protected override void UpdateVisibilityIndex(Size availableSize)
        {
            if (ParentItemsControl != null && ParentItemsControl.Items.Count > 0)
            {
                if (OwnerTreeView != null && OwnerTreeView.IsVirtualizing && OwnerTreeView.VirtualizationMode == VirtualizationMode.Normal)
                {
                    TreeViewAdv parentTreeViewAdv = (ParentItemsControl as TreeViewAdv);
                    TreeViewItemAdv parentTreeViewItemAdv = (ParentItemsControl as TreeViewItemAdv);
                    int oldFirstIndex = FirstVisibleItemIndex;
                    int oldLastIndex = LastVisibleItemIndex;
                    int itemsCount = ParentItemsControl.Items.Count;
                    OwnerTreeView.extendHeight = 0.0;
                    ArrangeOffset = 0;
                    if (parentTreeViewAdv != null)
                    {
                        int tFirstVisibleIndex = Convert.ToInt32(OwnerTreeView.m_scrollinfo.VerticalOffset / OwnerTreeView.m_treeviewitemadvHeight);
                        double extendHeight1 = 0.0;
                        double availableViewPortHeight = OwnerTreeView.ScrollHost.ViewportHeight;
                        int lastFIndex = 0;
                        double diff = 0.0;
                        double extendedItemsHeight = 0.0;
                        bool firstVisible = false;
                        foreach (int indx in OwnerTreeView.ExpandedItemsIndexCollection)
                        {
                            if (indx < tFirstVisibleIndex)
                            {
                                extendHeight1 += (indx - lastFIndex) * OwnerTreeView.m_treeviewitemadvHeight;
                                lastFIndex = indx;
                                extendedItemsHeight += OwnerTreeView.ExpandedTreeViewAdvItems[indx].ActualHeight - OwnerTreeView.m_treeviewitemadvHeight;
                                diff = Math.Abs(OwnerTreeView.m_scrollinfo.VerticalOffset - extendHeight1);
                                extendHeight1 += OwnerTreeView.ExpandedTreeViewAdvItems[indx].ActualHeight - OwnerTreeView.m_treeviewitemadvHeight;
                                if (extendHeight1 >= OwnerTreeView.m_scrollinfo.VerticalOffset)
                                {
                                    availableViewPortHeight = availableViewPortHeight - extendedItemsHeight;
                                    tFirstVisibleIndex = indx;
                                    TreeViewItemAdv tvitem = OwnerTreeView.ItemContainerGenerator.ContainerFromIndex(indx) as TreeViewItemAdv;
                                    if (tvitem == null && OwnerTreeView.Items.Count > indx)
                                    {
                                        tvitem = OwnerTreeView.Items[indx] as TreeViewItemAdv;
                                    }
                                    if (tvitem != null)
                                        UpdateFirstVisibleItem(tvitem, diff);
                                    ArrangeOffset = -diff;
                                    firstVisible = true;
                                    break;
                                }
                            }

                            if (!firstVisible)
                            {
                                tFirstVisibleIndex = Convert.ToInt32((OwnerTreeView.m_scrollinfo.VerticalOffset - extendedItemsHeight) / OwnerTreeView.m_treeviewitemadvHeight);
                            }
                        }
                        if (tFirstVisibleIndex >= 0)
                            OwnerTreeView.m_fakeItemsPanel.FirstVisibleItemIndex = tFirstVisibleIndex;
                    }
                    if (parentTreeViewItemAdv != null)
                        ArrangeOffset = 0;

                    FirstVisibleItemIndex = (FirstVisibleItemIndex < 0) ? 0 : FirstVisibleItemIndex;
                    OwnerTreeView.extendHeight = 0.0;

                    Size size = availableSize;
                    if (double.IsInfinity(availableSize.Height))
                        size = new Size(availableSize.Width, OwnerTreeView.ScrollHost.ViewportHeight);

                    IterateLastVisibleCalc(OwnerTreeView.m_fakeItemsPanel, OwnerTreeView, size);
                    if (itemsCount > 0 && oldLastIndex == -1)
                        LastVisibleItemIndex = (LastVisibleItemIndex < 0) ? itemsCount - 1 : LastVisibleItemIndex;
                    if (parentTreeViewAdv != null)
                    {
                        if (parentTreeViewAdv.ScrollHost.ViewportHeight <= 0)
                        {
                            LastVisibleItemIndex = itemsCount - 1;
                        }
                    }
                    if (parentTreeViewAdv != null)
                    {
                        if (LastVisibleItemIndex > 0)
                        {
                            TreeViewItemAdv tviewitem = parentTreeViewAdv.ItemContainerGenerator.ContainerFromIndex(LastVisibleItemIndex - 1) as TreeViewItemAdv;
                            if (tviewitem == null)
                            {
                                if (LastVisibleItemIndex > 0)
                                {
                                    tviewitem = parentTreeViewAdv.Items[LastVisibleItemIndex - 1] as TreeViewItemAdv;
                                }
                                else
                                {
                                    tviewitem = parentTreeViewAdv.Items[LastVisibleItemIndex] as TreeViewItemAdv;
                                }
                            }
                            if (LastVisibleItemIndex == parentTreeViewAdv.Items.Count - 1)
                            {
                                tviewitem = parentTreeViewAdv.ItemContainerGenerator.ContainerFromIndex(LastVisibleItemIndex) as TreeViewItemAdv;
                                if (tviewitem == null)
                                {
                                    tviewitem = parentTreeViewAdv.Items[LastVisibleItemIndex] as TreeViewItemAdv;
                                }
                            }
                            if (tviewitem != null && tviewitem.IsExpanded)
                            {
                                LastItemExpandInvalidate(tviewitem);
                            }
                        }
                    }

                    if (OwnerTreeView.m_scrollinfo.ExtentHeight != 0 && OwnerTreeView.m_scrollinfo.ExtentHeight > ArrangeOffset)
                    {
                        if (OwnerTreeView.m_scrollinfo.ExtentHeight > actualheight * 2)
                        {
                            if (ArrangeOffset > OwnerTreeView.m_scrollinfo.ExtentHeight - actualheight * 2)
                            {
                                ArrangeOffset = OwnerTreeView.m_scrollinfo.ExtentHeight - actualheight;
                            }
                        }
                    }

                    if (!IsNeedUpdateIndex && (FirstVisibleItemIndex < oldFirstIndex
                        || LastVisibleItemIndex < oldLastIndex))
                    {
                        FirstVisibleItemIndex = oldFirstIndex;
                        LastVisibleItemIndex = oldLastIndex;
                    }
                    ArrangeOffset += GetTopOffset();
                }
                else
                {
                    FirstVisibleItemIndex = 0;
                    LastVisibleItemIndex = (ParentItemsControl == null) ? 0 : ParentItemsControl.Items.Count;
                    ArrangeOffset = 0;
                }
            }

            if (ParentTreeView != null && ParentTreeView.ShowRootLines)
            {
                if (ParentItemsControl is TreeViewAdv)
                {
                    updateTreeViewAdvRootLine();
                }
                else
                {
                    updateTreeViewItemAdvRootLine();
                }
            }
        }

        internal void LastItemExpandInvalidate(TreeViewItemAdv item)
        {
            if (item != null && item.IsExpanded)
            {
                if (item.m_virtualizingpanel != null)
                {
                    item.m_virtualizingpanel.InvalidateMeasure();
                }
                TreeViewItemAdv tviewitem = null;
                if (item.m_fakeItemsPanel != null)
                {
                    if (item.m_fakeItemsPanel.LastVisibleItemIndex > 0)
                        tviewitem = item.ItemContainerGenerator.ContainerFromIndex(item.m_fakeItemsPanel.LastVisibleItemIndex - 1) as TreeViewItemAdv;
                    if (tviewitem == null && item.Items.Count > item.m_fakeItemsPanel.LastVisibleItemIndex)
                    {
                        if (item.m_fakeItemsPanel.LastVisibleItemIndex != 0)
                        {
                            tviewitem = item.Items[item.m_fakeItemsPanel.LastVisibleItemIndex - 1] as TreeViewItemAdv;
                        }
                        else
                        {
                            tviewitem = item.Items[item.m_fakeItemsPanel.LastVisibleItemIndex] as TreeViewItemAdv;
                        }
                    }
                    if (tviewitem != null && tviewitem.IsExpanded)
                    {
                        LastItemExpandInvalidate(tviewitem);
                    }
                }
            }
        }

        internal void UpdateFirstVisibleItem(TreeViewItemAdv item, double diff)
        {
            int tFirstVisibleIndex = Convert.ToInt32(diff / item.m_treeviewitemadvHeight);
            double extendHeight1 = 0.0;
            int lastFIndex = 0;
            double extendedItemsHeight = 0.0;
            ArrangeOffset = 0;
            bool firstVisible = false;
            if (item.ExpandedItemsIndexCollection != null)
                foreach (int indx in item.ExpandedItemsIndexCollection)
                {
                    if (indx < tFirstVisibleIndex)
                    {
                        extendHeight1 += (indx - lastFIndex) * actualheight;
                        lastFIndex = indx;
                        if (item.ExpandedTreeViewAdvItems.ContainsKey(indx))
                            extendedItemsHeight += item.ExpandedTreeViewAdvItems[indx].ActualHeight - item.m_treeviewitemadvHeight;
                        double localdiff = diff - extendHeight1;
                        if (item.ExpandedTreeViewAdvItems.ContainsKey(indx))
                            extendHeight1 += item.ExpandedTreeViewAdvItems[indx].ActualHeight - item.m_treeviewitemadvHeight;
                        if (extendHeight1 >= diff)
                        {
                            tFirstVisibleIndex = indx;
                            if (item.ExpandedTreeViewAdvItems.ContainsKey(indx))
                            {
                                var tvitem = item.ItemContainerGenerator.ContainerFromIndex(indx) as TreeViewItemAdv ??
                                             item.ExpandedTreeViewAdvItems[indx];
                                if (tvitem != null)
                                    UpdateFirstVisibleItem(tvitem, localdiff);
                            }
                            firstVisible = true;
                            break;
                        }
                    }
                }

            if (!firstVisible)
            {
                tFirstVisibleIndex = Convert.ToInt32((diff - extendedItemsHeight) / c_scrollOffset);
            }
            if (tFirstVisibleIndex >= item.Items.Count + 1)
            {
                tFirstVisibleIndex = 0;
            }
            if (tFirstVisibleIndex >= 0)
            {
                if (item.m_fakeItemsPanel != null)
                {
                    if (tFirstVisibleIndex > 0)
                        item.m_fakeItemsPanel.FirstVisibleItemIndex = tFirstVisibleIndex - 1;
                    else
                        item.m_fakeItemsPanel.FirstVisibleItemIndex = tFirstVisibleIndex;
                    if (item.m_virtualizingpanel != null)
                    {
                        item.m_virtualizingpanel.InvalidateMeasure();
                    }
                }
            }
        }

        internal void IterateLastVisibleCalc(FakeItemsPanel tvitemAdv, ItemsControl parent, Size availableSize)
        {
            if (tvitemAdv != null)
            {
                int i = 0;
                for (i = tvitemAdv.FirstVisibleItemIndex; i < parent.Items.Count; i++)
                {
                    TreeViewItemAdv item = parent.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                    if (item == null)
                    {
                        item = parent.Items[i] as TreeViewItemAdv;
                    }
                    if (item != null)
                    {
                        if (item.IsExpanded)
                        {
                            if (item.m_fakeItemsPanel != null)
                            {
                                IterateLastVisibleCalc(item.m_fakeItemsPanel, item, availableSize);
                            }
                        }
                        else
                        {
                            OwnerTreeView.extendHeight += OwnerTreeView.m_treeviewitemadvHeight;
                            if (availableSize.Height > 0)
                            {
                                if (OwnerTreeView.extendHeight > availableSize.Height)
                                {
                                    tvitemAdv.LastVisibleItemIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        OwnerTreeView.extendHeight += OwnerTreeView.m_treeviewitemadvHeight;
                        if (availableSize.Height > 0)
                        {
                            if (OwnerTreeView.extendHeight > availableSize.Height)
                            {
                                tvitemAdv.LastVisibleItemIndex = i;
                                break;
                            }
                        }
                    }
                }
                if (parent.Items.Count > 0 && i == parent.Items.Count)
                {
                    tvitemAdv.LastVisibleItemIndex = parent.Items.Count - 1;
                }
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="parentItemsControl">The parent items control.</param>
        /// <returns></returns>
        internal UIElement GetItem(int index, ItemsControl parentItemsControl)
        {
            UIElement item = null;

            if (parentItemsControl != null &&
                index > -1 && index < parentItemsControl.Items.Count)
            {
                item = parentItemsControl.Items[index] as UIElement;

                if (item == null)
                {
                    item = parentItemsControl.ItemContainerGenerator.ContainerFromIndex(index) as UIElement;
                }
            }

            if (item == null)
            {
                if (item == null)
                {
                    TreeViewItemAdv node = new TreeViewItemAdv();
                    node.Header = parentItemsControl.Items[index];
                    node.ApplyTemplate();
                    node.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    item = node;
                }
            }

            return item;
        }

        /// <summary>
        /// Iterates the items.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        internal Size IterateItems(TreeViewItemAdv tree, Size size)
        {
            for (int i = 0; i < tree.Items.Count; i++)
            {
                TreeViewItemAdv treeitem = GetItem(i, tree as ItemsControl) as TreeViewItemAdv;
                if (treeitem != null)
                {
                    if (treeitem.IsExpanded)
                    {
                        size.Height += size.Height + (20 * GetCountItems(treeitem));
                        for (int j = 0; j < treeitem.Items.Count; j++)
                        {
                            TreeViewItemAdv titem = GetItem(j, treeitem as ItemsControl) as TreeViewItemAdv;
                            if (titem != null)
                            {
                                if (titem.IsExpanded)
                                {
                                    size.Height += size.Height + (20 * GetCountItems(titem));
                                    size = IterateItems(titem, size);
                                }
                            }
                        }
                    }
                }
            }
            return size;
        }

        /// <summary>
        /// Gets appreciate complete size for unvisible item.
        /// </summary>
        protected override Size GetComplateUnvisibleItemSize(int index)
        {
            Size size = Size.Empty;
            Size finalsize = Size.Empty;
            TreeViewItemAdv item = GetItem(index) as TreeViewItemAdv;

            if (item != null)
            {
                object key = GetKey(index);

                if (item.IsExpanded && item.Items.Count > 0)
                {
                    if (key != null)
                    {
                        size = GetCashedMeasureSize(key);
                    }

                    if (size.IsEmpty)
                    {
                        size = new Size(0, 0);
                    }

                    TreeViewAdvItemsPanel panel = ((IItemsPanelRef)item).ItemsPanel;
                    Size cashedSize = Size.Empty;
                    int countChild = GetCountItems(item);

                    if (panel != null && key != null)
                    {
                        cashedSize = panel.GetCashedMeasureSize(panel.GetKey(0));
                    }

         
                    if (size.Height > 0)
                    {
                        finalsize = IterateItems(item, size);
                        size.Height += finalsize.Height;
                    }
                }
                else
                {
                    if (key != null)
                    {
                        size = GetCashedMeasureSize(key);
                    }

                    if (size.IsEmpty)
                    {
                        size = GetActualItemSize(index);
                    }
                }
            }

            return size;
        }

        /// <summary>
        /// Gets appreciate complete size for item.
        /// </summary>
        protected override Size GetCompleteItemSize(int index)
        {
            Size itemSize = Size.Empty;
            TreeViewItemAdv item = GetItem(index) as TreeViewItemAdv;
            object key = GetKey(index);

            if (index < FirstVisibleItemIndex || index > LastVisibleItemIndex)
            {
                itemSize = GetComplateUnvisibleItemSize(index);
            }
            else
            {
                itemSize = GetActualItemSize(index);
            }

            return itemSize;
        }

        /// <summary>
        /// Gets desired size for UIElement when size isn't determine.
        /// </summary>
        internal override Size GetActualItemSize(int index)
        {
            Size itemSize = new Size(0, 0);
            Size cashSize = Size.Empty;
            bool bIsSetCash = false;

            if (index > -1)
            {
                TreeViewItemAdv item = GetItem(index) as TreeViewItemAdv;
                object key = GetKey(index);
                if (item != null)
                {
                    item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    itemSize = item.DesiredSize;
                }

                if (!m_cashedMeasureSize.ContainsKey(key))
                {
                    bIsSetCash = true;
                    cashSize = itemSize;
                }

                if (m_cashedMeasureSize.ContainsKey(key) && item.CompleteHeaderElement != null && item != null
                    && GetCashedMeasureSize(key).Height != item.CompleteHeaderElement.DesiredSize.Height)
                {
                    bIsSetCash = true;
                    cashSize = item.CompleteHeaderElement.DesiredSize;
                }

                if (bIsSetCash)
                {
                    SetCashedMeasureSize(key, cashSize);
                }
            }

            return itemSize;
        }

        /// <summary>
        /// Gets count of the all children node.
        /// </summary>
        protected virtual int GetCountItems(TreeViewItemAdv item)
        {
            int count = 0;

            if (item != null)
            {
                count += item.Items.Count;

                for (int i = 0; i < item.Items.Count; i++)
                {
                    TreeViewItemAdv currentItem = item.Items[i] as TreeViewItemAdv;

                    if (currentItem != null && currentItem.IsExpanded && currentItem is IItemsPanelRef)
                    {
                        IItemsPanelRef panel = currentItem as IItemsPanelRef;

                        if (panel.ItemsPanel != null)
                        {
                            count += panel.ItemsPanel.GetCountItems(currentItem);
                        }
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Gets appreciate complete size of the this panel.
        /// </summary>
        protected override Size GetCompletePanelSize(Size availableSize)
        {
            availableSize = base.GetCompletePanelSize(availableSize);

            if (FakeItems.Count > 0)
            {
                for (int i = 0; i < FakeItems.Count; i++)
                {
                    if (InternalChildren.Contains(FakeItems[i]))
                    {
                        availableSize.Height += FakeItems[i].DesiredSize.Height;

                        if (availableSize.Width < FakeItems[i].DesiredSize.Width)
                        {
                            availableSize.Width = FakeItems[i].DesiredSize.Width;
                        }
                    }
                }
            }

            return availableSize;
        }

        /// <summary>
        /// Gets item from owner items control by index.
        /// </summary>
        protected override UIElement GetItem(int index)
        {
            TreeViewItemAdv item = base.GetItem(index) as TreeViewItemAdv;

            if (ParentTreeView != null && item == null && (index < lastVisibleIndex && ParentTreeView.VirtualizationMode == VirtualizationMode.Extended
                && ParentTreeView.IsVirtualizing) || (!ParentTreeView.IsVirtualizing && ParentTreeView.VirtualizationMode == VirtualizationMode.Normal && item == null))
                if (item == null)
                {
                    TreeViewItemAdv node = new TreeViewItemAdv();
                    node.Header = ParentItemsControl.Items[index];
                    node.ApplyTemplate();
                    node.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    item = node;
                }

            return item;
        }

        /// <summary>
        /// Gets pen for drawing line.
        /// </summary>
        private Pen GetLinePen()
        {
            Brush brush = (ParentItemsControl != null) ?
                TreeViewAdv.GetLineBrush(ParentItemsControl) : Brushes.Black;
            Pen linePen = new Pen(brush, 1);
            linePen.DashStyle = DashStyles.Dot;
            linePen.DashCap = PenLineCap.Square;
            linePen.StartLineCap = PenLineCap.Square;
            linePen.EndLineCap = PenLineCap.Square;
            linePen.LineJoin = PenLineJoin.Miter;

            return linePen;
        }

        /// <summary>
        /// Draws the content of a <see cref="T:System.Windows.Media.DrawingContext"/> object during the render pass of a <see cref="T:System.Windows.Controls.Panel"/> element.
        /// </summary>
        /// <param name="dc">The <see cref="T:System.Windows.Media.DrawingContext"/> object to draw.</param>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (ParentTreeView.ShowRootLines)
            {
                if (ParentItemsControl is TreeViewAdv)
                {
                    updateTreeViewAdvRootLine();
                }
                else
                {
                    updateTreeViewItemAdvRootLine();
                }
            }
        }

        /// <summary>
        /// Updates the tree view adv root line.
        /// </summary>
        private void updateTreeViewAdvRootLine()
        {
            if (InternalChildren.Count > 0)
            {
                TreeViewItemAdv firstVisibleItem = InternalChildren[0] as TreeViewItemAdv;
                TreeViewItemAdv lastVisibleItem = InternalChildren[InternalChildren.Count - 1] as TreeViewItemAdv;
                TreeViewItemAdv lastVItem = null;

                foreach (TreeViewItemAdv element in InternalChildren)
                {
                    if (element != null && element.VerticalLinePartOne != null && element.VerticalLinePartTwo != null)
                    {
                        prevElement = element;
                        element.VerticalLinePartOne.Visibility = Visibility.Visible;
                        element.VerticalLinePartTwo.Visibility = Visibility.Visible;
                        element.VerticalLinePartOne.ClearValue(FrameworkElement.VerticalAlignmentProperty);
                        element.VerticalLinePartOne.ClearValue(FrameworkElement.HeightProperty);
                        element.VerticalLinePartOne.ClearValue(FrameworkElement.MarginProperty);
                    }
         

                    if (element.Visibility == System.Windows.Visibility.Visible)
                        lastVItem = element;
                }
                if (lastVItem != null && lastVItem.VerticalLinePartTwo != null)
                {
                    lastVItem.VerticalLinePartTwo.Visibility = System.Windows.Visibility.Collapsed;
                    if ((lastVisibleItem.Visibility == Visibility.Hidden || lastVisibleItem.Visibility == Visibility.Collapsed) && lastVItem.VerticalLinePartOne != null)
                    {
                        lastVItem.VerticalLinePartOne.Height = lastVItem.CompleteHeaderElement.DesiredSize.Height / 2;
                        lastVItem.VerticalLinePartOne.VerticalAlignment = VerticalAlignment.Top;
                    }
                }
                if (InternalChildren.Count == 1 && ParentItemsControl.Items.Count == 1)
                {
                    if (firstVisibleItem != null && firstVisibleItem.VerticalLinePartOne != null && firstVisibleItem.VerticalLinePartTwo != null)
                    {
                        firstVisibleItem.VerticalLinePartOne.Visibility = Visibility.Collapsed;
                        firstVisibleItem.VerticalLinePartTwo.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    if (firstVisibleItem != null && firstVisibleItem.VerticalLinePartOne != null)
                    {
                        object firstitem = ParentItemsControl.ItemContainerGenerator.ItemFromContainer(firstVisibleItem);

                        if (firstVisibleItem.CompleteHeaderElement != null)
                        {
                            firstVisibleItem.VerticalLinePartOne.Margin = new Thickness(0, firstVisibleItem.CompleteHeaderElement.DesiredSize.Height / 2, 0, 0);
                        }
                        else if (firstVisibleItem.IsFakeItem && firstVisibleItem.CompleteHeaderElement != null)
                        {
                            firstVisibleItem.VerticalLinePartOne.Margin = new Thickness(0, firstVisibleItem.CompleteHeaderElement.DesiredSize.Height / 2, 0, 0);
                        }
                    }
                    if (lastVisibleItem != null && lastVisibleItem.VerticalLinePartTwo != null)
                    {
                        object lastitem = ParentItemsControl.ItemContainerGenerator.ItemFromContainer(lastVisibleItem);

                        if (ParentItemsControl.Items.Count > 0 && lastitem == ParentItemsControl.Items[ParentItemsControl.Items.Count - 1])
                        {
                            if (lastVisibleItem.CompleteHeaderElement != null)
                            {
                                lastVisibleItem.VerticalLinePartTwo.Visibility = Visibility.Collapsed;
                                lastVisibleItem.VerticalLinePartOne.Height = lastVisibleItem.CompleteHeaderElement.DesiredSize.Height / 2;
                                lastVisibleItem.VerticalLinePartOne.VerticalAlignment = VerticalAlignment.Top;
                            }
                            if (lastVisibleItem != null && lastVisibleItem.visiblityFlag && lastVisibleItem.CompleteHeaderElement != null && lastVisibleItem.Visibility != Visibility.Visible && ParentItemsControl.Items.Count > 1)
                            {
                                for (int i = ParentItemsControl.Items.Count - 2; i >= 0; i--)
                                {
                                    TreeViewItemAdv prevVisibleItem = ParentItemsControl.Items[i] as TreeViewItemAdv;
                                    if (prevVisibleItem != null && prevVisibleItem.Visibility == Visibility.Visible)
                                    {
                                        if (i != 0)
                                        {
                                            prevVisibleItem.VerticalLinePartOne.Height = prevVisibleItem.CompleteHeaderElement.DesiredSize.Height / 2;
                                            prevVisibleItem.VerticalLinePartTwo.Visibility = Visibility.Collapsed;
                                            prevVisibleItem.VerticalLinePartOne.VerticalAlignment = VerticalAlignment.Top;
                                            break;
                                        }
                                        else
                                        {
                                            prevVisibleItem.VerticalLinePartOne.Visibility = Visibility.Collapsed;
                                            prevVisibleItem.VerticalLinePartTwo.Visibility = Visibility.Collapsed;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (lastVisibleItem.IsFakeItem && lastVisibleItem.CompleteHeaderElement != null)
                        {
                            lastVisibleItem.VerticalLinePartTwo.Visibility = Visibility.Collapsed;
                            lastVisibleItem.VerticalLinePartOne.Height = lastVisibleItem.CompleteHeaderElement.DesiredSize.Height / 2;
                            lastVisibleItem.VerticalLinePartOne.VerticalAlignment = VerticalAlignment.Top;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the tree view item adv root line.
        /// </summary>
        private void updateTreeViewItemAdvRootLine()
        {
            if (InternalChildren.Count > 0)
            {
                TreeViewItemAdv firstVisibleItem = InternalChildren[0] as TreeViewItemAdv;
                TreeViewItemAdv lastVisibleItem = InternalChildren[InternalChildren.Count - 1] as TreeViewItemAdv;
                TreeViewItemAdv lastVItem = null;
                foreach (TreeViewItemAdv element in InternalChildren)
                {
                    if (element != null && element.VerticalLinePartOne != null && element.VerticalLinePartTwo != null)
                    {
                        element.VerticalLinePartOne.Visibility = Visibility.Visible;
                        if (ParentTreeView != null && ParentTreeView is TreeViewAdv && (ParentTreeView as TreeViewAdv) != null && (ParentTreeView as TreeViewAdv).MultiColumnEnable)
                            element.VerticalLinePartOne.Height = element.CompleteHeaderElement.DesiredSize.Height / 2;
                        element.VerticalLinePartTwo.Visibility = Visibility.Visible;
                        element.VerticalLinePartOne.ClearValue(FrameworkElement.VerticalAlignmentProperty);
                        if (!ParentTreeView.IsVirtualizing)
                            element.VerticalLinePartOne.ClearValue(FrameworkElement.HeightProperty);
                        element.VerticalLinePartOne.ClearValue(FrameworkElement.MarginProperty);
                    }
                    if (element.Visibility == System.Windows.Visibility.Visible)
                        lastVItem = element;
                }
                if (lastVItem != null && lastVItem.VerticalLinePartTwo != null)
                {
                    lastVItem.VerticalLinePartTwo.Visibility = System.Windows.Visibility.Collapsed;

                    if ((lastVisibleItem.Visibility == Visibility.Hidden || lastVisibleItem.Visibility == Visibility.Collapsed) && lastVItem.VerticalLinePartOne != null)
                    {
                        lastVItem.VerticalLinePartOne.Height = lastVItem.CompleteHeaderElement.DesiredSize.Height / 2;
                        lastVItem.VerticalLinePartOne.VerticalAlignment = VerticalAlignment.Top;
                    }
                }
                if (InternalChildren.Count > 0 && ParentItemsControl.Items.Count > 0)
                {
                    if (lastVisibleItem != null && lastVisibleItem.VerticalLinePartOne != null && lastVisibleItem.VerticalLinePartTwo != null)
                    {
                        object lastobj = ParentItemsControl.ItemContainerGenerator.ItemFromContainer(lastVisibleItem);

                        if (ParentItemsControl.Items.Count > 0 && lastobj == ParentItemsControl.Items[ParentItemsControl.Items.Count - 1])
                        {
                            lastVisibleItem.VerticalLinePartTwo.Visibility = Visibility.Collapsed;
                            lastVisibleItem.VerticalLinePartOne.Height = lastVisibleItem.CompleteHeaderElement.DesiredSize.Height / 2;
                            lastVisibleItem.VerticalLinePartOne.VerticalAlignment = VerticalAlignment.Top;
                        }
                        if (lastVisibleItem != null && lastVisibleItem.visiblityFlag && lastVisibleItem.Visibility != Visibility.Visible && ParentItemsControl.Items.Count > 1)
                        {
                            for (int i = ParentItemsControl.Items.Count - 2; i >= 0; i--)
                            {
                                TreeViewItemAdv prevVisibleItem = ParentItemsControl.Items[i] as TreeViewItemAdv;
                                if (prevVisibleItem != null && prevVisibleItem.Visibility == Visibility.Visible)
                                {
                                    if (i != 0)
                                    {
                                        prevVisibleItem.VerticalLinePartOne.Height = prevVisibleItem.CompleteHeaderElement.DesiredSize.Height / 2;
                                        prevVisibleItem.VerticalLinePartTwo.Visibility = Visibility.Collapsed;
                                        prevVisibleItem.VerticalLinePartOne.VerticalAlignment = VerticalAlignment.Top;
                                        break;
                                    }
                                    else
                                    {
                                        prevVisibleItem.VerticalLinePartOne.Visibility = Visibility.Collapsed;
                                        prevVisibleItem.VerticalLinePartTwo.Visibility = Visibility.Collapsed;
                                        break;
                                    }
                                }
                            }
                        }
                        if ((lastVisibleItem is TreeViewItemAdv) && lastVisibleItem.visiblityFlag)
                        {
                            TreeViewAdv root = (TreeViewAdv)ParentItemsControl.Parent;
                            if (root != null && root.Items.Count > 0)
                            {
                                lastVisibleItem = root.Items[root.Items.Count - 1] as TreeViewItemAdv;
                                if (lastVisibleItem != null && lastVisibleItem.Visibility != Visibility.Visible && root.Items.Count > 1)
                                {
                                    for (int i = root.Items.Count - 2; i >= 0; i--)
                                    {
                                        TreeViewItemAdv prevVisibleItem = root.Items[i] as TreeViewItemAdv;
                                        if (prevVisibleItem != null && prevVisibleItem.Visibility == Visibility.Visible)
                                        {
                                            if (i != 0)
                                            {
                                                prevVisibleItem.VerticalLinePartOne.Height = prevVisibleItem.CompleteHeaderElement.DesiredSize.Height / 2;
                                                prevVisibleItem.VerticalLinePartTwo.Visibility = Visibility.Collapsed;
                                                prevVisibleItem.VerticalLinePartOne.VerticalAlignment = VerticalAlignment.Top;
                                                break;
                                            }
                                            else
                                            {
                                                prevVisibleItem.VerticalLinePartOne.Visibility = Visibility.Collapsed;
                                                prevVisibleItem.VerticalLinePartTwo.Visibility = Visibility.Collapsed;
                                                break;
                                            }
                                        }
                                    }
                                }
                                else if (lastVisibleItem != null && lastVisibleItem.Visibility == Visibility.Visible && root.Items.Count > 1)
                                {
                                    for (int i = root.Items.Count - 2; i > 0; i--)
                                    {
                                        TreeViewItemAdv prevVisibleItem = root.Items[i] as TreeViewItemAdv;
                                        if (prevVisibleItem != null && prevVisibleItem.Visibility == Visibility.Visible)
                                        {
                                            prevVisibleItem.VerticalLinePartOne.Visibility = Visibility.Visible;
                                            prevVisibleItem.VerticalLinePartTwo.Visibility = Visibility.Visible;
                                            prevVisibleItem.VerticalLinePartOne.ClearValue(FrameworkElement.VerticalAlignmentProperty);
                                            prevVisibleItem.VerticalLinePartOne.ClearValue(FrameworkElement.HeightProperty);
                                            prevVisibleItem.VerticalLinePartOne.ClearValue(FrameworkElement.MarginProperty);
                                        }
                                    }
                                }
                            }
                        }
                        if (lastVisibleItem.IsFakeItem)
                        {
                            lastVisibleItem.VerticalLinePartTwo.Visibility = Visibility.Collapsed;
                            lastVisibleItem.VerticalLinePartOne.Height = lastVisibleItem.CompleteHeaderElement.DesiredSize.Height / 2;
                            lastVisibleItem.VerticalLinePartOne.VerticalAlignment = VerticalAlignment.Top;
                            //   lastVisibleItem.VerticalLinePartOne.Margin = new Thickness(0, 0, 0, (lastVisibleItem.DesiredSize.Height / 2) - (lastVisibleItem.CompleteHeaderElement.DesiredSize.Height / 2));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when loaded.
        /// </summary>
        private void TreeViewAdvItemsPanel_Loaded(object sender, RoutedEventArgs e)
        {
            InvalidateRender();
            Loaded -= new RoutedEventHandler(TreeViewAdvItemsPanel_Loaded);
        }

        /// <summary>
        /// Invalidates the render for the element.
        /// </summary>
        protected internal void InvalidateRender()
        {
            InvalidateRenderRequest = !InvalidateRenderRequest;
        }

        #endregion Implementation
    }
}