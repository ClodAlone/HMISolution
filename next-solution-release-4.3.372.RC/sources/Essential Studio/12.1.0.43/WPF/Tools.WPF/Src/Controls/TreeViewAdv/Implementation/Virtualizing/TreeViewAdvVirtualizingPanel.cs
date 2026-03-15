#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using

using System;
using System.Linq;
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
    ///  /// <summary>
    /// An class that provides functionality for virtualizing <see cref="TreeViewItemAdv"/>
    /// of the TreeViewAdv.
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
    /// 			<example><code>public class TreeViewAdvVirtualizingPanel : <see cref="TreeViewAdvItemsPanel"/></code></example>
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
    /// An class that provides functionality for virtualizing <see cref="TreeViewItemAdv"/>
    /// and shows fake items. Using for TreeViewAdv and support IScrollInfo interface.
    /// </remarks>
    /// </summary>
    public class TreeViewAdvVirtualizingPanel : TreeViewAdvItemsPanel, IScrollInfo
    {
        #region Constants

        /// <summary>
        /// Default offset increment for scroll.
        /// </summary>
        //SU I78477
        internal new double c_scrollOffset = 20.0;

        //EU I78477

        #endregion Constants

        #region Members

        /// <summary>
        ///  Value that indicates the expandedtreeviewitem.
        /// </summary>
        private Size m_expandedtreeviewitem = new Size(0, 0);

        internal Visual refVisual;
        internal static bool IsHorizontalScroll = false;
        internal Rect refrectangle = new Rect(0, 0, 0, 0);

        /// <summary>
        /// Value that indicates the expandedchildtreeviewitems.
        /// </summary>
        private Size m_expandedchildtreeviewitems = new Size(0, 0);

        /// <summary>
        /// Value that indicates the viewabletreeviewitem.
        /// </summary>
        private Size m_viewabletreeviewitem = new Size(0, 0);

        /// <summary>
        /// Value that indicates the temproary available size
        /// </summary>
        private static Size m_availablesize = new Size(0, 0);

        /// <summary>
        /// Size of the extent.
        /// </summary>
        internal Size m_extentSize = new Size(0, 0);

        /// <summary>
        /// Parent ItemsControl.
        /// </summary>

        /// <summary>
        /// ScrollViewer element that controls scrolling behavior.
        /// </summary>
        private ScrollViewer m_scrollOwner = null;

        /// <summary>
        /// Value that indicates whether scrolling on the horizontal axis is possible.
        /// </summary>
        private bool m_bCanHorizontallyScroll = false;

        /// <summary>
        /// Value that indicates whether scrolling on the vertical axis is possible.
        /// </summary>
        private bool m_bCanVerticallyScroll = false;

        /// <summary>
        /// Size of the viewport for this content.
        /// </summary>
        private Size m_viewportSize = new Size(0, 0);

        /// <summary>
        /// Offset of the scrolled content.
        /// </summary>
        private Point m_offsetPoint;

        /// <summary>
        /// Transform for scroller.
        /// </summary>
        internal TranslateTransform m_transform = null;

        /// <summary>
        /// Reference to IScrollInfo interface.
        /// </summary>
        private IScrollInfo m_scrollInfo = null;

        /// <summary>
        /// Reference to final expandable item
        /// </summary>
        private TreeViewItemAdv m_finalexpandableitem = new TreeViewItemAdv("nullitem");

        /// <summary>
        /// internal variable which has  height
        /// </summary>
        private double height = 0;

        internal bool IsWindowResized = false;

        /// <summary>
        /// internal variable which has found
        /// </summary>
        private bool isfound = false;

        #endregion Members

        #region Properties

        /// <summary>
        /// Gets reference to IScrollInfo interface.
        /// </summary>
        internal IScrollInfo ScrollInfo
        {
            get
            {
                if (m_scrollInfo == null)
                {
                    m_scrollInfo = this as IScrollInfo;
                }

                return m_scrollInfo;
            }
        }

        #endregion Properties

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewAdvVirtualizingPanel"/> class.
        /// </summary>
        public TreeViewAdvVirtualizingPanel()
        {
            m_transform = new TranslateTransform();
            this.RenderTransform = m_transform;
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Handles the LostMouseCapture event of the m_scrollOwner control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void m_scrollOwner_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            m_scrollmovemanually = false;
        }

        /// <summary>
        /// Raises the Initialized event. This method is invoked whenever IsInitialized
        /// is set to true internally.
        /// </summary>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (ParentItemsControl != null && ParentItemsControl is TreeViewAdv)
            {
                if (ParentTreeView != null && ParentTreeView.Columns != null)
                {
                    ParentTreeView.columnsState.Clear();
                    foreach (TreeViewColumn column in ParentTreeView.Columns)
                    {
                        if (column.State == ColumnMeasureState.Star)
                        {
                            ParentTreeView.columnsState.Add(column);
                        }
                    }
                }
                m_scrollOwner = ParentTreeView.ScrollHost;
                ParentTreeView.m_scrollinfo = this.ScrollInfo;
                if (m_scrollOwner != null)
                {
                    m_scrollOwner.LostMouseCapture += new System.Windows.Input.MouseEventHandler(m_scrollOwner_LostMouseCapture);
                    m_scrollOwner.GotMouseCapture += new System.Windows.Input.MouseEventHandler(m_scrollOwner_GotMouseCapture);
                }
            }
            m_parentItemsControl = ItemsControl.GetItemsOwner(this);
            if ((m_parentItemsControl as TreeViewItemAdv) != null)
            {
                (m_parentItemsControl as TreeViewItemAdv).m_treeviewadvVirtualizingPanel = this;
            }
            if ((m_parentItemsControl as TreeViewAdv) != null)
            {
                (m_parentItemsControl as TreeViewAdv).m_treeviewadvVirtualizingPanel = this;
            }
        }

        /// <summary>
        /// Handles the GotMouseCapture event of the m_scrollOwner control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void m_scrollOwner_GotMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (ParentTreeView != null && ParentTreeView.ItemsSource != null && this.ParentTreeView.m_startSelectContainer != null && this.ParentTreeView.m_startSelectContainer.Header.ToString() == "{DisconnectedItem}")
            {
                this.ParentTreeView.m_startSelectContainer.Header = this.ParentTreeView.m_startSelectContainer.m_treeviewitemactualobject;
            }
        }

        /// <summary>
        /// When overridden in a derived class, measures the size in layout required
        /// for child elements and determines a size for the FrameworkElement-derived class.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give
        /// to child elements. Infinity can be specified as a value to indicate
        /// that the element will size to whatever content is available.</param>
        /// <returns>The size that this element determines it needs during layout,
        /// based on its calculations of child element sizes.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            TreeViewAdv tree = null;
            if (ParentTreeView != null && ParentTreeView.IsLoaded)
            {
                if (ParentTreeView.SortingChangedCount <= 1 && !ParentTreeView.MultiColumnEnable && ParentTreeView.Sorting != SortDirection.None)
                {
                    tree = ParentTreeView;
                    ParentTreeView.SortingTreeView((DependencyObject)tree, tree.newvalue, tree.oldvalue);
                }
            }
            Size size = new Size(0, 0);

            if (ParentTreeView != null && ParentTreeView.IsVirtualizing && ParentTreeView.VirtualizationMode == VirtualizationMode.Extended)
            {
                if (ParentItemsControl != null && ParentItemsControl.IsLoaded && !ParentTreeView.isCalledByChildMeasure)
                {
                    if (!double.IsInfinity(availableSize.Height) && ParentTreeView != null && availableSize.Height > ParentTreeView.tempViewPortHeight)
                    {
                        IsWindowResized = true;
                        ParentTreeView.tempViewPortHeight = availableSize.Height;
                    }
                    else if (!double.IsInfinity(availableSize.Height) && ParentTreeView != null && availableSize.Height < ParentTreeView.tempViewPortHeight)
                    {
                        ParentTreeView.tempViewPortHeight = availableSize.Height;
                    }
                    size = base.MeasureOverride(availableSize);
                }
            }
            else
                size = base.MeasureOverride(availableSize);

            if (availableSize.Width == double.PositiveInfinity)
            {
                availableSize.Width = size.Width;
            }

            if (availableSize.Height == double.PositiveInfinity)
            {
                availableSize.Height = size.Height;
            }
            if (ParentItemsControl is TreeViewItemAdv)
            {
                if ((ParentItemsControl as TreeViewItemAdv).m_bAnimating)
                {
                    availableSize = GetCompletePanelSize(availableSize);
                }
            }
            if (ParentTreeView != null && ParentTreeView.ItemsSource != null && this.ParentTreeView.m_startSelectContainer != null && this.ParentTreeView.m_startSelectContainer.Header.ToString() == "{DisconnectedItem}")
            {
                this.ParentTreeView.m_startSelectContainer.Header = this.ParentTreeView.m_startSelectContainer.m_treeviewitemactualobject;
            }
            if (ParentTreeView != null)
            {
                if (availableSize.Width == double.PositiveInfinity || availableSize.Width == 0)
                {
                    if (m_availablesize.Width == 0)
                    {
                        availableSize.Width = GetCompletePanelSize(availableSize).Width;
                        m_availablesize.Width = availableSize.Width;
                    }
                    availableSize.Width = m_availablesize.Width;
                }
                if (availableSize.Height == double.PositiveInfinity || availableSize.Height == 0)
                {
                    if (m_availablesize.Height == 0)
                    {
                        availableSize.Height = GetCompletePanelSize(availableSize).Height;
                        m_availablesize.Height = availableSize.Height;
                    }
                    availableSize.Height = m_availablesize.Height;
                }

                UpdateScrollInfo(availableSize);
            }

            if (OwnerTreeView != null)
                OwnerTreeView.isPageDown = false;
            return availableSize;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements
        /// and determines a size for a FrameworkElement derived class.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that
        /// this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (ParentTreeView != null)
                finalSize = base.ArrangeOverride(finalSize);

            if (ParentTreeView != null)
            {
                if (ParentTreeView.allowArrange && ParentTreeView.rowPresenter != null && ParentTreeView.rowPresenter.IsLoaded && ParentTreeView.columnsState.Count > 0
                    && ParentTreeView.rowHeaderPresenterCollection.Count > 0 && ParentTreeView.rowPresenterCollection.Count > 0)
                {
                    foreach (TreeViewHeaderRowPresenter header in ParentTreeView.rowHeaderPresenterCollection)
                    {
                        header.ArrangeHeader(finalSize);
                    }
                    foreach (TreeViewRowPresenter row in ParentTreeView.rowPresenterCollection)
                    {
                        row.ArrangeRow(finalSize);
                    }
                }
                ParentTreeView.OnItemGenerated(new RoutedEventArgs(TreeViewAdv.ItemGeneratedEvent, this));
            }
            return finalSize;
        }

       
        /// <summary>
        /// Gets size of the item for MeasureOverride method.
        /// </summary>
        protected override Size GetMeasureItemSize(UIElement item)
        {
            Size size = new Size(0, 0);
            size = base.GetMeasureItemSize(item);
            return size;
        }

        /// <summary>
        /// Gets size of the item for ArrangeOverride method.
        /// </summary>
        protected override Size GetArrangeItemSize(UIElement item)
        {
            Size size = new Size(0, 0);
            size = base.GetArrangeItemSize(item);
            return size;
        }

        /// <summary>
        /// Gets top vertical offset.
        /// </summary>
        protected override double GetTopOffset()
        {
            return ScrollInfo.VerticalOffset;
        }

        /// <summary>
        /// Gets available height.
        /// </summary>
        protected override double GetAvailableHeight(Size availableSize)
        {
            return ScrollInfo.VerticalOffset + availableSize.Height;
        }

        /// <summary>
        /// Gets available width.
        /// </summary>
        protected override double GetAvailableWidth()
        {
            return ScrollInfo.ViewportWidth;
        }

        /// <summary>
        /// Fakes the size of the items.
        /// </summary>
        /// <param name="availableSize">Size of the available.</param>
        /// <param name="isOnlyfakeItemCalculation">if set to <c>true</c> [is onlyfake item calculation].</param>
        /// <returns></returns>
        private Size FakeItemsSize(Size availableSize, bool isOnlyfakeItemCalculation)
        {
            int count = 0;
            availableSize = new Size(0, 0);
            m_expandedtreeviewitem = new Size(0, 0);
            m_expandedchildtreeviewitems = new Size(0, 0);
            m_finalexpandableitem = null;
            if (!m_scrollmovemanually && ParentTreeView != null)
            {
                if (ParentTreeView.m_expandeditem != null)
                {
                    m_finalexpandableitem = ParentTreeView.m_expandeditem;
                }
            }
            else
            {
                m_finalexpandableitem = new TreeViewItemAdv("nullitem");
            }
            foreach (UIElement visibleItem in InternalChildren)
            {
                if (!isOnlyfakeItemCalculation)
                {
                    if (m_finalexpandableitem != null)
                    {
                        if ((visibleItem as TreeViewItemAdv).Equals(m_finalexpandableitem))
                        {
                            count++;
                            m_expandedtreeviewitem.Height = availableSize.Height;
                            m_expandedchildtreeviewitems.Height = availableSize.Height + visibleItem.DesiredSize.Height;
                        }
                    }
                    availableSize.Height += visibleItem.DesiredSize.Height;
                    if ((visibleItem as TreeViewItemAdv) != null && refVisual != null && (refVisual as TreeViewItemAdv) != null)
                    {
                        if ((visibleItem as TreeViewItemAdv).ActualHeight != visibleItem.DesiredSize.Height && !m_scrollmovemanually && ParentTreeView.EditingItem == null)
                        {
                            (refVisual as TreeViewItemAdv).isMakeVisibleCalled = true;
                        }
                    }
                    if (availableSize.Width < visibleItem.DesiredSize.Width)
                    {
                        availableSize.Width = visibleItem.DesiredSize.Width;
                    }
                }
            }
            if (!m_scrollmovemanually && ParentTreeView != null && !isOnlyfakeItemCalculation)
            {
                if (ParentTreeView.m_expandeditem != null && count == 0)
                {
                    if (ParentTreeView.m_expandeditem != null)
                    {
                        m_finalexpandableitem = ParentTreeView.m_expandeditem;
                    }
                    if (m_finalexpandableitem is TreeViewItemAdv)
                    {
                        double tempheight = 0d;
                        TreeViewItemAdv item = m_finalexpandableitem as TreeViewItemAdv;
                        int count1 = 0;
                        TreeViewAdv tree = item.ParentTreeView as TreeViewAdv;
                        m_expandedtreeviewitem = new Size(0, 0);
                        m_expandedchildtreeviewitems = new Size(0, 0);
                        if (item != null && tree != null)
                        {
                            foreach (object obj in tree.Items)
                            {
                                count1++;
                                TreeViewItemAdv titem =
                                    tree.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;
                                if (titem == null)
                                {
                                    titem = obj as TreeViewItemAdv;
                                }
                                if (titem != null)
                                {
                                    tempheight = titem.ActualHeight;
                                    if (titem == item)
                                    {
                                        m_expandedtreeviewitem.Height = height;
                                        if (titem.HasItems)
                                        {
                                            if (titem.IsExpanded)
                                            {
                                                foreach (object obj1 in titem.Items)
                                                {
                                                    TreeViewItemAdv titem1 =
                                                        titem.ItemContainerGenerator.ContainerFromItem(obj) as
                                                        TreeViewItemAdv;
                                                    if (titem1 == null)
                                                    {
                                                        titem1 = obj1 as TreeViewItemAdv;
                                                    }
                                                    if (titem1 != null)
                                                    {
                                                        if (titem1.CompleteHeaderElement != null)
                                                        {
                                                            m_expandedchildtreeviewitems.Height +=
                                                                titem1.CompleteHeaderElement.ActualHeight;
                                                        }
                                                        else
                                                        {
                                                            m_expandedchildtreeviewitems.Height +=
                                                                titem1.RenderSize.Height;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    }

                                    if (titem.HasItems)
                                    {
                                        if (titem.CompleteHeaderElement != null)
                                        {
                                            height += titem.CompleteHeaderElement.ActualHeight;
                                        }
                                        else
                                        {
                                            height += titem.RenderSize.Height;
                                        }

                                        if (titem.IsExpanded && item.Items.Count > 0)
                                        {
                                            IterateItems(titem, item);
                                        }
                                    }
                                    else
                                        height += titem.ActualHeight;

                                    if (isfound)
                                        break;
                                }
                            }
                            height = 0;
                        }
                    }
                }
            }
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
            isfound = false;
            return availableSize;
        }

        /// <summary>
        /// Headers the height of the row.
        /// </summary>
        /// <returns></returns>
        private double HeaderRowHeight()
        {
            if (ParentTreeView != null
               && ParentTreeView.MultiColumnEnable
               && ParentTreeView.HeaderRowPresenter != null)
            {
                return ParentTreeView.HeaderRowPresenter.ActualHeight;
            }

            return 0d;
        }

        /// <summary>
        /// Updates ScrollInfo.
        /// </summary>
        /// <param name="availableSize">Available size.</param>
        internal void UpdateScrollInfo(Size availableSize)
        {
            Size extent = new Size(0, 0);
            double itemHeight = 0d;
            double extentheight = 0d;
            // Size extent = GetCompletePanelSize(availableSize);

            if (ParentTreeView != null && ParentTreeView.IsVirtualizing && ParentTreeView.VirtualizationMode == VirtualizationMode.Normal)
            {
                double listcount = ParentTreeView.LinearList.Count;
                if (ParentTreeView.LinearList.Count > 0)
                {
                    double maximumHeight = 0;
                    var itemsHasSize = ParentTreeView.LinearList.Where(t => t.Value > 0);
                    foreach (var visibleitem in itemsHasSize)
                    {
                        maximumHeight = visibleitem.Value;
                        itemHeight += visibleitem.Value;
                    }
                    int zeroitems = ParentTreeView.LinearList.Count(t => t.Value == 0);
                    itemHeight += maximumHeight * zeroitems;
                }

                if (itemHeight == 0 && ParentTreeView.LinearList.Count > 0)
                {
                    itemHeight += c_scrollOffset * ParentTreeView.LinearList.Count;
                }

                double m_expandeditemsheight = OwnerTreeView.Items.Count * actualheight;
               
                foreach (int indx in OwnerTreeView.ExpandedItemsIndexCollection)
                {
                    if (OwnerTreeView.ExpandedTreeViewAdvItems.ContainsKey(indx))
                    {
                        if (OwnerTreeView.ExpandedTreeViewAdvItems[indx].DesiredSize.Height == 0 && OwnerTreeView.ExpandedTreeViewAdvItems[indx].IsLoaded)
                        {
                            OwnerTreeView.ExpandedTreeViewAdvItems[indx].Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                            m_expandeditemsheight += OwnerTreeView.ExpandedTreeViewAdvItems[indx].DesiredSize.Height;
                        }
                        else
                        {
                            m_expandeditemsheight += OwnerTreeView.ExpandedTreeViewAdvItems[indx].DesiredSize.Height;
                        }
                    }
                }

                if (OwnerTreeView.ScrollHost.ExtentHeight < m_expandeditemsheight)
                {
                    extent.Height = m_expandeditemsheight;
                }
             
                if (!ParentTreeView.m_loaded)
                {
                    if (ParentTreeView.m_itemschanged)
                    {
                        ParentTreeView.m_loaded = true;
                    }
                    if (extent.Height == 0)
                    {
                        if (itemHeight > extent.Height)
                        {
                            extent = new Size(0, 0);
                            if (ParentTreeView.Items.Count > 0)
                            {
                                if (extent.Height + actualheight < m_expandeditemsheight)
                                {
                                    extent.Height = m_expandeditemsheight;
                                }
                            }
                        }
                    }
                    Size fakeItemsSize = FakeItemsSize(availableSize, true);
                    if (extent.Height == 0)
                    {
                        extent.Height += fakeItemsSize.Height;
                    }
                    extent.Width = fakeItemsSize.Width;
                    if (extent.Height < m_expandeditemsheight)
                    {
                        if (ScrollInfo != null)
                        {
                            if (ScrollInfo.ScrollOwner != null && ParentTreeView != null)
                            {
                                if (ScrollInfo.ViewportHeight > 0)
                                {
                                    if (ScrollInfo.ScrollOwner.ViewportHeight < m_expandeditemsheight)
                                    {
                                        extent.Height = m_expandeditemsheight;
                                    }
                                }
                                else
                                {
                                    if (ScrollInfo.ScrollOwner.ExtentHeight <= 0)
                                    {
                                        extent.Height = m_expandeditemsheight;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (extent.Height == 0)
                    {
                        if (itemHeight > extent.Height)
                        {
                            extent = new Size(0, 0);
                            if (ParentTreeView.Items.Count > 0)
                            {
                                if (extent.Height < m_expandeditemsheight)
                                {
                                    extent.Height = m_expandeditemsheight;
                                }
                            }
                        }
                    }
                    Size fakeItemsSize = FakeItemsSize(availableSize, true);
                    if (extent.Height == 0)
                    {
                        extent.Height += fakeItemsSize.Height;
                    }
                    extent.Width = fakeItemsSize.Width;
                    if (extent.Height < m_expandeditemsheight)
                    {
                        if (ScrollInfo != null)
                        {
                            if (ScrollInfo.ScrollOwner != null && ParentTreeView != null)
                            {
                                if (ScrollInfo.ViewportHeight > 0)
                                {
                                    if (ScrollInfo.ScrollOwner.ViewportHeight < m_expandeditemsheight)
                                    {
                                        extent.Height = m_expandeditemsheight;
                                    }
                                }
                                else
                                {
                                    if (ScrollInfo.ScrollOwner.ExtentHeight <= 0)
                                    {
                                        extent.Height = m_expandeditemsheight;
                                    }
                                }
                            }
                        }
                    }
                }
                m_viewportSize = availableSize;
            }
            else if (ParentTreeView != null && ParentTreeView.IsVirtualizing && ParentTreeView.VirtualizationMode == VirtualizationMode.Extended)
            {
                if (ParentTreeView.MultiColumnEnable)
                {
                    Size fakeItemsSize = FakeItemsSize(availableSize, false);
                    m_extentSize.Width = fakeItemsSize.Width;
                }

                if (ParentTreeView != null && ParentTreeView.ItemsSource != null && this.ParentTreeView.m_startSelectContainer != null && this.ParentTreeView.m_startSelectContainer.Header.ToString() == "{DisconnectedItem}")
                {
                    this.ParentTreeView.m_startSelectContainer.Header = this.ParentTreeView.m_startSelectContainer.m_treeviewitemactualobject;
                }
                extentheight = CalculateExtentHeight();

                if (extentheight > availableSize.Height)
                {
                    this.extent.Height = extentheight;
                }
                else
                {
                    this.extent.Height = 0.0;
                }
                m_extentSize.Height = this.extent.Height;
            }

            
            if (ScrollInfo.ScrollOwner != null)
            {
                ScrollInfo.ScrollOwner.InvalidateScrollInfo();
            }

            #region NoVirtualizing

            if (ParentTreeView != null && !ParentTreeView.IsVirtualizing)
            {
                Size fakeItemsSize = FakeItemsSize(availableSize, false);
                extent.Height = fakeItemsSize.Height;
                extent.Width = fakeItemsSize.Width;
                double m_expandeditemsheight = ParentTreeView.Items.Count * actualheight, measuredheight = 0.0;
                foreach (TreeViewItemAdv tree in ParentTreeView.ExpandedItems)
                {
                    if (tree.ExpandedCount != tree.ExpandedItems.Count && tree.ExpandedItems.Count > 0)
                    {
                        tree.ExpandedCount = tree.ExpandedItems.Count;
                        measuredheight = IterateExpandedItems(tree);
                        m_expandeditemsheight += measuredheight;
                    }
                }
                if (extent.Height == 0)
                {
                    if (ParentTreeView.Items.Count > 0)
                    {
                        extent.Height = m_expandeditemsheight;
                    }
                }
                if (ParentTreeView.IsScrollOnExpand)
                {
                    if (!ParentTreeView.bringintoviewstatus)
                    {
                        if (m_expandedtreeviewitem.Height > 0 || m_expandedchildtreeviewitems.Height > 0)
                        {
                            if (m_finalexpandableitem != null)
                            {
                                if (m_finalexpandableitem.Items.Count >= 0)
                                {
                                    m_expandedchildtreeviewitems = new Size(0, 0);
                                    if (m_finalexpandableitem.Items.Count > 0)
                                    {
                                        if (m_finalexpandableitem.CompleteHeaderElement != null)
                                        {
                                            m_expandedchildtreeviewitems.Height =
                                                m_finalexpandableitem.CompleteHeaderElement.ActualHeight;
                                        }
                                        else
                                        {
                                            m_expandedchildtreeviewitems.Height = m_finalexpandableitem.RenderSize.Height;
                                        }
                                        for (int i = 0; i < m_finalexpandableitem.Items.Count; i++)
                                        {
                                            if ((m_finalexpandableitem.Items[i] as TreeViewItemAdv) != null)
                                            {
                                                if ((m_finalexpandableitem.Items[i] as TreeViewItemAdv).CompleteHeaderElement != null)
                                                {
                                                    m_expandedchildtreeviewitems.Height += (m_finalexpandableitem.Items[i] as TreeViewItemAdv).CompleteHeaderElement.ActualHeight;
                                                }
                                                else
                                                {
                                                    m_expandedchildtreeviewitems.Height += (m_finalexpandableitem.Items[i] as TreeViewItemAdv).RenderSize.Height;
                                                }
                                            }
                                        }
                                    }
                                    if (m_expandedchildtreeviewitems.Height != 0)
                                    {
                                        if (m_expandedtreeviewitem.Height < ScrollInfo.VerticalOffset)
                                        {
                                            ScrollInfo.SetVerticalOffset(m_expandedtreeviewitem.Height);
                                        }
                                        else if (m_expandedtreeviewitem.Height > ScrollInfo.VerticalOffset)
                                        {
                                            if (m_expandedchildtreeviewitems.Height > (ScrollInfo.ViewportHeight))
                                            {
                                                if ((m_expandedtreeviewitem.Height + m_expandedchildtreeviewitems.Height) >= ScrollInfo.ExtentHeight && m_scrollOwner != null)
                                                {
                                                    m_scrollOwner.ScrollToEnd();
                                                }
                                                else
                                                {
                                                    ScrollInfo.SetVerticalOffset(m_expandedtreeviewitem.Height + ScrollInfo.ViewportHeight - m_finalexpandableitem.CompleteHeaderElement.RenderSize.Height);
                                                }
                                            }
                                            else if (m_expandedchildtreeviewitems.Height < ScrollInfo.ViewportHeight)
                                            {
                                                ScrollInfo.SetVerticalOffset(m_expandedtreeviewitem.Height + m_expandedchildtreeviewitems.Height - m_finalexpandableitem.CompleteHeaderElement.RenderSize.Height);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (m_finalexpandableitem.CompleteHeaderElement != null)
                                        {
                                            if ((m_expandedtreeviewitem.Height + m_finalexpandableitem.CompleteHeaderElement.ActualHeight > (ScrollInfo.VerticalOffset + ScrollInfo.ViewportHeight)) || m_expandedtreeviewitem.Height < ScrollInfo.VerticalOffset)
                                            {
                                                if (m_scrollOwner != null && m_expandedtreeviewitem.Height + m_finalexpandableitem.CompleteHeaderElement.ActualHeight >= ScrollInfo.ExtentHeight)
                                                {
                                                    m_scrollOwner.ScrollToEnd();
                                                }
                                                else
                                                {
                                                    ScrollInfo.SetVerticalOffset(m_expandedtreeviewitem.Height);
                                                }
                                                if (m_scrollOwner != null && m_expandedtreeviewitem.Height - m_finalexpandableitem.CompleteHeaderElement.ActualHeight <= 0)
                                                {
                                                    m_scrollOwner.ScrollToHome();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((m_expandedtreeviewitem.Height + m_finalexpandableitem.RenderSize.Height > (ScrollInfo.VerticalOffset + ScrollInfo.ViewportHeight)) || m_expandedtreeviewitem.Height < ScrollInfo.VerticalOffset)
                                            {
                                                if (m_scrollOwner != null && m_expandedtreeviewitem.Height + m_finalexpandableitem.RenderSize.Height >= ScrollInfo.ExtentHeight)
                                                {
                                                    m_scrollOwner.ScrollToEnd();
                                                }
                                                else
                                                {
                                                    ScrollInfo.SetVerticalOffset(m_expandedtreeviewitem.Height);
                                                }
                                                if (m_scrollOwner != null && m_expandedtreeviewitem.Height - m_finalexpandableitem.RenderSize.Height <= 0)
                                                {
                                                    m_scrollOwner.ScrollToHome();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion NoVirtualizing

            if (extent != m_extentSize && ParentTreeView.VirtualizationMode == VirtualizationMode.Normal)
            {
                m_extentSize.Height = extent.Height;
                Size fakeItemsSize = FakeItemsSize(availableSize, false);
                if (!IsHorizontalScroll)
                    m_extentSize.Width = fakeItemsSize.Width;
                else if (IsHorizontalScroll && extent.Width != 0)
                    m_extentSize = extent;
                if (ScrollInfo.ScrollOwner != null)
                {
                    ScrollInfo.ScrollOwner.InvalidateScrollInfo();
                }
            }

            if (extent != m_extentSize && ParentTreeView.VirtualizationMode == VirtualizationMode.Extended)
            {
                Size fakeItemsSize = FakeItemsSize(availableSize, false);
                if (!IsHorizontalScroll)
                    m_extentSize.Width = fakeItemsSize.Width;
                else if (IsHorizontalScroll && extent.Width != 0)
                    m_extentSize = extent;

                if (ScrollInfo.ScrollOwner != null)
                {
                    ScrollInfo.ScrollOwner.InvalidateScrollInfo();
                }
            }

            if (m_extentSize.Width - availableSize.Width < ScrollInfo.HorizontalOffset && !IsHorizontalScroll && ParentTreeView.IsVirtualizing && (ParentTreeView.VirtualizationMode == VirtualizationMode.Normal || ParentTreeView.VirtualizationMode == VirtualizationMode.Extended))
            {
                Size fakeItemsSize = FakeItemsSize(availableSize, false);
                m_extentSize.Width = fakeItemsSize.Width;
            }

            if (availableSize != m_viewportSize)
            {
                double verticalOffset = availableSize.Height - m_viewportSize.Height;
                double horizontalOffset = availableSize.Width - m_viewportSize.Width;
                m_viewportSize = availableSize;

                if (ParentTreeView != null)
                {
                    if (ParentTreeView.IsVirtualizing)
                    {
                        if (m_extentSize.Height > availableSize.Height)
                        {
                            if (m_extentSize.Height - availableSize.Height < ScrollInfo.VerticalOffset)
                            {
                                ScrollInfo.SetVerticalOffset(ScrollInfo.VerticalOffset + verticalOffset);
                            }
                        }
                        if (m_extentSize.Width > availableSize.Width)
                        {
                            if (m_extentSize.Width - availableSize.Width < ScrollInfo.HorizontalOffset)
                            {
                                ScrollInfo.SetHorizontalOffset(ScrollInfo.HorizontalOffset + horizontalOffset);
                            }
                        }
                    }
                    else
                    {
                        if (m_extentSize.Height - availableSize.Height < ScrollInfo.VerticalOffset)
                        {
                            ScrollInfo.SetVerticalOffset(ScrollInfo.VerticalOffset + verticalOffset);
                        }
                        if (m_extentSize.Width - availableSize.Width < ScrollInfo.HorizontalOffset)
                        {
                            ScrollInfo.SetHorizontalOffset(ScrollInfo.HorizontalOffset + horizontalOffset);
                        }
                    }
                }
                if (ScrollInfo.ScrollOwner != null)
                {
                    if (ScrollInfo.ScrollOwner.ComputedVerticalScrollBarVisibility == Visibility.Collapsed)
                    {
                        if (ArrangeOffset > 0)
                        {
                            UpdateVisibilityIndex(availableSize);
                            UpdateScrollInfo(availableSize);
                        }
                    }
                    ScrollInfo.ScrollOwner.InvalidateScrollInfo();
                }
            }
        }

        #endregion Implementation

        /// <summary>
        /// calculate the extent height
        /// </summary>
        /// <returns></returns>
        private double CalculateExtentHeight()
        {
            if (ParentTreeView != null)
            {
                double totalHeight = ParentTreeView.Items.Count * ParentTreeView.treeHeight;
                if (totalHeight > ParentTreeView.ExtentHeight)
                    ParentTreeView.ExtentHeight = totalHeight;
                else if (totalHeight == ParentTreeView.ExtentHeight && ParentTreeView.ExtentHeight != 0 && totalHeight != 0)
                    ParentTreeView.ExtentHeight += ParentTreeView.treeHeight;
            }
            return ParentTreeView.ExtentHeight;
        }

        #region IScrollInfo implementation

        /// <summary>
        /// Gets or sets a ScrollViewer element that controls scrolling behavior.
        /// </summary>
        ScrollViewer IScrollInfo.ScrollOwner
        {
            get
            {
                return m_scrollOwner;
            }
            set
            {
                m_scrollOwner = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether scrolling on the horizontal axis is possible.
        /// </summary>
        bool IScrollInfo.CanHorizontallyScroll
        {
            get
            {
                return m_bCanHorizontallyScroll;
            }
            set
            {
                m_bCanHorizontallyScroll = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether scrolling on the vertical axis is possible.
        /// </summary>
        bool IScrollInfo.CanVerticallyScroll
        {
            get
            {
                return m_bCanVerticallyScroll;
            }
            set
            {
                m_bCanVerticallyScroll = value;
            }
        }

        /// <summary>
        /// Gets the vertical size of the extent.
        /// </summary>
        double IScrollInfo.ExtentHeight
        {
            get
            {
                return m_extentSize.Height;
            }
        }

        /// <summary>
        /// Gets the horizontal size of the extent.
        /// </summary>
        double IScrollInfo.ExtentWidth
        {
            get
            {
                return m_extentSize.Width;
            }
        }

        /// <summary>
        /// Gets the vertical size of the viewport for this content.
        /// </summary>
        double IScrollInfo.ViewportHeight
        {
            get
            {
                return m_viewportSize.Height;
            }
        }

        /// <summary>
        /// Gets the horizontal size of the viewport for this content.
        /// </summary>
        double IScrollInfo.ViewportWidth
        {
            get
            {
                return m_viewportSize.Width;
            }
        }

        /// <summary>
        /// Gets the horizontal offset of the scrolled content.
        /// </summary>
        double IScrollInfo.HorizontalOffset
        {
            get
            {
                return m_offsetPoint.X;
            }
        }

        /// <summary>
        /// Gets the vertical offset of the scrolled content.
        /// </summary>
        double IScrollInfo.VerticalOffset
        {
            get
            {
                return m_offsetPoint.Y;
            }
        }

        /// <summary>
        /// Sets the amount of horizontal offset.
        /// </summary>
        /// <param name="offset">The degree to which content is horizontally offset
        /// from the containing viewport.</param>
        void IScrollInfo.SetHorizontalOffset(double offset)
        {
            if (offset < 0 || ScrollInfo.ViewportWidth >= ScrollInfo.ExtentWidth)
            {
                offset = 0;
            }
            else if (offset + ScrollInfo.ViewportWidth >= ScrollInfo.ExtentWidth)
            {
                offset = ScrollInfo.ExtentWidth - ScrollInfo.ViewportWidth;
            }

            m_offsetPoint.X = offset;

            if (ScrollInfo.ScrollOwner != null)
            {
                ScrollInfo.ScrollOwner.InvalidateScrollInfo();
            }

            m_transform.X = -offset;
            InvalidateMeasure();
        }

        private static double oldOffset = 0.0;

        /// <summary>
        /// Sets the amount of vertical offset.
        /// </summary>
        /// <param name="offset">The degree to which content is vertically offset
        /// from the containing viewport.</param>
        void IScrollInfo.SetVerticalOffset(double offset)
        {
            if (oldOffset > offset)
            {
                scrollUp = true;
            }
            else
                scrollUp = false;

            if (ParentTreeView != null)
            {
                if (ParentTreeView.IsVirtualizing)
                {
                    if (ParentTreeView.VirtualizationMode == VirtualizationMode.Normal)
                    {
                        OwnerTreeView.m_itemsexpanded = false;
                        m_previousoffset = ScrollInfo.VerticalOffset;
                        double m_expandeditemsheight = 0.0;
                        TreeViewItemAdv m_previoustree = null;
                        foreach (TreeViewItemAdv tree in ParentTreeView.ExpandedItems)
                        {
                            int treeindx = ParentTreeView.ItemContainerGenerator.IndexFromContainer(tree);
                            if (m_previoustree == null)
                            {
                                if (treeindx != -1)
                                {
                                    m_expandeditemsheight += treeindx * actualheight + tree.Items.Count * actualheight;
                                }
                                else
                                {
                                    treeindx = ParentTreeView.Items.IndexOf(tree);
                                    if (treeindx != -1)
                                    {
                                        m_expandeditemsheight += treeindx * actualheight + tree.Items.Count * actualheight;
                                    }
                                }
                            }
                            else
                            {
                                int previoustreeindx = ParentTreeView.ItemContainerGenerator.IndexFromContainer(m_previoustree);
                                if (treeindx != -1 && previoustreeindx != -1)
                                {
                                    m_expandeditemsheight += (previoustreeindx - treeindx) * actualheight + tree.Items.Count * actualheight;
                                }
                                else
                                {
                                    if (treeindx == -1)
                                    {
                                        treeindx = ParentTreeView.Items.IndexOf(tree);
                                    }
                                    if (previoustreeindx == -1)
                                    {
                                        previoustreeindx = ParentTreeView.Items.IndexOf(m_previoustree);
                                    }
                                    if (treeindx != -1 && previoustreeindx != -1)
                                    {
                                        m_expandeditemsheight += (previoustreeindx - treeindx) * actualheight + tree.Items.Count * actualheight;
                                    }
                                }
                            }
                            m_previoustree = tree;
                        }
                        if (ScrollInfo.ScrollOwner.VerticalOffset >= m_expandeditemsheight - ScrollInfo.ScrollOwner.ViewportHeight)
                        {
                            OwnerTreeView.m_loaded = false;
                        }
                        if (offset < 0 && ScrollInfo.ViewportHeight >= ScrollInfo.ExtentHeight)
                        {
                            offset = 0;
                        }
                        else if (offset + ScrollInfo.ViewportHeight > (ScrollInfo.ExtentHeight + actualheight))
                        {
                            if (ScrollInfo.ExtentHeight > 0)
                            {
                                offset = ScrollInfo.ExtentHeight - ScrollInfo.ViewportHeight;
                            }
                        }
                        if (double.IsNegativeInfinity(offset))
                        {
                            offset = 0;
                        }

                        m_offsetPoint.Y = offset;

                        if (ScrollInfo.ScrollOwner != null)
                        {
                            ScrollInfo.ScrollOwner.InvalidateScrollInfo();
                        }
                        oldOffset = offset;
                        m_transform.Y = -offset;
                        InvalidateMeasure();
                    }
                    else
                    {
                        if (offset < 0 || extent.Height <= ParentTreeView.m_scrollinfo.ViewportHeight)
                        {
                            offset = 0;
                        }
                        else
                        {
                            if (offset + ParentTreeView.m_scrollinfo.ViewportHeight >= extent.Height)
                            {
                                offset = extent.Height - ParentTreeView.m_scrollinfo.ViewportHeight;
                            }
                        }

                        m_offsetPoint.Y = offset;

                        if (ParentTreeView.ScrollHost != null)
                            ParentTreeView.ScrollHost.InvalidateScrollInfo();

                        InvalidateMeasure();
                    }
                }
                else
                {
                    double factor = 0.0;

                    if (double.IsInfinity(offset))
                    {
                        factor = c_scrollOffset;
                    }

                    if (offset < 0 || ScrollInfo.ViewportHeight >= ScrollInfo.ExtentHeight)
                    {
                        offset = 0;
                    }
                    else if (offset + ScrollInfo.ViewportHeight >= ScrollInfo.ExtentHeight)
                    {
                        offset = ScrollInfo.ExtentHeight + factor - ScrollInfo.ViewportHeight;
                    }
                    if (double.IsNegativeInfinity(offset))
                    {
                        offset = 0;
                    }

                    m_offsetPoint.Y = offset;

                    if (ScrollInfo.ScrollOwner != null)
                    {
                        ScrollInfo.ScrollOwner.InvalidateScrollInfo();
                    }
                    oldOffset = offset;
                    m_transform.Y = -offset;
                    InvalidateMeasure();
                }
            }

            if (ParentTreeView != null && (ParentTreeView.VirtualizationMode == VirtualizationMode.Extended || ParentTreeView.VirtualizationMode == VirtualizationMode.Normal))
                TreeViewAdvVirtualizingPanel.IsHorizontalScroll = true;

        }

        /// <summary>
        /// Scrolls up within content by one logical unit.
        /// </summary>
        void IScrollInfo.LineUp()
        {
            if (OwnerTreeView != null && !OwnerTreeView.m_loaded)
            {
                OwnerTreeView.m_itemsexpanded = false;
                OwnerTreeView.m_loaded = true;
            }
            if (ScrollInfo.VerticalOffset - c_scrollOffset > 0)
            {
                ScrollInfo.SetVerticalOffset(ScrollInfo.VerticalOffset - c_scrollOffset);
            }
            else
            {
                ScrollInfo.SetVerticalOffset(0);
            }
        }

        /// <summary>
        /// Scrolls down within content by one logical unit.
        /// </summary>
        void IScrollInfo.LineDown()
        {
            if (OwnerTreeView != null && OwnerTreeView.IsVirtualizing && OwnerTreeView.m_loaded && OwnerTreeView.VirtualizationMode == VirtualizationMode.Normal)
            {
                OwnerTreeView.m_itemsexpanded = false;
                double m_expandeditemsheight = 0.0;
                TreeViewItemAdv m_previoustree = null;
                foreach (TreeViewItemAdv tree in OwnerTreeView.ExpandedItems)
                {
                    int treeindx = OwnerTreeView.ItemContainerGenerator.IndexFromContainer(tree);
                    if (m_previoustree == null)
                    {
                        if (treeindx != -1)
                        {
                            m_expandeditemsheight += treeindx * actualheight + tree.Items.Count * actualheight;
                        }
                        else
                        {
                            treeindx = OwnerTreeView.Items.IndexOf(tree);
                            if (treeindx != -1)
                            {
                                m_expandeditemsheight += treeindx * actualheight + tree.Items.Count * actualheight;
                            }
                        }
                    }
                    else
                    {
                        int previoustreeindx = OwnerTreeView.ItemContainerGenerator.IndexFromContainer(m_previoustree);
                        if (treeindx != -1 && previoustreeindx != -1)
                        {
                            m_expandeditemsheight += (previoustreeindx - treeindx) * actualheight + tree.Items.Count * actualheight;
                        }
                        else
                        {
                            if (treeindx == -1)
                            {
                                treeindx = OwnerTreeView.Items.IndexOf(tree);
                            }
                            if (previoustreeindx == -1)
                            {
                                previoustreeindx = OwnerTreeView.Items.IndexOf(m_previoustree);
                            }
                            if (treeindx != -1 && previoustreeindx != -1)
                            {
                                m_expandeditemsheight += (previoustreeindx - treeindx) * actualheight + tree.Items.Count * actualheight;
                            }
                        }
                    }
                    m_previoustree = tree;
                }
                if (ScrollInfo.ScrollOwner.VerticalOffset >= m_expandeditemsheight - ScrollInfo.ScrollOwner.ViewportHeight)
                {
                    OwnerTreeView.m_loaded = false;
                }
            }
            ScrollInfo.SetVerticalOffset(ScrollInfo.VerticalOffset + c_scrollOffset);
        }

        /// <summary>
        /// Scrolls up within content by one page.
        /// </summary>
        void IScrollInfo.PageUp()
        {
            if (OwnerTreeView != null && !OwnerTreeView.m_loaded)
            {
                OwnerTreeView.m_itemsexpanded = false;
                OwnerTreeView.m_loaded = true;
            }
            if (ScrollInfo.VerticalOffset - ScrollInfo.ViewportHeight > 0)
            {
                ScrollInfo.SetVerticalOffset(ScrollInfo.VerticalOffset - ScrollInfo.ViewportHeight);
            }
            else
            {
                ScrollInfo.SetVerticalOffset(0);
            }
        }

        /// <summary>
        /// Scrolls down within content by one page.
        /// </summary>
        void IScrollInfo.PageDown()
        {
            if (OwnerTreeView != null && OwnerTreeView.IsVirtualizing && OwnerTreeView.m_loaded)
            {
                OwnerTreeView.isPageDown = true;
                OwnerTreeView.m_itemsexpanded = false;
                double m_expandeditemsheight = 0.0;
                TreeViewItemAdv m_previoustree = null;
                foreach (TreeViewItemAdv tree in OwnerTreeView.ExpandedItems)
                {
                    int treeindx = OwnerTreeView.ItemContainerGenerator.IndexFromContainer(tree);
                    if (m_previoustree == null)
                    {
                        if (treeindx != -1)
                        {
                            m_expandeditemsheight += treeindx * actualheight + tree.Items.Count * actualheight;
                        }
                        else
                        {
                            treeindx = OwnerTreeView.Items.IndexOf(tree);
                            if (treeindx != -1)
                            {
                                m_expandeditemsheight += treeindx * actualheight + tree.Items.Count * actualheight;
                            }
                        }
                    }
                    else
                    {
                        int previoustreeindx = OwnerTreeView.ItemContainerGenerator.IndexFromContainer(m_previoustree);
                        if (treeindx != -1 && previoustreeindx != -1)
                        {
                            m_expandeditemsheight += (previoustreeindx - treeindx) * actualheight + tree.Items.Count * actualheight;
                        }
                        else
                        {
                            if (treeindx == -1)
                            {
                                treeindx = OwnerTreeView.Items.IndexOf(tree);
                            }
                            if (previoustreeindx == -1)
                            {
                                previoustreeindx = OwnerTreeView.Items.IndexOf(m_previoustree);
                            }
                            if (treeindx != -1 && previoustreeindx != -1)
                            {
                                m_expandeditemsheight += (previoustreeindx - treeindx) * actualheight + tree.Items.Count * actualheight;
                            }
                        }
                    }
                    m_previoustree = tree;
                }
                if (ScrollInfo.ScrollOwner.VerticalOffset >= m_expandeditemsheight - ScrollInfo.ScrollOwner.ViewportHeight)
                {
                    OwnerTreeView.m_loaded = false;
                }
            }
            ScrollInfo.SetVerticalOffset(ScrollInfo.VerticalOffset + ScrollInfo.ViewportHeight);
        }

        /// <summary>
        /// Scrolls up within content after a user clicks the wheel button on a mouse.
        /// </summary>
        void IScrollInfo.MouseWheelUp()
        {
            if (OwnerTreeView != null && !OwnerTreeView.m_loaded)
            {
                OwnerTreeView.m_itemsexpanded = false;
                OwnerTreeView.m_loaded = true;
            }
            if (OwnerTreeView != null && OwnerTreeView.ItemsSource != null && this.OwnerTreeView.m_startSelectContainer != null && this.OwnerTreeView.m_startSelectContainer.Header.ToString() == "{DisconnectedItem}")
            {
                int dragindex = OwnerTreeView.ItemContainerGenerator.IndexFromContainer(OwnerTreeView.start_treeitem);
                int selectindex = 0;
                if (dragindex > 0)
                    selectindex = dragindex - 1;
                else
                    dragindex = 0;
                if (OwnerTreeView.ItemContainerGenerator.ContainerFromIndex(selectindex) != null)
                    OwnerTreeView.dropSelectContainer = OwnerTreeView.ItemContainerGenerator.ContainerFromIndex(selectindex) as TreeViewItemAdv;
            }
            m_scrollmovemanually = true;
            if (ScrollInfo.VerticalOffset - c_scrollOffset > 0)
            {
                ScrollInfo.SetVerticalOffset(ScrollInfo.VerticalOffset - c_scrollOffset);
            }
            else
            {
                ScrollInfo.SetVerticalOffset(0);
            }
            if (OwnerTreeView != null)
                OwnerTreeView.Focus();
        }

        /// <summary>
        /// Scrolls down within content after a user clicks the wheel button on a mouse.
        /// </summary>
        void IScrollInfo.MouseWheelDown()
        {
            m_scrollmovemanually = true;
            if (OwnerTreeView != null && OwnerTreeView.IsVirtualizing && OwnerTreeView.m_loaded)
            {
                if (this.OwnerTreeView.m_startSelectContainer != null && OwnerTreeView.ItemsSource != null && this.OwnerTreeView.m_startSelectContainer.Header.ToString() == "{DisconnectedItem}")
                {
                    this.OwnerTreeView.m_startSelectContainer.Header = this.OwnerTreeView.m_startSelectContainer.m_treeviewitemactualobject;
                }
                if (OwnerTreeView.m_selectedContainers.Count != 0)
                {
                    OwnerTreeView.start_treeitem = (TreeViewItemAdv)OwnerTreeView.m_selectedContainers[0];
                }
                if (OwnerTreeView != null && OwnerTreeView.start_treeitem != null)
                {
                    int dragindex = OwnerTreeView.ItemContainerGenerator.IndexFromContainer(OwnerTreeView.start_treeitem);
                    int selectindex = 0;
                    if (dragindex > 0)
                        selectindex = dragindex - 1;
                    else
                        dragindex = 0;
                    if (OwnerTreeView.ItemContainerGenerator.ContainerFromIndex(selectindex) != null)
                        OwnerTreeView.dropSelectContainer = OwnerTreeView.ItemContainerGenerator.ContainerFromIndex(selectindex) as TreeViewItemAdv;
                }
                OwnerTreeView.m_itemsexpanded = false;
                double m_expandeditemsheight = OwnerTreeView.Items.Count * actualheight;
                foreach (TreeViewItemAdv tree in OwnerTreeView.ExpandedItems)
                {
                    m_expandeditemsheight += tree.Items.Count * actualheight + actualheight;
                    
                }
                if (ScrollInfo.ScrollOwner.VerticalOffset >= (m_expandeditemsheight - ScrollInfo.ScrollOwner.ViewportHeight))
                {
                    OwnerTreeView.m_loaded = false;
                }
            }
            if (OwnerTreeView != null)
                OwnerTreeView.Focus();
            ScrollInfo.SetVerticalOffset(ScrollInfo.VerticalOffset + c_scrollOffset);
        }

        /// <summary>
        /// Scrolls left within content by one logical unit.
        /// </summary>
        void IScrollInfo.LineLeft()
        {
            if (OwnerTreeView != null && !OwnerTreeView.m_loaded)
            {
                OwnerTreeView.m_itemsexpanded = false;
                OwnerTreeView.m_loaded = true;
            }
            ScrollInfo.SetHorizontalOffset(ScrollInfo.HorizontalOffset - c_scrollOffset);
        }

        /// <summary>
        /// Scrolls right within content by one logical unit.
        /// </summary>
        void IScrollInfo.LineRight()
        {
            if (OwnerTreeView != null && !OwnerTreeView.m_loaded)
            {
                OwnerTreeView.m_itemsexpanded = false;
                OwnerTreeView.m_loaded = true;
            }
            ScrollInfo.SetHorizontalOffset(ScrollInfo.HorizontalOffset + c_scrollOffset);
        }

        /// <summary>
        /// Forces content to scroll until the coordinate space of a Visual object is visible.
        /// </summary>
        /// <param name="visual">A Visual that becomes visible.</param>
        /// <param name="rectangle">A bounding rectangle that identifies the coordinate space
        /// to make visible.</param>
        /// <returns>A Rect that is visible.</returns>
        Rect IScrollInfo.MakeVisible(Visual visual, Rect rectangle)
        {
            if (visual is TreeViewItemAdv)
            {
                double tempheight = 0d;
                double actualcalculatedheight = 0, ss = 0;
                TreeViewItemAdv item = visual as TreeViewItemAdv;
                int count = 0;
                TreeViewAdv tree = item.ParentTreeView as TreeViewAdv;
                m_expandedtreeviewitem = new Size(0, 0);
                m_expandedchildtreeviewitems = new Size(0, 0);
                if (item != null && item.isMakeVisibleCalled && tree != null)
                {
                    if (tree.IsVirtualizing)
                    {
                        foreach (object obj in tree.Items)
                        {
                            count++;
                            TreeViewItemAdv titem = tree.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;
                            if (titem == null)
                            {
                                titem = obj as TreeViewItemAdv;
                            }
                            if (titem != null)
                            {
                                tempheight = titem.ActualHeight;
                                if (titem == item)
                                {
                                    m_expandedtreeviewitem.Height = height;
                                    if (titem.HasItems)
                                    {
                                        if (titem.IsExpanded)
                                        {
                                            foreach (object obj1 in titem.Items)
                                            {
                                                TreeViewItemAdv titem1 =
                                                    titem.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;
                                                if (titem1 == null)
                                                {
                                                    titem1 = obj1 as TreeViewItemAdv;
                                                }
                                                if (titem1 != null)
                                                {
                                                    if (titem1.CompleteHeaderElement != null)
                                                    {
                                                        m_expandedchildtreeviewitems.Height +=
                                                            titem1.CompleteHeaderElement.ActualHeight;
                                                    }
                                                    else
                                                    {
                                                        m_expandedchildtreeviewitems.Height += titem1.RenderSize.Height;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }

                                if (titem.HasItems)
                                {
                                    if (titem.CompleteHeaderElement != null)
                                    {
                                        height += titem.CompleteHeaderElement.ActualHeight;
                                    }
                                    else
                                    {
                                        height += titem.RenderSize.Height;
                                    }

                                    if (titem.IsExpanded)
                                    {
                                        IterateItems(titem, item);
                                    }
                                }
                                else
                                    height += titem.ActualHeight;

                                if (isfound)
                                    break;
                            }
                            else
                            {
                                if (item.CompleteHeaderElement != null)
                                {
                                    height += item.CompleteHeaderElement.ActualHeight;
                                }
                                else
                                {
                                    height += item.RenderSize.Height;
                                }
                            }

                            actualcalculatedheight = height;
                            ss = tree.ScrollHost.ViewportHeight;
                        }
                    }
                    else
                    {
                        foreach (object obj in tree.Items)
                        {
                            count++;
                            TreeViewItemAdv titem = tree.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;
                            if (titem == null)
                            {
                                titem = obj as TreeViewItemAdv;
                            }
                            if (titem != null)
                            {
                                tempheight = titem.ActualHeight;
                                if (titem == item)
                                {
                                    m_expandedtreeviewitem.Height = height;
                                    if (titem.HasItems)
                                    {
                                        if (titem.IsExpanded)
                                        {
                                            foreach (object obj1 in titem.Items)
                                            {
                                                TreeViewItemAdv titem1 =
                                                    titem.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;
                                                if (titem1 == null)
                                                {
                                                    titem1 = obj1 as TreeViewItemAdv;
                                                }
                                                if (titem1 != null)
                                                {
                                                    if (titem1.CompleteHeaderElement != null)
                                                    {
                                                        m_expandedchildtreeviewitems.Height +=
                                                            titem1.CompleteHeaderElement.ActualHeight;
                                                    }
                                                    else
                                                    {
                                                        m_expandedchildtreeviewitems.Height += titem1.RenderSize.Height;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }

                                if (titem.HasItems)
                                {
                                    if (titem.CompleteHeaderElement != null)
                                    {
                                        height += titem.CompleteHeaderElement.ActualHeight;
                                    }
                                    else
                                    {
                                        height += titem.RenderSize.Height;
                                    }

                                    if (titem.IsExpanded)
                                    {
                                        IterateItems(titem, item);
                                    }
                                }
                                else
                                    height += titem.ActualHeight;

                                if (isfound)
                                    break;
                            }

                            actualcalculatedheight = height;
                            ss = tree.ScrollHost.ViewportHeight;
                        }
                    }
                    if (m_expandedtreeviewitem.Height > 0 || m_expandedchildtreeviewitems.Height > 0)
                    {
                        if (item != null)
                        {
                            if (item.Items.Count >= 0)
                            {
                                m_expandedchildtreeviewitems = new Size(0, 0);
                                if (item.Items.Count > 0)
                                {
                                    if (item.CompleteHeaderElement != null)
                                    {
                                        m_expandedchildtreeviewitems.Height = item.CompleteHeaderElement.ActualHeight;
                                    }
                                    else
                                    {
                                        m_expandedchildtreeviewitems.Height = item.RenderSize.Height;
                                    }
                                    for (int i = 0; i < item.Items.Count; i++)
                                    {
                                        if ((item.Items[i] as TreeViewItemAdv) != null)
                                        {
                                            if ((item.Items[i] as TreeViewItemAdv).CompleteHeaderElement != null)
                                            {
                                                m_expandedchildtreeviewitems.Height += (item.Items[i] as TreeViewItemAdv).CompleteHeaderElement.ActualHeight;
                                            }
                                            else
                                            {
                                                m_expandedchildtreeviewitems.Height += (item.Items[i] as TreeViewItemAdv).RenderSize.Height;
                                            }
                                        }
                                    }
                                }
                                if (m_expandedchildtreeviewitems.Height != 0)
                                {
                                    if (m_expandedtreeviewitem.Height < ScrollInfo.VerticalOffset)
                                    {
                                        ScrollInfo.SetVerticalOffset(m_expandedtreeviewitem.Height);
                                    }
                                    else if (m_expandedtreeviewitem.Height > ScrollInfo.VerticalOffset)
                                    {
                                        if (m_expandedchildtreeviewitems.Height > (ScrollInfo.ViewportHeight))
                                        {
                                            if ((m_expandedtreeviewitem.Height + m_expandedchildtreeviewitems.Height) >= ScrollInfo.ExtentHeight)
                                            {
                                                m_scrollOwner.ScrollToEnd();
                                            }
                                            else
                                            {
                                                ScrollInfo.SetVerticalOffset(m_expandedtreeviewitem.Height + ScrollInfo.ViewportHeight);
                                            }
                                        }
                                        else if ((m_expandedtreeviewitem.Height + m_expandedchildtreeviewitems.Height) > ScrollInfo.ViewportHeight && item.IsExpanded)
                                        {
                                            ScrollInfo.SetVerticalOffset(m_expandedtreeviewitem.Height - m_expandedchildtreeviewitems.Height);
                                        }
                                        else if (m_expandedchildtreeviewitems.Height < ScrollInfo.ViewportHeight && !ParentTreeView.m_wasSelectedByMouseClick)
                                        {
                                            ScrollInfo.SetVerticalOffset(m_expandedtreeviewitem.Height + m_expandedchildtreeviewitems.Height);
                                        }
                                    }
                                }
                                else
                                {
                                    if (item.CompleteHeaderElement != null)
                                    {
                                        if ((m_expandedtreeviewitem.Height + item.CompleteHeaderElement.ActualHeight > (ScrollInfo.VerticalOffset + ScrollInfo.ViewportHeight)) || m_expandedtreeviewitem.Height < ScrollInfo.VerticalOffset)
                                        {
                                            if (m_expandedtreeviewitem.Height + item.CompleteHeaderElement.ActualHeight >= ScrollInfo.ExtentHeight)
                                            {
                                                m_scrollOwner.ScrollToEnd();
                                            }
                                            else
                                            {
                                                ScrollInfo.SetVerticalOffset(m_expandedtreeviewitem.Height);
                                            }
                                            if (m_expandedtreeviewitem.Height - item.CompleteHeaderElement.ActualHeight <= 0)
                                            {
                                                m_scrollOwner.ScrollToHome();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if ((m_expandedtreeviewitem.Height + item.RenderSize.Height > (ScrollInfo.VerticalOffset + ScrollInfo.ViewportHeight)) || m_expandedtreeviewitem.Height < ScrollInfo.VerticalOffset)
                                        {
                                            if (m_expandedtreeviewitem.Height + item.RenderSize.Height >= ScrollInfo.ExtentHeight)
                                            {
                                                m_scrollOwner.ScrollToEnd();
                                            }
                                            else
                                            {
                                                ScrollInfo.SetVerticalOffset(m_expandedtreeviewitem.Height);
                                            }
                                            if (m_expandedtreeviewitem.Height - item.RenderSize.Height <= 0)
                                            {
                                                m_scrollOwner.ScrollToHome();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (m_scrollOwner != null && !ParentTreeView.IsScrollOnExpand && !ParentTreeView.IsVirtualizing)
                        {
                            m_scrollOwner.InvalidateScrollInfo();
                        }
                        else if (m_scrollOwner != null)
                            m_scrollOwner.ScrollToHome();
                    }

                    item.isMakeVisibleCalled = false;
                }
            }

            refrectangle = rectangle;
            refVisual = visual;
            isfound = false;
            height = 0;
            return rectangle;
        }

        //Iterates the items
        /// <summary>
        /// Iterates the items.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="item">The item.</param>
        private void IterateItems(TreeViewItemAdv tree, TreeViewItemAdv item)
        {
            if (tree.Items != null)
            {
                foreach (object obj in tree.Items)
                {
                    TreeViewItemAdv titem = tree.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;
                    if (titem == null)
                    {
                        titem = obj as TreeViewItemAdv;
                    }
                    if (titem != null)
                    {
                        if (titem == item)
                        {
                            m_expandedtreeviewitem.Height = height;
                            if (titem.HasItems)
                            {
                                if (titem.IsExpanded)
                                {
                                    foreach (object obj1 in titem.Items)
                                    {
                                        TreeViewItemAdv titem1 =
                                            titem.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;
                                        if (titem1 == null)
                                        {
                                            titem1 = obj1 as TreeViewItemAdv;
                                        }
                                        if (titem1 != null)
                                        {
                                            if (titem.CompleteHeaderElement != null && titem1.CompleteHeaderElement != null)
                                            {
                                                m_expandedchildtreeviewitems.Height +=
                                                    titem1.CompleteHeaderElement.ActualHeight;
                                            }
                                            else
                                            {
                                                m_expandedchildtreeviewitems.Height += titem1.RenderSize.Height;
                                            }
                                        }
                                    }
                                }
                            }
                            isfound = true;
                            break;
                        }

                        if (titem.HasItems)
                        {
                            if (titem.CompleteHeaderElement != null)
                            {
                                height += titem.CompleteHeaderElement.ActualHeight;
                            }
                            else
                            {
                                height += titem.RenderSize.Height;
                            }

                            if (titem.IsExpanded)
                            {
                                IterateItems(titem, item);
                            }
                        }
                        else
                        {
                            height += titem.ActualHeight;
                        }
                    }
                    else
                    {
                        if (item.CompleteHeaderElement != null)
                        {
                            height += item.CompleteHeaderElement.ActualHeight;
                        }
                        else
                        {
                            height += item.RenderSize.Height;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Scrolls left within content after a user clicks the wheel button on a mouse.
        /// </summary>
        void IScrollInfo.MouseWheelLeft()
        {
            ScrollInfo.SetHorizontalOffset(ScrollInfo.HorizontalOffset - c_scrollOffset);
        }

        /// <summary>
        /// Scrolls right within content after a user clicks the wheel button on a mouse.
        /// </summary>
        void IScrollInfo.MouseWheelRight()
        {
            ScrollInfo.SetHorizontalOffset(ScrollInfo.HorizontalOffset + c_scrollOffset);
        }

        /// <summary>
        /// Scrolls left within content by one page.
        /// </summary>
        void IScrollInfo.PageLeft()
        {
            ScrollInfo.SetHorizontalOffset(ScrollInfo.HorizontalOffset - ScrollInfo.ViewportWidth);
        }

        /// <summary>
        /// Scrolls right within content by one page.
        /// </summary>
        void IScrollInfo.PageRight()
        {
            ScrollInfo.SetHorizontalOffset(ScrollInfo.HorizontalOffset + ScrollInfo.ViewportWidth);
        }

        #endregion IScrollInfo implementation
    }
}