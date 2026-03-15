#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.

#endregion Copyright Syncfusion Inc. 2001 - 2009

#region file using

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// An abstract base class that provides functionality
    /// for virtualizing items which have vertical orientation.
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
    /// 			<example><code>public abstract class FakeItemsPanel : <see cref="VirtualizingPanel"/></code></example>
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
    /// An abstract base class that provides functionality
    /// for virtualizing items which have vertical orientation and shows fake items.
    /// </remarks>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class FakeItemsPanel : VirtualizingPanel
    {
        #region Members

        /// <summary>
        /// Collection of the fake item.
        /// </summary>
        private static FakeElementCollection m_fakeItems = new FakeElementCollection();

        /// <summary>
        /// Collection of the original fake item.
        /// </summary>
        private static ObjectCollection m_originalFakeItems = new ObjectCollection();

        /// <summary>
        /// Parent items control.
        /// </summary>
        internal ItemsControl m_parentItemsControl = null;

        /// <summary>
        /// Last position of the fake items.
        /// </summary>
        //private int m_lastIndexFakeItems = -1;
        /// <summary>
        /// The item index of the first visible item.
        /// </summary>
        private int m_firstVisibleItemIndex = 0;

        /// <summary>
        /// The item index of the last visible item.
        /// </summary>
        private int m_lastVisibleItemIndex = 0;

        /// <summary>
        /// Offset for arrange.
        /// </summary>
        private double m_arrangeOffset = 0;

        /// <summary>
        /// Hashtable with measured size of the items.
        /// </summary>
        internal Hashtable m_cashedMeasureSize = new Hashtable();

        /// <summary>
        /// Parent ItemsControl.
        /// </summary>
        internal TreeViewAdv m_ownerTreeView = null;

        /// <summary>
        /// internal variable which has scroll offset
        /// </summary>
        protected const int c_scrollOffset = 20;

        internal bool scrollUp = false;

        #endregion Members

        #region IVirtualTree

        /// <summary>
        /// Default ScrollOffset
        /// </summary>
        private const double C_ScrollOffset = 20.0;

        /// <summary>
        /// Stores Parent Items control
        /// </summary>
        private ItemsControl parentItemsControl;

        /// <summary>
        /// Top offset value of panel that above the Viewport
        /// </summary>
        private TranslateTransform transform = new TranslateTransform();

        /// <summary>
        /// Stores total Children Size.
        /// </summary>
        private Size childrenSize;

        /// <summary>
        /// Total extent height of the Viewport
        /// </summary>
        internal Size extent;

        /// <summary>
        /// First and Last Visible item index in the viewport area
        /// </summary>
        internal int firstVisibleIndex = 0, lastVisibleIndex = 0;

        /// <summary>
        /// Root TreeViewAdv.
        /// </summary>
        private TreeViewAdv parentTreeView;

        /// <summary>
        /// used only with virtualization expanding last item.
        /// </summary>
        private bool IsLastItemExpanded = false;

        /// <summary>
        /// Root TreeViewAdv.
        /// </summary>
        internal TreeViewAdv ParentTreeView
        {
            get
            {
                if (parentTreeView == null)
                {
                    parentTreeView = VisualUtils.FindAncestor(this, typeof(TreeViewAdv)) as TreeViewAdv;
                }
                return parentTreeView;
            }
        }

        #endregion IVirtualTree

        #region Properties

        /// <summary>
        /// Gets or sets owner TreeView.
        /// </summary>
        internal TreeViewAdv OwnerTreeView
        {
            get
            {
                return m_ownerTreeView;
            }
        }

        /// <summary>
        /// Gets collection of the fake item.
        /// </summary>
        internal static FakeElementCollection FakeItems
        {
            get
            {
                return m_fakeItems;
            }
        }

        /// <summary>
        /// Gets collection of the fake item.
        /// </summary>
        internal static ObjectCollection OriginalFakeItems
        {
            get
            {
                return m_originalFakeItems;
            }
        }

        /// <summary>
        /// Gets parent items control.
        /// </summary>
        internal ItemsControl ParentItemsControl
        {
            get
            {
                return m_parentItemsControl;
            }
        }

        /// <summary>
        /// Gets or sets the item index of the first visible item.
        /// </summary>
        internal int FirstVisibleItemIndex
        {
            get
            {
                return m_firstVisibleItemIndex;
            }
            set
            {
                if (value != m_firstVisibleItemIndex)
                {
                    m_firstVisibleItemIndex = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the item index of the first visible item.
        /// </summary>
        internal int LastVisibleItemIndex
        {
            get
            {
                return m_lastVisibleItemIndex;
            }
            set
            {
                if (value != m_lastVisibleItemIndex)
                {
                    m_lastVisibleItemIndex = value;
                }
            }
        }

        /// <summary>
        /// Offset for arrange.
        /// </summary>
        internal double ArrangeOffset
        {
            get
            {
                return m_arrangeOffset;
            }
            set
            {
                if (value != m_arrangeOffset)
                {
                    m_arrangeOffset = value;
                }
            }
        }

        /// <summary>
        /// Gets count of the items and fake items.
        /// </summary>
        internal int CountInternalItems
        {
            get
            {
                return InternalChildren.Count;
            }
        }

        internal bool IsLineDown
        {
            get;
            set;
        }

        internal bool IsLineUp { get; set; }

        #endregion Properties

        #region Initialization

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FakeItemsPanel()
        {
            this.Loaded += new RoutedEventHandler(FakeItemsPanel_Loaded);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FakeItemsPanel_Loaded(object sender, RoutedEventArgs e)
        {
            m_parentItemsControl = ItemsControl.GetItemsOwner(this);
            parentItemsControl = ItemsControl.GetItemsOwner(this);

            if ((m_parentItemsControl as TreeViewItemAdv) != null)
            {
                (m_parentItemsControl as TreeViewItemAdv).m_virtualizingpanel = this as VirtualizingPanel;
                (m_parentItemsControl as TreeViewItemAdv).m_fakeItemsPanel = this;
            }
            if ((m_parentItemsControl as TreeViewAdv) != null)
            {
                (m_parentItemsControl as TreeViewAdv).m_virtualizingpanel = this as VirtualizingPanel;
                (m_parentItemsControl as TreeViewAdv).m_fakeItemsPanel = this;
            }
        }

        #endregion Initialization

        #region Public Methods

        /// <summary>
        /// Adds fake item.
        /// </summary>
        /// <param name="element">Fake item.</param>
        public void AddFakeItems(object element)
        {
            if (element != null && element is ICloneable
                && !OriginalFakeItems.Contains(element))
            {
                ICloneable clone = (ICloneable)element;
                UIElement container = GetContainerForFakeElement(clone.Clone());

                if (container != null)
                {
                    FakeItems.Add(container);
                    OriginalFakeItems.Add(element);
                }
            }

            OnFakeItemsChanged(ParentItemsControl);
        }

        /// <summary>
        /// Adds fake items.
        /// </summary>
        /// <param name="list">Fake items.</param>
        public void AddFakeItems(IList list)
        {
            if (list != null)
            {
                object element = null;
                UIElement container = null;

                for (int i = 0; i < list.Count; i++)
                {
                    element = list[i];
                    ICloneable clone = (ICloneable)element;

                    if (clone != null && !OriginalFakeItems.Contains(element))
                    {
                        container = GetContainerForFakeElement(clone.Clone());

                        if (container != null)
                        {
                            FakeItems.Add(container);
                            OriginalFakeItems.Add(element);
                        }
                    }
                }
            }

            OnFakeItemsChanged(ParentItemsControl);
        }

        /// <summary>
        /// Removes all fake items.
        /// </summary>
        public static void RemoveFakeItems()
        {
            FakeItems.Clear();
            OriginalFakeItems.Clear();
        }

        /// <summary>
        /// Shows fake items.
        /// </summary>
        /// <paparam name="element">Element for which will show fake items.</paparam>
        /// <param name="bIsTop">Indicates whether fake items will show over element;
        /// otherwise fake items will show under element.</param>
        public virtual void ShowFakeItems(UIElement element, bool bIsTop)
        {
            RemoveInternalFakeItems();
            UIElement fakeElement = null;

            if (element != null)
            {
                int index = InternalChildren.IndexOf(element);

                if (index > -1)
                {
                    index = (bIsTop) ? index : ((index < InternalChildren.Count) ? index + 1 : index);
                    TreeViewAdv.dropIndex = index;
                    for (int i = 0; i < FakeItems.Count; i++)
                    {
                        fakeElement = FakeItems[i];
                        base.InsertInternalChild(index + i, fakeElement);
                        fakeElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    TreeViewItemAdv lastItem = InternalChildren[InternalChildren.Count - 1] as TreeViewItemAdv;
                    if (lastItem != null && lastItem.VerticalLinePartTwo != null)
                    {
                        if (ParentItemsControl is TreeViewAdv)
                        {
                            InvalidatePanelMeasure(ParentItemsControl);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < FakeItems.Count; i++)
                {
                    fakeElement = FakeItems[i];
                    base.AddInternalChild(fakeElement);

                    fakeElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }
            }

            InvalidateMeasure();
        }

        /// <summary>
        /// Invalidates the panel measure.
        /// </summary>
        /// <param name="items">The items.</param>
        internal void InvalidatePanelMeasure(ItemsControl items)
        {
            if (items != null && items.Visibility != Visibility.Collapsed)
            {
                IItemsPanelRef panel = items as IItemsPanelRef;
                TreeViewAdvItemsPanel itemsPanel = (panel != null) ? panel.ItemsPanel : null;

                if (panel != null && itemsPanel != null)
                {
                    if (itemsPanel.Children.Count > 0)
                    {
                        itemsPanel.InvalidateRender();
                    }
                }
            }
        }

        /// <summary>
        /// Hide fake items.
        /// </summary>
        public virtual void HideFakeItems()
        {
            RemoveInternalFakeItems();
            InvalidateMeasure();
        }

        /// <summary>
        /// Gets index first no faks element.
        /// </summary>
        /// <param name="element">The element whose index position is required.</param>
        public int GetIndexNoFakeItems(UIElement element)
        {
            int index = -1;

            if (element != null)
            {
                if (FakeItems.Contains(element))
                {
                    int localIntex = -1;
                    UIElement localItem = null;
                    UIElement item = null;

                    for (int i = 0; i < InternalChildren.Count; i++)
                    {
                        item = InternalChildren[i];

                        if (FakeItems.Contains(item))
                        {
                            if (localIntex > -1)
                            {
                                localItem = InternalChildren[localIntex];
                            }
                            else if (i == 0)
                            {
                                localIntex = 0;
                            }

                            break;
                        }
                        else
                        {
                            localIntex++;
                        }
                    }

                    if (localItem != null)
                    {
                        index = ParentItemsControl.ItemContainerGenerator.IndexFromContainer(localItem);
                    }
                    else
                    {
                        index = localIntex;
                    }
                }
                else
                {
                    index = ParentItemsControl.ItemContainerGenerator.IndexFromContainer(element);
                }
            }

            return index;
        }

        #endregion Public Methods

        #region Implementation

        /// <summary >
        /// Raises the Initialized event. This method is invoked whenever IsInitialized
        /// is set to true internally.
        /// </summary>
        /// <param name="e">The RoutedEventArgs that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            m_parentItemsControl = ItemsControl.GetItemsOwner(this);
            parentItemsControl = ItemsControl.GetItemsOwner(this);

            if ((m_parentItemsControl as TreeViewItemAdv) != null)
            {
                (m_parentItemsControl as TreeViewItemAdv).m_virtualizingpanel = this as VirtualizingPanel;
                (m_parentItemsControl as TreeViewItemAdv).m_fakeItemsPanel = this;
            }
            if ((m_parentItemsControl as TreeViewAdv) != null)
            {
                (m_parentItemsControl as TreeViewAdv).m_virtualizingpanel = this as VirtualizingPanel;
                (m_parentItemsControl as TreeViewAdv).m_fakeItemsPanel = this;
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
            if ((ParentTreeView != null && ParentTreeView.IsLoaded && ParentTreeView.VirtualizationMode == VirtualizationMode.Normal) || (OwnerTreeView != null && OwnerTreeView.VirtualizationMode == VirtualizationMode.Normal) || (ParentTreeView != null && !ParentTreeView.IsVirtualizing))
            {
                UpdateVisibilityIndex(availableSize);
                availableSize = base.MeasureOverride(availableSize);
                UIElementCollection children = InternalChildren;
                IItemContainerGenerator generator = this.ItemContainerGenerator;
                if (generator != null && ParentItemsControl != null)
                {
                    UIElement child = null;
                    bool isNewlyRealized = false;
                    GeneratorPosition startPos = generator.GeneratorPositionFromIndex(FirstVisibleItemIndex);
                    int childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;
                    try
                    {
                        using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
                        {
                            //SU I78477
                            //int count = 0;
                            //EU I78477
                            for (int itemIndex = FirstVisibleItemIndex; itemIndex <= LastVisibleItemIndex; ++itemIndex, ++childIndex)
                            {
                                child = generator.GenerateNext(out isNewlyRealized) as UIElement;

                                if (child != null)
                                {
                                    if (isNewlyRealized)
                                    {
                                        if (childIndex >= children.Count)
                                        {
                                            base.AddInternalChild(child);
                                        }
                                        else
                                        {
                                            base.InsertInternalChild(childIndex, child);
                                        }

                                        generator.PrepareItemContainer(child);
                                    }
                                    child.Measure(GetMeasureItemSize(child));
                                    TreeViewItemAdv item = child as TreeViewItemAdv;

                                    if (item.ParentTreeView != null && item.ParentTreeView.LinearList.ContainsKey(child))
                                    {
                                        if (item != null && item.CompleteHeaderElement != null)
                                        {
                                            if (item.ParentTreeView.LinearList[item] != item.CompleteHeaderElement.DesiredSize.Height)
                                            {
                                                item.ParentTreeView.LinearList[item] = item.CompleteHeaderElement.DesiredSize.Height;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //SU I78477
                    //catch(Exception e)
                    catch (Exception)
                    //EU I78477
                    {
                    }
                    if (OwnerTreeView.IsVirtualizing)
                    {
                        if (OwnerTreeView.scrollToHome)
                        {
                            TreeViewItemAdv tviewitem = OwnerTreeView.ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItemAdv;
                            if (tviewitem == null)
                            {
                                tviewitem = OwnerTreeView.Items[0] as TreeViewItemAdv;
                            }
                            if (tviewitem != null)
                            {
                                tviewitem.Focus();
                                tviewitem.IsSelected = true;
                                OwnerTreeView.scrollToHome = false;
                            }
                        }
                        else if (OwnerTreeView.scrollToEnd)
                        {
                            TreeViewItemAdv tviewitem = OwnerTreeView.ItemContainerGenerator.ContainerFromIndex(OwnerTreeView.Items.Count - 1) as TreeViewItemAdv;
                            if (tviewitem == null)
                            {
                                if (OwnerTreeView.Items.Count > 0)
                                {
                                    tviewitem = OwnerTreeView.Items[OwnerTreeView.Items.Count - 1] as TreeViewItemAdv;
                                }
                            }
                            TreeViewItemAdv lastItem = LastItemIterate(tviewitem);
                            if (lastItem != null)
                            {
                                lastItem.Focus();
                                lastItem.IsSelected = true;
                                OwnerTreeView.scrollToEnd = false;
                            }
                        }
                    }
                }

                CleanItems();
                availableSize = GetPanelSize();
                return availableSize;
            }
            else
            {
                return VirtualMeasure(availableSize);
            }
        }

        internal TreeViewItemAdv LastItemIterate(TreeViewItemAdv tvitem)
        {
            if (tvitem != null)
            {
                if (tvitem.IsExpanded)
                {
                    TreeViewItemAdv tviewitem = tvitem.ItemContainerGenerator.ContainerFromIndex(tvitem.Items.Count - 1) as TreeViewItemAdv;
                    if (tviewitem == null)
                    {
                        if (tvitem.Items.Count > 0)
                        {
                            tviewitem = tvitem.Items[tvitem.Items.Count - 1] as TreeViewItemAdv;
                        }
                    }
                    LastItemIterate(tviewitem);
                }
                else
                    return tvitem;
            }
            return null;
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
            if ((ParentTreeView != null && ParentTreeView.IsLoaded && ParentTreeView.VirtualizationMode == VirtualizationMode.Normal) || (OwnerTreeView != null && OwnerTreeView.VirtualizationMode == VirtualizationMode.Normal))
            {
                finalSize = base.ArrangeOverride(finalSize);
            }
            UIElementCollection children = this.InternalChildren;
            Size itemSize = new Size(0, 0);
            double position = ArrangeOffset;
            try
            {
                if (children != null)
                {
                    UIElement item = null;

                    for (int i = 0; i < VisualChildrenCount; i++)
                    {
                        item = children[i] as UIElement;

                        if (item != null)
                        {
                            itemSize = GetArrangeItemSize(item);
                            itemSize.Width = Math.Max(finalSize.Width, itemSize.Width);
                            item.Arrange(new Rect(0, position, itemSize.Width, itemSize.Height));
                            position += itemSize.Height;
                        }
                    }
                }
            }
            //SU I78477
            //catch(Exception e)
            catch (Exception)
            //EU I78477
            {
            }
            return finalSize;
        }

        /// <summary>
        /// Update visible item index.
        /// </summary>
        protected virtual void UpdateVisibilityIndex(Size availableSize)
        {
            FirstVisibleItemIndex = 0;
            LastVisibleItemIndex = (ParentItemsControl == null) ? 0 : ParentItemsControl.Items.Count;
            ArrangeOffset = 0;
        }

        /// <summary>
        /// Revirtualize items that are no longer visible.
        /// </summary>
        protected virtual void CleanItems()
        {
            if (FirstVisibleItemIndex > -1 && LastVisibleItemIndex > -1)
            {
                UIElementCollection children = InternalChildren;
                IItemContainerGenerator generator = ItemContainerGenerator;
                if (ParentItemsControl is TreeViewAdv)
                {
                    for (int i = children.Count - 1 - FakeItems.Count; i >= 0; i--)
                    {
                        GeneratorPosition childGeneratorPos = new GeneratorPosition(i, 0);
                        int itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);

                        if (itemIndex > -1 && itemIndex < FirstVisibleItemIndex || itemIndex > LastVisibleItemIndex)
                        {
                            generator.Remove(childGeneratorPos, 1);
                            RemoveInternalChildRange(i, 1);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets size of the this panel.
        /// </summary>
        protected virtual Size GetPanelSize()
        {
            Size panelSize = new Size(0, 0);

            if (ParentItemsControl != null)
            {
                Size itemSize = new Size(0, 0);

                for (int i = 0; i < CountInternalItems; i++)
                {
                    itemSize = GetInternalItemSize(i);
                    panelSize.Height += itemSize.Height;

                    if (panelSize.Width < itemSize.Width)
                    {
                        panelSize.Width = itemSize.Width;
                    }
                }
            }

            return panelSize;
        }

        /// <summary>
        /// Gets appreciate complete size of the this panel.
        /// </summary>
        protected virtual Size GetCompletePanelSize(Size availableSize)
        {
            Size panelSize = new Size(availableSize.Width, 0);

            if (FirstVisibleItemIndex != -1 && LastVisibleItemIndex != -1)
            {
                int itemsCount = ParentItemsControl.Items.Count;
                Size itemSize = new Size(0, 0);

                for (int index = 0; index < itemsCount; index++)
                {
                    itemSize = GetCompleteItemSize(index);

                    if (itemSize != Size.Empty)
                    {
                        panelSize.Height += itemSize.Height;

                        if (panelSize.Width < itemSize.Width)
                        {
                            panelSize.Width = itemSize.Width;
                        }
                    }
                }
            }

            return panelSize;
        }

        /// <summary>
        /// Gets appreciate complete size for item.
        /// </summary>
        protected virtual Size GetCompleteItemSize(int index)
        {
            return GetActualItemSize(index);
        }

        /// <summary>
        /// Gets appreciate complete size for unvisible item.
        /// </summary>
        protected virtual Size GetComplateUnvisibleItemSize(int index)
        {
            return GetActualItemSize(index);
        }

        /// <summary>
        /// Gets occupy height of the ScrollViewer.
        /// </summary>
        protected virtual double GetOccupyHeight()
        {
            return 0d;
        }

        /// <summary>
        /// Gets top vertical offset.
        /// </summary>
        protected virtual double GetTopOffset()
        {
            return 0d;
        }

        /// <summary>
        /// Gets available height.
        /// </summary>
        protected virtual double GetAvailableHeight(Size availableSize)
        {
            return 0d;
        }

        /// <summary>
        /// Gets available width.
        /// </summary>
        protected virtual double GetAvailableWidth()
        {
            return 0d;
        }

        /// <summary>
        /// Gets size of the child item by index.
        /// </summary>
        protected virtual Size GetInternalItemSize(int index)
        {
            Size itemSize = new Size(0, 0);

            if (ParentItemsControl != null && index > -1 && index < CountInternalItems)
            {
                UIElement element = GetInternalItem(index);

                if (element != null)
                {
                    itemSize = element.DesiredSize;
                }
            }

            return itemSize;
        }

        /// <summary>
        /// Gets desired size for UIElement when size isn't determine.
        /// </summary>
        internal virtual Size GetActualItemSize(int index)
        {
            Size itemSize = new Size(0, 0);
            Size cashSize = Size.Empty;
            bool bIsSetCash = false;

            if (index > -1)
            {
                UIElement item = GetItem(index) as UIElement;

                object key = GetKey(index);
                item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                itemSize = item.DesiredSize;

                if (!m_cashedMeasureSize.ContainsKey(key) && !itemSize.IsEmpty && itemSize.Height > 0)
                {
                    bIsSetCash = true;
                    cashSize = itemSize;
                }

                if (bIsSetCash)
                {
                    SetCashedMeasureSize(key, cashSize);
                }
            }

            return itemSize;
        }

        /// <summary>
        /// Gets item by index from items and fake items.
        /// </summary>
        protected internal UIElement GetInternalItem(int index)
        {
            UIElement element = null;

            if (ParentItemsControl != null && index > -1 && index < CountInternalItems)
            {
                element = InternalChildren[index];
            }

            return element;
        }

        /// <summary>
        /// Gets item by index from items and fake items.
        /// </summary>
        protected internal int GetInternalIndex(UIElement element)
        {
            int index = -1;

            if (element != null)
            {
                index = InternalChildren.IndexOf(element);
            }

            return index;
        }

        /// <summary>
        /// Gets size of the item for MeasureOverride method.
        /// </summary>
        protected virtual Size GetMeasureItemSize(UIElement element)
        {
            return new Size(double.PositiveInfinity, double.PositiveInfinity);
        }

        /// <summary>
        /// Gets size of the item for ArrangeOverride method.
        /// </summary>
        protected virtual Size GetArrangeItemSize(UIElement element)
        {
            Size size = new Size(0, 0);

            if (element != null)
            {
                size = element.DesiredSize;
            }

            return size;
        }

        /// <summary>
        /// Gets type of the fake element.
        /// </summary>
        protected virtual Type GetFakeElementType()
        {
            return typeof(UIElement);
        }

        /// <summary>
        /// Gets container for fake element.
        /// </summary>
        protected virtual UIElement GetContainerForFakeElement(object element)
        {
            UIElement container = null;

            if (element != null)
            {
                if (IsNeedCreateContainer(element))
                {
                    ContentPresenter presenter = new ContentPresenter();
                    presenter.Content = element;
                    container = presenter;
                }
                else
                {
                    container = element as UIElement;
                }
            }

            return container;
        }

        /// <summary>
        /// Called when the FakeItems collection is changed.
        /// </summary>
        protected virtual void OnFakeItemsChanged(UIElement element)
        {
        }

        /// <summary>
        /// Gets value indicating whether need creates new container.
        /// </summary>
        protected bool IsNeedCreateContainer(object element)
        {
            bool need = false;

            if (element != null && !element.GetType().Equals(GetFakeElementType()))
            {
                need = true;
            }

            return need;
        }

        /// <summary>
        /// Revirtualize all items.
        /// </summary>
        protected internal void ClearAllItems()
        {
            UIElementCollection children = this.InternalChildren;
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            if (generator != null && children != null)
            {
                generator.RemoveAll();

                for (int i = children.Count - 1; i >= 0; i--)
                {
                    RemoveInternalChildRange(i, 1);
                }
            }
        }

        /// <summary>
        /// Gets item from owner items control by index.
        /// </summary>
        protected virtual UIElement GetItem(int index)
        {
            UIElement item = null;

            if (ParentItemsControl != null &&
                index > -1 && index < ParentItemsControl.Items.Count)
            {
                item = ParentItemsControl.Items[index] as UIElement;

                if (item == null)
                {
                    item = ParentItemsControl.ItemContainerGenerator.ContainerFromIndex(index) as UIElement;
                }
            }

            return item;
        }

        /// <summary>
        /// Sets measured size of the items to hashtable.
        /// </summary>
        protected void SetCashedMeasureSize(object key, Size value)
        {
            if (!value.IsEmpty && value.Height > 0 && value.Width > 0)
            {
                if (m_cashedMeasureSize.ContainsKey(key))
                {
                    m_cashedMeasureSize[key] = value;
                }
                else
                {
                    m_cashedMeasureSize.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Gets measured size of the items from hashtable.
        /// </summary>
        protected Size GetCashedMeasureSize(object key)
        {
            if (m_cashedMeasureSize.ContainsKey(key))
            {
                return (Size)m_cashedMeasureSize[key];
            }
            else
            {
                return Size.Empty;
            }
        }

        /// <summary>
        /// Gets key for hashtable.
        /// </summary>
        protected object GetKey(int index)
        {
            object key = null;

            if (ParentItemsControl != null
                && index > -1 && index < ParentItemsControl.Items.Count)
            {
                key = ParentItemsControl.Items[index];
            }

            return key;
        }

        /// <summary>
        /// Remove all fake items from InternalItems collection.
        /// </summary>
        private void RemoveInternalFakeItems()
        {
            //m_lastIndexFakeItems = -1;

            if (FakeItems.Count > 0)
            {
                for (int i = 0; i < FakeItems.Count; i++)
                {
                    int index = InternalChildren.IndexOf(FakeItems[i]);

                    if (index > -1)
                    {
                        base.RemoveInternalChildRange(index, 1);
                    }
                }
            }
        }

        #endregion Implementation

        #region IVirtualTree

        /// <summary>
        /// MeasureOverride calculations on Extended VirtualizationMode.
        /// </summary>
        private Size VirtualMeasure(Size availableSize)
        {
        
            if (ParentItemsControl is TreeViewAdv)
            {
                CalculateVisibleRange();
            }
            else if (ParentItemsControl is TreeViewItemAdv)
            {
                CalculateItemVisibleRange();
            }

            UIElementCollection uielementCollection = base.InternalChildren;
            IItemContainerGenerator generator = this.ItemContainerGenerator;
            ItemsControl itemsControl = parentItemsControl;
            GeneratorPosition startPos = generator.GeneratorPositionFromIndex(firstVisibleIndex);
            childrenSize = new Size();

            int childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;
            TreeViewAdv parentTreeView = ParentItemsControl as TreeViewAdv;

            if (parentTreeView != null && parentTreeView.m_scrollinfo.ViewportHeight + parentTreeView.m_scrollinfo.VerticalOffset == parentTreeView.ExtentHeight && parentTreeView.isPageDown)
            {
                childIndex = firstVisibleIndex;
            }

            using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
            {
                for (int itemIndex = firstVisibleIndex; itemIndex <= parentItemsControl.Items.Count; ++itemIndex, ++childIndex)
                {
                    bool newlyRealized;

                    // Get or create the child
                    UIElement child = generator.GenerateNext(out newlyRealized) as UIElement;
                    if (child != null)
                    {
                        if (newlyRealized)
                        {
                            // Figure out if we need to insert the child at the end or somewhere in the middle
                            if (childIndex >= base.InternalChildren.Count)
                            {
                                base.AddInternalChild(child);
                            }
                            else
                            {
                                base.InsertInternalChild(childIndex, child);
                            }
                            generator.PrepareItemContainer(child);
                            if (!((TreeViewItemAdv)child).IsExpanded && ((FrameworkElement)child).DataContext is IVirtualTree)
                                ((TreeViewItemAdv)child).IsExpanded = (((IVirtualTree)((FrameworkElement)child).DataContext)).IsExpanded;
                            TreeViewItemAdv tadv = child as TreeViewItemAdv;
                            if (tadv != null && OwnerTreeView.scrollToEnd)
                            {
                                if (tadv.ParentTreeView != null)
                                {
                                    if (!tadv.IsExpanded)
                                    {
                                        if ((tadv.ParentTreeView.Items[tadv.ParentTreeView.Items.Count - 1] as IVirtualTree) == tadv.DataContext)
                                        {
                                            tadv.Focus();
                                            tadv.IsSelected = true;
                                            OwnerTreeView.scrollToEnd = false;
                                        }
                                    }
                                }
                                if (tadv.ParentTreeViewItem != null)
                                {
                                    if (!tadv.IsExpanded && tadv.ParentTreeViewItem.IsExpanded)
                                    {
                                        if ((tadv.ParentTreeViewItem.Items[tadv.ParentTreeViewItem.Items.Count - 1] as IVirtualTree) == tadv.DataContext)
                                        {
                                            tadv.Focus();
                                            tadv.IsSelected = true;
                                            OwnerTreeView.scrollToEnd = false;
                                        }
                                    }
                                }
                            }
                            else if (tadv != null && OwnerTreeView.scrollToHome)
                            {
                                if (tadv.ParentTreeView != null)
                                {
                                    tadv.Focus();
                                    tadv.IsSelected = true;
                                    OwnerTreeView.scrollToHome = false;
                                }
                            }
                        }
                        else
                        {
                            // The child has already been created, let's be sure it's in the right spot
                            //Debug.Assert(child == base.InternalChildren[childIndex], "Wrong child was generated");
                        }

                        // Measurements will depend on layout algorithm
                        if (child.DesiredSize.Height == double.NaN || child.DesiredSize.Height == 0.0)
                        {
                            ParentTreeView.isCalledByChildMeasure = true;
                            child.Measure(availableSize);
                            ParentTreeView.isCalledByChildMeasure = false;
                        }

                        TreeViewItemAdv tvAdv = (child as TreeViewItemAdv);
                        if (ParentTreeView.treeHeight == 0.0)
                        {
                            ParentTreeView.treeHeight = child.DesiredSize.Height;
                        }
                        if (tvAdv.CompleteHeaderElement != null && tvAdv.CompleteHeaderElement.DesiredSize.Height > ParentTreeView.treeHeight)
                        {
                            ParentTreeView.treeHeight = tvAdv.CompleteHeaderElement.DesiredSize.Height;
                            ParentTreeView.ExtentHeight = ParentTreeView.Items.Count * ParentTreeView.treeHeight;
                        }
                        TreeViewItemAdv childtreeitem = child as TreeViewItemAdv;
                        if (parentItemsControl is TreeViewAdv)
                        {
                            if (childtreeitem.IsExpanded && child.DesiredSize.Height == ParentTreeView.treeHeight && childtreeitem.DataContext is IVirtualTree)
                            {
                                childrenSize.Height += ((IVirtualTree)childtreeitem.DataContext).ExtentHeight;
                            }
                            else
                                childrenSize.Height += child.DesiredSize.Height;
                            if (ParentTreeView.m_scrollinfo.ViewportHeight + ParentTreeView.m_scrollinfo.VerticalOffset == ParentTreeView.ExtentHeight && ParentTreeView.isPageDown)
                            {
                                if (childIndex == ParentTreeView.Items.Count - 1)
                                {
                                    lastVisibleIndex = ParentTreeView.Items.Count - 1;
                                    break;
                                }
                                else
                                    continue;
                            }
                            else
                                lastVisibleIndex = itemIndex;
                            if (child.DesiredSize.Height == ParentTreeView.m_scrollinfo.ViewportHeight)
                                break;
                            if ((childrenSize.Height - Math.Abs(ParentTreeView.m_treeviewadvVirtualizingPanel.m_transform.Y)) > (ParentTreeView.m_scrollinfo.ViewportHeight + ParentTreeView.treeHeight))
                                break;
                        }
                        else if (parentItemsControl is TreeViewItemAdv && (parentItemsControl as TreeViewItemAdv).IsLoaded)
                        {
                            Point treeItemPoint = parentItemsControl.TransformToVisual(ParentTreeView.m_fakeItemsPanel).Transform(new Point(0, 0));
                            double availableHeight = 0d;

                            availableHeight = (ParentTreeView.m_fakeItemsPanel.ActualHeight - treeItemPoint.Y) - ParentTreeView.treeHeight;
                            if (availableHeight <= 0)
                                availableHeight = ParentTreeView.m_fakeItemsPanel.ActualHeight;

                            childrenSize.Height += child.DesiredSize.Height;
                            lastVisibleIndex = itemIndex;

                            if (ParentTreeView.ExpandedItems.Count > 0)
                            {
                                if ((childrenSize.Height - Math.Abs(ParentTreeView.m_treeviewadvVirtualizingPanel.m_transform.Y)) > availableHeight + ParentTreeView.treeHeight && OwnerTreeView.Items.Contains(ParentTreeView.ExpandedItems[0]))
                                    break;
                            }
                            else
                            {
                                if ((childrenSize.Height - Math.Abs(ParentTreeView.m_treeviewadvVirtualizingPanel.m_transform.Y)) > availableHeight + ParentTreeView.treeHeight)
                                    break;
                            }
                        }
                    }
                    else
                        break;
                }
            }

            CleanUpItems(firstVisibleIndex, lastVisibleIndex);

            foreach (UIElement element in InternalChildren)
            {
                TreeViewItemAdv item = element as TreeViewItemAdv;
                if (item != null && item.IsExpanded)
                {
                    //Need to trigger the Measure pass of sub items here.
                    if (item.m_fakeItemsPanel != null)
                    {
                        item.m_fakeItemsPanel.InvalidateMeasure();
                    }
                }
            }

            //if (ParentTreeView != null && ParentTreeView.m_treeviewadvVirtualizingPanel != null)
            //{
            //   ParentTreeView.m_treeviewadvVirtualizingPanel.UpdateScrollInfo(availableSize);
            //}

            if (parentItemsControl is TreeViewAdv && !(double.IsInfinity(availableSize.Height) || double.IsInfinity(availableSize.Width)))
            {
                return availableSize;
            }
            else
            {
                return childrenSize;
            }
        }

        /// <summary>
        /// Cleans the Container of the Items that not in  Viewport.
        /// </summary>
        private void CleanUpItems(int firstVisibleItemIndex, int lastVisibleItemIndex)
        {
            UIElementCollection children = this.InternalChildren;
            IItemContainerGenerator generator = this.ItemContainerGenerator;

            for (int i = children.Count - 1; i >= 0; i--)
            {
                // Map a child index to an item index by going through a generator position

                GeneratorPosition childGeneratorPos = new GeneratorPosition(i, 0);
                int itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);

                if (itemIndex < firstVisibleItemIndex || itemIndex > lastVisibleItemIndex)
                {
                    generator.Remove(childGeneratorPos, 1);
                    RemoveInternalChildRange(i, 1);
                }
            }
        }

        /// <summary>
        /// FirstVisibleIndex Calculation for Root TreeViewAdv.
        /// </summary>
        private void CalculateVisibleRange()
        {
            if (ParentTreeView.treeHeight != 0)
            {
                if ((Convert.ToInt32(ParentTreeView.m_scrollinfo.VerticalOffset / ParentTreeView.treeHeight)) != 0 && !ParentTreeView.isSourceChanged)
                    firstVisibleIndex = (Convert.ToInt32(ParentTreeView.m_scrollinfo.VerticalOffset / ParentTreeView.treeHeight)) - 1;
                else
                {
                    firstVisibleIndex = (Convert.ToInt32(ParentTreeView.m_scrollinfo.VerticalOffset / ParentTreeView.treeHeight));
                    if (ParentTreeView.isSourceChanged)
                        ParentTreeView.isSourceChanged = false;
                }
            }
            if (ParentTreeView.expandedItemsCount > 0)
            {
                double treeHeight = 0.0;

                for (int i = 0; i < parentItemsControl.Items.Count; i++)
                {
                    firstVisibleIndex = i;
                    treeHeight += ParentTreeView.treeHeight + (((IVirtualTree)parentItemsControl.Items[i]).ExtentHeight);
        
                    if (OwnerTreeView.scrollToHome)
                    {
                        ParentTreeView.m_scrollinfo.SetVerticalOffset(0.0);
                    }
                    else if (OwnerTreeView.scrollToEnd)
                    {
                        ParentTreeView.m_scrollinfo.SetVerticalOffset(ParentTreeView.m_scrollinfo.ExtentHeight - ParentTreeView.m_scrollinfo.ViewportHeight);
                    }

                    if (treeHeight > ParentTreeView.m_scrollinfo.VerticalOffset)
                    {
                        TreeViewItemAdv item = parentItemsControl.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;

                        if (item == null)
                        {
                            if (((IVirtualTree)ParentTreeView.Items[i]).IsExpanded)
                            {
                                ParentTreeView.m_treeviewadvVirtualizingPanel.m_transform.Y -= ParentTreeView.treeHeight;
                            }
                        }

                        if (item != null && item.IsExpanded && item.TransformToVisual(ParentTreeView).Transform(new Point(0, 0)).Y < 0)
                        {
                            ParentTreeView.m_treeviewadvVirtualizingPanel.m_transform.Y += ParentTreeView.treeHeight;
                        }
                        if (item != null)
                        {
                            double yValue = Math.Ceiling(item.TransformToVisual(ParentTreeView).Transform(new Point(0, 0)).Y);

                            if ((yValue >= 1.0d) && item.IsExpanded && ParentTreeView.m_scrollinfo.VerticalOffset != 0.0)
                            {
                                ParentTreeView.m_treeviewadvVirtualizingPanel.m_transform.Y -= ParentTreeView.treeHeight;
                                if ((ParentTreeView.m_scrollinfo.VerticalOffset - ParentTreeView.treeHeight) <= Math.Ceiling(treeHeight - (ParentTreeView.treeHeight + (((IVirtualTree)parentItemsControl.Items[i]).ExtentHeight))))
                                {
                                    ParentTreeView.m_treeviewadvVirtualizingPanel.m_transform.Y += ParentTreeView.treeHeight;
                                }
                            }
                        }

                        break;
                    }
                }
            }
            int x = ParentTreeView.Items.Count - 1;
            TreeViewItemAdv item1 = parentItemsControl.ItemContainerGenerator.ContainerFromIndex(x) as TreeViewItemAdv;

            if (OwnerTreeView.scrollToHome && OwnerTreeView.scrollToEnd || (item1 != null && !item1.IsExpanded && IsLastItemExpanded && !OwnerTreeView.scrollToHome && !OwnerTreeView.scrollToEnd))
            {
                IsLastItemExpanded = false;
                int ViewportVisibleCount = Convert.ToInt32(ParentTreeView.m_scrollinfo.ViewportHeight / ParentTreeView.treeHeight);
                if (ParentTreeView.Items.Count > ViewportVisibleCount)
                    firstVisibleIndex = ParentTreeView.Items.Count - ViewportVisibleCount;
            }
            if (item1 != null && item1.IsExpanded)
            {
                IsLastItemExpanded = true;
            }
            if (ParentTreeView.Items.Count > 0 && firstVisibleIndex >= 0 && firstVisibleIndex < ParentTreeView.Items.Count)
            {
                IVirtualTree virtualTree = ParentTreeView.Items[firstVisibleIndex] as IVirtualTree;
                if (virtualTree != null && !virtualTree.IsExpanded && ParentTreeView.m_treeviewadvVirtualizingPanel.m_transform.Y < 0)
                {
                    ParentTreeView.m_treeviewadvVirtualizingPanel.m_transform.Y += ParentTreeView.treeHeight;
                }
            }
        }

        /// <summary>
        /// FirstVisibleIndex calculation for the TreeViewItems.
        /// </summary>
        private void CalculateItemVisibleRange()
        {
            double topPoint = parentItemsControl.TransformToVisual(ParentTreeView).Transform(new Point(0, 0)).Y;
            bool isEndOfViewport = false;
            if (ParentTreeView.m_scrollinfo.VerticalOffset + ParentTreeView.m_scrollinfo.ViewportHeight == ParentTreeView.ExtentHeight)
            {
                isEndOfViewport = true;
            }
            if (topPoint >= 0 && topPoint != 3.0 && !isEndOfViewport)
            {
                firstVisibleIndex = 0;
                return;
            }
            else
            {
                double totalheight = 0.0;
                TreeViewItemAdv treeviewItem = null, parentTreeViewItem = null;
                Visual visual = parentItemsControl as Visual;
                treeviewItem = parentItemsControl as TreeViewItemAdv;
                if (treeviewItem != null && treeviewItem.DataContext is IVirtualTree)
                {
                    do
                    {
                        parentTreeViewItem = VisualUtils.FindAncestor(treeviewItem, typeof(TreeViewItemAdv)) as TreeViewItemAdv;
                        if (parentTreeViewItem != null && treeviewItem != null)
                        {
                            for (int i = 0; i < parentTreeViewItem.Items.IndexOf(treeviewItem.DataContext); i++)
                            {
                                totalheight += ParentTreeView.treeHeight + (((IVirtualTree)parentTreeViewItem.Items[i]).ExtentHeight);
                            }
                        }
                        else if (parentTreeViewItem == null)
                        {
                            TreeViewAdv parentTreeView = VisualUtils.FindAncestor(treeviewItem, typeof(TreeViewAdv)) as TreeViewAdv;
                            if (parentTreeView != null)
                            {
                                for (int i = 0; i < parentTreeView.Items.IndexOf(treeviewItem.DataContext); i++)
                                {
                                    totalheight += parentTreeView.treeHeight + (((IVirtualTree)parentTreeView.Items[i]).ExtentHeight);
                                }
                            }
                        }
                        treeviewItem = VisualUtils.FindAncestor(visual, typeof(TreeViewItemAdv)) as TreeViewItemAdv;
                        totalheight += ParentTreeView.treeHeight;

                        visual = treeviewItem as Visual;
                    } while (treeviewItem != null);

                    for (int i = 0; i < parentItemsControl.Items.Count; i++)
                    {
                        totalheight += ParentTreeView.treeHeight + (((IVirtualTree)parentItemsControl.Items[i]).ExtentHeight);
                        firstVisibleIndex = i;
                        
                        if (totalheight > ParentTreeView.m_scrollinfo.VerticalOffset + Math.Abs(transform.Y))
                        {
                            TreeViewItemAdv item = parentItemsControl.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                            if (item == null)
                            {
                                if (((IVirtualTree)parentItemsControl.Items[i]).IsExpanded)
                                {
                                    ParentTreeView.m_treeviewadvVirtualizingPanel.m_transform.Y -= ParentTreeView.treeHeight;
                                }
                            }
                            if (item != null && item.IsExpanded && item.TransformToVisual(ParentTreeView).Transform(new Point(0, 0)).Y < 0)
                            {
                                ParentTreeView.m_treeviewadvVirtualizingPanel.m_transform.Y += ParentTreeView.treeHeight;
                            }
                            if (item != null && item.TransformToVisual(ParentTreeView).Transform(new Point(0, 0)).Y == 3.0d && item.IsExpanded && ParentTreeView.m_scrollinfo.VerticalOffset != 0.0)
                            {
                                ParentTreeView.m_treeviewadvVirtualizingPanel.m_transform.Y -= ParentTreeView.treeHeight;
                                if (ParentTreeView.ScrollHost.VerticalOffset == Math.Ceiling(totalheight - (ParentTreeView.treeHeight + (((IVirtualTree)parentItemsControl.Items[i]).ExtentHeight))))
                                {
                                    ParentTreeView.m_treeviewadvVirtualizingPanel.m_transform.Y += ParentTreeView.treeHeight;
                                }
                            }
                            else if (ParentTreeView.m_treeviewadvVirtualizingPanel.IsWindowResized)
                            {
                                ParentTreeView.m_treeviewadvVirtualizingPanel.m_transform.Y -= 18.5;
                                if (Math.Round(ParentTreeView.ScrollHost.VerticalOffset) - (Math.Round(totalheight - (ParentTreeView.treeHeight + (((IVirtualTree)parentItemsControl.Items[i]).ExtentHeight)))) > 14.0d)
                                {
                                    ParentTreeView.m_treeviewadvVirtualizingPanel.m_transform.Y -= Math.Round(ParentTreeView.ScrollHost.VerticalOffset) - (Math.Round(totalheight - (ParentTreeView.treeHeight + (((IVirtualTree)parentItemsControl.Items[i]).ExtentHeight))));
                                }
                                if (ParentTreeView.ScrollHost.VerticalOffset == Math.Ceiling(totalheight - (ParentTreeView.treeHeight + (((IVirtualTree)parentItemsControl.Items[i]).ExtentHeight))))
                                {
                                    ParentTreeView.m_treeviewadvVirtualizingPanel.m_transform.Y += ParentTreeView.treeHeight;
                                }
                                ParentTreeView.m_treeviewadvVirtualizingPanel.IsWindowResized = false;
                            }
                            break;
                        }
                    }
                }
            }
        }

        #endregion IVirtualTree
    }
}